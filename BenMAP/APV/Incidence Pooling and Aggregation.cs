using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Collections;

namespace BenMAP
{
    public partial class IncidencePoolingandAggregation : Form
    {
        public IncidencePoolingandAggregation()
        {
            InitializeComponent();
        }
        private List<CRSelectFunctionCalculateValue> lstSelectCRSelectFunctionCalculateValue=new List<CRSelectFunctionCalculateValue>();
        private IncidencePoolingAndAggregationAdvance incidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance();

        private void IncidencePoolingandAggregation_Load(object sender, EventArgs e)
        {
            try
            {
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                {
                    this.olvAvailable.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                    TypedObjectListView<CRSelectFunctionCalculateValue> tlist = new TypedObjectListView<CRSelectFunctionCalculateValue>(this.olvAvailable);
                    tlist.GenerateAspectGetters();
                    this.olvAvailable.TileSize = new Size(300, 130);
                    this.olvAvailable.ItemRenderer = new Tools.BusinessCardRenderer();
                                                                                this.olvAvailable.ItemRenderer = new Tools.BusinessCardRenderer();
                    olvAvailable.OwnerDraw = true;
                }
                else
                {
                    List<CRSelectFunctionCalculateValue> lst = new List<CRSelectFunctionCalculateValue>();
                    CRSelectFunctionCalculateValue cfcv = new CRSelectFunctionCalculateValue();
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(21);
                    lst.Add(cfcv);
                    CRSelectFunctionCalculateValue cfcv2 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv3 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv4 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv5 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv6 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv7 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv8 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv9 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv10 = new CRSelectFunctionCalculateValue();
                    CRSelectFunctionCalculateValue cfcv11 = new CRSelectFunctionCalculateValue();
                    cfcv2.CRSelectFunction = new CRSelectFunction();
                    cfcv2.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(22);
                    lst.Add(cfcv2);
                    cfcv3.CRSelectFunction = new CRSelectFunction();
                    cfcv3.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(40);
                    lst.Add(cfcv3);
                    cfcv4.CRSelectFunction = new CRSelectFunction();
                    cfcv4.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(41);
                    lst.Add(cfcv4);
                    cfcv5.CRSelectFunction = new CRSelectFunction();
                    cfcv5.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(62);
                    lst.Add(cfcv5);
                    cfcv6.CRSelectFunction = new CRSelectFunction();
                    cfcv6.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(63);
                    lst.Add(cfcv6);
                    cfcv7.CRSelectFunction = new CRSelectFunction();
                    cfcv7.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(64);
                    lst.Add(cfcv7);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    cfcv.CRSelectFunction = new CRSelectFunction();
                    cfcv.CRSelectFunction.BenMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(56);
                    lst.Add(cfcv);
                    this.olvAvailable.SetObjects(lst);
                    TypedObjectListView<CRSelectFunctionCalculateValue> tlist = new TypedObjectListView<CRSelectFunctionCalculateValue>(this.olvAvailable);
                    tlist.GenerateAspectGetters();
                    this.olvAvailable.TileSize = new Size(300, 130);
                    this.olvAvailable.ItemRenderer = new Tools.BusinessCardRenderer();
                    olvAvailable.OwnerDraw = true;
                }
                this.olvSelected.CheckBoxes = false;
                List<CRSelectFunctionCalculateValue> lstAvailable = (List<CRSelectFunctionCalculateValue>)this.olvAvailable.Objects;
                
                Dictionary<string, int> DicFilterDataSet = new Dictionary<string, int>();
                DicFilterDataSet.Add("", -1);
                var query = from a in lstAvailable select new { a.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName, a.CRSelectFunction.BenMAPHealthImpactFunction.DataSetID };
                if (query != null && query.Count() > 0)
                {
                    List<KeyValuePair<string, int>> lstFilterDataSet = DicFilterDataSet.ToList();
                    lstFilterDataSet.AddRange(query.Distinct().ToDictionary(p => p.DataSetName, p => p.DataSetID));
                    DicFilterDataSet = lstFilterDataSet.ToDictionary(p => p.Key, p => p.Value);
                                   }
                BindingSource bs = new BindingSource();
               
                               bs.DataSource = DicFilterDataSet;
                this.cbDataSet.DataSource = bs;
                cbDataSet.DisplayMember = "Key";
                cbDataSet.ValueMember = "Value";


                Dictionary<string, int> DicFilterGroup = new Dictionary<string, int>();
                DicFilterGroup.Add("", -1);
                var queryGroup = from a in lstAvailable select new { a.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, a.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID };
                if (queryGroup != null && queryGroup.Count() > 0)
                {
                    List<KeyValuePair<string, int>> lstGroup = DicFilterGroup.ToList();
                    lstGroup.AddRange(queryGroup.Distinct().ToDictionary(p => p.EndPointGroup, p => p.EndPointGroupID));
                    DicFilterGroup = lstGroup.ToDictionary(p=>p.Key,p=>p.Value);
                }
                BindingSource bsqueryGroup = new BindingSource();
                
                               bsqueryGroup.DataSource = DicFilterGroup;
                cbEndPointGroup.DataSource = bsqueryGroup;
                cbEndPointGroup.DisplayMember = "Key";
                cbEndPointGroup.ValueMember = "Value";

                                                this.cboPoolingMethod.DataSource = Enum.GetNames(typeof(PoolingMethodTypeEnum));

                                if (CommonClass.GBenMAPGrid != null)
                {
                    this.txtTargetGridType.Text = CommonClass.GBenMAPGrid.GridDefinitionName;
                }
            }
            catch(Exception ex)
            { 

            }


        }
       
        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            try
            {
                APVConfigurationAdvancedSettings frm = new APVConfigurationAdvancedSettings();
                frm.IncidencePoolingAndAggregationAdvance = incidencePoolingAndAggregationAdvance;
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                
                incidencePoolingAndAggregationAdvance = frm.IncidencePoolingAndAggregationAdvance;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

                                                                                                                                        
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbPoolingWindowName.Text))
            {
                MessageBox.Show("");                return;
            }
            if(olvSelected.Objects ==null)
            {
                MessageBox.Show("");                return;
            }
            List<CRSelectFunctionCalculateValue> lstSelected= olvSelected.Objects as List<CRSelectFunctionCalculateValue>;
            if (CommonClass.IncidencePoolingAndAggregation == null)
            {
                CommonClass.IncidencePoolingAndAggregation = new IncidencePoolingAndAggregation();
            }
            CommonClass.IncidencePoolingAndAggregation.PoolingName = tbPoolingWindowName.Text;
            CommonClass.IncidencePoolingAndAggregationAdvance = incidencePoolingAndAggregationAdvance;
            CommonClass.IncidencePoolingAndAggregation.IncidencePoolingAndAggregationAdvance = incidencePoolingAndAggregationAdvance;
            CommonClass.IncidencePoolingAndAggregation.PoolingMethods = lstSelected;
            CommonClass.IncidencePoolingAndAggregation.PoolingMethodType =(PoolingMethodTypeEnum) cboPoolingMethod.SelectedIndex  ;
            if (CommonClass.IncidencePoolingAndAggregation.PoolingMethodType == PoolingMethodTypeEnum.SubjectiveWeights)
            {
                                SelectSubjectiveWeight frmAPV = new SelectSubjectiveWeight();
                DialogResult rtnAPV = frmAPV.ShowDialog();
                if (rtnAPV != DialogResult.OK) { return; }
                            }
            CommonClass.IncidencePoolingAndAggregation.ConfigurationResultsFilePath = "";
            SelectValuationMethods frm = new SelectValuationMethods();
            DialogResult rtn = frm.ShowDialog();
            if (rtn != DialogResult.OK) { return; }
        }

        private void cbView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbView.SelectedIndex == 0 && this.olvAvailable.OwnerDraw)
                this.olvAvailable.TileSize = new Size(300, 130);

            this.ChangeView(this.olvAvailable, (ComboBox)sender);
        }
        private void ChangeView(ObjectListView listview, ComboBox comboBox)
        {
                        if (comboBox.SelectedIndex == 0)
            {
                if (listview.VirtualMode)
                {
                    MessageBox.Show("Sorry, Microsoft says that virtual lists can't use Tile view.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (listview.CheckBoxes)
                {
                    MessageBox.Show("Microsoft says that Tile view can't have checkboxes, so CheckBoxes have been turned off on this list.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listview.CheckBoxes = false;
                }
            }

            switch (comboBox.SelectedIndex)
            {
                                                                                                                                                                case 0:
                    listview.View = View.Tile;
                    
                    break;
                case 1:
                    listview.View = View.Details;
                    break;
            }
                   }

        private void textBoxFilterSimple_TextChanged(object sender, EventArgs e)
        {
            this.olvcDataSet.ValuesChosenForFiltering.Clear();
            this.olvcEndPointGroup.ValuesChosenForFiltering.Clear();
            this.TimedFilter(this.olvAvailable, textBoxFilterSimple.Text);
        }
        void TimedFilter(ObjectListView olv, string txt)
        {
            this.TimedFilter(olv, txt, 0);
        }

        void TimedFilter(ObjectListView olv, string txt, int matchKind)
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

                                            }

                        HighlightTextRenderer highlightingRenderer = olv.GetColumn(0).Renderer as HighlightTextRenderer;
            if (highlightingRenderer != null)
                highlightingRenderer.Filter = filter;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            olv.ModelFilter = filter;
            stopWatch.Stop();

            IList objects = olv.Objects as IList;
            if (objects == null)
                this.toolStripStatusLabel1.Text =
                    String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            else
                this.toolStripStatusLabel1.Text =
                    String.Format("Filtered {0} items down to {1} items in {2}ms",
                                  objects.Count,
                                  olv.Items.Count,
                                  stopWatch.ElapsedMilliseconds);
        }

        private void cbGroups_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvAvailable, (CheckBox)sender);
        }
        void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
        {
            if (cb.Checked && olv.View == View.List)
            {
                cb.Checked = false;
                MessageBox.Show("ListView's cannot show groups when in List view.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                olv.ShowGroups = cb.Checked;
                olv.BuildList();
            }
        
        }

        private void cbDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
                       ObjectListView olv = olvAvailable;
            if (olv == null || olv.IsDisposed)
                return;
            OLVColumn column = olv.GetColumn("olvcDataSet");

                        ArrayList chosenValues = new ArrayList();
            KeyValuePair<string, int> kvp = (KeyValuePair<string, int>)cbDataSet.SelectedItem;
            if (!string.IsNullOrEmpty(kvp.Key))
            {
                chosenValues.Add(kvp.Key);
                                                                                                                                                olvcDataSet.ValuesChosenForFiltering = chosenValues;

                olv.UpdateColumnFiltering();
            }
            else
            {
                olvcDataSet.ValuesChosenForFiltering.Clear();
                olv.UpdateColumnFiltering();
            }

        }

        private void cbEndPointGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
                        ObjectListView olv = olvAvailable;
            if (olv == null || olv.IsDisposed)
                return;
            OLVColumn column = olv.GetColumn("olvcEndPointGroup");

                        ArrayList chosenValues = new ArrayList();
            KeyValuePair<string, int> kvp = (KeyValuePair<string, int>)cbEndPointGroup.SelectedItem;
            if (!string.IsNullOrEmpty(kvp.Key))
            {
                chosenValues.Add(kvp.Key);
                                                                                                                                                olvcEndPointGroup.ValuesChosenForFiltering = chosenValues;
                olv.UpdateColumnFiltering();
                
            }
            else
            {
                olvcEndPointGroup.ValuesChosenForFiltering.Clear();
                olv.UpdateColumnFiltering();
                 
            }

        }

        private void btAddCRFunctions_Click(object sender, EventArgs e)
        {
            try
            {
                List<CRSelectFunctionCalculateValue> lstAvailable = new List<CRSelectFunctionCalculateValue>();
                foreach (CRSelectFunctionCalculateValue cr in olvAvailable.SelectedObjects)
                {
                    lstAvailable.Add(cr);
                }
                lstSelectCRSelectFunctionCalculateValue = lstSelectCRSelectFunctionCalculateValue.Union(lstAvailable).ToList();
                this.olvSelected.SetObjects(lstSelectCRSelectFunctionCalculateValue);
            }
            catch (Exception ex)
            {
 
            }

        }

        private void btDelSelectMethod_Click(object sender, EventArgs e)
        {
            foreach (CRSelectFunctionCalculateValue cr in olvSelected.SelectedObjects)
            {
                lstSelectCRSelectFunctionCalculateValue.Remove(cr);
 
            }
            this.olvSelected.SetObjects(lstSelectCRSelectFunctionCalculateValue);

        }
    }

  
}
