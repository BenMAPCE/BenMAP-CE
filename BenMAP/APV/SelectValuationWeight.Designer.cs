namespace BenMAP.APVX
{
    partial class SelectValuationWeight
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
            this.txtPoolingWindowName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.treeListView = new BrightIdeasSoftware.TreeListView();
            this.treeColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn35 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn14 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn15 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn16 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn17 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView)).BeginInit();
            this.SuspendLayout();
                                                this.txtPoolingWindowName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtPoolingWindowName.Enabled = false;
            this.txtPoolingWindowName.Location = new System.Drawing.Point(177, 488);
            this.txtPoolingWindowName.Name = "txtPoolingWindowName";
            this.txtPoolingWindowName.Size = new System.Drawing.Size(200, 22);
            this.txtPoolingWindowName.TabIndex = 23;
                                                this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 491);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 14);
            this.label1.TabIndex = 22;
            this.label1.Text = "Pooling Window Name:";
                                                this.treeListView.AllColumns.Add(this.treeColumnName);
            this.treeListView.AllColumns.Add(this.olvColumn1);
            this.treeListView.AllColumns.Add(this.olvColumn35);
            this.treeListView.AllColumns.Add(this.olvColumn14);
            this.treeListView.AllColumns.Add(this.olvColumn15);
            this.treeListView.AllColumns.Add(this.olvColumn16);
            this.treeListView.AllColumns.Add(this.olvColumn17);
            this.treeListView.AllowColumnReorder = true;
            this.treeListView.AllowDrop = true;
            this.treeListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.treeListView.CheckBoxes = false;
            this.treeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.treeColumnName,
            this.olvColumn1,
            this.olvColumn35,
            this.olvColumn14,
            this.olvColumn15,
            this.olvColumn16,
            this.olvColumn17});
            this.treeListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListView.EmptyListMsg = "This selection is empty.";
            this.treeListView.EmptyListMsgFont = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeListView.FullRowSelect = true;
            this.treeListView.HideSelection = false;
            this.treeListView.IsSimpleDragSource = true;
            this.treeListView.IsSimpleDropSink = true;
            this.treeListView.Location = new System.Drawing.Point(-2, 0);
            this.treeListView.Name = "treeListView";
            this.treeListView.OwnerDraw = true;
            this.treeListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.treeListView.ShowCommandMenuOnRightClick = true;
            this.treeListView.ShowGroups = false;
            this.treeListView.ShowImagesOnSubItems = true;
            this.treeListView.ShowItemToolTips = true;
            this.treeListView.Size = new System.Drawing.Size(640, 480);
            this.treeListView.TabIndex = 21;
            this.treeListView.UseCompatibleStateImageBehavior = false;
            this.treeListView.UseFiltering = true;
            this.treeListView.UseHotItem = true;
            this.treeListView.UseOverlays = false;
            this.treeListView.View = System.Windows.Forms.View.Details;
            this.treeListView.VirtualMode = true;
            this.treeListView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.treeListView_CellEditFinishing);
            this.treeListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.treeListView_CellEditStarting);
                                                this.treeColumnName.AspectName = "Name";
            this.treeColumnName.IsEditable = false;
            this.treeColumnName.IsTileViewColumn = true;
            this.treeColumnName.Text = "EndPointGroup/EndPoint/Author";
            this.treeColumnName.UseInitialLetterForGroup = true;
            this.treeColumnName.Width = 180;
            this.treeColumnName.WordWrap = true;
                                                this.olvColumn1.AspectName = "Weight";
            this.olvColumn1.Text = "Weight";
                                                this.olvColumn35.AspectName = "PoolingMethod";
            this.olvColumn35.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.olvColumn35.IsEditable = false;
            this.olvColumn35.Text = "Pooling Method";
            this.olvColumn35.Width = 100;
                                                this.olvColumn14.AspectName = "Qualifier";
            this.olvColumn14.IsEditable = false;
            this.olvColumn14.Text = "Qualifier";
            this.olvColumn14.Width = 180;
                                                this.olvColumn15.AspectName = "StartAge";
            this.olvColumn15.IsEditable = false;
            this.olvColumn15.Text = "Start Age";
                                                this.olvColumn16.AspectName = "EndAge";
            this.olvColumn16.IsEditable = false;
            this.olvColumn16.Text = "End Age";
                                                this.olvColumn17.AspectName = "Function";
            this.olvColumn17.IsEditable = false;
            this.olvColumn17.Text = "Function";
            this.olvColumn17.Width = 180;
                                                this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Location = new System.Drawing.Point(510, 486);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 27);
            this.btOK.TabIndex = 19;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
                                                this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.Location = new System.Drawing.Point(415, 486);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 27);
            this.btCancel.TabIndex = 20;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 518);
            this.Controls.Add(this.txtPoolingWindowName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeListView);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btCancel);
            this.Name = "SelectValuationWeight";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Valuation Weight";
            this.Load += new System.EventHandler(this.SelectValuationWeight_Load);
            ((System.ComponentModel.ISupportInitialize)(this.treeListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private BrightIdeasSoftware.TreeListView treeListView;
        private BrightIdeasSoftware.OLVColumn treeColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn35;
        private BrightIdeasSoftware.OLVColumn olvColumn14;
        private BrightIdeasSoftware.OLVColumn olvColumn15;
        private BrightIdeasSoftware.OLVColumn olvColumn16;
        private BrightIdeasSoftware.OLVColumn olvColumn17;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtPoolingWindowName;
    }
}