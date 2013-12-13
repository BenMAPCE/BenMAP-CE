namespace BenMAP
{
    partial class PopulationInformation
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
            this.lstLayers = new System.Windows.Forms.ListBox();
            this.dgvInfo = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLayers
            // 
            this.lstLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLayers.FormattingEnabled = true;
            this.lstLayers.ItemHeight = 14;
            this.lstLayers.Location = new System.Drawing.Point(3, 18);
            this.lstLayers.Name = "lstLayers";
            this.lstLayers.Size = new System.Drawing.Size(126, 392);
            this.lstLayers.TabIndex = 0;
            this.lstLayers.SelectedIndexChanged += new System.EventHandler(this.lstLayers_SelectedIndexChanged);
            // 
            // dgvInfo
            // 
            this.dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInfo.Location = new System.Drawing.Point(3, 18);
            this.dgvInfo.Name = "dgvInfo";
            this.dgvInfo.RowTemplate.Height = 23;
            this.dgvInfo.Size = new System.Drawing.Size(277, 392);
            this.dgvInfo.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstLayers);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(132, 413);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Layers";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvInfo);
            this.groupBox2.Location = new System.Drawing.Point(138, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(283, 413);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Informations";
            // 
            // PopulationInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 413);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PopulationInformation";
            this.Text = "Population Information";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PopulationInformation_FormClosed);
            this.Load += new System.EventHandler(this.PopulationInformation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstLayers;
        private System.Windows.Forms.DataGridView dgvInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}