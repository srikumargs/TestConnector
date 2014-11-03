using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Sage.CRE.Core.LinkedSource;

namespace ConnectorServiceMonitor.Internal
{
    internal sealed class ImageManager : IDisposable
    {
        public ImageManager()
        {
            //the 24x24 images are used on the General and Connection pages of the Monitor
            //ID needed more pixels to be able to represent the ideas
            _imageList24 = new ImageList();
            _imageList24.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            _imageList24.TransparentColor = System.Drawing.Color.Magenta;
            _imageList24.ImageSize = new Size(24, 24);

            _imageList24.Images.Add(ImageResources.OK);
            _imageList24.Images.Add(ImageResources.Critical);
            _imageList24.Images.Add(ImageResources.Serious);
            _imageList24.Images.Add(ImageResources.Blank);
            _imageList24.Images.Add(ImageResources.Running);
            _imageList24.Images.Add(ImageResources.Repeat);
            _imageList24.Images.Add(ImageResources.Fixing);
            _imageList24.Images.Add(ImageResources.OKWithFixes);
            _imageList24.Images.Add(ImageResources.Alert);
            _imageList24.Images.Add(ImageResources.greenLight);
            _imageList24.Images.Add(ImageResources.redLight); 
            _imageList24.Images.Add(ImageResources.whiteLight);
            _imageList24.Images.Add(ImageResources.greenLightWorking);
            _imageList24.Images.Add(ImageResources.redLightWorking);
            _imageList24.Images.Add(ImageResources.whiteLightWorking);
            _imageList24.Images.Add(ImageResources.greenLightLeft);
            _imageList24.Images.Add(ImageResources.redLightLeft);
            _imageList24.Images.Add(ImageResources.whiteLightLeft);
            _imageList24.Images.Add(ImageResources.refresh_icon);
            _okBitmapFileName = WriteResourceToTempFile(ImageResources.OK);
            _criticalBitmapFileName = WriteResourceToTempFile(ImageResources.Critical);
            _seriousBitmapFileName = WriteResourceToTempFile(ImageResources.Serious);
            _blankBitmapFileName = WriteResourceToTempFile(ImageResources.Blank);
            _runningBitmapFileName = WriteResourceToTempFile(ImageResources.Running);
            _repeatBitmapFileName = WriteResourceToTempFile(ImageResources.Repeat); //Not in use
            _fixingBitmapFileName = WriteResourceToTempFile(ImageResources.Fixing);
            _okWithFixesBitmapFileName = WriteResourceToTempFile(ImageResources.OKWithFixes); //Not in use
            _alertBitmapFileName = WriteResourceToTempFile(ImageResources.Alert);
            _greenLightFileName = WriteResourceToTempFile(ImageResources.greenLight);
            _redLightFileName = WriteResourceToTempFile(ImageResources.redLight);
            _whiteLightFileName = WriteResourceToTempFile(ImageResources.whiteLight);
            _greenLightWorkingFileName = WriteResourceToTempFile(ImageResources.greenLightWorking);
            _redLightWorkingFileName = WriteResourceToTempFile(ImageResources.redLightWorking);
            _whiteLightWorkingFileName = WriteResourceToTempFile(ImageResources.whiteLightWorking);
            _greenLightWorkingRightSideFileName = WriteResourceToTempFile(ImageResources.greenLightLeft);
            _redLightWorkingRightSideFileName = WriteResourceToTempFile(ImageResources.redLightLeft);
            _whiteLightWorkingRightSideFileName = WriteResourceToTempFile(ImageResources.whiteLightLeft);

            _imageList32x16 = new ImageList();
            _imageList32x16.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            _imageList32x16.ImageSize = new Size(32, 16);

            _arrowFileName = WriteResourceToTempFile(ImageResources.arrow32x16);
            _arrowBrokenFileName = WriteResourceToTempFile(ImageResources.arrows_icon_red);

            //the 16x16 images are used on the Last page of the Monitor for requests
            //These are just the old 16x16 size.
            _imageList16 = new ImageList();
            _imageList16.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            _imageList16.TransparentColor = System.Drawing.Color.Magenta;
            _imageList16.ImageSize = new Size(16, 16);

            _imageList16.Images.Add(ImageResources.Blank16);
            _imageList16.Images.Add(ImageResources.Running16);
            _imageList16.Images.Add(ImageResources.OK16);
            _imageList16.Images.Add(ImageResources.Critical16);
            _imageList16.Images.Add(ImageResources.UserCancelled16);

            _blank16ImageFileName = WriteResourceToTempFile(ImageResources.Blank16);
            _running16ImageFileName = WriteResourceToTempFile(ImageResources.Running16);
            _ok16ImageFileName = WriteResourceToTempFile(ImageResources.OK16);
            _critical16ImageFileName = WriteResourceToTempFile(ImageResources.Critical16);
            _cancel16ImageFileName = WriteResourceToTempFile(ImageResources.UserCancelled16);

            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                _uacShield = StockIcons.GetImage(StockIconIdentifier.Shield, StockIconOptions.Small);
                _uacShieldFileName = WriteResourceToTempFile(_uacShield);
            }
        }

