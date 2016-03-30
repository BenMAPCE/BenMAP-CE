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
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActiveSetup = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnUSACase = new System.Windows.Forms.ToolStripMenuItem();
            this.btnChinaCase = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModifySetup = new System.Windows.Forms.ToolStripButton();
            this.mnuAnalysis = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuCustom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOneStepAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGIS = new System.Windows.Forms.ToolStripButton();
            this.mnuTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.airQualityGridAggregationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelFileConcatenatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineDatabaseExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineDatabaseImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAirQualityGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbdRollbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gISMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neighborFileCreatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PopSIMtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMATs = new System.Windows.Forms.ToolStripDropDownButton();
            this.ozoneAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visibilityAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.annualPMAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMATsDaily = new System.Windows.Forms.ToolStripMenuItem();
            this.btnHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuOverview = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.errorReportingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.newToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.openToolStripMenuItem.Text = "&Open";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            // 
            // pnlMain
            // 
            this.pnlMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 25);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1252, 835);
            this.pnlMain.TabIndex = 9;
            // 
            // ToolStripFile
            // 
            this.ToolStripFile.Font = new System.Drawing.Font("Calibri", 11F);
            this.ToolStripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuActiveSetup,
            this.mnuModifySetup,
            this.mnuAnalysis,
            this.btnGIS,
            this.mnuTools,
            this.btnMATs,
            this.btnHelp});
            this.ToolStripFile.Location = new System.Drawing.Point(0, 0);
            this.ToolStripFile.Name = "ToolStripFile";
            this.ToolStripFile.Size = new System.Drawing.Size(1252, 30);
            this.ToolStripFile.TabIndex = 4;
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
            this.mnuExit});
            this.mnuFile.Image = ((System.Drawing.Image)(resources.GetObject("mnuFile.Image")));
            this.mnuFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(44, 22);
            this.mnuFile.Text = "&File";
            // 
            // mnuOpenFile
            // 
            this.mnuOpenFile.Name = "mnuOpenFile";
            this.mnuOpenFile.Size = new System.Drawing.Size(174, 22);
            this.mnuOpenFile.Text = "&Open project";
            this.mnuOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // mnuNewFile
            // 
            this.mnuNewFile.Name = "mnuNewFile";
            this.mnuNewFile.Size = new System.Drawing.Size(174, 22);
            this.mnuNewFile.Text = "&New project";
            this.mnuNewFile.Click += new System.EventHandler(this.btnNewFile_Click);
            // 
            // mnuRecentFileSep
            // 
            this.mnuRecentFileSep.Name = "mnuRecentFileSep";
            this.mnuRecentFileSep.Size = new System.Drawing.Size(171, 6);
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(174, 22);
            this.mnuSave.Text = "&Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuSaveAs
            // 
            this.mnuSaveAs.Name = "mnuSaveAs";
            this.mnuSaveAs.Size = new System.Drawing.Size(174, 22);
            this.mnuSaveAs.Text = "&Save as(*.projx)";
            this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
            // 
            // mnuToolStripSeparator2
            // 
            this.mnuToolStripSeparator2.Name = "mnuToolStripSeparator2";
            this.mnuToolStripSeparator2.Size = new System.Drawing.Size(171, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(174, 22);
            this.mnuExit.Text = "&Exit";
            this.mnuExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // mnuActiveSetup
            // 
            this.mnuActiveSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuActiveSetup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUSACase,
            this.btnChinaCase});
            this.mnuActiveSetup.Image = ((System.Drawing.Image)(resources.GetObject("mnuActiveSetup.Image")));
            this.mnuActiveSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuActiveSetup.Name = "mnuActiveSetup";
            this.mnuActiveSetup.Size = new System.Drawing.Size(98, 22);
            this.mnuActiveSetup.Text = "Active Setup";
            // 
            // btnUSACase
            // 
            this.btnUSACase.Name = "btnUSACase";
            this.btnUSACase.Size = new System.Drawing.Size(143, 22);
            this.btnUSACase.Tag = "USA Case";
            this.btnUSACase.Text = "USA Case";
            this.btnUSACase.Click += new System.EventHandler(this.mnuRecentFile0_Click);
            // 
            // btnChinaCase
            // 
            this.btnChinaCase.Name = "btnChinaCase";
            this.btnChinaCase.Size = new System.Drawing.Size(143, 22);
            this.btnChinaCase.Tag = "China Case";
            this.btnChinaCase.Text = "China Case";
            this.btnChinaCase.Click += new System.EventHandler(this.mnuRecentFile0_Click);
            // 
            // mnuModifySetup
            // 
            this.mnuModifySetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuModifySetup.Image = ((System.Drawing.Image)(resources.GetObject("mnuModifySetup.Image")));
            this.mnuModifySetup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuModifySetup.Name = "mnuModifySetup";
            this.mnuModifySetup.Size = new System.Drawing.Size(112, 22);
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
            this.mnuAnalysis.Size = new System.Drawing.Size(124, 22);
            this.mnuAnalysis.Text = "Analysis Method";
            this.mnuAnalysis.Visible = false;
            // 
            // mnuCustom
            // 
            this.mnuCustom.Name = "mnuCustom";
            this.mnuCustom.Size = new System.Drawing.Size(187, 22);
            this.mnuCustom.Text = "Custom Analysis";
            this.mnuCustom.Click += new System.EventHandler(this.mnuCustom_Click);
            // 
            // mnuOneStepAnalysis
            // 
            this.mnuOneStepAnalysis.Name = "mnuOneStepAnalysis";
            this.mnuOneStepAnalysis.Size = new System.Drawing.Size(187, 22);
            this.mnuOneStepAnalysis.Text = "One Step Analysis";
            this.mnuOneStepAnalysis.Click += new System.EventHandler(this.mnuOneStepAnalysis_Click);
            // 
            // btnGIS
            // 
            this.btnGIS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGIS.Enabled = false;
            this.btnGIS.Image = ((System.Drawing.Image)(resources.GetObject("btnGIS.Image")));
            this.btnGIS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGIS.Name = "btnGIS";
            this.btnGIS.Size = new System.Drawing.Size(89, 22);
            this.btnGIS.Text = "GIS Mapping";
            this.btnGIS.Visible = false;
            this.btnGIS.Click += new System.EventHandler(this.btnGIS_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.airQualityGridAggregationToolStripMenuItem,
            this.modelFileConcatenatorToolStripMenuItem,
            this.databaseExportToolStripMenuItem,
            this.databaseImportToolStripMenuItem,
            this.onlineDatabaseExportToolStripMenuItem,
            this.onlineDatabaseImportToolStripMenuItem,
            this.exportAirQualityGridToolStripMenuItem,
            this.gbdRollbackToolStripMenuItem,
            this.gISMappingToolStripMenuItem,
            this.neighborFileCreatorToolStripMenuItem,
            this.PopSIMtoolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.mnuTools.Image = ((System.Drawing.Image)(resources.GetObject("mnuTools.Image")));
            this.mnuTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(62, 27);
            this.mnuTools.Text = "Tools";
            //test for new
          
                //this.mnuTools.BackColor = System.Drawing.Color.PowderBlue;
            this.mnuTools.ToolTipText = "There are new health impact functions available online";
            this.mnuTools.Image = Properties.Resources.onlineAlert_Image;
            this.mnuTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.mnuTools.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mnuTools.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            
            // 
            // airQualityGridAggregationToolStripMenuItem
            // 
            this.airQualityGridAggregationToolStripMenuItem.Name = "airQualityGridAggregationToolStripMenuItem";
            this.airQualityGridAggregationToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.airQualityGridAggregationToolStripMenuItem.Text = "Air Quality Surface Aggregation";
            this.airQualityGridAggregationToolStripMenuItem.Click += new System.EventHandler(this.airQualityGridAggregationToolStripMenuItem_Click);
            // 
            // modelFileConcatenatorToolStripMenuItem
            // 
            this.modelFileConcatenatorToolStripMenuItem.Name = "modelFileConcatenatorToolStripMenuItem";
            this.modelFileConcatenatorToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.modelFileConcatenatorToolStripMenuItem.Text = "Model File Concatenator";
            this.modelFileConcatenatorToolStripMenuItem.Visible = false;
            this.modelFileConcatenatorToolStripMenuItem.Click += new System.EventHandler(this.modelFileConcatenatorToolStripMenuItem_Click);
            // 
            // databaseExportToolStripMenuItem
            // 
            this.databaseExportToolStripMenuItem.Name = "databaseExportToolStripMenuItem";
            this.databaseExportToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.databaseExportToolStripMenuItem.Text = "Database Export";
            this.databaseExportToolStripMenuItem.Click += new System.EventHandler(this.databaseExportToolStripMenuItem_Click);
            // 
            // databaseImportToolStripMenuItem
            // 
            this.databaseImportToolStripMenuItem.Name = "databaseImportToolStripMenuItem";
            this.databaseImportToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.databaseImportToolStripMenuItem.Text = "Database Import";
            this.databaseImportToolStripMenuItem.Click += new System.EventHandler(this.databaseImportToolStripMenuItem_Click);
            // 
            // onlineDatabaseExportToolStripMenuItem
            // 
            this.onlineDatabaseExportToolStripMenuItem.Name = "onlineDatabaseExportToolStripMenuItem";
            this.onlineDatabaseExportToolStripMenuItem.Size = new System.Drawing.Size(321, 28);
            this.onlineDatabaseExportToolStripMenuItem.Text = "Online Database Export";
            this.onlineDatabaseExportToolStripMenuItem.Click += new System.EventHandler(this.onlineDatabaseExportToolStripMenuItem_Click);
            // 
            // onlineDatabaseImportToolStripMenuItem
            // 
            this.onlineDatabaseImportToolStripMenuItem.Name = "onlineDatabaseImportToolStripMenuItem";
            this.onlineDatabaseImportToolStripMenuItem.Size = new System.Drawing.Size(321, 28);
            this.onlineDatabaseImportToolStripMenuItem.Text = "Online Database Import";
            //test for new
            //this.mnuTools.BackColor = System.Drawing.Color.PowderBlue;
            this.onlineDatabaseImportToolStripMenuItem.ToolTipText = "There are new health impact functions available online";
            this.onlineDatabaseImportToolStripMenuItem.Image = Properties.Resources.onlineAlert_Image;
            this.onlineDatabaseImportToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.onlineDatabaseImportToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.onlineDatabaseImportToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            
            this.onlineDatabaseImportToolStripMenuItem.Click += new System.EventHandler(this.onlineDatabaseImportToolStripMenuItem_Click);
            // 
            // exportAirQualityGridToolStripMenuItem
            // 
            this.exportAirQualityGridToolStripMenuItem.Name = "exportAirQualityGridToolStripMenuItem";
            this.exportAirQualityGridToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.exportAirQualityGridToolStripMenuItem.Text = "Export Air Quality Surface";
            this.exportAirQualityGridToolStripMenuItem.Click += new System.EventHandler(this.exportAirQualityGridToolStripMenuItem_Click);
            // 
            // gISMappingToolStripMenuItem
            // 
            this.gISMappingToolStripMenuItem.Name = "gISMappingToolStripMenuItem";
            // 
            // gbdRollbackToolStripMenuItem
            // 
            this.gbdRollbackToolStripMenuItem.Name = "gbdRollbackToolStripMenuItem";
            this.gbdRollbackToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.gbdRollbackToolStripMenuItem.Text = "GBD Rollback";
            this.gbdRollbackToolStripMenuItem.Click += new System.EventHandler(this.gbdRollbackToolStripMenuItem_Click);
            // 
            // gISMappingToolStripMenuItem
            // 
            this.gISMappingToolStripMenuItem.Name = "gISMappingToolStripMenuItem";
            this.gISMappingToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.gISMappingToolStripMenuItem.Text = "GIS/Mapping";
            this.gISMappingToolStripMenuItem.Visible = false;
            this.gISMappingToolStripMenuItem.Click += new System.EventHandler(this.gISMappingToolStripMenuItem_Click);
            // 
            // neighborFileCreatorToolStripMenuItem
            // 
            this.neighborFileCreatorToolStripMenuItem.Name = "neighborFileCreatorToolStripMenuItem";
            this.neighborFileCreatorToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.neighborFileCreatorToolStripMenuItem.Text = "Neighbor File Creator";
            this.neighborFileCreatorToolStripMenuItem.Click += new System.EventHandler(this.neighborFileCreatorToolStripMenuItem_Click);
            // 
            // PopSIMtoolStripMenuItem
            // 
            this.PopSIMtoolStripMenuItem.Name = "PopSIMtoolStripMenuItem";
            this.PopSIMtoolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.PopSIMtoolStripMenuItem.Text = "PopSim";
            this.PopSIMtoolStripMenuItem.Click += new System.EventHandler(this.PopSIMtoolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
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
            this.btnMATs.Size = new System.Drawing.Size(55, 22);
            this.btnMATs.Text = "MATS";
            this.btnMATs.Visible = false;
            // 
            // ozoneAnalysisToolStripMenuItem
            // 
            this.ozoneAnalysisToolStripMenuItem.Enabled = false;
            this.ozoneAnalysisToolStripMenuItem.Name = "ozoneAnalysisToolStripMenuItem";
            this.ozoneAnalysisToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.ozoneAnalysisToolStripMenuItem.Text = "Ozone Analysis";
            // 
            // visibilityAnalysisToolStripMenuItem
            // 
            this.visibilityAnalysisToolStripMenuItem.Enabled = false;
            this.visibilityAnalysisToolStripMenuItem.Name = "visibilityAnalysisToolStripMenuItem";
            this.visibilityAnalysisToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.visibilityAnalysisToolStripMenuItem.Text = "Visibility Analysis";
            // 
            // annualPMAnalysisToolStripMenuItem
            // 
            this.annualPMAnalysisToolStripMenuItem.Enabled = false;
            this.annualPMAnalysisToolStripMenuItem.Name = "annualPMAnalysisToolStripMenuItem";
            this.annualPMAnalysisToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.annualPMAnalysisToolStripMenuItem.Text = "Annual PM Analysis";
            // 
            // btnMATsDaily
            // 
            this.btnMATsDaily.Name = "btnMATsDaily";
            this.btnMATsDaily.Size = new System.Drawing.Size(197, 22);
            this.btnMATsDaily.Text = "Daily PM Analysis";
            // 
            // btnHelp
            // 
            this.btnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOverview,
            this.mnuAbout,
            this.errorReportingToolStripMenuItem});
            this.btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("btnHelp.Image")));
            this.btnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(50, 22);
            this.btnHelp.Text = "Help";
            // 
            // mnuOverview
            // 
            this.mnuOverview.Name = "mnuOverview";
            this.mnuOverview.Size = new System.Drawing.Size(301, 22);
            this.mnuOverview.Text = "Quick Start Guide of BenMAP CS 0.40";
            this.mnuOverview.Click += new System.EventHandler(this.mnuOverview_Click);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(301, 22);
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // errorReportingToolStripMenuItem
            // 
            this.errorReportingToolStripMenuItem.Name = "errorReportingToolStripMenuItem";
            this.errorReportingToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.errorReportingToolStripMenuItem.Text = "Provide Feedback";
            this.errorReportingToolStripMenuItem.Click += new System.EventHandler(this.errorReportingToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            lblStatus});
            
            
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 670);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(939, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 17);
            this.lblStatus.Text = "Status:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 692);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.ToolStripFile);
            this.Controls.Add(this.statusStrip1);
            this.HelpButton = true;
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
        private System.Windows.Forms.ToolStripButton btnGIS;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripButton mnuModifySetup;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PopSIMtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorReportingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gbdRollbackToolStripMenuItem;
    }
}

