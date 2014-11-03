using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  - N/A
namespace Sage.CRE.Core.LinkedSource
{
    /// <summary>
    /// Flags that specify attributes of the desired stock icon.
    /// </summary>
    [Flags]
    public enum StockIconOptions : uint
    {
        /// <summary>
        /// The StockIconOptions value is invalid
        /// </summary>
        /// <remarks>
        /// This is default value the runtime automatically initializes any StockIconOptions instance to.
        /// </remarks>
        None = 0x00000000,

        /// <summary>
        /// Modifies the SHGSI_ICON value by causing the function to retrieve the small version of the icon,
        /// as specified by the SM_CXSMICON and SM_CYSMICON system metrics.  (a.k.a. SHGSI_SMALLICON)
        /// </summary>
        Small = 0x00000001,

        /// <summary>
        /// Modifies the SHGSI_LARGEICON or SHGSI_SMALLICON values by causing the function to retrieve the
        /// Shell-sized icons rather than the sizes specified by the system metrics.  (a.k.a. SHGSI_SHELLICONSIZE)
        /// </summary>
        ShellSize = 0x00000004,

        /// <summary>
        /// The hIcon member of the SHSTOCKICONINFO structure receives a handle to the specified icon.  (a.k.a. SHGSI_ICON)
        /// </summary>
        Handle = 0x00000100,

        /// <summary>
        /// The iSysImageImage member of the SHSTOCKICONINFO structure receives the index of the specified icon in the system imagelist.  (a.k.a. SHGSI_SYSICONINDEX)
        /// </summary>
        SystemIndex = 0x00004000,

        /// <summary>
        /// Modifies the SHGSI_ICON value by causing the function to add the link overlay to the file's icon.  (a.k.a. SHGSI_LINKOVERLAY)
        /// </summary>
        LinkOverlay = 0x00008000,

        /// <summary>
        /// Modifies the SHGSI_ICON value by causing the function to blend the icon with the system highlight color.  (a.k.a. SHGSI_SELECTED)
        /// </summary>
        Selected = 0x00010000     // Blend the icon with the system highlight color.
    }

    /// <summary>
    /// Stock shell icon identifiers
    /// </summary>
    /// <remarks>see shellapi.h</remarks>
    public enum StockIconIdentifier : uint
    {
        /// <summary>
        /// Not associated with an application. (a.k.a. SIID_DOCNOASSOC)
        /// </summary>
        DocumentNotAssociated = 0,

        /// <summary>
        /// Associated with an application. (a.k.a. SIID_DOCASSOC)
        /// </summary>
        DocumentAssociated = 1,

        /// <summary>
        /// Application with no custom icon. (a.k.a. SIID_APPLICATION)
        /// </summary>
        Application = 2,

        /// <summary>
        /// A closed folder. (a.k.a. SIID_FOLDER)
        /// </summary>
        Folder = 3,

        /// <summary>
        /// An open folder. (a.k.a. SIID_FOLDEROPEN)
        /// </summary>
        FolderOpen = 4,

        /// <summary>
        /// A 5.25 inch drive. (a.k.a. SIID_DRIVE525)
        /// </summary>
        Drive525 = 5,

        /// <summary>
        /// A 3.5 inch drive. (a.k.a. SIID_DRIVE35)
        /// </summary>
        Drive35 = 6,

        /// <summary>
        /// A removable drive. (a.k.a. SIID_DRIVEREMOVE)
        /// </summary>
        DriveRemove = 7,

        /// <summary>
        /// A fixed drive. (a.k.a. SIID_DRIVEFIXED)
        /// </summary>
        DriveFixed = 8,

        /// <summary>
        /// A network drive. (a.k.a. SIID_DRIVENET)
        /// </summary>
        DriveNetwork = 9,

        /// <summary>
        /// A disconnected network drive. (a.k.a. SIID_DRIVENETDISABLED)
        /// </summary>
        DriveNetworkDisabled = 10,

        /// <summary>
        /// A CD drive. (a.k.a. SIID_DRIVECD)
        /// </summary>
        DriveCD = 11,

        /// <summary>
        /// A RAM Drive. (a.k.a. SIID_DRIVERAM)
        /// </summary>
        DriveRAM = 12,

        /// <summary>
        /// The entire network. (a.k.a. SIID_WORLD)
        /// </summary>
        World = 13,

        /// <summary>
        /// A computer on the network. (a.k.a. SIID_SERVER)
        /// </summary>
        Server = 15,

