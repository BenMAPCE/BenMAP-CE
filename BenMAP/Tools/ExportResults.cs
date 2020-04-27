using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ExportResults : Form
    {
        List<CRSelectFunctionCalculateValue> lstCRCV = new List<CRSelectFunctionCalculateValue>();

        public ExportResults()
        {
            InitializeComponent();
        }

        private void ExportResults_Load(object sender, EventArgs e)
        {
            rbSingle.Checked = true;

            object lstHIF = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue;

            lstCRCV = (CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue == null) ? null : CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue;

            foreach (CRSelectFunctionCalculateValue crcv in lstCRCV)
            {
                lbCRSelectFunctions.Items.Add(crcv.CRSelectFunction.BenMAPHealthImpactFunction.Author);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string reportFilePath = CommonClass.ResultFilePath + "\\Reports\\";
            string templateFilePath = reportFilePath + "\\Templates\\";
            string templateFile = "";

            if (String.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Please Enter A Valid File Name");
                return;
            }

            if (lbCRSelectFunctions.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please Select A Health Impact Function To Export");
                return;
            }

            string userFileName = txtFileName.Text + ".xlsx";

            if (rbSingle.Checked)
            {
                templateFile = "CPP LML.xlsx";
            }
            else if (rbMultiple.Checked)
            {
                templateFile = "CPP Final Benefits.xlsx";
            }
            else
            {
                templateFile = null;
                MessageBox.Show("Please Select A Report Type");
                return;
            }

            if (File.Exists(Path.Combine(templateFilePath, templateFile)))
            {
                if (File.Exists(Path.Combine(reportFilePath, userFileName)))
                {
                    DialogResult dialogResult = MessageBox.Show("A file with that name already exists. Do you want to overwrite?", "Warning", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                        File.Copy(Path.Combine(templateFilePath, templateFile), Path.Combine(reportFilePath, userFileName), true);
                    else
                        return;
                }
                else
                    File.Copy(Path.Combine(templateFilePath, templateFile), Path.Combine(reportFilePath, userFileName));
            }
            else
            {
                MessageBox.Show("Unable To Locate The Template For The Selected Report");
                return;
            }

            if (rbSingle.Checked)
            {
                GenerateSingleStudyReport(Path.Combine(reportFilePath, userFileName));
            }
            else
                return;
                //GenerateMultipleStudyReport();

        }

        private void GenerateSingleStudyReport(string filePath)
        {
                string newFile = filePath;
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSingle.Checked)
            {
                lbCRSelectFunctions.SelectionMode = SelectionMode.One;
            }
            else if (rbMultiple.Checked)
            {
                lbCRSelectFunctions.SelectionMode = SelectionMode.MultiExtended;
            }
            else
            {
                MessageBox.Show("Error with Report Type Selection");
            }

        }
    }
}
