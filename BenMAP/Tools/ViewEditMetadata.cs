using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ViewEditMetadata : Form
    {   
        private FileInfo _fInfo = null;
        public ViewEditMetadata()
        {
            InitializeComponent();
        }
        public ViewEditMetadata(string fileName): this()
        {
            _fInfo = new FileInfo(fileName);
        }

        private void btnSaveMetaData_Click(object sender, EventArgs e)
        {

        }

        private void ViewEditMetadata_Shown(object sender, EventArgs e)
        {
            txtFileName.Text = _fInfo.Name.Substring(0,_fInfo.Name.Length - _fInfo.Extension.Length);
            txtExtension.Text = _fInfo.Extension;
            txtFileDate.Text = _fInfo.CreationTime.ToShortDateString();
            txtImportDate.Text = DateTime.Today.ToShortDateString();
        }
    }
}
