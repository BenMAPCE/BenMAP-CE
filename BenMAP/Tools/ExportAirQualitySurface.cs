using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ExportAirQualitySurface : FormBase
    {
        public ExportAirQualitySurface()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                openFile.Filter = "Air Quality Surface(*.aqgx)|*.aqgx";
                openFile.FilterIndex = 1;
                openFile.RestoreDirectory = true;
                if (openFile.ShowDialog() != DialogResult.OK)
                { return; }
                txtAirQualitySurface.Text = openFile.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                BenMAPLine inputBenMAPLine = new BenMAPLine();
                string err = "";
                inputBenMAPLine = DataSourceCommonClass.LoadAQGFile(txtAirQualitySurface.Text, ref err);
                if (inputBenMAPLine == null)
                {
                    MessageBox.Show(err);
                    return;
                }
                SaveFileDialog saveCSV = new SaveFileDialog();
                saveCSV.Filter = "csv file(*.csv)|*.csv";
                saveCSV.FilterIndex = 2;
                saveCSV.RestoreDirectory = true;
                saveCSV.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                if (saveCSV.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string filePath = saveCSV.FileName.Substring(0, saveCSV.FileName.LastIndexOf(@"\") + 1);
                string fileName = saveCSV.FileName.Substring(saveCSV.FileName.LastIndexOf(@"\") + 1);
                DataSourceCommonClass.SaveModelDataLineToNewFormatCSV(inputBenMAPLine, saveCSV.FileName);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }
    }
}
