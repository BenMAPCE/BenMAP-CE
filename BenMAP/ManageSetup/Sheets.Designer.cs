namespace BenMAP
{
    partial class Sheets
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
            this.lstSheets = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
                                                this.lstSheets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSheets.FormattingEnabled = true;
            this.lstSheets.ItemHeight = 14;
            this.lstSheets.Location = new System.Drawing.Point(0, 0);
            this.lstSheets.Name = "lstSheets";
            this.lstSheets.Size = new System.Drawing.Size(196, 92);
            this.lstSheets.TabIndex = 0;
            this.lstSheets.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstSheets_MouseDoubleClick);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(196, 92);
            this.Controls.Add(this.lstSheets);
            this.MinimumSize = new System.Drawing.Size(212, 130);
            this.Name = "Sheets";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sheets";
            this.Load += new System.EventHandler(this.Sheets_Load);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.ListBox lstSheets;
    }
}