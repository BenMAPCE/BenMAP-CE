using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class CRFVarCov
    {
        private string _interactionPollutant;
        private double _varCov;
        private int _betaID1;
        private int _betaID2;
        private int _varCovID;

        public CRFVarCov()
        {
            this._interactionPollutant = "";
            this._varCov = 0;
            this._betaID1 = 0;
            this._betaID2 = 0;
            this._varCovID = 0;
        }

        public string InteractionPollutant
        {
            get { return _interactionPollutant; }
            set { _interactionPollutant = value; }
        }

        public double VarCov
        {
            get { return _varCov; }
            set { _varCov = value; }
        }

        public int BetaID1
        {
            get { return _betaID1; }
            set { _betaID1 = value; }
        }

        public int BetaID2
        {
            get { return _betaID2; }
            set { _betaID2 = value; }
        }

        public int VarCovID
        {
            get { return _varCovID; }
            set { _varCovID = value; }
        }
    }
}