        /// <summary>
        /// A printer. (a.k.a. SIID_PRINTER)
        /// </summary>
        Printer = 16,

        /// <summary>
        /// My Network Places. (a.k.a. SIID_MYNETWORK)
        /// </summary>
        MyNetwork = 17,

        /// <summary>
        /// Find. (a.k.a. SIID_FIND)
        /// </summary>
        Find = 22,

        /// <summary>
        /// Help. (a.k.a. SIID_HELP)
        /// </summary>
        Help = 23,

        /// <summary>
        /// A network share. (a.k.a. SIID_SHARE)
        /// </summary>
        Share = 28,

        /// <summary>
        /// A link to a network or internet resource. (a.k.a. SIID_LINK)
        /// </summary>
        Link = 29,

        /// <summary>
        /// A slow file. (a.k.a. SIID_SLOWFILE)
        /// </summary>
        SlowFile = 30,

        /// <summary>
        /// An empty recycle bin. (a.k.a. SIID_RECYCLER)
        /// </summary>
        Recycler = 31,

        /// <summary>
        /// A full recycle bin. (a.k.a. SIID_RECYCLERFULL)
        /// </summary>
        RecyclerFull = 32,

        /// <summary>
        /// Indicates an audio CD. (a.k.a. SIID_MEDIACDAUDIO)
        /// </summary>
        MediaCDAudio = 40,

        /// <summary>
        /// Security lock. (a.k.a. SIID_LOCK)
        /// </summary>
        Lock = 47,

        /// <summary>
        /// AutoList. (a.k.a. SIID_AUTOLIST)
        /// </summary>
        AutoList = 49,

        /// <summary>
        /// Network printer. (a.k.a. SIID_PRINTERNET)
        /// </summary>
        PrinterNet = 50,

        /// <summary>
        /// Server share. (a.k.a. SIID_SERVERSHARE)
        /// </summary>
        ServerShare = 51,

        /// <summary>
        /// Fax printer. (a.k.a. SIID_PRINTERFAX)
        /// </summary>
        PrinterFax = 52,

        /// <summary>
        /// Network fax printer. (a.k.a. SIID_PRINTERFAXNET)
        /// </summary>
        PrinterFaxNet = 53,

        /// <summary>
        /// Print to file. (a.k.a. SIID_PRINTERFILE)
        /// </summary>
        PrinterFile = 54,

        /// <summary>
        /// Stack. (a.k.a. SIID_STACK)
        /// </summary>
        Stack = 55,

        /// <summary>
        /// SVCD media. (a.k.a. SIID_MEDIASVCD)
        /// </summary>
        MediaSVCD = 56,

        /// <summary>
        /// Folder containing other items. (a.k.a. SIID_STUFFEDFOLDER)
        /// </summary>
        StuffedFolder = 57,

        /// <summary>
        /// Unknown drive. (a.k.a. SIID_DRIVEUNKNOWN)
        /// </summary>
        DriveUnknown = 58,

        /// <summary>
        /// DVD drive. (a.k.a. SIID_DRIVEDVD)
        /// </summary>
        DriveDVD = 59,

        /// <summary>
        /// DVD media. (a.k.a. SIID_MEDIADVD)
        /// </summary>
        MediaDVD = 60,

        /// <summary>
        /// DVD-RAM media. (a.k.a. SIID_MEDIADVDRAM)
        /// </summary>
        MediaDVDRAM = 61,

        /// <summary>
        /// DVD-RW media. (a.k.a. SIID_MEDIADVDRW)
        /// </summary>
        MediaDVDRW = 62,

        /// <summary>
        /// DVD-R media. (a.k.a. SIID_MEDIADVDR)
        /// </summary>
        MediaDVDR = 63,

        /// <summary>
        /// DVD-ROM media. (a.k.a. SIID_MEDIADVDROM)
        /// </summary>
        MediaDVDROM = 64,

        /// <summary>
        /// CD+ (Enhanced CD) media. (a.k.a. SIID_MEDIACDAUDIOPLUS)
        /// </summary>
        MediaCDAudioPlus = 65,

        /// <summary>
        /// CD-RW media. (a.k.a. SIID_MEDIACDRW)
        /// </summary>
        MediaCDRW = 66,

        /// <summary>
        /// CD-R media. (a.k.a. SIID_MEDIACDR)
        /// </summary>
        MediaCDR = 67,

