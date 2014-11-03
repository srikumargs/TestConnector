using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Sage.Connector.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SingleInstanceApplicationChecker
    {
        static string _requiredString;

        /// <summary>
        /// 
        /// </summary>
        private static class NativeMethods
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="lpEnumFunc"></param>
            /// <param name="lParam"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProcDel lpEnumFunc,
                IntPtr lParam);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="lpdwProcessId"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern int GetWindowThreadProcessId(IntPtr hWnd,
                ref Int32 lpdwProcessId);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="lpString"></param>
            /// <param name="nMaxCount"></param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
                Int32 nMaxCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumWindowsProcDel(IntPtr hWnd, Int32 lParam);

        private static IntPtr _hwndToRestore;

        //Find the window and if it exists set as foreground window
        static private bool EnumWindowsProc(IntPtr hWnd, Int32 lParam)
        {
            int processId = 0;
            NativeMethods.GetWindowThreadProcessId(hWnd, ref processId);

            StringBuilder caption = new StringBuilder(1024);
            NativeMethods.GetWindowText(hWnd, caption, 1024);

            // Use IndexOf to make sure our required string is in the title.
            if (processId == lParam && (caption.ToString().IndexOf(_requiredString,
                StringComparison.OrdinalIgnoreCase) != -1))
            {
                // remember this window handle, so that we can re-force it to the foreground if desireable
                _hwndToRestore = hWnd;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the application is not already running.
        /// </summary>
        /// <param name="forceTitle"></param>
        /// <param name="hwndToRestore"></param>
        /// <returns></returns>
        public static bool IsOnlyProcessOnThisMachine(string forceTitle, out IntPtr hwndToRestore)
        {
            hwndToRestore = IntPtr.Zero;
            _requiredString = forceTitle;


            foreach (Process proc in Process.GetProcessesByName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name))
            {
                // if the candidate process ID is not the same as us
                if (proc.Id != Process.GetCurrentProcess().Id)
                {
                    // if the candidate process DOES have the same main module location as us
                    if (proc.MainModule.FileName.ToLowerInvariant() == System.Reflection.Assembly.GetEntryAssembly().Location.ToLowerInvariant())
                    {
                        NativeMethods.EnumWindows(new EnumWindowsProcDel(EnumWindowsProc),
                            new IntPtr(proc.Id));
                        if (proc.SessionId == Process.GetCurrentProcess().SessionId)
                        {
                            // if the other process is in the same session, then remember the hwnd so that we can force it 
                            // back to the foreground
                            hwndToRestore = _hwndToRestore;
                        }

                        // some other process running; not the only one on this machine
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the application is not already running.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="hwndToRestore"></param>
        /// <returns></returns>
        public static bool IsApplicationRunningOnThisMachine(string applicationName, out IntPtr hwndToRestore)
        {
            hwndToRestore = IntPtr.Zero;


            foreach (Process proc in Process.GetProcessesByName(applicationName))
            {
                // if the candidate process ID is not the same as us
                if (proc.Id != Process.GetCurrentProcess().Id)
                {
                    // if the candidate process DOES have the same main module location as us
                    if (proc.MainModule.FileName.ToLowerInvariant() == System.Reflection.Assembly.GetEntryAssembly().Location.ToLowerInvariant())
                    {
                        NativeMethods.EnumWindows(new EnumWindowsProcDel(EnumWindowsProc),
                            new IntPtr(proc.Id));
                        if (proc.SessionId == Process.GetCurrentProcess().SessionId)
                        {
                            // if the other process is in the same session, then remember the hwnd so that we can force it 
                            // back to the foreground
                            hwndToRestore = _hwndToRestore;
                        }

                        // some other process running; not the only one on this machine
                        return false;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// Returns true if the application is not already running in this session.
        /// </summary>
        /// <param name="forceTitle"></param>
        /// <param name="hwndToRestore"></param>
        /// <returns></returns>
        public static bool IsOnlyProcessInThisSession(string forceTitle, out IntPtr hwndToRestore)
        {
            hwndToRestore = IntPtr.Zero;
            _requiredString = forceTitle;
            try
            {
                //Process myProc = Process.GetProcesses().FirstOrDefault(pp => pp.ProcessName.StartsWith(Assembly.GetEntryAssembly().GetName().Name));

                //Process existingProcess = Process.GetProcesses().FirstOrDefault(pp => pp.ProcessName == Assembly.GetEntryAssembly().GetName().Name && pp.SessionId == myProc.SessionId && pp.Id != myProc.Id);
                //if (existingProcess != null)
                //{
                //    NativeMethods.EnumWindows(new EnumWindowsProcDel(EnumWindowsProc),
                //                       new IntPtr(existingProcess.Id));
                //    // if the other process is in the same session, then remember the hwnd so that we can force it 
                //    // back to the foreground
                //    hwndToRestore = _hwndToRestore;
                //    return false;

                //}


                foreach (Process proc in Process.GetProcessesByName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name))
                {
                    // if the candidate process ID is not the same as us
                    if (proc.Id != Process.GetCurrentProcess().Id)
                    {
                        // if the candidate process DOES have the same main module location as us
                        if (proc.MainModule.FileName.ToLowerInvariant() ==
                            System.Reflection.Assembly.GetEntryAssembly().Location.ToLowerInvariant())
                        {

                            if (proc.SessionId == Process.GetCurrentProcess().SessionId)
                            {
                                NativeMethods.EnumWindows(new EnumWindowsProcDel(EnumWindowsProc),
                                    new IntPtr(proc.Id));
                                // if the other process is in the same session, then remember the hwnd so that we can force it 
                                // back to the foreground
                                hwndToRestore = _hwndToRestore;

                                // some other process running; not the only one in this session
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.HResult == -2147467259;
            }
            return true;
        }
    }
}
