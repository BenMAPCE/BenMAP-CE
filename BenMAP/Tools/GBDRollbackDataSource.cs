using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using System.Configuration;
using System.Windows.Forms;
using System.Data;

namespace BenMAP
{
    class GBDRollbackDataSource
    {

        private static FbConnection _connection;

        public static FbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionStringGBD"];
                    string str = settings.ConnectionString;
                    //if (!str.Contains(":"))
                    //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
                    str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                    
                    _connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);
                }
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public static DataSet GetStandardList()
        {
            DataSet ds = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                char micrograms = '\u00B5';
                char super3 = '\u00B3';

                string commandText =
                    "SELECT a.STD_ID, a.POLLUTANT, a.UNITS, a.EXPOSURE_DURATION, a.SAMP_PRD_DAYS, a.STD_GROUP, a.CONC_LIMIT, " +
                    "a.STD_GROUP || ' (' || a.EXPOSURE_DURATION || ')' as STANDARD_NAME " +
                    "FROM STANDARDS a " +
                    "WHERE a.POLLUTANT = 'PM2.5' " +
                    "ORDER BY a.STD_GROUP, a.EXPOSURE_DURATION";

                ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string val = dr["STANDARD_NAME"].ToString();
                            dr["STANDARD_NAME"] = val + " (" + dr["CONC_LIMIT"].ToString() + micrograms.ToString() + "g/m" + super3.ToString() + ")";
                        }
                    }
                }

                return ds;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return ds;
            }
        }

        public static double GetStandardValue(int standardId)
        {
            double standard = 0;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText =
                    "select conc_limit " +
                    "from standards " +
                    "where std_id = " + standardId;

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["conc_limit"] != DBNull.Value)
                        {
                            string val = ds.Tables[0].Rows[0]["conc_limit"].ToString();
                            Double.TryParse(val, out standard);
                        }
                    }
                }

                return standard;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return standard;
            }
        }

        public static DataSet GetRegionCountryList(int year)
        {
            //Some countries are missing either incidence or air quality data  
            //Currently, countries missing either incidence or air quality data have year as 0 in COUNTRYPOPULATIONS table.
            //They will not appear in tvRegions listbox. 
            DataSet ds = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText =
                    "select r.REGIONID, r.REGIONNAME, c.COUNTRYID, c.COUNTRYNAME, cp.POPULATION " +
                    "from REGIONS r " +
                    "INNER JOIN REGIONCOUNTRIES rc on r.REGIONID = rc.REGIONID " +
                    "INNER JOIN COUNTRIES c on rc.COUNTRYID = c.COUNTRYID " +
                    "INNER JOIN COUNTRYPOPULATIONS cp on c.COUNTRYID = cp.COUNTRYID " +
                    "WHERE cp.YEARNUM = " + year + " " +
                    "ORDER BY r.REGIONNAME, c.COUNTRYNAME ";

                ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                return ds;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return ds;
            }
        }



        public static double GetIncidenceRate(string countryid)
        {
            double incidenceRate = 0;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText =
                    "select ir.incidencerate " +
                    "from incidencerates ir " +
                    "inner join countries c " +
                    "on c.COUNTRYNUM = ir.COUNTRYNUM " +
                    "where c.COUNTRYID = '" + countryid + "'";

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["incidencerate"] != DBNull.Value)
                        {
                            string val = ds.Tables[0].Rows[0]["incidencerate"].ToString();
                            Double.TryParse(val, out incidenceRate);
                        }
                    }
                }

                return incidenceRate;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return incidenceRate;
            }
        }

        internal static DataTable GetVSLValue(int vslId)
        {
            DataTable dt = null;

            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //string commandText =
                //    "SELECT reg.REGIONNAME as REGION, cts.COUNTRYNAME as COUNTRY, CAST(vv1.VVALUE as integer) "
                //    + "as OECD, CAST(vv2.VVALUE as integer) as EPA "
                //    + "FROM REGIONS reg "
                //    + "INNER JOIN REGIONCOUNTRIES rcs ON reg.REGIONID = rcs.REGIONID "
                //    + "INNER JOIN COUNTRIES cts ON rcs.COUNTRYID = cts.COUNTRYID "
                //    + "INNER JOIN VSLVALUE vv1 ON cts.COUNTRYID = vv1.COUNTRYID "
                //    + "INNER JOIN VSLVALUE vv2 ON cts.COUNTRYID = vv2.COUNTRYID "
                //    + "WHERE vv1.VSLID = 1 AND vv2.VSLID = 2 "
                //    + "ORDER BY 1, 2;";
                string commandText =
                    "SELECT cts.COUNTRYNAME as Country, CAST(vv1.VVALUE as integer) "
                    + "as \"VSL (USD PPP)\" "
                    + "FROM COUNTRIES cts "
                    + "INNER JOIN VSLVALUE vv1 ON cts.COUNTRYID = vv1.COUNTRYID "
                    + "WHERE vv1.VSLID = " + vslId + " "
                    + "ORDER BY 1, 2;";
                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new System.ApplicationException("Please make sure your database has VSL data.");
            }
        }

        public static void GetPollutantBeta(int functionid, out double beta, out double se)
        {
            beta = 0;
            se = 0;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText =
                    "select distinct bc.BETAMEAN, bc.BETASE " +
                    "from functions f " +
                    "inner join betacoefficients bc on bc.FUNCTIONID = f.FUNCTIONID " +
                    "where f.FUNCTIONID = " + functionid;

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["betamean"] != DBNull.Value)
                        {
                            string strBeta = ds.Tables[0].Rows[0]["betamean"].ToString();
                            Double.TryParse(strBeta, out beta);
                        }

                        if (ds.Tables[0].Rows[0]["betase"] != DBNull.Value)
                        {
                            string strSe = ds.Tables[0].Rows[0]["betase"].ToString();
                            Double.TryParse(strSe, out se);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        public static DataTable GetCountryConcs(string countryid, int pollutantid, int year)
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText =
                    "select r.REGIONID, r.REGIONNAME, c.COUNTRYID, c.COUNTRYNAME, pv.CONCENTRATION, pop.POPESTIMATE " +
                    "from REGIONS r " +
                    "INNER JOIN REGIONCOUNTRIES rc on r.REGIONID = rc.REGIONID " +
                    "INNER JOIN COUNTRIES c on rc.COUNTRYID = c.COUNTRYID " +
                    "inner join COUNTRYCOORDINATES cc " +
                    "on c.COUNTRYID = cc.COUNTRYID " +
                    "inner join POLLUTANTVALUES pv " +
                    "on cc.COORDID = pv.COORDID " +
                    "inner join POPULATION pop " +
                    "on pv.COORDID = pop.COORDID " +
                    "and pv.YEARNUM = pop.YEARNUM " +
                    "where c.COUNTRYID = '" + countryid + "' " +
                    "and pv.POLLUTANTID = " + pollutantid + " " +
                    "and pop.YEARNUM = " + year;

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        // Get concentration, incidence, population for each age/ gender/ endpoint combo per grid cell
        // Not in use any more since July 2017 
        public static DataTable GetGBDDataPerGridCell(int functionID, string countryID, int pollutantID, int coordID)
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                /*
                                string commandText =
                                "select cc.COORDID, r.REGIONID, r.REGIONNAME, c.COUNTRYNUM,  " +
                                "c.COUNTRYID, c.COUNTRYNAME, endpt.ENDPOINTNAME, age.AGERANGENAME,  " +
                                "gen.GENDERNAME, pv.CONCENTRATION, pop.POPESTIMATE, inc.INCIDENCERATE " +
                                "from REGIONS r " +
                                "inner join REGIONCOUNTRIES rc on r.REGIONID = rc.REGIONID " +
                                "inner join COUNTRIES c on rc.COUNTRYID = c.COUNTRYID " +
                                "inner join COUNTRYCOORDINATES cc on c.COUNTRYID = cc.COUNTRYID " +
                                "inner join POLLUTANTVALUES pv on cc.COORDID = pv.COORDID " +
                                "inner join POPULATION pop on pv.COORDID = pop.COORDID " +
                                "inner join GENDERS gen on gen.GENDERID = pop.GENDERID " +
                                "inner join AGERANGES age on age.AGERANGEID = pop.AGERANGEID " +
                                "inner join INCIDENCERATES inc on inc.AGERANGEID = pop.AGERANGEID " +
                                    "and inc.GENDERID = pop.GENDERID and inc.COUNTRYID = c.COUNTRYID " +
                                "inner join ENDPOINTS endpt on endpt.ENDPOINTID = inc.ENDPOINTID " +
                                "inner join BETACOEFFICIENTS betas on betas.ENDPOINTID = endpt.ENDPOINTID " +
                                "inner join FUNCTIONS fun on fun.FUNCTIONID = betas.FUNCTIONID " +
                                "where fun.FUNCTIONID = " + functionID + " and c.COUNTRYID = '" + countryID + "' and pv.POLLUTANTID = " + pollutantID + " " +
                                    "and cc.COORDID = " + coordID + " and pop.YEARNUM = '2015' and pv.YEARNUM = '2013' ";
                */
                // Temorarily using a SQL query that aggregates all the endpoints, genders, and age ranges together to improve performance
                // IEc 2017-04-12: Modifying query to remove age <30 to meet Krewski requirement
                string commandText = @"
with p as (SELECT a.coordid, sum(b.POPESTIMATE) POPESTIMATE
FROM COUNTRYCOORDINATES a
join POPULATION b on a.COORDID = b.COORDID
inner join AGERANGES age on age.AGERANGEID = b.AGERANGEID 
where a.COUNTRYID = '" + countryID + @"'
and a.COORDID = " + coordID + @"
and age.STARTAGE >= 30
group by 1
) 
                select 
                cc.COORDID
                , r.REGIONID
                , r.REGIONNAME
                , c.COUNTRYNUM
                , c.COUNTRYID
                , c.COUNTRYNAME
                , 'Mortality, All' ENDPOINTNAME -- endpt.ENDPOINTNAME
                , 'ALL AGE' AGERANGENAME -- age.AGERANGENAME
                , 'ALL GENDER' GENDERNAME -- gen.GENDERNAME
                , pv.CONCENTRATION
                , p.POPESTIMATE
                , sum(pop.POPESTIMATE * inc.INCIDENCERATE ) INCIDENCERATE
                from REGIONS r 
                inner join REGIONCOUNTRIES rc on r.REGIONID = rc.REGIONID 
                inner join COUNTRIES c on rc.COUNTRYID = c.COUNTRYID 
                inner join COUNTRYCOORDINATES cc on c.COUNTRYID = cc.COUNTRYID 
                inner join POLLUTANTVALUES pv on cc.COORDID = pv.COORDID 
                inner join POPULATION pop on pv.COORDID = pop.COORDID 
                inner join GENDERS gen on gen.GENDERID = pop.GENDERID 
                inner join AGERANGES age on age.AGERANGEID = pop.AGERANGEID 
                inner join INCIDENCERATES inc on inc.AGERANGEID = pop.AGERANGEID 
                    and inc.GENDERID = pop.GENDERID and inc.COUNTRYID = c.COUNTRYID 
                inner join ENDPOINTS endpt on endpt.ENDPOINTID = inc.ENDPOINTID 
                inner join BETACOEFFICIENTS betas on betas.ENDPOINTID = endpt.ENDPOINTID 
                inner join FUNCTIONS fun on fun.FUNCTIONID = betas.FUNCTIONID 
                inner join p on cc.COORDID = p.COORDID
                where fun.FUNCTIONID = " + functionID + @" and c.COUNTRYID = '" + countryID + @"' and pv.POLLUTANTID = " + pollutantID + @"
                    and pop.YEARNUM = 2015 and pv.YEARNUM = 2015
                    and cc.COORDID = " + coordID + @"
                    and age.STARTAGE >= 30
                    group by 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11";

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }


        internal static DataTable GetVSLlist()
        {
            DataTable dt = null;

            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText =
                    "SELECT VSLID, iif(a.GBDDEFAULT = 'T', ('GBD Default (' || VSLNAME || ')'), 'GBD Alternative (' || VSLNAME || ')')  AS VSLSTANDS FROM VSL a";
                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new System.ApplicationException("Please make sure your database has VSL data.");
            }
        }

        // Get concentration, per grid cell for current country and active year.
        public static DataTable GetGBDConcPerGridCell(string countryID, int pollutantID, int year)
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "SELECT c.REGIONID, a.COUNTRYID, a.COORDID, b.YEARNUM, b.POLLUTANTID, b.CONCENTRATION "
                                       + "FROM COUNTRYCOORDINATES a "
                                       + "INNER JOIN POLLUTANTVALUES b ON a.COORDID = b.COORDID "
                                       + "INNER JOIN REGIONCOUNTRIES c ON a.COUNTRYID = c.COUNTRYID "
                                       + "WHERE a.COUNTRYID = '" + countryID + "' and b.POLLUTANTID = " + pollutantID + " and b.YEARNUM = " + year + ";";

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        public static DataTable GetAgeTable()
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                string commandText = @"SELECT a.AGERANGEID, a.AGERANGENAME FROM AGERANGES a;";

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }
        public static DataTable GetGenderTable()
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                string commandText = @"SELECT a.GENDERID, a.GENDERNAME FROM GENDERS a;";

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        public static DataTable GetLifeTable()
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                string commandText = @"SELECT a.AGERANGEID, a.PROBOFDEATH, a.LIFEEXPECT FROM LIFETABLE a;";

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        public static DataTable GetFunctionTable(int functionId)
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                string commandText = @"SELECT a.FUNCTIONID, a.ENDPOINTID, a.GENDERID, a.AGERANGEID, a.BETAMEAN, a.BETASE, a.A, a.B, a.FUNCTIONTEXT, a.C 
FROM BETACOEFFICIENTS a WHERE a.FUNCTIONID = " + functionId;

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        // Get population data for current country and active year.
        public static DataTable GetCountryPopulation(string countryID, int year, int functionId)
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = new DataSet();
                //Only affected population is included.
                string commandText = "with ag AS (SELECT distinct fun.AGERANGEID, fun.GENDERID FROM BETACOEFFICIENTS fun WHERE fun.FUNCTIONID = " + functionId + ") "
                                     + "SELECT a.YEARNUM, a.COORDID, a.GENDERID, a.AGERANGEID, a.POPESTIMATE "
                                     + "FROM POPULATION a INNER JOIN COUNTRYCOORDINATES b " 
                                     + "ON a.COORDID=b.COORDID "
                                     + "INNER JOIN  ag ON a.AGERANGEID=ag.AGERANGEID AND a.GENDERID=ag.GENDERID "
                                     + "WHERE b.COUNTRYID = '" + countryID + "' AND a.YEARNUM = " + year + ";";

                ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        // Get incidence data for current country and selcted fucntion.
        public static DataTable GetCountryIncidence(string countryID, int functionId)
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //Different countries may use different incidence dataasets. This info is stored in function (betacoefficient) table
                //For SCHIF functions, either non-accidental and reattributed incidence rate are used depends on country id.
                string commandText = "";
                if (functionId == 2) //SCHIF
                {
                    commandText = @"SELECT a.COUNTRYID, a.GENDERID, a.AGERANGEID, a.ENDPOINTID, a.INCIDENCERATE FROM INCIDENCERATES a
INNER JOIN COUNTRIES b on a.COUNTRYID=b.COUNTRYID
WHERE ((a.ENDPOINTID = 6 and b.REATTRIBUTED='F') or (a.ENDPOINTID = 7 and b.REATTRIBUTED='T')) and a.COUNTRYID =  '" + countryID + "';";
                }
                else
                {
                    commandText = @"SELECT a.COUNTRYID, a.GENDERID, a.AGERANGEID, a.ENDPOINTID, a.INCIDENCERATE FROM INCIDENCERATES a 
                                       WHERE a.COUNTRYID = '" + countryID + "';"; 
                } 

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                    else
                    {
                        //This is just for debugging purpose as if testing on old database endpointId 6 does not exist. 
                        //                        commandText = @"SELECT a.COUNTRYID, a.GENDERID, a.AGERANGEID, 6 as ENDPOINTID, Sum(a.INCIDENCERATE) as INCIDENCERATE 
                        //FROM INCIDENCERATES a 
                        //WHERE a.COUNTRYID = '" + countryID + 
                        // "'GROUP BY a.COUNTRYID, a.GENDERID, a.AGERANGEID";
                        //                        ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                        //                        dt = ds.Tables[0].Copy();
                        //MessageBox.Show("No incidence data available for selected function.");
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        // Get region id by country ID
        public static void GetRegionCountryName(string countryID, ref int regionId, ref string regionName, ref string countryName)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = @"SELECT a.REGIONID, c.REGIONNAME, b.COUNTRYNAME FROM REGIONCOUNTRIES a 
                                        INNER JOIN COUNTRIES b ON a.COUNTRYID=b.COUNTRYID
                                        INNER JOIN REGIONS c ON a.REGIONID=c.REGIONID 
                                       WHERE a.COUNTRYID = '" + countryID + "';";
                FbDataReader dr = fb.ExecuteReader(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                while (dr.Read())
                {
                    regionId = Convert.ToInt16(dr["REGIONID"]);
                    regionName = dr["REGIONNAME"].ToString();
                    countryName = dr["COUNTRYNAME"].ToString();

                }
                dr.Dispose();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
                //return ds;
                throw new System.ApplicationException("Error Getting Region Country Name.");
            }
         }
            
        // Get concentration, incidence, population for country 
        public static DataTable GetGBDDataPerCountry(int functionID, string countryID, int pollutantID)
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                // IEc 2017-04-12: Modifying query to remove age <30 to meet Krewski requirement
                string commandText = @"
