using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using RestSharp;

namespace BenMAP.Jira
{
    /// <summary>
    /// Client for interacting with a JIRA REST API. Requires HTTPS and authentication.
    /// </summary>
    class JiraClient
    {
        private IRestClient client;
        private int maxAttachSizeKb;

        /// <summary>
        /// Constructor for JiraClient.
        /// 
        /// Throws an ArgumentException if JIRA server does not use HTTPS protocol due to security
        /// concerns of sending authentication credentials without encryption.
        /// </summary>
        /// <param name="baseUrl">JIRA server's base/host URL, must use HTTPS (e.g. https://myjira.atlassian.net)</param>
        /// <param name="username">Username to authenticate client</param>
        /// <param name="password">Password for given username</param>
        public JiraClient(string baseUrl, string username, string password, int pMaxAttachSizeKb=4096)
        {
            if (!baseUrl.ToLower().Trim().StartsWith("https"))
            {
                throw new ArgumentException("JIRA URL does not support HTTPS protocol: " + baseUrl);
            }

            client = new RestClient(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
            maxAttachSizeKb = pMaxAttachSizeKb;
        }

        /// <summary>
        /// Create a new JIRA issue.
        /// 
        /// Calls JIRA REST API resource: POST rest/api/2/issue
        /// </summary>
        /// <example>
        /// <code>
        /// NewJiraIssue issue = new NewJiraIssue(
        ///     "USERBUGS",
        ///     NewJiraIssue.ISSUE_TYPE_BUG, 
        ///     "found a new bug",
        ///     "I opened the tool and it crashed"
        /// );
        /// JiraProjectComponent comp = new JiraProjectComponent() 
        /// {
        ///     name = "GUI Component"
        /// };
        /// issue.SetField(NewJiraIssue.FIELD_COMPONENTS, new JiraProjectComponent[] { comp });
        /// bool success = client.CreateIssue(issue);
        /// </code>
        /// </example>
        /// <param name="issue">A ``BenMAP.Jira.Issue`` instance with field values to save</param>
        /// <returns>A NewJiraIssueResponse for new issue, or null if it was not created</returns>
        public NewJiraIssueResponse CreateIssue(NewJiraIssue issue)
        {
            var request = new RestRequest("rest/api/2/issue", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(issue);
            var response = client.Execute<NewJiraIssueResponse>(request);

            NewJiraIssueResponse issueResponse = null;
            if (response.StatusCode.Equals(HttpStatusCode.Created))
            {
                issueResponse = response.Data as NewJiraIssueResponse;
            }

            return issueResponse;
        }

        /// <summary>
        /// Attach file(s) to an existing JIRA issue.
        /// 
        /// If any file's size is above this client's max attach size, only the maximum
        /// number of bytes are read and uploaded.
        /// 
        /// Calls JIRA REST API resource: POST rest/api/2/issue/{projectIdOrKey}/attachments
        /// </summary>
        /// <param name="issueKey">Issue key to attach files to (e.g. "USERBUGS-1234")</param>
        /// <param name="files">Info of file(s) to attach</param>
        /// <returns>Whether server returned ``HttpStatusCode.OK`` response (code 200)</returns>
        public bool AttachFilesToIssue(string issueKey, params FileInfo[] files)
        {
            string resource = string.Format("rest/api/2/issue/{0}/attachments", issueKey);
            var request = new RestRequest(resource, Method.POST);

            // these headers required by this particular API resource
            request.AddHeader("X-Atlassian-Token", "nocheck"); 
            request.AddHeader("ContentType", "multipart/form-data");

            foreach (FileInfo fileInfo in files)
            {
                byte[] fileBytes;
                if (fileInfo.Length > maxAttachSizeKb * 1000)
                {
                    // if file size gt max, only read in max allowed bytes
                    fileBytes = new byte[maxAttachSizeKb * 1000];
                    FileStream stream = File.OpenRead(fileInfo.FullName);
                    stream.Read(fileBytes, 0, maxAttachSizeKb * 1000);
                    stream.Close();
                } else {
                    fileBytes = File.ReadAllBytes(fileInfo.FullName);
                }

                request.AddFile("file", fileBytes, fileInfo.Name);
            }
            var response = client.Execute(request);

            return response.StatusCode.Equals(HttpStatusCode.OK);
        }

        /// <summary>
        /// Get a listing of components for a JIRA project.
        /// 
        /// Calls JIRA REST API resource: GET rest/api/2/project/[projectIdOrKey]/components
        /// </summary>
        /// <param name="projectKey">Project key to get a listing for (e.g. "USERBUGS")</param>
        /// <returns>Listing of components</returns>
        public IEnumerable<JiraProjectComponent> GetProjectComponents(string projectKey)
        {
            string resource = string.Format("rest/api/2/project/{0}/components", projectKey);
            var request = new RestRequest(resource, Method.GET);
            var response = client.Execute<List<JiraProjectComponent>>(request);

            return response.Data;
        }
    }
}