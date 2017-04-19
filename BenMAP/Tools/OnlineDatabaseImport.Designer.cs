namespace BenMAP
{
    partial class OnlineDatabaseImport
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
            this.gvImportSetups = new System.Windows.Forms.DataGridView();
            this.fbFileID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uploaded = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BenMAPVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Company = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Downloads = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pBarImport = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboSetup = new System.Windows.Forms.ComboBox();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lbProcess = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gvImportSetups)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gvImportSetups
            // 
            this.gvImportSetups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvImportSetups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fbFileID,
            this.CreatedBy,
            this.Uploaded,
            this.BenMAPVersion,
            this.Company,
            this.FileDescription,
            this.FileSize,
            this.Downloads});
            this.gvImportSetups.Location = new System.Drawing.Point(7, 19);
            this.gvImportSetups.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gvImportSetups.Name = "gvImportSetups";
            this.gvImportSetups.RowTemplate.Height = 24;
            this.gvImportSetups.Size = new System.Drawing.Size(611, 187);
            this.gvImportSetups.TabIndex = 0;
            // 
            // fbFileID
            // 
            this.fbFileID.DataPropertyName = "fbFileID";
            this.fbFileID.HeaderText = "fbFileID";
            this.fbFileID.Name = "fbFileID";
            this.fbFileID.ReadOnly = true;
            this.fbFileID.Visible = false;
            // 
            // CreatedBy
            // 
            this.CreatedBy.DataPropertyName = "CreatedBy";
            this.CreatedBy.HeaderText = "Uploaded By";
            this.CreatedBy.Name = "CreatedBy";
            this.CreatedBy.ReadOnly = true;
            this.CreatedBy.Width = 150;
            // 
            // Uploaded
            // 
            this.Uploaded.DataPropertyName = "Uploaded";
            this.Uploaded.HeaderText = "Uploaded";
            this.Uploaded.Name = "Uploaded";
            this.Uploaded.ReadOnly = true;
            this.Uploaded.Width = 120;
            // 
            // BenMAPVersion
            // 
            this.BenMAPVersion.DataPropertyName = "BenMAPVersion";
            this.BenMAPVersion.HeaderText = "BenMAP Version";
            this.BenMAPVersion.Name = "BenMAPVersion";
            this.BenMAPVersion.ReadOnly = true;
            // 
            // Company
            // 
            this.Company.DataPropertyName = "Company";
            this.Company.HeaderText = "Organization";
            this.Company.Name = "Company";
            this.Company.ReadOnly = true;
            this.Company.Width = 150;
            // 
            // FileDescription
            // 
            this.FileDescription.DataPropertyName = "FileDescription";
            this.FileDescription.HeaderText = "Description";
            this.FileDescription.Name = "FileDescription";
            this.FileDescription.ReadOnly = true;
            this.FileDescription.Width = 200;
            // 
            // FileSize
            // 
            this.FileSize.DataPropertyName = "FileSize";
            this.FileSize.HeaderText = "Size";
            this.FileSize.Name = "FileSize";
            this.FileSize.ReadOnly = true;
            this.FileSize.Width = 75;
            // 
            // Downloads
            // 
            this.Downloads.DataPropertyName = "Downloads";
            this.Downloads.HeaderText = "Downloads";
            this.Downloads.Name = "Downloads";
            this.Downloads.ReadOnly = true;
            this.Downloads.Width = 85;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(546, 292);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 26);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(465, 292);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pBarImport
            // 
            this.pBarImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBarImport.Location = new System.Drawing.Point(10, 264);
            this.pBarImport.Name = "pBarImport";
            this.pBarImport.Size = new System.Drawing.Size(611, 21);
            this.pBarImport.Step = 1;
            this.pBarImport.TabIndex = 15;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboSetup);
            this.groupBox1.Controls.Add(this.lblTarget);
            this.groupBox1.Controls.Add(this.gvImportSetups);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(626, 255);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // cboSetup
            // 
            this.cboSetup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSetup.FormattingEnabled = true;
            this.cboSetup.Location = new System.Drawing.Point(170, 226);
            this.cboSetup.Name = "cboSetup";
            this.cboSetup.Size = new System.Drawing.Size(449, 22);
            this.cboSetup.TabIndex = 10;
            this.cboSetup.SelectedIndexChanged += new System.EventHandler(this.cboSetup_SelectedIndexChanged);
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(168, 208);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(77, 14);
            this.lblTarget.TabIndex = 9;
            this.lblTarget.Text = "Target Setup:";
            // 
            // lbProcess
            // 
            this.lbProcess.AutoSize = true;
            this.lbProcess.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProcess.ForeColor = System.Drawing.Color.Black;
            this.lbProcess.Location = new System.Drawing.Point(10, 288);
            this.lbProcess.Name = "lbProcess";
            this.lbProcess.Size = new System.Drawing.Size(0, 14);
            this.lbProcess.TabIndex = 16;
            // 
            // OnlineDatabaseImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 325);
            this.Controls.Add(this.lbProcess);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pBarImport);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "OnlineDatabaseImport";
            this.Text = "Online Database Import";
            this.Load += new System.EventHandler(this.OnlineDatabaseImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvImportSetups)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gvImportSetups;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar pBarImport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboSetup;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lbProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn fbFileID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn Uploaded;
        private System.Windows.Forms.DataGridViewTextBoxColumn BenMAPVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Company;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn Downloads;
    }
}