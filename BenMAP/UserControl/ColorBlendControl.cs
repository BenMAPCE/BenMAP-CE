using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BenMAP;

namespace WinControls
{
    public partial class ColorBlendControl : UserControl
    {
        /// <summary>
        /// ���ֹ�������ֵ��Χ�󴥷����¼�
        /// </summary>
        [Description("���ֹ�������ֵ��Χ�󴥷����¼�")]
        public event EventHandler CustomizeValueRange;
        protected void OnCustomizeValueRange(object sender, EventArgs e)
        {
            if (CustomizeValueRange != null)
            {
                CustomizeValueRange(sender, e);
            }
        }

        // �û��Ƿ��Զ���ֵ�ķ�Χ
        bool _useCustomValueRange = false;
        // �����ʵ����С�����ֵ
        public double _minPlotValue = double.NaN;
        public double _maxPlotValue = double.NaN;

        // ������Сֵ�����ֵ
        double _minValue = 0;
        /// <summary>
        /// ColorBlend��Сֵ
        /// </summary>
        public double MinValue
        {
            get { return _minValue; }
        }

        double _maxValue = 0;
        /// <summary>
        /// ColorBlend��Сֵ
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
        }

        /// <summary>
        /// ��Ϊ��ɢɫ�ʷֿ�ʱ,ÿ������ɫ�����ֵ����
        /// </summary>
        double _discreteValueStep = 1;

        /// <summary>
        /// ��Ϊ����ɫ��ʱ,ÿ����_colorArray����ֵ����
        /// </summary>
        double _continuousValueStep = 1;

        // Legend�б߽��ĸ���
        int _count = 6;
        // Legend��ɫ��
        Color[] _colorArray = new Color[6];

        // �û����Ը����Լ���Ҫ����ɫ��
        public Color[] ColorArray
        {
            get { return _colorArray; }
            set { _colorArray = value; }
        }

        // ���»�Legend
        ColorBlend _colorBlend = new ColorBlend();

        private double[] _valueArray = new double[6];

        /// <summary>
        /// Legend��ÿ��ɫ��Ŀ�ʼֵ,�����Զ����Colorblend,Ŀǰδ��
        /// </summary>
        public double[] ValueArray
        {
            get { return _valueArray; }
            set { _valueArray = value; }
        }

        /// <summary>
        /// ֵ�ĵ�λ
        /// </summary>
        public string ValueUnit
        {
            set
            {
                lblUnit.Text = value;
                lblUnit.Visible = true;
                SetUnitLabelPosition();
            }
        }

        public ColorBlendControl()
        {
            InitializeComponent();
            // �����ڹ��캯���г�ʼ���߽�����ɫ�����������쳣
            SetDefaultColors();

            //lblUnit.Text = "";
        }

        // ��ʼ���߽����ɫ��Ҳ������������Ĭ����ɫ
        private void SetDefaultColors()
        {
            try
            {
                _colorArray[0] = Color.Blue;
                _colorArray[1] = Color.FromArgb(0, 255, 255);
                _colorArray[2] = Color.FromArgb(0, 255, 0);
                _colorArray[3] = Color.Yellow;//�� Color.FromArgb(255, 255, 0);
               
                _colorArray[4] = Color.FromArgb(255, 0, 255);
                _colorArray[5] = Color.Red;
                //���������ɫ
                _colorBlend.Colors = _colorArray;
                _colorBlend.Positions = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };
                // ��Legend��ɫ����ֵ��һ������
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                e.Graphics.Clear(Color.White);
            }
            catch
            { 
            }
            Point pt = new Point(0, 0);
            int height = lblMax.Top - 2;
            if (height < 10) { height = 10; }
            Size size = new Size((int)(Convert.ToDouble(ClientSize.Width)/Convert.ToDouble(ColorArray.Length)), height);
            Rectangle rect = new Rectangle(pt, size);
            for (int i = 0; i < ColorArray.Length; i++)
            {
                pt = new Point(i * (int)(Convert.ToDouble(ClientSize.Width) / Convert.ToDouble(ColorArray.Length)), 0);
                rect = new Rectangle(pt, size);
                e.Graphics.FillRectangle(new SolidBrush(ColorArray[i]), rect);
 
            }
            //--------����
            //Size size = new Size(ClientSize.Width, height);
            //using (LinearGradientBrush brush = new LinearGradientBrush(rect, _colorArray[0], _colorArray[1], LinearGradientMode.Horizontal))
            //{
            //    brush.InterpolationColors = _colorBlend;
            //    e.Graphics.FillRectangle(brush, rect);
            //}

