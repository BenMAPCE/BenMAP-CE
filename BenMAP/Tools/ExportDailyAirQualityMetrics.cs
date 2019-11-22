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
    public partial class ExportDailyAirQualityMetrics : FormBase
    {
        public ExportDailyAirQualityMetrics()
        {
            InitializeComponent();
            txtAirQualitySurface.Text = CommonClass.ResultFilePath;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog outputFolder = new FolderBrowserDialog();
                outputFolder.SelectedPath = CommonClass.ResultFilePath;
                if (outputFolder.ShowDialog() != DialogResult.OK)
                { return; }
                txtAirQualitySurface.Text = outputFolder.SelectedPath;
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
                btnOK.Enabled = false;
                btnCancel.Enabled = false;

                //First, cleanup the monitor metrics and recalculate the daily and seasonal metrics
                if (CommonClass.LstBaseControlGroup[0].Base is MonitorDataLine)
                {
                    DataSourceCommonClass.UpdateModelAttributesMonitorData_Multipollutant();
                }
                string sTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                //For each pollutant, export daily metrics for baseline and control
                foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                {
                    for (int iBaseControlToggle = 0; iBaseControlToggle <= 1; iBaseControlToggle++) //0=base, 1=control
                    {
                        MonitorDataLine mdl = (MonitorDataLine)(iBaseControlToggle == 0 ? bcg.Base : bcg.Control);
                        string fileName = string.Format("{0}_{1}_{2}.csv", mdl.Pollutant.PollutantName, iBaseControlToggle == 0 ? "Baseline" : "Control", sTimestamp);
                        DataSourceCommonClass.SaveDailyModelMetricsToCSV(mdl, txtAirQualitySurface.Text + "\\" + fileName);
                    }
                }
                MessageBox.Show("Export is complete.");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }
    }
}
