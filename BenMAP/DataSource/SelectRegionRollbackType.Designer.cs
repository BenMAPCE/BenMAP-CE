namespace BenMAP
{
    partial class SelectRegionRollbackType
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnToStandard = new System.Windows.Forms.RadioButton();
            this.rbtnIncremental = new System.Windows.Forms.RadioButton();
            this.rbtnPercentage = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnToStandard);
            this.groupBox1.Controls.Add(this.rbtnIncremental);
            this.groupBox1.Controls.Add(this.rbtnPercentage);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 131);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rollback Type";
            // 
            // rbtnToStandard
            // 
            this.rbtnToStandard.AutoSize = true;
            this.rbtnToStandard.Location = new System.Drawing.Point(20, 99);
            this.rbtnToStandard.Name = "rbtnToStandard";
            this.rbtnToStandard.Size = new System.Drawing.Size(155, 16);
            this.rbtnToStandard.TabIndex = 2;
            this.rbtnToStandard.Text = "Rollback to a Standard";
            this.rbtnToStandard.UseVisualStyleBackColor = true;
            // 
            // rbtnIncremental
            // 
            this.rbtnIncremental.AutoSize = true;
            this.rbtnIncremental.Location = new System.Drawing.Point(20, 63);
            this.rbtnIncremental.Name = "rbtnIncremental";
            this.rbtnIncremental.Size = new System.Drawing.Size(143, 16);
            this.rbtnIncremental.TabIndex = 1;
            this.rbtnIncremental.Text = "Incremental Rollback";
            this.rbtnIncremental.UseVisualStyleBackColor = true;
            // 
            // rbtnPercentage
            // 
            this.rbtnPercentage.AutoSize = true;
            this.rbtnPercentage.Checked = true;
            this.rbtnPercentage.Location = new System.Drawing.Point(20, 27);
            this.rbtnPercentage.Name = "rbtnPercentage";
            this.rbtnPercentage.Size = new System.Drawing.Size(137, 16);
            this.rbtnPercentage.TabIndex = 0;
            this.rbtnPercentage.TabStop = true;
            this.rbtnPercentage.Text = "Percentage Rollback";
            this.rbtnPercentage.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(242, 146);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(161, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SelectRegionRollbackType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 181);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SelectRegionRollbackType";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Region Rollback Type";
            this.Load += new System.EventHandler(this.SelectRegionRollbackType_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnToStandard;
        private System.Windows.Forms.RadioButton rbtnIncremental;
        private System.Windows.Forms.RadioButton rbtnPercentage;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}