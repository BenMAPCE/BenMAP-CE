using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using DotSpatial.Data;
using System.Xml.Serialization;
using ProtoBuf;
using System.Collections;
using System.Data.Common;
using System.Data.OleDb;
using DotSpatial.Topology;
using System.Runtime.InteropServices;
using Excel;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;


namespace BenMAP
{
    public enum ConfigurationSettingType
    {
        Configuration = 0,
        LatinHypercubePoints = 1,
        PopulationDataset = 2,
        IncidenceDataset = 3,
        HealthFunction = 4
    }

    public class CommonClass
    {
        public static void DeleteShapeFileName(string FileName)
        {
            if (!File.Exists(FileName)) return;
            string temppath = System.IO.Path.GetDirectoryName(FileName);
            string ExtFileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
            DeleteFile(temppath, ExtFileName);
        }

        public static void DeleteFile(string dirRoot, string deleteFileName)
        {
            try
            {
                string[] rootFiles = Directory.GetFiles(dirRoot); foreach (string s in rootFiles)
                {
                    string fname = System.IO.Path.GetFileNameWithoutExtension(s);
                    if (fname == deleteFileName)
                    {
                        File.Delete(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        static public void IniWriteValue(string Section, string Key, string Value, string filepath)
        {
            WritePrivateProfileString(Section, Key, Value, filepath);
        }
        static public string IniReadValue(string Section, string Key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, filepath);
            return temp.ToString();
        }

        private static string _dataFilePath = "";

        public static string DataFilePath
        {
            get
            {
                if (_dataFilePath == "")
                {
                    if (IsWindows7 || IsElse || IsWindowsVista)
                        _dataFilePath = Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + @"\BenMAP-CE";
                    else
                        _dataFilePath = Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + @"\Application Data\BenMAP-CE";
                }

                return CommonClass._dataFilePath;
            }
            set { CommonClass._dataFilePath = value; }
        }

        private static string _resultFilePath = "";

        public static string ResultFilePath
        {
            get
            {
                if (_resultFilePath == "")
                {
                    _resultFilePath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files";
                }
                return CommonClass._resultFilePath;
            }
            set { CommonClass._resultFilePath = value; }
        }

        private const string JIRA_CONNECTOR_FILE_NAME = "BenMAPJiraConnector.dll";
        public static string JiraConnectorFilePath
        {
            get
            {
                string jiraConnectorFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + JIRA_CONNECTOR_FILE_NAME;

                if (File.Exists(jiraConnectorFilePath))
                {
                    return jiraConnectorFilePath;
                }
                else
                {                
                    return "";
                }
            }
        }

        private const string JIRA_CONNECTOR_FILE_NAME_TXT = "BenMAPJiraConnector.txt";
        public static string JiraConnectorFilePathTXT
        {
            get
            {
                string jiraConnectorFilePathTXT = AppDomain.CurrentDomain.BaseDirectory + @"\" + JIRA_CONNECTOR_FILE_NAME_TXT;

                if (File.Exists(jiraConnectorFilePathTXT))
                {
                    return jiraConnectorFilePathTXT;
                }
                else
                {
                    return "";
                }
            }
        }

        private static BenMAP _benMAPForm;

        public static BenMAP BenMAPForm
        {
            get { return CommonClass._benMAPForm; }
            set { CommonClass._benMAPForm = value; }
        }
        private static string[] inputParams;

        public static string[] InputParams
        {
            get { return CommonClass.inputParams; }
            set { CommonClass.inputParams = value; }
        }


        private static string _activeSetup = "USA";

        public static string ActiveSetup
        {
            get { return _activeSetup; }
            set { _activeSetup = value; }
        }

        private static ConfigurationAtt _conAtt;

        public static ConfigurationAtt ConAtt
        {
            get { return _conAtt; }
            set { _conAtt = value; }
        }

        private static string _currentMainFormStat;

        public static string CurrentMainFormStat
        {
            get { return _currentMainFormStat; }
            set
            {
                _currentMainFormStat = value;
                OnFormChangedStat();
            }
        }

        public static BenMAPSetup ManageSetup;

        private static BenMAPSetup _mainSetup;
        public static BenMAPSetup MainSetup
        {
            get { return _mainSetup; }
            set
            {
                if (_mainSetup != null && _mainSetup.SetupID != value.SetupID)
                {

                    CommonClass.CurrentMainFormStat = "Current Setup: " + value.SetupName;
                }
                _mainSetup = value;
                CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);
                OnFormChangedSetup();
            }
        }

        private static FbConnection _connection;

        public static FbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
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

        public static FbConnection getNewConnection()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            string str = settings.ConnectionString;
            if (!str.Contains(":"))
                str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
            FbConnection connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);

            return connection;
        }





        public static List<BenMAPPollutant> LstPollutant; public static BenMAPGrid rBenMAPGrid;
        public static void SaveCSV(DataTable dt, string fileName)
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
                    if (dt.Rows[i][j].ToString().Contains(","))
                    {
                        data += "\"" + dt.Rows[i][j].ToString() + "\"";
                    }
                    else
                        data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            bool isBatch = false;

            if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
            {
                isBatch = true;
            }
            else
                MessageBox.Show("CSV file saved.", "File saved");
        }
        public static CRSelectFunction getCRSelectFunctionClone(CRSelectFunction cr)
        {
            try
            {
                CRSelectFunction crNew = new CRSelectFunction()
                {
                    BenMAPHealthImpactFunction = getBenMAPHealthImpactFunctionClone(cr.BenMAPHealthImpactFunction),
                    CRID = cr.CRID,
                    EndAge = cr.EndAge,
                    Ethnicity = cr.Ethnicity,
                    Gender = cr.Gender,
                    IncidenceDataSetID = cr.IncidenceDataSetID,
                    IncidenceDataSetName = cr.IncidenceDataSetName,
                    Locations = cr.Locations,
                    lstLatinPoints = cr.lstLatinPoints,
                    PrevalenceDataSetID = cr.PrevalenceDataSetID,
                    PrevalenceDataSetName = cr.PrevalenceDataSetName,
                    Race = cr.Race,
                    StartAge = cr.StartAge,
                    VariableDataSetID = cr.VariableDataSetID,
                    VariableDataSetName = cr.VariableDataSetName
                };
                return crNew;
            }
            catch
            {
            }
            return null;
        }

        public static BenMAPHealthImpactFunction getBenMAPHealthImpactFunctionClone(BenMAPHealthImpactFunction benMAPHealthImpactFunction)
        {
            try
            {
                BenMAPHealthImpactFunction bNew = new BenMAPHealthImpactFunction()
                                    {
                                        AContantDescription = benMAPHealthImpactFunction.AContantDescription,
                                        AContantValue = benMAPHealthImpactFunction.AContantValue,
                                        Author = benMAPHealthImpactFunction.Author,
                                        BaseLineIncidenceFunction = benMAPHealthImpactFunction.BaseLineIncidenceFunction,
                                        BContantDescription = benMAPHealthImpactFunction.BContantDescription,
                                        BContantValue = benMAPHealthImpactFunction.BContantValue,
                                        Beta = benMAPHealthImpactFunction.Beta,
                                        BetaDistribution = benMAPHealthImpactFunction.BetaDistribution,
                                        BetaParameter1 = benMAPHealthImpactFunction.BetaParameter1,
                                        BetaParameter2 = benMAPHealthImpactFunction.BetaParameter2,
                                        CContantDescription = benMAPHealthImpactFunction.CContantDescription,
                                        CContantValue = benMAPHealthImpactFunction.CContantValue,
                                        DataSetID = benMAPHealthImpactFunction.DataSetID,
                                        DataSetName = benMAPHealthImpactFunction.DataSetName,
                                        EndAge = benMAPHealthImpactFunction.EndAge,
                                        EndPoint = "Population Weighted Delta",
                                        EndPointGroup = benMAPHealthImpactFunction.EndPointGroup,
                                        EndPointGroupID = benMAPHealthImpactFunction.EndPointGroupID,
                                        EndPointID = benMAPHealthImpactFunction.EndPointID,
                                        Ethnicity = benMAPHealthImpactFunction.Ethnicity,
                                        Function = benMAPHealthImpactFunction.Function,
                                        Gender = benMAPHealthImpactFunction.Gender,
                                        ID = benMAPHealthImpactFunction.ID,
                                        IncidenceDataSetID = benMAPHealthImpactFunction.IncidenceDataSetID,
                                        Locations = benMAPHealthImpactFunction.Locations,
                                        Metric = benMAPHealthImpactFunction.Metric,
                                        MetricStatistic = benMAPHealthImpactFunction.MetricStatistic,
                                        OtherPollutants = benMAPHealthImpactFunction.OtherPollutants,
                                        Percentile = benMAPHealthImpactFunction.Percentile,
                                        Pollutant = benMAPHealthImpactFunction.Pollutant,
                                        PrevalenceDataSetID = benMAPHealthImpactFunction.PrevalenceDataSetID,
                                        Qualifier = benMAPHealthImpactFunction.Qualifier,
                                        Race = benMAPHealthImpactFunction.Race,
                                        Reference = benMAPHealthImpactFunction.Reference,
                                        strLocations = benMAPHealthImpactFunction.strLocations,
                                        SeasonalMetric = benMAPHealthImpactFunction.SeasonalMetric,
                                        StartAge = benMAPHealthImpactFunction.StartAge,
                                        VariableDataSetID = benMAPHealthImpactFunction.VariableDataSetID,
                                        Year = benMAPHealthImpactFunction.Year,
                                    };
                return bNew;
            }
            catch
            {
            }
            return null;
        }
        public static BenMAPGrid RBenMAPGrid
        {
            get
            {
                return rBenMAPGrid;
            }
            set
            {
                rBenMAPGrid = value;
                changeGridType(rBenMAPGrid);
            }
        }

