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
	public partial class PopulationTipForm : FormBase
	{
		public PopulationTipForm(int maxValue, int minValue, string msg)
		{
			InitializeComponent();
			progressBar1.Maximum = maxValue;
			progressBar1.Minimum = minValue;
			this.Text = msg;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ControlBox = false;
		}

		public void setPos(int value)
		{
			try
			{
				if (value < progressBar1.Maximum)
				{
					progressBar1.Value = value;
					int percentValue = value * 100 / progressBar1.Maximum;
					label1.Text = percentValue.ToString() + "% completed";
				}
				Application.DoEvents();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}

		}

		private void PopulationTipForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				this.Owner.Enabled = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void PopulationTipForm_Load(object sender, EventArgs e)
		{
			try
			{
				this.Owner.Enabled = false;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
	}
}
