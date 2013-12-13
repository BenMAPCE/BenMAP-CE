using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using System.Threading;
using FirebirdSql.Data.FirebirdClient;
using System.Text.RegularExpressions;

namespace BenMAP
{
    public partial class PopulationMap : FormBase
    {
        private string _popDataset;

        public string PopDataset
        {
            get { return _popDataset; }
            set { _popDataset = value; }
        }

        private string _popYear;

        public string PopYear
        {
            get { return _popYear; }
            set { _popYear = value; }
        }

        Dictionary<string, Dictionary<string, Dictionary<string, float>>> dicAllPopData = new Dictionary<string, Dictionary<string, Dictionary<string, float>>>();
        Dictionary<int, Dictionary<string, double>> dicAllGrowth = new Dictionary<int, Dictionary<string, double>>();
        Dictionary<string, Dictionary<string, Dictionary<string, WeightAttribute>>> dicAllWeight = new Dictionary<string, Dictionary<string, Dictionary<string, WeightAttribute>>>();
        private double _dMinValue = 0.0;//最小值
        private double _dMaxValue = 0.0;// 最大值
        private int _currentLayerIndex = 1;
        private string _columnName = string.Empty;
        private Color[] _blendColors;
        Dictionary<string, string> dicAgeRange = new Dictionary<string, string>();
        Dictionary<string, string> dicRace = new Dictionary<string, string>();
        Dictionary<string, string> dicEthnicity = new Dictionary<string, string>();
        Dictionary<string, string> dicGender = new Dictionary<string, string>();
        DotSpatial.Data.IFeatureSet fs = new DotSpatial.Data.FeatureSet();

        PopulationInformation frmPopInfo;
        bool frmPopInfoClose = true;

        public bool FrmPopInfoClose
        {
            get { return frmPopInfoClose; }
            set { frmPopInfoClose = value; }
        }


        public PopulationMap()
        {
            InitializeComponent();
        }