        ~ImageManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Standard IDisposable implementation
        /// </summary>
        /// <param name="disposing">true if this object is being disposed; false if it is being finalized</param>
        private void Dispose(Boolean disposing)
        {
            // If disposing equals true, dispose all managed and unmanaged resources ... the
            // method has been called directly (or indirectly) by user's code.
            //
            // If disposing equals false, then the method has been called by the runtime from
            // inside the finalizer and so, no other managed objects should be referenced.  Only
            // unmanaged resources can be disposed.
            if (disposing)
            {
                // Dispose managed resources here.
                _imageList32x16.Dispose();
                _imageList32x16 = null;

                _imageList24.Dispose();
                _imageList24 = null;

                _imageList16.Dispose();
                _imageList16 = null;
            }

            // Dispose unmanaged resources here.
            DisposeResource(ref _okBitmapFileName);
            DisposeResource(ref _criticalBitmapFileName);
            DisposeResource(ref _seriousBitmapFileName);
            DisposeResource(ref _blankBitmapFileName);
            DisposeResource(ref _runningBitmapFileName);
            DisposeResource(ref _repeatBitmapFileName);
            DisposeResource(ref _fixingBitmapFileName);
            DisposeResource(ref _okWithFixesBitmapFileName);
            DisposeResource(ref _alertBitmapFileName);
            DisposeResource(ref _greenLightFileName);
            DisposeResource(ref _redLightFileName);
            DisposeResource(ref _whiteLightFileName);
            DisposeResource(ref _greenLightWorkingFileName);
            DisposeResource(ref _redLightWorkingFileName);
            DisposeResource(ref _whiteLightWorkingFileName);
            DisposeResource(ref _greenLightWorkingRightSideFileName);
            DisposeResource(ref _redLightWorkingRightSideFileName);
            DisposeResource(ref _whiteLightWorkingRightSideFileName);

            DisposeResource(ref _uacShieldFileName);

            DisposeResource(ref _arrowFileName);
            DisposeResource(ref _arrowBrokenFileName);
            DisposeResource(ref _blank16ImageFileName);
            DisposeResource(ref _running16ImageFileName);
            DisposeResource(ref _ok16ImageFileName);
            DisposeResource(ref _critical16ImageFileName);
            DisposeResource(ref _cancel16ImageFileName);
        }

