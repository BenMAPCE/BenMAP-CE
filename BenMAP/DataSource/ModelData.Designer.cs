namespace BenMAP
{
    partial class ModelData
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
            this.txtPollutant = new System.Windows.Forms.TextBox();
            this.txtGridType = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbp = new System.Windows.Forms.TabPage();
            this.btnViewMetadata = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseDatabase = new System.Windows.Forms.Button();
            this.txtModelDatabase = new System.Windows.Forms.TextBox();
            this.lblModelDatabase = new System.Windows.Forms.Label();
            this.tbp2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.txtModelFile = new System.Windows.Forms.TextBox();
            this.lblModelFile = new System.Windows.Forms.Label();
            this.cboGridType = new System.Windows.Forms.ComboBox();
            this.lblPollutant = new System.Windows.Forms.Label();
            this.lblGridType = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbp.SuspendLayout();
            this.tbp2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPollutant);
            this.groupBox1.Controls.Add(this.txtGridType);
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.cboGridType);
            this.groupBox1.Controls.Add(this.lblPollutant);
            this.groupBox1.Controls.Add(this.lblGridType);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 295);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtPollutant
            // 
            this.txtPollutant.Location = new System.Drawing.Point(82, 63);
            this.txtPollutant.Name = "txtPollutant";
            this.txtPollutant.Size = new System.Drawing.Size(168, 22);
            this.txtPollutant.TabIndex = 6;
            // 
            // txtGridType
            // 
            this.txtGridType.Location = new System.Drawing.Point(82, 16);
            this.txtGridType.Name = "txtGridType";
            this.txtGridType.Size = new System.Drawing.Size(168, 22);
            this.txtGridType.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbp);
            this.tabControl1.Controls.Add(this.tbp2);
            this.tabControl1.Location = new System.Drawing.Point(21, 93);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(313, 194);
            this.tabControl1.TabIndex = 4;
            // 
            // tbp
            // 
            this.tbp.Controls.Add(this.btnViewMetadata);
            this.tbp.Controls.Add(this.btnValidate);
            this.tbp.Controls.Add(this.label1);
            this.tbp.Controls.Add(this.btnBrowseDatabase);
            this.tbp.Controls.Add(this.txtModelDatabase);
            this.tbp.Controls.Add(this.lblModelDatabase);
            this.tbp.Location = new System.Drawing.Point(4, 23);
            this.tbp.Name = "tbp";
            this.tbp.Padding = new System.Windows.Forms.Padding(3);
            this.tbp.Size = new System.Drawing.Size(305, 167);
            this.tbp.TabIndex = 0;
            this.tbp.Text = "Generic Model Database";
            this.tbp.UseVisualStyleBackColor = true;
            // 
            // btnViewMetadata
            // 
            this.btnViewMetadata.Location = new System.Drawing.Point(91, 131);
            this.btnViewMetadata.Name = "btnViewMetadata";
            this.btnViewMetadata.Size = new System.Drawing.Size(105, 27);
            this.btnViewMetadata.TabIndex = 5;
            this.btnViewMetadata.Text = "View Metadata";
            this.btnViewMetadata.UseVisualStyleBackColor = true;
            this.btnViewMetadata.Visible = false;
            this.btnViewMetadata.Click += new System.EventHandler(this.btnViewMetadata_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.Enabled = false;
            this.btnValidate.Location = new System.Drawing.Point(10, 131);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(75, 27);
            this.btnValidate.TabIndex = 4;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 49);
            this.label1.TabIndex = 3;
            this.label1.Text = "The file layout is: Column, Row, Metric, Seasonal Metric, Annual Metric, Values; " +
    "Values is a string of comma delimited model values.";
            // 
            // btnBrowseDatabase
            // 
            this.btnBrowseDatabase.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowseDatabase.Location = new System.Drawing.Point(251, 35);
            this.btnBrowseDatabase.Name = "btnBrowseDatabase";
            this.btnBrowseDatabase.Size = new System.Drawing.Size(39, 27);
            this.btnBrowseDatabase.TabIndex = 2;
            this.btnBrowseDatabase.UseVisualStyleBackColor = true;
            this.btnBrowseDatabase.Click += new System.EventHandler(this.btnBrowseDatabase_Click);
            // 
            // txtModelDatabase
            // 
            this.txtModelDatabase.Location = new System.Drawing.Point(6, 37);
            this.txtModelDatabase.Name = "txtModelDatabase";
            this.txtModelDatabase.ReadOnly = true;
            this.txtModelDatabase.Size = new System.Drawing.Size(239, 22);
            this.txtModelDatabase.TabIndex = 1;
            this.txtModelDatabase.TextChanged += new System.EventHandler(this.txtModelDatabase_TextChanged);
            // 
            // lblModelDatabase
            // 
            this.lblModelDatabase.AutoSize = true;
            this.lblModelDatabase.Location = new System.Drawing.Point(6, 15);
            this.lblModelDatabase.Name = "lblModelDatabase";
            this.lblModelDatabase.Size = new System.Drawing.Size(101, 14);
            this.lblModelDatabase.TabIndex = 0;
            this.lblModelDatabase.Text = "Model Database:";
            // 
            // tbp2
            // 
            this.tbp2.Controls.Add(this.label2);
            this.tbp2.Controls.Add(this.btnBrowseFile);
            this.tbp2.Controls.Add(this.txtModelFile);
            this.tbp2.Controls.Add(this.lblModelFile);
            this.tbp2.Location = new System.Drawing.Point(4, 23);
            this.tbp2.Name = "tbp2";
            this.tbp2.Padding = new System.Windows.Forms.Padding(3);
            this.tbp2.Size = new System.Drawing.Size(305, 167);
            this.tbp2.TabIndex = 1;
            this.tbp2.Text = "New Format Database";
            this.tbp2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(299, 77);
            this.label2.TabIndex = 3;
            this.label2.Text = "This is a new format model file.The first row format: Pollutant name,grid definit" +
    "ion name,model.The second Row format: col, row, metricname0, metricname1 ...";
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowseFile.Location = new System.Drawing.Point(260, 37);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(39, 27);
            this.btnBrowseFile.TabIndex = 2;
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // txtModelFile
            // 
            this.txtModelFile.Location = new System.Drawing.Point(6, 40);
            this.txtModelFile.Name = "txtModelFile";
            this.txtModelFile.ReadOnly = true;
            this.txtModelFile.Size = new System.Drawing.Size(248, 22);
            this.txtModelFile.TabIndex = 1;
            // 
            // lblModelFile
            // 
            this.lblModelFile.AutoSize = true;
            this.lblModelFile.Location = new System.Drawing.Point(6, 15);
            this.lblModelFile.Name = "lblModelFile";
            this.lblModelFile.Size = new System.Drawing.Size(69, 14);
            this.lblModelFile.TabIndex = 0;
            this.lblModelFile.Text = "Model File:";
            // 
            // cboGridType
            // 
            this.cboGridType.FormattingEnabled = true;
            this.cboGridType.Location = new System.Drawing.Point(82, 16);
            this.cboGridType.Name = "cboGridType";
            this.cboGridType.Size = new System.Drawing.Size(168, 22);
            this.cboGridType.TabIndex = 2;
            // 
            // lblPollutant
            // 
            this.lblPollutant.AutoSize = true;
            this.lblPollutant.Location = new System.Drawing.Point(19, 66);
            this.lblPollutant.Name = "lblPollutant";
            this.lblPollutant.Size = new System.Drawing.Size(60, 14);
            this.lblPollutant.TabIndex = 1;
            this.lblPollutant.Text = "Pollutant:";
            // 
            // lblGridType
            // 
            this.lblGridType.AutoSize = true;
            this.lblGridType.Location = new System.Drawing.Point(19, 20);
            this.lblGridType.Name = "lblGridType";
            this.lblGridType.Size = new System.Drawing.Size(60, 14);
            this.lblGridType.TabIndex = 0;
            this.lblGridType.Text = "Grid Type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOK);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(12, 304);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(340, 51);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(257, 17);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(66, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(174, 17);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(63, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Maroon;
            this.label3.Location = new System.Drawing.Point(9, 366);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(343, 49);
            this.label3.TabIndex = 8;
            this.label3.Text = "Note: missing days will not be cleaned up across pollutants. Enter model data by " +
    "double-clicking the “Source of Air Quality Data…” tree node instead.";
            // 
            // ModelData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 424);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ModelData";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model Data";
            this.Load += new System.EventHandler(this.ModelData_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbp.ResumeLayout(false);
            this.tbp.PerformLayout();
            this.tbp2.ResumeLayout(false);
            this.tbp2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbp;
        private System.Windows.Forms.Button btnBrowseDatabase;
        private System.Windows.Forms.TextBox txtModelDatabase;
        private System.Windows.Forms.Label lblModelDatabase;
        private System.Windows.Forms.TabPage tbp2;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.TextBox txtModelFile;
        private System.Windows.Forms.Label lblModelFile;
        private System.Windows.Forms.ComboBox cboGridType;
        private System.Windows.Forms.Label lblPollutant;
        private System.Windows.Forms.Label lblGridType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPollutant;
        private System.Windows.Forms.TextBox txtGridType;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button btnViewMetadata;
        private System.Windows.Forms.Label label3;
    }
}