        /// <summary>
        /// Buring CD media. (a.k.a. SIID_MEDIACDBURN)
        /// </summary>
        MediaCDBurn = 68,

        /// <summary>
        /// Blank CD media. (a.k.a. SIID_MEDIABLANKCD)
        /// </summary>
        MediaBlankCD = 69,

        /// <summary>
        /// CD-ROM media. (a.k.a. SIID_MEDIACDROM)
        /// </summary>
        MediaCDROM = 70,

        /// <summary>
        /// Audio files. (a.k.a. SIID_AUDIOFILES)
        /// </summary>
        AudioFiles = 71,

        /// <summary>
        /// Image files. (a.k.a. SIID_IMAGEFILES)
        /// </summary>
        ImageFiles = 72,

        /// <summary>
        /// Video files. (a.k.a. SIID_VIDEOFILES)
        /// </summary>
        VideoFiles = 73,

        /// <summary>
        /// Mixed files. (a.k.a. SIID_MIXEDFILES)
        /// </summary>
        MixedFiles = 74,

        /// <summary>
        /// Folder back. (a.k.a. SIID_FOLDERBACK)
        /// </summary>
        FolderBack = 75,

        /// <summary>
        /// Folder front. (a.k.a. SIID_FOLDERFRONT)
        /// </summary>
        FolderFront = 76,

        /// <summary>
        /// Security sheild.  Use for UAC prompts only. (a.k.a. SIID_SHIELD)
        /// </summary>
        Shield = 77,

        /// <summary>
        /// Warning. (a.k.a. SIID_WARNING)
        /// </summary>
        Warning = 78,

        /// <summary>
        /// Informational. (a.k.a. SIID_INFO)
        /// </summary>
        Info = 79,

        /// <summary>
        /// Error. (a.k.a. SIID_ERROR)
        /// </summary>
        Error = 80,

        /// <summary>
        /// Key / Secure. (a.k.a. SIID_KEY)
        /// </summary>
        Key = 81,

        /// <summary>
        /// Software. (a.k.a. SIID_SOFTWARE)
        /// </summary>
        Software = 82,

        /// <summary>
        /// Rename. (a.k.a. SIID_RENAME)
        /// </summary>
        Rename = 83,

        /// <summary>
        /// Delete. (a.k.a. SIID_DELETE)
        /// </summary>
        Delete = 84,

        /// <summary>
        /// Audio DVD media. (a.k.a. SIID_MEDIAAUDIODVD)
        /// </summary>
        MediaAudioDVD = 85,

        /// <summary>
        /// Movie DVD media. (a.k.a. SIID_MEDIAMOVIEDVD)
        /// </summary>
        MediaMovieDVD = 86,

        /// <summary>
        /// Enhanced CD media. (a.k.a. SIID_MEDIAENHANCEDCD)
        /// </summary>
        MediaEnhancedCD = 87,

        /// <summary>
        /// Enhanced DVD media. (a.k.a. SIID_MEDIAENHANCEDDVD)
        /// </summary>
        MediaEnhancedDVD = 88,

        /// <summary>
        /// HD-DVD media. (a.k.a. SIID_MEDIAHDDVD)
        /// </summary>
        MediaHDDVD = 89,

        /// <summary>
        /// BluRay media. (a.k.a. SIID_MEDIABLURAY)
        /// </summary>
        MediaBluRay = 90,

        /// <summary>
        /// VCD media. (a.k.a. SIID_MEDIAVCD)
        /// </summary>
        MediaVCD = 91,

        /// <summary>
        /// DVD+R media. (a.k.a. SIID_MEDIADVDPLUSR)
        /// </summary>
        MediaDVDPlusR = 92,

        /// <summary>
        /// DVD+RW media. (a.k.a. SIID_MEDIADVDPLUSRW)
        /// </summary>
        MediaDVDPlusRW = 93,

        /// <summary>
        /// Desktop computer. (a.k.a. SIID_DESKTOPPC)
        /// </summary>
        DesktopPC = 94,

        /// <summary>
        /// Mobile computer (laptop/notebook). (a.k.a. SIID_MOBILEPC)
        /// </summary>
        MobilePC = 95,

        /// <summary>
        /// Users. (a.k.a. SIID_USERS)
        /// </summary>
        Users = 96,

        /// <summary>
        /// Smart media. (a.k.a. SIID_MEDIASMARTMEDIA)
        /// </summary>
        MediaSmartMedia = 97,

        /// <summary>
        /// Compact flash. (a.k.a. SIID_MEDIACOMPACTFLASH)
        /// </summary>
        MediaCompactFlash = 98,

