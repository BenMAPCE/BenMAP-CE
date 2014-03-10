using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace BenMAP.APVX
{

    public partial class SelectValuationWeight : FormBase
    {
        public List<AllSelectValuationMethod> lstAllSelectValuationMethod = null;
        public SelectValuationWeight()
        {
            InitializeComponent();
        }
        private void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValueMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
        {
            List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID).ToList(); lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 2000).ToList());
            foreach (AllSelectValuationMethod asvm in lstOne.Where(p => p.PoolingMethod == "None"))
            {
                getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);

            }
        }


        private List<AllSelectValuationMethod> getChildFromAllSelectValuationMethod(AllSelectValuationMethod allSelectValuationMethod)
        {
            List<AllSelectValuationMethod> lstAll = new List<AllSelectValuationMethod>();
            var query = from a in lstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
            lstAll = query.ToList();
            return lstAll;

        }

        private void treeListView_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (e.Column.Text == "Weight")
            {

                updateTreeView();
                e.Cancel = true;
            }
        }
        private void updateTreeView()
        {
            treeListView.RebuildAll(true);
            treeListView.ExpandAll();

        }
        private void getAllParent(AllSelectValuationMethod allSelectCRFunction, List<AllSelectValuationMethod> lstReturn)
        {

            var query = lstAllSelectValuationMethod.Where(p => p.ID == allSelectCRFunction.PID);
            if (query != null && query.Count() > 0)
            {
                lstReturn.Add(query.First());
                getAllParent(query.First(), lstReturn);
            }

        }
        private void treeListView_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            base.OnClick(e);
            if (e.Column == null) return;
            if (e.Column.Text == "Weight")
            {
                TextBox txt = new TextBox();
                txt.Bounds = e.CellBounds;
                txt.Font = ((ObjectListView)sender).Font;
                AllSelectValuationMethod asm = (AllSelectValuationMethod)e.RowObject;
                if (e.Value != null && asm.PoolingMethod != "None")
                {
                    List<AllSelectValuationMethod> lstParent = new List<AllSelectValuationMethod>();
                    getAllParent(asm, lstParent);
                    if (lstParent.Where(p => p.PoolingMethod == "User Defined Weights").Count() > 0)
                    {
                        txt.Text = e.Value.ToString();
                        txt.TextChanged += new EventHandler(txt_TextChanged);
                        txt.Tag = e.RowObject;
                        e.Control = txt;
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
        }
        void txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)sender;
                List<double> list = new List<double>();
                AllSelectValuationMethod allSelectValuation = (AllSelectValuationMethod)txt.Tag;
                if (Convert.ToDouble(txt.Text) >= 0 && Convert.ToDouble(txt.Text) < 1)
                {
                    this.lstAllSelectValuationMethod.Where(p => p.ID == allSelectValuation.ID).First().Weight = Convert.ToDouble(txt.Text);
                    allSelectValuation.Weight = Math.Round(Convert.ToDouble(txt.Text), 2);
                }

            }
            catch (Exception ex)
            {

                Logger.LogError(ex);
            }

        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void SelectValuationWeight_Load(object sender, EventArgs e)
        {
            double d = 0;
            foreach (AllSelectValuationMethod allSelectValuationMethod in lstAllSelectValuationMethod)
            {
                allSelectValuationMethod.Weight = 0;
            }
            foreach (AllSelectValuationMethod allSelectValuationMethod in lstAllSelectValuationMethod)
            {
                if (allSelectValuationMethod.PoolingMethod == "User Defined Weights")
                {
                    List<AllSelectValuationMethod> lst = new List<AllSelectValuationMethod>();
                    getAllChildMethodNotNone(allSelectValuationMethod, lstAllSelectValuationMethod, ref lst);
                    d = 0;
                    if (lst.Count > 0)
                    {
                        d = Math.Round(Convert.ToDouble(1.000 / Convert.ToDouble(lst.Count)), 2);
                        for (int i = 0; i < lst.Count; i++)
                        {
                            lst[i].Weight = d;
                        }
                    }
                }
                else if (allSelectValuationMethod.PoolingMethod == "None") allSelectValuationMethod.Weight = 0;

            }
            List<AllSelectValuationMethod> lstRoot = new List<AllSelectValuationMethod>();

            lstRoot = new List<AllSelectValuationMethod>();
            lstRoot.Add(lstAllSelectValuationMethod.First());
            for (int iRoot = 1; iRoot < lstAllSelectValuationMethod.Count; iRoot++)
            {
                if (lstRoot.Where(p => p.EndPointGroup == lstAllSelectValuationMethod[iRoot].EndPointGroup).Count() == 0
                   )
                    lstRoot.Add(lstAllSelectValuationMethod[iRoot]);
            }

            treeListView.Roots = lstRoot;
            this.treeListView.CanExpandGetter = delegate(object x)
            {
                AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
                return (dir.NodeType != 2000);
            };
            this.treeListView.ChildrenGetter = delegate(object x)
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

            treeListView.RebuildAll(true);

            treeListView.ExpandAll();
        }
    }
}
