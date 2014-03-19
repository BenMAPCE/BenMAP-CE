using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ESIL.DBUtility;


namespace BenMAP
{
    public partial class ValidateDatabaseImport : Form
    {
        private DataTable _tbl = null;
        private List<string> _colNames = null;
        private Dictionary<string,string> _dicTableDef = null;
        private string _datasetname = string.Empty;
        private string _file = string.Empty;
        //private bool _bPassed = true;
        private int errors = 0;
        private int warnings = 0;
         
        public ValidateDatabaseImport(DataTable tblToValidate, string datasetName, string selectedFile)
        {
            InitializeComponent();
            _tbl = tblToValidate;
            _datasetname = datasetName;
            _file = selectedFile;
            txtReportOutput.SelectionTabs = new int[] {120, 200, 350};
            txtReportOutput.SelectionIndent = 10;
            Get_ColumnNames();
            Get_DatasetDefinition();
        }
        //Getting column names from the passed in datatable as they are displayed in the file.
        private void Get_ColumnNames()
        {
          _colNames = new List<string>();
          for (int i = 0; i < _tbl.Columns.Count; i++)
          {
              _colNames.Add(_tbl.Columns[i].ColumnName.ToLower());
          }
        }
        //Getting dataset definition from the firebird database for the dataset name that was passed in.
        private void Get_DatasetDefinition()
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string cmdText = string.Empty;
            _dicTableDef = new Dictionary<string, string>();
            try
            {
                cmdText = string.Format("SELECT COLUMNNAME, DATATYPE FROM DATASETDEFININTION WHERE DATASETNAME='{0}'", _datasetname);
                DataTable _obj =   fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, cmdText).Tables[0] as DataTable;
                foreach(DataRow dr in _obj.Rows)
                {
                    _dicTableDef.Add(dr[0].ToString().ToLower(), dr[1].ToString());
                }
            }
            catch (Exception)
            {

                //throw;
            }
            
            

        }

        private void ValidateDatabaseImport_Load(object sender, EventArgs e)
        {
            txtReportOutput.Text += string.Format("Date:\t{0}\r\n",DateTime.Today.ToShortDateString());
            FileInfo fInfo = new FileInfo(_file);
            txtReportOutput.Text += string.Format("File Name:\t{0}\r\n\r\n\r\n", fInfo.Name);
            
            this.Refresh();
            Application.DoEvents();
            //txtReportOutput.Text += "Error/Warnings\t Row\t Column Name \t Error/Warning Message\r\n";
        }

        private void ValidateDatabaseImport_Shown(object sender, EventArgs e)
        {
            bool bPassed = true;
            Application.DoEvents();
            if(bPassed)
                bPassed = VarifyColumnNames();
            if(bPassed)
                bPassed = VarifyTableHasData();
            if(bPassed)
                bPassed = VarifyTableDataTypes();//errors

            txtReportOutput.Text += "\r\n\r\n\r\nSummary\r\n";
            if(errors > 0)
                txtReportOutput.Text += "-----\r\nValidation failed!\r\n";

            txtReportOutput.Text += string.Format("{0} errors\r\n{1} warnings\r\n", errors,warnings);
            SaveValidateResults();
            btnLoad.Enabled = bPassed;
        }
        private void SaveValidateResults()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files";
            string FileName = string.Format("{0}_{1}.rtf", _datasetname, DateTime.Today.ToFileTime());
            txtReportOutput.Text += string.Format("Saved to: {0}", path + string.Format(@"\{0}", FileName));
            File.WriteAllText(path + string.Format(@"\{0}", FileName), txtReportOutput.Text);
        }
        
        private bool VarifyColumnNames()
        {
            bool bPassed = true;
            txtReportOutput.Text += "Verifying Column Names\r\n\r\n";
            txtReportOutput.Text += "Error/Warnings\tRow\tColumn Name\tError/Warning Message\r\n";
            for (int i = 0; i < _colNames.Count; i++)
            {
                if (!_dicTableDef.ContainsKey(_colNames[i].ToLower().ToString()))
                {
                    txtReportOutput.Text += string.Format("Error\t\t{0}\t is not a valid column name for dataset {1}\r\n", _colNames[i].ToString(), _datasetname);
                    errors++;
                    bPassed = false;
                }
            }

            foreach(KeyValuePair<string,string> kvpEntry in _dicTableDef)
            {
                if(!_colNames.Contains(kvpEntry.Key.ToLower()))
                {
                    txtReportOutput.Text += string.Format("Error\t\t{0}\t column is missing for dataset {1}\r\n", kvpEntry.Key.ToString(), _datasetname);
                    errors++;
                    bPassed = false;
                }
            }

            if(!bPassed)
                txtReportOutput.Text +="\r\nValidation of columns failed.\r\nPlease check the column header and file.  The columns could be incorrect or the incorrect file was selected.\r\n";
            else
                txtReportOutput.Text += "\r\nValidation of columns passed.\r\n";

            return bPassed;
        }
        
        private bool VarifyTableHasData()
        {
            bool bPassed = true;
            if(_tbl.Rows.Count < 1)
            {
                txtReportOutput.Text += string.Format("Error\t\t\t\t\t No rows found in file for dataset {1}\r\n", "", _datasetname);
                errors++;
                bPassed = false;
            }

            return bPassed;
        }
    
        private bool VarifyTableDataTypes()
        {
            string errMsg = string.Empty;
            bool bPassed = true;
            //int count1 = 0,count2 = 0;
            lblProgress.Visible = true;
            progressBar1.Visible = true;

            progressBar1.Step = 1;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = _tbl.Rows.Count * _tbl.Columns.Count;
            progressBar1.Value = progressBar1.Minimum;
            txtReportOutput.Text += "\r\n\r\nVarifying data types.\r\n\r\n";
            txtReportOutput.Text += "Error/Warnings\t Row\t Column Name \t Error/Warning Message\r\n";
            foreach(DataRow dr in _tbl.Rows)
            {
               foreach(DataColumn dc in dr.Table.Columns)
               {
                    double tempVal;
                    string dataType = _dicTableDef[dc.ColumnName.ToLower()].ToString();
                    string someVal = dr[dc.ColumnName].ToString();

                    try
                    {
                        if (!string.IsNullOrEmpty(someVal))
                        {
                            if (!VarifyDataRowValues(dataType, dc.ColumnName, someVal, out errMsg))
                            {
                                txtReportOutput.Text += string.Format("Error\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr), dc.ColumnName, errMsg);
                                errors++;
                                bPassed = false;
                            }
                        }
                        progressBar1.PerformStep();
                        lblProgress.Text = Convert.ToString((int)((double)progressBar1.Value / progressBar1.Maximum * 100)) + "%";
                        lblProgress.Refresh();
                        this.Refresh();
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message + "\r\nCount 1: " + count1 + "     Count 2: " + count2);
                    }
               }
            }
            if(!bPassed)
                txtReportOutput.Text += "Validating Datatable values failed.";

            progressBar1.Visible = false;
            lblProgress.Text = "";
            lblProgress.Visible = false;

            return bPassed;
        }

        private bool VarifyDataRowValues(string dataType, string columnName, string valToV, out string errMsg)
        {
            
            string min = Get_Min(columnName, dataType);
            string max = Get_Max(columnName, dataType);
            string valToVarify = valToV;
            Regex regx = new Regex(@"^[^~!@#$%`^&*+]+$");

            bool bPassed = true;

            errMsg = string.Empty;
            switch (dataType)
            {
                case "string":
                    double tempVal;
                    if (double.TryParse(valToVarify, out tempVal))
                    {
                        valToVarify = tempVal.ToString();
                    }

                    if(!string.IsNullOrEmpty(min) && bPassed)
                    {
                        if(valToVarify.Length < Convert.ToInt32( min ))
                        {
                            errMsg = "Value has an invalid length, too short.";
                            bPassed = false;
                        }
                    }
                    if(!string.IsNullOrEmpty(max) && bPassed)
                    {
                        if (valToVarify.Length > Convert.ToInt32(max))
                        {
                            errMsg = "Value has an invalid length, too long.";
                            bPassed = false;
                        }
                    }

                    if(!regx.IsMatch(valToVarify) && bPassed)
                    {
                        errMsg = "Value has invalid characters.";
                        bPassed = false;
                    }

                    break;
                case "integer":
                    
                    int outVal = -1;
                    if(int.TryParse(valToVarify, out outVal))
                    {
                        if(!string.IsNullOrEmpty(min) && bPassed)
                        {
                            if(outVal < Convert.ToInt32(min))
                            {
                                errMsg = string.Format("Value is not within min({0}) range.", min);
                                bPassed = false;
                            }
                        }
                        if(!string.IsNullOrEmpty(max) && bPassed)
                        {
                            if(outVal > Convert.ToInt32(max))
                            {
                                errMsg = string.Format("Value is not within max({0}) range.", max);
                                bPassed = false;
                            }
                        }
                    }
                    else
                    {
                        errMsg = "Value is not a valid integer.";
                        bPassed = false;
                    }
                    break;
                    case "float":
                        float outValf = -1;
                        if(float.TryParse(valToVarify, out outValf))
                        {
                            if (!string.IsNullOrEmpty(min) && bPassed)
                            {
                                if (outValf < Convert.ToInt32(min))
                                {
                                    errMsg = string.Format("Value is not within min({0}) range.", min);
                                    bPassed = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(max) && bPassed)
                            {
                                if (outValf > Convert.ToInt32(max))
                                {
                                    errMsg = string.Format("Value is not within max({0}) range.", max);
                                    bPassed = false;
                                }
                            }
                        }
                        else
                        {
                            errMsg = "Value is not a valid float.";
                            bPassed = false;
                        }
                    break;
                    case "blob":
                    break;
                default:
                    break;
            }
            return bPassed;
        }

        private string Get_Min(string columnName, string dataType)
        {
            string cmdText = string.Format("SELECT LOWERLIMIT FROM DATASETDEFININTION where DATASETNAME='{0}' " +
                                            "and COLUMNNAME='{1}' and DATATYPE='{2}'", _datasetname, columnName, dataType);
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText).ToString();
            if(string.IsNullOrEmpty(obj))
                obj = string.Empty;
            return obj;
        }

        private string Get_Max(string columnName, string dataType)
        {
            string cmdText = string.Format("SELECT UPPERLIMIT FROM DATASETDEFININTION where DATASETNAME='{0}' " +
                                "and COLUMNNAME='{1}' and DATATYPE='{2}'", _datasetname, columnName, dataType);
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText).ToString();
            if (string.IsNullOrEmpty(obj))
                obj = string.Empty;
            return obj;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