        private void PopulationMap_Load(object sender, EventArgs e)
        {
            try
            {
                //WaitShow("Waiting...");
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Empty;
                commandText = string.Format("select * from PopulationDataSets where PopulationDataSetID<>37 and  SetupID={0} order by PopulationDataSetID", CommonClass.MainSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboPopulationDataSet.DataSource = ds.Tables[0];
                cboPopulationDataSet.DisplayMember = "PopulationDataSetName";
                cboPopulationDataSet.Text = _popDataset;
                cboPopulationYear.Text = _popYear;
                commandText = "select AgeRangeID,AgeRangeName from AgeRanges";
                DataTable dt = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dicAgeRange.Add(dt.Rows[i]["AgeRangeID"].ToString(), dt.Rows[i]["AgeRangeName"].ToString());
                }
                commandText = "select GenderID,GenderName from Genders";
                dt = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dicGender.Add(dt.Rows[i]["GenderID"].ToString(), dt.Rows[i]["GenderName"].ToString());
                }
                commandText = "select EthnicityID,EthnicityName from Ethnicity";
                dt = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dicEthnicity.Add(dt.Rows[i]["EthnicityID"].ToString(), dt.Rows[i]["EthnicityName"].ToString());
                }
                commandText = "select RaceID,RaceName from Races";
                dt = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dicRace.Add(dt.Rows[i]["RaceID"].ToString(), dt.Rows[i]["RaceName"].ToString());
                }
                List<string> keys = dicGender.Where(q => q.Value == "").Select(q => q.Key).ToList();
                if (keys.Count > 0)
                    dicGender[keys[0]] = "A";
                keys = dicEthnicity.Where(q => q.Value == "").Select(q => q.Key).ToList();
                if (keys.Count > 0)
                    dicEthnicity[keys[0]] = "A";
                keys = dicRace.Where(q => q.Value == "").Select(q => q.Key).ToList();
                if (keys.Count > 0)
                    dicRace[keys[0]] = "A";
                //DrawPop();
                //WaitClose();
                colorBlend.CustomizeValueRange -= ResetGisMap;
                colorBlend.CustomizeValueRange += ResetGisMap;
            }
            catch
            { }
        }

        private void cboPopulationDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Empty;
                List<string> lstYear = new List<string>();
                DataRowView drv = cboPopulationDataSet.SelectedItem as DataRowView;
                int PopulationDataSetID = Convert.ToInt32(drv["PopulationDataSetID"]);
                commandText = string.Format("select distinct Yyear from t_PopulationDataSetIDYear  where PopulationDataSetID={0}", PopulationDataSetID);
                //cboPopulationYear.Items.Clear();
                //if (CommonClass.MainSetup.SetupID == 1 && Convert.ToInt16(drv["PopulationConfigurationID"]) == 1)//----------美国数据里面包含了人口增长数据，基数数据为2010
                //{
                //    DataTable dtYear = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                //    commandText = string.Format("select min(Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID=30");
                //    int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                //    bool growth = false;
                //    for (int i = 0; i < dtYear.Rows.Count; i++)
                //    {
                //        if (Convert.ToInt16(dtYear.Rows[i][0]) == commonYear)
                //        {
                //            string strWeightCount = "select count(*) from PopulationGrowthWeights where PopulationDataSetID=" + PopulationDataSetID + " and YYear=" + commonYear;
                //            int weightCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strWeightCount));
                //            if (weightCount > 0 || Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"])).GridDefinitionID == 18)
                //                growth = true;
                //            else
                //                cboPopulationYear.Items.Add(dtYear.Rows[i][0]);
                //        }
                //        else
                //        {
                //            cboPopulationYear.Items.Add(dtYear.Rows[i][0]);
                //        }
                //    }
                //    if (growth)
                //    {
                //        commandText = string.Format("select distinct Yyear from t_PopulationDataSetIDYear where PopulationDataSetID in(select PopulationDataSetID from PopulationDataSets where    SetupID={0})", CommonClass.MainSetup.SetupID);// where PopulationDataSetID={0}", PopulationDataSetID);
                //        dtYear = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                //        for (int i = 0; i < dtYear.Rows.Count; i++)
                //        {
                //            if (!cboPopulationYear.Items.Contains(dtYear.Rows[i][0]))
                //                cboPopulationYear.Items.Add(dtYear.Rows[i][0]);
                //        }
                //    }
                //}
                //else
                //{
                //    DataSet dsYear = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                //    for (int i = 0; i < dsYear.Tables[0].Rows.Count; i++)
                //    {
                //        if (!cboPopulationYear.Items.Contains(dsYear.Tables[0].Rows[i][0]))
                //            cboPopulationYear.Items.Add(dsYear.Tables[0].Rows[i][0]);
                //    }
                //}
                if (CommonClass.MainSetup.SetupID == 1 && Convert.ToInt16(drv["APPLYGROWTH"]) == 1)
                {
                    commandText = string.Format("select distinct Yyear from t_PopulationDataSetIDYear where PopulationDataSetID in (select PopulationDataSetID from PopulationDataSets where SetupID={0})", CommonClass.MainSetup.SetupID);
                }
                else
                {
                    commandText = string.Format("select distinct Yyear from t_PopulationDataSetIDYear where PopulationDataSetID={0}", PopulationDataSetID);
                }
                DataTable dtYear = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                cboPopulationYear.DataSource = dtYear;
                cboPopulationYear.DisplayMember = "Yyear";
                cboPopulationYear.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void DrawPop()
        {
            try
            {
                WaitShow("Loading population...");
                //prgLoadPOP.Value = 0;
                //prgLoadPOP.Visible = true;
                //lblProgress.Text = "Loading population...";
                //lblProgress.Visible = true;
                //this.Refresh();

                string commandText = string.Empty;
                DataRowView drv = cboPopulationDataSet.SelectedItem as DataRowView;
                BenMAPPopulation benMAPPOP = new BenMAPPopulation()
                {
                    DataSetID = Convert.ToInt32(drv["PopulationDataSetID"]),
                    DataSetName = drv["PopulationDataSetName"].ToString(),
                    GridType = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"])),
                    PopulationConfiguration = Convert.ToInt32(drv["PopulationConfigurationID"]),
                    Year = Convert.ToInt32(cboPopulationYear.Text)
                };
                Dictionary<string, Dictionary<string, float>> dicPopulationAgeIn = new Dictionary<string, Dictionary<string, float>>();
                if (dicAllPopData.ContainsKey(cboPopulationDataSet.Text + "," + cboPopulationYear.Text))
                {
                    dicPopulationAgeIn = dicAllPopData[cboPopulationDataSet.Text + "," + cboPopulationYear.Text];
                }
                else
                {
                    getPopulationDataSetFromCRSelectFunction(ref dicPopulationAgeIn, benMAPPOP);
                    dicPopulationAgeIn = dicPopulationAgeIn.OrderBy(q => int.Parse(Regex.Match(q.Key, @"\d+").Value)).ToDictionary(o => o.Key, p => p.Value);
                    if (!dicAllPopData.ContainsKey(cboPopulationDataSet.Text + "," + cboPopulationYear.Text))
                        dicAllPopData.Add(cboPopulationDataSet.Text + "," + cboPopulationYear.Text, dicPopulationAgeIn);
                }
                cboAgeRange.Items.Clear();
                //List<string> lstAddField = new List<string>();
                //foreach (string s in dicPopulationAgeIn.Keys)
                //{
                //    string[] rgea = s.Split(',');
                //    lstAddField.Add(dicRace[rgea[0]] + "_" + dicGender[rgea[1]] + "_" + dicEthnicity[rgea[2]] + "_" + dicAgeRange[rgea[3]]);
                //}
                //lstAddField.Sort();
                //for (int j = 0; j < lstAddField.Count; j++)
                //{
                //    cboAgeRange.Items.Add(dicAgeRange[lstAddField[j]]);
                //}
                List<float[]> lstResultCopy = new List<float[]>();
                List<float> lstd = new List<float>();
                fs = new DotSpatial.Data.FeatureSet();
                string AppPath = Application.StartupPath;
                string shapefilename = string.Empty;
                if (benMAPPOP.GridType.TType == GridTypeEnum.Shapefile)
                    shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPPOP.GridType as ShapefileGrid).ShapefileName + ".shp";
                else if (benMAPPOP.GridType.TType == GridTypeEnum.Regular)
                    shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPPOP.GridType as RegularGrid).ShapefileName + ".shp";
                fs = DotSpatial.Data.FeatureSet.Open(shapefilename);

                int i = 0;
                int iCol = 0;
                int iRow = 0;
                List<string> lstRemoveName = new List<string>();
                while (i < fs.DataTable.Columns.Count)
                {
                    if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col" || fs.DataTable.Columns[i].ColumnName.ToLower() == "row")
                    { }
                    else
                        lstRemoveName.Add(fs.DataTable.Columns[i].ColumnName);

                    i++;
                }
                foreach (string s in lstRemoveName)
                {
                    fs.DataTable.Columns.Remove(s);
                }
                i = 0;
                while (i < fs.DataTable.Columns.Count)
                {
                    if (fs.DataTable.Columns[i].ColumnName.ToLower() == "col") iCol = i;
                    if (fs.DataTable.Columns[i].ColumnName.ToLower() == "row") iRow = i;

                    i++;
                }
                i = 0;
                WaitChangeMsg("Adding data to shapefile...");
                //lblProgress.Text = "Adding data to shapefile...";
                //prgLoadPOP.Value = 0;
                //tspPOPMap.Refresh();
                //prgLoadPOP.Maximum = lstAddField.Count * fs.DataTable.Rows.Count;
                //for (int j = 0; j < lstAddField.Count; j++)
                foreach (string k in dicPopulationAgeIn.Keys)
                {
                    string[] rgea = k.Split(',');
                    string addField = dicRace[rgea[0]].Substring(0, 1) + "_" + dicEthnicity[rgea[2]].Substring(0, 1) + "_" + dicGender[rgea[1]].Substring(0, 1) + "_" + dicAgeRange[rgea[3]];
                    cboAgeRange.Items.Add(addField);
                    fs.DataTable.Columns.Add(addField, typeof(double));
                }
                foreach (DataRow dr in fs.DataTable.Rows)
                {
                    object[] o = new object[fs.DataTable.Columns.Count];
                    o[0] = dr[0]; o[1] = dr[1];
                    for (int idr = 2; idr < fs.DataTable.Columns.Count; idr++)
                    {
                        o[idr] = 0.0;
                    }
                    dr.ItemArray = o;
                }
                i = 0;
                List<string> lstAgeInKeys=dicPopulationAgeIn.Keys.ToList();
                foreach (DataRow dr in fs.DataTable.Rows)
                {
                    object[] o = new object[fs.DataTable.Columns.Count];
                    o[0] = dr[iCol]; o[1] = dr[iRow];
                    string s = dr[iCol] + "," + dr[iRow];
                    for (int idr = 2; idr < fs.DataTable.Columns.Count; idr++)
                    {
                        if(dicPopulationAgeIn[lstAgeInKeys[idr-2]].ContainsKey(s) &&!double.IsNaN(dicPopulationAgeIn[lstAgeInKeys[idr-2]][s]))
                        o[idr] = Math.Round(dicPopulationAgeIn[lstAgeInKeys[idr-2]][s], 4);
                    }
                    dr.ItemArray = o;
                    //string s = fs.DataTable.Rows[i][iCol] + "," + fs.DataTable.Rows[i][iRow];
                    //if (dicPopulationAgeIn[k].Keys.Contains(s))
                    //{
                    //    fs.DataTable.Rows[i][addField] = Math.Round(dicPopulationAgeIn[k][s], 4);
                    //    if (double.IsNaN(dicPopulationAgeIn[k][s]))
                    //        fs.DataTable.Rows[i][addField] = Convert.ToDouble(0.0000);
                    //}
                    //else
                    //    fs.DataTable.Rows[i][addField] = Convert.ToDouble(0.0000);

                    i++;
                    //prgLoadPOP.PerformStep();
                }
                //foreach(string k in dicPopulationAgeIn.Keys)
                //{
                //    try
                //    {
                //        string[] rgea = k.Split(',');
                //        string addField = dicRace[rgea[0]].Substring(0, 1) + "_" + dicEthnicity[rgea[2]].Substring(0, 1) + "_" + dicGender[rgea[1]].Substring(0, 1) + "_" + dicAgeRange[rgea[3]];

                //        i = 0;
                //        while (i < fs.DataTable.Rows.Count)
                //        {
                //            string s = fs.DataTable.Rows[i][iCol] + "," + fs.DataTable.Rows[i][iRow];
                //            if (dicPopulationAgeIn[k].Keys.Contains(s))
                //            {
                //                fs.DataTable.Rows[i][addField] = Math.Round(dicPopulationAgeIn[k][s], 4);
                //                if (double.IsNaN(dicPopulationAgeIn[k][s]))
                //                    fs.DataTable.Rows[i][addField] = Convert.ToDouble(0.0000);
                //            }
                //            else
                //                fs.DataTable.Rows[i][addField] = Convert.ToDouble(0.0000);

                //            i++;
                //            //prgLoadPOP.PerformStep();
                //        }
                //    }
                //    catch
                //    { }
                //}
                dicPopulationAgeIn.Clear();
                mainMap.Layers.Clear();
                GC.Collect();
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;

                //string newshapefilename = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, "POP" + cboPopulationDataSet.Text + cboPopulationYear.Text + ".shp");
                //fs.SaveAs(newshapefilename, true);
                mainMap.Layers.Add(fs);
                cboAgeRange.SelectedIndex = cboAgeRange.Items.Count - 1;
                //DrawOneAge();
                WaitClose();
                addRegionLayerToMainMap();
            }
            catch
            {
                prgLoadPOP.Visible = false;
                lblProgress.Visible = false;
                prgLoadPOP.Value = 0;
                lblProgress.Text = "";
                WaitClose();
            }
        }

        private void DrawOneAge()
        {
            try
            {
                prgLoadPOP.Visible = true;
                lblProgress.Visible = true;
                lblProgress.Text = "Drawing...";
                tspPOPMap.Refresh();

                MapPolygonLayer polLayer = mainMap.Layers[0] as MapPolygonLayer;
                string strValueField = cboAgeRange.Text;
                (mainMap.Layers[0] as MapPolygonLayer).LegendText = cboPopulationDataSet.Text + " (" + cboPopulationYear.Text + ")";
                PolygonScheme myScheme1 = new PolygonScheme();
                float fl = (float)0.1;
                myScheme1.EditorSettings.StartColor = Color.Blue;
                myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                myScheme1.EditorSettings.EndColor = Color.Red;
                myScheme1.EditorSettings.EndColor.ToTransparent(fl);
                myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                myScheme1.EditorSettings.NumBreaks = 6;
                myScheme1.EditorSettings.FieldName = strValueField;
                myScheme1.EditorSettings.UseGradient = false;
                myScheme1.CreateCategories(polLayer.DataSet.DataTable);
                double dMinValue = 0.0;
                double dMaxValue = 0.0;
                object min = fs.DataTable.Compute("min([" + strValueField + "])", "");
                object max = fs.DataTable.Compute("max([" + strValueField + "])", "");
                if (!(min is DBNull))
                    dMinValue = Convert.ToDouble(min);
                if (!(max is DBNull))
                    dMaxValue = Convert.ToDouble(max);
                if (double.IsNaN(dMinValue)) dMinValue = 0;
                if (double.IsNaN(dMaxValue)) dMaxValue = 0;

                _currentLayerIndex = 0;
                _dMinValue = dMinValue;
                _dMaxValue = dMaxValue;
                _columnName = strValueField;
                RenderMainMap(true);

                prgLoadPOP.Visible = false;
                lblProgress.Visible = false;
                prgLoadPOP.Value = 0;
                lblProgress.Text = "";
            }
            catch
            {
                prgLoadPOP.Visible = false;
                lblProgress.Visible = false;
                prgLoadPOP.Value = 0;
                lblProgress.Text = "";
            }
        }

        private void addRegionLayerToMainMap()
        {
            try
            {
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;
                if (CommonClass.RBenMAPGrid is ShapefileGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                    {
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                    }
                }
                else if (CommonClass.RBenMAPGrid is RegularGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                    {
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                    }
                }
                mainMap.Layers[mainMap.Layers.Count() - 1].LegendText = CommonClass.rBenMAPGrid.GridDefinitionName;
                PolygonLayer playerRegion = mainMap.Layers[mainMap.Layers.Count - 1] as PolygonLayer;
                Color cRegion = Color.Transparent;
                PolygonSymbolizer TransparentRegion = new PolygonSymbolizer(cRegion);

                TransparentRegion.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);
                playerRegion.Symbolizer = TransparentRegion;
            }
            catch
            {
            }
        }

        private void RenderMainMap(bool isCone)
        {
            double min = _dMinValue;
            double max = _dMaxValue;
            colorBlend.SetValueRange(min, max, true);
            colorBlend._minPlotValue = _dMinValue;
            colorBlend._maxPlotValue = _dMaxValue;
            ResetGisMap(null, null);
            return;
        }

        private void ResetGisMap(object sender, EventArgs e)
        {
            try
            {
                _blendColors = colorBlend.ColorArray;
                _dMaxValue = colorBlend.MaxValue;
                _dMinValue = colorBlend.MinValue;
                colorBlend.SetValueRange(_dMinValue, _dMaxValue, false);
                //colorBlend._minPlotValue = _dMinValue;
                //colorBlend._maxPlotValue = _dMaxValue;
                Color[] colors = new Color[_blendColors.Length];
                _blendColors.CopyTo(colors, 0);
                //Quantities+半透明
                PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                int iColor = 0;
                //PolygonScheme pgs = (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology as PolygonScheme;
                string ColumnName = _columnName;
                PolygonScheme myScheme1 = new PolygonScheme();
                float fl = (float)0.1;
                myScheme1.EditorSettings.StartColor = Color.Blue;
                myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                myScheme1.EditorSettings.EndColor = Color.Red;
                myScheme1.EditorSettings.EndColor.ToTransparent(fl);
                float fColor = (float)0.2;
                Color ctemp = new Color();
                myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                myScheme1.EditorSettings.NumBreaks = 6;
                myScheme1.EditorSettings.FieldName = _columnName;// "Value";
                myScheme1.EditorSettings.UseGradient = false;
                myScheme1.CreateCategories((mainMap.Layers[_currentLayerIndex] as IFeatureLayer).DataSet.DataTable);
                if (myScheme1.Categories.Count == 1)
                {

                    //pc.Symbolizer.SetOutlineWidth(0);
                    PolygonSymbolizer ps = new PolygonSymbolizer();
                    ps.SetFillColor(colors[iColor]);
                    ps.SetOutline(Color.Transparent, 0);
                    //player.Symbology = myScheme1;

                    (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbolizer = ps;
                    return;

                }
                iColor = 0;
                //foreach (PolygonCategory pc in myScheme1.Categories)
                prgLoadPOP.Maximum = 6;
                for (int iBlend = 0; iBlend < 6; iBlend++)
                {
                    //pc.Symbolizer.SetOutlineWidth(0);
                    PolygonCategory pcin = new PolygonCategory();
                    double dnow = 0;// Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.00) * Convert.ToDouble(iColor), 3);
                    double dnowUp = 0;// Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.00) * Convert.ToDouble(iColor + 1), 3);
                    //------------实现value--
                    dnow = colorBlend.ValueArray[iBlend];
                    if (iBlend < 5)
                        dnowUp = colorBlend.ValueArray[iBlend + 1];
                    pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, ColumnName);
                    pcin.LegendText = ">=" + dnow.ToString() + " and <" + dnowUp.ToString();// string.Format("[{0}]>=" + dnow.ToString("E2") + " and [{0}] <" + dnowUp.ToString("E2"), ColumnName);
                    if (iBlend == 0)
                    {
                        pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp, ColumnName);
                        //pcin.LegendText = string.Format(" [{0}] <" + dnowUp.ToString("E2"), ColumnName);
                        pcin.LegendText = "<" + dnowUp.ToString();
                    }
                    if (iBlend == 5)
                    {
                        pcin.FilterExpression = string.Format(" [{0}] >=" + dnow, ColumnName);
                        //pcin.LegendText = string.Format(" [{0}] >=" + dnow.ToString("E2"), ColumnName);
                        pcin.LegendText = ">=" + dnow.ToString();
                    }

                    //pcin.LegendText = pcin.FilterExpression;// string.Format("{0} >= " + dnow + " and {0} < " + dnowUp, strValueField);

                    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                    ctemp = pcin.Symbolizer.GetFillColor();
                    pcin.Symbolizer.SetFillColor(ctemp.ToTransparent(fColor));
                    ctemp.ToTransparent(fColor);
                    pcin.Symbolizer.SetFillColor(colors[iColor]);
                    pcc.Add(pcin);
                    iColor++;
                    prgLoadPOP.PerformStep();
                }
                myScheme1.ClearCategories();
                foreach (PolygonCategory pct in pcc)
                {
                    myScheme1.Categories.Add(pct);
                }
                //player.Symbology = myScheme1;
                //if (myScheme1.LegendText == "Pooled Inci") myScheme1.LegendText = "Pooled Incidence";
                //if (myScheme1.LegendText == "Pooled Valu") myScheme1.LegendText = "Pooled Valuation";
                myScheme1.EditorSettings.ClassificationType = ClassificationType.Custom;
                (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology = myScheme1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        void getPopulationDataSetFromCRSelectFunction(ref Dictionary<string, Dictionary<string, float>> diclstPopulationAttributeAge, BenMAPPopulation benMAPPopulation)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<string, double> DicGrowth = new Dictionary<string, double>();
                Dictionary<string, Dictionary<string, WeightAttribute>> DicWeight = new Dictionary<string, Dictionary<string, WeightAttribute>>();
                Dictionary<string, Dictionary<string, double>> dicPopweightfromPercentage = new Dictionary<string, Dictionary<string, double>>();

                string commandText = string.Format("select min(Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID);
                int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
                commandText = "";

                FbDataReader fbDataReader = null;
                if (CommonClass.MainSetup.SetupID == 1)
                {
                    #region Growth
                    if (benMAPPopulation.Year != commonYear)
                    {
                        string strGrowth = "select CColumn||','||Row||','||EthnicityID||','||RaceID||','||GenderID||','||AgeRangeID,VValue from PopulationEntries where PopulationDataSetID=37 and YYear=" + benMAPPopulation.Year + "  ";
                        if (!dicAllGrowth.ContainsKey(benMAPPopulation.Year))
                        {
                            WaitChangeMsg("Loading population growth data...");
                            //lblProgress.Text = "Loading population growth data...";
                            //prgLoadPOP.Value = 0;
                            //this.Refresh();
                            string strGrowthCount = "select count(*) from PopulationEntries where PopulationDataSetID=37 and YYear=" + benMAPPopulation.Year + "  ";
                            int growthCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strGrowthCount));
                            //prgLoadPOP.Maximum = growthCount;

                            fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strGrowth);
                            DicGrowth = new Dictionary<string, double>();
                            while (fbDataReader.Read())
                            {
                                if (!DicGrowth.ContainsKey(fbDataReader[0].ToString()))
                                    DicGrowth.Add(fbDataReader[0].ToString(), Convert.ToDouble(fbDataReader["VValue"]));
                                else
                                    DicGrowth[fbDataReader[0].ToString()] = DicGrowth[fbDataReader[0].ToString()] + Convert.ToDouble(fbDataReader["VValue"]);
                                //prgLoadPOP.PerformStep();
                            }
                            fbDataReader.Dispose();
                        }
                        else
                        {
                            DicGrowth = dicAllGrowth[benMAPPopulation.Year];
                        }
                    }
                    else
                    {
                        DicGrowth = null;
                    }
                    #endregion

                    #region Weight
                    string strWeight = "select * from PopulationGrowthWeights where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                    if (!dicAllWeight.ContainsKey(benMAPPopulation.DataSetName + "," + benMAPPopulation.Year) && benMAPPopulation.Year != commonYear && benMAPPopulation.GridType.GridDefinitionID != 18)
                    {
                        string strWeightCount = "select count(*) from PopulationGrowthWeights where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                        int weightCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strWeightCount));
                        if (weightCount > 0)
                        {
                            WaitChangeMsg("Loading population growth weight...");
                            //lblProgress.Text = "Loading population growth weight...";
                            //prgLoadPOP.Value = 0;
                            //this.Refresh();
                            //prgLoadPOP.Maximum = weightCount;

                            fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strWeight);
                            DicWeight = new Dictionary<string, Dictionary<string, WeightAttribute>>();
                            while (fbDataReader.Read())
                            {
                                if (DicWeight.ContainsKey(fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()))
                                {
                                    DicWeight[fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()].Add(fbDataReader["SourceColumn"].ToString() + "," + fbDataReader["SourceRow"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                                              fbDataReader["RaceID"].ToString(),new WeightAttribute(){RaceID=fbDataReader["RaceID"].ToString(), EthnicityID=fbDataReader["EthnicityID"].ToString(),Value= Convert.ToDouble(fbDataReader["VValue"])});
                                }
                                else
                                {
                                    DicWeight.Add(fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString(), new Dictionary<string, WeightAttribute>());
                                    DicWeight[fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()].Add(fbDataReader["SourceColumn"].ToString() + "," + fbDataReader["SourceRow"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                                              fbDataReader["RaceID"].ToString(), new WeightAttribute() { RaceID = fbDataReader["RaceID"].ToString(), EthnicityID = fbDataReader["EthnicityID"].ToString(), Value = Convert.ToDouble(fbDataReader["VValue"]) });
                                }
                                //prgLoadPOP.PerformStep();
                            }
                            fbDataReader.Dispose();
                        }
                        else
                        {
                            DicWeight = null;
                            prgLoadPOP.Value = 0;
                            string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + benMAPPopulation.GridType.GridDefinitionID + " and  targetgriddefinitionid =18 ) and normalizationstate in (0,1)";
                            DataSet dsPercentage = null;
                            try
                            {
                                dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                                if (dsPercentage.Tables[0].Rows.Count == 0)
                                {
                                    WaitChangeMsg("Creating percentage...");
                                    //lblProgress.Text = "Creating percentage...";
                                    Configuration.ConfigurationCommonClass.creatPercentageToDatabase(18, benMAPPopulation.GridType.GridDefinitionID);
                                    dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                                }
                                //lblProgress.Text = "Loading population growth weight...";
                                //prgLoadPOP.Maximum = dsPercentage.Tables[0].Rows.Count;
                                WaitChangeMsg("Loading weight...");
                                foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                                {
                                    if (dicPopweightfromPercentage.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                                    {
                                        if (!dicPopweightfromPercentage[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                                            dicPopweightfromPercentage[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                                    }
                                    else
                                    {
                                        dicPopweightfromPercentage.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                                        dicPopweightfromPercentage[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                                    }
                                    //prgLoadPOP.PerformStep();
                                }
                                dsPercentage.Dispose();
                            }
                            catch
                            { }
                        }
                    }
                    else if (dicAllWeight.ContainsKey(benMAPPopulation.DataSetName + "," + commonYear) && benMAPPopulation.Year != commonYear)
                    {
                        DicWeight = dicAllWeight[benMAPPopulation.DataSetName + "," + commonYear];
                    }
                    #endregion
                }

                //string strPop = "select count(*) from PopulationEntries where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                //int popCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strPop));
                WaitChangeMsg("Creating population grid...");
                //string strPop = "select distinct RaceID,GenderID,EthnicityID,AgeRangeID from PopulationEntries where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                //DataTable dtGroup = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strPop).Tables[0];
                //lblProgress.Text = "Creating population grid...";
                //prgLoadPOP.Value = 0;
                //this.Refresh();
                //prgLoadPOP.Maximum = popCount;
                string strPop = "select CColumn,Row,EthnicityID,RaceID,GenderID,AgeRangeID,VValue from PopulationEntries where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strPop);
                char[] c = new char[] { ',' };
                //string[] sArray = null;
                double d = 0;
                while (fbDataReader.Read())
                {
                    d = 0;
                    
                    
                    if (CommonClass.MainSetup.SetupID == 1 && DicWeight != null && DicWeight.ContainsKey(fbDataReader["CColumn"] + "," + fbDataReader["Row"]) && DicGrowth != null && DicGrowth.Count > 0)
                    {
                        string se = fbDataReader["EthnicityID"].ToString(), sr = fbDataReader["RaceID"].ToString(),
                            sg = fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"];

                        foreach (KeyValuePair<string, WeightAttribute> k in DicWeight[fbDataReader["CColumn"] + "," + fbDataReader["Row"]])
                        {
                            //sArray = k.Key.Split(c);
                            if (k.Value.EthnicityID == se && k.Value.RaceID == sr && DicGrowth.ContainsKey(k.Key + "," + sg))
                                d += Convert.ToDouble(fbDataReader["VValue"]) * DicGrowth[k.Key + "," + sg] * k.Value.Value;
                        }
                    }
                    else if (CommonClass.MainSetup.SetupID == 1 && benMAPPopulation.GridType.GridDefinitionID == 18 && DicGrowth != null && DicGrowth.Count > 0)
                    {
                        if (DicGrowth.ContainsKey(fbDataReader["CColumn"] + "," + fbDataReader["Row"] + "," + fbDataReader["EthnicityID"] + "," +
                               fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["AgeRangeID"]))
                        {
                            d = Convert.ToDouble(fbDataReader["VValue"]) * DicGrowth[fbDataReader["CColumn"] + "," + fbDataReader["Row"] + "," + fbDataReader["EthnicityID"] + "," +
                                fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["AgeRangeID"]];
                        }
                    }
                    else if (CommonClass.MainSetup.SetupID == 1 && DicGrowth != null && DicGrowth.Count > 0 && dicPopweightfromPercentage != null && dicPopweightfromPercentage.Count > 0 && dicPopweightfromPercentage.ContainsKey(fbDataReader["CColumn"] + "," + fbDataReader["Row"]))
                    {
                        foreach (KeyValuePair<string, double> k in dicPopweightfromPercentage[fbDataReader["CColumn"] + "," + fbDataReader["Row"]])
                        {
                            if (DicGrowth.ContainsKey(k.Key + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["AgeRangeID"]))
                                d += Convert.ToDouble(fbDataReader["VValue"]) * DicGrowth[k.Key + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["AgeRangeID"]] * k.Value;
                        }
                    }
                    else
                    {
                        d = Convert.ToDouble(fbDataReader["VValue"]);
                    }

                    if (!diclstPopulationAttributeAge.ContainsKey(fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]))
                    {
                        diclstPopulationAttributeAge.Add(fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"], new Dictionary<string,float>());
                        diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]].Add(fbDataReader["CColumn"] + "," + fbDataReader["Row"], Convert.ToSingle(d));
                    }
                    else
                    {
                        if(diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]].ContainsKey(fbDataReader["CColumn"] + "," + fbDataReader["Row"]))
                            diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]][fbDataReader["CColumn"] + "," + fbDataReader["Row"]] += Convert.ToSingle(d);
                        else
                            diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]].Add(fbDataReader["CColumn"] + "," + fbDataReader["Row"], Convert.ToSingle(d));
                    }
                    //prgLoadPOP.PerformStep();
                }
                fbDataReader.Dispose();
                if (!dicAllGrowth.ContainsKey(benMAPPopulation.Year) && DicGrowth != null && DicGrowth.Count > 0)
                    dicAllGrowth.Add(benMAPPopulation.Year, DicGrowth);
                if (!dicAllWeight.ContainsKey(benMAPPopulation.DataSetName + "," + benMAPPopulation.Year))
                    dicAllWeight.Add(benMAPPopulation.DataSetName + "," + benMAPPopulation.Year, DicWeight);
            }
            catch (Exception ex)
            {
            }
        }

        #region ToolStrip
        private void btnDraw_Click(object sender, EventArgs e)
        {
            if (cboPopulationDataSet.Text != _popDataset || cboPopulationYear.Text != _popYear)
            {
                _popDataset = cboPopulationDataSet.Text;
                _popYear = cboPopulationYear.Text;
                DrawPop();
            }
            showInfo = false;
            mainMap.Cursor = Cursors.Default;
        }

        bool showInfo = false;
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.ZoomIn;
            showInfo = false;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.ZoomOut;
            showInfo = false;
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.Pan;
            showInfo = false;
        }

        private void btnFullExtent_Click(object sender, EventArgs e)
        {
            mainMap.ZoomToMaxExtent();
            mainMap.FunctionMode = FunctionMode.None;
            showInfo = false;
        }

        private void btnIdentify_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.None;
            mainMap.Cursor = Cursors.Help;
            showInfo = true;
        }

        /// <summary>
        /// 点击弹出资料框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainMap_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (mainMap.Layers.Count <= 0 || !showInfo) return;

                int x = 0, y = 0;
                GeoMouseArgs args = new GeoMouseArgs(e, mainMap);

                if (args.Button != MouseButtons.Left) return;
                //Rectangle rtol = new Rectangle(args.X - 8, args.Y - 8, 16, 16); //范围大，用于点、线图层
                Rectangle rstr = new Rectangle(args.X - 1, args.Y - 1, 2, 2);   //范围小，用于面图层
                //DotSpatial.Data.Extent tolerant = args.Map.PixelToProj(rtol);
                DotSpatial.Data.Extent strict = args.Map.PixelToProj(rstr);

                Dictionary<int, string> diclayers = new Dictionary<int, string>();
                List<DotSpatial.Data.IFeature> result = new List<DotSpatial.Data.IFeature>();
                Dictionary<int, DataRow> dicInfo = new Dictionary<int, DataRow>();

                for (int i = 0; i < mainMap.Layers.Count; i++)
                {
                    IFeatureLayer layer = mainMap.Layers[i] as IFeatureLayer;
                    result = layer.DataSet.Select(strict);
                    diclayers.Add(i, mainMap.Layers[i].LegendText);
                    dicInfo.Add(i, GetFeatureDataRow(layer, result));
                }

                if (frmPopInfoClose)
                {
                    frmPopInfo = new PopulationInformation();
                    frmPopInfo.Diclayers = diclayers;
                    frmPopInfo.DicInfo = dicInfo;
                    frmPopInfoClose = true;
                    frmPopInfo.Getform(this);
                    frmPopInfo.TopMost = true;
                    frmPopInfo.Show();
                }
                else
                {
                    frmPopInfo.Diclayers = diclayers;
                    frmPopInfo.DicInfo = dicInfo;
                    frmPopInfo.showInfo();
                }


            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// 获取图层信息（来自dotspetial源代码）
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private DataRow GetFeatureDataRow(IFeatureLayer layer, List<DotSpatial.Data.IFeature> result)
        {
            try
            {
                DataRow dr = null;
                foreach (DotSpatial.Data.IFeature feature in result)
                {
                    dr = null;
                    if (!layer.DataSet.AttributesPopulated)
                    {
                        int fid;
                        if (feature.ShapeIndex != null)
                            fid = layer.DataSet.ShapeIndices.IndexOf(feature.ShapeIndex);
                        else
                            fid = feature.Fid;

                        using (DataTable dt = layer.DataSet.GetAttributes(fid, 1))
                        {
                            if ((dt != null) && (dt.Rows.Count > 0))
                                dr = layer.DataSet.GetAttributes(fid, 1).Rows[0];
                        }

                        feature.DataRow = dr;
                    }
                    else
                    {
                        dr = feature.DataRow;
                    }
                }

                return dr;
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }

        private void tsbSaveMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (mainMap.Layers.Count == 0)
                    return;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "SHP(*.shp)|*.shp";
                saveFileDialog1.InitialDirectory = "C:\\";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string fileName = saveFileDialog1.FileName;
                FeatureLayer fl = mainMap.Layers[0] as FeatureLayer;
                fl.DataSet.SaveAs(fileName, true);
                MessageBox.Show("Shapefile saved.", "File saved");
            }
            catch
            {
            }
        }

        private void tsbSavePic_Click(object sender, EventArgs e)
        {
            try
            {
                //Thread.Sleep(new TimeSpan(0, 0, 2));
                string s = tsbSavePic.ToolTipText;
                tsbSavePic.ToolTipText = "";
                Image i = new Bitmap(mainMap.Width, mainMap.Height - toolStrip1.Height);
                Graphics g = Graphics.FromImage(i);
                tsbSavePic.ToolTipText = s;
                g.CopyFromScreen(this.PointToScreen(new System.Drawing.Point(splitContainer2.Panel1.Width + 6, splitContainer2.Location.Y + toolStrip1.Height)), new System.Drawing.Point(0, 0), new Size(this.Width, splitContainer2.Height - toolStrip1.Height - 1));
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "PNG(*.png)|*.png|JPG(*.jpg)|*.jpg";
                saveFileDialog1.InitialDirectory = "C:\\";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string fileName = saveFileDialog1.FileName;
                //Thread.Sleep(300);

                i.Save(fileName);
                MessageBox.Show("Map exported.");
                //----Save SHP File
                g.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        private void tsbChangeProjection_Click(object sender, EventArgs e)
        {
            try
            {
                if (mainMap.Layers.Count == 0) return;
                if (CommonClass.MainSetup.SetupName.ToLower() == "china")
                {
                    if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic)
                    {
                        mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic;
                        //mainMap.Projection.LatitudeOfOrigin = 34;
                        //mainMap.Projection.LongitudeOfCenter = 110;
                        foreach (FeatureLayer layer in mainMap.Layers)
                        {
                            //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                            layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                            layer.Reproject(mainMap.Projection);
                        }
                        tsbChangeProjection.Text = "change projection to GCS/NAD 83";
                    }
                    else
                    {
                        mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        foreach (FeatureLayer layer in mainMap.Layers)
                        {
                            //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                            layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic;
                            layer.Reproject(mainMap.Projection);
                        }
                        tsbChangeProjection.Text = "change projection to Albers";
                        //mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                    }


                    foreach (IMapGroup grp in mainMap.GetAllGroups())
                    {
                        grp.Projection.CopyProperties(mainMap.Projection);
                    }

                    //re-assign the map projection
                    mainMap.Projection.CopyProperties(mainMap.Projection);

                    //zoom to reprojected extent
                    //mainMap.Invalidate();
                    //mainMap.ResetExtents();
                    mainMap.ViewExtents = mainMap.Layers[0].Extent;
                    return;
                }


                if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic)
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
                    foreach (FeatureLayer layer in mainMap.Layers)
                    {
                        //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        layer.Reproject(mainMap.Projection);
                    }
                    tsbChangeProjection.Text = "change projection to GCS/NAD 83";
                }
                else
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                    foreach (FeatureLayer layer in mainMap.Layers)
                    {
                        //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
                        layer.Reproject(mainMap.Projection);
                    }
                    tsbChangeProjection.Text = "change projection to Albers";
                    //mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                }


                foreach (IMapGroup grp in mainMap.GetAllGroups())
                {
                    grp.Projection.CopyProperties(mainMap.Projection);
                }

                //re-assign the map projection
                mainMap.Projection.CopyProperties(mainMap.Projection);

                //zoom to reprojected extent
                //mainMap.Invalidate();
                //mainMap.ResetExtents();
                mainMap.ViewExtents = mainMap.Layers[0].Extent;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region 等待窗口
        TipFormGIF waitMess = new TipFormGIF();//等待窗体
        bool sFlog = true;
        //--显示等待窗体 
        private void ShowWaitMess()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    waitMess.ShowDialog();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        //--新开辟一个线程调用 
        public void WaitShow(string msg)
        {
            try
            {
                if (sFlog == true)
                {
                    sFlog = false;
                    waitMess.Msg = msg;
                    //ShowWaitMess();
                    System.Threading.Thread upgradeThread = null;
                    upgradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWaitMess));
                    upgradeThread.Start();
                    //upgradeThread.IsBackground = true;
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private delegate void CloseFormDelegate();
        private delegate void ChangeDelegate(string msg);
        //--关闭等待窗体 
        public void WaitClose()
        {
            //同步到主线程上
            if (waitMess.InvokeRequired)
                waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
            else
                DoCloseJob();
        }

        public void WaitChangeMsg(string msg)
        {
            try
            {
                if (waitMess.InvokeRequired)
                    waitMess.Invoke(new ChangeDelegate(DoChange), msg);
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private void DoChange(string msg)
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    if (waitMess.Created)
                    {
                        //sFlog = true;
                        waitMess.Msg = msg;
                    }
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private void DoCloseJob()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    if (waitMess.Created)
                    {
                        sFlog = true;
                        waitMess.Close();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        #endregion 等待窗口

        private void cboAgeRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawOneAge();
        }

        private void PopulationMap_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            DrawPop();
        }

        private void PopulationMap_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!frmPopInfoClose)
                frmPopInfo.Close();
            mainMap.Layers.Clear();
            fs.Dispose();
            fs.Close();
            dicAllPopData.Clear();
            dicAllGrowth.Clear();
            dicAllWeight.Clear();
            GC.Collect();
        }

    }
}
