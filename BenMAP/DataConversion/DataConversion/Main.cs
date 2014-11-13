using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataConversion
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFilePathInput.Text = openFileDialog1.FileName;
            }

        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFilePathOutput.Text = saveFileDialog1.FileName;
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            int lineNum = 0;
            try
            {
                txtStatus.AppendText("Begin Conversion...");



                //Monitor Name,Monitor Description,Latitude,Longitude,Metric,Seasonal Metric,Statistic,Date,Value
                DataTable dt = new DataTable();
                dt.Columns.Add("Monitor Name",typeof(String));
                dt.Columns.Add("Monitor Description",typeof(String));
                dt.Columns.Add("Latitude",typeof(String));
                dt.Columns.Add("Longitude",typeof(String));
                dt.Columns.Add("Metric",typeof(String));
                dt.Columns.Add("Seasonal Metric",typeof(String));
                dt.Columns.Add("Statistic",typeof(String));
                dt.Columns.Add("Date",typeof(String));
                dt.Columns.Add("Value",typeof(String));

                //open files
                string inputPath = txtFilePathInput.Text.Trim();
                if (!File.Exists(inputPath))
                {
                    return;
                }


                
                using (StreamReader sr = new StreamReader(inputPath))
                {
                    sr.ReadLine(); //skip header line
                    lineNum++;

                    while (sr.Peek() >= 0)
                    {                        
                        string inputLine = sr.ReadLine();
                        lineNum++;

                        //fix monitor description, replace commas with bars
                        //so we can split fields on CSV
                        int startIndex = inputLine.IndexOf('"');
                        int endIndex = inputLine.IndexOf('"', startIndex + 1);
                        string monitorDescription = inputLine.Substring(startIndex, endIndex - startIndex);
                        string monitorDescriptionNew = monitorDescription.Replace(",", "|");

                        inputLine = inputLine.Replace(monitorDescription, monitorDescriptionNew);                    

                        string [] arrInputLine = inputLine.Split(',');
                        arrInputLine[1] = arrInputLine[1].Replace("|", ","); //put back commas in monitor description
                        dt.Rows.Add(arrInputLine);
                    }
                }

                int hello = 0;
                hello = 1;
                string outputPath = txtFilePathOutput.Text.Trim();
                using (StreamWriter sw = new StreamWriter(outputPath))
                {
                    string outputLine = "Monitor Name,Monitor Description,Latitude,Longitude,Metric,Seasonal Metric,Statistic,Date,Value";
                    sw.WriteLine(outputLine);
                    foreach (DataRow dr in dt.Rows)
                    {
                        outputLine = string.Join(",", dr.ItemArray);
                        sw.WriteLine(outputLine);
                    }
                    
                }

                txtStatus.AppendText(Environment.NewLine);
                txtStatus.AppendText("Conversion Complete!");


            }
            catch (Exception ex)
            {
                txtStatus.AppendText(Environment.NewLine);
                txtStatus.AppendText("Conversion Failed! Line " + lineNum.ToString());
            }

            
        }
    }
}
