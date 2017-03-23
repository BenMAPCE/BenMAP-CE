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
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFG";
                openFileDialog.Filter = "CFG files(*.cfgx)|*.cfgx";
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
                if (txtExistingConfiguration.Text == "")
                {
                    MessageBox.Show("Select (*.cfgx) file first.");
                    return;
                }
                else if (txtExistingConfiguration.Text.Substring(txtExistingConfiguration.Text.Length - 5, 5) == "cfgrx")
                {
                    strCRPath = txtExistingConfiguration.Text;
                    string err = "";
                    BaseControlCRSelectFunction baseControlCRSelectFunction = Configuration.ConfigurationCommonClass.loadCFGFile(txtExistingConfiguration.Text, ref err);
                    if (baseControlCRSelectFunction == null)
                    {
                        MessageBox.Show(err);
                        this.DialogResult = System.Windows.Forms.DialogResult.None;
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

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    strCRPath = "";
                    string err = "";
                    BaseControlCRSelectFunction baseControlCRSelectFunction = Configuration.ConfigurationCommonClass.loadCFGFile(txtExistingConfiguration.Text, ref err);
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

        private void btBrowseCR_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFGR";
                openFileDialog.Filter = "CFGR files(*.cfgrx)|*.cfgrx";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtOpenExistingCFGR.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        public string strCRPath = "";
        private void btCROK_Click(object sender, EventArgs e)
        {
            strCRPath = txtOpenExistingCFGR.Text;
            GC.Collect();
            if (txtOpenExistingCFGR.Text == "") return;
            string err = "";
            BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile(txtOpenExistingCFGR.Text, ref err);
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
    }
}
