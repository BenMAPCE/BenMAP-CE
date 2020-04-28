namespace DataConversion
{
	partial class DataConversionTool
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
			this.components = new System.ComponentModel.Container();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnBrowseInput = new System.Windows.Forms.Button();
			this.txtFilePathInput = new System.Windows.Forms.TextBox();
			this.btnConvert = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnBrowseOutput = new System.Windows.Forms.Button();
			this.txtFilePathOutput = new System.Windows.Forms.TextBox();
			this.txtStatus = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
			this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
			this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
			this.toolTip5 = new System.Windows.Forms.ToolTip(this.components);
			this.btnOK = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnBrowseInput);
			this.groupBox1.Controls.Add(this.txtFilePathInput);
			this.groupBox1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(12, 46);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(536, 73);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Source Data";
			this.toolTip2.SetToolTip(this.groupBox1, "File to convert ");
			// 
			// btnBrowseInput
			// 
			this.btnBrowseInput.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowseInput.Location = new System.Drawing.Point(450, 27);
			this.btnBrowseInput.Name = "btnBrowseInput";
			this.btnBrowseInput.Size = new System.Drawing.Size(75, 25);
			this.btnBrowseInput.TabIndex = 1;
			this.btnBrowseInput.Text = "Browse...";
			this.btnBrowseInput.UseVisualStyleBackColor = true;
			this.btnBrowseInput.Click += new System.EventHandler(this.btnBrowseInput_Click);
			// 
			// txtFilePathInput
			// 
			this.txtFilePathInput.BackColor = System.Drawing.SystemColors.Window;
			this.txtFilePathInput.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtFilePathInput.Location = new System.Drawing.Point(15, 28);
			this.txtFilePathInput.MaxLength = 15;
			this.txtFilePathInput.Name = "txtFilePathInput";
			this.txtFilePathInput.ReadOnly = true;
			this.txtFilePathInput.Size = new System.Drawing.Size(427, 22);
			this.txtFilePathInput.TabIndex = 0;
			this.toolTip4.SetToolTip(this.txtFilePathInput, "File to convert ");
			// 
			// btnConvert
			// 
			this.btnConvert.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConvert.Location = new System.Drawing.Point(234, 221);
			this.btnConvert.Name = "btnConvert";
			this.btnConvert.Size = new System.Drawing.Size(93, 25);
			this.btnConvert.TabIndex = 4;
			this.btnConvert.Text = "Convert";
			this.btnConvert.UseVisualStyleBackColor = true;
			this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnBrowseOutput);
			this.groupBox2.Controls.Add(this.txtFilePathOutput);
			this.groupBox2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(12, 134);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(536, 73);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Converted Data";
			this.toolTip1.SetToolTip(this.groupBox2, "Location/ name for converted data CSV");
			// 
			// btnBrowseOutput
			// 
			this.btnBrowseOutput.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowseOutput.Location = new System.Drawing.Point(450, 28);
			this.btnBrowseOutput.Name = "btnBrowseOutput";
			this.btnBrowseOutput.Size = new System.Drawing.Size(75, 25);
			this.btnBrowseOutput.TabIndex = 1;
			this.btnBrowseOutput.Text = "Browse...";
			this.btnBrowseOutput.UseVisualStyleBackColor = true;
			this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
			// 
			// txtFilePathOutput
			// 
			this.txtFilePathOutput.BackColor = System.Drawing.SystemColors.Window;
			this.txtFilePathOutput.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtFilePathOutput.Location = new System.Drawing.Point(15, 28);
			this.txtFilePathOutput.MaxLength = 15;
			this.txtFilePathOutput.Name = "txtFilePathOutput";
			this.txtFilePathOutput.ReadOnly = true;
			this.txtFilePathOutput.Size = new System.Drawing.Size(427, 22);
			this.txtFilePathOutput.TabIndex = 0;
			this.toolTip5.SetToolTip(this.txtFilePathOutput, "Location/ name for converted data CSV");
			// 
			// txtStatus
			// 
			this.txtStatus.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtStatus.Location = new System.Drawing.Point(12, 269);
			this.txtStatus.Multiline = true;
			this.txtStatus.Name = "txtStatus";
			this.txtStatus.Size = new System.Drawing.Size(536, 110);
			this.txtStatus.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(91, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(354, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "This tool converts daily monitor data to a BenMAP-ready format.";
			// 
			// button1
			// 
			this.button1.BackgroundImage = global::BenMAP.Properties.Resources.help_16x16;
			this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.ForeColor = System.Drawing.SystemColors.Control;
			this.button1.Location = new System.Drawing.Point(448, 9);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(25, 25);
			this.button1.TabIndex = 1;
			this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip3.SetToolTip(this.button1, "Click for more information on using the tool.");
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(471, 395);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(66, 27);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// DataConversionTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(563, 436);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtStatus);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.btnConvert);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataConversionTool";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Monitor Data Conversion Tool";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnBrowseInput;
		private System.Windows.Forms.TextBox txtFilePathInput;
		private System.Windows.Forms.Button btnConvert;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnBrowseOutput;
		private System.Windows.Forms.TextBox txtFilePathOutput;
		private System.Windows.Forms.TextBox txtStatus;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ToolTip toolTip2;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolTip toolTip3;
		private System.Windows.Forms.ToolTip toolTip4;
		private System.Windows.Forms.ToolTip toolTip5;
		private System.Windows.Forms.Button btnOK;
	}
}

