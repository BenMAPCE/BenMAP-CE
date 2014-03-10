using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BenMAP.DataSource;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;

namespace BenMAP
{
    public partial class MonitorRollbackSettings2 : FormBase
    {

        public MonitorModelRollbackLine _monitorRollbackLine;
        private List<RowCol> _selectedRegions;

        private BaseControlGroup _bgc;

        public BaseControlGroup Bgc
        {
            get { return _bgc; }
            set { _bgc = value; }
        }

        private BenMAPRollback _currentBenMAPRollback;
        private Dictionary<string, int> dicMyColorIndex = new Dictionary<string, int>();

        private string _currentStat = string.Empty;

        List<MonitorValue> lstMonitorValues = new List<MonitorValue>();

        public MonitorRollbackSettings2(string currentStat, MonitorModelRollbackLine MonitorModelRollbackLine)
        {
            InitializeComponent();
            _currentStat = currentStat;
            _monitorRollbackLine = MonitorModelRollbackLine;
            _monitorRollbackLine.BenMAPRollbacks = new List<BenMAPRollback>();
            _selectedRegions = new List<RowCol>();
        }

        private void MonitorRollbackSettings2_Load(object sender, EventArgs e)
        {
            try
            {
                if (CommonClass.GBenMAPGrid == null) return;
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;
                if (_monitorRollbackLine.RollbackGrid is ShapefileGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (_monitorRollbackLine.RollbackGrid as ShapefileGrid).ShapefileName + ".shp"))
                    {
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (_monitorRollbackLine.RollbackGrid as ShapefileGrid).ShapefileName + ".shp");
                    }
                }
                else if (_monitorRollbackLine.RollbackGrid is RegularGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (_monitorRollbackLine.RollbackGrid as RegularGrid).ShapefileName + ".shp"))
                    {
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (_monitorRollbackLine.RollbackGrid as RegularGrid).ShapefileName + ".shp");
                    }
                }
                PolygonLayer playerRegion = mainMap.Layers[mainMap.Layers.Count - 1] as PolygonLayer;
                playerRegion.DataSet.DataTable.Columns.Add("MyColorIndex", typeof(int));
                for (int i = 0; i < playerRegion.DataSet.DataTable.Rows.Count; i++)
                {
                    playerRegion.DataSet.DataTable.Rows[i]["MyColorIndex"] = i;
                    dicMyColorIndex.Add(Convert.ToInt32(playerRegion.DataSet.DataTable.Rows[i]["COL"]).ToString() + "," + Convert.ToInt32(playerRegion.DataSet.DataTable.Rows[i]["ROW"]).ToString(), i);
                }
                Color cRegion = Color.Transparent;
                PolygonSymbolizer TransparentRegion = new PolygonSymbolizer(cRegion);

                TransparentRegion.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1); playerRegion.Symbolizer = TransparentRegion;

                lstMonitorValues = DataSourceCommonClass.GetMonitorData(_monitorRollbackLine.GridType, _monitorRollbackLine.Pollutant, _monitorRollbackLine);
                IFeatureSet fsPoints = new FeatureSet();
                MonitorValue mv = null;
                Feature feature = null;
                List<DotSpatial.Topology.Coordinate> lstCoordinate = new List<DotSpatial.Topology.Coordinate>();
                List<double> fsInter = new List<double>();
                if (lstMonitorValues != null && lstMonitorValues.Count > 0)
                {
                    PolygonScheme myScheme = new PolygonScheme();
                    PolygonCategory pcin = new PolygonCategory();
                    pcin.Symbolizer.SetFillColor(Color.Red);
                    myScheme.Categories.Add(pcin);
                    DotSpatial.Topology.Point point;
                    for (int i = 0; i < lstMonitorValues.Count; i++)
                    {
                        mv = lstMonitorValues[i];
                        point = new DotSpatial.Topology.Point(mv.Longitude, mv.Latitude);
                        feature = new Feature(point);
                        fsPoints.AddFeature(feature);
                    }
                    mainMap.Layers.Add(fsPoints);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        string saveAQGPath = string.Empty;
        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (_monitorRollbackLine.BenMAPRollbacks.Count < 1) { MessageBox.Show("You must select at least one region."); return; }
                if (chbExportAfterRollback.Checked)
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "CSV File|*.csv";
                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                    FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    sw.WriteLine("Monitor Name,Monitor Description,Latitude,Longitude,Metric,Seasonal Metric,Statistic,Values");
                    foreach (MonitorValue mv in lstMonitorValues)
                    {
                        sw.Write(mv.MonitorName);
                        sw.Write(",");
                        sw.Write("\"" + mv.MonitorMethod + "\"");
                        sw.Write(",");
                        sw.Write(mv.Latitude);
                        sw.Write(",");
                        sw.Write(mv.Longitude);
                        sw.Write(",");
                        if (mv.Metric != null)
                        {
                            sw.Write(mv.Metric.MetricName);
                        }
                        sw.Write(",");
                        if (mv.SeasonalMetric != null)
                        {
                            sw.Write(mv.SeasonalMetric.SeasonalMetricName);
                        }
                        sw.Write(",");
                        sw.Write(mv.Statistic);
                        sw.Write(",");
                        string value = string.Empty;
                        string commandText = string.Format("select VValues from MonitorEntries where MonitorID={0}", mv.MonitorID);
                        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                        byte[] blob = null;
                        object ob = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        blob = ob as byte[];
                        value = System.Text.Encoding.Default.GetString(blob);
                        sw.Write("\"" + value + "\"");
                        sw.WriteLine();
                    }
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                    MessageBox.Show("Export is complete.", "File saved", MessageBoxButtons.OK);
                }
                MonitorRollbackSettings3 frm = new MonitorRollbackSettings3();
                frm.CurrentStat = _currentStat;
                frm._monitorRollbackLine = _monitorRollbackLine;
                frm.Bgc = _bgc;
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private string AsyncUpdateMonitorRollbackData(string currentStat, MonitorModelRollbackLine monitorRollbackLine, out int threadId)
        {
            threadId = -1;
            string str = string.Empty;
            try
            {
                if (CommonClass.LstAsynchronizationStates == null) { CommonClass.LstAsynchronizationStates = new List<string>(); }
                lock (CommonClass.LstAsynchronizationStates)
                {
                    str = string.Format("{0}{1}", monitorRollbackLine.Pollutant.PollutantName.ToLower(), currentStat);
                    CommonClass.LstAsynchronizationStates.Add(str);
                    if (currentStat != "")
                    {
                        CommonClass.CurrentMainFormStat = currentStat.Substring(0, 1).ToUpper() + currentStat.Substring(1) + " is being created.";
                    }
                }
                lock (CommonClass.NodeAnscyStatus)
                { CommonClass.NodeAnscyStatus = string.Format("{0};{1};on", monitorRollbackLine.Pollutant.PollutantName.ToLower(), _currentStat); }
                switch (_currentStat)
                {
                    case "baseline":
                        RollBackDalgorithm.UpdateMonitorDataRollBack(ref _monitorRollbackLine);
                        lock (CommonClass.LstBaseControlGroup)
                        {
                            foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                            {
                                if (bc.Pollutant.PollutantID == _monitorRollbackLine.Pollutant.PollutantID)
                                {
                                    _monitorRollbackLine.ShapeFile = _monitorRollbackLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + _currentStat + ".shp";
                                    string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _monitorRollbackLine.ShapeFile);
                                    bc.Base = _monitorRollbackLine;
                                    DataSourceCommonClass.SaveBenMAPLineShapeFile(_monitorRollbackLine.GridType, _monitorRollbackLine.Pollutant, _monitorRollbackLine, shipFile);
                                    DataSourceCommonClass.CreateAQGFromBenMAPLine(bc.Base, saveAQGPath); bc.Base.ShapeFile = "";
                                }
                            }
                        }
                        break;
                    case "control":
                        RollBackDalgorithm.UpdateMonitorDataRollBack(ref _monitorRollbackLine);
                        lock (CommonClass.LstBaseControlGroup)
                        {
                            foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                            {
                                if (bc.Pollutant.PollutantID == _monitorRollbackLine.Pollutant.PollutantID)
                                {
                                    _monitorRollbackLine.ShapeFile = _monitorRollbackLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "C" + _currentStat + ".shp";
                                    string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _monitorRollbackLine.ShapeFile);
                                    bc.Control = _monitorRollbackLine;
                                    DataSourceCommonClass.SaveBenMAPLineShapeFile(_monitorRollbackLine.GridType, _monitorRollbackLine.Pollutant, _monitorRollbackLine, shipFile);
                                    DataSourceCommonClass.CreateAQGFromBenMAPLine(bc.Control, saveAQGPath); bc.Control.ShapeFile = "";
                                }
                            }
                        }
                        break;
                }
                lock (CommonClass.LstAsynchronizationStates)
                {
                    CommonClass.LstAsynchronizationStates.Remove(str);
                    if (CommonClass.LstAsynchronizationStates.Count == 0)
                    {
                        CommonClass.CurrentMainFormStat = "Current Setup: " + CommonClass.MainSetup.SetupName;
                    }
                }
                lock (CommonClass.NodeAnscyStatus)
                { CommonClass.NodeAnscyStatus = string.Format("{0};{1};off", monitorRollbackLine.Pollutant.PollutantName.ToLower(), _currentStat); }
                return str;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return string.Empty;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        Dictionary<string, int> dicRegion = new Dictionary<string, int>();
        int regionnumber = 1;
        private void btnAddRegion_Click(object sender, EventArgs e)
        {
            try
            {
                SelectRegionRollbackType frm = new SelectRegionRollbackType();
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                string name = frm.ControlName;
                int i = GetPercentageControlCount(flowLayoutPanel1);
                int j = GetIncrementalControlCount(flowLayoutPanel1);
                int k = GetToStandardControlCount(flowLayoutPanel1);
                switch (name)
                {
                    case "Percentage":
                        PercentageControl pc = new PercentageControl();
                        if (i == 0 && j == 0 && k == 0)
                        { pc.Location = new Point(0, 0); }
                        else
                        {
                            pc.Location = new Point(0, i * 101 + j * 101 + k * 370 + (i + j + k - 1) * 1);
                        }
                        Label lablePc = new Label();
                        pc.Controls.Add(lablePc);
                        while (dicRegion.ContainsValue(regionnumber))
                        {
                            regionnumber++;
                        }
                        lablePc.Text = "Region" + " " + regionnumber.ToString();
                        lablePc.Location = new Point(40, 3);
                        Button buttonPc = new Button();
                        pc.Controls.Add(buttonPc);
                        buttonPc.Size = new Size(35, 23);
                        buttonPc.Location = new Point(0, 0);
                        buttonPc.Text = "";
                        Random randomPc = new Random();
                        buttonPc.BackColor = Color.FromArgb(randomPc.Next(255), randomPc.Next(255), randomPc.Next(255));
                        Regex reg = new Regex(@"\D"); string s = reg.Replace(lablePc.Text, ""); int regionIDInt = int.Parse(s);
                        pc.RegionID = regionIDInt;
                        PercentageRollback pr = new PercentageRollback();
                        pr.DrawingColor = buttonPc.BackColor.R + "," + buttonPc.BackColor.G + "," + buttonPc.BackColor.B;
                        pr.RegionID = regionIDInt;
                        pr.SelectRegions = new List<RowCol>();
                        pr.RollbackType = RollbackType.percentage;
                        _monitorRollbackLine.BenMAPRollbacks.Add(pr);
                        _currentBenMAPRollback = pr;
                        pc._PercentageRollback = pr;
                        this.flowLayoutPanel1.Controls.Add(pc);
                        dicRegion.Add(lablePc.Text, regionIDInt);
                        break;
                    case "Incremental":
                        IncrementalControl ic = new IncrementalControl();
                        if (i == 0 && j == 0 && k == 0)
                        { ic.Location = new Point(0, 0); }
                        else
                        {
                            ic.Location = new Point(0, i * 101 + j * 101 + k * 370 + (i + j + k - 1) * 1);
                        }
                        Label lableIc = new Label();
                        ic.Controls.Add(lableIc);
                        while (dicRegion.ContainsValue(regionnumber))
                        {
                            regionnumber++;
                        }
                        lableIc.Text = "Region" + " " + regionnumber.ToString();
                        lableIc.Location = new Point(40, 3);
                        Button buttonIc = new Button();
                        ic.Controls.Add(buttonIc);
                        buttonIc.Size = new Size(35, 23);
                        buttonIc.Location = new Point(0, 0);
                        buttonIc.Text = "";
                        Random randomIc = new Random();
                        buttonIc.BackColor = Color.FromArgb(randomIc.Next(255), randomIc.Next(255), randomIc.Next(255));
                        ic.Name = lableIc.Text;
                        reg = new Regex(@"\D"); s = reg.Replace(lableIc.Text, ""); regionIDInt = int.Parse(s);
                        ic.RegionID = regionIDInt;

                        IncrementalRollback ir = new IncrementalRollback();
                        ir.DrawingColor = buttonIc.BackColor.R + "," + buttonIc.BackColor.G + "," + buttonIc.BackColor.B; ir.RegionID = regionIDInt;
                        ir.SelectRegions = new List<RowCol>();
                        ir.RollbackType = RollbackType.incremental;
                        _monitorRollbackLine.BenMAPRollbacks.Add(ir);
                        _currentBenMAPRollback = ir;
                        ic._IncrementalRollback = ir;
                        this.flowLayoutPanel1.Controls.Add(ic);
                        dicRegion.Add(lableIc.Text, regionIDInt);
                        break;
                    case "ToStandard":
                        ToStandardControl sc = new ToStandardControl(_monitorRollbackLine.Pollutant);

                        if (i == 0 && j == 0 && k == 0)
                        { sc.Location = new Point(0, 0); }
                        else
                        {
                            sc.Location = new Point(0, i * 101 + j * 101 + k * 370 + (i + j + k - 1) * 1);
                        }
                        Label lableSc = new Label();
                        sc.Controls.Add(lableSc);
                        while (dicRegion.ContainsValue(regionnumber))
                        {
                            regionnumber++;
                        }
                        lableSc.Text = "Region" + " " + regionnumber.ToString();
                        lableSc.Location = new Point(40, 3);
                        Button buttonSc = new Button();
                        sc.Controls.Add(buttonSc);
                        buttonSc.Size = new Size(35, 23);
                        buttonSc.Location = new Point(0, 0);
                        buttonSc.Text = "";
                        Random randomSc = new Random();
                        buttonSc.BackColor = Color.FromArgb(randomSc.Next(255), randomSc.Next(255), randomSc.Next(255));

                        reg = new Regex(@"\D"); s = reg.Replace(lableSc.Text, ""); regionIDInt = int.Parse(s);

                        StandardRollback sr = new StandardRollback();
                        sr.DrawingColor = buttonSc.BackColor.R + "," + buttonSc.BackColor.G + "," + buttonSc.BackColor.B; sr.RegionID = regionIDInt;
                        sr.SelectRegions = new List<RowCol>();
                        sr.RollbackType = RollbackType.standard;
                        _monitorRollbackLine.BenMAPRollbacks.Add(sr);
                        _currentBenMAPRollback = sr;
                        sc._StandardRollback = sr;
                        this.flowLayoutPanel1.Controls.Add(sc);
                        dicRegion.Add(lableSc.Text, regionIDInt);
                        break;
                    default:
                        break;
                }

                int flpCtlInx = this.flowLayoutPanel1.Controls.Count - 1;
                if (this.flowLayoutPanel1.Controls[flpCtlInx].Controls[1].GetType().Name == "Label")
                {
                    cboDeleteRegion.Items.Add(this.flowLayoutPanel1.Controls[flpCtlInx].Controls[1].Text);
                }
                cboDeleteRegion.SelectedIndex = cboDeleteRegion.Items.Count - 1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private int GetPercentageControlCount(FlowLayoutPanel panel)
        {
            int PercentageControlCount = 0;
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c is PercentageControl)
                {
                    PercentageControlCount++;
                }
            }
            return PercentageControlCount;
        }

        private int GetIncrementalControlCount(FlowLayoutPanel panel)
        {
            int IncrementalControlCount = 0;
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c is IncrementalControl)
                {
                    IncrementalControlCount++;
                }
            }
            return IncrementalControlCount;
        }

        private int GetToStandardControlCount(FlowLayoutPanel panel)
        {
            int ToStandardControlCount = 0;
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c is ToStandardControl)
                {
                    ToStandardControlCount++;
                }
            }
            return ToStandardControlCount;
        }

        private void btnDeleteRegion_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
                {
                    if (flowLayoutPanel1.Controls[i].Controls.Count != 0)
                    {
                        if (flowLayoutPanel1.Controls[i].Controls[1].Text == cboDeleteRegion.Text)
                        {
                            if (_monitorRollbackLine.BenMAPRollbacks.Count == 1)
                            {
                                _monitorRollbackLine.BenMAPRollbacks[0].DrawingColor = "255,255,255";
                                ColorMap();
                                flowLayoutPanel1.Controls.RemoveAt(i);
                                dicRegion.Remove(cboDeleteRegion.Text);
                                cboDeleteRegion.Items.Remove(cboDeleteRegion.SelectedItem);
                                cboDeleteRegion.SelectedIndex = -1;
                                _monitorRollbackLine.BenMAPRollbacks.RemoveAt(i);
                                return;
                            }
                            flowLayoutPanel1.Controls.RemoveAt(i); dicRegion.Remove(cboDeleteRegion.Text);
                            cboDeleteRegion.Items.Remove(cboDeleteRegion.SelectedItem);
                            cboDeleteRegion.SelectedIndex = cboDeleteRegion.Items.Count - 1;
                            _monitorRollbackLine.BenMAPRollbacks.RemoveAt(i);
                            ColorMap();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboActiveCtl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                OnSelectedIndexChanged += new EventHandler(cboActiveCtl_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboDeleteRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            Regex reg = new Regex(@"\D"); string s = reg.Replace(cboDeleteRegion.SelectedItem.ToString(), ""); int regionIDInt = int.Parse(s);

            foreach (BenMAPRollback rollback in _monitorRollbackLine.BenMAPRollbacks)
            {
                if (regionIDInt == rollback.RegionID)
                {
                    _currentBenMAPRollback = rollback;
                }
            }
            ColorMap();
        }

        public event EventHandler OnSelectedIndexChanged
        {
            add
            {
                cboDeleteRegion.SelectedIndexChanged += value;
            }
            remove
            {
            }
        }

        private void mainMap_SelectionChanged(object sender, EventArgs e)
        {
            try
            {








            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            finally
            {
            }
        }

        private void ColorMap()
        {
            try
            {
                FeatureLayer fl = mainMap.Layers[0] as FeatureLayer;
                PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                PolygonScheme myScheme = new PolygonScheme();

                myScheme.Categories = new PolygonCategoryCollection();


                string strrow = "";
                int iRegionColor = 0;

                foreach (BenMAPRollback brb in _monitorRollbackLine.BenMAPRollbacks)
                {
                    if (brb.SelectRegions.Count != 0)
                    {
                        iRegionColor += brb.SelectRegions.Count;
                    }
                }
                foreach (BenMAPRollback brb in _monitorRollbackLine.BenMAPRollbacks)
                {
                    if (brb.SelectRegions.Count != 0)
                    {

                        foreach (RowCol rc in brb.SelectRegions)
                        {
                            if (iRegionColor != dicMyColorIndex.Count)
                            {
                                if (strrow == "") strrow = dicMyColorIndex[rc.Col + "," + rc.Row].ToString();
                                else strrow += "," + dicMyColorIndex[rc.Col + "," + rc.Row].ToString();
                            }
                            PolygonCategory pcin = new PolygonCategory();

                            pcin.FilterExpression = string.Format("[{0}]={1} ", "MyColorIndex", dicMyColorIndex[rc.Col + "," + rc.Row].ToString());
                            pcin.Symbolizer.SetOutline(Color.Black, 1);
                            string[] strColor = brb.DrawingColor.Split(new char[] { ',' });
                            pcin.Symbolizer.SetFillColor(Color.FromArgb(Convert.ToInt32(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2])));
                            myScheme.Categories.Add(pcin);
                        }

                    }
                }


                if (myScheme.Categories.Count > 0)
                {
                    if (iRegionColor != dicMyColorIndex.Count)
                    {
                        PolygonCategory pcin = new PolygonCategory();
                        pcin.FilterExpression = string.Format("[{0}] not in({1})", "MyColorIndex", strrow);

                        pcin.Symbolizer.SetFillColor(Color.White);
                        pcin.Symbolizer.SetOutline(Color.Black, 1);
                        myScheme.Categories.Add(pcin);
                    }
                    (fl as IFeatureLayer).Symbology = myScheme;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void mainMap_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (!click || (_currentBenMAPRollback == null)) return;
                Rectangle rtol = new Rectangle(e.X - 8, e.Y - 8, 16, 16);
                Rectangle rstr = new Rectangle(e.X - 1, e.Y - 1, 2, 2);
                Extent tolerant = mainMap.PixelToProj(rtol);
                Extent strict = (new DotSpatial.Topology.Point(mainMap.PixelToProj(new Point(e.X, e.Y)))).Envelope.ToExtent();
                List<int> result = (mainMap.Layers[0] as IFeatureLayer).DataSet.SelectIndices(strict);
                IFeature fSelect = null;
                if (result.Count > 0)
                {
                    foreach (int iSelect in result)
                    {
                        IFeature fSelectTemp = (mainMap.Layers[0] as IFeatureLayer).DataSet.GetFeature(iSelect);
                        if (fSelectTemp.BasicGeometry is DotSpatial.Topology.Polygon)
                        {

                            if ((fSelectTemp.BasicGeometry as DotSpatial.Topology.Polygon).Contains(new DotSpatial.Topology.Point(mainMap.PixelToProj(new Point(e.X, e.Y)))))
                            {
                                fSelect = fSelectTemp;
                                break;
                            }
                        }
                        else
                        {
                            if ((fSelectTemp.BasicGeometry as DotSpatial.Topology.MultiPolygon).Contains(new DotSpatial.Topology.Point(mainMap.PixelToProj(new Point(e.X, e.Y)))))
                            {
                                fSelect = fSelectTemp;
                                break;

                            }

                        }
                    }

                    int iCol = Convert.ToInt32(fSelect.DataRow["COL"]);
                    int iRow = Convert.ToInt32(fSelect.DataRow["ROW"]);
                    RowCol iRowCol = new RowCol();
                    iRowCol.Col = iCol;
                    iRowCol.Row = iRow;
                    int iRemove = -1;
                    int i = 0;

                    if (_currentBenMAPRollback.SelectRegions.Contains(iRowCol, new RowColComparer()))
                    {
                        foreach (RowCol rowCol in _currentBenMAPRollback.SelectRegions)
                        {
                            if (rowCol.Col == iCol && rowCol.Row == iRow)
                            {
                                iRemove = i;
                                if (_currentBenMAPRollback.SelectRegions.Count == 1)
                                {
                                    string color = _currentBenMAPRollback.DrawingColor;
                                    _currentBenMAPRollback.DrawingColor = "255,255,255";
                                    ColorMap();
                                    _currentBenMAPRollback.DrawingColor = color;
                                }
                            }
                            i++;
                        }
                        _currentBenMAPRollback.SelectRegions.RemoveAt(iRemove);
                    }
                    else
                    {
                        foreach (BenMAPRollback brb in _monitorRollbackLine.BenMAPRollbacks)
                        {
                            i = 0;
                            if (_currentBenMAPRollback != brb)
                            {
                                if (brb.SelectRegions.Contains(iRowCol, new RowColComparer()))
                                {
                                    foreach (RowCol rowCol in brb.SelectRegions)
                                    {
                                        if (rowCol.Col == iCol && rowCol.Row == iRow)
                                        {
                                            iRemove = i;
                                        }
                                        i++;
                                    }
                                    if (iRemove >= 0)
                                    {
                                        brb.SelectRegions.RemoveAt(iRemove);
                                    }
                                }
                            }
                        }
                        _currentBenMAPRollback.SelectRegions.Add(iRowCol);
                    }

                    ColorMap();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentBenMAPRollback != null)
                {
                    int icount = 0;
                    List<RowCol> lstExist = new List<RowCol>();
                    foreach (BenMAPRollback br in _monitorRollbackLine.BenMAPRollbacks)
                    {
                        if (br.SelectRegions != null)
                        {
                            icount += br.SelectRegions.Count;
                            lstExist.AddRange(br.SelectRegions);
                        }
                    }
                    List<string> lstString = lstExist.Select(p => p.Col + "," + p.Row).ToList();
                    if ((mainMap.Layers[0] as IFeatureLayer).DataSet.DataTable.Rows.Count == icount) return;

                    foreach (DataRow dr in (mainMap.Layers[0] as IFeatureLayer).DataSet.DataTable.Rows)
                    {
                        RowCol iRowCol = new RowCol();
                        iRowCol.Col = Convert.ToInt32(dr["COL"]);
                        iRowCol.Row = Convert.ToInt32(dr["ROW"]);
                        if (!lstString.Contains(iRowCol.Col + "," + iRowCol.Row))
                        {
                            _currentBenMAPRollback.SelectRegions.Add(iRowCol);
                        }
                    }

                    ColorMap();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentBenMAPRollback != null)
                {
                    string color = _currentBenMAPRollback.DrawingColor;
                    _currentBenMAPRollback.DrawingColor = "255,255,255";
                    ColorMap();
                    _currentBenMAPRollback.SelectRegions = new List<RowCol>();
                    _currentBenMAPRollback.DrawingColor = color;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        bool click = true;
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            click = false;
            mainMap.FunctionMode = FunctionMode.ZoomIn;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            click = false;
            mainMap.FunctionMode = FunctionMode.ZoomOut;
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            click = false;
            mainMap.FunctionMode = FunctionMode.Pan;
        }

        private void btnFullExtent_Click(object sender, EventArgs e)
        {
            click = false;
            mainMap.ZoomToMaxExtent();
            mainMap.FunctionMode = FunctionMode.None;
            toolStripButton1_Click(sender, e);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            click = true;
            mainMap.FunctionMode = FunctionMode.None;
        }
    }
}