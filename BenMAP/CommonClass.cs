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

namespace BenMAP
{
    /// <summary>
    /// 对ConfigurationSettings节点下的每个子节点进行相应的权限控制
    /// </summary>
    public enum ConfigurationSettingType
    {
        /// <summary>
        /// Configuration 界面所有控件都是可操作
        /// </summary>
        Configuration = 0,
        /// <summary>
        /// Latin Hypercube Points 可操作点拉丁取样
        /// </summary>
        LatinHypercubePoints = 1,
        /// <summary>
        /// Population Dataset 选择人口数据集
        /// </summary>
        PopulationDataset = 2,
        /// <summary>
        /// IncidenceDataset 选择发病率数据集
        /// </summary>
        IncidenceDataset = 3,
        /// <summary>
        /// Health function 选择函数
        /// </summary>
        HealthFunction = 4
    }

    public class CommonClass
    {
        #region delete shp
        //判断shape是否存在,如果存在删除
        public static void DeleteShapeFileName(string FileName)
        {
            if (!File.Exists(FileName)) return;
            string temppath = System.IO.Path.GetDirectoryName(FileName);
            string ExtFileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
            DeleteFile(temppath, ExtFileName);
        }

        /// <summary>
        /// 按名称删除
        /// </summary>
        /// <param name="dirRoot"></param>
        /// <param name="deleteFileName"></param>
        public static void DeleteFile(string dirRoot, string deleteFileName)
        {
            try
            {
                //string[] rootDirs = Directory.GetDirectories(dirRoot); //当前目录的子目录：
                string[] rootFiles = Directory.GetFiles(dirRoot);        //当前目录下的文件：
                foreach (string s in rootFiles)
                {
                    string fname = System.IO.Path.GetFileNameWithoutExtension(s);
                    if (fname == deleteFileName)
                    {
                        File.Delete(s);                      //删除文件
                    }
                }
                //foreach (string s1 in rootDirs)
                //{
                //    DeleteFile(s1, deleteFileName);
                //}
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        #endregion

        #region ini
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name=^Section^>节点名称</param>
        /// <param name=^Key^>关键字</param>
        /// <param name=^Value^>值</param>
        /// <param name=^filepath^>INI文件路径</param>
        static public void IniWriteValue(string Section, string Key, string Value, string filepath)
        {
            WritePrivateProfileString(Section, Key, Value, filepath);
        }
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name=^Section^>节点名称</param>
        /// <param name=^Key^>关键字</param>
        /// <param name=^filepath^>INI文件路径</param>
        /// <returns>值</returns>
        static public string IniReadValue(string Section, string Key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, filepath);
            return temp.ToString();
        }
        #endregion

        #region 私有变量或属性
        private static string _dataFilePath = "";

        public static string DataFilePath
        {
            get {
                if (_dataFilePath == "")
                {
                    if (IsWindows7 || IsElse || IsWindowsVista)
                        _dataFilePath = Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + @"\BenMAP-CE";
                    else
                        _dataFilePath = Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + @"\Application Data\BenMAP-CE";
                }
                
                return CommonClass._dataFilePath; }
            set { CommonClass._dataFilePath = value; }
        }

        private static string _resultFilePath="";

        public static string ResultFilePath
        {
            get {
                if (_resultFilePath == "")
                {
                    _resultFilePath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files";
                }
                return CommonClass._resultFilePath; }
            set { CommonClass._resultFilePath = value; }
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

        /// <summary>
        /// 当前的设定：美国区/中国区
        /// </summary>
        //public static string ActiveSetup = "";

        private static string _activeSetup = "USA";

        /// <summary>
        /// 当前操作区域：默认是美国区
        /// </summary>
        public static string ActiveSetup
        {
            get { return _activeSetup; }
            set { _activeSetup = value; }
        }

        private static ConfigurationAtt _conAtt;

        /// <summary>
        /// 保存ConfigurationSettings中配置信息
        /// </summary>
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

        private static BenMAPSetup _mainSetup;// 当前活动区域
        public static BenMAPSetup MainSetup// 当前活动区域
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

        /// <summary>
        /// Firebird的链接字符串
        /// </summary>
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

        //private static List<RowCol> lstGBenMAPGridRowCol;

        //public static List<RowCol> LstGBenMAPGridRowCol
        //{
        //    get {
        //        int iCol = -1;
        //        int iRow = -1;
        //        if (CommonClass.lstGBenMAPGridRowCol == null &&CommonClass.GBenMAPGrid!=null)
        //        {
        //            CommonClass.lstGBenMAPGridRowCol = new List<RowCol>();
        //            DotSpatial.Data.FeatureSet fs = new DotSpatial.Data.FeatureSet();
        //            string strSHP="";
        //            if(CommonClass.GBenMAPGrid is ShapefileGrid)
        //            {
        //                strSHP=Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
        //            }
        //            else if(CommonClass.GBenMAPGrid is RegularGrid)
        //            {
        //                    strSHP=Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";

        //            }
        //            fs.Open(strSHP);
        //            int i = 0;
        //            foreach (DataColumn dc in fs.DataTable.Columns)
        //            {
        //                if (dc.ColumnName.ToLower() == "col")
        //                    iCol = i;
        //                if (dc.ColumnName.ToLower() == "row")
        //                    iRow = i;

        //                i++;
        //            }
        //            foreach (DataRow dr in fs.DataTable.Rows)
        //            {
        //                lstRowCol.Add(new RowCol() { Col = Convert.ToInt32(dr[iCol]), Row = Convert.ToInt32(dr[iRow]) });
        //            }

        //            fs.Close();
        //            fs.Dispose();
        //        }
        //        return CommonClass.lstGBenMAPGridRowCol;
        //    }
        //    //set { CommonClass.lstGBenMAPGridRowCol = value; }
        //}
        public static List<BenMAPPollutant> LstPollutant;//污染物列表
        public static BenMAPGrid rBenMAPGrid;
        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static void SaveCSV(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
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
        public static BenMAPGrid RBenMAPGrid//系统选择的Grid
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
            //调出Region-------求得DataSet----包括name/col/row
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
                    //dc.ColumnName = "Name";
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

        public static BenMAPGrid GBenMAPGrid;//系统选择的Region
        public static List<BaseControlGroup> LstBaseControlGroup;//系统设置的DataSource Base and Control
        //public static List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
        public static double CRThreshold = 0;//阈值
        public static int CRLatinHypercubePoints = 10;//拉丁立体方采样点数
        public static bool CRRunInPointMode = false;//是否使用拉丁立体方方采样
        public static int CRSeeds=1 ;//种子数
        public static BenMAPPopulation BenMAPPopulation;//系统设置的Population

        //public static List<GridRelationship> LstAllGridRelationship;//所有的网格之间的关系
        public static List<GridRelationship> LstCurrentGridRelationship;//当前网格和所有其他网格的关系
        /// <summary>
        /// 界面当前的操作状态
        /// </summary>
        public static string CurrentStat;
        /// <summary>
        /// 根据List判断节点是否异步，string 并成规则：pollutant+baseline/control例如：pm10baseline
        /// list中包含string说明异步还在进行中，否则是可以操作该节点
        /// </summary>
        public static List<string> LstAsynchronizationStates;//BaseControl异步状态

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
                    //   lstGridRelationshipAll = Grid.GridCommon.getAllGridRelationship();
                    List<GridRelationship> lstGridRelationship = new List<GridRelationship>(); // TODO: 初始化为适当的值
                    //string filePath = @"D:\软件项目\BenMap\BenMap\trunk\Code\BenMAP\bin\Debug\Data\GridRelationship\GridRelationship.gr"; // TODO: 初始化为适当的值
                   // string filePath = string.Format(@"{0}\Data\GridRelationship\GridRelationship.gr", Application.StartupPath);
                    //Grid.GridCommon.getGridRelationshipFromFile(filePath, ref lstGridRelationship);
                    //GRGridRelationShip gr = new GRGridRelationShip() { lstRelationship = lstGridRelationship };
                    //SaveGridRelationship(@"d:\GridRelationship.gr", gr);
                    lstGridRelationshipAll = new List<GridRelationship>();// LoadGridRelationship(filePath).lstRelationship;
                    //lstGridRelationshipAll = lstGridRelationship;
                    //----------------if percentage have but lstGridRelationshipAll not,then add new --------
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
                    //----------------------------------------------
                    //-----------修正中国区36和Region----------------------------------------------------
                    //-----------修正中国区36和Region----------------------------------------------------
                    //  getRelationshipFromBenMAPGridPercentage(21, 18);
                    #region 测试percentage 根据charlies 算法
                    //GridDefinition gd = new GridDefinition();
                    //List<GridRelationshipAttributePercentage> lst = gd.getRelationshipFromBenMAPGridPercentage(3, 1).ToArray()[0].Value;
                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("sourcecol");
                    //dt.Columns.Add("sourcerow");
                    //dt.Columns.Add("targetcol");
                    //dt.Columns.Add("targetrow");
                    //dt.Columns.Add("percentage");
                    //foreach (GridRelationshipAttributePercentage gr in lst)
                    //{

                    //    DataRow dr = dt.NewRow();
                    //    dr[0] = gr.sourceCol;
                    //    dr[1] = gr.sourceRow;
                    //    dr[2] = gr.targetCol;
                    //    dr[3] = gr.targetRow;
                    //    dr[4] = gr.percentage;
                    //    dt.Rows.Add(dr);
                    //}
                    //BenMAP benmap = new BenMAP();
                    //benmap.SaveCSV(dt, @"d:\percentage.csv");
                    # endregion
                    //  getRelationshipFromBenMAPGridPercentage(21, 18);
                    //GridRelationship grRemove=lstGridRelationshipAll.Where(p => p.bigGridID == 21 && p.smallGridID == 18).First();
                    //lstGridRelationshipAll.Remove(grRemove);
                    //lstGridRelationshipAll.Add(gr);
                    //Grid.GridCommon.createGridRelationshipFile(@"D:\GridRelationship.gr", lstGridRelationshipAll);
                    //-----------修正12km和county----------------------从PopulationGrowth得出-------------
                    //string commandText = "select distinct   Populationdatasetid,yyear,sourcecolumn,sourcerow,targetcolumn,targetrow from PopulationGrowthWeights";
                    //ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    //System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    //GridRelationship grs = new GridRelationship();
                    //GridRelationship grsclip = new GridRelationship();
                    //grs.bigGridID = 1;
                    //grs.smallGridID = 13;
                    //grsclip.bigGridID = 1;
                    //grsclip.smallGridID = 7;
                    //grs.lstGridRelationshipAttribute = new List<GridRelationshipAttribute>();
                    //grsclip.lstGridRelationshipAttribute = new List<GridRelationshipAttribute>();
                    //Dictionary<string, List<RowCol>> dicRelation = new Dictionary<string, List<RowCol>>();
                    //foreach (DataRow dr in dsGrid.Tables[0].Rows)
                    //{
                    //    if (!dicRelation.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                    //    {
                    //        dicRelation.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new List<RowCol>());
                    //        dicRelation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(new RowCol() { Col = Convert.ToInt32(dr["targetcolumn"]), Row = Convert.ToInt32(dr["targetrow"]) });
                    //    }
                    //    else
                    //    {
                    //        dicRelation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(new RowCol() { Col = Convert.ToInt32(dr["targetcolumn"]), Row = Convert.ToInt32(dr["targetrow"]) });
                    //    }

                    //}
                    //foreach (KeyValuePair<string, List<RowCol>> k in dicRelation)
                    //{
                    //    grs.lstGridRelationshipAttribute.Add(new GridRelationshipAttribute()
                    //    {
                    //         bigGridRowCol=new RowCol(){ Col=Convert.ToInt32( k.Key.Substring(0,k.Key.IndexOf(","))), Row= Convert.ToInt32( k.Key.Substring(k.Key.IndexOf(",")+1))},
                    //          smallGridRowCol=k.Value
                    //    });
                    //    grsclip.lstGridRelationshipAttribute.Add(new GridRelationshipAttribute()
                    //    {
                    //        bigGridRowCol = new RowCol() { Col = Convert.ToInt32(k.Key.Substring(0, k.Key.IndexOf(","))), Row = Convert.ToInt32(k.Key.Substring(k.Key.IndexOf(",") + 1)) },
                    //        smallGridRowCol = k.Value
                    //    });
                    //}
                    //GridRelationship remove = lstGridRelationship.Where(p => p.bigGridID == 1 && p.smallGridID == 13).First();
                    //lstGridRelationship.Remove(remove);
                    //remove = lstGridRelationship.Where(p => p.bigGridID == 1 && p.smallGridID == 7).First();
                    //lstGridRelationship.Remove(remove);
                    //lstGridRelationship.Add(grs);
                    //lstGridRelationship.Add(grsclip);
                    //Grid.GridCommon.createGridRelationshipFile(@"D:\GridRelationship.gr", lstGridRelationship);
                    //---------修正12km 36km--------
                    //GridRelationship grs = getRelationshipFromBenMAPGrid(1, 4);
                    //GridRelationship remove = lstGridRelationship.Where(p => p.bigGridID == 1 && p.smallGridID == 4).First();
                    //lstGridRelationship.Remove(remove);
                    //lstGridRelationship.Add(grs);

                    //  grs = getRelationshipFromBenMAPGrid(1, 13);
                    //  remove = lstGridRelationship.Where(p => p.bigGridID == 1 && p.smallGridID == 13).First();
                    //lstGridRelationship.Remove(remove);
                    //lstGridRelationship.Add(grs);

                    //remove = lstGridRelationship.Where(p => p.bigGridID == 1 && p.smallGridID == 7).First();
                    //grs.smallGridID = 7;
                    //lstGridRelationship.Remove(remove);
                    //lstGridRelationship.Add(grs);
                    //GridRelationship grs2 = new GridRelationship();
                    //grs2.bigGridID = 4;
                    //grs2.smallGridID = 7;
                    //grs2.lstGridRelationshipAttribute = grs.lstGridRelationshipAttribute;
                    //remove = lstGridRelationship.Where(p => p.bigGridID == 4 && p.smallGridID == 7).First();
                    //lstGridRelationship.Remove(remove);
                    //lstGridRelationship.Add(grs2);
                    // //----------修正12kmclip To 12km
                    // //var query = lstGridRelationship.Where(p => p.bigGridID == 7 || p.smallGridID == 7).ToList();
                    // //foreach (GridRelationship gr in query)
                    // //{
                    // //    lstGridRelationship.Remove(gr);
                    // //}
                    // //var query13 = lstGridRelationship.Where(p => p.bigGridID == 13 || p.smallGridID == 13).ToList();
                    // //foreach (GridRelationship gr in query13)
                    // //{
                    // //    GridRelationship grnew = new GridRelationship();
                    // //    grnew.smallGridID = gr.smallGridID;
                    // //    grnew.bigGridID = gr.bigGridID;
                    // //    if (grnew.smallGridID == 13) grnew.smallGridID = 7;
                    // //    if (grnew.bigGridID == 13) grnew.bigGridID = 7;

                    // //    grnew.lstGridRelationshipAttribute = gr.lstGridRelationshipAttribute;
                    // //    lstGridRelationship.Add(grnew);
                    // //}
                    // //GridRelationship grnew713 = new GridRelationship();
                    // //grnew713.bigGridID = 13;
                    // //grnew713.smallGridID = 7;
                    // //grnew713.lstGridRelationshipAttribute = new List<GridRelationshipAttribute>();

                    // ////---------add 13---------
                    // //FeatureSet fs = new FeatureSet();
                    // //BenMAPGrid bg = Grid.GridCommon.getBenMAPGridFromID(13);
                    // //if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + (bg as ShapefileGrid).ShapefileName + ".shp"))
                    // //{
                    // //    string shapeFileName = Application.StartupPath + @"\Data\Shapefiles\" + (bg as ShapefileGrid).ShapefileName + ".shp";
                    // //    fs.Open(shapeFileName);
                    // //    foreach (DataRow dr in fs.DataTable.Rows)
                    // //    {
                    // //        grnew713.lstGridRelationshipAttribute.Add(new GridRelationshipAttribute()
                    // //        {
                    // //            bigGridRowCol = new RowCol() { Col = Convert.ToInt32(dr["Col"]), Row = Convert.ToInt32(dr["Row"]) },
                    // //            smallGridRowCol = new List<RowCol>() { new RowCol() { Col = Convert.ToInt32(dr["Col"]), Row = Convert.ToInt32(dr["Row"]) } }
                    // //        });

                    // //    }
                    // //    //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                    // //}
                    // //lstGridRelationship.Add(grnew713);
                    //Grid.GridCommon.createGridRelationshipFile(@"D:\GridRelationship.gr", lstGridRelationship);
                }
                return CommonClass.lstGridRelationshipAll;
            }
        }
        public static void SaveGridRelationship(string strFile,GRGridRelationShip gr)
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
                    //FeatureSet fsMid = new FeatureSet();
                    for (int i = 0; i < fsSmall.DataTable.Rows.Count; i++)
                    {
                        IFeature f = fsSmall.GetFeature(i);
                        lstRowColSmall.Add(new RowCol() { Col = Convert.ToInt32(f.DataRow["Col"]), Row = Convert.ToInt32(f.DataRow["Row"]) });
                        lstMid.Add(new DotSpatial.Topology.Point(f.Centroid().Coordinates[0]));
                        //fsSmall.AddFeature(new DotSpatial.Topology.Point(f.Centroid().Coordinates[0]));
                    }
                    for (int j = 0; j < fsBig.DataTable.Rows.Count; j++)
                    {
                        IFeature f = fsBig.GetFeature(j);
                        //grnew713.lstGridRelationshipAttribute.Add(new GridRelationshipAttribute()
                        //{
                        //    bigGridRowCol = new RowCol() { Col = Convert.ToInt32(dr["Col"]), Row = Convert.ToInt32(dr["Row"]) },
                        //    smallGridRowCol = new List<RowCol>() { new RowCol() { Col = Convert.ToInt32(dr["Col"]), Row = Convert.ToInt32(dr["Row"]) } }
                        //});
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
                    //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                }
                return grReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
          /// <summary>
        /// This tests each feature of the input
        /// </summary>
        /// <param name="self">This featureSet</param>
        /// <param name="other">The featureSet to perform intersection with</param>
        /// <param name="joinType">The attribute join type</param>
        /// <param name="progHandler">A progress handler for status messages</param>
        /// <returns>An IFeatureSet with the intersecting features, broken down based on the join Type</returns>
        public static IFeatureSet Intersection( IFeatureSet self, IFeatureSet other, FieldJoinType joinType)
        {
            IFeatureSet result = null;
            //ProgressMeter pm = new ProgressMeter(progHandler, "Calculating Intersection", self.Features.Count);
            if (joinType == FieldJoinType.All)
            {
                result = self.CombinedFields( other);
                // Intersection is symmetric, so only consider I X J where J <= I
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
                    //pm.CurrentValue = i;
                    i++;
                }
                //pm.Reset();
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
                        //union = union.Union(other.Features[i]);

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
        /// <summary>
        /// Nation Intersaction Slow ,County replace Nation
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        public static List<GridRelationshipAttributePercentage> IntersectionPercentageNation(IFeatureSet self, IFeatureSet other, FieldJoinType joinType,int big,int small)
        {
            List<GridRelationshipAttributePercentage> result = new List<GridRelationshipAttributePercentage>();
            try
            {
                //ProgressMeter pm = new ProgressMeter(progHandler, "Calculating Intersection", self.Features.Count);
                if (joinType == FieldJoinType.All)
                {
                    //result = self.CombinedFields(other);
                    // Intersection is symmetric, so only consider I X J where J <= I
                    if (!self.AttributesPopulated) self.FillAttributes();
                    if (!other.AttributesPopulated) other.FillAttributes();
                    int i = 0;
                    //DotSpatial.Topology.IGeometry geo = null;
                    Dictionary<string, Dictionary<string, double>> dicRelation = new Dictionary<string, Dictionary<string, double>>();
                    Polygon pSelfExtent = null;
                    Polygon pOtherExtent = null;
                    double dSumArea = 0.0;
                    foreach (IFeature selfFeature in self.Features)
                    {
                        IFeature intersactFeature = null;// selfFeature.Intersection(other.Features[iotherFeature]);
                        if (big == 20)
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
                                intersactFeature = null;// selfFeature.Intersection(other.Features[iotherFeature]);

                                if ((other.Features.Count < 5 || self.Features.Count < 5) && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum)) == 0 &&
                                    other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum.X, selfFeature.Envelope.Minimum.Y)) == 0 &&
                                    other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum)) == 0 &&
                                    other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum.X, selfFeature.Envelope.Maximum.Y)) == 0
                                    )//.Contains(selfFeature))
                                {
                                    intersactFeature = selfFeature;// selfFeature.Intersection(other.Features[iotherFeature]);
                                    //geo = selfFeature.Intersection(other.Features[iotherFeature]).BasicGeometry;// DotSpatial.Topology.Geometry.FromBasicGeometry(selfFeature.BasicGeometry).Intersection(DotSpatial.Topology.Geometry.FromBasicGeometry(other.Features[iotherFeature]));
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
                                            intersactFeature = selfFeature.Intersection(other.Features[iotherFeature]);// other.Features[iotherFeature].Intersection(selfFeature);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        //geo =DotSpatial.Topology.Geometry.FromBasicGeometry(other.Features[iotherFeature]).Intersection( DotSpatial.Topology.Geometry.FromBasicGeometry(selfFeature.BasicGeometry));

                                    }

                                //if (geo != null && geo.IsEmpty==false)
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

                                //selfFeature.Intersection(otherFeature, result, joinType);
                            }

                        }
                        //pm.CurrentValue = i;
                        i++;
                    }
                    //--------to
                    //---------------求sum(Area)--------

                    if (small == 20)
                    {
                        dSumArea = dicRelation.Sum(p => p.Value.Sum(a => a.Value));
                    }

                    if (big == 20)
                    {
                        //------------则Percentage都为1只要是和它交叉-------------
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
                                foreach(KeyValuePair<string, double> kin in k.Value)
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
                                percentage = kin.Value/dSumArea,
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
        /// <summary>
        /// This tests each feature of the input
        /// </summary>
        /// <param name="self">This featureSet</param>
        /// <param name="other">The featureSet to perform intersection with</param>
        /// <param name="joinType">The attribute join type</param>
        /// <param name="progHandler">A progress handler for status messages</param>
        /// <returns>An IFeatureSet with the intersecting features, broken down based on the join Type</returns>
        public static List<GridRelationshipAttributePercentage> IntersectionPercentage(IFeatureSet self, IFeatureSet other, FieldJoinType joinType)
        {
            List<GridRelationshipAttributePercentage> result = new List<GridRelationshipAttributePercentage>();
            try
            {
                //ProgressMeter pm = new ProgressMeter(progHandler, "Calculating Intersection", self.Features.Count);
                if (joinType == FieldJoinType.All)
                {
                    //result = self.CombinedFields(other);
                    // Intersection is symmetric, so only consider I X J where J <= I
                    if (!self.AttributesPopulated) self.FillAttributes();
                    if (!other.AttributesPopulated) other.FillAttributes();
                    int i = 0;
                    //DotSpatial.Topology.IGeometry geo = null;
                    Dictionary<string, Dictionary<string, double>> dicRelation = new Dictionary<string, Dictionary<string, double>>();
                    Polygon pSelfExtent = null;
                    Polygon pOtherExtent = null;

                    foreach (IFeature selfFeature in self.Features)
                    {
                        List<int> potentialOthers = other.SelectIndices(selfFeature.Envelope.ToExtent());
                        foreach (int iotherFeature in potentialOthers)
                        {
                            if (iotherFeature == 33)//28
                            { 
                            }
                            IFeature intersactFeature = null;// selfFeature.Intersection(other.Features[iotherFeature]);
                            //List<Coordinate> lstCoor = new List<Coordinate>();
                            //lstCoor.Add(new Coordinate(selfFeature.Envelope.X, selfFeature.Envelope.Y));
                            //lstCoor.Add(new Coordinate(selfFeature.Envelope.X + selfFeature.Envelope.Width, selfFeature.Envelope.Y));
                            //lstCoor.Add(new Coordinate(selfFeature.Envelope.X + selfFeature.Envelope.Width, selfFeature.Envelope.Y + selfFeature.Envelope.Height));
                            //lstCoor.Add(new Coordinate(selfFeature.Envelope.X, selfFeature.Envelope.Y + selfFeature.Envelope.Height));

                            //pSelfExtent = new Polygon(lstCoor);

                            //List<Coordinate> lstCoorOther = new List<Coordinate>();
                            //lstCoorOther.Add(new Coordinate(other.Features[iotherFeature].Envelope.X, other.Features[iotherFeature].Envelope.Y));
                            //lstCoorOther.Add(new Coordinate(other.Features[iotherFeature].Envelope.X + other.Features[iotherFeature].Envelope.Width, other.Features[iotherFeature].Envelope.Y));
                            //lstCoorOther.Add(new Coordinate(other.Features[iotherFeature].Envelope.X + other.Features[iotherFeature].Envelope.Width, other.Features[iotherFeature].Envelope.Y + other.Features[iotherFeature].Envelope.Height));
                            //lstCoorOther.Add(new Coordinate(other.Features[iotherFeature].Envelope.X, other.Features[iotherFeature].Envelope.Y + other.Features[iotherFeature].Envelope.Height));

                            //pOtherExtent = new Polygon(lstCoorOther);
                            //if (other.Features[iotherFeature].Contains(new Point(selfFeature.Envelope.X, selfFeature.Envelope.Y)) &&
                            //    other.Features[iotherFeature].Contains(new Point(selfFeature.Envelope.X + selfFeature.Envelope.Width, selfFeature.Envelope.Y)) &&
                            //    other.Features[iotherFeature].Contains(new Point(selfFeature.Envelope.X + selfFeature.Envelope.Width, selfFeature.Envelope.Y + selfFeature.Envelope.Height)) &&
                            //    other.Features[iotherFeature].Contains(new Point(selfFeature.Envelope.X, selfFeature.Envelope.Y + selfFeature.Envelope.Height)))
                            //{
                            //    intersactFeature = selfFeature;// selfFeature.Intersection(other.Features[iotherFeature]);
                            //    //geo = selfFeature.Intersection(other.Features[iotherFeature]).BasicGeometry;// DotSpatial.Topology.Geometry.FromBasicGeometry(selfFeature.BasicGeometry).Intersection(DotSpatial.Topology.Geometry.FromBasicGeometry(other.Features[iotherFeature]));
                            //}
                            //else if (selfFeature.Contains(new Point(other.Features[iotherFeature].Envelope.X, other.Features[iotherFeature].Envelope.Y)) &&
                            //    selfFeature.Contains(new Point(other.Features[iotherFeature].Envelope.X + other.Features[iotherFeature].Envelope.Width, other.Features[iotherFeature].Envelope.Y)) &&
                            //    selfFeature.Contains(new Point(other.Features[iotherFeature].Envelope.X + other.Features[iotherFeature].Envelope.Width, other.Features[iotherFeature].Envelope.Y + other.Features[iotherFeature].Envelope.Height)) &&
                            //    selfFeature.Contains(new Point(other.Features[iotherFeature].Envelope.X, other.Features[iotherFeature].Envelope.Y + other.Features[iotherFeature].Envelope.Height)))
                            //{
                            //    intersactFeature = other.Features[iotherFeature];
                            //}
                            if ((other.Features.Count < 5 || self.Features.Count < 5) && other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum)) == 0 &&
                                other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum.X, selfFeature.Envelope.Minimum.Y)) == 0 &&
                                other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Maximum)) == 0 &&
                                other.Features[iotherFeature].Distance(new Point(selfFeature.Envelope.Minimum.X, selfFeature.Envelope.Maximum.Y)) == 0
                                )//.Contains(selfFeature))
                                //if (other.Features[iotherFeature].Contains(selfFeature))
                                {
                                    intersactFeature = selfFeature;// selfFeature.Intersection(other.Features[iotherFeature]);
                                    //geo = selfFeature.Intersection(other.Features[iotherFeature]).BasicGeometry;// DotSpatial.Topology.Geometry.FromBasicGeometry(selfFeature.BasicGeometry).Intersection(DotSpatial.Topology.Geometry.FromBasicGeometry(other.Features[iotherFeature]));
                                }

                            else if ((other.Features.Count < 5 || self.Features.Count < 5) && selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum)) == 0 &&
                           selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum.X, other.Features[iotherFeature].Envelope.Minimum.Y)) == 0 &&
                            selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Maximum)) == 0 &&
                            selfFeature.Distance(new Point(other.Features[iotherFeature].Envelope.Minimum.X, other.Features[iotherFeature].Envelope.Maximum.Y)) == 0)
                            //else if (selfFeature.Contains(other.Features[iotherFeature]))
                            {
                                intersactFeature = other.Features[iotherFeature];
                            }
                            else
                            {
                                try
                                {
                                    intersactFeature = selfFeature.Intersection(other.Features[iotherFeature]);// other.Features[iotherFeature].Intersection(selfFeature);
                                }
                                catch (Exception ex)
                                { 
                                    //------------一旦出错，看看是否是在里面
                                    try
                                    {
                                        if (selfFeature.IsWithinDistance(other.Features[iotherFeature], 0.00001))
                                        {
                                            if (selfFeature.Area() > other.Features[iotherFeature].Area())
                                                intersactFeature = other.Features[iotherFeature];
                                            else
                                                intersactFeature = selfFeature;
                                        }
                                       // List<Coordinate> lstTemp = new List<Coordinate>() { new Coordinate(151.301, -33.69), new Coordinate(151.301, -33.70), new Coordinate(151.303, -33.70), new Coordinate(151.303, -33.69) };
                                       //IFeature ftemp= other.Features[iotherFeature].Intersection(new Feature(new Polygon(lstTemp)));
                                    }
                                    catch
                                    { 
                                    }
                                }
                                //geo =DotSpatial.Topology.Geometry.FromBasicGeometry(other.Features[iotherFeature]).Intersection( DotSpatial.Topology.Geometry.FromBasicGeometry(selfFeature.BasicGeometry));

                            }
                            //if (geo != null && geo.IsEmpty==false)
                            if (intersactFeature != null && intersactFeature.BasicGeometry != null)
                            {
                                //GridRelationshipAttributePercentage gr = new GridRelationshipAttributePercentage()
                                //{

                                //    sourceCol = Convert.ToInt32(other.Features[iotherFeature].DataRow["Col"]),
                                //    sourceRow = Convert.ToInt32(other.Features[iotherFeature].DataRow["Row"]),
                                //    targetCol = Convert.ToInt32(selfFeature.DataRow["Col"]),
                                //    targetRow = Convert.ToInt32(selfFeature.DataRow["Row"]),
                                //    percentage = geo.Area / other.Features[iotherFeature].Area(),
                                //};
                                //result.Add(gr);
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
                            //selfFeature.Intersection(otherFeature, result, joinType);
                        }

                        //pm.CurrentValue = i;
                        i++;
                    }
                    //--------to
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
                                        percentage = kin.Value/d,
                                    };
                                    result.Add(gr);
                                }
                            }
                        }
                    }
                    //pm.Reset();
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
                    //----------Clip first! ----------
                    //List<IFeature> lstClip= fsSmall.Select(fsBig.Extent);
                    //IFeatureSet fsSmallClip = new FeatureSet(lstClip);
                    //List<int> lstRemove = new List<int>();
                    //for (int i = 0; i < fsSmall.Features.Count; i++)
                    //{
                    //    if (!lstClip.Contains(i))
                    //        lstRemove.Add(i);
 
                    //}
                    //List<IFeature> lstRemoveFeature = new List<IFeature>();
                    //foreach (int i in lstRemove)
                    //{
                    //    lstRemoveFeature.Add(fsSmall.Features[i]);
                         
                    //}
                    //foreach (IFeature f in lstRemoveFeature)
                    //{
                    //    fsSmall.Features.Remove(f);
                    //}
                    List<GridRelationshipAttributePercentage> lstGR = IntersectionPercentage(fsBig, fsSmall, FieldJoinType.All);
                    //---------------------------填入数据库！--------------------------------
                    string commandText = "select max(PercentageID) from GridDefinitionPercentages";
                    //-----first get the max of id in griddefinitonpercentages
                    //ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText))+1;

                    //-----insert into griddefinitonpercentages
                    commandText = string.Format("insert into GridDefinitionPercentages values({0},{1},{2})", iMax, small, big);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    //-----insert into GridDefinitionPercentageEntries
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
        public static List<BenMAPPollutant> lstPollutantAll;//所有污染物

        public static BaseControlCRSelectFunction BaseControlCRSelectFunction;//所有BaseControlAndCRSelectFunciton
        public static BaseControlCRSelectFunctionCalculateValue BaseControlCRSelectFunctionCalculateValue;//所有BaseControlAndCRSelectFunciton以及Value

        private static Dictionary<string, int> dicAllRace;//所有race

        public static Dictionary<string, int> DicAllRace
        {
            get
            {
                if (dicAllRace == null)
                    dicAllRace = Configuration.ConfigurationCommonClass.getAllRace();
                return CommonClass.dicAllRace;
            }
        }

        private static Dictionary<string, int> dicAllGender;//所有Gender

        public static Dictionary<string, int> DicAllGender
        {
            get
            {
                if (dicAllGender == null)
                    dicAllGender = Configuration.ConfigurationCommonClass.getAllGender();
                return CommonClass.dicAllGender;
            }
        }

        private static Dictionary<string, int> dicAllEthnicity;//所有Ethnicity

        public static Dictionary<string, int> DicAllEthnicity
        {
            get
            {
                if (dicAllEthnicity == null)
                    dicAllEthnicity = Configuration.ConfigurationCommonClass.getAllEthnicity();
                return CommonClass.dicAllEthnicity;
            }
        }

        //-------------------APVX-------------------------------
        public static List<CRSelectFunctionCalculateValue> lstCRResultAggregation;
        public static List<CRSelectFunctionCalculateValue> lstCRResultAggregationQALY;
        public static IncidencePoolingAndAggregationAdvance IncidencePoolingAndAggregationAdvance;
        //= new IncidencePoolingAndAggregationAdvance() { 
        // IncidenceAggregation=CommonClass.MainSetup!=null && CommonClass.MainSetup.SetupID==1?Grid.GridCommon.getBenMAPGridFromID(2):null,
        // ValuationAggregation=CommonClass.MainSetup!=null && CommonClass.MainSetup.SetupID==1?Grid.GridCommon.getBenMAPGridFromID(2):null,
        // QALYAggregation = CommonClass.MainSetup != null && CommonClass.MainSetup.SetupID == 1 ? Grid.GridCommon.getBenMAPGridFromID(2) : null
        //}
        //;//Advance
        public static List<IncidencePoolingAndAggregation> lstIncidencePoolingAndAggregation;
        //public static List<ValuationMethodPoolingAndAggregation> lstValuationMethodPoolingAndAggregation;
        //public static IncidencePoolingAndAggregation IncidencePoolingAndAggregation;//IncidencePooling;
        public static CRSelectFunctionCalculateValue IncidencePoolingResult;
        public static ValuationMethodPoolingAndAggregation ValuationMethodPoolingAndAggregation;
        //--------------------Result---------------------------
        public static List<ChartResult> lstChartResult;

        private static List<CRSelectFunction> _lstAddCRFunction = new List<CRSelectFunction>();
        /// <summary>
        /// 记录HealthImpactFunctions中新增CRFunction的ID
        /// </summary>
        public static List<CRSelectFunction> LstUpdateCRFunction
        {
            get { return _lstAddCRFunction; }
            set
            { _lstAddCRFunction = value; }
        }

        private static List<CRSelectFunction> _lstDelCRFunction = new List<CRSelectFunction>();
        /// <summary>
        /// 记录HealthImpactFunctions中删除CRFunction的ID
        /// </summary>
        public static List<CRSelectFunction> LstDelCRFunction
        {
            get { return _lstDelCRFunction; }
            set
            { _lstDelCRFunction = value; }
        }
        #endregion 私有变量或属性
        #region GetSystemType
        public static bool IsWindows98
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32Windows) && (Environment.OSVersion.Version.Minor == 10) && (Environment.OSVersion.Version.Revision.ToString() != "2222A");
            }
        }

        //C#判断操作系统是否为Windows98第二版
        public static bool IsWindows98Second
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32Windows) && (Environment.OSVersion.Version.Minor == 10) && (Environment.OSVersion.Version.Revision.ToString() == "2222A");
            }
        }

        //C#判断操作系统是否为Windows2000
        public static bool IsWindows2000
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 0);
            }
        }

        //C#判断操作系统是否为WindowsXP
        public static bool IsWindowsXP
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 1);
            }
        }

        //C#判断操作系统是否为Windows2003
        public static bool IsWindows2003
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 2);
            }
        }

        //C#判断操作系统是否为WindowsVista
        public static bool IsWindowsVista
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 0);
            }
        }

        //C#判断操作系统是否为Windows7
        public static bool IsWindows7
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1);
            }
        }

        //C#判断操作系统是否为Unix
        public static bool IsUnix
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Unix;
            }
        }

        //C#判断操作系统是否为Unix
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
        #endregion
        /// <summary>
        /// 取得user选得工作簿
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        //public static int SelectedSheetIndex(string filePath)
        //{
        //    try
        //    {
        //        int Index = 0;
        //        List<string> lstSheetsName = new List<string>();
        //        lstSheetsName = GetSheetNames(filePath);
        //        if (lstSheetsName.Count == 1) return 1;
        //        Dictionary<string, int> dicSheetNum = new Dictionary<string, int>();
        //        for (int i = 0; i < lstSheetsName.Count; i++)
        //        {
        //            //strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
        //            dicSheetNum.Add(lstSheetsName[i], i + 1);
        //        }

        //        Sheets frm = new Sheets(dicSheetNum);
        //        DialogResult rtn = frm.ShowDialog();
        //        if (rtn == DialogResult.OK)
        //        {
        //            Index = frm.sheetIndex;
        //        }
        //        return Index;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex.Message);
        //        return 1;
        //    }
        //}

        //public static List<string> GetSheetNames(string path)
        //{
        //    List<string> sheets = new List<string>();
        //    //string constring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\"", path); ;
        //    //OleDbConnection oleCon = new OleDbConnection(constring);
        //    //if (oleCon.State != ConnectionState.Closed) { oleCon.Close(); }
        //    //oleCon.Open();
        //    ////返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等
        //    //DataTable dtSheetName = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
        //    //string[] tableNames = new string[dtSheetName.Rows.Count];
        //    //for (int k = 0; k < dtSheetName.Rows.Count; k++)
        //    //{
        //    //    tableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
        //    //    sheets.Add(tableNames[k]);
        //    //}
        //    //string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;""", path);
        //    //DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
        //    //DbConnection connection = factory.CreateConnection();
        //    //connection.ConnectionString = connectionString;
        //    //connection.Open();
        //    //DataTable tbl = connection.GetSchema("Tables");
        //    //connection.Close();
        //    //foreach (DataRow row in tbl.Rows)
        //    //{
        //    //    string sheetName = (string)row["TABLE_NAME"];
        //    //    if (sheetName.EndsWith("$"))
        //    //    {
        //    //        sheetName = sheetName.Substring(0, sheetName.Length - 1);
        //    //    }
        //    //    sheets.Add(sheetName);
        //    //}

        //    Microsoft.Office.Interop.Excel.Workbook wb = null;
        //    object missing = System.Reflection.Missing.Value;
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();//lauch excel application
        //    if (excel != null)
        //    {
        //        excel.Visible = false;
        //        excel.UserControl = true;
        //        // 以只读的形式打开EXCEL文件 
        //        wb = excel.Workbooks.Open(path, missing, true, missing, missing, missing,
        //         missing, missing, missing, true, missing, missing, missing, missing, missing);
        //        for (int i = 1; i <= wb.Worksheets.Count; i++)
        //        {
        //            sheets.Add((wb.Worksheets[i]).Name);
        //        }
        //    }
        //    wb.Close(false, Type.Missing, Type.Missing);
        //    Kill(excel);
        //    //excel.Quit();
        //    return sheets;
        //}

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        //public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
        //{
        //    IntPtr t = new IntPtr(excel.Hwnd);   //得到这个句柄，具体作用是得到这块内存入口   

        //    int k = 0;
        //    GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k  
        //    System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用  
        //    p.Kill();     //关闭进程k  
        //}

        /// <summary> 
        /// 把Excel里的数据转换为DataTable,应用引用的com组件：Microsoft.Office.Interop.Excel.dll 读取EXCEL文件
        /// </summary> 
        /// <param name="filenameurl">物理路径</param> 
        /// <param name="sheetIndex">sheet名称的索引</param> 
        /// <param name="splitstr">如果是已存在列，则自定义添加的字符串</param> 
        /// <returns></returns> 
        public static System.Data.DataTable ExcelToDataTable(string filenameurl)
        {
            try
            {
                if (filenameurl.Substring(filenameurl.Length - 3, 3).ToLower() == "csv")
                {
                    return DataSourceCommonClass.getDataTableFromCSV(filenameurl);
                }
                #region Microsoft.Office.Interop.Excel
                //Microsoft.Office.Interop.Excel.Workbook wb = null;
                //Microsoft.Office.Interop.Excel.Worksheet ws = null;
                //bool isEqual = false;//不相等 
                //ArrayList columnArr = new ArrayList();//列字段表 
                //System.Data.DataSet myDs = new System.Data.DataSet();
                ////DataTable xlsTable = myDs.Tables.Add("show");
                //DataTable xlsTable = new DataTable() { TableName = "show" };
                //object missing = System.Reflection.Missing.Value;
                //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();//lauch excel application
                //if (excel != null)
                //{
                //    excel.Visible = false;
                //    excel.UserControl = true;
                //    // 以只读的形式打开EXCEL文件 
                //    wb = excel.Workbooks.Open(filenameurl, missing, true, missing, missing, missing,
                //     missing, missing, missing, true, missing, missing, missing, missing, missing);
                //    //取得第一个工作薄 
                //    ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.get_Item(sheetIndex);
                //    //取得总记录行数(包括标题列) 
                //    int rowsint = ws.UsedRange.Cells.Rows.Count; //得到行数 
                //    int columnsint = ws.UsedRange.Cells.Columns.Count;//得到列数 
                //    DataRow dr;
                //    for (int i = 1; i <= columnsint; i++)
                //    {
                //        //判断是否有列相同 
                //        if (i >= 2)
                //        {
                //            int r = 0;
                //            for (int k = 1; k <= i - 1; k++)//列从第一列到第i-1列遍历进行比较 
                //            {
                //                if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, i]).Text.ToString() == ((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString())
                //                {
                //                    //如果该列的值等于前面列中某一列的值 
                //                    xlsTable.Columns.Add(((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, i]).Text.ToString() + splitstr + (r + 1).ToString(), typeof(string));
                //                    columnArr.Add(((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, i]).Text.ToString() + splitstr + (r + 1).ToString());
                //                    isEqual = true;
                //                    r++;
                //                    break;
                //                }
                //                else
                //                {
                //                    isEqual = false;
                //                    continue;
                //                }
                //            }
                //            if (!isEqual)
                //            {
                //                xlsTable.Columns.Add(((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, i]).Text.ToString(), typeof(string));
                //                columnArr.Add(((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, i]).Text.ToString());
                //            }
                //        }
                //        else
                //        {
                //            xlsTable.Columns.Add(((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, i]).Text.ToString(), typeof(string));
                //            columnArr.Add(((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, i]).Text.ToString());
                //        }
                //    }
                //    for (int i = 2; i <= rowsint; i++)
                //    {
                //        dr = xlsTable.NewRow();

                //        xlsTable.Rows.Add(dr);
                //    }
                //    for (int j = 1; j <= columnsint; j++)
                //    {
                //        //dr[columnArr[j - 1].ToString()] = ((Microsoft.Office.Interop.Excel.Range)ws.Cells[i, j]).Value2.ToString();
                //        System.Array values = (System.Array)((Microsoft.Office.Interop.Excel.Range)ws.Columns[j]).Value;
                //        for (int i = 2; i <= rowsint; i++)
                //        {
                //            xlsTable.Rows[i - 2][j - 1] = values.GetValue(i, 1);
                //        }
                //        //var v = (Microsoft.Office.Interop.Excel.Range)ws.Rows[2];
                //        //string s= ((Microsoft.Office.Interop.Excel.Range)ws.Rows[i]).get;
                //    }
                //}
                //if (xlsTable != null) { myDs.Tables.Add(xlsTable); }
                //wb.Close(false, Type.Missing, Type.Missing);
                //Kill(excel);
                ////excel.Quit();
                ////excel = null;
                ////Dispose(ws, wb);
                #endregion

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
            //从末到开始
            try
            {
                if (strFile == "") return;
                BenMAPProject benMAPProject = new BenMAPProject();
                if (ValuationMethodPoolingAndAggregation != null)
                {
                    benMAPProject.ValuationMethodPoolingAndAggregation = APVX.APVCommonClass.getNoResultValuationMethodPoolingAndAggregation(ValuationMethodPoolingAndAggregation);// .BaseControlCRSelectFunctionCalculateValue = new BaseControlCRSelectFunctionCalculateValue();
                    benMAPProject.ValuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                    //Save----------
                }
                else if (BaseControlCRSelectFunctionCalculateValue != null)
                {
                    // benMAPProject.BaseControlCRSelectFunctionCalculateValue =   APVX.APVCommonClass.getNoResultBaseControlCRSelectFunctionCalculateValue(BaseControlCRSelectFunctionCalculateValue); // BaseControlCRSelectFunctionCalculateValue;
                    benMAPProject.IncidencePoolingAndAggregationAdvance = IncidencePoolingAndAggregationAdvance;
                    benMAPProject.lstIncidencePoolingAndAggregation = lstIncidencePoolingAndAggregation;
                    benMAPProject.IncidencePoolingResult = IncidencePoolingResult;
                    benMAPProject.BaseControlCRSelectFunction = BaseControlCRSelectFunction;
                    benMAPProject.BaseControlCRSelectFunction.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 4);

                    //Save------------
                }
                else if (BaseControlCRSelectFunction != null)
                {
                    benMAPProject.BaseControlCRSelectFunction = BaseControlCRSelectFunction;
                    //Save--------------
                }
                else
                {
                    benMAPProject.ManageSetup = ManageSetup;
                    benMAPProject.MainSetup = MainSetup;// 当前活动区域
                    benMAPProject.LstPollutant = LstPollutant;//污染物列表
                    benMAPProject.RBenMAPGrid = RBenMAPGrid;//系统选择的Grid

                    benMAPProject.GBenMAPGrid = GBenMAPGrid;//系统选择的Region
                    benMAPProject.LstBaseControlGroup = LstBaseControlGroup;//系统设置的DataSource Base and Control
                    //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                    benMAPProject.CRThreshold = CRThreshold;//阈值
                    benMAPProject.CRLatinHypercubePoints = CRLatinHypercubePoints;//拉丁立体方采样点数
                    benMAPProject.CRRunInPointMode = CRRunInPointMode;//是否使用拉丁立体方方采样
                    benMAPProject.CRSeeds = CRSeeds;
                    benMAPProject.BenMAPPopulation = BenMAPPopulation;//系统设置的Population

                    benMAPProject.lstPollutantAll = lstPollutantAll;//所有污染物
                }
                benMAPProject.IncidencePoolingAndAggregationAdvance = IncidencePoolingAndAggregationAdvance;
                benMAPProject.BenMAPPopulation = BenMAPPopulation;//系统设置的Population
                if (File.Exists(strFile))
                {
                    File.Delete(strFile);
                }
                using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
                {
                    //BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        //formatter.Serialize(fs, benMAPProject);

                        //fs.Close();
                        //fs.Dispose();
                        //formatter = null;
                        Serializer.Serialize<BenMAPProject>(fs, benMAPProject);//, PrefixStyle.Fixed32);
                        //fs.Flush();
                        //fs.Position = 0;

                        //TestObject obj2 = Serializer.Deserialize<TestObject>(fs);
                        //Console.WriteLine(obj2);  
                        fs.Close();
                        fs.Dispose();
                        //return true;
                    }
                    catch
                    {
                        fs.Close();
                        fs.Dispose();
                        //formatter = null;
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
                string commandText = "select SetupID,SetupName from Setups where  SetupName='" + SetupName+"'";
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
                CommonClass.LstPollutant = null;//污染物列表
                CommonClass.RBenMAPGrid = null;//系统选择的Grid

                CommonClass.GBenMAPGrid = null;//系统选择的Region
                CommonClass.LstBaseControlGroup = null;//系统设置的DataSource Base and Control
                //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                CommonClass.CRThreshold = 0;//阈值
                CommonClass.CRLatinHypercubePoints = 10;//拉丁立体方采样点数
                CommonClass.CRRunInPointMode = false;//是否使用拉丁立体方方采样

                CommonClass.BenMAPPopulation = null;//系统设置的Population

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
                }//所有BaseControlAndCRSelectFunciton
                CommonClass.BaseControlCRSelectFunction = null;
                //-------------------APVX-------------------------------
                //CommonClass.IncidencePoolingAndAggregationAdvance = null;//Advance
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
                //public  List<ValuationMethodPoolingAndAggregation> lstValuationMethodPoolingAndAggregation;
                //public  IncidencePoolingAndAggregation IncidencePoolingAndAggregation;//IncidencePooling;

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
                //GC.Collect();
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
                    //BinaryFormatter formatter = new BinaryFormatter();
                    //benMAPProject = (BenMAPProject)formatter.Deserialize(fs);//在这里大家要注意咯,他的返回值是object
                    benMAPProject = Serializer.Deserialize<BenMAPProject>(fs);//, PrefixStyle.Fixed32);
                    fs.Close();
                    fs.Dispose();
                    //GC.Collect();
                }
                if (benMAPProject.ValuationMethodPoolingAndAggregation != null)
                {
                    ValuationMethodPoolingAndAggregation = benMAPProject.ValuationMethodPoolingAndAggregation;
                    //------------update数据-------------------------
                    //foreach (BaseControlGroup bcg in ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
                    //{
                    //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
                    //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
                    //}
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

                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);//..ManageSetup;
                    MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);//..ManageSetup;;// benMAPProject.MainSetup;// 当前活动区域
                    LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList();// benMAPProject.LstPollutant;//污染物列表
                    RBenMAPGrid = BaseControlCRSelectFunction.RBenMapGrid;// benMAPProject.RBenMAPGrid;//系统选择的Grid

                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType;//系统选择的Region
                    LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup;// benMAPProject.LstBaseControlGroup;//系统设置的DataSource Base and Control
                    //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                    CRThreshold = BaseControlCRSelectFunction.CRThreshold;// benMAPProject.CRThreshold;//阈值
                    CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints;//拉丁立体方采样点数
                    CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode;//是否使用拉丁立体方方采样
                    CRSeeds = BaseControlCRSelectFunction.CRSeeds;
                    BenMAPPopulation = BaseControlCRSelectFunction.BenMAPPopulation;//系统设置的Population

                    //lstPollutantAll = benMAPProject.lstPollutantAll;//所有污染物
                    //lstIncidencePoolingAndAggregation = benMAPProject.ValuationMethodPoolingAndAggregation.lstIncidencePoolingAndAggregation;
                    //IncidencePoolingResult = benMAPProject.ValuationMethodPoolingAndAggregation.IncidencePoolingResult;
                    //Save----------
                }
                else if (benMAPProject.BaseControlCRSelectFunctionCalculateValue != null)
                {
                    BaseControlCRSelectFunctionCalculateValue = benMAPProject.BaseControlCRSelectFunctionCalculateValue;
                    //------------update数据-------------------------
                    //foreach (BaseControlGroup bcg in BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
                    //{
                    //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
                    //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
                    //}
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

                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);//..ManageSetup;
                    MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);//..ManageSetup;;// benMAPProject.MainSetup;// 当前活动区域
                    LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList();// benMAPProject.LstPollutant;//污染物列表

                    RBenMAPGrid = BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;// benMAPProject.RBenMAPGrid;//系统选择的Grid

                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType;//系统选择的Region
                    LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup;// benMAPProject.LstBaseControlGroup;//系统设置的DataSource Base and Control
                    //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                    CRThreshold = BaseControlCRSelectFunction.CRThreshold;// benMAPProject.CRThreshold;//阈值
                    CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints;//拉丁立体方采样点数
                    CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode;//是否使用拉丁立体方方采样
                    CRSeeds = BaseControlCRSelectFunction.CRSeeds;
                    BenMAPPopulation = BaseControlCRSelectFunction.BenMAPPopulation;//系统设置的Population

                    //Save------------
                }
                else if (benMAPProject.BaseControlCRSelectFunction != null)
                {
                    BaseControlCRSelectFunction = benMAPProject.BaseControlCRSelectFunction;
                    //------------update数据-------------------------
                    //foreach (BaseControlGroup bcg in BaseControlCRSelectFunction.BaseControlGroup)
                    //{
                    //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
                    //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
                    //}
                    ManageSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);//..ManageSetup;
                    MainSetup = getBenMAPSetupFromID(BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);//..ManageSetup;;// benMAPProject.MainSetup;// 当前活动区域
                    LstPollutant = BaseControlCRSelectFunction.BaseControlGroup.Select(p => p.Pollutant).ToList();// benMAPProject.LstPollutant;//污染物列表
                    RBenMAPGrid = BaseControlCRSelectFunction.RBenMapGrid;// benMAPProject.RBenMAPGrid;//系统选择的Grid

                    GBenMAPGrid = BaseControlCRSelectFunction.BaseControlGroup.First().GridType;//系统选择的Region
                    LstBaseControlGroup = BaseControlCRSelectFunction.BaseControlGroup;// benMAPProject.LstBaseControlGroup;//系统设置的DataSource Base and Control
                    //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                    CRThreshold = BaseControlCRSelectFunction.CRThreshold;// benMAPProject.CRThreshold;//阈值
                    CRLatinHypercubePoints = BaseControlCRSelectFunction.CRLatinHypercubePoints;//拉丁立体方采样点数
                    CRRunInPointMode = BaseControlCRSelectFunction.CRRunInPointMode;//是否使用拉丁立体方方采样
                    CRSeeds = BaseControlCRSelectFunction.CRSeeds;
                    BenMAPPopulation = BaseControlCRSelectFunction.BenMAPPopulation;//系统设置的Population

                    //Save--------------
                }
                else
                {
                    ManageSetup = benMAPProject.ManageSetup;
                    MainSetup = benMAPProject.MainSetup;// 当前活动区域
                    LstPollutant = benMAPProject.LstPollutant != null ? benMAPProject.LstPollutant : null;//污染物列表
                    RBenMAPGrid = benMAPProject.RBenMAPGrid != null ? benMAPProject.RBenMAPGrid : null;//系统选择的Grid

                    GBenMAPGrid = benMAPProject.GBenMAPGrid != null ? benMAPProject.GBenMAPGrid : null;//系统选择的Region
                    if (benMAPProject.LstBaseControlGroup != null)
                    {
                        LstBaseControlGroup = benMAPProject.LstBaseControlGroup;//系统设置的DataSource Base and Control
                        //------------update数据-------------------------
                        //foreach (BaseControlGroup bcg in LstBaseControlGroup)
                        //{
                        //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
                        //    DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
                        //}
                    }
                    //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                    CRThreshold = benMAPProject.CRThreshold;//阈值
                    CRLatinHypercubePoints = benMAPProject.CRLatinHypercubePoints;//拉丁立体方采样点数
                    CRRunInPointMode = benMAPProject.CRRunInPointMode;//是否使用拉丁立体方方采样
                    CRSeeds = benMAPProject.CRSeeds;

                    BenMAPPopulation = benMAPProject.BenMAPPopulation != null ? benMAPProject.BenMAPPopulation : null;//系统设置的Population

                    lstPollutantAll = benMAPProject.lstPollutantAll;//所有污染物
                }
                IncidencePoolingAndAggregationAdvance = benMAPProject.IncidencePoolingAndAggregationAdvance;
                BenMAPPopulation = benMAPProject.BenMAPPopulation;//系统设置的Population
                CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);

                //---------
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
            //return benMAPProject;
        }

        /// <summary>
        ///
        /// </summary>
        [Description("当从主窗体进入另外一个窗体后出发改变当前状态提示")]
        public static event FormChangedStatHandler FormChangedStat;

        protected static void OnFormChangedStat()
        {
            if (FormChangedStat != null)
            {
                FormChangedStat();
            }
        }//eventMethod


        private static string _nodeAnscyStatus = "";
        /// <summary>
        /// 记录节点异步状态:污染物名称；节点名称;是否开始：pm25;baseline;on   o3;baseline;off
        /// </summary>
        public static string NodeAnscyStatus
        {
            get { return _nodeAnscyStatus; }
            set
            {
                _nodeAnscyStatus = value;
                OnNodeAnscy();
            }
        }// NodeAnscy

        [Description("当从主窗体进入另外一个窗体后出发改变当前状态提示")]
        public static event FormChangedStatHandler NodeAnscy;
        protected static void OnNodeAnscy()
        {
            if (NodeAnscy != null)
            {
                NodeAnscy();
            }
        }//eventMethod

        /// <summary>
        ///
        /// </summary>
        [Description("改变Setup时发生")]
        public static event FormChangedStatHandler FormChangedSetup;

        protected static void OnFormChangedSetup()
        {
            if (FormChangedSetup != null)
            {
                FormChangedSetup();
            }
        }//eventMethod
    }//commonclass
    class Percentile<T> where T : IComparable
    {
        uint position, size, count;
        double percentile;
        private List<T> list;

        public List<T> List
        {
            get { return list; }
            //set { list = value; }
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
    /// <summary>
    ///  界面改变 ，做相应处理
    /// </summary>
    public delegate void FormChangedStatHandler();

    /// <summary>
    /// 异步生成shapefile
    /// </summary>
    /// <param name="bcg">生成ShapeFile所需数据</param>
    /// <param name="currentStat">当前操作节点：baseline/control</param>
    /// <param name="threadId"></param>
    /// <returns></returns>
    public delegate string AsyncDelegate(BaseControlGroup bcg, ModelDataLine m, string currentStat, out int threadId);

    /// <summary>
    /// 异步生成shapefile
    /// </summary>
    /// <param name="currentStat"></param>
    /// <param name="?"></param>
    /// <param name="threadId"></param>
    /// <returns></returns>
    public delegate string AsynDelegateRollBack(string currentStat, MonitorModelRollbackLine rollbackData, out int threadId);
    #region xml
    [Serializable]
    [ProtoContract]
    public class XMLSerializableDictionary<T, TValue> : Dictionary<T, TValue>, System.Xml.Serialization.IXmlSerializable
    {
        #region IXmlSerializable 成员
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
        #endregion
    #endregion
    #region other objects
    public class FieldCheck
    {
        public string FieldName;
        public bool isChecked;
    }
    #endregion
    #region project

    [Serializable]
    [ProtoContract]
    public class BenMAPProject
    {
        [ProtoMember(1)]
        public BenMAPSetup ManageSetup;
        [ProtoMember(2)]
        public BenMAPSetup MainSetup;// 当前活动区域
        [ProtoMember(3)]
        public List<BenMAPPollutant> LstPollutant;//污染物列表
        [ProtoMember(4)]
        public BenMAPGrid RBenMAPGrid;//系统选择的Grid

        [ProtoMember(5)]
        public BenMAPGrid GBenMAPGrid;//系统选择的Region
        [ProtoMember(6)]
        public List<BaseControlGroup> LstBaseControlGroup;//系统设置的DataSource Base and Control
        //[ProtoMember(1)]public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
        [ProtoMember(7)]
        public double CRThreshold = 0;//阈值
        [ProtoMember(8)]
        public int CRLatinHypercubePoints = 10;//拉丁立体方采样点数
        [ProtoMember(9)]
        public bool CRRunInPointMode = false;//是否使用拉丁立体方方采样
        [ProtoMember(10)]
        public int CRSeeds = 1;//种子数
        [ProtoMember(11)]
        public BenMAPPopulation BenMAPPopulation;//系统设置的Population

        [ProtoMember(12)]
        public List<BenMAPPollutant> lstPollutantAll;//所有污染物

        [ProtoMember(13)]
        public BaseControlCRSelectFunction BaseControlCRSelectFunction;//所有BaseControlAndCRSelectFunciton
        [ProtoMember(14)]
        public BaseControlCRSelectFunctionCalculateValue BaseControlCRSelectFunctionCalculateValue;//所有BaseControlAndCRSelectFunciton以及Value

        //-------------------APVX-------------------------------
        [ProtoMember(15)]
        public IncidencePoolingAndAggregationAdvance IncidencePoolingAndAggregationAdvance;//Advance
        [ProtoMember(16)]
        public List<IncidencePoolingAndAggregation> lstIncidencePoolingAndAggregation;
        //[ProtoMember(1)]public  List<ValuationMethodPoolingAndAggregation> lstValuationMethodPoolingAndAggregation;
        //[ProtoMember(1)]public  IncidencePoolingAndAggregation IncidencePoolingAndAggregation;//IncidencePooling;
        [ProtoMember(17)]
        public CRSelectFunctionCalculateValue IncidencePoolingResult;
        [ProtoMember(18)]
        public ValuationMethodPoolingAndAggregation ValuationMethodPoolingAndAggregation;
    }

    #endregion project
    #region GridRelationShip
    [Serializable]
    [ProtoContract]
    public class GRGridRelationShip
    {
        [ProtoMember(1)]
       public List<GridRelationship> lstRelationship;
    }
    #endregion

    #region setup

    [Serializable]
    [ProtoContract]
    public class BenMAPSetup
    {
        [ProtoMember(1)]
        public int SetupID;
        [ProtoMember(2)]
        public string SetupName;
    }

    #endregion setup

    #region Pollutant


    public enum ObservationtypeEnum
    {
        Hourly = 0, Daily = 1
    }

    /// <summary>
    /// 污染物
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class BenMAPPollutant
    {
        [ProtoMember(1)]
        public int PollutantID;//POLLUTANTID;
        [ProtoMember(2)]
        public string PollutantName;//POLLUTANTNAME;
        [ProtoMember(3)]
        public ObservationtypeEnum Observationtype;
        [ProtoMember(4)]
        public List<Metric> Metrics;
        [ProtoMember(5)]
        public List<Season> Seasons;
        [ProtoMember(6)]
        public List<SeasonalMetric> SesonalMetrics=new List<SeasonalMetric>();
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
        public int HourlyMetricGeneration;//0-Hourly 1-Daily
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

    #endregion Pollutant

    #region Grid


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

    #endregion Grid

    #region Model

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
        public Dictionary<string, float> Values;//key为Metric名称（包括Metric和SeasonalMetric)
        //private List<MetricValueAttributes> _attributes = new List<MetricValueAttributes>();
        //[ProtoMember(1)]
        //public List<MetricValueAttributes> Attributes
        //{
        //    get
        //    {
        //        if (Values.Count > 0)
        //        {

        //            foreach (string key in Values.Keys)
        //            {
        //                _attributes.Add(new MetricValueAttributes() { MetricName = key, MetricValue = Values[key] });
        //            }
        //            return _attributes;
        //        }
        //        return _attributes;
        //    }
        //}
        // [ProtoMember(1)]
        //public List<MetricValueAttributes> Values;
    }

    /// <summary>
    /// Base Control 的基类
    /// </summary>
    [Serializable]
    [ProtoContract]
    [ProtoInclude(7, typeof(ModelDataLine))]
    [ProtoInclude(8, typeof(MonitorDataLine))]
    //[ProtoInclude(9, typeof(MonitorModelRelativeLine))]
    //[ProtoInclude(10, typeof(MonitorModelRollbackLine))]
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
        //[ProtoMember(5)]
        //public List<float[]> ResultCopy;
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
        public int MonitorDirectType; //0 Library 1 Text File
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
        public double FilterMinLongitude=-130;
        [ProtoMember(9)]
        public double FilterMaxLongitude=-65;
        [ProtoMember(10)]
        public double FilterMinLatitude=20;
        [ProtoMember(11)]
        public double FilterMaxLatitude=55;
        [ProtoMember(12)]
        public int FilterMaximumPOC=-1;
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
        //-For Ozone--
        public int StartHour=8;
        public int EndHour=19;
        public int StartDate=120;
        public int EndDate=272;
        public int NumberOfValidHour=9;
        public int PercentOfValidDays=50;
        //------For PM2.5 PM10
        public int NumberOfPerQuarter=11;
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
            return x.Col.Equals(y.Col) && x.Row.Equals(y.Row);// x.UserId.Equals(y.UserId);
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

    /// <summary>
    ///  每个rollback类型对应一个对象
    /// </summary>
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
        public RollbackType RollbackType;  //削减的类别
        [ProtoMember(3)]
        public double Background;
        [ProtoMember(4)]
        public List<RowCol> SelectRegions;
        [ProtoMember(5)]
        public string DrawingColor;// = System.Drawing.Color.Red;  //颜色
    }

    [Serializable]
    [ProtoContract]
    public class PercentageRollback : BenMAPRollback
    {
        // [ProtoMember(1)]public int RegionID;
        [ProtoMember(1)]
        public double Percent;
        //[ProtoMember(1)]public double Background;
        //[ProtoMember(1)]public List<RowCol> SelectRegions;
    }

    [Serializable]
    [ProtoContract]
    public class IncrementalRollback : BenMAPRollback
    {
        //[ProtoMember(1)]public int RegionID;
        [ProtoMember(1)]
        public double Increment;
        //[ProtoMember(1)]public double Background;
        //[ProtoMember(1)]public List<RowCol> SelectRegions;
    }

    [Serializable]
    [ProtoContract]
    public class StandardRollback : BenMAPRollback
    {
        //[ProtoMember(1)]public int RegionID;
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

        //[ProtoMember(1)]public double Background;
    }

    /// <summary>
    /// 需以后根据需求更改
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class MonitorModelRollbackLine : MonitorDataLine
    {
        [ProtoMember(1)]
        public BenMAPGrid RollbackGrid;
        [ProtoMember(2)]
        public List<BenMAPRollback> BenMAPRollbacks;
        [ProtoMember(3)]
        public int ScalingMethod;//0- None 1-SpatialOnly;
        [ProtoMember(4)]
        public BenMAPGrid AdditionalGrid;
        [ProtoMember(5)]
        public string AdustmentFilePath;
        [ProtoMember(6)]
        public bool isMakeBaseLineGrid;
    }

    #endregion Model

    #region Monitor

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
        //[ProtoMember(1)]public int Year;

        [ProtoMember(1)]
        public string MonitorName;
        [ProtoMember(2)]
        public string MonitorMethod;
        [ProtoMember(3)]
        public double Latitude;//纬度
        [ProtoMember(4)]
        public double Longitude;//经度
        [ProtoMember(5)]
        public int Row;//行索引
        [ProtoMember(6)]
        public int Col;// 列索引
        [ProtoMember(7)]
        public Metric Metric;

        [ProtoMember(8)]
        public SeasonalMetric SeasonalMetric;

        [ProtoMember(9)]
        public string Statistic;
        //[ProtoMember(1)]public List<MetricValueAttributes> dicMetricValues;
        //[XmlIgnore]
        [ProtoMember(10)]
        public Dictionary<string, float> dicMetricValues;//用来保存所有的Metric值
        [ProtoMember(11)]
        public Dictionary<string, List<float>> dicMetricValues365;//用来保存365天的连续值
        //private List<MetricValueAttributes> _attributes = new List<MetricValueAttributes>();
        //[ProtoMember(1)]
        //public List<MetricValueAttributes> Attributes
        //{
        //    get
        //    {
        //        if (Values.Count > 0)
        //        {

        //            foreach (string key in dicMetricValues.Keys)
        //            {
        //                _attributes.Add(new MetricValueAttributes() { MetricName = key, MetricValue = dicMetricValues[key] });
        //            }
        //            return _attributes;
        //        }
        //        return _attributes;
        //    }
        //}
        //        [ProtoMember(1)]
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
    #endregion Monitor

    #region Population

    [Serializable]
    [ProtoContract]
    public class PopulationAttribute
    {
        [ProtoMember(1)]
        public int Row;

        [ProtoMember(2)]
        public int Col;

        //[ProtoMember(1)]public int Year;

        //[ProtoMember(1)]public int RaceID;

        //[ProtoMember(1)]public int GenderID;

        //[ProtoMember(1)]public int EthnicityID;
        //[ProtoMember(1)]public int AgeRangeID;
        //[ProtoMember(1)]public int StartAge;
        //[ProtoMember(1)]public int EndAge;
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

        //[ProtoMember(1)]public int Year;

        //[ProtoMember(1)]public int RaceID;

        //[ProtoMember(1)]public int GenderID;

        //[ProtoMember(1)]public int EthnicityID;
        //[ProtoMember(1)]public int AgeRangeID;
        //[ProtoMember(1)]public int StartAge;
        //[ProtoMember(1)]public int EndAge;
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
        // [ProtoMember(1)]public List<PopulationAttribute> PopulationAttributes;
    }

    #endregion Population

    #region Configuration


    public enum MetricStatic
    {
        None = 0, Mean = 1, Median = 2, Max = 3, Min = 4, Sum = 5
    }

    //[ProtoMember(1)]
    //public enum WindowsStatic
    //{
    //     Mean = 1, Median = 2, Max = 3, Min = 4, Sum = 5
    //}
    //[ProtoMember(1)]
    //public enum DailyStatic
    //{
    //     Mean = 1, Median = 2, Max = 3, Min = 4, Sum = 5
    //}
    [Serializable]
    [ProtoContract]
    public class Location
    {
        [ProtoMember(1)]
        public int LocationType;//GridID
        [ProtoMember(2)]
        public int GridDifinitionID;//冗余
        [ProtoMember(3)]
        public int LocationID;
        [ProtoMember(4)]
        public int Col;//冗余字段
        [ProtoMember(5)]
        public int Row;//冗余字段
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
        //[ProtoMember(1)]public int bigGridID;
        //[ProtoMember(1)]public int smallGridID;
        [ProtoMember(1)]
        public RowCol bigGridRowCol;
        [ProtoMember(2)]
        public List<RowCol> smallGridRowCol;
    }
    [Serializable]
    [ProtoContract]
    public class GridRelationshipAttributePercentage
    {
        //[ProtoMember(1)]public int bigGridID;
        //[ProtoMember(1)]public int smallGridID;
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
        //[ProtoMember(1)]public int IncidenceRateID;
        [ProtoMember(1)]
        public int Col;
        [ProtoMember(2)]
        public int Row;
        [ProtoMember(3)]
        public float Value;
        //[ProtoMember(1)]public int StartAge;
        //[ProtoMember(1)]public int EndAge;
        //[ProtoMember(1)]public int RaceID;
        //[ProtoMember(1)]public int EthnicityID;
        //[ProtoMember(1)]public int GenderID;
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
        public string DataSetName;//有选择，可能要换
        [ProtoMember(4)]
        public int EndPointGroupID;
        [ProtoMember(5)]
        public string EndPointGroup;//有选择，可能要换
        [ProtoMember(6)]
        public int EndPointID;
        [ProtoMember(7)]
        public string EndPoint;//有选择，可能要换
        [ProtoMember(8)]
        public BenMAPPollutant Pollutant;
        [ProtoMember(9)]
        public Metric Metric;
        [ProtoMember(10)]
        public MetricStatic MetricStatistic;
        [ProtoMember(11)]
        public SeasonalMetric SeasonalMetric;
        [ProtoMember(12)]
        public string Race;//有选择，可能要换
        [ProtoMember(13)]
        public string Ethnicity;//有选择，可能要换
        [ProtoMember(14)]
        public string Gender;//有选择，可能要换
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
        public string BetaDistribution;//有选择，可能要换
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
    public class CRSelectFunction //: BenMAPHealthImpactFunction
    {
        [ProtoMember(1)]
        public int CRID;
        [ProtoMember(2)]
        public BenMAPHealthImpactFunction BenMAPHealthImpactFunction;
        [ProtoMember(3)]
        public List<Location> Locations;
        [ProtoMember(4)]
        public string Race;//有选择，可能要换
        [ProtoMember(5)]
        public string Ethnicity;//有选择，可能要换
        [ProtoMember(6)]
        public string Gender;//有选择，可能要换
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
        public List<LatinPoints> lstLatinPoints;//种子
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
        //[ProtoMember(1)]public List<double> lstLatin;
        [ProtoMember(2)]
        public List<CRCalculateValue> CRCalculateValues;

        //[ProtoMember(3)]
        //public List<float[]> ResultCopy;
    }

    [Serializable]
    [ProtoContract]
    public class CRCalculateValue
    {
        //[ProtoMember(1)]public CRSelectFunction CRSelectFunction;
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
        public double CRThreshold = 0;//阈值
        [ProtoMember(4)]
        public int CRLatinHypercubePoints = 10;//拉丁立体方采样点数
        [ProtoMember(5)]
        public bool CRRunInPointMode = false;//是否使用拉丁立体方方采样
        
        [ProtoMember(6)]
        public BenMAPPopulation BenMAPPopulation;//系统设置的Population
        [ProtoMember(7)]
        public BenMAPGrid RBenMapGrid;
        [ProtoMember(8)]
        public int CRSeeds=1;
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
        public double CRThreshold = 0;//阈值
        [ProtoMember(4)]
        public int CRLatinHypercubePoints = 10;//拉丁立体方采样点数
        [ProtoMember(5)]
        public bool CRRunInPointMode = false;//是否使用拉丁立体方方采样

        [ProtoMember(6)]
        public BenMAPPopulation BenMAPPopulation;//系统设置的Population
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

    #endregion Configuration

    #region APVX

    //------------QALY--------------------
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

        //------add for display
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
        //----- add for display


        // [ProtoMember(1)]public string Function;
        [ProtoMember(22)]
        public int NodeType;//0-endPointGroup;1-endPointID;2-Author;3-ValuationFunction;4-All
        [ProtoMember(23)]
        public int ID;//按顺序的ID
        [ProtoMember(24)]
        public int PID;//父节点ID
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
        //[ProtoMember(2)]
        //public List<float[]> arrayQALYValueAttributes;
        //[ProtoMember(1)]public List<double> lstQALYMonteCarlo;
        //[ProtoMember(1)]public List<double[]> ResultCopy;
    }

    [Serializable]
    [ProtoContract]
    public class AllSelectValuationMethod
    {
        [ProtoMember(1)]
        public int CRIndex = -1;
        [ProtoMember(2)]
        public string Name;
        //------add for display
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
        //----- add for display
        //----- add for display
        [ProtoMember(22)]
        public string PoolingMethod;
        [ProtoMember(23)]
        public int NodeType;//0-endPointGroup;1-endPointID;2-Author;3-ValuationFunction;4-All //------modify by xiejp --3-Qulify;4-CRFunction 5-ValuationFunction
        [ProtoMember(24)]
        public int ID;//按顺序的ID
        [ProtoMember(25)]
        public int PID;//父节点ID
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

        //------add for display
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
        //----- add for display

        [ProtoMember(24)]
        public int NodeType;//0-endPointGroup;1-endPointID;2-Author;3-Qulify;4-CRFunction;
        [ProtoMember(25)]
        public int ID;//按顺序的ID
        [ProtoMember(26)]
        public int PID;//父节点ID
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
        //[ProtoMember(3)]
        //public List<float[]> ResultCopy;
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
        public string RandomSeed="1" ;
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
        //[ProtoMember(1)]public PoolingMethodTypeEnum PoolingMethodType;
        //[ProtoMember(1)]public List<CRSelectFunctionCalculateValue> PoolingMethods;
        [ProtoMember(2)]
        public List<string> lstColumns;
        [ProtoMember(3)]
        public List<AllSelectCRFunction> lstAllSelectCRFuntion;
        [ProtoMember(4)]
        public List<double> Weights;
        [ProtoMember(5)]
        public string ConfigurationResultsFilePath;
        //[ProtoMember(1)]public IncidencePoolingAndAggregationAdvance IncidencePoolingAndAggregationAdvance;
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
        public string DataSet;//有选择，可能要换
        [ProtoMember(3)]
        public int EndPointGroupID;
        [ProtoMember(4)]
        public string EndPointGroup;//有选择，可能要换
        [ProtoMember(5)]
        public int EndPointID;
        [ProtoMember(6)]
        public string EndPoint;//有选择，可能要换

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
        public string NameA;//有选择，可能要换

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

    /// <summary>
    /// 待需求确定再修改
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class ValuationMethodPoolingAndAggregation
    {
        //[ProtoMember(1)]public string PoolingName;
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
        public int VariableDatasetID=-1;
        [ProtoMember(8)]
        public string Version;
        [ProtoMember(9)]
        public string VariableDatasetName = "";
        //        //[ProtoMember(1)]
        //public IncidencePoolingAndAggregation IncidencePoolingAndAggregation;
        //        //[ProtoMember(1)]
        //public List<AllSelectValuationMethod> LstAllSelectValuationMethod;
        //        //[ProtoMember(1)]
        //public List<AllSelectValuationMethodAndValue> LstAllSelectValuationMethodAndValue;
        //        //[ProtoMember(1)]
        //public List<AllSelectQALYMethod> lstAllSelectQALYMethod;
        //        //[ProtoMember(1)]
        //public List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue;
        //        //[ProtoMember(1)]
        //public CRSelectFunctionCalculateValue IncidencePoolingResult;
        //        //[ProtoMember(1)]
        //public List<CRSelectFunctionValuationFunction> lstCRSelectFunctionValuationFunction;
        //        //[ProtoMember(1)]
        //public List<APVValueAttribute> lstAPVValueAttributes;
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

    #endregion APVX

    # region Result

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

    #endregion

    #region databaseExport
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
    #endregion

    #region Batch
    public class BatchBase
    {
       public string ActiveSetup;
       public ArrayList BatchText;
    }
    public class BatchAQGBase:BatchBase
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
        public string MonitorDataType;//MonitorDataType must be one of Library or TextFile.
        public string InterpolationMethod;
        public string MonitorDataSet;
        public int MonitorYear;
        public string MonitorFile;
        public double MaxDistance=-1;
        public double MaxRelativeDistance=-1;
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
        public int Year=-1;
        public int LatinHypercubePoints=-1;
        public int Seeds = 1;
        public double Threshold=-1;
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
        public  string Totals;
        public string GridFields;
        public string CustomFields;
        public string ResultFields;
      
    }

    #endregion

}