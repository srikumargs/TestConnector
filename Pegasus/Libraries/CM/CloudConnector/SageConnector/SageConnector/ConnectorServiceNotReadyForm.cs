using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Sage.Connector.Common;
using Sage.CRE.Core.UI;
using Sage.CRE.HostingFramework.Interfaces;
using SageConnector.Properties;

namespace SageConnector
{
    internal partial class ConnectorServiceNotReadyForm : Form
    {
        public ConnectorServiceNotReadyForm()
        {
            InitializeComponent();
            this.Text = ConnectorRegistryUtils.BriefProductName;
        }


        /// <summary>
        /// Run the account selection form in the specified mode
        /// </summary>
        /// <param name="mode"></param>
        internal static bool RunAccountSelectionForm(AccountSelectionFormMode mode)
        {
            bool userCancelled = false;

            // Don't launch if form mode is nons
            if (mode == AccountSelectionFormMode.None)
            {
                return userCancelled;
            }

            // Reset credentials
            _user = null;
            _password = null;
            _requiresReInstall = false;

            // Get credentials first
            using (AccountSelectionForm accountSelectionForm = new AccountSelectionForm(mode))
            {
                DialogResult result = Program.ParentHandle != IntPtr.Zero 
                    ? accountSelectionForm.ShowDialog(Win32Window.FromHandle(Program.ParentHandle)) 
                    : accountSelectionForm.ShowDialog(); 

                if (result != DialogResult.OK)
                {
                    userCancelled = true;
                }
                else
                {
                    _user = accountSelectionForm.User;
                    _password = accountSelectionForm.Password;
                    _requiresReInstall = accountSelectionForm.RequiresReInstall;
                }
            }

            return userCancelled;
        }

        internal static void Configure(out Boolean userCancelled, AccountSelectionFormMode mode)
        {
            // Reset
            userCancelled = false;
            _uninstallExistingServiceFirst = false;

            // Get credentials first, if applicable
            bool accountSelectionUserCancelled = RunAccountSelectionForm(mode);

            if (!accountSelectionUserCancelled)
            {
                if (mode != AccountSelectionFormMode.None)
                {   
                    // We entered the account selection form, 
                    // And the user made changes to the login
                    // So we need to uninstall the HF first
                    _uninstallExistingServiceFirst = true;
                }

                _progressForm = new ProgressForm();
                _progressForm.UserCanRequestCancel = true;
                _progressForm.UserCancelled += new EventHandler(_progressForm_UserCancelled);
                _progressForm.Text = "Configure Connector Service";

                _backgroundWorker = new System.ComponentModel.BackgroundWorker();
                _backgroundWorker.WorkerSupportsCancellation = true;
                _backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(_backgroundWorker_DoWork);
                _backgroundWorker.RunWorkerCompleted +=
                    new System.ComponentModel.RunWorkerCompletedEventHandler(_backgroundWorker_DoWorkCompleted);
                _backgroundWorker.RunWorkerAsync();

                if (Program.ParentHandle != IntPtr.Zero)
                {
                    _progressForm.ShowDialog(Win32Window.FromHandle(Program.ParentHandle));
                }
                else
                {
                    _progressForm.ShowDialog();
                }

                userCancelled = _userCancelled;
            }

            if (userCancelled)
            {
                Environment.ExitCode = 1;
            }
            else
            {
                Environment.ExitCode = 0;
            }
        }

        private static void _progressForm_UserCancelled(Object sender, EventArgs e)
        {
            if (_backgroundWorker.WorkerSupportsCancellation)
            {
                lock (_lockObject)
                {
                    _backgroundWorker.CancelAsync();
                    _progressForm.Dispose();
                    _progressForm = null;
                    _userCancelled = true;
                }
            }
        }