with p as (SELECT a.coordid, sum(b.POPESTIMATE) POPESTIMATE
FROM COUNTRYCOORDINATES a
inner join POPULATION b on a.COORDID = b.COORDID
inner join AGERANGES age on age.AGERANGEID = b.AGERANGEID 
where a.COUNTRYID = '" + countryID + @"'
and age.STARTAGE >= 30
group by 1
) 
                select 
                cc.COORDID
                , r.REGIONID
                , r.REGIONNAME
                , c.COUNTRYNUM
                , c.COUNTRYID
                , c.COUNTRYNAME
                , 'Mortality, All' ENDPOINTNAME -- endpt.ENDPOINTNAME
                , 'ALL AGE' AGERANGENAME -- age.AGERANGENAME
                , 'ALL GENDER' GENDERNAME -- gen.GENDERNAME
                , pv.CONCENTRATION
                , p.POPESTIMATE
                , sum(pop.POPESTIMATE * inc.INCIDENCERATE ) INCIDENCERATE
                from REGIONS r 
                inner join REGIONCOUNTRIES rc on r.REGIONID = rc.REGIONID 
                inner join COUNTRIES c on rc.COUNTRYID = c.COUNTRYID 
                inner join COUNTRYCOORDINATES cc on c.COUNTRYID = cc.COUNTRYID 
                inner join POLLUTANTVALUES pv on cc.COORDID = pv.COORDID 
                inner join POPULATION pop on pv.COORDID = pop.COORDID 
                inner join GENDERS gen on gen.GENDERID = pop.GENDERID 
                inner join AGERANGES age on age.AGERANGEID = pop.AGERANGEID 
                inner join INCIDENCERATES inc on inc.AGERANGEID = pop.AGERANGEID 
                    and inc.GENDERID = pop.GENDERID and inc.COUNTRYID = c.COUNTRYID 
                inner join ENDPOINTS endpt on endpt.ENDPOINTID = inc.ENDPOINTID 
                inner join BETACOEFFICIENTS betas on betas.ENDPOINTID = endpt.ENDPOINTID 
                inner join FUNCTIONS fun on fun.FUNCTIONID = betas.FUNCTIONID 
                inner join p on cc.COORDID = p.COORDID
                where fun.FUNCTIONID = " + functionID + @" and c.COUNTRYID = '" + countryID + @"' and pv.POLLUTANTID = " + pollutantID + @"
                    and pop.YEARNUM = 2015 and pv.YEARNUM = 2015
                    and age.STARTAGE >= 30
                    group by 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11";

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }


        // Get all available functions for dropdown
        public static DataTable GetGBDFunctions()
        {
            DataSet ds = null;
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "select FUNCTIONID, FUNCTIONNAME from FUNCTIONS WHERE FUNCTIONID != 1"; //FunctionId = 1 is Krewski function

                ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        // Get all coordinates for specified country
        public static DataTable GetCountryCoords(string countryID)
        {
            DataSet ds = null;
            DataTable dt = null;

            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "select coordid from COUNTRYCOORDINATES where countryid = '" + countryID + "' order by coordid";

                ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        internal static void GetRegionCountryVsl(string countryId, int VslId, ref double countryVsl)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = @"SELECT a.VVALUE FROM VSLVALUE a WHERE a.COUNTRYID = '" + countryId + "' AND a.VSLID= " + VslId +";";
                FbDataReader dr = fb.ExecuteReader(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
                while (dr.Read())
                {
                    countryVsl = Convert.ToDouble(dr["VVALUE"]);
                }
                dr.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new System.ApplicationException("Please make sure your database has VSL data.");
            }
        }

        internal static DataTable GetFunctionAgeList(int functionID)
        {
            DataSet ds = null;
            DataTable dt = null;

            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "SELECT distinct a.AGERANGEID FROM BETACOEFFICIENTS a WHERE a.FUNCTIONID = " + functionID + ";";

                ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }
        }

        internal static DataTable GetCountrySumIncidence(string countryId, int functionId) //One record per country
        {
            DataTable dt = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //For SCHIF functions, either non-accidental and reattributed incidence rate are used depends on country id.
                string commandText = "";
                if (functionId == 2) //SCHIF
                {
                    commandText = @"SELECT a.COUNTRYID, a.GENDERID, a.AGERANGEID, 0 AS ENDPOINTID, SUM(a.INCIDENCERATE) AS INCIDENCERATE FROM INCIDENCERATES a
INNER JOIN COUNTRIES b on a.COUNTRYID=b.COUNTRYID
WHERE ((a.ENDPOINTID = 6 and b.REATTRIBUTED='F') or (a.ENDPOINTID = 7 and b.REATTRIBUTED='T')) and a.COUNTRYID =  '" + countryId + @"' 
GROUP BY a.COUNTRYID, a.GENDERID, a.AGERANGEID; ";
                }
                else
                {
                    commandText = @"SELECT a.COUNTRYID, a.GENDERID, a.AGERANGEID, 0 AS ENDPOINTID, SUM(a.INCIDENCERATE) AS INCIDENCERATE FROM INCIDENCERATES a 
WHERE a.COUNTRYID = '" + countryId + @"' 
GROUP BY a.COUNTRYID, a.GENDERID, a.AGERANGEID; ";
                }

                DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dt = ds.Tables[0].Copy();
                    }
                    else
                    {
                        //Incidence data not found.
                        return dt;
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return dt;
            }

        }
    }
}
