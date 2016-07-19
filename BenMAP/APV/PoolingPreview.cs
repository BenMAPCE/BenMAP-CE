using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Color = Microsoft.Msagl.Drawing.Color;
using Label = Microsoft.Msagl.Drawing.Label;
//using Node = Microsoft.Msagl.Core.Layout.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using DrawingNode = Microsoft.Msagl.Drawing.Node;

namespace BenMAP
{
    public partial class PoolingPreview : Form
    {
        readonly ToolTip toolTip = new ToolTip();
        public PoolingPreview()
        {
            InitializeComponent();
            this.toolTip.Active = true;
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;
        }

        private void PoolingPreview_Load(object sender, EventArgs e)
        {
            try
            {
                gViewer.ObjectUnderMouseCursorChanged += new EventHandler<ObjectUnderMouseCursorChangedEventArgs>(gViewer_ObjectUnderMouseCursorChanged);

                Graph graph = new Graph();
                graph.LayoutAlgorithmSettings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.StraightLine;
                graph.Directed = false;
                graph.Attr.LayerDirection = LayerDirection.LR;
                graph.Attr.AspectRatio = 0.5;
                gViewer.BackColor = System.Drawing.Color.White;

                foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                {

                    // Populate weights for random and fixed effects
                    List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                    if (ip.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                    {
                        APVX.APVCommonClass.getAllChildCRNotNoneForPooling(ip.lstAllSelectCRFuntion.First(), ip.lstAllSelectCRFuntion, ref lstCR);
                    }
                    lstCR.Insert(0, ip.lstAllSelectCRFuntion.First());
                    if (lstCR.Count == 1 && ip.lstAllSelectCRFuntion.First().CRID < 9999 && ip.lstAllSelectCRFuntion.First().CRID > 0) { }
                    else
                    {
                        APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref ip.lstAllSelectCRFuntion, ref ip.lstAllSelectCRFuntion, ip.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), ip.lstColumns);
                    }
                    //Subgraph subgraph = new Subgraph(ip.PoolingName);
                    CreatePoolingPreviewGraph(graph, ip, null, ip.lstAllSelectCRFuntion[0]);
                    //graph.RootSubgraph.AddSubgraph(subgraph);
                }


                gViewer.Graph = graph;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "The pooling preview window cannot be displayed due to an internal error.", "Pooling Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Logger.LogError(ex);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        void CreatePoolingPreviewGraph(Graph graph, IncidencePoolingAndAggregation ip, Node node, AllSelectCRFunction treeEntry)
        {
            Node nodeConnect;
            if(node == null)
            {
                // This is the first call into this recursive function.
                // Render parent/endpoint node and method if applicable
                node = new Node(Guid.NewGuid() + treeEntry.Name);
                if (ip.lstAllSelectCRFuntion.Count == 1)
                {
                    node.LabelText = treeEntry.EndPointGroup;
                }
                else
                {
                    node.LabelText = treeEntry.Name;
                }

                node.Attr.Shape = Shape.Box;
                node.Attr.XRadius = 20;
                node.Attr.YRadius = 20;
                node.Attr.LabelMargin = 10;
                node.Attr.Padding = 200;
                graph.AddNode(node);
            }

            if(ip.lstAllSelectCRFuntion.Count == 1)
            {
                //This is a single study, no pooling scenario. Just render the study and connect to the endpoint
                Node nodeChild = new Node(Guid.NewGuid() + treeEntry.ID.ToString());
                nodeChild.LabelText = treeEntry.Author;

                nodeChild.Attr.Shape = Shape.Box;
                nodeChild.Attr.XRadius = 0;
                nodeChild.Attr.YRadius = 0;
                nodeChild.Attr.LabelMargin = 10;
                nodeChild.UserData = String.Format("{0}\n{1}\n{2}\nAge: {3}-{4}\nRace: {5}\nEthnicity: {6}\nGender: {7}\nYear: {8}", treeEntry.Author, treeEntry.EndPoint, treeEntry.DataSet,
                    treeEntry.StartAge, treeEntry.EndAge, treeEntry.Race,
                    treeEntry.Ethnicity, treeEntry.Gender, treeEntry.Year);
                graph.AddNode(nodeChild);

                Edge edgeChild = new Edge(nodeChild, node, ConnectionToGraph.Connected);
                edgeChild.Attr.ArrowheadAtTarget = ArrowStyle.None;
                edgeChild.Attr.ArrowheadAtSource = ArrowStyle.None;
                return;
            }

            // Add a method node if needed
            if (treeEntry.PoolingMethod != "None" && treeEntry.PoolingMethod != "")
            {
                Node nodeMethod = new Node(Guid.NewGuid() + treeEntry.PoolingMethod + treeEntry.Author);
                nodeMethod.LabelText = treeEntry.PoolingMethod;
                nodeMethod.Attr.Shape = Shape.Diamond;
                nodeMethod.Attr.LabelMargin = 10;
                graph.AddNode(nodeMethod);

                Edge edgeMethod = new Edge(nodeMethod, node, ConnectionToGraph.Connected);
                edgeMethod.Attr.ArrowheadAtTarget = ArrowStyle.None;
                edgeMethod.Attr.ArrowheadAtSource = ArrowStyle.None;
                nodeConnect = nodeMethod;
            }
            else
            {
                nodeConnect = node;
            }

            // Render the children and then make a recursive call to see if any of them are also parents
            List<AllSelectCRFunction> lst = new List<AllSelectCRFunction>();
            getAllChildMethodNotNone(treeEntry, ip.lstAllSelectCRFuntion, ref lst);
            foreach (AllSelectCRFunction treeEntryChild in lst)
            {
                Node nodeChild = new Node(Guid.NewGuid() + treeEntryChild.ID.ToString());

                if (treeEntryChild.PoolingMethod != "None" && treeEntryChild.PoolingMethod != "")
                {
                    nodeChild.LabelText = treeEntryChild.Name + " (Pooled)";
                }
                else
                {
                    nodeChild.LabelText = treeEntryChild.Name;
                }
                nodeChild.Attr.Shape = Shape.Box;
                nodeChild.Attr.XRadius = 0;
                nodeChild.Attr.YRadius = 0;
                nodeChild.Attr.LabelMargin = 10;
                nodeChild.UserData = String.Format("{0}\n{1}\n{2}\nAge: {3}-{4}\nRace: {5}\nEthnicity: {6}\nGender: {7}\nYear: {8}", treeEntryChild.Author, treeEntryChild.EndPoint, treeEntryChild.DataSet, 
                    treeEntryChild.StartAge, treeEntryChild.EndAge, treeEntryChild.Race, 
                    treeEntryChild.Ethnicity, treeEntryChild.Gender, treeEntryChild.Year);
                graph.AddNode(nodeChild);

                Edge edgeChild = new Edge(nodeChild, nodeConnect, ConnectionToGraph.Connected);
                edgeChild.Attr.ArrowheadAtTarget = ArrowStyle.None;
                edgeChild.Attr.ArrowheadAtSource = ArrowStyle.None;
                if (treeEntryChild.Weight != 0)
                {
                    edgeChild.LabelText = String.Format("{0:0.00}", treeEntryChild.Weight);
                }

                CreatePoolingPreviewGraph(graph, ip, nodeChild, treeEntryChild);
            }


        }

        private void getAllChildMethodNotNone(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();
            lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 100).ToList());
            foreach (AllSelectCRFunction asvm in lstOne.Where(p => p.PoolingMethod == "None").ToList())
            {
                getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);

            }
        }

        object selectedObjectAttr;
        object selectedObject;
        void gViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            try
            {
                selectedObject = e.OldObject != null ? e.OldObject.DrawingObject : null;

                if (selectedObject != null)
                {
                    if (selectedObject is Edge)
                        (selectedObject as Edge).Attr = selectedObjectAttr as EdgeAttr;
                    else if (selectedObject is Node)
                        (selectedObject as Node).Attr = selectedObjectAttr as NodeAttr;

                    selectedObject = null;
                }

                if (gViewer.SelectedObject == null)
                {
                    this.gViewer.SetToolTip(toolTip, "");

                }
                else
                {
                    selectedObject = gViewer.SelectedObject;
                    Edge edge = selectedObject as Edge;
                    if (edge != null)
                    {
                        selectedObjectAttr = edge.Attr.Clone();
                    }
                    else if (selectedObject is Node)
                    {
                        Node node = (Node)selectedObject;
                        selectedObjectAttr = (gViewer.SelectedObject as Node).Attr.Clone();

                        if (node.UserData != null)
                        {
                            (selectedObject as Node).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                            this.gViewer.SetToolTip(toolTip, (String)((Node)selectedObject).UserData);
                        }

                    }
                }
                gViewer.Invalidate();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void btClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
