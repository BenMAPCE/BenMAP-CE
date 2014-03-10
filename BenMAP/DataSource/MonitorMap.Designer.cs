namespace BenMAP
{
    partial class MonitorMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorMap));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
                                                this.splitContainer2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
                                                this.splitContainer2.Panel1.Controls.Add(this.mapLegend);
                                                this.splitContainer2.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer2.Panel2.Controls.Add(this.mainMap);
            this.splitContainer2.Size = new System.Drawing.Size(667, 367);
            this.splitContainer2.SplitterDistance = 135;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 8;
                                                this.mapLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapLegend.BackColor = System.Drawing.Color.White;
            this.mapLegend.ControlRectangle = new System.Drawing.Rectangle(0, 0, 129, 363);
            this.mapLegend.DocumentRectangle = new System.Drawing.Rectangle(0, 0, 168, 284);
            this.mapLegend.HorizontalScrollEnabled = true;
            this.mapLegend.Indentation = 30;
            this.mapLegend.IsInitialized = false;
            this.mapLegend.Location = new System.Drawing.Point(-1, -1);
            this.mapLegend.MinimumSize = new System.Drawing.Size(5, 6);
            this.mapLegend.Name = "mapLegend";
            this.mapLegend.ProgressHandler = null;
            this.mapLegend.ResetOnResize = false;
            this.mapLegend.SelectionFontColor = System.Drawing.Color.Black;
            this.mapLegend.SelectionHighlight = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(238)))), ((int)(((byte)(252)))));
            this.mapLegend.Size = new System.Drawing.Size(129, 363);
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
            this.toolStrip1.Size = new System.Drawing.Size(525, 25);
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
            this.btnLayerSet.Click += new System.EventHandler(this.btnLayerSet_Click);
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
            this.tsbChangeCone.Click += new System.EventHandler(this.tsbChangeCone_Click);
                                                this.tsbAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddLayer.Image = global::BenMAP.Properties.Resources.add;
            this.tsbAddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddLayer.Name = "tsbAddLayer";
            this.tsbAddLayer.Size = new System.Drawing.Size(23, 22);
            this.tsbAddLayer.Text = "Add Layer";
            this.tsbAddLayer.Visible = false;
            this.tsbAddLayer.Click += new System.EventHandler(this.tsbAddLayer_Click);
                                                this.mainMap.AllowDrop = true;
            this.mainMap.BackColor = System.Drawing.Color.White;
            this.mainMap.CollectAfterDraw = false;
            this.mainMap.CollisionDetection = false;
            this.mainMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMap.ExtendBuffer = false;
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
            this.mainMap.Size = new System.Drawing.Size(525, 365);
            this.mainMap.TabIndex = 0;
            this.mainMap.Tag = "GIS map will be created here.";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 367);
            this.Controls.Add(this.splitContainer2);
            this.Name = "MonitorMap";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MonitorMap";
            this.Load += new System.EventHandler(this.MonitorMap_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private DotSpatial.Controls.Legend mapLegend;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private DotSpatial.Controls.Map mainMap;
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

    }
}