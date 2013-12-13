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
//using AxMapWinGIS;
//using MapWinGIS;
//using MapWindow6;

namespace BenMAP
{
    public partial class Main : FormBase
    {
        #region fields
        /// <summary>
        /// 表示已经有设置的图标key
        /// </summary>
        private const string _readyImageKey = "ready";

        /// <summary>
        /// 表示尚未设置的图标key
        /// </summary>
        private const string _unreadyImageKey = "unready";

        /// <summary>
        /// 基本的窗体标题,包括应用程序名称和版本
        /// </summary>
        private string _baseFormTitle = "";
        /// <summary>
        /// 当前的BenMAPForm
        /// </summary>
        private Form _currentForm = null;
        private string _status = "";
        /// <summary>
        /// 状态栏显示
        /// 默认在状态栏显示当前setup
        /// 当正在线程（异步）处理时，也要在状态栏显示
        /// </summary>
        public string Status
        {
            get { return _status; }
            set { _status = value; lblStatus.Text = value; }
        }
        private string _projFileName = "";
        private bool _hasSave = true;
        #endregion

        private void SetCurrentStat()
        {
            try
            {
                if (CommonClass.CurrentMainFormStat != string.Empty)
                {
                    lblStatus.Text = CommonClass.CurrentMainFormStat;
                    if (CommonClass.CurrentMainFormStat.Contains("Current Setup"))
                    {
                        //----------证明已经完成某个base或者control的异步
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
        public void CheckFirebirdAndStartFirebird()
        {
            try
            {
                bool isOK = true;
                try
                {
                    string commandText = "select SetupID,SetupName from Setups order by SetupID";
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                }
                catch
                {
                    isOK = false;
                }
                if (isOK == true) return;
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
                string str = settings.ConnectionString;
                if (!str.Contains(":"))
                    str = Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
                else
                    str = str.Substring(str.IndexOf("initial catalog=") + 16);
                str = str.Substring(0, str.IndexOf(";"));
                if (!File.Exists(str))
                {
                    MessageBox.Show(string.Format("The BenMAP database file {0} does not exist.", str));
                    Environment.Exit(0);
                }
                try
                {

                    System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController("FirebirdServerBenMAP-CE");
                    if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running)
                        service.Start();
                    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);

                    //FirebirdGuardianDefaultInstance

                }
                catch
                {
                    //--------run 表示firebird安装异常！--重新安装firebird----------
                    MessageBox.Show("Firebird was not installed successfully. Please install it again.");
                    Process pro = new Process();
                    pro.StartInfo.UseShellExecute = true;
                    pro.StartInfo.FileName = Application.StartupPath + @"\NecessarySoftware\Firebird-2.1.2_Win32.exe";
                    //pro.StartInfo.FileName = Application.StartupPath + @"\Database\Firebird\bin\install_super.bat";
                    //pro.StartInfo.CreateNoWindow = true;
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
                        //pro.StartInfo.FileName = Application.StartupPath + @"\Database\Firebird\bin\install_super.bat";
                        pro.StartInfo.FileName = Application.StartupPath + @"\NecessarySoftware\Firebird-2.1.2_Win32.exe";
                        //pro.StartInfo.CreateNoWindow = true;
                        pro.Start();
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {

                Environment.Exit(0);
            }
        }
        public Main()
        {
            try
            {
                InitializeComponent();
                //_baseFormTitle = this.Text + " (Ver:" + Application.ProductVersion + ") ";
                CheckFirebirdAndStartFirebird();
                _baseFormTitle = this.Text + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);// " (Version: 0.40)";
                mnuOverview.Text = "Quick-Start Guide";// "BenMAP CE Quick Start Guide " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 4);// " (Version: 0.40)";
                this.Text = _baseFormTitle;

                //2011-01-26：起始页面,3秒后自动消失
                string sPicName = "";
                CommonClass.ActiveSetup = "USA";
                string commandText = "select SetupID,SetupName from Setups order by SetupID";
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
                }
                //XmlDocument doc = new XmlDocument();
                //doc.Load(Application.ExecutablePath + ".config");
                //XmlNode node = doc.SelectSingleNode(@"//add[@key='DefaultSetup']");
                //XmlElement ele = (XmlElement)node;
                //ele = (XmlElement)node;
                //string defaultSetup = ele.GetAttribute("value");
                DataRow[] drs = ds.Tables[0].Select("SetupName='" + defaultSetup + "'");
                DataRow dr;
                if (drs != null && drs.Count() > 0)
                    dr = drs[0];
                else
                {
                    dr = ds.Tables[0].Rows[0];
                    //ele.SetAttribute("value", dr["SetupName"].ToString());
                    //doc.Save(Application.ExecutablePath + ".config");
                    //ConfigurationManager.RefreshSection("appSettings");
                    CommonClass.IniWriteValue("appSettings", "DefaultSetup", dr["SetupName"].ToString(), iniPath);
                }
                BenMAPSetup benMAPSetup = new BenMAPSetup()
                {
                    SetupID = Convert.ToInt32(dr["SetupID"]),
                    SetupName = dr["SetupName"].ToString()
                };
                mnuActiveSetup.DropDownItems.Clear();

                foreach (DataRow drSetup in ds.Tables[0].Rows)
                {
                    BenMAPSetup benMAPSetupIn = new BenMAPSetup()
                    {
                        SetupID = Convert.ToInt32(drSetup["SetupID"]),
                        SetupName = drSetup["SetupName"].ToString()
                    };
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
                    //sPicName = startFrm.sPicName;
                }
                else if(CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    //------------Batch Job-----------

                    this.Hide();
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                    return;
                    //--------------------------------
                }
                LoadForm(new BenMAP(sPicName));
                InitRecentFile();

                //// 刷新报表列表
                //ResetReportList();
                //2011-01-26：初始化时，默认打开usa case
               

                // 
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
                        //------------do for input Params---------------
                        frm.loadInputParamProject();
                        //------------end do for input params-----------
                        //frm.NewFile();
                    }
                    frm.mainFrm = this;
                }
                //2011-01-26：初始化时，默认打开usa case
                this.Status = "Current Setup: " + CommonClass.MainSetup.SetupName;
                lblStatus.Text = this.Status;   //默认在状态栏显示当前setup

                //load网格之间的关系

              
               

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                //Application.Exit();
            }
        }

