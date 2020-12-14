using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BenMAP.Configuration;
using BrightIdeasSoftware;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Projections;
using OxyPlot;
using OxyPlot.Series;
using ESIL.DBUtility;
using ProtoBuf;
using System.Collections;
using OxyPlot.Axes;
using System.ComponentModel.Composition;
using BenMAP.SelectByLocation;
using BenMAP.DataLayerExport;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Concurrent;

namespace BenMAP
{
	public partial class BenMAP : FormBase
	{
		[Export("Shell", typeof(ContainerControl))]
		private static ContainerControl Shell = new Form(); // Dummy form to hold default appmanager controls

		BenMAPGrid chartGrid;

		BenMAPGrid ChartGrid
		{
			get { return chartGrid; }
			set
			{
				chartGrid = value;
				CommonClass.changeGridType(chartGrid);
			}
		}

		FeatureSet fs36km = new FeatureSet();

		private Boolean _RaiseLayerChangeEvents = false;

		IMapLayerCollection _mapLayers = null;

		private const string _readyImageKey = "ready";

		private const string _unreadyImageKey = "unready";

		private const string _yibuImageKey = "yibu";

		private const string _errorImageKey = "error";

		public Main mainFrm = null;

		private string _CurrentMapTitle = "";

		private Extent _SavedExtent;

		private List<string> _listAddGridTo36km = new List<string>();

		private string _reportTableFileName = "";

		private FeatureSet _fsregion = new FeatureSet();

		private bool isLegendHide = false;

		string chartXAxis = ""; string strchartTitle = ""; string strchartX = ""; string strchartY = ""; string strCDFTitle = "";

		string strCDFX = "";

		string strCDFY = "";

		int iCDF = -1;

		bool canshowCDF = false;

		private string _drawStatus = string.Empty;

		private double _dMinValue = 0.0;

		private double _dMaxValue = 0.0;

		private IMapLayer _CurrentIMapLayer = null;

		private string _columnName = string.Empty;

		private string regionGroupLegendText = "Region Admin Layers";

		private string _bcgGroupLegendText = "Pollutants";

		private bool _HealthResultsDragged = false;

		private bool _IncidenceDragged = false;

		private bool _APVdragged = false;

		private object LayerObject = null; private string _currentNode = string.Empty; private string _homePageName;

		private string _currentImage = _unreadyImageKey;

		private string _currentAnsyNode = string.Empty;

		private string _currentPollutant = string.Empty;

		private StreamWriter sw;

		int _count = 0;

		List<CRSelectFunctionCalculateValue> lstCFGRforCDF = new List<CRSelectFunctionCalculateValue>();

		List<AllSelectCRFunction> lstCFGRpoolingforCDF = new List<AllSelectCRFunction>();

		List<AllSelectValuationMethodAndValue> lstAPVRforCDF = new List<AllSelectValuationMethodAndValue>();

		private readonly DataLayerExporter _dataLayerExporter;

		private IEnumerable _lastResult;

		TipFormGIF waitMess = new TipFormGIF();

		bool sFlog = true;

		public List<FieldCheck> cflstColumnRow;

		public List<FieldCheck> cflstHealth;

		public List<FieldCheck> cflstResult;

		public List<FieldCheck> IncidencelstColumnRow;

		public List<FieldCheck> IncidencelstHealth;

		public List<FieldCheck> IncidencelstResult;

		public List<FieldCheck> apvlstColumnRow;

		public List<FieldCheck> apvlstHealth;

		public List<FieldCheck> apvlstResult;

		public List<FieldCheck> qalylstColumnRow;

		public List<FieldCheck> qalylstHealth;

		public List<FieldCheck> qalylstResult;

		public List<string> strHealthImpactPercentiles;

		public List<string> strPoolIncidencePercentiles;

		public List<string> strAPVPercentiles;

		public bool assignHealthImpactPercentile;

		public bool assignPoolIncidencePercentile;

		public bool assignAPVPercentile;

		private Color[] _blendColors;

		private int _pageCurrent;

		private int _currentRow;

		private const int _pageSize = 50;

		private int _pageCount;

		public bool _MapAlreadyDisplayed = false;

		public object _tableObject;

		Dictionary<AllSelectQALYMethod, string> dicQALYPoolingAndAggregation;

		Dictionary<AllSelectQALYMethod, string> dicQALYPoolingAndAggregationUnPooled;

		public Dictionary<AllSelectCRFunction, string> dicIncidencePoolingAndAggregation;

		public Dictionary<AllSelectCRFunction, string> dicIncidencePoolingAndAggregationUnPooled;

		private List<AllSelectValuationMethod> lstAPVPoolingAndAggregationAll;

		public Dictionary<AllSelectValuationMethod, string> dicAPVPoolingAndAggregation;

		public Dictionary<AllSelectValuationMethod, string> dicAPVPoolingAndAggregationUnPooled;

		private string _AQGPath;

		private void BenMAP_Load(object sender, EventArgs e)
		{
			//set event handler for maplayers
			_mapLayers = mainMap.Layers;
			_mapLayers.ItemChanged += MapLayers_ItemChanged;

			//do remaining form loading activities
			CommonClass.SetupOLVEmptyListOverlay(this.olvCRFunctionResult.EmptyListMsgOverlay as TextOverlay);
			CommonClass.SetupOLVEmptyListOverlay(this.olvIncidence.EmptyListMsgOverlay as TextOverlay);
			CommonClass.SetupOLVEmptyListOverlay(this.tlvAPVResult.EmptyListMsgOverlay as TextOverlay);

			mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
			if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\CFGR"))
				System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\CFGR");
			if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\Project"))
				System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\Project");
			if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\APV"))
				System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\APV");
			if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\CFG"))
				System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\CFG");
			if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\APVR"))
				System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\APVR");
			if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\AQG"))
				System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\AQG");
			if (!Directory.Exists(CommonClass.ResultFilePath + @"\PopSim"))
				System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\PopSim");
			if (!Directory.Exists(CommonClass.DataFilePath + @"\Tmp"))
				System.IO.Directory.CreateDirectory(CommonClass.DataFilePath + @"\Tmp");

			bindingNavigatorCountItem.Enabled = true;
			bindingNavigatorMoveFirstItem.Enabled = true;
			bindingNavigatorMoveNextItem.Enabled = true;
			bindingNavigatorMoveLastItem.Enabled = true;
			bindingNavigatorMovePreviousItem.Enabled = true;
			bindingNavigatorPositionItem.Enabled = true;
			InitAggregationAndRegionList();
			Dictionary<string, string> dicSeasonStaticsAll = DataSourceCommonClass.DicSeasonStaticsAll;
			InitColumnsShowSet();

			this.treeListView.CanExpandGetter = delegate (object x)
			{
				return (x is TreeNode && (x as TreeNode).Nodes.Count > 0);
			};
			this.treeListView.ChildrenGetter = delegate (object x)
			{
				TreeNode dir = (TreeNode)x;
				try
				{
					return dir.Nodes;
				}
				catch (UnauthorizedAccessException ex)
				{
					MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return null;
				}
			};
			addAdminLayers();
			isLoad = true;
		}

		#region "GIS Data"

		/// <summary>
		/// This function is used to get the default layer from the database and load it on the map layer based on the setup name
		/// </summary>
		public void addAdminLayers()
		{
			try
			{
				_RaiseLayerChangeEvents = false;

				string setupID;
				string commandText;
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

				//Step 1: Get the SetupID based on the selected menu items
				commandText = string.Format("Select SETUPID from SETUPS where SETUPNAME = '{0}'", CommonClass.MainSetup.SetupName.ToString());
				object temp = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				setupID = temp.ToString();

				//Step 2: Get the GRIDDEFINITIONID's based on selected SETUP ID and True from IsAdmin Layer
				commandText = string.Format("select GRIDDEFINITIONID, GRIDDEFINITIONNAME, ISADMIN from GRIDDEFINITIONS WHERE SETUPID = {0} order by DRAWPRIORITY desc", setupID);
				System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				List<string> GridDefinitionIds = new List<string>();
				List<string> GridDefinitionNames = new List<string>();
				foreach (DataTable table in dsGrid.Tables)
				{
					foreach (DataRow row in table.Rows)
					{
						object col1Val = row[0].ToString();
						object col2Val = row[1].ToString();
						object col3Val = row[2];

						if (col3Val is DBNull) { }
						else
						{
							if (Convert.ToChar(col3Val) == 'T')
							{
								GridDefinitionIds.Add(Convert.ToString(col1Val));
								GridDefinitionNames.Add(Convert.ToString(col2Val));
							}
						}
					}
				}

				//Step 3: Get the shapefile names based on the GRIDDEFINITIONID
				List<string> shapeFileNames = new List<string>();
				foreach (string s in GridDefinitionIds)
				{

					// BENMAP-386: Query both tables to make sure we catch shapefile and regular grids
					commandText = string.Format(@"select shapefilename from shapefilegriddefinitiondetails where griddefinitionid = {0}
union
SELECT SHAPEFILENAME FROM REGULARGRIDDEFINITIONDETAILS where griddefinitionid = {0}", s);
					object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					shapeFileNames.Add(Convert.ToString(obj));
				}
				if (shapeFileNames.Count == 0)
				{
					MessageBox.Show("There are no Admin Layers configured in the current setup.\n\nPlease use the 'Modify Datasets' menu and the 'Manage' button under 'Grid Definitions' to edit a layer and mark it as an 'Admin Layer'.", "Missing Admin Layers", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				for (int i = 0; i < shapeFileNames.Count; i++)
				{
					string strPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + shapeFileNames[i] + ".shp";
					AddLayer(strPath, GridDefinitionNames[i], GridDefinitionIds[i]);
				}

				mainMap.ZoomToMaxExtent();
				//mainMap.ViewExtents = mainMap.GetAllLayers()[0].Extent;
				mainMap.Refresh();
				legend1.Refresh();
				_RaiseLayerChangeEvents = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private FeatureSet getThemeFeatureSet(int iValue, ref double MinValue, ref double MaxValue)
		{
			try
			{
				FeatureSet fsReturn = new FeatureSet();
				MinValue = 0;
				MaxValue = 0;

				Dictionary<string, double> dicValue = new Dictionary<string, double>();
				GridRelationship gRegionGridRelationship = null;
				foreach (GridRelationship gr in CommonClass.LstGridRelationshipAll)
				{
					if (gr.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID)
					{
						gRegionGridRelationship = gr;
					}
					else if (gr.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID)
					{
						gRegionGridRelationship = gr;
					}
				}

				int idCboAPV = Convert.ToInt32((cbAPVAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
				int idCboCFGR = Convert.ToInt32((this.cbCRAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
				int idCboQALY = -1; int idTo = Convert.ToInt32((cboRegion.SelectedItem as DataRowView)["GridDefinitionID"]);
				IFeatureSet fsValue = (mainMap.GetAllLayers()[0] as FeatureLayer).DataSet;
				IFeatureSet fsRegion = (mainMap.GetAllLayers()[1] as FeatureLayer).DataSet;
				int iRow = 0, iCol = 0;
				int i = 0;
				foreach (DataColumn dc in fsValue.DataTable.Columns)
				{
					if (dc.ColumnName.ToLower() == "col")
						iCol = i;
					if (dc.ColumnName.ToLower() == "row")
						iRow = i;

					i++;
				}
				foreach (DataRow dr in fsValue.DataTable.Rows)
				{
					dicValue.Add(dr[iCol].ToString() + "," + dr[iRow].ToString(), Convert.ToDouble(dr[iValue]));
				}
				fsReturn.DataTable.Columns.Add("Col", typeof(int));
				fsReturn.DataTable.Columns.Add("Row", typeof(int));
				fsReturn.DataTable.Columns.Add("ThemeValue", typeof(double));
				i = 0;
				//HIF
				if (_tableObject is List<CRSelectFunctionCalculateValue>)
				{
					CRSelectFunctionCalculateValue cr = ((List<CRSelectFunctionCalculateValue>)_tableObject).First();
					cr = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(cr, idCboCFGR == -1 ? CommonClass.GBenMAPGrid.GridDefinitionID : idCboCFGR, idTo);
					dicValue.Clear();
					foreach (CRCalculateValue crv in cr.CRCalculateValues)
					{
						if (!dicValue.ContainsKey(crv.Col + "," + crv.Row))
							dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);

					}
					foreach (DataRow dr in fsRegion.DataTable.Rows)
					{
						var geom = new NetTopologySuite.Geometries.Point(fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.X, fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.Y);
						fsReturn.AddFeature(geom);
						fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
						fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
						fsReturn.DataTable.Rows[i]["ThemeValue"] = 0;
						try
						{
							if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
							{
								fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
							}
						}
						catch (Exception ex)
						{ }
						i++;
					}
					return fsReturn;
				}
				//Valuation Pooling
				else if (_tableObject is List<AllSelectValuationMethodAndValue>)
				{
					AllSelectValuationMethodAndValue cr = ((List<AllSelectValuationMethodAndValue>)_tableObject).First();
					GridRelationship gr = null;
					int id = idCboAPV == -1 ? (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != -1) ? CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID : idCboAPV;
					BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(id);
					if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == id && p.smallGridID == idTo).Count() > 0)
					{
						gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == benMAPGrid.GridDefinitionID && p.smallGridID == idTo).First();
					}
					else if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).Count() > 0)
					{
						gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).First();

					}
					cr = APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gr, benMAPGrid, cr);
					dicValue.Clear();
					foreach (APVValueAttribute crv in cr.lstAPVValueAttributes)
					{
						if (!dicValue.ContainsKey(crv.Col + "," + crv.Row))
							dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);

					}
					foreach (DataRow dr in fsRegion.DataTable.Rows)
					{
						var geom = new NetTopologySuite.Geometries.Point(fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.X, fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.Y);
						fsReturn.AddFeature(geom);
						fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
						fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
						fsReturn.DataTable.Rows[i]["ThemeValue"] = 0;
						try
						{
							if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
							{
								fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
							}
						}
						catch (Exception ex)
						{ }
						i++;
					}
					return fsReturn;
				}
				//Incidence Pooling (new)
				else if (_tableObject is Dictionary<AllSelectCRFunction, string>)
				{
					Dictionary<AllSelectCRFunction, string> dicAllSelectCRFunctionPoolingName = (Dictionary<AllSelectCRFunction, string>)_tableObject;
					//YY: there was no code for Incidence Pooling. The button btnPieTheme may not be in use anymore. Leave this section blank
				}
				//QALY
				else if (_tableObject is List<AllSelectQALYMethodAndValue>)
				{
					AllSelectQALYMethodAndValue cr = ((List<AllSelectQALYMethodAndValue>)_tableObject).First();
					GridRelationship gr = null;
					int id = idCboQALY == -1 ? (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != -1) ? CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID : idCboQALY;
					BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(id);
					if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == id && p.smallGridID == idTo).Count() > 0)
					{
						gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == benMAPGrid.GridDefinitionID && p.smallGridID == idTo).First();
					}
					else if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).Count() > 0)
					{
						gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).First();

					}
					cr = APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gr, benMAPGrid, cr);
					dicValue.Clear();
					foreach (QALYValueAttribute crv in cr.lstQALYValueAttributes)
					{
						if (!dicValue.ContainsKey(crv.Col + "," + crv.Row))
							dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);

					}
					foreach (DataRow dr in fsRegion.DataTable.Rows)
					{
						var geom = new NetTopologySuite.Geometries.Point(fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.X, fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.Y);
						fsReturn.AddFeature(geom);
						fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
						fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
						fsReturn.DataTable.Rows[i]["ThemeValue"] = 0;
						try
						{
							if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
							{
								fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
							}
						}
						catch (Exception ex)
						{ }
						i++;
					}
					return fsReturn;
				}


				if (CommonClass.RBenMAPGrid.GridDefinitionID == CommonClass.GBenMAPGrid.GridDefinitionID)
				{
					i = 0;
					foreach (DataRow dr in fsRegion.DataTable.Rows)
					{
						var geom = new NetTopologySuite.Geometries.Point(fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.X, fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.Y);
						fsReturn.AddFeature(geom);
						fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
						fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
						try
						{
							if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
							{
								fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
								if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
							}
						}
						catch (Exception ex)
						{ }
						i++;
					}
					return fsReturn;
				}
				i = 0;
				foreach (DataRow dr in fsRegion.DataTable.Rows)
				{
					var geom = new NetTopologySuite.Geometries.Point(fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.X, fsRegion.GetFeature(i).Geometry.EnvelopeInternal.ToExtent().Center.Y);
					fsReturn.AddFeature(geom);
					fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
					fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
					if (gRegionGridRelationship.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID)
					{
						var query = from a in gRegionGridRelationship.lstGridRelationshipAttribute
												where a.bigGridRowCol.Col == Convert.ToInt32(fsReturn.DataTable.Rows[i]["Col"]) &&
														a.bigGridRowCol.Row == Convert.ToInt32(fsReturn.DataTable.Rows[i]["Row"])
												select a.smallGridRowCol;
						double d = 0;
						if (query != null && query.Count() > 0)
						{
							foreach (RowCol rc in query.First())
							{
								try
								{
									if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
										d += dicValue[rc.Col + "," + rc.Row];
								}
								catch (Exception ex)
								{ }
							}
							d = d / Convert.ToDouble(query.First().Count());
						}
						if (MinValue > d) MinValue = d;
						if (MaxValue < d) MaxValue = d;
						fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(d, 4);
					}
					else
					{
						var query = from a in gRegionGridRelationship.lstGridRelationshipAttribute
												where a.smallGridRowCol.Contains(new RowCol()
												{
													Col = Convert.ToInt32(fsReturn.DataTable.Rows[i]["Col"])
														,
													Row = Convert.ToInt32(fsReturn.DataTable.Rows[i]["Row"])
												}, new RowColComparer())
												select a.bigGridRowCol;
						if (query != null && query.Count() > 0)
						{
							RowCol rc = query.First();
							try
							{
								if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
								{
									fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
								}
								else
								{
									fsReturn.DataTable.Rows[i]["ThemeValue"] = 0.000;
								}
								if (MinValue > Math.Round(dicValue[rc.Col + "," + rc.Row], 12)) MinValue = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
								if (MaxValue < Math.Round(dicValue[rc.Col + "," + rc.Row], 12)) MaxValue = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
							}
							catch (Exception ex)
							{ }
						}
					}
					i++;
				}

				return fsReturn;
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		private void AddLayer(string strPath, string legendName, string gridID)
		{
			try
			{
				bool LayerFound = false;
				MapGroup RegionMapGroup = null;
				LayerFound = mainMap.GetAllLayers().Any<ILayer>(mylay => mylay.LegendText == legendName);
				if (!LayerFound)
				{
					mainMap.ProjectionModeReproject = ActionMode.Never;
					mainMap.ProjectionModeDefine = ActionMode.Never;
					if (!mainMap.GetAllLayers().Any<ILayer>(mylay => mylay.LegendText == regionGroupLegendText))
					{
						RegionMapGroup = AddMapGroup(regionGroupLegendText, "Map Layers", false, false);
					}
					MapPolygonLayer RegionReferenceLayer = new MapPolygonLayer();
				}
				implementSymbology(strPath, RegionMapGroup, legendName, gridID);
				//legend1.Refresh();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				Debug.WriteLine("Drawing layers: " + ex.ToString());
			}
		}


		private void implementSymbology(string shapefile, MapGroup RegionMapGroup, string legendName, string gridID)
		{
			if (RegionMapGroup != null)
			{
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				string commandText = "select OUTLINECOLOR from GRIDDEFINITIONS where GRIDDEFINITIONID =" + gridID + "";
				object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
				Color lineColor = System.Drawing.ColorTranslator.FromHtml(Convert.ToString(obj));
				PolygonSymbolizer polygonSym;
				IFeatureSet fs = FeatureSet.Open(shapefile);
				MapPolygonLayer polygonLayer = new MapPolygonLayer();

				if (fs.FeatureType == FeatureType.MultiPoint)
				{
					polygonLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(shapefile);
					polygonLayer.LegendText = legendName;
					PolygonSymbolizer StateRegionSym = new PolygonSymbolizer(Color.Transparent);
					StateRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1.5);
					polygonLayer.Symbolizer = StateRegionSym;
					polygonLayer.IsExpanded = true;
					polygonLayer.IsVisible = true;
				}
				else if (fs.FeatureType == FeatureType.Polygon)
				{
					polygonLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(shapefile);
					polygonLayer.LegendText = legendName;

					if (lineColor != null)
					{
						//Get the line color for this layer from the GridDefinitions table
						polygonSym = new PolygonSymbolizer(Color.Transparent);
						polygonSym.OutlineSymbolizer = new LineSymbolizer(lineColor, 1.5);
					}
					else
					{
						//Use a default color in case there is not a color specified in the table.
						polygonSym = new PolygonSymbolizer(Color.Transparent);
						polygonSym.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1.5);
					}
					polygonLayer.Symbolizer = polygonSym;
					polygonLayer.IsExpanded = true;
					polygonLayer.IsVisible = true;
				}
				else if (fs.FeatureType == FeatureType.Line)
				{
					LineLayer lineLayer = (MapLineLayer)RegionMapGroup.Layers.Add(shapefile);
					lineLayer.LegendText = legendName;
					LineSymbolizer lineSym = new LineSymbolizer(lineColor, 1.5);
					lineLayer.Symbolizer = lineSym;
					lineLayer.IsExpanded = true;
					lineLayer.IsVisible = true;
				}

				else if (fs.FeatureType == FeatureType.Point)
				{
					polygonLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(shapefile);
					polygonLayer.LegendText = legendName;
					PolygonSymbolizer StateRegionSym = new PolygonSymbolizer(Color.Transparent);
					StateRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1.5);
					polygonLayer.Symbolizer = StateRegionSym;
					polygonLayer.IsExpanded = true;
					polygonLayer.IsVisible = true;
				}

				else if (fs.FeatureType == FeatureType.Unspecified)
				{
					polygonLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(shapefile);
					polygonLayer.LegendText = legendName;
					PolygonSymbolizer StateRegionSym = new PolygonSymbolizer(Color.FromArgb(50, Color.LightCoral), Color.Black);
					StateRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1.0);
					polygonLayer.Symbolizer = StateRegionSym;
					polygonLayer.IsExpanded = true;
					polygonLayer.IsVisible = true;
				}

				else
				{
					polygonLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(shapefile);
					polygonLayer.LegendText = legendName;
					PolygonSymbolizer StateRegionSym = new PolygonSymbolizer(Color.FromArgb(50, Color.LightCoral), Color.Black);
					StateRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1.0);
					polygonLayer.Symbolizer = StateRegionSym;
					polygonLayer.IsExpanded = true;
					polygonLayer.IsVisible = true;
				}
				fs.Close();
			}
		}

		private MapGroup AddMapGroup(string mgName, string parentMGText, bool ShrinkOtherMG = false, bool TurnOffNonReference = false)
		{
			if (mgName == null || mgName == "") return null;   //confirm map group name is valid

			bool mgFound = false;
			MapGroup NewMapGroup = null;
			MapGroup ParentMapGroup = null;
			string parentText;

			//See if a map group with the Map group name already exists and find the name of the parent (if a map group)
			foreach (IMapLayer layer in mainMap.GetAllGroups())
			{
				if (layer is MapGroup || layer is IMapGroup)
				{
					if (layer.LegendText == mgName)//  && layer.GetParentItem().LegendText == parentMGText)
					{
						//Make sure the layer is in the same map group
						ParentMapGroup = (MapGroup)mainMap.GetAllGroups().Find(m => m.Contains(layer));
						if (ParentMapGroup == null)
						{
							//then assume top level
							parentText = "Map Layers";           //default parent item- so top level map groups are detected correctly
						}
						else
						{
							parentText = ParentMapGroup.LegendText;
						}

						if (layer.GetParentItem() != null)   //MCB--problem getting the parent map group of some map groups??????
						{
							parentText = layer.GetParentItem().LegendText;
						}

						if (parentText == parentMGText)      //Map group already exists
						{
							NewMapGroup = (MapGroup)layer;
							mgFound = true;
							break;
						}
					}
					else
					{
						if (ShrinkOtherMG)
						{
							layer.IsExpanded = false;             // Unexpand other mapgroups to increase display room for new layer    
						}
						if (layer.LegendText != regionGroupLegendText && parentMGText != regionGroupLegendText)
						{
							if (TurnOffNonReference)
							{
								layer.IsVisible = false;        //turn off other layers
							}
						}
					}
					if (layer.LegendText == parentMGText) ParentMapGroup = (MapGroup)layer;
				}
			}

			if (!mgFound)  //New map group not found already, so add it
			{
				NewMapGroup = new MapGroup();
				NewMapGroup.Layers = new MapLayerCollection(mainMap.MapFrame, NewMapGroup, null);  // This is neccessary for manually created groups
				NewMapGroup.LegendText = mgName;

				if (parentMGText == "Map Layers")  //add map group at top level
				{
					mainMap.Layers.Add(NewMapGroup);
				}
				else
				{
					if (ParentMapGroup == null)
					{
						ParentMapGroup = (MapGroup)mainMap.GetAllGroups().Find(m => m.LegendText == parentMGText);
					}
					if (ParentMapGroup == null)  //If parent still null then we can't find a map group to put this layer in.
					{
						mainMap.Layers.Add(NewMapGroup);   //adding it to top level
					}
					else  //Add new group under it's parent Map Group
					{
						ParentMapGroup.Add(NewMapGroup);
					}
					NewMapGroup.SetParentItem(ParentMapGroup);
				}

			}
			return NewMapGroup;
		}

		private void mainMap_DragEnter(object sender, DragEventArgs e)
		{
			Debug.WriteLine("mainMap_DragEnter");
			if (_HealthResultsDragged)
			{
				btShowCRResult_Click(sender, e);
				_HealthResultsDragged = false;
			}

			if (_IncidenceDragged)
			{
				tlvIncidence_DoubleClick(sender, e);
				_IncidenceDragged = false;
			}

			if (_APVdragged)
			{
				tlvAPVResult_DoubleClick(sender, e);
				_APVdragged = false;
			}
			return;
		}

		private void mainMap_DragLeave(object sender, EventArgs e)
		{
			Debug.WriteLine("mainMap_DragLeave");
			_HealthResultsDragged = false;
			_IncidenceDragged = false;
			_APVdragged = false;
			return;
		}



		void DrawMapResults(List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue, Boolean bTable)
		{
			//BenMAP-400  Previously code relied on the first instance of a list to generate layer, now updated to cycle through each set of results
			//code for drawing the incidence data results in the DotSpatial map.               
			//CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = null;
			//crSelectFunctionCalculateValue = lstCRSelectFunctionCalculateValue.First();

			if (_tableObject == null)
			{
				InitTableResult(lstCRSelectFunctionCalculateValue);
				if (!bTable)
				{
					SetOLVResultsShowObjects(null);
				}
			}

			//Add Pollutants Mapgroup if it doesn't exist already -MCB
			MapGroup ResultsMapGroup = AddMapGroup("Results", "Map Layers", false, false);
			MapGroup HIFResultsMapGroup = AddMapGroup("Health Impacts", "Results", false, false);
			//MapGroup adminLayerMapGroup = AddMapGroup(regionGroupLegendText, regionGroupLegendText, false, false);

			foreach (CRSelectFunctionCalculateValue cr in lstCRSelectFunctionCalculateValue)
			{
				string author;

				author = cr.CRSelectFunction.BenMAPHealthImpactFunction.Author;
				if (author.IndexOf(" ") != -1)
				{
					author = author.Substring(0, author.IndexOf(" "));
				}

				string LayerNameText = author + " " + cr.CRSelectFunction.BenMAPHealthImpactFunction.Year;          //Distinguishing between author by Year field

				//Remove the old version of the layer if exists already
				RemoveOldPolygonLayer(LayerNameText, HIFResultsMapGroup.Layers, false);

				//set change projection text
				string changeProjText = "change projection to setup projection";
				if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
				{
					changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
				}
				tsbChangeProjection.Text = changeProjText;

				mainMap.ProjectionModeReproject = ActionMode.Never;
				mainMap.ProjectionModeDefine = ActionMode.Never;
				string shapeFileName = "";

				if (CommonClass.GBenMAPGrid is ShapefileGrid)
				{
					if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
					{
						shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
					}
				}
				else if (CommonClass.GBenMAPGrid is RegularGrid)
				{
					if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
					{
						shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
					}
				}

				//bring the shapefile into memory as a polygon layer object
				//BenMAP-477: Addressed the serious time lag by implementing parallel access of results & reverting to original approach of directly updating the polygon layer dataset's datatable
				MapPolygonLayer polLayer = (MapPolygonLayer)HIFResultsMapGroup.Layers.Add(shapeFileName);
				DataTable dt = polLayer.DataSet.DataTable;
				List<string> lstRemoveName = new List<string>();

				//remove all fields that aren't the row or column identifier
				for (int j = 0; j < dt.Columns.Count; j++)
				{
					if (dt.Columns[j].ColumnName.ToLower() == "col" || dt.Columns[j].ColumnName.ToLower() == "row")
					{ }
					else
						lstRemoveName.Add(dt.Columns[j].ColumnName);
				}
				foreach (string s in lstRemoveName)
				{
					dt.Columns.Remove(s);
				}

				//BenMAP-400: Previous code only generated point estimate data with a given layer, making it difficult to know other data points at the time of export
				//Previously, the code took whatever information was in the "Data" tab and appended it to the point estimate calculated here.
				//In order to avoid creating a data table twice, this code now builds a datatable based on default/selected variables--this makes the drawing portion take much longer, but the export much faster (and now includes the correct data)
				dt.Columns.Add("Point Estimate", typeof(double));
				if (cflstHealth != null)
				{
					foreach (FieldCheck fieldCheck in cflstHealth)
					{
						if (fieldCheck.isChecked && fieldCheck.FieldName != "Start Age" && fieldCheck.FieldName != "End Age")
						{
							dt.Columns.Add(fieldCheck.FieldName, typeof(string));
						}

						if (fieldCheck.isChecked && fieldCheck.FieldName == "Start Age" || fieldCheck.FieldName == "End Age")
						{
							dt.Columns.Add(fieldCheck.FieldName, typeof(int));
						}
					}
				}

				if (cflstResult != null)
				{
					foreach (FieldCheck fieldCheck in cflstResult)
					{
						if (fieldCheck.isChecked && (fieldCheck.FieldName != "Percentiles" && fieldCheck.FieldName != "Point Estimate"))
						{
							dt.Columns.Add(fieldCheck.FieldName, typeof(double));
						}
					}

					if ((cflstResult.Find(p => p.FieldName.Equals("Percentiles")).isChecked) && cr.CRCalculateValues.First().LstPercentile != null)
					{
						int numPercentiles = cr.CRSelectFunction.lstLatinPoints.First().values.Count;
						double step = 100 / (double)numPercentiles;
						double current = step / 2;

						for (int j = 0; j < numPercentiles; j++)
						{
							string currLabel = String.Concat("Percentile ", current.ToString());
							dt.Columns.Add(currLabel, typeof(float));
							current += step;
						}
					}
				}
				else
				{
					dt.Columns.Add("Population", typeof(double));
					dt.Columns.Add("Delta", typeof(double));
					dt.Columns.Add("Mean", typeof(double));
					dt.Columns.Add("Baseline", typeof(double));
					dt.Columns.Add("Percent Of Baseline", typeof(double));
					dt.Columns.Add("Standard Deviation", typeof(double));
					dt.Columns.Add("Variance", typeof(double));

					int numPercentiles = cr.CRSelectFunction.lstLatinPoints.First().values.Count;
					double step = 100 / (double)numPercentiles;
					double current = step / 2;

					for (int j = 0; j < numPercentiles; j++)
					{
						string currLabel = String.Concat("Percentile ", current.ToString());
						dt.Columns.Add(currLabel, typeof(float));
						current += step;
					}
				}

				//Once datatable columns are set, gather results into a concurrent dictionary based on col/row
				ConcurrentDictionary<string, Dictionary<string, string>> dictResults = new ConcurrentDictionary<string, Dictionary<string, string>>();

				Parallel.ForEach(cr.CRCalculateValues, crcv =>
				{
					string keyCoord = crcv.Col + "," + crcv.Row;

					Dictionary<string, string> crResults = new Dictionary<string, string>();

					if (cflstHealth != null)
					{
						foreach (FieldCheck fieldCheck in cflstHealth)
						{
							if (fieldCheck.isChecked)
							{
								var fieldResult = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, cr.CRSelectFunction);
								crResults.Add(fieldCheck.FieldName, Convert.ToString(fieldResult));
							}
						}
					}
				
					if (cflstResult == null)
					{
						crResults.Add("Point Estimate", Convert.ToString(crcv.PointEstimate));
						crResults.Add("Population", Convert.ToString(crcv.Population));
						crResults.Add("Delta", Convert.ToString(crcv.Delta));
						crResults.Add("Mean", Convert.ToString(crcv.Mean));
						crResults.Add("Baseline", Convert.ToString(crcv.Baseline));
						crResults.Add("Percent of Baseline", Convert.ToString(crcv.PercentOfBaseline));
						crResults.Add("Standard Deviation", Convert.ToString(crcv.StandardDeviation));
						crResults.Add("Variance", Convert.ToString(crcv.Variance));

						int i = 0;
						while (i < crcv.LstPercentile.Count())
						{
							crResults.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crcv.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crcv.LstPercentile.Count()))))), Convert.ToString(crcv.LstPercentile[i]));
							i++;
						}
					}
					else
					{
						foreach (FieldCheck fieldCheck in cflstResult)
						{
							if (fieldCheck.isChecked && fieldCheck.FieldName != "Percentiles" && fieldCheck.FieldName != "Population Weighted Delta"
							&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
							{
								var fieldResult = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, cr.CRSelectFunction);
								crResults.Add(fieldCheck.FieldName, Convert.ToString(fieldResult));
							}
							int i = 0;
							if (fieldCheck.isChecked && fieldCheck.FieldName == "Percentiles")
							{
								while (i < crcv.LstPercentile.Count())
								{
									crResults.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crcv.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crcv.LstPercentile.Count()))))), Convert.ToString(crcv.LstPercentile[i]));
									i++;
								}
							}
						}
					}

					dictResults.TryAdd(keyCoord, crResults);
				});

				//Remove rows in the layer datatable if there are no HIF results
				List<int> IndicestoRemove = new List<int>();

				for (int idx = 0; idx < dt.Rows.Count; idx++)
				{
					DataRow dr = dt.Rows[idx];
					string keyCoord = dr["COL"] + "," + dr["ROW"];

					if (dictResults.TryGetValue(keyCoord, out Dictionary<string, string> currResults))
					{
						foreach (KeyValuePair<string, string> kvp in currResults)
						{
							Type colType = dt.Columns[kvp.Key].DataType;
							dr[kvp.Key] = Convert.ChangeType(kvp.Value, colType);
						}
					}
					else
						IndicestoRemove.Add(idx);
				}

				polLayer.RemoveFeaturesAt(IndicestoRemove);

				//save the current in-memory layer to a temporary shapefile
				if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
				polLayer.DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);

				//set up the legend
				polLayer.LegendText = author + " " + cr.CRSelectFunction.BenMAPHealthImpactFunction.Year;
				polLayer.Name = author;
				string strValueField = polLayer.DataSet.DataTable.Columns["Point Estimate"].ColumnName;
				_columnName = strValueField;

				//build symbology 
				polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "R"); //-MCB added

				double dMinValue = 0.0;
				double dMaxValue = 0.0;
				dMinValue = cr.CRCalculateValues.Min(a => a.PointEstimate);
				dMaxValue = cr.CRCalculateValues.Max(a => a.PointEstimate);

				_dMinValue = dMinValue;
				_dMaxValue = dMaxValue;

				_CurrentIMapLayer = polLayer;
				string pollutantUnit = string.Empty;
				_columnName = strValueField;
			}
		}



		#region Map toolbar functions
		//dpa moved all these map toolbar functions into a single region block
		private void tsbChangeProjection_Click(object sender, EventArgs e)
		{
			try
			{
				if (mainMap.GetAllLayers().Count == 0) return;

				//exit if we have no setup projection
				if (String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
				{
					return;
				}

				ProjectionInfo setupProjection = CommonClass.getProjectionInfoFromName(CommonClass.MainSetup.SetupProjection);
				if (setupProjection == null)
				{
					return;
				}

				if (mainMap.Projection != setupProjection)
				{
					mainMap.Projection = setupProjection;
					foreach (FeatureLayer layer in mainMap.GetAllLayers())
					{
						layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
						layer.Reproject(mainMap.Projection);
					}
					tsbChangeProjection.Text = "change projection to WGS1984";
				}
				else
				{
					mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
					foreach (FeatureLayer layer in mainMap.GetAllLayers())
					{
						layer.Projection = setupProjection;
						layer.Reproject(mainMap.Projection);
					}
					tsbChangeProjection.Text = "change projection to setup projection (" + CommonClass.MainSetup.SetupProjection + ")";
				}

				mainMap.Projection.CopyProperties(mainMap.Projection);
				mainMap.ZoomToMaxExtent();
				_SavedExtent = mainMap.ViewExtents;

			}
			catch (Exception ex)
			{
			}
		}


		private void tsbAddLayer_Click(object sender, EventArgs e)
		{
			IMapLayer mylayer = mainMap.AddLayer();
		}

		private void tsbSaveShapefile_Click(object sender, EventArgs e)
		{
			_dataLayerExporter.ShowExportWindow();
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
			//dpa - change the map mode to zoom in 
			//this button toggles map mode, hence changing the "checked" state.
			mainMap.FunctionMode = FunctionMode.ZoomIn;
			btnSelect.Checked = false;
			btnIdentify.Checked = false;
			btnZoomIn.Checked = true;
			btnZoomOut.Checked = false;
			btnPan.Checked = false;
		}

		private void btnZoomOut_Click(object sender, EventArgs e)
		{
			//dpa - change the map mode to zoom out
			//this button toggles map mode, hence changing the "checked" state.
			mainMap.FunctionMode = FunctionMode.ZoomOut;
			btnSelect.Checked = false;
			btnIdentify.Checked = false;
			btnZoomIn.Checked = false;
			btnZoomOut.Checked = true;
			btnPan.Checked = false;

		}

		private void btnIdentify_Click(object sender, EventArgs e)
		{
			//dpa - change function mode to identify
			//this button toggles map mode, hence changing the "checked" state.
			mainMap.FunctionMode = FunctionMode.Info;
			btnSelect.Checked = false;
			btnIdentify.Checked = true;
			btnZoomIn.Checked = false;
			btnZoomOut.Checked = false;
			btnPan.Checked = false;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			//dpa - change map cursor mode to selection
			//this button toggles map mode, hence changing the "checked" state.
			mainMap.FunctionMode = FunctionMode.Select;
			btnSelect.Checked = true;
			btnIdentify.Checked = false;
			btnZoomIn.Checked = false;
			btnZoomOut.Checked = false;
			btnPan.Checked = false;
		}

		private void btnPan_Click(object sender, EventArgs e)
		{
			//dpa - change the map mode to pan
			//this button toggles map mode, hence changing the "checked" state.
			mainMap.FunctionMode = FunctionMode.Pan;
			btnSelect.Checked = false;
			btnIdentify.Checked = false;
			btnZoomIn.Checked = false;
			btnZoomOut.Checked = false;
			btnPan.Checked = true;
		}

		private void btnFullExtent_Click(object sender, EventArgs e)
		{
			//dpa - zoom to the map full extent
			mainMap.ZoomToMaxExtent();
			mainMap.FunctionMode = FunctionMode.None;
		}

		private void tsbSelectByLocation_Click(object sender, EventArgs e)
		{
			//dpa - show the select by location dialog box.
			if (_SelectByLocationDialogShown) return;

			_SelectByLocationDialogShown = true;
			var sb = new SelectByLocationDialog(mainMap);
			sb.Closed += SbOnClosed;
			sb.Show(this);
		}

		private void btnClearSelection_Click(object sender, EventArgs e)
		{
			//dpa - clear selected features from the map.
			mainMap.ClearSelection();
		}


		#endregion

		#endregion


		public BenMAP(string homePageName)
		{
			try
			{
				InitializeComponent();
				Control.CheckForIllegalCrossThreadCalls = false;
				_homePageName = homePageName;
				splitContainer1.Visible = false;
				oxyPlotView.Visible = false;
				this.tabCtlReport.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
				this.tabCtlReport.DrawItem += new DrawItemEventHandler(DrawTabControlItems);

				CommonClass.NodeAnscy -= ChangeNodeStatus;
				CommonClass.NodeAnscy += ChangeNodeStatus;

				_dataLayerExporter = new DataLayerExporter(mainMap, this, OLVResultsShow, () => _lastResult);
				appManager1.LoadExtensions();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		public string HomePageName
		{
			get { return _homePageName; }
			set { _homePageName = value; }
		}



		private void ChangeNodeStatus()
		{
			try
			{
				string ansy = string.Empty;
				lock (CommonClass.NodeAnscyStatus)
				{
					if (CommonClass.NodeAnscyStatus != string.Empty)
					{
						ansy = CommonClass.NodeAnscyStatus;
						string[] tmps = ansy.Split(new char[] { ';' }); _currentPollutant = tmps[0].ToLower();
						_currentAnsyNode = tmps[1];
						if (tmps[2] == "on") { _currentImage = _yibuImageKey; }
						else if (tmps[2] == "off") { _currentImage = _readyImageKey; }
						foreach (TreeNode node in trvSetting.Nodes)
						{
							RecursiveQuery(node);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void RecursiveQuery(TreeNode node)
		{
			try
			{
				if (_currentAnsyNode == node.Name)
				{
					string nodeTag = node.Parent.Tag.ToString();
					string pollutantName = "";
					foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
					{
						if (int.Parse(nodeTag) == b.Pollutant.PollutantID)
						{
							pollutantName = b.Pollutant.PollutantName;
						}
					}
					if (_currentPollutant.ToLower() == pollutantName.ToLower())
					{
						node.ImageKey = _currentImage;
						node.SelectedImageKey = _currentImage;

						return;
					}
				}
				else
				{
					if (node.Nodes.Count > 0)
					{
						foreach (TreeNode child in node.Nodes)
						{
							RecursiveQuery(child);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void SetTabControl(TabControl tc)
		{
			try
			{
				if (tc == null) { return; }
				for (int i = tc.TabPages.Count - 2; i >= 0; i--)
				{
				}
				tc.Refresh();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void DrawTabControlItems(object sender, DrawItemEventArgs e)
		{
			try
			{
				Font tabFont = null;
				Brush bBackColor = null;
				Brush bForeColor = null;
				string tag = tabCtlReport.TabPages[e.Index].Tag.ToString().ToLower();
				string selectedPage = tabCtlReport.SelectedTab.Tag.ToString().ToLower();

				tabFont = new System.Drawing.Font("Cambri", 9, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				int iImageKey = -1;
				switch (tag)
				{
					case "function":
						iImageKey = 12;
						break;
					case "apvx":
						iImageKey = 13;
						break;
					case "incidence":
						iImageKey = 13;
						break;
					case "qaly":
						iImageKey = 13;
						break;
				}
				if ((tag == "function") && (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count > 0 && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues != null))
				{
					bForeColor = Brushes.Black;
					if (tag == selectedPage)
					{
						bBackColor = new SolidBrush(Color.White);
					}
					else
					{
						bBackColor = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
					}
					iImageKey = 12;
				}
				else if ((tag == "apvx") && (cbPoolingWindowAPV != null && cbPoolingWindowAPV.Items.Count > 0))
				{
					bForeColor = Brushes.Black;
					if (tag == selectedPage)
					{
						bBackColor = new SolidBrush(Color.White);
					}
					else
					{
						bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
					}
					iImageKey = 13;
				}
				else if ((tag == "incidence") && (cbPoolingWindowIncidence != null && cbPoolingWindowIncidence.Items.Count > 0))
				{
					bForeColor = Brushes.Black;
					if (tag == selectedPage)
					{
						bBackColor = new SolidBrush(Color.White);
					}
					else
					{
						bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
					}
					iImageKey = 13;
				}
				else if (tag == "audit")
				{
					bForeColor = Brushes.Black;
					if (tag == selectedPage)
					{
						bBackColor = new SolidBrush(Color.White);
					}
					else
					{
						bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
					}
				}
				else
				{
					bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
					bForeColor = Brushes.Gray;
				}
				string tabName = this.tabCtlReport.TabPages[e.Index].Text;
				StringFormat sftTab = new StringFormat();
				e.Graphics.FillRectangle(bBackColor, e.Bounds);
				Rectangle recTab = e.Bounds;
				if (iImageKey != -1)
				{

					recTab = new Rectangle(recTab.X + 18, recTab.Y + 4, recTab.Width - 12, recTab.Height - 4);
					e.Graphics.DrawImage(SmallImageList.Images[iImageKey], (new Point(e.Bounds.X + 2, e.Bounds.Y + 4)));
					e.Graphics.DrawString(tabName, tabFont, bForeColor, recTab, sftTab);
				}
				else
				{
					recTab = new Rectangle(recTab.X, recTab.Y + 4, recTab.Width + 6, recTab.Height - 4);
					e.Graphics.DrawString(tabName, tabFont, bForeColor, recTab, sftTab);
				}

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}



		private bool AddData2CommonClass(TreeView tree)
		{
			try
			{
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return false;
			}
		}

		public void NewFile()
		{
			splitContainer1.Visible = true;
			CommonClass.ClearAllObject();
			_MapAlreadyDisplayed = false;
			ClearAll();
			ResetParamsTree(Application.StartupPath + @"\Configs\ParamsTree_USA.xml");
		}



		public void ClearAll()
		{
			try
			{
				if (!_MapAlreadyDisplayed)
				{
					_RaiseLayerChangeEvents = false;
					mainMap.Layers.Clear();
					_RaiseLayerChangeEvents = true;
				}
				pnlChart.BackgroundImage = null;
				tabCtlMain.SelectTab(tabGIS);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		public string ProjFileName = "";

		public void OpenProject()
		{
			try
			{
				OpenFileDialog openfile = new OpenFileDialog();
				openfile.InitialDirectory = CommonClass.ResultFilePath + @"\Result\Project";
				openfile.Filter = "BenMAP Project File(*.projx)|*.projx";
				openfile.FilterIndex = 1;
				openfile.RestoreDirectory = true;
				if (openfile.ShowDialog() != DialogResult.OK)
				{ ProjFileName = ""; return; }
				else
					ProjFileName = openfile.FileName;
				WaitShow("Loading project file");


				if (!CommonClass.LoadBenMAPProject(openfile.FileName))
				{
					WaitClose();
					MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");
					return;
				}
				this.OpenFile();
				CommonClass.LstPollutant = null; CommonClass.RBenMAPGrid = null;
				CommonClass.GBenMAPGrid = null; CommonClass.LstBaseControlGroup = null; CommonClass.CRThreshold = 0; CommonClass.CRLatinHypercubePoints = 20; CommonClass.CRRunInPointMode = false;
				CommonClass.BenMAPPopulation = null;
				CommonClass.BaseControlCRSelectFunction = null; CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
				CommonClass.lstIncidencePoolingAndAggregation = null;

				CommonClass.IncidencePoolingResult = null;
				CommonClass.ValuationMethodPoolingAndAggregation = null;
				CommonClass.BaseControlCRSelectFunction = null;
				CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
				CommonClass.ValuationMethodPoolingAndAggregation = null;
				GC.Collect();
				CommonClass.LoadBenMAPProject(openfile.FileName);
				BenMAP_Load(this, null);
				if (CommonClass.ValuationMethodPoolingAndAggregation != null)
				{
					CommonClass.lstIncidencePoolingAndAggregation = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList();
					CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
					cbPoolingWindowAPV.Items.Clear();
					CommonClass.BaseControlCRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
					CommonClass.BaseControlCRSelectFunction = null;
					CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
					CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
					CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
					CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
					CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
					CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
					CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
					CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
					CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
					CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
					for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
					{
						CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
					}
					try
					{
						if (CommonClass.BaseControlCRSelectFunction != null)
						{
							showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
						Logger.LogError(ex);
					}

					errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]); //health impact functions
																																													//errorNodeImage(trvSetting.Nodes[2].Nodes[0]); //aggregation
					errorNodeImage(trvSetting.Nodes[2].Nodes[1]); //pooling
					errorNodeImage(trvSetting.Nodes[2].Nodes[2]); //valuation



				}
				else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
				{
					errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
					showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);
				}
				else if (CommonClass.BaseControlCRSelectFunction != null)
				{
					errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
					showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

				}
				else if (CommonClass.LstBaseControlGroup != null && CommonClass.LstBaseControlGroup.Count > 0)
				{
					int nodesCount = 0;
					foreach (TreeNode trchild in trvSetting.Nodes)
					{
						if (trchild.Name == "airqualitygridgroup")
						{
							nodesCount = trchild.Nodes.Count;


							for (int i = nodesCount - 1; i > -1; i--)
							{
								TreeNode node = trchild.Nodes[i];
								if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
							}
							for (int i = CommonClass.LstBaseControlGroup.Count - 1; i > -1; i--)
							{
								AddDataSourceNode(CommonClass.LstBaseControlGroup[i], trchild);
								if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null)
								{
									changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[0]);
								}
								if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Control != null)
								{
									changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[1]);
								}
								if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null && CommonClass.LstBaseControlGroup[i].Control != null)
								{
									changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1]);
								}
							}
							trchild.ExpandAll();

							foreach (TreeNode trair in trchild.Nodes)
							{
								if (trair.Name != "datasource")
									changeNodeImage(trair);
								TreeNode tr = trair;
								if (trair.Name == "gridtype")
								{
									AddChildNodes(ref tr, "", "", new BenMAPLine());
									trair.ExpandAll();
								}
							}
						}
						if (trchild.Name == "configuration")
						{
							foreach (TreeNode tr in trchild.Nodes)
							{
								initNodeImage(tr);
							}
							trchild.ExpandAll();
						}
						if (trchild.Name == "aggregationpoolingvaluation")
						{
							foreach (TreeNode tr in trchild.Nodes)
							{
								initNodeImage(tr);
							}
							trchild.ExpandAll();
						}
					}
				}
				else
				{
					if (CommonClass.GBenMAPGrid != null)
					{
						TreeNode currentNode = trvSetting.Nodes[0].Nodes["gridtype"];
						AddChildNodes(ref currentNode, "", "", null);
						changeNodeImage(currentNode);
					}
					if (CommonClass.LstPollutant != null)
					{
						int nodesCount = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.Count;
						if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
						{
							for (int i = nodesCount - 2; i > -1; i--)
							{
								TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
								if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource") { trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.RemoveAt(i); }
							}
							for (int i = nodesCount - 1; i > -1; i--)
							{
								TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
								if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource")
								{
									trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Text = "Source of Air Quality Data";
									trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Tag = null;
									trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Nodes.Clear();
									trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Tag = null;
									trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Nodes.Clear();
								}
							}

							olvCRFunctionResult.SetObjects(null);
							olvIncidence.SetObjects(null);
							tlvAPVResult.SetObjects(null);
							cbPoolingWindowIncidence.Items.Clear();
							cbPoolingWindowAPV.Items.Clear();
							ClearMapTableChart();
							initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

							initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
							initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
							return;
						}

						trvSetting.Nodes["pollutant"].Parent.ExpandAll();
					}
				}
				if (CommonClass.BenMAPPopulation != null)
				{
					changeNodeImage(trvSetting.Nodes[1].Nodes[0]);
				}
				if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
				{
					changeNodeImage(trvSetting.Nodes[2].Nodes[0]);
				}
				WaitClose();
			}
			catch (Exception ex)
			{
				WaitClose();
				Logger.LogError(ex);
			}
		}

		private void ChangeAllAggregationCombox()
		{
			System.Data.DataSet dsCRAggregationGridType = BindGridtype();
			cbCRAggregation.DataSource = dsCRAggregationGridType.Tables[0];
			cbCRAggregation.DisplayMember = "GridDefinitionName";
			cbCRAggregation.SelectedIndex = 0;
			System.Data.DataSet dsIncidenceAggregationGridType = BindGridtype();
			cbIncidenceAggregation.DataSource = dsIncidenceAggregationGridType.Tables[0];
			cbIncidenceAggregation.DisplayMember = "GridDefinitionName";
			cbIncidenceAggregation.SelectedIndex = 0;
			System.Data.DataSet dsAPVAggregationGridType = BindGridtype();
			cbAPVAggregation.DataSource = dsAPVAggregationGridType.Tables[0];
			cbAPVAggregation.DisplayMember = "GridDefinitionName";
			cbAPVAggregation.SelectedIndex = 0;
			System.Data.DataSet dsQALYAggregationGridType = BindGridtype();
			FireBirdHelperBase fbRegion = new ESILFireBirdHelper();
			string commandTextRegion = string.Format("select * from GridDefinitions where columns<=56 and setupid={0}  order by GridDefinitionName desc", CommonClass.MainSetup.SetupID);
			System.Data.DataSet dsRegion = fbRegion.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandTextRegion);
			cboRegion.DataSource = dsRegion.Tables[0];
			cboRegion.DisplayMember = "GridDefinitionName";
			for (int i = 0; i < dsRegion.Tables[0].Rows.Count; i++)
			{
				if (CommonClass.rBenMAPGrid != null && CommonClass.rBenMAPGrid.GridDefinitionID == Convert.ToInt32(dsRegion.Tables[0].Rows[i]["GridDefinitionID"]))
				{
					cboRegion.SelectedIndex = i;
					break;
				}

			}
		}

		public void OpenFile()
		{
			try
			{
				splitContainer1.Visible = true;
				CommonClass.ClearAllObject();
				CommonClass.CRSeeds = 1;
				_MapAlreadyDisplayed = false;
				ClearAll();

				ResetParamsTree("");

				ClearMapTableChart();
				initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
				initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
				initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);

				CommonClass.IncidencePoolingAndAggregationAdvance = null;
				if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
				{
					changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
				}
				olvCRFunctionResult.SetObjects(null);
				olvIncidence.SetObjects(null);
				tlvAPVResult.SetObjects(null);

				cbPoolingWindowIncidence.Items.Clear();
				cbPoolingWindowAPV.Items.Clear();
				ClearMapTableChart();

				SetTabControl(tabCtlReport);
				HealthImpactFunctions.MaxCRID = 0;
				BenMAP_Load(this, null);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}



		public void OpenFile(string filePath)
		{
			try
			{
				splitContainer1.Visible = true;
				_MapAlreadyDisplayed = false;
				ClearAll();
				ResetParamsTree(filePath);
				string chinaOrUSA = System.IO.Path.GetFileNameWithoutExtension(filePath);
				chinaOrUSA = chinaOrUSA.Substring(chinaOrUSA.LastIndexOf('_') + 1, chinaOrUSA.Length - chinaOrUSA.LastIndexOf('_') - 1);
				switch (chinaOrUSA)
				{
					case "China":
						System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5_China.jpg");
						string shapeRoot = string.Format(@"{0}\Data\ChinaData\ShapeFile\", Application.StartupPath);
						string shapeFile = shapeRoot + "China_Region.shp";
						break;
					case "USA":
						backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5.png");
						shapeRoot = string.Format(@"{0}\Data\USAData\ShapeFile\", Application.StartupPath);
						shapeFile = shapeRoot + "US_Region.shp";
						break;
					default:
						backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5_China.jpg");
						backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5.png");
						shapeRoot = string.Format(@"{0}\Data\USAData\ShapeFile\", Application.StartupPath);
						shapeFile = shapeRoot + "US_Region.shp";
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private TreeNode FindNodeByText(TreeNode root, string nodeText)
		{
			if (root == null) return null;
			if (root.Text == nodeText) return root;
			TreeNode node = null;
			foreach (TreeNode tn in root.Nodes)
			{
				node = FindNodeByText(tn, nodeText);
				if (node != null) break;
			}
			return node;
		}

		public void changeBaseControlDelta()
		{
			int nodesCount = 0;
			BaseControlGroup bcg = null;
			foreach (TreeNode trchild in trvSetting.Nodes)
			{
				if (trchild.Name == "airqualitygridgroup")
				{
					nodesCount = trchild.Nodes.Count;


					for (int i = nodesCount - 1; i > -1; i--)
					{
						TreeNode node = trchild.Nodes[i];
						if (trchild.Nodes[i].Name == "datasource")
						{
							if (CommonClass.LstBaseControlGroup == null) continue;
							foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
							{
								if (int.Parse(node.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
							}

							if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0)
							{
								node.Nodes[0].Nodes[0].ImageKey = "doc";
								node.Nodes[0].Nodes[0].SelectedImageKey = "doc";
							}
							else if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count == 0)
							{
								errorNodeImage(node);

							}
							if (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0)
							{
								node.Nodes[1].Nodes[0].ImageKey = "doc";
								node.Nodes[1].Nodes[0].SelectedImageKey = "doc";
							}
							else if (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count == 0)
							{
								errorNodeImage(node);
							}
							if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 && bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0)
							{
								node.Nodes[2].ImageKey = "doc";
								node.Nodes[2].SelectedImageKey = "doc";


							}
							trvSetting.Refresh();
						}
					}
				}
			}

		}

		private bool AddDataSourceNode(BaseControlGroup bcg, TreeNode parentNode)
		{
			string nodeName = string.Empty;
			string pollutantName = string.Empty;
			try
			{
				if (bcg == null) { return false; }
				TreeNode node = new TreeNode();
				int index = 2;
				node = new TreeNode()
				{
					Name = "datasource",
					Tag = bcg.Pollutant.PollutantID,
					Text = string.Format("Source of Air Quality Data ({0})", bcg.Pollutant.PollutantName),
					ToolTipText = "Double-click to load AQ data files",
					ImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
					SelectedImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
					Nodes = { new TreeNode() {
														Name = "baseline",
														Text = "Baseline",
														ToolTipText="Double-click to load AQ data",
														Tag = bcg.Base,
														ImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
														SelectedImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey
														},
														new TreeNode() {
																Name = "control",
																Text = "Control",
																ToolTipText="Double-click to load AQ data",
																Tag = bcg.Control,
														ImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
														SelectedImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey },
														new TreeNode() {
																Name = "delta",
																Text = "Air quality delta (baseline - control)",
																ToolTipText="Double-click AQ data file to display map/data",
																Tag = bcg,
														 ImageKey =(bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 && bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ?"doc": "docgrey",
												SelectedImageKey =(bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 && bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ?"doc": "docgrey" }
								}
				};
				if (bcg.Base != null)
				{
					string s = "";
					try
					{
						if(bcg.Base is ModelDataLine)
						{
							s = (bcg.Base as ModelDataLine).DatabaseFilePath.Substring((bcg.Base as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);
						}
						

					}
					catch
					{ }
					node.Nodes[0].Nodes.Add(new TreeNode()
					{
						Name = "basedata",
						Text = (bcg.Base is ModelDataLine) ? s : "Base Data",
						ToolTipText = "Double-click AQ data file to display map/data",
						Tag = bcg.Base,
						ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
						SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
					});
				}
				if (bcg.Control != null)
				{
					string s = "";
					try
					{
						if (bcg.Base is ModelDataLine)
						{
							s = (bcg.Control as ModelDataLine).DatabaseFilePath.Substring((bcg.Control as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);
						}
							

					}
					catch
					{ }
					node.Nodes[1].Nodes.Add(new TreeNode()
					{
						Name = "controldata",
						Text = (bcg.Control is ModelDataLine) ? s : "Control Data",
						ToolTipText = "Double-click AQ data file to display map/data",
						Tag = bcg.Control,
						ImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
						SelectedImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
					});
				}
				parentNode.Nodes.Insert(index, node);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				return false;
			}
		}

		private int iGridTypeOld = -1;


		private void trvSetting_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			try
			{
				iGridTypeOld = CommonClass.MainSetup.SetupID;
				TreeNode currentNode = trvSetting.SelectedNode; if (currentNode == null) { currentNode = (sender as TreeNode); }
				string nodeName = currentNode.Name.ToLower();
				string nodeTag = string.Empty;
				TreeNode parentNode = currentNode.Parent as TreeNode;
				TreeNode childNode = new TreeNode();
				TreeNode deltaNode = new TreeNode();
				DialogResult rtn = DialogResult.Cancel;
				var frm = new Form();
				BenMAPPollutant p;
				BenMAPLine bml = new BenMAPLine();
				BaseControlGroup bcg = null;
				string currStat = string.Empty;
				string msg = string.Empty;
				string str = string.Empty;
				List<ModelResultAttribute> DeltaQ = null;
				switch (nodeName)
				{
					case "gridtype":
						_currentNode = "gridtype";
						if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
						{
							MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map."), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						int GridTypId = -1;
						if (CommonClass.GBenMAPGrid != null)
							GridTypId = CommonClass.GBenMAPGrid.GridDefinitionID;
						frm = new GridType();
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return; }
						AddChildNodes(ref currentNode, currStat, currStat, bml);
						changeNodeImage(currentNode);
						currentNode.ExpandAll();
						int nodesCountGrid = currentNode.Parent.Nodes.Count;

						if (GridTypId != CommonClass.GBenMAPGrid.GridDefinitionID)
						{
							if (CommonClass.LstBaseControlGroup != null)
							{
								for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
								{
									CommonClass.LstBaseControlGroup[i].Base = null;
									CommonClass.LstBaseControlGroup[i].Control = null;
									GC.Collect();
								}
							}
							for (int i = nodesCountGrid - 1; i > -1; i--)
							{
								TreeNode node = currentNode.Parent.Nodes[i];
								if (currentNode.Parent.Nodes[i].Name == "datasource")
								{

									currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
									initNodeImage(currentNode.Parent.Nodes[i].Nodes[0]);
									currentNode.Parent.Nodes[i].Nodes[1].Nodes.Clear();
									initNodeImage(currentNode.Parent.Nodes[i].Nodes[1]);
									initNodeImage(currentNode.Parent.Nodes[i]);
								}
							}



							if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
							{
								CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
							}
							olvCRFunctionResult.SetObjects(null);
							olvIncidence.SetObjects(null);
							tlvAPVResult.SetObjects(null);

							cbPoolingWindowIncidence.Items.Clear();
							cbPoolingWindowAPV.Items.Clear();
							ClearMapTableChart();
							ClearMapTableChart();
							CommonClass.BenMAPPopulation = null;
							initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[0]);
							if (CommonClass.BaseControlCRSelectFunction != null)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes.Count - 1]);
							}
							if (CommonClass.lstIncidencePoolingAndAggregation != null)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
							}
							if (CommonClass.ValuationMethodPoolingAndAggregation != null)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
							}
						}
						break;
					case "grid":
						_currentNode = "grid";
						mainMap.Layers.Clear();
						mainMap.ProjectionModeReproject = ActionMode.Never;
						mainMap.ProjectionModeDefine = ActionMode.Never;
						tabCtlMain.SelectedIndex = 0;
						PolygonLayer player = null; //mainMap.GetAllLayers()[0] as PolygonLayer; //may not work if there aren't any layers
						if (currentNode.Tag is ShapefileGrid)
						{
							mainMap.Layers.Clear();
							if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as ShapefileGrid).ShapefileName + ".shp"))
							{
								player = (PolygonLayer)mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as ShapefileGrid).ShapefileName + ".shp");

							}
						}
						else if (currentNode.Tag is RegularGrid)
						{
							mainMap.Layers.Clear();
							if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as RegularGrid).ShapefileName + ".shp"))
							{
								player = (PolygonLayer)mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as RegularGrid).ShapefileName + ".shp");
							}
						}
						Color c = Color.Transparent;
						PolygonSymbolizer Transparent = new PolygonSymbolizer(c);

						Transparent.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1); player.Symbolizer = Transparent;
						LayerObject = null;
						break;
					case "region":
						_currentNode = "region";
						_MapAlreadyDisplayed = false;
						mainMap.Layers.Clear();
						tabCtlMain.SelectedIndex = 0;
						addAdminLayers();
						LayerObject = null;
						break;
					case "pollutant":
						_currentNode = "pollutant";
						if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
						{
							MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map."), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						BenMAPPollutant[] benMAPPollutantArray = null;
						if (CommonClass.LstPollutant != null)
						{
							benMAPPollutantArray = CommonClass.LstPollutant.ToArray();
						}
						frm = new Pollutant();
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK)
						{
							if (benMAPPollutantArray != null)
							{
								CommonClass.LstPollutant = new List<BenMAPPollutant>();
								CommonClass.LstPollutant = benMAPPollutantArray.ToList();
							}
							else
								CommonClass.LstPollutant = new List<BenMAPPollutant>();

							return;
						}
						changeNodeImage(currentNode);
						int nodesCount = currentNode.Parent.Nodes.Count;
						if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)  //the branch under this if statement never called as far as I can tell????-MCB
						{
							for (int i = nodesCount - 2; i > -1; i--)
							{
								TreeNode node = currentNode.Parent.Nodes[i];
								if (currentNode.Parent.Nodes[i].Name == "datasource") { currentNode.Parent.Nodes.RemoveAt(i); }
							}
							for (int i = nodesCount - 1; i > -1; i--)
							{
								TreeNode node = currentNode.Parent.Nodes[i];
								if (currentNode.Parent.Nodes[i].Name == "datasource")
								{
									currentNode.Parent.Nodes[i].Text = "Source of Air Quality Data";
									currentNode.Parent.Nodes[i].Nodes[0].Tag = null;
									currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
									currentNode.Parent.Nodes[i].Nodes[1].Tag = null;
									currentNode.Parent.Nodes[i].Nodes[1].Nodes.Clear();
									currentNode.Parent.Nodes[i].Nodes.RemoveAt(2);
								}
							}
							CommonClass.LstBaseControlGroup.Clear();
							CommonClass.BaseControlCRSelectFunction = null;
							CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
							CommonClass.lstIncidencePoolingAndAggregation = null;
							CommonClass.IncidencePoolingResult = null;
							CommonClass.ValuationMethodPoolingAndAggregation = null;
							olvCRFunctionResult.SetObjects(null);
							olvIncidence.SetObjects(null);
							tlvAPVResult.SetObjects(null);

							cbPoolingWindowIncidence.Items.Clear();
							cbPoolingWindowAPV.Items.Clear();
							//ClearMapTableChart();
							ClearMapTableChart();

							//Update tree node symbols
							initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
							foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes) //turn yellow the pooled nodes
							{
								initNodeImage(tn);
							}
							foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes)  //turn yellow the population nodes
							{
								initNodeImage(tn);
							}

							return;
						}
						else
						{

							if (benMAPPollutantArray == null || (benMAPPollutantArray != null && CommonClass.lstPollutantAll.Count != benMAPPollutantArray.Count()) ||
							(benMAPPollutantArray != null && benMAPPollutantArray.ToList().Select(pp => pp.PollutantID).ToList() != CommonClass.lstPollutantAll.Select(ppp => ppp.PollutantID).ToList()))
							{
								currentNode.Tag = CommonClass.LstPollutant;

								List<BaseControlGroup> ExtraListBCG = new List<BaseControlGroup>(CommonClass.LstPollutant.Count + 1);
								List<BenMAPPollutant> MissingLstPollutant = new List<BenMAPPollutant>(CommonClass.LstPollutant.Count);

								//removes the pollunat template node in the pollutant area if it exists
								for (int i = nodesCount - 1; i > -1; i--)
								{
									TreeNode node = currentNode.Parent.Nodes[i];
									if (currentNode.Parent.Nodes[i].Name == "datasource" && currentNode.Parent.Nodes[i].Text == "Source of Air Quality Data")
									{
										currentNode.Parent.Nodes.RemoveAt(i);
									}
								}

								//check for extra bcgs and remove if found
								if (CommonClass.LstBaseControlGroup != null)  //Pollutants have been added earlier already, add these to the existing pollutants
								{
									foreach (BaseControlGroup testbcg in CommonClass.LstBaseControlGroup)  //look for matching pollutant in pollutant list
									{
										bool PopulatedPollutantsAlreadyExist = false;
										foreach (BenMAPPollutant BMpol in CommonClass.LstPollutant)
										{
											if (testbcg.Pollutant != null)
											{
												if (BMpol.PollutantID == testbcg.Pollutant.PollutantID)
												{
													PopulatedPollutantsAlreadyExist = true;
													break;
												}
											}
										}

										if (!PopulatedPollutantsAlreadyExist)  //can't find it in pollutant list, so it must be an extra bcg record
										{
											ExtraListBCG.Add(testbcg);
										}

									}

									if (ExtraListBCG.Count > 0)                 //remove extra bcg records
									{
										foreach (BaseControlGroup extrabcg in ExtraListBCG)
										{
											//remove this pollutant node
											//refresh node count in case a node was removed above
											nodesCount = currentNode.Parent.Nodes.Count;
											for (int i = nodesCount - 1; i > -1; i--)
											{
												TreeNode node = currentNode.Parent.Nodes[i];
												if (currentNode.Parent.Nodes[i].Name == "datasource" && (int)currentNode.Parent.Nodes[i].Tag == extrabcg.Pollutant.PollutantID)
												{
													currentNode.Parent.Nodes.RemoveAt(i);
												}
											}
											//remove this pollutant's bcg record too
											CommonClass.LstBaseControlGroup.Remove(extrabcg);
										}
									}

									//Find any missing bcgs and add them
									foreach (BenMAPPollutant BMpol in CommonClass.LstPollutant)  //look for matching pollutant in bcg records
									{
										bool PopulatedPollutantsAlreadyExist = false;
										foreach (BaseControlGroup testBCG in CommonClass.LstBaseControlGroup)
										{

											if (testBCG.Pollutant.PollutantID == BMpol.PollutantID)
											{
												PopulatedPollutantsAlreadyExist = true;
												break;
											}
										}
										if (!PopulatedPollutantsAlreadyExist)  //can't find match so need to add a bcg record for this pollutant
										{
											MissingLstPollutant.Add(BMpol);
										}

									}
									if (MissingLstPollutant.Count > 0)                 //Add missing bcg records
									{
										foreach (BenMAPPollutant missingPol in MissingLstPollutant)
										{
											p = missingPol;
											bcg = new BaseControlGroup() { GridType = CommonClass.GBenMAPGrid, Pollutant = p };
											CommonClass.LstBaseControlGroup.Add(bcg);
											AddDataSourceNode(bcg, currentNode.Parent);
											//turn the new pollutant header node yellow.
											initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
										}
									}


								}
								else
								{
									CommonClass.LstBaseControlGroup = null;
									GC.Collect();

									//rebuilds the polluntant section of the tree
									CommonClass.LstBaseControlGroup = new List<BaseControlGroup>(CommonClass.LstPollutant.Count);
									for (int i = CommonClass.LstPollutant.Count - 1; i > -1; i--)
									{


										p = CommonClass.LstPollutant[i];
										bcg = new BaseControlGroup() { GridType = CommonClass.GBenMAPGrid, Pollutant = p };
										CommonClass.LstBaseControlGroup.Add(bcg);
										AddDataSourceNode(bcg, currentNode.Parent);

										//turn the new pollutant header node yellow.
										initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
									}
								}
								//turn all nodes after BCDG to yellow (AKA unready)
								foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes) //turn yellow the pooled nodes
								{
									initNodeImage(tn);
								}
								foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes)  //turn yellow the population nodes
								{
									initNodeImage(tn);
								}
								//Assumes everything else is unready
								CommonClass.BaseControlCRSelectFunction = null;
								CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
								CommonClass.lstIncidencePoolingAndAggregation = null;
								CommonClass.IncidencePoolingResult = null;
								CommonClass.ValuationMethodPoolingAndAggregation = null;
								//ClearMapTableChart();

								CommonClass.BenMAPPopulation = null;
								CommonClass.IncidencePoolingAndAggregationAdvance = null;

								olvCRFunctionResult.SetObjects(null);
								olvIncidence.SetObjects(null);
								tlvAPVResult.SetObjects(null);

								cbPoolingWindowIncidence.Items.Clear();
								cbPoolingWindowAPV.Items.Clear();
								ClearMapTableChart();
							}
						}
						currentNode.Parent.ExpandAll();

						break;
					case "datasource":
						_currentNode = "datasource";
						TreeNode pNode = currentNode.Parent;
						BaseControlGroup bcgLoadAQG = new BaseControlGroup();
						if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
						{
							CommonClass.LstBaseControlGroup = new List<BaseControlGroup>();
							CommonClass.LstBaseControlGroup.Add(new BaseControlGroup());
							bcgLoadAQG = CommonClass.LstBaseControlGroup[0];
							currentNode.Tag = null;

						}
						string currentNodeTag = currentNode.Tag != null ? currentNode.Tag.ToString() : "";
						if (currentNodeTag != "")
						{
							foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
							{
								if (int.Parse(currentNodeTag) == b.Pollutant.PollutantID) { bcgLoadAQG = b; break; }
							}
						}
						if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0 && bcgLoadAQG.Pollutant != null)
						{
							lock (CommonClass.LstAsynchronizationStates)
							{
								if (CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcgLoadAQG.Pollutant.PollutantName.ToLower(), "baseline"))
										|| CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcgLoadAQG.Pollutant.PollutantName.ToLower(), "control")))
								{
									msg = " BenMAP is still creating the air quality surface map.";
									MessageBox.Show(msg);
									return;
								}
							}
						}
						OpenExistingAQG openAQG = new OpenExistingAQG(bcgLoadAQG);
						openAQG.ShowDialog();
						if (openAQG.DialogResult != DialogResult.OK) { return; }
						bcgLoadAQG.DeltaQ = null;
						if (openAQG.isGridTypeChanged)
						{
							if (CommonClass.LstBaseControlGroup != null)
							{
								for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
								{
									if (currentNodeTag != null && currentNodeTag.ToString() != "" && CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID != int.Parse(currentNodeTag))
									{
										CommonClass.LstBaseControlGroup[i].Base = null;
										CommonClass.LstBaseControlGroup[i].Control = null;
										GC.Collect();
									}
								}
							}

							for (int i = currentNode.Parent.Nodes.Count - 1; i > -1; i--)
							{
								TreeNode node = currentNode.Parent.Nodes[i];
								if (currentNode.Parent.Nodes[i].Name == "datasource")
								{
									if (currentNodeTag != null && currentNodeTag.ToString() != "" && int.Parse(currentNodeTag) != int.Parse(currentNode.Parent.Nodes[i].Tag.ToString()))
									{
										currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
										initNodeImage(currentNode.Parent.Nodes[i].Nodes[0]);
										currentNode.Parent.Nodes[i].Nodes[1].Nodes.Clear();
										initNodeImage(currentNode.Parent.Nodes[i].Nodes[1]);
										initNodeImage(currentNode.Parent.Nodes[i]);
									}
								}
							}


							if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
							{
								CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
							}

							olvCRFunctionResult.SetObjects(null);
							olvIncidence.SetObjects(null);
							tlvAPVResult.SetObjects(null);

							cbPoolingWindowIncidence.Items.Clear();
							cbPoolingWindowAPV.Items.Clear();
							ClearMapTableChart();
							CommonClass.BenMAPPopulation = null;
							initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[0]);
							if (CommonClass.BaseControlCRSelectFunction != null)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes.Count - 1]);
							}
							if (CommonClass.lstIncidencePoolingAndAggregation != null)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
							}
							if (CommonClass.ValuationMethodPoolingAndAggregation != null)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
							}
						}
						bcgLoadAQG = openAQG.bcgOpenAQG;
						int index = currentNode.Index;
						BrushBaseControl(ref pNode, bcgLoadAQG, index);

						break;
					case "baseline":
						_currentNode = "baseline";
						currStat = "baseline";
						bool BCResultOK = BaseControlOP(currStat, ref currentNode);
						break;
					case "basedata":
						DrawBaseline(currentNode, str); //-MCB
						break;
					case "delta":
						DrawDelta(currentNode, str);
						break;
					case "control":
						_currentNode = "control";
						currStat = "control";
						bool BCResultOK2 = BaseControlOP(currStat, ref currentNode);
						break;
					case "controldata":
						DrawControlData(currentNode, str); //-MCB
						break;
					case "configuration":
						_currentNode = "gridtype";
						frm = new OpenExistingConfiguration();
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return; }
						string CRFilePath = (frm as OpenExistingConfiguration).strCRPath;
						if (CRFilePath.Substring(CRFilePath.Length - 5, 5) == "cfgrx")
						{
							WaitShow("Loading configuration results file");
							this.mainFrm.Enabled = false;
							try
							{
								CommonClass.ClearAllObject();
								string err = "";
								CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile((frm as OpenExistingConfiguration).strCRPath, ref err);
								if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
								{
									System.Threading.Thread.Sleep(300);
									WaitClose();
									MessageBox.Show(err);
									return;
								}

								if (CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue == null)
								{
									System.Threading.Thread.Sleep(300); WaitClose();
									MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");
									return;
								}
								CommonClass.BaseControlCRSelectFunction = null;
								CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
								CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
								CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
								CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
								CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
								CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
								CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
								CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
								CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
								CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
								CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

								for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
								{
									CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
								}
								try
								{
									if (CommonClass.BaseControlCRSelectFunction != null)
									{
										showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);
										tabCtlReport.SelectedIndex = 0;
									}
								}
								catch (Exception ex)
								{
									MessageBox.Show(ex.Message);
									Logger.LogError(ex);
								}

								olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
								CommonClass.ValuationMethodPoolingAndAggregation = null;
								CommonClass.lstIncidencePoolingAndAggregation = null;
								CommonClass.IncidencePoolingResult = null;
								CommonClass.GBenMAPGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType;
								foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
								{
									initNodeImage(tn);
								}

								ConfigurationResultsReport frmColumn = new ConfigurationResultsReport();
								frmColumn.userAssignPercentile = false;
								strHealthImpactPercentiles = null;

								olvIncidence.SetObjects(null);
								tlvAPVResult.SetObjects(null);
								cbPoolingWindowIncidence.Items.Clear();
								cbPoolingWindowAPV.Items.Clear();
								ClearMapTableChart();
							}
							catch (Exception ex)
							{
								Logger.LogError(ex);
							}
							WaitClose();
							this.mainFrm.Enabled = true;
						}
						else
						{
							try
							{
								if (CommonClass.BaseControlCRSelectFunction != null)
								{
									CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

									showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);
									olvIncidence.SetObjects(null);
									tlvAPVResult.SetObjects(null);
									cbPoolingWindowIncidence.Items.Clear();
									cbPoolingWindowAPV.Items.Clear();
									ClearMapTableChart();
									CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
									CommonClass.lstIncidencePoolingAndAggregation = null;
									CommonClass.IncidencePoolingResult = null;
									CommonClass.ValuationMethodPoolingAndAggregation = null;

									GC.Collect();

									olvCRFunctionResult.SetObjects(null);
									foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
									{
										initNodeImage(tn);
									}
								}
							}
							catch (Exception ex)
							{
							}
						}
						SetTabControl(tabCtlReport);
						break;
					case "latinhypercubepoints":
						_currentNode = "latinhypercubepoints";
						frm = new LatinHypercubePoints();
						(frm as LatinHypercubePoints).LatinHypercubePointsCount = CommonClass.CRLatinHypercubePoints;
						(frm as LatinHypercubePoints).IsRunInPointMode = CommonClass.CRRunInPointMode;
						(frm as LatinHypercubePoints).Threshold = CommonClass.CRThreshold;
						(frm as LatinHypercubePoints).DefaultMonteCarloIterations = CommonClass.CRDefaultMonteCarloIterations;
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return; }
						CommonClass.CRLatinHypercubePoints = (frm as LatinHypercubePoints).LatinHypercubePointsCount;
						CommonClass.CRRunInPointMode = (frm as LatinHypercubePoints).IsRunInPointMode;
						CommonClass.CRThreshold = (frm as LatinHypercubePoints).Threshold;
						CommonClass.CRDefaultMonteCarloIterations = (frm as LatinHypercubePoints).DefaultMonteCarloIterations;
						changeNodeImage(currentNode);
						break;
					case "populationdataset":
						_currentNode = "populationdataset";
						frm = new PopulationDataset();
						(frm as PopulationDataset).BenMAPPopulation = CommonClass.BenMAPPopulation;
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return; }
						if (CommonClass.BenMAPPopulation == null || CommonClass.BenMAPPopulation.DataSetID != (frm as PopulationDataset).BenMAPPopulation.DataSetID ||
								CommonClass.BenMAPPopulation.Year != (frm as PopulationDataset).BenMAPPopulation.Year)
						{
							Configuration.ConfigurationCommonClass.DicGrowth = null;
							Configuration.ConfigurationCommonClass.DicWeight = null;
						}
						if (CommonClass.BenMAPPopulation != null && (CommonClass.BenMAPPopulation.DataSetID != (frm as PopulationDataset).BenMAPPopulation.DataSetID ||
								CommonClass.BenMAPPopulation.Year != (frm as PopulationDataset).BenMAPPopulation.Year))
						{
							if (currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 1].ImageKey != _errorImageKey)
								initNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 1]);
						}
						CommonClass.BenMAPPopulation = (frm as PopulationDataset).BenMAPPopulation;
						changeNodeImage(currentNode);

						break;
					case "healthimpactfunctions":
						_currentNode = "healthimpactfunctions";
						if (CommonClass.GBenMAPGrid == null)
						{
							MessageBox.Show("Please select an air quality surface.");
							return;
						}
						if (CommonClass.LstPollutant == null)
						{
							MessageBox.Show("Please select a pollutant.");
							return;
						}
						if (CommonClass.LstBaseControlGroup == null)
						{
							MessageBox.Show("Please define baseline and control air quality surfaces.");
							return;
						}
						foreach (BaseControlGroup bcGroup in CommonClass.LstBaseControlGroup)
						{
							if (bcGroup.Base == null || bcGroup.Control == null || bcGroup.Base.ModelResultAttributes == null ||
										bcGroup.Control.ModelResultAttributes == null)
							{
								MessageBox.Show("Please define baseline and control air quality surfaces.");
								return;
							}
						}
						if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
						{
							MessageBox.Show("Baseline or control air quality surface is being created. Please wait.");
							return;
						}
						if (CommonClass.BenMAPPopulation == null)
						{
							MessageBox.Show("Please select population dataset and year.");
							return;
						}
						frm = new HealthImpactFunctions();
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK)
						{
							return;
						}
						ConfigurationResultsReport frmReport = new ConfigurationResultsReport();
						frmReport.userAssignPercentile = false;
						strHealthImpactPercentiles = null;

						olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
						changeNodeImage(currentNode);
						if (CommonClass.ValuationMethodPoolingAndAggregation != null)
						{
							CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
							CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath = "";
							foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
							{
								foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.ToList())
								{
									if (alsc.PoolingMethod == "")
									{
										try
										{
											alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(a => a.CRSelectFunction.CRID == alsc.CRID).First();
										}
										catch
										{
											alsc.CRSelectFunctionCalculateValue = null;
										}
									}
									else
									{
										alsc.CRSelectFunctionCalculateValue = null;
									}

								}
							}


							if (trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 2].ImageKey == _readyImageKey)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 2]);
							}
							if (trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1].ImageKey == _readyImageKey)
							{
								errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
							}

						}
						olvIncidence.SetObjects(null);
						tlvAPVResult.SetObjects(null);

						cbPoolingWindowIncidence.Items.Clear();
						cbPoolingWindowAPV.Items.Clear();
						ClearMapTableChart();
						changeNodeImage(currentNode);
						SetTabControl(tabCtlReport);
						break;
					case "aggregationpoolingvaluation":
						_currentNode = "aggregationpoolingvaluation";
						frm = new OpenExistingAPVConfiguration();
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return; }
						if ((frm as OpenExistingAPVConfiguration).strCRPath != "")
						{
							WaitShow("Loading configuration results file ");
							try
							{
								string err = "";
								CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile((frm as OpenExistingAPVConfiguration).strCRPath, ref err);
								if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
								{
									MessageBox.Show(err);
									return;
								}
								CommonClass.BaseControlCRSelectFunction = null;
								CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
								CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
								CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
								CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
								CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
								CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
								CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
								CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
								CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
								CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
								CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

								for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
								{
									CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
								}
								try
								{
									if (CommonClass.BaseControlCRSelectFunction != null)
									{
										showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);
									}
								}
								catch (Exception ex)
								{
									MessageBox.Show(ex.Message);
									Logger.LogError(ex);
								}

								olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
								CommonClass.ValuationMethodPoolingAndAggregation = null;
								CommonClass.lstIncidencePoolingAndAggregation = null;
								CommonClass.IncidencePoolingResult = null;
								foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
								{
									initNodeImage(tn);
								}
								frmReport = new ConfigurationResultsReport();
								frmReport.userAssignPercentile = false;
								strHealthImpactPercentiles = null;
								strPoolIncidencePercentiles = null;

								olvIncidence.SetObjects(null);
								tlvAPVResult.SetObjects(null);

								cbPoolingWindowIncidence.Items.Clear();
								cbPoolingWindowAPV.Items.Clear();
								ClearMapTableChart();
							}
							catch (Exception ex)
							{
								Logger.LogError(ex);
							}
							WaitClose();
						}
						else if ((frm as OpenExistingAPVConfiguration).strAPVPath != "")
						{
							if (((frm as OpenExistingAPVConfiguration).strAPVPath.Substring(((frm as OpenExistingAPVConfiguration).strAPVPath.Length - 5), 5)) == "apvrx")
							{
								WaitShow("Loading APV results file ");
							}
							else
							{
								WaitShow("Loading APV configuration file ");
							}
							CommonClass.ValuationMethodPoolingAndAggregation = null;
							CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
							CommonClass.lstIncidencePoolingAndAggregation = null;
							CommonClass.LstBaseControlGroup = null;
							CommonClass.BaseControlCRSelectFunction = null;
							GC.Collect();
							CommonClass.ClearAllObject();
							string err = "";
							CommonClass.ValuationMethodPoolingAndAggregation = APVX.APVCommonClass.loadAPVRFile((frm as OpenExistingAPVConfiguration).strAPVPath, ref err);
							if (CommonClass.ValuationMethodPoolingAndAggregation == null)
							{
								WaitClose();
								MessageBox.Show(err);
								return;
							}
							foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
							{
								for (int iVB = 0; iVB < vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count; iVB++)
								{
									if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iVB].EndPointGroup == null)
									{
										vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iVB].EndPointGroup = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[0].EndPointGroup;
										vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iVB].EndPointGroupID = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[0].EndPointGroupID;

									}
								}
								if (vb.LstAllSelectValuationMethod != null)
								{
									for (int iVB = 0; iVB < vb.LstAllSelectValuationMethod.Count; iVB++)
									{
										if (vb.LstAllSelectValuationMethod[iVB].EndPointGroup == null)
										{
											vb.LstAllSelectValuationMethod[iVB].EndPointGroup = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[0].EndPointGroup;

										}
									}
								}
							}
							CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
							CommonClass.BaseControlCRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
							CommonClass.BaseControlCRSelectFunction = null;
							CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
							CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
							CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
							CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
							CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
							CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
							CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
							CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
							CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
							CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
							CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

							for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
							{
								CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
							}
							try
							{
								if (CommonClass.BaseControlCRSelectFunction != null)
								{
									showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);

								}
							}
							catch (Exception ex)
							{
							}
							if (((frm as OpenExistingAPVConfiguration).strAPVPath.Substring(((frm as OpenExistingAPVConfiguration).strAPVPath.Length - 5), 5)) == "apvrx")
							{
								olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);

							}
							else
							{
								olvCRFunctionResult.SetObjects(null);
							}

							//CommonClass.lstIncidencePoolingAndAggregation = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList();
							CommonClass.lstIncidencePoolingAndAggregation = CommonClassExtension.DeepClone(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList()); //YY:
							CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
							cbPoolingWindowAPV.Items.Clear();

							olvIncidence.SetObjects(null);
							tlvAPVResult.SetObjects(null);

							cbPoolingWindowIncidence.Items.Clear();
							cbPoolingWindowAPV.Items.Clear();
							ClearMapTableChart();
							if ((frm as OpenExistingAPVConfiguration).strAPVPath.Substring((frm as OpenExistingAPVConfiguration).strAPVPath.Count() - 5, 5).ToLower() == "apvrx")
							{
								if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
&& CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID && (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0))
								{
									CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
									CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>(); //YY: new added
									foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
									{
										//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
										//YY: calculate incidence aggregation
										CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID));
										//YY: calculate valuation aggregation in a seperate object
										CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
									}
								}
								//YY: valuation
								foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
								{
									bool bHavePooling = false;
									foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.PoolingMethod == "").ToList())
									{
										if (alsc.PoolingMethod == "" && alsc.NodeType == 100)
										{
											try
											{
												if (bHavePooling == false && alsc.CRSelectFunctionCalculateValue != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues.Count > 0)
												{
													bHavePooling = true;
												}
												if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Count == 0)
												{
													if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
														  && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID
															&& CommonClass.lstCRResultAggregation != null
															&& CommonClass.lstCRResultAggregation.Count != 0)
													{
														alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
													}
													else
													{
														alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
													}

												}
												else
												{
													alsc.CRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
												}
											}
											catch
											{
											}
										}
										else
										{
											alsc.CRSelectFunctionCalculateValue = null;
										}
									}

									if (bHavePooling == false)
									{
										List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
										if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None" || (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "" && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().NodeType != 100))
										{
											APVX.APVCommonClass.getAllChildCRNotNoneForPooling(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

										}
										lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
										if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0) { }
										else
										{
											APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
										}
									}
								}

								//YY: pooling -  new added 
								foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
								{
									bool bHavePooling = false;
									foreach (AllSelectCRFunction ascr in ip.lstAllSelectCRFuntion.Where(pa => pa.PoolingMethod == "").ToList())
									{
										if (ascr.PoolingMethod == "" && ascr.NodeType == 100)
										{
											try
											{
												if (bHavePooling == false && ascr.CRSelectFunctionCalculateValue != null && ascr.CRSelectFunctionCalculateValue.CRCalculateValues != null && ascr.CRSelectFunctionCalculateValue.CRCalculateValues.Count > 0)
												{
													bHavePooling = true;
												}
												if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
												{
													ascr.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(pa => pa.CRSelectFunction.CRID == ascr.CRID).First();
												}
												else
												{
													ascr.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(pa => pa.CRSelectFunction.CRID == ascr.CRID).First();
												}
											}
											catch
											{

											}
										}
										else
										{
											ascr.CRSelectFunctionCalculateValue = null;
										}
									}

									if (bHavePooling == false)
									{
										List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
										if (ip.lstAllSelectCRFuntion.First().PoolingMethod == "None" || (ip.lstAllSelectCRFuntion.First().PoolingMethod == "" && ip.lstAllSelectCRFuntion.First().NodeType != 100))
										{
											APVX.APVCommonClass.getAllChildCRNotNoneForPooling(ip.lstAllSelectCRFuntion.First(), ip.lstAllSelectCRFuntion, ref lstCR);

										}
										lstCR.Insert(0, ip.lstAllSelectCRFuntion.First());
										if (lstCR.Count == 1 && ip.lstAllSelectCRFuntion.First().CRID < 9999 && ip.lstAllSelectCRFuntion.First().CRID > 0) { }
										else
										{
											APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref ip.lstAllSelectCRFuntion, ref ip.lstAllSelectCRFuntion, ip.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), ip.lstColumns);
										}
									}
								}

								foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
								{
									cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
								}
								cbPoolingWindowAPV.SelectedIndex = 0;
								cbPoolingWindowIncidence.Items.Clear();
								foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
								{
									this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
								}
								cbPoolingWindowIncidence.SelectedIndex = 0;


								foreach (TreeNode trnd in currentNode.Nodes)
								{
									changeNodeImage(trnd);
								}
								if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance == null)
								{
									initNodeImage(currentNode.Nodes[0]);
								}

							}
							else
							{
								foreach (TreeNode trnd in currentNode.Nodes)
								{
									changeNodeImage(trnd);
								}
								if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance == null)
								{
									initNodeImage(currentNode.Nodes[0]);
								}
								initNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
								errorNodeImage(currentNode.Nodes[1]);
								errorNodeImage(currentNode.Nodes[2]);
							}





							SetTabControl(tabCtlReport);
							WaitClose();
						}
						this.trvSetting.ExpandAll();
						break;
					case "aggregation":
						_currentNode = "aggregation";
						frm = new APVX.Aggregation();
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return; }
						DataRowView drv = null;
						if (CommonClass.IncidencePoolingAndAggregationAdvance == null)
							CommonClass.IncidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance();
						if ((frm as APVX.Aggregation).cboIncidenceAggregation.SelectedIndex != -1)
						{
							drv = (frm as APVX.Aggregation).cboIncidenceAggregation.SelectedItem as DataRowView;
							CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
						}
						if ((frm as APVX.Aggregation).cboValuationAggregation.SelectedIndex != -1)
						{
							drv = (frm as APVX.Aggregation).cboValuationAggregation.SelectedItem as DataRowView;
							if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != Convert.ToInt32(drv["GridDefinitionID"]))
							{
								CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
								CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>(); //YY:
							}
							CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
						}
						if ((frm as APVX.Aggregation).cboQALYAggregation.SelectedIndex != -1)
						{
							drv = (frm as APVX.Aggregation).cboQALYAggregation.SelectedItem as DataRowView;
							if (CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != Convert.ToInt32(drv["GridDefinitionID"]))
							{
								CommonClass.lstCRResultAggregationQALY = new List<CRSelectFunctionCalculateValue>();
							}
							CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
						}
						changeNodeImage(currentNode);
						break;
					case "poolingmethod":
						_currentNode = "poolingmethod";
						if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
						{
							frm = new IncidencePoolingandAggregation();
							rtn = frm.ShowDialog();
							if (rtn != DialogResult.OK) { return; }
							cbPoolingWindowAPV.Items.Clear();
							foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
							{
								cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
							}
							cbPoolingWindowAPV.SelectedIndex = 0;
							cbPoolingWindowIncidence.Items.Clear();
							foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
							{
								this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
							}
							cbPoolingWindowIncidence.SelectedIndex = 0;
							olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
							changeNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 2]);
							changeNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
							changeNodeImage(currentNode);
							changeNodeImage(currentNode);
							changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
						}
						SetTabControl(tabCtlReport);
						break;
					case "valuationmethod":
						_currentNode = "valuationmethod";
						if (CommonClass.ValuationMethodPoolingAndAggregation == null)
							return;
						if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().LstAllSelectValuationMethod == null)
							return;
						if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
						{
						}
						else
							return;
						frm = new SelectValuationMethods();
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return; }

						frmReport = new ConfigurationResultsReport();
						frmReport.userAssignPercentile = false;
						strHealthImpactPercentiles = null;
						strPoolIncidencePercentiles = null;

						cbPoolingWindowAPV.Items.Clear();
						foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
						{
							cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
						}
						cbPoolingWindowAPV.SelectedIndex = 0;
						cbPoolingWindowIncidence.Items.Clear();
						foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
						{
							this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
						}
						cbPoolingWindowIncidence.SelectedIndex = 0;


						olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
						changeNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 2]);
						changeNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
						changeNodeImage(currentNode);
						SetTabControl(tabCtlReport);
						break;
				}
				if (iGridTypeOld != CommonClass.MainSetup.SetupID)
				{
					ChangeAllAggregationCombox();
				}
				if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
				{
					changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
					FireBirdHelperBase fb = new ESILFireBirdHelper();
					string commandText = "";
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void DrawBaseline(TreeNode currentNode, string str)
		{   //Draws base data on main map
				//pause legend event handling
			_RaiseLayerChangeEvents = false;

			_currentNode = "basedata";
			str = string.Format("{0}baseline", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName);
			string _PollutantName = (currentNode.Tag as BenMAPLine).Pollutant.PollutantName;
			string _BenMapSetupName = CommonClass.MainSetup.SetupName;
			_CurrentMapTitle = _BenMapSetupName + " Setup: " + _PollutantName + ", Baseline";

			if (CommonClass.LstAsynchronizationStates != null &&
					CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
			{
				MessageBox.Show(string.Format("BenMAP is still creating the Baseline air quality surface map.", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			WaitShow("Drawing layer...");
			try
			{
				tabCtlMain.SelectedIndex = 0;
				//set change projection text
				string changeProjText = "change projection to setup projection";
				if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
				{
					changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
				}
				tsbChangeProjection.Text = changeProjText;

				BenMAPLine b = currentNode.Tag as BenMAPLine;
				foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
				{
					if (bc.Pollutant.PollutantID == b.Pollutant.PollutantID)
					{ b = bc.Base; }
				}
				currentNode.Tag = b;
				addBenMAPLineToMainMap(b, "B");
				MoveAdminGroupToTop();
				LayerObject = currentNode.Tag as BenMAPLine;
				InitTableResult(currentNode.Tag as BenMAPLine);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				Debug.WriteLine("DraawBaseline: " + ex.ToString());
				_RaiseLayerChangeEvents = true;
			}
			WaitClose();
			_RaiseLayerChangeEvents = true;
		}

		private void DrawControlData(TreeNode currentNode, string str)
		{
			_RaiseLayerChangeEvents = false;
			_currentNode = "controldata";

			//Map Title
			string _PollutantName = (currentNode.Tag as BenMAPLine).Pollutant.PollutantName;
			string _BenMapSetupName = (currentNode.Tag as BenMAPLine).GridType.SetupName;
			_CurrentMapTitle = _BenMapSetupName + " Setup: " + _PollutantName + ", Control";

			str = string.Format("{0}control", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName);
			if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
			{
				MessageBox.Show(string.Format("BenMAP is still creating the Control air quality surface map. ", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			WaitShow("Drawing layer...");
			try
			{
				tabCtlMain.SelectedIndex = 0;
				BenMAPLine cc = currentNode.Tag as BenMAPLine;
				foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
				{
					if (bc.Pollutant.PollutantID == cc.Pollutant.PollutantID)
					{ cc = bc.Control; }
				}
				currentNode.Tag = cc;

				addBenMAPLineToMainMap(cc, "C");
				MoveAdminGroupToTop();
				LayerObject = currentNode.Tag as BenMAPLine;
				InitTableResult(currentNode.Tag as BenMAPLine);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				Debug.WriteLine("DrawControlData: " + ex.ToString());
				_RaiseLayerChangeEvents = true;
			}
			WaitClose();
			_RaiseLayerChangeEvents = true;
		}

		private void DrawDelta(TreeNode currentNode, string str)
		{
			_RaiseLayerChangeEvents = false;

			_currentNode = "delta";
			BaseControlGroup bcgDelta = currentNode.Tag as BaseControlGroup;
			if (bcgDelta == null)
			{
				MessageBox.Show("There is no result for delta.");
				return;
			}
			if (bcgDelta.Base == null || bcgDelta.Control == null)
			{
				MessageBox.Show("There is no result for delta.");
				return;
			}
			if (bcgDelta.Base.ModelResultAttributes == null || bcgDelta.Control.ModelResultAttributes == null)
			{
				MessageBox.Show("There is no result for delta.");
				return;
			}
			str = string.Format("{0}baseline", bcgDelta.Pollutant.PollutantName);
			if (CommonClass.LstAsynchronizationStates != null &&
					CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
			{
				MessageBox.Show(string.Format("BenMAP is still creating the Baseline air quality surface map. ", bcgDelta.Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			str = string.Format("{0}control", bcgDelta.Pollutant.PollutantName);
			if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
			{
				MessageBox.Show(string.Format("BenMAP is still creating the Control air quality surface map. ", bcgDelta.Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			WaitShow("Drawing layer...");

			//set change projection text
			string changeProjText = "change projection to setup projection";
			if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
			{
				changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
			}
			tsbChangeProjection.Text = changeProjText;

			if (bcgDelta.DeltaQ == null)
			{
				bcgDelta.DeltaQ = new BenMAPLine();
				bcgDelta.DeltaQ.Pollutant = bcgDelta.Base.Pollutant;

				bcgDelta.DeltaQ.GridType = bcgDelta.Base.GridType;
				bcgDelta.DeltaQ.ModelResultAttributes = new List<ModelResultAttribute>();
				float deltaresult;
				Dictionary<string, Dictionary<string, float>> dicControl = new Dictionary<string, Dictionary<string, float>>();
				foreach (ModelResultAttribute mra in bcgDelta.Control.ModelResultAttributes)
				{
					if (!dicControl.ContainsKey(mra.Col + "," + mra.Row))
					{
						dicControl.Add(mra.Col + "," + mra.Row, mra.Values);
					}
				}
				foreach (ModelResultAttribute mra in bcgDelta.Base.ModelResultAttributes)
				{
					try
					{
						if (dicControl.ContainsKey(mra.Col + "," + mra.Row))
						{
							bcgDelta.DeltaQ.ModelResultAttributes.Add(new ModelResultAttribute()
							{
								Col = mra.Col,
								Row = mra.Row
							});
							bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values = new Dictionary<string, float>();
							foreach (KeyValuePair<string, float> k in mra.Values)        //Populates the Delta modelresultattributes by subtracting the control values from the base line values
							{
								if (dicControl[mra.Col + "," + mra.Row].ContainsKey(k.Key))
								{
									deltaresult = k.Value - (dicControl[mra.Col + "," + mra.Row][k.Key]);
									//if (deltaresult < 0) deltaresult = (float)0.0; // This line doesn't allow GIS Map to show negative delta. Comment out to allow in Jun 2018.
									bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, deltaresult);
								}
								else
									bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, Convert.ToSingle(0.0));
							}
						}

					}
					catch (Exception ex)
					{
						Logger.LogError(ex);
						Debug.WriteLine("DrawDelta: " + ex.ToString());
					}
				}
				_RaiseLayerChangeEvents = true;
			}

			try
			{
				//Map Title
				string PollutantName = bcgDelta.DeltaQ.Pollutant.PollutantName;
				string BenMapSetupName = bcgDelta.Base.GridType.SetupName;
				_CurrentMapTitle = BenMapSetupName + " Setup: " + PollutantName + ", Delta";

				tabCtlMain.SelectedIndex = 0;
				addBenMAPLineToMainMap(bcgDelta.DeltaQ, "D");
				MoveAdminGroupToTop();
				LayerObject = bcgDelta.DeltaQ;
				InitTableResult(bcgDelta.DeltaQ);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				Debug.WriteLine("DrawDelta (2): " + ex.ToString());
			}
			WaitClose();
			return;
		}

		private void CRResultChangeVPV()
		{
			try
			{
				if (CommonClass.lstIncidencePoolingAndAggregation == null) return;
				List<string> lstCRID = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Select(p => p.CRSelectFunction.CRID + "," + p.CRSelectFunction.BenMAPHealthImpactFunction.ID).ToList();
				foreach (IncidencePoolingAndAggregation ipa in CommonClass.lstIncidencePoolingAndAggregation)
				{
					List<AllSelectCRFunction> lstRemove = new List<AllSelectCRFunction>();
					foreach (AllSelectCRFunction asc in ipa.lstAllSelectCRFuntion)
					{
						if (asc.NodeType != 100) continue;
						if (!lstCRID.Contains(asc.CRSelectFunctionCalculateValue.CRSelectFunction.CRID + "," + asc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.ID))
						{
							lstRemove.Add(asc);
						}
					}
					foreach (AllSelectCRFunction ascremove in lstRemove)
					{

						ipa.lstAllSelectCRFuntion.Remove(ascremove);
					}

					List<AllSelectCRFunction> lstRemoveSec = new List<AllSelectCRFunction>();
					foreach (AllSelectCRFunction allSelectCRFunction in ipa.lstAllSelectCRFuntion)
					{
						List<AllSelectCRFunction> lstTmp = new List<AllSelectCRFunction>();
						APVX.APVCommonClass.getAllChildCR(allSelectCRFunction, ipa.lstAllSelectCRFuntion, ref lstTmp);
						if (lstTmp.Where(p => p.NodeType == 100).Count() == 0)
							lstRemoveSec.Add(allSelectCRFunction);
					}
					lstRemoveSec = lstRemoveSec.Where(p => p.NodeType != 100).ToList();
					foreach (AllSelectCRFunction allSelectCRFunction in lstRemoveSec)
					{
						ipa.lstAllSelectCRFuntion.Remove(allSelectCRFunction);
					}
					foreach (AllSelectCRFunction allSelectCRFunction in ipa.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == ""))
					{
						allSelectCRFunction.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == allSelectCRFunction.CRID).First();
					}
				}
				List<IncidencePoolingAndAggregation> lstIPRemove = new List<IncidencePoolingAndAggregation>();
				foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
				{
					if (ip.lstAllSelectCRFuntion == null || ip.lstAllSelectCRFuntion.Count == 0)
					{
						lstIPRemove.Add(ip);
					}
				}
				foreach (IncidencePoolingAndAggregation ip in lstIPRemove)
				{
					CommonClass.lstIncidencePoolingAndAggregation.Remove(ip);
				}

				if (CommonClass.ValuationMethodPoolingAndAggregation == null) return;
				List<ValuationMethodPoolingAndAggregationBase> lstVBRemove = new List<ValuationMethodPoolingAndAggregationBase>();
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count == 0)
					{
						lstVBRemove.Add(vb);
					}
					else
					{



						List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
						if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
						{
							APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

						}
						lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
						if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0) { }
						else
						{
							APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
						}
						List<AllSelectValuationMethod> lstASVMRemove = new List<AllSelectValuationMethod>();
						List<int> lstAVM = new List<int>();
						if (vb.LstAllSelectValuationMethod != null && vb.LstAllSelectValuationMethod.Count > 0)
						{
							lstAVM = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.CRID).ToList();
							List<AllSelectValuationMethod> lstVTemp = vb.LstAllSelectValuationMethod.Where(p => vb.LstAllSelectValuationMethod.Where(a => a.PID == p.ID).Where(c => c.NodeType != 2000).Count() == 0 && p.NodeType != 2000).ToList();
							foreach (AllSelectValuationMethod asvm in lstVTemp)
							{
								if (!lstAVM.Contains(asvm.CRID))
								{
									lstASVMRemove.Add(asvm);
								}
							}
							foreach (AllSelectValuationMethod asvm in lstASVMRemove)
							{
								vb.LstAllSelectValuationMethod.Remove(asvm);
							}
							var query = vb.LstAllSelectValuationMethod.Where(p => p.NodeType == 2000);
							lstASVMRemove = new List<AllSelectValuationMethod>();
							foreach (AllSelectValuationMethod asvm in query)
							{
								if (vb.LstAllSelectValuationMethod.Where(p => p.ID == asvm.PID).Count() == 0)
									lstASVMRemove.Add(asvm);
							}
							foreach (AllSelectValuationMethod asvm in lstASVMRemove)
							{
								vb.LstAllSelectValuationMethod.Remove(asvm);
							}
						}
						List<AllSelectQALYMethod> lstQALYRemove = new List<AllSelectQALYMethod>();
						if (vb.lstAllSelectQALYMethod != null && vb.lstAllSelectQALYMethod.Count > 0)
						{
							lstAVM = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.CRID).ToList();
							List<AllSelectQALYMethod> lstQTemp = vb.lstAllSelectQALYMethod.Where(p => vb.lstAllSelectQALYMethod.Where(a => a.PID == p.ID).Where(c => c.NodeType != 3000).Count() == 0 && p.NodeType != 3000).ToList();
							foreach (AllSelectQALYMethod QALY in lstQTemp)
							{
								if (!lstAVM.Contains(QALY.CRID))
								{
									lstQALYRemove.Add(QALY);
								}
							}
							foreach (AllSelectQALYMethod QALY in lstQALYRemove)
							{
								vb.lstAllSelectQALYMethod.Remove(QALY);
							}
							var queryQALY = vb.lstAllSelectQALYMethod.Where(p => p.NodeType == 3000);
							lstQALYRemove = new List<AllSelectQALYMethod>();
							foreach (AllSelectQALYMethod QALY in queryQALY)
							{
								if (vb.lstAllSelectQALYMethod.Where(p => p.ID == QALY.PID).Count() == 0)
									lstQALYRemove.Add(QALY);
							}
							foreach (AllSelectQALYMethod QALY in lstQALYRemove)
							{
								vb.lstAllSelectQALYMethod.Remove(QALY);
							}
						}
					}
				}
				lstVBRemove = new List<ValuationMethodPoolingAndAggregationBase>();
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion == null || vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count == 0)
					{
						lstVBRemove.Add(vb);
					}
				}
				foreach (ValuationMethodPoolingAndAggregationBase vbRemove in lstVBRemove)
				{
					CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Remove(vbRemove);
				}

			}
			catch (Exception ex)
			{
			}
		}

		private void showExistBaseControlCRSelectFunction(BaseControlCRSelectFunction baseControlCRSelectFunction, TreeNode currentNode)
		{
			HealthImpactFunctions.MaxCRID = baseControlCRSelectFunction.lstCRSelectFunction.Max(p => p.CRID);
			CommonClass.LstPollutant = baseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
			CommonClass.LstBaseControlGroup = baseControlCRSelectFunction.BaseControlGroup;
			CommonClass.CRLatinHypercubePoints = baseControlCRSelectFunction.CRLatinHypercubePoints;
			CommonClass.CRDefaultMonteCarloIterations = baseControlCRSelectFunction.CRDefaultMonteCarloIterations;
			CommonClass.CRRunInPointMode = baseControlCRSelectFunction.CRRunInPointMode;
			CommonClass.CRThreshold = baseControlCRSelectFunction.CRThreshold;
			CommonClass.CRSeeds = baseControlCRSelectFunction.CRSeeds;
			CommonClass.BenMAPPopulation = baseControlCRSelectFunction.BenMAPPopulation;
			CommonClass.GBenMAPGrid = baseControlCRSelectFunction.BaseControlGroup.First().GridType;
			if (baseControlCRSelectFunction.RBenMapGrid != null)
				CommonClass.RBenMAPGrid = baseControlCRSelectFunction.RBenMapGrid;
			CommonClass.BenMAPPopulation = baseControlCRSelectFunction.BenMAPPopulation;
			int nodesCount = 0;
			foreach (TreeNode trchild in trvSetting.Nodes)
			{
				if (trchild.Name == "airqualitygridgroup")
				{
					nodesCount = trchild.Nodes.Count;


					for (int i = nodesCount - 1; i > -1; i--)
					{
						TreeNode node = trchild.Nodes[i];
						if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
					}
					for (int i = CommonClass.LstBaseControlGroup.Count - 1; i > -1; i--)
					{
						AddDataSourceNode(CommonClass.LstBaseControlGroup[i], trchild);
					}
					trchild.ExpandAll();

					foreach (TreeNode trair in trchild.Nodes)
					{
						changeNodeImage(trair);
						TreeNode tr = trair;
						if (trair.Name == "gridtype")
						{
							AddChildNodes(ref tr, "", "", new BenMAPLine());
							trair.ExpandAll();
						}
					}
				}
				if (trchild.Name == "configuration")
				{
					foreach (TreeNode tr in trchild.Nodes)
					{
						changeNodeImage(tr);
					}
					if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue == null)
						errorNodeImage(trchild.Nodes[trchild.Nodes.Count - 1]);
					trchild.ExpandAll();
				}
			}
		}

		private List<AllSelectQALYMethod> getChildFromAllSelectQALYMethod(AllSelectQALYMethod allSelectQALYMethod, ValuationMethodPoolingAndAggregationBase vb)
		{
			List<AllSelectQALYMethod> lstAll = new List<AllSelectQALYMethod>();
			var query = from a in vb.lstAllSelectQALYMethod where a.PID == allSelectQALYMethod.ID select a;
			lstAll = query.ToList();
			return lstAll;
		}

		private List<AllSelectValuationMethod> getChildFromAllSelectValuationMethod(AllSelectValuationMethod allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb)
		{
			List<AllSelectValuationMethod> lstAll = new List<AllSelectValuationMethod>();
			if (vb == null)
			{
				lstAll = lstAPVPoolingAndAggregationAll.Where(p => p.PID == allSelectValuationMethod.ID).ToList();
			}
			else
			{
				var query = from a in vb.LstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
				lstAll = query.ToList();
			}
			return lstAll;
		}



		private void getChildFromAllSelectCRFunctionUnPooled(AllSelectCRFunction allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb, ref List<AllSelectCRFunction> lstAll)
		{
			if (allSelectValuationMethod.PoolingMethod != null && (allSelectValuationMethod.PoolingMethod == "None" || allSelectValuationMethod.PoolingMethod == ""))//YY:
			{
				var query = from a in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion where a.PID == allSelectValuationMethod.ID select a;
				lstAll.AddRange(query.ToList());
				foreach (AllSelectCRFunction acr in query)
				{
					getChildFromAllSelectCRFunctionUnPooled(acr, vb, ref lstAll);
				}
			}

		}

		private void getChildFromAllSelectCRFunctionUnPooled(AllSelectCRFunction allSelectValuationMethod, IncidencePoolingAndAggregation ip, ref List<AllSelectCRFunction> lstAll)
		{
			//YY: (2/3) Another getChildFromAllSelectCRFunctionUnPooled but the second parameter is ip instead of vb.
			if (allSelectValuationMethod.PoolingMethod != null && (allSelectValuationMethod.PoolingMethod == "None" || allSelectValuationMethod.PoolingMethod == ""))//YY:
			{
				var query = from a in ip.lstAllSelectCRFuntion where a.PID == allSelectValuationMethod.ID select a;
				lstAll.AddRange(query.ToList());
				foreach (AllSelectCRFunction acr in query)
				{
					getChildFromAllSelectCRFunctionUnPooled(acr, ip, ref lstAll);
				}
			}

		}

		private void getChildFromAllSelectCRFunctionUnPooled(AllSelectCRFunction allSelectValuationMethod, List<AllSelectCRFunction> lstAllSelectValuationMethod, ref List<AllSelectCRFunction> lstAll)
		{
			//YY: (3/3) Another getChildFromAllSelectCRFunctionUnPooled but the second parameter is List<AllSelectCRFunction> instead of vb.
			if (allSelectValuationMethod.PoolingMethod != null && (allSelectValuationMethod.PoolingMethod == "None" || allSelectValuationMethod.PoolingMethod == ""))//YY:
			{
				var query = from a in lstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
				lstAll.AddRange(query.ToList());
				foreach (AllSelectCRFunction acr in query)
				{
					getChildFromAllSelectCRFunctionUnPooled(acr, lstAllSelectValuationMethod, ref lstAll);
				}
			}

		}

		private void getChildFromAllSelectValuationMethodUnPooled(AllSelectValuationMethod allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb, ref List<AllSelectValuationMethod> lstAll)
		{
			if (allSelectValuationMethod.PoolingMethod != null && (allSelectValuationMethod.PoolingMethod == "None" || allSelectValuationMethod.PoolingMethod == "" || allSelectValuationMethod.PoolingMethod == null)) //YY: Valuation may have pooling method = null for unpooled incidence.
			{
				var query = from a in vb.LstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
				lstAll.AddRange(query.ToList());
				foreach (AllSelectValuationMethod acr in query)
				{
					getChildFromAllSelectValuationMethodUnPooled(acr, vb, ref lstAll);
				}
			}

		}

		private void getChildFromAllSelectQALYMethodUnPooled(AllSelectQALYMethod allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb, ref List<AllSelectQALYMethod> lstAll)
		{
			if (allSelectValuationMethod.PoolingMethod != null && allSelectValuationMethod.PoolingMethod == "None")
			{
				var query = from a in vb.lstAllSelectQALYMethod where a.PID == allSelectValuationMethod.ID select a;
				lstAll.AddRange(query.ToList());
				foreach (AllSelectQALYMethod acr in query)
				{
					getChildFromAllSelectQALYMethodUnPooled(acr, vb, ref lstAll);
				}
			}

		}

		private void addBenMAPLineToMainMap(BenMAPLine benMAPLine, string isBase)
		{
			mainMap.ProjectionModeReproject = ActionMode.Never;
			mainMap.ProjectionModeDefine = ActionMode.Never;

			string IsBaseLongText;
			switch (isBase)
			{
				case "B":
					IsBaseLongText = "Baseline";
					break;
				case "C":
					IsBaseLongText = "Control";
					break;
				case "D":
					IsBaseLongText = "Delta";
					break;
				default:
					return;
			}

			MapGroup bcgGreatGrandParentGroup = new MapGroup();
			MapGroup bcgMapGroup = new MapGroup();
			MapGroup adminLayerMapGroup = new MapGroup();
			MapGroup polMapGroup = new MapGroup();

			string pollutantMGText;
			string bcgMGText;
			string LayerNameText;
			string LayerLegendText;

			//Get Metrics fields for this pollutant. 
			//If no metrics then return with warning/error
			List<string> lstAddField = new List<string>();
			if (benMAPLine.Pollutant.Metrics != null)
			{
				foreach (Metric metric in benMAPLine.Pollutant.Metrics)
				{
					lstAddField.Add(metric.MetricName);
				}
			}
			if (benMAPLine.Pollutant.SesonalMetrics != null)
			{
				foreach (SeasonalMetric sesonalMetric in benMAPLine.Pollutant.SesonalMetrics)
				{
					lstAddField.Add(sesonalMetric.SeasonalMetricName);
				}
			}
			//Add a layer for each metric for this pollutant
			for (int iAddField = 2; iAddField < 2 + lstAddField.Count; iAddField++)
			{
				MapPolygonLayer polLayer = new MapPolygonLayer();

				pollutantMGText = benMAPLine.Pollutant.PollutantName.ToString();
				_columnName = lstAddField[iAddField - 2];
				bcgMGText = _columnName.ToString();
				LayerLegendText = IsBaseLongText;
				LayerNameText = pollutantMGText + "_" + "_" + bcgMGText + "_" + LayerLegendText;

				try
				{

					bcgGreatGrandParentGroup = AddMapGroup(_bcgGroupLegendText, "Map Layers", false, false);
					polMapGroup = AddMapGroup(pollutantMGText, _bcgGroupLegendText, false, false);
					bcgMapGroup = AddMapGroup(bcgMGText, pollutantMGText, false, false);

					//Remove the old version of the layer if exists already
					RemoveOldPolygonLayer(LayerNameText, polMapGroup.Layers, false);  //!!!!!!!!!!!!Need to trap for problems removing the old layer if it exists?
					RemoveOldPolygonLayer(LayerNameText, bcgMapGroup.Layers, false);  //!!!!!!!!!!!!Need to trap for problems removing the old layer if it exists?

					// Add a new layer baseline, control or delta layer to the Pollutants group
					if (File.Exists(benMAPLine.ShapeFile))
					{
						try
						{
							polLayer = (MapPolygonLayer)bcgMapGroup.Layers.Add(benMAPLine.ShapeFile);
							polLayer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
							polLayer.Reproject(mainMap.Projection);
						}
						catch (Exception ex)
						{
							//dpa todo: check on this. Why are we doing these tasks during an exception?
							DataSourceCommonClass.SaveBenMAPLineShapeFile(CommonClass.GBenMAPGrid, benMAPLine.Pollutant, benMAPLine, benMAPLine.ShapeFile);
							polLayer = (MapPolygonLayer)bcgMapGroup.Layers.Add(benMAPLine.ShapeFile);   //-MCB use when mapgroup layers is working correctly
						}
					}
					else
					{
						if (string.IsNullOrEmpty(benMAPLine.ShapeFile))
						{
							//benMAPLine.ShapeFile = benMAPLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + isBase + ".shp";
							benMAPLine.ShapeFile = benMAPLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + IsBaseLongText + ".shp";
							benMAPLine.ShapeFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, benMAPLine.ShapeFile);
						}
						if (benMAPLine.ModelResultAttributes != null)  //MCB added this until we can figure out why the result attributes are not being populated 
						{
							DataSourceCommonClass.SaveBenMAPLineShapeFile(CommonClass.GBenMAPGrid, benMAPLine.Pollutant, benMAPLine, benMAPLine.ShapeFile);    ///MCB- Commemented out to resolve issues with not drawing non-saved data (e.g., Monitor data).  This may just be a twmp fix and May cause problems elsewhere
						}
						polLayer = (MapPolygonLayer)bcgMapGroup.Layers.Add(benMAPLine.ShapeFile);
					}
				}
				catch (Exception ex)
				{
					Logger.LogError(ex);
					Debug.WriteLine("addBenMAPLineToMainMap: Error adding new layer " + LayerNameText + " :" + ex.ToString());
				}

				//define the symbology, legend text and identifying name for the layer
				polLayer.DataSet.DataTable.Columns[iAddField].ColumnName = _columnName; // lstAddField[iAddField - 2];
				polLayer.LegendText = LayerLegendText;
				polLayer.Name = LayerNameText;

				DataView view = new DataView(polLayer.DataSet.DataTable);

				DataTable distinctValues = view.ToTable(true, _columnName);

				int numCategories;
				if (distinctValues.Rows.Count < 6)
					numCategories = distinctValues.Rows.Count;
				else
					numCategories = 6;

				try
				{
					PolygonScheme myNewScheme = CreateBCGPolyScheme(ref polLayer, numCategories, isBase);
					polLayer.Symbology = myNewScheme;
					polLayer.ApplyScheme(myNewScheme);

					//set the layer to have its symbology expanded by default //-MCB
					polLayer.Symbology.IsExpanded = true;
				}
				catch (Exception ex)
				{
					Logger.LogError(ex);
					Debug.WriteLine("Error applying symbology for " + LayerNameText + " :" + ex.ToString());
				}

			}
			return;
		}

		public Color[] BlendColors
		{
			get { return _blendColors; }
			set { _blendColors = value; }
		}

		private PolygonScheme CreateBCGPolyScheme(ref MapPolygonLayer polLayer, int CategoryNumber, string isBase = "B")
		{
			// Note: colorBlend function was hard-coded to 6 categories, so the "CategoryNumber" specification here must also be hard-coded. - dpa
			if (isBase == "D") //use the delta color ramp
			{
				colorBlend.ColorArray = GetColorRamp("oranges", CategoryNumber);
			}
			else //use the default color ramp
			{
				colorBlend.ColorArray = GetColorRamp("pale_yellow_blue", CategoryNumber); //pale_yellow_blue
			}
			PolygonScheme myScheme1 = new PolygonScheme();
			myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
			myScheme1.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;
			myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding;
			myScheme1.EditorSettings.IntervalRoundingDigits = 3; //number of significant figures (or decimal places if using rounding)
			myScheme1.EditorSettings.NumBreaks = CategoryNumber + 1;
			myScheme1.EditorSettings.FieldName = _columnName;
			myScheme1.EditorSettings.UseGradient = false;
			myScheme1.EditorSettings.MaxSampleCount = 5000; // default is 10000 - how many samples from the data table to use when computing breaks.
			myScheme1.ClearCategories();
			myScheme1.CreateCategories(polLayer.DataSet.DataTable);    ///MCB- Note: This method can't deal with negative numbers correctly

			// Set the category colors equal to the selected color ramp
			SimplePattern sp;
			PolygonSymbolizer poly;
			for (int catNum = 0; catNum < CategoryNumber; catNum++)
			{
				//Create the simple pattern with opacity
				sp = new SimplePattern(colorBlend.ColorArray[catNum]);
				sp.Outline = new LineSymbolizer(Color.Transparent, 0); // Outline is nessasary
				sp.Opacity = 1.0F;  //100% opaque = 0% transparent
				poly = new PolygonSymbolizer(new List<IPattern> { sp });
				myScheme1.Categories[catNum].Symbolizer = poly;
			}

			if (CategoryNumber.Equals(6))
			{
				// Create a final category to catch the few outlier values at the top end
				Double topSampledValue = (Double)myScheme1.Categories[myScheme1.Categories.Count - 1].Maximum;
				ICategory outlierCategory = (ICategory)myScheme1.Categories[0].Clone();
				outlierCategory.Minimum = topSampledValue;
				outlierCategory.Maximum = topSampledValue * 2;
				//outlierCategory.Range.Maximum = double.MaxValue;
				//outlierCategory.Range.Minimum = topSampledValue;

				//myScheme1.EditorSettings.NumBreaks = CategoryNumber + 1;
				outlierCategory.ApplyMinMax(myScheme1.EditorSettings);
				outlierCategory.LegendText = string.Format("> {0}", topSampledValue.ToString("F3"));
				myScheme1.AddCategory(outlierCategory);
				sp = new SimplePattern(Color.FromArgb(0, 40, 120)); // Darker blue for the last outlier cateogory
				sp.Outline = new LineSymbolizer(Color.Transparent, 0); // Outline is nessasary
				sp.Opacity = 1.0F;  //100% opaque = 0% transparent
				poly = new PolygonSymbolizer(new List<IPattern> { sp });
				myScheme1.Categories[6].Symbolizer = poly;
			}

			// Final scheme properties
			myScheme1.AppearsInLegend = false; //if true then legend text displayed
			myScheme1.IsExpanded = true;
			myScheme1.LegendText = _columnName;

			return myScheme1;
		}

		private PolygonScheme CreateResultPolyScheme(ref MapPolygonLayer polLayer, int CategoryCount = 6, string isBase = "R")
		{
			PolygonScheme myScheme1 = new PolygonScheme();
			myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
			myScheme1.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;
			myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding;
			myScheme1.EditorSettings.IntervalRoundingDigits = 3; //number of significant figures (or decimal places if using rounding)
			myScheme1.EditorSettings.NumBreaks = CategoryCount;
			myScheme1.EditorSettings.FieldName = _columnName;
			myScheme1.EditorSettings.UseGradient = false;
			myScheme1.EditorSettings.MaxSampleCount = int.MaxValue; //added from merge develop

			// Build the color ramp
			switch (isBase)
			{
				case "D":  //use the delta color ramp
									 //colorBlend.ColorArray = GetColorRamp("blue_red", CategoryNumber);
					colorBlend.ColorArray = GetColorRamp("oranges", CategoryCount);
					break;
				case "R": //Configuration Results -MCB choose another color ramp???
					colorBlend.ColorArray = GetColorRamp("brown_green", CategoryCount);
					break;
				case "I": //Pooled Incidence Results??? -MCB choose another color ramp???
					colorBlend.ColorArray = GetColorRamp("yellow_red", CategoryCount);
					break;
				case "H": //Health Impact Function -MCB choose another color ramp???
					colorBlend.ColorArray = GetColorRamp("blues", CategoryCount);  //"pale_blue_green"
					break;
				case "A": //Pooled Valuation Results -MCB choose another color ramp???
					colorBlend.ColorArray = GetColorRamp("purples", CategoryCount);
					break;
				case "IP": //Pooled Incidence Results -MCB choose another color ramp???
					colorBlend.ColorArray = GetColorRamp("oranges", CategoryCount);
					break;
				default: //use the default color ramp
					colorBlend.ColorArray = GetColorRamp("pale_yellow_blue", CategoryCount); //pale_yellow_blue
					break;
			}
			myScheme1.CreateCategories(polLayer.DataSet.DataTable);

			for (int catNum = 0; catNum < myScheme1.Categories.Count; catNum++)
			{
				myScheme1.Categories[catNum].Symbolizer.SetOutline(Color.Transparent, 0); //make the outlines invisble
				myScheme1.Categories[catNum].SetColor(colorBlend.ColorArray[catNum].ToTransparent((float)0.9));
				// Force the top category to be open-ended to avoid issues with rounding errors in the max value
				if (catNum == myScheme1.Categories.Count - 1)
				{
					String tmp = myScheme1.Categories[catNum].FilterExpression;
					if (tmp.IndexOf(" AND ") > 0)
					{
						myScheme1.Categories[catNum].FilterExpression = tmp.Substring(0, tmp.IndexOf(" AND ")); //BenMAP 400- Previous code caused issues when exporting pooled results--left brackets in filter expression, throwing
					}                                                                                           //an exception after returning scheme--previous code was tmp.Substring(0, tmp.Length - tmp.IndexOf(" AND ") - 6)
				}

			}

			myScheme1.AppearsInLegend = true; //if true then legend text displayed
			myScheme1.IsExpanded = true;
			myScheme1.LegendText = _columnName;

			return myScheme1;

		}

		private bool BaseControlOP(string currStat, ref TreeNode currentNode)
		{
			string msg = string.Empty;
			DialogResult rtn;
			BaseControlGroup bcg = new BaseControlGroup();
			BenMAPLine bml = new BenMAPLine();
			try
			{
				if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0) { msg = "Please select pollutant."; return false; }
				if (CommonClass.LstBaseControlGroup == null || CommonClass.LstBaseControlGroup.Count == 0) { msg = "Please select pollutant."; return false; }
				string nodeTag = currentNode.Parent.Tag.ToString();
				foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
				{
					if (int.Parse(nodeTag) == b.Pollutant.PollutantID) { bcg = b; break; }
				}
				if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
				{
					lock (CommonClass.LstAsynchronizationStates)
					{
						if (CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcg.Pollutant.PollutantName.ToLower(), currStat)))
						{
							msg = " BenMAP is creating the air quality surface.";
							return false; ;
						}
					}
				}

				GridCreationMethods frm = null;
				bool isGridTypeChanged = false;
				bool removeNode = true; switch (currStat)
				{
					case "baseline":
						if (bcg.Base == null)
						{
							bcg.Base = new BenMAPLine();
							bcg.Base.GridType = bcg.GridType;
							bcg.Base.Pollutant = bcg.Pollutant;
						}
						frm = new GridCreationMethods(bcg, currStat, _AQGPath); //added from merge develop
						rtn = frm.ShowDialog();
						if (rtn != DialogResult.OK) { return false; }
						_AQGPath = frm.SaveAQGPath;
						isGridTypeChanged = frm.isGridTypeChanged;
						if (frm.PageStat == "monitor")
						{
						}
						if (frm.PageStat == "monitorrollback" && MonitorRollbackSettings3.MakeBaselineGrid.Length > 1 && MonitorRollbackSettings3.MakeBaselineGrid.Substring(0, 1) == "T")
						{
							string err = "";
							bcg.Base = DataSourceCommonClass.LoadAQGFile(MonitorRollbackSettings3.MakeBaselineGrid.Substring(1, MonitorRollbackSettings3.MakeBaselineGrid.Length - 1), ref err);
							bml = bcg.Base;
							removeNode = false;
							if (CommonClass.LstBaseControlGroup != null)
							{
								for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
								{
									if (CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID == bcg.Pollutant.PollutantID)
									{
										initNodeImage(currentNode.Parent.Nodes[1]);
										currentNode.Parent.Nodes[1].Nodes.Clear();
									}
								}
							}
							TreeNode controlNode = currentNode.Parent.Nodes[1];
							AddChildNodes(ref controlNode, frm.PageStat, frm.StrPath.Substring(frm.StrPath.LastIndexOf(@"\") + 1), bcg.Control);
							changeNodeImage(currentNode.Parent.Nodes[1]);
						}
						else
						{
							bml = bcg.Base;
							changeNodeImage(currentNode);
						}
						break;

					case "control":
						if (bcg.Control == null)
						{
							bcg.Control = new BenMAPLine();
							bcg.Control.GridType = bcg.GridType;
							bcg.Control.Pollutant = bcg.Pollutant;
						}
						frm = new GridCreationMethods(bcg, currStat, _AQGPath); //added from merge develop
						rtn = frm.ShowDialog();
						_AQGPath = frm.SaveAQGPath;
						if (rtn != DialogResult.OK) { return false; }
						isGridTypeChanged = frm.isGridTypeChanged;
						if (frm.PageStat == "monitor")
						{
						}
						if (frm.PageStat == "monitorrollback" && MonitorRollbackSettings3.MakeBaselineGrid.Length > 1 && MonitorRollbackSettings3.MakeBaselineGrid.Substring(0, 1) == "T")
						{
							string err = "";
							bcg.Base = DataSourceCommonClass.LoadAQGFile(MonitorRollbackSettings3.MakeBaselineGrid.Substring(1, MonitorRollbackSettings3.MakeBaselineGrid.Length - 1), ref err);
							bml = bcg.Control;
							removeNode = false;
							if (CommonClass.LstBaseControlGroup != null)
							{
								for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
								{
									if (CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID == bcg.Pollutant.PollutantID)
									{
										initNodeImage(currentNode.Parent.Nodes[0]);
										currentNode.Parent.Nodes[0].Nodes.Clear();
									}
								}
							}
							TreeNode bsaeNode = currentNode.Parent.Nodes[0];
							AddChildNodes(ref bsaeNode, frm.PageStat, frm.StrPath.Substring(frm.StrPath.LastIndexOf(@"\") + 1), bcg.Base);
						}
						else
						{
							bml = bcg.Control;
							changeNodeImage(currentNode);
						}
						break;
				}
				bcg.DeltaQ = null;
				if (isGridTypeChanged)
				{
					if (CommonClass.LstBaseControlGroup != null)
					{
						for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
						{
							if (CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID != bcg.Pollutant.PollutantID)
							{
								CommonClass.LstBaseControlGroup[i].Base = null;
								CommonClass.LstBaseControlGroup[i].Control = null;
								GC.Collect();
							}
							else
							{
								switch (currStat)
								{
									case "baseline":
										if (removeNode)
										{
											initNodeImage(currentNode.Parent.Nodes[1]);
											currentNode.Parent.Nodes[1].Nodes.Clear();
											CommonClass.LstBaseControlGroup[i].Control = null;
										}
										break;
									case "control":
										if (removeNode)
										{
											initNodeImage(currentNode.Parent.Nodes[0]);
											currentNode.Parent.Nodes[0].Nodes.Clear();
											CommonClass.LstBaseControlGroup[i].Base = null;
										}
										break;
								}

							}
						}
					}

					for (int i = currentNode.Parent.Parent.Nodes.Count - 1; i > -1; i--)
					{
						TreeNode node = currentNode.Parent.Parent.Nodes[i];
						if (currentNode.Parent.Parent.Nodes[i].Name == "datasource")
						{
							if (bcg.Pollutant.PollutantID != int.Parse(currentNode.Parent.Parent.Nodes[i].Tag.ToString()))
							{
								currentNode.Parent.Parent.Nodes[i].Nodes[0].Nodes.Clear();
								initNodeImage(currentNode.Parent.Parent.Nodes[i].Nodes[0]);
								currentNode.Parent.Parent.Nodes[i].Nodes[1].Nodes.Clear();
								initNodeImage(currentNode.Parent.Parent.Nodes[i].Nodes[1]);
								initNodeImage(currentNode.Parent.Parent.Nodes[i]);
							}
						}
					}


					if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
					{
						CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
					}

					olvCRFunctionResult.SetObjects(null);
					olvIncidence.SetObjects(null);
					tlvAPVResult.SetObjects(null);

					cbPoolingWindowIncidence.Items.Clear();
					cbPoolingWindowAPV.Items.Clear();
					ClearMapTableChart();
					CommonClass.BenMAPPopulation = null;
					initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[0]);
					if (CommonClass.BaseControlCRSelectFunction != null)
					{
						errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes.Count - 1]);
					}
					if (CommonClass.lstIncidencePoolingAndAggregation != null)
					{
						errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
					}
					if (CommonClass.ValuationMethodPoolingAndAggregation != null)
					{
						errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
					}
				}
				string strName = "";
				try
				{
					strName = frm.StrPath.Substring(frm.StrPath.LastIndexOf(@"\") + 1);
				}
				catch
				{ }
				AddChildNodes(ref currentNode, frm.PageStat, strName, bml);
				bcg.DeltaQ = null;
				currentNode.ExpandAll();
				if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
				{
					CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
				}
				if (bcg.Base != null && bcg.Control != null) changeNodeImage(currentNode.Parent);
				if (CommonClass.BaseControlCRSelectFunction != null)
				{
					errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[1]);
				}
				if (CommonClass.lstIncidencePoolingAndAggregation != null)
				{
					errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[1]);
				}
				if (CommonClass.ValuationMethodPoolingAndAggregation != null)
				{
					errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[2]);
				}
				int index = currentNode.Parent.Index;
				TreeNode parent = currentNode.Parent.Parent;
				BrushBaseControl(ref parent, bcg, index);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return false;
			}
			finally
			{
				if (msg != string.Empty)
				{ MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
			}
		}

		private void BrushBaseControl(ref TreeNode pnode, BaseControlGroup bcg, int index)
		{
			try
			{
				TreeNode node = new TreeNode()
				{
					Name = "datasource",
					Tag = bcg.Pollutant.PollutantID,
					Text = string.Format("Source of Air Quality Data ({0})", bcg.Pollutant.PollutantName),
					ToolTipText = "Double-click to upload AQ data files",
					ImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
					SelectedImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
					Nodes = { new TreeNode() {
														Name = "baseline",
														Text = "Baseline",
														ToolTipText="Double-click to load AQ data",
														Tag = bcg.Base,
														ImageKey = (CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"baseline"))? _yibuImageKey:(bcg.Base == null || bcg.Base.ModelResultAttributes==null || bcg.Base.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey,
														SelectedImageKey =(CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"baseline"))? _yibuImageKey:(bcg.Base == null || bcg.Base.ModelResultAttributes==null || bcg.Base.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey,
														},
														new TreeNode() {
																Name = "control",
																Text = "Control",
																ToolTipText="Double-click to load AQ data",
																Tag = bcg.Control,
														ImageKey = (CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"control"))? _yibuImageKey:(bcg.Control == null || bcg.Control.ModelResultAttributes==null || bcg.Control.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey,
														SelectedImageKey =(CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"control"))? _yibuImageKey:(bcg.Control == null || bcg.Control.ModelResultAttributes==null || bcg.Control.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey},
														new TreeNode() {
																Name = "delta",
																Text = "Air quality delta (baseline - control)",
																ToolTipText="Double-click to load AQ data",
																Tag = bcg,
														 ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 &&(bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) )? "doc" :"docgrey",
												SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 &&(bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) )? "doc" :"docgrey", }
								}
				};
				if (bcg.Base != null)
				{
					string s = "";
					try
					{
						//ModelDataLine mdl = (BenMAPLine)bcg.Base;
						//String test = mdl.DatabaseFilePath;
						//bcg.Base.da
						if(bcg.Base is ModelDataLine)
						{
							s = (bcg.Base as ModelDataLine).DatabaseFilePath.Substring((bcg.Base as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("Broken cast " + e.ToString());
					}
					node.Nodes[0].Nodes.Add(new TreeNode()
					{
						Name = "basedata",
						Text = (bcg.Base is ModelDataLine) ? s : "Base Data",
						ToolTipText = "Double-click AQ data file to display map/data",
						Tag = bcg.Base,
						ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
						SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
					});
				}
				if (bcg.Control != null)
				{
					string s = "";
					try
					{
						if(bcg.Control is ModelDataLine)
						{
							s = (bcg.Control as ModelDataLine).DatabaseFilePath.Substring((bcg.Control as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);
						}
						

					}
					catch
					{ }
					node.Nodes[1].Nodes.Add(new TreeNode()
					{
						Name = "controldata",
						Text = (bcg.Control is ModelDataLine) ? s : "Control Data",
						ToolTipText = "Double-click AQ data file to display map/data",
						Tag = bcg.Control,
						ImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
						SelectedImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
					});
				}
				if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
				{
					changeNodeImage(trvSetting.Nodes[0].Nodes[0]);
				}
				node.ExpandAll();
				pnode.Nodes.RemoveAt(index);
				pnode.Nodes.Insert(index, node);
				pnode.ExpandAll();
				if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
				{
					CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
				}
				{ changeNodeImage(pnode); }
				if (CommonClass.BaseControlCRSelectFunction != null)
				{
					errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[1]);
				}
				if (CommonClass.lstIncidencePoolingAndAggregation != null)
				{
					errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[1]);
				}
				if (CommonClass.ValuationMethodPoolingAndAggregation != null)
				{
					errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[2]);
				}

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void AddChildNodes(ref TreeNode node, string currStat, string txt, BenMAPLine bml)
		{
			try
			{
				if (node == null) { return; }
				string nodeName = node.Name; ;
				string tag = string.Empty;
				TreeNode newNode;
				BaseControlGroup bcg = null;
				switch (nodeName)
				{
					case "gridtype":
						if (CommonClass.GBenMAPGrid == null) { return; }
						node.Nodes.Clear(); newNode = CreateNewNode(CommonClass.GBenMAPGrid, "Grid");
						if (newNode != null) { node.Nodes.Add(newNode); }
						if (CommonClass.RBenMAPGrid != null)
						{
							string target = "Domain";
							BenMAPGrid grid = CommonClass.RBenMAPGrid;
							newNode = new TreeNode
							{
								Name = "region",
								Text = string.Format("{0}: {1}", target, grid.GridDefinitionName),
								Tag = grid,
								ToolTipText = "Double-click domain data file to display ",
								ImageKey = "doc",
								SelectedImageKey = "doc",
							};
							if (newNode != null) { node.Nodes.Add(newNode); }
						}
						break;
					case "baseline":
						node.Nodes.Clear();
						foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
						{
							if (int.Parse(node.Parent.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
						}
						if (string.IsNullOrEmpty(txt)) { txt = "data from library"; }
						newNode = new TreeNode()
						{
							Name = "basedata",
							Text = txt,
							Tag = bml,
							ToolTipText = "Double-click AQ data file to display ",
							ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
							SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
						};
						node.Nodes.Add(newNode);
						break;
					case "control":
						node.Nodes.Clear(); foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
						{
							if (int.Parse(node.Parent.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
						}
						newNode = new TreeNode()
						{
							Name = "controldata",
							Text = txt,
							Tag = bml,
							ToolTipText = "Double-click AQ data file to display ",
							ImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
							SelectedImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
						};
						node.Nodes.Add(newNode);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private TreeNode CreateNewNode(BenMAPGrid grid, string target)
		{
			TreeNode node;
			try
			{
				node = new TreeNode()
				{
					Name = target.ToLower(),
					Text = string.Format("{0}: {1}", target, grid.GridDefinitionName),

					Tag = grid,
					ToolTipText = "Double-click grid data file to display ",
					ImageKey = "doc",
					SelectedImageKey = "doc",
				};

				return node;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return null;
			}
		}

		private void initNodeImage(TreeNode rootNode)
		{
			try
			{
				rootNode.ImageKey = _unreadyImageKey;
				rootNode.SelectedImageKey = _unreadyImageKey;
			}
			catch
			{
			}
		}

		private void errorNodeImage(TreeNode rootNode)
		{

			try
			{
				rootNode.ImageKey = _errorImageKey;
				rootNode.SelectedImageKey = _errorImageKey;
			}
			catch
			{
			}
		}

		private void changeNodeImage(TreeNode rootNode)
		{
			bool hasUnRead = false;
			try
			{
				hasUnRead = JudgeStatus(rootNode);
				if (hasUnRead) { return; }
				rootNode.ImageKey = _readyImageKey;
				rootNode.SelectedImageKey = _readyImageKey;

				rootNode.ExpandAll();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private bool JudgeStatus(TreeNode node)
		{
			try
			{
				bool hasUnRead = false;
				string nodeName = node.Name;
				switch (nodeName)
				{
					case "airqualitygridgroup":
					case "configuration":
					case "aggregationpoolingvaluation":
						hasUnRead = true;
						break;
				}
				return hasUnRead;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		private void ShowTable(string file)
		{
			_reportTableFileName = file;
			tabCtlMain.SelectTab(tabData);
		}

		private void ShowChart(string resultFile)
		{
			/* zedGraphCtl.Visible = true;
			ZedGraphResult(zedGraphCtl, resultFile);
			zedGraphCtl.AxisChange();
			zedGraphCtl.Refresh();
			tabCtlMain.SelectTab(tabChart); */
		}

		private Dictionary<string, double> addRegionValue(DataTable dt)
		{
			Dictionary<string, double> DicRegionValue = new Dictionary<string, double>();
			int im = 0, iCmaqGrid = 0, i = 0, j = 0, iRegion = 0;

			i = 0;
			List<ModelAttribute> lsdt = new List<ModelAttribute>();


			return DicRegionValue;
		}

		#region "Un used"
		// ZED
		/* private void ZedGraphResult(ZedGraphControl zgc, string file)
		{ 
				try
				{
						System.Data.DataTable dt = DataSourceCommonClass.getDataSetFromCSV(file).Tables[0]; System.Data.DataSet dsOut = new System.Data.DataSet();

						int i = 0;
						int irepeat = 0;

						while (i < 5)
						{
								if (dt.Rows[0][0].ToString() == dt.Rows[i][0].ToString() &&
										dt.Rows[0][1].ToString() == dt.Rows[i][1].ToString())
								{
										irepeat = i + 1;
								}
								i++;
						}
						DataTable dt1 = dt.Clone();
						DataTable dt2 = dt.Clone();
						dt2.TableName = "Table2";
						DataTable dt3 = dt.Clone();
						dt3.TableName = "Table3";

						dt1.Rows.Clear();
						dt2.Rows.Clear();
						dt3.Rows.Clear();

						dsOut.Tables.Add(dt1);
						dsOut.Tables.Add(dt2);
						dsOut.Tables.Add(dt3);

						if (irepeat > 0)
						{
								while (i < dt.Rows.Count / irepeat)
								{
										if (irepeat > 1)
										{
												dsOut.Tables[0].Rows.Add(dt.Rows[i * irepeat + 0].ItemArray);
												dsOut.Tables[1].Rows.Add(dt.Rows[i * irepeat + 1].ItemArray);
										}
										if (irepeat > 2)
										{
												dsOut.Tables[2].Rows.Add(dt.Rows[i * irepeat + 2].ItemArray);
										}
										i++;
								}
						}
						if (dsOut.Tables[2].Rows.Count == 0) dsOut.Tables.RemoveAt(2);
						if (dsOut.Tables[1].Rows.Count == 0) dsOut.Tables.RemoveAt(1);
						if (dsOut.Tables[0].Rows.Count == 0)
						{
								MessageBox.Show("");
								return;
						}
						Dictionary<string, double>[] dicRegionValues = new Dictionary<string, double>[irepeat];
						i = 0;
						while (i < irepeat)
						{
								dicRegionValues[i] = addRegionValue(dsOut.Tables[i]);
								i++;
						}


						string[] strValuations = { "Woodruff,etc.", "Laden,etc.", "Pope,etc." };
						List<string> strValuationsNow = new List<string>();
						GraphPane myPane = zgc.GraphPane;
						myPane.Title.Text = "Valuation Result";
						myPane.XAxis.Title.Text = "Region";
						myPane.YAxis.Title.Text = "Monetary Value($)";
						myPane.CurveList.Clear();
						i = 0;
						Color[] colorArray = new Color[] { Color.Blue, Color.Red, Color.Green };
						while (i < irepeat)
						{
								PointPairList list = new PointPairList();
								int j = 0;
								while (j < 5)
								{
										list.Add(new PointPair(Convert.ToInt32(j), dicRegionValues[i].ToArray()[j].Value));
										j++;
								}

								OxyPlot.ColumnSeries
								OxyPlot.Series.BarItem myCurve = myPane.AddBar(strValuationsNow[i], list, colorArray[i]);

								i++;
						}
						myPane.Chart.Fill = new Fill(Color.White,
						 Color.FromArgb(255, 255, 166), 45.0F);

						myPane.XAxis.Scale.TextLabels = new string[] { dicRegionValues[0].Keys.ToArray()[0], dicRegionValues[0].Keys.ToArray()[1], dicRegionValues[0].Keys.ToArray()[2], dicRegionValues[0].Keys.ToArray()[3], dicRegionValues[0].Keys.ToArray()[4] };
						myPane.XAxis.Type = AxisType.Text;
						myPane.XAxis.Scale.FontSpec.Angle = 65;
						myPane.XAxis.Scale.FontSpec.IsBold = true;
						myPane.XAxis.Scale.FontSpec.Size = 12;
						zgc.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;

						zgc.AxisChange();
						zgc.Refresh();


				}
				catch (Exception err)
				{
						Logger.LogError(err);
				} 
		} */

		// ZED 
		/* private void ZedGraphDemo(ZedGraphControl zgc)
		{ 
				GraphPane myPane = zgc.GraphPane;
				string[] str = { "North", "South", "West", "East", "Central" };

				myPane.Title.Text = "Vertical Bars with Value Labels Above Each Bar";
				myPane.XAxis.Title.Text = "Position Number";
				myPane.YAxis.Title.Text = "Some Random Thing";

				PointPairList list = new PointPairList();
				PointPairList list2 = new PointPairList();
				PointPairList list3 = new PointPairList();
				Random rand = new Random();


				for (int i = 0; i < 5; i++)
				{
						double x = (double)i;
						double y = rand.NextDouble() * 1000;
						double y2 = rand.NextDouble() * 1000;
						double y3 = rand.NextDouble() * 1000;
						list.Add(x, y, 0, "aaaa");
						list2.Add(x, y2);
						list3.Add(x, y3);
				}

				OxyPlot.Series.BarItem myCurve = myPane.AddBar("curve 1", list, Color.Blue);
				OxyPlot.Series.BarItem myCurve2 = myPane.AddBar("curve 2", list2, Color.Red);
				OxyPlot.Series.BarItem myCurve3 = myPane.AddBar("curve 3", list3, Color.Green);

				myPane.Chart.Fill = new Fill(Color.White,
Color.FromArgb(255, 255, 166), 45.0F);

				myPane.XAxis.Scale.TextLabels = str;
				myPane.XAxis.Type = AxisType.Text;
				OxyPlot.Series.BarItem.CreateBarLabels(myPane, false, "f0");
				zgc.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;

				zgc.AxisChange();
				zgc.Refresh(); 

		} */

		//TODO: Get Metadata for the autid trail
		/// <summary>
		/// Gets the metadata of the dataset used for audit trail.
		/// NOT YET COMPLETED
		/// passing in the tree view that will be added to.
		/// </summary>
		/// <param name="trv">The TRV.</param>
		#endregion

		private void ShowCumulative()
		{
			oxyPlotView.Visible = false;
			System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\Cumulative Distributions.JPG");
			pnlChart.BackgroundImage = backImg;
			tabCtlMain.SelectTab(tabChart);
		}

		private void ShowBoxPlot()
		{
			oxyPlotView.Visible = false;
			System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\BoxPlot.jpg");
			pnlChart.BackgroundImage = backImg;
			tabCtlMain.SelectTab(tabChart);
		}

		private bool ExportDataset2CSV(System.Data.DataSet ds, string fileName)
		{
			try
			{
				StreamWriter swriter = new StreamWriter(fileName, false, Encoding.Default);
				string str = string.Empty;
				str = string.Format("Column,Row,ValuationResult");
				swriter.WriteLine(str);

				int tableCount = ds.Tables.Count;
				int rowCount = ds.Tables[0].Rows.Count;
				for (int i = 0; i < rowCount; i++)
				{
					for (int j = 0; j < tableCount; j++)
					{
						if (ds.Tables[j].Rows.Count == 0) continue;

						str = string.Format("{0},{1},{2}", ds.Tables[j].Rows[i][0].ToString(), ds.Tables[j].Rows[i][1].ToString(), ds.Tables[j].Rows[i][2].ToString());

						swriter.WriteLine(str);
					}
				}


				swriter.Flush(); swriter.Close();
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return false;
			}
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		public string _outputFileName;

		public void btnTableOutput_Click(object sender, EventArgs e)
		{
			if (_tableObject != null)
			{
				bool isBatch = false;

				if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
				{
					isBatch = true;
				}
				try
				{

					if (!isBatch)
					{
						SaveFileDialog saveFileDialog1 = new SaveFileDialog();
						saveFileDialog1.Filter = "CSV File|*.CSV";
						if (_tableObject is CRSelectFunctionCalculateValue || _tableObject is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> || _tableObject is List<CRSelectFunctionCalculateValue>)
							saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFGR";
						else if (_tableObject is BenMAPLine)
							saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
						else if (_tableObject is List<AllSelectValuationMethodAndValue> || _tableObject is AllSelectValuationMethodAndValue)
							saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";
						else if (_tableObject is List<AllSelectCRFunction>)
							saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";
						else if (_tableObject is Dictionary<AllSelectCRFunction, string>)
							saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";

						if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
						{
							return;
						}

						_outputFileName = saveFileDialog1.FileName;
					}
					int i = 0;
					DataTable dt = new DataTable();
					//Incidence Pooling (old and obsolete)
					if (_tableObject is List<AllSelectCRFunction>)
					{
						List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)_tableObject;
						if (this.IncidencelstColumnRow == null)
						{
							dt.Columns.Add("Col", typeof(int));
							dt.Columns.Add("Row", typeof(int));
						}
						else
						{
							foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(getFieldNameFromlstHealth(fieldCheck.FieldName), typeof(int));
								}
							}
						}

						if (IncidencelstHealth != null)
						{
							foreach (FieldCheck fieldCheck in IncidencelstHealth)
							{
								if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
								{
									dt.Columns.Add("Version", typeof(string));

								}
								else if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName, typeof(string));
								}
							}
						}
						if (IncidencelstResult == null)
						{
							dt.Columns.Add("Point Estimate", typeof(double));
							dt.Columns.Add("Population", typeof(double));
							dt.Columns.Add("Delta", typeof(double));
							dt.Columns.Add("Mean", typeof(double));
							dt.Columns.Add("Baseline", typeof(double));
							dt.Columns.Add("Percent Of Baseline", typeof(double));
							dt.Columns.Add("Standard Deviation", typeof(double));
							dt.Columns.Add("Variance", typeof(double));
						}
						else
						{
							foreach (FieldCheck fieldCheck in IncidencelstResult)
							{

								if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName)
								{
									dt.Columns.Add(fieldCheck.FieldName, typeof(double));
								}
							}
						}
						if (lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue != null &&
								lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues != null &&
								lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
						{
							while (i < lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())
							{

								dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
								i++;
							}
						}

						foreach (AllSelectCRFunction cr in lstAllSelectCRFuntion)
						{
							foreach (CRCalculateValue crcv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
							{
								DataRow dr = dt.NewRow();
								if (dt.Columns.Contains("Col")) dr["Col"] = crcv.Col;
								if (dt.Columns.Contains("Row")) dr["Row"] = crcv.Row;
								if (IncidencelstHealth != null)
								{
									foreach (FieldCheck fieldCheck in IncidencelstHealth)
									{
										if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
										{
											dr["Version"] = cr.Version;

										}
										else if (fieldCheck.FieldName.ToLower() == "dataset" && fieldCheck.isChecked)
										{
											dr["DataSet"] = cr.DataSet;
										}
										else if (fieldCheck.FieldName.ToLower() == "geographic area" && fieldCheck.isChecked)
										{
											dr["Geographic Area"] = cr.GeographicArea;
										}
										else if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, cr.CRSelectFunctionCalculateValue.CRSelectFunction);
										}
									}
								}
								if (dt.Columns.Contains("Point Estimate")) dr["Point Estimate"] = crcv.PointEstimate;
								if (dt.Columns.Contains("Population")) dr["Population"] = crcv.Population;
								if (dt.Columns.Contains("Delta")) dr["Delta"] = crcv.Delta;
								if (dt.Columns.Contains("Mean")) dr["Mean"] = crcv.Mean;
								if (dt.Columns.Contains("Baseline")) dr["Baseline"] = crcv.Baseline;
								if (dt.Columns.Contains("Percent Of Baseline")) dr["Percent Of Baseline"] = crcv.PercentOfBaseline;
								if (dt.Columns.Contains("Standard Deviation")) dr["Standard Deviation"] = crcv.StandardDeviation;
								if (dt.Columns.Contains("Variance")) dr["Variance"] = crcv.Variance;
								i = 0;
								if (cr.CRSelectFunctionCalculateValue.CRCalculateValues != null && cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
								{
									while (i < cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())
									{

										dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())))))] = crcv.LstPercentile[i];
										i++;
									}
								}
								dt.Rows.Add(dr);
							}

						}
						if (!chbAllPercentiles.Checked & !isBatch) //BenMAP-284
						{
							List<int> toRemoveColIdx = new List<int>();         //User requests only 2.5 and 97.5--remove other percentile columns
							foreach (DataColumn dc in dt.Columns)
							{
								if (dc.ColumnName.Contains("Percentile") && (dc.ColumnName != "Percentile 2.5" && dc.ColumnName != "Percentile 97.5"))
									toRemoveColIdx.Add(dc.Ordinal);
							}
							for (int idx = toRemoveColIdx.Count; idx > 0; idx--)
							{
								dt.Columns.RemoveAt(toRemoveColIdx[idx - 1]);
							}
							string[] columnNames = dt.Columns.Cast<DataColumn>()
											.Select(x => x.ColumnName)
											.ToArray();
							int meanIdx = Array.FindIndex(columnNames, x => x.Equals("Mean"));
							int firstPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 2.5"));
							int lastPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 97.5"));
							if (firstPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 2.5 results in the selected data");
								return;
							}
							if (lastPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 97.5 results in the selected data");
								return;
							}
							if (meanIdx != -1 && firstPctIdx != -1 && lastPctIdx != -1)
							{
								int numSigDigits = 2;
								double inMean, inPctLow, inPctHigh;
								string fmtPercentile = "#,##0.;(#,##0.)";
								string outString = "";
								dt.Columns.Add("Formatted Results");

								foreach (DataRow dr in dt.Rows)
								{
									try
									{
										inMean = RoundToSignificantDigits(Convert.ToDouble(dr[meanIdx]), numSigDigits);
										inPctLow = RoundToSignificantDigits(Convert.ToDouble(dr[firstPctIdx]), numSigDigits);
										inPctHigh = RoundToSignificantDigits(Convert.ToDouble(dr[lastPctIdx]), numSigDigits);

										outString = inMean.ToString(fmtPercentile) + Environment.NewLine + "(" + inPctLow.ToString(fmtPercentile) + " to " + inPctHigh.ToString(fmtPercentile) + ")";
										dr["Formatted Results"] = outString;
									}
									catch (Exception ex)
									{
										Logger.LogError(ex);
									}
								}
							}

						}
						CommonClass.SaveCSV(dt, _outputFileName);
					}
					//Incidence Pooling (new)
					else if (_tableObject is Dictionary<AllSelectCRFunction, string>)
					{
						Dictionary<AllSelectCRFunction, string> dicAllSelectCRFunctionPoolName = (Dictionary<AllSelectCRFunction, string>)_tableObject;
						if (this.IncidencelstColumnRow == null)
						{
							dt.Columns.Add("Col", typeof(int));
							dt.Columns.Add("Row", typeof(int));
						}
						else
						{
							foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(getFieldNameFromlstHealth(fieldCheck.FieldName), typeof(int));
								}
							}
						}
						if (IncidencelstHealth != null)
						{
							foreach (FieldCheck fieldCheck in IncidencelstHealth)
							{
								if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
								{
									dt.Columns.Add("Version", typeof(string));

								}
								else if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName, typeof(string));
								}
							}
						}

						if (IncidencelstResult == null)
						{
							dt.Columns.Add("Point Estimate", typeof(double));
							dt.Columns.Add("Population", typeof(double));
							dt.Columns.Add("Delta", typeof(double));
							dt.Columns.Add("Mean", typeof(double));
							dt.Columns.Add("Baseline", typeof(double));
							dt.Columns.Add("Percent Of Baseline", typeof(double));
							dt.Columns.Add("Standard Deviation", typeof(double));
							dt.Columns.Add("Variance", typeof(double));
						}
						else
						{
							foreach (FieldCheck fieldCheck in IncidencelstResult)
							{
								if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName)
								{
									dt.Columns.Add(fieldCheck.FieldName, typeof(double));
								}
							}
						}

						CRSelectFunctionCalculateValue tmpFirstCRV = dicAllSelectCRFunctionPoolName.First().Key.CRSelectFunctionCalculateValue;
						if (tmpFirstCRV != null && tmpFirstCRV.CRCalculateValues != null && tmpFirstCRV.CRCalculateValues.First().LstPercentile != null)
						{
							while (i < tmpFirstCRV.CRCalculateValues.First().LstPercentile.Count())
							{
								dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(tmpFirstCRV.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(tmpFirstCRV.CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
								i++;
							}
						}

						if (!dt.Columns.Contains("Pooling Name")) dt.Columns.Add("Pooling Name", typeof(string));
						if (!dt.Columns.Contains("Grid Definition")) dt.Columns.Add("Grid Definition", typeof(string));

						string gridDefinitionName = "";
						if(CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation == null)
						{
							gridDefinitionName = CommonClass.GBenMAPGrid.GridDefinitionName;
						}
						else
						{
							gridDefinitionName = CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionName;
						}


						foreach (KeyValuePair<AllSelectCRFunction, string> keyValuePair in dicAllSelectCRFunctionPoolName)
						{
							AllSelectCRFunction cr = keyValuePair.Key;
							foreach (CRCalculateValue crcv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
							{
								DataRow dr = dt.NewRow();
								if (dt.Columns.Contains("Pooling Name")) dr["Pooling Name"] = keyValuePair.Value;
								if (dt.Columns.Contains("Grid Definition")) dr["Grid Definition"] = gridDefinitionName;
								if (dt.Columns.Contains("Col")) dr["Col"] = crcv.Col;
								if (dt.Columns.Contains("Row")) dr["Row"] = crcv.Row;
								if (IncidencelstHealth != null)
								{
									foreach (FieldCheck fieldCheck in IncidencelstHealth)
									{
										if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
										{
											dr["Version"] = cr.Version;

										}
										else if (fieldCheck.FieldName.ToLower() == "dataset" && fieldCheck.isChecked)
										{
											dr["DataSet"] = cr.DataSet;
										}
										else if (fieldCheck.FieldName.ToLower() == "geographic area" && fieldCheck.isChecked)
										{
											dr["Geographic Area"] = cr.GeographicArea;
										}
										else if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, cr.CRSelectFunctionCalculateValue.CRSelectFunction);
										}
									}
								}
								if (dt.Columns.Contains("Point Estimate")) dr["Point Estimate"] = crcv.PointEstimate;
								if (dt.Columns.Contains("Population")) dr["Population"] = crcv.Population;
								if (dt.Columns.Contains("Delta")) dr["Delta"] = crcv.Delta;
								if (dt.Columns.Contains("Mean")) dr["Mean"] = crcv.Mean;
								if (dt.Columns.Contains("Baseline")) dr["Baseline"] = crcv.Baseline;
								if (dt.Columns.Contains("Percent Of Baseline")) dr["Percent Of Baseline"] = crcv.PercentOfBaseline;
								if (dt.Columns.Contains("Standard Deviation")) dr["Standard Deviation"] = crcv.StandardDeviation;
								if (dt.Columns.Contains("Variance")) dr["Variance"] = crcv.Variance;
								i = 0;
								if (cr.CRSelectFunctionCalculateValue.CRCalculateValues != null && cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
								{
									while (i < cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())
									{

										dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())))))] = crcv.LstPercentile[i];
										i++;
									}
								}
								dt.Rows.Add(dr);
							}
						}
						if (!chbAllPercentiles.Checked & !isBatch) //BenMAP-284
						{
							List<int> toRemoveColIdx = new List<int>();         //User requests only 2.5 and 97.5--remove other percentile columns

							foreach (DataColumn dc in dt.Columns)
							{
								if (dc.ColumnName.Contains("Percentile") && (dc.ColumnName != "Percentile 2.5" && dc.ColumnName != "Percentile 97.5"))
									toRemoveColIdx.Add(dc.Ordinal);
							}

							for (int idx = toRemoveColIdx.Count; idx > 0; idx--)
							{
								dt.Columns.RemoveAt(toRemoveColIdx[idx - 1]);
							}

							string[] columnNames = dt.Columns.Cast<DataColumn>()
															.Select(x => x.ColumnName)
															.ToArray();

							int meanIdx = Array.FindIndex(columnNames, x => x.Equals("Mean"));
							int firstPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 2.5"));
							int lastPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 97.5"));

							if (firstPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 2.5 results in the selected data");
								return;
							}
							if (lastPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 97.5 results in the selected data");
								return;
							}

							if (meanIdx != -1 && firstPctIdx != -1 && lastPctIdx != -1)
							{
								int numSigDigits = 2;
								double inMean, inPctLow, inPctHigh;
								string fmtPercentile = "#,##0.;(#,##0.)";
								string outString = "";
								dt.Columns.Add("Formatted Results");

								foreach (DataRow dr in dt.Rows)
								{
									try
									{
										inMean = RoundToSignificantDigits(Convert.ToDouble(dr[meanIdx]), numSigDigits);
										inPctLow = RoundToSignificantDigits(Convert.ToDouble(dr[firstPctIdx]), numSigDigits);
										inPctHigh = RoundToSignificantDigits(Convert.ToDouble(dr[lastPctIdx]), numSigDigits);

										outString = inMean.ToString(fmtPercentile) + Environment.NewLine + "(" + inPctLow.ToString(fmtPercentile) + " to " + inPctHigh.ToString(fmtPercentile) + ")";
										dr["Formatted Results"] = outString;
									}
									catch (Exception ex)
									{
										Logger.LogError(ex);
									}
								}
							}
						}
						CommonClass.SaveCSV(dt, _outputFileName);
					}
					//One HIF result
					else if (_tableObject is CRSelectFunctionCalculateValue)
					{
						CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)_tableObject;
						dt.Columns.Add("Col", typeof(int));
						dt.Columns.Add("Row", typeof(int));
						dt.Columns.Add("Point Estimate", typeof(double));
						dt.Columns.Add("Population", typeof(double));
						dt.Columns.Add("Delta", typeof(double));
						dt.Columns.Add("Mean", typeof(double));
						dt.Columns.Add("Baseline", typeof(double));
						dt.Columns.Add("Percent Of Baseline", typeof(double));
						dt.Columns.Add("Standard Deviation", typeof(double));
						dt.Columns.Add("Variance", typeof(double));
						i = 0;
						while (i < crTable.CRCalculateValues.First().LstPercentile.Count())
						{

							dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
							i++;
						}

						foreach (CRCalculateValue crcv in crTable.CRCalculateValues)
						{
							DataRow dr = dt.NewRow();
							dr["Col"] = crcv.Col;
							dr["Row"] = crcv.Row;
							dr["Point Estimate"] = crcv.PointEstimate;
							dr["Population"] = crcv.Population;
							dr["Delta"] = crcv.Delta;
							dr["Mean"] = crcv.Mean;
							dr["Baseline"] = crcv.Baseline;
							dr["Percent Of Baseline"] = crcv.PercentOfBaseline;
							dr["Standard Deviation"] = crcv.StandardDeviation;
							dr["Variance"] = crcv.Variance;
							i = 0;
							if (crTable.CRCalculateValues != null && crTable.CRCalculateValues.First().LstPercentile != null)
							{
								while (i < crTable.CRCalculateValues.First().LstPercentile.Count())
								{

									dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count())))))] = crcv.LstPercentile[i];
									i++;
								}
							}
							dt.Rows.Add(dr);
						}
						CommonClass.SaveCSV(dt, _outputFileName);
					}
					//???
					else if (_tableObject is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)
					{
						Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = _tableObject as Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>;
						if (cflstColumnRow == null)
						{
							dt.Columns.Add("Col", typeof(int));
							dt.Columns.Add("Row", typeof(int));
						}
						else
						{
							foreach (FieldCheck fieldCheck in cflstColumnRow)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}
						}
						if (cflstHealth != null)
						{
							foreach (FieldCheck fieldCheck in cflstHealth)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}
						}
						if (cflstResult == null)
						{

							dt.Columns.Add("Point Estimate", typeof(double));
							dt.Columns.Add("Population", typeof(double));
							dt.Columns.Add("Delta", typeof(double));
							dt.Columns.Add("Mean", typeof(double));
							dt.Columns.Add("Baseline", typeof(double));
							dt.Columns.Add("Percent Of Baseline", typeof(double));
							dt.Columns.Add("Standard Deviation", typeof(double));
							dt.Columns.Add("Variance", typeof(double));
						}
						else
						{
							foreach (FieldCheck fieldCheck in cflstResult)
							{

								if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
										&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}
						}
						if (dicAPV.First().Key.Key.LstPercentile != null)
						{
							i = 0;
							while (i < dicAPV.First().Key.Key.LstPercentile.Count())
							{
								if (cflstResult == null || cflstResult.Last().isChecked)
								{
									dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(dicAPV.First().Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(dicAPV.First().Key.Key.LstPercentile.Count()))))), typeof(double));
								}
								i++;
							}
						}
						foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> k in dicAPV)
						{
							CRCalculateValue crcv = k.Key.Key;

							DataRow dr = dt.NewRow();
							if (cflstColumnRow == null)
							{
								dr["Col"] = crcv.Col;
								dr["Row"] = crcv.Row;
							}
							else
							{
								foreach (FieldCheck fieldCheck in cflstColumnRow)
								{

									if (fieldCheck.isChecked)
									{
										dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, k.Value);
									}
								}
							}
							if (cflstHealth != null)
							{
								foreach (FieldCheck fieldCheck in cflstHealth)
								{

									if (fieldCheck.isChecked)
									{
										dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, k.Value);
									}
								}
							}
							if (cflstResult == null)
							{

								dr["Point Estimate"] = crcv.PointEstimate;
								dr["Population"] = crcv.Population;
								dr["Delta"] = crcv.Delta;
								dr["Mean"] = crcv.Mean;
								dr["Baseline"] = crcv.Baseline;
								dr["Percent of Baseline"] = crcv.PercentOfBaseline;
								dr["Standard Deviation"] = crcv.StandardDeviation;
								dr["Variance"] = crcv.Variance;
							}
							else
							{
								foreach (FieldCheck fieldCheck in cflstResult)
								{

									if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
									&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
									{
										dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, k.Value);
									}
								}
							}


							i = 0;
							if ((cflstResult == null || cflstResult.Last().isChecked) && crcv.LstPercentile != null)
							{
								while (i < crcv.LstPercentile.Count())
								{

									dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crcv.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crcv.LstPercentile.Count())))))] = crcv.LstPercentile[i];
									i++;
								}
							}


							dt.Rows.Add(dr);
						}

						CommonClass.SaveCSV(dt, _outputFileName);
					}
					//List of HIF result
					else if (_tableObject is List<CRSelectFunctionCalculateValue>)
					{
						List<CRSelectFunctionCalculateValue> lstCRTable = (List<CRSelectFunctionCalculateValue>)_tableObject;
						if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
						{
							if (IncidencelstColumnRow == null)
							{
								dt.Columns.Add("Col", typeof(int));
								dt.Columns.Add("Row", typeof(int));
							}
							else
							{
								foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
								{

									if (fieldCheck.isChecked)
									{
										dt.Columns.Add(fieldCheck.FieldName);
									}
								}
							}
							if (IncidencelstHealth != null)
							{
								foreach (FieldCheck fieldCheck in IncidencelstHealth)
								{

									if (fieldCheck.isChecked)
									{
										dt.Columns.Add(fieldCheck.FieldName);
									}
								}
							}
							if (IncidencelstResult == null)
							{

								dt.Columns.Add("Point Estimate", typeof(double));
								dt.Columns.Add("Population", typeof(double));
								dt.Columns.Add("Delta", typeof(double));
								dt.Columns.Add("Mean", typeof(double));
								dt.Columns.Add("Baseline", typeof(double));
								dt.Columns.Add("Percent of Baseline", typeof(double));
								dt.Columns.Add("Standard Deviation", typeof(double));
								dt.Columns.Add("Variance", typeof(double));
							}
							else
							{
								foreach (FieldCheck fieldCheck in IncidencelstResult)
								{

									if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
											&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
									{
										dt.Columns.Add(fieldCheck.FieldName);
									}
								}
							}

							if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
							{
								i = 0;
								while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
								{
									if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
									{
										dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
									}
									i++;
								}
							}
						}
						else
						{
							if (cflstColumnRow == null)
							{
								dt.Columns.Add("Col", typeof(int));
								dt.Columns.Add("Row", typeof(int));
							}
							else
							{
								foreach (FieldCheck fieldCheck in cflstColumnRow)
								{

									if (fieldCheck.isChecked)
									{
										dt.Columns.Add(fieldCheck.FieldName);
									}
								}
							}
							if (cflstHealth != null)
							{
								foreach (FieldCheck fieldCheck in cflstHealth)
								{

									if (fieldCheck.isChecked)
									{
										dt.Columns.Add(fieldCheck.FieldName);
									}
								}
							}
							if (cflstResult == null)
							{

								dt.Columns.Add("Point Estimate", typeof(double));
								dt.Columns.Add("Population", typeof(double));
								dt.Columns.Add("Delta", typeof(double));
								dt.Columns.Add("Mean", typeof(double));
								dt.Columns.Add("Baseline", typeof(double));
								dt.Columns.Add("Percent of Baseline", typeof(double));
								dt.Columns.Add("Standard Deviation", typeof(double));
								dt.Columns.Add("Variance", typeof(double));
							}
							else
							{
								foreach (FieldCheck fieldCheck in cflstResult)
								{

									if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
											&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
									{
										dt.Columns.Add(fieldCheck.FieldName);
									}
								}
							}

							if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
							{
								i = 0;
								while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
								{
									if (cflstResult == null || cflstResult.Last().isChecked)
									{
										dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
									}
									i++;
								}
							}
						}
						Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();
						if (!isBatch && cflstResult != null && cflstResult.Where(p => p.FieldName == "Population Weighted Delta").Count() == 1 && this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() != "incidence")
						{
							if (cflstResult.Where(p => p.FieldName == "Population Weighted Delta").First().isChecked == true)
							{
								foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
								{

									CRCalculateValue crv = new CRCalculateValue();
									crv.PointEstimate = cr.CRCalculateValues.Sum(p => p.Population * p.Delta) / cr.CRCalculateValues.Sum(p => p.Population);
									if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
									{
										crv.LstPercentile = new List<float>();
										foreach (float f in cr.CRCalculateValues.First().LstPercentile)
										{
											crv.LstPercentile.Add(0);
										}
									}

									CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
									crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Delta";
									Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
									dicKey.Add(crv, dicAPV.Count);
									dicAPV.Add(dicKey.First(), crNew);
								}
							}
							if (cflstResult.Where(p => p.FieldName == "Population Weighted Base").First().isChecked == true)
							{
								foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
								{

									CRCalculateValue crv = new CRCalculateValue();
									CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
									BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Base;
									Dictionary<string, float> dicBase = new Dictionary<string, float>();
									string strMetric = "";
									if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
									{
										strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
									}
									else
										strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
									foreach (ModelResultAttribute mr in benMAPLineBase.ModelResultAttributes)
									{
										dicBase.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

									}
									float dPointEstimate = 0;
									foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
									{
										if (dicBase.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
										{
											dPointEstimate = dPointEstimate + dicBase[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
										}
									}
									crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
									if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
									{
										crv.LstPercentile = new List<float>();
										foreach (float f in cr.CRCalculateValues.First().LstPercentile)
										{
											crv.LstPercentile.Add(0);
										}
									}


									crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Base";
									Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
									dicKey.Add(crv, dicAPV.Count);
									dicAPV.Add(dicKey.First(), crNew);
								}
							}
							if (cflstResult.Where(p => p.FieldName == "Population Weighted Control").First().isChecked == true)
							{
								foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
								{

									CRCalculateValue crv = new CRCalculateValue();
									CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
									BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Control;
									Dictionary<string, float> dicControl = new Dictionary<string, float>();
									string strMetric = "";
									if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
									{
										strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
									}
									else
										strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
									foreach (ModelResultAttribute mr in benMAPLineControl.ModelResultAttributes)
									{
										dicControl.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

									}
									float dPointEstimate = 0;
									foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
									{
										if (dicControl.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
										{
											dPointEstimate = dPointEstimate + dicControl[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
										}
									}
									crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
									if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
									{
										crv.LstPercentile = new List<float>();
										foreach (float f in cr.CRCalculateValues.First().LstPercentile)
										{
											crv.LstPercentile.Add(0);
										}
									}


									crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Control";
									Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
									dicKey.Add(crv, dicAPV.Count);
									dicAPV.Add(dicKey.First(), crNew);
								}
							}
						}

						foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
						{
							foreach (CRCalculateValue crcv in cr.CRCalculateValues)
							{
								Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
								dicKey.Add(crcv, dicAPV.Count);
								dicAPV.Add(dicKey.First(), cr.CRSelectFunction);

							}
						}
						foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> k in dicAPV)
						{

							DataRow dr = dt.NewRow();
							if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
							{
								if (IncidencelstColumnRow == null)
								{
									dr["Col"] = k.Key.Key.Col;
									dr["Row"] = k.Key.Key.Row;
								}
								else
								{
									foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
										}
									}
								}
								if (IncidencelstHealth != null)
								{
									foreach (FieldCheck fieldCheck in IncidencelstHealth)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
										}
									}
								}
								if (IncidencelstResult == null)
								{

									dr["Point Estimate"] = k.Key.Key.PointEstimate;
									dr["Population"] = k.Key.Key.Population;
									dr["Delta"] = k.Key.Key.Delta;
									dr["Mean"] = k.Key.Key.Mean;
									dr["Baseline"] = k.Key.Key.Baseline;
									dr["Percent Of Baseline"] = k.Key.Key.PercentOfBaseline;
									dr["Standard Deviation"] = k.Key.Key.StandardDeviation;
									dr["Variance"] = k.Key.Key.Variance;
								}
								else
								{
									foreach (FieldCheck fieldCheck in IncidencelstResult)
									{

										if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
								&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
										}
									}
								}


								i = 0;
								if ((IncidencelstResult == null || IncidencelstResult.Last().isChecked) && k.Key.Key.LstPercentile != null)
								{
									while (i < k.Key.Key.LstPercentile.Count())
									{

										dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(k.Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(k.Key.Key.LstPercentile.Count())))))] = k.Key.Key.LstPercentile[i];
										i++;
									}
								}
							}
							else
							{
								if (cflstColumnRow == null)
								{
									dr["Col"] = k.Key.Key.Col;
									dr["Row"] = k.Key.Key.Row;
								}
								else
								{
									foreach (FieldCheck fieldCheck in cflstColumnRow)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
										}
									}
								}
								if (cflstHealth != null)
								{
									foreach (FieldCheck fieldCheck in cflstHealth)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
										}
									}
								}
								if (cflstResult == null)
								{

									dr["Point Estimate"] = k.Key.Key.PointEstimate;
									dr["Population"] = k.Key.Key.Population;
									dr["Delta"] = k.Key.Key.Delta;
									dr["Mean"] = k.Key.Key.Mean;
									dr["Baseline"] = k.Key.Key.Baseline;
									dr["Percent Of Baseline"] = k.Key.Key.PercentOfBaseline;
									dr["Standard Deviation"] = k.Key.Key.StandardDeviation;
									dr["Variance"] = k.Key.Key.Variance;
								}
								else
								{
									foreach (FieldCheck fieldCheck in cflstResult)
									{

										if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
								&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
										}
									}
								}


								i = 0;
								if ((cflstResult == null || cflstResult.Last().isChecked) && k.Key.Key.LstPercentile != null)
								{
									while (i < k.Key.Key.LstPercentile.Count())
									{

										dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(k.Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(k.Key.Key.LstPercentile.Count())))))] = k.Key.Key.LstPercentile[i];
										i++;
									}
								}

							}

							dt.Rows.Add(dr);

						}
						if (!chbAllPercentiles.Checked & !isBatch) //BenMAP-284 
						{
							List<int> toRemoveColIdx = new List<int>();         //User requests only 2.5 and 97.5--remove other percentile columns

							foreach (DataColumn dc in dt.Columns)
							{
								if (dc.ColumnName.Contains("Percentile") && (dc.ColumnName != "Percentile 2.5" && dc.ColumnName != "Percentile 97.5"))
									toRemoveColIdx.Add(dc.Ordinal);
							}

							for (int idx = toRemoveColIdx.Count; idx > 0; idx--)
							{
								dt.Columns.RemoveAt(toRemoveColIdx[idx - 1]);
							}

							string[] columnNames = dt.Columns.Cast<DataColumn>()
									 .Select(x => x.ColumnName)
									 .ToArray();

							int meanIdx = Array.FindIndex(columnNames, x => x.Equals("Mean"));          //Locate the columns with mean, 2.5 and 97.5
							int firstPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 2.5"));
							int lastPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 97.5"));

							if (firstPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 2.5 results in the selected data");
								return;
							}
							if (lastPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 97.5 results in the selected data");
								return;
							}

							if (meanIdx != -1 && firstPctIdx != -1 && lastPctIdx != -1)             //Apply EPA formatting to the located values
							{
								int numSigDigits = 2;
								double inMean, inPctLow, inPctHigh;
								string fmtPercentile = "#,##0.;(#,##0.)";
								string outString = "";
								dt.Columns.Add("Formatted Results");

								foreach (DataRow dr in dt.Rows)
								{
									try
									{
										inMean = RoundToSignificantDigits(Convert.ToDouble(dr[meanIdx]), numSigDigits);
										inPctLow = RoundToSignificantDigits(Convert.ToDouble(dr[firstPctIdx]), numSigDigits);
										inPctHigh = RoundToSignificantDigits(Convert.ToDouble(dr[lastPctIdx]), numSigDigits);

										outString = inMean.ToString(fmtPercentile) + Environment.NewLine + "(" + inPctLow.ToString(fmtPercentile) + " to " + inPctHigh.ToString(fmtPercentile) + ")";
										dr["Formatted Results"] = outString;
									}
									catch (Exception ex)
									{
										Logger.LogError(ex);
									}
								}
							}
						}
						CommonClass.SaveCSV(dt, _outputFileName);

					}
					//AQ
					else if (_tableObject is BenMAPLine)
					{
						BenMAPLine crTable = (BenMAPLine)_tableObject;
						DataSourceCommonClass.SaveModelDataLineToNewFormatCSV(crTable, _outputFileName);
					}
					//Valuation Pooling
					else if (_tableObject is List<AllSelectValuationMethodAndValue> || _tableObject is AllSelectValuationMethodAndValue)
					{
						List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
						if (_tableObject is List<AllSelectValuationMethodAndValue>)
						{
							lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)_tableObject;
						}
						else
						{
							lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)_tableObject);
						}
						if (apvlstColumnRow == null)
						{
							dt.Columns.Add("Col", typeof(int));
							dt.Columns.Add("Row", typeof(int));
						}
						else
						{
							foreach (FieldCheck fieldCheck in apvlstColumnRow)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}

						}
						if (apvlstHealth != null)
						{
							foreach (FieldCheck fieldCheck in apvlstHealth)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}

						}
						if (apvlstResult == null)
						{
							dt.Columns.Add("Point Estimate", typeof(double));

							dt.Columns.Add("Mean", typeof(double));

							dt.Columns.Add("Standard Deviation", typeof(double));
							dt.Columns.Add("Variance", typeof(double));
						}
						else
						{
							foreach (FieldCheck fieldCheck in apvlstResult)
							{

								if (fieldCheck.FieldName != apvlstResult.Last().FieldName && fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}

						}
						if ((apvlstResult == null || apvlstResult.Last().isChecked) && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
						{
							i = 0;
							while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
							{

								dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()))))), typeof(double));
								i++;
							}
						}


						foreach (AllSelectValuationMethodAndValue allSelectValuationMethodAndValue in lstallSelectValuationMethodAndValue)
						{
							foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
							{
								DataRow dr = dt.NewRow();
								if (apvlstColumnRow == null)
								{
									dr["Col"] = apvx.Col;
									dr["Row"] = apvx.Row;
								}
								else
								{
									foreach (FieldCheck fieldCheck in apvlstColumnRow)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, allSelectValuationMethodAndValue.AllSelectValuationMethod, apvx);
										}
									}

								}
								if (apvlstHealth != null)
								{
									foreach (FieldCheck fieldCheck in apvlstHealth)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, allSelectValuationMethodAndValue.AllSelectValuationMethod, apvx);
										}
									}

								}
								if (apvlstResult == null)
								{

									dr["Point Estimate"] = apvx.PointEstimate;
									dr["Mean"] = apvx.Mean;
									dr["Standard Deviation"] = apvx.StandardDeviation;
									dr["Variance"] = apvx.Variance;

								}
								else
								{
									foreach (FieldCheck fieldCheck in apvlstResult)
									{

										if (fieldCheck.FieldName != apvlstResult.Last().FieldName && fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, allSelectValuationMethodAndValue.AllSelectValuationMethod, apvx);
										}
									}

								}
								if ((apvlstResult == null || apvlstResult.Last().isChecked) && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
								{
									i = 0;
									while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
									{
										dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
										i++;
									}
								}


								dt.Rows.Add(dr);
							}
						}
						if (!chbAllPercentiles.Checked && !isBatch) //BenMAP-284
						{
							List<int> toRemoveColIdx = new List<int>();         //User requests only 2.5 and 97.5--remove other percentile columns

							foreach (DataColumn dc in dt.Columns)
							{
								if (dc.ColumnName.Contains("Percentile") && (dc.ColumnName != "Percentile 2.5" && dc.ColumnName != "Percentile 97.5"))
									toRemoveColIdx.Add(dc.Ordinal);
							}

							for (int idx = toRemoveColIdx.Count; idx > 0; idx--)
							{
								dt.Columns.RemoveAt(toRemoveColIdx[idx - 1]);
							}

							string[] columnNames = dt.Columns.Cast<DataColumn>()
							 .Select(x => x.ColumnName)
							 .ToArray();

							int meanIdx = Array.FindIndex(columnNames, x => x.Equals("Mean"));
							int firstPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 2.5"));
							int lastPctIdx = Array.FindIndex(columnNames, x => x.Contains("Percentile 97.5"));

							if (firstPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 2.5 results in the selected data");
								return;
							}
							if (lastPctIdx == -1)
							{
								MessageBox.Show("Unable to locate Percentile 97.5 results in the selected data");
								return;
							}

							if (meanIdx != -1 && firstPctIdx != -1 && lastPctIdx != -1)
							{
								int numSigDigits = 2;
								double inMean, inPctLow, inPctHigh;
								string fmtPercentile = "$#,##0.;$(#,##0.)";
								string outString = "";
								dt.Columns.Add("Formatted Results");

								foreach (DataRow dr in dt.Rows)
								{
									try
									{
										inMean = RoundToSignificantDigits(Convert.ToDouble(dr[meanIdx]), numSigDigits);
										inPctLow = RoundToSignificantDigits(Convert.ToDouble(dr[firstPctIdx]), numSigDigits);
										inPctHigh = RoundToSignificantDigits(Convert.ToDouble(dr[lastPctIdx]), numSigDigits);

										outString = inMean.ToString(fmtPercentile) + Environment.NewLine + "(" + inPctLow.ToString(fmtPercentile) + " to " + inPctHigh.ToString(fmtPercentile) + ")";
										dr["Formatted Results"] = outString;
									}
									catch (Exception ex)
									{
										Logger.LogError(ex);
									}
								}
							}
						}
						CommonClass.SaveCSV(dt, _outputFileName);

					}
					//QALY
					else if (_tableObject is List<AllSelectQALYMethodAndValue> || _tableObject is AllSelectQALYMethodAndValue)
					{
						List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
						if (_tableObject is List<AllSelectQALYMethodAndValue>)
						{
							lstAllSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)_tableObject;
						}
						else
						{
							lstAllSelectQALYMethodAndValue.Add((AllSelectQALYMethodAndValue)_tableObject);
						}
						if (qalylstColumnRow == null)
						{
							dt.Columns.Add("Col", typeof(int));
							dt.Columns.Add("Row", typeof(int));
						}
						else
						{
							foreach (FieldCheck fieldCheck in qalylstColumnRow)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}

						}
						if (qalylstHealth != null)
						{
							foreach (FieldCheck fieldCheck in qalylstHealth)
							{

								if (fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}

						}
						if (qalylstResult == null)
						{
							dt.Columns.Add("Point Estimate", typeof(double));

							dt.Columns.Add("Mean", typeof(double));

							dt.Columns.Add("Standard Deviation", typeof(double));
							dt.Columns.Add("Variance", typeof(double));
						}
						else
						{
							foreach (FieldCheck fieldCheck in qalylstResult)
							{

								if (fieldCheck.FieldName != qalylstResult.Last().FieldName && fieldCheck.isChecked)
								{
									dt.Columns.Add(fieldCheck.FieldName);
								}
							}

						}
						if ((qalylstResult == null || qalylstResult.Last().isChecked) && lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
						{
							i = 0;
							while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
							{

								dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));
								i++;
							}
						}


						foreach (AllSelectQALYMethodAndValue AllSelectQALYMethodAndValue in lstAllSelectQALYMethodAndValue)
						{
							foreach (QALYValueAttribute apvx in AllSelectQALYMethodAndValue.lstQALYValueAttributes)
							{
								DataRow dr = dt.NewRow();
								if (qalylstColumnRow == null)
								{
									dr["Col"] = apvx.Col;
									dr["Row"] = apvx.Row;
								}
								else
								{
									foreach (FieldCheck fieldCheck in qalylstColumnRow)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstQALYObject(fieldCheck.FieldName, AllSelectQALYMethodAndValue.AllSelectQALYMethod, apvx);
										}
									}

								}
								if (qalylstHealth != null)
								{
									foreach (FieldCheck fieldCheck in qalylstHealth)
									{

										if (fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstQALYObject(fieldCheck.FieldName, AllSelectQALYMethodAndValue.AllSelectQALYMethod, apvx);
										}
									}

								}
								if (qalylstResult == null)
								{

									dr["Point Estimate"] = apvx.PointEstimate;
									dr["Mean"] = apvx.Mean;
									dr["Standard Deviation"] = apvx.StandardDeviation;
									dr["Variance"] = apvx.Variance;

								}
								else
								{
									foreach (FieldCheck fieldCheck in qalylstResult)
									{

										if (fieldCheck.FieldName != qalylstResult.Last().FieldName && fieldCheck.isChecked)
										{
											dr[fieldCheck.FieldName] = getFieldNameFromlstQALYObject(fieldCheck.FieldName, AllSelectQALYMethodAndValue.AllSelectQALYMethod, apvx);
										}
									}

								}
								if ((qalylstResult == null || qalylstResult.Last().isChecked) && lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
								{
									i = 0;
									while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
									{
										dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
										i++;
									}
								}


								dt.Rows.Add(dr);
							}
						}
						CommonClass.SaveCSV(dt, _outputFileName);

					}
					else if (_tableObject is AllSelectQALYMethodAndValue)
					{
						AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = (AllSelectQALYMethodAndValue)_tableObject;
						dt.Columns.Add("Col", typeof(int));
						dt.Columns.Add("Row", typeof(int));
						dt.Columns.Add("Point Estimate", typeof(double));

						dt.Columns.Add("Mean", typeof(double));

						dt.Columns.Add("Standard Deviation", typeof(double));
						dt.Columns.Add("Variance", typeof(double));

						i = 0;
						while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
						{
							dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));
							i++;
						}

						dt.Columns.Add("Name", typeof(string));
						dt.Columns.Add("Pooling Method", typeof(string));
						dt.Columns.Add("Qualifier", typeof(string));
						dt.Columns.Add("Start Age", typeof(string));
						dt.Columns.Add("End Age", typeof(string));


						foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
						{
							DataRow dr = dt.NewRow();
							dr["Col"] = apvx.Col;
							dr["Row"] = apvx.Row;
							dr["Point Estimate"] = apvx.PointEstimate;

							dr["Mean"] = apvx.Mean;

							dr["Standard Deviation"] = apvx.StandardDeviation;
							dr["Variance"] = apvx.Variance;

							dr["Name"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Name;
							dr["Pooling Method"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.PoolingMethod;
							dr["Qualifier"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Qualifier;
							dr["Start Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.StartAge;
							dr["End Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.EndAge;

							i = 0;
							while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
							{

								dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
								i++;
							}
							dt.Rows.Add(dr);

						}
						CommonClass.SaveCSV(dt, _outputFileName);

					}
					else if (_tableObject is List<AllSelectQALYMethodAndValue>)
					{
						List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)_tableObject;
						dt.Columns.Add("Col", typeof(int));
						dt.Columns.Add("Row", typeof(int));
						dt.Columns.Add("Point Estimate", typeof(double));

						dt.Columns.Add("Mean", typeof(double));

						dt.Columns.Add("Standard Deviation", typeof(double));
						dt.Columns.Add("Variance", typeof(double));

						i = 0;
						while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
						{
							dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));
							i++;
						}

						dt.Columns.Add("Name", typeof(string));
						dt.Columns.Add("Pooling Method", typeof(string));
						dt.Columns.Add("Qualifier", typeof(string));
						dt.Columns.Add("Start Age", typeof(string));
						dt.Columns.Add("End Age", typeof(string));

						foreach (AllSelectQALYMethodAndValue allSelectQALYMethodAndValue in lstAllSelectQALYMethodAndValue)
						{
							foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
							{
								DataRow dr = dt.NewRow();
								dr["Col"] = apvx.Col;
								dr["Row"] = apvx.Row;
								dr["Point Estimate"] = apvx.PointEstimate;

								dr["Mean"] = apvx.Mean;

								dr["Standard Deviation"] = apvx.StandardDeviation;
								dr["Variance"] = apvx.Variance;

								dr["Name"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Name;
								dr["Pooling Method"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.PoolingMethod;
								dr["Qualifier"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Qualifier;
								dr["Start Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.StartAge;
								dr["End Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.EndAge;

								i = 0;
								while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
								{

									dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
									i++;
								}
								dt.Rows.Add(dr);

							}
						}
						CommonClass.SaveCSV(dt, _outputFileName);

					}
				}
				catch (Exception ex)
				{
					Logger.LogError(ex);
				}
			}
		}


		//<<<<<<< HEAD
		//=======
		//>>>>>>> develop


		//<<<<<<< HEAD
		//=======
		//>>>>>>> develop


		//<<<<<<< HEAD
		//=======
		//>>>>>>> develop


		private void btnSaveChart_Click(object sender, EventArgs e)
		{
			try
			{
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void pnlRSM_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void ListviewSelectionChange(ListView list)
		{
			foreach (ListViewItem item in list.Items)
			{
				if (item.Selected)
				{
					item.BackColor = Color.LightBlue;
					item.ForeColor = Color.Red;
					list.Refresh();
				}
				else
				{
					item.BackColor = Color.Transparent;
				}
			}

		}

		private void btnRawAudit_Click(object sender, EventArgs e)
		{

			if (mainMap.GetAllLayers().Count == 0)
			{
				MessageBox.Show("Please set up necessary data.", "Error", MessageBoxButtons.OK);
				return;
			}
		}

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

		public void WaitClose()
		{
			try
			{
				if (waitMess.InvokeRequired)
					waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
				else
					DoCloseJob();
			}
			catch (Exception ex)
			{ }
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

		public void CopyDir(string srcPath, string aimPath)
		{
			try
			{
				if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
					aimPath += Path.DirectorySeparatorChar;

				if (!Directory.Exists(aimPath))
					Directory.CreateDirectory(aimPath);


				string[] fileList = Directory.GetFileSystemEntries(srcPath);

				foreach (string file in fileList)
				{
					if (Directory.Exists(file))
					{
						CopyDir(file, aimPath + Path.GetFileName(file));
					}
					else
					{
						File.Copy(file, aimPath + Path.GetFileName(file), true);
					}
				}
			}
			catch
			{
			}
		}

		public void InitAggregationAndRegionList()
		{
			try
			{
				System.Data.DataSet dsCRAggregationGridType = BindGridtype();
				cbCRAggregation.DataSource = dsCRAggregationGridType.Tables[0];
				cbCRAggregation.DisplayMember = "GridDefinitionName";
				cbCRAggregation.SelectedIndex = 0;
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				string commandText = string.Format("select distinct GridDefinitionID,GridDefinitionName from GridDefinitions where columns<=56 and setupid={0}  order by GridDefinitionName desc", CommonClass.MainSetup.SetupID);
				System.Data.DataSet dsRegion = BindGridtypeDomain(); cboRegion.DisplayMember = "GridDefinitionName";
				cboRegion.DataSource = dsRegion.Tables[0];

				for (int i = 0; i < dsRegion.Tables[0].Rows.Count; i++)
				{
					if (CommonClass.rBenMAPGrid != null && CommonClass.rBenMAPGrid.GridDefinitionID == Convert.ToInt32(dsRegion.Tables[0].Rows[i]["GridDefinitionID"]))
					{
						cboRegion.SelectedIndex = i;
						break;
					}
				}
			}
			catch
			{

			}
		}

		public void loadHomePageFunction()
		{
			switch (_homePageName)
			{
				case "picCreateProject":
					break;
				case "picLoadProject":
					OpenProject();
					break;
				case "picLoadCFG":
					trvSetting.SelectedNode = trvSetting.Nodes[1];
					trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[1], null);
					break;
				case "picLoadCFGR":
					trvSetting.SelectedNode = trvSetting.Nodes[1];
					trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[1], null);
					break;
				case "picLoadAPV":
					trvSetting.SelectedNode = trvSetting.Nodes[2];
					trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[2], null);
					break;
				case "picLoadAPVR":
					trvSetting.SelectedNode = trvSetting.Nodes[2];
					trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[2], null);
					break;
			}
		}

		public void loadInputParamProject()
		{
			try
			{
				if (CommonClass.InputParams != null && CommonClass.InputParams.Length > 0 && CommonClass.InputParams[0].ToLower().IndexOf("ctlx") > 0)
				{


				}
				else if (CommonClass.InputParams != null && CommonClass.InputParams.Length > 0 && CommonClass.InputParams[0].ToLower().IndexOf("projx") > 0)
				{
					splitContainer1.Visible = true;
					CommonClass.ClearAllObject();
					_MapAlreadyDisplayed = false;
					ClearAll();
					ResetParamsTree("");

					ClearMapTableChart();
					initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

					initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
					initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
					CommonClass.IncidencePoolingAndAggregationAdvance = null;
					if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
					{
						changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);

					}
					olvCRFunctionResult.SetObjects(null);
					olvIncidence.SetObjects(null);
					tlvAPVResult.SetObjects(null);

					cbPoolingWindowIncidence.Items.Clear();
					cbPoolingWindowAPV.Items.Clear();
					ClearMapTableChart();
					SetTabControl(tabCtlReport);
					CommonClass.LstPollutant = null; CommonClass.RBenMAPGrid = null;
					CommonClass.GBenMAPGrid = null; CommonClass.LstBaseControlGroup = null; CommonClass.CRThreshold = 0; CommonClass.CRLatinHypercubePoints = 20; CommonClass.CRRunInPointMode = false;
					CommonClass.BenMAPPopulation = null;
					CommonClass.BaseControlCRSelectFunction = null; CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
					CommonClass.lstIncidencePoolingAndAggregation = null;

					CommonClass.IncidencePoolingResult = null;
					CommonClass.ValuationMethodPoolingAndAggregation = null;
					CommonClass.BaseControlCRSelectFunction = null;
					CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
					CommonClass.ValuationMethodPoolingAndAggregation = null;
					GC.Collect();
					CommonClass.LoadBenMAPProject(CommonClass.InputParams[0]);
					CommonClass.InputParams = null;
					if (CommonClass.ValuationMethodPoolingAndAggregation != null)
					{
						CommonClass.lstIncidencePoolingAndAggregation = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList();
						CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
						cbPoolingWindowAPV.Items.Clear();
						CommonClass.BaseControlCRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
						CommonClass.BaseControlCRSelectFunction = null;
						CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
						CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
						CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
						CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
						CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
						CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
						CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
						CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
						CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
						CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
						for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
						{
							CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
						}
						try
						{
							if (CommonClass.BaseControlCRSelectFunction != null)
							{
								showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

							}
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.Message);
							Logger.LogError(ex);
						}

						errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);

						errorNodeImage(trvSetting.Nodes[2].Nodes[1]);
						errorNodeImage(trvSetting.Nodes[2].Nodes[2]);


					}
					else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
					{


						showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);
						errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
					}
					else if (CommonClass.BaseControlCRSelectFunction != null)
					{
						errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
						showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

					}
					else if (CommonClass.LstBaseControlGroup != null && CommonClass.LstBaseControlGroup.Count > 0)
					{
						int nodesCount = 0;
						foreach (TreeNode trchild in trvSetting.Nodes)
						{
							if (trchild.Name == "airqualitygridgroup")
							{
								nodesCount = trchild.Nodes.Count;


								for (int i = nodesCount - 1; i > -1; i--)
								{
									TreeNode node = trchild.Nodes[i];
									if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
								}
								for (int i = CommonClass.LstBaseControlGroup.Count - 1; i > -1; i--)
								{
									AddDataSourceNode(CommonClass.LstBaseControlGroup[i], trchild);
									if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null)
									{
										changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[0]);
									}
									if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Control != null)
									{
										changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[1]);
									}
									if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null && CommonClass.LstBaseControlGroup[i].Control != null)
									{
										changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1]);
									}
								}
								trchild.ExpandAll();

								foreach (TreeNode trair in trchild.Nodes)
								{
									if (trair.Name != "datasource")
										changeNodeImage(trair);
									TreeNode tr = trair;
									if (trair.Name == "gridtype")
									{
										AddChildNodes(ref tr, "", "", new BenMAPLine());
										trair.ExpandAll();
									}
								}
							}
							if (trchild.Name == "configuration")
							{
								foreach (TreeNode tr in trchild.Nodes)
								{
									initNodeImage(tr);
								}
								trchild.ExpandAll();
							}
							if (trchild.Name == "aggregationpoolingvaluation")
							{
								foreach (TreeNode tr in trchild.Nodes)
								{
									initNodeImage(tr);
								}
								trchild.ExpandAll();
							}
						}
					}
					else
					{
						if (CommonClass.GBenMAPGrid != null)
						{
							TreeNode currentNode = trvSetting.Nodes[0].Nodes["gridtype"];
							AddChildNodes(ref currentNode, "", "", null);
							changeNodeImage(currentNode);
						}
						if (CommonClass.LstPollutant != null)
						{
							int nodesCount = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.Count;
							if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
							{
								for (int i = nodesCount - 2; i > -1; i--)
								{
									TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
									if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource") { trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.RemoveAt(i); }
								}
								for (int i = nodesCount - 1; i > -1; i--)
								{
									TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
									if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource")
									{
										trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Text = "Source of Air Quality Data";
										trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Tag = null;
										trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Nodes.Clear();
										trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Tag = null;
										trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Nodes.Clear();
									}
								}


								olvCRFunctionResult.SetObjects(null);
								olvIncidence.SetObjects(null);
								tlvAPVResult.SetObjects(null);
								cbPoolingWindowIncidence.Items.Clear();
								cbPoolingWindowAPV.Items.Clear();
								ClearMapTableChart();
								initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

								initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
								initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
								return;
							}

							trvSetting.Nodes["pollutant"].Parent.ExpandAll();
						}
					}
					if (CommonClass.BenMAPPopulation != null)
					{
						changeNodeImage(trvSetting.Nodes[1].Nodes[0]);
					}
					if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
					{
						changeNodeImage(trvSetting.Nodes[2].Nodes[0]);
					}


				}
				else if (CommonClass.InputParams != null && CommonClass.InputParams.Length > 0 && CommonClass.InputParams[0].ToLower().IndexOf("smat") > 0)
				{
					splitContainer1.Visible = true;
					CommonClass.ClearAllObject();
					ClearAll();
					ResetParamsTree("");

					ClearMapTableChart();
					initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

					initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
					initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
					CommonClass.IncidencePoolingAndAggregationAdvance = null;

					olvCRFunctionResult.SetObjects(null);
					olvIncidence.SetObjects(null);
					tlvAPVResult.SetObjects(null);

					cbPoolingWindowIncidence.Items.Clear();
					cbPoolingWindowAPV.Items.Clear();
					ClearMapTableChart();
					SetTabControl(tabCtlReport);
					CommonClass.LstPollutant = null;
					CommonClass.RBenMAPGrid = null;

					CommonClass.GBenMAPGrid = null;
					CommonClass.LstBaseControlGroup = null;
					CommonClass.CRThreshold = 0;
					CommonClass.CRLatinHypercubePoints = 20;
					CommonClass.CRRunInPointMode = false;

					CommonClass.BenMAPPopulation = null;

					CommonClass.BaseControlCRSelectFunction = null;
					CommonClass.BaseControlCRSelectFunctionCalculateValue = null;

					CommonClass.lstIncidencePoolingAndAggregation = null;

					CommonClass.IncidencePoolingResult = null;
					CommonClass.ValuationMethodPoolingAndAggregation = null;
					CommonClass.BaseControlCRSelectFunction = null;
					CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
					CommonClass.ValuationMethodPoolingAndAggregation = null;
					GC.Collect();

					LoadAirQualityData frm = new LoadAirQualityData(CommonClass.InputParams[0]);
					DialogResult dr = frm.ShowDialog();
					if (dr != DialogResult.OK)
						Environment.Exit(0);
					BaseControlGroup bcg = frm.bcgOpenAQG;
					frm.Dispose();

					for (int i = 0; i < trvSetting.Nodes.Count; i++)
					{
						TreeNode trchild = trvSetting.Nodes[i];
						if (trchild.Name == "airqualitygridgroup")
						{
							int nodesCount = trchild.Nodes.Count;
							for (int j = nodesCount - 1; j > -1; j--)
							{
								TreeNode node = trchild.Nodes[j];
								if (trchild.Nodes[j].Name == "datasource")
								{
									BrushBaseControl(ref trchild, bcg, trchild.Nodes[j].Index);
								}
							}
						}
					}

				}
			}
			catch (Exception ex)
			{

			}
		}

		private bool isLoad = false;

		private System.Data.DataSet BindGridtype()
		{
			try
			{
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select -1 as  GridDefinitionID, '' as GridDefinitionName from GridDefinitions union select distinct GridDefinitionID,GridDefinitionName from griddefinitions where SetupID={0}  ", CommonClass.MainSetup.SetupID);
				System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				return dsGrid;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return null;
			}
		}

		private System.Data.DataSet BindGridtypeDomain()
		{
			try
			{
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select distinct GridDefinitionID,GridDefinitionName from griddefinitions  where columns<=56 and setupid={0}  order by GridDefinitionName desc  ", CommonClass.MainSetup.SetupID);
				System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				return dsGrid;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return null;
			}
		}

		private void tsbPrintLayout_Click(object sender, EventArgs e)
		{
			try
			{
				SetUpPrintLayout();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				Debug.WriteLine("tsbSavePic_Click: " + ex.ToString());
			}
		}

		private void btnPieTheme_Click(object sender, EventArgs e)
		{
			try
			{
				double MinValue = 0;
				double MaxValue = 0;
				IMapLayer removeLayer = null;
				if (mainMap.GetAllLayers().Count == 3)
				{
					foreach (IMapLayer layer in mainMap.GetAllLayers())
					{
						if (layer is MapPointLayer)
							removeLayer = layer;
					}
				}
				if (removeLayer != null)
					mainMap.GetAllLayers().Remove(removeLayer);
				if (mainMap.GetAllLayers().Count != 2)
				{
					MessageBox.Show("No available layer to generate pie theme.");
					return;
				}
				else
				{
					IFeatureSet fs = (mainMap.GetAllLayers()[1] as PolygonLayer).DataSet;
					if (fs.DataTable.Rows.Count > 5000)
					{
						MessageBox.Show("Too many features to be displayed in this aggregation layer.");
						return;
					}
					WaitShow("Loading theme layer... ");
					FeatureSet fsValue = null;
					if (LayerObject != null && LayerObject is BenMAPLine)
					{
					}

					fsValue = getThemeFeatureSet((mainMap.GetAllLayers()[0] as PolygonLayer).DataSet.DataTable.Columns.Count - 1, ref MinValue, ref MaxValue);

					if (MaxValue <= 0)
					{
						double fz = Math.Abs(MaxValue);
						if (fz < Math.Abs(MinValue))
						{
							MaxValue = Math.Abs(MinValue);
							MinValue = fz;
						}
						else
						{
							MinValue = Math.Abs(MinValue);
							MaxValue = Math.Abs(MaxValue);
						}
					}
					if (MaxValue == 0)
					{
						WaitClose();
						return;
					}
					MaxValue = Math.Abs(MaxValue);
					MinValue = Math.Abs(MinValue);
					if (MinValue > MaxValue)
					{
						double d = MinValue;
						MinValue = MaxValue;
						MaxValue = d;
					}
					fsValue.Name = "ThemeLayer";
					mainMap.Layers.Add(fsValue);
					PointScheme ps = new PointScheme();

					PointSymbolizer commonSymboliser = new PointSymbolizer();
					commonSymboliser.IsVisible = true;
					commonSymboliser.Smoothing = true;
					ps.AppearsInLegend = true;

					ps.LegendText = "ThemeValue";
					ps.ClearCategories();
					ps.Categories.Clear();
					foreach (DataRow dr in fsValue.DataTable.Rows)
					{
						double gridvalue = Math.Abs(Convert.ToDouble(dr["ThemeValue"]) * (100.000 / MaxValue));
						if (gridvalue > 0)
						{
							Bitmap ig = null; if (Convert.ToDouble(dr["ThemeValue"]) < 0)
							{
								ig = getonly3DPie(150, Color.Green);
							}
							else
							{
								ig = getonly3DPie(150, Color.Red);
							}
							if (Convert.ToInt32(gridvalue) == 0) continue;
							Bitmap ig2 = null;
							if ((sender as ToolStripButton).Name == "btnPieTheme")
							{
								ig2 = new Bitmap(ig, new System.Drawing.Size(Convert.ToInt32(gridvalue), Convert.ToInt32(gridvalue)));
							}
							else
							{
								if (Convert.ToDouble(dr["ThemeValue"]) < 0)
								{
									ig2 = DrawCell(Color.Green, 0, 10, 20, Convert.ToInt32(gridvalue), 10);
								}
								else
									ig2 = DrawCell(Color.Red, 0, 10, 20, Convert.ToInt32(gridvalue), 10);
							}

							PictureSymbol psymbol = new PictureSymbol(ig2);
							psymbol.Size = new Size2D(ig2.Width, ig2.Height);
							PointSymbolizer NONcommonSymboliser = new PointSymbolizer();
							NONcommonSymboliser.Smoothing = true;
							NONcommonSymboliser.IsVisible = true;
							NONcommonSymboliser.Symbols.Clear();
							NONcommonSymboliser.Symbols.Add(psymbol);

							PointCategory pc1 = new PointCategory(NONcommonSymboliser);
							pc1.FilterExpression = "[ThemeValue] = " + dr["ThemeValue"].ToString();
							pc1.LegendText = "[ThemeValue] = " + dr["ThemeValue"].ToString();
							pc1.DisplayExpression();

							ps.AddCategory(pc1);
						}
					}
						(mainMap.GetAllLayers()[2] as PointLayer).Symbology = ps;
				}
				WaitClose();
			}
			catch
			{
				WaitClose();
			}
		}

		private Bitmap DrawCell(Color myColor, int x, int y, int width, int height, float iDeep)
		{
			Bitmap objBitmap = new Bitmap(Convert.ToInt32(width + iDeep * 1.2), Convert.ToInt32(height + iDeep * 1.2));
			Graphics objGraphics = Graphics.FromImage(objBitmap);
			objGraphics.Clear(Color.Transparent);
			Rectangle Frect =
					new Rectangle((int)x, (int)y, (int)width, (int)height);
			Rectangle Brect =
					new Rectangle(Frect.X + (int)iDeep, Frect.Y - (int)iDeep, Frect.Width, Frect.Height);

			PointF p1 = new PointF((float)Frect.X, (float)Frect.Y);
			PointF p2 = new PointF((float)Frect.X, (float)(Frect.Y + Frect.Height));
			PointF p3 = new PointF((float)(Frect.X + Frect.Width), (float)(Frect.Y + Frect.Height));
			PointF p4 = new PointF((float)(Frect.X + Frect.Width), (float)Frect.Y);

			PointF p5 = new PointF((float)Brect.X, (float)Brect.Y);
			PointF p6 = new PointF((float)Brect.X, (float)(Brect.Y + Brect.Height));
			PointF p7 = new PointF((float)(Brect.X + Brect.Width), (float)(Brect.Y + Brect.Height));
			PointF p8 = new PointF((float)(Brect.X + Brect.Width), (float)Brect.Y);

			PointF[] ptsArray1 =         {
					p1, p2, p6, p5
			};

			PointF[] ptsArray2 =          {
					p2, p3, p7, p6
			};

			PointF[] ptsArray3 =         {
					p4, p3, p7, p8
			};

			PointF[] ptsArray4 =         {
					p1, p4, p8, p5
			};

			SolidBrush defaultBrush = new SolidBrush(Color.FromArgb(128, myColor.R, myColor.G, myColor.B));
			int r, g, b;
			r = myColor.R - 37;
			g = myColor.G - 37;
			b = myColor.B - 45;
			if (r < 0) r = 0;
			if (g < 0) g = 0;
			if (b < 0) b = 0;
			SolidBrush topBrush = new SolidBrush(Color.FromArgb(188, r, g, b));
			int r1, g1, b1;

			r1 = myColor.R - 52;
			g1 = myColor.G - 52;
			b1 = myColor.B - 88;
			if (r1 < 0) r1 = 0;
			if (g1 < 0) g1 = 0;
			if (b1 < 0) b1 = 0;
			SolidBrush rightBrush = new SolidBrush(Color.FromArgb(220, r1, g1, b1));

			SolidBrush invisibleBrush =
					new SolidBrush(Color.FromArgb(111, myColor.R, myColor.G, myColor.B));
			SolidBrush visibleBrush =
					new SolidBrush(Color.FromArgb(188, myColor.R, myColor.G, myColor.B));
			SolidBrush FrectBrush =
					new SolidBrush(Color.FromArgb(220, myColor.R, myColor.G, myColor.B));

			objGraphics.FillRectangle(defaultBrush, Brect);
			objGraphics.DrawRectangle(Pens.Black, Brect);

			objGraphics.FillPolygon(defaultBrush, ptsArray1);
			objGraphics.DrawPolygon(Pens.Black, ptsArray1);
			objGraphics.FillPolygon(defaultBrush, ptsArray2);
			objGraphics.DrawPolygon(Pens.Black, ptsArray2);
			objGraphics.FillPolygon(rightBrush, ptsArray3);
			objGraphics.DrawPolygon(Pens.Black, ptsArray3);
			objGraphics.FillPolygon(topBrush, ptsArray4);
			objGraphics.DrawPolygon(Pens.Black, ptsArray4);

			objGraphics.FillRectangle(FrectBrush, Frect);
			objGraphics.DrawRectangle(Pens.Black, Frect);

			invisibleBrush.Dispose();
			visibleBrush.Dispose();
			FrectBrush.Dispose();
			return objBitmap;
		}

		private Bitmap get3DPie()
		{
			const int width = 150, height = 150;
			int x = 30, y = 50;

			int pieWidth = 120, pieHeight = 40, pieShadow = 15;
			int[] arrVote = { 70, 90, 80, 20, 60, 40 };
			Random oRan = new Random();

			Bitmap objBitmap = new Bitmap(width, height);
			Graphics objGraphics = Graphics.FromImage(objBitmap);
			objGraphics.Clear(Color.Transparent);
			SolidBrush objBrush = new SolidBrush(Color.Blue);
			objGraphics.SmoothingMode = SmoothingMode.AntiAlias;

			int iCurrentPos = 0;

			Color[] arrColor = { Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red };

			for (int i = arrVote.Length - 1; i >= 0; i--)
			{
				arrColor[i] = Color.FromArgb(oRan.Next(255), oRan.Next(255), oRan.Next(255));
			}

			for (int i = arrVote.Length - 1; i >= 0; i--)
			{
				objBrush.Color = arrColor[i];
				for (int iLoop2 = 0; iLoop2 < pieShadow; iLoop2++)
					objGraphics.FillPie(new HatchBrush(HatchStyle.Percent50, objBrush.Color), x, y + iLoop2, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
				iCurrentPos += arrVote[i];
			}

			iCurrentPos = 0;
			for (int i = arrVote.Length - 1; i >= 0; i--)
			{
				objBrush.Color = arrColor[i];
				objGraphics.FillPie(objBrush, x, y, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
				iCurrentPos += arrVote[i];
			}
			return objBitmap;
		}

		private Bitmap getonly3DPie(int width, Color c)
		{
			int height = width;
			int x = 30, y = 50;

			int pieWidth = 120, pieHeight = 40, pieShadow = 15;
			int[] arrVote = { 70, 90, 80, 20, 60, 40 };
			Random oRan = new Random();

			Bitmap objBitmap = new Bitmap(width, height);
			Graphics objGraphics = Graphics.FromImage(objBitmap);
			objGraphics.Clear(Color.Transparent);
			SolidBrush objBrush = new SolidBrush(Color.Blue);
			objGraphics.SmoothingMode = SmoothingMode.AntiAlias;

			int iCurrentPos = 0;

			Color[] arrColor = { c, c, c, c, c, c };
			for (int i = arrVote.Length - 1; i >= 0; i--)
			{
				arrColor[i] = c;
			}

			for (int i = arrVote.Length - 1; i >= 0; i--)
			{
				objBrush.Color = arrColor[i];
				for (int iLoop2 = 0; iLoop2 < pieShadow; iLoop2++)
					objGraphics.FillPie(new HatchBrush(HatchStyle.Percent50, objBrush.Color), x, y + iLoop2, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
				iCurrentPos += arrVote[i];
			}

			iCurrentPos = 0;
			for (int i = arrVote.Length - 1; i >= 0; i--)
			{
				objBrush.Color = arrColor[i];
				objGraphics.FillPie(objBrush, x, y, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
				iCurrentPos += arrVote[i];
			}
			return objBitmap;
		}

		private void ClearMapTableChart()
		{
			//if (!_MapAlreadyDisplayed) mainMap.Layers.Clear();

			SetOLVResultsShowObjects(null);
			_tableObject = null;
			oxyPlotView.Visible = false;
			btnApply.Visible = false;
			olvRegions.Visible = false;
			cbGraph.Visible = false;
			groupBox9.Visible = false;
			groupBox1.Visible = false;
			btnSelectAll.Visible = false;
		}

		private void olvCRFunctionResult_DoubleClick(object sender, EventArgs e)
		{
			//dpa 9/11/2017 removed commented code - all work is done in btShowCRResult_Click now.
			//this event fires when the user double clicks a record in the Health Impact Results tab (Tab #2)
			btShowCRResult_Click(sender, e);
		}

		private void tlvAPVResult_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				ClearMapTableChart();
				if (tlvAPVResult.SelectedObjects.Count == 0) return;

				string Tip = "Drawing pooled valuation results layer";
				WaitShow(Tip);
				bool bGIS = true;
				bool bTable = true;
				bool bChart = true;

				List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
				if (CommonClass.IncidencePoolingAndAggregationAdvance == null || CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation == null)
				{
					chbAPVAggregation.Checked = false;

				}
				else
				{
					chbAPVAggregation.Checked = true;

				}

				if (sender is ObjectListView || sender is Button)
				{
					foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in tlvAPVResult.SelectedObjects)
					{
						AllSelectValuationMethod allSelectValuationMethod = keyValue.Key;
						if (rbAPVOnlyOne.Checked)
						{
							ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

							AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;

							try
							{
								if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
								{

									allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

								}
								else
								{

									allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();
								}
								if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
									lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
							}
							catch
							{ }
						}
						else
						{
							AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
							if (allSelectValuationMethod.ID < 0) continue;

							ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

							try
							{


								allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

								if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
									lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
							}
							catch
							{
							}
						}
					}
					tabCtlMain.SelectedIndex = 1;
					if (tlvAPVResult.SelectedObjects.Count > 1)
					{

						bGIS = false;
						bChart = false;
					}
				}
				else
				{
					if (_MapAlreadyDisplayed && _APVdragged)//MCB- a kluge: Need a better way to determine if sender was from map
					{
						bGIS = true;
						bChart = false;
						tabCtlMain.SelectedIndex = 0;

					}
					else
					{
						bGIS = false;
						bChart = false;
						tabCtlMain.SelectedIndex = 1;
					}

					foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in tlvAPVResult.Objects)
					{
						AllSelectValuationMethod allSelectValuationMethod = keyValue.Key;
						if (rbAPVOnlyOne.Checked)
						{
							ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

							AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;

							try
							{
								if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
								{

									allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

								}
								else
								{

									allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();
								}
								if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
									lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
							}
							catch
							{ }
						}
						else
						{
							AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;

							ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();
							try
							{


								allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

								if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
									lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
							}
							catch
							{
							}
						}
					}
				}
				if (lstallSelectValuationMethodAndValue[0].lstAPVValueAttributes.Count == 1 && !CommonClass.CRRunInPointMode)
				{
					lstAPVRforCDF = lstallSelectValuationMethodAndValue;
					canshowCDF = true;
					bChart = true;
				}
				else
				{
					canshowCDF = false;
					lstAPVRforCDF = new List<AllSelectValuationMethodAndValue>();
				}
				iCDF = 2;
				ClearMapTableChart();
				if (this.rbShowActiveAPV.Checked)
				{
					if (tabCtlMain.SelectedIndex == 0)
					{
						bTable = false;
						bChart = false;
					}
					else if (tabCtlMain.SelectedIndex == 1)
					{
						bGIS = false;
						bChart = false;
					}
					else if (tabCtlMain.SelectedIndex == 2)
					{
						bGIS = false;
						bTable = false;
					}
				}
				if (lstallSelectValuationMethodAndValue == null)
				{
					WaitClose();
					MessageBox.Show("No result in this method. It might have been pooled before!");
					return;
				}

				else if (lstallSelectValuationMethodAndValue.Count == 0)
				{
					WaitClose();
					MessageBox.Show("No result in this method. It might have been pooled before!");
					return;
				}
				BenMAPGrid benMapGridShow = null;

				if (bChart && lstallSelectValuationMethodAndValue != null)
				{

					InitChartResult(lstallSelectValuationMethodAndValue, CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null ? CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID);
				}

				if (lstallSelectValuationMethodAndValue != null)
				{
					if (this.cbAPVAggregation.SelectedIndex != -1 && cbAPVAggregation.SelectedIndex != 0)
					{
						int idCbo = Convert.ToInt32((cbAPVAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
						int idAggregation = -1;
						GridRelationship gridRelationship = null;
						if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
						{
							idAggregation = CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;

							if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == idAggregation).Count() > 0)
							{
								gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == idAggregation).First();
							}
							else
							{
								CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == idCbo).First();

							}
						}
						else
						{
							if (CommonClass.GBenMAPGrid.GridDefinitionID == idCbo)
							{
							}
							else if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID).Count() > 0)
							{
								gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID).First();
							}
							else
							{
								gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID && p.smallGridID == idCbo).First();

							}
						}

						if (idCbo != idAggregation)
						{
							List<AllSelectValuationMethodAndValue> lstTmp = new List<AllSelectValuationMethodAndValue>();
							foreach (AllSelectValuationMethodAndValue asvm in lstallSelectValuationMethodAndValue)
							{
								lstTmp.Add(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gridRelationship, idAggregation == -1 ? CommonClass.GBenMAPGrid : CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation, asvm));
							}
							lstallSelectValuationMethodAndValue = lstTmp;
							benMapGridShow = Grid.GridCommon.getBenMAPGridFromID(idCbo);
						}

					}

				}

				if (bTable && lstallSelectValuationMethodAndValue != null)
				{
					InitTableResult(lstallSelectValuationMethodAndValue);
				}

				if (bGIS)
				{
					//set change projection text
					string changeProjText = "change projection to setup projection";
					if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
					{
						changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
					}
					tsbChangeProjection.Text = changeProjText;

					mainMap.ProjectionModeReproject = ActionMode.Never;
					mainMap.ProjectionModeDefine = ActionMode.Never;
					string shapeFileName = "";

					MapGroup ResultsMG = AddMapGroup("Results", "Map Layers", false, false);
					MapGroup PVResultsMG = AddMapGroup("Pooled Valuation", "Results", false, false);

					string author = "Author Unknown";
					string LayerTextName;
					string poolingWindow = "";

					if (lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod != null
							&& lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Name != null)
					{
						author = lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Author;
					}
					LayerTextName = author;

					foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in tlvAPVResult.SelectedObjects)
					{
						if (keyValue.Key.BenMAPValuationFunction != null && keyValue.Key.BenMAPValuationFunction.ID == lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.BenMAPValuationFunction.ID)
						{
							poolingWindow = keyValue.Value;
						}
					}
					if (!String.IsNullOrEmpty(poolingWindow))
						author = poolingWindow + "--" + LayerTextName;
					RemoveOldPolygonLayer(LayerTextName, PVResultsMG.Layers, false);

					if (!chbAPVAggregation.Checked)
					{
						if (CommonClass.GBenMAPGrid is ShapefileGrid)
						{
							if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
							{
								shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
							}
						}
						else if (CommonClass.GBenMAPGrid is RegularGrid)
						{
							if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
							{
								shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
							}
						}
					}
					else
					{
						if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation is ShapefileGrid)
						{
							if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as ShapefileGrid).ShapefileName + ".shp"))
							{
								shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as ShapefileGrid).ShapefileName + ".shp";
							}
						}
						else if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation is RegularGrid)
						{
							if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as RegularGrid).ShapefileName + ".shp"))
							{
								shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as RegularGrid).ShapefileName + ".shp";
							}
						}
					}
					if (benMapGridShow != null)
					{
						shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + ((benMapGridShow is ShapefileGrid) ? (benMapGridShow as ShapefileGrid).ShapefileName : (benMapGridShow as RegularGrid).ShapefileName) + ".shp";
					}
					MapPolygonLayer APVResultPolyLayer1 = (MapPolygonLayer)PVResultsMG.Layers.Add(shapeFileName);
					IFeatureSet fs = APVResultPolyLayer1.DataSet;
					APVResultPolyLayer1.Name = author;
					APVResultPolyLayer1.LegendText = APVResultPolyLayer1.Name;
					int j = 0;
					int iCol = 0;
					int iRow = 0;
					List<string> lstRemoveName = new List<string>();
					while (j < fs.DataTable.Columns.Count)
					{
						if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
						if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

						j++;
					}
					j = 0;

					while (j < fs.DataTable.Columns.Count)
					{
						if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col" || fs.DataTable.Columns[j].ColumnName.ToLower() == "row")
						{ }
						else
							lstRemoveName.Add(fs.DataTable.Columns[j].ColumnName);

						j++;
					}
					foreach (string s in lstRemoveName)
					{
						fs.DataTable.Columns.Remove(s);
					}
					fs.DataTable.Columns.Add("Point Estimate", typeof(double));
					j = 0;
					while (j < fs.DataTable.Columns.Count)
					{
						if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
						if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

						j++;
					}
					j = 0;

					DataTable dt = fs.DataTable;
					if (apvlstHealth == null)
					{
						dt.Columns.Add("Endpoint", typeof(string));
						dt.Columns.Add("Author", typeof(string));
						dt.Columns.Add("Start Age", typeof(int));
						dt.Columns.Add("End Age", typeof(int));
						dt.Columns.Add("Version", typeof(string));
					}
					else
					{
						foreach (FieldCheck fieldCheck in apvlstHealth)
						{
							if (fieldCheck.isChecked)
							{
								dt.Columns.Add(fieldCheck.FieldName);
							}
						}
					}
					if (apvlstResult == null)
					{
						dt.Columns.Add("Mean", typeof(double));
						dt.Columns.Add("Standard Deviation", typeof(double));
						dt.Columns.Add("Variance", typeof(double));
						int numPercentiles = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count;
						double step = 100 / (double)numPercentiles;
						double current = step / 2;

						for (int k = 0; k < numPercentiles; k++)
						{
							string currLabel = String.Concat("Percentile ", current.ToString());
							dt.Columns.Add(currLabel, typeof(float));
							current += step;
						}
					}
					else
					{
						foreach (FieldCheck fieldCheck in apvlstResult)
						{
							if (fieldCheck.isChecked && (fieldCheck.FieldName != "Percentiles" && fieldCheck.FieldName != "Point Estimate"))
							{
								dt.Columns.Add(fieldCheck.FieldName, typeof(double));
							}
						}

						if ((apvlstResult.Find(p => p.FieldName.Equals("Percentiles")).isChecked) && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
						{
							int numPercentiles = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count;
							double step = 100 / (double)numPercentiles;
							double current = step / 2;

							for (int k = 0; k < numPercentiles; k++)
							{
								string currLabel = String.Concat("Percentile ", current.ToString());
								dt.Columns.Add(currLabel, typeof(float));
								current += step;
							}
						}

					}

					foreach (AllSelectValuationMethodAndValue asvm in lstallSelectValuationMethodAndValue)
					{
						foreach (APVValueAttribute apv in asvm.lstAPVValueAttributes)
						{
							DataRow[] foundRow = dt.Select(String.Format("Row = '{0}' AND Col = '{1}'", apv.Row, apv.Col));

							if (foundRow.Length == 1)
							{
								if (apvlstHealth != null)
								{
									foreach (FieldCheck fieldCheck in apvlstHealth)
									{
										if (fieldCheck.isChecked)
										{
											foundRow[0][fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, asvm.AllSelectValuationMethod, apv);
										}
									}
								}
								else
								{
									foundRow[0]["Endpoint"] = asvm.AllSelectValuationMethod.EndPoint;
									foundRow[0]["Author"] = asvm.AllSelectValuationMethod.Author;
									foundRow[0]["Start Age"] = asvm.AllSelectValuationMethod.StartAge;
									foundRow[0]["End Age"] = asvm.AllSelectValuationMethod.EndAge; ;
									foundRow[0]["Version"] = asvm.AllSelectValuationMethod.Version;
								}

								if (apvlstResult != null)
								{
									foreach (FieldCheck fieldCheck in apvlstResult)
									{
										if (fieldCheck.isChecked && fieldCheck.FieldName != "Percentiles")
										{
											foundRow[0][fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, asvm.AllSelectValuationMethod, apv);
										}
										int k = 0;
										if (fieldCheck.isChecked && fieldCheck.FieldName == "Percentiles")
										{
											while (k < apv.LstPercentile.Count())
											{

												foundRow[0]["Percentile " + ((Convert.ToDouble(k + 1) * 100.00 / Convert.ToDouble(apv.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(apv.LstPercentile.Count())))))] = apv.LstPercentile[k];
												k++;
											}
										}
									}
								}
								else
								{
									foundRow[0]["Point Estimate"] = apv.PointEstimate;
									foundRow[0]["Mean"] = apv.Mean;
									foundRow[0]["Standard Deviation"] = apv.StandardDeviation;
									foundRow[0]["Variance"] = apv.Variance;

									int numPercentiles = apv.LstPercentile.Count;
									double step = 100 / (double)numPercentiles;
									double current = step / 2;

									for (int k = 0; k < numPercentiles; k++)
									{
										string currLabel = String.Concat("Percentile ", current.ToString());
										foundRow[0][currLabel] = apv.LstPercentile[k];
										current += step;
									}
								}
							}
						}
					}

					if (File.Exists(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp");
					fs.SaveAs(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp", true);
					//APVResultPolyLayer1.DataSet.DataTable.Columns[(APVResultPolyLayer1).DataSet.DataTable.Columns.Count - 1].ColumnName = "Pooled Valuation";

					MapPolygonLayer polLayer = APVResultPolyLayer1;
					string strValueField = polLayer.DataSet.DataTable.Columns["Point Estimate"].ColumnName;

					_columnName = strValueField;
					polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "A"); //-MCB added

					double dMinValue = 0.0;
					double dMaxValue = 0.0;
					dMinValue = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Min(a => a.PointEstimate);
					dMaxValue = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Max(a => a.PointEstimate);
					_dMinValue = dMinValue;
					_dMaxValue = dMaxValue;

					_CurrentIMapLayer = APVResultPolyLayer1;
					_columnName = strValueField;
					_CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: Pooled Valuation- " + APVResultPolyLayer1.LegendText;
					addAdminLayers();
				}
				WaitClose();
			}
			catch (Exception ex)
			{ }
			finally
			{
				WaitClose();
			}
		}

		private void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValueMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
		{
			List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID).ToList(); lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 3).ToList());
			foreach (AllSelectValuationMethod asvm in lstOne)
			{
				getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);
			}
		}

		private void getAllChildQALYMethodNotNone(AllSelectQALYMethod allSelectQALYMethod, List<AllSelectQALYMethod> lstAll, ref List<AllSelectQALYMethod> lstReturn)
		{
			List<AllSelectQALYMethod> lstOne = lstAll.Where(p => p.PID == allSelectQALYMethod.ID).ToList(); lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 3).ToList());
			foreach (AllSelectQALYMethod asvm in lstOne)
			{
				getAllChildQALYMethodNotNone(asvm, lstAll, ref lstReturn);
			}
		}

		private string getFieldNameFromlstHealth(string s)
		{
			string fieldName = "";

			switch (s)
			{
				case "Column":
					fieldName = "Col";
					break;
				case "Row":
					fieldName = "Row";
					break;
				case "Point Estimate":
					fieldName = "PointEstimate";
					break;
				case "Incidence":
					fieldName = "Incidence";
					break;
				case "Population":
					fieldName = "Population";
					break;
				case "Delta":
					fieldName = "Delta";
					break;
				case "Mean":
					fieldName = "Mean";
					break;
				case "Baseline":
					fieldName = "Baseline";
					break;
				case "Percent of Baseline":
					fieldName = "PercentOfBaseline";
					break;
				case "Standard Deviation":
					fieldName = "StandardDeviation";
					break;
				case "Variance":
					fieldName = "Variance";
					break;
				case "Dataset":
					fieldName = "BenMAPHealthImpactFunction.DataSetName";
					break;
				case "Endpoint Group":
					fieldName = "BenMAPHealthImpactFunction.EndPointGroup";
					break;
				case "Endpoint":
					fieldName = "BenMAPHealthImpactFunction.EndPoint";
					break;
				case "Pollutant":
					fieldName = "BenMAPHealthImpactFunction.Pollutant.PollutantName";
					break;
				case "Metric":
					fieldName = "BenMAPHealthImpactFunction.Metric.MetricName";
					break;
				case "Seasonal Metric":
					fieldName = "BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName";
					break;
				case "Metric Statistic":
					fieldName = "BenMAPHealthImpactFunction.MetricStatistic";
					break;
				case "Author":
					fieldName = "BenMAPHealthImpactFunction.Author";
					break;
				case "Year":
					fieldName = "BenMAPHealthImpactFunction.Year";
					break;
				case "Study Location":
					fieldName = "BenMAPHealthImpactFunction.strLocations";
					break;
				case "Geographic Area":
					//fieldName = "BenMAPHealthImpactFunction.GeographicAreaName";
					fieldName = "GeographicAreaName";
					break;
				case "Other Pollutants":
					fieldName = "BenMAPHealthImpactFunction.OtherPollutants";
					break;
				case "Qualifier":
					fieldName = "BenMAPHealthImpactFunction.Qualifier";
					break;
				case "Reference":
					fieldName = "BenMAPHealthImpactFunction.Reference";
					break;
				case "Race":
					fieldName = "Race";
					break;
				case "Ethnicity":
					fieldName = "Ethnicity";
					break;
				case "Gender":
					fieldName = "Gender";
					break;
				case "Start Age":
					fieldName = "StartAge";
					break;
				case "End Age":
					fieldName = "EndAge";
					break;
				case "Function":
					fieldName = "BenMAPHealthImpactFunction.Function";
					break;
				case "Incidence Dataset":
					fieldName = "IncidenceDataSetName";
					break;
				case "Prevalence Dataset":
					fieldName = "PrevalenceDataSetName";
					break;
				case "Beta":
					fieldName = "BenMAPHealthImpactFunction.Beta";
					break;
				case "Beta Distribution":
					fieldName = "BenMAPHealthImpactFunction.BetaDistribution";
					break;
				case "Beta Parameter 1":
					fieldName = "BenMAPHealthImpactFunction.BetaParameter1";
					break;
				case "Beta Parameter 2":
					fieldName = "BenMAPHealthImpactFunction.BetaParameter2";
					break;
				case "A":
					fieldName = "BenMAPHealthImpactFunction.AContantValue";
					break;
				case "A Description":
					fieldName = "BenMAPHealthImpactFunction.AContantDescription";
					break;
				case "B":
					fieldName = "BenMAPHealthImpactFunction.BContantValue";
					break;
				case "B Description":
					fieldName = "BenMAPHealthImpactFunction.BContantDescription";
					break;
				case "C":
					fieldName = "BenMAPHealthImpactFunction.CContantValue";
					break;
				case "C Description":
					fieldName = "BenMAPHealthImpactFunction.CContantDescription";
					break;


			}
			return fieldName;
		}

		private object getFieldNameFromlstHealthObject(string s, CRCalculateValue crv, CRSelectFunction crf)
		{
			object fieldName = "";

			switch (s)
			{
				case "Column":
					fieldName = crv.Col;
					break;
				case "Row":
					fieldName = crv.Row;
					break;
				case "Point Estimate":
					fieldName = crv.PointEstimate;
					break;
				case "Incidence":
					fieldName = crv.Incidence;
					break;
				case "Population":
					fieldName = crv.Population;
					break;
				case "Delta":
					fieldName = crv.Delta;
					break;
				case "Mean":
					fieldName = crv.Mean;
					break;
				case "Baseline":
					fieldName = crv.Baseline;
					break;
				case "Percent of Baseline":
					fieldName = crv.PercentOfBaseline;
					break;
				case "Standard Deviation":
					fieldName = crv.StandardDeviation;
					break;
				case "Variance":
					fieldName = crv.Variance;
					break;
				case "Dataset":
					fieldName = crf.BenMAPHealthImpactFunction.DataSetName;
					break;
				case "Endpoint Group":
					fieldName = crf.BenMAPHealthImpactFunction.EndPointGroup == null ? "" : crf.BenMAPHealthImpactFunction.EndPointGroup.Replace(",", " ");
					break;
				case "Endpoint":
					fieldName = crf.BenMAPHealthImpactFunction.EndPoint == null ? "" : crf.BenMAPHealthImpactFunction.EndPoint.Replace(",", " ");
					break;
				case "Pollutant":
					fieldName = crf.BenMAPHealthImpactFunction.Pollutant.PollutantName;
					break;
				case "Metric":
					fieldName = crf.BenMAPHealthImpactFunction.Metric.MetricName;
					break;
				case "Seasonal Metric":
					fieldName = crf.BenMAPHealthImpactFunction.SeasonalMetric == null ? "" : crf.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
					break;
				case "Metric Statistic":
					fieldName = crf.BenMAPHealthImpactFunction.MetricStatistic;
					break;
				case "Author":
					fieldName = crf.BenMAPHealthImpactFunction.Author == null ? "" : crf.BenMAPHealthImpactFunction.Author.Replace(",", " ");
					break;
				case "Year":
					fieldName = crf.BenMAPHealthImpactFunction.Year;
					break;
				case "Study Location":
					fieldName = crf.BenMAPHealthImpactFunction.strLocations;
					break;
				case "Geographic Area":
					fieldName = crf.GeographicAreaName;
					break;
				case "Other Pollutants":
					fieldName = crf.BenMAPHealthImpactFunction.OtherPollutants == null ? "" : crf.BenMAPHealthImpactFunction.OtherPollutants.Replace(",", " ");
					break;
				case "Qualifier":
					fieldName = crf.BenMAPHealthImpactFunction.Qualifier == null ? "" : crf.BenMAPHealthImpactFunction.Qualifier.Replace(",", " ");
					break;
				case "Reference":
					fieldName = crf.BenMAPHealthImpactFunction.Reference == null ? "" : crf.BenMAPHealthImpactFunction.Reference.Replace(",", " ");
					break;
				case "Race":
					fieldName = crf.Race;
					break;
				case "Ethnicity":
					fieldName = crf.Ethnicity;
					break;
				case "Gender":
					fieldName = crf.Gender;
					break;
				case "Start Age":
					fieldName = crf.StartAge;
					break;
				case "End Age":
					fieldName = crf.EndAge;
					break;
				case "Function":
					fieldName = crf.BenMAPHealthImpactFunction.Function;
					break;
				case "Incidence Dataset":
					fieldName = crf.IncidenceDataSetName;
					break;
				case "Prevalence Dataset":
					fieldName = crf.PrevalenceDataSetName;
					break;
				case "Beta":
					fieldName = crf.BenMAPHealthImpactFunction.Beta;
					break;
				case "Beta Distribution":
					fieldName = crf.BenMAPHealthImpactFunction.BetaDistribution;
					break;
				case "Beta Parameter 1":
					fieldName = crf.BenMAPHealthImpactFunction.BetaParameter1;
					break;
				case "Beta Parameter 2":
					fieldName = crf.BenMAPHealthImpactFunction.BetaParameter2;
					break;
				case "A":
					fieldName = crf.BenMAPHealthImpactFunction.AContantValue;
					break;
				case "A Description":
					fieldName = crf.BenMAPHealthImpactFunction.AContantDescription;
					break;
				case "B":
					fieldName = crf.BenMAPHealthImpactFunction.BContantValue;
					break;
				case "B Description":
					fieldName = crf.BenMAPHealthImpactFunction.BContantDescription;
					break;
				case "C":
					fieldName = crf.BenMAPHealthImpactFunction.CContantValue;
					break;
				case "C Description":
					fieldName = crf.BenMAPHealthImpactFunction.CContantDescription;
					break;


			}
			return fieldName;
		}

		private string getFieldNameFromlstAPV(string s)
		{
			string fieldName = s;

			switch (s)
			{
				case "Column":
					fieldName = "Col";
					break;
				case "Row":
					fieldName = "Row";
					break;
				case "Point Estimate":
					fieldName = "PointEstimate";
					break;
				case "Population":
					fieldName = "Population";
					break;
				case "Delta":
					fieldName = "Delta";
					break;
				case "Mean":
					fieldName = "Mean";
					break;
				case "Baseline":
					fieldName = "Baseline";
					break;
				case "Percent of Baseline":
					fieldName = "PercentOfBaseline";
					break;
				case "Standard Deviation":
					fieldName = "StandardDeviation";
					break;
				case "Variance":
					fieldName = "Variance";
					break;
				case "Dataset":
					fieldName = "DataSet";
					break;
				case "Endpoint Group":
					fieldName = "EndPointGroup";
					break;
				case "Endpoint":
					fieldName = "EndPoint";
					break;
				case "Pollutant":
					fieldName = "Pollutant";
					break;
				case "Metric":
					fieldName = "Metric";
					break;
				case "Seasonal Metric":
					fieldName = "SeasonalMetric";
					break;
				case "Metric Statistic":
					fieldName = "MetricStatistic";
					break;
				case "Author":
					fieldName = "Author";
					break;
				case "Year":
					fieldName = "Year";
					break;
				case "Study Location":
					fieldName = "Location";
					break;
				case "Geographic Area":
					fieldName = "GeographicArea";
					break;
				case "Other Pollutants":
					fieldName = "OtherPollutants";
					break;
				case "Qualifier":
					fieldName = "Qualifier";
					break;
				case "Reference":
					fieldName = "Reference";
					break;
				case "Race":
					fieldName = "Race";
					break;
				case "Ethnicity":
					fieldName = "Ethnicity";
					break;
				case "Gender":
					fieldName = "Gender";
					break;
				case "Start Age":
					fieldName = "StartAge";
					break;
				case "End Age":
					fieldName = "EndAge";
					break;
				case "Function":
					fieldName = "Function";
					break;
				case "Incidence Dataset":
					fieldName = "IncidenceDataSetID";
					break;
				case "Prevalence Dataset":
					fieldName = "PrevalenceDataSetID";
					break;
				case "Version":
					fieldName = "Version";
					break;
				case "Age Range":
					fieldName = "AgeRange";
					break;
			}
			return fieldName;
		}

		private object getFieldNameFromlstAPVObject(string s, AllSelectValuationMethod allSelectValuationMethod, APVValueAttribute apvValueAttribute)
		{
			object fieldName = "";

			switch (s)
			{
				case "Column":
					fieldName = apvValueAttribute.Col;
					break;
				case "Row":
					fieldName = apvValueAttribute.Row;
					break;
				case "Point Estimate":
					fieldName = apvValueAttribute.PointEstimate;
					break;

				case "Mean":
					fieldName = apvValueAttribute.Mean;
					break;

				case "Standard Deviation":
					fieldName = apvValueAttribute.StandardDeviation;
					break;
				case "Variance":
					fieldName = apvValueAttribute.Variance;
					break;
				case "Name":
					fieldName = allSelectValuationMethod.Name;
					break;
				case "Dataset":
					fieldName = allSelectValuationMethod.DataSet;
					break;
				case "Endpoint Group":
					fieldName = allSelectValuationMethod.EndPointGroup;
					break;
				case "Endpoint":
					fieldName = allSelectValuationMethod.EndPoint;
					break;
				case "Pollutant":
					fieldName = allSelectValuationMethod.Pollutant;
					break;
				case "Metric":
					fieldName = allSelectValuationMethod.Metric;
					break;
				case "Seasonal Metric":
					fieldName = allSelectValuationMethod.SeasonalMetric;
					break;
				case "Metric Statistic":
					fieldName = allSelectValuationMethod.MetricStatistic;
					break;
				case "Author":
					fieldName = allSelectValuationMethod.Author;
					break;
				case "Year":
					fieldName = allSelectValuationMethod.Year;
					break;
				case "Study Location":
					fieldName = allSelectValuationMethod.Location;
					break;
				case "Geographic Area":
					fieldName = allSelectValuationMethod.GeographicArea;
					break;
				case "Other Pollutants":
					fieldName = allSelectValuationMethod.OtherPollutants;
					break;
				case "Qualifier":
					fieldName = allSelectValuationMethod.Qualifier;
					break;

				case "Race":
					fieldName = allSelectValuationMethod.Race;
					break;
				case "Ethnicity":
					fieldName = allSelectValuationMethod.Ethnicity;
					break;
				case "Gender":
					fieldName = allSelectValuationMethod.Gender;
					break;
				case "Start Age":
					fieldName = allSelectValuationMethod.StartAge;
					break;
				case "End Age":
					fieldName = allSelectValuationMethod.EndAge;
					break;
				case "Function":
					fieldName = allSelectValuationMethod.Function;
					break;

				case "Version":
					fieldName = allSelectValuationMethod.Version;
					break;
				case "Age Range":
					fieldName = allSelectValuationMethod.AgeRange;
					break;
			}
			return fieldName;
		}

		private object getFieldNameFromlstQALYObject(string s, AllSelectQALYMethod allSelectQALYMethod, QALYValueAttribute qalyValueAttribute)
		{
			object fieldName = "";

			switch (s)
			{
				case "Column":
					fieldName = qalyValueAttribute.Col;
					break;
				case "Row":
					fieldName = qalyValueAttribute.Row;
					break;
				case "Point Estimate":
					fieldName = qalyValueAttribute.PointEstimate;
					break;

				case "Mean":
					fieldName = qalyValueAttribute.Mean;
					break;

				case "Standard Deviation":
					fieldName = qalyValueAttribute.StandardDeviation;
					break;
				case "Variance":
					fieldName = qalyValueAttribute.Variance;
					break;
				case "Dataset":
					fieldName = allSelectQALYMethod.DataSet;
					break;
				case "Endpoint Group":
					fieldName = allSelectQALYMethod.EndPointGroup;
					break;
				case "Endpoint":
					fieldName = allSelectQALYMethod.EndPoint;
					break;
				case "Pollutant":
					fieldName = allSelectQALYMethod.Pollutant;
					break;
				case "Metric":
					fieldName = allSelectQALYMethod.Metric;
					break;
				case "Seasonal Metric":
					fieldName = allSelectQALYMethod.SeasonalMetric;
					break;
				case "Metric Statistic":
					fieldName = allSelectQALYMethod.MetricStatistic;
					break;
				case "Author":
					fieldName = allSelectQALYMethod.Author;
					break;
				case "Year":
					fieldName = allSelectQALYMethod.Year;
					break;
				case "Location":
					fieldName = allSelectQALYMethod.Location;
					break;
				case "Other Pollutants":
					fieldName = allSelectQALYMethod.OtherPollutants;
					break;
				case "Qualifier":
					fieldName = allSelectQALYMethod.Qualifier;
					break;

				case "Race":
					fieldName = allSelectQALYMethod.Race;
					break;
				case "Ethnicity":
					fieldName = allSelectQALYMethod.Ethnicity;
					break;
				case "Gender":
					fieldName = allSelectQALYMethod.Gender;
					break;
				case "Start Age":
					fieldName = allSelectQALYMethod.StartAge;
					break;
				case "End Age":
					fieldName = allSelectQALYMethod.EndAge;
					break;
				case "Function":
					fieldName = allSelectQALYMethod.Function;
					break;

				case "Version":
					fieldName = allSelectQALYMethod.Version;
					break;
			}
			return fieldName;
		}

		private void InitColumnsShowSet()
		{
			if (IncidencelstHealth == null)
			{
				IncidencelstHealth = new List<FieldCheck>();
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = true });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Reference", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Incidence Dataset", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Prevalence Dataset", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta Distribution", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 1", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 2", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "A", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "A Description", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "B", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "B Description", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "C", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "C Description", isChecked = false });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = true });
				IncidencelstHealth.Add(new FieldCheck() { FieldName = "Age Range", isChecked = true });
			}
			if (cflstHealth == null)
			{
				cflstHealth = new List<FieldCheck>();
				cflstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
				cflstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
				cflstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Reference", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
				cflstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
				cflstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Incidence Dataset", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Prevalence Dataset", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Beta", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Beta Distribution", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 1", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 2", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "A", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "A Description", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "B", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "B Description", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "C", isChecked = false });
				cflstHealth.Add(new FieldCheck() { FieldName = "C Description", isChecked = false });
			}
			if (apvlstHealth == null)
			{
				apvlstHealth = new List<FieldCheck>();
				apvlstHealth.Add(new FieldCheck() { FieldName = "Name", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
				apvlstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = true });
				apvlstHealth.Add(new FieldCheck() { FieldName = "Age Range", isChecked = false }); //skip adding Age Range to Pooled Valuation Results?

			}
			if (qalylstHealth == null)
			{
				qalylstHealth = new List<FieldCheck>();
				qalylstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
				qalylstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = true });
			}
		}
		private void InitTableResult(object oTable)
		{
			try
			{

				numericUpDownResult.ValueChanged -= numericUpDownResult_ValueChanged;
				numericUpDownResult.Value = 4;
				numericUpDownResult.ValueChanged += numericUpDownResult_ValueChanged;

				OLVResultsShow.Items.Clear();
				OLVResultsShow.Columns.Clear();
				int i = 0;
				Boolean forceShowGeographicArea = false;
				//unknown
				if (oTable is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)
				{
					List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
					if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
					{
						foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.SelectedObjects)
						{
							AllSelectCRFunction cr = keyValueCR.Key;

							if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
							{
								continue;
							}
							else
							{

								lstCRSelectFunctionCalculateValue.Add(cr.CRSelectFunctionCalculateValue);
							}

						}
					}
					else
					{
						foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
						{
							lstCRSelectFunctionCalculateValue.Add(cr);
						}
						if (cbCRAggregation.SelectedIndex != -1 && cbCRAggregation.SelectedIndex != 0)
						{
							DataRowView drv = cbCRAggregation.SelectedItem as DataRowView;
							int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
							if (iAggregationGridType != CommonClass.GBenMAPGrid.GridDefinitionID)
							{
								List<CRSelectFunctionCalculateValue> lstTemp = new List<CRSelectFunctionCalculateValue>();
								foreach (CRSelectFunctionCalculateValue cr in lstCRSelectFunctionCalculateValue)
								{
									lstTemp.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(cr, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType));
								}

								lstCRSelectFunctionCalculateValue = lstTemp;
							}

						}
					}
					oTable = lstCRSelectFunctionCalculateValue;
				}
				//incidence pooling result. string is the name of the pooling window.
				if (oTable is Dictionary<AllSelectCRFunction, string>) //YY: replace List<AllSelectCRFunction> to include pooling name
				{
					//List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)oTable;
					Dictionary<AllSelectCRFunction, string> dicAllSelectCRFunctionPoolName = (Dictionary<AllSelectCRFunction, string>)oTable;
					List<AllSelectCRFunction> lstAllSelectCRFuntion = new List<AllSelectCRFunction>();

					foreach (KeyValuePair<AllSelectCRFunction, string> ascrp in dicAllSelectCRFunctionPoolName)
					{
						lstAllSelectCRFuntion.Add(ascrp.Key);
					}


					foreach (AllSelectCRFunction cf in lstAllSelectCRFuntion)
					{
						if (string.IsNullOrWhiteSpace(cf.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName) == false &&
								cf.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName.Equals(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE) == false)
						{
							forceShowGeographicArea = true;
						}
					}
					//Add columns
					//add pooling window  name column
					BrightIdeasSoftware.OLVColumn olvColumnPoolName = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "Pooling Name", IsEditable = false, Width = "Pooling Name".Length * 8 };
					OLVResultsShow.Columns.Add(olvColumnPoolName);

					if (this.IncidencelstColumnRow == null)
					{
						BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.Col", Text = "Column", IsEditable = false, Width = 8 * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
						BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.Row", Text = "Row", IsEditable = false, Width = 6 * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
					}
					else
					{
						foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
						{

							if (fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
							}
						}
					}
					if (IncidencelstHealth != null)
					{
						foreach (FieldCheck fieldCheck in IncidencelstHealth)
						{
							if (fieldCheck.FieldName.Equals("Geographic Area") && forceShowGeographicArea)
							{
								fieldCheck.isChecked = true;
							}

							else if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Value.Version", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);

							}
							else if (fieldCheck.FieldName.ToLower() == "age range" && fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Value.AgeRange", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);

							}
							else if (fieldCheck.FieldName.Equals("Geographic Area") && fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Value.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
							}
							else if (fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Value.CRSelectFunctionCalculateValue.CRSelectFunction." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
							}
						}
					}
					if (IncidencelstResult == null)
					{
						BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "Point Estimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
						if (CommonClass.IncidencePoolingAndAggregationAdvance.CalculatePooledPopulationYN)
						{
							BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
							BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.Baseline", AspectToStringFormat = "{0:N4}", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
							BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
						}

						BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
						BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);

						BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
						BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.Variance", Text = "Variance", AspectToStringFormat = "{0:N4}", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
					}
					else
					{
						foreach (FieldCheck fieldCheck in IncidencelstResult)
						{

							if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName)
							{
								BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
							}
						}
					}
					//Note: AspectName like "lstPercentile[1]" works fines in ObjectListView 2.5.0.0. However, it does not work in version 2.9.1.
					//Try adding AspectGetter if the ObjectListView is upgraded. 
					if (lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
					{
						if (strPoolIncidencePercentiles != null && strPoolIncidencePercentiles.Count > 0)
						{
							double interval = 50.0 / CommonClass.CRLatinHypercubePoints;
							i = 0;
							while (i < strPoolIncidencePercentiles.Count)
							{
								//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strPoolIncidencePercentiles[i]) / interval - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strPoolIncidencePercentiles[i].ToString(), IsEditable = false };
								BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strPoolIncidencePercentiles[i].ToString(), IsEditable = false };
								int j = i;
								olvPercentile.AspectGetter = delegate (object x)
								{
									KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string> y = (KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string>)x;
									return (y.Key.Key.Key.LstPercentile[(int)(Convert.ToDouble(strPoolIncidencePercentiles[j]) / interval - 1) / 2]);
								};
								OLVResultsShow.Columns.Add(olvPercentile);
								i++;
							}
						}
						else
						{
							i = 0;
							int pctCount = 0;
							while (i < lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())   //Not using user-assigned percentiles
							{
								if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
								{
									if (chbAllPercentiles.Checked)         //BenMAP-284: Add all percentiles if selected, otherwise only add the 2.5 and 97.5
									{
										//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; 
										BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false };
										int j = i;
										olvPercentile.AspectGetter = delegate (object x)
										{
											KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string> y = (KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string>)x;
											return (y.Key.Key.Key.LstPercentile[j]);
										};
										OLVResultsShow.Columns.Add(olvPercentile);
									}
									else
									{
										double currInterval = ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())))));

										if (currInterval == 2.5 || currInterval == 97.5)
										{
											//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + currInterval, IsEditable = false };
											BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + currInterval, IsEditable = false };
											int j = i;
											olvPercentile.AspectGetter = delegate (object x)
											{
												KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string> y = (KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string>)x;
												return (y.Key.Key.Key.LstPercentile[j]);
											};
											OLVResultsShow.Columns.Add(olvPercentile);
											pctCount++;
										}
									}
								}
								i++;
							}
							if (!chbAllPercentiles.Checked && pctCount == 0)
							{
								MessageBox.Show("Unable to locate the 2.5 and 97.5 percentile values.");
							}
						}
					}
					Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();
					int iLstCRTable = 0;
					//dicAPVNew is like dicAPV with pooling name.
					Dictionary<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string> dicAPVNew = new Dictionary<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string>();


					Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();

					//load data to each dicKey then all together to dicAPV
					foreach (AllSelectCRFunction cr in lstAllSelectCRFuntion)
					{
						foreach (CRCalculateValue crv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
						{
							dicKey = null;
							dicKey = new Dictionary<CRCalculateValue, int>();
							dicKey.Add(crv, iLstCRTable);
							dicAPV.Add(dicKey.ToList()[0], cr);
						}
						iLstCRTable++;
					}
					//YY: load data to each dicKey then all together to dicAPVNew
					//foreach (KeyValuePair<KeyValuePair<CRCalculateValue,int>,CRSelectFunction> apvp in )
					Dictionary<CRCalculateValue, int> dicKey1 = new Dictionary<CRCalculateValue, int>();
					Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicKey2 = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();
					iLstCRTable = 0;
					foreach (KeyValuePair<AllSelectCRFunction, string> dicAscrPoolName in dicAllSelectCRFunctionPoolName)
					{
						AllSelectCRFunction acr = dicAscrPoolName.Key;
						foreach (CRCalculateValue crv in acr.CRSelectFunctionCalculateValue.CRCalculateValues)
						{
							dicKey1 = new Dictionary<CRCalculateValue, int>();
							dicKey2 = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();
							dicKey1.Add(crv, iLstCRTable);
							dicKey2.Add(dicKey1.ToList()[0], acr);
							dicAPVNew.Add(dicKey2.ToList()[0], dicAscrPoolName.Value);
						}
						iLstCRTable++;

					}





					//_tableObject = lstAllSelectCRFuntion;
					_tableObject = dicAllSelectCRFunctionPoolName; //YY: change to divAPV so that pooling name can be kept after sorting.
					_currentRow = 0;
					//_pageCount = dicAPV.Count / 50 + 1; _pageCurrent = 1;
					_pageCount = dicAPVNew.Count / 50 + 1; _pageCurrent = 1;
					//SetOLVResultsShowObjects(dicAPV);
					SetOLVResultsShowObjects(dicAPVNew);

					bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
					bindingNavigatorCountItem.Text = _pageCount.ToString();
				}
				//Health Impact Results
				if (oTable is List<CRSelectFunctionCalculateValue> || oTable is CRSelectFunctionCalculateValue)
				{

					List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
					if (oTable is List<CRSelectFunctionCalculateValue>)
						lstCRTable = (List<CRSelectFunctionCalculateValue>)oTable;
					else
						lstCRTable.Add(oTable as CRSelectFunctionCalculateValue);
					for (int iCR = 0; iCR < lstCRTable.Count; iCR++)
					{
						CRSelectFunctionCalculateValue cr = lstCRTable[iCR];
						cr.CRCalculateValues = cr.CRCalculateValues.Where(p => p != null).OrderBy(p => p.Col).ToList();
						if (string.IsNullOrWhiteSpace(cr.CRSelectFunction.GeographicAreaName) == false && cr.CRSelectFunction.GeographicAreaName.Equals(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE) == false)
						{
							forceShowGeographicArea = true;
						}
					}
					if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
					{
						if (this.IncidencelstColumnRow == null)
						{
							BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Col", Text = "Column", IsEditable = false, Width = 8 * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
							BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Row", Text = "Row", IsEditable = false, Width = 6 * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
						}
						else
						{
							foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
							{

								if (fieldCheck.isChecked)
								{
									BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
								}
							}
						}

						if (IncidencelstHealth != null)
						{
							foreach (FieldCheck fieldCheck in IncidencelstHealth)
							{

								if (fieldCheck.isChecked)
								{
									BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
								}
							}
						}
						if (IncidencelstResult == null)
						{
							BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "PointEstimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
							BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
							BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
							BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
							BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", AspectToStringFormat = "{0:N4}", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
							BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
							BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
							BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", Text = "Variance", AspectToStringFormat = "{0:N4}", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
						}
						else
						{
							foreach (FieldCheck fieldCheck in IncidencelstResult)
							{

								if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName)
								{
									BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
								}
							}
						}
						if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
						{
							if (strPoolIncidencePercentiles != null && strPoolIncidencePercentiles.Count > 0)
							{
								double interval = 50.0 / CommonClass.CRLatinHypercubePoints;
								i = 0;
								while (i < strPoolIncidencePercentiles.Count)
								{
									//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strPoolIncidencePercentiles[i]) / interval - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strPoolIncidencePercentiles[i].ToString(), IsEditable = false };
									BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strPoolIncidencePercentiles[i].ToString(), IsEditable = false };
									int j = i;
									olvPercentile.AspectGetter = delegate (object x)
									{
										KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string> y = (KeyValuePair<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string>)x;
										return (y.Key.Key.Key.LstPercentile[(int)(Convert.ToDouble(strPoolIncidencePercentiles[j]) / interval - 1) / 2]);
									};
									OLVResultsShow.Columns.Add(olvPercentile);
									i++;
								}
							}
							else
							{
								i = 0;
								while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
								{
									if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
									{
										//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false };
										BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false };
										int j = i;
										olvPercentile.AspectGetter = delegate (object x)
										{
											KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> y = (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)x;
											return (y.Key.Key.LstPercentile[j]);
										};
										OLVResultsShow.Columns.Add(olvPercentile);

										i++;
									}
									i++;
								}
							}
						}
					}
					else
					{
						foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
						{
							cr.CRCalculateValues = cr.CRCalculateValues.Where(p => p.Population > 0).ToList();
						}
						if (cflstColumnRow == null)
						{
							BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Col", Text = "Column", IsEditable = false, Width = "Columnnn".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
							BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Row", Text = "Row", IsEditable = false, Width = "Rowww".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
						}
						else
						{
							foreach (FieldCheck fieldCheck in cflstColumnRow)
							{

								if (fieldCheck.isChecked)
								{
									BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
								}
							}
						}
						if (cflstHealth != null)
						{
							foreach (FieldCheck fieldCheck in cflstHealth)
							{
								if (fieldCheck.FieldName.Equals("Geographic Area") && forceShowGeographicArea)
								{
									fieldCheck.isChecked = true;
								}
								if (fieldCheck.isChecked)
								{
									BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
								}
							}
						}
						if (cflstResult == null)
						{
							BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "PointEstimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
							BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
							BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
							BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
							BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", Text = "Baseline", AspectToStringFormat = "{0:N4}", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
							BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
							BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
							BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", AspectToStringFormat = "{0:N4}", Text = "Variance", IsEditable = false, Width = "Variance".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnVariance);
						}
						else
						{
							foreach (FieldCheck fieldCheck in cflstResult)
							{

								if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
										&& fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
								{
									BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
								}
							}
						}
						if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
						{
							if (strHealthImpactPercentiles != null && strHealthImpactPercentiles.Count > 0)
							{
								double interval = 50.0 / CommonClass.CRLatinHypercubePoints;
								i = 0;
								while (i < strHealthImpactPercentiles.Count)
								{
									//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strHealthImpactPercentiles[i]) / interval - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strHealthImpactPercentiles[i].ToString(), IsEditable = false }; 
									BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strHealthImpactPercentiles[i].ToString(), IsEditable = false };
									int j = i;
									olvPercentile.AspectGetter = delegate (object x)
									{
										KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> y = (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)x;
										return (y.Key.Key.LstPercentile[(int)(Convert.ToDouble(strHealthImpactPercentiles[j]) / interval - 1) / 2]);
									};
									OLVResultsShow.Columns.Add(olvPercentile);
									i++;
								}
							}
							else
							{
								i = 0;
								int pctCount = 0;
								while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
								{
									if (cflstResult == null || cflstResult.Last().isChecked)
									{
										if (chbAllPercentiles.Checked)
										{
											//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() {AspectName= "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{ 0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + (Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())).ToString(), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
											BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + (Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())).ToString(), IsEditable = false };
											int j = i;
											olvPercentile.AspectGetter = delegate (object x)
											{
												KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> y = (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)x;
												return (y.Key.Key.LstPercentile[j]);
											};
											OLVResultsShow.Columns.Add(olvPercentile);

										}
										else
										{
											double currInterval = ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())))));

											if (currInterval == 2.5 || currInterval == 97.5)
											{
												//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + currInterval, IsEditable = false };
												BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + currInterval, IsEditable = false };
												int j = i;
												olvPercentile.AspectGetter = delegate (object x)
												{
													//KeyValuePair<QALYValueAttribute, AllSelectQALYMethod> y = (KeyValuePair<QALYValueAttribute, AllSelectQALYMethod>)x;
													//return (y.Key.LstPercentile[j]);
													KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> y = (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)x;
													return (y.Key.Key.LstPercentile[j]);
												};
												OLVResultsShow.Columns.Add(olvPercentile);
												pctCount++;
											}
										}
									}
									i++;
								}

								if (!chbAllPercentiles.Checked && pctCount == 0)
								{
									MessageBox.Show("Unable to locate the 2.5 and 97.5 percentile values.");
								}
							}
						}
					}
					Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();
					int iLstCRTable = 0;

					Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
					if (cflstResult != null && cflstResult.Where(p => p.FieldName == "Population Weighted Delta").Count() == 1 && this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() != "incidence")
					{
						if (cflstResult.Where(p => p.FieldName == "Population Weighted Delta").First().isChecked == true)
						{
							foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
							{
								dicKey = null;
								dicKey = new Dictionary<CRCalculateValue, int>();
								CRCalculateValue crv = new CRCalculateValue();
								crv.PointEstimate = cr.CRCalculateValues.Sum(p => p.Population * p.Delta) / cr.CRCalculateValues.Sum(p => p.Population);
								if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
								{
									crv.LstPercentile = new List<float>();
									foreach (float f in cr.CRCalculateValues.First().LstPercentile)
									{
										crv.LstPercentile.Add(0);
									}
								}
								dicKey.Add(crv, iLstCRTable);
								CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
								crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Delta";
								dicAPV.Add(dicKey.ToList()[0], crNew);
								iLstCRTable++;
							}
						}
						if (cflstResult.Where(p => p.FieldName == "Population Weighted Base").First().isChecked == true)
						{
							foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
							{
								dicKey = null;
								dicKey = new Dictionary<CRCalculateValue, int>();
								CRCalculateValue crv = new CRCalculateValue();
								CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
								BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Base;
								Dictionary<string, float> dicBase = new Dictionary<string, float>();
								string strMetric = "";
								if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
								{
									strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
								}
								else
									strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
								foreach (ModelResultAttribute mr in benMAPLineBase.ModelResultAttributes)
								{
									dicBase.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

								}
								float dPointEstimate = 0;
								foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
								{
									if (dicBase.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
									{
										dPointEstimate = dPointEstimate + dicBase[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
									}
								}
								crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
								if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
								{
									crv.LstPercentile = new List<float>();
									foreach (float f in cr.CRCalculateValues.First().LstPercentile)
									{
										crv.LstPercentile.Add(0);
									}
								}
								dicKey.Add(crv, iLstCRTable);

								crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Base";
								dicAPV.Add(dicKey.ToList()[0], crNew);
								iLstCRTable++;
							}
						}
						if (cflstResult.Where(p => p.FieldName == "Population Weighted Control").First().isChecked == true)
						{
							foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
							{
								dicKey = null;
								dicKey = new Dictionary<CRCalculateValue, int>();
								CRCalculateValue crv = new CRCalculateValue();
								CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
								BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Control;
								Dictionary<string, float> dicControl = new Dictionary<string, float>();
								string strMetric = "";
								if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
								{
									strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
								}
								else
									strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
								foreach (ModelResultAttribute mr in benMAPLineControl.ModelResultAttributes)
								{
									dicControl.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

								}
								float dPointEstimate = 0;
								foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
								{
									if (dicControl.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
									{
										dPointEstimate = dPointEstimate + dicControl[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
									}
								}
								crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
								if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
								{
									crv.LstPercentile = new List<float>();
									foreach (float f in cr.CRCalculateValues.First().LstPercentile)
									{
										crv.LstPercentile.Add(0);
									}
								}
								dicKey.Add(crv, iLstCRTable);

								crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Control";
								dicAPV.Add(dicKey.ToList()[0], crNew);
								iLstCRTable++;
							}
						}
					}
					foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
					{
						foreach (CRCalculateValue crv in cr.CRCalculateValues)
						{
							dicKey = null;
							dicKey = new Dictionary<CRCalculateValue, int>();
							dicKey.Add(crv, iLstCRTable);
							dicAPV.Add(dicKey.ToList()[0], cr.CRSelectFunction);
						}
						iLstCRTable++;
					}
					_tableObject = lstCRTable;
					_currentRow = 0;
					_pageCount = dicAPV.Count / 50 + 1; _pageCurrent = 1;
					SetOLVResultsShowObjects(dicAPV);
					bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
					bindingNavigatorCountItem.Text = _pageCount.ToString();
				}
				//Valuation Results
				else if (oTable is List<AllSelectValuationMethodAndValue> || oTable is AllSelectValuationMethodAndValue) //Pooled Valuation Results
				{
					List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
					if (oTable is List<AllSelectValuationMethodAndValue>)
					{
						lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)oTable;
					}
					else
					{
						lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)oTable);
					}
					for (int iValuation = 0; iValuation < lstallSelectValuationMethodAndValue.Count; iValuation++)
					{
						lstallSelectValuationMethodAndValue[iValuation].lstAPVValueAttributes = lstallSelectValuationMethodAndValue[iValuation].lstAPVValueAttributes.OrderBy(p => p.Col).ToList();

						if (string.IsNullOrWhiteSpace(lstallSelectValuationMethodAndValue[iValuation].AllSelectValuationMethod.GeographicArea) == false &&
							 lstallSelectValuationMethodAndValue[iValuation].AllSelectValuationMethod.GeographicArea.Equals(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE) == false)
						{
							forceShowGeographicArea = true;
						}
					}
					if (apvlstColumnRow == null)
					{
						BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Col", Text = "Column", IsEditable = false, Width = "Columnnn".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
						BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Row", Text = "Row", IsEditable = false, Width = "Rowww".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
					}
					else
					{
						foreach (FieldCheck fieldCheck in apvlstColumnRow)
						{

							if (fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
							}
						}

					}
					if (apvlstHealth != null)
					{
						foreach (FieldCheck fieldCheck in apvlstHealth)
						{
							if (fieldCheck.FieldName.Equals("Geographic Area") && forceShowGeographicArea)
							{
								fieldCheck.isChecked = true;
							}

							if (fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
							}
						}

					}
					if (apvlstResult == null)
					{
						BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "Point Estimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
						BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
						BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
						BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", AspectToStringFormat = "{0:N4}", Text = "Variance", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
					}
					else
					{
						foreach (FieldCheck fieldCheck in apvlstResult)
						{

							if (fieldCheck.FieldName != apvlstResult.Last().FieldName && fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
							}
						}

					}
					if (apvlstResult == null || apvlstResult.Last().isChecked)
					{
						if (strAPVPercentiles != null && strAPVPercentiles.Count > 0)
						{
							i = 0;
							while (i < strAPVPercentiles.Count)
							{
								//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strAPVPercentiles[i]) / 0.5 - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strAPVPercentiles[i].ToString(), IsEditable = false }; 
								BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strAPVPercentiles[i].ToString(), IsEditable = false };
								int j = i;
								olvPercentile.AspectGetter = delegate (object x)
								{
									KeyValuePair<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod> y = (KeyValuePair<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod>)x;
									return (y.Key.Key.LstPercentile[(int)(Convert.ToDouble(strAPVPercentiles[j]) / 0.5 - 1) / 2]);
								};

								OLVResultsShow.Columns.Add(olvPercentile);
								i++;
							}
						}
						else
						{
							i = 0;
							int pctCount = 0;
							if (lstallSelectValuationMethodAndValue != null && lstallSelectValuationMethodAndValue.Count > 0
									&& lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes != null
									&& lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Count > 0
									&& lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
							{

								while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
								{
									if (chbAllPercentiles.Checked)
									{
										//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()))))), IsEditable = false };
										BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()))))), IsEditable = false };
										int j = i;
										olvPercentile.AspectGetter = delegate (object x)
										{
											KeyValuePair<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod> y = (KeyValuePair<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod>)x;
											return (y.Key.Key.LstPercentile[j]);
										};
										OLVResultsShow.Columns.Add(olvPercentile);
									}
									else
									{
										double currInterval = ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())))));

										if (currInterval == 2.5 || currInterval == 97.5)
										{
											//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + currInterval, IsEditable = false };
											BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + currInterval, IsEditable = false };
											int j = i;
											olvPercentile.AspectGetter = delegate (object x)
											{
												KeyValuePair<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod> y = (KeyValuePair<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod>)x;
												return (y.Key.Key.LstPercentile[j]);
											};

											OLVResultsShow.Columns.Add(olvPercentile);
											pctCount++;
										}
									}
									i++;
								}
								if (!chbAllPercentiles.Checked && pctCount == 0)
								{
									MessageBox.Show("Unable to locate the 2.5 and 97.5 percentile values.");
								}
							}
						}
					}
					_tableObject = lstallSelectValuationMethodAndValue;
					Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod> dicAPV = new Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod>();
					Dictionary<APVValueAttribute, int> dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
					int ilstallSelectValuationMethodAndValue = 0;
					foreach (AllSelectValuationMethodAndValue allSelectValuationMethodAndValue in lstallSelectValuationMethodAndValue)
					{
						foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
						{
							dicLstAllSelectValuationMethodAndValue = null;
							dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
							dicLstAllSelectValuationMethodAndValue.Add(apvx, ilstallSelectValuationMethodAndValue);
							dicAPV.Add(dicLstAllSelectValuationMethodAndValue.ToList()[0], allSelectValuationMethodAndValue.AllSelectValuationMethod);
						}
						ilstallSelectValuationMethodAndValue++;
					}

					_currentRow = 0;
					_pageCount = dicAPV.Count / 50 + 1;
					_pageCurrent = 1;
					SetOLVResultsShowObjects(dicAPV);
					bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
					bindingNavigatorCountItem.Text = _pageCount.ToString();
				}
				//QALY Results
				else if (oTable is List<AllSelectQALYMethodAndValue> || oTable is AllSelectQALYMethodAndValue)
				{
					List<AllSelectQALYMethodAndValue> lstallSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
					if (oTable is List<AllSelectQALYMethodAndValue>)
					{
						lstallSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)oTable;
					}
					else
					{
						lstallSelectQALYMethodAndValue.Add((AllSelectQALYMethodAndValue)oTable);
					}
					for (int iQALY = 0; iQALY < lstallSelectQALYMethodAndValue.Count; iQALY++)
					{
						lstallSelectQALYMethodAndValue[iQALY].lstQALYValueAttributes = lstallSelectQALYMethodAndValue[iQALY].lstQALYValueAttributes.OrderBy(p => p.Col).ToList();
					}
					if (qalylstColumnRow == null)
					{
						BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Col", Text = "Column", IsEditable = false, Width = "Columnnn".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
						BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Row", Text = "Row", IsEditable = false, Width = "Rowww".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
					}
					else
					{
						foreach (FieldCheck fieldCheck in qalylstColumnRow)
						{

							if (fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnQALYID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnQALYID);
							}
						}

					}
					if (qalylstHealth != null)
					{
						foreach (FieldCheck fieldCheck in qalylstHealth)
						{

							if (fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnQALYID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnQALYID);
							}
						}

					}
					if (qalylstResult == null)
					{
						BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "Point Estimatee".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
						BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
						BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviationn".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
						BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Variance", AspectToStringFormat = "{0:N4}", Text = "Variance", Width = "Variancee".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
					}
					else
					{
						foreach (FieldCheck fieldCheck in qalylstResult)
						{

							if (fieldCheck.FieldName != qalylstResult.Last().FieldName && fieldCheck.isChecked)
							{
								BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
							}
						}

					}
					if (qalylstResult == null || qalylstResult.Last().isChecked)
					{
						i = 0;
						if (lstallSelectQALYMethodAndValue != null && lstallSelectQALYMethodAndValue.Count > 0 && lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes != null
								&& lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.Count > 0
								&& lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
						{
							while (i < lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
							{

								//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
								BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), IsEditable = false };
								int j = i;
								olvPercentile.AspectGetter = delegate (object x)
								{
									KeyValuePair<QALYValueAttribute, AllSelectQALYMethod> y = (KeyValuePair<QALYValueAttribute, AllSelectQALYMethod>)x;
									return (y.Key.LstPercentile[j]);
								};
								OLVResultsShow.Columns.Add(olvPercentile);

								i++;
							}
						}
					}
					_tableObject = lstallSelectQALYMethodAndValue;
					Dictionary<QALYValueAttribute, AllSelectQALYMethod> dicAPV = new Dictionary<QALYValueAttribute, AllSelectQALYMethod>();
					foreach (AllSelectQALYMethodAndValue allSelectQALYMethodAndValue in lstallSelectQALYMethodAndValue)
					{
						foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
						{
							dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod);
						}
					}

					_currentRow = 0;
					_pageCount = dicAPV.Count / 50 + 1;
					_pageCurrent = 1;
					SetOLVResultsShowObjects(dicAPV);
					bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
					bindingNavigatorCountItem.Text = _pageCount.ToString();
				}
				//AQ data
				else if (oTable is BenMAPLine)
				{
					BenMAPLine crTable = (BenMAPLine)oTable;
					crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Col).ToList();
					BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Col", Text = "Column", IsEditable = false, Width = "Columnss".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
					BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Row", Text = "Row", IsEditable = false, Width = "Rowss".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
					List<string> lstAddField = new List<string>();
					List<double[]> lstResultCopy = new List<double[]>();
					if (crTable.Pollutant.Metrics != null)
					{
						foreach (Metric metric in crTable.Pollutant.Metrics)
						{
							lstAddField.Add(metric.MetricName);
						}
					}
					if (crTable.Pollutant.SesonalMetrics != null)
					{
						foreach (SeasonalMetric sesonalMetric in crTable.Pollutant.SesonalMetrics)
						{
							lstAddField.Add(sesonalMetric.SeasonalMetricName);
						}
					}

					i = 0;
					while (i < lstAddField.Count())
					{
						//BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Values[" + lstAddField[i] + "]", AspectToStringFormat = "{0:N2}", Width = lstAddField[i].Length * 9, Text = lstAddField[i], IsEditable = false };
						BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectToStringFormat = "{0:N2}", Width = lstAddField[i].Length * 9, Text = lstAddField[i], IsEditable = false };
						int j = i;
						olvPercentile.AspectGetter = delegate (object x)
						{
							ModelResultAttribute y = (ModelResultAttribute)x;
							float returnVal = 0;
							y.Values.TryGetValue(olvPercentile.Text, out returnVal);
							return returnVal;
						};
						OLVResultsShow.Columns.Add(olvPercentile);


						i++;
					}

					_tableObject = crTable;
					_currentRow = 0;
					_pageCount = crTable.ModelResultAttributes.Count / 50 + 1;
					_pageCurrent = 1;
					SetOLVResultsShowObjects(crTable.ModelResultAttributes);
					bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
					bindingNavigatorCountItem.Text = _pageCount.ToString();
				}
			}
			catch (Exception ex)
			{
			}
		}

		private void InitChartResult(object oTable, int GridID)
		{
			try
			{
				oxyPlotView.Visible = true;
				olvRegions.Visible = true;
				cbGraph.Visible = true;
				btnApply.Visible = true;
				groupBox9.Visible = true;
				groupBox1.Visible = true;
				btnSelectAll.Visible = true;
				Dictionary<string, double> dicValue = new Dictionary<string, double>();
				GridRelationship gRegionGridRelationship = null;
				List<List<RowCol>> lstlstRowCol = null;
				List<string> lstString = null;
				List<RowCol> lstRowCol = null;
				int iRowCount = 0;
				double d = 0;
				// GraphPane myPane = this.oxyPlotView.GraphPane;
				PlotModel myPane = new PlotModel();
				DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
				bool showOriginalGrid;
				if (tabCtlReport.SelectedTab == tabPoolingIncidence || tabCtlReport.SelectedTab == tabAPVResultGISShow)
					showOriginalGrid = true;
				else
					showOriginalGrid = false;
				if (!showOriginalGrid)
				{
					if (CommonClass.RBenMAPGrid == null)
					{
						CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
					}
					if (GridID != CommonClass.RBenMAPGrid.GridDefinitionID)
					{
						foreach (GridRelationship gr in CommonClass.LstGridRelationshipAll)
						{
							if (gr.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.smallGridID == GridID)
							{
								gRegionGridRelationship = gr;
							}
							else if (gr.smallGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.bigGridID == GridID)
							{
								gRegionGridRelationship = gr;
							}
						}
					}
				}
				if (oTable is CRSelectFunctionCalculateValue)
				{
					CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)oTable;
					if (!showOriginalGrid && CommonClass.RBenMAPGrid.GridDefinitionID != GridID)
						crTable = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crTable, GridID, CommonClass.RBenMAPGrid.GridDefinitionID);
					foreach (CRCalculateValue crv in crTable.CRCalculateValues)
					{
						if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
							dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
					}

					strCDFTitle = "Health Impact Results";
					strCDFX = "Health Impact";
					strCDFY = "Cumulative Percentage of Health Impact";
					strchartTitle = "Incidence";
					strchartX = "Region";
					strchartY = "Health Impact";
				}
				else if (oTable is AllSelectValuationMethodAndValue)
				{
					AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = (AllSelectValuationMethodAndValue)oTable;
					foreach (APVValueAttribute crv in allSelectValuationMethodAndValue.lstAPVValueAttributes)
					{
						if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
							dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
					}

					strCDFTitle = "Monetized Benefits";
					strCDFX = "Monetized Benefits";
					strCDFY = "Cumulative Percentage of Monetized Benefits";
					strchartTitle = "Pooled Valuation";
					strchartX = "Region";
					strchartY = "Valuation($)";
				}
				else if (oTable is List<AllSelectValuationMethodAndValue>)
				{
					List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)oTable;
					AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.First(); foreach (APVValueAttribute crv in allSelectValuationMethodAndValue.lstAPVValueAttributes)
					{
						if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
							dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
					}

					strCDFTitle = "Monetized Benefits";
					strCDFX = "Monetized Benefits";
					strCDFY = "Cumulative Percentage of Monetized Benefits";
					strchartTitle = "Pooled Valuation";
					strchartX = "Region";
					strchartY = "Valuation($)";
				}
				if (canshowCDF)
				{
					cbGraph.Enabled = true;
				}
				else
				{
					cbGraph.SelectedIndex = 0;
					cbGraph.Enabled = false;
				}
				if (cbGraph.Text == "") cbGraph.SelectedIndex = 0;
				if (showOriginalGrid)
				{
					ChartGrid = Grid.GridCommon.getBenMAPGridFromID(GridID);
				}
				else
					ChartGrid = CommonClass.RBenMAPGrid;
				if (CommonClass.lstChartResult.Count > 5) iRowCount = 5;
				else iRowCount = CommonClass.lstChartResult.Count;
				if (dicValue.Count == 0) return;
				int i = 0;
				if (1 == 1)
				{
					i = 0;
					while (i < CommonClass.lstChartResult.Count)
					{
						if (dicValue.Keys.Contains(CommonClass.lstChartResult[i].Col + "," + CommonClass.lstChartResult[i].Row))
						{
							CommonClass.lstChartResult[i].RegionValue = dicValue[CommonClass.lstChartResult[i].Col + "," + CommonClass.lstChartResult[i].Row];
						}
						else
							CommonClass.lstChartResult[i].RegionValue = 0;
						i++;
					}
				}
				else if (CommonClass.RBenMAPGrid.GridDefinitionID == gRegionGridRelationship.bigGridID)
				{
					foreach (ChartResult cr in CommonClass.lstChartResult)
					{
						cr.RegionValue = 0;
					}
					IEnumerable<KeyValuePair<string, double>> IDicValue = null;
					ChartResult crResult = null;
					Dictionary<string, double> dicSum = new Dictionary<string, double>();
					foreach (GridRelationshipAttribute gridRelationshipAttribute in gRegionGridRelationship.lstGridRelationshipAttribute)
					{
						dicSum.Add(gridRelationshipAttribute.bigGridRowCol.Col + "," + gridRelationshipAttribute.bigGridRowCol.Row, 0.0);
					}
					foreach (GridRelationshipAttribute gridRelationshipAttribute in gRegionGridRelationship.lstGridRelationshipAttribute)
					{
						try
						{
							foreach (RowCol rc in gridRelationshipAttribute.smallGridRowCol)
							{
								if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
								{
									dicSum[gridRelationshipAttribute.bigGridRowCol.Col + "," + gridRelationshipAttribute.bigGridRowCol.Row] += dicValue[rc.Col + "," + rc.Row];
								}
							}
						}
						catch
						{
						}
					}
					foreach (ChartResult chartResult in CommonClass.lstChartResult)
					{
						if (dicSum.Keys.Contains(chartResult.Col + "," + chartResult.Row))
							chartResult.RegionValue = dicSum[chartResult.Col + "," + chartResult.Row];
					}

				}
				else
				{
					i = 0;
					while (i < CommonClass.lstChartResult.Count)
					{
						var query = from a in gRegionGridRelationship.lstGridRelationshipAttribute
												where a.smallGridRowCol.Contains(new RowCol()
												{
													Col = CommonClass.lstChartResult[i].Col
														,
													Row = CommonClass.lstChartResult[i].Row
												}, new RowColComparer())
												select a.bigGridRowCol;
						if (query != null && query.Count() > 0)
						{
							RowCol rc = query.First();
							try
							{
								if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
								{
									CommonClass.lstChartResult[i].RegionValue = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
								}
								else
								{
									CommonClass.lstChartResult[i].RegionValue = 0.00;
								}
							}
							catch (Exception ex)
							{ }
						}
						i++;
					}
				}

				List<string> lstPane = new List<string>();

				olvRegions.SetObjects(CommonClass.lstChartResult);

				olvRegions.MultiSelect = true;
				if (CommonClass.lstChartResult.Count > 5)
				{
					lstPane = CommonClass.lstChartResult.Select(p => p.RegionName).ToList().GetRange(0, 5);

					for (int j = 0; j < 5; j++)
					{
						OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
						olvi.Checked = true;

					}
				}
				else
				{
					lstPane = CommonClass.lstChartResult.Select(p => p.RegionName).ToList();

					for (int j = 0; j < CommonClass.lstChartResult.Count; j++)
					{
						OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
						olvi.Checked = true;

					}
				}







				cbChartXAxis.Items.Clear();
				string shapefilename = "";
				if (ChartGrid is ShapefileGrid)
					shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as ShapefileGrid).ShapefileName + ".shp";
				else if (CommonClass.RBenMAPGrid is RegularGrid)
					shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as RegularGrid).ShapefileName + ".shp";
				DotSpatial.Data.IFeatureSet fs = new DotSpatial.Data.FeatureSet();
				if (File.Exists(shapefilename))
				{
					int iName = -1;
					int icName = -1;
					int c = 0;
					cbChartXAxis.Items.Add("Col/Row");
					fs = DotSpatial.Data.FeatureSet.Open(shapefilename);
					foreach (DataColumn dc in fs.DataTable.Columns)
					{
						if (dc.ColumnName.ToLower().Trim() != "col" && dc.ColumnName.ToLower().Trim() != "row")
						{
							cbChartXAxis.Items.Add(dc.ColumnName);
							if (dc.ColumnName.ToLower().Trim() == "name")
							{
								iName = c;
							}
							else if (dc.ColumnName.ToLower().Contains("name"))
							{
								icName = c;
							}
						}
						c++;
					}
					if (iName != -1)
					{
						cbChartXAxis.Text = fs.DataTable.Columns[iName].ColumnName;
					}
					else if (icName != -1)
					{
						cbChartXAxis.Text = fs.DataTable.Columns[icName].ColumnName;
					}
					else
					{
						cbChartXAxis.Text = "Col/Row";
					}
					chartXAxis = cbChartXAxis.Text;
					fs.Close();
					fs.Dispose();
				}

				if (cbGraph.Text == "Cumulative Distribution Functions")
					cbGraph_SelectedIndexChanged(null, null);

			}
			catch (Exception ex)
			{
			}
		}

		private static IQueryable GetPage(IQueryable query, int page, int pageSize, out int count)
		{
			var skip = (page - 1) * pageSize;
			dynamic dynamicQuery = query;
			count = Queryable.Count(dynamicQuery);
			return Queryable.Take(Queryable.Skip(dynamicQuery, skip), pageSize);
		}

		private void SetOLVResultsShowObjects(IEnumerable results)
		{
			_lastResult = results;

			IQueryable curPage;
			if (results == null)
			{
				curPage = null;
			}
			else
			{
				int cnt;
				curPage = GetPage(results.AsQueryable(), _pageCurrent, _pageSize, out cnt);
			}

			OLVResultsShow.SetObjects(curPage);
		}

		private void UpdateTableResult(object oTable)
		{

			//Incidence Pooling (old and obsolete)
			if (oTable is List<AllSelectCRFunction>)
			{
				List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)oTable;

				Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>(); int iLstCRTable = 0;
				Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();

				foreach (AllSelectCRFunction cr in lstAllSelectCRFuntion)
				{
					foreach (CRCalculateValue crv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
					{
						dicKey = null;
						dicKey = new Dictionary<CRCalculateValue, int>();
						dicKey.Add(crv, iLstCRTable);
						dicAPV.Add(dicKey.ToList()[0], cr);
					}
					iLstCRTable++;
				}
				_tableObject = lstAllSelectCRFuntion;
				SetOLVResultsShowObjects(dicAPV);
			}
			//Incidence Pooling (new) YY:
			if (oTable is Dictionary<AllSelectCRFunction, string>)
			{
				Dictionary<AllSelectCRFunction, string> dicAllSelectCRFunctionPoolName = (Dictionary<AllSelectCRFunction, string>)oTable;


				Dictionary<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string> dicAPVNew = new Dictionary<KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>, string>();
				Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
				List<AllSelectCRFunction> lstAllSelectCRFuntion = new List<AllSelectCRFunction>();



				foreach (KeyValuePair<AllSelectCRFunction, string> ascrp in dicAllSelectCRFunctionPoolName)
				{
					lstAllSelectCRFuntion.Add(ascrp.Key);
				}
				int iLstCRTable = 0;
				//YY: load data to each dicKey then all together to dicAPVNew
				Dictionary<CRCalculateValue, int> dicKey1 = new Dictionary<CRCalculateValue, int>();
				Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicKey2 = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();
				iLstCRTable = 0;
				foreach (KeyValuePair<AllSelectCRFunction, string> dicAscrPoolName in dicAllSelectCRFunctionPoolName)
				{
					AllSelectCRFunction acr = dicAscrPoolName.Key;
					foreach (CRCalculateValue crv in acr.CRSelectFunctionCalculateValue.CRCalculateValues)
					{
						dicKey1 = new Dictionary<CRCalculateValue, int>();
						dicKey2 = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();
						dicKey1.Add(crv, iLstCRTable);
						dicKey2.Add(dicKey1.ToList()[0], acr);
						dicAPVNew.Add(dicKey2.ToList()[0], dicAscrPoolName.Value);
					}
					iLstCRTable++;
				}

				_tableObject = dicAllSelectCRFunctionPoolName;
				SetOLVResultsShowObjects(dicAPVNew);
			}
			//HIF
			if (oTable is List<CRSelectFunctionCalculateValue> || oTable is CRSelectFunctionCalculateValue)
			{

				List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
				if (oTable is List<CRSelectFunctionCalculateValue>)
					lstCRTable = (List<CRSelectFunctionCalculateValue>)oTable;
				else
					lstCRTable.Add(oTable as CRSelectFunctionCalculateValue);
				Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>(); int iLstCRTable = 0;
				Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
				if (cflstResult != null && cflstResult.Where(p => p.FieldName == "Population Weighted Delta").Count() == 1 && this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() != "incidence")
				{
					if (cflstResult.Where(p => p.FieldName == "Population Weighted Delta").First().isChecked == true)
					{
						foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
						{
							dicKey = null;
							dicKey = new Dictionary<CRCalculateValue, int>();
							CRCalculateValue crv = new CRCalculateValue();
							crv.PointEstimate = cr.CRCalculateValues.Sum(p => p.Population * p.Delta) / cr.CRCalculateValues.Sum(p => p.Population);
							if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
							{
								crv.LstPercentile = new List<float>();
								foreach (float f in cr.CRCalculateValues.First().LstPercentile)
								{
									crv.LstPercentile.Add(0);
								}
							}
							dicKey.Add(crv, iLstCRTable);
							CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
							crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Delta";
							dicAPV.Add(dicKey.ToList()[0], crNew);
							iLstCRTable++;
						}
					}
					if (cflstResult.Where(p => p.FieldName == "Population Weighted Base").First().isChecked == true)
					{
						foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
						{
							dicKey = null;
							dicKey = new Dictionary<CRCalculateValue, int>();
							CRCalculateValue crv = new CRCalculateValue();
							CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
							BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Base;
							Dictionary<string, float> dicBase = new Dictionary<string, float>();
							string strMetric = "";
							if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
							{
								strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
							}
							else
								strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
							foreach (ModelResultAttribute mr in benMAPLineBase.ModelResultAttributes)
							{
								dicBase.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

							}
							float dPointEstimate = 0;
							foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
							{
								if (dicBase.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
								{
									dPointEstimate = dPointEstimate + dicBase[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
								}
							}
							crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
							if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
							{
								crv.LstPercentile = new List<float>();
								foreach (float f in cr.CRCalculateValues.First().LstPercentile)
								{
									crv.LstPercentile.Add(0);
								}
							}
							dicKey.Add(crv, iLstCRTable);

							crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Base";
							dicAPV.Add(dicKey.ToList()[0], crNew);
							iLstCRTable++;
						}
					}
					if (cflstResult.Where(p => p.FieldName == "Population Weighted Control").First().isChecked == true)
					{
						foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
						{
							dicKey = null;
							dicKey = new Dictionary<CRCalculateValue, int>();
							CRCalculateValue crv = new CRCalculateValue();
							CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
							BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Control;
							Dictionary<string, float> dicControl = new Dictionary<string, float>();
							string strMetric = "";
							if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
							{
								strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
							}
							else
								strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
							foreach (ModelResultAttribute mr in benMAPLineControl.ModelResultAttributes)
							{
								dicControl.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

							}
							float dPointEstimate = 0;
							foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
							{
								if (dicControl.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
								{
									dPointEstimate = dPointEstimate + dicControl[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
								}
							}
							crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
							if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
							{
								crv.LstPercentile = new List<float>();
								foreach (float f in cr.CRCalculateValues.First().LstPercentile)
								{
									crv.LstPercentile.Add(0);
								}
							}
							dicKey.Add(crv, iLstCRTable);

							crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Control";
							dicAPV.Add(dicKey.ToList()[0], crNew);
							iLstCRTable++;
						}

					}
				}
				foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
				{
					foreach (CRCalculateValue crv in cr.CRCalculateValues)
					{
						dicKey = null;
						dicKey = new Dictionary<CRCalculateValue, int>();
						dicKey.Add(crv, iLstCRTable);
						dicAPV.Add(dicKey.ToList()[0], cr.CRSelectFunction);
					}
					iLstCRTable++;
				}


				SetOLVResultsShowObjects(dicAPV);
			}
			//unknown
			else if (oTable is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)
			{
				Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = oTable as Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>;
				SetOLVResultsShowObjects(dicAPV);
			}
			//Valuation Pooling
			else if (oTable is List<AllSelectValuationMethodAndValue> || oTable is AllSelectValuationMethodAndValue)
			{
				List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
				if (oTable is List<AllSelectValuationMethodAndValue>)
				{
					lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)oTable;
				}
				else
				{
					lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)oTable);
				}
				Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod> dicAPV = new Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod>();
				Dictionary<APVValueAttribute, int> dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
				int ilstallSelectValuationMethodAndValue = 0;
				foreach (AllSelectValuationMethodAndValue allSelectValuationMethodAndValue in lstallSelectValuationMethodAndValue)
				{
					foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
					{
						dicLstAllSelectValuationMethodAndValue = null;
						dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
						dicLstAllSelectValuationMethodAndValue.Add(apvx, ilstallSelectValuationMethodAndValue);
						dicAPV.Add(dicLstAllSelectValuationMethodAndValue.ToList()[0], allSelectValuationMethodAndValue.AllSelectValuationMethod);
					}
					ilstallSelectValuationMethodAndValue++;
				}
				SetOLVResultsShowObjects(dicAPV);
			}
			//QALY 1 (not in use)
			else if (oTable is AllSelectQALYMethodAndValue)
			{
				AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = (AllSelectQALYMethodAndValue)oTable;


				Dictionary<QALYValueAttribute, AllSelectQALYMethod> dicAPV = new Dictionary<QALYValueAttribute, AllSelectQALYMethod>();

				foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
				{
					dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod);
				}

				SetOLVResultsShowObjects(dicAPV);
			}
			//QALY 2 (not in use)
			else if (oTable is List<AllSelectQALYMethodAndValue>)
			{
				List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)oTable;

				Dictionary<QALYValueAttribute, AllSelectQALYMethod> dicAPV = new Dictionary<QALYValueAttribute, AllSelectQALYMethod>();
				foreach (AllSelectQALYMethodAndValue allSelectQALYMethodAndValue in lstAllSelectQALYMethodAndValue)
				{
					foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
					{
						dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod);
					}
				}

				SetOLVResultsShowObjects(dicAPV);
			}
			//AQ
			else if (oTable is BenMAPLine)
			{
				BenMAPLine crTable = (BenMAPLine)oTable;
				SetOLVResultsShowObjects(crTable.ModelResultAttributes);
			}
		}

		private void btnResultShow_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			try
			{
				if (e.ClickedItem == null) { return; }
				string tag = e.ClickedItem.Name.ToString();
				Console.WriteLine(tag);
				switch (tag)
				{
					case "bindingNavigatorMoveFirstItem":
						_pageCurrent = 1;
						_currentRow = 0;
						UpdateTableResult(_tableObject);
						break;
					case "bindingNavigatorMovePreviousItem":
						_pageCurrent--;
						if (_pageCurrent <= 0)
						{
							_pageCurrent = 1;
							return;
						}
						else
						{
							_currentRow = _pageSize * (_pageCurrent - 1);
						}
						UpdateTableResult(_tableObject);
						break;
					case "bindingNavigatorMoveNextItem":
						_pageCurrent++;
						if (_pageCurrent > _pageCount)
						{
							_pageCurrent = _pageCount;
							return;
						}
						else
						{
							_currentRow = _pageSize * (_pageCurrent - 1);
						}
						UpdateTableResult(_tableObject);
						break;
					case "bindingNavigatorMoveLastItem":
						_pageCurrent = _pageCount;
						_currentRow = _pageSize * (_pageCurrent - 1);
						UpdateTableResult(_tableObject);
						break;
				}
				bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void bindingNavigatorPositionItem_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				try
				{
					_pageCurrent = Convert.ToInt32(bindingNavigatorPositionItem.Text);
					_currentRow = _pageSize * (_pageCurrent - 1);
					UpdateTableResult(_tableObject);
				}
				catch
				{
				}
			}
		}

		private void btShowIncidencePooling_Click(object sender, EventArgs e)
		{
			try
			{
				if (cbPoolingWindowAPV.Items.Count == 0)
					return;
				ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowAPV.SelectedIndex].ToString()).First();
				if (chbAPVAggregation.Checked)
				{
					if (vb.IncidencePoolingResultAggregation == null)
					{
						MessageBox.Show("No aggregated result.");
						return;
					}
				}
				else
				{
					if (vb.IncidencePoolingAndAggregation == null)
					{
						MessageBox.Show("No result.");
						return;
					}
				}

				if (vb.IncidencePoolingResult != null && vb.IncidencePoolingResult.CRCalculateValues != null
						&& vb.IncidencePoolingResult.CRCalculateValues.Count > 0)
				{
					bool bGIS = true;
					bool bTable = true;
					bool bChart = true;
					WaitShow("Creating pooled incidence results");
					CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = vb.IncidencePoolingResult;
					if (chbAPVAggregation.Checked)
					{
						crSelectFunctionCalculateValue = vb.IncidencePoolingResultAggregation;
					}
					if (this.rbShowActiveAPV.Checked)
					{
						if (tabCtlMain.SelectedIndex == 0)
						{
							bTable = false;
							bChart = false;
						}
						else if (tabCtlMain.SelectedIndex == 1)
						{
							bGIS = false;
							bChart = false;
						}
						else if (tabCtlMain.SelectedIndex == 2)
						{
							bGIS = false;
							bTable = false;
						}
					}
					if (bChart)
					{
						InitChartResult(crSelectFunctionCalculateValue, CommonClass.GBenMAPGrid.GridDefinitionID);
					}
					if (bTable)
					{
						InitTableResult(crSelectFunctionCalculateValue);
					}

					if (bGIS)
					{
						//set change projection text
						string changeProjText = "change projection to setup projection";
						if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
						{
							changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
						}
						tsbChangeProjection.Text = changeProjText;

						mainMap.ProjectionModeReproject = ActionMode.Never;
						mainMap.ProjectionModeDefine = ActionMode.Never;
						string shapeFileName = "";
						if (chbAPVAggregation.Checked)
						{
							if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation is ShapefileGrid)
							{
								//mainMap.Layers.Clear();
								if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName + ".shp"))
								{
									shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName + ".shp";
								}
							}
							else if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation is RegularGrid)
							{
								//mainMap.Layers.Clear();
								if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName + ".shp"))
								{
									shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName + ".shp";
								}
							}
						}
						else
						{
							if (CommonClass.GBenMAPGrid is ShapefileGrid)
							{
								//mainMap.Layers.Clear();
								if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
								{
									shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
								}
							}
							else if (CommonClass.GBenMAPGrid is RegularGrid)
							{
								//mainMap.Layers.Clear();
								if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
								{
									shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
								}
							}
						}

						IFeatureSet fs = FeatureSet.Open(shapeFileName);

						int j = 0;
						int iCol = 0;
						int iRow = 0;
						List<string> lstRemoveName = new List<string>();
						while (j < fs.DataTable.Columns.Count)
						{
							if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
							if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

							j++;
						}
						j = 0;

						while (j < fs.DataTable.Columns.Count)
						{
							if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col" || fs.DataTable.Columns[j].ColumnName.ToLower() == "row")
							{ }
							else
								lstRemoveName.Add(fs.DataTable.Columns[j].ColumnName);

							j++;
						}
						foreach (string s in lstRemoveName)
						{
							fs.DataTable.Columns.Remove(s);
						}
						fs.DataTable.Columns.Add("Value", typeof(double));
						j = 0;
						while (j < fs.DataTable.Columns.Count)
						{
							if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
							if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

							j++;
						}
						j = 0;
						Dictionary<string, double> dicAll = new Dictionary<string, double>();
						foreach (CRCalculateValue crcv in crSelectFunctionCalculateValue.CRCalculateValues)
						{
							dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
						}
						foreach (DataRow dr in fs.DataTable.Rows)
						{
							try
							{
								if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
									dr["Value"] = dicAll[dr[iCol] + "," + dr[iRow]];
								else
									dr["Value"] = 0.0;
							}
							catch (Exception ex)
							{
							}
						}
						if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
						fs.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);

						MapPolygonLayer polLayer = (MapPolygonLayer)mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
						string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;

						_columnName = strValueField;
						polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "IP"); //-MCB added

						double dMinValue = 0.0;
						double dMaxValue = 0.0;
						dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
						dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);
						_dMinValue = dMinValue;
						_dMaxValue = dMaxValue;
						_CurrentIMapLayer = polLayer;
						_columnName = strValueField;
						_CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: Pooled Incidence- " + strValueField;
						addAdminLayers();
					}
					WaitClose();
				}
			}
			catch
			{
				WaitClose();
			}
		}

		private void tabCtlReport_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabCtlMain.SelectedIndex == 3 && tabCtlReport.SelectedIndex < 5)
			{
				tabCtlMain.SelectedIndex = 0;
			}
			if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabPoolingIncidence")
			{
				// olvIncidence.SelectAll();
				// _IncidenceDragged = true;
				//tlvIncidence_DoubleClick(sender, e);
				_IncidenceDragged = false;
			}
			if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
			{
				// tlvAPVResult.SelectAll();
				// _APVdragged = true;
				// tlvAPVResult_DoubleClick(sender, e);
				_APVdragged = false;
			}
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog openfile = new OpenFileDialog();
				openfile.InitialDirectory = CommonClass.ResultFilePath + @"\Result";
				openfile.Filter = "Supported File Types (*.aqgx, *.cfgx, *.cfgrx, *.apvx, *.apvrx)|*.aqgx; *.cfgx; *.cfgrx; *.apvx; *.apvrx|AQG file(*.aqgx)|*.aqgx|CFG file(*.cfgx)|*.cfgx|CFGR file(*.cfgrx)|*.cfgrx|APV file(*.apvx)|*.apvx|APVR file(*.apvrx)|*.apvrx";
				openfile.FilterIndex = 1;
				openfile.RestoreDirectory = true;
				if (openfile.ShowDialog() != DialogResult.OK)
				{ return; }
				txtExistingConfiguration.Text = openfile.FileName;
			}
			catch (Exception ex)
			{ }
		}

		private void btShowAudit_Click(object sender, EventArgs e)
		{
			try
			{
				About about = new About();
				tabCtlMain.SelectedIndex = 3;
				List<TreeNode> lstTmp = new List<TreeNode>();
				string runTime = "(Requested: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")";
				if (rbAuditFile.Checked)
				{
					string filePath = txtExistingConfiguration.Text;
					string fileType = Path.GetExtension(txtExistingConfiguration.Text);
					string fileName = Path.GetFileNameWithoutExtension(txtExistingConfiguration.Text);
					TreeNode runName = new TreeNode();
					runName.Text = "Audit Trail Report Name: " + fileName + fileType + " " + runTime;
					TreeNode setupNode = new TreeNode();
					TreeNode aqgTreeNode = new TreeNode();
					aqgTreeNode.Text = "Air Quality Surfaces";
					TreeNode runtime = new TreeNode();
					runtime.Text = "Runtime";

					int i = 1;

					switch (fileType)
					{
						case ".aqgx":
							BenMAPLine aqgBenMAPLine = new BenMAPLine();
							string err = "";
							aqgBenMAPLine = DataSourceCommonClass.LoadAQGFile(filePath, ref err);
							if (aqgBenMAPLine == null)
							{
								using (FileStream fs = new FileStream(filePath, FileMode.Open))
								{
									try
									{
										aqgBenMAPLine = Serializer.Deserialize<BenMAPLine>(fs);
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
							aqgTreeNode.Nodes.Add(AuditTrailReportCommonClass.getTreeNodeFromBenMAPPollutant(aqgBenMAPLine.Pollutant));
							aqgTreeNode.Nodes.Add(AuditTrailReportCommonClass.getTreeNodeFromBenMAPLine(aqgBenMAPLine));
							aqgTreeNode.Nodes.Add(AuditTrailReportCommonClass.getTreeNodeFromBenMAPGrid(aqgBenMAPLine.GridType));

							TreeNode tnVersion = new TreeNode();
							tnVersion.Text = aqgBenMAPLine.Version == null ? "BenMAP-CE" : aqgBenMAPLine.Version;
							setupNode.Text = "Setup Name: " + aqgBenMAPLine.Setup.SetupName;
							setupNode.Nodes.Add("GIS Projection: " + aqgBenMAPLine.Setup.SetupProjection);
							lstTmp.Add(tnVersion);
							lstTmp.Add(runName);
							lstTmp.Add(setupNode);
							lstTmp.Add(aqgTreeNode);
							break;
						case ".cfgx":
							BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
							err = "";
							cfgFunction = ConfigurationCommonClass.loadCFGFile(filePath, ref err);
							if (cfgFunction == null)
							{
								BaseControlCRSelectFunction baseControlCRSelectFunction = null;
								using (FileStream fs = new FileStream(filePath, FileMode.Open))
								{
									try
									{
										cfgFunction = Serializer.Deserialize<BaseControlCRSelectFunction>(fs);
										fs.Close();
										fs.Dispose();
									}
									catch (Exception ex)
									{
										fs.Close();
										fs.Dispose();
									}
								}
							}
							TreeNode cfgTreeNode = new TreeNode();
							cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);

							foreach (BaseControlGroup bcg in cfgFunction.BaseControlGroup)
							{
								TreeNode bcgNode = new TreeNode();
								bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
								aqgTreeNode.Nodes.Add(bcgNode);
							}
							tnVersion = new TreeNode();
							tnVersion.Text = cfgFunction.Version == null ? "BenMAP-CE" : cfgFunction.Version;
							setupNode.Text = "Setup Name: " + CommonClass.getBenMAPSetupFromName(cfgFunction.BaseControlGroup[0].GridType.SetupName).SetupName;
							setupNode.Nodes.Add("GIS Projection: " + CommonClass.getBenMAPSetupFromName(cfgFunction.BaseControlGroup[0].GridType.SetupName).SetupProjection);
							lstTmp.Add(tnVersion);
							lstTmp.Add(runName);
							lstTmp.Add(setupNode);
							lstTmp.Add(aqgTreeNode);
							lstTmp.Add(cfgTreeNode);
							break;
						case ".cfgrx":
							BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
							err = "";
							cfgrFunctionCV = ConfigurationCommonClass.LoadCFGRFile(filePath, ref err);
							if (cfgrFunctionCV == null)
							{
								using (FileStream fs = new FileStream(filePath, FileMode.Open))
								{
									try
									{
										cfgrFunctionCV = Serializer.Deserialize<BaseControlCRSelectFunctionCalculateValue>(fs);
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
							TreeNode cfgrTreeNode = new TreeNode();
							cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);

							foreach (BaseControlGroup bcg in cfgrFunctionCV.BaseControlGroup)
							{
								TreeNode bcgNode = new TreeNode();
								bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
								aqgTreeNode.Nodes.Add(bcgNode);
							}
							tnVersion = new TreeNode();
							tnVersion.Text = cfgrFunctionCV.Version == null ? "BenMAP-CE" : cfgrFunctionCV.Version;
							setupNode.Text = "Setup Name: " + CommonClass.getBenMAPSetupFromName(cfgrFunctionCV.BaseControlGroup[0].GridType.SetupName).SetupName;
							setupNode.Nodes.Add("GIS Projection: " + CommonClass.getBenMAPSetupFromName(cfgrFunctionCV.BaseControlGroup[0].GridType.SetupName).SetupProjection);
							runtime.Nodes.Add(AuditTrailReportCommonClass.getRuntimeTreeNode(cfgrFunctionCV.lstLog));
							lstTmp.Add(tnVersion);
							lstTmp.Add(runName);
							lstTmp.Add(runtime);
							lstTmp.Add(setupNode);
							lstTmp.Add(aqgTreeNode);
							lstTmp.Add(cfgrTreeNode);
							break;
						case ".apvx":
							ValuationMethodPoolingAndAggregation apvVMPA = new ValuationMethodPoolingAndAggregation();
							err = "";
							apvVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
							if (apvVMPA == null)
							{
								ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = null;
								using (FileStream fs = new FileStream(filePath, FileMode.Open))
								{
									try
									{
										valuationMethodPoolingAndAggregation = Serializer.Deserialize<ValuationMethodPoolingAndAggregation>(fs);
									}
									catch
									{
										fs.Close();
										fs.Dispose();
										FileStream fsSec = new FileStream(filePath, FileMode.Open);
										valuationMethodPoolingAndAggregation = Serializer.DeserializeWithLengthPrefix<ValuationMethodPoolingAndAggregation>(fsSec, PrefixStyle.Fixed32);
										fsSec.Close();
										fsSec.Dispose();
									}
									fs.Close();
									fs.Dispose();
								}
								apvVMPA = valuationMethodPoolingAndAggregation;
							}
							TreeNode apvTreeNode = new TreeNode();
							apvTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvVMPA);
							cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(apvVMPA.BaseControlCRSelectFunctionCalculateValue);

							foreach (BaseControlGroup bcg in apvVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
							{
								TreeNode bcgNode = new TreeNode();
								bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
								aqgTreeNode.Nodes.Add(bcgNode);
							}
							tnVersion = new TreeNode();
							tnVersion.Text = apvVMPA.Version == null ? "BenMAP-CE" : apvVMPA.Version;
							setupNode.Text = "Setup Name: " + CommonClass.getBenMAPSetupFromName(apvVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName).SetupName;
							setupNode.Nodes.Add("GIS Projection: " + CommonClass.getBenMAPSetupFromName(apvVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName).SetupProjection);
							lstTmp.Add(tnVersion);
							lstTmp.Add(runName);
							lstTmp.Add(setupNode);
							lstTmp.Add(aqgTreeNode);
							lstTmp.Add(cfgrTreeNode);
							lstTmp.Add(apvTreeNode);
							break;
						case ".apvrx":
							ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
							err = "";
							apvrVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
							if (apvrVMPA == null)
							{
								using (FileStream fs = new FileStream(filePath, FileMode.Open))
								{
									try
									{
										apvrVMPA = Serializer.Deserialize<ValuationMethodPoolingAndAggregation>(fs);
									}
									catch
									{
										fs.Close();
										fs.Dispose();
										FileStream fsSec = new FileStream(filePath, FileMode.Open);
										apvrVMPA = Serializer.DeserializeWithLengthPrefix<ValuationMethodPoolingAndAggregation>(fsSec, PrefixStyle.Fixed32);
										fsSec.Close();
										fsSec.Dispose();
									}
									fs.Close();
									fs.Dispose();
								}
							}
							TreeNode apvrTreeNode = new TreeNode();
							apvrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvrVMPA);
							cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(apvrVMPA.BaseControlCRSelectFunctionCalculateValue);

							foreach (BaseControlGroup bcg in apvrVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
							{
								TreeNode bcgNode = new TreeNode();
								bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
								aqgTreeNode.Nodes.Add(bcgNode);
							}
							tnVersion = new TreeNode();
							tnVersion.Text = apvrVMPA.Version == null ? "BenMAP-CE" : apvrVMPA.Version;
							setupNode.Text = "Setup Name: " + CommonClass.getBenMAPSetupFromName(apvrVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName).SetupName;
							setupNode.Nodes.Add("GIS Projection: " + CommonClass.getBenMAPSetupFromName(apvrVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName).SetupProjection);
							runtime.Nodes.Add(AuditTrailReportCommonClass.getRuntimeTreeNode(apvrVMPA.BaseControlCRSelectFunctionCalculateValue.lstLog));
							runtime.Nodes.Add(AuditTrailReportCommonClass.getRuntimeTreeNode(apvrVMPA.lstLog));
							lstTmp.Add(tnVersion);
							lstTmp.Add(runName);
							lstTmp.Add(runtime);
							lstTmp.Add(setupNode);
							lstTmp.Add(aqgTreeNode);
							lstTmp.Add(cfgrTreeNode);
							lstTmp.Add(apvrTreeNode);
							break;
					}
				}
				else if (rbAuditCurrent.Checked)
				{
					TreeNode runName = new TreeNode();
					runName.Text = "Current Audit Trail " + runTime;
					TreeNode setupNode = new TreeNode();

					if (CommonClass.ValuationMethodPoolingAndAggregation != null)
					{

						ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
						apvrVMPA = CommonClass.ValuationMethodPoolingAndAggregation;
						TreeNode apvrTreeNode = new TreeNode();
						apvrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvrVMPA);
						TreeNode cfgrTreeNode = new TreeNode();
						cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(apvrVMPA.BaseControlCRSelectFunctionCalculateValue);
						TreeNode aqgTreeNode = new TreeNode();
						aqgTreeNode.Text = "Air Quality Surfaces";

						foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
						{
							TreeNode bcgNode = new TreeNode();
							bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
							aqgTreeNode.Nodes.Add(bcgNode);
						}
						TreeNode tnVersion = new TreeNode();
						tnVersion.Text = apvrVMPA.Version == null ? "BenMAP-CE" : apvrVMPA.Version;
						setupNode.Text = "Setup Name: " + CommonClass.getBenMAPSetupFromName(apvrVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName).SetupName;
						setupNode.Nodes.Add("GIS Projection: " + CommonClass.getBenMAPSetupFromName(apvrVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName).SetupProjection);
						TreeNode runtime = new TreeNode();
						runtime.Text = "Runtime";
						runtime.Nodes.Add(AuditTrailReportCommonClass.getRuntimeTreeNode(apvrVMPA.BaseControlCRSelectFunctionCalculateValue.lstLog));
						runtime.Nodes.Add(AuditTrailReportCommonClass.getRuntimeTreeNode(apvrVMPA.lstLog));
						lstTmp.Add(tnVersion);
						lstTmp.Add(runName);
						lstTmp.Add(runtime);
						lstTmp.Add(setupNode);
						lstTmp.Add(aqgTreeNode);
						lstTmp.Add(cfgrTreeNode);
						lstTmp.Add(apvrTreeNode);
					}
					else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
					{
						BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
						cfgrFunctionCV = CommonClass.BaseControlCRSelectFunctionCalculateValue;
						TreeNode cfgrTreeNode = new TreeNode();
						cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);
						TreeNode aqgTreeNode = new TreeNode();
						aqgTreeNode.Text = "Air Quality Surfaces";

						foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
						{
							TreeNode bcgNode = new TreeNode();
							bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
							aqgTreeNode.Nodes.Add(bcgNode);
						}
						TreeNode tnVersion = new TreeNode();
						tnVersion.Text = cfgrFunctionCV.Version == null ? "BenMAP-CE" : cfgrFunctionCV.Version;
						BenMAPSetup currSetup = CommonClass.getBenMAPSetupFromName(cfgrFunctionCV.BaseControlGroup[0].GridType.SetupName);
						setupNode.Text = "Setup Name: " + currSetup.SetupName;
						setupNode.Nodes.Add("GIS Projection: " + currSetup.SetupProjection);
						TreeNode runtime = new TreeNode();
						runtime.Text = "Runtime";
						runtime.Nodes.Add(AuditTrailReportCommonClass.getRuntimeTreeNode(cfgrFunctionCV.lstLog));
						lstTmp.Add(tnVersion);
						lstTmp.Add(runName);
						lstTmp.Add(runtime);
						lstTmp.Add(setupNode);
						lstTmp.Add(aqgTreeNode);
						lstTmp.Add(cfgrTreeNode);
					}
					else if (CommonClass.BaseControlCRSelectFunction != null)
					{
						BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
						cfgFunction = CommonClass.BaseControlCRSelectFunction;
						TreeNode cfgTreeNode = new TreeNode();
						cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);
						TreeNode aqgTreeNode = new TreeNode();
						aqgTreeNode.Text = "Air Quality Surfaces";

						foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
						{
							TreeNode bcgNode = new TreeNode();
							bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
							aqgTreeNode.Nodes.Add(bcgNode);
						}
						TreeNode tnVersion = new TreeNode();
						tnVersion.Text = cfgFunction.Version == null ? "BenMAP-CE" : cfgFunction.Version;
						BenMAPSetup currSetup = CommonClass.getBenMAPSetupFromName(cfgFunction.BaseControlGroup[0].GridType.SetupName);
						setupNode.Text = "Setup Name: " + currSetup.SetupName;
						setupNode.Nodes.Add("GIS Projection: " + currSetup.SetupProjection);
						lstTmp.Add(tnVersion);
						lstTmp.Add(runName);
						lstTmp.Add(setupNode);
						lstTmp.Add(aqgTreeNode);
						lstTmp.Add(cfgTreeNode);
					}
					else if (CommonClass.LstBaseControlGroup[0].Control != null)        //(20/11/19, mp) assumes that user won't request audit trail until all Air Quality sections are completed
					{
						TreeNode aqgTreeNode = new TreeNode();
						aqgTreeNode.Text = "Air Quality Surfaces";

						foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
						{
							if (bcg.Base != null && bcg.Control != null)
							{

								TreeNode bcgNode = new TreeNode();
								bcgNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlGroup(bcg);
								aqgTreeNode.Nodes.Add(bcgNode);
							}
						}
						TreeNode tnVersion = new TreeNode();
						tnVersion.Text = CommonClass.LstBaseControlGroup[0].Base.Version == null ? "BenMAP-CE" : CommonClass.LstBaseControlGroup[0].Base.Version;
						BenMAPSetup currSetup = CommonClass.getBenMAPSetupFromName(CommonClass.LstBaseControlGroup[0].GridType.SetupName);
						setupNode.Text = "Setup Name: " + currSetup.SetupName;
						setupNode.Nodes.Add("GIS Projection: " + currSetup.SetupProjection);
						lstTmp.Add(tnVersion);
						lstTmp.Add(runName);
						lstTmp.Add(setupNode);
						lstTmp.Add(aqgTreeNode);
					}
					else
					{
						MessageBox.Show("Please finish your configuration first.");
						return;
					}
				}
				treeListView.Objects = lstTmp;
				foreach (TreeNode tn in treeListView.Objects)
				{
					TreeNodeCollapse(tn.Nodes);
					this.treeListView.Collapse(tn);
				}
				this.btnAuditTrailExpand.Enabled = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");
				Logger.LogError(ex.Message);
			}
		}
		private void getMetadataForAuditTrail(TreeView trv)
		{
			string datasetTypeName = string.Empty;
			string datasetName = string.Empty;
			TreeNode tnTemp = null;
			TreeNode tnDatasetTypeName = null;//i.e. Monitor
			TreeNode tnDataSetName = null;//i.e. MDS1
			TreeNode tnDataFileName = null;//i.e. DetroitMonitors PM 25
			TreeNode tnMetadata = new TreeNode();//Top node - Datasts
			tnMetadata.Text = "Datasets";
			tnMetadata.Name = "Datasets";
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			System.Data.DataSet ds = new System.Data.DataSet();
			string commandText = string.Empty;
			commandText = string.Format("SELECT a.METADATAID, a.SETUPID, a.DATASETID, a.DATASETTYPEID, a.FILENAME, a.EXTENSION, a.DATAREFERENCE, a.FILEDATE, " +
																	"a.IMPORTDATE, a.DESCRIPTION, a.PROJECTION, a.GEONAME, a.DATUMNAME, a.DATUMTYPE, a.SPHEROIDNAME, a.MERIDIANNAME, " +
																	"a.UNITNAME, a.PROJ4STRING, a.NUMBEROFFEATURES, a.METADATAENTRYID " +
																	"FROM METADATAINFORMATION a " +
																	"where setupid = {0} " +
																	"order by a.DATASETTYPEID", CommonClass.MainSetup.SetupID);
			ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				//get the dataset ID, with the dataset ID get the dataset name
				commandText = string.Format("SELECT DATASETTYPENAME FROM DATASETTYPES WHERE DATASETTYPEID = {0}", dr["DATASETTYPEID"].ToString());
				object temp = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				datasetTypeName = temp.ToString();

				if (!tnMetadata.Nodes.ContainsKey(datasetTypeName))//its new
				{
					tnDatasetTypeName = new TreeNode();
					tnDatasetTypeName.Name = datasetTypeName;
					tnDatasetTypeName.Text = datasetTypeName;

					tnMetadata.Nodes.Add(tnDatasetTypeName);
					//Get the Datasets names
					datasetName = getDatasetEntryName(datasetTypeName, Convert.ToInt32(dr["SETUPID"].ToString()), Convert.ToInt32(dr["DATASETID"].ToString()));
					if (!string.IsNullOrEmpty(datasetName))
					{
						tnTemp = tnMetadata.Nodes[datasetTypeName];
						if (!tnTemp.Nodes.ContainsKey(datasetName))
						{
							tnDataSetName = new TreeNode();
							tnDataSetName.Name = datasetName;
							tnDataSetName.Text = datasetName;
							tnDatasetTypeName.Nodes.Add(tnDataSetName);

							DataRow[] drs = ds.Tables[0].Select(string.Format("DATASETID = {0} AND DATASETTYPEID = {1}", Convert.ToInt32(dr["DATASETID"].ToString()), Convert.ToInt32(dr["DATASETTYPEID"].ToString())));
							foreach (DataRow drMetadata in drs)
							{
								tnDataFileName = new TreeNode();//file level
								tnDataFileName.Name = drMetadata["FILENAME"].ToString();
								tnDataFileName.Text = string.Format("File Name: {0}", drMetadata["FILENAME"].ToString());
								tnDataSetName.Nodes.Add(tnDataFileName);
								//now getting the metadata and placeing it under the tnDataFileName
								tnTemp = new TreeNode();
								tnTemp.Name = drMetadata["EXTENSION"].ToString();
								tnTemp.Text = string.Format("Extension: {0}", drMetadata["EXTENSION"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = drMetadata["DATAREFERENCE"].ToString();
								tnTemp.Text = string.Format("Data Reference: {0}", drMetadata["DATAREFERENCE"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = "FILEDATE";
								tnTemp.Text = string.Format("File Date: {0}", drMetadata["FILEDATE"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = "IMPORTDATE";
								tnTemp.Text = string.Format("Import Date: {0}", drMetadata["IMPORTDATE"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = "DESCRIPTION";
								tnTemp.Text = string.Format("Description: {0}", drMetadata["DESCRIPTION"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								if (drMetadata["EXTENSION"].ToString().ToLower().Equals(".shp"))
								{
									tnTemp = new TreeNode();
									tnTemp.Name = "PROJECTION";
									tnTemp.Text = string.Format("Projection: {0}", drMetadata["PROJECTION"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "GEONAME";
									tnTemp.Text = string.Format("Geoname: {0}", drMetadata["GEONAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "DATUMNAME";
									tnTemp.Text = string.Format("Datum Name: {0}", drMetadata["DATUMNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "DATUMTYPE";
									tnTemp.Text = string.Format("Datumtype: {0}", drMetadata["DATUMTYPE"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "SPHEROIDNAME";
									tnTemp.Text = string.Format("Spheroid Name: {0}", drMetadata["SPHEROIDNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "MERIDIANNAME";
									tnTemp.Text = string.Format("Meridian Name: {0}", drMetadata["MERIDIANNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "UNITNAME";
									tnTemp.Text = string.Format("Unit Name: {0}", drMetadata["UNITNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "PROJ4STRING";
									tnTemp.Text = string.Format("PROJ4STRING: {0}", drMetadata["PROJ4STRING"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "NUMBEROFFEATURES";
									tnTemp.Text = string.Format("Number Of Features: {0}", drMetadata["NUMBEROFFEATURES"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);
								}
							}
						}
					}
				}
				else
				{
					tnDatasetTypeName = tnMetadata.Nodes[datasetTypeName];
					datasetName = getDatasetEntryName(datasetTypeName, Convert.ToInt32(dr["SETUPID"].ToString()), Convert.ToInt32(dr["DATASETID"].ToString()));
					if (!string.IsNullOrEmpty(datasetName))
					{
						if (!tnDatasetTypeName.Nodes.ContainsKey(datasetName))
						{
							tnDataSetName = new TreeNode();
							tnDataSetName.Name = datasetName;
							tnDataSetName.Text = datasetName;
							tnDatasetTypeName.Nodes.Add(tnDataSetName);

							DataRow[] drs = ds.Tables[0].Select(string.Format("DATASETID = {0} AND DATASETTYPEID = {1}", Convert.ToInt32(dr["DATASETID"].ToString()), Convert.ToInt32(dr["DATASETTYPEID"].ToString())));
							foreach (DataRow drMetadata in drs)
							{
								tnDataFileName = new TreeNode();//file level
								tnDataFileName.Name = drMetadata["FILENAME"].ToString();
								tnDataFileName.Text = string.Format("File Name: {0}", drMetadata["FILENAME"].ToString());
								tnDataSetName.Nodes.Add(tnDataFileName);
								//now getting the metadata and placeing it under the tnDataFileName
								tnTemp = new TreeNode();
								tnTemp.Name = drMetadata["EXTENSION"].ToString();
								tnTemp.Text = string.Format("Extension: {0}", drMetadata["EXTENSION"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = drMetadata["DATAREFERENCE"].ToString();
								tnTemp.Text = string.Format("Data Reference: {0}", drMetadata["DATAREFERENCE"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = "FILEDATE"; //drMetadata["FILEDATE"].ToString();
								tnTemp.Text = string.Format("File Date: {0}", drMetadata["FILEDATE"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = "IMPORTDATE"; //drMetadata["IMPORTDATE"].ToString();
								tnTemp.Text = string.Format("Import Date: {0}", drMetadata["IMPORTDATE"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);
								tnTemp = new TreeNode();
								tnTemp.Name = "DESCRIPTION"; //drMetadata["IMPORTDATE"].ToString();
								tnTemp.Text = string.Format("Description: {0}", drMetadata["DESCRIPTION"].ToString());
								tnDataFileName.Nodes.Add(tnTemp);

								if (drMetadata["EXTENSION"].ToString().ToLower().Equals(".shp"))
								{
									tnTemp = new TreeNode();
									tnTemp.Name = "PROJECTION";
									tnTemp.Text = string.Format("Projection: {0}", drMetadata["PROJECTION"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "GEONAME";
									tnTemp.Text = string.Format("Geoname: {0}", drMetadata["GEONAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "DATUMNAME";
									tnTemp.Text = string.Format("Datum Name: {0}", drMetadata["DATUMNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "DATUMTYPE";
									tnTemp.Text = string.Format("Datumtype: {0}", drMetadata["DATUMTYPE"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "SPHEROIDNAME";
									tnTemp.Text = string.Format("Spheroid Name: {0}", drMetadata["SPHEROIDNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "MERIDIANNAME";
									tnTemp.Text = string.Format("Meridian Name: {0}", drMetadata["MERIDIANNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "UNITNAME";
									tnTemp.Text = string.Format("Unit Name: {0}", drMetadata["UNITNAME"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "PROJ4STRING";
									tnTemp.Text = string.Format("PROJ4STRING: {0}", drMetadata["PROJ4STRING"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);

									tnTemp = new TreeNode();
									tnTemp.Name = "NUMBEROFFEATURES";
									tnTemp.Text = string.Format("Number Of Features: {0}", drMetadata["NUMBEROFFEATURES"].ToString());
									tnDataFileName.Nodes.Add(tnTemp);
								}

							}
						}
					}
				}


			}
			trv.Nodes.Add(tnMetadata);
		}

		private string getDatasetEntryName(string DatasetTypeName, int setupid, int datasetid)
		{
			string commandText = string.Empty;
			object rtv = null;
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			switch (DatasetTypeName.ToLower())
			{
				case "incidence":
					commandText = string.Format("select INCIDENCEDATASETNAME from INCIDENCEDATASETS where SETUPID = {0} and INCIDENCEDATASETID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				case "variabledataset":
					commandText = string.Format("select SETUPVARIABLEDATASETNAME from SETUPVARIABLEDATASETS where SETUPID = {0} and SETUPVARIABLEDATASETID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				case "inflation":
					commandText = string.Format("select INFLATIONDATASETNAME from INFLATIONDATASETS where SETUPID = {0} and INFLATIONDATASETID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				case "incomegrowth":
					commandText = string.Format("select INCOMEGROWTHADJDATASETNAME from INCOMEGROWTHADJDATASETS where SETUPID = {0} and INCOMEGROWTHADJDATASETID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				case "healthfunctions":
					commandText = string.Format("select CRFUNCTIONDATASETNAME from CRFUNCTIONDATASETS where SETUPID = {0} and CRFUNCTIONDATASETID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				case "valuationfunction":
					commandText = string.Format("select VALUATIONFUNCTIONDATASETNAME from VALUATIONFUNCTIONDATASETS where SETUPID = {0} and VALUATIONFUNCTIONDATASETID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				case "population":
					commandText = string.Format("select POPULATIONDATASETNAME from POPULATIONDATASETS where SETUPID = {0} and POPULATIONDATASETID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				//case "baseline":

				//break;
				//case "control":

				//break;
				case "griddefinition":
					commandText = string.Format("select GRIDDEFINITIONNAME from GRIDDEFINITIONS where SETUPID = {0} and GRIDDEFINITIONID = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;
				case "monitor":
					commandText = string.Format("select monitordatasetname from monitordatasets where setupid = {0} and monitordatasetid = {1}", setupid, datasetid);
					rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					break;

				default:
					break;
			}

			return rtv.ToString();
		}

		private void rbAuditFile_Click(object sender, EventArgs e)
		{
			try
			{
				if (rbAuditFile.Checked)
				{
					txtExistingConfiguration.Enabled = true;
					btnBrowse.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void rbAuditCurrent_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (rbAuditCurrent.Checked)
				{
					txtExistingConfiguration.Enabled = false;
					btnBrowse.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			{
				try
				{
					PlotModel plotModel = new PlotModel();
					ColumnSeries barChart = new ColumnSeries();
					CategoryAxis catAxis = new CategoryAxis();
					LinearAxis yAxis = new LinearAxis();

					barChart.FillColor = OxyColors.RoyalBlue;
					barChart.Background = OxyColor.FromRgb(255, 255, 255);
					barChart.TrackerFormatString = "{0}\n. {1}:\n  {2:0.00000}";


					int i = 0;
					while (i < 1)
					{
						int j = 0;
						int count = olvRegions.CheckedObjects.Count;
						int maxDisplay = 8;

						if (count <= maxDisplay)
						{
							foreach (ChartResult cr in olvRegions.CheckedObjects)
							{
								barChart.Items.Add(new ColumnItem(cr.RegionValue, -1));
								catAxis.Labels.Add(cr.RegionName);
							}
							i++;
						}
						else
						{
							int findLast = 0;
							int skip = count / maxDisplay;
							catAxis.MajorGridlineThickness = 0.2;
							catAxis.MajorTickSize = 2;
							string label = "";
							foreach (ChartResult cr in olvRegions.CheckedObjects)
							{
								barChart.Items.Add(new ColumnItem(cr.RegionValue, -1));

								if (j == 0 || findLast == count) { label = cr.RegionName; }
								else { label = ""; }
								catAxis.Labels.Add(label);

								if (j == skip) { j = 0; }
								else { j++; }
								findLast++;
							}
							i++;
						}
					}

					catAxis.Title = strchartX;
					catAxis.TitleFontWeight = 700;
					catAxis.TitleFontSize = 14;
					catAxis.Angle = -30;
					catAxis.TickStyle = OxyPlot.Axes.TickStyle.Crossing;
					catAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
					catAxis.MinimumRange = 5;
					catAxis.IsPanEnabled = true;

					yAxis.Title = strchartY;
					yAxis.TitleFontWeight = 700;
					yAxis.TitleFontSize = 14;
					yAxis.AxisTitleDistance = 15;
					yAxis.Minimum = 0;
					yAxis.AbsoluteMinimum = 0;
					yAxis.MinimumPadding = 2;
					yAxis.MaximumPadding = 0.1;
					yAxis.TickStyle = OxyPlot.Axes.TickStyle.Crossing;
					yAxis.Position = OxyPlot.Axes.AxisPosition.Left;
					yAxis.StringFormat = String.Format("#,##0.#####");

					plotModel.Title = strchartTitle;
					plotModel.TitleFont = "Helvetica";
					plotModel.Padding = new OxyThickness(15);
					plotModel.Axes.Add(catAxis);
					plotModel.Axes.Add(yAxis);
					plotModel.Series.Add(barChart);
					this.oxyPlotView.Model = plotModel;
					this.splitContainer4.Panel2.Refresh();
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.Message);
				}
			}
		}

		private void cbChartXAxis_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (cbChartXAxis.Text != chartXAxis)
				{
					if (cbChartXAxis.Text == "Col/Row")
					{
						for (int j = 0; j < CommonClass.lstChartResult.Count; j++)
						{
							CommonClass.lstChartResult[j].RegionName = CommonClass.lstChartResult[j].Col.ToString() + "/" + CommonClass.lstChartResult[j].Row.ToString();
						}
					}
					else
					{
						string shapefilename = "";
						if (ChartGrid is ShapefileGrid)
							shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as ShapefileGrid).ShapefileName + ".shp";
						else if (CommonClass.RBenMAPGrid is RegularGrid)
							shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as RegularGrid).ShapefileName + ".shp";
						DotSpatial.Data.IFeatureSet fs = new DotSpatial.Data.FeatureSet();
						if (File.Exists(shapefilename))
						{
							fs = DotSpatial.Data.FeatureSet.Open(shapefilename);

							try
							{
								for (int iDt = 0; iDt < fs.DataTable.Rows.Count; iDt++)
								{
									fs.DataTable.Rows[iDt]["ROW"] = Convert.ToInt32(fs.DataTable.Rows[iDt]["ROW"].ToString());
									fs.DataTable.Rows[iDt]["COL"] = Convert.ToInt32(fs.DataTable.Rows[iDt]["COL"].ToString());

								}
							}
							catch
							{ }
							foreach (DataColumn dc in fs.DataTable.Columns)
							{
								if (dc.ColumnName == cbChartXAxis.Text)
								{
									for (int j = 0; j < CommonClass.lstChartResult.Count; j++)
									{
										DataRow dr = fs.DataTable.Select("COL='" + CommonClass.lstChartResult[j].Col + "' and ROW='" + CommonClass.lstChartResult[j].Row + "'").First();
										CommonClass.lstChartResult[j].RegionName = dr[cbChartXAxis.Text].ToString();
									}
									break;
								}
							}
							fs.Close();
							fs.Dispose();
						}
					}
					chartXAxis = cbChartXAxis.Text;
					List<int> lstChecked = new List<int>();
					for (int j = 0; j < olvRegions.Items.Count; j++)
					{
						if (olvRegions.Items[j].Checked)
							lstChecked.Add(j);
					}
					olvRegions.SetObjects(CommonClass.lstChartResult);
					for (int j = 0; j < olvRegions.Items.Count; j++)
					{
						OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
						if (lstChecked.Contains(j))
						{
							olvi.Checked = true;
						}
						else
							olvi.Checked = false;

					}
				}
				btnApply_Click(sender, e);
			}
			catch
			{ }
		}

		private void TimedFilter(ObjectListView olv, string txt)
		{
			this.TimedFilter(olv, txt, 0);
		}

		private void TimedFilter(ObjectListView olv, string txt, int matchKind)
		{
			TextMatchFilter filter = null;
			if (!String.IsNullOrEmpty(txt))
			{
				switch (matchKind)
				{
					case 0:
					default:
						filter = TextMatchFilter.Contains(olv, txt);
						break;
					case 1:
						filter = TextMatchFilter.Prefix(olv, txt);
						break;
					case 2:
						filter = TextMatchFilter.Regex(olv, txt);
						break;
				}
			}
			if (filter == null)
				olv.DefaultRenderer = null;
			else
			{
				olv.DefaultRenderer = new HighlightTextRenderer(filter);

				//olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = true };
			}

			// Some lists have renderers already installed
			HighlightTextRenderer highlightingRenderer = olv.GetColumn(0).Renderer as HighlightTextRenderer;
			if (highlightingRenderer != null)
				highlightingRenderer.Filter = filter;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			olv.ModelFilter = filter;
			stopWatch.Stop();
		}

		private void textChartFilter_TextChanged(object sender, EventArgs e)
		{
			this.TimedFilter(this.olvRegions, textChartFilter.Text);
		}

		public void loadAllAPVPooling()
		{
			try
			{
				dicAPVPoolingAndAggregation = new Dictionary<AllSelectValuationMethod, string>();
				dicAPVPoolingAndAggregationUnPooled = new Dictionary<AllSelectValuationMethod, string>();
				AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{

					var query = vb.LstAllSelectValuationMethod.Where(p => p.PID == -1);
					foreach (AllSelectValuationMethod allSelectValuationMethod in query)
					{
						List<AllSelectValuationMethod> lstShow = new List<AllSelectValuationMethod>();
						lstShow.Add(allSelectValuationMethod);
						getChildFromAllSelectValuationMethodUnPooled(allSelectValuationMethod, vb, ref lstShow); //add all pooling group nodes to lstShow
						foreach (AllSelectValuationMethod avm in lstShow)
						{
							if (1==1) //((avm.PoolingMethod != "None" && avm.PoolingMethod!="") || avm.NodeType==2000) || lstShow.Count()==1//groups with pooling methods assgined. or studies not pooled
							{
								try
								{
									if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
									{

										allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).FirstOrDefault();

									}
									else
									{

										allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).FirstOrDefault();
									}
									if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
										dicAPVPoolingAndAggregation.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
								}
								catch
								{
								}

							}
						}
					}

					foreach (AllSelectValuationMethod avm in vb.LstAllSelectValuationMethod)
					{
						if (avm.PoolingMethod == null)
						{
							try
							{
								if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
								{

									allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).FirstOrDefault();

								}
								else
								{

									allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();
								}
								if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
									dicAPVPoolingAndAggregationUnPooled.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
							}
							catch
							{
							}

						}
					}
				}







			}
			catch (Exception ex)
			{
			}
		}

		private void cbPoolingWindowAPV_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (rbAPVAll.Checked)
			{
				loadAllAPVPooling();
				if (this.btShowDetailValuation.Text == "Show aggregated") //default
				{
					this.tlvAPVResult.SetObjects(dicAPVPoolingAndAggregation);

				}
				else
				{
					tlvAPVResult.SetObjects(dicAPVPoolingAndAggregationUnPooled);
				}

				tlvAPVResult.SelectAll();
				if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
				{
					tlvAPVResult_DoubleClick(sender, e);
				}

				return;
			}
			if (CommonClass.ValuationMethodPoolingAndAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null) return;
			ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowAPV.SelectedIndex].ToString()).First();

			dicAPVPoolingAndAggregation = new Dictionary<AllSelectValuationMethod, string>();
			dicAPVPoolingAndAggregationUnPooled = new Dictionary<AllSelectValuationMethod, string>();
			AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
			var query = vb.LstAllSelectValuationMethod.Where(p => p.PID == -1);
			foreach (AllSelectValuationMethod allSelectValuationMethod in query)
			{
				List<AllSelectValuationMethod> lstShow = new List<AllSelectValuationMethod>();
				lstShow.Add(allSelectValuationMethod);
				getChildFromAllSelectValuationMethodUnPooled(allSelectValuationMethod, vb, ref lstShow);
				foreach (AllSelectValuationMethod avm in lstShow)
				{
					if (avm.PoolingMethod != "None")
					{
						try
						{
							if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
							{

								allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();

							}
							else
							{

								allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();
							}
							if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
								dicAPVPoolingAndAggregation.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
						}
						catch
						{
						}

					}
				}
			}

			foreach (AllSelectValuationMethod avm in vb.LstAllSelectValuationMethod)
			{
				if (avm.PoolingMethod == null)
				{
					try
					{
						if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
						{

							allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();

						}
						else
						{

							allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();
						}
						if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
							dicAPVPoolingAndAggregationUnPooled.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
					}
					catch
					{
					}

				}
			}

			if (this.btShowDetailValuation.Text == "Show aggregated")
			{
				this.tlvAPVResult.SetObjects(dicAPVPoolingAndAggregation);

			}
			else
			{
				tlvAPVResult.SetObjects(dicAPVPoolingAndAggregationUnPooled);
			}
			tlvAPVResult.SelectAll();
			if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
			{
				tlvAPVResult_DoubleClick(sender, e);
			}
		}

		public void LoadAllIncidencePooling(ref Dictionary<AllSelectCRFunction, string> Pooled, ref Dictionary<AllSelectCRFunction, string> UnPooled)
		{
			if (!rbIncidenceAll.Checked) return;
			try
			{
				Pooled = new Dictionary<AllSelectCRFunction, string>();
				UnPooled = new Dictionary<AllSelectCRFunction, string>();

				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					//YY: instead of vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, use vb.lstAllSelectCRFunctionIncidenceAggregation
					var query = vb.lstAllSelectCRFunctionIncidenceAggregation.Where(p => p.PID == -1);
					foreach (AllSelectCRFunction allSelectCRFunction in query)
					{
						List<AllSelectCRFunction> lstShow = new List<AllSelectCRFunction>();
						lstShow.Add(allSelectCRFunction);
						getChildFromAllSelectCRFunctionUnPooled(allSelectCRFunction, vb.lstAllSelectCRFunctionIncidenceAggregation, ref lstShow); // allSelectCRFunction: parent functions (PID = -1); vb: all functions (PID = -1 or PID = 0)

						foreach (AllSelectCRFunction acr in lstShow)
						{
							if (((acr.PoolingMethod != "None" && acr.PoolingMethod != "") || acr.NodeType == 100) || lstShow.Count() == 1) //groups with pooling methods assgined. or studies not pooled
							{
								Pooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
							}
						}
					}
					foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
					{
						if (acr.PoolingMethod == "" && acr.NodeType == 100) // studies.
						{
							UnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
						}

					}
				}
				return;
				//YY: below are old code
				foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
				{
					var query = ip.lstAllSelectCRFuntion.Where(p => p.PID == -1);
					foreach (AllSelectCRFunction allSelectCRFunction in query)
					{
						List<AllSelectCRFunction> lstShow = new List<AllSelectCRFunction>();
						lstShow.Add(allSelectCRFunction);
						//YY: use ip instead of vb
						getChildFromAllSelectCRFunctionUnPooled(allSelectCRFunction, ip, ref lstShow); // allSelectCRFunction: parent functions (PID = -1); ip: all functions (PID = -1 or PID = 0)

						foreach (AllSelectCRFunction acr in lstShow)
						{
							if ((acr.PoolingMethod != "None" && acr.PoolingMethod != "" && acr.NodeType != 100) || lstShow.Count() == 1) //groups with pooling methods assgined.
							{
								Pooled.Add(acr, ip.PoolingName);
							}
						}
					}
					foreach (AllSelectCRFunction acr in ip.lstAllSelectCRFuntion)
					{
						if (acr.PoolingMethod == "" && acr.NodeType == 100) // studies aren't pooled to any groups.
						{
							UnPooled.Add(acr, ip.PoolingName);
						}

					}
				}

				return;
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{

					var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1);
					foreach (AllSelectCRFunction allSelectCRFunction in query)
					{
						List<AllSelectCRFunction> lstShow = new List<AllSelectCRFunction>();
						lstShow.Add(allSelectCRFunction);
						getChildFromAllSelectCRFunctionUnPooled(allSelectCRFunction, vb, ref lstShow); // allSelectCRFunction: parent functions (PID = -1); vb: all functions (PID = -1 or PID = 0)

						foreach (AllSelectCRFunction acr in lstShow)
						{
							if ((acr.PoolingMethod != "None" && acr.PoolingMethod != "" && acr.NodeType != 100) || lstShow.Count() == 1) //groups with pooling methods assgined.
							{
								Pooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
							}
						}
					}
					foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
					{
						if (acr.PoolingMethod == "" && acr.NodeType == 100) // studies aren't pooled to any groups.
						{
							UnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
						}

					}
				}
			}
			catch (Exception ex)
			{
			}

		}

		private void cbPoolingWindowIncidence_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (rbIncidenceAll.Checked)
			{
				LoadAllIncidencePooling(ref dicIncidencePoolingAndAggregation, ref dicIncidencePoolingAndAggregationUnPooled);
				if (btShowDetailIncidence.Text == "Show aggregated")
				{
					olvIncidence.SetObjects(dicIncidencePoolingAndAggregation);

				}
				else
				{
					olvIncidence.SetObjects(dicIncidencePoolingAndAggregationUnPooled);
				}
				olvIncidence.SelectAll();
				if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabPoolingIncidence")
				{

					tlvIncidence_DoubleClick(sender, e);
				}

				return;
			}
			if (CommonClass.ValuationMethodPoolingAndAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null) return;
			ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowIncidence.SelectedIndex].ToString()).First();
			dicIncidencePoolingAndAggregation = new Dictionary<AllSelectCRFunction, string>();
			dicIncidencePoolingAndAggregationUnPooled = new Dictionary<AllSelectCRFunction, string>();
			var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1);
			foreach (AllSelectCRFunction allSelectCRFunction in query)
			{
				List<AllSelectCRFunction> lstShow = new List<AllSelectCRFunction>();
				lstShow.Add(allSelectCRFunction);
				getChildFromAllSelectCRFunctionUnPooled(allSelectCRFunction, vb, ref lstShow);

				foreach (AllSelectCRFunction acr in lstShow)
				{
					if (acr.PoolingMethod != "None")
					{
						dicIncidencePoolingAndAggregation.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
					}
				}
			}
			foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
			{
				if (acr.PoolingMethod == "")
				{
					dicIncidencePoolingAndAggregationUnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
				}

			}
			if (btShowDetailIncidence.Text == "Show aggregated")
			{
				olvIncidence.SetObjects(dicIncidencePoolingAndAggregation);

			}
			else
			{
				olvIncidence.SetObjects(dicIncidencePoolingAndAggregationUnPooled);
			}
			olvIncidence.SelectAll();
			if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabPoolingIncidence")
			{
				tlvIncidence_DoubleClick(sender, e);
			}


		}

		private void picCRHelp_Click(object sender, EventArgs e)
		{
			this.toolTip1.Show("Double click datagrid to create result.\r\nIf you choose \'Create map,data and chart \',GIS Map/Table" +
							"/Chart results will be created.\r\nIf you choose \'Create data (table) for multiple studies\',Only one acti" +
							"ve result will be created.", picCRHelp, 5000);
		}

		private void btnAuditTrailOutput_Click(object sender, EventArgs e)
		{
			try
			{
				SaveFileDialog sDlg = new SaveFileDialog();
				sDlg.Title = "Save Audit Trail Report to XML Document or Text";
				sDlg.Filter = "TXT Files (*.txt)|*.txt|CTLX Files (*.ctlx)|*.ctlx|XML Files (*.xml)|*.xml";
				sDlg.AddExtension = true;
				sDlg.InitialDirectory = CommonClass.ResultFilePath;// added from merge develop
				sDlg.RestoreDirectory = true;// added from merge develop
				bool saveOk = false;
				if (rbAuditFile.Checked)
				{
					if (string.IsNullOrEmpty(txtExistingConfiguration.Text))
						return;
				}
				else
				{
					if (CommonClass.ValuationMethodPoolingAndAggregation != null)
					{

					}
					else if (CommonClass.BaseControlCRSelectFunction != null)
					{
					}
					else
					{
						MessageBox.Show("Please finish your configuration first.");
						return;
					}

				}
				if (sDlg.ShowDialog() == DialogResult.OK && sDlg.FileName != string.Empty)
				{
					switch (Path.GetExtension(sDlg.FileName))
					{
						case ".ctlx":
							if (rbAuditFile.Checked)
							{
								string filePath = txtExistingConfiguration.Text;
								if (!System.IO.File.Exists(filePath))
								{
									MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK);
									return;
								}
								string fileType = Path.GetExtension(txtExistingConfiguration.Text);
								switch (fileType)
								{
									case ".aqgx":
										BenMAPLine aqgBenMAPLine = new BenMAPLine();
										string err = "";
										aqgBenMAPLine = DataSourceCommonClass.LoadAQGFile(filePath, ref err);
										if (aqgBenMAPLine == null)
										{
											MessageBox.Show(err);
											return;
										}
										BatchCommonClass.OutputAQG(aqgBenMAPLine, sDlg.FileName, filePath);
										break;
									case ".cfgx":
										BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
										err = "";
										cfgFunction = ConfigurationCommonClass.loadCFGFile(filePath, ref err);
										if (cfgFunction == null)
										{
											MessageBox.Show(err);
											return;
										}
										BatchCommonClass.OutputCFG(cfgFunction, sDlg.FileName, filePath);
										break;
									case ".cfgrx":
										BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
										err = "";
										cfgrFunctionCV = ConfigurationCommonClass.LoadCFGRFile(filePath, ref err);
										if (cfgrFunctionCV == null)
										{
											MessageBox.Show(err);
											return;
										}
										BaseControlCRSelectFunction bc = new BaseControlCRSelectFunction();
										bc.CRLatinHypercubePoints = cfgrFunctionCV.CRLatinHypercubePoints;
										bc.CRDefaultMonteCarloIterations = cfgrFunctionCV.CRDefaultMonteCarloIterations;
										bc.CRRunInPointMode = cfgrFunctionCV.CRRunInPointMode;
										bc.CRThreshold = cfgrFunctionCV.CRThreshold;
										bc.CRSeeds = cfgrFunctionCV.CRSeeds;
										bc.RBenMapGrid = cfgrFunctionCV.RBenMapGrid;
										bc.BenMAPPopulation = cfgrFunctionCV.BenMAPPopulation;
										bc.BaseControlGroup = cfgrFunctionCV.BaseControlGroup;
										List<CRSelectFunction> lstCRSelectFunction = new List<CRSelectFunction>();
										foreach (CRSelectFunctionCalculateValue crfc in cfgrFunctionCV.lstCRSelectFunctionCalculateValue)
										{
											lstCRSelectFunction.Add(crfc.CRSelectFunction);
										}
										bc.lstCRSelectFunction = lstCRSelectFunction;
										BatchCommonClass.OutputCFG(bc, sDlg.FileName, "");
										ConfigurationCommonClass.SaveCFGFile(bc, sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "cfgx");
										break;
									case ".apvx":
										ValuationMethodPoolingAndAggregation apvVMPA = new ValuationMethodPoolingAndAggregation();
										err = "";
										apvVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
										if (apvVMPA == null)
										{
											MessageBox.Show(err);
											return;
										}
										BatchCommonClass.OutputAPV(apvVMPA, sDlg.FileName, filePath);
										break;
									case ".apvrx":
										ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
										err = "";
										apvrVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
										if (apvrVMPA == null)
										{
											MessageBox.Show(err);
											return;
										}
										APVX.APVCommonClass.SaveAPVFile(sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "apvx", apvrVMPA);
										BatchCommonClass.OutputAPV(apvrVMPA, sDlg.FileName, "");
										break;
								}
								MessageBox.Show("Configuration file saved.", "File saved");
							}
							else if (rbAuditCurrent.Checked)
							{
								int retVal = AuditTrailReportCommonClass.exportToCtlx(sDlg.FileName);
								if (retVal == -1)
								{
									MessageBox.Show("Please finish your configuration first.");
								}
								else
								{
									MessageBox.Show("Configuration file saved.", "File saved");
								}
							}
							break;
						case ".txt":
							//MERGE CHECK
							List<TreeNode> lstTmp = new List<TreeNode>();//treeListView.Objects as List<TreeNode>;
							for (int i = 0; i < (treeListView.Objects as ArrayList).ToArray().Count(); i++)
							{
								lstTmp.Add((treeListView.Objects as ArrayList).ToArray()[i] as TreeNode);
							}
							if (lstTmp.Count > 0)
							{
								saveOk = exportToTxt(lstTmp, sDlg.FileName);
							}
							break;
						case ".xml":
							//MERGE CHECK
							List<TreeNode> lstTmpXML = new List<TreeNode>();//treeListView.Objects as List<TreeNode>;
							for (int i = 0; i < (treeListView.Objects as ArrayList).ToArray().Count(); i++)
							{
								lstTmpXML.Add((treeListView.Objects as ArrayList).ToArray()[i] as TreeNode);
							}
							if (lstTmpXML.Count > 0)
							{
								saveOk = exportToXml(lstTmpXML, sDlg.FileName);
							}
							break;
					}
				}
				if (saveOk)
				{
					MessageBox.Show("Audit trail report saved.", "File saved");
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		public bool exportToTxt(List<TreeNode> tv, string filename)
		{
			try
			{
				FileStream fs = new FileStream(filename, FileMode.Create);
				sw = new StreamWriter(fs, Encoding.UTF8);
				for (int i = 0; i < tv.Count; i++)
				{
					if (tv[i].Nodes.Count > 0)
					{
						//sw.WriteLine(tv[i].Nodes[0].Text);
						sw.WriteLine("<" + tv[i].Text + ">");
						foreach (TreeNode node in tv[i].Nodes)
						{

							if (node.Nodes.Count >= 1) //updated to address BenMAP 258/246--printing the text of first-level parent node  (11/26/2019,MP)
							{
								sw.WriteLine("<" + node.Text + ">");
								saveNode(node.Nodes);
								sw.WriteLine("</" + node.Text + ">");
							}
							else
								sw.WriteLine(node.Text);
						}
						sw.WriteLine("</" + tv[i].Text + ">");
					}
					else
						sw.WriteLine(tv[i].Text);
				}
				sw.Close();
				fs.Close();

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return false;
			}
		}

		public bool exportToXml(List<TreeNode> tv, string filename)
		{
			try
			{
				sw = new StreamWriter(filename, false, System.Text.Encoding.UTF8);
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
				string file = Path.GetFileNameWithoutExtension(filename);
				file = file.Replace(" ", ".");
				file = file.Replace(",", ".");
				file = file.Replace("&", "And");
				file = file.Replace(":", "");
				file = file.Replace("..", ".");
				sw.WriteLine("<" + file + ">");
				for (int i = 0; i < tv.Count; i++)
				{
					if (tv[i].Nodes.Count > 0)
					{
						string txtWithoutSpace = tv[i].Text;
						txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
						txtWithoutSpace = txtWithoutSpace.Replace(",", ".");
						txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
						txtWithoutSpace = txtWithoutSpace.Replace(":", "");
						txtWithoutSpace = txtWithoutSpace.Replace("..", ".");
						txtWithoutSpace = txtWithoutSpace.Replace("#", "");
						sw.WriteLine("<" + txtWithoutSpace + ">");

						foreach (TreeNode node in tv[i].Nodes)
						{
							string nodeText = node.Text.Replace(" ", ".");
							nodeText = nodeText.Replace(",", ".");
							nodeText = nodeText.Replace("&", "And");
							nodeText = nodeText.Replace(":", "");
							nodeText = nodeText.Replace("..", ".");
							nodeText = nodeText.Replace("#", "");

							if (node.Nodes.Count > 1) //updated to address BenMAP 258/246--printing the text of first-level parent node (11/26/2019,MP) 
							{
								sw.WriteLine("<" + nodeText + ">");
								saveNode(node.Nodes);
								sw.WriteLine("</" + nodeText + ">");
							}
							else
								sw.WriteLine(nodeText);
						}
						//Close the root node
						sw.WriteLine("</" + txtWithoutSpace + ">");
					}
					else
						sw.WriteLine(tv[i].Text);
				}
				sw.WriteLine("</" + file + ">");
				sw.Close();

				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return false;
			}
		}

		private void saveNode(TreeNodeCollection tnc)
		{
			foreach (TreeNode node in tnc)
			{
				if (node.Nodes.Count > 0)
				{
					string txtWithoutSpace = node.Text;
					txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
					txtWithoutSpace = txtWithoutSpace.Replace(",", ".");
					txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
					txtWithoutSpace = txtWithoutSpace.Replace(":", "");
					txtWithoutSpace = txtWithoutSpace.Replace("..", ".");

					sw.WriteLine("<" + txtWithoutSpace + ">");
					saveNode(node.Nodes);
					sw.WriteLine("</" + txtWithoutSpace + ">");
				}
				else sw.WriteLine(node.Text);
			}
		}

		private void btShowCRResult_Click(object sender, EventArgs e)
		{
			try
			{
				_RaiseLayerChangeEvents = false;

				if (olvCRFunctionResult.SelectedObjects == null || olvCRFunctionResult.SelectedObjects.Count == 0)
					return;
				string Tip = "Drawing configuration results layer";
				WaitShow(Tip);
				this.mainFrm.Enabled = false;
				bool bGIS = true;
				bool bTable = true;
				bool bChart = true;

				//Get the ID of the grid (shapefile) that we are using for computational units
				int iOldGridType = CommonClass.GBenMAPGrid.GridDefinitionID;

				CRSelectFunctionCalculateValue crSelectFunctionCalculateValueForChart = null;
				for (int icro = 0; icro < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; icro++)
				{
					CRSelectFunctionCalculateValue cro = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[icro];
					Configuration.ConfigurationCommonClass.ClearCRSelectFunctionCalculateValueLHS(ref cro);
				}
				List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
				foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
				{
					lstCRSelectFunctionCalculateValue.Add(cr);
					if (crSelectFunctionCalculateValueForChart == null) crSelectFunctionCalculateValueForChart = cr;
				}
				if (lstCRSelectFunctionCalculateValue.Count != 0)
				{
					if (cbCRAggregation.SelectedIndex != -1 && cbCRAggregation.SelectedIndex != 0)
					{
						DataRowView drv = cbCRAggregation.SelectedItem as DataRowView;
						int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
						if (iAggregationGridType != CommonClass.GBenMAPGrid.GridDefinitionID)
						{
							List<CRSelectFunctionCalculateValue> lstTemp = new List<CRSelectFunctionCalculateValue>();
							foreach (CRSelectFunctionCalculateValue cr in lstCRSelectFunctionCalculateValue)
							{
								lstTemp.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(cr, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType));
							}
							if (! APVX.APVCommonClass.PruneToGeographicAreas(lstTemp, iAggregationGridType, out string msg))
							{
								MessageBox.Show(msg);
							}
							lstCRSelectFunctionCalculateValue = lstTemp;
							CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iAggregationGridType);
						}

					}
					if (lstCRSelectFunctionCalculateValue[0].CRCalculateValues.Count == 1 && !CommonClass.CRRunInPointMode)
					{
						lstCFGRforCDF = lstCRSelectFunctionCalculateValue;
						canshowCDF = true;
					}
					else
					{
						lstCFGRforCDF = new List<CRSelectFunctionCalculateValue>();
						canshowCDF = false;
					}
					iCDF = 0;
					// ClearMapTableChart();
					if (rdbShowActiveCR.Checked)
					{
						if (tabCtlMain.SelectedIndex == 0)
						{
							bTable = false;
							bChart = false;
						}
						else if (tabCtlMain.SelectedIndex == 1)
						{
							bGIS = false;
							bChart = false;
						}
						else if (tabCtlMain.SelectedIndex == 2)
						{
							bGIS = false;
							bTable = false;
						}
					}

					InitTableResult(lstCRSelectFunctionCalculateValue);
					InitChartResult(crSelectFunctionCalculateValueForChart, iOldGridType);
					DrawMapResults(lstCRSelectFunctionCalculateValue, bTable);

					CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
				}


				MoveAdminGroupToTop();
				_RaiseLayerChangeEvents = true;
				WaitClose();
				this.mainFrm.Enabled = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				WaitClose();
			}

		}

		private void btSelectAttribute_Click(object sender, EventArgs e)
		{
			try
			{
				ConfigurationResultsReport frm = new ConfigurationResultsReport();
				frm.lstColumnRow = cflstColumnRow;
				frm.lstHealth = cflstHealth;
				frm.lstResult = cflstResult;
				frm.sArrayPercentile = strHealthImpactPercentiles;
				frm.userAssignPercentile = assignHealthImpactPercentile;
				DialogResult rt = frm.ShowDialog();
				if (rt != System.Windows.Forms.DialogResult.OK) return;
				cflstColumnRow = frm.lstColumnRow;
				cflstHealth = frm.lstHealth;
				cflstResult = frm.lstResult;
				strHealthImpactPercentiles = frm.sArrayPercentile;
				assignHealthImpactPercentile = frm.userAssignPercentile;


				if (_tableObject != null)
				{
					int pagecurrent = _pageCurrent;
					InitTableResult(_tableObject);
					_pageCurrent = pagecurrent;
					UpdateTableResult(_tableObject);
					bindingNavigatorPositionItem.Text = pagecurrent.ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btPoolingSelectAttribute_Click(object sender, EventArgs e)
		{
			try
			{
				ConfigurationResultsReport frm = new ConfigurationResultsReport();
				frm.lstColumnRow = IncidencelstColumnRow;
				frm.lstHealth = IncidencelstHealth;
				frm.lstResult = IncidencelstResult;
				frm.sArrayPercentile = strPoolIncidencePercentiles;
				frm.userAssignPercentile = assignPoolIncidencePercentile;
				frm.isPooledIncidence = true;
				DialogResult rt = frm.ShowDialog();
				if (rt != System.Windows.Forms.DialogResult.OK) return;
				IncidencelstColumnRow = frm.lstColumnRow;
				IncidencelstHealth = frm.lstHealth;
				IncidencelstResult = frm.lstResult;
				strPoolIncidencePercentiles = frm.sArrayPercentile;
				assignPoolIncidencePercentile = frm.userAssignPercentile;

				if (_tableObject != null)
				{
					int pagecurrent = _pageCurrent;
					InitTableResult(_tableObject);
					_pageCurrent = pagecurrent;
					UpdateTableResult(_tableObject);
					bindingNavigatorPositionItem.Text = pagecurrent.ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
				string shapeFileName;

				if (isLoad)
				{
					if (mainMap.GetAllLayers().Count > 1)
					{
						if (CommonClass.RBenMAPGrid == null)
						{
							mainMap.Layers.Clear();
							CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
						}
						else
						{
							if (CommonClass.RBenMAPGrid is ShapefileGrid)
							{
								shapeFileName = ((ShapefileGrid)CommonClass.RBenMAPGrid).GridDefinitionName;
							}
							else
							{
								shapeFileName = ((RegularGrid)CommonClass.RBenMAPGrid).GridDefinitionName;
							}
							for (int i = 0; i < mainMap.GetAllLayers().Count; i++)
							{
								if (mainMap.GetAllLayers()[i].LegendText == shapeFileName)
								{
									mainMap.GetAllLayers().RemoveAt(i);
									break;
								}
							}

							CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
							addAdminLayers();
						}

					}
					else
					{
						CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
					}
				}

				CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
			}
			catch (Exception E)
			{
				Console.WriteLine("Error setting up grids: " + e.ToString());

			}
		}

		private void btAPVSelectAttribute_Click(object sender, EventArgs e)
		{
			try
			{
				APVResultsReport frm = new APVResultsReport();
				frm.lstColumnRow = apvlstColumnRow;
				frm.lstHealth = apvlstHealth;
				frm.lstResult = apvlstResult;
				frm.sArrayPercentile = strAPVPercentiles;
				frm.userAssignPercentile = assignAPVPercentile;
				DialogResult rt = frm.ShowDialog();
				if (rt != System.Windows.Forms.DialogResult.OK) return;
				apvlstColumnRow = frm.lstColumnRow;
				apvlstHealth = frm.lstHealth;
				apvlstResult = frm.lstResult;
				strAPVPercentiles = frm.sArrayPercentile;
				assignAPVPercentile = frm.userAssignPercentile;
				if (_tableObject != null)
				{
					int pagecurrent = _pageCurrent;
					InitTableResult(_tableObject);
					_pageCurrent = pagecurrent;
					UpdateTableResult(_tableObject);
					bindingNavigatorPositionItem.Text = pagecurrent.ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btQALYSelectAttribute_Click(object sender, EventArgs e)
		{
			try
			{
				QALYResultsReport frm = new QALYResultsReport();
				frm.lstColumnRow = qalylstColumnRow;
				frm.lstHealth = qalylstHealth;
				frm.lstResult = qalylstResult;
				DialogResult rt = frm.ShowDialog();
				if (rt != System.Windows.Forms.DialogResult.OK) return;
				qalylstColumnRow = frm.lstColumnRow;
				qalylstHealth = frm.lstHealth;
				qalylstResult = frm.lstResult;
				if (_tableObject != null)
				{
					int pagecurrent = _pageCurrent;
					InitTableResult(_tableObject);
					_pageCurrent = pagecurrent;
					UpdateTableResult(_tableObject);
					bindingNavigatorPositionItem.Text = pagecurrent.ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void OLVResultsShow_BeforeSorting(object sender, BeforeSortingEventArgs e)
		{


		}

		private void OLVResultsShow_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			try
			{
				if (_tableObject == null || sender == null) return;
				int i = 0;
				//HIF
				if (_tableObject is List<CRSelectFunctionCalculateValue> || _tableObject is CRSelectFunctionCalculateValue)
				{

					List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
					if (_tableObject is List<CRSelectFunctionCalculateValue>)
						lstCRTable = (List<CRSelectFunctionCalculateValue>)_tableObject;
					else
						lstCRTable.Add(_tableObject as CRSelectFunctionCalculateValue);
					foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
					{
						cr.CRCalculateValues = cr.CRCalculateValues.Where(p => p.Population > 0).ToList();
						if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
						{

							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":

									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Col).ToList();

									break;
								case "row":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Row).ToList();
									break;
								case "pointestimate":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.PointEstimate).ToList();
									break;
								case "population":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Population).ToList();
									break;
								case "delta":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Delta).ToList();
									break;
								case "mean":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Mean).ToList();
									break;
								case "baseline":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Baseline).ToList();
									break;
								case "percentofbaseline":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.PercentOfBaseline).ToList();
									break;
								case "standarddeviation":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10) == "Percentile")
									{
										cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.LstPercentile[cr.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}
						}
						else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":

									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Col).ToList();

									break;
								case "row":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Row).ToList();
									break;
								case "pointestimate":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.PointEstimate).ToList();
									break;
								case "population":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Population).ToList();
									break;
								case "delta":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Delta).ToList();
									break;
								case "mean":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Mean).ToList();
									break;
								case "baseline":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Baseline).ToList();
									break;
								case "percentofbaseline":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.PercentOfBaseline).ToList();
									break;
								case "standarddeviation":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
									{
										cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.LstPercentile[cr.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}
						}
					}
					if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
								break;
							case "endpointgroup":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
								break;
							case "pollutant":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
								break;
							case "metric":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
								break;
							case "seasonalmetric":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
								break;
							case "metricstatistic":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
								break;
							case "author":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
								break;
							case "year":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
								break;
							case "geographicarea":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.GeographicAreaName).ToList();
								break;
							case "otherpollutants":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
								break;
							case "reference":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
								break;
							case "race":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.Race).ToList();
								break;
							case "ethnicity":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.Ethnicity).ToList();
								break;
							case "gender":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.Gender).ToList();
								break;
							case "startage":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.StartAge).ToList();
								break;
							case "endage":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.EndAge).ToList();
								break;
							case "function":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
								break;
							case "incidencedataset":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.IncidenceDataSetName).ToList();
								break;
							case "prevalencedataset":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.PrevalenceDataSetName).ToList();
								break;
							case "beta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
								break;
							case "disbeta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
								break;
							case "p1beta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
								break;
							case "p2beta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
								break;
							case "a":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
								break;
							case "namea":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
								break;
							case "b":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
								break;
							case "nameb":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
								break;
							case "c":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
								break;
							case "namec":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
								break;

						}
					}
					else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
								break;
							case "endpointgroup":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
								break;
							case "pollutant":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
								break;
							case "metric":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
								break;
							case "seasonalmetric":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
								break;
							case "metricstatistic":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
								break;
							case "author":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
								break;
							case "year":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
								break;
							case "geographicarea":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.GeographicAreaName).ToList();
								break;
							case "otherpollutants":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
								break;
							case "reference":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
								break;
							case "race":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.Race).ToList();
								break;
							case "ethnicity":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.Ethnicity).ToList();
								break;
							case "gender":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.Gender).ToList();
								break;
							case "startage":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.StartAge).ToList();
								break;
							case "endage":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.EndAge).ToList();
								break;
							case "function":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
								break;
							case "incidencedataset":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.IncidenceDataSetName).ToList();
								break;
							case "prevalencedataset":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.PrevalenceDataSetName).ToList();
								break;
							case "beta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
								break;
							case "disbeta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
								break;
							case "p1beta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
								break;
							case "p2beta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
								break;
							case "a":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
								break;
							case "namea":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
								break;
							case "b":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
								break;
							case "nameb":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
								break;
							case "c":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
								break;
							case "namec":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
								break;

						}
					}



					_tableObject = lstCRTable;
					UpdateTableResult(lstCRTable);
				}
				//Incidence Pooling (new)
				else if (_tableObject is Dictionary<AllSelectCRFunction, string>)
				{
					Dictionary<AllSelectCRFunction, string> dicAllSelectCRFunctionPoolName = (Dictionary<AllSelectCRFunction, string>)_tableObject;
					Dictionary<AllSelectCRFunction, string> dicNew = new Dictionary<AllSelectCRFunction, string>();
					//sort by pooling name
					if ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower() == "pooling name")
					{
						if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
						{
							var items = from pair in dicAllSelectCRFunctionPoolName orderby pair.Value ascending select pair;
							dicNew = (Dictionary<AllSelectCRFunction, string>)items;
						}
						else
						{
							var items = from pair in dicAllSelectCRFunctionPoolName orderby pair.Value descending select pair;
							dicNew = (Dictionary<AllSelectCRFunction, string>)items;
						}
					}
					else
					{
						List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
						foreach (KeyValuePair<AllSelectCRFunction, string> dicKey1 in dicAllSelectCRFunctionPoolName)
						{
							lstCR.Add(dicKey1.Key);
						}
						//sort by CR function fields
						if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "dataset":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
									break;
								case "endpointgroup":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
									break;
								case "endpoint":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
									break;
								case "pollutant":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
									break;
								case "metric":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
									break;
								case "seasonalmetric":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
									break;
								case "metricstatistic":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
									break;
								case "author":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
									break;
								case "year":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
									break;
								case "geographicarea":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName).ToList();
									break;
								case "otherpollutants":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
									break;
								case "qualifier":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
									break;
								case "reference":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
									break;
								case "race":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Race).ToList();
									break;
								case "ethnicity":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Ethnicity).ToList();
									break;
								case "gender":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Gender).ToList();
									break;
								case "startage":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge).ToList();
									break;
								case "endage":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge).ToList();
									break;
								case "function":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
									break;
								case "incidencedataset":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName).ToList();
									break;
								case "prevalencedataset":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.PrevalenceDataSetName).ToList();
									break;
								case "beta":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
									break;
								case "disbeta":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
									break;
								case "p1beta":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
									break;
								case "p2beta":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
									break;
								case "a":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
									break;
								case "namea":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
									break;
								case "b":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
									break;
								case "nameb":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
									break;
								case "c":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
									break;
								case "namec":
									lstCR = lstCR.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
									break;

							}
						}
						else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{

								case "dataset":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
									break;
								case "endpointgroup":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
									break;
								case "endpoint":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
									break;
								case "pollutant":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
									break;
								case "metric":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
									break;
								case "seasonalmetric":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
									break;
								case "metricstatistic":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
									break;
								case "author":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
									break;
								case "year":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
									break;
								case "geographicarea":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName).ToList();
									break;
								case "otherpollutants":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
									break;
								case "qualifier":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
									break;
								case "reference":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
									break;
								case "race":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Race).ToList();
									break;
								case "ethnicity":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Ethnicity).ToList();
									break;
								case "gender":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Gender).ToList();
									break;
								case "startage":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge).ToList();
									break;
								case "endage":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge).ToList();
									break;
								case "function":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
									break;
								case "incidencedataset":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName).ToList();
									break;
								case "prevalencedataset":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.PrevalenceDataSetName).ToList();
									break;
								case "beta":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
									break;
								case "disbeta":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
									break;
								case "p1beta":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
									break;
								case "p2beta":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
									break;
								case "a":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
									break;
								case "namea":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
									break;
								case "b":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
									break;
								case "nameb":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
									break;
								case "c":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
									break;
								case "namec":
									lstCR = lstCR.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
									break;
							}
						}

						//sort by CR function Calculated Value fields
						foreach (AllSelectCRFunction cr in lstCR)
						{
							if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
							{
								switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
								{
									case "column":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Col).ToList();
										break;
									case "row":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Row).ToList();
										break;
									case "pointestimate":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.PointEstimate).ToList();
										break;
									case "population":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Population).ToList();
										break;
									case "delta":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Delta).ToList();
										break;
									case "mean":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Mean).ToList();
										break;
									case "baseline":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Baseline).ToList();
										break;
									case "percentofbaseline":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.PercentOfBaseline).ToList();
										break;
									case "standarddeviation":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.StandardDeviation).ToList();
										break;
									case "variance":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Variance).ToList();
										break;
									default:
										if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10) == "Percentile")
										{
											cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.LstPercentile[cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();
										}
										break;
								}
							}
							else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
							{
								switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
								{
									case "column":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Col).ToList();
										break;
									case "row":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Row).ToList();
										break;
									case "pointestimate":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.PointEstimate).ToList();
										break;
									case "population":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Population).ToList();
										break;
									case "delta":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Delta).ToList();
										break;
									case "mean":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Mean).ToList();
										break;
									case "baseline":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Baseline).ToList();
										break;
									case "percentofbaseline":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.PercentOfBaseline).ToList();
										break;
									case "standarddeviation":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.StandardDeviation).ToList();
										break;
									case "variance":
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Variance).ToList();
										break;
									default:
										if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
										{
											cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.LstPercentile[cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

										}
										break;

								}
							}
						}

						//add sorted CR functions to dicNew
						foreach (AllSelectCRFunction cr in lstCR)
						{
							dicNew.Add(cr, dicAllSelectCRFunctionPoolName[cr]);
						}

						_tableObject = dicNew;
						UpdateTableResult(dicNew);
					}






				}
				//Incidence Pooling (old and obsolete)
				else if (_tableObject is List<AllSelectCRFunction> || _tableObject is AllSelectCRFunction)
				{

					List<AllSelectCRFunction> lstCRTable = new List<AllSelectCRFunction>();
					if (_tableObject is List<AllSelectCRFunction>)
						lstCRTable = (List<AllSelectCRFunction>)_tableObject;
					else
						lstCRTable.Add(_tableObject as AllSelectCRFunction);
					foreach (AllSelectCRFunction cr in lstCRTable)
					{
						cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.Where(p => p.Population > 0).ToList();
						if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
						{

							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":

									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Col).ToList();

									break;
								case "row":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Row).ToList();
									break;
								case "pointestimate":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.PointEstimate).ToList();
									break;
								case "population":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Population).ToList();
									break;
								case "delta":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Delta).ToList();
									break;
								case "mean":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Mean).ToList();
									break;
								case "baseline":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Baseline).ToList();
									break;
								case "percentofbaseline":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.PercentOfBaseline).ToList();
									break;
								case "standarddeviation":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10) == "Percentile")
									{
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.LstPercentile[cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}
						}
						else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":

									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Col).ToList();

									break;
								case "row":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Row).ToList();
									break;
								case "pointestimate":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.PointEstimate).ToList();
									break;
								case "population":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Population).ToList();
									break;
								case "delta":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Delta).ToList();
									break;
								case "mean":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Mean).ToList();
									break;
								case "baseline":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Baseline).ToList();
									break;
								case "percentofbaseline":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.PercentOfBaseline).ToList();
									break;
								case "standarddeviation":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
									{
										cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.LstPercentile[cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}
						}
					}
					if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
								break;
							case "endpointgroup":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
								break;
							case "pollutant":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
								break;
							case "metric":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
								break;
							case "seasonalmetric":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
								break;
							case "metricstatistic":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
								break;
							case "author":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
								break;
							case "year":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
								break;
							case "geographicarea":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName).ToList();
								break;
							case "otherpollutants":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
								break;
							case "reference":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
								break;
							case "race":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Race).ToList();
								break;
							case "ethnicity":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Ethnicity).ToList();
								break;
							case "gender":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Gender).ToList();
								break;
							case "startage":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge).ToList();
								break;
							case "endage":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge).ToList();
								break;
							case "function":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
								break;
							case "incidencedataset":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName).ToList();
								break;
							case "prevalencedataset":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.PrevalenceDataSetName).ToList();
								break;
							case "beta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
								break;
							case "disbeta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
								break;
							case "p1beta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
								break;
							case "p2beta":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
								break;
							case "a":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
								break;
							case "namea":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
								break;
							case "b":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
								break;
							case "nameb":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
								break;
							case "c":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
								break;
							case "namec":
								lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
								break;

						}
					}
					else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
								break;
							case "endpointgroup":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
								break;
							case "pollutant":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
								break;
							case "metric":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
								break;
							case "seasonalmetric":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
								break;
							case "metricstatistic":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
								break;
							case "author":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
								break;
							case "year":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
								break;
							case "geographicarea":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName).ToList();
								break;
							case "otherpollutants":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
								break;
							case "reference":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
								break;
							case "race":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Race).ToList();
								break;
							case "ethnicity":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Ethnicity).ToList();
								break;
							case "gender":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Gender).ToList();
								break;
							case "startage":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge).ToList();
								break;
							case "endage":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge).ToList();
								break;
							case "function":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
								break;
							case "incidencedataset":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName).ToList();
								break;
							case "prevalencedataset":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.PrevalenceDataSetName).ToList();
								break;
							case "beta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
								break;
							case "disbeta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
								break;
							case "p1beta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
								break;
							case "p2beta":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
								break;
							case "a":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
								break;
							case "namea":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
								break;
							case "b":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
								break;
							case "nameb":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
								break;
							case "c":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
								break;
							case "namec":
								lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
								break;

						}
					}
					_tableObject = lstCRTable;
					UpdateTableResult(lstCRTable);
				}
				else if (_tableObject is List<AllSelectValuationMethodAndValue> || _tableObject is AllSelectValuationMethodAndValue)
				{
					List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
					if (_tableObject is List<AllSelectValuationMethodAndValue>)
					{
						lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)_tableObject;
					}
					else
					{
						lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)_tableObject);
					}
					foreach (AllSelectValuationMethodAndValue avmav in lstallSelectValuationMethodAndValue)
					{
						if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Col).ToList();
									break;
								case "row":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Row).ToList();
									break;
								case "pointestimate":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.PointEstimate).ToList();
									break;

								case "mean":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Mean).ToList();
									break;

								case "standarddeviation":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
									{
										avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.LstPercentile[avmav.lstAPVValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}

						}
						else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Col).ToList();
									break;
								case "row":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Row).ToList();
									break;
								case "pointestimate":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.PointEstimate).ToList();
									break;

								case "mean":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Mean).ToList();
									break;

								case "standarddeviation":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
									{
										avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.LstPercentile[avmav.lstAPVValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}
						}
					}
					if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.DataSet).ToList();
								break;
							case "endpointgroup":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.EndPoint).ToList();
								break;
							case "pollutant":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Pollutant).ToList();
								break;

							case "author":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Author).ToList();
								break;
							case "year":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Year).ToList();
								break;
							case "location":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Location).ToList();
								break;
							case "otherpollutants":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Qualifier).ToList();
								break;

							case "race":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Race).ToList();
								break;
							case "ethnicity":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Ethnicity).ToList();
								break;
							case "gender":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Gender).ToList();
								break;
							case "startage":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.StartAge).ToList();
								break;
							case "endage":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.EndAge).ToList();
								break;
							case "function":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Function).ToList();
								break;

							case "version":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Version).ToList();
								break;
							case "agerange":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.AgeRange).ToList();
								break;
						}

					}
					else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.DataSet).ToList();
								break;
							case "endpointgroup":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.EndPoint).ToList();
								break;
							case "pollutant":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Pollutant).ToList();
								break;

							case "author":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Author).ToList();
								break;
							case "year":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Year).ToList();
								break;
							case "location":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Location).ToList();
								break;
							case "otherpollutants":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Qualifier).ToList();
								break;

							case "race":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Race).ToList();
								break;
							case "ethnicity":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Ethnicity).ToList();
								break;
							case "gender":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Gender).ToList();
								break;
							case "startage":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.StartAge).ToList();
								break;
							case "endage":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.EndAge).ToList();
								break;
							case "function":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Function).ToList();
								break;

							case "version":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Version).ToList();
								break;
							case "agerange":
								lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.AgeRange).ToList();
								break;
						}
					}
					_tableObject = lstallSelectValuationMethodAndValue;
					UpdateTableResult(lstallSelectValuationMethodAndValue);
				}

				else if (_tableObject is List<AllSelectQALYMethodAndValue> || _tableObject is AllSelectQALYMethodAndValue)
				{
					List<AllSelectQALYMethodAndValue> lstallSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
					if (_tableObject is List<AllSelectQALYMethodAndValue>)
					{
						lstallSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)_tableObject;
					}
					else
					{
						lstallSelectQALYMethodAndValue.Add((AllSelectQALYMethodAndValue)_tableObject);
					}
					foreach (AllSelectQALYMethodAndValue avmav in lstallSelectQALYMethodAndValue)
					{
						if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Col).ToList();
									break;
								case "row":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Row).ToList();
									break;
								case "pointestimate":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.PointEstimate).ToList();
									break;

								case "mean":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Mean).ToList();
									break;

								case "standarddeviation":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
									{
										avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.LstPercentile[avmav.lstQALYValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}

						}
						else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
						{
							switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
							{
								case "column":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Col).ToList();
									break;
								case "row":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Row).ToList();
									break;
								case "pointestimate":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.PointEstimate).ToList();
									break;

								case "mean":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Mean).ToList();
									break;

								case "standarddeviation":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.StandardDeviation).ToList();
									break;
								case "variance":
									avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Variance).ToList();
									break;
								default:
									if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
									{
										avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.LstPercentile[avmav.lstQALYValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

									}
									break;

							}
						}
					}
					if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.DataSet).ToList();
								break;
							case "endpointgroup":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.EndPoint).ToList();
								break;
							case "pollutant":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Pollutant).ToList();
								break;

							case "author":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Author).ToList();
								break;
							case "year":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Year).ToList();
								break;
							case "location":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Location).ToList();
								break;
							case "otherpollutants":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Qualifier).ToList();
								break;

							case "race":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Race).ToList();
								break;
							case "ethnicity":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Ethnicity).ToList();
								break;
							case "gender":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Gender).ToList();
								break;
							case "startage":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.StartAge).ToList();
								break;
							case "endage":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.EndAge).ToList();
								break;
							case "function":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Function).ToList();
								break;

							case "version":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Version).ToList();
								break;
								//no agerange in individual HIF results
						}

					}
					else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{

							case "dataset":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.DataSet).ToList();
								break;
							case "endpointgroup":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.EndPointGroup).ToList();
								break;
							case "endpoint":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.EndPoint).ToList();
								break;
							case "pollutant":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Pollutant).ToList();
								break;

							case "author":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Author).ToList();
								break;
							case "year":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Year).ToList();
								break;
							case "location":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Location).ToList();
								break;
							case "otherpollutants":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.OtherPollutants).ToList();
								break;
							case "qualifier":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Qualifier).ToList();
								break;

							case "race":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Race).ToList();
								break;
							case "ethnicity":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Ethnicity).ToList();
								break;
							case "gender":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Gender).ToList();
								break;
							case "startage":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.StartAge).ToList();
								break;
							case "endage":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.EndAge).ToList();
								break;
							case "function":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Function).ToList();
								break;

							case "version":
								lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Version).ToList();
								break;
								//no age range for individual function results
						}
					}
					_tableObject = lstallSelectQALYMethodAndValue;
					UpdateTableResult(lstallSelectQALYMethodAndValue);
				}
				else if (_tableObject is BenMAPLine)
				{
					BenMAPLine crTable = (BenMAPLine)_tableObject;
					if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{
							case "column":
								crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Col).ToList();
								break;
							case "row":
								crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Row).ToList();
								break;
						}
					}
					else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
					{
						switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
						{
							case "column":
								crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderByDescending(p => p.Col).ToList();
								break;
							case "row":
								crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderByDescending(p => p.Row).ToList();
								break;
						}
					}
					List<string> lstAddField = new List<string>();
					List<double[]> lstResultCopy = new List<double[]>();
					foreach (Metric metric in crTable.Pollutant.Metrics)
					{
						lstAddField.Add(metric.MetricName);
					}
					if (crTable.Pollutant.SesonalMetrics != null)
					{
						foreach (SeasonalMetric sesonalMetric in crTable.Pollutant.SesonalMetrics)
						{
							lstAddField.Add(sesonalMetric.SeasonalMetricName);
						}
					}

					i = 0;
					while (i < lstAddField.Count())
					{

						if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
						{
							if ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "") == lstAddField[i])
								crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Values[lstAddField[i]]).ToList();


						}
						else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
						{
							if ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "") == lstAddField[i])
								crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderByDescending(p => p.Values[lstAddField[i]]).ToList();
						}
						i++;
					}

					_tableObject = crTable;
					UpdateTableResult(_tableObject);
				}
			}
			catch (Exception ex)
			{
			}
		}

		private void BenMAP_Shown(object sender, EventArgs e)
		{
			StartTip st = new StartTip();
			string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
			string isShow = "T";
			if (System.IO.File.Exists(iniPath))
			{
				isShow = CommonClass.IniReadValue("appSettings", "IsShowStart", iniPath);
			}
			if (isShow == "T")
			{ st.Show(); }
		}

		private void btnShowAPVResult_Click(object sender, EventArgs e)
		{
			tlvAPVResult_DoubleClick(sender, e);
		}

		private void trvSetting_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			Rectangle rec = e.Bounds;


			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			e.Graphics.DrawString(e.Node.Text, trvSetting.Font, Brushes.Black, Rectangle.Inflate(e.Bounds, 2, 0), sf);


		}

		private void spTable_Panel2_Paint(object sender, PaintEventArgs e)
		{

		}

		private void btnResultShow_RefreshItems(object sender, EventArgs e)
		{

		}

		private void RemoveOldPolygonLayer(string LayerName, IMapLayerCollection layerList, bool ShrinkOtherLayersInMapGroup = false)
		{
			MapGroup aMGLayer = new MapGroup();
			MapPolygonLayer aPolyLayer = new MapPolygonLayer();
			List<ILayer> layersToRemove = new List<ILayer>();

			//Remove the old version of the layer if exists already
			foreach (ILayer aLayer in layerList)
			{
				if (aLayer is MapGroup || aLayer is IMapGroup) //Look within Map groups
				{
					aMGLayer = (MapGroup)aLayer;
					RemoveOldPolygonLayer(LayerName, aMGLayer.Layers, ShrinkOtherLayersInMapGroup);

				}
				else if (aLayer is FeatureLayer || aLayer is IFeatureLayer) // layer at root level(not in a mapgroup
				{
					if (aLayer is MapPolygonLayer)
					{
						aPolyLayer = (MapPolygonLayer)aLayer;

						if (aPolyLayer.Name == LayerName)
						{
							layersToRemove.Add(aLayer); //add to list of layers to remove                            
						}
						else if (ShrinkOtherLayersInMapGroup)  // Unexpand this layer to increase display room for new layer
						{
							aLayer.IsExpanded = false;
						}
					}
				}
			}

			//remove layers
			foreach (ILayer layer in layersToRemove)
			{
				layerList.Remove((IMapLayer)layer);
			}

			return;
		}

		private ILayer FindTopVisibleLayer(bool ignoreAdminMapGroup = false)
		{  //Loop through all of the layers and find the topmost visible one - used to update the map title
			 //if ignoreAdminMapGroup = true then ignore the visible layers within that map group when finding the topvislayer.

			ILayer TopVisLayer = null;
			ILayer ThisLayer = null;
			List<ILayer> AllLayers = null;
			AllLayers = mainMap.GetAllLayers();
			AllLayers.Reverse(); //reverser list so Last added one is top visible
			TopVisLayer = null;
			for (int j = 0; j <= AllLayers.Count - 1; j++)
			{
				ThisLayer = AllLayers[j];
				if (ThisLayer.IsVisible)
				{
					if (!ignoreAdminMapGroup)
					{
						TopVisLayer = ThisLayer;
						break;
					}
					else //make sure the layer is not in the admin group
					{
						foreach (MapGroup ThisMG in mainMap.GetAllGroups())
						{
							if (!ThisMG.Contains(ThisLayer))
							//if (ThisMG.LegendText == regionGroupLegendText & !ThisMG.Contains(ThisLayer))
							{
								TopVisLayer = ThisLayer;
								return TopVisLayer;
							}
						}
					}
				}
			}
			//this loop will be true ONLY when there is ONLY admin layer is on the map control 
			// AND user is trying to print the visible admin layer
			if (TopVisLayer == null & mainMap.GetAllLayers().Count > 0)
			{
				TopVisLayer = mainMap.GetAllLayers()[0];
			}
			return TopVisLayer;
		}
		private void tlvIncidence_DoubleClick(object sender, EventArgs e)
		{
			//olvIncidence_DoubleClick
			try
			{
				if (olvIncidence.SelectedObjects.Count == 0) return;
				ClearMapTableChart();
				bool bGIS = true;
				bool bTable = true;
				bool bChart = true;
				int i = 0;
				int iOldGridType = CommonClass.GBenMAPGrid.GridDefinitionID;
				CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = null;
				BenMAPGrid incidenceGrid = CommonClass.GBenMAPGrid;
				if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
											CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
				{
					incidenceGrid = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation;
				}
				List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();
				Dictionary<AllSelectCRFunction, string> dicAllSelectCRFunctionPoolName = new Dictionary<AllSelectCRFunction, string>();//yy: new, to replace lstAllSelectCRFunction for table part (lstAllSelectCRFunction is kept as we do not want to change chart and map part yet.)
				if ((sender is ObjectListView) || sender is Button)
				{
					foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.SelectedObjects)
					{
						AllSelectCRFunction cr = keyValueCR.Key;

						if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
						{
							continue;
						}
						else
						{
							if (crSelectFunctionCalculateValue == null)
								crSelectFunctionCalculateValue = cr.CRSelectFunctionCalculateValue;
							lstAllSelectCRFunction.Add(cr);
							dicAllSelectCRFunctionPoolName.Add(cr, keyValueCR.Value); //YY: new
						}

					}
					tabCtlMain.SelectedIndex = 1;
					if (olvIncidence.SelectedObjects.Count > 1)
					{
						bGIS = false;
						bChart = false;
					}
				}
				else
				{
					foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.Objects)
					{
						AllSelectCRFunction cr = keyValueCR.Key;

						if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
						{
							continue;
						}
						else
						{
							if (crSelectFunctionCalculateValue == null)
								crSelectFunctionCalculateValue = cr.CRSelectFunctionCalculateValue;
							lstAllSelectCRFunction.Add(cr);
							dicAllSelectCRFunctionPoolName.Add(cr, keyValueCR.Value); //YY: new
						}

					}

					if (_MapAlreadyDisplayed && _IncidenceDragged)//MCB- a kluge: Need a better way to determine if sender was from map
					{
						bGIS = true;
						tabCtlMain.SelectedIndex = 0;
						bChart = false;
					}
					else
					{
						bGIS = false;
						bChart = false;
						tabCtlMain.SelectedIndex = 1;
					}
				}
				if (lstAllSelectCRFunction.Count > 0) crSelectFunctionCalculateValue = lstAllSelectCRFunction.First().CRSelectFunctionCalculateValue;
				else return;
				if (lstAllSelectCRFunction[0].CRSelectFunctionCalculateValue.CRCalculateValues.Count == 1 && !CommonClass.CRRunInPointMode)
				{
					lstCFGRpoolingforCDF = lstAllSelectCRFunction;
					canshowCDF = true;
					bChart = true;
				}
				else
				{
					lstCFGRpoolingforCDF = new List<AllSelectCRFunction>();
					canshowCDF = false;
				}
				iCDF = 1;
				string Tip = "Drawing pooled incidence results layer";
				WaitShow(Tip);
				if (crSelectFunctionCalculateValue != null)
				{
					if (cbIncidenceAggregation.SelectedIndex != -1 && cbIncidenceAggregation.SelectedIndex != 0)
					{
						DataRowView drv = cbIncidenceAggregation.SelectedItem as DataRowView;
						int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
						if (iAggregationGridType != incidenceGrid.GridDefinitionID)
						{
							crSelectFunctionCalculateValue = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crSelectFunctionCalculateValue, incidenceGrid.GridDefinitionID, iAggregationGridType);
							incidenceGrid = Grid.GridCommon.getBenMAPGridFromID(iAggregationGridType);
						}

					}
					if (i == 0)
					{
						ClearMapTableChart();
						if (this.rbShowActiveIncidence.Checked)
						{
							if (tabCtlMain.SelectedIndex == 0)
							{
								bTable = false;
								bChart = false;
							}
							else if (tabCtlMain.SelectedIndex == 1)
							{
								bGIS = false;
								bChart = false;
							}
							else if (tabCtlMain.SelectedIndex == 2)
							{
								bGIS = false;
								bTable = false;
							}
						}
						if (bTable)
						{
							//InitTableResult(lstAllSelectCRFunction);
							InitTableResult(dicAllSelectCRFunctionPoolName); //YY: replace lstAllSelectCRFunction here so that we can pass pooling name

						}
						if (bChart)
						{

							InitChartResult(crSelectFunctionCalculateValue, (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null) ? CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID);
						}
						if (bGIS)
						{
							//set change projection text
							string changeProjText = "change projection to setup projection";
							if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
							{
								changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
							}
							tsbChangeProjection.Text = changeProjText;

							mainMap.ProjectionModeReproject = ActionMode.Never;
							mainMap.ProjectionModeDefine = ActionMode.Never;
							string shapeFileName = "";

							MapGroup ResultsMG = AddMapGroup("Results", "Map Layers", false, false);
							MapGroup PIResultsMapGroup = AddMapGroup("Pooled Incidence", "Results", false, false);

							//string LayerNameText = "Pooled Incidence";
							string author = "Author Unknown";
							if (crSelectFunctionCalculateValue.CRSelectFunction != null && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction != null
																							&& crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author != null)
							{
								author = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author;
								if (author.IndexOf(" ") > -1)
								{
									author = author.Substring(0, author.IndexOf(" "));
								}
							}
							string LayerNameText = author;
							string poolingWindow = "";
							foreach (KeyValuePair<AllSelectCRFunction, string> keyValue in olvIncidence.SelectedObjects)
							{
								if (keyValue.Key.CRSelectFunctionCalculateValue == crSelectFunctionCalculateValue)
								{
									poolingWindow = keyValue.Value;
								}
							}

							author = poolingWindow + "--" + LayerNameText;
							RemoveOldPolygonLayer(LayerNameText, PIResultsMapGroup.Layers, false);

							//mainMap.Layers.Clear();
							if (incidenceGrid is ShapefileGrid)
							{
								if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as ShapefileGrid).ShapefileName + ".shp"))
								{
									shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as ShapefileGrid).ShapefileName + ".shp";
								}
							}
							else if (incidenceGrid is RegularGrid)
							{
								if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as RegularGrid).ShapefileName + ".shp"))
								{
									shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as RegularGrid).ShapefileName + ".shp";
								}
							}

							MapPolygonLayer tlvIPoolMapPolyLayer = (MapPolygonLayer)PIResultsMapGroup.Layers.Add(shapeFileName);

							DataTable dt = tlvIPoolMapPolyLayer.DataSet.DataTable.Copy();
							tlvIPoolMapPolyLayer.LegendText = LayerNameText;
							//tlvIPoolMapPolyLayer.Name = "Pooled Incidence";
							int j = 0;
							int iCol = 0;
							int iRow = 0;
							List<string> lstRemoveName = new List<string>();
							while (j < dt.Columns.Count)
							{
								if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
								if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

								j++;
							}
							j = 0;

							while (j < dt.Columns.Count)
							{
								if (dt.Columns[j].ColumnName.ToLower() == "col" || dt.Columns[j].ColumnName.ToLower() == "row")
								{ }
								else
									lstRemoveName.Add(dt.Columns[j].ColumnName);

								j++;
							}
							foreach (string s in lstRemoveName)
							{
								dt.Columns.Remove(s);
							}
							dt.Columns.Add("Point Estimate", typeof(double));
							j = 0;
							while (j < dt.Columns.Count)
							{
								if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
								if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

								j++;
							}
							j = 0;

							if (IncidencelstHealth == null)
							{
								dt.Columns.Add("Endpoint Group", typeof(string));
								dt.Columns.Add("Endpoint", typeof(string));
								dt.Columns.Add("Author", typeof(string));
								dt.Columns.Add("Start Age", typeof(int));
								dt.Columns.Add("End Age", typeof(int));
								dt.Columns.Add("Version", typeof(string));
							}
							else
							{
								foreach (FieldCheck fieldCheck in IncidencelstHealth)
								{
									if (fieldCheck.isChecked)
									{
										dt.Columns.Add(fieldCheck.FieldName);
									}
								}
							}
							if (IncidencelstResult == null)
							{
								dt.Columns.Add("Population", typeof(double));
								dt.Columns.Add("Delta", typeof(double));
								dt.Columns.Add("Mean", typeof(double));
								dt.Columns.Add("Baseline", typeof(double));
								dt.Columns.Add("Percent of Baseline", typeof(double));
								dt.Columns.Add("Standard Deviation", typeof(double));
								dt.Columns.Add("Variance", typeof(double));

								int numPercentiles = crSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count;
								double step = 100 / (double)numPercentiles;
								double current = step / 2;

								for (int k = 0; k < numPercentiles; k++)
								{
									string currLabel = String.Concat("Percentile ", current.ToString());
									dt.Columns.Add(currLabel, typeof(float));
									current += step;
								}
							}
							else
							{
								foreach (FieldCheck fieldCheck in IncidencelstResult)
								{
									if (fieldCheck.isChecked && (fieldCheck.FieldName != "Percentiles" && fieldCheck.FieldName != "Point Estimate"))
									{
										dt.Columns.Add(fieldCheck.FieldName, typeof(double));
									}
								}

								if ((IncidencelstResult.Find(p => p.FieldName.Equals("Percentiles")).isChecked) && crSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
								{
									int numPercentiles = crSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count;
									double step = 100 / (double)numPercentiles;
									double current = step / 2;

									for (int k = 0; k < numPercentiles; k++)
									{
										string currLabel = String.Concat("Percentile ", current.ToString());
										dt.Columns.Add(currLabel, typeof(float));
										current += step;
									}
								}

							}

							foreach (CRCalculateValue crcv in crSelectFunctionCalculateValue.CRCalculateValues)
							{
								DataRow[] foundRow = dt.Select(String.Format("Row = '{0}' AND Col = '{1}'", crcv.Row, crcv.Col));

								if (foundRow.Length == 1)
								{
									if (IncidencelstHealth != null)
									{
										foreach (FieldCheck fieldCheck in IncidencelstHealth)
										{
											if (fieldCheck.isChecked)
											{
												foundRow[0][fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, crSelectFunctionCalculateValue.CRSelectFunction);
											}
										}
									}
									else
									{
										foundRow[0]["Endpoint Group"] = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup;
										foundRow[0]["Endpoint"] = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint;
										foundRow[0]["Author"] = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author;
										foundRow[0]["Start Age"] = crSelectFunctionCalculateValue.CRSelectFunction.StartAge;
										foundRow[0]["End Age"] = crSelectFunctionCalculateValue.CRSelectFunction.EndAge;
									}

									if (IncidencelstResult != null)
									{
										foreach (FieldCheck fieldCheck in IncidencelstResult)
										{
											if (fieldCheck.isChecked && fieldCheck.FieldName != "Percentiles")
											{
												foundRow[0][fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, crSelectFunctionCalculateValue.CRSelectFunction);
											}
											int k = 0;
											if (fieldCheck.isChecked && fieldCheck.FieldName == "Percentiles")
											{
												while (k < crcv.LstPercentile.Count())
												{

													foundRow[0]["Percentile " + ((Convert.ToDouble(k + 1) * 100.00 / Convert.ToDouble(crcv.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crcv.LstPercentile.Count())))))] = crcv.LstPercentile[k];
													k++;
												}
											}
										}
									}
									else
									{
										foundRow[0]["Point Estimate"] = crcv.PointEstimate;
										foundRow[0]["Population"] = crcv.Population;
										foundRow[0]["Delta"] = crcv.Delta;
										foundRow[0]["Mean"] = crcv.Mean;
										foundRow[0]["Baseline"] = crcv.Baseline;
										foundRow[0]["Percent of Baseline"] = crcv.PercentOfBaseline;
										foundRow[0]["Standard Deviation"] = crcv.StandardDeviation;
										foundRow[0]["Variance"] = crcv.Variance;

										int numPercentiles = crcv.LstPercentile.Count;
										double step = 100 / (double)numPercentiles;
										double current = step / 2;

										for (int k = 0; k < numPercentiles; k++)
										{
											string currLabel = String.Concat("Percentile ", current.ToString());
											foundRow[0][currLabel] = crcv.LstPercentile[k];
											current += step;
										}
									}
								}
							}

							var rowsToDelete = new List<DataRow>();
							foreach (DataRow dr in dt.Rows)
							{
								if (String.IsNullOrEmpty(dr[2].ToString()))
								{
									rowsToDelete.Add(dr);
								}
							}

							rowsToDelete.ForEach(x => dt.Rows.Remove(x));

							tlvIPoolMapPolyLayer.DataSet.DataTable = dt;

							if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
							tlvIPoolMapPolyLayer.DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);
							// mainMap.Layers.Clear();  -MCB, will need to add code to clear the equivalent layer if it exists already

							//tlvIPoolMapPolyLayer = (MapPolygonLayer)ResultsMG.Layers.Add(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
							//tlvIPoolMapPolyLayer.DataSet.DataTable.Columns["Point Estimate"].ColumnName = "Pooled Incidence";

							//if (crSelectFunctionCalculateValue.CRSelectFunction != null && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction != null
							//    && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author != null)
							//{
							//    author = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author;
							//    if (author.IndexOf(" ") > -1)
							//    {
							//        author = author.Substring(0, author.IndexOf(" "));
							//    }
							//}

							MapPolygonLayer polLayer = tlvIPoolMapPolyLayer;
							string strValueField = polLayer.DataSet.DataTable.Columns["Point Estimate"].ColumnName;
							_columnName = strValueField;
							polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "I"); //-MCB added

							double dMinValue = 0.0;
							double dMaxValue = 0.0;
							dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
							dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);

							_dMinValue = dMinValue;
							_dMaxValue = dMaxValue;
							//_currentLayerIndex = mainMap.Layers.Count - 1;
							polLayer.LegendText = author;
							polLayer.Name = "Pooled Incidence:" + author; // "PIR_" + author;
							_CurrentIMapLayer = polLayer;
							string pollutantUnit = string.Empty;
							_columnName = strValueField;
							_CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: Pooled Incidence-" + tlvIPoolMapPolyLayer.LegendText;
							addAdminLayers();
						}
					}
					i++;
					CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
				}
				WaitClose();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);

				WaitClose();
			}
		}

		private void btPoolingShowResult_Click(object sender, EventArgs e)
		{
			tlvIncidence_DoubleClick(sender, e);
		}

		int icbCRAggregationSelectIndexOld = -1;

		private void cbCRAggregation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbCRAggregation.SelectedIndex != icbCRAggregationSelectIndexOld)
			{
				icbCRAggregationSelectIndexOld = cbCRAggregation.SelectedIndex;
				btShowCRResult_Click(sender, e);
			}
			tabCtlMain.Focus();
		}

		private void rbIncidenceOnlyOne_CheckedChanged(object sender, EventArgs e)
		{
			if (rbIncidenceOnlyOne.Checked)
			{
				lblIncidence.Enabled = true;
				cbPoolingWindowIncidence.Enabled = true;

			}
			else
			{
				lblIncidence.Enabled = false;
				cbPoolingWindowIncidence.Enabled = false;

			}
			cbPoolingWindowIncidence_SelectedIndexChanged(sender, e);
		}

		private void rbAPVOnlyOne_CheckedChanged(object sender, EventArgs e)
		{
			if (rbAPVOnlyOne.Checked)
			{
				lblAPV.Enabled = true;
				cbPoolingWindowAPV.Enabled = true;
			}
			else
			{
				lblAPV.Enabled = false;
				cbPoolingWindowAPV.Enabled = false;

			}
			cbPoolingWindowAPV_SelectedIndexChanged(sender, e);

		}

		private void numericUpDownResult_ValueChanged(object sender, EventArgs e)
		{
			if (_tableObject == null) return;
			foreach (OLVColumn olvc in OLVResultsShow.Columns)
			{
				if (olvc.AspectToStringFormat != "" && olvc.AspectToStringFormat != null)
				{
					olvc.AspectToStringFormat = "{0:N" + numericUpDownResult.Value + "}";
				}
			}
			UpdateTableResult(_tableObject);
		}

		private void btShowDetailIncidence_Click(object sender, EventArgs e)
		{
			if (btShowDetailIncidence.Text == "Show aggregated")
			{
				btShowDetailIncidence.Text = "Show pooled";
			}
			else
				btShowDetailIncidence.Text = "Show aggregated";
			cbPoolingWindowIncidence_SelectedIndexChanged(sender, e);
		}

		private void btShowDetailValuation_Click(object sender, EventArgs e)
		{
			if (btShowDetailValuation.Text == "Show aggregated")
			{
				btShowDetailValuation.Text = "Show pooled";
			}
			else
				btShowDetailValuation.Text = "Show aggregated";

			cbPoolingWindowAPV_SelectedIndexChanged(sender, e);


		}

		private void olvIncidence_Validated(object sender, EventArgs e)
		{
			foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
			{
				item.UseItemStyleForSubItems = true; item.BackColor = SystemColors.Highlight;
				item.ForeColor = Color.White;
			}
		}

		private void tlvAPVResult_Validated(object sender, EventArgs e)
		{
			foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
			{
				item.UseItemStyleForSubItems = true; item.BackColor = SystemColors.Highlight;
				item.ForeColor = Color.White;
			}
		}

		private void olvCRFunctionResult_MouseLeave(object sender, EventArgs e)
		{
			foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
			{
				item.UseItemStyleForSubItems = true; item.BackColor = SystemColors.Highlight;
				item.ForeColor = Color.White;
			}
		}

		private void cbGraph_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				//  zedGraphCtl2.GraphPane = new GraphPane(new Rectangle(0, 0, zedGraphCtl2.Width, zedGraphCtl2.Height), "", "", "");
				switch (cbGraph.Text)
				{
					case "Bar Graph":
						olvRegions.Enabled = true;
						groupBox1.Enabled = true;
						groupBox9.Enabled = true;
						btnApply.Enabled = true;
						btnSelectAll.Enabled = true;
						btnApply_Click(sender, e);
						break;
					case "Cumulative Distribution Functions":
						olvRegions.Enabled = false;
						groupBox1.Enabled = false;
						groupBox9.Enabled = false;
						btnSelectAll.Enabled = false;
						btnApply.Enabled = false;
						ShowCDFgraph();
						break;
				}
			}
			catch
			{ }
		}

		private void ShowCDFgraph()
		{/* 
						try
						{
								GraphPane myPane = zedGraphCtl2.GraphPane;
								switch (iCDF)
								{
										case 0:
												List<double> lstp = new List<double>();
												int p = lstCFGRforCDF[0].CRCalculateValues[0].LstPercentile.Count;
												double percentile = (double)50 / p * 0.01;
												for (int i = 0; i < p; i++)
												{
														lstp.Add(percentile);
														percentile = percentile + 100 / p * 0.01;
												}
												foreach (CRSelectFunctionCalculateValue cr in lstCFGRforCDF)
												{
														PointPairList list = new PointPairList();
														for (int i = 0; i < p; i++)
														{
																list.Add(new PointPair(cr.CRCalculateValues[0].LstPercentile[i], lstp[i]));
														}
														myPane.AddCurve(cr.CRSelectFunction.BenMAPHealthImpactFunction.Author, list, GetRandomColor(), ZedGraph.SymbolType.None).Line.Width = 1.2F;
												}

												break;
										case 1:
												lstp = new List<double>();
												p = lstCFGRpoolingforCDF[0].CRSelectFunctionCalculateValue.CRCalculateValues[0].LstPercentile.Count;
												percentile = (double)50 / p * 0.01;
												for (int i = 0; i < p; i++)
												{
														lstp.Add(percentile);
														percentile = percentile + 100 / p * 0.01;
												}
												foreach (AllSelectCRFunction cr in lstCFGRpoolingforCDF)
												{
														PointPairList list = new PointPairList();
														for (int i = 0; i < p; i++)
														{
																list.Add(new PointPair(cr.CRSelectFunctionCalculateValue.CRCalculateValues[0].LstPercentile[i], lstp[i]));
														}
														myPane.AddCurve(cr.Author, list, GetRandomColor(), ZedGraph.SymbolType.None).Line.Width = 1.2F;
												}
												break;
										case 2:
												lstp = new List<double>();
												p = lstAPVRforCDF[0].lstAPVValueAttributes[0].LstPercentile.Count;
												percentile = (double)50 / p * 0.01;
												for (int i = 0; i < p; i++)
												{
														lstp.Add(percentile);
														percentile = percentile + 100 / p * 0.01;
												}
												foreach (AllSelectValuationMethodAndValue value in lstAPVRforCDF)
												{
														PointPairList list = new PointPairList();
														for (int i = 0; i < p; i++)
														{
																list.Add(new PointPair(value.lstAPVValueAttributes[0].LstPercentile[i], lstp[i]));
														}
														myPane.AddCurve(value.AllSelectValuationMethod.Author, list, GetRandomColor(), ZedGraph.SymbolType.None).Line.Width = 1.2F;
												}
												myPane.XAxis.Scale.Format = "$#0.####";
												break;
								}
								myPane.IsFontsScaled = false; myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0F);
								myPane.YAxis.Scale.Format = "#0.####%";
								myPane.YAxis.Scale.Min = 0;
								myPane.YAxis.Scale.Max = 1;
								myPane.YAxis.MajorGrid.IsVisible = true;
								myPane.Title.Text = strCDFTitle;
								myPane.XAxis.Title.Text = strCDFX;
								myPane.YAxis.Title.Text = strCDFY;
								zedGraphCtl2.AxisChange();
								zedGraphCtl2.Refresh();
						}
						catch
						{ } */
		}

		public System.Drawing.Color GetRandomColor()
		{
			Random rand = new Random(Guid.NewGuid().GetHashCode());
			return Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
		}

		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			try
			{
				bool selectOrDeselect;

				if (olvRegions.CheckedItems.Count != olvRegions.Items.Count)
					selectOrDeselect = true;
				else
					selectOrDeselect = false;

				for (int j = 0; j < olvRegions.Items.Count; j++)
				{
					OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
					olvi.Checked = selectOrDeselect;
				}

				if (selectOrDeselect == true) btnSelectAll.Text = "Deselect All";
				else { btnSelectAll.Text = "Select All"; }

				btnApply_Click(null, null);
			}
			catch
			{ }
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
			//red_blue chosen as default by client for delta layers
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
				case "blue_red":
					// _colorArray = (System.Drawing.Color[])_red_blue_Array.Reverse();
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

		private void olvCRFunctionResult_DragLeave(object sender, EventArgs e)
		{
			Debug.WriteLine("olvCRFunctionResul_DragLeave");
			_HealthResultsDragged = true;
			return;
		}

		private void olvCRFunctionResult_DragDrop(object sender, DragEventArgs e)
		{
			Debug.WriteLine("olvCRFunctionResult_DragDrop");
			//
			//{
			//  btShowCRResult_Click(sender, e);
			_HealthResultsDragged = false;
			//}
			return;
		}

		private void tlvIncidence_DragLeave(object sender, EventArgs e)
		{
			Debug.WriteLine("tlvIncidence_DragLeave");
			_IncidenceDragged = true;
			return;
		}
		//MERGE Check
		private void textBoxFilterSimple_TextChanged(object sender, EventArgs e)
		{
			//This function maintains the previous filter functionality through radio button selection.
			//When a user selects the "Search" option, this function clears the list of previously found search results.
			if (rbFilter.Checked)
				this.TimedFilter(this.treeListView, textBoxFilterSimple.Text);
			if (rbSearch.Checked)
			{
				strList.Clear();
				btnNext.Visible = false;
				lblAuditSearch.Visible = false;
			}
		}

		private void tlvAPVResult_DragLeave(object sender, EventArgs e)
		{
			Debug.WriteLine("APVdragged_DragLeave");
			_APVdragged = true;
			return;
		}

		private void btnShowHideAttributeTable_Click(object sender, EventArgs e)
		{
			//User has to select a feature layer to see it's attribute table.  If none selected then show the attribute table of the top visible layer.  If none then don't show an attribute table.
			//If the selected layer is not a feature layer (point, line, polygon_ then don't show an attribute table.

			if (mainMap.GetAllLayers().Count > 0) //Only perform if any featurelayers present
			{
				ILayer SelLayer = null;
				string LayerType;
				MapPolygonLayer SelPolyMapLayer;
				MapPointLayer SelPointLayer;
				MapLineLayer SelLineMapLayer;

				//Get the selected layer
				SelLayer = mainMap.Layers.SelectedLayer;
				if (SelLayer == null)                    //Use the top visible layer
				{
					SelLayer = FindTopVisibleLayer(false);
					Debug.WriteLine("No layer selected, top visible layer used instead to dsplay the attribute table of");
				}

				if (SelLayer == null)                   //No layers are visible on the map
				{
					Debug.WriteLine("User tried to show an attribute table When none of the layers are visible or selected.");
					return;
				}

				//Get it's type
				LayerType = SelLayer.ToString();
				switch (LayerType.ToLower())
				{
					case "dotspatial.controls.mappolygonlayer":
						SelPolyMapLayer = (MapPolygonLayer)SelLayer;
						SelPolyMapLayer.ShowAttributes();
						break;
					case "dotspatial.controls.mapmultipointlayer":
					case "dotspatial.controls.mappointlayer":
						SelPointLayer = (MapPointLayer)SelLayer;
						SelPointLayer.ShowAttributes();
						break;
					case "dotspatial.controls.mappolylinelayer":
					case "dotspatial.controls.maplinelayer":
						SelLineMapLayer = (MapLineLayer)SelLayer;
						SelLineMapLayer.ShowAttributes();
						break;
					default:
						Debug.WriteLine("user tried to show the Attribute table for a non-feature layer.");
						break;
				}
			}
			else
			{
				Debug.WriteLine("No Layers to display the attribute table of");
			}
			return;
		}

		private bool _SelectByLocationDialogShown;

		private void SbOnClosed(object sender, EventArgs eventArgs)
		{
			_SelectByLocationDialogShown = false;
			((Form)sender).Closed -= SbOnClosed;
		}

		private void MapLayers_ItemChanged(object sender, EventArgs e)
		{
			//placeholder to capture event when map layers have changed so that we can update the colors to the database of the admin layers.
			if (_RaiseLayerChangeEvents == true)
			{
				//MessageBox.Show("something changed");
				SyncLayersWithDB();
				mainMap.Refresh();
			}
		}
		//<<<<<<< HEAD
		//=======
		//>>>>>>> develop

		private void SyncLayersWithDB()
		{
			//If the user changes the layer colors in the map, sync these changes with the database tables
			//eventually we could sync all aspects of the symbology. for now start with polygon outline color.

			//get list of admin layers from database table GridDefinitions

			List<ILayer> lyrs = mainMap.GetAllLayers();
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			string commandText = "";
			string fName = "";
			string gridID = "";
			string newColor = "";
			string oldColor = "";
			object obj = null;
			string setupID = Convert.ToString(CommonClass.MainSetup.SetupID);
			//MessageBox.Show("Checking for Changes.");
			foreach (ILayer lyr in lyrs)
			{
				fName = Path.GetFileNameWithoutExtension(lyr.DataSet.Filename);
				IMapFeatureLayer f = (IMapFeatureLayer)lyr;
				if (f.DataSet.FeatureType == FeatureType.Polygon)
				{
					PolygonLayer plyr = (PolygonLayer)lyr;
					newColor = System.Drawing.ColorTranslator.ToHtml(plyr.Symbolizer.OutlineSymbolizer.GetFillColor());
				}
				else if (f.DataSet.FeatureType == FeatureType.Line)
				{
					LineLayer llyr = (LineLayer)lyr;
					newColor = System.Drawing.ColorTranslator.ToHtml(llyr.Symbolizer.GetFillColor());
				}

				else if (f.DataSet.FeatureType == FeatureType.Point)
				{
					PointLayer tlyr = (PointLayer)lyr;
					newColor = System.Drawing.ColorTranslator.ToHtml(tlyr.Symbolizer.GetFillColor());
				}
				if (fName != "")
				{
					commandText = string.Format(@"select SHAPEFILEGRIDDEFINITIONDETAILS.GRIDDEFINITIONID 
from SHAPEFILEGRIDDEFINITIONDETAILS 
join GRIDDEFINITIONS on SHAPEFILEGRIDDEFINITIONDETAILS.GRIDDEFINITIONID = GRIDDEFINITIONS.GRIDDEFINITIONID where SHAPEFILENAME = '{0}' and GRIDDEFINITIONS.SETUPID = {1}
UNION
select REGULARGRIDDEFINITIONDETAILS.GRIDDEFINITIONID 
from REGULARGRIDDEFINITIONDETAILS 
join GRIDDEFINITIONS on REGULARGRIDDEFINITIONDETAILS.GRIDDEFINITIONID = GRIDDEFINITIONS.GRIDDEFINITIONID where SHAPEFILENAME = '{0}' and GRIDDEFINITIONS.SETUPID = {1}", fName, setupID);
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					gridID = Convert.ToString(obj);
					if (gridID != "")
					{
						gridID = Convert.ToString(obj);
						commandText = string.Format("select OUTLINECOLOR from GRIDDEFINITIONS where GRIDDEFINITIONID={0}", gridID);
						obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
						oldColor = Convert.ToString(obj).Trim();
						if (newColor != oldColor)
						{
							// MessageBox.Show(string.Format("Updating changes. The GridID is {0}, the filename is {1} the new color is {2} the old color is {3}", gridID, fName, newLineColor, oldLineColor));

							commandText = string.Format("update GRIDDEFINITIONS set OUTLINECOLOR='{0}' where GRIDDEFINITIONID={1}", newColor, gridID);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
						//<<<<<<< HEAD
						//=======
						//>>>>>>> develop
					}
				}

			}
		}

		public void MoveAdminGroupToTop()
		{
			_RaiseLayerChangeEvents = false;
			foreach (IMapLayer lyr in mainMap.Layers)
			{
				if (lyr.LegendText == "Region Admin Layers")
				{
					mainMap.Layers.Remove(lyr);
					mainMap.Layers.Insert(mainMap.Layers.Count, lyr);
					lyr.IsExpanded = false;
					mainMap.Refresh();
					legend1.Refresh();
					break;
				}
			}
			_RaiseLayerChangeEvents = true;
		}

		//<<<<<<< HEAD
		//=======
		//>>>>>>> develop

		#region PrintLayout
		//Functions in this region are related to building and showing the print layout dialog form.

		public void SetUpPrintLayout()
		{
			//Turn off all hidden/obfuscated layers so that their legends don't appear in the map
			MapPolygonLayer TopLayer = TurnOffHiddenLayers();

			//Create a print layout form
			LayoutForm frmLayout = new LayoutForm();
			LayoutControl lc = frmLayout.LayoutControl;
			lc.PrinterSettings.DefaultPageSettings.Landscape = true;

			//Clear the layout
			lc.NewLayout(false);

			//Add outer rectangle "neatline"
			LayoutRectangle lr1 = new LayoutRectangle();
			lr1.Name = "Outer Neatline";
			lr1.Background = new PolygonSymbolizer(Color.LightGray, Color.Black);
			lr1.Location = new Point(50, 50);
			lr1.Size = new Size(1000, 750);
			lc.AddToLayout(lr1);

			//Add the map control to the layout
			LayoutMap lm = new LayoutMap(mainMap);
			lm.Name = "Map";
			lm.Location = new Point(75, 175);
			lm.Size = new Size(675, 575);
			lm.Background = new PolygonSymbolizer(Color.White, Color.DarkGray, 2);
			lc.AddToLayout(lm);

			//Add the legend control to the layout
			LayoutLegend ll = new LayoutLegend();
			ll.Map = lm;
			ll.Name = "Legend";
			ll.NumColumns = 1;
			ll.Location = new Point(775, 175);
			ll.Size = new Size(250, 575);
			ll.Background = new PolygonSymbolizer(Color.White, Color.DarkGray, 2);
			lc.AddToLayout(ll);

			//Add a north arrow to the layout
			LayoutNorthArrow ln = new LayoutNorthArrow();
			ln.Name = "North Arrow";
			ln.Location = new Point(670, 670);
			ln.Size = new Size(75, 75);
			lc.AddToLayout(ln);

			//Add a scalebar to the layout - only show when using projected coordinates
			if (mainMap.Projection == DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984)
			{
				LayoutText lt4 = new LayoutText();
				lt4.Name = "Projection Notice";
				lt4.Text = "Map is in WGS1984 Geographic Coordinates." + Environment.NewLine + "No scale bar available.";
				lt4.Location = new Point(80, 690);
				lt4.Size = new Size(345, 50);
				lt4.Font = new Font("Arial", 8, FontStyle.Italic);
				lt4.ContentAlignment = ContentAlignment.MiddleLeft;
				lc.AddToLayout(lt4);
			}
			else
			{
				LayoutScaleBar ls = new LayoutScaleBar();
				ls.Map = lm;
				ls.Name = "Scale Bar";
				ls.Location = new Point(80, 690);
				ls.Size = new Size(345, 50);
				ls.Font = new Font("Arial", 8);
				lc.AddToLayout(ls);
			}

			//Add a title
			LayoutText lt1 = new LayoutText();
			lt1.Name = "Title";
			lt1.Text = "BenMAP: " + CommonClass.ManageSetup.SetupName;
			lt1.Location = new Point(50, 75);
			lt1.Size = new Size(1000, 50);
			lt1.Font = new Font("Arial", 24);
			lt1.ContentAlignment = ContentAlignment.MiddleCenter;
			lc.AddToLayout(lt1);

			//Add a subtititle
			LayoutText lt2 = new LayoutText();
			lt2.Name = "Subtitle";
			if (TopLayer is null)
			{
				lt2.Text = string.Empty;
			}
			else
			{
				string strBreadCrumb = CreateMapSubtitle(TopLayer, "");
				string[] lstBreadCrumb = strBreadCrumb.TrimEnd('|').Split('|');
				Array.Reverse(lstBreadCrumb);
				lt2.Text = string.Join(" : ", lstBreadCrumb);
			}
			lt2.Location = new Point(50, 125);
			lt2.Size = new Size(1000, 50);
			lt2.Font = new Font("Arial", 14);
			lt2.ContentAlignment = ContentAlignment.MiddleCenter;
			lc.AddToLayout(lt2);

			//Add a disclaimer
			LayoutText lt3 = new LayoutText();
			lt3.Name = "Disclaimer";
			lt3.Text = "This map was created on " + System.DateTime.Today.ToLongDateString() + " at " + System.DateTime.Now.ToLongTimeString() + " using the US EPA BenMAP-CE 1.4.4 software system including DotSpatial mapping components." + Environment.NewLine + "For more information see https://www.epa.gov/benmap";
			lt3.Location = new Point(50, 750);
			lt3.Size = new Size(975, 50);
			lt3.Font = new Font("Arial", 9);
			lt3.ContentAlignment = ContentAlignment.MiddleCenter;
			lc.AddToLayout(lt3);

			//Reset the page margins to 1/2 inch.
			lc.PrinterSettings.DefaultPageSettings.Margins.Left = 50;
			lc.PrinterSettings.DefaultPageSettings.Margins.Right = 50;
			lc.PrinterSettings.DefaultPageSettings.Margins.Top = 50;
			lc.PrinterSettings.DefaultPageSettings.Margins.Bottom = 50;

			//Set drawing quality
			lc.DrawingQuality = SmoothingMode.HighQuality;
			lc.ZoomFitToScreen();
			lc.ZoomFullViewExtentMap();

			//Show the layout form
			frmLayout.ShowDialog(this);
			frmLayout.Dispose();
		}

		public void RemoveAdminGroup()
		{
			_RaiseLayerChangeEvents = false;
			foreach (IMapLayer lyr in mainMap.Layers)
			{
				if (lyr.LegendText == "Region Admin Layers")
				{
					mainMap.Layers.Remove(lyr);
					break;
				}
			}
			_RaiseLayerChangeEvents = true;
		}

		private string CreateMapSubtitle(ILegendItem CurrentItem, string BaseText)
		{
			BaseText += CurrentItem.LegendText + "|";
			ILegendItem ParentItem = CurrentItem.GetParentItem();
			if (ParentItem is null)
			{
				return BaseText;
			}
			else
			{
				return CreateMapSubtitle(CurrentItem.GetParentItem(), BaseText);
			}

		}

		private MapPolygonLayer TurnOffHiddenLayers()
		{
			//Turn off visibility of all hidden layers.
			MapPolygonLayer TopLayer = null;
			List<MapPolygonLayer> pLyrs = new List<MapPolygonLayer>(); //list of visible transparent polygon layers

			foreach (Layer lyr in mainMap.GetAllLayers())
			{
				if (lyr is MapPolygonLayer)
				{
					MapPolygonLayer pLyr = (MapPolygonLayer)lyr;
					if (pLyr.Symbolizer.GetFillColor() != Color.Transparent && pLyr.IsVisible)
					{
						pLyrs.Add(pLyr);
					}
				}
			}
			pLyrs.Reverse();
			foreach (MapPolygonLayer pLyr in pLyrs)
			{
				if (!(TopLayer is null))
				{
					pLyr.IsVisible = false;
				}
				else
				{
					TopLayer = pLyr;
				}
			}
			return TopLayer;
		}
		#endregion

		private void btnAuditTrailExpand_Click(object sender, EventArgs e)
		{
			// This click event clears the textbox and recursively expands all nodes in the treeListView.
			this.textBoxFilterSimple.Clear();

			foreach (TreeNode tn in this.treeListView.Objects)
			{
				this.treeListView.Expand(tn);
				TreeNodeExpand(tn.Nodes);
			}
		}

		private void TreeNodeExpand(TreeNodeCollection tnc)
		{
			foreach (TreeNode tnChild in tnc)
			{
				this.treeListView.Expand(tnChild);
				TreeNodeExpand(tnChild.Nodes);
			}
		}

		private void btnAuditTrailCollapse_Click(object sender, EventArgs e)
		{
			// This click event clears the textbox (to avoid error from filtering on collapsed treelist) and then recursively collapses all nodes in the treeListView

			this.textBoxFilterSimple.Clear();
			TreeListCollapse();
		}

		private void TreeListCollapse()
		{
			//This function collapses the entire tree based from the top level nodes
			//Note: The the top/parent level of a "TreeListView" has "Objects" or "Roots" while subsequent child nodes only have "Nodes"

			foreach (TreeNode tn in this.treeListView.Objects)
			{
				TreeNodeCollapse(tn.Nodes);
				this.treeListView.Collapse(tn);
			}
		}

		private void TreeNodeCollapse(TreeNodeCollection tnc)
		{
			foreach (TreeNode tnChild in tnc)
			{
				this.treeListView.Collapse(tnChild);
				TreeNodeCollapse(tnChild.Nodes);
			}
		}
		private void TreeNodeSearch(TreeNode tn, string SearchText)
		{
			//This function iterates through each node of the tree, checks the text against the search term, and (if it matches) calls the function to save the node path index.. 
			try
			{
				if (tn.Level == 0)
				{
					bool resultFound = tn.Text.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
					if (resultFound)
						strList.Add(TreeNodePath(tn, ""));
				}
				foreach (TreeNode tnChild in tn.Nodes)
				{
					bool resultFound = tnChild.Text.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;

					if (resultFound)
						strList.Add(TreeNodePath(tnChild, ""));
					TreeNodeSearch(tnChild, SearchText);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private string TreeNodePath(TreeNode tn, string nodePath)
		{
			//This function takes the node with a matched search and creates a string of numbers indicating the index to select at each level to navigate to the node.

			//If the node is not at the top/parent level, then take the index of the node and add to it until reaching the top level.
			//If the node is at the parent level, then iterate through each top object to find the index of the corresponding node.
			if (tn.Level != 0)
			{
				string buildPath;
				if (nodePath == "")
					buildPath = tn.Index.ToString();
				else
					buildPath = tn.Index.ToString() + " " + nodePath;
				return TreeNodePath(tn.Parent, buildPath);
			}
			else
			{
				string baseIndex = "";
				int i = 0;
				foreach (TreeNode baseNode in this.treeListView.Objects)
				{
					if (String.Compare(baseNode.Text, tn.Text) == 0)
					{
						baseIndex = i.ToString();
					}
					i++;
				}
				return baseIndex + " " + nodePath;
			}
		}
		private void TreeNodeSearch_Expand(TreeNodeCollection tnc, string[] str, int nodeLevel)
		{
			//If search results are found, this function takes the first (or next) entry in the string array and navigates to node's location, expanding and selecting along the way.
			int i = 0;

			foreach (TreeNode tn in tnc)
			{
				if (i.ToString() == str[nodeLevel])
				{
					this.treeListView.Expand(tn);
					this.treeListView.SelectObject(tn);
					tn.EnsureVisible();

					if (nodeLevel + 1 == str.Length)    //The node is found once the "nodeLevel" is equal to the number of elements in the string array.
					{
						this.treeListView.Expand(tn);
						this.treeListView.SelectObject(tn);
						ObjectListView olv = treeListView as ObjectListView;      //BenMAP-447: After the node is found, convert the nodes to an ObjectListView in order to set the "TopItem" of the OLV later on.
						var prevIdx = olv.SelectedIndex;                    //Find the current location in the list, because certain search terms ("Health Impact Function", "Beta",etc.) can appear multiple times.

						ListViewItem findNode;
						if (prevIdx == -1)                            //If no node was previously selected (i.e. if it's the first time searching the phrase), start search from beginning.
							findNode = olv.FindItemWithText(tn.Text);
						else

							findNode = olv.FindItemWithText(tn.Text, true, prevIdx);  //Else, find the node starting from the spot of the previously selected node--otherwise the method returns the first instance

						this.treeListView.TopItem = findNode;               //Set the current node to the "TopItem" to ensure that it is in view when the form is not maximized.

						return;
					}
					else                                //Otherwise, keep searching.
						TreeNodeSearch_Expand(tn.Nodes, str, nodeLevel + 1);
				}
				i++;
			}
		}
		private void rbSearch_CheckedChanged(object sender, EventArgs e)
		{
			//If a user has previously searched and is changing to filter, this event handler makes invisible the controls related to search.
			RadioButton rb = sender as RadioButton;
			if (rb != null)
			{

				this.TimedFilter(this.treeListView, "");
				this.textBoxFilterSimple.Clear();
				if (!rb.Checked)
				{
					btnNext.Visible = false;
					lblAuditSearch.Visible = false;
				}
			}
		}

		public int nodeSearchEntry = 0;
		public IList<string> strList = new List<string>();

		private void textBoxFilterSimple_KeyDown(object sender, KeyEventArgs e)
		{
			//The previous filter functionality is utilized in the "Text Changed" event of textBoxFilterSimple. 
			//This function searches the tree after the user hits "Enter" and forces the user to click the "Next" button, rather than continuing to hit enter, to cycle through search results.

			if (e.KeyCode == Keys.Enter && strList.Count == 0)
			{
				if (rbSearch.Checked)
				{   //Clear previous searches and collapse tree to hide paths of previous search
					strList.Clear();
					TreeListCollapse();
					//Search the tree for the entered text
					foreach (TreeNode tn in this.treeListView.Objects)
					{
						TreeNodeSearch(tn, textBoxFilterSimple.Text);
					}
					//Display the results to user
					lblAuditSearch.Visible = true;
					if (strList.Count != 1)
						lblAuditSearch.Text = strList.Count.ToString() + " results found";
					else
						lblAuditSearch.Text = strList.Count.ToString() + " result found";
					//If results are found, expand out to the target node and select it.
					if (strList.Count > 0)
					{
						int i = 0;
						int nodeLevel = 1;
						string[] str = strList[0].Split(new char[] { ' ' }); //Find which top level node contains the search result, and pass all children node.
						foreach (TreeNode tn in this.treeListView.Objects)
						{
							if (i == Convert.ToInt32(str[0]))
							{
								this.treeListView.Expand(tn);
								this.treeListView.SelectObject(tn);
								tn.EnsureVisible();
								TreeNodeSearch_Expand(tn.Nodes, str, nodeLevel);
							}
							i++;
						}
						nodeSearchEntry++; //Increase the counter showing position within the list of search results.
					}
					if (strList.Count > 1)
					{
						btnNext.Visible = true;
						btnNext.Focus();     //BenMAP-447: Focus on next button to allow the user to navigate the results using the enter button.
					}
				}
			}
		}
		private void btnNext_Click(object sender, EventArgs e)
		{
			if (nodeSearchEntry > strList.Count - 1) //If the user clicks the Next button, through the end of the results. Reset the counter to 0, and collapse the tree.
			{                                        //Without collapsing, the highlighting/cycling through result nodes continues but moving the screen into view of the selecte nodes stops. 
				nodeSearchEntry = 0;                 //Tried reseting the selection to the top of the treeList but didn't seem to work.
				TreeListCollapse();
			}

			int i = 0;
			int nodeLevel = 1;
			string[] str = strList[nodeSearchEntry].Split(new char[] { ' ' });
			foreach (TreeNode tn in this.treeListView.Objects)
			{
				if (i == Convert.ToInt32(str[0]))
				{
					this.treeListView.Expand(tn);
					this.treeListView.SelectObject(tn);
					tn.EnsureVisible();
					TreeNodeSearch_Expand(tn.Nodes, str, nodeLevel);
				}
				i++;
			}
			nodeSearchEntry++;
		}

		private void rbFilter_CheckedChanged(object sender, EventArgs e)
		{

		}
		private double RoundToSignificantDigits(double d, int digits)
		{
			if (d == 0)
				return 0;

			int rounding = 2 - (1 + Convert.ToInt32(Math.Log10(Math.Abs(d))));

			if (rounding >= 0)
				return Math.Round(d, rounding);
			else
			{
				bool isNeg = false;

				if (d == 0)
					return 0;

				if (d < 0)
				{
					d = Math.Abs(d);
					isNeg = true;
				}

				double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);

				if (isNeg)
					return -1 * scale * Math.Round(d / scale, digits);
				else
					return scale * Math.Round(d / scale, digits);
			}
		}

		private void chbAllPercentiles_CheckedChanged(object sender, EventArgs e)
		{
			InitTableResult(_tableObject);
		}
	}
}