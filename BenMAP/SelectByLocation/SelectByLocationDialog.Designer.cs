namespace BenMAP.SelectByLocation
{
    partial class SelectByLocationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectByLocationDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSelectionMethod = new System.Windows.Forms.ComboBox();
            this.cmbSelectionLayer = new System.Windows.Forms.ComboBox();
            this.cmbTargetLayer = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSpatialSelectionMethod = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.paMain = new System.Windows.Forms.Panel();
            this.pbProgress = new System.Windows.Forms.PictureBox();
            this.paMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selection method:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Selection layer:";
            // 
            // cmbSelectionMethod
            // 
            this.cmbSelectionMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSelectionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectionMethod.FormattingEnabled = true;
            this.cmbSelectionMethod.Location = new System.Drawing.Point(9, 27);
            this.cmbSelectionMethod.Name = "cmbSelectionMethod";
            this.cmbSelectionMethod.Size = new System.Drawing.Size(281, 21);
            this.cmbSelectionMethod.TabIndex = 0;
            // 
            // cmbSelectionLayer
            // 
            this.cmbSelectionLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSelectionLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectionLayer.FormattingEnabled = true;
            this.cmbSelectionLayer.Location = new System.Drawing.Point(9, 77);
            this.cmbSelectionLayer.Name = "cmbSelectionLayer";
            this.cmbSelectionLayer.Size = new System.Drawing.Size(281, 21);
            this.cmbSelectionLayer.TabIndex = 1;
            // 
            // cmbTargetLayer
            // 
            this.cmbTargetLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTargetLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetLayer.FormattingEnabled = true;
            this.cmbTargetLayer.Location = new System.Drawing.Point(9, 130);
            this.cmbTargetLayer.Name = "cmbTargetLayer";
            this.cmbTargetLayer.Size = new System.Drawing.Size(281, 21);
            this.cmbTargetLayer.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Target layer:";
            // 
            // cmbSpatialSelectionMethod
            // 
            this.cmbSpatialSelectionMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSpatialSelectionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpatialSelectionMethod.FormattingEnabled = true;
            this.cmbSpatialSelectionMethod.Location = new System.Drawing.Point(9, 185);
            this.cmbSpatialSelectionMethod.Name = "cmbSpatialSelectionMethod";
            this.cmbSpatialSelectionMethod.Size = new System.Drawing.Size(281, 21);
            this.cmbSpatialSelectionMethod.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(242, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Spatial selection method for target layer feature(s):";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(9, 237);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(90, 237);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(215, 237);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // paMain
            // 
            this.paMain.Controls.Add(this.btnCancel);
            this.paMain.Controls.Add(this.btnApply);
            this.paMain.Controls.Add(this.btnOK);
            this.paMain.Controls.Add(this.label2);
            this.paMain.Controls.Add(this.cmbSelectionMethod);
            this.paMain.Controls.Add(this.cmbSelectionLayer);
            this.paMain.Controls.Add(this.label4);
            this.paMain.Controls.Add(this.label3);
            this.paMain.Controls.Add(this.cmbSpatialSelectionMethod);
            this.paMain.Controls.Add(this.cmbTargetLayer);
            this.paMain.Controls.Add(this.label1);
            this.paMain.Location = new System.Drawing.Point(12, 12);
            this.paMain.Name = "paMain";
            this.paMain.Size = new System.Drawing.Size(300, 278);
            this.paMain.TabIndex = 8;
            // 
            // pbProgress
            // 
            this.pbProgress.Image = global::BenMAP.Properties.Resources.progressbar;
            this.pbProgress.Location = new System.Drawing.Point(110, 85);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(80, 80);
            this.pbProgress.TabIndex = 8;
            this.pbProgress.TabStop = false;
            // 
            // SelectByLocationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 302);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.paMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectByLocationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select By Location";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectByLocationDialog_FormClosing);
            this.paMain.ResumeLayout(false);
            this.paMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSelectionMethod;
        private System.Windows.Forms.ComboBox cmbSelectionLayer;
        private System.Windows.Forms.ComboBox cmbTargetLayer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbSpatialSelectionMethod;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel paMain;
        private System.Windows.Forms.PictureBox pbProgress;
    }
}