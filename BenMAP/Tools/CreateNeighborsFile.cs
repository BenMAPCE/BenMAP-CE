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
    public partial class CreateNeighborsFile : FormBase
    {
        public CreateNeighborsFile()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "aqg files (*.aqgx)|*.aqgx";
                openFile.FilterIndex = 2;
                openFile.RestoreDirectory = true;
                openFile.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAirQualitySurface.Text != "")
                {
                    string err = "";
                    BenMAPLine benMAPLine = DataSourceCommonClass.LoadAQGFile(txtAirQualitySurface.Text, ref err);
                    if (benMAPLine == null)
                    {
                        MessageBox.Show(err);
                        return;
                    }
                    if (benMAPLine is MonitorDataLine)
                    {
                        MonitorDataLine monitorDataLine = (benMAPLine as MonitorDataLine);
                        if (monitorDataLine.MonitorNeighbors != null && monitorDataLine.MonitorNeighbors.Count > 0)
                        {
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Filter = "Neighbors files (*.csv)|*.csv";
                            sfd.FilterIndex = 2;
                            sfd.RestoreDirectory = true;
                            sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                            string _filePath = "";
                            _filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);

                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                DataTable dt = new DataTable();
                                dt.Columns.Add("Col");
                                dt.Columns.Add("Row");
                                dt.Columns.Add("MonitorName");
                                dt.Columns.Add("Weight");
                                dt.Columns.Add("Distance");
                                foreach (MonitorNeighborAttribute m in monitorDataLine.MonitorNeighbors)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["Col"] = m.Col;
                                    dr["Row"] = m.Row;
                                    dr["MonitorName"] = m.MonitorName;
                                    dr["Weight"] = m.Weight;
                                    dr["Distance"] = m.Distance;
                                    dt.Rows.Add(dr);
                                }
                                CommonClass.SaveCSV(dt, sfd.FileName);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No neighbors in this air quality grid.");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This air quality grid is not a monitor direct grid.");
                        return;
                    }
                }

            }
            catch
            {
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

    }
}
