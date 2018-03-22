using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Diagnostics;
using DotSpatial.Data;
using DotSpatial.NTSExtension.Voronoi;
using FirebirdSql.Data.FirebirdClient;
using ProtoBuf;

using LumenWorks.Framework.IO.Csv;
using System.Reflection;
using DotSpatial.NTSExtension;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace BenMAP
{
    public class DataSourceCommonClass
    {
        public static Dictionary<string, string> _dicSeasonStaticsAll;

        public static Dictionary<string, string> DicSeasonStaticsAll
        {
            get
            {
                if (_dicSeasonStaticsAll == null)
                {
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    string commandText = string.Format("select * from SEASONALMETRICSEASONS");
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    _dicSeasonStaticsAll = new Dictionary<string, string>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        _dicSeasonStaticsAll.Add(dr["StartDay"].ToString() + "," + dr["SeasonalMetricID"].ToString(), dr["METRICFUNCTION"].ToString());
                    }

                }
                return _dicSeasonStaticsAll;
            }
            set { _dicSeasonStaticsAll = value; }
        }
        public static void UpdateModelValues(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, BenMAPLine benMAPLine)
        {
            string s = "";
            s.ToString();
        }
        public static System.Data.DataSet getDataSetFromCSV(string strPath)
        {




            System.Data.DataSet ds = new System.Data.DataSet();

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

                    }
                    dt.Rows.Add(dr);
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }
        public static System.Data.DataTable getDataTableFromCSV(string strPath)
        {






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

                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        public static void UpdateModelDataLineFromDataSet(BenMAPPollutant benMAPPollutant, ModelDataLine modelDataLine, System.Data.DataTable dtModel)
        {
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
                    if (drModel[iMetric] == null || drModel[iMetric].ToString() == "")
                        modelAttribute.Metric = null;
                    else
                    {
                        if (dicMetric.ContainsKey(drModel[iMetric].ToString().ToLower()))
                        {
                            modelAttribute.Metric = dicMetric[drModel[iMetric].ToString().ToLower()];
                        }

                    }

                    if (drModel[iSeasonalMetric] == null || drModel[iSeasonalMetric].ToString() == "")
                        modelAttribute.SeasonalMetric = null;
                    else
                    {

                        if (dicSeasonalMetric.ContainsKey(drModel[iSeasonalMetric].ToString().ToLower()))
                        {
                            modelAttribute.SeasonalMetric = dicSeasonalMetric[drModel[iSeasonalMetric].ToString().ToLower()];
                        }
                    }
                    if (drModel[iStatistic] == null || drModel[iStatistic].ToString() == "")
                        modelAttribute.Statistic = MetricStatic.None;
                    else
                    {
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
            }
        }

        public static void SaveModelDataLineToNewFormatCSV(BenMAPLine modelDataLine, string strFile)
        {
            try
            {
                string sfirst = "\"" + modelDataLine.Pollutant.PollutantName + "," + modelDataLine.GridType.GridDefinitionName + ",Model" + ", Annual Mean\"";
                FileStream fs = new FileStream(strFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine(sfirst);
                string data = "Col,Row,";

                for (int i = 0; i < modelDataLine.ModelResultAttributes.First().Values.Count; i++)
                {
                    if (modelDataLine.ModelResultAttributes.First().Values.Keys.ToArray()[i].Contains(",")) continue;
                    data += modelDataLine.ModelResultAttributes.First().Values.Keys.ToArray()[i];
                    if (i < modelDataLine.ModelResultAttributes.First().Values.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);

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

        public static void UpdateModelDataLineFromDataSetNewFormat(BenMAPPollutant benMAPPollutant, ref ModelDataLine modelDataLine, System.Data.DataSet dsModel)
        {
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

                    modelDataLine.ModelResultAttributes.Add(modelAttribute);
                }
                dsModel = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public static void UpdateModelValuesModelData(Dictionary<string, string> dicSeasonStatics, BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, ModelDataLine modelDataLine, string strShapeFile)
        {
            try
            {
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
                int i = 0;
                modelDataLine.ModelResultAttributes = new List<ModelResultAttribute>();

                List<RowCol> lstRowCol = new List<RowCol>(); Dictionary<string, RowCol> dicRowCol = new Dictionary<string, RowCol>();
                Dictionary<string, ModelResultAttribute> dicModelResultAttribute = new Dictionary<string, ModelResultAttribute>();
                foreach (ModelAttribute ma in modelDataLine.ModelAttributes)
                {
                    if (!dicRowCol.ContainsKey(ma.Col + "," + ma.Row))
                    {
                        dicRowCol.Add(ma.Col + "," + ma.Row, new RowCol() { Col = ma.Col, Row = ma.Row });
                        dicModelResultAttribute.Add(ma.Col + "," + ma.Row, new ModelResultAttribute() { Col = ma.Col, Row = ma.Row, Values = new Dictionary<string, float>() });
                    }
                }
                lstRowCol = dicRowCol.Values.ToList(); List<ModelAttribute> lstModelAttribute365 = new List<ModelAttribute>();
                foreach (Metric metric in benMAPPollutant.Metrics)
                {
                    MetricStatic metricStatic = new MetricStatic();
                    metricStatic = MetricStatic.Mean;
                    if (metric is FixedWindowMetric)
                        metricStatic = (metric as FixedWindowMetric).Statistic;
                    else if (metric is MovingWindowMetric)
                        metricStatic = (metric as MovingWindowMetric).WindowStatistic;
                    var group = from a in modelDataLine.ModelAttributes where a.Metric == metric || a.Metric == null group a by new { a.Col, a.Row } into g select g;
                    foreach (var ingroup in group)
                    {
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
                                            dicModelResultAttribute[m.Col + "," + m.Row].Values.Add(m.Metric.MetricName + ",Median", m.Values.OrderBy(p => p).Median());
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
                                            List<float> lstTemp = m.Values;
                                            lstModelAttribute365.Add(new ModelAttribute() { Col = m.Col, Row = m.Row, Metric = metric, Values = lstTemp });
                                        }
                                        break;
                                }
                            }


                        }
                        foreach (ModelAttribute m in ingroup)
                        {
                            if (m.SeasonalMetric != null)
                            {
                                switch (m.Statistic)
                                {
                                    case MetricStatic.Max:
                                        if (!dicModelResultAttribute[m.Col + "," + m.Row].Values.ContainsKey(m.Metric.MetricName + ",Max"))
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
                                        }
                                        break;
                                }
                            }


                        }
                        ModelAttribute mAttribute = null;
                        var staticquery = from a in ingroup where a.Statistic == metricStatic select a;
                        if (staticquery != null && staticquery.Count() > 0) { mAttribute = staticquery.First(); }
                        else
                        { mAttribute = ingroup.First(); }
                        ModelResultAttribute mrAttribute;
                        mrAttribute = dicModelResultAttribute[mAttribute.Col + "," + mAttribute.Row];
                        int hourly = 0;
                        if (mAttribute.Values.Count >= 8759) hourly = 1;
                        if (metric is FixedWindowMetric)
                        {
                            FixedWindowMetric fixedWindowMetric = (FixedWindowMetric)metric;
                            if (hourly == 0)
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
                                Dictionary<int, List<float>> dicHourlyValue = new Dictionary<int, List<float>>();
                                i = 0;
                                while (i < 365)
                                {
                                    try
                                    {
                                        List<float> lstTemp = new List<float>();
                                        if (i * 24 + fixedWindowMetric.StartHour < mAttribute.Values.Count && i * 24 + fixedWindowMetric.StartHour + fixedWindowMetric.EndHour - fixedWindowMetric.StartHour < mAttribute.Values.Count)
                                            lstTemp = mAttribute.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour + 1);
                                        else
                                            lstTemp = mAttribute.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, mAttribute.Values.Count - (i * 24 + fixedWindowMetric.StartHour));
                                        lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                                        if (lstTemp != null && lstTemp.Count > 0)
                                            dicHourlyValue.Add(i, lstTemp);
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
                                            lstTempDayValue.Add(iTemp, iTemp);
                                        }
                                    }
                                    dicHourlyValue = dicHourlyValue.Where(p => lstTempDayValue.ContainsKey(p.Key)).ToDictionary(p => p.Key, p => p.Value);
                                }


                                switch (fixedWindowMetric.Statistic)
                                {
                                    case MetricStatic.Max:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Max(p => p.Max()));
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Max()).ToList()
                                        });
                                        break;
                                    case MetricStatic.Mean:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Average()));
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Average()).ToList()
                                        });

                                        break;
                                    case MetricStatic.Median:
                                        List<float> lstTemp = new List<float>();
                                        foreach (List<float> ld in dicHourlyValue.Values)
                                        {
                                            if (!(ld.Count == 1 && ld[0] == float.MinValue))
                                                lstTemp.Add(ld.OrderBy(p => p).Median());
                                        }
                                        mrAttribute.Values.Add(metric.MetricName, lstTemp.OrderBy(p => p).Median()); lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.OrderBy(a => a).Median()).ToList()
                                        });

                                        break;
                                    case MetricStatic.Min:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Min(p => p.Min()));
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Min()).ToList()
                                        });

                                        break;
                                    case MetricStatic.None:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Average()));
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Average()).ToList()
                                        });

                                        break;
                                    case MetricStatic.Sum:
                                        mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Sum(p => p.Sum()));
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Sum()).ToList()
                                        });

                                        break;
                                }
                            }

                        }
                        else if (metric is MovingWindowMetric)
                        {
                            MovingWindowMetric movingWindowMetric = (MovingWindowMetric)metric;
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
                                            mrAttribute.Values.Add(metric.MetricName, lstmAttribute.OrderBy(p => p).Median()); break;
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
                                Dictionary<int, List<float>> dicHourlyValue = new Dictionary<int, List<float>>();
                                i = 0;

                                while (i < 365)
                                {
                                    List<float> lstTemp = mAttribute.Values.GetRange(i * 24, 24);
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
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Max(p => p.Max()));
                                                break;
                                            case MetricStatic.Mean:
                                                mrAttribute.Values.Add(metric.MetricName, dicHourlyValue.Values.Where(p => !(p.Count == 1 && p[0] == float.MinValue)).Average(p => p.Max()));
                                                break;
                                            case MetricStatic.Median:
                                                List<float> lstTempIn = new List<float>();
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
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Max()).ToList()
                                        });


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
                                                    if (!(k.Count == 1 && k[0] == float.MinValue))
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
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Average()).ToList()
                                        });

                                        break;
                                    case MetricStatic.Median:
                                        List<float> lstTemp = new List<float>();
                                        foreach (List<float> ld in dicHourlyValue.Values)
                                        {
                                            if (!(ld.Count == 1 && ld[0] == float.MinValue))
                                                lstTemp.Add(ld.OrderBy(p => p).Median());
                                        }
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
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
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
                                                    if (!(k.Count == 1 && k[0] == float.MinValue))
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
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Min()).ToList()
                                        });


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
                                                    if (!(k.Count == 1 && k[0] == float.MinValue))
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
                                        lstModelAttribute365.Add(new ModelAttribute()
                                        {
                                            Col = mrAttribute.Col,
                                            Row = mrAttribute.Row,
                                            Metric = metric,
                                            Statistic = MetricStatic.None,
                                            Values =
                                                dicHourlyValue.Values.Select(p => p.Sum()).ToList()
                                        });

                                        break;
                                }
                            }
                        }
                        else if (metric is CustomerMetric)
                        {
                        }
                        else
                        {

                            List<float> lstmAttribute = mAttribute.Values.Where(p => p != float.MinValue).ToList();
                            if (lstmAttribute != null && lstmAttribute.Count > 0)
                                mrAttribute.Values.Add(metric.MetricName, lstmAttribute.Average());
                            else
                                mrAttribute.Values.Add(metric.MetricName, 0);
                        }
                    }
                }
                foreach (SeasonalMetric seasonalmetric in benMAPPollutant.SesonalMetrics)
                {
                    MetricStatic metricStatic = new MetricStatic();
                    metricStatic = MetricStatic.Mean;
                    if (seasonalmetric.Metric is FixedWindowMetric)
                        metricStatic = (seasonalmetric.Metric as FixedWindowMetric).Statistic;
                    else if (seasonalmetric.Metric is MovingWindowMetric)
                        metricStatic = (seasonalmetric.Metric as MovingWindowMetric).WindowStatistic;
                    var group = from a in modelDataLine.ModelAttributes where a.SeasonalMetric == seasonalmetric group a by new { a.Col, a.Row } into g select g;
                    if (group != null && group.Count() > 0)
                    {
                        foreach (var ingroup in group)
                        {
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
                            ModelAttribute mAttribute = null;
                            var staticquery = from a in ingroup where a.Statistic == metricStatic select a;
                            if (staticquery != null && staticquery.Count() > 0)
                                mAttribute = staticquery.First();
                            else
                            {
                                mAttribute = ingroup.First();
                            }

                            ModelResultAttribute mrAttribute;
                            mrAttribute = dicModelResultAttribute[mAttribute.Col + "," + mAttribute.Row];
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
                        }
                    }
                    else 
                    {
                        var groupSeasonal = from a in lstModelAttribute365 where a.Metric != null && a.Metric.MetricID == seasonalmetric.Metric.MetricID group a by new { a.Col, a.Row } into g select g;
                        List<ModelAttribute> lstSeasonalAdd = new List<ModelAttribute>();
                        if (groupSeasonal == null || groupSeasonal.Count() == 0)
                            groupSeasonal = from a in lstModelAttribute365 where a.Metric == null group a by new { a.Col, a.Row } into g select g; if (groupSeasonal != null && groupSeasonal.Count() > 0)
                        {
                            foreach (var ingroup in groupSeasonal)
                            {
                                ModelAttribute mAttribute = ingroup.First();
                                ModelResultAttribute mrAttribute;
                                mrAttribute = dicModelResultAttribute[mAttribute.Col + "," + mAttribute.Row];
                                List<float> lstQuality = new List<float>();
                                if (seasonalmetric.Seasons == null || seasonalmetric.Seasons.Count == 0)
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
                                        switch (dicSeasonStatics[s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()])
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
                modelDataLine.ModelAttributes = lstModelAttribute365;
                List<string> lstColRow = dicModelResultAttribute.Select(p => p.Key).ToList();
                foreach (string s in lstColRow)
                {
                    var queryMinValue = dicModelResultAttribute[s].Values.Where(p => p.Value == float.MinValue).ToList();
                    foreach (KeyValuePair<string, float> k in queryMinValue)
                    {
                        dicModelResultAttribute[s].Values.Remove(k.Key);
                    }
                    if (dicModelResultAttribute[s].Values.Count == 0) dicModelResultAttribute.Remove(s);
                }
                modelDataLine.ModelResultAttributes = dicModelResultAttribute.Values.ToList();
                if (strShapeFile != "")
                    SaveBenMAPLineShapeFile(benMAPGrid, benMAPPollutant, modelDataLine, strShapeFile);
                GC.Collect();








            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public static void SaveBenMAPLineShapeFile(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, BenMAPLine modelDataLine, string strShapeFile)
        {
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
            if (benMAPGrid is ShapefileGrid) shapeFileName = (benMAPGrid as ShapefileGrid).ShapefileName;
            if (benMAPGrid is RegularGrid) shapeFileName = (benMAPGrid as RegularGrid).ShapefileName;
            fs = FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + shapeFileName + ".shp");

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
            Dictionary<string, Dictionary<string, float>> DicResult = new Dictionary<string, Dictionary<string, float>>();
            if (modelDataLine.ModelResultAttributes != null)
            {
                foreach (ModelResultAttribute mra in modelDataLine.ModelResultAttributes)
                {
                    DicResult.Add(mra.Col + "," + mra.Row, mra.Values);
                }
            }
            else  
            {
                Debug.WriteLine("SaveBenMAPLineShapeFile: modelDataLine.ModelResultAttributes is null");
            }
            while (i < fs.DataTable.Rows.Count)
            {
                try
                {
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
                                fs.DataTable.Rows[i][sField] = Math.Round(kyp[sField], 2);
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
                        fs.DataTable.Rows[i][sField] = Convert.ToDouble(0.00);
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

            }
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
            try
            {
                int hourly = 0; int i = 0;
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
                        if (monitorValue.Statistic.ToLower() == "none" || monitorValue.Statistic.Trim() == "")
                        {
                            monitorValue.dicMetricValues365.Add(monitorValue.SeasonalMetric.SeasonalMetricName, monitorValue.Values);
                            List<float> lstQuality = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                            if (lstQuality.Where(p => p != float.MinValue).Count() > 0)
                            {
                                monitorValue.dicMetricValues[monitorValue.Metric.MetricName] = lstQuality.Where(p => p != float.MinValue).Average();
                                monitorValue.dicMetricValues[monitorValue.SeasonalMetric.SeasonalMetricName] = lstQuality.Where(p => p != float.MinValue).Average();
                            }
                        }
                        else
                        {
                            monitorValue.dicMetricValues.Add(monitorValue.SeasonalMetric.SeasonalMetricName, monitorValue.Values.First());
                            monitorValue.dicMetricValues.Add(monitorValue.Metric.MetricName, monitorValue.Values.First());
                        }
                    }
                    else if (monitorValue.Metric != null)
                    {
                        if (monitorValue.Statistic.ToLower() == "none" || monitorValue.Statistic.Trim() == "")
                        {
                            Metric m = monitorValue.Metric;
                            if (m is FixedWindowMetric)
                            {
                                fixedWindowMetric = (FixedWindowMetric)m;
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
                                            monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Median());
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
                            else if (m is MovingWindowMetric)
                            {
                                movingWindowMetric = (MovingWindowMetric)m;
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
                                            monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Median());
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
                            else if (m is CustomerMetric)
                            { }
                            
                            if (benMAPPollutant.SesonalMetrics != null && monitorValue.dicMetricValues365.Count > 0)
                            {
                                foreach (SeasonalMetric seasonalmetric in benMAPPollutant.SesonalMetrics)
                                {
                                    if (seasonalmetric.Metric.MetricID == monitorValue.Metric.MetricID)
                                    {
                                        List<float> lstQuality = new List<float>();
                                        if ((seasonalmetric.Seasons == null || seasonalmetric.Seasons.Count == 0) && monitorValue.dicMetricValues365.ContainsKey(seasonalmetric.Metric.MetricName))
                                        {
                                            lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Average());
                                            lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Average());
                                            lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Average());
                                            lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Average());

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
                                                switch (DicSeasonStaticsAll[s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()])
                                                {
                                                    case "":
                                                    case "Mean":
                                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Average() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Average());
                                                        break;
                                                    case "Median":
                                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].OrderBy(p => p).Median() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).OrderBy(p => p).Median());
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
                                    }
                                }
                            }
                        }
                        else
                        {
                            monitorValue.dicMetricValues.Add(monitorValue.Metric.MetricName, monitorValue.Values.First());
                        }
                    }
                    else
                    {
                        if (monitorValue.Values.Count >= 8759) hourly = 1;
                        if (benMAPPollutant.Metrics != null && benMAPPollutant.Metrics.Count > 0)
                        {
                            foreach (Metric m in benMAPPollutant.Metrics)
                            {
                                if (m is FixedWindowMetric)
                                {
                                    fixedWindowMetric = (FixedWindowMetric)m;
                                    if (hourly == 0)
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
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue[lstMonitorValue.Count / 2]); break;
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
                                                    dicHourlyValue.Add(i, lstTemp); dicHourlyValue365.Add(i, lstTemp);
                                                }
                                                else
                                                {
                                                    dicHourlyValue365.Add(i, new List<float>() { float.MinValue });
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
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
                                                    lstTemp.Add(ld.OrderBy(p => p).Median());
                                                }
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p => p).Median()); monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.OrderBy(p => p).Select(p => p.Median()).ToList());
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
                                    }

                                }
                                else if (m is MovingWindowMetric)
                                {
                                    movingWindowMetric = (MovingWindowMetric)m;
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
                                                    monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue[lstMonitorValue.Count / 2]); break;
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
                                        dicHourlyValue = new Dictionary<int, List<float>>();
                                        dicHourlyValue365 = new Dictionary<int, List<float>>();
                                        i = 0;

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
                                                    lstTemp.Add(ld.OrderBy(p => p).Median());
                                                }
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p => p).Median()); switch (movingWindowMetric.WindowStatistic)
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
                                                monitorValue.dicMetricValues365.Add(m.MetricName, dicHourlyValue365.Values.Select(p => p.OrderBy(a => a).Median()).ToList());
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
                                    }
                                }
                                else if (m is CustomerMetric)
                                {
                                }
                                else
                                {

                                    List<float> lstMonitorValue = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                                    if (lstMonitorValue != null && lstMonitorValue.Count > 0)
                                        monitorValue.dicMetricValues.Add(m.MetricName, lstMonitorValue.Average());
                                    else
                                        monitorValue.dicMetricValues.Add(m.MetricName, 0);
                                }
                            }
                            if (benMAPPollutant.SesonalMetrics != null && monitorValue.dicMetricValues365.Count > 0)
                            {

                                foreach (SeasonalMetric seasonalmetric in benMAPPollutant.SesonalMetrics)
                                {
                                    List<float> lstQuality = new List<float>();
                                    if ((seasonalmetric.Seasons == null || seasonalmetric.Seasons.Count == 0) && monitorValue.dicMetricValues365.ContainsKey(seasonalmetric.Metric.MetricName))
                                    {
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Average());
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Average());
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Average());
                                        lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                            float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Average());
                                        //YY: lstQuality is not used????
                                        if (monitorValue.dicMetricValues.Keys.Contains(seasonalmetric.Metric.MetricName))
                                        {
                                            monitorValue.dicMetricValues.Add(seasonalmetric.SeasonalMetricName, monitorValue.dicMetricValues[seasonalmetric.Metric.MetricName]);
                                        }
                                        //YY: if the seasonal metric has the same name as processed metric, use the same values as the processed metric??????
                                    }
                                    else
                                    {
                                        foreach (Season s in seasonalmetric.Seasons)
                                        {
                                            if (!DicSeasonStaticsAll.ContainsKey(s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()))
                                                _dicSeasonStaticsAll = null;
                                            switch (DicSeasonStaticsAll[s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()])
                                            {
                                                case "":
                                                case "Mean":
                                                    lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Average() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                        float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Average());
                                                    break;
                                                case "Median":
                                                    lstQuality.Add(monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].OrderBy(p => p).Median() : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
                                                        float.MinValue : monitorValue.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).OrderBy(p => p).Median());
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
                                }

                            }
                        }
                    }
                }

                //YY: monitorValue.dicMetricValues365[pollutant metric] should fill missing (daily) values by using average of the season
                //YY: monitorValue.dicMetricValues365[Seasonal metric] can be left as is ??? If monitor is missing one season, use other points instead????
                //Fill missing daily values
                foreach (MonitorValue monitorValue in lstMonitorValues)
                {
                    foreach (KeyValuePair<string, List<float>> metricMonitor in monitorValue.dicMetricValues365)
                    {
                        if (metricMonitor.Value.Count >= 365) //daily
                        {

                            foreach (Season s in benMAPPollutant.Seasons)
                            {
                                float seasonalAverage = metricMonitor.Value.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Average();
                                for (i = s.StartDay; i <= s.EndDay; i++)
                                {
                                    if (metricMonitor.Value[i] == float.MinValue)
                                    {
                                        metricMonitor.Value[i] = seasonalAverage;
                                    }
                                }
                            }


                        }
                        else if (metricMonitor.Value.Count < 365) //seasonal or annual???
                        {
                            // do nothing. HIF will be able to handle missing values.
                        }
                    }


                }


            }
            catch (Exception ex)
            {

            }
        }

        public static double getDistanceFrom2Point(Point start, Point end)
        {
            return 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((start.Y / 180 * Math.PI - end.Y / 180 * Math.PI) / 2)), 2) +
