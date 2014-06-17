namespace BenMAP
{
    partial class GBDRollback
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.gbCountrySelection = new System.Windows.Forms.GroupBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.btnNext2 = new System.Windows.Forms.Button();
            this.tvCountries = new System.Windows.Forms.TreeView();
            this.gbRollbacks = new System.Windows.Forms.GroupBox();
            this.dgvRollbacks = new System.Windows.Forms.DataGridView();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.mapGBD = new DotSpatial.Controls.Map();
            this.gbParameterSelection = new System.Windows.Forms.GroupBox();
            this.gbOptionsIncremental = new System.Windows.Forms.GroupBox();
            this.txtIncrementBackground = new System.Windows.Forms.TextBox();
            this.txtIncrement = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboRollbackType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chartPreview = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnSaveRollback = new System.Windows.Forms.Button();
            this.btnBack2 = new System.Windows.Forms.Button();
            this.gbOptionsPercentage = new System.Windows.Forms.GroupBox();
            this.txtPercentageBackground = new System.Windows.Forms.TextBox();
            this.txtPercentage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.gbOptionsStandard = new System.Windows.Forms.GroupBox();
            this.cboStandard = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.gbName = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.toolTipScenarioName = new System.Windows.Forms.ToolTip(this.components);
            this.btnExecuteRollbacks = new System.Windows.Forms.Button();
            this.btnEditRollback = new System.Windows.Forms.Button();
            this.btnDeleteRollback = new System.Windows.Forms.Button();
            this.colArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCountry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalCountries = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalPopulation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRollbackType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colParameters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbCountrySelection.SuspendLayout();
            this.gbRollbacks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRollbacks)).BeginInit();
            this.gbMap.SuspendLayout();
            this.gbParameterSelection.SuspendLayout();
            this.gbOptionsIncremental.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPreview)).BeginInit();
            this.gbOptionsPercentage.SuspendLayout();
            this.gbOptionsStandard.SuspendLayout();
            this.gbName.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbCountrySelection
            // 
            this.gbCountrySelection.Controls.Add(this.btnBack);
            this.gbCountrySelection.Controls.Add(this.txtInfo);
            this.gbCountrySelection.Controls.Add(this.btnNext2);
            this.gbCountrySelection.Controls.Add(this.tvCountries);
            this.gbCountrySelection.Location = new System.Drawing.Point(1208, 12);
            this.gbCountrySelection.Name = "gbCountrySelection";
            this.gbCountrySelection.Size = new System.Drawing.Size(279, 420);
            this.gbCountrySelection.TabIndex = 0;
            this.gbCountrySelection.TabStop = false;
            this.gbCountrySelection.Text = "Country Selection";
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(68, 386);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(97, 23);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "<- Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(10, 261);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.Size = new System.Drawing.Size(258, 119);
            this.txtInfo.TabIndex = 2;
            // 
            // btnNext2
            // 
            this.btnNext2.Location = new System.Drawing.Point(171, 386);
            this.btnNext2.Name = "btnNext2";
            this.btnNext2.Size = new System.Drawing.Size(97, 23);
            this.btnNext2.TabIndex = 1;
            this.btnNext2.Text = "Next ->";
            this.btnNext2.UseVisualStyleBackColor = true;
            this.btnNext2.Click += new System.EventHandler(this.btnNext2_Click);
            // 
            // tvCountries
            // 
            this.tvCountries.CheckBoxes = true;
            this.tvCountries.Location = new System.Drawing.Point(10, 20);
            this.tvCountries.Name = "tvCountries";
            this.tvCountries.Size = new System.Drawing.Size(258, 234);
            this.tvCountries.TabIndex = 0;
            this.tvCountries.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvCountries_AfterCheck);
            // 
            // gbRollbacks
            // 
            this.gbRollbacks.Controls.Add(this.btnDeleteRollback);
            this.gbRollbacks.Controls.Add(this.btnEditRollback);
            this.gbRollbacks.Controls.Add(this.btnExecuteRollbacks);
            this.gbRollbacks.Controls.Add(this.dgvRollbacks);
            this.gbRollbacks.Location = new System.Drawing.Point(13, 440);
            this.gbRollbacks.Name = "gbRollbacks";
            this.gbRollbacks.Size = new System.Drawing.Size(874, 269);
            this.gbRollbacks.TabIndex = 1;
            this.gbRollbacks.TabStop = false;
            this.gbRollbacks.Text = "Scenarios";
            // 
            // dgvRollbacks
            // 
            this.dgvRollbacks.AllowUserToAddRows = false;
            this.dgvRollbacks.AllowUserToDeleteRows = false;
            this.dgvRollbacks.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvRollbacks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRollbacks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colArea,
            this.colColor,
            this.colCountry,
            this.colTotalCountries,
            this.colTotalPopulation,
            this.colRollbackType,
            this.colParameters});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRollbacks.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRollbacks.Location = new System.Drawing.Point(17, 55);
            this.dgvRollbacks.Name = "dgvRollbacks";
            this.dgvRollbacks.ReadOnly = true;
            this.dgvRollbacks.Size = new System.Drawing.Size(841, 197);
            this.dgvRollbacks.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(813, 730);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gbMap
            // 
            this.gbMap.Controls.Add(this.mapGBD);
            this.gbMap.Location = new System.Drawing.Point(299, 7);
            this.gbMap.Name = "gbMap";
            this.gbMap.Size = new System.Drawing.Size(588, 420);
            this.gbMap.TabIndex = 4;
            this.gbMap.TabStop = false;
            this.gbMap.Text = "Map";
            // 
            // mapGBD
            // 
            this.mapGBD.AllowDrop = true;
            this.mapGBD.BackColor = System.Drawing.Color.White;
            this.mapGBD.CollectAfterDraw = false;
            this.mapGBD.CollisionDetection = false;
            this.mapGBD.ExtendBuffer = false;
            this.mapGBD.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.mapGBD.IsBusy = false;
            this.mapGBD.IsZoomedToMaxExtent = false;
            this.mapGBD.Legend = null;
            this.mapGBD.Location = new System.Drawing.Point(16, 20);
            this.mapGBD.Name = "mapGBD";
            this.mapGBD.ProgressHandler = null;
            this.mapGBD.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.mapGBD.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.mapGBD.RedrawLayersWhileResizing = false;
            this.mapGBD.SelectionEnabled = true;
            this.mapGBD.Size = new System.Drawing.Size(556, 389);
            this.mapGBD.TabIndex = 0;
            // 
            // gbParameterSelection
            // 
            this.gbParameterSelection.Controls.Add(this.gbOptionsIncremental);
            this.gbParameterSelection.Controls.Add(this.cboRollbackType);
            this.gbParameterSelection.Controls.Add(this.label3);
            this.gbParameterSelection.Controls.Add(this.chartPreview);
            this.gbParameterSelection.Controls.Add(this.btnSaveRollback);
            this.gbParameterSelection.Controls.Add(this.btnBack2);
            this.gbParameterSelection.Location = new System.Drawing.Point(909, 7);
            this.gbParameterSelection.Name = "gbParameterSelection";
            this.gbParameterSelection.Size = new System.Drawing.Size(279, 420);
            this.gbParameterSelection.TabIndex = 3;
            this.gbParameterSelection.TabStop = false;
            this.gbParameterSelection.Text = "Parameter Selection";
            // 
            // gbOptionsIncremental
            // 
            this.gbOptionsIncremental.Controls.Add(this.txtIncrementBackground);
            this.gbOptionsIncremental.Controls.Add(this.txtIncrement);
            this.gbOptionsIncremental.Controls.Add(this.label5);
            this.gbOptionsIncremental.Controls.Add(this.label4);
            this.gbOptionsIncremental.Location = new System.Drawing.Point(15, 48);
            this.gbOptionsIncremental.Name = "gbOptionsIncremental";
            this.gbOptionsIncremental.Size = new System.Drawing.Size(253, 206);
            this.gbOptionsIncremental.TabIndex = 6;
            this.gbOptionsIncremental.TabStop = false;
            this.gbOptionsIncremental.Text = "Options";
            // 
            // txtIncrementBackground
            // 
            this.txtIncrementBackground.Location = new System.Drawing.Point(96, 45);
            this.txtIncrementBackground.Name = "txtIncrementBackground";
            this.txtIncrementBackground.Size = new System.Drawing.Size(100, 20);
            this.txtIncrementBackground.TabIndex = 3;
            // 
            // txtIncrement
            // 
            this.txtIncrement.Location = new System.Drawing.Point(96, 19);
            this.txtIncrement.Name = "txtIncrement";
            this.txtIncrement.Size = new System.Drawing.Size(100, 20);
            this.txtIncrement.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Background:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Increment:";
            // 
            // cboRollbackType
            // 
            this.cboRollbackType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRollbackType.FormattingEnabled = true;
            this.cboRollbackType.Items.AddRange(new object[] {
            "Percentage Rollback",
            "Incremental Rollback",
            "Rollback to a Standard"});
            this.cboRollbackType.Location = new System.Drawing.Point(98, 20);
            this.cboRollbackType.Name = "cboRollbackType";
            this.cboRollbackType.Size = new System.Drawing.Size(175, 21);
            this.cboRollbackType.TabIndex = 5;
            this.cboRollbackType.SelectedIndexChanged += new System.EventHandler(this.cboRollbackType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Rollback Type:";
            // 
            // chartPreview
            // 
            chartArea1.Name = "ChartArea1";
            this.chartPreview.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartPreview.Legends.Add(legend1);
            this.chartPreview.Location = new System.Drawing.Point(12, 261);
            this.chartPreview.Name = "chartPreview";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartPreview.Series.Add(series1);
            this.chartPreview.Size = new System.Drawing.Size(256, 119);
            this.chartPreview.TabIndex = 3;
            this.chartPreview.Text = "chart1";
            // 
            // btnSaveRollback
            // 
            this.btnSaveRollback.Location = new System.Drawing.Point(171, 386);
            this.btnSaveRollback.Name = "btnSaveRollback";
            this.btnSaveRollback.Size = new System.Drawing.Size(97, 23);
            this.btnSaveRollback.TabIndex = 2;
            this.btnSaveRollback.Text = "Save Rollback";
            this.btnSaveRollback.UseVisualStyleBackColor = true;
            this.btnSaveRollback.Click += new System.EventHandler(this.btnSaveRollback_Click);
            // 
            // btnBack2
            // 
            this.btnBack2.Location = new System.Drawing.Point(68, 386);
            this.btnBack2.Name = "btnBack2";
            this.btnBack2.Size = new System.Drawing.Size(97, 23);
            this.btnBack2.TabIndex = 1;
            this.btnBack2.Text = "<- Back";
            this.btnBack2.UseVisualStyleBackColor = true;
            this.btnBack2.Click += new System.EventHandler(this.btnBack2_Click);
            // 
            // gbOptionsPercentage
            // 
            this.gbOptionsPercentage.Controls.Add(this.txtPercentageBackground);
            this.gbOptionsPercentage.Controls.Add(this.txtPercentage);
            this.gbOptionsPercentage.Controls.Add(this.label6);
            this.gbOptionsPercentage.Controls.Add(this.label7);
            this.gbOptionsPercentage.Location = new System.Drawing.Point(924, 440);
            this.gbOptionsPercentage.Name = "gbOptionsPercentage";
            this.gbOptionsPercentage.Size = new System.Drawing.Size(253, 206);
            this.gbOptionsPercentage.TabIndex = 7;
            this.gbOptionsPercentage.TabStop = false;
            this.gbOptionsPercentage.Text = "Options";
            // 
            // txtPercentageBackground
            // 
            this.txtPercentageBackground.Location = new System.Drawing.Point(96, 45);
            this.txtPercentageBackground.Name = "txtPercentageBackground";
            this.txtPercentageBackground.Size = new System.Drawing.Size(100, 20);
            this.txtPercentageBackground.TabIndex = 3;
            // 
            // txtPercentage
            // 
            this.txtPercentage.Location = new System.Drawing.Point(96, 19);
            this.txtPercentage.Name = "txtPercentage";
            this.txtPercentage.Size = new System.Drawing.Size(100, 20);
            this.txtPercentage.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Background:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Percentage:";
            // 
            // gbOptionsStandard
            // 
            this.gbOptionsStandard.Controls.Add(this.cboStandard);
            this.gbOptionsStandard.Controls.Add(this.label9);
            this.gbOptionsStandard.Location = new System.Drawing.Point(929, 669);
            this.gbOptionsStandard.Name = "gbOptionsStandard";
            this.gbOptionsStandard.Size = new System.Drawing.Size(253, 206);
            this.gbOptionsStandard.TabIndex = 8;
            this.gbOptionsStandard.TabStop = false;
            this.gbOptionsStandard.Text = "Options";
            // 
            // cboStandard
            // 
            this.cboStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStandard.FormattingEnabled = true;
            this.cboStandard.Items.AddRange(new object[] {
            "One",
            "Two",
            "Three"});
            this.cboStandard.Location = new System.Drawing.Point(91, 19);
            this.cboStandard.Name = "cboStandard";
            this.cboStandard.Size = new System.Drawing.Size(144, 21);
            this.cboStandard.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Standard:";
            // 
            // gbName
            // 
            this.gbName.Controls.Add(this.txtDescription);
            this.gbName.Controls.Add(this.label10);
            this.gbName.Controls.Add(this.txtName);
            this.gbName.Controls.Add(this.label8);
            this.gbName.Controls.Add(this.btnNext);
            this.gbName.Location = new System.Drawing.Point(12, 7);
            this.gbName.Name = "gbName";
            this.gbName.Size = new System.Drawing.Size(279, 420);
            this.gbName.TabIndex = 3;
            this.gbName.TabStop = false;
            this.gbName.Text = "Create a new scenario";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(96, 76);
            this.txtDescription.MaxLength = 255;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(172, 113);
            this.txtDescription.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(7, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 31);
            this.label10.TabIndex = 4;
            this.label10.Text = "Scenario Description:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(96, 35);
            this.txtName.MaxLength = 15;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(172, 20);
            this.txtName.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Scenario Name:";
            this.toolTipScenarioName.SetToolTip(this.label8, "The name of the scenario will also be used in the rollback report filename.  It i" +
                    "s limited to 15 characters.");
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(171, 386);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(97, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next ->";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnExecuteRollbacks
            // 
            this.btnExecuteRollbacks.Location = new System.Drawing.Point(736, 20);
            this.btnExecuteRollbacks.Name = "btnExecuteRollbacks";
            this.btnExecuteRollbacks.Size = new System.Drawing.Size(122, 23);
            this.btnExecuteRollbacks.TabIndex = 3;
            this.btnExecuteRollbacks.Text = "Execute Scenarios";
            this.btnExecuteRollbacks.UseVisualStyleBackColor = true;
            // 
            // btnEditRollback
            // 
            this.btnEditRollback.Location = new System.Drawing.Point(17, 20);
            this.btnEditRollback.Name = "btnEditRollback";
            this.btnEditRollback.Size = new System.Drawing.Size(86, 23);
            this.btnEditRollback.TabIndex = 4;
            this.btnEditRollback.Text = "Edit Scenario";
            this.btnEditRollback.UseVisualStyleBackColor = true;
            // 
            // btnDeleteRollback
            // 
            this.btnDeleteRollback.Location = new System.Drawing.Point(109, 20);
            this.btnDeleteRollback.Name = "btnDeleteRollback";
            this.btnDeleteRollback.Size = new System.Drawing.Size(99, 23);
            this.btnDeleteRollback.TabIndex = 5;
            this.btnDeleteRollback.Text = "Delete Scenario";
            this.btnDeleteRollback.UseVisualStyleBackColor = true;
            // 
            // colArea
            // 
            this.colArea.HeaderText = "Name";
            this.colArea.Name = "colArea";
            this.colArea.ReadOnly = true;
            // 
            // colColor
            // 
            this.colColor.HeaderText = "Color";
            this.colColor.Name = "colColor";
            this.colColor.ReadOnly = true;
            this.colColor.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCountry
            // 
            this.colCountry.HeaderText = "Countries";
            this.colCountry.Name = "colCountry";
            this.colCountry.ReadOnly = true;
            // 
            // colTotalCountries
            // 
            this.colTotalCountries.HeaderText = "Total Countries";
            this.colTotalCountries.Name = "colTotalCountries";
            this.colTotalCountries.ReadOnly = true;
            // 
            // colTotalPopulation
            // 
            this.colTotalPopulation.HeaderText = "Total Population";
            this.colTotalPopulation.Name = "colTotalPopulation";
            this.colTotalPopulation.ReadOnly = true;
            // 
            // colRollbackType
            // 
            this.colRollbackType.HeaderText = "Rollback Type";
            this.colRollbackType.Name = "colRollbackType";
            this.colRollbackType.ReadOnly = true;
            // 
            // colParameters
            // 
            this.colParameters.HeaderText = "Parameters";
            this.colParameters.Name = "colParameters";
            this.colParameters.ReadOnly = true;
            // 
            // GBDRollback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1567, 766);
            this.Controls.Add(this.gbName);
            this.Controls.Add(this.gbOptionsStandard);
            this.Controls.Add(this.gbOptionsPercentage);
            this.Controls.Add(this.gbParameterSelection);
            this.Controls.Add(this.gbMap);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gbRollbacks);
            this.Controls.Add(this.gbCountrySelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "GBDRollback";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GBD Rollback Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GBDRollback_FormClosing);
            this.gbCountrySelection.ResumeLayout(false);
            this.gbCountrySelection.PerformLayout();
            this.gbRollbacks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRollbacks)).EndInit();
            this.gbMap.ResumeLayout(false);
            this.gbParameterSelection.ResumeLayout(false);
            this.gbParameterSelection.PerformLayout();
            this.gbOptionsIncremental.ResumeLayout(false);
            this.gbOptionsIncremental.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPreview)).EndInit();
            this.gbOptionsPercentage.ResumeLayout(false);
            this.gbOptionsPercentage.PerformLayout();
            this.gbOptionsStandard.ResumeLayout(false);
            this.gbOptionsStandard.PerformLayout();
            this.gbName.ResumeLayout(false);
            this.gbName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCountrySelection;
        private System.Windows.Forms.GroupBox gbRollbacks;
        private System.Windows.Forms.DataGridView dgvRollbacks;
        private System.Windows.Forms.TreeView tvCountries;
        private System.Windows.Forms.Button btnNext2;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbMap;
        private DotSpatial.Controls.Map mapGBD;
        private System.Windows.Forms.GroupBox gbParameterSelection;
        private System.Windows.Forms.Button btnBack2;
        private System.Windows.Forms.Button btnSaveRollback;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPreview;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboRollbackType;
        private System.Windows.Forms.GroupBox gbOptionsIncremental;
        private System.Windows.Forms.TextBox txtIncrementBackground;
        private System.Windows.Forms.TextBox txtIncrement;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbOptionsPercentage;
        private System.Windows.Forms.TextBox txtPercentageBackground;
        private System.Windows.Forms.TextBox txtPercentage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbOptionsStandard;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboStandard;
        private System.Windows.Forms.GroupBox gbName;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolTip toolTipScenarioName;
        private System.Windows.Forms.Button btnDeleteRollback;
        private System.Windows.Forms.Button btnEditRollback;
        private System.Windows.Forms.Button btnExecuteRollbacks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCountry;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalCountries;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalPopulation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRollbackType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colParameters;
    }
}