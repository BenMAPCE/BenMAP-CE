namespace BenMAP
{
    partial class ModelFileConcatenator
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
            this.txtWLowCol = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectWesFiles = new System.Windows.Forms.Button();
            this.chkConvert = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbColRow = new System.Windows.Forms.RadioButton();
            this.rbRowCol = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbWhitespace = new System.Windows.Forms.RadioButton();
            this.rbComma = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtWUppCol = new System.Windows.Forms.TextBox();
            this.txtWLowRow = new System.Windows.Forms.TextBox();
            this.txtWUppRow = new System.Windows.Forms.TextBox();
            this.txtUppRow = new System.Windows.Forms.TextBox();
            this.txtELowRow = new System.Windows.Forms.TextBox();
            this.txtUppCol = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnselectEasFiles = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtELowCol = new System.Windows.Forms.TextBox();
            this.txtWFiles = new System.Windows.Forms.TextBox();
            this.txtEFiles = new System.Windows.Forms.TextBox();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtWLowCol
            // 
            this.txtWLowCol.Location = new System.Drawing.Point(280, 141);
            this.txtWLowCol.Name = "txtWLowCol";
            this.txtWLowCol.Size = new System.Drawing.Size(68, 22);
            this.txtWLowCol.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(278, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "Lower Bound";
            // 
            // btnSelectWesFiles
            // 
            this.btnSelectWesFiles.Location = new System.Drawing.Point(12, 178);
            this.btnSelectWesFiles.Name = "btnSelectWesFiles";
            this.btnSelectWesFiles.Size = new System.Drawing.Size(195, 27);
            this.btnSelectWesFiles.TabIndex = 8;
            this.btnSelectWesFiles.Text = "Select Western Domain files";
            this.btnSelectWesFiles.UseVisualStyleBackColor = true;
            // 
            // chkConvert
            // 
            this.chkConvert.AutoSize = true;
            this.chkConvert.Location = new System.Drawing.Point(15, 85);
            this.chkConvert.Name = "chkConvert";
            this.chkConvert.Size = new System.Drawing.Size(372, 18);
            this.chkConvert.TabIndex = 2;
            this.chkConvert.Text = "Convert model values from parts per million to parts per billion";
            this.chkConvert.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbColRow);
            this.groupBox1.Controls.Add(this.rbRowCol);
            this.groupBox1.Location = new System.Drawing.Point(15, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 54);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input Files";
            // 
            // rbColRow
            // 
            this.rbColRow.AutoSize = true;
            this.rbColRow.Location = new System.Drawing.Point(109, 23);
            this.rbColRow.Name = "rbColRow";
            this.rbColRow.Size = new System.Drawing.Size(94, 18);
            this.rbColRow.TabIndex = 1;
            this.rbColRow.TabStop = true;
            this.rbColRow.Text = "Column/Row";
            this.rbColRow.UseVisualStyleBackColor = true;
            // 
            // rbRowCol
            // 
            this.rbRowCol.AutoSize = true;
            this.rbRowCol.Location = new System.Drawing.Point(9, 23);
            this.rbRowCol.Name = "rbRowCol";
            this.rbRowCol.Size = new System.Drawing.Size(94, 18);
            this.rbRowCol.TabIndex = 0;
            this.rbRowCol.TabStop = true;
            this.rbRowCol.Text = "Row/Column";
            this.rbRowCol.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbWhitespace);
            this.groupBox2.Controls.Add(this.rbComma);
            this.groupBox2.Location = new System.Drawing.Point(232, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(194, 54);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Column Delimiter";
            // 
            // rbWhitespace
            // 
            this.rbWhitespace.AutoSize = true;
            this.rbWhitespace.Location = new System.Drawing.Point(9, 23);
            this.rbWhitespace.Name = "rbWhitespace";
            this.rbWhitespace.Size = new System.Drawing.Size(90, 18);
            this.rbWhitespace.TabIndex = 0;
            this.rbWhitespace.TabStop = true;
            this.rbWhitespace.Text = "Whitespace";
            this.rbWhitespace.UseVisualStyleBackColor = true;
            // 
            // rbComma
            // 
            this.rbComma.AutoSize = true;
            this.rbComma.Location = new System.Drawing.Point(119, 23);
            this.rbComma.Name = "rbComma";
            this.rbComma.Size = new System.Drawing.Size(65, 18);
            this.rbComma.TabIndex = 1;
            this.rbComma.TabStop = true;
            this.rbComma.Text = "Comma";
            this.rbComma.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(355, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "Upper Bound";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 14);
            this.label3.TabIndex = 9;
            this.label3.Text = "Rows:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(215, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 14);
            this.label4.TabIndex = 5;
            this.label4.Text = "Columns:";
            // 
            // txtWUppCol
            // 
            this.txtWUppCol.Location = new System.Drawing.Point(357, 141);
            this.txtWUppCol.Name = "txtWUppCol";
            this.txtWUppCol.Size = new System.Drawing.Size(69, 22);
            this.txtWUppCol.TabIndex = 7;
            // 
            // txtWLowRow
            // 
            this.txtWLowRow.Location = new System.Drawing.Point(280, 181);
            this.txtWLowRow.Name = "txtWLowRow";
            this.txtWLowRow.Size = new System.Drawing.Size(69, 22);
            this.txtWLowRow.TabIndex = 10;
            // 
            // txtWUppRow
            // 
            this.txtWUppRow.Location = new System.Drawing.Point(358, 181);
            this.txtWUppRow.Name = "txtWUppRow";
            this.txtWUppRow.Size = new System.Drawing.Size(68, 22);
            this.txtWUppRow.TabIndex = 11;
            // 
            // txtUppRow
            // 
            this.txtUppRow.Location = new System.Drawing.Point(357, 386);
            this.txtUppRow.Name = "txtUppRow";
            this.txtUppRow.Size = new System.Drawing.Size(69, 22);
            this.txtUppRow.TabIndex = 20;
            // 
            // txtELowRow
            // 
            this.txtELowRow.Location = new System.Drawing.Point(280, 386);
            this.txtELowRow.Name = "txtELowRow";
            this.txtELowRow.Size = new System.Drawing.Size(69, 22);
            this.txtELowRow.TabIndex = 19;
            // 
            // txtUppCol
            // 
            this.txtUppCol.Location = new System.Drawing.Point(357, 346);
            this.txtUppCol.Name = "txtUppCol";
            this.txtUppCol.Size = new System.Drawing.Size(69, 22);
            this.txtUppCol.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(215, 349);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 14);
            this.label5.TabIndex = 15;
            this.label5.Text = "Columns:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(233, 389);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 14);
            this.label6.TabIndex = 18;
            this.label6.Text = "Rows:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(355, 321);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 14);
            this.label7.TabIndex = 14;
            this.label7.Text = "Upper Bound";
            // 
            // btnselectEasFiles
            // 
            this.btnselectEasFiles.Location = new System.Drawing.Point(19, 384);
            this.btnselectEasFiles.Name = "btnselectEasFiles";
            this.btnselectEasFiles.Size = new System.Drawing.Size(195, 27);
            this.btnselectEasFiles.TabIndex = 21;
            this.btnselectEasFiles.Text = "Select Eastern Domain files";
            this.btnselectEasFiles.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(278, 321);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 14);
            this.label8.TabIndex = 13;
            this.label8.Text = "Lower Bound";
            // 
            // txtELowCol
            // 
            this.txtELowCol.Location = new System.Drawing.Point(280, 346);
            this.txtELowCol.Name = "txtELowCol";
            this.txtELowCol.Size = new System.Drawing.Size(68, 22);
            this.txtELowCol.TabIndex = 16;
            // 
            // txtWFiles
            // 
            this.txtWFiles.Location = new System.Drawing.Point(12, 212);
            this.txtWFiles.Multiline = true;
            this.txtWFiles.Name = "txtWFiles";
            this.txtWFiles.Size = new System.Drawing.Size(414, 95);
            this.txtWFiles.TabIndex = 12;
            // 
            // txtEFiles
            // 
            this.txtEFiles.Location = new System.Drawing.Point(12, 418);
            this.txtEFiles.Multiline = true;
            this.txtEFiles.Name = "txtEFiles";
            this.txtEFiles.Size = new System.Drawing.Size(414, 95);
            this.txtEFiles.TabIndex = 22;
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(273, 520);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 27);
            this.btnDone.TabIndex = 23;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(351, 520);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 27);
            this.btnGo.TabIndex = 24;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            // 
            // ModelFileConcatenator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 559);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.txtEFiles);
            this.Controls.Add(this.txtWFiles);
            this.Controls.Add(this.txtUppRow);
            this.Controls.Add(this.txtELowRow);
            this.Controls.Add(this.txtUppCol);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnselectEasFiles);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtELowCol);
            this.Controls.Add(this.txtWUppRow);
            this.Controls.Add(this.txtWLowRow);
            this.Controls.Add(this.txtWUppCol);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkConvert);
            this.Controls.Add(this.btnSelectWesFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtWLowCol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ModelFileConcatenator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Model File Concatenator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.TextBox txtWLowCol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectWesFiles;
        private System.Windows.Forms.CheckBox chkConvert;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbColRow;
        private System.Windows.Forms.RadioButton rbRowCol;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbWhitespace;
        private System.Windows.Forms.RadioButton rbComma;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtWUppCol;
        private System.Windows.Forms.TextBox txtWLowRow;
        private System.Windows.Forms.TextBox txtWUppRow;
        private System.Windows.Forms.TextBox txtUppRow;
        private System.Windows.Forms.TextBox txtELowRow;
        private System.Windows.Forms.TextBox txtUppCol;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnselectEasFiles;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtELowCol;
        private System.Windows.Forms.TextBox txtWFiles;
        private System.Windows.Forms.TextBox txtEFiles;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnGo;
    }
}