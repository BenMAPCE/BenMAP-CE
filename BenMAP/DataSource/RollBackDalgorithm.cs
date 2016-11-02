using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.NTSExtension.Voronoi;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using FirebirdSql.Data.FirebirdClient;

namespace BenMAP.DataSource
{
    public class RollBackDalgorithm
    {
        public RollBackDalgorithm()
        {
        }

        public static bool UpdateMonitorDataRollBack(ref MonitorModelRollbackLine monitorModelRollbackLine)
        {
            bool ok = false;
            try
            {
                List<BenMAPRollback> lstBenMapRollback = monitorModelRollbackLine.BenMAPRollbacks;
                if (lstBenMapRollback == null || lstBenMapRollback.Count < 1) { return false; }
                MonitorDataLine mdl = new MonitorDataLine()
                {
                    GridType = monitorModelRollbackLine.GridType,
                    Pollutant = monitorModelRollbackLine.Pollutant,
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
                List<MonitorValue> lstMonitorValues = DataSourceCommonClass.GetMonitorData(mdl.GridType, mdl.Pollutant, mdl);
                if (lstMonitorValues == null || lstMonitorValues.Count < 1)
                { return false; }
                ok = GetMonitorPointsColRow(monitorModelRollbackLine.RollbackGrid, ref lstMonitorValues);
                if (!ok) { return false; }

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
                            { }
                            DataSourceCommonClass.UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, lstMonitorValues);
                            break;
                        case RollbackType.incremental:
                            IncrementalRollback ir = b as IncrementalRollback;
                            ok = GetIncrementalRollBackValues(ir, ref lstMonitorValues);
                            if (!ok)
                            { }
                            DataSourceCommonClass.UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, lstMonitorValues);
                            break;
                        case RollbackType.standard:
                            StandardRollback sr = b as StandardRollback;
                            if (mdl.Pollutant.Observationtype == ObservationtypeEnum.Hourly)
                            {
                                ok = GetRollBack2StandardHourly(sr, ref lstMonitorValues); if (!ok)
                                { }
                            }
                            if (mdl.Pollutant.Observationtype == ObservationtypeEnum.Daily)
                            {
                                ok = GetRollBack2StandardDaily(sr, ref lstMonitorValues); if (!ok)
                                { }
                            }

                            DataSourceCommonClass.UpdateMonitorDicMetricValue(monitorModelRollbackLine.Pollutant, lstMonitorValues);
                            break;
                    }
                }
                UpdateModelValuesMonitorData(lstMonitorValues, ref mdl);
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

