namespace BenMAP
{
    partial class GBDRollback
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GBDRollback));
            this.gbCountrySelection = new System.Windows.Forms.GroupBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext2 = new System.Windows.Forms.Button();
            this.tvRegions = new System.Windows.Forms.TreeView();
            this.gbRollbacks = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnDeleteRollback = new System.Windows.Forms.Button();
            this.btnEditRollback = new System.Windows.Forms.Button();
            this.btnExecuteRollbacks = new System.Windows.Forms.Button();
            this.dgvRollbacks = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalCountries = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalPopulation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRollbackType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExecute = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnPan = new System.Windows.Forms.ToolStripButton();
            this.btnFullExtent = new System.Windows.Forms.ToolStripButton();
            this.btnSpatial = new System.Windows.Forms.ToolStripButton();
            this.btnIdentify = new System.Windows.Forms.ToolStripButton();
            this.btnLayerSet = new System.Windows.Forms.ToolStripButton();
            this.btnPieTheme = new System.Windows.Forms.ToolStripButton();
            this.btnColumnTheme = new System.Windows.Forms.ToolStripButton();
            this.tsbSaveMap = new System.Windows.Forms.ToolStripButton();
            this.tsbSavePic = new System.Windows.Forms.ToolStripButton();
            this.tsbChangeProjection = new System.Windows.Forms.ToolStripButton();
            this.tsbChangeCone = new System.Windows.Forms.ToolStripButton();
            this.tsbAddLayer = new System.Windows.Forms.ToolStripButton();
            this.mapGBD = new DotSpatial.Controls.Map();
            this.gbParameterSelection = new System.Windows.Forms.GroupBox();
            this.pb_incremental = new System.Windows.Forms.PictureBox();
            this.gbOptionsIncremental = new System.Windows.Forms.GroupBox();
            this.txtIncrementBackground = new System.Windows.Forms.TextBox();
            this.txtIncrement = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboRollbackType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSaveRollback = new System.Windows.Forms.Button();
            this.btnBack2 = new System.Windows.Forms.Button();
            this.pb_standard = new System.Windows.Forms.PictureBox();
            this.pb_percent = new System.Windows.Forms.PictureBox();
            this.gbOptionsPercentage = new System.Windows.Forms.GroupBox();
            this.txtPercentageBackground = new System.Windows.Forms.TextBox();
            this.txtPercentage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.gbOptionsStandard = new System.Windows.Forms.GroupBox();
            this.cboStandard = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.gbName = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rbRegions = new System.Windows.Forms.RadioButton();
            this.rbCountries = new System.Windows.Forms.RadioButton();
            this.listCountries = new System.Windows.Forms.ListView();
            this.gbCountrySelection.SuspendLayout();
            this.gbRollbacks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRollbacks)).BeginInit();
            this.gbMap.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.gbParameterSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_incremental)).BeginInit();
            this.gbOptionsIncremental.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_standard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_percent)).BeginInit();
            this.gbOptionsPercentage.SuspendLayout();
            this.gbOptionsStandard.SuspendLayout();
            this.gbName.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbCountrySelection
            // 
            this.gbCountrySelection.Controls.Add(this.listCountries);
            this.gbCountrySelection.Controls.Add(this.rbCountries);
            this.gbCountrySelection.Controls.Add(this.rbRegions);
            this.gbCountrySelection.Controls.Add(this.btnBack);
            this.gbCountrySelection.Controls.Add(this.btnNext2);
            this.gbCountrySelection.Controls.Add(this.tvRegions);
            this.gbCountrySelection.Location = new System.Drawing.Point(1208, 12);
            this.gbCountrySelection.Name = "gbCountrySelection";
            this.gbCountrySelection.Size = new System.Drawing.Size(279, 420);
            this.gbCountrySelection.TabIndex = 0;
            this.gbCountrySelection.TabStop = false;
            this.gbCountrySelection.Text = "Region Selection";
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(24, 386);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(118, 23);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "<- Scenario Name";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext2
            // 
            this.btnNext2.Location = new System.Drawing.Point(149, 386);
            this.btnNext2.Name = "btnNext2";
            this.btnNext2.Size = new System.Drawing.Size(118, 23);
            this.btnNext2.TabIndex = 1;
            this.btnNext2.Text = "Rollback Settings ->";
            this.btnNext2.UseVisualStyleBackColor = true;
            this.btnNext2.Click += new System.EventHandler(this.btnNext2_Click);
            // 
            // tvRegions
            // 
            this.tvRegions.CheckBoxes = true;
            this.tvRegions.Location = new System.Drawing.Point(10, 43);
            this.tvRegions.Name = "tvRegions";
            this.tvRegions.Size = new System.Drawing.Size(258, 332);
            this.tvRegions.TabIndex = 0;
            this.tvRegions.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvRegions_AfterCheck);
            // 
            // gbRollbacks
            // 
            this.gbRollbacks.Controls.Add(this.btnBrowse);
            this.gbRollbacks.Controls.Add(this.txtFilePath);
            this.gbRollbacks.Controls.Add(this.btnDeleteRollback);
            this.gbRollbacks.Controls.Add(this.btnEditRollback);
            this.gbRollbacks.Controls.Add(this.btnExecuteRollbacks);
            this.gbRollbacks.Controls.Add(this.dgvRollbacks);
            this.gbRollbacks.Location = new System.Drawing.Point(13, 440);
            this.gbRollbacks.Name = "gbRollbacks";
            this.gbRollbacks.Size = new System.Drawing.Size(874, 269);
            this.gbRollbacks.TabIndex = 1;
            this.gbRollbacks.TabStop = false;
            this.gbRollbacks.Text = "Scenarios";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(783, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.BackColor = System.Drawing.SystemColors.Window;
            this.txtFilePath.Location = new System.Drawing.Point(456, 23);
            this.txtFilePath.MaxLength = 15;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(321, 20);
            this.txtFilePath.TabIndex = 6;
            // 
            // btnDeleteRollback
            // 
            this.btnDeleteRollback.Location = new System.Drawing.Point(109, 20);
            this.btnDeleteRollback.Name = "btnDeleteRollback";
            this.btnDeleteRollback.Size = new System.Drawing.Size(99, 23);
            this.btnDeleteRollback.TabIndex = 5;
            this.btnDeleteRollback.Text = "Delete Scenario";
            this.btnDeleteRollback.UseVisualStyleBackColor = true;
            this.btnDeleteRollback.Click += new System.EventHandler(this.btnDeleteRollback_Click);
            // 
            // btnEditRollback
            // 
            this.btnEditRollback.Location = new System.Drawing.Point(17, 20);
            this.btnEditRollback.Name = "btnEditRollback";
            this.btnEditRollback.Size = new System.Drawing.Size(86, 23);
            this.btnEditRollback.TabIndex = 4;
            this.btnEditRollback.Text = "Edit Scenario";
            this.btnEditRollback.UseVisualStyleBackColor = true;
            this.btnEditRollback.Click += new System.EventHandler(this.btnEditRollback_Click);
            // 
            // btnExecuteRollbacks
            // 
            this.btnExecuteRollbacks.Enabled = false;
            this.btnExecuteRollbacks.Location = new System.Drawing.Point(328, 21);
            this.btnExecuteRollbacks.Name = "btnExecuteRollbacks";
            this.btnExecuteRollbacks.Size = new System.Drawing.Size(122, 23);
            this.btnExecuteRollbacks.TabIndex = 3;
            this.btnExecuteRollbacks.Text = "Execute Scenarios";
            this.btnExecuteRollbacks.UseVisualStyleBackColor = true;
            this.btnExecuteRollbacks.Click += new System.EventHandler(this.btnExecuteRollbacks_Click);
            // 
            // dgvRollbacks
            // 
            this.dgvRollbacks.AllowUserToAddRows = false;
            this.dgvRollbacks.AllowUserToDeleteRows = false;
            this.dgvRollbacks.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvRollbacks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRollbacks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colColor,
            this.colTotalCountries,
            this.colTotalPopulation,
            this.colRollbackType,
            this.colExecute});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRollbacks.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRollbacks.Location = new System.Drawing.Point(17, 55);
            this.dgvRollbacks.MultiSelect = false;
            this.dgvRollbacks.Name = "dgvRollbacks";
            this.dgvRollbacks.Size = new System.Drawing.Size(841, 197);
            this.dgvRollbacks.TabIndex = 0;
            this.dgvRollbacks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRollbacks_CellContentClick);
            this.dgvRollbacks.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRollbacks_CellDoubleClick);
            // 
            // colName
            // 
            this.colName.HeaderText = "Scenario Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colColor
            // 
            this.colColor.HeaderText = "Color";
            this.colColor.Name = "colColor";
            this.colColor.ReadOnly = true;
            this.colColor.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colTotalCountries
            // 
            this.colTotalCountries.HeaderText = "Total Countries";
            this.colTotalCountries.Name = "colTotalCountries";
            this.colTotalCountries.ReadOnly = true;
            // 
            // colTotalPopulation
            // 
            this.colTotalPopulation.HeaderText = "Total Population";
            this.colTotalPopulation.Name = "colTotalPopulation";
            this.colTotalPopulation.ReadOnly = true;
            // 
            // colRollbackType
            // 
            this.colRollbackType.HeaderText = "Type Of Rollback";
            this.colRollbackType.Name = "colRollbackType";
            this.colRollbackType.ReadOnly = true;
            // 
            // colExecute
            // 
            this.colExecute.HeaderText = "Execute?";
            this.colExecute.Name = "colExecute";
            this.colExecute.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colExecute.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(813, 716);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gbMap
            // 
            this.gbMap.Controls.Add(this.toolStrip1);
            this.gbMap.Controls.Add(this.mapGBD);
            this.gbMap.Location = new System.Drawing.Point(299, 7);
            this.gbMap.Name = "gbMap";
            this.gbMap.Size = new System.Drawing.Size(589, 420);
            this.gbMap.TabIndex = 4;
            this.gbMap.TabStop = false;
            this.gbMap.Text = "Map";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomIn,
            this.btnZoomOut,
            this.btnPan,
            this.btnFullExtent,
            this.btnSpatial,
            this.btnIdentify,
            this.btnLayerSet,
            this.btnPieTheme,
            this.btnColumnTheme,
            this.tsbSaveMap,
            this.tsbSavePic,
            this.tsbChangeProjection,
            this.tsbChangeCone,
            this.tsbAddLayer});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(583, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = global::BenMAP.Properties.Resources.magnifier_zoom_in;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoomIn.Text = "toolStripButton1";
            this.btnZoomIn.ToolTipText = "Zoom in";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = global::BenMAP.Properties.Resources.magnifier_zoom_out;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.btnZoomOut.Text = "toolStripButton2";
            this.btnZoomOut.ToolTipText = "Zoom out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnPan
            // 
            this.btnPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPan.Image = global::BenMAP.Properties.Resources.pan_2;
            this.btnPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(23, 22);
            this.btnPan.Text = "toolStripButton3";
            this.btnPan.ToolTipText = "Pan";
            this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
            // 
            // btnFullExtent
            // 
            this.btnFullExtent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFullExtent.Image = global::BenMAP.Properties.Resources.globe_7;
            this.btnFullExtent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFullExtent.Name = "btnFullExtent";
            this.btnFullExtent.Size = new System.Drawing.Size(23, 22);
            this.btnFullExtent.Text = "toolStripButton4";
            this.btnFullExtent.ToolTipText = "Full extent";
            this.btnFullExtent.Click += new System.EventHandler(this.btnFullExtent_Click);
            // 
            // btnSpatial
            // 
            this.btnSpatial.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpatial.Image = global::BenMAP.Properties.Resources.chart4;
            this.btnSpatial.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpatial.Name = "btnSpatial";
            this.btnSpatial.Size = new System.Drawing.Size(23, 22);
            this.btnSpatial.Text = "toolStripButton5";
            this.btnSpatial.ToolTipText = "Spatial analysis";
            this.btnSpatial.Visible = false;
            // 
            // btnIdentify
            // 
            this.btnIdentify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnIdentify.Image = global::BenMAP.Properties.Resources.identifier_16;
            this.btnIdentify.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIdentify.Name = "btnIdentify";
            this.btnIdentify.Size = new System.Drawing.Size(23, 22);
            this.btnIdentify.Text = "Identify";
            this.btnIdentify.Click += new System.EventHandler(this.btnIdentify_Click);
            // 
            // btnLayerSet
            // 
            this.btnLayerSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLayerSet.Image = global::BenMAP.Properties.Resources.legend;
            this.btnLayerSet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLayerSet.Name = "btnLayerSet";
            this.btnLayerSet.Size = new System.Drawing.Size(23, 22);
            this.btnLayerSet.Text = "Show Table of Contents";
            this.btnLayerSet.ToolTipText = "Show Table of Contents";
            this.btnLayerSet.Visible = false;
            // 
            // btnPieTheme
            // 
            this.btnPieTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPieTheme.Image = global::BenMAP.Properties.Resources.tableView1;
            this.btnPieTheme.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPieTheme.Name = "btnPieTheme";
            this.btnPieTheme.Size = new System.Drawing.Size(23, 22);
            this.btnPieTheme.Text = "Pie Theme";
            this.btnPieTheme.Visible = false;
            // 
            // btnColumnTheme
            // 
            this.btnColumnTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnColumnTheme.Image = global::BenMAP.Properties.Resources.tableView_Bar;
            this.btnColumnTheme.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnColumnTheme.Name = "btnColumnTheme";
            this.btnColumnTheme.Size = new System.Drawing.Size(23, 22);
            this.btnColumnTheme.Text = "Column Theme";
            this.btnColumnTheme.Visible = false;
            // 
            // tsbSaveMap
            // 
            this.tsbSaveMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveMap.Image = global::BenMAP.Properties.Resources.save_as;
            this.tsbSaveMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveMap.Name = "tsbSaveMap";
            this.tsbSaveMap.Size = new System.Drawing.Size(23, 22);
            this.tsbSaveMap.Tag = "";
            this.tsbSaveMap.Text = "Save shapefile";
            this.tsbSaveMap.Visible = false;
            // 
            // tsbSavePic
            // 
            this.tsbSavePic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSavePic.Image = ((System.Drawing.Image)(resources.GetObject("tsbSavePic.Image")));
            this.tsbSavePic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSavePic.Name = "tsbSavePic";
            this.tsbSavePic.Size = new System.Drawing.Size(23, 22);
            this.tsbSavePic.Tag = "";
            this.tsbSavePic.Text = "Export map image";
            this.tsbSavePic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbSavePic.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsbSavePic.Visible = false;
            // 
            // tsbChangeProjection
            // 
            this.tsbChangeProjection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbChangeProjection.Image = global::BenMAP.Properties.Resources.cuahsi_logo1;
            this.tsbChangeProjection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbChangeProjection.Name = "tsbChangeProjection";
            this.tsbChangeProjection.Size = new System.Drawing.Size(23, 22);
            this.tsbChangeProjection.Text = "ChangeProjection";
            this.tsbChangeProjection.Visible = false;
            // 
            // tsbChangeCone
            // 
            this.tsbChangeCone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbChangeCone.Image = ((System.Drawing.Image)(resources.GetObject("tsbChangeCone.Image")));
            this.tsbChangeCone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbChangeCone.Name = "tsbChangeCone";
            this.tsbChangeCone.Size = new System.Drawing.Size(23, 22);
            this.tsbChangeCone.Text = "Change to square view";
            this.tsbChangeCone.Visible = false;
            // 
            // tsbAddLayer
            // 
            this.tsbAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddLayer.Image = global::BenMAP.Properties.Resources.add;
            this.tsbAddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddLayer.Name = "tsbAddLayer";
            this.tsbAddLayer.Size = new System.Drawing.Size(23, 22);
            this.tsbAddLayer.Text = "Add Layer";
            this.tsbAddLayer.Visible = false;
            // 
            // mapGBD
            // 
            this.mapGBD.AllowDrop = true;
            this.mapGBD.BackColor = System.Drawing.Color.White;
            this.mapGBD.CollectAfterDraw = false;
            this.mapGBD.CollisionDetection = false;
            this.mapGBD.ExtendBuffer = false;
            this.mapGBD.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.mapGBD.IsBusy = false;
            this.mapGBD.IsZoomedToMaxExtent = false;
            this.mapGBD.Legend = null;
            this.mapGBD.Location = new System.Drawing.Point(3, 20);
            this.mapGBD.Name = "mapGBD";
            this.mapGBD.ProgressHandler = null;
            this.mapGBD.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.mapGBD.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.mapGBD.RedrawLayersWhileResizing = false;
            this.mapGBD.SelectionEnabled = true;
            this.mapGBD.Size = new System.Drawing.Size(582, 389);
            this.mapGBD.TabIndex = 0;
            // 
            // gbParameterSelection
            // 
            this.gbParameterSelection.Controls.Add(this.pb_incremental);
            this.gbParameterSelection.Controls.Add(this.gbOptionsIncremental);
            this.gbParameterSelection.Controls.Add(this.cboRollbackType);
            this.gbParameterSelection.Controls.Add(this.label3);
            this.gbParameterSelection.Controls.Add(this.btnSaveRollback);
            this.gbParameterSelection.Controls.Add(this.btnBack2);
            this.gbParameterSelection.Controls.Add(this.pb_standard);
            this.gbParameterSelection.Controls.Add(this.pb_percent);
            this.gbParameterSelection.Location = new System.Drawing.Point(909, 7);
            this.gbParameterSelection.Name = "gbParameterSelection";
            this.gbParameterSelection.Size = new System.Drawing.Size(279, 420);
            this.gbParameterSelection.TabIndex = 3;
            this.gbParameterSelection.TabStop = false;
            this.gbParameterSelection.Text = "Rollback Settings";
            // 
            // pb_incremental
            // 
            this.pb_incremental.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pb_incremental.BackgroundImage")));
            this.pb_incremental.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pb_incremental.InitialImage = null;
            this.pb_incremental.Location = new System.Drawing.Point(15, 261);
            this.pb_incremental.Name = "pb_incremental";
            this.pb_incremental.Size = new System.Drawing.Size(253, 119);
            this.pb_incremental.TabIndex = 7;
            this.pb_incremental.TabStop = false;
            this.pb_incremental.Visible = false;
            // 
            // gbOptionsIncremental
            // 
            this.gbOptionsIncremental.Controls.Add(this.txtIncrementBackground);
            this.gbOptionsIncremental.Controls.Add(this.txtIncrement);
            this.gbOptionsIncremental.Controls.Add(this.label5);
            this.gbOptionsIncremental.Controls.Add(this.label4);
            this.gbOptionsIncremental.Location = new System.Drawing.Point(15, 48);
            this.gbOptionsIncremental.Name = "gbOptionsIncremental";
            this.gbOptionsIncremental.Size = new System.Drawing.Size(253, 206);
            this.gbOptionsIncremental.TabIndex = 6;
            this.gbOptionsIncremental.TabStop = false;
            this.gbOptionsIncremental.Text = "Options";
            // 
            // txtIncrementBackground
            // 
            this.txtIncrementBackground.Location = new System.Drawing.Point(96, 45);
            this.txtIncrementBackground.Name = "txtIncrementBackground";
            this.txtIncrementBackground.Size = new System.Drawing.Size(100, 20);
            this.txtIncrementBackground.TabIndex = 3;
            this.txtIncrementBackground.Visible = false;
            // 
            // txtIncrement
            // 
            this.txtIncrement.Location = new System.Drawing.Point(96, 19);
            this.txtIncrement.Name = "txtIncrement";
            this.txtIncrement.Size = new System.Drawing.Size(100, 20);
            this.txtIncrement.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Background:";
            this.label5.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Increment:";
            // 
            // cboRollbackType
            // 
            this.cboRollbackType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRollbackType.FormattingEnabled = true;
            this.cboRollbackType.Items.AddRange(new object[] {
            "Percentage Rollback",
            "Incremental Rollback",
            "Rollback to a Standard"});
            this.cboRollbackType.Location = new System.Drawing.Point(98, 20);
            this.cboRollbackType.Name = "cboRollbackType";
            this.cboRollbackType.Size = new System.Drawing.Size(175, 21);
            this.cboRollbackType.TabIndex = 5;
            this.cboRollbackType.SelectedIndexChanged += new System.EventHandler(this.cboRollbackType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Rollback Type:";
            // 
            // btnSaveRollback
            // 
            this.btnSaveRollback.Location = new System.Drawing.Point(171, 386);
            this.btnSaveRollback.Name = "btnSaveRollback";
            this.btnSaveRollback.Size = new System.Drawing.Size(97, 23);
            this.btnSaveRollback.TabIndex = 2;
            this.btnSaveRollback.Text = "Save Scenario";
            this.btnSaveRollback.UseVisualStyleBackColor = true;
            this.btnSaveRollback.Click += new System.EventHandler(this.btnSaveRollback_Click);
            // 
            // btnBack2
            // 
            this.btnBack2.Location = new System.Drawing.Point(46, 386);
            this.btnBack2.Name = "btnBack2";
            this.btnBack2.Size = new System.Drawing.Size(118, 23);
            this.btnBack2.TabIndex = 1;
            this.btnBack2.Text = "<- Region Selection";
            this.btnBack2.UseVisualStyleBackColor = true;
            this.btnBack2.Click += new System.EventHandler(this.btnBack2_Click);
            // 
            // pb_standard
            // 
            this.pb_standard.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pb_standard.BackgroundImage")));
            this.pb_standard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pb_standard.InitialImage = null;
            this.pb_standard.Location = new System.Drawing.Point(15, 261);
            this.pb_standard.Name = "pb_standard";
            this.pb_standard.Size = new System.Drawing.Size(253, 119);
            this.pb_standard.TabIndex = 9;
            this.pb_standard.TabStop = false;
            this.pb_standard.Visible = false;
            // 
            // pb_percent
            // 
            this.pb_percent.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pb_percent.BackgroundImage")));
            this.pb_percent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pb_percent.InitialImage = null;
            this.pb_percent.Location = new System.Drawing.Point(15, 260);
            this.pb_percent.Name = "pb_percent";
            this.pb_percent.Size = new System.Drawing.Size(253, 119);
            this.pb_percent.TabIndex = 8;
            this.pb_percent.TabStop = false;
            // 
            // gbOptionsPercentage
            // 
            this.gbOptionsPercentage.Controls.Add(this.txtPercentageBackground);
            this.gbOptionsPercentage.Controls.Add(this.txtPercentage);
            this.gbOptionsPercentage.Controls.Add(this.label6);
            this.gbOptionsPercentage.Controls.Add(this.label7);
            this.gbOptionsPercentage.Location = new System.Drawing.Point(924, 440);
            this.gbOptionsPercentage.Name = "gbOptionsPercentage";
            this.gbOptionsPercentage.Size = new System.Drawing.Size(253, 206);
            this.gbOptionsPercentage.TabIndex = 7;
            this.gbOptionsPercentage.TabStop = false;
            this.gbOptionsPercentage.Text = "Options";
            // 
            // txtPercentageBackground
            // 
            this.txtPercentageBackground.Location = new System.Drawing.Point(96, 45);
            this.txtPercentageBackground.Name = "txtPercentageBackground";
            this.txtPercentageBackground.Size = new System.Drawing.Size(100, 20);
            this.txtPercentageBackground.TabIndex = 3;
            this.txtPercentageBackground.Visible = false;
            // 
            // txtPercentage
            // 
            this.txtPercentage.Location = new System.Drawing.Point(96, 19);
            this.txtPercentage.Name = "txtPercentage";
            this.txtPercentage.Size = new System.Drawing.Size(100, 20);
            this.txtPercentage.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Background:";
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Percentage:";
            // 
            // gbOptionsStandard
            // 
            this.gbOptionsStandard.Controls.Add(this.cboStandard);
            this.gbOptionsStandard.Controls.Add(this.label9);
            this.gbOptionsStandard.Location = new System.Drawing.Point(929, 669);
            this.gbOptionsStandard.Name = "gbOptionsStandard";
            this.gbOptionsStandard.Size = new System.Drawing.Size(253, 206);
            this.gbOptionsStandard.TabIndex = 8;
            this.gbOptionsStandard.TabStop = false;
            this.gbOptionsStandard.Text = "Options";
            // 
            // cboStandard
            // 
            this.cboStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStandard.FormattingEnabled = true;
            this.cboStandard.Location = new System.Drawing.Point(63, 16);
            this.cboStandard.Name = "cboStandard";
            this.cboStandard.Size = new System.Drawing.Size(185, 21);
            this.cboStandard.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Standard:";
            // 
            // gbName
            // 
            this.gbName.Controls.Add(this.txtDescription);
            this.gbName.Controls.Add(this.label10);
            this.gbName.Controls.Add(this.txtName);
            this.gbName.Controls.Add(this.label8);
            this.gbName.Controls.Add(this.btnNext);
            this.gbName.Location = new System.Drawing.Point(12, 7);
            this.gbName.Name = "gbName";
            this.gbName.Size = new System.Drawing.Size(279, 420);
            this.gbName.TabIndex = 3;
            this.gbName.TabStop = false;
            this.gbName.Text = "Create a new scenario";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(96, 76);
            this.txtDescription.MaxLength = 255;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(172, 113);
            this.txtDescription.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(7, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 31);
            this.label10.TabIndex = 4;
            this.label10.Text = "Describe Scenario:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(96, 35);
            this.txtName.MaxLength = 15;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(172, 20);
            this.txtName.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Scenario Name:";
            this.toolTip1.SetToolTip(this.label8, "The name of the scenario will also be used in the rollback report filename.  It i" +
                    "s limited to 15 characters.");
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(150, 386);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(118, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Select Region ->";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // rbRegions
            // 
            this.rbRegions.AutoSize = true;
            this.rbRegions.Location = new System.Drawing.Point(12, 20);
            this.rbRegions.Name = "rbRegions";
            this.rbRegions.Size = new System.Drawing.Size(64, 17);
            this.rbRegions.TabIndex = 4;
            this.rbRegions.TabStop = true;
            this.rbRegions.Text = "Regions";
            this.rbRegions.UseVisualStyleBackColor = true;
            this.rbRegions.CheckedChanged += new System.EventHandler(this.rbRegions_CheckedChanged);
            // 
            // rbCountries
            // 
            this.rbCountries.AutoSize = true;
            this.rbCountries.Location = new System.Drawing.Point(80, 20);
            this.rbCountries.Name = "rbCountries";
            this.rbCountries.Size = new System.Drawing.Size(69, 17);
            this.rbCountries.TabIndex = 5;
            this.rbCountries.TabStop = true;
            this.rbCountries.Text = "Countries";
            this.rbCountries.UseVisualStyleBackColor = true;
            this.rbCountries.CheckedChanged += new System.EventHandler(this.rbCountries_CheckedChanged);
            // 
            // listCountries
            // 
            this.listCountries.Location = new System.Drawing.Point(241, 23);
            this.listCountries.Name = "listCountries";
            this.listCountries.Size = new System.Drawing.Size(26, 10);
            this.listCountries.TabIndex = 10;
            this.listCountries.UseCompatibleStateImageBehavior = false;
            // 
            // GBDRollback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1567, 749);
            this.Controls.Add(this.gbName);
            this.Controls.Add(this.gbOptionsStandard);
            this.Controls.Add(this.gbOptionsPercentage);
            this.Controls.Add(this.gbParameterSelection);
            this.Controls.Add(this.gbMap);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gbRollbacks);
            this.Controls.Add(this.gbCountrySelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "GBDRollback";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GBD Rollback Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GBDRollback_FormClosing);
            this.gbCountrySelection.ResumeLayout(false);
            this.gbCountrySelection.PerformLayout();
            this.gbRollbacks.ResumeLayout(false);
            this.gbRollbacks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRollbacks)).EndInit();
            this.gbMap.ResumeLayout(false);
            this.gbMap.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.gbParameterSelection.ResumeLayout(false);
            this.gbParameterSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_incremental)).EndInit();
            this.gbOptionsIncremental.ResumeLayout(false);
            this.gbOptionsIncremental.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_standard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_percent)).EndInit();
            this.gbOptionsPercentage.ResumeLayout(false);
            this.gbOptionsPercentage.PerformLayout();
            this.gbOptionsStandard.ResumeLayout(false);
            this.gbOptionsStandard.PerformLayout();
            this.gbName.ResumeLayout(false);
            this.gbName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCountrySelection;
        private System.Windows.Forms.GroupBox gbRollbacks;
        private System.Windows.Forms.DataGridView dgvRollbacks;
        private System.Windows.Forms.TreeView tvRegions;
        private System.Windows.Forms.Button btnNext2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbMap;
        private DotSpatial.Controls.Map mapGBD;
        private System.Windows.Forms.GroupBox gbParameterSelection;
        private System.Windows.Forms.Button btnBack2;
        private System.Windows.Forms.Button btnSaveRollback;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboRollbackType;
        private System.Windows.Forms.GroupBox gbOptionsIncremental;
        private System.Windows.Forms.TextBox txtIncrementBackground;
        private System.Windows.Forms.TextBox txtIncrement;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbOptionsPercentage;
        private System.Windows.Forms.TextBox txtPercentageBackground;
        private System.Windows.Forms.TextBox txtPercentage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbOptionsStandard;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboStandard;
        private System.Windows.Forms.GroupBox gbName;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnDeleteRollback;
        private System.Windows.Forms.Button btnEditRollback;
        private System.Windows.Forms.Button btnExecuteRollbacks;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnPan;
        private System.Windows.Forms.ToolStripButton btnFullExtent;
        private System.Windows.Forms.ToolStripButton btnSpatial;
        private System.Windows.Forms.ToolStripButton btnIdentify;
        private System.Windows.Forms.ToolStripButton btnLayerSet;
        private System.Windows.Forms.ToolStripButton btnPieTheme;
        private System.Windows.Forms.ToolStripButton btnColumnTheme;
        private System.Windows.Forms.ToolStripButton tsbSaveMap;
        private System.Windows.Forms.ToolStripButton tsbSavePic;
        private System.Windows.Forms.ToolStripButton tsbChangeProjection;
        private System.Windows.Forms.ToolStripButton tsbChangeCone;
        private System.Windows.Forms.ToolStripButton tsbAddLayer;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.PictureBox pb_incremental;
        private System.Windows.Forms.PictureBox pb_percent;
        private System.Windows.Forms.PictureBox pb_standard;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalCountries;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalPopulation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRollbackType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colExecute;
        private System.Windows.Forms.RadioButton rbCountries;
        private System.Windows.Forms.RadioButton rbRegions;
        private System.Windows.Forms.ListView listCountries;
    }
}