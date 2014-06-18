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
            string commandText = string.Format("select 'World' as REGION, COUNTRYNAME from COUNTRIES");
            DataSet ds = fb.ExecuteDataset(GBDRollbackDataSource.Connection, CommandType.Text, commandText);
            return ds;
        }
    }
}