        public static void changeGridType(BenMAPGrid rBenMAPGrid)
        {
            if (rBenMAPGrid == null) return;
            int i = 0;
            int iCol = 0;
            int iRow = 0;
            int iName = -1;
            int icName = -1;
            int iValue = 0;
            string _regionPath = "";
            if (rBenMAPGrid is ShapefileGrid)
            {
                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (rBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                {
                    _regionPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (rBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                }
            }
            else if (rBenMAPGrid is RegularGrid)
            {
                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (rBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                {
                    _regionPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (rBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                }
            }
            DotSpatial.Data.IFeatureSet fs = DotSpatial.Data.FeatureSet.Open(_regionPath);
            System.Data.DataTable dt = fs.DataTable;
            fs.Close();
            fs.Dispose();

            i = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToLower() == "name")
                {
                    dc.ColumnName = "Name";
                    iName = i;
                }
                if (dc.ColumnName.ToLower().Contains("name"))
                {
                    icName = i;
                }
                if (dc.ColumnName.ToLower().Trim() == "col")
                {
                    iCol = i;
                    dc.ColumnName = "Col";
                }
                if (dc.ColumnName.ToLower().Trim() == "row")
                {
                    iRow = i;
                    dc.ColumnName = "Row";
                }
                i++;
            }

            if (iName == -1)
            {
                if (icName != -1)
                {
                    iName = icName;
                    dt.Columns[iName].ColumnName = "Name";
                }
                else
                {
                    dt.Columns.Add("Name", typeof(string));
                    iName = i;

                    for (int ii = 0; ii < dt.Rows.Count; ii++)
                    {
                        dt.Rows[ii][iName] = dt.Rows[ii][iCol].ToString() + "/" + dt.Rows[ii][iRow].ToString();
                    }
                }
            }
            lstChartResult = new List<ChartResult>();
            ChartResult chartResult = null;
            foreach (DataRow dr in dt.Rows)
            {
                chartResult = new ChartResult();
                chartResult.Col = Convert.ToInt32(dr[iCol]);
                chartResult.Row = Convert.ToInt32(dr[iRow]);
                chartResult.RegionName = dr[iName].ToString();
                lstChartResult.Add(chartResult);
            }
            fs.Close();
            fs.Dispose();
        }

        public static BenMAPGrid GBenMAPGrid; public static List<BaseControlGroup> LstBaseControlGroup; public static double CRThreshold = 0; public static int CRLatinHypercubePoints = 10; public static bool CRRunInPointMode = false; public static int CRSeeds = 1; public static BenMAPPopulation BenMAPPopulation;
        public static List<GridRelationship> LstCurrentGridRelationship; public static string CurrentStat;
        public static List<string> LstAsynchronizationStates;
        private static List<GridRelationship> lstGridRelationshipAll;
        private static bool isAddPercentage = false;

        public static bool IsAddPercentage
        {
            get { return CommonClass.isAddPercentage; }
            set { CommonClass.isAddPercentage = value; }
        }
        public static List<GridRelationship> LstGridRelationshipAll
        {
            get
            {
                if (lstGridRelationshipAll == null || isAddPercentage == true)
                {
                    isAddPercentage = false;
                    List<GridRelationship> lstGridRelationship = new List<GridRelationship>(); lstGridRelationshipAll = new List<GridRelationship>(); string commandText = "select   PercentageID,SourceGridDefinitionID,TargetGridDefinitionID  from GridDefinitionPercentages";
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    foreach (DataRow dr in dsGrid.Tables[0].Rows)
                    {
                        if (lstGridRelationshipAll.Where(p => p.bigGridID == Convert.ToInt32(dr["TargetGridDefinitionID"])
                            && p.smallGridID == Convert.ToInt32(dr["SourceGridDefinitionID"])).Count() == 0)
                        {
                            lstGridRelationshipAll.Add(new GridRelationship()
                            {
                                bigGridID = Convert.ToInt32(dr["TargetGridDefinitionID"]),
                                smallGridID = Convert.ToInt32(dr["SourceGridDefinitionID"]),

                            });
                        }

                    }







                }
                return CommonClass.lstGridRelationshipAll;
            }
        }
        public static void SaveGridRelationship(string strFile, GRGridRelationShip gr)
        {
            try
            {
                if (File.Exists(strFile))
                    File.Delete(strFile);
                using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
                {
                    Serializer.Serialize<GRGridRelationShip>(fs, gr);

                    fs.Close();
                    fs.Dispose();
                }

            }
            catch
            { }
        }
        public static GRGridRelationShip LoadGridRelationship(string strFile)
        {
            GRGridRelationShip gr = null;
            using (FileStream fs = new FileStream(strFile, FileMode.Open))
            {
                try
                {
                    gr = Serializer.Deserialize<GRGridRelationShip>(fs);

                    fs.Close();
                    fs.Dispose();
                    return gr;
                }
                catch (Exception ex)
                {
                    fs.Close();
                    fs.Dispose();
                    return null;
                }
            }
        }
        public static GridRelationship getRelationshipFromBenMAPGrid(int big, int small)
        {
            try
            {
                GridRelationship grReturn = new GridRelationship();
                IFeatureSet fsBig = new FeatureSet();
                IFeatureSet fsSmall = new FeatureSet();
                BenMAPGrid bigBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(big);
                BenMAPGrid smallBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(small);
                grReturn.bigGridID = big;
                grReturn.smallGridID = small;
                grReturn.lstGridRelationshipAttribute = new List<GridRelationshipAttribute>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string finsSetupname = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", big);
                string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, finsSetupname));
                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + (bigBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                {
                    string shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + (bigBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                    fsBig = DotSpatial.Data.FeatureSet.Open(shapeFileName);
                    string shapeFileNameSmall = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + (smallBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                    fsSmall = DotSpatial.Data.FeatureSet.Open(shapeFileNameSmall);
                    List<RowCol> lstRowColSmall = new List<RowCol>();
                    List<DotSpatial.Topology.Point> lstMid = new List<DotSpatial.Topology.Point>();
                    for (int i = 0; i < fsSmall.DataTable.Rows.Count; i++)
                    {
                        IFeature f = fsSmall.GetFeature(i);
                        lstRowColSmall.Add(new RowCol() { Col = Convert.ToInt32(f.DataRow["Col"]), Row = Convert.ToInt32(f.DataRow["Row"]) });
                        lstMid.Add(new DotSpatial.Topology.Point(f.Centroid().Coordinates[0]));
                    }
                    for (int j = 0; j < fsBig.DataTable.Rows.Count; j++)
                    {
                        IFeature f = fsBig.GetFeature(j);
                        GridRelationshipAttribute gra = new GridRelationshipAttribute();
                        gra.bigGridRowCol = new RowCol() { Col = Convert.ToInt32(f.DataRow["Col"]), Row = Convert.ToInt32(f.DataRow["Row"]) };
                        gra.smallGridRowCol = new List<RowCol>();
                        Extent ext = f.Envelope.ToExtent();
                        for (int i = 0; i < lstMid.Count(); i++)
                        {
                            if (ext.Contains(lstMid[i].Coordinate))
                            {
                                if (f.Contains(lstMid[i]))
                                {
                                    gra.smallGridRowCol.Add(lstRowColSmall[i]);
                                }
                            }
                        }
                        grReturn.lstGridRelationshipAttribute.Add(gra);
                    }
                    fsBig.Close();
                    fsSmall.Close();
                }
                return grReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static IFeatureSet Intersection(IFeatureSet self, IFeatureSet other, FieldJoinType joinType)
        {
            IFeatureSet result = null;
            if (joinType == FieldJoinType.All)
            {
                result = self.CombinedFields(other);
                if (!self.AttributesPopulated) self.FillAttributes();
                if (!other.AttributesPopulated) other.FillAttributes();
                int i = 0;
                foreach (IFeature selfFeature in self.Features)
                {
                    List<int> potentialOthers = other.SelectIndices(selfFeature.Envelope.ToExtent());
                    foreach (int iotherFeature in potentialOthers)
                    {
                        IFeature otherFeature = other.Features[iotherFeature];
                        selfFeature.Intersection(otherFeature, result, joinType);
                    }
                    i++;
                }
            }
            if (joinType == FieldJoinType.LocalOnly)
            {
                if (!self.AttributesPopulated) self.FillAttributes();

                result = new FeatureSet();
                result.CopyTableSchema(self);
                result.FeatureType = self.FeatureType;
                IFeature union;
                if (other.Features != null && other.Features.Count > 0)
                {
                    union = other.Features[0];
                    IGeometry g = union.BasicGeometry as IGeometry;
                    for (int i = 1; i < other.Features.Count; i++)
                    {
                        g = g.Union(Geometry.FromBasicGeometry(other.Features[i].BasicGeometry));

                    }
                    union.BasicGeometry = g;

                    Extent otherEnvelope = new Extent(union.Envelope);
                    for (int shp = 0; shp < self.ShapeIndices.Count; shp++)
                    {
                        if (!self.ShapeIndices[shp].Extent.Intersects(otherEnvelope)) continue;
                        IFeature selfFeature = self.GetFeature(shp);
                        selfFeature.Intersection(union, result, joinType);

                    }
                }
            }
            return result;

        }
        public static List<GridRelationshipAttributePercentage> IntersectionPercentageNation(IFeatureSet self, IFeatureSet other, FieldJoinType joinType, int big, int small)
        {
            List<GridRelationshipAttributePercentage> result = new List<GridRelationshipAttributePercentage>();
            try
            {
                if (joinType == FieldJoinType.All)
                {
                    if (!self.AttributesPopulated) self.FillAttributes();
                    if (!other.AttributesPopulated) other.FillAttributes();
                    int i = 0;
                    Dictionary<string, Dictionary<string, double>> dicRelation = new Dictionary<string, Dictionary<string, double>>();
                    Polygon pSelfExtent = null;
                    Polygon pOtherExtent = null;
                    double dSumArea = 0.0;
                    foreach (IFeature selfFeature in self.Features)
                    {
                        IFeature intersactFeature = null; if (big == 20)
                        {
                            dSumArea += selfFeature.Area();
                        }
                        if (self.Filename == other.Filename)
                        {
                            intersactFeature = selfFeature;
                            if (dicRelation.ContainsKey(selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]))
                            {
                                if (big == 20)
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                        (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area() / selfFeature.Area());
                                }
                                else
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                    (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area());

                                }
                            }
                            else
                            {
                                dicRelation.Add(selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"],
                                    new Dictionary<string, double>());
                                if (big == 20)
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                       (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area() / selfFeature.Area());
                                }
                                else
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                   (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area());

                                }
                            }

                        }
                        else
                        {
                            List<int> potentialOthers = other.SelectIndices(selfFeature.Envelope.ToExtent());
                            foreach (int iotherFeature in potentialOthers)
                            {
                                intersactFeature = null;
                                if ((other.Features.Count < 5 || self.Features.Count < 5) && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum)) == 0 &&
                                    other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum.X, selfFeature.Envelope.Minimum.Y)) == 0 &&
                                    other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum)) == 0 &&
                                    other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum.X, selfFeature.Envelope.Maximum.Y)) == 0
                                    )
                                {
                                    intersactFeature = selfFeature;
                                }
                                else
                                    if ((other.Features.Count < 5 || self.Features.Count < 5) && selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum)) == 0 &&
                                       selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum.X, other.Features[iotherFeature].Envelope.Minimum.Y)) == 0 &&
                                        selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum)) == 0 &&
                                        selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum.X, other.Features[iotherFeature].Envelope.Maximum.Y)) == 0)
                                    {
                                        intersactFeature = other.Features[iotherFeature];
                                    }
                                    else
                                    {
                                        try
                                        {
                                            intersactFeature = selfFeature.Intersection(other.Features[iotherFeature]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                    }

                                try
                                {
                                    if (intersactFeature != null && intersactFeature.BasicGeometry != null)
                                    {

                                        if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                                        {
                                            if (big == 20)
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                    (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area() / other.Features[iotherFeature].Area());
                                            }
                                            else
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area());

                                            }
                                        }
                                        else
                                        {
                                            dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                                                new Dictionary<string, double>());
                                            if (big == 20)
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                   (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area() / other.Features[iotherFeature].Area());
                                            }
                                            else
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                               (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area());

                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                            }

                        }
                        i++;
                    }

                    if (small == 20)
                    {
                        dSumArea = dicRelation.Sum(p => p.Value.Sum(a => a.Value));
                    }

                    if (big == 20)
                    {
                        Dictionary<string, double> dicTemp = new Dictionary<string, double>();
                        foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelation)
                        {
                            if (k.Value.Count > 0)
                            {

                                dicTemp.Add(k.Key, 1.0);


                            }
                        }
                        foreach (KeyValuePair<string, double> kin in dicTemp)
                        {
                            string[] str = kin.Key.Split(new char[] { ',' });
                            GridRelationshipAttributePercentage gr = new GridRelationshipAttributePercentage()
                            {

                                sourceCol = Convert.ToInt32(str[0]),
                                sourceRow = Convert.ToInt32(str[1]),
                                targetCol = 1,
                                targetRow = 1,
                                percentage = 1,
                            };
                            result.Add(gr);
                        }
                    }
                    else
                    {
                        Dictionary<string, double> dicTemp = new Dictionary<string, double>();
                        foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelation)
                        {
                            if (k.Value.Count > 0)
                            {
                                foreach (KeyValuePair<string, double> kin in k.Value)
                                {
                                    if (!dicTemp.ContainsKey(kin.Key))
                                        dicTemp.Add(kin.Key, kin.Value);
                                    else
                                        dicTemp[kin.Key] += kin.Value;

                                }
                            }
                        }
                        foreach (KeyValuePair<string, double> kin in dicTemp)
                        {
                            string[] str = kin.Key.Split(new char[] { ',' });
                            GridRelationshipAttributePercentage gr = new GridRelationshipAttributePercentage()
                            {

                                sourceCol = 1,
                                sourceRow = 1,
                                targetCol = Convert.ToInt32(str[0]),
                                targetRow = Convert.ToInt32(str[1]),
                                percentage = kin.Value / dSumArea,
                            };
                            result.Add(gr);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static List<GridRelationshipAttributePercentage> IntersectionPercentage(IFeatureSet self, IFeatureSet other, FieldJoinType joinType)
        {
            List<GridRelationshipAttributePercentage> result = new List<GridRelationshipAttributePercentage>();
            try
            {
                if (joinType == FieldJoinType.All)
                {
                    if (!self.AttributesPopulated) self.FillAttributes();
                    if (!other.AttributesPopulated) other.FillAttributes();
                    int i = 0;
                    Dictionary<string, Dictionary<string, double>> dicRelation = new Dictionary<string, Dictionary<string, double>>();
                    Polygon pSelfExtent = null;
                    Polygon pOtherExtent = null;

                    foreach (IFeature selfFeature in self.Features)
                    {
                        List<int> potentialOthers = other.SelectIndices(selfFeature.Envelope.ToExtent());
                        foreach (int iotherFeature in potentialOthers)
                        {
                            if (iotherFeature == 33)
                            {
                            }
                            IFeature intersactFeature = null;


                            if ((other.Features.Count < 5 || self.Features.Count < 5) && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum)) == 0 &&
other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum.X, selfFeature.Envelope.Minimum.Y)) == 0 &&
other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum)) == 0 &&
other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum.X, selfFeature.Envelope.Maximum.Y)) == 0
)
                            {
                                intersactFeature = selfFeature;
                            }

                            else if ((other.Features.Count < 5 || self.Features.Count < 5) && selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum)) == 0 &&
                           selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum.X, other.Features[iotherFeature].Envelope.Minimum.Y)) == 0 &&
                            selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum)) == 0 &&
                            selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum.X, other.Features[iotherFeature].Envelope.Maximum.Y)) == 0)
                            {
                                intersactFeature = other.Features[iotherFeature];
                            }
                            else
                            {
                                try
                                {
                                    intersactFeature = selfFeature.Intersection(other.Features[iotherFeature]);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        if (selfFeature.IsWithinDistance(other.Features[iotherFeature], 0.00001))
                                        {
                                            if (selfFeature.Area() > other.Features[iotherFeature].Area())
                                                intersactFeature = other.Features[iotherFeature];
                                            else
                                                intersactFeature = selfFeature;
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                            if (intersactFeature != null && intersactFeature.BasicGeometry != null)
                            {

                                try
                                {
                                    double dArea = 0;
                                    try
                                    {
                                        dArea = intersactFeature.Area();
                                    }
                                    catch
                                    {
                                        dArea = Math.Abs(DotSpatial.Topology.Algorithm.CgAlgorithms.SignedArea(intersactFeature.BasicGeometry.Coordinates));
                                    }
                                    if (dArea > 0)
                                    {

                                        if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                                        {
                                            dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], dArea / other.Features[iotherFeature].Area());
                                        }
                                        else
                                        {
                                            dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                                                new Dictionary<string, double>());
                                            dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                               (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], dArea / other.Features[iotherFeature].Area());
                                        }
                                    }
                                }
                                catch
                                {
                                    try
                                    {
                                        if (selfFeature.IsWithinDistance(other.Features[iotherFeature], 0.00001))
                                        {
                                            if (selfFeature.Area() > other.Features[iotherFeature].Area())
                                                intersactFeature = other.Features[iotherFeature];
                                            else
                                                intersactFeature = selfFeature;
                                        }
                                        if (intersactFeature.Area() > 0)
                                        {

                                            if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                    (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area() / other.Features[iotherFeature].Area());
                                            }
                                            else
                                            {
                                                dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                                                    new Dictionary<string, double>());
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                   (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Area() / other.Features[iotherFeature].Area());
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                        i++;
                    }
                    foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelation)
                    {
                        if (k.Value.Count > 0)
                        {
                            string[] str = k.Key.Split(new char[] { ',' });
                            if (k.Value.Count == 1)
                            {

                                string[] strin = k.Value.ToArray()[0].Key.Split(new char[] { ',' });

                                GridRelationshipAttributePercentage gr = new GridRelationshipAttributePercentage()
                                {

                                    sourceCol = Convert.ToInt32(str[0]),
                                    sourceRow = Convert.ToInt32(str[1]),
                                    targetCol = Convert.ToInt32(strin[0]),
                                    targetRow = Convert.ToInt32(strin[1]),
                                    percentage = 1,
                                };
                                result.Add(gr);

                            }
                            else
                            {
                                foreach (KeyValuePair<string, double> kin in k.Value)
                                {
                                    string[] strin = kin.Key.Split(new char[] { ',' });
                                    double d = k.Value.Sum(p => p.Value);
                                    GridRelationshipAttributePercentage gr = new GridRelationshipAttributePercentage()
                                    {

                                        sourceCol = Convert.ToInt32(str[0]),
                                        sourceRow = Convert.ToInt32(str[1]),
                                        targetCol = Convert.ToInt32(strin[0]),
                                        targetRow = Convert.ToInt32(strin[1]),
                                        percentage = kin.Value / d,
                                    };
                                    result.Add(gr);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static void getRelationshipFromBenMAPGridPercentage(int big, int small)
        {
            try
            {

                IFeatureSet fsBig = new FeatureSet();
                IFeatureSet fsSmall = new FeatureSet();
                BenMAPGrid bigBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(big);
                BenMAPGrid smallBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(small);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string finsSetupname = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", big);
                string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, finsSetupname));
                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + (bigBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                {
                    string shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + (bigBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                    fsBig = DotSpatial.Data.FeatureSet.Open(shapeFileName);
                    string shapeFileNameSmall = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + (smallBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                    fsSmall = DotSpatial.Data.FeatureSet.Open(shapeFileNameSmall);


                    List<GridRelationshipAttributePercentage> lstGR = IntersectionPercentage(fsBig, fsSmall, FieldJoinType.All);
                    string commandText = "select max(PercentageID) from GridDefinitionPercentages";
                    int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

                    commandText = string.Format("insert into GridDefinitionPercentages values({0},{1},{2})", iMax, small, big);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    foreach (GridRelationshipAttributePercentage grp in lstGR)
                    {
                        commandText = string.Format("insert into GridDefinitionPercentageEntries values({0},{1},{2},{3},{4},{5},{6})",
                            iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow, grp.percentage, 0);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static List<BenMAPPollutant> lstPollutantAll;
        public static BaseControlCRSelectFunction BaseControlCRSelectFunction; public static BaseControlCRSelectFunctionCalculateValue BaseControlCRSelectFunctionCalculateValue;
        private static Dictionary<string, int> dicAllRace;
        public static Dictionary<string, int> DicAllRace
        {
            get
            {
                if (dicAllRace == null)
                    dicAllRace = Configuration.ConfigurationCommonClass.getAllRace();
                return CommonClass.dicAllRace;
            }
        }

        private static Dictionary<string, int> dicAllGender;
        public static Dictionary<string, int> DicAllGender
        {
            get
            {
                if (dicAllGender == null)
                    dicAllGender = Configuration.ConfigurationCommonClass.getAllGender();
                return CommonClass.dicAllGender;
            }
        }

        private static Dictionary<string, int> dicAllEthnicity;
        public static Dictionary<string, int> DicAllEthnicity
        {
            get
            {
                if (dicAllEthnicity == null)
                    dicAllEthnicity = Configuration.ConfigurationCommonClass.getAllEthnicity();
                return CommonClass.dicAllEthnicity;
            }
        }

        public static List<CRSelectFunctionCalculateValue> lstCRResultAggregation;
        public static List<CRSelectFunctionCalculateValue> lstCRResultAggregationQALY;
        public static IncidencePoolingAndAggregationAdvance IncidencePoolingAndAggregationAdvance;
        public static List<IncidencePoolingAndAggregation> lstIncidencePoolingAndAggregation;
        public static CRSelectFunctionCalculateValue IncidencePoolingResult;
        public static ValuationMethodPoolingAndAggregation ValuationMethodPoolingAndAggregation;
        public static List<ChartResult> lstChartResult;

        private static List<CRSelectFunction> _lstAddCRFunction = new List<CRSelectFunction>();
        public static List<CRSelectFunction> LstUpdateCRFunction
        {
            get { return _lstAddCRFunction; }
            set
            { _lstAddCRFunction = value; }
        }

        private static List<CRSelectFunction> _lstDelCRFunction = new List<CRSelectFunction>();
        public static List<CRSelectFunction> LstDelCRFunction
        {
            get { return _lstDelCRFunction; }
            set
            { _lstDelCRFunction = value; }
        }
        public static bool IsWindows98
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32Windows) && (Environment.OSVersion.Version.Minor == 10) && (Environment.OSVersion.Version.Revision.ToString() != "2222A");
            }
        }

        public static bool IsWindows98Second
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32Windows) && (Environment.OSVersion.Version.Minor == 10) && (Environment.OSVersion.Version.Revision.ToString() == "2222A");
            }
        }

        public static bool IsWindows2000
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 0);
            }
        }

        public static bool IsWindowsXP
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 1);
            }
        }

        public static bool IsWindows2003
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 2);
            }
        }

        public static bool IsWindowsVista
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 0);
            }
        }

        public static bool IsWindows7
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1);
            }
        }

        public static bool IsUnix
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Unix;
            }
        }

        public static bool IsElse
        {
            get
            {
                if (Environment.OSVersion.Platform != PlatformID.MacOSX
                    && Environment.OSVersion.Platform != PlatformID.Unix
                    && Environment.OSVersion.Platform != PlatformID.Win32NT
                    && Environment.OSVersion.Platform != PlatformID.Win32S
                    && Environment.OSVersion.Platform != PlatformID.Win32Windows
                    && Environment.OSVersion.Platform != PlatformID.WinCE
                    && Environment.OSVersion.Platform != PlatformID.Xbox)
                    return true;
                else
                    return false;
            }
        }




        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);



        public static System.Data.DataTable ExcelToDataTable(string filenameurl)
        {
            try
            {
                if (filenameurl.Substring(filenameurl.Length - 3, 3).ToLower() == "csv")
                {
                    return DataSourceCommonClass.getDataTableFromCSV(filenameurl);
                }


                FileStream stream = File.Open(filenameurl, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelReader = filenameurl.ToLower().EndsWith("xls")
                                           ? ExcelReaderFactory.CreateBinaryReader(stream)
                                           : ExcelReaderFactory.CreateOpenXmlReader(stream);
                excelReader.IsFirstRowAsColumnNames = true;
                bool isBatch = false;
                System.Data.DataSet ds = excelReader.AsDataSet();
                if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    isBatch = true;
                }
                if (isBatch || ds.Tables.Count == 1) return ds.Tables[0];
                int Index = 0;
                Dictionary<string, int> dicSheetNum = new Dictionary<string, int>();
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    dicSheetNum.Add(ds.Tables[i].TableName, i);
                }
                Sheets frm = new Sheets(dicSheetNum);
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    Index = frm.sheetIndex;
                }
                return ds.Tables[Index];
            }
            catch
            {
                return null;
            }
        }
        public static void SaveBenMAPProject(string strFile)
        {
            try
            {
                if (strFile == "") return;
                BenMAPProject benMAPProject = new BenMAPProject();
                if (ValuationMethodPoolingAndAggregation != null)
                {
                    benMAPProject.ValuationMethodPoolingAndAggregation = APVX.APVCommonClass.getNoResultValuationMethodPoolingAndAggregation(ValuationMethodPoolingAndAggregation); benMAPProject.ValuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                }
                else if (BaseControlCRSelectFunctionCalculateValue != null)
                {
                    benMAPProject.IncidencePoolingAndAggregationAdvance = IncidencePoolingAndAggregationAdvance;
                    benMAPProject.lstIncidencePoolingAndAggregation = lstIncidencePoolingAndAggregation;
                    benMAPProject.IncidencePoolingResult = IncidencePoolingResult;
                    benMAPProject.BaseControlCRSelectFunction = BaseControlCRSelectFunction;
                    benMAPProject.BaseControlCRSelectFunction.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 4);

                }
                else if (BaseControlCRSelectFunction != null)
                {
                    benMAPProject.BaseControlCRSelectFunction = BaseControlCRSelectFunction;
                }
                else
                {
                    benMAPProject.ManageSetup = ManageSetup;
                    benMAPProject.MainSetup = MainSetup; benMAPProject.LstPollutant = LstPollutant; benMAPProject.RBenMAPGrid = RBenMAPGrid;
                    benMAPProject.GBenMAPGrid = GBenMAPGrid; benMAPProject.LstBaseControlGroup = LstBaseControlGroup; benMAPProject.CRThreshold = CRThreshold; benMAPProject.CRLatinHypercubePoints = CRLatinHypercubePoints; benMAPProject.CRRunInPointMode = CRRunInPointMode; benMAPProject.CRSeeds = CRSeeds;
                    benMAPProject.BenMAPPopulation = BenMAPPopulation;
                    benMAPProject.lstPollutantAll = lstPollutantAll;
                }
                benMAPProject.IncidencePoolingAndAggregationAdvance = IncidencePoolingAndAggregationAdvance;
                benMAPProject.BenMAPPopulation = BenMAPPopulation; if (File.Exists(strFile))
                {
                    File.Delete(strFile);
                }
                using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
                {
                    try
                    {

                        Serializer.Serialize<BenMAPProject>(fs, benMAPProject);
                        fs.Close();
                        fs.Dispose();
                    }
                    catch
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            catch
            {
            }
        }

        public static BenMAPSetup getBenMAPSetupFromID(int setupID)
        {
            string commandText = "select SetupID,SetupName from Setups where  SetupID=" + setupID;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            DataRow dr = ds.Tables[0].Rows[0];
            BenMAPSetup benMAPSetup = new BenMAPSetup()
            {
                SetupID = Convert.ToInt32(dr["SetupID"]),
                SetupName = dr["SetupName"].ToString()
            };
            return benMAPSetup;
        }

        public static BenMAPSetup getBenMAPSetupFromName(string SetupName)
        {
            try
            {
                string commandText = "select SetupID,SetupName from Setups where  SetupName='" + SetupName + "'";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                DataRow dr = ds.Tables[0].Rows[0];
                BenMAPSetup benMAPSetup = new BenMAPSetup()
                {
                    SetupID = Convert.ToInt32(dr["SetupID"]),
                    SetupName = dr["SetupName"].ToString()
                };
                return benMAPSetup;
            }
            catch
            {
                return null;
            }
        }
        public static void ClearAllObject()
        {
            try
            {
                CommonClass.LstPollutant = null; CommonClass.RBenMAPGrid = null;
                CommonClass.GBenMAPGrid = null; CommonClass.LstBaseControlGroup = null; CommonClass.CRThreshold = 0; CommonClass.CRLatinHypercubePoints = 10; CommonClass.CRRunInPointMode = false;
                CommonClass.BenMAPPopulation = null;
                if (CommonClass.BaseControlCRSelectFunction != null)
                {
                    if (CommonClass.BaseControlCRSelectFunction.BaseControlGroup != null)
                    {
                        foreach (BaseControlGroup bc in CommonClass.BaseControlCRSelectFunction.BaseControlGroup)
                        {
                            if (bc.Base != null)
                                bc.Base.ModelResultAttributes = null;
                            if (bc.Control != null)
                                bc.Control.ModelResultAttributes = null;
                            if (bc.DeltaQ != null)
                                bc.DeltaQ.ModelResultAttributes = null;
                        }
                    }
                } CommonClass.BaseControlCRSelectFunction = null;
                if (CommonClass.lstIncidencePoolingAndAggregation != null)
                {
                    foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                    {
                        if (ip.lstAllSelectCRFuntion != null)
                        {
                            foreach (AllSelectCRFunction ac in ip.lstAllSelectCRFuntion)
                            {
                                if (ac.CRSelectFunctionCalculateValue != null)
                                    ac.CRSelectFunctionCalculateValue.CRCalculateValues = null;
                            }
                        }
                    }

                }
                CommonClass.lstIncidencePoolingAndAggregation = null;

                CommonClass.IncidencePoolingResult = null;
                if (CommonClass.ValuationMethodPoolingAndAggregation != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                {
                    foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    {
                        if (vb.lstAllSelectQALYMethodAndValue != null)
                        {
                            foreach (AllSelectQALYMethodAndValue aq in vb.lstAllSelectQALYMethodAndValue)
                            {
                                aq.lstQALYValueAttributes = null;
                            }
                        }
                        vb.lstAllSelectQALYMethodAndValue = null;
                        if (vb.lstAllSelectQALYMethodAndValueAggregation != null)
                        {
                            foreach (AllSelectQALYMethodAndValue aq in vb.lstAllSelectQALYMethodAndValueAggregation)
                            {
                                aq.lstQALYValueAttributes = null;
                            }
                        }
                        vb.lstAllSelectQALYMethodAndValueAggregation = null;
                        if (vb.LstAllSelectValuationMethodAndValue != null)
                        {
                            foreach (AllSelectValuationMethodAndValue aq in vb.LstAllSelectValuationMethodAndValue)
                            {
                                aq.lstAPVValueAttributes = null;
                            }
                        }
                        vb.LstAllSelectValuationMethodAndValue = null;
                        if (vb.LstAllSelectValuationMethodAndValueAggregation != null)
                        {
                            foreach (AllSelectValuationMethodAndValue aq in vb.LstAllSelectValuationMethodAndValueAggregation)
                            {
                                aq.lstAPVValueAttributes = null;
                            }
                        }
                        vb.LstAllSelectValuationMethodAndValueAggregation = null;
                    }

                }
                CommonClass.BaseControlCRSelectFunction = null;
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                {
                    foreach (CRSelectFunctionCalculateValue crv in CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                    {
                        crv.CRCalculateValues = null;
                    }
                    CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
                    GC.Collect();
                }
                CommonClass.ValuationMethodPoolingAndAggregation = null;
                if (CommonClass.lstCRResultAggregation != null)
                {
                    foreach (CRSelectFunctionCalculateValue crv in CommonClass.lstCRResultAggregation)
                    {
                        crv.CRCalculateValues = null;
                    }
                    CommonClass.lstCRResultAggregation = null;
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
            }
        }
        public static bool LoadBenMAPProject(string strFile)
        {
            try
            {
                BenMAPProject benMAPProject = null;
                using (FileStream fs = new FileStream(strFile, FileMode.Open))
                {
                    benMAPProject = Serializer.Deserialize<BenMAPProject>(fs); fs.Close();
                    fs.Dispose();
                }
                if (benMAPProject.ValuationMethodPoolingAndAggregation != null)
                {
                    ValuationMethodPoolingAndAggregation = benMAPProject.ValuationMethodPoolingAndAggregation;
                    BaseControlCRSelectFunctionCalculateValue = ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
                    IncidencePoolingAndAggregationAdvance = benMAPProject.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
                    lstIncidencePoolingAndAggregation = new List<IncidencePoolingAndAggregation>();
                    if (benMAPProject.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                    {
                        lstIncidencePoolingAndAggregation = benMAPProject.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(p => p.IncidencePoolingAndAggregation).ToList();
                    }
                    BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                    BaseControlCRSelectFunction.BaseControlGroup = BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
                    BaseControlCRSelectFunction.BenMAPPopulation = BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                    BaseControlCRSelectFunction.CRLatinHypercubePoints = BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                    BaseControlCRSelectFunction.CRRunInPointMode = BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                    CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                    BaseControlCRSelectFunction.CRThreshold = BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                    if (BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                    {
                        BaseControlCRSelectFunction.lstCRSelectFunction = BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Select(p => p.CRSelectFunction).ToList();
                    }

                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList(); RBenMAPGrid = BaseControlCRSelectFunction.RBenMapGrid;
                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType; LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup; CRThreshold = BaseControlCRSelectFunction.CRThreshold; CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints; CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode; CRSeeds = BaseControlCRSelectFunction.CRSeeds;
                    BenMAPPopulation = BaseControlCRSelectFunction.BenMAPPopulation;
                }
                else if (benMAPProject.BaseControlCRSelectFunctionCalculateValue != null)
                {
                    BaseControlCRSelectFunctionCalculateValue = benMAPProject.BaseControlCRSelectFunctionCalculateValue;
                    IncidencePoolingAndAggregationAdvance = benMAPProject.IncidencePoolingAndAggregationAdvance;
                    lstIncidencePoolingAndAggregation = benMAPProject.lstIncidencePoolingAndAggregation;
                    IncidencePoolingResult = benMAPProject.IncidencePoolingResult;
                    BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                    BaseControlCRSelectFunction.BaseControlGroup = BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
                    BaseControlCRSelectFunction.BenMAPPopulation = BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                    BaseControlCRSelectFunction.CRLatinHypercubePoints = BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                    BaseControlCRSelectFunction.CRRunInPointMode = BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                    BaseControlCRSelectFunction.CRSeeds = BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                    BaseControlCRSelectFunction.CRThreshold = BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                    if (BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                    {
                        BaseControlCRSelectFunction.lstCRSelectFunction = BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Select(p => p.CRSelectFunction).ToList();
                    }

                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList();
                    RBenMAPGrid = BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType; LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup; CRThreshold = BaseControlCRSelectFunction.CRThreshold; CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints; CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode; CRSeeds = BaseControlCRSelectFunction.CRSeeds;
                    BenMAPPopulation = BaseControlCRSelectFunction.BenMAPPopulation;
                }
                else if (benMAPProject.BaseControlCRSelectFunction != null)
                {
                    BaseControlCRSelectFunction = benMAPProject.BaseControlCRSelectFunction;
                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList(); RBenMAPGrid = BaseControlCRSelectFunction.RBenMapGrid;
                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType; LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup; CRThreshold = BaseControlCRSelectFunction.CRThreshold; CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints; CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode; CRSeeds = BaseControlCRSelectFunction.CRSeeds;
                    BenMAPPopulation = BaseControlCRSelectFunction.BenMAPPopulation;
                }
                else
                {
                    ManageSetup = benMAPProject.ManageSetup;
                    MainSetup = benMAPProject.MainSetup; LstPollutant = benMAPProject.LstPollutant != null ? benMAPProject.LstPollutant : null; RBenMAPGrid = benMAPProject.RBenMAPGrid != null ? benMAPProject.RBenMAPGrid : null;
                    GBenMAPGrid = benMAPProject.GBenMAPGrid != null ? benMAPProject.GBenMAPGrid : null; if (benMAPProject.LstBaseControlGroup != null)
                    {
                        LstBaseControlGroup = benMAPProject.LstBaseControlGroup;
                    }
                    CRThreshold = benMAPProject.CRThreshold; CRLatinHypercubePoints = benMAPProject.CRLatinHypercubePoints; CRRunInPointMode = benMAPProject.CRRunInPointMode; CRSeeds = benMAPProject.CRSeeds;

                    BenMAPPopulation = benMAPProject.BenMAPPopulation != null ? benMAPProject.BenMAPPopulation : null;
                    lstPollutantAll = benMAPProject.lstPollutantAll;
                }
                IncidencePoolingAndAggregationAdvance = benMAPProject.IncidencePoolingAndAggregationAdvance;
                BenMAPPopulation = benMAPProject.BenMAPPopulation; CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);

                if (LstBaseControlGroup != null)
                {
                    foreach (BaseControlGroup bcg in LstBaseControlGroup)
                    {
                        if (bcg.Base != null) bcg.Base.ShapeFile = "";
                        if (bcg.Control != null) bcg.Control.ShapeFile = "";
                        if (bcg.DeltaQ != null) bcg.DeltaQ.ShapeFile = "";
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Description("当从主窗体进入另外一个窗体后出发改变当前状态提示")]
        public static event FormChangedStatHandler FormChangedStat;

        protected static void OnFormChangedStat()
        {
            if (FormChangedStat != null)
            {
                FormChangedStat();
            }
        }

        private static string _nodeAnscyStatus = "";
        public static string NodeAnscyStatus
        {
            get { return _nodeAnscyStatus; }
            set
            {
                _nodeAnscyStatus = value;
                OnNodeAnscy();
            }
        }
        [Description("当从主窗体进入另外一个窗体后出发改变当前状态提示")]
        public static event FormChangedStatHandler NodeAnscy;
        protected static void OnNodeAnscy()
        {
            if (NodeAnscy != null)
            {
                NodeAnscy();
            }
        }
        [Description("改变Setup时发生")]
        public static event FormChangedStatHandler FormChangedSetup;

        protected static void OnFormChangedSetup()
        {
            if (FormChangedSetup != null)
            {
                FormChangedSetup();
            }
        }
    }
    class Percentile<T> where T : IComparable
    {
        uint position, size, count;
        double percentile;
        private List<T> list;

        public List<T> List
        {
            get { return list; }
        }
        public Percentile(double percentile, uint buffersize)
        {
            size = buffersize;
            this.percentile = percentile;
            count = position = 0;
            list = new List<T>();
        }
        public void add(T newvalue)
        {
            if (++count > size)
            {
                int newpos = (int)System.Math.Floor(count * percentile - size / 2.0);
                if (newpos < 0) newpos = 0;
                if (newvalue.CompareTo(list.First()) > 0)
                {
                    if (newpos > position)
                    {
                        list.RemoveAt(0);
                        ++position;
                    }
                    else
                    {
                        if (newvalue.CompareTo(list.Last()) >= 0)
                            return;
                        list.RemoveAt(list.Count - 1);
                    }
                }
                else
                {
                    if (newpos > position)
                    {
                        ++position;
                    }
                    else
                    {
                        list.RemoveAt(list.Count - 1);
                        list.Insert(0, newvalue);
                    }
                    return;
                }
            }
            int index = list.BinarySearch(newvalue);
            if (index < 0)
                list.Insert(~index, newvalue);
            else
                list.Insert(index, newvalue);
        }
        public T value
        {
            get
            {
                return list.ElementAt((int)System.Math.Floor(count * percentile - position));
            }
        }
    }
    public delegate void FormChangedStatHandler();

    public delegate string AsyncDelegate(BaseControlGroup bcg, ModelDataLine m, string currentStat, out int threadId);

    public delegate string AsynDelegateRollBack(string currentStat, MonitorModelRollbackLine rollbackData, out int threadId);
    [Serializable]
    [ProtoContract]
    public class XMLSerializableDictionary<T, TValue> : Dictionary<T, TValue>, System.Xml.Serialization.IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(T));
            XmlSerializer valueSerializer = null;
            T KeyValue;
            TValue value;
            string typename;
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                KeyValue = (T)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("valuetype");
                typename = reader.ReadString();
                reader.ReadEndElement();
                valueSerializer = GetXmlSerializer(Type.GetType(typename), null);
                reader.ReadStartElement("value");
                value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(KeyValue, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            object value;
            XmlSerializer keySerializer = new XmlSerializer(typeof(T));
            XmlSerializer valueSerializer = null;
            foreach (T key in Keys)
            {
                value = this[key];
                if (value != null)
                {
                    writer.WriteStartElement("item");
                    writer.WriteStartElement("key");
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("valuetype");
                    writer.WriteString(value.GetType().AssemblyQualifiedName);
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");
                    valueSerializer = GetXmlSerializer(value.GetType(), null);
                    valueSerializer.Serialize(writer, value);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
        }
        static Dictionary<string, XmlSerializer> mXmlSerializerTable = new Dictionary<string, XmlSerializer>();
        static object mLockSerializer = new object();
        internal static XmlSerializer GetXmlSerializer(Type type, XmlRootAttribute xRoot)
        {
            string key = type.FullName + (xRoot != null ? xRoot.ElementName : "");
            if (!mXmlSerializerTable.ContainsKey(key))
            {
                lock (mLockSerializer)
                {
                    if (!mXmlSerializerTable.ContainsKey(key))
                    {
                        if (xRoot != null)
                        {
                            mXmlSerializerTable.Add(key, new XmlSerializer(type, xRoot));
                        }
                        else
                        {
                            mXmlSerializerTable.Add(key, new XmlSerializer(type));
                        }
                    }
                }
            }
            return mXmlSerializerTable[key];
        }
    }
    public class FieldCheck
    {
        public string FieldName;
        public bool isChecked;
    }

    [Serializable]
    [ProtoContract]
    public class BenMAPProject
    {
        [ProtoMember(1)]
        public BenMAPSetup ManageSetup;
        [ProtoMember(2)]
        public BenMAPSetup MainSetup;
        [ProtoMember(3)]
        public List<BenMAPPollutant> LstPollutant;
        [ProtoMember(4)]
        public BenMAPGrid RBenMAPGrid;
        [ProtoMember(5)]
        public BenMAPGrid GBenMAPGrid;
        [ProtoMember(6)]
        public List<BaseControlGroup> LstBaseControlGroup;
        [ProtoMember(7)]
        public double CRThreshold = 0;
        [ProtoMember(8)]
        public int CRLatinHypercubePoints = 10;
        [ProtoMember(9)]
        public bool CRRunInPointMode = false;
        [ProtoMember(10)]
        public int CRSeeds = 1;
        [ProtoMember(11)]
        public BenMAPPopulation BenMAPPopulation;
        [ProtoMember(12)]
        public List<BenMAPPollutant> lstPollutantAll;
        [ProtoMember(13)]
        public BaseControlCRSelectFunction BaseControlCRSelectFunction;
        [ProtoMember(14)]
        public BaseControlCRSelectFunctionCalculateValue BaseControlCRSelectFunctionCalculateValue;
        [ProtoMember(15)]
        public IncidencePoolingAndAggregationAdvance IncidencePoolingAndAggregationAdvance;
        [ProtoMember(16)]
        public List<IncidencePoolingAndAggregation> lstIncidencePoolingAndAggregation;
        [ProtoMember(17)]
        public CRSelectFunctionCalculateValue IncidencePoolingResult;
        [ProtoMember(18)]
        public ValuationMethodPoolingAndAggregation ValuationMethodPoolingAndAggregation;
    }

    [Serializable]
    [ProtoContract]
    public class GRGridRelationShip
    {
        [ProtoMember(1)]
        public List<GridRelationship> lstRelationship;
    }


    [Serializable]
    [ProtoContract]
    public class BenMAPSetup
    {
        [ProtoMember(1)]
        public int SetupID;
        [ProtoMember(2)]
        public string SetupName;
    }




    public enum ObservationtypeEnum
    {
        Hourly = 0, Daily = 1
    }

    [Serializable]
    [ProtoContract]
    public class BenMAPPollutant
    {
        [ProtoMember(1)]
        public int PollutantID;
        [ProtoMember(2)]
        public string PollutantName;
        [ProtoMember(3)]
        public ObservationtypeEnum Observationtype;
        [ProtoMember(4)]
        public List<Metric> Metrics;
        [ProtoMember(5)]
        public List<Season> Seasons;
        [ProtoMember(6)]
        public List<SeasonalMetric> SesonalMetrics = new List<SeasonalMetric>();
    }

    [Serializable]
    [ProtoContract]
    public class SeasonalMetric
    {
        [ProtoMember(1)]
        public int SeasonalMetricID;
        [ProtoMember(2)]
        public string SeasonalMetricName;
        [ProtoMember(3)]
        public Metric Metric;
        [ProtoMember(4)]
        public List<Season> Seasons;
    }

    [Serializable]
    [ProtoContract]
    public class Season
    {
        [ProtoMember(1)]
        public int PollutantSeasonID;
        [ProtoMember(2)]
        public int PollutantID;
        [ProtoMember(3)]
        public int StartDay;
        [ProtoMember(4)]
        public int EndDay;
        [ProtoMember(5)]
        public int StartHour;
        [ProtoMember(6)]
        public int EndHour;
        [ProtoMember(7)]
        public int Numbins;
    }

    [Serializable]
    [ProtoContract]
    [ProtoInclude(5, typeof(FixedWindowMetric))]
    [ProtoInclude(6, typeof(MovingWindowMetric))]
    [ProtoInclude(7, typeof(CustomerMetric))]
    public class Metric
    {
        [ProtoMember(1)]
        public int MetricID;
        [ProtoMember(2)]
        public int PollutantID;
        [ProtoMember(3)]
        public string MetricName;
        [ProtoMember(4)]
        public int HourlyMetricGeneration;
    }

    [Serializable]
    [ProtoContract]
    public class FixedWindowMetric : Metric
    {
        [ProtoMember(1)]
        public int StartHour;
        [ProtoMember(2)]
        public int EndHour;
        [ProtoMember(3)]
        public MetricStatic Statistic;
    }

    [Serializable]
    [ProtoContract]
    public class MovingWindowMetric : Metric
    {
        [ProtoMember(1)]
        public int WindowSize;
        [ProtoMember(2)]
        public MetricStatic WindowStatistic;
        [ProtoMember(3)]
        public MetricStatic DailyStatistic;
    }

    [Serializable]
    [ProtoContract]
    public class CustomerMetric : Metric
    {
        [ProtoMember(1)]
        public string MetricFunction;
    }




    public enum GridTypeEnum
    {
        Regular = 0, Shapefile = 1
    }

    [Serializable]
    [ProtoContract]
    [ProtoInclude(8, typeof(RegularGrid))]
    [ProtoInclude(9, typeof(ShapefileGrid))]
    public class BenMAPGrid
    {
        [ProtoMember(1)]
        public int GridDefinitionID;
        [ProtoMember(2)]
        public int SetupID;
        [ProtoMember(3)]
        public string GridDefinitionName;
        [ProtoMember(4)]
        public int Columns;
        [ProtoMember(5)]
        public int RRows;
        [ProtoMember(6)]
        public GridTypeEnum TType;
        [ProtoMember(7)]
        public string SetupName;
    }

    [Serializable]
    [ProtoContract]
    public class RegularGrid : BenMAPGrid
    {
        [ProtoMember(1)]
        public double MinimumLatitude;
        [ProtoMember(2)]
        public double MinimumLongitude;
        [ProtoMember(3)]
        public int ColumnsperLongitude;
        [ProtoMember(4)]
        public int RowsperLatitude;
        [ProtoMember(5)]
        public string ShapefileName;
    }

    [Serializable]
    [ProtoContract]
    public class ShapefileGrid : BenMAPGrid
    {
        [ProtoMember(1)]
        public string ShapefileName;
    }



    [Serializable]
    [ProtoContract]
    public class ModelAttribute
    {
        [ProtoMember(1)]
        public int Row;
        [ProtoMember(2)]
        public int Col;
        [ProtoMember(3)]
        public Metric Metric;
        [ProtoMember(4)]
        public SeasonalMetric SeasonalMetric;
        [ProtoMember(5)]
        public MetricStatic Statistic;
        [ProtoMember(6)]
        public List<float> Values;
    }

    [Serializable]
    [ProtoContract]
    public class ModelResultAttribute
    {
        [ProtoMember(1)]
        public int Row;
        [ProtoMember(2)]
        public int Col;
        [XmlIgnore]
        [ProtoMember(3)]
        public Dictionary<string, float> Values;
    }

    [Serializable]
    [ProtoContract]
    [ProtoInclude(7, typeof(ModelDataLine))]
    [ProtoInclude(8, typeof(MonitorDataLine))]
    public class BenMAPLine
    {
        [ProtoMember(1)]
        public BenMAPGrid GridType;
        [ProtoMember(2)]
        public BenMAPPollutant Pollutant;
        [ProtoMember(3)]
        public List<ModelAttribute> ModelAttributes;
        [ProtoMember(4)]
        public List<ModelResultAttribute> ModelResultAttributes;
        [ProtoMember(6)]
        public string ShapeFile;
        [ProtoMember(9)]
        public DateTime CreateTime;
        [ProtoMember(11)]
        public string Version;
    }

    [Serializable]
    [ProtoContract]
    public class BaseControlGroup
    {
        [ProtoMember(1)]
        public BenMAPGrid GridType;
        [ProtoMember(2)]
        public BenMAPPollutant Pollutant;
        [ProtoMember(3)]
        public BenMAPLine Base;
        [ProtoMember(4)]
        public BenMAPLine Control;
        [ProtoMember(5)]
        public BenMAPLine DeltaQ;
    }

    [Serializable]
    [ProtoContract]
    public class ModelDataLine : BenMAPLine
    {
        [ProtoMember(1)]
        public int DatabaseType;
        [ProtoMember(2)]
        public string DatabaseFilePath;
    }


    public enum InterpolationMethodEnum
    {
        ClosestMonitor = 0, VoronoiNeighborhoodAveragin = 1, FixedRadius = 2
    }

    [Serializable]
    [ProtoContract]

    [ProtoInclude(9, typeof(MonitorModelRelativeLine))]
    [ProtoInclude(10, typeof(MonitorModelRollbackLine))]
    public class MonitorDataLine : BenMAPLine
    {
        [ProtoMember(1)]
        public int MonitorDirectType;
        [ProtoMember(2)]
        public int MonitorDataSetID;
        [ProtoMember(3)]
        public int MonitorLibraryYear;
        [ProtoMember(4)]
        public string MonitorDataFilePath;
        [ProtoMember(5)]
        public string MonitorDefinitionFile;
        [ProtoMember(6)]
        public InterpolationMethodEnum InterpolationMethod;
        [ProtoMember(7)]
        public double FixedRadius;
        [ProtoMember(8)]
        public MonitorAdvance MonitorAdvance;
        [ProtoMember(11)]
        public List<MonitorNeighborAttribute> MonitorNeighbors;
        [ProtoMember(12)]
        public List<MonitorValue> MonitorValues;
    }
    [Serializable]
    [ProtoContract]
    public class MonitorNeighborAttribute
    {
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public string MonitorName;
        [ProtoMember(4)]
        public double Weight;
        [ProtoMember(5)]
        public double Distance;
    }
    public enum WeightingApproachEnum
    {
        InverseDistance = 0, InverseDistanceSquared = 1
    }


    public enum MonitorAdvanceDataTypeEnum
    {
        Local = 0, Standard = 1, Both = 2
    }

    [Serializable]
    [ProtoContract]
    public class MonitorAdvance
    {
        [ProtoMember(1)]
        public double MaxinumNeighborDistance = -1;
        [ProtoMember(2)]
        public double RelativeNeighborDistance = -1;
        [ProtoMember(3)]
        public WeightingApproachEnum WeightingApproach;
        [ProtoMember(4)]
        public List<string> FilterIncludeIDs;
        [ProtoMember(5)]
        public List<string> FilterExcludeIDs;
        [ProtoMember(6)]
        public List<Location> FilterStates;
        [ProtoMember(7)]
        public bool GetClosedIfNoneWithinRadius;
        [ProtoMember(8)]
        public double FilterMinLongitude = -130;
        [ProtoMember(9)]
        public double FilterMaxLongitude = -65;
        [ProtoMember(10)]
        public double FilterMinLatitude = 20;
        [ProtoMember(11)]
        public double FilterMaxLatitude = 55;
        [ProtoMember(12)]
        public int FilterMaximumPOC = -1;
        [ProtoMember(13)]
        public string POCPreferenceOrder;
        [ProtoMember(14)]
        public List<string> IncludeMethods;
        [ProtoMember(15)]
        public MonitorAdvanceDataTypeEnum DataTypesToUse;
        [ProtoMember(16)]
        public MonitorAdvanceDataTypeEnum PreferredType;
        [ProtoMember(17)]
        public MonitorAdvanceDataTypeEnum OutputType;
        public int StartHour = 8;
        public int EndHour = 19;
        public int StartDate = 120;
        public int EndDate = 272;
        public int NumberOfValidHour = 9;
        public int PercentOfValidDays = 50;
        public int NumberOfPerQuarter = 11;
    }


    public enum MonitorModelRelativeScalingTypeEnum
    {
        SpatialOnly = 0, TemporalOnly = 1, SpatialAndTemporal = 2, ModelAdjustment = 3
    }

    [Serializable]
    [ProtoContract]
    public class MonitorModelRelativeLine : MonitorDataLine
    {
        [ProtoMember(1)]
        public int BaseYearModelType;
        [ProtoMember(2)]
        public string BaseYearModelFilePath;
        [ProtoMember(3)]
        public int FutureYearModelType;
        [ProtoMember(4)]
        public string FutureYearModelFilePath;
        [ProtoMember(5)]
        public MonitorModelRelativeScalingTypeEnum MonitorModelRelativeScalingType;
    }

    [Serializable]
    [ProtoContract]
    public class RowCol
    {
        [ProtoMember(1)]
        public int Row;
        [ProtoMember(2)]
        public int Col;
    }


    public class RowColComparer : IEqualityComparer<RowCol>
    {

        public bool Equals(RowCol x, RowCol y)
        {
            return x.Col.Equals(y.Col) && x.Row.Equals(y.Row);
        }


        public int GetHashCode(RowCol obj)
        {
            return 0;
        }
    }


    public enum RollbackType
    { percentage = 0, incremental = 1, standard = 2, quadratic = 3, peakshaving = 4 }


    public enum RollbackMethod
    { percentage = 0, incremental = 1, quadratic = 2, peakshaving = 3 }

    [Serializable]
    [ProtoContract]
    [ProtoInclude(6, typeof(IncrementalRollback))]
    [ProtoInclude(7, typeof(PercentageRollback))]
    [ProtoInclude(8, typeof(StandardRollback))]
    public class BenMAPRollback
    {
        [ProtoMember(1)]
        public int RegionID;
        [ProtoMember(2)]
        public RollbackType RollbackType;
        [ProtoMember(3)]
        public double Background;
        [ProtoMember(4)]
        public List<RowCol> SelectRegions;
        [ProtoMember(5)]
        public string DrawingColor;
    }

    [Serializable]
    [ProtoContract]
    public class PercentageRollback : BenMAPRollback
    {
        [ProtoMember(1)]
        public double Percent;
    }

    [Serializable]
    [ProtoContract]
    public class IncrementalRollback : BenMAPRollback
    {
        [ProtoMember(1)]
        public double Increment;
    }

    [Serializable]
    [ProtoContract]
    public class StandardRollback : BenMAPRollback
    {
        [ProtoMember(1)]
        public Metric DailyMetric;
        [ProtoMember(2)]
        public SeasonalMetric SeasonalMetric;
        [ProtoMember(3)]
        public MetricStatic AnnualStatistic;
        [ProtoMember(4)]
        public int Ordinality = 1;
        [ProtoMember(5)]
        public double Standard;
        [ProtoMember(6)]
        public string InterdayRollbackMethod;
        [ProtoMember(7)]
        public double InterdayBackground;
        [ProtoMember(8)]
        public string IntradayRollbackMethod;
        [ProtoMember(9)]
        public double IntradayBackground;

    }

    [Serializable]
    [ProtoContract]
    public class MonitorModelRollbackLine : MonitorDataLine
    {
        [ProtoMember(1)]
        public BenMAPGrid RollbackGrid;
        [ProtoMember(2)]
        public List<BenMAPRollback> BenMAPRollbacks;
        [ProtoMember(3)]
        public int ScalingMethod;
        [ProtoMember(4)]
        public BenMAPGrid AdditionalGrid;
        [ProtoMember(5)]
        public string AdustmentFilePath;
        [ProtoMember(6)]
        public bool isMakeBaseLineGrid;
    }



    [Serializable]
    [ProtoContract]
    public class Monitor
    {
        [ProtoMember(1)]
        public int MonitorID;
        [ProtoMember(2)]
        public int MonitorDatasetID;

        [ProtoMember(3)]
        public int PollutantID;
        [ProtoMember(4)]
        public double Latitude;
        [ProtoMember(5)]
        public double Longitude;
        [ProtoMember(6)]
        public string MonitorName;
        [ProtoMember(7)]
        public string MonitorDescription;
        [ProtoMember(8)]
        public List<MonitorValue> MonitorValues;
    }

    [Serializable]
    [ProtoContract]
    public class MonitorValue
    {

        [ProtoMember(1)]
        public string MonitorName;
        [ProtoMember(2)]
        public string MonitorMethod;
        [ProtoMember(3)]
        public double Latitude;
        [ProtoMember(4)]
        public double Longitude;
        [ProtoMember(5)]
        public int Row;
        [ProtoMember(6)]
        public int Col;
        [ProtoMember(7)]
        public Metric Metric;

        [ProtoMember(8)]
        public SeasonalMetric SeasonalMetric;

        [ProtoMember(9)]
        public string Statistic;
        [ProtoMember(10)]
        public Dictionary<string, float> dicMetricValues;
        [ProtoMember(11)]
        public Dictionary<string, List<float>> dicMetricValues365;
        public List<float> Values;
        public int MonitorID;
    }
    [Serializable]
    [ProtoContract]
    public class MetricValueAttributes
    {
        [ProtoMember(1)]
        public string MetricName;
        [ProtoMember(2)]
        public float MetricValue;
    }


    [Serializable]
    [ProtoContract]
    public class PopulationAttribute
    {
        [ProtoMember(1)]
        public int Row;

        [ProtoMember(2)]
        public int Col;




        [ProtoMember(3)]
        public float Value;
    }
    [Serializable]
    [ProtoContract]
    public class WeightAttribute
    {
        [ProtoMember(1)]
        public string RaceID;

        [ProtoMember(2)]
        public string EthnicityID;




        [ProtoMember(3)]
        public double Value;
    }

    [Serializable]
    [ProtoContract]
    public class BenMAPPopulation
    {
        [ProtoMember(1)]
        public int DataSetID;
        [ProtoMember(2)]
        public string DataSetName;
        [ProtoMember(3)]
        public int Year;
        [ProtoMember(4)]
        public BenMAPGrid GridType;
        [ProtoMember(5)]
        public int PopulationConfiguration;
    }




    public enum MetricStatic
    {
        None = 0, Mean = 1, Median = 2, Max = 3, Min = 4, Sum = 5
    }

    [Serializable]
    [ProtoContract]
    public class Location
    {
        [ProtoMember(1)]
        public int LocationType;
        [ProtoMember(2)]
        public int GridDifinitionID;
        [ProtoMember(3)]
        public int LocationID;
        [ProtoMember(4)]
        public int Col;
        [ProtoMember(5)]
        public int Row;
        [ProtoMember(6)]
        public string LocationName;
    }

    [Serializable]
    [ProtoContract]
    public class RegionTypeGrid
    {
        [ProtoMember(1)]
        public int RegionType;
        [ProtoMember(2)]
        public int FIPSCode;
        [ProtoMember(3)]
        public List<RowCol> RowCols;
    }

    [Serializable]
    [ProtoContract]
    public class GridRelationshipAttribute
    {
        [ProtoMember(1)]
        public RowCol bigGridRowCol;
        [ProtoMember(2)]
        public List<RowCol> smallGridRowCol;
    }
    [Serializable]
    [ProtoContract]
    public class GridRelationshipAttributePercentage
    {
        [ProtoMember(1)]
        public int sourceCol;
        [ProtoMember(2)]
        public int sourceRow;
        [ProtoMember(3)]
        public int targetCol;
        [ProtoMember(4)]
        public int targetRow;
        [ProtoMember(5)]
        public double percentage;

    }
    [Serializable]
    [ProtoContract]
    public class GridRelationship
    {
        [ProtoMember(1)]
        public int bigGridID;
        [ProtoMember(2)]
        public int smallGridID;
        [ProtoMember(3)]
        public List<GridRelationshipAttribute> lstGridRelationshipAttribute;
    }

    [Serializable]
    [ProtoContract]
    public class IncidenceRateAttribute
    {
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public float Value;
    }

    [Serializable]
    [ProtoContract]
    public class SetupVariableValues
    {
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public float Value;
    }

    [Serializable]
    [ProtoContract]
    public class SetupVariableJoinAllValues
    {
        [ProtoMember(1)]
        public string SetupVariableName;
        [ProtoMember(2)]
        public int SetupVariableID;
        [ProtoMember(3)]
        public int SetupVariableGridType;
        [ProtoMember(4)]
        public List<SetupVariableValues> lstValues;
    }

    [Serializable]
    [ProtoContract]
    public class BenMAPHealthImpactFunction
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public int DataSetID;
        [ProtoMember(3)]
        public string DataSetName;
        [ProtoMember(4)]
        public int EndPointGroupID;
        [ProtoMember(5)]
        public string EndPointGroup;
        [ProtoMember(6)]
        public int EndPointID;
        [ProtoMember(7)]
        public string EndPoint;
        [ProtoMember(8)]
        public BenMAPPollutant Pollutant;
        [ProtoMember(9)]
        public Metric Metric;
        [ProtoMember(10)]
        public MetricStatic MetricStatistic;
        [ProtoMember(11)]
        public SeasonalMetric SeasonalMetric;
        [ProtoMember(12)]
        public string Race;
        [ProtoMember(13)]
        public string Ethnicity;
        [ProtoMember(14)]
        public string Gender;
        [ProtoMember(15)]
        public int Percentile;
        [ProtoMember(16)]
        public int StartAge = -1;
        [ProtoMember(17)]
        public int EndAge = -1;
        [ProtoMember(18)]
        public string Author;
        [ProtoMember(19)]
        public int Year;
        [ProtoMember(20)]
        public List<Location> Locations;
        [ProtoMember(21)]
        public string strLocations;
        [ProtoMember(22)]
        public string Qualifier;
        [ProtoMember(23)]
        public string OtherPollutants;
        [ProtoMember(24)]
        public string Reference;
        [ProtoMember(25)]
        public string Function;
        [ProtoMember(26)]
        public string BaseLineIncidenceFunction;
        [ProtoMember(27)]
        public string BetaDistribution;
        [ProtoMember(28)]
        public double Beta;
        [ProtoMember(29)]
        public double BetaParameter1;
        [ProtoMember(30)]
        public double BetaParameter2;
        [ProtoMember(31)]
        public string AContantDescription;
        [ProtoMember(32)]
        public double AContantValue;
        [ProtoMember(33)]
        public string BContantDescription;
        [ProtoMember(34)]
        public double BContantValue;
        [ProtoMember(35)]
        public string CContantDescription;
        [ProtoMember(36)]
        public double CContantValue;
        [ProtoMember(37)]
        public int IncidenceDataSetID = -1;
        [ProtoMember(38)]
        public int PrevalenceDataSetID = -1;
        [ProtoMember(39)]
        public int VariableDataSetID = -1;
        [ProtoMember(40)]
        public string IncidenceDataSetName;
        [ProtoMember(41)]
        public string PrevalenceDataSetName;
        [ProtoMember(42)]
        public string VariableDataSetName;
    }

    [Serializable]
    [ProtoContract]
    public class CRSelectFunction
    {
        [ProtoMember(1)]
        public int CRID;
        [ProtoMember(2)]
        public BenMAPHealthImpactFunction BenMAPHealthImpactFunction;
        [ProtoMember(3)]
        public List<Location> Locations;
        [ProtoMember(4)]
        public string Race;
        [ProtoMember(5)]
        public string Ethnicity;
        [ProtoMember(6)]
        public string Gender;
        [ProtoMember(7)]
        public int StartAge = 0;
        [ProtoMember(8)]
        public int EndAge = 0;
        [ProtoMember(9)]
        public int IncidenceDataSetID = -1;
        [ProtoMember(10)]
        public string IncidenceDataSetName;
        [ProtoMember(11)]
        public int PrevalenceDataSetID = -1;
        [ProtoMember(12)]
        public string PrevalenceDataSetName;
        [ProtoMember(13)]
        public int VariableDataSetID = -1;
        [ProtoMember(14)]
        public string VariableDataSetName;
        [ProtoMember(15)]
        public List<LatinPoints> lstLatinPoints;
    }

    [ProtoContract]
    public class LatinPoints
    {
        [ProtoMember(1)]
        public List<double> values;
    }
    [Serializable]
    [ProtoContract]
    public class CRSelectFunctionCalculateValue
    {
        [ProtoMember(1)]
        public CRSelectFunction CRSelectFunction;
        [ProtoMember(2)]
        public List<CRCalculateValue> CRCalculateValues;

    }

    [Serializable]
    [ProtoContract]
    public class CRCalculateValue
    {
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public float PointEstimate;
        [ProtoMember(4)]
        public float Population;
        [ProtoMember(5)]
        public float Incidence;
        [ProtoMember(6)]
        public float Delta;
        [ProtoMember(7)]
        public float Mean;
        [ProtoMember(8)]
        public float Baseline;
        [ProtoMember(9)]
        public float PercentOfBaseline;
        [ProtoMember(10)]
        public float StandardDeviation;
        [ProtoMember(11)]
        public float Variance;
        [ProtoMember(12)]
        public List<float> LstPercentile;
    }

    [Serializable]
    [ProtoContract]
    public class BaseControlCRSelectFunction
    {
        [ProtoMember(1)]
        public List<BaseControlGroup> BaseControlGroup;
        [ProtoMember(2)]
        public List<CRSelectFunction> lstCRSelectFunction;
        [ProtoMember(3)]
        public double CRThreshold = 0;
        [ProtoMember(4)]
        public int CRLatinHypercubePoints = 10;
        [ProtoMember(5)]
        public bool CRRunInPointMode = false;
        [ProtoMember(6)]
        public BenMAPPopulation BenMAPPopulation;
        [ProtoMember(7)]
        public BenMAPGrid RBenMapGrid;
        [ProtoMember(8)]
        public int CRSeeds = 1;
        [ProtoMember(9)]
        public DateTime CreateTime;
        [ProtoMember(10)]
        public string Version;
    }

    [Serializable]
    [ProtoContract]
    public class BaseControlCRSelectFunctionCalculateValue
    {
        [ProtoMember(1)]
        public List<BaseControlGroup> BaseControlGroup;
        [ProtoMember(2)]
        public List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue;
        [ProtoMember(3)]
        public double CRThreshold = 0;
        [ProtoMember(4)]
        public int CRLatinHypercubePoints = 10;
        [ProtoMember(5)]
        public bool CRRunInPointMode = false;
        [ProtoMember(6)]
        public BenMAPPopulation BenMAPPopulation;
        [ProtoMember(7)]
        public BenMAPGrid RBenMapGrid;
        [ProtoMember(8)]
        public int CRSeeds = 1;
        [ProtoMember(9)]
        public List<string> lstLog;
        [ProtoMember(10)]
        public DateTime CreateTime;
        [ProtoMember(11)]
        public string Version;
    }



    [Serializable]
    [ProtoContract]
    public class BenMAPQALY
    {
        [ProtoMember(1)]
        public int QalyDatasetID;
        [ProtoMember(2)]
        public int SetupID;
        [ProtoMember(3)]
        public string QalyDatasetName;
        [ProtoMember(4)]
        public string EndPointGroup;
        [ProtoMember(5)]
        public string EndPoint;
        [ProtoMember(6)]
        public string Qualifier;
        [ProtoMember(7)]
        public string Description;
        [ProtoMember(8)]
        public int StartAge;
        [ProtoMember(9)]
        public int EndAge;
        [ProtoMember(10)]
        public double QALYValue;
    }

    [Serializable]
    [ProtoContract]
    public class AllSelectQALYMethod
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public string PoolingMethod;

        [ProtoMember(3)]
        public string EndPointGroup;
        [ProtoMember(4)]
        public string EndPoint;
        [ProtoMember(5)]
        public string Author;
        [ProtoMember(6)]
        public string Qualifier;
        [ProtoMember(7)]
        public string Location;
        [ProtoMember(8)]
        public string StartAge;
        [ProtoMember(9)]
        public string EndAge;
        [ProtoMember(10)]
        public string Year;
        [ProtoMember(11)]
        public string OtherPollutants;
        [ProtoMember(12)]
        public string Race;
        [ProtoMember(13)]
        public string Ethnicity;
        [ProtoMember(14)]
        public string Gender;
        [ProtoMember(15)]
        public string Function;
        [ProtoMember(16)]
        public string Pollutant;
        [ProtoMember(17)]
        public string Metric;
        [ProtoMember(18)]
        public string SeasonalMetric;
        [ProtoMember(19)]
        public string MetricStatistic;
        [ProtoMember(20)]
        public string DataSet;
        [ProtoMember(21)]
        public string Version;


        [ProtoMember(22)]
        public int NodeType;
        [ProtoMember(23)]
        public int ID;
        [ProtoMember(24)]
        public int PID;
        [ProtoMember(25)]
        public int EndPointID;
        [ProtoMember(26)]
        public int CRID;
        [ProtoMember(27)]
        public int QalyDatasetID;
        [ProtoMember(28)]
        public BenMAPQALY BenMAPQALY;
        [ProtoMember(29)]
        public double Weight;
        [ProtoMember(30)]
        public List<float> lstQALYLast;
        [ProtoMember(31)]
        public float fQALYFirst = -9999;
    }

    [Serializable]
    [ProtoContract]
    public class QALYValueAttribute
    {
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public float PointEstimate;
        [ProtoMember(4)]
        public float Mean;
        [ProtoMember(5)]
        public float StandardDeviation;
        [ProtoMember(6)]
        public float Variance;
        [ProtoMember(7)]
        public List<float> LstPercentile;
    }

    [Serializable]
    [ProtoContract]
    public class AllSelectQALYMethodAndValue
    {
        [ProtoMember(1)]
        public AllSelectQALYMethod AllSelectQALYMethod;
        [ProtoMember(2)]
        public List<QALYValueAttribute> lstQALYValueAttributes;
    }

    [Serializable]
    [ProtoContract]
    public class AllSelectValuationMethod
    {
        [ProtoMember(1)]
        public int CRIndex = -1;
        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public string EndPointGroup;
        [ProtoMember(4)]
        public string EndPoint;
        [ProtoMember(5)]
        public string Author;
        [ProtoMember(6)]
        public string Qualifier;
        [ProtoMember(7)]
        public string Location;
        [ProtoMember(8)]
        public string StartAge;
        [ProtoMember(9)]
        public string EndAge;
        [ProtoMember(10)]
        public string Year;
        [ProtoMember(11)]
        public string OtherPollutants;
        [ProtoMember(12)]
        public string Race;
        [ProtoMember(13)]
        public string Ethnicity;
        [ProtoMember(14)]
        public string Gender;
        [ProtoMember(15)]
        public string Function;
        [ProtoMember(16)]
        public string Pollutant;
        [ProtoMember(17)]
        public string Metric;
        [ProtoMember(18)]
        public string SeasonalMetric;
        [ProtoMember(19)]
        public string MetricStatistic;
        [ProtoMember(20)]
        public string DataSet;
        [ProtoMember(21)]
        public string Version;
        [ProtoMember(22)]
        public string PoolingMethod;
        [ProtoMember(23)]
        public int NodeType;
        [ProtoMember(24)]
        public int ID;
        [ProtoMember(25)]
        public int PID;
        [ProtoMember(26)]
        public int EndPointID;
        [ProtoMember(27)]
        public int CRID;
        [ProtoMember(28)]
        public int APVID;
        [ProtoMember(29)]
        public BenMAPValuationFunction BenMAPValuationFunction;
        [ProtoMember(30)]
        public double Weight;
        [ProtoMember(31)]
        public List<LatinPoints> lstMonte;
    }

    [Serializable]
    [ProtoContract]
    public class AllSelectCRFunction
    {
        [ProtoMember(1)]
        public int CRIndex = -1;
        [ProtoMember(2)]
        public string Version;
        [ProtoMember(3)]
        public int EndPointGroupID = -1;
        [ProtoMember(4)]
        public string Name;
        [ProtoMember(5)]
        public string PoolingMethod;

        [ProtoMember(6)]
        public string EndPointGroup;
        [ProtoMember(7)]
        public string EndPoint;
        [ProtoMember(8)]
        public string Author;
        [ProtoMember(9)]
        public string Qualifier;
        [ProtoMember(10)]
        public string Location;
        [ProtoMember(11)]
        public string StartAge;
        [ProtoMember(12)]
        public string EndAge;
        [ProtoMember(13)]
        public string Year;
        [ProtoMember(14)]
        public string OtherPollutants;
        [ProtoMember(15)]
        public string Race;
        [ProtoMember(16)]
        public string Ethnicity;
        [ProtoMember(17)]
        public string Gender;
        [ProtoMember(18)]
        public string Function;
        [ProtoMember(19)]
        public string Pollutant;
        [ProtoMember(20)]
        public string Metric;
        [ProtoMember(21)]
        public string SeasonalMetric;
        [ProtoMember(22)]
        public string MetricStatistic;
        [ProtoMember(23)]
        public string DataSet;

        [ProtoMember(24)]
        public int NodeType;
        [ProtoMember(25)]
        public int ID;
        [ProtoMember(26)]
        public int PID;
        [ProtoMember(27)]
        public int EndPointID = -1;
        [ProtoMember(28)]
        public int CRID = -1;
        [ProtoMember(29)]
        public CRSelectFunctionCalculateValue CRSelectFunctionCalculateValue;
        [ProtoMember(30)]
        public double Weight;
    }

    [Serializable]
    [ProtoContract]
    public class AllSelectValuationMethodAndValue
    {
        [ProtoMember(1)]
        public AllSelectValuationMethod AllSelectValuationMethod;
        [ProtoMember(2)]
        public List<APVValueAttribute> lstAPVValueAttributes;
    }


    public enum PoolingMethodTypeEnum
    {
        None = 0, SumDependent = 1, SumIndependent = 2, SubtractionDependent = 3, SubtractionIndependent = 4, SubjectiveWeights = 5, RandomOrFixedEffects = 6, FixedEffects = 7
    }


    public enum IPAdvancePoolingMethodEnum
    {
        Roundweightstotwodigits = 0, Roundweightstothreedigits = 1, UseexactweightsforMonteCarlo = 2
    }

    [Serializable]
    [ProtoContract]
    public class IncidencePoolingAndAggregationAdvance
    {
        [ProtoMember(1)]
        public BenMAPGrid IncidenceAggregation;
        [ProtoMember(2)]
        public BenMAPGrid ValuationAggregation;
        [ProtoMember(3)]
        public BenMAPGrid QALYAggregation;
        [ProtoMember(4)]
        public IPAdvancePoolingMethodEnum IPAdvancePoolingMethod;
        [ProtoMember(5)]
        public int DefaultMonteCarloIterations = 5000;
        [ProtoMember(6)]
        public string RandomSeed = "1";
        [ProtoMember(7)]
        public bool SortIncidenceResults;
        [ProtoMember(8)]
        public int InflationDatasetID = -1;
        [ProtoMember(9)]
        public string InflationDatasetName = "";
        [ProtoMember(10)]
        public int CurrencyYear = -1;
        [ProtoMember(11)]
        public int AdjustIncomeGrowthDatasetID = -1;
        [ProtoMember(12)]
        public string AdjustIncomeGrowthDatasetName = "";
        [ProtoMember(13)]
        public int IncomeGrowthYear = -1;
        [ProtoMember(14)]
        public List<string> EndpointGroups;
    }

    [Serializable]
    [ProtoContract]
    public class IncidencePoolingAndAggregation
    {
        [ProtoMember(1)]
        public string PoolingName;
        [ProtoMember(2)]
        public List<string> lstColumns;
        [ProtoMember(3)]
        public List<AllSelectCRFunction> lstAllSelectCRFuntion;
        [ProtoMember(4)]
        public List<double> Weights;
        [ProtoMember(5)]
        public string ConfigurationResultsFilePath;
        [ProtoMember(6)]
        public string VariableDataset;
    }

    [Serializable]
    [ProtoContract]
    public class APVValueAttribute
    {
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public float PointEstimate;
        [ProtoMember(4)]
        public float Mean = 0;
        [ProtoMember(5)]
        public float StandardDeviation = 0;
        [ProtoMember(6)]
        public float Variance = 0;
        [ProtoMember(7)]
        public List<float> LstPercentile;
    }

    [Serializable]
    [ProtoContract]
    public class BenMAPValuationFunction
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public string DataSet;
        [ProtoMember(3)]
        public int EndPointGroupID;
        [ProtoMember(4)]
        public string EndPointGroup;
        [ProtoMember(5)]
        public int EndPointID;
        [ProtoMember(6)]
        public string EndPoint;
        [ProtoMember(7)]
        public int StartAge = -1;
        [ProtoMember(8)]
        public int EndAge = -1;

        [ProtoMember(9)]
        public string Qualifier;
        [ProtoMember(10)]
        public string Reference;
        [ProtoMember(11)]
        public string Function;

        [ProtoMember(12)]
        public string NameA;
        [ProtoMember(13)]
        public string DistA;
        [ProtoMember(14)]
        public double A;
        [ProtoMember(15)]
        public double P1A;
        [ProtoMember(16)]
        public double P2A;
        [ProtoMember(17)]
        public string NameB;
        [ProtoMember(18)]
        public double B;
        [ProtoMember(19)]
        public string NameC;
        [ProtoMember(20)]
        public double C;
        [ProtoMember(21)]
        public string NameD;
        [ProtoMember(22)]
        public double D;
    }

    [Serializable]
    [ProtoContract]
    public class ValuationMethodPoolingAndAggregation
    {
        [ProtoMember(1)]
        public string CFGRPath = "";
        [ProtoMember(2)]
        public BaseControlCRSelectFunctionCalculateValue BaseControlCRSelectFunctionCalculateValue;
        [ProtoMember(3)]
        public IncidencePoolingAndAggregationAdvance IncidencePoolingAndAggregationAdvance;
        [ProtoMember(4)]
        public List<ValuationMethodPoolingAndAggregationBase> lstValuationMethodPoolingAndAggregationBase;
        [ProtoMember(5)]
        public List<string> lstLog;
        [ProtoMember(6)]
        public DateTime CreateTime;
        [ProtoMember(7)]
        public int VariableDatasetID = -1;
        [ProtoMember(8)]
        public string Version;
        [ProtoMember(9)]
        public string VariableDatasetName = "";
    }

    [Serializable]
    [ProtoContract]
    public class ValuationMethodPoolingAndAggregationBase
    {
        [ProtoMember(1)]
        public IncidencePoolingAndAggregation IncidencePoolingAndAggregation;
        [ProtoMember(2)]
        public List<string> lstValuationColumns;
        [ProtoMember(3)]
        public List<AllSelectValuationMethod> LstAllSelectValuationMethod;
        [ProtoMember(4)]
        public List<AllSelectValuationMethodAndValue> LstAllSelectValuationMethodAndValue;
        [ProtoMember(5)]
        public List<string> lstQALYColumns;
        [ProtoMember(6)]
        public List<AllSelectQALYMethod> lstAllSelectQALYMethod;
        [ProtoMember(7)]
        public List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue;
        [ProtoMember(8)]
        public CRSelectFunctionCalculateValue IncidencePoolingResult;

        [ProtoMember(9)]
        public List<AllSelectValuationMethodAndValue> LstAllSelectValuationMethodAndValueAggregation;
        [ProtoMember(10)]
        public List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValueAggregation;
        [ProtoMember(11)]
        public CRSelectFunctionCalculateValue IncidencePoolingResultAggregation;
    }



    [Serializable]
    [ProtoContract]
    public class ChartResult
    {
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public string RegionName;
        [ProtoMember(4)]
        public double RegionValue;
    }


    public enum enumDatabaseExport
    {
        Setups = 0,
        GridDefinitions = 1,
        Pollutants = 2,
        MonitorDatasets = 3,
        IncidenceDatasets = 4,
        PopulationDatasets = 5,
        CrfunctionDatasets = 6,
        SetupvariableDatasets = 7,
        InflationDatasets = 8,
        ValuationfunctionDatasets = 9,
        IncomegrowthadjDatasets = 10,
        QalyDatasets = 11
    }

    [Serializable]
    [ProtoContract]
    public class objDatabaseExport
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public enumDatabaseExport objType;
        [ProtoMember(3)]
        public string name;
        [ProtoMember(4)]
        public int tableID;
        [ProtoMember(5)]
        public int PID;
    }

    public class BatchBase
    {
        public string ActiveSetup;
        public ArrayList BatchText;
    }
    public class BatchAQGBase : BatchBase
    {
        public string Filename;
        public string GridType;
        public string Pollutant;
    }
    public class BatchModelDirect : BatchAQGBase
    {
        public string ModelFilename;
        public string DSNName;
    }
    public class BatchMonitorDirect : BatchAQGBase
    {
        public string MonitorDataType; public string InterpolationMethod;
        public string MonitorDataSet;
        public int MonitorYear;
        public string MonitorFile;
        public double MaxDistance = -1;
        public double MaxRelativeDistance = -1;
        public double FixRadius = -1;
        public string WeightingMethod;

    }
    public class BatchMonitorRollbackDirect : BatchAQGBase
    {

    }
    public class BatchCFG : BatchBase
    {
        public string CFGFilename;
        public string ResultsFilename;
        public string BaselineAQG;
        public string ControlAQG;
        public int Year = -1;
        public int LatinHypercubePoints = -1;
        public int Seeds = 1;
        public double Threshold = -1;
    }
    public class BatchAPV : BatchBase
    {
        public string APVFilename;
        public string ResultsFilename;
        public string CFGRFilename;
        public string IncidenceAggregation;
        public string ValuationAggregation;
        public int RandomSeed;
        public string DollarYear;

    }
    public class BatchReport : BatchBase
    {

        public string InputFile;
        public string ReportFile;

    }
    public class BatchReportAuditTrail : BatchReport
    { }
    public class BatchReportCFGR : BatchReport
    {
        public string GridFields;
        public string CustomFields;
        public string ResultFields;
        public string Grouping;
        public string DecimalDigits;

    }
    public class BatchReportAPVR : BatchReport
    {
        public string ResultType;
        public string Totals;
        public string GridFields;
        public string CustomFields;
        public string ResultFields;

    }


    public class RegexUtilities
    {
        bool invalid = false;

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            }
            catch (Exception ex)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$",
                      RegexOptions.IgnoreCase);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }



}