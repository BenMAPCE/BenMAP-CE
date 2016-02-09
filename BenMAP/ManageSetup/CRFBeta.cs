using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class CRFBeta
    {
        private int _betaID;
        private int _distributionTypeID;
        public string _distribution;
        private double _beta;
        private double _p1Beta;
        private double _p2Beta;
        private double _aConstantValue;
        private string _aConstantName;
        private double _bConstantValue;
        private string _bConstantName;
        private double _cConstantValue;
        private string _cConstantName;
        private string _seasNumName;
        private string _seasonName;
        private string _startDate;
        private string _endDate;

        // New beta without values
        public CRFBeta()
        {
            this._beta = 0;
            this._aConstantValue = 0;
            this._bConstantValue = 0;
            this._cConstantValue = 0;
            this._p1Beta = 0;
            this._p2Beta = 0;
            this._seasonName = string.Empty;
            this._startDate = string.Empty;
            this._endDate = string.Empty;
            this._seasonName = string.Empty;
        }

        // Full year
        public CRFBeta(int bID, double beta, double aConst, string aDesc, double bConst, string bDesc, double cConst,
            string cDesc, double p1, double p2)
        {
            this._betaID = bID;
            this._beta = beta;
            this._aConstantValue = aConst;
            this._aConstantName = aDesc;
            this._bConstantValue = bConst;
            this._bConstantName = bDesc;
            this._cConstantValue = cConst;
            this._cConstantName = cDesc;
            this._p1Beta = p1;
            this._p2Beta = p2;
            this._seasonName = string.Empty;
            this._startDate = string.Empty;
            this._endDate = string.Empty;
            this._seasNumName = string.Empty;
        }

        // Seasonal
        public CRFBeta(int bID, double beta, double aConst, string aDesc, double bConst, string bDesc, double cConst, 
            string cDesc, double p1, double p2, string seasName, string startDate, string endDate, string seasNumName)
        {
            this._betaID = bID;
            this._beta = beta;
            this._aConstantValue = aConst;
            this._aConstantName = aDesc;
            this._bConstantValue = bConst;
            this._bConstantName = bDesc;
            this._cConstantValue = cConst;
            this._cConstantName = cDesc;
            this._p1Beta = p1;
            this._p2Beta = p2;
            this._seasonName = seasName;
            this._startDate = startDate;
            this._endDate = endDate;
            this._seasNumName = seasNumName;
        }

        public int BetaID
        {
            get { return _betaID; }
            set { _betaID = value; }
        }

        public int DistributionTypeID
        {
            get { return _distributionTypeID; }
            set { _distributionTypeID = value; }
        }

        public string Distribution
        {
            get { return _distribution; }
            set { _distribution = value; }
        }

        public double Beta
        {
            get { return _beta; }
            set { _beta = value; }
        }

        public double P1Beta
        {
            get { return _p1Beta; }
            set { _p1Beta = value; }
        }

        public double P2Beta
        {
            get { return _p2Beta; }
            set { _p2Beta = value; }
        }

        public double AConstantValue
        {
            get { return _aConstantValue; }
            set { _aConstantValue = value; }
        }

        public string AConstantName
        {
            get { return _aConstantName; }
            set { _aConstantName = value; }
        }

        public double BConstantValue
        {
            get { return _bConstantValue; }
            set { _bConstantValue = value; }
        }

        public string BConstantName
        {
            get { return _bConstantName; }
            set { _bConstantName = value; }
        }

        public double CConstantValue
        {
            get { return _cConstantValue; }
            set { _cConstantValue = value; }
        }

        public string CConstantName
        {
            get { return _cConstantName; }
            set { _cConstantName = value; }
        }

        public string SeasNumName
        {
            get { return _seasNumName; }
            set { _seasNumName = value; }
        }

        public string SeasonName
        {
            get { return _seasonName; }
            set { _seasonName = value; }
        }

        public string StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public string EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
    }
}
