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
    public partial class PollutantManage : FormBase
    {
        public PollutantManage()
        {
            InitializeComponent();
        }
                private void btnSAdd_Click(object sender, EventArgs e)
        {

            if (txtSeasonalMetric.Text != "")
                lstMetric.Items.Add(txtSeasonalMetric.Text);
            lstMetric.Refresh();
        }
                private void btnSDelete_Click(object sender, EventArgs e)
        {
            if (lstMetric.Items.Count != 0)
            {
                lstMetric.Items.Remove(lstMetric.SelectedItem);
                lstMetric.Focus();
                lstMetric.Refresh();
            }
            else
            {
                return;
            }

        }
                Dictionary<string, Time> vartime = new Dictionary<string, Time>();
        private void btnSSAdd_Click(object sender, EventArgs e)
        {

            if (lstSMetric.Items.Count == 0)            {
                dmudownStartMonth.Enabled = true;
                nudownStartDate.Enabled = true;
                dmudownEndMonth.Enabled = true;
                nudownEndDate.Enabled = true;
                string index = "season" + i.ToString();
                lstSMetric.Items.Add(index);
                saveSeasonDic(i);                i++;
                dmudownStartMonth.SelectedIndex = 0;
                dmudownEndMonth.SelectedIndex = 11;
                return;
            }
            else
            {
                                string time = string.Format("{0}-{1}", dmudownStartMonth.SelectedIndex + 1, nudownStartDate.Value);
                DateTime startTime = DateTime.Parse(time);
                time = string.Format("{0}-{1}", dmudownEndMonth.SelectedIndex + 1, nudownEndDate.Value);
                DateTime endTime = DateTime.Parse(time);
                if (endTime.Month == 12 && endTime.Day == 31)                {
                    MessageBox.Show("The end date should be less than December 31.");
                    return;
                }
                else
                {
                                        if (endTime < startTime)                    {
                        MessageBox.Show("The end date should be later than the start date. Please revise.");
                        return;
                    }
                                        string index = "season" + i.ToString();
                    lstSMetric.Items.Add(index);
                                        saveSeasonDic(i - 1);
                                        startTime = endTime.AddDays(1);
                    dmudownStartMonth.SelectedIndex = startTime.Month - 1;
                    nudownStartDate.Value = startTime.Day;
                                        dmudownEndMonth.SelectedIndex = 11;
                    nudownEndDate.Value = 31;
                                        saveSeasonDic(i);
                    i++;
                }
            }

                                                                                                                                                                                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    

                                                                                }

        private void saveSeasonDic(int index)
        {
            Time timemonthday = new Time();
            timemonthday.Month1 = dmudownStartMonth.SelectedIndex;
            timemonthday.Month2 = dmudownEndMonth.SelectedIndex;
            timemonthday.Day1 = nudownStartDate.Value;
            timemonthday.Day2 = nudownEndDate.Value;
            string tmpKey = "season" + index.ToString();
            if (vartime.ContainsKey(tmpKey))
            {
                vartime[tmpKey] = timemonthday;
            }
            else
            {
                vartime.Add(tmpKey, timemonthday);
            }
        }
        private int i = 1;
                private void btnSSDelete_Click(object sender, EventArgs e)
        {
            if (lstSMetric.Items.Count != 0)
            {
                lstSMetric.Items.RemoveAt(lstSMetric.Items.IndexOf(lstSMetric.Items[lstSMetric.Items.Count - 1]));
                lstSMetric.Focus();
                i--;
            }
            else
            {
                return;
            }
        }
        private void lstSMetric_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (lstSMetric.SelectedItem != null)
            {
                string str = lstSMetric.SelectedItem.ToString();
                dmudownStartMonth.SelectedIndex = vartime[str].Month1;
                dmudownEndMonth.SelectedIndex = vartime[str].Month2;
                nudownStartDate.Value = vartime[str].Day1;
                nudownEndDate.Value = vartime[str].Day2;
            }
        }
                private void lstSAvailableFunctions_DoubleClick(object sender, EventArgs e)
        {
            string msg = "";
            try
            {
                if (lstSAvailableFunctions == null || lstSAvailableFunctions.Items.Count == 0)
                {
                    msg = "No function can be selected";
                    return;
                }
                Dictionary<string, string> varDic = new Dictionary<string, string>();
                varDic.Add("ABS(x)", "ABS()");
                varDic.Add("EXP(x)", "EXP()");
                varDic.Add("IPOWER(x,y)", "IPOWER(,)");
                varDic.Add("LN(x)", "LN()");
                varDic.Add("POWER(x,y)", "POWER(,)");
                varDic.Add("SQR(x)", "SQR()");
                varDic.Add("SQRT(x)", "SQRT()");
                string insertedFun = string.Empty;
                string tag = lstSAvailableFunctions.SelectedItem.ToString();
                insertedFun = varDic[tag];
                string fun = string.Empty;
                string oldFun = txtFunctionManage.Text;
                int oldInsertIndex = txtFunctionManage.SelectionStart;
                int index = lstSAvailableFunctions.SelectedItem.ToString().IndexOf("(") + 1;
                if (txtFunctionManage.Text == string.Empty)
                {
                    txtFunctionManage.Text = insertedFun;
                    txtFunctionManage.SelectionStart = index;
                    txtFunctionManage.Focus();
                }
                else
                {
                    Console.WriteLine(oldInsertIndex + "\n" + index);
                    string strFun = txtFunctionManage.Text.Insert(oldInsertIndex, insertedFun);
                    txtFunctionManage.Text = strFun;
                    index += oldInsertIndex;
                    txtFunctionManage.SelectionStart = index;
                    txtFunctionManage.Focus();
                }

            }
            catch (Exception ex)
            {
                CommonClass.Equals(ex, ex);
            }
            finally
            {
                if (msg != "")
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
                private void lstSAvailableVaribles_DoubleClick(object sender, EventArgs e)
        {
            string msg = "";
            try
            {
                if (lstSAvailableVaribles == null || lstSAvailableVaribles.Items.Count == 0)
                {
                    msg = "No varible can be selected.";
                    return;
                }
                string lstVar = lstSAvailableVaribles.SelectedItem.ToString();
                int varindex = lstVar.IndexOf("[");
                int oldInsertIndex = txtFunctionManage.SelectionStart;
                if (txtFunctionManage.Text == string.Empty)
                {
                    if (lstVar == "MetricValues[]" || lstVar == "SeasonalMetricValues[]" || lstVar == "SortedMetricValues[]")
                    {
                        txtFunctionManage.Text = lstVar;
                        txtFunctionManage.SelectionStart = varindex + 1;
                        txtFunctionManage.Focus();
                    }
                    else
                    {
                        txtFunctionManage.Text = lstVar;
                        txtFunctionManage.SelectionStart = txtFunctionManage.Text.Length;
                        txtFunctionManage.Focus();
                    }
                }
                else
                {

                    string lstVarFun = txtFunctionManage.Text.Insert(oldInsertIndex, lstVar);

                    if (lstVar == "MetricValues[]" || lstVar == "SeasonalMetricValues[]" || lstVar == "SortedMetricValues[]")
                    {
                        txtFunctionManage.Text = lstVarFun;
                        varindex += oldInsertIndex + 1;
                        txtFunctionManage.SelectionStart = varindex;
                        txtFunctionManage.Focus();
                    }
                    else
                    {
                        txtFunctionManage.Text = lstVarFun;
                        oldInsertIndex += lstSAvailableVaribles.Text.Length;
                        txtFunctionManage.SelectionStart = oldInsertIndex;
                        txtFunctionManage.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonClass.Equals(ex, ex);
            }
            finally
            {
                if (msg != "")
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
                private void lstSAvailableFunctions_Click(object sender, EventArgs e)
        {
            int k = lstSAvailableFunctions.SelectedIndex;
            switch (k)
            {
                case 0:
                    label13.Text = "Returns the absolute value of x";
                    break;
                case 1:
                    label13.Text = "Returns e to the power x,\r\nwhere e is the base of \r\nthe natural logarithms";
                    break;
                case 2:
                    label13.Text = "Returns x to the power y \r\n[y an integer value]";
                    break;
                case 3:
                    label13.Text = "Returns the natural \r\nlogarithm of x";
                    break;
                case 4:
                    label13.Text = "Returns x to the power y \r\n[y a floating point value]";
                    break;
                case 5:
                    label13.Text = "Returns the square of x";
                    break;
                case 6:
                    label13.Text = "Returns the positive square\r\nroot of x";
                    break;
                default:
                    break;
            }
        }
                private void lstSAvailableVaribles_Click(object sender, EventArgs e)
        {
            int l = lstSAvailableVaribles.SelectedIndex;
            switch (l)
            {
                case 0:
                    label14.Text = "All metric values for the year\r\n[index begins at zero]";
                    break;
                case 1:
                    label14.Text = "All metric values for the season\r\n[index begins at zero]";
                    break;
                case 2:
                    label14.Text = "All metric values for the season,\r\nsorted from low to hige\r\n[index begins at zero]";
                    break;
                case 3:
                    label14.Text = "Index of the first day of the season";
                    break;
                case 4:
                    label14.Text = "Index of the last day of the season";
                    break;
                case 5:
                    label14.Text = "Mean of the seasonal metric values";
                    break;
                case 6:
                    label14.Text = "Median of the seasonal metric values";
                    break;
                case 7:
                    label14.Text = "Minimum of the seasonal metric values";
                    break;
                case 8:
                    label14.Text = "Maximum of the seasonal metric values";
                    break;
                case 9:
                    label14.Text = "Sum of the seasonal values";
                    break;
                case 10:
                    label14.Text = "Flag value indicating a missing\r\nmetric value [-345]";
                    break;
                default:
                    break;
            }
        }
                private void btnPMok_Click(object sender, EventArgs e)
        {
            string output = txtFunctionManage.Text;
            System.IO.File.WriteAllText("d:\\functionmanage.txt", output, System.Text.Encoding.Default);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
                private void btnPMcancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
                private void dmudownStartM_TextChanged(object sender, EventArgs e)
        {
            if (dmudownStartMonth.Text == "一月" || dmudownStartMonth.Text == "三月" || dmudownStartMonth.Text == "五月" || dmudownStartMonth.Text == "七月" || dmudownStartMonth.Text == "八月" || dmudownStartMonth.Text == "十月" || dmudownStartMonth.Text == "十二月")
            {
                nudownStartDate.Minimum = 1;
                nudownStartDate.Maximum = 31;
            }
            else
            {
                if (dmudownStartMonth.Text == "四月" || dmudownStartMonth.Text == "六月" || dmudownStartMonth.Text == "九月" || dmudownStartMonth.Text == "十一月")
                {
                    nudownStartDate.Minimum = 1;
                    nudownStartDate.Maximum = 30;
                }
                else
                {
                    nudownStartDate.Minimum = 1;
                    nudownStartDate.Maximum = 28;
                }
            }
        }
                private void dmudownEndMonth_TextChanged(object sender, EventArgs e)
        {
            if (dmudownEndMonth.Text == "一月" || dmudownEndMonth.Text == "三月" || dmudownEndMonth.Text == "五月" || dmudownEndMonth.Text == "七月" || dmudownEndMonth.Text == "八月" || dmudownEndMonth.Text == "十月" || dmudownEndMonth.Text == "十二月")
            {
                nudownEndDate.Minimum = 1;
                nudownEndDate.Maximum = 31;
            }
            else
            {
                if (dmudownEndMonth.Text == "四月" || dmudownEndMonth.Text == "六月" || dmudownEndMonth.Text == "九月" || dmudownEndMonth.Text == "十一月")
                {
                    nudownEndDate.Minimum = 1;
                    nudownEndDate.Maximum = 30;
                }
                else
                {
                    nudownEndDate.Minimum = 1;
                    nudownEndDate.Maximum = 28;
                }
            }
        }
    }
}
public class Time
{
    private int _month1;

    public int Month1
    {
        get { return _month1; }
        set { _month1 = value; }
    }
    private int _month2;

    public int Month2
    {
        get { return _month2; }
        set { _month2 = value; }
    }

    private decimal _day1;

    public decimal Day1
    {
        get { return _day1; }
        set { _day1 = value; }
    }
    private decimal _day2;

    public decimal Day2
    {
        get { return _day2; }
        set { _day2 = value; }
    }
}