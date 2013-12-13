namespace BenMAP
{
    partial class APVConfigurationAdvancedSettings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tab = new System.Windows.Forms.TabControl();
            this.tbpAggreationAndPooling = new System.Windows.Forms.TabPage();
            this.chbSortIncidenceResults = new System.Windows.Forms.CheckBox();
            this.txtRandomSeed = new System.Windows.Forms.TextBox();
            this.cboDefaultMonteCarloIterations = new System.Windows.Forms.ComboBox();
            this.cboDefaultAdvancedPoolingMethod = new System.Windows.Forms.ComboBox();
            this.cboQALYAggregation = new System.Windows.Forms.ComboBox();
            this.cboValuationAggregation = new System.Windows.Forms.ComboBox();
            this.cboIncidenceAggregation = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbpCurrencyAndIncome = new System.Windows.Forms.TabPage();
            this.grpIncomeGrowthAdjustment = new System.Windows.Forms.GroupBox();
            this.lstEndpointGroups = new System.Windows.Forms.ListBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cboIncomeGrowthYear = new System.Windows.Forms.ComboBox();
            this.cboIncomeGrowthDataset = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.grpInflationAdjustment = new System.Windows.Forms.GroupBox();
            this.cboCurrencyYear = new System.Windows.Forms.ComboBox();
            this.cboInflationDataset = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tab.SuspendLayout();
            this.tbpAggreationAndPooling.SuspendLayout();
            this.tbpCurrencyAndIncome.SuspendLayout();
            this.grpIncomeGrowthAdjustment.SuspendLayout();
            this.grpInflationAdjustment.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(13, 498);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(433, 62);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(332, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(227, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tab);
            this.groupBox1.Location = new System.Drawing.Point(13, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(433, 476);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // tab
            // 
            this.tab.Controls.Add(this.tbpAggreationAndPooling);
            this.tab.Controls.Add(this.tbpCurrencyAndIncome);
            this.tab.Location = new System.Drawing.Point(15, 12);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(412, 457);
            this.tab.TabIndex = 0;
            // 
            // tbpAggreationAndPooling
            // 
            this.tbpAggreationAndPooling.Controls.Add(this.chbSortIncidenceResults);
            this.tbpAggreationAndPooling.Controls.Add(this.txtRandomSeed);
            this.tbpAggreationAndPooling.Controls.Add(this.cboDefaultMonteCarloIterations);
            this.tbpAggreationAndPooling.Controls.Add(this.cboDefaultAdvancedPoolingMethod);
            this.tbpAggreationAndPooling.Controls.Add(this.cboQALYAggregation);
            this.tbpAggreationAndPooling.Controls.Add(this.cboValuationAggregation);
            this.tbpAggreationAndPooling.Controls.Add(this.cboIncidenceAggregation);
            this.tbpAggreationAndPooling.Controls.Add(this.label6);
            this.tbpAggreationAndPooling.Controls.Add(this.label5);
            this.tbpAggreationAndPooling.Controls.Add(this.label4);
            this.tbpAggreationAndPooling.Controls.Add(this.label3);
            this.tbpAggreationAndPooling.Controls.Add(this.label2);
            this.tbpAggreationAndPooling.Controls.Add(this.label1);
            this.tbpAggreationAndPooling.Location = new System.Drawing.Point(4, 23);
            this.tbpAggreationAndPooling.Name = "tbpAggreationAndPooling";
            this.tbpAggreationAndPooling.Padding = new System.Windows.Forms.Padding(3);
            this.tbpAggreationAndPooling.Size = new System.Drawing.Size(404, 430);
            this.tbpAggreationAndPooling.TabIndex = 0;
            this.tbpAggreationAndPooling.Text = "Aggregation and Pooling";
            this.tbpAggreationAndPooling.UseVisualStyleBackColor = true;
            // 
            // chbSortIncidenceResults
            // 
            this.chbSortIncidenceResults.AutoSize = true;
            this.chbSortIncidenceResults.Location = new System.Drawing.Point(16, 161);
            this.chbSortIncidenceResults.Name = "chbSortIncidenceResults";
            this.chbSortIncidenceResults.Size = new System.Drawing.Size(147, 18);
            this.chbSortIncidenceResults.TabIndex = 13;
            this.chbSortIncidenceResults.Text = "Sort Incidence Results";
            this.chbSortIncidenceResults.UseVisualStyleBackColor = true;
            // 
            // txtRandomSeed
            // 
            this.txtRandomSeed.Location = new System.Drawing.Point(215, 119);
            this.txtRandomSeed.Name = "txtRandomSeed";
            this.txtRandomSeed.Size = new System.Drawing.Size(173, 22);
            this.txtRandomSeed.TabIndex = 12;
            this.txtRandomSeed.Text = "1";
            // 
            // cboDefaultMonteCarloIterations
            // 
            this.cboDefaultMonteCarloIterations.FormattingEnabled = true;
            this.cboDefaultMonteCarloIterations.Location = new System.Drawing.Point(215, 78);
            this.cboDefaultMonteCarloIterations.Name = "cboDefaultMonteCarloIterations";
            this.cboDefaultMonteCarloIterations.Size = new System.Drawing.Size(173, 22);
            this.cboDefaultMonteCarloIterations.TabIndex = 11;
            this.cboDefaultMonteCarloIterations.Text = "5000";
            // 
            // cboDefaultAdvancedPoolingMethod
            // 
            this.cboDefaultAdvancedPoolingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDefaultAdvancedPoolingMethod.FormattingEnabled = true;
            this.cboDefaultAdvancedPoolingMethod.Location = new System.Drawing.Point(215, 37);
            this.cboDefaultAdvancedPoolingMethod.Name = "cboDefaultAdvancedPoolingMethod";
            this.cboDefaultAdvancedPoolingMethod.Size = new System.Drawing.Size(173, 22);
            this.cboDefaultAdvancedPoolingMethod.TabIndex = 10;
            // 
            // cboQALYAggregation
            // 
            this.cboQALYAggregation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboQALYAggregation.FormattingEnabled = true;
            this.cboQALYAggregation.Location = new System.Drawing.Point(216, 296);
            this.cboQALYAggregation.Name = "cboQALYAggregation";
            this.cboQALYAggregation.Size = new System.Drawing.Size(172, 22);
            this.cboQALYAggregation.TabIndex = 9;
            this.cboQALYAggregation.Visible = false;
            // 
            // cboValuationAggregation
            // 
            this.cboValuationAggregation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboValuationAggregation.FormattingEnabled = true;
            this.cboValuationAggregation.Location = new System.Drawing.Point(215, 251);
            this.cboValuationAggregation.Name = "cboValuationAggregation";
            this.cboValuationAggregation.Size = new System.Drawing.Size(172, 22);
            this.cboValuationAggregation.TabIndex = 8;
            this.cboValuationAggregation.Visible = false;
            // 
            // cboIncidenceAggregation
            // 
            this.cboIncidenceAggregation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIncidenceAggregation.FormattingEnabled = true;
            this.cboIncidenceAggregation.Location = new System.Drawing.Point(215, 206);
            this.cboIncidenceAggregation.Name = "cboIncidenceAggregation";
            this.cboIncidenceAggregation.Size = new System.Drawing.Size(172, 22);
            this.cboIncidenceAggregation.TabIndex = 7;
            this.cboIncidenceAggregation.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 14);
            this.label6.TabIndex = 5;
            this.label6.Text = "Random Seed:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 14);
            this.label5.TabIndex = 4;
            this.label5.Text = "Default Monte Carlo Iterations:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(195, 14);
            this.label4.TabIndex = 3;
            this.label4.Text = "Default Advanced Pooling Method:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "QALY Aggregation:";
            this.label3.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 254);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Valuation Aggregation:";
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Incidence Aggregation:";
            this.label1.Visible = false;
            // 
            // tbpCurrencyAndIncome
            // 
            this.tbpCurrencyAndIncome.Controls.Add(this.grpIncomeGrowthAdjustment);
            this.tbpCurrencyAndIncome.Controls.Add(this.grpInflationAdjustment);
            this.tbpCurrencyAndIncome.Location = new System.Drawing.Point(4, 23);
            this.tbpCurrencyAndIncome.Name = "tbpCurrencyAndIncome";
            this.tbpCurrencyAndIncome.Padding = new System.Windows.Forms.Padding(3);
            this.tbpCurrencyAndIncome.Size = new System.Drawing.Size(404, 430);
            this.tbpCurrencyAndIncome.TabIndex = 1;
            this.tbpCurrencyAndIncome.Text = "Currency Year and Income Growth";
            this.tbpCurrencyAndIncome.UseVisualStyleBackColor = true;
            // 
            // grpIncomeGrowthAdjustment
            // 
            this.grpIncomeGrowthAdjustment.Controls.Add(this.lstEndpointGroups);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.label14);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.label13);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.label12);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.cboIncomeGrowthYear);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.cboIncomeGrowthDataset);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.label11);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.label10);
            this.grpIncomeGrowthAdjustment.Controls.Add(this.label9);
            this.grpIncomeGrowthAdjustment.Location = new System.Drawing.Point(20, 136);
            this.grpIncomeGrowthAdjustment.Name = "grpIncomeGrowthAdjustment";
            this.grpIncomeGrowthAdjustment.Size = new System.Drawing.Size(368, 283);
            this.grpIncomeGrowthAdjustment.TabIndex = 1;
            this.grpIncomeGrowthAdjustment.TabStop = false;
            this.grpIncomeGrowthAdjustment.Text = "Income Growth Adjustment";
            // 
            // lstEndpointGroups
            // 
            this.lstEndpointGroups.FormattingEnabled = true;
            this.lstEndpointGroups.HorizontalScrollbar = true;
            this.lstEndpointGroups.ItemHeight = 14;
            this.lstEndpointGroups.Location = new System.Drawing.Point(157, 101);
            this.lstEndpointGroups.Name = "lstEndpointGroups";
            this.lstEndpointGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstEndpointGroups.Size = new System.Drawing.Size(193, 116);
            this.lstEndpointGroups.TabIndex = 9;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(13, 232);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(337, 48);
            this.label14.TabIndex = 8;
            this.label14.Text = "Select the income growth dataset, then the year and the endpoint groups to which " +
                "you want to apply the adjustment. You may select more than one endpoint group.";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(19, 169);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(0, 14);
            this.label13.TabIndex = 7;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 101);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(101, 14);
            this.label12.TabIndex = 5;
            this.label12.Text = "Endpoint Groups:";
            // 
            // cboIncomeGrowthYear
            // 
            this.cboIncomeGrowthYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIncomeGrowthYear.FormattingEnabled = true;
            this.cboIncomeGrowthYear.Location = new System.Drawing.Point(157, 65);
            this.cboIncomeGrowthYear.Name = "cboIncomeGrowthYear";
            this.cboIncomeGrowthYear.Size = new System.Drawing.Size(193, 22);
            this.cboIncomeGrowthYear.TabIndex = 4;
            this.cboIncomeGrowthYear.SelectedValueChanged += new System.EventHandler(this.cboIncomeGrowthYear_SelectedValueChanged);
            // 
            // cboIncomeGrowthDataset
            // 
            this.cboIncomeGrowthDataset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIncomeGrowthDataset.FormattingEnabled = true;
            this.cboIncomeGrowthDataset.Location = new System.Drawing.Point(157, 33);
            this.cboIncomeGrowthDataset.Name = "cboIncomeGrowthDataset";
            this.cboIncomeGrowthDataset.Size = new System.Drawing.Size(193, 22);
            this.cboIncomeGrowthDataset.TabIndex = 3;
            this.cboIncomeGrowthDataset.SelectedValueChanged += new System.EventHandler(this.cboIncomeGrowthDataset_SelectedValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 36);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(138, 14);
            this.label11.TabIndex = 2;
            this.label11.Text = "Income Growth Dataset:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(118, 14);
            this.label10.TabIndex = 1;
            this.label10.Text = "Income Growth Year:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 14);
            this.label9.TabIndex = 0;
            // 
            // grpInflationAdjustment
            // 
            this.grpInflationAdjustment.Controls.Add(this.cboCurrencyYear);
            this.grpInflationAdjustment.Controls.Add(this.cboInflationDataset);
            this.grpInflationAdjustment.Controls.Add(this.label8);
            this.grpInflationAdjustment.Controls.Add(this.label7);
            this.grpInflationAdjustment.Location = new System.Drawing.Point(20, 19);
            this.grpInflationAdjustment.Name = "grpInflationAdjustment";
            this.grpInflationAdjustment.Size = new System.Drawing.Size(368, 111);
            this.grpInflationAdjustment.TabIndex = 0;
            this.grpInflationAdjustment.TabStop = false;
            this.grpInflationAdjustment.Text = "Inflation Adjustment";
            // 
            // cboCurrencyYear
            // 
            this.cboCurrencyYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCurrencyYear.FormattingEnabled = true;
            this.cboCurrencyYear.Location = new System.Drawing.Point(157, 69);
            this.cboCurrencyYear.Name = "cboCurrencyYear";
            this.cboCurrencyYear.Size = new System.Drawing.Size(193, 22);
            this.cboCurrencyYear.TabIndex = 3;
            // 
            // cboInflationDataset
            // 
            this.cboInflationDataset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInflationDataset.FormattingEnabled = true;
            this.cboInflationDataset.Location = new System.Drawing.Point(157, 31);
            this.cboInflationDataset.Name = "cboInflationDataset";
            this.cboInflationDataset.Size = new System.Drawing.Size(193, 22);
            this.cboInflationDataset.TabIndex = 2;
            this.cboInflationDataset.SelectedValueChanged += new System.EventHandler(this.cboInflationDataset_SelectedValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 14);
            this.label8.TabIndex = 1;
            this.label8.Text = "Currency Year:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 14);
            this.label7.TabIndex = 0;
            this.label7.Text = "Inflation Dataset:";
            // 
            // APVConfigurationAdvancedSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 574);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "APVConfigurationAdvancedSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Pooling Settings";
            this.Load += new System.EventHandler(this.APVConfigurationAdvancedSettings_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tab.ResumeLayout(false);
            this.tbpAggreationAndPooling.ResumeLayout(false);
            this.tbpAggreationAndPooling.PerformLayout();
            this.tbpCurrencyAndIncome.ResumeLayout(false);
            this.grpIncomeGrowthAdjustment.ResumeLayout(false);
            this.grpIncomeGrowthAdjustment.PerformLayout();
            this.grpInflationAdjustment.ResumeLayout(false);
            this.grpInflationAdjustment.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage tbpAggreationAndPooling;
        private System.Windows.Forms.CheckBox chbSortIncidenceResults;
        private System.Windows.Forms.TextBox txtRandomSeed;
        private System.Windows.Forms.ComboBox cboDefaultMonteCarloIterations;
        private System.Windows.Forms.ComboBox cboDefaultAdvancedPoolingMethod;
        private System.Windows.Forms.ComboBox cboQALYAggregation;
        private System.Windows.Forms.ComboBox cboValuationAggregation;
        private System.Windows.Forms.ComboBox cboIncidenceAggregation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tbpCurrencyAndIncome;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox grpIncomeGrowthAdjustment;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox grpInflationAdjustment;
        private System.Windows.Forms.ComboBox cboCurrencyYear;
        private System.Windows.Forms.ComboBox cboInflationDataset;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cboIncomeGrowthYear;
        private System.Windows.Forms.ComboBox cboIncomeGrowthDataset;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lstEndpointGroups;
    }
}