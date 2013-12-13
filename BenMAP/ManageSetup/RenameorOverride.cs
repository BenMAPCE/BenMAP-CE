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
    public partial class RenameorOverride : FormBase
    {
        public RenameorOverride()
        {
            InitializeComponent();
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnOverride_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }
    }
}
