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
    partial class ErrorReporting : FormBase
    {
        public ErrorReporting()
        {
            InitializeComponent();

            //populate component combo from Jira


        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //validate inputs

            //send inputs to Jira


            //get response


            //alert user of success or failure of submittal
        }
    }
}
