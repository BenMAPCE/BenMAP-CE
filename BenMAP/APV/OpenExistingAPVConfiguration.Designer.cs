namespace BenMAP
{
    partial class OpenExistingAPVConfiguration
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtOpenApvrx = new System.Windows.Forms.RadioButton();
            this.rbtOpenApvx = new System.Windows.Forms.RadioButton();
            this.btnCancelAPV = new System.Windows.Forms.Button();
            this.btnOpenAPV = new System.Windows.Forms.Button();
            this.btnBrowseAPV = new System.Windows.Forms.Button();
            this.txtAPV = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnCancelAPV);
            this.groupBox1.Controls.Add(this.btnOpenAPV);
            this.groupBox1.Controls.Add(this.btnBrowseAPV);
            this.groupBox1.Controls.Add(this.txtAPV);
            this.groupBox1.Location = new System.Drawing.Point(12, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 170);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.rbtOpenApvrx);
            this.groupBox2.Controls.Add(this.rbtOpenApvx);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(496, 78);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select File Type";
            // 
            // rbtOpenApvrx
            // 
            this.rbtOpenApvrx.AutoSize = true;
            this.rbtOpenApvrx.Location = new System.Drawing.Point(11, 39);
            this.rbtOpenApvrx.Name = "rbtOpenApvrx";
            this.rbtOpenApvrx.Size = new System.Drawing.Size(241, 18);
            this.rbtOpenApvrx.TabIndex = 2;
            this.rbtOpenApvrx.Text = "Open Existing APVR Result File (*.apvrx):";
            this.rbtOpenApvrx.UseVisualStyleBackColor = true;
            this.rbtOpenApvrx.CheckedChanged += new System.EventHandler(this.rbtOpenApvx_CheckedChanged);
            // 
            // rbtOpenApvx
            // 
            this.rbtOpenApvx.AutoSize = true;
            this.rbtOpenApvx.Checked = true;
            this.rbtOpenApvx.Location = new System.Drawing.Point(11, 18);
            this.rbtOpenApvx.Name = "rbtOpenApvx";
            this.rbtOpenApvx.Size = new System.Drawing.Size(269, 18);
            this.rbtOpenApvx.TabIndex = 1;
            this.rbtOpenApvx.TabStop = true;
            this.rbtOpenApvx.Text = "Open Existing APV Configuration File (*.apvx) :";
            this.rbtOpenApvx.UseVisualStyleBackColor = true;
            this.rbtOpenApvx.CheckedChanged += new System.EventHandler(this.rbtOpenApvx_CheckedChanged);
            // 
            // btnCancelAPV
            // 
            this.btnCancelAPV.Location = new System.Drawing.Point(325, 134);
            this.btnCancelAPV.Name = "btnCancelAPV";
            this.btnCancelAPV.Size = new System.Drawing.Size(75, 27);
            this.btnCancelAPV.TabIndex = 3;
            this.btnCancelAPV.Text = "Cancel";
            this.btnCancelAPV.UseVisualStyleBackColor = true;
            this.btnCancelAPV.Click += new System.EventHandler(this.btnCancelAPV_Click);
            // 
            // btnOpenAPV
            // 
            this.btnOpenAPV.Location = new System.Drawing.Point(418, 134);
            this.btnOpenAPV.Name = "btnOpenAPV";
            this.btnOpenAPV.Size = new System.Drawing.Size(75, 27);
            this.btnOpenAPV.TabIndex = 4;
            this.btnOpenAPV.Text = "OK";
            this.btnOpenAPV.UseVisualStyleBackColor = true;
            this.btnOpenAPV.Click += new System.EventHandler(this.btnOpenAPV_Click);
            // 
            // btnBrowseAPV
            // 
            this.btnBrowseAPV.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowseAPV.Location = new System.Drawing.Point(418, 99);
            this.btnBrowseAPV.Name = "btnBrowseAPV";
            this.btnBrowseAPV.Size = new System.Drawing.Size(75, 27);
            this.btnBrowseAPV.TabIndex = 2;
            this.btnBrowseAPV.UseVisualStyleBackColor = true;
            this.btnBrowseAPV.Click += new System.EventHandler(this.btBrowerAPV_Click);
            // 
            // txtAPV
            // 
            this.txtAPV.Location = new System.Drawing.Point(6, 102);
            this.txtAPV.Name = "txtAPV";
            this.txtAPV.ReadOnly = true;
            this.txtAPV.Size = new System.Drawing.Size(376, 22);
            this.txtAPV.TabIndex = 1;
            // 
            // OpenExistingAPVConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 175);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OpenExistingAPVConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Existing APV/APVR File";
            this.Load += new System.EventHandler(this.OpenExistingAPVConfiguration_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancelAPV;
        private System.Windows.Forms.Button btnOpenAPV;
        private System.Windows.Forms.Button btnBrowseAPV;
        private System.Windows.Forms.TextBox txtAPV;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbtOpenApvx;
        private System.Windows.Forms.RadioButton rbtOpenApvrx;
    }
}