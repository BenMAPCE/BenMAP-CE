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
    public partial class NewGender : FormBase
    {
        public NewGender()
        {
            InitializeComponent();
        }

        public string _newGenderName = string.Empty;

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                //string msg = string.Empty;
                if (txtNewGenderName.Text == string.Empty)
                { MessageBox.Show("Please input a gender name."); return; }
                else
                {
                    _newGenderName = txtNewGenderName.Text;
                    string commandText = string.Format("select GenderID from Genders where GenderName='{0}'", txtNewGenderName.Text);
                    object GenderID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (GenderID == null)
                    {
                        commandText = "select max(GENDERID) from GENDERS";
                        GenderID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into Genders values ({0},'{1}')", GenderID, txtNewGenderName.Text);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    else { MessageBox.Show("This gender name is already in use. Please enter a different name."); return; }
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NewGender_Activated(object sender, EventArgs e)
        {
            txtNewGenderName.Focus();
        }
    }
}
