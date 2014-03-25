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
        JiraClient client;

        public ErrorReporting()
        {           

            InitializeComponent();

            //populate component combo from Jira
            client = new JiraClient("https://f8nnm8p.atlassian.net/", "mscruggs", "tempAcct1");
            //client = new JiraClient("https://f8nnm8p.atlassian.net/", "BenMAP@epa.gov", "BenMAPOpenSource14");
            List<JiraProjectComponent> components = (List<JiraProjectComponent>)client.GetProjectComponents("USERBUGS");

            //if components cannot be retrieved, alert the user and disable the submit button.
            if ((components == null) || (components.Count == 0))
            {
                lblErrorText.Text = "An error occurred while connecting to the BenMAP error reporting repository." +
                        "  Error Reporting is temporarily disabled.";
                //btnSubmit.Enabled = false;
            }
            else {
                lblErrorText.Text = "";
                btnSubmit.Enabled = true;        
                //fill components drop down
                cboComponent.DisplayMember = "name";
                cboComponent.ValueMember = "id";
                cboComponent.DataSource = components;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //validate inputs

            //send inputs to Jira
            NewJiraIssue issue = new NewJiraIssue("USERBUGS", "Bug", "test", "test description");

            NewJiraIssueResponse response = client.CreateIssue(issue);

            //get response


            //alert user of success or failure of submittal
        }
    }
}
