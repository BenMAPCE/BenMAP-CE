namespace BenMAP
{
    partial class LoadInflationDataSet
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
            this.lblInflationDataSetName = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtInflationDataSetName = new System.Windows.Forms.TextBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.btnViewMetadata = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpCancelOK.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInflationDataSetName
            // 
            this.lblInflationDataSetName.AutoSize = true;
            this.lblInflationDataSetName.Location = new System.Drawing.Point(6, 18);
            this.lblInflationDataSetName.Name = "lblInflationDataSetName";
            this.lblInflationDataSetName.Size = new System.Drawing.Size(136, 14);
            this.lblInflationDataSetName.TabIndex = 0;
            this.lblInflationDataSetName.Text = "Inflation Dataset Name:";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(6, 68);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(63, 14);
            this.lblDatabase.TabIndex = 1;
            this.lblDatabase.Text = "Database:";
            // 
            // txtInflationDataSetName
            // 
            this.txtInflationDataSetName.Location = new System.Drawing.Point(9, 35);
            this.txtInflationDataSetName.Name = "txtInflationDataSetName";
            this.txtInflationDataSetName.Size = new System.Drawing.Size(386, 22);
            this.txtInflationDataSetName.TabIndex = 2;
            this.txtInflationDataSetName.Text = "InflationDataSet0";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Enabled = false;
            this.txtDatabase.Location = new System.Drawing.Point(9, 84);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(305, 22);
            this.txtDatabase.TabIndex = 3;
            this.txtDatabase.TextChanged += new System.EventHandler(this.txtDatabase_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(319, 82);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(227, 18);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(318, 18);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Controls.Add(this.btnViewMetadata);
            this.grpCancelOK.Controls.Add(this.btnValidate);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 125);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(413, 58);
            this.grpCancelOK.TabIndex = 7;
            this.grpCancelOK.TabStop = false;
            // 
            // btnViewMetadata
            // 
            this.btnViewMetadata.Enabled = false;
            this.btnViewMetadata.Location = new System.Drawing.Point(103, 18);
            this.btnViewMetadata.Name = "btnViewMetadata";
            this.btnViewMetadata.Size = new System.Drawing.Size(108, 27);
            this.btnViewMetadata.TabIndex = 8;
            this.btnViewMetadata.Text = "View Metadata";
            this.btnViewMetadata.UseVisualStyleBackColor = true;
            this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.Enabled = false;
            this.btnValidate.Location = new System.Drawing.Point(12, 18);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(75, 27);
            this.btnValidate.TabIndex = 7;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblInflationDataSetName);
            this.groupBox1.Controls.Add(this.txtInflationDataSetName);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.lblDatabase);
            this.groupBox1.Controls.Add(this.txtDatabase);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(413, 117);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // LoadInflationDataSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 191);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpCancelOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(358, 200);
            this.Name = "LoadInflationDataSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Inflation Dataset";
            this.Load += new System.EventHandler(this.LoadInflationDataSet_Load);
            this.grpCancelOK.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label lblInflationDataSetName;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtInflationDataSetName;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnViewMetadata;
        private System.Windows.Forms.Button btnValidate;
    }
}