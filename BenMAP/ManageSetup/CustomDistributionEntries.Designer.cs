namespace BenMAP
{
    partial class CustomDistributionEntries
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
            this.lstCurrentEntries = new System.Windows.Forms.ListBox();
            this.btnLoadFromDatabase = new System.Windows.Forms.Button();
            this.btnLoadFromTextFile = new System.Windows.Forms.Button();
            this.txtCurrentStandard = new System.Windows.Forms.TextBox();
            this.txtCurrentMean = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
                                                this.groupBox1.Controls.Add(this.lstCurrentEntries);
            this.groupBox1.Controls.Add(this.btnLoadFromDatabase);
            this.groupBox1.Controls.Add(this.btnLoadFromTextFile);
            this.groupBox1.Controls.Add(this.txtCurrentStandard);
            this.groupBox1.Controls.Add(this.txtCurrentMean);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 236);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
                                                this.lstCurrentEntries.FormattingEnabled = true;
            this.lstCurrentEntries.ItemHeight = 14;
            this.lstCurrentEntries.Location = new System.Drawing.Point(9, 42);
            this.lstCurrentEntries.Name = "lstCurrentEntries";
            this.lstCurrentEntries.Size = new System.Drawing.Size(156, 186);
            this.lstCurrentEntries.TabIndex = 8;
                                                this.btnLoadFromDatabase.Enabled = false;
            this.btnLoadFromDatabase.Location = new System.Drawing.Point(173, 202);
            this.btnLoadFromDatabase.Name = "btnLoadFromDatabase";
            this.btnLoadFromDatabase.Size = new System.Drawing.Size(161, 27);
            this.btnLoadFromDatabase.TabIndex = 7;
            this.btnLoadFromDatabase.Text = "Load From Database";
            this.btnLoadFromDatabase.UseVisualStyleBackColor = true;
                                                this.btnLoadFromTextFile.Location = new System.Drawing.Point(173, 155);
            this.btnLoadFromTextFile.Name = "btnLoadFromTextFile";
            this.btnLoadFromTextFile.Size = new System.Drawing.Size(161, 27);
            this.btnLoadFromTextFile.TabIndex = 6;
            this.btnLoadFromTextFile.Text = "Load From *.csv File";
            this.btnLoadFromTextFile.UseVisualStyleBackColor = true;
            this.btnLoadFromTextFile.Click += new System.EventHandler(this.btnLoadFromTextFile_Click);
                                                this.txtCurrentStandard.Location = new System.Drawing.Point(173, 108);
            this.txtCurrentStandard.Name = "txtCurrentStandard";
            this.txtCurrentStandard.Size = new System.Drawing.Size(161, 22);
            this.txtCurrentStandard.TabIndex = 5;
                                                this.txtCurrentMean.Location = new System.Drawing.Point(173, 43);
            this.txtCurrentMean.Name = "txtCurrentMean";
            this.txtCurrentMean.Size = new System.Drawing.Size(161, 22);
            this.txtCurrentMean.TabIndex = 4;
                                                this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(171, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Current Standard Deviation:";
                                                this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(171, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Current Mean:";
                                                this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Entries:";
                                                this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(7, 238);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(345, 63);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
                                                this.btnOK.Location = new System.Drawing.Point(259, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Location = new System.Drawing.Point(160, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 309);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(368, 337);
            this.Name = "CustomDistributionEntries";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Distribution Entries";
            this.Load += new System.EventHandler(this.CustomDistributionEntries_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnLoadFromDatabase;
        private System.Windows.Forms.Button btnLoadFromTextFile;
        private System.Windows.Forms.TextBox txtCurrentStandard;
        private System.Windows.Forms.TextBox txtCurrentMean;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lstCurrentEntries;
    }
}