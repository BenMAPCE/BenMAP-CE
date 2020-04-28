namespace BenMAP
{
	partial class PopulationTipForm
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			this.progressBar1.Location = new System.Drawing.Point(19, 31);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(282, 23);
			this.progressBar1.TabIndex = 0;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 14);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(317, 81);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.progressBar1);
			this.Name = "PopulationTipForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Saving...";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PopulationTipForm_FormClosed);
			this.Load += new System.EventHandler(this.PopulationTipForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label1;
	}
}