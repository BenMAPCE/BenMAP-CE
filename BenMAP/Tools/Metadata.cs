using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;

namespace BenMAP
{
    class Metadata
    {
        private MetadataClassObj metadataObj;
        private FileInfo _fInfo = null;
        public Metadata(string filePath)
        {
            _fInfo = new FileInfo(filePath);
            metadataObj = new MetadataClassObj();
        }

        public MetadataClassObj GetMetadata()
        {
            //txtDatabase.Text

            metadataObj.SetupId = Convert.ToInt32(CommonClass.ManageSetup.SetupID.ToString());
            metadataObj.SetupName = CommonClass.MainSetup.SetupName.ToString();
            metadataObj.FileName = _fInfo.Name.Substring(0, _fInfo.Name.Length - _fInfo.Extension.Length);
            metadataObj.Extension = _fInfo.Extension;
            metadataObj.FileDate = _fInfo.CreationTime.ToShortDateString();
            metadataObj.ImportDate = DateTime.Today.ToShortDateString();

            if (_fInfo.Extension == ".shp")
            {
                Readin_shp_Metadata();
            }

            //if (_fInfo.Extension == ".csv")
            //{
            //    Readin_csv_Metadata();
            //}

            return metadataObj;
        }
        private void Readin_shp_Metadata()
        {
            string projection = string.Empty;
            string geoname = string.Empty;
            string datumName = string.Empty;
            string datumType = string.Empty;
            string spheroidName = string.Empty;
            string meridianName = string.Empty;
            string unitName = string.Empty;
            string proj4string = string.Empty;
            string numberOfFeatures = string.Empty;

            IFeatureSet ifs = FeatureSet.Open(_fInfo.FullName);

            ProjectionInfo proInfo = ifs.Projection;
            GeographicInfo geoInfo = proInfo.GeographicInfo;
            Datum datumInfo = geoInfo.Datum;
            Meridian meridianInfo = geoInfo.Meridian;
            Spheroid spheroidInfo = datumInfo.Spheroid;

            metadataObj.Proj4String = proInfo.ToProj4String();
            metadataObj.GeoName = geoInfo.Name;
            metadataObj.DatumName = datumInfo.Name;
            metadataObj.DatumType = datumInfo.DatumType.ToString();
            metadataObj.SpheroidName = spheroidInfo.Name;
            metadataObj.MeridianName = meridianInfo.Name;
            metadataObj.UnitName = geoInfo.Unit.Name;
            metadataObj.NumberOfFeatures = ifs.Features.Count.ToString();

            //AddLabelandTextbox("Name", geoname);
            //AddLabelandTextbox("Number of Features", ifs.Features.Count.ToString());
            //AddLabelandTextbox("Proj4String", proj4string);
            //AddLabelandTextbox("Datum", datumName);
            //AddLabelandTextbox("Datum Type", datumType);
            //AddLabelandTextbox("Spheroid", spheroidName);
            //AddLabelandTextbox("Meridian", meridianName);
            //AddLabelandTextbox("Unit", unitName);
        }
    }
}
