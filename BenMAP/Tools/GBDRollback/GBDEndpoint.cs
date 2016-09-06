using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP.Tools.GBDRollback
{
    public class GBDEndpoint
    {
        private string _endpointName;
        private int _endpointID;
        private int _functionID;
        private List<GBDBetaCoefficients> _betaCoefficients;

        public string EndpointName
        {
            get { return _endpointName; }
            set { _endpointName = value; }
        }

        public int EndpointID
        {
            get { return _endpointID; }
            set { _endpointID = value; }
        }

        public int FunctionID
        {
            get { return _functionID; }
            set { _functionID = value; }
        }

        public List<GBDBetaCoefficients> BetaCoefficients
        {
            get { return _betaCoefficients; }
            set { _betaCoefficients = value; }
        }
    }
}
