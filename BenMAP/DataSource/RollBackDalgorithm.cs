using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Topology.Voronoi;
using FirebirdSql.Data.FirebirdClient;

namespace BenMAP.DataSource
{
    public class RollBackDalgorithm
    {
        public RollBackDalgorithm()
        {
            //Todo: 构造函数
        }

        /// <summary>
        /// MonitorRollBack 设置完成对应的参数后计算对应的削减，并将计算结果保存在monitorDataLine对应中返回
        /// </summary>
        /// <param name="benMAPGrid">网格属性对象</param>
        /// <param name="benMAPPollutant">污染物属性对象</param>
        /// <param name="monitorDataLine">BaseControlGroup</param>
        /// <returns></returns>
        public static bool UpdateMonitorDataRollBack(ref MonitorModelRollbackLine monitorModelRollbackLine)
        {
            bool ok = false;
            try
            {
                //BaseControlGroup
                // 选中的需要削减的监测点所在网格的行列号
                List<BenMAPRollback> lstBenMapRollback = monitorModelRollbackLine.BenMAPRollbacks;
                if (lstBenMapRollback == null || lstBenMapRollback.Count < 1) { return false; }
                MonitorDataLine mdl = new MonitorDataLine()
                {
                    GridType = monitorModelRollbackLine.GridType,
                    Pollutant = monitorModelRollbackLine.Pollutant,
                    //ResultCopy=monitorModelRollbackLine.ModelResultAttributes.
                    MonitorAdvance = monitorModelRollbackLine.MonitorAdvance,
                    MonitorLibraryYear = monitorModelRollbackLine.MonitorLibraryYear,
                    MonitorDataSetID = monitorModelRollbackLine.MonitorDataSetID,
                    MonitorDataFilePath = monitorModelRollbackLine.MonitorDataFilePath,
                    MonitorDefinitionFile = monitorModelRollbackLine.MonitorDefinitionFile,
                    InterpolationMethod = monitorModelRollbackLine.InterpolationMethod,
                    MonitorDirectType = monitorModelRollbackLine.MonitorDirectType,
                    ModelAttributes = monitorModelRollbackLine.ModelAttributes,
                    ModelResultAttributes = monitorModelRollbackLine.ModelResultAttributes,
                    FixedRadius = monitorModelRollbackLine.FixedRadius
                };
                List<MonitorValue> lstMonitorValues = DataSourceCommonClass.GetMonitorData(mdl.GridType,mdl.Pollutant,  mdl);
                if (lstMonitorValues == null || lstMonitorValues.Count < 1)
                { return false; }
                //将监测点对应的经纬度坐标转换为网格的行列索引
                ok = GetMonitorPointsColRow(monitorModelRollbackLine.RollbackGrid, ref lstMonitorValues);
                if (!ok) { return false; }
                //更新DicMetricValue
                //DataSourceCommonClass.UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, lstMonitorValues);
                //UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, ref lstMonitorValues);

                RollbackType rbt;
                foreach (BenMAPRollback b in lstBenMapRollback)
                {
                    rbt = b.RollbackType;
                    switch (rbt)
                    {
                        case RollbackType.percentage:
                            PercentageRollback pr = b as PercentageRollback;
                            ok = GetPercentageRollbackValues(pr, ref lstMonitorValues);
                            if (!ok)
                            { //错误处理
                            }
                            DataSourceCommonClass.UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, lstMonitorValues);
                            break;
                        case RollbackType.incremental:
                            IncrementalRollback ir = b as IncrementalRollback;
                            ok = GetIncrementalRollBackValues(ir, ref lstMonitorValues);
                            if (!ok)
                            { //错误处理
                            }
                            DataSourceCommonClass.UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, lstMonitorValues);
                            break;
                        case RollbackType.standard:
                            StandardRollback sr = b as StandardRollback;
                            if (mdl.Pollutant.Observationtype == ObservationtypeEnum.Hourly)
                            {
                                ok = GetRollBack2StandardHourly(sr, ref lstMonitorValues);//rollback the observation value
                                if (!ok)
                                { }
                            }
                            if (mdl.Pollutant.Observationtype == ObservationtypeEnum.Daily)
                            {
                                ok = GetRollBack2StandardDaily(sr, ref lstMonitorValues);//rollback the 365 value
                                if (!ok)
                                { }
                            }

                            DataSourceCommonClass.UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, lstMonitorValues);//calculate the metric value

                            break;
                    }// Swith
                }//Foreach
                
