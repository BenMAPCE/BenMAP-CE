using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;

namespace BenMAP
{
    [Serializable]
    [ProtoContract]
    public class MetadataClassObj
    {
        private int _setupId;
        private int _datasetId;
        private int _datasetTypeId;
        private int _metadataEntryId;
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


        [ProtoMember(1)]
        public string SetupName
        {
            get { return _setupName; }
            set { _setupName = value; }
        }

        [ProtoMember(2)]
        public string NumberOfFeatures
        {
            get { return _numberOfFeatures; }
            set { _numberOfFeatures = value; }
        }

        [ProtoMember(3)]
        public string Proj4String
        {
            get { return _proj4String; }
            set { _proj4String = value; }
        }

        [ProtoMember(4)]
        public string UnitName
        {
            get { return _unitName; }
            set { _unitName = value; }
        }

        [ProtoMember(5)]
        public string MeridianName
        {
            get { return _meridianName; }
            set { _meridianName = value; }
        }

        [ProtoMember(6)]
        public string SpheroidName
        {
            get { return _spheroidName; }
            set { _spheroidName = value; }
        }

        [ProtoMember(7)]
        public string DatumType
        {
            get { return _datumType; }
            set { _datumType = value; }
        }

        [ProtoMember(8)]
        public string DatumName
        {
            get { return _datumName; }
            set { _datumName = value; }
        }

        [ProtoMember(9)]
        public string GeoName
        {
            get { return _geoName; }
            set { _geoName = value; }
        }

        [ProtoMember(10)]
        public string Projection
        {
            get { return _projection; }
            set { _projection = value; }
        }

        [ProtoMember(11)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [ProtoMember(12)]
        public string ImportDate
        {
            get { return _importDate; }
            set { _importDate = value; }
        }

        [ProtoMember(13)]
        public string FileDate
        {
            get { return _fileDate; }
            set { _fileDate = value; }
        }

        [ProtoMember(14)]
        public string DataReference
        {
            get { return _dataReference; }
            set { _dataReference = value; }
        }

        [ProtoMember(15)]
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        [ProtoMember(16)]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// Gets or sets the metadata identifier.
        /// This is the MetadataEntryId
        /// </summary>
        /// <value>The metadata identifier.</value>
        [ProtoMember(17)]
        public int MetadataEntryId
        {
            get { return _metadataEntryId; }
            set { _metadataEntryId = value; }
        }

        [ProtoMember(18)]
        public int DatasetTypeId
        {
            get { return _datasetTypeId; }
            set { _datasetTypeId = value; }
        }

        [ProtoMember(19)]
        public int DatasetId
        {
            get { return _datasetId; }
            set { _datasetId = value; }
        }

        [ProtoMember(20)]
        public int SetupId
        {
            get { return _setupId; }
            set { _setupId = value; }
        }
    }
}
