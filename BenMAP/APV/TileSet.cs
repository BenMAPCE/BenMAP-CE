using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP.APVX
{
    public partial class TileSet : FormBase
    {
        public BrightIdeasSoftware.ObjectListView olv;
        private List<bool> lstIsTile;
        public TileSet()
        {
            InitializeComponent();
        }

        private void TileSet_Load(object sender, EventArgs e)
        {
            if (olv != null)
            {
                this.olvAvailable.SetObjects(olv.AllColumns);
                lstIsTile = new List<bool>();
                foreach (BrightIdeasSoftware.OLVColumn olvc in olv.AllColumns)
                {
                    lstIsTile.Add(olvc.IsTileViewColumn);
                }

            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < olv.AllColumns.Count; i++)
            {
                BrightIdeasSoftware.OLVColumn olvc = olv.AllColumns[i] as BrightIdeasSoftware.OLVColumn;
                olvc.IsTileViewColumn = olvAvailable.Items[i].Checked;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                for (int i = 0; i < olv.AllColumns.Count; i++)
                {
                    BrightIdeasSoftware.OLVColumn olvc = olv.AllColumns[i] as BrightIdeasSoftware.OLVColumn;
                    if (chkSelectAll.Checked)
                        olvc.IsTileViewColumn = true;
                    else
                        olvc.IsTileViewColumn = false;
                }
                olvAvailable.RefreshObjects(olv.AllColumns);
            }
            catch
            { }
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < olv.Columns.Count; i++)
            {
                BrightIdeasSoftware.OLVColumn olvc = olv.Columns[i] as BrightIdeasSoftware.OLVColumn;
                if (lstIsTile[i])
                    olvc.IsTileViewColumn = true;
                else
                    olvc.IsTileViewColumn = false;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
