namespace BenMAP
{
    partial class SelectAirQualityGrid
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
            this.rbtnUseOldAirQualityGrid = new System.Windows.Forms.RadioButton();
            this.rbtnUseNewAirQualityGrid = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
                                                this.groupBox1.Controls.Add(this.rbtnUseOldAirQualityGrid);
            this.groupBox1.Controls.Add(this.rbtnUseNewAirQualityGrid);
            this.groupBox1.Location = new System.Drawing.Point(12, -3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(223, 117);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
                                                this.rbtnUseOldAirQualityGrid.AutoSize = true;
            this.rbtnUseOldAirQualityGrid.Location = new System.Drawing.Point(7, 72);
            this.rbtnUseOldAirQualityGrid.Name = "rbtnUseOldAirQualityGrid";
            this.rbtnUseOldAirQualityGrid.Size = new System.Drawing.Size(167, 16);
            this.rbtnUseOldAirQualityGrid.TabIndex = 1;
            this.rbtnUseOldAirQualityGrid.TabStop = true;
            this.rbtnUseOldAirQualityGrid.Text = "use old air quality grid";
            this.rbtnUseOldAirQualityGrid.UseVisualStyleBackColor = true;
                                                this.rbtnUseNewAirQualityGrid.AutoSize = true;
            this.rbtnUseNewAirQualityGrid.Location = new System.Drawing.Point(7, 24);
            this.rbtnUseNewAirQualityGrid.Name = "rbtnUseNewAirQualityGrid";
            this.rbtnUseNewAirQualityGrid.Size = new System.Drawing.Size(167, 16);
            this.rbtnUseNewAirQualityGrid.TabIndex = 0;
            this.rbtnUseNewAirQualityGrid.TabStop = true;
            this.rbtnUseNewAirQualityGrid.Text = "use new air quality grid";
            this.rbtnUseNewAirQualityGrid.UseVisualStyleBackColor = true;
                                                this.groupBox2.Controls.Add(this.btnGo);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Location = new System.Drawing.Point(246, -3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(116, 117);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
                                                this.btnGo.Location = new System.Drawing.Point(18, 72);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 27);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
                                                this.btnCancel.Location = new System.Drawing.Point(18, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 122);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SelectAirQualityGrid";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Air Quality Grid";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnUseOldAirQualityGrid;
        private System.Windows.Forms.RadioButton rbtnUseNewAirQualityGrid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnCancel;
    }
}