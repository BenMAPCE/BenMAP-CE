namespace BenMAP
{
    partial class DatabaseExport
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
            this.treDatabase = new System.Windows.Forms.TreeView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pBarExport = new System.Windows.Forms.ProgressBar();
            this.lbProcess = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioExportTypeFile = new System.Windows.Forms.RadioButton();
            this.radioExportTypeDb = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treDatabase
            // 
            this.treDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treDatabase.Location = new System.Drawing.Point(3, 18);
            this.treDatabase.Name = "treDatabase";
            this.treDatabase.Size = new System.Drawing.Size(375, 313);
            this.treDatabase.TabIndex = 0;
            this.treDatabase.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treDatabase_AfterSelect);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(233, 422);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(309, 422);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.treDatabase);
            this.groupBox1.Location = new System.Drawing.Point(1, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 334);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Object to Export:";
            // 
            // pBarExport
            // 
            this.pBarExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBarExport.Location = new System.Drawing.Point(4, 404);
            this.pBarExport.Name = "pBarExport";
            this.pBarExport.Size = new System.Drawing.Size(375, 12);
            this.pBarExport.Step = 1;
            this.pBarExport.TabIndex = 2;
            // 
            // lbProcess
            // 
            this.lbProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbProcess.AutoSize = true;
            this.lbProcess.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProcess.ForeColor = System.Drawing.Color.Black;
            this.lbProcess.Location = new System.Drawing.Point(1, 428);
            this.lbProcess.Name = "lbProcess";
            this.lbProcess.Size = new System.Drawing.Size(0, 14);
            this.lbProcess.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.radioExportTypeFile);
            this.groupBox2.Controls.Add(this.radioExportTypeDb);
            this.groupBox2.Location = new System.Drawing.Point(1, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(381, 50);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select Type of Export:";
            // 
            // radioExportTypeFile
            // 
            this.radioExportTypeFile.AutoSize = true;
            this.radioExportTypeFile.Location = new System.Drawing.Point(151, 21);
            this.radioExportTypeFile.Name = "radioExportTypeFile";
            this.radioExportTypeFile.Size = new System.Drawing.Size(195, 18);
            this.radioExportTypeFile.TabIndex = 1;
            this.radioExportTypeFile.Text = "Other File Format (e.g. shp, csv)";
            this.radioExportTypeFile.UseVisualStyleBackColor = true;
            this.radioExportTypeFile.CheckedChanged += new System.EventHandler(this.radioExportTypeFile_CheckedChanged);
            // 
            // radioExportTypeDb
            // 
            this.radioExportTypeDb.AutoSize = true;
            this.radioExportTypeDb.Checked = true;
            this.radioExportTypeDb.Location = new System.Drawing.Point(5, 21);
            this.radioExportTypeDb.Name = "radioExportTypeDb";
            this.radioExportTypeDb.Size = new System.Drawing.Size(140, 18);
            this.radioExportTypeDb.TabIndex = 0;
            this.radioExportTypeDb.TabStop = true;
            this.radioExportTypeDb.Text = "BenMAP CE Database";
            this.radioExportTypeDb.UseVisualStyleBackColor = true;
            this.radioExportTypeDb.CheckedChanged += new System.EventHandler(this.radioExportType_CheckedChanged);
            // 
            // DatabaseExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lbProcess);
            this.Controls.Add(this.pBarExport);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Name = "DatabaseExport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database Export";
            this.Load += new System.EventHandler(this.DatabaseExport_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.TreeView treDatabase;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar pBarExport;
        private System.Windows.Forms.Label lbProcess;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioExportTypeFile;
        private System.Windows.Forms.RadioButton radioExportTypeDb;
    }
}