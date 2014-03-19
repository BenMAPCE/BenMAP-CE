namespace BenMAP
{
    partial class ViewEditMetadata
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
            this.flpLabels = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.flbTextBoxes = new System.Windows.Forms.FlowLayoutPanel();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.txtExtension = new System.Windows.Forms.TextBox();
            this.txtFileDate = new System.Windows.Forms.TextBox();
            this.txtImportDate = new System.Windows.Forms.TextBox();
            this.txtCountry = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSaveMetaData = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flpLabels.SuspendLayout();
            this.flbTextBoxes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.21505F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.78494F));
            this.tableLayoutPanel1.Controls.Add(this.flpLabels, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flbTextBoxes, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.46652F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.263499F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.48596F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(372, 463);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flpLabels
            // 
            this.flpLabels.Controls.Add(this.label3);
            this.flpLabels.Controls.Add(this.label4);
            this.flpLabels.Controls.Add(this.label5);
            this.flpLabels.Controls.Add(this.label6);
            this.flpLabels.Controls.Add(this.label7);
            this.flpLabels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpLabels.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpLabels.Location = new System.Drawing.Point(3, 3);
            this.flpLabels.Name = "flpLabels";
            this.flpLabels.Size = new System.Drawing.Size(124, 310);
            this.flpLabels.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "File name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 40);
            this.label4.Margin = new System.Windows.Forms.Padding(9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Extension:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 71);
            this.label5.Margin = new System.Windows.Forms.Padding(9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "File Date:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 102);
            this.label6.Margin = new System.Windows.Forms.Padding(9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Import Date";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 133);
            this.label7.Margin = new System.Windows.Forms.Padding(9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Country:";
            // 
            // flbTextBoxes
            // 
            this.flbTextBoxes.Controls.Add(this.txtFileName);
            this.flbTextBoxes.Controls.Add(this.txtExtension);
            this.flbTextBoxes.Controls.Add(this.txtFileDate);
            this.flbTextBoxes.Controls.Add(this.txtImportDate);
            this.flbTextBoxes.Controls.Add(this.txtCountry);
            this.flbTextBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flbTextBoxes.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flbTextBoxes.Location = new System.Drawing.Point(133, 3);
            this.flbTextBoxes.Name = "flbTextBoxes";
            this.flbTextBoxes.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.flbTextBoxes.Size = new System.Drawing.Size(236, 310);
            this.flbTextBoxes.TabIndex = 0;
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(10, 10);
            this.txtFileName.Margin = new System.Windows.Forms.Padding(5);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(215, 20);
            this.txtFileName.TabIndex = 0;
            // 
            // txtExtension
            // 
            this.txtExtension.Location = new System.Drawing.Point(10, 40);
            this.txtExtension.Margin = new System.Windows.Forms.Padding(5);
            this.txtExtension.Name = "txtExtension";
            this.txtExtension.Size = new System.Drawing.Size(215, 20);
            this.txtExtension.TabIndex = 0;
            // 
            // txtFileDate
            // 
            this.txtFileDate.Location = new System.Drawing.Point(10, 70);
            this.txtFileDate.Margin = new System.Windows.Forms.Padding(5);
            this.txtFileDate.Name = "txtFileDate";
            this.txtFileDate.Size = new System.Drawing.Size(215, 20);
            this.txtFileDate.TabIndex = 0;
            // 
            // txtImportDate
            // 
            this.txtImportDate.Location = new System.Drawing.Point(10, 100);
            this.txtImportDate.Margin = new System.Windows.Forms.Padding(5);
            this.txtImportDate.Name = "txtImportDate";
            this.txtImportDate.Size = new System.Drawing.Size(215, 20);
            this.txtImportDate.TabIndex = 0;
            // 
            // txtCountry
            // 
            this.txtCountry.Location = new System.Drawing.Point(10, 130);
            this.txtCountry.Margin = new System.Windows.Forms.Padding(5);
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.Size = new System.Drawing.Size(215, 20);
            this.txtCountry.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 28);
            this.label1.TabIndex = 1;
            this.label1.Text = "Description";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // richTextBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.richTextBox1, 2);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 347);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(366, 113);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(133, 316);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 28);
            this.label2.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(285, 472);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 27);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnSaveMetaData
            // 
            this.btnSaveMetaData.Location = new System.Drawing.Point(189, 472);
            this.btnSaveMetaData.Name = "btnSaveMetaData";
            this.btnSaveMetaData.Size = new System.Drawing.Size(75, 27);
            this.btnSaveMetaData.TabIndex = 2;
            this.btnSaveMetaData.Text = "Save";
            this.btnSaveMetaData.UseVisualStyleBackColor = true;
            // 
            // ViewEditMetadata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 507);
            this.Controls.Add(this.btnSaveMetaData);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ViewEditMetadata";
            this.Text = "ViewEditMetadata";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flpLabels.ResumeLayout(false);
            this.flpLabels.PerformLayout();
            this.flbTextBoxes.ResumeLayout(false);
            this.flbTextBoxes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flpLabels;
        private System.Windows.Forms.FlowLayoutPanel flbTextBoxes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSaveMetaData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.TextBox txtExtension;
        private System.Windows.Forms.TextBox txtFileDate;
        private System.Windows.Forms.TextBox txtImportDate;
        private System.Windows.Forms.TextBox txtCountry;
    }
}