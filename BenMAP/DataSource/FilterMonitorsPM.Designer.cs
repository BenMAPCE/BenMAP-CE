namespace BenMAP
{
    partial class FilterMonitorsPM
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
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rbtnOutputStandard = new System.Windows.Forms.RadioButton();
            this.rbtnOutputLocal = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbtnStandardPreferred = new System.Windows.Forms.RadioButton();
            this.rbtnPreferredLocal = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbtnBoth = new System.Windows.Forms.RadioButton();
            this.rbtnStandard = new System.Windows.Forms.RadioButton();
            this.rbtnLocal = new System.Windows.Forms.RadioButton();
            this.nudownNumberOfValidObservations = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownNumberOfValidObservations)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
                                                this.groupBox6.Controls.Add(this.rbtnOutputStandard);
            this.groupBox6.Controls.Add(this.rbtnOutputLocal);
            this.groupBox6.Location = new System.Drawing.Point(8, 171);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(261, 50);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Output Type";
                                                this.rbtnOutputStandard.AutoSize = true;
            this.rbtnOutputStandard.Location = new System.Drawing.Point(91, 20);
            this.rbtnOutputStandard.Name = "rbtnOutputStandard";
            this.rbtnOutputStandard.Size = new System.Drawing.Size(71, 16);
            this.rbtnOutputStandard.TabIndex = 1;
            this.rbtnOutputStandard.TabStop = true;
            this.rbtnOutputStandard.Text = "Standard";
            this.rbtnOutputStandard.UseVisualStyleBackColor = true;
                                                this.rbtnOutputLocal.AutoSize = true;
            this.rbtnOutputLocal.Location = new System.Drawing.Point(6, 20);
            this.rbtnOutputLocal.Name = "rbtnOutputLocal";
            this.rbtnOutputLocal.Size = new System.Drawing.Size(53, 16);
            this.rbtnOutputLocal.TabIndex = 0;
            this.rbtnOutputLocal.TabStop = true;
            this.rbtnOutputLocal.Text = "Local";
            this.rbtnOutputLocal.UseVisualStyleBackColor = true;
                                                this.groupBox5.Controls.Add(this.rbtnStandardPreferred);
            this.groupBox5.Controls.Add(this.rbtnPreferredLocal);
            this.groupBox5.Location = new System.Drawing.Point(8, 115);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(261, 50);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Preferred Type";
                                                this.rbtnStandardPreferred.AutoSize = true;
            this.rbtnStandardPreferred.Location = new System.Drawing.Point(91, 20);
            this.rbtnStandardPreferred.Name = "rbtnStandardPreferred";
            this.rbtnStandardPreferred.Size = new System.Drawing.Size(71, 16);
            this.rbtnStandardPreferred.TabIndex = 1;
            this.rbtnStandardPreferred.TabStop = true;
            this.rbtnStandardPreferred.Text = "Standard";
            this.rbtnStandardPreferred.UseVisualStyleBackColor = true;
                                                this.rbtnPreferredLocal.AutoSize = true;
            this.rbtnPreferredLocal.Location = new System.Drawing.Point(6, 20);
            this.rbtnPreferredLocal.Name = "rbtnPreferredLocal";
            this.rbtnPreferredLocal.Size = new System.Drawing.Size(53, 16);
            this.rbtnPreferredLocal.TabIndex = 0;
            this.rbtnPreferredLocal.TabStop = true;
            this.rbtnPreferredLocal.Text = "Local";
            this.rbtnPreferredLocal.UseVisualStyleBackColor = true;
                                                this.groupBox4.Controls.Add(this.rbtnBoth);
            this.groupBox4.Controls.Add(this.rbtnStandard);
            this.groupBox4.Controls.Add(this.rbtnLocal);
            this.groupBox4.Location = new System.Drawing.Point(8, 59);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(261, 50);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Data Types To Use";
                                                this.rbtnBoth.AutoSize = true;
            this.rbtnBoth.Location = new System.Drawing.Point(195, 20);
            this.rbtnBoth.Name = "rbtnBoth";
            this.rbtnBoth.Size = new System.Drawing.Size(47, 16);
            this.rbtnBoth.TabIndex = 2;
            this.rbtnBoth.TabStop = true;
            this.rbtnBoth.Text = "Both";
            this.rbtnBoth.UseVisualStyleBackColor = true;
                                                this.rbtnStandard.AutoSize = true;
            this.rbtnStandard.Location = new System.Drawing.Point(91, 20);
            this.rbtnStandard.Name = "rbtnStandard";
            this.rbtnStandard.Size = new System.Drawing.Size(71, 16);
            this.rbtnStandard.TabIndex = 1;
            this.rbtnStandard.TabStop = true;
            this.rbtnStandard.Text = "Standard";
            this.rbtnStandard.UseVisualStyleBackColor = true;
                                                this.rbtnLocal.AutoSize = true;
            this.rbtnLocal.Location = new System.Drawing.Point(6, 20);
            this.rbtnLocal.Name = "rbtnLocal";
            this.rbtnLocal.Size = new System.Drawing.Size(53, 16);
            this.rbtnLocal.TabIndex = 0;
            this.rbtnLocal.TabStop = true;
            this.rbtnLocal.Text = "Local";
            this.rbtnLocal.UseVisualStyleBackColor = true;
                                                this.nudownNumberOfValidObservations.Location = new System.Drawing.Point(191, 20);
            this.nudownNumberOfValidObservations.Name = "nudownNumberOfValidObservations";
            this.nudownNumberOfValidObservations.Size = new System.Drawing.Size(78, 21);
            this.nudownNumberOfValidObservations.TabIndex = 1;
                                                this.label20.Location = new System.Drawing.Point(6, 17);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(180, 39);
            this.label20.TabIndex = 0;
            this.label20.Text = "Number of Valid Observations Required Per Quarter:";
                                                this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.nudownNumberOfValidObservations);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Location = new System.Drawing.Point(3, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 228);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FilterMonitorsPM";
            this.Size = new System.Drawing.Size(285, 238);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudownNumberOfValidObservations)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.RadioButton rbtnOutputStandard;
        public System.Windows.Forms.RadioButton rbtnOutputLocal;
        public System.Windows.Forms.RadioButton rbtnStandardPreferred;
        public System.Windows.Forms.RadioButton rbtnPreferredLocal;
        public System.Windows.Forms.RadioButton rbtnBoth;
        public System.Windows.Forms.RadioButton rbtnStandard;
        public System.Windows.Forms.RadioButton rbtnLocal;
        public System.Windows.Forms.NumericUpDown nudownNumberOfValidObservations;
    }
}