        public static bool GetPercentageRollbackValues(PercentageRollback pRollback, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                double background = pRollback.Background;
                double percentage = pRollback.Percent / 100;
                List<RowCol> lstSelectGrids = pRollback.SelectRegions;
                List<string> lstSelectGridsString = lstSelectGrids.Select(p => p.Col + "," + p.Row).ToList();
                string[] keyIndex;
                foreach (MonitorValue b in lstMonitorValues)
                {
                    if (lstSelectGridsString.Contains(b.Col + "," + b.Row))
                    {
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
        public static bool GetIncrementalRollBackValues(IncrementalRollback iRollback, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                double background = iRollback.Background;
                double increment = iRollback.Increment;
                double tmp = 0.0;
                List<RowCol> lstSelectGrids = iRollback.SelectRegions;
                List<string> lstSelectGridsString = lstSelectGrids.Select(p => p.Col + "," + p.Row).ToList();
                string[] keyIndex;
                foreach (MonitorValue b in lstMonitorValues)
                {
                    if (lstSelectGridsString.Contains(b.Col + "," + b.Row))
                    {

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
        }
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

                if (sRollback.SeasonalMetric != null && monitorValue.dicMetricValues365.Count > 0)
                {
                    SeasonalMetric seasonalmetric = sRollback.SeasonalMetric;
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
        }
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

        public static bool GetMonitorPointsColRow(BenMAPGrid rollbackGridType, ref List<MonitorValue> lstMonitorValues)
        {
            try
            {
                IFeatureSet fs = new FeatureSet();
                string str = string.Format("{0}\\Data\\Shapefiles\\" + CommonClass.MainSetup.SetupName + "\\{1}.shp", CommonClass.DataFilePath, (rollbackGridType as ShapefileGrid).ShapefileName);
                fs = FeatureSet.Open(str);
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
                    p = new Point(mv.Longitude, mv.Latitude);
                    foreach (var fl in fs.Features)
                    {
                        if (fl.Geometry.EnvelopeInternal.Contains(mv.Longitude, mv.Latitude))
                        {
                            if (fl.Geometry.Contains(p))
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
                if (x >= env.MinX && x <= env.MaxX && y >= env.MinY && y <= env.MaxY)
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
        public static void UpdateModelValuesMonitorData(List<MonitorValue> lstMonitorValues, ref MonitorDataLine monitorDataLine)
        {




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
                        dClose = monitorDataLine.MonitorAdvance.MaxinumNeighborDistance;
                    else if (monitorDataLine.MonitorAdvance != null && monitorDataLine.MonitorAdvance.RelativeNeighborDistance != -1)
                        dClose = monitorDataLine.MonitorAdvance.RelativeNeighborDistance;
                    IFeatureSet fsPoints = new FeatureSet();
                    List<Coordinate> lstCoordinate = new List<Coordinate>();

                    List<double> fsInter = new List<double>();
                    Dictionary<string, MonitorValue> dicMonitorValues = new Dictionary<string, MonitorValue>(); ;
                    foreach (MonitorValue monitorValue in lstMonitorValues)
                    {
                        if (monitorValue.dicMetricValues == null || monitorValue.dicMetricValues.Count == 0) continue; if (!dicMonitorValues.ContainsKey(monitorValue.Longitude + "," + monitorValue.Latitude))
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
                    foreach (MonitorValue monitorValue in lstMonitorValues)
                    {
                        dicCoordinateDistanceCenter.Add(monitorValue, DataSourceCommonClass.getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, cCenter.X, cCenter.Y));
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
                                if (dClose != -1 && DataSourceCommonClass.getDistanceFrom2Point(coordinate.X, coordinate.Y, DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude) > dClose) { }
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
    Distance = DataSourceCommonClass.getDistanceFrom2Point(new Point(coordinate), new Point(DicMonitorDistanceKeyValue.Key.Longitude, DicMonitorDistanceKeyValue.Key.Latitude)),
    MonitorName = DicMonitorDistanceKeyValue.Key.MonitorName,
    Weight = 1
});
                                }
                                break;
                            case InterpolationMethodEnum.FixedRadius:
                                Point tPoint = new Point(coordinate);
                                DicMonitorDistance = new Dictionary<MonitorValue, float>();

                                foreach (MonitorValue monitorValue in lstMonitorValues)
                                {
                                    DicMonitorDistance.Add(monitorValue, DataSourceCommonClass.getDistanceFrom2Point(monitorValue.Longitude, monitorValue.Latitude, coordinate.X, coordinate.Y));
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

        public static List<MonitorValue> GetMonitorPointsValues(ref MonitorDataLine monitorDataLine)
        {
            try
            {
                int i = 0;
                BenMAPGrid benMAPGrid = monitorDataLine.GridType;
                BenMAPPollutant benMAPPollutant = monitorDataLine.Pollutant;
                List<MonitorValue> lstMonitorValues = new List<MonitorValue>();
                List<MonitorValue> lstMonitorValuesProcessed = new List<MonitorValue>();

                string commandText = "";
                MonitorValue mv = new MonitorValue();
                if (monitorDataLine.MonitorDirectType == 0)
                {
                    commandText = string.Format("select a.MonitorEntryID,a.MonitorID,a.YYear,a.MetricID,a.SeasonalMetricID,a.Statistic,a.VValues,b.PollutantID,b.Latitude,b.Longitude,b.MonitorName,b.MonitorDescription from MonitorEntries a,Monitors b,MonitorDataSets c where a.MonitorID=b.MonitorID and b.MonitorDataSetID=c.MonitorDataSetID and b.PollutantID={0} and c.MonitorDataSetID={1} and a.YYear={2} ", benMAPPollutant.PollutantID, monitorDataLine.MonitorDataSetID, monitorDataLine.MonitorLibraryYear); ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    Byte[] blob = null;
                    lstMonitorValues = new List<MonitorValue>();

                    string str = "";
                    string[] strArray = null;
                    while (fbDataReader.Read())
                    {
                        mv = new MonitorValue();

                        blob = fbDataReader[6] as byte[];
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
                int hourly = 0; int i = 0;
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
                        if (monitorValue.Values.Count == 8759) hourly = 1;
                        if (benMAPPollutant.Metrics != null && benMAPPollutant.Metrics.Count > 0)
                        {
                            foreach (Metric m in benMAPPollutant.Metrics)
                            {
                                if (m is FixedWindowMetric)
                                {
                                    fixedWindowMetric = (FixedWindowMetric)m;
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
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values[monitorValue.Values.Count / 2]); break;
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
                                        dicHourlyValue = new Dictionary<int, List<float>>();
                                        i = 0;
                                        while (i < 365)
                                        {
                                            lstTemp = monitorValue.Values.GetRange(i * 24 + fixedWindowMetric.StartHour, fixedWindowMetric.EndHour - fixedWindowMetric.StartHour);
                                            lstTemp.Remove(Convert.ToSingle(float.MinValue));
                                            if (lstTemp != null && lstTemp.Count > 0)
                                                dicHourlyValue.Add(i, lstTemp); i++;
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
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p => p).Median()); break;
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
                                    }

                                }
                                else if (m is MovingWindowMetric)
                                {
                                    movingWindowMetric = (MovingWindowMetric)m;
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
                                                    monitorValue.dicMetricValues.Add(m.MetricName, monitorValue.Values[monitorValue.Values.Count / 2]); break;
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
                                        dicHourlyValue = new Dictionary<int, List<float>>();
                                        i = 0;

                                        while (i < 365)
                                        {
                                            lstTemp = monitorValue.Values.GetRange(i * 24, 24);
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
                                                monitorValue.dicMetricValues.Add(m.MetricName, lstTemp.OrderBy(p => p).Median()); break;
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
                                    }
                                }
                                else if (m is CustomerMetric)
                                {
                                }
                                else
                                {

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
                                    if (monitorValue.dicMetricValues.Keys.Contains(seasonalMetric.Metric.MetricName))
                                    {
                                        monitorValue.dicMetricValues.Add(seasonalMetric.SeasonalMetricName, monitorValue.dicMetricValues[seasonalMetric.Metric.MetricName]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public static double GetDistanceFrom2Point(Point start, Point end)
        {
            return 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((start.Y / 180 * Math.PI - end.Y / 180 * Math.PI) / 2)), 2) +
             Math.Cos(start.Y / 180 * Math.PI) * Math.Cos(end.Y / 180 * Math.PI) * Math.Pow(Math.Sin((start.X / 180 * Math.PI - end.X / 180 * Math.PI) / 2), 2))) * 6371.000;
        }

        public static float GetDistanceFrom2Point(double X0, double Y0, double X1, double Y1)
        {
            return Convert.ToSingle(2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((Y0 / 180 * Math.PI - Y1 / 180 * Math.PI) / 2)), 2) +
             Math.Cos(Y0 / 180 * Math.PI) * Math.Cos(Y1 / 180 * Math.PI) * Math.Pow(Math.Sin((X0 / 180 * Math.PI - X1 / 180 * Math.PI) / 2), 2))) * 6371.000);
        }

        public static double GetDistanceFromExtent(Coordinate coordinate, IEnvelope env, Point end)
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

        public static void VoronoiPoints(double[] vertices, ref List<double> result)
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        public static List<MonitorValue> GetMonitorData(BenMAPGrid benMAPGrid, BenMAPPollutant benMAPPollutant, MonitorDataLine monitorDataLine)
        {
            try
            {
                int i = 0;
                List<MonitorValue> lstMonitorValues = new List<MonitorValue>();
                List<MonitorValue> lstMonitorValuesProcessed = new List<MonitorValue>();
                string commandText = "";
                MonitorValue mv = new MonitorValue();
                if (monitorDataLine.MonitorDirectType == 0)
                {
                    commandText = string.Format("select a.MonitorEntryID,a.MonitorID,a.YYear,a.MetricID,a.SeasonalMetricID,a.Statistic,a.VValues,b.PollutantID,b.Latitude,b.Longitude,b.MonitorName,b.MonitorDescription from MonitorEntries a,Monitors b,MonitorDataSets c where a.MonitorID=b.MonitorID and b.MonitorDataSetID=c.MonitorDataSetID and b.PollutantID={0} and c.MonitorDataSetID={1} and a.YYear={2} ", benMAPPollutant.PollutantID, monitorDataLine.MonitorDataSetID, monitorDataLine.MonitorLibraryYear); ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    Byte[] blob = null;
                    lstMonitorValues = new List<MonitorValue>();
                    string str = "";
                    string[] strArray = null;
                    while (fbDataReader.Read())
                    {
                        mv = new MonitorValue();
                        blob = fbDataReader[6] as byte[];
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
                } return lstMonitorValues;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }


    }
}