        /// <summary>
        /// Cell phone. (a.k.a. SIID_DEVICECELLPHONE)
        /// </summary>
        DeviceCellPhone = 99,

        /// <summary>
        /// Camera. (a.k.a. SIID_DEVICECAMERA)
        /// </summary>
        DeviceCamera = 100,

        /// <summary>
        /// Video camera. (a.k.a. SIID_DEVICEVIDEOCAMERA)
        /// </summary>
        DeviceVideoCamera = 101,

        /// <summary>
        /// Audio player. (a.k.a. SIID_DEVICEAUDIOPLAYER)
        /// </summary>
        DeviceAudioPlayer = 102,

        /// <summary>
        /// Connect to network. (a.k.a. SIID_NETWORKCONNECT)
        /// </summary>
        NetworkConnect = 103,

        /// <summary>
        /// Internet. (a.k.a. SIID_INTERNET)
        /// </summary>
        Internet = 104,

        /// <summary>
        /// ZIP file. (a.k.a. SIID_ZIPFILE)
        /// </summary>
        ZipFile = 105,

        /// <summary>
        /// Settings. (a.k.a. SIID_SETTINGS)
        /// </summary>
        Settings = 106,

        /// <summary>
        /// The number of value values in this enumeration. (a.k.a. SIID_MAX)
        /// </summary>
        Max = 107
    }

    /// <summary>
    /// Retrieves information about system-defined Shell icons.
    /// </summary>
    /// <remarks>
    /// Class only supported on Vista platforms.
    /// </remarks>
    public static class StockIcons
    {
        /// <summary>
        /// Retrieves system-defined Shell icons.
        /// </summary>
        /// <param name="identifier">One of the values from the StockIconIdentifier enumeration that specifies which icon should be retrieved.</param>
        /// <returns>The stock Shell icon.</returns>
        public static Image GetImage(StockIconIdentifier identifier)
        {
            return GetImage(identifier, StockIconOptions.None);
        }

        /// <summary>
        /// Retrieves system-defined Shell icons.
        /// </summary>
        /// <param name="identifier">One of the values from the StockIconIdentifier enumeration that specifies which icon should be retrieved.</param>
        /// <param name="flags">A combination of zero or more of the StockIconOptions flags that specify additional attributes of the desired icon.</param>
        /// <returns>The stock Shell icon.</returns>
        public static Image GetImage(StockIconIdentifier identifier, StockIconOptions flags)
        {
            return MakeImageForStockIcon(identifier, StockIconOptions.Handle | flags);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Image Shield { get { return GetImage(StockIconIdentifier.Shield); } }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct StockIconInfo
        {
            internal UInt32 cbSize;
            internal IntPtr hIcon;
            internal Int32 iSysImageIndex;
            internal Int32 iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szPath;
        }

        private static class NativeMethods
        {
            [DllImport("Shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
            public static extern Int32 SHGetStockIconInfo(StockIconIdentifier identifier, StockIconOptions flags, ref StockIconInfo info);

            [DllImport("User32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean DestroyIcon(IntPtr handle);
        }

        private static Image MakeImageForStockIcon(StockIconIdentifier identifier, StockIconOptions flags)
        {
            Image result;

            IntPtr iconHandle = GetStockIconHandle(identifier, flags);
            try
            {
                result = Bitmap.FromHicon(iconHandle);
            }
            finally
            {
                NativeMethods.DestroyIcon(iconHandle);
            }

            return result;
        }

        private static IntPtr GetStockIconHandle(StockIconIdentifier identifier, StockIconOptions flags)
        {
            Debug.Assert(System.Environment.OSVersion.Version.Major >= 6, "SHGetStockIconInfo only supported on Vista");

            StockIconInfo stockIconInfo = new StockIconInfo();
            stockIconInfo.cbSize = (UInt32)Marshal.SizeOf(typeof(StockIconInfo));

            Int32 hResult = NativeMethods.SHGetStockIconInfo(identifier, flags, ref stockIconInfo);
            if (hResult < 0)
            {
                Int32 lastWin32Error = Marshal.GetLastWin32Error();
                throw new Win32Exception(lastWin32Error, "SHGetStockIconInfo failed. Marshal.GetLastWin32Error = 0x" + lastWin32Error.ToString("X8", CultureInfo.CurrentUICulture));
            }

            return stockIconInfo.hIcon;
        }
    }
}
