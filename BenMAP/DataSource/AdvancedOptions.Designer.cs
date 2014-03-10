namespace BenMAP
{
    partial class AdvancedOptions
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
            this.lblMaximumNeighborDistance = new System.Windows.Forms.Label();
            this.lblMaximumRelativeNeighbor = new System.Windows.Forms.Label();
            this.txtMaximumNeighborDistance = new System.Windows.Forms.TextBox();
            this.txtMaximumRelativeNeighborDistance = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCustomMonitor = new System.Windows.Forms.Button();
            this.chbGetClosest = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnInverseDistanceSquared = new System.Windows.Forms.RadioButton();
            this.rbtnInverseDistance = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
                                                this.lblMaximumNeighborDistance.AutoSize = true;
            this.lblMaximumNeighborDistance.Location = new System.Drawing.Point(22, 20);
            this.lblMaximumNeighborDistance.Name = "lblMaximumNeighborDistance";
            this.lblMaximumNeighborDistance.Size = new System.Drawing.Size(208, 14);
            this.lblMaximumNeighborDistance.TabIndex = 0;
            this.lblMaximumNeighborDistance.Text = "Maximum Neighbor Distance (in km):";
                                                this.lblMaximumRelativeNeighbor.AutoSize = true;
            this.lblMaximumRelativeNeighbor.Location = new System.Drawing.Point(22, 80);
            this.lblMaximumRelativeNeighbor.Name = "lblMaximumRelativeNeighbor";
            this.lblMaximumRelativeNeighbor.Size = new System.Drawing.Size(214, 14);
            this.lblMaximumRelativeNeighbor.TabIndex = 1;
            this.lblMaximumRelativeNeighbor.Text = "Maximum Relative Neighbor Distance:";
                                                this.txtMaximumNeighborDistance.Location = new System.Drawing.Point(24, 41);
            this.txtMaximumNeighborDistance.Name = "txtMaximumNeighborDistance";
            this.txtMaximumNeighborDistance.Size = new System.Drawing.Size(213, 22);
            this.txtMaximumNeighborDistance.TabIndex = 2;
            this.txtMaximumNeighborDistance.Text = "- No Maximum Distance -";
            this.txtMaximumNeighborDistance.Enter += new System.EventHandler(this.txtMaximumNeighborDistance_Enter);
            this.txtMaximumNeighborDistance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaximumNeighborDistance_KeyPress);
            this.txtMaximumNeighborDistance.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtMaximumNeighborDistance_MouseUp);
                                                this.txtMaximumRelativeNeighborDistance.Location = new System.Drawing.Point(24, 98);
            this.txtMaximumRelativeNeighborDistance.Name = "txtMaximumRelativeNeighborDistance";
            this.txtMaximumRelativeNeighborDistance.Size = new System.Drawing.Size(213, 22);
            this.txtMaximumRelativeNeighborDistance.TabIndex = 3;
            this.txtMaximumRelativeNeighborDistance.Text = "- No Maximum Relative Distance -";
            this.txtMaximumRelativeNeighborDistance.Enter += new System.EventHandler(this.txtMaximumRelativeNeighborDistance_Enter);
            this.txtMaximumRelativeNeighborDistance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaximumRelativeNeighborDistance_KeyPress);
            this.txtMaximumRelativeNeighborDistance.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtMaximumRelativeNeighborDistance_MouseUp);
                                                this.groupBox1.Controls.Add(this.btnCustomMonitor);
            this.groupBox1.Controls.Add(this.chbGetClosest);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.lblMaximumNeighborDistance);
            this.groupBox1.Controls.Add(this.txtMaximumRelativeNeighborDistance);
            this.groupBox1.Controls.Add(this.txtMaximumNeighborDistance);
            this.groupBox1.Controls.Add(this.lblMaximumRelativeNeighbor);
            this.groupBox1.Location = new System.Drawing.Point(12, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(468, 191);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
                                                this.btnCustomMonitor.Location = new System.Drawing.Point(263, 149);
            this.btnCustomMonitor.Name = "btnCustomMonitor";
            this.btnCustomMonitor.Size = new System.Drawing.Size(199, 27);
            this.btnCustomMonitor.TabIndex = 6;
            this.btnCustomMonitor.Text = "Custom Monitor Filtering";
            this.btnCustomMonitor.UseVisualStyleBackColor = true;
            this.btnCustomMonitor.Click += new System.EventHandler(this.btnCustomMonitor_Click);
                                                this.chbGetClosest.AutoSize = true;
            this.chbGetClosest.Location = new System.Drawing.Point(24, 154);
            this.chbGetClosest.Name = "chbGetClosest";
            this.chbGetClosest.Size = new System.Drawing.Size(222, 16);
            this.chbGetClosest.TabIndex = 5;
            this.chbGetClosest.Text = "Get Closest if None within Radius";
            this.chbGetClosest.UseVisualStyleBackColor = true;
                                                this.groupBox2.Controls.Add(this.rbtnInverseDistanceSquared);
            this.groupBox2.Controls.Add(this.rbtnInverseDistance);
            this.groupBox2.Location = new System.Drawing.Point(262, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 99);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Weighting Approach";
                                                this.rbtnInverseDistanceSquared.AutoSize = true;
            this.rbtnInverseDistanceSquared.Location = new System.Drawing.Point(7, 75);
            this.rbtnInverseDistanceSquared.Name = "rbtnInverseDistanceSquared";
            this.rbtnInverseDistanceSquared.Size = new System.Drawing.Size(167, 16);
            this.rbtnInverseDistanceSquared.TabIndex = 1;
            this.rbtnInverseDistanceSquared.Tag = "inversedistancesquared";
            this.rbtnInverseDistanceSquared.Text = "Inverse Distance Squared";
            this.rbtnInverseDistanceSquared.UseVisualStyleBackColor = true;
                                                this.rbtnInverseDistance.AutoSize = true;
            this.rbtnInverseDistance.Checked = true;
            this.rbtnInverseDistance.Location = new System.Drawing.Point(7, 22);
            this.rbtnInverseDistance.Name = "rbtnInverseDistance";
            this.rbtnInverseDistance.Size = new System.Drawing.Size(119, 16);
            this.rbtnInverseDistance.TabIndex = 0;
            this.rbtnInverseDistance.TabStop = true;
            this.rbtnInverseDistance.Tag = "inversedistance";
            this.rbtnInverseDistance.Text = "Inverse Distance";
            this.rbtnInverseDistance.UseVisualStyleBackColor = true;
                                                this.groupBox3.Controls.Add(this.btnOK);
            this.groupBox3.Controls.Add(this.btnCancel);
            this.groupBox3.Location = new System.Drawing.Point(12, 199);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(468, 62);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
                                                this.btnOK.Location = new System.Drawing.Point(387, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Location = new System.Drawing.Point(294, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 272);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AdvancedOptions";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Options";
            this.Load += new System.EventHandler(this.AdvancedOptions_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label lblMaximumNeighborDistance;
        private System.Windows.Forms.Label lblMaximumRelativeNeighbor;
        private System.Windows.Forms.TextBox txtMaximumNeighborDistance;
        private System.Windows.Forms.TextBox txtMaximumRelativeNeighborDistance;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCustomMonitor;
        private System.Windows.Forms.CheckBox chbGetClosest;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbtnInverseDistanceSquared;
        private System.Windows.Forms.RadioButton rbtnInverseDistance;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}