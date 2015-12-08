using System;
using System.Windows.Forms;

namespace BenMAP
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
            this.mainGroup = new System.Windows.Forms.GroupBox();
            this.pollTreeView = new System.Windows.Forms.TreeView();
            this.selectTreeView = new System.Windows.Forms.TreeView();
            this.pollLabel = new System.Windows.Forms.Label();
            this.dragDropLabel = new System.Windows.Forms.Label();
            this.doubClickLabel = new System.Windows.Forms.Label();
            this.cbShowDetails = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.selectedLabel = new System.Windows.Forms.Label();
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
            this.tclFixed.SuspendLayout();
            this.tabFixed.SuspendLayout();
            this.tabMoving.SuspendLayout();
            this.tabCustom.SuspendLayout();
            this.detailGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainGroup
            // 
            this.mainGroup.Controls.Add(this.pollTreeView);
            this.mainGroup.Controls.Add(this.selectTreeView);
            this.mainGroup.Controls.Add(this.pollLabel);
            this.mainGroup.Controls.Add(this.dragDropLabel);
            this.mainGroup.Controls.Add(this.doubClickLabel);
            this.mainGroup.Controls.Add(this.cbShowDetails);
            this.mainGroup.Controls.Add(this.btnCancel);
            this.mainGroup.Controls.Add(this.btnOK);
            this.mainGroup.Controls.Add(this.selectedLabel);
            this.mainGroup.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainGroup.Location = new System.Drawing.Point(12, 13);
            this.mainGroup.Name = "mainGroup";
            this.mainGroup.Size = new System.Drawing.Size(483, 320);
            this.mainGroup.TabIndex = 11;
            this.mainGroup.TabStop = false;
            this.mainGroup.Text = "Main";
            // 
            // pollTreeView
            // 
            this.pollTreeView.AllowDrop = true;
            this.pollTreeView.Location = new System.Drawing.Point(23, 45);
            this.pollTreeView.Name = "pollTreeView";
            this.pollTreeView.Size = new System.Drawing.Size(210, 200);
            this.pollTreeView.TabIndex = 14;
            this.pollTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.groupTreeView_ItemDrag);
            this.pollTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.pollTreeView_SelectedIndexChanged);
            this.pollTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.pollTreeView_SelectedIndexChanged);
            this.pollTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.selectedTree_DragDrop);
            // 
            // selectTreeView
            // 
            this.selectTreeView.AllowDrop = true;
            this.selectTreeView.Location = new System.Drawing.Point(250, 45);
            this.selectTreeView.Name = "selectTreeView";
            this.selectTreeView.Size = new System.Drawing.Size(210, 200);
            this.selectTreeView.TabIndex = 15;
            this.selectTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.groupTreeView_ItemDrag);
            this.selectTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.selectedTree_DragDrop);
            this.selectTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.selectedTree_DragEnter);
            this.selectTreeView.DoubleClick += new System.EventHandler(this.selectedTree_DoubleClick);
            // 
            // pollLabel
            // 
            this.pollLabel.AutoSize = true;
            this.pollLabel.Location = new System.Drawing.Point(23, 26);
            this.pollLabel.Name = "pollLabel";
            this.pollLabel.Size = new System.Drawing.Size(74, 14);
            this.pollLabel.TabIndex = 16;
            this.pollLabel.Text = "Pollutant(s):";
            // 
            // dragDropLabel
            // 
            this.dragDropLabel.AutoSize = true;
            this.dragDropLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.dragDropLabel.Location = new System.Drawing.Point(23, 250);
            this.dragDropLabel.Name = "dragDropLabel";
            this.dragDropLabel.Size = new System.Drawing.Size(134, 14);
            this.dragDropLabel.TabIndex = 11;
            this.dragDropLabel.Text = "Drag and drop to select";
            // 
            // doubClickLabel
            // 
            this.doubClickLabel.AutoSize = true;
            this.doubClickLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.doubClickLabel.Location = new System.Drawing.Point(249, 249);
            this.doubClickLabel.Name = "doubClickLabel";
            this.doubClickLabel.Size = new System.Drawing.Size(128, 14);
            this.doubClickLabel.TabIndex = 10;
            this.doubClickLabel.Text = "Double-click to delete";
            // 
            // cbShowDetails
            // 
            this.cbShowDetails.AutoSize = true;
            this.cbShowDetails.Location = new System.Drawing.Point(23, 281);
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
            this.btnCancel.Location = new System.Drawing.Point(282, 276);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 29);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOK.Location = new System.Drawing.Point(367, 276);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 29);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // selectedLabel
            // 
            this.selectedLabel.AutoSize = true;
            this.selectedLabel.Location = new System.Drawing.Point(248, 26);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(116, 14);
            this.selectedLabel.TabIndex = 1;
            this.selectedLabel.Text = "Selected Pollutants:";
            // 
            // metLabel
            // 
            this.metLabel.AutoSize = true;
            this.metLabel.Location = new System.Drawing.Point(27, 59);
            this.metLabel.Name = "metLabel";
            this.metLabel.Size = new System.Drawing.Size(44, 14);
            this.metLabel.TabIndex = 4;
            this.metLabel.Text = "Metric:";
            // 
            // txtPollutantName
            // 
            this.txtPollutantName.Location = new System.Drawing.Point(90, 22);
            this.txtPollutantName.Name = "txtPollutantName";
            this.txtPollutantName.ReadOnly = true;
            this.txtPollutantName.Size = new System.Drawing.Size(114, 22);
            this.txtPollutantName.TabIndex = 1;
            // 
            // polLabel
            // 
            this.polLabel.AutoSize = true;
            this.polLabel.Location = new System.Drawing.Point(27, 26);
            this.polLabel.Name = "polLabel";
            this.polLabel.Size = new System.Drawing.Size(60, 14);
            this.polLabel.TabIndex = 0;
            this.polLabel.Text = "Pollutant:";
            // 
            // hrlyMetLabel
            // 
            this.hrlyMetLabel.AutoSize = true;
            this.hrlyMetLabel.Location = new System.Drawing.Point(5, 93);
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
            this.tclFixed.Location = new System.Drawing.Point(7, 112);
            this.tclFixed.Name = "tclFixed";
            this.tclFixed.SelectedIndex = 0;
            this.tclFixed.Size = new System.Drawing.Size(470, 140);
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
            this.tabFixed.Size = new System.Drawing.Size(462, 113);
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
            this.cmbStatistic.Location = new System.Drawing.Point(78, 75);
            this.cmbStatistic.Name = "cmbStatistic";
            this.cmbStatistic.Size = new System.Drawing.Size(100, 22);
            this.cmbStatistic.TabIndex = 5;
            this.cmbStatistic.Text = "Mean";
            // 
            // fwLabel3
            // 
            this.fwLabel3.AutoSize = true;
            this.fwLabel3.Location = new System.Drawing.Point(6, 79);
            this.fwLabel3.Name = "fwLabel3";
            this.fwLabel3.Size = new System.Drawing.Size(52, 14);
            this.fwLabel3.TabIndex = 4;
            this.fwLabel3.Text = "Statistic:";
            // 
            // txtEndHour
            // 
            this.txtEndHour.Location = new System.Drawing.Point(78, 40);
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
            this.fwLabel2.Location = new System.Drawing.Point(6, 44);
            this.fwLabel2.Name = "fwLabel2";
            this.fwLabel2.Size = new System.Drawing.Size(59, 14);
            this.fwLabel2.TabIndex = 2;
            this.fwLabel2.Text = "End Hour:";
            // 
            // fwLabel1
            // 
            this.fwLabel1.AutoSize = true;
            this.fwLabel1.Location = new System.Drawing.Point(6, 9);
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
            this.tabMoving.Size = new System.Drawing.Size(462, 113);
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
            this.cmbDaily.Location = new System.Drawing.Point(127, 78);
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
            this.cmbWStatistic.Location = new System.Drawing.Point(127, 41);
            this.cmbWStatistic.Name = "cmbWStatistic";
            this.cmbWStatistic.Size = new System.Drawing.Size(121, 22);
            this.cmbWStatistic.TabIndex = 4;
            this.cmbWStatistic.Text = "Mean";
            // 
            // txtMovingWindowSize
            // 
            this.txtMovingWindowSize.Location = new System.Drawing.Point(127, 8);
            this.txtMovingWindowSize.Name = "txtMovingWindowSize";
            this.txtMovingWindowSize.ReadOnly = true;
            this.txtMovingWindowSize.Size = new System.Drawing.Size(121, 22);
            this.txtMovingWindowSize.TabIndex = 3;
            this.txtMovingWindowSize.Text = "1";
            // 
            // mwLabel3
            // 
            this.mwLabel3.AutoSize = true;
            this.mwLabel3.Location = new System.Drawing.Point(10, 75);
            this.mwLabel3.Name = "mwLabel3";
            this.mwLabel3.Size = new System.Drawing.Size(83, 14);
            this.mwLabel3.TabIndex = 2;
            this.mwLabel3.Text = "Daily Statistic:";
            // 
            // mwLabel2
            // 
            this.mwLabel2.AutoSize = true;
            this.mwLabel2.Location = new System.Drawing.Point(10, 44);
            this.mwLabel2.Name = "mwLabel2";
            this.mwLabel2.Size = new System.Drawing.Size(100, 14);
            this.mwLabel2.TabIndex = 1;
            this.mwLabel2.Text = "Window Statistic:";
            // 
            // mwLabel1
            // 
            this.mwLabel1.AutoSize = true;
            this.mwLabel1.Location = new System.Drawing.Point(13, 11);
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
            this.tabCustom.Size = new System.Drawing.Size(462, 113);
            this.tabCustom.TabIndex = 2;
            this.tabCustom.Text = "Custom";
            this.tabCustom.UseVisualStyleBackColor = true;
            // 
            // txtFunction
            // 
            this.txtFunction.Location = new System.Drawing.Point(78, 62);
            this.txtFunction.Multiline = true;
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.Size = new System.Drawing.Size(294, 21);
            this.txtFunction.TabIndex = 5;
            // 
            // custLabel1
            // 
            this.custLabel1.AutoSize = true;
            this.custLabel1.Location = new System.Drawing.Point(19, 62);
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
            this.detailGroup.Location = new System.Drawing.Point(12, 348);
            this.detailGroup.Name = "detailGroup";
            this.detailGroup.Size = new System.Drawing.Size(483, 258);
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
            this.cmbMetric.Location = new System.Drawing.Point(90, 56);
            this.cmbMetric.Name = "cmbMetric";
            this.cmbMetric.Size = new System.Drawing.Size(114, 22);
            this.cmbMetric.TabIndex = 5;
            this.cmbMetric.SelectedIndexChanged += new System.EventHandler(this.cmbMetric_SelectedIndexChanged);
            // 
            // txtObservationType
            // 
            this.txtObservationType.Location = new System.Drawing.Point(344, 22);
            this.txtObservationType.Name = "txtObservationType";
            this.txtObservationType.ReadOnly = true;
            this.txtObservationType.Size = new System.Drawing.Size(112, 22);
            this.txtObservationType.TabIndex = 5;
            // 
            // obsLabel
            // 
            this.obsLabel.AutoSize = true;
            this.obsLabel.Location = new System.Drawing.Point(243, 26);
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
            this.cmbSeasonalMetric.Location = new System.Drawing.Point(344, 56);
            this.cmbSeasonalMetric.Name = "cmbSeasonalMetric";
            this.cmbSeasonalMetric.Size = new System.Drawing.Size(112, 22);
            this.cmbSeasonalMetric.TabIndex = 3;
            // 
            // seasLabel
            // 
            this.seasLabel.AutoSize = true;
            this.seasLabel.Location = new System.Drawing.Point(243, 59);
            this.seasLabel.Name = "seasLabel";
            this.seasLabel.Size = new System.Drawing.Size(98, 14);
            this.seasLabel.TabIndex = 2;
            this.seasLabel.Text = "Seasonal Metric:";
            // 
            // PollutantMulti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 348);
            this.Controls.Add(this.detailGroup);
            this.Controls.Add(this.mainGroup);
            this.MinimumSize = new System.Drawing.Size(523, 386);
            this.Name = "PollutantMulti";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pollutant Definition";
            this.Load += new System.EventHandler(this.PollutantMulti_Load);
            this.mainGroup.ResumeLayout(false);
            this.mainGroup.PerformLayout();
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
        private System.Windows.Forms.Label selectedLabel;
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
        private System.Windows.Forms.TreeView pollTreeView;
        private TreeView selectTreeView;
        private Label pollLabel;
    }
}