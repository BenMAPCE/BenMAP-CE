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
    public partial class Sheets : FormBase
    {
        private Dictionary<string, int> sheets = new Dictionary<string, int>();
        public int sheetIndex = 0;
        public Sheets(Dictionary<string, int> sheetInfo)
        {
            InitializeComponent();
            sheets = sheetInfo;
        }

        private void Sheets_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (string sheetname in sheets.Keys)
                {
                    lstSheets.Items.Add(sheetname);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void lstSheets_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                string sheetName=lstSheets.GetItemText(lstSheets.SelectedItem);
                sheetIndex=sheets[sheetName];
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }
}
