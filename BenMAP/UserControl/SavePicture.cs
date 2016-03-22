using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace WinControls
{
    internal class SavePicture
    {
        ColorBlendControl _colorBlendControl = null;

        public SavePicture(ColorBlendControl colorBlendControl)
        {
            _colorBlendControl = colorBlendControl;
        }

        private bool _isShowPointValues = false;
        private bool _isShowCursorValues = false;
    
     // requires zedgraph: this variable is also unused...? -AS 
    //    private string _pointValueFormat = PointPair.DefaultFormat;

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


        private Image ImageLegend()
        {
            return _colorBlendControl.DrawImage();
        }



        private Graphics CreateGraphics()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SaveAs()
        {
            SaveAs(null);
        }

        public String SaveAs(String DefaultFileName)
        {
            _saveFileDialog.Filter =
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
                    case ".png": _saveFileDialog.FilterIndex = 2; break;
                    case ".gif": _saveFileDialog.FilterIndex = 3; break;
                    case ".jpeg": _saveFileDialog.FilterIndex = 1; break;
                    case ".jpg": _saveFileDialog.FilterIndex = 1; break;
                    case ".tiff": _saveFileDialog.FilterIndex = 4; break;
                    case ".tif": _saveFileDialog.FilterIndex = 4; break;
                    case ".bmp": _saveFileDialog.FilterIndex = 5; break;
                }
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
                    ImageFormat format = ImageFormat.Png;
                    switch (_saveFileDialog.FilterIndex)
                    {
                        case 1: format = ImageFormat.Jpeg; break;
                        case 2: format = ImageFormat.Png; break;
                        case 3: format = ImageFormat.Gif; break;
                        case 4: format = ImageFormat.Tiff; break;
                        case 5: format = ImageFormat.Bmp; break;
                    }
                    myStream.Close();
                    return _saveFileDialog.FileName;
                }
            }
            return "";
        }

        public void SaveAs(bool isLegend)
        {
            SaveAs(null, isLegend);
        }

        public String SaveAs(String DefaultFileName, bool isLegend)
        {
            _saveFileDialog.Filter =
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
                    case ".jpeg": _saveFileDialog.FilterIndex = 1; break;
                    case ".jpg": _saveFileDialog.FilterIndex = 1; break;
                    case ".png": _saveFileDialog.FilterIndex = 2; break;
                    case ".gif": _saveFileDialog.FilterIndex = 3; break;
                    case ".tiff": _saveFileDialog.FilterIndex = 4; break;
                    case ".tif": _saveFileDialog.FilterIndex = 4; break;
                    case ".bmp": _saveFileDialog.FilterIndex = 5; break;
                }
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
                    ImageFormat format = ImageFormat.Png;
                    switch (_saveFileDialog.FilterIndex)
                    {
                        case 1: format = ImageFormat.Jpeg; break;
                        case 2: format = ImageFormat.Png; break;
                        case 3: format = ImageFormat.Gif; break;
                        case 4: format = ImageFormat.Tiff; break;
                        case 5: format = ImageFormat.Bmp; break;
                    }

                    ImageLegend().Save(myStream, format);
                    myStream.Close();
                    return _saveFileDialog.FileName;
                }
            }
            return "";
        }

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
                    myStream.Close();
                }
            }
        }

        public String SaveAs(String DefaultFileName, ColorBlendControl _colorBlendControl)
        {
            _saveFileDialog.Filter =
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
                    case ".png": _saveFileDialog.FilterIndex = 2; break;
                    case ".gif": _saveFileDialog.FilterIndex = 3; break;
                    case ".jpeg": _saveFileDialog.FilterIndex = 1; break;
                    case ".jpg": _saveFileDialog.FilterIndex = 1; break;
                    case ".tiff": _saveFileDialog.FilterIndex = 4; break;
                    case ".tif": _saveFileDialog.FilterIndex = 4; break;
                    case ".bmp": _saveFileDialog.FilterIndex = 5; break;
                }
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
                    ImageFormat format = ImageFormat.Png;
                    switch (_saveFileDialog.FilterIndex)
                    {
                        case 1: format = ImageFormat.Jpeg; break;
                        case 2: format = ImageFormat.Png; break;
                        case 3: format = ImageFormat.Gif; break;
                        case 4: format = ImageFormat.Tiff; break;
                        case 5: format = ImageFormat.Bmp; break;
                    }

                    long a = myStream.Length;
                    Bitmap _bmpBlend = _colorBlendControl.DrawImage();
                    a = myStream.Length;






                    myStream.Close();
                    return _saveFileDialog.FileName;
                }
            }
            return "";
        }


        public void SaveAsEmf()
        {
            _saveFileDialog.Filter = "Emf Format (*.jpg)|*.jpg";

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = _saveFileDialog.OpenFile();
                if (myStream != null)
                {
                    myStream.Close();
                }
            }
        }






        internal void SaveEmfLegendFile(string fileName)
        {
            using (Graphics g = _colorBlendControl.CreateGraphics())
            {
                IntPtr hdc = g.GetHdc();
                Metafile metaFile = new Metafile(hdc, EmfType.EmfPlusOnly);
                using (Graphics gMeta = Graphics.FromImage(metaFile))
                {
                    _colorBlendControl.DrawImage();
                }

                ClipboardMetafileHelper.SaveEnhMetafileToFile(metaFile, fileName);

                g.ReleaseHdc(hdc);
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
                hEMF = mf.GetHenhmetafile(); if (!hEMF.Equals(new IntPtr(0)))
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
                hEMF = mf.GetHenhmetafile(); if (!hEMF.Equals(new IntPtr(0)))
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

            static internal bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile mf)
            {
                bool bResult = false;
                IntPtr hEMF, hEMF2;
                hEMF = mf.GetHenhmetafile(); if (!hEMF.Equals(new IntPtr(0)))
                {
                    hEMF2 = CopyEnhMetaFile(hEMF, null);
                    if (!hEMF2.Equals(new IntPtr(0)))
                    {
                        if (OpenClipboard(hWnd))
                        {
                            if (EmptyClipboard())
                            {
                                IntPtr hRes = SetClipboardData(14, hEMF2);
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
    }
}