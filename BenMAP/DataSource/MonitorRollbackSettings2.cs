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
using GeoAPI.Geometries;

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
				PolygonLayer myLayer = mainMap.Layers[mainMap.Layers.Count - 1] as PolygonLayer;
				myLayer.DataSet.DataTable.Columns.Add("MyColorIndex", typeof(int));
				for (int i = 0; i < myLayer.DataSet.DataTable.Rows.Count; i++)
				{
					//dpa 1/12/2017 I modified this to actually use the MyColorIndex which wasn't really used before.
					myLayer.DataSet.DataTable.Rows[i]["MyColorIndex"] = 0; //dpa default all the features to be colored as "0"
					dicMyColorIndex.Add(Convert.ToInt32(myLayer.DataSet.DataTable.Rows[i]["COL"]).ToString() + "," + Convert.ToInt32(myLayer.DataSet.DataTable.Rows[i]["ROW"]).ToString(), i);
				}
				Color cRegion = Color.Transparent;
				PolygonSymbolizer TransparentRegion = new PolygonSymbolizer(cRegion);

				TransparentRegion.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1); myLayer.Symbolizer = TransparentRegion;

				lstMonitorValues = DataSourceCommonClass.GetMonitorData(_monitorRollbackLine.GridType, _monitorRollbackLine.Pollutant, _monitorRollbackLine);
				if (lstMonitorValues == null)
				{
					this.Hide();
					this.DialogResult = DialogResult.Cancel;
					MessageBox.Show("Please review your monitor data file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				IFeatureSet fsPoints = new FeatureSet();
				MonitorValue mv = null;
				List<Coordinate> lstCoordinate = new List<Coordinate>();
				List<double> fsInter = new List<double>();
				if (lstMonitorValues != null && lstMonitorValues.Count > 0)
				{
					//PolygonScheme myScheme = new PolygonScheme(); //unused
					PolygonCategory pcin = new PolygonCategory();
					pcin.Symbolizer.SetFillColor(Color.Red);
					//myScheme.Categories.Add(pcin); //dpa unused
					NetTopologySuite.Geometries.Point point;
					for (int i = 0; i < lstMonitorValues.Count; i++)
					{
						mv = lstMonitorValues[i];
						point = new NetTopologySuite.Geometries.Point(mv.Longitude, mv.Latitude);
						fsPoints.AddFeature(point);
					}
					MapPointLayer mpl = new MapPointLayer(fsPoints);
					mpl.Symbolizer = new PointSymbolizer();
					mpl.Symbolizer.SetFillColor(Color.FromArgb(0, 255, 255));
					mpl.Symbolizer.SetOutline(Color.Black, 1);
					mpl.SelectionEnabled = false;  //dpa 1/12/2017 don't need to be able to select from this layer.
					mainMap.Layers.Add(mpl);
				}

				//Create Add Region button in flow layout container
				Button btnRegionAdd = new Button();
				btnRegionAdd.Size = new Size(90, 27);
				btnRegionAdd.Tag = "Add Region";
				btnRegionAdd.Text = "Add Region";
				btnRegionAdd.Click += new System.EventHandler(this.btnAddRegion_Click);
				this.flowLayoutPanel1.Controls.Add(btnRegionAdd);
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
				if (_monitorRollbackLine.BenMAPRollbacks.Count < 1)
				{
					MessageBox.Show("You must add at least one region.");
					return;
				}
				foreach (BenMAPRollback brb in _monitorRollbackLine.BenMAPRollbacks)
				{
					if (brb.SelectRegions.Count < 1)
					{
						MessageBox.Show("Region " + brb.RegionID + " does not have any areas selected");
						return;
					}
				}
				if (chbExportAfterRollback.Checked)
				{
					SaveFileDialog saveFileDialog1 = new SaveFileDialog();
					saveFileDialog1.Filter = "CSV File|*.csv";
					saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath;
					saveFileDialog1.RestoreDirectory = true;
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
				int rollbackControlTop = 0;
				int addRegionButtonTop = 0;
				if (i + j + k != 0)
				{
					rollbackControlTop = i * 101 + j * 101 + k * 370 + (i + j + k - 1) * 1;
				}
				while (dicRegion.ContainsValue(regionnumber))
				{
					regionnumber++;
				}

				//Create Region Selector Radio Button
				RadioButton rbRegion = new RadioButton();
				rbRegion.AutoSize = true;
				rbRegion.AutoCheck = false;
				rbRegion.Tag = regionnumber.ToString();
				rbRegion.Size = new Size(35, 23);
				rbRegion.Location = new Point(5, 8);
				rbRegion.Click += new System.EventHandler(this.RegionSelect_Click);

				//Create Delete Region Button
				Button btnRegionDelete = new Button();
				btnRegionDelete.Tag = regionnumber.ToString();
				btnRegionDelete.Size = new Size(20, 20);
				btnRegionDelete.Padding = new Padding(0);
				btnRegionDelete.Location = new Point(157, 5);
				btnRegionDelete.Text = "X";
				btnRegionDelete.FlatStyle = FlatStyle.Flat;
				btnRegionDelete.Font = new Font(btnRegionDelete.Font.Name, btnRegionDelete.Font.Size, FontStyle.Bold);
				btnRegionDelete.Click += new System.EventHandler(this.btnRegionDelete_Click);

				//Create Region Label
				Label lblRegion = new Label();
				lblRegion.Text = "Region" + " " + regionnumber.ToString();
				lblRegion.Location = new Point(60, 8);
				lblRegion.Tag = regionnumber.ToString();
				lblRegion.Size = new Size(90, 15);
				lblRegion.Click += new System.EventHandler(this.RegionSelect_Click);

				//Create Region Color Tile
				Button btnRegionColor = new Button();
				btnRegionColor.Size = new Size(35, 23);
				btnRegionColor.Location = new Point(23, 3);
				btnRegionColor.Text = "";
				btnRegionColor.Tag = regionnumber.ToString();
				btnRegionDelete.FlatStyle = FlatStyle.Flat;
				Random randomPc = new Random();
				btnRegionColor.BackColor = Color.FromArgb(randomPc.Next(255), randomPc.Next(255), randomPc.Next(255));
				btnRegionColor.Click += new System.EventHandler(this.RegionSelect_Click);

				Regex reg = new Regex(@"\D");
				string s = reg.Replace(lblRegion.Text, "");
				int regionIDInt = int.Parse(s);

				switch (name)
				{
					case "Percentage":
						PercentageControl pc = new PercentageControl();
						pc.Location = new Point(0, rollbackControlTop);
						pc.Controls.Add(rbRegion);
						pc.Controls.Add(btnRegionColor);
						pc.Controls.Add(lblRegion);
						pc.Controls.Add(btnRegionDelete);
						pc.RegionID = regionIDInt;

						PercentageRollback pr = new PercentageRollback();
						pr.RegionID = regionIDInt;
						pr.DrawingColor = btnRegionColor.BackColor.R + "," + btnRegionColor.BackColor.G + "," + btnRegionColor.BackColor.B;
						pr.RegionID = regionIDInt;
						pr.SelectRegions = new List<RowCol>();
						pr.RollbackType = RollbackType.percentage;
						_monitorRollbackLine.BenMAPRollbacks.Add(pr);
						_currentBenMAPRollback = pr;
						pc._PercentageRollback = pr;
						this.flowLayoutPanel1.Controls.Add(pc);

						// Automatically select the newly added region
						rbRegion.PerformClick();

						dicRegion.Add(lblRegion.Text, regionIDInt);
						break;
					case "Incremental":
						IncrementalControl ic = new IncrementalControl();
						ic.Location = new Point(0, rollbackControlTop);
						ic.Controls.Add(rbRegion);
						ic.Controls.Add(btnRegionColor);
						ic.Controls.Add(lblRegion);
						ic.Controls.Add(btnRegionDelete);

						//ic.Name = lblRegion.Text;
						ic.RegionID = regionIDInt;

						IncrementalRollback ir = new IncrementalRollback();
						ir.RegionID = regionIDInt;
						ir.DrawingColor = btnRegionColor.BackColor.R + "," + btnRegionColor.BackColor.G + "," + btnRegionColor.BackColor.B;
						ir.SelectRegions = new List<RowCol>();
						ir.RollbackType = RollbackType.incremental;
						_monitorRollbackLine.BenMAPRollbacks.Add(ir);
						_currentBenMAPRollback = ir;
						ic._IncrementalRollback = ir;
						this.flowLayoutPanel1.Controls.Add(ic);

						// Automatically select the newly added region
						rbRegion.PerformClick();

						dicRegion.Add(lblRegion.Text, regionIDInt);
						break;
					case "ToStandard":
						ToStandardControl sc = new ToStandardControl(_monitorRollbackLine.Pollutant);
						sc.Location = new Point(0, rollbackControlTop);
						sc.Controls.Add(rbRegion);
						sc.Controls.Add(btnRegionColor);
						sc.Controls.Add(lblRegion);
						sc.Controls.Add(btnRegionDelete);
						sc.RegionID = regionIDInt;

						StandardRollback sr = new StandardRollback();
						sr.RegionID = regionIDInt;
						sr.DrawingColor = btnRegionColor.BackColor.R + "," + btnRegionColor.BackColor.G + "," + btnRegionColor.BackColor.B;
						sr.SelectRegions = new List<RowCol>();
						sr.RollbackType = RollbackType.standard;
						_monitorRollbackLine.BenMAPRollbacks.Add(sr);
						_currentBenMAPRollback = sr;
						sc._StandardRollback = sr;
						this.flowLayoutPanel1.Controls.Add(sc);

						// Automatically select the newly added region
						rbRegion.PerformClick();

						dicRegion.Add(lblRegion.Text, regionIDInt);
						break;
					default:
						break;
				}

				//Move Region Button to bottom
				this.flowLayoutPanel1.Controls.SetChildIndex((Control)sender, this.flowLayoutPanel1.Controls.Count);

				//Enable buttons if regions exist
				btnNext.Enabled = true;
				btnSelectAll.Enabled = true;
				btnDeleteAll.Enabled = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void RegionSelect_Click(object sender, EventArgs e)
		{
			try
			{
				string selectedRegionTag = (string)((Control)sender).Tag;

				// Update UI selection
				foreach (Control c in flowLayoutPanel1.Controls)
				{
					if (c.GetType() == typeof(Button))
					{
						//skip the Add Region button
										} else if (c.Controls[1].Tag.Equals(selectedRegionTag) )
					{
						((RadioButton)c.Controls[1]).Checked = true;
						c.BackColor = Color.White;
										} else {
						((RadioButton)c.Controls[1]).Checked = false;
						c.BackColor = Color.Transparent;
					}
				}

				// Update backend selection
				int regionIDInt = int.Parse(selectedRegionTag);

				foreach (BenMAPRollback rollback in _monitorRollbackLine.BenMAPRollbacks)
				{
					if (regionIDInt == rollback.RegionID)
					{
						_currentBenMAPRollback = rollback;
						break;
					}
				}
				//ColorMap(); dpa why do this just when changing a selection?
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void btnRegionDelete_Click(object sender, EventArgs e)
		{
			try
			{
				string selectedRegionTag = (string)((Control)sender).Tag;

				for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
				{
					if (flowLayoutPanel1.Controls[i].GetType().Equals(typeof(Button)))
					{
						//Skip the Add Region button
					}
					else if (flowLayoutPanel1.Controls[i].Controls[1].Tag.Equals(selectedRegionTag))
					{
						if (_monitorRollbackLine.BenMAPRollbacks.Count == 1)
						{
							_monitorRollbackLine.BenMAPRollbacks[0].DrawingColor = "255,255,255";
						}
						dicRegion.Remove(flowLayoutPanel1.Controls[i].Controls[3].Text);
						flowLayoutPanel1.Controls.RemoveAt(i);

						/* dpa 1/12/2017 
						 * it turns out that if you remove a region, the remaining ones keep their region ID. 
						 * This is good because we are now referencing the coloring scheme by region ID...
						 * So now we just need to reset any MyColorIndex fields related to this deleted region ID.
						 */
						foreach (DataRow dr in (mainMap.Layers[0] as IFeatureLayer).DataSet.DataTable.Rows)
						{
							if (Convert.ToInt32(dr["MyColorIndex"]) == _monitorRollbackLine.BenMAPRollbacks[i].RegionID)
							{
								dr["MyColorIndex"] = 0;
							}
						}

						//now it's safe to remove the region
						_monitorRollbackLine.BenMAPRollbacks.RemoveAt(i);

						//redo the coloring now that the region is gone.
						ColorMap();

						//Disable buttons if all regions are gone
						if (flowLayoutPanel1.Controls.Count == 1)
						{
							btnNext.Enabled = false;
							btnSelectAll.Enabled = false;
							btnDeleteAll.Enabled = false;
						}
						return;
					}
				}
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

		//dpa added this bool so we can see if we are in the process of processing a selection
		bool MapClicked = true;

		private void mainMap_SelectionChanged(object sender, EventArgs e)
		{
			try
			{
				//dpa 1/12/2017
				//We should be using this event instead of the mouse click event. Then we can use multi select as well. 
				//better to use the built in DotSpatial selection capabilities... no?
				//selectionchanged is called a lot. We only need to process the selection once during a process.
				if (MapClicked == true)
				{
					List<IFeature> SelectedFeatures = (mainMap.Layers[0] as IFeatureLayer).Selection.ToFeatureList();
					if (SelectedFeatures.Count > 0)
					{
						//Check if there is a current rollback to work on. If not, then clear the selection and leave the function.
						if (_currentBenMAPRollback == null)
						{
							MessageBox.Show("Please add a roll back region before selecting features.", "BenMAP Notification", MessageBoxButtons.OK);
							mainMap.ClearSelection();
							return;
						}
						PerformSelection(SelectedFeatures);
						MapClicked = false;
					}
				}
				else  //the event was raised without a map click so ignore it
				{
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}



		private void ColorMap()
		{
			//dpa 1/12/2017
			//This is a significant simplification of ColorMap. Code is cut in half and the speed is massively faster.
			try
			{
				// FeatureLayer fl = mainMap.Layers[0] as FeatureLayer;
				PolygonScheme myScheme = new PolygonScheme();
				PolygonCategory pc = new PolygonCategory();

				myScheme.Categories = new PolygonCategoryCollection();

				//Make everything white with black outline that has not been assigned to a region.
				pc = new PolygonCategory();
				pc.FilterExpression = "[MyColorIndex]=0 ";
				pc.Symbolizer.SetOutline(Color.Black, 1);
				pc.Symbolizer.SetFillColor(Color.White);
				myScheme.Categories.Add(pc);

				//for each of roll back regions, assign a new color category. 
				foreach (BenMAPRollback brb in _monitorRollbackLine.BenMAPRollbacks)
				{
					pc = new PolygonCategory();
					pc.FilterExpression = string.Format("[{0}]={1} ", "MyColorIndex", brb.RegionID.ToString());
					pc.Symbolizer.SetOutline(Color.Black, 1);
					string[] strColor = brb.DrawingColor.Split(new char[] { ',' });
					pc.Symbolizer.SetFillColor(Color.FromArgb(Convert.ToInt32(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2])));
					myScheme.Categories.Add(pc);
				}

					//apply the symbology to the layer.
					//(fl as IFeatureLayer).Symbology = myScheme;
					(mainMap.Layers[0] as IFeatureLayer).Symbology = myScheme;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}


		private void PerformSelection(List<IFeature> SelectedFeatures)
		{
			//dpa 1/12/2017 - cleaned up and simplified map selection, now allows multiple selection
			try
			{
				foreach (IFeature fSelect in SelectedFeatures)
				{
					int iCol = Convert.ToInt32(fSelect.DataRow["COL"]);
					int iRow = Convert.ToInt32(fSelect.DataRow["ROW"]);
					RowCol iRowCol = new RowCol();
					iRowCol.Col = iCol;
					iRowCol.Row = iRow;
					int iRemove = -1;
					int i = 0;

					bool isRemovedFromCurrent = false;
					int iRemoveCurrent = -1;

					//check to see if this feature exists in any of the rollbacks, excluding the current one
					//if it is there, then remove it
					foreach (BenMAPRollback brb in _monitorRollbackLine.BenMAPRollbacks)
					{
						i = 0;

						if (brb.RegionID != _currentBenMAPRollback.RegionID && SelectedFeatures.Count > 1)
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

					//jk 2/25/2017 - special case when clicking one selected feature in current rollback
					//this feature will be unselected
					//check to see if this feature exists in the current rollback
					if (_currentBenMAPRollback.SelectRegions.Contains(iRowCol, new RowColComparer()) && SelectedFeatures.Count == 1)
					{
						i = 0;
						foreach (RowCol rowCol in _currentBenMAPRollback.SelectRegions)
						{
							if (rowCol.Col == iCol && rowCol.Row == iRow)
							{
								iRemoveCurrent = i;
							}
							i++;
						}
						if (iRemoveCurrent >= 0)
						{
							_currentBenMAPRollback.SelectRegions.RemoveAt(iRemoveCurrent);
							fSelect.DataRow["MyColorIndex"] = 0;
						}
					}
					else
					{
						//now add the feature to the current rollback break
						_currentBenMAPRollback.SelectRegions.Add(iRowCol);

						//and change the layer MyColorIndex ID to be that of the current rollback
						fSelect.DataRow["MyColorIndex"] = _currentBenMAPRollback.RegionID;
					}

				}
				ColorMap();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			//dpa 1/12/2017 I moved all this code to a new function, SelectAll, and significantly simplified it.
			SelectAll();
		}

		private void DeSelectAll()
		{
			//dpa 1/12/2017
			//This replaces the code previously in the btnDeleteAll_click method and should be much faster for deselecting all features.

			try
			{
				if (_currentBenMAPRollback == null)
				{
					return;
				}

				//reset the selected features in the current rollback
				_currentBenMAPRollback.SelectRegions = new List<RowCol>();
				foreach (DataRow dr in (mainMap.Layers[0] as IFeatureLayer).DataSet.DataTable.Rows)
				{
					if (Convert.ToInt32(dr["MyColorIndex"]) == _currentBenMAPRollback.RegionID)
					{
						dr["MyColorIndex"] = 0;
					}

				}
				ColorMap();
			}

			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void SelectAll()
		{
			//dpa 1/12/2017
			//This replaces the code previously in the btnSelectAll_click method and should be much faster for selecting all features.
			try
			{
				if (_currentBenMAPRollback == null)
				{
					return;
				}

				foreach (BenMAPRollback br in _monitorRollbackLine.BenMAPRollbacks)
				{
					//clear out all the regions that have been selected in the different rollbacks
										if (br.SelectRegions.Count > 0) {
						br.SelectRegions.Clear();

					}
				}
				//now add all the row/cols of the current data/grid (actually a featureset/layer) to the current rollback
				foreach (DataRow dr in (mainMap.Layers[0] as IFeatureLayer).DataSet.DataTable.Rows)
				{
					RowCol iRowCol = new RowCol();
					iRowCol.Col = Convert.ToInt32(dr["COL"]);
					iRowCol.Row = Convert.ToInt32(dr["ROW"]);
					_currentBenMAPRollback.SelectRegions.Add(iRowCol);
					dr["MyColorIndex"] = _currentBenMAPRollback.RegionID;

				}

				//redraw the map
				ColorMap();

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}


		}

		private void btnDeleteAll_Click(object sender, EventArgs e)
		{
			//dpa 1/12/2017
			//Moved this code to DeSelectAll and simplified and sped it up.
			DeSelectAll();
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
			mainMap.FunctionMode = FunctionMode.ZoomIn;
		}

		private void btnZoomOut_Click(object sender, EventArgs e)
		{
			mainMap.FunctionMode = FunctionMode.ZoomOut;
		}

		private void btnPan_Click(object sender, EventArgs e)
		{
			mainMap.FunctionMode = FunctionMode.Pan;
		}

		private void btnFullExtent_Click(object sender, EventArgs e)
		{
			mainMap.ZoomToMaxExtent();
			mainMap.FunctionMode = FunctionMode.None;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			//dpa 1/12/2017 cleaner way to handle selection
			//change map to selection mode
			mainMap.FunctionMode = FunctionMode.Select;
		}

		private void mainMap_MouseUp(object sender, MouseEventArgs e)
		{
			//dpa 1/12/2017
			//this is needed to check if the selection event is part of a mouse click
			MapClicked = true;
		}

		/*
		 * dpa 1/12/2017 delted these codes...
		 * 
		private void mainMap_MouseClick(object sender, MouseEventArgs e)
		{
				//dpa 1/12/2017 Moved this content to "PerformSelection" which is called from mainMap_MouseUp
				try
				{
						if (!click || (_currentBenMAPRollback == null)) return;
						Rectangle rtol = new Rectangle(e.X - 8, e.Y - 8, 16, 16); 
						Rectangle rstr = new Rectangle(e.X - 1, e.Y - 1, 2, 2);
						Extent tolerant = mainMap.PixelToProj(rtol); 
						Extent strict = (new NetTopologySuite.Geometries.Point(mainMap.PixelToProj(new Point(e.X, e.Y)))).Envelope.EnvelopeInternal.ToExtent();
						List<int> result = (mainMap.Layers[0] as IFeatureLayer).DataSet.SelectIndices(strict);
						IFeature fSelect = null;
						if (result.Count > 0)
						{
								foreach (int iSelect in result)
								{
										IFeature fSelectTemp = (mainMap.Layers[0] as IFeatureLayer).DataSet.GetFeature(iSelect);
										if (fSelectTemp.Geometry is NetTopologySuite.Geometries.Polygon)
										{

												if ((fSelectTemp.Geometry as NetTopologySuite.Geometries.Polygon).Contains(new NetTopologySuite.Geometries.Point(mainMap.PixelToProj(new Point(e.X, e.Y)))))
												{
														fSelect = fSelectTemp;
														break;
												}
										}
										else
										{
												if ((fSelectTemp.Geometry as NetTopologySuite.Geometries.MultiPolygon).Contains(new NetTopologySuite.Geometries.Point(mainMap.PixelToProj(new Point(e.X, e.Y)))))
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
		 * 
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
	 */

	}
}