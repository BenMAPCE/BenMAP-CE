using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP.Tools.GBDRollback
{
    public class GBDBetaCoefficients
    {
        private double _betaMean;
        private double _betaSE;
        private double _a;
        private double _b;
        private int _segmentID;
        private int _endpointID;

        public double BetaMean
        {
            get { return _betaMean; }
            set { _betaMean = value; }
        }

        public double BetaSE
        {
            get { return _betaSE; }
            set { _betaSE = value; }
        }

        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        public double B
        {
            get { return _b; }
            set { _b = value; }
        }

        public int SegmentID
        {
            get { return _segmentID; }
            set { _segmentID = value; }
        }

        public int EndpointID
        {
            get { return _endpointID; }
            set { _endpointID = value; }
        }
    }
}
