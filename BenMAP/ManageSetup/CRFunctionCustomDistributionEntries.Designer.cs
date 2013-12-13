namespace BenMAP
{
    partial class CRFunctionCustomDistributionEntries
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CRFunctionCustomDistributionEntries));
            this.label1 = new System.Windows.Forms.Label();
            this.lstCurrentEntries = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCurrentMean = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCurrentStandard = new System.Windows.Forms.TextBox();
            this.btnLoadFromTextFile = new System.Windows.Forms.Button();
            this.btnLoadFromDatabase = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current Entries:";
            // 
            // lstCurrentEntries
            // 
            this.lstCurrentEntries.FormattingEnabled = true;
            this.lstCurrentEntries.ItemHeight = 12;
            this.lstCurrentEntries.Location = new System.Drawing.Point(6, 32);
            this.lstCurrentEntries.Name = "lstCurrentEntries";
            this.lstCurrentEntries.Size = new System.Drawing.Size(129, 160);
            this.lstCurrentEntries.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Current Mean:";
            // 
            // txtCurrentMean
            // 
            this.txtCurrentMean.Location = new System.Drawing.Point(167, 32);
            this.txtCurrentMean.Name = "txtCurrentMean";
            this.txtCurrentMean.Size = new System.Drawing.Size(161, 21);
            this.txtCurrentMean.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(165, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "Current Standard Deviation:";
            // 
            // txtCurrentStandard
            // 
            this.txtCurrentStandard.Location = new System.Drawing.Point(167, 84);
            this.txtCurrentStandard.Name = "txtCurrentStandard";
            this.txtCurrentStandard.Size = new System.Drawing.Size(161, 21);
            this.txtCurrentStandard.TabIndex = 13;
            // 
            // btnLoadFromTextFile
            // 
            this.btnLoadFromTextFile.Location = new System.Drawing.Point(167, 130);
            this.btnLoadFromTextFile.Name = "btnLoadFromTextFile";
            this.btnLoadFromTextFile.Size = new System.Drawing.Size(161, 23);
            this.btnLoadFromTextFile.TabIndex = 14;
            this.btnLoadFromTextFile.Text = "Load From *.csv File";
            this.btnLoadFromTextFile.UseVisualStyleBackColor = true;
            this.btnLoadFromTextFile.Click += new System.EventHandler(this.btnLoadFromTextFile_Click);
            // 
            // btnLoadFromDatabase
            // 
            this.btnLoadFromDatabase.Location = new System.Drawing.Point(167, 169);
            this.btnLoadFromDatabase.Name = "btnLoadFromDatabase";
            this.btnLoadFromDatabase.Size = new System.Drawing.Size(161, 23);
            this.btnLoadFromDatabase.TabIndex = 15;
            this.btnLoadFromDatabase.Text = "Load From Database";
            this.btnLoadFromDatabase.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(154, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(252, 20);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lstCurrentEntries);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCurrentMean);
            this.groupBox1.Controls.Add(this.btnLoadFromTextFile);
            this.groupBox1.Controls.Add(this.btnLoadFromDatabase);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtCurrentStandard);
            this.groupBox1.Location = new System.Drawing.Point(11, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 200);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(12, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(339, 52);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // CRFunctionCustomDistributionEntries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 261);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CRFunctionCustomDistributionEntries";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Distribution Entries";
            this.Load += new System.EventHandler(this.CRFunctionCustomDistributionEntries_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstCurrentEntries;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCurrentMean;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCurrentStandard;
        private System.Windows.Forms.Button btnLoadFromTextFile;
        private System.Windows.Forms.Button btnLoadFromDatabase;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}