Math.Cos(start.Y / 180 * Math.PI) * Math.Cos(end.Y / 180 * Math.PI) * Math.Pow(Math.Sin((start.X / 180 * Math.PI - end.X / 180 * Math.PI) / 2), 2))) * 6371.000;
        }

        public static float getDistanceFrom2Point(double X0, double Y0, double X1, double Y1)
        {
            return Convert.ToSingle(2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((Y0 / 180 * Math.PI - Y1 / 180 * Math.PI) / 2)), 2) +
Math.Cos(Y0 / 180 * Math.PI) * Math.Cos(Y1 / 180 * Math.PI) * Math.Pow(Math.Sin((X0 / 180 * Math.PI - X1 / 180 * Math.PI) / 2), 2))) * 6371.000);
        }

        public static double getDistanceFromExtent(Coordinate coordinate, IEnvelope env, Point end)
        {
            double d = Math.Sqrt((coordinate.X - end.X) * (coordinate.X - end.X) + (coordinate.Y - end.Y) * (coordinate.Y - end.Y)) * 111.0000; if (d < env.Height / 2.00 && d < env.Width / 2.00)
            {
                return 0.0;
            }
            else
            {
                return env.Height > env.Width ? d - env.Width / 2.00 : d - env.Height;
            }
        }

        private static void HandleBoundaries(VoronoiGraph graph, IEnvelope bounds)
        {
            List<ILineString> boundSegments = new List<ILineString>();
            List<VoronoiEdge> unboundEdges = new List<VoronoiEdge>();

            foreach (VoronoiEdge edge in graph.Edges)
            {
                if (edge.VVertexA.ContainsNan() || edge.VVertexB.ContainsNan())
                {
                    unboundEdges.Add(edge);
                    continue;
                }

                boundSegments.Add(new LineString(new[] { edge.VVertexA.ToCoordinate(), edge.VVertexB.ToCoordinate() }));
            }

            IEnvelope env = bounds;
            double h = env.Height;
            double w = env.Width;
            double len = Math.Sqrt((w * w) + (h * h));
            foreach (VoronoiEdge edge in unboundEdges)
            {
                Coordinate start = (edge.VVertexB.ContainsNan())
                        ? edge.VVertexA.ToCoordinate()
                        : edge.VVertexB.ToCoordinate();

                double dx = edge.LeftData.X - edge.RightData.X;
                double dy = edge.LeftData.Y - edge.RightData.Y;
                double l = Math.Sqrt((dx * dx) + (dy * dy));

                double sx = -dy / l;
                double sy = dx / l;

                var center = new Coordinate(bounds.Centre);
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
                    edge.VVertexA = new Vector2(end.X, end.Y);
                }
                else
                {
                    edge.VVertexB = new Vector2(end.X, end.Y);
                }
            }
        }

        public static void VoronoiPolygons(IFeatureSet points, ref List<Polygon> result, Envelope envBounds)
        {
            result = new List<Polygon>();
            double[] vertices = points.Vertex;
            VoronoiGraph gp = Fortune.ComputeVoronoiGraph(vertices);

            Extent ext = points.Extent;
            ext.ExpandBy(ext.Width / 100, ext.Height / 100);
            IEnvelope env = ext.ToEnvelope();
            var bounds = envBounds.ToPolygon();

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
                        if (myEdges[j].VVertexA.Equals(previous))
                        {
                            previous = myEdges[j].VVertexB;
                            coords.Add(previous.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        if (myEdges[j].VVertexB.Equals(start))
                        {
                            start = myEdges[j].VVertexA;
                            coords.Insert(0, start.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        if (myEdges[j].VVertexB.Equals(previous))
                        {
                            previous = myEdges[j].VVertexA;
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

                    if (double.IsNaN(coords[j].X) || double.IsNaN(coords[j].Y))
                    {
                        coords.Remove(coords[j]);
                    }

                    for (int k = j + 1; k < coords.Count; k++)
                    {
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

                var lr = new LinearRing(coords.ToArray());
                Polygon pg = new Polygon(lr);
                try
                {
                    IGeometry g = pg.Intersection(bounds);
                    Polygon p = g as Polygon;
                    if (p != null)
                    {
                        result.Add(p);
                    }
                }
                catch (Exception)
                {

                }
            }
            return;
        }
        public static void VoronoiPointsNew(double[] vertices, ref List<Polygon> result)
        {
            result = new List<Polygon>();
            VoronoiGraph gp = Fortune.ComputeVoronoiGraph(vertices);
            double XMin = vertices[0], YMin = vertices[1], XMax = vertices[0], YMax = vertices[1];
            for (int i = 0; i < vertices.Length / 2; i++)
            {
                if (vertices[i] < XMin) XMin = vertices[i];
                if (vertices[i + 1] < YMin) YMin = vertices[i + 1];
                if (vertices[i] > XMax) XMax = vertices[i];
                if (vertices[i + 1] > YMax) YMax = vertices[i + 1];

            }
            Extent ext = new Extent(XMin, YMin, XMax, YMax);
            ext.ExpandBy(ext.Width / 100, ext.Height / 100);
            IEnvelope env = ext.ToEnvelope();

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
                        if (myEdges[j].VVertexA.Equals(previous))
                        {
                            previous = myEdges[j].VVertexB;
                            coords.Add(previous.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        if (myEdges[j].VVertexB.Equals(start))
                        {
                            start = myEdges[j].VVertexA;
                            coords.Insert(0, start.ToCoordinate());
                            myEdges.Remove(myEdges[j]);
                            break;
                        }

                        if (myEdges[j].VVertexB.Equals(previous))
                        {
                            previous = myEdges[j].VVertexA;
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

                    if (double.IsNaN(coords[j].X) || double.IsNaN(coords[j].Y))
                    {
                        coords.Remove(coords[j]);
                    }

                    for (int k = j + 1; k < coords.Count; k++)
                    {
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

                var lr = new LinearRing(coords.ToArray());
                result.Add(new Polygon(lr));

            }

        }
        public static void VoronoiPoints(double[] vertices, ref List<double> result)
        {
            VoronoiGraph gp = Fortune.ComputeVoronoiGraph(vertices);

            foreach (VoronoiEdge edge in gp.Edges)
            {
                if (vertices[0] == edge.RightData.X && vertices[1] == edge.RightData.Y)
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

        public static void AddToMonitorDataList(Dictionary<int, int> dicPOCOrder, BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, MonitorDataLine monitorDataLine, MonitorValue mv, List<MonitorValue>lstMonitorValues)
        {
            // Add one monitor site values into lstMonitorValues
            // If error happens during processing this monitor e.g. not enough values to calculate seasonal metric, 
            //   values of this monitor are not appended and therefore will not be used in interpolation.
            int iPOC = -1;
            try
            {
                if (!string.IsNullOrEmpty(mv.MonitorMethod))
                {
                    if (mv.MonitorMethod.Contains("POC=.") || mv.MonitorMethod.Contains("POC=\'") || !mv.MonitorMethod.Contains("POC=") )
                    {
                        iPOC = 1;
                    }
                    else
                    {
                        iPOC = Convert.ToInt16(mv.MonitorMethod.Substring(mv.MonitorMethod.IndexOf("POC=") + 4, mv.MonitorMethod.IndexOf('\'', mv.MonitorMethod.IndexOf("POC=") + 4) - mv.MonitorMethod.IndexOf("POC=") - 4));
                    }
                }
            }
            catch
            {
                //Add some handling here
            }

            if (CommonClass.MainSetup.SetupID == 1 && benMAPPollutant.Seasons != null && (benMAPPollutant.PollutantName.ToLower() == "pm2.5" || benMAPPollutant.PollutantName.ToLower() == "pm10") && mv.Values.Count == 365)
            {
                foreach (Season s in benMAPPollutant.Seasons)
                {
                    int iPerQuarter = 11;
                    //YY: users can only customize iPerQuater when pollutant is exactly "PM2.5". If not, the advanced button is grey. 
                    if (monitorDataLine.MonitorAdvance != null) iPerQuarter = monitorDataLine.MonitorAdvance.NumberOfPerQuarter;
                    if (mv.Values.GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Count(p => p != float.MinValue) < iPerQuarter)
                    {
                        // This row is invalid. Bail out.
                        // TODO: Add some error notification
                        return;
                    }
                }
            }

            // United States with no advanced settings
            if (CommonClass.MainSetup.SetupID == 1 && monitorDataLine.MonitorAdvance == null)
            {
                switch (benMAPPollutant.PollutantName.ToLower())
                {
                    case "pm2.5":
                        if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                        {
                            if (string.IsNullOrEmpty(mv.MonitorMethod) || (iPOC == 1 || iPOC == 2 || iPOC == 3 || iPOC == 4))
                            {

                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case "ozone":
                        if (mv.Values.Count > 8700) //If Monitor data is hourly 
                        {
                            List<float> lstFloatTemp = new List<float>();
                            for (int iMV = 0; iMV < mv.Values.Count / 24; iMV++)
                            {
                                try
                                {
                                    //YY: For 8am to 8pm, if there are less than 11 hours of data available, the AQ of this day is considered missing. 
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
                            //If day 120 to day 153 has more than half of the days (>16 days) with values = float.MinValue, don't add this monitor value record to lstMonitorValues
                            if (lstFloatTemp.GetRange(120, 272 - 120 + 1).Where(p => p != float.MinValue).Count() < lstFloatTemp.GetRange(120, 272 - 120 + 1).Count / 2)
                            {
                                return;
                            }

                        }
                        else
                        {
                            //If day 120 to day 153 has more than half of the days (>16 days) with values = float.MinValue, don't add this monitor value record to lstMonitorValues
                            if (mv.Values.GetRange(120, 272 - 120 + 1).Where(p => p != float.MinValue).Count() < mv.Values.GetRange(120, 272 - 120 + 1).Count / 2)
                            {
                                return;
                            }

                        }
                        if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                        {
                            if (string.IsNullOrEmpty(mv.MonitorMethod) || (iPOC == 1 || iPOC == 2 || iPOC == 3 || iPOC == 4))
                            {

                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case "pm10":
                        if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                        {
                            if ((iPOC == 1 || iPOC == 2 || iPOC == 3 || iPOC == 4))
                            {

                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case "so2":
                        if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                        {
                            if (string.IsNullOrEmpty(mv.MonitorMethod) || (iPOC == 1 || iPOC == 2 || iPOC == 3 || iPOC == 4 || iPOC == 5 || iPOC == 6 || iPOC == 7 || iPOC == 8 || iPOC == 9))
                            {
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case "no2":
                        if ((Convert.ToDouble(mv.Latitude) >= 20.0) && (Convert.ToDouble(mv.Latitude) <= 55.0) && (Convert.ToDouble(mv.Longitude) <= -65.0) && (mv.Longitude) >= -130.0)
                        {
                            if (string.IsNullOrEmpty(mv.MonitorMethod) || (iPOC == 1 || iPOC == 2 || iPOC == 3 || iPOC == 4 || iPOC == 5 || iPOC == 6 || iPOC == 7 || iPOC == 8 || iPOC == 9))
                            {

                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                }
            }
            // United States WITH advanced settings
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
                    if (isInclude) return;
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
                    if (isexclude) return;
                }

                if (!((Convert.ToDouble(mv.Latitude) >= monitorDataLine.MonitorAdvance.FilterMinLatitude) && (Convert.ToDouble(mv.Latitude) <= monitorDataLine.MonitorAdvance.FilterMaxLatitude) && (Convert.ToDouble(mv.Longitude) <= monitorDataLine.MonitorAdvance.FilterMaxLongitude) && (mv.Longitude) >= monitorDataLine.MonitorAdvance.FilterMinLongitude))
                {
                    return;
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
                    if (bValidMethod == false) return;
                }
                else if (monitorDataLine.MonitorAdvance.IncludeMethods != null && monitorDataLine.MonitorAdvance.IncludeMethods.Count() == 0 && !string.IsNullOrEmpty(mv.MonitorMethod))
                {
                    return;
                }
                if (iPOC > monitorDataLine.MonitorAdvance.FilterMaximumPOC && monitorDataLine.MonitorAdvance.FilterMaximumPOC != -1)
                {
                    return;
                }

                if (benMAPPollutant.PollutantName.ToLower() == "ozone")
                {
                    if (mv.Values.Count > 8700)
                    {
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
                        if (lstFloatTemp.GetRange(monitorDataLine.MonitorAdvance.StartDate, monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1).Where(p => p != float.MinValue).Count() < (monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1) * monitorDataLine.MonitorAdvance.PercentOfValidDays / 100)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (mv.Values.GetRange(monitorDataLine.MonitorAdvance.StartDate, monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1).Where(p => p != float.MinValue).Count() < (monitorDataLine.MonitorAdvance.EndDate - monitorDataLine.MonitorAdvance.StartDate + 1) * monitorDataLine.MonitorAdvance.PercentOfValidDays / 100)
                        {
                            return;
                        }

                    }

                }
            }

            // We're past all the filtering. Let's see if it can be added.

            if (lstMonitorValues.Where(p => p.MonitorName == mv.MonitorName).Count() == 0 && lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).Count() == 0)
            {
                while (mv.MonitorName.Trim().Length < 15)
                {
                    mv.MonitorName = "0" + mv.MonitorName;

                }
                lstMonitorValues.Add(mv);
            }
            // If this monitor name or latitude and longtitude already exist...
            else if (lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).Count() > 0)
            {
                if (string.IsNullOrEmpty(mv.MonitorMethod) || string.IsNullOrEmpty(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0].MonitorMethod))
                {
                    return;
                }
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
                        if (isInclude)
                        {
                            return;
                        }
                        else
                        {
                            lstMonitorValues.Remove(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0]);
                            lstMonitorValues.Add(mv);
                        }
                    }
                    else
                        return;
                }
                else if (!dicPOCOrder.ContainsKey(iPOC) && dicPOCOrder.ContainsKey(iPOCold))
                {
                    return;
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
                    if (isInclude) return;
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
                        if (isInclude) return;
                        else
                        {
                            lstMonitorValues.Remove(lstMonitorValues.Where(p => p.Longitude == mv.Longitude && p.Latitude == mv.Latitude).ToArray()[0]);
                            lstMonitorValues.Add(mv);
                        }
                    }
                    else
                    {
                        return;
                    }
                }

            }

    }

    public static List<MonitorValue> GetMonitorData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, MonitorDataLine monitorDataLine)
        {
            try
            {
                int i = 0;
                List<MonitorValue> lstMonitorValues = new List<MonitorValue>();
                Dictionary<int, int> dicPOCOrder = new Dictionary<int, int>();
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
                if (monitorDataLine.MonitorDirectType == 0)
                {
                    commandText = string.Format("select a.MonitorEntryID,a.MonitorID,a.YYear,a.MetricID,a.SeasonalMetricID,a.Statistic,a.VValues,b.PollutantID,b.Latitude,b.Longitude,b.MonitorName,b.MonitorDescription from MonitorEntries a,Monitors b,MonitorDataSets c where a.MonitorID=b.MonitorID and b.MonitorDataSetID=c.MonitorDataSetID and b.PollutantID={0} and c.MonitorDataSetID={1} and a.YYear={2}", benMAPPollutant.PollutantID, monitorDataLine.MonitorDataSetID, monitorDataLine.MonitorLibraryYear); ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    Byte[] blob = null;
                    lstMonitorValues = new List<MonitorValue>();

                    string str = "";
                    string[] strArray = null;
                    monitorDataLine.MonitorNeighbors = new List<MonitorNeighborAttribute>();
                    while (fbDataReader.Read())
                    {
                        mv = new MonitorValue();

                        blob = fbDataReader[6] as byte[];
                        str = System.Text.Encoding.Default.GetString(blob);
                        strArray = str.Split(new char[] { ',' });
                        mv.MonitorID = Convert.ToInt32(fbDataReader["MonitorID"]);
                        //Adding ToString below because converting straight to double adds false precision
                        //mv.Latitude = Convert.ToDouble(fbDataReader["Latitude"].ToString());
                        //mv.Longitude = Convert.ToDouble(fbDataReader["Longitude"].ToString());
                        mv.Latitude = Convert.ToDouble(fbDataReader["Latitude"]);
                        mv.Longitude = Convert.ToDouble(fbDataReader["Longitude"]);
                        mv.MonitorName = fbDataReader["MonitorName"].ToString();
                        mv.MonitorMethod = fbDataReader["MonitorDescription"].ToString();


                        if (!(fbDataReader["MetricID"] is DBNull))
                        {
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

                        AddToMonitorDataList(dicPOCOrder, benMAPGrid, benMAPPollutant, monitorDataLine, mv, lstMonitorValues);

                    }
                    fbDataReader.Close();

                }
                if (monitorDataLine.MonitorDirectType == 1)
                {
                    int iMonitorName = -1;
                    int iMonitorDescription = -1;
                    int iLatitude = -1;
                    int iLongitude = -1;
                    int iMetric = -1;
                    int iSeasonalMetric = -1;
                    int iStatistic = -1;
                    int iValues = -1;
                    i = 0;
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
                    //Pull data from monitor csv (now in dt) into MonitorValue mv
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
                        AddToMonitorDataList(dicPOCOrder, benMAPGrid, benMAPPollutant, monitorDataLine, mv, lstMonitorValues);
                        //lstMonitorValues.Add(mv);
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
                UpdateMonitorDicMetricValue(benMAPPollutant, lstMonitorValues);
                monitorDataLine.MonitorValues = lstMonitorValues;
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
                    fs = FeatureSet.Open(strSHP);
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
                        dClose = monitorDataLine.MonitorAdvance.MaxinumNeighborDistance;
                    else if (monitorDataLine.MonitorAdvance != null && monitorDataLine.MonitorAdvance.RelativeNeighborDistance != -1)
                        dClose = monitorDataLine.MonitorAdvance.RelativeNeighborDistance;
                    IFeatureSet fsPoints = new FeatureSet();
                    List<Coordinate> lstCoordinate = new List<Coordinate>();

                    List<double> fsInter = new List<double>();
                    Dictionary<string, MonitorValue> dicMonitorValues = new Dictionary<string, MonitorValue>(); ;
                    //YY: for monitor sites at the same location (lat/long) they are considered as one monitor with average AQ value.
                    foreach (MonitorValue monitorValue in lstMonitorValues)
                    {
                        if (monitorValue.dicMetricValues == null || monitorValue.dicMetricValues.Count == 0) continue;
                        if (!dicMonitorValues.ContainsKey(monitorValue.Longitude + "," + monitorValue.Latitude))
                        {
                            dicMonitorValues.Add(monitorValue.Longitude + "," + monitorValue.Latitude, monitorValue);
                            fsPoints.AddFeature(new Point(monitorValue.Longitude, monitorValue.Latitude));
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

                    }
                    Coordinate cCenter = fsPoints.Extent.Center;
                    Dictionary<MonitorValue, double> dicCoordinateDistanceCenter = new Dictionary<MonitorValue, double>();
                    //YY: Find list of top 300 (or less if number of monitor sites < 300) monitor sites which are closest to the centroid of all monitor sites. This doesn't seem used anywhere. 
                    foreach (MonitorValue monitorValue in lstMonitorValues)
                    {
                        dicCoordinateDistanceCenter.Add(monitorValue, getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, cCenter.X, cCenter.Y));
                    }
                    List<MonitorValue> lstCenter = dicCoordinateDistanceCenter.OrderByDescending(p => p.Value).Select(p => p.Key).ToList().GetRange(0, lstMonitorValues.Count > 300 ? 300 : lstMonitorValues.Count);
                    int idicMonitorValues = dicMonitorValues.Count;
                    if (idicMonitorValues > 100) idicMonitorValues = 100;
                    Polygon fOnlyPoint = null; IFeatureSet fsVoronoi = new FeatureSet();
                    var coordinate = new Coordinate();
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
                        if (fs.GetFeature(i).Geometry.GeometryType == "Polygon")
                        {
                            coordinate = new Coordinate((fs.GetFeature(i).Geometry as Polygon).Centroid.X, (fs.GetFeature(i).Geometry as Polygon).Centroid.Y);
                        }
                        else
                        {
                            coordinate = new Coordinate((fs.GetFeature(i).Geometry as MultiPolygon).Centroid.X, (fs.GetFeature(i).Geometry as MultiPolygon).Centroid.Y);
                        }
                        switch (monitorDataLine.InterpolationMethod)
                        {
                            case InterpolationMethodEnum.ClosestMonitor:
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();
                                foreach (MonitorValue monitorValue in lstMonitorValues)
                                {
                                    DicMonitorDistance.Add(monitorValue, Convert.ToSingle((coordinate.X - monitorValue.Longitude) * (coordinate.X - monitorValue.Longitude) + (coordinate.Y - monitorValue.Latitude) * (coordinate.Y - monitorValue.Latitude)));
                                }
                                dmin = DicMonitorDistance.Min(p => p.Value);
                                DicMonitorDistanceKeyValue = DicMonitorDistance.Where(a => a.Value == dmin).First();
                                if (dClose != -1 && getDistanceFrom2Point(coordinate.X, coordinate.Y, DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude) > dClose) { }
                                else
                                {
                                    monitorDataLine.ModelResultAttributes.Add(new ModelResultAttribute()
                                    {
                                        Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
                                        Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
                                        Values = DicMonitorDistanceKeyValue.Key.dicMetricValues
                                    });
                                    monitorDataLine.MonitorNeighbors.Add(new MonitorNeighborAttribute()
{
    Col = Convert.ToInt32(fs.DataTable.Rows[i][iCol]),
    Row = Convert.ToInt32(fs.DataTable.Rows[i][iRow]),
    Distance = getDistanceFrom2Point(new Point(coordinate), new Point(DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude)),
    MonitorName = DicMonitorDistanceKeyValue.Key.MonitorName,
    Weight = 1
});
                                }
                                break;
                            case InterpolationMethodEnum.FixedRadius:
                                var tPoint = new NetTopologySuite.Geometries.Point(coordinate);
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();

                                foreach (MonitorValue monitorValue in lstMonitorValues)
                                {
                                    DicMonitorDistance.Add(monitorValue, getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, coordinate.X, coordinate.Y));
                                }
                                fixdic = monitorDataLine.FixedRadius;
                                DicMonitorDistanceTemp = DicMonitorDistance.Where(p => (p.Value) <= fixdic).ToList(); if (DicMonitorDistanceTemp.Count == 0)
                                {

                                }
                                else
                                {
                                    DicMonitorDistanceKeyValue = DicMonitorDistanceTemp.First();
                                    Dictionary<string, ModelAttribute> dicModelAttribute = new Dictionary<string, ModelAttribute>();
                                    if (monitorDataLine.MonitorAdvance == null || monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistance)
                                    {
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
                                            List<float> lstdfm = new List<float>();
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
                                                lstdfm[idfm] = lstdfz[idfm] == 0 ? float.MinValue : lstdfm[idfm] / (lstdfz[idfm]);
                                            }
                                            dicModelAttribute[dicsd.Key].Values = lstdfm;
                                            monitorDataLine.ModelResultAttributes.Last().Values.Add(dicsd.Key, Convert.ToSingle(Math.Round(dfm / dfz, 2)));

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
                                    }
                                    else if (monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistanceSquared)
                                    {
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
                                            for (int idfm = 0; idfm < lstdfm.Count; idfm++)
                                            {
                                                lstdfm[idfm] = lstdfz[idfm] == 0 ? float.MinValue : lstdfm[idfm] / lstdfz[idfm];
                                            }
                                            dicModelAttribute[dicsd.Key].Values = lstdfm;
                                            monitorDataLine.ModelResultAttributes.Last().Values.Add(dicsd.Key, Convert.ToSingle(Math.Round(dfm / dfz, 2)));

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
                                    }
                                }
                                break;
                            case InterpolationMethodEnum.VoronoiNeighborhoodAveragin:
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();
                                idicMonitorValues = 500;
                                foreach (MonitorValue monitorValue in lstMonitorValues)
                                {
                                    DicMonitorDistance.Add(monitorValue, Convert.ToSingle((coordinate.X - monitorValue.Longitude) * (coordinate.X - monitorValue.Longitude) + (coordinate.Y - monitorValue.Latitude) * (coordinate.Y - monitorValue.Latitude)));
                                }

                                var query = DicMonitorDistance.Where(p => p.Value < 25).ToList(); int iDistanceForQuery = 1;
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
                                        float d = getDistanceFrom2Point(coordinate.X, coordinate.Y, k.Key.Longitude, k.Key.Latitude);
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

                                DicMonitorDistance = new Dictionary<MonitorValue, float>();
                                dicNeighbors.Add(fs.DataTable.Rows[i]["COL"].ToString() + "," + fs.DataTable.Rows[i]["ROW"].ToString(), new Dictionary<string, string>());
                                for (int ifsout = 0; ifsout < fsout.Count; ifsout++)
                                {
                                    if (dicMonitorValues.ContainsKey(fsout[ifsout] + "," + fsout[ifsout + 1]))
                                    {
                                        mvTemp = dicMonitorValues[fsout[ifsout] + "," + fsout[ifsout + 1]];
                                        if (!DicMonitorDistance.ContainsKey(mvTemp))
                                        {
                                            float dDistance = getDistanceFrom2Point(mvTemp.Longitude, mvTemp.Latitude,
                                            coordinate.X, coordinate.Y);
                                            DicMonitorDistance.Add(mvTemp, dDistance);
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
                                }
                                else if (monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistanceSquared)
                                {
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
                                    }
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
                                }

                                break;
                        }
                        i++;
                    }
                    fs.Close();
                    fs.Dispose();
                }



            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
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
                benMAPLineCopy.ShapeFile = benMAPLine.ShapeFile;
                benMAPLineCopy.DatabaseFilePath = (benMAPLine as ModelDataLine).DatabaseFilePath;
                benMAPLineCopy.DatabaseType = (benMAPLine as ModelDataLine).DatabaseType;

                return benMAPLineCopy;

            }
            else if (benMAPLine is MonitorDataLine)
            {
                MonitorDataLine benMAPLineCopy = new MonitorDataLine();

                benMAPLineCopy.GridType = benMAPLine.GridType;
                benMAPLineCopy.Pollutant = benMAPLine.Pollutant;
                benMAPLineCopy.ShapeFile = benMAPLine.ShapeFile;
                benMAPLineCopy.MonitorDirectType = (benMAPLine as MonitorDataLine).MonitorDirectType; benMAPLineCopy.MonitorDataSetID = (benMAPLine as MonitorDataLine).MonitorDataSetID;
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
                benMAPLineCopy.ShapeFile = benMAPLine.ShapeFile;

                benMAPLineCopy.RollbackGrid = (benMAPLine as MonitorModelRollbackLine).RollbackGrid;
                benMAPLineCopy.BenMAPRollbacks = (benMAPLine as MonitorModelRollbackLine).BenMAPRollbacks;
                benMAPLineCopy.ScalingMethod = (benMAPLine as MonitorModelRollbackLine).ScalingMethod; benMAPLineCopy.AdditionalGrid = (benMAPLine as MonitorModelRollbackLine).AdditionalGrid;
                benMAPLineCopy.AdustmentFilePath = (benMAPLine as MonitorModelRollbackLine).AdustmentFilePath;
                benMAPLineCopy.isMakeBaseLineGrid = (benMAPLine as MonitorModelRollbackLine).isMakeBaseLineGrid;
                return benMAPLineCopy;
            }
            return null;
        }

        public static void CreateAQGFromBenMAPLine(BenMAPLine benMAPLine, string strAQGPath)
        {
            try
            {
                if (File.Exists(strAQGPath))
                    File.Delete(strAQGPath);
                if ((benMAPLine.ModelAttributes != null && benMAPLine is MonitorDataLine))
                {
                    benMAPLine.ModelAttributes.Clear();
                    GC.Collect();

                }
                using (FileStream fs = new FileStream(strAQGPath, FileMode.OpenOrCreate))
                {
                    if (benMAPLine.GridType == null && CommonClass.GBenMAPGrid != null) benMAPLine.GridType = CommonClass.GBenMAPGrid;
                    benMAPLine.CreateTime = DateTime.Now;
                    benMAPLine.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                    Serializer.Serialize<BenMAPLine>(fs, benMAPLine);

                    fs.Close();
                    fs.Dispose();
                }
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


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public static BenMAPLine LoadAQGFile(string strAQGPath, ref string err)
        {

            using (FileStream fs = new FileStream(strAQGPath, FileMode.Open))
            {
                try
                {

                    BenMAPLine benMAPLine = Serializer.Deserialize<BenMAPLine>(fs);
                    //YY: test serializer to xml
                    //var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(benMAPLine);
                    //Console.WriteLine(json);

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
                    BenMAPSetup benMAPSetup = null;
                    if (benMAPLine.GridType != null)
                    {
                        benMAPSetup = CommonClass.getBenMAPSetupFromName(benMAPLine.GridType.SetupName);
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

                    fs.Close();
                    fs.Dispose();
                    return benMAPLine;
                }
                catch (Exception ex)
                {
                    fs.Close();
                    fs.Dispose();
                    err = "BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.";
                    return null;
                }
            }




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

        }

        public static void UpdateModelValuesModelData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, ModelDataLine modelDataLine)
        {
            throw new NotImplementedException();
        }
    }

    public static class EnumerableExtensions
    {
        public static double Median(this IEnumerable<double> list)
        {
            //Note: this function does not sort list/array. The input list is usually already sorted.
            double[] lstarray = list.ToArray();
            int midPoint;
            double median, sum;
            int items = list.Count() + 1;
            sum = 0.0;

            if (((int)Math.Round((double)items / 2.0) * 2) != items)
            {
                midPoint = items / 2;

                sum = lstarray[midPoint - 1];
                sum += lstarray[midPoint];
                sum /= 2.0;
            }
            else
            {
                midPoint = (items / 2);
                sum = lstarray[midPoint];
            }

            median = sum;
            return median;

        }

        public static float Median(this IEnumerable<float> list)
        {
            //Note: this function does not sort list/array. The input list is usually already sorted.
            float[] lstarray = list.ToArray();
            int midPoint;
            float median, sum;
            int items = list.Count() + 1;
            sum = 0;

            if (items % 2 == 1)
            {
                midPoint = items / 2;

                sum = lstarray[midPoint - 1];
                sum += lstarray[midPoint];
                sum /= 2;
            }
            else
            {
                midPoint = (items / 2);
                sum = lstarray[midPoint - 1];
            }

            median = sum;
            return median;
        }
    }
}