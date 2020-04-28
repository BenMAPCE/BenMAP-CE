namespace BenMAP
{
	partial class LoadPopulationDataSet
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
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.grpCancelOK = new System.Windows.Forms.GroupBox();
			this.btnViewMetadata = new System.Windows.Forms.Button();
			this.btnValidate = new System.Windows.Forms.Button();
			this.lblprogbar = new System.Windows.Forms.Label();
			this.progBarLoadPop = new System.Windows.Forms.ProgressBar();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.grpDataSetDetail = new System.Windows.Forms.GroupBox();
			this.grpPopulationWeight = new System.Windows.Forms.GroupBox();
			this.btnViewMetadataGW = new System.Windows.Forms.Button();
			this.btnValidateGW = new System.Windows.Forms.Button();
			this.btnViewMetadataDB = new System.Windows.Forms.Button();
			this.btnBrowseDB = new System.Windows.Forms.Button();
			this.btnValidateDB = new System.Windows.Forms.Button();
			this.btnBrowseGW = new System.Windows.Forms.Button();
			this.txtDataBase = new System.Windows.Forms.TextBox();
			this.txtGrowthWeights = new System.Windows.Forms.TextBox();
			this.lblDatabase = new System.Windows.Forms.Label();
			this.chkPopulationGrowth = new System.Windows.Forms.CheckBox();
			this.cboGridDefinition = new System.Windows.Forms.ComboBox();
			this.grpConfiguration = new System.Windows.Forms.GroupBox();
			this.chkUseWoodsPoole = new System.Windows.Forms.CheckBox();
			this.btnView = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.cboConfiguration = new System.Windows.Forms.ComboBox();
			this.lblPopulationConfig = new System.Windows.Forms.Label();
			this.txtDataSetName = new System.Windows.Forms.TextBox();
			this.lblGridDefinition = new System.Windows.Forms.Label();
			this.lblPopulationDataSetName = new System.Windows.Forms.Label();
			this.grpCancelOK.SuspendLayout();
			this.grpDataSetDetail.SuspendLayout();
			this.grpPopulationWeight.SuspendLayout();
			this.grpConfiguration.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpCancelOK
			// 
			this.grpCancelOK.Controls.Add(this.btnViewMetadata);
			this.grpCancelOK.Controls.Add(this.btnValidate);
			this.grpCancelOK.Controls.Add(this.lblprogbar);
			this.grpCancelOK.Controls.Add(this.progBarLoadPop);
			this.grpCancelOK.Controls.Add(this.btnOK);
			this.grpCancelOK.Controls.Add(this.btnCancel);
			this.grpCancelOK.Location = new System.Drawing.Point(3, 383);
			this.grpCancelOK.Name = "grpCancelOK";
			this.grpCancelOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.grpCancelOK.Size = new System.Drawing.Size(397, 74);
			this.grpCancelOK.TabIndex = 1;
			this.grpCancelOK.TabStop = false;
			// 
			// btnViewMetadata
			// 
			this.btnViewMetadata.Enabled = false;
			this.btnViewMetadata.Location = new System.Drawing.Point(107, 36);
			this.btnViewMetadata.Name = "btnViewMetadata";
			this.btnViewMetadata.Size = new System.Drawing.Size(108, 27);
			this.btnViewMetadata.TabIndex = 3;
			this.btnViewMetadata.Text = "View Metadata";
			this.btnViewMetadata.UseVisualStyleBackColor = true;
			this.btnViewMetadata.Visible = false;
			// 
			// btnValidate
			// 
			this.btnValidate.Enabled = false;
			this.btnValidate.Location = new System.Drawing.Point(9, 36);
			this.btnValidate.Name = "btnValidate";
			this.btnValidate.Size = new System.Drawing.Size(75, 27);
			this.btnValidate.TabIndex = 1;
			this.btnValidate.Text = "Validate";
			this.btnValidate.UseVisualStyleBackColor = true;
			this.btnValidate.Visible = false;
			// 
			// lblprogbar
			// 
			this.lblprogbar.AutoSize = true;
			this.lblprogbar.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblprogbar.ForeColor = System.Drawing.Color.Black;
			this.lblprogbar.Location = new System.Drawing.Point(8, 42);
			this.lblprogbar.Name = "lblprogbar";
			this.lblprogbar.Size = new System.Drawing.Size(0, 14);
			this.lblprogbar.TabIndex = 2;
			// 
			// progBarLoadPop
			// 
			this.progBarLoadPop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.progBarLoadPop.Location = new System.Drawing.Point(6, 18);
			this.progBarLoadPop.Name = "progBarLoadPop";
			this.progBarLoadPop.Size = new System.Drawing.Size(382, 12);
			this.progBarLoadPop.Step = 1;
			this.progBarLoadPop.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(324, 36);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(63, 27);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(238, 36);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(63, 27);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// grpDataSetDetail
			// 
			this.grpDataSetDetail.Controls.Add(this.grpPopulationWeight);
			this.grpDataSetDetail.Controls.Add(this.cboGridDefinition);
			this.grpDataSetDetail.Controls.Add(this.grpConfiguration);
			this.grpDataSetDetail.Controls.Add(this.txtDataSetName);
			this.grpDataSetDetail.Controls.Add(this.lblGridDefinition);
			this.grpDataSetDetail.Controls.Add(this.lblPopulationDataSetName);
			this.grpDataSetDetail.Location = new System.Drawing.Point(3, 3);
			this.grpDataSetDetail.Name = "grpDataSetDetail";
			this.grpDataSetDetail.Size = new System.Drawing.Size(397, 374);
			this.grpDataSetDetail.TabIndex = 0;
			this.grpDataSetDetail.TabStop = false;
			// 
			// grpPopulationWeight
			// 
			this.grpPopulationWeight.Controls.Add(this.btnViewMetadataGW);
			this.grpPopulationWeight.Controls.Add(this.btnValidateGW);
			this.grpPopulationWeight.Controls.Add(this.btnViewMetadataDB);
			this.grpPopulationWeight.Controls.Add(this.btnBrowseDB);
			this.grpPopulationWeight.Controls.Add(this.btnValidateDB);
			this.grpPopulationWeight.Controls.Add(this.btnBrowseGW);
			this.grpPopulationWeight.Controls.Add(this.txtDataBase);
			this.grpPopulationWeight.Controls.Add(this.txtGrowthWeights);
			this.grpPopulationWeight.Controls.Add(this.lblDatabase);
			this.grpPopulationWeight.Controls.Add(this.chkPopulationGrowth);
			this.grpPopulationWeight.Location = new System.Drawing.Point(6, 179);
			this.grpPopulationWeight.Name = "grpPopulationWeight";
			this.grpPopulationWeight.Size = new System.Drawing.Size(382, 182);
			this.grpPopulationWeight.TabIndex = 5;
			this.grpPopulationWeight.TabStop = false;
			// 
			// btnViewMetadataGW
			// 
			this.btnViewMetadataGW.Enabled = false;
			this.btnViewMetadataGW.Location = new System.Drawing.Point(266, 144);
			this.btnViewMetadataGW.Name = "btnViewMetadataGW";
			this.btnViewMetadataGW.Size = new System.Drawing.Size(108, 27);
			this.btnViewMetadataGW.TabIndex = 9;
			this.btnViewMetadataGW.Text = "View Metadata";
			this.btnViewMetadataGW.UseVisualStyleBackColor = true;
			this.btnViewMetadataGW.Visible = false;
			this.btnViewMetadataGW.Click += new System.EventHandler(this.btnViewMetadataGW_Click);
			// 
			// btnValidateGW
			// 
			this.btnValidateGW.Enabled = false;
			this.btnValidateGW.Location = new System.Drawing.Point(185, 144);
			this.btnValidateGW.Name = "btnValidateGW";
			this.btnValidateGW.Size = new System.Drawing.Size(75, 27);
			this.btnValidateGW.TabIndex = 8;
			this.btnValidateGW.Text = "Validate";
			this.btnValidateGW.UseVisualStyleBackColor = true;
			this.btnValidateGW.Visible = false;
			this.btnValidateGW.Click += new System.EventHandler(this.btnValidateGW_Click);
			// 
			// btnViewMetadataDB
			// 
			this.btnViewMetadataDB.Enabled = false;
			this.btnViewMetadataDB.Location = new System.Drawing.Point(266, 59);
			this.btnViewMetadataDB.Name = "btnViewMetadataDB";
			this.btnViewMetadataDB.Size = new System.Drawing.Size(108, 27);
			this.btnViewMetadataDB.TabIndex = 4;
			this.btnViewMetadataDB.Text = "View Metadata";
			this.btnViewMetadataDB.UseVisualStyleBackColor = true;
			this.btnViewMetadataDB.Click += new System.EventHandler(this.btnViewMetadataDB_Click);
			// 
			// btnBrowseDB
			// 
			this.btnBrowseDB.Location = new System.Drawing.Point(299, 28);
			this.btnBrowseDB.Name = "btnBrowseDB";
			this.btnBrowseDB.Size = new System.Drawing.Size(75, 27);
			this.btnBrowseDB.TabIndex = 2;
			this.btnBrowseDB.Text = "Browse";
			this.btnBrowseDB.UseVisualStyleBackColor = true;
			this.btnBrowseDB.Click += new System.EventHandler(this.btnBrowseDB_Click);
			// 
			// btnValidateDB
			// 
			this.btnValidateDB.Enabled = false;
			this.btnValidateDB.Location = new System.Drawing.Point(185, 59);
			this.btnValidateDB.Name = "btnValidateDB";
			this.btnValidateDB.Size = new System.Drawing.Size(75, 27);
			this.btnValidateDB.TabIndex = 3;
			this.btnValidateDB.Text = "Validate";
			this.btnValidateDB.UseVisualStyleBackColor = true;
			this.btnValidateDB.Click += new System.EventHandler(this.btnValidateDB_Click);
			// 
			// btnBrowseGW
			// 
			this.btnBrowseGW.Location = new System.Drawing.Point(301, 111);
			this.btnBrowseGW.Name = "btnBrowseGW";
			this.btnBrowseGW.Size = new System.Drawing.Size(75, 27);
			this.btnBrowseGW.TabIndex = 7;
			this.btnBrowseGW.Text = "Browse";
			this.btnBrowseGW.UseVisualStyleBackColor = true;
			this.btnBrowseGW.Click += new System.EventHandler(this.btnBrowseGW_Click);
			// 
			// txtDataBase
			// 
			this.txtDataBase.Location = new System.Drawing.Point(9, 31);
			this.txtDataBase.Name = "txtDataBase";
			this.txtDataBase.ReadOnly = true;
			this.txtDataBase.Size = new System.Drawing.Size(282, 22);
			this.txtDataBase.TabIndex = 1;
			this.txtDataBase.TextChanged += new System.EventHandler(this.txtDataBase_TextChanged);
			// 
			// txtGrowthWeights
			// 
			this.txtGrowthWeights.Location = new System.Drawing.Point(9, 113);
			this.txtGrowthWeights.Name = "txtGrowthWeights";
			this.txtGrowthWeights.ReadOnly = true;
			this.txtGrowthWeights.Size = new System.Drawing.Size(282, 22);
			this.txtGrowthWeights.TabIndex = 6;
			this.txtGrowthWeights.TextChanged += new System.EventHandler(this.txtGrowthWeights_TextChanged);
			// 
			// lblDatabase
			// 
			this.lblDatabase.AutoSize = true;
			this.lblDatabase.Location = new System.Drawing.Point(6, 14);
			this.lblDatabase.Name = "lblDatabase";
			this.lblDatabase.Size = new System.Drawing.Size(63, 14);
			this.lblDatabase.TabIndex = 0;
			this.lblDatabase.Text = "Database:";
			// 
			// chkPopulationGrowth
			// 
			this.chkPopulationGrowth.AutoSize = true;
			this.chkPopulationGrowth.Location = new System.Drawing.Point(9, 92);
			this.chkPopulationGrowth.Name = "chkPopulationGrowth";
			this.chkPopulationGrowth.Size = new System.Drawing.Size(199, 18);
			this.chkPopulationGrowth.TabIndex = 5;
			this.chkPopulationGrowth.Text = "Use Population Growth Weights";
			this.chkPopulationGrowth.UseVisualStyleBackColor = true;
			this.chkPopulationGrowth.CheckedChanged += new System.EventHandler(this.chkPopulationGrowth_CheckedChanged);
			// 
			// cboGridDefinition
			// 
			this.cboGridDefinition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboGridDefinition.FormattingEnabled = true;
			this.cboGridDefinition.Location = new System.Drawing.Point(165, 43);
			this.cboGridDefinition.Name = "cboGridDefinition";
			this.cboGridDefinition.Size = new System.Drawing.Size(215, 22);
			this.cboGridDefinition.TabIndex = 3;
			this.cboGridDefinition.SelectedIndexChanged += new System.EventHandler(this.cboGridDefinition_SelectedIndexChanged);
			// 
			// grpConfiguration
			// 
			this.grpConfiguration.Controls.Add(this.chkUseWoodsPoole);
			this.grpConfiguration.Controls.Add(this.btnView);
			this.grpConfiguration.Controls.Add(this.btnAdd);
			this.grpConfiguration.Controls.Add(this.btnDelete);
			this.grpConfiguration.Controls.Add(this.cboConfiguration);
			this.grpConfiguration.Controls.Add(this.lblPopulationConfig);
			this.grpConfiguration.Location = new System.Drawing.Point(6, 72);
			this.grpConfiguration.Name = "grpConfiguration";
			this.grpConfiguration.Size = new System.Drawing.Size(382, 101);
			this.grpConfiguration.TabIndex = 4;
			this.grpConfiguration.TabStop = false;
			// 
			// chkUseWoodsPoole
			// 
			this.chkUseWoodsPoole.AutoSize = true;
			this.chkUseWoodsPoole.Location = new System.Drawing.Point(9, 48);
			this.chkUseWoodsPoole.Name = "chkUseWoodsPoole";
			this.chkUseWoodsPoole.Size = new System.Drawing.Size(269, 18);
			this.chkUseWoodsPoole.TabIndex = 2;
			this.chkUseWoodsPoole.Text = "Use Woods and Poole Population Projections";
			this.chkUseWoodsPoole.UseVisualStyleBackColor = true;
			// 
			// btnView
			// 
			this.btnView.Location = new System.Drawing.Point(323, 68);
			this.btnView.Name = "btnView";
			this.btnView.Size = new System.Drawing.Size(51, 27);
			this.btnView.TabIndex = 5;
			this.btnView.Text = "View";
			this.btnView.UseVisualStyleBackColor = true;
			this.btnView.Click += new System.EventHandler(this.btnView_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(266, 68);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(51, 27);
			this.btnAdd.TabIndex = 4;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(201, 68);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(59, 27);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// cboConfiguration
			// 
			this.cboConfiguration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboConfiguration.FormattingEnabled = true;
			this.cboConfiguration.Location = new System.Drawing.Point(159, 20);
			this.cboConfiguration.Name = "cboConfiguration";
			this.cboConfiguration.Size = new System.Drawing.Size(215, 22);
			this.cboConfiguration.TabIndex = 1;
			this.cboConfiguration.SelectedIndexChanged += new System.EventHandler(this.cboConfiguration_SelectedIndexChanged);
			// 
			// lblPopulationConfig
			// 
			this.lblPopulationConfig.AutoSize = true;
			this.lblPopulationConfig.Location = new System.Drawing.Point(6, 20);
			this.lblPopulationConfig.Name = "lblPopulationConfig";
			this.lblPopulationConfig.Size = new System.Drawing.Size(143, 14);
			this.lblPopulationConfig.TabIndex = 0;
			this.lblPopulationConfig.Text = "Population Configuration:";
			// 
			// txtDataSetName
			// 
			this.txtDataSetName.Location = new System.Drawing.Point(165, 16);
			this.txtDataSetName.Name = "txtDataSetName";
			this.txtDataSetName.Size = new System.Drawing.Size(215, 22);
			this.txtDataSetName.TabIndex = 1;
			// 
			// lblGridDefinition
			// 
			this.lblGridDefinition.AutoSize = true;
			this.lblGridDefinition.Location = new System.Drawing.Point(9, 47);
			this.lblGridDefinition.Name = "lblGridDefinition";
			this.lblGridDefinition.Size = new System.Drawing.Size(89, 14);
			this.lblGridDefinition.TabIndex = 2;
			this.lblGridDefinition.Text = "Grid Definition:";
			// 
			// lblPopulationDataSetName
			// 
			this.lblPopulationDataSetName.AutoSize = true;
			this.lblPopulationDataSetName.Location = new System.Drawing.Point(9, 20);
			this.lblPopulationDataSetName.Name = "lblPopulationDataSetName";
			this.lblPopulationDataSetName.Size = new System.Drawing.Size(150, 14);
			this.lblPopulationDataSetName.TabIndex = 0;
			this.lblPopulationDataSetName.Text = "Population Dataset Name:";
			this.lblPopulationDataSetName.Click += new System.EventHandler(this.lblPopulationDataSetName_Click);
			// 
			// LoadPopulationDataSet
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(407, 462);
			this.Controls.Add(this.grpCancelOK);
			this.Controls.Add(this.grpDataSetDetail);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(413, 399);
			this.Name = "LoadPopulationDataSet";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Load Population Dataset";
			this.Load += new System.EventHandler(this.LoadPopulationDataSet_Load);
			this.grpCancelOK.ResumeLayout(false);
			this.grpCancelOK.PerformLayout();
			this.grpDataSetDetail.ResumeLayout(false);
			this.grpDataSetDetail.PerformLayout();
			this.grpPopulationWeight.ResumeLayout(false);
			this.grpPopulationWeight.PerformLayout();
			this.grpConfiguration.ResumeLayout(false);
			this.grpConfiguration.PerformLayout();
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.Label lblPopulationDataSetName;
		private System.Windows.Forms.GroupBox grpDataSetDetail;
		private System.Windows.Forms.GroupBox grpPopulationWeight;
		private System.Windows.Forms.TextBox txtGrowthWeights;
		private System.Windows.Forms.CheckBox chkPopulationGrowth;
		private System.Windows.Forms.TextBox txtDataBase;
		private System.Windows.Forms.ComboBox cboGridDefinition;
		private System.Windows.Forms.GroupBox grpConfiguration;
		private System.Windows.Forms.Button btnView;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.ComboBox cboConfiguration;
		private System.Windows.Forms.Label lblPopulationConfig;
		private System.Windows.Forms.TextBox txtDataSetName;
		private System.Windows.Forms.Label lblDatabase;
		private System.Windows.Forms.Label lblGridDefinition;
		private System.Windows.Forms.GroupBox grpCancelOK;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnBrowseDB;
		private System.Windows.Forms.Button btnBrowseGW;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ProgressBar progBarLoadPop;
		private System.Windows.Forms.Label lblprogbar;
		private System.Windows.Forms.CheckBox chkUseWoodsPoole;
		private System.Windows.Forms.Button btnViewMetadata;
		private System.Windows.Forms.Button btnValidate;
		private System.Windows.Forms.Button btnViewMetadataGW;
		private System.Windows.Forms.Button btnValidateGW;
		private System.Windows.Forms.Button btnViewMetadataDB;
		private System.Windows.Forms.Button btnValidateDB;
	}
}