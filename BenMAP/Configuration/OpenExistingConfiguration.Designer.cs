namespace BenMAP
{
    partial class OpenExistingConfiguration
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
            this.txtExistingConfiguration = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbtOpenCfgr = new System.Windows.Forms.RadioButton();
            this.rbtOpenCfg = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtExistingConfiguration
            // 
            this.txtExistingConfiguration.Location = new System.Drawing.Point(14, 112);
            this.txtExistingConfiguration.Name = "txtExistingConfiguration";
            this.txtExistingConfiguration.ReadOnly = true;
            this.txtExistingConfiguration.Size = new System.Drawing.Size(387, 22);
            this.txtExistingConfiguration.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowse.Location = new System.Drawing.Point(420, 109);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Open Existing Configuration File (*.cfgx) ";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(419, 144);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(321, 144);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(76, 27);
            this.btCancel.TabIndex = 4;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(326, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "If you wish to save this configuration, click the Save button.";
            this.label2.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.Controls.Add(this.rbtOpenCfgr);
            this.groupBox3.Controls.Add(this.rbtOpenCfg);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(3, 18);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(496, 78);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select File Type";
            // 
            // rbtOpenCfgr
            // 
            this.rbtOpenCfgr.AutoSize = true;
            this.rbtOpenCfgr.Location = new System.Drawing.Point(11, 39);
            this.rbtOpenCfgr.Name = "rbtOpenCfgr";
            this.rbtOpenCfgr.Size = new System.Drawing.Size(284, 18);
            this.rbtOpenCfgr.TabIndex = 2;
            this.rbtOpenCfgr.Text = "Open Existing Configuration Result File (*.cfgrx) :";
            this.rbtOpenCfgr.UseVisualStyleBackColor = true;
            this.rbtOpenCfgr.CheckedChanged += new System.EventHandler(this.rbtOpenCfgr_CheckedChanged);
            // 
            // rbtOpenCfg
            // 
            this.rbtOpenCfg.AutoSize = true;
            this.rbtOpenCfg.Checked = true;
            this.rbtOpenCfg.Location = new System.Drawing.Point(11, 18);
            this.rbtOpenCfg.Name = "rbtOpenCfg";
            this.rbtOpenCfg.Size = new System.Drawing.Size(242, 18);
            this.rbtOpenCfg.TabIndex = 1;
            this.rbtOpenCfg.TabStop = true;
            this.rbtOpenCfg.Text = "Open Existing Configuration File (*.cfgx) :";
            this.rbtOpenCfg.UseVisualStyleBackColor = true;
            this.rbtOpenCfg.CheckedChanged += new System.EventHandler(this.rbtOpenCfg_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtExistingConfiguration);
            this.groupBox1.Location = new System.Drawing.Point(12, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 178);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // OpenExistingConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 180);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OpenExistingConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Existing Configuration";
            this.Load += new System.EventHandler(this.OpenExistingConfiguration_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TextBox txtExistingConfiguration;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbtOpenCfgr;
        private System.Windows.Forms.RadioButton rbtOpenCfg;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}