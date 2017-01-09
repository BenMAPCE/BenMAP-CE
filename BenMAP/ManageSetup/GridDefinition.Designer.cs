namespace BenMAP
{
    partial class GridDefinition
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpPictureView = new System.Windows.Forms.GroupBox();
            this.mainMap = new DotSpatial.Controls.Map();
            this.grpGridDefinition = new System.Windows.Forms.GroupBox();
            this.chkBoxUseAsGeographicArea = new System.Windows.Forms.CheckBox();
            this.btn_browsePopRaster = new System.Windows.Forms.Button();
            this.txtb_popGridLoc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnViewMetadata = new System.Windows.Forms.Button();
            this.picCRHelp = new System.Windows.Forms.PictureBox();
            this.chkBoxCreatePercentage = new System.Windows.Forms.CheckBox();
            this.cboGridType = new System.Windows.Forms.ComboBox();
            this.lblGridType = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.tabGrid = new System.Windows.Forms.TabControl();
            this.tpShapefileGrid = new System.Windows.Forms.TabPage();
            this.lblRow = new System.Windows.Forms.Label();
            this.lblCol = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblShapeFileName = new System.Windows.Forms.Label();
            this.lblShapeFile = new System.Windows.Forms.Label();
            this.btnShapefile = new System.Windows.Forms.Button();
            this.txtShapefile = new System.Windows.Forms.TextBox();
            this.lblLoadShapefile = new System.Windows.Forms.Label();
            this.tpgRegularGrid = new System.Windows.Forms.TabPage();
            this.nudRowsPerLatitude = new System.Windows.Forms.NumericUpDown();
            this.nudColumnsPerLongitude = new System.Windows.Forms.NumericUpDown();
            this.txtMinimumLatitude = new System.Windows.Forms.TextBox();
            this.txtMinimumLongitude = new System.Windows.Forms.TextBox();
            this.nudRows = new System.Windows.Forms.NumericUpDown();
            this.nudColumns = new System.Windows.Forms.NumericUpDown();
            this.lblRowsPerLatitude = new System.Windows.Forms.Label();
            this.lblColumsPerLongitude = new System.Windows.Forms.Label();
            this.lblMinimumLatitude = new System.Windows.Forms.Label();
            this.lblMinimumLongitude = new System.Windows.Forms.Label();
            this.lblRows = new System.Windows.Forms.Label();
            this.lblColumns = new System.Windows.Forms.Label();
            this.txtGridID = new System.Windows.Forms.TextBox();
            this.lblGridID = new System.Windows.Forms.Label();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.lblprogress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpPictureView.SuspendLayout();
            this.grpGridDefinition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCRHelp)).BeginInit();
            this.tabGrid.SuspendLayout();
            this.tpShapefileGrid.SuspendLayout();
            this.tpgRegularGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRowsPerLatitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumnsPerLongitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumns)).BeginInit();
            this.grpCancelOK.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPictureView
            // 
            this.grpPictureView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPictureView.Controls.Add(this.mainMap);
            this.grpPictureView.Location = new System.Drawing.Point(363, 14);
            this.grpPictureView.Name = "grpPictureView";
            this.grpPictureView.Size = new System.Drawing.Size(426, 481);
            this.grpPictureView.TabIndex = 0;
            this.grpPictureView.TabStop = false;
            // 
            // mainMap
            // 
            this.mainMap.AllowDrop = true;
            this.mainMap.AutoScroll = true;
            this.mainMap.BackColor = System.Drawing.Color.White;
            this.mainMap.CollectAfterDraw = false;
            this.mainMap.CollisionDetection = false;
            this.mainMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMap.ExtendBuffer = false;
            this.mainMap.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.mainMap.IsBusy = false;
            this.mainMap.IsZoomedToMaxExtent = false;
            this.mainMap.Legend = null;
            this.mainMap.Location = new System.Drawing.Point(3, 18);
            this.mainMap.Name = "mainMap";
            this.mainMap.ProgressHandler = null;
            this.mainMap.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.RedrawLayersWhileResizing = false;
            this.mainMap.SelectionEnabled = true;
            this.mainMap.Size = new System.Drawing.Size(420, 460);
            this.mainMap.TabIndex = 8;
            this.mainMap.ZoomOutFartherThanMaxExtent = false;
            // 
            // grpGridDefinition
            // 
            this.grpGridDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpGridDefinition.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpGridDefinition.Controls.Add(this.chkBoxUseAsGeographicArea);
            this.grpGridDefinition.Controls.Add(this.btn_browsePopRaster);
            this.grpGridDefinition.Controls.Add(this.txtb_popGridLoc);
            this.grpGridDefinition.Controls.Add(this.label4);
            this.grpGridDefinition.Controls.Add(this.btnViewMetadata);
            this.grpGridDefinition.Controls.Add(this.picCRHelp);
            this.grpGridDefinition.Controls.Add(this.chkBoxCreatePercentage);
            this.grpGridDefinition.Controls.Add(this.cboGridType);
            this.grpGridDefinition.Controls.Add(this.lblGridType);
            this.grpGridDefinition.Controls.Add(this.btnPreview);
            this.grpGridDefinition.Controls.Add(this.tabGrid);
            this.grpGridDefinition.Controls.Add(this.txtGridID);
            this.grpGridDefinition.Controls.Add(this.lblGridID);
            this.grpGridDefinition.Location = new System.Drawing.Point(12, 14);
            this.grpGridDefinition.Name = "grpGridDefinition";
            this.grpGridDefinition.Size = new System.Drawing.Size(345, 481);
            this.grpGridDefinition.TabIndex = 0;
            this.grpGridDefinition.TabStop = false;
            this.grpGridDefinition.Text = "Grid Definition";
            // 
            // chkBoxUseAsGeographicArea
            // 
            this.chkBoxUseAsGeographicArea.AutoSize = true;
            this.chkBoxUseAsGeographicArea.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkBoxUseAsGeographicArea.Location = new System.Drawing.Point(9, 443);
            this.chkBoxUseAsGeographicArea.Name = "chkBoxUseAsGeographicArea";
            this.chkBoxUseAsGeographicArea.Size = new System.Drawing.Size(295, 32);
            this.chkBoxUseAsGeographicArea.TabIndex = 12;
            this.chkBoxUseAsGeographicArea.Text = "Use this grid definition as a geographic area that \r\ncan be assigned to health im" +
    "pact functions";
            this.chkBoxUseAsGeographicArea.UseVisualStyleBackColor = true;
            this.chkBoxUseAsGeographicArea.CheckedChanged += new System.EventHandler(this.chkBoxCreatePercentage_CheckedChanged);
            // 
            // btn_browsePopRaster
            // 
            this.btn_browsePopRaster.Location = new System.Drawing.Point(266, 114);
            this.btn_browsePopRaster.Name = "btn_browsePopRaster";
            this.btn_browsePopRaster.Size = new System.Drawing.Size(73, 27);
            this.btn_browsePopRaster.TabIndex = 11;
            this.btn_browsePopRaster.Text = "Browse . .";
            this.btn_browsePopRaster.UseVisualStyleBackColor = true;
            this.btn_browsePopRaster.Visible = false;
            this.btn_browsePopRaster.Click += new System.EventHandler(this.btn_browsePopRaster_Click);
            // 
            // txtb_popGridLoc
            // 
            this.txtb_popGridLoc.Location = new System.Drawing.Point(24, 114);
            this.txtb_popGridLoc.Name = "txtb_popGridLoc";
            this.txtb_popGridLoc.Size = new System.Drawing.Size(236, 22);
            this.txtb_popGridLoc.TabIndex = 10;
            this.txtb_popGridLoc.Text = "BenMAP-CE\\Data\\populationRaster\\PopUS_90mX10_int16uWz4.tif";
            this.txtb_popGridLoc.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(283, 14);
            this.label4.TabIndex = 9;
            this.label4.Text = "Population Raster (tif: NAD_1983_Albers projection)";
            this.label4.Visible = false;
            // 
            // btnViewMetadata
            // 
            this.btnViewMetadata.Enabled = false;
            this.btnViewMetadata.Location = new System.Drawing.Point(13, 370);
            this.btnViewMetadata.Name = "btnViewMetadata";
            this.btnViewMetadata.Size = new System.Drawing.Size(130, 27);
            this.btnViewMetadata.TabIndex = 8;
            this.btnViewMetadata.Text = "View Metadata";
            this.btnViewMetadata.UseVisualStyleBackColor = true;
            this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
            // 
            // picCRHelp
            // 
            this.picCRHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picCRHelp.BackgroundImage = global::BenMAP.Properties.Resources.help_16x16;
            this.picCRHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picCRHelp.Location = new System.Drawing.Point(311, 414);
            this.picCRHelp.Name = "picCRHelp";
            this.picCRHelp.Size = new System.Drawing.Size(20, 19);
            this.picCRHelp.TabIndex = 7;
            this.picCRHelp.TabStop = false;
            this.picCRHelp.Tag = "";
            this.picCRHelp.Click += new System.EventHandler(this.picCRHelp_Click);
            this.picCRHelp.MouseHover += new System.EventHandler(this.picCRHelp_MouseHover);
            // 
            // chkBoxCreatePercentage
            // 
            this.chkBoxCreatePercentage.AutoSize = true;
            this.chkBoxCreatePercentage.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkBoxCreatePercentage.Location = new System.Drawing.Point(9, 407);
            this.chkBoxCreatePercentage.Name = "chkBoxCreatePercentage";
            this.chkBoxCreatePercentage.Size = new System.Drawing.Size(296, 32);
            this.chkBoxCreatePercentage.TabIndex = 5;
            this.chkBoxCreatePercentage.Text = "Create crosswalk between this grid definition and\r\nall other grid definitions in " +
    "this setup.";
            this.chkBoxCreatePercentage.UseVisualStyleBackColor = true;
            this.chkBoxCreatePercentage.CheckedChanged += new System.EventHandler(this.chkBoxCreatePercentage_CheckedChanged);
            // 
            // cboGridType
            // 
            this.cboGridType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridType.FormattingEnabled = true;
            this.cboGridType.Location = new System.Drawing.Point(78, 55);
            this.cboGridType.Name = "cboGridType";
            this.cboGridType.Size = new System.Drawing.Size(247, 22);
            this.cboGridType.TabIndex = 4;
            this.cboGridType.SelectedValueChanged += new System.EventHandler(this.cboGridType_SelectedValueChanged);
            // 
            // lblGridType
            // 
            this.lblGridType.AutoSize = true;
            this.lblGridType.Location = new System.Drawing.Point(6, 55);
            this.lblGridType.Name = "lblGridType";
            this.lblGridType.Size = new System.Drawing.Size(60, 14);
            this.lblGridType.TabIndex = 3;
            this.lblGridType.Text = "Grid Type:";
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(253, 374);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(73, 27);
            this.btnPreview.TabIndex = 1;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // tabGrid
            // 
            this.tabGrid.Controls.Add(this.tpShapefileGrid);
            this.tabGrid.Controls.Add(this.tpgRegularGrid);
            this.tabGrid.Location = new System.Drawing.Point(9, 142);
            this.tabGrid.Name = "tabGrid";
            this.tabGrid.SelectedIndex = 0;
            this.tabGrid.Size = new System.Drawing.Size(321, 226);
            this.tabGrid.TabIndex = 2;
            // 
            // tpShapefileGrid
            // 
            this.tpShapefileGrid.Controls.Add(this.lblRow);
            this.tpShapefileGrid.Controls.Add(this.lblCol);
            this.tpShapefileGrid.Controls.Add(this.label2);
            this.tpShapefileGrid.Controls.Add(this.label1);
            this.tpShapefileGrid.Controls.Add(this.lblShapeFileName);
            this.tpShapefileGrid.Controls.Add(this.lblShapeFile);
            this.tpShapefileGrid.Controls.Add(this.btnShapefile);
            this.tpShapefileGrid.Controls.Add(this.txtShapefile);
            this.tpShapefileGrid.Controls.Add(this.lblLoadShapefile);
            this.tpShapefileGrid.Location = new System.Drawing.Point(4, 23);
            this.tpShapefileGrid.Name = "tpShapefileGrid";
            this.tpShapefileGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tpShapefileGrid.Size = new System.Drawing.Size(313, 199);
            this.tpShapefileGrid.TabIndex = 0;
            this.tpShapefileGrid.Text = "Shapefile Grid";
            this.tpShapefileGrid.UseVisualStyleBackColor = true;
            // 
            // lblRow
            // 
            this.lblRow.AutoSize = true;
            this.lblRow.Location = new System.Drawing.Point(162, 139);
            this.lblRow.Name = "lblRow";
            this.lblRow.Size = new System.Drawing.Size(0, 14);
            this.lblRow.TabIndex = 9;
            // 
            // lblCol
            // 
            this.lblCol.AutoSize = true;
            this.lblCol.Location = new System.Drawing.Point(162, 107);
            this.lblCol.Name = "lblCol";
            this.lblCol.Size = new System.Drawing.Size(0, 14);
            this.lblCol.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 14);
            this.label2.TabIndex = 7;
            this.label2.Text = "Rows:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 14);
            this.label1.TabIndex = 6;
            this.label1.Text = "Columns:";
            // 
            // lblShapeFileName
            // 
            this.lblShapeFileName.AutoSize = true;
            this.lblShapeFileName.Location = new System.Drawing.Point(162, 78);
            this.lblShapeFileName.Name = "lblShapeFileName";
            this.lblShapeFileName.Size = new System.Drawing.Size(0, 14);
            this.lblShapeFileName.TabIndex = 5;
            // 
            // lblShapeFile
            // 
            this.lblShapeFile.AutoSize = true;
            this.lblShapeFile.Location = new System.Drawing.Point(13, 78);
            this.lblShapeFile.Name = "lblShapeFile";
            this.lblShapeFile.Size = new System.Drawing.Size(138, 14);
            this.lblShapeFile.TabIndex = 4;
            this.lblShapeFile.Text = "Current Shapefile Name:";
            // 
            // btnShapefile
            // 
            this.btnShapefile.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnShapefile.Location = new System.Drawing.Point(228, 36);
            this.btnShapefile.Name = "btnShapefile";
            this.btnShapefile.Size = new System.Drawing.Size(73, 27);
            this.btnShapefile.TabIndex = 3;
            this.btnShapefile.UseVisualStyleBackColor = true;
            this.btnShapefile.Click += new System.EventHandler(this.btnShapefile_Click);
            // 
            // txtShapefile
            // 
            this.txtShapefile.Location = new System.Drawing.Point(11, 38);
            this.txtShapefile.Name = "txtShapefile";
            this.txtShapefile.ReadOnly = true;
            this.txtShapefile.Size = new System.Drawing.Size(211, 22);
            this.txtShapefile.TabIndex = 2;
            this.txtShapefile.TextChanged += new System.EventHandler(this.txtShapefile_TextChanged);
            // 
            // lblLoadShapefile
            // 
            this.lblLoadShapefile.AutoSize = true;
            this.lblLoadShapefile.Location = new System.Drawing.Point(13, 15);
            this.lblLoadShapefile.Name = "lblLoadShapefile";
            this.lblLoadShapefile.Size = new System.Drawing.Size(90, 14);
            this.lblLoadShapefile.TabIndex = 0;
            this.lblLoadShapefile.Text = "Load Shapefile:";
            // 
            // tpgRegularGrid
            // 
            this.tpgRegularGrid.Controls.Add(this.nudRowsPerLatitude);
            this.tpgRegularGrid.Controls.Add(this.nudColumnsPerLongitude);
            this.tpgRegularGrid.Controls.Add(this.txtMinimumLatitude);
            this.tpgRegularGrid.Controls.Add(this.txtMinimumLongitude);
            this.tpgRegularGrid.Controls.Add(this.nudRows);
            this.tpgRegularGrid.Controls.Add(this.nudColumns);
            this.tpgRegularGrid.Controls.Add(this.lblRowsPerLatitude);
            this.tpgRegularGrid.Controls.Add(this.lblColumsPerLongitude);
            this.tpgRegularGrid.Controls.Add(this.lblMinimumLatitude);
            this.tpgRegularGrid.Controls.Add(this.lblMinimumLongitude);
            this.tpgRegularGrid.Controls.Add(this.lblRows);
            this.tpgRegularGrid.Controls.Add(this.lblColumns);
            this.tpgRegularGrid.Location = new System.Drawing.Point(4, 23);
            this.tpgRegularGrid.Name = "tpgRegularGrid";
            this.tpgRegularGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tpgRegularGrid.Size = new System.Drawing.Size(313, 199);
            this.tpgRegularGrid.TabIndex = 1;
            this.tpgRegularGrid.Text = "Regular Grid";
            this.tpgRegularGrid.UseVisualStyleBackColor = true;
            // 
            // nudRowsPerLatitude
            // 
            this.nudRowsPerLatitude.Location = new System.Drawing.Point(162, 154);
            this.nudRowsPerLatitude.Name = "nudRowsPerLatitude";
            this.nudRowsPerLatitude.Size = new System.Drawing.Size(99, 22);
            this.nudRowsPerLatitude.TabIndex = 11;
            this.nudRowsPerLatitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudRowsPerLatitude.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRowsPerLatitude.ValueChanged += new System.EventHandler(this.nudRowsPerLatitude_ValueChanged);
            // 
            // nudColumnsPerLongitude
            // 
            this.nudColumnsPerLongitude.Location = new System.Drawing.Point(9, 155);
            this.nudColumnsPerLongitude.Name = "nudColumnsPerLongitude";
            this.nudColumnsPerLongitude.Size = new System.Drawing.Size(99, 22);
            this.nudColumnsPerLongitude.TabIndex = 10;
            this.nudColumnsPerLongitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudColumnsPerLongitude.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudColumnsPerLongitude.ValueChanged += new System.EventHandler(this.nudColumnsPerLongitude_ValueChanged);
            // 
            // txtMinimumLatitude
            // 
            this.txtMinimumLatitude.Location = new System.Drawing.Point(162, 98);
            this.txtMinimumLatitude.Name = "txtMinimumLatitude";
            this.txtMinimumLatitude.Size = new System.Drawing.Size(100, 22);
            this.txtMinimumLatitude.TabIndex = 9;
            this.txtMinimumLatitude.Text = "0";
            this.txtMinimumLatitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMinimumLongitude
            // 
            this.txtMinimumLongitude.Location = new System.Drawing.Point(9, 98);
            this.txtMinimumLongitude.Name = "txtMinimumLongitude";
            this.txtMinimumLongitude.Size = new System.Drawing.Size(99, 22);
            this.txtMinimumLongitude.TabIndex = 8;
            this.txtMinimumLongitude.Text = "0";
            this.txtMinimumLongitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // nudRows
            // 
            this.nudRows.Location = new System.Drawing.Point(162, 36);
            this.nudRows.Name = "nudRows";
            this.nudRows.Size = new System.Drawing.Size(99, 22);
            this.nudRows.TabIndex = 7;
            this.nudRows.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRows.ValueChanged += new System.EventHandler(this.nudRows_ValueChanged);
            // 
            // nudColumns
            // 
            this.nudColumns.Location = new System.Drawing.Point(9, 36);
            this.nudColumns.Name = "nudColumns";
            this.nudColumns.Size = new System.Drawing.Size(99, 22);
            this.nudColumns.TabIndex = 6;
            this.nudColumns.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudColumns.ValueChanged += new System.EventHandler(this.nudColumns_ValueChanged);
            // 
            // lblRowsPerLatitude
            // 
            this.lblRowsPerLatitude.AutoSize = true;
            this.lblRowsPerLatitude.Location = new System.Drawing.Point(160, 136);
            this.lblRowsPerLatitude.Name = "lblRowsPerLatitude";
            this.lblRowsPerLatitude.Size = new System.Drawing.Size(103, 14);
            this.lblRowsPerLatitude.TabIndex = 5;
            this.lblRowsPerLatitude.Text = "Rows Per Latitude";
            // 
            // lblColumsPerLongitude
            // 
            this.lblColumsPerLongitude.AutoSize = true;
            this.lblColumsPerLongitude.Location = new System.Drawing.Point(7, 136);
            this.lblColumsPerLongitude.Name = "lblColumsPerLongitude";
            this.lblColumsPerLongitude.Size = new System.Drawing.Size(131, 14);
            this.lblColumsPerLongitude.TabIndex = 4;
            this.lblColumsPerLongitude.Text = "Columns Per Longitude";
            // 
            // lblMinimumLatitude
            // 
            this.lblMinimumLatitude.AutoSize = true;
            this.lblMinimumLatitude.Location = new System.Drawing.Point(160, 79);
            this.lblMinimumLatitude.Name = "lblMinimumLatitude";
            this.lblMinimumLatitude.Size = new System.Drawing.Size(106, 14);
            this.lblMinimumLatitude.TabIndex = 3;
            this.lblMinimumLatitude.Text = "Minimum Latitude";
            // 
            // lblMinimumLongitude
            // 
            this.lblMinimumLongitude.AutoSize = true;
            this.lblMinimumLongitude.Location = new System.Drawing.Point(6, 79);
            this.lblMinimumLongitude.Name = "lblMinimumLongitude";
            this.lblMinimumLongitude.Size = new System.Drawing.Size(116, 14);
            this.lblMinimumLongitude.TabIndex = 2;
            this.lblMinimumLongitude.Text = "Minimum Longitude";
            // 
            // lblRows
            // 
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new System.Drawing.Point(160, 19);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(36, 14);
            this.lblRows.TabIndex = 1;
            this.lblRows.Text = "Rows";
            // 
            // lblColumns
            // 
            this.lblColumns.AutoSize = true;
            this.lblColumns.Location = new System.Drawing.Point(7, 19);
            this.lblColumns.Name = "lblColumns";
            this.lblColumns.Size = new System.Drawing.Size(54, 14);
            this.lblColumns.TabIndex = 0;
            this.lblColumns.Text = "Columns";
            // 
            // txtGridID
            // 
            this.txtGridID.Location = new System.Drawing.Point(77, 27);
            this.txtGridID.Name = "txtGridID";
            this.txtGridID.Size = new System.Drawing.Size(248, 22);
            this.txtGridID.TabIndex = 1;
            // 
            // lblGridID
            // 
            this.lblGridID.AutoSize = true;
            this.lblGridID.Location = new System.Drawing.Point(6, 30);
            this.lblGridID.Name = "lblGridID";
            this.lblGridID.Size = new System.Drawing.Size(48, 14);
            this.lblGridID.TabIndex = 0;
            this.lblGridID.Text = "Grid ID:";
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.lblprogress);
            this.grpCancelOK.Controls.Add(this.progressBar1);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 501);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(777, 56);
            this.grpCancelOK.TabIndex = 4;
            this.grpCancelOK.TabStop = false;
            // 
            // lblprogress
            // 
            this.lblprogress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblprogress.AutoSize = true;
            this.lblprogress.Location = new System.Drawing.Point(9, 24);
            this.lblprogress.Name = "lblprogress";
            this.lblprogress.Size = new System.Drawing.Size(133, 14);
            this.lblprogress.TabIndex = 5;
            this.lblprogress.Text = "Calculating Percentage:";
            this.lblprogress.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(146, 26);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(460, 10);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(615, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(696, 19);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // GridDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(801, 561);
            this.Controls.Add(this.grpPictureView);
            this.Controls.Add(this.grpGridDefinition);
            this.Controls.Add(this.grpCancelOK);
            this.Name = "GridDefinition";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Grid Definition";
            this.Load += new System.EventHandler(this.GridDefinition_Load);
            this.grpPictureView.ResumeLayout(false);
            this.grpGridDefinition.ResumeLayout(false);
            this.grpGridDefinition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCRHelp)).EndInit();
            this.tabGrid.ResumeLayout(false);
            this.tpShapefileGrid.ResumeLayout(false);
            this.tpShapefileGrid.PerformLayout();
            this.tpgRegularGrid.ResumeLayout(false);
            this.tpgRegularGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRowsPerLatitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumnsPerLongitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumns)).EndInit();
            this.grpCancelOK.ResumeLayout(false);
            this.grpCancelOK.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grpGridDefinition;
        private System.Windows.Forms.TabControl tabGrid;
        private System.Windows.Forms.TabPage tpShapefileGrid;
        private System.Windows.Forms.Button btnShapefile;
        private System.Windows.Forms.TextBox txtShapefile;
        private System.Windows.Forms.Label lblLoadShapefile;
        private System.Windows.Forms.TabPage tpgRegularGrid;
        private System.Windows.Forms.NumericUpDown nudRowsPerLatitude;
        private System.Windows.Forms.NumericUpDown nudColumnsPerLongitude;
        private System.Windows.Forms.TextBox txtMinimumLatitude;
        private System.Windows.Forms.TextBox txtMinimumLongitude;
        private System.Windows.Forms.NumericUpDown nudRows;
        private System.Windows.Forms.NumericUpDown nudColumns;
        private System.Windows.Forms.Label lblRowsPerLatitude;
        private System.Windows.Forms.Label lblColumsPerLongitude;
        private System.Windows.Forms.Label lblMinimumLatitude;
        private System.Windows.Forms.Label lblMinimumLongitude;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.Label lblColumns;
        private System.Windows.Forms.TextBox txtGridID;
        private System.Windows.Forms.Label lblGridID;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.GroupBox grpPictureView;
        private System.Windows.Forms.ComboBox cboGridType;
        private System.Windows.Forms.Label lblGridType;
        private DotSpatial.Controls.Map mainMap;
        private System.Windows.Forms.Label lblShapeFileName;
        private System.Windows.Forms.Label lblShapeFile;
        private System.Windows.Forms.Label lblRow;
        private System.Windows.Forms.Label lblCol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblprogress;
        private System.Windows.Forms.CheckBox chkBoxCreatePercentage;
        private System.Windows.Forms.PictureBox picCRHelp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnViewMetadata;
        private System.Windows.Forms.Button btn_browsePopRaster;
        private System.Windows.Forms.TextBox txtb_popGridLoc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkBoxUseAsGeographicArea;
    }
}