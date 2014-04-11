namespace BenMAP
{
    partial class ManageIncidenceDataSets
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
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpDataSetIncidenceRate = new System.Windows.Forms.GroupBox();
            this.chbGroup = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboEndpointGroup = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboEndpoint = new System.Windows.Forms.ComboBox();
            this.olvIncidenceRates = new BrightIdeasSoftware.DataListView();
            this.olvcEndpointGroup = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcEndpoint = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.grpAvailableDataSets = new System.Windows.Forms.GroupBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstAvailableDataSets = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnViewMetadata = new System.Windows.Forms.Button();
            this.grpCancelOK.SuspendLayout();
            this.grpDataSetIncidenceRate.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvIncidenceRates)).BeginInit();
            this.grpAvailableDataSets.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.flowLayoutPanel1);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 402);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(770, 53);
            this.grpCancelOK.TabIndex = 2;
            this.grpCancelOK.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(686, 3);
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
            this.btnCancel.Location = new System.Drawing.Point(605, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpDataSetIncidenceRate
            // 
            this.grpDataSetIncidenceRate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDataSetIncidenceRate.Controls.Add(this.chbGroup);
            this.grpDataSetIncidenceRate.Controls.Add(this.groupBox3);
            this.grpDataSetIncidenceRate.Controls.Add(this.groupBox1);
            this.grpDataSetIncidenceRate.Controls.Add(this.groupBox2);
            this.grpDataSetIncidenceRate.Controls.Add(this.olvIncidenceRates);
            this.grpDataSetIncidenceRate.Location = new System.Drawing.Point(233, 8);
            this.grpDataSetIncidenceRate.Name = "grpDataSetIncidenceRate";
            this.grpDataSetIncidenceRate.Size = new System.Drawing.Size(549, 388);
            this.grpDataSetIncidenceRate.TabIndex = 1;
            this.grpDataSetIncidenceRate.TabStop = false;
            this.grpDataSetIncidenceRate.Text = "Dataset Incidence Rates";
            // 
            // chbGroup
            // 
            this.chbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbGroup.AutoSize = true;
            this.chbGroup.Location = new System.Drawing.Point(481, 357);
            this.chbGroup.Name = "chbGroup";
            this.chbGroup.Size = new System.Drawing.Size(59, 18);
            this.chbGroup.TabIndex = 29;
            this.chbGroup.Text = "Group";
            this.chbGroup.UseVisualStyleBackColor = true;
            this.chbGroup.CheckedChanged += new System.EventHandler(this.chbGroup_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.txtFilter);
            this.groupBox3.Location = new System.Drawing.Point(375, 337);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(100, 46);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filter";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtFilter.Location = new System.Drawing.Point(6, 16);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(88, 22);
            this.txtFilter.TabIndex = 0;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.cboEndpointGroup);
            this.groupBox1.Location = new System.Drawing.Point(6, 337);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 46);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Endpoint Group";
            // 
            // cboEndpointGroup
            // 
            this.cboEndpointGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboEndpointGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEndpointGroup.FormattingEnabled = true;
            this.cboEndpointGroup.Location = new System.Drawing.Point(6, 16);
            this.cboEndpointGroup.Name = "cboEndpointGroup";
            this.cboEndpointGroup.Size = new System.Drawing.Size(188, 22);
            this.cboEndpointGroup.TabIndex = 0;
            this.cboEndpointGroup.SelectedIndexChanged += new System.EventHandler(this.cboEndpointGroup_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.cboEndpoint);
            this.groupBox2.Location = new System.Drawing.Point(211, 337);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(160, 46);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter Endpoint";
            // 
            // cboEndpoint
            // 
            this.cboEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEndpoint.FormattingEnabled = true;
            this.cboEndpoint.Location = new System.Drawing.Point(6, 16);
            this.cboEndpoint.Name = "cboEndpoint";
            this.cboEndpoint.Size = new System.Drawing.Size(148, 22);
            this.cboEndpoint.TabIndex = 0;
            this.cboEndpoint.SelectedIndexChanged += new System.EventHandler(this.cboEndpoint_SelectedIndexChanged);
            // 
            // olvIncidenceRates
            // 
            this.olvIncidenceRates.AllColumns.Add(this.olvcEndpointGroup);
            this.olvIncidenceRates.AllColumns.Add(this.olvcEndpoint);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn3);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn4);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn5);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn6);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn7);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn8);
            this.olvIncidenceRates.AllowColumnReorder = true;
            this.olvIncidenceRates.AllowDrop = true;
            this.olvIncidenceRates.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvIncidenceRates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvIncidenceRates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcEndpointGroup,
            this.olvcEndpoint,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn7,
            this.olvColumn8});
            this.olvIncidenceRates.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvIncidenceRates.DataSource = null;
            this.olvIncidenceRates.EmptyListMsg = "This list is empty.";
            this.olvIncidenceRates.FullRowSelect = true;
            this.olvIncidenceRates.GridLines = true;
            this.olvIncidenceRates.GroupWithItemCountFormat = "";
            this.olvIncidenceRates.GroupWithItemCountSingularFormat = "";
            this.olvIncidenceRates.HeaderUsesThemes = false;
            this.olvIncidenceRates.HideSelection = false;
            this.olvIncidenceRates.Location = new System.Drawing.Point(3, 17);
            this.olvIncidenceRates.MultiSelect = false;
            this.olvIncidenceRates.Name = "olvIncidenceRates";
            this.olvIncidenceRates.OwnerDraw = true;
            this.olvIncidenceRates.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvIncidenceRates.ShowCommandMenuOnRightClick = true;
            this.olvIncidenceRates.ShowGroups = false;
            this.olvIncidenceRates.ShowImagesOnSubItems = true;
            this.olvIncidenceRates.ShowItemToolTips = true;
            this.olvIncidenceRates.Size = new System.Drawing.Size(543, 318);
            this.olvIncidenceRates.TabIndex = 25;
            this.olvIncidenceRates.UseAlternatingBackColors = true;
            this.olvIncidenceRates.UseCompatibleStateImageBehavior = false;
            this.olvIncidenceRates.UseFiltering = true;
            this.olvIncidenceRates.UseHotItem = true;
            this.olvIncidenceRates.UseHyperlinks = true;
            this.olvIncidenceRates.View = System.Windows.Forms.View.Details;
            // 
            // olvcEndpointGroup
            // 
            this.olvcEndpointGroup.AspectName = "EndPointGroupName";
            this.olvcEndpointGroup.Text = "Endpoint Group";
            this.olvcEndpointGroup.Width = 98;
            // 
            // olvcEndpoint
            // 
            this.olvcEndpoint.AspectName = "EndPointName";
            this.olvcEndpoint.Text = "Endpoint";
            this.olvcEndpoint.Width = 78;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Prevalence";
            this.olvColumn3.Text = "Type";
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "RaceName";
            this.olvColumn4.Text = "Race";
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "EthnicityName";
            this.olvColumn5.Text = "Ethnicity";
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "GenderName";
            this.olvColumn6.Text = "Gender";
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "StartAge";
            this.olvColumn7.Text = "Start Age";
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "EndAge";
            this.olvColumn8.Text = "End Age";
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
            this.grpAvailableDataSets.Size = new System.Drawing.Size(214, 388);
            this.grpAvailableDataSets.TabIndex = 0;
            this.grpAvailableDataSets.TabStop = false;
            this.grpAvailableDataSets.Text = "Available Datasets";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnEdit.Location = new System.Drawing.Point(147, 349);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(61, 27);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAdd.Location = new System.Drawing.Point(82, 349);
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
            this.btnDelete.Location = new System.Drawing.Point(17, 348);
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
            this.lstAvailableDataSets.Location = new System.Drawing.Point(7, 17);
            this.lstAvailableDataSets.Name = "lstAvailableDataSets";
            this.lstAvailableDataSets.Size = new System.Drawing.Size(201, 312);
            this.lstAvailableDataSets.TabIndex = 0;
            this.lstAvailableDataSets.SelectedValueChanged += new System.EventHandler(this.lstAvailableDataSets_SelectedValueChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnViewMetadata);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(764, 32);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btnViewMetadata
            // 
            this.btnViewMetadata.Location = new System.Drawing.Point(487, 3);
            this.btnViewMetadata.Name = "btnViewMetadata";
            this.btnViewMetadata.Size = new System.Drawing.Size(112, 27);
            this.btnViewMetadata.TabIndex = 33;
            this.btnViewMetadata.Text = "View Metadata";
            this.btnViewMetadata.UseVisualStyleBackColor = true;
            this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
            // 
            // ManageIncidenceDataSets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 459);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpDataSetIncidenceRate);
            this.Controls.Add(this.grpAvailableDataSets);
            this.MinimumSize = new System.Drawing.Size(810, 487);
            this.Name = "ManageIncidenceDataSets";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Incidence Datasets";
            this.Load += new System.EventHandler(this.ManageIncidenceDataSets_Load);
            this.grpCancelOK.ResumeLayout(false);
            this.grpDataSetIncidenceRate.ResumeLayout(false);
            this.grpDataSetIncidenceRate.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvIncidenceRates)).EndInit();
            this.grpAvailableDataSets.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpAvailableDataSets;
        private System.Windows.Forms.ListBox lstAvailableDataSets;
        private System.Windows.Forms.GroupBox grpDataSetIncidenceRate;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private BrightIdeasSoftware.OLVColumn olvcEndpointGroup;
        private BrightIdeasSoftware.OLVColumn olvcEndpoint;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        public BrightIdeasSoftware.DataListView olvIncidenceRates;
        private System.Windows.Forms.CheckBox chbGroup;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboEndpointGroup;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboEndpoint;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnViewMetadata;
    }
}