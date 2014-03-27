using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;

namespace BenMAP
{
    public partial class ViewEditMetadata : Form
    {   
        private FileInfo _fInfo = null;
        private MetadataClassObj metadataObj = null;
        private Metadata metadata = null;
        public ViewEditMetadata()
        {
            InitializeComponent();
        }
        public ViewEditMetadata(string fileName): this()
        {
            _fInfo = new FileInfo(fileName);
            metadata = new Metadata(fileName);
            metadataObj = metadata.GetMetadata();
        }

        private void ViewEditMetadata_Shown(object sender, EventArgs e)
        {
            txtSetupID.Text = metadataObj.SetupId.ToString(); //CommonClass.MainSetup.SetupID.ToString();
            txtSetupName.Text = metadataObj.SetupName; //CommonClass.MainSetup.SetupName.ToString();
            txtFileName.Text = metadataObj.FileName;// _fInfo.Name.Substring(0,_fInfo.Name.Length - _fInfo.Extension.Length);
            txtExtension.Text = metadataObj.Extension;// _fInfo.Extension;
            txtFileDate.Text = metadataObj.FileDate;// _fInfo.CreationTime.ToShortDateString();
            txtImportDate.Text = metadataObj.ImportDate;// DateTime.Today.ToShortDateString();
            if(_fInfo.Extension == ".shp")
            {
                LoadShapeInfo();
            }

            if(_fInfo.Extension == ".csv")
            {
                LoadCSVInof();
            }

        }

        private void LoadShapeInfo()
        {
            AddLabelandTextbox("Name", metadataObj.GeoName);
            AddLabelandTextbox("Number of Features", metadataObj.NumberOfFeatures);
            AddLabelandTextbox("Proj4String", metadataObj.Proj4String);
            AddLabelandTextbox("Datum",metadataObj.DatumName);
            AddLabelandTextbox("Datum Type",metadataObj.DatumType);
            AddLabelandTextbox("Spheroid",metadataObj.SpheroidName);
            AddLabelandTextbox("Meridian", metadataObj.MeridianName);
            AddLabelandTextbox("Unit", metadataObj.UnitName);
            this.Size = new Size(382, this.Size.Height + (40 * 8));
        }

        private void LoadCSVInof()
        {
            this.Size = new Size(382, 390);
        }

        private void AddLabelandTextbox(string name, string value)
        {
            Label lbl = new Label();
            TextBox txtbox = new TextBox();
            try
            {
                lbl.Text = name;
                lbl.Name = string.Format("lbl{0}", name);
                lbl.Margin = new System.Windows.Forms.Padding(7);
                lbl.AutoSize = true;

                txtbox.Text = value;
                txtbox.Name = string.Format("txt{0}", name);
                txtbox.Margin = new System.Windows.Forms.Padding(4);
                txtbox.Size = new System.Drawing.Size(215, 20);


                flpLabels.Controls.Add(lbl);
                flbTextBoxes.Controls.Add(txtbox);
                flpLabels.Refresh();
                flbTextBoxes.Refresh();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
        }
        
    }
}
