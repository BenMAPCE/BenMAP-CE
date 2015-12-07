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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tbPollutant = new System.Windows.Forms.TextBox();
            this.lblPollutant = new System.Windows.Forms.Label();
            this.tbModelSpec = new System.Windows.Forms.TextBox();
            this.lblModSpec = new System.Windows.Forms.Label();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 482);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(357, 61);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(273, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(192, 23);
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
            this.panel1.Size = new System.Drawing.Size(357, 482);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(357, 482);
            this.panel2.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.objectListView1);
            this.panel3.Controls.Add(this.lblEnd);
            this.panel3.Controls.Add(this.tbEnd);
            this.panel3.Controls.Add(this.lblStart);
            this.panel3.Controls.Add(this.tbStart);
            this.panel3.Controls.Add(this.lblSeason);
            this.panel3.Controls.Add(this.textBox1);
            this.panel3.Controls.Add(this.tbPollutant);
            this.panel3.Controls.Add(this.lblPollutant);
            this.panel3.Controls.Add(this.tbModelSpec);
            this.panel3.Controls.Add(this.lblModSpec);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(357, 482);
            this.panel3.TabIndex = 9;
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(271, 67);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(59, 14);
            this.lblEnd.TabIndex = 54;
            this.lblEnd.Text = "End Date:";
            // 
            // tbEnd
            // 
            this.tbEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEnd.Location = new System.Drawing.Point(273, 82);
            this.tbEnd.Name = "tbEnd";
            this.tbEnd.Size = new System.Drawing.Size(74, 22);
            this.tbEnd.TabIndex = 55;
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(176, 67);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(64, 14);
            this.lblStart.TabIndex = 52;
            this.lblStart.Text = "Start Date:";
            // 
            // tbStart
            // 
            this.tbStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStart.BackColor = System.Drawing.SystemColors.Window;
            this.tbStart.Location = new System.Drawing.Point(179, 82);
            this.tbStart.Name = "tbStart";
            this.tbStart.ReadOnly = true;
            this.tbStart.Size = new System.Drawing.Size(74, 22);
            this.tbStart.TabIndex = 53;
            // 
            // lblSeason
            // 
            this.lblSeason.AutoSize = true;
            this.lblSeason.Location = new System.Drawing.Point(10, 67);
            this.lblSeason.Name = "lblSeason";
            this.lblSeason.Size = new System.Drawing.Size(50, 14);
            this.lblSeason.TabIndex = 50;
            this.lblSeason.Text = "Season:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(10, 82);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(148, 22);
            this.textBox1.TabIndex = 51;
            // 
            // tbPollutant
            // 
            this.tbPollutant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPollutant.BackColor = System.Drawing.SystemColors.Window;
            this.tbPollutant.Location = new System.Drawing.Point(133, 37);
            this.tbPollutant.MaximumSize = new System.Drawing.Size(282, 22);
            this.tbPollutant.Name = "tbPollutant";
            this.tbPollutant.ReadOnly = true;
            this.tbPollutant.Size = new System.Drawing.Size(214, 22);
            this.tbPollutant.TabIndex = 34;
            // 
            // lblPollutant
            // 
            this.lblPollutant.AutoSize = true;
            this.lblPollutant.Location = new System.Drawing.Point(10, 41);
            this.lblPollutant.Name = "lblPollutant";
            this.lblPollutant.Size = new System.Drawing.Size(60, 14);
            this.lblPollutant.TabIndex = 35;
            this.lblPollutant.Text = "Pollutant:";
            // 
            // tbModelSpec
            // 
            this.tbModelSpec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbModelSpec.BackColor = System.Drawing.SystemColors.Window;
            this.tbModelSpec.Location = new System.Drawing.Point(133, 9);
            this.tbModelSpec.MaximumSize = new System.Drawing.Size(282, 22);
            this.tbModelSpec.Name = "tbModelSpec";
            this.tbModelSpec.ReadOnly = true;
            this.tbModelSpec.Size = new System.Drawing.Size(214, 22);
            this.tbModelSpec.TabIndex = 32;
            // 
            // lblModSpec
            // 
            this.lblModSpec.AutoSize = true;
            this.lblModSpec.Location = new System.Drawing.Point(10, 13);
            this.lblModSpec.Name = "lblModSpec";
            this.lblModSpec.Size = new System.Drawing.Size(116, 14);
            this.lblModSpec.TabIndex = 33;
            this.lblModSpec.Text = "Model Specification:";
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumn3);
            this.objectListView1.AllColumns.Add(this.olvColumn4);
            this.objectListView1.AllowColumnReorder = true;
            this.objectListView1.AllowDrop = true;
            this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView1.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.objectListView1.CheckBoxes = true;
            this.objectListView1.CheckedAspectName = "";
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn3,
            this.olvColumn4});
            this.objectListView1.CopySelectionOnControlC = false;
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.EmptyListMsg = "Pollutant Variance List";
            this.objectListView1.EmptyListMsgFont = new System.Drawing.Font("Calibri", 14.25F);
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.objectListView1.HeaderUsesThemes = false;
            this.objectListView1.HideSelection = false;
            this.objectListView1.Location = new System.Drawing.Point(63, 110);
            this.objectListView1.MultiSelect = false;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.objectListView1.OverlayText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.objectListView1.OverlayText.BorderWidth = 2F;
            this.objectListView1.OverlayText.Rotation = -20;
            this.objectListView1.OverlayText.Text = "";
            this.objectListView1.OwnerDraw = true;
            this.objectListView1.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.objectListView1.ShowCommandMenuOnRightClick = true;
            this.objectListView1.ShowGroups = false;
            this.objectListView1.ShowImagesOnSubItems = true;
            this.objectListView1.ShowItemCountOnGroups = true;
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.Size = new System.Drawing.Size(230, 366);
            this.objectListView1.SpaceBetweenGroups = 20;
            this.objectListView1.TabIndex = 57;
            this.objectListView1.UseAlternatingBackColors = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseFiltering = true;
            this.objectListView1.UseHotItem = true;
            this.objectListView1.UseHyperlinks = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "RegionName";
            this.olvColumn3.IsEditable = false;
            this.olvColumn3.Text = "Pollutant";
            this.olvColumn3.Width = 114;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "Col";
            this.olvColumn4.FillsFreeSpace = true;
            this.olvColumn4.IsEditable = false;
            this.olvColumn4.Text = "VarCov";
            this.olvColumn4.Width = 114;
            // 
            // VarianceMulti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 543);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Name = "VarianceMulti";
            this.Text = "Variance/ Covariance";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.VarianceMulti_Load);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
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
        private System.Windows.Forms.TextBox textBox1;
        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
    }
}