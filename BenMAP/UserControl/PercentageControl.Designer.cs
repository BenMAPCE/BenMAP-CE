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
                                                this.groupBox2.Controls.Add(this.lblBackground);
            this.groupBox2.Controls.Add(this.lblPercent);
            this.groupBox2.Controls.Add(this.txtBackground);
            this.groupBox2.Controls.Add(this.txtPercent);
            this.groupBox2.Location = new System.Drawing.Point(3, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(175, 77);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rollback Parameters";
                                                this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(6, 56);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(71, 12);
            this.lblBackground.TabIndex = 3;
            this.lblBackground.Text = "Background:";
                                                this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(6, 26);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(53, 12);
            this.lblPercent.TabIndex = 2;
            this.lblPercent.Text = "Percent:";
                                                this.txtBackground.Location = new System.Drawing.Point(83, 53);
            this.txtBackground.Name = "txtBackground";
            this.txtBackground.Size = new System.Drawing.Size(82, 21);
            this.txtBackground.TabIndex = 1;
            this.txtBackground.Text = "0.00";
            this.txtBackground.TextChanged += new System.EventHandler(this.txtBackground_TextChanged);
            this.txtBackground.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBackground_KeyPress);
                                                this.txtPercent.Location = new System.Drawing.Point(83, 23);
            this.txtPercent.Name = "txtPercent";
            this.txtPercent.Size = new System.Drawing.Size(82, 21);
            this.txtPercent.TabIndex = 0;
            this.txtPercent.Text = "0.00";
            this.txtPercent.TextChanged += new System.EventHandler(this.txtPercent_TextChanged);
            this.txtPercent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPercent_KeyPress);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Name = "PercentageControl";
            this.Size = new System.Drawing.Size(181, 101);
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
