namespace BenMAP
{
    partial class EditDistributionValues
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblNotesContext = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtParameter2 = new System.Windows.Forms.TextBox();
            this.txtParameter1 = new System.Windows.Forms.TextBox();
            this.txtMeanValue = new System.Windows.Forms.TextBox();
            this.lblPDF = new System.Windows.Forms.Label();
            this.lblParameter2 = new System.Windows.Forms.Label();
            this.lblNotes = new System.Windows.Forms.Label();
            this.lblParameter1 = new System.Windows.Forms.Label();
            this.lblMeanValue = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
                                                this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 406);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(511, 61);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
                                                this.btnOK.Location = new System.Drawing.Point(392, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Location = new System.Drawing.Point(311, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.lblNotesContext);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.txtParameter2);
            this.groupBox1.Controls.Add(this.txtParameter1);
            this.groupBox1.Controls.Add(this.txtMeanValue);
            this.groupBox1.Controls.Add(this.lblPDF);
            this.groupBox1.Controls.Add(this.lblParameter2);
            this.groupBox1.Controls.Add(this.lblNotes);
            this.groupBox1.Controls.Add(this.lblParameter1);
            this.groupBox1.Controls.Add(this.lblMeanValue);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 407);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
                                                this.pictureBox2.Location = new System.Drawing.Point(109, 74);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(358, 185);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
                                                this.lblNotesContext.AutoSize = true;
            this.lblNotesContext.BackColor = System.Drawing.SystemColors.HighlightText;
            this.lblNotesContext.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNotesContext.Location = new System.Drawing.Point(109, 262);
            this.lblNotesContext.Name = "lblNotesContext";
            this.lblNotesContext.Size = new System.Drawing.Size(42, 14);
            this.lblNotesContext.TabIndex = 10;
            this.lblNotesContext.Text = "label1";
                                                this.pictureBox1.Location = new System.Drawing.Point(109, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(358, 58);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
                                                this.txtParameter2.Location = new System.Drawing.Point(109, 378);
            this.txtParameter2.Name = "txtParameter2";
            this.txtParameter2.Size = new System.Drawing.Size(119, 22);
            this.txtParameter2.TabIndex = 8;
                                                this.txtParameter1.Location = new System.Drawing.Point(109, 350);
            this.txtParameter1.Name = "txtParameter1";
            this.txtParameter1.Size = new System.Drawing.Size(119, 22);
            this.txtParameter1.TabIndex = 7;
                                                this.txtMeanValue.Location = new System.Drawing.Point(109, 322);
            this.txtMeanValue.Name = "txtMeanValue";
            this.txtMeanValue.Size = new System.Drawing.Size(119, 22);
            this.txtMeanValue.TabIndex = 6;
                                                this.lblPDF.AutoSize = true;
            this.lblPDF.Location = new System.Drawing.Point(3, 20);
            this.lblPDF.Name = "lblPDF";
            this.lblPDF.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPDF.Size = new System.Drawing.Size(97, 14);
            this.lblPDF.TabIndex = 0;
            this.lblPDF.Text = "Exponential PDF:";
            this.lblPDF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                                                this.lblParameter2.AutoSize = true;
            this.lblParameter2.Location = new System.Drawing.Point(55, 382);
            this.lblParameter2.Name = "lblParameter2";
            this.lblParameter2.Size = new System.Drawing.Size(45, 14);
            this.lblParameter2.TabIndex = 4;
            this.lblParameter2.Text = "label5:";
            this.lblParameter2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblParameter2.Click += new System.EventHandler(this.lblParameter2_Click);
                                                this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(58, 262);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(42, 14);
            this.lblNotes.TabIndex = 1;
            this.lblNotes.Text = "Notes:";
            this.lblNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                                                this.lblParameter1.AutoSize = true;
            this.lblParameter1.Location = new System.Drawing.Point(55, 354);
            this.lblParameter1.Name = "lblParameter1";
            this.lblParameter1.Size = new System.Drawing.Size(45, 14);
            this.lblParameter1.TabIndex = 3;
            this.lblParameter1.Text = "label4:";
            this.lblParameter1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                                                this.lblMeanValue.AutoSize = true;
            this.lblMeanValue.Location = new System.Drawing.Point(25, 326);
            this.lblMeanValue.Name = "lblMeanValue";
            this.lblMeanValue.Size = new System.Drawing.Size(75, 14);
            this.lblMeanValue.TabIndex = 2;
            this.lblMeanValue.Text = "Mean Value:";
            this.lblMeanValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMeanValue.Click += new System.EventHandler(this.lblMeanValue_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 467);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(517, 495);
            this.Name = "EditDistributionValues";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Distribution Values";
            this.Load += new System.EventHandler(this.EditDistributionValues_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label lblPDF;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.Label lblMeanValue;
        private System.Windows.Forms.Label lblParameter1;
        private System.Windows.Forms.Label lblParameter2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtParameter2;
        private System.Windows.Forms.TextBox txtParameter1;
        private System.Windows.Forms.TextBox txtMeanValue;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblNotesContext;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}