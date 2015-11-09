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
using System.Drawing.Drawing2D;  //-added by MCB for color ramps

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
        private double _dMinValue = 0.0; private double _dMaxValue = 0.0; private int _currentLayerIndex = 1;
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
                List<string> lstAgeInKeys = dicPopulationAgeIn.Keys.ToList();
                foreach (DataRow dr in fs.DataTable.Rows)
                {
                    object[] o = new object[fs.DataTable.Columns.Count];
                    o[0] = dr[iCol]; o[1] = dr[iRow];
                    string s = dr[iCol] + "," + dr[iRow];
                    for (int idr = 2; idr < fs.DataTable.Columns.Count; idr++)
                    {
                        if (dicPopulationAgeIn[lstAgeInKeys[idr - 2]].ContainsKey(s) && !double.IsNaN(dicPopulationAgeIn[lstAgeInKeys[idr - 2]][s]))
                            o[idr] = Math.Round(dicPopulationAgeIn[lstAgeInKeys[idr - 2]][s], 4);
                    }
                    dr.ItemArray = o;

                    i++;
                }


                dicPopulationAgeIn.Clear();
                mainMap.Layers.Clear();
                GC.Collect();
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;

                mainMap.Layers.Add(fs);
                cboAgeRange.SelectedIndex = cboAgeRange.Items.Count - 1;
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
                myScheme1.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;
                myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding;
                myScheme1.EditorSettings.IntervalRoundingDigits = 1;
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
            {   //Replace the color ramp
                colorBlend.ColorArray = GetColorRamp("yellow_red", 6);

                _blendColors = colorBlend.ColorArray;
                _dMaxValue = colorBlend.MaxValue;
                _dMinValue = colorBlend.MinValue;
                colorBlend.SetValueRange(_dMinValue, _dMaxValue, false);
                Color[] colors = new Color[_blendColors.Length];
                _blendColors.CopyTo(colors, 0);
                PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                int iColor = 0;
                string ColumnName = _columnName;
                PolygonScheme myScheme1 = new PolygonScheme();
                float fl = (float)0.1;
                //Original scheme editor settings
                //myScheme1.EditorSettings.StartColor = Color.Blue;
                //myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                //myScheme1.EditorSettings.EndColor = Color.Red;
                //myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                //new scheme editor settings
                myScheme1.EditorSettings.StartColor = colorBlend.ColorArray[0];
                myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                myScheme1.EditorSettings.EndColor = colorBlend.ColorArray[5];
                myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                float fColor = (float)0.2;
                Color ctemp = new Color();
                myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                myScheme1.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;
                myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding;
                myScheme1.EditorSettings.IntervalRoundingDigits = 1;
                myScheme1.EditorSettings.NumBreaks = 6;
                myScheme1.EditorSettings.FieldName = _columnName;
                myScheme1.EditorSettings.UseGradient = false;
                myScheme1.CreateCategories((mainMap.Layers[_currentLayerIndex] as IFeatureLayer).DataSet.DataTable);
                if (myScheme1.Categories.Count == 1)
                {

                    PolygonSymbolizer ps = new PolygonSymbolizer();
                    ps.SetFillColor(colors[iColor]);
                    ps.SetOutline(Color.Transparent, 0);

                    (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbolizer = ps;
                    return;

                }
                iColor = 0;
                prgLoadPOP.Maximum = 6;
                for (int iBlend = 0; iBlend < 6; iBlend++)
                {
                    PolygonCategory pcin = new PolygonCategory();
                    double dnow = 0; double dnowUp = 0; dnow = colorBlend.ValueArray[iBlend];
                    if (iBlend < 5)
                        dnowUp = colorBlend.ValueArray[iBlend + 1];
                    pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, ColumnName);
                    pcin.LegendText = ">=" + dnow.ToString() + " and <" + dnowUp.ToString(); if (iBlend == 0)
                    {
                        pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp, ColumnName);
                        pcin.LegendText = "<" + dnowUp.ToString();
                    }
                    if (iBlend == 5)
                    {
                        pcin.FilterExpression = string.Format(" [{0}] >=" + dnow, ColumnName);
                        pcin.LegendText = ">=" + dnow.ToString();
                    }


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
                    if (benMAPPopulation.Year != commonYear)
                    {
                        string strGrowth = "select CColumn||','||Row||','||EthnicityID||','||RaceID||','||GenderID||','||AgeRangeID,VValue from PopulationEntries where PopulationDataSetID=37 and YYear=" + benMAPPopulation.Year + "  ";
                        if (!dicAllGrowth.ContainsKey(benMAPPopulation.Year))
                        {
                            WaitChangeMsg("Loading population growth data...");
                            string strGrowthCount = "select count(*) from PopulationEntries where PopulationDataSetID=37 and YYear=" + benMAPPopulation.Year + "  ";
                            int growthCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strGrowthCount));

                            fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strGrowth);
                            DicGrowth = new Dictionary<string, double>();
                            while (fbDataReader.Read())
                            {
                                if (!DicGrowth.ContainsKey(fbDataReader[0].ToString()))
                                    DicGrowth.Add(fbDataReader[0].ToString(), Convert.ToDouble(fbDataReader["VValue"]));
                                else
                                    DicGrowth[fbDataReader[0].ToString()] = DicGrowth[fbDataReader[0].ToString()] + Convert.ToDouble(fbDataReader["VValue"]);
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

                    string strWeight = "select * from PopulationGrowthWeights where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                    if (!dicAllWeight.ContainsKey(benMAPPopulation.DataSetName + "," + benMAPPopulation.Year) && benMAPPopulation.Year != commonYear && benMAPPopulation.GridType.GridDefinitionID != 18)
                    {
                        string strWeightCount = "select count(*) from PopulationGrowthWeights where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                        int weightCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strWeightCount));
                        if (weightCount > 0)
                        {
                            WaitChangeMsg("Loading population growth weight...");

                            fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strWeight);
                            DicWeight = new Dictionary<string, Dictionary<string, WeightAttribute>>();
                            while (fbDataReader.Read())
                            {
                                if (DicWeight.ContainsKey(fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()))
                                {
                                    DicWeight[fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()].Add(fbDataReader["SourceColumn"].ToString() + "," + fbDataReader["SourceRow"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                                              fbDataReader["RaceID"].ToString(), new WeightAttribute() { RaceID = fbDataReader["RaceID"].ToString(), EthnicityID = fbDataReader["EthnicityID"].ToString(), Value = Convert.ToDouble(fbDataReader["VValue"]) });
                                }
                                else
                                {
                                    DicWeight.Add(fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString(), new Dictionary<string, WeightAttribute>());
                                    DicWeight[fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()].Add(fbDataReader["SourceColumn"].ToString() + "," + fbDataReader["SourceRow"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                                              fbDataReader["RaceID"].ToString(), new WeightAttribute() { RaceID = fbDataReader["RaceID"].ToString(), EthnicityID = fbDataReader["EthnicityID"].ToString(), Value = Convert.ToDouble(fbDataReader["VValue"]) });
                                }
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
                                    Configuration.ConfigurationCommonClass.creatPercentageToDatabase(18, benMAPPopulation.GridType.GridDefinitionID,null);
                                    dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                                }
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
                }

                WaitChangeMsg("Creating population grid...");
                string strPop = "select CColumn,Row,EthnicityID,RaceID,GenderID,AgeRangeID,VValue from PopulationEntries where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
                fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strPop);
                char[] c = new char[] { ',' };
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
                        diclstPopulationAttributeAge.Add(fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"], new Dictionary<string, float>());
                        diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]].Add(fbDataReader["CColumn"] + "," + fbDataReader["Row"], Convert.ToSingle(d));
                    }
                    else
                    {
                        if (diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]].ContainsKey(fbDataReader["CColumn"] + "," + fbDataReader["Row"]))
                            diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]][fbDataReader["CColumn"] + "," + fbDataReader["Row"]] += Convert.ToSingle(d);
                        else
                            diclstPopulationAttributeAge[fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["AgeRangeID"]].Add(fbDataReader["CColumn"] + "," + fbDataReader["Row"], Convert.ToSingle(d));
                    }
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

        private void mainMap_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (mainMap.Layers.Count <= 0 || !showInfo) return;

                int x = 0, y = 0;
                GeoMouseArgs args = new GeoMouseArgs(e, mainMap);

                if (args.Button != MouseButtons.Left) return;
                Rectangle rstr = new Rectangle(args.X - 1, args.Y - 1, 2, 2); DotSpatial.Data.Extent strict = args.Map.PixelToProj(rstr);

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

                i.Save(fileName);
                MessageBox.Show("Map exported.");
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
                        foreach (FeatureLayer layer in mainMap.Layers)
                        {
                            layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                            layer.Reproject(mainMap.Projection);
                        }
                        tsbChangeProjection.Text = "change projection to WGS1984";
                    }
                    else
                    {
                        mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        foreach (FeatureLayer layer in mainMap.Layers)
                        {
                            layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic;
                            layer.Reproject(mainMap.Projection);
                        }
                        tsbChangeProjection.Text = "change projection to Albers";
                    }


                    foreach (IMapGroup grp in mainMap.GetAllGroups())
                    {
                        grp.Projection.CopyProperties(mainMap.Projection);
                    }

                    mainMap.Projection.CopyProperties(mainMap.Projection);

                    mainMap.ViewExtents = mainMap.Layers[0].Extent;
                    return;
                }


                if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic)
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
                    foreach (FeatureLayer layer in mainMap.Layers)
                    {
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        layer.Reproject(mainMap.Projection);
                    }
                    tsbChangeProjection.Text = "change projection to WGS1984";
                }
                else
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                    foreach (FeatureLayer layer in mainMap.Layers)
                    {
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
                        layer.Reproject(mainMap.Projection);
                    }
                    tsbChangeProjection.Text = "change projection to Albers";
                }


                foreach (IMapGroup grp in mainMap.GetAllGroups())
                {
                    grp.Projection.CopyProperties(mainMap.Projection);
                }

                mainMap.Projection.CopyProperties(mainMap.Projection);

                mainMap.ViewExtents = mainMap.Layers[0].Extent;
            }
            catch (Exception ex)
            {
            }
        }

        TipFormGIF waitMess = new TipFormGIF(); bool sFlog = true;
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

        public void WaitShow(string msg)
        {
            try
            {
                if (sFlog == true)
                {
                    sFlog = false;
                    waitMess.Msg = msg;
                    System.Threading.Thread upgradeThread = null;
                    upgradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWaitMess));
                    upgradeThread.Start();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private delegate void CloseFormDelegate();
        private delegate void ChangeDelegate(string msg);
        public void WaitClose()
        {
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
        private Color[] GetColorRamp(string rampID, int numClasses)
        {
            // FYI: numClasses is just a stub for now- only 6 class color ramps have been created so far.-MCB

            //New empty color array to fill and return
            Color[] _colorArray = new Color[6];

            //Create a selection of color ramps to choose from
            //Note: Color ramps created using ColorBrewer 2.0: chose from color blind safe six class ramps. -MCB
            //Sequential- color ramps
            //single hue (light to dark)
            
            Color[] _oranges_Array = { Color.FromArgb(254, 237, 222), Color.FromArgb(254, 217, 118), Color.FromArgb(253, 174, 107), Color.FromArgb(253, 141, 60), Color.FromArgb(230, 85, 13), Color.FromArgb(166, 54, 3) };
            Color[] _purples_Array = { Color.FromArgb(242, 240, 247), Color.FromArgb(218, 218, 235), Color.FromArgb(188, 189, 220), Color.FromArgb(158, 154, 200), Color.FromArgb(117, 107, 177), Color.FromArgb(84, 39, 143) };
            Color[] _blues_Array = { Color.FromArgb(239, 243, 255), Color.FromArgb(198, 219, 239), Color.FromArgb(158, 202, 225), Color.FromArgb(107, 174, 214), Color.FromArgb(49, 130, 189), Color.FromArgb(8, 81, 156) };

            //multi-hue
            //pale_yellow_blue chosen as default ramp for main map (and default)
            Color[] _pale_yellow_blue_Array = { Color.FromArgb(240, 249, 232), Color.FromArgb(204, 235, 197), Color.FromArgb(168, 221, 181), Color.FromArgb(123, 204, 196), Color.FromArgb(67, 162, 202), Color.FromArgb(8, 104, 172) };
            //Pale blue to green - an alternative mentioned by the client in an e-mail
            Color[] _pale_blue_green_Array = { Color.FromArgb(246, 239, 247), Color.FromArgb(208, 209, 230), Color.FromArgb(166, 189, 219), Color.FromArgb(103, 169, 207), Color.FromArgb(28, 144, 153), Color.FromArgb(1, 108, 89) }; 
            Color[] _yellow_red_Array = { Color.FromArgb(255, 255, 178), Color.FromArgb(254, 217, 118), Color.FromArgb(254, 178, 76), Color.FromArgb(253, 141, 60), Color.FromArgb(240, 59, 32), Color.FromArgb(189, 0, 38) };
            Color[] _yellow_blue_Array = { Color.FromArgb(255, 255, 204), Color.FromArgb(199, 233, 180), Color.FromArgb(127, 205, 187), Color.FromArgb(65, 182, 196), Color.FromArgb(44, 127, 184), Color.FromArgb(37, 52, 148) };
            Color[] _yellow_green_Array = { Color.FromArgb(255, 255, 204), Color.FromArgb(217, 240, 163), Color.FromArgb(173, 221, 142), Color.FromArgb(120, 198, 121), Color.FromArgb(49, 163, 84), Color.FromArgb(0, 104, 55) };

            //Diverging color ramps
            Color[] _brown_green_Array = { Color.FromArgb(140, 81, 10), Color.FromArgb(216, 179, 101), Color.FromArgb(246, 232, 195), Color.FromArgb(199, 234, 229), Color.FromArgb(90, 180, 172), Color.FromArgb(1, 102, 94) };
            Color[] _magenta_green_Array = { Color.FromArgb(197, 27, 125), Color.FromArgb(233, 163, 201), Color.FromArgb(253, 224, 239), Color.FromArgb(230, 245, 208), Color.FromArgb(161, 215, 106), Color.FromArgb(77, 146, 33) };
            Color[] _red_blue_Array = { Color.FromArgb(215, 48, 39), Color.FromArgb(252, 141, 89), Color.FromArgb(254, 224, 144), Color.FromArgb(224, 243, 248), Color.FromArgb(145, 191, 219), Color.FromArgb(69, 117, 180) };
            Color[] _red_black_Array = { Color.FromArgb(178, 24, 43), Color.FromArgb(239, 138, 98), Color.FromArgb(253, 219, 199), Color.FromArgb(224, 224, 224), Color.FromArgb(153, 153, 153), Color.FromArgb(77, 77, 77) };
            Color[] _purple_green_Array = { Color.FromArgb(118, 42, 131), Color.FromArgb(175, 141, 195), Color.FromArgb(231, 212, 232), Color.FromArgb(217, 240, 211), Color.FromArgb(127, 191, 123), Color.FromArgb(27, 120, 55) };

            //Note: Could double the ramps by allowing the case hue names in reverse order and just reversing the array contents. 
            switch (rampID)
            {
                case "oranges":
                    _colorArray = _oranges_Array;
                    break;
                case "purples":
                    _colorArray = _purples_Array;
                    break;
                case "blues":
                    _colorArray = _blues_Array;
                    break;

                
                case "yellow_red":
                    _colorArray = _yellow_red_Array;
                    break;
                case "pale_yellow_blue":
                    _colorArray = _pale_yellow_blue_Array;
                    break;
                case "pale_blue_green":
                    _colorArray = _pale_blue_green_Array;
                    break;
                case "yellow_blue":
                    _colorArray = _yellow_blue_Array;
                    break;
                case "yellow_green":
                    _colorArray = _yellow_green_Array;
                    break;


                case "brown_green":
                    _colorArray = _brown_green_Array;
                    break;
                case "magenta_green":
                    _colorArray = _magenta_green_Array;
                    break;
                case "red_blue":
                    _colorArray = _red_blue_Array;
                    break;
                case "red_black":
                    _colorArray = _red_black_Array;
                    break;
                case "purple_green":
                    _colorArray = _purple_green_Array;
                    break;
            }

            return _colorArray;
        }
    }
}
