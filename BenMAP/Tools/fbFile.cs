using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Newtonsoft.Json;


namespace BenMAP
{
    class fbFile
    {
        public long fbFileID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public double FileSize { get; set; }
        public string CreatedBy { get; set; }
        public string Company { get; set; }
        public string FileDescription { get; set; }
        public DateTime Uploaded { get; set; }
        public string BenMAPVersion { get; set; }
        public long Downloads { get; set; }
        public byte[] Streamified { get; set; }

        public fbFile()
        {

        }

        public fbFile(string filename, string createdby, string company, string filedesc, DateTime uploaded, string benmapversion, int downloads, MemoryStream filestream)
        {
            this.FileName = filename;
            this.FilePath = string.Empty;
            this.FileSize = filestream.Length;
            this.CreatedBy = createdby;
            this.Company = company;
            this.FileDescription = filedesc;
            this.Uploaded = uploaded;
            this.BenMAPVersion = benmapversion;
            this.Downloads = downloads;
            this.Streamified = filestream.ToArray();
        }

        public fbFile(string filename, string createdby, string company, string filedesc, DateTime uploaded, string benmapversion, int downloads, byte[] filestream)
        {
            this.FileName = filename;
            this.FilePath = string.Empty;
            this.FileSize = filestream.Length;
            this.CreatedBy = createdby;
            this.Company = company;
            this.FileDescription = filedesc;
            this.Uploaded = uploaded;
            this.BenMAPVersion = benmapversion;
            this.Downloads = downloads;
            this.Streamified = filestream;
        }
    }

    class fbFileCollection
    {
        public Dictionary<string, fbFile> fbFiles { get; set; }
    }
}
