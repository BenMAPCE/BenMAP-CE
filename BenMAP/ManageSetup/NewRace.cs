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
	public partial class NewRace : FormBase
	{
		public NewRace()
		{
			InitializeComponent();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
		public string _newRaceName = string.Empty;
		private void btnOK_Click(object sender, EventArgs e)
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			try
			{
				string msg = string.Empty;

				if (txtNewRaceName.Text == string.Empty)
				{ MessageBox.Show("Please input a race name."); return; }
				else
				{
					_newRaceName = txtNewRaceName.Text;
					string commandText = string.Format("select RaceID from Races where RaceName='{0}'", txtNewRaceName.Text.ToUpper()); //BenMAP 242--Check List of Available Races for Entered Value
					object raceID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					if (raceID == null)
					{
						commandText = "select max(RACEID) from RACES";
						raceID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
						commandText = string.Format("insert into Races values ({0},'{1}')", raceID, txtNewRaceName.Text.ToUpper());
						fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					}
					else { MessageBox.Show("This race name is already in use. Please enter a different name."); return; }
				}
				this.DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}


		}

		private void NewRace_Activated(object sender, EventArgs e)
		{
			txtNewRaceName.Focus();
		}
	}
}
