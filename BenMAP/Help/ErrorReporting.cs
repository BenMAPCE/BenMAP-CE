using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using BenMAP.Jira;

namespace BenMAP
{
	partial class ErrorReporting : FormBase
	{
		private JiraClient client;
		private string baseURL = "";
		private string username = "";
		private string password = "";
		private string projectKey = "";
		private string errorMessage;

		private bool auditTrailCanBeGenerated = false;

		public ErrorReporting()
		{

			InitializeComponent();

			About frmAbout = new About();

			//default options
			rbError.Checked = true;
			rbMajor.Checked = true;
			txtOS.Text = Environment.OSVersion.VersionString;
			txtBenMAPCEVersion.Text = frmAbout.AssemblyVersion.Replace("Version: ", "");


			//if we have jira connector, then load and get info
			if ((!String.IsNullOrEmpty(CommonClass.JiraConnectorFilePath)) && (!String.IsNullOrEmpty(CommonClass.JiraConnectorFilePathTXT)))
			{
				//load dll
				var connectorDLL = Assembly.LoadFile(CommonClass.JiraConnectorFilePath);
				var type = connectorDLL.GetType("BenMAPJiraConnector.Connection");
				//Now you can use dynamic to call the method.
				dynamic connection = Activator.CreateInstance(type);


				baseURL = connection.GetURL();
				username = connection.GetUsername();
				password = connection.GetPassword();
				projectKey = connection.GetProjectKey();

			}

			//populate component combo from Jira
			client = new JiraClient(baseURL, username, password);

			List<JiraProjectComponent> components = (List<JiraProjectComponent>)client.GetProjectComponents(projectKey);

			//if components cannot be retrieved, alert the user and disable the submit button.
			if ((components == null) || (components.Count == 0))
			{
				lblErrorText.Text = "An error occurred while connecting to the BenMAP feedback repository." +
								" Submission of feedback is temporarily disabled.";
				btnSubmit.Enabled = false;
			}
			else
			{
				lblErrorText.Text = "";
				btnSubmit.Enabled = true;
				//fill components drop down
				cboComponent.DisplayMember = "name";
				cboComponent.ValueMember = "id";
				cboComponent.DataSource = components;

				//if audit trail cannot be generated, alert the user and disablet the audit trail checkbox
				TreeView tv = new TreeView();
				int retVal = AuditTrailReportCommonClass.generateAuditTrailReportTreeView(tv);
				if (retVal == -1)
				{
					lblErrorText.Text = "Audit Trail feature is disabled. Please open or configure a complete project to attach an audit trail. Values in all other fields will be submitted.";
					lblAuditTrail.Enabled = false;
					chkAuditTrail.Checked = false;
					chkAuditTrail.Enabled = false;
					auditTrailCanBeGenerated = false;
				}
				else
				{
					lblAuditTrail.Enabled = true;
					chkAuditTrail.Checked = true;
					chkAuditTrail.Enabled = true;
					auditTrailCanBeGenerated = true;
				}

			}




		}



		public string ErrorMessage
		{
			get
			{
				return errorMessage;
			}
			set
			{
				errorMessage = value;
			}
		}


		private void ErrorReporting_Shown(Object sender, EventArgs e)
		{

			txtName.Focus();

			if (!String.IsNullOrEmpty(errorMessage))
			{
				txtDescription.Text = "Error: " + errorMessage;
			}

		}


		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("Are you sure you do not want to Provide Feedback?", "Confirm Cancel", MessageBoxButtons.YesNo);

			if (dialogResult == DialogResult.Yes)
			{
				Close();
			}
		}

