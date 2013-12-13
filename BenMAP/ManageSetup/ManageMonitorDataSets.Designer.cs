namespace BenMAP
{
    partial class ManageMonitorDataSets
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
            this.grpAvailableDataSets = new System.Windows.Forms.GroupBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstAvailableDataSets = new System.Windows.Forms.ListBox();
            this.grpDataSetContents = new System.Windows.Forms.GroupBox();
            this.olvMonitorDataSets = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.grpAvailableDataSets.SuspendLayout();
            this.grpDataSetContents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvMonitorDataSets)).BeginInit();
            this.grpCancelOK.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpAvailableDataSets
            // 
            this.grpAvailableDataSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.grpAvailableDataSets.Controls.Add(this.btnEdit);
            this.grpAvailableDataSets.Controls.Add(this.btnAdd);
            this.grpAvailableDataSets.Controls.Add(this.btnDelete);
            this.grpAvailableDataSets.Controls.Add(this.lstAvailableDataSets);
            this.grpAvailableDataSets.Location = new System.Drawing.Point(13, 8);
            this.grpAvailableDataSets.Name = "grpAvailableDataSets";
            this.grpAvailableDataSets.Size = new System.Drawing.Size(225, 447);
            this.grpAvailableDataSets.TabIndex = 0;
            this.grpAvailableDataSets.TabStop = false;
            this.grpAvailableDataSets.Text = "Available Datasets";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnEdit.Location = new System.Drawing.Point(160, 413);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(59, 27);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAdd.Location = new System.Drawing.Point(95, 413);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(59, 27);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnDelete.Location = new System.Drawing.Point(30, 413);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(59, 27);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstAvailableDataSets
            // 
            this.lstAvailableDataSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstAvailableDataSets.FormattingEnabled = true;
            this.lstAvailableDataSets.HorizontalScrollbar = true;
            this.lstAvailableDataSets.ItemHeight = 14;
            this.lstAvailableDataSets.Location = new System.Drawing.Point(6, 19);
            this.lstAvailableDataSets.Name = "lstAvailableDataSets";
            this.lstAvailableDataSets.Size = new System.Drawing.Size(213, 382);
            this.lstAvailableDataSets.TabIndex = 0;
            this.lstAvailableDataSets.SelectedIndexChanged += new System.EventHandler(this.lstAvailableDataSets_SelectedIndexChanged);
            // 
            // grpDataSetContents
            // 
            this.grpDataSetContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDataSetContents.Controls.Add(this.olvMonitorDataSets);
            this.grpDataSetContents.Location = new System.Drawing.Point(244, 8);
            this.grpDataSetContents.Name = "grpDataSetContents";
            this.grpDataSetContents.Size = new System.Drawing.Size(405, 447);
            this.grpDataSetContents.TabIndex = 1;
            this.grpDataSetContents.TabStop = false;
            this.grpDataSetContents.Text = "Dataset Contents (Number of Monitors by Pollutant by Year)";
            // 
            // olvMonitorDataSets
            // 
            this.olvMonitorDataSets.AllColumns.Add(this.olvColumn1);
            this.olvMonitorDataSets.AllColumns.Add(this.olvColumn2);
            this.olvMonitorDataSets.AllColumns.Add(this.olvColumn9);
            this.olvMonitorDataSets.AllowColumnReorder = true;
            this.olvMonitorDataSets.AllowDrop = true;
            this.olvMonitorDataSets.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvMonitorDataSets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
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
            this.olvMonitorDataSets.Location = new System.Drawing.Point(6, 21);
            this.olvMonitorDataSets.MultiSelect = false;
            this.olvMonitorDataSets.Name = "olvMonitorDataSets";
            this.olvMonitorDataSets.OwnerDraw = true;
            this.olvMonitorDataSets.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMonitorDataSets.ShowCommandMenuOnRightClick = true;
            this.olvMonitorDataSets.ShowGroups = false;
            this.olvMonitorDataSets.ShowImagesOnSubItems = true;
            this.olvMonitorDataSets.ShowItemToolTips = true;
            this.olvMonitorDataSets.Size = new System.Drawing.Size(393, 419);
            this.olvMonitorDataSets.TabIndex = 26;
            this.olvMonitorDataSets.UseAlternatingBackColors = true;
            this.olvMonitorDataSets.UseCompatibleStateImageBehavior = false;
            this.olvMonitorDataSets.UseFiltering = true;
            this.olvMonitorDataSets.UseHotItem = true;
            this.olvMonitorDataSets.UseHyperlinks = true;
            this.olvMonitorDataSets.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "pollutantname";
            this.olvColumn1.Text = "Pollutant";
            this.olvColumn1.Width = 146;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "yyear";
            this.olvColumn2.Text = "Year";
            this.olvColumn2.Width = 128;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "Count";
            this.olvColumn9.FillsFreeSpace = true;
            this.olvColumn9.Text = "Monitor Count";
            this.olvColumn9.Width = 109;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(463, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(544, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Location = new System.Drawing.Point(13, 461);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(636, 52);
            this.grpCancelOK.TabIndex = 4;
            this.grpCancelOK.TabStop = false;
            // 
            // ManageMonitorDataSets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 522);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpDataSetContents);
            this.Controls.Add(this.grpAvailableDataSets);
            this.MinimumSize = new System.Drawing.Size(677, 560);
            this.Name = "ManageMonitorDataSets";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Monitor Datasets";
            this.Load += new System.EventHandler(this.ManageMonitorDataSets_Load);
            this.grpAvailableDataSets.ResumeLayout(false);
            this.grpDataSetContents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvMonitorDataSets)).EndInit();
            this.grpCancelOK.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpAvailableDataSets;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstAvailableDataSets;
        private System.Windows.Forms.GroupBox grpDataSetContents;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grpCancelOK;
        public BrightIdeasSoftware.DataListView olvMonitorDataSets;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
    }
}