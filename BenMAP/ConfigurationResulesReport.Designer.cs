namespace BenMAP
{
	partial class ConfigurationResultsReport
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
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblInputTip = new System.Windows.Forms.Label();
			this.chkLstPercentile10 = new System.Windows.Forms.CheckedListBox();
			this.tbPercentiles = new System.Windows.Forms.TextBox();
			this.cbPercentile = new System.Windows.Forms.CheckBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.olvColumnRow = new BrightIdeasSoftware.ObjectListView();
			this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.panel3 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.olvHealth = new BrightIdeasSoftware.ObjectListView();
			this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.panel4 = new System.Windows.Forms.Panel();
			this.olvResult = new BrightIdeasSoftware.ObjectListView();
			this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvColumnRow)).BeginInit();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvHealth)).BeginInit();
			this.panel4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvResult)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.panel1);
			this.groupBox3.Controls.Add(this.tbPercentiles);
			this.groupBox3.Controls.Add(this.cbPercentile);
			this.groupBox3.Controls.Add(this.btnOK);
			this.groupBox3.Location = new System.Drawing.Point(2, 235);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(689, 89);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.lblInputTip);
			this.panel1.Controls.Add(this.chkLstPercentile10);
			this.panel1.Location = new System.Drawing.Point(7, 45);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(676, 38);
			this.panel1.TabIndex = 5;
			// 
			// lblInputTip
			// 
			this.lblInputTip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblInputTip.AutoSize = true;
			this.lblInputTip.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInputTip.ForeColor = System.Drawing.Color.Peru;
			this.lblInputTip.Location = new System.Drawing.Point(232, 0);
			this.lblInputTip.Name = "lblInputTip";
			this.lblInputTip.Size = new System.Drawing.Size(218, 14);
			this.lblInputTip.TabIndex = 1;
			this.lblInputTip.Text = "Input the percentiles (such as ¡°2.5, 97.5¡±)";
			this.lblInputTip.Visible = false;
			// 
			// chkLstPercentile10
			// 
			this.chkLstPercentile10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.chkLstPercentile10.CheckOnClick = true;
			this.chkLstPercentile10.FormattingEnabled = true;
			this.chkLstPercentile10.Items.AddRange(new object[] {
						"Percentile5",
						"Percentile15",
						"Percentile25",
						"Percentile35",
						"Percentile45",
						"Percentile55",
						"Percentile65",
						"Percentile75",
						"Percentile85",
						"Percentile95"});
			this.chkLstPercentile10.Location = new System.Drawing.Point(0, 0);
			this.chkLstPercentile10.MultiColumn = true;
			this.chkLstPercentile10.Name = "chkLstPercentile10";
			this.chkLstPercentile10.Size = new System.Drawing.Size(676, 38);
			this.chkLstPercentile10.TabIndex = 0;
			this.chkLstPercentile10.Visible = false;
			// 
			// tbPercentiles
			// 
			this.tbPercentiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPercentiles.Location = new System.Drawing.Point(242, 16);
			this.tbPercentiles.Name = "tbPercentiles";
			this.tbPercentiles.Size = new System.Drawing.Size(357, 22);
			this.tbPercentiles.TabIndex = 1;
			this.tbPercentiles.Visible = false;
			this.tbPercentiles.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPercentiles_KeyPress);
			// 
			// cbPercentile
			// 
			this.cbPercentile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbPercentile.AutoSize = true;
			this.cbPercentile.Location = new System.Drawing.Point(9, 18);
			this.cbPercentile.Name = "cbPercentile";
			this.cbPercentile.Size = new System.Drawing.Size(227, 18);
			this.cbPercentile.TabIndex = 0;
			this.cbPercentile.Text = "Only show user-assigned percentiles";
			this.cbPercentile.UseVisualStyleBackColor = true;
			this.cbPercentile.CheckedChanged += new System.EventHandler(this.cbPercentile_CheckedChanged);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(605, 15);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.tableLayoutPanel1);
			this.groupBox1.Location = new System.Drawing.Point(2, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(689, 241);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Column Selection";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.27273F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.36364F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.36364F));
			this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel4, 2, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 18);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(678, 217);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.olvColumnRow);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(178, 211);
			this.panel2.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Grid Fields:";
			// 
			// olvColumnRow
			// 
			this.olvColumnRow.AllColumns.Add(this.olvColumn2);
			this.olvColumnRow.AllowColumnReorder = true;
			this.olvColumnRow.AllowDrop = true;
			this.olvColumnRow.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
			this.olvColumnRow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.olvColumnRow.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this.olvColumnRow.CheckBoxes = true;
			this.olvColumnRow.CheckedAspectName = "isChecked";
			this.olvColumnRow.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.olvColumn2});
			this.olvColumnRow.CopySelectionOnControlC = false;
			this.olvColumnRow.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvColumnRow.EmptyListMsg = "This list is empty. ";
			this.olvColumnRow.FullRowSelect = true;
			this.olvColumnRow.HeaderUsesThemes = false;
			this.olvColumnRow.HideSelection = false;
			this.olvColumnRow.Location = new System.Drawing.Point(0, 20);
			this.olvColumnRow.Name = "olvColumnRow";
			this.olvColumnRow.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
			this.olvColumnRow.OverlayText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.olvColumnRow.OverlayText.BorderWidth = 2F;
			this.olvColumnRow.OverlayText.Rotation = -20;
			this.olvColumnRow.OverlayText.Text = "";
			this.olvColumnRow.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
			this.olvColumnRow.ShowCommandMenuOnRightClick = true;
			this.olvColumnRow.ShowGroups = false;
			this.olvColumnRow.ShowImagesOnSubItems = true;
			this.olvColumnRow.ShowItemCountOnGroups = true;
			this.olvColumnRow.ShowItemToolTips = true;
			this.olvColumnRow.Size = new System.Drawing.Size(178, 191);
			this.olvColumnRow.SpaceBetweenGroups = 20;
			this.olvColumnRow.TabIndex = 7;
			this.olvColumnRow.UseAlternatingBackColors = true;
			this.olvColumnRow.UseCompatibleStateImageBehavior = false;
			this.olvColumnRow.UseFiltering = true;
			this.olvColumnRow.UseHotItem = true;
			this.olvColumnRow.UseHyperlinks = true;
			this.olvColumnRow.View = System.Windows.Forms.View.Details;
			// 
			// olvColumn2
			// 
			this.olvColumn2.AspectName = "FieldName";
			this.olvColumn2.IsEditable = false;
			this.olvColumn2.IsTileViewColumn = true;
			this.olvColumn2.MaximumWidth = 180;
			this.olvColumn2.MinimumWidth = 50;
			this.olvColumn2.Text = "Field Name";
			this.olvColumn2.Width = 169;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.label2);
			this.panel3.Controls.Add(this.olvHealth);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(187, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(240, 211);
			this.panel3.TabIndex = 10;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(173, 14);
			this.label2.TabIndex = 3;
			this.label2.Text = "Health impact Function Fields:";
			// 
			// olvHealth
			// 
			this.olvHealth.AllColumns.Add(this.olvColumn1);
			this.olvHealth.AllowColumnReorder = true;
			this.olvHealth.AllowDrop = true;
			this.olvHealth.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
			this.olvHealth.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.olvHealth.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this.olvHealth.CheckBoxes = true;
			this.olvHealth.CheckedAspectName = "isChecked";
			this.olvHealth.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.olvColumn1});
			this.olvHealth.CopySelectionOnControlC = false;
			this.olvHealth.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvHealth.EmptyListMsg = "This list is empty. ";
			this.olvHealth.FullRowSelect = true;
			this.olvHealth.HeaderUsesThemes = false;
			this.olvHealth.HideSelection = false;
			this.olvHealth.Location = new System.Drawing.Point(0, 20);
			this.olvHealth.Name = "olvHealth";
			this.olvHealth.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
			this.olvHealth.OverlayText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.olvHealth.OverlayText.BorderWidth = 2F;
			this.olvHealth.OverlayText.Rotation = -20;
			this.olvHealth.OverlayText.Text = "";
			this.olvHealth.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
			this.olvHealth.ShowCommandMenuOnRightClick = true;
			this.olvHealth.ShowGroups = false;
			this.olvHealth.ShowImagesOnSubItems = true;
			this.olvHealth.ShowItemCountOnGroups = true;
			this.olvHealth.ShowItemToolTips = true;
			this.olvHealth.Size = new System.Drawing.Size(240, 191);
			this.olvHealth.SpaceBetweenGroups = 20;
			this.olvHealth.TabIndex = 8;
			this.olvHealth.UseAlternatingBackColors = true;
			this.olvHealth.UseCompatibleStateImageBehavior = false;
			this.olvHealth.UseFiltering = true;
			this.olvHealth.UseHotItem = true;
			this.olvHealth.UseHyperlinks = true;
			this.olvHealth.View = System.Windows.Forms.View.Details;
			// 
			// olvColumn1
			// 
			this.olvColumn1.AspectName = "FieldName";
			this.olvColumn1.IsEditable = false;
			this.olvColumn1.IsTileViewColumn = true;
			this.olvColumn1.MaximumWidth = 180;
			this.olvColumn1.MinimumWidth = 50;
			this.olvColumn1.Text = "Field Name";
			this.olvColumn1.Width = 180;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.olvResult);
			this.panel4.Controls.Add(this.label3);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(433, 3);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(242, 211);
			this.panel4.TabIndex = 11;
			// 
			// olvResult
			// 
			this.olvResult.AllColumns.Add(this.olvColumn3);
			this.olvResult.AllowColumnReorder = true;
			this.olvResult.AllowDrop = true;
			this.olvResult.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
			this.olvResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.olvResult.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this.olvResult.CellEditEnterChangesRows = true;
			this.olvResult.CheckBoxes = true;
			this.olvResult.CheckedAspectName = "isChecked";
			this.olvResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.olvColumn3});
			this.olvResult.CopySelectionOnControlC = false;
			this.olvResult.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvResult.EmptyListMsg = "This list is empty. ";
			this.olvResult.FullRowSelect = true;
			this.olvResult.HeaderUsesThemes = false;
			this.olvResult.HideSelection = false;
			this.olvResult.Location = new System.Drawing.Point(0, 20);
			this.olvResult.Name = "olvResult";
			this.olvResult.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
			this.olvResult.OverlayText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.olvResult.OverlayText.BorderWidth = 2F;
			this.olvResult.OverlayText.Rotation = -20;
			this.olvResult.OverlayText.Text = "";
			this.olvResult.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
			this.olvResult.ShowCommandMenuOnRightClick = true;
			this.olvResult.ShowGroups = false;
			this.olvResult.ShowImagesOnSubItems = true;
			this.olvResult.ShowItemCountOnGroups = true;
			this.olvResult.ShowItemToolTips = true;
			this.olvResult.Size = new System.Drawing.Size(242, 191);
			this.olvResult.SpaceBetweenGroups = 20;
			this.olvResult.TabIndex = 9;
			this.olvResult.UseAlternatingBackColors = true;
			this.olvResult.UseCompatibleStateImageBehavior = false;
			this.olvResult.UseFiltering = true;
			this.olvResult.UseHotItem = true;
			this.olvResult.UseHyperlinks = true;
			this.olvResult.View = System.Windows.Forms.View.Details;
			this.olvResult.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.olvResult_ItemChecked);
			// 
			// olvColumn3
			// 
			this.olvColumn3.AspectName = "FieldName";
			this.olvColumn3.IsEditable = false;
			this.olvColumn3.IsTileViewColumn = true;
			this.olvColumn3.MaximumWidth = 180;
			this.olvColumn3.MinimumWidth = 50;
			this.olvColumn3.Text = "Field Name";
			this.olvColumn3.Width = 180;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Result Fields:";
			// 
			// ConfigurationResultsReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(694, 324);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.MinimumSize = new System.Drawing.Size(710, 362);
			this.Name = "ConfigurationResultsReport";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Configuration Results Report";
			this.Load += new System.EventHandler(this.ConfigurationResultsReport_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvColumnRow)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvHealth)).EndInit();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvResult)).EndInit();
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private BrightIdeasSoftware.ObjectListView olvResult;
		private BrightIdeasSoftware.OLVColumn olvColumn3;
		private BrightIdeasSoftware.ObjectListView olvHealth;
		private BrightIdeasSoftware.OLVColumn olvColumn1;
		private BrightIdeasSoftware.ObjectListView olvColumnRow;
		private BrightIdeasSoftware.OLVColumn olvColumn2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox tbPercentiles;
		private System.Windows.Forms.CheckedListBox chkLstPercentile10;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label lblInputTip;
		private System.Windows.Forms.CheckBox cbPercentile;
	}
}