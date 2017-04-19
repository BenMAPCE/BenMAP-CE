using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;

namespace BenMAP
{
    class OutputCommonClass
    {
        public static Dictionary<int, string> getAllIncidenceDataset()
        {
            try
            {
                Dictionary<int, string> dicIncidenceDataset = new Dictionary<int, string>();
                string commandText = "select IncidenceDatasetID,IncidenceDatasetName from IncidenceDatasets where setupId=" + CommonClass.ManageSetup.SetupID;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicIncidenceDataset.Add(Convert.ToInt32(dr["IncidenceDatasetID"]), dr["IncidenceDatasetName"].ToString());
                }

                return dicIncidenceDataset;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllPrevalence()
        {
            try
            {
                Dictionary<int, string> dicPrevalence = new Dictionary<int, string>();
                string commandText = "select IncidenceDatasetID,IncidenceDatasetName from IncidenceDatasets where setupId=" + CommonClass.ManageSetup.SetupID;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicPrevalence.Add(Convert.ToInt32(dr["IncidenceDatasetID"]), dr["IncidenceDatasetName"].ToString());
                }

                return dicPrevalence;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllVariableDatasets()
        {
            try
            {
                Dictionary<int, string> dicVariableDataset = new Dictionary<int, string>();
                string commandText = "select SetupVariableDatasetID,SetupVariableDatasetName from SetupVariableDatasets where setupId=" + CommonClass.ManageSetup.SetupID;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicVariableDataset.Add(Convert.ToInt32(dr["SetupVariableDatasetID"]), dr["SetupVariableDatasetName"].ToString());
                }

                return dicVariableDataset;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string getMetricStastic(int MetricStasticID)
        {
            try
            {
                Dictionary<int, string> dicMetricStastic = new Dictionary<int, string>();
                string returnValue = string.Empty;
                dicMetricStastic.Add(0, "None");
                dicMetricStastic.Add(1, "Mean");
                dicMetricStastic.Add(2, "Median");
                dicMetricStastic.Add(3, "Max");
                dicMetricStastic.Add(4, "Min");
                dicMetricStastic.Add(5, "Sum");
                foreach (int id in dicMetricStastic.Keys)
                {
                    if (id == MetricStasticID)
                    {
                        returnValue = dicMetricStastic[id];
                    }
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        public static Dictionary<int, string> getAllBaselineFunctions()
        {
            try
            {
                Dictionary<int, string> dicBaselineFunction = new Dictionary<int, string>();
                string commandText = "select FunctionalFormID,FunctionalFormText from BaselineFunctionalForms";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicBaselineFunction.Add(Convert.ToInt32(dr["FunctionalFormID"]), dr["FunctionalFormText"].ToString());
                }

                return dicBaselineFunction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllGeographicAreas()
        {
            try
            {
                /*************************************/
                Dictionary<int, string> dicGeographicArea = new Dictionary<int, string>();
                string commandText = string.Format(@"select GeographicAreaID,GeographicAreaName 
from GeographicAreas a
join GRIDDEFINITIONS b on a.GRIDDEFINITIONID = b.GRIDDEFINITIONID
where b.SETUPID={0}", CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicGeographicArea.Add(Convert.ToInt32(dr["GeographicAreaID"]), dr["GeographicAreaName"].ToString());
                }

                return dicGeographicArea;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllValuationFunctions()
        {
            try
            {
                Dictionary<int, string> dicFunction = new Dictionary<int, string>();
                string commandText = "select FunctionalFormID,FunctionalFormText from ValuationFunctionalForms";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicFunction.Add(Convert.ToInt32(dr["FunctionalFormID"]), dr["FunctionalFormText"].ToString());
                }

                return dicFunction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllFunctions()
        {
            try
            {
                Dictionary<int, string> dicFunction = new Dictionary<int, string>();
                string commandText = "select FunctionalFormID,FunctionalFormText from FunctionalForms";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicFunction.Add(Convert.ToInt32(dr["FunctionalFormID"]), dr["FunctionalFormText"].ToString());
                }

                return dicFunction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllPollutants()
        {
            try
            {
                Dictionary<int, string> dicPollutant = new Dictionary<int, string>();
                string commandText = "select PollutantID,PollutantName from Pollutants where SetupID=" + CommonClass.ManageSetup.SetupID;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicPollutant.Add(Convert.ToInt32(dr["PollutantID"]), dr["PollutantName"].ToString());
                }

                return dicPollutant;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllRace()
        {
            try
            {
                Dictionary<int, string> dicRace = new Dictionary<int, string>();
                string commandText = "select RaceID,RaceName from Races";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicRace.Add(Convert.ToInt32(dr["RaceID"]), dr["RaceName"].ToString());
                }

                return dicRace;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Dictionary<int, string> getAllEthnicity()
        {
            try
            {
                Dictionary<int, string> dicEthnicity = new Dictionary<int, string>();
                string commandText = "select  EthnicityID,EthnicityName from Ethnicity ";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicEthnicity.Add(Convert.ToInt32(dr["EthnicityID"]), dr["EthnicityName"].ToString());
                }

                return dicEthnicity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Dictionary<int, string> getAllGender()
        {
            try
            {
                Dictionary<int, string> dicGender = new Dictionary<int, string>();
                string commandText = "select  GenderID,GenderName from Genders ";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicGender.Add(Convert.ToInt32(dr["GenderID"]), dr["GenderName"].ToString());
                }


                return dicGender;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllEndPointGroup()
        {
            try
            {
                Dictionary<int, string> dicEndPointGroup = new Dictionary<int, string>();
                string commandText = "select EndPointGroupID,EndPointGroupName from EndPointGroups";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicEndPointGroup.Add(Convert.ToInt32(dr["EndPointGroupID"]), dr["EndPointGroupName"].ToString());
                }

                return dicEndPointGroup;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getAllEndPoint()
        {
            try
            {
                Dictionary<int, string> dicEndPoint = new Dictionary<int, string>();
                string commandText = "select EndPointID,EndPointName from EndPoints "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicEndPoint.Add(Convert.ToInt32(dr["EndPointID"]), dr["EndPointName"].ToString());
                }
                return dicEndPoint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, string> getMetric()
        {
            try
            {
                Dictionary<int, string> dicMetric = new Dictionary<int, string>();
                string commandText = "select MetricID,MetricName from Metrics "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicMetric.Keys.Contains(Convert.ToInt32(dr["MetricID"])))
                        dicMetric.Add(Convert.ToInt32(dr["MetricID"]), dr["MetricName"].ToString());
                }
                return dicMetric;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        public static Dictionary<int, string> getSeasonalMetric()
        {
            try
            {
                Dictionary<int, string> dicSMetric = new Dictionary<int, string>();
                string commandText = "select SeasonalMetricID,SeasonalMetricName from SeasonalMetrics "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicSMetric.Keys.Contains(Convert.ToInt32(dr["SeasonalMetricID"])))
                        dicSMetric.Add(Convert.ToInt32(dr["SeasonalMetricID"]), dr["SeasonalMetricName"].ToString());
                }
                return dicSMetric;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        public static string getStringFromID(int id, Dictionary<int, string> dic)
        {
            try
            {
                string result = string.Empty;
                foreach (int s in dic.Keys)
                {
                    if (s == id)
                        result = dic[s];
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        public static void SaveCSV(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString().Contains(","))
                    { data += "\"" + dt.Rows[i][j].ToString() + "\""; }
                    else
                    {
                        data += dt.Rows[i][j].ToString();
                    }
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            MessageBox.Show("CSV file saved.", "File saved");
        }
    }
}
