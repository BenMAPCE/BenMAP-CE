using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    /// <summary>
    /// 保存公式计算的结果
    /// </summary>
    public class CalcResult
    {

        private int _row;
        /// <summary>
        /// 所在行
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        private int _col;
        /// <summary>
        /// 列
        /// </summary>
        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }// _pointEstimate

        private double _pointEstimate;
        /// <summary>
        /// 公式计算出来的值
        /// </summary>
        public double PointEstimate
        {
            get { return _pointEstimate; }
            set { _pointEstimate = value; }
        }

        private HealthFunctionAtt _funciton;
        /// <summary>
        /// 公式中的参数
        /// </summary>
        public HealthFunctionAtt Function
        {
            get { return _funciton; }
            set { _funciton = value; }
        }

        private double _population;
        /// <summary>
        /// 计算公式中的人口
        /// </summary>
        public double Population
        {
            get { return _population; }
            set { _population = value; }
        }


        private double _delta;
        /// <summary>
        /// 
        /// </summary>
        public double Delta
        {
            get { return _delta; }
            set { _delta = value; }
        }

        private string _mean = "NAN";
        public string Mean
        {
            get { return _mean; }
            set { _mean = value; }
        }

        private double _baseLine;
        /// <summary>
        /// Incidence*POP
        /// </summary>
        public double BaseLine
        {
            get { return _baseLine; }
            set { _baseLine = value; }
        }
        //  Percent of Baseline    Standard Deviation   Variance

        private string _percentofBaseline = "NAN";
        public string PercentofBaseline
        {
            get { return _percentofBaseline; }
            set { _percentofBaseline = value; }
        }

        private string _standardDeviation = "NAN";
        public string StandardDeviation
        {
            get { return _standardDeviation; }
            set { _standardDeviation = value; }
        }

        private string _variance = "NAN";
        public string Variance
        {
            get { return _variance; }
            set { _variance = value; }
        }
    }// class
}
