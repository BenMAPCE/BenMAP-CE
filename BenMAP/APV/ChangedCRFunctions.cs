using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using BrightIdeasSoftware;
using System.Diagnostics;

namespace BenMAP.APVX
{
    public partial class ChangedCRFunctions : FormBase
    {
        public ChangedCRFunctions()
        {
            InitializeComponent();
        }

        private void cbDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cbEndPointGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void textBoxFilterSimple_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.olvcDataSet.ValuesChosenForFiltering.Clear();
                this.olvcEndPointGroup.ValuesChosenForFiltering.Clear();
                this.TimedFilter(this.olvAvailable, textBoxFilterSimple.Text);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            olv.ModelFilter = filter;
            stopWatch.Stop();
            IList objects = olv.Objects as IList;
        }


        private void cbGroups_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ShowGroupsChecked(this.olvAvailable, (CheckBox)sender);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cbView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.ChangeView(this.olvAvailable, (ComboBox)sender);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void ChangeView(ObjectListView listview, ComboBox comboBox)
        {
            try
            {
                if (comboBox.SelectedIndex == 0)
                {
                    if (listview.VirtualMode)
                    {
                        MessageBox.Show("Sorry, Virtual lists can't use Tile view under Microsoft framework.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (listview.CheckBoxes)
                    {
                        MessageBox.Show("Tile view can't have checkboxes under Microsoft framework., so CheckBoxes have been turned off.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void ChangedCRFunctions_Load(object sender, EventArgs e)
        {
            try
            {
                if (CommonClass.LstUpdateCRFunction != null && CommonClass.LstUpdateCRFunction.Count > 0)
                {
                    LoadAddandDelete(CommonClass.LstUpdateCRFunction, this.olvAvailable, this.cbDataSet, this.cbEndPointGroup, this.cbView, false);
                }
                if (CommonClass.LstDelCRFunction != null && CommonClass.LstDelCRFunction.Count > 0)
                {
                    LoadAddandDelete(CommonClass.LstDelCRFunction, this.blvDelCRFunctions, this.cboDelDataSet, this.cboDelEndPointGroup, this.cboDelView, true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private bool LoadAddandDelete(List<CRSelectFunction> lstCRFunction, ObjectListView blv, ComboBox cboDataSet, ComboBox cboEndPointGroup, ComboBox cboView, bool isDel)
        {
            try
            {
                blv.SetObjects(lstCRFunction);
                TypedObjectListView<CRSelectFunction> tlist = new TypedObjectListView<CRSelectFunction>(blv);
                tlist.GenerateAspectGetters();
                blv.TileSize = new Size(120, 90);
                Tools.IncidenceBusinessCardRenderer blvRender = new Tools.IncidenceBusinessCardRenderer();
                blv.ItemRenderer = blvRender; blv.OwnerDraw = true;
                cboView.SelectedIndex = 0;
                List<CRSelectFunction> lstAvailable = (List<CRSelectFunction>)blv.Objects;
                Dictionary<string, int> DicFilterDataSet = new Dictionary<string, int>();
                DicFilterDataSet.Add("", -1);
                var query = from a in lstAvailable select new { a.BenMAPHealthImpactFunction.DataSetName, a.BenMAPHealthImpactFunction.DataSetID };
                if (query != null && query.Count() > 0)
                {
                    List<KeyValuePair<string, int>> lstFilterDataSet = DicFilterDataSet.ToList();
                    lstFilterDataSet.AddRange(query.Distinct().ToDictionary(p => p.DataSetName, p => p.DataSetID));
                    DicFilterDataSet = lstFilterDataSet.ToDictionary(p => p.Key, p => p.Value);
                }
                BindingSource bs = new BindingSource();
                bs.DataSource = DicFilterDataSet;
                cboDataSet.DataSource = bs;
                cboDataSet.DisplayMember = "Key";
                cboDataSet.ValueMember = "Value";
                Dictionary<string, int> DicFilterGroup = new Dictionary<string, int>();
                DicFilterGroup.Add("", -1);
                var queryGroup = from a in lstAvailable select new { a.BenMAPHealthImpactFunction.EndPointGroup, a.BenMAPHealthImpactFunction.EndPointGroupID };
                if (queryGroup != null && queryGroup.Count() > 0)
                {
                    List<KeyValuePair<string, int>> lstGroup = DicFilterGroup.ToList();
                    lstGroup.AddRange(queryGroup.Distinct().ToDictionary(p => p.EndPointGroup, p => p.EndPointGroupID));
                    DicFilterGroup = lstGroup.ToDictionary(p => p.Key, p => p.Value);
                }
                BindingSource bsqueryGroup = new BindingSource();

                bsqueryGroup.DataSource = DicFilterGroup;
                cboEndPointGroup.DataSource = bsqueryGroup;
                cboEndPointGroup.DisplayMember = "Key";
                cboEndPointGroup.ValueMember = "Value";
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btTileSet_Click(object sender, EventArgs e)
        {
            try
            {
                APVX.TileSet tileSet = new APVX.TileSet();
                tileSet.olv = this.olvAvailable;
                DialogResult dr = tileSet.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return;
                int count = 0;
                for (int i = 0; i < this.olvAvailable.AllColumns.Count; i++)
                {
                    if ((this.olvAvailable.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsTileViewColumn)
                    {
                        count++;
                        (this.olvAvailable.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = true;
                    }
                    else
                    {
                        (this.olvAvailable.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = false;
                    }
                }
                this.olvAvailable.TileSize = new Size(120, Convert.ToInt32(count * 15.2 + 10));
                this.olvAvailable.RebuildColumns();
                this.olvAvailable.Refresh();
            }
            catch
            {
            }
        }

        private void cboDelDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (blvDelCRFunctions == null || blvDelCRFunctions.IsDisposed)
                    return;
                OLVColumn column = blvDelCRFunctions.GetColumn("colDataSet");
                ArrayList chosenValues = new ArrayList();
                KeyValuePair<string, int> kvp = (KeyValuePair<string, int>)cboDelDataSet.SelectedItem;
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    chosenValues.Add(kvp.Key);
                    colDataSet.ValuesChosenForFiltering = chosenValues;
                    blvDelCRFunctions.UpdateColumnFiltering();
                }
                else
                {
                    colDataSet.ValuesChosenForFiltering.Clear();
                    blvDelCRFunctions.UpdateColumnFiltering();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboDelEndPointGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ObjectListView olv = blvDelCRFunctions;
                if (olv == null || olv.IsDisposed)
                    return;
                OLVColumn column = olv.GetColumn("colEndPointGroup");
                ArrayList chosenValues = new ArrayList();
                KeyValuePair<string, int> kvp = (KeyValuePair<string, int>)cboDelEndPointGroup.SelectedItem;
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    chosenValues.Add(kvp.Key);
                    colEndPointGroup.ValuesChosenForFiltering = chosenValues;
                    olv.UpdateColumnFiltering();
                }
                else
                {
                    colEndPointGroup.ValuesChosenForFiltering.Clear();
                    olv.UpdateColumnFiltering();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void textBoxDelFilterSimple_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.TimedFilter(this.blvDelCRFunctions, this.textBoxDelFilterSimple.Text);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboDelView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.ChangeView(this.blvDelCRFunctions, (ComboBox)sender);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnDelTitleSet_Click(object sender, EventArgs e)
        {
            try
            {
                APVX.TileSet tileSet = new APVX.TileSet();
                tileSet.olv = this.blvDelCRFunctions;
                DialogResult dr = tileSet.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return;
                int count = 0;
                for (int i = 0; i < this.blvDelCRFunctions.AllColumns.Count; i++)
                {
                    if ((this.blvDelCRFunctions.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsTileViewColumn)
                    {
                        count++;
                        (this.blvDelCRFunctions.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = true;
                    }
                    else
                    {
                        (this.blvDelCRFunctions.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = false;
                    }
                }
                this.blvDelCRFunctions.TileSize = new Size(120, Convert.ToInt32(count * 15.2 + 10));
                this.blvDelCRFunctions.RebuildColumns();
                this.blvDelCRFunctions.Refresh();
            }
            catch
            {
            }
        }

        private void chkGroup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ShowGroupsChecked(this.blvDelCRFunctions, (CheckBox)sender);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
