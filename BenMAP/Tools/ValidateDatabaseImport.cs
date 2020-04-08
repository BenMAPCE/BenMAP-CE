// ***********************************************************************
// Assembly         : BenMAP
// Author           :
// Created          : 03-17-2014
//
// Last Modified By : Adam Shelton
// Last Modified On : 11-03-2014
// ***********************************************************************
// <copyright file="ValidateDatabaseImport.cs" company="RTI International">
//     RTI International. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
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


/// <summary>
/// The BenMAP namespace.
/// </summary>
namespace BenMAP
{
	/// <summary>
	/// Class ValidateDatabaseImport
	/// </summary>
	public partial class ValidateDatabaseImport : Form
	{

		TipFormGIF waitMess = new TipFormGIF(); bool sFlog = true;
		/// <summary>
		/// The _TBL
		/// </summary>
		private DataTable _tbl = null;
		/// <summary>
		/// The _col names
		/// </summary>
		private List<string> _colNames = null;
		/// <summary>
		/// The _dic table definition
		/// </summary>
		//private Dictionary<string,string> _dicTableDef = null;
		private Hashtable _hashTableDef = null;
		private Hashtable _hashTableEPG = null;//Endpointgroups lookup table.
		private Hashtable _hashTableEPS = null; //Endpoints Lookup table.
		private Hashtable _hashTableRace = null; //Race Lookup table.
		private Hashtable _hashTableGender = null;//Genders Lookup table.
		private Hashtable _hashTableEthnicity = null;//Ethnicity lookup table.
		private Hashtable _hashTableAgeRange = null;//Ageranges lookup tabel.

		/// <summary>
		/// The _datasetname
		/// </summary>
		private string _datasetname = string.Empty;
		/// <summary>
		/// The _file
		/// </summary>
		private string _file = string.Empty;
		//private bool _bPassed = true;
		/// <summary>
		/// The errors
		/// </summary>
		private int errors = 0;
		/// <summary>
		/// The warnings
		/// </summary>
		private int warnings = 0;

		private bool _passedValidation = true;

		public bool PassedValidation
		{
			get { return _passedValidation; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateDatabaseImport"/> class.
		/// </summary>
		/// <param name="tblToValidate">The table to validate.</param>
		/// <param name="datasetName">Name of the dataset.</param>
		/// <param name="selectedFile">The selected file.</param>
		public ValidateDatabaseImport(DataTable tblToValidate, string datasetName, string selectedFile) : this()
		{
			_tbl = tblToValidate;
			_tbl.CaseSensitive = false;

			var rowsToDelete = new List<DataRow>();         //BenMAP 441/442/444--Begin validation process with a "clean" datatable--one that has no blank rows or leading/trailing white spaces
			foreach (DataRow dr in _tbl.Rows)
			{
				if (String.IsNullOrEmpty(dr[0].ToString()))
				{
					rowsToDelete.Add(dr);
				}
				dr[0].ToString().Trim();
				dr[1].ToString().Trim();
			}

			rowsToDelete.ForEach(x => _tbl.Rows.Remove(x));

			_datasetname = datasetName;
			_file = selectedFile;
			txtReportOutput.SelectionTabs = new int[] { 120, 200, 350 };
			txtReportOutput.SelectionIndent = 10;
			Get_ColumnNames();
			Get_DatasetDefinition();
			Get_EndPointGroup();
			Get_EndPoints();
			Get_Race();
			Get_Genders();
			Get_Ethnicity();
			Get_AgeRange();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateDatabaseImport"/> class.
		/// </summary>
		public ValidateDatabaseImport()
		{
			InitializeComponent();
		}
		//Getting column names from the passed in datatable as they are displayed in the file.
		/// <summary>
		/// Get_s the column names.
		/// </summary>
		private void Get_ColumnNames()
		{
			_colNames = new List<string>();
			for (int i = 0; i < _tbl.Columns.Count; i++)
			{
				_colNames.Add(_tbl.Columns[i].ColumnName);
			}
		}
		//Getting dataset definition from the firebird database for the dataset name that was passed in.
		/// <summary>
		/// Get_s the dataset definition.
		/// </summary>
		private void Get_DatasetDefinition()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string cmdText = string.Empty;

			_hashTableDef = new Hashtable(StringComparer.OrdinalIgnoreCase);
			// _dicTableDef = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			try
			{
				cmdText = string.Format("SELECT COLUMNNAME, DATATYPE, REQUIRED, LOWERLIMIT, UPPERLIMIT, CHECKTYPE FROM DATASETDEFINITION WHERE DATASETTYPENAME='{0}'", _datasetname);
				DataTable _obj = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, cmdText).Tables[0] as DataTable;
				foreach (DataRow dr in _obj.Rows)
				{

					//_dicTableDef.Add(dr[0].ToString(), dr[1].ToString());
					_hashTableDef.Add(dr[0].ToString() + "##COLUMNNAME", dr[0].ToString());
					_hashTableDef.Add(dr[0].ToString() + "##DATATYPE", dr[1].ToString());
					_hashTableDef.Add(dr[0].ToString() + "##REQUIRED", dr[2].ToString());
					_hashTableDef.Add(dr[0].ToString() + "##LOWERLIMIT", dr[3].ToString());
					_hashTableDef.Add(dr[0].ToString() + "##UPPERLIMIT", dr[4].ToString());
					_hashTableDef.Add(dr[0].ToString() + "##CHECKTYPE", dr[5].ToString());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

		private void Get_EndPointGroup()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			_hashTableEPG = new Hashtable(StringComparer.OrdinalIgnoreCase);
			string commandText = "SELECT DISTINCT ENDPOINTGROUPID, ENDPOINTGROUPNAME FROM ENDPOINTGROUPS";
			DataTable _obj = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0] as DataTable;
			foreach (DataRow dr in _obj.Rows)
			{
				_hashTableEPG.Add(dr[0], dr[1].ToString());
			}
		}

		private void Get_EndPoints()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			_hashTableEPS = new Hashtable(StringComparer.OrdinalIgnoreCase);
			string commandText = "SELECT DISTINCT ENDPOINTID, ENDPOINTNAME from ENDPOINTS";
			DataTable _obj = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0] as DataTable;
			foreach (DataRow dr in _obj.Rows)
			{
				_hashTableEPS.Add(dr[0], dr[1].ToString());
			}
		}

