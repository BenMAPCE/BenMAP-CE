namespace BenMAP
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnReleaseNotes = new System.Windows.Forms.Button();
            this.rtBoxDescription = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(435, 14);
            this.okButton.Margin = new System.Windows.Forms.Padding(2, 4, 3, 2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(84, 31);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelProductName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelVersion, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCopyright, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.rtBoxDescription, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 11);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(528, 344);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProductName.Location = new System.Drawing.Point(8, 101);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(8, 0, 3, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 21);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(517, 21);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Product Name";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(8, 122);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(8, 0, 3, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 21);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(517, 21);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logoPictureBox.Image = global::BenMAP.Properties.Resources.BenMap_about;
            this.logoPictureBox.Location = new System.Drawing.Point(3, 4);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(522, 93);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCopyright.Location = new System.Drawing.Point(8, 143);
            this.lblCopyright.Margin = new System.Windows.Forms.Padding(8, 0, 3, 0);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(517, 25);
            this.lblCopyright.TabIndex = 26;
            this.lblCopyright.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnReleaseNotes);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 291);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(522, 50);
            this.panel1.TabIndex = 27;
            // 
            // btnReleaseNotes
            // 
            this.btnReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReleaseNotes.Location = new System.Drawing.Point(313, 14);
            this.btnReleaseNotes.Name = "btnReleaseNotes";
            this.btnReleaseNotes.Size = new System.Drawing.Size(117, 31);
            this.btnReleaseNotes.TabIndex = 1;
            this.btnReleaseNotes.Text = "Release Notes";
            this.btnReleaseNotes.UseVisualStyleBackColor = true;
            this.btnReleaseNotes.Click += new System.EventHandler(this.btnReleaseNotes_Click);
            // 
            // rtBoxDescription
            // 
            this.rtBoxDescription.BackColor = System.Drawing.Color.White;
            this.rtBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtBoxDescription.Location = new System.Drawing.Point(3, 171);
            this.rtBoxDescription.Name = "rtBoxDescription";
            this.rtBoxDescription.ReadOnly = true;
            this.rtBoxDescription.Size = new System.Drawing.Size(522, 114);
            this.rtBoxDescription.TabIndex = 28;
            this.rtBoxDescription.Text = "Description";
            this.rtBoxDescription.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtBoxDescription_LinkClicked);
            // 
            // About
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 366);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Padding = new System.Windows.Forms.Padding(11);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About BenMAP CE 0.40";
            this.Load += new System.EventHandler(this.About_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Button btnReleaseNotes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox rtBoxDescription;
    }
}
