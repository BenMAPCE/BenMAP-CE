namespace BenMAP
{
    partial class GridCreationMethods
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
            this.components = new System.ComponentModel.Container();
            this.rbtnModelData = new System.Windows.Forms.RadioButton();
            this.rbtnMonitorData = new System.Windows.Forms.RadioButton();
            this.rbtnMonitorRollback = new System.Windows.Forms.RadioButton();
            this.grp = new System.Windows.Forms.GroupBox();
            this.picGTHelp = new System.Windows.Forms.PictureBox();
            this.cboGrid = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtExistingAQG = new System.Windows.Forms.TextBox();
            this.rbtnOpenFile = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSaveNewFormat = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGTHelp)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbtnModelData
            // 
            this.rbtnModelData.AutoSize = true;
            this.rbtnModelData.Location = new System.Drawing.Point(28, 55);
            this.rbtnModelData.Name = "rbtnModelData";
            this.rbtnModelData.Size = new System.Drawing.Size(89, 18);
            this.rbtnModelData.TabIndex = 3;
            this.rbtnModelData.TabStop = true;
            this.rbtnModelData.Tag = "model";
            this.rbtnModelData.Text = "Model Data";
            this.toolTip1.SetToolTip(this.rbtnModelData, "Missing daily model data will be estimated by averaging available data for that season. ");
            this.rbtnModelData.UseVisualStyleBackColor = true;
            this.rbtnModelData.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // rbtnMonitorData
            // 
            this.rbtnMonitorData.AutoSize = true;
            this.rbtnMonitorData.Location = new System.Drawing.Point(28, 97);
            this.rbtnMonitorData.Name = "rbtnMonitorData";
            this.rbtnMonitorData.Size = new System.Drawing.Size(97, 18);
            this.rbtnMonitorData.TabIndex = 4;
            this.rbtnMonitorData.TabStop = true;
            this.rbtnMonitorData.Tag = "monitor";
            this.rbtnMonitorData.Text = "Monitor Data";
            this.toolTip1.SetToolTip(this.rbtnMonitorData, "Missing daily monitor observations will be estimated by averaging available data for that season. ");
            this.rbtnMonitorData.UseVisualStyleBackColor = true;
            this.rbtnMonitorData.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // rbtnMonitorRollback
            // 
            this.rbtnMonitorRollback.AutoSize = true;
            this.rbtnMonitorRollback.Location = new System.Drawing.Point(28, 136);
            this.rbtnMonitorRollback.Name = "rbtnMonitorRollback";
            this.rbtnMonitorRollback.Size = new System.Drawing.Size(118, 18);
            this.rbtnMonitorRollback.TabIndex = 5;
            this.rbtnMonitorRollback.TabStop = true;
            this.rbtnMonitorRollback.Tag = "monitorrollback";
            this.rbtnMonitorRollback.Text = "Monitor Rollback";
            this.toolTip1.SetToolTip(this.rbtnMonitorRollback, "Missing daily monitor observations will be estimated by averaging available data for that season. ");
            this.rbtnMonitorRollback.UseVisualStyleBackColor = true;
            this.rbtnMonitorRollback.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // grp
            // 
            this.grp.Controls.Add(this.picGTHelp);
            this.grp.Controls.Add(this.cboGrid);
            this.grp.Controls.Add(this.label1);
            this.grp.Controls.Add(this.btnBrowse);
            this.grp.Controls.Add(this.txtExistingAQG);
            this.grp.Controls.Add(this.rbtnOpenFile);
            this.grp.Controls.Add(this.rbtnModelData);
            this.grp.Controls.Add(this.rbtnMonitorRollback);
            this.grp.Controls.Add(this.rbtnMonitorData);
            this.grp.Location = new System.Drawing.Point(3, 3);
            this.grp.Name = "grp";
            this.grp.Size = new System.Drawing.Size(385, 243);
            this.grp.TabIndex = 6;
            this.grp.TabStop = false;
            // 
            // picGTHelp
            // 
            this.picGTHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picGTHelp.BackgroundImage = global::BenMAP.Properties.Resources.help_16x16;
            this.picGTHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picGTHelp.Location = new System.Drawing.Point(348, 49);
            this.picGTHelp.Name = "picGTHelp";
            this.picGTHelp.Size = new System.Drawing.Size(20, 19);
            this.picGTHelp.TabIndex = 9;
            this.picGTHelp.TabStop = false;
            this.picGTHelp.Tag = "";
            this.picGTHelp.Click += new System.EventHandler(this.picGTHelp_Click);
            this.picGTHelp.MouseHover += new System.EventHandler(this.picGTHelp_MouseHover);
            // 
            // cboGrid
            // 
            this.cboGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGrid.FormattingEnabled = true;
            this.cboGrid.Location = new System.Drawing.Point(137, 21);
            this.cboGrid.Name = "cboGrid";
            this.cboGrid.Size = new System.Drawing.Size(231, 22);
            this.cboGrid.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(25, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 14);
            this.label1.TabIndex = 16;
            this.label1.Text = "Grid Type:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowse.Location = new System.Drawing.Point(307, 210);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(62, 27);
            this.btnBrowse.TabIndex = 9;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtExistingAQG
            // 
            this.txtExistingAQG.Enabled = false;
            this.txtExistingAQG.Location = new System.Drawing.Point(28, 213);
            this.txtExistingAQG.Name = "txtExistingAQG";
            this.txtExistingAQG.ReadOnly = true;
            this.txtExistingAQG.Size = new System.Drawing.Size(273, 22);
            this.txtExistingAQG.TabIndex = 8;
            // 
            // rbtnOpenFile
            // 
            this.rbtnOpenFile.AutoSize = true;
            this.rbtnOpenFile.Location = new System.Drawing.Point(28, 175);
            this.rbtnOpenFile.Name = "rbtnOpenFile";
            this.rbtnOpenFile.Size = new System.Drawing.Size(110, 18);
            this.rbtnOpenFile.TabIndex = 6;
            this.rbtnOpenFile.TabStop = true;
            this.rbtnOpenFile.Tag = "openfile";
            this.rbtnOpenFile.Text = "open *.aqgx file";
            this.rbtnOpenFile.UseVisualStyleBackColor = true;
            this.rbtnOpenFile.CheckedChanged += new System.EventHandler(this.rbtnOpenFile_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSaveNewFormat);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnNext);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(3, 252);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(385, 65);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // btnSaveNewFormat
            // 
            this.btnSaveNewFormat.Enabled = false;
            this.btnSaveNewFormat.Location = new System.Drawing.Point(119, 23);
            this.btnSaveNewFormat.Name = "btnSaveNewFormat";
            this.btnSaveNewFormat.Size = new System.Drawing.Size(115, 27);
            this.btnSaveNewFormat.TabIndex = 4;
            this.btnSaveNewFormat.Text = "Save NewFormat";
            this.btnSaveNewFormat.UseVisualStyleBackColor = true;
            this.btnSaveNewFormat.Click += new System.EventHandler(this.SaveNewFormat_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(6, 23);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(107, 27);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save As(*.aqgx)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(307, 23);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(62, 27);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(240, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // GridCreationMethods
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 329);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GridCreationMethods";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose a Grid Creation Method";
            this.Load += new System.EventHandler(this.GridCreationMethods_Load);
            this.grp.ResumeLayout(false);
            this.grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGTHelp)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.RadioButton rbtnModelData;
        private System.Windows.Forms.RadioButton rbtnMonitorData;
        private System.Windows.Forms.RadioButton rbtnMonitorRollback;
        private System.Windows.Forms.GroupBox grp;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbtnOpenFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtExistingAQG;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveNewFormat;
        private System.Windows.Forms.ComboBox cboGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picGTHelp;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}