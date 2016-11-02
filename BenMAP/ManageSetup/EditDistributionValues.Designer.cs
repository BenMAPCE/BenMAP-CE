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
            this.plot1 = new OxyPlot.WindowsForms.PlotView();
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
            // 
            // plot1
            // 
            this.plot1.BackColor = System.Drawing.SystemColors.Control;
            this.plot1.Location = new System.Drawing.Point(4, 12);
            this.plot1.Name = "plot1";
            this.plot1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plot1.Size = new System.Drawing.Size(477, 319);
            this.plot1.TabIndex = 11;
            this.plot1.Text = "plot1";
            this.plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 559);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(511, 56);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(397, 20);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(316, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.plot1);
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
            this.groupBox1.Size = new System.Drawing.Size(511, 529);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(41, 75);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(429, 238);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // lblNotesContext
            // 
            this.lblNotesContext.AutoSize = true;
            this.lblNotesContext.BackColor = System.Drawing.SystemColors.HighlightText;
            this.lblNotesContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNotesContext.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNotesContext.Location = new System.Drawing.Point(76, 495);
            this.lblNotesContext.MinimumSize = new System.Drawing.Size(358, 10);
            this.lblNotesContext.Name = "lblNotesContext";
            this.lblNotesContext.Size = new System.Drawing.Size(358, 16);
            this.lblNotesContext.TabIndex = 10;
            this.lblNotesContext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(76, 440);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(358, 42);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // txtParameter2
            // 
            this.txtParameter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParameter2.Location = new System.Drawing.Point(242, 405);
            this.txtParameter2.Name = "txtParameter2";
            this.txtParameter2.Size = new System.Drawing.Size(90, 22);
            this.txtParameter2.TabIndex = 8;
            this.txtParameter2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtParameter1
            // 
            this.txtParameter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParameter1.Location = new System.Drawing.Point(242, 370);
            this.txtParameter1.Name = "txtParameter1";
            this.txtParameter1.Size = new System.Drawing.Size(90, 22);
            this.txtParameter1.TabIndex = 7;
            this.txtParameter1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMeanValue
            // 
            this.txtMeanValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMeanValue.Location = new System.Drawing.Point(242, 335);
            this.txtMeanValue.Name = "txtMeanValue";
            this.txtMeanValue.Size = new System.Drawing.Size(90, 22);
            this.txtMeanValue.TabIndex = 6;
            this.txtMeanValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPDF
            // 
            this.lblPDF.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPDF.Font = new System.Drawing.Font("Calibri", 20F);
            this.lblPDF.Location = new System.Drawing.Point(3, 18);
            this.lblPDF.Name = "lblPDF";
            this.lblPDF.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPDF.Size = new System.Drawing.Size(505, 33);
            this.lblPDF.TabIndex = 0;
            this.lblPDF.Text = "Exponential PDF:";
            this.lblPDF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblParameter2
            // 
            this.lblParameter2.AutoSize = true;
            this.lblParameter2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParameter2.Location = new System.Drawing.Point(188, 409);
            this.lblParameter2.Name = "lblParameter2";
            this.lblParameter2.Size = new System.Drawing.Size(40, 14);
            this.lblParameter2.TabIndex = 4;
            this.lblParameter2.Text = "label5:";
            this.lblParameter2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblParameter2.Click += new System.EventHandler(this.lblParameter2_Click);
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(76, 480);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(0, 14);
            this.lblNotes.TabIndex = 1;
            this.lblNotes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblParameter1
            // 
            this.lblParameter1.AutoSize = true;
            this.lblParameter1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParameter1.Location = new System.Drawing.Point(188, 374);
            this.lblParameter1.Name = "lblParameter1";
            this.lblParameter1.Size = new System.Drawing.Size(40, 14);
            this.lblParameter1.TabIndex = 3;
            this.lblParameter1.Text = "label4:";
            this.lblParameter1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMeanValue
            // 
            this.lblMeanValue.AutoSize = true;
            this.lblMeanValue.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMeanValue.Location = new System.Drawing.Point(158, 339);
            this.lblMeanValue.Name = "lblMeanValue";
            this.lblMeanValue.Size = new System.Drawing.Size(68, 14);
            this.lblMeanValue.TabIndex = 2;
            this.lblMeanValue.Text = "Mean Value:";
            this.lblMeanValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMeanValue.Click += new System.EventHandler(this.lblMeanValue_Click);
            // 
            // EditDistributionValues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(511, 615);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(517, 515);
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
            this.PerformLayout();

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
        private OxyPlot.WindowsForms.PlotView plot1;
    }
}