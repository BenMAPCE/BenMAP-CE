namespace BenMAP
{
    partial class Pollutant
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cbShowDetails = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstSPollutant = new System.Windows.Forms.ListBox();
            this.lstPollutant = new System.Windows.Forms.ListBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbMetric = new System.Windows.Forms.ComboBox();
            this.txtObservationType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPollutantName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSeasonalMetric = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tclFixed = new System.Windows.Forms.TabControl();
            this.tabFixed = new System.Windows.Forms.TabPage();
            this.cmbStatistic = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtEndHour = new System.Windows.Forms.TextBox();
            this.txtStartHour = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabMoving = new System.Windows.Forms.TabPage();
            this.cmbDaily = new System.Windows.Forms.ComboBox();
            this.cmbWStatistic = new System.Windows.Forms.ComboBox();
            this.txtMovingWindowSize = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tabCustom = new System.Windows.Forms.TabPage();
            this.txtFunction = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tclFixed.SuspendLayout();
            this.tabFixed.SuspendLayout();
            this.tabMoving.SuspendLayout();
            this.tabCustom.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.cbShowDetails);
            this.groupBox3.Controls.Add(this.btnCancel);
            this.groupBox3.Controls.Add(this.btnOK);
            this.groupBox3.Controls.Add(this.btnSelect);
            this.groupBox3.Controls.Add(this.btnDelete);
            this.groupBox3.Controls.Add(this.lstSPollutant);
            this.groupBox3.Controls.Add(this.lstPollutant);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(426, 233);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Main";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label13.Location = new System.Drawing.Point(31, 165);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(134, 14);
            this.label13.TabIndex = 11;
            this.label13.Text = "Drag and drop to select";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label12.Location = new System.Drawing.Point(215, 164);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(128, 14);
            this.label12.TabIndex = 10;
            this.label12.Text = "Double-click to delete";
            // 
            // cbShowDetails
            // 
            this.cbShowDetails.AutoSize = true;
            this.cbShowDetails.Location = new System.Drawing.Point(30, 199);
            this.cbShowDetails.Name = "cbShowDetails";
            this.cbShowDetails.Size = new System.Drawing.Size(119, 18);
            this.cbShowDetails.TabIndex = 6;
            this.cbShowDetails.Text = "Pollutant Details";
            this.cbShowDetails.UseVisualStyleBackColor = true;
            this.cbShowDetails.CheckedChanged += new System.EventHandler(this.cbShowDetails_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancel.Location = new System.Drawing.Point(221, 194);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOK.Location = new System.Drawing.Point(306, 194);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(134, 190);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 27);
            this.btnSelect.TabIndex = 5;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Visible = false;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(64, 194);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstSPollutant
            // 
            this.lstSPollutant.AllowDrop = true;
            this.lstSPollutant.FormattingEnabled = true;
            this.lstSPollutant.ItemHeight = 14;
            this.lstSPollutant.Location = new System.Drawing.Point(212, 42);
            this.lstSPollutant.Name = "lstSPollutant";
            this.lstSPollutant.Size = new System.Drawing.Size(180, 116);
            this.lstSPollutant.TabIndex = 3;
            this.lstSPollutant.SelectedIndexChanged += new System.EventHandler(this.lstSPollutant_SelectedIndexChanged);
            this.lstSPollutant.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstSPollutant_DragDrop);
            this.lstSPollutant.DragOver += new System.Windows.Forms.DragEventHandler(this.lstSPollutant_DragOver);
            this.lstSPollutant.DoubleClick += new System.EventHandler(this.lstSPollutant_DoubleClick);
            // 
            // lstPollutant
            // 
            this.lstPollutant.AllowDrop = true;
            this.lstPollutant.FormattingEnabled = true;
            this.lstPollutant.ItemHeight = 14;
            this.lstPollutant.Location = new System.Drawing.Point(29, 42);
            this.lstPollutant.Name = "lstPollutant";
            this.lstPollutant.Size = new System.Drawing.Size(160, 116);
            this.lstPollutant.TabIndex = 2;
            this.lstPollutant.SelectedIndexChanged += new System.EventHandler(this.lstPollutant_SelectedIndexChanged);
            this.lstPollutant.DoubleClick += new System.EventHandler(this.lstPollutant_DoubleClick);
            this.lstPollutant.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstPollutant_MouseDown);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(209, 24);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(116, 14);
            this.label19.TabIndex = 1;
            this.label19.Text = "Selected Pollutants:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(27, 24);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(66, 14);
            this.label18.TabIndex = 0;
            this.label18.Text = "Pollutants:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbMetric);
            this.groupBox2.Controls.Add(this.txtObservationType);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtPollutantName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cmbSeasonalMetric);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tclFixed);
            this.groupBox2.Location = new System.Drawing.Point(12, 259);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(426, 240);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Details";
            // 
            // cmbMetric
            // 
            this.cmbMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetric.FormattingEnabled = true;
            this.cmbMetric.Items.AddRange(new object[] {
            "D24HourMean"});
            this.cmbMetric.Location = new System.Drawing.Point(75, 52);
            this.cmbMetric.Name = "cmbMetric";
            this.cmbMetric.Size = new System.Drawing.Size(114, 22);
            this.cmbMetric.TabIndex = 5;
            this.cmbMetric.SelectedIndexChanged += new System.EventHandler(this.cmbMetric_SelectedIndexChanged);
            // 
            // txtObservationType
            // 
            this.txtObservationType.Location = new System.Drawing.Point(299, 20);
            this.txtObservationType.Name = "txtObservationType";
            this.txtObservationType.ReadOnly = true;
            this.txtObservationType.Size = new System.Drawing.Size(112, 22);
            this.txtObservationType.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "Metric:";
            // 
            // txtPollutantName
            // 
            this.txtPollutantName.Location = new System.Drawing.Point(75, 20);
            this.txtPollutantName.Name = "txtPollutantName";
            this.txtPollutantName.ReadOnly = true;
            this.txtPollutantName.Size = new System.Drawing.Size(114, 22);
            this.txtPollutantName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(198, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "Observation Type:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pollutant:";
            // 
            // cmbSeasonalMetric
            // 
            this.cmbSeasonalMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSeasonalMetric.FormattingEnabled = true;
            this.cmbSeasonalMetric.Items.AddRange(new object[] {
            "QuarterlyMean",
            "(More...)"});
            this.cmbSeasonalMetric.Location = new System.Drawing.Point(299, 52);
            this.cmbSeasonalMetric.Name = "cmbSeasonalMetric";
            this.cmbSeasonalMetric.Size = new System.Drawing.Size(112, 22);
            this.cmbSeasonalMetric.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(198, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 14);
            this.label8.TabIndex = 2;
            this.label8.Text = "Seasonal Metric:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 14);
            this.label4.TabIndex = 0;
            this.label4.Text = "Hourly Metric";
            // 
            // tclFixed
            // 
            this.tclFixed.Controls.Add(this.tabFixed);
            this.tclFixed.Controls.Add(this.tabMoving);
            this.tclFixed.Controls.Add(this.tabCustom);
            this.tclFixed.Location = new System.Drawing.Point(7, 104);
            this.tclFixed.Name = "tclFixed";
            this.tclFixed.SelectedIndex = 0;
            this.tclFixed.Size = new System.Drawing.Size(404, 130);
            this.tclFixed.TabIndex = 1;
            // 
            // tabFixed
            // 
            this.tabFixed.Controls.Add(this.cmbStatistic);
            this.tabFixed.Controls.Add(this.label7);
            this.tabFixed.Controls.Add(this.txtEndHour);
            this.tabFixed.Controls.Add(this.txtStartHour);
            this.tabFixed.Controls.Add(this.label6);
            this.tabFixed.Controls.Add(this.label5);
            this.tabFixed.Location = new System.Drawing.Point(4, 23);
            this.tabFixed.Name = "tabFixed";
            this.tabFixed.Padding = new System.Windows.Forms.Padding(3);
            this.tabFixed.Size = new System.Drawing.Size(396, 103);
            this.tabFixed.TabIndex = 0;
            this.tabFixed.Text = "Fixed Window";
            this.tabFixed.UseVisualStyleBackColor = true;
            // 
            // cmbStatistic
            // 
            this.cmbStatistic.FormattingEnabled = true;
            this.cmbStatistic.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cmbStatistic.Location = new System.Drawing.Point(78, 70);
            this.cmbStatistic.Name = "cmbStatistic";
            this.cmbStatistic.Size = new System.Drawing.Size(100, 22);
            this.cmbStatistic.TabIndex = 5;
            this.cmbStatistic.Text = "Mean";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 14);
            this.label7.TabIndex = 4;
            this.label7.Text = "Statistic:";
            // 
            // txtEndHour
            // 
            this.txtEndHour.Location = new System.Drawing.Point(78, 37);
            this.txtEndHour.Name = "txtEndHour";
            this.txtEndHour.ReadOnly = true;
            this.txtEndHour.Size = new System.Drawing.Size(100, 22);
            this.txtEndHour.TabIndex = 3;
            this.txtEndHour.Text = "23";
            // 
            // txtStartHour
            // 
            this.txtStartHour.Location = new System.Drawing.Point(78, 5);
            this.txtStartHour.Name = "txtStartHour";
            this.txtStartHour.ReadOnly = true;
            this.txtStartHour.Size = new System.Drawing.Size(100, 22);
            this.txtStartHour.TabIndex = 1;
            this.txtStartHour.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 14);
            this.label6.TabIndex = 2;
            this.label6.Text = "End Hour:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 14);
            this.label5.TabIndex = 0;
            this.label5.Text = "Start Hour:";
            // 
            // tabMoving
            // 
            this.tabMoving.Controls.Add(this.cmbDaily);
            this.tabMoving.Controls.Add(this.cmbWStatistic);
            this.tabMoving.Controls.Add(this.txtMovingWindowSize);
            this.tabMoving.Controls.Add(this.label11);
            this.tabMoving.Controls.Add(this.label10);
            this.tabMoving.Controls.Add(this.label9);
            this.tabMoving.Location = new System.Drawing.Point(4, 23);
            this.tabMoving.Name = "tabMoving";
            this.tabMoving.Padding = new System.Windows.Forms.Padding(3);
            this.tabMoving.Size = new System.Drawing.Size(396, 103);
            this.tabMoving.TabIndex = 1;
            this.tabMoving.Text = "Moving Window";
            this.tabMoving.UseVisualStyleBackColor = true;
            // 
            // cmbDaily
            // 
            this.cmbDaily.FormattingEnabled = true;
            this.cmbDaily.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cmbDaily.Location = new System.Drawing.Point(127, 72);
            this.cmbDaily.Name = "cmbDaily";
            this.cmbDaily.Size = new System.Drawing.Size(121, 22);
            this.cmbDaily.TabIndex = 5;
            this.cmbDaily.Text = "Mean";
            // 
            // cmbWStatistic
            // 
            this.cmbWStatistic.FormattingEnabled = true;
            this.cmbWStatistic.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cmbWStatistic.Location = new System.Drawing.Point(127, 38);
            this.cmbWStatistic.Name = "cmbWStatistic";
            this.cmbWStatistic.Size = new System.Drawing.Size(121, 22);
            this.cmbWStatistic.TabIndex = 4;
            this.cmbWStatistic.Text = "Mean";
            // 
            // txtMovingWindowSize
            // 
            this.txtMovingWindowSize.Location = new System.Drawing.Point(127, 7);
            this.txtMovingWindowSize.Name = "txtMovingWindowSize";
            this.txtMovingWindowSize.ReadOnly = true;
            this.txtMovingWindowSize.Size = new System.Drawing.Size(121, 22);
            this.txtMovingWindowSize.TabIndex = 3;
            this.txtMovingWindowSize.Text = "1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 14);
            this.label11.TabIndex = 2;
            this.label11.Text = "Daily Statistic:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 14);
            this.label10.TabIndex = 1;
            this.label10.Text = "Window Statistic:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 14);
            this.label9.TabIndex = 0;
            this.label9.Text = "Window Size:";
            // 
            // tabCustom
            // 
            this.tabCustom.Controls.Add(this.txtFunction);
            this.tabCustom.Controls.Add(this.label14);
            this.tabCustom.Location = new System.Drawing.Point(4, 23);
            this.tabCustom.Name = "tabCustom";
            this.tabCustom.Size = new System.Drawing.Size(396, 103);
            this.tabCustom.TabIndex = 2;
            this.tabCustom.Text = "Custom";
            this.tabCustom.UseVisualStyleBackColor = true;
            // 
            // txtFunction
            // 
            this.txtFunction.Location = new System.Drawing.Point(78, 58);
            this.txtFunction.Multiline = true;
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.Size = new System.Drawing.Size(294, 20);
            this.txtFunction.TabIndex = 5;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(19, 58);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 14);
            this.label14.TabIndex = 4;
            this.label14.Text = "Function:";
            // 
            // Pollutant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 256);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Pollutant";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pollutant Definition";
            this.Load += new System.EventHandler(this.Pollutant_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tclFixed.ResumeLayout(false);
            this.tabFixed.ResumeLayout(false);
            this.tabFixed.PerformLayout();
            this.tabMoving.ResumeLayout(false);
            this.tabMoving.PerformLayout();
            this.tabCustom.ResumeLayout(false);
            this.tabCustom.PerformLayout();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPollutantName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbMetric;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbSeasonalMetric;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabControl tclFixed;
        private System.Windows.Forms.TabPage tabFixed;
        private System.Windows.Forms.ComboBox cmbStatistic;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtEndHour;
        private System.Windows.Forms.TextBox txtStartHour;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabMoving;
        private System.Windows.Forms.ComboBox cmbDaily;
        private System.Windows.Forms.ComboBox cmbWStatistic;
        private System.Windows.Forms.TextBox txtMovingWindowSize;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage tabCustom;
        private System.Windows.Forms.TextBox txtFunction;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstSPollutant;
        private System.Windows.Forms.ListBox lstPollutant;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtObservationType;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.CheckBox cbShowDetails;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
    }
}