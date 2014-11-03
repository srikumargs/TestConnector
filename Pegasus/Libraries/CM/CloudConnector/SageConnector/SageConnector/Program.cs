using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sage.Connector.Common;
using SageConnector.ViewModel;

namespace SageConnector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Environment.ExitCode = -1;

            if (UpgradeAccountSelection)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    Boolean exitApp = UpgradeShowAccountSelectionFormIfNecessary();
                    if (!exitApp)
                    {
                        Environment.ExitCode = 0;
                    }
                    else
                    {
                        Environment.ExitCode = 1;
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
            else if (Config)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ConnectorServiceNotReadyForm());
            }
            else
            {
                try
                {
                    IntPtr hwnd = IntPtr.Zero;
                    if (SingleInstanceApplicationChecker.IsOnlyProcessOnThisMachine(ConnectorRegistryUtils.BriefProductName, out hwnd))
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        try
                        {
                            Boolean exitApp = ShowServiceNotReadyFormIfNecessary();
                            if (!exitApp)
                            {
                                // service should now be ready, run the main form
                                Application.Run(new CloudConnectorMainForm());
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
                    else
                    {
                        // bring the already-running window back to the foreground
                        if (hwnd != IntPtr.Zero)
                        {
                            NativeMethods.ShowWindowAsync(hwnd, NativeMethods.SW_SHOWNORMAL);
                            NativeMethods.SetForegroundWindow(hwnd);
                        }
                        else
                        {
                            // window is running in some other user's session
                            MessageBox.Show(null, String.Format("One or more users are already running the {0}.\n\nThese users must first close the application before you can use it.", ConnectorRegistryUtils.BriefProductName), ConnectorRegistryUtils.BriefProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }

        private static bool ShowServiceNotReadyFormIfNecessary()
        {
            Boolean exitApp = false;
            bool serviceIsReady = false;
            bool giveUp = false;
            do
            {
                using (var logger = new SimpleTraceLogger())
                {
                    serviceIsReady = ConnectorViewModel.IsHostingFrameworkServiceReady(logger);
                    if (!serviceIsReady)
                    {
                        if (ConnectorServiceUtils.IsServiceRegistered(logger) && !ConnectorServiceUtils.IsServiceEnabled())
                        {
                            if (DialogResult.Cancel == MessageBox.Show(ConnectorViewModel.ConnectorServiceNotEnabledMessage, ConnectorRegistryUtils.BriefProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Stop))
                            {
                                giveUp = true;
                                exitApp = true;
                            }
                        }
                        else
                        {
                            try
                            {
                                ConnectorServiceNotReadyForm form = new ConnectorServiceNotReadyForm();
                                Application.Run(form);

                                if (form.DialogResult != DialogResult.OK)
                                {
                                    giveUp = true;
                                }
                            }
                            catch
                            {
                                //TODO: should we be trying to present ex?
                                //cant count on log working if we are here.
                                //but this does prevent us from showing the form when
                                //we get an exception here at least.
                                //write to windows event log at least....?
                                exitApp = true;
                                giveUp = true;
                            }
                        }
                    }
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

        private static bool UpgradeShowAccountSelectionFormIfNecessary()
        {
            Boolean exitApp = false;
            bool serviceIsReady = false;
            bool giveUp = false;
            do
            {
                using (var logger = new SimpleTraceLogger())
                {
                    serviceIsReady = ConnectorViewModel.IsHostingFrameworkServiceReady(logger);
                    if (!serviceIsReady)
                    {
                        if (ConnectorServiceUtils.IsServiceRegistered(logger) && !ConnectorServiceUtils.IsServiceEnabled())
                        {
                            if (DialogResult.Cancel == MessageBox.Show(ConnectorViewModel.ConnectorServiceNotEnabledMessage, ConnectorRegistryUtils.BriefProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Stop))
                            {
                                giveUp = true;
                                exitApp = true;
                            }
                        }
                        else
                        {
                            try
                            {
                                ConnectorServiceNotReadyForm form = new ConnectorServiceNotReadyForm();
                                Application.Run(form);

                                if (form.DialogResult != DialogResult.OK)
                                {
                                    giveUp = true;
                                }
                            }
                            catch
                            {
                                //TODO: should we be trying to present ex?
                                //cant count on log working if we are here.
                                //but this does prevent us from showing the form when
                                //we get an exception here at least.
                                //write to windows event log at least....?
                                exitApp = true;
                                giveUp = true;
                            }
                        }
                    }
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

        internal static Boolean Config
        { get { return (Array.FindIndex(System.Environment.GetCommandLineArgs(), delegate(String argument) { return (argument.Equals("/config", StringComparison.InvariantCultureIgnoreCase) || argument.Equals("-config", StringComparison.InvariantCultureIgnoreCase)); }) != -1); } }

        internal static Boolean UpgradeAccountSelection
        { get { return (Array.FindIndex(System.Environment.GetCommandLineArgs(), delegate(String argument) { return (argument.Equals("/upgradeaccountselection", StringComparison.InvariantCultureIgnoreCase) || argument.Equals("-upgradeaccountselection", StringComparison.InvariantCultureIgnoreCase)); }) != -1); } }

        internal static String User
        {
            get
            {
                String result = String.Empty;

                Int32 index = Array.FindIndex(System.Environment.GetCommandLineArgs(), delegate(String argument) { return (argument.StartsWith("/user:", StringComparison.InvariantCultureIgnoreCase) || argument.Equals("-user:", StringComparison.InvariantCultureIgnoreCase)); });
                if (index != -1)
                {
                    String commandLineArg = System.Environment.GetCommandLineArgs()[index];
                    result = commandLineArg.Substring(commandLineArg.IndexOf(':') + 1);
                }

                return result;
            }
        }


        /// <summary>
        /// To be used in config mode, additional start only option
        /// </summary>
        internal static Boolean Start
        { get { return (Array.FindIndex(System.Environment.GetCommandLineArgs(), delegate(String argument) { return (argument.Equals("/start", StringComparison.InvariantCultureIgnoreCase) || argument.Equals("-start", StringComparison.InvariantCultureIgnoreCase)); }) != -1); } }

        internal static IntPtr ParentHandle
        {
            get
            {
                IntPtr result = IntPtr.Zero;

                Int32 index = Array.FindIndex(System.Environment.GetCommandLineArgs(), delegate(String argument) { return (argument.StartsWith("/parentHandle:", StringComparison.InvariantCultureIgnoreCase) || argument.Equals("-parentHandle:", StringComparison.InvariantCultureIgnoreCase)); });
                if (index != -1)
                {
                    String handleAsString = System.Environment.GetCommandLineArgs()[index];
                    result = new IntPtr(System.Convert.ToInt64(handleAsString.Substring(handleAsString.IndexOf(':') + 1)));
                }

                return result;
            }
        }


        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            public const int SW_SHOWNORMAL = 1;
        }
    }
}
