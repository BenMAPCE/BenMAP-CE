namespace BenMAP
{
	partial class MonitorData
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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnGo = new System.Windows.Forms.Button();
			this.btnMap = new System.Windows.Forms.Button();
			this.btnAdvanced = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cboMonitorType = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPollutant = new System.Windows.Forms.TextBox();
			this.txtGridType = new System.Windows.Forms.TextBox();
			this.tcMonitorData = new System.Windows.Forms.TabControl();
			this.tpLibrary = new System.Windows.Forms.TabPage();
			this.cboMonitorLibraryYear = new System.Windows.Forms.ComboBox();
			this.cboMonitorDataSet = new System.Windows.Forms.ComboBox();
			this.lblMonitorLibraryYear = new System.Windows.Forms.Label();
			this.lblMonitorDataSet = new System.Windows.Forms.Label();
			this.tpText = new System.Windows.Forms.TabPage();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.txtMonitorDataFile = new System.Windows.Forms.TextBox();
			this.lblMonitorDataFile = new System.Windows.Forms.Label();
			this.grpInterpolationMethod = new System.Windows.Forms.GroupBox();
			this.txtRadiums = new System.Windows.Forms.TextBox();
			this.rbtnFixedRadiums = new System.Windows.Forms.RadioButton();
			this.rbtnVoronoi = new System.Windows.Forms.RadioButton();
			this.rbtnClosestMonitor = new System.Windows.Forms.RadioButton();
			this.lblPollutant = new System.Windows.Forms.Label();
			this.lblGridType = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tcMonitorData.SuspendLayout();
			this.tpLibrary.SuspendLayout();
			this.tpText.SuspendLayout();
			this.grpInterpolationMethod.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnGo);
			this.groupBox2.Controls.Add(this.btnMap);
			this.groupBox2.Controls.Add(this.btnAdvanced);
			this.groupBox2.Controls.Add(this.btnCancel);
			this.groupBox2.Location = new System.Drawing.Point(12, 220);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(501, 55);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(421, 20);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(66, 27);
			this.btnGo.TabIndex = 3;
			this.btnGo.Text = "Next";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// btnMap
			// 
			this.btnMap.Location = new System.Drawing.Point(337, 20);
			this.btnMap.Name = "btnMap";
			this.btnMap.Size = new System.Drawing.Size(66, 27);
			this.btnMap.TabIndex = 2;
			this.btnMap.Text = "Map";
			this.btnMap.UseVisualStyleBackColor = true;
			this.btnMap.Click += new System.EventHandler(this.btnMap_Click);
			// 
			// btnAdvanced
			// 
			this.btnAdvanced.Location = new System.Drawing.Point(248, 21);
			this.btnAdvanced.Name = "btnAdvanced";
			this.btnAdvanced.Size = new System.Drawing.Size(70, 27);
			this.btnAdvanced.TabIndex = 1;
			this.btnAdvanced.Text = "Advanced";
			this.btnAdvanced.UseVisualStyleBackColor = true;
			this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(162, 21);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(66, 27);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cboMonitorType);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.txtPollutant);
			this.groupBox1.Controls.Add(this.txtGridType);
			this.groupBox1.Controls.Add(this.tcMonitorData);
			this.groupBox1.Controls.Add(this.grpInterpolationMethod);
			this.groupBox1.Controls.Add(this.lblPollutant);
			this.groupBox1.Controls.Add(this.lblGridType);
			this.groupBox1.Location = new System.Drawing.Point(12, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(501, 219);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// cboMonitorType
			// 
			this.cboMonitorType.BackColor = System.Drawing.Color.White;
			this.cboMonitorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMonitorType.FormattingEnabled = true;
			this.cboMonitorType.Location = new System.Drawing.Point(346, 27);
			this.cboMonitorType.Name = "cboMonitorType";
			this.cboMonitorType.Size = new System.Drawing.Size(145, 22);
			this.cboMonitorType.TabIndex = 6;
			this.cboMonitorType.SelectedIndexChanged += new System.EventHandler(this.cboMonitorType_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(245, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 14);
			this.label1.TabIndex = 5;
			this.label1.Text = "Monitor Dataset:";
			// 
			// txtPollutant
			// 
			this.txtPollutant.Location = new System.Drawing.Point(87, 62);
			this.txtPollutant.Name = "txtPollutant";
			this.txtPollutant.Size = new System.Drawing.Size(141, 22);
			this.txtPollutant.TabIndex = 3;
			// 
			// txtGridType
			// 
			this.txtGridType.Location = new System.Drawing.Point(87, 27);
			this.txtGridType.Name = "txtGridType";
			this.txtGridType.Size = new System.Drawing.Size(141, 22);
			this.txtGridType.TabIndex = 1;
			// 
			// tcMonitorData
			// 
			this.tcMonitorData.Controls.Add(this.tpLibrary);
			this.tcMonitorData.Controls.Add(this.tpText);
			this.tcMonitorData.Location = new System.Drawing.Point(248, 62);
			this.tcMonitorData.Name = "tcMonitorData";
			this.tcMonitorData.SelectedIndex = 0;
			this.tcMonitorData.Size = new System.Drawing.Size(247, 143);
			this.tcMonitorData.TabIndex = 7;
			// 
			// tpLibrary
			// 
			this.tpLibrary.Controls.Add(this.cboMonitorLibraryYear);
			this.tpLibrary.Controls.Add(this.cboMonitorDataSet);
			this.tpLibrary.Controls.Add(this.lblMonitorLibraryYear);
			this.tpLibrary.Controls.Add(this.lblMonitorDataSet);
			this.tpLibrary.Location = new System.Drawing.Point(4, 23);
			this.tpLibrary.Name = "tpLibrary";
			this.tpLibrary.Padding = new System.Windows.Forms.Padding(3);
			this.tpLibrary.Size = new System.Drawing.Size(239, 116);
			this.tpLibrary.TabIndex = 0;
			this.tpLibrary.Text = "Library";
			this.tpLibrary.UseVisualStyleBackColor = true;
			// 
			// cboMonitorLibraryYear
			// 
			this.cboMonitorLibraryYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMonitorLibraryYear.FormattingEnabled = true;
			this.cboMonitorLibraryYear.Location = new System.Drawing.Point(9, 83);
			this.cboMonitorLibraryYear.Name = "cboMonitorLibraryYear";
			this.cboMonitorLibraryYear.Size = new System.Drawing.Size(217, 22);
			this.cboMonitorLibraryYear.TabIndex = 3;
			// 
			// cboMonitorDataSet
			// 
			this.cboMonitorDataSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMonitorDataSet.FormattingEnabled = true;
			this.cboMonitorDataSet.Location = new System.Drawing.Point(9, 31);
			this.cboMonitorDataSet.Name = "cboMonitorDataSet";
			this.cboMonitorDataSet.Size = new System.Drawing.Size(217, 22);
			this.cboMonitorDataSet.TabIndex = 1;
			this.cboMonitorDataSet.SelectedValueChanged += new System.EventHandler(this.cboMonitorDataSet_SelectedValueChanged);
			// 
			// lblMonitorLibraryYear
			// 
			this.lblMonitorLibraryYear.AutoSize = true;
			this.lblMonitorLibraryYear.Location = new System.Drawing.Point(6, 64);
			this.lblMonitorLibraryYear.Name = "lblMonitorLibraryYear";
			this.lblMonitorLibraryYear.Size = new System.Drawing.Size(118, 14);
			this.lblMonitorLibraryYear.TabIndex = 2;
			this.lblMonitorLibraryYear.Text = "Monitor Library Year:";
			// 
			// lblMonitorDataSet
			// 
			this.lblMonitorDataSet.AutoSize = true;
			this.lblMonitorDataSet.Location = new System.Drawing.Point(6, 14);
			this.lblMonitorDataSet.Name = "lblMonitorDataSet";
			this.lblMonitorDataSet.Size = new System.Drawing.Size(96, 14);
			this.lblMonitorDataSet.TabIndex = 0;
			this.lblMonitorDataSet.Text = "Monitor Dataset";
			// 
			// tpText
			// 
			this.tpText.Controls.Add(this.btnBrowse);
			this.tpText.Controls.Add(this.txtMonitorDataFile);
			this.tpText.Controls.Add(this.lblMonitorDataFile);
			this.tpText.Location = new System.Drawing.Point(4, 23);
			this.tpText.Name = "tpText";
			this.tpText.Padding = new System.Windows.Forms.Padding(3);
			this.tpText.Size = new System.Drawing.Size(239, 116);
			this.tpText.TabIndex = 1;
			this.tpText.Text = "Text File";
			this.tpText.UseVisualStyleBackColor = true;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Image = global::BenMAP.Properties.Resources.folder_add;
			this.btnBrowse.Location = new System.Drawing.Point(194, 55);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(42, 22);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// txtMonitorDataFile
			// 
			this.txtMonitorDataFile.BackColor = System.Drawing.Color.White;
			this.txtMonitorDataFile.Location = new System.Drawing.Point(8, 55);
			this.txtMonitorDataFile.Name = "txtMonitorDataFile";
			this.txtMonitorDataFile.ReadOnly = true;
			this.txtMonitorDataFile.Size = new System.Drawing.Size(179, 22);
			this.txtMonitorDataFile.TabIndex = 1;
			// 
			// lblMonitorDataFile
			// 
			this.lblMonitorDataFile.AutoSize = true;
			this.lblMonitorDataFile.Location = new System.Drawing.Point(6, 33);
			this.lblMonitorDataFile.Name = "lblMonitorDataFile";
			this.lblMonitorDataFile.Size = new System.Drawing.Size(106, 14);
			this.lblMonitorDataFile.TabIndex = 0;
			this.lblMonitorDataFile.Text = "Monitor Data File:";
			// 
			// grpInterpolationMethod
			// 
			this.grpInterpolationMethod.Controls.Add(this.txtRadiums);
			this.grpInterpolationMethod.Controls.Add(this.rbtnFixedRadiums);
			this.grpInterpolationMethod.Controls.Add(this.rbtnVoronoi);
			this.grpInterpolationMethod.Controls.Add(this.rbtnClosestMonitor);
			this.grpInterpolationMethod.Location = new System.Drawing.Point(15, 94);
			this.grpInterpolationMethod.Name = "grpInterpolationMethod";
			this.grpInterpolationMethod.Size = new System.Drawing.Size(213, 111);
			this.grpInterpolationMethod.TabIndex = 4;
			this.grpInterpolationMethod.TabStop = false;
			this.grpInterpolationMethod.Text = "Interpolation Method";
			// 
			// txtRadiums
			// 
			this.txtRadiums.Location = new System.Drawing.Point(133, 78);
			this.txtRadiums.Name = "txtRadiums";
			this.txtRadiums.Size = new System.Drawing.Size(49, 22);
			this.txtRadiums.TabIndex = 3;
			this.txtRadiums.Click += new System.EventHandler(this.txtRadiums_Click);
			this.txtRadiums.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRadiums_KeyPress);
			// 
			// rbtnFixedRadiums
			// 
			this.rbtnFixedRadiums.AutoSize = true;
			this.rbtnFixedRadiums.Location = new System.Drawing.Point(7, 78);
			this.rbtnFixedRadiums.Name = "rbtnFixedRadiums";
			this.rbtnFixedRadiums.Size = new System.Drawing.Size(122, 18);
			this.rbtnFixedRadiums.TabIndex = 2;
			this.rbtnFixedRadiums.TabStop = true;
			this.rbtnFixedRadiums.Tag = "fixedradius";
			this.rbtnFixedRadiums.Text = "Fixed Radius (km)";
			this.rbtnFixedRadiums.UseVisualStyleBackColor = true;
			this.rbtnFixedRadiums.Click += new System.EventHandler(this.rbtnFixedRadiums_Click);
			// 
			// rbtnVoronoi
			// 
			this.rbtnVoronoi.AutoSize = true;
			this.rbtnVoronoi.Location = new System.Drawing.Point(7, 51);
			this.rbtnVoronoi.Name = "rbtnVoronoi";
			this.rbtnVoronoi.Size = new System.Drawing.Size(204, 18);
			this.rbtnVoronoi.TabIndex = 1;
			this.rbtnVoronoi.TabStop = true;
			this.rbtnVoronoi.Tag = "voronoineighborhoodaveraging";
			this.rbtnVoronoi.Text = "Voronoi Neighborhood Averaging";
			this.rbtnVoronoi.UseVisualStyleBackColor = true;
			// 
			// rbtnClosestMonitor
			// 
			this.rbtnClosestMonitor.AutoSize = true;
			this.rbtnClosestMonitor.Location = new System.Drawing.Point(7, 24);
			this.rbtnClosestMonitor.Name = "rbtnClosestMonitor";
			this.rbtnClosestMonitor.Size = new System.Drawing.Size(111, 18);
			this.rbtnClosestMonitor.TabIndex = 0;
			this.rbtnClosestMonitor.TabStop = true;
			this.rbtnClosestMonitor.Tag = "closestmonitor";
			this.rbtnClosestMonitor.Text = "Closest Monitor";
			this.rbtnClosestMonitor.UseVisualStyleBackColor = true;
			// 
			// lblPollutant
			// 
			this.lblPollutant.AutoSize = true;
			this.lblPollutant.Location = new System.Drawing.Point(16, 65);
			this.lblPollutant.Name = "lblPollutant";
			this.lblPollutant.Size = new System.Drawing.Size(60, 14);
			this.lblPollutant.TabIndex = 2;
			this.lblPollutant.Text = "Pollutant:";
			// 
			// lblGridType
			// 
			this.lblGridType.AutoSize = true;
			this.lblGridType.Location = new System.Drawing.Point(16, 30);
			this.lblGridType.Name = "lblGridType";
			this.lblGridType.Size = new System.Drawing.Size(60, 14);
			this.lblGridType.TabIndex = 0;
			this.lblGridType.Text = "Grid Type:";
			// 
			// MonitorData
			// 
			this.AcceptButton = this.btnGo;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(525, 284);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MonitorData";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Monitor Data";
			this.Load += new System.EventHandler(this.MonitorData_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tcMonitorData.ResumeLayout(false);
			this.tpLibrary.ResumeLayout(false);
			this.tpLibrary.PerformLayout();
			this.tpText.ResumeLayout(false);
			this.tpText.PerformLayout();
			this.grpInterpolationMethod.ResumeLayout(false);
			this.grpInterpolationMethod.PerformLayout();
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.Label lblGridType;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox grpInterpolationMethod;
		private System.Windows.Forms.Label lblPollutant;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TabControl tcMonitorData;
		private System.Windows.Forms.TabPage tpLibrary;
		private System.Windows.Forms.ComboBox cboMonitorLibraryYear;
		private System.Windows.Forms.ComboBox cboMonitorDataSet;
		private System.Windows.Forms.Label lblMonitorLibraryYear;
		private System.Windows.Forms.Label lblMonitorDataSet;
		private System.Windows.Forms.TabPage tpText;
		private System.Windows.Forms.TextBox txtRadiums;
		private System.Windows.Forms.RadioButton rbtnFixedRadiums;
		private System.Windows.Forms.RadioButton rbtnVoronoi;
		private System.Windows.Forms.RadioButton rbtnClosestMonitor;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox txtMonitorDataFile;
		private System.Windows.Forms.Label lblMonitorDataFile;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnMap;
		private System.Windows.Forms.Button btnAdvanced;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtPollutant;
		private System.Windows.Forms.TextBox txtGridType;
		private System.Windows.Forms.ComboBox cboMonitorType;
		private System.Windows.Forms.Label label1;
	}
}