namespace BenMAP
{
    partial class ValidateDatabaseImport
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
            this.txtReportOutput = new System.Windows.Forms.RichTextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.pbarValidation = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtReportOutput
            // 
            this.txtReportOutput.BackColor = System.Drawing.Color.White;
            this.txtReportOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtReportOutput.Location = new System.Drawing.Point(0, 0);
            this.txtReportOutput.Name = "txtReportOutput";
            this.txtReportOutput.ReadOnly = true;
            this.txtReportOutput.Size = new System.Drawing.Size(691, 491);
            this.txtReportOutput.TabIndex = 0;
            this.txtReportOutput.Text = "";
            this.txtReportOutput.WordWrap = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(609, 503);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnLoad.Enabled = false;
            this.btnLoad.Location = new System.Drawing.Point(528, 503);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 27);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // pbarValidation
            // 
            this.pbarValidation.Location = new System.Drawing.Point(5, 505);
            this.pbarValidation.Name = "pbarValidation";
            this.pbarValidation.Size = new System.Drawing.Size(460, 23);
            this.pbarValidation.TabIndex = 3;
            this.pbarValidation.Visible = false;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(481, 510);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 4;
            this.lblProgress.Visible = false;
            // 
            // ValidateDatabaseImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 540);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.pbarValidation);
            this.Controls.Add(this.txtReportOutput);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ValidateDatabaseImport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ValidateDatabaseImport";
            this.Load += new System.EventHandler(this.ValidateDatabaseImport_Load);
            this.Shown += new System.EventHandler(this.ValidateDatabaseImport_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtReportOutput;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ProgressBar pbarValidation;
        private System.Windows.Forms.Label lblProgress;
    }
}