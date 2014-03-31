﻿using System;
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
        private MetadataClassObj _metadataObj = null;
        private Metadata metadata = null;

        public MetadataClassObj MetadataObj
        {
            get { return _metadataObj; }
        }

        public ViewEditMetadata()
        {
            InitializeComponent();
        }

        public ViewEditMetadata(string fileName)
            : this()
        {
            _fInfo = new FileInfo(fileName);
            metadata = new Metadata(fileName);
            _metadataObj = metadata.GetMetadata();
        }

        public ViewEditMetadata(string fileName, MetadataClassObj metadataClsObj) : this()
        {
            _fInfo = new FileInfo(fileName);
            metadata = new Metadata(fileName);
            _metadataObj = metadataClsObj;
        }

        private void ViewEditMetadata_Shown(object sender, EventArgs e)
        {
            txtSetupID.Text = _metadataObj.SetupId.ToString();
            txtSetupName.Text = _metadataObj.SetupName;
            txtFileName.Text = _metadataObj.FileName;
            txtExtension.Text = _metadataObj.Extension;
            txtFileDate.Text = _metadataObj.FileDate;
            txtImportDate.Text = _metadataObj.ImportDate;
            txtReference.Text = _metadataObj.DataReference;
            rtbDescription.Text = _metadataObj.Description;
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
            AddLabelandTextbox("Name", _metadataObj.GeoName);
            AddLabelandTextbox("Number of Features", _metadataObj.NumberOfFeatures);
            AddLabelandTextbox("Proj4String", _metadataObj.Proj4String);
            AddLabelandTextbox("Datum", _metadataObj.DatumName);
            AddLabelandTextbox("Datum Type", _metadataObj.DatumType);
            AddLabelandTextbox("Spheroid", _metadataObj.SpheroidName);
            AddLabelandTextbox("Meridian", _metadataObj.MeridianName);
            AddLabelandTextbox("Unit", _metadataObj.UnitName);
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

        private void btnOK_Click(object sender, EventArgs e)
        {

        }

        private void txtReference_TextChanged(object sender, EventArgs e)
        {
            _metadataObj.DataReference = txtReference.Text;
        }

        private void rtbDescription_TextChanged(object sender, EventArgs e)
        {
            _metadataObj.Description = rtbDescription.Text;
        }
        
    }
}