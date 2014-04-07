using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using BenMAP.Jira;

namespace BenMAP
{
    partial class ErrorReporting : FormBase
    {
        private JiraClient client;
        private const string baseURL = "https://f8nnm8p.atlassian.net/";
        //private const string username = "mscruggs";
        //private const string password = "tempAcct1";
        private const string username = "BenMAP-CE";
        private const string password = "BenMAPOpenSource14";
        private const string projectKey = "USERBUGS";       

        public ErrorReporting()
        {           

            InitializeComponent();

            //default options
            rbError.Checked = true;
            rbMajor.Checked = true;

            //populate component combo from Jira
            client = new JiraClient(baseURL, username, password);
           
            List<JiraProjectComponent> components = (List<JiraProjectComponent>)client.GetProjectComponents(projectKey);

            //if components cannot be retrieved, alert the user and disable the submit button.
            if ((components == null) || (components.Count == 0))
            {
                lblErrorText.Text = "An error occurred while connecting to the BenMAP feedback repository." +
                        "  Provide Feedback is temporarily disabled.";
                btnSubmit.Enabled = false;
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

            //issue type
            string issueType;
            if (rbError.Checked)
            {
                issueType = NewJiraIssue.ISSUE_TYPE_BUG;
            }
            else
            {
                issueType = NewJiraIssue.ISSUE_TYPE_NEW_FEATURE;
            }

            //name, email, country, description
            string description = "Name: " + txtName.Text.Trim() + "\n";
            description = description + "Email: " + txtEmail.Text.Trim() + "\n";
            description = description + "Country: " + txtCountry.Text.Trim() + "\n\n";
            description = description + "Description: " + txtDescription.Text.Trim();

            NewJiraIssue issue = new NewJiraIssue(projectKey, issueType, issueType, description);

            //os
            issue.SetField(NewJiraIssue.FIELD_ENVIRONMENT, txtOS.Text.Trim());

            //priority
            string priority;
            if (rbMinor.Checked)
            {
                priority = NewJiraIssue.PRIORITY_MINOR;
            }
            else if (rbMajor.Checked)
            {
                priority = NewJiraIssue.PRIORITY_MAJOR;
            }
            else
            {
                priority = NewJiraIssue.PRIORITY_BLOCKER;
            }
            issue.SetField(NewJiraIssue.FIELD_PRIORITY, new { name = priority }); 
                
            //component
            //string component = ((JiraProjectComponent)cboComponent.SelectedItem).name;
            issue.SetField(NewJiraIssue.FIELD_COMPONENTS, new JiraProjectComponent[] { (JiraProjectComponent)cboComponent.SelectedItem });

            NewJiraIssueResponse response = client.CreateIssue(issue);

            if (response != null)
            {
                //attach error log
                FileInfo fi = new FileInfo(Logger.GetLogPath(null));
                FileInfo[] files = new FileInfo[1];
                files[0] = fi;
                client.AttachFilesToIssue(response.key, files);
                

                //add attachments if required
                if (chkAuditTrail.Checked)
                {
                    TreeView tv = new TreeView();
                    int retVal = AuditTrailReportCommonClass.generateAuditTrailReportTreeView(tv);
                    if (retVal == -1)
                    {
                        MessageBox.Show("Provide Feedback submittal failed - Audit Trail could not be attached because your configuration is not complete.");
                        return;
                    }                    

                    string auditTrailReportPath = fi.DirectoryName + @"\audit_trail.xml";
                    AuditTrailReportCommonClass.exportToXml(tv, auditTrailReportPath);
                    fi = new FileInfo(auditTrailReportPath);
                    files = new FileInfo[1];
                    files[0] = fi;
                    client.AttachFilesToIssue(response.key, files);
                }

                

                //alert user of success or failure of submittal    
                MessageBox.Show("Provide Feedback was submitted successfully!");
                this.Close();


            }
            else 
            {
                //alert to failure of submittal
                MessageBox.Show("Provide Feedback submittal failed.");
            
            }


 
           

        }
    }
}
