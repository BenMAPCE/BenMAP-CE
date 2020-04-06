using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Collections.Specialized;
using System.Xml;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using BenMAP.Crosswalks;
using DataConversion;

namespace BenMAP
{
    public partial class Main : FormBase
    {
        private const string _readyImageKey = "ready";

        private const string _unreadyImageKey = "unready";

        private string _baseFormTitle = "";

        private BenMAP _currentForm = null;
        private string _status = "";
        public string Status
        {
            get { return _status; }
            set { _status = value; lblStatus.Text = value; }
        }
        private string _projFileName = "";
        private bool _hasSave = true;

        private void SetCurrentStat()
        {
            try
            {
                if (CommonClass.CurrentMainFormStat != string.Empty)
                {
                    lblStatus.Text = CommonClass.CurrentMainFormStat;
                    if (CommonClass.CurrentMainFormStat.Contains("Current Setup"))
                    {
                        (_currentForm as BenMAP).changeBaseControlDelta();

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void SetCurrentSetup()
        {
            try
            {
                if (CommonClass.MainSetup != null)
                {
                    this.mnuActiveSetup.Text = CommonClass.MainSetup.SetupName;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        //public void CheckFirebirdAndStartFirebird()
        public bool CheckFirebirdAndStartFirebird()
        {
            try
            {
                bool isOK = true;
                try
                {
                    //     MessageBox.Show("Database Path Information: "+ CommonClass.Connection.ConnectionString);
                    string commandText = "select SetupID,SetupName from Setups order by SetupID";
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);


                    //    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
                    return isOK;
                }
                catch (Exception ex)
                {
                    isOK = false;

                    //MessageBox.Show(ex.StackTrace);
                    MessageBox.Show("Unable to load database at " + CommonClass.Connection.ConnectionString + ".");//\nReason: "+ex.ToString());
                    return isOK;
                }

                //   if (isOK == true) return isOK;
                // ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
                //string str = settings.ConnectionString;
                //if (!str.Contains(":"))
                //    str = Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
                //else
                //    str = str.Substring(str.IndexOf("initial catalog=") + 16);
                //str = str.Substring(0, str.IndexOf(";"));
                //if (!File.Exists(str))
                //{
                //    MessageBox.Show(string.Format("The BenMAP database file {0} does not exist.", str));
                //    Environment.Exit(0);
                //}
                /*
                try
                {

                    System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController("FirebirdServerBenMAP-CE");
                    if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running)
                        service.Start();
                    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);


                }
                catch
                {
                    MessageBox.Show("Firebird was not installed successfully. Please install it again.");
                    Process pro = new Process();
                    pro.StartInfo.UseShellExecute = true;
                    pro.StartInfo.FileName = Application.StartupPath + @"\NecessarySoftware\Firebird-2.1.2_Win32.exe";
                    pro.Start();

                    Environment.Exit(0);
                }
                try
                {
                    System.ServiceProcess.ServiceController serviceGuard = new System.ServiceProcess.ServiceController("FirebirdServerBenMAP-CE");
                    if (serviceGuard.Status != System.ServiceProcess.ServiceControllerStatus.Running)
                        serviceGuard.Start();
                    serviceGuard.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);

                }
                catch
                {
                    try
                    {
                        Process pro = new Process();
                        pro.StartInfo.UseShellExecute = true;
                        pro.StartInfo.FileName = Application.StartupPath + @"\NecessarySoftware\Firebird-2.1.2_Win32.exe";
                        pro.Start();
                    }
                    catch
                    {
                    }
                }*/
            }
            catch (Exception ex)
            {
                //              System.Console.WriteLine(ex.StackTrace);

                return false;
                Environment.Exit(0);
            }
        }


        public Main()
        {
            try
            {
                InitializeComponent();

                //  CheckFirebirdAndStartFirebird();
                if (CheckFirebirdAndStartFirebird() == false)
                {
                    MessageBox.Show("Firebird Database connection not found.");
                }
                _baseFormTitle = this.Text + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);

                mnuOverview.Text = "Quick-Start Guide";
                this.Text = _baseFormTitle;

                string sPicName = "";
                CommonClass.ActiveSetup = "USA";
                string commandText = "select SetupID,SetupName,SetupProjection from Setups order by SetupID";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
                string defaultSetup = "United States";
                if (System.IO.File.Exists(iniPath))
                {
                    defaultSetup = CommonClass.IniReadValue("appSettings", "DefaultSetup", iniPath);
                }
                else
                {
                    CommonClass.IniWriteValue("appSettings", "IsShowStart", "T", iniPath);
                    CommonClass.IniWriteValue("appSettings", "IsShowExit", "T", iniPath);
                    CommonClass.IniWriteValue("appSettings", "DefaultSetup", "United States", iniPath);
                    CommonClass.IniWriteValue("appSettings", "IsForceValidate", "T", iniPath);
                    CommonClass.IniWriteValue("appSettings", "NumDaysToDelete", "30", iniPath);
                }
                DataRow[] drs = ds.Tables[0].Select("SetupName='" + defaultSetup + "'");
                DataRow dr;
                if (drs != null && drs.Count() > 0)
                    dr = drs[0];
                else
                {
                    dr = ds.Tables[0].Rows[0];
                    CommonClass.IniWriteValue("appSettings", "DefaultSetup", dr["SetupName"].ToString(), iniPath);
                }
                BenMAPSetup benMAPSetup = new BenMAPSetup()
                {
                    SetupID = Convert.ToInt32(dr["SetupID"]),
                    SetupName = dr["SetupName"].ToString()
                };
                if (dr["SetupProjection"] != DBNull.Value)
                {
                    benMAPSetup.SetupProjection = dr["SetupProjection"].ToString();
                }

                mnuActiveSetup.DropDownItems.Clear();

                foreach (DataRow drSetup in ds.Tables[0].Rows)
                {
                    BenMAPSetup benMAPSetupIn = new BenMAPSetup()
                    {
                        SetupID = Convert.ToInt32(drSetup["SetupID"]),
                        SetupName = drSetup["SetupName"].ToString()
                    };
                    if (drSetup["SetupProjection"] != DBNull.Value)
                    {
                        benMAPSetupIn.SetupProjection = drSetup["SetupProjection"].ToString();
                    }
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
                    toolStripMenuItem.Text = drSetup["SetupName"].ToString();
                    toolStripMenuItem.Tag = benMAPSetupIn;
                    toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);
                    mnuActiveSetup.DropDownItems.Add(toolStripMenuItem);

                }
                if (mnuActiveSetup.DropDownItems.Count > 0)
                    mnuActiveSetup.Text = dr["SetupName"].ToString();
                CommonClass.MainSetup = benMAPSetup;
                CommonClass.ManageSetup = benMAPSetup;

                CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);
                if (CommonClass.InputParams == null || CommonClass.InputParams.Length == 0)
                {
                    StartPage startFrm = new StartPage();
                    startFrm.ShowDialog();
                }
                else if (CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {

                    this.Hide();
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                    return;
                }
                LoadForm(new BenMAP(sPicName));
                InitRecentFile();



                CommonClass.FormChangedStat -= SetCurrentStat;
                CommonClass.FormChangedStat += SetCurrentStat;

                CommonClass.FormChangedSetup -= SetCurrentSetup;
                CommonClass.FormChangedSetup += SetCurrentSetup;



                if (_currentForm != null)
                {
                    BenMAP frm = _currentForm as BenMAP;
                    if (frm != null)
                    {
                        frm.OpenFile();
                        frm.loadInputParamProject();
                    }
                    frm.mainFrm = this;
                }
                this.Status = "Current Setup: " + CommonClass.MainSetup.SetupName;
                lblStatus.Text = this.Status;


                //check for Jira Connector
                if ((!String.IsNullOrEmpty(CommonClass.JiraConnectorFilePath)) && (!String.IsNullOrEmpty(CommonClass.JiraConnectorFilePathTXT)))
                {
                    errorReportingToolStripMenuItem.Visible = true;
                }
                else
                {
                    errorReportingToolStripMenuItem.Visible = false;
                }




            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This is the event that occurs when changing setups from the main menu dropdown.
            if (CommonClass.MainSetup != null && ((sender as ToolStripMenuItem).Tag as BenMAPSetup).SetupID == CommonClass.MainSetup.SetupID)
                return;
            string msg = string.Empty;
            string tip = string.Empty;
            if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
            {
                msg = "Baseline or control air quality surface is being created. Please wait.";
                tip = "Please wait";
                MessageBox.Show(msg, tip, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0 && MessageBox.Show("Save the current case before switching to another case?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                mnuSaveAs_Click(sender, e);
                if (!_hasSave) return;
            }
            CommonClass.MainSetup = (sender as ToolStripMenuItem).Tag as BenMAPSetup;
            CommonClass.ManageSetup = (sender as ToolStripMenuItem).Tag as BenMAPSetup;
            BenMAP frm = _currentForm as BenMAP;
            frm.OpenFile();
            CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _projFileName = "";
            if (CommonClass.InputParams != null && CommonClass.InputParams.Count() != 0
                     && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
            {
                if (BatchCommonClass.RunBatch(CommonClass.InputParams[0]) == false)
                {
                    System.Console.WriteLine("false return from batch call");
                };
                Console.Write("Press Enter To Exit Batch Mode");
                Console.ReadLine();

                Environment.Exit(0);
            }
            CommonClass.BenMAPForm = _currentForm as BenMAP;
            String errorcode = "0";
            if (_currentForm == null)
            {
                errorcode = "1";
                MessageBox.Show("Currentform is null, this happens when DB is not found or is locked by another process.  Exiting BenMAP.");
                return;
            }

            try
            {   //NOT EQUAL
                if (!(_currentForm as BenMAP).HomePageName.Equals(null))
                {
                    if ((_currentForm as BenMAP).HomePageName != "")
                    {
                        (_currentForm as BenMAP).loadHomePageFunction();
                    }
                    //    else
                    //    {
                    //        MessageBox.Show("Home page name is empty string");
                    //    }
                }

                else
                {
                    MessageBox.Show("CurrentForm.homepageName is Null");
                }
                CommonClass.EmptyTmpFolder();
            }
            catch (Exception ex)
            {
                //  MessageBox.Show("Database may be broken. Please install BenMAP CE again.");
                MessageBox.Show("Error " + errorcode + " found launching application: " + ex.ToString() + "\n Stack trace : " + ex.StackTrace);
                Environment.Exit(0);
            }


        }

        void LoadForm(Form destForm)
        {
            this.Visible = false;
            destForm.Visible = false;
            destForm.TopLevel = false;
            destForm.ShowIcon = false;
            destForm.ShowInTaskbar = false;
            destForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            destForm.Dock = DockStyle.Fill;
            destForm.BackColor = Color.FromArgb(240, 236, 230);

            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(destForm);
            _currentForm = (BenMAP)destForm;
            destForm.Show();

        }

        private void mnuOneStepAnalysis_Click(object sender, EventArgs e)
        {
            LoadForm(new BenMAP(""));
            CommonClass.ActiveSetup = "";
        }

        private void mnuOverview_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.epa.gov/benmap/benmap-ce-training-materials");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void InitRecentFile()
        {
            try
            {
                mnuRecentFileSep.Visible = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void mnuRecentFile0_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
                if (menuItem == null) { return; }
                string tag = menuItem.Tag.ToString();
                string path = "";

                if (_currentForm != null)
                {
                    BenMAP frm = _currentForm as BenMAP;
                    if (frm != null)
                    {
                        switch (tag)
                        {
                            case "China Case":
                                CommonClass.ActiveSetup = "China";
                                path = Application.StartupPath + @"\Configs\ParamsTree_China.xml";
                                break;
                            case "USA Case":
                                CommonClass.ActiveSetup = "USA";
                                path = Application.StartupPath + @"\Configs\ParamsTree_USA.xml";
                                break;
                            default:
                                break;
                        }
                        frm.OpenFile(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnNewFile_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = string.Empty;
                string tip = string.Empty;
                if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                {
                    msg = "Baseline or control air quality surface is being created. Please wait.";
                    tip = "Please wait";
                    MessageBox.Show(msg, tip, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_currentForm != null)
                {
                    BenMAP frm = _currentForm as BenMAP;
                    if (frm != null)
                    {
                        if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0 && MessageBox.Show("Save the current case before switching to another case?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            mnuSaveAs_Click(sender, e);
                            if (_hasSave)
                            {
                                frm.OpenFile();
                                _projFileName = "";
                            }
                        }
                        else
                        {
                            frm.OpenFile();
                            _projFileName = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = string.Empty;
                string tip = string.Empty;
                if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                {
                    msg = "Baseline or control air quality surface is being created. Please wait.";
                    tip = "Please wait";
                    MessageBox.Show(msg, tip, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_currentForm != null)
                {
                    BenMAP frm = _currentForm as BenMAP;
                    if (frm != null)
                    {
                        frm.OpenProject();
                        if (frm.ProjFileName != "")
                        {
                            _projFileName = frm.ProjFileName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            CommonClass.EmptyTmpFolder();
            this.Close();
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            About frm = new About();
            frm.ShowDialog();
        }

        private void gISMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void mnuCustomSetup_Click(object sender, EventArgs e)
        {
        }

        private void mnuOneSetup_Click(object sender, EventArgs e)
        {
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CommonClass.LstPollutant == null && CommonClass.LstPollutant.Count == 0) return;
                if (_projFileName == "" || !File.Exists(_projFileName))
                {
                    mnuSaveAs_Click(sender, e);
                }
                else
                {
                    string msg = string.Empty;
                    string tip = string.Empty;
                    if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                    {
                        msg = "Baseline or control air quality surface is being created. Please wait.";
                        tip = "Please wait";
                        MessageBox.Show(msg, tip, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    CommonClass.SaveBenMAPProject(_projFileName);
                    MessageBox.Show("BenMAP project file saved.", "File saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            { }
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                if (CommonClass.LstPollutant == null && CommonClass.LstPollutant.Count == 0) return;
                string msg = string.Empty;
                string tip = string.Empty;
                if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                {
                    msg = "Baseline or control air quality surface is being created. Please wait.";
                    tip = "Please wait";
                    MessageBox.Show(msg, tip, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.Filter = "BenMAP Project File(*.projx)|*.projx";
                saveDlg.Title = "Save as";
                saveDlg.InitialDirectory = CommonClass.ResultFilePath + @"\Result\Project";

                if (saveDlg.ShowDialog() != DialogResult.OK)
                { _hasSave = false; return; }
                else
                    _hasSave = true;
                _projFileName = saveDlg.FileName;
                CommonClass.SaveBenMAPProject(saveDlg.FileName);
                MessageBox.Show("BenMAP project file saved.", "File saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }


        }
        private void mnuModifySetup_Click(object sender, EventArgs e)
        {
            ManageSetup frm = new ManageSetup();
            DialogResult dialogResult = frm.ShowDialog();

            WaitShow("Verifying BenMAP Setups...");
            if (_currentForm != null)
            {
                BenMAP frmBenMAP = _currentForm as BenMAP;
                if (frmBenMAP != null)
                {
                    frmBenMAP.InitAggregationAndRegionList();
                    frmBenMAP.RemoveAdminGroup(); //because it may have changed
                    frmBenMAP.addAdminLayers();
                    frmBenMAP.MoveAdminGroupToTop(); //in case there are other groups in the map
                }

            }

            string commandText = "select SetupID,SetupName,SetupProjection from Setups order by SetupID";
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

            mnuActiveSetup.DropDownItems.Clear();

            foreach (DataRow drSetup in ds.Tables[0].Rows)
            {
                BenMAPSetup benMAPSetupIn = new BenMAPSetup()
                {
                    SetupID = Convert.ToInt32(drSetup["SetupID"]),
                    SetupName = drSetup["SetupName"].ToString()
                };
                if (drSetup["SetupProjection"] != DBNull.Value)
                {
                    benMAPSetupIn.SetupProjection = drSetup["SetupProjection"].ToString();
                }

                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
                toolStripMenuItem.Text = drSetup["SetupName"].ToString();
                toolStripMenuItem.Tag = benMAPSetupIn;
                toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);
                mnuActiveSetup.DropDownItems.Add(toolStripMenuItem);

            }
            DataRow[] dr = ds.Tables[0].Select("SETUPNAME ='" + mnuActiveSetup.Text + "'");
            if (dr.Count() <= 0 && ds.Tables[0].Rows.Count > 0)
            {
                mnuActiveSetup.Text = ds.Tables[0].Rows[0]["SetupName"].ToString();
            }

            CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);
            DataSourceCommonClass._dicSeasonStaticsAll = null;
            bool isDel = false;
            if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
            {
                for (int iPollutant = 0; iPollutant < CommonClass.LstPollutant.Count; iPollutant++)
                {
                    try
                    {
                        CommonClass.LstPollutant[iPollutant] = CommonClass.lstPollutantAll.Where(p => p.PollutantID == CommonClass.LstPollutant[iPollutant].PollutantID).First();
                    }
                    catch
                    {
                        isDel = true;
                        break;
                    }
                }
            }
            if (isDel)
            {
                if (_currentForm != null)
                {
                    BenMAP frmBenMAP = _currentForm as BenMAP;
                    if (frmBenMAP != null)
                    {
                        if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0 && MessageBox.Show("The selected pollutant no longer exists in the database. Please select other pollutants. Save the current case before switching to another case?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            mnuSaveAs_Click(sender, e);
                            if (_hasSave)
                            {
                                frmBenMAP.OpenFile();
                                _projFileName = "";
                            }
                        }
                        else
                        {
                            frmBenMAP.OpenFile();
                            _projFileName = "";
                        }
                    }
                }
                return;
            }
            if (CommonClass.LstBaseControlGroup != null && CommonClass.LstBaseControlGroup.Count > 0)
            {
                foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                {
                    b.Pollutant = CommonClass.lstPollutantAll.Where(p => p.PollutantID == b.Pollutant.PollutantID).First();
                    if (b.Base != null && b.Base.Pollutant != null)
                        b.Base.Pollutant = b.Pollutant;
                    if (b.Control != null && b.Control.Pollutant != null)
                        b.Control.Pollutant = b.Pollutant;
                }
            }
            if (CommonClass.BaseControlCRSelectFunction != null && CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction != null && CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count > 0)
            {
                foreach (CRSelectFunction cr in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    List<BenMAPPollutant> lstpollutant = CommonClass.lstPollutantAll.Where(p => p.PollutantID == cr.BenMAPHealthImpactFunction.Pollutant.PollutantID).ToList();
                    if (lstpollutant != null && lstpollutant.Count > 0)
                        cr.BenMAPHealthImpactFunction.Pollutant = lstpollutant.First();
                }
            }
            WaitClose();
        }

        private void airQualityGridAggregationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AirQualitySurfaceAggregation frm = new AirQualitySurfaceAggregation();
            DialogResult rtn = frm.ShowDialog();
        }

        private void exportAirQualityGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportAirQualitySurface frm = new ExportAirQualitySurface();
            DialogResult rtn = frm.ShowDialog();
        }

        private void neighborFileCreatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNeighborsFile frm = new CreateNeighborsFile();
            DialogResult rtn = frm.ShowDialog();
        }

        private void modelFileConcatenatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelFileConcatenator frm = new ModelFileConcatenator();
            DialogResult rtn = frm.ShowDialog();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private void databaseExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatabaseExport frm = new DatabaseExport();
            DialogResult rtn = frm.ShowDialog();
        }

        private void databaseExport2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatabaseExport2 frm = new DatabaseExport2();
            DialogResult rtn = frm.ShowDialog();
        }

        private void databaseImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatabaseImport frm = new DatabaseImport();
            DialogResult rtn = frm.ShowDialog();
        }

        private void onlineDatabaseExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineDatabaseExport frm = new OnlineDatabaseExport();
            DialogResult rtn = frm.ShowDialog();
        }

        private void onlineDatabaseImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineDatabaseImport frm = new OnlineDatabaseImport();
            DialogResult rtn = frm.ShowDialog();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (CommonClass.InputParams != null
&& CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    e.Cancel = true;
                    return;
                }
                ExitConfirm exit = new ExitConfirm();
                string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
                string isShowExit = "T";
                if (System.IO.File.Exists(iniPath))
                {
                    isShowExit = CommonClass.IniReadValue("appSettings", "IsShowExit", iniPath);
                }
                DialogResult rtn = new DialogResult();
                if (isShowExit == "T")
                { rtn = exit.ShowDialog(); }
                if (rtn == System.Windows.Forms.DialogResult.Cancel) { e.Cancel = true; return; }
                deleteValidationLogFiles();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        //this is for deleting the validation log files after the specified time listed in the BenMAP.ini file
        private void deleteValidationLogFiles()
        {//doing clean up.
            string validationResultsPath = CommonClass.ResultFilePath + @"\ValidationResults";
            //exit proc if dir does not exist
            if (!Directory.Exists(validationResultsPath))
            {
                return;
            }

            string[] strFiles = System.IO.Directory.GetFiles(validationResultsPath, "*rtf");
            string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            int NumDaysToDelete = Convert.ToInt32(CommonClass.IniReadValue("appSettings", "NumDaysToDelete", iniPath));
            DateTime createDate;
            try
            {
                System.IO.FileInfo fInfo = null;
                foreach (string s in strFiles)
                {
                    fInfo = new System.IO.FileInfo(s);
                    createDate = fInfo.CreationTime;

                    if (createDate.Date < DateTime.Now.Date.Subtract(TimeSpan.FromDays(NumDaysToDelete)))
                    {
                        System.IO.File.Delete(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options frm = new Options();
            DialogResult rtn = frm.ShowDialog();
        }

        private void PopSIMtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopSim.frm_PopSim frm = new PopSim.frm_PopSim();
            DialogResult rtn = frm.ShowDialog();
        }
        private void errorReportingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorReporting frm = new ErrorReporting();
            frm.ShowDialog();
        }

        private void gbdRollbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GBDRollback frm = new GBDRollback();
            frm.ShowDialog();

        }

        private void monitorDataConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataConversionTool frm = new DataConversionTool();
            frm.ShowDialog();
        }

        private void mnuComputeCrosswalks_Click(object sender, EventArgs e)
        {
            /* dpa 1/28/2017 New menu item to manually compute grid crosswalks using new algorithm
             * Current approach is to use the new algorithm through this manual calculator which will
             * update the records in the database that are used by the other functions in code.
             */
            using (var f = new CrosswalksConfiguration() { Owner = this })
            {
                f.StartPosition = FormStartPosition.CenterParent;
                f.ShowDialog();
            }
        }

        private void computeCrosswalkMinimizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CrosswalksConfiguration f = new CrosswalksConfiguration();
            f.StartPosition = FormStartPosition.CenterParent;
            f.RunCompact(19, 18, null);
        }

        private void databaseImportNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatabaseImport2 frm = new DatabaseImport2();
            DialogResult rtn = frm.ShowDialog();
        }

        private void userDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.epa.gov/benmap/manual-and-appendices-benmap-ce");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void printMapLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _currentForm.SetUpPrintLayout();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Debug.WriteLine("tsbSavePic_Click: " + ex.ToString());
            }
        }

        TipFormGIF waitMess = new TipFormGIF(); 
        bool sFlog = true;
        private delegate void CloseFormDelegate();
        public void WaitShow(string msg)
        {
            try
            {
                if (sFlog == true)
                {
                    sFlog = false;
                    waitMess.Msg = msg;
                    System.Threading.Thread upgradeThread = null;
                    upgradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWaitMess));
                    upgradeThread.Start();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private void ShowWaitMess()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    waitMess.ShowDialog();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        public void WaitClose()
        {
            if (waitMess.InvokeRequired)
                waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
            else
                DoCloseJob();
        }

        private void DoCloseJob()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    if (waitMess.Created)
                    {
                        sFlog = true;
                        waitMess.Close();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
    }
}
