namespace BenMAP
{
    partial class ManageGridDefinetions
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
            this.grpManageGrid = new System.Windows.Forms.GroupBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.grpAvailableGrid = new System.Windows.Forms.GroupBox();
            this.cboDefaultGridType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtGridType = new System.Windows.Forms.TextBox();
            this.lblGridType = new System.Windows.Forms.Label();
            this.lstAvailableGrid = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpManageGrid.SuspendLayout();
            this.grpAvailableGrid.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpManageGrid
            // 
            this.grpManageGrid.Controls.Add(this.btnEdit);
            this.grpManageGrid.Controls.Add(this.btnAdd);
            this.grpManageGrid.Controls.Add(this.btnDelete);
            this.grpManageGrid.Controls.Add(this.grpAvailableGrid);
            this.grpManageGrid.Location = new System.Drawing.Point(6, 3);
            this.grpManageGrid.Name = "grpManageGrid";
            this.grpManageGrid.Size = new System.Drawing.Size(417, 252);
            this.grpManageGrid.TabIndex = 0;
            this.grpManageGrid.TabStop = false;
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(325, 214);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(77, 27);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(238, 214);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(81, 27);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(156, 214);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(76, 27);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // grpAvailableGrid
            // 
            this.grpAvailableGrid.Controls.Add(this.cboDefaultGridType);
            this.grpAvailableGrid.Controls.Add(this.label1);
            this.grpAvailableGrid.Controls.Add(this.txtGridType);
            this.grpAvailableGrid.Controls.Add(this.lblGridType);
            this.grpAvailableGrid.Controls.Add(this.lstAvailableGrid);
            this.grpAvailableGrid.Location = new System.Drawing.Point(6, 12);
            this.grpAvailableGrid.Name = "grpAvailableGrid";
            this.grpAvailableGrid.Size = new System.Drawing.Size(400, 196);
            this.grpAvailableGrid.TabIndex = 0;
            this.grpAvailableGrid.TabStop = false;
            this.grpAvailableGrid.Text = "Available Grid Definitions";
            // 
            // cboDefaultGridType
            // 
            this.cboDefaultGridType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDefaultGridType.FormattingEnabled = true;
            this.cboDefaultGridType.Location = new System.Drawing.Point(230, 145);
            this.cboDefaultGridType.Name = "cboDefaultGridType";
            this.cboDefaultGridType.Size = new System.Drawing.Size(164, 22);
            this.cboDefaultGridType.TabIndex = 4;
            this.cboDefaultGridType.SelectedValueChanged += new System.EventHandler(this.cboDefaultGridType_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "Default Grid Type:";
            // 
            // txtGridType
            // 
            this.txtGridType.Location = new System.Drawing.Point(230, 76);
            this.txtGridType.Name = "txtGridType";
            this.txtGridType.ReadOnly = true;
            this.txtGridType.Size = new System.Drawing.Size(164, 22);
            this.txtGridType.TabIndex = 2;
            this.txtGridType.Text = "Shapefile";
            // 
            // lblGridType
            // 
            this.lblGridType.AutoSize = true;
            this.lblGridType.Location = new System.Drawing.Point(227, 40);
            this.lblGridType.Name = "lblGridType";
            this.lblGridType.Size = new System.Drawing.Size(60, 14);
            this.lblGridType.TabIndex = 1;
            this.lblGridType.Text = "Grid Type:";
            // 
            // lstAvailableGrid
            // 
            this.lstAvailableGrid.FormattingEnabled = true;
            this.lstAvailableGrid.ItemHeight = 14;
            this.lstAvailableGrid.Location = new System.Drawing.Point(6, 23);
            this.lstAvailableGrid.Name = "lstAvailableGrid";
            this.lstAvailableGrid.Size = new System.Drawing.Size(215, 144);
            this.lstAvailableGrid.TabIndex = 0;
            this.lstAvailableGrid.SelectedValueChanged += new System.EventHandler(this.lstAvailableGrid_SelectedValueChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(238, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(325, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 27);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Location = new System.Drawing.Point(6, 261);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(417, 56);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // ManageGridDefinetions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 326);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpManageGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(436, 354);
            this.Name = "ManageGridDefinetions";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Grid Definitions";
            this.Load += new System.EventHandler(this.ManageGridDefinetions_Load);
            this.grpManageGrid.ResumeLayout(false);
            this.grpAvailableGrid.ResumeLayout(false);
            this.grpAvailableGrid.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpManageGrid;
        private System.Windows.Forms.GroupBox grpAvailableGrid;
        private System.Windows.Forms.TextBox txtGridType;
        private System.Windows.Forms.Label lblGridType;
        private System.Windows.Forms.ListBox lstAvailableGrid;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboDefaultGridType;
        private System.Windows.Forms.Label label1;
    }
}