namespace BenMAP
{
    partial class frmCrosswalk
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCrosswalk));
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbProgress = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lstCrosswalks1 = new System.Windows.Forms.ListBox();
            this.btnClearCrosswalks = new System.Windows.Forms.Button();
            this.lstCrosswalks2 = new System.Windows.Forms.ListBox();
            this.btnCompute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(289, 211);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(109, 26);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel Operation";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button4_Click);
            // 
            // tbProgress
            // 
            this.tbProgress.Location = new System.Drawing.Point(12, 330);
            this.tbProgress.Name = "tbProgress";
            this.tbProgress.ReadOnly = true;
            this.tbProgress.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbProgress.Size = new System.Drawing.Size(658, 20);
            this.tbProgress.TabIndex = 9;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 356);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(658, 13);
            this.progressBar1.TabIndex = 11;
            // 
            // lstCrosswalks1
            // 
            this.lstCrosswalks1.FormattingEnabled = true;
            this.lstCrosswalks1.Location = new System.Drawing.Point(12, 112);
            this.lstCrosswalks1.Name = "lstCrosswalks1";
            this.lstCrosswalks1.Size = new System.Drawing.Size(266, 212);
            this.lstCrosswalks1.TabIndex = 12;
            // 
            // btnClearCrosswalks
            // 
            this.btnClearCrosswalks.Location = new System.Drawing.Point(12, 52);
            this.btnClearCrosswalks.Name = "btnClearCrosswalks";
            this.btnClearCrosswalks.Size = new System.Drawing.Size(197, 34);
            this.btnClearCrosswalks.TabIndex = 13;
            this.btnClearCrosswalks.Text = "Clear Existing Crosswalks";
            this.btnClearCrosswalks.UseVisualStyleBackColor = true;
            this.btnClearCrosswalks.Click += new System.EventHandler(this.btnClearCrosswalks_Click);
            // 
            // lstCrosswalks2
            // 
            this.lstCrosswalks2.FormattingEnabled = true;
            this.lstCrosswalks2.Location = new System.Drawing.Point(404, 112);
            this.lstCrosswalks2.Name = "lstCrosswalks2";
            this.lstCrosswalks2.Size = new System.Drawing.Size(266, 212);
            this.lstCrosswalks2.TabIndex = 15;
            // 
            // btnCompute
            // 
            this.btnCompute.Location = new System.Drawing.Point(289, 127);
            this.btnCompute.Name = "btnCompute";
            this.btnCompute.Size = new System.Drawing.Size(109, 78);
            this.btnCompute.TabIndex = 16;
            this.btnCompute.Text = "Compute Crosswalk";
            this.btnCompute.UseVisualStyleBackColor = true;
            this.btnCompute.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(685, 28);
            this.label1.TabIndex = 17;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(570, 391);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 34);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmCrosswalk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 432);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCompute);
            this.Controls.Add(this.lstCrosswalks2);
            this.Controls.Add(this.btnClearCrosswalks);
            this.Controls.Add(this.lstCrosswalks1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tbProgress);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 50);
            this.Name = "frmCrosswalk";
            this.Text = "BenMAP - Crosswalk Calculator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCrosswalk_FormClosing);
            this.Load += new System.EventHandler(this.frmCrosswalk_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbProgress;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox lstCrosswalks1;
        private System.Windows.Forms.Button btnClearCrosswalks;
        private System.Windows.Forms.ListBox lstCrosswalks2;
        private System.Windows.Forms.Button btnCompute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
    }
}

