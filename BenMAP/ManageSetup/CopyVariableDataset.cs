using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using ESIL.DBUtility;

namespace BenMAP
{
    class CopyVariableDataset
    {
        public int Copy(int oldID, int SetupID, string VariableDataSetName)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();

            try
            {
                string commandText = string.Empty;
                int maxID = 0;
                int minID = 0;
                int iNewDataSetID;
                object rVal = null;

                //check and see if name is used
                commandText = string.Format("Select SetupVariableDATASETNAME from SetupVariableDATASETS WHERE SetupVariableDATASETNAME = '{0}'", VariableDataSetName );
                rVal = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

                if (rVal != null)
                {
                    MessageBox.Show("Name is already used.  Please select a new name.");
                    return 0;
                }

                // string msg = string.Format("Save this file associated with {0} and {1} ?", cboPollutant.GetItemText(cboPollutant.SelectedItem), txtYear.Text);
                string msg = "Copy Variable Data Set";
                DialogResult result = MessageBox.Show(msg, "Confirm Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return 0;
                
                //getting the max dataset ID as new dataset id
                commandText = commandText = "select max(SetupVariableDataSetID) from SetupVariableDataSets";
                iNewDataSetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

                // first, create a new variable data set
                //the 'F' is for the LOCKED column in SetupVariableDataSets.  This is being added and is not a predefined.
                commandText = string.Format("insert into SetupVariableDataSets(SetupVariableDATASETID, SETUPID, SETUPVARIABLEDATASETNAME, LOCKED) "
                         + " values ({0},{1},'{2}', 'F')", iNewDataSetID, SetupID, VariableDataSetName);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                // then, fill the setup variables table with copies of records linked to the original variable dataset
                // get max id for all records
                commandText = "select max(SetupVariableID) from SetupVariables ";
                maxID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                // get minimum id (in old dataset)
                commandText = string.Format("select min(SetupVariableID) from SetupVariables "
                                + " where SETUPVARIABLEDATASETID = {0}", oldID);
                minID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

                //inserting - copying the locked data to the new data set
                commandText = string.Format("insert into SetupVariables(SetupVariableID, SetupVariableDataSetID, SetupVariableName, GridDefinitionID, MetaDataID) " +
                              "SELECT SetupVariableID + ({0} - {1}) + 1, " +
                              "{2}, SetupVariableName, GridDefinitionID, MetaDataID " +
                              "FROM SetupVariables WHERE SetupVariableDataSetID = {3}", maxID, minID, iNewDataSetID, oldID);

                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                // now copy the geographic values to the new set
                commandText = string.Format("INSERT INTO SetupGeographicVariables(SetupVariableID, CCOLUMN, \"ROW\", VVALUE) "
                                + "SELECT {0} + 1 + C.SetupVariableID - {1}, CCOLUMN, \"ROW\", VVALUE "
                                + "from SetupGeographicVariables as C INNER JOIN SetupVariables AS P ON C.SetupVariableID = P.SetupVariableID "
                                + "WHERE P.SetupVariableDataSetID = {2} ", maxID, minID, oldID);

                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                // now copy the global values to the new set
                commandText = string.Format("INSERT INTO SetupGlobalVariables(SetupVariableID, VVALUE) "
                                + "SELECT {0} + 1 + C.SetupVariableID - {1}, VVALUE "
                                + "from SetupGlobalVariables as C INNER JOIN SetupVariables AS P ON C.SetupVariableID = P.SetupVariableID "
                                + "WHERE P.SetupVariableDataSetID = {2} ", maxID, minID, oldID);

                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return 0;   
        }
    }
}
