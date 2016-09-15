namespace BenMAP
{
    partial class AirQualitySurfaceAggregation
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
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboAggregationSurface = new System.Windows.Forms.ComboBox();
            this.txtAirQualitySurface = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(225, 26);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Air Quality Surface:";
            // 
            // cboAggregationSurface
            // 
            this.cboAggregationSurface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAggregationSurface.FormattingEnabled = true;
            this.cboAggregationSurface.Location = new System.Drawing.Point(14, 82);
            this.cboAggregationSurface.Name = "cboAggregationSurface";
            this.cboAggregationSurface.Size = new System.Drawing.Size(286, 22);
            this.cboAggregationSurface.TabIndex = 2;
            // 
            // txtAirQualitySurface
            // 
            this.txtAirQualitySurface.Location = new System.Drawing.Point(12, 28);
            this.txtAirQualitySurface.Name = "txtAirQualitySurface";
            this.txtAirQualitySurface.ReadOnly = true;
            this.txtAirQualitySurface.Size = new System.Drawing.Size(207, 22);
            this.txtAirQualitySurface.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "Aggregation Surface:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(144, 112);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(225, 112);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // AirQualitySurfaceAggregation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 152);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAirQualitySurface);
            this.Controls.Add(this.cboAggregationSurface);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowse);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AirQualitySurfaceAggregation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aggregate Air Quality Surface";
            this.Load += new System.EventHandler(this.AirQualitySurfaceAggregator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboAggregationSurface;
        private System.Windows.Forms.TextBox txtAirQualitySurface;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}