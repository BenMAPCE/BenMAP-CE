namespace BenMAP
{
    partial class IncidenceDatasetDefinition
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IncidenceDatasetDefinition));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnOutPut = new System.Windows.Forms.Button();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpDataView = new System.Windows.Forms.GroupBox();
            this.olvValues = new BrightIdeasSoftware.DataListView();
            this.olvColumn15 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn16 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.bdnInfo = new System.Windows.Forms.BindingNavigator(this.components);
            this.lblPageCount = new System.Windows.Forms.ToolStripLabel();
            this.tsbFirst = new System.Windows.Forms.ToolStripButton();
            this.tsbPrevious = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.txtCurrentPage = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbNext = new System.Windows.Forms.ToolStripButton();
            this.tsbLast = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.grpIncidenceDataSetDefinition = new System.Windows.Forms.GroupBox();
            this.grpDataSetIncidenceRates = new System.Windows.Forms.GroupBox();
            this.chbGroup = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboEndpointGroup = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboEndpoint = new System.Windows.Forms.ComboBox();
            this.olvIncidenceRates = new BrightIdeasSoftware.DataListView();
            this.olvcEndpointGroup = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcEndpoint = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.cboGridDefinition = new System.Windows.Forms.ComboBox();
            this.txtDataName = new System.Windows.Forms.TextBox();
            this.lblGridDefinition = new System.Windows.Forms.Label();
            this.lblDataSetName = new System.Windows.Forms.Label();
            this.grpCancelOK.SuspendLayout();
            this.grpDataView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdnInfo)).BeginInit();
            this.bdnInfo.SuspendLayout();
            this.grpIncidenceDataSetDefinition.SuspendLayout();
            this.grpDataSetIncidenceRates.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvIncidenceRates)).BeginInit();
            this.SuspendLayout();
                                                this.btnOutPut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOutPut.Location = new System.Drawing.Point(5, 426);
            this.btnOutPut.Name = "btnOutPut";
            this.btnOutPut.Size = new System.Drawing.Size(131, 27);
            this.btnOutPut.TabIndex = 34;
            this.btnOutPut.Text = "Output Sample File";
            this.toolTip1.SetToolTip(this.btnOutPut, "Click to save a template with standard .csv format. It only contains 50 rows data" +
                    " and can be used as an example to prepare the input file.");
            this.btnOutPut.UseVisualStyleBackColor = true;
            this.btnOutPut.Click += new System.EventHandler(this.btnOutPut_Click);
                                                this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.lblProgress);
            this.grpCancelOK.Controls.Add(this.progressBar1);
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(4, 563);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(810, 43);
            this.grpCancelOK.TabIndex = 2;
            this.grpCancelOK.TabStop = false;
                                                this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Location = new System.Drawing.Point(673, 18);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 14);
            this.lblProgress.TabIndex = 3;
                                                this.progressBar1.Location = new System.Drawing.Point(6, 19);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(661, 12);
            this.progressBar1.TabIndex = 2;
                                                this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(732, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                                                this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(651, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.grpDataView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDataView.Controls.Add(this.olvValues);
            this.grpDataView.Controls.Add(this.bdnInfo);
            this.grpDataView.Location = new System.Drawing.Point(581, 2);
            this.grpDataView.Name = "grpDataView";
            this.grpDataView.Size = new System.Drawing.Size(233, 555);
            this.grpDataView.TabIndex = 1;
            this.grpDataView.TabStop = false;
                                                this.olvValues.AllColumns.Add(this.olvColumn15);
            this.olvValues.AllColumns.Add(this.olvColumn16);
            this.olvValues.AllColumns.Add(this.olvcValue);
            this.olvValues.AllowColumnReorder = true;
            this.olvValues.AllowDrop = true;
            this.olvValues.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.olvValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn15,
            this.olvColumn16,
            this.olvcValue});
            this.olvValues.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvValues.DataSource = null;
            this.olvValues.EmptyListMsg = "This list is empty.";
            this.olvValues.FullRowSelect = true;
            this.olvValues.GridLines = true;
            this.olvValues.GroupWithItemCountFormat = "";
            this.olvValues.GroupWithItemCountSingularFormat = "";
            this.olvValues.HeaderUsesThemes = false;
            this.olvValues.HideSelection = false;
            this.olvValues.Location = new System.Drawing.Point(6, 15);
            this.olvValues.MultiSelect = false;
            this.olvValues.Name = "olvValues";
            this.olvValues.OwnerDraw = true;
            this.olvValues.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvValues.ShowCommandMenuOnRightClick = true;
            this.olvValues.ShowGroups = false;
            this.olvValues.ShowImagesOnSubItems = true;
            this.olvValues.ShowItemCountOnGroups = true;
            this.olvValues.ShowItemToolTips = true;
            this.olvValues.Size = new System.Drawing.Size(221, 510);
            this.olvValues.TabIndex = 27;
            this.olvValues.UseAlternatingBackColors = true;
            this.olvValues.UseCompatibleStateImageBehavior = false;
            this.olvValues.UseFiltering = true;
            this.olvValues.UseHotItem = true;
            this.olvValues.UseHyperlinks = true;
            this.olvValues.UseOverlays = false;
            this.olvValues.View = System.Windows.Forms.View.Details;
            this.olvValues.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.olvValues_ColumnClick);
                                                this.olvColumn15.AspectName = "CColumn";
            this.olvColumn15.Text = "Column";
                                                this.olvColumn16.AspectName = "Row";
            this.olvColumn16.Text = "Row";
                                                this.olvcValue.AspectName = "VValue";
            this.olvcValue.AspectToStringFormat = "{0:N8}";
            this.olvcValue.FillsFreeSpace = true;
            this.olvcValue.Text = "Value";
            this.olvcValue.Width = 74;
                                                this.bdnInfo.AddNewItem = null;
            this.bdnInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bdnInfo.BackColor = System.Drawing.Color.Transparent;
            this.bdnInfo.CountItem = this.lblPageCount;
            this.bdnInfo.DeleteItem = null;
            this.bdnInfo.Dock = System.Windows.Forms.DockStyle.None;
            this.bdnInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFirst,
            this.tsbPrevious,
            this.bindingNavigatorSeparator,
            this.txtCurrentPage,
            this.lblPageCount,
            this.bindingNavigatorSeparator1,
            this.tsbNext,
            this.tsbLast,
            this.bindingNavigatorSeparator2});
            this.bdnInfo.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.bdnInfo.Location = new System.Drawing.Point(14, 528);
            this.bdnInfo.MoveFirstItem = this.tsbFirst;
            this.bdnInfo.MoveLastItem = this.tsbLast;
            this.bdnInfo.MoveNextItem = this.tsbNext;
            this.bdnInfo.MovePreviousItem = this.tsbPrevious;
            this.bdnInfo.Name = "bdnInfo";
            this.bdnInfo.PositionItem = this.txtCurrentPage;
            this.bdnInfo.Size = new System.Drawing.Size(195, 23);
            this.bdnInfo.TabIndex = 1;
            this.bdnInfo.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.bdnInfo_ItemClicked);
                                                this.lblPageCount.Name = "lblPageCount";
            this.lblPageCount.Size = new System.Drawing.Size(32, 17);
            this.lblPageCount.Text = "/ {0}";
            this.lblPageCount.ToolTipText = "Total number of items";
                                                this.tsbFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFirst.Image = ((System.Drawing.Image)(resources.GetObject("tsbFirst.Image")));
            this.tsbFirst.Name = "tsbFirst";
            this.tsbFirst.RightToLeftAutoMirrorImage = true;
            this.tsbFirst.Size = new System.Drawing.Size(23, 20);
            this.tsbFirst.Tag = "first";
            this.tsbFirst.ToolTipText = "Move to First Page";
                                                this.tsbPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPrevious.Image = ((System.Drawing.Image)(resources.GetObject("tsbPrevious.Image")));
            this.tsbPrevious.Name = "tsbPrevious";
            this.tsbPrevious.RightToLeftAutoMirrorImage = true;
            this.tsbPrevious.Size = new System.Drawing.Size(23, 20);
            this.tsbPrevious.Tag = "previous";
            this.tsbPrevious.ToolTipText = "Move to Previous Page";
                                                this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 23);
                                                this.txtCurrentPage.AccessibleName = "Position";
            this.txtCurrentPage.AutoSize = false;
            this.txtCurrentPage.Name = "txtCurrentPage";
            this.txtCurrentPage.Size = new System.Drawing.Size(50, 23);
            this.txtCurrentPage.Text = "0";
            this.txtCurrentPage.ToolTipText = "Current position";
            this.txtCurrentPage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCurrentPage_KeyDown);
                                                this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 23);
                                                this.tsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNext.Image = ((System.Drawing.Image)(resources.GetObject("tsbNext.Image")));
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.RightToLeftAutoMirrorImage = true;
            this.tsbNext.Size = new System.Drawing.Size(23, 20);
            this.tsbNext.Tag = "next";
            this.tsbNext.ToolTipText = "Move to Next Page";
                                                this.tsbLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLast.Image = ((System.Drawing.Image)(resources.GetObject("tsbLast.Image")));
            this.tsbLast.Name = "tsbLast";
            this.tsbLast.RightToLeftAutoMirrorImage = true;
            this.tsbLast.Size = new System.Drawing.Size(23, 20);
            this.tsbLast.Tag = "last";
            this.tsbLast.ToolTipText = "Move to Last Page";
                                                this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 23);
                                                this.grpIncidenceDataSetDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpIncidenceDataSetDefinition.Controls.Add(this.grpDataSetIncidenceRates);
            this.grpIncidenceDataSetDefinition.Controls.Add(this.cboGridDefinition);
            this.grpIncidenceDataSetDefinition.Controls.Add(this.txtDataName);
            this.grpIncidenceDataSetDefinition.Controls.Add(this.lblGridDefinition);
            this.grpIncidenceDataSetDefinition.Controls.Add(this.lblDataSetName);
            this.grpIncidenceDataSetDefinition.Location = new System.Drawing.Point(4, 2);
            this.grpIncidenceDataSetDefinition.Name = "grpIncidenceDataSetDefinition";
            this.grpIncidenceDataSetDefinition.Size = new System.Drawing.Size(571, 555);
            this.grpIncidenceDataSetDefinition.TabIndex = 0;
            this.grpIncidenceDataSetDefinition.TabStop = false;
                                                this.grpDataSetIncidenceRates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDataSetIncidenceRates.Controls.Add(this.btnOutPut);
            this.grpDataSetIncidenceRates.Controls.Add(this.chbGroup);
            this.grpDataSetIncidenceRates.Controls.Add(this.groupBox3);
            this.grpDataSetIncidenceRates.Controls.Add(this.groupBox1);
            this.grpDataSetIncidenceRates.Controls.Add(this.groupBox2);
            this.grpDataSetIncidenceRates.Controls.Add(this.olvIncidenceRates);
            this.grpDataSetIncidenceRates.Controls.Add(this.btnBrowse);
            this.grpDataSetIncidenceRates.Controls.Add(this.btnDelete);
            this.grpDataSetIncidenceRates.Location = new System.Drawing.Point(10, 84);
            this.grpDataSetIncidenceRates.Name = "grpDataSetIncidenceRates";
            this.grpDataSetIncidenceRates.Size = new System.Drawing.Size(555, 460);
            this.grpDataSetIncidenceRates.TabIndex = 0;
            this.grpDataSetIncidenceRates.TabStop = false;
            this.grpDataSetIncidenceRates.Text = "Dataset Incidence Rates";
                                                this.chbGroup.AutoSize = true;
            this.chbGroup.Location = new System.Drawing.Point(484, 38);
            this.chbGroup.Name = "chbGroup";
            this.chbGroup.Size = new System.Drawing.Size(54, 16);
            this.chbGroup.TabIndex = 33;
            this.chbGroup.Text = "Group";
            this.chbGroup.UseVisualStyleBackColor = true;
            this.chbGroup.CheckedChanged += new System.EventHandler(this.chbGroup_CheckedChanged);
                                                this.groupBox3.Controls.Add(this.txtFilter);
            this.groupBox3.Location = new System.Drawing.Point(371, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(106, 46);
            this.groupBox3.TabIndex = 32;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filter";
                                                this.txtFilter.Location = new System.Drawing.Point(6, 16);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(94, 22);
            this.txtFilter.TabIndex = 0;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
                                                this.groupBox1.Controls.Add(this.cboEndpointGroup);
            this.groupBox1.Location = new System.Drawing.Point(7, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(204, 46);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Endpoint Group";
                                                this.cboEndpointGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboEndpointGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEndpointGroup.FormattingEnabled = true;
            this.cboEndpointGroup.Location = new System.Drawing.Point(6, 16);
            this.cboEndpointGroup.Name = "cboEndpointGroup";
            this.cboEndpointGroup.Size = new System.Drawing.Size(192, 22);
            this.cboEndpointGroup.TabIndex = 0;
            this.cboEndpointGroup.SelectedIndexChanged += new System.EventHandler(this.cboEndpointGroup_SelectedIndexChanged);
                                                this.groupBox2.Controls.Add(this.cboEndpoint);
            this.groupBox2.Location = new System.Drawing.Point(217, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(148, 46);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter Endpoint";
                                                this.cboEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEndpoint.FormattingEnabled = true;
            this.cboEndpoint.Location = new System.Drawing.Point(6, 16);
            this.cboEndpoint.Name = "cboEndpoint";
            this.cboEndpoint.Size = new System.Drawing.Size(136, 22);
            this.cboEndpoint.TabIndex = 0;
            this.cboEndpoint.SelectedIndexChanged += new System.EventHandler(this.cboEndpoint_SelectedIndexChanged);
                                                this.olvIncidenceRates.AllColumns.Add(this.olvcEndpointGroup);
            this.olvIncidenceRates.AllColumns.Add(this.olvcEndpoint);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn3);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn4);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn5);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn6);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn7);
            this.olvIncidenceRates.AllColumns.Add(this.olvColumn8);
            this.olvIncidenceRates.AllowColumnReorder = true;
            this.olvIncidenceRates.AllowDrop = true;
            this.olvIncidenceRates.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220)))));
            this.olvIncidenceRates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.olvIncidenceRates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcEndpointGroup,
            this.olvcEndpoint,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn7,
            this.olvColumn8});
            this.olvIncidenceRates.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvIncidenceRates.DataSource = null;
            this.olvIncidenceRates.EmptyListMsg = "This list is empty.";
            this.olvIncidenceRates.FullRowSelect = true;
            this.olvIncidenceRates.GridLines = true;
            this.olvIncidenceRates.GroupWithItemCountFormat = "";
            this.olvIncidenceRates.GroupWithItemCountSingularFormat = "";
            this.olvIncidenceRates.HeaderUsesThemes = false;
            this.olvIncidenceRates.HideSelection = false;
            this.olvIncidenceRates.Location = new System.Drawing.Point(5, 75);
            this.olvIncidenceRates.MultiSelect = false;
            this.olvIncidenceRates.Name = "olvIncidenceRates";
            this.olvIncidenceRates.OwnerDraw = true;
            this.olvIncidenceRates.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvIncidenceRates.ShowCommandMenuOnRightClick = true;
            this.olvIncidenceRates.ShowGroups = false;
            this.olvIncidenceRates.ShowImagesOnSubItems = true;
            this.olvIncidenceRates.ShowItemToolTips = true;
            this.olvIncidenceRates.Size = new System.Drawing.Size(543, 345);
            this.olvIncidenceRates.TabIndex = 26;
            this.olvIncidenceRates.UseAlternatingBackColors = true;
            this.olvIncidenceRates.UseCompatibleStateImageBehavior = false;
            this.olvIncidenceRates.UseFiltering = true;
            this.olvIncidenceRates.UseHotItem = true;
            this.olvIncidenceRates.UseHyperlinks = true;
            this.olvIncidenceRates.UseOverlays = false;
            this.olvIncidenceRates.View = System.Windows.Forms.View.Details;
            this.olvIncidenceRates.SelectionChanged += new System.EventHandler(this.olvIncidenceRates_SelectionChanged);
                                                this.olvcEndpointGroup.AspectName = "EndPointGroupName";
            this.olvcEndpointGroup.Text = "Endpoint Group";
            this.olvcEndpointGroup.Width = 98;
                                                this.olvcEndpoint.AspectName = "EndPointName";
            this.olvcEndpoint.Text = "Endpoint";
            this.olvcEndpoint.Width = 78;
                                                this.olvColumn3.AspectName = "Prevalence";
            this.olvColumn3.Text = "Type";
                                                this.olvColumn4.AspectName = "RaceName";
            this.olvColumn4.Text = "Race";
                                                this.olvColumn5.AspectName = "EthnicityName";
            this.olvColumn5.Text = "Ethnicity";
                                                this.olvColumn6.AspectName = "GenderName";
            this.olvColumn6.Text = "Gender";
                                                this.olvColumn7.AspectName = "StartAge";
            this.olvColumn7.Text = "Start Age";
                                                this.olvColumn8.AspectName = "EndAge";
            this.olvColumn8.Text = "End Age";
                                                this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(340, 426);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(131, 27);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Load From Database";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
                                                this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(474, 426);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
                                                this.cboGridDefinition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridDefinition.FormattingEnabled = true;
            this.cboGridDefinition.Location = new System.Drawing.Point(121, 56);
            this.cboGridDefinition.Name = "cboGridDefinition";
            this.cboGridDefinition.Size = new System.Drawing.Size(211, 22);
            this.cboGridDefinition.TabIndex = 3;
            this.cboGridDefinition.SelectedValueChanged += new System.EventHandler(this.cboGridDefinition_SelectedValueChanged);
                                                this.txtDataName.Location = new System.Drawing.Point(121, 20);
            this.txtDataName.Name = "txtDataName";
            this.txtDataName.Size = new System.Drawing.Size(211, 22);
            this.txtDataName.TabIndex = 2;
                                                this.lblGridDefinition.AutoSize = true;
            this.lblGridDefinition.Location = new System.Drawing.Point(14, 59);
            this.lblGridDefinition.Name = "lblGridDefinition";
            this.lblGridDefinition.Size = new System.Drawing.Size(89, 14);
            this.lblGridDefinition.TabIndex = 1;
            this.lblGridDefinition.Text = "Grid Definition:";
                                                this.lblDataSetName.AutoSize = true;
            this.lblDataSetName.Location = new System.Drawing.Point(14, 23);
            this.lblDataSetName.Name = "lblDataSetName";
            this.lblDataSetName.Size = new System.Drawing.Size(88, 14);
            this.lblDataSetName.TabIndex = 0;
            this.lblDataSetName.Text = "Dataset Name:";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 614);
            this.Controls.Add(this.grpCancelOK);
            this.Controls.Add(this.grpDataView);
            this.Controls.Add(this.grpIncidenceDataSetDefinition);
            this.MinimumSize = new System.Drawing.Size(839, 652);
            this.Name = "IncidenceDatasetDefinition";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Incidence Dataset Definition";
            this.Load += new System.EventHandler(this.IncidenceDatasetDefinition_Load);
            this.grpCancelOK.ResumeLayout(false);
            this.grpCancelOK.PerformLayout();
            this.grpDataView.ResumeLayout(false);
            this.grpDataView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdnInfo)).EndInit();
            this.bdnInfo.ResumeLayout(false);
            this.bdnInfo.PerformLayout();
            this.grpIncidenceDataSetDefinition.ResumeLayout(false);
            this.grpIncidenceDataSetDefinition.PerformLayout();
            this.grpDataSetIncidenceRates.ResumeLayout(false);
            this.grpDataSetIncidenceRates.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvIncidenceRates)).EndInit();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.GroupBox grpIncidenceDataSetDefinition;
        private System.Windows.Forms.GroupBox grpDataView;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Label lblGridDefinition;
        private System.Windows.Forms.Label lblDataSetName;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ComboBox cboGridDefinition;
        private System.Windows.Forms.TextBox txtDataName;
        private System.Windows.Forms.GroupBox grpDataSetIncidenceRates;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.BindingNavigator bdnInfo;
        private System.Windows.Forms.ToolStripLabel lblPageCount;
        private System.Windows.Forms.ToolStripButton tsbFirst;
        private System.Windows.Forms.ToolStripButton tsbPrevious;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox txtCurrentPage;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton tsbNext;
        private System.Windows.Forms.ToolStripButton tsbLast;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblProgress;
        public BrightIdeasSoftware.DataListView olvIncidenceRates;
        private BrightIdeasSoftware.OLVColumn olvcEndpointGroup;
        private BrightIdeasSoftware.OLVColumn olvcEndpoint;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private System.Windows.Forms.CheckBox chbGroup;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboEndpointGroup;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboEndpoint;
        private BrightIdeasSoftware.OLVColumn olvColumn15;
        private BrightIdeasSoftware.OLVColumn olvColumn16;
        private BrightIdeasSoftware.OLVColumn olvcValue;
        private System.Windows.Forms.Button btnOutPut;
        private System.Windows.Forms.ToolTip toolTip1;
        private BrightIdeasSoftware.DataListView olvValues;
    }
}