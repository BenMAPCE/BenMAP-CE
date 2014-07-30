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
                    if (!str.Contains(":"))
                        str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
                    _connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);
                }
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public static DataSet GetRegionCountryList(int year)
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

            DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
            return ds;
        }

        public static double GetIncidenceRate(string countryid)
        {
            double incidenceRate = 0;

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

        public static void GetPollutantBeta(int pollutantid, out double beta, out double se)
        {
            beta = 0;
            se = 0;

            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText =
                "select p.beta, p.se " +
                "from pollutants p " +
                "where p.pollutantid = " + pollutantid.ToString();               

            DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                { 
                    if (ds.Tables[0].Rows[0]["beta"] != DBNull.Value)
                    {
                        string strBeta = ds.Tables[0].Rows[0]["beta"].ToString();
                        Double.TryParse(strBeta, out beta);
                    }     
           
                    if (ds.Tables[0].Rows[0]["se"] != DBNull.Value)
                    {
                        string strSe = ds.Tables[0].Rows[0]["se"].ToString();
                        Double.TryParse(strSe, out se);
                    }  
                }         
            }

        }


        public static DataTable GetCountryConcs(string countryid, int pollutantid, int year)
        {
            DataTable dt = null;

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

    }
}
