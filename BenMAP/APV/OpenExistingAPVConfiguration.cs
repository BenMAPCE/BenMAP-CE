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

        //private void btBrowseCR_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        OpenFileDialog openFileDialog = new OpenFileDialog();
        //        openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
        //        openFileDialog.Filter = "cfgrx   files   (*.cfgrx)|*.cfgrx";
        //        openFileDialog.FilterIndex = 3;
        //        openFileDialog.RestoreDirectory = true;
        //        if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
        //        txtOpenExistingCFGR.Text = openFileDialog.FileName;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //    }
        //}

        //private void btCROK_Click(object sender, EventArgs e)
        //{
        //      strAPVPath="";
        //       strCRPath="";
        //       strCRPath = txtOpenExistingCFGR.Text;
        //    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
        //    GC.Collect();
        //    if (txtOpenExistingCFGR.Text == "") return;
        //    //CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCRFRFile(txtOpenExistingCFGR.Text);
        //    this.DialogResult = System.Windows.Forms.DialogResult.OK;
        //}

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
            CommonClass.BaseControlCRSelectFunctionCalculateValue = null;

            if (CommonClass.LstUpdateCRFunction != null)
            { //清空添加的公式
                CommonClass.LstUpdateCRFunction.Clear();
            }
            CommonClass.LstUpdateCRFunction = new List<CRSelectFunction>();
            if (CommonClass.LstDelCRFunction != null)
            {//清空 删除的公式
                CommonClass.LstDelCRFunction.Clear();
            }
            CommonClass.LstDelCRFunction = new List<CRSelectFunction>();
            GC.Collect();
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
                strAPVPath = "";
                strCRPath = "";
                strAPVPath = txtAPVR.Text;
                CommonClass.BaseControlCRSelectFunctionCalculateValue = null;

                if (CommonClass.LstUpdateCRFunction != null)
                { //添加公式
                    CommonClass.LstUpdateCRFunction.Clear();
                }
                CommonClass.LstUpdateCRFunction = new List<CRSelectFunction>();
                if (CommonClass.LstDelCRFunction != null)
                {// 删除公式
                    CommonClass.LstDelCRFunction.Clear();
                }
                CommonClass.LstDelCRFunction = new List<CRSelectFunction>();
                GC.Collect();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
