using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class CRFBeta
    {
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
        public CRFBeta(double beta, double aConst, string aDesc, double bConst, string bDesc, double cConst,
            string cDesc, double p1, double p2)
        {
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
        public CRFBeta(double beta, double aConst, string aDesc, double bConst, string bDesc, double cConst, 
            string cDesc, double p1, double p2, string seasName, string startDate, string endDate, string seasNumName)
        {
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

        private int _distributionTypeID;
        public int DistributionTypeID
        {
            get { return _distributionTypeID; }
            set { _distributionTypeID = value; }
        }

        public string _distribution;
        public string Distribution
        {
            get { return _distribution; }
            set { _distribution = value; }
        }

        private double _beta;
        public double Beta
        {
            get { return _beta; }
            set { _beta = value; }
        }

        private double _p1Beta;
        public double P1Beta
        {
            get { return _p1Beta; }
            set { _p1Beta = value; }
        }

        private double _p2Beta;
        public double P2Beta
        {
            get { return _p2Beta; }
            set { _p2Beta = value; }
        }

        private double _aConstantValue;
        public double AConstantValue
        {
            get { return _aConstantValue; }
            set { _aConstantValue = value; }
        }

        private string _aConstantName;
        public string AConstantName
        {
            get { return _aConstantName; }
            set { _aConstantName = value; }
        }

        private double _bConstantValue;
        public double BConstantValue
        {
            get { return _bConstantValue; }
            set { _bConstantValue = value; }
        }

        private string _bConstantName;
        public string BConstantName
        {
            get { return _bConstantName; }
            set { _bConstantName = value; }
        }

        private double _cConstantValue;
        public double CConstantValue
        {
            get { return _cConstantValue; }
            set { _cConstantValue = value; }
        }

        private string _cConstantName;
        public string CConstantName
        {
            get { return _cConstantName; }
            set { _cConstantName = value; }
        }

        private string _seasNumName;
        public string SeasNumName
        {
            get { return _seasNumName; }
            set { _seasNumName = value; }
        }

        private string _seasonName;
        public string SeasonName
        {
            get { return _seasonName; }
            set { _seasonName = value; }
        }

        private string _startDate;
        public string StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        private string _endDate;
        public string EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
    }
}
