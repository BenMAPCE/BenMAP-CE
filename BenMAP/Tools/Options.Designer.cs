namespace BenMAP
{
    partial class Options
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
            this.cboExit = new System.Windows.Forms.CheckBox();
            this.cboStart = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboDefaultSetup = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboExit
            // 
            this.cboExit.AutoSize = true;
            this.cboExit.Location = new System.Drawing.Point(39, 46);
            this.cboExit.Name = "cboExit";
            this.cboExit.Size = new System.Drawing.Size(125, 18);
            this.cboExit.TabIndex = 0;
            this.cboExit.Text = "Show Exit Window";
            this.cboExit.UseVisualStyleBackColor = true;
            // 
            // cboStart
            // 
            this.cboStart.AutoSize = true;
            this.cboStart.Location = new System.Drawing.Point(39, 22);
            this.cboStart.Name = "cboStart";
            this.cboStart.Size = new System.Drawing.Size(131, 18);
            this.cboStart.TabIndex = 0;
            this.cboStart.Text = "Show Start Window";
            this.cboStart.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Default setup:";
            // 
            // cboDefaultSetup
            // 
            this.cboDefaultSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDefaultSetup.FormattingEnabled = true;
            this.cboDefaultSetup.Location = new System.Drawing.Point(39, 96);
            this.cboDefaultSetup.Name = "cboDefaultSetup";
            this.cboDefaultSetup.Size = new System.Drawing.Size(162, 22);
            this.cboDefaultSetup.TabIndex = 2;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(83, 144);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 179);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboDefaultSetup);
            this.Controls.Add(this.label1);
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

        #endregion

        private System.Windows.Forms.CheckBox cboStart;
        private System.Windows.Forms.CheckBox cboExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDefaultSetup;
        private System.Windows.Forms.Button btnOK;
    }
}