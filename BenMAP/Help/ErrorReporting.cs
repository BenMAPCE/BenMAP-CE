using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BenMAP.Jira;

namespace BenMAP
{
    partial class ErrorReporting : FormBase
    {
        public ErrorReporting()
        {
            InitializeComponent();

            //populate component combo from Jira
            //JiraClient jc = new JiraClient("", "", "");
            //List<JiraProjectComponent> components = (List<JiraProjectComponent>)jc.GetProjectComponents("USERBUGS");

            //if components cannot be retrieved, alert the user and disable the submit button.


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
