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
    public partial class OpenExistingAPVConfiguration : FormBase
    {
        public OpenExistingAPVConfiguration()
        {
            InitializeComponent();
        }
        public string strAPVPath = "";
        public string strCRPath = "";


        private void OpenExistingAPVConfiguration_Load(object sender, EventArgs e)
        {
        }



        private void btAPVCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }



        private void btCRCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btBrowerAPV_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APV";
                openFileDialog.Filter = "APV Files(*.apvx)|*.apvx";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtAPV.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOpenAPV_Click(object sender, EventArgs e)
        {
            if (txtAPV.Text == "")
            {
                MessageBox.Show("Select (*.apvx) file first.");
                return;
            }
            strAPVPath = "";
            strCRPath = "";
            strAPVPath = txtAPV.Text;
            string err = "";
            ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = APVX.APVCommonClass.loadAPVRFile(txtAPV.Text, ref err);
            if (valuationMethodPoolingAndAggregation == null)
            {
                MessageBox.Show(err);
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }
            BenMAPSetup benMAPSetup = null;
            benMAPSetup = CommonClass.getBenMAPSetupFromName(valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName);
            if (CommonClass.MainSetup.SetupName != benMAPSetup.SetupName)
            {
                DialogResult dialogResult = MessageBox.Show("Setup Name in selected configuratin file is different from current set up. Do you want to continue?", "warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                }
                else if (dialogResult == DialogResult.No)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.None;
                    return;
                }

            }

            CommonClass.BaseControlCRSelectFunctionCalculateValue = null;

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
            GC.Collect();
            CommonClass.EmptyTmpFolder();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }

        private void btnCancelAPV_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btBrowerAPVR_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";
                openFileDialog.Filter = "APVR Files(*.apvrx)|*.apvrx";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtAPVR.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOpenAPVR_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAPVR.Text == "")
                {
                    MessageBox.Show("Select (*.apvrx) file first.");
                    return;
                }
                string err = "";
                ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = APVX.APVCommonClass.loadAPVRFile(txtAPVR.Text, ref err);
                if (valuationMethodPoolingAndAggregation == null)
                {
                    MessageBox.Show(err);
                    this.DialogResult = System.Windows.Forms.DialogResult.None;
                    return;
                }
                BenMAPSetup benMAPSetup = null;
                benMAPSetup = CommonClass.getBenMAPSetupFromName(valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName);
                if (CommonClass.MainSetup.SetupName != benMAPSetup.SetupName)
                {
                    DialogResult dialogResult = MessageBox.Show("Setup Name in selected configuratin file is different from current set up. Do you want to continue?", "warning", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.None;
                        return;
                    }
                }
                strAPVPath = "";
                strCRPath = "";
                strAPVPath = txtAPVR.Text;
                CommonClass.BaseControlCRSelectFunctionCalculateValue = null;

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
                GC.Collect();
                CommonClass.EmptyTmpFolder();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
