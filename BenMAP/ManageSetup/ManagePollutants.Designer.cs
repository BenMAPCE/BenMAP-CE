namespace BenMAP
{
    partial class ManagePollutants
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
            this.grpPollutantMetrics = new System.Windows.Forms.GroupBox();
            this.lstPollutantMetrics = new System.Windows.Forms.ListBox();
            this.grpAvailablePollutants = new System.Windows.Forms.GroupBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lstAvailablePollutants = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpPollutantMetrics.SuspendLayout();
            this.grpAvailablePollutants.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPollutantMetrics
            // 
            this.grpPollutantMetrics.Controls.Add(this.lstPollutantMetrics);
            this.grpPollutantMetrics.Location = new System.Drawing.Point(274, 8);
            this.grpPollutantMetrics.Name = "grpPollutantMetrics";
            this.grpPollutantMetrics.Size = new System.Drawing.Size(162, 210);
            this.grpPollutantMetrics.TabIndex = 1;
            this.grpPollutantMetrics.TabStop = false;
            this.grpPollutantMetrics.Text = "Pollutant Metrics";
            // 
            // lstPollutantMetrics
            // 
            this.lstPollutantMetrics.Enabled = false;
            this.lstPollutantMetrics.FormattingEnabled = true;
            this.lstPollutantMetrics.ItemHeight = 14;
            this.lstPollutantMetrics.Location = new System.Drawing.Point(6, 23);
            this.lstPollutantMetrics.Name = "lstPollutantMetrics";
            this.lstPollutantMetrics.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstPollutantMetrics.Size = new System.Drawing.Size(150, 172);
            this.lstPollutantMetrics.TabIndex = 0;
            // 
            // grpAvailablePollutants
            // 
            this.grpAvailablePollutants.Controls.Add(this.btnCopy);
            this.grpAvailablePollutants.Controls.Add(this.btnEdit);
            this.grpAvailablePollutants.Controls.Add(this.lstAvailablePollutants);
            this.grpAvailablePollutants.Controls.Add(this.btnAdd);
            this.grpAvailablePollutants.Controls.Add(this.btnDelete);
            this.grpAvailablePollutants.Location = new System.Drawing.Point(12, 8);
            this.grpAvailablePollutants.Name = "grpAvailablePollutants";
            this.grpAvailablePollutants.Size = new System.Drawing.Size(256, 210);
            this.grpAvailablePollutants.TabIndex = 0;
            this.grpAvailablePollutants.TabStop = false;
            this.grpAvailablePollutants.Text = "Available Pollutants";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(195, 176);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(55, 27);
            this.btnCopy.TabIndex = 5;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Visible = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(132, 176);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(57, 27);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lstAvailablePollutants
            // 
            this.lstAvailablePollutants.FormattingEnabled = true;
            this.lstAvailablePollutants.ItemHeight = 14;
            this.lstAvailablePollutants.Location = new System.Drawing.Point(6, 21);
            this.lstAvailablePollutants.Name = "lstAvailablePollutants";
            this.lstAvailablePollutants.Size = new System.Drawing.Size(244, 144);
            this.lstAvailablePollutants.TabIndex = 0;
            this.lstAvailablePollutants.SelectedIndexChanged += new System.EventHandler(this.lstAvailablePollutants_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(69, 176);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(57, 27);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(6, 176);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(57, 27);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(262, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(342, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(69, 27);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Location = new System.Drawing.Point(12, 220);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 59);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // ManagePollutants
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 286);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpPollutantMetrics);
            this.Controls.Add(this.grpAvailablePollutants);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(454, 314);
            this.Name = "ManagePollutants";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Pollutants";
            this.Load += new System.EventHandler(this.ManagePollutants_Load);
            this.grpPollutantMetrics.ResumeLayout(false);
            this.grpAvailablePollutants.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpPollutantMetrics;
        private System.Windows.Forms.ListBox lstPollutantMetrics;
        private System.Windows.Forms.GroupBox grpAvailablePollutants;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.ListBox lstAvailablePollutants;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCopy;
    }
}