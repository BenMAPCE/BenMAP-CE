namespace BenMAP
{
    partial class ManageSetup
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.grpManageSet = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grpIncidenceOrPrevalence = new System.Windows.Forms.GroupBox();
            this.btnIncidence = new System.Windows.Forms.Button();
            this.lstIncidenceOrPrevalence = new System.Windows.Forms.ListBox();
            this.grpMonitorDataSets = new System.Windows.Forms.GroupBox();
            this.btnMonitor = new System.Windows.Forms.Button();
            this.lstMonitor = new System.Windows.Forms.ListBox();
            this.grpPollutants = new System.Windows.Forms.GroupBox();
            this.btnPollutants = new System.Windows.Forms.Button();
            this.lstPollutans = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnGridDef = new System.Windows.Forms.Button();
            this.lstGridDefinitions = new System.Windows.Forms.ListBox();
            this.grpIncomeGrowthAdjustments = new System.Windows.Forms.GroupBox();
            this.btnIncome = new System.Windows.Forms.Button();
            this.lstIncome = new System.Windows.Forms.ListBox();
            this.grpPopulationDataSets = new System.Windows.Forms.GroupBox();
            this.btnPopulation = new System.Windows.Forms.Button();
            this.lstPopulation = new System.Windows.Forms.ListBox();
            this.grpValuationDataSets = new System.Windows.Forms.GroupBox();
            this.btnValuation = new System.Windows.Forms.Button();
            this.lstValuation = new System.Windows.Forms.ListBox();
            this.grpHealthImpactDataSets = new System.Windows.Forms.GroupBox();
            this.btnHealthImpact = new System.Windows.Forms.Button();
            this.lstHealthImpact = new System.Windows.Forms.ListBox();
            this.grpInflationDataSets = new System.Windows.Forms.GroupBox();
            this.btnInflation = new System.Windows.Forms.Button();
            this.lstInflation = new System.Windows.Forms.ListBox();
            this.grpVariableDataSets = new System.Windows.Forms.GroupBox();
            this.btnVariable = new System.Windows.Forms.Button();
            this.lstVariable = new System.Windows.Forms.ListBox();
            this.grpNewName = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblSetupName = new System.Windows.Forms.Label();
            this.cboSetupName = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.grpManageSet.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpIncidenceOrPrevalence.SuspendLayout();
            this.grpMonitorDataSets.SuspendLayout();
            this.grpPollutants.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grpIncomeGrowthAdjustments.SuspendLayout();
            this.grpPopulationDataSets.SuspendLayout();
            this.grpValuationDataSets.SuspendLayout();
            this.grpHealthImpactDataSets.SuspendLayout();
            this.grpInflationDataSets.SuspendLayout();
            this.grpVariableDataSets.SuspendLayout();
            this.grpNewName.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnOk);
            this.groupBox2.Location = new System.Drawing.Point(15, 622);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(663, 41);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(525, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 25);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(593, 11);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(64, 25);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // grpManageSet
            // 
            this.grpManageSet.AutoSize = true;
            this.grpManageSet.Controls.Add(this.tableLayoutPanel1);
            this.grpManageSet.Controls.Add(this.grpNewName);
            this.grpManageSet.Controls.Add(this.groupBox1);
            this.grpManageSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpManageSet.Location = new System.Drawing.Point(0, 0);
            this.grpManageSet.Name = "grpManageSet";
            this.grpManageSet.Size = new System.Drawing.Size(691, 665);
            this.grpManageSet.TabIndex = 0;
            this.grpManageSet.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.grpIncidenceOrPrevalence, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.grpMonitorDataSets, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpPollutants, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpIncomeGrowthAdjustments, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.grpPopulationDataSets, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.grpValuationDataSets, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.grpHealthImpactDataSets, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.grpInflationDataSets, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.grpVariableDataSets, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 59);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 560);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // grpIncidenceOrPrevalence
            // 
            this.grpIncidenceOrPrevalence.Controls.Add(this.btnIncidence);
            this.grpIncidenceOrPrevalence.Controls.Add(this.lstIncidenceOrPrevalence);
            this.grpIncidenceOrPrevalence.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpIncidenceOrPrevalence.Location = new System.Drawing.Point(3, 143);
            this.grpIncidenceOrPrevalence.Name = "grpIncidenceOrPrevalence";
            this.grpIncidenceOrPrevalence.Size = new System.Drawing.Size(216, 134);
            this.grpIncidenceOrPrevalence.TabIndex = 4;
            this.grpIncidenceOrPrevalence.TabStop = false;
            this.grpIncidenceOrPrevalence.Text = "Incidence/Prevalence Rates";
            // 
            // btnIncidence
            // 
            this.btnIncidence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIncidence.Location = new System.Drawing.Point(153, 108);
            this.btnIncidence.Name = "btnIncidence";
            this.btnIncidence.Size = new System.Drawing.Size(60, 25);
            this.btnIncidence.TabIndex = 1;
            this.btnIncidence.Text = "Manage";
            this.btnIncidence.UseVisualStyleBackColor = true;
            this.btnIncidence.Click += new System.EventHandler(this.btnIncidence_Click);
            // 
            // lstIncidenceOrPrevalence
            // 
            this.lstIncidenceOrPrevalence.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstIncidenceOrPrevalence.FormattingEnabled = true;
            this.lstIncidenceOrPrevalence.ItemHeight = 14;
            this.lstIncidenceOrPrevalence.Location = new System.Drawing.Point(3, 17);
            this.lstIncidenceOrPrevalence.Name = "lstIncidenceOrPrevalence";
            this.lstIncidenceOrPrevalence.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstIncidenceOrPrevalence.Size = new System.Drawing.Size(210, 88);
            this.lstIncidenceOrPrevalence.TabIndex = 0;
            // 
            // grpMonitorDataSets
            // 
            this.grpMonitorDataSets.Controls.Add(this.btnMonitor);
            this.grpMonitorDataSets.Controls.Add(this.lstMonitor);
            this.grpMonitorDataSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMonitorDataSets.Location = new System.Drawing.Point(447, 3);
            this.grpMonitorDataSets.Name = "grpMonitorDataSets";
            this.grpMonitorDataSets.Size = new System.Drawing.Size(217, 134);
            this.grpMonitorDataSets.TabIndex = 3;
            this.grpMonitorDataSets.TabStop = false;
            this.grpMonitorDataSets.Text = "Monitor Datasets";
            // 
            // btnMonitor
            // 
            this.btnMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMonitor.Location = new System.Drawing.Point(151, 108);
            this.btnMonitor.Name = "btnMonitor";
            this.btnMonitor.Size = new System.Drawing.Size(60, 25);
            this.btnMonitor.TabIndex = 1;
            this.btnMonitor.Text = "Manage";
            this.btnMonitor.UseVisualStyleBackColor = true;
            this.btnMonitor.Click += new System.EventHandler(this.btnMonitor_Click);
            // 
            // lstMonitor
            // 
            this.lstMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstMonitor.FormattingEnabled = true;
            this.lstMonitor.ItemHeight = 14;
            this.lstMonitor.Location = new System.Drawing.Point(3, 18);
            this.lstMonitor.Name = "lstMonitor";
            this.lstMonitor.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstMonitor.Size = new System.Drawing.Size(208, 88);
            this.lstMonitor.TabIndex = 0;
            // 
            // grpPollutants
            // 
            this.grpPollutants.Controls.Add(this.btnPollutants);
            this.grpPollutants.Controls.Add(this.lstPollutans);
            this.grpPollutants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPollutants.Location = new System.Drawing.Point(225, 3);
            this.grpPollutants.Name = "grpPollutants";
            this.grpPollutants.Size = new System.Drawing.Size(216, 134);
            this.grpPollutants.TabIndex = 2;
            this.grpPollutants.TabStop = false;
            this.grpPollutants.Text = "Pollutants";
            // 
            // btnPollutants
            // 
            this.btnPollutants.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPollutants.Location = new System.Drawing.Point(153, 108);
            this.btnPollutants.Name = "btnPollutants";
            this.btnPollutants.Size = new System.Drawing.Size(60, 25);
            this.btnPollutants.TabIndex = 1;
            this.btnPollutants.Text = "Manage";
            this.btnPollutants.UseVisualStyleBackColor = true;
            this.btnPollutants.Click += new System.EventHandler(this.btnPollutants_Click);
            // 
            // lstPollutans
            // 
            this.lstPollutans.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPollutans.FormattingEnabled = true;
            this.lstPollutans.ItemHeight = 14;
            this.lstPollutans.Location = new System.Drawing.Point(3, 18);
            this.lstPollutans.Name = "lstPollutans";
            this.lstPollutans.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstPollutans.Size = new System.Drawing.Size(210, 88);
            this.lstPollutans.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnGridDef);
            this.groupBox3.Controls.Add(this.lstGridDefinitions);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(216, 134);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Grid Definitions";
            // 
            // btnGridDef
            // 
            this.btnGridDef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGridDef.Location = new System.Drawing.Point(153, 108);
            this.btnGridDef.Name = "btnGridDef";
            this.btnGridDef.Size = new System.Drawing.Size(60, 25);
            this.btnGridDef.TabIndex = 1;
            this.btnGridDef.Text = "Manage";
            this.btnGridDef.UseVisualStyleBackColor = true;
            this.btnGridDef.Click += new System.EventHandler(this.btnGridDef_Click);
            // 
            // lstGridDefinitions
            // 
            this.lstGridDefinitions.AllowDrop = true;
            this.lstGridDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstGridDefinitions.FormattingEnabled = true;
            this.lstGridDefinitions.ItemHeight = 14;
            this.lstGridDefinitions.Location = new System.Drawing.Point(3, 18);
            this.lstGridDefinitions.Name = "lstGridDefinitions";
            this.lstGridDefinitions.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstGridDefinitions.Size = new System.Drawing.Size(210, 88);
            this.lstGridDefinitions.TabIndex = 0;
            // 
            // grpIncomeGrowthAdjustments
            // 
            this.grpIncomeGrowthAdjustments.Controls.Add(this.btnIncome);
            this.grpIncomeGrowthAdjustments.Controls.Add(this.lstIncome);
            this.grpIncomeGrowthAdjustments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpIncomeGrowthAdjustments.Location = new System.Drawing.Point(3, 423);
            this.grpIncomeGrowthAdjustments.Name = "grpIncomeGrowthAdjustments";
            this.grpIncomeGrowthAdjustments.Size = new System.Drawing.Size(216, 134);
            this.grpIncomeGrowthAdjustments.TabIndex = 10;
            this.grpIncomeGrowthAdjustments.TabStop = false;
            this.grpIncomeGrowthAdjustments.Text = "Income Growth Adjustments";
            // 
            // btnIncome
            // 
            this.btnIncome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIncome.Location = new System.Drawing.Point(153, 108);
            this.btnIncome.Name = "btnIncome";
            this.btnIncome.Size = new System.Drawing.Size(60, 25);
            this.btnIncome.TabIndex = 1;
            this.btnIncome.Text = "Manage";
            this.btnIncome.UseVisualStyleBackColor = true;
            this.btnIncome.Click += new System.EventHandler(this.btnIncome_Click);
            // 
            // lstIncome
            // 
            this.lstIncome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstIncome.FormattingEnabled = true;
            this.lstIncome.ItemHeight = 14;
            this.lstIncome.Location = new System.Drawing.Point(3, 17);
            this.lstIncome.Name = "lstIncome";
            this.lstIncome.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstIncome.Size = new System.Drawing.Size(210, 88);
            this.lstIncome.TabIndex = 0;
            // 
            // grpPopulationDataSets
            // 
            this.grpPopulationDataSets.Controls.Add(this.btnPopulation);
            this.grpPopulationDataSets.Controls.Add(this.lstPopulation);
            this.grpPopulationDataSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPopulationDataSets.Location = new System.Drawing.Point(225, 143);
            this.grpPopulationDataSets.Name = "grpPopulationDataSets";
            this.grpPopulationDataSets.Size = new System.Drawing.Size(216, 134);
            this.grpPopulationDataSets.TabIndex = 5;
            this.grpPopulationDataSets.TabStop = false;
            this.grpPopulationDataSets.Text = "Population Datasets";
            // 
            // btnPopulation
            // 
            this.btnPopulation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPopulation.Location = new System.Drawing.Point(153, 108);
            this.btnPopulation.Name = "btnPopulation";
            this.btnPopulation.Size = new System.Drawing.Size(60, 25);
            this.btnPopulation.TabIndex = 1;
            this.btnPopulation.Text = "Manage";
            this.btnPopulation.UseVisualStyleBackColor = true;
            this.btnPopulation.Click += new System.EventHandler(this.btnPopulation_Click);
            // 
            // lstPopulation
            // 
            this.lstPopulation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPopulation.FormattingEnabled = true;
            this.lstPopulation.ItemHeight = 14;
            this.lstPopulation.Location = new System.Drawing.Point(3, 17);
            this.lstPopulation.Name = "lstPopulation";
            this.lstPopulation.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstPopulation.Size = new System.Drawing.Size(210, 88);
            this.lstPopulation.TabIndex = 0;
            // 
            // grpValuationDataSets
            // 
            this.grpValuationDataSets.Controls.Add(this.btnValuation);
            this.grpValuationDataSets.Controls.Add(this.lstValuation);
            this.grpValuationDataSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpValuationDataSets.Location = new System.Drawing.Point(447, 283);
            this.grpValuationDataSets.Name = "grpValuationDataSets";
            this.grpValuationDataSets.Size = new System.Drawing.Size(217, 134);
            this.grpValuationDataSets.TabIndex = 9;
            this.grpValuationDataSets.TabStop = false;
            this.grpValuationDataSets.Text = "Valuation Functions";
            // 
            // btnValuation
            // 
            this.btnValuation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValuation.Location = new System.Drawing.Point(151, 108);
            this.btnValuation.Name = "btnValuation";
            this.btnValuation.Size = new System.Drawing.Size(60, 25);
            this.btnValuation.TabIndex = 1;
            this.btnValuation.Text = "Manage";
            this.btnValuation.UseVisualStyleBackColor = true;
            this.btnValuation.Click += new System.EventHandler(this.btnValuation_Click);
            // 
            // lstValuation
            // 
            this.lstValuation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstValuation.FormattingEnabled = true;
            this.lstValuation.ItemHeight = 14;
            this.lstValuation.Location = new System.Drawing.Point(3, 17);
            this.lstValuation.Name = "lstValuation";
            this.lstValuation.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstValuation.Size = new System.Drawing.Size(208, 88);
            this.lstValuation.TabIndex = 0;
            // 
            // grpHealthImpactDataSets
            // 
            this.grpHealthImpactDataSets.Controls.Add(this.btnHealthImpact);
            this.grpHealthImpactDataSets.Controls.Add(this.lstHealthImpact);
            this.grpHealthImpactDataSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHealthImpactDataSets.Location = new System.Drawing.Point(447, 143);
            this.grpHealthImpactDataSets.Name = "grpHealthImpactDataSets";
            this.grpHealthImpactDataSets.Size = new System.Drawing.Size(217, 134);
            this.grpHealthImpactDataSets.TabIndex = 5;
            this.grpHealthImpactDataSets.TabStop = false;
            this.grpHealthImpactDataSets.Text = "Health Impact Functions";
            // 
            // btnHealthImpact
            // 
            this.btnHealthImpact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHealthImpact.Location = new System.Drawing.Point(151, 108);
            this.btnHealthImpact.Name = "btnHealthImpact";
            this.btnHealthImpact.Size = new System.Drawing.Size(60, 25);
            this.btnHealthImpact.TabIndex = 1;
            this.btnHealthImpact.Text = "Manage";
            this.btnHealthImpact.UseVisualStyleBackColor = true;
            this.btnHealthImpact.Click += new System.EventHandler(this.btnHealthImpact_Click);
            // 
            // lstHealthImpact
            // 
            this.lstHealthImpact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstHealthImpact.FormattingEnabled = true;
            this.lstHealthImpact.ItemHeight = 14;
            this.lstHealthImpact.Location = new System.Drawing.Point(3, 17);
            this.lstHealthImpact.Name = "lstHealthImpact";
            this.lstHealthImpact.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstHealthImpact.Size = new System.Drawing.Size(208, 88);
            this.lstHealthImpact.TabIndex = 0;
            // 
            // grpInflationDataSets
            // 
            this.grpInflationDataSets.Controls.Add(this.btnInflation);
            this.grpInflationDataSets.Controls.Add(this.lstInflation);
            this.grpInflationDataSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInflationDataSets.Location = new System.Drawing.Point(225, 283);
            this.grpInflationDataSets.Name = "grpInflationDataSets";
            this.grpInflationDataSets.Size = new System.Drawing.Size(216, 134);
            this.grpInflationDataSets.TabIndex = 8;
            this.grpInflationDataSets.TabStop = false;
            this.grpInflationDataSets.Text = "Inflation Datasets";
            // 
            // btnInflation
            // 
            this.btnInflation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInflation.Location = new System.Drawing.Point(153, 108);
            this.btnInflation.Name = "btnInflation";
            this.btnInflation.Size = new System.Drawing.Size(60, 25);
            this.btnInflation.TabIndex = 1;
            this.btnInflation.Text = "Manage";
            this.btnInflation.UseVisualStyleBackColor = true;
            this.btnInflation.Click += new System.EventHandler(this.btnInflation_Click);
            // 
            // lstInflation
            // 
            this.lstInflation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstInflation.FormattingEnabled = true;
            this.lstInflation.ItemHeight = 14;
            this.lstInflation.Location = new System.Drawing.Point(3, 17);
            this.lstInflation.Name = "lstInflation";
            this.lstInflation.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstInflation.Size = new System.Drawing.Size(210, 88);
            this.lstInflation.TabIndex = 0;
            // 
            // grpVariableDataSets
            // 
            this.grpVariableDataSets.Controls.Add(this.btnVariable);
            this.grpVariableDataSets.Controls.Add(this.lstVariable);
            this.grpVariableDataSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpVariableDataSets.Location = new System.Drawing.Point(3, 283);
            this.grpVariableDataSets.Name = "grpVariableDataSets";
            this.grpVariableDataSets.Size = new System.Drawing.Size(216, 134);
            this.grpVariableDataSets.TabIndex = 7;
            this.grpVariableDataSets.TabStop = false;
            this.grpVariableDataSets.Text = "Variable Datasets";
            // 
            // btnVariable
            // 
            this.btnVariable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVariable.Location = new System.Drawing.Point(153, 108);
            this.btnVariable.Name = "btnVariable";
            this.btnVariable.Size = new System.Drawing.Size(60, 25);
            this.btnVariable.TabIndex = 7;
            this.btnVariable.Text = "Manage";
            this.btnVariable.UseVisualStyleBackColor = true;
            this.btnVariable.Click += new System.EventHandler(this.btnVariable_Click);
            // 
            // lstVariable
            // 
            this.lstVariable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstVariable.FormattingEnabled = true;
            this.lstVariable.ItemHeight = 14;
            this.lstVariable.Location = new System.Drawing.Point(3, 17);
            this.lstVariable.Name = "lstVariable";
            this.lstVariable.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstVariable.Size = new System.Drawing.Size(210, 88);
            this.lstVariable.TabIndex = 6;
            // 
            // grpNewName
            // 
            this.grpNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNewName.Controls.Add(this.btnDelete);
            this.grpNewName.Controls.Add(this.btnAdd);
            this.grpNewName.Controls.Add(this.lblSetupName);
            this.grpNewName.Controls.Add(this.cboSetupName);
            this.grpNewName.Location = new System.Drawing.Point(15, 12);
            this.grpNewName.Name = "grpNewName";
            this.grpNewName.Size = new System.Drawing.Size(663, 41);
            this.grpNewName.TabIndex = 2;
            this.grpNewName.TabStop = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(595, 11);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(62, 25);
            this.btnDelete.TabIndex = 15;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(525, 11);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(62, 25);
            this.btnAdd.TabIndex = 14;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblSetupName
            // 
            this.lblSetupName.AutoSize = true;
            this.lblSetupName.Location = new System.Drawing.Point(6, 17);
            this.lblSetupName.Name = "lblSetupName";
            this.lblSetupName.Size = new System.Drawing.Size(99, 14);
            this.lblSetupName.TabIndex = 13;
            this.lblSetupName.Text = "Available Setups";
            // 
            // cboSetupName
            // 
            this.cboSetupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSetupName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSetupName.FormattingEnabled = true;
            this.cboSetupName.Location = new System.Drawing.Point(111, 12);
            this.cboSetupName.Name = "cboSetupName";
            this.cboSetupName.Size = new System.Drawing.Size(158, 22);
            this.cboSetupName.TabIndex = 12;
            this.cboSetupName.SelectedValueChanged += new System.EventHandler(this.cboSetupName_SelectedValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(191, -86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(50, 79);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // ManageSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(691, 665);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grpManageSet);
            this.Name = "ManageSetup";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modify Datasets";
            this.Load += new System.EventHandler(this.frmManageSetup_Load);
            this.groupBox2.ResumeLayout(false);
            this.grpManageSet.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.grpIncidenceOrPrevalence.ResumeLayout(false);
            this.grpMonitorDataSets.ResumeLayout(false);
            this.grpPollutants.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.grpIncomeGrowthAdjustments.ResumeLayout(false);
            this.grpPopulationDataSets.ResumeLayout(false);
            this.grpValuationDataSets.ResumeLayout(false);
            this.grpHealthImpactDataSets.ResumeLayout(false);
            this.grpInflationDataSets.ResumeLayout(false);
            this.grpVariableDataSets.ResumeLayout(false);
            this.grpNewName.ResumeLayout(false);
            this.grpNewName.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox grpHealthImpactDataSets;
        private System.Windows.Forms.Button btnHealthImpact;
        private System.Windows.Forms.ListBox lstHealthImpact;
        private System.Windows.Forms.GroupBox grpVariableDataSets;
        private System.Windows.Forms.Button btnVariable;
        private System.Windows.Forms.ListBox lstVariable;
        private System.Windows.Forms.GroupBox grpInflationDataSets;
        private System.Windows.Forms.Button btnInflation;
        private System.Windows.Forms.ListBox lstInflation;
        private System.Windows.Forms.GroupBox grpValuationDataSets;
        private System.Windows.Forms.Button btnValuation;
        private System.Windows.Forms.ListBox lstValuation;
        private System.Windows.Forms.GroupBox grpIncomeGrowthAdjustments;
        private System.Windows.Forms.Button btnIncome;
        private System.Windows.Forms.ListBox lstIncome;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpNewName;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblSetupName;
        private System.Windows.Forms.ComboBox cboSetupName;
        private System.Windows.Forms.GroupBox grpManageSet;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpIncidenceOrPrevalence;
        private System.Windows.Forms.Button btnIncidence;
        private System.Windows.Forms.ListBox lstIncidenceOrPrevalence;
        private System.Windows.Forms.GroupBox grpMonitorDataSets;
        private System.Windows.Forms.Button btnMonitor;
        private System.Windows.Forms.ListBox lstMonitor;
        private System.Windows.Forms.GroupBox grpPollutants;
        private System.Windows.Forms.Button btnPollutants;
        private System.Windows.Forms.ListBox lstPollutans;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnGridDef;
        private System.Windows.Forms.ListBox lstGridDefinitions;
        private System.Windows.Forms.GroupBox grpPopulationDataSets;
        private System.Windows.Forms.Button btnPopulation;
        private System.Windows.Forms.ListBox lstPopulation;
    }
}