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
where a.COUNTRYID = '" + countryID + @"'
and a.COORDID = " + coordID + @"
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
                    and pop.YEARNUM = 2015 and pv.YEARNUM = 2013
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
join POPULATION b on a.COORDID = b.COORDID
where a.COUNTRYID = '" + countryID + @"'
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
                    and pop.YEARNUM = 2015 and pv.YEARNUM = 2013
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
        public static DataSet GetGBDFunctions()
        {
            DataSet ds = null;
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "select functionid, functionname from functions";

                ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return ds;
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
    }
}
