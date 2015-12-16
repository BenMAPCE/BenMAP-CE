using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class HealthImpact
    {

        private string _endpointGroup;
        public string EndpointGroup
        {
            get { return _endpointGroup; }
            set { _endpointGroup = value; }
        }

        private string _endpoint;
        public string Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        private string _pollutant;
        public string Pollutant
        {
            get { return _pollutant; }
            set { _pollutant = value; }
        }

        private string _metric;
        public string Metric
        {
            get { return _metric; }
            set { _metric = value; }
        }

        private string _metricStatistic;
        public string MetricStatistis
        {
            get { return _metricStatistic; }
            set { _metricStatistic = value; }
        }

        private string _seasonalMetric;
        public string SeasonalMetric
        {
            get { return _seasonalMetric; }
            set { _seasonalMetric = value; }
        }

        private string _race;
        public string Race
        {
            get { return _race; }
            set { _race = value; }
        }

        private string _ethnicity;
        public string Ethnicity
        {
            get { return _ethnicity; }
            set { _ethnicity = value; }
        }

        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private string _startAge;
        public string StartAge
        {
            get { return _startAge; }
            set { _startAge = value; }
        }

        private string _endAge;
        public string EndAge
        {
            get { return _endAge; }
            set { _endAge = value; }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        private string _year;
        public string Year
        {
            get { return _year; }
            set { _year = value; }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private string _qualifier;
        public string Qualifier
        {
            get { return _qualifier; }
            set { _qualifier = value; }
        }

        private string _otherPollutant;
        public string OtherPollutant
        {
            get { return _otherPollutant; }
            set { _otherPollutant = value; }
        }

        private string _reference;
        public string Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        private string _function;
        public string Function
        {
            get { return _function; }
            set { _function = value; }
        }

        private string _baselineIncidenceFunction;
        public string BaselineIncidenceFunction
        {
            get { return _baselineIncidenceFunction; }
            set { _baselineIncidenceFunction = value; }
        }

        private string _betaDistribution;
        public string BetaDistribution
        {
            get { return _betaDistribution; }
            set { _betaDistribution = value; }
        }

        private string _beta;
        public string Beta
        {
            get { return _beta; }
            set { _beta = value; }
        }

        private string _betaParameter1;
        public string BetaParameter1
        {
            get { return _betaParameter1; }
            set { _betaParameter1 = value; }
        }

        private string _betaParameter2;
        public string BetaParameter2
        {
            get { return _betaParameter2; }
            set { _betaParameter2 = value; }
        }

        private string _aConstantDescription;
        public string AConstantDescription
        {
            get { return _aConstantDescription; }
            set { _aConstantDescription = value; }
        }

        private string _bConstantDescription;
        public string BConstantDescription
        {
            get { return _bConstantDescription; }
            set { _bConstantDescription = value; }
        }

        private string _cconstantDescription;
        public string CConstantDescription
        {
            get { return _cconstantDescription; }
            set { _cconstantDescription = value; }
        }

        private string _aConstantValue;
        public string AConstantValue
        {
            get { return _aConstantValue; }
            set { _aConstantValue = value; }
        }

        private string _bConstantValue;
        public string BConstantValue
        {
            get { return _bConstantValue; }
            set { _bConstantValue = value; }
        }

        private string _cConstantValue;
        public string CConstantValue
        {
            get { return _cConstantValue; }
            set { _cConstantValue = value; }
        }
        private string _incidence;
        public string Incidence
        {
            get { return _incidence; }
            set { _incidence = value; }
        }
        private string _prevalence;
        public string Prevalence
        {
            get { return _prevalence; }
            set { _prevalence = value; }
        }
        private string _variable;
        public string Variable
        {
            get { return _variable; }
            set { _variable = value; }
        }

        private string _locationName;
        public string LocationName
        {
            get { return _locationName; }
            set { _locationName = value; }
        }

        private List<CRFVariable> _pollVariables;
        public List<CRFVariable> PollVariables
        {
            get { return _pollVariables; }
            set { _pollVariables = value; }
        }

        private string _betaVariation;
        public string BetaVariation
        {
            get { return _betaVariation; }
            set { _betaVariation = value; }
        }
    }
}
