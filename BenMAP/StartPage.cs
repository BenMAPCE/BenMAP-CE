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
           //this.DialogResult = DialogResult.OK;
           //this.Close();
            //Image backImg = Image.FromFile(Application.StartupPath + @"\Data\Image\StartPage.jpg");
            //this.BackgroundImage = backImg;
            // Sets the timer interval to 3 seconds.
            _mytimer.Interval = 20;
            _mytimer.Start();
            //mytimer_Tick(null, null);           
            /* Adds the event and the event handler for the method that will process the timer event to the timer. */
            _mytimer.Tick += new EventHandler(TimerEventProcessor);
            lblVersion.Text = "version " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
        }

        // This is the method to run when the timer is raised.
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            List<GridRelationship> lst = CommonClass.LstGridRelationshipAll;
          
            _mytimer.Stop();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
