namespace BenMAP
{
	partial class MonitorRollbackSettings3
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
			this.btnBrowse = new System.Windows.Forms.Button();
			this.chbMakeBaselineGrid = new System.Windows.Forms.CheckBox();
			this.txtAdjustment = new System.Windows.Forms.TextBox();
			this.cboGridType = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.grbScaling = new System.Windows.Forms.GroupBox();
			this.rbtnSpatialOnly = new System.Windows.Forms.RadioButton();
			this.rbtnNone = new System.Windows.Forms.RadioButton();
			this.grpInterpolation = new System.Windows.Forms.GroupBox();
			this.txtFixRadio = new System.Windows.Forms.TextBox();
			this.rbtnFixedRadius = new System.Windows.Forms.RadioButton();
			this.rbtnVoronoiNeighborhood = new System.Windows.Forms.RadioButton();
			this.rbtnClosestMonitor = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.btnGo = new System.Windows.Forms.Button();
			this.btnAdvanced = new System.Windows.Forms.Button();
			this.btnBack = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.grbScaling.SuspendLayout();
			this.grpInterpolation.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnBrowse);
			this.groupBox1.Controls.Add(this.chbMakeBaselineGrid);
			this.groupBox1.Controls.Add(this.txtAdjustment);
			this.groupBox1.Controls.Add(this.cboGridType);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.grbScaling);
			this.groupBox1.Controls.Add(this.grpInterpolation);
			this.groupBox1.Location = new System.Drawing.Point(12, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(417, 272);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(329, 187);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(69, 27);
			this.btnBrowse.TabIndex = 6;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// chbMakeBaselineGrid
			// 
			this.chbMakeBaselineGrid.AutoSize = true;
			this.chbMakeBaselineGrid.Checked = true;
			this.chbMakeBaselineGrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbMakeBaselineGrid.Location = new System.Drawing.Point(13, 237);
			this.chbMakeBaselineGrid.Name = "chbMakeBaselineGrid";
			this.chbMakeBaselineGrid.Size = new System.Drawing.Size(290, 18);
			this.chbMakeBaselineGrid.TabIndex = 7;
			this.chbMakeBaselineGrid.Text = "Make Baseline Grid (in addition to Control Grid).";
			this.chbMakeBaselineGrid.UseVisualStyleBackColor = true;
			this.chbMakeBaselineGrid.CheckedChanged += new System.EventHandler(this.chbMakeBaselineGrid_CheckedChanged);
			// 
			// txtAdjustment
			// 
			this.txtAdjustment.Location = new System.Drawing.Point(84, 189);
			this.txtAdjustment.Name = "txtAdjustment";
			this.txtAdjustment.ReadOnly = true;
			this.txtAdjustment.Size = new System.Drawing.Size(242, 22);
			this.txtAdjustment.TabIndex = 5;
			// 
			// cboGridType
			// 
			this.cboGridType.FormattingEnabled = true;
			this.cboGridType.Location = new System.Drawing.Point(84, 149);
			this.cboGridType.Name = "cboGridType";
			this.cboGridType.Size = new System.Drawing.Size(303, 22);
			this.cboGridType.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 192);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 14);
			this.label2.TabIndex = 3;
			this.label2.Text = "Adjustment:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 153);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 14);
			this.label1.TabIndex = 2;
			this.label1.Text = "Grid Type:";
			// 
			// grbScaling
			// 
			this.grbScaling.Controls.Add(this.rbtnSpatialOnly);
			this.grbScaling.Controls.Add(this.rbtnNone);
			this.grbScaling.Location = new System.Drawing.Point(224, 23);
			this.grbScaling.Name = "grbScaling";
			this.grbScaling.Size = new System.Drawing.Size(174, 111);
			this.grbScaling.TabIndex = 1;
			this.grbScaling.TabStop = false;
			this.grbScaling.Text = "Select Scaling Method";
			this.grbScaling.Visible = false;
			// 
			// rbtnSpatialOnly
			// 
			this.rbtnSpatialOnly.AutoSize = true;
			this.rbtnSpatialOnly.Location = new System.Drawing.Point(7, 78);
			this.rbtnSpatialOnly.Name = "rbtnSpatialOnly";
			this.rbtnSpatialOnly.Size = new System.Drawing.Size(90, 18);
			this.rbtnSpatialOnly.TabIndex = 1;
			this.rbtnSpatialOnly.TabStop = true;
			this.rbtnSpatialOnly.Tag = "spatial";
			this.rbtnSpatialOnly.Text = "Spatial Only";
			this.rbtnSpatialOnly.UseVisualStyleBackColor = true;
			this.rbtnSpatialOnly.Click += new System.EventHandler(this.rbtnSpatialOnly_Click);
			// 
			// rbtnNone
			// 
			this.rbtnNone.AutoSize = true;
			this.rbtnNone.Location = new System.Drawing.Point(7, 23);
			this.rbtnNone.Name = "rbtnNone";
			this.rbtnNone.Size = new System.Drawing.Size(54, 18);
			this.rbtnNone.TabIndex = 0;
			this.rbtnNone.TabStop = true;
			this.rbtnNone.Tag = "none";
			this.rbtnNone.Text = "None";
			this.rbtnNone.UseVisualStyleBackColor = true;
			this.rbtnNone.CheckedChanged += new System.EventHandler(this.rbtnNone_CheckedChanged);
			// 
			// grpInterpolation
			// 
			this.grpInterpolation.Controls.Add(this.txtFixRadio);
			this.grpInterpolation.Controls.Add(this.rbtnFixedRadius);
			this.grpInterpolation.Controls.Add(this.rbtnVoronoiNeighborhood);
			this.grpInterpolation.Controls.Add(this.rbtnClosestMonitor);
			this.grpInterpolation.Location = new System.Drawing.Point(6, 23);
			this.grpInterpolation.Name = "grpInterpolation";
			this.grpInterpolation.Size = new System.Drawing.Size(392, 111);
			this.grpInterpolation.TabIndex = 0;
			this.grpInterpolation.TabStop = false;
			this.grpInterpolation.Text = "Select Interpolation Method";
			// 
			// txtFixRadio
			// 
			this.txtFixRadio.Location = new System.Drawing.Point(132, 74);
			this.txtFixRadio.Name = "txtFixRadio";
			this.txtFixRadio.Size = new System.Drawing.Size(69, 22);
			this.txtFixRadio.TabIndex = 3;
			this.txtFixRadio.Click += new System.EventHandler(this.txtFixRadio_Click);
			this.txtFixRadio.TextChanged += new System.EventHandler(this.txtFixRadio_TextChanged);
			this.txtFixRadio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFixRadio_KeyPress);
			// 
			// rbtnFixedRadius
			// 
			this.rbtnFixedRadius.AutoSize = true;
			this.rbtnFixedRadius.Location = new System.Drawing.Point(7, 76);
			this.rbtnFixedRadius.Name = "rbtnFixedRadius";
			this.rbtnFixedRadius.Size = new System.Drawing.Size(122, 18);
			this.rbtnFixedRadius.TabIndex = 2;
			this.rbtnFixedRadius.TabStop = true;
			this.rbtnFixedRadius.Tag = "fixedradius";
			this.rbtnFixedRadius.Text = "Fixed Radius (km)";
			this.rbtnFixedRadius.UseVisualStyleBackColor = true;
			this.rbtnFixedRadius.CheckedChanged += new System.EventHandler(this.rbtnFixedRadius_CheckedChanged);
			// 
			// rbtnVoronoiNeighborhood
			// 
			this.rbtnVoronoiNeighborhood.AutoSize = true;
			this.rbtnVoronoiNeighborhood.Location = new System.Drawing.Point(7, 50);
			this.rbtnVoronoiNeighborhood.Name = "rbtnVoronoiNeighborhood";
			this.rbtnVoronoiNeighborhood.Size = new System.Drawing.Size(204, 18);
			this.rbtnVoronoiNeighborhood.TabIndex = 1;
			this.rbtnVoronoiNeighborhood.TabStop = true;
			this.rbtnVoronoiNeighborhood.Tag = "voronoineighborhoodaveraging";
			this.rbtnVoronoiNeighborhood.Text = "Voronoi Neighborhood Averaging";
			this.rbtnVoronoiNeighborhood.UseVisualStyleBackColor = true;
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
			this.rbtnClosestMonitor.CheckedChanged += new System.EventHandler(this.rbtnClosestMonitor_CheckedChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.btnGo);
			this.groupBox4.Controls.Add(this.btnAdvanced);
			this.groupBox4.Controls.Add(this.btnBack);
			this.groupBox4.Location = new System.Drawing.Point(12, 281);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(417, 55);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(336, 17);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(75, 27);
			this.btnGo.TabIndex = 2;
			this.btnGo.Text = "Go";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// btnAdvanced
			// 
			this.btnAdvanced.Location = new System.Drawing.Point(255, 17);
			this.btnAdvanced.Name = "btnAdvanced";
			this.btnAdvanced.Size = new System.Drawing.Size(75, 27);
			this.btnAdvanced.TabIndex = 1;
			this.btnAdvanced.Text = "Advanced";
			this.btnAdvanced.UseVisualStyleBackColor = true;
			this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
			// 
			// btnBack
			// 
			this.btnBack.Location = new System.Drawing.Point(174, 17);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(75, 27);
			this.btnBack.TabIndex = 0;
			this.btnBack.Text = "Back";
			this.btnBack.UseVisualStyleBackColor = true;
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			// 
			// MonitorRollbackSettings3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(441, 341);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MonitorRollbackSettings3";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Monitor Rollback Settings: (3) Additional Grid Settings";
			this.Load += new System.EventHandler(this.MonitorRollbackSettings3_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.grbScaling.ResumeLayout(false);
			this.grbScaling.PerformLayout();
			this.grpInterpolation.ResumeLayout(false);
			this.grpInterpolation.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.CheckBox chbMakeBaselineGrid;
		private System.Windows.Forms.TextBox txtAdjustment;
		private System.Windows.Forms.ComboBox cboGridType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox grbScaling;
		private System.Windows.Forms.RadioButton rbtnSpatialOnly;
		private System.Windows.Forms.RadioButton rbtnNone;
		private System.Windows.Forms.GroupBox grpInterpolation;
		private System.Windows.Forms.RadioButton rbtnFixedRadius;
		private System.Windows.Forms.RadioButton rbtnVoronoiNeighborhood;
		private System.Windows.Forms.RadioButton rbtnClosestMonitor;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnAdvanced;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.TextBox txtFixRadio;
	}
}