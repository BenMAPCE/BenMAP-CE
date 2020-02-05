using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using DotSpatial.Data;
using System.Xml.Serialization;
using ProtoBuf;
using System.Collections;
using System.Runtime.InteropServices;
using Excel;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
using benmap;
using DotSpatial.Projections;
using System.Diagnostics;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using BrightIdeasSoftware;

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
        // 2014 12 22 - added destructor to try and force connection to close on exit
        ~CommonClass(){ // class destructor
            // close connection if open
            if ((_connection == null) || (_connection.State != ConnectionState.Open))
            {
                _connection.Close();
            }
            
        }
        // values used to specify a grid cell to print debugging output for
        
        public static int debugRow = 2;
        public static int debugCol = 1;
        public static bool debugGridCell=true;
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

        public static void EmptyTmpFolder()
        {
            String tempShpLocDir = CommonClass.DataFilePath + @"\Tmp";
            DirectoryInfo di = new DirectoryInfo(tempShpLocDir);

            foreach (FileInfo file in di.GetFiles())
            {
                if (!file.Name.EndsWith(".dll"))
                {
                    file.Delete();
                }

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
                        _dataFilePath = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + @"\BenMAP-CE";
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
                if ((_connection == null) || (_connection.State != ConnectionState.Open))
                {
                    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
                    string str = settings.ConnectionString;
                    //if (!str.Contains(":"))
                    //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
                    //need to modify string to use general data location
                    str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

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
            // 2015 01 29 - removed code and return class connection to prevent spawning connections that were never closing.
            return CommonClass.Connection;

            
     /*       ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            string str = settings.ConnectionString;
            //if (!str.Contains(":"))
            //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
            str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                    
            FbConnection connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);

            return connection;*/
             
        }

        public static FbConnection getNewConnectionForTransactions()
        {
            // 2015 01 29 - removed code and return class connection to prevent spawning connections that were never closing.
            // STOPPED HERE
            // return CommonClass.Connection;


            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            string str = settings.ConnectionString;
            //if (!str.Contains(":"))
            //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
            str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

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
                    GeographicAreaName = cr.GeographicAreaName,
                    GeographicAreaFeatureID = cr.GeographicAreaFeatureID,
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
                                        GeographicAreaName = benMAPHealthImpactFunction.GeographicAreaName,
                                        GeographicAreaFeatureID = benMAPHealthImpactFunction.GeographicAreaFeatureID,
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

        public static BenMAPGrid GBenMAPGrid; public static List<BaseControlGroup> LstBaseControlGroup; public static double CRThreshold = 0; public static int CRLatinHypercubePoints = 20; public static bool CRRunInPointMode = false; public static int CRSeeds = 1; public static int CRDefaultMonteCarloIterations = 10000;  public static BenMAPPopulation BenMAPPopulation;
        public static List<GridRelationship> LstCurrentGridRelationship; public static string CurrentStat;
        public static List<string> LstAsynchronizationStates;

        public static Dictionary<string, Dictionary<string, float>> DicPopulationAgeInCache = new Dictionary<string, Dictionary<string, float>>();

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
                    List<GridRelationship> lstGridRelationship = new List<GridRelationship>(); 
                    lstGridRelationshipAll = new List<GridRelationship>(); 
                    string commandText = "select   PercentageID,SourceGridDefinitionID,TargetGridDefinitionID  from GridDefinitionPercentages";
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
       public static bool Debug = false;

       public static Boolean getDebugValue()
        {
            #if DEBUG
                return true;
            #endif
            return Debug;
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
                    List<Point> lstMid = new List<Point>();
                    for (int i = 0; i < fsSmall.DataTable.Rows.Count; i++)
                    {
                        IFeature f = fsSmall.GetFeature(i);
                        lstRowColSmall.Add(new RowCol() { Col = Convert.ToInt32(f.DataRow["Col"]), Row = Convert.ToInt32(f.DataRow["Row"]) });
                        lstMid.Add(new Point(f.Geometry.Centroid.Coordinates[0]));
                    }
                    for (int j = 0; j < fsBig.DataTable.Rows.Count; j++)
                    {
                        IFeature f = fsBig.GetFeature(j);
                        GridRelationshipAttribute gra = new GridRelationshipAttribute();
                        gra.bigGridRowCol = new RowCol() { Col = Convert.ToInt32(f.DataRow["Col"]), Row = Convert.ToInt32(f.DataRow["Row"]) };
                        gra.smallGridRowCol = new List<RowCol>();
                        Extent ext = f.Geometry.EnvelopeInternal.ToExtent();
                        for (int i = 0; i < lstMid.Count(); i++)
                        {
                            if (ext.Contains(lstMid[i].Coordinate))
                            {
                                if (f.Geometry.Contains(lstMid[i]))
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
                    List<int> potentialOthers = other.SelectIndices(selfFeature.Geometry.EnvelopeInternal.ToExtent());
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
                    IGeometry g = union.Geometry;
                    for (int i = 1; i < other.Features.Count; i++)
                    {
                        g = g.Union(other.Features[i].Geometry);

                    }
                    union.Geometry = g;

                    Extent otherEnvelope = new Extent(union.Geometry.EnvelopeInternal);
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
                    //Polygon pSelfExtent = null;
                   // Polygon pOtherExtent = null;


                    //ensure consistent GIS projections
                    //check for setup projection
                    ProjectionInfo projInfo = null;
                    if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                    {
                        projInfo = CommonClass.getProjectionInfoFromName(CommonClass.MainSetup.SetupProjection);
                    }
                    if (projInfo == null) //if no setup projection, use default of WGS1984
                    {
                        projInfo = KnownCoordinateSystems.Geographic.World.WGS1984;
                    }

                    self.Reproject(projInfo);

                    other.Reproject(projInfo);


                    double dSumArea = 0.0;
                    foreach (IFeature selfFeature in self.Features)
                    {
                        IFeature intersactFeature = null; if (big == 20)
                        {
                            dSumArea += selfFeature.Geometry.Area;
                        }
                        if (self.Filename == other.Filename)
                        {
                            intersactFeature = selfFeature;
                            if (dicRelation.ContainsKey(selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]))
                            {
                                if (big == 20)
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                        (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area / selfFeature.Geometry.Area);
                                }
                                else
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                    (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area);

                                }
                            }
                            else
                            {
                                dicRelation.Add(selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"],
                                    new Dictionary<string, double>());
                                if (big == 20)
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                       (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area / selfFeature.Geometry.Area);
                                }
                                else
                                {
                                    dicRelation[selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"]].Add
                                   (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area);

                                }
                            }

                        }
                        else
                        {
                            List<int> potentialOthers = other.SelectIndices(selfFeature.Geometry.EnvelopeInternal.ToExtent());
                            foreach (int iotherFeature in potentialOthers)
                            {
                                intersactFeature = null;
                                if ((other.Features.Count < 5 || self.Features.Count < 5) && other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Minimum)) == 0 &&
                                    other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Maximum.X, selfFeature.Geometry.EnvelopeInternal.Minimum.Y)) == 0 &&
                                    other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Maximum)) == 0 &&
                                    other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Minimum.X, selfFeature.Geometry.EnvelopeInternal.Maximum.Y)) == 0
                                    )
                                {
                                    intersactFeature = selfFeature;
                                }
                                else
                                    if ((other.Features.Count < 5 || self.Features.Count < 5) && selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Minimum)) == 0 &&
                                       selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Maximum.X, other.Features[iotherFeature].Geometry.EnvelopeInternal.Minimum.Y)) == 0 &&
                                        selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Maximum)) == 0 &&
                                        selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Minimum.X, other.Features[iotherFeature].Geometry.EnvelopeInternal.Maximum.Y)) == 0)
                                    {
                                        intersactFeature = other.Features[iotherFeature];
                                    }
                                    else
                                    {
                                        try
                                        {
                                            intersactFeature = selfFeature.Intersection(other.Features[iotherFeature].Geometry);
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                    }

                                try
                                {
                                    if (intersactFeature != null && intersactFeature.Geometry != null)
                                    {

                                        if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                                        {
                                            if (big == 20)
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                    (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area / other.Features[iotherFeature].Geometry.Area);
                                            }
                                            else
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area);

                                            }
                                        }
                                        else
                                        {
                                            dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                                                new Dictionary<string, double>());
                                            if (big == 20)
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                   (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area / other.Features[iotherFeature].Geometry.Area);
                                            }
                                            else
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                               (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area);

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

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("HH:mm:ss");
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

                    //ensure consistent GIS projections
                    //check for setup projection
                    ProjectionInfo projInfo = null;
                    if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                    {
                        projInfo = CommonClass.getProjectionInfoFromName(CommonClass.MainSetup.SetupProjection);
                    }
                    if (projInfo == null) //if no setup projection, use default of WGS1984
                    {
                        projInfo = KnownCoordinateSystems.Geographic.World.WGS1984;
                    }

                    self.Reproject(projInfo);
       
                    other.Reproject(projInfo);

                    foreach (IFeature selfFeature in self.Features)
                    {
                        List<int> potentialOthers = other.SelectIndices(selfFeature.Geometry.EnvelopeInternal.ToExtent());
                        foreach (int iotherFeature in potentialOthers)
                        {
                            if (iotherFeature == 33)
                            {
                            }
                            IFeature intersactFeature = null;


                            if ((other.Features.Count < 5 || self.Features.Count < 5) && other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Minimum)) == 0 &&
other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Maximum.X, selfFeature.Geometry.EnvelopeInternal.Minimum.Y)) == 0 &&
other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Maximum)) == 0 &&
other.Features[iotherFeature].Geometry.Distance(new Point(selfFeature.Geometry.EnvelopeInternal.Minimum.X, selfFeature.Geometry.EnvelopeInternal.Maximum.Y)) == 0
)
                            {
                                intersactFeature = selfFeature;
                            }

                            else if ((other.Features.Count < 5 || self.Features.Count < 5) && selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Minimum)) == 0 &&
                           selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Maximum.X, other.Features[iotherFeature].Geometry.EnvelopeInternal.Minimum.Y)) == 0 &&
                            selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Maximum)) == 0 &&
                            selfFeature.Geometry.Distance(new Point(other.Features[iotherFeature].Geometry.EnvelopeInternal.Minimum.X, other.Features[iotherFeature].Geometry.EnvelopeInternal.Maximum.Y)) == 0)
                            {
                                intersactFeature = other.Features[iotherFeature];
                            }
                            else
                            {
                                try
                                {
                                    intersactFeature = selfFeature.Intersection(other.Features[iotherFeature].Geometry);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        if (selfFeature.Geometry.IsWithinDistance(other.Features[iotherFeature].Geometry, 0.00001))
                                        {
                                            if (selfFeature.Geometry.Area > other.Features[iotherFeature].Geometry.Area)
                                            {
                                                bool isContains = false;
                                                isContains = polygonContainPolygon(selfFeature, other.Features[iotherFeature]);
                                                if (isContains)
                                                {
                                                    intersactFeature = other.Features[iotherFeature];
                                                }
                                                else
                                                    intersactFeature = null;
                                            }
                                            else if (selfFeature.Geometry.Area < other.Features[iotherFeature].Geometry.Area)
                                            {
                                                intersactFeature = selfFeature;

                                                bool isContains = false;
                                                isContains = polygonContainPolygon(other.Features[iotherFeature], selfFeature);
                                                if (isContains)
                                                {
                                                    intersactFeature = selfFeature;
                                                }
                                                else
                                                    intersactFeature = null;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                            if (intersactFeature != null && intersactFeature.Geometry != null)
                            {
                                try
                                {
                                    double dArea = 0;
                                    try
                                    {
                                        dArea = intersactFeature.Geometry.Area;
                                    }
                                    catch
                                    {
                                        if (selfFeature.Geometry.IsWithinDistance(other.Features[iotherFeature].Geometry, 0.00001))
                                        {
                                            if (selfFeature.Geometry.Area > other.Features[iotherFeature].Geometry.Area)
                                            {
                                                bool isContains = false;
                                                isContains = polygonContainPolygon(selfFeature, other.Features[iotherFeature]);
                                                if (isContains)
                                                {
                                                    intersactFeature = other.Features[iotherFeature];
                                                    dArea = intersactFeature.Geometry.Area;
                                                }
                                                else
                                                    dArea = 0;

                                            }
                                            else if (selfFeature.Geometry.Area < other.Features[iotherFeature].Geometry.Area)
                                            {
                                                intersactFeature = selfFeature;
                                                dArea = intersactFeature.Geometry.Area;

                                                bool isContains = false;
                                                isContains = polygonContainPolygon(other.Features[iotherFeature], selfFeature);
                                                if (isContains)
                                                {
                                                    intersactFeature = selfFeature;
                                                    dArea = intersactFeature.Geometry.Area;
                                                }
                                                else
                                                    dArea = 0;
                                            }
                                            else
                                                dArea = 0;

                                        }
                                        else
                                            dArea = 0;
                                    }
                                    if (dArea > 0)
                                    {

                                        if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                                        {
                                            dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], dArea / other.Features[iotherFeature].Geometry.Area);
                                        }
                                        else
                                        {
                                            dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                                                new Dictionary<string, double>());
                                            dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                               (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], dArea / other.Features[iotherFeature].Geometry.Area);
                                        }
                                    }
                                }
                                catch
                                {
                                    try
                                    {
                                        if (selfFeature.Geometry.IsWithinDistance(other.Features[iotherFeature].Geometry, 0.00001))
                                        {
                                            if (selfFeature.Geometry.Area > other.Features[iotherFeature].Geometry.Area)
                                                intersactFeature = other.Features[iotherFeature];
                                            else
                                                intersactFeature = selfFeature;
                                        }
                                        if (intersactFeature.Geometry.Area > 0)
                                        {

                                            if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                                            {
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                    (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area / other.Features[iotherFeature].Geometry.Area);
                                            }
                                            else
                                            {
                                                dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                                                    new Dictionary<string, double>());
                                                dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                   (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersactFeature.Geometry.Area / other.Features[iotherFeature].Geometry.Area);
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
                                if (k.Value.First().Value > 0.000000000001)
                                    result.Add(gr);

                            }
                            else
                            {
                                double d = 0.0;
                                foreach (KeyValuePair<string, double> kin in k.Value)
                                {
                                    if (kin.Value > 0.000000000001) d = d + kin.Value;
                                }
                                foreach (KeyValuePair<string, double> kin in k.Value)
                                {
                                    if (kin.Value < 0.000000000001) continue;
                                    string[] strin = kin.Key.Split(new char[] { ',' });

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

        public static Dictionary<string, double> IntersectionsWithGeographicArea(int gridDefId, int geoAreaId, string geoAreaFeatureId)
        {

            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            IFeatureSet gridDefFeatureSet = new FeatureSet();
            IFeatureSet geoAreaFeatureSet = new FeatureSet();
            BenMAPGrid bigBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(gridDefId == 20 ? 18 : gridDefId);

            // Get the grid definition associated with the geographic area
            string sqlGetGridDef = string.Format("select griddefinitionid, GeographicAreaFeatureIdField from geographicareas where geographicareaid ={0}", geoAreaId);
            System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, sqlGetGridDef);
            DataRow dr = ds.Tables[0].Rows[0];

            int iGeoAreaGridDef = Convert.ToInt32(dr["griddefinitionid"]);
            string geoAreaFeatureIdField = dr["GeographicAreaFeatureIdField"].ToString();

            BenMAPGrid geoBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iGeoAreaGridDef);

            string bigShapefileName = "";
            string geoShapefileName = "";
            if (bigBenMAPGrid as ShapefileGrid != null)
            { bigShapefileName = (bigBenMAPGrid as ShapefileGrid).ShapefileName;
            }
            else
            { bigShapefileName = (bigBenMAPGrid as RegularGrid).ShapefileName;
            }
            if (geoBenMAPGrid as ShapefileGrid != null)
            { geoShapefileName = (geoBenMAPGrid as ShapefileGrid).ShapefileName;
            }
            else
            { geoShapefileName = (geoBenMAPGrid as RegularGrid).ShapefileName;
            }
            string finsSetupname = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", gridDefId);
            string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, finsSetupname));
            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + bigShapefileName + ".shp"))
            {
                string shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + bigShapefileName + ".shp";
                //for debugging!
                gridDefFeatureSet = DotSpatial.Data.FeatureSet.Open(shapeFileName);
                string shapeFileNameSmall = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + geoShapefileName + ".shp";
                geoAreaFeatureSet = DotSpatial.Data.FeatureSet.Open(shapeFileNameSmall);
            }
            else
            {
                return null;
            }

            Dictionary<string, double> dicGeoAreaPercentages = new Dictionary<string, double>();
            try
            {
                if (!gridDefFeatureSet.AttributesPopulated) gridDefFeatureSet.FillAttributes();
                if (!geoAreaFeatureSet.AttributesPopulated) geoAreaFeatureSet.FillAttributes();
                int i = 0;


                //ensure consistent GIS projections
                //check for setup projection
                ProjectionInfo projInfo = null;
                if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                {
                    projInfo = CommonClass.getProjectionInfoFromName(CommonClass.MainSetup.SetupProjection);
                }
                if (projInfo == null) //if no setup projection, use default of WGS1984
                {
                    projInfo = KnownCoordinateSystems.Geographic.World.WGS1984;
                }

                if (geoBenMAPGrid.TType == GridTypeEnum.Shapefile)
                {
                    gridDefFeatureSet.Reproject(projInfo);
                    geoAreaFeatureSet.Reproject(projInfo);
                }

                IFeatureSet geoArea = null;

                // Are we looking at the entire grid definition, or a single feature?
                if(string.IsNullOrEmpty(geoAreaFeatureId) )
                {
                    geoArea = geoAreaFeatureSet.UnionShapes(ShapeRelateType.All);
                } else
                {
                    List<IFeature> featureList = geoAreaFeatureSet.SelectByAttribute(geoAreaFeatureIdField + " = '" + geoAreaFeatureId + "'");
                    FeatureSet fs = new FeatureSet(featureList);
                    geoArea = fs.UnionShapes(ShapeRelateType.All);
                }
                IGeometry geoAreaGeometry = geoArea.Features[0].Geometry;

                List<int> potentialCells = gridDefFeatureSet.SelectIndices(geoAreaGeometry.EnvelopeInternal.ToExtent());

                System.Console.WriteLine("Start: " + geoShapefileName);
                foreach (int iotherFeature in potentialCells)
                {
                    IFeature gridFeature = gridDefFeatureSet.GetFeature(iotherFeature);

                    IFeature geoAreaIntersection = gridFeature.Intersection(geoAreaGeometry);
                    System.Console.WriteLine("Testing: " + gridFeature.DataRow["Col"] + "," + gridFeature.DataRow["Row"]);
                    if (geoAreaIntersection != null)
                    {
                        double intersectionArea = geoAreaIntersection.Geometry.Area;
                        double gridFeatureArea = gridFeature.Geometry.Area;
                        string gridFeatureKey = gridFeature.DataRow["Col"] + "," + gridFeature.DataRow["Row"];
                        if (geoAreaIntersection.Geometry.Area > 0)
                        {
                            dicGeoAreaPercentages.Add(gridFeatureKey, intersectionArea / gridFeatureArea);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            System.Console.WriteLine("Finish: " + geoShapefileName);
            return dicGeoAreaPercentages;
  
        }

        public static Dictionary<String, Dictionary<int, double>> otherXrefCache = new Dictionary<string, Dictionary<int, double>>();
        public static List<GridRelationshipAttributePercentage> IntersectionPercentagePopulation(IFeatureSet self, IFeatureSet other, FieldJoinType joinType, String popRasterLoc)
        {
            
            //CommonClass.Debug = true;
            IRaster myRS=null;
            ArrayList lines = new ArrayList();
            string gdalWarpEXELoc = (new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location)).Directory.ToString();
            gdalWarpEXELoc += @"\GDAL-EXE\gdalwarp.exe";
            String tempRasterLocDir = CommonClass.DataFilePath + @"\Tmp";
            String tempRasterFullPath = null;
            String tempShapeFullPath = null;
            ProcessStartInfo warpStep = new System.Diagnostics.ProcessStartInfo();

            double minx = 0.0;
            double maxy = 0.0;
            double maxx = 0.0;
            double miny = 0.0;
            Boolean doSingleColumnClip=false;

            List<GridRelationshipAttributePercentage> result = new List<GridRelationshipAttributePercentage>();
            try
            {
                if (joinType == FieldJoinType.All)
                {
                    if (!self.AttributesPopulated) self.FillAttributes();
                    if (!other.AttributesPopulated) other.FillAttributes();
                    int i = 0;
                    Dictionary<string, Dictionary<string, double>> dicRelation = new Dictionary<string, Dictionary<string, double>>();
                    IFeatureSet ifs = new FeatureSet();
                    //self.SaveAs(@"P:\temp\self.shp",true);
                    //other.SaveAs(@"P:\temp\other.shp", true);
                    //Console.WriteLine("Starting loop");


                    //ensure consistent GIS projections
                    //check for setup projection
                    ProjectionInfo projInfo = null;
                    if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                    {
                        projInfo = CommonClass.getProjectionInfoFromName(CommonClass.MainSetup.SetupProjection);
                    }
                    if (projInfo == null) //if no setup projection, use default of WGS1984
                    {
                        projInfo = KnownCoordinateSystems.Geographic.World.WGS1984;
                    }

                    self.Reproject(projInfo);
                    //self.SaveAs(@"P:\temp\selfReProject.shp", true);
                    ifs.Projection = self.Projection;                        
                    //Console.WriteLine("Self proj4: " + self.Projection.ToProj4String());
                    //other.Reproject(myRS.Projection);
                    other.Reproject(projInfo);
                    
                    
                    Dictionary<int,double> otherXRef=new Dictionary<int,double>();

                    //CommonClass.Debug = true;
                    if (other.Filename!=null && otherXrefCache.ContainsKey(other.Filename))
                    {
                        Console.WriteLine("Using cached value for "+other.Filename);
                        otherXRef = otherXrefCache[other.Filename];
                        lines.Add("File," + other.Filename+",cached");
                        lines.Add("PopVal,fid");

                        foreach(int iDx in otherXRef.Keys){
                            
                            lines.Add(iDx + "," + otherXRef[iDx]);
                        }
                    }
                    else
                    {
                         //other.SaveAs(@"P:\temp\otherReProject.shp", true);

                        //Console.WriteLine("Other proj4: " + other.Projection.ToProj4String());
                        //Console.WriteLine("raster proj4: " + myRS.Projection.ToProj4String());
                        Console.WriteLine("Starting other cache at " + GetTimestamp(DateTime.Now));
                        
                        lines.Add("File," + other.Filename);
                        lines.Add("PopVal,fid");

                        //int polyToDebug = 1790;
                        foreach (IFeature otherFeature in other.Features)
                        {
                            //if (otherFeature.Fid == polyToDebug)
                            //{
                            //    CommonClass.Debug = true;
                            //}
                            //else
                            //{
                            //    continue;
                            //}
                            //make a much smaller one
                            minx = otherFeature.Geometry.EnvelopeInternal.Minimum.X - 100.0;
                            maxy = otherFeature.Geometry.EnvelopeInternal.Maximum.Y + 100.0;
                            maxx = otherFeature.Geometry.EnvelopeInternal.Maximum.X + 100.0;
                            miny = otherFeature.Geometry.EnvelopeInternal.Minimum.Y - 100.0;

                            warpStep = new System.Diagnostics.ProcessStartInfo();

                            warpStep.FileName = gdalWarpEXELoc;
                            tempRasterFullPath = Path.Combine(tempRasterLocDir, "clippedRaster-" + otherFeature.Fid + "-" + Guid.NewGuid().ToString() + ".tif");
                            warpStep.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            warpStep.UseShellExecute = false;
                            warpStep.CreateNoWindow = true;
                            //warpStep.Arguments = "-ot Float32 ";
                            warpStep.Arguments += " -te " + minx + " " + miny + " " + maxx + " " + maxy + " ";
                            //warpStep.Arguments += "-t_srs \"+proj=merc +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378137 +b=6378137 +units=m +no_defs \" ";
                            warpStep.Arguments += "\"" + popRasterLoc + "\" \"" + tempRasterFullPath + "\"";
                            try
                            {
                                // Start the process with the info we specified.
                                // Call WaitForExit and then the using statement will close.
                                using (Process exeProcess = Process.Start(warpStep))
                                {
                                    exeProcess.WaitForExit();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Couldn't output: " + e.ToString());
                            }
                            FileInfo fiTemp = new FileInfo(tempRasterFullPath);
                            FileInfo fiOrig = new FileInfo(popRasterLoc);
                            if (fiTemp.Length < fiOrig.Length*2)
                            {
                                //clip made a smaller one use that
                                doSingleColumnClip = false;
                                myRS = Raster.Open(tempRasterFullPath);
                            }
                            else
                            {
                                doSingleColumnClip = true;
                                //no need to reopen over and over again if we keep using same source raster
                                if (myRS == null)
                                {
                                    myRS = Raster.Open(popRasterLoc);
                                }

                            }
                            //myRS = Raster.Open(tempRasterFullPath);
                            //myRS = Raster.Open(@"P:\Projects\BenMAP\Code\Git-BB\BenMAP\bin\Release\Data\PopulationRaster\PopUS_90mX10_int16uWz4.tif");
                            double popValForOtherShape = ClipRasterRTI.ClipRasterWithPolygon(otherFeature, myRS, null, doSingleColumnClip, null);
                            //Console.WriteLine("Cacheing "+popValForOtherShape+" for "+otherFeature.Fid+ " out of "+other.Features.Count+ " at " +GetTimestamp(DateTime.Now));
                            lines.Add(popValForOtherShape + "," + otherFeature.Fid);
                            otherXRef.Add(otherFeature.Fid, popValForOtherShape);
                            try
                            {
                                myRS.Close();
                            }catch(Exception e){
                                Console.WriteLine("Error closing file: "+myRS.Filename+", reason: "+e.ToString());
                            }
                            myRS=null;
                            Boolean deleted = false;
                            int counter = 5;
                            while (!deleted && counter > 0)
                            {
                                try
                                {
                                    if (File.Exists(tempRasterFullPath))
                                    {
                                        File.Delete(tempRasterFullPath);
                                    }

                                    if (File.Exists(tempRasterFullPath + ".aux.xml"))
                                    {
                                        File.Delete(tempRasterFullPath + ".aux.xml");
                                    }
                                    deleted = true;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Could not delete temp raster, waiting 1 second and trying again, " + counter + " attemps left.");
                                    System.Threading.Thread.Sleep(1000);
                                    counter--;
                                }
                            }

                            if (counter == 0)
                            {
                                Console.WriteLine("Could not delete temp raster " + tempRasterFullPath);
                            }
                            //if (otherFeature.Fid % 100 == 0)
                            // {
                            //     Console.WriteLine(otherFeature.Fid + " done of " +other.Features.Count);
                            // }
                            //CommonClass.Debug = false;
                        }
                    }

                    //System.IO.File.WriteAllLines(@"p:\temp\otherPopCounts."+ Guid.NewGuid().ToString()+".csv", (String[])lines.ToArray(typeof(string)));

                    lines.Clear();
                    lines.Add("File," + self.Filename);
                    lines.Add("Population,SelfFID,OtherFid");
                    //CommonClass.Debug = true;
                    foreach (IFeature selfFeature in self.Features)
                    {



                        //Console.WriteLine("CurFeature: " + selfFeature.Fid);
                        ifs.Features.Clear();
                        ifs.AddFeature(selfFeature.Geometry);
                        //Console.WriteLine("My new fid: " + selfFeature.Fid);
                        //ifs.SaveAs(@"P:\temp\singleShape-"+selfFeature.Fid+".shp", true);
                        
                        //ifs.Reproject(myRS.Projection);
                        if (CommonClass.Debug)
                        {
                            ifs.SaveAs(@"P:\temp\singleShapeReproj-" + selfFeature.Fid + ".shp", true);
                        }


                        List<int> potentialOthers = other.SelectIndices(selfFeature.Geometry.EnvelopeInternal.ToExtent());
                        
                        foreach (int iotherFeature in potentialOthers)
                        {
                           // if (iotherFeature == 33)
                           // {
                           // }
                            IFeature intersectFeature = null;
                            double popVal = 0.0;

                            //if ((other.Features.Count < 5 || self.Features.Count < 5) 
                            //            && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum)) == 0 
                            //            && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum.X, selfFeature.Envelope.Minimum.Y)) == 0 
                            //            && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum)) == 0 
                            //            && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum.X, selfFeature.Envelope.Maximum.Y)) == 0)
                            //{
                            //    intersectFeature = selfFeature;
                            //} else if ((other.Features.Count < 5 || self.Features.Count < 5) 
                            //            && selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum)) == 0 
                            //            && selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum.X, other.Features[iotherFeature].Envelope.Minimum.Y)) == 0 
                            //            && selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum)) == 0 
                            //            && selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum.X, other.Features[iotherFeature].Envelope.Maximum.Y)) == 0)
                           // {
                            //    intersectFeature = other.Features[iotherFeature];
                           // } else
                           // {
                                try
                                {
                                    Boolean goodIntersection=false;
                                    int count = 1;
                                    int maxTries = 15;
                                    Random rand = new Random();
                                    double valToTry = 0.0; // .00001;
                                    while(!goodIntersection && count<maxTries)
                                        try{
                                            intersectFeature = selfFeature.Buffer(valToTry).Intersection(other.Features[iotherFeature].Geometry);
                                            goodIntersection = true;
                                        }catch(Exception e){
                                            Console.WriteLine("Could not do intersection on try "+count+" of "+maxTries+", reason: "+e.ToString());
                                            count++;
                                            if (count % 2 == 0)
                                            {
                                                valToTry = rand.NextDouble() * (double)count / 10.0;
                                            }
                                            else
                                            {
                                                valToTry = rand.NextDouble() / (double)count;
                                            }                               

                                    }
                                    
                                    if (CommonClass.Debug)
                                    {
                                        IFeatureSet ifsTemp = new FeatureSet();
                                        ifsTemp.Projection = other.Projection;
                                        //ifsTemp.AddFeature(selfFeature);
                                        //ifsTemp.AddFeature(other.Features[iotherFeature]);
                                        if (intersectFeature != null)
                                        {
                                            ifsTemp.AddFeature(intersectFeature.Geometry);
                                            if (CommonClass.Debug)
                                            {
                                                try{
                                                    ifsTemp.SaveAs(@"p:\temp\interSectFeat-" + selfFeature.Fid + "-" + iotherFeature + ".shp", true);
                                                }catch(Exception e){
                                                    Console.WriteLine("could not save intersectFeat debug: "+e.ToString());
                                                    try
                                                    {
                                                        ifsTemp.Features.Clear();
                                                        ifsTemp.AddFeature(selfFeature.Geometry);
                                                        ifsTemp.AddFeature(other.Features[iotherFeature].Geometry);
                                                        ifsTemp.AddFeature(intersectFeature.Geometry);
                                                        ifsTemp.SaveAs(@"p:\temp\interSectFeat-" + selfFeature.Fid + "-" + iotherFeature + "-T2.shp", true);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("could not save intersectFeat 2 debug: " + ex.ToString());
                                                    }

                                                }
                                            }
                                        }

                                       
                                    }
                                    //fist population of selfFeature\ warpStep.FileName = gdalWarpEXELoc;
                                    warpStep = new System.Diagnostics.ProcessStartInfo();
                                    //make a much smaller one
                                    if (intersectFeature == null || intersectFeature.Geometry.EnvelopeInternal == null)
                                    {
                                        //this case is when shape is in bounding box, but not actually overlapping.
                                        continue;
                                    }
                                    //this object is needed to set up structure for calculating edges
                                    IFeatureSet ifsHold = new FeatureSet();
                                    ifsHold.Projection = other.Projection;
                                    //this intersectFeature may have multiple geometries
                                    if (intersectFeature.Geometry.NumGeometries > 1)
                                    {
                                        for (int idx = 0; idx < intersectFeature.Geometry.NumGeometries; idx++)
                                        {

                                            var ibm = intersectFeature.Geometry.GetGeometryN(idx);
                                            if (ibm.GeometryType == "Polygon")
                                            {
                                                ifsHold.AddFeature(ibm);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ifsHold.AddFeature(intersectFeature.Geometry);
                                    }
                                    if (ifsHold.Features.Count < 1)
                                    {
                                        //if all we got from intersection was ponts/lines, don't do any more processing.
                                        continue;
                                    }
                                    tempShapeFullPath = Path.Combine(tempRasterLocDir, "clippedShape-" + selfFeature.Fid + "-" + iotherFeature + "-" + Guid.NewGuid().ToString() + ".shp");
                                    try
                                    {
                                        ifsHold.SaveAs(tempShapeFullPath, true);
                                        ifsHold.Close();
                                        ifsHold = null;
                                        intersectFeature = null;
                                        ifsHold = FeatureSet.Open(tempShapeFullPath);
                                        //intersectFeature = ifsHold.Features[0];
                                    }
                                    catch (Exception e)
                                    {
                                        //can't save poly?
                                        Console.WriteLine("Can't save poly? "+e.ToString());
                                        continue;
                                    }

                                    minx = ifsHold.Extent.ToEnvelope().Minimum.X - 100.0;
                                    maxy = ifsHold.Extent.ToEnvelope().Maximum.Y + 100.0;
                                    maxx = ifsHold.Extent.ToEnvelope().Maximum.X + 100.0;
                                    miny = ifsHold.Extent.ToEnvelope().Minimum.Y - 100.0;
                                    tempRasterFullPath = Path.Combine(tempRasterLocDir, "clippedRaster-Intersection-"+selfFeature.Fid+"-"+iotherFeature+ "-"+Guid.NewGuid().ToString()+".tif");
                                    warpStep.FileName = gdalWarpEXELoc;
                                    warpStep.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                    warpStep.UseShellExecute = false;
                                    warpStep.CreateNoWindow = true;
                                    //warpStep.Arguments = "-ot Float32 ";
                                    warpStep.Arguments += " -te " + minx + " " + miny + " " + maxx + " " + maxy + " ";
                                    //warpStep.Arguments += "-t_srs \"+proj=merc +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378137 +b=6378137 +units=m +no_defs \" ";
                                    warpStep.Arguments += "\"" + popRasterLoc + "\" \"" + tempRasterFullPath + "\"";
                                    try
                                    {
                                        // Start the process with the info we specified.
                                        // Call WaitForExit and then the using statement will close.
                                        using (Process exeProcess = Process.Start(warpStep))
                                        {
                                            exeProcess.WaitForExit();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Couldn't output: " + e.ToString());
                                    }
                                    myRS = Raster.Open(tempRasterFullPath);

                                    for (int curFS = 0; curFS < ifsHold.Features.Count; curFS++)
                                    {
                                        popVal += ClipRasterRTI.ClipRasterWithPolygon(ifsHold.Features[curFS], myRS, null, false, null);
                                    }
                                    //Console.WriteLine("Got population: " + popVal);
                                    if (popVal > 0)
                                    {
                                        Console.WriteLine("NonZero val on -" + selfFeature.Fid + "-" + iotherFeature);
                                        lines.Add(popVal+","+ selfFeature.Fid+","+ iotherFeature);
                                    }
                                    try
                                    {
                                        myRS.Close();
                                        ifsHold.Close();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Could not close raster/shape: "+e.ToString());
                                    }
                                    Boolean deleted = false;
                                    int counter = 5;
                                    while (!deleted && counter>0)
                                    {
                                        try
                                        {

                                            string foldPath = Path.GetDirectoryName(tempShapeFullPath);
                                            string contains = Path.GetFileNameWithoutExtension(tempShapeFullPath)+".*";
                                            string[] files = Directory.GetFiles(foldPath,contains);
                                            foreach(string file in files)
                                            {
                                            //    System.Diagnostics.Debug.WriteLine(file + "will be deleted");
                                                if (file.ToUpper().Contains("TMP"))
                                                {
                                                    System.IO.File.Delete(file);
                                                }
                                            }

                                            if (File.Exists(tempRasterFullPath) && tempRasterFullPath.ToUpper().Contains("TMP"))
                                            {
                                                File.Delete(tempRasterFullPath);
                                            }

                                            if (File.Exists(tempRasterFullPath + ".aux.xml") && tempRasterFullPath.ToUpper().Contains("TMP"))
                                            {
                                                File.Delete(tempRasterFullPath + ".aux.xml");
                                            }
                                            deleted = true;
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Could not delete temp raster, waiting 1 second and trying again, " + counter + " attemps left.");
                                            System.Threading.Thread.Sleep(1000);
                                            counter--;
                                        }
                                    }

                                    if (counter == 0)
                                    {
                                        Console.WriteLine("Could not delete temp raster " + tempRasterFullPath);
                                    }
                                    
                                    //myRS.Close();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Caught an error getting counts: " + ex.ToString());
                                    //try
                                   // {
                                   //     if (selfFeature.IsWithinDistance(other.Features[iotherFeature], 0.00001))
                                   //     {
                                   //         if (selfFeature.Area() > other.Features[iotherFeature].Area())
                                   //         {
                                   //             bool isContains = false;
                                   //             isContains = polygonContainPolygon(selfFeature, other.Features[iotherFeature]);
                                  //              if (isContains)
                                  //              {
                                  //                  intersectFeature = other.Features[iotherFeature];
                                  //              }
                                  //              else
                                  //              {
                                  //                  intersectFeature = null;
                                  //              }
                                  //          }
                                  //          else if (selfFeature.Area() < other.Features[iotherFeature].Area())
                                  //          {
                                  //              intersectFeature = selfFeature;

                                  //              bool isContains = false;
                                  //              isContains = polygonContainPolygon(other.Features[iotherFeature], selfFeature);
                                  //              if (isContains)
                                  //              {
                                  //                  intersectFeature = selfFeature;
                                  //              }
                                  //              else
                                  //              {
                                   //                 intersectFeature = null;
                                  //              }
                                   //         }
                                    //    }
                                   // }
                                    //catch(Exception e)
                                   // {
                                  //      Console.WriteLine("Error finding matching features: " + e.ToString());
                                    //}
                                }

                            //}
                           // if (intersectFeature != null && intersectFeature.BasicGeometry != null)
                           // {
                                
                               // try
                               // {
                                    /*
                                    double dArea = 0;
                                    try
                                    {
                                        dArea = intersectFeature.Area();
                                    }
                                    catch
                                    {
                                        if (selfFeature.IsWithinDistance(other.Features[iotherFeature], 0.00001))
                                        {
                                            if (selfFeature.Area() > other.Features[iotherFeature].Area())
                                            {
                                                bool isContains = false;
                                                isContains = polygonContainPolygon(selfFeature, other.Features[iotherFeature]);
                                                if (isContains)
                                                {
                                                    intersectFeature = other.Features[iotherFeature];
                                                    dArea = intersectFeature.Area();
                                                }
                                                else
                                                    dArea = 0;

                                            }
                                            else if (selfFeature.Area() < other.Features[iotherFeature].Area())
                                            {
                                                intersectFeature = selfFeature;
                                                dArea = intersectFeature.Area();

                                                bool isContains = false;
                                                isContains = polygonContainPolygon(other.Features[iotherFeature], selfFeature);
                                                if (isContains)
                                                {
                                                    intersectFeature = selfFeature;
                                                    dArea = intersectFeature.Area();
                                                }
                                                else
                                                    dArea = 0;
                                            }
                                            else
                                                dArea = 0;

                                        }
                                        else
                                            dArea = 0;
                                    }*/
                                    if (popVal > 0)
                                    {
                                        Console.WriteLine("Got a pop of " + popVal + " for " + other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]);
                                        if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                                        {
                                            dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                                (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], popVal / otherXRef[other.Features[iotherFeature].Fid]);
                                        }
                                        else
                                        {
                                            dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                                                new Dictionary<string, double>());
                                            dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                                               (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], popVal / otherXRef[other.Features[iotherFeature].Fid]);
                                        }
                                    }
                               // }
                               // catch
                               // {
                               //     try
                                //    {
                               //         if (selfFeature.IsWithinDistance(other.Features[iotherFeature], 0.00001))
                               //         {
                              //              if (selfFeature.Area() > other.Features[iotherFeature].Area())
                              //                  intersectFeature = other.Features[iotherFeature];
                              //              else
                              //                  intersectFeature = selfFeature;
                              //          }
                               //         if (intersectFeature.Area() > 0)
                               //         {
                            //
                              //              if (dicRelation.ContainsKey(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]))
                              //              {
                              //                  dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                              //                      (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersectFeature.Area() / other.Features[iotherFeature].Area());
                              //              }
                              ////              else
                               //             {
                               //                 dicRelation.Add(other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"],
                              //                      new Dictionary<string, double>());
                               //                 dicRelation[other.Features[iotherFeature].DataRow["Col"] + "," + other.Features[iotherFeature].DataRow["Row"]].Add
                               //                    (selfFeature.DataRow["Col"] + "," + selfFeature.DataRow["Row"], intersectFeature.Area() / other.Features[iotherFeature].Area());
                               //             }
                               //         }
                               //     }
                               //     catch
                               //     {
                               //     }
                                ////}
                            //}
                        }

                        i++;
                        if (selfFeature.Fid % 100 == 0)
                        {
                            Console.WriteLine(selfFeature.Fid + " done of " + self.Features.Count);
                        }
                    }
                    //System.IO.File.WriteAllLines(@"p:\temp\otherSelfIntersectPopCounts."+ Guid.NewGuid().ToString()+".csv", (String[])lines.ToArray(typeof(string)));
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
                                if (k.Value.First().Value > 0.000000000001)
                                {
                                    result.Add(gr);
                                }

                            }
                            else
                            {
                                double d = 0.0;
                                foreach (KeyValuePair<string, double> kin in k.Value)
                                {
                                    if (kin.Value > 0.000000000001)
                                    {
                                        d = d + kin.Value;
                                    }
                                }
                                foreach (KeyValuePair<string, double> kin in k.Value)
                                {
                                    if (kin.Value < 0.000000000001) continue;
                                    string[] strin = kin.Key.Split(new char[] { ',' });

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
                Console.WriteLine("Exception doing calcs: " + ex.ToString());
            }
            return result;
        }

        

        public static bool polygonContainPolygon(IFeature big, IFeature small)
        {
            try
            {
                for (int i = 0; i < small.Geometry.Coordinates.Length; i++)
                {
                    if (!big.Geometry.IsWithinDistance(new Point(small.Geometry.Coordinates[i]), 0.00001))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void getRelationshipFromBenMAPGridPercentage(int big, int small, String popRasterFileLoc)
        {
            //dpa 1/29/2017 note that the popRasterFileLoc is never used... 
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

                    commandText = string.Format("insert into GridDefinitionPercentages(PERCENTAGEID,SOURCEGRIDDEFINITIONID, TARGETGRIDDEFINITIONID) "
                        + "values({0},{1},{2})", iMax, small, big);
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
                Console.WriteLine("Error in getRelationshipFromBenMAPGridPercentage: "+ex.ToString());
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
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6) && (Environment.OSVersion.Version.Minor >= 1);
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

        public static System.Data.DataTable ExcelToDataTable(string filenameurl, string tabname = null)
        {
            //This function is created so that tabnameref works as an optional reference parameter. 
            //We want this parameter optional so that adding tabnameref won't break any existing codes.
            string dummyref = string.Empty;
            return ExcelToDataTable(filenameurl, ref dummyref, tabname);
        }

        public static System.Data.DataTable ExcelToDataTable(string filenameurl, ref string tabnameref, string tabname = null)
        {
            try
            {
                if (filenameurl.Substring(filenameurl.Length - 3, 3).ToLower() == "csv")
                {
                    tabnameref = string.Empty;
                    return DataSourceCommonClass.getDataTableFromCSV(filenameurl);
                }


                FileStream stream = File.Open(filenameurl, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelReader = filenameurl.ToLower().EndsWith("xls")
                                           ? ExcelReaderFactory.CreateBinaryReader(stream)
                                           : ExcelReaderFactory.CreateOpenXmlReader(stream);
                excelReader.IsFirstRowAsColumnNames = true;
                bool isBatch = false;
                System.Data.DataSet ds = excelReader.AsDataSet();

                if (ds.Tables.Count == 1)
                {
                    tabnameref = string.Empty;
                    return ds.Tables[0];
                }

                if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    isBatch = true;
                }

                if (String.IsNullOrWhiteSpace(tabname))
                {
                    if(isBatch) //return the first tab
                    {
                        tabnameref = string.Empty;
                        return ds.Tables[0];
                    }
                    else // popup selection window
                    {
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
                        tabnameref = ds.Tables[Index].TableName;
                        return ds.Tables[Index];
                    }
                }
                else // for both batch and UI
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        if (ds.Tables[i].TableName == tabname)
                        {
                            tabnameref = tabname;
                            return ds.Tables[i];
                        }
                    }

                    //If tab name doesn't exist in excel
                    tabnameref = string.Empty;
                    return ds.Tables[0]; 
                }
            }
            catch
            {
                tabnameref = string.Empty;
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
                    benMAPProject.BaseControlCRSelectFunctionCalculateValue = BaseControlCRSelectFunctionCalculateValue;
                }
                else if (BaseControlCRSelectFunction != null)
                {
                    benMAPProject.BaseControlCRSelectFunction = BaseControlCRSelectFunction;
                }
                else
                {
                    benMAPProject.ManageSetup = ManageSetup;
                    benMAPProject.MainSetup = MainSetup; benMAPProject.LstPollutant = LstPollutant; benMAPProject.RBenMAPGrid = RBenMAPGrid;
                    benMAPProject.GBenMAPGrid = GBenMAPGrid; benMAPProject.LstBaseControlGroup = LstBaseControlGroup; benMAPProject.CRThreshold = CRThreshold; benMAPProject.CRLatinHypercubePoints = CRLatinHypercubePoints; benMAPProject.CRRunInPointMode = CRRunInPointMode; benMAPProject.CRDefaultMonteCarloIterations= CRDefaultMonteCarloIterations;  benMAPProject.CRSeeds = CRSeeds;
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

        public static ProjectionInfo getProjectionInfoFromName(string projName)
        {
            if (String.IsNullOrEmpty(projName))
            {
                return null;
            }

            projName = projName.Replace(" ", "");
            string [] name = projName.Split('-');

            return KnownCoordinateSystems.Projected.GetCategory(name[0]).GetProjection(name[1]);        
        }

        public static BenMAPSetup getBenMAPSetupFromID(int setupID)
        {
            string commandText = "select SetupID,SetupName,SetupProjection from Setups where  SetupID=" + setupID;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            DataRow dr = ds.Tables[0].Rows[0];
            BenMAPSetup benMAPSetup = new BenMAPSetup()
            {
                SetupID = Convert.ToInt32(dr["SetupID"]),
                SetupName = dr["SetupName"].ToString()
            };
            if (dr["SetupProjection"] != DBNull.Value)
            {
                benMAPSetup.SetupProjection = dr["SetupProjection"].ToString();
            }
            return benMAPSetup;
        }

        public static BenMAPSetup getBenMAPSetupFromName(string SetupName)
        {
            try
            {
                string commandText = "select SetupID,SetupName,SetupProjection from Setups where  SetupName='" + SetupName + "'";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                DataRow dr = ds.Tables[0].Rows[0];
                BenMAPSetup benMAPSetup = new BenMAPSetup()
                {
                    SetupID = Convert.ToInt32(dr["SetupID"]),
                    SetupName = dr["SetupName"].ToString()
                };
                if (dr["SetupProjection"] != DBNull.Value)
                {
                    benMAPSetup.SetupProjection = dr["SetupProjection"].ToString();
                }
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
                CommonClass.GBenMAPGrid = null; CommonClass.LstBaseControlGroup = null; CommonClass.CRThreshold = 0; CommonClass.CRLatinHypercubePoints = 20; CommonClass.CRRunInPointMode = false;
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
                    BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
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
                    BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                    BaseControlCRSelectFunction.CRThreshold = BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                    if (BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                    {
                        BaseControlCRSelectFunction.lstCRSelectFunction = BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Select(p => p.CRSelectFunction).ToList();
                    }

                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList();
                    RBenMAPGrid = BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType; LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup; CRThreshold = BaseControlCRSelectFunction.CRThreshold; CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints; CRDefaultMonteCarloIterations = BaseControlCRSelectFunction.CRDefaultMonteCarloIterations; CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode; CRSeeds = BaseControlCRSelectFunction.CRSeeds;
                    BenMAPPopulation = BaseControlCRSelectFunction.BenMAPPopulation;
                }
                else if (benMAPProject.BaseControlCRSelectFunction != null)
                {
                    BaseControlCRSelectFunction = benMAPProject.BaseControlCRSelectFunction;
                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID); LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList(); RBenMAPGrid = BaseControlCRSelectFunction.RBenMapGrid;
                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType; LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup; CRThreshold = BaseControlCRSelectFunction.CRThreshold; CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints; CRDefaultMonteCarloIterations = BaseControlCRSelectFunction.CRDefaultMonteCarloIterations; CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode; CRSeeds = BaseControlCRSelectFunction.CRSeeds;
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

        internal static void SetupOLVEmptyListOverlay(TextOverlay textOverlay)
        {
            textOverlay.TextColor = System.Drawing.Color.LightGray;
            textOverlay.BackColor = System.Drawing.Color.White;
            textOverlay.BorderWidth = 0.0f;
            textOverlay.Font = new System.Drawing.Font("Calibri", 16);
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
        [ProtoMember(19)]
        public int CRDefaultMonteCarloIterations = 10000;
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
        [ProtoMember(3)]
        public string SetupProjection;
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
        [ProtoMember(12)]
        public BenMAPSetup Setup;
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
    public class GeographicArea
    {
        [ProtoMember(1)]
        public int GridDefinitionID;
        [ProtoMember(2)]
        public int GeographicAreaID;
        [ProtoMember(3)]
        public List<RowCol> RowCols;
        [ProtoMember(4)]
        public string GeographicAreaName;
        [ProtoMember(5)]
        public string GeographicAreaFeatureIdField;
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
        public string GeographicAreaName;
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
        [ProtoMember(43)]
        public int GeographicAreaID;
        [ProtoMember(44)]
        public string GeographicAreaFeatureID;
        [ProtoMember(45)]
        public int CountStudies; //YY: Added Nov 2019.  
        [ProtoMember(46)]
        public string AgeRange; //YY: Added Nov 2019. 
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
        public string GeographicAreaName;
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
        [ProtoMember(16)]
        public int GeographicAreaID;
        [ProtoMember(17)]
        public string GeographicAreaFeatureID;
        [ProtoMember(18)]
        public int CountStudies; //YY: Added Nov 2019. Removed if not needed here. 
        [ProtoMember(19)]
        public string AgeRange; //YY: Added Nov 2019. Removed if not needed here.
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
        [ProtoMember(11)]
        public int CRDefaultMonteCarloIterations = 10000;
        [ProtoMember(12)]
        public BenMAPSetup Setup;
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
        [ProtoMember(12)]
        public int CRDefaultMonteCarloIterations = 10000;
        [ProtoMember(13)]
        public BenMAPSetup Setup;
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
        [ProtoMember(32)]
        public string GeographicArea;
        [ProtoMember(33)]
        public string GeographicAreaFeatureId;
        [ProtoMember(34)]
        public int CountStudies; // Added Nov 2019
        [ProtoMember(35)]
        public string AgeRange; // Added Nov 2019
        [ProtoMember(36)]
        public int ChildCount; // added Nov 2019. Count of direct children.
        [ProtoMember(37)]
        public string Nickname; // Added Nov 2019
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
        [ProtoMember(31)]
        public string GeographicArea;
        [ProtoMember(32)]
        public string GeographicAreaFeatureId;
        [ProtoMember(33)]
        public int ChildCount; // Added Nov 2019. Count of direct children.
        [ProtoMember(34)]
        public int CountStudies; // Added Nov 2019
        [ProtoMember(35)]
        public string AgeRange; // Added Nov 2019
        [ProtoMember(36)]
        public string Nickname; // Added Nov 2019
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
        public int DefaultMonteCarloIterations = 10000;
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
        [ProtoMember(7)]
        public int PoolLevel; //YY: added Nov 2019
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
        public string ModelTablename; //Database table or Excel tab name
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