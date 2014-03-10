using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class CalcResult
    {

        private int _row;
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        private int _col;
        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }
        private double _pointEstimate;
        public double PointEstimate
        {
            get { return _pointEstimate; }
            set { _pointEstimate = value; }
        }

        private HealthFunctionAtt _funciton;
        public HealthFunctionAtt Function
        {
            get { return _funciton; }
            set { _funciton = value; }
        }

        private double _population;
        public double Population
        {
            get { return _population; }
            set { _population = value; }
        }


        private double _delta;
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
        public double BaseLine
        {
            get { return _baseLine; }
            set { _baseLine = value; }
        }

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
    }
}
