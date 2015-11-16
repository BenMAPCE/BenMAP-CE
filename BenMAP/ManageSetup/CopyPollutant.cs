using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESIL.DBUtility;

namespace BenMAP{
    class CopyPollutant
    {
        public int Copy(int oldPollutantID, int SetupID, string PollutantName)
        {
            int ID=0;
            int iObsType;
            // first get the maximum pollutant id
            ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();
            
            object newID = fb.ExecuteScalar(CommonClass.Connection, "Select max(pollutantid) from Pollutants");
            ID = int.Parse(newID.ToString());
            ID = ID + 1;    // add 1 to get the new pollutant's id
            // get the observation type
            object newOBS = fb.ExecuteScalar(CommonClass.Connection, "Select ObservationType from Pollutants where PollutantID =" + oldPollutantID.ToString());
            iObsType = int.Parse(newOBS.ToString());

            // now add new pollutant
            string strSQL = "INSERT INTO POLLUTANTS(POLLUTANTID,SETUPID, POLLUTANTNAME, OBSERVATIONTYPE) VALUES("
                + ID.ToString() + ',' + SetupID.ToString() + ",'" + PollutantName + "'," + iObsType.ToString() + ")";
            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType() , strSQL);

            // copy the old pollutant's metrics into the new pollutant
            // does old pollutant have metrics
            int iMetricCount;
            newID = fb.ExecuteScalar(CommonClass.Connection, "Select Count(MetricID) from Metrics where PollutantID=" + oldPollutantID);
            iMetricCount = int.Parse(newID.ToString());

            if (iMetricCount > 0) // old pollutant has metrics, copy them for new pollutant
            {

                // get max and min metric IDs
                int maxMetricID;
                newID = fb.ExecuteScalar(CommonClass.Connection, "Select max(MetricID) from Metrics");
                maxMetricID = int.Parse(newID.ToString());

                int minMetricID;
                newID = fb.ExecuteScalar(CommonClass.Connection, "Select min(MetricID) from Metrics where PollutantID=" + oldPollutantID);
                minMetricID = int.Parse(newID.ToString());

                // insert metric rows from old pollutant
                strSQL = "INSERT INTO METRICS(METRICID, POLLUTANTID, METRICNAME, HOURLYMETRICGENERATION) "
                    +"SELECT " +maxMetricID.ToString() + "+ METRICID + 1 - " + minMetricID.ToString() + 
                ", " +ID.ToString() + ", METRICNAME, HOURLYMETRICGENERATION FROM METRICS WHERE POLLUTANTID=" + oldPollutantID  ;
                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), strSQL);

            }

            return ID;


        }
    }
}
