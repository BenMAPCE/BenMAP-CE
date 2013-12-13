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
        public List<AllSelectValuationMethod> lstAllSelectValuationMethod=null;
        public SelectValuationWeight()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectValueMethod"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        private void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValueMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
        {
            List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID).ToList();// && (p.PoolingMethod != "None" || p.NodeType == 3)).ToList();
            lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 2000).ToList());
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
                //((TextBox)e.Control).TextChanged -= new EventHandler(txt_TextChanged);

                //((ObjectListView)sender).RefreshObject(e.Column);
                //olvSelectWeights.SetObjects(dicAllWeight);
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
                //if(lst.Count()>0)
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
                //if (Convert.ToDouble(e.Value.ToString()) >= 0 && Convert.ToDouble(e.Value.ToString()) <= 1)
                //{
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
                    //if (e.Value != null)
                    //    txt.Text = e.Value.ToString();
                    //txt.TextChanged += new EventHandler(txt_TextChanged);
                    //txt.Tag = e.RowObject;
                    //e.Control = txt;
                }
                else
                {
                    e.Cancel = true;
                }
                //}
                //else
                //{
                //    MessageBox.Show("The value of Weight cann't greater 1.00 or no negative.");
                //}
            }
        }
        void txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)sender;
                //((KeyValuePair<CRSelectFunctionCalculateValue,double>)txt.Tag).Value =Convert.ToDouble( txt.Text);
                List<double> list = new List<double>();
                //KeyValuePair<CRSelectFunctionCalculateValue, double> txttag = (KeyValuePair<CRSelectFunctionCalculateValue, double>)txt.Tag;
                //dicAllWeight[txttag.Key] = Convert.ToDouble(txt.Text);
                AllSelectValuationMethod allSelectValuation = (AllSelectValuationMethod)txt.Tag;
                if (Convert.ToDouble(txt.Text) >= 0 && Convert.ToDouble(txt.Text) < 1)
                {
                    this.lstAllSelectValuationMethod.Where(p => p.ID == allSelectValuation.ID).First().Weight = Convert.ToDouble(txt.Text);
                    allSelectValuation.Weight = Math.Round(Convert.ToDouble(txt.Text), 2);
                }
                //txt.Text = allSelectValuation.Weight.ToString();
                //txttag.Value = Convert.ToDouble(txt.Text);
                // olvweight.PutValue(olvweight, txt.Text);
                //list.Add(Convert.ToDouble(txt.Text));
                //CommonClass.IncidencePoolingAndAggregation.Weights=list;

            }
            catch (Exception ex)
            {
                //TextBox txt = (TextBox)sender;
                //AllSelectValuationMethod allSelectQALY = (AllSelectValuationMethod)txt.Tag;
                //txt.Text = allSelectQALY.Weight.ToString();

                Logger.LogError(ex);
            }

            //dicAllWeight.Add()
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
            //--------------------如果都为0 则进行归化------所有child的值为1/n
            //bool isGH = false;
            double d = 0;
            //if (lstAllSelectValuationMethod.Select(p => p.Weight).Sum() == 0)
            //{
            //    isGH = true;
            //}
            //if (isGH)
            //{
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
                        // lst=lst.Where(p=>p.NodeType==5 ||(p.PoolingMethod!="None" &&p.NodeType!=5)).ToList();
                        d = 0;
                        if (lst.Count > 0)
                        {
                            d = Math.Round(Convert.ToDouble(1.000 / Convert.ToDouble(lst.Count)), 2);
                            for (int i = 0; i < lst.Count; i++)
                            {
                                lst[i].Weight = d;
                            }
                            // lst.Last().Weight = 1 - (d * (lst.Count - 1));
                        }
                    }
                    else if (allSelectValuationMethod.PoolingMethod == "None") allSelectValuationMethod.Weight = 0;

                }
            //}
            List<AllSelectValuationMethod> lstRoot = new List<AllSelectValuationMethod>();

            //lstRoot.Add(new AllSelectQALYMethod()
            //{
            //    Name = CommonClass.IncidencePoolingAndAggregation.PoolingMethods.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup + "/All/All",
            //    NodeType = 4,
            //    ID = 1,
            //    PID = 0,
            //    PoolingMethod = Enum.GetName(typeof(PoolingMethodTypeEnum), PoolingMethodTypeEnum.None)
            //});
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
                    //return new ArrayList();
                }
            };

            treeListView.RebuildAll(true);

            treeListView.ExpandAll();
        }
    }
}
