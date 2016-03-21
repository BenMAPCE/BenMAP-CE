using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class CRFVariable
    {

        private int _variableID;
        private int _functionID;
        private string _variableName;
        private string _pollutantName;
        private int _pollutant1ID;
        private int _pollutant2ID;
        private Metric _metric;
        private List<CRFBeta> _pollBetas;

        // Constructor for new/ edited functions
        public CRFVariable(string varName, string pollName, int poll1ID)
        {
            this._variableName = varName;
            this._pollutantName = pollName;
            this._pollutant1ID = poll1ID;
        }

        public CRFVariable(string varName, int varID, int funID, string pollName, int poll1ID)
        {
            this._variableName = varName;
            this._variableID = varID;
            this._functionID = funID;
            this._pollutantName = pollName;
            this._pollutant1ID = poll1ID;
        }

        public CRFVariable(string varName, int varID, int funID, string pollName, int poll1ID, int poll2ID)
        {
            this._variableName = varName;
            this._variableID = varID;
            this._functionID = funID;
            this._pollutantName = pollName;
            this._pollutant1ID = poll1ID;
            this._pollutant2ID = poll2ID;
        }

        public CRFVariable(string varName, int varID, int funID, string pollName, int poll1ID, string metric, int metricID)
        {
            this._variableName = varName;
            this._variableID = varID;
            this._functionID = funID;
            this._pollutantName = pollName;
            this._pollutant1ID = poll1ID;
        }

        public CRFVariable(string varName, int varID, int funID, string pollName, int poll1ID, int poll2ID, string metric, int metricID)
        {
            this._variableName = varName;
            this._variableID = varID;
            this._functionID = funID;
            this._pollutantName = pollName;
            this._pollutant1ID = poll1ID;
            this._pollutant2ID = poll2ID;
        }

        public int VariableID
        {
            get { return _variableID; }
            set { _variableID = value; }
        }

        public int FunctionID
        {
            get { return _functionID; }
            set { _functionID = value; }
        }

        public Metric Metric
        {
            get { return _metric; }
            set { _metric = value; }
        }      

        public string VariableName
        {
            get { return _variableName; }
            set { _variableName = value; }
        }

        public string PollutantName
        {
            get { return _pollutantName; }
            set { _pollutantName = value; }
        }

        public int Pollutant1ID
        {
            get { return _pollutant1ID; }
            set { _pollutant1ID = value; }
        }

        public int Pollutant2ID
        {
            get { return _pollutant2ID; }
            set { _pollutant2ID = value; }
        }

        public List<CRFBeta> PollBetas
        {
            get { return _pollBetas; }
            set { _pollBetas = value; }
        }
    }
}
