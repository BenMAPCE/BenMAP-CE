using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using ESIL.DBUtility;


namespace BenMAP
{
    class CopyMonitors
    {
        public int Copy(int oldID, int SetupID, string VariableDataSetName)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                string commandText = string.Empty;
                int maxID = 0;
                int minID = 0;
                int newID;
                object rVal = null;
                
                //check and see if name is used
                commandText = string.Format("Select MONITORDATASETNAME from MONITORDATASETS WHERE MONITORDATASETNAME = '{0}'",VariableDataSetName);
                rVal = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if(rVal != null)
                {
                    MessageBox.Show("Dataset Name is already used.  Please select a new name.");
                    return 0;
                }
                
                string msg = "Copy Monitor Data Set";
                DialogResult result = MessageBox.Show(msg, "Confirm Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return 0;

                // display wait form (as it will take a long time to do the copy)


                //getting a new dataset
                commandText = commandText = "select max(MonitorDataSetID) from MonitorDataSets";
                newID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

                //the 'F' is for the LOCKED column in MonitorDataSets.  This is being added and is not a predefined.
                commandText = string.Format("insert into MonitorDataSets(MonitorDataSetID, SetupID, MonitorDataSetName, Locked) "
                    + "values ({0},{1},'{2}', 'F')", newID, SetupID, VariableDataSetName);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                commandText = "select max(MonitorID) from MONITORS";
                maxID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                commandText = string.Format("select min(MonitorID) from MONITORS where MONITORDATASETID = {0}", oldID);
                minID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //inserting - copying the locked data to the new data set
                commandText = string.Format("insert into Monitors(Monitorid, Monitordatasetid, Pollutantid, Latitude, Longitude, Monitorname, Monitordescription, Metadataid) " +
                              "SELECT Monitorid + ({0} - {1}) + 1, " +
                              "{2}, Pollutantid, Latitude, Longitude, Monitorname,Monitordescription, " +
                              "Metadataid FROM Monitors WHERE MONITORDATASETID = {3}", maxID, minID, newID, oldID );
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                commandText = string.Format("insert into MonitorEntries (MONITORENTRYID,MONITORID,YYEAR,METRICID,SEASONALMETRICID,STATISTIC,VVALUES) " +
                                            "SELECT (SELECT MAX(MONITORENTRYID) FROM MONITORENTRIES)+OE.MONITORENTRYID - (SELECT MIN(MONITORENTRYID) FROM MONITORENTRIES A " +
                                            "INNER JOIN MONITORS B " +
                                            "ON A.MONITORID = B.MONITORID " +
                                            "WHERE B.MONITORDATASETID = {0}) +1 AS NEWMONITORENTRYID, " +
                                            "NM.MONITORID AS NEWMONITORID, OE.YYEAR,OE.METRICID, OE.SEASONALMETRICID, OE.STATISTIC, OE.VVALUES FROM MONITORENTRIES OE " +
                                            "INNER JOIN MONITORS OM " +
                                            "ON OE.MONITORID = OM.MONITORID " +
                                            "INNER JOIN MONITORS NM " +
                                            "ON NM.MONITORID = OM.Monitorid + ({2} - {3}) + 1 " +
                                            "WHERE OM.MONITORDATASETID = {0} " +
                                            "AND NM.MONITORDATASETID = {1}", oldID, newID, maxID, minID);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                
                return newID;

            }
            catch (Exception ex)
            {
               Logger.LogError(ex.Message);
            }
            return 0;
        }
    }
}
