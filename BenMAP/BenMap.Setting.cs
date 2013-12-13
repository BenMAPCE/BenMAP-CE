using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
//using BenMAP.DataTypes;

namespace BenMAP
{
    public partial class BenMAP
    {
        /// <summary>
        /// all reports definitions.
        /// key=report name
        /// </summary>
        private Dictionary<string, Report> _dicReports = null;

        #region 参数列表树
        /// <summary>
        /// 重设参数列表树
        /// </summary>
        void ResetParamsTree(string paramsFile)
        {
            try
            {
                trvSetting.Nodes.Clear();
                // Todo:陈志润 设置水平距离
                //trvSetting.Indent = 10;
                //trvSetting.Font = new System.Drawing.Font("Cambri", 10, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                //trvSetting.ItemHeight = 25;
                // 指定参数文件路径
                if (string.IsNullOrEmpty(paramsFile))
                {
                    paramsFile = Application.StartupPath + @"\Configs\ParamsTree_USA.xml";
                }
                // 把整个参数文件作为一个Element,此处root为参数文件里的params节
                var root = XElement.Load(paramsFile);
                // 将params的所有子节点加到treeview
                foreach (var element in root.Elements())
                {
                    trvSetting.Nodes.Add(ParseElement(element));
                }
                //trvSetting.Margin = new System.Windows.Forms.Padding(5, 1000, 5, 10);
                trvSetting.ExpandAll();
                // 报告参数改变了
                //OnParamsChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// 将节点转换成Treeview的node
        /// </summary>
        /// <param name="element">节点的xml定义</param>
        /// <returns></returns>
        TreeNode ParseElement(XElement element)
        {
            // 将节点加到Treeview
            TreeNode node = new TreeNode()
            {
                Name = element.Attribute("name").Value,
                Text = element.Attribute("text").Value,
                ToolTipText=element.Attribute("tooltip").Value,
                ImageKey = element.Attribute("icon").Value,
                SelectedImageKey = element.Attribute("icon").Value
            };
            if (node.Name == "GridType")
            {
                // 加上本节点的子节点(使用递归)
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
            {                 // 加上本节点的子节点(使用递归)
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
            // 此处root为xml文件里的root节点
            var root = XElement.Load(file);
            // raw report
            var element = from emt in root.Elements()
                          where emt.Attribute("type").Value == "raw"
                          select emt;
            //AddReport2List(element, lvwRawData);

            // processed report
            element = from emt in root.Elements()
                      where emt.Attribute("type").Value == "processed"
                      select emt;
            //AddReport2List(element, lvwResultType);
        }
        /// <summary>
        /// 把xml解析,加到listview中.同时把报表名称保存在ListViewItem.Tag里,报表则保存在_dicReports中
        /// </summary>
        /// <param name="root"></param>
        /// <param name="lvw"></param>
        void AddReport2List(IEnumerable<XElement> root, ListView lvw)
        {
            lvw.Items.Clear();
            string reportType = root.ElementAt(0).Attribute("type").Value;
            foreach (var item in root.Elements())
            {
                string reportName = item.Attribute("text").Value;
                var report = new Report() { Name = reportName, ReportType = reportType };
                ListViewItem listViewItem = new ListViewItem(reportName);
                //string tag = "";
                foreach (var p in item.Element("params").Elements())
                {
                    //if (tag == "")
                    //{
                    //    tag = p.Attribute("name").Value;
                    //}
                    //else
                    //{
                    //    tag += "," + p.Attribute("name").Value;
                    //}
                    report.Params.Add(p.Attribute("name").Value);
                }
                //listViewItem.Tag = tag;
                listViewItem.Tag = reportName;
                lvw.Items.Add(listViewItem);

                foreach (var p in item.Element("forms").Elements())
                {
                    report.Forms.Add(p.Attribute("name").Value);
                }
                _dicReports.Add(reportName, report);
            }
        }
        #endregion

        #region 报表列表
        /// <summary>
        /// 参数列表树设置改变
        /// </summary>
        //void OnParamsChanged()
        //{
        //    //SetReportTip(lvwRawData);
        //    //SetReportTip(lvwResultType);
        //    //SetReportTip(lvwRawForm);
        //}

        //void SetReportForm(ListView lvw)
        //{
        //    foreach (ListViewItem item in lvw.Items)
        //    {
        //        StringBuilder tip = new StringBuilder();
        //        string[] needParams = item.Tag.ToString().Split(new char[] { ',' });
        //    }
        //}

        /// <summary>
        /// 设置报表提示
        /// </summary>
        /// <param name="lvw"></param>
        void SetReportTip(ListView lvw)
        {
            foreach (ListViewItem item in lvw.Items)
            {
                StringBuilder tip = new StringBuilder();
                //string[] needParams = item.Tag.ToString().Split(new char[] { ',' });
                // 找到报表需要的条件列表
                string[] needParams = _dicReports[item.Tag.ToString()].Params.ToArray();
                // 遍历条件列表,在treeview中寻找相应的node,如果找不到,或者node存在但未设置值,则将该条件加入提示
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
                        // node存在,但未设置值
                        if (node.ImageKey == _unreadyImageKey)
                        {
                            tip.AppendLine(node.Text + " not set.");
                        }
                    }
                }//needParams
                item.ToolTipText = tip.ToString();
                item.ImageKey = tip.Length == 0 ? _readyImageKey : _unreadyImageKey;
            }//reports
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
        #endregion
    }//class
}