		private bool FormIsValid()
		{

			string email = txtEmail.Text.Trim();
			if (!String.IsNullOrEmpty(email))
			{
				RegexUtilities regExUtil = new RegexUtilities();
				if (!regExUtil.IsValidEmail(email))
				{
					lblErrorText.Text = "You must enter a valid email address.";
					txtEmail.Focus();
					return false;
				}
			}

			if (String.IsNullOrEmpty(txtDescription.Text.Trim()))
			{
				lblErrorText.Text = "Description (Please describe what you were doing...) is required.";
				txtDescription.Focus();
				return false;
			}

			return true;

		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			//validate inputs

			if (!FormIsValid())
			{
				return;
			}

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

			//BenMAP-CE Version, name, email, country, description
			string description = "BenMAP-CE Version: " + txtBenMAPCEVersion.Text.Trim() + "\n";
			description = description + "Name: " + txtName.Text.Trim() + "\n";
			description = description + "Email: " + txtEmail.Text.Trim() + "\n";
			description = description + "Country: " + txtCountry.Text.Trim() + "\n\n";
			description = description + "Description: " + txtDescription.Text.Trim();

			NewJiraIssue issue = new NewJiraIssue(projectKey, issueType, issueType, description);

			//os
			issue.SetField(NewJiraIssue.FIELD_ENVIRONMENT, txtOS.Text.Trim());

			//priority
			if (gbSeverity.Enabled)
			{
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
			}

			//component
			//string component = ((JiraProjectComponent)cboComponent.SelectedItem).name;
			issue.SetField(NewJiraIssue.FIELD_COMPONENTS, new JiraProjectComponent[] { (JiraProjectComponent)cboComponent.SelectedItem });

			NewJiraIssueResponse response = client.CreateIssue(issue);
			FileInfo[] files;

			if (response != null)
			{
				//attach error log
				FileInfo fi = new FileInfo(Logger.GetLogPath(null));
				if (fi.Exists)
				{
					files = new FileInfo[1];
					files[0] = fi;
					client.AttachFilesToIssue(response.key, files);
				}

				//add audit trail if required
				bool auditTrailGenerated = false;
				if (chkAuditTrail.Checked)
				{
					TreeView tv = new TreeView();
					int retVal = AuditTrailReportCommonClass.generateAuditTrailReportTreeView(tv);
					if (retVal != -1)
					{
						auditTrailGenerated = true;
						string auditTrailReportPath = fi.DirectoryName + @"\audit_trail.xml";
						AuditTrailReportCommonClass.exportToXml(tv, auditTrailReportPath);
						fi = new FileInfo(auditTrailReportPath);
						if (fi.Exists)
						{
							files = new FileInfo[1];
							files[0] = fi;
							client.AttachFilesToIssue(response.key, files);
						}
					}

				}

				//alert user of success or failure of submittal 
				if ((chkAuditTrail.Checked) && (!auditTrailGenerated))
				{
					MessageBox.Show("Your feedback was submitted successfully! However, Audit Trail could not be attached because your configuration is not complete.");
				}
				else
				{
					MessageBox.Show("Your feedback was submitted successfully!");
				}
				this.Close();


			}
			else
			{
				//alert to failure of submittal
				MessageBox.Show("Your feedback submission failed.");

			}





		}

		private void rbError_CheckedChanged(object sender, EventArgs e)
		{

			if (rbError.Checked)
			{
				lblSeverity.Enabled = true;
				gbSeverity.Enabled = true;
				rbMajor.Checked = true;
				if (auditTrailCanBeGenerated)
				{
					lblAuditTrail.Enabled = true;
					chkAuditTrail.Checked = true;
					chkAuditTrail.Enabled = true;
				}
				lblDescription.Text = "Please describe what you were doing when you encountered the error. Can you tell us how to reproduce the error? (5000 character limit)";
			}
			else
			{
				lblSeverity.Enabled = false;
				gbSeverity.Enabled = false;
				rbMajor.Checked = false;
				rbMinor.Checked = false;
				rbBlocking.Checked = false;
				lblAuditTrail.Enabled = false;
				chkAuditTrail.Checked = false;
				chkAuditTrail.Enabled = false;
				lblDescription.Text = "Please describe the feature you are requesting. (5000 character limit)";
			}
		}



	}
}
