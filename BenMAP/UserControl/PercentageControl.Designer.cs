namespace BenMAP.DataSource
{
    partial class PercentageControl
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.txtBackground = new System.Windows.Forms.TextBox();
            this.txtPercent = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblBackground);
            this.groupBox2.Controls.Add(this.lblPercent);
            this.groupBox2.Controls.Add(this.txtBackground);
            this.groupBox2.Controls.Add(this.txtPercent);
            this.groupBox2.Location = new System.Drawing.Point(3, 26);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(175, 83);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rollback Parameters";
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(6, 61);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(68, 13);
            this.lblBackground.TabIndex = 3;
            this.lblBackground.Text = "Background:";
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(6, 28);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(47, 13);
            this.lblPercent.TabIndex = 2;
            this.lblPercent.Text = "Percent:";
            // 
            // txtBackground
            // 
            this.txtBackground.Location = new System.Drawing.Point(83, 57);
            this.txtBackground.Name = "txtBackground";
            this.txtBackground.Size = new System.Drawing.Size(82, 20);
            this.txtBackground.TabIndex = 1;
            this.txtBackground.Text = "0.00";
            this.txtBackground.TextChanged += new System.EventHandler(this.txtBackground_TextChanged);
            this.txtBackground.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBackground_KeyPress);
            // 
            // txtPercent
            // 
            this.txtPercent.Location = new System.Drawing.Point(83, 25);
            this.txtPercent.Name = "txtPercent";
            this.txtPercent.Size = new System.Drawing.Size(82, 20);
            this.txtPercent.TabIndex = 0;
            this.txtPercent.Text = "0.00";
            this.txtPercent.TextChanged += new System.EventHandler(this.txtPercent_TextChanged);
            this.txtPercent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPercent_KeyPress);
            // 
            // PercentageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Name = "PercentageControl";
            this.Size = new System.Drawing.Size(181, 111);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.TextBox txtBackground;
        private System.Windows.Forms.TextBox txtPercent;
    }
}
