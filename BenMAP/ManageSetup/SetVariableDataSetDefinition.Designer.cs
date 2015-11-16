namespace BenMAP
{
    partial class VariableDataSetDefinition
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.olvData = new BrightIdeasSoftware.DataListView();
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.lblLoadedValue = new System.Windows.Forms.Label();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.lblProgressBar = new System.Windows.Forms.Label();
            this.progBarVariable = new System.Windows.Forms.ProgressBar();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpDataSetDetail = new System.Windows.Forms.GroupBox();
            this.txtGridDefinition = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.lblDataSetVariables = new System.Windows.Forms.Label();
            this.lstDataSetVariable = new System.Windows.Forms.ListBox();
            this.txtDataSetName = new System.Windows.Forms.TextBox();
            this.lblGridDefinition = new System.Windows.Forms.Label();
            this.lblDataSetName = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvData)).BeginInit();
            this.grpCancelOK.SuspendLayout();
            this.grpDataSetDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOutput
            // 
            this.btnOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOutput.Location = new System.Drawing.Point(9, 352);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(122, 27);
            this.btnOutput.TabIndex = 12;
            this.btnOutput.Text = "Output Sample File";
            this.toolTip1.SetToolTip(this.btnOutput, "Click to save a template in .csv format. It contains 50 rows data of sample data " +
        "and can be used as an example to prepare the input file.");
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.olvData);
            this.groupBox1.Controls.Add(this.lblLoadedValue);
            this.groupBox1.Location = new System.Drawing.Point(289, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(298, 423);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // olvData
            // 
            this.olvData.AllColumns.Add(this.olvColumn7);
            this.olvData.AllColumns.Add(this.olvColumn8);
            this.olvData.AllColumns.Add(this.olvColumn9);
            this.olvData.AllowColumnReorder = true;
            this.olvData.AllowDrop = true;
            this.olvData.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn7,
            this.olvColumn8,
            this.olvColumn9});
            this.olvData.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvData.DataSource = null;
            this.olvData.EmptyListMsg = "Add rows to the above table to see them here";
            this.olvData.EmptyListMsgFont = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvData.FullRowSelect = true;
            this.olvData.GridLines = true;
            this.olvData.GroupWithItemCountFormat = "{0} ({1} people)";
            this.olvData.GroupWithItemCountSingularFormat = "{0} (1 person)";
            this.olvData.HeaderUsesThemes = false;
            this.olvData.HideSelection = false;
            this.olvData.Location = new System.Drawing.Point(9, 34);
            this.olvData.Name = "olvData";
            this.olvData.OwnerDraw = true;
            this.olvData.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvData.ShowCommandMenuOnRightClick = true;
            this.olvData.ShowGroups = false;
            this.olvData.ShowImagesOnSubItems = true;
            this.olvData.ShowItemCountOnGroups = true;
            this.olvData.ShowItemToolTips = true;
            this.olvData.Size = new System.Drawing.Size(281, 382);
            this.olvData.TabIndex = 27;
            this.olvData.UseAlternatingBackColors = true;
            this.olvData.UseCellFormatEvents = true;
            this.olvData.UseCompatibleStateImageBehavior = false;
            this.olvData.UseFiltering = true;
            this.olvData.UseHotItem = true;
            this.olvData.UseOverlays = false;
            this.olvData.UseTranslucentHotItem = true;
            this.olvData.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "ccolumn";
            this.olvColumn7.Text = "Column";
            this.olvColumn7.Width = 100;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "row";
            this.olvColumn8.Text = "Row";
            this.olvColumn8.Width = 100;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "vvalue";
            this.olvColumn9.AspectToStringFormat = "{0}";
            this.olvColumn9.Text = "Value";
            this.olvColumn9.Width = 120;
            // 
            // lblLoadedValue
            // 
            this.lblLoadedValue.AutoSize = true;
            this.lblLoadedValue.Location = new System.Drawing.Point(6, 15);
            this.lblLoadedValue.Name = "lblLoadedValue";
            this.lblLoadedValue.Size = new System.Drawing.Size(84, 14);
            this.lblLoadedValue.TabIndex = 5;
            this.lblLoadedValue.Text = "Loaded Value:";
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.lblProgressBar);
            this.grpCancelOK.Controls.Add(this.progBarVariable);
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 430);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(575, 55);
            this.grpCancelOK.TabIndex = 7;
            this.grpCancelOK.TabStop = false;
            // 
            // lblProgressBar
            // 
            this.lblProgressBar.AutoSize = true;
            this.lblProgressBar.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressBar.ForeColor = System.Drawing.Color.Black;
            this.lblProgressBar.Location = new System.Drawing.Point(378, 27);
            this.lblProgressBar.Name = "lblProgressBar";
            this.lblProgressBar.Size = new System.Drawing.Size(0, 14);
            this.lblProgressBar.TabIndex = 3;
            // 
            // progBarVariable
            // 
            this.progBarVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progBarVariable.Location = new System.Drawing.Point(9, 28);
            this.progBarVariable.Name = "progBarVariable";
            this.progBarVariable.Size = new System.Drawing.Size(363, 12);
            this.progBarVariable.TabIndex = 2;
            this.progBarVariable.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(492, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(411, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpDataSetDetail
            // 
            this.grpDataSetDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpDataSetDetail.Controls.Add(this.btnOutput);
            this.grpDataSetDetail.Controls.Add(this.txtGridDefinition);
            this.grpDataSetDetail.Controls.Add(this.btnDelete);
            this.grpDataSetDetail.Controls.Add(this.btnLoadData);
            this.grpDataSetDetail.Controls.Add(this.lblDataSetVariables);
            this.grpDataSetDetail.Controls.Add(this.lstDataSetVariable);
            this.grpDataSetDetail.Controls.Add(this.txtDataSetName);
            this.grpDataSetDetail.Controls.Add(this.lblGridDefinition);
            this.grpDataSetDetail.Controls.Add(this.lblDataSetName);
            this.grpDataSetDetail.Location = new System.Drawing.Point(12, 2);
            this.grpDataSetDetail.Name = "grpDataSetDetail";
            this.grpDataSetDetail.Size = new System.Drawing.Size(271, 423);
            this.grpDataSetDetail.TabIndex = 2;
            this.grpDataSetDetail.TabStop = false;
            // 
            // txtGridDefinition
            // 
            this.txtGridDefinition.Location = new System.Drawing.Point(11, 76);
            this.txtGridDefinition.Name = "txtGridDefinition";
            this.txtGridDefinition.Size = new System.Drawing.Size(199, 22);
            this.txtGridDefinition.TabIndex = 10;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(9, 385);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(62, 27);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnLoadData
            // 
            this.btnLoadData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoadData.Location = new System.Drawing.Point(134, 352);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(131, 27);
            this.btnLoadData.TabIndex = 7;
            this.btnLoadData.Text = "Load From File";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // lblDataSetVariables
            // 
            this.lblDataSetVariables.AutoSize = true;
            this.lblDataSetVariables.Location = new System.Drawing.Point(8, 101);
            this.lblDataSetVariables.Name = "lblDataSetVariables";
            this.lblDataSetVariables.Size = new System.Drawing.Size(108, 14);
            this.lblDataSetVariables.TabIndex = 4;
            this.lblDataSetVariables.Text = "Dataset Variables:";
            // 
            // lstDataSetVariable
            // 
            this.lstDataSetVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstDataSetVariable.FormattingEnabled = true;
            this.lstDataSetVariable.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.lstDataSetVariable.ItemHeight = 14;
            this.lstDataSetVariable.Location = new System.Drawing.Point(10, 118);
            this.lstDataSetVariable.Name = "lstDataSetVariable";
            this.lstDataSetVariable.Size = new System.Drawing.Size(255, 228);
            this.lstDataSetVariable.TabIndex = 5;
            this.lstDataSetVariable.SelectedValueChanged += new System.EventHandler(this.lstDataSetVariable_SelectedValueChanged);
            // 
            // txtDataSetName
            // 
            this.txtDataSetName.Location = new System.Drawing.Point(11, 34);
            this.txtDataSetName.Name = "txtDataSetName";
            this.txtDataSetName.Size = new System.Drawing.Size(199, 22);
            this.txtDataSetName.TabIndex = 2;
            // 
            // lblGridDefinition
            // 
            this.lblGridDefinition.AutoSize = true;
            this.lblGridDefinition.Location = new System.Drawing.Point(6, 62);
            this.lblGridDefinition.Name = "lblGridDefinition";
            this.lblGridDefinition.Size = new System.Drawing.Size(89, 14);
            this.lblGridDefinition.TabIndex = 1;
            this.lblGridDefinition.Text = "Grid Definition:";
            // 
            // lblDataSetName
            // 
            this.lblDataSetName.AutoSize = true;
            this.lblDataSetName.Location = new System.Drawing.Point(6, 15);
            this.lblDataSetName.Name = "lblDataSetName";
            this.lblDataSetName.Size = new System.Drawing.Size(88, 14);
            this.lblDataSetName.TabIndex = 0;
            this.lblDataSetName.Text = "Dataset Name:";
            // 
            // VariableDataSetDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(599, 494);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpDataSetDetail);
            this.Name = "VariableDataSetDefinition";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setup Variable Datasets Definition";
            this.Load += new System.EventHandler(this.VariableDataSetDefinition_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvData)).EndInit();
            this.grpCancelOK.ResumeLayout(false);
            this.grpCancelOK.PerformLayout();
            this.grpDataSetDetail.ResumeLayout(false);
            this.grpDataSetDetail.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label lblDataSetName;
        private System.Windows.Forms.Label lblGridDefinition;
        private System.Windows.Forms.GroupBox grpDataSetDetail;
        private System.Windows.Forms.Label lblDataSetVariables;
        private System.Windows.Forms.TextBox txtDataSetName;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.ListBox lstDataSetVariable;
        private System.Windows.Forms.Label lblLoadedValue;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtGridDefinition;
        private System.Windows.Forms.GroupBox groupBox1;
        private BrightIdeasSoftware.DataListView olvData;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ProgressBar progBarVariable;
        private System.Windows.Forms.Label lblProgressBar;
    }
}