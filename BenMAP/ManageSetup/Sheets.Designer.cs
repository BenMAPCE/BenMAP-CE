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
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lstSheets
			// 
			this.lstSheets.FormattingEnabled = true;
			this.lstSheets.ItemHeight = 14;
			this.lstSheets.Location = new System.Drawing.Point(4, 21);
			this.lstSheets.Name = "lstSheets";
			this.lstSheets.Size = new System.Drawing.Size(196, 88);
			this.lstSheets.TabIndex = 0;
			this.lstSheets.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstSheets_MouseDoubleClick);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(125, 114);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(2, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(127, 14);
			this.label1.TabIndex = 2;
			this.label1.Text = "Please select a sheet:";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// Sheets
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(204, 142);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lstSheets);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(212, 130);
			this.Name = "Sheets";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Sheets";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.Sheets_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		private System.Windows.Forms.ListBox lstSheets;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
	}
}