            //-----------modify by xiejp -------��Ҫ��������ֱ����ɫ�飡

        }

        /// <summary>
        /// �������ֵ,��Сֵ��ColorBlendControl����ʾ
        /// </summary>
        /// <param name="minValue">�������Сֵ</param>
        /// <param name="maxValue">��������ֵ</param>
        /// <param name="isFirstLoad">���������Ƿ�ָ�Ĭ��״̬:true �ָ�Ĭ��״̬,flase �����û��趨����</param>
        public void SetValueRange(double minValue, double maxValue, bool isFirstLoad)
        {
            Int64 tmp = (Int64)(minValue * 10000d);
            minValue = tmp / 10000d;
            tmp = (Int64)(maxValue * 10000d);
            maxValue = tmp / 10000d;
            _minValue = minValue;
            _maxValue = maxValue;
            //--------------���valuearray�Ƿ�����������ʹ��valuearray----
            bool IsValidValueArray = false;
            for (int i = 1; i < 6; i++)
            {
                if (_valueArray[i] <= _valueArray[i - 1])
                {
                    IsValidValueArray = false;
                    break;
                }
                else
                {
                    IsValidValueArray = true;
                }
            }
           
            //DefaultStartValue(_minValue, _maxValue);
            if (isFirstLoad)
            {
                SetDefaultValues(minValue, maxValue);
                SetDefaultColors();
            }
            else
            {
                if (IsValidValueArray == false)
                {
                    SetDefaultValues(minValue, maxValue);

                }
                else
                {
                    maxValue = _valueArray[5];
                    _maxValue = maxValue;
                    minValue = _valueArray[0];
                    _minValue = minValue;
                }
            }
            //lblMin.Text = minValue.ToString("0.###");// minValue.ToString("g4");g4->general 4 λ��Ч����
            //lblMax.Text = maxValue.ToString("0.###");// maxValue.ToString("g4");

            if ((0.00001 < minValue) && (minValue < 100000)) { lblMin.Text = string.Format("{0}", Math.Round(minValue, 3)); }
            else
            { lblMin.Text = string.Format("{0}", Math.Round(minValue, 3)); }//.ToString("G3")); }
            if ((0.00001 < maxValue) && (maxValue < 100000))
            { lblMax.Text = string.Format("{0}", Math.Round(maxValue, 3)); }
            else
            { lblMax.Text = string.Format("{0}", Math.Round(maxValue, 3)); }//.ToString("G3")); }
            //lblMin.Text = string.Format("{0}", Math.Round(minValue, 3).ToString("E2"));
            //lblMax.Text = string.Format("{0}", Math.Round(maxValue, 3).ToString("E2"));
            lblMax.Left = this.ClientSize.Width - lblMax.Width;
            lblMax.Visible = true;
            lblMin.Visible = true;
            this.Invalidate();
        }

        // ����Ĭ�Ϲ������ñ߽���ֵ
        private void SetDefaultValues(double minValue, double maxValue)
        {
            try
            {
                //��ʼ��ɫ�飺����ɫ���������߽�����
                //int count = _valueArray.Length;
                // ��Сֵ�����ֵ֮��ֳɵĵȷ���Ϊ5��
                double var = (maxValue - minValue) / (_count);
                _valueArray[0] = minValue;
                _valueArray[_valueArray.Length - 1] = maxValue;
                for (int i = 0; i < _count; i++)
                {
                    _valueArray[i] = minValue + i * var;
                    _valueArray[i] = Math.Round(_valueArray[i], 3);
                }

                _continuousValueStep = var;
                _discreteValueStep = (maxValue - minValue) / 10.0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// ���ݴ��������ֵ,ȡ����Ӧ����ɫ
        /// </summary>
        /// <param name="val">�����ֵ</param>
        /// <returns>��Ӧ����ɫ</returns>
        public Color GetValueColor2(double val)
        {
            try
            {
                //int zoneIndex = 0; // ��������,��_colorArray[]ȷ��,����Ϊ5
                int partColor = 255;
                double dblPercent = (val - _minValue) / (_maxValue - _minValue);
                // Console.Write(dblPercent+"  ");
                //zoneIndex = (int)dblPercent;
                if (dblPercent < 0)
                {
                    //return _colorArray[0];
                    return Color.Transparent;
                }
                else if (dblPercent < 0.2)
                {
                    partColor = (int)(255 * 5 * dblPercent);
                    // Console.WriteLine(partColor);
                    return Color.FromArgb(0, partColor, 255);
                }
                else if (dblPercent < 0.4)
                {
                    partColor = 255 - (int)(255 * 5 * (dblPercent - 0.2));
                    //Console.WriteLine(partColor);
                    return Color.FromArgb(0, 255, partColor);
                }
                else if (dblPercent < 0.6)
                {
                    partColor = (int)(255 * 5 * (dblPercent - 0.4));
                    //Console.WriteLine(partColor);
                    return Color.FromArgb(partColor, 255, 0);
                }
                else if (dblPercent < 0.8)
                {
                    partColor = 255 - (int)(255 * 5 * (dblPercent - 0.6));
                    //Console.WriteLine(partColor);
                    return Color.FromArgb(255, partColor, 0);
                }
                else if (dblPercent <= 1)
                {
                    partColor = (int)(255 * 5 * (dblPercent - 0.8));
                    //Console.WriteLine(partColor);
                    return Color.FromArgb(255, 0, partColor);
                }
                else
                {
                    //return _colorArray[5];
                    return Color.Transparent;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return _colorArray[0];
            }
        }

        /// <summary>
        /// ���ݴ��������ֵ,ȡ����Ӧ����ɫ.���Դ���Ⱦ���Զ�����ɫ��Χ���
        /// </summary>
        /// <param name="val">�����ֵ</param>
        /// <returns>��Ӧ����ɫ</returns>
        public Color GetValueColor(double val)
        {
            try
            {
                Color partColor = Color.Transparent;
                if (val < _valueArray[0])
                {
                    return _colorArray[0];
                }
                else if (val > _valueArray[_count - 1])
                {
                    return _colorArray[_count - 1];
                }
                double dbPercent = 0.00;
                byte a = 255;
                byte r = 255;
                byte g = 255;
                byte b = 255;

                // ֻ�ܴ���Ⱦ����
                //double tmp = val / _continuousValueStep;
                //int i = (int)Math.Floor(tmp);
                //double dbPercent = tmp - i;
                //a = (byte)(_colorArray[i].A + (_colorArray[i + 1].A - _colorArray[i].A) * dbPercent);
                //r = (byte)(_colorArray[i].R + (_colorArray[i + 1].R - _colorArray[i].R) * dbPercent);
                //g = (byte)(_colorArray[i].G + (_colorArray[i + 1].G - _colorArray[i].G) * dbPercent);
                //b = (byte)(_colorArray[i].B + (_colorArray[i + 1].B - _colorArray[i].B) * dbPercent);
                //partColor = Color.FromArgb(a, r, g, b);

                // count��ȫ�ֱ���,���ڱ߽��ĸ���
                for (int i = 1; i < _count; i++)
                {
                    // ���ݴ���ֵ�����ж�������ɫ
                    // �ж�˼·:�������ֵС����С�߽�ֵ����ô��ʾΪ��һ����ɫ��;
                    // �������ֵ�������߽�ֵ,��ô��ʾΪ��һ����ɫ
                    if (val < _valueArray[i])
                    {
                        dbPercent = (val - _valueArray[i - 1]) / (_valueArray[i] - _valueArray[i - 1]);
                        //partColor.ToKnownColor();
                        a = (byte)(_colorArray[i - 1].A + (_colorArray[i].A - _colorArray[i - 1].A) * dbPercent);
                        // Console.Write(a+" ");
                        r = (byte)(_colorArray[i - 1].R + (_colorArray[i].R - _colorArray[i - 1].R) * dbPercent);
                        g = (byte)(_colorArray[i - 1].G + (_colorArray[i].G - _colorArray[i - 1].G) * dbPercent);
                        b = (byte)(_colorArray[i - 1].B + (_colorArray[i].B - _colorArray[i - 1].B) * dbPercent);
                        // tmpColorValue = (int)(_colorArray[i - 1].ToArgb() + (_colorArray[i].ToArgb() - _colorArray[i - 1].ToArgb()) * dbPercent);
                        // partColor = Color.FromArgb(tmpColorValue);
                        partColor = Color.FromArgb(a, r, g, b);
                        break;
                    }
                    //}
                }
                return partColor;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return _colorArray[0];
            }
        }

        /// <summary>
        /// ���ݴ��������ֵ,ȡ����Ӧ����ɫ,�޲�ֵ
        /// </summary>
        /// <param name="val">�����ֵ</param>
        /// <returns>��Ӧ����ɫ</returns>
        public Color GetValueRangeColor(double val)
        {
            val = _minValue + _discreteValueStep * Math.Floor(val / _discreteValueStep);
            return GetValueColor(val);
        }

        /// <summary>
        /// ���õ�λLabel��λ��
        /// </summary>
        private void SetUnitLabelPosition()
        {
            try
            {
                lblUnit.Left = (this.ClientSize.Width - lblUnit.Width) / 2;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                if (_maxValue > _minValue)
                {
                    double val = _minValue + ((double)e.X / (double)this.Width) * (_maxValue - _minValue);
                    //toolTip1.SetToolTip(this, val.ToString("0.##"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            finally
            {
                base.OnMouseMove(e);
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            try
            {
                SetValueRange frm = new SetValueRange();
                // ����С�����ֵ������λ���뵽Set Value Range������
                frm.MinValue = _minValue;
                frm.MaxValue = _maxValue;

                frm.Unit = lblUnit.Text;
                // ��ÿ������Ŀ�ʼֵ��ÿ���ȷֵ�ɫ����Ϊ���鴫�뵽Set Value Range������
                frm.ValueArray = _valueArray;
                frm.ColorArray = _colorArray;
                //�����ؼ���һ��ʹ��ʱisFirstLoad=True,����Ϊfalse
                bool isFirstLoad = false;
                // ��¼Ĭ��ֵ
                // ע������ִ�е��Ⱥ�˳��Ĭ��ֵ�ĸ�ֵ�������¼�ί��֮ǰ������Ϊ��������ִ�к�ĵ�һ��ִ�С�Rebuild����OK��
                if (double.IsNaN(_minPlotValue))
                {
                    _minPlotValue = _minValue;
                    _maxPlotValue = _maxValue;
                }

                // ��ʵ��ί�и�������ΪSetValueRange_ValueRangeChanged�������д���
                frm.ValueRangeChanged += SetValueRange_ValueRangeChanged;

                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    isFirstLoad = false;
                    SetValueRangeAndBundaryColor(frm, isFirstLoad);
                }
                else if (rtn == DialogResult.Ignore)
                {
                    isFirstLoad = true;
                    SetValueRangeAndBundaryColor(frm, isFirstLoad);
                }

                // ���ٶ���
                frm.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            finally
            {
                base.OnDoubleClick(e);
            }
        }

        // �¼�SetValueRange_ValueRangeChanged�Ĵ�����
        private void SetValueRange_ValueRangeChanged(SetValueRange frm)
        {
            try
            {
                // MessageBox.Show("........");
                SetValueRangeAndBundaryColor(frm, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// ���ݴ���ֵ����ColorBlendControl�ı߽�����ɫ
        /// </summary>
        ///<param name="frm">��������</param>
        /// <param name="isFirstLoad">tru:�ָ�Ĭ�����ã�false:Ϊ����ʱ��ֵ</param>
        private void SetValueRangeAndBundaryColor(SetValueRange frm, bool isFirstLoad)
        {
            try
            {
                _useCustomValueRange = false;
                //SetValueRange();
                if (!isFirstLoad)
                {
                    _minValue = frm.MinValue;
                    _maxValue = frm.MaxValue;
                    _colorArray = frm.ColorArray;
                    _valueArray = frm.ValueArray;
                    SetValueRange(_minValue, _maxValue, isFirstLoad);
                }
                else
                {
                    SetValueRange(_minPlotValue, _maxPlotValue, isFirstLoad);
                }
                _useCustomValueRange = true;
                // �������úõ���ɫ�ͱ߽�ֵ
                OnCustomizeValueRange(this, null);
                this.Invalidate();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                bool isLegend = true;
                SavePicture save = new SavePicture(this);
                save.SaveAs(isLegend);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// ��ͼ�λ���һ��Bitmap�Ϸ���.��Bitmap�ڱ���ʱ���õ�,���ڵ�ǰ������OnPaint��������ʱ,ʹ�õ���_bmp,��ֱ��ʹ�÷���ֵ
        /// </summary>
        /// <returns></returns>
        public Bitmap DrawImage()
        {
            try
            {
                Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                //using (Graphics bmpGraphics = Graphics.FromImage(bmp))
                //{
                //    Point pt = new Point(0, 0);
                //    int height = lblMax.Top - 2;
                //    if (height < 10) { height = 10; }
                //    Size size = new Size(this.ClientSize.Width, height);
                //    Rectangle rect = new Rectangle(pt, size);
                //    using (LinearGradientBrush brush = new LinearGradientBrush(rect, _colorArray[0], _colorArray[1], LinearGradientMode.Horizontal))
                //    {
                //        brush.InterpolationColors = _colorBlend;
                //        e.Graphics.FillRectangle(brush, rect);
                //    }

                //}
                this.DrawToBitmap(bmp, this.ClientRectangle);// new Rectangle(new Point(0,0),new Size(bmp.Width,bmp.Height)));
                return bmp;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
    }//class
}//namespace