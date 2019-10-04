namespace BenMAP
{
    partial class LoadAirQualityData
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboSetup = new System.Windows.Forms.ComboBox();
            this.cboPollutant = new System.Windows.Forms.ComboBox();
            this.cboGridType = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F);
            this.label1.Location = new System.Drawing.Point(18, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Setup:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F);
            this.label2.Location = new System.Drawing.Point(18, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pollutant:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F);
            this.label3.Location = new System.Drawing.Point(18, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "Grid Type:";
            // 
            // cboSetup
            // 
            this.cboSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSetup.Font = new System.Drawing.Font("Calibri", 12F);
            this.cboSetup.FormattingEnabled = true;
            this.cboSetup.Location = new System.Drawing.Point(100, 24);
            this.cboSetup.Name = "cboSetup";
            this.cboSetup.Size = new System.Drawing.Size(205, 27);
            this.cboSetup.TabIndex = 1;
            this.cboSetup.SelectedIndexChanged += new System.EventHandler(this.cboSetup_SelectedIndexChanged);
            // 
            // cboPollutant
            // 
            this.cboPollutant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPollutant.Font = new System.Drawing.Font("Calibri", 12F);
            this.cboPollutant.FormattingEnabled = true;
            this.cboPollutant.Location = new System.Drawing.Point(100, 72);
            this.cboPollutant.Name = "cboPollutant";
            this.cboPollutant.Size = new System.Drawing.Size(205, 27);
            this.cboPollutant.TabIndex = 2;
            // 
            // cboGridType
            // 
            this.cboGridType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridType.Font = new System.Drawing.Font("Calibri", 12F);
            this.cboGridType.FormattingEnabled = true;
            this.cboGridType.Location = new System.Drawing.Point(100, 120);
            this.cboGridType.Name = "cboGridType";
            this.cboGridType.Size = new System.Drawing.Size(205, 27);
            this.cboGridType.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Calibri", 12F);
            this.btnOK.Location = new System.Drawing.Point(230, 174);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Calibri", 12F);
            this.btnCancel.Location = new System.Drawing.Point(149, 174);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LoadAirQualityData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 218);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboGridType);
            this.Controls.Add(this.cboPollutant);
            this.Controls.Add(this.cboSetup);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "LoadAirQualityData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Air Quality Data";
            this.Load += new System.EventHandler(this.LoadAirQualityData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboSetup;
        private System.Windows.Forms.ComboBox cboPollutant;
        private System.Windows.Forms.ComboBox cboGridType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}