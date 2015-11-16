namespace BenMAP
{
    partial class ManageVariableDataSets
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
            this.grpAvailable = new System.Windows.Forms.GroupBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btn = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstAvailable = new System.Windows.Forms.ListBox();
            this.grpVariables = new System.Windows.Forms.GroupBox();
            this.lstVariables = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnViewMetadata = new System.Windows.Forms.Button();
            this.grpAvailable.SuspendLayout();
            this.grpVariables.SuspendLayout();
            this.grpCancelOK.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpAvailable
            // 
            this.grpAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpAvailable.Controls.Add(this.btnCopy);
            this.grpAvailable.Controls.Add(this.btnEdit);
            this.grpAvailable.Controls.Add(this.btn);
            this.grpAvailable.Controls.Add(this.btnDelete);
            this.grpAvailable.Controls.Add(this.lstAvailable);
            this.grpAvailable.Location = new System.Drawing.Point(12, 8);
            this.grpAvailable.Name = "grpAvailable";
            this.grpAvailable.Size = new System.Drawing.Size(275, 373);
            this.grpAvailable.TabIndex = 0;
            this.grpAvailable.TabStop = false;
            this.grpAvailable.Text = "Available Datasets";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(202, 339);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(54, 27);
            this.btnCopy.TabIndex = 5;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Visible = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Location = new System.Drawing.Point(140, 339);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(56, 27);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btn
            // 
            this.btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn.Location = new System.Drawing.Point(78, 339);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(56, 27);
            this.btn.TabIndex = 3;
            this.btn.Text = "Add";
            this.btn.UseVisualStyleBackColor = true;
            this.btn.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(16, 339);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(56, 27);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstAvailable
            // 
            this.lstAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstAvailable.FormattingEnabled = true;
            this.lstAvailable.HorizontalScrollbar = true;
            this.lstAvailable.ItemHeight = 14;
            this.lstAvailable.Location = new System.Drawing.Point(8, 21);
            this.lstAvailable.Name = "lstAvailable";
            this.lstAvailable.Size = new System.Drawing.Size(251, 298);
            this.lstAvailable.TabIndex = 1;
            this.lstAvailable.SelectedValueChanged += new System.EventHandler(this.lstAvailable_SelectedValueChanged);
            // 
            // grpVariables
            // 
            this.grpVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVariables.Controls.Add(this.lstVariables);
            this.grpVariables.Location = new System.Drawing.Point(296, 8);
            this.grpVariables.Name = "grpVariables";
            this.grpVariables.Size = new System.Drawing.Size(280, 373);
            this.grpVariables.TabIndex = 1;
            this.grpVariables.TabStop = false;
            this.grpVariables.Text = "Dataset Variables";
            // 
            // lstVariables
            // 
            this.lstVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstVariables.FormattingEnabled = true;
            this.lstVariables.HorizontalScrollbar = true;
            this.lstVariables.ItemHeight = 14;
            this.lstVariables.Location = new System.Drawing.Point(17, 21);
            this.lstVariables.Name = "lstVariables";
            this.lstVariables.Size = new System.Drawing.Size(246, 298);
            this.lstVariables.TabIndex = 1;
            this.lstVariables.SelectedIndexChanged += new System.EventHandler(this.lstVariables_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(399, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(480, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.flowLayoutPanel1);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 380);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(564, 54);
            this.grpCancelOK.TabIndex = 4;
            this.grpCancelOK.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnViewMetadata);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(558, 33);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // btnViewMetadata
            // 
            this.btnViewMetadata.Enabled = false;
            this.btnViewMetadata.Location = new System.Drawing.Point(281, 3);
            this.btnViewMetadata.Name = "btnViewMetadata";
            this.btnViewMetadata.Size = new System.Drawing.Size(112, 27);
            this.btnViewMetadata.TabIndex = 33;
            this.btnViewMetadata.Text = "View Metadata";
            this.btnViewMetadata.UseVisualStyleBackColor = true;
            this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
            // 
            // ManageVariableDataSets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 439);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpVariables);
            this.Controls.Add(this.grpAvailable);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(456, 477);
            this.Name = "ManageVariableDataSets";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Setup Variable Datasets";
            this.Load += new System.EventHandler(this.ManageVariableDataSets_Load);
            this.grpAvailable.ResumeLayout(false);
            this.grpVariables.ResumeLayout(false);
            this.grpCancelOK.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpAvailable;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btn;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstAvailable;
        private System.Windows.Forms.GroupBox grpVariables;
        private System.Windows.Forms.ListBox lstVariables;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnViewMetadata;
        private System.Windows.Forms.Button btnCopy;
    }
}