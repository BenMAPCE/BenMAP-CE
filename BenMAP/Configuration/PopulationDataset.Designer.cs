namespace BenMAP
{
    partial class PopulationDataset
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
            this.lblPopulationDataSet = new System.Windows.Forms.Label();
            this.lblPopulationYear = new System.Windows.Forms.Label();
            this.cboPopulationDataSet = new System.Windows.Forms.ComboBox();
            this.cboPopulationYear = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPopMAP = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPopulationDataSet
            // 
            this.lblPopulationDataSet.AutoSize = true;
            this.lblPopulationDataSet.Location = new System.Drawing.Point(15, 29);
            this.lblPopulationDataSet.Name = "lblPopulationDataSet";
            this.lblPopulationDataSet.Size = new System.Drawing.Size(115, 14);
            this.lblPopulationDataSet.TabIndex = 0;
            this.lblPopulationDataSet.Text = "Population Dataset:";
            // 
            // lblPopulationYear
            // 
            this.lblPopulationYear.AutoSize = true;
            this.lblPopulationYear.Location = new System.Drawing.Point(15, 74);
            this.lblPopulationYear.Name = "lblPopulationYear";
            this.lblPopulationYear.Size = new System.Drawing.Size(95, 14);
            this.lblPopulationYear.TabIndex = 1;
            this.lblPopulationYear.Text = "Population Year:";
            // 
            // cboPopulationDataSet
            // 
            this.cboPopulationDataSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPopulationDataSet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboPopulationDataSet.Location = new System.Drawing.Point(140, 26);
            this.cboPopulationDataSet.Name = "cboPopulationDataSet";
            this.cboPopulationDataSet.Size = new System.Drawing.Size(220, 22);
            this.cboPopulationDataSet.TabIndex = 2;
            this.cboPopulationDataSet.SelectedIndexChanged += new System.EventHandler(this.cboPopulationDataSet_SelectedValueChanged);
            // 
            // cboPopulationYear
            // 
            this.cboPopulationYear.DropDownHeight = 140;
            this.cboPopulationYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPopulationYear.IntegralHeight = false;
            this.cboPopulationYear.Location = new System.Drawing.Point(140, 71);
            this.cboPopulationYear.Name = "cboPopulationYear";
            this.cboPopulationYear.Size = new System.Drawing.Size(220, 22);
            this.cboPopulationYear.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnPopMAP);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(380, 64);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // btnPopMAP
            // 
            this.btnPopMAP.Location = new System.Drawing.Point(12, 21);
            this.btnPopMAP.Name = "btnPopMAP";
            this.btnPopMAP.Size = new System.Drawing.Size(75, 27);
            this.btnPopMAP.TabIndex = 0;
            this.btnPopMAP.Text = "Map";
            this.btnPopMAP.UseVisualStyleBackColor = true;
            this.btnPopMAP.Click += new System.EventHandler(this.btnPopMAP_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(293, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(212, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PopulationDataset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 181);
            this.Controls.Add(this.lblPopulationDataSet);
            this.Controls.Add(this.lblPopulationYear);
            this.Controls.Add(this.cboPopulationDataSet);
            this.Controls.Add(this.cboPopulationYear);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PopulationDataset";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Population Dataset";
            this.Load += new System.EventHandler(this.PopulationDataset_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.Label lblPopulationDataSet;
        private System.Windows.Forms.Label lblPopulationYear;
        private System.Windows.Forms.ComboBox cboPopulationYear;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPopMAP;
        private System.Windows.Forms.ComboBox cboPopulationDataSet;
    }
}