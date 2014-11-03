using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;
using Sage.Connector.Common;
using System.Runtime.InteropServices;

namespace ConnectorServiceMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {

                // Programativc option to configure current user for local
                // machine monitoring
                if ((args.Length > 0) && (args[0] == "CONFIGUREFORLOCAL"))
                {
                    Internal.ConfigureForLocalMachine.Configure();
                    return;
                }

                IntPtr hwnd = IntPtr.Zero;
                if(SingleInstanceApplicationChecker.IsOnlyProcessInThisSession(Common.MonitorBriefProductName, out hwnd))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    try
                    {
                        Application.Run(new MainForm());
                    }

                    catch (Exception ex)
                    {
                        using (var logger = new SimpleTraceLogger())
                        {
                            logger.WriteError(null, ex.ExceptionAsString());
                        }
                    }
                }
                else
                {
                    if (hwnd != IntPtr.Zero)
                    {
                        NativeMethods.ShowWindowAsync(hwnd, NativeMethods.SW_SHOWNORMAL);
                        NativeMethods.SetForegroundWindow(hwnd);
                        NativeMethods.PostMessage(hwnd, NativeMethods.WM_CONNECTOR_SHOW_NORMAL, IntPtr.Zero, IntPtr.Zero);
                    }
                    else
                    {
                        // window is running in some other user's session
                        MessageBox.Show(null, String.Format("The {0} application is already running.\n\nUnable to bring the current instance to the foreground.", Common.MonitorBriefProductName), 
                            Common.MonitorBriefProductName, 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private static bool ShowServerRegistrationFormIfNecessary()
        {
            Boolean exitApp = false;
            bool serviceIsReady = false;
            bool giveUp = false;
            do
            {
                var serverRegistration = new ServerRegistration(new ServerRegistrationParams(Common.ServerRegistrySubKeyPath, Common.DefaultCatalogServicePortNumber, null));
                if (!serverRegistration.IsServerEstablished(IntPtr.Zero))
                {
                    exitApp = true;
                    giveUp = true;
                }
                else
                {
                    serviceIsReady = true;
                }
            } while (!serviceIsReady && !giveUp);


            if (!exitApp)
            {
                if (!serviceIsReady)
                {
                    exitApp = true;
                }

                if (giveUp)
                {
                    exitApp = true;
                }
            }

            return exitApp;
        }

        internal static Boolean ForceGui
        { get { return (Array.FindIndex(System.Environment.GetCommandLineArgs(), delegate(String argument) { return (argument.Equals("/forcegui", StringComparison.InvariantCultureIgnoreCase) || argument.Equals("-forcegui", StringComparison.InvariantCultureIgnoreCase)); }) != -1); } }
    }

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
