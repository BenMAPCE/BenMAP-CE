using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class PopulationInformation : FormBase
    {
        Dictionary<int, string> diclayers;

        public Dictionary<int, string> Diclayers
        {
            get { return diclayers; }
            set { diclayers = value; }
        }
        Dictionary<int, DataRow> dicInfo;

        public Dictionary<int, DataRow> DicInfo
        {
            get { return dicInfo; }
            set { dicInfo = value; }
        }

        public PopulationMap frmpopMap = null;

        public PopulationInformation()
        {
            InitializeComponent();
        }
        private void PopulationInformation_Load(object sender, EventArgs e)
        {
            showInfo();
        }

        public void showInfo()
        {
            try
            {
                DataTable dt = new DataTable();
                DataColumn idColumn = new DataColumn();
                idColumn.DataType = System.Type.GetType("System.Int32");
                idColumn.ColumnName = "id";
                dt.Columns.Add(idColumn);

                DataColumn NameColumn = new DataColumn();
                NameColumn.DataType = System.Type.GetType("System.String");
                NameColumn.ColumnName = "LayerName";
                dt.Columns.Add(NameColumn);

                foreach (KeyValuePair<int, string> k in diclayers)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = k.Key;
                    dr[1] = k.Value;
                    dt.Rows.Add(dr);
                }
                lstLayers.DataSource = dt;
                lstLayers.DisplayMember = "LayerName";
                frmpopMap.FrmPopInfoClose = false;
            }
            catch
            {
            }
        }

        private void lstLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRowView drv = lstLayers.SelectedItem as DataRowView;
                int id = Convert.ToInt32(drv["id"]);
                DataRow dr = dicInfo[id];
                DataTable dt = new DataTable();
                dt.Columns.Add("Field", typeof(string));
                dt.Columns.Add("Information", typeof(string));
                if (dr == null || dr.Table.Columns.Count <= 0)
                {
                    dgvInfo.DataSource = null;
                    return;
                }
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    DataRow _dr = dt.NewRow();
                    _dr[0] = dr.Table.Columns[i].ColumnName;
                    _dr[1] = dr[i];
                    dt.Rows.Add(_dr);
                }
                dgvInfo.DataSource = dt;
            }
            catch
            {
            }
        }

        public void Getform(PopulationMap f)
        {
            frmpopMap = f;
        }

        private void PopulationInformation_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmpopMap.FrmPopInfoClose = true;
        }
    }
}
