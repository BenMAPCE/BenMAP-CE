namespace BenMAP.APVX
{
    partial class TileSet
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.olvAvailable = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvAvailable)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.olvAvailable);
            this.groupBox1.Location = new System.Drawing.Point(13, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 421);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(6, 396);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(84, 16);
            this.chkSelectAll.TabIndex = 2;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // olvAvailable
            // 
            this.olvAvailable.AllColumns.Add(this.olvColumn2);
            this.olvAvailable.AllowColumnReorder = true;
            this.olvAvailable.AllowDrop = true;
            this.olvAvailable.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvAvailable.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvAvailable.CheckBoxes = true;
            this.olvAvailable.CheckedAspectName = "IsTileViewColumn";
            this.olvAvailable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn2});
            this.olvAvailable.CopySelectionOnControlC = false;
            this.olvAvailable.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvAvailable.Dock = System.Windows.Forms.DockStyle.Top;
            this.olvAvailable.EmptyListMsg = "This list is empty. ";
            this.olvAvailable.FullRowSelect = true;
            this.olvAvailable.HeaderUsesThemes = false;
            this.olvAvailable.HideSelection = false;
            this.olvAvailable.Location = new System.Drawing.Point(3, 17);
            this.olvAvailable.Name = "olvAvailable";
            this.olvAvailable.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.olvAvailable.OverlayText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.olvAvailable.OverlayText.BorderWidth = 2F;
            this.olvAvailable.OverlayText.Rotation = -20;
            this.olvAvailable.OverlayText.Text = "";
            this.olvAvailable.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvAvailable.ShowCommandMenuOnRightClick = true;
            this.olvAvailable.ShowGroups = false;
            this.olvAvailable.ShowImagesOnSubItems = true;
            this.olvAvailable.ShowItemCountOnGroups = true;
            this.olvAvailable.ShowItemToolTips = true;
            this.olvAvailable.Size = new System.Drawing.Size(189, 372);
            this.olvAvailable.SpaceBetweenGroups = 20;
            this.olvAvailable.TabIndex = 1;
            this.olvAvailable.UseAlternatingBackColors = true;
            this.olvAvailable.UseCompatibleStateImageBehavior = false;
            this.olvAvailable.UseFiltering = true;
            this.olvAvailable.UseHotItem = true;
            this.olvAvailable.UseHyperlinks = true;
            this.olvAvailable.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Text";
            this.olvColumn2.IsEditable = false;
            this.olvColumn2.IsTileViewColumn = true;
            this.olvColumn2.MaximumWidth = 180;
            this.olvColumn2.MinimumWidth = 50;
            this.olvColumn2.Text = "Field Name";
            this.olvColumn2.Width = 180;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btOK.Location = new System.Drawing.Point(144, 442);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(61, 27);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(77, 442);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // TileSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 484);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TileSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select study fields";
            this.Load += new System.EventHandler(this.TileSet_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvAvailable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView olvAvailable;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnCancel;
    }
}