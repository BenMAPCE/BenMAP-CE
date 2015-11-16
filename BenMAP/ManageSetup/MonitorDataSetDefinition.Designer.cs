namespace BenMAP
{
    partial class MonitorDataSetDefinition
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
            this.btnOutput = new System.Windows.Forms.Button();
            this.grpMonitorDataSetDefinition = new System.Windows.Forms.GroupBox();
            this.olvMonitorDataSets = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.grpFileSource = new System.Windows.Forms.GroupBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbpTextFile = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtMonitorDataFile = new System.Windows.Forms.TextBox();
            this.lblMonitorDataFile3 = new System.Windows.Forms.Label();
            this.txtDataSetName = new System.Windows.Forms.TextBox();
            this.lblDataSetName = new System.Windows.Forms.Label();
            this.lblDataSetContents = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowse1 = new System.Windows.Forms.Button();
            this.cboPollutant = new System.Windows.Forms.ComboBox();
            this.lblPollutant = new System.Windows.Forms.Label();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.lblYear = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.grpMonitorDataSetDefinition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvMonitorDataSets)).BeginInit();
            this.grpFileSource.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbpTextFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(10, 48);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(168, 27);
            this.btnOutput.TabIndex = 6;
            this.btnOutput.Text = "Output Sample File";
            this.toolTip1.SetToolTip(this.btnOutput, "Click to save a template with standard .csv format. It only contains 50 rows data" +
        " and can be used as an example to prepare the input file.");
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // grpMonitorDataSetDefinition
            // 
            this.grpMonitorDataSetDefinition.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.grpMonitorDataSetDefinition.AutoSize = true;
            this.grpMonitorDataSetDefinition.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpMonitorDataSetDefinition.Controls.Add(this.olvMonitorDataSets);
            this.grpMonitorDataSetDefinition.Controls.Add(this.grpFileSource);
            this.grpMonitorDataSetDefinition.Controls.Add(this.txtDataSetName);
            this.grpMonitorDataSetDefinition.Controls.Add(this.lblDataSetName);
            this.grpMonitorDataSetDefinition.Controls.Add(this.lblDataSetContents);
            this.grpMonitorDataSetDefinition.Location = new System.Drawing.Point(4, 48);
            this.grpMonitorDataSetDefinition.Name = "grpMonitorDataSetDefinition";
            this.grpMonitorDataSetDefinition.Size = new System.Drawing.Size(412, 506);
            this.grpMonitorDataSetDefinition.TabIndex = 0;
            this.grpMonitorDataSetDefinition.TabStop = false;
            // 
            // olvMonitorDataSets
            // 
            this.olvMonitorDataSets.AllColumns.Add(this.olvColumn1);
            this.olvMonitorDataSets.AllColumns.Add(this.olvColumn2);
            this.olvMonitorDataSets.AllColumns.Add(this.olvColumn9);
            this.olvMonitorDataSets.AllowColumnReorder = true;
            this.olvMonitorDataSets.AllowDrop = true;
            this.olvMonitorDataSets.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvMonitorDataSets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn9});
            this.olvMonitorDataSets.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMonitorDataSets.DataSource = null;
            this.olvMonitorDataSets.EmptyListMsg = "This list is empty.";
            this.olvMonitorDataSets.FullRowSelect = true;
            this.olvMonitorDataSets.GridLines = true;
            this.olvMonitorDataSets.GroupWithItemCountFormat = "";
            this.olvMonitorDataSets.GroupWithItemCountSingularFormat = "";
            this.olvMonitorDataSets.HeaderUsesThemes = false;
            this.olvMonitorDataSets.HideSelection = false;
            this.olvMonitorDataSets.Location = new System.Drawing.Point(9, 78);
            this.olvMonitorDataSets.MultiSelect = false;
            this.olvMonitorDataSets.Name = "olvMonitorDataSets";
            this.olvMonitorDataSets.OwnerDraw = true;
            this.olvMonitorDataSets.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMonitorDataSets.ShowCommandMenuOnRightClick = true;
            this.olvMonitorDataSets.ShowGroups = false;
            this.olvMonitorDataSets.ShowImagesOnSubItems = true;
            this.olvMonitorDataSets.ShowItemToolTips = true;
            this.olvMonitorDataSets.Size = new System.Drawing.Size(397, 377);
            this.olvMonitorDataSets.TabIndex = 27;
            this.olvMonitorDataSets.UseAlternatingBackColors = true;
            this.olvMonitorDataSets.UseCompatibleStateImageBehavior = false;
            this.olvMonitorDataSets.UseFiltering = true;
            this.olvMonitorDataSets.UseHotItem = true;
            this.olvMonitorDataSets.UseHyperlinks = true;
            this.olvMonitorDataSets.UseOverlays = false;
            this.olvMonitorDataSets.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "pollutantname";
            this.olvColumn1.Text = "Pollutant";
            this.olvColumn1.Width = 137;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "yyear";
            this.olvColumn2.Text = "Year";
            this.olvColumn2.Width = 97;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "Count";
            this.olvColumn9.FillsFreeSpace = true;
            this.olvColumn9.Text = "Monitor Count";
            this.olvColumn9.Width = 109;
            // 
            // grpFileSource
            // 
            this.grpFileSource.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.grpFileSource.Controls.Add(this.btnLoad);
            this.grpFileSource.Controls.Add(this.tabControl1);
            this.grpFileSource.Location = new System.Drawing.Point(499, 49);
            this.grpFileSource.Name = "grpFileSource";
            this.grpFileSource.Size = new System.Drawing.Size(25, 436);
            this.grpFileSource.TabIndex = 6;
            this.grpFileSource.TabStop = false;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(-414, 531);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(128, 27);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Import To Database";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Visible = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tbpTextFile);
            this.tabControl1.Location = new System.Drawing.Point(-302, 56);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(321, 335);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.Visible = false;
            // 
            // tbpTextFile
            // 
            this.tbpTextFile.Controls.Add(this.label1);
            this.tbpTextFile.Controls.Add(this.btnBrowse);
            this.tbpTextFile.Controls.Add(this.txtMonitorDataFile);
            this.tbpTextFile.Controls.Add(this.lblMonitorDataFile3);
            this.tbpTextFile.Location = new System.Drawing.Point(4, 23);
            this.tbpTextFile.Name = "tbpTextFile";
            this.tbpTextFile.Padding = new System.Windows.Forms.Padding(3);
            this.tbpTextFile.Size = new System.Drawing.Size(313, 308);
            this.tbpTextFile.TabIndex = 2;
            this.tbpTextFile.Text = "Monitor File";
            this.tbpTextFile.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(297, 63);
            this.label1.TabIndex = 3;
            this.label1.Text = "The file layout is: Column, Row, Metric, Seasonal Metric, Statistic, Values; Valu" +
    "es is a string of comma delimited model values.";
            this.label1.Visible = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowse.Location = new System.Drawing.Point(257, 45);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(49, 27);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Visible = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click_1);
            // 
            // txtMonitorDataFile
            // 
            this.txtMonitorDataFile.Location = new System.Drawing.Point(8, 45);
            this.txtMonitorDataFile.Name = "txtMonitorDataFile";
            this.txtMonitorDataFile.ReadOnly = true;
            this.txtMonitorDataFile.Size = new System.Drawing.Size(243, 22);
            this.txtMonitorDataFile.TabIndex = 1;
            this.txtMonitorDataFile.Visible = false;
            // 
            // lblMonitorDataFile3
            // 
            this.lblMonitorDataFile3.AutoSize = true;
            this.lblMonitorDataFile3.Location = new System.Drawing.Point(6, 28);
            this.lblMonitorDataFile3.Name = "lblMonitorDataFile3";
            this.lblMonitorDataFile3.Size = new System.Drawing.Size(106, 14);
            this.lblMonitorDataFile3.TabIndex = 0;
            this.lblMonitorDataFile3.Text = "Monitor Data File:";
            this.lblMonitorDataFile3.Visible = false;
            // 
            // txtDataSetName
            // 
            this.txtDataSetName.Location = new System.Drawing.Point(109, 15);
            this.txtDataSetName.Name = "txtDataSetName";
            this.txtDataSetName.Size = new System.Drawing.Size(258, 22);
            this.txtDataSetName.TabIndex = 5;
            // 
            // lblDataSetName
            // 
            this.lblDataSetName.AutoSize = true;
            this.lblDataSetName.Location = new System.Drawing.Point(8, 18);
            this.lblDataSetName.Name = "lblDataSetName";
            this.lblDataSetName.Size = new System.Drawing.Size(88, 14);
            this.lblDataSetName.TabIndex = 4;
            this.lblDataSetName.Text = "Dataset Name:";
            // 
            // lblDataSetContents
            // 
            this.lblDataSetContents.AutoSize = true;
            this.lblDataSetContents.Location = new System.Drawing.Point(7, 45);
            this.lblDataSetContents.Name = "lblDataSetContents";
            this.lblDataSetContents.Size = new System.Drawing.Size(327, 14);
            this.lblDataSetContents.TabIndex = 3;
            this.lblDataSetContents.Text = "Dataset Contents (Number of Monitor by Pollutant by Year):";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.btnBrowse1);
            this.groupBox1.Controls.Add(this.btnOutput);
            this.groupBox1.Controls.Add(this.cboPollutant);
            this.groupBox1.Controls.Add(this.lblPollutant);
            this.groupBox1.Controls.Add(this.txtYear);
            this.groupBox1.Controls.Add(this.lblYear);
            this.groupBox1.Location = new System.Drawing.Point(4, 509);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(413, 96);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // btnBrowse1
            // 
            this.btnBrowse1.Location = new System.Drawing.Point(266, 48);
            this.btnBrowse1.Name = "btnBrowse1";
            this.btnBrowse1.Size = new System.Drawing.Size(141, 27);
            this.btnBrowse1.TabIndex = 7;
            this.btnBrowse1.Text = "Load Data From File";
            this.btnBrowse1.UseVisualStyleBackColor = true;
            this.btnBrowse1.Click += new System.EventHandler(this.btnBrowse1_Click);
            // 
            // cboPollutant
            // 
            this.cboPollutant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPollutant.FormattingEnabled = true;
            this.cboPollutant.Location = new System.Drawing.Point(78, 20);
            this.cboPollutant.Name = "cboPollutant";
            this.cboPollutant.Size = new System.Drawing.Size(100, 22);
            this.cboPollutant.TabIndex = 1;
            this.cboPollutant.SelectedIndexChanged += new System.EventHandler(this.cboPollutant_SelectedIndexChanged);
            // 
            // lblPollutant
            // 
            this.lblPollutant.AutoSize = true;
            this.lblPollutant.Location = new System.Drawing.Point(7, 24);
            this.lblPollutant.Name = "lblPollutant";
            this.lblPollutant.Size = new System.Drawing.Size(60, 14);
            this.lblPollutant.TabIndex = 0;
            this.lblPollutant.Text = "Pollutant:";
            // 
            // txtYear
            // 
            this.txtYear.Location = new System.Drawing.Point(266, 20);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(141, 22);
            this.txtYear.TabIndex = 3;
            this.txtYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtYear_KeyPress);
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(227, 24);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(33, 14);
            this.lblYear.TabIndex = 2;
            this.lblYear.Text = "Year:";
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Location = new System.Drawing.Point(282, 652);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 14);
            this.lblProgress.TabIndex = 4;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(13, 654);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(224, 12);
            this.progressBar1.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(328, 646);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(328, 613);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(247, 646);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // MonitorDataSetDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(1, 1);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(427, 680);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpMonitorDataSetDefinition);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Name = "MonitorDataSetDefinition";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor Dataset Definition";
            this.Load += new System.EventHandler(this.MonitorDataSetDefinition_Load);
            this.grpMonitorDataSetDefinition.ResumeLayout(false);
            this.grpMonitorDataSetDefinition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvMonitorDataSets)).EndInit();
            this.grpFileSource.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tbpTextFile.ResumeLayout(false);
            this.tbpTextFile.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.GroupBox grpMonitorDataSetDefinition;
        private System.Windows.Forms.Label lblDataSetContents;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtDataSetName;
        private System.Windows.Forms.Label lblDataSetName;
        private System.Windows.Forms.GroupBox grpFileSource;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbpTextFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtMonitorDataFile;
        private System.Windows.Forms.Label lblMonitorDataFile3;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.ComboBox cboPollutant;
        private System.Windows.Forms.Label lblPollutant;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ProgressBar progressBar1;
        public BrightIdeasSoftware.DataListView olvMonitorDataSets;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBrowse1;
        private System.Windows.Forms.Button btnDelete;
    }
}