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
            this.tvAvailablePollutants = new System.Windows.Forms.TreeView();
            this.btnAddgroup = new System.Windows.Forms.Button();
            this.chkShowGroups = new System.Windows.Forms.CheckBox();
            this.btnEdit = new System.Windows.Forms.Button();
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
            this.grpPollutantMetrics.Location = new System.Drawing.Point(320, 8);
            this.grpPollutantMetrics.Name = "grpPollutantMetrics";
            this.grpPollutantMetrics.Size = new System.Drawing.Size(162, 221);
            this.grpPollutantMetrics.TabIndex = 1;
            this.grpPollutantMetrics.TabStop = false;
            this.grpPollutantMetrics.Text = "Pollutant Metrics";
            // 
            // lstPollutantMetrics
            // 
            this.lstPollutantMetrics.Enabled = false;
            this.lstPollutantMetrics.FormattingEnabled = true;
            this.lstPollutantMetrics.ItemHeight = 14;
            this.lstPollutantMetrics.Location = new System.Drawing.Point(6, 21);
            this.lstPollutantMetrics.Name = "lstPollutantMetrics";
            this.lstPollutantMetrics.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstPollutantMetrics.Size = new System.Drawing.Size(150, 186);
            this.lstPollutantMetrics.TabIndex = 0;
            // 
            // grpAvailablePollutants
            // 
            this.grpAvailablePollutants.Controls.Add(this.tvAvailablePollutants);
            this.grpAvailablePollutants.Controls.Add(this.btnAddgroup);
            this.grpAvailablePollutants.Controls.Add(this.chkShowGroups);
            this.grpAvailablePollutants.Controls.Add(this.btnEdit);
            this.grpAvailablePollutants.Controls.Add(this.btnAdd);
            this.grpAvailablePollutants.Controls.Add(this.btnDelete);
            this.grpAvailablePollutants.Location = new System.Drawing.Point(6, 8);
            this.grpAvailablePollutants.Name = "grpAvailablePollutants";
            this.grpAvailablePollutants.Size = new System.Drawing.Size(306, 221);
            this.grpAvailablePollutants.TabIndex = 0;
            this.grpAvailablePollutants.TabStop = false;
            this.grpAvailablePollutants.Text = "Available Pollutants";
            // 
            // tvAvailablePollutants
            // 
            this.tvAvailablePollutants.FullRowSelect = true;
            this.tvAvailablePollutants.HideSelection = false;
            this.tvAvailablePollutants.Location = new System.Drawing.Point(6, 21);
            this.tvAvailablePollutants.Name = "tvAvailablePollutants";
            this.tvAvailablePollutants.Size = new System.Drawing.Size(288, 158);
            this.tvAvailablePollutants.TabIndex = 0;
            this.tvAvailablePollutants.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvAvailablePollutants_AfterSelect);
            // 
            // btnAddgroup
            // 
            this.btnAddgroup.Location = new System.Drawing.Point(6, 185);
            this.btnAddgroup.Name = "btnAddgroup";
            this.btnAddgroup.Size = new System.Drawing.Size(75, 27);
            this.btnAddgroup.TabIndex = 2;
            this.btnAddgroup.Text = "Add Group";
            this.btnAddgroup.UseVisualStyleBackColor = true;
            this.btnAddgroup.Click += new System.EventHandler(this.btnAddgroup_Click);
            // 
            // chkShowGroups
            // 
            this.chkShowGroups.AutoSize = true;
            this.chkShowGroups.Location = new System.Drawing.Point(229, 0);
            this.chkShowGroups.Name = "chkShowGroups";
            this.chkShowGroups.Size = new System.Drawing.Size(65, 18);
            this.chkShowGroups.TabIndex = 1;
            this.chkShowGroups.Text = "Groups";
            this.chkShowGroups.UseVisualStyleBackColor = true;
            this.chkShowGroups.CheckedChanged += new System.EventHandler(this.chkShowGroups_CheckedChanged);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(183, 185);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(50, 27);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(87, 185);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 27);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add Pollutant";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(239, 185);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(55, 27);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(326, 14);
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
            this.btnOK.Location = new System.Drawing.Point(401, 14);
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
            this.groupBox1.Location = new System.Drawing.Point(6, 233);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(1);
            this.groupBox1.Size = new System.Drawing.Size(476, 49);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // ManagePollutants
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 286);
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
            this.grpAvailablePollutants.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpPollutantMetrics;
        private System.Windows.Forms.ListBox lstPollutantMetrics;
        private System.Windows.Forms.GroupBox grpAvailablePollutants;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkShowGroups;
        private System.Windows.Forms.Button btnAddgroup;
        private System.Windows.Forms.TreeView tvAvailablePollutants;
    }
}