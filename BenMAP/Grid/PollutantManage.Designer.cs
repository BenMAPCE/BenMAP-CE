namespace BenMAP
{
    partial class PollutantManage
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lstSMetric = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dmudownStartMonth = new System.Windows.Forms.DomainUpDown();
            this.dmudownEndMonth = new System.Windows.Forms.DomainUpDown();
            this.nudownEndDate = new System.Windows.Forms.NumericUpDown();
            this.nudownStartDate = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.lstMetric = new System.Windows.Forms.ListBox();
            this.btnSDelete = new System.Windows.Forms.Button();
            this.btnSAdd = new System.Windows.Forms.Button();
            this.btnSSDelete = new System.Windows.Forms.Button();
            this.btnSSAdd = new System.Windows.Forms.Button();
            this.gpbManage = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSeasonalMetric = new System.Windows.Forms.TextBox();
            this.tabSDetails = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtFunctionManage = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lstSAvailableVaribles = new System.Windows.Forms.ListBox();
            this.lstSAvailableFunctions = new System.Windows.Forms.ListBox();
            this.btnPMcancel = new System.Windows.Forms.Button();
            this.btnPMok = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudownEndDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownStartDate)).BeginInit();
            this.gpbManage.SuspendLayout();
            this.tabSDetails.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
                                                this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seasonal Metric Seasons";
                                                this.lstSMetric.FormattingEnabled = true;
            this.lstSMetric.ItemHeight = 14;
            this.lstSMetric.Location = new System.Drawing.Point(6, 217);
            this.lstSMetric.Name = "lstSMetric";
            this.lstSMetric.Size = new System.Drawing.Size(145, 74);
            this.lstSMetric.TabIndex = 1;
            this.lstSMetric.SelectedIndexChanged += new System.EventHandler(this.lstSMetric_SelectedIndexChanged);
                                                this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 14);
            this.label2.TabIndex = 2;
                                                this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(173, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 14);
            this.label3.TabIndex = 3;
            this.label3.Text = "Season Details";
                                                this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(173, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 14);
            this.label4.TabIndex = 4;
            this.label4.Text = "Start Date";
                                                this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(417, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 14);
            this.label5.TabIndex = 5;
            this.label5.Text = "End Date";
                                                this.dmudownStartMonth.Enabled = false;
            this.dmudownStartMonth.Items.Add("一月");
            this.dmudownStartMonth.Items.Add("二月");
            this.dmudownStartMonth.Items.Add("三月");
            this.dmudownStartMonth.Items.Add("四月");
            this.dmudownStartMonth.Items.Add("五月");
            this.dmudownStartMonth.Items.Add("六月");
            this.dmudownStartMonth.Items.Add("七月");
            this.dmudownStartMonth.Items.Add("八月");
            this.dmudownStartMonth.Items.Add("九月");
            this.dmudownStartMonth.Items.Add("十月");
            this.dmudownStartMonth.Items.Add("十一月");
            this.dmudownStartMonth.Items.Add("十二月");
            this.dmudownStartMonth.Location = new System.Drawing.Point(262, 59);
            this.dmudownStartMonth.Name = "dmudownStartMonth";
            this.dmudownStartMonth.ReadOnly = true;
            this.dmudownStartMonth.Size = new System.Drawing.Size(54, 22);
            this.dmudownStartMonth.TabIndex = 12;
            this.dmudownStartMonth.Text = "一月";
            this.dmudownStartMonth.Wrap = true;
            this.dmudownStartMonth.TextChanged += new System.EventHandler(this.dmudownStartM_TextChanged);
                                                this.dmudownEndMonth.Enabled = false;
            this.dmudownEndMonth.Items.Add("一月");
            this.dmudownEndMonth.Items.Add("二月");
            this.dmudownEndMonth.Items.Add("三月");
            this.dmudownEndMonth.Items.Add("四月");
            this.dmudownEndMonth.Items.Add("五月");
            this.dmudownEndMonth.Items.Add("六月");
            this.dmudownEndMonth.Items.Add("七月");
            this.dmudownEndMonth.Items.Add("八月");
            this.dmudownEndMonth.Items.Add("九月");
            this.dmudownEndMonth.Items.Add("十月");
            this.dmudownEndMonth.Items.Add("十一月");
            this.dmudownEndMonth.Items.Add("十二月");
            this.dmudownEndMonth.Location = new System.Drawing.Point(476, 59);
            this.dmudownEndMonth.Name = "dmudownEndMonth";
            this.dmudownEndMonth.ReadOnly = true;
            this.dmudownEndMonth.Size = new System.Drawing.Size(54, 22);
            this.dmudownEndMonth.TabIndex = 11;
            this.dmudownEndMonth.Text = "十二月";
            this.dmudownEndMonth.Wrap = true;
            this.dmudownEndMonth.TextChanged += new System.EventHandler(this.dmudownEndMonth_TextChanged);
                                                this.nudownEndDate.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.nudownEndDate.Enabled = false;
            this.nudownEndDate.Location = new System.Drawing.Point(536, 59);
            this.nudownEndDate.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudownEndDate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudownEndDate.Name = "nudownEndDate";
            this.nudownEndDate.Size = new System.Drawing.Size(46, 22);
            this.nudownEndDate.TabIndex = 15;
            this.nudownEndDate.Value = new decimal(new int[] {
            31,
            0,
            0,
            0});
                                                this.nudownStartDate.Enabled = false;
            this.nudownStartDate.Location = new System.Drawing.Point(322, 59);
            this.nudownStartDate.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudownStartDate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudownStartDate.Name = "nudownStartDate";
            this.nudownStartDate.ReadOnly = true;
            this.nudownStartDate.Size = new System.Drawing.Size(46, 22);
            this.nudownStartDate.TabIndex = 14;
            this.nudownStartDate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
                                                this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 14);
            this.label6.TabIndex = 13;
            this.label6.Text = "Seasonal Metric";
                                                this.lstMetric.FormattingEnabled = true;
            this.lstMetric.ItemHeight = 14;
            this.lstMetric.Location = new System.Drawing.Point(4, 54);
            this.lstMetric.Name = "lstMetric";
            this.lstMetric.Size = new System.Drawing.Size(147, 74);
            this.lstMetric.TabIndex = 14;
                                                this.btnSDelete.Location = new System.Drawing.Point(8, 168);
            this.btnSDelete.Name = "btnSDelete";
            this.btnSDelete.Size = new System.Drawing.Size(54, 28);
            this.btnSDelete.TabIndex = 15;
            this.btnSDelete.Text = "Delete";
            this.btnSDelete.UseVisualStyleBackColor = true;
            this.btnSDelete.Click += new System.EventHandler(this.btnSDelete_Click);
                                                this.btnSAdd.Location = new System.Drawing.Point(88, 169);
            this.btnSAdd.Name = "btnSAdd";
            this.btnSAdd.Size = new System.Drawing.Size(57, 27);
            this.btnSAdd.TabIndex = 16;
            this.btnSAdd.Text = "Add";
            this.btnSAdd.UseVisualStyleBackColor = true;
            this.btnSAdd.Click += new System.EventHandler(this.btnSAdd_Click);
                                                this.btnSSDelete.Location = new System.Drawing.Point(6, 303);
            this.btnSSDelete.Name = "btnSSDelete";
            this.btnSSDelete.Size = new System.Drawing.Size(54, 27);
            this.btnSSDelete.TabIndex = 17;
            this.btnSSDelete.Text = "Delete";
            this.btnSSDelete.UseVisualStyleBackColor = true;
            this.btnSSDelete.Click += new System.EventHandler(this.btnSSDelete_Click);
                                                this.btnSSAdd.Location = new System.Drawing.Point(88, 303);
            this.btnSSAdd.Name = "btnSSAdd";
            this.btnSSAdd.Size = new System.Drawing.Size(57, 27);
            this.btnSSAdd.TabIndex = 18;
            this.btnSSAdd.Text = "Add";
            this.btnSSAdd.UseVisualStyleBackColor = true;
            this.btnSSAdd.Click += new System.EventHandler(this.btnSSAdd_Click);
                                                this.gpbManage.Controls.Add(this.nudownEndDate);
            this.gpbManage.Controls.Add(this.label12);
            this.gpbManage.Controls.Add(this.dmudownEndMonth);
            this.gpbManage.Controls.Add(this.nudownStartDate);
            this.gpbManage.Controls.Add(this.label5);
            this.gpbManage.Controls.Add(this.txtSeasonalMetric);
            this.gpbManage.Controls.Add(this.label4);
            this.gpbManage.Controls.Add(this.dmudownStartMonth);
            this.gpbManage.Controls.Add(this.label6);
            this.gpbManage.Controls.Add(this.label3);
            this.gpbManage.Controls.Add(this.tabSDetails);
            this.gpbManage.Controls.Add(this.btnSSAdd);
            this.gpbManage.Controls.Add(this.lstMetric);
            this.gpbManage.Controls.Add(this.btnSSDelete);
            this.gpbManage.Controls.Add(this.btnSDelete);
            this.gpbManage.Controls.Add(this.btnSAdd);
            this.gpbManage.Controls.Add(this.label1);
            this.gpbManage.Controls.Add(this.lstSMetric);
            this.gpbManage.Location = new System.Drawing.Point(14, 14);
            this.gpbManage.Name = "gpbManage";
            this.gpbManage.Size = new System.Drawing.Size(618, 346);
            this.gpbManage.TabIndex = 19;
            this.gpbManage.TabStop = false;
            this.gpbManage.Text = "Manage Season";
                                                this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 136);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 14);
            this.label12.TabIndex = 20;
            this.label12.Text = "Item";
                                                this.txtSeasonalMetric.Location = new System.Drawing.Point(41, 133);
            this.txtSeasonalMetric.Name = "txtSeasonalMetric";
            this.txtSeasonalMetric.Size = new System.Drawing.Size(110, 22);
            this.txtSeasonalMetric.TabIndex = 19;
                                                this.tabSDetails.Controls.Add(this.tabPage2);
            this.tabSDetails.Controls.Add(this.tabPage3);
            this.tabSDetails.Location = new System.Drawing.Point(175, 85);
            this.tabSDetails.Name = "tabSDetails";
            this.tabSDetails.SelectedIndex = 0;
            this.tabSDetails.Size = new System.Drawing.Size(443, 245);
            this.tabSDetails.TabIndex = 12;
                                                this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.comboBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(435, 218);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Statistic";
            this.tabPage2.UseVisualStyleBackColor = true;
                                                this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(0, 17);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(49, 14);
            this.label16.TabIndex = 3;
            this.label16.Text = "Statistic";
                                                this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 8);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(0, 14);
            this.label15.TabIndex = 2;
                                                this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.comboBox1.Location = new System.Drawing.Point(65, 14);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(95, 22);
            this.comboBox1.TabIndex = 1;
                                                this.tabPage3.Controls.Add(this.label14);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.txtFunctionManage);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.lstSAvailableVaribles);
            this.tabPage3.Controls.Add(this.lstSAvailableFunctions);
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(435, 218);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Custom Function";
            this.tabPage3.UseVisualStyleBackColor = true;
                                                this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(205, 113);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 14);
            this.label14.TabIndex = 9;
                                                this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 113);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(0, 14);
            this.label13.TabIndex = 8;
                                                this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(205, 154);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 14);
            this.label11.TabIndex = 7;
                                                this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 154);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 14);
            this.label10.TabIndex = 6;
                                                this.txtFunctionManage.Location = new System.Drawing.Point(65, 171);
            this.txtFunctionManage.Name = "txtFunctionManage";
            this.txtFunctionManage.Size = new System.Drawing.Size(360, 22);
            this.txtFunctionManage.TabIndex = 5;
                                                this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 171);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 14);
            this.label9.TabIndex = 4;
            this.label9.Text = "Function";
                                                this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(205, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 14);
            this.label8.TabIndex = 3;
            this.label8.Text = "Available Variables";
                                                this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 14);
            this.label7.TabIndex = 2;
            this.label7.Text = "Available Functions";
                                                this.lstSAvailableVaribles.FormattingEnabled = true;
            this.lstSAvailableVaribles.ItemHeight = 14;
            this.lstSAvailableVaribles.Items.AddRange(new object[] {
            "MetricValues[]",
            "SeasonalMetricValues[]",
            "SortedMetricValues[]",
            "StartDay",
            "EndDay",
            "Mean",
            "Median",
            "Min",
            "Max",
            "Sum",
            "NoObservation"});
            this.lstSAvailableVaribles.Location = new System.Drawing.Point(207, 21);
            this.lstSAvailableVaribles.Name = "lstSAvailableVaribles";
            this.lstSAvailableVaribles.Size = new System.Drawing.Size(218, 88);
            this.lstSAvailableVaribles.TabIndex = 1;
            this.lstSAvailableVaribles.Click += new System.EventHandler(this.lstSAvailableVaribles_Click);
            this.lstSAvailableVaribles.DoubleClick += new System.EventHandler(this.lstSAvailableVaribles_DoubleClick);
                                                this.lstSAvailableFunctions.FormattingEnabled = true;
            this.lstSAvailableFunctions.ItemHeight = 14;
            this.lstSAvailableFunctions.Items.AddRange(new object[] {
            "ABS(x)",
            "EXP(x)",
            "IPOWER(x,y)",
            "LN(x)",
            "POWER(x,y)",
            "SQR(x)",
            "SQRT(x)"});
            this.lstSAvailableFunctions.Location = new System.Drawing.Point(8, 21);
            this.lstSAvailableFunctions.Name = "lstSAvailableFunctions";
            this.lstSAvailableFunctions.Size = new System.Drawing.Size(191, 88);
            this.lstSAvailableFunctions.TabIndex = 0;
            this.lstSAvailableFunctions.Click += new System.EventHandler(this.lstSAvailableFunctions_Click);
            this.lstSAvailableFunctions.DoubleClick += new System.EventHandler(this.lstSAvailableFunctions_DoubleClick);
                                                this.btnPMcancel.Location = new System.Drawing.Point(419, 367);
            this.btnPMcancel.Name = "btnPMcancel";
            this.btnPMcancel.Size = new System.Drawing.Size(65, 27);
            this.btnPMcancel.TabIndex = 20;
            this.btnPMcancel.Text = "Cancel";
            this.btnPMcancel.UseVisualStyleBackColor = true;
            this.btnPMcancel.Click += new System.EventHandler(this.btnPMcancel_Click);
                                                this.btnPMok.Location = new System.Drawing.Point(531, 367);
            this.btnPMok.Name = "btnPMok";
            this.btnPMok.Size = new System.Drawing.Size(65, 27);
            this.btnPMok.TabIndex = 21;
            this.btnPMok.Text = "OK";
            this.btnPMok.UseVisualStyleBackColor = true;
            this.btnPMok.Click += new System.EventHandler(this.btnPMok_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 408);
            this.Controls.Add(this.btnPMok);
            this.Controls.Add(this.btnPMcancel);
            this.Controls.Add(this.gpbManage);
            this.Controls.Add(this.label2);
            this.Location = new System.Drawing.Point(180, 180);
            this.Name = "PollutantManage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pollutant Manage";
            ((System.ComponentModel.ISupportInitialize)(this.nudownEndDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownStartDate)).EndInit();
            this.gpbManage.ResumeLayout(false);
            this.gpbManage.PerformLayout();
            this.tabSDetails.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstSMetric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DomainUpDown dmudownStartMonth;
        private System.Windows.Forms.DomainUpDown dmudownEndMonth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lstMetric;
        private System.Windows.Forms.Button btnSDelete;
        private System.Windows.Forms.Button btnSAdd;
        private System.Windows.Forms.Button btnSSDelete;
        private System.Windows.Forms.Button btnSSAdd;
        private System.Windows.Forms.GroupBox gpbManage;
        private System.Windows.Forms.Button btnPMcancel;
        private System.Windows.Forms.Button btnPMok;
        private System.Windows.Forms.NumericUpDown nudownEndDate;
        private System.Windows.Forms.NumericUpDown nudownStartDate;
        private System.Windows.Forms.TextBox txtSeasonalMetric;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabControl tabSDetails;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtFunctionManage;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lstSAvailableVaribles;
        private System.Windows.Forms.ListBox lstSAvailableFunctions;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;

    }
}