                UpdateModelValuesMonitorData(lstMonitorValues, ref mdl);
                //add by xiejp 20131113
                monitorModelRollbackLine.MonitorValues = lstMonitorValues;
                monitorModelRollbackLine.ModelResultAttributes = mdl.ModelResultAttributes;
                monitorModelRollbackLine.ModelAttributes = mdl.ModelAttributes;
                monitorModelRollbackLine.BenMAPRollbacks = lstBenMapRollback;
                monitorModelRollbackLine.MonitorNeighbors = mdl.MonitorNeighbors;
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取选中网格在Percentage Roll back的计算值
        /// Percentage Rollback involves setting only two parameters - a percentage and a background level.
        ///The rollback procedure is similarly straightforward - each observation at each monitor in the region has the portion of its value which is above background level reduced by percentage.
        ///Example: Background Level: 35; Percentage: 25
        ///Initial Observations at a monitor in rollback region:
        ///20 20 25 59 35 51 83 35 30 67 87 79 63 35 35
        ///If we select the background level of 35, we first calculate the portion of each observation that is above background level, that is, we subtract the background level from the initial observation level.  Observations below background level are given a value of 0.
        ///Observation portions above background level:
        ///0 0 0 24 0 16 48 0 0 32 52 44 28 0 0
        ///When we apply the rollback percentage, each observation portion gets reduced by 25%.
        ///Reduced portions above background level:
        ///0 0 0 18 0 12 36 0 0 24 39 33 21 0 0
        ///Then, each reduced portion is added to the background level of 35.  Zero values are replaced by the initial observations.
        ///Reduced Observations:
        ///20 20 25 53 35 47 71 35 30 59 74 68 56 35 35
        /// </summary>
        /// <param name="monitorDataLine"></param>
        /// <returns></returns>
        public static bool GetPercentageRollbackValues(PercentageRollback pRollback, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                //monitorModelRollbackLine.BenMAPRollbacks[0].
                double background = pRollback.Background;
                double percentage = pRollback.Percent / 100;
                List<RowCol> lstSelectGrids = pRollback.SelectRegions;
                List<string> lstSelectGridsString = lstSelectGrids.Select(p => p.Col + "," + p.Row).ToList();
                //RowCol rc;
                //int count = 0;
                string[] keyIndex;
                foreach (MonitorValue b in lstMonitorValues)
                {
                    //rc = new RowCol() { Col = b.Col, Row = b.Row };
                    //foreach (RowCol rc in lstSelectGrids)
                    if (lstSelectGridsString.Contains(b.Col + "," + b.Row))
                    {
                        //if (rc.Row == b.row && rc.Col == b.col)
                        //{
                        //keyIndex = new string[b.dicMetricValues.Count];
                        //b.dicMetricValues.Keys.CopyTo(keyIndex, 0);
                        //for (int i = 0; i < b.dicMetricValues.Count; i++)
                        //{
                        //    double var = b.dicMetricValues[keyIndex[i]];
                        //    if (var > background)
                        //    {
                        //        var = (var - background) * (1 - percentage) + background;
                        //        if (var < background) { var = background; }
                        //        b.dicMetricValues[keyIndex[i]] = Convert.ToSingle(var);
                        //    }
                        //}
                        for (int i = 0; i < b.Values.Count; i++)
                        {
                            double var = b.Values[i];
                            if (var > background)
                            {
                                var = (var - background) * (1 - percentage) + background;
                                if (var < background) { var = background; }
                                b.Values[i] = Convert.ToSingle(var);
                            }
                        }
                        //    break;
                        //}//if_lstSelectGrids
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }// F_GetPercentageRollbackValues

        /// <summary>
        ///Incremental Rollback similarly involves setting only two parameters - an increment and a background level.
        ///The rollback procedure is quite similar to the percentage rollback procedure - each observation at each monitor in the region has the portion of its value which is above background level reduced by increment.
        ///The reduced values are not allowed to become negative, however - that is, they are truncated at zero.
        ///Example: Background Level: 35; Increment: 25
        ///Initial Observations:
        ///20 20 25 59 35 51 83 35 30 67 87 79 63 35 35
        ///Observation portions above background level:
        ///0 0 0 24 0 16 48 0 0 32 52 44 28 0 0
        ///Reduced portions above background level:
        ///0 0 0 0 0 0 23 0 0 7 27 19 3 0 0
        ///Reduced Observations:
        ///20 20 25 35 35 35 58 35 30 42 62 54 38 35 35
        /// </summary>
        /// <param name="iRollback"></param>
        /// <param name="monitorModelRollbackLine"></param>
        /// <returns></returns>
        public static bool GetIncrementalRollBackValues(IncrementalRollback iRollback, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                //monitorModelRollbackLine.BenMAPRollbacks[0].
                double background = iRollback.Background;
                double increment = iRollback.Increment;
                double tmp = 0.0;
                List<RowCol> lstSelectGrids = iRollback.SelectRegions;
                List<string> lstSelectGridsString = lstSelectGrids.Select(p => p.Col + "," + p.Row).ToList();
                string[] keyIndex;
                //RowCol rc;
                foreach (MonitorValue b in lstMonitorValues)
                {
                    //rc = new RowCol() { Col = b.Col, Row = b.Row };
                    //foreach (RowCol rc in lstSelectGrids)
                    //{
                    //if (rc.Row == b.row && rc.Col == b.col)
                    //{
                    if (lstSelectGridsString.Contains(b.Col + "," + b.Row))
                    {

                        //keyIndex = new string[b.dicMetricValues.Count];
                        //b.dicMetricValues.Keys.CopyTo(keyIndex, 0);
                        //for (int i = 0; i < b.dicMetricValues.Count; i++)
                        //{
                        //    double var = b.dicMetricValues[keyIndex[i]];
                        //    if (var > background)
                        //    {
                        //        tmp = var - (background + increment);
                        //        if (tmp > 0)
                        //        { var = tmp + background; }
                        //        else { var = background; }
                        //        b.dicMetricValues[keyIndex[i]] = Convert.ToSingle(var);
                        //    }
                        //}
                        for (int i = 0; i < b.Values.Count; i++)
                        {
                            double var = b.Values[i];
                            if (var > background)
                            {
                                tmp = var - (background + increment);
                                if (tmp > 0)
                                { var = tmp + background; }
                                else { var = background; }
                                b.Values[i] = Convert.ToSingle(var);
                            }
                        }

                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }//F_GetIncrementalRollBackValues

        public static bool GetRollBack2StandardHourly(StandardRollback sRollback, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                int ordinality = sRollback.Ordinality;
                float standard = Convert.ToSingle(sRollback.Standard);
                string method = sRollback.IntradayRollbackMethod;
                float Background = Convert.ToSingle(sRollback.IntradayBackground);

                List<RowCol> lstSelectGrids = sRollback.SelectRegions;
                List<string> lstSelectGridsString = lstSelectGrids.Select(p => p.Col + "," + p.Row).ToList();
                foreach (MonitorValue b in lstMonitorValues)
                {
                    if (lstSelectGridsString.Contains(b.Col + "," + b.Row))
                    {
                        List<float> InitialValues = b.Values;
                        float OutofAttainmentValue = InitialValues.Distinct().OrderByDescending(q => q).ToList()[ordinality - 1];
                        Metric metric = sRollback.DailyMetric;
                        OutofAttainmentValue = calculateMetricValue(metric, InitialValues, ordinality);
                        while (OutofAttainmentValue > standard)
                        {
                            switch (method)
                            {
                                case "Percentage":
                                    float AnthropogenicOutofAttainmentValue = OutofAttainmentValue - Background;
                                    float AnthropogenicStandard = standard - Background;
                                    float PercentageReduction = (AnthropogenicOutofAttainmentValue - AnthropogenicStandard) / AnthropogenicOutofAttainmentValue;
                                    for (int i = 0; i < InitialValues.Count; i++)
                                    {
                                        float m = InitialValues[i];
                                        if (m > 0)
                                            InitialValues[i] = m < Background ? m : Background + (m < Background ? 0 : (m - Background)) * PercentageReduction;
                                    }
                                    break;
                                case "Incremental":
                                    float IncrementalReduction = OutofAttainmentValue - standard;
                                    for (int i = 0; i < InitialValues.Count; i++)
                                    {
                                        float m = InitialValues[i];
                                        if (m > 0)
                                        {
                                            float reduceportion = m < Background ? 0 : m - Background;
                                            InitialValues[i] = m < Background ? m : Background + (reduceportion < IncrementalReduction ? 0 : reduceportion - IncrementalReduction);
                                        }
                                    }
                                    break;
                                case "Quadratic":
                                    break;
                                case "Peak Shaving":
                                    AnthropogenicStandard = standard - Background;
                                    for (int i = 0; i < InitialValues.Count; i++)
                                    {
                                        float m = InitialValues[i];
                                        if (m > 0)
                                        {
                                            float reduceportion = m < Background ? 0 : m - Background;
                                            InitialValues[i] = m < Background ? m : Background + (reduceportion < AnthropogenicStandard ? reduceportion : AnthropogenicStandard);
                                        }
                                    }
                                    break;
                            }
                            OutofAttainmentValue = calculateMetricValue(metric, InitialValues, ordinality);
                        }
                        b.Values = InitialValues;
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        static float calculateMetricValue(Metric metric, List<float> monitorValue, int ordinality)
        {
            try
            {
                Dictionary<int, List<float>> dicHourlyValue365 = new Dictionary<int, List<float>>();
                List<float> lstMetricValue = new List<float>();

                if (metric is FixedWindowMetric)
                {
                    FixedWindowMetric fixedWindowMetric = metric as FixedWindowMetric;
                    int i = 0;
                    while (i < 365)
                    {
                        try
                        {
                            List<float> lstTemp = new List<float>();
                            if (i * 24 + fixedWindowMetric.StartHour < monitorValue.Count && i * 24 + fixedWindowMetric.StartHour + fixedWindowMetric.EndHour - fixedWindowMetric.StartHour < monitorValue.Count)
                                lstTemp = monitorValue.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour + 1);
                            else
                                lstTemp = monitorValue.GetRange(i * 24 + fixedWindowMetric.StartHour, monitorValue.Count - (i * 24 + fixedWindowMetric.StartHour));
                            lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                            if (lstTemp != null && lstTemp.Count > 0)
                            {
                                dicHourlyValue365.Add(i, lstTemp);
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
                    switch (fixedWindowMetric.Statistic)
                    {
                        case MetricStatic.Max:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Max()).ToList();
                            break;
                        case MetricStatic.Mean:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Average()).ToList();
                            break;
                        case MetricStatic.Median:
                            lstMetricValue = dicHourlyValue365.Values.OrderBy(p => p).Select(p => p.Median()).ToList();
                            break;
                        case MetricStatic.Min:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Min()).ToList();
                            break;
                        case MetricStatic.None:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Average()).ToList();
                            break;
                        case MetricStatic.Sum:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Sum()).ToList();
                            break;
                    }
                }
                else if (metric is MovingWindowMetric)
                {
                    MovingWindowMetric movingWindowMetric = metric as MovingWindowMetric;
                    int i = 0;
                    List<float> lstTemp = new List<float>();
                    while (i < monitorValue.Count / 24 + 1)
                    {
                        if (i < monitorValue.Count / 24)
                        {
                            try
                            {
                                lstTemp = monitorValue.GetRange(i * 24, 24);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            try
                            {
                                lstTemp = monitorValue.GetRange(i * 24, monitorValue.Count - i * 24);
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
                        //-----------------end 采用WindowSize------------------
                        lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                        if (lstWindowSize != null && lstWindowSize.Count > 0)
                        {
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
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Max()).ToList();
                            break;
                        case MetricStatic.Mean:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Average()).ToList();
                            break;
                        case MetricStatic.Median:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.OrderBy(a => a).Median()).ToList();
                            break;
                        case MetricStatic.Min:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Min()).ToList();
                            break;
                        case MetricStatic.None:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Average()).ToList();
                            break;
                        case MetricStatic.Sum:
                            lstMetricValue = dicHourlyValue365.Values.Select(p => p.Sum()).ToList();
                            break;
                    }
                }
                else if (metric is CustomerMetric)
                { }
                return lstMetricValue.Where(p => p != float.MinValue).Distinct().OrderByDescending(q => q).ToList()[ordinality - 1];
            }
            catch
            {
                return 0;
            }
        }

        #region rollback metric value -- not use
        //public static bool GetRollBack2StandardMetricValue(StandardRollback sRollback, ref List<MonitorValue> lstMonitorValues, BenMAPPollutant pollutant)
        //{
        //    try
        //    {
        //        int ordinality = sRollback.Ordinality;
        //        float standard = Convert.ToSingle(sRollback.Standard);
        //        string intermethod = sRollback.InterdayRollbackMethod;
        //        float interBackground = Convert.ToSingle(sRollback.InterdayBackground);
        //        Dictionary<string, string> dicSeasonStaticsAll = DataSourceCommonClass.DicSeasonStaticsAll;
        //        List<RowCol> lstSelectGrids = sRollback.SelectRegions;
        //        List<string> lstSelectGridsString = lstSelectGrids.Select(p => p.Col + "," + p.Row).ToList();
        //        foreach (MonitorValue b in lstMonitorValues)
        //        {
        //            if (lstSelectGridsString.Contains(b.Col + "," + b.Row))
        //            {
        //                for (int j = 0; j < b.dicMetricValues365.Count; j++)
        //                {
        //                    KeyValuePair<string, List<float>> kv = b.dicMetricValues365.ElementAt(j);
        //                    string key = kv.Key;
        //                    List<float> InitialMetricValues = kv.Value;
        //                    float OutofAttainmentValue = calculateOutofAttainmentValue(InitialMetricValues, sRollback, ordinality);


        //                    while (OutofAttainmentValue > standard)
        //                    {
        //                        switch (intermethod)
        //                        {
        //                            case "Percentage":
        //                                float AnthropogenicOutofAttainmentValue = OutofAttainmentValue - interBackground;
        //                                float AnthropogenicStandard = standard - interBackground;
        //                                float PercentageReduction = (AnthropogenicOutofAttainmentValue - AnthropogenicStandard) / AnthropogenicOutofAttainmentValue;
        //                                for (int i = 0; i < InitialMetricValues.Count; i++)
        //                                {
        //                                    float m = InitialMetricValues[i];
        //                                    if (m > 0)
        //                                        InitialMetricValues[i] = m < interBackground ? m : interBackground + (m < interBackground ? 0 : (m - interBackground)) * PercentageReduction;
        //                                }
        //                                break;
        //                            case "Incremental":
        //                                float IncrementalReduction = OutofAttainmentValue - standard;
        //                                for (int i = 0; i < InitialMetricValues.Count; i++)
        //                                {
        //                                    float m = InitialMetricValues[i];
        //                                    if (m > 0)
        //                                    {
        //                                        float reduceportion = m < interBackground ? 0 : m - interBackground;
        //                                        InitialMetricValues[i] = m < interBackground ? m : interBackground + (reduceportion < IncrementalReduction ? 0 : reduceportion - IncrementalReduction);
        //                                    }
        //                                }
        //                                break;
        //                            case "Quadratic":
        //                                break;
        //                            case "Peak Shaving":
        //                                AnthropogenicStandard = standard - interBackground;
        //                                for (int i = 0; i < InitialMetricValues.Count; i++)
        //                                {
        //                                    float m = InitialMetricValues[i];
        //                                    if (m > 0)
        //                                    {
        //                                        float reduceportion = m < interBackground ? 0 : m - interBackground;
        //                                        InitialMetricValues[i] = m < interBackground ? m : interBackground + (reduceportion < AnthropogenicStandard ? reduceportion : AnthropogenicStandard);
        //                                    }
        //                                }
        //                                break;
        //                        }
        //                        OutofAttainmentValue = calculateOutofAttainmentValue(InitialMetricValues, sRollback, ordinality);
        //                    }
        //                    b.dicMetricValues365[key] = InitialMetricValues;
        //                    #region
        //                    List<float> lstMonitorValue = InitialMetricValues.Where(p => p != float.MinValue).ToList();
        //                    foreach (Metric m in pollutant.Metrics)
        //                    {
        //                        if (m.MetricName == key)
        //                        {
        //                            if (m is FixedWindowMetric)
        //                            {
        //                                FixedWindowMetric fixedWindowMetric = m as FixedWindowMetric;
        //                                switch (fixedWindowMetric.Statistic)
        //                                {
        //                                    case MetricStatic.Max:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Max();
        //                                        break;
        //                                    case MetricStatic.Mean:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Average();
        //                                        break;
        //                                    case MetricStatic.Median:
        //                                        lstMonitorValue.Sort();
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue[lstMonitorValue.Count / 2];
        //                                        break;
        //                                    case MetricStatic.Min:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Min();
        //                                        break;
        //                                    case MetricStatic.None:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Average();
        //                                        break;
        //                                    case MetricStatic.Sum:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Sum();
        //                                        break;
        //                                }
        //                            }
        //                            else if (m is MovingWindowMetric)
        //                            {
        //                                MovingWindowMetric movingWindowMetric = m as MovingWindowMetric;
        //                                switch (movingWindowMetric.WindowStatistic)
        //                                {
        //                                    case MetricStatic.Max:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Max();
        //                                        break;
        //                                    case MetricStatic.Mean:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Average();
        //                                        break;
        //                                    case MetricStatic.Median:
        //                                        lstMonitorValue.Sort();
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue[lstMonitorValue.Count / 2];
        //                                        break;
        //                                    case MetricStatic.Min:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Min();
        //                                        break;
        //                                    case MetricStatic.None:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Average();
        //                                        break;
        //                                    case MetricStatic.Sum:
        //                                        b.dicMetricValues[m.MetricName] = lstMonitorValue.Sum();
        //                                        break;
        //                                }
        //                            }
        //                            else if (m is CustomerMetric)
        //                            { }
        //                        }
        //                    }
        //                    #endregion
        //                    #region calculate seasonalmetric
        //                    if (pollutant.SesonalMetrics != null && b.dicMetricValues365.Count > 0)
        //                    {
        //                        foreach (SeasonalMetric seasonalmetric in pollutant.SesonalMetrics)
        //                        {
        //                            List<float> lstQuality = new List<float>();
        //                            if ((seasonalmetric.Seasons == null || seasonalmetric.Seasons.Count == 0) && b.dicMetricValues365.ContainsKey(seasonalmetric.Metric.MetricName))//如果没有Seasons则是Mean
        //                            {
        //                                lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                    float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(0, 89 - 0 + 1).Where(p => p != float.MinValue).Average());
        //                                lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                    float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(90, 180 - 90 + 1).Where(p => p != float.MinValue).Average());
        //                                lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                    float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(181, 272 - 181 + 1).Where(p => p != float.MinValue).Average());
        //                                lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                    float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(273, 364 - 273 + 1).Where(p => p != float.MinValue).Average());

        //                                //if (seasonalMetric.Seasons == null || seasonalMetric.Seasons.Count == 0)
        //                                //{
        //                                //----------------基本都包含了四个季度，而且都是Mean，所以应该一致-----------也许要修改
        //                                if (b.dicMetricValues.Keys.Contains(seasonalmetric.Metric.MetricName))
        //                                {
        //                                    b.dicMetricValues.Add(seasonalmetric.SeasonalMetricName, b.dicMetricValues[seasonalmetric.Metric.MetricName]);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                foreach (Season s in seasonalmetric.Seasons)
        //                                {
        //                                    switch (dicSeasonStaticsAll[s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()])
        //                                    {
        //                                        case "":
        //                                        case "Mean":
        //                                            lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? b.dicMetricValues365[seasonalmetric.Metric.MetricName].Average() : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                                float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Average());
        //                                            break;
        //                                        case "Median":
        //                                            lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? b.dicMetricValues365[seasonalmetric.Metric.MetricName].OrderBy(p => p).Median() : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                                float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).OrderBy(p => p).Median());
        //                                            break;
        //                                        case "Max":
        //                                            lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? b.dicMetricValues365[seasonalmetric.Metric.MetricName].Max() : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                                float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Max());
        //                                            break;
        //                                        case "Min":
        //                                            lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? b.dicMetricValues365[seasonalmetric.Metric.MetricName].Min() : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                                float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Min());
        //                                            break;
        //                                        case "Sum":
        //                                            lstQuality.Add(b.dicMetricValues365[seasonalmetric.Metric.MetricName].Count < 365 ? b.dicMetricValues365[seasonalmetric.Metric.MetricName].Sum() : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Count() == 0 ?
        //                                                float.MinValue : b.dicMetricValues365[seasonalmetric.Metric.MetricName].GetRange(s.StartDay, s.EndDay - s.StartDay + 1).Where(p => p != float.MinValue).Sum());
        //                                            break;

        //                                    }

        //                                }
        //                                if (b.dicMetricValues.Keys.Contains(seasonalmetric.Metric.MetricName))
        //                                {
        //                                    if (lstQuality.Where(p => p != float.MinValue).Count() > 0)
        //                                        b.dicMetricValues[seasonalmetric.SeasonalMetricName] = lstQuality.Where(p => p != float.MinValue).Average();
        //                                }

        //                            }
        //                            b.dicMetricValues365[seasonalmetric.SeasonalMetricName] = lstQuality;
        //                        }

        //                    }
        //                    #endregion

        //                }
        //            }
        //        }
        //        return true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogError(ex);
        //        return false;
        //    }
        //}
        #endregion
        static Dictionary<string, string> dicSeasonStaticsAll;
        public static bool GetRollBack2StandardDaily(StandardRollback sRollback, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                dicSeasonStaticsAll = DataSourceCommonClass.DicSeasonStaticsAll;
                int ordinality = sRollback.Ordinality;
                float standard = Convert.ToSingle(sRollback.Standard);
                string method = sRollback.InterdayRollbackMethod;
                float Background = Convert.ToSingle(sRollback.InterdayBackground);

                List<RowCol> lstSelectGrids = sRollback.SelectRegions;
                List<string> lstSelectGridsString = lstSelectGrids.Select(p => p.Col + "," + p.Row).ToList();
                foreach (MonitorValue b in lstMonitorValues)
                {
                    if (lstSelectGridsString.Contains(b.Col + "," + b.Row))
                    {
                        List<float> InitialValues = b.Values;
                        Metric metric = sRollback.DailyMetric;
                        float OutofAttainmentValue = calculateOutofAttainmentValue(b, sRollback, ordinality);
                        if (OutofAttainmentValue <= 0) continue;
                        while (OutofAttainmentValue > standard)
                        {
                            switch (method)
                            {
                                case "Percentage":
                                    float AnthropogenicOutofAttainmentValue = OutofAttainmentValue - Background;
                                    float AnthropogenicStandard = standard - Background;
                                    float PercentageReduction = (AnthropogenicOutofAttainmentValue - AnthropogenicStandard) / AnthropogenicOutofAttainmentValue;
                                    for (int i = 0; i < InitialValues.Count; i++)
                                    {
                                        float m = InitialValues[i];
                                        if (m > 0)
                                            InitialValues[i] = m < Background ? m : Background + (m < Background ? 0 : (m - Background)) * PercentageReduction;
                                    }
                                    break;
                                case "Incremental":
                                    float IncrementalReduction = OutofAttainmentValue - standard;
                                    for (int i = 0; i < InitialValues.Count; i++)
                                    {
                                        float m = InitialValues[i];
                                        if (m > 0)
                                        {
                                            float reduceportion = m < Background ? 0 : m - Background;
                                            InitialValues[i] = m < Background ? m : Background + (reduceportion < IncrementalReduction ? 0 : reduceportion - IncrementalReduction);
                                        }
                                    }
                                    break;
                                case "Quadratic":
                                    break;
                                case "Peak Shaving":
                                    AnthropogenicStandard = standard - Background;
                                    for (int i = 0; i < InitialValues.Count; i++)
                                    {
                                        float m = InitialValues[i];
                                        if (m > 0)
                                        {
                                            float reduceportion = m < Background ? 0 : m - Background;
                                            InitialValues[i] = m < Background ? m : Background + (reduceportion < AnthropogenicStandard ? reduceportion : AnthropogenicStandard);
                                        }
                                    }
                                    break;
                            }
                            b.Values = InitialValues;
                            OutofAttainmentValue = calculateOutofAttainmentValue(b, sRollback, ordinality);
                        }
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        static float calculateOutofAttainmentValue(MonitorValue monitorValue, StandardRollback sRollback, int ordinality)
        {
            try
            {
                float OutofAttainmentValue = 0;
                Metric m = sRollback.DailyMetric;
                monitorValue.dicMetricValues = new Dictionary<string, float>();
                monitorValue.dicMetricValues365 = new Dictionary<string, List<float>>();
                if (m is FixedWindowMetric)
                {
                    FixedWindowMetric fixedWindowMetric = (FixedWindowMetric)m;
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
                else if (m is MovingWindowMetric)
                {
                    MovingWindowMetric movingWindowMetric = (MovingWindowMetric)m;
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

                if (sRollback.SeasonalMetric != null && monitorValue.dicMetricValues365.Count > 0)
                {
                    SeasonalMetric seasonalmetric = sRollback.SeasonalMetric;
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
                            switch (dicSeasonStaticsAll[s.StartDay.ToString() + "," + seasonalmetric.SeasonalMetricID.ToString()])
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

                if (sRollback.SeasonalMetric != null && sRollback.AnnualStatistic == MetricStatic.None)
                {
                    OutofAttainmentValue = monitorValue.dicMetricValues365[sRollback.SeasonalMetric.SeasonalMetricName].Distinct().OrderByDescending(q => q).ToList()[ordinality - 1];
                }
                else if (sRollback.SeasonalMetric != null && sRollback.AnnualStatistic != MetricStatic.None)
                {
                    List<float> lstTmp = monitorValue.dicMetricValues365[sRollback.SeasonalMetric.SeasonalMetricName].Where(p => p != float.MinValue).ToList();
                    switch (sRollback.AnnualStatistic)
                    {
                        case MetricStatic.Max:
                            OutofAttainmentValue = lstTmp.Max();
                            break;
                        case MetricStatic.Mean:
                            OutofAttainmentValue = lstTmp.Average();
                            break;
                        case MetricStatic.Median:
                            lstTmp.Sort();
                            OutofAttainmentValue = lstTmp[lstTmp.Count / 2];
                            break;
                        case MetricStatic.Min:
                            OutofAttainmentValue = lstTmp.Min();
                            break;
                        case MetricStatic.Sum:
                            OutofAttainmentValue = lstTmp.Sum();
                            break;
                    }
                }
                else if (sRollback.SeasonalMetric == null && sRollback.AnnualStatistic != MetricStatic.None)
                {
                    List<float> lstTmp = monitorValue.dicMetricValues365[sRollback.DailyMetric.MetricName].Where(p => p != float.MinValue).ToList();
                    switch (sRollback.AnnualStatistic)
                    {
                        case MetricStatic.Max:
                            OutofAttainmentValue = lstTmp.Max();
                            break;
                        case MetricStatic.Mean:
                            OutofAttainmentValue = lstTmp.Average();
                            break;
                        case MetricStatic.Median:
                            lstTmp.Sort();
                            OutofAttainmentValue = lstTmp[lstTmp.Count / 2];
                            break;
                        case MetricStatic.Min:
                            OutofAttainmentValue = lstTmp.Min();
                            break;
                        case MetricStatic.Sum:
                            OutofAttainmentValue = lstTmp.Sum();
                            break;
                    }
                }
                else
                { OutofAttainmentValue = monitorValue.dicMetricValues365[sRollback.DailyMetric.MetricName].Distinct().OrderByDescending(q => q).ToList()[ordinality - 1]; }
                return OutofAttainmentValue;
            }
            catch
            {
                return 0;
            }
        }

        private bool GetStandardPercentage(ref double[] values, double standard, double background)
        {
            try
            {
                if (values == null || values.Length < 1) { return false; }
                double[] tmps = new double[values.Length];
                values.CopyTo(tmps, 0);
                Array.Sort(tmps);
                double max = tmps[values.Length - 1];
                double percentage = ((max - background) - (standard - background)) / (max - background);
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] > background) { values[i] = (values[i] - background) * percentage + background; }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }//F_GetStandardPercentage

        private bool GetStandardIncremental(ref double[] values, double standard, double background)
        {
            try
            {
                if (values == null || values.Length < 1) { return false; }
                double[] tmps = new double[values.Length];
                values.CopyTo(tmps, 0);
                Array.Sort(tmps);
                double max = tmps[values.Length - 1];
                double incremental = ((max - background) - (standard - background)) / (max - background);
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] > background) { values[i] = (values[i] - background) * incremental + background; }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 找到监测点对应的行列号
        /// </summary>
        /// <param name="rollbackGridType"></param>
        /// <param name="lstMonitorValues"></param>
        /// <returns></returns>
        public static bool GetMonitorPointsColRow(BenMAPGrid rollbackGridType, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                IFeatureSet fs = new FeatureSet();
                string str = string.Format("{0}\\Data\\Shapefiles\\" + CommonClass.MainSetup.SetupName + "\\{1}.shp", CommonClass.DataFilePath, (rollbackGridType as ShapefileGrid).ShapefileName);
                fs=FeatureSet.Open(str);
                Coordinate c;
                Point p;
                int row = -1, col = -1, i = 0, iCol = -1, iRow = -1;
                foreach (DataColumn dc in fs.DataTable.Columns)
                {
                    if (dc.ColumnName.ToLower() == "col")
                    {
                        iCol = i;
                    }
                    if (dc.ColumnName.ToLower() == "row")
                    {
                        iRow = i;
                    }
                    i++;
                }
                foreach (MonitorValue mv in lstMonitorValues)
                {
                    //c = new Coordinate(mv.Longitude, mv.Latitude);
                    p = new Point(mv.Longitude, mv.Latitude);
                    foreach (Feature fl in fs.Features)
                    {
                        if (fl.Envelope.Contains(mv.Longitude, mv.Latitude))
                        //if(isEnvelopContainXY(fl.Envelope,mv.Longitude, mv.Latitude))
                        {
                            if (fl.Contains(p))
                            {
                                col = Convert.ToInt32(fl.DataRow[iCol]);
                                row = Convert.ToInt32(fl.DataRow[iRow]);
                                break;
                            }
                        }
                    }
                    mv.Row = row;
                    mv.Col = col;
                }
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        public static bool isEnvelopContainXY(IEnvelope env, double x, double y)
        {
            try
            {
                bool breturn = false;
                if (x >= env.Minimum.X && x <= env.Maximum.X && y >= env.Minimum.Y && y <= env.Maximum.Y)
                {
                    breturn = true;
                }
                return breturn;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="benMAPGrid"></param>
        /// <param name="benMAPPollutant"></param>
        /// <param name="monitorDataLine"></param>
        public static void UpdateModelValuesMonitorData(List<MonitorValue> lstMonitorValues, ref MonitorDataLine monitorDataLine)
        {
            #region 注释说明

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

            #endregion 注释说明

            try
            {
                int i = 0;
                Dictionary<string, Metric> dicMetric = new Dictionary<string, Metric>();
                BenMAPPollutant benMAPPollutant = monitorDataLine.Pollutant;
                BenMAPGrid benMAPGrid = monitorDataLine.GridType;
                foreach (Metric m in benMAPPollutant.Metrics)
                {
                    dicMetric.Add(m.MetricName, m);
                }
                Dictionary<string, SeasonalMetric> dicSeasonalMetric = new Dictionary<string, SeasonalMetric>();
                foreach (SeasonalMetric m in benMAPPollutant.SesonalMetrics)
                {
                    dicSeasonalMetric.Add(m.SeasonalMetricName, m);
                }
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
                    Coordinate cCenter = fsPoints.Extent.Center;
                    //-----------求到中心点的距离排倒序，取100个
                    Dictionary<MonitorValue, double> dicCoordinateDistanceCenter = new Dictionary<MonitorValue, double>();
                    foreach (MonitorValue monitorValue in lstMonitorValues)
                    {
                        dicCoordinateDistanceCenter.Add(monitorValue, DataSourceCommonClass.getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, cCenter.X, cCenter.Y));
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
                                if (dClose != -1 && DataSourceCommonClass.getDistanceFrom2Point(coordinate.X, coordinate.Y, DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude) > dClose)// DicMonitorDistanceKeyValue.Value<dClose)//Math.Sqrt(DicMonitorDistanceKeyValue.Value)-Math.Min(fs.GetFeature(i).Envelope.Height,fs.GetFeature(i).Envelope.Width) > dClose)
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
                                        Distance = DataSourceCommonClass.getDistanceFrom2Point(new Point(coordinate), new Point(DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude)),
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
                                    DicMonitorDistance.Add(monitorValue, DataSourceCommonClass.getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, coordinate.X, coordinate.Y));//(coordinate.X - monitorValue.Longitude) * (coordinate.X - monitorValue.Longitude) + (coordinate.Y - monitorValue.Latitude) * (coordinate.Y - monitorValue.Latitude));// getDistanceFrom2Point(coordinate.X,coordinate.Y,monitorValue.Longitude,monitorValue.Latitude));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// getDistanceFromExtent(fs.GetFeature(i).Envelope.ToExtent().Center,fs.GetFeature(i).Envelope, new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));//(coordinate.X-monitorValue.Longitude)*(coordinate.X-monitorValue.Longitude)+(coordinate.Y-monitorValue.Latitude)*(coordinate.Y-monitorValue.Latitude));// fs.GetFeature(i).Envelope.ToExtent().Center.Distance(new DotSpatial.Topology.Point(monitorValue.Longitude, monitorValue.Latitude)));
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

                                var query = DicMonitorDistance.Where(p => p.Value < 25).ToList();//.OrderBy(p=>p.Value).ToList().GetRange(0,idicMonitorValues).ToDictionary(p=>p.Key,p=>p.Value);// .Where(p => lstDouble.GetRange(0, idicMonitorValues).Contains(p.Value));
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
                                        float d = DataSourceCommonClass.getDistanceFrom2Point(coordinate.X, coordinate.Y, k.Key.Longitude, k.Key.Latitude);
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
                                        float d = DataSourceCommonClass.getDistanceFrom2Point(coordinate.X, coordinate.Y, k.Key.Longitude, k.Key.Latitude);
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
                                            float dDistance = DataSourceCommonClass.getDistanceFrom2Point(mvTemp.Longitude, mvTemp.Latitude,
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

        public static List<MonitorValue> GetMonitorPointsValues(ref MonitorDataLine monitorDataLine)
        {
            try
            {
                int i = 0;
                BenMAPGrid benMAPGrid = monitorDataLine.GridType;
                BenMAPPollutant benMAPPollutant = monitorDataLine.Pollutant;
                List<MonitorValue> lstMonitorValues = new List<MonitorValue>();
                List<MonitorValue> lstMonitorValuesProcessed = new List<MonitorValue>();
                //首先获取Monitor的Attributes
                //Monitor的存储方法，用byte[] byteArray = System.Text.Encoding.Default.GetBytes ( str ); To MONITORENTRIES 表
                //string str = System.Text.Encoding.Default.GetString ( byteArray );

                string commandText = "";
                MonitorValue mv = new MonitorValue();
                if (monitorDataLine.MonitorDirectType == 0)//Library
                {
                    commandText = string.Format("select a.MonitorEntryID,a.MonitorID,a.YYear,a.MetricID,a.SeasonalMetricID,a.Statistic,a.VValues,b.PollutantID,b.Latitude,b.Longitude,b.MonitorName,b.MonitorDescription from MonitorEntries a,Monitors b,MonitorDataSets c where a.MonitorID=b.MonitorID and b.MonitorDataSetID=c.MonitorDataSetID and b.PollutantID={0} and c.MonitorDataSetID={1} and a.YYear={2} ", benMAPPollutant.PollutantID, monitorDataLine.MonitorDataSetID, monitorDataLine.MonitorLibraryYear);//----------------------------------------------
                    ///---得到所有Monitor的值
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    Byte[] blob = null;
                    lstMonitorValues = new List<MonitorValue>();

                    string str = "";
                    string[] strArray = null;
                    while (fbDataReader.Read())
                    {
                        mv = new MonitorValue();

                        //blob = new Byte[(fbDataReader.GetBytes(6, 0, null, 0, int.MaxValue))];
                        //fbDataReader.GetBytes(0, 0, blob, 0, blob.Length);
                        blob = fbDataReader[6] as byte[];
                        // object test = DeserializeObject(blob);
                        str = System.Text.Encoding.Default.GetString(blob);
                        strArray = str.Split(new char[] { ',' });
                        mv.Latitude = Convert.ToDouble(fbDataReader["Latitude"]);
                        mv.Longitude = Convert.ToDouble(fbDataReader["Longitude"]);
                        if (!(fbDataReader["MetricID"] is DBNull))
                        {
                            mv.Metric = Grid.GridCommon.getMetricFromID(Convert.ToInt32(fbDataReader["MetricID"]));
                        }
                        if (!(fbDataReader["SeasonalMetricID"] is DBNull))
                        {
                            mv.SeasonalMetric = Grid.GridCommon.getSeasonalMetric(Convert.ToInt32(fbDataReader["SeasonalMetricID"]));
                        }
                        mv.Statistic = fbDataReader["Statistic"].ToString();
                        mv.Values = new List<float>();
                        foreach (string s in strArray)
                        {
                            if (string.IsNullOrEmpty(s) || s.Trim() == ".")
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
                    fbDataReader.Close();
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
                   System.Data.DataTable dt = CommonClass.ExcelToDataTable(monitorDataLine.MonitorDataFilePath);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        switch (dc.ColumnName.ToLower().Replace(" ", ""))
                        {
                            case "monitorname":
                                iMonitorName = i;
                                break;
                            case "monitordescription":
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
                                iStatistic = i;
                                break;
                            case "values":
                                iValues = i;
                                break;
                        }
                        i++;
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
                            lstMetric = benMAPPollutant.Metrics.Where(p => p.MetricName.ToLower() == dr[iMetric].ToString().ToLower()).ToList();
                            if (lstMetric != null && lstMetric.Count > 0)
                            {
                                mv.Metric = lstMetric.First();
                            }
                        }
                        if (!string.IsNullOrEmpty(dr[iSeasonalMetric].ToString()) && benMAPPollutant.SesonalMetrics != null && benMAPPollutant.SesonalMetrics.Count > 0)
                        {
                            lstSeasonalMetric = benMAPPollutant.SesonalMetrics.Where(p => p.SeasonalMetricName == dr[iSeasonalMetric].ToString()).ToList();
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
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }

        public static void UpdateMonitorDicMetricValue(BenMAPPollutant benMAPPollutant, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                ///-------------首先生成Metric值-----------先忽略是否有效的问题------------------
                int hourly = 0;//1代表hourly
                int i = 0;
                FixedWindowMetric fixedWindowMetric = null;
                MovingWindowMetric movingWindowMetric = null;
                Dictionary<int, List<float>> dicHourlyValue = new Dictionary<int, List<float>>();
                List<float> lstTemp = null;
                foreach (MonitorValue monitorValue in lstMonitorValues)
                {
                    monitorValue.dicMetricValues = new Dictionary<string, float>();
                    if (monitorValue.SeasonalMetric != null)
                    {
                        monitorValue.dicMetricValues.Add(monitorValue.SeasonalMetric.SeasonalMetricName, monitorValue.Values.First());
                    }
                    else if (monitorValue.Metric != null)
                    {
                        monitorValue.dicMetricValues.Add(monitorValue.Metric.MetricName, monitorValue.Values.First());
                    }
                    else
                    {
                        //如果没有metric ，判断value 大小 +1/365 -> 24 每个小时一个数据  +1/365 1 每天一个数据 少于365 默认为从第一天算起的按天的数据
                        if (monitorValue.Values.Count == 8759) hourly = 1;
                        if (benMAPPollutant.Metrics != null && benMAPPollutant.Metrics.Count > 0)
                        {
                            foreach (Metric m in benMAPPollutant.Metrics)
                            {
                                if (m is FixedWindowMetric)
                                {
                                    fixedWindowMetric = (FixedWindowMetric)m;
                                    ///-----------如果是hourly，计算小时值根据staticstic,然后计算年值
                                    if (hourly == 0 || fixedWindowMetric.HourlyMetricGeneration == 1 || (fixedWindowMetric.StartHour == 0 && fixedWindowMetric.EndHour == 23))
                                    {
                                        monitorValue.Values = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                                        if (monitorValue.Values != null && monitorValue.Values.Count > 0)
                                        {
                                            switch (fixedWindowMetric.Statistic)
                                            {
                                                case MetricStatic.Max:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Max());
                                                    break;
                                                case MetricStatic.Mean:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Average());
                                                    break;
                                                case MetricStatic.Median:
                                                    monitorValue.Values.Sort();
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values[monitorValue.Values.Count / 2]);//----------错的，要重做为中间值
                                                    break;
                                                case MetricStatic.Min:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Min());
                                                    break;
                                                case MetricStatic.None:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Average());
                                                    break;
                                                case MetricStatic.Sum:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Sum());
                                                    break;
                                            }// swith
                                        }//if_monitorValue
                                    }
                                    else
                                    {
                                        ///---根据开始hour和结束hour再通过Statistic计算---------首先生成一个List<double>---
                                        dicHourlyValue = new Dictionary<int, List<float>>();
                                        i = 0;
                                        while (i < 365)
                                        {
                                            //lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour);
                                            //lstTemp.Sort();
                                            lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour);
                                            lstTemp.Remove(Convert.ToSingle(float.MinValue));
                                            if (lstTemp != null && lstTemp.Count > 0)
                                                dicHourlyValue.Add(i, lstTemp);// monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour));
                                            i++;
                                        }
                                        switch (fixedWindowMetric.Statistic)
                                        {
                                            case MetricStatic.Max:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Max()));
                                                break;
                                            case MetricStatic.Mean:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                break;
                                            case MetricStatic.Median:
                                                lstTemp = new List<float>();
                                                foreach (List<float> ld in dicHourlyValue.Values)
                                                {
                                                    lstTemp.Add(ld.OrderBy(p => p).Median());
                                                }
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p=>p).Median());//----------错的，要重做为中间值
                                                break;
                                            case MetricStatic.Min:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Min()));
                                                break;
                                            case MetricStatic.None:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Max()));
                                                break;
                                            case MetricStatic.Sum:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Sum()));
                                                break;
                                        }//swith_fixedWindowMetric.Statistic
                                        ///---------计算值
                                    }

                                    ///------------如果不是，直接计算-----------------------
                                }
                                else if (m is MovingWindowMetric)
                                {
                                    movingWindowMetric = (MovingWindowMetric)m;
                                    ///------------如果是hourly，计算小时值根据staticstic,然后计算年值
                                    if (hourly == 0 || movingWindowMetric.HourlyMetricGeneration == 1)
                                    {
                                        monitorValue.Values = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                                        if (monitorValue.Values != null && monitorValue.Values.Count > 0)
                                        {
                                            switch (movingWindowMetric.DailyStatistic)
                                            {
                                                case MetricStatic.Max:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Max());
                                                    break;
                                                case MetricStatic.Mean:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Average());
                                                    break;
                                                case MetricStatic.Median:
                                                    monitorValue.Values.Sort();
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values[monitorValue.Values.Count / 2]);//----------错的，要重做为中间值
                                                    break;
                                                case MetricStatic.Min:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Min());
                                                    break;
                                                case MetricStatic.None:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Average());
                                                    break;
                                                case MetricStatic.Sum:
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Sum());
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ///----根据开始hour和结束hour再通过Statistic计算---------首先生成一个List<double>---
                                        dicHourlyValue = new Dictionary<int, List<float>>();
                                        i = 0;
                                        //monitorValue.Values.Remove(float.MinValue);

                                        while (i < 365)
                                        {
                                            lstTemp = monitorValue.Values.GetRange(i * 24, 24);
                                            //lstTemp.Remove(float.MinValue);
                                            lstTemp = lstTemp.Where(p => p != float.MinValue).ToList();
                                            if (lstTemp != null && lstTemp.Count > 0)
                                                dicHourlyValue.Add(i, lstTemp);
                                            i++;
                                        }
                                        switch (movingWindowMetric.DailyStatistic)
                                        {
                                            case MetricStatic.Max:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Max()));
                                                break;
                                            case MetricStatic.Mean:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Average(p => p.Average()));
                                                break;
                                            case MetricStatic.Median:
                                                lstTemp = new List<float>();
                                                foreach (List<float> ld in dicHourlyValue.Values)
                                                {
                                                    lstTemp.Add(ld.OrderBy(p => p).Median());
                                                }
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p=>p).Median());//----------错的，要重做为中间值
                                                break;
                                            case MetricStatic.Min:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Min(p => p.Min()));
                                                break;
                                            case MetricStatic.None:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Max(p => p.Max()));
                                                break;
                                            case MetricStatic.Sum:
                                                monitorValue.dicMetricValues.Add(m.MetricName, dicHourlyValue.Values.Sum(p => p.Sum()));
                                                break;
                                        }
                                        ///---------计算值
                                    }
                                }
                                else if (m is CustomerMetric)
                                {
                                    ///---------------按函数来统计------------------暂不实现--------------------------需要讨论后再做---
                                }
                                else
                                {
                                    //fixedWindowMetric = (FixedWindowMetric)m;
                                    ///-----------如果是hourly，计算小时值根据staticstic,然后计算年值

                                    //monitorValue.Values.Remove(float.MinValue);
                                    monitorValue.Values = monitorValue.Values.Where(p => p != float.MinValue).ToList();
                                    if (monitorValue.Values != null && monitorValue.Values.Count > 0)
                                        monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values.Average());
                                    else
                                        monitorValue.dicMetricValues.Add(m.MetricName, Convert.ToSingle(0.0));
                                }
                            }
                            if (benMAPPollutant.SesonalMetrics != null)
                            {
                                foreach (SeasonalMetric seasonalMetric in benMAPPollutant.SesonalMetrics)
                                {
                                    //if (seasonalMetric.Seasons == null || seasonalMetric.Seasons.Count == 0)
                                    //{
                                    ///---------------基本都包含了四个季度，而且都是Mean，所以应该一致-----------也许要修改
                                    if (monitorValue.dicMetricValues.Keys.Contains(seasonalMetric.Metric.MetricName))
                                    {
                                        monitorValue.dicMetricValues.Add(seasonalMetric.SeasonalMetricName, monitorValue.dicMetricValues[seasonalMetric.Metric.MetricName]);
                                    }
                                    //}
                                }// foreach
                            }//if_benMAPPollutant
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public static double GetDistanceFrom2Point(DotSpatial.Topology.Point start, DotSpatial.Topology.Point end)
        {
            return 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((start.Y / 180 * Math.PI - end.Y / 180 * Math.PI) / 2)), 2) +
             Math.Cos(start.Y / 180 * Math.PI) * Math.Cos(end.Y / 180 * Math.PI) * Math.Pow(Math.Sin((start.X / 180 * Math.PI - end.X / 180 * Math.PI) / 2), 2))) * 6371.000;
        }

        public static float GetDistanceFrom2Point(double X0, double Y0, double X1, double Y1)
        {
            return Convert.ToSingle(2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((Y0 / 180 * Math.PI - Y1 / 180 * Math.PI) / 2)), 2) +
             Math.Cos(Y0 / 180 * Math.PI) * Math.Cos(Y1 / 180 * Math.PI) * Math.Pow(Math.Sin((X0 / 180 * Math.PI - X1 / 180 * Math.PI) / 2), 2))) * 6371.000);
        }

        public static double GetDistanceFromExtent(DotSpatial.Topology.Coordinate coordinate, DotSpatial.Topology.IEnvelope env, DotSpatial.Topology.Point end)
        {
            double d = Math.Sqrt((coordinate.X - end.X) * (coordinate.X - end.X) + (coordinate.Y - end.Y) * (coordinate.Y - end.Y)) * 111.0000;// getDistanceFrom2Point(new DotSpatial.Topology.Point(env.ToExtent().Center.X, env.ToExtent().Center.Y), end);
            if (d < env.Height / 2.00 && d < env.Width / 2.00)
            {
                return 0.0;
            }
            else
            {
                return env.Height > env.Width ? d - env.Width / 2.00 : d - env.Height;
            }
        }

        /// <summary>
        /// 泰森多边形
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="result"></param>
        public static void VoronoiPoints(double[] vertices, ref List<double> result)//,List<KeyValuePair<Coordinate,double>> ikey)
        {
            try
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
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }// F_VoronoiPoints

        public static List<MonitorValue> GetMonitorData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant,  MonitorDataLine monitorDataLine)
        {
            try
            {
                int i = 0;
                List<MonitorValue> lstMonitorValues = new List<MonitorValue>();
                List<MonitorValue> lstMonitorValuesProcessed = new List<MonitorValue>();
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
                    while (fbDataReader.Read())
                    {
                        mv = new MonitorValue();
                        blob = fbDataReader[6] as byte[];
                        // object test = DeserializeObject(blob);
                        str = System.Text.Encoding.Default.GetString(blob);
                        strArray = str.Split(new char[] { ',' });
                        mv.Latitude = Convert.ToDouble(fbDataReader["Latitude"]);
                        mv.Longitude = Convert.ToDouble(fbDataReader["Longitude"]);
                        if (!(fbDataReader["MetricID"] is DBNull))
                        {
                            mv.Metric = Grid.GridCommon.getMetricFromID(Convert.ToInt32(fbDataReader["MetricID"]));
                        }
                        if (!(fbDataReader["SeasonalMetricID"] is DBNull))
                        {
                            mv.SeasonalMetric = Grid.GridCommon.getSeasonalMetric(Convert.ToInt32(fbDataReader["SeasonalMetricID"]));
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
                        lstMonitorValues.Add(mv);
                    }
                    fbDataReader.Close();
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
                    System.Data.DataTable dt = CommonClass.ExcelToDataTable(monitorDataLine.MonitorDataFilePath);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        switch (dc.ColumnName.ToLower().Replace(" ", ""))
                        {
                            case "monitorname":
                                iMonitorName = i;
                                break;
                            case "monitordescription":
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
                                iStatistic = i;
                                break;
                            case "values":
                                iValues = i;
                                break;
                        }
                        i++;
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
                            lstMetric = benMAPPollutant.Metrics.Where(p => p.MetricName.ToLower() == dr[iMetric].ToString().ToLower()).ToList();
                            if (lstMetric != null && lstMetric.Count > 0)
                            {
                                mv.Metric = lstMetric.First();
                            }
                        }
                        if (!string.IsNullOrEmpty(dr[iSeasonalMetric].ToString()) && benMAPPollutant.SesonalMetrics != null && benMAPPollutant.SesonalMetrics.Count > 0)
                        {
                            lstSeasonalMetric = benMAPPollutant.SesonalMetrics.Where(p => p.SeasonalMetricName == dr[iSeasonalMetric].ToString()).ToList();
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
                    }// foreach datarow
                }//if
                return lstMonitorValues;
            }// try
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }


    }//C_RollBackDalgorithm
}