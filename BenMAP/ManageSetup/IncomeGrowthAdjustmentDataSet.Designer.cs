namespace BenMAP
{
    partial class frmIncomeGrowthAdjustmentDataSet
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
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpGrowthDetail = new System.Windows.Forms.GroupBox();
            this.olvData = new BrightIdeasSoftware.DataListView();
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.grpAvailableDataSets = new System.Windows.Forms.GroupBox();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstDataSetName = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpCancelOK.SuspendLayout();
            this.grpGrowthDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvData)).BeginInit();
            this.grpAvailableDataSets.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 428);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(563, 53);
            this.grpCancelOK.TabIndex = 2;
            this.grpCancelOK.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(480, 16);
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
            this.btnCancel.Location = new System.Drawing.Point(399, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpGrowthDetail
            // 
            this.grpGrowthDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpGrowthDetail.Controls.Add(this.olvData);
            this.grpGrowthDetail.Location = new System.Drawing.Point(203, 7);
            this.grpGrowthDetail.Name = "grpGrowthDetail";
            this.grpGrowthDetail.Size = new System.Drawing.Size(372, 415);
            this.grpGrowthDetail.TabIndex = 1;
            this.grpGrowthDetail.TabStop = false;
            this.grpGrowthDetail.Text = "Income Growth Detail";
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
            this.olvData.Location = new System.Drawing.Point(6, 17);
            this.olvData.Name = "olvData";
            this.olvData.OwnerDraw = true;
            this.olvData.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvData.ShowCommandMenuOnRightClick = true;
            this.olvData.ShowGroups = false;
            this.olvData.ShowImagesOnSubItems = true;
            this.olvData.ShowItemCountOnGroups = true;
            this.olvData.ShowItemToolTips = true;
            this.olvData.Size = new System.Drawing.Size(358, 385);
            this.olvData.TabIndex = 27;
            this.olvData.UseAlternatingBackColors = true;
            this.olvData.UseCellFormatEvents = true;
            this.olvData.UseCompatibleStateImageBehavior = false;
            this.olvData.UseFiltering = true;
            this.olvData.UseHotItem = true;
            this.olvData.UseTranslucentHotItem = true;
            this.olvData.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "yyear";
            this.olvColumn7.AspectToStringFormat = "{0}";
            this.olvColumn7.Text = "Year";
            this.olvColumn7.Width = 120;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "Mean";
            this.olvColumn8.Text = "Factor";
            this.olvColumn8.Width = 100;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "EndpointGroups";
            this.olvColumn9.Text = "Endpoint Groups";
            this.olvColumn9.Width = 120;
            // 
            // grpAvailableDataSets
            // 
            this.grpAvailableDataSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.grpAvailableDataSets.Controls.Add(this.btnOutput);
            this.grpAvailableDataSets.Controls.Add(this.btnDelete);
            this.grpAvailableDataSets.Controls.Add(this.btnAdd);
            this.grpAvailableDataSets.Controls.Add(this.lstDataSetName);
            this.grpAvailableDataSets.Location = new System.Drawing.Point(13, 7);
            this.grpAvailableDataSets.Name = "grpAvailableDataSets";
            this.grpAvailableDataSets.Size = new System.Drawing.Size(184, 415);
            this.grpAvailableDataSets.TabIndex = 0;
            this.grpAvailableDataSets.TabStop = false;
            this.grpAvailableDataSets.Text = "Available Datasets";
            // 
            // btnOutput
            // 
            this.btnOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOutput.Location = new System.Drawing.Point(7, 350);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(159, 27);
            this.btnOutput.TabIndex = 3;
            this.btnOutput.Text = "Output Sample File";
            this.toolTip1.SetToolTip(this.btnOutput, "Click to save a template with standard .csv format. It only contains 50 rows data" +
                    " and can be used as an example to prepare the input file.");
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(7, 382);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(73, 27);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(95, 382);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(71, 27);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstDataSetName
            // 
            this.lstDataSetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstDataSetName.FormattingEnabled = true;
            this.lstDataSetName.ItemHeight = 14;
            this.lstDataSetName.Location = new System.Drawing.Point(7, 17);
            this.lstDataSetName.Name = "lstDataSetName";
            this.lstDataSetName.Size = new System.Drawing.Size(170, 326);
            this.lstDataSetName.TabIndex = 0;
            this.lstDataSetName.SelectedValueChanged += new System.EventHandler(this.lstDataSetName_SelectedValueChanged);
            // 
            // frmIncomeGrowthAdjustmentDataSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 493);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpGrowthDetail);
            this.Controls.Add(this.grpAvailableDataSets);
            this.MinimumSize = new System.Drawing.Size(596, 531);
            this.Name = "frmIncomeGrowthAdjustmentDataSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Income Growth Adjustment Dataset Manager";
            this.Load += new System.EventHandler(this.IncomeGrowthAdjustmentDataSet_Load);
            this.grpCancelOK.ResumeLayout(false);
            this.grpGrowthDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvData)).EndInit();
            this.grpAvailableDataSets.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpAvailableDataSets;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstDataSetName;
        private System.Windows.Forms.GroupBox grpGrowthDetail;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button btnOK;
        private BrightIdeasSoftware.DataListView olvData;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}