using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class CRFBeta
    {
        public CRFBeta()
        {
            this._beta = 0;
            this._aConstantValue = 0;
            this._bConstantValue = 0;
            this._cConstantValue = 0;
            this._p1Beta = 0;
            this._p2Beta = 0;
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

        private int _seasonalMetricSeasonID;
        public int SeasonalMetricSeasonID
        {
            get { return _seasonalMetricSeasonID; }
            set { _seasonalMetricSeasonID = value; }
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
    }
}
