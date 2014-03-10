namespace WinControls
{
    partial class SetValueRange
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.lblUnit1 = new System.Windows.Forms.Label();
            this.lblUnit2 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.dgvSetColor = new System.Windows.Forms.DataGridView();
            this.grbSetValueRange = new System.Windows.Forms.GroupBox();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.gbSetColor = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSetColor)).BeginInit();
            this.grbSetValueRange.SuspendLayout();
            this.gbSetColor.SuspendLayout();
            this.SuspendLayout();
                                                this.btnOK.Location = new System.Drawing.Point(411, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Location = new System.Drawing.Point(480, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min Value:";
                                                this.txtMin.Location = new System.Drawing.Point(68, 22);
            this.txtMin.Name = "txtMin";
            this.txtMin.Size = new System.Drawing.Size(83, 21);
            this.txtMin.TabIndex = 1;
                                                this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max Value:";
                                                this.txtMax.Location = new System.Drawing.Point(303, 22);
            this.txtMax.Name = "txtMax";
            this.txtMax.Size = new System.Drawing.Size(83, 21);
            this.txtMax.TabIndex = 5;
                                                this.lblUnit1.AutoSize = true;
            this.lblUnit1.Location = new System.Drawing.Point(156, 26);
            this.lblUnit1.Name = "lblUnit1";
            this.lblUnit1.Size = new System.Drawing.Size(17, 12);
            this.lblUnit1.TabIndex = 2;
            this.lblUnit1.Text = "  ";
            this.lblUnit1.Visible = false;
                                                this.lblUnit2.AutoSize = true;
            this.lblUnit2.Location = new System.Drawing.Point(390, 26);
            this.lblUnit2.Name = "lblUnit2";
            this.lblUnit2.Size = new System.Drawing.Size(17, 12);
            this.lblUnit2.TabIndex = 6;
            this.lblUnit2.Text = "  ";
            this.lblUnit2.Visible = false;
                                                this.btnReset.Location = new System.Drawing.Point(278, 246);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(117, 23);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "&Reset to Defaults";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
                                                this.dgvSetColor.AllowUserToAddRows = false;
            this.dgvSetColor.AllowUserToDeleteRows = false;
            this.dgvSetColor.AllowUserToResizeColumns = false;
            this.dgvSetColor.AllowUserToResizeRows = false;
            this.dgvSetColor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dgvSetColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvSetColor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSetColor.ColumnHeadersVisible = false;
            this.dgvSetColor.EnableHeadersVisualStyles = false;
            this.dgvSetColor.Location = new System.Drawing.Point(6, 20);
            this.dgvSetColor.MultiSelect = false;
            this.dgvSetColor.Name = "dgvSetColor";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSetColor.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSetColor.RowHeadersWidth = 146;
            this.dgvSetColor.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvSetColor.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dgvSetColor.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Transparent;
            this.dgvSetColor.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Transparent;
            this.dgvSetColor.RowTemplate.Height = 23;
            this.dgvSetColor.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvSetColor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvSetColor.ShowCellErrors = false;
            this.dgvSetColor.ShowEditingIcon = false;
            this.dgvSetColor.Size = new System.Drawing.Size(538, 69);
            this.dgvSetColor.TabIndex = 5;
            this.dgvSetColor.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSetColor_CellDoubleClick);
                                                this.grbSetValueRange.Controls.Add(this.btnRebuild);
            this.grbSetValueRange.Controls.Add(this.txtMin);
            this.grbSetValueRange.Controls.Add(this.label1);
            this.grbSetValueRange.Controls.Add(this.txtMax);
            this.grbSetValueRange.Controls.Add(this.lblUnit1);
            this.grbSetValueRange.Controls.Add(this.label2);
            this.grbSetValueRange.Controls.Add(this.lblUnit2);
            this.grbSetValueRange.Location = new System.Drawing.Point(8, 19);
            this.grbSetValueRange.Name = "grbSetValueRange";
            this.grbSetValueRange.Size = new System.Drawing.Size(552, 58);
            this.grbSetValueRange.TabIndex = 0;
            this.grbSetValueRange.TabStop = false;
            this.grbSetValueRange.Text = "Set Value Range";
                                                this.btnRebuild.Location = new System.Drawing.Point(457, 21);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(75, 23);
            this.btnRebuild.TabIndex = 6;
            this.btnRebuild.Text = "Rebuild";
            this.toolTip1.SetToolTip(this.btnRebuild, "Click button to rebuild legend using values between \"Min Value\" and \"Max Value\".");
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
                                                this.gbSetColor.Controls.Add(this.label3);
            this.gbSetColor.Controls.Add(this.dgvSetColor);
            this.gbSetColor.Location = new System.Drawing.Point(8, 94);
            this.gbSetColor.Name = "gbSetColor";
            this.gbSetColor.Size = new System.Drawing.Size(552, 127);
            this.gbSetColor.TabIndex = 7;
            this.gbSetColor.TabStop = false;
            this.gbSetColor.Text = "Set Colors and Boundary Values";
                                                this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label3.Location = new System.Drawing.Point(6, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(395, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "Double click  a color cell to customize that cell\'s color.\r\n    Single click a bounda" +
                "ry value cell to change the boundary value.";
                                                this.toolTip1.Tag = "btnRebuild";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 282);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.gbSetColor);
            this.Controls.Add(this.grbSetValueRange);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetValueRange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Value Range";
            this.Load += new System.EventHandler(this.SetValueRange_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSetColor)).EndInit();
            this.grbSetValueRange.ResumeLayout(false);
            this.grbSetValueRange.PerformLayout();
            this.gbSetColor.ResumeLayout(false);
            this.gbSetColor.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMax;
        private System.Windows.Forms.Label lblUnit1;
        private System.Windows.Forms.Label lblUnit2;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.DataGridView dgvSetColor;
        private System.Windows.Forms.GroupBox grbSetValueRange;
        private System.Windows.Forms.GroupBox gbSetColor;
        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}