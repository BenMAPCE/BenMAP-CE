using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class OpenExistingConfiguration : FormBase
    {
        public OpenExistingConfiguration()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                string filter = "";
                string resultPath = "";
                if (rbtOpenCfg.Checked)
                {
                    filter = "CFG files(*.cfgx)|*.cfgx";
                    resultPath = CommonClass.ResultFilePath + @"\Result\CFG";
                }
                else if (rbtOpenCfgr.Checked)
                {
                    filter = "CFGR files(*.cfgrx)|*.cfgrx";
                    resultPath = CommonClass.ResultFilePath + @"\Result\CFGR";
                }

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = resultPath;
                openFileDialog.Filter = filter;
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtExistingConfiguration.Text = openFileDialog.FileName;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void OpenExistingConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                strCRPath = txtExistingConfiguration.Text;
                string err = "";

                if (strCRPath == "")
                {
                    MessageBox.Show("Select (*.cfgx) or (*.cfgrx) file first.");
                    return;
                }
                //YY: handle both cfg and cfgr
                else if (strCRPath.Substring(txtExistingConfiguration.Text.Length - 5, 5) == "cfgrx" && rbtOpenCfgr.Checked)
                {
                    strCRPath = txtExistingConfiguration.Text;
                    err = "";
                    BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile(txtExistingConfiguration.Text, ref err);
                    if (baseControlCRSelectFunctionCalculateValue == null)
                    {
                        MessageBox.Show(err);
                        this.DialogResult = System.Windows.Forms.DialogResult.None;
                        return;
                    }
                    BenMAPSetup benMAPSetup = null;
                    benMAPSetup = CommonClass.getBenMAPSetupFromName(baseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName);
                    if (CommonClass.MainSetup.SetupName != benMAPSetup.SetupName)
                    {
                        DialogResult dialogResult = MessageBox.Show("Setup Name in selected configuration file is different from current setup. Do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            this.DialogResult = System.Windows.Forms.DialogResult.None;
                            return;
                        }
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else if (strCRPath.Substring(txtExistingConfiguration.Text.Length - 4, 4) == "cfgr" && rbtOpenCfg.Checked)
                {
                    strCRPath = "";
                    err = "";
                    BaseControlCRSelectFunction baseControlCRSelectFunction = Configuration.ConfigurationCommonClass.loadCFGFile(strCRPath, ref err);
                    if (baseControlCRSelectFunction == null)
                    {
                        MessageBox.Show(err);
                        return;
                    }
                    BenMAPSetup benMAPSetup = null;
                    benMAPSetup = CommonClass.getBenMAPSetupFromName(baseControlCRSelectFunction.BaseControlGroup[0].GridType.SetupName);
                    if (CommonClass.MainSetup.SetupName != benMAPSetup.SetupName)
                    {
                        DialogResult dialogResult = MessageBox.Show("Setup Name in selected configuration file is different from current setup. Do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            this.DialogResult = System.Windows.Forms.DialogResult.None;
                            return;
                        }
                    }
                    CommonClass.ClearAllObject();
                    CommonClass.BaseControlCRSelectFunction = baseControlCRSelectFunction;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;

                    if (baseControlCRSelectFunction == null)
                    {
                        MessageBox.Show(err);
                        return;
                    }


                }
                if (CommonClass.LstUpdateCRFunction != null)
                {
                    CommonClass.LstUpdateCRFunction.Clear();
                }
                CommonClass.LstUpdateCRFunction = new List<CRSelectFunction>();
                if (CommonClass.LstDelCRFunction != null)
                {
                    CommonClass.LstDelCRFunction.Clear();
                }
                CommonClass.LstDelCRFunction = new List<CRSelectFunction>();
                CommonClass.EmptyTmpFolder();
            }
            catch
            {
                MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");

            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btCRCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public string strCRPath = "";

        private void rbtOpenCfg_CheckedChanged(object sender, EventArgs e)
        {
            txtExistingConfiguration.Text = "";
        }

        private void rbtOpenCfgr_CheckedChanged(object sender, EventArgs e)
        {
            txtExistingConfiguration.Text = "";
        }
    }
}
