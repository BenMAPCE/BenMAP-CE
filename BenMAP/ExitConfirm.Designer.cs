namespace BenMAP
{
	partial class ExitConfirm
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
			this.chkCloseTip = new System.Windows.Forms.CheckBox();
			this.btnNO = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// chkCloseTip
			// 
			this.chkCloseTip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkCloseTip.AutoSize = true;
			this.chkCloseTip.Location = new System.Drawing.Point(5, 68);
			this.chkCloseTip.Name = "chkCloseTip";
			this.chkCloseTip.Size = new System.Drawing.Size(270, 18);
			this.chkCloseTip.TabIndex = 3;
			this.chkCloseTip.Text = "Don\'t show this confirmation message again.";
			this.chkCloseTip.UseVisualStyleBackColor = true;
			// 
			// btnNO
			// 
			this.btnNO.Location = new System.Drawing.Point(194, 37);
			this.btnNO.Name = "btnNO";
			this.btnNO.Size = new System.Drawing.Size(75, 23);
			this.btnNO.TabIndex = 2;
			this.btnNO.Text = "No";
			this.btnNO.UseVisualStyleBackColor = true;
			this.btnNO.Click += new System.EventHandler(this.btnNO_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(113, 37);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "Yes";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(95, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Exit BenMAP-CE?\r\n";
			// 
			// ExitConfirm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(281, 98);
			this.Controls.Add(this.chkCloseTip);
			this.Controls.Add(this.btnNO);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ExitConfirm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Exit";
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnNO;
		private System.Windows.Forms.CheckBox chkCloseTip;
	}
}