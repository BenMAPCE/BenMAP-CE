using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Topology.Voronoi;
using FirebirdSql.Data.FirebirdClient;
using ProtoBuf;

using LumenWorks.Framework.IO.Csv;
using System.Reflection;

namespace BenMAP
{
    public class DataSourceCommonClass
    {
        public static Dictionary<string, string> _dicSeasonStaticsAll;

        public static Dictionary<string, string> DicSeasonStaticsAll
        {
            get {
                if (_dicSeasonStaticsAll == null)
                {
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    string commandText = string.Format("select * from SEASONALMETRICSEASONS");
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    _dicSeasonStaticsAll = new Dictionary<string, string>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        _dicSeasonStaticsAll.Add(dr["StartDay"].ToString()+"," + dr["SeasonalMetricID"].ToString(), dr["METRICFUNCTION"].ToString());
                    }
 
                }
                return _dicSeasonStaticsAll; }
            set { _dicSeasonStaticsAll = value; }
        }
        /// <summary>
        /// 根据各种情况生成模型值，并且生成SHP
        /// </summary>
        /// <param name="benMAPGrid"></param>
        /// <param name="benMAPLine"></param>
        public static void UpdateModelValues(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, BenMAPLine benMAPLine)
        {
            string s = "";
            s.ToString();
        }
        public static System.Data.DataSet getDataSetFromCSV(string strPath)
        {
            //StreamReader sr = new StreamReader(strPath);
            //string csvDataLine;

            //csvDataLine = "";


            //DataTable dt = new DataTable();
            //DataRow dr = null;
            //string strLine = "";
            //string[] strArray;
            //strLine = sr.ReadLine();
            //if (strLine != null && strLine.Length > 0)
            //{
            //    strArray = strLine.Split(',');
            //    for (int i = 0; i < strArray.Count(); i++)
            //    {
            //        dt.Columns.Add(strArray[i]);
            //    }

            //    //Debug.WriteLine(strLine);
            //}
            //while (strLine != null)
            //{
            //    strLine = sr.ReadLine();
            //    if (strLine != null && strLine.Length > 0)
            //    {
            //        dr = dt.NewRow();
            //        strArray = strLine.Split(',');
            //        for (int i = 0; i < strArray.Count(); i++)
            //        {
            //            dr[i] = strArray[i];
            //        }
            //        dt.Rows.Add(dr);
            //        //Debug.WriteLine(strLine);
            //    }
            //}
            System.Data.DataSet ds = new System.Data.DataSet();
            //ds.Tables.Add(dt);

            System.Data.DataTable dt = new DataTable();
            using (CsvReader csv =
           new CsvReader(new StreamReader(strPath), true))
            {
                int fieldCount = csv.FieldCount;
                string[] headers = csv.GetFieldHeaders();
                foreach (string s in headers)
                {
                    dt.Columns.Add(s);

                }
                while (csv.ReadNextRecord())
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < fieldCount; i++)
                    {

                        dr[i] = csv[i];

                        //Console.Write(string.Format("{0} = {1};",
                        //              headers[i], csv[i]));
                        //Console.WriteLine();
                    }
                    dt.Rows.Add(dr);
                }
                ds.Tables.Add(dt);
            }
            //sr.Dispose();
            return ds;
        }
        public static System.Data.DataTable getDataTableFromCSV(string strPath)
        {
            //StreamReader sr = new StreamReader(strPath);
             

            

            
            //DataTable dt = new DataTable();
            //DataRow dr = null;
            //string strLine = "";
            //string[] strArray;
            //strLine = sr.ReadLine();
            //if (strLine != null && strLine.Length > 0)
            //{
            //    strArray = strLine.Split(',');
            //    for (int i = 0; i < strArray.Count(); i++)
            //    {
            //        dt.Columns.Add(strArray[i]);
            //    }

            //    //Debug.WriteLine(strLine);
            //}
            //while (strLine != null)
            //{
            //    strLine = sr.ReadLine();
            //    if (strLine != null && strLine.Length > 0)
            //    {
            //        dr = dt.NewRow();
            //        strArray = strLine.Split(',');
            //        for (int i = 0; i < strArray.Count(); i++)
            //        {
            //            dr[i] = strArray[i];
            //        }
            //        dt.Rows.Add(dr);
            //        //Debug.WriteLine(strLine);
            //    }
            //}
            //System.Data.DataSet ds = new System.Data.DataSet();
            //ds.Tables.Add(dt);
            FileStream stream = File.Open(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            System.Data.DataTable dt = new DataTable();
            using (CsvReader csv =
           new CsvReader(new StreamReader(stream), true))
            {
                int fieldCount = csv.FieldCount;

                string[] headers = csv.GetFieldHeaders();
                foreach (string s in headers)
                {
                    dt.Columns.Add(s);

                }
                while (csv.ReadNextRecord())
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < fieldCount; i++)
                    {

                        dr[i] = csv[i];

                        //Console.Write(string.Format("{0} = {1};",
                        //              headers[i], csv[i]));
                        //Console.WriteLine();
                    }
                    dt.Rows.Add(dr);
                }
                //ds.Tables.Add(dt);
            }
            //sr.Dispose();
            return dt;
        }
        /// <summary>
        /// 从DataSet中读取所有的Model值
        /// </summary>
        /// <param name="modelDataLine"></param>
        /// <summary>
        /// 从DataSet中读取所有的Model值
        /// </summary>
        /// <param name="modelDataLine"></param>
        public static void UpdateModelDataLineFromDataSet(BenMAPPollutant benMAPPollutant, ModelDataLine modelDataLine, System.Data.DataTable dtModel)
        {
            //直接默认为：Column	Row	Metric	Seasonal Metric	Annual Metric	Values
            try
            {
                modelDataLine.Pollutant = benMAPPollutant;
                modelDataLine.ModelAttributes = new List<ModelAttribute>();
                int iCol = 0;
                int iRow = 0;
                int iMetric = 0;
                int iSeasonalMetric = 0;
                int iStatistic = 0;
                int iValue = 0;
                int i = 0;
                foreach (DataColumn dc in dtModel.Columns)
                {
                    if (dc.ColumnName.ToLower().Trim().Replace(" ", "") == "seasonalmetric")
                    {
                        iSeasonalMetric = i;
                    }
                    else if (dc.ColumnName.ToLower().Trim().Replace(" ", "") == "statistic" || dc.ColumnName.ToLower().Trim().Replace(" ", "") == "annualmetric")
                    {
                        iStatistic = i;
                    }
                    else if (dc.ColumnName.ToLower().Trim() == "values")
                    {
                        iValue = i;
                    }
                    else if (dc.ColumnName.ToLower().Trim() == "metric")
                    {
                        iMetric = i;
                    }
                    else if (dc.ColumnName.ToLower().Trim() == "column" || dc.ColumnName.ToLower().Trim() == "col")
                    {
                        iCol = i;
                    }
                    else if (dc.ColumnName.ToLower().Trim() == "row")
                    {
                        iRow = i;
                    }
                    i++;
                }
                Dictionary<string, SeasonalMetric> dicSeasonalMetric = new Dictionary<string, SeasonalMetric>();
                if (benMAPPollutant.SesonalMetrics != null)
                {
                    foreach (SeasonalMetric m in benMAPPollutant.SesonalMetrics)
                    {
                        dicSeasonalMetric.Add(m.SeasonalMetricName.ToLower(), m);
                    }
                }
                Dictionary<string, Metric> dicMetric = new Dictionary<string, Metric>();
                foreach (Metric m in benMAPPollutant.Metrics)
                {
                    dicMetric.Add(m.MetricName.ToLower(), m);
                }
                foreach (DataRow drModel in dtModel.Rows)
                {
                    ModelAttribute modelAttribute = new ModelAttribute();
                    modelAttribute.Col = Convert.ToInt32(drModel[iCol]);
                    modelAttribute.Row = Convert.ToInt32(drModel[iRow]);
                    //modelAttribute.Metric
                    if (drModel[iMetric] == null || drModel[iMetric].ToString() == "")
                        modelAttribute.Metric = null;
                    else
                    {
                        //var queryMetric = from a in benMAPPollutant.Metrics where a.MetricName.ToLower() == drModel[iMetric].ToString().ToLower() select a;
                        //if (queryMetric != null && queryMetric.Count() > 0) modelAttribute.Metric = queryMetric.First();
                        if(dicMetric.ContainsKey(drModel[iMetric].ToString().ToLower() ))
                        {
                            modelAttribute.Metric = dicMetric[drModel[iMetric].ToString().ToLower()];
                        }

                        //commandText = string.Format("select MetricID from Metrics where MetricName='{0}'", drModel["Metric"].ToString());
                        //ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                        //object oMetricID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        //if (oMetricID == null)
                        //    modelAttribute.Metric = null;
                        //else
                        //{
                        //    modelAttribute.Metric = Grid.GridCommon.getMetricFromID(Convert.ToInt32(oMetricID));
                        //}
                    }
                    //SeasonalMetric

                    if (drModel[iSeasonalMetric] == null || drModel[iSeasonalMetric].ToString() == "")
                        modelAttribute.SeasonalMetric = null;
                    else
                    {
                        //var querySeasonalMetric = from a in benMAPPollutant.SesonalMetrics where a.SeasonalMetricName.ToLower() == drModel[iSeasonalMetric].ToString().ToLower() select a;
                        //if (querySeasonalMetric != null && querySeasonalMetric.Count() > 0) modelAttribute.SeasonalMetric = querySeasonalMetric.First();

                        if(dicSeasonalMetric.ContainsKey(drModel[iSeasonalMetric].ToString().ToLower()))
                        {
                            modelAttribute.SeasonalMetric = dicSeasonalMetric[drModel[iSeasonalMetric].ToString().ToLower()];
                        }
                    }
                    if (drModel[iStatistic] == null || drModel[iStatistic].ToString() == "")
                        modelAttribute.Statistic = MetricStatic.None;
                    else
                    {
                        // None = 0, Mean = 1, Median = 2, Max = 3, Min = 4, Sum = 5
                        switch (drModel[iStatistic].ToString().ToLower())
                        {
                            case "none":
                                modelAttribute.Statistic = MetricStatic.None;
                                break;
                            case "mean":
                                modelAttribute.Statistic = MetricStatic.Mean;
                                break;
                            case "median":
                                modelAttribute.Statistic = MetricStatic.Median;
                                break;
                            case "max":
                                modelAttribute.Statistic = MetricStatic.Max;
                                break;
                            case "min":
                                modelAttribute.Statistic = MetricStatic.Min;
                                break;
                            case "sum":
                                modelAttribute.Statistic = MetricStatic.Sum;
                                break;
                            default:
                                modelAttribute.Statistic = MetricStatic.None;
                                break;
                        }
                    }
                    if (drModel[iValue] == null || drModel[iValue].ToString() == "")
                        modelAttribute.Values = null;
                    else
                    {
                        modelAttribute.Values = new List<float>();
                        string[] strValues = drModel[iValue].ToString().Split(new char[] { ',' });
                        i = 0;
                        while (i < strValues.Length)
                        {
                            string sValue = strValues[i];
                            if (sValue == ".") modelAttribute.Values.Add(float.MinValue);
                            else
                            {
                                modelAttribute.Values.Add(float.Parse(sValue));
                            }
                            i++;
                        }
                    }
                    modelDataLine.ModelAttributes.Add(modelAttribute);
                }
                dtModel = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                // return null;
            }
        }

        public static void SaveModelDataLineToNewFormatCSV(BenMAPLine modelDataLine, string strFile)
        {
            try
            {
                //PM2.5, 12km CMAQ Nation, Model  污染物名称,GridType名称,Model(Monitor?)
                string sfirst = "\"" + modelDataLine.Pollutant.PollutantName + "," + modelDataLine.GridType.GridDefinitionName + ",Model" + ", Annual Mean\"";
                FileStream fs = new FileStream(strFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine(sfirst);
                string data = "Col,Row,";

                //写出列名称
                for (int i = 0; i < modelDataLine.ModelResultAttributes.First().Values.Count; i++)
                {
                    if(modelDataLine.ModelResultAttributes.First().Values.Keys.ToArray()[i].Contains(",")) continue;
                    data += modelDataLine.ModelResultAttributes.First().Values.Keys.ToArray()[i];
                    if (i < modelDataLine.ModelResultAttributes.First().Values.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);

                //写出各行数据
                for (int i = 0; i < modelDataLine.ModelResultAttributes.Count; i++)
                {
                    try
                    {
                        data = modelDataLine.ModelResultAttributes[i].Col + "," + modelDataLine.ModelResultAttributes[i].Row + ",";
                        for (int j = 0; j < modelDataLine.ModelResultAttributes.First().Values.Count; j++)
                        {
                            if (modelDataLine.ModelResultAttributes.First().Values.Keys.ToArray()[j].Contains(",")) continue;
                            data += modelDataLine.ModelResultAttributes[i].Values.Values.ToArray()[j].ToString();
                            if (j < modelDataLine.ModelResultAttributes.First().Values.Count - 1)
                            {
                                data += ",";
                            }
                        }
                        sw.WriteLine(data);
                    }
                    catch
                    { 
                    }
                }

                sw.Close();
                fs.Close();
                MessageBox.Show("CSV file saved.", "File saved");
            }
            catch
            {
            }
        }

        /// <summary>
        /// 新格式支持
        /// </summary>
        /// <param name="benMAPPollutant"></param>
        /// <param name="modelDataLine"></param>
        /// <param name="dsModel"></param>
        public static void UpdateModelDataLineFromDataSetNewFormat(BenMAPPollutant benMAPPollutant, ref ModelDataLine modelDataLine, System.Data.DataSet dsModel)
        {
            //格式说明：Col Row Metric0...
            try
            {
                modelDataLine.Pollutant = benMAPPollutant;
                modelDataLine.ModelAttributes = new List<ModelAttribute>();
                modelDataLine.ModelResultAttributes = new List<ModelResultAttribute>();
                int iCol = 0;
                int iRow = 1;
                List<string> lstMetricName = new List<string>();
                List<string> lstMetricNameLower = new List<string>();
                Dictionary<string, Metric> dicMetric = new Dictionary<string, Metric>();
                Dictionary<string, SeasonalMetric> dicSeasonalMetric = new Dictionary<string, SeasonalMetric>();
                foreach (Metric metric in benMAPPollutant.Metrics)
                {
                    lstMetricName.Add(metric.MetricName);
                    dicMetric.Add(metric.MetricName.ToLower(), metric);
                }
                if (benMAPPollutant.SesonalMetrics != null)
                {
                    foreach (SeasonalMetric sesonalMetric in benMAPPollutant.SesonalMetrics)
                    {
                        lstMetricName.Add(sesonalMetric.SeasonalMetricName);
                        dicSeasonalMetric.Add(sesonalMetric.SeasonalMetricName.ToLower(), sesonalMetric);
                    }
                }
                lstMetricNameLower = lstMetricName.Select(p => p.ToLower()).ToList();
                Dictionary<int, string> dicRightColumns = new Dictionary<int, string>();
                for (int i = 2; i < dsModel.Tables[0].Columns.Count; i++)
                {
                    if (lstMetricNameLower.Contains(dsModel.Tables[0].Columns[i].ColumnName.ToLower()))
                    {
                        dicRightColumns.Add(i, lstMetricName[lstMetricNameLower.IndexOf(dsModel.Tables[0].Columns[i].ColumnName.ToLower())]);
                    }
                }
                foreach (DataRow drModel in dsModel.Tables[0].Rows)
                {
                    ModelResultAttribute modelAttribute = new ModelResultAttribute();
                    modelAttribute.Col = Convert.ToInt32(drModel[iCol]);
                    modelAttribute.Row = Convert.ToInt32(drModel[iRow]);
                    modelAttribute.Values = new Dictionary<string, float>();
                    foreach (KeyValuePair<int, string> k in dicRightColumns)
                    {
                        modelAttribute.Values.Add(k.Value, Convert.ToSingle(drModel[k.Key]));
                        ModelAttribute mo = new ModelAttribute() { Col = modelAttribute.Col, Row = modelAttribute.Row, Values = new List<float>() };
                        if (dicMetric.ContainsKey(k.Value.ToLower()))
                        {
                            mo.Metric = dicMetric[k.Value.ToLower()];
                            mo.Statistic = (dicMetric[k.Value.ToLower()] is FixedWindowMetric) ? (dicMetric[k.Value.ToLower()] as FixedWindowMetric).Statistic :
                                (dicMetric[k.Value.ToLower()] is MovingWindowMetric) ? (dicMetric[k.Value.ToLower()] as MovingWindowMetric).WindowStatistic : MetricStatic.None;
                        }
                        else if (dicSeasonalMetric.ContainsKey(k.Value.ToLower()))
                        {
                            mo.SeasonalMetric = dicSeasonalMetric[k.Value.ToLower()];
                            mo.Statistic = (dicSeasonalMetric[k.Value.ToLower()].Metric is FixedWindowMetric) ? (dicSeasonalMetric[k.Value.ToLower()].Metric as FixedWindowMetric).Statistic :
                                (dicSeasonalMetric[k.Value.ToLower()].Metric is MovingWindowMetric) ? (dicSeasonalMetric[k.Value.ToLower()].Metric as MovingWindowMetric).WindowStatistic : MetricStatic.None;
                            mo.Metric = mo.SeasonalMetric.Metric;
                        }
                        mo.Values.Add(Convert.ToSingle(drModel[k.Key]));
                        modelDataLine.ModelAttributes.Add(mo);

                    }
                    //modelAttribute.Metric

                    modelDataLine.ModelResultAttributes.Add(modelAttribute);
                }
                dsModel = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                // return null;
            }
        }

        /// <summary>
        /// 根据污染物和Base(Control)生成所需要的统计值以及SHP文件
        /// </summary>
        /// <param name="benMAPGrid"></param>
        /// <param name="benMAPPollutant"></param>
        /// <param name="modelDataLine"></param>
        /// <param name="strShapeFile"></param>
        public static void UpdateModelValuesModelData(Dictionary<string, string> dicSeasonStatics,BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, ModelDataLine modelDataLine, string strShapeFile)
        {
            try
            {
                //首先获取需要增加的列明列表。
                //Dictionary<int, string> dicSeasonStatics = new Dictionary<int, string>();
                //ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //string commandText = string.Format("select * from SEASONALMETRICSEASONS where POLLUTANTSEASONID in (select POLLUTANTSEASONID from POLLUTANTSEASONS where pollutantid= " + benMAPPollutant.PollutantID + ")");
                //System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    dicSeasonStatics.Add(Convert.ToInt32(dr["PollutantSeasonID"]), dr["METRICFUNCTION"].ToString());
                //}
                modelDataLine.GridType = benMAPGrid;
                List<string> lstAddField = new List<string>();
                foreach (Metric metric in benMAPPollutant.Metrics)
                {
                    lstAddField.Add(metric.MetricName);
                }
                if (benMAPPollutant.SesonalMetrics != null)
                {
                    foreach (SeasonalMetric sesonalMetric in benMAPPollutant.SesonalMetrics)
                    {
                        lstAddField.Add(sesonalMetric.SeasonalMetricName);
                    }
                }
                //var colrow = from a in modelDataLine.ModelAttributes select new RowCol { Col = a.Col, Row = a.Row };
                int i = 0;
                modelDataLine.ModelResultAttributes = new List<ModelResultAttribute>();

                //  colrow = colrow.Distinct();
                List <RowCol> lstRowCol = new List<RowCol>();// colrow.ToList();
                Dictionary<string, RowCol> dicRowCol = new Dictionary<string, RowCol>();
                Dictionary<string, ModelResultAttribute> dicModelResultAttribute = new Dictionary<string, ModelResultAttribute>();
                foreach (ModelAttribute ma in modelDataLine.ModelAttributes)
                {
                    if (!dicRowCol.ContainsKey(ma.Col + "," + ma.Row))
                    {
                        dicRowCol.Add(ma.Col + "," + ma.Row, new RowCol() { Col = ma.Col, Row = ma.Row });
                        dicModelResultAttribute.Add(ma.Col + "," + ma.Row, new ModelResultAttribute() { Col = ma.Col, Row = ma.Row, Values = new Dictionary<string, float>() });
                    }
                }
                lstRowCol = dicRowCol.Values.ToList();// lstRowCol.Distinct(new RowColComparer()).ToList();
                List<ModelAttribute> lstModelAttribute365 = new List<ModelAttribute>();
                foreach (Metric metric in benMAPPollutant.Metrics)
                {
                    MetricStatic metricStatic = new MetricStatic();
                    metricStatic = MetricStatic.Mean;
                    if (metric is FixedWindowMetric)
                        metricStatic = (metric as FixedWindowMetric).Statistic;
                    else if (metric is MovingWindowMetric)
                        metricStatic = (metric as MovingWindowMetric).WindowStatistic;//.DailyStatistic;//modify by xiejp 0614 -->
                    var group = from a in modelDataLine.ModelAttributes where a.Metric == metric || a.Metric==null group a by new { a.Col, a.Row } into g select g;//可能要根据metric的类型确定Statics应该选哪个----------xjp
                    foreach (var ingroup in group)
                    {
                        //如果有Static则用Static，假如没有那么用第一条多列数据的平均值
                        foreach (ModelAttribute m in ingroup)
                        {
                            if (m.SeasonalMetric == null)
                            {
                                switch (m.Statistic)
                                {
                                    case MetricStatic.Max:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Max"))
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Max", m.Values.Max());
                                        else
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values[m.Metric.MetricName + ",Max"] = m.Values.Max();
                                        break;
                                    case MetricStatic.Median:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Median"))
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Median", m.Values.OrderBy(p=>p).Median());
                                        else
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values[m.Metric.MetricName + ",Median"] = m.Values.OrderBy(p => p).Median();

                                        break;
                                    case MetricStatic.Min:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Min"))
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Min", m.Values.Min());
                                        else
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values[m.Metric.MetricName + ",Min"] = m.Values.Min();
                                        break;
                                    case MetricStatic.Sum:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Sum"))
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Sum", m.Values.Sum());
                                        else
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values[m.Metric.MetricName + ",Sum"] = m.Values.Sum();
                                        break;
                                    case MetricStatic.Mean:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Mean"))
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Mean", m.Values.Average());
                                        else
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values[m.Metric.MetricName + ",Mean"] = m.Values.Average();
                                        break;
                                    default:
                                        if (m.Values.Count() == 365)
                                        {
                                            List<float> lstTemp = new List<float>();
                                            if (benMAPPollutant.Seasons != null && benMAPPollutant.Seasons.Count > 0)
                                            {
                                                foreach (Season s in benMAPPollutant.Seasons)
                                                {
                                                    lstTemp.AddRange(m.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1));
                                                }

                                            }
                                            else
                                                lstTemp = m.Values;
                                            lstModelAttribute365.Add(new ModelAttribute() { Col = m.Col, Row = m.Row, Metric = metric, Values = lstTemp });
                                        }
                                        break;
                                }
                            }


                        }
                        //如果有Static则用Static，假如没有那么用第一条多列数据的平均值
                        foreach (ModelAttribute m in ingroup)
                        {
                            if (m.SeasonalMetric != null)
                            {
                                switch (m.Statistic)
                                {
                                    case MetricStatic.Max:
                                        if(!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Max"))
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Max", m.Values.Max());
                                        break;
                                    case MetricStatic.Median:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Median"))
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Median", m.Values.OrderBy(p => p).Median());
                                        break;
                                    case MetricStatic.Min:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Min"))
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Min", m.Values.Min());
                                        break;
                                    case MetricStatic.Sum:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Sum"))
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Sum", m.Values.Sum());
                                        break;
                                    case MetricStatic.Mean:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Mean"))
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Mean", m.Values.Average());
                                        break;
                                    default:
                                        if (m.Values.Count() == 365)
                                        {
                                            List<float> lstTemp = new List<float>();
                                            if (benMAPPollutant.Seasons != null && benMAPPollutant.Seasons.Count > 0)
                                            {
                                                foreach (Season s in benMAPPollutant.Seasons)
                                                {
                                                    lstTemp.AddRange(m.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1));
                                                }

                                            }
                                            else
                                                lstTemp = m.Values;
                                            lstModelAttribute365.Add(new ModelAttribute() { Col = m.Col, Row = m.Row, Metric = metric, Values = lstTemp });
                                            //lstModelAttribute365.Add(new ModelAttribute() { Col = m.Col, Row = m.Row, Metric = metric, Values = m.Values });
                                        }
                                        break;
                                }
                            }


                        }
                        ModelAttribute mAttribute = null;
                        var staticquery = from a in ingroup where a.Statistic == metricStatic select a;
                        if (staticquery != null && staticquery.Count()>0) { mAttribute = staticquery.First(); }
                        else
                        { mAttribute = ingroup.First(); }
                        ModelResultAttribute mrAttribute;
                        //var query = from a in modelDataLine.ModelResultAttributes where a.Col == mAttribute.Col && a.Row == mAttribute.Row select a;
                        //mrAttribute = query.First();
                        mrAttribute = dicModelResultAttribute[mAttribute.Col + "," + mAttribute.Row];
                        int hourly = 0;
                        if (mAttribute.Values.Count >= 8759) hourly = 1;
                        //if (mrAttribute.Values == null) { mrAttribute.Values = new Dictionary<string, float>(); }
                        //---------------modify by xiejp 20120626 可以解析连续值-------------------------------
                        if (metric is FixedWindowMetric)
                        {
                            FixedWindowMetric fixedWindowMetric = (FixedWindowMetric)metric;
                            //------------如果是hourly，计算小时值根据staticstic,然后计算年值
                            if (hourly == 0)// || (fixedWindowMetric.StartHour == 0 && fixedWindowMetric.EndHour == 23))
                            {
                                List<float> lstmAttribute = mAttribute.Values.Where(p => p != float.MinValue).ToList();
                                if (lstmAttribute != null && lstmAttribute.Count > 0)
                                {
                                    switch (fixedWindowMetric.Statistic)
                                    {
                                        case MetricStatic.Max:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Max());
                                            break;
                                        case MetricStatic.Mean:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Average());   
                                            break;
                                        case MetricStatic.Median:
                                            lstmAttribute.Sort();
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.OrderBy(p => p).Median());  
                                            break;
                                        case MetricStatic.Min:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Min()); 
                                            break;
                                        case MetricStatic.None:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Average());  
                                            break;
                                        case MetricStatic.Sum:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Sum()); 
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                //----根据开始hour和结束hour再通过Statistic计算---------首先生成一个List<double>---
                                Dictionary<int, List<float>> dicHourlyValue = new Dictionary<int, List<float>>();
                                i = 0;
                                while (i < 365)
                                {
                                    //lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour);
                                    //lstTemp.Sort();
                                    try
                                    {
                                        List<float> lstTemp = new List<float>();
                                        if (i * 24 + fixedWindowMetric.StartHour < mAttribute.Values.Count && i * 24 + fixedWindowMetric.StartHour + fixedWindowMetric.EndHour - fixedWindowMetric.StartHour < mAttribute.Values.Count)
                                            lstTemp = mAttribute.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour + 1);
                                        else
                                            lstTemp = mAttribute.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, mAttribute.Values.Count - (i * 24 + fixedWindowMetric.StartHour));
                                        lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                                        if (lstTemp != null && lstTemp.Count > 0)
                                            dicHourlyValue.Add(i, lstTemp);// monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour));
                                        else
                                            dicHourlyValue.Add(i, new List<float>() { float.MinValue });
                                    }
                                    catch (Exception ex)
                                    { 
                                    }
                                    i++;
                                }
                                Dictionary<int, int> lstTempDayValue = new Dictionary<int, int>();
                                if (benMAPPollutant.Seasons != null && benMAPPollutant.Seasons.Count > 0)
                                {
                                    foreach (Season s in benMAPPollutant.Seasons)
                                    {
                                        
                                        for (int iTemp = s.StartDay; iTemp <= s.EndDay; iTemp++)
                                        {
                                            lstTempDayValue.Add(iTemp,iTemp);
                                        }
                                    }
                                   dicHourlyValue= dicHourlyValue.Where(p => lstTempDayValue.ContainsKey(p.Key)).ToDictionary(p=>p.Key,p=>p.Value);
                                }
                                
                                 
                               //modify by xiejp 把值更新为365天
                                switch (fixedWindowMetric.Statistic)
                                {
                                    case MetricStatic.Max:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p=>!(p.Count==1 && p[0]==float.MinValue)).Max(p => p.Max()));
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                             dicHourlyValue.Values.Select(p => p.Max()).ToList()});
                                        break;
                                    case MetricStatic.Mean:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Average()));
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Average()).ToList()});
                                       
                                        break;
                                    case MetricStatic.Median:
                                        List<float> lstTemp = new List<float>();
                                        foreach (List<float> ld in dicHourlyValue.Values)
                                        {
                                            if(!(ld.Count==1 && ld[0]==float.MinValue))
                                                lstTemp.Add(ld.OrderBy(p => p).Median());
                                        }
                                        mrAttribute.Values.Add(metric.MetricName,lstTemp.OrderBy(p=>p).Median());//----------错的，要重做为中间值
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.OrderBy(a => a).Median()).ToList()
                                        });
                                       
                                        break;
                                    case MetricStatic.Min:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Min(p => p.Min()));
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Min()).ToList()});
                                       
                                        break;
                                    case MetricStatic.None:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Average()));
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Average()).ToList()});
                                       
                                        break;
                                    case MetricStatic.Sum:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Sum(p => p.Sum()));
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Sum()).ToList()});
                                       
                                        break;
                                }
                                //----------计算值
                            }

                            //------------如果不是，直接计算-----------------------
                        }
                        else if (metric is MovingWindowMetric)
                        {
                            MovingWindowMetric movingWindowMetric = (MovingWindowMetric)metric;
                            //------------如果是hourly，计算小时值根据staticstic,然后计算年值
                            if (hourly == 0 || movingWindowMetric.HourlyMetricGeneration == 1)
                            {
                                List<float> lstmAttribute = mAttribute.Values.Where(p => p != float.MinValue).ToList();
                                if (lstmAttribute != null && lstmAttribute.Count > 0)
                                {
                                    switch (movingWindowMetric.WindowStatistic)
                                    {
                                        case MetricStatic.Max:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Max());
                                            break;
                                        case MetricStatic.Mean:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Average());
                                            break;
                                        case MetricStatic.Median:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.OrderBy(p=>p).Median());//----------错的，要重做为中间值
                                            break;
                                        case MetricStatic.Min:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Min());
                                            break;
                                        case MetricStatic.None:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Average());
                                            break;
                                        case MetricStatic.Sum:
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Sum());
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                //----根据开始hour和结束hour再通过Statistic计算---------首先生成一个List<double>---
                                Dictionary<int, List<float>> dicHourlyValue = new Dictionary<int, List<float>>();
                                i = 0;
                                //monitorValue.Values.Remove(float.MinValue);

                                while (i < 365)
                                {
                                    List<float> lstTemp = mAttribute.Values.GetRange(i * 24, 24);
                                    //lstTemp.Remove(float.MinValue);
                                    //-------------------采用WindowSize------------------
                                    List<float> lstWindowSize = new List<float>();
                                    for (int iWindowSize = 0; iWindowSize <= lstTemp.Count - movingWindowMetric.WindowSize; iWindowSize++)
                                    {
                                        if (iWindowSize == lstTemp.Count - movingWindowMetric.WindowSize - 1)
                                        {
                                            try
                                            {
                                                if (lstTemp.GetRange(iWindowSize, lstTemp.Count - iWindowSize - 1).Where(p => p != float.MinValue).Count() > 0)
                                                    lstWindowSize.Add(lstTemp.GetRange(iWindowSize, lstTemp.Count - iWindowSize - 1).Where(p => p != float.MinValue).Average());
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                if (lstTemp.GetRange(iWindowSize, movingWindowMetric.WindowSize).Where(p => p != float.MinValue).Count() > 0)
                                                    lstWindowSize.Add(lstTemp.GetRange(iWindowSize, movingWindowMetric.WindowSize).Where(p => p != float.MinValue).Average());
                                            }
                                            catch
                                            {
                                            }
                                        }

                                    }
                                    //-----------------end 采用WindowSize------------------
                                    lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                                    if (lstWindowSize != null && lstWindowSize.Count > 0)
                                        dicHourlyValue.Add(i, lstWindowSize);
                                    else
                                        dicHourlyValue.Add(i, new List<float>() { float.MinValue });
                                    i++;
                                }
                                Dictionary<int, int> lstTempDayValue = new Dictionary<int, int>();
                                if (benMAPPollutant.Seasons != null && benMAPPollutant.Seasons.Count > 0)
                                {
                                    foreach (Season s in benMAPPollutant.Seasons)
                                    {

                                        for (int iTemp = s.StartDay; iTemp <= s.EndDay; iTemp++)
                                        {
                                            lstTempDayValue.Add(iTemp, iTemp);
                                        }
                                    }
                                    dicHourlyValue = dicHourlyValue.Where(p => lstTempDayValue.ContainsKey(p.Key)).ToDictionary(p => p.Key, p => p.Value);
                                }
                                
                                 
                                double d = 0;
                                switch (movingWindowMetric.DailyStatistic)
                                {
                                    case MetricStatic.Max:
                                        switch (movingWindowMetric.WindowStatistic)
                                        {
                                            case MetricStatic.Max:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p=>!(p.Count==1 &&p[0]==float.MinValue)).Max(p => p.Max()));
                                                break;
                                            case MetricStatic.Mean:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Max()));
                                                break;
                                            case MetricStatic.Median:
                                                List<float> lstTempIn=new List<float>();
                                                foreach (List<float> k in dicHourlyValue.Values)
                                                {
                                                    if (!(k.Count == 1 && k[0] == float.MinValue))
                                                        lstTempIn.Add(k.Max());
                                                }
                                                mrAttribute.Values.Add(metric.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                break;
                                            case MetricStatic.Min:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Min(p => p.Max()));
                                                break;
                                            case MetricStatic.None:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Max()));
                                                break;
                                            case MetricStatic.Sum:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Sum(p => p.Max()));
                                                break;

                                        }
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Max()).ToList()});
                                       
                                       
                                        break;
                                    case MetricStatic.Mean:
                                    case MetricStatic.None:
                                        switch (movingWindowMetric.WindowStatistic)
                                        {
                                            case MetricStatic.Max:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Max(p => p.Average()));
                                                break;
                                            case MetricStatic.Mean:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Average()));
                                                break;
                                            case MetricStatic.Median:
                                                List<float> lstTempIn = new List<float>();
                                                foreach (List<float> k in dicHourlyValue.Values)
                                                {
                                                    if(!(k.Count==1 && k[0]==float.MinValue))
                                                    lstTempIn.Add(k.Average());
                                                }
                                                mrAttribute.Values.Add(metric.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                break;
                                            case MetricStatic.Min:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Min(p => p.Average()));
                                                break;
                                            case MetricStatic.None:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Average()));
                                                break;
                                            case MetricStatic.Sum:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Sum(p => p.Average()));
                                                break;

                                        }
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Average()).ToList()});
                                       
                                        //mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                        break;
                                    case MetricStatic.Median:
                                        List<float> lstTemp = new List<float>();
                                        foreach (List<float> ld in dicHourlyValue.Values)
                                        {
                                            if(!(ld.Count==1 &&ld[0]==float.MinValue))
                                                lstTemp.Add(ld.OrderBy(p => p).Median());
                                        }
                                        //mrAttribute.Values.Add(metric.MetricName, lstTemp.Median());//----------错的，要重做为中间值
                                        switch (movingWindowMetric.WindowStatistic)
                                        {
                                            case MetricStatic.Max:
                                                mrAttribute.Values.Add(metric.MetricName, lstTemp.Max());
                                                break;
                                            case MetricStatic.Mean:
                                                mrAttribute.Values.Add(metric.MetricName, lstTemp.Average());
                                                break;
                                            case MetricStatic.Median:

                                                mrAttribute.Values.Add(metric.MetricName, lstTemp.OrderBy(p => p).Median());
                                                break;
                                            case MetricStatic.Min:
                                                mrAttribute.Values.Add(metric.MetricName, lstTemp.Min());
                                                break;
                                            case MetricStatic.None:
                                                mrAttribute.Values.Add(metric.MetricName, lstTemp.Average());
                                                break;
                                            case MetricStatic.Sum:
                                                mrAttribute.Values.Add(metric.MetricName, lstTemp.Sum());
                                                break;

                                        }
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.OrderBy(a => a).Median()).ToList()
                                        });
                                       
                                        break;
                                    case MetricStatic.Min:
                                        switch (movingWindowMetric.WindowStatistic)
                                        {
                                            case MetricStatic.Max:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Max(p => p.Min()));
                                                break;
                                            case MetricStatic.Mean:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Min()));
                                                break;
                                            case MetricStatic.Median:
                                                List<float> lstTempIn = new List<float>();
                                                foreach (List<float> k in dicHourlyValue.Values)
                                                {
                                                    if(!(k.Count==1 &&k[0]==float.MinValue))
                                                    lstTempIn.Add(k.Min());
                                                }
                                                mrAttribute.Values.Add(metric.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                break;
                                            case MetricStatic.Min:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Min(p => p.Min()));
                                                break;
                                            case MetricStatic.None:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Min()));
                                                break;
                                            case MetricStatic.Sum:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Sum(p => p.Min()));
                                                break;

                                        }
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Min()).ToList()});
                                       
                                        
                                        break;
                                    
                                    case MetricStatic.Sum:
                                        switch (movingWindowMetric.WindowStatistic)
                                        {
                                            case MetricStatic.Max:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Max(p => p.Sum()));
                                                break;
                                            case MetricStatic.Mean:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Sum()));
                                                break;
                                            case MetricStatic.Median:
                                                List<float> lstTempIn = new List<float>();
                                                foreach (List<float> k in dicHourlyValue.Values)
                                                {
                                                    if(!(k.Count==1 &&k[0]==float.MinValue))
                                                    lstTempIn.Add(k.Sum());
                                                }
                                                mrAttribute.Values.Add(metric.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                break;
                                            case MetricStatic.Min:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Min(p => p.Sum()));
                                                break;
                                            case MetricStatic.None:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Sum()));
                                                break;
                                            case MetricStatic.Sum:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Sum(p => p.Sum()));
                                                break;

                                        }
                                        lstModelAttribute365.Add(new ModelAttribute(){ Col= mrAttribute.Col, Row= mrAttribute.Row, Metric= metric, Statistic= MetricStatic.None, Values=
                                            dicHourlyValue.Values.Select(p => p.Sum()).ToList()});
                                       
                                        break;
                                }
                                //----------计算值
                            }
                        }
                        else if (metric is CustomerMetric)
                        {
                            //----------------按函数来统计------------------暂不实现--------------------------需要讨论后再做---
                        }
                        else
                        {
                            //fixedWindowMetric = (FixedWindowMetric)m;
                            //------------如果是hourly，计算小时值根据staticstic,然后计算年值

                            //monitorValue.Values.Remove(float.MinValue);
                            List<float> lstmAttribute = mAttribute.Values.Where(p => p != float.MinValue).ToList();
                            if (lstmAttribute != null && lstmAttribute.Count > 0)
                                mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Average());
                            else
                                mrAttribute.Values.Add(metric.MetricName, 0);
                        }
                        //----------------end modify by xiejp 20120626
                        //switch (metricStatic)
                        //{
                        //    case MetricStatic.Max:
                        //        mrAttribute.Values.Add(mAttribute.Metric.MetricName, mAttribute.Values.Max());
                        //        break;
                        //    case MetricStatic.Median:
                        //        mrAttribute.Values.Add(mAttribute.Metric.MetricName, Convert.ToSingle((mAttribute.Values.Max() - mAttribute.Values.Min()) / 2.00000));
                        //        break;
                        //    case MetricStatic.Min:
                        //        mrAttribute.Values.Add(mAttribute.Metric.MetricName, mAttribute.Values.Min());
                        //        break;
                        //    case MetricStatic.Sum:
                        //        mrAttribute.Values.Add(mAttribute.Metric.MetricName, mAttribute.Values.Sum());
                        //        break;
                        //    default:
                        //        mrAttribute.Values.Add(mAttribute.Metric.MetricName, mAttribute.Values.Average());
                        //        break;
                        //}
                        //foreach (var item in ingroup)
                        //  {
                        //      Console.WriteLine("\t" + item);
                        //  }
                    }
                }
                //----------------------------SeasonalMetric add                          ---------------------------------------------------
                foreach (SeasonalMetric seasonalmetric in benMAPPollutant.SesonalMetrics)
                {
                    MetricStatic metricStatic = new MetricStatic();
                    metricStatic = MetricStatic.Mean;
                    if (seasonalmetric.Metric is FixedWindowMetric)
                        metricStatic = (seasonalmetric.Metric as FixedWindowMetric).Statistic;
                    else if (seasonalmetric.Metric is MovingWindowMetric)
                        metricStatic = (seasonalmetric.Metric as MovingWindowMetric).WindowStatistic;//.DailyStatistic;//modify by xiejp 0614 -->.DailyStatistic;
                    var group = from a in modelDataLine.ModelAttributes where a.SeasonalMetric == seasonalmetric group a by new { a.Col, a.Row } into g select g;//可能要根据metric的类型确定Statics应该选哪个----------xjp
                    if (group != null && group.Count() > 0)
                    {
                        foreach (var ingroup in group)
                        {
                            //------------得到不同Static的值填充如结果数据--------------------
                            foreach (ModelAttribute m in ingroup)
                            {

                                switch (m.Statistic)
                                {
                                    case MetricStatic.Max:
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.SeasonalMetric.SeasonalMetricName + ",Max", m.Values.Max());
                                        break;
                                    case MetricStatic.Median:
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.SeasonalMetric.SeasonalMetricName + ",Median", m.Values.OrderBy(p => p).Median());
                                        break;
                                    case MetricStatic.Min:
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.SeasonalMetric.SeasonalMetricName + ",Min", m.Values.Min());
                                        break;
                                    case MetricStatic.Sum:
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.SeasonalMetric.SeasonalMetricName + ",Sum", m.Values.Sum());
                                        break;
                                    case MetricStatic.Mean:
                                        dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.SeasonalMetric.SeasonalMetricName + ",Mean", m.Values.Average());
                                        break;
                                    case MetricStatic.None:
                                        lstModelAttribute365.Add(m);
                                        break;
                                }

                            }
                            //如果有Static则用Static，假如没有那么用第一条多列数据的平均值
                            ModelAttribute mAttribute = null;
                            var staticquery = from a in ingroup where a.Statistic == metricStatic select a;
                            if (staticquery != null && staticquery.Count() > 0)
                                mAttribute = staticquery.First();
                            else
                            {
                                mAttribute = ingroup.First();
                            }

                            ModelResultAttribute mrAttribute;
                            //var query = from a in modelDataLine.ModelResultAttributes where a.Col == mAttribute.Col && a.Row == mAttribute.Row select a;
                            //mrAttribute = query.First();
                            mrAttribute = dicModelResultAttribute[mAttribute.Col + "," + mAttribute.Row];
                            //if (mrAttribute.Values == null) mrAttribute.Values = new Dictionary<string, float>();
                            // mrAttribute.Values.Add(mAttribute.SeasonalMetric.SeasonalMetricName, mAttribute.Values.Average());
                            switch (metricStatic)
                            {
                                case MetricStatic.Max:
                                    mrAttribute.Values.Add(mAttribute.SeasonalMetric.SeasonalMetricName, mAttribute.Values.Max());
                                    break;
                                case MetricStatic.Median:
                                    mrAttribute.Values.Add(mAttribute.SeasonalMetric.SeasonalMetricName, Convert.ToSingle((mAttribute.Values.Max() - mAttribute.Values.Min()) / 2.00000));
                                    break;
                                case MetricStatic.Min:
                                    mrAttribute.Values.Add(mAttribute.SeasonalMetric.SeasonalMetricName, mAttribute.Values.Min());
                                    break;
                                case MetricStatic.Sum:
                                    mrAttribute.Values.Add(mAttribute.SeasonalMetric.SeasonalMetricName, mAttribute.Values.Sum());
                                    break;
                                case MetricStatic.Mean:
                                    mrAttribute.Values.Add(mAttribute.SeasonalMetric.SeasonalMetricName, mAttribute.Values.Average());
                                    break;
                            }
                            //foreach (var item in ingroup)
                            //  {
                            //      Console.WriteLine("\t" + item);
                            //  }
                        }
                    }
                    else
                    {
                        var groupSeasonal = from a in lstModelAttribute365 where a.Metric!=null && a.Metric.MetricID == seasonalmetric.Metric.MetricID group a by new { a.Col, a.Row } into g select g;//可能要根据metric的类型确定Statics应该选哪个----------xjp
                        List<ModelAttribute> lstSeasonalAdd = new List<ModelAttribute>();
                        if(groupSeasonal==null || groupSeasonal.Count()==0)
                            groupSeasonal = from a in lstModelAttribute365 where a.Metric ==null group a by new { a.Col, a.Row } into g select g;//可能要根据metric的类型确定Statics应该选哪个----------xjp
                        if (groupSeasonal != null && groupSeasonal.Count() > 0)
                        {
                            foreach (var ingroup in groupSeasonal)
                            {
                                ModelAttribute mAttribute = ingroup.First();
                                ModelResultAttribute mrAttribute;
                                //var query = from a in modelDataLine.ModelResultAttributes where a.Col == mAttribute.Col && a.Row == mAttribute.Row select a;
                                //mrAttribute = query.First();
                                mrAttribute = dicModelResultAttribute[mAttribute.Col + "," + mAttribute.Row];
                                //------------得到需要的季度值列表----------------
                                List<float> lstQuality = new List<float>();
                                if (seasonalmetric.Seasons == null || seasonalmetric.Seasons.Count == 0)//如果没有Seasons则是Mean
                                {
                                    mrAttribute.Values.Add(seasonalmetric.SeasonalMetricName, mAttribute.Values.Average());
                                    lstQuality.Add(mAttribute.Values.GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Average());
                                    lstQuality.Add(mAttribute.Values.GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Average());
                                    lstQuality.Add(mAttribute.Values.GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Average());
                                    lstQuality.Add(mAttribute.Values.GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Average());
                                }
                                else
                                {

                                    foreach (Season s in seasonalmetric.Seasons)
                                    {
                                        switch (dicSeasonStatics[s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()])//dr["PollutantSeasonID"].ToString()+"," + dr["SeasonalMetricID"].ToString()
                                        {
                                            case "":
                                            case "Mean":
                                                lstQuality.Add(mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Average());
                                                break;
                                            case "Median":
                                                lstQuality.Add(mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).OrderBy(p => p).Median());
                                                break;
                                            case "Max":
                                                lstQuality.Add(mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Max());
                                                break;
                                            case "Min":
                                                lstQuality.Add(mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Min());
                                                break;
                                            case "Sum":
                                                lstQuality.Add(mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ? float.MinValue : mAttribute.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Sum());
                                                break;

                                        }

                                    }
                                    if (lstQuality.Where(p => p != float.MinValue).Count() > 0)
                                        mrAttribute.Values.Add(seasonalmetric.SeasonalMetricName, lstQuality.Where(p => p != float.MinValue).Average());

                                }
                                lstSeasonalAdd.Add(new ModelAttribute()
                                {
                                    Col = mrAttribute.Col,
                                    Row = mrAttribute.Row,
                                    Metric = seasonalmetric.Metric,
                                    SeasonalMetric = seasonalmetric,
                                    Statistic = MetricStatic.None,
                                    Values = lstQuality
                                });
                            }
                            lstModelAttribute365.AddRange(lstSeasonalAdd);


                        }

                    }
                }
                //---------modify by xiejp2011927---为了加快加载aqg文件的速度，删掉ModelAttributes --modify by xiejp 20120710 继续保留ModelAttributes ,但是更新为365天的值如果有的话
                //modelDataLine.ModelAttributes = null;
                modelDataLine.ModelAttributes = lstModelAttribute365;
                //---------------清理所有的空值-------------------
                List<string> lstColRow = dicModelResultAttribute.Select(p => p.Key).ToList();
                foreach (string s in lstColRow)
                {
                   var queryMinValue= dicModelResultAttribute[s].Values.Where(p => p.Value == float.MinValue).ToList();
                   foreach (KeyValuePair<string, float> k in queryMinValue)
                   {
                       dicModelResultAttribute[s].Values.Remove(k.Key);
                   }
                   if (dicModelResultAttribute[s].Values.Count == 0) dicModelResultAttribute.Remove(s);
                }
                modelDataLine.ModelResultAttributes = dicModelResultAttribute.Values.ToList();
                //modelDataLine.ShapeFile = strShapeFile;
                if(strShapeFile!="")
                SaveBenMAPLineShapeFile(benMAPGrid, benMAPPollutant, modelDataLine, strShapeFile);
                GC.Collect();
                // //生成SHP为了完成此功能，必须在程序中加上目录放入运行程序下Result\tmp中，命名规则 ：污染物+baseline/control+当前做操时间+.shp,例如：pm10baseline20110921090622.shp
                // FeatureSet fs = new FeatureSet();
                // string shapeFileName = "";
                // string AppPath = Application.StartupPath;
                // //----------------------------------------xjp-------------仅用于测试！
                // //AppPath = @"D:\软件项目\BenMap\BenMap\trunk\Code\BenMAP\bin\Debug";
                // if (benMAPGrid is ShapefileGrid) shapeFileName = (benMAPGrid as ShapefileGrid).ShapefileName;
                // if (benMAPGrid is RegularGrid) shapeFileName = (benMAPGrid as RegularGrid).ShapefileName;
                // fs.Open(AppPath + @"\Data\Shapefiles\" + shapeFileName + ".shp");

                // //加值
                // i = 0;
                // int iCol = 0;
                // int iRow = 0;
                // List<string> lstRemoveName = new List<string>();
                // while (i < fs.DataTable.Columns.Count)
                // {
                //     if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col") iCol = i;
                //     if (fs.DataTable.Columns[i].ColumnName.ToLower() == "row") iRow = i;

                //     i++;
                // }
                // i = 0;

                // while (i < fs.DataTable.Columns.Count)
                // {
                //     if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col" || fs.DataTable.Columns[i].ColumnName.ToLower() == "row")
                //     { }
                //     else
                //         lstRemoveName.Add(fs.DataTable.Columns[i].ColumnName);

                //     i++;
                // }
                // foreach (string s in lstRemoveName)
                // {
                //     fs.DataTable.Columns.Remove(s);
                // }
                // foreach (string sField in lstAddField)
                // {
                //     fs.DataTable.Columns.Add(sField, typeof(double));

                // }
                // i = 0;
                // while (i < fs.DataTable.Columns.Count)
                // {
                //     if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col") iCol = i;
                //     if (fs.DataTable.Columns[i].ColumnName.ToLower() == "row") iRow = i;

                //     i++;
                // }
                // i = 0;
                // while (i < fs.DataTable.Rows.Count)
                // {
                //     var fsQuery = from a in modelDataLine.ModelResultAttributes
                //                   where a.Col == Convert.ToInt32(fs.DataTable.Rows[i][iCol]) && a.Row == Convert.ToInt32(fs.DataTable.Rows[i][iRow])
                //                   select a;
                //     ModelResultAttribute fsModelResultAttribute = fsQuery.First();
                //     foreach (string sField in lstAddField)
                //     {
                //         if( fsModelResultAttribute.Values.Keys.Contains(sField))
                //         {
                //         fs.DataTable.Rows[i][sField] = fsModelResultAttribute.Values[sField];
                //         }

                //     }
                //     i++;
                // }
                // //Save SHPFile field只有10个字符串长度-----------------------所以读入的时候需修正，如果前面9个对应则ok!!

                //// string strPath = Application.StartupPath + @"\Data\Result\" + benMAPPollutant.PollutantName + DateTime.Now.ToString() + ".shp";
                // modelDataLine.ShapeFile = strShapeFile;
                // fs.SaveAs(strShapeFile, true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                // return null;
            }
        }

        public static void SaveBenMAPLineShapeFile(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, BenMAPLine modelDataLine, string strShapeFile)
        {
            //生成SHP为了完成此功能，必须在程序中加上目录放入运行程序下Result\tmp中，命名规则 ：污染物+baseline/control+当前做操时间+.shp,例如：pm10baseline20110921090622.shp
            List<string> lstAddField = new List<string>();
            List<float[]> lstResultCopy = new List<float[]>();
            foreach (Metric metric in benMAPPollutant.Metrics)
            {
                lstAddField.Add(metric.MetricName);
            }
            if (benMAPPollutant.SesonalMetrics != null)
            {
                foreach (SeasonalMetric sesonalMetric in benMAPPollutant.SesonalMetrics)
                {
                    lstAddField.Add(sesonalMetric.SeasonalMetricName);
                }
            }
            List<float> lstd = new List<float>();
            IFeatureSet fs = new FeatureSet();
            string shapeFileName = "";
            string AppPath = Application.StartupPath;
            //----------------------------------------xjp-------------仅用于测试！
            //AppPath = @"D:\软件项目\BenMap\BenMap\trunk\Code\BenMAP\bin\Debug";
            if (benMAPGrid is ShapefileGrid) shapeFileName = (benMAPGrid as ShapefileGrid).ShapefileName;
            if (benMAPGrid is RegularGrid) shapeFileName = (benMAPGrid as RegularGrid).ShapefileName;
            fs = FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + shapeFileName + ".shp");

            //加值
            int i = 0;
            int iCol = 0;
            int iRow = 0;
            List<string> lstRemoveName = new List<string>();
            while (i < fs.DataTable.Columns.Count)
            {
                if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col") iCol = i;
                if (fs.DataTable.Columns[i].ColumnName.ToLower() == "row") iRow = i;

                i++;
            }
            i = 0;

            while (i < fs.DataTable.Columns.Count)
            {
                if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col" || fs.DataTable.Columns[i].ColumnName.ToLower() == "row")
                { }
                else
                    lstRemoveName.Add(fs.DataTable.Columns[i].ColumnName);

                i++;
            }
            foreach (string s in lstRemoveName)
            {
                fs.DataTable.Columns.Remove(s);
            }
            foreach (string sField in lstAddField)
            {
                fs.DataTable.Columns.Add(sField, typeof(double));
            }
            i = 0;
            while (i < fs.DataTable.Columns.Count)
            {
                if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col") iCol = i;
                if (fs.DataTable.Columns[i].ColumnName.ToLower() == "row") iRow = i;

                i++;
            }
            i = 0;
            //------------------------为了优化速度，写成dictionary
            Dictionary<string, Dictionary<string, float>> DicResult = new Dictionary<string, Dictionary<string, float>>();
            foreach (ModelResultAttribute mra in modelDataLine.ModelResultAttributes)
            {
                DicResult.Add(mra.Col + "," + mra.Row, mra.Values);
            }
            while (i < fs.DataTable.Rows.Count)
            {
                try
                {
                    // RowCol rc = new RowCol() { Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]), Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]) };
                    string s = fs.DataTable.Rows[i][iCol] + "," + fs.DataTable.Rows[i][iRow];
                    lstd = new List<float>();
                    lstd.Add(Convert.ToSingle(fs.DataTable.Rows[i][iCol]));
                    lstd.Add(Convert.ToSingle(fs.DataTable.Rows[i][iRow]));
                    if (DicResult.Keys.Contains(s))
                    {
                        Dictionary<string, float> kyp = DicResult[s];
                        foreach (string sField in lstAddField)
                        {
                            if (kyp.Keys.Contains(sField))
                            {
                                fs.DataTable.Rows[i][sField] = Math.Round( kyp[sField],2);
                                if (double.IsNaN(kyp[sField]))
                                {
                                    fs.DataTable.Rows[i][sField] = Convert.ToDouble(0.00);
                                }
                            }
                            else
                            {
                                fs.DataTable.Rows[i][sField] = Convert.ToDouble(0.00);
                            }
                        }
                    }
                    else
                    {
                        foreach (string sField in lstAddField)
                        {
                            fs.DataTable.Rows[i][sField] = Convert.ToDouble(0.00);
                        }
                    }

                    i++;
                }
                catch
                {
                    foreach (string sField in lstAddField)
                    {
                        //if (fsModelResultAttribute.Values.Keys.Contains(sField))
                        //{
                        fs.DataTable.Rows[i][sField] = Convert.ToDouble(0.00);
                        //}
                    }
                    i++;
                }
                finally
                {
                    foreach (string sField in lstAddField)
                    {
                        lstd.Add(Convert.ToSingle(fs.DataTable.Rows[i - 1][sField]));
                    }
                    lstResultCopy.Add(lstd.ToArray());
                }
                //var fsQuery = from a in modelDataLine.ModelResultAttributes
                //              where a.Col == Convert.ToInt32(fs.DataTable.Rows[i][iCol]) && a.Row == Convert.ToInt32(fs.DataTable.Rows[i][iRow])
                //              select a;
                //ModelResultAttribute fsModelResultAttribute = fsQuery.First();
                //foreach (string sField in lstAddField)
                //{
                //    if (fsModelResultAttribute.Values.Keys.Contains(sField))
                //    {
                //        fs.DataTable.Rows[i][sField] = fsModelResultAttribute.Values[sField];
                //    }

                //}
                //i++;
            }
            //Save SHPFile field只有10个字符串长度-----------------------所以读入的时候需修正，如果前面9个对应则ok!!
            //modelDataLine.ResultCopy = lstResultCopy;
            // string strPath = Application.StartupPath + @"\Data\Result\" + benMAPPollutant.PollutantName + DateTime.Now.ToString() + ".shp";
            modelDataLine.ShapeFile = strShapeFile;
            if (File.Exists(strShapeFile)) CommonClass.DeleteShapeFileName(strShapeFile);
            fs.SaveAs(strShapeFile, true);
        }

        public static Metric getMetricFromName(string MetricName)
        {
            try
            {
                Metric metric = new Metric();
                return metric;
            }
            catch
            {
                return null;
            }
        }

        public static void UpdateMonitorDicMetricValue(BenMAPPollutant benMAPPollutant, List<MonitorValue> lstMonitorValues)
        {
            //--------------首先生成Metric值-----------先忽略是否有效的问题------------------
            try
            {
                int hourly = 0;//1代表hourly
                int i = 0;
                FixedWindowMetric fixedWindowMetric = null;
                MovingWindowMetric movingWindowMetric = null;
                CustomerMetric customerMetric = null;
                Dictionary<int, List<float>> dicHourlyValue = new Dictionary<int, List<float>>();
                Dictionary<int, List<float>> dicHourlyValue365 = new Dictionary<int, List<float>>();
                List<float> lstTemp = null;
                foreach (MonitorValue monitorValue in lstMonitorValues)
                {
                    monitorValue.dicMetricValues = new Dictionary<string, float>();
                    monitorValue.dicMetricValues365 = new Dictionary<string, List<float>>();
                    if (monitorValue.SeasonalMetric != null)
                    {
                        monitorValue.dicMetricValues.Add(monitorValue.SeasonalMetric.SeasonalMetricName, monitorValue.Values.First());
                        if (monitorValue.Metric != null)
                        {
                            monitorValue.dicMetricValues.Add(monitorValue.Metric.MetricName, monitorValue.Values.First());
                        }
                    }
                    else if (monitorValue.Metric != null)
                    {
                        monitorValue.dicMetricValues.Add(monitorValue.Metric.MetricName, monitorValue.Values.First());
                    }
                    else
                    {
                        //如果没有metric ，判断value 大小 +1/365 -> 24 每个小时一个数据  +1/365 1 每天一个数据 少于365 默认为从第一天算起的按天的数据
                        if (monitorValue.Values.Count >= 8759) hourly = 1;
                        if (benMAPPollutant.Metrics != null && benMAPPollutant.Metrics.Count > 0)
                        {
                            foreach (Metric m in benMAPPollutant.Metrics)
                            {
                                if (m is FixedWindowMetric)
                                {
                                    fixedWindowMetric = (FixedWindowMetric)m;
                                    //------------如果是hourly，计算小时值根据staticstic,然后计算年值
                                    if (hourly == 0)//|| (fixedWindowMetric.StartHour == 0 && fixedWindowMetric.EndHour == 23))
                                    {
                                        List<float> lstMonitorValue = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                                        if (lstMonitorValue != null && lstMonitorValue.Count > 0)
                                        {
                                            switch (fixedWindowMetric.Statistic)
                                            {
                                                case MetricStatic.Max:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Max());
                                                    break;
                                                case MetricStatic.Mean:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Average());
                                                    break;
                                                case MetricStatic.Median:
                                                    lstMonitorValue.Sort();
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue[lstMonitorValue.Count / 2]);//----------错的，要重做为中间值
                                                    break;
                                                case MetricStatic.Min:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Min());
                                                    break;
                                                case MetricStatic.None:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Average());
                                                    break;
                                                case MetricStatic.Sum:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Sum());
                                                    break;
                                            }
                                            monitorValue.dicMetricValues365.Add(m.MetricName, monitorValue.Values);
                                        }
                                    }
                                    else
                                    {
                                        //----根据开始hour和结束hour再通过Statistic计算---------首先生成一个List<double>---
                                        dicHourlyValue = new Dictionary<int, List<float>>();
                                        dicHourlyValue365 = new Dictionary<int, List<float>>();
                                        i = 0;
                                        while (i < 365)
                                        {
                                            try
                                            {
                                                lstTemp = new List<float>();
                                                if (i * 24 + fixedWindowMetric.StartHour < monitorValue.Values.Count && i * 24 + fixedWindowMetric.StartHour + fixedWindowMetric.EndHour - fixedWindowMetric.StartHour < monitorValue.Values.Count)
                                                    lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour + 1);
                                                else
                                                    lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, monitorValue.Values.Count - (i * 24 + fixedWindowMetric.StartHour));
                                                lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                                                if (lstTemp != null && lstTemp.Count > 0)
                                                {
                                                    dicHourlyValue.Add(i, lstTemp);// monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour));
                                                    dicHourlyValue365.Add(i, lstTemp);
                                                }
                                                else
                                                {
                                                    dicHourlyValue365.Add(i,new List<float>(){float.MinValue});
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                            //lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour);
                                            //lstTemp.Sort();
                                            //lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour+1);
                                            //lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();//.Remove(float.MinValue);
                                            //if (lstTemp != null && lstTemp.Count > 0)
                                            //    dicHourlyValue.Add(i, lstTemp);// monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour));
                                            i++;
                                        }
                                        if (dicHourlyValue.Count == 0)
                                        {
                                            continue;
                                        }
                                        switch (fixedWindowMetric.Statistic)
                                        {
                                            case MetricStatic.Max:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Max()));

                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Max()).ToList());
                                                break;
                                            case MetricStatic.Mean:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Average()).ToList());
                                                break;
                                            case MetricStatic.Median:
                                                lstTemp = new List<float>();
                                                foreach (List<float> ld in dicHourlyValue.Values)
                                                {
                                                    //ld.Sort();
                                                    lstTemp.Add(ld.OrderBy(p => p).Median());
                                                }
                                                //lstTemp.Sort();
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p=>p).Median());//----------错的，要重做为中间值
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.OrderBy(p => p).Select(p => p.Median()).ToList());
                                                break;
                                            case MetricStatic.Min:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Min()));
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Min()).ToList());
                                                break;
                                            case MetricStatic.None:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Average()).ToList());
                                                break;
                                            case MetricStatic.Sum:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Sum()));
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Sum()).ToList());
                                                break;
                                        }
                                        //----------计算值
                                    }

                                    //------------如果不是，直接计算-----------------------
                                }
                                else if (m is MovingWindowMetric)
                                {
                                    movingWindowMetric = (MovingWindowMetric)m;
                                    //------------如果是hourly，计算小时值根据staticstic,然后计算年值
                                    if (hourly == 0 || movingWindowMetric.HourlyMetricGeneration == 1)
                                    {
                                        List<float> lstMonitorValue = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                                        if (lstMonitorValue != null && lstMonitorValue.Count > 0)
                                        {
                                            switch (movingWindowMetric.WindowStatistic)
                                            {
                                                case MetricStatic.Max:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Max());
                                                    break;
                                                case MetricStatic.Mean:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Average());
                                                    break;
                                                case MetricStatic.Median:
                                                    lstMonitorValue.Sort();
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue[lstMonitorValue.Count / 2]);//----------错的，要重做为中间值
                                                    break;
                                                case MetricStatic.Min:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Min());
                                                    break;
                                                case MetricStatic.None:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Average());
                                                    break;
                                                case MetricStatic.Sum:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Sum());
                                                    break;
                                            }
                                            monitorValue.dicMetricValues365.Add(m.MetricName, monitorValue.Values);
                                        }
                                    }
                                    else
                                    {
                                        //----根据开始hour和结束hour再通过Statistic计算---------首先生成一个List<double>---
                                        dicHourlyValue = new Dictionary<int, List<float>>();
                                        dicHourlyValue365 = new Dictionary<int, List<float>>();
                                        i = 0;
                                        //monitorValue.Values.Remove(float.MinValue);

                                        while (i < monitorValue.Values.Count / 24 + 1)
                                        {
                                            if (i < monitorValue.Values.Count / 24)
                                            {
                                                try
                                                {
                                                    lstTemp = monitorValue.Values.GetRange(i * 24, 24);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    lstTemp = monitorValue.Values.GetRange(i * 24, monitorValue.Values.Count - i * 24);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            //lstTemp.Remove(float.MinValue);
                                            //-------------------采用WindowSize------------------
                                            List<float> lstWindowSize = new List<float>();
                                            for (int iWindowSize = 0; iWindowSize <= lstTemp.Count - movingWindowMetric.WindowSize; iWindowSize++)
                                            {
                                                if (iWindowSize == lstTemp.Count - movingWindowMetric.WindowSize - 1)
                                                {
                                                    List<float> lstIn = lstTemp.GetRange(iWindowSize, lstTemp.Count - iWindowSize - 1).Where(p => p != float.MinValue).ToList();
                                                    try
                                                    {
                                                        if (lstIn.Count() > 0)
                                                            lstWindowSize.Add(lstIn.Average());
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                else
                                                {
                                                    List<float> lstIn = lstTemp.GetRange(iWindowSize, movingWindowMetric.WindowSize).Where(p => p != float.MinValue).ToList();
                                                    try
                                                    {
                                                        if (lstIn.Count() > 0)
                                                            lstWindowSize.Add(lstIn.Average());
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }

                                            }
                                            //-----------------end 采用WindowSize------------------
                                            lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                                            if (lstWindowSize != null && lstWindowSize.Count > 0)
                                            {
                                                dicHourlyValue.Add(i, lstWindowSize);
                                                dicHourlyValue365.Add(i, lstWindowSize);
                                            }
                                            else
                                            {
                                                dicHourlyValue365.Add(i, new List<float>() { float.MinValue });
 
                                            }

                                            i++;
                                        }
                                        switch (movingWindowMetric.DailyStatistic)
                                        {
                                            case MetricStatic.Max:
                                                switch (movingWindowMetric.WindowStatistic)
                                                {
                                                    case MetricStatic.Max:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Max()));
                                                        break;
                                                    case MetricStatic.Mean:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Max()));
                                                        break;
                                                    case MetricStatic.Median:
                                                        List<float> lstTempIn = new List<float>();
                                                        foreach (List<float> k in dicHourlyValue.Values)
                                                        {
                                                            lstTempIn.Add(k.Max());
                                                        }
                                                        //lstTempIn.Sort();
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                        break;
                                                    case MetricStatic.Min:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Max()));
                                                        break;
                                                    case MetricStatic.None:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Max()));
                                                        break;
                                                    case MetricStatic.Sum:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Max()));
                                                        break;

                                                }
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Max()).ToList());
                                                break;
                                            case MetricStatic.Mean:
                                                switch (movingWindowMetric.WindowStatistic)
                                                {
                                                    case MetricStatic.Max:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Average()));
                                                        break;
                                                    case MetricStatic.Mean:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                        break;
                                                    case MetricStatic.Median:
                                                        List<float> lstTempIn = new List<float>();
                                                        foreach (List<float> k in dicHourlyValue.Values)
                                                        {
                                                            lstTempIn.Add(k.Average());
                                                        }
                                                        //lstTempIn.Sort();
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                        break;
                                                    case MetricStatic.Min:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Average()));
                                                        break;
                                                    case MetricStatic.None:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                        break;
                                                    case MetricStatic.Sum:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Average()));
                                                        break;

                                                }
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Average()).ToList());
                                                break;
                                            case MetricStatic.Median:
                                                lstTemp = new List<float>();
                                                foreach (List<float> ld in dicHourlyValue.Values)
                                                {
                                                    //ld.Sort();
                                                    lstTemp.Add(ld.OrderBy(p => p).Median());
                                                }
                                                //lstTemp.Sort();
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p=>p).Median());//----------错的，要重做为中间值
                                                switch (movingWindowMetric.WindowStatistic)
                                                {
                                                    case MetricStatic.Max:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.Max());
                                                        break;
                                                    case MetricStatic.Mean:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.Average());
                                                        break;
                                                    case MetricStatic.Median:
                                                        lstTemp.Sort();
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p => p).Median());
                                                        break;
                                                    case MetricStatic.Min:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.Min());
                                                        break;
                                                    case MetricStatic.None:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.Average());
                                                        break;
                                                    case MetricStatic.Sum:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.Sum());
                                                        break;

                                                }
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.OrderBy(a=>a).Median()).ToList());
                                                break;
                                            case MetricStatic.Min:
                                                switch (movingWindowMetric.WindowStatistic)
                                                {
                                                    case MetricStatic.Max:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Min()));
                                                        break;
                                                    case MetricStatic.Mean:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Min()));
                                                        break;
                                                    case MetricStatic.Median:
                                                        List<float> lstTempIn = new List<float>();
                                                        foreach (List<float> k in dicHourlyValue.Values)
                                                        {
                                                            lstTempIn.Add(k.Min());
                                                        }
                                                        //lstTempIn.Sort();
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                        break;
                                                    case MetricStatic.Min:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Min()));
                                                        break;
                                                    case MetricStatic.None:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Min()));
                                                        break;
                                                    case MetricStatic.Sum:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Min()));
                                                        break;

                                                }
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Min()).ToList());
                                                break;
                                            case MetricStatic.None:
                                                switch (movingWindowMetric.WindowStatistic)
                                                {
                                                    case MetricStatic.Max:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Average()));
                                                        break;
                                                    case MetricStatic.Mean:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                        break;
                                                    case MetricStatic.Median:
                                                        List<float> lstTempIn = new List<float>();
                                                        foreach (List<float> k in dicHourlyValue.Values)
                                                        {
                                                            lstTempIn.Add(k.Average());
                                                        }
                                                        //lstTempIn.Sort();
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                        break;
                                                    case MetricStatic.Min:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Average()));
                                                        break;
                                                    case MetricStatic.None:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                        break;
                                                    case MetricStatic.Sum:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Average()));
                                                        break;

                                                }
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Average()).ToList());
                                                break;
                                            case MetricStatic.Sum:
                                                switch (movingWindowMetric.WindowStatistic)
                                                {
                                                    case MetricStatic.Max:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Sum()));
                                                        break;
                                                    case MetricStatic.Mean:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Sum()));
                                                        break;
                                                    case MetricStatic.Median:
                                                        List<float> lstTempIn = new List<float>();
                                                        foreach (List<float> k in dicHourlyValue.Values)
                                                        {
                                                            lstTempIn.Add(k.Sum());
                                                        }
                                                        lstTempIn.Sort();
                                                        monitorValue.dicMetricValues.Add(m.MetricName, lstTempIn.OrderBy(p => p).Median());
                                                        break;
                                                    case MetricStatic.Min:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Sum()));
                                                        break;
                                                    case MetricStatic.None:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Sum()));
                                                        break;
                                                    case MetricStatic.Sum:
                                                        monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Sum()));
                                                        break;

                                                }
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.Sum()).ToList());
                                                break;
                                        }
                                        //----------计算值
                                    }
                                }
                                else if (m is CustomerMetric)
                                {
                                    //----------------按函数来统计------------------暂不实现--------------------------需要讨论后再做---
                                }
                                else
                                {
                                    //fixedWindowMetric = (FixedWindowMetric)m;
                                    //------------如果是hourly，计算小时值根据staticstic,然后计算年值

                                    //monitorValue.Values.Remove(float.MinValue);
                                    List<float> lstMonitorValue = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                                    if (lstMonitorValue != null && lstMonitorValue.Count > 0)
                                        monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Average());
                                    else
                                        monitorValue.dicMetricValues.Add(m.MetricName, 0);
                                }
                            }
                            if (benMAPPollutant.SesonalMetrics != null && monitorValue.dicMetricValues365.Count>0)
                            {
                                //Dictionary<int, string> dicSeasonStatics = new Dictionary<int, string>();
                                //ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                                //string commandText = string.Format("select * from SEASONALMETRICSEASONS where POLLUTANTSEASONID in (select POLLUTANTSEASONID from POLLUTANTSEASONS where pollutantid= " + benMAPPollutant.PollutantID + ")");
                                //System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                                //foreach (DataRow dr in ds.Tables[0].Rows)
                                //{
                                //    dicSeasonStatics.Add(Convert.ToInt32(dr["PollutantSeasonID"]), dr["METRICFUNCTION"].ToString());
                                //}

                                foreach (SeasonalMetric seasonalmetric in benMAPPollutant.SesonalMetrics)
                                {
                                    List<float> lstQuality = new List<float>();
                                    if ((seasonalmetric.Seasons == null || seasonalmetric.Seasons.Count == 0) && monitorValue.dicMetricValues365.ContainsKey(seasonalmetric.Metric.MetricName))//如果没有Seasons则是Mean
                                    {
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Average());
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Average());
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Average());
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Average());

                                        //if (seasonalMetric.Seasons == null || seasonalMetric.Seasons.Count == 0)
                                        //{
                                        //----------------基本都包含了四个季度，而且都是Mean，所以应该一致-----------也许要修改
                                        if (monitorValue.dicMetricValues.Keys.Contains(seasonalmetric.Metric.MetricName))
                                        {
                                            monitorValue.dicMetricValues.Add(seasonalmetric.SeasonalMetricName, monitorValue.dicMetricValues[seasonalmetric.Metric.MetricName]);
                                        }
                                    }
                                    else
                                    {
                                        foreach (Season s in seasonalmetric.Seasons)
                                        {
                                            if (!DicSeasonStaticsAll.ContainsKey(s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()))
                                                _dicSeasonStaticsAll = null;
                                            switch (DicSeasonStaticsAll[s.StartDay.ToString()+"," + seasonalmetric.SeasonalMetricID.ToString()])
                                            {
                                                case "":
                                                case "Mean":
                                                    lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count<365?monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Average():monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                        float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Average());
                                                    break;
                                                case "Median":
                                                    lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].OrderBy(p=>p).Median() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                        float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).OrderBy(p=>p).Median());
                                                    break;
                                                case "Max":
                                                    lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Max() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                        float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Max());
                                                    break;
                                                case "Min":
                                                    lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Min() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                        float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Min());
                                                    break;
                                                case "Sum":
                                                    lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Sum() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                        float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Sum());
                                                    break;

                                            }

                                        }
                                        if (monitorValue.dicMetricValues.Keys.Contains(seasonalmetric.Metric.MetricName))
                                        {
                                            if (lstQuality.Where(p => p != float.MinValue).Count() > 0)
                                                monitorValue.dicMetricValues.Add(seasonalmetric.SeasonalMetricName, lstQuality.Where(p => p != float.MinValue).Average());
                                        }
                                       
                                    }
                                    monitorValue.dicMetricValues365.Add(seasonalmetric.SeasonalMetricName, lstQuality);
                                    //}
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

        public static double getDistanceFrom2Point(DotSpatial.Topology.Point start, DotSpatial.Topology.Point end)
        {
            //double EarthRadius = 6378.137; //kilometers. Change to 3960 miles to return all values in miles instead
            //double lon1 = start.X / 180 * Math.PI;
            //double lon2 = end.X / 180 * Math.PI;
            //double lat1 = start.Y / 180 * Math.PI;
            //double lat2 = end.Y / 180 * Math.PI;
            return 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((start.Y / 180 * Math.PI - end.Y / 180 * Math.PI) / 2)), 2) +
             Math.Cos(start.Y / 180 * Math.PI) * Math.Cos(end.Y / 180 * Math.PI) * Math.Pow(Math.Sin((start.X / 180 * Math.PI - end.X / 180 * Math.PI) / 2), 2))) * 6371.000;
        }

        public static float getDistanceFrom2Point(double X0, double Y0, double X1, double Y1)
        {
            //double EarthRadius = 6378.137; //kilometers. Change to  1.60931 3960 miles to return all values in miles instead
            //double lon1 = start.X / 180 * Math.PI;
            //double lon2 = end.X / 180 * Math.PI;
            //double lat1 = start.Y / 180 * Math.PI;
            //double lat2 = end.Y / 180 * Math.PI;
            //return Math.Sqrt(Math.Pow(X1 - X0, 2) + Math.Pow(Y1 - Y0, 2)) ;
            return Convert.ToSingle(2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((Y0 / 180 * Math.PI - Y1 / 180 * Math.PI) / 2)), 2) +
             Math.Cos(Y0 / 180 * Math.PI) * Math.Cos(Y1 / 180 * Math.PI) * Math.Pow(Math.Sin((X0 / 180 * Math.PI - X1 / 180 * Math.PI) / 2), 2))) * 6371.000);
        }

        public static double getDistanceFromExtent(DotSpatial.Topology.Coordinate coordinate, DotSpatial.Topology.IEnvelope env, DotSpatial.Topology.Point end)
        {
            //if (env.ToExtent().Contains(end.Envelope))
            //    return 0.0;
            //else
            //{
            double d = Math.Sqrt((coordinate.X - end.X) * (coordinate.X - end.X) + (coordinate.Y - end.Y) * (coordinate.Y - end.Y)) * 111.0000;// getDistanceFrom2Point(new DotSpatial.Topology.Point(env.ToExtent().Center.X, env.ToExtent().Center.Y), end);
            if (d < env.Height / 2.00 && d < env.Width / 2.00)
            {
                return 0.0;
            }
            else
            {
                return env.Height > env.Width ? d - env.Width / 2.00 : d - env.Height;
            }
            //}
        }

        private static void HandleBoundaries(VoronoiGraph graph, IEnvelope bounds)
        {
            List<ILineString> boundSegments = new List<ILineString>();
            List<VoronoiEdge> unboundEdges = new List<VoronoiEdge>();

            // Identify bound edges for intersection testing
            foreach (VoronoiEdge edge in graph.Edges)
            {
                if (edge.VVertexA.ContainsNan() || edge.VVertexB.ContainsNan())
                {
                    unboundEdges.Add(edge);
                    continue;
                }

                boundSegments.Add(new LineString(new List<Coordinate> { edge.VVertexA.ToCoordinate(), edge.VVertexB.ToCoordinate() }));
            }

            // calculate a length to extend a ray to look for intersections
            IEnvelope env = bounds;
            double h = env.Height;
            double w = env.Width;
            double len = Math.Sqrt((w * w) + (h * h));  // len is now long enough to pass entirely through the dataset no matter where it starts

            foreach (VoronoiEdge edge in unboundEdges)
            {
                // the unbound line passes thorugh start
                Coordinate start = (edge.VVertexB.ContainsNan())
                                        ? edge.VVertexA.ToCoordinate()
                                        : edge.VVertexB.ToCoordinate();

                // the unbound line should have a direction normal to the line joining the left and right source points
                double dx = edge.LeftData.X - edge.RightData.X;
                double dy = edge.LeftData.Y - edge.RightData.Y;
                double l = Math.Sqrt((dx * dx) + (dy * dy));

                // the slope of the bisector between left and right
                double sx = -dy / l;
                double sy = dx / l;

                Coordinate center = bounds.Center();
                if ((start.X > center.X && start.Y > center.Y) || (start.X < center.X && start.Y < center.Y))
                {
                    sx = dy / l;
                    sy = -dx / l;
                }

                Coordinate end1 = new Coordinate(start.X + (len * sx), start.Y + (len * sy));
                Coordinate end2 = new Coordinate(start.X - (sx * len), start.Y - (sy * len));
                Coordinate end = (end1.Distance(center) < end2.Distance(center)) ? end2 : end1;
                if (bounds.Contains(end))
                {
                    end = new Coordinate(start.X - (sx * len), start.Y - (sy * len));
                }

                if (edge.VVertexA.ContainsNan())
                {
                    edge.VVertexA = new Vector2(end.ToArray());
                }
                else
                {
                    edge.VVertexB = new Vector2(end.ToArray());
                }
            }
        }

        public static void VoronoiPolygons(IFeatureSet points, ref List<Polygon> result,IEnvelope envBounds)
        {
            result = new List<Polygon>();
            double[] vertices = points.Vertex;
            VoronoiGraph gp = Fortune.ComputeVoronoiGraph(vertices);

            Extent ext = points.Extent;
            ext.ExpandBy(ext.Width / 100, ext.Height / 100);
            IEnvelope env = ext.ToEnvelope();
            IPolygon bounds = envBounds.ToPolygon();

            // Convert undefined coordinates to a defined coordinate.
            HandleBoundaries(gp, env);
            VoronoiEdge firstEdge = null;
            Vector2 previous = new Vector2();
            Vector2 start = new Vector2();
            List<VoronoiEdge> myEdges = new List<VoronoiEdge>();
            List<Coordinate> coords = new List<Coordinate>();
            Vector2 v = new Vector2();
            for (int i = 0; i < vertices.Length / 2; i++)
            {
                myEdges = new List<VoronoiEdge>();
                v = new Vector2(vertices, i * 2);
                foreach (VoronoiEdge edge in gp.Edges)
                {
                    if (!v.Equals(edge.RightData) && !v.Equals(edge.LeftData))
                    {
                        continue;
                    }
                    myEdges.Add(edge);
                }
                coords = new List<Coordinate>();
                firstEdge = myEdges[0];
                coords.Add(firstEdge.VVertexA.ToCoordinate());
                coords.Add(firstEdge.VVertexB.ToCoordinate());
                previous = firstEdge.VVertexB;
                myEdges.Remove(myEdges[0]);
                start = firstEdge.VVertexA;
                while (myEdges.Count > 0)
                {
                    for (int j = 0; j < myEdges.Count; j++)
                    {
                        //VoronoiEdge edge = myEdges[j];
                        if (myEdges[j].VVertexA.Equals(previous))
                        {
                            previous = myEdges[j].VVertexB;
                            //Coordinate c = previous.ToCoordinate();
                            coords.Add(previous.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        // couldn't match by adding to the end, so try adding to the beginning
                        if (myEdges[j].VVertexB.Equals(start))
                        {
                            start = myEdges[j].VVertexA;
                            coords.Insert(0, start.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        // I don't like the reverse situation, but it seems necessary.
                        if (myEdges[j].VVertexB.Equals(previous))
                        {
                            previous = myEdges[j].VVertexA;
                            //Coordinate c = previous.ToCoordinate();
                            coords.Add(previous.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        if (myEdges[j].VVertexA.Equals(start))
                        {
                            start = myEdges[j].VVertexB;
                            coords.Insert(0, start.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }
                    }
                }
                for (int j = 0; j < coords.Count; j++)
                {
                    //Coordinate cA = coords[j];

                    // Remove NAN values
                    if (double.IsNaN(coords[j].X) || double.IsNaN(coords[j].Y))
                    {
                        coords.Remove(coords[j]);
                    }

                    // Remove duplicate coordinates
                    for (int k = j + 1; k < coords.Count; k++)
                    {
                        //Coordinate cB = coords[k];
                        if (coords[j].Equals2D(coords[k])) { coords.Remove(coords[k]); }
                    }
                }
                foreach (Coordinate coord in coords)
                {
                    if (double.IsNaN(coord.X) || double.IsNaN(coord.Y))
                    {
                        coords.Remove(coord);
                    }
                }
                if (coords.Count <= 2) { continue; }

                Polygon pg = new Polygon(coords);
               // result.Add(new Polygon(coords));
                try
                {
                    IGeometry g = pg.Intersection(bounds);
                    Polygon p = g as Polygon;
                    if (p != null)
                    {
                        //Feature f = new Feature(p, result);
                        result.Add(p);
                        //f.CopyAttributes(points.Features[i]);
                    }
                }
                catch (Exception)
                {
                    
                }
                //Feature f = new Feature(pg, result);
            }
            return;
        }
        public static void VoronoiPointsNew(double[] vertices, ref List<Polygon> result)
        {
            result = new List<Polygon>();
            VoronoiGraph gp = Fortune.ComputeVoronoiGraph(vertices);
            double XMin = vertices[0], YMin = vertices[1], XMax = vertices[0], YMax = vertices[1];
            //List<Coordinate> lstCoordinate = new List<Coordinate>();
            for (int i = 0; i < vertices.Length / 2; i++)
            {
                if (vertices[i] < XMin) XMin = vertices[i];
                if (vertices[i+1] < YMin) YMin = vertices[i+1];
                if (vertices[i] > XMax) XMax = vertices[i];
                if (vertices[i+1] > YMax) YMax = vertices[i+1];

            }
            Extent ext = new Extent(XMin, YMin, XMax, YMax);
            ext.ExpandBy(ext.Width / 100, ext.Height / 100);
            IEnvelope env = ext.ToEnvelope();
            IPolygon bounds = env.ToPolygon();

            // Convert undefined coordinates to a defined coordinate.
            HandleBoundaries(gp, env);
            VoronoiEdge firstEdge = null;
            Vector2 previous = new Vector2();
            Vector2 start = new Vector2();
            List<VoronoiEdge> myEdges = new List<VoronoiEdge>();
            List<Coordinate> coords = new List<Coordinate>();
            Vector2 v = new Vector2();
            for (int i = 0; i < vertices.Length / 2; i++)
            {
                myEdges = new List<VoronoiEdge>();
                v = new Vector2(vertices, i * 2);
                foreach (VoronoiEdge edge in gp.Edges)
                {
                    if (!v.Equals(edge.RightData) && !v.Equals(edge.LeftData))
                    {
                        continue;
                    }
                    myEdges.Add(edge);
                }
                coords = new List<Coordinate>();
                firstEdge = myEdges[0];
                coords.Add(firstEdge.VVertexA.ToCoordinate());
                coords.Add(firstEdge.VVertexB.ToCoordinate());
                previous = firstEdge.VVertexB;
                myEdges.Remove(myEdges[0]);
                start = firstEdge.VVertexA;
                while (myEdges.Count > 0)
                {
                    for (int j = 0; j < myEdges.Count; j++)
                    {
                        //VoronoiEdge edge = myEdges[j];
                        if (myEdges[j].VVertexA.Equals(previous))
                        {
                            previous = myEdges[j].VVertexB;
                            //Coordinate c = previous.ToCoordinate();
                            coords.Add(previous.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        // couldn't match by adding to the end, so try adding to the beginning
                        if (myEdges[j].VVertexB.Equals(start))
                        {
                            start = myEdges[j].VVertexA;
                            coords.Insert(0, start.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        // I don't like the reverse situation, but it seems necessary.
                        if (myEdges[j].VVertexB.Equals(previous))
                        {
                            previous = myEdges[j].VVertexA;
                            //Coordinate c = previous.ToCoordinate();
                            coords.Add(previous.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        if (myEdges[j].VVertexA.Equals(start))
                        {
                            start = myEdges[j].VVertexB;
                            coords.Insert(0, start.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }
                    }
                }
                for (int j = 0; j < coords.Count; j++)
                {
                    //Coordinate cA = coords[j];

                    // Remove NAN values
                    if (double.IsNaN(coords[j].X) || double.IsNaN(coords[j].Y))
                    {
                        coords.Remove(coords[j]);
                    }

                    // Remove duplicate coordinates
                    for (int k = j + 1; k < coords.Count; k++)
                    {
                        //Coordinate cB = coords[k];
                        if (coords[j].Equals2D(coords[k])) { coords.Remove(coords[k]); }
                    }
                }
                foreach (Coordinate coord in coords)
                {
                    if (double.IsNaN(coord.X) || double.IsNaN(coord.Y))
                    {
                        coords.Remove(coord);
                    }
                }
                if (coords.Count <= 2) { continue; }

                //Polygon pg = new Polygon(coords);

                result.Add(new Polygon(coords));

                //Feature f = new Feature(pg, result);
            }
             
        }
        public static void VoronoiPoints(double[] vertices, ref List<double> result)//,List<KeyValuePair<Coordinate,double>> ikey)
        {
            VoronoiGraph gp = Fortune.ComputeVoronoiGraph(vertices);

            foreach (VoronoiEdge edge in gp.Edges)
            {
                if (vertices[0] == edge.RightData.X && vertices[1] == edge.RightData.Y) //|| (vertices[0] == edge.LeftData.X && vertices[1] == edge.LeftData.Y))
                {
                    result.Add(edge.LeftData.X);
                    result.Add(edge.LeftData.Y);
                }
                else if (vertices[0] == edge.LeftData.X && vertices[1] == edge.LeftData.Y)
                {
                    result.Add(edge.RightData.X);
                    result.Add(edge.RightData.Y);
                }
            }
            //foreach (VoronoiEdge edge in myEdgesFirst)
            //{
            //    if (edge.LeftData.X != vertices[0] && edge.LeftData.Y != vertices[1])
            //    {
            //        result.Add(edge.LeftData.X);
            //        result.Add(edge.LeftData.Y);
            //    }
            //    if (edge.RightData.X != vertices[0] && edge.RightData.Y != vertices[1])
            //    {
            //        result.Add(edge.RightData.X);
            //        result.Add(edge.RightData.Y);
            //    }
            //}
            //result.Remove(new Coordinate(vertices[0], vertices[1]));

            //for (int i = 2; i < vertices.Count()/2; i++)
            //{
            //        foreach (VoronoiEdge edge in gp.Edges)
            //        {
            //            if ((vertices[i * 2] == edge.RightData.X && vertices[i * 2 + 1] == edge.RightData.Y) || (vertices[i * 2] == edge.LeftData.X && vertices[i * 2 + 1] == edge.LeftData.Y))
            //            {
            //                if (myEdgesFirst.Contains(edge))
            //                {
            //                    result.Add(new Coordinate(vertices[i * 2], vertices[i * 2 + 1]));
            //                    // i = vertices.Length / 2;
            //                    break;
            //                }
            //            }
            //        }

            //}
            //return result;
        }
        public static bool isValidValue(List<float> lstFloat)
        {
            int iValid = 0;
            for (int i = 0; i < lstFloat.Count; i++)
            {
                if (lstFloat[i] == float.MinValue)
                {
                    iValid = 0;
                }
                else
                    iValid++;
            }
            if (iValid >= 11) return true;
            return false;
        }
        public static List<MonitorValue> GetMonitorData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, MonitorDataLine monitorDataLine)
        {
            try
            {
                int i = 0;
                List<MonitorValue> lstMonitorValues = new List<MonitorValue>();
                Dictionary<int,int> dicPOCOrder=new Dictionary<int,int>();
                if (monitorDataLine.MonitorAdvance != null && !string.IsNullOrEmpty(monitorDataLine.MonitorAdvance.POCPreferenceOrder))
                {
                    string[] sPOC = monitorDataLine.MonitorAdvance.POCPreferenceOrder.Split(',');
                    for (int p = 0; p < sPOC.Count(); p++)
                    {
                        if (!string.IsNullOrEmpty(sPOC[p]) && !dicPOCOrder.ContainsKey(Convert.ToInt16(sPOC[p])))
                        {
                            dicPOCOrder.Add(Convert.ToInt16(sPOC[p]), p);
                        }
                    }
                }
                //首先获取Monitor的Attributes
                //Monitor的存储方法，用byte[] byteArray = System.Text.Encoding.Default.GetBytes ( str ); To MONITORENTRIES 表
                //string str = System.Text.Encoding.Default.GetString ( byteArray );
                //-------get dictionary of metric and seasonalmetric
                Dictionary<string, Metric> dicMetric = new Dictionary<string, Metric>();
                foreach (Metric m in benMAPPollutant.Metrics)
                {
                    dicMetric.Add(m.MetricName, m);
                }
                Dictionary<string, SeasonalMetric> dicSeasonalMetric = new Dictionary<string, SeasonalMetric>();
                foreach (SeasonalMetric m in benMAPPollutant.SesonalMetrics)
                {
                    dicSeasonalMetric.Add(m.SeasonalMetricName, m);
                }
                string commandText = "";
                MonitorValue mv = new MonitorValue();
                if (monitorDataLine.MonitorDirectType == 0)//Library
                {
                    commandText = string.Format("select a.MonitorEntryID,a.MonitorID,a.YYear,a.MetricID,a.SeasonalMetricID,a.Statistic,a.VValues,b.PollutantID,b.Latitude,b.Longitude,b.MonitorName,b.MonitorDescription from MonitorEntries a,Monitors b,MonitorDataSets c where a.MonitorID=b.MonitorID and b.MonitorDataSetID=c.MonitorDataSetID and b.PollutantID={0} and c.MonitorDataSetID={1} and a.YYear={2} ", benMAPPollutant.PollutantID, monitorDataLine.MonitorDataSetID, monitorDataLine.MonitorLibraryYear);//----------------------------------------------
                    //----得到所有Monitor的值
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    Byte[] blob = null;
                    lstMonitorValues = new List<MonitorValue>();

                    string str = "";
                    string[] strArray = null;
                    //DataTable dtO = new DataTable();
                    //dtO.Columns.Add("name");
                    //dtO.Columns.Add("Dest");
                    //dtO.Columns.Add("lon");
                    //dtO.Columns.Add("lat");
                    monitorDataLine.MonitorNeighbors = new List<MonitorNeighborAttribute>();
                    while (fbDataReader.Read())
                    {
                        mv = new MonitorValue();

                        //blob = new Byte[(fbDataReader.GetBytes(6, 0, null, 0, int.MaxValue))];
                        //fbDataReader.GetBytes(0, 0, blob, 0, blob.Length);
                        blob = fbDataReader[6] as byte[];
                        // object test = DeserializeObject(blob);
                        str = System.Text.Encoding.Default.GetString(blob);
                        strArray = str.Split(new char[] { ',' });
                        mv.MonitorID = Convert.ToInt32(fbDataReader["MonitorID"]);
                        mv.Latitude = Convert.ToDouble(fbDataReader["Latitude"]);
                        mv.Longitude = Convert.ToDouble(fbDataReader["Longitude"]);
                        mv.MonitorName = fbDataReader["MonitorName"].ToString();
                        mv.MonitorMethod = fbDataReader["MonitorDescription"].ToString();
                        if (!(fbDataReader["MetricID"] is DBNull))
                        {
                            //mv.Metric = Grid.GridCommon.getMetricFromID(Convert.ToInt32(fbDataReader["MetricID"]));
                            for (int m = 0; m < benMAPPollutant.Metrics.Count; m++)
                            {
                                if (Convert.ToInt32(fbDataReader["MetricID"]) == benMAPPollutant.Metrics[m].MetricID)
                                {
                                    mv.Metric = benMAPPollutant.Metrics[m];
                                    break;
                                }
                            }
                        }
                        if (!(fbDataReader["SeasonalMetricID"] is DBNull))
                        {
                            //mv.SeasonalMetric = Grid.GridCommon.getSeasonalMetric(Convert.ToInt32(fbDataReader["SeasonalMetricID"]));
                            for (int m = 0; m < benMAPPollutant.Metrics.Count; m++)
                            {
                                if (Convert.ToInt32(fbDataReader["SeasonalMetricID"]) == benMAPPollutant.SesonalMetrics[m].SeasonalMetricID)
                                {
                                    mv.SeasonalMetric = benMAPPollutant.SesonalMetrics[m];
                                    break;
                                }
                            }
                        } 
                        mv.Statistic = fbDataReader["Statistic"].ToString();
                        mv.Values = new List<float>();
                        foreach (string s in strArray)
                        {
                            if (string.IsNullOrEmpty(s) || s.Trim() == ".")
                            {
                                mv.Values.Add(Convert.ToSingle(float.MinValue));
                            }
                            else
                            {
                                mv.Values.Add(Convert.ToSingle(s));
                            }
                        }
                        int iPOC = -1;
                        try
                        {
                            if (!string.IsNullOrEmpty(mv.MonitorMethod))
                            {
                                if (mv.MonitorMethod.Contains("POC=.") || mv.MonitorMethod.Contains("POC=\'"))
                                    iPOC = 1;
                                else
                                    iPOC = Convert.ToInt16(mv.MonitorMethod.Substring(mv.MonitorMethod.IndexOf("POC=") + 4, mv.MonitorMethod.IndexOf('\'', mv.MonitorMethod.IndexOf("POC=") + 4) - mv.MonitorMethod.IndexOf("POC=") - 4));
                            }
                        }
                        catch
                        {
                        }
                        //------------modify by xiejp 首先判断是否有Seasons，然后每个Seasons里面需要有NumBin的数据
                        //-----------写死每个Pollutant的MethodCode和POC
                        //-----------经纬度范围如果是美国那么固定------------
                        //----------同一经纬度取POC最大值---------
                        bool isValid = true;
                        if (CommonClass.MainSetup.SetupID == 1 && benMAPPollutant.Seasons != null && (benMAPPollutant.PollutantName.ToLower() == "pm2.5" || benMAPPollutant.PollutantName.ToLower() == "pm10") && mv.Values.Count == 365)
                        {
                            foreach (Season s in benMAPPollutant.Seasons)
                            {
                                int iPerQuarter = 11;
                                if (monitorDataLine.MonitorAdvance != null) iPerQuarter = monitorDataLine.MonitorAdvance.NumberOfPerQuarter;
                                if (mv.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Count(p => p != float.MinValue) < iPerQuarter)
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                        }
                        if (!isValid) continue;
                        if (mv.MonitorName == "80290004881011")
                        { 
                        }
                        
                        //-----------修正--以POC为准----------------
                        if (CommonClass.MainSetup.SetupID == 1 && monitorDataLine.MonitorAdvance==null)
                        {
                            switch (benMAPPollutant.PollutantName.ToLower())
                            {
                                case "pm2.5":
                                    if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                                    {
                                        //if ((mv.MonitorMethod.Contains("MethodCode=116") || mv.MonitorMethod.Contains("MethodCode=117") || mv.MonitorMethod.Contains("MethodCode=118") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=119") || mv.MonitorMethod.Contains("MethodCode=120")) && (
                                        //    mv.MonitorMethod.Contains("POC=1") || mv.MonitorMethod.Contains("POC=2") ||
                                        //    mv.MonitorMethod.Contains("POC=3") || mv.MonitorMethod.Contains("POC=4")))
                                        if (string.IsNullOrEmpty(mv.MonitorMethod) || (
                                            iPOC == 1 || iPOC == 2 ||
                                            iPOC == 3 || iPOC == 4))
                                        {

                                        }
                                        else
                                            continue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    break;
                                case "ozone":
                                    if (mv.Values.Count > 8700)
                                    {
                                        //---
                                        List<float> lstFloatTemp = new List<float>();
                                        for (int iMV = 0; iMV < mv.Values.Count / 24; iMV++)
                                        {
                                            try
                                            {
                                                if (mv.Values.GetRange(iMV * 24 + 8, 19 - 8 + 1).Count(p => p != float.MinValue) < 11)
                                                {
                                                    lstFloatTemp.Add(float.MinValue);
                                                }
                                                else
                                                    lstFloatTemp.Add(1);
                                            }
                                            catch
                                            {
                                                lstFloatTemp.Add(float.MinValue);

                                            }
                                        }
                                        if (lstFloatTemp.GetRange(120,272-120+1).Where(p => p != float.MinValue).Count() < lstFloatTemp.GetRange(120,272-120+1).Count / 2) continue;

                                    }
                                    else
                                    {
                                        if (mv.Values.GetRange(120,272-120+1).Where(p => p != float.MinValue).Count() < mv.Values.GetRange(120,272-120+1).Count / 2) continue;
 
                                    }
                                    if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                                    {
                                        //if ((mv.MonitorMethod.Contains("MethodCode=003") || mv.MonitorMethod.Contains("MethodCode=047") || mv.MonitorMethod.Contains("MethodCode=091") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=011") || mv.MonitorMethod.Contains("MethodCode=053") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=103") || mv.MonitorMethod.Contains("MethodCode=014") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=056") || mv.MonitorMethod.Contains("MethodCode=112") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=019") || mv.MonitorMethod.Contains("MethodCode=078") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=020") || mv.MonitorMethod.Contains("MethodCode=087")) && (
                                        //    mv.MonitorMethod.Contains("POC=1") || mv.MonitorMethod.Contains("POC=2") ||
                                        //    mv.MonitorMethod.Contains("POC=3") || mv.MonitorMethod.Contains("POC=4")))
                                        if (string.IsNullOrEmpty(mv.MonitorMethod) || (
                                            iPOC == 1 || iPOC == 2 ||
                                            iPOC == 3 || iPOC == 4))
                                        {

                                        }
                                        else
                                            continue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    break;
                                case "pm10":
                                    if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                                    {
                                        //if ((mv.MonitorMethod.Contains("MethodCode=062") || mv.MonitorMethod.Contains("MethodCode=065") || mv.MonitorMethod.Contains("MethodCode=076") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=063") || mv.MonitorMethod.Contains("MethodCode=071") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=079") || mv.MonitorMethod.Contains("MethodCode=064") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=073") || mv.MonitorMethod.Contains("MethodCode=081")) && (
                                        //    mv.MonitorMethod.Contains("POC=1") || mv.MonitorMethod.Contains("POC=2") ||
                                        //    mv.MonitorMethod.Contains("POC=3") || mv.MonitorMethod.Contains("POC=4")))
                                            if ((
                                            iPOC == 1 || iPOC == 2 ||
                                            iPOC == 3 || iPOC == 4))
                                            {

                                        }
                                        else
                                            continue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    break;
                                case "so2":
                                    if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                                    {
                                        //if ((mv.MonitorMethod.Contains("MethodCode=009") || mv.MonitorMethod.Contains("MethodCode=061") || mv.MonitorMethod.Contains("MethodCode=020") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=075") || mv.MonitorMethod.Contains("MethodCode=023") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=077") || mv.MonitorMethod.Contains("MethodCode=039") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=092") || mv.MonitorMethod.Contains("MethodCode=060") || mv.MonitorMethod.Contains("MethodCode=100")) && (
                                        //    mv.MonitorMethod.Contains("POC=1") || mv.MonitorMethod.Contains("POC=2") ||
                                        //    mv.MonitorMethod.Contains("POC=3") || mv.MonitorMethod.Contains("POC=4") ||
                                        //    mv.MonitorMethod.Contains("POC=5") || mv.MonitorMethod.Contains("POC=6") ||
                                        //    mv.MonitorMethod.Contains("POC=7") || mv.MonitorMethod.Contains("POC=8") || mv.MonitorMethod.Contains("POC=9")))
                                        //{
                                        if (string.IsNullOrEmpty(mv.MonitorMethod) || (
                                            iPOC == 1 || iPOC == 2 || iPOC == 3 || iPOC == 4 ||
                                            iPOC == 5 || iPOC == 6 || iPOC == 7 || iPOC == 8 || iPOC == 9))
                                        {
                                        }
                                        else
                                            continue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    break;
                                case "no2":
                                    if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                                    {
                                        //if ((mv.MonitorMethod.Contains("MethodCode=014") || mv.MonitorMethod.Contains("MethodCode=042") || mv.MonitorMethod.Contains("MethodCode=090") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=022") || mv.MonitorMethod.Contains("MethodCode=074") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=099") || mv.MonitorMethod.Contains("MethodCode=025") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=075") || mv.MonitorMethod.Contains("MethodCode=102") || mv.MonitorMethod.Contains("MethodCode=035") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=082") || mv.MonitorMethod.Contains("MethodCode=111") ||
                                        //    mv.MonitorMethod.Contains("MethodCode=037") || mv.MonitorMethod.Contains("MethodCode=089")) && (
                                        //    mv.MonitorMethod.Contains("POC=1") || mv.MonitorMethod.Contains("POC=2") ||
                                        //    mv.MonitorMethod.Contains("POC=3") || mv.MonitorMethod.Contains("POC=4") ||
                                        //    mv.MonitorMethod.Contains("POC=5") || mv.MonitorMethod.Contains("POC=6") ||
                                        //    mv.MonitorMethod.Contains("POC=7") || mv.MonitorMethod.Contains("POC=8") || mv.MonitorMethod.Contains("POC=9")))
                                        if (string.IsNullOrEmpty(mv.MonitorMethod) || (
                                            iPOC == 1 || iPOC == 2 || iPOC == 3 || iPOC == 4 ||
                                            iPOC == 5 || iPOC == 6 || iPOC == 7 || iPOC == 8 || iPOC == 9))
                                        {

                                        }
                                        else
                                            continue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    break;
                            }
                        }
                        else if (CommonClass.MainSetup.SetupID == 1 && monitorDataLine.MonitorAdvance != null)
                        {
                            if (monitorDataLine.MonitorAdvance.FilterIncludeIDs != null && monitorDataLine.MonitorAdvance.FilterIncludeIDs.Count > 0)
                            {
                                bool isInclude = false;
                                foreach (string include in monitorDataLine.MonitorAdvance.FilterIncludeIDs)
                                {
                                    if (mv.MonitorName == include)
                                    {
                                        if (lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).Count() > 0)
                                        {
                                            lstMonitorValues.RemoveAll(p => { if (p.Longitude == mv.Longitude && p.Latitude == mv.Latitude) { return true; } else { return false; } });
                                        }
                                        lstMonitorValues.Add(mv);
                                        isInclude = true;
                                        break;
                                    }
                                }
                                if (isInclude) continue;
                            }

                            if (monitorDataLine.MonitorAdvance.FilterExcludeIDs != null && monitorDataLine.MonitorAdvance.FilterExcludeIDs.Count > 0)
                            {
                                bool isexclude = false;
                                foreach (string exclude in monitorDataLine.MonitorAdvance.FilterExcludeIDs)
                                {
                                    if (mv.MonitorName == exclude)
                                    {
                                        isexclude = true;
                                        break;
                                    }
                                }
                                if (isexclude) continue;
                            }

                            if (!((Convert.ToDouble(mv.Latitude) >= monitorDataLine.MonitorAdvance.FilterMinLatitude) && (Convert.ToDouble(mv.Latitude) <= monitorDataLine.MonitorAdvance.FilterMaxLatitude) && (Convert.ToDouble(mv.Longitude) <= monitorDataLine.MonitorAdvance.FilterMaxLongitude) && (mv.Longitude) >= monitorDataLine.MonitorAdvance.FilterMinLongitude))
                            {
                                continue;
                            }
                            if (monitorDataLine.MonitorAdvance.IncludeMethods != null && monitorDataLine.MonitorAdvance.IncludeMethods.Count() > 0)
                            {
                                bool bValidMethod = false;
                                foreach (string s in monitorDataLine.MonitorAdvance.IncludeMethods)
                                {
                                    if (mv.MonitorMethod.Contains("MethodCode=" + s))
                                    {
                                        bValidMethod = true;
                                        break;
                                    }
                                }
                                if (bValidMethod == false) continue;
                            }
                            else if (monitorDataLine.MonitorAdvance.IncludeMethods != null && monitorDataLine.MonitorAdvance.IncludeMethods.Count() == 0 && !string.IsNullOrEmpty(mv.MonitorMethod))
                            {
                                continue;
                            }
                            if (iPOC > monitorDataLine.MonitorAdvance.FilterMaximumPOC && monitorDataLine.MonitorAdvance.FilterMaximumPOC != -1)
                            {
                                continue;
                            }
                            //if (monitorDataLine.MonitorAdvance.POCPreferenceOrder != null && monitorDataLine.MonitorAdvance.POCPreferenceOrder != "")
                            //{
                            //    bool bValidPOC = false;
                            //    string[] sPOC = monitorDataLine.MonitorAdvance.POCPreferenceOrder.Split(new char[] { ',' });
                            //    foreach (string s in sPOC)
                            //    {
                            //        if (mv.MonitorMethod.Contains("POC=" + s))
                            //        {
                            //            bValidPOC = true;
                            //            break;
                            //        }
                            //    }
                            //    if (bValidPOC == false) continue;

                            //}
                            //-----------OnlyForOzone---------
                            if (benMAPPollutant.PollutantName.ToLower() == "ozone")
                            {
                                if (mv.Values.Count > 8700)
                                {
                                    //---
                                    List<float> lstFloatTemp = new List<float>();
                                    for (int iMV = 0; iMV < mv.Values.Count / 24; iMV++)
                                    {
                                        try
                                        {
                                            if (mv.Values.GetRange(iMV * 24 + monitorDataLine.MonitorAdvance.StartHour, monitorDataLine.MonitorAdvance.EndHour - monitorDataLine.MonitorAdvance.StartHour + 1).Count(p => p != float.MinValue) < monitorDataLine.MonitorAdvance.NumberOfValidHour)
                                            {
                                                lstFloatTemp.Add(float.MinValue);
                                            }
                                            else
                                                lstFloatTemp.Add(1);
                                        }
                                        catch
                                        {
                                            lstFloatTemp.Add(float.MinValue);

                                        }
                                    }
                                    if (lstFloatTemp.GetRange(monitorDataLine.MonitorAdvance.StartDate, monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1).Where(p => p != float.MinValue).Count() < (monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1) * monitorDataLine.MonitorAdvance.PercentOfValidDays / 100) continue;

                                }
                                else
                                {
                                    if (mv.Values.GetRange(monitorDataLine.MonitorAdvance.StartDate, monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1).Where(p => p != float.MinValue).Count() < (monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1) * monitorDataLine.MonitorAdvance.PercentOfValidDays / 100) continue;

                                }

                            }
                        }

                        if (lstMonitorValues.Where(p => p.MonitorName == mv.MonitorName).Count() == 0 && lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).Count() == 0)
                        {
                            while (mv.MonitorName.Trim().Length < 15)
                            {
                                mv.MonitorName = "0" + mv.MonitorName;

                            }
                            lstMonitorValues.Add(mv);
                            //DataRow dr = dtO.NewRow();
                            //dr[0] = mv.MonitorName;
                            //dr[1] = mv.MonitorMethod;
                            //dr[2] = mv.Longitude;
                            //dr[3] = mv.Latitude;
                            //dtO.Rows.Add(dr);
                        }
                        else if (lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).Count() > 0)
                        {
                            if (string.IsNullOrEmpty(mv.MonitorMethod) || string.IsNullOrEmpty(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod))
                            { continue; }
                            int iPOCold = -1;
                            try
                            {
                                iPOCold = Convert.ToInt16(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod.Substring(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod.IndexOf("POC=") + 4, lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod.IndexOf('\'', lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod.IndexOf("POC=") + 4) - lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod.IndexOf("POC=") - 4));
                            }
                            catch
                            {
                            }
                            if (dicPOCOrder.ContainsKey(iPOC) && dicPOCOrder.ContainsKey(iPOCold))
                            {
                                if (dicPOCOrder[iPOC] < dicPOCOrder[iPOCold])
                                {
                                    bool isInclude = false;
                                    if (monitorDataLine.MonitorAdvance.FilterIncludeIDs != null && monitorDataLine.MonitorAdvance.FilterIncludeIDs.Count > 0)
                                    {
                                        foreach (string include in monitorDataLine.MonitorAdvance.FilterIncludeIDs)
                                        {
                                            if (lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorName == include)
                                            {
                                                isInclude = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (isInclude) continue;
                                    else
                                    {
                                        lstMonitorValues.Remove(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0]);
                                        lstMonitorValues.Add(mv);
                                    }
                                }
                                else
                                    continue;
                            }
                            else if (!dicPOCOrder.ContainsKey(iPOC) && dicPOCOrder.ContainsKey(iPOCold))
                            {
                                continue;
                            }
                            else if (dicPOCOrder.ContainsKey(iPOC) && !dicPOCOrder.ContainsKey(iPOCold))
                            {
                                bool isInclude = false;
                                if (monitorDataLine.MonitorAdvance.FilterIncludeIDs != null && monitorDataLine.MonitorAdvance.FilterIncludeIDs.Count > 0)
                                {
                                    foreach (string include in monitorDataLine.MonitorAdvance.FilterIncludeIDs)
                                    {
                                        if (lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorName == include)
                                        {
                                            isInclude = true;
                                            break;
                                        }
                                    }
                                }
                                if (isInclude) continue;
                                else
                                {
                                    lstMonitorValues.Remove(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0]);
                                    lstMonitorValues.Add(mv);
                                }
                            }
                            else if (!dicPOCOrder.ContainsKey(iPOC) && !dicPOCOrder.ContainsKey(iPOCold))
                            {
                                if (iPOC > 0 && iPOCold > 0 && iPOC < iPOCold)
                                {
                                    bool isInclude = false;
                                    if (monitorDataLine.MonitorAdvance != null && monitorDataLine.MonitorAdvance.FilterIncludeIDs != null && monitorDataLine.MonitorAdvance.FilterIncludeIDs.Count > 0)
                                    {
                                        foreach (string include in monitorDataLine.MonitorAdvance.FilterIncludeIDs)
                                        {
                                            if (lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorName == include)
                                            {
                                                isInclude = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (isInclude) continue;
                                    else
                                    {
                                        lstMonitorValues.Remove(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0]);
                                        lstMonitorValues.Add(mv);
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            // lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod

                        }
                        /*
                        if (mv.Values.GetRange(0, 90).Count(p => p != float.MinValue) >= 11 &&
                            mv.Values.GetRange(90, 91).Count(p => p != float.MinValue) >= 11 &&
                            mv.Values.GetRange(181, 92).Count(p => p != float.MinValue) >=11 &&
                                mv.Values.GetRange(273, 92).Count(p => p != float.MinValue) >=11)
                        {
                         //   if (mv.Values.Count(p => p != float.MinValue) > 11 )
                        //{
                        //if(isValidValue(mv.Values))
                        
                            if ((mv.MonitorMethod.Contains("MethodCode=116") ||
                                mv.MonitorMethod.Contains("MethodCode=117") ||
                                mv.MonitorMethod.Contains("MethodCode=118") ||
                                mv.MonitorMethod.Contains("MethodCode=119") ||
                                mv.MonitorMethod.Contains("MethodCode=120") )&&(
                                mv.MonitorMethod.Contains("POC=1") ||
                                mv.MonitorMethod.Contains("POC=2") ||
                                mv.MonitorMethod.Contains("POC=3") ||
                                mv.MonitorMethod.Contains("POC=4")))
                            {
                                if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                                {
                                    if (lstMonitorValues.Where(p => p.MonitorName == mv.MonitorName).Count() == 0 && lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).Count() == 0)
                                    {
                                        while (mv.MonitorName.Trim().Length < 15)
                                        {
                                            mv.MonitorName = "0" + mv.MonitorName;
 
                                        }
                                        lstMonitorValues.Add(mv);
                                        DataRow dr = dtO.NewRow();
                                        dr[0] = mv.MonitorName;
                                        dr[1] = mv.MonitorMethod;
                                        dr[2] = mv.Longitude;
                                        dr[3] = mv.Latitude;
                                        dtO.Rows.Add(dr);
                                    }
                                    else if( lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).Count() >1)
                                    {
                                        if(Convert.ToInt32(mv.MonitorMethod.Substring(mv.MonitorMethod.IndexOf("POC=")+4,1))>Convert.ToInt32(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod.Substring(mv.MonitorMethod.IndexOf("POC=")+4,1)))
                                        {
                                            lstMonitorValues.Remove(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0]);
                                            lstMonitorValues.Add(mv);
                                        }
                                        // lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod

                                    }
                                }
                            }
                        }
                         * */

                    }
                    fbDataReader.Close();
                    //BenMAP b = new BenMAP("");
                    //b.SaveCSV(dtO, @"D:\Monitor.csv");
                }
                if (monitorDataLine.MonitorDirectType == 1)//TextFile
                {
                    //load textFile  Monitor Name	Monitor Description	Latitude	Longitude	Metric	Seasonal Metric	Statistic	Values
                    int iMonitorName = -1;
                    int iMonitorDescription = -1;
                    int iLatitude = -1;
                    int iLongitude = -1;
                    int iMetric = -1;
                    int iSeasonalMetric = -1;
                    int iStatistic = -1;
                    int iValues = -1;
                    i = 0;
                    //DataWorker.DataReader dp = new DataWorker.DataReader();
                    //System.Data.DataSet ds = dp.GetDataFromFile(monitorDataLine.MonitorDataFilePath);
                    DataTable dt = CommonClass.ExcelToDataTable(monitorDataLine.MonitorDataFilePath);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        switch (dc.ColumnName.ToLower().Replace(" ", ""))
                        {
                            case "monitorname":
                                iMonitorName = i;
                                break;
                            case "monitordescription":
                            case "description":
                                iMonitorDescription = i;
                                break;
                            case "latitude":
                                iLatitude = i;
                                break;
                            case "longitude":
                                iLongitude = i;
                                break;
                            case "metric":
                                iMetric = i;
                                break;
                            case "seasonalmetric":
                                iSeasonalMetric = i;
                                break;
                            case "statistic":
                            case "annualmetric":
                                iStatistic = i;
                                break;
                            case "values":
                                iValues = i;
                                break;
                        }
                        i++;
                    }
                    if (!(CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx")))
                    {
                        string warningtip = "";
                        if (iMonitorName < 0) warningtip = "'Monitor Name', ";
                        if (iMonitorDescription < 0) warningtip += "'Monitor Description', ";
                        if (iLatitude < 0) warningtip += "'Latitude', ";
                        if (iLongitude < 0) warningtip += "'Longitude', ";
                        if (iMetric < 0) warningtip += "'Metric', ";
                        if (iSeasonalMetric < 0) warningtip += "'Seasonal Metric', ";
                        if (iStatistic < 0) warningtip += "'Statistic', ";
                        if (iValues < 0) warningtip += "'Values', ";
                        if (warningtip != "")
                        {
                            warningtip = warningtip.Substring(0, warningtip.Length - 2);
                            warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
                            MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    lstMonitorValues = new List<MonitorValue>();
                    List<Metric> lstMetric = null;
                    List<SeasonalMetric> lstSeasonalMetric = null;
                    string[] strArray = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        mv = new MonitorValue();
                        mv.Latitude = Convert.ToDouble(dr[iLatitude]);
                        mv.Longitude = Convert.ToDouble(dr[iLongitude]);
                        if (!string.IsNullOrEmpty(dr[iMetric].ToString()))
                        {
                            lstMetric = benMAPPollutant.Metrics.Where(p => p.MetricName.ToLower() == dr[iMetric].ToString().ToLower().Trim()).ToList();
                            if (lstMetric != null && lstMetric.Count > 0)
                            {
                                mv.Metric = lstMetric.First();
                            }
                        }
                        if (!string.IsNullOrEmpty(dr[iSeasonalMetric].ToString()) && benMAPPollutant.SesonalMetrics != null && benMAPPollutant.SesonalMetrics.Count > 0)
                        {
                            lstSeasonalMetric = benMAPPollutant.SesonalMetrics.Where(p => p.SeasonalMetricName.ToLower() == dr[iSeasonalMetric].ToString().ToLower().Trim()).ToList();
                            if (lstSeasonalMetric != null && lstSeasonalMetric.Count > 0)
                            {
                                mv.SeasonalMetric = lstSeasonalMetric.First();
                            }
                        }
                        mv.MonitorMethod = dr[iMonitorDescription].ToString();
                        mv.MonitorName = dr[iMonitorName].ToString();
                        mv.Statistic = dr[iStatistic].ToString();
                        strArray = dr[iValues].ToString().Split(new char[] { ',' });
                        mv.Values = new List<float>();
                        foreach (string s in strArray)
                        {
                            if (string.IsNullOrEmpty(s) || s == ".")
                            {
                                mv.Values.Add(float.MinValue);
                            }
                            else
                            {
                                mv.Values.Add(Convert.ToSingle(s));
                            }
                        }
                        lstMonitorValues.Add(mv);
                    }
                }
                return lstMonitorValues;
            }
            catch
            {
                return null;
            }
        }
        public static void UpdateModelValuesMonitorData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, ref MonitorDataLine monitorDataLine)
        {
            try
            {
                int i = 0;
                List<MonitorValue> lstMonitorValues = GetMonitorData(benMAPGrid, benMAPPollutant, monitorDataLine);
                Dictionary<string, Metric> dicMetric = new Dictionary<string, Metric>();
                foreach (Metric m in benMAPPollutant.Metrics)
                {
                    dicMetric.Add(m.MetricName, m);
                }
                Dictionary<string, SeasonalMetric> dicSeasonalMetric = new Dictionary<string, SeasonalMetric>();
                foreach (SeasonalMetric m in benMAPPollutant.SesonalMetrics)
                {
                    dicSeasonalMetric.Add(m.SeasonalMetricName, m);
                }
                //更新DicMetricValue
                UpdateMonitorDicMetricValue(benMAPPollutant, lstMonitorValues);
                //add by xiejp 20131113
                monitorDataLine.MonitorValues = lstMonitorValues;
                //转化成所属污染物的需要模型值
                //-----------首先要从Grid中生成List<RowCol>
                //List<RowCol> lstRowCol = getAllRowColFromGridType(benMAPGrid);
                //-----------循环List<RowCol>
                //--VNA---选取最近的3-8个（5）个 Closet 最近的进行插值
                //Fixed Radius 选取多少km半径内的Monitor数据。如果Advance的Get Closest if None within Radius, 如果半径范围内没有Monitor则取最近的
                int iCol = -1, iRow = -1;
                Dictionary<MonitorValue, float> DicMonitorDistance = null;
                Dictionary<string, Dictionary<string, string>> dicNeighbors = new Dictionary<string, Dictionary<string, string>>();
                List<KeyValuePair<MonitorValue, float>> DicMonitorDistanceTemp = null;

                KeyValuePair<MonitorValue, float> DicMonitorDistanceKeyValue = new KeyValuePair<MonitorValue, float>();
                double dfz = 0, dfm = 1;
                double fixdic = 0;
                monitorDataLine.ModelResultAttributes = new List<ModelResultAttribute>();
                if (benMAPGrid != null)
                {
                    DotSpatial.Data.IFeatureSet fs = new DotSpatial.Data.FeatureSet();
                    string strSHP = "";
                    if (benMAPGrid is ShapefileGrid)
                    {
                        strSHP = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                    }
                    else if (benMAPGrid is RegularGrid)
                    {
                        strSHP = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as RegularGrid).ShapefileName + ".shp";
                    }
                    fs=FeatureSet.Open(strSHP);
                    i = 0;
                    foreach (DataColumn dc in fs.DataTable.Columns)
                    {
                        if (dc.ColumnName.ToLower() == "col")
                            iCol = i;
                        if (dc.ColumnName.ToLower() == "row")
                            iRow = i;

                        i++;
                    }
                    i = 0;
                    double dmin = 0.0;
                    double dClose = -1;
                    double dtempfz = 0.0;
                    double dtempfm = 0.0;
                    if (monitorDataLine.MonitorAdvance != null && monitorDataLine.MonitorAdvance.MaxinumNeighborDistance != -1)
                        dClose = monitorDataLine.MonitorAdvance.MaxinumNeighborDistance;// / 111.000;
                    else if (monitorDataLine.MonitorAdvance != null && monitorDataLine.MonitorAdvance.RelativeNeighborDistance != -1)
                        dClose = monitorDataLine.MonitorAdvance.RelativeNeighborDistance;
                    IFeatureSet fsPoints = new FeatureSet();
                    List<Coordinate> lstCoordinate = new List<Coordinate>();

                    List<double> fsInter = new List<double>();
                    //fsInter.DataTable.Columns.Add("LongLat");
                    //fsInter.DataTable.Columns.Add("Long");
                    //fsInter.DataTable.Columns.Add("Lat");
                    Dictionary<string, MonitorValue> dicMonitorValues = new Dictionary<string, MonitorValue>(); ;
                    foreach (MonitorValue monitorValue in lstMonitorValues)
                    {
                        if (monitorValue.dicMetricValues == null || monitorValue.dicMetricValues.Count == 0) continue;//|| monitorValue.dicMetricValues365 == null || monitorValue.dicMetricValues365.Count == 0) continue;
                        if (!dicMonitorValues.ContainsKey(monitorValue.Longitude + "," + monitorValue.Latitude))
                        {
                            dicMonitorValues.Add(monitorValue.Longitude + "," + monitorValue.Latitude, monitorValue);
                            fsPoints.AddFeature(new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude));
                            lstCoordinate.Add(new Coordinate(monitorValue.Longitude, monitorValue.Latitude));
                            fsInter.Add(monitorValue.Longitude);
                            fsInter.Add(monitorValue.Latitude);
                        }
                        else
                        {
                            for (int idic = 0; idic < dicMonitorValues[monitorValue.Longitude + "," + monitorValue.Latitude].dicMetricValues.Count; idic++)
                            {

                                dicMonitorValues[monitorValue.Longitude + "," + monitorValue.Latitude].dicMetricValues[dicMonitorValues[monitorValue.Longitude + "," + monitorValue.Latitude].dicMetricValues.Keys.ToList()[idic]] = (dicMonitorValues[monitorValue.Longitude + "," + monitorValue.Latitude].dicMetricValues.ToList()[idic].Value +
                                    monitorValue.dicMetricValues.ToList()[idic].Value) / 2;
                            }
                        }

                        // fsInter.DataTable.Rows[fsInter.DataTable.Rows.Count - 1]["LongLat"] = monitorValue.Longitude + "," + monitorValue.Latitude;
                        //fsInter.DataTable.Rows[fsInter.DataTable.Rows.Count - 1]["Long"] = monitorValue.Longitude;// + "," + monitorValue.Latitude;
                        //fsInter.DataTable.Rows[fsInter.DataTable.Rows.Count - 1]["Lat"] = monitorValue.Latitude;// + "," + monitorValue.Latitude;
                    }
                    Coordinate cCenter= fsPoints.Extent.Center;
                    //-----------求到中心点的距离排倒序，取100个
                    Dictionary<MonitorValue, double> dicCoordinateDistanceCenter = new Dictionary<MonitorValue, double>();
                    foreach (MonitorValue monitorValue in lstMonitorValues)
                    {
                        dicCoordinateDistanceCenter.Add(monitorValue, getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, cCenter.X, cCenter.Y));
                    }
                    List<MonitorValue> lstCenter = dicCoordinateDistanceCenter.OrderByDescending(p => p.Value).Select(p => p.Key).ToList().GetRange(0, lstMonitorValues.Count > 300 ? 300 : lstMonitorValues.Count);
                    //IFeatureSet fsVoronoiAnalysis= DotSpatial.Analysis.Voronoi.VoronoiPolygons(fsPoints, false);
                    int idicMonitorValues = dicMonitorValues.Count;
                    if (idicMonitorValues > 100) idicMonitorValues = 100;
                    Polygon fOnlyPoint = null;// new Polygon();
                    IFeatureSet fsVoronoi = new FeatureSet();
                    //IFeatureSet fsLast = new FeatureSet();// fsInter.Intersection(fsVoronoi.Intersection(fsOnlyPoint, FieldJoinType.LocalOnly, null)).Intersection(fsVoronoi, FieldJoinType.All, null);
                    //IFeatureSet fsVoronoiInter = new FeatureSet();// fsVoronoi.Intersection(fsOnlyPoint, FieldJoinType.LocalOnly, null);
                    DotSpatial.Topology.Coordinate coordinate = new DotSpatial.Topology.Coordinate();
                    List<Polygon> lstPolygon = new List<Polygon>();
                    List<float> lstDouble = new List<float>();
                    List<double> fsout = new List<double>(); ;
                    double dtemp = -1;
                    double dtemp2 = 0.0;

                    Dictionary<Coordinate, double> dicCoordinateDistance = new Dictionary<Coordinate, double>();
                    MonitorValue mvTemp = null;
                    Dictionary<MonitorValue, float> dicCenter = null;
                    if (monitorDataLine.ModelAttributes == null) monitorDataLine.ModelAttributes = new List<ModelAttribute>();
                    monitorDataLine.MonitorNeighbors = new List<MonitorNeighborAttribute>();
                    while (i < fs.DataTable.Rows.Count)
                    {
                        if (fs.GetFeature(i).BasicGeometry.GeometryType == "Polygon")
                        {
                            coordinate = new Coordinate((fs.GetFeature(i).BasicGeometry as Polygon).Centroid.X, (fs.GetFeature(i).BasicGeometry as Polygon).Centroid.Y);
                            //coordinate = (fs.GetFeature(i).BasicGeometry as Polygon).Envelope.Center();//new Coordinate((.X, (fs.GetFeature(i).BasicGeometry as Polygon).Centroid.Y);
                        }
                        else
                        {
                            coordinate = new Coordinate((fs.GetFeature(i).BasicGeometry as MultiPolygon).Centroid.X, (fs.GetFeature(i).BasicGeometry as MultiPolygon).Centroid.Y);
                            //coordinate = (fs.GetFeature(i).BasicGeometry as MultiPolygon).Envelope.Center();// new Coordinate((fs.GetFeature(i).BasicGeometry as MultiPolygon).Centroid.X, (fs.GetFeature(i).BasicGeometry as MultiPolygon).Centroid.Y);
                        }
                        switch (monitorDataLine.InterpolationMethod)
                        {
                            case InterpolationMethodEnum.ClosestMonitor://以最近的那个作为值--
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();
                                // DotSpatial.Topology.Coordinate coordinate= fs.GetFeature(i).Envelope.ToExtent().Center;
                                foreach (MonitorValue monitorValue in lstMonitorValues)
                                {
                                    DicMonitorDistance.Add(monitorValue, Convert.ToSingle((coordinate.X - monitorValue.Longitude) * (coordinate.X - monitorValue.Longitude) + (coordinate.Y - monitorValue.Latitude) * (coordinate.Y - monitorValue.Latitude)));// getDistanceFrom2Point(coordinate.X,coordinate.Y,monitorValue.Longitude,monitorValue.Latitude));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// getDistanceFromExtent(fs.GetFeature(i).Envelope.ToExtent().Center,fs.GetFeature(i).Envelope, new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// fs.GetFeature(i).Envelope.ToExtent().Center.Distance(new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));
                                }
                                dmin = DicMonitorDistance.Min(p => p.Value);
                                DicMonitorDistanceKeyValue = DicMonitorDistance.Where(a => a.Value == dmin).First();
                                if (dClose != -1 && getDistanceFrom2Point(coordinate.X, coordinate.Y, DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude) > dClose)// DicMonitorDistanceKeyValue.Value<dClose)//Math.Sqrt(DicMonitorDistanceKeyValue.Value)-Math.Min(fs.GetFeature(i).Envelope.Height,fs.GetFeature(i).Envelope.Width) > dClose)
                                { }
                                else
                                {
                                    monitorDataLine.ModelResultAttributes.Add(new ModelResultAttribute()
                                    {
                                        Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                        Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                        Values = DicMonitorDistanceKeyValue.Key.dicMetricValues
                                    });
                                    //---------Add Neighbors
                                    monitorDataLine.MonitorNeighbors.Add(new MonitorNeighborAttribute()
                                    {
                                        Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                        Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                        Distance = getDistanceFrom2Point(new Point(coordinate), new Point(DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude)),
                                        MonitorName = DicMonitorDistanceKeyValue.Key.MonitorName,
                                        Weight = 1
                                    });
                                    //if (DicMonitorDistanceKeyValue.Key.dicMetricValues365 != null)
                                    //{
                                    //    foreach (KeyValuePair<string, List<float>> k in DicMonitorDistanceKeyValue.Key.dicMetricValues365)
                                    //    {
                                    //        monitorDataLine.ModelAttributes.Add(new ModelAttribute()
                                    //        {
                                    //            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                    //            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                    //            Metric = dicMetric.ContainsKey(k.Key) ? dicMetric[k.Key] : null,
                                    //            Values = k.Value
                                    //        });
                                    //    }
                                    //}
                                }
                                break;
                            case InterpolationMethodEnum.FixedRadius://以半径范围内
                                //Fixed Radius 选取多少km半径内的Monitor数据。如果Advance的Get Closest if None within Radius, 如果半径范围内没有Monitor则取最近的
                                //经纬度距离计算
                                DotSpatial.Topology.Point tPoint = new DotSpatial.Topology.Point(coordinate);
                                //Feature fBuffer=new Feature( tPoint.Buffer(monitorDataLine.FixedRadius/111.00));
                                //IFeatureSet fsetBuffer = new FeatureSet();
                                //fsetBuffer.AddFeature(fBuffer);
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();
                                //foreach (Coordinate c in lstCoordinate)
                                //{
                                //    if (fBuffer.Contains(new Point(c)))
                                //    {
                                //        DicMonitorDistance.Add(dicMonitorValues[c.X + "," + c.Y], getDistanceFrom2Point(c.X, c.Y, coordinate.X, coordinate.Y));

                                //    }
                                //}
                                foreach (MonitorValue monitorValue in lstMonitorValues)
                                {
                                    DicMonitorDistance.Add(monitorValue, getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, coordinate.X, coordinate.Y));//(coordinate.X - monitorValue.Longitude) * (coordinate.X - monitorValue.Longitude) + (coordinate.Y - monitorValue.Latitude) * (coordinate.Y - monitorValue.Latitude));// getDistanceFrom2Point(coordinate.X,coordinate.Y,monitorValue.Longitude,monitorValue.Latitude));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// getDistanceFromExtent(fs.GetFeature(i).Envelope.ToExtent().Center,fs.GetFeature(i).Envelope, new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// fs.GetFeature(i).Envelope.ToExtent().Center.Distance(new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));
                                }
                                //IFeatureSet fsIntersection= fsPoints.Intersection(fsetBuffer, FieldJoinType.All, null);
                                //foreach (Feature fInter in fsIntersection.Features)
                                //{
                                //    DotSpatial.Topology.Point pf = fInter.BasicGeometry as DotSpatial.Topology.Point;
                                //    DicMonitorDistance.Add(dicMonitorValues[pf.X + "," + pf.Y], getDistanceFrom2Point(pf.X, pf.Y, coordinate.X, coordinate.Y));
                                //}
                                fixdic = monitorDataLine.FixedRadius;
                                DicMonitorDistanceTemp = DicMonitorDistance.Where(p => (p.Value) <= fixdic).ToList();// * 1.60931).ToList();//如果出来还是经纬度则需要*111也可以用中心点做距离
                                if (DicMonitorDistanceTemp.Count == 0)
                                {
                                    //continue;
                                    //dmin = DicMonitorDistance.Min(p => p.Value);
                                    //DicMonitorDistanceKeyValue = DicMonitorDistance.Where(a => a.Value == dmin).First();

                                    //if (monitorDataLine.MonitorAdvance == null || monitorDataLine.MonitorAdvance.GetClosedIfNoneWithinRadius)
                                    //{
                                    //    //--得到最近的
                                    //    monitorDataLine.ModelResultAttributes.Add(new ModelResultAttribute()
                                    //    {
                                    //        Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                    //        Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                    //        Values = DicMonitorDistanceKeyValue.Key.dicMetricValues
                                    //    });
                                    //}
                                    //if (DicMonitorDistanceKeyValue.Key.dicMetricValues365 != null)
                                    //{
                                    //    foreach (KeyValuePair<string, List<float>> k in DicMonitorDistanceKeyValue.Key.dicMetricValues365)
                                    //    {
                                    //        monitorDataLine.ModelAttributes.Add(new ModelAttribute()
                                    //        {
                                    //            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                    //            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                    //            Metric = dicMetric.ContainsKey(k.Key) ? dicMetric[k.Key] : null,
                                    //            Values = k.Value
                                    //        });
                                    //    }
                                    //}
                                    ////---------Add Neighbors
                                    //monitorDataLine.MonitorNeighbors.Add(new MonitorNeighborAttribute()
                                    //{
                                    //    Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                    //    Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                    //    Distance = getDistanceFrom2Point(new Point(coordinate), new Point(DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude)),
                                    //    MonitorName = DicMonitorDistanceKeyValue.Key.MonitorName,
                                    //    Weight = 1
                                    //});
                                }
                                else
                                {
                                    //--------------------------------------------------------------------------
                                    DicMonitorDistanceKeyValue = DicMonitorDistanceTemp.First();
                                    Dictionary<string, ModelAttribute> dicModelAttribute = new Dictionary<string, ModelAttribute>();
                                    if (monitorDataLine.MonitorAdvance == null || monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistance)
                                    {
                                        //反距离
                                        monitorDataLine.ModelResultAttributes.Add(new ModelResultAttribute()
                                        {
                                            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                            Values = new Dictionary<string, float>()
                                        });
                                        foreach (KeyValuePair<string, float> dicsd in DicMonitorDistanceKeyValue.Key.dicMetricValues)
                                        {
                                            dicModelAttribute.Add(dicsd.Key, new ModelAttribute()
                                            {
                                                Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                                Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                                Metric = dicMetric.ContainsKey(dicsd.Key) ? dicMetric[dicsd.Key] : null,
                                                Values = new List<float>()
                                            });
                                        }
                                        foreach (KeyValuePair<string, float> dicsd in DicMonitorDistanceKeyValue.Key.dicMetricValues)
                                        {
                                            dfm = 0; dfz = 0;
                                            List<float> lstdfm=new List<float>();
                                            List<float> lstdfz = new List<float>();
                                            foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistanceTemp)
                                            {
                                                dfm += k.Key.dicMetricValues[dicsd.Key] / k.Value;
                                                dfz += 1.0000 / k.Value;
                                                if (lstdfm.Count == 0)
                                                {
                                                    if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                    {
                                                        lstdfm = k.Key.dicMetricValues365[dicsd.Key].Select(p => p == float.MinValue ? 0 : p / k.Value).ToList();
                                                        lstdfz = k.Key.dicMetricValues365[dicsd.Key].Select(p => p == float.MinValue ? 0 : 1 / k.Value).ToList();

                                                    }
                                                }
                                                else
                                                {
                                                    if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                    {
                                                        for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                                        {
                                                            lstdfm[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : k.Key.dicMetricValues365[dicsd.Key][idfm] / k.Value;
                                                            lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / k.Value;

                                                        }
                                                    }
                                                }
                                            }
                                            for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                            {
                                                lstdfm[idfm] =lstdfz[idfm]==0?float.MinValue: lstdfm[idfm] / (lstdfz[idfm]);
                                            }
                                            dicModelAttribute[dicsd.Key].Values = lstdfm;
                                            monitorDataLine.ModelResultAttributes.Last().Values.Add(dicsd.Key, Convert.ToSingle(Math.Round(dfm / dfz,2)));
                                            //---------Add Neighbors
                                           
                                        }
                                        foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistanceTemp)
                                        {
                                            monitorDataLine.MonitorNeighbors.Add(new MonitorNeighborAttribute()
                                            {
                                                Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                                Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                                Distance = k.Value,
                                                MonitorName = k.Key.MonitorName,
                                                Weight = (1.0000 / k.Value) / DicMonitorDistanceTemp.Sum(p => (1.000 / p.Value))
                                            });
                                        }
                                        //foreach (KeyValuePair<string, ModelAttribute> k in dicModelAttribute)
                                        //{
                                        //    monitorDataLine.ModelAttributes.Add(k.Value);
                                        //}
                                    }
                                    else if (monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistanceSquared)
                                    {
                                        //反距离平方
                                        monitorDataLine.ModelResultAttributes.Add(new ModelResultAttribute()
                                        {
                                            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                            Values = new Dictionary<string, float>()
                                        });
                                        foreach (KeyValuePair<string, float> dicsd in DicMonitorDistanceKeyValue.Key.dicMetricValues)
                                        {
                                            dicModelAttribute.Add(dicsd.Key, new ModelAttribute()
                                            {
                                                Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                                Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                                Metric = dicMetric.ContainsKey(dicsd.Key) ? dicMetric[dicsd.Key] : null,
                                                SeasonalMetric = dicSeasonalMetric.ContainsKey(dicsd.Key) ? dicSeasonalMetric[dicsd.Key] : null,
                                                Values = new List<float>()
                                            });
                                        }
                                        foreach (KeyValuePair<string, float> dicsd in DicMonitorDistanceKeyValue.Key.dicMetricValues)
                                        {
                                            List<float> lstdfm = new List<float>();
                                            List<float> lstdfz = new List<float>();
                                            dfm = 0; dfz = 0;
                                            foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistanceTemp)
                                            {
                                                dfm += k.Key.dicMetricValues[dicsd.Key] / Math.Pow(k.Value, 2);
                                                dfz += 1.0000 / Math.Pow(k.Value, 2);
                                                if (lstdfm.Count == 0)
                                                {
                                                    if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                    {
                                                        lstdfm = k.Key.dicMetricValues365[dicsd.Key].Select(p => p==float.MinValue?0:p / Convert.ToSingle(Math.Pow(k.Value, 2))).ToList();
                                                        lstdfz = k.Key.dicMetricValues365[dicsd.Key].Select(p => p == float.MinValue ? 0 : 1 / Convert.ToSingle(Math.Pow(k.Value, 2))).ToList();
                                                    }
                                                }
                                                else
                                                {
                                                    if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                    {
                                                        for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                                        {
                                                            lstdfm[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : k.Key.dicMetricValues365[dicsd.Key][idfm] / Convert.ToSingle(Math.Pow(k.Value, 2));
                                                            lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / Convert.ToSingle(Math.Pow(k.Value, 2));


                                                        }
                                                    }
                                                }
                                            }
                                            for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                            {
                                                lstdfm[idfm] = lstdfz[idfm] == 0 ? float.MinValue : lstdfm[idfm] / lstdfz[idfm];
                                            }
                                            dicModelAttribute[dicsd.Key].Values = lstdfm;
                                            monitorDataLine.ModelResultAttributes.Last().Values.Add(dicsd.Key, Convert.ToSingle(Math.Round(dfm / dfz, 2)));
                                            //---------Add Neighbors
                                            
                                        }
                                        foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistanceTemp)
                                        {
                                            monitorDataLine.MonitorNeighbors.Add(new MonitorNeighborAttribute()
                                            {
                                                Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                                Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                                Distance = k.Value,
                                                MonitorName = k.Key.MonitorName,
                                                Weight = (1.0000 / Math.Pow(k.Value, 2)) / DicMonitorDistanceTemp.Sum(p => (1.000 / Math.Pow(p.Value, 2)))
                                            });
                                        }
                                        //foreach (KeyValuePair<string, ModelAttribute> k in dicModelAttribute)
                                        //{
                                        //    monitorDataLine.ModelAttributes.Add(k.Value);
                                        //}
                                    }
                                }
                                break;
                            case InterpolationMethodEnum.VoronoiNeighborhoodAveragin://以5个作为需要的值-----兼顾Advance---还需要小于最大的距离-------------------------未写---
                                //DicMonitorDistance = new Dictionary<MonitorValue, double>();
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();
                                idicMonitorValues = 500;
                                //DotSpatial.Topology.Coordinate coordinate= fs.GetFeature(i).Envelope.ToExtent().Center;
                                foreach (MonitorValue monitorValue in lstMonitorValues)
                                {
                                    DicMonitorDistance.Add(monitorValue, Convert.ToSingle((coordinate.X - monitorValue.Longitude) * (coordinate.X - monitorValue.Longitude) + (coordinate.Y - monitorValue.Latitude) * (coordinate.Y - monitorValue.Latitude)));// getDistanceFrom2Point(coordinate.X,coordinate.Y,monitorValue.Longitude,monitorValue.Latitude));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// getDistanceFromExtent(fs.GetFeature(i).Envelope.ToExtent().Center,fs.GetFeature(i).Envelope, new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// fs.GetFeature(i).Envelope.ToExtent().Center.Distance(new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));
                                }
                                //lstDouble = DicMonitorDistance.Select(p => p.Value).ToList();
                                //lstDouble.Sort();
                                //var query = DicMonitorDistance.OrderBy(p=>p.Value).ToList().GetRange(0,idicMonitorValues).ToDictionary(p=>p.Key,p=>p.Value);// .Where(p => lstDouble.GetRange(0, idicMonitorValues).Contains(p.Value));
                                //modify by xiejp use radius
                                
                                var query = DicMonitorDistance.Where(p=>p.Value<25).ToList();//.OrderBy(p=>p.Value).ToList().GetRange(0,idicMonitorValues).ToDictionary(p=>p.Key,p=>p.Value);// .Where(p => lstDouble.GetRange(0, idicMonitorValues).Contains(p.Value));
                                int iDistanceForQuery = 1;
                                while (query.Count < 10 && query.Count < DicMonitorDistance.Count)
                                {
                                    query = DicMonitorDistance.Where(p => p.Value < 25 + iDistanceForQuery).ToList();
                                    iDistanceForQuery++;

                                }
                                if (monitorDataLine.MonitorAdvance != null && monitorDataLine.MonitorAdvance.MaxinumNeighborDistance != -1)
                                {
                                    Dictionary<MonitorValue, float> dicMaxinumNeighbor = new Dictionary<MonitorValue, float>();
                                    foreach (KeyValuePair<MonitorValue, float> k in query)
                                    {
                                        float d=getDistanceFrom2Point(coordinate.X, coordinate.Y, k.Key.Longitude, k.Key.Latitude);
                                        if (d <= monitorDataLine.MonitorAdvance.MaxinumNeighborDistance)
                                        {
                                            dicMaxinumNeighbor.Add(k.Key, d);
                                        }
                                    }
                                    query = dicMaxinumNeighbor.ToList();
                                }
                                if (monitorDataLine.MonitorAdvance != null && monitorDataLine.MonitorAdvance.RelativeNeighborDistance != -1)
                                {
                                    Dictionary<MonitorValue, float> dicMaxinumNeighbor = new Dictionary<MonitorValue, float>();
                                    foreach (KeyValuePair<MonitorValue, float> k in query)
                                    {
                                        float d = getDistanceFrom2Point(coordinate.X, coordinate.Y, k.Key.Longitude, k.Key.Latitude);
                                        dicMaxinumNeighbor.Add(k.Key, d);
                                        

                                    }
                                    if (dicMaxinumNeighbor.Count == 0) break;
                                    float dMin = dicMaxinumNeighbor.Min(p => p.Value);
                                    double dRelative = monitorDataLine.MonitorAdvance.RelativeNeighborDistance;
                                    query = dicMaxinumNeighbor.Where(p => p.Value / dMin <= dRelative).ToList();
                                }
                                
                                
                                
                                fsInter = new List<double>();
                                fsInter.Add(coordinate.X);
                                fsInter.Add(coordinate.Y);
                                List<Coordinate> lstCIn = new List<Coordinate>();
                                List<Polygon> lstVoronoiPolygon = new List<Polygon>();
                                foreach (KeyValuePair<MonitorValue, float> k in query)
                                {
                                    fsInter.Add(k.Key.Longitude);
                                    fsInter.Add(k.Key.Latitude);
                                   
                                }
                              
                                fsout = new List<double>();
                                
                                VoronoiPoints(fsInter.ToArray(), ref fsout);
                                /*double XMin, XMax, YMin, YMax;
                                fsVoronoi=new FeatureSet();
                                fsVoronoi.AddFeature(new Point(coordinate));
                                foreach (KeyValuePair<MonitorValue, float> k in query)
                                {
                                    fsVoronoi.AddFeature(new Point(k.Key.Longitude, k.Key.Latitude));
                                    

                                }
                                VoronoiPolygons(fsVoronoi, ref lstVoronoiPolygon, fs.Extent.ToEnvelope());
                               // VoronoiPoints(fsInter.ToArray(), ref fsout);// , dicCoordinateDistance.Where(p => lstDouble.GetRange(0, lstDouble.Count > 20 ? 20 : lstDouble.Count).Contains(p.Value)).ToList());
                                //VoronoiPolygons(
                                if (lstVoronoiPolygon.Count > 0)
                                {
                                   

                                    foreach (KeyValuePair<MonitorValue, float> k in query)
                                    {
                                        //if (k.Key.Longitude >= XMin && k.Key.Latitude >= YMin &&
                                        //    k.Key.Longitude <= XMax && k.Key.Latitude <= YMax)
                                        //{
                                        lstCIn.Add(new Coordinate(k.Key.Longitude, k.Key.Latitude));
                                        //}

                                    }
                                    Polygon pCoordinate=null;
                                    foreach (Polygon p in lstVoronoiPolygon)
                                    {

                                        if (p.Contains(new Point(coordinate)))
                                            {
                                                pCoordinate = p;
                                                
                                                
                                            }
                                     }
                                    lstVoronoiPolygon.Remove(pCoordinate);
                                    //--------------如果有相交则OK。
                                    List<Polygon> lstPolygonResult = new List<Polygon>();
                                    foreach (Polygon p in lstVoronoiPolygon)
                                    {
                                        if (pCoordinate.Intersects(p))
                                        {
                                            foreach (Coordinate c in lstCIn)
                                            {
                                                if (p.Contains(new Point(c)))
                                                {
                                                    fsout.Add(c.X);
                                                    fsout.Add(c.Y);
 
                                                }
                                            }
                                        }
                                    }

                                }
                                 * */
                                //break;
                                //fsout = lstCoordinate.Where(p=>fsout.Contains(p)).ToList();
                                //DicMonitorDistance = new Dictionary<MonitorValue, double>();
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();
                                dicNeighbors.Add(fs.DataTable.Rows[i]["COL"].ToString() + "," + fs.DataTable.Rows[i]["ROW"].ToString(), new Dictionary<string, string>());
                                for (int ifsout = 0; ifsout < fsout.Count; ifsout++)
                                {
                                    if (dicMonitorValues.ContainsKey(fsout[ifsout] + "," + fsout[ifsout + 1]))
                                    {
                                        mvTemp = dicMonitorValues[fsout[ifsout] + "," + fsout[ifsout + 1]];
                                        if (!DicMonitorDistance.ContainsKey(mvTemp))
                                        {
                                            float dDistance=getDistanceFrom2Point(mvTemp.Longitude, mvTemp.Latitude,
                                            coordinate.X, coordinate.Y);
                                            DicMonitorDistance.Add(mvTemp,dDistance );
                                            dicNeighbors[fs.DataTable.Rows[i]["COL"].ToString() + "," + fs.DataTable.Rows[i]["ROW"].ToString()].Add(mvTemp.MonitorName, dDistance.ToString());
                                        }
                                    }
                                    ifsout++;
                                }
                                if (DicMonitorDistance.Count == 0 || DicMonitorDistance.First().Key.dicMetricValues.Count == 0)
                                {
                                }
                                else if (monitorDataLine.MonitorAdvance == null || monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistance)
                                {
                                    Dictionary<string, ModelAttribute> dicModelAttribute = new Dictionary<string, ModelAttribute>();

                                    foreach (KeyValuePair<string, float> dicsd in DicMonitorDistance.First().Key.dicMetricValues)
                                    {
                                        dicModelAttribute.Add(dicsd.Key, new ModelAttribute()
                                        {
                                            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                            Metric = dicMetric.ContainsKey(dicsd.Key) ? dicMetric[dicsd.Key] : null,
                                            SeasonalMetric = dicSeasonalMetric.ContainsKey(dicsd.Key) ? dicSeasonalMetric[dicsd.Key] : null,
                                            Values = new List<float>()
                                        });
                                    }
                                    //反距离
                                    monitorDataLine.ModelResultAttributes.Add(new ModelResultAttribute()
                                        {
                                            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                            Values = new Dictionary<string, float>()
                                        });
                                    foreach (KeyValuePair<string, float> dicsd in DicMonitorDistance.First().Key.dicMetricValues)
                                    {
                                        dtempfz = 0.0;
                                        dtempfm = 0.0;
                                        List<float> lstdfm = new List<float>();
                                        List<float> lstdfz = new List<float>();
                                        foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistance)
                                        {
                                            if (k.Key.dicMetricValues.Keys.Contains(dicsd.Key))
                                            {
                                                dtempfz += k.Key.dicMetricValues[dicsd.Key] / k.Value;
                                                dtempfm += 1.0000 / k.Value;
                                            }
                                            if (lstdfm.Count == 0)
                                            {
                                                if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                {
                                                    lstdfm = k.Key.dicMetricValues365[dicsd.Key].Select(p => p == float.MinValue ? 0 : p / k.Value).ToList();
                                                    lstdfz = k.Key.dicMetricValues365[dicsd.Key].Select(p => p == float.MinValue ? 0 : 1 / k.Value).ToList();
                                                }
                                            }
                                            else
                                            {
                                                if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                {
                                                    for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                                    {
                                                        lstdfm[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : k.Key.dicMetricValues365[dicsd.Key][idfm] / k.Value;
                                                        lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / k.Value;
                                                    }
                                                }
                                            }
                                        }

                                        if (dtempfm == 0) dtempfm = 1;
                                        monitorDataLine.ModelResultAttributes.Last().Values.Add(dicsd.Key, Convert.ToSingle(Math.Round(dtempfz / dtempfm, 2)));
                                        for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                        {
                                            lstdfm[idfm] = lstdfz[idfm] == 0 ? float.MinValue : lstdfm[idfm] / lstdfz[idfm];
                                        }
                                        dicModelAttribute[dicsd.Key].Values = lstdfm;
                                    }
                                    //---------Add Neighbors
                                    foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistance)
                                    {
                                        monitorDataLine.MonitorNeighbors.Add(new MonitorNeighborAttribute()
                                        {
                                            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                            Distance = k.Value,
                                            MonitorName = k.Key.MonitorName,
                                            Weight = (1.0000 / k.Value) / DicMonitorDistance.Sum(p => (1.000 / p.Value))
                                        });
                                    }
                                    //foreach (KeyValuePair<string, ModelAttribute> k in dicModelAttribute)
                                    //{
                                    //    monitorDataLine.ModelAttributes.Add(k.Value);
                                    //}
                                }
                                else if (monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistanceSquared)
                                {
                                    //反距离平方
                                    Dictionary<string, ModelAttribute> dicModelAttribute = new Dictionary<string, ModelAttribute>();
                                    foreach (KeyValuePair<string, float> dicsd in DicMonitorDistance.First().Key.dicMetricValues)
                                    {
                                        dicModelAttribute.Add(dicsd.Key, new ModelAttribute()
                                        {
                                            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                            Metric = dicMetric.ContainsKey(dicsd.Key) ? dicMetric[dicsd.Key] : null,
                                            Values = new List<float>()
                                        });
                                    }
                                    monitorDataLine.ModelResultAttributes.Add(new ModelResultAttribute()
                                    {
                                        Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                        Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                        Values = new Dictionary<string, float>()
                                    });
                                    foreach (KeyValuePair<string, float> dicsd in DicMonitorDistance.First().Key.dicMetricValues)
                                    {
                                        dtempfz = 0.0;
                                        dtempfm = 0.0;
                                        List<float> lstdfm = new List<float>();
                                        List<float> lstdfz = new List<float>();
                                        foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistance)
                                        {
                                            if (k.Key.dicMetricValues.Keys.Contains(dicsd.Key))
                                            {
                                                dtempfz += k.Key.dicMetricValues[dicsd.Key] / Math.Pow(k.Value, 2);
                                                dtempfm += 1.0000 / Math.Pow(k.Value, 2);
                                            }
                                            if (lstdfm.Count == 0)
                                            {
                                                if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                {
                                                    lstdfm = k.Key.dicMetricValues365[dicsd.Key].Select(p => p == float.MinValue ? 0 : p / Convert.ToSingle(Math.Pow(k.Value, 2))).ToList();
                                                    lstdfz = k.Key.dicMetricValues365[dicsd.Key].Select(p => p == float.MinValue ? 0 : 1 / Convert.ToSingle(Math.Pow(k.Value, 2))).ToList();
                                                }
                                            }
                                            else
                                            {
                                                if (k.Key.dicMetricValues365 != null && k.Key.dicMetricValues365.ContainsKey(dicsd.Key))
                                                {
                                                    for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                                    {
                                                        lstdfm[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : k.Key.dicMetricValues365[dicsd.Key][idfm] / Convert.ToSingle(Math.Pow(k.Value, 2));
                                                        lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / Convert.ToSingle(Math.Pow(k.Value, 2));


                                                    }
                                                }
                                            }
                                        }
                                        if (dtempfm == 0) dtempfm = 1;
                                        monitorDataLine.ModelResultAttributes.Last().Values.Add(dicsd.Key, Convert.ToSingle(Math.Round(dtempfz / dtempfm, 2)));
                                        for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                        {
                                            lstdfm[idfm] = lstdfz[idfm] == 0 ? float.MinValue : lstdfm[idfm] / lstdfz[idfm];
                                        }
                                        dicModelAttribute[dicsd.Key].Values = lstdfm;
                                        //monitorDataLine.ModelResultAttributes.Last().Values.Add(dicsd.Key, (
                                        //    (DicMonitorDistanceTemp[0].Key.dicMetricValues[dicsd.Key] /Math.Pow( DicMonitorDistanceTemp[0].Value,2)) +
                                        // (DicMonitorDistanceTemp[0].Key.dicMetricValues[dicsd.Key] / Math.Pow(DicMonitorDistanceTemp[1].Value,2)) +
                                        // (DicMonitorDistanceTemp[0].Key.dicMetricValues[dicsd.Key] / Math.Pow(DicMonitorDistanceTemp[2].Value,2)) +
                                        // (DicMonitorDistanceTemp[0].Key.dicMetricValues[dicsd.Key] / Math.Pow(DicMonitorDistanceTemp[3].Value,2)) +
                                        // (DicMonitorDistanceTemp[0].Key.dicMetricValues[dicsd.Key] / Math.Pow(DicMonitorDistanceTemp[4].Value,2))) /
                                        // (1.0000 / Math.Pow(DicMonitorDistanceTemp[0].Value,2)) +
                                        // (1.0000 / Math.Pow(DicMonitorDistanceTemp[1].Value,2)) +
                                        // (1.0000 / Math.Pow(DicMonitorDistanceTemp[2].Value,2)) +
                                        // (1.0000 / Math.Pow(DicMonitorDistanceTemp[3].Value,2)) +
                                        // (1.0000 / Math.Pow(DicMonitorDistanceTemp[4].Value, 2)));
                                    }
                                    //---------Add Neighbors
                                    foreach (KeyValuePair<MonitorValue, float> k in DicMonitorDistance)
                                    {
                                        monitorDataLine.MonitorNeighbors.Add(new MonitorNeighborAttribute()
                                        {
                                            Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                            Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                            Distance = k.Value,
                                            MonitorName = k.Key.MonitorName,
                                            Weight = (1.0000 / Math.Pow(k.Value, 2)) / DicMonitorDistance.Sum(p => (1.000 / Math.Pow(p.Value, 2)))
                                        });
                                    }
                                    //foreach (KeyValuePair<string, ModelAttribute> k in dicModelAttribute)
                                    //{
                                    //    monitorDataLine.ModelAttributes.Add(k.Value);
                                    //}
                                }

                                //fsVoronoi.Dispose();
                                //fsVoronoiInter.Dispose();
                                //fsInter.Dispose();
                                //fsVoronoiInter.Dispose();
                                //GC.Collect();
                                break;
                        }
                        i++;
                    }
                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("ModelCell");
                    //dt.Columns.Add("MonitorName");
                    //dt.Columns.Add("MonitorLatLong");
                    //foreach(KeyValuePair<string,Dictionary<string,string>> k in dicNeighbors)
                    //{
                    //    foreach (KeyValuePair<string, string> kin in k.Value)
                    //    {
                    //        DataRow dr = dt.NewRow();
                    //        dr[0] = k.Key;
                    //        dr[1] = kin.Key;
                    //        dr[2] = kin.Value;
                    //        dt.Rows.Add(dr);
                    //    }
                    //}
                    //BenMAP b = new BenMAP("");
                    //b.SaveCSV(dt,@"D:\Neighbors.csv");
                    fs.Close();
                    fs.Dispose();
                }

                //foreach (MonitorValue monitorValue in lstMonitorValues)
                //{
                /* //如果没有metric ，判断value 大小 +1/365 -> 24 每个小时一个数据  +1/365 1 每天一个数据

                 按照污染物的metric 得到处理好的数据。
                当选择为使用最近监测点的浓度值的插值方式，在Advanced Option窗口中，只能输入最大临近距离（以km为单位）。默认“- No Maximum Distance -”时，即没有最大距离（某格点与监测点之间）的限制，即格点被赋予了离它最近的那个监测点的浓度值——所有格点都有值。
   当输入Maximum Neighbor Distance，设为x，即某监测点A以x为半径的区域内的格点被赋予了相同的监测值。若某格点不在任何监测点所覆盖的区域内，其值则为0。
   注意：Maximum Relative Neighbor Distance和 Weighting Approach是不能更改的，因为在Closet Monitor模式下，BenMAP仅仅选择一个监测点，把浓度值分配给各个格点。
               只有选择Manage Setup中的EPA Standard Monitor Library才可以自定义过滤条件。该选项允许用户对监测数据进行过滤，制图和导出功能。详细功能包括：
  (1)	包括指定监测点
  可以指定特定几个监测站点的ID号，只使用这几个监测站点的数据来进行你的分析。默认为空，则自动选择所有站点。
  (2)	排除指定监测点
  可以排除指定ID号的监测站点数据。默认不排除。
  (3)	规定特定的省和/或经纬度范围
  只选择在指定省份内的监测点数据。输入的省份用两个字母缩写代表，例如，CA代表加利福尼亚州。
  同时，可以选择指定经纬度范围。默认纬度20-50，经度-130- -65，覆盖全美。
  (4)	POC码
  最大POC：指定数据里允许的最高POC值，默认为4。
  POC参考顺序：当同一地点有多于一个监测点，系统会按照该顺序来指定顺序。
  问题：POC具体如何影响计算过程？
  (5)	方法
  可选方法取决于污染物。
  对于O3或PM10，所有方法默认都为选中；对于PM2.5，只有编号116-120的联邦参照方法（FRM）是默认选中的。
  (6)	监测对象
  对于PM10：一般/背景；最高浓度；最大臭氧浓度。
  对于PM2.5和Ozone：一般/背景；最高浓度；极端情况下风区。
  问题：哪里有这些浓度对象？在数据上如何体现不同类型？
  (7)	污染物参数
  根据污染物不同，参数也不同。
  对于PM10和PM2.5：
  Number of Valid Observations Required Per Quarter：指定每个季度至少需要多少天的监测数据。默认11个。
  Data Types to Use：只选用本地数据（参数代号85101）/只选用标准数据（参数代号81102）/两种都使用。PM10：两种都使用。PM2.5：只选用Local。
  Preferred Type：选择使用那种数据类型，默认Local。
  Output Type：保证当两种数据都被选用时，输出数据也合理地保持一致性。默认选择Local，即Standard类型的会被转化为Local类型的格式输出。
  对于Ozone：
  由于Ozone是一种以小时为特征的污染物，因此它的参数与颗粒物有很大差异。
  Number of Valid Hours：指定一天需要有多少小时的数据才能被认为“有效”。BenMAP要计算出从Start Hour到End Hour一共有多少个小时的值，然后对比这个值和Number of Valid Hours是否相符。
  Percent of Valid Days：指定Start Date到End Date之间，至少需要多少百分比的日子是有监测数据的，才能被认为是“有效”的监测数据。默认是从5月1日到9月30日，至少有50%的日子有监测值。
                */

                //}
                //结合Grid转换成模型值
                //SaveBenMAPLineShapeFile(benMAPGrid, benMAPPollutant, monitorDataLine, );
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                // return null;
            }

            //生成SHP
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
                        fs = FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as ShapefileGrid).ShapefileName + ".shp");
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
                        fs = FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as RegularGrid).ShapefileName + ".shp");
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

        public static void UpdataModelValuesMonitorRollbackData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, MonitorModelRollbackLine monitorModelRollbackLine)
        {
        }

        public static BenMAPLine getBenMapLineCopyOnlyResultCopy(BenMAPLine benMAPLine)
        {
            if (benMAPLine is ModelDataLine)
            {
                ModelDataLine benMAPLineCopy = new ModelDataLine();
                benMAPLineCopy.GridType = benMAPLine.GridType;
                benMAPLineCopy.Pollutant = benMAPLine.Pollutant;
                //benMAPLineCopy.ResultCopy = benMAPLine.ResultCopy;
                benMAPLineCopy.ShapeFile = benMAPLine.ShapeFile;
                benMAPLineCopy.DatabaseFilePath = (benMAPLine as ModelDataLine).DatabaseFilePath;
                benMAPLineCopy.DatabaseType = (benMAPLine as ModelDataLine).DatabaseType;

                return benMAPLineCopy;

                //  serializer.WriteObject(stream, benMAPLineCopy);
            }
            else if (benMAPLine is MonitorDataLine)
            {
                MonitorDataLine benMAPLineCopy = new MonitorDataLine();

                benMAPLineCopy.GridType = benMAPLine.GridType;
                benMAPLineCopy.Pollutant = benMAPLine.Pollutant;
                //benMAPLineCopy.ResultCopy = benMAPLine.ResultCopy;
                benMAPLineCopy.ShapeFile = benMAPLine.ShapeFile;
                benMAPLineCopy.MonitorDirectType = (benMAPLine as MonitorDataLine).MonitorDirectType; //0 Library 1 Text File
                benMAPLineCopy.MonitorDataSetID = (benMAPLine as MonitorDataLine).MonitorDataSetID;
                benMAPLineCopy.MonitorLibraryYear = (benMAPLine as MonitorDataLine).MonitorLibraryYear;
                benMAPLineCopy.MonitorDataFilePath = (benMAPLine as MonitorDataLine).MonitorDataFilePath;
                benMAPLineCopy.MonitorDefinitionFile = (benMAPLine as MonitorDataLine).MonitorDefinitionFile;
                benMAPLineCopy.InterpolationMethod = (benMAPLine as MonitorDataLine).InterpolationMethod;
                benMAPLineCopy.FixedRadius = (benMAPLine as MonitorDataLine).FixedRadius;
                benMAPLineCopy.MonitorAdvance = (benMAPLine as MonitorDataLine).MonitorAdvance;

                return benMAPLineCopy;
            }
            else if (benMAPLine is MonitorModelRelativeLine)
            {
            }
            else if (benMAPLine is MonitorModelRollbackLine)
            {
                MonitorModelRollbackLine benMAPLineCopy = new MonitorModelRollbackLine();
                benMAPLineCopy.GridType = benMAPLine.GridType;
                benMAPLineCopy.Pollutant = benMAPLine.Pollutant;
                //benMAPLineCopy.ResultCopy = benMAPLine.ResultCopy;
                benMAPLineCopy.ShapeFile = benMAPLine.ShapeFile;

                benMAPLineCopy.RollbackGrid = (benMAPLine as MonitorModelRollbackLine).RollbackGrid;
                benMAPLineCopy.BenMAPRollbacks = (benMAPLine as MonitorModelRollbackLine).BenMAPRollbacks;
                benMAPLineCopy.ScalingMethod = (benMAPLine as MonitorModelRollbackLine).ScalingMethod;//0- None 1-SpatialOnly;
                benMAPLineCopy.AdditionalGrid = (benMAPLine as MonitorModelRollbackLine).AdditionalGrid;
                benMAPLineCopy.AdustmentFilePath = (benMAPLine as MonitorModelRollbackLine).AdustmentFilePath;
                benMAPLineCopy.isMakeBaseLineGrid = (benMAPLine as MonitorModelRollbackLine).isMakeBaseLineGrid;
                return benMAPLineCopy;
            }
            return null;
        }

        /// <summary>
        /// 生成空气质量网格*.aqg文件-json
        /// </summary>
        /// <param name="benMAPLine"></param>
        /// <param name="strAQGPath">空气质量网格*.aqg存储路径</param>
        public static void CreateAQGFromBenMAPLine(BenMAPLine benMAPLine, string strAQGPath)
        {
            try
            {
                if (File.Exists(strAQGPath))
                    File.Delete(strAQGPath);
                if (benMAPLine.ModelAttributes!=null)
                {
                    benMAPLine.ModelAttributes.Clear();
                    GC.Collect();

                }
                using (FileStream fs = new FileStream(strAQGPath, FileMode.OpenOrCreate))
                {
                    if(benMAPLine.GridType==null && CommonClass.GBenMAPGrid!=null)      benMAPLine.GridType = CommonClass.GBenMAPGrid;
                    benMAPLine.CreateTime = DateTime.Now;
                    benMAPLine.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                    Serializer.Serialize<BenMAPLine>(fs, benMAPLine);
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

                List<Type> knownTypes = new List<Type>();
                knownTypes.Add(typeof(List<CustomerMetric>));
                knownTypes.Add(typeof(List<FixedWindowMetric>));
                knownTypes.Add(typeof(List<MovingWindowMetric>));
                knownTypes.Add(typeof(ModelDataLine));
                knownTypes.Add(typeof(MonitorDataLine));
                knownTypes.Add(typeof(MonitorModelRelativeLine));
                knownTypes.Add(typeof(MonitorModelRollbackLine));
                knownTypes.Add(typeof(ShapefileGrid));
                knownTypes.Add(typeof(RegularGrid));
                var serializer = new DataContractJsonSerializer(typeof(BenMAPLine), knownTypes);
                MemoryStream stream = new MemoryStream();

                //serializer.WriteObject(stream, getBenMapLineCopyOnlyResultCopy(benMAPLine));

                if (File.Exists(strAQGPath))
                {
                    File.Delete(strAQGPath);
                }
                using (FileStream fs = new FileStream(strAQGPath, FileMode.OpenOrCreate))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, getBenMapLineCopyOnlyResultCopy(benMAPLine));
                    fs.Close();
                    fs.Dispose();
                }

                //FileStream fs = new FileStream(strAQGPath, FileMode.OpenOrCreate);
                //fs.Write(stream.ToArray(), 0, stream.ToArray().Count());
                ////BinaryWriter w = new BinaryWriter(fs);
                ////w.Write(stream.ToArray());//dataBytes);
                ////w.Close();
                //fs.Close();
                //stream.Dispose();
                //MemoryStream ms = new MemoryStream();
                //JsonSerializer serializer = new JsonSerializer();

                ////Serialize product to BSON
                //BsonWriter writer = new BsonWriter(ms);
                //serializer.Serialize(writer, benMAPLine);
                ////strAQGPath = @"D:\control.aqg";
                //FileStream fs = new FileStream(strAQGPath, FileMode.OpenOrCreate);
                //BinaryWriter w = new BinaryWriter(fs);
                //w.Write(ms.ToArray());//dataBytes);
                //w.Close();
                //fs.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                // return null;
            }
        }

        /// <summary>
        /// 加载空气质量网格文件
        /// </summary>
        /// <param name="strAQGPath"></param>
        /// <returns></returns>
        public static BenMAPLine LoadAQGFile(string strAQGPath,ref string err)
        {
            //FileStream fs = null;
             
                using (FileStream fs = new FileStream(strAQGPath, FileMode.Open))
                {
                    try
                    {
                        BenMAPLine benMAPLine = Serializer.Deserialize<BenMAPLine>(fs);
                        List<ModelResultAttribute> lstRemove = new List<ModelResultAttribute>();
                        foreach (ModelResultAttribute m in benMAPLine.ModelResultAttributes)
                        {
                            if (m.Values == null || m.Values.Count == 0)
                            {
                                lstRemove.Add(m);
                            }
                        }
                        foreach (ModelResultAttribute m in lstRemove)
                        {
                            benMAPLine.ModelResultAttributes.Remove(m);
                        }
                        //----------Modify setup from setupname --Modify GridDefinition from Name Modify Pollutant from Name
                        BenMAPSetup benMAPSetup = null;
                        if (benMAPLine.GridType != null)
                        {
                           benMAPSetup= CommonClass.getBenMAPSetupFromName(benMAPLine.GridType.SetupName);
                        }
                        if (benMAPSetup == null)
                        {
                            err = "The setup name \"" + benMAPLine.GridType.SetupName + "\" can't be found in the database.";
                            return null;
                        }

                        BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromName(benMAPLine.GridType.GridDefinitionName, benMAPSetup);
                        if (benMAPGrid == null)
                        {
                            err = "The grid definition name \"" + benMAPLine.GridType.GridDefinitionName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
                            return null;
                        }
                        benMAPLine.GridType = benMAPGrid;

                        BenMAPPollutant pollutant = Grid.GridCommon.getPollutantFromName(benMAPLine.Pollutant.PollutantName, benMAPSetup.SetupID);
                        if (pollutant == null)
                        {
                            err = "The pollutant name \"" + benMAPLine.Pollutant.PollutantName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
                            return null;
                        }
                        benMAPLine.Pollutant = pollutant;
                        //Serializer.Serialize<List<BaseControlGroup>>(fs, baseControlCRSelectFunctionCalculateValue.BaseControlGroup);
                        //Serializer.Serialize<BaseControlCRSelectFunctionCalculateValue>(fs, baseControlCRSelectFunctionCalculateValue);
                        //fs.Flush();
                        //fs.Position = 0;

                        //TestObject obj2 = Serializer.Deserialize<TestObject>(fs);
                        //Console.WriteLine(obj2);  
                        fs.Close();
                        fs.Dispose();
                        return benMAPLine;
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                        fs.Close();
                        fs.Dispose();
                        err = "BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.";
                        return null;
                    }
                }
                //BenMAPLine benMAPLine = new BenMAPLine();
                ////List<Type> knownTypes = new List<Type>();
                ////knownTypes.Add(typeof(List<CustomerMetric>));
                ////knownTypes.Add(typeof(List<FixedWindowMetric>));
                ////knownTypes.Add(typeof(List<MovingWindowMetric>));
                ////knownTypes.Add(typeof(ModelDataLine));
                ////knownTypes.Add(typeof(MonitorDataLine));
                ////knownTypes.Add(typeof(MonitorModelRelativeLine));
                ////knownTypes.Add(typeof(MonitorModelRollbackLine));
                ////knownTypes.Add(typeof(ShapefileGrid));
                ////knownTypes.Add(typeof(RegularGrid));

                ////  fs = new FileStream(strAQGPath, FileMode.Open);
                ////// BinaryReader brtest = new BinaryReader(fstest);
                ////byte[] data = new byte[fs.Length];
                ////fs.Read(data, 0, data.Length);
                ////var serializertest = new DataContractJsonSerializer(typeof(BenMAPLine), knownTypes);
                ////var mStream = new MemoryStream(data);
                //////BinaryReader br = new BinaryReader(fs);
                //////MemoryStream memoryFile = GridCommon.DecryptToMemoryStream(strFile, "&%#@?,:*");
                ////benMAPLine = (BenMAPLine)serializertest.ReadObject(mStream);
                //using (FileStream fs = new FileStream(strAQGPath, FileMode.Open))
                //{
                //    BinaryFormatter formatter = new BinaryFormatter();
                //    benMAPLine = (BenMAPLine)formatter.Deserialize(fs);//在这里大家要注意咯,他的返回值是object
                //    fs.Close();
                //    fs.Dispose();
                //    GC.Collect();
                //}
                ////通过resultcopy生成数据
                //getModelValuesFromResultCopy(ref benMAPLine);
                ////mStream.Close();

                ////JsonSerializer serializer = new JsonSerializer();
                ////FileStream fs = new FileStream(strAQGPath, FileMode.Open);
                ////// BinaryReader brtest = new BinaryReader(fstest);
                ////byte[] data = new byte[fs.Length];
                ////fs.Read(data, 0, data.Length);
                //////var serializertest = new DataContractJsonSerializer(typeof(BenMAPLine), knownTypes);
                ////var mStream = new MemoryStream(data);
                //////BinaryReader br = new BinaryReader(fs);
                //////MemoryStream memoryFile = GridCommon.DecryptToMemoryStream(strFile, "&%#@?,:*");
                ////BsonReader reader = new BsonReader(mStream);
                ////BenMAPLine benMAPLine = serializer.Deserialize<BenMAPLine>(reader);
                ////mStream.Close();
                ////fs.Close();

                //return benMAPLine;
            
        }

        public static void getModelValuesFromResultCopy(ref BenMAPLine benMAPLine)
        {
            benMAPLine.ModelResultAttributes = new List<ModelResultAttribute>();
            List<string> lstAddField = new List<string>();

            foreach (Metric metric in benMAPLine.Pollutant.Metrics)
            {
                lstAddField.Add(metric.MetricName);
            }
            foreach (SeasonalMetric sesonalMetric in benMAPLine.Pollutant.SesonalMetrics)
            {
                lstAddField.Add(sesonalMetric.SeasonalMetricName);
            }
            //foreach (float[] d in benMAPLine.ResultCopy)
            //{
            //    ModelResultAttribute modelResultAttribute = new ModelResultAttribute();
            //    modelResultAttribute.Col = Convert.ToInt32(d[0]);
            //    modelResultAttribute.Row = Convert.ToInt32(d[1]);
            //    modelResultAttribute.Values = new Dictionary<string, float>();

            //    for (int i = 0; i < lstAddField.Count; i++)
            //    {
            //        modelResultAttribute.Values.Add(lstAddField[i], d[2 + i]);
            //    }
            //    benMAPLine.ModelResultAttributes.Add(modelResultAttribute);
            //}
        }

        public static void UpdateModelValuesModelData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, ModelDataLine modelDataLine)
        {
            throw new NotImplementedException();
        }
    }

    public static class EnumerableExtensions
    {
        public static double Median(this IEnumerable<double> list)
        {         // Implementation goes here.
            //list.ToList().Sort();
            //list = list.OrderBy(p => p);
            double[] lstarray = list.ToArray();
            int midPoint;
            double median, sum;
            int items = list.Count()+1;
            sum = 0.0;

            if (((int)Math.Round((double)items / 2.0) * 2) != items)
            {
                midPoint = items / 2;

                sum = lstarray[midPoint-1];
                sum += lstarray[midPoint ];
                sum /= 2.0;
            }
            else
            {
                midPoint = (items / 2) ;
                sum = lstarray[midPoint];
            }

            median = sum;
            return median;

            //return list.ToList()[Convert.ToInt32(list.Count() / 2)];
        }

        public static float Median(this IEnumerable<float> list)
        {         // Implementation goes here.
            //list = list.OrderBy(p => p);
            float[] lstarray = list.ToArray();
            //lstarray.Sort();
            int midPoint;
            float median, sum;
            int items = list.Count()+1;
            sum = 0;

            //if (((int)Math.Round((float)items / 2.0) * 2) != items)
            if(items % 2==1)
            {
                midPoint = items / 2;

                sum = lstarray[midPoint-1];
                sum += lstarray[midPoint ];
                sum /= 2;
            }
            else
            {
                midPoint = (items / 2) ;
                sum = lstarray[midPoint-1];
            }

            median = sum;
            return median;
            //list.ToList().Sort();
            //return list.ToList()[Convert.ToInt32(list.Count() / 2)];
        }
    }
}