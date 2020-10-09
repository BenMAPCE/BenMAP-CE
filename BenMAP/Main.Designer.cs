using System.Reflection;
using System.Windows.Forms;

namespace BenMAP
{
	partial class Main
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.tipBallon = new System.Windows.Forms.ToolTip(this.components);
			this.toolItemFile = new System.Windows.Forms.ToolStripDropDownButton();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.ToolStripFile = new System.Windows.Forms.ToolStrip();
			this.mnuFile = new System.Windows.Forms.ToolStripDropDownButton();
			this.mnuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNewFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuRecentFileSep = new System.Windows.Forms.ToolStripSeparator();
			this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.printMapLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuActiveSetup = new System.Windows.Forms.ToolStripDropDownButton();
			this.mnuModifySetup = new System.Windows.Forms.ToolStripButton();
			this.mnuAnalysis = new System.Windows.Forms.ToolStripDropDownButton();
			this.mnuCustom = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuOneStepAnalysis = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuTools = new System.Windows.Forms.ToolStripDropDownButton();
			this.airQualityGridAggregationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.modelFileConcatenatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.databaseExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.databaseExportNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.databaseImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.databaseImportNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.onlineDatabaseExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.onlineDatabaseImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportAirQualityGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gbdRollbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gISMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.monitorDataConversionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.neighborFileCreatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PopSIMtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuComputeCrosswalks = new System.Windows.Forms.ToolStripMenuItem();
			this.computeCrosswalkMinimizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnMATs = new System.Windows.Forms.ToolStripDropDownButton();
			this.ozoneAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.visibilityAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.annualPMAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnMATsDaily = new System.Windows.Forms.ToolStripMenuItem();
			this.btnHelp = new System.Windows.Forms.ToolStripDropDownButton();
			this.mnuOverview = new System.Windows.Forms.ToolStripMenuItem();
			this.userDocumentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.errorReportingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnUSACase = new System.Windows.Forms.ToolStripMenuItem();
			this.btnChinaCase = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.ToolStripFile.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tipBallon
			// 
			this.tipBallon.IsBalloon = true;
			// 
			// toolItemFile
			// 
			this.toolItemFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.toolItemFile.Image = ((System.Drawing.Image)(resources.GetObject("toolItemFile.Image")));
			this.toolItemFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItemFile.Name = "toolItemFile";
			this.toolItemFile.Size = new System.Drawing.Size(38, 22);
			this.toolItemFile.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
			this.newToolStripMenuItem.Text = "&New";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
			this.openToolStripMenuItem.Text = "&Open";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
			this.exitToolStripMenuItem.Text = "&Exit";
			// 
			// pnlMain
			// 
			this.pnlMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 36);
			this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(1408, 1019);
			this.pnlMain.TabIndex = 1;
			// 
			// ToolStripFile
			// 
			this.ToolStripFile.Font = new System.Drawing.Font("Calibri", 11F);
			this.ToolStripFile.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.ToolStripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuActiveSetup,
            this.mnuModifySetup,
            this.mnuAnalysis,
            this.mnuTools,
            this.btnMATs,
            this.btnHelp});
			this.ToolStripFile.Location = new System.Drawing.Point(0, 0);
			this.ToolStripFile.Name = "ToolStripFile";
			this.ToolStripFile.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.ToolStripFile.Size = new System.Drawing.Size(1408, 36);
			this.ToolStripFile.TabIndex = 0;
			this.ToolStripFile.Text = "toolStrip1";
			// 
			// mnuFile
			// 
			this.mnuFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpenFile,
            this.mnuNewFile,
            this.mnuRecentFileSep,
            this.mnuSave,
            this.mnuSaveAs,
            this.mnuToolStripSeparator2,
            this.printMapLayoutToolStripMenuItem,
            this.toolStripSeparator2,
            this.mnuExit});
			this.mnuFile.Image = ((System.Drawing.Image)(resources.GetObject("mnuFile.Image")));
			this.mnuFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuFile.Name = "mnuFile";
			this.mnuFile.Size = new System.Drawing.Size(61, 31);
			this.mnuFile.Text = "&File";
			// 
			// mnuOpenFile
			// 
			this.mnuOpenFile.Name = "mnuOpenFile";
			this.mnuOpenFile.Size = new System.Drawing.Size(270, 36);
			this.mnuOpenFile.Text = "&Open Project";
			this.mnuOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
			// 
			// mnuNewFile
			// 
			this.mnuNewFile.Name = "mnuNewFile";
			this.mnuNewFile.Size = new System.Drawing.Size(270, 36);
			this.mnuNewFile.Text = "&New Project";
			this.mnuNewFile.Click += new System.EventHandler(this.btnNewFile_Click);
			// 
			// mnuRecentFileSep
			// 
			this.mnuRecentFileSep.Name = "mnuRecentFileSep";
			this.mnuRecentFileSep.Size = new System.Drawing.Size(267, 6);
			// 
			// mnuSave
			// 
			this.mnuSave.Name = "mnuSave";
			this.mnuSave.Size = new System.Drawing.Size(270, 36);
			this.mnuSave.Text = "&Save";
			this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
			// 
			// mnuSaveAs
			// 
			this.mnuSaveAs.Name = "mnuSaveAs";
			this.mnuSaveAs.Size = new System.Drawing.Size(270, 36);
			this.mnuSaveAs.Text = "&Save As (*.projx)";
			this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
			// 
			// mnuToolStripSeparator2
			// 
			this.mnuToolStripSeparator2.Name = "mnuToolStripSeparator2";
			this.mnuToolStripSeparator2.Size = new System.Drawing.Size(267, 6);
			// 
			// printMapLayoutToolStripMenuItem
			// 
			this.printMapLayoutToolStripMenuItem.Image = global::BenMAP.Properties.Resources.printer_32x32;
			this.printMapLayoutToolStripMenuItem.Name = "printMapLayoutToolStripMenuItem";
			this.printMapLayoutToolStripMenuItem.Size = new System.Drawing.Size(270, 36);
			this.printMapLayoutToolStripMenuItem.Text = "&Print Map Layout";
			this.printMapLayoutToolStripMenuItem.Click += new System.EventHandler(this.printMapLayoutToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(267, 6);
			// 
			// mnuExit
			// 
			this.mnuExit.Name = "mnuExit";
			this.mnuExit.Size = new System.Drawing.Size(270, 36);
			this.mnuExit.Text = "&Exit";
			this.mnuExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// mnuActiveSetup
			// 
			this.mnuActiveSetup.Name = "mnuActiveSetup";
			this.mnuActiveSetup.Size = new System.Drawing.Size(142, 31);
			this.mnuActiveSetup.Text = "Active Setup";
			// 
			// mnuModifySetup
			// 
			this.mnuModifySetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mnuModifySetup.Image = ((System.Drawing.Image)(resources.GetObject("mnuModifySetup.Image")));
			this.mnuModifySetup.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuModifySetup.Name = "mnuModifySetup";
			this.mnuModifySetup.Size = new System.Drawing.Size(165, 31);
			this.mnuModifySetup.Text = "Modify Datasets";
			this.mnuModifySetup.Click += new System.EventHandler(this.mnuModifySetup_Click);
			// 
			// mnuAnalysis
			// 
			this.mnuAnalysis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mnuAnalysis.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCustom,
            this.mnuOneStepAnalysis});
			this.mnuAnalysis.Image = ((System.Drawing.Image)(resources.GetObject("mnuAnalysis.Image")));
			this.mnuAnalysis.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuAnalysis.Name = "mnuAnalysis";
			this.mnuAnalysis.Size = new System.Drawing.Size(182, 31);
			this.mnuAnalysis.Text = "Analysis Method";
			this.mnuAnalysis.Visible = false;
			// 
			// mnuCustom
			// 
			this.mnuCustom.Name = "mnuCustom";
			this.mnuCustom.Size = new System.Drawing.Size(276, 36);
			// 
			// mnuOneStepAnalysis
			// 
			this.mnuOneStepAnalysis.Name = "mnuOneStepAnalysis";
			this.mnuOneStepAnalysis.Size = new System.Drawing.Size(276, 36);
			this.mnuOneStepAnalysis.Text = "One Step Analysis";
			this.mnuOneStepAnalysis.Click += new System.EventHandler(this.mnuOneStepAnalysis_Click);
			// 
			// mnuTools
			// 
			this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.airQualityGridAggregationToolStripMenuItem,
            this.modelFileConcatenatorToolStripMenuItem,
            this.databaseExportToolStripMenuItem,
            this.databaseExportNewToolStripMenuItem,
            this.databaseImportToolStripMenuItem,
            this.databaseImportNewToolStripMenuItem,
            this.onlineDatabaseExportToolStripMenuItem,
            this.onlineDatabaseImportToolStripMenuItem,
            this.exportAirQualityGridToolStripMenuItem,
            this.gbdRollbackToolStripMenuItem,
            this.gISMappingToolStripMenuItem,
            this.monitorDataConversionToolStripMenuItem,
            this.neighborFileCreatorToolStripMenuItem,
            this.PopSIMtoolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.mnuComputeCrosswalks,
            this.computeCrosswalkMinimizedToolStripMenuItem});
			this.mnuTools.Image = global::BenMAP.Properties.Resources.onlineAlert_Image;
			this.mnuTools.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.mnuTools.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuTools.Name = "mnuTools";
			this.mnuTools.Size = new System.Drawing.Size(101, 31);
			this.mnuTools.Text = "Tools";
			this.mnuTools.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.mnuTools.ToolTipText = "There are new health impact functions available online";
			// 
			// airQualityGridAggregationToolStripMenuItem
			// 
			this.airQualityGridAggregationToolStripMenuItem.Name = "airQualityGridAggregationToolStripMenuItem";
			this.airQualityGridAggregationToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.airQualityGridAggregationToolStripMenuItem.Text = "Aggregate Air Quality Surface";
			this.airQualityGridAggregationToolStripMenuItem.Click += new System.EventHandler(this.airQualityGridAggregationToolStripMenuItem_Click);
			// 
			// modelFileConcatenatorToolStripMenuItem
			// 
			this.modelFileConcatenatorToolStripMenuItem.Name = "modelFileConcatenatorToolStripMenuItem";
			this.modelFileConcatenatorToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.modelFileConcatenatorToolStripMenuItem.Text = "Model File Concatenator";
			this.modelFileConcatenatorToolStripMenuItem.Visible = false;
			this.modelFileConcatenatorToolStripMenuItem.Click += new System.EventHandler(this.modelFileConcatenatorToolStripMenuItem_Click);
			// 
			// databaseExportToolStripMenuItem
			// 
			this.databaseExportToolStripMenuItem.Name = "databaseExportToolStripMenuItem";
			this.databaseExportToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.databaseExportToolStripMenuItem.Text = "Database Export (Old)";
			this.databaseExportToolStripMenuItem.Visible = false;
			this.databaseExportToolStripMenuItem.Click += new System.EventHandler(this.databaseExportToolStripMenuItem_Click);
			// 
			// databaseExportNewToolStripMenuItem
			// 
			this.databaseExportNewToolStripMenuItem.Name = "databaseExportNewToolStripMenuItem";
			this.databaseExportNewToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.databaseExportNewToolStripMenuItem.Text = "Database Export";
			this.databaseExportNewToolStripMenuItem.Click += new System.EventHandler(this.databaseExport2ToolStripMenuItem_Click);
			// 
			// databaseImportToolStripMenuItem
			// 
			this.databaseImportToolStripMenuItem.Name = "databaseImportToolStripMenuItem";
			this.databaseImportToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.databaseImportToolStripMenuItem.Text = "Database Import (Old)";
			this.databaseImportToolStripMenuItem.Visible = false;
			this.databaseImportToolStripMenuItem.Click += new System.EventHandler(this.databaseImportToolStripMenuItem_Click);
			// 
			// databaseImportNewToolStripMenuItem
			// 
			this.databaseImportNewToolStripMenuItem.Name = "databaseImportNewToolStripMenuItem";
			this.databaseImportNewToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.databaseImportNewToolStripMenuItem.Text = "Database Import";
			this.databaseImportNewToolStripMenuItem.Click += new System.EventHandler(this.databaseImportNewToolStripMenuItem_Click);
			// 
			// onlineDatabaseExportToolStripMenuItem
			// 
			this.onlineDatabaseExportToolStripMenuItem.Name = "onlineDatabaseExportToolStripMenuItem";
			this.onlineDatabaseExportToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.onlineDatabaseExportToolStripMenuItem.Text = "Online Database Export";
			this.onlineDatabaseExportToolStripMenuItem.Click += new System.EventHandler(this.onlineDatabaseExportToolStripMenuItem_Click);
			// 
			// onlineDatabaseImportToolStripMenuItem
			// 
			this.onlineDatabaseImportToolStripMenuItem.Image = global::BenMAP.Properties.Resources.onlineAlert_Image;
			this.onlineDatabaseImportToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.onlineDatabaseImportToolStripMenuItem.Name = "onlineDatabaseImportToolStripMenuItem";
			this.onlineDatabaseImportToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.onlineDatabaseImportToolStripMenuItem.Text = "Online Database Import";
			this.onlineDatabaseImportToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.onlineDatabaseImportToolStripMenuItem.ToolTipText = "There are new health impact functions available online";
			this.onlineDatabaseImportToolStripMenuItem.Click += new System.EventHandler(this.onlineDatabaseImportToolStripMenuItem_Click);
			// 
			// exportAirQualityGridToolStripMenuItem
			// 
			this.exportAirQualityGridToolStripMenuItem.Name = "exportAirQualityGridToolStripMenuItem";
			this.exportAirQualityGridToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.exportAirQualityGridToolStripMenuItem.Text = "Export Air Quality Surface";
			this.exportAirQualityGridToolStripMenuItem.Click += new System.EventHandler(this.exportAirQualityGridToolStripMenuItem_Click);
			// 
			// gbdRollbackToolStripMenuItem
			// 
			this.gbdRollbackToolStripMenuItem.Name = "gbdRollbackToolStripMenuItem";
			this.gbdRollbackToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.gbdRollbackToolStripMenuItem.Text = "GBD Rollback";
			this.gbdRollbackToolStripMenuItem.Click += new System.EventHandler(this.gbdRollbackToolStripMenuItem_Click);
			// 
			// gISMappingToolStripMenuItem
			// 
			this.gISMappingToolStripMenuItem.Name = "gISMappingToolStripMenuItem";
			this.gISMappingToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.gISMappingToolStripMenuItem.Text = "GIS/Mapping";
			this.gISMappingToolStripMenuItem.Visible = false;
			this.gISMappingToolStripMenuItem.Click += new System.EventHandler(this.gISMappingToolStripMenuItem_Click);
			// 
			// monitorDataConversionToolStripMenuItem
			// 
			this.monitorDataConversionToolStripMenuItem.Name = "monitorDataConversionToolStripMenuItem";
			this.monitorDataConversionToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.monitorDataConversionToolStripMenuItem.Text = "Monitor Data Conversion";
			this.monitorDataConversionToolStripMenuItem.Click += new System.EventHandler(this.monitorDataConversionToolStripMenuItem_Click);
			// 
			// neighborFileCreatorToolStripMenuItem
			// 
			this.neighborFileCreatorToolStripMenuItem.Name = "neighborFileCreatorToolStripMenuItem";
			this.neighborFileCreatorToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.neighborFileCreatorToolStripMenuItem.Text = "Neighbor File Creator";
			this.neighborFileCreatorToolStripMenuItem.Click += new System.EventHandler(this.neighborFileCreatorToolStripMenuItem_Click);
			// 
			// PopSIMtoolStripMenuItem
			// 
			this.PopSIMtoolStripMenuItem.Name = "PopSIMtoolStripMenuItem";
			this.PopSIMtoolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.PopSIMtoolStripMenuItem.Text = "PopSim";
			this.PopSIMtoolStripMenuItem.Click += new System.EventHandler(this.PopSIMtoolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.optionsToolStripMenuItem.Text = "Options";
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
			// 
			// mnuComputeCrosswalks
			// 
			this.mnuComputeCrosswalks.Name = "mnuComputeCrosswalks";
			this.mnuComputeCrosswalks.Size = new System.Drawing.Size(449, 36);
			this.mnuComputeCrosswalks.Text = "Compute Grid Crosswalks";
			this.mnuComputeCrosswalks.Click += new System.EventHandler(this.mnuComputeCrosswalks_Click);
			// 
			// computeCrosswalkMinimizedToolStripMenuItem
			// 
			this.computeCrosswalkMinimizedToolStripMenuItem.Name = "computeCrosswalkMinimizedToolStripMenuItem";
			this.computeCrosswalkMinimizedToolStripMenuItem.Size = new System.Drawing.Size(449, 36);
			this.computeCrosswalkMinimizedToolStripMenuItem.Text = "(test) Compute Crosswalk Minimized";
			this.computeCrosswalkMinimizedToolStripMenuItem.Visible = false;
			this.computeCrosswalkMinimizedToolStripMenuItem.Click += new System.EventHandler(this.computeCrosswalkMinimizedToolStripMenuItem_Click);
			// 
			// btnMATs
			// 
			this.btnMATs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnMATs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ozoneAnalysisToolStripMenuItem,
            this.visibilityAnalysisToolStripMenuItem,
            this.annualPMAnalysisToolStripMenuItem,
            this.btnMATsDaily});
			this.btnMATs.Image = ((System.Drawing.Image)(resources.GetObject("btnMATs.Image")));
			this.btnMATs.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnMATs.Name = "btnMATs";
			this.btnMATs.Size = new System.Drawing.Size(81, 31);
			this.btnMATs.Text = "MATS";
			this.btnMATs.Visible = false;
			// 
			// ozoneAnalysisToolStripMenuItem
			// 
			this.ozoneAnalysisToolStripMenuItem.Enabled = false;
			this.ozoneAnalysisToolStripMenuItem.Name = "ozoneAnalysisToolStripMenuItem";
			this.ozoneAnalysisToolStripMenuItem.Size = new System.Drawing.Size(293, 36);
			this.ozoneAnalysisToolStripMenuItem.Text = "Ozone Analysis";
			// 
			// visibilityAnalysisToolStripMenuItem
			// 
			this.visibilityAnalysisToolStripMenuItem.Enabled = false;
			this.visibilityAnalysisToolStripMenuItem.Name = "visibilityAnalysisToolStripMenuItem";
			this.visibilityAnalysisToolStripMenuItem.Size = new System.Drawing.Size(293, 36);
			this.visibilityAnalysisToolStripMenuItem.Text = "Visibility Analysis";
			// 
			// annualPMAnalysisToolStripMenuItem
			// 
			this.annualPMAnalysisToolStripMenuItem.Enabled = false;
			this.annualPMAnalysisToolStripMenuItem.Name = "annualPMAnalysisToolStripMenuItem";
			this.annualPMAnalysisToolStripMenuItem.Size = new System.Drawing.Size(293, 36);
			this.annualPMAnalysisToolStripMenuItem.Text = "Annual PM Analysis";
			// 
			// btnMATsDaily
			// 
			this.btnMATsDaily.Name = "btnMATsDaily";
			this.btnMATsDaily.Size = new System.Drawing.Size(293, 36);
			this.btnMATsDaily.Text = "Daily PM Analysis";
			// 
			// btnHelp
			// 
			this.btnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOverview,
            this.userDocumentationToolStripMenuItem,
            this.mnuAbout,
            this.errorReportingToolStripMenuItem});
			this.btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("btnHelp.Image")));
			this.btnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.Size = new System.Drawing.Size(72, 31);
			this.btnHelp.Text = "Help";
			// 
			// mnuOverview
			// 
			this.mnuOverview.Name = "mnuOverview";
			this.mnuOverview.Size = new System.Drawing.Size(303, 36);
			this.mnuOverview.Text = "Quick Start Guide";
			this.mnuOverview.Click += new System.EventHandler(this.mnuOverview_Click);
			// 
			// userDocumentationToolStripMenuItem
			// 
			this.userDocumentationToolStripMenuItem.Name = "userDocumentationToolStripMenuItem";
			this.userDocumentationToolStripMenuItem.Size = new System.Drawing.Size(303, 36);
			this.userDocumentationToolStripMenuItem.Text = "User Documentation";
			this.userDocumentationToolStripMenuItem.Click += new System.EventHandler(this.userDocumentationToolStripMenuItem_Click);
			// 
			// mnuAbout
			// 
			this.mnuAbout.Name = "mnuAbout";
			this.mnuAbout.Size = new System.Drawing.Size(303, 36);
			this.mnuAbout.Text = "About";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// errorReportingToolStripMenuItem
			// 
			this.errorReportingToolStripMenuItem.Name = "errorReportingToolStripMenuItem";
			this.errorReportingToolStripMenuItem.Size = new System.Drawing.Size(303, 36);
			this.errorReportingToolStripMenuItem.Text = "Provide Feedback";
			this.errorReportingToolStripMenuItem.Click += new System.EventHandler(this.errorReportingToolStripMenuItem_Click);
			// 
			// btnUSACase
			// 
			this.btnUSACase.Name = "btnUSACase";
			this.btnUSACase.Size = new System.Drawing.Size(32, 19);
			// 
			// btnChinaCase
			// 
			this.btnChinaCase.Name = "btnChinaCase";
			this.btnChinaCase.Size = new System.Drawing.Size(32, 19);
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 1055);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
			this.statusStrip1.Size = new System.Drawing.Size(1408, 32);
			this.statusStrip1.TabIndex = 8;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// lblStatus
			// 
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(64, 25);
			this.lblStatus.Text = "Status:";
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 22F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1408, 1087);
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.ToolStripFile);
			this.Controls.Add(this.statusStrip1);
			this.HelpButton = true;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "BenMAP-CE ";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
			this.Load += new System.EventHandler(this.Main_Load);
			this.ToolStripFile.ResumeLayout(false);
			this.ToolStripFile.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		private System.Windows.Forms.ToolTip tipBallon;
		private System.Windows.Forms.ToolStrip ToolStripFile;
		private System.Windows.Forms.ToolStripDropDownButton toolItemFile;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripDropDownButton mnuFile;
		private System.Windows.Forms.ToolStripMenuItem mnuNewFile;
		private System.Windows.Forms.ToolStripMenuItem mnuOpenFile;
		private System.Windows.Forms.ToolStripSeparator mnuRecentFileSep;
		private System.Windows.Forms.ToolStripMenuItem mnuExit;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.ToolStripDropDownButton btnMATs;
		private System.Windows.Forms.ToolStripMenuItem ozoneAnalysisToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem visibilityAnalysisToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem annualPMAnalysisToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem btnMATsDaily;
		private System.Windows.Forms.ToolStripDropDownButton btnHelp;
		private System.Windows.Forms.ToolStripSeparator mnuToolStripSeparator2;
		private System.Windows.Forms.ToolStripDropDownButton mnuAnalysis;
		private System.Windows.Forms.ToolStripMenuItem mnuCustom;
		private System.Windows.Forms.ToolStripMenuItem mnuOneStepAnalysis;
		private System.Windows.Forms.ToolStripMenuItem mnuOverview;
		private System.Windows.Forms.ToolStripDropDownButton mnuActiveSetup;
		private System.Windows.Forms.ToolStripMenuItem btnUSACase;
		private System.Windows.Forms.ToolStripMenuItem btnChinaCase;
		private System.Windows.Forms.ToolStripDropDownButton mnuTools;
		private System.Windows.Forms.ToolStripMenuItem airQualityGridAggregationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem modelFileConcatenatorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem databaseExportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem databaseImportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem onlineDatabaseExportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem onlineDatabaseImportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportAirQualityGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gISMappingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem neighborFileCreatorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuAbout;
		private System.Windows.Forms.ToolStripMenuItem mnuSave;
		private System.Windows.Forms.ToolStripMenuItem mnuSaveAs;
		private System.Windows.Forms.ToolStripStatusLabel lblStatus;
		private System.Windows.Forms.ToolStripButton mnuModifySetup;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PopSIMtoolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem errorReportingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gbdRollbackToolStripMenuItem;
		private ToolStripMenuItem monitorDataConversionToolStripMenuItem;
		private ToolStripMenuItem mnuComputeCrosswalks;
		private ToolStripMenuItem computeCrosswalkMinimizedToolStripMenuItem;
		private ToolStripMenuItem databaseExportNewToolStripMenuItem;
		private ToolStripMenuItem databaseImportNewToolStripMenuItem;
		private ToolStripMenuItem userDocumentationToolStripMenuItem;
		private ToolStripMenuItem printMapLayoutToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
	}
}