        void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
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
            if (CommonClass.LstPollutant!=null && CommonClass.LstPollutant.Count>0&&MessageBox.Show("Save the current case before switching to another case?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                mnuSaveAs_Click(sender, e);
                if (!_hasSave) return;
            }
            CommonClass.MainSetup = (sender as ToolStripMenuItem).Tag as BenMAPSetup;
            CommonClass.ManageSetup = (sender as ToolStripMenuItem).Tag as BenMAPSetup;
            this.Status = "Current Setup: " + CommonClass.MainSetup.SetupName;
            lblStatus.Text = this.Status;   //默认在状态栏显示当前setup
            mnuActiveSetup.Text = (sender as ToolStripMenuItem).Text;
            //load网格之间的关系
            BenMAP frm = _currentForm as BenMAP;
            frm.OpenFile();
            CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (CommonClass.InputParams != null && CommonClass.InputParams.Count()!=0
                     && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
            {
                //------------------Batch--------------------------------------
                if (BatchCommonClass.RunBatch(CommonClass.InputParams[0]) == false)
                {
                   // MessageBox.Show("Batch file is wrong"
                };
                Environment.Exit(0);
                return;
            }
            CommonClass.BenMAPForm = _currentForm as BenMAP;
            try
            {
                if ((_currentForm as BenMAP).HomePageName != "")
                    (_currentForm as BenMAP).loadHomePageFunction();
            }
            catch
            {
                MessageBox.Show("Database may be broken. Please install BenMAP CE again.");
                Environment.Exit(0);
            }
            _projFileName = "";
           
        }

        /// <summary>
        /// 当前打开的工程
        /// </summary>
        
        void LoadForm(Form destForm)
        {
            this.Visible = false;
            destForm.Visible = false;
            // 此句必须,否则报错
            destForm.TopLevel = false;
            destForm.ShowIcon = false;
            destForm.ShowInTaskbar = false;
            // 隐藏其标题栏
            destForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            destForm.Dock = DockStyle.Fill;
            destForm.BackColor = Color.FromArgb(240, 236, 230);

            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(destForm);
            _currentForm = destForm;
            destForm.Show();

            //StartTip s = new StartTip();
            //s.Show();
        }

        /// <summary>
        /// 加载一幅图片，提示用户应该如何操作
        /// </summary>
        private void mnuCustom_Click(object sender, EventArgs e)
        {
            //CustomAnalysis customFrm = new CustomAnalysis();
            //DialogResult rtn = customFrm.ShowDialog();
            //if (rtn != DialogResult.OK)
            //{ return; }
        }

        private void mnuOneStepAnalysis_Click(object sender, EventArgs e)
        {
            LoadForm(new BenMAP(""));
            CommonClass.ActiveSetup = "";
        }

        /// <summary>
        /// 显示介绍
        /// </summary>
        private void mnuOverview_Click(object sender, EventArgs e)
        {
            try
            {
                //About frm = new About(this);
                //Overview frm = new Overview();
                //frm.ShowDialog();
                //Help.ShowHelp(this, Application.StartupPath + @"\Data\QuickStartGuide.chm");
                System.Diagnostics.Process.Start("http://www.epa.gov/air/benmap/ce.html"); 
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        #region 最近打开的文件相关
        private void InitRecentFile()
        {
            try
            {
                mnuRecentFileSep.Visible = true;

                ////增加一个美国区和一个中国区的recent File
                //ToolStripMenuItem menuItem = new ToolStripMenuItem("China Case");
                //menuItem.Click += mnuRecentFile0_Click;
                //menuItem.Tag = "China Case";
                //mnuFile.DropDownItems .Insert (mnuFile.DropDownItems.Count - 2, menuItem);

                //menuItem = new ToolStripMenuItem("USA Case");
                //menuItem.Click += mnuRecentFile0_Click;
                //menuItem.Tag = "USA Case";
                //mnuFile.DropDownItems.Insert(mnuFile.DropDownItems.Count - 2, menuItem);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// 最近打开的文件,菜单处理
        /// </summary>
        private void mnuRecentFile0_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
                if (menuItem == null) { return; }
                string tag = menuItem.Tag.ToString();
                string path = "";

                //打开中国区或美国区数据
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
                                //path = Application.StartupPath + @"\Configs\ParamsTree_USA.xml";
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

        ///// <summary>
        ///// 初始化最近打开的文件,从User Setting中获取
        ///// </summary>
        //private void InitRecentFile()
        //{
        //    try
        //    {
        //        // 本函数在每次打开一个文件后都会被调用
        //        // 取得最近使用的文件列表,加入到菜单中
        //        _recentFileCollecton = Properties.Settings.Default.RecentFiles;
        //        if (_recentFileCollecton == null || _recentFileCollecton.Count == 0)
        //        {
        //            return;
        //        }
        //        mnuRecentFileSep.Visible = true;

        //        // 移除旧的
        //        while (mnuFile.DropDownItems[mnuFile.DropDownItems.Count - 3].Name != "mnuRecentFileSep")
        //        {
        //            mnuFile.DropDownItems.RemoveAt(mnuFile.DropDownItems.Count - 3);
        //        }

        //        for (int i = 0; i < _recentFileCollecton.Count; i++)
        //        {
        //            ToolStripMenuItem menuItem = new ToolStripMenuItem();
        //            menuItem.Text = _recentFileCollecton[i];
        //            menuItem.Tag = _recentFileCollecton[i];
        //            menuItem.Click += mnuRecentFile0_Click;
        //            mnuFile.DropDownItems.Insert(mnuFile.DropDownItems.Count - 2, menuItem);
        //        }
        //        // 取得最多最近文件数
        //        _maxRecentFileCount = Properties.Settings.Default.MaxRecentFileCount;
        //        if (_maxRecentFileCount < 1) { _maxRecentFileCount = 4; }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);

        //    }
        //}

        ///// <summary>
        ///// 把打开的文件路径加入到最近打的文件列表中
        ///// </summary>
        ///// <param name="file"></param>
        //private void AddToRecentFile(string file)
        //{
        //    try
        //    {
        //        if (_recentFileCollecton == null || _recentFileCollecton.Count == 0)
        //        {
        //            _recentFileCollecton = new StringCollection();
        //            _recentFileCollecton.Add(file);
        //        }
        //        else if (!_recentFileCollecton.Contains(file))
        //        {
        //            if (_recentFileCollecton.Count < _maxRecentFileCount)
        //            {
        //                _recentFileCollecton.Add(_recentFileCollecton[_recentFileCollecton.Count - 1]);
        //                for (int i = _recentFileCollecton.Count - 2; i > 0; i--)
        //                {
        //                    _recentFileCollecton[i] = _recentFileCollecton[i - 1];
        //                }
        //            }
        //            else
        //            {
        //                for (int i = _recentFileCollecton.Count - 1; i > 0; i--)
        //                {
        //                    _recentFileCollecton[i] = _recentFileCollecton[i - 1];
        //                }
        //            }
        //            _recentFileCollecton[0] = file;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //        // 保存
        //        SaveRecentFile();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //    }
        //}

        ///// <summary>
        ///// 保存最近打开的文件列表
        ///// </summary>
        //private void SaveRecentFile()
        //{
        //    try
        //    {
        //        Properties.Settings.Default.RecentFiles = _recentFileCollecton;
        //        Properties.Settings.Default.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //    }
        //}

        ///// <summary>
        ///// 最近打开的文件,菜单处理
        ///// </summary>
        //private void mnuRecentFile0_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
        //        if (menuItem == null) { return; }
        //        string path = menuItem.Tag.ToString();
        //        if (!File.Exists(path))
        //        {
        //            DialogResult rtn = MessageBox.Show(string.Format("File {0} does not exist!\nWould you like to remove it from the recent files list?", path), "Tip", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //            if (rtn == DialogResult.Yes)
        //            {
        //                mnuFile.DropDownItems.Remove(menuItem);
        //                _recentFileCollecton.Remove(menuItem.Text);
        //                SaveRecentFile();
        //            }
        //            return;
        //        }
        //        OpenRVTFile(path);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //    }
        //}


        #endregion

        #region menus
        /// <summary>
        /// 新建BenMAP
        /// </summary>
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

        /// <summary>
        /// 打开一个BenMAP
        /// </summary>
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //CommonClass.ActiveSetup = "USA";
            //// 加载地图
            //LoadGIS(axMap);
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
            this.Close();
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            About frm = new About();
            frm.ShowDialog();
        }

        private void gISMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MainRibbonForm frm = new MainRibbonForm(null);
            //frm.ShowDialog();
        }

        /// <summary>
        /// 根据当前设定（美国区），查看自定义设定
        /// </summary>
        private void mnuCustomSetup_Click(object sender, EventArgs e)
        {
            //if (CommonClass.ActiveSetup == string.Empty)
            //{
            //    MessageBox.Show("Please new or open a BenMAP file first!", "Tip", MessageBoxButtons.OK);
            //}
            //else
            //{
            //    CurrentSetup currentFrm = new CurrentSetup(CommonClass.ActiveSetup);
            //    currentFrm.ShowDialog();
            //}
            //CommonClass.ActiveSetup = "USA";
            //CurrentSetup currentFrm = new CurrentSetup(CommonClass.ActiveSetup);
            //currentFrm.ShowDialog();
            // ManageSetup frm = new ManageSetup();
            //frm.ShowDialog();
        }

        /// <summary>
        /// 2011-01-25:根据当前设定（中国区），查看自定义设定
        /// 根据当前设定（美国区/中国区），查看treeView设定
        /// </summary>
        private void mnuOneSetup_Click(object sender, EventArgs e)
        {
            //if (CommonClass.ActiveSetup == string.Empty)
            //{
            //    MessageBox.Show("Please new or open a BenMAP file first!", "Tip", MessageBoxButtons.OK);
            //}
            //else
            //{
            //    OneStepSetup oneFrm = new OneStepSetup(CommonClass.ActiveSetup);
            //    oneFrm.ShowDialog();
            //}
            //CommonClass.ActiveSetup = "China";
            //CurrentSetup currentFrm = new CurrentSetup(CommonClass.ActiveSetup);
            //currentFrm.ShowDialog();
        }

        /// <summary>
        /// 保存：提示用户所有设置会自动保存
        /// </summary>
        private void mnuSave_Click(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("All settings will be saved automatically to xml files!", "Tip", MessageBoxButtons.OK);
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
        
        /// <summary>
        /// 另存为：
        /// </summary>
        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Save the current case before switch to other case?", "Tip", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            //{
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
            //}
            //if (CommonClass.ActiveSetup != string.Empty)
            //{
            //    SaveFileDialog saveDlg = new SaveFileDialog();
            //    saveDlg.Filter = "Configuration File(*.xml)|*.xml";
            //    saveDlg.Title = "Save as";
            //    saveDlg.InitialDirectory = Application.StartupPath + @"\Configs";

            //    if (saveDlg.ShowDialog() != DialogResult.OK)
            //    { return; }
            //    else
            //    {
            //        string strPath = saveDlg.FileName;
            //        XmlDocument xmlDoc = new XmlDocument();

            //        switch (CommonClass.ActiveSetup)
            //        {
            //            case "USA":
            //                xmlDoc.Load(Application.StartupPath + @"\Configs\ParamsTree_USA.xml");
            //                xmlDoc.Save(strPath);
            //                MessageBox.Show("Successfully saved!", "Tip", MessageBoxButtons.OK);
            //                break;
            //            case "China":
            //                xmlDoc.Load(Application.StartupPath + @"\Configs\ParamsTree_China.xml");
            //                xmlDoc.Save(strPath);
            //                MessageBox.Show("Successfully saved!", "Tip", MessageBoxButtons.OK);
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please new or open a BenMAP file first!", "Tip", MessageBoxButtons.OK);
            //}
        }

        /// <summary>
        /// 调出Stand Alone GIS
        /// </summary>
        private void btnGIS_Click(object sender, EventArgs e)
        {
            //MainRibbonForm frm = new MainRibbonForm(null);
            //frm.ShowDialog();
        }

        private void mnuModifySetup_Click(object sender, EventArgs e)
        {
            ManageSetup frm = new ManageSetup();
            DialogResult dialogResult= frm.ShowDialog();
            if (_currentForm != null)
            {
                BenMAP frmBenMAP = _currentForm as BenMAP;
                if (frmBenMAP != null)
                {
                    frmBenMAP.InitAggregationAndRegionList();
                }
                 
            }
             
            
            string commandText = "select SetupID,SetupName from Setups order by SetupID";
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
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
                toolStripMenuItem.Text = drSetup["SetupName"].ToString();
                toolStripMenuItem.Tag = benMAPSetupIn;
                toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);
                mnuActiveSetup.DropDownItems.Add(toolStripMenuItem);

            }
            //如果删掉了当前的setup
            DataRow[] dr = ds.Tables[0].Select("SETUPNAME ='" + mnuActiveSetup.Text + "'");
            if (dr.Count() <= 0 && ds.Tables[0].Rows.Count > 0)
            {
                mnuActiveSetup.Text = ds.Tables[0].Rows[0]["SetupName"].ToString();
            }

            //-------修正Pollutant
            CommonClass.lstPollutantAll = Grid.GridCommon.getAllPollutant(CommonClass.MainSetup.SetupID);
            DataSourceCommonClass._dicSeasonStaticsAll = null;
            //--------修正CurrentPollutant
            bool isDel = false;
            if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count>0)
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
                //btnNewFile_Click(sender, e);
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
            if (CommonClass.LstBaseControlGroup!=null && CommonClass.LstBaseControlGroup.Count>0)
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
            //modify crfunctions' pollutant
            if (CommonClass.BaseControlCRSelectFunction != null && CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction != null && CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count > 0)
            {
                foreach (CRSelectFunction cr in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    List<BenMAPPollutant> lstpollutant = CommonClass.lstPollutantAll.Where(p => p.PollutantID == cr.BenMAPHealthImpactFunction.Pollutant.PollutantID).ToList();
                    if (lstpollutant != null && lstpollutant.Count > 0)
                        cr.BenMAPHealthImpactFunction.Pollutant = lstpollutant.First();
                }
            }
        }
        #endregion

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
                //this.Close();

                Environment.Exit(0);//退出整个应用程序
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

        private void databaseImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatabaseImport frm = new DatabaseImport();
            DialogResult rtn = frm.ShowDialog();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                //this.Close();
                //DialogResult rtn = MessageBox.Show("Exit BenMAP CS?", "Tip", MessageBoxButtons.YesNo);
                //if (rtn == System.Windows.Forms.DialogResult.No) { e.Cancel = true; return; }
                if (CommonClass.InputParams!=null
                     &&CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    e.Cancel = true;
                    return;
                }
                ExitConfirm exit = new ExitConfirm();
                string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
                //string isShowExit = ConfigurationManager.AppSettings["IsShowExit"];
                string isShowExit = "T";
                if (System.IO.File.Exists(iniPath))
                {
                    isShowExit = CommonClass.IniReadValue("appSettings", "IsShowExit", iniPath);
                }
                DialogResult rtn = new DialogResult();
                if (isShowExit == "T")
                { rtn = exit.ShowDialog(); }
                if (rtn == System.Windows.Forms.DialogResult.Cancel) { e.Cancel = true; return; }
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

    }//class
}
