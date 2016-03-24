using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace BenMAP
{
    [Serializable]
    [ProtoContract]
    public class CRFBeta
    {
        [ProtoMember(1)]
        private int _betaID;
        [ProtoMember(2)]
        private int _distributionTypeID;
        [ProtoMember(3)]
        public string _distribution;
        [ProtoMember(4)]
        private double _beta;
        [ProtoMember(5)]
        private double _p1Beta;
        [ProtoMember(6)]
        private double _p2Beta;
        [ProtoMember(7)]
        private double _aConstantValue;
        [ProtoMember(8)]
        private string _aConstantName;
        [ProtoMember(9)]
        private double _bConstantValue;
        [ProtoMember(10)]
        private string _bConstantName;
        [ProtoMember(11)]
        private double _cConstantValue;
        [ProtoMember(12)]
        private string _cConstantName;
        [ProtoMember(13)]
        private string _seasNumName;
        [ProtoMember(14)]
        private string _seasonName;
        [ProtoMember(15)]
        private string _startDate;
        [ProtoMember(16)]
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
