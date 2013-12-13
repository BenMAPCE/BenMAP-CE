namespace BenMAP
{
    partial class ManageInflationDataSet
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
            this.grpAvailableDataSets = new System.Windows.Forms.GroupBox();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstAvailableDataSets = new System.Windows.Forms.ListBox();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpInflationDetail = new System.Windows.Forms.GroupBox();
            this.olvData = new BrightIdeasSoftware.DataListView();
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpAvailableDataSets.SuspendLayout();
            this.grpCancelOK.SuspendLayout();
            this.grpInflationDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvData)).BeginInit();
            this.SuspendLayout();
            // 
            // grpAvailableDataSets
            // 
            this.grpAvailableDataSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.grpAvailableDataSets.Controls.Add(this.btnOutput);
            this.grpAvailableDataSets.Controls.Add(this.btnDelete);
            this.grpAvailableDataSets.Controls.Add(this.btnAdd);
            this.grpAvailableDataSets.Controls.Add(this.lstAvailableDataSets);
            this.grpAvailableDataSets.Location = new System.Drawing.Point(12, 1);
            this.grpAvailableDataSets.Name = "grpAvailableDataSets";
            this.grpAvailableDataSets.Size = new System.Drawing.Size(173, 431);
            this.grpAvailableDataSets.TabIndex = 0;
            this.grpAvailableDataSets.TabStop = false;
            this.grpAvailableDataSets.Text = "Available Datasets";
            // 
            // btnOutput
            // 
            this.btnOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOutput.Location = new System.Drawing.Point(11, 369);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(156, 27);
            this.btnOutput.TabIndex = 8;
            this.btnOutput.Text = "Output Sample File";
            this.toolTip1.SetToolTip(this.btnOutput, "Click to save a template with standard .csv format. It only contains 50 rows data" +
                    " and can be used as an example to prepare the input file.");
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(11, 398);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(92, 398);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 27);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstAvailableDataSets
            // 
            this.lstAvailableDataSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstAvailableDataSets.FormattingEnabled = true;
            this.lstAvailableDataSets.HorizontalScrollbar = true;
            this.lstAvailableDataSets.ItemHeight = 14;
            this.lstAvailableDataSets.Location = new System.Drawing.Point(11, 23);
            this.lstAvailableDataSets.Name = "lstAvailableDataSets";
            this.lstAvailableDataSets.Size = new System.Drawing.Size(156, 340);
            this.lstAvailableDataSets.TabIndex = 1;
            this.lstAvailableDataSets.SelectedValueChanged += new System.EventHandler(this.lstAvailableDataSets_SelectedValueChanged);
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 432);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(661, 59);
            this.grpCancelOK.TabIndex = 2;
            this.grpCancelOK.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(568, 21);
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
            this.btnCancel.Location = new System.Drawing.Point(487, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpInflationDetail
            // 
            this.grpInflationDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpInflationDetail.Controls.Add(this.olvData);
            this.grpInflationDetail.Location = new System.Drawing.Point(191, 1);
            this.grpInflationDetail.Name = "grpInflationDetail";
            this.grpInflationDetail.Size = new System.Drawing.Size(484, 431);
            this.grpInflationDetail.TabIndex = 3;
            this.grpInflationDetail.TabStop = false;
            this.grpInflationDetail.Text = "Inflation Detail";
            // 
            // olvData
            // 
            this.olvData.AllColumns.Add(this.olvColumn7);
            this.olvData.AllColumns.Add(this.olvColumn8);
            this.olvData.AllColumns.Add(this.olvColumn9);
            this.olvData.AllColumns.Add(this.olvColumn1);
            this.olvData.AllowColumnReorder = true;
            this.olvData.AllowDrop = true;
            this.olvData.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.olvData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn7,
            this.olvColumn8,
            this.olvColumn9,
            this.olvColumn1});
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
            this.olvData.Location = new System.Drawing.Point(6, 23);
            this.olvData.Name = "olvData";
            this.olvData.OwnerDraw = true;
            this.olvData.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvData.ShowCommandMenuOnRightClick = true;
            this.olvData.ShowGroups = false;
            this.olvData.ShowImagesOnSubItems = true;
            this.olvData.ShowItemToolTips = true;
            this.olvData.Size = new System.Drawing.Size(476, 401);
            this.olvData.TabIndex = 28;
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
            this.olvColumn7.Text = "Year";
            this.olvColumn7.Width = 90;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "AllGoodsIndex";
            this.olvColumn8.AspectToStringFormat = "{0}";
            this.olvColumn8.Text = "All Goods Index";
            this.olvColumn8.Width = 131;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "MedicalCostIndex";
            this.olvColumn9.AspectToStringFormat = "{0}";
            this.olvColumn9.Text = "Medical Cost Index";
            this.olvColumn9.Width = 124;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "WageIndex";
            this.olvColumn1.AspectToStringFormat = "{0}";
            this.olvColumn1.Text = "Wage Index";
            this.olvColumn1.Width = 129;
            // 
            // ManageInflationDataSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 497);
            this.Controls.Add(this.grpInflationDetail);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpAvailableDataSets);
            this.MinimumSize = new System.Drawing.Size(697, 535);
            this.Name = "ManageInflationDataSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Inflation Datasets";
            this.Load += new System.EventHandler(this.ManageInflationDataSet_Load);
            this.grpAvailableDataSets.ResumeLayout(false);
            this.grpCancelOK.ResumeLayout(false);
            this.grpInflationDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpAvailableDataSets;
        private System.Windows.Forms.ListBox lstAvailableDataSets;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpInflationDetail;
        private BrightIdeasSoftware.DataListView olvData;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}