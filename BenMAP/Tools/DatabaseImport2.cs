using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using JR.Utils.GUI.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace BenMAP
{
    public partial class DatabaseImport2 : FormBase
    {
        // These global variables are used by the various functions to relink dependencies since the IDs are changed during import
        // The key is the new ID and the value is the new ID
        Dictionary<int, int> gdicGridDefinition = new Dictionary<int, int>();
        Dictionary<int, int> gdicGeographicArea = new Dictionary<int, int>();
        Dictionary<int, int> gdicPollutant = new Dictionary<int, int>();
        Dictionary<int, int> gdicIncidence = new Dictionary<int, int>();
        Dictionary<int, int> gdicPrevalence = new Dictionary<int, int>();
        Dictionary<int, int> gdicVariable = new Dictionary<int, int>();
        Dictionary<int, int> gdicMetric = new Dictionary<int, int>();
        Dictionary<int, int> gdicSeasonalMetric = new Dictionary<int, int>();

        public DatabaseImport2()
        {
            InitializeComponent();
        }

        private void DatabaseImport_Load(object sender, EventArgs e)
        {
            SetupName();
            lblTarget.Enabled = false;
            cboSetup.Enabled = false;
            txtFile.Text = "";
            pBarImport.Value = 0;
            lbProcess.Text = "";
            lbPhase.Text = "";
            lstSetupID = new Dictionary<int, int>();
        }

        private void SetupName()
        {
            try
            {
                cboSetup.Items.Clear();
                string commandText = "select SetupName from Setups";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                System.Data.DataSet dsSetups = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                for (int i = 0; i < dsSetups.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = dsSetups.Tables[0].Rows[i];
                    cboSetup.Items.Add(Convert.ToString(dr[0]));
                }

                cboSetup.SelectedIndex = -1;
            }
            catch (Exception)
            { }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.InitialDirectory = CommonClass.ResultFilePath;
            openfile.Filter = "BenMAPCEDatabase(*.bdbx)|*.bdbx";
            openfile.FilterIndex = 1;
            openfile.RestoreDirectory = true;
            // 2015 02 16 - added initial directory
            openfile.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files\Exports";
            if (openfile.ShowDialog() != DialogResult.OK)
            { return; }
            txtFile.Text = openfile.FileName;
            lblTarget.Enabled = true;
            cboSetup.Enabled = true;
        }

        bool errorOccur = false;
        String strImportLog = "";
        String fileFormat = "";
        private void btnOK_Click(object sender, EventArgs e)
        {
            errorOccur = false;
            if (cboSetup.Text == "") return;
            CommonClass.Connection = CommonClass.getNewConnection();

            // The import file is completely traversed twice.
            // Phase = 1 is the "scan" phase where we see what the file contains and determine how each dataset will be handled. This information is reported to the user and they get to decide if they want to proceed.
            // Phase = 2 is when the import file is committed to the database
            for (int currentPhase = 1; currentPhase <= 2; currentPhase++)
            {
                pBarImport.Value = 0;

                strImportLog = "";
                // Reset the global objects because they are reused for each phase in each import
                gdicGridDefinition = new Dictionary<int, int>();
                gdicGeographicArea = new Dictionary<int, int>();
                gdicPollutant = new Dictionary<int, int>();
                gdicIncidence = new Dictionary<int, int>();
                gdicPrevalence = new Dictionary<int, int>();
                gdicVariable = new Dictionary<int, int>();
                gdicMetric = new Dictionary<int, int>();
                gdicSeasonalMetric = new Dictionary<int, int>();

                using (Stream stream = new FileStream(txtFile.Text, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        //Peek ahead and use the first table name to determine the file format
                        string tableName = reader.ReadString();
                        switch (tableName)
                        {
                            // New Format
                            case "griddefinitions3":
                            case "CrFunctionDatasets3":
                                fileFormat = "1.5 or later";
                                lbPhase.Text = "Pass " + currentPhase + " of 2. " + (currentPhase == 1 ? "Scanning" : "Importing") + " file version " + fileFormat + ".";
                                reader.BaseStream.Position = reader.BaseStream.Position - tableName.Length - 1;
                                break;
                            case "griddefinitions2":
                            case "pollutants2":
                            case "MonitorDataSets2":
                            case "IncidenceDatasets2":
                            case "PopulationConfigurations2":
                            case "CrFunctionDatasets2":
                            case "SetupVariableDatasets2":
                            case "InflationDatasets2":
                            case "ValuationFunctionDatasets2":
                            case "IncomeGrowthAdjDatasets2":
                                fileFormat = "1.4 or later";
                                lbPhase.Text = "Pass " + currentPhase + " of 2. " + (currentPhase==1 ? "Scanning" : "Importing") + " file version " + fileFormat + ".";
                                reader.BaseStream.Position = reader.BaseStream.Position - tableName.Length - 1;
                                break;
                            default:
                                fileFormat = "1.3 or earlier";
                                lbPhase.Text = "Pass " + currentPhase + " of 2. " + (currentPhase == 1 ? "Scanning" : "Importing") + " file version " + fileFormat + ".";
                                reader.BaseStream.Position = reader.BaseStream.Position - tableName.Length - 1;
                                break;
                        }


                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            tableName = reader.ReadString();
                            switch (tableName)
                            {
                                // New Format
                                case "griddefinitions3":
                                    ReadGriddefinition2(currentPhase, reader, 3);
                                    break;
                                case "griddefinitions2":
                                    ReadGriddefinition2(currentPhase, reader, 2);
                                    break;
                                case "pollutants2":
                                    ReadPollutant2(currentPhase, reader);
                                    break;
                                case "MonitorDataSets2":
                                    ReadMonitor2(currentPhase, reader);
                                    break;
                                case "IncidenceDatasets2":
                                    ReadIncidence2(currentPhase, reader);
                                    break;
                                case "PopulationConfigurations2":
                                    ReadPopulation2(currentPhase, reader);
                                    break;
                                case "CrFunctionDatasets3":
                                    ReadCRFunction2(currentPhase, reader, 3);
                                    break;
                                case "CrFunctionDatasets2":
                                    ReadCRFunction2(currentPhase, reader, 2);
                                    break;
                                case "SetupVariableDatasets2":
                                    ReadVariable2(currentPhase, reader);
                                    break;
                                case "InflationDatasets2":
                                    ReadInflation2(currentPhase, reader);
                                    break;
                                case "ValuationFunctionDatasets2":;
                                    ReadValuation2(currentPhase, reader);
                                    break;
                                case "IncomeGrowthAdjDatasets2":
                                    ReadIncomeGrowth2(currentPhase, reader);
                                    break;
                                // Old Format
                                case "setups":
                                    ReadAll(currentPhase, reader);
                                    break;
                                case "OneSetup":
                                    ReadOnesetup(currentPhase, reader);
                                    break;
                                case "PopulationGriddefinitions":
                                    ReadPopulation(currentPhase, reader);
                                    break;
                                case "IncidenceGriddefinitions":
                                    ReadIncidence(currentPhase, reader);
                                    break;
                                case "griddefinitions":
                                    ReadGriddefinition(currentPhase, reader);
                                    break;
                                case "pollutants":
                                    ReadPollutant(currentPhase, reader);
                                    break;
                                case "MonitorDataSets":
                                    ReadMonitor(currentPhase, reader);
                                    break;
                                case "IncidenceDatasets":
                                    ReadIncidence2(currentPhase, reader);
                                    break;
                                case "PopulationConfigurations":
                                    ReadPopulation2(currentPhase, reader);
                                    break; 
                                case "CrFunctionDatasets":
                                    ReadCRFunction(currentPhase, reader);
                                    break;
                                case "SetupVariableDatasets":
                                    ReadVariable(currentPhase, reader);
                                    break;
                                case "InflationDatasets":
                                    ReadInflation(currentPhase, reader);
                                    break;
                                case "ValuationFunctionDatasets":
                                    ReadValuation(currentPhase, reader);
                                    break;
                                case "IncomeGrowthAdjDatasets":
                                    ReadIncomeGrowth(currentPhase, reader);
                                    break;
                            }
                        }

                        reader.Close();

                        if (errorOccur)
                        {
                            MessageBox.Show("Error. The import process was interrupted.", "Error");
                            CommonClass.Connection = CommonClass.getNewConnection();
                        }
                        else
                        {
                            // If we're in the "scan" phase, show the expected changes and ask the user if they wish to continue
                            if(currentPhase == 1)
                            {
                                lbProcess.Text = "";
                                if (FlexibleMessageBox.Show("The following changes will be made to your database. Do you want to continue?\n" + strImportLog + "\n\nThe import process will not overwrite existing datasets having the same name.", "Database Import", MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button3) != DialogResult.Yes)
                                {
                                    pBarImport.Value = 0;
                                    lbProcess.Text = "Import canceled";
                                    lbPhase.Text = "";
                                    lbProcess.Refresh();
                                    this.Refresh();
                                    return;
                                }
                            }
                            // Show the outcome of the "import" phase
                            else
                            {
                                FlexibleMessageBox.Show("The import is complete. Please review the change log below:\n" + strImportLog, "File Imported");
                            }

                        }
                        if(currentPhase == 2)
                        {
                            DatabaseImport_Load(sender, e);
                            btnCancel.Text = "Close";
                        }
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        int importsetupID = 0;
        private void cboSetup_SelectedIndexChanged(object sender, EventArgs e)
        {
            string setupName = cboSetup.Text;
            string commandText = string.Format("select SetupId from Setups where SetupName='{0}'", setupName);
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            importsetupID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
        }

        private void ReadGriddefinition(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Grid definitions...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, int> dicPercentageID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
                    int origGridDefinitionID = griddefinitionID;

                    int oldSetupid = reader.ReadInt32();
                    string griddefinitionName = reader.ReadString();
                    int tmpColumns = reader.ReadInt32();
                    int tmpRrows = reader.ReadInt32();
                    int tmpTtype = reader.ReadInt32();
                    int tmpDefaulttype = reader.ReadInt32();
                    string commandText = string.Format("select GriddefinitionID from griddefinitions where setupid={0} and GriddefinitionName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existgriddefinitionID = Convert.ToInt16(obj);

                        dicGriddefinitionID.Add(griddefinitionID, existgriddefinitionID);
                        //reader.BaseStream.Position = reader.BaseStream.Position + 4 * sizeof(Int32);
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origGridDefinitionID, false);
                    }
                    else
                    {
                        commandText = string.Format("select GriddefinitionID from griddefinitions where GriddefinitionID={0}", griddefinitionID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(GriddefinitionID) from griddefinitions";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicGriddefinitionID.Add(griddefinitionID, 1);
                                griddefinitionID = 1;
                            }
                            else
                            {
                                int maxGriddefinitionID = Convert.ToInt16(obj);
                                dicGriddefinitionID.Add(griddefinitionID, maxGriddefinitionID + 1);
                                griddefinitionID = maxGriddefinitionID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" was imported";
                            //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                            commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", griddefinitionID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, tmpColumns, tmpRrows, tmpTtype, tmpDefaulttype);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        dicDoImport.Add(origGridDefinitionID, true);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Shapefiles...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) return;
                for (int j = 0; j < tableCount; j++)
                {
                    int GriddefinitionID = reader.ReadInt16();
                    string commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID);
                    string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    string filetype = reader.ReadString();
                    string shapefilename = reader.ReadString();
                    switch (filetype)
                    {
                        case "regular":
                            commandText = string.Format("insert into regulargriddefinitiondetails(GriddefinitionID,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude,ShapefileName) values({0},{1},{2},{3},{4},'{5}')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, reader.ReadSingle(), reader.ReadSingle(), reader.ReadInt16(), reader.ReadInt16(), shapefilename);
                            if (currentPhase == 2 && dicDoImport[GriddefinitionID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                            break;
                        case "shapefile":
                            //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                            // 2015 02 12 added Locked to field list
                            commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename,LOCKED) values({0},'{1}', 'F')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, shapefilename);
                            if (currentPhase == 2 && dicDoImport[GriddefinitionID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                            break;
                    }
                    if (currentPhase == 2 && dicDoImport[GriddefinitionID])
                    {
                        if (!Directory.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname))
                        {
                            Directory.CreateDirectory(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname);
                        }
                        if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp");
                        FileStream file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Create, FileAccess.Write);
                        Int64 length64 = reader.ReadInt64();
                        Int64 diff = 0;
                        byte[] array;
                        while (diff <= length64 - Int32.MaxValue)
                        {
                            array = reader.ReadBytes(Int32.MaxValue);
                            file.Write(array, 0, Int32.MaxValue);
                            diff += Int32.MaxValue;
                        }
                        array = reader.ReadBytes((Int32)(length64 - diff));
                        file.Write(array, 0, (Int32)(length64 - diff));
                        file.Close();

                        file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Create, FileAccess.Write);
                        length64 = reader.ReadInt64();
                        diff = 0;
                        while (diff <= length64 - Int32.MaxValue)
                        {
                            array = reader.ReadBytes(Int32.MaxValue);
                            file.Write(array, 0, Int32.MaxValue);
                            diff += Int32.MaxValue;
                        }
                        array = reader.ReadBytes((Int32)(length64 - diff));
                        file.Write(array, 0, (Int32)(length64 - diff));
                        file.Close();

                        file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Create, FileAccess.Write);
                        length64 = reader.ReadInt64();
                        diff = 0;
                        while (diff <= length64 - Int32.MaxValue)
                        {
                            array = reader.ReadBytes(Int32.MaxValue);
                            file.Write(array, 0, Int32.MaxValue);
                            diff += Int32.MaxValue;
                        }
                        array = reader.ReadBytes((Int32)(length64 - diff));
                        file.Write(array, 0, (Int32)(length64 - diff));
                        file.Close();
                    }
                    else
                    // We're either in phase 1 and just scanning, or we're not importing the grid definition so we should just blindly scan through the shapefile contents so we can get to the next thing
                    {
                        // We have three segments here for shx, shp, dbf.  Skip over each of them
                        for (int i = 0; i <= 2; i++)
                        {
                            Int64 length64 = reader.ReadInt64();
                            reader.BaseStream.Position += length64;
                        }
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Grid definition percentages...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportPercentage = new Dictionary<int, bool>();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable == "GriddefinitionPercentages")
                {
                    int GriddefinitionPercentagescount = reader.ReadInt32();
                    pBarImport.Maximum = GriddefinitionPercentagescount;
                    for (int i = 0; i < GriddefinitionPercentagescount; i++)
                    {
                        int PercentageID = reader.ReadInt32();
                        int SourceGriddefinitionID = reader.ReadInt32();
                        int TargetGriddefinitionID = reader.ReadInt32();
                        string commandText = string.Format("select PercentageID from GriddefinitionPercentages where PercentageID={0}", PercentageID);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PercentageID) from GriddefinitionPercentages";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicPercentageID.Add(PercentageID, 1); PercentageID = 1; }
                            else
                            {
                                int maxPercentageID = Convert.ToInt16(obj);
                                dicPercentageID.Add(PercentageID, maxPercentageID + 1);
                                PercentageID = maxPercentageID + 1;
                            }
                        }
                        commandText = string.Format("insert into GriddefinitionPercentages(PercentageID,SourceGriddefinitionID,TargetGriddefinitionID) values({0},{1},{2})", PercentageID,
                            dicGriddefinitionID.ContainsKey(SourceGriddefinitionID) ? dicGriddefinitionID[SourceGriddefinitionID] : SourceGriddefinitionID,
                            dicGriddefinitionID.ContainsKey(TargetGriddefinitionID) ? dicGriddefinitionID[TargetGriddefinitionID] : TargetGriddefinitionID);
                        if (currentPhase == 2 && (dicDoImport[SourceGriddefinitionID] || dicDoImport[TargetGriddefinitionID]))
                        {
                            if(dicDoImportPercentage.ContainsKey(PercentageID))
                            {
                                dicDoImportPercentage[PercentageID] = true;
                            } else
                            {
                                dicDoImportPercentage.Add(PercentageID, true);
                            }

                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        else if ( ! dicDoImportPercentage.ContainsKey(PercentageID))
                        {

                            dicDoImportPercentage.Add(PercentageID, false);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Grid definition percentage entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "GriddefinitionPercentageEntries")
                {
                    int GriddefinitionPercentageEntriescount = reader.ReadInt32();
                    pBarImport.Maximum = GriddefinitionPercentageEntriescount;
                    for (int i = 0; i < (GriddefinitionPercentageEntriescount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        Boolean doBatch = false;
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < GriddefinitionPercentageEntriescount)
                            {
                                int PercentageID = reader.ReadInt32();
                                if (currentPhase == 2 && dicDoImportPercentage[dicPercentageID.ContainsKey(PercentageID) ? dicPercentageID[PercentageID] : PercentageID])
                                {
                                    commandText = commandText + string.Format("insert into GriddefinitionPercentageEntries(PercentageID,SourceColumn,SourceRow,TargetColumn,TargetRow,Percentage,NormalizationState) values({0},{1},{2},{3},{4},{5},{6});", dicPercentageID.ContainsKey(PercentageID) ? dicPercentageID[PercentageID] : PercentageID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), Convert.ToDouble(reader.ReadString()), reader.ReadInt32());
                                    doBatch = true;
                                }
                                else
                                {
                                    // Throw these values away 
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadString();
                                    reader.ReadInt32();
                                }
                                pBarImport.PerformStep();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doBatch)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
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

        private void ReadGriddefinition2(int currentPhase, BinaryReader reader, int fileVersion)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Grid definitions...";
                lbProcess.Refresh();
                this.Refresh();
                int origGridDefinitionID;
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, int> dicPercentageID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
                    origGridDefinitionID = griddefinitionID;
                    int oldSetupid = reader.ReadInt32();
                    string griddefinitionName = reader.ReadString();
                    int tmpColumns = reader.ReadInt32(); 
                    int tmpRrows = reader.ReadInt32(); 
                    int tmpTtype = reader.ReadInt32();
                    int tmpDefaulttype = reader.ReadInt32();

                    string commandText = string.Format("select GriddefinitionID from griddefinitions where setupid={0} and GriddefinitionName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existgriddefinitionID = Convert.ToInt16(obj);

                        dicGriddefinitionID.Add(griddefinitionID, existgriddefinitionID);
                        gdicGridDefinition.Add(griddefinitionID, existgriddefinitionID);
                        //reader.BaseStream.Position = reader.BaseStream.Position + 4 * sizeof(Int32);
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origGridDefinitionID, false);
                    }
                    else
                    {
                        commandText = string.Format("select GriddefinitionID from griddefinitions where GriddefinitionID={0}", griddefinitionID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(GriddefinitionID) from griddefinitions";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicGriddefinitionID.Add(griddefinitionID, 1);
                                griddefinitionID = 1;
                            }
                            else
                            {
                                int maxGriddefinitionID = Convert.ToInt16(obj);
                                dicGriddefinitionID.Add(griddefinitionID, maxGriddefinitionID + 1);
                                griddefinitionID = maxGriddefinitionID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" was imported";
                            //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                            commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", griddefinitionID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName,tmpColumns, tmpRrows,tmpTtype,tmpDefaulttype);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        dicDoImport.Add(origGridDefinitionID, true);
                        gdicGridDefinition.Add(origGridDefinitionID, griddefinitionID);
                    }
                    
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Shapefiles...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) return;
                for (int j = 0; j < tableCount; j++)
                {
                    int GriddefinitionID = reader.ReadInt16();
                    string commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID);
                    string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    string filetype = reader.ReadString();
                    string shapefilename = reader.ReadString();
                    switch (filetype)
                    {
                        case "regular":
                            commandText = string.Format("insert into regulargriddefinitiondetails(GriddefinitionID,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude,ShapefileName) values({0},{1},{2},{3},{4},'{5}')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, reader.ReadSingle(), reader.ReadSingle(), reader.ReadInt16(), reader.ReadInt16(), shapefilename);
                            if (currentPhase == 2 && dicDoImport[GriddefinitionID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                            break;
                        case "shapefile":
                            //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                            // 2015 02 12 added Locked to field list
                            commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename,LOCKED) values({0},'{1}', 'F')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, shapefilename);
                            if (currentPhase == 2 && dicDoImport[GriddefinitionID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                            break;
                    }

                    if (currentPhase == 2 && dicDoImport[GriddefinitionID])
                    {
                        if (!Directory.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname))
                        {
                            Directory.CreateDirectory(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname);
                        }

                        if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp");

                        FileStream file; 
                        Int64 diff = 0;
                        byte[] array;

                        Int64 length64 = reader.ReadInt64();
                        if (length64 > 0)
                        {
                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Create, FileAccess.Write);
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();
                        }

                        length64 = reader.ReadInt64();
                        if (length64 > 0)
                        {
                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Create, FileAccess.Write);
                            diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();
                        }

                        length64 = reader.ReadInt64();
                        if (length64 > 0)
                        {
                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Create, FileAccess.Write);
                            diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();
                        }

                        length64 = reader.ReadInt64();
                        if (length64 > 0)
                        {
                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".prj", FileMode.Create, FileAccess.Write);
                            diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();
                        }
                    }
                    else
                    // We're either in phase 1 and just scanning, or we're not importing the grid definition so we should just blindly scan through the shapefile contents so we can get to the next thing
                    {
                        // We have four segments here for shx, shp, dbf, and prj.  Skip over each of them
                        for (int i=0; i<=3; i++)
                        {
                            Int64 length64 = reader.ReadInt64();
                            reader.BaseStream.Position += length64;
                        }
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Grid definition percentages...";
                lbProcess.Refresh();
                this.Refresh();

                Dictionary<int, bool> dicDoImportPercentage = new Dictionary<int, bool>();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable == "GriddefinitionPercentages")
                {
                    int GriddefinitionPercentagescount = reader.ReadInt32();
                    pBarImport.Maximum = GriddefinitionPercentagescount;
                    for (int i = 0; i < GriddefinitionPercentagescount; i++)
                    {
                        int PercentageID = reader.ReadInt32();
                        int SourceGriddefinitionID = reader.ReadInt32();
                        int TargetGriddefinitionID = reader.ReadInt32();
                        string commandText = string.Format("select PercentageID from GriddefinitionPercentages where PercentageID={0}", PercentageID);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PercentageID) from GriddefinitionPercentages";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicPercentageID.Add(PercentageID, 1); PercentageID = 1; }
                            else
                            {
                                int maxPercentageID = Convert.ToInt16(obj);
                                dicPercentageID.Add(PercentageID, maxPercentageID + 1);
                                PercentageID = maxPercentageID + 1;
                            }
                        }
                        commandText = string.Format("insert into GriddefinitionPercentages(PercentageID,SourceGriddefinitionID,TargetGriddefinitionID) values({0},{1},{2})", PercentageID,
                            dicGriddefinitionID.ContainsKey(SourceGriddefinitionID) ? dicGriddefinitionID[SourceGriddefinitionID] : SourceGriddefinitionID,
                            dicGriddefinitionID.ContainsKey(TargetGriddefinitionID) ? dicGriddefinitionID[TargetGriddefinitionID] : TargetGriddefinitionID);
                        if (currentPhase == 2 && (dicDoImport[SourceGriddefinitionID] || dicDoImport[TargetGriddefinitionID]))
                        {
                            dicDoImportPercentage.Add(PercentageID, true);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Grid definition percentage entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "GriddefinitionPercentageEntries")
                {
                    int GriddefinitionPercentageEntriescount = reader.ReadInt32();
                    pBarImport.Maximum = GriddefinitionPercentageEntriescount;
                    for (int i = 0; i < (GriddefinitionPercentageEntriescount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        Boolean doBatch = false;
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < GriddefinitionPercentageEntriescount)
                            {
                                int PercentageID = reader.ReadInt32();
                                if(dicDoImportPercentage[dicPercentageID.ContainsKey(PercentageID) ? dicPercentageID[PercentageID] : PercentageID])
                                {
                                    commandText = commandText + string.Format("insert into GriddefinitionPercentageEntries(PercentageID,SourceColumn,SourceRow,TargetColumn,TargetRow,Percentage,NormalizationState) values({0},{1},{2},{3},{4},{5},{6});", dicPercentageID.ContainsKey(PercentageID) ? dicPercentageID[PercentageID] : PercentageID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), Convert.ToDouble(reader.ReadString()), reader.ReadInt32());
                                    doBatch = true;
                                }
                                else
                                {
                                    // Throw these values away 
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadString();
                                    reader.ReadInt32();
                                }
                                pBarImport.PerformStep();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doBatch)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Geographic area entries...";
                lbProcess.Refresh();
                this.Refresh();
                nextTable = reader.ReadString();
                if (nextTable == "GeographicAreas")
                {
                    int geographicAreaCount = reader.ReadInt32();
                    pBarImport.Maximum = geographicAreaCount;
                    for (int i = 0; i < geographicAreaCount; i++)
                    {
                        int geographicAreaId = reader.ReadInt32();
                        string geographicAreaName = reader.ReadString();
                        int gridDefinitionId = reader.ReadInt32();
                        string entireGridDefinition = reader.ReadString();
                        string geographicAreaFeatureId = null;

                        if(fileVersion >= 3)
                        {
                            geographicAreaFeatureId = reader.ReadString();
                        }

                        // Check for a geographic area with the same name. If it's there, use that ID. If not, create it.

                        string commandText = string.Format(@"select GeographicAreaID 
                            from GeographicAreas a
                            join GRIDDEFINITIONS b on a.GRIDDEFINITIONID = b.GRIDDEFINITIONID
                            where a.GeographicAreaName='{0}'
                            and b.setupid = {1}", 
                            geographicAreaName, importsetupID);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            int existingId = Convert.ToInt16(obj);
                            gdicGeographicArea.Add(geographicAreaId, existingId);
                            // We already have a geographic area named after the grid definition in this setup.  Let's realign the grid definition id
                            commandText = string.Format(@"update GEOGRAPHICAREAS a
                                set a.GridDefinitionId = {0}, a.GeographicAreaFeatureIdField = {1}
                                where a.GeographicAreaId={2}", gdicGridDefinition[gridDefinitionId], (geographicAreaFeatureId==null ? "NULL" : "'" + geographicAreaFeatureId + "'"), existingId);
                        } else
                        {
                            commandText = "select max(GeographicAreaID) from GeographicAreas";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            int maxGeographicAreaID = 0;
                            if (!Convert.IsDBNull(obj))
                            {
                                maxGeographicAreaID = Convert.ToInt16(obj);
                            }
                            gdicGeographicArea.Add(geographicAreaId, maxGeographicAreaID + 1);
                            geographicAreaId = maxGeographicAreaID + 1;
                            commandText = string.Format("insert into GeographicAreas(GeographicAreaId,GeographicAreaName,GridDefinitionId,EntireGridDefinition,GeographicAreaFeatureIdField) values({0},'{1}',{2},'{3}', {4})", geographicAreaId,
                                geographicAreaName, gdicGridDefinition[gridDefinitionId], entireGridDefinition, (geographicAreaFeatureId == null ? "NULL" : "'" + geographicAreaFeatureId + "'"));
                        }
                        if (currentPhase == 2 && dicDoImport[gridDefinitionId])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadPollutant(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Pollutants...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int PollutantID = 0;
                
                int existPollutantID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicpollutantid = new Dictionary<int, int>();
                Dictionary<int, int> dicpollutantseasonid = new Dictionary<int, int>();
                Dictionary<int, int> dicMetricID = new Dictionary<int, int>();
                Dictionary<int, int> dicSeasonalMetricID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    string pollutantName = reader.ReadString();
                    PollutantID = reader.ReadInt32();
                    int origPollutantID = PollutantID;
                    int oldSetupid = reader.ReadInt32();
                    string commandText = string.Format("select PollutantID from pollutants where setupid={0} and PollutantName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, pollutantName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        existPollutantID = Convert.ToInt16(obj);

                        dicpollutantid.Add(PollutantID, existPollutantID);
                        PollutantID = existPollutantID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origPollutantID, false);
                    }
                    else
                    {
                        commandText = string.Format("select PollutantID from pollutants where PollutantID={0}", PollutantID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PollutantID) from pollutants";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicpollutantid.Add(PollutantID, 1); PollutantID = 1; }
                            else
                            {
                                int maxPollutantID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                                dicpollutantid.Add(PollutantID, maxPollutantID + 1);
                                PollutantID = maxPollutantID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" was imported";
                        }
                        dicDoImport.Add(origPollutantID, true);
                    }
                    gdicPollutant[origPollutantID] = PollutantID;
                    commandText = string.Format("insert into pollutants(PollutantName,PollutantID,SetupID,ObservationType) values('{0}',{1},{2},{3})", pollutantName, PollutantID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, reader.ReadInt32());
                    if (currentPhase == 2 && dicDoImport[origPollutantID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "PollutantSeasons" && nextTable != "Metrics")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                if (nextTable == "PollutantSeasons")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Pollutant seasons...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int changePollutantSeasonID = 0;
                    int PollutantSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = PollutantSeasonscount;
                    string commandText = "select max(PollutantSeasonID) from PollutantSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changePollutantSeasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < PollutantSeasonscount; i++)
                    {
                        int pollutantseasonid = reader.ReadInt32();
                        PollutantID = reader.ReadInt32();
                        changePollutantSeasonID++;
                        dicpollutantseasonid.Add(pollutantseasonid, changePollutantSeasonID);
                        commandText = string.Format("insert into PollutantSeasons(PollutantSeasonID,PollutantID,StartDay,EndDay,StartHour,EndHour,Numbins) values({0},{1},{2},{3},{4},{5},{6})", changePollutantSeasonID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImport[PollutantID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                    if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                    nextTable = reader.ReadString();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Metrics...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportMetric = new Dictionary<int, bool>();
                int changeMetricID = 0;
                if (nextTable == "Metrics")
                {
                    int Metricscount = reader.ReadInt32();
                    pBarImport.Maximum = Metricscount;
                    string commandText = "select max(MetricID) from Metrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < Metricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        int origMetricID = MetricID;
                        commandText = string.Format("select MetricID from Metrics where MetricID={0}", MetricID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            dicMetricID.Add(MetricID, ++changeMetricID);
                            MetricID = changeMetricID;
                        }
                        PollutantID = reader.ReadInt32();
                        commandText = string.Format("insert into Metrics(MetricID,PollutantID,MetricName,HourlyMetricGeneration) values({0},{1},'{2}',{3})", MetricID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, reader.ReadString(), reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImport[PollutantID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            dicDoImportMetric.Add(origMetricID, true);
                        } else
                        {
                            dicDoImportMetric.Add(origMetricID, false);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "FixedWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Fixed window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int FixedWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = FixedWindowMetricscount;
                    for (int i = 0; i < FixedWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into fixedwindowMetrics(MetricID,Starthour,Endhour,Statistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImportMetric[MetricID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "MovingWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Moving window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int MovingWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = MovingWindowMetricscount;
                    for (int i = 0; i < MovingWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into MovingWindowMetrics(MetricID,Windowsize,Windowstatistic,Dailystatistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImportMetric[MetricID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "CustomMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Custom metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int CustomMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = CustomMetricscount;
                    for (int i = 0; i < CustomMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into CustomMetrics(MetricID,MetricFunction) values({0},'{1}')", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                        if (currentPhase == 2 && dicDoImportMetric[MetricID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metrics...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricID = 0;
                Dictionary<int, bool> dicDoImportSeasonalMetric = new Dictionary<int, bool>();
                if (nextTable == "SeasonalMetrics")
                {
                    int SeasonalMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricscount;
                    string commandText = "select max(SeasonalMetricID) from SeasonalMetrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricscount; i++)
                    {
                        int SeasonalMetricID = reader.ReadInt32();
                        int origSeasonalMetricID = SeasonalMetricID;
                        commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where SeasonalMetricID={0}", SeasonalMetricID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            dicSeasonalMetricID.Add(SeasonalMetricID, ++changeSeasonalMetricID);
                            SeasonalMetricID = changeSeasonalMetricID;
                        }
                        int MetricID = reader.ReadInt32();
                        commandText = string.Format("insert into SeasonalMetrics(SeasonalMetricID,MetricID,SeasonalMetricName) values({0},{1},'{2}')", SeasonalMetricID, dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                        if (currentPhase == 2 && dicDoImportMetric[MetricID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            dicDoImportSeasonalMetric.Add(origSeasonalMetricID, true);
                        } else
                        {
                            dicDoImportSeasonalMetric.Add(origSeasonalMetricID, false);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metric seasons...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricseasonID = 0;
                if (nextTable == "SeasonalMetricSeasons")
                {
                    int SeasonalMetricSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricSeasonscount;
                    string commandText = "select max(SeasonalMetricseasonID) from SeasonalMetricSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricseasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricSeasonscount; i++)
                    {
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        int SeasonalMetricID = reader.ReadInt32();
                        int StartDay = reader.ReadInt32();
                        int EndDay = reader.ReadInt32();
                        int SeasonalMetricType = reader.ReadInt32();
                        string MetricFunction = reader.ReadString();
                        int PollutantseasonID = reader.ReadInt32();
                        commandText = string.Format("insert into SeasonalMetricSeasons(SeasonalMetricseasonID,SeasonalMetricID,StartDay,EndDay,SeasonalMetricType,MetricFunction,PollutantseasonID) values({0},{1},{2},{3},{4},'{5}',{6})", ++changeSeasonalMetricseasonID, dicSeasonalMetricID.ContainsKey(SeasonalMetricID) ? dicSeasonalMetricID[SeasonalMetricID] : SeasonalMetricID, Convert.ToInt16(StartDay), Convert.ToInt16(EndDay), Convert.ToInt16(SeasonalMetricType), MetricFunction, dicpollutantseasonid.ContainsKey(PollutantseasonID) ? dicpollutantseasonid[PollutantseasonID] : PollutantseasonID);
                        if (currentPhase == 2 && dicDoImportSeasonalMetric[SeasonalMetricID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }


        private void ReadPollutant2(int currentPhase, BinaryReader reader)
        {
           
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Pollutants...";
                lbProcess.Refresh();
                this.Refresh();
                int origPollutantID;
                Boolean doImport = false;
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int PollutantID = 0;
                int existPollutantID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicpollutantid = new Dictionary<int, int>();
                Dictionary<int, int> dicpollutantseasonid = new Dictionary<int, int>();
                Dictionary<int, int> dicMetricID = new Dictionary<int, int>();
                Dictionary<int, int> dicSeasonalMetricID = new Dictionary<int, int>();

                for (int i = 0; i < tableCount; i++)
                {
                    string pollutantName = reader.ReadString();
                    PollutantID = reader.ReadInt32();
                    origPollutantID = PollutantID;
                    int oldSetupid = reader.ReadInt32();
                    string commandText = string.Format("select PollutantID from pollutants where setupid={0} and PollutantName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, pollutantName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        existPollutantID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from PollutantSeasons where PollutantID in ({0})", existPollutantID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from Metrics where PollutantID in ({0})", existPollutantID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        // Note this also cascades to deleting the monitors. The export always includes monitor datasets after pollutant datasets to put them back
                        commandText = string.Format("delete from pollutants where PollutantID in ({0})", existPollutantID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicpollutantid.Add(PollutantID, existPollutantID);
                        PollutantID = existPollutantID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        commandText = string.Format("select PollutantID from pollutants where PollutantID={0}", PollutantID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PollutantID) from pollutants";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicpollutantid.Add(PollutantID, 1); PollutantID = 1; }
                            else
                            {
                                int maxPollutantID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                                dicpollutantid.Add(PollutantID, maxPollutantID + 1);
                                PollutantID = maxPollutantID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" was imported";
                        }
                        doImport = true;
                    }
                    // Map the old id to the new so the HIF import can use it
                    gdicPollutant[origPollutantID] = PollutantID;
                    commandText = string.Format("insert into pollutants(PollutantName,PollutantID,SetupID,ObservationType) values('{0}',{1},{2},{3})", pollutantName, PollutantID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, reader.ReadInt32());
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "PollutantSeasons" && nextTable != "Metrics")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                if (nextTable == "PollutantSeasons")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Pollutant seasons...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int changePollutantSeasonID = 0;
                    int PollutantSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = PollutantSeasonscount;
                    string commandText = "select max(PollutantSeasonID) from PollutantSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changePollutantSeasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < PollutantSeasonscount; i++)
                    {
                        int pollutantseasonid = reader.ReadInt32();
                        PollutantID = reader.ReadInt32();
                        changePollutantSeasonID++;
                        dicpollutantseasonid.Add(pollutantseasonid, changePollutantSeasonID);
                        commandText = string.Format("insert into PollutantSeasons(PollutantSeasonID,PollutantID,StartDay,EndDay,StartHour,EndHour,Numbins) values({0},{1},{2},{3},{4},{5},{6})", changePollutantSeasonID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                    if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                    nextTable = reader.ReadString();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Metrics...";
                lbProcess.Refresh();
                this.Refresh();

                int changeMetricID = 0;
                if (nextTable == "Metrics")
                {
                    int Metricscount = reader.ReadInt32();
                    pBarImport.Maximum = Metricscount;
                    string commandText = "select max(MetricID) from Metrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < Metricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        PollutantID = reader.ReadInt32();
                        String metricName = reader.ReadString();
                        int hourlyMetricGen = reader.ReadInt32();

                        // Look to see if this pollutant already has this metric
                        commandText = string.Format("select MetricID from metrics where pollutantid={0} and metricname='{1}'", gdicPollutant[PollutantID], metricName);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            int existMetricID = Convert.ToInt16(obj);
                            if (!gdicMetric.ContainsKey(MetricID))
                            {
                                gdicMetric.Add(MetricID, existMetricID);
                            }
                            MetricID = existMetricID;

                        }
                        // We don't have a metric with this name. See if we can still use the same id.
                        else
                        {
                            commandText = string.Format("select MetricID from Metrics where MetricID={0}", MetricID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicMetricID.Add(MetricID, ++changeMetricID);
                                gdicMetric.Add(MetricID, changeMetricID);
                                MetricID = changeMetricID;
                            }
                            else
                            {
                                if(!gdicMetric.ContainsKey(MetricID) )
                                {
                                    gdicMetric.Add(MetricID, MetricID);
                                }

                            }
                            commandText = string.Format("insert into Metrics(MetricID,PollutantID,MetricName,HourlyMetricGeneration) values({0},{1},'{2}',{3})", MetricID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, metricName,hourlyMetricGen);
                            if (currentPhase == 2) // Not checking doImport since, even if the pollutant is there, we might need to add the metric
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                if (doImport == false)
                                {
                                    strImportLog += "\nMetric \"" + metricName + "\" was imported";
                                }
                            } else
                            {
                                if (doImport == false)
                                {
                                    strImportLog += "\nMetric \"" + metricName + "\" will be imported";
                                }
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "FixedWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Fixed window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int FixedWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = FixedWindowMetricscount;
                    for (int i = 0; i < FixedWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into fixedwindowMetrics(MetricID,Starthour,Endhour,Statistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "MovingWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Moving window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int MovingWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = MovingWindowMetricscount;
                    for (int i = 0; i < MovingWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into MovingWindowMetrics(MetricID,Windowsize,Windowstatistic,Dailystatistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "CustomMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Custom metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int CustomMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = CustomMetricscount;
                    for (int i = 0; i < CustomMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into CustomMetrics(MetricID,MetricFunction) values({0},'{1}')", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metrics...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricID = 0;
                if (nextTable == "SeasonalMetrics")
                {
                    int SeasonalMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricscount;
                    string commandText = "select max(SeasonalMetricID) from SeasonalMetrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricscount; i++)
                    {
                        int SeasonalMetricID = reader.ReadInt32();
                        int MetricID = reader.ReadInt32();
                        string SeasonalMetricName = reader.ReadString();

                        // Look to see if this pollutant already has this metric
                        commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where MetricID={0} and SeasonalMetricName='{1}'", gdicMetric[MetricID], SeasonalMetricName);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            int existSeasonalMetricID = Convert.ToInt16(obj);
                            if (!gdicSeasonalMetric.ContainsKey(SeasonalMetricID))
                            {
                                gdicSeasonalMetric.Add(SeasonalMetricID, existSeasonalMetricID);
                            }
                            SeasonalMetricID = existSeasonalMetricID;

                        }
                        // We don't have a seasonal metric with this name. See if we can still use the same id.
                        else
                        {


                            commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where SeasonalMetricID={0}", SeasonalMetricID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicSeasonalMetricID.Add(SeasonalMetricID, ++changeSeasonalMetricID);
                                gdicSeasonalMetric.Add(SeasonalMetricID, changeSeasonalMetricID);
                                SeasonalMetricID = changeSeasonalMetricID;
                            }
                            else
                            {
                                gdicSeasonalMetric.Add(SeasonalMetricID, SeasonalMetricID);
                            }

                            commandText = string.Format("insert into SeasonalMetrics(SeasonalMetricID,MetricID,SeasonalMetricName) values({0},{1},'{2}')", SeasonalMetricID, dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, SeasonalMetricName);
                            if (currentPhase == 2) // Not checking doImport since, even if the pollutant is there, we might need to add the seasonal metric
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metric seasons...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricseasonID = 0;
                if (nextTable == "SeasonalMetricSeasons")
                {
                    int SeasonalMetricSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricSeasonscount;
                    string commandText = "select max(SeasonalMetricseasonID) from SeasonalMetricSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricseasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricSeasonscount; i++)
                    {
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        int SeasonalMetricID = reader.ReadInt32();
                        int StartDay = reader.ReadInt32();
                        int EndDay = reader.ReadInt32();
                        int SeasonalMetricType = reader.ReadInt32();
                        string MetricFunction = reader.ReadString();
                        int PollutantseasonID = reader.ReadInt32();
                        commandText = string.Format("insert into SeasonalMetricSeasons(SeasonalMetricseasonID,SeasonalMetricID,StartDay,EndDay,SeasonalMetricType,MetricFunction,PollutantseasonID) values({0},{1},{2},{3},{4},'{5}',{6})", ++changeSeasonalMetricseasonID, dicSeasonalMetricID.ContainsKey(SeasonalMetricID) ? dicSeasonalMetricID[SeasonalMetricID] : SeasonalMetricID, Convert.ToInt16(StartDay), Convert.ToInt16(EndDay), Convert.ToInt16(SeasonalMetricType), MetricFunction, dicpollutantseasonid.ContainsKey(PollutantseasonID) ? dicpollutantseasonid[PollutantseasonID] : PollutantseasonID);
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadMonitor(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Monitor datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int MonitorDatasetID = 0;
                Dictionary<int, int> dicMonitorDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicMonitorID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    MonitorDatasetID = reader.ReadInt32();
                    int origMonitorDatasetID = MonitorDatasetID;
                    int oldSetupid = reader.ReadInt32();
                    string MonitorDatasetName = reader.ReadString();
                    string commandText = string.Format("select MonitorDatasetID from MonitorDataSets where setupid={0} and MonitorDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, MonitorDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existMonitorDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from MonitorEntries where MonitorID in (select MonitorID from Monitors where MonitorDatasetID in ({0}))", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from Monitors where MonitorDatasetID in ({0})", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from MonitorDataSets where MonitorDatasetID in ({0})", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicMonitorDatasetID.Add(MonitorDatasetID, existMonitorDatasetID);
                        MonitorDatasetID = existMonitorDatasetID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origMonitorDatasetID, false);
                    }
                    else
                    {
                        commandText = string.Format("select MonitorDatasetID from MonitorDataSets where MonitorDatasetID={0}", MonitorDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(MonitorDatasetID) from MonitorDataSets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicMonitorDatasetID.Add(MonitorDatasetID, 1); MonitorDatasetID = 1; }
                            else
                            {
                                int maxMonitorDatasetID = Convert.ToInt16(obj);
                                dicMonitorDatasetID.Add(MonitorDatasetID, maxMonitorDatasetID + 1);
                                MonitorDatasetID = maxMonitorDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" was imported";
                        }
                        dicDoImport.Add(origMonitorDatasetID, true);
                    }
                    //the 'F' is for the LOCKED column in MonitorDataSets.  This is being added and is not a predefined.
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into MonitorDataSets(MonitorDatasetID,SetupID,MonitorDatasetName, LOCKED) values({0},{1},'{2}', 'F')", MonitorDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, MonitorDatasetName);
                    if (currentPhase == 2 && dicDoImport[origMonitorDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                string nextTable = reader.ReadString();
                if (nextTable != "pollutants")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                pBarImport.Value = 0;
                lbProcess.Text = "Importing pollutants...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportPollutant = new Dictionary<int, bool>();

                tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int PollutantID = 0;
                int existPollutantID = 0;
                Dictionary<int, int> dicpollutantid = new Dictionary<int, int>();
                Dictionary<int, int> dicpollutantseasonid = new Dictionary<int, int>();
                Dictionary<int, int> dicMetricID = new Dictionary<int, int>();
                Dictionary<int, int> dicSeasonalMetricID = new Dictionary<int, int>();
                Dictionary<int, bool> dicExistPollutant = new Dictionary<int, bool>();
                for (int i = 0; i < tableCount; i++)
                {
                    string pollutantName = reader.ReadString();
                    PollutantID = reader.ReadInt32();
                    int origPollutantID = PollutantID;
                    int oldSetupid = reader.ReadInt32();
                    string commandText = string.Format("select PollutantID from pollutants where setupid={0} and PollutantName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, pollutantName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        dicExistPollutant.Add(PollutantID, true);
                        existPollutantID = Convert.ToInt16(obj);
                        dicpollutantid.Add(PollutantID, existPollutantID);
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and was retained";
                        }
                        dicDoImportPollutant.Add(origPollutantID, false);
                    }
                    else
                    {
                        dicExistPollutant.Add(PollutantID, false);
                        commandText = string.Format("select PollutantID from pollutants where PollutantID={0}", PollutantID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PollutantID) from pollutants";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicpollutantid.Add(PollutantID, 1); PollutantID = 1; }
                            else
                            {
                                int maxPollutantID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                                dicpollutantid.Add(PollutantID, maxPollutantID + 1);
                                PollutantID = maxPollutantID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" was imported";
                        }
                        dicDoImportPollutant.Add(origPollutantID, true);
                        commandText = string.Format("insert into pollutants(PollutantName,PollutantID,SetupID,ObservationType) values('{0}',{1},{2},{3})", pollutantName, PollutantID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImportPollutant[origPollutantID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    pBarImport.PerformStep();
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "PollutantSeasons" && nextTable != "Metrics")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Pollutant seasons...";
                lbProcess.Refresh();
                this.Refresh();
                int changePollutantSeasonID = 0;
                if (nextTable == "PollutantSeasons")
                {
                    int PollutantSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = PollutantSeasonscount;
                    string commandText = "select max(PollutantSeasonID) from PollutantSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changePollutantSeasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < PollutantSeasonscount; i++)
                    {
                        int pollutantseasonid = reader.ReadInt32();
                        PollutantID = reader.ReadInt32();
                        if (dicExistPollutant[PollutantID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 5 * sizeof(Int32);
                        }
                        else
                        {
                            changePollutantSeasonID++;
                            dicpollutantseasonid.Add(pollutantseasonid, changePollutantSeasonID);
                            commandText = string.Format("insert into PollutantSeasons(PollutantSeasonID,PollutantID,StartDay,EndDay,StartHour,EndHour,Numbins) values({0},{1},{2},{3},{4},{5},{6})", changePollutantSeasonID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                            if (currentPhase == 2 && dicDoImportPollutant[PollutantID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                    if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                    nextTable = reader.ReadString();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Metrics...";
                lbProcess.Refresh();
                this.Refresh();

                Dictionary<int, bool> dicExistMetric = new Dictionary<int, bool>();
                int changeMetricID = 0;
                Dictionary<int, bool> dicDoImportMetric = new Dictionary<int, bool>();
                if (nextTable == "Metrics")
                {
                    int Metricscount = reader.ReadInt32();
                    pBarImport.Maximum = Metricscount;
                    string commandText = "select max(MetricID) from Metrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

                    if (!Convert.IsDBNull(obj))
                    {
                        changeMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < Metricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        PollutantID = reader.ReadInt32();
                        if (dicExistPollutant[PollutantID])
                        {
                            dicExistMetric.Add(MetricID, true);
                            reader.ReadString();
                            reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        }
                        else
                        {
                            dicExistMetric.Add(MetricID, false);
                            commandText = string.Format("select MetricID from Metrics where MetricID={0}", MetricID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicMetricID.Add(MetricID, ++changeMetricID);
                                MetricID = changeMetricID;
                            }
                            commandText = string.Format("insert into Metrics(MetricID,PollutantID,MetricName,HourlyMetricGeneration) values({0},{1},'{2}',{3})", MetricID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, reader.ReadString(), reader.ReadInt32());
                            if (currentPhase == 2 && dicDoImportPollutant[PollutantID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                dicDoImportMetric.Add(MetricID, true);
                            }
                            else
                            {
                                dicDoImportMetric.Add(MetricID, false);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "FixedWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Fixed window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int FixedWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = FixedWindowMetricscount;
                    for (int i = 0; i < FixedWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 3 * sizeof(Int32);
                        }
                        else
                        {
                            string commandText = string.Format("insert into fixedwindowMetrics(MetricID,Starthour,Endhour,Statistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "MovingWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Moving window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int MovingWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = MovingWindowMetricscount;
                    for (int i = 0; i < MovingWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 3 * sizeof(Int32);
                        }
                        else
                        {
                            string commandText = string.Format("insert into MovingWindowMetrics(MetricID,Windowsize,Windowstatistic,Dailystatistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "CustomMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Custom metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int CustomMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = CustomMetricscount;
                    for (int i = 0; i < CustomMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            reader.ReadString();
                        }
                        else
                        {
                            string commandText = string.Format("insert into CustomMetrics(MetricID,MetricFunction) values({0},'{1}')", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metrics...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicExistSeasonalMetric = new Dictionary<int, bool>();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricID = 0;
                Dictionary<int, bool> dicDoImportSeasonalMetric = new Dictionary<int, bool>();
                if (nextTable == "SeasonalMetrics")
                {

                    int SeasonalMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricscount;
                    string commandText = "select max(SeasonalMetricID) from SeasonalMetrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricscount; i++)
                    {
                        int SeasonalMetricID = reader.ReadInt32();
                        int origSeasonalMetricID = SeasonalMetricID;
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            dicExistSeasonalMetric.Add(SeasonalMetricID, true);
                            reader.ReadString();
                        }
                        else
                        {
                            dicExistSeasonalMetric.Add(SeasonalMetricID, false);
                            commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where SeasonalMetricID={0}", SeasonalMetricID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicSeasonalMetricID.Add(SeasonalMetricID, ++changeSeasonalMetricID);
                                SeasonalMetricID = changeSeasonalMetricID;
                            }
                            commandText = string.Format("insert into SeasonalMetrics(SeasonalMetricID,MetricID,SeasonalMetricName) values({0},{1},'{2}')", SeasonalMetricID, dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                dicDoImportSeasonalMetric.Add(origSeasonalMetricID, true);
                            } else
                            {
                                dicDoImportSeasonalMetric.Add(origSeasonalMetricID, false);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metric seasons...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricseasonID = 0;
                if (nextTable == "SeasonalMetricSeasons")
                {
                    int SeasonalMetricSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricSeasonscount;
                    string commandText = "select max(SeasonalMetricseasonID) from SeasonalMetricSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricseasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricSeasonscount; i++)
                    {
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        int SeasonalMetricID = reader.ReadInt32();
                        if (dicExistSeasonalMetric[SeasonalMetricID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 3 * sizeof(Int32);
                            reader.ReadString();
                            reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        }
                        else
                        {
                            int StartDay = reader.ReadInt32();
                            int EndDay = reader.ReadInt32();
                            int SeasonalMetricType = reader.ReadInt32();
                            string MetricFunction = reader.ReadString();
                            int PollutantseasonID = reader.ReadInt32();
                            commandText = string.Format("insert into SeasonalMetricSeasons(SeasonalMetricseasonID,SeasonalMetricID,StartDay,EndDay,SeasonalMetricType,MetricFunction,PollutantseasonID) values({0},{1},{2},{3},{4},'{5}',{6})", ++changeSeasonalMetricseasonID, dicSeasonalMetricID.ContainsKey(SeasonalMetricID) ? dicSeasonalMetricID[SeasonalMetricID] : SeasonalMetricID, Convert.ToInt16(StartDay), Convert.ToInt16(EndDay), Convert.ToInt16(SeasonalMetricType), MetricFunction, dicpollutantseasonid.ContainsKey(PollutantseasonID) ? dicpollutantseasonid[PollutantseasonID] : PollutantseasonID);
                            if (currentPhase == 2 && dicDoImportSeasonalMetric[SeasonalMetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Monitors...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "Monitors")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int Monitorscount = reader.ReadInt32();
                pBarImport.Maximum = Monitorscount;
                int maxMonitorID = 0;
                string commandMonitorID = "select max(MonitorID) from Monitors";
                object oMonitorID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandMonitorID);
                Dictionary<int, bool> dicDoImportMonitor = new Dictionary<int, bool>();
                if (!Convert.IsDBNull(oMonitorID))
                {
                    maxMonitorID = Convert.ToInt32(oMonitorID);
                }

                for (int i = 0; i < (Monitorscount / 150) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    Boolean doBatch = false;
                    for (int k = 0; k < 150; k++)
                    {
                        if (i * 150 + k < Monitorscount)
                        {
                            int MonitorID = reader.ReadInt32();
                            int origMonitorID = MonitorID;
                            MonitorDatasetID = reader.ReadInt32();
                            int pollutantID = reader.ReadInt32();
                            dicMonitorID.Add(MonitorID, ++maxMonitorID);
                            MonitorID = maxMonitorID;
                            if (dicDoImport[MonitorDatasetID])
                            {
                                commandText = commandText + string.Format("insert into Monitors(MonitorID,MonitorDatasetID,PollutantID,Latitude,Longitude,MonitorName,MonitorDescription) values({0},{1},{2},{3},{4},'{5}','{6}');", MonitorID, dicMonitorDatasetID.ContainsKey(MonitorDatasetID) ? dicMonitorDatasetID[MonitorDatasetID] : MonitorDatasetID, dicpollutantid.ContainsKey(pollutantID) ? dicpollutantid[pollutantID] : pollutantID, reader.ReadSingle(), reader.ReadSingle(), reader.ReadString(), reader.ReadString().Replace("'", "''''"));
                                dicDoImportMonitor.Add(origMonitorID, true);
                                doBatch = true;
                            } else {
                                dicDoImportMonitor.Add(origMonitorID, false);
                                reader.ReadSingle();
                                reader.ReadSingle();
                                reader.ReadString();
                                reader.ReadString();
                            }
                            pBarImport.PerformStep();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doBatch)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }

                }

                pBarImport.Value = 0;
                lbProcess.Text = "Monitor entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "MonitorEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                FirebirdSql.Data.FirebirdClient.FbParameter fbParameter = new FirebirdSql.Data.FirebirdClient.FbParameter();
                int MonitorEntriescount = reader.ReadInt32();
                pBarImport.Maximum = MonitorEntriescount;
                int maxMonitorEntryID = 0;
                string commandMonitorEntryID = "select max(MonitorEntryID) from MonitorEntries";
                object oMonitorEntryID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandMonitorEntryID);
                if (!Convert.IsDBNull(oMonitorEntryID))
                {
                    maxMonitorEntryID = Convert.ToInt32(oMonitorEntryID);
                }
                for (int i = 0; i < MonitorEntriescount; i++)
                {
                    string commandText = "";
                    reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                    int MonitorID = reader.ReadInt32();
                    int _bytes = reader.ReadInt32();
                    byte[] vvalues = reader.ReadBytes(_bytes);
                    fbParameter = new FirebirdSql.Data.FirebirdClient.FbParameter("@Vvaluesblob", FirebirdSql.Data.FirebirdClient.FbDbType.Binary);
                    fbParameter.Value = vvalues;
                    int MetricID = reader.ReadInt32();
                    int SeasonalMetricID = reader.ReadInt32();
                    commandText = string.Format("insert into MonitorEntries(Vvalues,MonitorEntryID,MonitorID,MetricID,SeasonalMetricID,Yyear,Statistic) values(@Vvaluesblob,{0},{1},{2},{3},{4},'{5}');", ++maxMonitorEntryID, dicMonitorID.ContainsKey(MonitorID) ? dicMonitorID[MonitorID] : MonitorID, MetricID == -1 ? "NULL" : MetricID.ToString(), SeasonalMetricID == -1 ? "NULL" : SeasonalMetricID.ToString(), reader.ReadInt32(), reader.ReadString());
                    if (currentPhase == 2 && dicDoImportMonitor[MonitorID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText, fbParameter);
                    }
                    pBarImport.PerformStep();
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }


        private void ReadMonitor2(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Monitor datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false;

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int MonitorDatasetID = 0;
                Dictionary<int, int> dicMonitorDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicMonitorID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    // First, check to see if we have a dataset with this name
                    // TODO: If we do, just log that we are going to keep it.  Don't overwrite it.
                    MonitorDatasetID = reader.ReadInt32();
                    int oldSetupid = reader.ReadInt32();
                    string MonitorDatasetName = reader.ReadString();
                    string commandText = string.Format("select MonitorDatasetID from MonitorDataSets where setupid={0} and MonitorDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, MonitorDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existMonitorDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from MonitorEntries where MonitorID in (select MonitorID from Monitors where MonitorDatasetID in ({0}))", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from Monitors where MonitorDatasetID in ({0})", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from MonitorDataSets where MonitorDatasetID in ({0})", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicMonitorDatasetID.Add(MonitorDatasetID, existMonitorDatasetID);
                        MonitorDatasetID = existMonitorDatasetID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        // If we have a datset with this ID (but a different name) then get a new ID to use when inserting it
                        commandText = string.Format("select MonitorDatasetID from MonitorDataSets where MonitorDatasetID={0}", MonitorDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(MonitorDatasetID) from MonitorDataSets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicMonitorDatasetID.Add(MonitorDatasetID, 1); MonitorDatasetID = 1; }
                            else
                            {
                                int maxMonitorDatasetID = Convert.ToInt16(obj);
                                dicMonitorDatasetID.Add(MonitorDatasetID, maxMonitorDatasetID + 1);
                                MonitorDatasetID = maxMonitorDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nMonitor dataset \"" + MonitorDatasetName + "\" was imported";
                        }
                        doImport = true;
                    }
                    //the 'F' is for the LOCKED column in MonitorDataSets.  This is being added and is not a predefined.
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into MonitorDataSets(MonitorDatasetID,SetupID,MonitorDatasetName, LOCKED) values({0},{1},'{2}', 'F')", MonitorDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, MonitorDatasetName);
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Monitors...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                /*
                if(nextTable.Equals("pollutants2"))
                {
                    ReadPollutant2(currentPhase, reader);
                    nextTable = reader.ReadString();
                } 
                */
                //TODO: This was previously an ELSE IF to the above.  Seems like we could have both pollutants2 and Monitors?  Need to test...
                if (nextTable != "Monitors")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int Monitorscount = reader.ReadInt32();
                pBarImport.Maximum = Monitorscount;
                int maxMonitorID = 0;
                string commandMonitorID = "select max(MonitorID) from Monitors";
                object oMonitorID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandMonitorID);
                if (!Convert.IsDBNull(oMonitorID))
                {
                    maxMonitorID = Convert.ToInt32(oMonitorID);
                }
                for (int i = 0; i < (Monitorscount / 150) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    for (int k = 0; k < 150; k++)
                    {
                        if (i * 150 + k < Monitorscount)
                        {
                            int MonitorID = reader.ReadInt32();
                            MonitorDatasetID = reader.ReadInt32();
                            int pollutantID = reader.ReadInt32();
                            dicMonitorID.Add(MonitorID, ++maxMonitorID);
                            MonitorID = maxMonitorID;
                            commandText = commandText + string.Format("insert into Monitors(MonitorID,MonitorDatasetID,PollutantID,Latitude,Longitude,MonitorName,MonitorDescription) values({0},{1},{2},{3},{4},'{5}','{6}');", MonitorID, dicMonitorDatasetID.ContainsKey(MonitorDatasetID) ? dicMonitorDatasetID[MonitorDatasetID] : MonitorDatasetID, gdicPollutant.ContainsKey(pollutantID) ? gdicPollutant[pollutantID] : pollutantID, reader.ReadSingle(), reader.ReadSingle(), reader.ReadString(), reader.ReadString().Replace("'", "''''"));
                            pBarImport.PerformStep();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }

                }

                pBarImport.Value = 0;
                lbProcess.Text = "Monitor entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "MonitorEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                FirebirdSql.Data.FirebirdClient.FbParameter fbParameter = new FirebirdSql.Data.FirebirdClient.FbParameter();
                int MonitorEntriescount = reader.ReadInt32();
                pBarImport.Maximum = MonitorEntriescount;
                int maxMonitorEntryID = 0;
                string commandMonitorEntryID = "select max(MonitorEntryID) from MonitorEntries";
                object oMonitorEntryID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandMonitorEntryID);
                if (!Convert.IsDBNull(oMonitorEntryID))
                {
                    maxMonitorEntryID = Convert.ToInt32(oMonitorEntryID);
                }
                for (int i = 0; i < MonitorEntriescount; i++)
                {
                    string commandText = "";
                    reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                    int MonitorID = reader.ReadInt32();
                    int _bytes = reader.ReadInt32();
                    byte[] vvalues = reader.ReadBytes(_bytes);
                    fbParameter = new FirebirdSql.Data.FirebirdClient.FbParameter("@Vvaluesblob", FirebirdSql.Data.FirebirdClient.FbDbType.Binary);
                    fbParameter.Value = vvalues;
                    int MetricID = reader.ReadInt32();
                    int SeasonalMetricID = reader.ReadInt32();
                    commandText = string.Format("insert into MonitorEntries(Vvalues,MonitorEntryID,MonitorID,MetricID,SeasonalMetricID,Yyear,Statistic) values(@Vvaluesblob,{0},{1},{2},{3},{4},'{5}');", ++maxMonitorEntryID, dicMonitorID.ContainsKey(MonitorID) ? dicMonitorID[MonitorID] : MonitorID, MetricID == -1 ? "NULL" : MetricID.ToString(), SeasonalMetricID == -1 ? "NULL" : SeasonalMetricID.ToString(), reader.ReadInt32(), reader.ReadString());
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText, fbParameter);
                    }
                    pBarImport.PerformStep();
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadIncidence(int currentPhase, BinaryReader reader)
        {
            try
            {
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, bool> dicExistGriddefinition = new Dictionary<int, bool>();
                Dictionary<int, int> dicEndPointGroupID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointID = new Dictionary<int, int>();
                Dictionary<int, int> dicRaceID = new Dictionary<int, int>();
                Dictionary<int, int> dicGenderID = new Dictionary<int, int>();
                Dictionary<int, int> dicEthnicityID = new Dictionary<int, int>();

                pBarImport.Value = 0;
                lbProcess.Text = "Related grid definitions...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportGrid = new Dictionary<int, bool>();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
                    dicGriddefinitionID.Add(griddefinitionID, griddefinitionID);
                    int oldSetupid = reader.ReadInt32();
                    string griddefinitionName = reader.ReadString();
                    string commandText = string.Format("select GriddefinitionID from griddefinitions where setupid={0} and GriddefinitionName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        dicExistGriddefinition.Add(griddefinitionID, true);
                        int existgriddefinitionID = Convert.ToInt16(obj);
                        dicGriddefinitionID[griddefinitionID] = existgriddefinitionID;
                        reader.BaseStream.Position = reader.BaseStream.Position + 4 * sizeof(Int32);
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and was retained";
                        }
                        dicDoImportGrid.Add(griddefinitionID, false);
                    }
                    else
                    {
                        dicExistGriddefinition.Add(griddefinitionID, false);
                        commandText = string.Format("select GriddefinitionID from griddefinitions where GriddefinitionID={0}", griddefinitionID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(GriddefinitionID) from griddefinitions";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicGriddefinitionID[griddefinitionID] = 1;
                            }
                            else
                            {
                                int maxGriddefinitionID = Convert.ToInt16(obj);
                                dicGriddefinitionID[griddefinitionID] = maxGriddefinitionID + 1;
                            }
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nGrid definition \"" + griddefinitionName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nGrid definition \"" + griddefinitionName + "\" was imported";
                            }
                            dicDoImportGrid.Add(griddefinitionID, true);
                        }
                        //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                        commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", dicGriddefinitionID[griddefinitionID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImportGrid[griddefinitionID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Related shapefiles...";
                lbProcess.Refresh();
                this.Refresh();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) return;
                for (int j = 0; j < tableCount; j++)
                {
                    int GriddefinitionID = reader.ReadInt16();
                    if (dicExistGriddefinition[GriddefinitionID])
                    {
                        string filetype = reader.ReadString();
                        reader.ReadString();
                        switch (filetype)
                        {
                            case "regular":
                                reader.ReadSingle(); reader.ReadSingle(); reader.ReadInt16(); reader.ReadInt16();
                                break;
                            case "shapefile":
                                break;
                        }
                        int shapefiles = 0;
                        while (shapefiles < 3)
                        {
                            Int64 length64 = reader.ReadInt64();
                            Int64 diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                reader.ReadBytes(Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            reader.ReadBytes((Int32)(length64 - diff));
                            shapefiles++;
                        }
                    }
                    else
                    {
                        string commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID);
                        string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        string filetype = reader.ReadString();
                        string shapefilename = reader.ReadString();
                        switch (filetype)
                        {
                            case "regular":
                                commandText = string.Format("insert into regulargriddefinitiondetails(GriddefinitionID,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude,ShapefileName) values({0},{1},{2},{3},{4},'{5}')", dicGriddefinitionID[GriddefinitionID], reader.ReadSingle(), reader.ReadSingle(), reader.ReadInt16(), reader.ReadInt16(), shapefilename);
                                if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                                {
                                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                }
                                break;
                            case "shapefile":
                                //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                                // 2015 02 12 added LOCKED to field list
                                commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename, LOCKED) values({0},'{1}', 'F')", dicGriddefinitionID[GriddefinitionID], shapefilename);
                                if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                                {
                                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                }
                                break;
                        }
                        if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                        {
                            if (!Directory.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname))
                        {
                            Directory.CreateDirectory(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname);
                        }
                        if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp");
                        FileStream file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Create, FileAccess.Write);
                        Int64 length64 = reader.ReadInt64();
                        Int64 diff = 0;
                        byte[] array;
                        while (diff <= length64 - Int32.MaxValue)
                        {
                            array = reader.ReadBytes(Int32.MaxValue);
                            file.Write(array, 0, Int32.MaxValue);
                            diff += Int32.MaxValue;
                        }
                        array = reader.ReadBytes((Int32)(length64 - diff));
                        file.Write(array, 0, (Int32)(length64 - diff));
                        file.Close();

                        file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Create, FileAccess.Write);
                        length64 = reader.ReadInt64();
                        diff = 0;
                        while (diff <= length64 - Int32.MaxValue)
                        {
                            array = reader.ReadBytes(Int32.MaxValue);
                            file.Write(array, 0, Int32.MaxValue);
                            diff += Int32.MaxValue;
                        }
                        array = reader.ReadBytes((Int32)(length64 - diff));
                        file.Write(array, 0, (Int32)(length64 - diff));
                        file.Close();

                        file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Create, FileAccess.Write);
                        length64 = reader.ReadInt64();
                        diff = 0;
                        while (diff <= length64 - Int32.MaxValue)
                        {
                            array = reader.ReadBytes(Int32.MaxValue);
                            file.Write(array, 0, Int32.MaxValue);
                            diff += Int32.MaxValue;
                        }
                        array = reader.ReadBytes((Int32)(length64 - diff));
                        file.Write(array, 0, (Int32)(length64 - diff));
                        file.Close();
                        }
                        else
                        // We're either in phase 1 and just scanning, or we're not importing the grid definition so we should just blindly scan through the shapefile contents so we can get to the next thing
                        {
                            // We have three segments here for shx, shp, dbf.  Skip over each of them
                            for (int i = 0; i <= 2; i++)
                            {
                                Int64 length64 = reader.ReadInt64();
                                reader.BaseStream.Position += length64;
                            }
                        }
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Incidence datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "IncidenceDatasets")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                Dictionary<int, int> dicIncidenceDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicIncidenceRateID = new Dictionary<int, int>();
                int IncidenceDatasetID = 0;
                for (int i = 0; i < tableCount; i++)
                {
                    IncidenceDatasetID = reader.ReadInt32();
                    dicIncidenceDatasetID.Add(IncidenceDatasetID, IncidenceDatasetID);
                    int oldSetupid = reader.ReadInt32();
                    string IncidenceDatasetName = reader.ReadString();
                    string commandText = string.Format("select IncidenceDatasetID from IncidenceDatasets where setupid={0} and IncidenceDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncidenceDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existIncidenceDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from Incidenceentries where IncidenceRateID in (select IncidenceRateID  from IncidenceRates where IncidenceDatasetID in ({0}))", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncidenceRates where IncidenceDatasetID ={0}", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncidenceDatasets where IncidenceDatasetID={0}", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicIncidenceDatasetID[IncidenceDatasetID] = existIncidenceDatasetID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" exists and was retained";
                        }
                        dicDoImport.Add(IncidenceDatasetID, false);
                    }
                    else
                    {
                        commandText = string.Format("select IncidenceDatasetID from IncidenceDatasets where IncidenceDatasetID={0}", IncidenceDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(IncidenceDatasetID) from IncidenceDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                                dicIncidenceDatasetID[IncidenceDatasetID] = 1;
                            else
                                dicIncidenceDatasetID[IncidenceDatasetID] = Convert.ToInt16(obj) + 1;
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" was imported";
                        }
                        dicDoImport.Add(IncidenceDatasetID, true);
                    }
                    int relateGridDefinitionID = reader.ReadInt32();
                    //The 'F' is for the Locked column in INCIDENCEDATESTS - imported not predefined.
                    commandText = string.Format("insert into IncidenceDatasets(IncidenceDatasetID,SetupID,IncidenceDatasetName,GridDefinitionID, LOCKED) values({0},{1},'{2}',{3}, 'F')", dicIncidenceDatasetID[IncidenceDatasetID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncidenceDatasetName, dicGriddefinitionID[relateGridDefinitionID]);
                    if (currentPhase == 2 && dicDoImport[IncidenceDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoint groups...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndpointGroups")
                {
                    int EndpointGroupscount = reader.ReadInt32();
                    pBarImport.Maximum = EndpointGroupscount;
                    for (int i = 0; i < EndpointGroupscount; i++)
                    {
                        int EndPointGroupID = reader.ReadInt32();
                        dicEndPointGroupID.Add(EndPointGroupID, EndPointGroupID);
                        string EndPointGroupName = reader.ReadString();
                        string commandExist = string.Format("select EndPointGroupID from EndpointGroups where EndPointGroupName='{0}'", EndPointGroupName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointGroupID) from EndpointGroups";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndpointGroups(EndPointGroupID,EndPointGroupName) values({0},'{1}')", dicEndPointGroupID[EndPointGroupID], EndPointGroupName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoints...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndPoints")
                {
                    int EndPointscount = reader.ReadInt32();
                    pBarImport.Maximum = EndPointscount;
                    for (int i = 0; i < EndPointscount; i++)
                    {
                        int EndPointID = reader.ReadInt32();
                        dicEndPointID.Add(EndPointID, EndPointID);
                        int EndPointGroupID = reader.ReadInt32();
                        string EndPointName = reader.ReadString();
                        string commandExist = string.Format("select EndPointID from EndPoints where EndPointgroupID={0} and EndPointName='{1}'", dicEndPointGroupID[EndPointGroupID], EndPointName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointID) from EndPoints";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointID[EndPointID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndPoints(EndPointID,EndPointGroupID,EndPointName) values({0},{1},'{2}')", dicEndPointID[EndPointID], dicEndPointGroupID[EndPointGroupID], EndPointName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointID[EndPointID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Race...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Race")
                {
                    int Racecount = reader.ReadInt32();
                    pBarImport.Maximum = Racecount;
                    for (int i = 0; i < Racecount; i++)
                    {
                        int RaceID = reader.ReadInt32();
                        dicRaceID.Add(RaceID, RaceID);
                        string RaceName = reader.ReadString();
                        string commandExist = string.Format("select RaceID from Races where RaceName='{0}'", RaceName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(RaceID) from Races";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicRaceID[RaceID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Races(RaceID,RaceName) values({0},'{1}')", dicRaceID[RaceID], RaceName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicRaceID[RaceID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Gender...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Gender")
                {
                    int Gendercount = reader.ReadInt32();
                    pBarImport.Maximum = Gendercount;
                    for (int i = 0; i < Gendercount; i++)
                    {
                        int GenderID = reader.ReadInt32();
                        dicGenderID.Add(GenderID, GenderID);
                        string GenderName = reader.ReadString();
                        string commandExist = string.Format("select GenderID from Genders where GenderName='{0}'", GenderName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(GenderID) from Genders";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicGenderID[GenderID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Genders(GenderID,GenderName) values({0},'{1}')", dicGenderID[GenderID], GenderName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicGenderID[GenderID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Ethnicity...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Ethnicity")
                {
                    int Ethnicitycount = reader.ReadInt32();
                    pBarImport.Maximum = Ethnicitycount;
                    for (int i = 0; i < Ethnicitycount; i++)
                    {
                        int EthnicityID = reader.ReadInt32();
                        dicEthnicityID.Add(EthnicityID, EthnicityID);
                        string EthnicityName = reader.ReadString();
                        string commandExist = string.Format("select EthnicityID from Ethnicity where EthnicityName='{0}'", EthnicityName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EthnicityID) from Ethnicity";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEthnicityID[EthnicityID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Ethnicity(EthnicityID,EthnicityName) values({0},'{1}')", dicEthnicityID[EthnicityID], EthnicityName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEthnicityID[EthnicityID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Incidence rates...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "IncidenceRates")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncidenceRatescount = reader.ReadInt32();
                pBarImport.Maximum = IncidenceRatescount;
                int maxIncidenceRateID = 0;
                string commandIncidenceRateID = "select max(IncidenceRateID) from IncidenceRates";
                object oIncidenceRateID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandIncidenceRateID);
                if (!Convert.IsDBNull(oIncidenceRateID))
                { maxIncidenceRateID = Convert.ToInt16(oIncidenceRateID); }
                Dictionary<int, bool> dicDoImportRate = new Dictionary<int, bool>();
                for (int i = 0; i < (IncidenceRatescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    Boolean doBatch = false;
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < IncidenceRatescount)
                        {
                            int IncidenceRateID = reader.ReadInt32();
                            dicIncidenceRateID.Add(IncidenceRateID, IncidenceRateID);
                            IncidenceDatasetID = reader.ReadInt32();
                            int GriddefinitionID = reader.ReadInt32();
                            int EndPointGroupID = reader.ReadInt32();
                            int EndPointID = reader.ReadInt32();
                            int RaceID = reader.ReadInt32();
                            int GenderID = reader.ReadInt32();
                            int StartAge = reader.ReadInt32();
                            int EndAge = reader.ReadInt32();
                            char Prevalence = reader.ReadChar();
                            int EthnicityID = reader.ReadInt32();
                            string existIncidenceRateID = string.Format("select IncidenceRateID from IncidenceRates where IncidenceRateID={0}", IncidenceRateID);
                            object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, existIncidenceRateID);
                            //if (Convert.ToInt32(obj) > 0)
                            //{
                            // Forcing this override to compensate for an issue with inserting batches
                            dicIncidenceRateID[IncidenceRateID] = ++maxIncidenceRateID;
                            //}
                            if (dicDoImport[IncidenceDatasetID])
                            {
                                commandText = commandText + string.Format("insert into IncidenceRates(IncidenceRateID,IncidenceDatasetID,GriddefinitionID,EndPointGroupID,EndPointID,RaceID,GenderID,StartAge,EndAge,Prevalence,EthnicityID) values({0},{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10});", dicIncidenceRateID[IncidenceRateID], dicIncidenceDatasetID[IncidenceDatasetID], dicGriddefinitionID[GriddefinitionID], dicEndPointGroupID[EndPointGroupID], dicEndPointID[EndPointID], dicRaceID[RaceID], dicGenderID[GenderID], StartAge, EndAge, Prevalence, dicEthnicityID[EthnicityID]);
                                dicDoImportRate.Add(IncidenceRateID, true);
                                doBatch = true;
                            }
                            else
                            {
                                dicDoImportRate.Add(IncidenceRateID, false);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doBatch)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Incidence entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "IncidenceEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncidenceEntriescount = reader.ReadInt32();
                pBarImport.Maximum = IncidenceEntriescount;
                for (int i = 0; i < (IncidenceEntriescount / 150) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    Boolean doBatch = false;
                    for (int k = 0; k < 150; k++)
                    {
                        if (i * 150 + k < IncidenceEntriescount)
                        {
                            int IncidenceRateID = reader.ReadInt32();
                            if(dicDoImportRate[IncidenceRateID])
                            {
                                commandText = commandText + string.Format("insert into Incidenceentries(IncidenceRateID,Ccolumn,Row,Vvalue) values({0},{1},{2},{3});", dicIncidenceRateID[IncidenceRateID], reader.ReadInt32(), reader.ReadInt32(), reader.ReadSingle());
                                doBatch = true;
                            }
                            else
                            {
                                reader.ReadInt32(); reader.ReadInt32(); reader.ReadSingle();
                            }
                            pBarImport.PerformStep();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doBatch)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadIncidence2(int currentPhase, BinaryReader reader)
        {
            try
            {
                Dictionary<int, int> dicEndPointGroupID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointID = new Dictionary<int, int>();
                Dictionary<int, int> dicRaceID = new Dictionary<int, int>();
                Dictionary<int, int> dicGenderID = new Dictionary<int, int>();
                Dictionary<int, int> dicEthnicityID = new Dictionary<int, int>();

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                pBarImport.Value = 0;
                lbProcess.Text = "Incidence datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false;

                int origIncidenceID;
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                Dictionary<int, int> dicIncidenceDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicIncidenceRateID = new Dictionary<int, int>();
                int IncidenceDatasetID = 0;
                for (int i = 0; i < tableCount; i++)
                {
                    IncidenceDatasetID = reader.ReadInt32();
                    origIncidenceID = IncidenceDatasetID;
                    dicIncidenceDatasetID.Add(IncidenceDatasetID, IncidenceDatasetID);
                    int oldSetupid = reader.ReadInt32();
                    string IncidenceDatasetName = reader.ReadString();
                    string commandText = string.Format("select IncidenceDatasetID from IncidenceDatasets where setupid={0} and IncidenceDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncidenceDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existIncidenceDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from Incidenceentries where IncidenceRateID in (select IncidenceRateID  from IncidenceRates where IncidenceDatasetID in ({0}))", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncidenceRates where IncidenceDatasetID ={0}", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncidenceDatasets where IncidenceDatasetID={0}", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicIncidenceDatasetID[IncidenceDatasetID] = existIncidenceDatasetID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        commandText = string.Format("select IncidenceDatasetID from IncidenceDatasets where IncidenceDatasetID={0}", IncidenceDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(IncidenceDatasetID) from IncidenceDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                                dicIncidenceDatasetID[IncidenceDatasetID] = 1;
                            else
                                dicIncidenceDatasetID[IncidenceDatasetID] = Convert.ToInt16(obj) + 1;
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nIncidence dataset \"" + IncidenceDatasetName + "\" was imported";
                        }
                        doImport = true;
                    }
                    int relateGridDefinitionID = reader.ReadInt32();
                    //Map the old to the new incidence ID so the HIF import can use it
                    gdicIncidence[origIncidenceID] = IncidenceDatasetID;
                    //The 'F' is for the Locked column in INCIDENCEDATESTS - imported not predefined.
                    commandText = string.Format("insert into IncidenceDatasets(IncidenceDatasetID,SetupID,IncidenceDatasetName,GridDefinitionID, LOCKED) values({0},{1},'{2}',{3}, 'F')", dicIncidenceDatasetID[IncidenceDatasetID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncidenceDatasetName, gdicGridDefinition[relateGridDefinitionID]);
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoint groups...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable == "EndpointGroups")
                {
                    int EndpointGroupscount = reader.ReadInt32();
                    pBarImport.Maximum = EndpointGroupscount;
                    for (int i = 0; i < EndpointGroupscount; i++)
                    {
                        int EndPointGroupID = reader.ReadInt32();
                        dicEndPointGroupID.Add(EndPointGroupID, EndPointGroupID);
                        string EndPointGroupName = reader.ReadString();
                        string commandExist = string.Format("select EndPointGroupID from EndpointGroups where EndPointGroupName='{0}'", EndPointGroupName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointGroupID) from EndpointGroups";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndpointGroups(EndPointGroupID,EndPointGroupName) values({0},'{1}')", dicEndPointGroupID[EndPointGroupID], EndPointGroupName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoints...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndPoints")
                {
                    int EndPointscount = reader.ReadInt32();
                    pBarImport.Maximum = EndPointscount;
                    for (int i = 0; i < EndPointscount; i++)
                    {
                        int EndPointID = reader.ReadInt32();
                        dicEndPointID.Add(EndPointID, EndPointID);
                        int EndPointGroupID = reader.ReadInt32();
                        string EndPointName = reader.ReadString();
                        string commandExist = string.Format("select EndPointID from EndPoints where EndPointgroupID={0} and EndPointName='{1}'", dicEndPointGroupID[EndPointGroupID], EndPointName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointID) from EndPoints";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointID[EndPointID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndPoints(EndPointID,EndPointGroupID,EndPointName) values({0},{1},'{2}')", dicEndPointID[EndPointID], dicEndPointGroupID[EndPointGroupID], EndPointName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointID[EndPointID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Race...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Race")
                {
                    int Racecount = reader.ReadInt32();
                    pBarImport.Maximum = Racecount;
                    for (int i = 0; i < Racecount; i++)
                    {
                        int RaceID = reader.ReadInt32();
                        dicRaceID.Add(RaceID, RaceID);
                        string RaceName = reader.ReadString();
                        string commandExist = string.Format("select RaceID from Races where RaceName='{0}'", RaceName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(RaceID) from Races";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicRaceID[RaceID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Races(RaceID,RaceName) values({0},'{1}')", dicRaceID[RaceID], RaceName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicRaceID[RaceID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Gender...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Gender")
                {
                    int Gendercount = reader.ReadInt32();
                    pBarImport.Maximum = Gendercount;
                    for (int i = 0; i < Gendercount; i++)
                    {
                        int GenderID = reader.ReadInt32();
                        dicGenderID.Add(GenderID, GenderID);
                        string GenderName = reader.ReadString();
                        string commandExist = string.Format("select GenderID from Genders where GenderName='{0}'", GenderName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(GenderID) from Genders";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicGenderID[GenderID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Genders(GenderID,GenderName) values({0},'{1}')", dicGenderID[GenderID], GenderName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicGenderID[GenderID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Ethnicity...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Ethnicity")
                {
                    int Ethnicitycount = reader.ReadInt32();
                    pBarImport.Maximum = Ethnicitycount;
                    for (int i = 0; i < Ethnicitycount; i++)
                    {
                        int EthnicityID = reader.ReadInt32();
                        dicEthnicityID.Add(EthnicityID, EthnicityID);
                        string EthnicityName = reader.ReadString();
                        string commandExist = string.Format("select EthnicityID from Ethnicity where EthnicityName='{0}'", EthnicityName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EthnicityID) from Ethnicity";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEthnicityID[EthnicityID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Ethnicity(EthnicityID,EthnicityName) values({0},'{1}')", dicEthnicityID[EthnicityID], EthnicityName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEthnicityID[EthnicityID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Incidence rates...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "IncidenceRates")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncidenceRatescount = reader.ReadInt32();
                pBarImport.Maximum = IncidenceRatescount;
                int maxIncidenceRateID = 0;
                string commandIncidenceRateID = "select max(IncidenceRateID) from IncidenceRates";
                object oIncidenceRateID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandIncidenceRateID);
                if (!Convert.IsDBNull(oIncidenceRateID))
                { maxIncidenceRateID = Convert.ToInt16(oIncidenceRateID); }
                for (int i = 0; i < (IncidenceRatescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < IncidenceRatescount)
                        {
                            int IncidenceRateID = reader.ReadInt32();
                            dicIncidenceRateID.Add(IncidenceRateID, IncidenceRateID);
                            IncidenceDatasetID = reader.ReadInt32();
                            int GriddefinitionID = reader.ReadInt32();
                            int EndPointGroupID = reader.ReadInt32();
                            int EndPointID = reader.ReadInt32();
                            int RaceID = reader.ReadInt32();
                            int GenderID = reader.ReadInt32();
                            int StartAge = reader.ReadInt32();
                            int EndAge = reader.ReadInt32();
                            char Prevalence = reader.ReadChar();
                            int EthnicityID = reader.ReadInt32();
                            string existIncidenceRateID = string.Format("select IncidenceRateID from IncidenceRates where IncidenceRateID={0}", IncidenceRateID);
                            object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, existIncidenceRateID);
                            // Forcing this override to compensate for an issue with inserting batches
                            //if (Convert.ToInt32(obj) > 0)
                            //{
                                dicIncidenceRateID[IncidenceRateID] = ++maxIncidenceRateID;
                            //}
                            commandText = commandText + string.Format("insert into IncidenceRates(IncidenceRateID,IncidenceDatasetID,GriddefinitionID,EndPointGroupID,EndPointID,RaceID,GenderID,StartAge,EndAge,Prevalence,EthnicityID) values({0},{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10});", dicIncidenceRateID[IncidenceRateID], dicIncidenceDatasetID[IncidenceDatasetID], gdicGridDefinition[GriddefinitionID], dicEndPointGroupID[EndPointGroupID], dicEndPointID[EndPointID], dicRaceID[RaceID], dicGenderID[GenderID], StartAge, EndAge, Prevalence, dicEthnicityID[EthnicityID]);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Incidence entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "IncidenceEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncidenceEntriescount = reader.ReadInt32();
                pBarImport.Maximum = IncidenceEntriescount;
                for (int i = 0; i < (IncidenceEntriescount / 150) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    for (int k = 0; k < 150; k++)
                    {
                        if (i * 150 + k < IncidenceEntriescount)
                        {
                            int IncidenceRateID = reader.ReadInt32();
                            commandText = commandText + string.Format("insert into Incidenceentries(IncidenceRateID,Ccolumn,Row,Vvalue) values({0},{1},{2},{3});", dicIncidenceRateID[IncidenceRateID], reader.ReadInt32(), reader.ReadInt32(), reader.ReadSingle());
                            pBarImport.PerformStep();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadPopulation(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Related grid definitions...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportGrid = new Dictionary<int, bool>();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, bool> dicExistGriddefition = new Dictionary<int, bool>();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
                    int origGridDefinitionID = griddefinitionID;
                    int oldSetupid = reader.ReadInt32();
                    string griddefinitionName = reader.ReadString();
                    string commandText = string.Format("select GriddefinitionID from griddefinitions where setupid={0} and GriddefinitionName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        dicExistGriddefition.Add(griddefinitionID, true);
                        int existgriddefinitionID = Convert.ToInt16(obj);
                        dicGriddefinitionID.Add(griddefinitionID, existgriddefinitionID);
                        reader.BaseStream.Position = reader.BaseStream.Position + 4 * sizeof(Int32);
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and was retained";
                        }
                        dicDoImportGrid.Add(origGridDefinitionID, false);
                    }
                    else
                    {
                        dicExistGriddefition.Add(griddefinitionID, false);
                        commandText = string.Format("select GriddefinitionID from griddefinitions where GriddefinitionID={0}", griddefinitionID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(GriddefinitionID) from griddefinitions";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicGriddefinitionID.Add(griddefinitionID, 1);
                                griddefinitionID = 1;
                            }
                            else
                            {
                                int maxGriddefinitionID = Convert.ToInt16(obj);
                                dicGriddefinitionID.Add(griddefinitionID, maxGriddefinitionID + 1);
                                griddefinitionID = maxGriddefinitionID + 1;
                            }
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nGrid definition \"" + griddefinitionName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nGrid definition \"" + griddefinitionName + "\" was imported";
                            }
                            dicDoImportGrid.Add(origGridDefinitionID, true);
                        }
                        //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                        commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", griddefinitionID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImportGrid[origGridDefinitionID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Related shapefiles...";
                lbProcess.Refresh();
                this.Refresh();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) return;
                for (int j = 0; j < tableCount; j++)
                {
                    int GriddefinitionID = reader.ReadInt16();
                    if (dicExistGriddefition[GriddefinitionID])
                    {
                        string filetype = reader.ReadString();
                        reader.ReadString();
                        switch (filetype)
                        {
                            case "regular":
                                reader.ReadSingle(); reader.ReadSingle(); reader.ReadInt16(); reader.ReadInt16();
                                break;
                            case "shapefile":
                                break;
                        }
                        int shapefiles = 0;
                        while (shapefiles < 3)
                        {
                            Int64 length64 = reader.ReadInt64();
                            Int64 diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                reader.ReadBytes(Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            reader.ReadBytes((Int32)(length64 - diff));
                            shapefiles++;
                        }
                    }
                    else
                    {
                        string commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID);
                        string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        string filetype = reader.ReadString();
                        string shapefilename = reader.ReadString();
                        switch (filetype)
                        {
                            case "regular":
                                commandText = string.Format("insert into regulargriddefinitiondetails(GriddefinitionID,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude,ShapefileName) values({0},{1},{2},{3},{4},'{5}')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, reader.ReadSingle(), reader.ReadSingle(), reader.ReadInt16(), reader.ReadInt16(), shapefilename);
                                if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                                {
                                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                }
                                break;
                            case "shapefile":
                                //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                                commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename,LOCKED) values({0},'{1}', 'F')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, shapefilename);
                                if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                                {
                                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                }
                                break;
                        }
                        if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                        {
                            if (!Directory.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname))
                            {
                                Directory.CreateDirectory(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname);
                            }
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp");
                            FileStream file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Create, FileAccess.Write);
                            Int64 length64 = reader.ReadInt64();
                            Int64 diff = 0;
                            byte[] array;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();

                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Create, FileAccess.Write);
                            length64 = reader.ReadInt64();
                            diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();

                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Create, FileAccess.Write);
                            length64 = reader.ReadInt64();
                            diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();
                        } else
                        // We're either in phase 1 and just scanning, or we're not importing the grid definition so we should just blindly scan through the shapefile contents so we can get to the next thing
                        {
                            // We have three segments here for shx, shp, dbf.  Skip over each of them
                            for (int i = 0; i <= 2; i++)
                            {
                                Int64 length64 = reader.ReadInt64();
                                reader.BaseStream.Position += length64;
                            }
                        }
                    }

                        pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "PopulationConfigurations")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                Dictionary<int, int> dicPopulationConfigurationID = new Dictionary<int, int>();
                Dictionary<int, int> dicPopulationDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicAgeRangeID = new Dictionary<int, int>();
                Dictionary<int, int> dicEthnicityID = new Dictionary<int, int>();
                Dictionary<int, int> dicGenderID = new Dictionary<int, int>();
                Dictionary<int, int> dicRaceID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    int PopulationConfigurationID = reader.ReadInt32();
                    string PopulationConfigurationName = reader.ReadString();
                    string commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", PopulationConfigurationName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existPopulationConfigurationID = Convert.ToInt16(obj);
                        dicPopulationConfigurationID.Add(PopulationConfigurationID, existPopulationConfigurationID);
                        PopulationConfigurationID = existPopulationConfigurationID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" exists and was retained";
                        }
                    }
                    else
                    {
                        commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationID={0}", PopulationConfigurationID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PopulationConfigurationID) from PopulationConfigurations";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicPopulationConfigurationID.Add(PopulationConfigurationID, 1); PopulationConfigurationID = 1; }
                            else
                            {
                                int maxPopulationConfigurationID = Convert.ToInt16(obj);
                                dicPopulationConfigurationID.Add(PopulationConfigurationID, maxPopulationConfigurationID + 1);
                                PopulationConfigurationID = maxPopulationConfigurationID + 1;
                            }
                        } else
                        {
                            dicPopulationConfigurationID.Add(PopulationConfigurationID, PopulationConfigurationID);
                        }

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" was imported";
                            //The 'F' is for the locked column in POPULATIONCONFIGURATIONS - this is imported and not predefined
                            commandText = string.Format("insert into PopulationConfigurations(PopulationConfigurationID,PopulationConfigurationName) values({0},'{1}')", PopulationConfigurationID, PopulationConfigurationName);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration ethnicity map...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigEthnicityMap")
                {
                    int PopConfigEthnicityMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigEthnicityMapcount;

                    // Read the ethnicity records
                    // Since we don't have a reliable counter for this, we'll loop until we find a name that doesn't look like a string
                    while(true) 
                    {

                        int EthnicityID = reader.ReadInt32();
                        string EthnicityName = reader.ReadString();
                        if(EthnicityName.StartsWith("\0"))
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position - EthnicityName.Length - 1 - sizeof(Int32);
                            break;
                        }
                        dicEthnicityID.Add(EthnicityID, EthnicityID);
                        string commandExist = string.Format("select EthnicityID from Ethnicity where EthnicityName='{0}'", EthnicityName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);

                        if (obj == null)
                        {
                            string commandText2 = "select max(EthnicityID) from Ethnicity";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText2);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEthnicityID[EthnicityID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText2 = string.Format("insert into Ethnicity(EthnicityID,EthnicityName) values({0},'{1}')", dicEthnicityID[EthnicityID], EthnicityName);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nEthnicity \"" + EthnicityName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nEthnicity \"" + EthnicityName + "\"was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText2);
                            }
                        }
                        // If this ethnicity exists, just use it
                        else
                        {
                            dicEthnicityID[EthnicityID] = Convert.ToInt16(obj);
                        }

                    } 

                    // Read the popconfig ethnicity map records
                    for (int i = 0; i < PopConfigEthnicityMapcount; i++)
                    {
                        int PopulationConfigurationID = reader.ReadInt32();
                        int EthnicityID = reader.ReadInt32();
                        string commandText = string.Format("select * from PopConfigEthnicityMap where PopulationConfigurationID={0} and EthnicityID={1}", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicEthnicityID[EthnicityID]);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj == null)
                        {
                            commandText = string.Format("insert into PopConfigEthnicityMap(PopulationConfigurationID,EthnicityID) values({0},{1})", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicEthnicityID[EthnicityID]);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }

                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration gender map...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigGenderMap")
                {
                    int PopConfigGenderMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigGenderMapcount;
                    // Import genders
                    // Since we don't have a reliable counter for this, we'll loop until we find a name that doesn't look like a string
                    while(true)
                    {
                        int GenderID = reader.ReadInt32();
                        string GenderName = reader.ReadString();
                        if (GenderName.StartsWith("\0"))
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position - GenderName.Length - 1 - sizeof(Int32);
                            break;
                        }
                        dicGenderID.Add(GenderID, GenderID);
                        string commandExist = string.Format("select GenderID from Genders where GenderName='{0}'", GenderName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(GenderID) from Genders";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicGenderID[GenderID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Genders(GenderID,GenderName) values({0},'{1}')", dicGenderID[GenderID], GenderName);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nGender \"" + GenderName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nGender \"" + GenderName + "\" was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicGenderID[GenderID] = Convert.ToInt16(obj);
                        }
                    }
                    for (int i = 0; i < PopConfigGenderMapcount; i++)
                    {
                        int PopulationConfigurationID = reader.ReadInt32();
                        int GenderID = reader.ReadInt32();
                        string commandText = string.Format("select * from PopConfigGenderMap where PopulationConfigurationID={0} and GenderID={1}", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicGenderID[GenderID]);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj == null)
                        {
                            commandText = string.Format("insert into PopConfigGenderMap(PopulationConfigurationID,GenderID) values({0},{1})", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicGenderID[GenderID]);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration racemap...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigRaceMap")
                {
                    int PopConfigRaceMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigRaceMapcount;
                    // Load Race
                    // Since we don't have a reliable counter for this, we'll loop until we find a name that doesn't look like a string
                    while(true)
                    {
                        int RaceID = reader.ReadInt32();
                        string RaceName = reader.ReadString();
                        if (RaceName.StartsWith("\0"))
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position - RaceName.Length - 1 - sizeof(Int32);
                            break;
                        }
                        dicRaceID.Add(RaceID, RaceID);
                        string commandExist = string.Format("select RaceID from Races where RaceName='{0}'", RaceName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(RaceID) from Races";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicRaceID[RaceID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Races(RaceID,RaceName) values({0},'{1}')", dicRaceID[RaceID], RaceName);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nRace \"" + RaceName + "\" will be imported";

                            }
                            else
                            {
                                strImportLog += "\nRace \"" + RaceName + "\" was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicRaceID[RaceID] = Convert.ToInt16(obj);
                        }
                    }
                    for (int i = 0; i < PopConfigRaceMapcount; i++)
                    {
                        int PopulationConfigurationID = reader.ReadInt32();
                        int RaceID = reader.ReadInt32();
                        string commandText = string.Format("select * from PopConfigRaceMap where PopulationConfigurationID={0} and RaceID={1}", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicRaceID[RaceID]);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj == null)
                        {
                            commandText = string.Format("insert into PopConfigRaceMap(PopulationConfigurationID,RaceID) values({0},{1})", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicRaceID[RaceID]);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Age ranges...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "AgeRanges")
                {
                    int AgeRangescount = reader.ReadInt32();
                    pBarImport.Maximum = AgeRangescount;
                    for (int i = 0; i < AgeRangescount; i++)
                    {
                        int AgeRangeID = reader.ReadInt32();
                        int PopulationConfigurationID = reader.ReadInt32();
                        string AgeRangeName = reader.ReadString();
                        int StartAge = reader.ReadInt32();
                        int EndAge = reader.ReadInt32();
                        string commandText = string.Format("select AgeRangeID from AgeRanges where PopulationConfigurationID={0} and AgeRangeName='{1}'", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, AgeRangeName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            dicAgeRangeID.Add(AgeRangeID, Convert.ToInt16(obj));
                        }
                        else
                        {
                            commandText = "select max(AgeRangeID) from AgeRanges";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicAgeRangeID.Add(AgeRangeID, 1); AgeRangeID = 1; }
                            else
                            {
                                int maxAgeRangeID = Convert.ToInt16(obj);
                                dicAgeRangeID.Add(AgeRangeID, maxAgeRangeID + 1);
                                AgeRangeID = maxAgeRangeID + 1;
                            }
                            commandText = string.Format("insert into AgeRanges(AgeRangeID,PopulationConfigurationID,AgeRangeName,StartAge,EndAge) values({0},{1},'{2}',{3},{4});", AgeRangeID, dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, AgeRangeName, StartAge, EndAge);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nAge Range \"" + AgeRangeName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nAge Range \"" + AgeRangeName + "\" was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population datasets...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopulationDatasets")
                {
                    int PopulationDatasetscount = reader.ReadInt32();
                    pBarImport.Maximum = PopulationDatasetscount;
                    for (int i = 0; i < PopulationDatasetscount; i++)
                    {
                        int PopulationDatasetID = reader.ReadInt32();
                        int origPopulationDatasetID = PopulationDatasetID;
                        int oldSetupid = reader.ReadInt32();
                        string PopulationDatasetName = reader.ReadString();
                        string commandText = string.Format("select PopulationDatasetID from PopulationDatasets where setupid={0} and PopulationDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, PopulationDatasetName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            int existPopulationDatasetID = Convert.ToInt16(obj);
                            /*
                            commandText = string.Format("delete from PopulationEntries where PopulationDatasetID in ({0})", existPopulationDatasetID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            commandText = string.Format("delete from PopulationGrowthWeights where PopulationDatasetID in ({0})", existPopulationDatasetID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            commandText = string.Format("delete from PopulationDatasets where PopulationDatasetID={0}", existPopulationDatasetID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            */

                            dicPopulationDatasetID.Add(PopulationDatasetID, existPopulationDatasetID);
                            PopulationDatasetID = existPopulationDatasetID;
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" exists and will be retained";
                            }
                            else
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" exists and was retained";
                            }
                            dicDoImport.Add(origPopulationDatasetID, false);
                        }
                        else
                        {
                            commandText = string.Format("select PopulationDatasetID from PopulationDatasets where PopulationDatasetID={0}", PopulationDatasetID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                commandText = "select max(PopulationDatasetID) from PopulationDatasets";
                                obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                                if (Convert.IsDBNull(obj))
                                { dicPopulationDatasetID.Add(PopulationDatasetID, 1); PopulationDatasetID = 1; }
                                else
                                {
                                    int maxPopulationDatasetID = Convert.ToInt16(obj);
                                    dicPopulationDatasetID.Add(PopulationDatasetID, maxPopulationDatasetID + 1);
                                    PopulationDatasetID = maxPopulationDatasetID + 1;
                                }
                            }
                            else
                            {
                                dicPopulationDatasetID.Add(PopulationDatasetID, PopulationDatasetID);
                            }
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" was imported";
                            }
                            dicDoImport.Add(origPopulationDatasetID, true);
                        }
                        int PopulationConfigurationID = reader.ReadInt32();
                        int GriddefinitionID = reader.ReadInt32();
                        //The 'F' is for the Locked column in PopulationDataSets - this is imported not predefined.
                        // 2015 02 12 added LOCKED to field list
                        commandText = string.Format("insert into PopulationDatasets(PopulationDatasetID,SetupID,PopulationDatasetName,PopulationConfigurationID,GriddefinitionID,ApplyGrowth,LOCKED) values({0},{1},'{2}',{3},{4},{5}, 'F')", PopulationDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, PopulationDatasetName, dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImport[origPopulationDatasetID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopulationEntries")
                {
                    Dictionary<int, List<int>> dicyyear = new Dictionary<int, List<int>>();
                    int PopulationEntriescount = reader.ReadInt32();
                    pBarImport.Maximum = PopulationEntriescount;
                    for (int i = 0; i < (PopulationEntriescount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        Boolean doBatch = false;
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < PopulationEntriescount)
                            {
                                int PopulationDatasetID = reader.ReadInt32();
                                int RaceID = reader.ReadInt32();
                                int GenderID = reader.ReadInt32();
                                int AgerangeID = reader.ReadInt32();
                                int Ccolumn = reader.ReadInt32();
                                int Row = reader.ReadInt32();
                                int Yyear = reader.ReadInt32();
                                float value = reader.ReadSingle();
                                int EthnicityID = reader.ReadInt32();
                                if (!dicyyear.ContainsKey(PopulationDatasetID))
                                {
                                    List<int> year = new List<int>();
                                    year.Add(Yyear);
                                    dicyyear.Add(PopulationDatasetID, year);
                                }
                                else if (dicyyear.ContainsKey(PopulationDatasetID) && !dicyyear[PopulationDatasetID].Contains(Yyear))
                                {
                                    dicyyear[PopulationDatasetID].Add(Yyear);
                                }
                                if (dicDoImport[PopulationDatasetID])
                                {
                                    commandText = commandText + string.Format("insert into PopulationEntries(PopulationDatasetID,RaceID,GenderID,AgerangeID,Ccolumn,Row,Yyear,Vvalue,EthnicityID) values({0},{1},{2},{3},{4},{5},{6},{7},{8});", dicPopulationDatasetID.ContainsKey(PopulationDatasetID) ? dicPopulationDatasetID[PopulationDatasetID] : PopulationDatasetID, dicRaceID.ContainsKey(RaceID) ? dicRaceID[RaceID] : RaceID, dicGenderID.ContainsKey(GenderID) ? dicGenderID[GenderID] : GenderID, dicAgeRangeID[AgerangeID], Ccolumn, Row, Yyear, value, dicEthnicityID.ContainsKey(EthnicityID) ? dicEthnicityID[EthnicityID] : EthnicityID);
                                    doBatch = true;
                                }
                                    pBarImport.PerformStep();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doBatch)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    foreach (int popdatasetid in dicyyear.Keys)
                    {
                        for (int i = 0; i < dicyyear[popdatasetid].Count; i++)
                        {
                            string commandText = string.Format("insert into T_POPULATIONDATASETIDYEAR(POPULATIONDATASETID,YYEAR) values({0},{1})", dicPopulationDatasetID.ContainsKey(popdatasetid) ? dicPopulationDatasetID[popdatasetid] : popdatasetid, dicyyear[popdatasetid][i]);
                            if (currentPhase == 2 && dicDoImport[popdatasetid])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population growth weights...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopulationGrowthWeights")
                {
                    int PopulationGrowthWeightscount = reader.ReadInt32();
                    pBarImport.Maximum = PopulationGrowthWeightscount;
                    for (int i = 0; i < (PopulationGrowthWeightscount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        Boolean doBatch = false;
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < PopulationGrowthWeightscount)
                            {
                                int PopulationDatasetID = reader.ReadInt32();
                                int Yyear = reader.ReadInt32();
                                int SourceColumn = reader.ReadInt32();
                                int SourceRow = reader.ReadInt32();
                                int TargetColumn = reader.ReadInt32();
                                int TargetRow = reader.ReadInt32();
                                int RaceID = reader.ReadInt32();
                                int EthnicityID = reader.ReadInt32();
                                if (dicDoImport[PopulationDatasetID])
                                {
                                    commandText = commandText + string.Format("insert into PopulationGrowthWeights(PopulationDatasetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue) values({0},{1},{2},{3},{4},{5},{6},{7},{8});", dicPopulationDatasetID.ContainsKey(PopulationDatasetID) ? dicPopulationDatasetID[PopulationDatasetID] : PopulationDatasetID, Yyear, SourceColumn, SourceRow, TargetColumn, TargetRow, dicRaceID.ContainsKey(RaceID) ? dicRaceID[RaceID] : RaceID, dicEthnicityID.ContainsKey(EthnicityID) ? dicEthnicityID[EthnicityID] : EthnicityID, reader.ReadSingle());
                                    doBatch = true;
                                }
                                else
                                {
                                    // Throw these values away 
                                    reader.ReadSingle();
                                }
                                pBarImport.PerformStep();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doBatch)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }

        private void ReadPopulation2(int currentPhase, BinaryReader reader)
        {
            try
            {

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                /*
                 * string nextTable = reader.ReadString();
                if (nextTable != "PopulationConfigurations")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                */
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                Dictionary<int, int> dicPopulationConfigurationID = new Dictionary<int, int>();
                Dictionary<int, int> dicPopulationDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicAgeRangeID = new Dictionary<int, int>();
                Dictionary<int, int> dicEthnicityID = new Dictionary<int, int>();
                Dictionary<int, int> dicGenderID = new Dictionary<int, int>();
                Dictionary<int, int> dicRaceID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    int PopulationConfigurationID = reader.ReadInt32();
                    string PopulationConfigurationName = reader.ReadString();
                    string commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", PopulationConfigurationName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existPopulationConfigurationID = Convert.ToInt16(obj);
                        dicPopulationConfigurationID.Add(PopulationConfigurationID, existPopulationConfigurationID);
                        PopulationConfigurationID = existPopulationConfigurationID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationID={0}", PopulationConfigurationID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PopulationConfigurationID) from PopulationConfigurations";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicPopulationConfigurationID.Add(PopulationConfigurationID, 1); PopulationConfigurationID = 1; }
                            else
                            {
                                int maxPopulationConfigurationID = Convert.ToInt16(obj);
                                dicPopulationConfigurationID.Add(PopulationConfigurationID, maxPopulationConfigurationID + 1);
                                PopulationConfigurationID = maxPopulationConfigurationID + 1;
                            }
                        }

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nPopulation configuration \"" + PopulationConfigurationName + "\" was imported";
                            //The 'F' is for the locked column in POPULATIONCONFIGURATIONS - this is imported and not predefined
                            commandText = string.Format("insert into PopulationConfigurations(PopulationConfigurationID,PopulationConfigurationName) values({0},'{1}')", PopulationConfigurationID, PopulationConfigurationName);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        doImport = true;

                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration ethnicity map...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                String nextTable = reader.ReadString();
                if (nextTable == "PopConfigEthnicityMap")
                {
                    int PopConfigEthnicityMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigEthnicityMapcount;
                    // Read the ethnicity records
                    // Since we don't have a reliable counter for this, we'll loop until we find a name that doesn't look like a string
                    while (true)
                    {
                        int EthnicityID = reader.ReadInt32();
                        string EthnicityName = reader.ReadString();
                        if (EthnicityName.StartsWith("\0"))
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position - EthnicityName.Length - 1 - sizeof(Int32);
                            break;
                        }
                        dicEthnicityID.Add(EthnicityID, EthnicityID);
                        string commandExist = string.Format("select EthnicityID from Ethnicity where EthnicityName='{0}'", EthnicityName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EthnicityID) from Ethnicity";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEthnicityID[EthnicityID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Ethnicity(EthnicityID,EthnicityName) values({0},'{1}')", dicEthnicityID[EthnicityID], EthnicityName);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nEthnicity \"" + EthnicityName + "\" will be imported";
                            } else
                            {
                                strImportLog += "\nEthnicity \"" + EthnicityName + "\"was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEthnicityID[EthnicityID] = Convert.ToInt16(obj);
                        }
                    }
                    for (int i = 0; i < PopConfigEthnicityMapcount; i++)
                    {
                        int PopulationConfigurationID = reader.ReadInt32();
                        int EthnicityID = reader.ReadInt32();
                        string commandText = string.Format("select * from PopConfigEthnicityMap where PopulationConfigurationID={0} and EthnicityID={1}", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicEthnicityID[EthnicityID]);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj == null)
                        {
                            commandText = string.Format("insert into PopConfigEthnicityMap(PopulationConfigurationID,EthnicityID) values({0},{1})", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicEthnicityID[EthnicityID]);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            } 
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration gender map...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigGenderMap")
                {
                    int PopConfigGenderMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigGenderMapcount;
                    // Import genders
                    // Since we don't have a reliable counter for this, we'll loop until we find a name that doesn't look like a string
                    while (true)
                    {
                        int GenderID = reader.ReadInt32();
                        string GenderName = reader.ReadString();
                        if (GenderName.StartsWith("\0"))
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position - GenderName.Length - 1 - sizeof(Int32);
                            break;
                        }
                        dicGenderID.Add(GenderID, GenderID);
                        string commandExist = string.Format("select GenderID from Genders where GenderName='{0}'", GenderName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(GenderID) from Genders";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicGenderID[GenderID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Genders(GenderID,GenderName) values({0},'{1}')", dicGenderID[GenderID], GenderName);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nGender \"" + GenderName + "\" will be imported";
                            } else
                            {
                                strImportLog += "\nGender \"" + GenderName + "\" was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicGenderID[GenderID] = Convert.ToInt16(obj);
                        }
                    }
                    for (int i = 0; i < PopConfigGenderMapcount; i++)
                    {
                        int PopulationConfigurationID = reader.ReadInt32();
                        int GenderID = reader.ReadInt32();
                        string commandText = string.Format("select * from PopConfigGenderMap where PopulationConfigurationID={0} and GenderID={1}", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicGenderID[GenderID]);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj == null)
                        {
                            commandText = string.Format("insert into PopConfigGenderMap(PopulationConfigurationID,GenderID) values({0},{1})", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicGenderID[GenderID]);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population configuration racemap...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigRaceMap")
                {
                    int PopConfigRaceMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigRaceMapcount;
                    // Load Race
                    // Since we don't have a reliable counter for this, we'll loop until we find a name that doesn't look like a string
                    while (true)
                    {
                        int RaceID = reader.ReadInt32();
                        string RaceName = reader.ReadString();
                        if (RaceName.StartsWith("\0"))
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position - RaceName.Length - 1 - sizeof(Int32);
                            break;
                        }
                        dicRaceID.Add(RaceID, RaceID);
                        string commandExist = string.Format("select RaceID from Races where RaceName='{0}'", RaceName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(RaceID) from Races";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicRaceID[RaceID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into Races(RaceID,RaceName) values({0},'{1}')", dicRaceID[RaceID], RaceName);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nRace \"" + RaceName + "\" will be imported";

                            } else
                            {
                                strImportLog += "\nRace \"" + RaceName + "\" was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicRaceID[RaceID] = Convert.ToInt16(obj);
                        }
                    }
                    for (int i = 0; i < PopConfigRaceMapcount; i++)
                    {
                        int PopulationConfigurationID = reader.ReadInt32();
                        int RaceID = reader.ReadInt32();
                        string commandText = string.Format("select * from PopConfigRaceMap where PopulationConfigurationID={0} and RaceID={1}", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicRaceID[RaceID]);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj == null)
                        {
                            commandText = string.Format("insert into PopConfigRaceMap(PopulationConfigurationID,RaceID) values({0},{1})", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicRaceID[RaceID]);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Age ranges...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "AgeRanges")
                {
                    int AgeRangescount = reader.ReadInt32();
                    pBarImport.Maximum = AgeRangescount;
                    for (int i = 0; i < AgeRangescount; i++)
                    {
                        int AgeRangeID = reader.ReadInt32();
                        int PopulationConfigurationID = reader.ReadInt32();
                        string AgeRangeName = reader.ReadString();
                        int StartAge = reader.ReadInt32();
                        int EndAge = reader.ReadInt32();
                        string commandText = string.Format("select AgeRangeID from AgeRanges where PopulationConfigurationID={0} and AgeRangeName='{1}'", dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, AgeRangeName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            dicAgeRangeID.Add(AgeRangeID, Convert.ToInt16(obj));
                        }
                        else
                        {
                            commandText = "select max(AgeRangeID) from AgeRanges";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicAgeRangeID.Add(AgeRangeID, 1); AgeRangeID = 1; }
                            else
                            {
                                int maxAgeRangeID = Convert.ToInt16(obj);
                                dicAgeRangeID.Add(AgeRangeID, maxAgeRangeID + 1);
                                AgeRangeID = maxAgeRangeID + 1;
                            }
                            commandText = string.Format("insert into AgeRanges(AgeRangeID,PopulationConfigurationID,AgeRangeName,StartAge,EndAge) values({0},{1},'{2}',{3},{4});", AgeRangeID, dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, AgeRangeName, StartAge, EndAge);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nAge Range \"" + AgeRangeName + "\" will be imported";
                            } else
                            {
                                strImportLog += "\nAge Range \"" + AgeRangeName + "\" was imported";
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population datasets...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopulationDatasets")
                {
                    int PopulationDatasetscount = reader.ReadInt32();
                    pBarImport.Maximum = PopulationDatasetscount;
                    for (int i = 0; i < PopulationDatasetscount; i++)
                    {
                        int PopulationDatasetID = reader.ReadInt32();
                        int oldSetupid = reader.ReadInt32();
                        string PopulationDatasetName = reader.ReadString();
                        string commandText = string.Format("select PopulationDatasetID from PopulationDatasets where setupid={0} and PopulationDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, PopulationDatasetName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            int existPopulationDatasetID = Convert.ToInt16(obj);
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" exists and will be retained";
                            } else
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" exists and was retained";
                            }
                            doImport = false;
                            /*
                                commandText = string.Format("delete from PopulationEntries where PopulationDatasetID in ({0})", existPopulationDatasetID);
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                commandText = string.Format("delete from PopulationGrowthWeights where PopulationDatasetID in ({0})", existPopulationDatasetID);
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                commandText = string.Format("delete from PopulationDatasets where PopulationDatasetID={0}", existPopulationDatasetID);
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                */

                            dicPopulationDatasetID.Add(PopulationDatasetID, existPopulationDatasetID);
                            PopulationDatasetID = existPopulationDatasetID;
                        }
                        else
                        {
                            commandText = string.Format("select PopulationDatasetID from PopulationDatasets where PopulationDatasetID={0}", PopulationDatasetID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                commandText = "select max(PopulationDatasetID) from PopulationDatasets";
                                obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                                if (Convert.IsDBNull(obj))
                                { dicPopulationDatasetID.Add(PopulationDatasetID, 1); PopulationDatasetID = 1; }
                                else
                                {
                                    int maxPopulationDatasetID = Convert.ToInt16(obj);
                                    dicPopulationDatasetID.Add(PopulationDatasetID, maxPopulationDatasetID + 1);
                                    PopulationDatasetID = maxPopulationDatasetID + 1;
                                }
                            }
                            else
                            {
                                dicPopulationDatasetID.Add(PopulationDatasetID, PopulationDatasetID);
                            }
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nPopulation dataset \"" + PopulationDatasetName + "\" was imported";
                            }
                            doImport = true;
                        }
                        int PopulationConfigurationID = reader.ReadInt32();
                        int GriddefinitionID = reader.ReadInt32();
                        //The 'F' is for the Locked column in PopulationDataSets - this is imported not predefined.
                        // 2015 02 12 added LOCKED to field list
                        commandText = string.Format("insert into PopulationDatasets(PopulationDatasetID,SetupID,PopulationDatasetName,PopulationConfigurationID,GriddefinitionID,ApplyGrowth,LOCKED) values({0},{1},'{2}',{3},{4},{5}, 'F')", PopulationDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, PopulationDatasetName, dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, gdicGridDefinition[GriddefinitionID], reader.ReadInt32());
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopulationEntries")
                {
                    Dictionary<int, List<int>> dicyyear = new Dictionary<int, List<int>>();
                    int PopulationEntriescount = reader.ReadInt32();
                    pBarImport.Maximum = PopulationEntriescount;
                    for (int i = 0; i < (PopulationEntriescount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < PopulationEntriescount)
                            {
                                int PopulationDatasetID = reader.ReadInt32();
                                int RaceID = reader.ReadInt32();
                                int GenderID = reader.ReadInt32();
                                int AgerangeID = reader.ReadInt32();
                                int Ccolumn = reader.ReadInt32();
                                int Row = reader.ReadInt32();
                                int Yyear = reader.ReadInt32();
                                float value = reader.ReadSingle();
                                int EthnicityID = reader.ReadInt32();
                                if (!dicyyear.ContainsKey(PopulationDatasetID))
                                {
                                    List<int> year = new List<int>();
                                    year.Add(Yyear);
                                    dicyyear.Add(PopulationDatasetID, year);
                                }
                                else if (dicyyear.ContainsKey(PopulationDatasetID) && !dicyyear[PopulationDatasetID].Contains(Yyear))
                                {
                                    dicyyear[PopulationDatasetID].Add(Yyear);
                                }
                                commandText = commandText + string.Format("insert into PopulationEntries(PopulationDatasetID,RaceID,GenderID,AgerangeID,Ccolumn,Row,Yyear,Vvalue,EthnicityID) values({0},{1},{2},{3},{4},{5},{6},{7},{8});", dicPopulationDatasetID.ContainsKey(PopulationDatasetID) ? dicPopulationDatasetID[PopulationDatasetID] : PopulationDatasetID, dicRaceID.ContainsKey(RaceID) ? dicRaceID[RaceID] : RaceID, dicGenderID.ContainsKey(GenderID) ? dicGenderID[GenderID] : GenderID, dicAgeRangeID[AgerangeID], Ccolumn, Row, Yyear, value, dicEthnicityID.ContainsKey(EthnicityID) ? dicEthnicityID[EthnicityID] : EthnicityID);
                                pBarImport.PerformStep();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    foreach (int popdatasetid in dicyyear.Keys)
                    {
                        for (int i = 0; i < dicyyear[popdatasetid].Count; i++)
                        {
                            string commandText = string.Format("insert into T_POPULATIONDATASETIDYEAR(POPULATIONDATASETID,YYEAR) values({0},{1})", dicPopulationDatasetID.ContainsKey(popdatasetid) ? dicPopulationDatasetID[popdatasetid] : popdatasetid, dicyyear[popdatasetid][i]);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Population growth weights...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopulationGrowthWeights")
                {
                    int PopulationGrowthWeightscount = reader.ReadInt32();
                    pBarImport.Maximum = PopulationGrowthWeightscount;
                    for (int i = 0; i < (PopulationGrowthWeightscount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < PopulationGrowthWeightscount)
                            {
                                int PopulationDatasetID = reader.ReadInt32();
                                int Yyear = reader.ReadInt32();
                                int SourceColumn = reader.ReadInt32();
                                int SourceRow = reader.ReadInt32();
                                int TargetColumn = reader.ReadInt32();
                                int TargetRow = reader.ReadInt32();
                                int RaceID = reader.ReadInt32();
                                int EthnicityID = reader.ReadInt32();
                                commandText = commandText + string.Format("insert into PopulationGrowthWeights(PopulationDatasetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue) values({0},{1},{2},{3},{4},{5},{6},{7},{8});", dicPopulationDatasetID.ContainsKey(PopulationDatasetID) ? dicPopulationDatasetID[PopulationDatasetID] : PopulationDatasetID, Yyear, SourceColumn, SourceRow, TargetColumn, TargetRow, dicRaceID.ContainsKey(RaceID) ? dicRaceID[RaceID] : RaceID, dicEthnicityID.ContainsKey(EthnicityID) ? dicEthnicityID[EthnicityID] : EthnicityID, reader.ReadSingle());
                                pBarImport.PerformStep();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }
        private void ReadCRFunction(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Health impact function datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int CrfunctionDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicCrfunctionDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicFunctionalFormID = new Dictionary<int, int>();
                Dictionary<int, int> dicBaselineFunctionalFormID = new Dictionary<int, int>();
                Dictionary<int, int> dicCrfunctionID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointGroupID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointID = new Dictionary<int, int>();
                Dictionary<int, int> dicLocationTypeID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    CrfunctionDatasetID = reader.ReadInt32();
                    dicCrfunctionDatasetID.Add(CrfunctionDatasetID, CrfunctionDatasetID);
                    int oldSetupid = reader.ReadInt32();
                    string CrfunctionDatasetName = reader.ReadString();
                    string commandText = string.Format("select CrfunctionDatasetID from CrFunctionDatasets where setupid={0} and CrfunctionDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, CrfunctionDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existCrfunctionDatasetID = Convert.ToInt16(obj);

                        dicCrfunctionDatasetID[CrfunctionDatasetID] = existCrfunctionDatasetID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" exists and was retained";
                        }
                        dicDoImport.Add(CrfunctionDatasetID, false);
                    }
                    else
                    {
                        commandText = string.Format("select CrfunctionDatasetID from CrFunctionDatasets where CrfunctionDatasetID={0}", CrfunctionDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(CrfunctionDatasetID) from CrFunctionDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                                dicCrfunctionDatasetID[CrfunctionDatasetID] = 1;
                            else
                                dicCrfunctionDatasetID[CrfunctionDatasetID] = Convert.ToInt16(obj) + 1;
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" was imported";
                        }
                        dicDoImport.Add(CrfunctionDatasetID, true);
                    }
                    //The F is for the locked column in CRFunctionDataSet - this is being imported and not predefined.
                    // added locked column to values list
                    commandText = string.Format("insert into CrFunctionDatasets(CrfunctionDatasetID,SetupID,CrfunctionDatasetName,Readonly,Locked) values({0},{1},'{2}','{3}', 'F')", dicCrfunctionDatasetID[CrfunctionDatasetID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, CrfunctionDatasetName, reader.ReadChar());
                    if (currentPhase == 2 && dicDoImport[CrfunctionDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                string nextTable = reader.ReadString();
                if (nextTable != "pollutants")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                pBarImport.Value = 0;
                lbProcess.Text = "Importing pollutants...";
                lbProcess.Refresh();
                this.Refresh();

                Dictionary<int, bool> dicDoImportPollutant = new Dictionary<int, bool>();

                tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int PollutantID = 0;
                int origPollutantID = 0;
                int existPollutantID = 0;
                Dictionary<int, int> dicpollutantid = new Dictionary<int, int>();
                Dictionary<int, int> dicpollutantseasonid = new Dictionary<int, int>();
                Dictionary<int, int> dicMetricID = new Dictionary<int, int>();
                Dictionary<int, int> dicSeasonalMetricID = new Dictionary<int, int>();
                Dictionary<int, bool> dicExistPollutant = new Dictionary<int, bool>();
                for (int i = 0; i < tableCount; i++)
                {
                    string pollutantName = reader.ReadString();
                    PollutantID = reader.ReadInt32();
                    origPollutantID = PollutantID;
                    int oldSetupid = reader.ReadInt32();
                    string commandText = string.Format("select PollutantID from pollutants where setupid={0} and PollutantName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, pollutantName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        dicExistPollutant.Add(PollutantID, true);
                        existPollutantID = Convert.ToInt16(obj);
                        dicpollutantid.Add(PollutantID, existPollutantID);
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" exists and was retained";
                        }
                        dicDoImportPollutant.Add(origPollutantID, false);
                    }
                    else
                    {
                        dicExistPollutant.Add(PollutantID, false);
                        commandText = string.Format("select PollutantID from pollutants where PollutantID={0}", PollutantID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(PollutantID) from pollutants";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            { dicpollutantid.Add(PollutantID, 1); PollutantID = 1; }
                            else
                            {
                                int maxPollutantID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                                dicpollutantid.Add(PollutantID, maxPollutantID + 1);
                                PollutantID = maxPollutantID + 1;
                            }
                        } else
                        {
                            dicpollutantid.Add(PollutantID, PollutantID);
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nPollutant \"" + pollutantName + "\" was imported";
                        }
                        dicDoImportPollutant.Add(origPollutantID, true);
                        commandText = string.Format("insert into pollutants(PollutantName,PollutantID,SetupID,ObservationType) values('{0}',{1},{2},{3})", pollutantName, PollutantID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImportPollutant[origPollutantID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    pBarImport.PerformStep();
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "PollutantSeasons" && nextTable != "Metrics")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Pollutant seasons...";
                lbProcess.Refresh();
                this.Refresh();
                int changePollutantSeasonID = 0;
                if (nextTable == "PollutantSeasons")
                {
                    int PollutantSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = PollutantSeasonscount;
                    string commandText = "select max(PollutantSeasonID) from PollutantSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changePollutantSeasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < PollutantSeasonscount; i++)
                    {
                        int pollutantseasonid = reader.ReadInt32();
                        PollutantID = reader.ReadInt32();
                        if (dicExistPollutant[PollutantID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 5 * sizeof(Int32);
                        }
                        else
                        {
                            changePollutantSeasonID++;
                            dicpollutantseasonid.Add(pollutantseasonid, changePollutantSeasonID);
                            commandText = string.Format("insert into PollutantSeasons(PollutantSeasonID,PollutantID,StartDay,EndDay,StartHour,EndHour,Numbins) values({0},{1},{2},{3},{4},{5},{6})", changePollutantSeasonID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                            if (currentPhase == 2 && dicDoImportPollutant[PollutantID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                    if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                    nextTable = reader.ReadString();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Metrics...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportMetric = new Dictionary<int, bool>();

                Dictionary<int, bool> dicExistMetric = new Dictionary<int, bool>();
                int changeMetricID = 0;
                if (nextTable == "Metrics")
                {
                    int Metricscount = reader.ReadInt32();
                    pBarImport.Maximum = Metricscount;
                    string commandText = "select max(MetricID) from Metrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < Metricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        int origMetricID = MetricID;
                        PollutantID = reader.ReadInt32();
                        String metricName = reader.ReadString();
                        int hourlyMetricGen = reader.ReadInt32();

                        // Look to see if this pollutant already has this metric
                        commandText = string.Format("select MetricID from metrics where pollutantid={0} and metricname='{1}'", dicpollutantid[PollutantID], metricName);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            dicExistMetric.Add(MetricID, true);
                            int existMetricID = Convert.ToInt16(obj);
                            if (!dicMetricID.ContainsKey(MetricID))
                            {
                                dicMetricID.Add(MetricID, existMetricID);
                            }
                            MetricID = existMetricID;

                        }
                        // We don't have a metric with this name. See if we can still use the same id.
                        else
                        {
                            dicExistMetric.Add(MetricID, false);
                            commandText = string.Format("select MetricID from Metrics where MetricID={0}", MetricID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicMetricID.Add(MetricID, ++changeMetricID);
                                MetricID = changeMetricID;
                            }
                            commandText = string.Format("insert into Metrics(MetricID,PollutantID,MetricName,HourlyMetricGeneration) values({0},{1},'{2}',{3})", MetricID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, metricName, hourlyMetricGen);
                            dicDoImportMetric.Add(origMetricID, true);

                            if (currentPhase == 2) // Not checking doImportPollutant here because, even if we already have the pollutant, we might need to add the metric
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                if (dicDoImportPollutant[PollutantID] == false)
                                {
                                    strImportLog += "\nMetric \"" + metricName + "\" was imported";
                                }
                            }
                            else
                            {
                                if (dicDoImportPollutant[PollutantID] == false)
                                {
                                    strImportLog += "\nMetric \"" + metricName + "\" will be imported";
                                }
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "FixedWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Fixed window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int FixedWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = FixedWindowMetricscount;
                    for (int i = 0; i < FixedWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 3 * sizeof(Int32);
                        }
                        else
                        {
                            string commandText = string.Format("insert into fixedwindowMetrics(MetricID,Starthour,Endhour,Statistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "MovingWindowMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Moving window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int MovingWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = MovingWindowMetricscount;
                    for (int i = 0; i < MovingWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 3 * sizeof(Int32);
                        }
                        else
                        {
                            string commandText = string.Format("insert into MovingWindowMetrics(MetricID,Windowsize,Windowstatistic,Dailystatistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "CustomMetrics")
                {
                    pBarImport.Value = 0;
                    lbProcess.Text = "Custom metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int CustomMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = CustomMetricscount;
                    for (int i = 0; i < CustomMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            reader.ReadString();
                        }
                        else
                        {
                            string commandText = string.Format("insert into CustomMetrics(MetricID,MetricFunction) values({0},'{1}')", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metrics...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportSeasonalMetric = new Dictionary<int, bool>();
                Dictionary<int, bool> dicExistSeasonalMetric = new Dictionary<int, bool>();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricID = 0;
                if (nextTable == "SeasonalMetrics")
                {
                    int SeasonalMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricscount;
                    string commandText = "select max(SeasonalMetricID) from SeasonalMetrics";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricscount; i++)
                    {
                        int SeasonalMetricID = reader.ReadInt32();
                        int origSeasonalMetricID = SeasonalMetricID;
                        int MetricID = reader.ReadInt32();
                        if (dicExistMetric[MetricID])
                        {
                            dicExistSeasonalMetric.Add(SeasonalMetricID, true);
                            string SeasonalMetricName = reader.ReadString();
                            commandText = string.Format("select seasonalmetricid from seasonalmetrics where SeasonalMetricName='{0}' and metricid={1}", SeasonalMetricName, dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicSeasonalMetricID.Add(SeasonalMetricID, Convert.ToInt16(obj));
                            }
                        }
                        else
                        {
                            dicExistSeasonalMetric.Add(SeasonalMetricID, false);
                            commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where SeasonalMetricID={0}", SeasonalMetricID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicSeasonalMetricID.Add(SeasonalMetricID, ++changeSeasonalMetricID);
                                SeasonalMetricID = changeSeasonalMetricID;
                            }
                            if(dicDoImportMetric[MetricID])
                            {
                                dicDoImportSeasonalMetric.Add(origSeasonalMetricID, true);
                            }
                            commandText = string.Format("insert into SeasonalMetrics(SeasonalMetricID,MetricID,SeasonalMetricName) values({0},{1},'{2}')", SeasonalMetricID, dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                            if (currentPhase == 2 && dicDoImportMetric[MetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Seasonal metric seasons...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                int changeSeasonalMetricseasonID = 0;
                if (nextTable == "SeasonalMetricSeasons")
                {
                    int SeasonalMetricSeasonscount = reader.ReadInt32();
                    pBarImport.Maximum = SeasonalMetricSeasonscount;
                    string commandText = "select max(SeasonalMetricseasonID) from SeasonalMetricSeasons";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        changeSeasonalMetricseasonID = Convert.ToInt16(obj);
                    }
                    for (int i = 0; i < SeasonalMetricSeasonscount; i++)
                    {
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        int SeasonalMetricID = reader.ReadInt32();
                        if (dicExistSeasonalMetric[SeasonalMetricID])
                        {
                            reader.BaseStream.Position = reader.BaseStream.Position + 3 * sizeof(Int32);
                            reader.ReadString();
                            reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                        }
                        else
                        {
                            int StartDay = reader.ReadInt32();
                            int EndDay = reader.ReadInt32();
                            int SeasonalMetricType = reader.ReadInt32();
                            string MetricFunction = reader.ReadString();
                            int PollutantseasonID = reader.ReadInt32();
                            commandText = string.Format("insert into SeasonalMetricSeasons(SeasonalMetricseasonID,SeasonalMetricID,StartDay,EndDay,SeasonalMetricType,MetricFunction,PollutantseasonID) values({0},{1},{2},{3},{4},'{5}',{6})", ++changeSeasonalMetricseasonID, dicSeasonalMetricID.ContainsKey(SeasonalMetricID) ? dicSeasonalMetricID[SeasonalMetricID] : SeasonalMetricID, Convert.ToInt16(StartDay), Convert.ToInt16(EndDay), Convert.ToInt16(SeasonalMetricType), MetricFunction, dicpollutantseasonid.ContainsKey(PollutantseasonID) ? dicpollutantseasonid[PollutantseasonID] : PollutantseasonID);
                            if (currentPhase == 2 && dicDoImportSeasonalMetric[SeasonalMetricID])
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        pBarImport.PerformStep();
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Functional forms...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "FunctionalForms")
                {
                    int FunctionalFormscount = reader.ReadInt32();
                    pBarImport.Maximum = FunctionalFormscount;
                    for (int i = 0; i < FunctionalFormscount; i++)
                    {
                        int FunctionalFormID = reader.ReadInt32();
                        dicFunctionalFormID.Add(FunctionalFormID, FunctionalFormID);
                        string FunctionalFormText = reader.ReadString();
                        string commandExist = string.Format("select FunctionalFormID from FunctionalForms where FunctionalFormText='{0}'", FunctionalFormText);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(FunctionalFormID) from FunctionalForms";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicFunctionalFormID[FunctionalFormID] = Convert.ToInt16(obj) + 1;
                            }

                            commandText = string.Format("insert into FunctionalForms(FunctionalFormID,FunctionalFormText) values({0},'{1}')", dicFunctionalFormID[FunctionalFormID], FunctionalFormText);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicFunctionalFormID[FunctionalFormID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Baseline functional forms...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "BaselineFunctionalForms")
                {
                    int BaselineFunctionalFormscount = reader.ReadInt32();
                    pBarImport.Maximum = BaselineFunctionalFormscount;
                    for (int i = 0; i < BaselineFunctionalFormscount; i++)
                    {
                        int FunctionalFormID = reader.ReadInt32();
                        dicBaselineFunctionalFormID.Add(FunctionalFormID, FunctionalFormID);
                        string FunctionalFormText = reader.ReadString();
                        string commandExist = string.Format("select FunctionalFormID from BaselineFunctionalForms where FunctionalFormText='{0}'", FunctionalFormText);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(FunctionalFormID) from BaselineFunctionalForms";
                            object max = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(max))
                            {
                                dicBaselineFunctionalFormID[FunctionalFormID] = Convert.ToInt16(max) + 1;
                            }
                            commandText = string.Format("insert into BaselineFunctionalForms(FunctionalFormID,FunctionalFormText) values({0},'{1}')", dicBaselineFunctionalFormID[FunctionalFormID], FunctionalFormText);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicBaselineFunctionalFormID[FunctionalFormID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoint groups...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndpointGroups")
                {
                    int EndpointGroupscount = reader.ReadInt32();
                    pBarImport.Maximum = EndpointGroupscount;
                    for (int i = 0; i < EndpointGroupscount; i++)
                    {
                        int EndPointGroupID = reader.ReadInt32();
                        dicEndPointGroupID.Add(EndPointGroupID, EndPointGroupID);
                        string EndPointGroupName = reader.ReadString();
                        string commandExist = string.Format("select EndPointGroupID from EndpointGroups where EndPointGroupName='{0}'", EndPointGroupName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointGroupID) from EndpointGroups";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndpointGroups(EndPointGroupID,EndPointGroupName) values({0},'{1}')", dicEndPointGroupID[EndPointGroupID], EndPointGroupName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoints...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndPoints")
                {
                    int EndPointscount = reader.ReadInt32();
                    pBarImport.Maximum = EndPointscount;
                    for (int i = 0; i < EndPointscount; i++)
                    {
                        int EndPointID = reader.ReadInt32();
                        dicEndPointID.Add(EndPointID, EndPointID);
                        int EndPointGroupID = reader.ReadInt32();
                        string EndPointName = reader.ReadString();
                        string commandExist = string.Format("select EndPointID from EndPoints where EndPointgroupID={0} and EndPointName='{1}'", dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointID) from EndPoints";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointID[EndPointID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndPoints(EndPointID,EndPointGroupID,EndPointName) values({0},{1},'{2}')", dicEndPointID[EndPointID], dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointID[EndPointID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Location type...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "LocationType")
                {
                    int LocationTypecount = reader.ReadInt32();
                    pBarImport.Maximum = LocationTypecount;
                    for (int i = 0; i < LocationTypecount; i++)
                    {
                        int oldSetupid = reader.ReadInt32();
                        int LocationTypeID = reader.ReadInt32();
                        dicLocationTypeID.Add(LocationTypeID, LocationTypeID);
                        string LocationTypeName = reader.ReadString();
                        string commandExist = string.Format("select LocationTypeID from LocationType where SetupID={0} and LocationTypeName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, LocationTypeName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(LocationTypeID) from LocationType";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicLocationTypeID[LocationTypeID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into LocationType(SetupID,LocationTypeID,LocationTypeName) values({0},{1},'{2}')", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, dicLocationTypeID[LocationTypeID], LocationTypeName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicLocationTypeID[LocationTypeID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Health impact functions...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportCrFunction = new Dictionary<int, bool>();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Crfunctions")
                {
                    int Crfunctionscount = reader.ReadInt32();
                    pBarImport.Maximum = Crfunctionscount;
                    int maxCrfunctionID = 0;
                    string commandText = "select max(CrfunctionID) from Crfunctions";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    { maxCrfunctionID = Convert.ToInt16(obj); }
                    for (int i = 0; i < Crfunctionscount; i++)
                    {
                        int CrfunctionID = reader.ReadInt32();
                        dicCrfunctionID.Add(CrfunctionID, ++maxCrfunctionID);
                        CrfunctionDatasetID = reader.ReadInt32();
                        int FunctionalFormID = reader.ReadInt32();
                        int MetricID = reader.ReadInt32();
                        int SeasonalMetricID = reader.ReadInt32();
                        int IncidenceDatasetID = reader.ReadInt32();
                        int PrevalenceDatasetID = reader.ReadInt32();
                        int VariableDatasetID = reader.ReadInt32();
                        int LocationTypeID = reader.ReadInt32();
                        int BaselineFunctionalFormID = reader.ReadInt32();
                        int EndPointgroupID = reader.ReadInt32();
                        int EndPointID = reader.ReadInt32();
                        int Pollutantid = reader.ReadInt32();
                        commandText = string.Format("insert into Crfunctions(CrfunctionID,CrfunctionDatasetID,FunctionalFormID,MetricID,SeasonalMetricID,IncidenceDatasetID,PrevalenceDatasetID,VariableDatasetID,LocationTypeID,BaselineFunctionalFormID,EndPointgroupID,EndPointID,PollutantID,Metricstatistic,Author,Yyear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,EndAge,Beta,DistBeta,P1beta,P2beta,A,NameA,B,NameB,C,NameC,Ethnicity,Percentile) values({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},'{14}',{15},'{16}','{17}','{18}','{19}','{20}','{21}',{22},{23},{24},'{25}',{26},{27},{28},'{29}',{30},'{31}',{32},'{33}','{34}',{35})", maxCrfunctionID, dicCrfunctionDatasetID[CrfunctionDatasetID], dicFunctionalFormID[FunctionalFormID], dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, SeasonalMetricID == -1 ? "NULL" : (dicSeasonalMetricID.ContainsKey(SeasonalMetricID) ? dicSeasonalMetricID[SeasonalMetricID].ToString() : SeasonalMetricID.ToString()), IncidenceDatasetID == -1 ? "NULL" : IncidenceDatasetID.ToString(), PrevalenceDatasetID == -1 ? "NULL" : PrevalenceDatasetID.ToString(), VariableDatasetID == -1 ? "NULL" : VariableDatasetID.ToString(), LocationTypeID == -1 ? "NULL" : (dicLocationTypeID[LocationTypeID].ToString()), dicBaselineFunctionalFormID[BaselineFunctionalFormID], dicEndPointGroupID[EndPointgroupID], dicEndPointID[EndPointID], dicpollutantid.ContainsKey(Pollutantid) ? dicpollutantid[Pollutantid] : Pollutantid, reader.ReadInt32(), reader.ReadString().Replace("'", "''''"), reader.ReadInt32(), reader.ReadString().Replace("'", "''''"), reader.ReadString(), reader.ReadString().Replace("'", "''''"), reader.ReadString().Replace("'", "''''"), reader.ReadString(), reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), Convert.ToDouble(reader.ReadString()), reader.ReadString(), Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()), reader.ReadString(), Convert.ToDouble(reader.ReadString()), reader.ReadString(), Convert.ToDouble(reader.ReadString()), reader.ReadString(), reader.ReadString(), reader.ReadInt32());
                        if (dicDoImport[CrfunctionDatasetID])
                        {
                            dicDoImportCrFunction.Add(CrfunctionID, true);
                        }
                        if (currentPhase == 2 && dicDoImport[CrfunctionDatasetID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Health impact function custom entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "CrFunctionCustomEntries")
                {
                    int CrFunctionCustomEntriescount = reader.ReadInt32();
                    pBarImport.Maximum = CrFunctionCustomEntriescount;
                    for (int i = 0; i < (CrFunctionCustomEntriescount / 200) + 1; i++)
                    {
                        Boolean doBatch = false;
                        string commandText = "execute block as" + " BEGIN ";
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < CrFunctionCustomEntriescount)
                            {
                                int CrFunctionID = reader.ReadInt32();
                                if (dicDoImportCrFunction.ContainsKey(CrFunctionID) && dicDoImportCrFunction[CrFunctionID])
                                {
                                    commandText = commandText + string.Format("insert into CrFunctionCustomEntries(CrFunctionID,Vvalue) values({0},{1});", dicCrfunctionID.ContainsKey(CrFunctionID) ? dicCrfunctionID[CrFunctionID] : CrFunctionID, reader.ReadSingle());
                                    doBatch = true;
                                } else
                                {
                                    // Throw it away
                                    reader.ReadSingle();
                                }
                                pBarImport.PerformStep();
                            }
                            else
                                continue;
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doBatch)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }


        private void ReadCRFunction2(int currentPhase, BinaryReader reader, int fileVersion)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Health impact function datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false;
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int CrfunctionDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicCrfunctionDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicFunctionalFormID = new Dictionary<int, int>();
                Dictionary<int, int> dicBaselineFunctionalFormID = new Dictionary<int, int>();
                Dictionary<int, int> dicCrfunctionID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointGroupID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointID = new Dictionary<int, int>();
                Dictionary<int, int> dicLocationTypeID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    CrfunctionDatasetID = reader.ReadInt32();
                    dicCrfunctionDatasetID.Add(CrfunctionDatasetID, CrfunctionDatasetID);
                    int oldSetupid = reader.ReadInt32();
                    string CrfunctionDatasetName = reader.ReadString();
                    string commandText = string.Format("select CrfunctionDatasetID from CrFunctionDatasets where setupid={0} and CrfunctionDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, CrfunctionDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existCrfunctionDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from CrFunctionCustomEntries where CrfunctionID in (select CrfunctionID from Crfunctions where CrfunctionDatasetID in ({0}))", existCrfunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from Crfunctions where CrfunctionDatasetID in ({0})", existCrfunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from CrFunctionDatasets where CrfunctionDatasetID in ({0})", existCrfunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicCrfunctionDatasetID[CrfunctionDatasetID] = existCrfunctionDatasetID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        commandText = string.Format("select CrfunctionDatasetID from CrFunctionDatasets where CrfunctionDatasetID={0}", CrfunctionDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(CrfunctionDatasetID) from CrFunctionDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                                dicCrfunctionDatasetID[CrfunctionDatasetID] = 1;
                            else
                                dicCrfunctionDatasetID[CrfunctionDatasetID] = Convert.ToInt16(obj) + 1;
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nHealth Impact Function dataset \"" + CrfunctionDatasetName + "\" was imported";
                        }
                        doImport = true;
                    }
                    //The F is for the locked column in CRFunctionDataSet - this is being imported and not predefined.
                    // added locked column to values list
                    commandText = string.Format("insert into CrFunctionDatasets(CrfunctionDatasetID,SetupID,CrfunctionDatasetName,Readonly,Locked) values({0},{1},'{2}','{3}', 'F')", dicCrfunctionDatasetID[CrfunctionDatasetID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, CrfunctionDatasetName, reader.ReadChar());
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

 
                pBarImport.Value = 0;
                lbProcess.Text = "Functional forms...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable == "FunctionalForms")
                {
                    int FunctionalFormscount = reader.ReadInt32();
                    pBarImport.Maximum = FunctionalFormscount;
                    for (int i = 0; i < FunctionalFormscount; i++)
                    {
                        int FunctionalFormID = reader.ReadInt32();
                        dicFunctionalFormID.Add(FunctionalFormID, FunctionalFormID);
                        string FunctionalFormText = reader.ReadString();
                        string commandExist = string.Format("select FunctionalFormID from FunctionalForms where FunctionalFormText='{0}'", FunctionalFormText);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(FunctionalFormID) from FunctionalForms";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicFunctionalFormID[FunctionalFormID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into FunctionalForms(FunctionalFormID,FunctionalFormText) values({0},'{1}')", dicFunctionalFormID[FunctionalFormID], FunctionalFormText);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicFunctionalFormID[FunctionalFormID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Baseline functional forms...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "BaselineFunctionalForms")
                {
                    int BaselineFunctionalFormscount = reader.ReadInt32();
                    pBarImport.Maximum = BaselineFunctionalFormscount;
                    for (int i = 0; i < BaselineFunctionalFormscount; i++)
                    {
                        int FunctionalFormID = reader.ReadInt32();
                        dicBaselineFunctionalFormID.Add(FunctionalFormID, FunctionalFormID);
                        string FunctionalFormText = reader.ReadString();
                        string commandExist = string.Format("select FunctionalFormID from BaselineFunctionalForms where FunctionalFormText='{0}'", FunctionalFormText);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(FunctionalFormID) from BaselineFunctionalForms";
                            object max = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(max))
                            {
                                dicBaselineFunctionalFormID[FunctionalFormID] = Convert.ToInt16(max) + 1;
                            }
                            commandText = string.Format("insert into BaselineFunctionalForms(FunctionalFormID,FunctionalFormText) values({0},'{1}')", dicBaselineFunctionalFormID[FunctionalFormID], FunctionalFormText);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicBaselineFunctionalFormID[FunctionalFormID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoint groups...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndpointGroups")
                {
                    int EndpointGroupscount = reader.ReadInt32();
                    pBarImport.Maximum = EndpointGroupscount;
                    for (int i = 0; i < EndpointGroupscount; i++)
                    {
                        int EndPointGroupID = reader.ReadInt32();
                        dicEndPointGroupID.Add(EndPointGroupID, EndPointGroupID);
                        string EndPointGroupName = reader.ReadString();
                        string commandExist = string.Format("select EndPointGroupID from EndpointGroups where EndPointGroupName='{0}'", EndPointGroupName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointGroupID) from EndpointGroups";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndpointGroups(EndPointGroupID,EndPointGroupName) values({0},'{1}')", dicEndPointGroupID[EndPointGroupID], EndPointGroupName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointGroupID[EndPointGroupID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoints...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndPoints")
                {
                    int EndPointscount = reader.ReadInt32();
                    pBarImport.Maximum = EndPointscount;
                    for (int i = 0; i < EndPointscount; i++)
                    {
                        int EndPointID = reader.ReadInt32();
                        dicEndPointID.Add(EndPointID, EndPointID);
                        int EndPointGroupID = reader.ReadInt32();
                        string EndPointName = reader.ReadString();
                        string commandExist = string.Format("select EndPointID from EndPoints where EndPointgroupID={0} and EndPointName='{1}'", dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointID) from EndPoints";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                dicEndPointID[EndPointID] = Convert.ToInt16(obj) + 1;
                            }
                            commandText = string.Format("insert into EndPoints(EndPointID,EndPointGroupID,EndPointName) values({0},{1},'{2}')", dicEndPointID[EndPointID], dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointID[EndPointID] = Convert.ToInt16(obj);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Health impact functions...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "Crfunctions")
                {
                    int Crfunctionscount = reader.ReadInt32();
                    pBarImport.Maximum = Crfunctionscount;
                    int maxCrfunctionID = 0;
                    string commandText = "select max(CrfunctionID) from Crfunctions";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    { maxCrfunctionID = Convert.ToInt16(obj); }
                    for (int i = 0; i < Crfunctionscount; i++)
                    {
                        int CrfunctionID = reader.ReadInt32();
                        dicCrfunctionID.Add(CrfunctionID, ++maxCrfunctionID);
                        CrfunctionDatasetID = reader.ReadInt32();
                        int FunctionalFormID = reader.ReadInt32();
                        int MetricID = reader.ReadInt32();
                        int SeasonalMetricID = reader.ReadInt32();
                        int IncidenceDatasetID = reader.ReadInt32();
                        int PrevalenceDatasetID = reader.ReadInt32();
                        int VariableDatasetID = reader.ReadInt32();
                        int LocationTypeID = reader.ReadInt32();
                        int BaselineFunctionalFormID = reader.ReadInt32();
                        int EndPointgroupID = reader.ReadInt32();
                        int EndPointID = reader.ReadInt32();
                        int Pollutantid = reader.ReadInt32();

                        int Metricstatistic = reader.ReadInt32(); 
                        string Author = reader.ReadString().Replace("'", "''''");
                        int Yyear = reader.ReadInt32();
                        string Location = reader.ReadString().Replace("'", "''''");
                        string OtherPollutants = reader.ReadString();
                        string Qualifier = reader.ReadString().Replace("'", "''''");
                        string Reference = reader.ReadString().Replace("'", "''''");
                        string Race = reader.ReadString();
                        string Gender = reader.ReadString();
                        int Startage = reader.ReadInt32();
                        int EndAge = reader.ReadInt32();
                        double Beta = Convert.ToDouble(reader.ReadString());
                        string DistBeta = reader.ReadString();
                        double P1beta = Convert.ToDouble(reader.ReadString());
                        double P2beta = Convert.ToDouble(reader.ReadString());
                        double ValA = Convert.ToDouble(reader.ReadString());
                        string NameA = reader.ReadString();
                        double ValB = Convert.ToDouble(reader.ReadString());
                        string NameB = reader.ReadString();
                        double ValC = Convert.ToDouble(reader.ReadString());
                        string NameC = reader.ReadString();
                        string Ethnicity = reader.ReadString();
                        int Percentile = reader.ReadInt32();
                        int GeographicAreaID = reader.ReadInt32();
                        string GeographicAreaFeatureID = null;
                        if(fileVersion >= 3)
                        {
                            GeographicAreaFeatureID = reader.ReadString();
                        }

                    commandText = string.Format("insert into Crfunctions(CrfunctionID,CrfunctionDatasetID,FunctionalFormID,MetricID,SeasonalMetricID,IncidenceDatasetID,PrevalenceDatasetID,VariableDatasetID,LocationTypeID,BaselineFunctionalFormID,EndPointgroupID,EndPointID,PollutantID,Metricstatistic,Author,Yyear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,EndAge,Beta,DistBeta,P1beta,P2beta,A,NameA,B,NameB,C,NameC,Ethnicity,Percentile,GeographicAreaID,GeographicAreaFeatureID) values({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},'{14}',{15},'{16}','{17}','{18}','{19}','{20}','{21}',{22},{23},{24},'{25}',{26},{27},{28},'{29}',{30},'{31}',{32},'{33}','{34}',{35}, {36}, {37})",
                        maxCrfunctionID, dicCrfunctionDatasetID[CrfunctionDatasetID], dicFunctionalFormID[FunctionalFormID], gdicMetric[MetricID], gdicSeasonalMetric.ContainsKey(SeasonalMetricID) ? gdicSeasonalMetric[SeasonalMetricID].ToString() : "NULL", 
                        gdicIncidence.ContainsKey(IncidenceDatasetID) ? gdicIncidence[IncidenceDatasetID].ToString() : "NULL", gdicPrevalence.ContainsKey(PrevalenceDatasetID) ? PrevalenceDatasetID.ToString() : "NULL", 
                        gdicVariable.ContainsKey(VariableDatasetID) ? gdicVariable[VariableDatasetID].ToString() : "NULL", LocationTypeID == -1 ? "NULL" : (dicLocationTypeID[LocationTypeID].ToString()), 
                        dicBaselineFunctionalFormID[BaselineFunctionalFormID], dicEndPointGroupID[EndPointgroupID], dicEndPointID[EndPointID], gdicPollutant[Pollutantid],
                        Metricstatistic, Author, Yyear, Location, OtherPollutants, Qualifier, Reference, Race, Gender, Startage, EndAge, Beta, DistBeta, P1beta, P2beta, ValA, NameA, ValB, NameB, ValC, NameC, 
                        Ethnicity, Percentile, GeographicAreaID == -1 ? "NULL" : (gdicGeographicArea[GeographicAreaID].ToString()), GeographicAreaFeatureID == null ? "NULL" : "'" + GeographicAreaFeatureID + "'");
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Health impact function custom entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "CrFunctionCustomEntries")
                {
                    int CrFunctionCustomEntriescount = reader.ReadInt32();
                    pBarImport.Maximum = CrFunctionCustomEntriescount;
                    for (int i = 0; i < (CrFunctionCustomEntriescount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < CrFunctionCustomEntriescount)
                            {
                                int CrFunctionID = reader.ReadInt32();
                                commandText = commandText + string.Format("insert into CrFunctionCustomEntries(CrFunctionID,Vvalue) values({0},{1});", dicCrfunctionID.ContainsKey(CrFunctionID) ? dicCrfunctionID[CrFunctionID] : CrFunctionID, reader.ReadSingle());
                                pBarImport.PerformStep();
                            }
                            else
                                continue;
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadVariable(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Variable datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>(); ;
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int SetupVariableDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicSetupVariableDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicSetupVariableID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    SetupVariableDatasetID = reader.ReadInt32();
                    int origSetupVariableDatasetID = SetupVariableDatasetID;
                    int oldSetupid = reader.ReadInt32();
                    string SetupVariableDatasetName = reader.ReadString();
                    string commandText = string.Format("select SetupVariableDatasetID from SetupVariableDatasets where setupid={0} and SetupVariableDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, SetupVariableDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existSetupVariableDatasetID = Convert.ToInt16(obj);

                        dicSetupVariableDatasetID.Add(SetupVariableDatasetID, existSetupVariableDatasetID);
                        SetupVariableDatasetID = existSetupVariableDatasetID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origSetupVariableDatasetID, false);
                    }
                    else
                    {
                        commandText = string.Format("select SetupVariableDatasetID from SetupVariableDatasets where SetupVariableDatasetID={0}", SetupVariableDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(SetupVariableDatasetID) from SetupVariableDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicSetupVariableDatasetID.Add(SetupVariableDatasetID, 1);
                                SetupVariableDatasetID = 1;
                            }
                            else
                            {
                                int maxSetupVariableDatasetID = Convert.ToInt16(obj);
                                dicSetupVariableDatasetID.Add(SetupVariableDatasetID, maxSetupVariableDatasetID + 1);
                                SetupVariableDatasetID = maxSetupVariableDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" was imported";
                        }
                        dicDoImport.Add(origSetupVariableDatasetID, true);
                    }
                    //The 'F' is for the Locked column in SetUpVariableDataSets - this is improted and not predefined
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into SetupVariableDatasets(SetupVariableDatasetID,SetupID,SetupVariableDatasetName,LOCKED) values({0},{1},'{2}', 'F')", SetupVariableDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, SetupVariableDatasetName);
                    if (currentPhase == 2 && dicDoImport[origSetupVariableDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "griddefinitions")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                pBarImport.Value = 0;
                lbProcess.Text = "Related grid definitions...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImportGrid = new Dictionary<int, bool>();
                tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, bool> dicExistGriddefinition = new Dictionary<int, bool>();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
                    int origGridDefinitionID = griddefinitionID;
                    int oldSetupid = reader.ReadInt32();
                    string griddefinitionName = reader.ReadString();
                    string commandText = string.Format("select GriddefinitionID from griddefinitions where setupid={0} and GriddefinitionName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        dicExistGriddefinition.Add(griddefinitionID, true);
                        int existgriddefinitionID = Convert.ToInt16(obj);
                        dicGriddefinitionID.Add(griddefinitionID, existgriddefinitionID);
                        reader.BaseStream.Position = reader.BaseStream.Position + 4 * sizeof(Int32);
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nGrid definition \"" + griddefinitionName + "\" exists and was retained";
                        }
                        dicDoImportGrid.Add(origGridDefinitionID, false);
                    }
                    else
                    {
                        dicExistGriddefinition.Add(griddefinitionID, false);
                        commandText = string.Format("select GriddefinitionID from griddefinitions where GriddefinitionID={0}", griddefinitionID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(GriddefinitionID) from griddefinitions";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicGriddefinitionID.Add(griddefinitionID, 1);
                                griddefinitionID = 1;
                            }
                            else
                            {
                                int maxGriddefinitionID = Convert.ToInt16(obj);
                                dicGriddefinitionID.Add(griddefinitionID, maxGriddefinitionID + 1);
                                griddefinitionID = maxGriddefinitionID + 1;

                            }
                            if (currentPhase == 1)
                            {
                                strImportLog += "\nGrid definition \"" + griddefinitionName + "\" will be imported";
                            }
                            else
                            {
                                strImportLog += "\nGrid definition \"" + griddefinitionName + "\" was imported";
                            }
                            dicDoImportGrid.Add(origGridDefinitionID, true);
                        }
                        //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                        commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", griddefinitionID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        if (currentPhase == 2 && dicDoImportGrid[origGridDefinitionID])
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Related shapefiles...";
                lbProcess.Refresh();
                this.Refresh();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) return;
                for (int j = 0; j < tableCount; j++)
                {
                    int GriddefinitionID = reader.ReadInt16();
                    if (dicExistGriddefinition[GriddefinitionID])
                    {
                        string filetype = reader.ReadString();
                        reader.ReadString();
                        switch (filetype)
                        {
                            case "regular":
                                reader.ReadSingle(); reader.ReadSingle(); reader.ReadInt16(); reader.ReadInt16();
                                break;
                            case "shapefile":
                                break;
                        }
                        int shapefiles = 0;
                        while (shapefiles < 3)
                        {
                            Int64 length64 = reader.ReadInt64();
                            Int64 diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                reader.ReadBytes(Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            reader.ReadBytes((Int32)(length64 - diff));
                            shapefiles++;
                        }
                    }
                    else
                    {
                        string commandText = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID);
                        string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        string filetype = reader.ReadString();
                        string shapefilename = reader.ReadString();
                        switch (filetype)
                        {
                            case "regular":
                                commandText = string.Format("insert into regulargriddefinitiondetails(GriddefinitionID,MinimumLatitude,MinimumLongitude,ColumnSperlongitude,RowSperlatitude,ShapefileName) values({0},{1},{2},{3},{4},'{5}')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, reader.ReadSingle(), reader.ReadSingle(), reader.ReadInt16(), reader.ReadInt16(), shapefilename);
                                if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                                {
                                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                }
                                break;
                            case "shapefile":
                                //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                                commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename, LOCKED) values({0},'{1}', 'F')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, shapefilename);
                                if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                                {
                                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                }
                                break;
                        }
                        if (currentPhase == 2 && dicDoImportGrid[GriddefinitionID])
                        {
                            if (!Directory.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname))
                            {
                                Directory.CreateDirectory(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname);
                            }
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp");
                            FileStream file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Create, FileAccess.Write);
                            Int64 length64 = reader.ReadInt64();
                            Int64 diff = 0;
                            byte[] array;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();

                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Create, FileAccess.Write);
                            length64 = reader.ReadInt64();
                            diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();

                            file = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Create, FileAccess.Write);
                            length64 = reader.ReadInt64();
                            diff = 0;
                            while (diff <= length64 - Int32.MaxValue)
                            {
                                array = reader.ReadBytes(Int32.MaxValue);
                                file.Write(array, 0, Int32.MaxValue);
                                diff += Int32.MaxValue;
                            }
                            array = reader.ReadBytes((Int32)(length64 - diff));
                            file.Write(array, 0, (Int32)(length64 - diff));
                            file.Close();
                        }
                        else
                        // We're either in phase 1 and just scanning, or we're not importing the grid definition so we should just blindly scan through the shapefile contents so we can get to the next thing
                        {
                            // We have three segments here for shx, shp, dbf.  Skip over each of them
                            for (int i = 0; i <= 2; i++)
                            {
                                Int64 length64 = reader.ReadInt64();
                                reader.BaseStream.Position += length64;
                            }
                        }
                    }

                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Variables...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "SetupVariables")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int SetupVariablescount = reader.ReadInt32();
                pBarImport.Maximum = SetupVariablescount;
                int maxSetupVariableID = 0;
                Dictionary<int, bool> dicDoImportVariable = new Dictionary<int, bool>();

                string commandmaxSetupVariableID = "select max(SetupVariableID) from SetupVariables";
                object oSetupVariableID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandmaxSetupVariableID);
                if (!Convert.IsDBNull(oSetupVariableID))
                { maxSetupVariableID = Convert.ToInt16(oSetupVariableID); }
                for (int i = 0; i < (SetupVariablescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    Boolean doBatch = false;
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < SetupVariablescount)
                        {
                            int SetupVariableID = reader.ReadInt32();
                            dicSetupVariableID.Add(SetupVariableID, maxSetupVariableID + k + 1);
                            SetupVariableDatasetID = reader.ReadInt32();
                            string SetupVariableName = reader.ReadString();
                            int Griddefinitionid = reader.ReadInt32();
                            if (dicDoImport[SetupVariableDatasetID])
                            {
                                commandText = commandText + string.Format("insert into SetupVariables(SetupVariableID,SetupVariableDatasetID,SetupVariableName,GriddefinitionID) values({0},{1},'{2}',{3});", maxSetupVariableID + k + 1, dicSetupVariableDatasetID.ContainsKey(SetupVariableDatasetID) ? dicSetupVariableDatasetID[SetupVariableDatasetID] : SetupVariableDatasetID, SetupVariableName, dicGriddefinitionID.ContainsKey(Griddefinitionid) ? dicGriddefinitionID[Griddefinitionid] : Griddefinitionid);
                                dicDoImportVariable.Add(SetupVariableID, true);
                                doBatch = true;
                            } else
                            {
                                dicDoImportVariable.Add(SetupVariableID, false);
                            }
                            pBarImport.PerformStep();
                        }
                        else
                            continue;
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doBatch)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Geographic variables...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "SetupGeographicVariables")
                {
                    int SetupGeographicVariablescount = reader.ReadInt32();
                    pBarImport.Maximum = SetupGeographicVariablescount;
                    for (int i = 0; i < (SetupGeographicVariablescount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        Boolean doBatch = false;
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < SetupGeographicVariablescount)
                            {
                                int SetupVariableID = reader.ReadInt32();
                                if(dicDoImportVariable[SetupVariableID])
                                {
                                    commandText = commandText + string.Format("insert into SetupGeographicVariables(SetupVariableID,Ccolumn,Row,Vvalue) values({0},{1},{2},{3});", dicSetupVariableID.ContainsKey(SetupVariableID) ? dicSetupVariableID[SetupVariableID] : SetupVariableID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadSingle());
                                    doBatch = true;

                                } else
                                {
                                    reader.ReadInt32(); reader.ReadInt32(); reader.ReadSingle();
                                }
                                pBarImport.PerformStep();
                            }
                            else
                                continue;
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doBatch)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Global variables...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "SetupGlobalVariables")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int SetupGlobalVariablescount = reader.ReadInt32();
                pBarImport.Maximum = SetupGlobalVariablescount;
                for (int i = 0; i < (SetupGlobalVariablescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    Boolean doBatch = false;
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < SetupGlobalVariablescount)
                        {
                            int SetupVariableID = reader.ReadInt32();
                            if(dicDoImportVariable[SetupVariableID])
                            {
                                commandText = commandText + string.Format("insert into SetupGlobalVariables(SetupVariableID,Vvalue) values({0},{1});", dicSetupVariableID.ContainsKey(SetupVariableID) ? dicSetupVariableID[SetupVariableID] : SetupVariableID, reader.ReadSingle());
                                doBatch = true;
                            } else
                            {
                                reader.ReadSingle();
                            }
                            pBarImport.PerformStep();
                        }
                        else
                            continue;
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doBatch)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }


        private void ReadVariable2(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Variable datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false; 

                int tableCount = reader.ReadInt32();
                int origSetupVariableDatasetID;
                pBarImport.Maximum = tableCount;
                int SetupVariableDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicSetupVariableDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicSetupVariableID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    SetupVariableDatasetID = reader.ReadInt32();
                    origSetupVariableDatasetID = SetupVariableDatasetID;
                    int oldSetupid = reader.ReadInt32();
                    string SetupVariableDatasetName = reader.ReadString();
                    string commandText = string.Format("select SetupVariableDatasetID from SetupVariableDatasets where setupid={0} and SetupVariableDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, SetupVariableDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existSetupVariableDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from SetupGlobalVariables where SetupVariableID in (select SetupVariableID from SetupVariables where SetupVariableDatasetID in ({0}))", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from SetupGeographicVariables where SetupVariableID in (select SetupVariableID from SetupVariables where SetupVariableDatasetID in ({0}))", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from SetupVariables where SetupVariableDatasetID in ({0})", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from SetupVariableDatasets where SetupVariableDatasetID in ({0})", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicSetupVariableDatasetID.Add(SetupVariableDatasetID, existSetupVariableDatasetID);
                        SetupVariableDatasetID = existSetupVariableDatasetID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        commandText = string.Format("select SetupVariableDatasetID from SetupVariableDatasets where SetupVariableDatasetID={0}", SetupVariableDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(SetupVariableDatasetID) from SetupVariableDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicSetupVariableDatasetID.Add(SetupVariableDatasetID, 1);
                                SetupVariableDatasetID = 1;
                            }
                            else
                            {
                                int maxSetupVariableDatasetID = Convert.ToInt16(obj);
                                dicSetupVariableDatasetID.Add(SetupVariableDatasetID, maxSetupVariableDatasetID + 1);
                                SetupVariableDatasetID = maxSetupVariableDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nVariable dataset \"" + SetupVariableDatasetName + "\" was imported";
                        }
                        doImport = true;
                    }

                    // Map the old id to the new so the HIF import has it
                    gdicVariable[origSetupVariableDatasetID] = SetupVariableDatasetID;

                    //The 'F' is for the Locked column in SetUpVariableDataSets - this is improted and not predefined
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into SetupVariableDatasets(SetupVariableDatasetID,SetupID,SetupVariableDatasetName,LOCKED) values({0},{1},'{2}', 'F')", SetupVariableDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, SetupVariableDatasetName);
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Variables...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "SetupVariables")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int SetupVariablescount = reader.ReadInt32();
                pBarImport.Maximum = SetupVariablescount;
                int maxSetupVariableID = 0;
                string commandmaxSetupVariableID = "select max(SetupVariableID) from SetupVariables";
                object oSetupVariableID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandmaxSetupVariableID);
                if (!Convert.IsDBNull(oSetupVariableID))
                { maxSetupVariableID = Convert.ToInt16(oSetupVariableID); }
                for (int i = 0; i < (SetupVariablescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < SetupVariablescount)
                        {
                            int SetupVariableID = reader.ReadInt32();
                            dicSetupVariableID.Add(SetupVariableID, maxSetupVariableID + k + 1);
                            SetupVariableDatasetID = reader.ReadInt32();
                            string SetupVariableName = reader.ReadString();
                            int Griddefinitionid = reader.ReadInt32();
                            commandText = commandText + string.Format("insert into SetupVariables(SetupVariableID,SetupVariableDatasetID,SetupVariableName,GriddefinitionID) values({0},{1},'{2}',{3});", maxSetupVariableID + k + 1, dicSetupVariableDatasetID.ContainsKey(SetupVariableDatasetID) ? dicSetupVariableDatasetID[SetupVariableDatasetID] : SetupVariableDatasetID, SetupVariableName, gdicGridDefinition[Griddefinitionid]);
                            pBarImport.PerformStep();
                        }
                        else
                            continue;
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Geographic variables...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "SetupGeographicVariables")
                {
                    int SetupGeographicVariablescount = reader.ReadInt32();
                    pBarImport.Maximum = SetupGeographicVariablescount;
                    for (int i = 0; i < (SetupGeographicVariablescount / 200) + 1; i++)
                    {
                        string commandText = "execute block as" + " BEGIN ";
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < SetupGeographicVariablescount)
                            {
                                int SetupVariableID = reader.ReadInt32();
                                commandText = commandText + string.Format("insert into SetupGeographicVariables(SetupVariableID,Ccolumn,Row,Vvalue) values({0},{1},{2},{3});", dicSetupVariableID.ContainsKey(SetupVariableID) ? dicSetupVariableID[SetupVariableID] : SetupVariableID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadSingle());
                                pBarImport.PerformStep();
                            }
                            else
                                continue;
                        }
                        commandText = commandText + "END";
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Global variables...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "SetupGlobalVariables")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int SetupGlobalVariablescount = reader.ReadInt32();
                pBarImport.Maximum = SetupGlobalVariablescount;
                for (int i = 0; i < (SetupGlobalVariablescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < SetupGlobalVariablescount)
                        {
                            int SetupVariableID = reader.ReadInt32();
                            commandText = commandText + string.Format("insert into SetupGlobalVariables(SetupVariableID,Vvalue) values({0},{1});", dicSetupVariableID.ContainsKey(SetupVariableID) ? dicSetupVariableID[SetupVariableID] : SetupVariableID, reader.ReadSingle());
                            pBarImport.PerformStep();
                        }
                        else
                            continue;
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }


        private void ReadInflation(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Inflation datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int InflationDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicInflationDatasetID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    InflationDatasetID = reader.ReadInt32();
                    int origInflationDatasetID = InflationDatasetID;
                    int oldSetupid = reader.ReadInt32();
                    string InflationDatasetName = reader.ReadString();
                    string commandText = string.Format("select InflationDatasetID from InflationDatasets where setupid={0} and InflationDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, InflationDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existInflationDatasetID = Convert.ToInt16(obj);
                        dicInflationDatasetID.Add(InflationDatasetID, existInflationDatasetID);
                        InflationDatasetID = existInflationDatasetID;
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origInflationDatasetID, false);
                    }
                    else
                    {
                        commandText = string.Format("select InflationDatasetID from InflationDatasets where InflationDatasetID={0}", InflationDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(InflationDatasetID) from InflationDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicInflationDatasetID.Add(InflationDatasetID, 1);
                                InflationDatasetID = 1;
                            }
                            else
                            {
                                int maxInflationDatasetID = Convert.ToInt16(obj);
                                dicInflationDatasetID.Add(InflationDatasetID, maxInflationDatasetID + 1);
                                InflationDatasetID = maxInflationDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" was imported";
                        }
                        dicDoImport.Add(origInflationDatasetID, true);
                    }
                    //The 'F' is for the locked column in inflationdatasets - this is imported not predefined
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into InflationDatasets(InflationDatasetID,SetupID,InflationDatasetName, LOCKED) values({0},{1},'{2}', 'F')", InflationDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, InflationDatasetName);
                    if (currentPhase == 2 && dicDoImport[origInflationDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Inflation entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "InflationEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncomeGrowthAdjFactorscount = reader.ReadInt32();
                pBarImport.Maximum = IncomeGrowthAdjFactorscount;
                for (int i = 0; i < IncomeGrowthAdjFactorscount; i++)
                {
                    InflationDatasetID = reader.ReadInt32();
                    string commandText = string.Format("insert into InflationEntries(InflationDatasetID,Yyear,AllGoodsIndex,MedicalCostIndex,WageIndex) values({0},{1},{2},{3},{4})", dicInflationDatasetID.ContainsKey(InflationDatasetID) ? dicInflationDatasetID[InflationDatasetID] : InflationDatasetID, reader.ReadInt32(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    if (currentPhase == 2 && dicDoImport[InflationDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }


        private void ReadInflation2(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Inflation datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false;
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int InflationDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicInflationDatasetID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    InflationDatasetID = reader.ReadInt32();
                    int oldSetupid = reader.ReadInt32();
                    string InflationDatasetName = reader.ReadString();
                    string commandText = string.Format("select InflationDatasetID from InflationDatasets where setupid={0} and InflationDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, InflationDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existInflationDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from InflationEntries where InflationDatasetID in ({0})", existInflationDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from InflationDatasets where InflationDatasetID in ({0})", existInflationDatasetID);
                        */
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicInflationDatasetID.Add(InflationDatasetID, existInflationDatasetID);
                        InflationDatasetID = existInflationDatasetID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        commandText = string.Format("select InflationDatasetID from InflationDatasets where InflationDatasetID={0}", InflationDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(InflationDatasetID) from InflationDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicInflationDatasetID.Add(InflationDatasetID, 1);
                                InflationDatasetID = 1;
                            }
                            else
                            {
                                int maxInflationDatasetID = Convert.ToInt16(obj);
                                dicInflationDatasetID.Add(InflationDatasetID, maxInflationDatasetID + 1);
                                InflationDatasetID = maxInflationDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nInflation dataset \"" + InflationDatasetName + "\" was imported";
                        }
                        doImport = true;
                    }
                    //The 'F' is for the locked column in inflationdatasets - this is imported not predefined
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into InflationDatasets(InflationDatasetID,SetupID,InflationDatasetName, LOCKED) values({0},{1},'{2}', 'F')", InflationDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, InflationDatasetName);
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Inflation entries...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "InflationEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncomeGrowthAdjFactorscount = reader.ReadInt32();
                pBarImport.Maximum = IncomeGrowthAdjFactorscount;
                for (int i = 0; i < IncomeGrowthAdjFactorscount; i++)
                {
                    InflationDatasetID = reader.ReadInt32();
                    string commandText = string.Format("insert into InflationEntries(InflationDatasetID,Yyear,AllGoodsIndex,MedicalCostIndex,WageIndex) values({0},{1},{2},{3},{4})", dicInflationDatasetID.ContainsKey(InflationDatasetID) ? dicInflationDatasetID[InflationDatasetID] : InflationDatasetID, reader.ReadInt32(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }

        private void ReadValuation(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Valuation function datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int ValuationFunctionDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicValuationFunctionDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicFunctionalFormID = new Dictionary<int, int>();
                Dictionary<int, int> dicValuationFunctionID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointGroupID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    ValuationFunctionDatasetID = reader.ReadInt32();
                    int origValuationFunctionDatasetID = ValuationFunctionDatasetID;
                    int oldSetupid = reader.ReadInt32();
                    string ValuationFunctionDatasetName = reader.ReadString();
                    string commandText = string.Format("select ValuationFunctionDatasetID from ValuationFunctionDatasets where setupid={0} and ValuationFunctionDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, ValuationFunctionDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existValuationFunctionDatasetID = Convert.ToInt16(obj);

                        dicValuationFunctionDatasetID.Add(ValuationFunctionDatasetID, existValuationFunctionDatasetID);
                        ValuationFunctionDatasetID = existValuationFunctionDatasetID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origValuationFunctionDatasetID, false);
                    }
                    else
                    {
                        commandText = string.Format("select ValuationFunctionDatasetID from ValuationFunctionDatasets where ValuationFunctionDatasetID={0}", ValuationFunctionDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(ValuationFunctionDatasetID) from ValuationFunctionDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicValuationFunctionDatasetID.Add(ValuationFunctionDatasetID, 1);
                                ValuationFunctionDatasetID = 1;
                            }
                            else
                            {
                                int maxValuationFunctionDatasetID = Convert.ToInt16(obj);
                                dicValuationFunctionDatasetID.Add(ValuationFunctionDatasetID, maxValuationFunctionDatasetID + 1);
                                ValuationFunctionDatasetID = maxValuationFunctionDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" was imported";
                        }
                        dicDoImport.Add(origValuationFunctionDatasetID, true);
                    }
                    //The 'F' is for the locked column in ValuationFunctionDataSets - this is imported and is not predefined.
                    // 2015 02 12 added LOCKED to field list - also removed extra 'F' from values list
                    commandText = string.Format("insert into ValuationFunctionDatasets(ValuationFunctionDatasetID,SetupID,ValuationFunctionDatasetName,Readonly, LOCKED) values({0},{1},'{2}','{3}', 'F')", ValuationFunctionDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, ValuationFunctionDatasetName, reader.ReadChar());
                    if (currentPhase == 2 && dicDoImport[origValuationFunctionDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Valuation functional forms...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "ValuationFunctionalForms")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int ValuationFunctionalFormscount = reader.ReadInt32();
                pBarImport.Maximum = ValuationFunctionalFormscount;
                int maxFunctionalFormID = 1;
                for (int i = 0; i < ValuationFunctionalFormscount; i++)
                {
                    int FunctionalFormID = reader.ReadInt32();
                    string FunctionalFormText = reader.ReadString();
                    string commandExist = string.Format("select FunctionalFormID from ValuationFunctionalForms where FunctionalFormText='{0}'", FunctionalFormText);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                    if (obj == null)
                    {
                        string commandText = "select max(FunctionalFormID) from ValuationFunctionalForms";
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (!Convert.IsDBNull(obj))
                        {
                            maxFunctionalFormID = Convert.ToInt16(obj);
                            dicFunctionalFormID.Add(FunctionalFormID, ++maxFunctionalFormID);
                        }
                        else
                        { dicFunctionalFormID.Add(FunctionalFormID, 1); }
                        commandText = string.Format("insert into ValuationFunctionalForms(FunctionalFormID,FunctionalFormText) values({0},'{1}')", maxFunctionalFormID, FunctionalFormText);
                        if (currentPhase == 2)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    else
                    { dicFunctionalFormID.Add(FunctionalFormID, Convert.ToInt16(obj)); }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoint groups...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndpointGroups")
                {
                    int EndpointGroupscount = reader.ReadInt32();
                    pBarImport.Maximum = EndpointGroupscount;
                    for (int i = 0; i < EndpointGroupscount; i++)
                    {
                        int EndPointGroupID = reader.ReadInt32();
                        int maxEndPointGroupID = 1;
                        string EndPointGroupName = reader.ReadString();
                        string commandExist = string.Format("select EndPointGroupID from EndpointGroups where EndPointGroupName='{0}'", EndPointGroupName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointGroupID) from EndpointGroups";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                maxEndPointGroupID = Convert.ToInt16(obj);
                                dicEndPointGroupID.Add(EndPointGroupID, ++maxEndPointGroupID);
                            }
                            commandText = string.Format("insert into EndpointGroups(EndPointGroupID,EndPointGroupName) values({0},'{1}')", maxEndPointGroupID, EndPointGroupName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointGroupID.Add(EndPointGroupID, Convert.ToInt16(obj));
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoints...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndPoints")
                {
                    int EndPointscount = reader.ReadInt32();
                    pBarImport.Maximum = EndPointscount;
                    for (int i = 0; i < EndPointscount; i++)
                    {
                        int EndPointID = reader.ReadInt32();
                        int EndPointGroupID = reader.ReadInt32();
                        int maxEndPointID = 1;
                        string EndPointName = reader.ReadString();
                        string commandExist = string.Format("select EndPointID from EndPoints where EndPointgroupID={0} and EndPointName='{1}'", dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointID) from EndPoints";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                maxEndPointID = Convert.ToInt16(obj);
                                dicEndPointID.Add(EndPointID, ++maxEndPointID);
                            }
                            commandText = string.Format("insert into EndPoints(EndPointID,EndPointGroupID,EndPointName) values({0},{1},'{2}')", maxEndPointID, dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                            if (currentPhase == 2)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointID.Add(EndPointID, Convert.ToInt16(obj));
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Valuation functions...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "ValuationFunctions")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int ValuationFunctionscount = reader.ReadInt32();
                pBarImport.Maximum = ValuationFunctionscount;
                Dictionary<int, bool> dicDoImportFunction = new Dictionary<int, bool>();
                for (int i = 0; i < ValuationFunctionscount; i++)
                {
                    int ValuationFunctionID = reader.ReadInt32();
                    int origValuationFunctionID = ValuationFunctionID;
                    string commandText = "select max(ValuationFunctionID) from ValuationFunctions";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (Convert.IsDBNull(obj))
                    {
                        dicValuationFunctionID.Add(ValuationFunctionID, 1); ValuationFunctionID = 1;
                    }
                    else
                    {
                        int maxValuationFunctionID = Convert.ToInt16(obj);
                        dicValuationFunctionID.Add(ValuationFunctionID, ++maxValuationFunctionID);
                        ValuationFunctionID = maxValuationFunctionID;
                    }
                    ValuationFunctionDatasetID = reader.ReadInt32();
                    int FunctionalFormID = reader.ReadInt32();
                    commandText = string.Format("insert into ValuationFunctions(ValuationFunctionID,ValuationFunctionDatasetID,FunctionalFormID,EndPointGroupID,EndPointID,Qualifier," +
                                                "Reference,StartAge,EndAge,NameA,DistA,NameB,NameC,NameD,A,P1A,P2A,B,C,D) " +
                                                "values({0},{1},{2},{3},{4},'{5}','{6}',{7},{8},'{9}','{10}','{11}','{12}','{13}',{14},{15},{16},{17},{18},{19})",
                                                ValuationFunctionID, dicValuationFunctionDatasetID.ContainsKey(ValuationFunctionDatasetID) ? dicValuationFunctionDatasetID[ValuationFunctionDatasetID] : ValuationFunctionDatasetID, dicFunctionalFormID.ContainsKey(FunctionalFormID) ? dicFunctionalFormID[FunctionalFormID] : FunctionalFormID,
                                                reader.ReadInt32(), reader.ReadInt32(), reader.ReadString(), reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadString(),
                                                reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString(), Convert.ToDouble(reader.ReadString()),
                                                Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()),
                                                Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()));
                    if (currentPhase == 2 && dicDoImport[ValuationFunctionDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicDoImportFunction.Add(origValuationFunctionID, true);
                    }
                    else
                    {
                        dicDoImportFunction.Add(origValuationFunctionID, false);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Valuation function custom entries...";
                lbProcess.Refresh();
                this.Refresh();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "ValuationFunctionCustomEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int ValuationFunctionCustomEntriescount = reader.ReadInt32();
                pBarImport.Maximum = ValuationFunctionCustomEntriescount;
                for (int i = 0; i < (ValuationFunctionCustomEntriescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    Boolean doBatch = false;
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < ValuationFunctionCustomEntriescount)
                        {
                            int ValuationFunctionID = reader.ReadInt32();
                            if(dicDoImportFunction[ValuationFunctionID])
                            {
                                commandText = commandText + string.Format("insert into ValuationFunctionCustomEntries(ValuationFunctionID,Vvalue) values({0},{1});", dicValuationFunctionID.ContainsKey(ValuationFunctionID) ? dicValuationFunctionID[ValuationFunctionID] : ValuationFunctionID, reader.ReadSingle());
                                doBatch = true;
                            } else
                            {
                                reader.ReadSingle();
                            }
                            pBarImport.PerformStep();
                        }
                        else
                            continue;
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doBatch)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadIncomeGrowth(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing incomegrowth adjdatasets...";
                lbProcess.Refresh();
                this.Refresh();
                Dictionary<int, bool> dicDoImport = new Dictionary<int, bool>();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int IncomeGrowthAdjDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicIncomeGrowthAdjDatasetID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    IncomeGrowthAdjDatasetID = reader.ReadInt32();
                    int origIncomeGrowthAdjDatasetID = IncomeGrowthAdjDatasetID;
                    int oldSetupid = reader.ReadInt32();
                    string IncomeGrowthAdjDatasetName = reader.ReadString();
                    string commandText = string.Format("select IncomeGrowthAdjDatasetID from IncomeGrowthAdjDatasets where setupid={0} and IncomeGrowthAdjDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncomeGrowthAdjDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existIncomeGrowthAdjDatasetID = Convert.ToInt16(obj);

                        dicIncomeGrowthAdjDatasetID.Add(IncomeGrowthAdjDatasetID, existIncomeGrowthAdjDatasetID);
                        IncomeGrowthAdjDatasetID = existIncomeGrowthAdjDatasetID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" exists and was retained";
                        }
                        dicDoImport.Add(origIncomeGrowthAdjDatasetID, false);
                    }
                    else
                    {
                        commandText = string.Format("select IncomeGrowthAdjDatasetID from IncomeGrowthAdjDatasets where IncomeGrowthAdjDatasetID={0}", IncomeGrowthAdjDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(IncomeGrowthAdjDatasetID) from IncomeGrowthAdjDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicIncomeGrowthAdjDatasetID.Add(IncomeGrowthAdjDatasetID, 1);
                                IncomeGrowthAdjDatasetID = 1;
                            }
                            else
                            {
                                int maxIncomeGrowthAdjDatasetID = Convert.ToInt16(obj);
                                dicIncomeGrowthAdjDatasetID.Add(IncomeGrowthAdjDatasetID, maxIncomeGrowthAdjDatasetID + 1);
                                IncomeGrowthAdjDatasetID = maxIncomeGrowthAdjDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" was imported";
                        }
                        dicDoImport.Add(origIncomeGrowthAdjDatasetID, true);
                    }
                    //The 'F' is for the locked column in incomegrowthandadjatests - this is being imported and is not predefined.
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into IncomeGrowthAdjDatasets(IncomeGrowthAdjDatasetID,SetupID,IncomeGrowthAdjDatasetName,LOCKED) values({0},{1},'{2}', 'F')", IncomeGrowthAdjDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncomeGrowthAdjDatasetName);
                    if (currentPhase == 2 && dicDoImport[origIncomeGrowthAdjDatasetID])
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing incomegrowth adjfactors...";
                lbProcess.Refresh();
                this.Refresh();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "IncomeGrowthAdjFactors")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncomeGrowthAdjFactorscount = reader.ReadInt32();
                pBarImport.Maximum = IncomeGrowthAdjFactorscount;

                for (int i = 0; i < (IncomeGrowthAdjFactorscount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    Boolean doBatch = false;
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < IncomeGrowthAdjFactorscount)
                        {
                            IncomeGrowthAdjDatasetID = reader.ReadInt32();
                            string Distribution = reader.ReadString();
                            Single P1 = reader.ReadSingle();
                            Single P2 = reader.ReadSingle();
                            if(dicDoImport[IncomeGrowthAdjDatasetID])
                            {
                                commandText = commandText + string.Format("insert into IncomeGrowthAdjFactors(IncomeGrowthAdjDatasetID,Distribution,P1,P2,Yyear,Mean,EndPointGroups) values({0},'{1}',{2},{3},{4},{5},'{6}');", dicIncomeGrowthAdjDatasetID.ContainsKey(IncomeGrowthAdjDatasetID) ? dicIncomeGrowthAdjDatasetID[IncomeGrowthAdjDatasetID] : IncomeGrowthAdjDatasetID, Distribution, P1 == -1 ? "NULL" : P1.ToString(), P2 == -1 ? "NULL" : P2.ToString(), reader.ReadInt32(), reader.ReadSingle(), reader.ReadString());
                                doBatch = true;
                            } else
                            {
                                reader.ReadInt32();
                                reader.ReadSingle();
                                reader.ReadString();
                            }
                            pBarImport.PerformStep();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doBatch)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }


        private void ReadValuation2(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Valuation function datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false;
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int ValuationFunctionDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicValuationFunctionDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicFunctionalFormID = new Dictionary<int, int>();
                Dictionary<int, int> dicValuationFunctionID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointGroupID = new Dictionary<int, int>();
                Dictionary<int, int> dicEndPointID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    ValuationFunctionDatasetID = reader.ReadInt32();
                    int oldSetupid = reader.ReadInt32();
                    string ValuationFunctionDatasetName = reader.ReadString();
                    string commandText = string.Format("select ValuationFunctionDatasetID from ValuationFunctionDatasets where setupid={0} and ValuationFunctionDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, ValuationFunctionDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existValuationFunctionDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from ValuationFunctionCustomEntries where ValuationFunctionID in (select ValuationFunctionID from ValuationFunctions where ValuationFunctionDatasetID in ({0}))", existValuationFunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from ValuationFunctions where ValuationFunctionDatasetID in ({0})", existValuationFunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from ValuationFunctionDatasets where ValuationFunctionDatasetID in ({0})", existValuationFunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicValuationFunctionDatasetID.Add(ValuationFunctionDatasetID, existValuationFunctionDatasetID);
                        ValuationFunctionDatasetID = existValuationFunctionDatasetID;

                        if (currentPhase == 1)
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" exists and was retained";
                        }
                        doImport = false;
                    }
                    else
                    {
                        commandText = string.Format("select ValuationFunctionDatasetID from ValuationFunctionDatasets where ValuationFunctionDatasetID={0}", ValuationFunctionDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(ValuationFunctionDatasetID) from ValuationFunctionDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicValuationFunctionDatasetID.Add(ValuationFunctionDatasetID, 1);
                                ValuationFunctionDatasetID = 1;
                            }
                            else
                            {
                                int maxValuationFunctionDatasetID = Convert.ToInt16(obj);
                                dicValuationFunctionDatasetID.Add(ValuationFunctionDatasetID, maxValuationFunctionDatasetID + 1);
                                ValuationFunctionDatasetID = maxValuationFunctionDatasetID + 1;
                            }
                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nValuation Function dataset \"" + ValuationFunctionDatasetName + "\" was imported";
                        }
                        doImport = true;
                    }
                    //The 'F' is for the locked column in ValuationFunctionDataSets - this is imported and is not predefined.
                    // 2015 02 12 added LOCKED to field list - also removed extra 'F' from values list
                    commandText = string.Format("insert into ValuationFunctionDatasets(ValuationFunctionDatasetID,SetupID,ValuationFunctionDatasetName,Readonly, LOCKED) values({0},{1},'{2}','{3}', 'F')", ValuationFunctionDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, ValuationFunctionDatasetName, reader.ReadChar());
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Valuation functional forms...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "ValuationFunctionalForms")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int ValuationFunctionalFormscount = reader.ReadInt32();
                pBarImport.Maximum = ValuationFunctionalFormscount;
                int maxFunctionalFormID = 1;
                for (int i = 0; i < ValuationFunctionalFormscount; i++)
                {
                    int FunctionalFormID = reader.ReadInt32();
                    string FunctionalFormText = reader.ReadString();
                    string commandExist = string.Format("select FunctionalFormID from ValuationFunctionalForms where FunctionalFormText='{0}'", FunctionalFormText);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                    if (obj == null)
                    {
                        string commandText = "select max(FunctionalFormID) from ValuationFunctionalForms";
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (!Convert.IsDBNull(obj))
                        {
                            maxFunctionalFormID = Convert.ToInt16(obj);
                            dicFunctionalFormID.Add(FunctionalFormID, ++maxFunctionalFormID);
                        }
                        else
                        { dicFunctionalFormID.Add(FunctionalFormID, 1); }
                        commandText = string.Format("insert into ValuationFunctionalForms(FunctionalFormID,FunctionalFormText) values({0},'{1}')", maxFunctionalFormID, FunctionalFormText);
                        if (currentPhase == 2 && doImport)
                        {
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    else
                    { dicFunctionalFormID.Add(FunctionalFormID, Convert.ToInt16(obj)); }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoint groups...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndpointGroups")
                {
                    int EndpointGroupscount = reader.ReadInt32();
                    pBarImport.Maximum = EndpointGroupscount;
                    for (int i = 0; i < EndpointGroupscount; i++)
                    {
                        int EndPointGroupID = reader.ReadInt32();
                        int maxEndPointGroupID = 1;
                        string EndPointGroupName = reader.ReadString();
                        string commandExist = string.Format("select EndPointGroupID from EndpointGroups where EndPointGroupName='{0}'", EndPointGroupName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointGroupID) from EndpointGroups";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                maxEndPointGroupID = Convert.ToInt16(obj);
                                dicEndPointGroupID.Add(EndPointGroupID, ++maxEndPointGroupID);
                            }
                            commandText = string.Format("insert into EndpointGroups(EndPointGroupID,EndPointGroupName) values({0},'{1}')", maxEndPointGroupID, EndPointGroupName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointGroupID.Add(EndPointGroupID, Convert.ToInt16(obj));
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Endpoints...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "EndPoints")
                {
                    int EndPointscount = reader.ReadInt32();
                    pBarImport.Maximum = EndPointscount;
                    for (int i = 0; i < EndPointscount; i++)
                    {
                        int EndPointID = reader.ReadInt32();
                        int EndPointGroupID = reader.ReadInt32();
                        int maxEndPointID = 1;
                        string EndPointName = reader.ReadString();
                        string commandExist = string.Format("select EndPointID from EndPoints where EndPointgroupID={0} and EndPointName='{1}'", dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandExist);
                        if (obj == null)
                        {
                            string commandText = "select max(EndPointID) from EndPoints";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (!Convert.IsDBNull(obj))
                            {
                                maxEndPointID = Convert.ToInt16(obj);
                                dicEndPointID.Add(EndPointID, ++maxEndPointID);
                            }
                            commandText = string.Format("insert into EndPoints(EndPointID,EndPointGroupID,EndPointName) values({0},{1},'{2}')", maxEndPointID, dicEndPointGroupID.ContainsKey(EndPointGroupID) ? dicEndPointGroupID[EndPointGroupID] : EndPointGroupID, EndPointName);
                            if (currentPhase == 2 && doImport)
                            {
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            }
                        }
                        else
                        {
                            dicEndPointID.Add(EndPointID, Convert.ToInt16(obj));
                        }
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Valuation functions...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "ValuationFunctions")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int ValuationFunctionscount = reader.ReadInt32();
                pBarImport.Maximum = ValuationFunctionscount;
                for (int i = 0; i < ValuationFunctionscount; i++)
                {
                    int ValuationFunctionID = reader.ReadInt32();
                    string commandText = "select max(ValuationFunctionID) from ValuationFunctions";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (Convert.IsDBNull(obj))
                    {
                        dicValuationFunctionID.Add(ValuationFunctionID, 1); ValuationFunctionID = 1;
                    }
                    else
                    {
                        int maxValuationFunctionID = Convert.ToInt16(obj);
                        dicValuationFunctionID.Add(ValuationFunctionID, ++maxValuationFunctionID);
                        ValuationFunctionID = maxValuationFunctionID;
                    }
                    ValuationFunctionDatasetID = reader.ReadInt32();
                    int FunctionalFormID = reader.ReadInt32();
                    commandText = string.Format("insert into ValuationFunctions(ValuationFunctionID,ValuationFunctionDatasetID,FunctionalFormID,EndPointGroupID,EndPointID,Qualifier," +
                                                "Reference,StartAge,EndAge,NameA,DistA,NameB,NameC,NameD,A,P1A,P2A,B,C,D) " +
                                                "values({0},{1},{2},{3},{4},'{5}','{6}',{7},{8},'{9}','{10}','{11}','{12}','{13}',{14},{15},{16},{17},{18},{19})",
                                                ValuationFunctionID, dicValuationFunctionDatasetID.ContainsKey(ValuationFunctionDatasetID) ? dicValuationFunctionDatasetID[ValuationFunctionDatasetID] : ValuationFunctionDatasetID, dicFunctionalFormID.ContainsKey(FunctionalFormID) ? dicFunctionalFormID[FunctionalFormID] : FunctionalFormID,
                                                reader.ReadInt32(), reader.ReadInt32(), reader.ReadString(), reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadString(),
                                                reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString(), Convert.ToDouble(reader.ReadString()),
                                                Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()),
                                                Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()));
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Valuation function custom entries...";
                lbProcess.Refresh();
                this.Refresh();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable != "ValuationFunctionCustomEntries")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int ValuationFunctionCustomEntriescount = reader.ReadInt32();
                pBarImport.Maximum = ValuationFunctionCustomEntriescount;
                for (int i = 0; i < (ValuationFunctionCustomEntriescount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < ValuationFunctionCustomEntriescount)
                        {
                            int ValuationFunctionID = reader.ReadInt32();
                            commandText = commandText + string.Format("insert into ValuationFunctionCustomEntries(ValuationFunctionID,Vvalue) values({0},{1});", dicValuationFunctionID.ContainsKey(ValuationFunctionID) ? dicValuationFunctionID[ValuationFunctionID] : ValuationFunctionID, reader.ReadSingle());
                            pBarImport.PerformStep();
                        }
                        else
                            continue;
                    }
                    commandText = commandText + "END";
                    if (currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }


        private void ReadIncomeGrowth2(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Income growth datasets...";
                lbProcess.Refresh();
                this.Refresh();
                Boolean doImport = false;
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int IncomeGrowthAdjDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicIncomeGrowthAdjDatasetID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    IncomeGrowthAdjDatasetID = reader.ReadInt32();
                    int oldSetupid = reader.ReadInt32();
                    string IncomeGrowthAdjDatasetName = reader.ReadString();
                    string commandText = string.Format("select IncomeGrowthAdjDatasetID from IncomeGrowthAdjDatasets where setupid={0} and IncomeGrowthAdjDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncomeGrowthAdjDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        if(currentPhase == 1)
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" exists and will be retained";
                        }
                        else
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" exists and was retained";
                        }
                        doImport = false;
                        int existIncomeGrowthAdjDatasetID = Convert.ToInt16(obj);
                        /*
                        commandText = string.Format("delete from IncomeGrowthAdjFactors where IncomeGrowthAdjDatasetID in ({0})", existIncomeGrowthAdjDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncomeGrowthAdjDatasets where IncomeGrowthAdjDatasetID in ({0})", existIncomeGrowthAdjDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        */
                        dicIncomeGrowthAdjDatasetID.Add(IncomeGrowthAdjDatasetID, existIncomeGrowthAdjDatasetID);
                        IncomeGrowthAdjDatasetID = existIncomeGrowthAdjDatasetID;
                    }
                    else
                    {

                        commandText = string.Format("select IncomeGrowthAdjDatasetID from IncomeGrowthAdjDatasets where IncomeGrowthAdjDatasetID={0}", IncomeGrowthAdjDatasetID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            commandText = "select max(IncomeGrowthAdjDatasetID) from IncomeGrowthAdjDatasets";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (Convert.IsDBNull(obj))
                            {
                                dicIncomeGrowthAdjDatasetID.Add(IncomeGrowthAdjDatasetID, 1);
                                IncomeGrowthAdjDatasetID = 1;
                            }
                            else
                            {
                                int maxIncomeGrowthAdjDatasetID = Convert.ToInt16(obj);
                                dicIncomeGrowthAdjDatasetID.Add(IncomeGrowthAdjDatasetID, maxIncomeGrowthAdjDatasetID + 1);
                                IncomeGrowthAdjDatasetID = maxIncomeGrowthAdjDatasetID + 1;
                            }

                        }
                        if (currentPhase == 1)
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" will be imported";
                        }
                        else
                        {
                            strImportLog += "\nIncome Growth dataset \"" + IncomeGrowthAdjDatasetName + "\" was imported";
                        }
                        doImport = true;
                    }
                    //The 'F' is for the locked column in incomegrowthandadjatests - this is being imported and is not predefined.
                    // 2015 02 12 added LOCKED to field list
                    if (currentPhase == 2 && doImport)
                    {
                        commandText = string.Format("insert into IncomeGrowthAdjDatasets(IncomeGrowthAdjDatasetID,SetupID,IncomeGrowthAdjDatasetName,LOCKED) values({0},{1},'{2}', 'F')", IncomeGrowthAdjDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncomeGrowthAdjDatasetName);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }

                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Income growth factors...";
                lbProcess.Refresh();
                this.Refresh();
                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                string nextTable = reader.ReadString();
                if (nextTable != "IncomeGrowthAdjFactors")
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                    return;
                }
                int IncomeGrowthAdjFactorscount = reader.ReadInt32();
                pBarImport.Maximum = IncomeGrowthAdjFactorscount;
                for (int i = 0; i < (IncomeGrowthAdjFactorscount / 200) + 1; i++)
                {
                    string commandText = "execute block as" + " BEGIN ";
                    for (int k = 0; k < 200; k++)
                    {
                        if (i * 200 + k < IncomeGrowthAdjFactorscount)
                        {
                            IncomeGrowthAdjDatasetID = reader.ReadInt32();
                            string Distribution = reader.ReadString();
                            Single P1 = reader.ReadSingle();
                            Single P2 = reader.ReadSingle();
                            commandText = commandText + string.Format("insert into IncomeGrowthAdjFactors(IncomeGrowthAdjDatasetID,Distribution,P1,P2,Yyear,Mean,EndPointGroups) values({0},'{1}',{2},{3},{4},{5},'{6}');", dicIncomeGrowthAdjDatasetID.ContainsKey(IncomeGrowthAdjDatasetID) ? dicIncomeGrowthAdjDatasetID[IncomeGrowthAdjDatasetID] : IncomeGrowthAdjDatasetID, Distribution, P1 == -1 ? "NULL" : P1.ToString(), P2 == -1 ? "NULL" : P2.ToString(), reader.ReadInt32(), reader.ReadSingle(), reader.ReadString());
                            pBarImport.PerformStep();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    if(currentPhase == 2 && doImport)
                    {
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }

                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }

        Dictionary<int, int> lstSetupID = new Dictionary<int, int>();
        private void ReadSetups(BinaryReader reader)
        {
            try
            {
                int tableCount = reader.ReadInt32();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, string> dicSetups = new Dictionary<int, string>();
                for (int i = 0; i < tableCount; i++)
                {
                    dicSetups.Add(reader.ReadInt32(), reader.ReadString());
                }
                foreach (KeyValuePair<int, string> entry in dicSetups)
                {
                    string commandText = string.Format("select setupID from Setups where setupName='{0}'", entry.Value);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (!Convert.IsDBNull(obj))
                    {
                        int existSetupID = Convert.ToInt16(obj);
                        lstSetupID.Add(entry.Key, existSetupID);
                    }
                    else
                    {
                        commandText = "select max(setupID) from Setups";
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        int newsetupid = 0;
                        if (!Convert.IsDBNull(obj))
                        {
                            int maxsetupID = Convert.ToInt16(obj);
                            lstSetupID.Add(entry.Key, maxsetupID + 1);
                            newsetupid = maxsetupID + 1;
                        }
                        else
                        {
                            lstSetupID.Add(entry.Key, 1);
                            newsetupid = 1;
                        }
                        //The 'F' is for the Locked column in Setups - this is imported and not predefined
                        // 2012 02 15 added LOCKED to field list
                        commandText = string.Format("insert into Setups (setupID,setupName,LOCKED) values({0},'{1}', 'F')", newsetupid, entry.Value);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadAll(int currentPhase, BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                ReadSetups(reader);
                importsetupID = -1;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController("FirebirdServerDefaultInstance");
                    service.Stop();
                    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);
                    service.Start();
                    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);

                    string tableName = reader.ReadString();
                    switch (tableName)
                    {
                        case "griddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing grid definitions...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadGriddefinition(currentPhase, reader);
                            break;
                        case "pollutants":
                            Application.DoEvents();
                            lbProcess.Text = "Importing pollutants...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPollutant(currentPhase, reader);
                            break;
                        case "MonitorDataSets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing monitor dataSets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadMonitor(currentPhase, reader);
                            break;
                        case "IncidenceGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incidence datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncidence(currentPhase, reader);
                            break;
                        case "PopulationGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing population datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPopulation(currentPhase, reader);
                            break;
                        case "CrFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing HIF datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadCRFunction(currentPhase, reader);
                            break;
                        case "SetupVariableDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing variable datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadVariable(currentPhase, reader);
                            break;
                        case "InflationDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing inflation datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadInflation(currentPhase, reader);
                            break;
                        case "ValuationFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing valuation function datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadValuation(currentPhase, reader);
                            break;
                        case "IncomeGrowthAdjDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incomegrowth datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncomeGrowth(currentPhase, reader);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                errorOccur = true;
            }
        }

        private void ReadOnesetup(int currentPhase, BinaryReader reader)
        {
            try
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {

                    string tableName = reader.ReadString();
                    switch (tableName)
                    {
                        case "griddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing grid definitions...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadGriddefinition(currentPhase, reader);
                            break;
                        case "pollutants":
                            Application.DoEvents();
                            lbProcess.Text = "Importing pollutants...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPollutant(currentPhase, reader);
                            break;
                        case "MonitorDataSets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing monitor dataSets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadMonitor(currentPhase, reader);
                            break;
                        case "IncidenceGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incidence datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncidence(currentPhase, reader);
                            break;
                        case "PopulationGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing population datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPopulation(currentPhase, reader);
                            break;
                        case "CrFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing HIF datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadCRFunction(currentPhase, reader);
                            break;
                        case "SetupVariableDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing variable datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadVariable(currentPhase, reader);
                            break;
                        case "InflationDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing inflation datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadInflation(currentPhase, reader);
                            break;
                        case "ValuationFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing valuation function datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadValuation(currentPhase, reader);
                            break;
                        case "IncomeGrowthAdjDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incomeGrowth datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncomeGrowth(currentPhase, reader);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                errorOccur = true;
            }
        }

    }
}
