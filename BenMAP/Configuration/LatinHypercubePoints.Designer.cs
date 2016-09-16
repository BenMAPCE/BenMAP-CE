namespace BenMAP
{
    partial class LatinHypercubePoints
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
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblAvgWarning = new System.Windows.Forms.Label();
            this.grpIncidenceAvg = new System.Windows.Forms.GroupBox();
            this.rbFiltered = new System.Windows.Forms.RadioButton();
            this.rbAvg = new System.Windows.Forms.RadioButton();
            this.txtRandomSeed = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chbRunInPointMode = new System.Windows.Forms.CheckBox();
            this.lblLatinHypercubePoints = new System.Windows.Forms.Label();
            this.cboLatinHypercubePoints = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.grpIncidenceAvg.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtThreshold
            // 
            this.txtThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThreshold.Location = new System.Drawing.Point(750, 115);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(98, 22);
            this.txtThreshold.TabIndex = 10;
            this.txtThreshold.TextChanged += new System.EventHandler(this.txtThreshold_TextChanged);
            // 
            // lblThreshold
            // 
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(12, 119);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(122, 14);
            this.lblThreshold.TabIndex = 9;
            this.lblThreshold.Text = "Air quality threshold:";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblAvgWarning);
            this.groupBox3.Controls.Add(this.grpIncidenceAvg);
            this.groupBox3.Controls.Add(this.txtRandomSeed);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtThreshold);
            this.groupBox3.Controls.Add(this.chbRunInPointMode);
            this.groupBox3.Controls.Add(this.lblThreshold);
            this.groupBox3.Controls.Add(this.lblLatinHypercubePoints);
            this.groupBox3.Controls.Add(this.cboLatinHypercubePoints);
            this.groupBox3.Location = new System.Drawing.Point(11, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(866, 310);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            // 
            // lblAvgWarning
            // 
            this.lblAvgWarning.Font = new System.Drawing.Font("Calibri", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAvgWarning.Location = new System.Drawing.Point(12, 253);
            this.lblAvgWarning.Name = "lblAvgWarning";
            this.lblAvgWarning.Size = new System.Drawing.Size(854, 54);
            this.lblAvgWarning.TabIndex = 16;
            this.lblAvgWarning.Text = "Warning: If you select option 2, and have not imported rates that match each subg" +
    "roup, the program will return point estimates of zero.";
            this.lblAvgWarning.UseWaitCursor = true;
            // 
            // grpIncidenceAvg
            // 
            this.grpIncidenceAvg.Controls.Add(this.rbFiltered);
            this.grpIncidenceAvg.Controls.Add(this.rbAvg);
            this.grpIncidenceAvg.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpIncidenceAvg.Location = new System.Drawing.Point(15, 143);
            this.grpIncidenceAvg.Name = "grpIncidenceAvg";
            this.grpIncidenceAvg.Size = new System.Drawing.Size(850, 100);
            this.grpIncidenceAvg.TabIndex = 15;
            this.grpIncidenceAvg.TabStop = false;
            this.grpIncidenceAvg.Text = "If you are estimating impacts for specific population subgroups (e.g. race/ethnic" +
    "ity/sex), do you want BenMAP-CE to:";
            // 
            // rbFiltered
            // 
            this.rbFiltered.AutoSize = true;
            this.rbFiltered.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFiltered.Location = new System.Drawing.Point(2, 68);
            this.rbFiltered.Name = "rbFiltered";
            this.rbFiltered.Size = new System.Drawing.Size(831, 18);
            this.rbFiltered.TabIndex = 1;
            this.rbFiltered.TabStop = true;
            this.rbFiltered.Text = "2) Use incidence rates that exactly match each gender/race/ethnicity population s" +
    "ubgroup. (You must import stratified rates for use with this option).\r\n";
            this.rbFiltered.UseVisualStyleBackColor = true;
            this.rbFiltered.CheckedChanged += new System.EventHandler(this.rbFiltered_CheckedChanged);
            // 
            // rbAvg
            // 
            this.rbAvg.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAvg.Location = new System.Drawing.Point(2, 14);
            this.rbAvg.Name = "rbAvg";
            this.rbAvg.Size = new System.Drawing.Size(849, 48);
            this.rbAvg.TabIndex = 0;
            this.rbAvg.TabStop = true;
            this.rbAvg.Text = "\r\n1)  Use incidence rates averaged across gender/race/ethnicity (default).  Use t" +
    "his option with preloaded incidence rates in BenMAP-CE, \r\nas they are not strati" +
    "fied by gender, race, or ethnicity.";
            this.rbAvg.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbAvg.UseVisualStyleBackColor = true;
            this.rbAvg.CheckedChanged += new System.EventHandler(this.rbAvg_CheckedChanged);
            // 
            // txtRandomSeed
            // 
            this.txtRandomSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRandomSeed.Location = new System.Drawing.Point(750, 87);
            this.txtRandomSeed.Name = "txtRandomSeed";
            this.txtRandomSeed.Size = new System.Drawing.Size(98, 22);
            this.txtRandomSeed.TabIndex = 14;
            this.txtRandomSeed.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 14);
            this.label6.TabIndex = 13;
            this.label6.Text = "Random Seed:";
            // 
            // chbRunInPointMode
            // 
            this.chbRunInPointMode.AutoSize = true;
            this.chbRunInPointMode.Location = new System.Drawing.Point(14, 23);
            this.chbRunInPointMode.Name = "chbRunInPointMode";
            this.chbRunInPointMode.Size = new System.Drawing.Size(126, 18);
            this.chbRunInPointMode.TabIndex = 0;
            this.chbRunInPointMode.Text = "Run In Point Mode";
            this.chbRunInPointMode.UseVisualStyleBackColor = true;
            this.chbRunInPointMode.CheckedChanged += new System.EventHandler(this.chbRunInPointMode_CheckedChanged);
            // 
            // lblLatinHypercubePoints
            // 
            this.lblLatinHypercubePoints.AutoSize = true;
            this.lblLatinHypercubePoints.Location = new System.Drawing.Point(12, 63);
            this.lblLatinHypercubePoints.Name = "lblLatinHypercubePoints";
            this.lblLatinHypercubePoints.Size = new System.Drawing.Size(70, 14);
            this.lblLatinHypercubePoints.TabIndex = 1;
            this.lblLatinHypercubePoints.Text = "Percentiles:";
            // 
            // cboLatinHypercubePoints
            // 
            this.cboLatinHypercubePoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLatinHypercubePoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLatinHypercubePoints.FormattingEnabled = true;
            this.cboLatinHypercubePoints.Items.AddRange(new object[] {
            "10",
            "20",
            "50",
            "100"});
            this.cboLatinHypercubePoints.Location = new System.Drawing.Point(750, 59);
            this.cboLatinHypercubePoints.Name = "cboLatinHypercubePoints";
            this.cboLatinHypercubePoints.Size = new System.Drawing.Size(98, 22);
            this.cboLatinHypercubePoints.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(11, 319);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(866, 54);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(783, 18);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 25);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(712, 18);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LatinHypercubePoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 385);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LatinHypercubePoints";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Settings";
            this.Load += new System.EventHandler(this.LatinHypercubePoints_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpIncidenceAvg.ResumeLayout(false);
            this.grpIncidenceAvg.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.ComboBox cboLatinHypercubePoints;
        private System.Windows.Forms.Label lblLatinHypercubePoints;
        private System.Windows.Forms.CheckBox chbRunInPointMode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtThreshold;
        private System.Windows.Forms.TextBox txtRandomSeed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpIncidenceAvg;
        private System.Windows.Forms.RadioButton rbFiltered;
        private System.Windows.Forms.RadioButton rbAvg;
        private System.Windows.Forms.Label lblAvgWarning;
    }
}