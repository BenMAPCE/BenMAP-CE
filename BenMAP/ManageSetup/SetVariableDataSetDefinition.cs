using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
	public partial class VariableDataSetDefinition : FormBase
	{
		private string _datasetName = string.Empty;

		int variabledatasetID = -1;
		private MetadataClassObj _metadataObj = null;
		private bool _bEdit = false;
		private bool _isLocked = false;
		// private bool _CopyingDataset = false;
		private object _newDataSetID = null;//used for copying an existing dataset that is locked. (the new datasetid)
		private object _oldDataSetID = null;//used for copying an existing dataset that is locked. (the locked datasetid)

		private List<MetadataClassObj> _lstMetadata;

		//public List<MetadataClassObj> LstMetadata
		//{
		//    get { return _lstMetadata; }
		//}



		public VariableDataSetDefinition()
		{
			InitializeComponent();
			_datasetName = string.Empty;
			_lstMetadata = new List<MetadataClassObj>();
		}

		public VariableDataSetDefinition(string datasetName)
		{
			InitializeComponent();
			_datasetName = datasetName;
			_lstMetadata = new List<MetadataClassObj>();
		}

		public VariableDataSetDefinition(string datasetName, bool isLocked) : this(datasetName)
		{

			_bEdit = !isLocked;
			_isLocked = isLocked;

			FireBirdHelperBase fb = new ESILFireBirdHelper();

			if (_isLocked)
			{
				txtDataSetName.Enabled = true;
				_datasetName = datasetName + "_Copy";
				// get old data set ID for use in copy

				txtDataSetName.Text = _datasetName;
				string commandText = string.Format("Select SetupVariableDataSetID from SetupVariableDataSets where SetupVariableDataSetName='{0}'", datasetName);
				_oldDataSetID = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);

				//_CopyingDataset = true;
			}
			else
			{
				txtDataSetName.Enabled = false;
			}
		}

		private void VariableDataSetDefinition_Load(object sender, EventArgs e)
		{
			string commandText = string.Empty;
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			CommonClass.SetupOLVEmptyListOverlay(this.olvData.EmptyListMsgOverlay as BrightIdeasSoftware.TextOverlay);
			DataSet ds;
			DataSet dsOrigin;
			try
			{
				if (_datasetName != string.Empty)
				{
					txtDataSetName.Text = _datasetName;
					commandText = string.Format("select SetUpVariableDataSetID from SetUpVariableDataSets where SetUpVariableDataSetName='{0}'", txtDataSetName.Text);
					object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					variabledatasetID = Convert.ToInt16(obj);
					commandText = string.Format("select SetUpVariableName,SetUpVariableID,GridDefinitionID from SetUpVariables where SetUpVariableDataSetID={0}", obj);
					dsOrigin = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
					
					//BENMAP-553 Grid Definition field should show nothing when the window is just loaded. 
					//int GridDefinitionID = 0;					
					//foreach (DataRow dr in dsOrigin.Tables[0].Rows)
					//{
					//	GridDefinitionID = int.Parse(dr[2].ToString());
					//}
					//commandText = string.Format("select GridDefinitionName from GridDefinitions where GridDefinitionID={0}", GridDefinitionID);
					//ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
					//foreach (DataRow dr in ds.Tables[0].Rows)
					//{
					//	string GridDefinitionName = dr[0].ToString();
					//	txtGridDefinition.Text = GridDefinitionName;
					//	txtGridDefinition.ReadOnly = true;
					//}
					txtGridDefinition.ReadOnly = true;
					int itemCount = dsOrigin.Tables[0].Rows.Count;
					string str = string.Empty;
					_dsSelectedData = new DataSet();
					string variableName = string.Empty;
					string variableID = string.Empty;
					for (int i = 0; i < itemCount; i++)
					{
						variableName = dsOrigin.Tables[0].Rows[i][0].ToString();
						lstDataSetVariable.Items.Add(variableName);
						variableID = dsOrigin.Tables[0].Rows[i][1].ToString();

						commandText = string.Format("select ccolumn ,row ,vvalue from SetUpGeographicVariables where SetUpVariableID={0}", variableID);
						ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
						dt = ds.Tables[0].Clone();
						dt = ds.Tables[0].Copy();
						dt.TableName = variableName;
						_dsSelectedData.Tables.Add(dt);
					}
					lstDataSetVariable.SelectedIndex = 0;
				}
				else
				{
					commandText = string.Format("select ccolumn ,row ,vvalue from SetUpGeographicVariables where SetUpVariableID=null");
					ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
					olvData.DataSource = ds.Tables[0];
					_dt = ds.Tables[0];

					int number = 0;
					int VariableDatasetID = 0;
					do
					{
						string comText = "select setupVariableDatasetID from setupVariableDatasets where setupVariableDatasetName=" + "'VariableDataSet" + Convert.ToString(number) + "'";
						VariableDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
						number++;
					} while (VariableDatasetID > 0);
					txtDataSetName.Text = "VariableDataSet" + Convert.ToString(number - 1);
					txtGridDefinition.Text = string.Empty;

				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{


			Object obj = null;


			string commandText = string.Empty;

			if (txtDataSetName.Text == string.Empty)
			{
				MessageBox.Show("Please input a valid dataset name.");
				return;
			}

			if (_isLocked)//doing a copy
			{

				CopyDatabase();

				this.DialogResult = DialogResult.OK;
				this.Close();
				return;
			}
			else if (lstDataSetVariable.Items.Count == 0)
			{
				MessageBox.Show("Please load a variable datafile.");
				return;
			}
			//ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			if (CommonClass.Connection.State != ConnectionState.Open)
			{ CommonClass.Connection.Open(); }

			// STOPPED HERE
			// attempt to use common connection without transaction (commented out transactions)
			//FirebirdSql.Data.FirebirdClient.FbConnection fbconnection = CommonClass.getNewConnection();
			//FirebirdSql.Data.FirebirdClient.FbConnection fbconnection = CommonClass.Connection;
			//fbconnection.Open();
			//FirebirdSql.Data.FirebirdClient.FbTransaction fbtra = fbconnection.BeginTransaction(); 
			//FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
			//FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = CommonClass.Connection.CreateCommand();
			//fbCommand.Connection = fbconnection;
			//fbCommand.CommandType = CommandType.Text;
			// fbCommand.Transaction = fbtra;


			DataSet ds = new DataSet();
			DataTable dt;
			int rowCount;
			int datasetId = 0;
			string variableDatasetID = string.Empty;
			int variableID = 0;
			try
			{
				lblProgressBar.Visible = true;
				progBarVariable.Visible = true;
				progBarVariable.Value = 0;
				progBarVariable.Minimum = 0;
				progBarVariable.Maximum = 0;
				progBarVariable.Refresh();
				commandText = string.Format("select SETUPVARIABLEDATASETID from  SetUpVariableDataSets where SETUPVARIABLEDATASETNAME='{0}' and SetupID={1}", txtDataSetName.Text, CommonClass.ManageSetup.SetupID);
				obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
				if (_datasetName == string.Empty)
				{
					if (obj != null) { MessageBox.Show("The dataset name has already been defined. Please enter a different name."); return; }
					commandText = "select max(SETUPVARIABLEDATASETID) from SETUPVARIABLEDATASETS";
					obj = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
					datasetId = Convert.ToInt32(obj);
					variableDatasetID = obj.ToString();
					//The 'F' is for the Locked column in SetUpVariableDataSets - this is improted and not predefined
					commandText = string.Format("insert into SetUpVariableDataSets values({0},{1},'{2}', 'F')", variableDatasetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text);
					// fbCommand.CommandText = commandText;
					// fbCommand.ExecuteNonQuery();
					// STOPPED HERE
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = string.Format("select GridDefinitionID from GridDefinitions where GridDefinitionName='{0}' and SetupID={1}", txtGridDefinition.Text, CommonClass.ManageSetup.SetupID);
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					string gridDefinationID = obj.ToString();
					int count = _dsSelectedData.Tables.Count;
					foreach (DataTable dtcount in _dsSelectedData.Tables)
					{
						progBarVariable.Maximum += dtcount.Rows.Count;
					}
					commandText = "select max(SETUPVARIABLEID) from SetUpVariables";
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					if (obj != null)
						variableID = Convert.ToInt32(obj);
					for (int i = 0; i < count; i++)
					{
						dt = _dsSelectedData.Tables[i].Clone();
						dt = _dsSelectedData.Tables[i].Copy();

						string variableName = dt.TableName;
						commandText = string.Format("select SETUPVARIABLEID from SETUPVARIABLES where SETUPVARIABLENAME='{0}' and SETUPVARIABLEDATASETID='{1}'", variableName, variableDatasetID);
						obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);

						if (obj == null)
						{
							variableID++;
							// 2015 09 23 - BENMAP-347 - replaced metada data on insert statement
							// removed metaadata from insert statement
							//commandText = string.Format("insert into SetUpVariables values({0},{1},'{2}','{3}', {4})", variableID, variableDatasetID, variableName, gridDefinationID, _lstMetadata[i].MetadataEntryId);
							commandText = string.Format("insert into SetUpVariables(SETUPVARIABLEID, SETUPVARIABLEDATASETID, SETUPVARIABLENAME, GRIDDEFINITIONID, METADATAID ) "
									+ "values({0},{1},'{2}',{3}, {4})", variableID, variableDatasetID, variableName, gridDefinationID, _lstMetadata[i].MetadataEntryId);

							//fbCommand.CommandText = commandText;
							//fbCommand.ExecuteNonQuery();
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

							rowCount = dt.Rows.Count;
							for (int j = 0; j < (rowCount / 125) + 1; j++)
							{
								commandText = "execute block as declare SetupVariableID int;" + " BEGIN ";
								for (int k = 0; k < 125; k++)
								{
									if (j * 125 + k < dt.Rows.Count)
									{
										commandText = commandText + string.Format(" select SetupVariableID from SetupVariables  where SetupVariableDataSetID={0} and SetupVariableName='{1}' and GridDefinitionID={2} into :SetupVariableID;", variableDatasetID, variableName, gridDefinationID);
										progBarVariable.Value++;
										progBarVariable.Refresh();
										lblProgressBar.Text = Convert.ToString((int)((double)progBarVariable.Value * 100 / progBarVariable.Maximum)) + "%";
										lblProgressBar.Refresh();
									}
									else
									{
										continue;
									}
									float vvalue = Convert.ToSingle(dt.Rows[j * 125 + k][2]);
									string vvalueStr = vvalue.ToString(System.Globalization.CultureInfo.InvariantCulture);
									commandText = commandText + string.Format(" insert into SETUPGEOGRAPHICVARIABLES values (:SetupVariableID,{0},{1},{2});", dt.Rows[j * 125 + k][0], dt.Rows[j * 125 + k][1], vvalueStr);
								}
								commandText = commandText + "END";
								//fbCommand.CommandText = commandText;
								//fbCommand.ExecuteNonQuery();
								fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

							}
						}
					}
				}
				else
				{
					commandText = string.Format("select SETUPVARIABLEDATASETID from  SetUpVariableDataSets where SETUPVARIABLEDATASETNAME='{0}' and SetupID={1}", _datasetName, CommonClass.ManageSetup.SetupID);
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					variableDatasetID = obj.ToString();
					commandText = string.Format("update SETUPVARIABLEDATASETS set SETUPVARIABLEDATASETNAME='{0}' where setupid={1} and SETUPVARIABLEDATASETID={2}", txtDataSetName.Text, CommonClass.ManageSetup.SetupID, variableDatasetID);
					//fbCommand.CommandText = commandText;
					//fbCommand.ExecuteNonQuery();
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);


					commandText = string.Format("select GridDefinitionID from GridDefinitions where GridDefinitionName='{0}' and SetupID={1}", txtGridDefinition.Text, CommonClass.ManageSetup.SetupID);
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					string gridDefinationID = obj.ToString();
					int count = _dsSelectedDataTemp.Tables.Count;
					foreach (DataTable dtcount in _dsSelectedDataTemp.Tables)
					{
						progBarVariable.Maximum += dtcount.Rows.Count;
					}
					commandText = "select max(SETUPVARIABLEID) from SetUpVariables";
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					if (obj != null)
						variableID = Convert.ToInt32(obj);
					for (int i = 0; i < count; i++)
					{
						dt = _dsSelectedDataTemp.Tables[i].Clone();
						dt = _dsSelectedDataTemp.Tables[i].Copy();

						string variableName = dt.TableName;
						commandText = string.Format("select SETUPVARIABLEID from SETUPVARIABLES where SETUPVARIABLENAME='{0}' and SETUPVARIABLEDATASETID='{1}'", variableName, variableDatasetID);
						obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);

						variableID++;
						commandText = string.Format("insert into SetUpVariables values({0},{1},'{2}','{3}', {4})", variableID, variableDatasetID, variableName, gridDefinationID, _lstMetadata[i].MetadataEntryId);
						//fbCommand.CommandText = commandText;
						//fbCommand.ExecuteNonQuery();
						fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						rowCount = dt.Rows.Count;
						for (int j = 0; j < (rowCount / 125) + 1; j++)
						{
							commandText = "execute block as declare SetupVariableID int;" + " BEGIN ";
							for (int k = 0; k < 125; k++)
							{
								if (j * 125 + k < dt.Rows.Count)
								{
									commandText = commandText + string.Format(" select SetupVariableID from SetupVariables  where SetupVariableDataSetID={0} and SetupVariableName='{1}' and GridDefinitionID={2} into :SetupVariableID;", variableDatasetID, variableName, gridDefinationID);
									progBarVariable.Value++;
									progBarVariable.Refresh();
									lblProgressBar.Text = Convert.ToString((int)((double)progBarVariable.Value * 100 / progBarVariable.Maximum)) + "%";
									lblProgressBar.Refresh();
								}
								else
								{
									continue;
								}
								float vvalue = Convert.ToSingle(dt.Rows[j * 125 + k][2]);
								string vvalueStr = vvalue.ToString(System.Globalization.CultureInfo.InvariantCulture);
								commandText = commandText + string.Format(" insert into SETUPGEOGRAPHICVARIABLES values (:SetupVariableID,{0},{1},{2});", dt.Rows[j * 125 + k][0], dt.Rows[j * 125 + k][1], vvalueStr);
							}
							commandText = commandText + "END";
							//fbCommand.CommandText = commandText;
							//fbCommand.ExecuteNonQuery();
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

						}
					}
				}
				// STOPPED HERE
				//fbtra.Commit();
				// turned off connection close to allow exit routine to handle
				// fbCommand.Connection.Close();

				//if(!_bEdit)
				//{
				insertMetadata(datasetId);
				//}
				progBarVariable.Visible = false;
				lblProgressBar.Visible = false;

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to load variable dataset.  Please validate file for more detailed informaton.");
				// STOPPED HERE
				// fbtra.Rollback();
				progBarVariable.Value = 0;
				progBarVariable.Visible = false;
				lblProgressBar.Text = "";
				lblProgressBar.Visible = false;
				Logger.LogError(ex.Message);
			}
		}
		private void insertMetadata(int dataSetID)
		{
			//_metadataObj.DatasetId = dataSetID;
			try
			{
				foreach (MetadataClassObj mcobj in _lstMetadata)
				{
					mcobj.DatasetId = dataSetID;
					mcobj.SetupId = CommonClass.ManageSetup.SetupID;
					mcobj.DatasetTypeId = SQLStatementsCommonClass.getDatasetID("VariableDataset");
					SQLStatementsCommonClass.insertMetadata(mcobj);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to save Metadata.");
				Logger.LogError(ex);
			}

			//if (!SQLStatementsCommonClass.insertMetadata(_metadataObj))
			//{
			//    MessageBox.Show("Failed to save Metadata.");
			//}
		}

		DataSet _dsSelectedData = new DataSet();
		DataSet _dsSelectedDataTemp = new DataSet();
		DataTable _dtOrigin = new DataTable();
		DataTable dt;
		DataTable _dt;
		private void btnLoadData_Click(object sender, EventArgs e)
		{
			try
			{
				string tip = "Loading the datafile.";

				LoadVariableDatabase frm = new LoadVariableDatabase();
				frm.DefinitionID = txtGridDefinition.Text;
				DialogResult rtn = frm.ShowDialog();
				if (rtn != DialogResult.OK)
				{
					return;
				}
				_metadataObj = frm.MetadataObj;
				txtGridDefinition.Text = frm.DefinitionID;
				txtGridDefinition.ReadOnly = false;
				string strPath = frm.DataPath;
				WaitShow(tip);
				_dtOrigin = CommonClass.ExcelToDataTable(strPath);
				int colCount = _dtOrigin.Columns.Count;
				if (_dtOrigin == null || colCount <= 2)
				{
					WaitClose();
					MessageBox.Show("Failed to load variable dataset.  Please validate file for more detailed informaton.");
					return;
				}
				int icol = -1;
				int irow = -1;
				for (int i = 0; i < _dtOrigin.Columns.Count; i++)
				{
					if (_dtOrigin.Columns[i].ColumnName.ToLower().Replace(" ", "") == "row")
						irow = i;
					if (_dtOrigin.Columns[i].ColumnName.ToLower().Replace(" ", "") == "col" || _dtOrigin.Columns[i].ColumnName.ToLower().Replace(" ", "") == "column")
						icol = i;
				}

				string warningtip = "";
				if (irow < 0) warningtip = "'Row', ";
				if (icol < 0) warningtip += "'Column', ";
				if (warningtip != "")
				{
					warningtip = warningtip.Substring(0, warningtip.Length - 2);
					WaitClose();
					warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
					MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				string header = string.Empty;

				DataColumn[] cols = new DataColumn[colCount];
				_dtOrigin.Columns.CopyTo(cols, 0);
				for (int i = 0; i < colCount; i++)
				{
					if (i == icol || i == irow) continue;
					header = _dtOrigin.Columns[i].ColumnName;
					dt = new DataTable(header);
					dt = _dtOrigin.DefaultView.ToTable(false, _dtOrigin.Columns[icol].ColumnName, _dtOrigin.Columns[irow].ColumnName, header);
					dt.TableName = header;
					dt.Columns[0].ColumnName = "ccolumn";
					dt.Columns[1].ColumnName = "row";
					dt.Columns[2].ColumnName = "vvalue";

					if (_dsSelectedData.Tables.Contains(dt.TableName))
					{
						string commandText = string.Format("select SETUPVARIABLEID from SETUPVARIABLES where SETUPVARIABLENAME='{0}' and SETUPVARIABLEDATASETID='{1}'", dt.TableName, variabledatasetID);
						ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
						object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
						WaitClose();
						DialogResult dr = MessageBox.Show("The variable name '" + dt.TableName + "' is already in use. Please enter a different name. Click cancel to skip this variable.", "Error", MessageBoxButtons.OKCancel);
						if (dr == DialogResult.OK)
						{
							Renamefile frmRename = new Renamefile();
							frmRename.lblRename.Text = "Please rename the variable:";
							frmRename.newfileName = dt.TableName;
							frmRename.manage = "Variable";
							frmRename.datasetID = Convert.ToInt16(variabledatasetID);
							if (frmRename.ShowDialog() == DialogResult.OK)
							{
								string newvariableName = frmRename.newfileName;
								dt.TableName = newvariableName;
							}
							else
							{
								continue;
							}
						}
						else
						{
							continue;
						}
					}
					WaitShow(tip);
					lstDataSetVariable.Items.Add(dt.TableName);
					_dsSelectedData.Tables.Add(dt);
					DataTable _dt = dt.Copy();
					_dsSelectedDataTemp.Tables.Add(_dt);
				}

				lstDataSetVariable.SelectedIndex = 0;
				if (_lstMetadata.Count < 1)
				{
					_lstMetadata.Add(_metadataObj);
				}
				else
				{   //no not glamorous. Get Metadata gets the most current METADATAENTRYID from the METADATAINFORMATION table.  
						//If an isert is not done, it will always retrun the same number.  In this case I am loading the metadata 
						//objects and will insert them once the final ok is done.
						//This will ensure that each metadataId is uniquie.
					int temp = _lstMetadata[_lstMetadata.Count - 1].MetadataEntryId + 1;
					_metadataObj.MetadataEntryId = temp;
					_lstMetadata.Add(_metadataObj);
				}
				txtGridDefinition.Enabled = false;
				WaitClose();

			}
			catch (Exception ex)
			{
				WaitClose();
				MessageBox.Show("Failed to load variable dataset.  Please validate file for more detailed informaton.");
				Logger.LogError(ex);
			}
		}

		private void lstDataSetVariable_SelectedValueChanged(object sender, EventArgs e)
		{
			string selectedItem = string.Empty;
			string strSql = string.Empty;
			try
			{
				if (lstDataSetVariable.SelectedItem != null)
				{
					selectedItem = lstDataSetVariable.SelectedItem.ToString();
					//BENMAP-553 update grid definition name on screen.
					FireBirdHelperBase fb = new ESILFireBirdHelper();
					string commandText = string.Format(@"select s.GRIDDEFINITIONNAME 
from SetUpVariables r 
inner join GRIDDEFINITIONS s on r.GRIDDEFINITIONID=s.GRIDDEFINITIONID 
where r.SETUPVARIABLENAME = '{0}'", selectedItem);
					object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					txtGridDefinition.Text = obj.ToString();
					txtGridDefinition.ReadOnly = true;
				}
				if (_datasetName != string.Empty)
				{
					olvData.DataSource = _dsSelectedData.Tables[selectedItem];
				}
				else
				{
					_dt = _dsSelectedData.Tables[selectedItem];
					olvData.DataSource = _dt;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			int index = 0;
			string commandText = string.Empty;
			object obj;
			// 2015 09 02 - add conformation form - BENMAP 327 
			if (MessageBox.Show("Delete the selected variable?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return; // cancel action 
			}

			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			string setupVariableID = string.Empty;
			string variableDatasetID = string.Empty;
			try
			{
				WaitShow("Deleting data...");
				string selectedItem = lstDataSetVariable.SelectedItem.ToString();
				int itemCount = lstDataSetVariable.Items.Count;
				int selectedIndex = lstDataSetVariable.SelectedIndex;
				lstDataSetVariable.Items.RemoveAt(selectedIndex);
				if (itemCount - 1 == selectedIndex) { selectedIndex--; }
				index = _dsSelectedData.Tables.IndexOf(selectedItem);
				_dsSelectedData.Tables.RemoveAt(index);
				if (_dsSelectedDataTemp.Tables.Contains(selectedItem))
				{
					_dsSelectedDataTemp.Tables.Remove(selectedItem);
				}
				if (_datasetName != string.Empty)
				{
					commandText = string.Format("select SETUPVARIABLEDATASETID from SETUPVARIABLEDATASETS where setupid={0} and SETUPVARIABLEDATASETNAME='{1}'", CommonClass.ManageSetup.SetupID, _datasetName);
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					int ID = int.Parse(obj.ToString());
					commandText = string.Format("select SetUpVariableID from SetUpVariables where SetUpVariableName='{0}' and SETUPVARIABLEDATASETID={1}", selectedItem, ID);
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					setupVariableID = obj.ToString();
					commandText = string.Format("delete from SetUpGeographicVariables where SetUpVariableID={0}", setupVariableID);
					int rtn = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
					commandText = string.Format("select SetUpVariableDataSetID from SetUpVariableDataSets where SetUpVariableDataSetName='{0}'", txtDataSetName.Text);
					obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
					variableDatasetID = obj.ToString();
					commandText = string.Format("delete from SETUPVARIABLES where SetUpVariableID={0}", setupVariableID);
					rtn = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

					if (lstDataSetVariable.Items.Count == 0)
					{
						DataTable dt = new DataTable();
						olvData.DataSource = dt;
					}
				}
				lstDataSetVariable.SelectedIndex = selectedIndex;
				System.Threading.Thread.Sleep(300);
				WaitClose();
			}
			catch (Exception ex)
			{
				WaitClose();
				Logger.LogError(ex);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		TipFormGIF waitMess = new TipFormGIF(); bool sFlog = true;

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

		private void btnOutput_Click(object sender, EventArgs e)
		{
			try
			{
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();
				saveFileDialog1.Filter = "CSV File|*.CSV";
				saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Data Files";
				saveFileDialog1.RestoreDirectory = true;

				if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
				{ return; }
				string fileName = saveFileDialog1.FileName;
				DataTable dtOut = new DataTable();

				dtOut.Columns.Add("Column", typeof(int));
				dtOut.Columns.Add("Row", typeof(int));
				dtOut.Columns.Add("Variable", typeof(double));
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				string commandText = string.Empty;
				int outputRowsNumber = 50;
				commandText = "select count(*) from SetupGeoGraphicvariables";
				int count = (int)fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				if (count < outputRowsNumber) { outputRowsNumber = count; }
				commandText = string.Format("select first {0} Ccolumn, Row,VValue from Setupgeographicvariables", outputRowsNumber);
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					DataRow newdr = dtOut.NewRow();
					newdr["Column"] = Convert.ToInt32(dr["Ccolumn"]);
					newdr["Row"] = Convert.ToDouble(dr["Row"]);
					newdr["Variable"] = Convert.ToDouble(dr["VValue"]);
					dtOut.Rows.Add(newdr);
				}
				CommonClass.SaveCSV(dtOut, fileName);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
		private void CopyDatabase()
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();

			try
			{
				string commandText = string.Empty;
				int maxID = 0;
				int minID = 0;
				object rVal = null;

				//check and see if name is used
				commandText = string.Format("Select SetupVariableDATASETNAME from SetupVariableDATASETS WHERE SetupVariableDATASETNAME = '{0}'", txtDataSetName.Text.Trim());
				rVal = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

				if (rVal != null)
				{
					MessageBox.Show("Name is already used.  Please select a new name.");
					txtDataSetName.Focus();
					return;
				}

				// string msg = string.Format("Save this file associated with {0} and {1} ?", cboPollutant.GetItemText(cboPollutant.SelectedItem), txtYear.Text);
				string msg = "Copy Variable Data Set";
				DialogResult result = MessageBox.Show(msg, "Confirm Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (result == DialogResult.No) return;
				//getting a new dataset id
				if (_newDataSetID == null)
				{
					commandText = commandText = "select max(SetupVariableDataSetID) from SetupVariableDataSets";
					_newDataSetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
				}
				// first, create a new variable data set
				//the 'F' is for the LOCKED column in SetupVariableDataSets.  This is being added and is not a predefined.
				commandText = string.Format("insert into SetupVariableDataSets(SetupVariableDATASETID, SETUPID, SETUPVARIABLEDATASETNAME, LOCKED) "
								 + " values ({0},{1},'{2}', 'F')", _newDataSetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text);
				fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

				// then, fill the setup variables table with copies of records linked to the original variable dataset
				// get max id for all records
				commandText = "select max(SetupVariableID) from SetupVariables ";
				maxID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
				// get minimum id (in old dataset)
				commandText = string.Format("select min(SetupVariableID) from SetupVariables "
												+ " where SETUPVARIABLEDATASETID = {0}", _oldDataSetID);
				minID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

				//inserting - copying the locked data to the new data set
				commandText = string.Format("insert into SetupVariables(SetupVariableID, SetupVariableDataSetID, SetupVariableName, GridDefinitionID, MetaDataID) " +
											"SELECT SetupVariableID + ({0} - {1}) + 1, " +
											"{2}, SetupVariableName, GridDefinitionID, MetaDataID " +
											"FROM SetupVariables WHERE SetupVariableDataSetID = {3}", maxID, minID, _newDataSetID, _oldDataSetID);

				fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

				// now copy the geographic values to the new set
				commandText = string.Format("INSERT INTO SetupGeographicVariables(SetupVariableID, CCOLUMN, \"ROW\", VVALUE) "
												+ "SELECT {0} + 1 + C.SetupVariableID - {1}, CCOLUMN, \"ROW\", VVALUE "
												+ "from SetupGeographicVariables as C INNER JOIN SetupVariables AS P ON C.SetupVariableID = P.SetupVariableID "
												+ "WHERE P.SetupVariableDataSetID = {2} ", maxID, minID, _oldDataSetID);

				fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

				// now copy the global values to the new set
				commandText = string.Format("INSERT INTO SetupGlobalVariables(SetupVariableID, VVALUE) "
												+ "SELECT {0} + 1 + C.SetupVariableID - {1}, VVALUE "
												+ "from SetupGlobalVariables as C INNER JOIN SetupVariables AS P ON C.SetupVariableID = P.SetupVariableID "
												+ "WHERE P.SetupVariableDataSetID = {2} ", maxID, minID, _oldDataSetID);

				fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

				_metadataObj = new MetadataClassObj();
				_metadataObj.DatasetId = Convert.ToInt32(_newDataSetID);
				_metadataObj.FileName = txtDataSetName.Text;

			}
			catch (Exception ex)
			{
				progBarVariable.Visible = false;
				lblProgressBar.Text = "";
				Logger.LogError(ex.Message);
			}
		}
	}
}