using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BenMAP
{
    public partial class BenMAP
    {
        private Dictionary<string, Report> _dicReports = null;

        void ResetParamsTree(string paramsFile)
        {
            try
            {
                trvSetting.Nodes.Clear();
                if (string.IsNullOrEmpty(paramsFile))
                {
                    paramsFile = Application.StartupPath + @"\Configs\ParamsTree_USA.xml";
                }
                var root = XElement.Load(paramsFile);
                foreach (var element in root.Elements())
                {
                    trvSetting.Nodes.Add(ParseElement(element));
                }
                trvSetting.ExpandAll();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        TreeNode ParseElement(XElement element)
        {
            TreeNode node = new TreeNode()
{
    Name = element.Attribute("name").Value,
    Text = element.Attribute("text").Value,
    ToolTipText = element.Attribute("tooltip").Value,
    ImageKey = element.Attribute("icon").Value,
    SelectedImageKey = element.Attribute("icon").Value
};
            if (node.Name == "GridType")
            {
                foreach (var e in element.Elements())
                {
                    TreeNode child = new TreeNode()
                     {
                         Name = e.Attribute("name").Value,
                         Text = Application.StartupPath + e.Attribute("text").Value,
                         ToolTipText = element.Attribute("tooltip").Value,
                         ImageKey = e.Attribute("icon").Value,
                         SelectedImageKey = e.Attribute("icon").Value
                     };
                    node.Nodes.Add(child);
                }
            }
            else
            {
                foreach (var e in element.Elements())
                {
                    node.Nodes.Add(ParseElement(e));
                }
            }

            return node;
        }

        void ResetReportList()
        {
            _dicReports = new Dictionary<string, Report>();
            string file = Application.StartupPath + @"\Configs\ReportParams.xml";
            var root = XElement.Load(file);
            var element = from emt in root.Elements()
                          where emt.Attribute("type").Value == "raw"
                          select emt;

            element = from emt in root.Elements()
                      where emt.Attribute("type").Value == "processed"
                      select emt;
        }
        void AddReport2List(IEnumerable<XElement> root, ListView lvw)
        {
            lvw.Items.Clear();
            string reportType = root.ElementAt(0).Attribute("type").Value;
            foreach (var item in root.Elements())
            {
                string reportName = item.Attribute("text").Value;
                var report = new Report() { Name = reportName, ReportType = reportType };
                ListViewItem listViewItem = new ListViewItem(reportName);
                foreach (var p in item.Element("params").Elements())
                {
                    report.Params.Add(p.Attribute("name").Value);
                }
                listViewItem.Tag = reportName;
                lvw.Items.Add(listViewItem);

                foreach (var p in item.Element("forms").Elements())
                {
                    report.Forms.Add(p.Attribute("name").Value);
                }
                _dicReports.Add(reportName, report);
            }
        }



        void SetReportTip(ListView lvw)
        {
            foreach (ListViewItem item in lvw.Items)
            {
                StringBuilder tip = new StringBuilder();
                string[] needParams = _dicReports[item.Tag.ToString()].Params.ToArray();
                for (int i = 0; i < needParams.Length; i++)
                {
                    TreeNode node = FindNodeByName(needParams[i]);
                    if (node == null)
                    {
                        string msg = string.Format("Param {0} not found.", needParams[i]);
                        Console.WriteLine(msg);
                        tip.AppendLine(msg);
                    }
                    else if (node.Name == needParams[i])
                    {
                        if (node.ImageKey == _unreadyImageKey)
                        {
                            tip.AppendLine(node.Text + " not set.");
                        }
                    }
                } item.ToolTipText = tip.ToString();
                item.ImageKey = tip.Length == 0 ? _readyImageKey : _unreadyImageKey;
            }
        }

        TreeNode FindNodeByName(string name)
        {
            foreach (TreeNode root in trvSetting.Nodes)
            {
                if (root.Name == name)
                {
                    return root;
                }
                TreeNode node = FindChildNodeByName(root, name);
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }
        TreeNode FindChildNodeByName(TreeNode root, string name)
        {
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Name == name)
                {
                    return node;
                }
                else
                {
                    TreeNode nod = FindChildNodeByName(node, name);
                    if (nod != null)
                    {
                        return nod;
                    }
                }
            }
            return null;
        }
    }
}
