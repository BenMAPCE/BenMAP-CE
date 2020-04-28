namespace BenMAP
{
	partial class ManagePopulationDataSets
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnViewMetadata = new System.Windows.Forms.Button();
			this.grpDataSetsDetail = new System.Windows.Forms.GroupBox();
			this.grpValues = new System.Windows.Forms.GroupBox();
			this.olvPopulationValues = new BrightIdeasSoftware.DataListView();
			this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn10 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.txtPopulationConfig = new System.Windows.Forms.TextBox();
			this.txtGridDefinition = new System.Windows.Forms.TextBox();
			this.lblPopulationConfiguration = new System.Windows.Forms.Label();
			this.lblGridDefinition = new System.Windows.Forms.Label();
			this.grpAvailableDataSets = new System.Windows.Forms.GroupBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.lstAvailableDataSetsName = new System.Windows.Forms.ListBox();
			this.grpCancelOK.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.grpDataSetsDetail.SuspendLayout();
			this.grpValues.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvPopulationValues)).BeginInit();
			this.grpAvailableDataSets.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpCancelOK
			// 
			this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.grpCancelOK.Controls.Add(this.flowLayoutPanel1);
			this.grpCancelOK.Location = new System.Drawing.Point(12, 344);
			this.grpCancelOK.Name = "grpCancelOK";
			this.grpCancelOK.Size = new System.Drawing.Size(615, 52);
			this.grpCancelOK.TabIndex = 2;
			this.grpCancelOK.TabStop = false;
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
			this.flowLayoutPanel1.Size = new System.Drawing.Size(609, 31);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(531, 3);
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
			this.btnCancel.Location = new System.Drawing.Point(450, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 27);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Visible = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnViewMetadata
			// 
			this.btnViewMetadata.Enabled = false;
			this.btnViewMetadata.Location = new System.Drawing.Point(332, 3);
			this.btnViewMetadata.Name = "btnViewMetadata";
			this.btnViewMetadata.Size = new System.Drawing.Size(112, 27);
			this.btnViewMetadata.TabIndex = 0;
			this.btnViewMetadata.Text = "View Metadata";
			this.btnViewMetadata.UseVisualStyleBackColor = true;
			this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
			// 
			// grpDataSetsDetail
			// 
			this.grpDataSetsDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.grpDataSetsDetail.Controls.Add(this.grpValues);
			this.grpDataSetsDetail.Controls.Add(this.txtPopulationConfig);
			this.grpDataSetsDetail.Controls.Add(this.txtGridDefinition);
			this.grpDataSetsDetail.Controls.Add(this.lblPopulationConfiguration);
			this.grpDataSetsDetail.Controls.Add(this.lblGridDefinition);
			this.grpDataSetsDetail.Location = new System.Drawing.Point(180, 8);
			this.grpDataSetsDetail.Name = "grpDataSetsDetail";
			this.grpDataSetsDetail.Size = new System.Drawing.Size(447, 330);
			this.grpDataSetsDetail.TabIndex = 1;
			this.grpDataSetsDetail.TabStop = false;
			this.grpDataSetsDetail.Text = "Dataset Detail";
			// 
			// grpValues
			// 
			this.grpValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.grpValues.Controls.Add(this.olvPopulationValues);
			this.grpValues.Location = new System.Drawing.Point(6, 84);
			this.grpValues.Name = "grpValues";
			this.grpValues.Size = new System.Drawing.Size(435, 239);
			this.grpValues.TabIndex = 4;
			this.grpValues.TabStop = false;
			this.grpValues.Text = "Values";
			// 
			// olvPopulationValues
			// 
			this.olvPopulationValues.AllColumns.Add(this.olvColumn3);
			this.olvPopulationValues.AllColumns.Add(this.olvColumn4);
			this.olvPopulationValues.AllColumns.Add(this.olvColumn5);
			this.olvPopulationValues.AllColumns.Add(this.olvColumn6);
			this.olvPopulationValues.AllColumns.Add(this.olvColumn7);
			this.olvPopulationValues.AllColumns.Add(this.olvColumn8);
			this.olvPopulationValues.AllColumns.Add(this.olvColumn10);
			this.olvPopulationValues.AllowColumnReorder = true;
			this.olvPopulationValues.AllowDrop = true;
			this.olvPopulationValues.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
			this.olvPopulationValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.olvPopulationValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.olvColumn3,
						this.olvColumn4,
						this.olvColumn5,
						this.olvColumn6,
						this.olvColumn7,
						this.olvColumn8,
						this.olvColumn10});
			this.olvPopulationValues.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvPopulationValues.DataSource = null;
			this.olvPopulationValues.EmptyListMsg = "This list is empty.";
			this.olvPopulationValues.FullRowSelect = true;
			this.olvPopulationValues.GridLines = true;
			this.olvPopulationValues.GroupWithItemCountFormat = "";
			this.olvPopulationValues.GroupWithItemCountSingularFormat = "";
			this.olvPopulationValues.HeaderUsesThemes = false;
			this.olvPopulationValues.HideSelection = false;
			this.olvPopulationValues.Location = new System.Drawing.Point(6, 21);
			this.olvPopulationValues.MultiSelect = false;
			this.olvPopulationValues.Name = "olvPopulationValues";
			this.olvPopulationValues.OwnerDraw = true;
			this.olvPopulationValues.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
			this.olvPopulationValues.ShowCommandMenuOnRightClick = true;
			this.olvPopulationValues.ShowGroups = false;
			this.olvPopulationValues.ShowImagesOnSubItems = true;
			this.olvPopulationValues.ShowItemToolTips = true;
			this.olvPopulationValues.Size = new System.Drawing.Size(423, 212);
			this.olvPopulationValues.TabIndex = 0;
			this.olvPopulationValues.UseAlternatingBackColors = true;
			this.olvPopulationValues.UseCompatibleStateImageBehavior = false;
			this.olvPopulationValues.UseFiltering = true;
			this.olvPopulationValues.UseHotItem = true;
			this.olvPopulationValues.UseHyperlinks = true;
			this.olvPopulationValues.UseOverlays = false;
			this.olvPopulationValues.View = System.Windows.Forms.View.Details;
			this.olvPopulationValues.SelectedIndexChanged += new System.EventHandler(this.olvPopulationValues_SelectedIndexChanged);
			// 
			// olvColumn3
			// 
			this.olvColumn3.AspectName = "RaceName";
			this.olvColumn3.Text = "Race";
			// 
			// olvColumn4
			// 
			this.olvColumn4.AspectName = "EthnicityName";
			this.olvColumn4.Text = "Ethnicity";
			// 
			// olvColumn5
			// 
			this.olvColumn5.AspectName = "GenderName";
			this.olvColumn5.Text = "Gender";
			// 
			// olvColumn6
			// 
			this.olvColumn6.AspectName = "AgeRangeName";
			this.olvColumn6.Text = "Age Range";
			// 
			// olvColumn7
			// 
			this.olvColumn7.AspectName = "CColumn";
			this.olvColumn7.Text = "Column";
			// 
			// olvColumn8
			// 
			this.olvColumn8.AspectName = "Row";
			this.olvColumn8.Text = "Row";
			this.olvColumn8.Width = 50;
			// 
			// olvColumn10
			// 
			this.olvColumn10.AspectName = "VValue";
			this.olvColumn10.AspectToStringFormat = "{0:N4}";
			this.olvColumn10.FillsFreeSpace = true;
			this.olvColumn10.Text = "Value";
			// 
			// txtPopulationConfig
			// 
			this.txtPopulationConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPopulationConfig.Enabled = false;
			this.txtPopulationConfig.Location = new System.Drawing.Point(179, 54);
			this.txtPopulationConfig.Name = "txtPopulationConfig";
			this.txtPopulationConfig.Size = new System.Drawing.Size(262, 22);
			this.txtPopulationConfig.TabIndex = 3;
			// 
			// txtGridDefinition
			// 
			this.txtGridDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtGridDefinition.Enabled = false;
			this.txtGridDefinition.Location = new System.Drawing.Point(179, 21);
			this.txtGridDefinition.Name = "txtGridDefinition";
			this.txtGridDefinition.Size = new System.Drawing.Size(262, 22);
			this.txtGridDefinition.TabIndex = 2;
			// 
			// lblPopulationConfiguration
			// 
			this.lblPopulationConfiguration.AutoSize = true;
			this.lblPopulationConfiguration.Location = new System.Drawing.Point(16, 57);
			this.lblPopulationConfiguration.Name = "lblPopulationConfiguration";
			this.lblPopulationConfiguration.Size = new System.Drawing.Size(143, 14);
			this.lblPopulationConfiguration.TabIndex = 1;
			this.lblPopulationConfiguration.Text = "Population Configuration:";
			// 
			// lblGridDefinition
			// 
			this.lblGridDefinition.AutoSize = true;
			this.lblGridDefinition.Location = new System.Drawing.Point(16, 24);
			this.lblGridDefinition.Name = "lblGridDefinition";
			this.lblGridDefinition.Size = new System.Drawing.Size(89, 14);
			this.lblGridDefinition.TabIndex = 0;
			this.lblGridDefinition.Text = "Grid Definition:";
			// 
			// grpAvailableDataSets
			// 
			this.grpAvailableDataSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)));
			this.grpAvailableDataSets.Controls.Add(this.btnAdd);
			this.grpAvailableDataSets.Controls.Add(this.btnDelete);
			this.grpAvailableDataSets.Controls.Add(this.lstAvailableDataSetsName);
			this.grpAvailableDataSets.Location = new System.Drawing.Point(12, 8);
			this.grpAvailableDataSets.Name = "grpAvailableDataSets";
			this.grpAvailableDataSets.Size = new System.Drawing.Size(162, 330);
			this.grpAvailableDataSets.TabIndex = 0;
			this.grpAvailableDataSets.TabStop = false;
			this.grpAvailableDataSets.Text = "Available Datasets";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.Location = new System.Drawing.Point(81, 292);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(69, 27);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelete.Location = new System.Drawing.Point(6, 292);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(69, 27);
			this.btnDelete.TabIndex = 1;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// lstAvailableDataSetsName
			// 
			this.lstAvailableDataSetsName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)));
			this.lstAvailableDataSetsName.FormattingEnabled = true;
			this.lstAvailableDataSetsName.HorizontalScrollbar = true;
			this.lstAvailableDataSetsName.ItemHeight = 14;
			this.lstAvailableDataSetsName.Location = new System.Drawing.Point(6, 17);
			this.lstAvailableDataSetsName.Name = "lstAvailableDataSetsName";
			this.lstAvailableDataSetsName.Size = new System.Drawing.Size(150, 256);
			this.lstAvailableDataSetsName.TabIndex = 0;
			this.lstAvailableDataSetsName.SelectedValueChanged += new System.EventHandler(this.lstAvailableDataSetsName_SelectedValueChanged);
			// 
			// ManagePopulationDataSets
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(639, 403);
			this.Controls.Add(this.grpCancelOK);
			this.Controls.Add(this.grpDataSetsDetail);
			this.Controls.Add(this.grpAvailableDataSets);
			this.Name = "ManagePopulationDataSets";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Manage Population Datasets";
			this.Load += new System.EventHandler(this.ManagePopulationDataSets_Load);
			this.grpCancelOK.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.grpDataSetsDetail.ResumeLayout(false);
			this.grpDataSetsDetail.PerformLayout();
			this.grpValues.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvPopulationValues)).EndInit();
			this.grpAvailableDataSets.ResumeLayout(false);
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.GroupBox grpAvailableDataSets;
		private System.Windows.Forms.ListBox lstAvailableDataSetsName;
		private System.Windows.Forms.GroupBox grpDataSetsDetail;
		private System.Windows.Forms.TextBox txtPopulationConfig;
		private System.Windows.Forms.TextBox txtGridDefinition;
		private System.Windows.Forms.Label lblPopulationConfiguration;
		private System.Windows.Forms.Label lblGridDefinition;
		private System.Windows.Forms.GroupBox grpCancelOK;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.GroupBox grpValues;
		public BrightIdeasSoftware.DataListView olvPopulationValues;
		private BrightIdeasSoftware.OLVColumn olvColumn1;
		private BrightIdeasSoftware.OLVColumn olvColumn2;
		private BrightIdeasSoftware.OLVColumn olvColumn9;
		private BrightIdeasSoftware.OLVColumn olvColumn3;
		private BrightIdeasSoftware.OLVColumn olvColumn4;
		private BrightIdeasSoftware.OLVColumn olvColumn5;
		private BrightIdeasSoftware.OLVColumn olvColumn6;
		private BrightIdeasSoftware.OLVColumn olvColumn7;
		private BrightIdeasSoftware.OLVColumn olvColumn8;
		private BrightIdeasSoftware.OLVColumn olvColumn10;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button btnViewMetadata;
	}
}