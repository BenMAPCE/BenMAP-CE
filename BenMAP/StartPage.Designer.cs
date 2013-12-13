namespace BenMAP
{
    partial class StartPage
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
            this.components = new System.ComponentModel.Container();
            this._mytimer = new System.Windows.Forms.Timer(this.components);
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblVersion.Location = new System.Drawing.Point(414, 196);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(108, 23);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "version 0.40";
            // 
            // StartPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::BenMAP.Properties.Resources.BenMAP_CE_Start;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(600, 600);
            this.Controls.Add(this.lblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "StartPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StartPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVersion;

    }
}