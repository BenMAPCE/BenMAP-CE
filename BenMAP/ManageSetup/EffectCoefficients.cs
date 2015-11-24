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
    public partial class EffectCoefficients : FormBase
    {
        private String betaVariation;
        public EffectCoefficients(String varSelected)
        {
            InitializeComponent();
            betaVariation = varSelected;
        }

        // Some of these fields will be filled dynamically
        // Hard coded values put in for tesing UI features 
        private void EffectCoefficients_Load(object sender, EventArgs e)
        {
            try
            {
                if (betaVariation == "Full Year")
                {
                    tabPage1.Text = "Full Year";
                    tbSeasMetric.Text = "None";
                    showForSeasonal.Visible = false;
                    panel2.Visible = true;
                    panel1.Visible = true;
                    
                }

                else if (betaVariation == "Seasonal")
                {
                    tabPage1.Text = "Season 1";
                    tbSeasMetric.Text = "ColdWarm";
                    showForSeasonal.Visible = true;
                    panel2.Visible = false;
                    panel1.Visible = true;
                }

                else // Geographic
                {
                    panel1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void tabControl1_SelectIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedTab.Controls.Add(panel1);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

    }

}
