using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class Report
    {
        public Report()
        {
            Params = new List<string>();
            ProcessTypes = new List<string>();
            Forms = new List<string>();
        }
        public string Name { get; set; }

        public string ReportType { get; set; }

        public List<string> Params { get; set; }

        public List<string> ProcessTypes { get; set; }

        public List<string> Forms { get; set; }
    }
}
