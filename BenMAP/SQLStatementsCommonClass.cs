// ***********************************************************************
// Assembly         : BenMAP
// Author           : boberkirsch
// Created          : 04-09-2014
//
// Last Modified By : boberkirsch
// Last Modified On : 04-09-2014
// ***********************************************************************
// <copyright file="SQLStatementsCommonClass.cs" company="RTI International">
//     RTI International. All rights reserved.
// </copyright>
// <summary>
//  This class is for all and any sql commands that relate to commands doing inserts, updates, deletes, 
//  or any other type of quering against the firebird BenMap database.
//  </summary>
// ***********************************************************************
using System;
using System.Data;
using ESIL.DBUtility;
using FirebirdSql.Data.FirebirdClient;
/// <summary>
/// The BenMAP namespace.
/// </summary>
namespace BenMAP
{
    /// <summary>
    /// Class SQL Statements Common Class.
    /// 
    /// </summary>
    class SQLStatementsCommonClass
    {

        /// <summary>
        /// Inserts the metadata into the Metadata Table.
        /// </summary>
        /// <param name="metadataObj">The metadata object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        /// <exception cref="System.Exception"></exception>
        public static bool insertMetadata(MetadataClassObj metadataObj)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();

            int rtv = 0;
            bool bPassed = false;
            string commandText = string.Empty;
            try
            {
                        commandText = string.Format("INSERT INTO METADATAINFORMATION " +
                                        "(SETUPID, DATASETID, DATASETTYPEID, FILENAME, " +
                                        "EXTENSION, DATAREFERENCE, FILEDATE, IMPORTDATE, DESCRIPTION, " +
                                        "PROJECTION, GEONAME, DATUMNAME, DATUMTYPE, SPHEROIDNAME, " +
                                        "MERIDIANNAME, UNITNAME, PROJ4STRING, NUMBEROFFEATURES) " +
                                        "VALUES('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}', '{8}', '{9}', " +
                                        "'{10}', '{11}', '{12}', '{13}', '{14}','{15}', '{16}', '{17}')",
                                        metadataObj.SetupId, metadataObj.DatasetId, metadataObj.DatasetTypeId, metadataObj.FileName,
                                        metadataObj.Extension, metadataObj.DataReference, metadataObj.FileDate, metadataObj.ImportDate,
                                        metadataObj.Description, metadataObj.Projection, metadataObj.GeoName, metadataObj.DatumName,
                                        metadataObj.DatumType, metadataObj.SpheroidName, metadataObj.MeridianName, metadataObj.UnitName,
                                        metadataObj.Proj4String, metadataObj.NumberOfFeatures);
                        rtv = fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        bPassed = true;
            }
            catch (Exception ex)
            {
                throw( new Exception(ex.Message));
            }

            return bPassed;
        }

        public static bool updateMetadata(MetadataClassObj metadataObj)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            bool bPassed = false;
            int rtv = 0;

            string commandText = string.Empty;
            try
            {
                commandText = string.Format("UPDATE METADATAINFORMATION set DATAREFERENCE = '{0}', DESCRIPTION = '{1}' " +
                                            "WHERE DATASETID = {2} AND SETUPID = {3}", 
                                            metadataObj.DataReference, metadataObj.Description, metadataObj.DatasetId, metadataObj.SetupId);
                rtv = fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                bPassed = true;

            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }

            return bPassed;
        }

        /// <summary>
        /// Gets the dataset identifier.
        /// Selects Dataset Id from Datasets table where Dataset Name is the name passed in.
        /// "SELECT DATASETID FROM DATASETS WHERE DATASETNAME = '{0}'"
        /// </summary>
        /// <param name="datasetname">The datasetname.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.Exception"></exception>
        public static int getDatasetID(string datasetname)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            int rtvID;
            string commandText = string.Empty;
            try
            {
                commandText = string.Format("SELECT DATASETID FROM DATASETS WHERE DATASETNAME = '{0}'", datasetname);
                rtvID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
            }
            catch (Exception ex)
            {
                throw(new Exception(ex.Message));
            }

            return rtvID;
        }

        public static MetadataClassObj getMetadata(int datasetID, int setupId)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            //FbDataReader fbDataReader = null;
            MetadataClassObj _metadataObj = new MetadataClassObj();
            DataSet ds = null;

            string commandText = string.Format("SELECT METADATAID, SETUPID, DATASETID, DATASETTYPEID, FILENAME, " +
                      "EXTENSION, DATAREFERENCE, FILEDATE, IMPORTDATE, DESCRIPTION, " +
                      "PROJECTION, GEONAME, DATUMNAME, DATUMTYPE, SPHEROIDNAME, " +
                      "MERIDIANNAME, UNITNAME, PROJ4STRING, NUMBEROFFEATURES " +
                      "FROM METADATAINFORMATION " +
                      "WHERE DATASETID = '{0}' AND SETUPID = '{1}'", datasetID, setupId);

            //fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
            ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                _metadataObj.MetadataId = Convert.ToInt32(dr["METADATAID"]);
                _metadataObj.SetupId = Convert.ToInt32(dr["SETUPID"]);
                _metadataObj.DatasetId = Convert.ToInt32(dr["DATASETID"]);
                _metadataObj.DatasetTypeId = Convert.ToInt32(dr["DATASETTYPEID"]);
                _metadataObj.FileName = dr["FILENAME"].ToString();
                _metadataObj.Extension = dr["EXTENSION"].ToString();
                _metadataObj.DataReference = dr["DATAREFERENCE"].ToString();
                _metadataObj.FileDate = dr["FILEDATE"].ToString();
                _metadataObj.ImportDate = dr["IMPORTDATE"].ToString();
                _metadataObj.Description = dr["DESCRIPTION"].ToString();
                _metadataObj.Projection = dr["PROJECTION"].ToString();
                _metadataObj.GeoName = dr["GEONAME"].ToString();
                _metadataObj.DatumName = dr["DATUMNAME"].ToString();
                _metadataObj.DatumType = dr["DATUMTYPE"].ToString();
                _metadataObj.SpheroidName = dr["SPHEROIDNAME"].ToString();
                _metadataObj.MeridianName = dr["MERIDIANNAME"].ToString();
                _metadataObj.UnitName = dr["UNITNAME"].ToString();
                _metadataObj.Proj4String = dr["PROJ4STRING"].ToString();
                _metadataObj.NumberOfFeatures = dr["NUMBEROFFEATURES"].ToString();

            }
            return _metadataObj;
        }

        public static int selectMaxID(string nameId, string fromTableName)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            int rtvID = -1;
            string commandText = string.Empty;
            try 
	        {
                commandText = string.Format("select max({0}) from {1}", nameId, fromTableName);
                rtvID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
	        }
	        catch (Exception ex)
	        {
		
		        throw(new Exception(ex.Message));
	        }

            return rtvID;
        }
    }
}
