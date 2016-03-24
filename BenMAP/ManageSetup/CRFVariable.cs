using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace BenMAP
{
    [Serializable]
    [ProtoContract]
    public class CRFVariable
    {
        [ProtoMember(1)]
        private int _variableID;
        [ProtoMember(2)]
        private int _functionID;
        [ProtoMember(3)]
        private string _variableName;
        [ProtoMember(4)]
        private string _pollutantName;
        [ProtoMember(5)]
        private int _pollutant1ID;
        [ProtoMember(6)]
        private int _pollutant2ID;
        [ProtoMember(7)]
        private Metric _metric;
        [ProtoMember(8)]
        private List<CRFBeta> _pollBetas;

        // parameterless constructor for serializer 
        public CRFVariable()
        {
            this._variableName = "";
            this._variableID = 0;
            this._functionID = 0;
            this._pollutantName = "";
            this._pollutant1ID = 0;
            this._pollutant2ID = 0;
            this._metric = null;
            this._pollBetas = new List<CRFBeta>();
        }

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
            this._metric = null;
        }

        public CRFVariable(string varName, int varID, int funID, string pollName, int poll1ID, Metric metric)
        {
            this._variableName = varName;
            this._variableID = varID;
            this._functionID = funID;
            this._pollutantName = pollName;
            this._pollutant1ID = poll1ID;
            this._metric = metric;
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
