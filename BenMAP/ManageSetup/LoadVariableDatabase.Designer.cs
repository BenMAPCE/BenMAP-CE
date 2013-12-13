namespace BenMAP
{
    partial class LoadVariableDatabase
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
            this.lblGridDefinition = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.grpLoadDatabase = new System.Windows.Forms.GroupBox();
            this.cboGridDefinition = new System.Windows.Forms.ComboBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.grpCanceOK = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpLoadDatabase.SuspendLayout();
            this.grpCanceOK.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblGridDefinition
            // 
            this.lblGridDefinition.AutoSize = true;
            this.lblGridDefinition.Location = new System.Drawing.Point(6, 15);
            this.lblGridDefinition.Name = "lblGridDefinition";
            this.lblGridDefinition.Size = new System.Drawing.Size(89, 14);
            this.lblGridDefinition.TabIndex = 0;
            this.lblGridDefinition.Text = "Grid Definition:";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(6, 65);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(63, 14);
            this.lblDatabase.TabIndex = 1;
            this.lblDatabase.Text = "Database:";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(8, 82);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(197, 22);
            this.txtDatabase.TabIndex = 3;
            // 
            // grpLoadDatabase
            // 
            this.grpLoadDatabase.Controls.Add(this.cboGridDefinition);
            this.grpLoadDatabase.Controls.Add(this.btnBrowse);
            this.grpLoadDatabase.Controls.Add(this.lblGridDefinition);
            this.grpLoadDatabase.Controls.Add(this.txtDatabase);
            this.grpLoadDatabase.Controls.Add(this.lblDatabase);
            this.grpLoadDatabase.Location = new System.Drawing.Point(12, 3);
            this.grpLoadDatabase.Name = "grpLoadDatabase";
            this.grpLoadDatabase.Size = new System.Drawing.Size(295, 114);
            this.grpLoadDatabase.TabIndex = 4;
            this.grpLoadDatabase.TabStop = false;
            // 
            // cboGridDefinition
            // 
            this.cboGridDefinition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridDefinition.FormattingEnabled = true;
            this.cboGridDefinition.Location = new System.Drawing.Point(8, 32);
            this.cboGridDefinition.Name = "cboGridDefinition";
            this.cboGridDefinition.Size = new System.Drawing.Size(197, 22);
            this.cboGridDefinition.TabIndex = 5;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(211, 79);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // grpCanceOK
            // 
            this.grpCanceOK.Controls.Add(this.btnOK);
            this.grpCanceOK.Controls.Add(this.btnCancel);
            this.grpCanceOK.Location = new System.Drawing.Point(12, 120);
            this.grpCanceOK.Name = "grpCanceOK";
            this.grpCanceOK.Size = new System.Drawing.Size(297, 57);
            this.grpCanceOK.TabIndex = 5;
            this.grpCanceOK.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(211, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(130, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LoadVariableDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 183);
            this.Controls.Add(this.grpCanceOK);
            this.Controls.Add(this.grpLoadDatabase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(327, 211);
            this.Name = "LoadVariableDatabase";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Variable Database";
            this.Load += new System.EventHandler(this.LoadVariableDatabase_Load);
            this.grpLoadDatabase.ResumeLayout(false);
            this.grpLoadDatabase.PerformLayout();
            this.grpCanceOK.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblGridDefinition;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.GroupBox grpLoadDatabase;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpCanceOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboGridDefinition;
    }
}