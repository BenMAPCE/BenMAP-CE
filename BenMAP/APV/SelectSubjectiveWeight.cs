using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace BenMAP
{
    public partial class SelectSubjectiveWeight : FormBase
    {
        private IncidencePoolingAndAggregation currentIP;
        public SelectSubjectiveWeight(IncidencePoolingAndAggregation ip)
        {
            InitializeComponent();
            currentIP = ip;
        }
       public Dictionary<CRSelectFunctionCalculateValue, double> dicAllWeight=new Dictionary<CRSelectFunctionCalculateValue,double>();

        private void SelectSubjectiveWeight_Load(object sender, EventArgs e)
        {
            txtPoolingWindowName.Text = currentIP.PoolingName;
            txtPoolingWindowName.Enabled = false;
            //--------------------如果都为0 则进行归化------所有child的值为1/n
            //bool isGH = false;
            double d = 0;
            //if (currentIP.lstAllSelectCRFuntion.Select(p => p.Weight).Sum() == 0)
            //{
            //    isGH = true;
            //}
            //if (isGH)
            //{
            //foreach (AllSelectCRFunction allSelectCRFunction in currentIP.lstAllSelectCRFuntion)
            //{
            //    allSelectCRFunction.Weight = 0;
            //}
                foreach (AllSelectCRFunction allSelectCRFunction in currentIP.lstAllSelectCRFuntion)
                {
                    if (allSelectCRFunction.PoolingMethod == "User Defined Weights")
                    {
                        List<AllSelectCRFunction> lst = new List<AllSelectCRFunction>();
                        getAllChildMethodNotNone(allSelectCRFunction, currentIP.lstAllSelectCRFuntion, ref lst);
                        // lst = lst.Where(p => p.NodeType == 4 || (p.PoolingMethod != "None" && p.NodeType != 4)).ToList();
                        d = 0;
                        if (lst.Count > 0 && lst.Min(p => p.Weight) == 0)
                        {
                            d = Math.Round(Convert.ToDouble(1.000 / Convert.ToDouble(lst.Count)), 2);
                            for (int i = 0; i < lst.Count; i++)
                            {
                                lst[i].Weight = d;
                            }
                            //lst.Last().Weight = 1 - (d * (lst.Count - 1));
                        }
                    }
                    else if (allSelectCRFunction.PoolingMethod == "None")
                        allSelectCRFunction.Weight = 0;
                    //else if( allSelectCRFunction.PoolingMethod!="")
                    //{
                    //    List<AllSelectCRFunction> lst = new List<AllSelectCRFunction>();
                    //    getAllChildMethodNotNone(allSelectCRFunction, currentIP.lstAllSelectCRFuntion, ref lst);
                    //    // lst = lst.Where(p => p.NodeType == 4 || (p.PoolingMethod != "None" && p.NodeType != 4)).ToList();
                    //    d = 0;
                        
                    //        for (int i = 0; i < lst.Count ; i++)
                    //        {
                    //            lst[i].Weight = d;
                    //        }
                          
                    //}

                }
            //}
            //创建 一个 Dictionary加载
            //currentIP.PoolingMethods--> dicAllWeight
            //----------------------init treelistview events------------------
            //treeListView.Roots = currentIP.lstAllSelectCRFuntion.GetRange(0, 1);
            List<AllSelectCRFunction> lstRoot = new List<AllSelectCRFunction>();
            lstRoot.Add(currentIP.lstAllSelectCRFuntion.First());
            for (int i = 1; i < currentIP.lstAllSelectCRFuntion.Count(); i++)
            {
                if (currentIP.lstAllSelectCRFuntion[i].EndPointGroup != currentIP.lstAllSelectCRFuntion[i - 1].EndPointGroup)
                    lstRoot.Add(currentIP.lstAllSelectCRFuntion[i]);
            }
            treeListView.Roots = lstRoot;// incidencePoolingAndAggregation.lstAllSelectCRFuntion.GetRange(0, 1);
            this.treeListView.CanExpandGetter = delegate(object x)
            {
                AllSelectCRFunction dir = (AllSelectCRFunction)x;
                return (dir.NodeType != 100);
            };
            this.treeListView.ChildrenGetter = delegate(object x)
            {
                AllSelectCRFunction dir = (AllSelectCRFunction)x;
                try
                {
                    return getChildFromAllSelectCRFunction(dir);


                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return new List<AllSelectCRFunction>();
                    //return new ArrayList();
                }
            };
            treeListView.ExpandAll();
            //if (CommonClass.lstIncidencePoolingAndAggregation.First() != null && CommonClass.lstIncidencePoolingAndAggregation.First().lstAllSelectCRFuntion != null)
            //{
            //    //    treeListView.SetObjects(CommonClass.lstIncidencePoolingAndAggregation.First().lstAllSelectCRFuntion);
            //    initTreeView(CommonClass.lstIncidencePoolingAndAggregation.First());
            //}

            


        }
        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectCRFunction"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        private void getAllChildMethodNotNone(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();// && (p.PoolingMethod != "None" || p.NodeType == 3)).ToList();
            lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 100).ToList());
            foreach (AllSelectCRFunction asvm in lstOne.Where(p => p.PoolingMethod == "None" ).ToList())
            {
                getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);

            }
        }


        private List<AllSelectCRFunction> getChildFromAllSelectCRFunction(AllSelectCRFunction allSelectValuationMethod)
        {
            List<AllSelectCRFunction> lstAll = new List<AllSelectCRFunction>();
           // IncidencePoolingAndAggregation incidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[cbPoolingWindow_SelectedIndexOld].Text).First();

            var query = from a in this.currentIP.lstAllSelectCRFuntion where a.PID == allSelectValuationMethod.ID select a;
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
                AllSelectCRFunction acr = (AllSelectCRFunction)e.RowObject;
                if (e.Value != null && acr.PoolingMethod != "None")
                {
                    List<AllSelectCRFunction> lstParent = new List<AllSelectCRFunction>();
                    getAllParent(acr, lstParent);
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
                //}
                //else
                //{
                //    MessageBox.Show("The value of Weight cann't greater 1.00 or no negative.");
                //}
            }
        }

        private void getAllParent(AllSelectCRFunction allSelectCRFunction,List<AllSelectCRFunction> lstReturn )
        {
            
           var query= currentIP.lstAllSelectCRFuntion.Where(p => p.ID == allSelectCRFunction.PID);
           if (query != null && query.Count()>0)
           {
               lstReturn.Add(query.First());
                 getAllParent( query.First(),lstReturn);
               //if(lst.Count()>0)
           }
            
        }
        /// <summary>
        /// 初始化TreeViewList
        /// </summary>
        /// <param name="incidencePoolingAndAggregation"></param>
        private void initTreeView(IncidencePoolingAndAggregation incidencePoolingAndAggregation)
        {


            List<AllSelectCRFunction> lstRoot = new List<AllSelectCRFunction>();
            if (incidencePoolingAndAggregation.lstAllSelectCRFuntion == null)
            {
                incidencePoolingAndAggregation.lstAllSelectCRFuntion = new List<AllSelectCRFunction>();
            }
            else
            {

                treeListView.Roots = incidencePoolingAndAggregation.lstAllSelectCRFuntion.GetRange(0,1);//.Where(p => p.NodeType == 0);
                
                treeListView.RebuildAll(true);
                treeListView.ExpandAll();
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //double d = 0;
            //foreach (double din in dicAllWeight.Values)
            //{
            //    d += din;
            //}
            //for (int i = 0; i < dicAllWeight.Values.Count; i++)
            //{
            //    dicAllWeight[dicAllWeight.Keys.ToArray()[i]] = Math.Round(dicAllWeight.Values.ToArray()[i], 2);
            //}
            ////检查 所有 的 weight之和 -->
            //if (Math.Round(dicAllWeight.Values.Sum(),2) != 1.00)
            //{
            //    MessageBox.Show("The sum of all weights must be 1!");
            //}
            //else
            //返回
            this.DialogResult = DialogResult.OK;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }



        private void SelectSubjectiveWeight_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void olvSelectWeights_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (e.Column.Text == "Weight")
            {
                TextBox  txt= new TextBox();
                txt.Bounds = e.CellBounds;
                txt.Font = ((ObjectListView)sender).Font;
                //if (Convert.ToDouble(e.Value.ToString()) >= 0 && Convert.ToDouble(e.Value.ToString()) <= 1)
                //{
                AllSelectCRFunction acr = (AllSelectCRFunction)e.RowObject;
                if (e.Value != null && acr.PoolingMethod != "None")
                {
                    txt.Text = e.Value.ToString();
                    txt.TextChanged += new EventHandler(txt_TextChanged);
                    txt.Tag = e.RowObject;
                    e.Control = txt;
                }
                else
                {
                    e.Cancel = true ;
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
                AllSelectCRFunction txttag = (AllSelectCRFunction)txt.Tag;
                //if (txt.Text.Last() == '.') txt.Text += "0";
                if (Convert.ToDouble(txt.Text) >= 0 && Convert.ToDouble(txt.Text)<1)
                txttag.Weight = Math.Round(Convert.ToDouble(txt.Text), 2);
                //txt.Text = txttag.Weight.ToString();
                //txttag.Value = Convert.ToDouble(txt.Text);
                // olvweight.PutValue(olvweight, txt.Text);
                //list.Add(Convert.ToDouble(txt.Text));
                //currentIP.Weights=list;
               
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            
            //dicAllWeight.Add()
        }
        //private void olvSelectWeights_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        //{
        //    if (e.Column.Text == "Weight")
        //    {
        //        //((TextBox)e.Control).TextChanged -= new EventHandler(txt_TextChanged);

        //        //((ObjectListView)sender).RefreshObject(e.Column);
        //        olvSelectWeights.SetObjects(dicAllWeight);
        //        e.Cancel = true;
        //    }
        //}
    }
}
