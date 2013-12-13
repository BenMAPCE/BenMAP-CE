using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class ConfigurationAtt
    {
        private bool _isPointMode;
        /// <summary>
        /// 运行点模式
        /// </summary>
        public bool IsPointMode
        {
            get { return _isPointMode; }
            set { _isPointMode = value; }
        }


        private int _latinHypercubeValue;
        /// <summary>
        /// 采用拉丁方采样的值
        /// </summary>
        public int LatinHypercubeValue
        {
            get { return _latinHypercubeValue; }
            set { _latinHypercubeValue = value; }
        }


        private string _popDataSetPath;
        /// <summary>
        /// 人口数据集路径
        /// </summary>
        public string PopDataSetPath
        {
            get { return _popDataSetPath; }
            set { _popDataSetPath = value; }
        }


        private int _popYear;
        /// <summary>
        /// 选用人口数据集的年份
        /// </summary>
        public int PopYear
        {
            get { return _popYear; }
            set { _popYear = value; }
        }


        private string _incidenceDatasetPath;
        /// <summary>
        /// 发病率数据集的路径
        /// </summary>
        public string IncidentDatasetPath
        {
            get { return _incidenceDatasetPath; }
            set { _incidenceDatasetPath = value; }
        }


        private string _healthFunctionPath;
        /// <summary>
        /// 健康影响公式
        /// </summary>
        public string HealthFunctionPath
        {
            get { return _healthFunctionPath; }
            set { _healthFunctionPath = value; }
        }


        private string _thresholdValue;
        /// <summary>
        /// 阈值,在使用的时候要将字符串转换成double
        /// </summary>
        public string ThresholdValue
        {
            get { return _thresholdValue; }
            set { _thresholdValue = value; }
        }
    }
}
