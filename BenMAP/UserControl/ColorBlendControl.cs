using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BenMAP;
using System.Collections.Generic;

namespace WinControls
{
    public partial class ColorBlendControl : UserControl
    {
        [Description("")]
        public event EventHandler CustomizeValueRange;
        protected void OnCustomizeValueRange(object sender, EventArgs e)
        {
            if (CustomizeValueRange != null)
            {
                CustomizeValueRange(sender, e);
            }
        }

        bool _useCustomValueRange = false;
        public double _minPlotValue = double.NaN;
        public double _maxPlotValue = double.NaN;

        double _minValue = 0;
        public double MinValue
        {
            get { return _minValue; }
        }

        double _maxValue = 0;
        public double MaxValue
        {
            get { return _maxValue; }
        }

        double _discreteValueStep = 1;

        double _continuousValueStep = 1;

        int _count = 6;
        Color[] _colorArray = new Color[6];

        public Color[] ColorArray
        {
            get { return _colorArray; }
            set { _colorArray = value; }
        }

        ColorBlend _colorBlend = new ColorBlend();

        private double[] _valueArray = new double[6];

        public double[] ValueArray
        {
            get { return _valueArray; }
            set { _valueArray = value; }
        }

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
            SetDefaultColors();

        }

        private void SetDefaultColors()
        {
            try
            {
                _colorArray[0] = Color.Blue;
                _colorArray[1] = Color.FromArgb(0, 255, 255);
                _colorArray[2] = Color.FromArgb(0, 255, 0);
                _colorArray[3] = Color.Yellow;
                _colorArray[4] = Color.FromArgb(255, 0, 255);
                _colorArray[5] = Color.Red;
                _colorBlend.Colors = _colorArray;
                _colorBlend.Positions = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };
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
            Size size = new Size((int)(Convert.ToDouble(ClientSize.Width) / Convert.ToDouble(ColorArray.Length)), height);
            Rectangle rect = new Rectangle(pt, size);
            for (int i = 0; i < ColorArray.Length; i++)
            {
                pt = new Point(i * (int)(Convert.ToDouble(ClientSize.Width) / Convert.ToDouble(ColorArray.Length)), 0);
                rect = new Rectangle(pt, size);
                e.Graphics.FillRectangle(new SolidBrush(ColorArray[i]), rect);

            }


        }

        public void SetValueRange(double minValue, double maxValue, bool isFirstLoad)
        {
            Int64 tmp = (Int64)(minValue * 10000d);
            minValue = tmp / 10000d;
            tmp = (Int64)(maxValue * 10000d);
            maxValue = tmp / 10000d;
            _minValue = minValue;
            _maxValue = maxValue;
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

            if (isFirstLoad)
            {
                SetDefaultValues(minValue, maxValue);
                //SetDefaultColors();
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

            if ((0.00001 < minValue) && (minValue < 100000)) { lblMin.Text = string.Format("{0}", Math.Round(minValue, 3)); }
            else
            { lblMin.Text = string.Format("{0}", Math.Round(minValue, 3)); } if ((0.00001 < maxValue) && (maxValue < 100000))
            { lblMax.Text = string.Format("{0}", Math.Round(maxValue, 3)); }
            else
            { lblMax.Text = string.Format("{0}", Math.Round(maxValue, 3)); } lblMax.Left = this.ClientSize.Width - lblMax.Width;
            lblMax.Visible = true;
            lblMin.Visible = true;
            this.Invalidate();
        }

        public void pharseValue(double[] v)
        {
            if (!(v.Length == 5))
            { return; }
            ValueArray[0] = 0;
            for (int i = 1; i < 6; i++)
            {
                ValueArray[i] = v[i - 1];
            }
            lblMin.Text = ValueArray[0].ToString();
            lblMin.Visible = true;
            lblMax.Text = v[4].ToString();
            lblMax.Visible = true;
            SetUnitLabelPosition();
            this.Refresh();
        }
        

        private void SetDefaultValues(double minValue, double maxValue)
        {
            try
            {
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

        public Color GetValueColor2(double val)
        {
            try
            {
                int partColor = 255;
                double dblPercent = (val - _minValue) / (_maxValue - _minValue);
                if (dblPercent < 0)
                {
                    return Color.Transparent;
                }
                else if (dblPercent < 0.2)
                {
                    partColor = (int)(255 * 5 * dblPercent);
                    return Color.FromArgb(0, partColor, 255);
                }
                else if (dblPercent < 0.4)
                {
                    partColor = 255 - (int)(255 * 5 * (dblPercent - 0.2));
                    return Color.FromArgb(0, 255, partColor);
                }
                else if (dblPercent < 0.6)
                {
                    partColor = (int)(255 * 5 * (dblPercent - 0.4));
                    return Color.FromArgb(partColor, 255, 0);
                }
                else if (dblPercent < 0.8)
                {
                    partColor = 255 - (int)(255 * 5 * (dblPercent - 0.6));
                    return Color.FromArgb(255, partColor, 0);
                }
                else if (dblPercent <= 1)
                {
                    partColor = (int)(255 * 5 * (dblPercent - 0.8));
                    return Color.FromArgb(255, 0, partColor);
                }
                else
                {
                    return Color.Transparent;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return _colorArray[0];
            }
        }

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


                for (int i = 1; i < _count; i++)
                {
                    if (val < _valueArray[i])
                    {
                        dbPercent = (val - _valueArray[i - 1]) / (_valueArray[i] - _valueArray[i - 1]);
                        a = (byte)(_colorArray[i - 1].A + (_colorArray[i].A - _colorArray[i - 1].A) * dbPercent);
                        r = (byte)(_colorArray[i - 1].R + (_colorArray[i].R - _colorArray[i - 1].R) * dbPercent);
                        g = (byte)(_colorArray[i - 1].G + (_colorArray[i].G - _colorArray[i - 1].G) * dbPercent);
                        b = (byte)(_colorArray[i - 1].B + (_colorArray[i].B - _colorArray[i - 1].B) * dbPercent);
                        partColor = Color.FromArgb(a, r, g, b);
                        break;
                    }
                }
                return partColor;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return _colorArray[0];
            }
        }

        public Color GetValueRangeColor(double val)
        {
            val = _minValue + _discreteValueStep * Math.Floor(val / _discreteValueStep);
            return GetValueColor(val);
        }

        private void SetUnitLabelPosition()
        {
            try
            {
                lblUnit.Left = (this.ClientSize.Width - lblUnit.Width) / 2;
                lblMax.Left = this.ClientSize.Width-lblMax.Width;
                lblMin.Left = 0;
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
                frm.MinValue = _valueArray[0];
                frm.MaxValue = _valueArray[5];

                frm.Unit = lblUnit.Text;
                frm.ValueArray = _valueArray;
                frm.ColorArray = _colorArray;
                bool isFirstLoad = false;
                if (double.IsNaN(_minPlotValue))
                {
                    _minPlotValue = _minValue;
                    _maxPlotValue = _maxValue;
                }

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

        private void SetValueRange_ValueRangeChanged(SetValueRange frm)
        {
            try
            {
                SetValueRangeAndBundaryColor(frm, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void SetValueRangeAndBundaryColor(SetValueRange frm, bool isFirstLoad)
        {
            try
            {
                _useCustomValueRange = false;
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

        public Bitmap DrawImage()
        {
            try
            {
                Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

                this.DrawToBitmap(bmp, this.ClientRectangle); return bmp;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }       
    }
}