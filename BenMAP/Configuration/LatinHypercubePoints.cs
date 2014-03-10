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
    public partial class LatinHypercubePoints : FormBase
    {
        private int latinHypercubePointsCount = 10;

        public int LatinHypercubePointsCount
        {
            get { return latinHypercubePointsCount; }
            set { latinHypercubePointsCount = value; }
        }
        private bool isRunInPointMode;

        public bool IsRunInPointMode
        {
            get { return isRunInPointMode; }
            set { isRunInPointMode = value; }
        }
        private double threshold;

        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }
        public LatinHypercubePoints()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void LatinHypercubePoints_Load(object sender, EventArgs e)
        {
            try
            {
                cboLatinHypercubePoints.Text = latinHypercubePointsCount.ToString();
                txtThreshold.Text = threshold.ToString();
                if (CommonClass.CRSeeds != null && CommonClass.CRSeeds != -1)
                    txtRandomSeed.Text = CommonClass.CRSeeds.ToString();
                else
                    txtRandomSeed.Text = "Random Integer";
                if (isRunInPointMode)
                {
                    this.chbRunInPointMode.Checked = true;
                    txtRandomSeed.Enabled = false;
                }
                else
                {
                    this.chbRunInPointMode.Checked = false;
                    txtRandomSeed.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            latinHypercubePointsCount = Convert.ToInt16(cboLatinHypercubePoints.Text);
            if (CommonClass.BaseControlCRSelectFunction != null && CommonClass.CRLatinHypercubePoints != latinHypercubePointsCount)
            {
                for (int i = 0; i < CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count; i++)
                {
                    CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction[i].lstLatinPoints = null;
                }
            }
            try
            {
                int Seeds = -1; if (txtRandomSeed.Text != "Random Integer" && Int32.TryParse(txtRandomSeed.Text, out Seeds) == false)
                {
                    MessageBox.Show("The random seed must be a number.");
                    return;
                }
                CommonClass.CRSeeds = Seeds;

            }
            catch
            {
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void chbRunInPointMode_CheckedChanged(object sender, EventArgs e)
        {
            IsRunInPointMode = chbRunInPointMode.Checked;
            if (isRunInPointMode)
            {
                cboLatinHypercubePoints.Enabled = false;
                txtRandomSeed.Enabled = false;
            }
            else
            {
                cboLatinHypercubePoints.Enabled = true;
                txtRandomSeed.Enabled = true;
            }
        }

        private void txtThreshold_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtThreshold.Text == string.Empty)
                { txtThreshold.Text = Convert.ToString(0); }
                threshold = Convert.ToDouble(txtThreshold.Text);
            }
            catch
            {
                txtThreshold.Text = threshold.ToString();
            }
        }

        private void txtRandomSeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
