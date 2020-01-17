using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class NewEthnicity : FormBase
    {
        public NewEthnicity()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        public string _newEthnicityName = string.Empty;

        private void btnOK_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                string msg = string.Empty;
                if (txtNewEthnicityName.Text == string.Empty)
                { MessageBox.Show("Please input a ethnicity name."); return; }
                else
                {
                    _newEthnicityName = txtNewEthnicityName.Text;
                    string commandText = string.Format("select EthnicityID from Ethnicity where EthnicityName='{0}'", txtNewEthnicityName.Text.ToUpper()); //BenMAP 242--Check List of Available Ethnicities for Entered Value
                    object EthnicityID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (EthnicityID == null)
                    {
                        commandText = "select max(ETHNICITYID) from ETHNICITY";
                        EthnicityID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into Ethnicity values ({0},'{1}')", EthnicityID, txtNewEthnicityName.Text.ToUpper());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    else { MessageBox.Show("This ethnicity name is already in use. Please enter a different name."); return; }
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void NewEthnicity_Activated(object sender, EventArgs e)
        {
            txtNewEthnicityName.Focus();
        }
    }
}
