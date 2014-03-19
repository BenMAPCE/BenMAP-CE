using System;
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

        /// <summary>
        /// Constructor for JiraClient.
        /// 
        /// Throws an ArgumentException if JIRA server does not use HTTPS protocol due to security
        /// concerns of sending authentication credentials without encryption.
        /// </summary>
        /// <param name="baseUrl">JIRA server's base/host URL, must use HTTPS (e.g. https://myjira.atlassian.net)</param>
        /// <param name="username">Username to authenticate client</param>
        /// <param name="password">Password for given username</param>
        public JiraClient(string baseUrl, string username, string password)
        {
            if (!baseUrl.ToLower().Trim().StartsWith("https"))
            {
                throw new ArgumentException("JIRA URL does not support HTTPS protocol: " + baseUrl);
            }

            client = new RestClient(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
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
        /// <returns>``RestSharp.RestResponse`` returned from the server</returns>
        public bool CreateIssue(NewJiraIssue issue)
        {
            var request = new RestRequest("rest/api/2/issue", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(issue);
            var response = client.Execute(request);

            return response.StatusCode.Equals(HttpStatusCode.Created);
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