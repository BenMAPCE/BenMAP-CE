namespace BenMAP
{
    partial class LoadIncidenceDatabase
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
            this.grpCanceOK = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpLoadDatabase = new System.Windows.Forms.GroupBox();
            this.cboGridDefinition = new System.Windows.Forms.ComboBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblGridDefinition = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.grpCanceOK.SuspendLayout();
            this.grpLoadDatabase.SuspendLayout();
            this.SuspendLayout();
                                                this.grpCanceOK.Controls.Add(this.btnOK);
            this.grpCanceOK.Controls.Add(this.btnCancel);
            this.grpCanceOK.Location = new System.Drawing.Point(12, 140);
            this.grpCanceOK.Name = "grpCanceOK";
            this.grpCanceOK.Size = new System.Drawing.Size(317, 57);
            this.grpCanceOK.TabIndex = 7;
            this.grpCanceOK.TabStop = false;
                                                this.btnOK.Location = new System.Drawing.Point(236, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Location = new System.Drawing.Point(155, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.grpLoadDatabase.Controls.Add(this.cboGridDefinition);
            this.grpLoadDatabase.Controls.Add(this.btnBrowse);
            this.grpLoadDatabase.Controls.Add(this.lblGridDefinition);
            this.grpLoadDatabase.Controls.Add(this.txtDatabase);
            this.grpLoadDatabase.Controls.Add(this.lblDatabase);
            this.grpLoadDatabase.Location = new System.Drawing.Point(12, 4);
            this.grpLoadDatabase.Name = "grpLoadDatabase";
            this.grpLoadDatabase.Size = new System.Drawing.Size(317, 135);
            this.grpLoadDatabase.TabIndex = 6;
            this.grpLoadDatabase.TabStop = false;
                                                this.cboGridDefinition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridDefinition.Enabled = false;
            this.cboGridDefinition.FormattingEnabled = true;
            this.cboGridDefinition.Location = new System.Drawing.Point(8, 37);
            this.cboGridDefinition.Name = "cboGridDefinition";
            this.cboGridDefinition.Size = new System.Drawing.Size(303, 22);
            this.cboGridDefinition.TabIndex = 5;
                                                this.btnBrowse.Location = new System.Drawing.Point(236, 92);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
                                                this.lblGridDefinition.AutoSize = true;
            this.lblGridDefinition.Location = new System.Drawing.Point(6, 20);
            this.lblGridDefinition.Name = "lblGridDefinition";
            this.lblGridDefinition.Size = new System.Drawing.Size(89, 14);
            this.lblGridDefinition.TabIndex = 0;
            this.lblGridDefinition.Text = "Grid Definition:";
                                                this.txtDatabase.Location = new System.Drawing.Point(8, 94);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(222, 22);
            this.txtDatabase.TabIndex = 3;
                                                this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(6, 77);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(63, 14);
            this.lblDatabase.TabIndex = 1;
            this.lblDatabase.Text = "Database:";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 205);
            this.Controls.Add(this.grpCanceOK);
            this.Controls.Add(this.grpLoadDatabase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(347, 233);
            this.Name = "LoadIncidenceDatabase";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Incidence/Prevalence Database";
            this.Load += new System.EventHandler(this.LoadIncidenceDatabase_Load);
            this.grpCanceOK.ResumeLayout(false);
            this.grpLoadDatabase.ResumeLayout(false);
            this.grpLoadDatabase.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpCanceOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpLoadDatabase;
        private System.Windows.Forms.ComboBox cboGridDefinition;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblGridDefinition;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblDatabase;
    }
}