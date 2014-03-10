namespace BenMAP
{
    partial class OpenExistingAQG
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
            this.btnOpenBase = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPollutant = new System.Windows.Forms.TextBox();
            this.txtBase = new System.Windows.Forms.TextBox();
            this.txtControl = new System.Windows.Forms.TextBox();
            this.btnCreatBase = new System.Windows.Forms.Button();
            this.btnOpenControl = new System.Windows.Forms.Button();
            this.btnCreatControl = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cboGrid = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
                                                this.btnOpenBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenBase.Location = new System.Drawing.Point(315, 89);
            this.btnOpenBase.Name = "btnOpenBase";
            this.btnOpenBase.Size = new System.Drawing.Size(56, 27);
            this.btnOpenBase.TabIndex = 0;
            this.btnOpenBase.Text = "Open";
            this.btnOpenBase.UseVisualStyleBackColor = true;
            this.btnOpenBase.Click += new System.EventHandler(this.btnOpenBase_Click);
                                                this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Grid Type:";
                                                this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pollutant:";
                                                this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "Baseline:";
                                                this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 14);
            this.label4.TabIndex = 5;
            this.label4.Text = "Control:";
                                                this.txtPollutant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPollutant.Location = new System.Drawing.Point(80, 43);
            this.txtPollutant.Name = "txtPollutant";
            this.txtPollutant.Size = new System.Drawing.Size(229, 22);
            this.txtPollutant.TabIndex = 6;
            this.txtPollutant.Text = "  ";
                                                this.txtBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBase.Location = new System.Drawing.Point(80, 92);
            this.txtBase.Name = "txtBase";
            this.txtBase.Size = new System.Drawing.Size(229, 22);
            this.txtBase.TabIndex = 7;
                                                this.txtControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtControl.Location = new System.Drawing.Point(80, 122);
            this.txtControl.Name = "txtControl";
            this.txtControl.Size = new System.Drawing.Size(229, 22);
            this.txtControl.TabIndex = 8;
                                                this.btnCreatBase.Location = new System.Drawing.Point(36, 151);
            this.btnCreatBase.Name = "btnCreatBase";
            this.btnCreatBase.Size = new System.Drawing.Size(56, 27);
            this.btnCreatBase.TabIndex = 9;
            this.btnCreatBase.Text = "Creat";
            this.btnCreatBase.UseVisualStyleBackColor = true;
            this.btnCreatBase.Visible = false;
            this.btnCreatBase.Click += new System.EventHandler(this.btnCreatBase_Click);
                                                this.btnOpenControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenControl.Location = new System.Drawing.Point(315, 120);
            this.btnOpenControl.Name = "btnOpenControl";
            this.btnOpenControl.Size = new System.Drawing.Size(56, 27);
            this.btnOpenControl.TabIndex = 10;
            this.btnOpenControl.Text = "Open";
            this.btnOpenControl.UseVisualStyleBackColor = true;
            this.btnOpenControl.Click += new System.EventHandler(this.btnOpenControl_Click);
                                                this.btnCreatControl.Location = new System.Drawing.Point(98, 150);
            this.btnCreatControl.Name = "btnCreatControl";
            this.btnCreatControl.Size = new System.Drawing.Size(56, 27);
            this.btnCreatControl.TabIndex = 11;
            this.btnCreatControl.Text = "Creat";
            this.btnCreatControl.UseVisualStyleBackColor = true;
            this.btnCreatControl.Visible = false;
            this.btnCreatControl.Click += new System.EventHandler(this.btnCreatControl_Click);
                                                this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(216, 158);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(297, 158);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(218, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "Open existing AQ data (.csv, .aqgx, etc.):";
                                                this.cboGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGrid.FormattingEnabled = true;
            this.cboGrid.Location = new System.Drawing.Point(80, 14);
            this.cboGrid.Name = "cboGrid";
            this.cboGrid.Size = new System.Drawing.Size(229, 22);
            this.cboGrid.TabIndex = 15;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 191);
            this.Controls.Add(this.cboGrid);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreatControl);
            this.Controls.Add(this.btnOpenControl);
            this.Controls.Add(this.btnCreatBase);
            this.Controls.Add(this.txtControl);
            this.Controls.Add(this.txtBase);
            this.Controls.Add(this.txtPollutant);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpenBase);
            this.MinimumSize = new System.Drawing.Size(401, 229);
            this.Name = "OpenExistingAQG";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Existing AQ Data ";
            this.Load += new System.EventHandler(this.OpenExistingAQG_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.Button btnOpenBase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPollutant;
        private System.Windows.Forms.TextBox txtBase;
        private System.Windows.Forms.TextBox txtControl;
        private System.Windows.Forms.Button btnCreatBase;
        private System.Windows.Forms.Button btnOpenControl;
        private System.Windows.Forms.Button btnCreatControl;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboGrid;
    }
}