        private static void _backgroundWorker_DoWork(Object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                // Reset results
                _workerExitCode = ExitCode.Unknown;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                using (var logger = new SimpleTraceLogger())
                {
                    // SETUP
                    var baselineDataFolder = Path.Combine(ConnectorServiceUtils.InstanceApplicationDataFolder, "Baseline");
                    foreach (var file in Directory.GetFiles(baselineDataFolder, "*.*"))
                    {
                        if (!CopyFileFromBaseline(e, Path.GetFileName(file), false))
                            return;
                    }

                    // Store the initial is service registered state
                    Boolean isServiceRegistered = ConnectorServiceUtils.IsServiceRegistered(logger);

                    if (_requiresReInstall)
                    {
                        // STOP AND UNINSTALL EXISTING CONNECTOR SERVICE
                        if (isServiceRegistered && _uninstallExistingServiceFirst)
                        {
                            if (ConnectorServiceUtils.IsServiceRunning(logger))
                            {
                                // If the service is currently running, stop it before uninstalling.
                                // This is an attempt to clean up any lingering mutexes
                                MethodInvoker i = delegate()
                                    {
                                        lock (_lockObject)
                                        {
                                            _progressForm.Text = "Stop Existing Connector Service";
                                        }
                                    };
                                if (!HandleCancelOrUpdateProgress(e, 0, i))
                                    return;

                                // Perform the stop
                                bool stopResult = ConnectorServiceUtils.UninstallService(
                                    ConnectorRegistryUtils.ConnectorServiceInstallTimeout);

                                if (!stopResult)
                                {
                                    // Error encountered
                                    _workerExitCode = ExitCode.Fail;
                                    return;
                                }
                            }

                            // Uninstall the existing service before continuing
                            // For example, when the user has changed the service account
                            // First update progress bar
                            MethodInvoker i2 = delegate()
                                {
                                    lock (_lockObject)
                                    {
                                        _progressForm.Text = "Uninstall Existing Connector Service";
                                    }
                                };
                            if (!HandleCancelOrUpdateProgress(e, 0, i2))
                                return;

                            // Perform the uninstall
                            bool uninstallResult = ConnectorServiceUtils.UninstallService(
                                ConnectorRegistryUtils.ConnectorServiceInstallTimeout);

                            if (!uninstallResult)
                            {
                                // Error encountered
                                _workerExitCode = ExitCode.Fail;
                                return;
                            }

                            // Set isServiceRegistered to false so we install next
                            isServiceRegistered = false;
                        }


                        // INSTALL CONNECTOR SERVICE
                        if (!isServiceRegistered)
                        {
                            MethodInvoker i = delegate()
                                {
                                    lock (_lockObject)
                                    {
                                        _progressForm.Text = "Install Connector Service";
                                    }
                                };
                            if (!HandleCancelOrUpdateProgress(e, 0, i))
                                return;

                            // Perform the install
                            _workerExitCode =
                                ConnectorServiceUtils.InstallService(
                                    ConnectorRegistryUtils.ConnectorServiceInstallTimeout,
                                    _user,
                                    _password,
                                    true);

                            if (_workerExitCode != ExitCode.Success)
                            {
                                // Error encountered
                                return;
                            }

                            // artificial delay so that the text is up for a minimum time
                            if (!HandleCancelOrUpdateProgress(e, 500, null))
                                return;
                        }
                    }

                    // START CONNECTOR SERVICE
                    if (!ConnectorServiceUtils.IsServiceRunning(logger))
                    {
                        // Verify and patch up the service execution path
                        ConnectorServiceUtils.VerifyAndPatchExecutionPath();

                        // Check if the service is enabled since the user has the option
                        // To specify installing in disabled mode via the instance config xml
                        // Note: added the isServiceRegistered check for the odd case where we got
                        // To the "Start" screen, and then the user disabled the service before
                        // Clicking start.  In that case we want to try to start so we get the
                        // Appropriate error message
                        if (isServiceRegistered || ConnectorServiceUtils.IsServiceEnabled())
                        {
                            // Service is not started; start it up
                            MethodInvoker i = delegate()
                                {
                                    lock (_lockObject)
                                    {
                                        _progressForm.Text = "Start Connector Service";
                                    }
                                };
                            if (!HandleCancelOrUpdateProgress(e, 0, i))
                                return;

                            // Perform the start
                            _workerExitCode =
                                ConnectorServiceUtils.StartService(
                                    ConnectorRegistryUtils.ConnectorServiceStartTimeout,
                                    ConnectorRegistryUtils.ConnectorServiceStartRetries);

                            if (_workerExitCode != ExitCode.Success)
                            {
                                // Error encountered
                                return;
                            }

                            // artificial delay so that the text is up for a minimum time
                            if (!HandleCancelOrUpdateProgress(e, 500, null))
                                return;
                        }
                    }
                    else if (!ConnectorServiceUtils.IsServiceReady())
                    {
                        // It is possible that the service is not ready yet, but it is started.  This could happen
                        // if the user manually restarted the service in the SCM and then launched the 
                        // Connector UI before the service reached the READY state.
                        MethodInvoker i = delegate()
                            {
                                lock (_lockObject)
                                {
                                    _progressForm.Text = "Waiting for Connector Service";
                                }
                            };
                        if (!HandleCancelOrUpdateProgress(e, 0, i))
                            return;

                        ConnectorServiceUtils.WaitForServiceReady(ConnectorRegistryUtils.ConnectorServiceWaitForReadyTimeout, ConnectorRegistryUtils.HostingFxWaitForReadySleepInterval);

                        // artificial delay so that the text is up for a minimum time
                        if (!HandleCancelOrUpdateProgress(e, 500, null))
                            return;
                    }


                    // INSTALL MONITOR SERVICE
                    Boolean monitorWasJustRegistered = false;
                    if (!ConnectorMonitorServiceUtils.IsServiceRegistered(logger))
                    {
                        MethodInvoker i = delegate()
                            {
                                lock (_lockObject)
                                {
                                    _progressForm.Text = "Install Connector Monitor Service";
                                }
                            };
                        if (!HandleCancelOrUpdateProgress(e, 0, i))
                            return;

                        // Perform the install
                        _workerExitCode = 
                            ConnectorMonitorServiceUtils.InstallService(
                                ConnectorRegistryUtils.MonitorServiceInstallTimeout,
                                StockAccountUtils.GetHostingFrameworkParamForStockAccountType(ConnectorRegistryUtils.MonitorServiceAccount),
                                null,
                                true);

                        if (_workerExitCode != ExitCode.Success)
                        {
                            // Error encountered
                            return;
                        }

                        // artificial delay so that the text is up for a minimum time
                        if (!HandleCancelOrUpdateProgress(e, 500, null))
                            return;
                        monitorWasJustRegistered = true;
                    }


                    // START MONITOR SERVICE
                    if (monitorWasJustRegistered && !ConnectorMonitorServiceUtils.IsServiceRunning(logger))
                    {
                        // Check if the service is enabled since the user has the option
                        // To specify installing in disabled mode via the instance config xml
                        if (ConnectorServiceUtils.IsServiceEnabled())
                        {
                            // Verify and patch up the service execution path
                            ConnectorMonitorServiceUtils.VerifyAndPatchExecutionPath();

                            MethodInvoker i = delegate()
                                {
                                    lock (_lockObject)
                                    {
                                        _progressForm.Text = "Start Connector Monitor Service";
                                    }
                                };
                            if (!HandleCancelOrUpdateProgress(e, 0, i))
                                return;

                            // Perform the start
                            _workerExitCode =
                                ConnectorMonitorServiceUtils.StartService(
                                    ConnectorRegistryUtils.MonitorServiceStartTimeout,
                                    ConnectorRegistryUtils.MonitorServiceStartRetries);

                            if (_workerExitCode != ExitCode.Success)
                            {
                                // Error encountered
                                return;
                            }

                            // artificial delay so that the text is up for a minimum time
                            if (!HandleCancelOrUpdateProgress(e, 500, null))
                                return;
                        }
                    }
                }

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;


                // COMPLETED SUCCESSFULLY
                _workerExitCode = ExitCode.Success;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private static Boolean CopyFileFromBaseline(DoWorkEventArgs e, String fileName, Boolean allowOverwrite)
        {
            var dataFolder = ConnectorServiceUtils.InstanceApplicationDataFolder;
            var baselineDataFolder = Path.Combine(ConnectorServiceUtils.InstanceApplicationDataFolder, "Baseline");
            bool fileExists = File.Exists(Path.Combine(dataFolder, fileName));

            if (!fileExists || allowOverwrite)
            {
                MethodInvoker i = delegate()
                {
                    lock(_lockObject)
                    {
                        _progressForm.Text = String.Format("Create {0} from Baseline", fileName);
                    }
                };
                if (!HandleCancelOrUpdateProgress(e, 0, i))
                    return false;

                File.Copy(Path.Combine(baselineDataFolder, fileName), Path.Combine(dataFolder, fileName));

                // artificial delay so that the text is up for a minimum time
                if (!HandleCancelOrUpdateProgress(e, 500, null))
                    return false;
            }

            return true;
        }

        private static Boolean HandleCancelOrUpdateProgress(DoWorkEventArgs e, Int32 sleep, MethodInvoker i)
        {
            lock (_lockObject)
            {
                if (_backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return false;
                }
                else
                {
                    // artificial delay so that the text is up for a minimum time
                    if (sleep != 0)
                    {
                        System.Threading.Thread.Sleep(sleep);
                    }

                    // update status
                    if (i != null)
                    {
                        _progressForm.BeginInvoke(i);
                    }
                }
            }

            return true;
        }

        private static void _backgroundWorker_DoWorkCompleted(Object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // If we need to go back to the account selection form
            // Then store the mode we want to go back to
            AccountSelectionFormMode restartAccountSelectionMode = AccountSelectionFormMode.None;

            lock (_lockObject)
            {
                if (_progressForm != null)
                {
                    _progressForm.Dispose();
                    _progressForm = null;
                }

                if (!_userCancelled)
                {
                    string errorText, caption;

                    // Handle the result
                    // Nothing to do for None or Success types
                    switch(_workerExitCode)
                    {
                        case ExitCode.Fail:
                            // Generic error case: show message box to the user
                            errorText = Resources.ConnectorServiceNotReady_ConfigureErrorText;
                            caption = Resources.ConnectorServiceNotReady_ConfigureErrorCaption;
                            if (Program.ParentHandle != IntPtr.Zero)
                            {
                                MessageBox.Show((Win32Window.FromHandle(Program.ParentHandle)), errorText, caption,
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show(errorText, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;

                        case ExitCode.FailWaitForServiceReady:
                            // Wait for service ready failure case: show message box to the user
                            errorText = String.Format(
                                Resources.ConnectorServiceNotReady_WaitForServiceErrorText,
                                ConnectorRegistryUtils.BriefProductName);
                            caption = Resources.ConnectorServiceNotReady_WaitForServiceErrorCaption;
                            if (Program.ParentHandle != IntPtr.Zero)
                            {
                                MessageBox.Show((Win32Window.FromHandle(Program.ParentHandle)), errorText, caption,
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show(errorText, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;

                        case ExitCode.FailInvalidLogin:
                            // Login error case: restart config with account selection in login error mode
                            restartAccountSelectionMode = AccountSelectionFormMode.LoginError;
                            break;

                        case ExitCode.FailAccountNameInvalid:
                            // Account error case: restart config with account selection in account error mode
                            restartAccountSelectionMode = AccountSelectionFormMode.AccountError;
                            break;

                        case ExitCode.FailAccountFolderAccess:
                            // Account access error case: user does not have expected file system access rights
                            restartAccountSelectionMode = AccountSelectionFormMode.AccountAccessError;
                            break;
                    }
                }
            }

            if (restartAccountSelectionMode != AccountSelectionFormMode.None)
            {
                // Need to reconfigure
                // Make sure we do this call outside of the lock!
                bool reConfigureCancelled;
                Configure(out reConfigureCancelled, restartAccountSelectionMode);
            }
        }

        private static BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private static ProgressForm _progressForm;

        /// <summary>
        /// Init based on the state of the connector service
        /// </summary>
        private void InitializeForState()
        {
            Boolean isServiceRegistered;
            using (var logger = new SimpleTraceLogger())
            {
                isServiceRegistered = ConnectorServiceUtils.IsServiceRegistered(logger);
            }

            if (!isServiceRegistered)
            {
                label1.Text = String.Format(Resources.ConnectorServiceNotReady_NotConfiguredText, 
                    ConnectorServiceUtils.ServiceDisplayName, ConnectorRegistryUtils.BriefProductName);
                pictureBox1.Image = Resources.Image_new_service;
                _startButton.Visible = false;
                _configureButton.Text = "C&onfigure";
            }
            else
            {
                // Format text with unicode bullet point character
                label1.Text = String.Format(Resources.ConnectorServiceNotReady_NotRunningText,
                    ConnectorServiceUtils.ServiceDisplayName, "\u2022");
                pictureBox1.Image = Resources.Image_service_stopped;
                _startButton.Visible = true;
                _configureButton.Text = "Rec&onfigure";
            }
        }

        private void ConnectorServiceNotReadyForm_Load(object sender, EventArgs e)
        {
            if (Program.Config)
            {
                Visible = false;
                ShowInTaskbar = false;

                Boolean userCancelled;

                // Determine the account selection mode based on whether 
                // The start argument was supplied
                AccountSelectionFormMode accountSelectionFormMode =
                    (Program.Start) ? AccountSelectionFormMode.None : AccountSelectionFormMode.EditConfiguration;

                // Configure
                Configure(out userCancelled, accountSelectionFormMode);
                DialogResult = userCancelled ? DialogResult.Cancel : DialogResult.OK;
                Close();
            }
            else
            {
                try
                {
                    // Initialize the form based on the state of the service
                    InitializeForState();

                    if (!WindowsIdentity.GetCurrent().IsUserAdmin())
                    {
                        AddShieldToButton(_configureButton);
                        AddShieldToButton(_startButton);
                    }

                    FlashWindow();
                }
                catch (Exception ex)
                {
                    using (var logger = new SimpleTraceLogger())
                    {
                        logger.WriteError(this, ex.ExceptionAsString());
                    }
                    Close();
                }
            }
        }

        private static void AddShieldToButton(Button b)
        {
            b.FlatStyle = FlatStyle.System;
            NativeMethods.SendMessage(b.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
        }

        private const int BCM_FIRST = 0x1600; //Normal button
        private const int BCM_SETSHIELD = (BCM_FIRST + 0x000C);        //Elevated button
        private const int UserCancelled = 1223;

        private void _configureButton_Click(object sender, EventArgs e)
        {
            Boolean userCancelled;
            ConfigWithConditionalElevation(AccountSelectionFormMode.EditConfiguration, false, out userCancelled);
            DialogResult = userCancelled ? DialogResult.Cancel : DialogResult.OK;

            Close();
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            bool userCancelled;
            ConfigWithConditionalElevation(AccountSelectionFormMode.None, true, out userCancelled);
            DialogResult = userCancelled ? DialogResult.Cancel : DialogResult.OK;
            
            Close();
        }

        /// <summary>
        /// Common code for running config elevated if needed
        /// </summary>
        /// <param name="startMode"></param>
        /// <param name="accountFormSelectionMode"></param>
        /// <param name="userCancelled"></param>
        private void ConfigWithConditionalElevation(AccountSelectionFormMode accountFormSelectionMode, bool startMode, out Boolean userCancelled)
        {
            userCancelled = false;
            if (!WindowsIdentity.GetCurrent().IsUserAdmin())
            {
                Process p = null;
                try
                {
                    // Setup base arguments
                    List<String> processArgs = new List<string>()
                        { "/config", String.Format("/parentHandle:{0:x}", this.Handle) };

                    if (startMode)
                    {
                        // Extra param for start mode
                        processArgs.Add("/start");
                    }
                    
                    // Start the process 
                    p = LaunchAsAdmin(processArgs);
                    p.WaitForExit();
                    userCancelled = (p.ExitCode == 1);
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode != UserCancelled)
                    {
                        throw;
                    }
                    else
                    {
                        userCancelled = true;
                    }
                }
            }
            else
            {
                // No need to elevate
                Configure(out userCancelled, accountFormSelectionMode);
            }
        }

        private static Process LaunchAsAdmin(IEnumerable<String> arguments)
        {
            StringBuilder argumentsStringBuilder = new StringBuilder();
            foreach (String argument in arguments)
            {
                argumentsStringBuilder.AppendFormat("{0} ", (argument));
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location, argumentsStringBuilder.ToString());
            processStartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
            processStartInfo.UseShellExecute = true;
            processStartInfo.Verb = "runas";
            return Process.Start(processStartInfo);
        }

        private void FlashWindow()
        {
            NativeMethods.FLASHWINFO fInfo = new NativeMethods.FLASHWINFO();

            fInfo.cbSize = System.Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = this.Handle;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            NativeMethods.FlashWindowEx(ref fInfo);
        }

        //Stop flashing. The system restores the window to its original state. 
        public const UInt32 FLASHW_STOP = 0;
        //Flash the window caption. 
        public const UInt32 FLASHW_CAPTION = 1;
        //Flash the taskbar button. 
        public const UInt32 FLASHW_TRAY = 2;
        //Flash both the window caption and taskbar button.
        //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
        public const UInt32 FLASHW_ALL = 3;
        //Flash continuously, until the FLASHW_STOP flag is set. 
        public const UInt32 FLASHW_TIMER = 4;
        //Flash continuously until the window comes to the foreground. 
        public const UInt32 FLASHW_TIMERNOFG = 12;

        /// <summary>
        /// Provides a Windows.Forms implementation of the IWin32Window interface for windows owned by
        /// a non-.Net window handle.
        /// </summary>
        public class Win32Window : IWin32Window
        {
            private readonly IntPtr _handle;

            private Win32Window(IntPtr handle) { _handle = handle; }

            /// <summary> Constructs an IWin32Window from a valid handle or returns null if handle == IntPtr.Zero </summary>
            public static IWin32Window FromHandle(IntPtr handle)
            {
                if (handle == IntPtr.Zero)
                    return null;
                return new Win32Window(handle);
            }

            IntPtr IWin32Window.Handle { get { return _handle; } }
        }

        private class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct FLASHWINFO
            {
                public UInt32 cbSize;
                public IntPtr hwnd;
                public UInt32 dwFlags;
                public UInt32 uCount;
                public UInt32 dwTimeout;
            }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern Boolean FlashWindowEx([In] ref FLASHWINFO pwfi);

            [DllImport("user32")]
            internal static extern UIntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32")]
            internal static extern UIntPtr SendMessage(IntPtr hWnd, UInt32 msg, UIntPtr wParam, UIntPtr lParam);

            internal static UIntPtr SendMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam)
            {
                return SendMessage(hWnd, msg, (UIntPtr)wParam, (UIntPtr)lParam);
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private readonly static Object _lockObject = new Object();
        private static Boolean _userCancelled;
        private static string _user;
        private static SecureString _password;
        private static Boolean _requiresReInstall;
        private static ExitCode _workerExitCode;
        private static Boolean _uninstallExistingServiceFirst;
    }
}
