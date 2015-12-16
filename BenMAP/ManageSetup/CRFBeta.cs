using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class CRFBeta
    {
        private int _distributionTypeID;
        public int DistributionTypeID
        {
            get { return _distributionTypeID; }
            set { _distributionTypeID = value; }
        }

        private int _seasonalMetricSeasonID;
        public int SeasonalMetricSeasonID
        {
            get { return _seasonalMetricSeasonID; }
            set { _seasonalMetricSeasonID = value; }
        }

        private float _beta;
        public float Beta
        {
            get { return _beta; }
            set { _beta = value; }
        }

        private float _p1Beta;
        public float P1Beta
        {
            get { return _p1Beta; }
            set { _p1Beta = value; }
        }

        private float _p2Beta;
        public float P2Beta
        {
            get { return _p2Beta; }
            set { _p2Beta = value; }
        }

        private float _aConstantValue;
        public float AConstantValue
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

        private float _bConstantValue;
        public float BConstantValue
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

        private float _cConstantValue;
        public float CConstantValue
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
