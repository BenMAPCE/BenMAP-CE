namespace BenMAP
{
    partial class PopulationMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopulationMap));
            this.cboAgeRange = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDraw = new System.Windows.Forms.Button();
            this.lblPopulationYear = new System.Windows.Forms.Label();
            this.cboPopulationYear = new System.Windows.Forms.ComboBox();
            this.lblPopulationDataSet = new System.Windows.Forms.Label();
            this.cboPopulationDataSet = new System.Windows.Forms.ComboBox();
            this.tspPOPMap = new System.Windows.Forms.StatusStrip();
            this.prgLoadPOP = new System.Windows.Forms.ToolStripProgressBar();
            this.lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.colorBlend = new WinControls.ColorBlendControl();
            this.mapLegend = new DotSpatial.Controls.Legend();
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
            this.mainMap = new DotSpatial.Controls.Map();
            this.groupBox1.SuspendLayout();
            this.tspPOPMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
                                                this.cboAgeRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAgeRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAgeRange.FormattingEnabled = true;
            this.cboAgeRange.Location = new System.Drawing.Point(306, 64);
            this.cboAgeRange.Name = "cboAgeRange";
            this.cboAgeRange.Size = new System.Drawing.Size(374, 22);
            this.cboAgeRange.TabIndex = 15;
            this.cboAgeRange.SelectedIndexChanged += new System.EventHandler(this.cboAgeRange_SelectedIndexChanged);
                                                this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(285, 14);
            this.label1.TabIndex = 12;
            this.label1.Text = "Select Race, Gender, Ethnicity, AgeRange to display:";
                                                this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnDraw);
            this.groupBox1.Controls.Add(this.lblPopulationYear);
            this.groupBox1.Controls.Add(this.cboPopulationYear);
            this.groupBox1.Controls.Add(this.lblPopulationDataSet);
            this.groupBox1.Controls.Add(this.cboPopulationDataSet);
            this.groupBox1.Location = new System.Drawing.Point(8, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(678, 53);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Population Dataset";
                                                this.btnDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDraw.Location = new System.Drawing.Point(620, 19);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(52, 24);
            this.btnDraw.TabIndex = 14;
            this.btnDraw.Text = "Draw";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
                                                this.lblPopulationYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPopulationYear.AutoSize = true;
            this.lblPopulationYear.Location = new System.Drawing.Point(371, 23);
            this.lblPopulationYear.Name = "lblPopulationYear";
            this.lblPopulationYear.Size = new System.Drawing.Size(95, 14);
            this.lblPopulationYear.TabIndex = 12;
            this.lblPopulationYear.Text = "Population Year:";
                                                this.cboPopulationYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPopulationYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPopulationYear.Location = new System.Drawing.Point(472, 20);
            this.cboPopulationYear.Name = "cboPopulationYear";
            this.cboPopulationYear.Size = new System.Drawing.Size(142, 22);
            this.cboPopulationYear.TabIndex = 13;
                                                this.lblPopulationDataSet.AutoSize = true;
            this.lblPopulationDataSet.Location = new System.Drawing.Point(7, 23);
            this.lblPopulationDataSet.Name = "lblPopulationDataSet";
            this.lblPopulationDataSet.Size = new System.Drawing.Size(115, 14);
            this.lblPopulationDataSet.TabIndex = 10;
            this.lblPopulationDataSet.Text = "Population Dataset:";
                                                this.cboPopulationDataSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPopulationDataSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPopulationDataSet.Location = new System.Drawing.Point(128, 20);
            this.cboPopulationDataSet.Name = "cboPopulationDataSet";
            this.cboPopulationDataSet.Size = new System.Drawing.Size(237, 22);
            this.cboPopulationDataSet.TabIndex = 11;
            this.cboPopulationDataSet.SelectedIndexChanged += new System.EventHandler(this.cboPopulationDataSet_SelectedIndexChanged);
                                                this.tspPOPMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prgLoadPOP,
            this.lblProgress});
            this.tspPOPMap.Location = new System.Drawing.Point(0, 580);
            this.tspPOPMap.Name = "tspPOPMap";
            this.tspPOPMap.Size = new System.Drawing.Size(694, 22);
            this.tspPOPMap.TabIndex = 17;
            this.tspPOPMap.Text = "statusStrip1";
                                                this.prgLoadPOP.Name = "prgLoadPOP";
            this.prgLoadPOP.Size = new System.Drawing.Size(450, 16);
            this.prgLoadPOP.Step = 1;
            this.prgLoadPOP.Visible = false;
                                                this.lblProgress.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 17);
            this.lblProgress.Visible = false;
                                                this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Font = new System.Drawing.Font("Calibri", 9F);
            this.splitContainer2.Location = new System.Drawing.Point(0, 95);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
                                                this.splitContainer2.Panel1.Controls.Add(this.colorBlend);
            this.splitContainer2.Panel1.Controls.Add(this.mapLegend);
                                                this.splitContainer2.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer2.Panel2.Controls.Add(this.mainMap);
            this.splitContainer2.Size = new System.Drawing.Size(694, 485);
            this.splitContainer2.SplitterDistance = 140;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 9;
                                                this.colorBlend.ColorArray = new System.Drawing.Color[] {
        System.Drawing.Color.Blue,
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Magenta,
        System.Drawing.Color.Red};
            this.colorBlend.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.colorBlend.Location = new System.Drawing.Point(0, 443);
            this.colorBlend.Margin = new System.Windows.Forms.Padding(1);
            this.colorBlend.Name = "colorBlend";
            this.colorBlend.Size = new System.Drawing.Size(138, 40);
            this.colorBlend.TabIndex = 2;
            this.colorBlend.ValueArray = new double[] {
        0D,
        0D,
        0D,
        0D,
        0D,
        0D};
                                                this.mapLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapLegend.BackColor = System.Drawing.Color.White;
            this.mapLegend.ControlRectangle = new System.Drawing.Rectangle(0, 0, 140, 425);
            this.mapLegend.DocumentRectangle = new System.Drawing.Rectangle(0, 0, 168, 284);
            this.mapLegend.Font = new System.Drawing.Font("Calibri", 9F);
            this.mapLegend.HorizontalScrollEnabled = true;
            this.mapLegend.Indentation = 30;
            this.mapLegend.IsInitialized = false;
            this.mapLegend.Location = new System.Drawing.Point(-1, -1);
            this.mapLegend.MinimumSize = new System.Drawing.Size(5, 7);
            this.mapLegend.Name = "mapLegend";
            this.mapLegend.ProgressHandler = null;
            this.mapLegend.ResetOnResize = false;
            this.mapLegend.SelectionFontColor = System.Drawing.Color.Black;
            this.mapLegend.SelectionHighlight = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(238)))), ((int)(((byte)(252)))));
            this.mapLegend.Size = new System.Drawing.Size(140, 425);
            this.mapLegend.TabIndex = 0;
            this.mapLegend.Text = "legend1";
            this.mapLegend.VerticalScrollEnabled = true;
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
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(547, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
                                                this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = global::BenMAP.Properties.Resources.magnifier_zoom_in;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoomIn.Text = "toolStripButton1";
            this.btnZoomIn.ToolTipText = "Zoom in";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
                                                this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = global::BenMAP.Properties.Resources.magnifier_zoom_out;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.btnZoomOut.Text = "toolStripButton2";
            this.btnZoomOut.ToolTipText = "Zoom out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
                                                this.btnPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPan.Image = global::BenMAP.Properties.Resources.pan_2;
            this.btnPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(23, 22);
            this.btnPan.Text = "toolStripButton3";
            this.btnPan.ToolTipText = "Pan";
            this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
                                                this.btnFullExtent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFullExtent.Image = global::BenMAP.Properties.Resources.globe_7;
            this.btnFullExtent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFullExtent.Name = "btnFullExtent";
            this.btnFullExtent.Size = new System.Drawing.Size(23, 22);
            this.btnFullExtent.Text = "toolStripButton4";
            this.btnFullExtent.ToolTipText = "Full extent";
            this.btnFullExtent.Click += new System.EventHandler(this.btnFullExtent_Click);
                                                this.btnSpatial.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpatial.Image = global::BenMAP.Properties.Resources.chart4;
            this.btnSpatial.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpatial.Name = "btnSpatial";
            this.btnSpatial.Size = new System.Drawing.Size(23, 22);
            this.btnSpatial.Text = "toolStripButton5";
            this.btnSpatial.ToolTipText = "Spatial analysis";
            this.btnSpatial.Visible = false;
                                                this.btnIdentify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnIdentify.Image = global::BenMAP.Properties.Resources.identifier_16;
            this.btnIdentify.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIdentify.Name = "btnIdentify";
            this.btnIdentify.Size = new System.Drawing.Size(23, 22);
            this.btnIdentify.Text = "Identify";
            this.btnIdentify.Click += new System.EventHandler(this.btnIdentify_Click);
                                                this.btnLayerSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLayerSet.Image = global::BenMAP.Properties.Resources.legend;
            this.btnLayerSet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLayerSet.Name = "btnLayerSet";
            this.btnLayerSet.Size = new System.Drawing.Size(23, 22);
            this.btnLayerSet.Text = "Show Legend";
            this.btnLayerSet.ToolTipText = "Show Legend";
            this.btnLayerSet.Visible = false;
                                                this.btnPieTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPieTheme.Image = global::BenMAP.Properties.Resources.tableView1;
            this.btnPieTheme.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPieTheme.Name = "btnPieTheme";
            this.btnPieTheme.Size = new System.Drawing.Size(23, 22);
            this.btnPieTheme.Text = "Pie Theme";
            this.btnPieTheme.Visible = false;
                                                this.btnColumnTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnColumnTheme.Image = global::BenMAP.Properties.Resources.tableView_Bar;
            this.btnColumnTheme.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnColumnTheme.Name = "btnColumnTheme";
            this.btnColumnTheme.Size = new System.Drawing.Size(23, 22);
            this.btnColumnTheme.Text = "Column Theme";
            this.btnColumnTheme.Visible = false;
                                                this.tsbSaveMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveMap.Image = global::BenMAP.Properties.Resources.save_as;
            this.tsbSaveMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveMap.Name = "tsbSaveMap";
            this.tsbSaveMap.Size = new System.Drawing.Size(23, 22);
            this.tsbSaveMap.Tag = "";
            this.tsbSaveMap.Text = "Save shapefile";
            this.tsbSaveMap.Click += new System.EventHandler(this.tsbSaveMap_Click);
                                                this.tsbSavePic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSavePic.Image = ((System.Drawing.Image)(resources.GetObject("tsbSavePic.Image")));
            this.tsbSavePic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSavePic.Name = "tsbSavePic";
            this.tsbSavePic.Size = new System.Drawing.Size(23, 22);
            this.tsbSavePic.Tag = "";
            this.tsbSavePic.Text = "Export map image";
            this.tsbSavePic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsbSavePic.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsbSavePic.Click += new System.EventHandler(this.tsbSavePic_Click);
                                                this.tsbChangeProjection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbChangeProjection.Image = global::BenMAP.Properties.Resources.cuahsi_logo1;
            this.tsbChangeProjection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbChangeProjection.Name = "tsbChangeProjection";
            this.tsbChangeProjection.Size = new System.Drawing.Size(23, 22);
            this.tsbChangeProjection.Text = "ChangeProjection";
            this.tsbChangeProjection.Click += new System.EventHandler(this.tsbChangeProjection_Click);
                                                this.tsbChangeCone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbChangeCone.Image = ((System.Drawing.Image)(resources.GetObject("tsbChangeCone.Image")));
            this.tsbChangeCone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbChangeCone.Name = "tsbChangeCone";
            this.tsbChangeCone.Size = new System.Drawing.Size(23, 22);
            this.tsbChangeCone.Text = "Change to square view";
            this.tsbChangeCone.Visible = false;
                                                this.tsbAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddLayer.Image = global::BenMAP.Properties.Resources.add;
            this.tsbAddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddLayer.Name = "tsbAddLayer";
            this.tsbAddLayer.Size = new System.Drawing.Size(23, 22);
            this.tsbAddLayer.Text = "Add Layer";
            this.tsbAddLayer.Visible = false;
                                                this.mainMap.AllowDrop = true;
            this.mainMap.BackColor = System.Drawing.Color.White;
            this.mainMap.CollectAfterDraw = false;
            this.mainMap.CollisionDetection = false;
            this.mainMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMap.ExtendBuffer = false;
            this.mainMap.Font = new System.Drawing.Font("Calibri", 9F);
            this.mainMap.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.mainMap.IsBusy = false;
            this.mainMap.Legend = this.mapLegend;
            this.mainMap.Location = new System.Drawing.Point(0, 0);
            this.mainMap.Name = "mainMap";
            this.mainMap.ProgressHandler = null;
            this.mainMap.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.RedrawLayersWhileResizing = false;
            this.mainMap.SelectionEnabled = true;
            this.mainMap.Size = new System.Drawing.Size(547, 483);
            this.mainMap.TabIndex = 0;
            this.mainMap.Tag = "GIS map will be created here.";
            this.mainMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mainMap_MouseUp);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 602);
            this.Controls.Add(this.cboAgeRange);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tspPOPMap);
            this.Controls.Add(this.splitContainer2);
            this.MinimumSize = new System.Drawing.Size(710, 640);
            this.Name = "PopulationMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Population Map";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PopulationMap_FormClosed);
            this.Load += new System.EventHandler(this.PopulationMap_Load);
            this.Shown += new System.EventHandler(this.PopulationMap_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tspPOPMap.ResumeLayout(false);
            this.tspPOPMap.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.SplitContainer splitContainer2;
        private DotSpatial.Controls.Legend mapLegend;
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
        private DotSpatial.Controls.Map mainMap;
        private WinControls.ColorBlendControl colorBlend;
        private System.Windows.Forms.Label lblPopulationDataSet;
        private System.Windows.Forms.ComboBox cboPopulationDataSet;
        private System.Windows.Forms.Label lblPopulationYear;
        private System.Windows.Forms.ComboBox cboPopulationYear;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.ComboBox cboAgeRange;
        private System.Windows.Forms.StatusStrip tspPOPMap;
        private System.Windows.Forms.ToolStripProgressBar prgLoadPOP;
        private System.Windows.Forms.ToolStripStatusLabel lblProgress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}