namespace BenMAP
{
    partial class LatinHypercubePoints
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
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtRandomSeed = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chbRunInPointMode = new System.Windows.Forms.CheckBox();
            this.lblLatinHypercubePoints = new System.Windows.Forms.Label();
            this.cboLatinHypercubePoints = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
                                                this.txtThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThreshold.Location = new System.Drawing.Point(165, 115);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(98, 22);
            this.txtThreshold.TabIndex = 10;
            this.txtThreshold.TextChanged += new System.EventHandler(this.txtThreshold_TextChanged);
                                                this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(12, 119);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(122, 14);
            this.lblThreshold.TabIndex = 9;
            this.lblThreshold.Text = "Air quality threshold:";
                                                this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtRandomSeed);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtThreshold);
            this.groupBox3.Controls.Add(this.chbRunInPointMode);
            this.groupBox3.Controls.Add(this.lblThreshold);
            this.groupBox3.Controls.Add(this.lblLatinHypercubePoints);
            this.groupBox3.Controls.Add(this.cboLatinHypercubePoints);
            this.groupBox3.Location = new System.Drawing.Point(11, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(281, 158);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
                                                this.txtRandomSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRandomSeed.Location = new System.Drawing.Point(165, 87);
            this.txtRandomSeed.Name = "txtRandomSeed";
            this.txtRandomSeed.Size = new System.Drawing.Size(98, 22);
            this.txtRandomSeed.TabIndex = 14;
            this.txtRandomSeed.Text = "1";
                                                this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 14);
            this.label6.TabIndex = 13;
            this.label6.Text = "Random Seed:";
                                                this.chbRunInPointMode.AutoSize = true;
            this.chbRunInPointMode.Location = new System.Drawing.Point(14, 23);
            this.chbRunInPointMode.Name = "chbRunInPointMode";
            this.chbRunInPointMode.Size = new System.Drawing.Size(126, 16);
            this.chbRunInPointMode.TabIndex = 0;
            this.chbRunInPointMode.Text = "Run In Point Mode";
            this.chbRunInPointMode.UseVisualStyleBackColor = true;
            this.chbRunInPointMode.CheckedChanged += new System.EventHandler(this.chbRunInPointMode_CheckedChanged);
                                                this.lblLatinHypercubePoints.AutoSize = true;
            this.lblLatinHypercubePoints.Location = new System.Drawing.Point(12, 63);
            this.lblLatinHypercubePoints.Name = "lblLatinHypercubePoints";
            this.lblLatinHypercubePoints.Size = new System.Drawing.Size(70, 14);
            this.lblLatinHypercubePoints.TabIndex = 1;
            this.lblLatinHypercubePoints.Text = "Percentiles:";
                                                this.cboLatinHypercubePoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLatinHypercubePoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLatinHypercubePoints.FormattingEnabled = true;
            this.cboLatinHypercubePoints.Items.AddRange(new object[] {
            "10",
            "20",
            "50",
            "100"});
            this.cboLatinHypercubePoints.Location = new System.Drawing.Point(165, 59);
            this.cboLatinHypercubePoints.Name = "cboLatinHypercubePoints";
            this.cboLatinHypercubePoints.Size = new System.Drawing.Size(98, 22);
            this.cboLatinHypercubePoints.TabIndex = 2;
                                                this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(11, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(281, 54);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
                                                this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(198, 18);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 25);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(127, 18);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 233);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LatinHypercubePoints";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Settings";
            this.Load += new System.EventHandler(this.LatinHypercubePoints_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.ComboBox cboLatinHypercubePoints;
        private System.Windows.Forms.Label lblLatinHypercubePoints;
        private System.Windows.Forms.CheckBox chbRunInPointMode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtThreshold;
        private System.Windows.Forms.TextBox txtRandomSeed;
        private System.Windows.Forms.Label label6;
    }
}