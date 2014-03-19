namespace BenMAP
{
    partial class LoadIncomeGrowthDataSet
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
            this.lblDataSetName = new System.Windows.Forms.Label();
            this.grpDataSetName = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtDataSetName = new System.Windows.Forms.TextBox();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.btnViewMetadata = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpDataSetName.SuspendLayout();
            this.grpCancelOK.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDataSetName
            // 
            this.lblDataSetName.AutoSize = true;
            this.lblDataSetName.Location = new System.Drawing.Point(6, 20);
            this.lblDataSetName.Name = "lblDataSetName";
            this.lblDataSetName.Size = new System.Drawing.Size(241, 14);
            this.lblDataSetName.TabIndex = 0;
            this.lblDataSetName.Text = "Income Growth Adjustment Dataset Name: ";
            // 
            // grpDataSetName
            // 
            this.grpDataSetName.Controls.Add(this.btnBrowse);
            this.grpDataSetName.Controls.Add(this.txtDatabase);
            this.grpDataSetName.Controls.Add(this.lblDatabase);
            this.grpDataSetName.Controls.Add(this.txtDataSetName);
            this.grpDataSetName.Controls.Add(this.lblDataSetName);
            this.grpDataSetName.Location = new System.Drawing.Point(12, 6);
            this.grpDataSetName.Name = "grpDataSetName";
            this.grpDataSetName.Size = new System.Drawing.Size(413, 117);
            this.grpDataSetName.TabIndex = 1;
            this.grpDataSetName.TabStop = false;
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
            // txtDatabase
            // 
            this.txtDatabase.Enabled = false;
            this.txtDatabase.Location = new System.Drawing.Point(8, 85);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(305, 22);
            this.txtDatabase.TabIndex = 3;
            this.txtDatabase.TextChanged += new System.EventHandler(this.txtDatabase_TextChanged);
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(6, 65);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(63, 14);
            this.lblDatabase.TabIndex = 2;
            this.lblDatabase.Text = "Database:";
            // 
            // txtDataSetName
            // 
            this.txtDataSetName.Location = new System.Drawing.Point(8, 37);
            this.txtDataSetName.Name = "txtDataSetName";
            this.txtDataSetName.Size = new System.Drawing.Size(386, 22);
            this.txtDataSetName.TabIndex = 1;
            this.txtDataSetName.Text = "IncomeGrowthAdjustmentDataSet0";
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Controls.Add(this.btnViewMetadata);
            this.grpCancelOK.Controls.Add(this.btnValidate);
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 125);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(413, 58);
            this.grpCancelOK.TabIndex = 2;
            this.grpCancelOK.TabStop = false;
            // 
            // btnViewMetadata
            // 
            this.btnViewMetadata.Enabled = false;
            this.btnViewMetadata.Location = new System.Drawing.Point(101, 21);
            this.btnViewMetadata.Name = "btnViewMetadata";
            this.btnViewMetadata.Size = new System.Drawing.Size(108, 27);
            this.btnViewMetadata.TabIndex = 5;
            this.btnViewMetadata.Text = "View Metadata";
            this.btnViewMetadata.UseVisualStyleBackColor = true;
            this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.Enabled = false;
            this.btnValidate.Location = new System.Drawing.Point(9, 21);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(75, 27);
            this.btnValidate.TabIndex = 4;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(318, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(226, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LoadIncomeGrowthDataSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 191);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpDataSetName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(445, 219);
            this.Name = "LoadIncomeGrowthDataSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Income Growth Adjustment Factors Dataset";
            this.Load += new System.EventHandler(this.LoadIncomeGrowthDataSet_Load);
            this.grpDataSetName.ResumeLayout(false);
            this.grpDataSetName.PerformLayout();
            this.grpCancelOK.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label lblDataSetName;
        private System.Windows.Forms.GroupBox grpDataSetName;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtDataSetName;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnViewMetadata;
        private System.Windows.Forms.Button btnValidate;
    }
}