namespace WinControls
{
    partial class ColorBlendControl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblMin = new System.Windows.Forms.Label();
            this.lblMax = new System.Windows.Forms.Label();
            this.lblUnit = new System.Windows.Forms.Label();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMin
            // 
            this.lblMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMin.AutoSize = true;
            this.lblMin.Location = new System.Drawing.Point(0, 27);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(11, 12);
            this.lblMin.TabIndex = 0;
            this.lblMin.Text = "0";
            this.lblMin.Visible = false;
            // 
            // lblMax
            // 
            this.lblMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMax.AutoSize = true;
            this.lblMax.Location = new System.Drawing.Point(183, 27);
            this.lblMax.Name = "lblMax";
            this.lblMax.Size = new System.Drawing.Size(11, 12);
            this.lblMax.TabIndex = 0;
            this.lblMax.Text = "0";
            this.lblMax.Visible = false;
            // 
            // lblUnit
            // 
            this.lblUnit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(88, 27);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(35, 12);
            this.lblUnit.TabIndex = 0;
            this.lblUnit.Text = "ug/m3";
            this.lblUnit.Visible = false;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.toolStripMenuItem1.Text = "Save Image As...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(159, 26);
            // 
            // ColorBlendControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip2;
            this.Controls.Add(this.lblMax);
            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.lblMin);
            this.Name = "ColorBlendControl";
            this.Size = new System.Drawing.Size(194, 46);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion 组件设计器生成的代码

        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
    }
}