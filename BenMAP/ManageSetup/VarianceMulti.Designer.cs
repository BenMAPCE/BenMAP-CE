namespace BenMAP
{
    partial class VarianceMulti
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblEnd = new System.Windows.Forms.Label();
            this.tbEnd = new System.Windows.Forms.TextBox();
            this.lblStart = new System.Windows.Forms.Label();
            this.tbStart = new System.Windows.Forms.TextBox();
            this.lblSeason = new System.Windows.Forms.Label();
            this.tbSeason = new System.Windows.Forms.TextBox();
            this.tbPollutant = new System.Windows.Forms.TextBox();
            this.lblPollutant = new System.Windows.Forms.Label();
            this.tbModelSpec = new System.Windows.Forms.TextBox();
            this.lblModSpec = new System.Windows.Forms.Label();
            this.olvVariance = new BrightIdeasSoftware.DataListView();
            this.olvPollutant = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvVarCov = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvVariance)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 470);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(405, 61);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOK.Location = new System.Drawing.Point(297, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCancel.Location = new System.Drawing.Point(216, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(405, 470);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(405, 470);
            this.panel2.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblEnd);
            this.panel3.Controls.Add(this.tbEnd);
            this.panel3.Controls.Add(this.lblStart);
            this.panel3.Controls.Add(this.tbStart);
            this.panel3.Controls.Add(this.lblSeason);
            this.panel3.Controls.Add(this.tbSeason);
            this.panel3.Controls.Add(this.tbPollutant);
            this.panel3.Controls.Add(this.lblPollutant);
            this.panel3.Controls.Add(this.tbModelSpec);
            this.panel3.Controls.Add(this.lblModSpec);
            this.panel3.Controls.Add(this.olvVariance);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(405, 470);
            this.panel3.TabIndex = 9;
            // 
            // lblEnd
            // 
            this.lblEnd.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(289, 67);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(59, 14);
            this.lblEnd.TabIndex = 54;
            this.lblEnd.Text = "End Date:";
            // 
            // tbEnd
            // 
            this.tbEnd.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbEnd.Enabled = false;
            this.tbEnd.Location = new System.Drawing.Point(291, 82);
            this.tbEnd.Name = "tbEnd";
            this.tbEnd.Size = new System.Drawing.Size(74, 22);
            this.tbEnd.TabIndex = 55;
            // 
            // lblStart
            // 
            this.lblStart.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(199, 67);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(64, 14);
            this.lblStart.TabIndex = 52;
            this.lblStart.Text = "Start Date:";
            // 
            // tbStart
            // 
            this.tbStart.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbStart.BackColor = System.Drawing.SystemColors.Window;
            this.tbStart.Enabled = false;
            this.tbStart.Location = new System.Drawing.Point(202, 82);
            this.tbStart.Name = "tbStart";
            this.tbStart.ReadOnly = true;
            this.tbStart.Size = new System.Drawing.Size(74, 22);
            this.tbStart.TabIndex = 53;
            // 
            // lblSeason
            // 
            this.lblSeason.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblSeason.AutoSize = true;
            this.lblSeason.Location = new System.Drawing.Point(38, 67);
            this.lblSeason.Name = "lblSeason";
            this.lblSeason.Size = new System.Drawing.Size(50, 14);
            this.lblSeason.TabIndex = 50;
            this.lblSeason.Text = "Season:";
            // 
            // tbSeason
            // 
            this.tbSeason.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbSeason.BackColor = System.Drawing.SystemColors.Window;
            this.tbSeason.Enabled = false;
            this.tbSeason.Location = new System.Drawing.Point(39, 82);
            this.tbSeason.Name = "tbSeason";
            this.tbSeason.ReadOnly = true;
            this.tbSeason.Size = new System.Drawing.Size(148, 22);
            this.tbSeason.TabIndex = 51;
            // 
            // tbPollutant
            // 
            this.tbPollutant.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbPollutant.BackColor = System.Drawing.SystemColors.Window;
            this.tbPollutant.Enabled = false;
            this.tbPollutant.Location = new System.Drawing.Point(132, 37);
            this.tbPollutant.MaximumSize = new System.Drawing.Size(282, 22);
            this.tbPollutant.Name = "tbPollutant";
            this.tbPollutant.ReadOnly = true;
            this.tbPollutant.Size = new System.Drawing.Size(264, 22);
            this.tbPollutant.TabIndex = 34;
            // 
            // lblPollutant
            // 
            this.lblPollutant.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblPollutant.AutoSize = true;
            this.lblPollutant.Location = new System.Drawing.Point(9, 41);
            this.lblPollutant.Name = "lblPollutant";
            this.lblPollutant.Size = new System.Drawing.Size(60, 14);
            this.lblPollutant.TabIndex = 35;
            this.lblPollutant.Text = "Pollutant:";
            // 
            // tbModelSpec
            // 
            this.tbModelSpec.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbModelSpec.BackColor = System.Drawing.SystemColors.Window;
            this.tbModelSpec.Enabled = false;
            this.tbModelSpec.Location = new System.Drawing.Point(132, 9);
            this.tbModelSpec.Name = "tbModelSpec";
            this.tbModelSpec.ReadOnly = true;
            this.tbModelSpec.Size = new System.Drawing.Size(264, 22);
            this.tbModelSpec.TabIndex = 32;
            // 
            // lblModSpec
            // 
            this.lblModSpec.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblModSpec.AutoSize = true;
            this.lblModSpec.Location = new System.Drawing.Point(9, 13);
            this.lblModSpec.Name = "lblModSpec";
            this.lblModSpec.Size = new System.Drawing.Size(116, 14);
            this.lblModSpec.TabIndex = 33;
            this.lblModSpec.Text = "Model Specification:";
            // 
            // olvVariance
            // 
            this.olvVariance.AllColumns.Add(this.olvPollutant);
            this.olvVariance.AllColumns.Add(this.olvVarCov);
            this.olvVariance.AllowColumnReorder = true;
            this.olvVariance.AllowDrop = true;
            this.olvVariance.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvVariance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvVariance.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvVariance.CellEditEnterChangesRows = true;
            this.olvVariance.CellEditTabChangesRows = true;
            this.olvVariance.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvPollutant,
            this.olvVarCov});
            this.olvVariance.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvVariance.DataSource = null;
            this.olvVariance.EmptyListMsg = "";
            this.olvVariance.EmptyListMsgFont = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvVariance.FullRowSelect = true;
            this.olvVariance.GridLines = true;
            this.olvVariance.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvVariance.GroupWithItemCountSingularFormat = "{0} (1 item)";
            this.olvVariance.HeaderUsesThemes = false;
            this.olvVariance.HideSelection = false;
            this.olvVariance.Location = new System.Drawing.Point(82, 114);
            this.olvVariance.Name = "olvVariance";
            this.olvVariance.OwnerDraw = true;
            this.olvVariance.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvVariance.ShowCommandMenuOnRightClick = true;
            this.olvVariance.ShowGroups = false;
            this.olvVariance.ShowImagesOnSubItems = true;
            this.olvVariance.ShowItemCountOnGroups = true;
            this.olvVariance.ShowItemToolTips = true;
            this.olvVariance.Size = new System.Drawing.Size(241, 354);
            this.olvVariance.TabIndex = 57;
            this.olvVariance.UseAlternatingBackColors = true;
            this.olvVariance.UseCellFormatEvents = true;
            this.olvVariance.UseCompatibleStateImageBehavior = false;
            this.olvVariance.UseFiltering = true;
            this.olvVariance.UseHotItem = true;
            this.olvVariance.UseTranslucentHotItem = true;
            this.olvVariance.View = System.Windows.Forms.View.Details;
            // 
            // olvPollutant
            // 
            this.olvPollutant.AspectName = "InteractionPollutant";
            this.olvPollutant.IsEditable = false;
            this.olvPollutant.Text = "Pollutant";
            this.olvPollutant.Width = 119;
            // 
            // olvVarCov
            // 
            this.olvVarCov.AspectName = "VarCov";
            this.olvVarCov.AspectToStringFormat = "{0:0.##E+00}";
            this.olvVarCov.FillsFreeSpace = true;
            this.olvVarCov.Text = "VarCov";
            this.olvVarCov.Width = 118;
            // 
            // VarianceMulti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 531);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Name = "VarianceMulti";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Variance/ Covariance";
            this.Load += new System.EventHandler(this.VarianceMulti_Load);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvVariance)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox tbPollutant;
        private System.Windows.Forms.Label lblPollutant;
        private System.Windows.Forms.TextBox tbModelSpec;
        private System.Windows.Forms.Label lblModSpec;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.TextBox tbEnd;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.TextBox tbStart;
        private System.Windows.Forms.Label lblSeason;
        private System.Windows.Forms.TextBox tbSeason;
        private BrightIdeasSoftware.DataListView olvVariance;
        private BrightIdeasSoftware.OLVColumn olvPollutant;
        private BrightIdeasSoftware.OLVColumn olvVarCov;
    }
}