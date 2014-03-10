namespace BenMAP
{
    partial class RenameorOverride
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
            this.btnRename = new System.Windows.Forms.Button();
            this.btnOverride = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
                                                this.btnRename.Location = new System.Drawing.Point(98, 68);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(133, 23);
            this.btnRename.TabIndex = 0;
            this.btnRename.Text = "Rename and Import";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
                                                this.btnOverride.Location = new System.Drawing.Point(237, 68);
            this.btnOverride.Name = "btnOverride";
            this.btnOverride.Size = new System.Drawing.Size(75, 23);
            this.btnOverride.TabIndex = 0;
            this.btnOverride.Text = "Overwrite";
            this.btnOverride.UseVisualStyleBackColor = true;
            this.btnOverride.Click += new System.EventHandler(this.btnOverride_Click);
                                                this.label1.Location = new System.Drawing.Point(5, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 33);
            this.label1.TabIndex = 1;
            this.label1.Text = "The shapefile name already exists. Rename the loaded shapefile, or overwrite the " +
                "existing one?";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 110);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOverride);
            this.Controls.Add(this.btnRename);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(330, 138);
            this.Name = "RenameorOverride";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rename or Overwrite";
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnOverride;
        private System.Windows.Forms.Label label1;
    }
}