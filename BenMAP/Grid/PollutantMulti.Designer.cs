using System;
using System.Windows.Forms;

namespace BenMAP.Grid
{
    partial class PollutantMulti
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("O3");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("NO2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("SO2");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Oxidant Gases", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("O3");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Secondary PM2.5");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Secondary", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("O3");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("CO");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("NO2");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("SO2");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("PM2.5");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Criteria Pollutants", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12});
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("CO");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("NO2");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("EC");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Traffic", new System.Windows.Forms.TreeNode[] {
            treeNode14,
            treeNode15,
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("SO2");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("SO4^(2-)");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Power Plant", new System.Windows.Forms.TreeNode[] {
            treeNode18,
            treeNode19});
            this.mainGroup = new System.Windows.Forms.GroupBox();
            this.selectedTree = new System.Windows.Forms.TreeView();
            this.groupTreeView = new System.Windows.Forms.TreeView();
            this.togglePanel = new System.Windows.Forms.Panel();
            this.singleButton = new System.Windows.Forms.RadioButton();
            this.groupButton = new System.Windows.Forms.RadioButton();
            this.dragDropLabel = new System.Windows.Forms.Label();
            this.doubClickLabel = new System.Windows.Forms.Label();
            this.cbShowDetails = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstSPollutant = new System.Windows.Forms.ListBox();
            this.lstPollutant = new System.Windows.Forms.ListBox();
            this.SelectedLabel = new System.Windows.Forms.Label();
            this.metLabel = new System.Windows.Forms.Label();
            this.txtPollutantName = new System.Windows.Forms.TextBox();
            this.polLabel = new System.Windows.Forms.Label();
            this.hrlyMetLabel = new System.Windows.Forms.Label();
            this.tclFixed = new System.Windows.Forms.TabControl();
            this.tabFixed = new System.Windows.Forms.TabPage();
            this.cmbStatistic = new System.Windows.Forms.ComboBox();
            this.fwLabel3 = new System.Windows.Forms.Label();
            this.txtEndHour = new System.Windows.Forms.TextBox();
            this.txtStartHour = new System.Windows.Forms.TextBox();
            this.fwLabel2 = new System.Windows.Forms.Label();
            this.fwLabel1 = new System.Windows.Forms.Label();
            this.tabMoving = new System.Windows.Forms.TabPage();
            this.cmbDaily = new System.Windows.Forms.ComboBox();
            this.cmbWStatistic = new System.Windows.Forms.ComboBox();
            this.txtMovingWindowSize = new System.Windows.Forms.TextBox();
            this.mwLabel3 = new System.Windows.Forms.Label();
            this.mwLabel2 = new System.Windows.Forms.Label();
            this.mwLabel1 = new System.Windows.Forms.Label();
            this.tabCustom = new System.Windows.Forms.TabPage();
            this.txtFunction = new System.Windows.Forms.TextBox();
            this.custLabel1 = new System.Windows.Forms.Label();
            this.detailGroup = new System.Windows.Forms.GroupBox();
            this.cmbMetric = new System.Windows.Forms.ComboBox();
            this.txtObservationType = new System.Windows.Forms.TextBox();
            this.obsLabel = new System.Windows.Forms.Label();
            this.cmbSeasonalMetric = new System.Windows.Forms.ComboBox();
            this.seasLabel = new System.Windows.Forms.Label();
            this.mainGroup.SuspendLayout();
            this.togglePanel.SuspendLayout();
            this.tclFixed.SuspendLayout();
            this.tabFixed.SuspendLayout();
            this.tabMoving.SuspendLayout();
            this.tabCustom.SuspendLayout();
            this.detailGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainGroup
            // 
            this.mainGroup.Controls.Add(this.selectedTree);
            this.mainGroup.Controls.Add(this.groupTreeView);
            this.mainGroup.Controls.Add(this.dragDropLabel);
            this.mainGroup.Controls.Add(this.doubClickLabel);
            this.mainGroup.Controls.Add(this.cbShowDetails);
            this.mainGroup.Controls.Add(this.btnCancel);
            this.mainGroup.Controls.Add(this.btnOK);
            this.mainGroup.Controls.Add(this.btnSelect);
            this.mainGroup.Controls.Add(this.btnDelete);
            this.mainGroup.Controls.Add(this.lstSPollutant);
            this.mainGroup.Controls.Add(this.lstPollutant);
            this.mainGroup.Controls.Add(this.SelectedLabel);
            this.mainGroup.Controls.Add(this.togglePanel);
            this.mainGroup.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainGroup.Location = new System.Drawing.Point(12, 12);
            this.mainGroup.Name = "mainGroup";
            this.mainGroup.Size = new System.Drawing.Size(483, 297);
            this.mainGroup.TabIndex = 11;
            this.mainGroup.TabStop = false;
            this.mainGroup.Text = "Main";
            // 
            // selectedTree
            // 
            this.selectedTree.AllowDrop = true;
            this.selectedTree.Location = new System.Drawing.Point(250, 42);
            this.selectedTree.Name = "selectedTree";
            this.selectedTree.Size = new System.Drawing.Size(210, 186);
            this.selectedTree.TabIndex = 15;
            this.selectedTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.groupTreeView_ItemDrag);
            this.selectedTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.selectedTree_AfterSelect);
            this.selectedTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.selectedTree_DragDrop);
            this.selectedTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.selectedTree_DragEnter);
            // 
            // groupTreeView
            // 
            this.groupTreeView.AllowDrop = true;
            this.groupTreeView.Location = new System.Drawing.Point(23, 42);
            this.groupTreeView.Name = "groupTreeView";
            treeNode1.Name = "OG_O3";
            treeNode1.Text = "O3";
            treeNode2.Name = "OG_NO2";
            treeNode2.Text = "NO2";
            treeNode3.Name = "OG_SO2";
            treeNode3.Text = "SO2";
            treeNode4.Name = "OxidantGases";
            treeNode4.Tag = "Oxidant Gases";
            treeNode4.Text = "Oxidant Gases";
            treeNode5.Name = "Sec_O3";
            treeNode5.Text = "O3";
            treeNode6.Name = "Sec_PM25";
            treeNode6.Text = "Secondary PM2.5";
            treeNode7.Name = "Secondary";
            treeNode7.Tag = "Secondary";
            treeNode7.Text = "Secondary";
            treeNode8.Name = "CP_O3";
            treeNode8.Text = "O3";
            treeNode9.Name = "CP_CO";
            treeNode9.Text = "CO";
            treeNode10.Name = "CP_NO2";
            treeNode10.Text = "NO2";
            treeNode11.Name = "CP_SO2";
            treeNode11.Text = "SO2";
            treeNode12.Name = "CP_PM25";
            treeNode12.Text = "PM2.5";
            treeNode13.Name = "CriteriaPollutants";
            treeNode13.Text = "Criteria Pollutants";
            treeNode14.Name = "Traf_CO";
            treeNode14.Text = "CO";
            treeNode15.Name = "Traf_NO2";
            treeNode15.Text = "NO2";
            treeNode16.Name = "Traf_EC";
            treeNode16.Text = "EC";
            treeNode17.Name = "Traffic";
            treeNode17.Text = "Traffic";
            treeNode18.Name = "PP_SO2";
            treeNode18.Text = "SO2";
            treeNode19.Name = "PP_SO4";
            treeNode19.Text = "SO4^(2-)";
            treeNode20.Name = "PowerPlant";
            treeNode20.Text = "Power Plant";
            this.groupTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode7,
            treeNode13,
            treeNode17,
            treeNode20});
            this.groupTreeView.Size = new System.Drawing.Size(210, 186);
            this.groupTreeView.TabIndex = 14;
            this.groupTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.groupTreeView_ItemDrag);
            this.groupTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.selectedTree_DragDrop);
            // 
            // togglePanel
            // 
            this.togglePanel.Controls.Add(this.singleButton);
            this.togglePanel.Controls.Add(this.groupButton);
            this.togglePanel.Location = new System.Drawing.Point(11, 19);
            this.togglePanel.Name = "togglePanel";
            this.togglePanel.Size = new System.Drawing.Size(231, 27);
            this.togglePanel.TabIndex = 13;
            // 
            // singleButton
            // 
            this.singleButton.AutoSize = true;
            this.singleButton.Checked = true;
            this.singleButton.Location = new System.Drawing.Point(14, 5);
            this.singleButton.Name = "singleButton";
            this.singleButton.Size = new System.Drawing.Size(81, 18);
            this.singleButton.TabIndex = 12;
            this.singleButton.TabStop = true;
            this.singleButton.Text = "Pollutants";
            this.singleButton.UseVisualStyleBackColor = true;
            this.singleButton.CheckedChanged += new System.EventHandler(this.PollutantMulti_Load);
            // 
            // groupButton
            // 
            this.groupButton.AutoSize = true;
            this.groupButton.Location = new System.Drawing.Point(109, 5);
            this.groupButton.Name = "groupButton";
            this.groupButton.Size = new System.Drawing.Size(117, 18);
            this.groupButton.TabIndex = 13;
            this.groupButton.Text = "Pollutant Groups";
            this.groupButton.UseVisualStyleBackColor = true;
            this.groupButton.CheckedChanged += new System.EventHandler(this.PollutantMulti_Load);
            // 
            // dragDropLabel
            // 
            this.dragDropLabel.AutoSize = true;
            this.dragDropLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.dragDropLabel.Location = new System.Drawing.Point(23, 232);
            this.dragDropLabel.Name = "dragDropLabel";
            this.dragDropLabel.Size = new System.Drawing.Size(134, 14);
            this.dragDropLabel.TabIndex = 11;
            this.dragDropLabel.Text = "Drag and drop to select";
            // 
            // doubClickLabel
            // 
            this.doubClickLabel.AutoSize = true;
            this.doubClickLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.doubClickLabel.Location = new System.Drawing.Point(249, 231);
            this.doubClickLabel.Name = "doubClickLabel";
            this.doubClickLabel.Size = new System.Drawing.Size(128, 14);
            this.doubClickLabel.TabIndex = 10;
            this.doubClickLabel.Text = "Double-click to delete";
            // 
            // cbShowDetails
            // 
            this.cbShowDetails.AutoSize = true;
            this.cbShowDetails.Location = new System.Drawing.Point(23, 261);
            this.cbShowDetails.Name = "cbShowDetails";
            this.cbShowDetails.Size = new System.Drawing.Size(119, 18);
            this.cbShowDetails.TabIndex = 6;
            this.cbShowDetails.Text = "Pollutant Details";
            this.cbShowDetails.UseVisualStyleBackColor = true;
            this.cbShowDetails.CheckedChanged += new System.EventHandler(this.cbShowDetails_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancel.Location = new System.Drawing.Point(282, 256);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOK.Location = new System.Drawing.Point(367, 256);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(138, 256);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 27);
            this.btnSelect.TabIndex = 5;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Visible = false;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(68, 256);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstSPollutant
            // 
            this.lstSPollutant.AllowDrop = true;
            this.lstSPollutant.FormattingEnabled = true;
            this.lstSPollutant.ItemHeight = 14;
            this.lstSPollutant.Location = new System.Drawing.Point(250, 42);
            this.lstSPollutant.Name = "lstSPollutant";
            this.lstSPollutant.Size = new System.Drawing.Size(210, 186);
            this.lstSPollutant.TabIndex = 3;
            this.lstSPollutant.SelectedIndexChanged += new System.EventHandler(this.lstSPollutant_SelectedIndexChanged);
            this.lstSPollutant.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstSPollutant_DragDrop);
            this.lstSPollutant.DragOver += new System.Windows.Forms.DragEventHandler(this.lstSPollutant_DragOver);
            this.lstSPollutant.DoubleClick += new System.EventHandler(this.lstSPollutant_DoubleClick);
            // 
            // lstPollutant
            // 
            this.lstPollutant.AllowDrop = true;
            this.lstPollutant.FormattingEnabled = true;
            this.lstPollutant.ItemHeight = 14;
            this.lstPollutant.Location = new System.Drawing.Point(23, 42);
            this.lstPollutant.Name = "lstPollutant";
            this.lstPollutant.Size = new System.Drawing.Size(210, 186);
            this.lstPollutant.TabIndex = 2;
            this.lstPollutant.SelectedIndexChanged += new System.EventHandler(this.lstPollutant_SelectedIndexChanged);
            this.lstPollutant.DoubleClick += new System.EventHandler(this.lstPollutant_DoubleClick);
            this.lstPollutant.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstPollutant_MouseDown);
            // 
            // SelectedLabel
            // 
            this.SelectedLabel.AutoSize = true;
            this.SelectedLabel.Location = new System.Drawing.Point(248, 24);
            this.SelectedLabel.Name = "SelectedLabel";
            this.SelectedLabel.Size = new System.Drawing.Size(116, 14);
            this.SelectedLabel.TabIndex = 1;
            this.SelectedLabel.Text = "Selected Pollutants:";
            // 
            // metLabel
            // 
            this.metLabel.AutoSize = true;
            this.metLabel.Location = new System.Drawing.Point(27, 55);
            this.metLabel.Name = "metLabel";
            this.metLabel.Size = new System.Drawing.Size(44, 14);
            this.metLabel.TabIndex = 4;
            this.metLabel.Text = "Metric:";
            // 
            // txtPollutantName
            // 
            this.txtPollutantName.Location = new System.Drawing.Point(90, 20);
            this.txtPollutantName.Name = "txtPollutantName";
            this.txtPollutantName.ReadOnly = true;
            this.txtPollutantName.Size = new System.Drawing.Size(114, 22);
            this.txtPollutantName.TabIndex = 1;
            // 
            // polLabel
            // 
            this.polLabel.AutoSize = true;
            this.polLabel.Location = new System.Drawing.Point(27, 24);
            this.polLabel.Name = "polLabel";
            this.polLabel.Size = new System.Drawing.Size(60, 14);
            this.polLabel.TabIndex = 0;
            this.polLabel.Text = "Pollutant:";
            // 
            // hrlyMetLabel
            // 
            this.hrlyMetLabel.AutoSize = true;
            this.hrlyMetLabel.Location = new System.Drawing.Point(5, 86);
            this.hrlyMetLabel.Name = "hrlyMetLabel";
            this.hrlyMetLabel.Size = new System.Drawing.Size(79, 14);
            this.hrlyMetLabel.TabIndex = 0;
            this.hrlyMetLabel.Text = "Hourly Metric";
            // 
            // tclFixed
            // 
            this.tclFixed.Controls.Add(this.tabFixed);
            this.tclFixed.Controls.Add(this.tabMoving);
            this.tclFixed.Controls.Add(this.tabCustom);
            this.tclFixed.Location = new System.Drawing.Point(7, 104);
            this.tclFixed.Name = "tclFixed";
            this.tclFixed.SelectedIndex = 0;
            this.tclFixed.Size = new System.Drawing.Size(470, 130);
            this.tclFixed.TabIndex = 1;
            // 
            // tabFixed
            // 
            this.tabFixed.Controls.Add(this.cmbStatistic);
            this.tabFixed.Controls.Add(this.fwLabel3);
            this.tabFixed.Controls.Add(this.txtEndHour);
            this.tabFixed.Controls.Add(this.txtStartHour);
            this.tabFixed.Controls.Add(this.fwLabel2);
            this.tabFixed.Controls.Add(this.fwLabel1);
            this.tabFixed.Location = new System.Drawing.Point(4, 23);
            this.tabFixed.Name = "tabFixed";
            this.tabFixed.Padding = new System.Windows.Forms.Padding(3);
            this.tabFixed.Size = new System.Drawing.Size(462, 103);
            this.tabFixed.TabIndex = 0;
            this.tabFixed.Text = "Fixed Window";
            this.tabFixed.UseVisualStyleBackColor = true;
            // 
            // cmbStatistic
            // 
            this.cmbStatistic.FormattingEnabled = true;
            this.cmbStatistic.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cmbStatistic.Location = new System.Drawing.Point(78, 70);
            this.cmbStatistic.Name = "cmbStatistic";
            this.cmbStatistic.Size = new System.Drawing.Size(100, 22);
            this.cmbStatistic.TabIndex = 5;
            this.cmbStatistic.Text = "Mean";
            // 
            // fwLabel3
            // 
            this.fwLabel3.AutoSize = true;
            this.fwLabel3.Location = new System.Drawing.Point(6, 73);
            this.fwLabel3.Name = "fwLabel3";
            this.fwLabel3.Size = new System.Drawing.Size(52, 14);
            this.fwLabel3.TabIndex = 4;
            this.fwLabel3.Text = "Statistic:";
            // 
            // txtEndHour
            // 
            this.txtEndHour.Location = new System.Drawing.Point(78, 37);
            this.txtEndHour.Name = "txtEndHour";
            this.txtEndHour.ReadOnly = true;
            this.txtEndHour.Size = new System.Drawing.Size(100, 22);
            this.txtEndHour.TabIndex = 3;
            this.txtEndHour.Text = "23";
            // 
            // txtStartHour
            // 
            this.txtStartHour.Location = new System.Drawing.Point(78, 5);
            this.txtStartHour.Name = "txtStartHour";
            this.txtStartHour.ReadOnly = true;
            this.txtStartHour.Size = new System.Drawing.Size(100, 22);
            this.txtStartHour.TabIndex = 1;
            this.txtStartHour.Text = "0";
            // 
            // fwLabel2
            // 
            this.fwLabel2.AutoSize = true;
            this.fwLabel2.Location = new System.Drawing.Point(6, 41);
            this.fwLabel2.Name = "fwLabel2";
            this.fwLabel2.Size = new System.Drawing.Size(59, 14);
            this.fwLabel2.TabIndex = 2;
            this.fwLabel2.Text = "End Hour:";
            // 
            // fwLabel1
            // 
            this.fwLabel1.AutoSize = true;
            this.fwLabel1.Location = new System.Drawing.Point(6, 8);
            this.fwLabel1.Name = "fwLabel1";
            this.fwLabel1.Size = new System.Drawing.Size(64, 14);
            this.fwLabel1.TabIndex = 0;
            this.fwLabel1.Text = "Start Hour:";
            // 
            // tabMoving
            // 
            this.tabMoving.Controls.Add(this.cmbDaily);
            this.tabMoving.Controls.Add(this.cmbWStatistic);
            this.tabMoving.Controls.Add(this.txtMovingWindowSize);
            this.tabMoving.Controls.Add(this.mwLabel3);
            this.tabMoving.Controls.Add(this.mwLabel2);
            this.tabMoving.Controls.Add(this.mwLabel1);
            this.tabMoving.Location = new System.Drawing.Point(4, 23);
            this.tabMoving.Name = "tabMoving";
            this.tabMoving.Padding = new System.Windows.Forms.Padding(3);
            this.tabMoving.Size = new System.Drawing.Size(462, 103);
            this.tabMoving.TabIndex = 1;
            this.tabMoving.Text = "Moving Window";
            this.tabMoving.UseVisualStyleBackColor = true;
            // 
            // cmbDaily
            // 
            this.cmbDaily.FormattingEnabled = true;
            this.cmbDaily.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cmbDaily.Location = new System.Drawing.Point(127, 72);
            this.cmbDaily.Name = "cmbDaily";
            this.cmbDaily.Size = new System.Drawing.Size(121, 22);
            this.cmbDaily.TabIndex = 5;
            this.cmbDaily.Text = "Mean";
            // 
            // cmbWStatistic
            // 
            this.cmbWStatistic.FormattingEnabled = true;
            this.cmbWStatistic.Items.AddRange(new object[] {
            "Mean",
            "Median",
            "Max",
            "Min",
            "Sum"});
            this.cmbWStatistic.Location = new System.Drawing.Point(127, 38);
            this.cmbWStatistic.Name = "cmbWStatistic";
            this.cmbWStatistic.Size = new System.Drawing.Size(121, 22);
            this.cmbWStatistic.TabIndex = 4;
            this.cmbWStatistic.Text = "Mean";
            // 
            // txtMovingWindowSize
            // 
            this.txtMovingWindowSize.Location = new System.Drawing.Point(127, 7);
            this.txtMovingWindowSize.Name = "txtMovingWindowSize";
            this.txtMovingWindowSize.ReadOnly = true;
            this.txtMovingWindowSize.Size = new System.Drawing.Size(121, 22);
            this.txtMovingWindowSize.TabIndex = 3;
            this.txtMovingWindowSize.Text = "1";
            // 
            // mwLabel3
            // 
            this.mwLabel3.AutoSize = true;
            this.mwLabel3.Location = new System.Drawing.Point(10, 70);
            this.mwLabel3.Name = "mwLabel3";
            this.mwLabel3.Size = new System.Drawing.Size(83, 14);
            this.mwLabel3.TabIndex = 2;
            this.mwLabel3.Text = "Daily Statistic:";
            // 
            // mwLabel2
            // 
            this.mwLabel2.AutoSize = true;
            this.mwLabel2.Location = new System.Drawing.Point(10, 41);
            this.mwLabel2.Name = "mwLabel2";
            this.mwLabel2.Size = new System.Drawing.Size(100, 14);
            this.mwLabel2.TabIndex = 1;
            this.mwLabel2.Text = "Window Statistic:";
            // 
            // mwLabel1
            // 
            this.mwLabel1.AutoSize = true;
            this.mwLabel1.Location = new System.Drawing.Point(13, 10);
            this.mwLabel1.Name = "mwLabel1";
            this.mwLabel1.Size = new System.Drawing.Size(80, 14);
            this.mwLabel1.TabIndex = 0;
            this.mwLabel1.Text = "Window Size:";
            // 
            // tabCustom
            // 
            this.tabCustom.Controls.Add(this.txtFunction);
            this.tabCustom.Controls.Add(this.custLabel1);
            this.tabCustom.Location = new System.Drawing.Point(4, 23);
            this.tabCustom.Name = "tabCustom";
            this.tabCustom.Size = new System.Drawing.Size(462, 103);
            this.tabCustom.TabIndex = 2;
            this.tabCustom.Text = "Custom";
            this.tabCustom.UseVisualStyleBackColor = true;
            // 
            // txtFunction
            // 
            this.txtFunction.Location = new System.Drawing.Point(78, 58);
            this.txtFunction.Multiline = true;
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.Size = new System.Drawing.Size(294, 20);
            this.txtFunction.TabIndex = 5;
            // 
            // custLabel1
            // 
            this.custLabel1.AutoSize = true;
            this.custLabel1.Location = new System.Drawing.Point(19, 58);
            this.custLabel1.Name = "custLabel1";
            this.custLabel1.Size = new System.Drawing.Size(56, 14);
            this.custLabel1.TabIndex = 4;
            this.custLabel1.Text = "Function:";
            // 
            // detailGroup
            // 
            this.detailGroup.Controls.Add(this.cmbMetric);
            this.detailGroup.Controls.Add(this.txtObservationType);
            this.detailGroup.Controls.Add(this.metLabel);
            this.detailGroup.Controls.Add(this.txtPollutantName);
            this.detailGroup.Controls.Add(this.obsLabel);
            this.detailGroup.Controls.Add(this.polLabel);
            this.detailGroup.Controls.Add(this.cmbSeasonalMetric);
            this.detailGroup.Controls.Add(this.seasLabel);
            this.detailGroup.Controls.Add(this.hrlyMetLabel);
            this.detailGroup.Controls.Add(this.tclFixed);
            this.detailGroup.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailGroup.Location = new System.Drawing.Point(12, 323);
            this.detailGroup.Name = "detailGroup";
            this.detailGroup.Size = new System.Drawing.Size(483, 240);
            this.detailGroup.TabIndex = 12;
            this.detailGroup.TabStop = false;
            this.detailGroup.Text = "Details";
            // 
            // cmbMetric
            // 
            this.cmbMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetric.FormattingEnabled = true;
            this.cmbMetric.Items.AddRange(new object[] {
            "D24HourMean"});
            this.cmbMetric.Location = new System.Drawing.Point(90, 52);
            this.cmbMetric.Name = "cmbMetric";
            this.cmbMetric.Size = new System.Drawing.Size(114, 22);
            this.cmbMetric.TabIndex = 5;
            this.cmbMetric.SelectedIndexChanged += new System.EventHandler(this.cmbMetric_SelectedIndexChanged);
            // 
            // txtObservationType
            // 
            this.txtObservationType.Location = new System.Drawing.Point(344, 20);
            this.txtObservationType.Name = "txtObservationType";
            this.txtObservationType.ReadOnly = true;
            this.txtObservationType.Size = new System.Drawing.Size(112, 22);
            this.txtObservationType.TabIndex = 5;
            // 
            // obsLabel
            // 
            this.obsLabel.AutoSize = true;
            this.obsLabel.Location = new System.Drawing.Point(243, 24);
            this.obsLabel.Name = "obsLabel";
            this.obsLabel.Size = new System.Drawing.Size(102, 14);
            this.obsLabel.TabIndex = 2;
            this.obsLabel.Text = "Observation Type:";
            // 
            // cmbSeasonalMetric
            // 
            this.cmbSeasonalMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSeasonalMetric.FormattingEnabled = true;
            this.cmbSeasonalMetric.Items.AddRange(new object[] {
            "QuarterlyMean",
            "(More...)"});
            this.cmbSeasonalMetric.Location = new System.Drawing.Point(344, 52);
            this.cmbSeasonalMetric.Name = "cmbSeasonalMetric";
            this.cmbSeasonalMetric.Size = new System.Drawing.Size(112, 22);
            this.cmbSeasonalMetric.TabIndex = 3;
            // 
            // seasLabel
            // 
            this.seasLabel.AutoSize = true;
            this.seasLabel.Location = new System.Drawing.Point(243, 55);
            this.seasLabel.Name = "seasLabel";
            this.seasLabel.Size = new System.Drawing.Size(98, 14);
            this.seasLabel.TabIndex = 2;
            this.seasLabel.Text = "Seasonal Metric:";
            // 
            // PollutantMulti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 317);
            this.Controls.Add(this.detailGroup);
            this.Controls.Add(this.mainGroup);
            this.Name = "PollutantMulti";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PollutantMulti";
            this.Load += new System.EventHandler(this.PollutantMulti_Load);
            this.mainGroup.ResumeLayout(false);
            this.mainGroup.PerformLayout();
            this.togglePanel.ResumeLayout(false);
            this.togglePanel.PerformLayout();
            this.tclFixed.ResumeLayout(false);
            this.tabFixed.ResumeLayout(false);
            this.tabFixed.PerformLayout();
            this.tabMoving.ResumeLayout(false);
            this.tabMoving.PerformLayout();
            this.tabCustom.ResumeLayout(false);
            this.tabCustom.PerformLayout();
            this.detailGroup.ResumeLayout(false);
            this.detailGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        private void GroupTreeView_MouseDown(object sender, TreeViewEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.GroupBox mainGroup;
        private System.Windows.Forms.Label dragDropLabel;
        private System.Windows.Forms.Label doubClickLabel;
        private System.Windows.Forms.CheckBox cbShowDetails;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstSPollutant;
        private System.Windows.Forms.ListBox lstPollutant;
        private System.Windows.Forms.Label SelectedLabel;
        private System.Windows.Forms.Label metLabel;
        private System.Windows.Forms.TextBox txtPollutantName;
        private System.Windows.Forms.Label polLabel;
        private System.Windows.Forms.Label hrlyMetLabel;
        private System.Windows.Forms.TabControl tclFixed;
        private System.Windows.Forms.TabPage tabFixed;
        private System.Windows.Forms.ComboBox cmbStatistic;
        private System.Windows.Forms.Label fwLabel3;
        private System.Windows.Forms.TextBox txtEndHour;
        private System.Windows.Forms.TextBox txtStartHour;
        private System.Windows.Forms.Label fwLabel2;
        private System.Windows.Forms.Label fwLabel1;
        private System.Windows.Forms.TabPage tabMoving;
        private System.Windows.Forms.ComboBox cmbDaily;
        private System.Windows.Forms.ComboBox cmbWStatistic;
        private System.Windows.Forms.TextBox txtMovingWindowSize;
        private System.Windows.Forms.Label mwLabel3;
        private System.Windows.Forms.Label mwLabel2;
        private System.Windows.Forms.Label mwLabel1;
        private System.Windows.Forms.TabPage tabCustom;
        private System.Windows.Forms.TextBox txtFunction;
        private System.Windows.Forms.Label custLabel1;
        private System.Windows.Forms.GroupBox detailGroup;
        private System.Windows.Forms.ComboBox cmbMetric;
        private System.Windows.Forms.TextBox txtObservationType;
        private System.Windows.Forms.Label obsLabel;
        private System.Windows.Forms.ComboBox cmbSeasonalMetric;
        private System.Windows.Forms.Label seasLabel;
        private System.Windows.Forms.RadioButton singleButton;
        private System.Windows.Forms.Panel togglePanel;
        private System.Windows.Forms.RadioButton groupButton;
        private System.Windows.Forms.TreeView groupTreeView;
        private TreeView selectedTree;
    }
}