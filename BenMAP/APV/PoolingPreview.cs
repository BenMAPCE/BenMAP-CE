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
        public PoolingPreview()
        {
            InitializeComponent();
        }

        private void PoolingPreview_Load(object sender, EventArgs e)
        {
            CreateGraph();
        }

        void CreateGraph()
        {

            Graph graph = new Graph();

            foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
            {
                Node nodeEnd = new Node(ip.lstAllSelectCRFuntion[0].EndPointGroup);
                nodeEnd.Attr.Shape = Shape.Ellipse;
                graph.AddNode(nodeEnd);

                Node nodeMethod = new Node(ip.lstAllSelectCRFuntion[0].ID.ToString() + ip.lstAllSelectCRFuntion[0].PoolingMethod);
                nodeMethod.LabelText = ip.lstAllSelectCRFuntion[0].PoolingMethod;
                nodeMethod.Attr.Shape = Shape.Diamond;
                graph.AddNode(nodeMethod);
                graph.AddEdge(nodeMethod.Id, nodeEnd.Id);

                List<AllSelectCRFunction> lst = new List<AllSelectCRFunction>();
                getAllChildMethodNotNone(ip.lstAllSelectCRFuntion[0], ip.lstAllSelectCRFuntion, ref lst);
                foreach (AllSelectCRFunction s in lst) {
                    Node nodeStudy = new Node(s.ID.ToString());
                    nodeStudy.LabelText = s.Author;
                    nodeStudy.Attr.Shape = Shape.Box;
                    graph.AddNode(nodeStudy);
                    if (s.Weight != 0)
                    {
                        graph.AddEdge(nodeStudy.Id, s.Weight.ToString(), nodeMethod.Id);
                    }
                    else
                    {
                        graph.AddEdge(nodeStudy.Id, nodeMethod.Id);
                    }

                }
            }
            graph.LayoutAlgorithmSettings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.Rectilinear;
            graph.Directed = false;
            graph.Attr.LayerDirection = LayerDirection.LR;
            gViewer.BackColor = System.Drawing.Color.White;
            gViewer.Graph = graph;

            //            Graph graph = new Graph("graph");
            //            //graph.LayoutAlgorithmSettings=new MdsLayoutSettings();
            //            gViewer.BackColor = System.Drawing.Color.FromArgb(10, System.Drawing.Color.Red);
            //
            //            /*
            //              4->5
            //5->7
            //7->8
            //8->22
            //22->24
            //*/
            //
            //            //int wm = 80;
            //            graph.AddEdge("1", "2");
            //            graph.AddEdge("1", "3");
            //            var e = graph.AddEdge("4", "5");
            //            //e.Attr.Weight *= wm;
            //            e.Attr.Color = Color.Red;
            //            e.Attr.LineWidth *= 2;
            //
            //            e = graph.AddEdge("4", "6");
            //            e.LabelText = "Changing label";
            //            this.labelToChange = e.Label;
            //            e=graph.AddEdge("7", "8");
            //            //e.Attr.Weight *= wm;
            //            e.Attr.LineWidth *= 2;
            //            e.Attr.Color = Color.Red;
            //            
            //            graph.AddEdge("7", "9");
            //            e=graph.AddEdge("5", "7");
            //            //e.Attr.Weight *= wm;
            //            e.Attr.Color = Color.Red;
            //            e.Attr.LineWidth *= 2;
            //            
            //            graph.AddEdge("2", "7");
            //            graph.AddEdge("10", "11");
            //            graph.AddEdge("10", "12");
            //            graph.AddEdge("2", "10");
            //            graph.AddEdge("8", "10");
            //            graph.AddEdge("5", "10");
            //            graph.AddEdge("13", "14");
            //            graph.AddEdge("13", "15");
            //            graph.AddEdge("8", "13");
            //            graph.AddEdge("2", "13");
            //            graph.AddEdge("5", "13");
            //            graph.AddEdge("16", "17");
            //            graph.AddEdge("16", "18");
            //            graph.AddEdge("19", "20");
            //            graph.AddEdge("19", "21");
            //            graph.AddEdge("17", "19");
            //            graph.AddEdge("2", "19");
            //            graph.AddEdge("22", "23");
            //            
            //            e=graph.AddEdge("22", "24");
            //            //e.Attr.Weight *= wm;
            //            e.Attr.Color = Color.Red;
            //            e.Attr.LineWidth *= 2;
            //
            //            e = graph.AddEdge("8", "22");
            //            //e.Attr.Weight *= wm;
            //            e.Attr.Color = Color.Red;
            //            e.Attr.LineWidth *= 2;
            //            
            //            graph.AddEdge("20", "22");
            //            graph.AddEdge("25", "26");
            //            graph.AddEdge("25", "27");
            //            graph.AddEdge("20", "25");
            //            graph.AddEdge("28", "29");
            //            graph.AddEdge("28", "30");
            //            graph.AddEdge("31", "32");
            //            graph.AddEdge("31", "33");
            //            graph.AddEdge("5", "31");
            //            graph.AddEdge("8", "31");
            //            graph.AddEdge("2", "31");
            //            graph.AddEdge("20", "31");
            //            graph.AddEdge("17", "31");
            //            graph.AddEdge("29", "31");
            //            graph.AddEdge("34", "35");
            //            graph.AddEdge("34", "36");
            //            graph.AddEdge("20", "34");
            //            graph.AddEdge("29", "34");
            //            graph.AddEdge("5", "34");
            //            graph.AddEdge("2", "34");
            //            graph.AddEdge("8", "34");
            //            graph.AddEdge("17", "34");
            //            graph.AddEdge("37", "38");
            //            graph.AddEdge("37", "39");
            //            graph.AddEdge("29", "37");
            //            graph.AddEdge("5", "37");
            //            graph.AddEdge("20", "37");
            //            graph.AddEdge("8", "37");
            //            graph.AddEdge("2", "37");
            //            graph.AddEdge("40", "41");
            //            graph.AddEdge("40", "42");
            //            graph.AddEdge("17", "40");
            //            graph.AddEdge("2", "40");
            //            graph.AddEdge("8", "40");
            //            graph.AddEdge("5", "40");
            //            graph.AddEdge("20", "40");
            //            graph.AddEdge("29", "40");
            //            graph.AddEdge("43", "44");
            //            graph.AddEdge("43", "45");
            //            graph.AddEdge("8", "43");
            //            graph.AddEdge("2", "43");
            //            graph.AddEdge("20", "43");
            //            graph.AddEdge("17", "43");
            //            graph.AddEdge("5", "43");
            //            graph.AddEdge("29", "43");
            //            graph.AddEdge("46", "47");
            //            graph.AddEdge("46", "48");
            //            graph.AddEdge("29", "46");
            //            graph.AddEdge("5", "46");
            //            graph.AddEdge("17", "46");
            //            graph.AddEdge("49", "50");
            //            graph.AddEdge("49", "51");
            //            graph.AddEdge("5", "49");
            //            graph.AddEdge("2", "49");
            //            graph.AddEdge("52", "53");
            //            graph.AddEdge("52", "54");
            //            graph.AddEdge("17", "52");
            //            graph.AddEdge("20", "52");
            //            graph.AddEdge("2", "52");
            //            graph.AddEdge("50", "52");
            //            graph.AddEdge("55", "56");
            //            graph.AddEdge("55", "57");
            //            graph.AddEdge("58", "59");
            //            graph.AddEdge("58", "60");
            //            graph.AddEdge("20", "58");
            //            graph.AddEdge("29", "58");
            //            graph.AddEdge("5", "58");
            //            graph.AddEdge("47", "58");
            //
            //            //ChangeNodeSizes(graph);
            //
            //            //var sls = graph.LayoutAlgorithmSettings as SugiyamaLayoutSettings;
            //            //if (sls != null)
            //            //{
            //            //    sls.GridSizeByX = 30;
            //            //    // sls.GridSizeByY = 0;
            //            //}
            //            var subgraph = new Subgraph("subgraph label");
            //            graph.RootSubgraph.AddSubgraph(subgraph);
            //            subgraph.AddNode(graph.FindNode("47"));
            //            subgraph.AddNode(graph.FindNode("58"));
            //            //layout the graph and draw it
            //            gViewer.Graph = graph;
           // this.propertyGrid1.SelectedObject = graph;
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
    }
}
