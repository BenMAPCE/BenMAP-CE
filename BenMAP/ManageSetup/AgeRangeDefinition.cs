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
    public partial class AgeRangeDefinition : FormBase
    {
        public AgeRangeDefinition()
        {
            InitializeComponent();
        }
        private object configurationID;
        public AgeRangeDefinition(object configID)
        {
            InitializeComponent();
            configurationID = configID;
        }
        public int _lowAge;
        public string _newAgeRangID;
        public int _highAge;
        private void AgeRangeDefinition_Load(object sender, EventArgs e)
        {
            try
            {
              txtAgeRangeID.Text = "AgeRange 0";
              txtLowAgeofAgeRange.Text = (_lowAge+1).ToString();
              txtHighAge.Text = (_lowAge+1).ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        } 
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                int highAge=int.Parse(txtHighAge.Text);
                string commandText = "select AgeRangeID from AgeRanges where AgeRangeName='" + txtAgeRangeID.Text + "' and POPULATIONCONFIGURATIONID = "+configurationID+"";
                object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (obj != null)
                {
                    MessageBox.Show("The age range name is already in use. Please enter a different name.");
                    return;
                }
                if (highAge < _lowAge+1)
                {
                    MessageBox.Show("The end age must be equal to or larger than the start age.");
                }                
                else
                {
                    _newAgeRangID = txtAgeRangeID.Text;
                    _highAge = highAge;
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

       
    }
}
