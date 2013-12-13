using System;
using System.Drawing;
using System.Windows.Forms;
using BenMAP;

namespace WinControls
{
    // 定义一个委托
    public delegate void ValueRangeChangedHanler(SetValueRange frm);

    public partial class SetValueRange : FormBase
    {
        // 定义一个事件
        public event ValueRangeChangedHanler ValueRangeChanged;

        // 定义事件的处理方法
        protected void OnValueRangeChanged(SetValueRange frm)
        {
            if (ValueRangeChanged != null)
            {
                ValueRangeChanged(frm);
            }
        }

        private double _minValue;

        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                txtMin.Text = _minValue.ToString("0.###");
            }
        }

        private double _maxValue;

        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                txtMax.Text = _maxValue.ToString("0.###");
            }
        }

        //private string _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            //get { return _unit; }
            set
            {
                //_unit = value;
                lblUnit1.Text = value;
                lblUnit2.Text = value;
            }
        }

        // 定义边界点的个数
        private int count = 6;

        // Legend的色块
        Color[] _colorArray = new Color[6];

        // 用户可以根据自己需要定义色块
        public Color[] ColorArray
        {
            get { return _colorArray; }
            set
            {
                _colorArray = value;
            }
        }

        // Legend中每个色块的开始值
        private double[] _valueArray = new double[6];

        public double[] ValueArray
        {
            get { return _valueArray; }
            set { _valueArray = value; }
        }

        public SetValueRange()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double min = 0;
            double max = 0;
            bool ok = false;
            string tmp = "";
            string msg = "";
            try
            {
                ok = GetMinMaxValues(ref min, ref max);
                if (!ok) { return; }
                _minValue = min;
                _maxValue = max;

                for (int i = 0; i < count; i++)
                {
                    _colorArray[i] = dgvSetColor.Rows[0].Cells[i].Style.BackColor;
                    // _valueArray[i] = double.Parse(dgvSetColor.Rows[1].Cells[i].Value.ToString());
                    tmp = dgvSetColor.Rows[1].Cells[i].Value.ToString();
                    ok = double.TryParse(tmp, out _valueArray[i]);
                    if (!ok)
                    {
                        msg = string.Format("'{0}' is an invalid boundary value.", tmp);
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                // 验证边界点数组的值是否是从小到大的顺序，如果不是提示设定错误
                ok = VerifyBoundaryValue();
                if (!ok) { return; }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        // 验证边界点的有效性
        private bool VerifyBoundaryValue()
        {
            double tmpPre = 0;
            double tmpCurrent = 0;
            bool ok = false;
            string msg = "";
            try
            {
                for (int i = 1; i < count; i++)
                {
                    tmpPre = _valueArray[i - 1];
                    tmpCurrent = _valueArray[i];
                    if (tmpPre > tmpCurrent)
                    {
                        msg = "Boundary Values are invalid.";
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return ok;
                    }
                }
                ok = true;
                return ok;
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
                return ok;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
        }

        private void SetValueRange_Load(object sender, EventArgs e)
        {
            try
            {
                //InitStartValue();
                // DefaultValueArray(_minValue, _maxValue);
                InitdgColorSet();
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        // 初始化Legend的值
        private void SetDefaultValueArray(double minValue, double maxValue)
        {
            try
            {
                //将色块分成5个等份，有6个边界值
                //int count = _startValue.Length;
                double var = (maxValue - minValue) / (count - 1);
                for (int i = 0; i < count; i++)
                {
                    _valueArray[i] = _minValue + i * var;
                    _valueArray[i] = Math.Round(_valueArray[i], 3);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        // 初始化设置颜色和边界值
        private void InitdgColorSet()
        {
            try
            {
                // 为DataGridView添加列
                DataGridViewTextBoxColumn newColColor =
                            new DataGridViewTextBoxColumn();
                // newColColor.Width = 60;
                for (int i = 0; i < count; i++)
                {
                    newColColor = new DataGridViewTextBoxColumn();
                    newColColor.Width = 65;
                    dgvSetColor.Columns.Add(newColColor);
                }
                //dgvSetColor.ColumnCount = 6;
                DataGridViewRow newRowColor = new DataGridViewRow();
                // 设置当鼠标移到行标时提示
                newRowColor.HeaderCell.ToolTipText = "Color";
                // 设置行标题
                newRowColor.HeaderCell.Value = "Color";
                // 设置行标题宽度
                // newRowColor.HeaderCell.Size.Width = 100;
                // 设置行高
                newRowColor.Height = 33;
                //newRowColor.HeaderCell.Style.ApplyStyle(newRowColor.HeaderCell.Style.Alignment);

                DataGridViewRow newRowValueArray = new DataGridViewRow();
                // 设置当鼠标移到行标时提示
                newRowValueArray.HeaderCell.ToolTipText = "Boundary Values";
                // 设置行标题
                newRowValueArray.HeaderCell.Value = "Boundary Values";
                // 设置行高
                newRowValueArray.Height = 33;
                //newRowStartValue
                dgvSetColor.AutoSize = false;
                // Or this:
                DataGridViewTextBoxCell newCell = new DataGridViewTextBoxCell();
                for (int i = 0; i < count; i++)
                {
                    newCell = new DataGridViewTextBoxCell();
                    newRowColor.Cells.Add(newCell);
                    // 必须在newCell添加之后才能对newCell进行设置
                    newCell.Style.BackColor = _colorArray[i];
                    newCell.ToolTipText = "Double click to customize the color.";

                    //单元格在添加之前不能设为只读属性
                    newCell.ReadOnly = true;

                    newCell = new DataGridViewTextBoxCell();
                    newRowValueArray.Cells.Add(newCell);
                    //newRowColor.Cells[i].Style.BackColor =
                    //newCell.Resizable = true;
                    newCell.Value = _valueArray[i];
                    //newCell.Size.Width = 25;
                    //newCell.Style.BackColor = colorVarray[i];
                    newCell.ToolTipText = "Boundary Values";
                    //单元格在添加之前不能设为只读属性
                    //newCell.ReadOnly = true;
                }
                //newRowColor.ReadOnly = true;
                newRowColor.ReadOnly = false;
                dgvSetColor.Rows.Add(newRowColor);
                dgvSetColor.Rows.Add(newRowValueArray);
                dgvSetColor.Rows[0].Cells[0].Selected = false;
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        private void dgvSetColor_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == 0)
                {
                    ColorDialog colorDialog = new ColorDialog();
                    DialogResult rtn = colorDialog.ShowDialog();
                    if (rtn == DialogResult.OK)
                    {
                        foreach (DataGridViewCell cell in dgvSetColor.SelectedCells)
                        {
                            // 改变选中框的颜色
                            if (cell.Selected == true)
                            {
                                cell.Style.BackColor = colorDialog.Color;
                                cell.Selected = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        /// <summary>
        /// 根据界面输入的最小、最大值设定边界值
        /// </summary>
        private void btnRebuild_Click(object sender, EventArgs e)
        {
            bool ok = false;
            try
            {
                // 验证输入的最大值和最小值是否符合要求
                double min = 0;
                double max = 0;
                ok = GetMinMaxValues(ref  min, ref max);
                if (!ok) { return; }
                // 根据传出的最小、最大值进行设定
                _minValue = min;
                _maxValue = max;
                SetDefaultValueArray(_minValue, _maxValue);
                for (int i = 0; i < count; i++)
                {
                    _colorArray[i] = dgvSetColor.Rows[0].Cells[i].Style.BackColor;
                    dgvSetColor.Rows[1].Cells[i].Value = _valueArray[i];
                }

                // 事件触发
                OnValueRangeChanged(this);
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        /// <summary>
        /// 获得最大最小值
        /// </summary>
        /// <param name="min">最新值</param>
        /// <param name="max">最大值</param>
        private bool GetMinMaxValues(ref double min, ref double max)
        {
            bool ok = false;
            try
            {
                ok = double.TryParse(txtMin.Text, out min);
                if (!ok)
                {
                    MessageBox.Show("The minimum value must be a number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return ok;
                }
                ok = double.TryParse(txtMax.Text, out max);
                if (!ok)
                {
                    MessageBox.Show("The maximum value must be number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return ok;
                }
                if (max < min)
                {
                    MessageBox.Show("The maximum value must be greater than the minimum value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ok = false;
                    return ok;
                }
                return ok;
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
                return ok;
            }
        }
    }//class
}//namespace