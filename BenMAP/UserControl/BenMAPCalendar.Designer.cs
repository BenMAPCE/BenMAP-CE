namespace BenMAP
{
    partial class BenMAPCalendar
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
            this.lbMonth = new System.Windows.Forms.Label();
            this.lbDay = new System.Windows.Forms.Label();
            this.udDay = new System.Windows.Forms.NumericUpDown();
            this.dmMoths = new System.Windows.Forms.DomainUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMonth
            // 
            this.lbMonth.AutoSize = true;
            this.lbMonth.Location = new System.Drawing.Point(3, 7);
            this.lbMonth.Name = "lbMonth";
            this.lbMonth.Size = new System.Drawing.Size(41, 12);
            this.lbMonth.TabIndex = 2;
            this.lbMonth.Text = "Month:";
            // 
            // lbDay
            // 
            this.lbDay.AutoSize = true;
            this.lbDay.Location = new System.Drawing.Point(91, 7);
            this.lbDay.Name = "lbDay";
            this.lbDay.Size = new System.Drawing.Size(29, 12);
            this.lbDay.TabIndex = 3;
            this.lbDay.Text = "Day:";
            // 
            // udDay
            // 
            this.udDay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.udDay.Location = new System.Drawing.Point(116, 5);
            this.udDay.Name = "udDay";
            this.udDay.Size = new System.Drawing.Size(45, 21);
            this.udDay.TabIndex = 4;
            // 
            // dmMoths
            // 
            this.dmMoths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.dmMoths.Location = new System.Drawing.Point(44, 5);
            this.dmMoths.Name = "dmMoths";
            this.dmMoths.Size = new System.Drawing.Size(46, 21);
            this.dmMoths.TabIndex = 5;
            this.dmMoths.Text = "一月";
            // 
            // BenMAPCalendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dmMoths);
            this.Controls.Add(this.udDay);
            this.Controls.Add(this.lbDay);
            this.Controls.Add(this.lbMonth);
            this.Name = "BenMAPCalendar";
            this.Size = new System.Drawing.Size(167, 29);
            this.Load += new System.EventHandler(this.BenMAPCalendar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbMonth;
        private System.Windows.Forms.Label lbDay;
        private System.Windows.Forms.NumericUpDown udDay;
        private System.Windows.Forms.DomainUpDown dmMoths;
    }
}
