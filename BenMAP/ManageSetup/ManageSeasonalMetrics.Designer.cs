namespace BenMAP
{
    partial class ManageSeasonalMetrics
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
            this.grpManageSeasonalMetrics = new System.Windows.Forms.GroupBox();
            this.txtSeasonMetricName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpSeasonalMetricSeasons = new System.Windows.Forms.GroupBox();
            this.btnDeleteSeasonalMetricSeason = new System.Windows.Forms.Button();
            this.btnAddSeasonalMetricSeason = new System.Windows.Forms.Button();
            this.lstSeasonalMetricSeasons = new System.Windows.Forms.ListBox();
            this.grpSelectSeasonDetail = new System.Windows.Forms.GroupBox();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.tabMetricFunction = new System.Windows.Forms.TabControl();
            this.tbpStatistic = new System.Windows.Forms.TabPage();
            this.cboStatisticFunction = new System.Windows.Forms.ComboBox();
            this.lblStatisticFunction = new System.Windows.Forms.Label();
            this.tbpCustomFunction = new System.Windows.Forms.TabPage();
            this.txtFunctionManage = new System.Windows.Forms.TextBox();
            this.lblSeasonalFunction = new System.Windows.Forms.Label();
            this.lstVariables = new System.Windows.Forms.ListBox();
            this.lstFunctions = new System.Windows.Forms.ListBox();
            this.lblAvailableVariables = new System.Windows.Forms.Label();
            this.lblCustomAvailableFunction = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.txtMetricIDName = new System.Windows.Forms.TextBox();
            this.lblMetricID = new System.Windows.Forms.Label();
            this.grpSeasonalMetrics = new System.Windows.Forms.GroupBox();
            this.lstSeasonalMetrics = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpManageSeasonalMetrics.SuspendLayout();
            this.grpSeasonalMetricSeasons.SuspendLayout();
            this.grpSelectSeasonDetail.SuspendLayout();
            this.tabMetricFunction.SuspendLayout();
            this.tbpStatistic.SuspendLayout();
            this.tbpCustomFunction.SuspendLayout();
            this.grpSeasonalMetrics.SuspendLayout();
            this.grpCancelOK.SuspendLayout();
            this.SuspendLayout();
                                                this.grpManageSeasonalMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpManageSeasonalMetrics.Controls.Add(this.txtSeasonMetricName);
            this.grpManageSeasonalMetrics.Controls.Add(this.label5);
            this.grpManageSeasonalMetrics.Controls.Add(this.grpSeasonalMetricSeasons);
            this.grpManageSeasonalMetrics.Controls.Add(this.grpSelectSeasonDetail);
            this.grpManageSeasonalMetrics.Controls.Add(this.txtMetricIDName);
            this.grpManageSeasonalMetrics.Controls.Add(this.lblMetricID);
            this.grpManageSeasonalMetrics.Controls.Add(this.grpSeasonalMetrics);
            this.grpManageSeasonalMetrics.Location = new System.Drawing.Point(12, 6);
            this.grpManageSeasonalMetrics.Name = "grpManageSeasonalMetrics";
            this.grpManageSeasonalMetrics.Size = new System.Drawing.Size(780, 368);
            this.grpManageSeasonalMetrics.TabIndex = 4;
            this.grpManageSeasonalMetrics.TabStop = false;
                                                this.txtSeasonMetricName.Location = new System.Drawing.Point(335, 19);
            this.txtSeasonMetricName.Name = "txtSeasonMetricName";
            this.txtSeasonMetricName.Size = new System.Drawing.Size(117, 22);
            this.txtSeasonMetricName.TabIndex = 14;
            this.txtSeasonMetricName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSeasonMetricName_KeyDown);
            this.txtSeasonMetricName.Leave += new System.EventHandler(this.txtSeasonMetricName_Leave);
                                                this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(198, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 14);
            this.label5.TabIndex = 13;
            this.label5.Text = "Seasonal Metric Name:";
                                                this.grpSeasonalMetricSeasons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.grpSeasonalMetricSeasons.Controls.Add(this.btnDeleteSeasonalMetricSeason);
            this.grpSeasonalMetricSeasons.Controls.Add(this.btnAddSeasonalMetricSeason);
            this.grpSeasonalMetricSeasons.Controls.Add(this.lstSeasonalMetricSeasons);
            this.grpSeasonalMetricSeasons.Location = new System.Drawing.Point(188, 50);
            this.grpSeasonalMetricSeasons.Name = "grpSeasonalMetricSeasons";
            this.grpSeasonalMetricSeasons.Size = new System.Drawing.Size(176, 311);
            this.grpSeasonalMetricSeasons.TabIndex = 9;
            this.grpSeasonalMetricSeasons.TabStop = false;
            this.grpSeasonalMetricSeasons.Text = "Seasonal Metric Seasons";
                                                this.btnDeleteSeasonalMetricSeason.Location = new System.Drawing.Point(6, 278);
            this.btnDeleteSeasonalMetricSeason.Name = "btnDeleteSeasonalMetricSeason";
            this.btnDeleteSeasonalMetricSeason.Size = new System.Drawing.Size(75, 27);
            this.btnDeleteSeasonalMetricSeason.TabIndex = 10;
            this.btnDeleteSeasonalMetricSeason.Text = "Delete";
            this.btnDeleteSeasonalMetricSeason.UseVisualStyleBackColor = true;
            this.btnDeleteSeasonalMetricSeason.Click += new System.EventHandler(this.btnDeleteSeasonalMetricSeason_Click);
                                                this.btnAddSeasonalMetricSeason.Location = new System.Drawing.Point(95, 278);
            this.btnAddSeasonalMetricSeason.Name = "btnAddSeasonalMetricSeason";
            this.btnAddSeasonalMetricSeason.Size = new System.Drawing.Size(75, 27);
            this.btnAddSeasonalMetricSeason.TabIndex = 10;
            this.btnAddSeasonalMetricSeason.Text = "Add";
            this.btnAddSeasonalMetricSeason.UseVisualStyleBackColor = true;
            this.btnAddSeasonalMetricSeason.Click += new System.EventHandler(this.btnAddSeasonalMetricSeason_Click);
                                                this.lstSeasonalMetricSeasons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstSeasonalMetricSeasons.FormattingEnabled = true;
            this.lstSeasonalMetricSeasons.ItemHeight = 14;
            this.lstSeasonalMetricSeasons.Location = new System.Drawing.Point(3, 18);
            this.lstSeasonalMetricSeasons.Name = "lstSeasonalMetricSeasons";
            this.lstSeasonalMetricSeasons.Size = new System.Drawing.Size(170, 256);
            this.lstSeasonalMetricSeasons.TabIndex = 3;
            this.lstSeasonalMetricSeasons.SelectedIndexChanged += new System.EventHandler(this.lstSeasonalMetricSeasons_SelectedIndexChanged);
                                                this.grpSelectSeasonDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSelectSeasonDetail.Controls.Add(this.dtpEndTime);
            this.grpSelectSeasonDetail.Controls.Add(this.dtpStartTime);
            this.grpSelectSeasonDetail.Controls.Add(this.tabMetricFunction);
            this.grpSelectSeasonDetail.Controls.Add(this.lblEndDate);
            this.grpSelectSeasonDetail.Controls.Add(this.lblStartDate);
            this.grpSelectSeasonDetail.Location = new System.Drawing.Point(370, 50);
            this.grpSelectSeasonDetail.Name = "grpSelectSeasonDetail";
            this.grpSelectSeasonDetail.Size = new System.Drawing.Size(400, 311);
            this.grpSelectSeasonDetail.TabIndex = 12;
            this.grpSelectSeasonDetail.TabStop = false;
            this.grpSelectSeasonDetail.Text = "Selected Season Details";
                                                this.dtpEndTime.Enabled = false;
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.Location = new System.Drawing.Point(89, 53);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.ShowUpDown = true;
            this.dtpEndTime.Size = new System.Drawing.Size(101, 22);
            this.dtpEndTime.TabIndex = 30;
            this.dtpEndTime.Leave += new System.EventHandler(this.dtpEndTime_Leave);
                                                this.dtpStartTime.Enabled = false;
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(89, 25);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(101, 22);
            this.dtpStartTime.TabIndex = 29;
            this.dtpStartTime.Leave += new System.EventHandler(this.dtpStartTime_Leave);
                                                this.tabMetricFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMetricFunction.Controls.Add(this.tbpStatistic);
            this.tabMetricFunction.Controls.Add(this.tbpCustomFunction);
            this.tabMetricFunction.Location = new System.Drawing.Point(12, 93);
            this.tabMetricFunction.Name = "tabMetricFunction";
            this.tabMetricFunction.SelectedIndex = 0;
            this.tabMetricFunction.Size = new System.Drawing.Size(378, 212);
            this.tabMetricFunction.TabIndex = 6;
            this.tabMetricFunction.SelectedIndexChanged += new System.EventHandler(this.tabMetricFunction_SelectedIndexChanged);
            this.tabMetricFunction.Leave += new System.EventHandler(this.tabMetricFunction_Leave);
                                                this.tbpStatistic.Controls.Add(this.cboStatisticFunction);
            this.tbpStatistic.Controls.Add(this.lblStatisticFunction);
            this.tbpStatistic.Location = new System.Drawing.Point(4, 23);
            this.tbpStatistic.Name = "tbpStatistic";
            this.tbpStatistic.Padding = new System.Windows.Forms.Padding(3);
            this.tbpStatistic.Size = new System.Drawing.Size(370, 185);
            this.tbpStatistic.TabIndex = 0;
            this.tbpStatistic.Text = "Statistic";
            this.tbpStatistic.UseVisualStyleBackColor = true;
                                                this.cboStatisticFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatisticFunction.FormattingEnabled = true;
            this.cboStatisticFunction.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cboStatisticFunction.Location = new System.Drawing.Point(87, 20);
            this.cboStatisticFunction.Name = "cboStatisticFunction";
            this.cboStatisticFunction.Size = new System.Drawing.Size(121, 22);
            this.cboStatisticFunction.TabIndex = 1;
                                                this.lblStatisticFunction.AutoSize = true;
            this.lblStatisticFunction.Location = new System.Drawing.Point(16, 23);
            this.lblStatisticFunction.Name = "lblStatisticFunction";
            this.lblStatisticFunction.Size = new System.Drawing.Size(52, 14);
            this.lblStatisticFunction.TabIndex = 0;
            this.lblStatisticFunction.Text = "Statistic:";
                                                this.tbpCustomFunction.Controls.Add(this.txtFunctionManage);
            this.tbpCustomFunction.Controls.Add(this.lblSeasonalFunction);
            this.tbpCustomFunction.Controls.Add(this.lstVariables);
            this.tbpCustomFunction.Controls.Add(this.lstFunctions);
            this.tbpCustomFunction.Controls.Add(this.lblAvailableVariables);
            this.tbpCustomFunction.Controls.Add(this.lblCustomAvailableFunction);
            this.tbpCustomFunction.Location = new System.Drawing.Point(4, 23);
            this.tbpCustomFunction.Name = "tbpCustomFunction";
            this.tbpCustomFunction.Padding = new System.Windows.Forms.Padding(3);
            this.tbpCustomFunction.Size = new System.Drawing.Size(370, 184);
            this.tbpCustomFunction.TabIndex = 1;
            this.tbpCustomFunction.Text = "Custom Function";
            this.tbpCustomFunction.UseVisualStyleBackColor = true;
                                                this.txtFunctionManage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFunctionManage.Location = new System.Drawing.Point(65, 154);
            this.txtFunctionManage.Multiline = true;
            this.txtFunctionManage.Name = "txtFunctionManage";
            this.txtFunctionManage.Size = new System.Drawing.Size(299, 24);
            this.txtFunctionManage.TabIndex = 5;
                                                this.lblSeasonalFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSeasonalFunction.AutoSize = true;
            this.lblSeasonalFunction.Location = new System.Drawing.Point(3, 157);
            this.lblSeasonalFunction.Name = "lblSeasonalFunction";
            this.lblSeasonalFunction.Size = new System.Drawing.Size(56, 14);
            this.lblSeasonalFunction.TabIndex = 4;
            this.lblSeasonalFunction.Text = "Function:";
                                                this.lstVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstVariables.FormattingEnabled = true;
            this.lstVariables.ItemHeight = 14;
            this.lstVariables.Items.AddRange(new object[] {
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
            this.lstVariables.Location = new System.Drawing.Point(176, 24);
            this.lstVariables.Name = "lstVariables";
            this.lstVariables.Size = new System.Drawing.Size(188, 116);
            this.lstVariables.TabIndex = 3;
            this.lstVariables.DoubleClick += new System.EventHandler(this.lstVariables_DoubleClick);
                                                this.lstFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
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
            this.lstFunctions.Location = new System.Drawing.Point(6, 24);
            this.lstFunctions.Name = "lstFunctions";
            this.lstFunctions.Size = new System.Drawing.Size(164, 116);
            this.lstFunctions.TabIndex = 2;
            this.lstFunctions.DoubleClick += new System.EventHandler(this.lstFunctions_DoubleClick);
                                                this.lblAvailableVariables.AutoSize = true;
            this.lblAvailableVariables.Location = new System.Drawing.Point(173, 7);
            this.lblAvailableVariables.Name = "lblAvailableVariables";
            this.lblAvailableVariables.Size = new System.Drawing.Size(117, 14);
            this.lblAvailableVariables.TabIndex = 1;
            this.lblAvailableVariables.Text = "Available Variables:";
                                                this.lblCustomAvailableFunction.AutoSize = true;
            this.lblCustomAvailableFunction.Location = new System.Drawing.Point(3, 7);
            this.lblCustomAvailableFunction.Name = "lblCustomAvailableFunction";
            this.lblCustomAvailableFunction.Size = new System.Drawing.Size(117, 14);
            this.lblCustomAvailableFunction.TabIndex = 0;
            this.lblCustomAvailableFunction.Text = "Available Functions:";
                                                this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(24, 57);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(59, 14);
            this.lblEndDate.TabIndex = 17;
            this.lblEndDate.Text = "End Date:";
                                                this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(19, 29);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(64, 14);
            this.lblStartDate.TabIndex = 16;
            this.lblStartDate.Text = "Start Date:";
                                                this.txtMetricIDName.Enabled = false;
            this.txtMetricIDName.Location = new System.Drawing.Point(78, 19);
            this.txtMetricIDName.Name = "txtMetricIDName";
            this.txtMetricIDName.Size = new System.Drawing.Size(100, 22);
            this.txtMetricIDName.TabIndex = 11;
                                                this.lblMetricID.AutoSize = true;
            this.lblMetricID.Location = new System.Drawing.Point(12, 23);
            this.lblMetricID.Name = "lblMetricID";
            this.lblMetricID.Size = new System.Drawing.Size(59, 14);
            this.lblMetricID.TabIndex = 10;
            this.lblMetricID.Text = "Metric ID:";
                                                this.grpSeasonalMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.grpSeasonalMetrics.Controls.Add(this.lstSeasonalMetrics);
            this.grpSeasonalMetrics.Controls.Add(this.btnAdd);
            this.grpSeasonalMetrics.Controls.Add(this.btnDelete);
            this.grpSeasonalMetrics.Location = new System.Drawing.Point(6, 50);
            this.grpSeasonalMetrics.Name = "grpSeasonalMetrics";
            this.grpSeasonalMetrics.Size = new System.Drawing.Size(176, 311);
            this.grpSeasonalMetrics.TabIndex = 8;
            this.grpSeasonalMetrics.TabStop = false;
            this.grpSeasonalMetrics.Text = "Seasonal Metrics";
                                                this.lstSeasonalMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstSeasonalMetrics.FormattingEnabled = true;
            this.lstSeasonalMetrics.ItemHeight = 14;
            this.lstSeasonalMetrics.Location = new System.Drawing.Point(3, 18);
            this.lstSeasonalMetrics.Name = "lstSeasonalMetrics";
            this.lstSeasonalMetrics.Size = new System.Drawing.Size(170, 256);
            this.lstSeasonalMetrics.TabIndex = 1;
            this.lstSeasonalMetrics.SelectedIndexChanged += new System.EventHandler(this.lstSeasonalMetrics_SelectedIndexChanged);
                                                this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(95, 278);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 27);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
                                                this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(6, 278);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
                                                this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(12, 380);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(780, 52);
            this.grpCancelOK.TabIndex = 5;
            this.grpCancelOK.TabStop = false;
                                                this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(685, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(604, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 444);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpManageSeasonalMetrics);
            this.MinimumSize = new System.Drawing.Size(817, 482);
            this.Name = "ManageSeasonalMetrics";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Seasonal Metrics";
            this.Load += new System.EventHandler(this.ManageSeasonalMetrics_Load);
            this.grpManageSeasonalMetrics.ResumeLayout(false);
            this.grpManageSeasonalMetrics.PerformLayout();
            this.grpSeasonalMetricSeasons.ResumeLayout(false);
            this.grpSelectSeasonDetail.ResumeLayout(false);
            this.grpSelectSeasonDetail.PerformLayout();
            this.tabMetricFunction.ResumeLayout(false);
            this.tbpStatistic.ResumeLayout(false);
            this.tbpStatistic.PerformLayout();
            this.tbpCustomFunction.ResumeLayout(false);
            this.tbpCustomFunction.PerformLayout();
            this.grpSeasonalMetrics.ResumeLayout(false);
            this.grpCancelOK.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpManageSeasonalMetrics;
        private System.Windows.Forms.GroupBox grpSelectSeasonDetail;
        private System.Windows.Forms.TabControl tabMetricFunction;
        private System.Windows.Forms.TabPage tbpStatistic;
        private System.Windows.Forms.ComboBox cboStatisticFunction;
        private System.Windows.Forms.Label lblStatisticFunction;
        private System.Windows.Forms.TabPage tbpCustomFunction;
        private System.Windows.Forms.TextBox txtFunctionManage;
        private System.Windows.Forms.Label lblSeasonalFunction;
        private System.Windows.Forms.ListBox lstVariables;
        private System.Windows.Forms.ListBox lstFunctions;
        private System.Windows.Forms.Label lblAvailableVariables;
        private System.Windows.Forms.Label lblCustomAvailableFunction;
        private System.Windows.Forms.TextBox txtMetricIDName;
        private System.Windows.Forms.Label lblMetricID;
        private System.Windows.Forms.GroupBox grpSeasonalMetricSeasons;
        private System.Windows.Forms.ListBox lstSeasonalMetricSeasons;
        private System.Windows.Forms.GroupBox grpSeasonalMetrics;
        private System.Windows.Forms.ListBox lstSeasonalMetrics;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.TextBox txtSeasonMetricName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Button btnDeleteSeasonalMetricSeason;
        private System.Windows.Forms.Button btnAddSeasonalMetricSeason;
    }
}