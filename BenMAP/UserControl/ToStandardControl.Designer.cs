namespace BenMAP.DataSource
{
    partial class ToStandardControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtIntradayBackground = new System.Windows.Forms.TextBox();
            this.cboIntraday = new System.Windows.Forms.ComboBox();
            this.lblIntraBackground = new System.Windows.Forms.Label();
            this.lblIntradayRollbackMethod = new System.Windows.Forms.Label();
            this.txtInterdayBackground = new System.Windows.Forms.TextBox();
            this.cboInterday = new System.Windows.Forms.ComboBox();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblInterdayRollbackMethod = new System.Windows.Forms.Label();
            this.lblOrdinalityDestribution = new System.Windows.Forms.Label();
            this.txtStandard = new System.Windows.Forms.TextBox();
            this.nudnOrdinality = new System.Windows.Forms.NumericUpDown();
            this.cboAnnualStatistic = new System.Windows.Forms.ComboBox();
            this.cboSeasonalMetric = new System.Windows.Forms.ComboBox();
            this.cboDailyMetric = new System.Windows.Forms.ComboBox();
            this.lblStandard = new System.Windows.Forms.Label();
            this.lblOrdinality = new System.Windows.Forms.Label();
            this.lalAnnualStatistic = new System.Windows.Forms.Label();
            this.lblSeasonalMetric = new System.Windows.Forms.Label();
            this.lblDailyMetric = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudnOrdinality)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.lblOrdinalityDestribution);
            this.groupBox1.Controls.Add(this.txtStandard);
            this.groupBox1.Controls.Add(this.nudnOrdinality);
            this.groupBox1.Controls.Add(this.cboAnnualStatistic);
            this.groupBox1.Controls.Add(this.cboSeasonalMetric);
            this.groupBox1.Controls.Add(this.cboDailyMetric);
            this.groupBox1.Controls.Add(this.lblStandard);
            this.groupBox1.Controls.Add(this.lblOrdinality);
            this.groupBox1.Controls.Add(this.lalAnnualStatistic);
            this.groupBox1.Controls.Add(this.lblSeasonalMetric);
            this.groupBox1.Controls.Add(this.lblDailyMetric);
            this.groupBox1.Location = new System.Drawing.Point(3, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(175, 434);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Attainment Test";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtIntradayBackground);
            this.groupBox2.Controls.Add(this.cboIntraday);
            this.groupBox2.Controls.Add(this.lblIntraBackground);
            this.groupBox2.Controls.Add(this.lblIntradayRollbackMethod);
            this.groupBox2.Controls.Add(this.txtInterdayBackground);
            this.groupBox2.Controls.Add(this.cboInterday);
            this.groupBox2.Controls.Add(this.lblBackground);
            this.groupBox2.Controls.Add(this.lblInterdayRollbackMethod);
            this.groupBox2.Location = new System.Drawing.Point(6, 243);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(163, 186);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rollback Methods";
            // 
            // txtIntradayBackground
            // 
            this.txtIntradayBackground.Location = new System.Drawing.Point(8, 156);
            this.txtIntradayBackground.Name = "txtIntradayBackground";
            this.txtIntradayBackground.Size = new System.Drawing.Size(151, 21);
            this.txtIntradayBackground.TabIndex = 7;
            this.txtIntradayBackground.Text = "0.00";
            this.txtIntradayBackground.TextChanged += new System.EventHandler(this.txtIntradayBackground_TextChanged);
            this.txtIntradayBackground.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIntradayBackground_KeyPress);
            // 
            // cboIntraday
            // 
            this.cboIntraday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIntraday.FormattingEnabled = true;
            this.cboIntraday.Location = new System.Drawing.Point(8, 118);
            this.cboIntraday.Name = "cboIntraday";
            this.cboIntraday.Size = new System.Drawing.Size(151, 20);
            this.cboIntraday.TabIndex = 6;
            this.cboIntraday.SelectedIndexChanged += new System.EventHandler(this.cboIntraday_SelectedIndexChanged);
            // 
            // lblIntraBackground
            // 
            this.lblIntraBackground.AutoSize = true;
            this.lblIntraBackground.Location = new System.Drawing.Point(6, 141);
            this.lblIntraBackground.Name = "lblIntraBackground";
            this.lblIntraBackground.Size = new System.Drawing.Size(71, 12);
            this.lblIntraBackground.TabIndex = 5;
            this.lblIntraBackground.Text = "Background:";
            // 
            // lblIntradayRollbackMethod
            // 
            this.lblIntradayRollbackMethod.AutoSize = true;
            this.lblIntradayRollbackMethod.Location = new System.Drawing.Point(6, 103);
            this.lblIntradayRollbackMethod.Name = "lblIntradayRollbackMethod";
            this.lblIntradayRollbackMethod.Size = new System.Drawing.Size(155, 12);
            this.lblIntradayRollbackMethod.TabIndex = 4;
            this.lblIntradayRollbackMethod.Text = "Intraday Rollback Method:";
            // 
            // txtInterdayBackground
            // 
            this.txtInterdayBackground.Location = new System.Drawing.Point(8, 70);
            this.txtInterdayBackground.Name = "txtInterdayBackground";
            this.txtInterdayBackground.Size = new System.Drawing.Size(151, 21);
            this.txtInterdayBackground.TabIndex = 3;
            this.txtInterdayBackground.Text = "0.00";
            this.txtInterdayBackground.TextChanged += new System.EventHandler(this.txtBackground_TextChanged);
            this.txtInterdayBackground.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBackground_KeyPress);
            // 
            // cboInterday
            // 
            this.cboInterday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInterday.FormattingEnabled = true;
            this.cboInterday.Location = new System.Drawing.Point(8, 32);
            this.cboInterday.Name = "cboInterday";
            this.cboInterday.Size = new System.Drawing.Size(151, 20);
            this.cboInterday.TabIndex = 2;
            this.cboInterday.SelectedIndexChanged += new System.EventHandler(this.cboInterday_SelectedIndexChanged);
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(6, 55);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(71, 12);
            this.lblBackground.TabIndex = 1;
            this.lblBackground.Text = "Background:";
            // 
            // lblInterdayRollbackMethod
            // 
            this.lblInterdayRollbackMethod.AutoSize = true;
            this.lblInterdayRollbackMethod.Location = new System.Drawing.Point(6, 17);
            this.lblInterdayRollbackMethod.Name = "lblInterdayRollbackMethod";
            this.lblInterdayRollbackMethod.Size = new System.Drawing.Size(155, 12);
            this.lblInterdayRollbackMethod.TabIndex = 0;
            this.lblInterdayRollbackMethod.Text = "Interday Rollback Method:";
            // 
            // lblOrdinalityDestribution
            // 
            this.lblOrdinalityDestribution.Font = new System.Drawing.Font("Calibri", 8.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrdinalityDestribution.Location = new System.Drawing.Point(6, 209);
            this.lblOrdinalityDestribution.Name = "lblOrdinalityDestribution";
            this.lblOrdinalityDestribution.Size = new System.Drawing.Size(163, 31);
            this.lblOrdinalityDestribution.TabIndex = 10;
            this.lblOrdinalityDestribution.Text = "label1";
            // 
            // txtStandard
            // 
            this.txtStandard.Location = new System.Drawing.Point(8, 185);
            this.txtStandard.Name = "txtStandard";
            this.txtStandard.Size = new System.Drawing.Size(157, 21);
            this.txtStandard.TabIndex = 9;
            this.txtStandard.Text = "0.00";
            this.txtStandard.TextChanged += new System.EventHandler(this.txtStandard_TextChanged);
            this.txtStandard.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStandard_KeyPress);
            // 
            // nudnOrdinality
            // 
            this.nudnOrdinality.Location = new System.Drawing.Point(8, 146);
            this.nudnOrdinality.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudnOrdinality.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudnOrdinality.Name = "nudnOrdinality";
            this.nudnOrdinality.Size = new System.Drawing.Size(157, 21);
            this.nudnOrdinality.TabIndex = 8;
            this.nudnOrdinality.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudnOrdinality.ValueChanged += new System.EventHandler(this.nudnOrdinality_ValueChanged);
            this.nudnOrdinality.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nudnOrdinality_KeyPress);
            this.nudnOrdinality.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nudnOrdinality_KeyUp);
            // 
            // cboAnnualStatistic
            // 
            this.cboAnnualStatistic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnnualStatistic.FormattingEnabled = true;
            this.cboAnnualStatistic.Location = new System.Drawing.Point(8, 108);
            this.cboAnnualStatistic.Name = "cboAnnualStatistic";
            this.cboAnnualStatistic.Size = new System.Drawing.Size(157, 20);
            this.cboAnnualStatistic.TabIndex = 7;
            this.cboAnnualStatistic.SelectedIndexChanged += new System.EventHandler(this.cboAnnualStatistic_SelectedIndexChanged);
            // 
            // cboSeasonalMetric
            // 
            this.cboSeasonalMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSeasonalMetric.FormattingEnabled = true;
            this.cboSeasonalMetric.Location = new System.Drawing.Point(8, 70);
            this.cboSeasonalMetric.Name = "cboSeasonalMetric";
            this.cboSeasonalMetric.Size = new System.Drawing.Size(157, 20);
            this.cboSeasonalMetric.TabIndex = 6;
            this.cboSeasonalMetric.SelectedIndexChanged += new System.EventHandler(this.cboSeasonalMetric_SelectedIndexChanged);
            // 
            // cboDailyMetric
            // 
            this.cboDailyMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDailyMetric.FormattingEnabled = true;
            this.cboDailyMetric.Location = new System.Drawing.Point(8, 32);
            this.cboDailyMetric.Name = "cboDailyMetric";
            this.cboDailyMetric.Size = new System.Drawing.Size(157, 20);
            this.cboDailyMetric.TabIndex = 5;
            this.cboDailyMetric.SelectedIndexChanged += new System.EventHandler(this.cboDailyMetric_SelectedIndexChanged);
            // 
            // lblStandard
            // 
            this.lblStandard.AutoSize = true;
            this.lblStandard.Location = new System.Drawing.Point(6, 170);
            this.lblStandard.Name = "lblStandard";
            this.lblStandard.Size = new System.Drawing.Size(59, 12);
            this.lblStandard.TabIndex = 4;
            this.lblStandard.Text = "Standard:";
            // 
            // lblOrdinality
            // 
            this.lblOrdinality.AutoSize = true;
            this.lblOrdinality.Location = new System.Drawing.Point(6, 131);
            this.lblOrdinality.Name = "lblOrdinality";
            this.lblOrdinality.Size = new System.Drawing.Size(65, 12);
            this.lblOrdinality.TabIndex = 3;
            this.lblOrdinality.Text = "Ordinality";
            // 
            // lalAnnualStatistic
            // 
            this.lalAnnualStatistic.AutoSize = true;
            this.lalAnnualStatistic.Location = new System.Drawing.Point(6, 93);
            this.lalAnnualStatistic.Name = "lalAnnualStatistic";
            this.lalAnnualStatistic.Size = new System.Drawing.Size(107, 12);
            this.lalAnnualStatistic.TabIndex = 2;
            this.lalAnnualStatistic.Text = "Annual Statistic:";
            // 
            // lblSeasonalMetric
            // 
            this.lblSeasonalMetric.AutoSize = true;
            this.lblSeasonalMetric.Location = new System.Drawing.Point(6, 55);
            this.lblSeasonalMetric.Name = "lblSeasonalMetric";
            this.lblSeasonalMetric.Size = new System.Drawing.Size(101, 12);
            this.lblSeasonalMetric.TabIndex = 1;
            this.lblSeasonalMetric.Text = "Seasonal Metric:";
            // 
            // lblDailyMetric
            // 
            this.lblDailyMetric.AutoSize = true;
            this.lblDailyMetric.Location = new System.Drawing.Point(6, 17);
            this.lblDailyMetric.Name = "lblDailyMetric";
            this.lblDailyMetric.Size = new System.Drawing.Size(83, 12);
            this.lblDailyMetric.TabIndex = 0;
            this.lblDailyMetric.Text = "Daily Metric:";
            // 
            // ToStandardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ToStandardControl";
            this.Size = new System.Drawing.Size(181, 459);
            this.Load += new System.EventHandler(this.ToStandardControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudnOrdinality)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblOrdinalityDestribution;
        private System.Windows.Forms.TextBox txtStandard;
        private System.Windows.Forms.NumericUpDown nudnOrdinality;
        private System.Windows.Forms.ComboBox cboAnnualStatistic;
        private System.Windows.Forms.ComboBox cboSeasonalMetric;
        private System.Windows.Forms.ComboBox cboDailyMetric;
        private System.Windows.Forms.Label lblStandard;
        private System.Windows.Forms.Label lblOrdinality;
        private System.Windows.Forms.Label lalAnnualStatistic;
        private System.Windows.Forms.Label lblSeasonalMetric;
        private System.Windows.Forms.Label lblDailyMetric;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtInterdayBackground;
        private System.Windows.Forms.ComboBox cboInterday;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.Label lblInterdayRollbackMethod;
        private System.Windows.Forms.TextBox txtIntradayBackground;
        private System.Windows.Forms.ComboBox cboIntraday;
        private System.Windows.Forms.Label lblIntraBackground;
        private System.Windows.Forms.Label lblIntradayRollbackMethod;
    }
}
