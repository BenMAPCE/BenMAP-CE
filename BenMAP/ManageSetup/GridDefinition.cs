using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using ESIL.DBUtility;
using DotSpatial.Topology;
using DotSpatial.Projections;


namespace BenMAP
{
    public partial class GridDefinition : FormBase
    {

        private enum RowColFieldsValidationCode { BOTH_EXIST = 0, BOTH_MISSING = 1, COL_MISSING = 2, ROW_MISSING = 3, INCORRECT_FORMAT = 4, UNSPECIFIED_ERROR = 5};

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

        private RowColFieldsValidationCode ValidateColumnsRows(string strPath)
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
                    if (fs.DataTable.Columns[i].ToString().ToLower() == "row")
                    {
                        irow = i;
                    }

                    if (fs.DataTable.Columns[i].ToString().ToLower() == "col" || fs.DataTable.Columns[i].ToString().ToLower() == "column")
                    {
                        icol = i;
                    }
                }

                //if both fields are missing
                if ((icol < 0) && (irow < 0))
                {
                    MessageBox.Show("This shapefile does not have the required ROW and COL fields.");
                    fs.Close();
                    return RowColFieldsValidationCode.BOTH_MISSING;
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

                //rename COL, ROW field names to upper case
                fs.DataTable.Columns[icol].ColumnName = "COL";
                fs.DataTable.Columns[irow].ColumnName = "ROW";
                fs.SaveAs(strPath, true);
                fs.Close();

                return RowColFieldsValidationCode.BOTH_EXIST;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return RowColFieldsValidationCode.INCORRECT_FORMAT;
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
                    txtShapefile.Text = openFileDialog.FileName; lblShapeFileName.Text = System.IO.Path.GetFileNameWithoutExtension(txtShapefile.Text);
                    _shapeFilePath = openFileDialog.FileName;

                    if(CheckforSupportingfiles(_shapeFilePath))
                    {   
                        DotSpatial.Projections.ProjectionInfo GCSNAD83prj = DotSpatial.Projections.KnownCoordinateSystems.Geographic.NorthAmerica.NorthAmericanDatum1983;
                        bool ProjectionOK = false;
                        //get projection info from the ESRI shape file's .prj file.
                        FileInfo fInfo = new FileInfo(_shapeFilePath);
                        string strDir = fInfo.DirectoryName;
                        string fName = fInfo.Name.Substring(0,fInfo.Name.Length - fInfo.Extension.Length);
                        string prjfile = Path.Combine(strDir, fName + ".prj");
                        string GCSNAD83ShapeFilePath = Path.Combine(strDir, fName + "_GCSNAD83"+ ".shp");

                        if (File.Exists(prjfile))    //check for acceptable projection (GCS/NAD83)
                        {
                            //sets the ESRI PCS to the ESRI .prj file
                            ProjectionInfo pESRI = new ProjectionInfo();
                            StreamReader re = File.OpenText(prjfile);
                            pESRI.ParseEsriString(re.ReadLine());
                            if (pESRI.Equals(GCSNAD83prj)) ProjectionOK = true;  //MCB will need to add more code for other regions
                        }
                        if (!ProjectionOK)  //Then attempt to reporject it to GCS/NAD83
                        {   
                            MessageBox.Show("BenMAP-CE will attempt to reproject to GCS/NAD83.","WARNING: The shape file is not in the correct projection: GCS NAD83");
                            string originalShapeFilePath = _shapeFilePath;
                            _shapeFilePath = GCSNAD83ShapeFilePath;
                            if (File.Exists(_shapeFilePath)) CommonClass.DeleteShapeFileName(_shapeFilePath);
                            IFeatureSet fs = FeatureSet.Open(originalShapeFilePath);
                            fs.Reproject(GCSNAD83prj); //reproject
                            fs.SaveAs(_shapeFilePath, true);
                        }

                        // Add the grid 
                        AddLayer(_shapeFilePath);
                        //get columns, rows
                        if (ValidateColumnsRows(_shapeFilePath) != RowColFieldsValidationCode.BOTH_EXIST)
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
        //private void btnProjection_Click(object sender, EventArgs e)
        //{
        //    //declares a new ProjectionInfo for the startind and ending coordinate systems
        //    //sets the start GCS to WGS_1984
        //    //ProjectionInfo pStart = KnownCoordinateSystems.Geographic.World.WGS1984;
        //    //ProjectionInfo pESRIEnd = new ProjectionInfo();
        //    ProjectionInfo pESRIStart = new ProjectionInfo();
        //    ProjectionInfo pEnd = DotSpatial.Projections.KnownCoordinateSystems.Geographic.NorthAmerica.NorthAmericanDatum1983;
        //    //declares the point(s) that will be reprojected
        //    double[] xy = new double[2];
        //    double[] z = new double[1];
        //    //initiates a StreamReader to read the ESRI .prj file
        //    StreamReader re = File.OpenText("C:\\Program Files\\ArcGIS\\Coordinate Systems\\Projected Coordinate Systems\\UTM\\WGS 1984\\WGS 1984 UTM Zone 1N.prj");
        //    //sets the ending PCS to the ESRI .prj file
        //    pESRIEnd.ReadEsriString(re.ReadLine());
        //    //calls the reprojection function
        //    Reproject.ReprojectPoints(xy, z, pStart, pESRIEnd, 0, 1);
        //    MessageBox.Show("Points have been projected successfully.");
        //}
        //private void ChangeShapeFileProjection(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (mainMap.GetAllLayers().Count == 0) return;
        //        if (CommonClass.MainSetup.SetupName.ToLower() == "china")
        //        {
        //            if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic)
        //            {
        //                mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic;
        //                foreach (FeatureLayer layer in mainMap.GetAllLayers())
        //                {
        //                    layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
        //                    layer.Reproject(mainMap.Projection);
        //                }
        //                tsbChangeProjection.Text = "change projection to GCS/NAD 83";
        //            }
        //            else
        //            {
        //                mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
        //                foreach (FeatureLayer layer in mainMap.GetAllLayers())
        //                {
        //                    layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic;
        //                    layer.Reproject(mainMap.Projection);
        //                }
        //                tsbChangeProjection.Text = "change projection to Albers";
        //            }


        //            mainMap.Projection.CopyProperties(mainMap.Projection);

        //            //mainMap.ViewExtents = mainMap.GetAllLayers()[0].Extent;
        //            _SavedExtent = mainMap.GetAllLayers()[0].Extent;
        //            mainMap.ViewExtents = _SavedExtent;
        //            return;
        //        }


        //        if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic)
        //        {
        //            mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
        //            foreach (FeatureLayer layer in mainMap.GetAllLayers())
        //            {
        //                layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
        //                layer.Reproject(mainMap.Projection);
        //            }
        //            tsbChangeProjection.Text = "change projection to GCS/NAD 83";
        //        }
        //        else
        //        {
        //            mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
        //            foreach (FeatureLayer layer in mainMap.GetAllLayers())
        //            {
        //                layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
        //                layer.Reproject(mainMap.Projection);
        //            }
        //            tsbChangeProjection.Text = "change projection to Albers";
        //        }

        //        mainMap.Projection.CopyProperties(mainMap.Projection);
        //        _SavedExtent = mainMap.GetAllLayers()[0].Extent;
        //        mainMap.ViewExtents = _SavedExtent;

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
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
                        AddLayer(_shapeFilePath);
                        //get columns, rows
                        if (ValidateColumnsRows(_shapeFilePath) != RowColFieldsValidationCode.BOTH_EXIST)
                        {
                            return;
                            //AddColumnsRows(_shapeFilePath);
                        }
                        GetColumnsRows(_shapeFilePath);
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
                //if (rasterFileLoc == null || rasterFileLoc.Trim().Length == 0)
                //{
                //    MessageBox.Show("Please enter a raster path before continuing");
                //    return;
                //}
                ////see if it is relative path, if so assume from base of data
                //if (!System.IO.Path.IsPathRooted(rasterFileLoc))
                //{
                //    String exeDir = (new FileInfo(CommonClass.DataFilePath)).Directory.ToString();
                //    rasterFileLoc = Path.Combine(exeDir, rasterFileLoc);
                //}

                //if (!File.Exists(rasterFileLoc))
                //{
                //    MessageBox.Show("No raster file found at " + rasterFileLoc);
                //    return;
                //}

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
                if (IsEditor)
                {

                    commandText = string.Format("select Ttype from GridDefinitions where GridDefinitionID=" + _gridID + "");
                    int type = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                    switch (_gridType)
                    {
                        case 1:
                            if (_shapeFilePath == string.Empty)
                            {
                                commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}'WHERE GridDefinitionID={1}", txtGridID.Text, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                this.DialogResult = DialogResult.OK; return;
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

                else
                {
                    //ensure shapefile is correctly formatted.
                    if (ValidateColumnsRows(_shapeFilePath) != RowColFieldsValidationCode.BOTH_EXIST)
                    {
                        return;
                    }


                    commandText = "select GridDefinitionID from GridDefinitions where GridDefinitionName='" + txtGridID.Text + "' and SetupID=" + CommonClass.ManageSetup.SetupID;
                    object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    if (obj != null)
                    {
                        MessageBox.Show("This grid definition name is already in use. Please enter a different name.");
                        return;
                    }
                    // 2015 09 23 - BENMAP -345 modified next line to try and fix metadata load
                    _metadataObj.DatasetId = SQLStatementsCommonClass.selectMaxID("GRIDDEFINITIONID", "GRIDDEFINITIONS"); 
                    //_gridID =  _metadataObj.DatasetId;
                    commandText = string.Format("select max(GRIDDEFINITIONID) from GRIDDEFINITIONS");
                    _gridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                    //_metadataObj.DatasetId = _gridID;
                    string _filePath = string.Empty;
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
                            }
                            finally
                            {
                                fs.Close();
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
                        saveMetadata(_filePath, _gridID, _gridType);
                        return;
                    }
                }


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
                    progressBar1.Maximum = ds.Tables[0].Rows.Count*2;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //AP-for testing
                        //if (counter == 0)
                        //{
                            //counter++;
                            int gridDefinitionID = Convert.ToInt32(dr["GridDefinitionID"]);
                        //don't run against myself
                            if (gridDefinitionID == addBenMAPGrid.GridDefinitionID)
                            {
                                continue;
                            }


                            

                            int bigGridID, smallGridID;

                            //AP-for testing-remove
                            //if (gridDefinitionID != 20)
                            //{
                            //    continue;
                            //}


                            bigGridID = gridDefinitionID;
                            smallGridID = addBenMAPGrid.GridDefinitionID;
                            //AP-launches here
                            //AsyncgetRelationshipFromBenMAPGridPercentage dlgt = new AsyncgetRelationshipFromBenMAPGridPercentage(getRelationshipFromBenMAPGridPercentage);
                            //lstAsyns.Add(bigGridID + "," + smallGridID);
                            //lstAsyns.Add(smallGridID + "," + bigGridID);
                            //iAsyns++; iAsyns++;
                            //IAsyncResult ar = dlgt.BeginInvoke(bigGridID, smallGridID, rasterFileLoc, new AsyncCallback(outPut), dlgt);
                            //IAsyncResult ar2 = dlgt.BeginInvoke(smallGridID, bigGridID, rasterFileLoc, new AsyncCallback(outPut), dlgt);
                            Console.WriteLine("Starting grid " + bigGridID + " against " + smallGridID);
                            getRelationshipFromBenMAPGridPercentage(bigGridID, smallGridID, rasterFileLoc);
                            counter++;
                            progressBar1.Value = counter;
                            Application.DoEvents();
                            Console.WriteLine("Starting grid " + smallGridID + " against " + bigGridID);
                            getRelationshipFromBenMAPGridPercentage(smallGridID, bigGridID, rasterFileLoc);
                            counter++;
                            progressBar1.Value = counter;
                            Application.DoEvents();
                        }
                        //progressBar1.Step = 1;
                        //progressBar1.Minimum = 1;
                        //progressBar1.Maximum = iAsyns + 1;
                        //this.Enabled = false;

                    this.Enabled = true;
                    this.Close();
                    //}
                }

                catch (Exception ex)
                {

                    Logger.LogError(ex);
                }

                

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }

        private void saveMetadata(string filePath, int gridID, int gridType)
        {

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
                        Feature f = new Feature();
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



                        DotSpatial.Topology.Polygon p = new DotSpatial.Topology.Polygon(lstCoordinate.ToArray());
                        f.BasicGeometry = p;
                        fs.AddFeature(f);
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

        private void picCRHelp_Click(object sender, EventArgs e)
        {
            this.toolTip1.Show("To calculate health impacts and economic benefits, BenMAP" +
                "\r\nutilizes air quality, population and demographic data at" +
                "\r\ndifferent spatial scales. To do this, the program calculates a" +
                "\r\n'percentage file' that relates data at one spatial scale to" +
                "\r\nanother (e.g. 12km CMAQ grid to county).This step is" +
                "\r\nperformed only once per crosswalk and the results are saved" +
                "\r\nto the database for subsequent calculations. If you do not" +
                "\r\ncreate the crosswalks now, BenMAP will create crosswalks as" +
                "\r\nneeded during the configuration or aggregation, pooling and" +
                "\r\nvaluation stages.", picCRHelp, 32700);
        }

        private void picCRHelp_MouseHover(object sender, EventArgs e)
        {
            this.toolTip1.Show("To calculate health impacts and economic benefits, BenMAP" +
                "\r\nutilizes air quality, population and demographic data at" +
                "\r\ndifferent spatial scales. To do this, the program calculates a" +
                "\r\n'percentage file' that relates data at one spatial scale to" +
                "\r\nanother (e.g. 12km CMAQ grid to county).This step is" +
                "\r\nperformed only once per crosswalk and the results are saved" +
                "\r\nto the database for subsequent calculations. If you do not" +
                "\r\ncreate the crosswalks now, BenMAP will create crosswalks as" +
                "\r\nneeded during the configuration or aggregation, pooling and" +
                "\r\nvaluation stages.", picCRHelp, 32700);
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

    }
}
