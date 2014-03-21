using System.Collections.Generic;

using RestSharp;

namespace BenMAP.Jira
{
    /// <summary>
    /// Represents a new JIRA issue, mapping fields to their values.
    /// 
    /// Intended to be saved using the JIRA REST API resource at "rest/api/2/issue".
    /// </summary>
    class NewJiraIssue
    {
        // basic field names
        public const string FIELD_PROJECT = "project";
        public const string FIELD_ISSUE_TYPE = "issuetype";
        public const string FIELD_SUMMARY = "summary";
        public const string FIELD_DESCRIPTION = "description";
        public const string FIELD_COMPONENTS = "components";

        // basic issue type names
        public const string ISSUE_TYPE_BUG = "Bug";
        public const string ISSUE_TYPE_NEW_FEATURE = "New Feature";
        public const string ISSUE_TYPE_IMPROVEMENT = "Improvement";
        public const string ISSUE_TYPE_TASK = "Task";
        public const string ISSUE_TYPE_STORY = "Story";
        public const string ISSUE_TYPE_EPIC = "Epic";

        // mapping of field name:value
        public readonly Dictionary<string, object> fields;

        /// <summary>
        /// Constructor for the NewIssue class with required fields as parameters.
        /// </summary>
        /// <param name="projectKey">String representing the JIRA project key (e.g. "BENMAP")</param>
        /// <param name="issueTypeName">String representing the issue type name (e.g. "Bug")</param>
        /// <param name="summary">Issue summary</param>
        /// <param name="description">Issue description</param>
        public NewJiraIssue(string projectKey, string issueTypeName, string summary, string description)
        {
            fields = new Dictionary<string, object>();
            fields.Add(FIELD_PROJECT, new { key = projectKey });
            fields.Add(FIELD_ISSUE_TYPE, new { name = issueTypeName });
            fields.Add(FIELD_DESCRIPTION, description);
            fields.Add(FIELD_SUMMARY, summary);
        }

        /// <summary>
        /// Sets the value for the given field.
        /// 
        /// See public static string fields of this class for basic field names.
        /// 
        /// Note that some fields' values should be sub-objects or arrays of objects.
        /// <example>
        /// <code>
        /// JiraProjectComponent component = new JiraProjectComponent() 
        /// {
        ///     name = "Cool GUI Component"
        /// };
        /// newissue.SetField(NewIssue.FIELD_COMPONENTS, new JiraComponent[] { component });
        /// newissue.SetField(NewIssue.FIELD_PROJECT, new { key = "MY_PROJECT" });
        /// newissue.SetField(NewIssue.FIELD_ISSUE_TYPE, new { name = NewIssue.ISSUE_TYPE_BUG });
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="field">String identifying the field name; should match a field name expected by the JIRA REST API.</param>
        /// <param name="value">Value to save for the given field.</param>
        public void SetField(string field, object value)
        {
            fields.Add(field, value);
        }

        /// <summary>
        /// Get the value associated with a field.
        /// </summary>
        /// <param name="field">The field to get the value for.</param>
        /// <returns>Value of the field</returns>
        public object GetField(string field)
        {
            return fields[field];
        }

        /// <summary>
        /// Remove a field.
        /// </summary>
        /// <param name="field">The field to remove.</param>
        /// <returns>Boolean representing whether the field existed and was thus removed.</returns>
        public bool RemoveField(string field)
        {
            return fields.Remove(field);
        }
    }

    /// <summary>
    /// Represents a response from the JIRA REST API resource for creating a new issue.
    /// </summary>
    class NewJiraIssueResponse
    {
        public string id { get; set; }
        public string key { get; set; }
        public string self { get; set; }
    }
}