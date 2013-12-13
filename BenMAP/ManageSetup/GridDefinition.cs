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


namespace BenMAP
{
    public partial class GridDefinition : FormBase
    {
        public GridDefinition()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 是否为用户编辑状态:true->是点击ManageGridDefinetions中的Edit,否则是新增
        /// </summary>
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
        // public string _saveGirdIDName = "";
        public string _gridIDName = "";
        public string _shapeFileName = "";
        public string _shapeFilePath = "";
        //private string _loadPath = string.Empty;
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
                        //string strPath = Application.StartupPath + @"\Data\Shapefiles\" + ds.Tables[0].Rows[0]["shapeFileName"].ToString() + ".shp";
                        //mainMap.ProjectionModeReproject = ActionMode.Never;//不提示坐标系
                        //mainMap.ProjectionModeDefine = ActionMode.Never;
                        //mainMap.Layers.Clear();
                        //mainMap.Layers.Add(strPath);
                        //AddLayerAndGetAtt(strPath);
                        cboGridType.SelectedIndex = 1;
                        // 填入初始的数值
                        txtMinimumLatitude.Text = ds.Tables[0].Rows[0]["MinimumLatitude"].ToString();
                        txtMinimumLongitude.Text = ds.Tables[0].Rows[0]["MinimumLongitude"].ToString();
                        nudColumnsPerLongitude.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["ColumnsPerLongitude"]);
                        nudRowsPerLatitude.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["RowsPerLatitude"]);
                        //lblShapeFileName.Text = obj.ToString();
                        commandText = "select Columns,Rrows from GridDefinitions where GridDefinitionID=" + _gridID + "";
                        ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                        nudColumns.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["Columns"]);
                        nudRows.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["Rrows"]);
                    }
                    else
                    {
                        cboGridType.SelectedIndex = 0;
                        //string strPath = Application.StartupPath + @"\Data\Shapefiles\" + obj.ToString() + ".shp";
                        //AddLayerAndGetAtt(strPath);
                        lblShapeFileName.Text = obj.ToString();
                        //lblCol.Text = _shapeCol.ToString();
                        //lblRow.Text = _shapeRow.ToString();
                    }
                }
                else
                {
                    IsEditor = false;
                    //automatically generated name-increase the number at the end of the name
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

        private void AddLayerAndGetAtt(string strPath)
        {
            try
            {
                mainMap.ProjectionModeReproject = ActionMode.Never;//不提示坐标系
                mainMap.ProjectionModeDefine = ActionMode.Never;
                mainMap.Layers.Clear();
                mainMap.Layers.Add(strPath);
                IFeatureSet fs = FeatureSet.Open(strPath);
                List<int> lsCol = new List<int>();
                List<int> lsRow = new List<int>();
                int icol = -1;
                int irow = -1;
                bool colname = false;
                bool rowname = false;
                for (int i = 0; i < fs.DataTable.Columns.Count; i++)
                {
                    if (fs.DataTable.Columns[i].ToString() == "ROW")
                    {
                        irow = i;
                        rowname = true;
                    }
                    else if (fs.DataTable.Columns[i].ToString().ToLower() == "row")
                    {
                        irow = i;
                    }

                    if (fs.DataTable.Columns[i].ToString() == "COL")
                    {
                        icol = i;
                        colname = true;
                    }
                    else if (fs.DataTable.Columns[i].ToString().ToLower() == "col" || fs.DataTable.Columns[i].ToString().ToLower() == "column")
                    {
                        icol = i;
                    }
                }
                
                if (icol>=0 && irow>=0)
                {
                    foreach (DataRow dr in fs.DataTable.Rows)
                    {
                        lsCol.Add(Convert.ToInt32(dr[icol]));
                        lsRow.Add(Convert.ToInt32(dr[irow]));
                    }
                    _shapeCol = lsCol.Max();
                    _shapeRow = lsRow.Max();
                }
                else
                {
                    txtShapefile.Text = "";
                    MessageBox.Show("This shapefile does not have the required column and row variables.");
                }

                if (colname == false || rowname == false)
                {
                    fs.DataTable.Columns[icol].ColumnName = "COL";
                    fs.DataTable.Columns[irow].ColumnName = "ROW";
                    fs.SaveAs(strPath, true);
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
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
                // Open，获取文件所在的路径
                //OpenFileDialog openFileDialog = new OpenFileDialog();
                //if (!openFileDialog.RestoreDirectory)
                //{ openFileDialog.InitialDirectory = Application.StartupPath + @"\Data\Shapefiles"; }
                openFileDialog.Filter = "Shapefiles|*.shp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                _filePath = openFileDialog.FileName;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtShapefile.Text = openFileDialog.FileName;//将路径填充到txt文本框中
                    lblShapeFileName.Text = System.IO.Path.GetFileNameWithoutExtension(txtShapefile.Text);
                    //if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + lblShapeFileName.Text + ".shp"))
                    //{
                    //    MessageBox.Show("This file already exists! ");
                    //    txtShapefile.Text = "";
                    //    return;
                    //}
                    //else
                    //{
                    _shapeFilePath = openFileDialog.FileName;
                    //  string filePath = txtShapefile.Text;
                    AddLayerAndGetAtt(_shapeFilePath);
                    //}
                    lblCol.Text = _shapeCol.ToString();
                    lblRow.Text = _shapeRow.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }


        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboGridType.SelectedIndex == 0)
                {
                    mainMap.ProjectionModeReproject = ActionMode.Never;//不提示坐标系
                    mainMap.ProjectionModeDefine = ActionMode.Never;
                    mainMap.Layers.Clear();
                    if (txtShapefile.Text != "")
                    {
                        mainMap.Layers.Add(txtShapefile.Text.ToString());
                    }
                    else
                    {
                        string strPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + lblShapeFileName.Text + ".shp";
                        AddLayerAndGetAtt(strPath);
                        lblCol.Text = _shapeCol.ToString();
                        lblRow.Text = _shapeRow.ToString();
                    }
                }
                else
                {
                    //regular grid 怎么画
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
                    //-------------------必须校验-------------------------
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
            //_colPerLongitude = int.Parse(nudColumnsPerLongitude.Value.ToString());
            //_rowPerLatitude = int.Parse(nudRowsPerLatitude.Value.ToString());
            //_minLongitude = System.Convert.ToDouble(txtMinimumLongitude.Text);
            //_minLatitude = System.Convert.ToDouble(txtMinimumLatitude.Text);
            try
            {
                if (cboGridType.SelectedIndex == 0)
                {
                    //_saveGirdIDName = txtGridID.Text;
                    _shapeFileName = lblShapeFileName.Text;
                }
                else
                {
                    //_saveGirdIDName = txtGridID.Text;
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
                    //if(txtGridID.Text==_gridIDName&&

                    //commandText = "select GridDefinitionID from GridDefinitions where GridDefinitionName='" + txtGridID.Text + "'";
                    //object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    //if (obj != null)
                    //{
                    //    MessageBox.Show("The Grid Name has existed in DataBase.Please rename it .");
                    //    return;
                    //}
                    commandText = string.Format("select Ttype from GridDefinitions where GridDefinitionID=" + _gridID + "");
                    int type = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                    switch (_gridType)
                    {
                        case 1://shapefile
                            if (_shapeFilePath == string.Empty)
                            {
                                commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}'WHERE GridDefinitionID={1}", txtGridID.Text, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                this.DialogResult = DialogResult.OK; return;
                                //this.Close();//MessageBox.Show("Please select a shape file."); return;
                            }
                            if (type == 1)//shapefile-shapefile
                            {
                                //删除原先的文件
                                //commandText = "select ShapeFileName from ShapefileGriddefinitiondetails where GridDefinitionID=" + _gridID + "";
                                //object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                //FileInfo file = new FileInfo(Application.StartupPath + @"\Data\Shapefiles\" + obj.ToString() + ".shp");
                                //if (file.Exists) { file.Delete(); }
                                //add to shapefilegriddefinitondetails

                                commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GridDefinitionID={4}", txtGridID.Text, _shapeCol, _shapeRow, _gridType, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                //判断是否已存在，询问用户是重命名还是覆盖
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
                                        if (dr == DialogResult.OK)//rename
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
                            else//regular-shapefile
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
                                            if (dr == DialogResult.OK)//rename
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
                                            commandText = string.Format("insert into ShapeFileGridDefinitionDetails (GridDefinitionID,ShapeFileName) values ({0},'{1}')", _gridID, _shapeFileName);
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                        }
                                    }
                                    commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GridDefinitionID={4}", txtGridID.Text, _shapeCol, _shapeRow, _gridType, _gridID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    commandText = "delete From RegularGridDefinitionDetails where GridDefinitionID=" + _gridID + "";
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    //删除regular Grid的数据和文件
                                    //commandText = "select ShapeFileName from RegularGridDefinitionDetails where GridDefinitionID=" + _gridID + "";
                                    //object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                    //FileInfo file = new FileInfo(Application.StartupPath + @"\Data\Shapefiles\" + obj.ToString() + ".shp");
                                    //if (file.Exists) { file.Delete(); };
                                }
                                else { return; }
                            }
                            //move shapefile

                            IFeatureSet fs = FeatureSet.Open(_shapeFilePath);
                            try
                            {
                                fs.SaveAs(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp", true);
                            }
                            finally
                            {
                                fs.Close();
                            }
                            break;

                        case 0://regular
                            if (Math.Abs(_minLongitude) > 180 || Math.Abs(_minLatitude) > 90) { MessageBox.Show("Longitude must be less than 180 degrees and latitude less than 90 degrees. Please check the longitude and latitude values."); return; }
                            if (type == 0)//regular-regular
                            {
                                commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GridDefinitionID={4}", txtGridID.Text, _columns, _rrows, _gridType, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                //FileInfo files = new FileInfo(Application.StartupPath + @"\Data\Shapefiles\RegularGrid" + _gridID + ".shp");
                                //if (files.Exists) { files.Delete(); }
                                //add to regulargriddefinitiondetail                     
                                commandText = string.Format("Update RegularGriddefinitionDetails set minimumlatitude={0},minimumlongitude={1},columnsperlongitude={2},rowsperlatitude={3} where GridDefinitionID={4}", _minLatitude, _minLongitude, _colPerLongitude, _rowPerLatitude, _gridID);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                            }
                            else//shapefile-regular
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
                                    //删除shapefile的数据和文件
                                    //commandText = "select ShapeFileName from ShapefileGriddefinitiondetails where GridDefinitionID=" + _gridID + "";
                                    //object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                    //FileInfo file = new FileInfo(Application.StartupPath + @"\Data\Shapefiles\" + obj.ToString() + ".shp");
                                    //if (file.Exists) { file.Delete(); };
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
                        //如果是edit,然后又选择不计算percentage,就删掉之前相关的percentage
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
                    //if (txtShapefile.Text == string.Empty&&)
                    commandText = "select GridDefinitionID from GridDefinitions where GridDefinitionName='" + txtGridID.Text + "' and SetupID=" + CommonClass.ManageSetup.SetupID;
                    object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    if (obj != null)
                    {
                        MessageBox.Show("This grid definition name is already in use. Please enter a different name.");
                        return;
                    }
                    commandText = string.Format("select max(GRIDDEFINITIONID) from GRIDDEFINITIONS");
                    _gridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;

                    switch (_gridType)
                    {
                        case 1://shapefile
                            //判断是否已存在，已存在则重命名
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
                                //DialogResult dr = MessageBox.Show("The Shapefile already exists. Rename the file?", "Tip", MessageBoxButtons.OKCancel);
                                //if (dr == DialogResult.Cancel)
                                //{
                                //    return;
                                //}
                                //else
                                //{
                                //    string newFileName = "";
                                //    int i = 1;
                                //    do
                                //    {
                                //        newFileName = _shapeFileName + i.ToString();
                                //        i++;
                                //    }
                                //    while (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + newFileName + ".shp"));
                                //    _shapeFileName = newFileName;
                                //}
                            }

                            commandText = string.Format("INSERT INTO GRIDDEFINITIONS (GridDefinitionID,SetUpID,GridDefinitionName,Columns,Rrows,Ttype,DefaultType) VALUES(" + _gridID + ",{0},'{1}',{2},{3},{4},{5})", CommonClass.ManageSetup.SetupID, txtGridID.Text, _shapeCol, _shapeRow, _gridType, 0);
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            //add to shapefilegriddefinitondetails
                            commandText = string.Format("INSERT INTO SHAPEFILEGRIDDEFINITIONDETAILS (GridDefinitionID,ShapeFileName) VALUES(" + _gridID + ",'{0}')", _shapeFileName);
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            //move shapefile
                            CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp");
                            IFeatureSet fs = FeatureSet.Open(_shapeFilePath);
                            try
                            {
                                fs.SaveAs(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + _shapeFileName + ".shp", true);
                            }
                            finally
                            {
                                fs.Close();
                            }
                            break;
                        case 0://regular
                            DialogResult rtn = MessageBox.Show("Save this grid?", "", MessageBoxButtons.YesNo);
                            if (rtn == DialogResult.No) { this.DialogResult = DialogResult.Cancel; return; }
                            else
                            {
                                commandText = string.Format("INSERT INTO GRIDDEFINITIONS (GridDefinitionID,SetUpID,GridDefinitionName,Columns,Rrows,Ttype,DefaultType) VALUES(" + _gridID + ",{0},'{1}',{2},{3},{4},{5})", CommonClass.ManageSetup.SetupID, txtGridID.Text, _columns, _rrows, _gridType, 0);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                //add to regulargriddefinitiondetail
                                if (Math.Abs(_minLongitude) > 180 || Math.Abs(_minLatitude) > 90) { MessageBox.Show("Please input valid longitude and latitude values."); return; }
                                commandText = string.Format("INSERT INTO RegularGridDefinitionDetails (GridDefinitionID,MinimumLatitude,MinimumLongitude,ColumnsPerLongitude,RowsPerLatitude,ShapeFileName)  VALUES ({0},{1},{2},{3},{4},'{5}')", _gridID, txtMinimumLatitude.Text, txtMinimumLongitude.Text, nudColumnsPerLongitude.Value, nudRowsPerLatitude.Value, txtGridID.Text);
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                fs = getFeatureSetFromRegularGrid(_columns, _rrows, _minLatitude, _minLongitude, _colPerLongitude, _rowPerLatitude);
                                CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + txtGridID.Text + ".shp");

                                try
                                {
                                    fs.SaveAs(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + txtGridID.Text + ".shp", true);
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
                        return;
                    }
                }
                    //-----------------20111121 add relationship------------
                    //string tip = "Please Wait...";
                    //WaitShow(tip);
                    lblprogress.Visible = true;
                    lblprogress.Refresh();
                    progressBar1.Visible = true;
                    progressBar1.Refresh();
                    BenMAPGrid addBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(_gridID);
                    //---------------修改成多线程的方式----------------------------------
                    try
                    {

                          commandText = "select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,ShapeFileGridDefinitionDetails b " +
                                             " where a.GridDefinitionID=b.GridDefinitionID and a.TType=1  and a.SetupID=" + addBenMAPGrid.SetupID +
                                             " union " +
                                             " select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,RegularGridDefinitionDetails b " +
                                             " where a.GridDefinitionID=b.GridDefinitionID and a.TType=0  and a.SetupID=" + addBenMAPGrid.SetupID;
                         fb = new ESIL.DBUtility.ESILFireBirdHelper();
                        System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        // List<GridRelationship> lstGridRelationship = new List<GridRelationship>();
                        //------------remove from griddefinitionpercentage-------------
                        commandText = string.Format("delete from GridDefinitionPercentageEntries where PercentageID in ( select PercentageID  from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0})", addBenMAPGrid.GridDefinitionID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0}", addBenMAPGrid.GridDefinitionID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        string AppPath = Application.StartupPath;
                        //----------------------------------------xjp-------------仅用于测试！
                        //AppPath = Application.StartupPath + @"\Data\Shapefiles\";// @"C:\Program Files\BenMAP 4.0\Shapefiles\";
                        //if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                        //   {
                        if (ds.Tables[0].Rows.Count == 1) this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            //增加网格和当前网格的对应关系
                            int gridDefinitionID = Convert.ToInt32(dr["GridDefinitionID"]);
                            if (gridDefinitionID == addBenMAPGrid.GridDefinitionID)
                            {
                                continue;
                            }




                            int bigGridID, smallGridID;



                            //DotSpatial.Data.IFeatureSet relationFeatureSet = DotSpatial.Data.FeatureSet.Open(AppPath + dr["ShapeFileName"].ToString() + ".shp");
                            //DotSpatial.Data.IFeatureSet regionFeatureSet = null;
                            //DotSpatial.Data.IFeatureSet gridFeatureSet = null;
                          
                            //-------------开始多线程-------------------
                            bigGridID = gridDefinitionID;
                            smallGridID = addBenMAPGrid.GridDefinitionID;
                            AsyncgetRelationshipFromBenMAPGridPercentage dlgt = new AsyncgetRelationshipFromBenMAPGridPercentage(getRelationshipFromBenMAPGridPercentage);
                            //CommonClass.getRelationshipFromBenMAPGridPercentage(bigGridID, smallGridID);
                            lstAsyns.Add(bigGridID + "," + smallGridID);
                            lstAsyns.Add(smallGridID + "," + bigGridID);
                            iAsyns++; iAsyns++;
                            IAsyncResult ar = dlgt.BeginInvoke(bigGridID,smallGridID, new AsyncCallback(outPut), dlgt);


                            //AsyncgetRelationshipFromBenMAPGridPercentage dlgt2 = new AsyncgetRelationshipFromBenMAPGridPercentage(getRelationshipFromBenMAPGridPercentage);
                            IAsyncResult ar2 = dlgt.BeginInvoke(smallGridID, bigGridID, new AsyncCallback(outPut), dlgt);
                            //------------------------------------------

                        }
                        progressBar1.Step = 1;
                        progressBar1.Minimum = 1;
                        progressBar1.Maximum = iAsyns+1;
                        this.Enabled = false;
                        //return lstGridRelationship;
                    }

                    catch (Exception ex)
                    {

                        Logger.LogError(ex);
                        //return null;
                    }
                   // Grid.GridCommon.getGridRelationshipFromGridPercentage(addBenMAPGrid);
                    //List<GridRelationship> lstGridRelationship = CommonClass.LstGridRelationshipAll;
                    //Grid.GridCommon.getGridRelationshipFromGrid(addBenMAPGrid, ref lstGridRelationship);
                    //string filePath = string.Format(@"{0}\Data\GridRelationship\GridRelationship.gr", Application.StartupPath);
                    //Grid.GridCommon.createGridRelationshipFile(filePath, CommonClass.LstGridRelationshipAll);
                   // WaitClose();
                

                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }
        private int iAsyns = 0;
        public List<string> lstAsyns = new List<string>();
        Dictionary<string, List<GridRelationshipAttributePercentage>> dicAllGridPercentage =new Dictionary<string,List<GridRelationshipAttributePercentage>>();
        private void outPut(IAsyncResult ar)
        {
            try
            {
                //CRSelectFunctionCalculateValue crv = (ar.AsyncState as AsyncDelegateCalculateOneCRSelectFunction).EndInvoke(ar);
                //if (ar != null)
                //    lstCRSelectFunctionCalculateValue.Add(crv);
                //lstAsyns.Remove(crv.CRSelectFunction.BenMAPHealthImpactFunction.ID.ToString());
                lstAsyns.RemoveAt(0);
                progressBar1.Value++;
                Dictionary<string, List<GridRelationshipAttributePercentage>> dic = (ar.AsyncState as AsyncgetRelationshipFromBenMAPGridPercentage).EndInvoke(ar);
                if (dic != null && dic.Count > 0)
                    dicAllGridPercentage.Add(dic.ToArray()[0].Key, dic.ToArray()[0].Value);
                //lock (waitMess)
                //{
                //    waitMess.Msg = "Finish process cells for study " + icount + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count + "...";
                //}
                GC.Collect();
                if (lstAsyns.Count == 0)
                {
                    //------------------填入数据库-------------------------------
                    foreach (KeyValuePair<string, List<GridRelationshipAttributePercentage>> k in dicAllGridPercentage)
                    {
                        Configuration.ConfigurationCommonClass.updatePercentageToDatabase(k);
                        /*
                        string commandText = "select max(PercentageID) from GridDefinitionPercentages";
                        //-----first get the max of id in griddefinitonpercentages
                        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                        int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

                        //-----insert into griddefinitonpercentages
                        commandText = string.Format("insert into GridDefinitionPercentages values({0},{1})", iMax, k.Key);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        //-----insert into GridDefinitionPercentageEntries
                        int i = 1;
                        commandText = "execute block as declare incidenceRateID int;" + " BEGIN ";
                        FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                        fbCommand.Connection = CommonClass.Connection;
                        fbCommand.CommandType = CommandType.Text;
                        if (fbCommand.Connection.State != ConnectionState.Open)
                        { fbCommand.Connection.Open(); }
                        int j=0;
                        foreach (GridRelationshipAttributePercentage grp in k.Value)
                        {
                            //----------------批量提交-------------------------

                            if (i < 250 && j<k.Value.Count -1)
                            {
                                commandText = commandText + string.Format(" insert into GridDefinitionPercentageEntries values({0},{1},{2},{3},{4},{5},{6});",
                        iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow, grp.percentage, 0);

                               
                            }
                            else
                            {
                                commandText = commandText + string.Format(" insert into GridDefinitionPercentageEntries values({0},{1},{2},{3},{4},{5},{6});",
                                iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow, grp.percentage, 0);
                                
                                commandText = commandText + "END";
                                fbCommand.CommandText = commandText;
                                fbCommand.ExecuteNonQuery();
                                commandText = "execute block as declare incidenceRateID int;" + " BEGIN ";
                                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                                
                                i = 1;

                            }
                            i++;
                            j++;
                            //---------------End 批量提交----------------------

                            //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                         * */
                    }
                    
                    this.DialogResult = DialogResult.OK;
                }

            }
            catch (Exception ex)
            {
                
            }
        }
        public delegate Dictionary<string, List<GridRelationshipAttributePercentage>> AsyncgetRelationshipFromBenMAPGridPercentage(int big, int small);
        public Dictionary<string,List<GridRelationshipAttributePercentage>> getRelationshipFromBenMAPGridPercentage(int big, int small)
        {
            try
            {

                IFeatureSet fsBig = new FeatureSet();
                IFeatureSet fsSmall = new FeatureSet();
                BenMAPGrid bigBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(big==20?18:big);
                BenMAPGrid smallBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(small==20?18:small);
                if (bigBenMAPGrid == null)
                    bigBenMAPGrid = new ShapefileGrid()
                    {
                        ShapefileName = "County_epa2",
                    };
                if(smallBenMAPGrid==null)
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
                    fsBig = DotSpatial.Data.FeatureSet.Open(shapeFileName);
                    string shapeFileNameSmall = CommonClass.DataFilePath + @"\Data\Shapefiles\" + setupname + "\\" + smallShapefileName + ".shp";
                    fsSmall = DotSpatial.Data.FeatureSet.Open(shapeFileNameSmall);
                    //----------Clip first! ----------
                    //IFeatureSet fsSmallClip = CommonClass.Intersection(fsSmall,fsBig, FieldJoinType.LocalOnly);
                    //List<IFeature> lstClip = fsSmall.Select(fsBig.Extent);
                    //IFeatureSet fsSmallClip = new FeatureSet(lstClip);
                    //List<int> lstRemove = new List<int>();
                    //for (int i = 0; i < fsSmall.Features.Count; i++)
                    //{
                    //    if (!lstClip.Contains(i))
                    //        lstRemove.Add(i);

                    //}
                    //List<IFeature> lstRemoveFeature = new List<IFeature>();
                    //foreach (int i in lstRemove)
                    //{
                    //    lstRemoveFeature.Add(fsSmall.Features[i]);

                    //}
                    //foreach (IFeature f in lstRemoveFeature)
                    //{
                    //    fsSmall.Features.Remove(f);
                    //}
                    List<GridRelationshipAttributePercentage> lstGR = null;// CommonClass.IntersectionPercentage(fsBig, fsSmall, FieldJoinType.All);
                    if (big == 20 || small == 20)
                    {
                        
                        lstGR = CommonClass.IntersectionPercentageNation(fsBig, fsSmall, FieldJoinType.All,big,small);
                    }
                    else
                    {
                        lstGR = CommonClass.IntersectionPercentage(fsBig, fsSmall, FieldJoinType.All);
                    }
                    Dictionary<string, List<GridRelationshipAttributePercentage>> dic = new Dictionary<string, List<GridRelationshipAttributePercentage>>();
                    dic.Add(small + "," + big, lstGR);
                    return dic;
                    //---------------------------填入数据库！--------------------------------
                    string commandText = "select max(PercentageID) from GridDefinitionPercentages";
                    //-----first get the max of id in griddefinitonpercentages
                    int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

                    //-----insert into griddefinitonpercentages
                    commandText = string.Format("insert into GridDefinitionPercentages values({0},{1},{2})", iMax, small, big);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    //-----insert into GridDefinitionPercentageEntries
                    foreach (GridRelationshipAttributePercentage grp in lstGR)
                    {
                        commandText = string.Format("insert into GridDefinitionPercentageEntries values({0},{1},{2},{3},{4},{5},{6})",
                            iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow, grp.percentage, 0);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
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
                        //f.DataRow = new DataRow();
                        //f.DataRow["Col"] = i;
                        //f.DataRow["Row"] = j;
                        fs.AddFeature(f);
                        fs.DataTable.Rows[i * Rows + j]["Col"] = i;
                        fs.DataTable.Rows[i * Rows + j]["Row"] = j;


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

        TipFormGIF waitMess = new TipFormGIF();//等待窗体
        bool sFlog = true;
        //--显示等待窗体 
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

        //--新开辟一个线程调用 
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

        //--关闭等待窗体 
        public void WaitClose()
        {
            //同步到主线程上
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
            this.toolTip1.Show("To calculate health impacts and economic benefits, BenMAP"+
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

    }
}
