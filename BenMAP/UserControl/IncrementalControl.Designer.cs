namespace BenMAP.DataSource
{
    partial class IncrementalControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBackground = new System.Windows.Forms.TextBox();
            this.txtIncrement = new System.Windows.Forms.TextBox();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblIncrement = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBackground);
            this.groupBox1.Controls.Add(this.txtIncrement);
            this.groupBox1.Controls.Add(this.lblBackground);
            this.groupBox1.Controls.Add(this.lblIncrement);
            this.groupBox1.Location = new System.Drawing.Point(3, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(175, 77);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rollback Parameters";
            // 
            // txtBackground
            // 
            this.txtBackground.Location = new System.Drawing.Point(83, 52);
            this.txtBackground.Name = "txtBackground";
            this.txtBackground.Size = new System.Drawing.Size(82, 21);
            this.txtBackground.TabIndex = 3;
            this.txtBackground.Text = "0.00";
            this.txtBackground.TextChanged += new System.EventHandler(this.txtBackground_TextChanged);
            this.txtBackground.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBackground_KeyPress);
            // 
            // txtIncrement
            // 
            this.txtIncrement.Location = new System.Drawing.Point(83, 22);
            this.txtIncrement.Name = "txtIncrement";
            this.txtIncrement.Size = new System.Drawing.Size(82, 21);
            this.txtIncrement.TabIndex = 2;
            this.txtIncrement.Text = "0.00";
            this.txtIncrement.TextChanged += new System.EventHandler(this.txtIncrement_TextChanged);
            this.txtIncrement.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIncrement_KeyPress);
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(6, 55);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(71, 12);
            this.lblBackground.TabIndex = 1;
            this.lblBackground.Text = "Background:";
            // 
            // lblIncrement
            // 
            this.lblIncrement.AutoSize = true;
            this.lblIncrement.Location = new System.Drawing.Point(6, 25);
            this.lblIncrement.Name = "lblIncrement";
            this.lblIncrement.Size = new System.Drawing.Size(65, 12);
            this.lblIncrement.TabIndex = 0;
            this.lblIncrement.Text = "Increment:";
            // 
            // IncrementalControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "IncrementalControl";
            this.Size = new System.Drawing.Size(181, 101);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBackground;
        private System.Windows.Forms.TextBox txtIncrement;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.Label lblIncrement;
    }
}
