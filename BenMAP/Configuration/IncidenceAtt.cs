using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class IncidenceAtt
    {

        private string _ndpointGroup; 
        /// <summary>
        /// 
        /// </summary>
        public string EndpointGroup
        {
            get { return _ndpointGroup; }
            set { _ndpointGroup = value; }
        }


        private string _endpoint; 
        /// <summary>
        /// 
        /// </summary>
        public string Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }


        private int _year; 
        /// <summary>
        /// 
        /// </summary>
        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }


        private string _race; 
        /// <summary>
        /// 
        /// </summary>
        public string Race
        {
            get { return _race; }
            set { _race = value; }
        }


        private string _gender; 
        /// <summary>
        /// 
        /// </summary>
        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }


        private int _startAge;
        /// <summary>
        /// 
        /// </summary>
        public int StartAge
        {
            get { return _startAge; }
            set { _startAge = value; }
        }


        private int _endAge; 
        /// <summary>
        /// 
        /// </summary>
        public int EndAge
        {
            get { return _endAge; }
            set { _endAge = value; }
        }


        private int _row; 
        /// <summary>
        /// 
        /// </summary>
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }


        private int _column; 
        /// <summary>
        /// 
        /// </summary>
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }


        private double _value;
        /// <summary>
        /// 
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
