namespace BenMAP
{
    partial class LoadSelectedDataSet
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
            this.btnViewMetadata = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtDataSetName = new System.Windows.Forms.TextBox();
            this.grpDataSetName = new System.Windows.Forms.GroupBox();
            this.lblDataSetName = new System.Windows.Forms.Label();
            this.flpCancelOK = new System.Windows.Forms.FlowLayoutPanel();
            this.gbCancelOK = new System.Windows.Forms.GroupBox();
            this.grpDataSetName.SuspendLayout();
            this.flpCancelOK.SuspendLayout();
            this.gbCancelOK.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnViewMetadata
            // 
            this.btnViewMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewMetadata.Enabled = false;
            this.btnViewMetadata.Location = new System.Drawing.Point(233, 3);
            this.btnViewMetadata.Name = "btnViewMetadata";
            this.btnViewMetadata.Size = new System.Drawing.Size(108, 27);
            this.btnViewMetadata.TabIndex = 1;
            this.btnViewMetadata.Text = "View Metadata";
            this.btnViewMetadata.UseVisualStyleBackColor = true;
            this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValidate.Enabled = false;
            this.btnValidate.Location = new System.Drawing.Point(152, 3);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(75, 27);
            this.btnValidate.TabIndex = 0;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCancelOK.SetFlowBreak(this.btnOK, true);
            this.btnOK.Location = new System.Drawing.Point(428, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(347, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(435, 81);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtDatabase
            // 
            this.txtDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabase.Enabled = false;
            this.txtDatabase.Location = new System.Drawing.Point(6, 85);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(408, 20);
            this.txtDatabase.TabIndex = 3;
            this.txtDatabase.TextChanged += new System.EventHandler(this.txtDatabase_TextChanged);
            // 
            // lblDatabase
            // 
            this.lblDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(6, 65);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(56, 13);
            this.lblDatabase.TabIndex = 2;
            this.lblDatabase.Text = "Database:";
            // 
            // txtDataSetName
            // 
            this.txtDataSetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataSetName.BackColor = System.Drawing.Color.White;
            this.txtDataSetName.Location = new System.Drawing.Point(6, 37);
            this.txtDataSetName.Name = "txtDataSetName";
            this.txtDataSetName.ReadOnly = true;
            this.txtDataSetName.Size = new System.Drawing.Size(489, 20);
            this.txtDataSetName.TabIndex = 1;
            this.txtDataSetName.Text = "MonitorDataSet0";
            // 
            // grpDataSetName
            // 
            this.grpDataSetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDataSetName.Controls.Add(this.btnBrowse);
            this.grpDataSetName.Controls.Add(this.txtDatabase);
            this.grpDataSetName.Controls.Add(this.lblDatabase);
            this.grpDataSetName.Controls.Add(this.txtDataSetName);
            this.grpDataSetName.Controls.Add(this.lblDataSetName);
            this.grpDataSetName.Location = new System.Drawing.Point(8, 2);
            this.grpDataSetName.Name = "grpDataSetName";
            this.grpDataSetName.Size = new System.Drawing.Size(516, 117);
            this.grpDataSetName.TabIndex = 0;
            this.grpDataSetName.TabStop = false;
            // 
            // lblDataSetName
            // 
            this.lblDataSetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDataSetName.AutoSize = true;
            this.lblDataSetName.Location = new System.Drawing.Point(6, 20);
            this.lblDataSetName.Name = "lblDataSetName";
            this.lblDataSetName.Size = new System.Drawing.Size(78, 13);
            this.lblDataSetName.TabIndex = 0;
            this.lblDataSetName.Text = "Dataset Name:";
            // 
            // flpCancelOK
            // 
            this.flpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCancelOK.Controls.Add(this.btnOK);
            this.flpCancelOK.Controls.Add(this.btnCancel);
            this.flpCancelOK.Controls.Add(this.btnViewMetadata);
            this.flpCancelOK.Controls.Add(this.btnValidate);
            this.flpCancelOK.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpCancelOK.Location = new System.Drawing.Point(6, 13);
            this.flpCancelOK.Name = "flpCancelOK";
            this.flpCancelOK.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flpCancelOK.Size = new System.Drawing.Size(506, 40);
            this.flpCancelOK.TabIndex = 0;
            // 
            // gbCancelOK
            // 
            this.gbCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCancelOK.Controls.Add(this.flpCancelOK);
            this.gbCancelOK.Location = new System.Drawing.Point(8, 122);
            this.gbCancelOK.Name = "gbCancelOK";
            this.gbCancelOK.Size = new System.Drawing.Size(516, 65);
            this.gbCancelOK.TabIndex = 1;
            this.gbCancelOK.TabStop = false;
            // 
            // LoadSelectedDataSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 191);
            this.Controls.Add(this.gbCancelOK);
            this.Controls.Add(this.grpDataSetName);
            this.Name = "LoadSelectedDataSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Load Dataset";
            this.grpDataSetName.ResumeLayout(false);
            this.grpDataSetName.PerformLayout();
            this.flpCancelOK.ResumeLayout(false);
            this.gbCancelOK.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnViewMetadata;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtDataSetName;
        private System.Windows.Forms.GroupBox grpDataSetName;
        private System.Windows.Forms.Label lblDataSetName;
        private System.Windows.Forms.FlowLayoutPanel flpCancelOK;
        private System.Windows.Forms.GroupBox gbCancelOK;
    }
}