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
    public partial class QALYResultsReport : FormBase
    {
        public QALYResultsReport()
        {
            InitializeComponent();
        }

        private void QALYResultsReport_Load(object sender, EventArgs e)
        {
            try
            {
                if (lstColumnRow == null)
                {
                    lstColumnRow = new List<FieldCheck>();
                    lstColumnRow.Add(new FieldCheck() { FieldName = "Column", isChecked = true });
                    lstColumnRow.Add(new FieldCheck() { FieldName = "Row", isChecked = true });

                }
                if (lstHealth == null)
                {
                    lstHealth = new List<FieldCheck>();
                    lstHealth.Add(new FieldCheck() { FieldName = "DataSet", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
                    lstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
                    lstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = false });

                }
                if (lstResult == null)
                {
                    lstResult = new List<FieldCheck>();
                    lstResult.Add(new FieldCheck() { FieldName = "PointEstimate", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Mean", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Standard Deviation", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Variance", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Percentiles", isChecked = true });

                }
                olvColumnRow.SetObjects(lstColumnRow);
                olvHealth.SetObjects(lstHealth);
                olvResult.SetObjects(lstResult);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        public List<FieldCheck> lstColumnRow;
        public List<FieldCheck> lstHealth;
        public List<FieldCheck> lstResult;
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }

    }


}
