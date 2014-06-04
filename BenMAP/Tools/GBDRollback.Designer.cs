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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.gbAreaSelection = new System.Windows.Forms.GroupBox();
            this.gbRollbacks = new System.Windows.Forms.GroupBox();
            this.dgvRollbacks = new System.Windows.Forms.DataGridView();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewImageColumn();
            this.colRegion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCountry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOperation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colParameters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTotalCountries = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotalPopulation = new System.Windows.Forms.Label();
            this.tvCountries = new System.Windows.Forms.TreeView();
            this.btnSelectAndContinue = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.btnExecuteRollbacks = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.mapGBD = new DotSpatial.Controls.Map();
            this.gbParameterSelection = new System.Windows.Forms.GroupBox();
            this.btnAreaSelection = new System.Windows.Forms.Button();
            this.btnAddParameters = new System.Windows.Forms.Button();
            this.chartPreview = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label3 = new System.Windows.Forms.Label();
            this.cboRollbackType = new System.Windows.Forms.ComboBox();
            this.gbOptionsIncremental = new System.Windows.Forms.GroupBox();
            this.gbAreaSelection.SuspendLayout();
            this.gbRollbacks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRollbacks)).BeginInit();
            this.gbMap.SuspendLayout();
            this.gbParameterSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // gbAreaSelection
            // 
            this.gbAreaSelection.Controls.Add(this.txtInfo);
            this.gbAreaSelection.Controls.Add(this.btnSelectAndContinue);
            this.gbAreaSelection.Controls.Add(this.tvCountries);
            this.gbAreaSelection.Location = new System.Drawing.Point(13, 13);
            this.gbAreaSelection.Name = "gbAreaSelection";
            this.gbAreaSelection.Size = new System.Drawing.Size(279, 420);
            this.gbAreaSelection.TabIndex = 0;
            this.gbAreaSelection.TabStop = false;
            this.gbAreaSelection.Text = "Area Selection";
            // 
            // gbRollbacks
            // 
            this.gbRollbacks.Controls.Add(this.lblTotalPopulation);
            this.gbRollbacks.Controls.Add(this.label2);
            this.gbRollbacks.Controls.Add(this.lblTotalCountries);
            this.gbRollbacks.Controls.Add(this.label1);
            this.gbRollbacks.Controls.Add(this.dgvRollbacks);
            this.gbRollbacks.Location = new System.Drawing.Point(13, 446);
            this.gbRollbacks.Name = "gbRollbacks";
            this.gbRollbacks.Size = new System.Drawing.Size(874, 387);
            this.gbRollbacks.TabIndex = 1;
            this.gbRollbacks.TabStop = false;
            this.gbRollbacks.Text = "Rollbacks";
            // 
            // dgvRollbacks
            // 
            this.dgvRollbacks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRollbacks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAction,
            this.colColor,
            this.colRegion,
            this.colCountry,
            this.colOperation,
            this.colParameters});
            this.dgvRollbacks.Location = new System.Drawing.Point(17, 64);
            this.dgvRollbacks.Name = "dgvRollbacks";
            this.dgvRollbacks.Size = new System.Drawing.Size(841, 308);
            this.dgvRollbacks.TabIndex = 0;
            // 
            // colAction
            // 
            this.colAction.HeaderText = "Action";
            this.colAction.Name = "colAction";
            // 
            // colColor
            // 
            this.colColor.HeaderText = "Color";
            this.colColor.Name = "colColor";
            // 
            // colRegion
            // 
            this.colRegion.HeaderText = "Region";
            this.colRegion.Name = "colRegion";
            // 
            // colCountry
            // 
            this.colCountry.HeaderText = "Country";
            this.colCountry.Name = "colCountry";
            // 
            // colOperation
            // 
            this.colOperation.HeaderText = "Operation";
            this.colOperation.Name = "colOperation";
            // 
            // colParameters
            // 
            this.colParameters.HeaderText = "Parameters";
            this.colParameters.Name = "colParameters";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Total Countries:";
            // 
            // lblTotalCountries
            // 
            this.lblTotalCountries.AutoSize = true;
            this.lblTotalCountries.Location = new System.Drawing.Point(105, 32);
            this.lblTotalCountries.Name = "lblTotalCountries";
            this.lblTotalCountries.Size = new System.Drawing.Size(13, 13);
            this.lblTotalCountries.TabIndex = 2;
            this.lblTotalCountries.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Total Population:";
            // 
            // lblTotalPopulation
            // 
            this.lblTotalPopulation.AutoSize = true;
            this.lblTotalPopulation.Location = new System.Drawing.Point(266, 32);
            this.lblTotalPopulation.Name = "lblTotalPopulation";
            this.lblTotalPopulation.Size = new System.Drawing.Size(13, 13);
            this.lblTotalPopulation.TabIndex = 4;
            this.lblTotalPopulation.Text = "0";
            // 
            // tvCountries
            // 
            this.tvCountries.Location = new System.Drawing.Point(10, 20);
            this.tvCountries.Name = "tvCountries";
            this.tvCountries.Size = new System.Drawing.Size(258, 234);
            this.tvCountries.TabIndex = 0;
            // 
            // btnSelectAndContinue
            // 
            this.btnSelectAndContinue.Location = new System.Drawing.Point(121, 386);
            this.btnSelectAndContinue.Name = "btnSelectAndContinue";
            this.btnSelectAndContinue.Size = new System.Drawing.Size(144, 23);
            this.btnSelectAndContinue.TabIndex = 1;
            this.btnSelectAndContinue.Text = "Select And Continue ->";
            this.btnSelectAndContinue.UseVisualStyleBackColor = true;
            this.btnSelectAndContinue.Click += new System.EventHandler(this.btnSelectAndContinue_Click);
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
            // btnExecuteRollbacks
            // 
            this.btnExecuteRollbacks.Location = new System.Drawing.Point(674, 839);
            this.btnExecuteRollbacks.Name = "btnExecuteRollbacks";
            this.btnExecuteRollbacks.Size = new System.Drawing.Size(122, 23);
            this.btnExecuteRollbacks.TabIndex = 2;
            this.btnExecuteRollbacks.Text = "Execute Rollbacks";
            this.btnExecuteRollbacks.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(813, 839);
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
            this.gbMap.Location = new System.Drawing.Point(299, 13);
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
            this.gbParameterSelection.Controls.Add(this.btnAddParameters);
            this.gbParameterSelection.Controls.Add(this.btnAreaSelection);
            this.gbParameterSelection.Location = new System.Drawing.Point(909, 13);
            this.gbParameterSelection.Name = "gbParameterSelection";
            this.gbParameterSelection.Size = new System.Drawing.Size(279, 420);
            this.gbParameterSelection.TabIndex = 3;
            this.gbParameterSelection.TabStop = false;
            this.gbParameterSelection.Text = "Parameter Selection";
            // 
            // btnAreaSelection
            // 
            this.btnAreaSelection.Location = new System.Drawing.Point(7, 386);
            this.btnAreaSelection.Name = "btnAreaSelection";
            this.btnAreaSelection.Size = new System.Drawing.Size(144, 23);
            this.btnAreaSelection.TabIndex = 1;
            this.btnAreaSelection.Text = "<- Area Selection";
            this.btnAreaSelection.UseVisualStyleBackColor = true;
            this.btnAreaSelection.Click += new System.EventHandler(this.btnAreaSelection_Click);
            // 
            // btnAddParameters
            // 
            this.btnAddParameters.Location = new System.Drawing.Point(157, 386);
            this.btnAddParameters.Name = "btnAddParameters";
            this.btnAddParameters.Size = new System.Drawing.Size(116, 23);
            this.btnAddParameters.TabIndex = 2;
            this.btnAddParameters.Text = "Add Parameters";
            this.btnAddParameters.UseVisualStyleBackColor = true;
            // 
            // chartPreview
            // 
            chartArea5.Name = "ChartArea1";
            this.chartPreview.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.chartPreview.Legends.Add(legend5);
            this.chartPreview.Location = new System.Drawing.Point(12, 261);
            this.chartPreview.Name = "chartPreview";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chartPreview.Series.Add(series5);
            this.chartPreview.Size = new System.Drawing.Size(256, 119);
            this.chartPreview.TabIndex = 3;
            this.chartPreview.Text = "chart1";
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
            // cboRollbackType
            // 
            this.cboRollbackType.FormattingEnabled = true;
            this.cboRollbackType.Items.AddRange(new object[] {
            "Incremental",
            "Percentage",
            "To Standard"});
            this.cboRollbackType.Location = new System.Drawing.Point(98, 20);
            this.cboRollbackType.Name = "cboRollbackType";
            this.cboRollbackType.Size = new System.Drawing.Size(175, 21);
            this.cboRollbackType.TabIndex = 5;
            // 
            // gbOptionsIncremental
            // 
            this.gbOptionsIncremental.Location = new System.Drawing.Point(15, 48);
            this.gbOptionsIncremental.Name = "gbOptionsIncremental";
            this.gbOptionsIncremental.Size = new System.Drawing.Size(253, 206);
            this.gbOptionsIncremental.TabIndex = 6;
            this.gbOptionsIncremental.TabStop = false;
            this.gbOptionsIncremental.Text = "Options";
            // 
            // GBDRollback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 874);
            this.Controls.Add(this.gbParameterSelection);
            this.Controls.Add(this.gbMap);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExecuteRollbacks);
            this.Controls.Add(this.gbRollbacks);
            this.Controls.Add(this.gbAreaSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GBDRollback";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GBD Rollback Tool";
            this.gbAreaSelection.ResumeLayout(false);
            this.gbAreaSelection.PerformLayout();
            this.gbRollbacks.ResumeLayout(false);
            this.gbRollbacks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRollbacks)).EndInit();
            this.gbMap.ResumeLayout(false);
            this.gbParameterSelection.ResumeLayout(false);
            this.gbParameterSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbAreaSelection;
        private System.Windows.Forms.GroupBox gbRollbacks;
        private System.Windows.Forms.DataGridView dgvRollbacks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.DataGridViewImageColumn colColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRegion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCountry;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOperation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colParameters;
        private System.Windows.Forms.Label lblTotalPopulation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalCountries;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView tvCountries;
        private System.Windows.Forms.Button btnSelectAndContinue;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Button btnExecuteRollbacks;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbMap;
        private DotSpatial.Controls.Map mapGBD;
        private System.Windows.Forms.GroupBox gbParameterSelection;
        private System.Windows.Forms.Button btnAreaSelection;
        private System.Windows.Forms.Button btnAddParameters;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPreview;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboRollbackType;
        private System.Windows.Forms.GroupBox gbOptionsIncremental;
    }
}