namespace BenMAP
{
    partial class ExportResults
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.grpReportType = new System.Windows.Forms.GroupBox();
            this.rbMultiple = new System.Windows.Forms.RadioButton();
            this.rbSingle = new System.Windows.Forms.RadioButton();
            this.lbCRSelectFunctions = new System.Windows.Forms.ListBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpReportType.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.25628F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.74372F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 198F));
            this.tableLayoutPanel1.Controls.Add(this.txtFileName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.grpReportType, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbCRSelectFunctions, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(111, 81);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 29.56989F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.43011F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(666, 251);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.SetColumnSpan(this.txtFileName, 2);
            this.txtFileName.Location = new System.Drawing.Point(329, 194);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(246, 20);
            this.txtFileName.TabIndex = 2;
            // 
            // grpReportType
            // 
            this.grpReportType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.grpReportType.Controls.Add(this.rbMultiple);
            this.grpReportType.Controls.Add(this.rbSingle);
            this.grpReportType.Location = new System.Drawing.Point(3, 49);
            this.grpReportType.Name = "grpReportType";
            this.grpReportType.Size = new System.Drawing.Size(233, 105);
            this.grpReportType.TabIndex = 3;
            this.grpReportType.TabStop = false;
            // 
            // rbMultiple
            // 
            this.rbMultiple.AutoSize = true;
            this.rbMultiple.Location = new System.Drawing.Point(18, 50);
            this.rbMultiple.Name = "rbMultiple";
            this.rbMultiple.Size = new System.Drawing.Size(195, 17);
            this.rbMultiple.TabIndex = 1;
            this.rbMultiple.TabStop = true;
            this.rbMultiple.Text = "Multiple Studies (CPP Final Benefits)";
            this.rbMultiple.UseVisualStyleBackColor = true;
            this.rbMultiple.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
            // 
            // rbSingle
            // 
            this.rbSingle.AutoSize = true;
            this.rbSingle.Location = new System.Drawing.Point(18, 27);
            this.rbSingle.Name = "rbSingle";
            this.rbSingle.Size = new System.Drawing.Size(138, 17);
            this.rbSingle.TabIndex = 0;
            this.rbSingle.TabStop = true;
            this.rbSingle.Text = "Single Study (CPP LML)";
            this.rbSingle.UseVisualStyleBackColor = true;
            this.rbSingle.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
            // 
            // lbCRSelectFunctions
            // 
            this.lbCRSelectFunctions.FormattingEnabled = true;
            this.lbCRSelectFunctions.Location = new System.Drawing.Point(242, 49);
            this.lbCRSelectFunctions.Name = "lbCRSelectFunctions";
            this.lbCRSelectFunctions.Size = new System.Drawing.Size(222, 95);
            this.lbCRSelectFunctions.TabIndex = 4;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(503, 338);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // ExportResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExportResults";
            this.Text = "Export BenMAP Results";
            this.Load += new System.EventHandler(this.ExportResults_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grpReportType.ResumeLayout(false);
            this.grpReportType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.GroupBox grpReportType;
        private System.Windows.Forms.RadioButton rbMultiple;
        private System.Windows.Forms.RadioButton rbSingle;
        private System.Windows.Forms.ListBox lbCRSelectFunctions;
    }
}