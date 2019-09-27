namespace BenMAP
{
    partial class Options
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
            this.cboExit = new System.Windows.Forms.CheckBox();
            this.cboStart = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboDefaultSetup = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.cboRequireValidation = new System.Windows.Forms.CheckBox();
            this.lblDeletelogs = new System.Windows.Forms.Label();
            this.btnDeleteNow = new System.Windows.Forms.Button();
            this.txtNumDays = new System.Windows.Forms.TextBox();
            this.cboGeographicAreaInfo = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cboExit
            // 
            this.cboExit.AutoSize = true;
            this.cboExit.Location = new System.Drawing.Point(12, 32);
            this.cboExit.Name = "cboExit";
            this.cboExit.Size = new System.Drawing.Size(125, 18);
            this.cboExit.TabIndex = 1;
            this.cboExit.Text = "Show Exit Window";
            this.cboExit.UseVisualStyleBackColor = true;
            // 
            // cboStart
            // 
            this.cboStart.AutoSize = true;
            this.cboStart.Location = new System.Drawing.Point(12, 12);
            this.cboStart.Name = "cboStart";
            this.cboStart.Size = new System.Drawing.Size(131, 18);
            this.cboStart.TabIndex = 0;
            this.cboStart.Text = "Show Start Window";
            this.cboStart.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 14);
            this.label1.TabIndex = 7;
            this.label1.Text = "Default setup:";
            // 
            // cboDefaultSetup
            // 
            this.cboDefaultSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDefaultSetup.FormattingEnabled = true;
            this.cboDefaultSetup.Location = new System.Drawing.Point(12, 176);
            this.cboDefaultSetup.Name = "cboDefaultSetup";
            this.cboDefaultSetup.Size = new System.Drawing.Size(244, 22);
            this.cboDefaultSetup.TabIndex = 8;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(181, 208);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cboRequireValidation
            // 
            this.cboRequireValidation.AutoSize = true;
            this.cboRequireValidation.Location = new System.Drawing.Point(12, 72);
            this.cboRequireValidation.Name = "cboRequireValidation";
            this.cboRequireValidation.Size = new System.Drawing.Size(220, 18);
            this.cboRequireValidation.TabIndex = 3;
            this.cboRequireValidation.Text = "Require Validation for Data Imports";
            this.cboRequireValidation.UseVisualStyleBackColor = true;
            // 
            // lblDeletelogs
            // 
            this.lblDeletelogs.AutoSize = true;
            this.lblDeletelogs.Location = new System.Drawing.Point(9, 101);
            this.lblDeletelogs.Name = "lblDeletelogs";
            this.lblDeletelogs.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblDeletelogs.Size = new System.Drawing.Size(218, 24);
            this.lblDeletelogs.TabIndex = 4;
            this.lblDeletelogs.Text = "Delete Validation logs after            days";
            // 
            // btnDeleteNow
            // 
            this.btnDeleteNow.Location = new System.Drawing.Point(12, 131);
            this.btnDeleteNow.Name = "btnDeleteNow";
            this.btnDeleteNow.Size = new System.Drawing.Size(197, 23);
            this.btnDeleteNow.TabIndex = 6;
            this.btnDeleteNow.Text = "Delete Validation Error Logs Now";
            this.btnDeleteNow.UseVisualStyleBackColor = true;
            this.btnDeleteNow.Click += new System.EventHandler(this.btnDeleteNow_Click);
            // 
            // txtNumDays
            // 
            this.txtNumDays.Location = new System.Drawing.Point(166, 103);
            this.txtNumDays.Name = "txtNumDays";
            this.txtNumDays.Size = new System.Drawing.Size(30, 22);
            this.txtNumDays.TabIndex = 5;
            this.txtNumDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtNumDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumDays_KeyPress);
            // 
            // cboGeographicAreaInfo
            // 
            this.cboGeographicAreaInfo.AutoSize = true;
            this.cboGeographicAreaInfo.Location = new System.Drawing.Point(12, 52);
            this.cboGeographicAreaInfo.Name = "cboGeographicAreaInfo";
            this.cboGeographicAreaInfo.Size = new System.Drawing.Size(173, 18);
            this.cboGeographicAreaInfo.TabIndex = 2;
            this.cboGeographicAreaInfo.Text = "Show Geographic Area Info";
            this.cboGeographicAreaInfo.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 241);
            this.Controls.Add(this.cboGeographicAreaInfo);
            this.Controls.Add(this.txtNumDays);
            this.Controls.Add(this.btnDeleteNow);
            this.Controls.Add(this.lblDeletelogs);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboDefaultSetup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboRequireValidation);
            this.Controls.Add(this.cboExit);
            this.Controls.Add(this.cboStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.CheckBox cboStart;
        private System.Windows.Forms.CheckBox cboExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDefaultSetup;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cboRequireValidation;
        private System.Windows.Forms.Label lblDeletelogs;
        private System.Windows.Forms.Button btnDeleteNow;
        private System.Windows.Forms.TextBox txtNumDays;
        private System.Windows.Forms.CheckBox cboGeographicAreaInfo;
    }
}