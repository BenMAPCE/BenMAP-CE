namespace BenMAP
{
    partial class MonitorRollbackSettings2
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnPan = new System.Windows.Forms.ToolStripButton();
            this.btnFullExtent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.mainMap = new DotSpatial.Controls.Map();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chbExportAfterRollback = new System.Windows.Forms.CheckBox();
            this.cboDeleteRegion = new System.Windows.Forms.ComboBox();
            this.btnDeleteRegion = new System.Windows.Forms.Button();
            this.btnAddRegion = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
                                                this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnBack);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Location = new System.Drawing.Point(12, 465);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(884, 66);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
                                                this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(713, 23);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 27);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
                                                this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(794, 23);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 27);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
                                                this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.toolStrip1);
            this.groupBox3.Controls.Add(this.mainMap);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(235, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(661, 450);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
                                                this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomIn,
            this.btnZoomOut,
            this.btnPan,
            this.btnFullExtent,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 11);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(127, 25);
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
                                                this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::BenMAP.Properties.Resources.point;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Select";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
                                                this.mainMap.AllowDrop = true;
            this.mainMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mainMap.BackColor = System.Drawing.Color.White;
            this.mainMap.CollectAfterDraw = false;
            this.mainMap.CollisionDetection = false;
            this.mainMap.ExtendBuffer = false;
            this.mainMap.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.mainMap.IsBusy = false;
            this.mainMap.Legend = null;
            this.mainMap.Location = new System.Drawing.Point(3, 11);
            this.mainMap.Name = "mainMap";
            this.mainMap.ProgressHandler = null;
            this.mainMap.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.RedrawLayersWhileResizing = false;
            this.mainMap.SelectionEnabled = true;
            this.mainMap.Size = new System.Drawing.Size(652, 374);
            this.mainMap.TabIndex = 1;
            this.mainMap.Tag = "GIS map to be created here";
            this.mainMap.SelectionChanged += new System.EventHandler(this.mainMap_SelectionChanged);
            this.mainMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mainMap_MouseClick);
                                                this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.chbExportAfterRollback);
            this.groupBox4.Controls.Add(this.cboDeleteRegion);
            this.groupBox4.Controls.Add(this.btnDeleteRegion);
            this.groupBox4.Controls.Add(this.btnAddRegion);
            this.groupBox4.Location = new System.Drawing.Point(6, 384);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(649, 59);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
                                                this.chbExportAfterRollback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chbExportAfterRollback.AutoSize = true;
            this.chbExportAfterRollback.Location = new System.Drawing.Point(471, 27);
            this.chbExportAfterRollback.Name = "chbExportAfterRollback";
            this.chbExportAfterRollback.Size = new System.Drawing.Size(150, 16);
            this.chbExportAfterRollback.TabIndex = 3;
            this.chbExportAfterRollback.Text = "Export After Rollback";
            this.chbExportAfterRollback.UseVisualStyleBackColor = false;
                                                this.cboDeleteRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDeleteRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDeleteRegion.FormattingEnabled = true;
            this.cboDeleteRegion.Location = new System.Drawing.Point(340, 23);
            this.cboDeleteRegion.Name = "cboDeleteRegion";
            this.cboDeleteRegion.Size = new System.Drawing.Size(121, 22);
            this.cboDeleteRegion.TabIndex = 2;
            this.cboDeleteRegion.SelectedIndexChanged += new System.EventHandler(this.cboDeleteRegion_SelectedIndexChanged);
                                                this.btnDeleteRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRegion.Location = new System.Drawing.Point(225, 21);
            this.btnDeleteRegion.Name = "btnDeleteRegion";
            this.btnDeleteRegion.Size = new System.Drawing.Size(100, 27);
            this.btnDeleteRegion.TabIndex = 1;
            this.btnDeleteRegion.Text = "Delete Region";
            this.btnDeleteRegion.UseVisualStyleBackColor = true;
            this.btnDeleteRegion.Click += new System.EventHandler(this.btnDeleteRegion_Click);
                                                this.btnAddRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddRegion.Location = new System.Drawing.Point(122, 21);
            this.btnAddRegion.Name = "btnAddRegion";
            this.btnAddRegion.Size = new System.Drawing.Size(88, 27);
            this.btnAddRegion.TabIndex = 0;
            this.btnAddRegion.Text = "Add Region";
            this.btnAddRegion.UseVisualStyleBackColor = true;
            this.btnAddRegion.Click += new System.EventHandler(this.btnAddRegion_Click);
                                                this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.flowLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(12, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(217, 450);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rollback Regions";
                                                this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.btnSelectAll);
            this.groupBox5.Controls.Add(this.btnDeleteAll);
            this.groupBox5.Location = new System.Drawing.Point(12, 383);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(199, 61);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
                                                this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAll.Location = new System.Drawing.Point(16, 21);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 27);
            this.btnSelectAll.TabIndex = 1;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
                                                this.btnDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteAll.Location = new System.Drawing.Point(97, 21);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(88, 27);
            this.btnDeleteAll.TabIndex = 2;
            this.btnDeleteAll.Text = "Deselect All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
                                                this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 23);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(205, 352);
            this.flowLayoutPanel1.TabIndex = 1;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 540);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.MinimumSize = new System.Drawing.Size(924, 578);
            this.Name = "MonitorRollbackSettings2";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor Rollback Settings:(2) Select Rollback Regions and Settings";
            this.Load += new System.EventHandler(this.MonitorRollbackSettings2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chbExportAfterRollback;
        private System.Windows.Forms.ComboBox cboDeleteRegion;
        private System.Windows.Forms.Button btnDeleteRegion;
        private System.Windows.Forms.Button btnAddRegion;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.GroupBox groupBox1;
        private DataSource.PercentageControl percentageControl1;
        private DataSource.IncrementalControl incrementalControl1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DotSpatial.Controls.Map mainMap;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnPan;
        private System.Windows.Forms.ToolStripButton btnFullExtent;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}