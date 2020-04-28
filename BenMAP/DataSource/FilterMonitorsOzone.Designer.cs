namespace BenMAP
{
	partial class FilterMonitorsOzone
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.nudownNumberOfValidHours = new System.Windows.Forms.NumericUpDown();
			this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
			this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
			this.nudownNumberOfValidDays = new System.Windows.Forms.NumericUpDown();
			this.numDownStartHour = new System.Windows.Forms.NumericUpDown();
			this.numDownEndHour = new System.Windows.Forms.NumericUpDown();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.nudownNumberOfValidHours)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudownNumberOfValidDays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numDownStartHour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numDownEndHour)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Start Hour:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "End Hour:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 85);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(116, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Number of Valid Hours:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 135);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Start Date:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 167);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "End Date:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(13, 194);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Precent of Valid Days:";
			// 
			// nudownNumberOfValidHours
			// 
			this.nudownNumberOfValidHours.Location = new System.Drawing.Point(167, 82);
			this.nudownNumberOfValidHours.Maximum = new decimal(new int[] {
						23,
						0,
						0,
						0});
			this.nudownNumberOfValidHours.Name = "nudownNumberOfValidHours";
			this.nudownNumberOfValidHours.Size = new System.Drawing.Size(78, 20);
			this.nudownNumberOfValidHours.TabIndex = 3;
			// 
			// dtpEndTime
			// 
			this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpEndTime.Location = new System.Drawing.Point(144, 160);
			this.dtpEndTime.Name = "dtpEndTime";
			this.dtpEndTime.ShowUpDown = true;
			this.dtpEndTime.Size = new System.Drawing.Size(101, 20);
			this.dtpEndTime.TabIndex = 5;
			// 
			// dtpStartTime
			// 
			this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpStartTime.Location = new System.Drawing.Point(144, 129);
			this.dtpStartTime.Name = "dtpStartTime";
			this.dtpStartTime.ShowUpDown = true;
			this.dtpStartTime.Size = new System.Drawing.Size(101, 20);
			this.dtpStartTime.TabIndex = 4;
			// 
			// nudownNumberOfValidDays
			// 
			this.nudownNumberOfValidDays.Location = new System.Drawing.Point(167, 192);
			this.nudownNumberOfValidDays.Name = "nudownNumberOfValidDays";
			this.nudownNumberOfValidDays.Size = new System.Drawing.Size(78, 20);
			this.nudownNumberOfValidDays.TabIndex = 6;
			// 
			// numDownStartHour
			// 
			this.numDownStartHour.Location = new System.Drawing.Point(144, 22);
			this.numDownStartHour.Maximum = new decimal(new int[] {
						23,
						0,
						0,
						0});
			this.numDownStartHour.Name = "numDownStartHour";
			this.numDownStartHour.Size = new System.Drawing.Size(101, 20);
			this.numDownStartHour.TabIndex = 1;
			// 
			// numDownEndHour
			// 
			this.numDownEndHour.Location = new System.Drawing.Point(144, 52);
			this.numDownEndHour.Maximum = new decimal(new int[] {
						23,
						0,
						0,
						0});
			this.numDownEndHour.Name = "numDownEndHour";
			this.numDownEndHour.Size = new System.Drawing.Size(101, 20);
			this.numDownEndHour.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dtpEndTime);
			this.groupBox1.Controls.Add(this.dtpStartTime);
			this.groupBox1.Controls.Add(this.numDownEndHour);
			this.groupBox1.Controls.Add(this.numDownStartHour);
			this.groupBox1.Controls.Add(this.nudownNumberOfValidDays);
			this.groupBox1.Controls.Add(this.nudownNumberOfValidHours);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(260, 235);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// FilterMonitorsOzone
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "FilterMonitorsOzone";
			this.Size = new System.Drawing.Size(271, 250);
			((System.ComponentModel.ISupportInitialize)(this.nudownNumberOfValidHours)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudownNumberOfValidDays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numDownStartHour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numDownEndHour)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.NumericUpDown nudownNumberOfValidHours;
		public System.Windows.Forms.DateTimePicker dtpEndTime;
		public System.Windows.Forms.DateTimePicker dtpStartTime;
		public System.Windows.Forms.NumericUpDown nudownNumberOfValidDays;
		public System.Windows.Forms.NumericUpDown numDownStartHour;
		public System.Windows.Forms.NumericUpDown numDownEndHour;
	}
}
