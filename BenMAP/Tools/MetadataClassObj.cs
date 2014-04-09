using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    public class MetadataClassObj
    {
        private int _setupId;
        private int _datasetId;
        private int _datasetTypeId;
        private int _metadataId;
        private string _fileName;
        private string _extension;
        private string _dataReference;
        private string _fileDate;
        private string _importDate;
        private string _description;
        private string _projection;
        private string _geoName;
        private string _datumName;
        private string _datumType;
        private string _spheroidName;
        private string _meridianName;
        private string _unitName;
        private string _proj4String;
        private string _numberOfFeatures;
        private string _setupName;



        public string SetupName
        {
            get { return _setupName; }
            set { _setupName = value; }
        }

        public string NumberOfFeatures
        {
            get { return _numberOfFeatures; }
            set { _numberOfFeatures = value; }
        }

        public string Proj4String
        {
            get { return _proj4String; }
            set { _proj4String = value; }
        }

        public string UnitName
        {
            get { return _unitName; }
            set { _unitName = value; }
        }

        public string MeridianName
        {
            get { return _meridianName; }
            set { _meridianName = value; }
        }

        public string SpheroidName
        {
            get { return _spheroidName; }
            set { _spheroidName = value; }
        }

        public string DatumType
        {
            get { return _datumType; }
            set { _datumType = value; }
        }

        public string DatumName
        {
            get { return _datumName; }
            set { _datumName = value; }
        }

        public string GeoName
        {
            get { return _geoName; }
            set { _geoName = value; }
        }

        public string Projection
        {
            get { return _projection; }
            set { _projection = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string ImportDate
        {
            get { return _importDate; }
            set { _importDate = value; }
        }

        public string FileDate
        {
            get { return _fileDate; }
            set { _fileDate = value; }
        }

        public string DataReference
        {
            get { return _dataReference; }
            set { _dataReference = value; }
        }

        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public int MetadataId
        {
            get { return _metadataId; }
            set { _metadataId = value; }
        }

        public int DatasetTypeId
        {
            get { return _datasetTypeId; }
            set { _datasetTypeId = value; }
        }
        
        public int DatasetId
        {
            get { return _datasetId; }
            set { _datasetId = value; }
        }
        
        public int SetupId
        {
            get { return _setupId; }
            set { _setupId = value; }
        }
    }
}
