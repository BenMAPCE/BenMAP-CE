using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp;

namespace BenMAP.Jira
{
    /// <summary>
    /// Represents a JIRA project component. 
    /// 
    /// Intended for use with JIRA REST API via JSON (de)serialization.
    /// 
    /// Missing some fields because they are sub-objects not represented
    /// in this codebase as separate classes (not necessary as of 3/2014):
    ///   lead
    ///   assignee
    ///   realAssignee
    /// </summary>
    class JiraProjectComponent
    {
        public string self { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string assigneeType { get; set; }
        public string realAssigneeType { get; set; }
        public bool isAssigneeTypeValid { get; set; }
    }
}
