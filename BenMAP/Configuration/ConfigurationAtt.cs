using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
	public class ConfigurationAtt
	{
		private bool _isPointMode;
		public bool IsPointMode
		{
			get { return _isPointMode; }
			set { _isPointMode = value; }
		}


		private int _latinHypercubeValue;
		public int LatinHypercubeValue
		{
			get { return _latinHypercubeValue; }
			set { _latinHypercubeValue = value; }
		}


		private string _popDataSetPath;
		public string PopDataSetPath
		{
			get { return _popDataSetPath; }
			set { _popDataSetPath = value; }
		}


		private int _popYear;
		public int PopYear
		{
			get { return _popYear; }
			set { _popYear = value; }
		}


		private string _incidenceDatasetPath;
		public string IncidentDatasetPath
		{
			get { return _incidenceDatasetPath; }
			set { _incidenceDatasetPath = value; }
		}


		private string _healthFunctionPath;
		public string HealthFunctionPath
		{
			get { return _healthFunctionPath; }
			set { _healthFunctionPath = value; }
		}


		private string _thresholdValue;
		public string ThresholdValue
		{
			get { return _thresholdValue; }
			set { _thresholdValue = value; }
		}
	}
}
