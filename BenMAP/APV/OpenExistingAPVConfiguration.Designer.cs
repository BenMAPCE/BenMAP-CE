namespace BenMAP
{
    partial class OpenExistingAPVConfiguration
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCancelAPVR = new System.Windows.Forms.Button();
            this.btnOpenAPVR = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btBrowerAPVR = new System.Windows.Forms.Button();
            this.txtAPVR = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancelAPV = new System.Windows.Forms.Button();
            this.btnOpenAPV = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseAPV = new System.Windows.Forms.Button();
            this.txtAPV = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnCancelAPVR);
            this.groupBox3.Controls.Add(this.btnOpenAPVR);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.btBrowerAPVR);
            this.groupBox3.Controls.Add(this.txtAPVR);
            this.groupBox3.Location = new System.Drawing.Point(12, 119);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(502, 115);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            // 
            // btnCancelAPVR
            // 
            this.btnCancelAPVR.Location = new System.Drawing.Point(310, 79);
            this.btnCancelAPVR.Name = "btnCancelAPVR";
            this.btnCancelAPVR.Size = new System.Drawing.Size(75, 27);
            this.btnCancelAPVR.TabIndex = 10;
            this.btnCancelAPVR.Text = "Cancel";
            this.btnCancelAPVR.UseVisualStyleBackColor = true;
            this.btnCancelAPVR.Click += new System.EventHandler(this.btnCancelAPV_Click);
            // 
            // btnOpenAPVR
            // 
            this.btnOpenAPVR.Location = new System.Drawing.Point(406, 79);
            this.btnOpenAPVR.Name = "btnOpenAPVR";
            this.btnOpenAPVR.Size = new System.Drawing.Size(75, 27);
            this.btnOpenAPVR.TabIndex = 9;
            this.btnOpenAPVR.Text = "OK";
            this.btnOpenAPVR.UseVisualStyleBackColor = true;
            this.btnOpenAPVR.Click += new System.EventHandler(this.btnOpenAPVR_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "Or Open Existing APV Result File (*.apvrx):";
            // 
            // btBrowerAPVR
            // 
            this.btBrowerAPVR.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btBrowerAPVR.Location = new System.Drawing.Point(406, 38);
            this.btBrowerAPVR.Name = "btBrowerAPVR";
            this.btBrowerAPVR.Size = new System.Drawing.Size(75, 27);
            this.btBrowerAPVR.TabIndex = 2;
            this.btBrowerAPVR.UseVisualStyleBackColor = true;
            this.btBrowerAPVR.Click += new System.EventHandler(this.btBrowerAPVR_Click);
            // 
            // txtAPVR
            // 
            this.txtAPVR.Location = new System.Drawing.Point(9, 41);
            this.txtAPVR.Name = "txtAPVR";
            this.txtAPVR.ReadOnly = true;
            this.txtAPVR.Size = new System.Drawing.Size(376, 22);
            this.txtAPVR.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancelAPV);
            this.groupBox1.Controls.Add(this.btnOpenAPV);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBrowseAPV);
            this.groupBox1.Controls.Add(this.txtAPV);
            this.groupBox1.Location = new System.Drawing.Point(12, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 119);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // btnCancelAPV
            // 
            this.btnCancelAPV.Location = new System.Drawing.Point(310, 79);
            this.btnCancelAPV.Name = "btnCancelAPV";
            this.btnCancelAPV.Size = new System.Drawing.Size(75, 27);
            this.btnCancelAPV.TabIndex = 10;
            this.btnCancelAPV.Text = "Cancel";
            this.btnCancelAPV.UseVisualStyleBackColor = true;
            this.btnCancelAPV.Click += new System.EventHandler(this.btnCancelAPV_Click);
            // 
            // btnOpenAPV
            // 
            this.btnOpenAPV.Location = new System.Drawing.Point(406, 79);
            this.btnOpenAPV.Name = "btnOpenAPV";
            this.btnOpenAPV.Size = new System.Drawing.Size(75, 27);
            this.btnOpenAPV.TabIndex = 9;
            this.btnOpenAPV.Text = "OK";
            this.btnOpenAPV.UseVisualStyleBackColor = true;
            this.btnOpenAPV.Click += new System.EventHandler(this.btnOpenAPV_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Open Existing APV Configuration File (*.apvx) :";
            // 
            // btnBrowseAPV
            // 
            this.btnBrowseAPV.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowseAPV.Location = new System.Drawing.Point(406, 39);
            this.btnBrowseAPV.Name = "btnBrowseAPV";
            this.btnBrowseAPV.Size = new System.Drawing.Size(75, 27);
            this.btnBrowseAPV.TabIndex = 2;
            this.btnBrowseAPV.UseVisualStyleBackColor = true;
            this.btnBrowseAPV.Click += new System.EventHandler(this.btBrowerAPV_Click);
            // 
            // txtAPV
            // 
            this.txtAPV.Location = new System.Drawing.Point(9, 42);
            this.txtAPV.Name = "txtAPV";
            this.txtAPV.ReadOnly = true;
            this.txtAPV.Size = new System.Drawing.Size(376, 22);
            this.txtAPV.TabIndex = 1;
            // 
            // OpenExistingAPVConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 239);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OpenExistingAPVConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Existing APV/APVR File";
            this.Load += new System.EventHandler(this.OpenExistingAPVConfiguration_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCancelAPVR;
        private System.Windows.Forms.Button btnOpenAPVR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btBrowerAPVR;
        private System.Windows.Forms.TextBox txtAPVR;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancelAPV;
        private System.Windows.Forms.Button btnOpenAPV;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseAPV;
        private System.Windows.Forms.TextBox txtAPV;
    }
}