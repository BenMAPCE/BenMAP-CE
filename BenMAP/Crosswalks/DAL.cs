using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using DotSpatial.Data;
using ESIL.DBUtility;
using FirebirdSql.Data.FirebirdClient;

namespace BenMAP.Crosswalks
{
    internal class DAL
    {
        private readonly FbConnection _connection;
        private readonly FireBirdHelperBase _fbh;

        public DAL(FbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            _connection = connection;
            _fbh = new ESILFireBirdHelper();
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        private int ExecuteNonQuery(string commandText, FbTransaction transaction = null)
        {
            var result =  transaction != null ? 
                _fbh.ExecuteNonQuery(transaction, CommandType.Text, commandText) : 
                _fbh.ExecuteNonQuery(_connection, CommandType.Text, commandText);

            return result;
        }

        private object ExecuteScalar(string commandText, FbTransaction transaction = null)
        {
            var result = transaction != null ?
                _fbh.ExecuteScalar(transaction, CommandType.Text, commandText) :
                _fbh.ExecuteScalar(_connection, CommandType.Text, commandText);

            return result;
        }

        #region Public methods

        public void DeleteAllCrosswalks()
        {
            using (var tran = _connection.BeginTransaction())
            {
                ExecuteNonQuery("DELETE from GRIDDEFINITIONPERCENTAGES", tran);
                ExecuteNonQuery("DELETE from GRIDDEFINITIONPERCENTAGEENTRIES", tran);
                tran.Commit();
            }
        }

        public void DeleteCrosswalk(int grid1, int grid2)
        {
            /* dpa 1/28/2017 this function is used to clear the contents of the crosswalk tables just for the two selected entries.
             * The two tables of interest are: GRIDDEFINITIONPERCENTAGES and GRIDDEFINITIONPERCENTAGEENTRIES.
             */

            using (var tran = _connection.BeginTransaction())
            {

                string commandText = "";
                int iResult = 0;

                //find the correct percentageid entry for the forward direction crosswalk
                commandText = string.Format("SELECT PERCENTAGEID from GRIDDEFINITIONPERCENTAGES where SOURCEGRIDDEFINITIONID={0} and TARGETGRIDDEFINITIONID={1}", grid1, grid2);
                try
                {
                    iResult = Convert.ToInt32(ExecuteScalar(commandText, tran));
                }
                catch{}

                //remove this percentageid entry
                commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGEENTRIES where PERCENTAGEID={0}", iResult);
                ExecuteNonQuery(commandText, tran);

                //remove all data entries for this percentageid
                commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGES where PERCENTAGEID={0}", iResult);
                ExecuteNonQuery(commandText, tran);


                //find the correct percentageid entry for the backward direction crosswalk
                commandText =
                    string.Format(
                        "SELECT PERCENTAGEID from GRIDDEFINITIONPERCENTAGES where SOURCEGRIDDEFINITIONID={0} and TARGETGRIDDEFINITIONID={1}",
                        grid1, grid2);
                try
                {
                    iResult = Convert.ToInt32(ExecuteScalar(commandText, tran));
                }
                catch{}

                //remove this percentageid entry
                commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGEENTRIES where PERCENTAGEID={0}", iResult);
                ExecuteNonQuery(commandText, tran);

                //remove all data entries for this percentageid
                commandText = string.Format("DELETE from GRIDDEFINITIONPERCENTAGES where PERCENTAGEID={0}", iResult);
                ExecuteNonQuery(commandText, tran);
            }

        }

        public DataTable GetGridDefinitions(int setupId)
        {
            var commandText = string.Format("select GridDefinitionName, GridDefinitionID from GridDefinitions where setupID={0}", setupId);
            var ds = _fbh.ExecuteDataset(_connection, commandText);
            return ds.Tables[0];
        }

        public bool CrosswalkExists(int grid1, int grid2)
        {
            bool forwardExists = false, backwardExists = false;

            var commandText = string.Format("select Count(*) from GridDefinitionPercentages where SOURCEGRIDDEFINITIONID={0} AND TARGETGRIDDEFINITIONID={1}", grid1, grid2);
            var iResult = Convert.ToInt32(ExecuteScalar(commandText));
            if (iResult > 0)
            {
                forwardExists = true;
            }

            commandText = string.Format("select Count(*) from GridDefinitionPercentages where SOURCEGRIDDEFINITIONID={0} AND TARGETGRIDDEFINITIONID={1}", grid2, grid1);
            iResult = Convert.ToInt32(ExecuteScalar(commandText));
            if (iResult > 0)
            {
                backwardExists = true;
            }

            return forwardExists && backwardExists;
        }

        public string GetShapeFilenameForGrid(int gridId)
        {
            var commandText = string.Format("select ShapeFileName from ShapeFileGridDefinitionDetails where GridDefinitionID={0}", gridId);
            return ExecuteScalar(commandText).ToString();
        }

        public string GetGridDefinitionName(int grid)
        {
            var commandText = string.Format("select GRIDDEFINITIONNAME from GRIDDEFINITIONS where GRIDDEFINITIONID={0}", grid);
            return ExecuteScalar(commandText).ToString();
        }

        public void InsertCrosswalks(int grid1, int grid2, IFeatureSet fsInput1, IFeatureSet fsInput2, Dictionary<CrosswalkIndex, CrosswalkRatios> results, CancellationToken ctsToken, IProgress progress)
        {
            using (var tran = _connection.BeginTransaction())
            {

                string commandText = "";
                int PercentageID1, PercentageID2;

                /* first we need to add entries to the griddefinitionpercentages table
                 * get the highest index already in the griddefinitionpercentages table or use 1 for the next percentageID
                 */
                commandText = "select count(*) from GridDefinitionPercentages";
                int iResult = Convert.ToInt32(ExecuteScalar(commandText, tran));
                if (iResult > 0)
                {
                    commandText = "select max(PercentageID) from GridDefinitionPercentages";
                    PercentageID1 = Convert.ToInt32(ExecuteScalar(commandText, tran)) + 1;
                }
                else
                {
                    PercentageID1 = 1;
                }
                PercentageID2 = PercentageID1 + 1;

                commandText =
                    string.Format(
                        "insert into GridDefinitionPercentages(PERCENTAGEID,SOURCEGRIDDEFINITIONID, TARGETGRIDDEFINITIONID) "
                        + "values({0},{1},{2})", PercentageID1, grid1, grid2);
                ExecuteNonQuery(commandText, tran);

                commandText =
                    string.Format(
                        "insert into GridDefinitionPercentages(PERCENTAGEID,SOURCEGRIDDEFINITIONID, TARGETGRIDDEFINITIONID) "
                        + "values({0},{1},{2})", PercentageID2, grid2, grid1);
                ExecuteNonQuery(commandText, tran);

                DataTable attributes1;
                DataTable attributes2;
                int intResult = 0;
                double forward = 0.0;
                double backward = 0.0;
                int i = 0, j = 1;
                float prog = 0;
                var fieldNames = new[] {"COL", "ROW"};
                float step = results.Count / 100;
                foreach (var entry in results)
                {
                    //update the progress bar to show progress writing output to database - only 100 progres steps.
                    i += 1;
                    if (i > step * j)
                    {
                        j += 1;
                        prog = Convert.ToSingle(100 * i / results.Count);
                        progress.OnProgressChanged(string.Format("{0} of {1} written to database.", i, results.Count),
                            prog);
                    }
                    //get the column and row attributes and forward backward results
                    attributes1 = fsInput1.GetAttributes(entry.Key.FeatureId1, 1, fieldNames);
                    attributes2 = fsInput2.GetAttributes(entry.Key.FeatureId2, 1, fieldNames);
                    forward = entry.Value.ForwardRatio;
                    backward = entry.Value.BackwardRatio;

                    //write the entries to the firebird database

                    if (forward > 0.0001)
                    {
                        forward = Math.Round(forward, 4);
                            //note - rounding doesn't seem to help. The fb database still adds noise.
                        commandText =
                            string.Format(
                                "insert into GridDefinitionPercentageEntries(PERCENTAGEID, SOURCECOLUMN, SOURCEROW, TARGETCOLUMN, TARGETROW, PERCENTAGE,NORMALIZATIONSTATE) values({0},{1},{2},{3},{4},{5},{6})",
                                PercentageID1, attributes1.Rows[0]["COL"], attributes1.Rows[0]["ROW"],
                                attributes2.Rows[0]["COL"], attributes2.Rows[0]["ROW"], forward, 0);

                        ctsToken.ThrowIfCancellationRequested();
                        intResult = ExecuteNonQuery(commandText, tran);
                    }

                    if (entry.Value.BackwardRatio > 0.0001)
                    {
                        backward = Math.Round(backward, 4);
                        commandText =
                            string.Format(
                                "insert into GridDefinitionPercentageEntries(PERCENTAGEID, SOURCECOLUMN, SOURCEROW, TARGETCOLUMN, TARGETROW, PERCENTAGE,NORMALIZATIONSTATE) values({0},{1},{2},{3},{4},{5},{6})",
                                PercentageID2, attributes2.Rows[0]["COL"], attributes2.Rows[0]["ROW"],
                                attributes1.Rows[0]["COL"], attributes1.Rows[0]["ROW"], backward, 0);

                        ctsToken.ThrowIfCancellationRequested();
                        intResult = ExecuteNonQuery(commandText, tran);
                    }
                }

                ctsToken.ThrowIfCancellationRequested();
                tran.Commit();
            }
        }

        #endregion
    }
}