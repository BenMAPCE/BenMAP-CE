namespace BenMAP
{
    partial class PollutantDefinition
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.grpSeasonalMetrics = new System.Windows.Forms.GroupBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lstSeasonalMetrics = new System.Windows.Forms.ListBox();
            this.grpHourlyMetricGeneration = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMetricName = new System.Windows.Forms.TextBox();
            this.lblhourlymetricgeneration = new System.Windows.Forms.Label();
            this.cbohourlymetricgeneration = new System.Windows.Forms.ComboBox();
            this.tabHourlyMetricGeneration = new System.Windows.Forms.TabControl();
            this.tpFix = new System.Windows.Forms.TabPage();
            this.cboFixStatistic = new System.Windows.Forms.ComboBox();
            this.lblStatistic = new System.Windows.Forms.Label();
            this.nudownEndHour = new System.Windows.Forms.NumericUpDown();
            this.nudownStartHour = new System.Windows.Forms.NumericUpDown();
            this.lblEndHour = new System.Windows.Forms.Label();
            this.lblStartHour = new System.Windows.Forms.Label();
            this.tpMovingWindow = new System.Windows.Forms.TabPage();
            this.cboMovingStatistic = new System.Windows.Forms.ComboBox();
            this.cboWindowStatistic = new System.Windows.Forms.ComboBox();
            this.nudownWindowSize = new System.Windows.Forms.NumericUpDown();
            this.lblDailyStatistic = new System.Windows.Forms.Label();
            this.lblWindowStatistic = new System.Windows.Forms.Label();
            this.lblWindowSize = new System.Windows.Forms.Label();
            this.tpCustom = new System.Windows.Forms.TabPage();
            this.txtFunctionManage = new System.Windows.Forms.TextBox();
            this.lblFunction = new System.Windows.Forms.Label();
            this.lstVariables = new System.Windows.Forms.ListBox();
            this.lstFunctions = new System.Windows.Forms.ListBox();
            this.lblVariables = new System.Windows.Forms.Label();
            this.lblFunctions = new System.Windows.Forms.Label();
            this.grpPollutant = new System.Windows.Forms.GroupBox();
            this.grpMetrics = new System.Windows.Forms.GroupBox();
            this.btnAdvancedOptions = new System.Windows.Forms.Button();
            this.btnMetricsAdd = new System.Windows.Forms.Button();
            this.btnMetricsDelete = new System.Windows.Forms.Button();
            this.lstMetrics = new System.Windows.Forms.ListBox();
            this.cboObservationType = new System.Windows.Forms.ComboBox();
            this.lblObservationType = new System.Windows.Forms.Label();
            this.txtPollutantID = new System.Windows.Forms.TextBox();
            this.lblPollutantID = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grpDetail.SuspendLayout();
            this.grpSeasonalMetrics.SuspendLayout();
            this.grpHourlyMetricGeneration.SuspendLayout();
            this.tabHourlyMetricGeneration.SuspendLayout();
            this.tpFix.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownEndHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownStartHour)).BeginInit();
            this.tpMovingWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownWindowSize)).BeginInit();
            this.tpCustom.SuspendLayout();
            this.grpPollutant.SuspendLayout();
            this.grpMetrics.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Location = new System.Drawing.Point(4, 571);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(683, 44);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(497, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(589, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetail.Controls.Add(this.grpSeasonalMetrics);
            this.grpDetail.Controls.Add(this.grpHourlyMetricGeneration);
            this.grpDetail.Location = new System.Drawing.Point(208, 1);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(479, 571);
            this.grpDetail.TabIndex = 1;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Detail";
            // 
            // grpSeasonalMetrics
            // 
            this.grpSeasonalMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSeasonalMetrics.Controls.Add(this.btnEdit);
            this.grpSeasonalMetrics.Controls.Add(this.lstSeasonalMetrics);
            this.grpSeasonalMetrics.Location = new System.Drawing.Point(6, 341);
            this.grpSeasonalMetrics.Name = "grpSeasonalMetrics";
            this.grpSeasonalMetrics.Size = new System.Drawing.Size(457, 219);
            this.grpSeasonalMetrics.TabIndex = 3;
            this.grpSeasonalMetrics.TabStop = false;
            this.grpSeasonalMetrics.Text = "Seasonal Metrics (Seasons for Individual Pollutant Metrics)";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(379, 185);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 27);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lstSeasonalMetrics
            // 
            this.lstSeasonalMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSeasonalMetrics.FormattingEnabled = true;
            this.lstSeasonalMetrics.ItemHeight = 14;
            this.lstSeasonalMetrics.Location = new System.Drawing.Point(3, 18);
            this.lstSeasonalMetrics.Name = "lstSeasonalMetrics";
            this.lstSeasonalMetrics.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstSeasonalMetrics.Size = new System.Drawing.Size(451, 158);
            this.lstSeasonalMetrics.TabIndex = 1;
            // 
            // grpHourlyMetricGeneration
            // 
            this.grpHourlyMetricGeneration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpHourlyMetricGeneration.Controls.Add(this.label1);
            this.grpHourlyMetricGeneration.Controls.Add(this.txtMetricName);
            this.grpHourlyMetricGeneration.Controls.Add(this.lblhourlymetricgeneration);
            this.grpHourlyMetricGeneration.Controls.Add(this.cbohourlymetricgeneration);
            this.grpHourlyMetricGeneration.Controls.Add(this.tabHourlyMetricGeneration);
            this.grpHourlyMetricGeneration.Location = new System.Drawing.Point(6, 11);
            this.grpHourlyMetricGeneration.Name = "grpHourlyMetricGeneration";
            this.grpHourlyMetricGeneration.Size = new System.Drawing.Size(457, 324);
            this.grpHourlyMetricGeneration.TabIndex = 2;
            this.grpHourlyMetricGeneration.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Metric Name:";
            // 
            // txtMetricName
            // 
            this.txtMetricName.Location = new System.Drawing.Point(206, 19);
            this.txtMetricName.Name = "txtMetricName";
            this.txtMetricName.Size = new System.Drawing.Size(212, 22);
            this.txtMetricName.TabIndex = 4;
            this.txtMetricName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMetricName_KeyDown);
            this.txtMetricName.Leave += new System.EventHandler(this.txtMetricName_Leave);
            // 
            // lblhourlymetricgeneration
            // 
            this.lblhourlymetricgeneration.AutoSize = true;
            this.lblhourlymetricgeneration.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblhourlymetricgeneration.Location = new System.Drawing.Point(11, 54);
            this.lblhourlymetricgeneration.Name = "lblhourlymetricgeneration";
            this.lblhourlymetricgeneration.Size = new System.Drawing.Size(146, 14);
            this.lblhourlymetricgeneration.TabIndex = 3;
            this.lblhourlymetricgeneration.Text = "Hourly Metric Generation:";
            // 
            // cbohourlymetricgeneration
            // 
            this.cbohourlymetricgeneration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbohourlymetricgeneration.FormattingEnabled = true;
            this.cbohourlymetricgeneration.Items.AddRange(new object[] {
            "Fixed Window",
            "Moving Window",
            "Custom"});
            this.cbohourlymetricgeneration.Location = new System.Drawing.Point(206, 52);
            this.cbohourlymetricgeneration.Name = "cbohourlymetricgeneration";
            this.cbohourlymetricgeneration.Size = new System.Drawing.Size(212, 22);
            this.cbohourlymetricgeneration.TabIndex = 2;
            this.cbohourlymetricgeneration.SelectedValueChanged += new System.EventHandler(this.cbohourlymetricgeneration_SelectedValueChanged);
            // 
            // tabHourlyMetricGeneration
            // 
            this.tabHourlyMetricGeneration.Controls.Add(this.tpFix);
            this.tabHourlyMetricGeneration.Controls.Add(this.tpMovingWindow);
            this.tabHourlyMetricGeneration.Controls.Add(this.tpCustom);
            this.tabHourlyMetricGeneration.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabHourlyMetricGeneration.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tabHourlyMetricGeneration.Location = new System.Drawing.Point(3, 92);
            this.tabHourlyMetricGeneration.Name = "tabHourlyMetricGeneration";
            this.tabHourlyMetricGeneration.SelectedIndex = 0;
            this.tabHourlyMetricGeneration.Size = new System.Drawing.Size(451, 229);
            this.tabHourlyMetricGeneration.TabIndex = 1;
            // 
            // tpFix
            // 
            this.tpFix.Controls.Add(this.cboFixStatistic);
            this.tpFix.Controls.Add(this.lblStatistic);
            this.tpFix.Controls.Add(this.nudownEndHour);
            this.tpFix.Controls.Add(this.nudownStartHour);
            this.tpFix.Controls.Add(this.lblEndHour);
            this.tpFix.Controls.Add(this.lblStartHour);
            this.tpFix.Location = new System.Drawing.Point(4, 23);
            this.tpFix.Name = "tpFix";
            this.tpFix.Padding = new System.Windows.Forms.Padding(3);
            this.tpFix.Size = new System.Drawing.Size(443, 202);
            this.tpFix.TabIndex = 0;
            this.tpFix.Text = "Fixed Window";
            this.tpFix.UseVisualStyleBackColor = true;
            // 
            // cboFixStatistic
            // 
            this.cboFixStatistic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFixStatistic.FormattingEnabled = true;
            this.cboFixStatistic.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cboFixStatistic.Location = new System.Drawing.Point(119, 96);
            this.cboFixStatistic.Name = "cboFixStatistic";
            this.cboFixStatistic.Size = new System.Drawing.Size(74, 22);
            this.cboFixStatistic.TabIndex = 5;
            // 
            // lblStatistic
            // 
            this.lblStatistic.AutoSize = true;
            this.lblStatistic.Location = new System.Drawing.Point(14, 99);
            this.lblStatistic.Name = "lblStatistic";
            this.lblStatistic.Size = new System.Drawing.Size(52, 14);
            this.lblStatistic.TabIndex = 4;
            this.lblStatistic.Text = "Statistic:";
            // 
            // nudownEndHour
            // 
            this.nudownEndHour.Location = new System.Drawing.Point(119, 60);
            this.nudownEndHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nudownEndHour.Name = "nudownEndHour";
            this.nudownEndHour.Size = new System.Drawing.Size(74, 22);
            this.nudownEndHour.TabIndex = 3;
            this.nudownEndHour.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            // 
            // nudownStartHour
            // 
            this.nudownStartHour.Location = new System.Drawing.Point(119, 25);
            this.nudownStartHour.Maximum = new decimal(new int[] {
            22,
            0,
            0,
            0});
            this.nudownStartHour.Name = "nudownStartHour";
            this.nudownStartHour.Size = new System.Drawing.Size(74, 22);
            this.nudownStartHour.TabIndex = 2;
            // 
            // lblEndHour
            // 
            this.lblEndHour.AutoSize = true;
            this.lblEndHour.Location = new System.Drawing.Point(14, 63);
            this.lblEndHour.Name = "lblEndHour";
            this.lblEndHour.Size = new System.Drawing.Size(59, 14);
            this.lblEndHour.TabIndex = 1;
            this.lblEndHour.Text = "End Hour:";
            // 
            // lblStartHour
            // 
            this.lblStartHour.AutoSize = true;
            this.lblStartHour.Location = new System.Drawing.Point(14, 27);
            this.lblStartHour.Name = "lblStartHour";
            this.lblStartHour.Size = new System.Drawing.Size(64, 14);
            this.lblStartHour.TabIndex = 0;
            this.lblStartHour.Text = "Start Hour:";
            // 
            // tpMovingWindow
            // 
            this.tpMovingWindow.Controls.Add(this.cboMovingStatistic);
            this.tpMovingWindow.Controls.Add(this.cboWindowStatistic);
            this.tpMovingWindow.Controls.Add(this.nudownWindowSize);
            this.tpMovingWindow.Controls.Add(this.lblDailyStatistic);
            this.tpMovingWindow.Controls.Add(this.lblWindowStatistic);
            this.tpMovingWindow.Controls.Add(this.lblWindowSize);
            this.tpMovingWindow.Location = new System.Drawing.Point(4, 23);
            this.tpMovingWindow.Name = "tpMovingWindow";
            this.tpMovingWindow.Padding = new System.Windows.Forms.Padding(3);
            this.tpMovingWindow.Size = new System.Drawing.Size(443, 202);
            this.tpMovingWindow.TabIndex = 1;
            this.tpMovingWindow.Text = "Moving Window";
            this.tpMovingWindow.UseVisualStyleBackColor = true;
            // 
            // cboMovingStatistic
            // 
            this.cboMovingStatistic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMovingStatistic.FormattingEnabled = true;
            this.cboMovingStatistic.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cboMovingStatistic.Location = new System.Drawing.Point(119, 96);
            this.cboMovingStatistic.Name = "cboMovingStatistic";
            this.cboMovingStatistic.Size = new System.Drawing.Size(74, 22);
            this.cboMovingStatistic.TabIndex = 5;
            // 
            // cboWindowStatistic
            // 
            this.cboWindowStatistic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWindowStatistic.FormattingEnabled = true;
            this.cboWindowStatistic.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cboWindowStatistic.Location = new System.Drawing.Point(119, 60);
            this.cboWindowStatistic.Name = "cboWindowStatistic";
            this.cboWindowStatistic.Size = new System.Drawing.Size(74, 22);
            this.cboWindowStatistic.TabIndex = 4;
            // 
            // nudownWindowSize
            // 
            this.nudownWindowSize.Location = new System.Drawing.Point(119, 25);
            this.nudownWindowSize.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nudownWindowSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudownWindowSize.Name = "nudownWindowSize";
            this.nudownWindowSize.Size = new System.Drawing.Size(74, 22);
            this.nudownWindowSize.TabIndex = 3;
            this.nudownWindowSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblDailyStatistic
            // 
            this.lblDailyStatistic.AutoSize = true;
            this.lblDailyStatistic.Location = new System.Drawing.Point(14, 99);
            this.lblDailyStatistic.Name = "lblDailyStatistic";
            this.lblDailyStatistic.Size = new System.Drawing.Size(83, 14);
            this.lblDailyStatistic.TabIndex = 2;
            this.lblDailyStatistic.Text = "Daily Statistic:";
            // 
            // lblWindowStatistic
            // 
            this.lblWindowStatistic.AutoSize = true;
            this.lblWindowStatistic.Location = new System.Drawing.Point(13, 63);
            this.lblWindowStatistic.Name = "lblWindowStatistic";
            this.lblWindowStatistic.Size = new System.Drawing.Size(100, 14);
            this.lblWindowStatistic.TabIndex = 1;
            this.lblWindowStatistic.Text = "Window Statistic:";
            // 
            // lblWindowSize
            // 
            this.lblWindowSize.AutoSize = true;
            this.lblWindowSize.Location = new System.Drawing.Point(14, 27);
            this.lblWindowSize.Name = "lblWindowSize";
            this.lblWindowSize.Size = new System.Drawing.Size(80, 14);
            this.lblWindowSize.TabIndex = 0;
            this.lblWindowSize.Text = "Window Size:";
            // 
            // tpCustom
            // 
            this.tpCustom.Controls.Add(this.txtFunctionManage);
            this.tpCustom.Controls.Add(this.lblFunction);
            this.tpCustom.Controls.Add(this.lstVariables);
            this.tpCustom.Controls.Add(this.lstFunctions);
            this.tpCustom.Controls.Add(this.lblVariables);
            this.tpCustom.Controls.Add(this.lblFunctions);
            this.tpCustom.Location = new System.Drawing.Point(4, 23);
            this.tpCustom.Name = "tpCustom";
            this.tpCustom.Padding = new System.Windows.Forms.Padding(3);
            this.tpCustom.Size = new System.Drawing.Size(443, 202);
            this.tpCustom.TabIndex = 2;
            this.tpCustom.Text = "Custom";
            this.tpCustom.UseVisualStyleBackColor = true;
            // 
            // txtFunctionManage
            // 
            this.txtFunctionManage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFunctionManage.Location = new System.Drawing.Point(69, 173);
            this.txtFunctionManage.Multiline = true;
            this.txtFunctionManage.Name = "txtFunctionManage";
            this.txtFunctionManage.Size = new System.Drawing.Size(368, 24);
            this.txtFunctionManage.TabIndex = 5;
            // 
            // lblFunction
            // 
            this.lblFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFunction.AutoSize = true;
            this.lblFunction.Location = new System.Drawing.Point(7, 176);
            this.lblFunction.Name = "lblFunction";
            this.lblFunction.Size = new System.Drawing.Size(56, 14);
            this.lblFunction.TabIndex = 4;
            this.lblFunction.Text = "Function:";
            // 
            // lstVariables
            // 
            this.lstVariables.FormattingEnabled = true;
            this.lstVariables.ItemHeight = 14;
            this.lstVariables.Items.AddRange(new object[] {
            "Observations[]",
            "DailyObservation[]",
            "SoredObservation[]",
            "Day",
            "Mean",
            "Median",
            "Min",
            "Max",
            "Sum",
            "NoObservation"});
            this.lstVariables.Location = new System.Drawing.Point(210, 23);
            this.lstVariables.Name = "lstVariables";
            this.lstVariables.Size = new System.Drawing.Size(227, 144);
            this.lstVariables.TabIndex = 3;
            this.lstVariables.DoubleClick += new System.EventHandler(this.lstVariables_DoubleClick);
            // 
            // lstFunctions
            // 
            this.lstFunctions.FormattingEnabled = true;
            this.lstFunctions.ItemHeight = 14;
            this.lstFunctions.Items.AddRange(new object[] {
            "ABS(x)",
            "EXP(x)",
            "IPOWER(x,y)",
            "LN(x)",
            "POWER(x,y)",
            "SQR(x)",
            "SQRT(x)"});
            this.lstFunctions.Location = new System.Drawing.Point(6, 23);
            this.lstFunctions.Name = "lstFunctions";
            this.lstFunctions.Size = new System.Drawing.Size(198, 144);
            this.lstFunctions.TabIndex = 2;
            this.lstFunctions.DoubleClick += new System.EventHandler(this.lstFunctions_DoubleClick);
            // 
            // lblVariables
            // 
            this.lblVariables.AutoSize = true;
            this.lblVariables.Location = new System.Drawing.Point(208, 6);
            this.lblVariables.Name = "lblVariables";
            this.lblVariables.Size = new System.Drawing.Size(117, 14);
            this.lblVariables.TabIndex = 1;
            this.lblVariables.Text = "Available Variables:";
            // 
            // lblFunctions
            // 
            this.lblFunctions.AutoSize = true;
            this.lblFunctions.Location = new System.Drawing.Point(7, 6);
            this.lblFunctions.Name = "lblFunctions";
            this.lblFunctions.Size = new System.Drawing.Size(117, 14);
            this.lblFunctions.TabIndex = 0;
            this.lblFunctions.Text = "Available Functions:";
            // 
            // grpPollutant
            // 
            this.grpPollutant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpPollutant.Controls.Add(this.grpMetrics);
            this.grpPollutant.Controls.Add(this.cboObservationType);
            this.grpPollutant.Controls.Add(this.lblObservationType);
            this.grpPollutant.Controls.Add(this.txtPollutantID);
            this.grpPollutant.Controls.Add(this.lblPollutantID);
            this.grpPollutant.Location = new System.Drawing.Point(4, 1);
            this.grpPollutant.Name = "grpPollutant";
            this.grpPollutant.Size = new System.Drawing.Size(198, 571);
            this.grpPollutant.TabIndex = 0;
            this.grpPollutant.TabStop = false;
            this.grpPollutant.Text = "Main";
            // 
            // grpMetrics
            // 
            this.grpMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMetrics.Controls.Add(this.btnAdvancedOptions);
            this.grpMetrics.Controls.Add(this.btnMetricsAdd);
            this.grpMetrics.Controls.Add(this.btnMetricsDelete);
            this.grpMetrics.Controls.Add(this.lstMetrics);
            this.grpMetrics.Location = new System.Drawing.Point(6, 103);
            this.grpMetrics.Name = "grpMetrics";
            this.grpMetrics.Size = new System.Drawing.Size(186, 457);
            this.grpMetrics.TabIndex = 4;
            this.grpMetrics.TabStop = false;
            this.grpMetrics.Text = "Metrics";
            // 
            // btnAdvancedOptions
            // 
            this.btnAdvancedOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdvancedOptions.Location = new System.Drawing.Point(6, 415);
            this.btnAdvancedOptions.Name = "btnAdvancedOptions";
            this.btnAdvancedOptions.Size = new System.Drawing.Size(174, 42);
            this.btnAdvancedOptions.TabIndex = 3;
            this.btnAdvancedOptions.Text = "Define Seasons for all Pollutant Metrics";
            this.btnAdvancedOptions.UseVisualStyleBackColor = true;
            this.btnAdvancedOptions.Click += new System.EventHandler(this.btnAdvancedOptions_Click);
            // 
            // btnMetricsAdd
            // 
            this.btnMetricsAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMetricsAdd.Location = new System.Drawing.Point(106, 381);
            this.btnMetricsAdd.Name = "btnMetricsAdd";
            this.btnMetricsAdd.Size = new System.Drawing.Size(74, 27);
            this.btnMetricsAdd.TabIndex = 2;
            this.btnMetricsAdd.Text = "Add";
            this.btnMetricsAdd.UseVisualStyleBackColor = true;
            this.btnMetricsAdd.Click += new System.EventHandler(this.btnMetricsAdd_Click);
            // 
            // btnMetricsDelete
            // 
            this.btnMetricsDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMetricsDelete.Location = new System.Drawing.Point(6, 381);
            this.btnMetricsDelete.Name = "btnMetricsDelete";
            this.btnMetricsDelete.Size = new System.Drawing.Size(68, 27);
            this.btnMetricsDelete.TabIndex = 1;
            this.btnMetricsDelete.Text = "Delete";
            this.btnMetricsDelete.UseVisualStyleBackColor = true;
            this.btnMetricsDelete.Click += new System.EventHandler(this.btnMetricsDelete_Click);
            // 
            // lstMetrics
            // 
            this.lstMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstMetrics.FormattingEnabled = true;
            this.lstMetrics.ItemHeight = 14;
            this.lstMetrics.Location = new System.Drawing.Point(6, 18);
            this.lstMetrics.Name = "lstMetrics";
            this.lstMetrics.Size = new System.Drawing.Size(174, 354);
            this.lstMetrics.TabIndex = 0;
            this.lstMetrics.SelectedIndexChanged += new System.EventHandler(this.lstMetrics_SelectedIndexChanged);
            // 
            // cboObservationType
            // 
            this.cboObservationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObservationType.FormattingEnabled = true;
            this.cboObservationType.Items.AddRange(new object[] {
            "Hourly",
            "Daily"});
            this.cboObservationType.Location = new System.Drawing.Point(9, 80);
            this.cboObservationType.Name = "cboObservationType";
            this.cboObservationType.Size = new System.Drawing.Size(145, 22);
            this.cboObservationType.TabIndex = 3;
            this.cboObservationType.SelectedIndexChanged += new System.EventHandler(this.cboObservationType_SelectedIndexChanged);
            // 
            // lblObservationType
            // 
            this.lblObservationType.AutoSize = true;
            this.lblObservationType.Location = new System.Drawing.Point(7, 64);
            this.lblObservationType.Name = "lblObservationType";
            this.lblObservationType.Size = new System.Drawing.Size(102, 14);
            this.lblObservationType.TabIndex = 2;
            this.lblObservationType.Text = "Observation Type:";
            // 
            // txtPollutantID
            // 
            this.txtPollutantID.Location = new System.Drawing.Point(8, 38);
            this.txtPollutantID.Name = "txtPollutantID";
            this.txtPollutantID.Size = new System.Drawing.Size(146, 22);
            this.txtPollutantID.TabIndex = 1;
            // 
            // lblPollutantID
            // 
            this.lblPollutantID.AutoSize = true;
            this.lblPollutantID.Location = new System.Drawing.Point(6, 20);
            this.lblPollutantID.Name = "lblPollutantID";
            this.lblPollutantID.Size = new System.Drawing.Size(75, 14);
            this.lblPollutantID.TabIndex = 0;
            this.lblPollutantID.Text = "Pollutant ID:";
            // 
            // PollutantDefinition
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(700, 619);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpDetail);
            this.Controls.Add(this.grpPollutant);
            this.Name = "PollutantDefinition";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pollutant Definition";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PollutantDefinition_FormClosing);
            this.Load += new System.EventHandler(this.PollutantDefinition_Load);
            this.groupBox1.ResumeLayout(false);
            this.grpDetail.ResumeLayout(false);
            this.grpSeasonalMetrics.ResumeLayout(false);
            this.grpHourlyMetricGeneration.ResumeLayout(false);
            this.grpHourlyMetricGeneration.PerformLayout();
            this.tabHourlyMetricGeneration.ResumeLayout(false);
            this.tpFix.ResumeLayout(false);
            this.tpFix.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownEndHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownStartHour)).EndInit();
            this.tpMovingWindow.ResumeLayout(false);
            this.tpMovingWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownWindowSize)).EndInit();
            this.tpCustom.ResumeLayout(false);
            this.tpCustom.PerformLayout();
            this.grpPollutant.ResumeLayout(false);
            this.grpPollutant.PerformLayout();
            this.grpMetrics.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpPollutant;
        private System.Windows.Forms.GroupBox grpMetrics;
        private System.Windows.Forms.Button btnMetricsAdd;
        private System.Windows.Forms.Button btnMetricsDelete;
        private System.Windows.Forms.ListBox lstMetrics;
        private System.Windows.Forms.ComboBox cboObservationType;
        private System.Windows.Forms.Label lblObservationType;
        private System.Windows.Forms.TextBox txtPollutantID;
        private System.Windows.Forms.Label lblPollutantID;
        private System.Windows.Forms.GroupBox grpDetail;
        private System.Windows.Forms.TabControl tabHourlyMetricGeneration;
        private System.Windows.Forms.TabPage tpFix;
        private System.Windows.Forms.TabPage tpMovingWindow;
        private System.Windows.Forms.Label lblEndHour;
        private System.Windows.Forms.Label lblStartHour;
        private System.Windows.Forms.TabPage tpCustom;
        private System.Windows.Forms.NumericUpDown nudownEndHour;
        private System.Windows.Forms.NumericUpDown nudownStartHour;
        private System.Windows.Forms.ComboBox cboFixStatistic;
        private System.Windows.Forms.Label lblStatistic;
        private System.Windows.Forms.Label lblDailyStatistic;
        private System.Windows.Forms.Label lblWindowStatistic;
        private System.Windows.Forms.Label lblWindowSize;
        private System.Windows.Forms.ComboBox cboMovingStatistic;
        private System.Windows.Forms.ComboBox cboWindowStatistic;
        private System.Windows.Forms.NumericUpDown nudownWindowSize;
        private System.Windows.Forms.ListBox lstVariables;
        private System.Windows.Forms.ListBox lstFunctions;
        private System.Windows.Forms.Label lblVariables;
        private System.Windows.Forms.Label lblFunctions;
        private System.Windows.Forms.TextBox txtFunctionManage;
        private System.Windows.Forms.Label lblFunction;
        private System.Windows.Forms.GroupBox grpHourlyMetricGeneration;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grpSeasonalMetrics;
        private System.Windows.Forms.ListBox lstSeasonalMetrics;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdvancedOptions;
        private System.Windows.Forms.Label lblhourlymetricgeneration;
        private System.Windows.Forms.ComboBox cbohourlymetricgeneration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMetricName;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}