namespace BenMAP
{
    partial class GridType
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
            this.cboGrid = new System.Windows.Forms.ComboBox();
            this.cboRegion = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblGrid = new System.Windows.Forms.Label();
            this.lblRegion = new System.Windows.Forms.Label();
            this.lblTip = new System.Windows.Forms.Label();
            this.SuspendLayout();
                                                this.cboGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGrid.FormattingEnabled = true;
            this.cboGrid.Location = new System.Drawing.Point(65, 37);
            this.cboGrid.Name = "cboGrid";
            this.cboGrid.Size = new System.Drawing.Size(217, 22);
            this.cboGrid.TabIndex = 13;
            this.cboGrid.SelectedValueChanged += new System.EventHandler(this.cboGrid_SelectedValueChanged);
                                                this.cboRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRegion.FormattingEnabled = true;
            this.cboRegion.Location = new System.Drawing.Point(65, 79);
            this.cboRegion.Name = "cboRegion";
            this.cboRegion.Size = new System.Drawing.Size(217, 22);
            this.cboRegion.TabIndex = 12;
                                                this.btnCancel.Location = new System.Drawing.Point(123, 116);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.btnOK.Location = new System.Drawing.Point(210, 116);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.lblGrid.AutoSize = true;
            this.lblGrid.Location = new System.Drawing.Point(12, 40);
            this.lblGrid.Name = "lblGrid";
            this.lblGrid.Size = new System.Drawing.Size(33, 14);
            this.lblGrid.TabIndex = 3;
            this.lblGrid.Text = "Grid:";
                                                this.lblRegion.AutoSize = true;
            this.lblRegion.Location = new System.Drawing.Point(12, 82);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(53, 14);
            this.lblRegion.TabIndex = 2;
            this.lblRegion.Text = "Domain:";
                                                this.lblTip.AutoSize = true;
            this.lblTip.Location = new System.Drawing.Point(12, 9);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(145, 14);
            this.lblTip.TabIndex = 0;
            this.lblTip.Text = "Please choose Grid Type :";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 155);
            this.Controls.Add(this.cboGrid);
            this.Controls.Add(this.cboRegion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblGrid);
            this.Controls.Add(this.lblRegion);
            this.Controls.Add(this.lblTip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GridType";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Air Quality Surface Type";
            this.Load += new System.EventHandler(this.GridType_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.Label lblTip;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.Label lblGrid;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboRegion;
        private System.Windows.Forms.ComboBox cboGrid;
    }
}