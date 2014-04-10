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
        private MetadataClassObj _metadataObj = null;
        private Metadata metadata = null;
        private bool bDataChanged = false;
        private bool bEditMode = false;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEditMetadata"/> class.
        /// Use this for viewing metadata that was saved to the database.
        /// </summary>
        /// <param name="metaObj">The meta object.</param>
        public ViewEditMetadata(MetadataClassObj metaObj): this()
        {
            _metadataObj = metaObj;
            bEditMode = true;
        }

        public ViewEditMetadata(string fileName, MetadataClassObj metadataClsObj) : this()
        {
            _fInfo = new FileInfo(fileName);
            metadata = new Metadata(fileName);
            _metadataObj = metadataClsObj;
            //btnSaveMetaData.Visible = true;
            //btnSaveMetaData.Enabled = false;
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
            if(_fInfo != null)//if null I am pulling information out of the Database
            {
                if(_fInfo.Extension.ToLower() == ".shp")
                {
                    LoadShapeInfo();
                }

                if (_fInfo.Extension.ToLower() == ".csv")
                {
                    LoadCSVInof();
                }
            }
            else if (_metadataObj.Extension != null)
            {
                if (_metadataObj.Extension.ToLower() == ".shp")
                {
                    LoadShapeInfo();
                }
                else if (_metadataObj.Extension.ToLower() == ".csv" || _metadataObj.Extension == null)
                {
                    LoadCSVInof();
                }
            }
            else
            {
                LoadCSVInof();//default
            }
            if(bEditMode)
            {
                bDataChanged = false;
                btnSaveMetaData.Visible = true;
                btnSaveMetaData.Enabled = false;
            }
        }

        private void LoadShapeInfo()
        {
            lblName.Visible = true;
            txtGeoName.Visible = true;
            txtGeoName.Text = _metadataObj.GeoName;

            lblNumOfFeatures.Visible = true;
            txtNumOfFeatures.Visible = true;
            txtNumOfFeatures.Text = _metadataObj.NumberOfFeatures;

            lblProj4String.Visible = true;
            txtProj4String.Visible = true;
            txtProj4String.Text = _metadataObj.Proj4String;

            lblDatum.Visible = true;
            txtDatum.Visible = true;
            txtDatum.Text = _metadataObj.DatumName;

            lblDatumType.Visible = true;
            txtDatumType.Visible = true;
            txtDatumType.Text = _metadataObj.DatumType;

            lblSpheroid.Visible = true;
            txtSpheroid.Visible = true;
            txtSpheroid.Text = _metadataObj.SpheroidName;

            lblMedia.Visible = true;
            txtMedian.Visible = true;
            txtMedian.Text = _metadataObj.MeridianName;

            lblUnit.Visible = true;
            txtUnit.Visible = true;
            txtUnit.Text  = _metadataObj.UnitName;
        }

        private void LoadCSVInof()
        {
            this.Size = new Size(405, 460);
        }

        //private void AddLabelandTextbox(string name, string value)
        //{
        //    Label lbl = new Label();
        //    TextBox txtbox = new TextBox();
        //    try
        //    {
        //        lbl.Text = name;
        //        lbl.Name = string.Format("lbl{0}", name);
        //        lbl.Margin = new System.Windows.Forms.Padding(7);
        //        lbl.AutoSize = true;

        //        txtbox.Text = value;
        //        txtbox.Name = string.Format("txt{0}", name);
        //        txtbox.Margin = new System.Windows.Forms.Padding(4);
        //        txtbox.BorderStyle = BorderStyle.None;
        //        txtbox.BackColor = System.Drawing.SystemColors.Control;
        //        txtbox.Enabled = false;
        //        txtbox.ReadOnly = true;
        //        txtbox.Size = new System.Drawing.Size(239, 20);


        //        flpLabels.Controls.Add(lbl);
        //        flbTextBoxes.Controls.Add(txtbox);
        //        flpLabels.Refresh();
        //        flbTextBoxes.Refresh();
        //    }
        //    catch (Exception ex)
        //    {
        //       MessageBox.Show(ex.Message);
        //    }
        //}

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(bDataChanged && bEditMode)//If the data changed and I am in edit mode then prompted to save
            {
                DialogResult dlr = MessageBox.Show("Do you want to save your changes?","Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                if(dlr.Equals(DialogResult.Yes))
                {
                    saveMetadata();
                    this.DialogResult = DialogResult.OK;
                }
                if(dlr.Equals(DialogResult.No))
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                //else just exit out.  The changes will auto saved when the new dataset is added.
                this.DialogResult = DialogResult.OK;
            }
        }

        private void txtReference_TextChanged(object sender, EventArgs e)
        {
            _metadataObj.DataReference = txtReference.Text;
            SetSaveButton(true);
        }

        private void rtbDescription_TextChanged(object sender, EventArgs e)
        {
            _metadataObj.Description = rtbDescription.Text;
            SetSaveButton(true);
        }

        private void SetSaveButton(bool tf)
        {
            bDataChanged = tf;
            btnSaveMetaData.Enabled = tf;
        }
        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSaveMetaData_Click(object sender, EventArgs e)
        {
            saveMetadata();
            SetSaveButton(false);
        }

        private void saveMetadata()
        {
            SQLStatementsCommonClass.updateMetadata(_metadataObj);
        }
    }
}
