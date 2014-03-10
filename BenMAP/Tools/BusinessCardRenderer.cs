using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BenMAP.Tools
{
    internal class BusinessCardRenderer : AbstractRenderer
    {
        public override bool RenderItem(DrawListViewItemEventArgs e, Graphics g, Rectangle itemBounds, object rowObject)
        {
            ObjectListView olv = e.Item.ListView as ObjectListView;
            if (olv == null || olv.View != View.Tile)
                return false;

            BufferedGraphics buffered = BufferedGraphicsManager.Current.Allocate(g, itemBounds);
            g = buffered.Graphics;
            g.Clear(olv.BackColor);
            g.SmoothingMode = ObjectListView.SmoothingMode;
            g.TextRenderingHint = ObjectListView.TextRenderingHint;

            if (e.Item.Selected)
            {
                this.BorderPen = Pens.Blue;
                this.HeaderBackBrush = new SolidBrush(olv.HighlightBackgroundColorOrDefault);
            }
            else
            {
                this.BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
                this.HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
            }
            DrawBusinessCard(g, itemBounds, rowObject, olv, (OLVListItem)e.Item);

            buffered.Render();
            buffered.Dispose();

            return true;
        }

        internal Pen BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
        internal Brush TextBrush = new SolidBrush(Color.FromArgb(0x22, 0x22, 0x22));
        internal Brush HeaderTextBrush = Brushes.AliceBlue;
        internal Brush HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
        public Brush BackBrush = Brushes.LemonChiffon;

        public void DrawBusinessCard(Graphics g, Rectangle itemBounds, object rowObject, ObjectListView olv, OLVListItem item)
        {
            const int spacing = 8;

            itemBounds.Inflate(-2, -2);

            const int rounding = 20;
            GraphicsPath path = this.GetRoundedRect(itemBounds, rounding);
            g.FillPath(this.BackBrush, path);
            g.DrawPath(this.BorderPen, path);

            g.Clip = new Region(itemBounds);

            Rectangle photoRect = itemBounds;
            photoRect.Inflate(-spacing, -spacing);

            RectangleF textBoxRect = photoRect;
            textBoxRect.X += (photoRect.Width + spacing);
            textBoxRect.Width = itemBounds.Right - textBoxRect.X - spacing;
            textBoxRect.X = itemBounds.Left + spacing;
            textBoxRect.Width = itemBounds.Right - textBoxRect.X - spacing;
            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Near;
            String txt = item.Text;

            using (Font font = new Font("Tahoma", 11))
            {
                SizeF size = g.MeasureString(txt, font, (int)textBoxRect.Width, fmt);
                RectangleF r3 = textBoxRect;
                r3.Height = size.Height;
                path = this.GetRoundedRect(r3, 15);
                g.FillPath(this.HeaderBackBrush, path);
                g.DrawString(txt, font, this.HeaderTextBrush, textBoxRect, fmt);
                textBoxRect.Y += size.Height + spacing;
            }

            using (Font font = new Font("Tahoma", 8))
            {
                SizeF size = g.MeasureString("Wj", font, itemBounds.Width, fmt);
                textBoxRect.Height = size.Height;
                fmt.Alignment = StringAlignment.Near;
                for (int i = 0; i < olv.Columns.Count; i++)
                {
                    OLVColumn column = olv.GetColumn(i);
                    if (column.IsTileViewColumn)
                    {
                        txt = column.Text + ":" + column.GetStringValue(rowObject);
                        g.DrawString(txt, font, this.TextBrush, textBoxRect, fmt);
                        textBoxRect.Y += size.Height;
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRect(RectangleF rect, float diameter)
        {
            GraphicsPath path = new GraphicsPath();

            RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

    internal class IncidenceBusinessCardRenderer : AbstractRenderer
    {
        public List<int> lstExists;
        public override bool RenderItem(DrawListViewItemEventArgs e, Graphics g, Rectangle itemBounds, object rowObject)
        {
            ObjectListView olv = e.Item.ListView as ObjectListView;
            if (olv == null || olv.View != View.Tile)
                return false;

            BufferedGraphics buffered = BufferedGraphicsManager.Current.Allocate(g, itemBounds);
            g = buffered.Graphics;
            g.Clear(olv.BackColor);
            g.SmoothingMode = ObjectListView.SmoothingMode;
            g.TextRenderingHint = ObjectListView.TextRenderingHint;

            if (e.Item.Selected)
            {
                this.BorderPen = new Pen(Color.Black, 3);
                this.HeaderBackBrush = new SolidBrush(olv.HighlightBackgroundColorOrDefault);
            }
            else
            {
                this.BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
                this.HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
            }
            itemBounds.Height = itemBounds.Height - 6;
            DrawBusinessCard(g, itemBounds, rowObject, olv, (OLVListItem)e.Item);

            buffered.Render();
            buffered.Dispose();

            return true;
        }

        internal Pen BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
        internal Pen BorderPenGrey = new Pen(Color.Silver, 6);
        internal Brush TextBrush = new SolidBrush(Color.White); internal Brush HeaderTextBrush = Brushes.AliceBlue;
        internal Brush HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
        internal Brush whiteBrush = new SolidBrush(Color.White);
        internal Brush orangeBrush = new SolidBrush(Color.Orange);
        internal Brush BackBrush = Brushes.LemonChiffon;
        internal Brush BackBrushExist = new SolidBrush(System.Drawing.Color.FromArgb(193, 195, 243));

        public void DrawBusinessCard(Graphics g, Rectangle itemBounds, object rowObject, ObjectListView olv, OLVListItem item)
        {
            const int spacing = 8;

            itemBounds.Inflate(-2, -2);

            const int rounding = 20;
            GraphicsPath path = this.GetRoundedRect(itemBounds, rounding);
            Rectangle backBounds = new Rectangle(itemBounds.Location, itemBounds.Size);
            backBounds.Offset(0, 5);
            GraphicsPath pathBack = this.GetRoundedRect(backBounds, rounding);
            int iExist = 0;
            if (rowObject is CRSelectFunctionCalculateValue && lstExists != null && lstExists.Count > 0 && lstExists.Contains((rowObject as CRSelectFunctionCalculateValue).CRSelectFunction.CRID))
            {
                iExist = lstExists.Count(p => p == (rowObject as CRSelectFunctionCalculateValue).CRSelectFunction.CRID);
            }
            g.FillPath(new SolidBrush(Color.FromArgb(32, 86, 129)), pathBack);
            using (LinearGradientBrush brush = new LinearGradientBrush(itemBounds, Color.FromArgb(109, 166, 213), Color.FromArgb(48, 114, 175), LinearGradientMode.Vertical))
            {

                g.FillPath(brush, path);
            }
            if (BorderPen.Width > 2)
                g.DrawPath(this.BorderPen, path);
            Rectangle rectClip = new Rectangle(itemBounds.Location, itemBounds.Size);
            rectClip.Inflate(2, 2);
            g.Clip = new Region(rectClip);

            Rectangle photoRect = itemBounds;
            photoRect.Inflate(-spacing, -spacing);

            RectangleF textBoxRect = photoRect;
            textBoxRect.X += (photoRect.Width + spacing);
            textBoxRect.Width = itemBounds.Right - textBoxRect.X - spacing;
            textBoxRect.X = itemBounds.Left + spacing;
            textBoxRect.Width = itemBounds.Right - textBoxRect.X - spacing;
            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
            fmt.Trimming = StringTrimming.EllipsisCharacter;
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Near;
            String txt = item.Text;




            using (Font font = new Font("Tahoma", 10, FontStyle.Bold))
            {
                if (iExist > 0)
                {
                    RectangleF r5 = new RectangleF(textBoxRect.X + (int)textBoxRect.Width - 6, textBoxRect.Y - 10, 16, 16);
                    path = this.GetRoundedRect(r5, 8);
                    g.FillPath(new SolidBrush(Color.FromArgb(221, 140, 1)), path);
                    g.DrawString(iExist.ToString(), font, this.HeaderTextBrush, r5, fmt);

                }
            }
            using (Font font = new Font("Tahoma", 8, FontStyle.Bold))
            {
                SizeF size = g.MeasureString("Wj", font, itemBounds.Width, fmt);
                textBoxRect.Height = size.Height;
                fmt.Alignment = StringAlignment.Near;
                OLVColumn olvStartAge = null, olvEndAge = null;

                for (int i = 0; i < olv.Columns.Count; i++)
                {
                    OLVColumn column = olv.GetColumn(i);
                    if (column.IsTileViewColumn && column.Text.Replace(" ", "").ToLower() == "startage")
                    {
                        olvStartAge = column;
                    }
                    else if (column.IsTileViewColumn && column.Text.Replace(" ", "").ToLower() == "endage")
                    {
                        olvEndAge = column;
                    }

                }
                string txtAge = "";
                if (olvStartAge != null)
                {
                    txtAge = olvStartAge.GetStringValue(rowObject);
                }
                if (olvEndAge != null)
                {
                    txtAge = txtAge == "" ? olvEndAge.GetStringValue(rowObject) : txtAge + "-" + olvEndAge.GetStringValue(rowObject);
                }
                if (txtAge != "")
                {
                    g.DrawString(txtAge, font, this.TextBrush, textBoxRect, fmt);
                    textBoxRect.Y += size.Height;
                }
                for (int i = 0; i < olv.Columns.Count; i++)
                {
                    OLVColumn column = olv.GetColumn(i);
                    if (column.IsTileViewColumn && column.Text.Replace(" ", "").ToLower() != "startage" && column.Text.Replace(" ", "").ToLower() != "endage")
                    {
                        txt = column.GetStringValue(rowObject);
                        g.DrawString(txt, font, this.TextBrush, textBoxRect, fmt);
                        textBoxRect.Y += size.Height;
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRect(RectangleF rect, float diameter)
        {
            GraphicsPath path = new GraphicsPath();

            RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
