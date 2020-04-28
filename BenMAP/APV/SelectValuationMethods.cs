using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using System.Windows.Forms;
using BenMAP.APVX;

namespace BenMAP
{
	public partial class SelectValuationMethods : FormBase
	{
		public SelectValuationMethods()
		{
			InitializeComponent();
			this.tabControlSelection.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.tabControlSelection.DrawItem += new DrawItemEventHandler(tabControlSelection_DrawItem);
		}
		List<BenMAPQALY> lstBenMAPQALY = null;
		private bool bIncidenceCanSelected = true;
		private List<AllSelectValuationMethod> lstAllSelectValuationMethod;
		private bool bLoad = false;
		private void SelectValuationMethods_Load(object sender, EventArgs e)
		{

			//labDisplayIcon.Text = "Select the valuation method(s) and drag to the lower panel under the desired endpoints. Valuation studies can only be applied to health impact functions with similar endpoints.";
			labDisplay.Text = @"Incidence results for individual studies and pooled groups are listed in the bottom window. 
To assign a valuation function to a given set of incidence results, click and drag the valuation function(s) from the top window to the bottom window and release in the highlighted orange “drop zone” for the desired study or pooled group.";
			if (CommonClass.lstIncidencePoolingAndAggregation == null) this.Close();
			try
			{
				if ((CommonClass.LstDelCRFunction == null || CommonClass.LstDelCRFunction.Count == 0) && (CommonClass.LstUpdateCRFunction == null || CommonClass.LstUpdateCRFunction.Count == 0))
				{
					btnShowChanges.Enabled = false;
				}
				else
				{
					btnShowChanges.Enabled = true;
					this.toolTip1.SetToolTip(btnShowChanges, "Please resolve conflict between pooling and configuration Files.");
				}
				bIncidenceCanSelected = true;
				if (CommonClass.ValuationMethodPoolingAndAggregation == null)
					CommonClass.ValuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation();
				CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
				CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = CommonClass.IncidencePoolingAndAggregationAdvance;

				//Aggregate incidence results. For incidence pooling and valuation pooling seperately 
				if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count > 0 &&
				 CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues != null &&
					CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count >= 0)
				{

					CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
					CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
					CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>(); //YY: new added
					if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
					{
						//YY: aggregate incidence if incidence aggregation GD <> HIF result GD
						if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
						{
							foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
							{
								//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
								//YY: calculate incidence aggregation
								CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID));
							}
						}

						//aggregate valuation if incidence valuation GD <> HIF result GD
						if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
						{
							foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
							{
								//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
								//YY: calculate valuation aggregation in a seperate object
								CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
							}
						}
					}


				}
				//Try Aggregate incidence results if above method didn't work (CommonClass.lstCRResultAggregation == null)
				if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null && (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0))
				{
					//YY: aggregate incidence if incidence aggregation GD <> HIF result GD

					if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
					{
						CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
						foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
						{
							//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
							//YY: calculate incidence aggregation
							CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID));
						}
					}

					//aggregate valuation if incidence valuation GD <> HIF result GD
					if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
					{
						CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>(); //YY: new added
						foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
						{
							//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
							//YY: calculate valuation aggregation in a seperate object
							CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
						}
					}
				}


				//if it's the first time setting up the valuation window (lstValuationMethodPoolingAndAggregationBase = null), get everything from incidence pooling window.
				if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null)
				{
					CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase = new List<ValuationMethodPoolingAndAggregationBase>();
					foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
					{
						//CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = ip });
						CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = CommonClassExtension.DeepClone(ip) }); ;
					}

				}
				else //only add/remove pooling tabs which are added/removed from the incidence pooling window
				{
					//make sure valuation has all pooling (windows) as incidence pooling.
					var query = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => !CommonClass.lstIncidencePoolingAndAggregation.Select(a => a.PoolingName).Contains(p.IncidencePoolingAndAggregation.PoolingName));
					if (query != null && query.Count() > 0)
					{
						foreach (ValuationMethodPoolingAndAggregationBase vb in query)
						{
							CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Remove(vb);
						}
					}
					foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
					{
						if (!CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation.PoolingName).Contains(ip.PoolingName))
							//CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = ip });
							CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = CommonClassExtension.DeepClone(ip) });
					}
				}

				//Build the tabs
				int i = 0;
				foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
				{
					if (i == 0)
					{
						this.tabControlSelection.TabPages[0].Text = ip.PoolingName;
					}
					else
						this.tabControlSelection.TabPages.Add(ip.PoolingName);
					i++;
				}
				DataSet dsVariableDataset = BindVariableDataset();
				cbVariableDataset.DataSource = dsVariableDataset.Tables[0];
				cbVariableDataset.DisplayMember = "SetupVariableDataSetName";
				if (CommonClass.ValuationMethodPoolingAndAggregation != null)
				{
					int iVariableDataset = 0;
					foreach (DataRow dr in dsVariableDataset.Tables[0].Rows)
					{
						if (Convert.ToInt32(dr["SetupVariableDataSetID"]) == CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID)
						{
							cbVariableDataset.SelectedIndex = iVariableDataset;
							break;
						}
						iVariableDataset++;
					}
				}
				//prepare treeListView and olvValuationMethods
				initTreeView(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First());











				GC.Collect();
				this.treeListView.OwnerDraw = true;
				this.olvValuationMethods.OwnerDraw = true;
				this.olvValuationMethods.IsSimpleDropSink = true;
				this.olvValuationMethods.DropSink = new ValuationDropSink(true, this);
				this.treeListView.DropSink = new ValuationDropSink(true, this);

				btnNext.Enabled = false;
				this.treeListView.CanExpandGetter = delegate (object x)
				{
					AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
					return (dir.NodeType != 2000);
				};
				this.treeListView.ChildrenGetter = delegate (object x)
				{
					AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
					try
					{
						return getChildFromAllSelectValuationMethod(dir);


					}
					catch (UnauthorizedAccessException ex)
					{
						MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return new List<AllSelectValuationMethod>();
					}
				};




				bLoad = true;
				tabControlSelection.SelectedIndex = 0;
				updateTreeView();
				treeListView.ExpandAll();
			}
			catch (Exception ex)
			{
			}

		}
		private System.Data.DataSet BindVariableDataset()
		{
			try
			{
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select -1 as SetupVariableDatasetid,'' as SetupVariableDataSetName from setupvariabledatasets union select SetupVariableDatasetid,SetupVariableDataSetName from setupvariabledatasets  where SetupID={0}  ", CommonClass.MainSetup.SetupID);
				System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				return dsGrid;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return null;
			}
		}
		private List<AllSelectValuationMethod> getChildFromAllSelectValuationMethod(AllSelectValuationMethod allSelectValuationMethod)
		{
			List<AllSelectValuationMethod> lstAll = new List<AllSelectValuationMethod>();
			ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();

			var query = from a in vb.LstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
			lstAll = query.ToList();
			return lstAll;

		}

		private void btnAdvanced_Click(object sender, EventArgs e)
		{
			APVConfigurationAdvancedSettings frm = new APVConfigurationAdvancedSettings();
			frm.AdvanceOptionType(2);
			frm.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
			DialogResult rtn = frm.ShowDialog();
			if (rtn != DialogResult.OK) { return; }

			CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = frm.IncidencePoolingAndAggregationAdvance;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}





		private void btnNext_Click(object sender, EventArgs e)
		{
			bool bWeight = false;
			foreach (AllSelectValuationMethod allSelectValuationMethod in lstAllSelectValuationMethod)
			{
				if (allSelectValuationMethod.PoolingMethod == "User Defined Weights")
				{
					bWeight = true;
				}

			}
			if (bWeight)
			{
			}

		}
		public void addColumnsToTree(List<string> lstColumns)
		{
			while (treeListView.Columns.Count > 7) //YY: changed 5 to 7 as we added 2 fields.
			{
				treeListView.Columns.RemoveAt(7);
			}
			if (lstColumns == null || lstColumns.Count == 0) return;
			foreach (string s in lstColumns)
			{
				OLVColumn olvc = new OLVColumn();

				switch (s.Replace(" ", "").ToLower())
				{
					case "version":
						olvc.Text = "Version";
						olvc.AspectName = "Version";
						break;
					case "endpoint":
						olvc.Text = "End Point";
						olvc.AspectName = "EndPoint";
						break;
					case "author":
						olvc.Text = "Author";
						olvc.AspectName = "Author";
						break;
					case "qualifier":
						olvc.Text = "Qualifier";
						olvc.AspectName = "Qualifier";
						break;
					case "location":
						olvc.Text = "Location";
						olvc.AspectName = "Location";
						break;
					case "startage":
						olvc.Text = "Start Age";
						olvc.AspectName = "StartAge";
						break;
					case "endage":
						olvc.Text = "End Age";
						olvc.AspectName = "EndAge";
						break;
					case "year":
						olvc.Text = "Year";
						olvc.AspectName = "Year";
						break;
					case "otherpollutants":
						olvc.Text = "Other Pollutants";
						olvc.AspectName = "OtherPollutants";
						break;
					case "race":
						olvc.Text = "Race";
						olvc.AspectName = "Race";
						break;
					case "ethnicity":
						olvc.Text = "Ethnicity";
						olvc.AspectName = "Ethnicity";
						break;
					case "gender":
						olvc.Text = "Gender";
						olvc.AspectName = "Gender";
						break;
					case "function":
						olvc.Text = "Function";
						olvc.AspectName = "Function";
						break;
					case "pollutant":
						olvc.Text = "Pollutant";
						olvc.AspectName = "Pollutant";
						break;
					case "metric":
						olvc.Text = "Metric";
						olvc.AspectName = "Metric";
						break;
					case "seasonalmetric":
						olvc.Text = "Seasonal Metric";
						olvc.AspectName = "SeasonalMetric";
						break;
					case "metricstatistic":
						olvc.Text = "Metric Statistic";
						olvc.AspectName = "MetricStatistic";
						break;
					case "dataset":
						olvc.Text = "Data Set";
						olvc.AspectName = "DataSet";
						break;

				}
				olvc.Name = "Modify" + olvc.AspectName;

				olvc.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
				treeListView.Columns.Add(olvc);
			}
			treeListView.Refresh();

		}

		private void initTreeView(ValuationMethodPoolingAndAggregationBase vb)
		{
			try
			{
				IncidencePoolingAndAggregation incidencePoolingAndAggregation = vb.IncidencePoolingAndAggregation;
				List<BenMAPValuationFunction> lstBenMAPValuationFunction = new List<BenMAPValuationFunction>();
				lstAllSelectValuationMethod = vb.LstAllSelectValuationMethod;
				List<AllSelectValuationMethod> lstRoot = new List<AllSelectValuationMethod>();
				if (lstAllSelectValuationMethod == null) lstAllSelectValuationMethod = new List<AllSelectValuationMethod>();

				//Prepare available valuation functions base on pooled incidence endpoints in this(active) pooling/tab window
				lstBenMAPValuationFunction = APVX.APVCommonClass.getLstBenMAPValuationFuncitonFromEndPointGroupID(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().EndPointGroupID);
				for (int iEndPointGroup = 1; iEndPointGroup < vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count; iEndPointGroup++)
				{
					if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iEndPointGroup].EndPointGroup != vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iEndPointGroup - 1].EndPointGroup)
					{
						List<BenMAPValuationFunction> lstTemp = APVX.APVCommonClass.getLstBenMAPValuationFuncitonFromEndPointGroupID(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iEndPointGroup].EndPointGroupID);
						if (lstTemp != null && lstTemp.Count > 0) lstBenMAPValuationFunction.AddRange(lstTemp);

					}
				}
				olvValuationMethods.SetObjects(lstBenMAPValuationFunction);

				//prepare vb.LstAllSelectValuationMethod for treeListView
				if (lstAllSelectValuationMethod.Count == 0)
				{

					foreach (AllSelectCRFunction ascr in incidencePoolingAndAggregation.lstAllSelectCRFuntion)
					{
						if (ascr.CRID >= 9999) ascr.CRID = -1;
					}
					var query = incidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1); //all endpoint groups
					foreach (AllSelectCRFunction allSelectCRFunctionFirst in query)
					{
						List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
						APVX.APVCommonClass.getAllChildCR(allSelectCRFunctionFirst, incidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);
						lstCR.Insert(0, allSelectCRFunctionFirst);

						//lstCR is now all items in this endpoint group
						if (lstCR.Count == 1 && lstCR.First().CRID < 9999 && lstCR.First().CRID > 0) { }
						else
						{
							//update all not-none pooling groups. Get Reset their valutaion results.
							APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(false, ref lstCR, ref incidencePoolingAndAggregation.lstAllSelectCRFuntion, lstCR.Where(p => p.NodeType != 100).Max(p => p.NodeType), incidencePoolingAndAggregation.lstColumns);
						}
						lstCR = new List<AllSelectCRFunction>();
						if (allSelectCRFunctionFirst.PoolingMethod == "None")
						{
							APVX.APVCommonClass.getAllChildCRNotNone(allSelectCRFunctionFirst, incidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

						}
						lstCR.Insert(0, allSelectCRFunctionFirst);
						vb.LstAllSelectValuationMethod = new List<AllSelectValuationMethod>();
						foreach (AllSelectCRFunction allSelectCRFunction in lstCR)
						{
							AllSelectValuationMethod alv = new AllSelectValuationMethod()
							{
								CRID = allSelectCRFunction.CRID,
								ID = allSelectCRFunction.ID,
								PID = allSelectCRFunction.PID,
								NodeType = allSelectCRFunction.NodeType,
								Name = allSelectCRFunction.Name,
								Nickname = allSelectCRFunction.Nickname, //YY: new
								PoolingMethod = "", //YY: change to "" 
								EndPointGroup = allSelectCRFunctionFirst.EndPointGroup,
								EndPoint = allSelectCRFunction.EndPoint,
								Author = allSelectCRFunction.Author,
								Qualifier = allSelectCRFunction.Qualifier,
								Location = allSelectCRFunction.Location,
								GeographicArea = allSelectCRFunction.GeographicArea,
								StartAge = allSelectCRFunction.StartAge,
								EndAge = allSelectCRFunction.EndAge,
								Year = allSelectCRFunction.Year,
								OtherPollutants = allSelectCRFunction.OtherPollutants,
								Race = allSelectCRFunction.Race,
								Ethnicity = allSelectCRFunction.Ethnicity,
								Gender = allSelectCRFunction.Gender,
								Function = allSelectCRFunction.Function,
								Pollutant = allSelectCRFunction.Pollutant,
								Metric = allSelectCRFunction.Metric,
								SeasonalMetric = allSelectCRFunction.SeasonalMetric,
								MetricStatistic = allSelectCRFunction.MetricStatistic,
								DataSet = allSelectCRFunction.DataSet,
								Version = allSelectCRFunction.Version.ToString(),
								CountStudies = allSelectCRFunction.CountStudies,  //YY: new
								ChildCount = allSelectCRFunction.ChildCount,  //YY: new
								AgeRange = allSelectCRFunction.AgeRange  //YY: new

							};
							lstAllSelectValuationMethod.Add(alv);
						}
						if (vb.LstAllSelectValuationMethod.Count() > 0)
						{
							for (int iTemp = 0; iTemp < lstAllSelectValuationMethod.Count; iTemp++)
							{
								lstAllSelectValuationMethod[iTemp].ID = lstAllSelectValuationMethod[iTemp].ID + vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
								if (lstAllSelectValuationMethod[iTemp].PID != -1)
									lstAllSelectValuationMethod[iTemp].PID = lstAllSelectValuationMethod[iTemp].PID + vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
							}
						}
						vb.LstAllSelectValuationMethod.AddRange(lstAllSelectValuationMethod);

						if (lstCR.Count == 1)
						{
						}
						else
						{
							if (vb.lstValuationColumns == null || vb.lstValuationColumns.Count == 0)
								vb.lstValuationColumns = incidencePoolingAndAggregation.lstColumns.GetRange(0, incidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType));


						}
					}
					if (tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text != vb.IncidencePoolingAndAggregation.PoolingName) return;
					lstRoot = new List<AllSelectValuationMethod>();
					lstRoot.Add(vb.LstAllSelectValuationMethod.First());
					for (int iRoot = 1; iRoot < vb.LstAllSelectValuationMethod.Count; iRoot++)
					{
						if (lstRoot.Where(p => p.EndPointGroup == vb.LstAllSelectValuationMethod[iRoot].EndPointGroup).Count() == 0
							 )
							lstRoot.Add(vb.LstAllSelectValuationMethod[iRoot]);
					}
					treeListView.Roots = lstRoot; this.treeColumnName.ImageGetter = delegate (object x)
{
if (((AllSelectValuationMethod)x).NodeType == 100)
		//return 1;
		return 3; //YY: new image for studies added in Nov 2019
	else if (((AllSelectValuationMethod)x).NodeType == 2000)
return 2;
else
return 0;
};

					treeListView.RebuildAll(true);
					treeListView.ExpandAll();
				}
				else
				{
					lstRoot = new List<AllSelectValuationMethod>();
					lstRoot.Add(vb.LstAllSelectValuationMethod.First());
					for (int iRoot = 1; iRoot < vb.LstAllSelectValuationMethod.Count; iRoot++)
					{
						if (lstRoot.Where(p => p.EndPointGroup == vb.LstAllSelectValuationMethod[iRoot].EndPointGroup).Count() == 0
								)
							lstRoot.Add(vb.LstAllSelectValuationMethod[iRoot]);
					}
					treeListView.Roots = lstRoot; this.treeColumnName.ImageGetter = delegate (object x)
{
if (((AllSelectValuationMethod)x).NodeType == 100)
return 3;
else if (((AllSelectValuationMethod)x).NodeType == 2000)
return 2;
else
return 0;
};
					treeListView.RebuildAll(true);
					treeListView.ExpandAll();


				}
				vb.lstValuationColumns = incidencePoolingAndAggregation.lstColumns; //show all columns from incidence pooling.
				if (vb.LstAllSelectValuationMethod.Count > 1 && vb.lstValuationColumns != null && vb.lstValuationColumns.Count > 0) //&& treeListView.Columns.Count == 5
					addColumnsToTree(vb.lstValuationColumns);
			}
			catch
			{
			}
		}

		private void updateTreeView()
		{
			treeListView.RebuildAll(true);
			treeListView.ExpandAll();

		}



		private void olvIncidenceResults_SelectedIndexChanged(object sender, EventArgs e)
		{


		}

		private void treeListView_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
		{
			if (e.Column.Text == "Pooling Method")
			{

				((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbPoolingMethod_SelectedIndexChanged);

				((TreeListView)sender).RefreshItem(e.ListViewItem);
				e.Cancel = true;
			}

			else if (e.Column.Text == "Weight")
			{
				((TextBox)e.Control).TextChanged -= new EventHandler(txt_TextChanged);
				((TreeListView)sender).RefreshItem(e.ListViewItem);
				((TextBox)e.Control).Dispose();
				e.Cancel = true;
			}

		}

		private void treeListView_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
		{
			base.OnClick(e);
			if (e.Column == null) return;
			AllSelectValuationMethod asvm = (AllSelectValuationMethod)e.RowObject;

			if (e.Column.Text == "Pooling Method" && asvm.NodeType != 2000 && e.Value.ToString() != "")
			{

				ComboBox cb = new ComboBox();
				cb.Bounds = e.CellBounds;
				cb.Font = ((TreeListView)sender).Font;
				cb.DropDownStyle = ComboBoxStyle.DropDownList;
				if (!CommonClass.CRRunInPointMode)
				{
					cb.Items.Add("None");
					cb.Items.Add("Sum Dependent");
					cb.Items.Add("Sum Independent");
					cb.Items.Add("Subtraction Dependent");
					cb.Items.Add("Subtraction Independent");
					cb.Items.Add("User Defined Weights");
					cb.Items.Add("Random Or Fixed Effects");
					cb.Items.Add("Fixed Effects");
				}
				else
				{
					cb.Items.Add("None");
					cb.Items.Add("Sum Dependent");
					cb.Items.Add("Subtraction Dependent");
					cb.Items.Add("User Defined Weights");


				}
				if (e.Value != null)
					cb.SelectedText = e.Value.ToString(); cb.SelectedIndexChanged += new EventHandler(cbPoolingMethod_SelectedIndexChanged);
				cb.Tag = e.RowObject; e.Control = cb;
			}
			else if (e.Column.Text == "Weight")
			{
				if (asvm.PoolingMethod == "None" || (asvm.PoolingMethod == "" && asvm.NodeType != 2000))
				{
					e.Cancel = true;
					return;
				}
				List<AllSelectValuationMethod> lstParent = new List<AllSelectValuationMethod>();
				getParentNotNone(asvm, lstParent);
				if (lstParent.Where(p => p.PoolingMethod == "User Defined Weights").Count() > 0)
				{
					TextBox txt = new TextBox();
					txt.Bounds = e.CellBounds;
					txt.Font = ((ObjectListView)sender).Font;
					txt.TextChanged += new EventHandler(txt_TextChanged);
					txt.Tag = e.RowObject;
					e.Control = txt;
					if (e.Value != null)//&& asvm.PoolingMethod != "None")
					{
						txt.Text = e.Value.ToString();
					}
				}
				else
				{
					e.Cancel = true;
				}
			}
			else
			{
				e.Cancel = true;
			}
		}
		void cbPoolingMethod_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				ComboBox cb = (ComboBox)sender;
				if (((AllSelectValuationMethod)cb.Tag).PoolingMethod == cb.Text) return;
				((AllSelectValuationMethod)cb.Tag).PoolingMethod = cb.Text;

				lstAllSelectValuationMethod.Where(p => p.ID == ((AllSelectValuationMethod)cb.Tag).ID).First().PoolingMethod = cb.Text;
				if (cb.Text.Contains("Subtraction"))
				{
					AllSelectValuationMethod asvm = (AllSelectValuationMethod)treeListView.SelectedObjects[0]; ;
					if (asvm.AgeRange.Contains(";"))
					{
						MessageBox.Show("The functions within this group have discrete age ranges."
								+ "\n"
								+ "Using \"" + cb.Text + "\" may generate unreliable results."
								, "Questionable Method Selected"
								, MessageBoxButtons.OK
								, MessageBoxIcon.Warning
								);
					}
				}

				double d = 0;
				int widthWeight = 0;
				ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
				foreach (AllSelectValuationMethod asvm in vb.LstAllSelectValuationMethod)
				{
					if (asvm.PoolingMethod == "User Defined Weights")
					{
						widthWeight = 60;
						List<AllSelectValuationMethod> lst = new List<AllSelectValuationMethod>();
						getAllChildMethodNotNone(asvm, lstAllSelectValuationMethod, ref lst);
						d = 0;
						if (lst.Count > 0 && (lst.Sum(p => p.Weight) < 0.99 || lst.Sum(p => p.Weight) > 1.01)) // lst.Min(p => p.Weight) == 0)
						{
							d = Math.Round(Convert.ToDouble(1.000 / Convert.ToDouble(lst.Count)), 2);
							for (int i = 0; i < lst.Count; i++)
							{
								lst[i].Weight = d;
							}
						}
					}
					else if (asvm.PoolingMethod == "None" || (asvm.PoolingMethod == "" && asvm.NodeType != 2000))
					{
						asvm.Weight = 0;
					}
					else
					{
						List<AllSelectValuationMethod> lst = new List<AllSelectValuationMethod>();
						getAllChildMethodNotNone(asvm, vb.LstAllSelectValuationMethod, ref lst);
						d = 0;
						if (lst.Count > 0)
						{
							for (int i = 0; i < lst.Count; i++)
							{
								lst[i].Weight = d;
							}
						}
					}
				}

				int iTop = Convert.ToInt32(treeListView.TopItemIndex.ToString());

				OLVColumn weightColumn = treeListView.AllColumns[4];
				if (weightColumn.Width != widthWeight)
				{
					weightColumn.Width = widthWeight;
					treeListView.RebuildColumns();
				}

				treeListView.TopItemIndex = iTop;

			}
			catch
			{ }
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfdAPV = new SaveFileDialog();
			sfdAPV.Filter = "APV files (*.apvx)|*.apvx";
			sfdAPV.FilterIndex = 2;
			sfdAPV.RestoreDirectory = true;
			sfdAPV.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APV";
			if (sfdAPV.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string _filePathAPV = sfdAPV.FileName;
				CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID = Convert.ToInt32((cbVariableDataset.SelectedItem as DataRowView)["SetupVariableDatasetID"].ToString());
				CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetName = cbVariableDataset.Text;
				CommonClass.ValuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
				if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectCRFunctionIncidenceAggregation == null)
				{
					foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
					{
						IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(a => a.PoolingName == vb.IncidencePoolingAndAggregation.PoolingName).First();
						if (ip.lstAllSelectCRFuntion != null)
						{
							vb.lstAllSelectCRFunctionIncidenceAggregation = ip.lstAllSelectCRFuntion;
						}
						else
						{
							vb.lstAllSelectCRFunctionIncidenceAggregation = new List<AllSelectCRFunction>();
						}

					}
				}

				if (APVX.APVCommonClass.SaveAPVFile(_filePathAPV, CommonClass.ValuationMethodPoolingAndAggregation))
					MessageBox.Show("APV file has been saved.", "File saved");
				else
					MessageBox.Show("Out of memory.", "Error");
			}
		}
		public string _filePath = "";
		public void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				DialogResult rtn;
				bool isBatch = false;
				if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
				{
					isBatch = true;
					CommonClass.BenMAPPopulation = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
					CommonClass.GBenMAPGrid = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType;
				}
				string apvProcess = "Processing APV File...";
				if (isBatch)
				{
					Console.SetCursorPosition(0, Console.CursorTop);
					Console.Write(new String(' ', Console.BufferWidth));
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					Console.Write(apvProcess);
				}
				if (CommonClass.ValuationMethodPoolingAndAggregation == null) { CommonClass.ValuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation(); }

				if (!isBatch)
				{
					CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID = Convert.ToInt32((cbVariableDataset.SelectedItem as DataRowView)["SetupVariableDatasetID"].ToString());
					CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetName = cbVariableDataset.Text;
				}
				CommonClass.ValuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);

				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					if (vb.LstAllSelectValuationMethod == null || vb.LstAllSelectValuationMethod.Count == 0)
					{
						initTreeView(vb);

					}
					else if (vb.LstAllSelectValuationMethod.Where(p => p.NodeType == 2000).Count() == 0)
					{
					}
					else
					{
						//YY: Only check variable dataset if users do valuation
						if (cbVariableDataset.Items.Count > 1 && cbVariableDataset.SelectedIndex == 0)
						{
							MessageBox.Show("Select Variable Dataset first!");
							return;
						}
					}

				}
				if (Tools.CalculateFunctionString.dicValuationMethodInfo != null) Tools.CalculateFunctionString.dicValuationMethodInfo.Clear();
				//YY: Skip the following as we assign user defined weight in treeview weight column already. 
				//foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				//{
				//    bool bWeight = false;
				//    foreach (AllSelectValuationMethod allSelectValuationMethod in vb.LstAllSelectValuationMethod)
				//    {
				//        if (allSelectValuationMethod.PoolingMethod == "User Defined Weights")
				//        {
				//            bWeight = true;
				//        }

				//    }
				//    if (bWeight && !isBatch)// if "User Defined Weights", open weight form to setup weight values
				//    {
				//        APVX.SelectValuationWeight selectfrm = new APVX.SelectValuationWeight();
				//        selectfrm.txtPoolingWindowName.Text = vb.IncidencePoolingAndAggregation.PoolingName;
				//        selectfrm.lstAllSelectValuationMethod = vb.LstAllSelectValuationMethod;
				//        DialogResult rtnWeight = selectfrm.ShowDialog();
				//        if (rtnWeight != DialogResult.OK)
				//        {
				//            return;
				//        }
				//        lstAllSelectValuationMethod = selectfrm.lstAllSelectValuationMethod;
				//    }
				//}
				//aggregate if the aggregation results were not loaded when valuation window is loaded. 
				if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
				{
					CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
					CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>(); //YY: new added
					if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count == 0 ||
					CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues == null ||
					 CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count == 0)
					{
					}
					else
					{
						if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
						{
							//aggregate incidence if incidence aggregation GD <> HIF result GD
							if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
							{
								CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
								foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
								{
									//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
									//YY: calculate incidence aggregation
									CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID));
								}
							}

							//aggregate incidence for valuation if incidence valuation GD <> HIF result GD
							if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
							{
								CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>();
								foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
								{
									//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
									//YY: calculate valuation aggregation in a seperate object
									CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
								}
							}
						}
					}
				}
				//skip QALY for now. 
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					vb.lstAllSelectQALYMethod = null;
					vb.lstAllSelectQALYMethodAndValue = null;
					vb.lstAllSelectQALYMethodAndValueAggregation = null;
				}
				//If loading existing APVR, HIF need to be re-run. 
				if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count == 0 ||
CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues == null ||
CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count == 0)
				{
					if (CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath == "")
					{
						MessageBox.Show("Please run Health Impact Functions first.");

						return;
					}
					else if (!System.IO.File.Exists(CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath))
					{
						MessageBox.Show("Please run Health Impact Functions first.");

						return;
					}
				}
				//Start valuation calculation and pooling-------------------------------
				string _filePathAPV = "";
				if (!isBatch)
				{
					_filePath = "";

					rtn = MessageBox.Show("Save the APV results file (*.apvrx)?", "Confirm Save", MessageBoxButtons.YesNo);
					if (rtn == DialogResult.No) { return; }
					if (rtn == DialogResult.Yes)
					{

						SaveFileDialog sfd = new SaveFileDialog();
						sfd.Filter = "APVR files (*.apvrx)|*.apvrx";
						sfd.FilterIndex = 2;
						sfd.RestoreDirectory = true;
						sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";
						if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
						_filePath = sfd.FileName;


					}
				}
				string tip = "Running to create pooled results file. Please wait.";
				if (_filePathAPV != "")
				{
					if (APVX.APVCommonClass.SaveAPVFile(_filePathAPV, CommonClass.ValuationMethodPoolingAndAggregation))
						MessageBox.Show("APV file saved.", "File saved");
					else
						MessageBox.Show("Out of memory.", "Error");

				}



				if (!isBatch) WaitShow(tip);
				DateTime dtRunStart = DateTime.Now;
				//If CFGR is not loaded yet, load it.
				if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count == 0 ||
				 CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues == null ||
					CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count == 0)
				{
					string err = "";
					CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile(CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath, ref err);
					if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
					{
						MessageBox.Show(err);
						return;
					}
					CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;

					if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
					{
						//aggregate incidence if incidence aggregation GD <> HIF result GD
						if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
						{
							CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
							foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
							{
								//YY: calculate incidence aggregation
								CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID));
							}
						}

						//aggregate valuation if incidence valuation GD <> HIF result GD
						if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
						{
							CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>();
							foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
							{
								//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
								//YY: calculate valuation aggregation in a seperate object
								CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
							}
						}
					}
				}
				// Use aggregated incidence results (for vp) calculated at form_load to pop CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{

					foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
					{
						if (alsc.PoolingMethod == "" && alsc.NodeType == 100)
						{
							try
							{
								//YY: Check if lstValuationResultAggregation is null or 0 when old APV file is imported. 
								if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Count == 0) //YY: no valuation aggregation
								{
									if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
									{
										if (CommonClass.lstCRResultAggregation.Count != 0) // load old apv file where lstValuationResultAggregation didn't exist
										{
											alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
										}
										else
										{
											alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
										}
									}
									else
									{
										alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
									}
								}
								else
								{
									alsc.CRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
								}
							}
							catch (Exception ex)
							{
							}
						}
					}


				}
				//YY: Use aggregated incidence results (for ip) calculated at form_load to pop CommonClass.lstIncidencePoolingAndAggregation[i].lstAllSelectCRFuntion[i].CRSelectFunctionCalculateValue as well
				foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
				{
					foreach (AllSelectCRFunction ascr in ip.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
					{
						if (ascr.PoolingMethod == "" && ascr.NodeType == 100)
						{
							try
							{
								if (CommonClass.lstCRResultAggregation.Count == 0)
								{
									ascr.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == ascr.CRID).First();
								}
								else
								{
									ascr.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(p => p.CRSelectFunction.CRID == ascr.CRID).First();
								}
							}
							catch (Exception ex)
							{
							}
						}
					}

				}
				//YY: also populate CommonClass.ValuationMethodPoolingAndAggregation.lstVB.lstAllSelectCRFunctionIncidenceAggregation --- so that aggreated ip can be stored in apvrx file.
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(a => a.PoolingName == vb.IncidencePoolingAndAggregation.PoolingName).First();
					vb.lstAllSelectCRFunctionIncidenceAggregation = ip.lstAllSelectCRFuntion;
				}



				//Do pooling for incidence results aggregated at incidence pooling aggregation scale. 
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1);
					foreach (AllSelectCRFunction allSelectCRFunctionFirst in query)
					{
						List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
						APVX.APVCommonClass.getAllChildCR(allSelectCRFunctionFirst, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

						lstCR.Insert(0, allSelectCRFunctionFirst);
						if (lstCR.Count == 1 && lstCR.First().CRID < 9999 && lstCR.First().CRID > 0) { }
						else
						{
							APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref lstCR, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, lstCR.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
						}
					}

				}



				//Do pooling for incidence results at valuation pooling aggregation scale
				//foreach (IncidencePoolingAndAggregation ip in CommonClass.IncidencePoolingAndAggregation)
				//{
				//  var query = ip.lstAllSelectCRFuntion.Where(p => p.PID == -1);
				//  foreach (AllSelectCRFunction allSelectCRFunctionFirst in query).....
				//}
				//
				foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					var query = vb.lstAllSelectCRFunctionIncidenceAggregation.Where(p => p.PID == -1);
					foreach (AllSelectCRFunction allSelectCRFunctionFirst in query)
					{
						List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
						APVX.APVCommonClass.getAllChildCR(allSelectCRFunctionFirst, vb.lstAllSelectCRFunctionIncidenceAggregation, ref lstCR);

						lstCR.Insert(0, allSelectCRFunctionFirst);
						if (lstCR.Count == 1 && lstCR.First().CRID < 9999 && lstCR.First().CRID > 0) { }
						else
						{
							APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref lstCR, ref vb.lstAllSelectCRFunctionIncidenceAggregation, lstCR.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
						}
					}
				}

				CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = CommonClass.IncidencePoolingAndAggregationAdvance;

				//Do valuation calculation and then pooling for valuation results
				APVX.APVCommonClass.CalculateValuationMethodPoolingAndAggregation(ref CommonClass.ValuationMethodPoolingAndAggregation);
				if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null &&
CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
				{

				}
				DateTime dtNow = DateTime.Now;
				TimeSpan ts = dtNow - dtRunStart;
				CommonClass.ValuationMethodPoolingAndAggregation.lstLog = new List<string>();
				CommonClass.ValuationMethodPoolingAndAggregation.lstLog.Add("Processing complete. Valuation processing time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds.");
				if (!isBatch) WaitClose();
				else
				{
					Console.SetCursorPosition(apvProcess.Length, Console.CursorTop);
					Console.Write(new String(' ', Console.BufferWidth));
					Console.SetCursorPosition(apvProcess.Length, Console.CursorTop - 1);
					Console.Write("Completed" + Environment.NewLine);
				}

				CommonClass.ValuationMethodPoolingAndAggregation.CreateTime = DateTime.Now;
				if (_filePath != "")
				{
					// This line below (savings CFGRX file at end of APV process) results in duplicate CFGRX files in CFGR and APVR folders. When commented out, errors in running APV--.   01/17/2020 MP
					// Issue possibly around line 343 in "APVCommonClass.cs" when the APVR string filepath is used to locate CFGR folder for loading. Change to CFGR folder?
					Configuration.ConfigurationCommonClass.SaveCRFRFile(CommonClass.BaseControlCRSelectFunctionCalculateValue, _filePath.Substring(0, _filePath.Length - 6) + ".cfgrx");

					if (APVX.APVCommonClass.SaveAPVRFile(_filePath, CommonClass.ValuationMethodPoolingAndAggregation))
					{
						if (!isBatch) MessageBox.Show("File saved.", "File saved");
						else
						{
							Console.SetCursorPosition(0, Console.CursorTop);
							Console.Write(new String(' ', Console.BufferWidth));
							Console.SetCursorPosition(0, Console.CursorTop - 1);
							Console.WriteLine("Results Saved At:" + Environment.NewLine + _filePath);
						}
					}
					else
					{
						if (!isBatch) MessageBox.Show("Out of memory.", "Error");
						else
						{
							Console.SetCursorPosition(0, Console.CursorTop);
							Console.Write(new String(' ', Console.BufferWidth));
							Console.SetCursorPosition(0, Console.CursorTop - 1);
							Console.WriteLine("Error Saving APVR File");
						}
					}

				}
				if (!isBatch)
				{
					GC.Collect();
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
				}

			}

			catch (Exception ex)
			{

			}
		}
		public delegate void AsyncValuation(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, AllSelectValuationMethodAndValue asvv, ref ValuationMethodPoolingAndAggregationBase vb);
		public delegate void AsyncQALY(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, AllSelectQALYMethodAndValue asvv, ref ValuationMethodPoolingAndAggregationBase vb);
		public delegate void AsyncIncidence(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, CRSelectFunctionCalculateValue asvv, ref ValuationMethodPoolingAndAggregationBase vb);

		public void ApplyAggregationFromValuationMethodPoolingAndAggregation(List<GridRelationship> lstGridRelationshipAll, BenMAPGrid gBenMAPGrid, ref ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
		{
			try
			{
				int icount = 0;
				double d = 0;
				int idAggregation = -1;
				GridRelationship gridRelationship = null;
				GridRelationship gridRelationshipIncidence = null;
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
							 valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null &&
								valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
				{
					if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID)
					{
						idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;

						if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
						{
							gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
						}
						else
						{
							gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

						}


						for (int ivb = 0; ivb < valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count; ivb++)
						{
							ValuationMethodPoolingAndAggregationBase vb = valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[ivb];
							if (vb.LstAllSelectValuationMethodAndValue != null)
							{
								vb.LstAllSelectValuationMethodAndValueAggregation = new List<AllSelectValuationMethodAndValue>();

								foreach (AllSelectValuationMethodAndValue asvv in vb.LstAllSelectValuationMethodAndValue)
								{

									vb.LstAllSelectValuationMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));

								}
								vb.LstAllSelectValuationMethodAndValue = null;

							}
							if (vb.IncidencePoolingResult != null)
							{






							}


						}
					}
					else
					{

					}
				}
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
								valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation != null &&
								valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID && valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
				{
					idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID;
					if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
					{
						gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
					}
					else
					{
						gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

					}
					for (int ivb = 0; ivb < valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count; ivb++)
					{
						ValuationMethodPoolingAndAggregationBase vb = valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[ivb];
						if (vb.lstAllSelectQALYMethodAndValue != null)
						{
							vb.lstAllSelectQALYMethodAndValueAggregation = new List<AllSelectQALYMethodAndValue>();

							foreach (AllSelectQALYMethodAndValue asvv in vb.lstAllSelectQALYMethodAndValue)
							{
								vb.lstAllSelectQALYMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));



							}
							vb.lstAllSelectQALYMethodAndValue = null;

						}
					}
				}



			}
			catch
			{ }
		}
		private List<string> lstAsyns = new List<string>();
		private void outPut(IAsyncResult ar)
		{
			try
			{
				lstAsyns.RemoveAt(0);
				if (lstAsyns.Count == 0)
				{

					WaitClose();
					if (_filePath != "")
					{
						APVX.APVCommonClass.SaveAPVFile(_filePath, CommonClass.ValuationMethodPoolingAndAggregation);
						MessageBox.Show("File saved.", "File saved");

					}
					this.DialogResult = System.Windows.Forms.DialogResult.OK;

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
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

		public void WaitClose()
		{
			if (waitMess.InvokeRequired)
				waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
			else
				DoCloseJob();
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
		private void cbPoolingWindow_SelectedIndexChanged(object sender, EventArgs e)
		{



		}

		private void tabControlSelection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!bLoad) return;



			ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
			addColumnsToTree(vb.lstValuationColumns);
			initTreeView(vb);





			updateTreeView();
			treeListView.ExpandAll();
			tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Controls.Add(treeListView);

		}

		public void updateDrop(List<BenMAPValuationFunction> lstBenMAPValuationFunction, AllSelectValuationMethod Target, int iTargetIndex, bool isAbove)
		{
			try
			{
				ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
				if (isAbove && vb.LstAllSelectValuationMethod.Count > 1)
				{
					Target = treeListView.GetModelObject(iTargetIndex - 1) as AllSelectValuationMethod;
				}
				int maxid = vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
				if (Target == null) return;
				if (Target.NodeType == 2000)
				{
					var query = vb.LstAllSelectValuationMethod.Where(p => p.ID == Target.PID);
					if (query.Count() > 0)
					{
						AllSelectValuationMethod av5 = query.First();
						maxid = vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
						foreach (BenMAPValuationFunction bmv in lstBenMAPValuationFunction)
						{
							if (bmv.EndPointGroup != av5.EndPointGroup) continue;
							vb.LstAllSelectValuationMethod.Add(new AllSelectValuationMethod()
							{
								BenMAPValuationFunction = bmv,
								ID = maxid,
								PID = Target.PID,
								Name = bmv.Qualifier + "|" + bmv.DistA + "|" + bmv.StartAge + "-" + bmv.EndAge,
								Nickname = "", //YY: valuation function doesn't need nickname
								Function = bmv.Function,
								StartAge = av5.StartAge,
								EndAge = av5.EndAge,
								NodeType = 2000,
								Qualifier = bmv.Qualifier,
								Author = av5.Author,
								EndPointGroup = bmv.EndPointGroup,
								EndPoint = av5.EndPoint,
								EndPointID = av5.EndPointID,
								Version = av5.Version,
								DataSet = av5.DataSet,
								Ethnicity = av5.Ethnicity,
								Gender = av5.Gender,
								Location = av5.Location,
								GeographicArea = av5.GeographicArea,
								Race = av5.Race,
								Year = av5.Year,
								Metric = av5.Metric,
								MetricStatistic = av5.MetricStatistic,
								OtherPollutants = av5.OtherPollutants,
								Pollutant = av5.Pollutant,
								SeasonalMetric = av5.SeasonalMetric,
								AgeRange = av5.AgeRange,
							});
							maxid++;

						}
					}
				}
				else
				{
					var query = vb.LstAllSelectValuationMethod.Where(p => p.PID == Target.ID).ToList();
					maxid = vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
					if (query.Count() == 0 || query.First().NodeType == 2000)
					{
						foreach (BenMAPValuationFunction bmv in lstBenMAPValuationFunction)
						{
							if (bmv.EndPointGroup != Target.EndPointGroup) continue;
							vb.LstAllSelectValuationMethod.Add(new AllSelectValuationMethod()
							{
								BenMAPValuationFunction = bmv,
								ID = maxid,
								PID = Target.ID,
								Name = bmv.Qualifier + "|" + bmv.DistA + "|" + bmv.StartAge + "-" + bmv.EndAge,
								Nickname = "", //YY: valuation function doesn't need nickname
								Function = bmv.Function,
								StartAge = Target.StartAge,
								EndAge = Target.EndAge,
								NodeType = 2000,
								Qualifier = bmv.Qualifier,
								Author = Target.Author,
								EndPointGroup = bmv.EndPointGroup,
								EndPoint = bmv.EndPoint,
								EndPointID = bmv.EndPointID,
								Version = Target.Version,
								DataSet = Target.DataSet,
								Ethnicity = Target.Ethnicity,
								Gender = Target.Gender,
								Location = Target.Location,
								GeographicArea = Target.GeographicArea,
								Race = Target.Race,
								Year = Target.Year,
								Metric = Target.Metric,
								MetricStatistic = Target.MetricStatistic,
								OtherPollutants = Target.OtherPollutants,
								Pollutant = Target.Pollutant,
								SeasonalMetric = Target.SeasonalMetric,
							});
							maxid++;

						}
					}
					else
					{



					}
				}
				//YY: Update Pooling method
				foreach (AllSelectValuationMethod asvm in vb.LstAllSelectValuationMethod)
				{
					List<AllSelectValuationMethod> lstChildValuationNode = new List<AllSelectValuationMethod>();
					getAllChildMethodNotNone(asvm, vb.LstAllSelectValuationMethod, ref lstChildValuationNode);

					//TextDecoration decoration = new TextDecoration("None",ContentAlignment.MiddleLeft);
					CellBorderDecoration cbd = new CellBorderDecoration();
					if (asvm.NodeType != 2000 && lstChildValuationNode.Count() > 1)
					{
						if (asvm.PoolingMethod == "")
						{
							asvm.PoolingMethod = "None";
						}
					}
					else
					{
						asvm.PoolingMethod = "";
					}
				}

				updateTreeView();
			}
			catch
			{
			}

		}

		private void btDelSelection_Click(object sender, EventArgs e)
		{
			ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();

			foreach (AllSelectValuationMethod av in treeListView.SelectedObjects)
			{
				if (av.NodeType == 2000)
				{
					vb.LstAllSelectValuationMethod.Remove(av);
					//count deleted v function's parent's children. If <=1, set pooling method = "".
					foreach (AllSelectValuationMethod asvm in vb.LstAllSelectValuationMethod)
					{
						List<AllSelectValuationMethod> lstChildValuationNode = new List<AllSelectValuationMethod>();
						getAllChildMethodNotNone(asvm, vb.LstAllSelectValuationMethod, ref lstChildValuationNode);

						//TextDecoration decoration = new TextDecoration("None",ContentAlignment.MiddleLeft);
						CellBorderDecoration cbd = new CellBorderDecoration();
						if (asvm.NodeType != 2000 && lstChildValuationNode.Count() > 1)
						{
							if (asvm.PoolingMethod == "")
							{
								asvm.PoolingMethod = "None";
							}
						}
						else
						{
							asvm.PoolingMethod = "";
						}
					}
				}
			}



			updateTreeView();
		}

		private void treeListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
		{
			//YY: cancel re-order as we don't want to adjust groupings in valuation. (updated in Dec 2019)
			e.Cancel = true;
			return;
			try
			{
				if (e.NewDisplayIndex == 0 || e.NewDisplayIndex == 1 || e.OldDisplayIndex == 0 || e.OldDisplayIndex == 1 ||
						e.NewDisplayIndex == 2 || e.OldDisplayIndex == 2 || e.NewDisplayIndex == 3 || e.OldDisplayIndex == 3
						|| e.NewDisplayIndex == 4 || e.NewDisplayIndex == 4)
				{
					e.Cancel = true;
					return;
				}
				OLVColumn olvc = treeListView.Columns[e.OldDisplayIndex] as OLVColumn;
				List<BrightIdeasSoftware.OLVColumn> lstOLVColumns = new List<OLVColumn>();
				foreach (BrightIdeasSoftware.OLVColumn olvc2 in this.treeListView.Columns)
				{
					lstOLVColumns.Add(olvc2);
				}
				olvc = lstOLVColumns.Where(p => p.DisplayIndex == e.OldDisplayIndex).First();
				lstOLVColumns = lstOLVColumns.OrderBy(p => p.DisplayIndex).ToList();
				lstOLVColumns.Remove(olvc);
				lstOLVColumns.Insert(e.NewDisplayIndex, olvc);
				lstOLVColumns = lstOLVColumns.Where(p => p.DisplayIndex != 0 && p.DisplayIndex != 1 && p.DisplayIndex != 2 && p.DisplayIndex != 3 && p.DisplayIndex != 4).ToList();


				ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();

				vb.lstValuationColumns = lstOLVColumns.Select(p => p.Text).ToList();
				vb.LstAllSelectValuationMethod = new List<AllSelectValuationMethod>();
				List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
				APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);
				lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
				List<CRSelectFunctionCalculateValue> lstCRValue = new List<CRSelectFunctionCalculateValue>();
				foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
				{
					if (acr.CRSelectFunctionCalculateValue != null && acr.CRSelectFunctionCalculateValue.CRSelectFunction == null)
					{




					}
					else
					{
						if (acr.CRSelectFunctionCalculateValue != null && (acr.PoolingMethod == "" || (acr.PoolingMethod != "None" && acr.PoolingMethod != "")))
						{
							bool isCanChoose = true;
							if (acr.PID != -1)
							{
								var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == acr.PID);
								while (query != null && query.Count() > 0)
								{
									AllSelectCRFunction acrParent = query.First();
									if (acrParent.PoolingMethod != "None" && acrParent.PoolingMethod != "")
									{
										isCanChoose = false;
										break;
									}
									query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == acrParent.PID);
								}
							}
							if (isCanChoose)
								lstCRValue.Add(acr.CRSelectFunctionCalculateValue);
						}
					}
				}
				List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>(); Dictionary<string, List<CRSelectFunctionCalculateValue>> dicEndPointGroupCR = new Dictionary<string, List<CRSelectFunctionCalculateValue>>();
				foreach (CRSelectFunctionCalculateValue cr in lstCRValue)
				{
					if (dicEndPointGroupCR.ContainsKey(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup))
						dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
					else
					{
						dicEndPointGroupCR.Add(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, new List<CRSelectFunctionCalculateValue>());
						dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
					}
				}

				int levelPool = Convert.ToInt16(lbLevelPool.Text);
				foreach (KeyValuePair<string, List<CRSelectFunctionCalculateValue>> k in dicEndPointGroupCR)
				{

					List<AllSelectCRFunction> lstTemp = IncidencePoolingandAggregation.getLstAllSelectCRFunction(k.Value, vb.lstValuationColumns, k.Key, -1, levelPool); //YY: Need to remember to update this label when getting valuation step.
					if (lstAllSelectCRFunction.Count() > 0)
					{
						for (int iTemp = 0; iTemp < lstTemp.Count; iTemp++)
						{
							lstTemp[iTemp].ID = lstTemp[iTemp].ID + lstAllSelectCRFunction.Max(p => p.ID) + 1;
							if (lstTemp[iTemp].PID != -1)
								lstTemp[iTemp].PID = lstTemp[iTemp].PID + lstAllSelectCRFunction.Max(p => p.ID) + 1;
						}
					}
					if (lstTemp != null && lstTemp.Count > 0) lstAllSelectCRFunction.AddRange(lstTemp);
				}
				List<AllSelectValuationMethod> lstValuationMethod = new List<AllSelectValuationMethod>();
				foreach (AllSelectCRFunction allSelectCRFunction in lstAllSelectCRFunction)
				{
					AllSelectValuationMethod alv = new AllSelectValuationMethod()
					{
						CRID = allSelectCRFunction.CRID,
						ID = allSelectCRFunction.ID,
						PID = allSelectCRFunction.PID,
						NodeType = allSelectCRFunction.NodeType,
						Name = allSelectCRFunction.Name,
						Nickname = allSelectCRFunction.Nickname, //YY: new
						PoolingMethod = "None",
						EndPointGroup = allSelectCRFunction.EndPointGroup,
						EndPoint = allSelectCRFunction.EndPoint,
						Author = allSelectCRFunction.Author,
						Qualifier = allSelectCRFunction.Qualifier,
						Location = allSelectCRFunction.Location,
						GeographicArea = allSelectCRFunction.GeographicArea,
						StartAge = allSelectCRFunction.StartAge,
						EndAge = allSelectCRFunction.EndAge,
						Year = allSelectCRFunction.Year,
						OtherPollutants = allSelectCRFunction.OtherPollutants,
						Race = allSelectCRFunction.Race,
						Ethnicity = allSelectCRFunction.Ethnicity,
						Gender = allSelectCRFunction.Gender,
						Function = allSelectCRFunction.Function,
						Pollutant = allSelectCRFunction.Pollutant,
						Metric = allSelectCRFunction.Metric,
						SeasonalMetric = allSelectCRFunction.SeasonalMetric,
						MetricStatistic = allSelectCRFunction.MetricStatistic,
						DataSet = allSelectCRFunction.DataSet,
						Version = allSelectCRFunction.Version.ToString(),
					};
					lstValuationMethod.Add(alv);
				}
				int iMax = lstValuationMethod.Max(p => p.ID) + 1;
				List<AllSelectValuationMethod> lstExistLeaf = vb.LstAllSelectValuationMethod.Where(p => p.NodeType == 2000).ToList();
				foreach (AllSelectValuationMethod avmExist in lstExistLeaf)
				{
					var parent = vb.LstAllSelectValuationMethod.Where(p => p.ID == avmExist.PID).First();
					avmExist.PID = lstValuationMethod.Where(p => p.CRID == parent.CRID).First().ID;
					avmExist.ID = iMax;
					lstValuationMethod.Add(avmExist);
					iMax++;
				}
				vb.LstAllSelectValuationMethod = lstValuationMethod;
				initTreeView(vb);
				updateTreeView();
				treeListView.ExpandAll();
			}
			catch (Exception)
			{
			}
		}

		public static void getSecond(ref List<CRSelectFunctionCalculateValue> lstSecond, List<string> lstOLVColumns, int i, List<string> lstParent)
		{
			for (int k = 0; k < i; k++)
			{
				switch (lstOLVColumns[k].Replace(" ", "").ToLower())
				{
					case "endpoint":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == lstParent[i - k - 1]).ToList();
						break;

					case "author":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author == lstParent[i - k - 1]).ToList();
						break;
					case "qualifier":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier == lstParent[i - k - 1]).ToList();
						break;
					case "location":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.strLocations == lstParent[i - k - 1]).ToList();
						break;
					case "startage":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.StartAge.ToString() == lstParent[i - k - 1]).ToList();
						break;
					case "endage":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndAge.ToString() == lstParent[i - k - 1]).ToList();
						break;
					case "year":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString() == lstParent[i - k - 1]).ToList();
						break;
					case "otherpollutants":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants == lstParent[i - k - 1]).ToList();
						break;
					case "race":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Race == lstParent[i - k - 1]).ToList();
						break;
					case "ethnicity":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Ethnicity == lstParent[i - k - 1]).ToList();
						break;
					case "gender":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Gender == lstParent[i - k - 1]).ToList();
						break;
					case "function":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function == lstParent[i - k - 1]).ToList();
						break;
					case "pollutant":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName == lstParent[i - k - 1]).ToList();
						break;
					case "metric":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName == lstParent[i - k - 1]).ToList();
						break;
					case "seasonalmetric":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName == lstParent[i - k - 1]).ToList();
						break;
					case "metricstatistic":
						lstSecond = lstSecond.Where(p => Enum.GetName(typeof(MetricStatic), p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic) == lstParent[i - k - 1]).ToList();
						break;
					case "dataset":
						lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName == lstParent[i - k - 1]).ToList();
						break;
				}
			}
		}
		public static List<string> getLstStringFromColumnName(string columName, List<CRSelectFunctionCalculateValue> lstCR)
		{
			List<string> lstString = new List<string>();
			switch (columName)
			{
				case "endpoint":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).Distinct().ToList();
					break;
				case "author":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author).Distinct().ToList();
					break;
				case "qualifier":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).Distinct().ToList();
					break;
				case "location":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.strLocations).Distinct().ToList();
					break;
				case "startage":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.StartAge.ToString()).Distinct().ToList();
					break;
				case "endage":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndAge.ToString()).Distinct().ToList();
					break;
				case "year":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString()).Distinct().ToList();
					break;
				case "otherpollutants":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).Distinct().ToList();
					break;
				case "race":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Race).Distinct().ToList();
					break;
				case "ethnicity":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Ethnicity).Distinct().ToList();
					break;
				case "gender":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Gender).Distinct().ToList();
					break;
				case "function":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function).Distinct().ToList();
					break;
				case "pollutant":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).Distinct().ToList();
					break;
				case "metric":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).Distinct().ToList();
					break;
				case "seasonalmetric":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName).Distinct().ToList();
					break;
				case "metricstatistic":
					lstString = lstCR.Select(p => Enum.GetName(typeof(MetricStatic), p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic)).Distinct().ToList();
					break;
				case "dataset":
					lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).Distinct().ToList();
					break;
			}

			return lstString;
		}
		public static List<AllSelectCRFunction> getLstAllSelectCRFunction(List<CRSelectFunctionCalculateValue> lstCR, List<string> lstOLVColumns, string EndPointGroup)
		{
			try
			{
				List<AllSelectCRFunction> lstReturn = new List<AllSelectCRFunction>();
				if (lstCR == null) return null;
				if (lstCR.Count == 1)
				{
					lstReturn.Add(new AllSelectCRFunction()
					{
						CRID = lstCR.First().CRSelectFunction.CRID,
						CRSelectFunctionCalculateValue = lstCR.First(),
						EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
						EndPointID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointID,
						ID = 0,
						Name = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
						Nickname = "", //YY: new getLstAllSelectCRFunction
						NodeType = 0,
						PID = -1,
						PoolingMethod = "",
						Version = "1",
						AgeRange = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.AgeRange,  //YY: new

						EndPointGroup = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
						EndPoint = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPoint,
						Author = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Author,
						Qualifier = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Qualifier,
						Location = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.strLocations,
						GeographicArea = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName,
						StartAge = lstCR.First().CRSelectFunction.StartAge.ToString(),
						EndAge = lstCR.First().CRSelectFunction.EndAge.ToString(),
						Year = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString(),
						OtherPollutants = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants,
						Race = lstCR.First().CRSelectFunction.Race,
						Ethnicity = lstCR.First().CRSelectFunction.Ethnicity,
						Gender = lstCR.First().CRSelectFunction.Gender,
						Function = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Function,
						Pollutant = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName.ToString(),
						Metric = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName,
						SeasonalMetric = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName,
						MetricStatistic = Enum.GetName(typeof(MetricStatic), lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic),
						DataSet = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.DataSetName,


					});
					return lstReturn;
				}
				else
				{
					lstReturn.Add(new AllSelectCRFunction()
					{
						EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
						ID = 0,
						Name = EndPointGroup,
						Nickname = "", //YY: new getLstAllSelectCRFunction
						EndPointGroup = EndPointGroup,
						NodeType = 0,
						PID = -1,
						PoolingMethod = "", //YY: change to ""
						AgeRange = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.AgeRange,  //YY: new
					});

					List<string> lstColumns = new List<string>();

					for (int i = 0; i < lstOLVColumns.Count; i++)
					{

						List<string> lstString = new List<string>();
						int iParent = 0;




						lstString = getLstStringFromColumnName(lstOLVColumns[i].Replace(" ", "").ToLower(), lstCR); if (lstString.Count() > 0)
						{
							if (i == 0)
							{
								iParent = 0;


								for (int j = 0; j < lstString.Count(); j++)
								{
									lstReturn.Add(new AllSelectCRFunction()
									{
										EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
										ID = j + 1,
										Name = lstString[j],
										Nickname = "", //YY: new getLstAllSelectCRFunction
										NodeType = i + 1,
										PID = 0,
										PoolingMethod = "", //YY: change to ""

									});
								}
							}
							else
							{
								List<AllSelectCRFunction> query = lstReturn.Where(p => p.NodeType == i).ToList();
								if (query.Count() == 0) { i = lstOLVColumns.Count - 1; }
								else
								{
									for (int j = 0; j < query.Count(); j++)
									{
										List<string> lstParent = new List<string>();
										lstParent.Add(query[j].Name);
										var parent = lstReturn.Where(p => p.ID == query[j].PID).First();
										lstParent.Add(parent.Name);
										for (int k = i; k > 1; k--)
										{
											parent = lstReturn.Where(p => p.ID == parent.PID).First();
											lstParent.Add(parent.Name);

										}
										List<CRSelectFunctionCalculateValue> lstSecond = lstCR;
										getSecond(ref lstSecond, lstOLVColumns, i, lstParent);
										if (lstSecond.Count() > 1)
										{
											lstString = getLstStringFromColumnName(lstOLVColumns[i].Replace(" ", "").ToLower(), lstSecond); if (lstString.Count > 1)
											{
												for (int k = 0; k < lstString.Count(); k++)
												{
													lstReturn.Add(new AllSelectCRFunction()
													{
														EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
														ID = lstReturn.Count(),
														Name = lstString[k],
														Nickname = "", //YY: new getLstAllSelectCRFunction
														NodeType = i + 1,
														PID = query[j].ID,
														PoolingMethod = "", //YY: change to ""

													});
												}
											}
										}
									}
								}
							}


						}
						else
						{

							i = lstOLVColumns.Count - 1;
						}



					}
					var queryTree = lstReturn.Where(p => lstReturn.Where(a => a.PID == p.ID).Count() == 0).ToList();
					foreach (AllSelectCRFunction acf in queryTree)
					{
						List<string> lstParent = new List<string>();
						lstParent.Add(acf.Name);
						if (lstReturn.Where(p => p.ID == acf.PID).Count() > 0)
						{
							var parent = lstReturn.Where(p => p.ID == acf.PID).First();
							lstParent.Add(parent.Name);
							for (int k = acf.NodeType; k > 1; k--)
							{
								parent = lstReturn.Where(p => p.ID == parent.PID).First();
								lstParent.Add(parent.Name);

							}
						}
						List<CRSelectFunctionCalculateValue> lstSecond = lstCR;
						getSecond(ref lstSecond, lstOLVColumns, acf.NodeType, lstParent);
						foreach (CRSelectFunctionCalculateValue crc in lstSecond)
						{
							string strAuthor = crc.CRSelectFunction.BenMAPHealthImpactFunction.Author;
							if (strAuthor != null && strAuthor != "" && strAuthor.Contains(" "))
							{
								strAuthor = strAuthor.Substring(0, strAuthor.IndexOf(" "));
							}
							lstReturn.Add(new AllSelectCRFunction()
							{
								CRID = crc.CRSelectFunction.CRID,
								CRSelectFunctionCalculateValue = crc,
								EndPointGroupID = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
								EndPointID = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPointID,
								ID = lstReturn.Count(),
								Name = strAuthor,
								Nickname = "", //YY: new getLstAllSelectCRFunction
								NodeType = 100,
								PID = acf.ID,
								PoolingMethod = "",
								Version = "1",
								EndPointGroup = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
								EndPoint = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint,
								Author = crc.CRSelectFunction.BenMAPHealthImpactFunction.Author,
								Qualifier = crc.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier,
								Location = crc.CRSelectFunction.BenMAPHealthImpactFunction.strLocations,
								GeographicArea = crc.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName,
								StartAge = crc.CRSelectFunction.StartAge.ToString(),
								EndAge = crc.CRSelectFunction.EndAge.ToString(),
								Year = crc.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString(),
								OtherPollutants = crc.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants,
								Race = crc.CRSelectFunction.Race,
								Ethnicity = crc.CRSelectFunction.Ethnicity,
								Gender = crc.CRSelectFunction.Gender,
								Function = crc.CRSelectFunction.BenMAPHealthImpactFunction.Function,
								Pollutant = crc.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName,
								Metric = crc.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName,
								SeasonalMetric = crc.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null ? "" : crc.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName,
								MetricStatistic = Enum.GetName(typeof(MetricStatic), crc.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic),
								DataSet = crc.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName,
								AgeRange = crc.CRSelectFunction.BenMAPHealthImpactFunction.AgeRange,  //YY: new

							});
						}

					}


				}
				return lstReturn;
			}
			catch
			{
				return null;
			}
		}
		private void tabControlSelection_DrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				Font fntTab;
				Brush bshBack;
				Brush bshFore;
				if (e.Index == this.tabControlSelection.SelectedIndex)
				{
					fntTab = new Font(e.Font, FontStyle.Bold);
					bshBack = new SolidBrush(Color.White);
					bshFore = Brushes.Black;
				}
				else
				{
					fntTab = e.Font;
					bshBack = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
					bshFore = new SolidBrush(Color.Black);
				}
				string tabName = this.tabControlSelection.TabPages[e.Index].Text;
				StringFormat sftTab = new StringFormat();
				e.Graphics.FillRectangle(bshBack, e.Bounds);
				Rectangle recTab = e.Bounds;
				recTab = new Rectangle(recTab.X, recTab.Y + 4, recTab.Width, recTab.Height - 4);
				e.Graphics.DrawString(tabName, fntTab, bshFore, recTab, sftTab);
			}

			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnShowChanges_Click(object sender, EventArgs e)
		{
			try
			{
				ChangedCRFunctions crfunctions = new ChangedCRFunctions();
				crfunctions.Show();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void treeListView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 46)
			{
				btDelSelection_Click(sender, e);
			}
		}

		private void TreeListView_FormatCell(object sender, FormatCellEventArgs e)
		{
			if (e.Column.Text == "Studies, By Endpoint")//&& (string)e.CellValue != ""
			{
				//YY: If node has no (non VFunction) children and not a valuation function node, add an orange border.
				ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
				if (vb.LstAllSelectValuationMethod == null) return;
				AllSelectValuationMethod asvm = (AllSelectValuationMethod)e.Model;
				var query = vb.LstAllSelectValuationMethod.Where(p => p.PID == asvm.ID && p.NodeType != 2000).ToList();
				if (query.Count() == 0 && asvm.NodeType != 2000)
				{
					CellBorderDecoration cbd = new CellBorderDecoration();
					cbd.BorderPen = new Pen(Color.Orange);
					cbd.CornerRounding = 0.0f;
					cbd.FillBrush = null;
					cbd.BoundsPadding = new Size(0, -1);
					e.SubItem.Decorations.Add(cbd);
				}

				//YY: hilight direct children of selected record 
				if (treeListView.SelectedObjects.Count > 0)
				{
					//AllSelectValuationMethod cr = (AllSelectValuationMethod)treeListView.SelectedObjects[0];
					//if (asvm.PID == cr.ID)
					//{
					//    e.Item.BackColor = Color.LightGreen;
					//}

					if (e.Column.Text == "Studies, By Endpoint" || e.Column.Text == "Pooling Method" || e.Column.Text == "Weight" || e.Column.Text == "Nickname" || e.Column.Text == "No. of Studies")
					{
						if (treeListView.SelectedObjects.Count > 0)
						{
							AllSelectValuationMethod cr = (AllSelectValuationMethod)treeListView.SelectedObjects[0];
							List<AllSelectValuationMethod> lstAll = vb.LstAllSelectValuationMethod;
							List<AllSelectValuationMethod> lstGreen = new List<AllSelectValuationMethod>();
							getAllChildMethodNotNone(cr, lstAll, ref lstGreen);
							if (lstGreen.Contains(asvm))
							{
								e.SubItem.BackColor = Color.LightGreen;
								//if (e.Column.Text == "Weight")
								//{
								//    CellBorderDecoration cbd = new CellBorderDecoration();
								//    cbd.BorderPen = new Pen(Color.LightGray);
								//    cbd.FillBrush = null;
								//    cbd.BoundsPadding = new Size(0, -1);
								//    cbd.CornerRounding = 0.0f;
								//    e.SubItem.Decorations.Add(cbd);
								//}
							}
						}
					}
				}
			}
			else if (e.Column.Text == "Pooling Method" && (string)e.CellValue != "")
			{
				//YY: Add rectangle around Pooling method.
				if (1 == 1) //(string)e.CellValue != "" || (string)e.CellValue == ""
				{
					AllSelectValuationMethod asvm = (AllSelectValuationMethod)e.Model;
					ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
					List<AllSelectValuationMethod> lstChildValuationNode = new List<AllSelectValuationMethod>();
					getAllChildMethodNotNone(asvm, vb.LstAllSelectValuationMethod, ref lstChildValuationNode);

					//TextDecoration decoration = new TextDecoration("None",ContentAlignment.MiddleLeft);
					CellBorderDecoration cbd = new CellBorderDecoration();
					if (asvm.NodeType != 2000 && lstChildValuationNode.Count() > 1)
					{
						cbd.BorderPen = new Pen(Color.LightGray);
						e.SubItem.ForeColor = Color.Black;
						cbd.FillBrush = null;
						cbd.BoundsPadding = new Size(0, -1);
						cbd.CornerRounding = 0.0f;
						e.SubItem.Decorations.Add(cbd);

						Image imgDD = global::BenMAP.Properties.Resources.dropdown_hint;

						e.SubItem.Decorations.Add(new ImageDecoration(imgDD, ContentAlignment.MiddleRight));
					}


				}


			}
			else if (e.Column.Text == "Weight")
			{
				AllSelectValuationMethod asvm = (AllSelectValuationMethod)e.Item.RowObject;
				if ((asvm.PoolingMethod != "None" && asvm.PoolingMethod != "") || (asvm.NodeType == 2000))
				{
					List<AllSelectValuationMethod> lstParent = new List<AllSelectValuationMethod>();
					//List<AllSelectCRFunction> lstChildren = new List<AllSelectCRFunction>();
					getParentNotNone(asvm, lstParent);
					//lstChildren = getChildFromAllSelectCRFunction(avsm);
					if (lstParent.Where(p => p.PoolingMethod == "User Defined Weights").Count() > 0)//&& lstChildren.Count() == 0
					{
						CellBorderDecoration cbd = new CellBorderDecoration();
						cbd.BorderPen = new Pen(Color.LightGray);
						cbd.FillBrush = null;
						cbd.BoundsPadding = new Size(0, -1);
						cbd.CornerRounding = 0.0f;
						e.SubItem.Decorations.Add(cbd);
					}
				}
			}
		}

		private void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValuationMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
		{

			//YY: Ccode is same as the one in IncidencePoolingandAggregation.cs. Get all children (either pooled group or indivisual valuation function.)
			//Note that this function is different from the one in APVCommonClass.cs That one seems only used for valuation. It's mainly used to highlight pooled valuation functions
			List<AllSelectValuationMethod> lstDirectChildren = lstAll.Where(p => p.PID == allSelectValuationMethod.ID).ToList();
			foreach (AllSelectValuationMethod asvm in lstDirectChildren)
			{
				if ((asvm.PoolingMethod != "None" && asvm.PoolingMethod != "") || asvm.NodeType == 2000) //if (asvm.PoolingMethod != "None" && asvm.ChildCount != 1)
				{
					lstReturn.Add(asvm);
				}
				else //direct child is the a final node, look further
				{
					getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);
				}
			}

		}

		void txt_TextChanged(object sender, EventArgs e)
		{
			try
			{
				TextBox txt = (TextBox)sender;
				List<double> list = new List<double>();
				AllSelectValuationMethod txttag = (AllSelectValuationMethod)txt.Tag;
				if (Convert.ToDouble(txt.Text) >= 0 && Convert.ToDouble(txt.Text) < 1)
					txttag.Weight = Math.Round(Convert.ToDouble(txt.Text), 2);

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}

		}

		private void getParentNotNone(AllSelectValuationMethod asvm, List<AllSelectValuationMethod> lstReturn)
		{
			ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
			if (vb.LstAllSelectValuationMethod == null)
			{
				return;
			}
			var query = vb.LstAllSelectValuationMethod.Where(p => p.ID == asvm.PID);
			if (query != null && query.Count() > 0)
			{
				lstReturn.Add(query.First());
				if (query.First().PoolingMethod == "None" || query.First().PoolingMethod == "")
				{
					//getParent(query.First(), lstReturn);
					getParentNotNone(query.First(), lstReturn); //YY:
				}
			}

		}
	}



	public class ValuationDropSink : SimpleDropSink
	{
		public SelectValuationMethods mySelectValuationMethods;
		public ValuationDropSink()
		{
			this.CanDropBetween = true;
			this.CanDropOnBackground = true;
			this.CanDropOnItem = false;
		}

		public ValuationDropSink(bool acceptDropsFromOtherLists, SelectValuationMethods selectValuationMethods)
				: this()
		{
			this.AcceptExternal = acceptDropsFromOtherLists;
			mySelectValuationMethods = selectValuationMethods;
		}

		protected override void OnModelCanDrop(ModelDropEventArgs args)
		{
			base.OnModelCanDrop(args);

			if (args.Handled)
				return;

			args.Effect = DragDropEffects.Move;

			if (!this.AcceptExternal && args.SourceListView != this.ListView)
			{
				args.Effect = DragDropEffects.None;
				args.DropTargetLocation = DropTargetLocation.None;
				args.InfoMessage = "This list doesn't accept drops from other lists";
			}

			if (args.DropTargetLocation == DropTargetLocation.Background && args.SourceListView == this.ListView)
			{
				args.Effect = DragDropEffects.None;
				args.DropTargetLocation = DropTargetLocation.None;
			}
		}

		protected override void OnModelDropped(ModelDropEventArgs args)
		{

			if (!args.Handled)
				this.RearrangeModels(args);
		}

		public virtual void RearrangeModels(ModelDropEventArgs args)
		{
			int iTop = Convert.ToInt32(this.ListView.TopItemIndex.ToString());
			switch (args.DropTargetLocation)
			{
				case DropTargetLocation.AboveItem:
					if (this.ListView.Name == "treeListView" && args.SourceListView.Name != "treeListView")
					{
						List<BenMAPValuationFunction> lst = new List<BenMAPValuationFunction>();

						foreach (BenMAPValuationFunction bm in args.SourceModels)
						{
							lst.Add(bm);
						}

						mySelectValuationMethods.updateDrop(lst, args.TargetModel as AllSelectValuationMethod, args.DropTargetIndex, true);
						this.ListView.TopItemIndex = iTop;
					}
					else
					{


					}
					break;
				case DropTargetLocation.BelowItem:
					if (this.ListView.Name == "treeListView" && args.SourceListView.Name != "treeListView")
					{
						List<BenMAPValuationFunction> lst = new List<BenMAPValuationFunction>();
						foreach (BenMAPValuationFunction bm in args.SourceModels)
						{
							lst.Add(bm);
						}
						mySelectValuationMethods.updateDrop(lst, args.TargetModel as AllSelectValuationMethod, args.DropTargetIndex, false);
						this.ListView.TopItemIndex = iTop;
					}
					else
					{

					}
					break;
				case DropTargetLocation.Background:
					if (this.ListView.Name == "treeListView")
					{

					}
					else
					{

					}
					break;
				default:
					return;
			}

			if (args.SourceListView != this.ListView && args.SourceListView.Name == "treeListView")
			{
				args.SourceListView.RemoveObjects(args.SourceModels);
			}
		}
	}

}
