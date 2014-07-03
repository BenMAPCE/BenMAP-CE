using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BenMAP
{
    public class GBDRollbackItem
    {
        public enum RollbackType { Percentage, Incremental, Standard }
        public enum StandardType {One, Two, Three}

        private string name;
        private string description;
        List<string> countries;
        private RollbackType type;
        private double percentage;
        private double increment;
        private StandardType standard;
        private double background;
        private Color color;
        private int year;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public List<string> Countries
        {
            get { return countries; }
            set { countries = value; }
        }

        public RollbackType Type
        {
            get { return type; }
            set { type = value; }
        }

        public double Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }

        public double Increment
        {
            get { return increment; }
            set { increment = value; }
        }

        public StandardType Standard
        {
            get { return standard; }
            set { standard = value; }
        }

        public double Background
        {
            get { return background; }
            set { background = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public int Year
        {
            get { return year; }
            set { year = value; }
        }





    }
}
