using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace WinControls
{
    internal class SavePicture
    {
        //RSMPlotControl _rsmPlotControl = null;
        ///// <summary>
        ///// 保存成图形文件
        ///// </summary>
        ///// <param name="rsmPlotControl">RSMPlotControl对象,从中画图</param>
        //public SavePicture(RSMPlotControl rsmPlotControl)
        //{
        //    _rsmPlotControl = rsmPlotControl;
        //}

        ColorBlendControl _colorBlendControl = null;

        /// <summary>
        /// 保存成Legend文件
        /// </summary>
        /// <param name="rsmPlotControl">ColorBlendBontrol对象,从中画图</param>
        public SavePicture(ColorBlendControl colorBlendControl)
        {
            _colorBlendControl = colorBlendControl;
        }

        //Revision: JCarpenter 10/06
        /// <summary>
        /// This private field contains the instance for the MasterPane object of this control.
        /// You can access the MasterPane object through the public property
        /// <see cref="ZedGraphControl.MasterPane"/>. This is nulled when this Control is
        /// disposed.
        /// </summary>
        //private MasterPane _masterPane;

        /// <summary>
        /// private field that determines whether or not tooltips will be displayed
        /// when the mouse hovers over data values.  Use the public property
        /// <see cref="IsShowPointValues"/> to access this value.
        /// </summary>
        private bool _isShowPointValues = false;
        /// <summary>
        /// private field that determines whether or not tooltips will be displayed
        /// showing the scale values while the mouse is located within the ChartRect.
        /// Use the public property <see cref="IsShowCursorValues"/> to access this value.
        /// </summary>
        private bool _isShowCursorValues = false;
        /// <summary>
        /// private field that determines the format for displaying tooltip values.
        /// This format is passed to <see cref="PointPairBase.ToString(string)"/>.
        /// Use the public property <see cref="PointValueFormat"/> to access this
        /// value.
        /// </summary>
        private string _pointValueFormat = PointPair.DefaultFormat;

        /// <summary>
        /// private field that determines whether or not the context menu will be available.  Use the
        /// public property <see cref="IsShowContextMenu"/> to access this value.
        /// </summary>
        private bool _isShowContextMenu = true;

        private SaveFileDialog _saveFileDialog = new SaveFileDialog();

        public SaveFileDialog MySaveFileDialog
        {
            get
            { return _saveFileDialog; }
            set
            { _saveFileDialog = value; }
        }

        private ResourceManager _resourceManager;

        ////
        ///// <summary>
        ///// Setup for creation of a new image, applying appropriate anti-alias properties and
        ///// returning the resultant image file
        ///// </summary>
        ///// <returns></returns>
        //public Image ImageRender()
        //{
        //    // return _masterPane.GetImage(_masterPane.IsAntiAlias);
        //    //return _rsmPlotControl.DrawImage();
        //    return _rsmPlotControl.GetFixedPlot();
        //}

        /// <summary>
        /// Setup for creation of a new image, applying appropriate anti-alias properties and
        /// returning the resultant image file
        /// </summary>
        /// <returns></returns>
        private Image ImageLegend()
        {
            //return _masterPane.GetImage(_masterPane.IsAntiAlias);
            //return _rsmPlotControl.DrawImage();
            return _colorBlendControl.DrawImage();
        }

        ///// <summary>
        ///// Build a <see cref="Bitmap"/> object containing the graphical rendering of
        ///// all the <see cref="GraphPane"/> objects in this list.
        ///// </summary>
        ///// <value>A <see cref="Bitmap"/> object rendered with the current graph.</value>
        ///// <seealso cref="GetImage(int,int,float)"/>
        ///// <seealso cref="GetMetafile()"/>
        ///// <seealso cref="GetMetafile(int,int)"/>
        //private  Bitmap GetImage(bool isAntiAlias)
        //{
        //    Bitmap bitmap = new Bitmap((int)_rect.Width, (int)_rect.Height);
        //    using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
        //    {
        //        bitmapGraphics.TranslateTransform(-_rect.Left, -_rect.Top);
        //        this.Draw(bitmapGraphics);
        //    }

        //    return bitmap;
        //}

        private Graphics CreateGraphics()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Handler for the "Save Image As" context menu item.  Copies the current image to the selected
        /// file in either the Emf (vector), or a variety of Bitmap formats.
        /// </summary>
        /// <remarks>
        /// Note that <see cref="SaveAsBitmap" /> and <see cref="SaveAsEmf" /> methods are provided
        /// which allow for Bitmap-only or Emf-only handling of the "Save As" context menu item.
        /// </remarks>
        public void SaveAs()
        {
            SaveAs(null);
        }

        /// <summary>
        /// Copies the current image to the selected file in
        /// Emf (vector), or a variety of Bitmap formats.
        /// </summary>
        /// <param name="DefaultFileName">
        /// Accepts a default file name for the file dialog (if "" or null, default is not used)
        /// </param>
        /// <returns>
        /// The file name saved, or "" if cancelled.
        /// </returns>
        /// <remarks>
        /// Note that <see cref="SaveAsBitmap" /> and <see cref="SaveAsEmf" /> methods are provided
        /// which allow for Bitmap-only or Emf-only handling of the "Save As" context menu item.
        /// </remarks>
        public String SaveAs(String DefaultFileName)
        {
            _saveFileDialog.Filter =
                //"Emf Format (*.emf)|*.emf|" +
                "Jpeg Format (*.jpg)|*.jpg|" +
                "PNG Format (*.png)|*.png|" +
                "Gif Format (*.gif)|*.gif|" +
                 "Tiff Format (*.tif)|*.tif|" +
                "Bmp Format (*.bmp)|*.bmp";

            if (DefaultFileName != null && DefaultFileName.Length > 0)
            {
                //  获得扩展名
                String ext = System.IO.Path.GetExtension(DefaultFileName).ToLower();
                switch (ext)
                {
                    //case ".emf": _saveFileDialog.FilterIndex = 1; break;
                    case ".png": _saveFileDialog.FilterIndex = 2; break;
                    case ".gif": _saveFileDialog.FilterIndex = 3; break;
                    case ".jpeg": _saveFileDialog.FilterIndex = 1; break;
                    case ".jpg": _saveFileDialog.FilterIndex = 1; break;
                    case ".tiff": _saveFileDialog.FilterIndex = 4; break;
                    case ".tif": _saveFileDialog.FilterIndex = 4; break;
                    case ".bmp": _saveFileDialog.FilterIndex = 5; break;
                }
                //If we were passed a file name, not just an extension, use it
                if (DefaultFileName.Length > ext.Length)
                {
                    _saveFileDialog.FileName = DefaultFileName;
                }
            }

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = _saveFileDialog.OpenFile();
                //// 20090902 @陈志润
                //MemoryStream myStream = _saveFileDialog.OpenFile();
                if (myStream != null)
                {
                    ImageFormat format = ImageFormat.Png;
                    switch (_saveFileDialog.FilterIndex)
                    {
                        case 1: format = ImageFormat.Jpeg; break;
                        case 2: format = ImageFormat.Png; break;
                        case 3: format = ImageFormat.Gif; break;
                        case 4: format = ImageFormat.Tiff; break;
                        case 5: format = ImageFormat.Bmp; break;
                    }
                    //ImageRender().Save(myStream, format);
                    myStream.Close();
                    return _saveFileDialog.FileName;
                }
            }
            return "";
        }

        /// <summary>
        /// Handler for the "Save Image As" context menu item.  Copies the current image to the selected
        /// file in either the Emf (vector), or a variety of Bitmap formats.
        /// </summary>
        /// <remarks>
        /// Note that <see cref="SaveAsBitmap" /> and <see cref="SaveAsEmf" /> methods are provided
        /// which allow for Bitmap-only or Emf-only handling of the "Save As" context menu item.
        /// </remarks>
        public void SaveAs(bool isLegend)
        {
            SaveAs(null, isLegend);
        }

        /// <summary>
        /// Copies the current image to the selected file in
        /// Emf (vector), or a variety of Bitmap formats.
        /// </summary>
        /// <param name="DefaultFileName">
        /// Accepts a default file name for the file dialog (if "" or null, default is not used)
        /// </param>
        /// <returns>
        /// The file name saved, or "" if cancelled.
        /// </returns>
        /// <remarks>
        /// Note that <see cref="SaveAsBitmap" /> and <see cref="SaveAsEmf" /> methods are provided
        /// which allow for Bitmap-only or Emf-only handling of the "Save As" context menu item.
        /// </remarks>
        public String SaveAs(String DefaultFileName, bool isLegend)
        {
            _saveFileDialog.Filter =
                //"Emf Format (*.emf)|*.emf|" +
                "Jpeg Format (*.jpg)|*.jpg|" +
                "PNG Format (*.png)|*.png|" +
                "Gif Format (*.gif)|*.gif|" +
                 "Tiff Format (*.tif)|*.tif|" +
                "Bmp Format (*.bmp)|*.bmp";

            if (DefaultFileName != null && DefaultFileName.Length > 0)
            {
                String ext = System.IO.Path.GetExtension(DefaultFileName).ToLower();
                switch (ext)
                {
                    //case ".emf": _saveFileDialog.FilterIndex = 1; break;
                    case ".jpeg": _saveFileDialog.FilterIndex = 1; break;
                    case ".jpg": _saveFileDialog.FilterIndex = 1; break;
                    case ".png": _saveFileDialog.FilterIndex = 2; break;
                    case ".gif": _saveFileDialog.FilterIndex = 3; break;
                    case ".tiff": _saveFileDialog.FilterIndex = 4; break;
                    case ".tif": _saveFileDialog.FilterIndex = 4; break;
                    case ".bmp": _saveFileDialog.FilterIndex = 5; break;
                }
                //If we were passed a file name, not just an extension, use it
                if (DefaultFileName.Length > ext.Length)
                {
                    _saveFileDialog.FileName = DefaultFileName;
                }
            }

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = _saveFileDialog.OpenFile();
                if (myStream != null)
                {
                    //if (_saveFileDialog.FilterIndex == 1)
                    //{
                    //    myStream.Close();
                    //    //SaveEmfFile(_saveFileDialog.FileName);
                    //    SaveEmfLegendFile(_saveFileDialog.FileName);
                    //}
                    //else
                    //{
                    ImageFormat format = ImageFormat.Png;
                    switch (_saveFileDialog.FilterIndex)
                    {
                        case 1: format = ImageFormat.Jpeg; break;
                        case 2: format = ImageFormat.Png; break;
                        case 3: format = ImageFormat.Gif; break;
                        case 4: format = ImageFormat.Tiff; break;
                        case 5: format = ImageFormat.Bmp; break;
                    }

                    //ImageRender().Save(myStream, format);
                    //_masterPane.GetImage().Save( myStream, format );
                    ImageLegend().Save(myStream, format);
                    myStream.Close();
                    //}
                    return _saveFileDialog.FileName;
                }
            }
            return "";
        }

        /// <summary>
        /// Handler for the "Save Image As" context menu item.  Copies the current image to the selected
        /// Bitmap file.
        /// </summary>
        /// <remarks>
        /// Note that this handler saves as a bitmap only.  The default handler is
        /// <see cref="SaveAs()" />, which allows for Bitmap or EMF formats
        /// </remarks>
        public void SaveAsBitmap()
        {
            _saveFileDialog.Filter =
                "PNG Format (*.png)|*.png|" +
                "Gif Format (*.gif)|*.gif|" +
                "Jpeg Format (*.jpg)|*.jpg|" +
                "Tiff Format (*.tif)|*.tif|" +
                "Bmp Format (*.bmp)|*.bmp";

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageFormat format = ImageFormat.Png;
                if (_saveFileDialog.FilterIndex == 2)
                    format = ImageFormat.Gif;
                else if (_saveFileDialog.FilterIndex == 3)
                    format = ImageFormat.Jpeg;
                else if (_saveFileDialog.FilterIndex == 4)
                    format = ImageFormat.Tiff;
                else if (_saveFileDialog.FilterIndex == 5)
                    format = ImageFormat.Bmp;

                Stream myStream = _saveFileDialog.OpenFile();
                if (myStream != null)
                {
                    ////_masterPane.GetImage().Save( myStream, format );
                    //ImageRender().Save(myStream, format);
                    //ImageLegend().Save(myStream,format);
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// 保存Plot和ColorBlend
        /// Copies the current image to the selected file in
        /// Emf (vector), or a variety of Bitmap formats.
        /// </summary>
        /// <param name="DefaultFileName">
        /// Accepts a default file name for the file dialog (if "" or null, default is not used)
        /// </param>
        /// <returns>
        /// The file name saved, or "" if cancelled.
        /// </returns>
        /// <remarks>
        /// Note that <see cref="SaveAsBitmap" /> and <see cref="SaveAsEmf" /> methods are provided
        /// which allow for Bitmap-only or Emf-only handling of the "Save As" context menu item.
        /// </remarks>
        public String SaveAs(String DefaultFileName, ColorBlendControl _colorBlendControl)
        {
            _saveFileDialog.Filter =
                //"Emf Format (*.emf)|*.emf|" +
                "Jpeg Format (*.jpg)|*.jpg|" +
                "PNG Format (*.png)|*.png|" +
                "Gif Format (*.gif)|*.gif|" +
                 "Tiff Format (*.tif)|*.tif|" +
                "Bmp Format (*.bmp)|*.bmp";

            if (DefaultFileName != null && DefaultFileName.Length > 0)
            {
                //  获得扩展名
                String ext = System.IO.Path.GetExtension(DefaultFileName).ToLower();
                switch (ext)
                {
                    //case ".emf": _saveFileDialog.FilterIndex = 1; break;
                    case ".png": _saveFileDialog.FilterIndex = 2; break;
                    case ".gif": _saveFileDialog.FilterIndex = 3; break;
                    case ".jpeg": _saveFileDialog.FilterIndex = 1; break;
                    case ".jpg": _saveFileDialog.FilterIndex = 1; break;
                    case ".tiff": _saveFileDialog.FilterIndex = 4; break;
                    case ".tif": _saveFileDialog.FilterIndex = 4; break;
                    case ".bmp": _saveFileDialog.FilterIndex = 5; break;
                }
                //If we were passed a file name, not just an extension, use it
                if (DefaultFileName.Length > ext.Length)
                {
                    _saveFileDialog.FileName = DefaultFileName;
                }
            }

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = _saveFileDialog.OpenFile();
                //// 20090902 @陈志润
                //MemoryStream myStream = _saveFileDialog.OpenFile();
                if (myStream != null)
                {
                    ImageFormat format = ImageFormat.Png;
                    switch (_saveFileDialog.FilterIndex)
                    {
                        case 1: format = ImageFormat.Jpeg; break;
                        case 2: format = ImageFormat.Png; break;
                        case 3: format = ImageFormat.Gif; break;
                        case 4: format = ImageFormat.Tiff; break;
                        case 5: format = ImageFormat.Bmp; break;
                    }

                    //Bitmap _bmpRsm = _rsmPlotControl.GetFixedPlot();// _rsmPlotControl.DrawImage();
                    long a = myStream.Length;
                    Bitmap _bmpBlend = _colorBlendControl.DrawImage();
                    a = myStream.Length;
                    //WinAPIuse useWinAPI = new WinAPIuse();
                    //useWinAPI

                    //Image image = null;

                    //_bmpRsm.Save(myStream, format);
                    //a = myStream.Length;
                    //_bmpBlend.Save(myStream, format);
                    //a = myStream.Length;
                    //myStream.Flush();

                    #region 在rsmPlot上画blend

                    //using (Graphics g = Graphics.FromImage(_bmpRsm))
                    //{
                    //    //g.DrawString(s, SystemFonts.DefaultFont, Brushes.Black, 0, 0);
                    //    float xAxis = g.DpiX + 100;
                    //    float yAxis = g.DpiY + 250;
                    //    g.DrawImage(_bmpBlend, xAxis, yAxis, 300, 50);
                    //    g.Save();
                    //}

                    #endregion 在rsmPlot上画blend

                    myStream.Close();
                    //}
                    return _saveFileDialog.FileName;
                }
            }
            return "";
        }

        //[System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        //private  static extern bool BitBlt(
        //    IntPtr hdcDest, // handle to destination DC
        //    int nXDest, // x-coord of destination upper-left corner
        //    int nYDest, // y-coord of destination upper-left corner
        //    int nWidth, // width of destination rectangle
        //    int nHeight, // height of destination rectangle
        //    IntPtr hdcSrc, // handle to source DC
        //    int nXSrc, // x-coordinate of source upper-left corner
        //    int nYSrc, // y-coordinate of source upper-left corner
        //    System.Int32 dwRop // raster operation code
        //);

        /// <summary>
        /// Handler for the "Save Image As" context menu item.  Copies the current image to the selected
        /// Emf format file.
        /// </summary>
        /// <remarks>
        /// Note that this handler saves as an Emf format only.  The default handler is
        /// <see cref="SaveAs()" />, which allows for Bitmap or EMF formats.
        /// </remarks>
        public void SaveAsEmf()
        {
            _saveFileDialog.Filter = "Emf Format (*.jpg)|*.jpg";

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = _saveFileDialog.OpenFile();
                if (myStream != null)
                {
                    myStream.Close();
                    //_masterPane.GetMetafile().Save( _saveFileDialog.FileName );
                    //SaveEmfFile(_saveFileDialog.FileName);
                }
            }
        }

        ///// <summary>
        ///// Save the current Graph to the specified filename in EMF (vector) format.
        ///// See <see cref="SaveAsEmf()" /> for public access.
        ///// </summary>
        ///// <remarks>
        ///// Note that this handler saves as an Emf format only.  The default handler is
        ///// <see cref="SaveAs()" />, which allows for Bitmap or EMF formats.
        ///// </remarks>
        //internal void SaveEmfFile(string fileName)
        //{
        //    using (Graphics g = _rsmPlotControl.CreateGraphics())// this.CreateGraphics())
        //    {
        //        IntPtr hdc = g.GetHdc();
        //        Metafile metaFile = new Metafile(hdc, EmfType.EmfPlusOnly);
        //        using (Graphics gMeta = Graphics.FromImage(metaFile))
        //        {
        //            ////PaneBase.SetAntiAliasMode( gMeta, IsAntiAlias );
        //            ////gMeta.CompositingMode = CompositingMode.SourceCopy;
        //            ////gMeta.CompositingQuality = CompositingQuality.HighQuality;
        //            ////gMeta.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //            ////gMeta.SmoothingMode = SmoothingMode.AntiAlias;
        //            ////gMeta.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //            //this._masterPane.Draw(gMeta);
        //            ////gMeta.Dispose();
        //            _rsmPlotControl.DrawImageToGraphics(gMeta);
        //        }

        //        ClipboardMetafileHelper.SaveEnhMetafileToFile(metaFile, fileName);

        //        g.ReleaseHdc(hdc);
        //        //g.Dispose();
        //    }
        //}

        ////
        //private Bitmap _bmp = null;
        //public Bitmap Bmp
        //{
        //    get { return _bmp; }
        //}

        //private Rectangle _rect;
        //public Rectangle Rect
        //{
        //    get { return _rect; }
        //}

        /// <summary>
        /// Save the current Graph to the specified filename in EMF (vector) format.
        /// See <see cref="SaveAsEmf()" /> for public access.
        /// </summary>
        /// <remarks>
        /// Note that this handler saves as an Emf format only.  The default handler is
        /// <see cref="SaveAs()" />, which allows for Bitmap or EMF formats.
        /// </remarks>
        internal void SaveEmfLegendFile(string fileName)
        {
            using (Graphics g = _colorBlendControl.CreateGraphics())// this.CreateGraphics())
            {
                IntPtr hdc = g.GetHdc();
                Metafile metaFile = new Metafile(hdc, EmfType.EmfPlusOnly);
                // PaintEventArgs e = new PaintEventArgs(_colorBlendControl);
                //Bitmap _bmp = g.GetNearestColor(e);
                using (Graphics gMeta = Graphics.FromImage(metaFile))
                {
                    ////PaneBase.SetAntiAliasMode( gMeta, IsAntiAlias );
                    ////gMeta.CompositingMode = CompositingMode.SourceCopy;
                    ////gMeta.CompositingQuality = CompositingQuality.HighQuality;
                    ////gMeta.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    ////gMeta.SmoothingMode = SmoothingMode.AntiAlias;
                    ////gMeta.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //this._masterPane.Draw(gMeta);
                    ////gMeta.Dispose();
                    //_rsmPlotControl.DrawImageToGraphics(gMeta);
                    // _colorBlendControl.DrawToBitmap(Bmp,Rect);
                    _colorBlendControl.DrawImage();
                }

                ClipboardMetafileHelper.SaveEnhMetafileToFile(metaFile, fileName);

                g.ReleaseHdc(hdc);
                //g.Dispose();
            }
        }

        internal class ClipboardMetafileHelper
        {
            [DllImport("user32.dll")]
            private static extern bool OpenClipboard(IntPtr hWndNewOwner);

            [DllImport("user32.dll")]
            private static extern bool EmptyClipboard();

            [DllImport("user32.dll")]
            private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

            [DllImport("user32.dll")]
            private static extern bool CloseClipboard();

            [DllImport("gdi32.dll")]
            private static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, System.Text.StringBuilder hNULL);

            [DllImport("gdi32.dll")]
            private static extern bool DeleteEnhMetaFile(IntPtr hemf);

            static internal bool SaveEnhMetafileToFile(Metafile mf, string fileName)
            {
                bool bResult = false;
                IntPtr hEMF;
                hEMF = mf.GetHenhmetafile(); // invalidates mf
                if (!hEMF.Equals(new IntPtr(0)))
                {
                    StringBuilder tempName = new StringBuilder(fileName);
                    CopyEnhMetaFile(hEMF, tempName);
                    DeleteEnhMetaFile(hEMF);
                }
                return bResult;
            }

            static internal bool SaveEnhMetafileToFile(Metafile mf)
            {
                bool bResult = false;
                IntPtr hEMF;
                hEMF = mf.GetHenhmetafile(); // invalidates mf
                if (!hEMF.Equals(new IntPtr(0)))
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Extended Metafile (*.emf)|*.emf";
                    sfd.DefaultExt = ".emf";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        StringBuilder temp = new StringBuilder(sfd.FileName);
                        CopyEnhMetaFile(hEMF, temp);
                    }
                    DeleteEnhMetaFile(hEMF);
                }
                return bResult;
            }

            // Metafile mf is set to a state that is not valid inside this function.
            static internal bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile mf)
            {
                bool bResult = false;
                IntPtr hEMF, hEMF2;
                hEMF = mf.GetHenhmetafile(); // invalidates mf
                if (!hEMF.Equals(new IntPtr(0)))
                {
                    hEMF2 = CopyEnhMetaFile(hEMF, null);
                    if (!hEMF2.Equals(new IntPtr(0)))
                    {
                        if (OpenClipboard(hWnd))
                        {
                            if (EmptyClipboard())
                            {
                                IntPtr hRes = SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
                                bResult = hRes.Equals(hEMF2);
                                CloseClipboard();
                            }
                        }
                    }
                    DeleteEnhMetaFile(hEMF);
                }
                return bResult;
            }
        }
    }//class
}//namespace