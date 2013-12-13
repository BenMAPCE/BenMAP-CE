using System;
using System.Drawing;
using System.Windows.Forms;
using BenMAP;

namespace WinControls
{
    // ����һ��ί��
    public delegate void ValueRangeChangedHanler(SetValueRange frm);

    public partial class SetValueRange : FormBase
    {
        // ����һ���¼�
        public event ValueRangeChangedHanler ValueRangeChanged;

        // �����¼��Ĵ�����
        protected void OnValueRangeChanged(SetValueRange frm)
        {
            if (ValueRangeChanged != null)
            {
                ValueRangeChanged(frm);
            }
        }

        private double _minValue;

        /// <summary>
        /// ��Сֵ
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
        /// ���ֵ
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
        /// ��λ
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

        // ����߽��ĸ���
        private int count = 6;

        // Legend��ɫ��
        Color[] _colorArray = new Color[6];

        // �û����Ը����Լ���Ҫ����ɫ��
        public Color[] ColorArray
        {
            get { return _colorArray; }
            set
            {
                _colorArray = value;
            }
        }

        // Legend��ÿ��ɫ��Ŀ�ʼֵ
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
                // ��֤�߽�������ֵ�Ƿ��Ǵ�С�����˳�����������ʾ�趨����
                ok = VerifyBoundaryValue();
                if (!ok) { return; }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        // ��֤�߽�����Ч��
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

        // ��ʼ��Legend��ֵ
        private void SetDefaultValueArray(double minValue, double maxValue)
        {
            try
            {
                //��ɫ��ֳ�5���ȷݣ���6���߽�ֵ
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

        // ��ʼ��������ɫ�ͱ߽�ֵ
        private void InitdgColorSet()
        {
            try
            {
                // ΪDataGridView�����
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
                // ���õ�����Ƶ��б�ʱ��ʾ
                newRowColor.HeaderCell.ToolTipText = "Color";
                // �����б���
                newRowColor.HeaderCell.Value = "Color";
                // �����б�����
                // newRowColor.HeaderCell.Size.Width = 100;
                // �����и�
                newRowColor.Height = 33;
                //newRowColor.HeaderCell.Style.ApplyStyle(newRowColor.HeaderCell.Style.Alignment);

                DataGridViewRow newRowValueArray = new DataGridViewRow();
                // ���õ�����Ƶ��б�ʱ��ʾ
                newRowValueArray.HeaderCell.ToolTipText = "Boundary Values";
                // �����б���
                newRowValueArray.HeaderCell.Value = "Boundary Values";
                // �����и�
                newRowValueArray.Height = 33;
                //newRowStartValue
                dgvSetColor.AutoSize = false;
                // Or this:
                DataGridViewTextBoxCell newCell = new DataGridViewTextBoxCell();
                for (int i = 0; i < count; i++)
                {
                    newCell = new DataGridViewTextBoxCell();
                    newRowColor.Cells.Add(newCell);
                    // ������newCell���֮����ܶ�newCell��������
                    newCell.Style.BackColor = _colorArray[i];
                    newCell.ToolTipText = "Double click to customize the color.";

                    //��Ԫ�������֮ǰ������Ϊֻ������
                    newCell.ReadOnly = true;

                    newCell = new DataGridViewTextBoxCell();
                    newRowValueArray.Cells.Add(newCell);
                    //newRowColor.Cells[i].Style.BackColor =
                    //newCell.Resizable = true;
                    newCell.Value = _valueArray[i];
                    //newCell.Size.Width = 25;
                    //newCell.Style.BackColor = colorVarray[i];
                    newCell.ToolTipText = "Boundary Values";
                    //��Ԫ�������֮ǰ������Ϊֻ������
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
                            // �ı�ѡ�п����ɫ
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
        /// ���ݽ����������С�����ֵ�趨�߽�ֵ
        /// </summary>
        private void btnRebuild_Click(object sender, EventArgs e)
        {
            bool ok = false;
            try
            {
                // ��֤��������ֵ����Сֵ�Ƿ����Ҫ��
                double min = 0;
                double max = 0;
                ok = GetMinMaxValues(ref  min, ref max);
                if (!ok) { return; }
                // ���ݴ�������С�����ֵ�����趨
                _minValue = min;
                _maxValue = max;
                SetDefaultValueArray(_minValue, _maxValue);
                for (int i = 0; i < count; i++)
                {
                    _colorArray[i] = dgvSetColor.Rows[0].Cells[i].Style.BackColor;
                    dgvSetColor.Rows[1].Cells[i].Value = _valueArray[i];
                }

                // �¼�����
                OnValueRangeChanged(this);
            }
            catch (Exception ex)
            {
                Logger.LogError (ex);
            }
        }

        /// <summary>
        /// ��������Сֵ
        /// </summary>
        /// <param name="min">����ֵ</param>
        /// <param name="max">���ֵ</param>
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