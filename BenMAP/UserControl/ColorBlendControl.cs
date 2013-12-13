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
        /// 当手工设置了值范围后触发的事件
        /// </summary>
        [Description("当手工设置了值范围后触发的事件")]
        public event EventHandler CustomizeValueRange;
        protected void OnCustomizeValueRange(object sender, EventArgs e)
        {
            if (CustomizeValueRange != null)
            {
                CustomizeValueRange(sender, e);
            }
        }

        // 用户是否自定义值的范围
        bool _useCustomValueRange = false;
        // 窗体的实际最小和最大值
        public double _minPlotValue = double.NaN;
        public double _maxPlotValue = double.NaN;

        // 声明最小值和最大值
        double _minValue = 0;
        /// <summary>
        /// ColorBlend最小值
        /// </summary>
        public double MinValue
        {
            get { return _minValue; }
        }

        double _maxValue = 0;
        /// <summary>
        /// ColorBlend最小值
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
        }

        /// <summary>
        /// 当为离散色彩分块时,每相邻颜色块的数值距离
        /// </summary>
        double _discreteValueStep = 1;

        /// <summary>
        /// 当为连续色彩时,每相邻_colorArray的数值距离
        /// </summary>
        double _continuousValueStep = 1;

        // Legend中边界点的个数
        int _count = 6;
        // Legend的色块
        Color[] _colorArray = new Color[6];

        // 用户可以根据自己需要定义色块
        public Color[] ColorArray
        {
            get { return _colorArray; }
            set { _colorArray = value; }
        }

        // 重新画Legend
        ColorBlend _colorBlend = new ColorBlend();

        private double[] _valueArray = new double[6];

        /// <summary>
        /// Legend中每个色块的开始值,用于自定义的Colorblend,目前未用
        /// </summary>
        public double[] ValueArray
        {
            get { return _valueArray; }
            set { _valueArray = value; }
        }

        /// <summary>
        /// 值的单位
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
            // 必须在构造函数中初始化边界点的颜色，否则会产生异常
            SetDefaultColors();

            //lblUnit.Text = "";
        }

        // 初始化边界点颜色，也可以用于设置默认颜色
        private void SetDefaultColors()
        {
            try
            {
                _colorArray[0] = Color.Blue;
                _colorArray[1] = Color.FromArgb(0, 255, 255);
                _colorArray[2] = Color.FromArgb(0, 255, 0);
                _colorArray[3] = Color.Yellow;//即 Color.FromArgb(255, 255, 0);
               
                _colorArray[4] = Color.FromArgb(255, 0, 255);
                _colorArray[5] = Color.Red;
                //定义多种颜色
                _colorBlend.Colors = _colorArray;
                _colorBlend.Positions = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };
                // 给Legend颜色条赋值给一个数组
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
            //--------渐变
            //Size size = new Size(ClientSize.Width, height);
            //using (LinearGradientBrush brush = new LinearGradientBrush(rect, _colorArray[0], _colorArray[1], LinearGradientMode.Horizontal))
            //{
            //    brush.InterpolationColors = _colorBlend;
            //    e.Graphics.FillRectangle(brush, rect);
            //}

            //-----------modify by xiejp -------不要让它渐变直接颜色块！

        }

        /// <summary>
        /// 设置最大值,最小值在ColorBlendControl上显示
        /// </summary>
        /// <param name="minValue">传入的最小值</param>
        /// <param name="maxValue">传入的最大值</param>
        /// <param name="isFirstLoad">用来控制是否恢复默认状态:true 恢复默认状态,flase 按照用户设定进行</param>
        public void SetValueRange(double minValue, double maxValue, bool isFirstLoad)
        {
            Int64 tmp = (Int64)(minValue * 10000d);
            minValue = tmp / 10000d;
            tmp = (Int64)(maxValue * 10000d);
            maxValue = tmp / 10000d;
            _minValue = minValue;
            _maxValue = maxValue;
            //--------------检查valuearray是否合理，如果合理使用valuearray----
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
            //lblMin.Text = minValue.ToString("0.###");// minValue.ToString("g4");g4->general 4 位有效数字
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

        // 按照默认规则设置边界点的值
        private void SetDefaultValues(double minValue, double maxValue)
        {
            try
            {
                //初始化色块：整个色块有六个边界点组成
                //int count = _valueArray.Length;
                // 最小值和最大值之间分成的等分数为5份
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
        /// 根据传入的网格值,取得相应的颜色
        /// </summary>
        /// <param name="val">网格的值</param>
        /// <returns>对应的颜色</returns>
        public Color GetValueColor2(double val)
        {
            try
            {
                //int zoneIndex = 0; // 区间索引,由_colorArray[]确定,本例为5
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
        /// 根据传入的网格值,取得相应的颜色.可以处理等距和自定义颜色范围情况
        /// </summary>
        /// <param name="val">网格的值</param>
        /// <returns>对应的颜色</returns>
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

                // 只能处理等距情况
                //double tmp = val / _continuousValueStep;
                //int i = (int)Math.Floor(tmp);
                //double dbPercent = tmp - i;
                //a = (byte)(_colorArray[i].A + (_colorArray[i + 1].A - _colorArray[i].A) * dbPercent);
                //r = (byte)(_colorArray[i].R + (_colorArray[i + 1].R - _colorArray[i].R) * dbPercent);
                //g = (byte)(_colorArray[i].G + (_colorArray[i + 1].G - _colorArray[i].G) * dbPercent);
                //b = (byte)(_colorArray[i].B + (_colorArray[i + 1].B - _colorArray[i].B) * dbPercent);
                //partColor = Color.FromArgb(a, r, g, b);

                // count是全局变量,等于边界点的个数
                for (int i = 1; i < _count; i++)
                {
                    // 根据传入值进行判断填充的颜色
                    // 判断思路:如果传入值小于最小边界值，那么显示为第一个颜色点;
                    // 如果传入值大于最大边界值,那么显示为后一个颜色
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
        /// 根据传入的网格值,取得相应的颜色,无插值
        /// </summary>
        /// <param name="val">网格的值</param>
        /// <returns>对应的颜色</returns>
        public Color GetValueRangeColor(double val)
        {
            val = _minValue + _discreteValueStep * Math.Floor(val / _discreteValueStep);
            return GetValueColor(val);
        }

        /// <summary>
        /// 设置单位Label的位置
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
                // 将最小、最大值传、单位传入到Set Value Range窗体中
                frm.MinValue = _minValue;
                frm.MaxValue = _maxValue;

                frm.Unit = lblUnit.Text;
                // 将每个间隔的开始值和每个等分的色块作为数组传入到Set Value Range窗体中
                frm.ValueArray = _valueArray;
                frm.ColorArray = _colorArray;
                //　当控件第一次使用时isFirstLoad=True,否则为false
                bool isFirstLoad = false;
                // 记录默认值
                // 注意代码的执行的先后顺序，默认值的赋值必须在事件委托之前，否则为弹出窗体执行后的第一次执行“Rebuild”或“OK”
                if (double.IsNaN(_minPlotValue))
                {
                    _minPlotValue = _minValue;
                    _maxPlotValue = _maxValue;
                }

                // 将实践委托给方法名为SetValueRange_ValueRangeChanged方法进行处理
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

                // 销毁对象
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

        // 事件SetValueRange_ValueRangeChanged的处理方法
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
        /// 根据传入值设置ColorBlendControl的边界点和颜色
        /// </summary>
        ///<param name="frm">弹出窗体</param>
        /// <param name="isFirstLoad">tru:恢复默认设置，false:为设置时的值</param>
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
                // 传回设置好的颜色和边界值
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
        /// 把图形画到一个Bitmap上返回.该Bitmap在保存时会用到,但在当前窗体由OnPaint方法调用时,使用的是_bmp,不直接使用返回值
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