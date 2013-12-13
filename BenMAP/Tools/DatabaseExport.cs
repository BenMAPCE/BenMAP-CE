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
                            //case "QalyDatasets": database.name = "QALY Distribution Datasets"; database.PID = id; iTable = database.ID = ++j; lstdatabase.Add(database); break;
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
                string setupid = "";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string setup = "";
                if (nodeName != "Available Setups")
                {
                    setup = treDatabase.SelectedNode.Parent.Text;
                    string commandTextSetup = string.Format("select setupid from setups where setupname='{0}'", setup);
                    setupid = "setupid=" + Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandTextSetup));
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "BenMAPCEDatabase(*.bdbx)|*.bdbx";
                pBarExport.Value = 0;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (Stream stream = new FileStream(sfd.FileName, FileMode.Create))//初始化FileStream对象
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))//创建BinaryWriter对象
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
                                //case "QALY Distribution Datasets": 
                                //    WriteQALY(writer, setupid); 
                                //    break;
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
                                                    //case enumDatabaseExport.Setups:
                                                    //    string _setupid_name = "setupid=" + Convert.ToString(_setupid);
                                                    //    WriteSetup(writer, _setupid_name);
                                                    //    break;
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
                                                //case "QALY Distribution Datasets": 
                                                //    _setupid_name = "setupid=" + Convert.ToString(_setupid) + " and " + "QalyDatasetName=" + "'" + _Name + "'"; 
                                                //    WriteQALY(writer, _setupid_name);
                                                //    break;
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


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        ///// <summary> 
        ///// 序列化 
        ///// </summary> 
        ///// <param name="data">要序列化的对象</param> 
        ///// <returns>返回存放序列化后的数据缓冲区</returns> 
        //public static byte[] Serialize(object data)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    MemoryStream rems = new MemoryStream();
        //    formatter.Serialize(rems, data);
        //    return rems.GetBuffer();
        //}

        ///// <summary> 
        ///// 反序列化 
        ///// </summary> 
        ///// <param name="data">数据缓冲区</param> 
        ///// <returns>对象</returns> 
        //public static object Deserialize(byte[] data)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    MemoryStream rems = new MemoryStream(data);
        //    data = null;
        //    return formatter.Deserialize(rems);
        //}

        //int nodeIndex;
        string nodeName="";
        private void treDatabase_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treDatabase.SelectedNode != null)
            {
                //nodeIndex = treDatabase.SelectedNode.Index;
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
                                //byte[] buffer = Serialize(fbDataReader[i]);
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
                WriteGriddefinition(writer,allSetupid);
                WritePollutant(writer, allSetupid);
                WriteMonitor(writer, allSetupid);
                WriteIncidence(writer, allSetupid);
                WritePopulation(writer, allSetupid);
                WriteCRFunction(writer, allSetupid);
                WriteVariable(writer, allSetupid);
                WriteInflation(writer, allSetupid);
                WriteValuation(writer, allSetupid);
                WriteIncomeGrowth(writer, allSetupid);
                //WriteQALY(writer, allSetupid);
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
                    //先读取.shx文件,得到文件的总字节长度
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        Int64 BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    //shp
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        long BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    //dbf
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        long BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
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
                lstType = new List<string>() { "int", "int", "int" };
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

                //导出相关的pollutant
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

                //pBarExport.Value++;
                //lbProcess.Refresh();
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
                #region export related grid definition
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
                    //先读取.shx文件,得到文件的总字节长度
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        Int64 BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    //shp
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        long BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    //dbf
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        long BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }
                    pBarExport.PerformStep();
                }
                #endregion

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
                    //先读取.shx文件,得到文件的总字节长度
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shx", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        Int64 BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    //shp
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".shp", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        long BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
                        byte[] array = new byte[BytesSum];
                        fs.Read(array, 0, array.Length);
                        writer.Write(array);
                        BinaryFile.Close();
                        fs.Close();
                    }

                    //dbf
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf"))
                    {
                        FileStream fs = new FileStream(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + shapefilename + ".dbf", FileMode.Open, FileAccess.Read);   //文件流形式  
                        BinaryReader BinaryFile = new BinaryReader(fs);  //二进制读取文件的对象
                        long BytesSum = fs.Length;  //得到文件的字节总长  
                        writer.Write(BytesSum);
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

                //导出相关的pollutant
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
                lbProcess.Text = "Exporting locationtype...";
                lbProcess.Refresh();
                this.Refresh();
                commandText = string.Format("select count(*) from LocationType where LocationTypeID in (select LocationTypeID from Crfunctions where CrfunctionDatasetID in (select CrfunctionDatasetID from CrFunctionDatasets where {0}))", setupid);
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

                //export related griddefinition
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

        #region QALY
        //private void WriteQALY(BinaryWriter writer, string setupid)
        //{
        //    try
        //    {
        //        pBarExport.Value = 0;
        //        lbProcess.Text = "Exporting Qaly Datasets...";
        //        lbProcess.Refresh();
        //        this.Refresh();

        //        string commandText = string.Format("select count(*) from QalyDatasets where {0}", setupid);
        //        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        //        Int32 dsQalyDatasetscount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
        //        if (dsQalyDatasetscount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
        //        pBarExport.Maximum = dsQalyDatasetscount;
        //        writer.Write("QalyDatasets");
        //        writer.Write(dsQalyDatasetscount);
        //        commandText = string.Format("select QalyDatasetID,SetupID,QalyDatasetName,EndPointGroup,EndPoint,Qualifier,Description from QalyDatasets where {0}", setupid);
        //        List<string> lstType = new List<string>() { "int", "int", "string", "string", "string", "string", "string" };
        //        writeOneTable(writer, commandText, lstType);

        //        pBarExport.Value = 0;
        //        lbProcess.Text = "Exporting Qaly Entries...";
        //        lbProcess.Refresh();
        //        this.Refresh();

        //        commandText = string.Format("select count(*) from QalyEntries where QalyDatasetID in (select QalyDatasetID from QalyDatasets where {0})", setupid);
        //        Int32 dsQalyEntriescount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
        //        if (dsQalyEntriescount == 0) { pBarExport.Value = pBarExport.Maximum; lbProcess.Refresh(); this.Refresh(); return; }
        //        pBarExport.Maximum = dsQalyEntriescount;
        //        writer.Write("QalyEntries");
        //        writer.Write(dsQalyEntriescount);
        //        commandText = string.Format("select QalyDatasetID,StartAge,EndAge,Qaly from QalyEntries where QalyDatasetID in (select QalyDatasetID from QalyDatasets where {0})", setupid);
        //        lstType = new List<string>() { "int", "int", "int", "single" };
        //        writeOneTable(writer, commandText, lstType);
        //        pBarExport.Value++;
        //        lbProcess.Refresh();
        //        this.Refresh();
        //    }
        //    catch (Exception ex)
        //    {
        //        errorOccur = true;
        //        throw new Exception(ex.ToString());
        //    }
        //}
        #endregion
    }
}
