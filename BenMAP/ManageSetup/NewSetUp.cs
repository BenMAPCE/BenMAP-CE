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
    public partial class NewSetUp : FormBase
    {
        public NewSetUp()
        {
            InitializeComponent();
        }

        private string _newSetupName;
        /// <summary>
        /// 返回NewSetupName
        /// </summary>
        public string NewSetupName
        {
            get { return _newSetupName; }
            set { _newSetupName = value; }
        }
        private void NewSetUp_Load(object sender, EventArgs e)
        {
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtNewSetupName.Text.Trim() == "") return;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = "select SetupName from Setups";
            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            foreach (DataRow  dr in ds.Tables[0].Rows )
            {
                string str = dr[0].ToString();
                if (txtNewSetupName.Text == str)
                {
                    MessageBox.Show("This setup name is already in use. Please enter a different name.");
                    return;
                }
            }           
            _newSetupName = txtNewSetupName.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
