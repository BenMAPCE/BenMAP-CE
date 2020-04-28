using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace BenMAP
{
	public partial class StartPage : FormBase
	{

		private System.Windows.Forms.Timer _mytimer;

		public StartPage()
		{
			InitializeComponent();
			_mytimer.Interval = 20;
			_mytimer.Start();

			_mytimer.Tick += new EventHandler(TimerEventProcessor);
			lblVersion.Text = "version " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
		}

		private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
		{
			List<GridRelationship> lst = CommonClass.LstGridRelationshipAll;

			_mytimer.Stop();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
