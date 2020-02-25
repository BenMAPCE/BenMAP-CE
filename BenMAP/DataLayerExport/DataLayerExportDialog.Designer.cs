namespace BenMAP.DataLayerExport
{
    partial class DataLayerExportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataLayerExportDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chlbLayers = new System.Windows.Forms.CheckedListBox();
            this.chlbColumns_HIF = new System.Windows.Forms.CheckedListBox();
            this.tbExportFolder = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.paMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chlbColumns_Pool = new System.Windows.Forms.CheckedListBox();
            this.chlbColumns_Valuation = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboLayerType = new System.Windows.Forms.ComboBox();
            this.pbProgress = new System.Windows.Forms.PictureBox();
            this.lblProgressInfo = new System.Windows.Forms.Label();
            this.paMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Location = new System.Drawing.Point(55, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Layer(s) to Export:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 77);
            this.label2.Name = "label2";
            this.tableLayoutPanel1.SetRowSpan(this.label2, 2);
            this.label2.Size = new System.Drawing.Size(57, 241);
            this.label2.TabIndex = 1;
            this.label2.Text = "Data to Include:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 349);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Export to Folder:";
            // 
            // chlbLayers
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.chlbLayers, 2);
            this.chlbLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chlbLayers.FormattingEnabled = true;
            this.chlbLayers.Location = new System.Drawing.Point(205, 3);
            this.chlbLayers.Name = "chlbLayers";
            this.tableLayoutPanel1.SetRowSpan(this.chlbLayers, 2);
            this.chlbLayers.Size = new System.Drawing.Size(272, 71);
            this.chlbLayers.TabIndex = 2;
            this.chlbLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlbLayers_ItemCheck);
            // 
            // chlbColumns_HIF
            // 
            this.chlbColumns_HIF.CheckOnClick = true;
            this.chlbColumns_HIF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chlbColumns_HIF.FormattingEnabled = true;
            this.chlbColumns_HIF.Location = new System.Drawing.Point(66, 104);
            this.chlbColumns_HIF.Name = "chlbColumns_HIF";
            this.chlbColumns_HIF.Size = new System.Drawing.Size(133, 211);
            this.chlbColumns_HIF.TabIndex = 3;
            // 
            // tbExportFolder
            // 
            this.tbExportFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExportFolder.Location = new System.Drawing.Point(119, 346);
            this.tbExportFolder.Name = "tbExportFolder";
            this.tbExportFolder.Size = new System.Drawing.Size(296, 20);
            this.tbExportFolder.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(421, 344);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(335, 377);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(421, 377);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // paMain
            // 
            this.paMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paMain.Controls.Add(this.tableLayoutPanel1);
            this.paMain.Controls.Add(this.btnSave);
            this.paMain.Controls.Add(this.btnCancel);
            this.paMain.Controls.Add(this.btnBrowse);
            this.paMain.Controls.Add(this.label3);
            this.paMain.Controls.Add(this.tbExportFolder);
            this.paMain.Location = new System.Drawing.Point(12, 12);
            this.paMain.Name = "paMain";
            this.paMain.Size = new System.Drawing.Size(504, 413);
            this.paMain.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.label6, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.chlbColumns_Pool, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.chlbColumns_Valuation, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.chlbLayers, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chlbColumns_HIF, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cboLayerType, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(16, 15);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.84512F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58.20895F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.8806F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 216F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(480, 318);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(344, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 24);
            this.label6.TabIndex = 8;
            this.label6.Text = "Pooled Valuation";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(205, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 24);
            this.label5.TabIndex = 7;
            this.label5.Text = "Pooled Incidence";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chlbColumns_Pool
            // 
            this.chlbColumns_Pool.CheckOnClick = true;
            this.chlbColumns_Pool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chlbColumns_Pool.FormattingEnabled = true;
            this.chlbColumns_Pool.Location = new System.Drawing.Point(205, 104);
            this.chlbColumns_Pool.Name = "chlbColumns_Pool";
            this.chlbColumns_Pool.Size = new System.Drawing.Size(133, 211);
            this.chlbColumns_Pool.TabIndex = 5;
            // 
            // chlbColumns_Valuation
            // 
            this.chlbColumns_Valuation.CheckOnClick = true;
            this.chlbColumns_Valuation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chlbColumns_Valuation.FormattingEnabled = true;
            this.chlbColumns_Valuation.Location = new System.Drawing.Point(344, 104);
            this.chlbColumns_Valuation.Name = "chlbColumns_Valuation";
            this.chlbColumns_Valuation.Size = new System.Drawing.Size(133, 211);
            this.chlbColumns_Valuation.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "Health Impacts";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboLayerType
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.cboLayerType, 2);
            this.cboLayerType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cboLayerType.FormattingEnabled = true;
            this.cboLayerType.Items.AddRange(new object[] {
            "Air Quality",
            "Health Impacts",
            "Pooled Incidence",
            "Pooled Valuation"});
            this.cboLayerType.Location = new System.Drawing.Point(3, 21);
            this.cboLayerType.Name = "cboLayerType";
            this.cboLayerType.Size = new System.Drawing.Size(196, 21);
            this.cboLayerType.TabIndex = 9;
            this.cboLayerType.SelectedIndexChanged += new System.EventHandler(this.cboLayerType_SelectedIndexChanged);
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbProgress.BackColor = System.Drawing.Color.Transparent;
            this.pbProgress.Image = global::BenMAP.Properties.Resources.progressbar;
            this.pbProgress.Location = new System.Drawing.Point(224, 155);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(80, 80);
            this.pbProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbProgress.TabIndex = 10;
            this.pbProgress.TabStop = false;
            // 
            // lblProgressInfo
            // 
            this.lblProgressInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgressInfo.AutoSize = true;
            this.lblProgressInfo.BackColor = System.Drawing.SystemColors.Control;
            this.lblProgressInfo.Location = new System.Drawing.Point(31, 394);
            this.lblProgressInfo.Name = "lblProgressInfo";
            this.lblProgressInfo.Size = new System.Drawing.Size(76, 13);
            this.lblProgressInfo.TabIndex = 1;
            this.lblProgressInfo.Text = "lblProgressInfo";
            // 
            // DataLayerExportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 437);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.lblProgressInfo);
            this.Controls.Add(this.paMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(370, 350);
            this.Name = "DataLayerExportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Data Layer as Shapefile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataLayerExportDialog_FormClosing);
            this.Load += new System.EventHandler(this.DataLayerExportDialog_Load);
            this.paMain.ResumeLayout(false);
            this.paMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox chlbLayers;
        private System.Windows.Forms.CheckedListBox chlbColumns_HIF;
        private System.Windows.Forms.TextBox tbExportFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel paMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pbProgress;
        private System.Windows.Forms.Label lblProgressInfo;
        private System.Windows.Forms.CheckedListBox chlbColumns_Pool;
        private System.Windows.Forms.CheckedListBox chlbColumns_Valuation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboLayerType;
    }
}