		private void Get_Race()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			_hashTableRace = new Hashtable(StringComparer.OrdinalIgnoreCase);
			string commandText = "select DISTINCT RACEID, RACENAME from RACES";
			DataTable _obj = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0] as DataTable;
			foreach (DataRow dr in _obj.Rows)
			{
				_hashTableRace.Add(dr[0], dr[1].ToString());
			}
		}

		private void Get_Genders()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			_hashTableGender = new Hashtable(StringComparer.OrdinalIgnoreCase);
			string commandText = "select DISTINCT GENDERID, GENDERNAME from GENDERS";
			DataTable _obj = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0] as DataTable;
			foreach (DataRow dr in _obj.Rows)
			{
				_hashTableGender.Add(dr[0], dr[1].ToString());
			}
		}

		private void Get_Ethnicity()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			_hashTableEthnicity = new Hashtable(StringComparer.OrdinalIgnoreCase);
			string commandText = "select DISTINCT ETHNICITYID, ETHNICITYNAME from ETHNICITY";
			DataTable _obj = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0] as DataTable;
			foreach (DataRow dr in _obj.Rows)
			{
				_hashTableEthnicity.Add(dr[0], dr[1].ToString());
			}
		}

		private void Get_AgeRange()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			_hashTableAgeRange = new Hashtable(StringComparer.OrdinalIgnoreCase);
			string commandText = "select DISTINCT AGERANGEID, AGERANGENAME from AGERANGES";
			DataTable _obj = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0] as DataTable;
			foreach (DataRow dr in _obj.Rows)
			{
				_hashTableAgeRange.Add(dr[0], dr[1].ToString());
			}
		}

		/// <summary>
		/// Handles the Load event of the ValidateDatabaseImport control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void ValidateDatabaseImport_Load(object sender, EventArgs e)
		{
			txtReportOutput.Text += string.Format("Date:\t{0}\r\n", DateTime.Today.ToShortDateString());
			FileInfo fInfo = new FileInfo(_file);
			txtReportOutput.Text += string.Format("File Name:\t{0}\r\n\r\n\r\n", fInfo.Name);

			this.Refresh();
			Application.DoEvents();
			//txtReportOutput.Text += "Error/Warnings\t Row\t Column Name \t Error/Warning Message\r\n";
		}

		/// <summary>
		/// Handles the Shown event of the ValidateDatabaseImport control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void ValidateDatabaseImport_Shown(object sender, EventArgs e)
		{
			bool bPassed = true;
			string tip = "Validating, this could take several minutes.";
			WaitShow(tip);
			lblPassedFailed.BackColor = Color.Orange;
			lblPassedFailed.Text = "Validating";
			Application.DoEvents();
			if (bPassed)
			{
				bPassed = VerifyColumnNames();
				txtReportOutput.Refresh();
			}
			if (bPassed)
			{
				bPassed = VerifyTableHasData();
				txtReportOutput.Refresh();
			}
			if (bPassed)
			{
				errors = 0;
				warnings = 0;
				bPassed = VerifyTableDataTypes();//errors
				if (_tbl.Columns.Contains("Endpoint Group")) //BenMAP 215: Added so that validation without endpoint data (model AQ) doesn't fail
					VerifyEndpointData();
				txtReportOutput.Refresh();
			}
			txtReportOutput.Text += "\r\n\r\n\r\nSummary\r\n";
			if (errors > 0)
			{
				txtReportOutput.Text += string.Format("-----\r\nValidation failed with {0} error(s).\r\n", errors);
			}
			else
			{
				txtReportOutput.Text += string.Format("-----\r\nValidation passed with {0} warning(s).\r\n", warnings);
			}
			txtReportOutput.Text += string.Format("{0} errors\r\n{1} warnings\r\n", errors, warnings);
			txtReportOutput.Refresh();
			SaveValidateResults();

			// Allow load if there aren't errors
			if (errors > 0) btnLoad.Enabled = false;
			else btnLoad.Enabled = true;

			if (!bPassed)
			{
				lblPassedFailed.BackColor = Color.Red;
				lblPassedFailed.Text = string.Format("Validation failed with {0} errors.", errors);
			}
			else
			{
				lblPassedFailed.BackColor = Color.Green;
				lblPassedFailed.Text = string.Format("Validation passed with {0} warnings.", warnings);

			}
			WaitClose();
		}
		/// <summary>
		/// Saves the validate results.
		/// </summary>
		private void SaveValidateResults()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files\ValidationResults";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			//file is formated as <dataset name>_year_month_day_hour_min.rtf
			//Monitor_2014_3_24_10_18.rtf
			string FileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}.rtf",
													  _datasetname, DateTime.Now.Year,
													  DateTime.Now.Month, DateTime.Now.Day,
													  DateTime.Now.Hour, DateTime.Now.Minute);
			txtReportOutput.Text += string.Format("Saved to: {0}", path + string.Format(@"\{0}", FileName));
			File.WriteAllText(path + string.Format(@"\{0}", FileName), txtReportOutput.Text);
		}

		/// <summary>
		/// Varifies the column names.
		/// </summary>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
		private bool VerifyColumnNames()
		{
			bool bPassed = true;
			txtReportOutput.Text += "Verifying Column Names\r\n\r\n";
			txtReportOutput.Text += "Error/Warnings\tRow\tColumn Name\tError/Warning Message\r\n";
			for (int i = 0; i < _colNames.Count; i++)
			{
				if (!_hashTableDef.ContainsValue(_colNames[i].ToString()))
				{
					txtReportOutput.Text += string.Format("Error\t\t{0}\t is not a valid column name for dataset {1}\r\n", _colNames[i].ToString(), _datasetname);
					errors++;
					bPassed = false;
				}
			}
			string sKey = string.Empty;
			foreach (DictionaryEntry dEntry in _hashTableDef)
			{
				sKey = dEntry.Key.ToString();
				if (sKey.Contains("##COLUMNNAME"))
				{
					if (!_colNames.Contains(dEntry.Value.ToString()))
					{
						// We added Geographic Area to HIF definitions in 1.4 and removed Study Location Type.  We want this to be an optional column. We'll process it if we have it and ignore it if we don't
						if (_datasetname == "Healthfunctions" && (dEntry.Value.ToString() == "Geographic Area" || dEntry.Value.ToString() == "Geographic Area Feature" || dEntry.Value.ToString() == "Study Location Type"))
						{
							// allow it to be missing
								} else
						{
							txtReportOutput.Text += string.Format("Error\t\t{0}\t column is missing for dataset {1}\r\n", dEntry.Value.ToString(), _datasetname);
							errors++;
							bPassed = false;
						}


					}
				}

			}

			if (!bPassed)
				txtReportOutput.Text += "\r\nValidation of columns failed.\r\nPlease check the column header and file.  The columns could be incorrect or the incorrect file was selected.\r\n";
			else
				txtReportOutput.Text += "\r\nValidation of columns passed.\r\n";

			return bPassed;
		}

		/// <summary>
		/// Varifies the table has data.
		/// </summary>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
		private bool VerifyTableHasData()
		{
			bool bPassed = true;
			if (_tbl.Rows.Count < 1)
			{
				txtReportOutput.Text += string.Format("Error\t\t\t\t\t No rows found in file for dataset {1}\r\n", "", _datasetname);
				errors++;
				bPassed = false;
			}

			return bPassed;
		}

		/// <summary>
		/// Varifies the table data types.
		/// </summary>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
		private bool VerifyTableDataTypes()
		{
			string errMsg = string.Empty;
			bool bPassed = true;
			bool required = true;
			int numChecked = 0;
			string dataType = string.Empty;
			string dataVal = string.Empty;
			string checkType = string.Empty;
			lblProgress.Visible = true;
			pbarValidation.Visible = true;

			pbarValidation.Step = 1;
			pbarValidation.Minimum = 0;
			pbarValidation.Maximum = _tbl.Rows.Count;
			pbarValidation.Value = pbarValidation.Minimum;
			txtReportOutput.Text += "\r\n\r\nVerifying data types.\r\n\r\n";
			txtReportOutput.Text += "Error/Warnings\t Row\t Column Name \t Error/Warning Message\r\n";
			Action work = delegate
			{
				foreach (DataRow dr in _tbl.Rows)
				{
					foreach (DataColumn dc in dr.Table.Columns)
					{
						dataType = _hashTableDef[dc.ColumnName + "##DATATYPE"].ToString();
						checkType = _hashTableDef[dc.ColumnName + "##CHECKTYPE"].ToString();//Get check type - error, warning, or none (empty string or null)
							required = Convert.ToBoolean(Convert.ToInt32(_hashTableDef[dc.ColumnName + "##REQUIRED"].ToString()));
						dataVal = dr[dc.ColumnName].ToString();
						errMsg = string.Empty;//resetting to be on the safe side
							try
						{
							if (!VerifyDataRowValues(dataType, dc.ColumnName, dataVal, dr, out errMsg))
							{
								if (checkType.ToLower() == "error")//if check type is "Error" and Verify Data Row Values fail - it's an error.
									{
									txtReportOutput.Text += string.Format("Error\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, dc.ColumnName, errMsg);
									errors++;
								}
								else if (checkType.ToLower() == "warning" && !required)//if a check type is a warning and it is not a required field it is a warning.
									{
									txtReportOutput.Text += string.Format("Warning\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, dc.ColumnName, errMsg);
									warnings++;
								}
								else if (checkType.ToLower() == "warning" && required)//if a check type is a warning and it is a required field it is a error.
									{
										// Shows warning if value is outside of range but shows error if it's an invalid type
										if (errMsg.Contains("within"))
									{
										txtReportOutput.Text += string.Format("Warning\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, dc.ColumnName, errMsg);
										warnings++;
									}
									else
									{
										txtReportOutput.Text += string.Format("Error\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, dc.ColumnName, errMsg);
										errors++;
									}
								}
								else if (checkType == string.Empty && required)//if check type is an empty string and it is a required field it is a error.
									{
									txtReportOutput.Text += string.Format("Error\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, dc.ColumnName, errMsg);
									errors++;
								}
								else if (checkType == string.Empty && !required)//if check type is an empty string and it is not a required field it is a warning.
									{
									if (errMsg.Contains("not a valid"))
									{
										txtReportOutput.Text += string.Format("Error\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, dc.ColumnName, errMsg);
										errors++;
									}
									else
									{
										txtReportOutput.Text += string.Format("Warning\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, dc.ColumnName, errMsg);
										warnings++;
									}
								}
								txtReportOutput.Refresh();
								numChecked++;
								bPassed = false;
								if (errors == 50)
								{
									return;
								}
							}
							else
							{
								numChecked++;
							}
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.Message);
						}
					}

					if (numChecked % 5 == 0)
					{
						pbarValidation.PerformStep();
						lblProgress.Text = Convert.ToString((int)((double)pbarValidation.Value / pbarValidation.Maximum * 100)) + "%";
						lblProgress.Refresh();
					}
				}
			};
			work();
			if (!bPassed)
				txtReportOutput.Text += "Validating Datatable values failed.";

			pbarValidation.Visible = false;
			lblProgress.Text = "";
			lblProgress.Visible = false;

			return bPassed;
		}

		private void VerifyEndpointData()	//Added for BenMAP-215 (Valuation Fn Validation)--but will generate warnings for other imports (HIF, etc.)
		{
			txtReportOutput.Text += "\r\n\r\nVerifying Endpoint Data.\r\n\r\n";
			txtReportOutput.Text += "Error/Warnings\t Row\t Column Name \t Error/Warning Message\r\n";
			List<string> lstEndpointGroups = _hashTableEPG.Values.OfType<string>().ToList();
			List<string> lstEndpoint =  _hashTableEPS.Values.OfType<string>().ToList();

			foreach (DataRow dr in _tbl.Rows)
			{
				if (lstEndpointGroups.FindIndex(x => x.Equals(dr["Endpoint Group"].ToString(), StringComparison.OrdinalIgnoreCase)) == -1)
				{
					txtReportOutput.Text += string.Format("Warning\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, "Endpoint Group", "Value currently not stored in the database. Import will create a new entry.");
					warnings++;
				}

				if (lstEndpoint.FindIndex(x => x.Equals(dr["Endpoint"].ToString(), StringComparison.OrdinalIgnoreCase)) == -1)
				{
					txtReportOutput.Text += string.Format("Warning\t {0}\t {1} \t {2}\r\n", _tbl.Rows.IndexOf(dr) + 1, "Endpoint", "Value currently not stored in the database. Import will create a new entry.");
					warnings++;
				}
			}
		}

		private bool VerifyStartAge(int startAge, int endAge, out string errMsg)
		{
			bool bPassed = true;
			errMsg = string.Empty;
			if (startAge < 0)
			{
				errMsg = "Age can not be a negitive value";
				return false;
			}
			if (startAge > endAge && endAge > 0)
			{
				return false;
			}
			return bPassed;
		}
		private bool VerifyEndAge(int startAge, int endAge, out string errMsg)
		{
			bool bPassed = true;
			errMsg = string.Empty;
			if (endAge < 0)
			{
				errMsg = "Age can not be a negitive value";
				return false;
			}
			if (endAge < startAge && endAge > 0)
			{
				return false;
			}
			return bPassed;
		}

		/// <summary>
		/// Verifies the data row values.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="valToVerify">The value to verify.</param>
		/// <param name="errMsg">The error MSG.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
		private bool VerifyDataRowValues(string dataType, string columnName, string valToVerify, DataRow dr, out string errMsg)
		{

			string min = _hashTableDef[columnName + "##LOWERLIMIT"].ToString();// Get_Min(columnName, dataType);
			string max = _hashTableDef[columnName + "##UPPERLIMIT"].ToString();//Get_Max(columnName, dataType);
			bool required = Convert.ToBoolean(Convert.ToInt32(_hashTableDef[columnName + "##REQUIRED"].ToString()));//Get required value (true (1) or false (0))
			string checkType = _hashTableDef[columnName + "##CHECKTYPE"].ToString();//Get check type - error, warning, or none (empty string or null)
																											// removed $ and %, as these are used in the valuation functions (and several others, as well)
			Regex regx = new Regex(@"^[^~!@#`^]+");
			double tempVal;
			int outVal = -1;
			decimal outVal2 = 0;
			bool bPassed = true;

			errMsg = string.Empty;

			try
			{
				//is the field required and the value to varify is null or empty, it should fail. go no further
				if (required && string.IsNullOrEmpty(valToVerify))
				{
					errMsg = string.Format("Missing values.  {0} is a required field.", columnName);
					return false;
				}

				switch (dataType)
				{
					case "string":
						//getting value to varify. (some strings are doubles such as the monitors name.
						if (double.TryParse(valToVerify, out tempVal))
						{
							valToVerify = tempVal.ToString();
						}
						//checking it's min
						if (!string.IsNullOrEmpty(min) && bPassed)
						{
							if (valToVerify.Length < Convert.ToInt32(min))
							{
								errMsg = "Value does not meet the minimum length of " + min;            //BenMAP 441/442/444--Provide user with a more helpful message for min/max value error
								bPassed = false;
							}
						}
						//checking it's max
						if (!string.IsNullOrEmpty(max) && bPassed)
						{
							if (valToVerify.Length > Convert.ToInt32(max))
							{
								errMsg = "Value exceeds the maximum length of " + max;
								bPassed = false;
							}
						}
						//checking for invalid characters
						if (!String.IsNullOrEmpty(valToVerify))
						{
							if (!regx.IsMatch(valToVerify) && bPassed)
							{
								errMsg = "Value has invalid characters.";
								bPassed = false;
							}
						}
						break;
					case "integer":

						if (decimal.TryParse(valToVerify, out outVal2)) //use decimal so that values like 2.0 is also acceptable.
						{
							if (outVal2 % 1 == 0)
							{
								outVal = Convert.ToInt32(outVal2);

								if (!string.IsNullOrEmpty(min) && bPassed)
								{
									if (outVal < Convert.ToInt32(min))
									{
										errMsg = string.Format("Value is not within min({0}) range.", min);
										bPassed = false;
									}
								}
								if (!string.IsNullOrEmpty(max) && bPassed)
								{
									if (outVal > Convert.ToInt32(max))
									{
										errMsg = string.Format("Value is not within max({0}) range.", max);
										bPassed = false;
									}
								}
							}
							else
							{
								errMsg = string.Format("Value '{0}' is not a valid integer.", valToVerify);
								bPassed = false;
							}
						}
						else
						{
							errMsg = string.Format("Value '{0}' is not a valid integer.", valToVerify);
							bPassed = false;
						}
						break;
					case "float":
						float outValf = -1;
						//do a try parse
						//do a min and max check
						//if there is a Max there must be a min.
						if (float.TryParse(valToVerify, out outValf))
						{
							if (!string.IsNullOrEmpty(min) && !string.IsNullOrEmpty(max) && bPassed)
							{
								if (outValf < Convert.ToInt32(min) || outValf > Convert.ToInt32(max))
								{
									errMsg = string.Format("Value is not within min({0}) or max({1}) range.", min, max);
									bPassed = false;
								}
							}
						}
						else
						{
							// If it doesn't pass TryParse and is not empty then it isn't a valid float
							// Empty cases cause earlier 
							if (!string.IsNullOrEmpty(valToVerify))
							{
								errMsg = string.Format("Value '{0}' is not a valid float.", valToVerify);
								bPassed = false;
							}
						}
						break;
					case "double":

						break;
					case "blob":
						break;
					default:
						break;
				}
				string errorMsg = string.Empty;
				if (!verifyAgainstTable(columnName, valToVerify, dr, out errorMsg))
				{
					if (errorMsg != string.Empty)
					{
						errMsg += " " + errorMsg;
					}
					bPassed = false;
				}
			}
			catch (Exception ex)
			{
				bPassed = false;
				MessageBox.Show(ex.Message);
			}
			return bPassed;
		}

		private bool verifyAgainstTable(string columnName, string valToVerify, DataRow dr, out string errMsg)
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string cmdText = string.Empty;
			bool bPassed = true;
			object rtv = null;
			string strRtv = string.Empty;
			errMsg = string.Empty;
			try
			{
				switch (columnName)
				{
					case "EndpointGroup":
					case "Endpoint Group":
						//cmdText = string.Format("SELECT DISTINCT ENDPOINTGROUPID FROM ENDPOINTGROUPS WHERE ENDPOINTGROUPNAME = '{0}'", valToVerify);
						//rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText);
						//if (rtv == null) //did not find anyting from the query
						/* -- commented out next block to remove value checking 
						if(!_hashTableEPG.ContainsValue(valToVerify))
						{
							 errMsg = string.Format("Invalid Endpoint Group Name.  ({0})", valToVerify);
						}
						 */
						break;

					case "Endpoint":
						//cmdText = string.Format("SELECT DISTINCT ENDPOINTID FROM ENDPOINTS WHERE ENDPOINTNAME = '{0}'", valToVerify);
						//rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText);
						//if (rtv == null)
						/* -- commented out next block to remove value checking 
						if(!_hashTableEPS.ContainsValue(valToVerify))
						{
								  errMsg = string.Format("Invalid Endpoint Name given.  ({0})", valToVerify);
						}
						 */
						break;
					case "Study Year":
					case "Year":
						if (!Regex.IsMatch(valToVerify, "^(19|20)[0-9][0-9]"))
						{
							errMsg = string.Format("Invalid year entry. ({0})", valToVerify);
						}
						break;
					case "Race":
						//cmdText = string.Format("SELECT DISTINCT RACEID FROM RACES WHERE RACENAME = '{0}'", valToVerify);
						//rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText);
						//if(rtv == null)
						/* -- commented out next block to remove value checking 
						if(!_hashTableRace.ContainsValue(valToVerify))
						{
							 errMsg = string.Format("'{0}' is not a valid race.", valToVerify);
						}
						 */
						break;
					case "Gender":
						//cmdText = string.Format("SELECT DISTINCT GENDERID FROM GENDERS WHERE GENDERNAME = '{0}'", valToVerify);
						//rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText);
						//if (rtv == null)
						/* -- commented out next block to remove value checking 
						if(!_hashTableGender.ContainsValue(valToVerify))
						{
							 errMsg = string.Format("'{0}' is not valid gender.", valToVerify);
						}
						*/
						break;
					case "Ethnicity":
						//cmdText = string.Format("SELECT DISTINCT ETHNICITYID FROM ETHNICITY WHERE ETHNICITYNAME = '{0}'", valToVerify);
						//rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText);
						//if (rtv == null)
						/* -- commented out next block to remove value checking 
						if(!_hashTableEthnicity.ContainsValue(valToVerify))
						{
							 errMsg = string.Format("'{0}' is not valid ethnicity.", valToVerify);
						}
						*/
						break;
					case "Start Age":
					case "End Age":
						//this is for validating start Age and End Age
						string erMsg = string.Empty;
						if (columnName.Equals("Start Age"))
						{
							//get the end age and do a quick compair.
							if (!VerifyStartAge(Convert.ToInt32(valToVerify), Convert.ToInt32(dr["End Age"].ToString()), out erMsg))
							{
								if (string.IsNullOrEmpty(erMsg))//passed checking for negitives 
								{
									errMsg = string.Format("Start Age ({0}) should not be less than End Age.", valToVerify);
								}
							}
						}
						if (columnName.Equals("End Age"))
						{
							//get the start age and do a quick compair.
							if (!VerifyEndAge(Convert.ToInt32(dr["Start Age"].ToString()), Convert.ToInt32(valToVerify), out erMsg))
							{
								if (string.IsNullOrEmpty(erMsg))//passed checking for negitives 
								{
									errMsg = string.Format("End Age ({0}) should not be greater than Start Age.", valToVerify);
								}
							}
						}
						if (!string.IsNullOrEmpty(erMsg))
						{
							errMsg += " " + erMsg;
						}
						break;
					case "Latitude":
						float lat = 0;
						if (!string.IsNullOrEmpty(valToVerify))
						{
							if (float.TryParse(valToVerify, out lat))
							{
								if (lat < -90 || lat > 90)
								{
									errMsg = "Latitude must be between -90 and 90 degrees inclusive";
								}
							}
							else
							{
								errMsg = string.Format("Latitude '{0}' is an invalid value", valToVerify);
							}
						}

						break;
					case "Longitude":
						float longitude = 0;
						if (!string.IsNullOrEmpty(valToVerify))
						{
							if (float.TryParse(valToVerify, out longitude))
							{
								if (longitude < -180 || longitude > 180)
								{
									errMsg = "Longitude must be between -180 and 180 degrees inclusive";
								}
							}
							else
							{
								errMsg = string.Format("Longitude '{0}' is an invalid value", valToVerify);
							}
						}
						break;
					case "AgeRange":
						//cmdText = string.Format("SELECT DISTINCT AGERANGEID FROM AGERANGES WHERE AGERANGENAME  = '{0}'", valToVerify);
						//rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText);
						//if (rtv == null)
						if (!_hashTableAgeRange.ContainsValue(valToVerify))
						{
							errMsg = string.Format("Invalid Age Range Name given.  ({0})", valToVerify);
						}
						break;
					case "Column":

						break;
					case "Row":

						break;
					case "Type":

						break;
					case "Value": // note that Values (plural) is used for monitor data and Value (singular) is used for incidence data

						break;
					case "Values":  // note that Values (plural) is used for monitor data and Value (singular) is used for incidence data
						if (!string.IsNullOrEmpty(valToVerify))
						{
							// monitor values must have correct number of records
							// this is checked by testing that commas are 1 less than the number of records for the period 
							// (note that years and hours are in twice; once for leap and once for non-leap years)
							int iDayCount;
							iDayCount = Regex.Matches(valToVerify, ",").Count;
							// 2014 11 13 - added check for monthly data (11 commas) and quarterly data (3 commas) 
							if (!((iDayCount == 364) || (iDayCount == 365) || (iDayCount == 8759) || (iDayCount == 8783)
									  || (iDayCount == 0) || (iDayCount == 11) || (iDayCount == 3)))
							{
								errMsg = string.Format("Wrong number of days for monitor.  ({0})", valToVerify);
							}

						}

						break;
					case "Variable":

						break;
					default:
						break;
				}
				if (errMsg != string.Empty)
				{
					bPassed = false;
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			return bPassed;
		}

		/// <summary>
		/// Get_s the minimum.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="dataType">Type of the data.</param>
		/// <returns>System.String.</returns>
		private string Get_Min(string columnName, string dataType)
		{
			string cmdText = string.Format("SELECT LOWERLIMIT FROM DATASETDEFINITION where DATASETTYPENAME='{0}' " +
													  "and COLUMNNAME='{1}' and DATATYPE='{2}'", _datasetname, columnName, dataType);
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText).ToString();
			if (string.IsNullOrEmpty(obj))
				obj = string.Empty;
			return obj;
		}

		/// <summary>
		/// Get_s the maximum.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="dataType">Type of the data.</param>
		/// <returns>System.String.</returns>
		private string Get_Max(string columnName, string dataType)
		{
			string cmdText = string.Format("SELECT UPPERLIMIT FROM DATASETDEFINITION where DATASETTYPENAME='{0}' " +
									  "and COLUMNNAME='{1}' and DATATYPE='{2}'", _datasetname, columnName, dataType);
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, cmdText).ToString();
			if (string.IsNullOrEmpty(obj))
				obj = string.Empty;
			return obj;
		}

		/// <summary>
		/// Handles the Click event of the btnLoad control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void btnLoad_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		/// <summary>
		/// Handles the Click event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		public void WaitShow(string msg)
		{
			try
			{
				if (sFlog == true)
				{
					sFlog = false;
					waitMess.Msg = msg;
					System.Threading.Thread upgradeThread = null;
					upgradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWaitMess));
					upgradeThread.Start();
					upgradeThread.IsBackground = true;
				}
			}
			catch (System.Threading.ThreadAbortException Err)
			{
				MessageBox.Show(Err.Message);
			}
		}
		private delegate void CloseFormDelegate();

		public void WaitClose()
		{
			if (waitMess.InvokeRequired)
				waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
			else
				DoCloseJob();
		}
		private void DoCloseJob()
		{
			try
			{
				if (!waitMess.IsDisposed)
				{
					if (waitMess.Created)
					{
						sFlog = true;
						waitMess.Close();
					}
				}
			}
			catch (System.Threading.ThreadAbortException Err)
			{
				MessageBox.Show(Err.Message);
			}
		}
		private void ShowWaitMess()
		{
			try
			{
				if (!waitMess.IsDisposed)
				{
					waitMess.ShowDialog();
				}
			}
			catch (System.Threading.ThreadAbortException Err)
			{
				MessageBox.Show(Err.Message);
			}
		}
	}
}
