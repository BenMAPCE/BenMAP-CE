using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP.Tools.GBDRollback
{
    public class GBDHealthImpactFunction
    {
        private string _functionName;
        private int _functionID;
        private List<GBDEndpoint> _endpoints;

        public string FunctionName
        {
            get { return _functionName; }
            set { _functionName = value; }
        }

        public int FunctionID
        {
            get { return _functionID; }
            set { _functionID = value; }
        }

        public List<GBDEndpoint> Endpoints
        {
            get { return _endpoints; }
            set { _endpoints = value; }
        }
    }
}
