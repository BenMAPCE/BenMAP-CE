namespace BenMAP
{
    partial class EffectCoefficients
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpCancelOK = new System.Windows.Forms.GroupBox();
            this.nextBtn = new System.Windows.Forms.Button();
            this.prevBtn = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtBetaParameter1 = new System.Windows.Forms.TextBox();
            this.lblBetaParameter1 = new System.Windows.Forms.Label();
            this.txtBetaParameter2 = new System.Windows.Forms.TextBox();
            this.lblBetaParameter2 = new System.Windows.Forms.Label();
            this.txtBconstantValue = new System.Windows.Forms.TextBox();
            this.txtAconstantValue = new System.Windows.Forms.TextBox();
            this.txtCconstantValue = new System.Windows.Forms.TextBox();
            this.lblConstantValue = new System.Windows.Forms.Label();
            this.lblConstantDescription = new System.Windows.Forms.Label();
            this.txtAconstantDescription = new System.Windows.Forms.TextBox();
            this.lblC = new System.Windows.Forms.Label();
            this.lblA = new System.Windows.Forms.Label();
            this.txtCconstantDescription = new System.Windows.Forms.TextBox();
            this.lblB = new System.Windows.Forms.Label();
            this.txtBconstantDescription = new System.Windows.Forms.TextBox();
            this.txtBeta = new System.Windows.Forms.TextBox();
            this.cboBetaDistribution = new System.Windows.Forms.ComboBox();
            this.lblBetaDisribution = new System.Windows.Forms.Label();
            this.lblBeta = new System.Windows.Forms.Label();
            this.showForSeasonal = new System.Windows.Forms.Panel();
            this.lblEnd = new System.Windows.Forms.Label();
            this.tbEnd = new System.Windows.Forms.TextBox();
            this.lblStart = new System.Windows.Forms.Label();
            this.tbStart = new System.Windows.Forms.TextBox();
            this.lblSeason = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.editVarBtn = new System.Windows.Forms.Button();
            this.tbModelSpec = new System.Windows.Forms.TextBox();
            this.lblModSpec = new System.Windows.Forms.Label();
            this.tbPollutant = new System.Windows.Forms.TextBox();
            this.lblPollutant = new System.Windows.Forms.Label();
            this.tbSeasMetric = new System.Windows.Forms.TextBox();
            this.lblSeasMetric = new System.Windows.Forms.Label();
            this.grpCancelOK.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.showForSeasonal.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(216, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(133, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpCancelOK
            // 
            this.grpCancelOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCancelOK.Controls.Add(this.nextBtn);
            this.grpCancelOK.Controls.Add(this.prevBtn);
            this.grpCancelOK.Controls.Add(this.btnOK);
            this.grpCancelOK.Controls.Add(this.btnCancel);
            this.grpCancelOK.Location = new System.Drawing.Point(0, 364);
            this.grpCancelOK.Name = "grpCancelOK";
            this.grpCancelOK.Size = new System.Drawing.Size(425, 55);
            this.grpCancelOK.TabIndex = 26;
            this.grpCancelOK.TabStop = false;
            // 
            // nextBtn
            // 
            this.nextBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextBtn.Location = new System.Drawing.Point(345, 21);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(75, 27);
            this.nextBtn.TabIndex = 3;
            this.nextBtn.Text = "Next >";
            this.nextBtn.UseVisualStyleBackColor = true;
            // 
            // prevBtn
            // 
            this.prevBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.prevBtn.Location = new System.Drawing.Point(5, 21);
            this.prevBtn.Name = "prevBtn";
            this.prevBtn.Size = new System.Drawing.Size(75, 27);
            this.prevBtn.TabIndex = 2;
            this.prevBtn.Text = "< Previous";
            this.prevBtn.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(8, 98);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(408, 273);
            this.tabControl1.TabIndex = 27;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(400, 246);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.txtBconstantValue);
            this.panel1.Controls.Add(this.txtAconstantValue);
            this.panel1.Controls.Add(this.txtCconstantValue);
            this.panel1.Controls.Add(this.lblConstantValue);
            this.panel1.Controls.Add(this.lblConstantDescription);
            this.panel1.Controls.Add(this.txtAconstantDescription);
            this.panel1.Controls.Add(this.lblC);
            this.panel1.Controls.Add(this.lblA);
            this.panel1.Controls.Add(this.txtCconstantDescription);
            this.panel1.Controls.Add(this.lblB);
            this.panel1.Controls.Add(this.txtBconstantDescription);
            this.panel1.Controls.Add(this.txtBeta);
            this.panel1.Controls.Add(this.cboBetaDistribution);
            this.panel1.Controls.Add(this.lblBetaDisribution);
            this.panel1.Controls.Add(this.lblBeta);
            this.panel1.Controls.Add(this.showForSeasonal);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(394, 240);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.txtBetaParameter1);
            this.panel2.Controls.Add(this.lblBetaParameter1);
            this.panel2.Controls.Add(this.txtBetaParameter2);
            this.panel2.Controls.Add(this.lblBetaParameter2);
            this.panel2.Location = new System.Drawing.Point(0, 92);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(394, 43);
            this.panel2.TabIndex = 51;
            // 
            // txtBetaParameter1
            // 
            this.txtBetaParameter1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBetaParameter1.Location = new System.Drawing.Point(68, 18);
            this.txtBetaParameter1.Name = "txtBetaParameter1";
            this.txtBetaParameter1.Size = new System.Drawing.Size(123, 22);
            this.txtBetaParameter1.TabIndex = 32;
            this.txtBetaParameter1.Text = "0";
            // 
            // lblBetaParameter1
            // 
            this.lblBetaParameter1.AutoSize = true;
            this.lblBetaParameter1.Location = new System.Drawing.Point(64, 2);
            this.lblBetaParameter1.Name = "lblBetaParameter1";
            this.lblBetaParameter1.Size = new System.Drawing.Size(100, 14);
            this.lblBetaParameter1.TabIndex = 28;
            this.lblBetaParameter1.Text = "Beta Parameter1:";
            // 
            // txtBetaParameter2
            // 
            this.txtBetaParameter2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBetaParameter2.Location = new System.Drawing.Point(203, 17);
            this.txtBetaParameter2.Name = "txtBetaParameter2";
            this.txtBetaParameter2.Size = new System.Drawing.Size(123, 22);
            this.txtBetaParameter2.TabIndex = 44;
            this.txtBetaParameter2.Text = "0";
            // 
            // lblBetaParameter2
            // 
            this.lblBetaParameter2.AutoSize = true;
            this.lblBetaParameter2.Location = new System.Drawing.Point(201, 1);
            this.lblBetaParameter2.Name = "lblBetaParameter2";
            this.lblBetaParameter2.Size = new System.Drawing.Size(100, 14);
            this.lblBetaParameter2.TabIndex = 41;
            this.lblBetaParameter2.Text = "Beta Parameter2:";
            // 
            // txtBconstantValue
            // 
            this.txtBconstantValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBconstantValue.Location = new System.Drawing.Point(203, 183);
            this.txtBconstantValue.Name = "txtBconstantValue";
            this.txtBconstantValue.Size = new System.Drawing.Size(123, 22);
            this.txtBconstantValue.TabIndex = 42;
            this.txtBconstantValue.Text = "0";
            // 
            // txtAconstantValue
            // 
            this.txtAconstantValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAconstantValue.Location = new System.Drawing.Point(203, 155);
            this.txtAconstantValue.Name = "txtAconstantValue";
            this.txtAconstantValue.Size = new System.Drawing.Size(123, 22);
            this.txtAconstantValue.TabIndex = 40;
            this.txtAconstantValue.Text = "0";
            // 
            // txtCconstantValue
            // 
            this.txtCconstantValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCconstantValue.Location = new System.Drawing.Point(203, 211);
            this.txtCconstantValue.Name = "txtCconstantValue";
            this.txtCconstantValue.Size = new System.Drawing.Size(123, 22);
            this.txtCconstantValue.TabIndex = 45;
            this.txtCconstantValue.Text = "0";
            // 
            // lblConstantValue
            // 
            this.lblConstantValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConstantValue.AutoSize = true;
            this.lblConstantValue.Location = new System.Drawing.Point(201, 139);
            this.lblConstantValue.Name = "lblConstantValue";
            this.lblConstantValue.Size = new System.Drawing.Size(92, 14);
            this.lblConstantValue.TabIndex = 39;
            this.lblConstantValue.Text = "Constant Value:";
            // 
            // lblConstantDescription
            // 
            this.lblConstantDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConstantDescription.AutoSize = true;
            this.lblConstantDescription.Location = new System.Drawing.Point(66, 140);
            this.lblConstantDescription.Name = "lblConstantDescription";
            this.lblConstantDescription.Size = new System.Drawing.Size(123, 14);
            this.lblConstantDescription.TabIndex = 25;
            this.lblConstantDescription.Text = "Constant Description:";
            // 
            // txtAconstantDescription
            // 
            this.txtAconstantDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAconstantDescription.Location = new System.Drawing.Point(68, 157);
            this.txtAconstantDescription.Name = "txtAconstantDescription";
            this.txtAconstantDescription.Size = new System.Drawing.Size(123, 22);
            this.txtAconstantDescription.TabIndex = 27;
            // 
            // lblC
            // 
            this.lblC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblC.AutoSize = true;
            this.lblC.Location = new System.Drawing.Point(46, 216);
            this.lblC.Name = "lblC";
            this.lblC.Size = new System.Drawing.Size(16, 14);
            this.lblC.TabIndex = 35;
            this.lblC.Text = "C:";
            // 
            // lblA
            // 
            this.lblA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblA.AutoSize = true;
            this.lblA.Location = new System.Drawing.Point(46, 161);
            this.lblA.Name = "lblA";
            this.lblA.Size = new System.Drawing.Size(17, 14);
            this.lblA.TabIndex = 33;
            this.lblA.Text = "A:";
            // 
            // txtCconstantDescription
            // 
            this.txtCconstantDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCconstantDescription.Location = new System.Drawing.Point(68, 212);
            this.txtCconstantDescription.Name = "txtCconstantDescription";
            this.txtCconstantDescription.Size = new System.Drawing.Size(123, 22);
            this.txtCconstantDescription.TabIndex = 31;
            // 
            // lblB
            // 
            this.lblB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblB.AutoSize = true;
            this.lblB.Location = new System.Drawing.Point(46, 189);
            this.lblB.Name = "lblB";
            this.lblB.Size = new System.Drawing.Size(17, 14);
            this.lblB.TabIndex = 34;
            this.lblB.Text = "B:";
            // 
            // txtBconstantDescription
            // 
            this.txtBconstantDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBconstantDescription.Location = new System.Drawing.Point(68, 184);
            this.txtBconstantDescription.Name = "txtBconstantDescription";
            this.txtBconstantDescription.Size = new System.Drawing.Size(123, 22);
            this.txtBconstantDescription.TabIndex = 30;
            // 
            // txtBeta
            // 
            this.txtBeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBeta.Location = new System.Drawing.Point(203, 65);
            this.txtBeta.Name = "txtBeta";
            this.txtBeta.Size = new System.Drawing.Size(123, 22);
            this.txtBeta.TabIndex = 43;
            this.txtBeta.Text = "0";
            // 
            // cboBetaDistribution
            // 
            this.cboBetaDistribution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBetaDistribution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBetaDistribution.FormattingEnabled = true;
            this.cboBetaDistribution.Location = new System.Drawing.Point(68, 67);
            this.cboBetaDistribution.Name = "cboBetaDistribution";
            this.cboBetaDistribution.Size = new System.Drawing.Size(123, 22);
            this.cboBetaDistribution.TabIndex = 29;
            // 
            // lblBetaDisribution
            // 
            this.lblBetaDisribution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBetaDisribution.AutoSize = true;
            this.lblBetaDisribution.Location = new System.Drawing.Point(64, 51);
            this.lblBetaDisribution.Name = "lblBetaDisribution";
            this.lblBetaDisribution.Size = new System.Drawing.Size(103, 14);
            this.lblBetaDisribution.TabIndex = 26;
            this.lblBetaDisribution.Text = "Beta Distribution:";
            // 
            // lblBeta
            // 
            this.lblBeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBeta.AutoSize = true;
            this.lblBeta.Location = new System.Drawing.Point(201, 50);
            this.lblBeta.Name = "lblBeta";
            this.lblBeta.Size = new System.Drawing.Size(35, 14);
            this.lblBeta.TabIndex = 38;
            this.lblBeta.Text = "Beta:";
            // 
            // showForSeasonal
            // 
            this.showForSeasonal.AutoSize = true;
            this.showForSeasonal.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.showForSeasonal.Controls.Add(this.lblEnd);
            this.showForSeasonal.Controls.Add(this.tbEnd);
            this.showForSeasonal.Controls.Add(this.lblStart);
            this.showForSeasonal.Controls.Add(this.tbStart);
            this.showForSeasonal.Controls.Add(this.lblSeason);
            this.showForSeasonal.Controls.Add(this.textBox1);
            this.showForSeasonal.Controls.Add(this.editVarBtn);
            this.showForSeasonal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showForSeasonal.Location = new System.Drawing.Point(0, 0);
            this.showForSeasonal.Margin = new System.Windows.Forms.Padding(0);
            this.showForSeasonal.Name = "showForSeasonal";
            this.showForSeasonal.Size = new System.Drawing.Size(394, 240);
            this.showForSeasonal.TabIndex = 51;
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(292, 6);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(59, 14);
            this.lblEnd.TabIndex = 48;
            this.lblEnd.Text = "End Date:";
            // 
            // tbEnd
            // 
            this.tbEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEnd.Location = new System.Drawing.Point(294, 21);
            this.tbEnd.Name = "tbEnd";
            this.tbEnd.Size = new System.Drawing.Size(78, 22);
            this.tbEnd.TabIndex = 49;
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(201, 6);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(64, 14);
            this.lblStart.TabIndex = 46;
            this.lblStart.Text = "Start Date:";
            // 
            // tbStart
            // 
            this.tbStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStart.BackColor = System.Drawing.SystemColors.Window;
            this.tbStart.Location = new System.Drawing.Point(203, 21);
            this.tbStart.Name = "tbStart";
            this.tbStart.ReadOnly = true;
            this.tbStart.Size = new System.Drawing.Size(78, 22);
            this.tbStart.TabIndex = 47;
            // 
            // lblSeason
            // 
            this.lblSeason.AutoSize = true;
            this.lblSeason.Location = new System.Drawing.Point(21, 6);
            this.lblSeason.Name = "lblSeason";
            this.lblSeason.Size = new System.Drawing.Size(50, 14);
            this.lblSeason.TabIndex = 36;
            this.lblSeason.Text = "Season:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(23, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(168, 22);
            this.textBox1.TabIndex = 37;
            // 
            // editVarBtn
            // 
            this.editVarBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.editVarBtn.Location = new System.Drawing.Point(110, 99);
            this.editVarBtn.Margin = new System.Windows.Forms.Padding(0);
            this.editVarBtn.Name = "editVarBtn";
            this.editVarBtn.Size = new System.Drawing.Size(171, 27);
            this.editVarBtn.TabIndex = 50;
            this.editVarBtn.Text = "Edit Variance/ Covariance";
            this.editVarBtn.UseVisualStyleBackColor = true;
            this.editVarBtn.Click += new System.EventHandler(this.editVarBtn_Click);

            // 
            // tbModelSpec
            // 
            this.tbModelSpec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbModelSpec.BackColor = System.Drawing.SystemColors.Window;
            this.tbModelSpec.Location = new System.Drawing.Point(131, 6);
            this.tbModelSpec.MaximumSize = new System.Drawing.Size(282, 22);
            this.tbModelSpec.Name = "tbModelSpec";
            this.tbModelSpec.ReadOnly = true;
            this.tbModelSpec.Size = new System.Drawing.Size(282, 22);
            this.tbModelSpec.TabIndex = 28;
            // 
            // lblModSpec
            // 
            this.lblModSpec.AutoSize = true;
            this.lblModSpec.Location = new System.Drawing.Point(8, 10);
            this.lblModSpec.Name = "lblModSpec";
            this.lblModSpec.Size = new System.Drawing.Size(116, 14);
            this.lblModSpec.TabIndex = 29;
            this.lblModSpec.Text = "Model Specification:";
            // 
            // tbPollutant
            // 
            this.tbPollutant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPollutant.BackColor = System.Drawing.SystemColors.Window;
            this.tbPollutant.Location = new System.Drawing.Point(131, 34);
            this.tbPollutant.MaximumSize = new System.Drawing.Size(282, 22);
            this.tbPollutant.Name = "tbPollutant";
            this.tbPollutant.ReadOnly = true;
            this.tbPollutant.Size = new System.Drawing.Size(282, 22);
            this.tbPollutant.TabIndex = 30;
            // 
            // lblPollutant
            // 
            this.lblPollutant.AutoSize = true;
            this.lblPollutant.Location = new System.Drawing.Point(8, 38);
            this.lblPollutant.Name = "lblPollutant";
            this.lblPollutant.Size = new System.Drawing.Size(60, 14);
            this.lblPollutant.TabIndex = 31;
            this.lblPollutant.Text = "Pollutant:";
            // 
            // tbSeasMetric
            // 
            this.tbSeasMetric.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSeasMetric.BackColor = System.Drawing.SystemColors.Window;
            this.tbSeasMetric.Location = new System.Drawing.Point(131, 62);
            this.tbSeasMetric.MaximumSize = new System.Drawing.Size(282, 22);
            this.tbSeasMetric.Name = "tbSeasMetric";
            this.tbSeasMetric.ReadOnly = true;
            this.tbSeasMetric.Size = new System.Drawing.Size(282, 22);
            this.tbSeasMetric.TabIndex = 32;
            // 
            // lblSeasMetric
            // 
            this.lblSeasMetric.AutoSize = true;
            this.lblSeasMetric.Location = new System.Drawing.Point(8, 66);
            this.lblSeasMetric.Name = "lblSeasMetric";
            this.lblSeasMetric.Size = new System.Drawing.Size(98, 14);
            this.lblSeasMetric.TabIndex = 33;
            this.lblSeasMetric.Text = "Seasonal Metric:";
            // 
            // EffectCoefficients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(425, 419);
            this.Controls.Add(this.tbSeasMetric);
            this.Controls.Add(this.lblSeasMetric);
            this.Controls.Add(this.tbPollutant);
            this.Controls.Add(this.lblPollutant);
            this.Controls.Add(this.tbModelSpec);
            this.Controls.Add(this.lblModSpec);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.grpCancelOK);
            this.Name = "EffectCoefficients";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Effect Coefficients";
            this.Load += new System.EventHandler(this.EffectCoefficients_Load);
            this.grpCancelOK.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.showForSeasonal.ResumeLayout(false);
            this.showForSeasonal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpCancelOK;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.Button prevBtn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox tbModelSpec;
        private System.Windows.Forms.Label lblModSpec;
        private System.Windows.Forms.TextBox tbPollutant;
        private System.Windows.Forms.Label lblPollutant;
        private System.Windows.Forms.TextBox tbSeasMetric;
        private System.Windows.Forms.Label lblSeasMetric;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button editVarBtn;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.TextBox tbEnd;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.TextBox tbStart;
        private System.Windows.Forms.Label lblBeta;
        private System.Windows.Forms.TextBox txtBetaParameter2;
        private System.Windows.Forms.TextBox txtBconstantValue;
        private System.Windows.Forms.TextBox txtAconstantValue;
        private System.Windows.Forms.TextBox txtBeta;
        private System.Windows.Forms.TextBox txtCconstantValue;
        private System.Windows.Forms.Label lblConstantValue;
        private System.Windows.Forms.Label lblBetaParameter2;
        private System.Windows.Forms.Label lblSeason;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblBetaDisribution;
        private System.Windows.Forms.Label lblConstantDescription;
        private System.Windows.Forms.TextBox txtBetaParameter1;
        private System.Windows.Forms.Label lblBetaParameter1;
        private System.Windows.Forms.TextBox txtAconstantDescription;
        private System.Windows.Forms.Label lblC;
        private System.Windows.Forms.Label lblA;
        private System.Windows.Forms.TextBox txtCconstantDescription;
        private System.Windows.Forms.Label lblB;
        private System.Windows.Forms.TextBox txtBconstantDescription;
        private System.Windows.Forms.ComboBox cboBetaDistribution;
        private System.Windows.Forms.Panel showForSeasonal;
        private System.Windows.Forms.Panel panel2;
    }
}