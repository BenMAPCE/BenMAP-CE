namespace BenMAP
{
    partial class MonitorRollback
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
            this.lblPollutant = new System.Windows.Forms.Label();
            this.lblRollbackGridType = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboRollbackGridType = new System.Windows.Forms.ComboBox();
            this.txtPollutant = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbpLibrary = new System.Windows.Forms.TabPage();
            this.cboMonitorLibraryYear = new System.Windows.Forms.ComboBox();
            this.cboMonitorDataSet = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMonitorDataSet = new System.Windows.Forms.Label();
            this.tbgTextFile = new System.Windows.Forms.TabPage();
            this.btnValidate = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtMonitorDataFile = new System.Windows.Forms.TextBox();
            this.lblMonitorDataFile = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbpLibrary.SuspendLayout();
            this.tbgTextFile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPollutant
            // 
            this.lblPollutant.AutoSize = true;
            this.lblPollutant.Location = new System.Drawing.Point(19, 20);
            this.lblPollutant.Name = "lblPollutant";
            this.lblPollutant.Size = new System.Drawing.Size(60, 14);
            this.lblPollutant.TabIndex = 0;
            this.lblPollutant.Text = "Pollutant:";
            // 
            // lblRollbackGridType
            // 
            this.lblRollbackGridType.AutoSize = true;
            this.lblRollbackGridType.Location = new System.Drawing.Point(19, 59);
            this.lblRollbackGridType.Name = "lblRollbackGridType";
            this.lblRollbackGridType.Size = new System.Drawing.Size(110, 14);
            this.lblRollbackGridType.TabIndex = 2;
            this.lblRollbackGridType.Text = "Rollback Grid Type:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboRollbackGridType);
            this.groupBox1.Controls.Add(this.txtPollutant);
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.lblPollutant);
            this.groupBox1.Controls.Add(this.lblRollbackGridType);
            this.groupBox1.Location = new System.Drawing.Point(11, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 289);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // cboRollbackGridType
            // 
            this.cboRollbackGridType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRollbackGridType.FormattingEnabled = true;
            this.cboRollbackGridType.Location = new System.Drawing.Point(126, 55);
            this.cboRollbackGridType.Name = "cboRollbackGridType";
            this.cboRollbackGridType.Size = new System.Drawing.Size(189, 22);
            this.cboRollbackGridType.TabIndex = 7;
            this.cboRollbackGridType.SelectedIndexChanged += new System.EventHandler(this.cboRollbackGridType_SelectedIndexChanged);
            // 
            // txtPollutant
            // 
            this.txtPollutant.Location = new System.Drawing.Point(126, 17);
            this.txtPollutant.Name = "txtPollutant";
            this.txtPollutant.Size = new System.Drawing.Size(189, 22);
            this.txtPollutant.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbpLibrary);
            this.tabControl1.Controls.Add(this.tbgTextFile);
            this.tabControl1.Location = new System.Drawing.Point(19, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(301, 171);
            this.tabControl1.TabIndex = 4;
            // 
            // tbpLibrary
            // 
            this.tbpLibrary.Controls.Add(this.cboMonitorLibraryYear);
            this.tbpLibrary.Controls.Add(this.cboMonitorDataSet);
            this.tbpLibrary.Controls.Add(this.label2);
            this.tbpLibrary.Controls.Add(this.lblMonitorDataSet);
            this.tbpLibrary.Location = new System.Drawing.Point(4, 23);
            this.tbpLibrary.Name = "tbpLibrary";
            this.tbpLibrary.Padding = new System.Windows.Forms.Padding(3);
            this.tbpLibrary.Size = new System.Drawing.Size(293, 144);
            this.tbpLibrary.TabIndex = 0;
            this.tbpLibrary.Text = "Library";
            this.tbpLibrary.UseVisualStyleBackColor = true;
            // 
            // cboMonitorLibraryYear
            // 
            this.cboMonitorLibraryYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonitorLibraryYear.FormattingEnabled = true;
            this.cboMonitorLibraryYear.Location = new System.Drawing.Point(20, 97);
            this.cboMonitorLibraryYear.Name = "cboMonitorLibraryYear";
            this.cboMonitorLibraryYear.Size = new System.Drawing.Size(221, 22);
            this.cboMonitorLibraryYear.TabIndex = 3;
            this.cboMonitorLibraryYear.SelectedIndexChanged += new System.EventHandler(this.cboMonitorLibraryYear_SelectedIndexChanged);
            // 
            // cboMonitorDataSet
            // 
            this.cboMonitorDataSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonitorDataSet.FormattingEnabled = true;
            this.cboMonitorDataSet.Location = new System.Drawing.Point(20, 37);
            this.cboMonitorDataSet.Name = "cboMonitorDataSet";
            this.cboMonitorDataSet.Size = new System.Drawing.Size(221, 22);
            this.cboMonitorDataSet.TabIndex = 2;
            this.cboMonitorDataSet.SelectedIndexChanged += new System.EventHandler(this.cboMonitorDataSet_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Monitor Library Year:";
            // 
            // lblMonitorDataSet
            // 
            this.lblMonitorDataSet.AutoSize = true;
            this.lblMonitorDataSet.Location = new System.Drawing.Point(18, 20);
            this.lblMonitorDataSet.Name = "lblMonitorDataSet";
            this.lblMonitorDataSet.Size = new System.Drawing.Size(99, 14);
            this.lblMonitorDataSet.TabIndex = 0;
            this.lblMonitorDataSet.Text = "Monitor Dataset:";
            // 
            // tbgTextFile
            // 
            this.tbgTextFile.Controls.Add(this.btnValidate);
            this.tbgTextFile.Controls.Add(this.btnBrowse);
            this.tbgTextFile.Controls.Add(this.txtMonitorDataFile);
            this.tbgTextFile.Controls.Add(this.lblMonitorDataFile);
            this.tbgTextFile.Location = new System.Drawing.Point(4, 23);
            this.tbgTextFile.Name = "tbgTextFile";
            this.tbgTextFile.Padding = new System.Windows.Forms.Padding(3);
            this.tbgTextFile.Size = new System.Drawing.Size(293, 144);
            this.tbgTextFile.TabIndex = 1;
            this.tbgTextFile.Text = "Text File";
            this.tbgTextFile.UseVisualStyleBackColor = true;
            // 
            // btnValidate
            // 
            this.btnValidate.Enabled = false;
            this.btnValidate.Location = new System.Drawing.Point(19, 108);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(75, 23);
            this.btnValidate.TabIndex = 3;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowse.Location = new System.Drawing.Point(244, 52);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(43, 27);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtMonitorDataFile
            // 
            this.txtMonitorDataFile.Location = new System.Drawing.Point(18, 52);
            this.txtMonitorDataFile.Name = "txtMonitorDataFile";
            this.txtMonitorDataFile.ReadOnly = true;
            this.txtMonitorDataFile.Size = new System.Drawing.Size(220, 22);
            this.txtMonitorDataFile.TabIndex = 1;
            this.txtMonitorDataFile.TextChanged += new System.EventHandler(this.txtMonitorDataFile_TextChanged);
            // 
            // lblMonitorDataFile
            // 
            this.lblMonitorDataFile.AutoSize = true;
            this.lblMonitorDataFile.Location = new System.Drawing.Point(16, 19);
            this.lblMonitorDataFile.Name = "lblMonitorDataFile";
            this.lblMonitorDataFile.Size = new System.Drawing.Size(106, 14);
            this.lblMonitorDataFile.TabIndex = 0;
            this.lblMonitorDataFile.Text = "Monitor Data File:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnNext);
            this.groupBox2.Controls.Add(this.btnAdvanced);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(11, 297);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(340, 61);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(248, 23);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(67, 27);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Location = new System.Drawing.Point(164, 23);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(67, 27);
            this.btnAdvanced.TabIndex = 1;
            this.btnAdvanced.Text = "Advanced";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(74, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MonitorRollback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 365);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MonitorRollback";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor Rollback: (1) Select Monitors";
            this.Load += new System.EventHandler(this.MonitorRollback_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbpLibrary.ResumeLayout(false);
            this.tbpLibrary.PerformLayout();
            this.tbgTextFile.ResumeLayout(false);
            this.tbgTextFile.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label lblPollutant;
        private System.Windows.Forms.Label lblRollbackGridType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbpLibrary;
        private System.Windows.Forms.ComboBox cboMonitorLibraryYear;
        private System.Windows.Forms.ComboBox cboMonitorDataSet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMonitorDataSet;
        private System.Windows.Forms.TabPage tbgTextFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtMonitorDataFile;
        private System.Windows.Forms.Label lblMonitorDataFile;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtPollutant;
        private System.Windows.Forms.ComboBox cboRollbackGridType;
        private System.Windows.Forms.Button btnValidate;
    }
}