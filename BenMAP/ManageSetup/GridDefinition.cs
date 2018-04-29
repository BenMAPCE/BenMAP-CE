using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using System.IO;
using ESIL.DBUtility;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;


namespace BenMAP
{
    public partial class GridDefinition : FormBase
    {
        private static int REGULAR_GRID = 0;
        private enum RowColFieldsValidationCode { BOTH_EXIST = 0, BOTH_MISSING = 1, COL_MISSING = 2, ROW_MISSING = 3, DUPLICATE_PAIR = 4, INCORRECT_FORMAT = 5, UNSPECIFIED_ERROR = 6};

        public GridDefinition()
        {
            InitializeComponent();
        }

        public bool IsEditor { get; set; }
        public int _gridID;
        private int _shapeCol;
        private int _shapeRow;
        private int _columns;
        private int _rrows;
        private int _gridType;
        private int _colPerLongitude;
        private int _rowPerLatitude;
        private double _minLongitude;
        private double _minLatitude;
        public string _gridIDName = "";
        public string _shapeFileName = "";
        public string _shapeFilePath = "";
        private int _geoAreaID;

        private string _isForceValidate = string.Empty;
        private string _iniPath = string.Empty;
        //private string _strPath;
        private MetadataClassObj _metadataObj = null;

        private void GridDefinition_Load(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                cboGridType.Items.Add("Shapefile Grid");
                cboGridType.Items.Add("Regular Grid");
                cboGridType.SelectedIndex = 0;
                this.tabGrid.Region = new Region(new RectangleF(this.tpShapefileGrid.Left, this.tpShapefileGrid.Top, this.tpShapefileGrid.Width, this.tpShapefileGrid.Height));
                if (_gridIDName != string.Empty)
                {
                    IsEditor = true;
                    txtGridID.Text = _gridIDName;
                    string commandText = "select shapefilename from shapefilegriddefinitiondetails where griddefinitionid=" + _gridID + "";
                    object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);

                    // get the selected grid's default admin layer status
                    commandText = "select ISADMIN from GRIDDEFINITIONS where GRIDDEFINITIONID =" + _gridID + "";
                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    
                    if(obj!=null)
                    {
                        if (Convert.ToChar(obj) == 'T')
                        {
                            chkBoxIsAdmin.Checked = true;
                        }
                        else
                        {
                            chkBoxIsAdmin.Checked = false;
                        }
                    } 

                    // get the selected grid's default outline color
                    commandText = "select OUTLINECOLOR from GRIDDEFINITIONS where GRIDDEFINITIONID =" + _gridID + "";
                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);

                    if(obj!=null)
                    {
                        //Get the line color for this layer from the GridDefinitions table
                        Color lineColor = System.Drawing.ColorTranslator.FromHtml(Convert.ToString(obj));
                        btnAdminColor.BackColor = lineColor;

                    } else
                    {
                        //Use a default color in case there is not a color specified in the table.
                        btnAdminColor.BackColor = Color.Coral; 
                    }
                    

