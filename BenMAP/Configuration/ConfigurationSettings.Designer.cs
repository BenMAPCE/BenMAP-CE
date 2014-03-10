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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LatinHypercubePoints));
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chbRunInPointMode = new System.Windows.Forms.CheckBox();
            this.lblLatinHypercubePoints = new System.Windows.Forms.Label();
            this.cboLatinHypercubePoints = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
                                                this.txtThreshold.Location = new System.Drawing.Point(166, 78);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(134, 21);
            this.txtThreshold.TabIndex = 10;
                                                this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(12, 81);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(65, 12);
            this.lblThreshold.TabIndex = 9;
            this.lblThreshold.Text = "Threshold:";
                                                this.groupBox3.Controls.Add(this.txtThreshold);
            this.groupBox3.Controls.Add(this.chbRunInPointMode);
            this.groupBox3.Controls.Add(this.lblThreshold);
            this.groupBox3.Controls.Add(this.lblLatinHypercubePoints);
            this.groupBox3.Controls.Add(this.cboLatinHypercubePoints);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(308, 114);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
                                                this.chbRunInPointMode.AutoSize = true;
            this.chbRunInPointMode.Location = new System.Drawing.Point(14, 20);
            this.chbRunInPointMode.Name = "chbRunInPointMode";
            this.chbRunInPointMode.Size = new System.Drawing.Size(126, 16);
            this.chbRunInPointMode.TabIndex = 0;
            this.chbRunInPointMode.Text = "Run In Point Mode";
            this.chbRunInPointMode.UseVisualStyleBackColor = true;
                                                this.lblLatinHypercubePoints.AutoSize = true;
            this.lblLatinHypercubePoints.Location = new System.Drawing.Point(12, 48);
            this.lblLatinHypercubePoints.Name = "lblLatinHypercubePoints";
            this.lblLatinHypercubePoints.Size = new System.Drawing.Size(137, 12);
            this.lblLatinHypercubePoints.TabIndex = 1;
            this.lblLatinHypercubePoints.Text = "Latin Hypercube Points";
                                                this.cboLatinHypercubePoints.FormattingEnabled = true;
            this.cboLatinHypercubePoints.Location = new System.Drawing.Point(166, 45);
            this.cboLatinHypercubePoints.Name = "cboLatinHypercubePoints";
            this.cboLatinHypercubePoints.Size = new System.Drawing.Size(134, 20);
            this.cboLatinHypercubePoints.TabIndex = 2;
                                                this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(12, 132);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(308, 60);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
                                                this.btnOK.Location = new System.Drawing.Point(235, 20);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
                                                this.btnCancel.Location = new System.Drawing.Point(153, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 206);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LatinHypercubePoints";
            this.Text = "Latin Hypercube Points";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboLatinHypercubePoints;
        private System.Windows.Forms.Label lblLatinHypercubePoints;
        private System.Windows.Forms.CheckBox chbRunInPointMode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtThreshold;
    }
}