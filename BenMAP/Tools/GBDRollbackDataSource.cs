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

        public static DataSet GetRegionCountryList()
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText =
                "select r.REGIONID, r.REGIONNAME, c.COUNTRYID, c.COUNTRYNAME, cp.POPULATION " +
                "from REGIONS r " +
                "INNER JOIN REGIONCOUNTRIES rc on r.REGIONID = rc.REGIONID " +
                "INNER JOIN COUNTRIES c on rc.COUNTRYID = c.COUNTRYID " +
                "INNER JOIN COUNTRYPOPULATIONS cp on c.COUNTRYID = cp.COUNTRYID " +
                "WHERE cp.YEARNUM = 2010 " +
                "ORDER BY r.REGIONNAME, c.COUNTRYNAME ";

            DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
            return ds;
        }
    }
}
