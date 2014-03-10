using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class CustomDistributionEntries : FormBase
    {
        List<double> listDistribution = new List<double>();

        public CustomDistributionEntries()
        {
            InitializeComponent();
            listDistribution = null;
        }

        public CustomDistributionEntries(List<double> listValue)
        {
            InitializeComponent();
            listDistribution = listValue;
        }

        List<double> lstlst = new List<double>();

        private void CustomDistributionEntries_Load(object sender, EventArgs e)
        {
            if (listDistribution != null)
            {
                foreach (double item in listDistribution)
                {
                    lstCurrentEntries.Items.Add(item);
                    lstlst.Add(Convert.ToDouble(item));
                }
                txtCurrentMean.Text = GetMeanDistribution(lstlst).ToString();
                txtCurrentStandard.Text = (getStandardDeviation(lstlst, Convert.ToDouble(txtCurrentMean.Text))).ToString();
            }
            else
            { return; }
        }

        private List<double> _list;

        public List<double> list
        {
            get { return _list; }
            set { _list = value; }
        }

        private void btnLoadFromTextFile_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable _dt = new DataTable();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
                openFileDialog.Filter = "All Files|*.*|CSV files|*.csv";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                string _filePath = openFileDialog.FileName;
                string strfilepath = System.IO.Path.GetExtension(_filePath);
                dt = CommonClass.ExcelToDataTable(_filePath);
                if (dt == null) return;
                lstCurrentEntries.Items.Clear();
                lstlst.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        lstlst.Add(Convert.ToDouble(dt.Rows[i][0].ToString()));
                    }
                    catch
                    {
                        MessageBox.Show("'" + dt.Rows[i][0].ToString() + "' is not a vaild floating point value.", "Error", MessageBoxButtons.OK);
                        lstlst.Clear();
                        txtCurrentMean.Text = "";
                        txtCurrentStandard.Text = "";
                        return;
                    }
                    lstCurrentEntries.Items.Add(dt.Rows[i][0]);
                }
                txtCurrentMean.Text = GetMeanDistribution(lstlst).ToString();
                txtCurrentStandard.Text = (getStandardDeviation(lstlst, Convert.ToDouble(txtCurrentMean.Text))).ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private double GetMeanDistribution(List<double> lst)
        {
            return lst.Sum() / lst.Count;
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
            _list = lstlst;
        }
    }
}