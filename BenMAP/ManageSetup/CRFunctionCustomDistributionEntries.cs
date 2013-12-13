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
    public partial class CRFunctionCustomDistributionEntries : Form
    {
        public CRFunctionCustomDistributionEntries()
        {
            InitializeComponent();
        }

        private void CRFunctionCustomDistributionEntries_Load(object sender, EventArgs e)
        {

        }

        private void btnLoadFromTextFile_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable _dt = new DataTable();
                List<double> lstDistribution = new List<double>();
                DataWorker.DataParser dp = new DataWorker.DataParser();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
                openFileDialog.Filter = "All Files|*.*|CSV files|*.csv";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                string _filePath = openFileDialog.FileName;
                string strfilepath = System.IO.Path.GetExtension(_filePath);
                switch (strfilepath.ToLower())
                {
                    case ".xls":
                        ds = dp.ReadExcel2DataSet(_filePath);
                        break;
                    case ".xlsx":
                        ds = dp.ReadExcel2DataSet(_filePath);
                        break;
                    case ".csv":
                        ds = dp.ReadCSV2DataSet(_filePath, "table");
                        break;
                    default: break;
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstCurrentEntries.Items.Add(ds.Tables[0].Rows[i][0]);
                    lstDistribution.Add(Convert.ToDouble(ds.Tables[0].Rows[i][0]));
                }
                txtCurrentMean.Text = GetMeanDistribution(lstDistribution).ToString();
                txtCurrentStandard.Text = (getStandardDeviation(lstDistribution, Convert.ToDouble(txtCurrentMean.Text))).ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private double GetMeanDistribution(List<double> lst)
        {
            return lst.Sum() / lst.Count ;
        }

        public static double getStandardDeviation(List<double> values, double PointEstimate)
        {
            double sumd = 0;
            foreach (double di in values)
            {
                sumd = sumd + Math.Pow((di - PointEstimate), 2);
            }
            sumd = sumd / (values.Count - 1);
            sumd = Math.Sqrt(sumd);
            return sumd;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
