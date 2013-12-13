using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class HealthImpact
    {
        
        private string _endpointGroup;
        /// <summary>
        /// 返回EndpointGroup
        /// </summary>
        public string EndpointGroup
        {
            get { return _endpointGroup; }
            set { _endpointGroup = value; }
        }

        private string _endpoint;
        /// <summary>
        /// 返回Endpoint
        /// </summary>
        public string Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        private string _pollutant;
        /// <summary>
        /// 返回Pollutant
        /// </summary>
        public string Pollutant
        {
            get { return _pollutant; }
            set { _pollutant = value; }
        }

        private string _metric;
        /// <summary>
        /// 返回Metric
        /// </summary>
        public string Metric
        {
            get { return _metric; }
            set { _metric = value; }
        }

        private string _metricStatistic;
        /// <summary>
        /// 返回MetricStatistis
        /// </summary>
        public string MetricStatistis
        {
            get { return _metricStatistic; }
            set { _metricStatistic = value; }
        }

        private string _seasonalMetric;
        /// <summary>
        /// 返回SeasonalMetric
        /// </summary>
        public string SeasonalMetric
        {
            get { return _seasonalMetric; }
            set { _seasonalMetric = value; }
        }

        private string _race;
        /// <summary>
        /// 返回Race
        /// </summary>
        public string Race
        {
            get { return _race; }
            set { _race = value; }
        }

        private string _ethnicity;
        /// <summary>
        /// 返回Ethnicity
        /// </summary>
        public string Ethnicity
        {
            get { return _ethnicity; }
            set { _ethnicity = value; }
        }

        private string _gender;
        /// <summary>
        /// 返回Gender
        /// </summary>
        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private string _startAge;
        /// <summary>
        /// 返回StartAge
        /// </summary>
        public string StartAge
        {
            get { return _startAge; }
            set { _startAge = value; }
        }

        private string _endAge;
        /// <summary>
        /// 返回EndAge
        /// </summary>
        public string EndAge
        {
            get { return _endAge; }
            set { _endAge = value; }
        }

        private string _author;
        /// <summary>
        /// 返回Author
        /// </summary>
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        private string _year;
        /// <summary>
        /// 返回Year
        /// </summary>
        public string Year
        {
            get { return _year; }
            set { _year = value; }
        }

        private string _location;
        /// <summary>
        /// 返回Location
        /// </summary>
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private string _qualifier;
        /// <summary>
        /// 返回Qualifier
        /// </summary>
        public string Qualifier
        {
            get { return _qualifier; }
            set { _qualifier = value; }
        }

        private string _otherPollutant;
        /// <summary>
        /// 返回OtherPollutant
        /// </summary>
        public string OtherPollutant
        {
            get { return _otherPollutant; }
            set { _otherPollutant = value; }
        }

        private string _reference;
        /// <summary>
        /// 返回Reference
        /// </summary>
        public string Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        private string _function;
        /// <summary>
        /// 返回Function
        /// </summary>
        public string Function
        {
            get { return _function; }
            set { _function = value; }
        }

        private string _baselineIncidenceFunction;
        /// <summary>
        /// 返回BaselineIncidenceFunction
        /// </summary>
        public string BaselineIncidenceFunction
        {
            get { return _baselineIncidenceFunction; }
            set { _baselineIncidenceFunction = value; }
        }

        private string _betaDistribution;
        /// <summary>
        /// 返回BetaDistribution
        /// </summary>
        public string BetaDistribution
        {
            get { return _betaDistribution; }
            set { _betaDistribution = value; }
        }

        private string _beta;
        /// <summary>
        /// 返回Beta
        /// </summary>
        public string Beta
        {
            get { return _beta; }
            set { _beta = value; }
        }

        private string _betaParameter1;
        /// <summary>
        /// 返回BetaParameter1
        /// </summary>
        public string BetaParameter1
        {
            get { return _betaParameter1; }
            set { _betaParameter1 = value; }
        }

        private string _betaParameter2;
        /// <summary>
        /// 返回BetaParameter1
        /// </summary>
        public string BetaParameter2
        {
            get { return _betaParameter2; }
            set { _betaParameter2 = value; }
        }

        private string _aConstantDescription;
        /// <summary>
        /// 返回AConstantDescription
        /// </summary>
        public string AConstantDescription
        {
            get { return _aConstantDescription; }
            set { _aConstantDescription = value; }
        }

        private string _bConstantDescription;
        /// <summary>
        /// 返回BConstantDescription
        /// </summary>
        public string BConstantDescription
        {
            get { return _bConstantDescription; }
            set { _bConstantDescription = value; }
        }

        private string _cconstantDescription;
        /// <summary>
        /// 返回CConstantDescription
        /// </summary>
        public string CConstantDescription
        {
            get { return _cconstantDescription; }
            set { _cconstantDescription = value; }
        }

        private string _aConstantValue;
        /// <summary>
        /// 返回AConstantValue
        /// </summary>
        public string AConstantValue
        {
            get { return _aConstantValue; }
            set { _aConstantValue = value; }
        }

        private string _bConstantValue;
        /// <summary>
        /// 返回BConstantValue
        /// </summary>
        public string BConstantValue
        {
            get { return _bConstantValue; }
            set { _bConstantValue = value; }
        }

        private string _cConstantValue;
        /// <summary>
        /// 返回CConstantValue
        /// </summary>
        public string CConstantValue
        {
            get { return _cConstantValue; }
            set { _cConstantValue = value; }
        }
        private string _incidence;
        /// <summary>
        /// 返回Incidence
        /// </summary>
        public string Incidence
        {
            get { return _incidence; }
            set { _incidence = value; }
        }
        private string _prevalence;
        /// <summary>
        /// 返回Prevalence
        /// </summary>
        public string Prevalence
        {
            get { return _prevalence; }
            set { _prevalence = value; }
        }
        private string _variable;
        /// <summary>
        /// 返回Variable
        /// </summary>
        public string Variable
        {
            get { return _variable; }
            set { _variable = value; }
        }

        private string _locationName;
        /// <summary>
        /// 返回Variable
        /// </summary>
        public string LocationName
        {
            get { return _locationName; }
            set { _locationName = value; }
        }
    }
}