                    if (obj == null)
                    {
                        commandText = "select MinimumLatitude,MinimumLongitude,ColumnsPerLongitude,RowsPerLatitude,shapeFileName from RegularGridDefinitiondetails where GridDefinitionID=" + _gridID + "";
                        System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                        cboGridType.SelectedIndex = 1;
                        txtMinimumLatitude.Text = ds.Tables[0].Rows[0]["MinimumLatitude"].ToString();
                        txtMinimumLongitude.Text = ds.Tables[0].Rows[0]["MinimumLongitude"].ToString();
                        nudColumnsPerLongitude.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["ColumnsPerLongitude"]);
                        nudRowsPerLatitude.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["RowsPerLatitude"]);
                        commandText = "select Columns,Rrows from GridDefinitions where GridDefinitionID=" + _gridID + "";
                        ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                        nudColumns.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["Columns"]);
                        nudRows.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["Rrows"]);
                    }
                    else
                    {
                        cboGridType.SelectedIndex = 0;
                        lblShapeFileName.Text = obj.ToString();
                    }
                    //Check to see if a geographic area exists for this grid definition
                    commandText = string.Format("select GEOGRAPHICAREAID from GEOGRAPHICAREAS WHERE GEOGRAPHICAREANAME = '{0}' and GRIDDEFINITIONID={1}", _gridIDName, _gridID);
                    _geoAreaID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                    if(_geoAreaID > 0)
                    {
                        chkBoxUseAsGeographicArea.Checked = true;
                    }

                }
                else
                {
                    IsEditor = false;
                    int number = 0;
                    int GridDefinitionID = 0;
                    do
                    {
                        string comText = "select GridDefinitionID from GridDefinitions where GridDefinitionName=" + "'GridDefinition" + Convert.ToString(number) + "'";
                        GridDefinitionID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (GridDefinitionID > 0);
                    txtGridID.Text = "GridDefinition" + Convert.ToString(number - 1);
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }

        private void AddLayer(string strPath)
        {
            try
            {
                mainMap.ProjectionModeReproject = ActionMode.Never; 
                mainMap.ProjectionModeDefine = ActionMode.Never;
                mainMap.Layers.Clear();
                mainMap.Layers.Add(strPath);                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private RowColFieldsValidationCode ValidateColumnsRows(string strPath, bool offerAdd)
        {
            try
            {


                IFeatureSet fs = FeatureSet.Open(strPath);
                List<int> lsCol = new List<int>();
                List<int> lsRow = new List<int>();

                //search for ROW and COL fields
                int icol = -1;
                int irow = -1;
                for (int i = 0; i < fs.DataTable.Columns.Count; i++)
                {
                    // JAnderton@IEc - 2017-02-27 - Fixing code to properly handle shapefile with "col" and "column" (such as US 12km Clipped)
                    // If both exist, we will use "col"
                    if (fs.DataTable.Columns[i].ToString().ToLower() == "row")
                    {
                        irow = i;
                    }
                    else if (fs.DataTable.Columns[i].ToString().ToLower() == "col")
                    {
                        icol = i;
                    }
                    else if (icol == -1 && fs.DataTable.Columns[i].ToString().ToLower() == "column")
                    {
                        icol = i;
                    }
                }

                //if both fields are missing
                if ((icol < 0) && (irow < 0))
                {
                    if (offerAdd)
                    {
                        
                        DialogResult result = MessageBox.Show("This shapefile does not have the required ROW and COL fields.  Would you like BenMAP-CE to add them?", "Add Fields", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            fs.Close();
                            AddColumnsRows(strPath);
                            return RowColFieldsValidationCode.BOTH_EXIST;
                        }
                        else 
                        {
                            fs.Close();
                            return RowColFieldsValidationCode.BOTH_MISSING;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This shapefile does not have the required ROW and COL fields.");
                        fs.Close();
                        return RowColFieldsValidationCode.BOTH_MISSING;
                    }
                }
                else if (icol < 0) 
                {
                    MessageBox.Show("This shapefile does not have the required COL field.");
                    //txtShapefile.Text = "";
                    fs.Close();
                    return RowColFieldsValidationCode.COL_MISSING;
                }
                else if (irow < 0)
                {
                    MessageBox.Show("This shapefile does not have the required ROW field.");
                    //txtShapefile.Text = "";
                    fs.Close();
                    return RowColFieldsValidationCode.ROW_MISSING;
                }

                //ensure that ROW, COL fields contain integers
                foreach (DataRow dr in fs.DataTable.Rows)
                {
                    int iTest;
                    if ((!Int32.TryParse(dr[icol].ToString(), out iTest)) || (!Int32.TryParse(dr[irow].ToString(), out iTest)))
                    {
                        MessageBox.Show("Values in the ROW and COL fields must be integers.");
                        fs.Close();
                        return RowColFieldsValidationCode.INCORRECT_FORMAT;
                    }                   
                    
                }

                //ensure that ROW, COL fields contain integers
                List<string> lstPairs = new List<string>();
                foreach (DataRow dr in fs.DataTable.Rows)
                {
                    string col = dr[icol].ToString();
                    string row = dr[irow].ToString();
                    string pair = col  + "_" + row;

                    if (lstPairs.Contains(pair))
                    {
                        MessageBox.Show("Duplicate ROW and COL pair found. ROW " + row + ", COL " + col);
                        fs.Close();
                        return RowColFieldsValidationCode.DUPLICATE_PAIR;
                    }
                    else 
                    {
                        lstPairs.Add(pair);
                    }                   

                }   

                //rename COL, ROW field names to upper case
                fs.DataTable.Columns[icol].ColumnName = "COL";
                fs.DataTable.Columns[irow].ColumnName = "ROW";
                fs.Save();
                fs.Close();

                return RowColFieldsValidationCode.BOTH_EXIST;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return RowColFieldsValidationCode.UNSPECIFIED_ERROR;
            }
        }

        private void AddColumnsRows(string strPath)
        {
            try
            {
                IFeatureSet fs = FeatureSet.Open(strPath);
                
                fs.DataTable.Columns.Add("COL", typeof(int));
                fs.DataTable.Columns.Add("ROW", typeof(int));

                int iRow = 0;
                foreach (DataRow dr in fs.DataTable.Rows)
                {
                    dr["COL"] = 1; //set COL to 1

                    iRow++;
                    dr["ROW"] = iRow; //increment ROW                
                }

                fs.SaveAs(strPath, true);
                fs.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void GetColumnsRows(string strPath)
        {
            try
            {
                IFeatureSet fs = FeatureSet.Open(strPath);
                List<int> lsCol = new List<int>();
                List<int> lsRow = new List<int>();                

                foreach (DataRow dr in fs.DataTable.Rows)
                {
                    lsCol.Add(Convert.ToInt32(dr["COL"].ToString()));
                    lsRow.Add(Convert.ToInt32(dr["ROW"].ToString()));
                }
               
                _shapeCol = lsCol.Max();
                _shapeRow = lsRow.Max();  
              
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private bool CheckforSupportingfiles(string strPath)
        {
            bool bPassed = true;
            FileInfo fInfo = new FileInfo(strPath);
            string strDir = fInfo.DirectoryName;
            string fName = fInfo.Name.Substring(0,fInfo.Name.Length - fInfo.Extension.Length);
            if(!File.Exists(Path.Combine(strDir, fName + ".shx")))
            {
                MessageBox.Show(string.Format("Could not find file {0}.shx", fName));
                return false;
            }
            if (!File.Exists(Path.Combine(strDir, fName + ".prj")))
            {
                MessageBox.Show(string.Format("Could not find file {0}.prj", fName));
                return false;
            }
            if (!File.Exists(Path.Combine(strDir, fName + ".dbf")))
            {
                MessageBox.Show(string.Format("Could not find file {0}.dbf", fName));
                return false;
            }
            return bPassed;
        }
        
        private void cboGridType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboGridType.SelectedIndex == 0)
            {
                tabGrid.Controls.Clear();
                tabGrid.TabPages.Add(tpShapefileGrid);

                _gridType = 1;
            }
            if (cboGridType.SelectedIndex == 1)
            {
                tabGrid.Controls.Clear();
                tabGrid.TabPages.Add(tpgRegularGrid);
                _gridType = 0;
            }
        }

        string _filePath = string.Empty;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        private void btnShapefile_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog.Filter = "Shapefiles|*.shp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                _filePath = openFileDialog.FileName;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //set shapefile path and name variables
                    txtShapefile.Text = openFileDialog.FileName; 
                    lblShapeFileName.Text = System.IO.Path.GetFileNameWithoutExtension(txtShapefile.Text);
                    _shapeFilePath = openFileDialog.FileName;

                    if(CheckforSupportingfiles(_shapeFilePath))
                    {
                        DotSpatial.Projections.ProjectionInfo WGS1984prj = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        bool ProjectionOK = false;
                        //get projection info from the ESRI shape file's .prj file.
                        FileInfo fInfo = new FileInfo(_shapeFilePath);
                        string strDir = fInfo.DirectoryName;
                        string fName = fInfo.Name.Substring(0,fInfo.Name.Length - fInfo.Extension.Length);
                        string prjfile = Path.Combine(strDir, fName + ".prj");
                        string WGS1984ShapeFilePath = Path.Combine(strDir, fName + "_WGS1984" + ".shp");

                        if (File.Exists(prjfile))    //check for acceptable projection (WGS1984)
                        {
                            //sets the ESRI PCS to the ESRI .prj file
                            ProjectionInfo pESRI = new ProjectionInfo();
                            StreamReader re = File.OpenText(prjfile);
                            pESRI.ParseEsriString(re.ReadLine());
                            re.Close();
                            if (pESRI.Equals(WGS1984prj)) ProjectionOK = true;  
                        }
                        if (!ProjectionOK)  //Then reproject it to WGS1984
                        {
                            string title = "WARNING: The shape file is not in the correct projection: WGS1984";
                            string message = "BenMAP-CE will attempt to reproject to WGS1984.";                            
                            message = message.PadRight(title.Length + 20); // padding necessary so full title is displayed in message box
                            MessageBox.Show(message, title);
                            string originalShapeFilePath = _shapeFilePath;
                            _shapeFilePath = WGS1984ShapeFilePath;
                            if (File.Exists(_shapeFilePath)) CommonClass.DeleteShapeFileName(_shapeFilePath);
                            IFeatureSet fs = FeatureSet.Open(originalShapeFilePath);
                            fs.Reproject(WGS1984prj); //reproject
                            fs.SaveAs(_shapeFilePath, true);
                            fs.Close();

                            //set shapefile path and name variables to reprojected file
                            txtShapefile.Text = _shapeFilePath;
                            lblShapeFileName.Text = System.IO.Path.GetFileNameWithoutExtension(txtShapefile.Text);                           
                        }

                        IFeatureSet fsVertices = FeatureSet.Open(_shapeFilePath);
                        int numVertices = fsVertices.Vertex.Count();

                        // Add the grid 
                        AddLayer(_shapeFilePath);
                        //get columns, rows
                        if (ValidateColumnsRows(_shapeFilePath, true) != RowColFieldsValidationCode.BOTH_EXIST)
                        {
                            return;
                            //AddColumnsRows(_shapeFilePath);
                        }
                        GetColumnsRows(_shapeFilePath);
                        lblCol.Text = _shapeCol.ToString();
                        lblRow.Text = _shapeRow.ToString();
                        GetMetadata();                       
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
  
        private void GetMetadata()
        {
            _metadataObj = new MetadataClassObj();
            Metadata metadata = new Metadata(_shapeFilePath);
            _metadataObj = metadata.GetMetadata();
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {           
            
            try
            {
                if (cboGridType.SelectedIndex == 0)
                {
                    mainMap.ProjectionModeReproject = ActionMode.Never; mainMap.ProjectionModeDefine = ActionMode.Never;
                    mainMap.Layers.Clear();
                    if (txtShapefile.Text != "")
                    {
                        mainMap.Layers.Add(txtShapefile.Text.ToString());
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(lblShapeFileName.Text)) return;
                        string strPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + lblShapeFileName.Text + ".shp";
                        AddLayer(strPath);
                        //get columns, rows
                        if (ValidateColumnsRows(strPath, false) != RowColFieldsValidationCode.BOTH_EXIST)
                        {
                            return;
                        }
                        GetColumnsRows(strPath);
                        lblCol.Text = _shapeCol.ToString();
                        lblRow.Text = _shapeRow.ToString();
                    }
                }
                else
                {
                    try
                    {
                        _columns = int.Parse(nudColumns.Value.ToString());
                        _rrows = int.Parse(nudRows.Value.ToString());
                        _colPerLongitude = int.Parse(nudColumnsPerLongitude.Value.ToString());
                        _rowPerLatitude = int.Parse(nudRowsPerLatitude.Value.ToString());
                        _minLongitude = System.Convert.ToDouble(txtMinimumLongitude.Text);
                        _minLatitude = System.Convert.ToDouble(txtMinimumLatitude.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Input data was not in a correct format.");
                        return;
                    }
                    if (Math.Abs(_minLongitude) > 180 || Math.Abs(_minLatitude) > 90) { MessageBox.Show("Longitude must be less than 180 degrees and latitude less than 90 degrees. Please check the longitude and latitude values."); return; }
                    FeatureSet fs = getFeatureSetFromRegularGrid(_columns, _rrows, _minLatitude, _minLongitude, _colPerLongitude, _rowPerLatitude);
                    if (fs != null)
                    {
                        mainMap.ProjectionModeReproject = ActionMode.Never;
                        mainMap.ProjectionModeDefine = ActionMode.Never;
                        mainMap.Layers.Clear();
                        mainMap.Layers.Add(fs);

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            _columns = int.Parse(nudColumns.Value.ToString());
            _rrows = int.Parse(nudRows.Value.ToString());
            try
            {
                String rasterFileLoc = txtb_popGridLoc.Text;

                if (cboGridType.SelectedIndex == 0)
                {
                    _shapeFileName = lblShapeFileName.Text;
                }
                else
                {
                    _gridType = 0;
                    try
                    {
                        _columns = int.Parse(nudColumns.Value.ToString());
                        _rrows = int.Parse(nudRows.Value.ToString());
                        _colPerLongitude = int.Parse(nudColumnsPerLongitude.Value.ToString());
                        _rowPerLatitude = int.Parse(nudRowsPerLatitude.Value.ToString());
                        _minLongitude = System.Convert.ToDouble(txtMinimumLongitude.Text);
                        _minLatitude = System.Convert.ToDouble(txtMinimumLatitude.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Input data was not in a correct format.");
                        return;
                    }
                }
                if (IsEditor) // Editing an existing grid definition
                {

                    commandText = string.Format("select Ttype from GridDefinitions where GridDefinitionID=" + _gridID + "");
                    int type = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                    // If this grid definition is used as a geographic area, make sure the name stays in sync
                    if(chkBoxUseAsGeographicArea.Checked)
                    {
                        if(_geoAreaID == 0) // The option was just enabled
                        {
                            commandText = string.Format("select coalesce(max(GEOGRAPHICAREAID),0) from GEOGRAPHICAREAS");
                            _geoAreaID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                            commandText = string.Format("INSERT INTO GEOGRAPHICAREAS (GeographicAreaID, GeographicAreaName, GridDefinitionID, EntireGridDefinition)  VALUES({0},'{1}',{2},'Y')", _geoAreaID, txtGridID.Text, _gridID);
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }
                        else // The option was already enabled. Make sure the name is in sync.
                        {
                            commandText = string.Format("UPDATE GEOGRAPHICAREAS SET GeographicAreaName = '{0}' WHERE GEOGRAPHICAREAID = {1}", txtGridID.Text, _geoAreaID);
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }
                    }
                    else // The option may have been disabled. Clean up.
                    {
                        commandText = string.Format("DELETE FROM GEOGRAPHICAREAS WHERE GEOGRAPHICAREAID = {0}", _geoAreaID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }

                    //Set the default admin layer
                    if (chkBoxIsAdmin.Checked)
                    {
                        commandText = string.Format("Update GridDefinitions set ISADMIN='{0}' WHERE GridDefinitionID={1}", "T", _gridID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }
                    else
                    {
                        commandText = string.Format("Update GridDefinitions set ISADMIN='{0}' WHERE GridDefinitionID={1}", "F", _gridID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }

                    //Change the outline color
                    string lineColor = System.Drawing.ColorTranslator.ToHtml(btnAdminColor.BackColor);
                    commandText = string.Format("Update GridDefinitions set OUTLINECOLOR='{0}' WHERE GridDefinitionID={1}", lineColor, _gridID);
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                    switch (_gridType)
                    {
                        case 1:
                            if (_shapeFilePath == string.Empty) // The user may have just updated the name
                            {
                                commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}' WHERE GridDefinitionID={1}", txtGridID.Text, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                this.DialogResult = DialogResult.OK;
                                return;
                            }
                            if (type == 1)
                            {

                                commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GridDefinitionID={4}", txtGridID.Text, _shapeCol, _shapeRow, _gridType, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp"))
                                {
                                    RenameorOverride ro = new RenameorOverride();
                                    DialogResult dr = ro.ShowDialog();
                                    if (dr == DialogResult.Cancel)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        if (dr == DialogResult.OK)
                                        {
                                            Renamefile frm = new Renamefile();
                                            frm.newfileName = _shapeFileName;
                                            frm.manage = "GridDefinition";
                                            DialogResult dresult = frm.ShowDialog();
                                            if (dresult == DialogResult.OK)
                                            {
                                                _shapeFileName = frm.newfileName;
                                            }
                                            else
                                                return;
                                        }
                                        commandText = string.Format("Update ShapefileGriddefinitiondetails set shapefilename='{0}' where GridDefinitionID={1}", _shapeFileName, _gridID);
                                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    }
                                }
                            }
                            else
                            {
                                DialogResult rtn = MessageBox.Show("Replace the 'Regular Grid' with 'Shapefile'?", "", MessageBoxButtons.YesNo);
                                if (rtn == DialogResult.Yes)
                                {
                                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp"))
                                    {
                                        RenameorOverride ro = new RenameorOverride();
                                        DialogResult dr = ro.ShowDialog();
                                        if (dr == DialogResult.Cancel)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            if (dr == DialogResult.OK)
                                            {
                                                Renamefile frm = new Renamefile();
                                                frm.newfileName = _shapeFileName;
                                                frm.manage = "GridDefinition";
                                                DialogResult dresult = frm.ShowDialog();
                                                if (dresult == DialogResult.OK)
                                                {
                                                    _shapeFileName = frm.newfileName;
                                                }
                                                else
                                                    return;
                                            }
                                            //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined.
                                            commandText = string.Format("insert into ShapeFileGridDefinitionDetails (GridDefinitionID,ShapeFileName) values ({0},'{1}', 'F')", _gridID, _shapeFileName);
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                        }
                                    }
                                    commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GridDefinitionID={4}", txtGridID.Text, _shapeCol, _shapeRow, _gridType, _gridID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    commandText = "delete From RegularGridDefinitionDetails where GridDefinitionID=" + _gridID + "";
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                }
                                else { return; }
                            }

                            IFeatureSet fs = FeatureSet.Open(_shapeFilePath);
                            try
                            {
                                if (fs.DataTable.Columns["ROW"].DataType == typeof(System.String) || fs.DataTable.Columns["COL"].DataType == typeof(System.String))
                                {
                                    for (int iDt = 0; iDt < fs.DataTable.Rows.Count; iDt++)
                                    {
                                        int r = (int)Convert.ToDouble(fs.DataTable.Rows[iDt]["ROW"].ToString());
                                        int c = (int)Convert.ToDouble(fs.DataTable.Rows[iDt]["COL"].ToString());
                                        fs.DataTable.Rows[iDt]["ROW"] = r;
                                        fs.DataTable.Rows[iDt]["COL"] = c;
                                    }
                                }
                                fs.SaveAs(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp", true);
                            }
                            finally
                            {
                                fs.Close();
                            }
                            break;

                        case 0:
                            if (Math.Abs(_minLongitude) > 180 || Math.Abs(_minLatitude) > 90) { MessageBox.Show("Longitude must be less than 180 degrees and latitude less than 90 degrees. Please check the longitude and latitude values."); return; }
                            if (type == 0)
                            {
                                commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GridDefinitionID={4}", txtGridID.Text, _columns, _rrows, _gridType, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                commandText = string.Format("Update RegularGriddefinitionDetails set minimumlatitude={0},minimumlongitude={1},columnsperlongitude={2},rowsperlatitude={3} where GridDefinitionID={4}", _minLatitude, _minLongitude, _colPerLongitude, _rowPerLatitude, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                            }
                            else
                            {
                                DialogResult rtn = MessageBox.Show("Replace the 'Shapefile' with 'Regular Grid'?", "", MessageBoxButtons.YesNo);
                                if (rtn == DialogResult.Yes)
                                {
                                    commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GridDefinitionID={4}", txtGridID.Text, _columns, _rrows, _gridType, _gridID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    commandText = "delete From ShapefileGriddefinitiondetails where GridDefinitionID=" + _gridID + "";
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    commandText = string.Format("insert into RegularGriddefinitionDetails (GridDefinitionID,MinimumLatitude,MinimumLongitude,ColumnsPerLongitude,RowsPerLatitude,ShapeFileName) values ({0},{1},{2},{3},{4},'{5}')", _gridID, _minLatitude, _minLongitude, _colPerLongitude, _rowPerLatitude, txtGridID.Text);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                }
                                else { return; }

                            }
                            fs = getFeatureSetFromRegularGrid(_columns, _rrows, _minLatitude, _minLongitude, _colPerLongitude, _rowPerLatitude);
                            try
                            {
                                fs.SaveAs(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + txtGridID.Text + ".shp", true);
                            }
                            finally
                            {
                                fs.Close();
                            }
                            break;
                    }
                    if (!chkBoxCreatePercentage.Checked)
                    {
                        WaitShow("Updating the grid definition...");
                        commandText = string.Format("delete from GridDefinitionPercentageEntries where PercentageID in ( select PercentageID  from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0})", _gridID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0}", _gridID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        this.DialogResult = DialogResult.OK;
                        WaitClose();
                        return;
                    }
                }

                else // NEW GRID DEFINITION
                {
                    //ensure shapefile is correctly formatted.
                    if(_gridType != REGULAR_GRID)
                    {
                        if (ValidateColumnsRows(_shapeFilePath, false) != RowColFieldsValidationCode.BOTH_EXIST)
                        {
                            return;
                        }
                    }


                    commandText = "select GridDefinitionID from GridDefinitions where GridDefinitionName='" + txtGridID.Text + "' and SetupID=" + CommonClass.ManageSetup.SetupID;
                    object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    if (obj != null)
                    {
                        MessageBox.Show("This grid definition name is already in use. Please enter a different name.");
                        return;
                    }
                   
                    //_gridID =  _metadataObj.DatasetId;
                    commandText = string.Format("select max(GRIDDEFINITIONID) from GRIDDEFINITIONS");
                    _gridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                    //_metadataObj.DatasetId = _gridID;
                    string _filePath = string.Empty;
                   
                    //Set the default admin layer
                    if (chkBoxIsAdmin.Checked)
                    {

                        commandText = string.Format("Update GridDefinitions set ISADMIN='{0}' WHERE GridDefinitionID={1}", "T", _gridID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }
                    else
                    {
                        commandText = string.Format("Update GridDefinitions set ISADMIN='{0}' WHERE GridDefinitionID={1}", "F", _gridID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }

                    switch (_gridType)
                    {
                        case 1:
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp"))
                            {
                                Renamefile frm = new Renamefile();
                                frm.newfileName = _shapeFileName;
                                frm.manage = "GridDefinition";
                                DialogResult dr = frm.ShowDialog();
                                if (dr == DialogResult.OK)
                                {
                                    _shapeFileName = frm.newfileName;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            //The 'F' is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                            commandText = string.Format("INSERT INTO GRIDDEFINITIONS (GridDefinitionID,SetUpID,GridDefinitionName,Columns,Rrows,Ttype,DefaultType, LOCKED) VALUES(" + _gridID + ",{0},'{1}',{2},{3},{4},{5}, 'F')", CommonClass.ManageSetup.SetupID, txtGridID.Text, _shapeCol, _shapeRow, _gridType, 0);
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            //The 'F' is for the locked column in the SapeFileGridDefinitionDetails - this is imported and not predefined
                            commandText = string.Format("INSERT INTO SHAPEFILEGRIDDEFINITIONDETAILS (GridDefinitionID,ShapeFileName, LOCKED)  VALUES(" + _gridID + ",'{0}', 'F')", _shapeFileName);
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp");

                            //open shapeFilePath.  This could be original or reprojected file.  See btnShapefile_Click event.
                            IFeatureSet fs = FeatureSet.Open(_shapeFilePath);

                            try
                            {
                                if (fs.DataTable.Columns["ROW"].DataType == typeof(System.String) || fs.DataTable.Columns["COL"].DataType == typeof(System.String))
                                {
                                    for (int iDt = 0; iDt < fs.DataTable.Rows.Count; iDt++)
                                    {
                                        int r = (int)Convert.ToDouble(fs.DataTable.Rows[iDt]["ROW"].ToString());
                                        int c = (int)Convert.ToDouble(fs.DataTable.Rows[iDt]["COL"].ToString());
                                        fs.DataTable.Rows[iDt]["ROW"] = r;
                                        fs.DataTable.Rows[iDt]["COL"] = c;
                                    }
                                }
                                fs.SaveAs(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp", true);
                                _filePath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp";

                                //update metadata, we have to do this here in case the file is renamed above
                                _metadataObj.FileName = _shapeFileName;
                                _metadataObj.DatasetId = _gridID; //ensure datasetid of metadata obj is griddefinitionid
                            }
                            finally
                            {
                                fs.Close();
                            }
                            //If this new grid definition is to be used as a geographic area, add the necessary records
                            if (chkBoxUseAsGeographicArea.Checked)
                            {
                                commandText = string.Format("select coalesce(max(GEOGRAPHICAREAID),0) from GEOGRAPHICAREAS");
                                _geoAreaID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                                commandText = string.Format("INSERT INTO GEOGRAPHICAREAS (GeographicAreaID, GeographicAreaName, GridDefinitionID, EntireGridDefinition)  VALUES({0},'{1}',{2},'Y')", _geoAreaID, txtGridID.Text, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            }
                            break;
                        case 0:
                            DialogResult rtn = MessageBox.Show("Save this grid?", "", MessageBoxButtons.YesNo);
                            if (rtn == DialogResult.No) { this.DialogResult = DialogResult.Cancel; return; }
                            else
                            {
                                //The 'F' is for the column LOCKED in GRIDDEFINITIONS - it is being imported and not predefined
                                commandText = string.Format("INSERT INTO GRIDDEFINITIONS (GridDefinitionID, SetUpID, GridDefinitionName, Columns, Rrows, Ttype, DefaultType,  LOCKED) VALUES(" + _gridID + ",{0},'{1}',{2},{3},{4}, {5},'F')", CommonClass.ManageSetup.SetupID, txtGridID.Text, _columns, _rrows, _gridType, 0);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                if (Math.Abs(_minLongitude) > 180 || Math.Abs(_minLatitude) > 90) { MessageBox.Show("Please input valid longitude and latitude values."); return; }
                                commandText = string.Format("INSERT INTO RegularGridDefinitionDetails (GridDefinitionID,MinimumLatitude,MinimumLongitude,ColumnsPerLongitude,RowsPerLatitude,ShapeFileName)  VALUES ({0},{1},{2},{3},{4},'{5}')", _gridID, txtMinimumLatitude.Text, txtMinimumLongitude.Text, nudColumnsPerLongitude.Value, nudRowsPerLatitude.Value, txtGridID.Text);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                fs = getFeatureSetFromRegularGrid(_columns, _rrows, _minLatitude, _minLongitude, _colPerLongitude, _rowPerLatitude);
                                CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + txtGridID.Text + ".shp");

                                try
                                {
                                    fs.SaveAs(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + txtGridID.Text + ".shp", true);
                                    _filePath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + txtGridID.Text + ".shp";
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex);
                                }
                                finally
                                {
                                    fs.Close();
                                }
                            }
                            break;
                    }
                    if (!chkBoxCreatePercentage.Checked)
                    {
                        this.DialogResult = DialogResult.OK;
                        saveMetadata();
                        return;
                    }

                }           
                /*
                Replacing the following code with new crosswalk algorithm
                */

                //code reaches this line of execution if crosswalks are being created.
                lblprogress.Visible = true;
                lblprogress.Refresh();
                progressBar1.Visible = true;
                progressBar1.Refresh();
                BenMAPGrid addBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(_gridID);
                try
                {

                    commandText = "select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,ShapeFileGridDefinitionDetails b " +
                                       " where a.GridDefinitionID=b.GridDefinitionID and a.TType=1  and a.SetupID=" + addBenMAPGrid.SetupID +
                                       " union " +
                                       " select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,RegularGridDefinitionDetails b " +
                                       " where a.GridDefinitionID=b.GridDefinitionID and a.TType=0  and a.SetupID=" + addBenMAPGrid.SetupID;
                    fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    commandText = string.Format("delete from GridDefinitionPercentageEntries where PercentageID in ( select PercentageID  from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0})", addBenMAPGrid.GridDefinitionID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    commandText = string.Format("delete from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0}", addBenMAPGrid.GridDefinitionID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    string AppPath = Application.StartupPath;
                    if (ds.Tables[0].Rows.Count == 1) this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    int counter = 0;

                    this.Enabled = false;
                    progressBar1.Step = 1;
                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = ds.Tables[0].Rows.Count;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int gridDefinitionID = Convert.ToInt32(dr["GridDefinitionID"]);
                        //don't run against myself
                        if (gridDefinitionID == addBenMAPGrid.GridDefinitionID)
                        {
                            continue;
                        }


                        int bigGridID, smallGridID;
                        bigGridID = gridDefinitionID;
                        smallGridID = addBenMAPGrid.GridDefinitionID;
                        Console.WriteLine("Starting grid " + bigGridID + " against " + smallGridID);
                        Configuration.ConfigurationCommonClass.creatPercentageToDatabaseForSetup(bigGridID, smallGridID, CommonClass.ManageSetup.SetupName);
                        counter++;
                        progressBar1.Value = counter;
                        Application.DoEvents();
                    }
                }

                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
                
                this.Enabled = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                MessageBox.Show(ex.Message);
            }

        }
             
        private void saveMetadata()
        {
            if (_metadataObj == null)
            {
                GetMetadata();
            }
            _metadataObj.DatasetTypeId = SQLStatementsCommonClass.getDatasetID("GridDefinition");

            if(!SQLStatementsCommonClass.insertMetadata(_metadataObj))
            {
                MessageBox.Show("Failed to save Metadata.");
            }

        }

        private int iAsyns = 0;
        public List<string> lstAsyns = new List<string>();
        Dictionary<string, List<GridRelationshipAttributePercentage>> dicAllGridPercentage = new Dictionary<string, List<GridRelationshipAttributePercentage>>();
        private void outPut(IAsyncResult ar)
        {
            try
            {
                lstAsyns.RemoveAt(0);
                progressBar1.Value++;
                Dictionary<string, List<GridRelationshipAttributePercentage>> dic = (ar.AsyncState as AsyncgetRelationshipFromBenMAPGridPercentage).EndInvoke(ar);
                if (dic != null && dic.Count > 0)
                    dicAllGridPercentage.Add(dic.ToArray()[0].Key, dic.ToArray()[0].Value);
                GC.Collect();
                if (lstAsyns.Count == 0)
                {
                    foreach (KeyValuePair<string, List<GridRelationshipAttributePercentage>> k in dicAllGridPercentage)
                    {
                        Configuration.ConfigurationCommonClass.updatePercentageToDatabase(k);

                    }

                    this.DialogResult = DialogResult.OK;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errot in asynch output function: "+ex.ToString());
            }
        }
        public delegate Dictionary<string, List<GridRelationshipAttributePercentage>> AsyncgetRelationshipFromBenMAPGridPercentage(int big, int small,String poplocation);
        public Dictionary<string, List<GridRelationshipAttributePercentage>> getRelationshipFromBenMAPGridPercentage(int big, int small,String popLocation)
        {
            try
            {

                IFeatureSet fsBig = new FeatureSet();
                IFeatureSet fsSmall = new FeatureSet();
                BenMAPGrid bigBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(big == 20 ? 18 : big);
                BenMAPGrid smallBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(small == 20 ? 18 : small);
                if (bigBenMAPGrid == null)
                    bigBenMAPGrid = new ShapefileGrid()
                    {
                        ShapefileName = "County_epa2",
                    };
                if (smallBenMAPGrid == null)
                    smallBenMAPGrid = new ShapefileGrid()
                    {
                        ShapefileName = "County_epa2",
                    };
                string bigShapefileName = "";
                string smallShapefileName = "";
                if (bigBenMAPGrid as ShapefileGrid != null)
                { bigShapefileName = (bigBenMAPGrid as ShapefileGrid).ShapefileName; }
                else
                { bigShapefileName = (bigBenMAPGrid as RegularGrid).ShapefileName; }
                if (smallBenMAPGrid as ShapefileGrid != null)
                { smallShapefileName = (smallBenMAPGrid as ShapefileGrid).ShapefileName; }
                else
                { smallShapefileName = (smallBenMAPGrid as RegularGrid).ShapefileName; }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string finsSetupname = string.Format("select setupname from setups where setupid in (select setupid from griddefinitions where griddefinitionid={0})", big);
                string setupname = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, finsSetupname));
                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + bigShapefileName + ".shp"))
                {
                    string shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + bigShapefileName + ".shp";
                    //for debugging!
                    fsBig = DotSpatial.Data.FeatureSet.Open(shapeFileName);
                    string shapeFileNameSmall = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + smallShapefileName + ".shp";
                    fsSmall = DotSpatial.Data.FeatureSet.Open(shapeFileNameSmall);


                    List<GridRelationshipAttributePercentage> lstGR = null; 
                    
                    //if (big == 20 || small == 20)
                    //{

//                        lstGR = CommonClass.IntersectionPercentageNation(fsBig, fsSmall, FieldJoinType.All, big, small);
  //                  }
    //                else
      //              {
                        lstGR = CommonClass.IntersectionPercentage(fsBig, fsSmall, FieldJoinType.All);
        //            }
                    Dictionary<string, List<GridRelationshipAttributePercentage>> dic = new Dictionary<string, List<GridRelationshipAttributePercentage>>();
                    dic.Add(small + "," + big, lstGR);
                    
                    string commandText = "select max(PercentageID) from GridDefinitionPercentages";
                    int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                    //assume all are pop based.
                    commandText = string.Format("insert into GridDefinitionPercentages(PERCENTAGEID, SOURCEGRIDDEFINITIONID, TARGETGRIDDEFINITIONID) values({0},{1},{2})", iMax, small, big);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    foreach (GridRelationshipAttributePercentage grp in lstGR)
                    {
                        commandText = string.Format("insert into GridDefinitionPercentageEntries(PERCENTAGEID, SOURCECOLUMN, SOURCEROW, TARGETCOLUMN, TARGETROW, PERCENTAGE,NORMALIZATIONSTATE) values({0},{1},{2},{3},{4},{5},{6})",
                            iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow, grp.percentage, 0);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = String.Format("Select PERCENTAGE from GridDefinitionPercentageEntries where PERCENTAGEID={0} AND SOURCECOLUMN={1} and SOURCEROW={2} AND TARGETCOLUMN={3} and TARGETROW={4};",iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow);
                        Object result = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        Console.WriteLine("got back type " + result.GetType() + " value of " + result.ToString());

                    }
                    return dic;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not write ratio:  " + ex.ToString());
                return null;
            }
        }
        private FeatureSet getFeatureSetFromRegularGrid(int Cols, int Rows, double MinLatitude, double MinLongitude, int ColsPerLongitude, int RowsPerLatitude)
        {
            try
            {
                FeatureSet fs = new FeatureSet();
                fs.DataTable.Columns.Add("Col", typeof(int));
                fs.DataTable.Columns.Add("Row", typeof(int));
                for (int i = 0; i < Cols; i++)
                {
                    for (int j = 0; j < Rows; j++)
                    {
                        List<Coordinate> lstCoordinate = new List<Coordinate>();
                        Coordinate coordinate = new Coordinate();
                        coordinate.X = MinLongitude + i * (1.0000 / Convert.ToDouble(ColsPerLongitude));
                        coordinate.Y = MinLatitude + j * (1.0000 / Convert.ToDouble(RowsPerLatitude));
                        lstCoordinate.Add(coordinate);

                        coordinate = new Coordinate();
                        coordinate.X = MinLongitude + (i + 1) * (1.0000 / Convert.ToDouble(ColsPerLongitude));
                        coordinate.Y = MinLatitude + j * (1.0000 / Convert.ToDouble(RowsPerLatitude));
                        lstCoordinate.Add(coordinate);


                        coordinate = new Coordinate();
                        coordinate.X = MinLongitude + (i + 1) * (1.0000 / Convert.ToDouble(ColsPerLongitude));
                        coordinate.Y = MinLatitude + (j + 1) * (1.0000 / Convert.ToDouble(RowsPerLatitude));
                        lstCoordinate.Add(coordinate);

                        coordinate = new Coordinate();
                        coordinate.X = MinLongitude + i * (1.0000 / Convert.ToDouble(ColsPerLongitude));
                        coordinate.Y = MinLatitude + (j + 1) * (1.0000 / Convert.ToDouble(RowsPerLatitude));
                        lstCoordinate.Add(coordinate);

                        coordinate = new Coordinate();
                        coordinate.X = MinLongitude + i * (1.0000 / Convert.ToDouble(ColsPerLongitude));
                        coordinate.Y = MinLatitude + j * (1.0000 / Convert.ToDouble(RowsPerLatitude));
                        lstCoordinate.Add(coordinate);


                        var p = new Polygon(new LinearRing(lstCoordinate.ToArray()));
                        fs.AddFeature(p);
                        fs.DataTable.Rows[i * Rows + j]["Col"] = i + 1;
                        fs.DataTable.Rows[i * Rows + j]["Row"] = j + 1;

                    }
                }
                return fs;
            }
            catch
            {
                return null;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void nudColumns_ValueChanged(object sender, EventArgs e)
        {

            nudColumns.Minimum = 1;
        }

        private void nudRows_ValueChanged(object sender, EventArgs e)
        {
            nudRows.Minimum = 1;
        }

        private void nudRowsPerLatitude_ValueChanged(object sender, EventArgs e)
        {
            nudRowsPerLatitude.Minimum = 1;
        }

        private void nudColumnsPerLongitude_ValueChanged(object sender, EventArgs e)
        {
            nudColumnsPerLongitude.Minimum = 1;
        }

        TipFormGIF waitMess = new TipFormGIF(); bool sFlog = true;
        private void ShowWaitMess()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {

                    waitMess.ShowDialog();
                    waitMess.TopMost = true;
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




        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            //ViewEditMetadata viewEMdata = new ViewEditMetadata(_shapeFilePath);
            ViewEditMetadata viewEMdata = null;
            if (_metadataObj != null)
            {
                viewEMdata = new ViewEditMetadata(_shapeFilePath, _metadataObj);
            }
            else
            {
                viewEMdata = new ViewEditMetadata(_shapeFilePath);
            }
            viewEMdata.ShowDialog();
            _metadataObj = viewEMdata.MetadataObj;
        }

        private void txtShapefile_TextChanged(object sender, EventArgs e)
        {
            btnViewMetadata.Enabled = !string.IsNullOrEmpty(txtShapefile.Text);
            //btnViewMetadata.Enabled = !string.IsNullOrEmpty(txtShapefile.Text);
            //_strPath = txtShapefile.Text;
        }

        private void btn_browsePopRaster_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd=new OpenFileDialog();
             DialogResult result =ofd.ShowDialog(); // Show the dialog.
             if (result == DialogResult.OK) // Test result.
             {
                 txtb_popGridLoc.Text=ofd.FileName;
             }

        }

        private void chkBoxCreatePercentage_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ShowCRHelp()
        {
            this.toolTip1.Show(@"To calculate health impacts and economic benefits, BenMAP
utilizes air quality, population and demographic data at
different spatial scales. To do this, the program calculates a
'percentage file' that relates data at one spatial scale to
another (e.g. 12km CMAQ grid to county).This step is
performed only once per crosswalk and the results are saved
to the database for subsequent calculations. If you do not
create the crosswalks now, BenMAP will create crosswalks as
needed during the configuration or aggregation, pooling and
valuation stages.", picCRHelp,10000);
        }

        private void HideCRHelp()
        {
            this.toolTip1.Hide(this);
        }

        private void ShowGeoAreaHelp()
        {
            this.toolTip1.Show(@"Selecting the check box means that you can assign 
health impact functions to the cells in this grid 
in the “Health Impact Functions Definition” window, 
found in the “Manage Datasets” window. By default, 
the program assigns health impact functions to all 
grid cells, but you might instead prefer to assign 
it to a discrete location, like a state, county 
or city defined by your grid.  ", picGeoAreaHelp,10000);
        }
        private void HideGeoAreaHelp()
        {
            this.toolTip1.Hide(this);
        }

        private void ShowAdminHelp()
        {
            this.toolTip1.Show(@"Use this checkbox to identify this grid as an 
'Admin Layer' which means that it will appear in 
your maps as a base map to give spatial context to 
your analytical results. Use the color selector 
button to choose an outline color for this layer.", picGeoAreaHelp,10000);
        }
        private void HideAdminHelp()
        {
            this.toolTip1.Hide(this);
        }

        private void picGeoAreaHelp_Click(object sender, EventArgs e)
        {
            ShowGeoAreaHelp();
        }

        private void picCRHelp_Click(object sender, EventArgs e)
        {
            ShowCRHelp();
        }

        private void picAdminHelp_Click(object sender, EventArgs e)
        {
            ShowAdminHelp();
        }

        private void picCRHelp_MouseEnter(object sender, EventArgs e)
        {
            ShowCRHelp();
        }

        private void picCRHelp_MouseLeave(object sender, EventArgs e)
        {
            HideCRHelp();
        }

        private void picGeoAreaHelp_MouseMove(object sender, MouseEventArgs e)
        {
            ShowGeoAreaHelp();
        }

        private void picGeoAreaHelp_MouseLeave(object sender, EventArgs e)
        {
            HideGeoAreaHelp();
        }

        private void picAdminHelp_MouseMove(object sender, MouseEventArgs e)
        {
            ShowAdminHelp();
        }

        private void picAdminHelp_MouseLeave(object sender, EventArgs e)
        {
            HideAdminHelp();
        }

        private void picGeoAreaHelp_MouseHover(object sender, EventArgs e)
        {
            ShowGeoAreaHelp();
        }

        private void picAdminHelp_MouseHover(object sender, EventArgs e)
        {
            ShowAdminHelp();
        }

        private void picAdminHelp_MouseClick(object sender, MouseEventArgs e)
        {
            ShowAdminHelp();
        }

        private void picGeoAreaHelp_MouseClick(object sender, MouseEventArgs e)
        {
            ShowGeoAreaHelp();
        }

        private void picCRHelp_MouseClick(object sender, MouseEventArgs e)
        {
            ShowCRHelp();
        }

        private void picCRHelp_MouseHover(object sender, EventArgs e)
        {
            ShowCRHelp();
        }

        private void picGeoAreaHelp_MouseEnter(object sender, EventArgs e)
        {
            ShowGeoAreaHelp();
        }

        private void picAdminHelp_MouseEnter(object sender, EventArgs e)
        {
            ShowAdminHelp();
        }

        private void btnAdminColor_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnAdminColor.BackColor = colorDialog1.Color;
            }
        }
    }
}
