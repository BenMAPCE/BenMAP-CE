namespace BenMAP
{
    partial class Renamefile
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtfileName = new System.Windows.Forms.TextBox();
            this.lblRename = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(278, 89);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 26);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(202, 89);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 26);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtfileName
            // 
            this.txtfileName.Location = new System.Drawing.Point(15, 40);
            this.txtfileName.Name = "txtfileName";
            this.txtfileName.Size = new System.Drawing.Size(333, 22);
            this.txtfileName.TabIndex = 1;
            // 
            // lblRename
            // 
            this.lblRename.AutoSize = true;
            this.lblRename.Location = new System.Drawing.Point(12, 23);
            this.lblRename.Name = "lblRename";
            this.lblRename.Size = new System.Drawing.Size(336, 14);
            this.lblRename.TabIndex = 0;
            this.lblRename.Text = "The Shapefile already exists. Rename new loaded shapefile:";
            // 
            // Renamefile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 127);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtfileName);
            this.Controls.Add(this.lblRename);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(370, 155);
            this.Name = "Renamefile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rename";
            this.Load += new System.EventHandler(this.RenameShapefile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtfileName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label lblRename;
    }
}