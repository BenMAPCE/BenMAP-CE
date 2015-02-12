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

namespace BenMAP
{
    public partial class DatabaseImport : FormBase
    {
        public DatabaseImport()
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
            if (openfile.ShowDialog() != DialogResult.OK)
            { return; }
            txtFile.Text = openfile.FileName;
            lblTarget.Enabled = true;
            cboSetup.Enabled = true;
        }

        bool errorOccur = false;
        private void btnOK_Click(object sender, EventArgs e)
        {
            errorOccur = false;
            if (cboSetup.Text == "") return;
            CommonClass.Connection = CommonClass.getNewConnection();
            pBarImport.Value = 0;
            using (Stream stream = new FileStream(txtFile.Text, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    string tableName = reader.ReadString();
                    switch (tableName)
                    {
                        case "setups": ReadAll(reader); break;
                        case "OneSetup": ReadOnesetup(reader); break;
                        case "griddefinitions": ReadGriddefinition(reader); break;
                        case "pollutants": ReadPollutant(reader); break;
                        case "MonitorDataSets": ReadMonitor(reader); break;
                        case "IncidenceGriddefinitions": ReadIncidence(reader); break;
                        case "PopulationGriddefinitions": ReadPopulation(reader); break;
                        case "CrFunctionDatasets": ReadCRFunction(reader); break;
                        case "SetupVariableDatasets": ReadVariable(reader); break;
                        case "InflationDatasets": ReadInflation(reader); break;
                        case "ValuationFunctionDatasets": ReadValuation(reader); break;
                        case "IncomeGrowthAdjDatasets": ReadIncomeGrowth(reader); break;
                    }

                    reader.Close();
                    if (errorOccur)
                    {
                        MessageBox.Show("Error. The import process was interrupted.", "Error");
                        CommonClass.Connection = CommonClass.getNewConnection();
                    }
                    else
                    {
                        MessageBox.Show("The database file was imported successfully.", "File imported");
                    }
                    DatabaseImport_Load(sender, e);
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

        private void ReadGriddefinition(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing grid definitions...";
                lbProcess.Refresh();
                this.Refresh();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, int> dicPercentageID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
                    int oldSetupid = reader.ReadInt32();
                    string griddefinitionName = reader.ReadString();
                    string commandText = string.Format("select GriddefinitionID from griddefinitions where setupid={0} and GriddefinitionName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existgriddefinitionID = Convert.ToInt16(obj);
                        commandText = string.Format("delete from GriddefinitionPercentageEntries where PercentageID in (select PercentageID from GriddefinitionPercentages where (SourceGriddefinitionID in ({0})) or (TargetGriddefinitionID in ({0})))", existgriddefinitionID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from GriddefinitionPercentages where SourceGriddefinitionID in ({0}) or TargetGriddefinitionID in ({0})", existgriddefinitionID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from shapefilegriddefinitiondetails where GriddefinitionID in ({0})", existgriddefinitionID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from regulargriddefinitiondetails where GriddefinitionID in ({0})", existgriddefinitionID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicGriddefinitionID.Add(griddefinitionID, existgriddefinitionID);
                        reader.BaseStream.Position = reader.BaseStream.Position + 4 * sizeof(Int32);
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
                        //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                        commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", griddefinitionID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing shapefiles...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            break;
                        case "shapefile":
                            //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                            // 2015 02 12 added Locked to field list
                            commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename,LOCKED) values({0},'{1}', 'F')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, shapefilename);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            break;
                    }
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
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing grid definition percentages...";
                lbProcess.Refresh();
                this.Refresh();

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
                        commandText = string.Format("insert into GriddefinitionPercentages(PercentageID,SourceGriddefinitionID,TargetGriddefinitionID) values({0},{1},{2})", PercentageID, dicGriddefinitionID.ContainsKey(SourceGriddefinitionID) ? dicGriddefinitionID[SourceGriddefinitionID] : SourceGriddefinitionID, dicGriddefinitionID.ContainsKey(TargetGriddefinitionID) ? dicGriddefinitionID[TargetGriddefinitionID] : TargetGriddefinitionID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing grid definition percentage entries...";
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
                        for (int k = 0; k < 200; k++)
                        {
                            if (i * 200 + k < GriddefinitionPercentageEntriescount)
                            {
                                int PercentageID = reader.ReadInt32();
                                commandText = commandText + string.Format("insert into GriddefinitionPercentageEntries(PercentageID,SourceColumn,SourceRow,TargetColumn,TargetRow,Percentage,NormalizationState) values({0},{1},{2},{3},{4},{5},{6});", dicPercentageID.ContainsKey(PercentageID) ? dicPercentageID[PercentageID] : PercentageID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), Convert.ToDouble(reader.ReadString()), reader.ReadInt32());
                                pBarImport.PerformStep();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        commandText = commandText + "END";
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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

        private void ReadPollutant(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing pollutants...";
                lbProcess.Refresh();
                this.Refresh();

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
                    int oldSetupid = reader.ReadInt32();
                    string commandText = string.Format("select PollutantID from pollutants where setupid={0} and PollutantName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, pollutantName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        existPollutantID = Convert.ToInt16(obj);
                        commandText = string.Format("delete from PollutantSeasons where PollutantID in ({0})", existPollutantID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from Metrics where PollutantID in ({0})", existPollutantID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from pollutants where PollutantID in ({0})", existPollutantID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicpollutantid.Add(PollutantID, existPollutantID);
                        PollutantID = existPollutantID;
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
                    }
                    commandText = string.Format("insert into pollutants(PollutantName,PollutantID,SetupID,ObservationType) values('{0}',{1},{2},{3})", pollutantName, PollutantID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, reader.ReadInt32());
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    lbProcess.Text = "Importing pollutant seasons...";
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
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        pBarImport.PerformStep();
                    }
                    if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                    nextTable = reader.ReadString();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing metrics...";
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
                        commandText = string.Format("select MetricID from Metrics where MetricID={0}", MetricID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            dicMetricID.Add(MetricID, ++changeMetricID);
                            MetricID = changeMetricID;
                        }
                        PollutantID = reader.ReadInt32();
                        commandText = string.Format("insert into Metrics(MetricID,PollutantID,MetricName,HourlyMetricGeneration) values({0},{1},'{2}',{3})", MetricID, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID, reader.ReadString(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    lbProcess.Text = "Importing fixed window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int FixedWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = FixedWindowMetricscount;
                    for (int i = 0; i < FixedWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into fixedwindowMetrics(MetricID,Starthour,Endhour,Statistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    lbProcess.Text = "Importing moving window metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int MovingWindowMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = MovingWindowMetricscount;
                    for (int i = 0; i < MovingWindowMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into MovingWindowMetrics(MetricID,Windowsize,Windowstatistic,Dailystatistic) values({0},{1},{2},{3})", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    lbProcess.Text = "Importing custom metrics...";
                    lbProcess.Refresh();
                    this.Refresh();
                    int CustomMetricscount = reader.ReadInt32();
                    pBarImport.Maximum = CustomMetricscount;
                    for (int i = 0; i < CustomMetricscount; i++)
                    {
                        int MetricID = reader.ReadInt32();
                        string commandText = string.Format("insert into CustomMetrics(MetricID,MetricFunction) values({0},'{1}')", dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing seasonal metrics...";
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
                        commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where SeasonalMetricID={0}", SeasonalMetricID);
                        obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            dicSeasonalMetricID.Add(SeasonalMetricID, ++changeSeasonalMetricID);
                            SeasonalMetricID = changeSeasonalMetricID;
                        }
                        int MetricID = reader.ReadInt32();
                        commandText = string.Format("insert into SeasonalMetrics(SeasonalMetricID,MetricID,SeasonalMetricName) values({0},{1},'{2}')", SeasonalMetricID, dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing seasonal metric seasons...";
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
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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

        private void ReadMonitor(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing monitor datasets...";
                lbProcess.Refresh();
                this.Refresh();

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int MonitorDatasetID = 0;
                Dictionary<int, int> dicMonitorDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicMonitorID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    MonitorDatasetID = reader.ReadInt32();
                    int oldSetupid = reader.ReadInt32();
                    string MonitorDatasetName = reader.ReadString();
                    string commandText = string.Format("select MonitorDatasetID from MonitorDataSets where setupid={0} and MonitorDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, MonitorDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existMonitorDatasetID = Convert.ToInt16(obj);
                        commandText = string.Format("delete from MonitorEntries where MonitorID in (select MonitorID from Monitors where MonitorDatasetID in ({0}))", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from Monitors where MonitorDatasetID in ({0})", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from MonitorDataSets where MonitorDatasetID in ({0})", existMonitorDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicMonitorDatasetID.Add(MonitorDatasetID, existMonitorDatasetID);
                        MonitorDatasetID = existMonitorDatasetID;
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
                    }
                    //the 'F' is for the LOCKED column in MonitorDataSets.  This is being added and is not a predefined.
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into MonitorDataSets(MonitorDatasetID,SetupID,MonitorDatasetName, LOCKED) values({0},{1},'{2}', 'F')", MonitorDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, MonitorDatasetName);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    int oldSetupid = reader.ReadInt32();
                    string commandText = string.Format("select PollutantID from pollutants where setupid={0} and PollutantName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, pollutantName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        dicExistPollutant.Add(PollutantID, true);
                        existPollutantID = Convert.ToInt16(obj);
                        dicpollutantid.Add(PollutantID, existPollutantID);
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
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
                        commandText = string.Format("insert into pollutants(PollutantName,PollutantID,SetupID,ObservationType) values('{0}',{1},{2},{3})", pollutantName, PollutantID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing pollutant seasons...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                    if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                    nextTable = reader.ReadString();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing metrics...";
                lbProcess.Refresh();
                this.Refresh();

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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    lbProcess.Text = "Importing fixed window metrics...";
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
                    lbProcess.Text = "Importing moving window metrics...";
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
                    lbProcess.Text = "Importing custom metrics...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing seasonal metrics...";
                lbProcess.Refresh();
                this.Refresh();
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing seasonal metric seasons...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing monitors...";
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
                            commandText = commandText + string.Format("insert into Monitors(MonitorID,MonitorDatasetID,PollutantID,Latitude,Longitude,MonitorName,MonitorDescription) values({0},{1},{2},{3},{4},'{5}','{6}');", MonitorID, dicMonitorDatasetID.ContainsKey(MonitorDatasetID) ? dicMonitorDatasetID[MonitorDatasetID] : MonitorDatasetID, dicpollutantid.ContainsKey(pollutantID) ? dicpollutantid[pollutantID] : pollutantID, reader.ReadSingle(), reader.ReadSingle(), reader.ReadString(), reader.ReadString().Replace("'", "''''"));
                            pBarImport.PerformStep();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing monitor entries...";
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
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText, fbParameter);
                    pBarImport.PerformStep();
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadIncidence(BinaryReader reader)
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
                lbProcess.Text = "Importing related grid definitions...";
                lbProcess.Refresh();
                this.Refresh();

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
                        }
                        //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                        commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", dicGriddefinitionID[griddefinitionID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing related shapefiles...";
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
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                break;
                            case "shapefile":
                                //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                                // 2015 02 12 added LOCKED to field list
                                commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename, LOCKED) values({0},'{1}', 'F')", dicGriddefinitionID[GriddefinitionID], shapefilename);
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                break;
                        }
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
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing incidence datasets...";
                lbProcess.Refresh();
                this.Refresh();

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
                        commandText = string.Format("delete from Incidenceentries where IncidenceRateID in (select IncidenceRateID  from IncidenceRates where IncidenceDatasetID in ({0}))", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncidenceRates where IncidenceDatasetID ={0}", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncidenceDatasets where IncidenceDatasetID={0}", existIncidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicIncidenceDatasetID[IncidenceDatasetID] = existIncidenceDatasetID;
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
                    }
                    int relateGridDefinitionID = reader.ReadInt32();
                    //The 'F' is for the Locked column in INCIDENCEDATESTS - imported not predefined.
                    commandText = string.Format("insert into IncidenceDatasets(IncidenceDatasetID,SetupID,IncidenceDatasetName,GridDefinitionID, LOCKED) values({0},{1},'{2}',{3}, 'F')", dicIncidenceDatasetID[IncidenceDatasetID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncidenceDatasetName, dicGriddefinitionID[relateGridDefinitionID]);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing endpointgroups...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing endpoints...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing race...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing gender...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing ethnicity...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing incidence rates...";
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
                            if (Convert.ToInt32(obj) > 0)
                            {
                                dicIncidenceRateID[IncidenceRateID] = ++maxIncidenceRateID;
                            }
                            commandText = commandText + string.Format("insert into IncidenceRates(IncidenceRateID,IncidenceDatasetID,GriddefinitionID,EndPointGroupID,EndPointID,RaceID,GenderID,StartAge,EndAge,Prevalence,EthnicityID) values({0},{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10});", dicIncidenceRateID[IncidenceRateID], dicIncidenceDatasetID[IncidenceDatasetID], dicGriddefinitionID[GriddefinitionID], dicEndPointGroupID[EndPointGroupID], dicEndPointID[EndPointID], dicRaceID[RaceID], dicGenderID[GenderID], StartAge, EndAge, Prevalence, dicEthnicityID[EthnicityID]);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandText = commandText + "END";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing incidence entries...";
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
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadPopulation(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing related grid definitions...";
                lbProcess.Refresh();
                this.Refresh();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, bool> dicExistGriddefition = new Dictionary<int, bool>();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
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
                        }
                        //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                        commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", griddefinitionID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing related shapefiles...";
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
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                break;
                            case "shapefile":
                                //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                                commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename) values({0},'{1}', 'F')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, shapefilename);
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                break;
                        }
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
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing population configuration...";
                lbProcess.Refresh();
                this.Refresh();

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
                        //The 'F' is for the locked column in POPULATIONCONFIGURATIONS - this is imported and not predefined
                        commandText = string.Format("insert into PopulationConfigurations(PopulationConfigurationID,PopulationConfigurationName) values({0},'{1}')", PopulationConfigurationID, PopulationConfigurationName);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing population configuration ethnicity map...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigEthnicityMap")
                {
                    int PopConfigEthnicityMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigEthnicityMapcount;
                    for (int i = 0; i < PopConfigEthnicityMapcount; i++)
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing population configuration gender map...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigGenderMap")
                {
                    int PopConfigGenderMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigGenderMapcount;
                    for (int i = 0; i < PopConfigGenderMapcount; i++)
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing population configuration racemap...";
                lbProcess.Refresh();
                this.Refresh();

                if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                nextTable = reader.ReadString();
                if (nextTable == "PopConfigRaceMap")
                {
                    int PopConfigRaceMapcount = reader.ReadInt32();
                    pBarImport.Maximum = PopConfigRaceMapcount;
                    for (int i = 0; i < PopConfigRaceMapcount; i++)
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing ageranges...";
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
                lbProcess.Text = "Importing population datasets...";
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
                            commandText = string.Format("delete from PopulationEntries where PopulationDatasetID in ({0})", existPopulationDatasetID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            commandText = string.Format("delete from PopulationGrowthWeights where PopulationDatasetID in ({0})", existPopulationDatasetID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            commandText = string.Format("delete from PopulationDatasets where PopulationDatasetID={0}", existPopulationDatasetID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                        }
                        int PopulationConfigurationID = reader.ReadInt32();
                        int GriddefinitionID = reader.ReadInt32();
                        //The 'F' is for the Locked column in PopulationDataSets - this is imported not predefined.
                        // 2015 02 12 added LOCKED to field list
                        commandText = string.Format("insert into PopulationDatasets(PopulationDatasetID,SetupID,PopulationDatasetName,PopulationConfigurationID,GriddefinitionID,ApplyGrowth,LOCKED) values({0},{1},'{2}',{3},{4},{5}, 'F')", PopulationDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, PopulationDatasetName, dicPopulationConfigurationID.ContainsKey(PopulationConfigurationID) ? dicPopulationConfigurationID[PopulationConfigurationID] : PopulationConfigurationID, dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing population entries...";
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
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    foreach (int popdatasetid in dicyyear.Keys)
                    {
                        for (int i = 0; i < dicyyear[popdatasetid].Count; i++)
                        {
                            string commandText = string.Format("insert into T_POPULATIONDATASETIDYEAR(POPULATIONDATASETID,YYEAR) values({0},{1})", dicPopulationDatasetID.ContainsKey(popdatasetid) ? dicPopulationDatasetID[popdatasetid] : popdatasetid, dicyyear[popdatasetid][i]);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing population growthweights...";
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
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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

        private void ReadCRFunction(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing health impact function datasets...";
                lbProcess.Refresh();
                this.Refresh();

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
                        commandText = string.Format("delete from CrFunctionCustomEntries where CrfunctionID in (select CrfunctionID from Crfunctions where CrfunctionDatasetID in ({0}))", existCrfunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from Crfunctions where CrfunctionDatasetID in ({0})", existCrfunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from CrFunctionDatasets where CrfunctionDatasetID in ({0})", existCrfunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicCrfunctionDatasetID[CrfunctionDatasetID] = existCrfunctionDatasetID;
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
                    }
                    //The F is for the locked column in CRFunctionDataSet - this is being imported and not predefined.
                    // added locked column to values list
                    commandText = string.Format("insert into CrFunctionDatasets(CrfunctionDatasetID,SetupID,CrfunctionDatasetName,Readonly,Locked) values({0},{1},'{2}','{3}', 'F')", dicCrfunctionDatasetID[CrfunctionDatasetID], importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, CrfunctionDatasetName, reader.ReadChar());
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    int oldSetupid = reader.ReadInt32();
                    string commandText = string.Format("select PollutantID from pollutants where setupid={0} and PollutantName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, pollutantName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        dicExistPollutant.Add(PollutantID, true);
                        existPollutantID = Convert.ToInt16(obj);
                        dicpollutantid.Add(PollutantID, existPollutantID);
                        reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
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
                        commandText = string.Format("insert into pollutants(PollutantName,PollutantID,SetupID,ObservationType) values('{0}',{1},{2},{3})", pollutantName, PollutantID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing pollutant seasons...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                    if (reader.BaseStream.Position >= reader.BaseStream.Length) { pBarImport.Value = pBarImport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
                    nextTable = reader.ReadString();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing metrics...";
                lbProcess.Refresh();
                this.Refresh();

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
                        PollutantID = reader.ReadInt32();
                        if (dicExistPollutant[PollutantID])
                        {
                            dicExistMetric.Add(MetricID, true);
                            string MetricName = reader.ReadString();
                            reader.BaseStream.Position = reader.BaseStream.Position + sizeof(Int32);
                            commandText = string.Format("select metricid from metrics where metricname='{0}' and pollutantid={1}", MetricName, dicpollutantid.ContainsKey(PollutantID) ? dicpollutantid[PollutantID] : PollutantID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null)
                            {
                                dicMetricID.Add(MetricID, Convert.ToInt16(obj));
                            }
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    lbProcess.Text = "Importing fixed window metrics...";
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
                    lbProcess.Text = "Importing moving window metrics...";
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
                    lbProcess.Text = "Importing custom metrics...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing seasonal metrics...";
                lbProcess.Refresh();
                this.Refresh();
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
                            commandText = string.Format("insert into SeasonalMetrics(SeasonalMetricID,MetricID,SeasonalMetricName) values({0},{1},'{2}')", SeasonalMetricID, dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, reader.ReadString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing seasonal metric seasons...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        pBarImport.PerformStep();
                    }
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing functional forms...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing baseline functional forms...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing endpointgroups...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing endpoints...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing locationtype...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing health impact functions...";
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
                        commandText = string.Format("insert into Crfunctions(CrfunctionID,CrfunctionDatasetID,FunctionalFormID,MetricID,SeasonalMetricID,IncidenceDatasetID,PrevalenceDatasetID,VariableDatasetID,LocationTypeID,BaselineFunctionalFormID,EndPointgroupID,EndPointID,PollutantID,Metricstatistic,Author,Yyear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,EndAge,Beta,DistBeta,P1beta,P2beta,A,NameA,B,NameB,C,NameC,Ethnicity,Percentile) values({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},'{14}',{15},'{16}','{17}','{18}','{19}','{20}','{21}',{22},{23},{24},'{25}',{26},{27},{28},'{29}',{30},'{31}',{32},'{33}','{34}',{35})", maxCrfunctionID, dicCrfunctionDatasetID[CrfunctionDatasetID], dicFunctionalFormID[FunctionalFormID], dicMetricID.ContainsKey(MetricID) ? dicMetricID[MetricID] : MetricID, SeasonalMetricID == -1 ? "NULL" : (dicSeasonalMetricID.ContainsKey(SeasonalMetricID) ? dicSeasonalMetricID[SeasonalMetricID].ToString() : SeasonalMetricID.ToString()), IncidenceDatasetID == -1 ? "NULL" : IncidenceDatasetID.ToString(), PrevalenceDatasetID == -1 ? "NULL" : PrevalenceDatasetID.ToString(), VariableDatasetID == -1 ? "NULL" : VariableDatasetID.ToString(), LocationTypeID == -1 ? "NULL" : (dicLocationTypeID[LocationTypeID].ToString()), dicBaselineFunctionalFormID[BaselineFunctionalFormID], dicEndPointGroupID[EndPointgroupID], dicEndPointID[EndPointID], dicpollutantid.ContainsKey(Pollutantid) ? dicpollutantid[Pollutantid] : Pollutantid, reader.ReadInt32(), reader.ReadString().Replace("'", "''''"), reader.ReadInt32(), reader.ReadString().Replace("'", "''''"), reader.ReadString(), reader.ReadString().Replace("'", "''''"), reader.ReadString().Replace("'", "''''"), reader.ReadString(), reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), Convert.ToDouble(reader.ReadString()), reader.ReadString(), Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()), Convert.ToDouble(reader.ReadString()), reader.ReadString(), Convert.ToDouble(reader.ReadString()), reader.ReadString(), Convert.ToDouble(reader.ReadString()), reader.ReadString(), reader.ReadString(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        pBarImport.PerformStep();
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing health impact function customentries...";
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
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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

        private void ReadVariable(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing variable datasets...";
                lbProcess.Refresh();
                this.Refresh();

                int tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                int SetupVariableDatasetID = 0;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<int, int> dicSetupVariableDatasetID = new Dictionary<int, int>();
                Dictionary<int, int> dicSetupVariableID = new Dictionary<int, int>();
                for (int i = 0; i < tableCount; i++)
                {
                    SetupVariableDatasetID = reader.ReadInt32();
                    int oldSetupid = reader.ReadInt32();
                    string SetupVariableDatasetName = reader.ReadString();
                    string commandText = string.Format("select SetupVariableDatasetID from SetupVariableDatasets where setupid={0} and SetupVariableDatasetName='{1}'", importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, SetupVariableDatasetName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        int existSetupVariableDatasetID = Convert.ToInt16(obj);
                        commandText = string.Format("delete from SetupGlobalVariables where SetupVariableID in (select SetupVariableID from SetupVariables where SetupVariableDatasetID in ({0}))", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from SetupGeographicVariables where SetupVariableID in (select SetupVariableID from SetupVariables where SetupVariableDatasetID in ({0}))", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from SetupVariables where SetupVariableDatasetID in ({0})", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from SetupVariableDatasets where SetupVariableDatasetID in ({0})", existSetupVariableDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicSetupVariableDatasetID.Add(SetupVariableDatasetID, existSetupVariableDatasetID);
                        SetupVariableDatasetID = existSetupVariableDatasetID;
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
                    }
                    //The 'F' is for the Locked column in SetUpVariableDataSets - this is improted and not predefined
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into SetupVariableDatasets(SetupVariableDatasetID,SetupID,SetupVariableDatasetName,LOCKED) values({0},{1},'{2}', 'F')", SetupVariableDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, SetupVariableDatasetName);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing related grid definitions...";
                lbProcess.Refresh();
                this.Refresh();

                tableCount = reader.ReadInt32();
                pBarImport.Maximum = tableCount;
                Dictionary<int, int> dicGriddefinitionID = new Dictionary<int, int>();
                Dictionary<int, bool> dicExistGriddefinition = new Dictionary<int, bool>();
                for (int i = 0; i < tableCount; i++)
                {
                    int griddefinitionID = reader.ReadInt32();
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
                        }
                        //The 'F' (not locked) is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                        commandText = string.Format("insert into griddefinitions(GriddefinitionID,SetupID,GriddefinitionName,Columns,Rrows,Ttype,Defaulttype, LOCKED) values({0},{1},'{2}',{3},{4},{5},{6}, 'F')", griddefinitionID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, griddefinitionName, reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing related shapefiles...";
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
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                break;
                            case "shapefile":
                                //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                                commandText = string.Format("insert into shapefilegriddefinitiondetails(GriddefinitionID,shapefilename) values({0},'{1}', 'F')", dicGriddefinitionID.ContainsKey(GriddefinitionID) ? dicGriddefinitionID[GriddefinitionID] : GriddefinitionID, shapefilename);
                                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                break;
                        }
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
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing variables...";
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
                            commandText = commandText + string.Format("insert into SetupVariables(SetupVariableID,SetupVariableDatasetID,SetupVariableName,GriddefinitionID) values({0},{1},'{2}',{3});", maxSetupVariableID + k + 1, dicSetupVariableDatasetID.ContainsKey(SetupVariableDatasetID) ? dicSetupVariableDatasetID[SetupVariableDatasetID] : SetupVariableDatasetID, SetupVariableName, dicGriddefinitionID.ContainsKey(Griddefinitionid) ? dicGriddefinitionID[Griddefinitionid] : Griddefinitionid);
                            pBarImport.PerformStep();
                        }
                        else
                            continue;
                    }
                    commandText = commandText + "END";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing geographic variables...";
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
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                else
                {
                    reader.BaseStream.Position = reader.BaseStream.Position - nextTable.Length - 1;
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing global variables...";
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
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }

        private void ReadInflation(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing inflation datasets...";
                lbProcess.Refresh();
                this.Refresh();

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
                        commandText = string.Format("delete from InflationEntries where InflationDatasetID in ({0})", existInflationDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from InflationDatasets where InflationDatasetID in ({0})", existInflationDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicInflationDatasetID.Add(InflationDatasetID, existInflationDatasetID);
                        InflationDatasetID = existInflationDatasetID;
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
                    }
                    //The 'F' is for the locked column in inflationdatasets - this is imported not predefined
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into InflationDatasets(InflationDatasetID,SetupID,InflationDatasetName, LOCKED) values({0},{1},'{2}', 'F')", InflationDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, InflationDatasetName);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing inflation entries...";
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
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    pBarImport.PerformStep();
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }

        }

        private void ReadValuation(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing valuation function datasets...";
                lbProcess.Refresh();
                this.Refresh();

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
                        commandText = string.Format("delete from ValuationFunctionCustomEntries where ValuationFunctionID in (select ValuationFunctionID from ValuationFunctions where ValuationFunctionDatasetID in ({0}))", existValuationFunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from ValuationFunctions where ValuationFunctionDatasetID in ({0})", existValuationFunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from ValuationFunctionDatasets where ValuationFunctionDatasetID in ({0})", existValuationFunctionDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicValuationFunctionDatasetID.Add(ValuationFunctionDatasetID, existValuationFunctionDatasetID);
                        ValuationFunctionDatasetID = existValuationFunctionDatasetID;
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
                    }
                    //The 'F' is for the locked column in ValuationFunctionDataSets - this is imported and is not predefined.
                    // 2015 02 12 added LOCKED to field list - also removed extra 'F' from values list
                    commandText = string.Format("insert into ValuationFunctionDatasets(ValuationFunctionDatasetID,SetupID,ValuationFunctionDatasetName,Readonly, LOCKED) values({0},{1},'{2}','{3}', 'F')", ValuationFunctionDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, ValuationFunctionDatasetName, reader.ReadChar());
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing valuation functional forms...";
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
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    else
                    { dicFunctionalFormID.Add(FunctionalFormID, Convert.ToInt16(obj)); }
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing endpointgroups...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing endpoints...";
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
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                lbProcess.Text = "Importing valuation functions...";
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
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    pBarImport.PerformStep();
                }

                pBarImport.Value = 0;
                lbProcess.Text = "Importing valuation function customentries...";
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
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            catch (Exception ex)
            {
                errorOccur = true;
                throw new Exception(ex.ToString());
            }
        }

        private void ReadIncomeGrowth(BinaryReader reader)
        {
            try
            {
                pBarImport.Value = 0;
                lbProcess.Text = "Importing incomegrowth adjdatasets...";
                lbProcess.Refresh();
                this.Refresh();

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
                        int existIncomeGrowthAdjDatasetID = Convert.ToInt16(obj);
                        commandText = string.Format("delete from IncomeGrowthAdjFactors where IncomeGrowthAdjDatasetID in ({0})", existIncomeGrowthAdjDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from IncomeGrowthAdjDatasets where IncomeGrowthAdjDatasetID in ({0})", existIncomeGrowthAdjDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    }
                    //The 'F' is for the locked column in incomegrowthandadjatests - this is being imported and is not predefined.
                    // 2015 02 12 added LOCKED to field list
                    commandText = string.Format("insert into IncomeGrowthAdjDatasets(IncomeGrowthAdjDatasetID,SetupID,IncomeGrowthAdjDatasetName,LOCKED) values({0},{1},'{2}', 'F')", IncomeGrowthAdjDatasetID, importsetupID == -1 ? lstSetupID[oldSetupid] : importsetupID, IncomeGrowthAdjDatasetName);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
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
                        commandText = string.Format("insert into Setups(setupID,setupName,LOCKED) values({0},'{1}', 'F')", newsetupid, entry.Value);
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

        private void ReadAll(BinaryReader reader)
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
                            ReadGriddefinition(reader);
                            break;
                        case "pollutants":
                            Application.DoEvents();
                            lbProcess.Text = "Importing pollutants...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPollutant(reader);
                            break;
                        case "MonitorDataSets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing monitor dataSets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadMonitor(reader);
                            break;
                        case "IncidenceGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incidence datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncidence(reader);
                            break;
                        case "PopulationGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing population datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPopulation(reader);
                            break;
                        case "CrFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing HIF datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadCRFunction(reader);
                            break;
                        case "SetupVariableDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing variable datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadVariable(reader);
                            break;
                        case "InflationDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing inflation datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadInflation(reader);
                            break;
                        case "ValuationFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing valuation function datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadValuation(reader);
                            break;
                        case "IncomeGrowthAdjDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incomegrowth datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncomeGrowth(reader);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                errorOccur = true;
            }
        }

        private void ReadOnesetup(BinaryReader reader)
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
                            ReadGriddefinition(reader);
                            break;
                        case "pollutants":
                            Application.DoEvents();
                            lbProcess.Text = "Importing pollutants...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPollutant(reader);
                            break;
                        case "MonitorDataSets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing monitor dataSets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadMonitor(reader);
                            break;
                        case "IncidenceGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incidence datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncidence(reader);
                            break;
                        case "PopulationGriddefinitions":
                            Application.DoEvents();
                            lbProcess.Text = "Importing population datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadPopulation(reader);
                            break;
                        case "CrFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing HIF datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadCRFunction(reader);
                            break;
                        case "SetupVariableDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing variable datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadVariable(reader);
                            break;
                        case "InflationDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing inflation datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadInflation(reader);
                            break;
                        case "ValuationFunctionDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing valuation function datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadValuation(reader);
                            break;
                        case "IncomeGrowthAdjDatasets":
                            Application.DoEvents();
                            lbProcess.Text = "Importing incomeGrowth datasets...";
                            lbProcess.Refresh();
                            this.Refresh();
                            ReadIncomeGrowth(reader);
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
