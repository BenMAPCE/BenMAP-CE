using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace BenMAP
{
    public partial class DatabaseExport : FormBase
    {
        bool errorOccur = false;

        public DatabaseExport()
        {
            InitializeComponent();
        }

        private void DatabaseExport_Load(object sender, EventArgs e)
        {
            try
            {
                TreeNode node = new TreeNode();
                node.Text = "Available Setups";
                treDatabase.Nodes.Add(node);

                string commandText = "select SetupID,SetupName from Setups";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                System.Data.DataSet dsSetups = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                int j = 0; int iSetupID = 0; int id = 0; int iTable = 0;
                List<objDatabaseExport> lstdatabase = new List<objDatabaseExport>();
                objDatabaseExport database = new objDatabaseExport();

                for (int iSetup = 0; iSetup < dsSetups.Tables[0].Rows.Count; iSetup++)
                {
                    database = new objDatabaseExport();
                    DataRow dr = dsSetups.Tables[0].Rows[iSetup];
                    database.name = Convert.ToString(dr["SetupName"]);
                    database.tableID = Convert.ToInt16(dr["SetupID"]);
                    database.objType = (enumDatabaseExport)Enum.Parse(typeof(enumDatabaseExport), "Setups");
                    id = database.ID = ++j;
                    database.PID = 0;
                    lstdatabase.Add(database);

                    iSetupID = Convert.ToInt16(dr["SetupID"]);

                    foreach (var table in Enum.GetValues(typeof(enumDatabaseExport)))
                    {

                        string t0 = table.ToString();
                        if (t0 == "Setups") { continue; }
                        string t = table.ToString().Substring(0, table.ToString().Length - 1);
                        string tID = t + "ID";
                        string tName = t + "NAME";

                        commandText = string.Format("select {0},{1} from {2} where setupid={3}", tID, tName, t0, iSetupID);
                        fb = new ESIL.DBUtility.ESILFireBirdHelper();
                        DataSet dsTable = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        if (dsTable.Tables[0].Rows.Count == 0) { continue; }
                        database = new objDatabaseExport();
                        switch (t0)
                        {
                            case "GridDefinitions": database.name = "Grid Definitions"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "Pollutants": database.name = "Pollutant"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "MonitorDatasets": database.name = "Monitor Datasets"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "IncidenceDatasets": database.name = "Incidence/Prevalence Datasets"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "PopulationDatasets": database.name = "Population Datasets"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "CrfunctionDatasets": database.name = "Health Impact Functions"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "SetupvariableDatasets": database.name = "Variable Datasets"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "InflationDatasets": database.name = "Inflation Datasets"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "ValuationfunctionDatasets": database.name = "Valuation Functions"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                            case "IncomegrowthadjDatasets": database.name = "Income Growth Adjustments"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
                        }
                        for (int i = 0; i < dsTable.Tables[0].Rows.Count; i++)
                        {
                            database = new objDatabaseExport();
                            dr = dsTable.Tables[0].Rows[i];
                            database.objType = (enumDatabaseExport)Enum.Parse(typeof(enumDatabaseExport), t0);
                            database.name = Convert.ToString(dr[1]);
                            database.tableID = Convert.ToInt16(dr[0]);
                            database.ID = ++j;
                            database.PID = iTable;
                            lstdatabase.Add(database);
                        }

                    }
                }
                creatTree(lstdatabase, node, 0);
                node.Expand();
                pBarExport.Value = 0;
            }
            catch (Exception ex)
            { }
        }

        public void creatTree(List<objDatabaseExport> lstdatabase, TreeNode tn, int PID)
        {
            try
            {
                var db1 = lstdatabase.Where(p => p.PID == PID).Select(p => p);
                foreach (var db in db1)
                {
                    TreeNode t = new TreeNode();
                    t.Text = db.name;
                    tn.Nodes.Add(t);
                    creatTree(lstdatabase, t, db.ID);
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (nodeName == "")
                { return; }

                errorOccur = false;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string setupid = "";
                string setup = "";
                if (nodeName != "Available Setups")
                {
                    setup = treDatabase.SelectedNode.Parent.Text;
                    string commandTextSetup = string.Format("select setupid from setups where setupname='{0}'", setup);
                    setupid = "setupid=" + Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandTextSetup));
                }
                SaveFileDialog sfd = new SaveFileDialog();

                // Set appropriate file type(s) depending on export type
                if (radioExportTypeFile.Checked)
                {
                    sfd.FileName = MakeValidFileName(treDatabase.SelectedNode.Text);
                    if (treDatabase.SelectedNode.Parent.Text != "Grid Definitions")
                    {
                        //Disabling Excel option until performance can be improved for large datasets
                        //sfd.Filter = "CSV File (*.csv)|*.csv|Excel File (*.xlsx)|*.xlsx";
                        sfd.Filter = "CSV File (*.csv)|*.csv";
                    }
                    else
                    {
                        sfd.Filter = "Shape File (*.shp)|*.shp";
                    }
                }
                else
                {
                    sfd.Filter = "BenMAP CE Database (*.bdbx)|*.bdbx";
                }

                // Set initial directory
                //TODO: If this folder doesn't exist, create it?
                string initFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files\Exports";
                pBarExport.Value = 0;

                sfd.InitialDirectory = initFolder;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Call the appropriate export routine
                    if (radioExportTypeFile.Checked)
                    {
                        exportFile(setupid, fb, sfd.FileName);
                    }
                    else
                    {
                        exportDb(setupid, fb, sfd);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }


        private void exportFile(string setupid, ESIL.DBUtility.FireBirdHelperBase fb, string targetPath)
        {
            string _Name = treDatabase.SelectedNode.Text;
            string _tableName = treDatabase.SelectedNode.Parent.Text;
            string _setupName = treDatabase.SelectedNode.Parent.Parent.Text;
            string msg = "";
            string commandTextSetup = string.Format("select setupid from setups where setupname='{0}'", _setupName);
            int _setupid = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandTextSetup));
            int ret = 0;
            switch (_tableName)
            {
                case "Grid Definitions":
                    string _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "GriddefinitionName=" + "'" + _Name + "'";
                    msg = "The grid definition shape file has been exported successfully.";
                    writeGriddefinitionFile(_setupid_name, fb, targetPath);
                    break;
                case "Pollutant":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "PollutantName=" + "'" + _Name + "'";
                    msg = "The pollutant file has been exported successfully.";
                    writePollutantFile(_setupid_name, fb, targetPath);
                    break;
                case "Monitor Datasets":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "MonitorDatasetName=" + "'" + _Name + "'";
                    ret = writeMonitorFile(_setupid_name, fb, targetPath);
                    if (ret == 1)
                    {
                        msg = "The monitor dataset file has been exported successfully.";
                    }
                    else
                    {
                        msg = string.Format("{0} monitor dataset files have been exported successfully.", ret);
                    }
                    break;
                case "Incidence/Prevalence Datasets":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "IncidenceDatasetName=" + "'" + _Name + "'";
                    msg = "The incidence/prevalence file has been exported successfully.";
                    writeIncidenceFile(_setupid_name, fb, targetPath);
                    break;
                case "Population Datasets":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "PopulationDatasetName=" + "'" + _Name + "'";
                    msg = "The population dataset file has been exported successfully.";
                    writePopulationFile(_setupid_name, fb, targetPath);
                    break;
                case "Health Impact Functions":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "CrFunctionDatasetName=" + "'" + _Name + "'";
                    msg = "The health impact function dataset file has been exported successfully.";
                    writeCRFunctionFile(_setupid_name, fb, targetPath);
                    break;
                case "Variable Datasets":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "SetupvariableDatasetName=" + "'" + _Name + "'";
                    ret = writeVariableFile(_setupid_name, fb, targetPath);
                    if (ret == 1)
                    {
                        msg = "The monitor dataset file has been exported successfully.";
                    }
                    else
                    {
                        msg = string.Format("{0} variable files have been exported successfully.", ret);
                    }
                    
                    break;
                case "Inflation Datasets":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "InflationDatasetName=" + "'" + _Name + "'";
                    msg = "The inflation dataset file has been exported successfully.";
                    writeInflationFile(_setupid_name, fb, targetPath);
                    break;
                case "Valuation Functions":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "ValuationFunctionDatasetName=" + "'" + _Name + "'";
                    msg = "The valuation function dataset file has been exported successfully.";
                    writeValuationFile(_setupid_name, fb, targetPath);
                    break;
                case "Income Growth Adjustments":
                    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "IncomeGrowthAdjDatasetName=" + "'" + _Name + "'";
                    msg = "The income growth adjustments dataset file has been exported successfully.";
                    writeIncomeGrowthFile(_setupid_name, fb, targetPath);
                    break;
            }

            CommonClass.Connection.Close();
            CommonClass.Connection = CommonClass.getNewConnection();
            if (!errorOccur)
            {
                MessageBox.Show(msg, "File saved");
            }
            else
            {
                MessageBox.Show("An error occurred during the export process. Check application log for details.", "Export Failed"); //TODO: Adjust message for multiple files and for file type.
            }
            pBarExport.Value = 0;
            lbProcess.Text = "";
        }

        private void exportDb(string setupid, ESIL.DBUtility.FireBirdHelperBase fb, SaveFileDialog sfd)
        {
            using (Stream stream = new FileStream(sfd.FileName, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    switch (nodeName)
                    {
                        case "Available Setups":
                            ExportAvailableSetups(writer);
                            break;
                        case "Grid Definitions":
                            WriteGriddefinition(writer, setupid);
                            break;
                        case "Pollutant":
                            WritePollutant(writer, setupid);
                            break;
                        case "Monitor Datasets":
                            WriteMonitor(writer, setupid);
                            break;
                        case "Incidence/Prevalence Datasets":
                            WriteIncidence(writer, setupid);
                            break;
                        case "Population Datasets":
                            WritePopulation(writer, setupid);
                            break;
                        case "Health Impact Functions":
                            WriteCRFunction(writer, setupid);
                            break;
                        case "Variable Datasets":
                            WriteVariable(writer, setupid);
                            break;
                        case "Inflation Datasets":
                            WriteInflation(writer, setupid);
                            break;
                        case "Valuation Functions":
                            WriteValuation(writer, setupid);
                            break;
                        case "Income Growth Adjustments":
                            WriteIncomeGrowth(writer, setupid);
                            break;
                        default:
                            {
                                string _Name = treDatabase.SelectedNode.Text;
                                string _tableName = treDatabase.SelectedNode.Parent.Text;
                                string _setupName = "";
                                if (_tableName == "Available Setups")
                                {
                                    writer.Write("OneSetup");
                                    string commandTextSetup = string.Format("select setupid from setups where setupname='{0}'", _Name);
                                    int _setupid = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandTextSetup));
                                    foreach (var table in Enum.GetValues(typeof(enumDatabaseExport)))
                                    {
                                        enumDatabaseExport tablename = (enumDatabaseExport)table;
                                        switch (tablename)
                                        {
                                            case enumDatabaseExport.GridDefinitions:
                                                string _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteGriddefinition(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.Pollutants:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WritePollutant(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.MonitorDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteMonitor(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.IncidenceDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteIncidence(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.PopulationDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WritePopulation(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.CrfunctionDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteCRFunction(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.SetupvariableDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteVariable(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.InflationDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteInflation(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.ValuationfunctionDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteValuation(writer, _setupid_name);
                                                break;
                                            case enumDatabaseExport.IncomegrowthadjDatasets:
                                                _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                WriteIncomeGrowth(writer, _setupid_name);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    _setupName = treDatabase.SelectedNode.Parent.Parent.Text;
                                    string commandTextSetup = string.Format("select setupid from setups where setupname='{0}'", _setupName);
                                    int _setupid = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandTextSetup));

                                    switch (_tableName)
                                    {
                                        case "Grid Definitions":
                                            string _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "GriddefinitionName=" + "'" + _Name + "'";
                                            WriteGriddefinition(writer, _setupid_name);
                                            break;
                                        case "Pollutant":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "PollutantName=" + "'" + _Name + "'";
                                            WritePollutant(writer, _setupid_name);
                                            break;
                                        case "Monitor Datasets":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "MonitorDatasetName=" + "'" + _Name + "'";
                                            WriteMonitor(writer, _setupid_name);
                                            break;
                                        case "Incidence/Prevalence Datasets":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "IncidenceDatasetName=" + "'" + _Name + "'";
                                            WriteIncidence(writer, _setupid_name);
                                            break;
                                        case "Population Datasets":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "PopulationDatasetName=" + "'" + _Name + "'";
                                            WritePopulation(writer, _setupid_name);
                                            break;
                                        case "Health Impact Functions":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "CrFunctionDatasetName=" + "'" + _Name + "'";
                                            WriteCRFunction(writer, _setupid_name);
                                            break;
                                        case "Variable Datasets":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "SetupvariableDatasetName=" + "'" + _Name + "'";
                                            WriteVariable(writer, _setupid_name);
                                            break;
                                        case "Inflation Datasets":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "InflationDatasetName=" + "'" + _Name + "'";
                                            WriteInflation(writer, _setupid_name);
                                            break;
                                        case "Valuation Functions":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "ValuationFunctionDatasetName=" + "'" + _Name + "'";
                                            WriteValuation(writer, _setupid_name);
                                            break;
                                        case "Income Growth Adjustments":
                                            _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "IncomeGrowthAdjDatasetName=" + "'" + _Name + "'";
                                            WriteIncomeGrowth(writer, _setupid_name);
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                    writer.Flush();
                    writer.Close();
                    CommonClass.Connection.Close();
                    CommonClass.Connection = CommonClass.getNewConnection();
                    if (!errorOccur)
                        MessageBox.Show("The database file has been exported successfully.", "File saved");
                    pBarExport.Value = 0;
                    lbProcess.Text = "";
                }
            }
        }


        string nodeName = "";
        private void treDatabase_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Prevent selection of all "category" nodes if export type = file
            if(radioExportTypeFile.Checked)
            {
                if(treDatabase.SelectedNode != null && treDatabase.SelectedNode.Nodes.Count > 0)
                {
                    treDatabase.SelectedNode = null;
                    nodeName = "";
                    return;
                }
            }

            if (treDatabase.SelectedNode != null)
            {
                nodeName = treDatabase.SelectedNode.Text;
            }
            else nodeName = "";

        }

        private void writeOneTable(BinaryWriter writer, string commandText, List<string> lstType)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                FirebirdSql.Data.FirebirdClient.FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                if (!fbDataReader.HasRows) return;
                while (fbDataReader.Read())
                {
                    for (int i = 0; i < fbDataReader.FieldCount; i++)
                    {
                        switch (lstType[i])
                        {
                            case "int":
                                if (!Convert.IsDBNull(fbDataReader[i])) { writer.Write(Convert.ToInt32(fbDataReader[i])); }
                                else writer.Write(Convert.ToInt32(-1));
                                break;

                            case "single":
                                if (!Convert.IsDBNull(fbDataReader[i])) { writer.Write(Convert.ToSingle(fbDataReader[i])); }
                                else writer.Write(Convert.ToSingle(-1));
                                break;

                            case "double":
                                if (!Convert.IsDBNull(fbDataReader[i])) { writer.Write(Convert.ToDouble(fbDataReader[i])); }
                                writer.Write(Convert.ToDouble(-1));
                                break;

                            case "string":
                                writer.Write(Convert.ToString(fbDataReader[i]));
                                break;

                            case "byte[]":
                                byte[] buffer = (byte[])fbDataReader[i];
                                writer.Write(buffer.Length);
                                writer.Write(buffer);
                                break;

                            case "char":
                                writer.Write(Convert.ToChar(fbDataReader[i]));
                                break;

                            case "double2string":
                                if (!Convert.IsDBNull(fbDataReader[i]))
                                {
                                    writer.Write(Convert.ToString(Convert.ToDouble(fbDataReader[i])));
                                }
                                break;
                        }
                    }
                    pBarExport.PerformStep();
                }
                fbDataReader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        private void ExportAvailableSetups(BinaryWriter writer)
        {
            try
            {
                string allSetupid = "1=1";
                pBarExport.Value = 0;
                WriteSetup(writer);
                WriteGriddefinition(writer, allSetupid);
                WritePollutant(writer, allSetupid);
                WriteMonitor(writer, allSetupid);
                WriteIncidence(writer, allSetupid);
                WritePopulation(writer, allSetupid);
                WriteCRFunction(writer, allSetupid);
                WriteVariable(writer, allSetupid);
                WriteInflation(writer, allSetupid);
                WriteValuation(writer, allSetupid);
                WriteIncomeGrowth(writer, allSetupid);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteSetup(BinaryWriter writer)
        {
            try
            {
                string commandText = "select count(*) from setups";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 drsetupscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drsetupscount == 0) return;
                writer.Write("setups");
                writer.Write(drsetupscount);
                commandText = "select setupid,setupName from setups";
                List<string> lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteGriddefinition(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting grid definitions...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from griddefinitions where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 drgriddefinitionscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drgriddefinitionscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drgriddefinitionscount;
                writer.Write("griddefinitions");
                writer.Write(drgriddefinitionscount);
                commandText = string.Format("select GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype from griddefinitions where {0}", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string", "int", "int", "int", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting shapefiles...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select GriddefinitionID,Ttype from griddefinitions where {0}", setupid);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    writer.Write(Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                    string shapefilename = "";
                    switch (Convert.ToInt16(ds.Tables[0].Rows[i]["TTYPE"]))
                    {
                        case 0:
                            writer.Write("regular");
                            commandText = string.Format("select ShapefileName,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude from regulargriddefinitiondetails where GriddefinitionID ={0}", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                            DataRow dr = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0].Rows[0];
                            shapefilename = Convert.ToString(dr[0]);
                            writer.Write(shapefilename);
                            writer.Write(Convert.ToSingle(dr[1]));
                            writer.Write(Convert.ToSingle(dr[2]));
                            writer.Write(Convert.ToInt16(dr[3]));
                            writer.Write(Convert.ToInt16(dr[4]));
                            break;
                        case 1:
                            writer.Write("shapefile");
                            commandText = string.Format("select shapefilename from shapefilegriddefinitiondetails where GriddefinitionID ={0}", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                            shapefilename = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                            writer.Write(shapefilename);
                            break;
                    }

                    commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                    string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); Int64 BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); long BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); long BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }
                    pBarExport.PerformStep();

                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting grid definition percentages...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from GriddefinitionPercentages where SourceGriddefinitionID in (select GriddefinitionID from griddefinitions where {0}) and TargetGriddefinitionID in (select GriddefinitionID from griddefinitions where {0})", setupid);
                Int32 drGriddefinitionPercentagescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drGriddefinitionPercentagescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drGriddefinitionPercentagescount;
                writer.Write("GriddefinitionPercentages");
                writer.Write(drGriddefinitionPercentagescount);
                commandText = string.Format("select PercentageID,SourceGriddefinitionID,TargetGriddefinitionID from GriddefinitionPercentages where SourceGriddefinitionID in (select GriddefinitionID from griddefinitions where {0}) and TargetGriddefinitionID in (select GriddefinitionID from griddefinitions where {0})", setupid);
                System.Data.DataSet dsGriddefinitionPercentages = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                lstType = new List<string>() { "int", "int", "int", "int"};
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting grid definition percentage entries...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from GriddefinitionPercentageEntries where PercentageID in (select PercentageID from GriddefinitionPercentages where (SourceGriddefinitionID in (select GriddefinitionID from griddefinitions where {0})) and (TargetGriddefinitionID in (select GriddefinitionID from griddefinitions where {0})))", setupid);
                Int32 drGriddefinitionPercentageEntriescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drGriddefinitionPercentageEntriescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drGriddefinitionPercentageEntriescount;
                writer.Write("GriddefinitionPercentageEntries");
                writer.Write(drGriddefinitionPercentageEntriescount);
                commandText = string.Format("select PercentageID,SourceColumn,SourceRow,TargetColumn,TargetRow,Percentage,NormalizationState from GriddefinitionPercentageEntries where PercentageID in (select PercentageID from GriddefinitionPercentages where (SourceGriddefinitionID in (select GriddefinitionID from griddefinitions where {0})) and (TargetGriddefinitionID in (select GriddefinitionID from griddefinitions where {0})))", setupid);
                lstType = new List<string>() { "int", "int", "int", "int", "int", "double2string", "int" };
                writeOneTable(writer, commandText, lstType);

                lbProcess.Refresh();
                this.Refresh();
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WritePollutant(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting pollutants...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from pollutants where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 drpollutantscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drpollutantscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drpollutantscount;
                writer.Write("pollutants");
                writer.Write(drpollutantscount);
                commandText = string.Format("select PollutantName,PollutantID,SetupID,ObservationType from pollutants where {0}", setupid);
                List<string> lstType = new List<string>() { "string", "int", "int", "int" };
                writeOneTable(writer, commandText, lstType);

                commandText = string.Format("select count(*) from PollutantSeasons where PollutantID in (select PollutantID from pollutants where {0})", setupid);
                Int32 drPollutantSeasonscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drPollutantSeasonscount > 0)
                {
                    pBarExport.Value = 0;
                    lbProcess.Text = "Exporting pollutant seasons...";
                    lbProcess.Refresh();
                    this.Refresh();
                    pBarExport.Maximum = drPollutantSeasonscount;
                    writer.Write("PollutantSeasons");
                    writer.Write(drPollutantSeasonscount);
                    commandText = string.Format("select PollutantSeasonID,PollutantID,StartDay,EndDay,StartHour,EndHour,Numbins from PollutantSeasons where PollutantID in (select PollutantID from pollutants where {0})", setupid);
                    lstType = new List<string>() { "int", "int", "int", "int", "int", "int", "int" };
                    writeOneTable(writer, commandText, lstType);
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting pollutant metrics...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from Metrics where PollutantID in (select PollutantID from pollutants where {0})", setupid);
                Int32 drMetricscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drMetricscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drMetricscount;
                writer.Write("Metrics");
                writer.Write(drMetricscount);
                commandText = string.Format("select MetricID,PollutantID,MetricName,HourlyMetricGeneration from Metrics where PollutantID in (select PollutantID from pollutants where {0})", setupid);
                lstType = new List<string>() { "int", "int", "string", "int" };
                writeOneTable(writer, commandText, lstType);

                commandText = string.Format("select count(*) from fixedwindowMetrics where Metricid in (select Metricid from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                Int32 drFixedWindowMetricscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drFixedWindowMetricscount > 0)
                {
                    pBarExport.Value = 0;
                    lbProcess.Text = "Exporting fixed window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    pBarExport.Maximum = drFixedWindowMetricscount;
                    writer.Write("FixedWindowMetrics");
                    writer.Write(drFixedWindowMetricscount);
                    commandText = string.Format("select MetricID,Starthour,Endhour,Statistic from fixedwindowMetrics where Metricid in (select Metricid from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                    lstType = new List<string>() { "int", "int", "int", "int" };
                    writeOneTable(writer, commandText, lstType);
                }

                commandText = string.Format("select count(*) from MovingWindowMetrics where Metricid in (select Metricid from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                Int32 drMovingWindowMetricscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drMovingWindowMetricscount > 0)
                {
                    pBarExport.Value = 0;
                    lbProcess.Text = "Exporting moving window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    pBarExport.Maximum = drMovingWindowMetricscount;
                    writer.Write("MovingWindowMetrics");
                    writer.Write(drMovingWindowMetricscount);
                    commandText = string.Format("select MetricID,Windowsize,Windowstatistic,Dailystatistic from MovingWindowMetrics where Metricid in (select Metricid from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                    lstType = new List<string>() { "int", "int", "int", "int" };
                    writeOneTable(writer, commandText, lstType);
                }

                commandText = string.Format("select count(*) from CustomMetrics where Metricid in (select Metricid from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                Int32 drCustomMetricscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drCustomMetricscount > 0)
                {
                    pBarExport.Value = 0;
                    lbProcess.Text = "Exporting custom metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    pBarExport.Maximum = drCustomMetricscount;
                    writer.Write("CustomMetrics");
                    writer.Write(drCustomMetricscount);
                    commandText = string.Format("select MetricID,MetricFunction from CustomMetrics where Metricid in (select Metricid from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                    lstType = new List<string>() { "int", "string" };
                    writeOneTable(writer, commandText, lstType);
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting seasonal metrics...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                Int32 drSeasonalMetricscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drSeasonalMetricscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drSeasonalMetricscount;
                writer.Write("SeasonalMetrics");
                writer.Write(drSeasonalMetricscount);
                commandText = string.Format("select SeasonalMetricID,MetricID,SeasonalMetricName from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID in (select PollutantID from pollutants where {0}))", setupid);
                lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting seasonal metric seasons...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from SeasonalMetricSeasons where SeasonalMetricID in(select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID in (select PollutantID from pollutants where {0})))", setupid);
                Int32 drSeasonalMetricSeasonscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drSeasonalMetricSeasonscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drSeasonalMetricSeasonscount;
                writer.Write("SeasonalMetricSeasons");
                writer.Write(drSeasonalMetricSeasonscount);
                commandText = string.Format("select SeasonalMetricseasonID,SeasonalMetricID,StartDay,EndDay,SeasonalMetricType,MetricFunction,PollutantseasonID from SeasonalMetricSeasons where SeasonalMetricID in(select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID in (select PollutantID from pollutants where {0})))", setupid);
                lstType = new List<string>() { "int", "int", "int", "int", "int", "string", "int" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteMonitor(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting monitor datasets...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from MonitorDataSets where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 drMonitorDataSetscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drMonitorDataSetscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drMonitorDataSetscount;
                writer.Write("MonitorDataSets");
                writer.Write(drMonitorDataSetscount);
                commandText = string.Format("select MonitorDatasetID,SetupID,MonitorDatasetName from MonitorDataSets where {0}", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                string pollutant = string.Format("pollutantid in (select distinct pollutantid from monitors where monitordatasetid in (select monitordatasetid from monitordatasets where {0}))", setupid);
                WritePollutant(writer, pollutant);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting monitors...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from Monitors where MonitorDatasetID in (select MonitorDatasetID from MonitorDataSets where {0})", setupid);
                Int32 drMonitorscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drMonitorscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drMonitorscount;
                writer.Write("Monitors");
                writer.Write(drMonitorscount);
                commandText = string.Format("select MonitorID,MonitorDatasetID,PollutantID,Latitude,Longitude,MonitorName,MonitorDescription from Monitors where MonitorDatasetID in (select MonitorDatasetID from MonitorDataSets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "int", "single", "single", "string", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting monitor entries...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from MonitorEntries where MonitorID in (select MonitorID from Monitors where MonitorDatasetID in (select MonitorDatasetID from MonitorDataSets where {0}))", setupid);
                Int32 dsMonitorEntriescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsMonitorEntriescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsMonitorEntriescount;
                writer.Write("MonitorEntries");
                writer.Write(dsMonitorEntriescount);
                commandText = string.Format("select MonitorEntryID,MonitorID,Vvalues,MetricID,SeasonalMetricID,Yyear,Statistic from MonitorEntries where MonitorID in (select MonitorID from Monitors where MonitorDatasetID in (select MonitorDatasetID from MonitorDataSets where {0}))", setupid);
                lstType = new List<string>() { "int", "int", "byte[]", "int", "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                this.Refresh();
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteIncidence(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related grid definition...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(distinct griddefinitionID) from IncidenceDatasets where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 drgriddefinitionscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drgriddefinitionscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drgriddefinitionscount;
                writer.Write("IncidenceGriddefinitions");
                writer.Write(drgriddefinitionscount);
                commandText = string.Format("select GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype from griddefinitions where griddefinitionID in (select distinct griddefinitionID from IncidenceDatasets where {0})", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string", "int", "int", "int", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related shapefiles...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select GriddefinitionID,Ttype from griddefinitions where griddefinitionID in (select distinct griddefinitionID from IncidenceDatasets where {0})", setupid);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    writer.Write(Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                    string shapefilename = "";
                    switch (Convert.ToInt16(ds.Tables[0].Rows[i]["TTYPE"]))
                    {
                        case 0:
                            writer.Write("regular");
                            commandText = string.Format("select ShapefileName,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude from regulargriddefinitiondetails where GriddefinitionID ={0}", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                            DataRow dr = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0].Rows[0];
                            shapefilename = Convert.ToString(dr[0]);
                            writer.Write(shapefilename);
                            writer.Write(Convert.ToSingle(dr[1]));
                            writer.Write(Convert.ToSingle(dr[2]));
                            writer.Write(Convert.ToInt16(dr[3]));
                            writer.Write(Convert.ToInt16(dr[4]));
                            break;
                        case 1:
                            writer.Write("shapefile");
                            commandText = string.Format("select shapefilename from shapefilegriddefinitiondetails where GriddefinitionID ={0}", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                            shapefilename = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                            writer.Write(shapefilename);
                            break;
                    }

                    commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                    string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); Int64 BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); long BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); long BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }
                    pBarExport.PerformStep();
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting incidence datasets...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from IncidenceDatasets where {0}", setupid);
                Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("IncidenceDatasets");
                writer.Write(count);
                commandText = string.Format("select IncidenceDatasetID,SetupID,IncidenceDatasetName,GridDefinitionID from IncidenceDatasets where {0}", setupid);
                lstType = new List<string>() { "int", "int", "string", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related endpointgroups...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(distinct EndPointgroupID) from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("EndpointGroups");
                writer.Write(count);
                commandText = string.Format("select EndPointGroupID,EndPointGroupName from EndpointGroups where EndPointGroupID in (select distinct EndPointgroupID from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related endpoints...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(distinct EndPointID) from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("EndPoints");
                writer.Write(count);
                commandText = string.Format("select EndPointID,EndPointGroupID,EndPointName from EndPoints where EndPointID in (select distinct EndPointID from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related race...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(distinct RaceID) from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("Race");
                writer.Write(count);
                commandText = string.Format("select RaceID,RaceName from Races where RaceID in (select distinct RaceID from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related gender...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(distinct GenderID) from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("Gender");
                writer.Write(count);
                commandText = string.Format("select GenderID,GenderName from Genders where GenderID in (select distinct GenderID from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related ethnicity...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(distinct EthnicityID) from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("Ethnicity");
                writer.Write(count);
                commandText = string.Format("select EthnicityID,EthnicityName from Ethnicity where EthnicityID in (select distinct EthnicityID from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting incidence rates...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("IncidenceRates");
                writer.Write(count);
                commandText = string.Format("select IncidenceRateID,IncidenceDatasetID,GriddefinitionID,EndPointGroupID,EndPointID,RaceID,GenderID,StartAge,EndAge,Prevalence,EthnicityID from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "int", "int", "int", "int", "int", "int", "int", "char", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting incidence entries...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from IncidenceEntries where IncidenceRateID in (select IncidenceRateID  from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("IncidenceEntries");
                writer.Write(count);
                commandText = string.Format("select IncidenceRateID,Ccolumn,Row,Vvalue from Incidenceentries where IncidenceRateID in (select IncidenceRateID  from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "int", "int", "single" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WritePopulation(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related grid definition...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(distinct griddefinitionID) from PopulationDatasets where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 drgriddefinitionscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drgriddefinitionscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = drgriddefinitionscount;
                writer.Write("PopulationGriddefinitions");
                writer.Write(drgriddefinitionscount);
                commandText = string.Format("select GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype from griddefinitions where griddefinitionID in (select distinct griddefinitionID from PopulationDatasets where {0})", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string", "int", "int", "int", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting related shapefiles...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select GriddefinitionID,Ttype from griddefinitions where griddefinitionID in (select distinct griddefinitionID from PopulationDatasets where {0})", setupid);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    writer.Write(Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                    string shapefilename = "";
                    switch (Convert.ToInt16(ds.Tables[0].Rows[i]["TTYPE"]))
                    {
                        case 0:
                            writer.Write("regular");
                            commandText = string.Format("select ShapefileName,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude from regulargriddefinitiondetails where GriddefinitionID ={0}", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                            DataRow dr = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0].Rows[0];
                            shapefilename = Convert.ToString(dr[0]);
                            writer.Write(shapefilename);
                            writer.Write(Convert.ToSingle(dr[1]));
                            writer.Write(Convert.ToSingle(dr[2]));
                            writer.Write(Convert.ToInt16(dr[3]));
                            writer.Write(Convert.ToInt16(dr[4]));
                            break;
                        case 1:
                            writer.Write("shapefile");
                            commandText = string.Format("select shapefilename from shapefilegriddefinitiondetails where GriddefinitionID ={0}", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                            shapefilename = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                            writer.Write(shapefilename);
                            break;
                    }

                    commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                    string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); Int64 BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); long BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Open, FileAccess.Read); BinaryReader BinaryFile = new BinaryReader(fs); long BytesSum = fs.Length; writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }
                    pBarExport.PerformStep();
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting population configurations...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from PopulationConfigurations where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                Int32 dsPopulationConfigurationscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsPopulationConfigurationscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsPopulationConfigurationscount;
                writer.Write("PopulationConfigurations");
                writer.Write(dsPopulationConfigurationscount);
                commandText = string.Format("select PopulationConfigurationID,PopulationConfigurationName from PopulationConfigurations where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting popconfig ethnicity map...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from PopConfigEthnicityMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                Int32 PopconfigethnicityMapcount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (PopconfigethnicityMapcount > 0)
                {
                    pBarExport.Maximum = 2 * PopconfigethnicityMapcount;
                    writer.Write("PopConfigEthnicityMap");
                    writer.Write(PopconfigethnicityMapcount);
                    commandText = string.Format("select EthnicityID,EthnicityName from Ethnicity where EthnicityID in (select EthnicityID from PopConfigEthnicityMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0}))", setupid);
                    lstType = new List<string>() { "int", "string" };
                    writeOneTable(writer, commandText, lstType);

                    commandText = string.Format("select PopulationConfigurationID,EthnicityID from PopConfigEthnicityMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                    lstType = new List<string>() { "int", "int" };
                    writeOneTable(writer, commandText, lstType);
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting popconfig gender map...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from PopConfigGenderMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                Int32 PopConfigGenderMapcount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (PopconfigethnicityMapcount > 0)
                {
                    pBarExport.Maximum = 2 * PopconfigethnicityMapcount;
                    writer.Write("PopConfigGenderMap");
                    writer.Write(PopConfigGenderMapcount);
                    commandText = string.Format("select GenderID,GenderName from Genders where GenderID in (select GenderID from PopConfigGenderMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0}))", setupid);
                    lstType = new List<string>() { "int", "string" };
                    writeOneTable(writer, commandText, lstType);

                    commandText = string.Format("select PopulationConfigurationID,GenderID from PopConfigGenderMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                    lstType = new List<string>() { "int", "int" };
                    writeOneTable(writer, commandText, lstType);
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting popconfig race map...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from PopConfigRaceMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                Int32 PopConfigRaceMapcount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (PopConfigRaceMapcount > 0)
                {
                    pBarExport.Maximum = 2 * PopConfigRaceMapcount;
                    writer.Write("PopConfigRaceMap");
                    writer.Write(PopConfigRaceMapcount);
                    commandText = string.Format("select RaceID,RaceName from Races where raceid in (select RaceID from PopConfigRaceMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0}))", setupid);
                    lstType = new List<string>() { "int", "string" };
                    writeOneTable(writer, commandText, lstType);

                    commandText = string.Format("select PopulationConfigurationID,RaceID from PopConfigRaceMap where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                    lstType = new List<string>() { "int", "int" };
                    writeOneTable(writer, commandText, lstType);
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting ageranges...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from AgeRanges where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                Int32 dsAgeRangescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsAgeRangescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsAgeRangescount;
                writer.Write("AgeRanges");
                writer.Write(dsAgeRangescount);
                commandText = string.Format("select AgeRangeID,PopulationConfigurationID,AgeRangeName,StartAge,EndAge from AgeRanges where PopulationConfigurationID in (select PopulationConfigurationID from PopulationDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "string", "int", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting population datasets...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from PopulationDatasets where {0}", setupid);
                Int32 dsPopulationDatasetscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsPopulationDatasetscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsPopulationDatasetscount;
                writer.Write("PopulationDatasets");
                writer.Write(dsPopulationDatasetscount);
                commandText = string.Format("select PopulationDatasetID,SetupID,PopulationDatasetName,PopulationConfigurationID,GriddefinitionID,ApplyGrowth from PopulationDatasets where {0}", setupid);
                lstType = new List<string>() { "int", "int", "string", "int", "int", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting population entries...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from PopulationEntries where PopulationDatasetID in (select PopulationDatasetID from PopulationDatasets where {0})", setupid);
                Int32 dsPopulationEntriescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                pBarExport.Maximum = dsPopulationEntriescount;
                if (dsPopulationEntriescount > 0)
                {
                    writer.Write("PopulationEntries");
                    writer.Write(dsPopulationEntriescount);
                    commandText = string.Format("select PopulationDatasetID,RaceID,GenderID,AgerangeID,Ccolumn,Row,Yyear,Vvalue,EthnicityID from PopulationEntries where PopulationDatasetID in (select PopulationDatasetID from PopulationDatasets where {0})", setupid);
                    lstType = new List<string>() { "int", "int", "int", "int", "int", "int", "int", "single", "int" };
                    writeOneTable(writer, commandText, lstType);
                    lbProcess.Refresh();
                    this.Refresh();
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting population growthweights...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from PopulationGrowthWeights where PopulationDatasetID in (select PopulationDatasetID from PopulationDatasets where {0})", setupid);
                Int32 dsPopulationGrowthWeightscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsPopulationGrowthWeightscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsPopulationGrowthWeightscount;
                writer.Write("PopulationGrowthWeights");
                writer.Write(dsPopulationGrowthWeightscount);
                commandText = string.Format("select PopulationDatasetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue from PopulationGrowthWeights where PopulationDatasetID in (select PopulationDatasetID from PopulationDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "int", "int", "int", "int", "int", "int", "single" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteCRFunction(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting HIF datasets...";
                lbProcess.Refresh();
                this.Refresh();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select count(*) from CrFunctionDatasets where {0}", setupid);
                Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("CrFunctionDatasets");
                writer.Write(count);
                commandText = string.Format("select CrfunctionDatasetID,SetupID,CrfunctionDatasetName,Readonly from CrFunctionDatasets where {0}", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string", "char" };
                writeOneTable(writer, commandText, lstType);

                string pollutant = string.Format("pollutantid in (select distinct pollutantid from crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                WritePollutant(writer, pollutant);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting functional forms...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from FunctionalForms where FunctionalFormID in (select FunctionalFormID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("FunctionalForms");
                writer.Write(count);
                commandText = string.Format("select FunctionalFormID,FunctionalFormText from FunctionalForms where FunctionalFormID in (select FunctionalFormID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting baseline functional forms...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from BaselineFunctionalForms where FunctionalFormID in (select BaselineFunctionalFormID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                pBarExport.Maximum = count;
                if (count != 0)
                {
                    writer.Write("BaselineFunctionalForms");
                    writer.Write(count);
                    commandText = string.Format("select FunctionalFormID,FunctionalFormText from BaselineFunctionalForms where FunctionalFormID in (select BaselineFunctionalFormID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                    lstType = new List<string>() { "int", "string" };
                    writeOneTable(writer, commandText, lstType);
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting endpointgroups...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from EndpointGroups where EndPointGroupID in (select EndPointgroupID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("EndpointGroups");
                writer.Write(count);
                commandText = string.Format("select EndPointGroupID,EndPointGroupName from EndpointGroups where EndPointGroupID in (select EndPointgroupID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting endpoints...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(distinct EndPointID) from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("EndPoints");
                writer.Write(count);
                commandText = string.Format("select EndPointID,EndPointGroupID,EndPointName from EndPoints where EndPointID in (select distinct EndPointID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting geographicareas...";
                lbProcess.Refresh();
                this.Refresh();
                // TODO: Update for GeographicAreas?
                // Removing this from export because if we include it here, we also need to include the associated grid definition
                /*
                commandText = string.Format("select count(*) from geographicareas where GeographicAreaId in (select LocationTypeID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                pBarExport.Maximum = count;
                if (count != 0)
                {
                    writer.Write("LocationType");
                    writer.Write(count);
                    commandText = string.Format("select SetupID,LocationTypeID,LocationTypeName from EndPoints where LocationTypeID in (select LocationTypeID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                    lstType = new List<string>() { "int", "int", "string" };
                    writeOneTable(writer, commandText, lstType);
                }
                */
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting functions...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0})", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("Crfunctions");
                writer.Write(count);
                commandText = string.Format("select CrfunctionID,CrfunctionDatasetID,FunctionalFormID,MetricID,SeasonalMetricID,IncidenceDatasetID,PrevalenceDatasetID,VariableDatasetID,LocationTypeID,BaselineFunctionalFormID,EndPointgroupID,EndPointID,PollutantID,Metricstatistic,Author,Yyear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,EndAge,Beta,DistBeta,P1beta,P2beta,A,NameA,B,NameB,C,NameC,Ethnicity,Percentile from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "int", "int", "int", "int", "int", "int", "int", "int", "int", "int", "int", "int", "string", "int", "string", "string", "string", "string", "string", "string", "int", "int", "string", "string", "string", "string", "string", "string", "string", "string", "string", "string", "string", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting HIF custom entries...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from CrFunctionCustomEntries where CrfunctionID in (select CrfunctionID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = count;
                writer.Write("CrFunctionCustomEntries");
                writer.Write(count);
                commandText = string.Format("select CrFunctionID,Vvalue from CrFunctionCustomEntries where CrfunctionID in (select CrfunctionID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "single" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteVariable(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting setupvariable datasets...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from SetupVariableDatasets where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 dsSetupVariableDatasetscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsSetupVariableDatasetscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsSetupVariableDatasetscount;
                writer.Write("SetupVariableDatasets");
                writer.Write(dsSetupVariableDatasetscount);
                commandText = string.Format("select SetupVariableDatasetID,SetupID,SetupVariableDatasetName from SetupVariableDatasets where {0}", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                string griddefinition = string.Format("griddefinitionid in (select griddefinitionid from setupvariables where setupvariabledatasetid in (select setupvariabledatasetid from SetupVariableDatasets where {0}))", setupid);
                WriteGriddefinition(writer, griddefinition);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting setupvariables...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from SetupVariables where SetupVariableDatasetID in (select SetupVariableDatasetID from SetupVariableDatasets where {0})", setupid);
                Int32 dsSetupVariablescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsSetupVariablescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsSetupVariablescount;
                writer.Write("SetupVariables");
                writer.Write(dsSetupVariablescount);
                commandText = string.Format("select SetupVariableID,SetupVariableDatasetID,SetupVariableName,GriddefinitionID from SetupVariables where SetupVariableDatasetID in (select SetupVariableDatasetID from SetupVariableDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "string", "int" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting setup geographic variables...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from SetupGeographicVariables where SetupVariableID in(select SetupVariableID from SetupVariables where SetupVariableDatasetID in (select SetupVariableDatasetID from SetupVariableDatasets where {0}))", setupid);
                Int32 dsSetupGeographicvariablescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                pBarExport.Maximum = dsSetupGeographicvariablescount;
                if (dsSetupGeographicvariablescount != 0)
                {
                    writer.Write("SetupGeographicVariables");
                    writer.Write(dsSetupGeographicvariablescount);
                    commandText = string.Format("select SetupVariableID,Ccolumn,Row,Vvalue from SetupGeographicVariables where SetupVariableID in(select SetupVariableID from SetupVariables where SetupVariableDatasetID in (select SetupVariableDatasetID from SetupVariableDatasets where {0}))", setupid);
                    lstType = new List<string>() { "int", "int", "int", "single" };
                    writeOneTable(writer, commandText, lstType);
                }

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting setup global variables...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from SetupGlobalVariables where SetupVariableID in(select SetupVariableID from SetupVariables where SetupVariableDatasetID in (select SetupVariableDatasetID from SetupVariableDatasets where {0}))", setupid);
                Int32 dsSetupGlobalVariablescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsSetupGlobalVariablescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsSetupGlobalVariablescount;
                writer.Write("SetupGlobalVariables");
                writer.Write(dsSetupGlobalVariablescount);
                commandText = string.Format("select SetupVariableID,Vvalue from SetupGlobalVariables where SetupVariableID in(select SetupVariableID from SetupVariables where SetupVariableDatasetID in (select SetupVariableDatasetID from SetupVariableDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "single" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteInflation(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting inflation datasets...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from InflationDatasets where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 dsInflationDatasetscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsInflationDatasetscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsInflationDatasetscount;
                writer.Write("InflationDatasets");
                writer.Write(dsInflationDatasetscount);
                commandText = string.Format("select InflationDatasetID,SetupID,InflationDatasetName from InflationDatasets where {0}", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting inflation entries...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from InflationEntries where InflationDatasetID in (select InflationDatasetID from InflationDatasets where {0})", setupid);
                Int32 dsInflationEntriescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsInflationEntriescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsInflationEntriescount;
                writer.Write("InflationEntries");
                writer.Write(dsInflationEntriescount);
                commandText = string.Format("select InflationDatasetID,Yyear,AllGoodsIndex,MedicalCostIndex,WageIndex from InflationEntries where InflationDatasetID in (select InflationDatasetID from InflationDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "single", "single", "single" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteValuation(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting valuation function datasets...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from ValuationFunctionDatasets where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 dsValuationFunctionDatasetscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsValuationFunctionDatasetscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsValuationFunctionDatasetscount;
                writer.Write("ValuationFunctionDatasets");
                writer.Write(dsValuationFunctionDatasetscount);
                commandText = string.Format("select ValuationFunctionDatasetID,SetupID,ValuationFunctionDatasetName,Readonly from ValuationFunctionDatasets where {0}", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string", "char" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting valuation functional forms...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from ValuationFunctionalForms where FunctionalFormID in (select FunctionalFormID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                Int32 dsValuationFunctionalFormscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsValuationFunctionalFormscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsValuationFunctionalFormscount;
                writer.Write("ValuationFunctionalForms");
                writer.Write(dsValuationFunctionalFormscount);
                commandText = string.Format("select FunctionalFormID,FunctionalFormText from ValuationFunctionalForms where FunctionalFormID in (select FunctionalFormID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting endpointgroups...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from EndpointGroups where EndPointGroupID in (select EndPointgroupID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                Int32 dsEndpointGroupscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsEndpointGroupscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsEndpointGroupscount;
                writer.Write("EndpointGroups");
                writer.Write(dsEndpointGroupscount);
                commandText = string.Format("select EndPointGroupID,EndPointGroupName from EndpointGroups where EndPointGroupID in (select EndPointgroupID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting endpoints...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from EndPoints where EndPointGroupID in (select EndPointgroupID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                Int32 dsEndPointscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsEndPointscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsEndPointscount;
                writer.Write("EndPoints");
                writer.Write(dsEndPointscount);
                commandText = string.Format("select EndPointID,EndPointGroupID,EndPointName from EndPoints where EndPointGroupID in (select EndPointgroupID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting valuation functions...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0})", setupid);
                Int32 dsValuationFunctionscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsValuationFunctionscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsValuationFunctionscount;
                writer.Write("ValuationFunctions");
                writer.Write(dsValuationFunctionscount);
                commandText = string.Format("select ValuationFunctionID,ValuationFunctionDatasetID,FunctionalFormID,EndPointGroupID,EndPointID,Qualifier,Reference,StartAge,EndAge,NameA,DistA,NameB,NameC,NameD,A,P1A,P2A,B,C,D from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "int", "int", "int", "int", "string", "string", "int", "int", "string", "string", "string", "string", "string", "string", "string", "string", "string", "string", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting valuation function customentries...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from ValuationFunctionCustomEntries where ValuationFunctionID in (select ValuationFunctionID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                Int32 dsValuationFunctionCustomEntriescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsValuationFunctionCustomEntriescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsValuationFunctionCustomEntriescount;
                writer.Write("ValuationFunctionCustomEntries");
                writer.Write(dsValuationFunctionCustomEntriescount);
                commandText = string.Format("select ValuationFunctionID,Vvalue from ValuationFunctionCustomEntries where ValuationFunctionID in (select ValuationFunctionID from ValuationFunctions where ValuationFunctionDatasetID in (select ValuationFunctionDatasetID from ValuationFunctionDatasets where {0}))", setupid);
                lstType = new List<string>() { "int", "single" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void WriteIncomeGrowth(BinaryWriter writer, string setupid)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting incomegrowthadj datasets...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from IncomeGrowthAdjDatasets where {0}", setupid);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Int32 dsIncomeGrowthAdjDatasetscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsIncomeGrowthAdjDatasetscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsIncomeGrowthAdjDatasetscount;
                writer.Write("IncomeGrowthAdjDatasets");
                writer.Write(dsIncomeGrowthAdjDatasetscount);
                commandText = string.Format("select IncomeGrowthAdjDatasetID,SetupID,IncomeGrowthAdjDatasetName from IncomeGrowthAdjDatasets where {0}", setupid);
                List<string> lstType = new List<string>() { "int", "int", "string" };
                writeOneTable(writer, commandText, lstType);

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting incomegrowthadj factors...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select count(*) from IncomeGrowthAdjFactors where IncomeGrowthAdjDatasetID in (select IncomeGrowthAdjDatasetID from IncomeGrowthAdjDatasets where {0})", setupid);
                Int32 dsIncomeGrowthAdjFactorscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (dsIncomeGrowthAdjFactorscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                pBarExport.Maximum = dsIncomeGrowthAdjFactorscount;
                writer.Write("IncomeGrowthAdjFactors");
                writer.Write(dsIncomeGrowthAdjFactorscount);
                commandText = string.Format("select IncomeGrowthAdjDatasetID,Distribution,P1,P2,Yyear,Mean,EndPointGroups from IncomeGrowthAdjFactors where IncomeGrowthAdjDatasetID in (select IncomeGrowthAdjDatasetID from IncomeGrowthAdjDatasets where {0})", setupid);
                lstType = new List<string>() { "int", "string", "single", "single", "int", "single", "string" };
                writeOneTable(writer, commandText, lstType);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void radioExportType_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioExportTypeFile_CheckedChanged(object sender, EventArgs e)
        {
            // It's only valid to select nodes at the lowest level when exporting native files
            if (((RadioButton) sender).Checked)
            {
                if (treDatabase.SelectedNode != null && treDatabase.SelectedNode.Nodes.Count > 0)
                {
                    treDatabase.SelectedNode = null;
                }
            }
        }

        private void writeIncidenceFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                lbProcess.Text = "Querying Database...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Empty;
                //commandText = string.Format("select count(*) from IncidenceEntries where IncidenceRateID in (select IncidenceRateID  from IncidenceRates where IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0}))", sqlWhereClause);
                //Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //pBarExport.Maximum = count;

                DataTable dtOut = new DataTable();
                dtOut.Columns.Add("Endpoint Group", typeof(string));
                dtOut.Columns.Add("Endpoint", typeof(string));
                dtOut.Columns.Add("Race", typeof(string));
                dtOut.Columns.Add("Gender", typeof(string));
                dtOut.Columns.Add("Ethnicity", typeof(string));
                dtOut.Columns.Add("Start Age", typeof(int));
                dtOut.Columns.Add("End Age", typeof(int));
                dtOut.Columns.Add("Type", typeof(string));
                dtOut.Columns.Add("Column", typeof(int));
                dtOut.Columns.Add("Row", typeof(int));
                dtOut.Columns.Add("Value", typeof(double));
 
                commandText = string.Format(@"select c.ENDPOINTGROUPNAME 
                        , d.ENDPOINTNAME 
                        , e.RACENAME 
                        , f.GENDERNAME 
                        , g.ETHNICITYNAME 
                        , a.StartAge 
                        , a.EndAge 
                        , CASE WHEN a.Prevalence = 'T' THEN 'Prevalence' ELSE 'Incidence' END ""Type""
                        , b.Ccolumn, b.Row, b.Vvalue
                    from IncidenceRates a
                    join IncidenceEntries b on a.IncidenceRateId = b.IncidenceRateId
                    join EndPointGroups c on a.endpointGroupID = c.endpointGroupID
                    join ENDPOINTS d on a.ENDPOINTID = d.ENDPOINTID
                    join RACES e on a.RACEID = e.RACEID
                    join GENDERS f on a.GENDERID = f.GENDERID
                    join ETHNICITY g on a.ETHNICITYID = g.ETHNICITYID
                    where a.IncidenceDatasetID in (select IncidenceDatasetID from IncidenceDatasets where {0})", sqlWhereClause);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                lbProcess.Text = "Exporting File...";
                lbProcess.Refresh();
                this.Refresh();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Endpoint Group"] = dr["ENDPOINTGROUPNAME"];
                    newdr["Endpoint"] = dr["ENDPOINTNAME"];
                    newdr["Race"] = dr["RACENAME"];
                    newdr["Gender"] = dr["GENDERNAME"];
                    newdr["Ethnicity"] = dr["ETHNICITYNAME"];
                    newdr["Start Age"] = Convert.ToInt32(dr["StartAge"]);
                    newdr["End Age"] = Convert.ToInt32(dr["EndAge"]);
                    newdr["Type"] = dr["Type"];
                    newdr["Column"] = Convert.ToInt32(dr["Ccolumn"]);
                    newdr["Row"] = Convert.ToInt32(dr["Row"]);
                    newdr["Value"] = Convert.ToDouble(dr["Vvalue"]);
                    dtOut.Rows.Add(newdr);
                }

                SaveCSVOrExcel(dtOut, fileName);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
            }
        }

        private void writePopulationFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                lbProcess.Text = "Querying Database...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Empty;
                /*
                commandText = string.Format(@"select count(*)
                    from POPULATIONDATASETS a
                    join POPULATIONENTRIES b on a.POPULATIONDATASETID = b.POPULATIONDATASETID
                    where ({0})", sqlWhereClause);
                Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                pBarExport.Maximum = count;
                lbProcess.Refresh();
                */
                DataTable dtOut = new DataTable();
                dtOut.Columns.Add("Race", typeof(string));
                dtOut.Columns.Add("Gender", typeof(string));
                dtOut.Columns.Add("AgeRange", typeof(string));
                dtOut.Columns.Add("Ethnicity", typeof(string));
                dtOut.Columns.Add("Year", typeof(int));
                dtOut.Columns.Add("Row", typeof(int));
                dtOut.Columns.Add("Column", typeof(int));
                dtOut.Columns.Add("Population", typeof(double));

                commandText = string.Format(@"select c.RACENAME
                    , d.GENDERNAME
                    , e.AGERANGENAME
                    , f.ETHNICITYNAME
                    , b.CCOLUMN
                    , b.ROW
                    , b.YYEAR
                    , b.VVALUE
                from POPULATIONDATASETS a
                join POPULATIONENTRIES b on a.POPULATIONDATASETID = b.POPULATIONDATASETID
                join RACES c on b.raceid = c.RACEID
                join GENDERS d on b.GENDERID = d.GENDERID
                join AGERANGES e on b.AGERANGEID = e.AGERANGEID
                join ETHNICITY f on b.ETHNICITYID = f.ETHNICITYID
                where {0}", sqlWhereClause);

                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                lbProcess.Text = "Exporting File...";
                this.Refresh();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Race"] = dr["RACENAME"];
                    newdr["Gender"] = dr["GENDERNAME"];
                    newdr["AgeRange"] = dr["AGERANGENAME"];
                    newdr["Ethnicity"] = dr["ETHNICITYNAME"];
                    newdr["Row"] = Convert.ToInt32(dr["ROW"]);
                    newdr["Column"] = Convert.ToInt32(dr["CCOLUMN"]);
                    newdr["Year"] = Convert.ToInt32(dr["YYEAR"]);
                    newdr["Population"] = Convert.ToDouble(dr["VVALUE"]);
                    dtOut.Rows.Add(newdr);
                }

                SaveCSVOrExcel(dtOut, fileName);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
            }
        }
        string AddSuffix(string filename, string suffix)
        {
            string fDir = Path.GetDirectoryName(filename);
            string fName = Path.GetFileNameWithoutExtension(filename);
            string fExt = Path.GetExtension(filename);
            return Path.Combine(fDir, String.Concat(fName, suffix, fExt));
        }

        private int writeMonitorFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {
                string commandText = string.Empty;
                int ret = 0;
                // Group the entries by pollutant and year
                commandText = string.Format(@"select a.pollutantid 
                    , c.pollutantname
                    , b.yyear
                    , count(*) record_count
                from Monitors a
                join MonitorEntries b on a.MonitorID=b.MonitorID 
                join POLLUTANTS c on a.pollutantid = c.pollutantid
                where a.MonitorID in (select MonitorID from Monitors where MonitorDatasetID in (select MonitorDatasetID from MonitorDataSets where {0}))
                group by 1, 2, 3
                order by 2, 3", sqlWhereClause);

                FirebirdSql.Data.FirebirdClient.FbDataReader fbGroupDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                while (fbGroupDataReader.Read())
                {
                    lbProcess.Text = string.Format("Exporting {0} {1}...", fbGroupDataReader["pollutantname"].ToString(), fbGroupDataReader["yyear"].ToString());
//                    pBarExport.Value = 0;
//                    pBarExport.Maximum = Convert.ToInt32(fbGroupDataReader["record_count"]);
                    lbProcess.Refresh();
                    this.Refresh();

                    DataTable dtOut = new DataTable();
                    dtOut.Columns.Add("Monitor Name", typeof(string));
                    dtOut.Columns.Add("Monitor Description", typeof(string));
                    dtOut.Columns.Add("Latitude", typeof(double));
                    dtOut.Columns.Add("Longitude", typeof(double));
                    dtOut.Columns.Add("Metric", typeof(string));
                    dtOut.Columns.Add("Seasonal Metric", typeof(string));
                    dtOut.Columns.Add("Statistic", typeof(string));
                    dtOut.Columns.Add("Values", typeof(string));
                    Dictionary<int, string> dicMetric = getMetric();
                    Dictionary<int, string> dicSeasonalMetric = getSeasonalMetric();

                    commandText = string.Format(@"select a.MonitorName
                        , a.MonitorDescription
                        , a.Latitude
                        , a.Longitude
                        , c.MetricName
                        , d.SeasonalMetricName
                        , Statistic
                        , Vvalues 
                    from Monitors a
                    join MonitorEntries b on a.MonitorID=b.MonitorID 
                    left join METRICS c on b.MetricID = c.MetricID
                    left join SEASONALMETRICS d on b.SeasonalMetricID = d.SeasonalMetricID
                    where a.MonitorID in (select MonitorID from Monitors where MonitorDatasetID in (select MonitorDatasetID from MonitorDataSets where {0}))
                    and a.pollutantid = {1} 
                    and b.yyear = {2}", sqlWhereClause, Convert.ToInt32(fbGroupDataReader["pollutantid"]), Convert.ToInt32(fbGroupDataReader["yyear"]));

                    FirebirdSql.Data.FirebirdClient.FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    Byte[] blob = null;
                    string str = string.Empty;
                    while (fbDataReader.Read())
                    {
                        DataRow dr = dtOut.NewRow();
                        blob = fbDataReader["Vvalues"] as byte[];
                        str = System.Text.Encoding.Default.GetString(blob);
                        dr["Monitor Name"] = fbDataReader["MonitorName"].ToString();
                        dr["Monitor Description"] = fbDataReader["MonitorDescription"].ToString();
                        dr["Latitude"] = Convert.ToDouble(fbDataReader["Latitude"]);
                        dr["Longitude"] = Convert.ToDouble(fbDataReader["Longitude"]);
                        if (!(fbDataReader["MetricName"] is DBNull))
                        {
                            dr["Metric"] = fbDataReader["MetricName"].ToString();
                        }
                        if (!(fbDataReader["SeasonalMetricName"] is DBNull))
                        {
                            dr["Seasonal Metric"] = fbDataReader["SeasonalMetricName"].ToString();
                        }
                        dr["Statistic"] = fbDataReader["Statistic"].ToString();
                        dr["Values"] = str;
                        dtOut.Rows.Add(dr);

                    }
                    ret++;
                    SaveCSVOrExcel(dtOut, AddSuffix(fileName, MakeValidFileName(String.Format("_{0}_{1}", fbGroupDataReader["pollutantname"].ToString(), fbGroupDataReader["yyear"].ToString() ) )));

                }
                return ret;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return 0;
            }
        }

        private void writeCRFunctionFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                lbProcess.Text = "Querying Database...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Empty;
                //TODO: Fix this.  The count should be fore detail records
                //commandText = string.Format("select count(*) from CrFunctionDatasets where {0}", sqlWhereClause);
                //Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //pBarExport.Maximum = count;
                //lbProcess.Refresh();

                DataTable dtOut = new DataTable();

                dtOut.Columns.Add("Endpoint Group", typeof(string));
                dtOut.Columns.Add("Endpoint", typeof(string));
                dtOut.Columns.Add("Pollutant", typeof(string));
                dtOut.Columns.Add("Metric", typeof(string));
                dtOut.Columns.Add("Seasonal Metric", typeof(string));
                dtOut.Columns.Add("Metric Statistic", typeof(string));
                dtOut.Columns.Add("Study Author", typeof(string));
                dtOut.Columns.Add("Study Year", typeof(string));
                dtOut.Columns.Add("Geographic Area", typeof(string));
                dtOut.Columns.Add("Study Location", typeof(string));
                dtOut.Columns.Add("Other Pollutants", typeof(string));
                dtOut.Columns.Add("Qualifier", typeof(string));
                dtOut.Columns.Add("Reference", typeof(string));
                dtOut.Columns.Add("Race", typeof(string));
                dtOut.Columns.Add("Ethnicity", typeof(string));
                dtOut.Columns.Add("Gender", typeof(string));
                dtOut.Columns.Add("Start Age", typeof(string));
                dtOut.Columns.Add("End Age", typeof(string));
                dtOut.Columns.Add("Function", typeof(string));
                dtOut.Columns.Add("Baseline Function", typeof(string));
                dtOut.Columns.Add("Beta", typeof(string));
                dtOut.Columns.Add("Distribution Beta", typeof(string));
                dtOut.Columns.Add("Parameter 1 Beta", typeof(string));
                dtOut.Columns.Add("Parameter 2 Beta", typeof(string));
                dtOut.Columns.Add("A", typeof(string));
                dtOut.Columns.Add("Name A", typeof(string));
                dtOut.Columns.Add("B", typeof(string));
                dtOut.Columns.Add("Name B", typeof(string));
                dtOut.Columns.Add("C", typeof(string));
                dtOut.Columns.Add("Name C", typeof(string));
                dtOut.Columns.Add("Incidence DataSet", typeof(string));
                dtOut.Columns.Add("Prevalence DataSet", typeof(string));
                dtOut.Columns.Add("Variable DataSet", typeof(string));

                commandText = string.Format(@"select b.ENDPOINTGROUPNAME,c.ENDPOINTNAME,d.POLLUTANTNAME
, e.METRICNAME, f.SEASONALMETRICNAME, Metricstatistic, Author, Yyear, Location
, Otherpollutants, Qualifier, Reference,Race, Gender, Startage, Endage
, g.FUNCTIONALFORMTEXT FUNCTIONALFORM,h_i.INCIDENCEDATASETNAME INCIDENCEDATASET, h_p.INCIDENCEDATASETNAME PREVALENCEDATASET,i.SETUPVARIABLEDATASETNAME
, Beta,Distbeta,P1Beta,P2Beta,A,Namea,B,Nameb, C,Namec
, j.FUNCTIONALFORMTEXT BASELINEFUNCTIONALFORM, Ethnicity,l.GEOGRAPHICAREANAME 
from CRFunctions a
join ENDPOINTGROUPS b on a.ENDPOINTGROUPID = b.ENDPOINTGROUPID
join ENDPOINTS c on a.ENDPOINTID = c.ENDPOINTID
join POLLUTANTS d on a.POLLUTANTID = d.POLLUTANTID
join METRICS e on a.METRICID = e.METRICID
left join SEASONALMETRICS f on a.SEASONALMETRICID = f.SEASONALMETRICID
join FUNCTIONALFORMS g on a.FUNCTIONALFORMID = g.FUNCTIONALFORMID
left join INCIDENCEDATASETS h_i on a.INCIDENCEDATASETID = h_i.INCIDENCEDATASETID
left join INCIDENCEDATASETS h_p on a.PREVALENCEDATASETID = h_p.INCIDENCEDATASETID
left join SETUPVARIABLEDATASETS i on a.VARIABLEDATASETID = i.SETUPVARIABLEDATASETID
join BASELINEFUNCTIONALFORMS j on a.BASELINEFUNCTIONALFORMID = j.FUNCTIONALFORMID
left join GEOGRAPHICAREAS l on a.GEOGRAPHICAREAID = l.GEOGRAPHICAREAID
where crfunctiondatasetid in (select crfunctiondatasetid from crFunctionDatasets where {0})", sqlWhereClause);
                // setupid=1 and CrFunctionDatasetName='EPA Standard Health Functions
                //crfunctiondatasetid in (select crfunctiondatasetid from crFunctionDatasets where setupid=1 and crfunctiondatasetname = 'Expert PM25 Functions'


                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                lbProcess.Text = "Exporting File...";
                lbProcess.Refresh();
                this.Refresh();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Endpoint Group"] = dr["ENDPOINTGROUPNAME"];
                    newdr["Endpoint"] = dr["ENDPOINTNAME"];
                    newdr["Pollutant"] = dr["POLLUTANTNAME"];
                    newdr["Metric"] = dr["METRICNAME"];
                    newdr["Seasonal Metric"] = dr["SEASONALMETRICNAME"];
                    newdr["Metric Statistic"] = OutputCommonClass.getMetricStastic(Convert.ToInt32(dr["Metricstatistic"]));
                    newdr["Study Author"] = dr["Author"];
                    newdr["Study Year"] = dr["Yyear"];
                    newdr["Geographic Area"] = dr["GEOGRAPHICAREANAME"];
                    newdr["Study Location"] = dr["Location"];
                    newdr["Other Pollutants"] = dr["Otherpollutants"];
                    newdr["Qualifier"] = dr["Qualifier"];
                    newdr["Reference"] = dr["Reference"];
                    newdr["Race"] = dr["Race"];
                    newdr["Ethnicity"] = dr["Ethnicity"];
                    newdr["Gender"] = dr["Gender"];
                    newdr["Start Age"] = dr["Startage"];
                    newdr["End Age"] = dr["Endage"];
                    newdr["Function"] = dr["FUNCTIONALFORM"];
                    newdr["Baseline Function"] = dr["BASELINEFUNCTIONALFORM"];
                    newdr["Beta"] = dr["Beta"];
                    newdr["Distribution Beta"] = dr["Distbeta"];
                    newdr["Parameter 1 Beta"] = dr["P1Beta"];
                    newdr["Parameter 2 Beta"] = dr["P2Beta"];
                    newdr["A"] = dr["A"];
                    newdr["Name A"] = dr["Namea"];
                    newdr["B"] = dr["B"];
                    newdr["Name B"] = dr["Nameb"];
                    newdr["C"] = dr["C"];
                    newdr["Name C"] = dr["Namec"];
                    newdr["Incidence DataSet"] = dr["INCIDENCEDATASET"];
                    newdr["Prevalence DataSet"] = dr["PREVALENCEDATASET"];
                    newdr["Variable DataSet"] = dr["SETUPVARIABLEDATASETNAME"];
                    dtOut.Rows.Add(newdr);
                }

                SaveCSVOrExcel(dtOut, fileName);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
            }
        }

        private int writeVariableFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                string commandText = string.Empty;
                int ret = 0;
                // Group by variable
                commandText = string.Format(@"select b.SETUPVARIABLEID, b.SETUPVARIABLENAME, count(*) record_count
                    from SETUPVARIABLEDATASETS a
                    join SETUPVARIABLES b on a.SETUPVARIABLEDATASETID = b.SETUPVARIABLEDATASETID
                    join SETUPGEOGRAPHICVARIABLES c on b.SETUPVARIABLEID = c.SETUPVARIABLEID
                    where ({0})
                    group by 1, 2
                    order by 2", sqlWhereClause);

                FirebirdSql.Data.FirebirdClient.FbDataReader fbGroupDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                while (fbGroupDataReader.Read())
                {
                    lbProcess.Text = string.Format("Exporting {0} Variable...", fbGroupDataReader["SETUPVARIABLENAME"].ToString());
                    //pBarExport.Value = 0;
                    //pBarExport.Maximum = Convert.ToInt32(fbGroupDataReader["record_count"]);
                    lbProcess.Refresh();
                    this.Refresh();

                    DataTable dtOut = new DataTable();
                    dtOut.Columns.Add("Column", typeof(int));
                    dtOut.Columns.Add("Row", typeof(int));
                    dtOut.Columns.Add(fbGroupDataReader["SETUPVARIABLENAME"].ToString(), typeof(double));

                    commandText = string.Format(@"select Ccolumn, Row, VValue
                    from SETUPVARIABLEDATASETS a
                    join SETUPVARIABLES b on a.SETUPVARIABLEDATASETID = b.SETUPVARIABLEDATASETID
                    join SETUPGEOGRAPHICVARIABLES c on b.SETUPVARIABLEID = c.SETUPVARIABLEID
                    where ({0})
                    and c.setupvariableid = {1}
                    order by 1, 2", sqlWhereClause, Convert.ToInt32(fbGroupDataReader["setupvariableid"]));

                    FirebirdSql.Data.FirebirdClient.FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    while (fbDataReader.Read())
                    {
                        DataRow dr = dtOut.NewRow();
                        dr["Column"] = Convert.ToInt32(fbDataReader["Ccolumn"]);
                        dr["Row"] = Convert.ToDouble(fbDataReader["Row"]);
                        dr[fbGroupDataReader["SETUPVARIABLENAME"].ToString()] = Convert.ToDouble(fbDataReader["VValue"]);
                        dtOut.Rows.Add(dr);

                    }
                    ret++;
                    SaveCSVOrExcel(dtOut, AddSuffix(fileName, MakeValidFileName(String.Format("_{0}", fbGroupDataReader["setupvariablename"].ToString()))));

                }
                return ret;
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
                return 0;
            }
        }

        private void writeInflationFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                lbProcess.Text = "Querying Database...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Empty;
                //commandText = string.Format("select count(*) from InflationEntries where InflationDatasetID in (select InflationDatasetID from InflationDatasets where {0})", sqlWhereClause);

                //Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //pBarExport.Maximum = count;
                //lbProcess.Refresh();

                DataTable dtOut = new DataTable();

                dtOut.Columns.Add("Year", typeof(int));
                dtOut.Columns.Add("AllGoodsIndex", typeof(double));
                dtOut.Columns.Add("MedicalCostIndex", typeof(double));
                dtOut.Columns.Add("WageIndex", typeof(double));


                commandText = string.Format(@"
select YYear, AllGoodsIndex, MedicalCostIndex, WageIndex 
from INFLATIONENTRIES 
where InflationDatasetID in (select InflationDatasetID from InflationDatasets where {0})
order by 1", sqlWhereClause);

                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                lbProcess.Text = "Exporting File...";
                lbProcess.Refresh();
                this.Refresh();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Year"] = Convert.ToInt32(dr["YYear"]);
                    newdr["AllGoodsIndex"] = Convert.ToDouble(dr["AllGoodsIndex"]);
                    newdr["MedicalCostIndex"] = Convert.ToDouble(dr["MedicalCostIndex"]);
                    newdr["WageIndex"] = Convert.ToDouble(dr["WageIndex"]);
                    dtOut.Rows.Add(newdr);
                }

                SaveCSVOrExcel(dtOut, fileName);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
            }
        }

        private void writeValuationFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                lbProcess.Text = "Querying Database...";
                lbProcess.Refresh();
                this.Refresh();
                string commandText = string.Empty;
                /*
                commandText = string.Format(@"
select count(*)
from VALUATIONFUNCTIONS a
join VALUATIONFUNCTIONDATASETS e on a.VALUATIONFUNCTIONDATASETID = e.VALUATIONFUNCTIONDATASETID where {0}", sqlWhereClause);
                Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                pBarExport.Maximum = count;
                lbProcess.Refresh();
                */
                DataTable dtOut = new DataTable();
                dtOut.Columns.Add("Endpoint Group", typeof(string));
                dtOut.Columns.Add("Endpoint", typeof(string));
                dtOut.Columns.Add("Qualifier", typeof(string));
                dtOut.Columns.Add("Reference", typeof(string));
                dtOut.Columns.Add("Start Age", typeof(int));
                dtOut.Columns.Add("End Age", typeof(int));
                dtOut.Columns.Add("Function", typeof(string));
                dtOut.Columns.Add("A", typeof(double));
                dtOut.Columns.Add("Name A", typeof(string));
                dtOut.Columns.Add("Distribution A", typeof(string));
                dtOut.Columns.Add("Parameter 1 A", typeof(double));
                dtOut.Columns.Add("Parameter 2 A", typeof(double));
                dtOut.Columns.Add("B", typeof(double));
                dtOut.Columns.Add("Name B", typeof(string));
                dtOut.Columns.Add("C", typeof(double));
                dtOut.Columns.Add("Name C", typeof(string));
                dtOut.Columns.Add("D", typeof(double));
                dtOut.Columns.Add("Name D", typeof(string));

                commandText = string.Format(@"
select a.Valuationfunctionid,a.Valuationfunctiondatasetid
, b.ENDPOINTGROUPNAME, c.ENDPOINTNAME
, Qualifier,Reference,Startage,Endage
 ,d.FUNCTIONALFORMTEXT
, A, Namea, Dista, P1A,P2A, B, Nameb, C, Namec, D, Named 
from VALUATIONFUNCTIONS a
join ENDPOINTGROUPS b on a.ENDPOINTGROUPID = b.ENDPOINTGROUPID
join ENDPOINTS c on a.ENDPOINTID = c.ENDPOINTID
join VALUATIONFUNCTIONALFORMS d on a.FUNCTIONALFORMID = d.FUNCTIONALFORMID
join VALUATIONFUNCTIONDATASETS e on a.VALUATIONFUNCTIONDATASETID = e.VALUATIONFUNCTIONDATASETID
where {0}", sqlWhereClause);


                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                lbProcess.Text = "Exporting File...";
                lbProcess.Refresh();
                this.Refresh();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Endpoint Group"] = dr["endpointGroupName"].ToString();
                    newdr["Endpoint"] = dr["endpointName"].ToString();
                    newdr["Qualifier"] = dr["Qualifier"].ToString();
                    newdr["Reference"] = dr["Reference"].ToString();
                    newdr["Start Age"] = Convert.ToInt32(dr["StartAge"]);
                    newdr["End Age"] = Convert.ToInt32(dr["EndAge"]);
                    newdr["Function"] = dr["FunctionalformText"].ToString();
                    newdr["A"] = Convert.ToDouble(dr["A"]);
                    newdr["Name A"] = dr["Namea"].ToString();
                    newdr["Distribution A"] = dr["Dista"].ToString();
                    newdr["Parameter 1 A"] = Convert.ToDouble(dr["P1A"]);
                    newdr["Parameter 2 A"] = Convert.ToDouble(dr["P2A"]);
                    newdr["B"] = Convert.ToDouble(dr["B"]);
                    newdr["Name B"] = dr["Nameb"].ToString();
                    newdr["C"] = Convert.ToDouble(dr["C"]);
                    newdr["Name C"] = dr["Namec"].ToString();
                    newdr["D"] = Convert.ToDouble(dr["D"]);
                    newdr["Name D"] = dr["Named"].ToString();
                    dtOut.Rows.Add(newdr);
                }

                SaveCSVOrExcel(dtOut, fileName);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
            }
        }


        private void writeIncomeGrowthFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                lbProcess.Text = "Querying Database...";
                lbProcess.Refresh();
                this.Refresh();
                string commandText = string.Empty;
                /*
                commandText = string.Format(@"
select count(*)
FROM INCOMEGROWTHADJFACTORS a
join INCOMEGROWTHADJDATASETS b on a.INCOMEGROWTHADJDATASETID = b.INCOMEGROWTHADJDATASETID where {0}", sqlWhereClause);
                Int32 count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                pBarExport.Maximum = count;
                lbProcess.Refresh();
                */
                DataTable dtOut = new DataTable();
                dtOut.Columns.Add("Year", typeof(int));
                dtOut.Columns.Add("Mean", typeof(double));
                dtOut.Columns.Add("EndpointGroup", typeof(string));


                commandText = string.Format(@"
select endpointGroups, YYear, Mean
FROM INCOMEGROWTHADJFACTORS a
join INCOMEGROWTHADJDATASETS b on a.INCOMEGROWTHADJDATASETID = b.INCOMEGROWTHADJDATASETID where {0}", sqlWhereClause);


                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                lbProcess.Text = "Exporting File...";
                lbProcess.Refresh();
                this.Refresh();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Year"] = Convert.ToInt32(dr["YYear"]);
                    newdr["Mean"] = Convert.ToDouble(dr["Mean"]);
                    newdr["EndpointGroup"] = dr["endpointGroups"].ToString();
                    dtOut.Rows.Add(newdr);
                }

                SaveCSVOrExcel(dtOut, fileName);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
            }
        }

        private void writePollutantFile(string sqlWhereClause, ESIL.DBUtility.FireBirdHelperBase fb, string fileName)
        {
            try
            {

                lbProcess.Text = "Querying Database...";
                lbProcess.Refresh();
                this.Refresh();
                string commandText = string.Empty;
                //pBarExport.Maximum = 100; //TODO: What is reasonable here?
                //lbProcess.Refresh();
                DateTime dt = new DateTime(2011, 1, 1);
                int i;
                int j;
                DataTable dtOut = new DataTable();
                dtOut.Columns.Add("Label", typeof(string));
                dtOut.Columns.Add("Value", typeof(string));

                // Pollutant Info
                commandText = string.Format(@"
SELECT a.POLLUTANTNAME, a.OBSERVATIONTYPE
FROM POLLUTANTS a
where {0}", sqlWhereClause);

                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                lbProcess.Text = "Exporting File...";
                this.Refresh();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Label"] = "Pollutant";
                    newdr["Value"] = dr["POLLUTANTNAME"].ToString();
                    dtOut.Rows.Add(newdr);

                    newdr = dtOut.NewRow();
                    newdr["Label"] = "Observation Type";
                    newdr["Value"] = (((ObservationtypeEnum)Convert.ToInt32(dr["OBSERVATIONTYPE"]) == ObservationtypeEnum.Daily) ? "Daily" : "Hourly");
                    dtOut.Rows.Add(newdr);
                }

                // Seasons
                commandText = string.Format(@"
SELECT startday, endday
FROM POLLUTANTS a
join POLLUTANTSEASONS b on a.POLLUTANTID = b.POLLUTANTID
where {0}
order by 1, 2", sqlWhereClause);

                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                this.Refresh();
                i = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Label"] = "Season" + i++;
                    newdr["Value"] = dt.AddDays(Convert.ToInt32(dr["STARTDAY"])).GetDateTimeFormats('M')[0].ToString() + "-" + dt.AddDays(Convert.ToInt32(dr["ENDDAY"])).GetDateTimeFormats('M')[0].ToString();
                    dtOut.Rows.Add(newdr);
                }

                // Metrics
                commandText = string.Format(@"
SELECT METRICID, METRICNAME
FROM POLLUTANTS a
join METRICS b on a.POLLUTANTID = b.POLLUTANTID
where {0}", sqlWhereClause);

                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                this.Refresh();
                i = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Label"] = "Metric" + i++;
                    newdr["Value"] = dr["METRICNAME"].ToString();
                    dtOut.Rows.Add(newdr);


                    // Seasonal Metrics
                    commandText = string.Format(@"
SELECT SEASONALMETRICNAME
FROM POLLUTANTS a
join METRICS b on a.POLLUTANTID = b.POLLUTANTID
join SEASONALMETRICS c on b.METRICID = c.METRICID
where {0}
and b.METRICID = {1}", sqlWhereClause, Convert.ToInt32(dr["METRICID"]) );

                    FirebirdSql.Data.FirebirdClient.FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);

                    j = 0;
                    while (fbDataReader.Read())
                    {
                        DataRow newdr2 = dtOut.NewRow();
                        newdr2["Label"] = "SeasonalMetric" + j++;
                        newdr2["Value"] = fbDataReader["SEASONALMETRICNAME"].ToString();
                        dtOut.Rows.Add(newdr2);
                    }

                }

                SaveCSVOrExcel(dtOut, fileName);
            }
            catch (Exception ex)
            {
                errorOccur = true;
                Logger.LogError(ex.Message);
            }
        }
        public void SaveCSVOrExcel(DataTable dt, string fileName)
        {
            pBarExport.Value = 0;
            pBarExport.Maximum = dt.Rows.Count;

            if (Path.GetExtension(fileName) == ".csv")
            {
                SaveCSV(dt, fileName);
            }
            else
            {
                SaveExcel(dt, fileName);
            }
        }

        public void SaveCSV(DataTable dt, string fileName)
        {
           
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    // If the value contains a comma, wrap it in quotes
                    // If the value contains a double-quote, escape it by doubling it
                    if (dt.Rows[i][j].ToString().Contains(","))
                    {
                        data += "\"" + dt.Rows[i][j].ToString().Replace("\"", "\"\"") + "\"";
                    }
                    else
                    {
                        data += dt.Rows[i][j].ToString().Replace("\"", "\"\"");
                    }
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                pBarExport.PerformStep();
            }

            sw.Close();
            fs.Close();
        }

        public void SaveExcel(DataTable dt, string fileName)
        {

            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet1"
            };
            sheets.Append(sheet);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Cell cell = GetCell(worksheetPart.Worksheet, ColumnNameFromIndex(Convert.ToUInt32(i + 1)), 1);
                cell.CellValue = new CellValue(dt.Columns[i].ColumnName.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.String);

            }
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Cell cell = GetCell(worksheetPart.Worksheet, ColumnNameFromIndex(Convert.ToUInt32(j+1)), Convert.ToUInt32(i+2));  
                    cell.CellValue = new CellValue(dt.Rows[i][j].ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.String);
                }
                //lbProcess.Text = i + "/" + dt.Rows.Count;
                //lbProcess.Refresh();
                pBarExport.PerformStep();
            }
            
            worksheetPart.Worksheet.Save();
            workbookPart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
   
        }

        private Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row;
            string cellReference = columnName + rowIndex;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            if (row == null)
            {
                return null;
            }

            if (row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (String.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell()
                {
                    CellReference = cellReference
                };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
        }

        public static string ColumnNameFromIndex(uint columnIndex)
        {
            uint remainder;
            string columnName = "";

            while (columnIndex > 0)
            {
                remainder = (columnIndex - 1) % 26;
                columnName = System.Convert.ToChar(65 + remainder).ToString() + columnName;
                columnIndex = (uint)((columnIndex - remainder) / 26);
            }

            return columnName;
        }
    
        private void writeGriddefinitionFile(string setupid, ESIL.DBUtility.FireBirdHelperBase fb, string targetSHPFilePath)
        {
            try
            {
                pBarExport.Value = 0;
                lbProcess.Text = "Exporting grid definitions...";
                lbProcess.Refresh();
                this.Refresh();

                string commandText = string.Format("select count(*) from griddefinitions where {0}", setupid);
                Int32 drgriddefinitionscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (drgriddefinitionscount == 0)
                {
                    pBarExport.Value = pBarExport.Maximum;
                    lbProcess.Refresh();
                    this.Refresh();
                    return;
                }
                pBarExport.Maximum = drgriddefinitionscount;

                pBarExport.Value = 0;
                lbProcess.Text = "Exporting shapefiles...";
                lbProcess.Refresh();
                this.Refresh();

                commandText = string.Format("select GriddefinitionID,Ttype from griddefinitions where {0}", setupid);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string shapefilename = "";
                    switch (Convert.ToInt16(ds.Tables[0].Rows[i]["TTYPE"]))
                    {
                        case 0:
                            // TODO: How will we handle this type?
                            MessageBox.Show("This type of grid definition is not supported for export.", "Unsupported Format");
                            errorOccur = true;
                            return;
                        case 1:
                            commandText = string.Format("select shapefilename from shapefilegriddefinitiondetails where GriddefinitionID ={0}", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                            shapefilename = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                            
                            break;
                    }

                    commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", Convert.ToInt16(ds.Tables[0].Rows[i]["GRIDDEFINITIONID"]));
                    string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    string sourceFilePath = "";

                    sourceFilePath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx";
                    if (File.Exists(sourceFilePath))
                    {
                        File.Copy(sourceFilePath, Path.ChangeExtension(targetSHPFilePath, ".shx"), true);
                    }

                    sourceFilePath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp";
                    if (File.Exists(sourceFilePath))
                    {
                        File.Copy(sourceFilePath, targetSHPFilePath, true);
                    }

                    sourceFilePath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf";
                    if (File.Exists(sourceFilePath))
                    {
                        File.Copy(sourceFilePath, Path.ChangeExtension(targetSHPFilePath, ".dbf"), true);
                    }

                    sourceFilePath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".prj";
                    if (File.Exists(sourceFilePath))
                    {
                        File.Copy(sourceFilePath, Path.ChangeExtension(targetSHPFilePath, ".prj"), true);
                    }
                    pBarExport.PerformStep();

                }

                lbProcess.Refresh();
                this.Refresh();
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private static Dictionary<int, string> getMetric()
        {
            try
            {
                Dictionary<int, string> dicMetric = new Dictionary<int, string>();
                string commandText = "select MetricID,MetricName from Metrics ";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicMetric.Keys.Contains(Convert.ToInt32(dr["MetricID"])))
                        dicMetric.Add(Convert.ToInt32(dr["MetricID"]), dr["MetricName"].ToString());
                }
                return dicMetric;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        private static Dictionary<int, string> getSeasonalMetric()
        {
            try
            {
                Dictionary<int, string> dicSMetric = new Dictionary<int, string>();
                string commandText = "select SeasonalMetricID,SeasonalMetricName from SeasonalMetrics "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicSMetric.Keys.Contains(Convert.ToInt32(dr["SeasonalMetricID"])))
                        dicSMetric.Add(Convert.ToInt32(dr["SeasonalMetricID"]), dr["SeasonalMetricName"].ToString());
                }
                return dicSMetric;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        private string getStringFromID(int id, Dictionary<int, string> dic)
        {
            try
            {
                string result = string.Empty;
                foreach (int s in dic.Keys)
                {
                    if (s == id)
                        result = dic[s];
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
