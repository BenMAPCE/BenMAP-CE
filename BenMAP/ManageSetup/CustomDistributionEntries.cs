using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class CustomDistributionEntries : FormBase
    {
        private bool valuesPassed;
        List<double> listDistribution = new List<double>();

        public CustomDistributionEntries()
        {
            InitializeComponent();
            listDistribution = null;
            valuesPassed = false;
        }

        public CustomDistributionEntries(List<double> listValue)
        {
            InitializeComponent();
            listDistribution = listValue;
            valuesPassed = false;
        }

        public CustomDistributionEntries(List<double> listValue, string avg, string standDev)
        {
            InitializeComponent();
            listDistribution = listValue;
            MeanValue = avg;
            Parameter1 = standDev;
            valuesPassed = true;
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

                // Used so that SD and mean aren't calculated each time if already known
                if(valuesPassed)
                {
                    txtCurrentMean.Text = MeanValue;
                    txtCurrentStandard.Text = Parameter1;
                }
                else
                {
                    MeanValue = GetMeanDistribution(lstlst).ToString();
                    txtCurrentMean.Text = MeanValue;
                    Parameter1 = (getStandardDeviation(lstlst, Convert.ToDouble(txtCurrentMean.Text))).ToString();
                    txtCurrentStandard.Text = Parameter1;
                }
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

                MeanValue = GetMeanDistribution(lstlst).ToString();
                txtCurrentMean.Text = MeanValue;
                Parameter1 = (getStandardDeviation(lstlst, Convert.ToDouble(txtCurrentMean.Text))).ToString();
                txtCurrentStandard.Text = Parameter1;
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

        private string _meanValue;
        public string MeanValue
        {
            get { return _meanValue; }
            set { _meanValue = value; }
        }

        private string _parameter1;
        public string Parameter1
        {
            get { return _parameter1; }
            set { _parameter1 = value; }
        }
    }
}