        private void DisposeResource(ref string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                fileName = null;
            }
        }

        public Image OK
        {
            get { return _imageList24.Images[0]; }
        }

        public String OKBitmapFileName
        {
            get { return _okBitmapFileName; }
        }

        public Image Critical
        {
            get { return _imageList24.Images[1]; }
        }

        public String CriticalBitmapFileName
        {
            get { return _criticalBitmapFileName; }
        }

        public Image Serious
        {
            get { return _imageList24.Images[2]; }
        }

        public String SeriousBitmapFileName
        {
            get { return _seriousBitmapFileName; }
        }

        public Image Blank
        {
            get { return _imageList24.Images[3]; }
        }

        public String BlankBitmapFileName
        {
            get { return _blankBitmapFileName; }
        }

        public Image Running
        {
            get { return _imageList24.Images[4]; }
        }

        public String RunningBitmapFileName
        {
            get { return _runningBitmapFileName; }
        }

        public Image Repeat
        {
            get { return _imageList24.Images[5]; }
        }

        public String RepeatBitmapFileName
        {
            get { return _repeatBitmapFileName; }
        }

        public Image Fixing
        {
            get { return _imageList24.Images[6]; }
        }

        public String FixingBitmapFileName
        {
            get { return _fixingBitmapFileName; }
        }

        public Image OKWithFixes
        {
            get { return _imageList24.Images[7]; }
        }

        public String OKWithFixesBitmapFileName
        {
            get { return _okWithFixesBitmapFileName; }
        }

        public Image Alert
        {
            get { return _imageList24.Images[8]; }
        }

        public String AlertBitmapFileName
        {
            get { return _alertBitmapFileName; }
        }

        public Image Arrow
        {
            get { return _imageList32x16.Images[0]; }
        }

        public String ArrowFileName
        {
            get { return _arrowFileName; }
        }
         public String ArrowBrokenFileName
        {
            get { return _arrowBrokenFileName; }
        }
         
        public Image GreenLight
        {
            get { return _imageList24.Images[9]; }
        }

        public String GreenLightFileName
        {
            get { return _greenLightFileName; }
        }

        public Image RedLight
        {
            get { return _imageList24.Images[10]; }
        }

        public String RedLightFileName
        {
            get { return _redLightFileName; }
        }

        public Image WhiteLight
        {
            get { return _imageList24.Images[11]; }
        }

        public String WhiteLightFileName
        {
            get { return _whiteLightFileName; }
        }

        public Image GreenLightWorking
        {
            get { return _imageList24.Images[12]; }
        }

        public String GreenLightWorkingFileName
        {
            get { return _greenLightWorkingFileName; }
        }

        public Image RedLightWorking
        {
            get { return _imageList24.Images[13]; }
        }

        public String RedLightWorkingFileName
        {
            get { return _redLightWorkingFileName; }
        }

        public Image WhiteLightWorking
        {
            get { return _imageList24.Images[14]; }
        }

        public String WhiteLightWorkingFileName
        {
            get { return _whiteLightWorkingFileName; }
        }

        public Image GreenLightWorkingRightSide
        {
            get { return _imageList24.Images[15]; }
        }

        public String GreenLightWorkingRightSideFileName
        {
            get { return _greenLightWorkingRightSideFileName; }
        }

        public Image RedLightWorkingRightSide
        {
            get { return _imageList24.Images[16]; }
        }

        public String RedLightWorkingRightSideFileName
        {
            get { return _redLightWorkingRightSideFileName; }
        }

        public Image WhiteLightWorkingRightSide
        {
            get { return _imageList24.Images[17]; }
        }

        public String WhiteLightWorkingRightSideFileName
        {
            get { return _whiteLightWorkingRightSideFileName; }
        }

        public Image UACShield
        {
            get { return _uacShield; }
        }

        public String UACShieldFileName
        {
            get { return _uacShieldFileName; }
        }

        public Image Blank16
        {
            get { return _imageList16.Images[0]; }
        }

        public String Blank16ImageFileName
        {
            get { return _blank16ImageFileName; }
        }

        public Image Running16
        {
            get { return _imageList16.Images[1]; }
        }

        public String Running16ImageFileName
        {
            get { return _running16ImageFileName; }
        }

        public Image OK16
        {
            get { return _imageList16.Images[2]; }
        }

        public String OK16ImageFileName
        {
            get { return _ok16ImageFileName; }
        }

        public Image Critical16
        {
            get { return _imageList16.Images[3]; }
        }

        public String Critical16ImageFileName
        {
            get { return _critical16ImageFileName; }
        }

        public Image Cancel16
        {
            get { return _imageList16.Images[4]; }
        }
        public Image RefreshIcon
        {
            get { return _imageList24.Images[18]; }
        }
        public String Cancel16ImageFileName
        {
            get { return _cancel16ImageFileName; }
        }


        
        //public Image ImageForSystemCheckInfo(SystemCheckInfo systemCheckInfo)
        //{
        //    Image result = Blank;

        //    ISystemCheckResult systemCheckResult = systemCheckInfo.SystemCheckResult;
        //    if (systemCheckResult != null)
        //    {
        //        switch (systemCheckResult.ResultCode)
        //        {
        //            case SystemCheckResultCode.Fail:
        //                result = Critical;
        //                break;

        //            case SystemCheckResultCode.Pass:
        //                if (!systemCheckInfo.FixesAttempted)
        //                {
        //                    result = OK;
        //                }
        //                else
        //                {
        //                    result = OKWithFixes;
        //                }
        //                break;

        //            default:
        //                result = Serious;
        //                break;
        //        }
        //    }

        //    return result;
        //}

        private static String WriteResourceToTempFile(Image resource)
        {
            String result = Path.GetTempFileName();

            using (Bitmap b = new Bitmap(resource))
            {

                for (Int32 y = 0; y < b.Height; y++)
                {
                    for (Int32 x = 0; x < b.Width; x++)
                    {
                        if (b.GetPixel(x, y).ToArgb() == _argbMagenta)
                        {
                            b.SetPixel(x, y, SystemColors.Window);
                        }
                    }
                }

                b.Save(result);
            }

            return result;
        }

        #region Private fields
        private Boolean _disposed;              //= false; (automatically initialized by runtime)
        private ImageList _imageList24;           //= null; (automatically initialized by runtime)
        private ImageList _imageList32x16;           //= null; (automatically initialized by runtime)
        private ImageList _imageList16;           //= null; (automatically initialized by runtime)

        private String _okBitmapFileName;       //= null; (automatically initialized by runtime)
        private String _criticalBitmapFileName; //= null; (automatically initialized by runtime)
        private String _seriousBitmapFileName;  //= null; (automatically initialized by runtime)
        private String _blankBitmapFileName;    //= null; (automatically initialized by runtime)
        private String _runningBitmapFileName;  //= null; (automatically initialized by runtime)
        private String _repeatBitmapFileName;   //= null; (automatically initialized by runtime)
        private String _fixingBitmapFileName;   //= null; (automatically initialized by runtime)
        private String _okWithFixesBitmapFileName;   //= null; (automatically initialized by runtime)
        private String _alertBitmapFileName;   //= null; (automatically initialized by runtime)
        private String _greenLightFileName; //= null; (automatically initialized by runtime)
        private String _redLightFileName; //= null; (automatically initialized by runtime)
        private String _whiteLightFileName; //= null; (automatically initialized by runtime)
        private String _greenLightWorkingFileName; //= null; (automatically initialized by runtime)
        private String _redLightWorkingFileName; //= null; (automatically initialized by runtime)
        private String _whiteLightWorkingFileName; //= null; (automatically initialized by runtime)
        private String _greenLightWorkingRightSideFileName; //= null; (automatically initialized by runtime)
        private String _redLightWorkingRightSideFileName; //= null; (automatically initialized by runtime)
        private String _whiteLightWorkingRightSideFileName; //= null; (automatically initialized by runtime)

        private String _arrowFileName; //= null; (automatically initialized by runtime)
        private String _arrowBrokenFileName;
        private Image _uacShield;               //= null; (automatically initialized by runtime)
        private String _uacShieldFileName;      //= null; (automatically initialized by runtime)

        private String _blank16ImageFileName;//= null; (automatically initialized by runtime)
        private String _running16ImageFileName;//= null; (automatically initialized by runtime)
        private String _ok16ImageFileName;//= null; (automatically initialized by runtime)
        private String _critical16ImageFileName;//= null; (automatically initialized by runtime)
        private String _cancel16ImageFileName;//= null; (automatically initialized by runtime)

        private static readonly Int32 _argbMagenta = Color.FromKnownColor(KnownColor.Magenta).ToArgb();
        #endregion
    }
}
