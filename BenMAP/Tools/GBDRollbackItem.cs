using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DotSpatial.Symbology;

namespace BenMAP
{
    public class GBDRollbackItem
    {
        public enum RollbackType { Percentage, Incremental, Standard }

        private string name;
        private string description;
        Dictionary<string,string> countries;
        private RollbackType type;
        private double percentage;
        private double increment;
        private string standardName;
        private int standardId;
        private double standard;
        private bool isNegativeRollbackToStandard;
        private double background;
        private Color color;
        private int year;
        private List<IPolygonCategory> ipcList=new List<IPolygonCategory>();

        public void addIPC(IPolygonCategory ipc)
        {
          
            ipcList.Add(ipc);
        }

        public List<IPolygonCategory> IpcList
        {
            get { return ipcList; }
        }

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

        public Dictionary<string,string> Countries
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

        public double Standard
        {
            get { return standard; }
            set { standard = value; }
        }

        public int StandardId
        {
            get { return standardId; }
            set { standardId = value; }
        }

        public string StandardName
        {
            get { return standardName; }
            set { standardName = value; }
        }

        public bool IsNegativeRollbackToStandard
        {
            get { return isNegativeRollbackToStandard; }
            set { isNegativeRollbackToStandard = value; }
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
