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
		public int _rowCount;
		public bool _isEdit;
		public string _rangeName;

		private void AgeRangeDefinition_Load(object sender, EventArgs e)
		{
			try
			{
				if (_isEdit)                            //ensures access to the lower age limit during editing
				{
					txtAgeRangeID.Text = _rangeName;
					txtLowAgeofAgeRange.ReadOnly = false;
					txtLowAgeofAgeRange.BackColor = SystemColors.Window;
					txtLowAgeofAgeRange.Text = _lowAge.ToString();
					txtHighAge.Text = _highAge.ToString();
				}
				else        // if not editing, then increase low age and edit the Range ID accordingly.
				{
					txtLowAgeofAgeRange.Text = (_lowAge + 1).ToString();
					txtHighAge.Text = (_lowAge + 1).ToString();
					if (_rowCount == 0)
						txtLowAgeofAgeRange.ReadOnly = true;

					txtAgeRangeID.Text = (_lowAge + 1) + "TO" + (_highAge + 1);

				}
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
				int lowAge = int.Parse(txtLowAgeofAgeRange.Text);
				int highAge = int.Parse(txtHighAge.Text);
				if (!_isEdit)
				{
					string commandText = "select AgeRangeID from AgeRanges where AgeRangeName='" + txtAgeRangeID.Text + "' and POPULATIONCONFIGURATIONID = " + configurationID + "";
					object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					if (obj != null)
					{
						MessageBox.Show("The age range name is already in use. Please enter a different name.");
						return;
					}
				}
				//if user is editing the existing population data, they should be able to input the same RangeID (12/3/19, MP)
				//when editing, the program also needs access to input value (when adding, the lower year is calculated in relation to the previous age bin's highest year)
				_lowAge = lowAge;

				if ((highAge < _lowAge + 1) && (highAge != _lowAge))
				{
					MessageBox.Show("The end age must be equal to or larger than the start age.");
				}
				else
				{
					_newAgeRangID = txtAgeRangeID.Text;
					_lowAge = lowAge;
					_highAge = highAge; //the upper age limit is editable whether adding or editing the age group
				}

				this.DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void txtLowAgeofAgeRange_TextChanged(object sender, EventArgs e)
		{
			if (txtAgeRangeID.Text.Contains("UP"))
				txtAgeRangeID.Text = this.txtLowAgeofAgeRange.Text + "UP";
			else
				txtAgeRangeID.Text = this.txtLowAgeofAgeRange.Text + "TO" + txtHighAge.Text;


		}

		private void txtHighAge_TextChanged(object sender, EventArgs e)
		{
			if (txtAgeRangeID.Text.Contains("UP"))
				txtAgeRangeID.Text = txtLowAgeofAgeRange.Text + "UP";
			else
				txtAgeRangeID.Text = txtLowAgeofAgeRange.Text + "TO" + this.txtHighAge.Text;
		}
	}
}
