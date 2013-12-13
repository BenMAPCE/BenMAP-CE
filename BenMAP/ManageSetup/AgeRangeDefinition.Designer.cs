namespace BenMAP
{
    partial class AgeRangeDefinition
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
            this.btnOK = new System.Windows.Forms.Button();
            this.grpAgeRange = new System.Windows.Forms.GroupBox();
            this.lblAgeRangeID = new System.Windows.Forms.Label();
            this.txtHighAge = new System.Windows.Forms.TextBox();
            this.txtAgeRangeID = new System.Windows.Forms.TextBox();
            this.lblHighAge = new System.Windows.Forms.Label();
            this.txtLowAgeofAgeRange = new System.Windows.Forms.TextBox();
            this.lblLowAge = new System.Windows.Forms.Label();
            this.grpAgeRange.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(186, 143);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grpAgeRange
            // 
            this.grpAgeRange.Controls.Add(this.lblAgeRangeID);
            this.grpAgeRange.Controls.Add(this.txtHighAge);
            this.grpAgeRange.Controls.Add(this.txtAgeRangeID);
            this.grpAgeRange.Controls.Add(this.lblHighAge);
            this.grpAgeRange.Controls.Add(this.txtLowAgeofAgeRange);
            this.grpAgeRange.Controls.Add(this.lblLowAge);
            this.grpAgeRange.Location = new System.Drawing.Point(12, 5);
            this.grpAgeRange.Name = "grpAgeRange";
            this.grpAgeRange.Size = new System.Drawing.Size(249, 132);
            this.grpAgeRange.TabIndex = 6;
            this.grpAgeRange.TabStop = false;
            // 
            // lblAgeRangeID
            // 
            this.lblAgeRangeID.AutoSize = true;
            this.lblAgeRangeID.Location = new System.Drawing.Point(6, 20);
            this.lblAgeRangeID.Name = "lblAgeRangeID";
            this.lblAgeRangeID.Size = new System.Drawing.Size(82, 14);
            this.lblAgeRangeID.TabIndex = 0;
            this.lblAgeRangeID.Text = "Age Range ID:";
            // 
            // txtHighAge
            // 
            this.txtHighAge.Location = new System.Drawing.Point(100, 96);
            this.txtHighAge.Name = "txtHighAge";
            this.txtHighAge.Size = new System.Drawing.Size(143, 22);
            this.txtHighAge.TabIndex = 5;
            // 
            // txtAgeRangeID
            // 
            this.txtAgeRangeID.Location = new System.Drawing.Point(100, 20);
            this.txtAgeRangeID.Name = "txtAgeRangeID";
            this.txtAgeRangeID.Size = new System.Drawing.Size(143, 22);
            this.txtAgeRangeID.TabIndex = 3;
            // 
            // lblHighAge
            // 
            this.lblHighAge.AutoSize = true;
            this.lblHighAge.Location = new System.Drawing.Point(6, 99);
            this.lblHighAge.Name = "lblHighAge";
            this.lblHighAge.Size = new System.Drawing.Size(58, 14);
            this.lblHighAge.TabIndex = 2;
            this.lblHighAge.Text = "High Age:";
            // 
            // txtLowAgeofAgeRange
            // 
            this.txtLowAgeofAgeRange.Location = new System.Drawing.Point(100, 57);
            this.txtLowAgeofAgeRange.Name = "txtLowAgeofAgeRange";
            this.txtLowAgeofAgeRange.ReadOnly = true;
            this.txtLowAgeofAgeRange.Size = new System.Drawing.Size(143, 22);
            this.txtLowAgeofAgeRange.TabIndex = 4;
            // 
            // lblLowAge
            // 
            this.lblLowAge.AutoSize = true;
            this.lblLowAge.Location = new System.Drawing.Point(6, 61);
            this.lblLowAge.Name = "lblLowAge";
            this.lblLowAge.Size = new System.Drawing.Size(54, 14);
            this.lblLowAge.TabIndex = 1;
            this.lblLowAge.Text = "Low Age:";
            // 
            // AgeRangeDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 178);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpAgeRange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(279, 206);
            this.Name = "AgeRangeDefinition";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AgeRange Definition";
            this.Load += new System.EventHandler(this.AgeRangeDefinition_Load);
            this.grpAgeRange.ResumeLayout(false);
            this.grpAgeRange.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAgeRangeID;
        private System.Windows.Forms.Label lblLowAge;
        private System.Windows.Forms.Label lblHighAge;
        private System.Windows.Forms.TextBox txtAgeRangeID;
        private System.Windows.Forms.TextBox txtLowAgeofAgeRange;
        private System.Windows.Forms.TextBox txtHighAge;
        private System.Windows.Forms.GroupBox grpAgeRange;
        private System.Windows.Forms.Button btnOK;
    }
}