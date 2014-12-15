namespace BenMAP
{
    partial class PopulationConfigurationDefinition
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopulationConfigurationDefinition));
            this.label1 = new System.Windows.Forms.Label();
            this.txtConfigName = new System.Windows.Forms.TextBox();
            this.grp1 = new System.Windows.Forms.GroupBox();
            this.grp5 = new System.Windows.Forms.GroupBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvAgeRangs = new System.Windows.Forms.DataGridView();
            this.grp4 = new System.Windows.Forms.GroupBox();
            this.grp9 = new System.Windows.Forms.GroupBox();
            this.lstAvailableEthnicity = new System.Windows.Forms.ListBox();
            this.btnAddEthnicity = new System.Windows.Forms.Button();
            this.grpEthnicities = new System.Windows.Forms.GroupBox();
            this.btnEthnicitiesRemove = new System.Windows.Forms.Button();
            this.btnEthnicitiesNew = new System.Windows.Forms.Button();
            this.lstEthnicities = new System.Windows.Forms.ListBox();
            this.grp3 = new System.Windows.Forms.GroupBox();
            this.grp8 = new System.Windows.Forms.GroupBox();
            this.lstAvailableGrnders = new System.Windows.Forms.ListBox();
            this.btnAddGender = new System.Windows.Forms.Button();
            this.grpGenders = new System.Windows.Forms.GroupBox();
            this.btnGendersRemove = new System.Windows.Forms.Button();
            this.btnGendersNew = new System.Windows.Forms.Button();
            this.lstGenders = new System.Windows.Forms.ListBox();
            this.grp2 = new System.Windows.Forms.GroupBox();
            this.grp7 = new System.Windows.Forms.GroupBox();
            this.lstAvailableRaces = new System.Windows.Forms.ListBox();
            this.btnAddRace = new System.Windows.Forms.Button();
            this.grpRace = new System.Windows.Forms.GroupBox();
            this.btnRaceNew = new System.Windows.Forms.Button();
            this.btnRaceRemove = new System.Windows.Forms.Button();
            this.lstRaces = new System.Windows.Forms.ListBox();
            this.grp6 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grp1.SuspendLayout();
            this.grp5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAgeRangs)).BeginInit();
            this.grp4.SuspendLayout();
            this.grp9.SuspendLayout();
            this.grpEthnicities.SuspendLayout();
            this.grp3.SuspendLayout();
            this.grp8.SuspendLayout();
            this.grpGenders.SuspendLayout();
            this.grp2.SuspendLayout();
            this.grp7.SuspendLayout();
            this.grpRace.SuspendLayout();
            this.grp6.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Population Configuration Name:";
            // 
            // txtConfigName
            // 
            this.txtConfigName.Location = new System.Drawing.Point(197, 16);
            this.txtConfigName.Name = "txtConfigName";
            this.txtConfigName.Size = new System.Drawing.Size(510, 22);
            this.txtConfigName.TabIndex = 1;
            // 
            // grp1
            // 
            this.grp1.Controls.Add(this.grp5);
            this.grp1.Controls.Add(this.grp4);
            this.grp1.Controls.Add(this.grp3);
            this.grp1.Controls.Add(this.grp2);
            this.grp1.Controls.Add(this.label1);
            this.grp1.Controls.Add(this.txtConfigName);
            this.grp1.Location = new System.Drawing.Point(4, -5);
            this.grp1.Name = "grp1";
            this.grp1.Size = new System.Drawing.Size(751, 581);
            this.grp1.TabIndex = 2;
            this.grp1.TabStop = false;
            // 
            // grp5
            // 
            this.grp5.Controls.Add(this.btnAdd);
            this.grp5.Controls.Add(this.btnDelete);
            this.grp5.Controls.Add(this.dgvAgeRangs);
            this.grp5.Location = new System.Drawing.Point(408, 36);
            this.grp5.Name = "grp5";
            this.grp5.Size = new System.Drawing.Size(332, 538);
            this.grp5.TabIndex = 5;
            this.grp5.TabStop = false;
            this.grp5.Text = "Age Ranges";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(250, 504);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 27);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(169, 504);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvAgeRangs
            // 
            this.dgvAgeRangs.AllowUserToAddRows = false;
            this.dgvAgeRangs.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvAgeRangs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAgeRangs.Enabled = false;
            this.dgvAgeRangs.Location = new System.Drawing.Point(6, 23);
            this.dgvAgeRangs.Name = "dgvAgeRangs";
            this.dgvAgeRangs.RowTemplate.Height = 23;
            this.dgvAgeRangs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAgeRangs.Size = new System.Drawing.Size(319, 475);
            this.dgvAgeRangs.TabIndex = 0;
            // 
            // grp4
            // 
            this.grp4.Controls.Add(this.grp9);
            this.grp4.Controls.Add(this.btnAddEthnicity);
            this.grp4.Controls.Add(this.grpEthnicities);
            this.grp4.Location = new System.Drawing.Point(9, 392);
            this.grp4.Name = "grp4";
            this.grp4.Size = new System.Drawing.Size(385, 182);
            this.grp4.TabIndex = 4;
            this.grp4.TabStop = false;
            this.grp4.Text = "Ethnicity";
            // 
            // grp9
            // 
            this.grp9.Controls.Add(this.lstAvailableEthnicity);
            this.grp9.Location = new System.Drawing.Point(4, 20);
            this.grp9.Name = "grp9";
            this.grp9.Size = new System.Drawing.Size(158, 157);
            this.grp9.TabIndex = 0;
            this.grp9.TabStop = false;
            this.grp9.Text = "Available Ethnicity";
            // 
            // lstAvailableEthnicity
            // 
            this.lstAvailableEthnicity.FormattingEnabled = true;
            this.lstAvailableEthnicity.ItemHeight = 14;
            this.lstAvailableEthnicity.Location = new System.Drawing.Point(6, 23);
            this.lstAvailableEthnicity.Name = "lstAvailableEthnicity";
            this.lstAvailableEthnicity.Size = new System.Drawing.Size(143, 130);
            this.lstAvailableEthnicity.TabIndex = 0;
            // 
            // btnAddEthnicity
            // 
            this.btnAddEthnicity.Image = ((System.Drawing.Image)(resources.GetObject("btnAddEthnicity.Image")));
            this.btnAddEthnicity.Location = new System.Drawing.Point(167, 94);
            this.btnAddEthnicity.Name = "btnAddEthnicity";
            this.btnAddEthnicity.Size = new System.Drawing.Size(53, 30);
            this.btnAddEthnicity.TabIndex = 2;
            this.btnAddEthnicity.UseVisualStyleBackColor = true;
            this.btnAddEthnicity.Click += new System.EventHandler(this.btnAddEthnicity_Click);
            // 
            // grpEthnicities
            // 
            this.grpEthnicities.Controls.Add(this.btnEthnicitiesRemove);
            this.grpEthnicities.Controls.Add(this.btnEthnicitiesNew);
            this.grpEthnicities.Controls.Add(this.lstEthnicities);
            this.grpEthnicities.Location = new System.Drawing.Point(226, 20);
            this.grpEthnicities.Name = "grpEthnicities";
            this.grpEthnicities.Size = new System.Drawing.Size(153, 157);
            this.grpEthnicities.TabIndex = 0;
            this.grpEthnicities.TabStop = false;
            this.grpEthnicities.Text = "Ethnicities";
            // 
            // btnEthnicitiesRemove
            // 
            this.btnEthnicitiesRemove.Location = new System.Drawing.Point(84, 126);
            this.btnEthnicitiesRemove.Name = "btnEthnicitiesRemove";
            this.btnEthnicitiesRemove.Size = new System.Drawing.Size(63, 27);
            this.btnEthnicitiesRemove.TabIndex = 2;
            this.btnEthnicitiesRemove.Text = "Remove";
            this.btnEthnicitiesRemove.UseVisualStyleBackColor = true;
            this.btnEthnicitiesRemove.Click += new System.EventHandler(this.btnEthnicitiesRemove_Click);
            // 
            // btnEthnicitiesNew
            // 
            this.btnEthnicitiesNew.Location = new System.Drawing.Point(16, 126);
            this.btnEthnicitiesNew.Name = "btnEthnicitiesNew";
            this.btnEthnicitiesNew.Size = new System.Drawing.Size(52, 27);
            this.btnEthnicitiesNew.TabIndex = 3;
            this.btnEthnicitiesNew.Text = "New";
            this.btnEthnicitiesNew.UseVisualStyleBackColor = true;
            this.btnEthnicitiesNew.Click += new System.EventHandler(this.btnEthnicitiesNew_Click);
            // 
            // lstEthnicities
            // 
            this.lstEthnicities.FormattingEnabled = true;
            this.lstEthnicities.ItemHeight = 14;
            this.lstEthnicities.Location = new System.Drawing.Point(6, 20);
            this.lstEthnicities.Name = "lstEthnicities";
            this.lstEthnicities.Size = new System.Drawing.Size(141, 102);
            this.lstEthnicities.TabIndex = 1;
            // 
            // grp3
            // 
            this.grp3.Controls.Add(this.grp8);
            this.grp3.Controls.Add(this.btnAddGender);
            this.grp3.Controls.Add(this.grpGenders);
            this.grp3.Location = new System.Drawing.Point(9, 213);
            this.grp3.Name = "grp3";
            this.grp3.Size = new System.Drawing.Size(385, 178);
            this.grp3.TabIndex = 3;
            this.grp3.TabStop = false;
            this.grp3.Text = "Gender";
            // 
            // grp8
            // 
            this.grp8.Controls.Add(this.lstAvailableGrnders);
            this.grp8.Location = new System.Drawing.Point(6, 17);
            this.grp8.Name = "grp8";
            this.grp8.Size = new System.Drawing.Size(158, 157);
            this.grp8.TabIndex = 0;
            this.grp8.TabStop = false;
            this.grp8.Text = "Available Genders";
            // 
            // lstAvailableGrnders
            // 
            this.lstAvailableGrnders.FormattingEnabled = true;
            this.lstAvailableGrnders.ItemHeight = 14;
            this.lstAvailableGrnders.Location = new System.Drawing.Point(6, 23);
            this.lstAvailableGrnders.Name = "lstAvailableGrnders";
            this.lstAvailableGrnders.Size = new System.Drawing.Size(143, 130);
            this.lstAvailableGrnders.TabIndex = 0;
            // 
            // btnAddGender
            // 
            this.btnAddGender.Image = ((System.Drawing.Image)(resources.GetObject("btnAddGender.Image")));
            this.btnAddGender.Location = new System.Drawing.Point(167, 77);
            this.btnAddGender.Name = "btnAddGender";
            this.btnAddGender.Size = new System.Drawing.Size(53, 31);
            this.btnAddGender.TabIndex = 3;
            this.btnAddGender.UseVisualStyleBackColor = true;
            this.btnAddGender.Click += new System.EventHandler(this.btnAddGender_Click);
            // 
            // grpGenders
            // 
            this.grpGenders.Controls.Add(this.btnGendersRemove);
            this.grpGenders.Controls.Add(this.btnGendersNew);
            this.grpGenders.Controls.Add(this.lstGenders);
            this.grpGenders.Location = new System.Drawing.Point(226, 17);
            this.grpGenders.Name = "grpGenders";
            this.grpGenders.Size = new System.Drawing.Size(153, 157);
            this.grpGenders.TabIndex = 1;
            this.grpGenders.TabStop = false;
            this.grpGenders.Text = "Genders";
            // 
            // btnGendersRemove
            // 
            this.btnGendersRemove.Location = new System.Drawing.Point(84, 125);
            this.btnGendersRemove.Name = "btnGendersRemove";
            this.btnGendersRemove.Size = new System.Drawing.Size(63, 27);
            this.btnGendersRemove.TabIndex = 2;
            this.btnGendersRemove.Text = "Remove";
            this.btnGendersRemove.UseVisualStyleBackColor = true;
            this.btnGendersRemove.Click += new System.EventHandler(this.btnGendersRemove_Click);
            // 
            // btnGendersNew
            // 
            this.btnGendersNew.Location = new System.Drawing.Point(16, 125);
            this.btnGendersNew.Name = "btnGendersNew";
            this.btnGendersNew.Size = new System.Drawing.Size(52, 27);
            this.btnGendersNew.TabIndex = 3;
            this.btnGendersNew.Text = "New";
            this.btnGendersNew.UseVisualStyleBackColor = true;
            this.btnGendersNew.Click += new System.EventHandler(this.btnGendersNew_Click);
            // 
            // lstGenders
            // 
            this.lstGenders.FormattingEnabled = true;
            this.lstGenders.ItemHeight = 14;
            this.lstGenders.Location = new System.Drawing.Point(6, 18);
            this.lstGenders.Name = "lstGenders";
            this.lstGenders.Size = new System.Drawing.Size(141, 102);
            this.lstGenders.TabIndex = 1;
            // 
            // grp2
            // 
            this.grp2.Controls.Add(this.grp7);
            this.grp2.Controls.Add(this.btnAddRace);
            this.grp2.Controls.Add(this.grpRace);
            this.grp2.Location = new System.Drawing.Point(10, 36);
            this.grp2.Name = "grp2";
            this.grp2.Size = new System.Drawing.Size(385, 178);
            this.grp2.TabIndex = 2;
            this.grp2.TabStop = false;
            this.grp2.Text = "Race";
            // 
            // grp7
            // 
            this.grp7.Controls.Add(this.lstAvailableRaces);
            this.grp7.Location = new System.Drawing.Point(6, 15);
            this.grp7.Name = "grp7";
            this.grp7.Size = new System.Drawing.Size(158, 157);
            this.grp7.TabIndex = 0;
            this.grp7.TabStop = false;
            this.grp7.Text = "Available Races";
            // 
            // lstAvailableRaces
            // 
            this.lstAvailableRaces.FormattingEnabled = true;
            this.lstAvailableRaces.ItemHeight = 14;
            this.lstAvailableRaces.Location = new System.Drawing.Point(6, 23);
            this.lstAvailableRaces.Name = "lstAvailableRaces";
            this.lstAvailableRaces.Size = new System.Drawing.Size(143, 130);
            this.lstAvailableRaces.TabIndex = 0;
            // 
            // btnAddRace
            // 
            this.btnAddRace.Image = ((System.Drawing.Image)(resources.GetObject("btnAddRace.Image")));
            this.btnAddRace.Location = new System.Drawing.Point(170, 68);
            this.btnAddRace.Name = "btnAddRace";
            this.btnAddRace.Size = new System.Drawing.Size(50, 31);
            this.btnAddRace.TabIndex = 3;
            this.btnAddRace.UseVisualStyleBackColor = true;
            this.btnAddRace.Click += new System.EventHandler(this.btnAddRace_Click);
            // 
            // grpRace
            // 
            this.grpRace.Controls.Add(this.btnRaceNew);
            this.grpRace.Controls.Add(this.btnRaceRemove);
            this.grpRace.Controls.Add(this.lstRaces);
            this.grpRace.Location = new System.Drawing.Point(226, 15);
            this.grpRace.Name = "grpRace";
            this.grpRace.Size = new System.Drawing.Size(153, 159);
            this.grpRace.TabIndex = 1;
            this.grpRace.TabStop = false;
            this.grpRace.Text = "Races";
            // 
            // btnRaceNew
            // 
            this.btnRaceNew.Location = new System.Drawing.Point(16, 126);
            this.btnRaceNew.Name = "btnRaceNew";
            this.btnRaceNew.Size = new System.Drawing.Size(52, 27);
            this.btnRaceNew.TabIndex = 3;
            this.btnRaceNew.Text = "New";
            this.btnRaceNew.UseVisualStyleBackColor = true;
            this.btnRaceNew.Click += new System.EventHandler(this.btnRaceNew_Click);
            // 
            // btnRaceRemove
            // 
            this.btnRaceRemove.Location = new System.Drawing.Point(84, 126);
            this.btnRaceRemove.Name = "btnRaceRemove";
            this.btnRaceRemove.Size = new System.Drawing.Size(63, 27);
            this.btnRaceRemove.TabIndex = 2;
            this.btnRaceRemove.Text = "Remove";
            this.btnRaceRemove.UseVisualStyleBackColor = true;
            this.btnRaceRemove.Click += new System.EventHandler(this.btnRaceRemove_Click);
            // 
            // lstRaces
            // 
            this.lstRaces.FormattingEnabled = true;
            this.lstRaces.ItemHeight = 14;
            this.lstRaces.Location = new System.Drawing.Point(6, 16);
            this.lstRaces.Name = "lstRaces";
            this.lstRaces.Size = new System.Drawing.Size(141, 102);
            this.lstRaces.TabIndex = 1;
            // 
            // grp6
            // 
            this.grp6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grp6.Controls.Add(this.btnOK);
            this.grp6.Controls.Add(this.btnCancel);
            this.grp6.Location = new System.Drawing.Point(4, 576);
            this.grp6.Name = "grp6";
            this.grp6.Size = new System.Drawing.Size(751, 51);
            this.grp6.TabIndex = 3;
            this.grp6.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(658, 18);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(577, 17);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PopulationConfigurationDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(760, 629);
            this.Controls.Add(this.grp6);
            this.Controls.Add(this.grp1);
            this.Name = "PopulationConfigurationDefinition";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Population Configuration Definition";
            this.Load += new System.EventHandler(this.PopulationConfigurationDefinition_Load);
            this.grp1.ResumeLayout(false);
            this.grp1.PerformLayout();
            this.grp5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAgeRangs)).EndInit();
            this.grp4.ResumeLayout(false);
            this.grp9.ResumeLayout(false);
            this.grpEthnicities.ResumeLayout(false);
            this.grp3.ResumeLayout(false);
            this.grp8.ResumeLayout(false);
            this.grpGenders.ResumeLayout(false);
            this.grp2.ResumeLayout(false);
            this.grp7.ResumeLayout(false);
            this.grpRace.ResumeLayout(false);
            this.grp6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtConfigName;
        private System.Windows.Forms.GroupBox grp1;
        private System.Windows.Forms.GroupBox grp5;
        private System.Windows.Forms.GroupBox grp4;
        private System.Windows.Forms.ListBox lstEthnicities;
        private System.Windows.Forms.ListBox lstAvailableEthnicity;
        private System.Windows.Forms.GroupBox grp3;
        private System.Windows.Forms.ListBox lstGenders;
        private System.Windows.Forms.ListBox lstAvailableGrnders;
        private System.Windows.Forms.GroupBox grp2;
        private System.Windows.Forms.ListBox lstRaces;
        private System.Windows.Forms.ListBox lstAvailableRaces;
        private System.Windows.Forms.GroupBox grp6;
        private System.Windows.Forms.GroupBox grpGenders;
        private System.Windows.Forms.GroupBox grpEthnicities;
        private System.Windows.Forms.GroupBox grpRace;
        private System.Windows.Forms.Button btnEthnicitiesNew;
        private System.Windows.Forms.Button btnEthnicitiesRemove;
        private System.Windows.Forms.Button btnGendersNew;
        private System.Windows.Forms.Button btnGendersRemove;
        private System.Windows.Forms.Button btnRaceNew;
        private System.Windows.Forms.Button btnRaceRemove;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddEthnicity;
        private System.Windows.Forms.Button btnAddGender;
        private System.Windows.Forms.Button btnAddRace;
        private System.Windows.Forms.GroupBox grp9;
        private System.Windows.Forms.GroupBox grp8;
        private System.Windows.Forms.GroupBox grp7;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridView dgvAgeRangs;
    }
}