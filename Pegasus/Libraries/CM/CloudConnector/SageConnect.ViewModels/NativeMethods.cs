using System;
using System.Runtime.InteropServices;

namespace SageConnect.ViewModels
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(
            IntPtr hWnd, // handle to destination window
            uint Msg, // message
            IntPtr wParam, // first message parameter
            IntPtr lParam // second message parameter
            );

        public const int SW_SHOWNORMAL = 1;
        public const int WM_CONNECTOR_SHOW_NORMAL = 0x0400 + 0x0995;  //WM_USER + 995
    }
}
