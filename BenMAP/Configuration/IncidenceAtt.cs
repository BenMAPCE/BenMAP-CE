using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class IncidenceAtt
    {

        private string _ndpointGroup;
        public string EndpointGroup
        {
            get { return _ndpointGroup; }
            set { _ndpointGroup = value; }
        }


        private string _endpoint;
        public string Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }


        private int _year;
        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }


        private string _race;
        public string Race
        {
            get { return _race; }
            set { _race = value; }
        }


        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }


        private int _startAge;
        public int StartAge
        {
            get { return _startAge; }
            set { _startAge = value; }
        }


        private int _endAge;
        public int EndAge
        {
            get { return _endAge; }
            set { _endAge = value; }
        }


        private int _row;
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }


        private int _column;
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }


        private double _value;
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
