namespace BenMAP
{
    partial class DefineSeasons
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
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grp1 = new System.Windows.Forms.GroupBox();
            this.grpSelectedSeasonDetails = new System.Windows.Forms.GroupBox();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.nudownNumberofBins = new System.Windows.Forms.NumericUpDown();
            this.nudownEndHour = new System.Windows.Forms.NumericUpDown();
            this.nudownStartHour = new System.Windows.Forms.NumericUpDown();
            this.lblNumberofBins = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblStartHour = new System.Windows.Forms.Label();
            this.lblEndData = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.grpSeasons = new System.Windows.Forms.GroupBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstSeasons = new System.Windows.Forms.ListBox();
            this.grpCancelOK.SuspendLayout();
            this.grp1.SuspendLayout();
            this.grpSelectedSeasonDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownNumberofBins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownEndHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownStartHour)).BeginInit();
            this.grpSeasons.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(13, 246);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(440, 59);
            this.grpCancelOK.TabIndex = 2;
            this.grpCancelOK.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(358, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(277, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grp1
            // 
            this.grp1.Controls.Add(this.grpSelectedSeasonDetails);
            this.grp1.Controls.Add(this.grpSeasons);
            this.grp1.Location = new System.Drawing.Point(13, 1);
            this.grp1.Name = "grp1";
            this.grp1.Size = new System.Drawing.Size(440, 246);
            this.grp1.TabIndex = 0;
            this.grp1.TabStop = false;
            // 
            // grpSelectedSeasonDetails
            // 
            this.grpSelectedSeasonDetails.Controls.Add(this.dtpEndTime);
            this.grpSelectedSeasonDetails.Controls.Add(this.dtpStartTime);
            this.grpSelectedSeasonDetails.Controls.Add(this.nudownNumberofBins);
            this.grpSelectedSeasonDetails.Controls.Add(this.nudownEndHour);
            this.grpSelectedSeasonDetails.Controls.Add(this.nudownStartHour);
            this.grpSelectedSeasonDetails.Controls.Add(this.lblNumberofBins);
            this.grpSelectedSeasonDetails.Controls.Add(this.label4);
            this.grpSelectedSeasonDetails.Controls.Add(this.lblStartHour);
            this.grpSelectedSeasonDetails.Controls.Add(this.lblEndData);
            this.grpSelectedSeasonDetails.Controls.Add(this.lblStartDate);
            this.grpSelectedSeasonDetails.Location = new System.Drawing.Point(215, 14);
            this.grpSelectedSeasonDetails.Name = "grpSelectedSeasonDetails";
            this.grpSelectedSeasonDetails.Size = new System.Drawing.Size(214, 225);
            this.grpSelectedSeasonDetails.TabIndex = 1;
            this.grpSelectedSeasonDetails.TabStop = false;
            this.grpSelectedSeasonDetails.Text = "Selected Season Detail";
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.Location = new System.Drawing.Point(107, 61);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.ShowUpDown = true;
            this.dtpEndTime.Size = new System.Drawing.Size(101, 22);
            this.dtpEndTime.TabIndex = 28;
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(107, 23);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(101, 22);
            this.dtpStartTime.TabIndex = 27;
            // 
            // nudownNumberofBins
            // 
            this.nudownNumberofBins.Location = new System.Drawing.Point(107, 187);
            this.nudownNumberofBins.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.nudownNumberofBins.Name = "nudownNumberofBins";
            this.nudownNumberofBins.Size = new System.Drawing.Size(60, 22);
            this.nudownNumberofBins.TabIndex = 9;
            this.nudownNumberofBins.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudownNumberofBins.Visible = false;
            this.nudownNumberofBins.Leave += new System.EventHandler(this.nudownNumberofBins_Leave);
            // 
            // nudownEndHour
            // 
            this.nudownEndHour.Location = new System.Drawing.Point(107, 148);
            this.nudownEndHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nudownEndHour.Name = "nudownEndHour";
            this.nudownEndHour.Size = new System.Drawing.Size(60, 22);
            this.nudownEndHour.TabIndex = 8;
            this.nudownEndHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudownEndHour.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nudownEndHour.Leave += new System.EventHandler(this.nudownEndHour_Leave);
            // 
            // nudownStartHour
            // 
            this.nudownStartHour.Location = new System.Drawing.Point(107, 106);
            this.nudownStartHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nudownStartHour.Name = "nudownStartHour";
            this.nudownStartHour.Size = new System.Drawing.Size(60, 22);
            this.nudownStartHour.TabIndex = 7;
            this.nudownStartHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudownStartHour.Leave += new System.EventHandler(this.nudownStartHour_Leave);
            // 
            // lblNumberofBins
            // 
            this.lblNumberofBins.AutoSize = true;
            this.lblNumberofBins.Location = new System.Drawing.Point(6, 189);
            this.lblNumberofBins.Name = "lblNumberofBins";
            this.lblNumberofBins.Size = new System.Drawing.Size(94, 14);
            this.lblNumberofBins.TabIndex = 4;
            this.lblNumberofBins.Text = "Number of Bins:";
            this.lblNumberofBins.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 14);
            this.label4.TabIndex = 3;
            this.label4.Text = "End Hour:";
            // 
            // lblStartHour
            // 
            this.lblStartHour.AutoSize = true;
            this.lblStartHour.Location = new System.Drawing.Point(6, 108);
            this.lblStartHour.Name = "lblStartHour";
            this.lblStartHour.Size = new System.Drawing.Size(64, 14);
            this.lblStartHour.TabIndex = 2;
            this.lblStartHour.Text = "Start Hour:";
            // 
            // lblEndData
            // 
            this.lblEndData.AutoSize = true;
            this.lblEndData.Location = new System.Drawing.Point(6, 67);
            this.lblEndData.Name = "lblEndData";
            this.lblEndData.Size = new System.Drawing.Size(59, 14);
            this.lblEndData.TabIndex = 1;
            this.lblEndData.Text = "End Date:";
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(6, 29);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(64, 14);
            this.lblStartDate.TabIndex = 0;
            this.lblStartDate.Text = "Start Date:";
            // 
            // grpSeasons
            // 
            this.grpSeasons.Controls.Add(this.btnAdd);
            this.grpSeasons.Controls.Add(this.btnDelete);
            this.grpSeasons.Controls.Add(this.lstSeasons);
            this.grpSeasons.Location = new System.Drawing.Point(9, 14);
            this.grpSeasons.Name = "grpSeasons";
            this.grpSeasons.Size = new System.Drawing.Size(200, 225);
            this.grpSeasons.TabIndex = 0;
            this.grpSeasons.TabStop = false;
            this.grpSeasons.Text = "Seasons";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(119, 191);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 27);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(6, 192);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstSeasons
            // 
            this.lstSeasons.FormattingEnabled = true;
            this.lstSeasons.ItemHeight = 14;
            this.lstSeasons.Location = new System.Drawing.Point(6, 23);
            this.lstSeasons.Name = "lstSeasons";
            this.lstSeasons.Size = new System.Drawing.Size(188, 158);
            this.lstSeasons.TabIndex = 0;
            this.lstSeasons.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstSeasons_MouseClick);
            this.lstSeasons.SelectedIndexChanged += new System.EventHandler(this.lstSeasons_SelectedIndexChanged);
            // 
            // DefineSeasons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 312);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grp1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(469, 340);
            this.Name = "DefineSeasons";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Define Seasons";
            this.Load += new System.EventHandler(this.DefineSeasons_Load);
            this.grpCancelOK.ResumeLayout(false);
            this.grp1.ResumeLayout(false);
            this.grpSelectedSeasonDetails.ResumeLayout(false);
            this.grpSelectedSeasonDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownNumberofBins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownEndHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudownStartHour)).EndInit();
            this.grpSeasons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grp1;
        private System.Windows.Forms.GroupBox grpSelectedSeasonDetails;
        private System.Windows.Forms.NumericUpDown nudownNumberofBins;
        private System.Windows.Forms.NumericUpDown nudownEndHour;
        private System.Windows.Forms.NumericUpDown nudownStartHour;
        private System.Windows.Forms.Label lblNumberofBins;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblStartHour;
        private System.Windows.Forms.Label lblEndData;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.GroupBox grpSeasons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstSeasons;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
    }
}