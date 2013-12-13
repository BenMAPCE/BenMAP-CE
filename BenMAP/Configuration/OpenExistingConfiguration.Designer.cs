namespace BenMAP
{
    partial class OpenExistingConfiguration
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtExistingConfiguration = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btCRCancel = new System.Windows.Forms.Button();
            this.btCROK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btBrowseCR = new System.Windows.Forms.Button();
            this.txtOpenExistingCFGR = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
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
            // txtExistingConfiguration
            // 
            this.txtExistingConfiguration.Location = new System.Drawing.Point(15, 42);
            this.txtExistingConfiguration.Name = "txtExistingConfiguration";
            this.txtExistingConfiguration.ReadOnly = true;
            this.txtExistingConfiguration.Size = new System.Drawing.Size(387, 22);
            this.txtExistingConfiguration.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btnBrowse.Location = new System.Drawing.Point(409, 39);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtExistingConfiguration);
            this.groupBox1.Location = new System.Drawing.Point(12, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 119);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
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
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(310, 79);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(76, 27);
            this.btCancel.TabIndex = 4;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(408, 79);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 27);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btCRCancel);
            this.groupBox2.Controls.Add(this.btCROK);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btBrowseCR);
            this.groupBox2.Controls.Add(this.txtOpenExistingCFGR);
            this.groupBox2.Location = new System.Drawing.Point(12, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(502, 115);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // btCRCancel
            // 
            this.btCRCancel.Location = new System.Drawing.Point(310, 79);
            this.btCRCancel.Name = "btCRCancel";
            this.btCRCancel.Size = new System.Drawing.Size(75, 27);
            this.btCRCancel.TabIndex = 8;
            this.btCRCancel.Text = "Cancel";
            this.btCRCancel.UseVisualStyleBackColor = true;
            this.btCRCancel.Click += new System.EventHandler(this.btCRCancel_Click);
            // 
            // btCROK
            // 
            this.btCROK.Location = new System.Drawing.Point(409, 80);
            this.btCROK.Name = "btCROK";
            this.btCROK.Size = new System.Drawing.Size(75, 27);
            this.btCROK.TabIndex = 7;
            this.btCROK.Text = "OK";
            this.btCROK.UseVisualStyleBackColor = true;
            this.btCROK.Click += new System.EventHandler(this.btCROK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(281, 14);
            this.label4.TabIndex = 4;
            this.label4.Text = "Or Open Existing Configuration Result File (*.cfgrx) :";
            // 
            // btBrowseCR
            // 
            this.btBrowseCR.Image = global::BenMAP.Properties.Resources.folder_add;
            this.btBrowseCR.Location = new System.Drawing.Point(409, 38);
            this.btBrowseCR.Name = "btBrowseCR";
            this.btBrowseCR.Size = new System.Drawing.Size(75, 27);
            this.btBrowseCR.TabIndex = 6;
            this.btBrowseCR.UseVisualStyleBackColor = true;
            this.btBrowseCR.Click += new System.EventHandler(this.btBrowseCR_Click);
            // 
            // txtOpenExistingCFGR
            // 
            this.txtOpenExistingCFGR.Location = new System.Drawing.Point(15, 41);
            this.txtOpenExistingCFGR.Name = "txtOpenExistingCFGR";
            this.txtOpenExistingCFGR.ReadOnly = true;
            this.txtOpenExistingCFGR.Size = new System.Drawing.Size(387, 22);
            this.txtOpenExistingCFGR.TabIndex = 5;
            // 
            // OpenExistingConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 239);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OpenExistingConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Existing Configuration";
            this.Load += new System.EventHandler(this.OpenExistingConfiguration_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExistingConfiguration;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btCRCancel;
        private System.Windows.Forms.Button btCROK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btBrowseCR;
        private System.Windows.Forms.TextBox txtOpenExistingCFGR;
    }
}