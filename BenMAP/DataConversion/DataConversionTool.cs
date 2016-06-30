using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESIL.DBUtility;
using BenMAP;
using BenMAP.DataConversion;

namespace DataConversion
{
    public partial class DataConversionTool : FormBase
    {
        public DataConversionTool()
        {
            InitializeComponent();
        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFilePathInput.Text = openFileDialog1.FileName;

                if (!validateInputColumns())
                {
                    MessageBox.Show("Invalid column names in the input file. Please click\n" +
                        "the [?] button above for instructions on format.");
                    txtFilePathInput.Clear();
                }
            }
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFilePathOutput.Text = saveFileDialog1.FileName;
            }
        }

        private List<String> getColumnNamesFromFile()
        {
            List<String> resultList = new List<String>();

            String inputPath = txtFilePathInput.Text.Trim();
            if (!File.Exists(inputPath))
            {
                return resultList; // empty list if file isn't there
            }

            using (StreamReader sr = new StreamReader(inputPath))
            {
                String line = sr.ReadLine();
                String[] fields = line.Split(',');

                foreach(String s in fields)
                {
                    resultList.Add(s);
                }
            }

            return resultList;
        }

        private List<String> getColumnNamesFromDB()
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            String commandText = "select COLUMNNAME from DATASETDEFINITION where DATASETTYPENAME='Monitor'";
            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

            List<String> colNames = new List<String>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                colNames.Add(dr["COLUMNNAME"].ToString());
            }

            // Column names from DB will represent the converted columns
            // We need to check the input columns to make sure it will convert correctly
            if(colNames.Count() == 8)
            {
                colNames.RemoveAt(7); // remove Values
                colNames.Add("Date");
                colNames.Add("Value");
            }

            return colNames;
        }

        private Boolean validateInputColumns()
        {
            Boolean result = true;
            List<String> colsFromDB = getColumnNamesFromDB();
            List<String> colsFromFile = getColumnNamesFromFile();

            if (colsFromDB.Count() != colsFromFile.Count) return false;

            int i = 0;
            foreach(String s in colsFromDB)
            {
                if (!s.Equals(colsFromFile[i])) result = false;
                i++;
            }

            return result;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            int lineNum = 0;
            try
            {
                txtStatus.Clear();
                txtStatus.AppendText("Begin Conversion...");

                //Monitor Name,Monitor Description,Latitude,Longitude,Metric,Seasonal Metric,Statistic,Date,Value
                DataTable dt = new DataTable();
                dt.Columns.Add("MonitorName",typeof(String));
                dt.Columns.Add("MonitorDescription",typeof(String));
                dt.Columns.Add("Latitude",typeof(String));
                dt.Columns.Add("Longitude",typeof(String));
                dt.Columns.Add("Metric",typeof(String));
                dt.Columns.Add("SeasonalMetric",typeof(String));
                dt.Columns.Add("Statistic",typeof(String));
                dt.Columns.Add("Date",typeof(DateTime));
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

                //sort data by monitor name, date
                DataView dv = new DataView(dt);
                dv.Sort = "MonitorName ASC, Date ASC";
                dt = dv.ToTable();

                string outputPath = txtFilePathOutput.Text.Trim();
                using (StreamWriter sw = new StreamWriter(outputPath))
                {
                    string outputLine = "Monitor Name,Monitor Description,Latitude,Longitude,Metric,Seasonal Metric,Statistic,Values";
                    sw.WriteLine(outputLine);
                    string monitorName = "";
                    string monitorNameNext = "";
                    DateTime year = DateTime.MinValue;
                    int numValues = 365;
                    string[] arrValues = null;
                    string values = "";
                    List<object> listOutputLine = null;

                    //set up first monitor
                    if (dt.Rows.Count > 0)
                    {                      

                        monitorName = dt.Rows[0]["MonitorName"].ToString();
                        //determine year based on date in first row
                        year = DateTime.Parse(dt.Rows[0]["Date"].ToString());
                        if (DateTime.IsLeapYear(year.Year))
                        {
                            //add day for leap year
                            numValues++;
                        }
                        arrValues = new string[numValues];
                        //set default values
                        for (int i = 0; i < arrValues.Length; i++)
                        {
                            arrValues[i] = ".";
                        }

                        //set up list for output line
                        listOutputLine = dt.Rows[0].ItemArray.ToList<object>();
                        //remove value field
                        listOutputLine.RemoveAt(8);
                        //remove date field
                        listOutputLine.RemoveAt(7); 
                    }

                    foreach (DataRow dr in dt.Rows)
                    {                          
                        //build list of values
                        DateTime date = DateTime.Parse(dr["Date"].ToString());
                        string value = dr["Value"].ToString();
                        double valTest;
                        if (Double.TryParse(value, out valTest))
                        { 
                            //get value index
                            int valIndex = date.DayOfYear - 1;
                            arrValues[valIndex] = value;
                        }

                        //get monitor name
                        monitorNameNext = dr["MonitorName"].ToString();
                        if (!monitorNameNext.Equals(monitorName, StringComparison.OrdinalIgnoreCase))
                        {
                            //we are on new monitor so print current monitor                        
                            //add array of values
                            values = "\"" + string.Join(",", arrValues) + "\"";
                            listOutputLine.Add(values);
                            //write output line
                            outputLine = string.Join(",", listOutputLine);
                            sw.WriteLine(outputLine);

                            //set up new monitor and its values
                            monitorName = monitorNameNext;
                            arrValues = new string[numValues];
                            //set default values
                            for (int i = 0; i < arrValues.Length; i++)
                            {
                                arrValues[i] = ".";
                            }

                            //set up list for output line of new monitor
                            listOutputLine = dr.ItemArray.ToList<object>();
                            //remove value field
                            listOutputLine.RemoveAt(8);
                            //remove date field
                            listOutputLine.RemoveAt(7); 

                        }                        
                    }
                    //print last monitor
                    //add array of values
                    values = "\"" + string.Join(",", arrValues) + "\"";
                    listOutputLine.Add(values);
                    //write output line
                    outputLine = string.Join(",", listOutputLine);
                    sw.WriteLine(outputLine);
                    
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

        private void button1_Click(object sender, EventArgs e)
        {
            DataConversionInstructions form = new DataConversionInstructions();
            form.ShowDialog();
        }
    }
}
