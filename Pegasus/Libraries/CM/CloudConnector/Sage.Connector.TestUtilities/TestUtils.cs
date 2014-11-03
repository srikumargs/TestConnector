using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.Data;
using Sage.Connector.LinkedSource;

/*
    HOW TO RUN TEST HOSTING FRAMEWORK AS SPECIFIC USER
 
    1. In test setup, provide user login and setup flag, User must have privelages to run scheduled task!:
        Set environment variables
            SAGE_CONNECTOR_UNIT_TEST_SERVICE_ACCOUNT_NAME = best\ggasperin
            SAGE_CONNECTOR_UNIT_TEST_SERVICE_ACCOUNT_PASSWORD = xxxx
 
        TestUtils.UnitTestSetup(
            TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning | TestUtils.UnitTestSetupFlags.RunAsSpecifiedUser);
 
    2. In test or class cleanup, provide cleanup flag for unit tests running HF as a service:
        TestUtils.UnitTestSetup(
                TestUtils.UnitTestSetupFlags.StopHostingFx | 
                TestUtils.UnitTestSetupFlags.UndoServiceRunAsSpecifiedUser);
 */


namespace Sage.Connector.TestUtilities
{
    /// <summary>
    /// Utilities to help unit testing.
    /// </summary>
    public class TestUtils
    {
        /// <summary>
        /// Made canned tenant Ids publicly visible for unit tests
        /// </summary>
        /// 
        public static String[] CannedTenantIds
        { get { return _cannedTenantIds; } }

        private static readonly String[] _cannedTenantIds = 
        {
          new Guid("5dc75a87-4688-4d45-b4ff-91d43db98072").ToString(),
          new Guid("db94139b-38b5-495f-86b6-680f02908094").ToString()                                    
        };

        public static Dictionary<int, string> CreateNewPremiseStoreConfigurations(int qty)
        {
            Dictionary<int, string> configs = new Dictionary<int, string>();
            for (int i = 0; i < qty; i++)
            {
                try
                {
                    using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        //TODO: JSB update this for the BackOfficeConnectionCredentials
                        PremiseConfigurationRecord rep = proxy.CreateNewConfiguration();
                        rep.ConnectorPluginId = "Mock";
                        rep.BackOfficeProductName = "Mock Back Office Product";
                        rep.CloudTenantId = Guid.NewGuid().ToString();
                        rep.CloudPremiseKey = "PremiseKey-" + i.ToString();
                        rep.Id = Guid.NewGuid();

                        proxy.AddConfiguration(rep);
                        configs.Add(i, rep.CloudTenantId);
                    }
                }
                catch (Exception)
                {
                }
            }
            return configs;
        }

        public static bool DeletePremiseStoreConfigurations(Dictionary<int, string> list)
        {
            bool success = true;
            foreach (KeyValuePair<int, string> entry in list)
            {
                if (!DeletePremiseStorCofiguration(entry.Value))
                { success = false; }
            }
            return success;
        }

        public static bool DeletePremiseStorCofiguration(string tenantId)
        {
            bool success = false;
            try
            {
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                    "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.DeleteConfiguration(tenantId);
                    success = true;
                }
            }
            catch (Exception)
            {
            }
            return success;
        }

        private static String _sandboxDir = null;
        private static String FindSandboxDir(String dir)
        {
            String result = String.Empty;

            if (_sandboxDir == null)
            {
                if (!String.IsNullOrEmpty(dir))
                {
                    DirectoryInfo di = new DirectoryInfo(dir);

                    // use LibraryConfig-Sandbox.xml existence as the sentinel to indicate where the root of the sandbox is
                    var sandboxFile = di.GetFiles("LibraryConfig-Sandbox.xml", SearchOption.AllDirectories).SingleOrDefault();
                    if (sandboxFile != null)
                    {
                        _sandboxDir = sandboxFile.DirectoryName;
                    }
                    else
                    {
                        if (di.Parent != null)
                        {
                            _sandboxDir = FindSandboxDir(di.Parent.FullName);
                        }
                    }
                }
            }

            return _sandboxDir;
        }

        /// <summary>
        /// Invoke a LibraryConfig action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        private static Boolean InvokeLibraryConfigAction(String action, Int32 timeoutInMS)
        {
            Boolean result = false;

            using (var p = new Process())
            {
                // when executing tests in a desktop build, they are loaded from $(SolutionRoot)\Libraries\CRE\CloudConnector\TestResults\<timestamped id>\Out
                // when executing tests in a TFS server build, they are loaded from $(SolutionRoot)\..\TestResults\<timestamped id>\Out
                String sandboxDir = FindSandboxDir(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                String libraryConfigPath = Path.GetFullPath(Path.Combine(sandboxDir, @"Libraries\CM\CloudConnector\LibraryConfig-Library.xml"));
                p.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"LibraryConfigTool.exe");
                p.StartInfo.Arguments = String.Format("/sandbox:\"{0}\" /lc:\"{1}\" /a:{2} /d:SkyfireProduct=SDC", sandboxDir, libraryConfigPath, action);
                p.StartInfo.WorkingDirectory = Path.Combine(sandboxDir, @"Libraries\CM\CloudConnector");
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit(timeoutInMS);
                if (p.ExitCode == 0)
                {
                    result = true;
                }
            }

            return result;
        }

        private static String QueryLibraryConfig(String query)
        {
            String result;

            using (var p = new Process())
            {
                // when executing tests in a desktop build, they are loaded from $(SolutionRoot)\Libraries\CRE\CloudConnector\TestResults\<timestamped id>\Out
                // when executing tests in a TFS server build, they are loaded from $(SolutionRoot)\..\TestResults\<timestamped id>\Out
                String sandboxDir = FindSandboxDir(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                String libraryConfigPath = Path.GetFullPath(Path.Combine(sandboxDir, @"Libraries\CM\CloudConnector\LibraryConfig-Library.xml"));
                p.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"LibraryConfigTool.exe");
                p.StartInfo.Arguments = String.Format("/sandbox:\"{0}\" /lc:\"{1}\" /q:{2} /d:SkyfireProduct=SDC", sandboxDir, libraryConfigPath, query);
                p.StartInfo.WorkingDirectory = Path.Combine(sandboxDir, @"Libraries\CM\CloudConnector");
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                result = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }

            return result;
        }


        /// <summary>
        /// Copy the Configuration SDF to enable restore of the database.
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        private static Boolean CopyConfigurationSDFToRuntimeFiles(Int32 timeoutInMS)
        { return InvokeLibraryConfigAction("CopyConfigurationSDF", timeoutInMS); }

        /// <summary>
        /// Copy the Log SDF to enable restore of the database.
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        private static Boolean CopyLogSDFToRuntimeFiles(Int32 timeoutInMS)
        { return InvokeLibraryConfigAction("CopyLogSDF", timeoutInMS); }

        /// <summary>
        /// Copy the Queue SDF to enable restore of the database.
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        private static Boolean CopyQueueSDFToRuntimeFiles(Int32 timeoutInMS)
        { return InvokeLibraryConfigAction("CopyQueueSDF", timeoutInMS); }

        private static Boolean ActivateForProduct(Int32 timeoutInMS)
        { return InvokeLibraryConfigAction("ActivateForProduct", timeoutInMS); }

        private static String GetConfigurationSDFDestinationFilePath()
        {
            if (String.IsNullOrEmpty(_configurationSDFDestinationFilePath))
            {
                _configurationSDFDestinationFilePath = QueryLibraryConfig("$(ConfigurationSDFDestinationFile)");
            }
            return _configurationSDFDestinationFilePath;
        }

        private static String GetConfigurationSDFUnitTestBaselineFilePath()
        {
            if (String.IsNullOrEmpty(_configurationSDFUnitTestBaselineFilePath))
            {
                _configurationSDFUnitTestBaselineFilePath = QueryLibraryConfig("$(ConfigurationSDFUnitTestBaselineFile)");
            }
            return _configurationSDFUnitTestBaselineFilePath;
        }

        private static String GetLogSDFDestinationFilePath()
        {
            if (String.IsNullOrEmpty(_logSDFDestinationFilePath))
            {
                _logSDFDestinationFilePath = QueryLibraryConfig("$(LogSDFDestinationFile)");
            }
            return _logSDFDestinationFilePath;
        }

        private static String GetLogSDFUnitTestBaselineFilePath()
        {
            if (String.IsNullOrEmpty(_logSDFUnitTestBaselineFilePath))
            {
                _logSDFUnitTestBaselineFilePath = QueryLibraryConfig("$(LogSDFUnitTestBaselineFile)");
            }
            return _logSDFUnitTestBaselineFilePath;
        }

        private static String GetQueueSDFDestinationFilePath()
        {
            if (String.IsNullOrEmpty(_queueSDFDestinationFilePath))
            {
                _queueSDFDestinationFilePath = QueryLibraryConfig("$(QueueSDFDestinationFile)");
            }
            return _queueSDFDestinationFilePath;
        }

        private static String GetQueueSDFUnitTestBaselineFilePath()
        {
            if (String.IsNullOrEmpty(_queueSDFUnitTestBaselineFilePath))
            {
                _queueSDFUnitTestBaselineFilePath = QueryLibraryConfig("$(QueueSDFUnitTestBaselineFile)");
            }
            return _queueSDFUnitTestBaselineFilePath;
        }

        private static String _configurationSDFDestinationFilePath;
        private static String _configurationSDFUnitTestBaselineFilePath;
        private static String _logSDFDestinationFilePath;
        private static String _logSDFUnitTestBaselineFilePath;
        private static String _queueSDFDestinationFilePath;
        private static String _queueSDFUnitTestBaselineFilePath;

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum UnitTestSetupFlags
        {
            /// <summary>
            /// No UnitTestSetupFlags (default value automatically initialized by runtime)
            /// </summary>
            None = 0x00,

            /// <summary>
            /// stop the hosting framework service
            /// </summary>
            StopHostingFx = 0x01,

            /// <summary>
            /// copy the baseline premise store SDF to runtime files
            /// </summary>
            CopySDF = 0x02,

            /// <summary>
            /// set the mock servicer to be the product plugin
            /// </summary>
            ActivateForProduct = 0x04,

            /// <summary>
            /// start the hosting framework service
            /// </summary>
            StartHostingFx = 0x08,

            /// <summary>
            /// 
            /// </summary>
            ForceRunAsService = 0x10,

            /// <summary>
            /// Run the service as a specific user
            /// </summary>
            RunAsSpecifiedUser = 0x20,

            /// <summary>
            /// Undo any effects of uninstalling a service and re-installing as a test user
            /// </summary>
            UndoServiceRunAsSpecifiedUser = 0x40,

            /// <summary>
            /// 
            /// </summary>
            ResetAndEnsureStopped = StopHostingFx | CopySDF | ActivateForProduct,

            /// <summary>
            /// 
            /// </summary>
            ResetAndEnsureRunning = StopHostingFx | CopySDF | ActivateForProduct | StartHostingFx
        }

        [Flags]
        public enum UnitTestSetupConnectorMonitorServiceFlags
        {
            /// <summary>
            /// No UnitTestSetupFlags (default value automatically initialized by runtime)
            /// </summary>
            None = 0x00,

            /// <summary>
            /// stop the hosting framework service
            /// </summary>
            StopHostingFx = 0x01,

            /// <summary>
            /// start the hosting framework service
            /// </summary>
            StartHostingFx = 0x02,

            /// <summary>
            /// 
            /// </summary>
            ForceRunAsService = 0x04,

            /// <summary>
            /// 
            /// </summary>
            ResetAndEnsureStopped = StopHostingFx,

            /// <summary>
            /// 
            /// </summary>
            ResetAndEnsureRunning = StopHostingFx | StartHostingFx
        }

        private static readonly String CONNECTOR_SERVICE_SCHEDULED_TASK_NAME = "UnitTestDriven_ConnectorServiceHostingFx";
        private static readonly String CONNECTOR_MONITOR_SERVICE_SCHEDULED_TASK_NAME = "UnitTestDriven_ConnectorMonitorServiceHostingFx";
        private static readonly String _schtasksExeFilePath = Environment.ExpandEnvironmentVariables(@"%windir%\system32\schtasks.exe");

        private static void CopySDF(String baseline, String destination, Func<Int32, Boolean> copyFromSourceFunc, Action<String, String, PremiseConfigurationRecord> setConfigurationValues)
        {
            if (!File.Exists(baseline))
            {
                TraceUtils.WriteLine(String.Format("UnitTestSetup: Creating {0} from baseline", Path.GetFileName(destination)));

                copyFromSourceFunc(60 * 1000);
                if (setConfigurationValues != null)
                {
                    ActivateMockCloudConfiguration(setConfigurationValues);
                }
                File.Copy(destination, baseline, true);
            }

            if (File.GetLastWriteTimeUtc(destination) > File.GetLastWriteTimeUtc(baseline))
            {
                TraceUtils.WriteLine(String.Format("UnitTestSetup: Replacing modified {0} with baseline", Path.GetFileName(destination)));

                File.Copy(baseline, destination, true);
            }

        }

        /// <summary>
        /// Types of services we can start/stop
        /// </summary>
        private enum TestServiceType
        {
            /// <summary>
            /// The connector service
            /// </summary>
            Connector = 0,

            /// <summary>
            /// The monitor service
            /// </summary>
            Monitor
        }

        /// <summary>
        /// Helper function to reset the local machine state; useful for unit testing.
        /// Will set configuration to mock cloud by default, if we are copying the SDF.
        /// </summary>
        /// <param name="flags"></param>
        public static void UnitTestSetup(UnitTestSetupFlags flags)
        {
            UnitTestSetup(flags, UnitTestSetupConnectorMonitorServiceFlags.None, SetConfigValuesForMockCloud);
        }

        /// <summary>
        /// Helper function to reset the local machine state; useful for unit testing.
        /// Will set configuration to mock cloud by default, if we are copying the SDF.
        /// Provides both flags for connector and monitor services
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="connectorMonitorServiceFlags"></param>
        public static void UnitTestSetup(
            UnitTestSetupFlags flags,
            UnitTestSetupConnectorMonitorServiceFlags connectorMonitorServiceFlags)
        {
            UnitTestSetup(flags, connectorMonitorServiceFlags, SetConfigValuesForMockCloud);
        }

        /// <summary>
        /// Helper function to reset the local machine state; useful for unit testing.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="connectorMonitorServiceFlags"> </param>
        /// <param name="setConfigurationValues">Method to set the configuration values if we are using CopySDF. Null will not set configuration values.</param>
        public static void UnitTestSetup(
            UnitTestSetupFlags flags,
            UnitTestSetupConnectorMonitorServiceFlags connectorMonitorServiceFlags,
            Action<String, String, PremiseConfigurationRecord> setConfigurationValues)
        {
            // STOP MONITOR SERVICE
            if ((connectorMonitorServiceFlags & UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx) != UnitTestSetupConnectorMonitorServiceFlags.None)
            {
                try
                {
                    if (ConnectorMonitorServiceUtils.IsServiceReady())
                    {
                        Trace.WriteLine("UnitTestSetup:  Stopping Connector Monitor Service HostingFx");

                        if ((connectorMonitorServiceFlags & UnitTestSetupConnectorMonitorServiceFlags.ForceRunAsService) != UnitTestSetupConnectorMonitorServiceFlags.None || RunHostingFxAsService())
                        {
                            ConnectorMonitorServiceUtils.StopService(TIMEOUT_IN_MS);
                            using (var logger = new SimpleTraceLogger())
                            {
                                if (ConnectorMonitorServiceUtils.IsServiceRunning(logger))
                                {
                                    ConnectorMonitorServiceUtils.KillService(logger);
                                }
                            }
                        }
                        else
                        {
                            // Signal console to stop
                            ConsoleStop(TestServiceType.Monitor);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceUtils.WriteLine("Exception during attempt to StopHostingFx (Monitor): {0}", ex.ExceptionAsString());
                }

                using (var logger = new SimpleTraceLogger())
                {
                    if (ConnectorMonitorServiceUtils.IsServiceProcessRunning())
                    {
                        Trace.WriteLine("UnitTestSetup:  Connector Monitor Service Process still alive after attempts to terminate it polietly");
                        ConnectorMonitorServiceUtils.KillService(logger);
                    }
                }
            }


            // STOP CONNECTOR SERVICE
            if ((flags & UnitTestSetupFlags.StopHostingFx) != UnitTestSetupFlags.None)
            {
                try
                {
                    if (ConnectorServiceUtils.IsServiceReady())
                    {
                        TraceUtils.WriteLine("UnitTestSetup: Stopping Connector Service HostingFx");

                        if ((flags & UnitTestSetupFlags.ForceRunAsService) != UnitTestSetupFlags.None || RunHostingFxAsService())
                        {
                            ConnectorServiceUtils.StopService(TIMEOUT_IN_MS);

                            using (var logger = new SimpleTraceLogger())
                            {
                                if (ConnectorServiceUtils.IsServiceRunning(logger))
                                {
                                    ConnectorServiceUtils.KillService(logger);
                                }
                            }
                        }
                        else
                        {
                            // Signal console to stop
                            ConsoleStop(TestServiceType.Connector);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceUtils.WriteLine("Exception during attempt to StopHostingFx (Connector): {0}", ex.ExceptionAsString());
                }

                using (var logger = new SimpleTraceLogger())
                {
                    if (ConnectorServiceUtils.IsServiceProcessRunning())
                    {
                        Trace.WriteLine("UnitTestSetup:  Connector Service Process still alive after attempts to terminate it polietly");
                        ConnectorServiceUtils.KillService(logger);
                    }
                }
            }

            // RESTORE ANY PREVIOUS UNINSTALL/REINSTALL OF SERVICE
            if ((flags & UnitTestSetupFlags.UndoServiceRunAsSpecifiedUser) != UnitTestSetupFlags.None)
            {
                // Service mode only
                if ((flags & UnitTestSetupFlags.ForceRunAsService) != UnitTestSetupFlags.None ||
                    RunHostingFxAsService())
                {
                    // Get the current service account
                    string currentServiceAccountUser = null;
                    try
                    {
                        currentServiceAccountUser = ConnectorServiceUtils.GetServiceAccountUserName();
                    }
                    catch
                    {
                        // Error reading registry, use default account
                    }

                    // Only restore if different than the original
                    if (string.IsNullOrEmpty(_originalServiceAccountUser) ||
                        string.IsNullOrEmpty(currentServiceAccountUser) ||
                        !currentServiceAccountUser.Equals(_originalServiceAccountUser,
                                                          StringComparison.InvariantCultureIgnoreCase))
                    {

                        // We can only restore stock accounts, since otherwise we will need a password
                        // Get the stock account to restore to
                        string connectorServiceStockUser = GetStockAccountToRestoreTo();

                        // Uninstall the existing service and re-install with the desired stock account
                        UninstallAndReInstallAs(connectorServiceStockUser, string.Empty);
                    }
                }
            }

            // RUN CONNECTOR AS SPECIFIED USER (UNINSTALL/REINSTALL FOR SERVICE MODE)
            else if ((flags & UnitTestSetupFlags.RunAsSpecifiedUser) != UnitTestSetupFlags.None)
            {
                // Get the relevant service account info has been provided
                string connectorServiceAccountUser, connectorServiceAccountPassword;
                GetDataForRunAsSpecifiedUser(out connectorServiceAccountUser, out connectorServiceAccountPassword);

                // Only proceed if at least user name was provided
                if (!String.IsNullOrEmpty(connectorServiceAccountUser))
                {
                    // Service mode
                    if ((flags & UnitTestSetupFlags.ForceRunAsService) != UnitTestSetupFlags.None ||
                        RunHostingFxAsService())
                    {
                        // Uninstall the existing service and re-install with the desired account
                        UninstallAndReInstallAs(
                            connectorServiceAccountUser,
                            connectorServiceAccountPassword);
                    }

                    // Non service mode
                    else
                    {
                        // Cannot run as specified user in console mode!
                        // The app data folder is set up at service install time, and
                        // Since we're not setting up the service we will have a mismatch
                        throw new ArgumentException(
                            "Cannot run unit tests both in console mode and as a specified user!");
                    }
                }
            }


            // ACTIVATE
            if ((flags & UnitTestSetupFlags.ActivateForProduct) != UnitTestSetupFlags.None)
            {
                TestUtils.ActivateForProduct(TIMEOUT_IN_MS);
            }


            // COPY SDF FILES
            if ((flags & UnitTestSetupFlags.CopySDF) != UnitTestSetupFlags.None)
            {
                CopySDF(GetConfigurationSDFUnitTestBaselineFilePath(), GetConfigurationSDFDestinationFilePath(), TestUtils.CopyConfigurationSDFToRuntimeFiles, setConfigurationValues);
                CopySDF(GetLogSDFUnitTestBaselineFilePath(), GetLogSDFDestinationFilePath(), TestUtils.CopyLogSDFToRuntimeFiles, null);
                CopySDF(GetQueueSDFUnitTestBaselineFilePath(), GetQueueSDFDestinationFilePath(), TestUtils.CopyQueueSDFToRuntimeFiles, null);
            }


            // START CONNECTOR SERVICE
            if ((flags & UnitTestSetupFlags.StartHostingFx) != UnitTestSetupFlags.None)
            {
                if (!ConnectorServiceUtils.IsServiceReady())
                {
                    TraceUtils.WriteLine("UnitTestSetup: Starting Connector Service HostingFx");

                    if ((flags & UnitTestSetupFlags.ForceRunAsService) != UnitTestSetupFlags.None || RunHostingFxAsService())
                    {
                        ConnectorServiceUtils.StartService(TIMEOUT_IN_MS, 0);
                    }
                    else
                    {
                        // Signal the console to start
                        ConsoleStart(TestServiceType.Connector, @"NT AUTHORITY\SYSTEM");
                    }
                }
            }


            // START MONITOR SERVICE
            if ((connectorMonitorServiceFlags & UnitTestSetupConnectorMonitorServiceFlags.StartHostingFx) != UnitTestSetupConnectorMonitorServiceFlags.None)
            {
                if (!ConnectorMonitorServiceUtils.IsServiceReady())
                {
                    TraceUtils.WriteLine("UnitTestSetup: Starting Connector Monitor HostingFx");

                    if ((connectorMonitorServiceFlags & UnitTestSetupConnectorMonitorServiceFlags.ForceRunAsService) != UnitTestSetupConnectorMonitorServiceFlags.None || RunHostingFxAsService())
                    {
                        ConnectorMonitorServiceUtils.StartService(TIMEOUT_IN_MS, 0);
                    }
                    else
                    {
                        ConsoleStart(TestServiceType.Monitor, @"NT AUTHORITY\LOCAL SERVICE");
                    }
                }
            }
        }

        /// <summary>
        /// Given the original service account user, determine if it matches a stock account
        /// If not, provide a default stock account
        /// </summary>
        /// <returns></returns>
        private static string GetStockAccountToRestoreTo()
        {
            // Get the default
            string stockAccountParam = StockAccountUtils.GetHostingFrameworkParamForStockAccountType(
                ConnectorRegistryUtils.ConnectorServiceStockAccount);

            // Check if we were not set to some other stock account before running the unit tests
            string tmpStockAccountParam = ConvertToTokenIfStockAccount(_originalServiceAccountUser);
            if (!string.IsNullOrEmpty(tmpStockAccountParam))
            {
                stockAccountParam = tmpStockAccountParam;
            }

            return stockAccountParam;
        }

        /// <summary>
        /// Convert the user name to the appropriate stock account token if it is in fact a stock account
        /// Otherwise return null
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static string ConvertToTokenIfStockAccount(string user)
        {
            // Convert
            string result =
                StockAccountUtils.GetHostingFrameworkParamForStockAccountType(
                    StockAccountUtils.GetStockAccountFromLoginString(user));

            return result;
        }

        /// <summary>
        /// Common code for stopping the service using consolestop
        /// </summary>
        /// <param name="serviceType"></param>
        private static void ConsoleStop(TestServiceType serviceType)
        {
            string taskName, exePath;
            GetDataForServiceType(serviceType, out taskName, out exePath);

            using (var p = new Process())
            {
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = "/consolestop";
                p.Start();
                p.WaitForExit(TIMEOUT_IN_MS);
            }

            WaitForServiceProcessNotRunning(serviceType);

            DeleteScheduledTask(taskName);
        }

        /// <summary>
        /// Common code for starting the service using consolerun
        /// Provide a default account if none specified
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="serviceAccountUser"></param>
        /// <param name="serviceAccountPassword"></param>
        private static void ConsoleStart(
            TestServiceType serviceType,
            string serviceAccountUser,
            string serviceAccountPassword = "")
        {
            // Check that a user was provided
            if (string.IsNullOrEmpty(serviceAccountUser))
            {
                throw new ArgumentException("ConsoleStart(): Service account user must be provided");
            }

            string taskName, exePath;
            GetDataForServiceType(serviceType, out taskName, out exePath);

            // invoke the Hosting FX using a scheduled task so that it can be run using the same
            // credentials as it uses when running as a service (i.e., NETWORK SERVICE)
            DateTime oneMinuteAgo = DateTime.Now.Subtract(new TimeSpan(0, 1, 0));
            String arguments = String.Format(
                @"/create /RL highest /RU ""{0}"" /SC once /ST {1} /TN {2} /TR ""'{3}' /consolerun""",
                serviceAccountUser,
                oneMinuteAgo.ToString("HH:mm"),
                taskName,
                exePath);

            // Append the password if one was provided
            if (!string.IsNullOrEmpty(serviceAccountPassword))
            {
                arguments = String.Concat(arguments, String.Format(@" /RP ""{0}"" ", serviceAccountPassword));
            }

            // Create the scheduled task to run as the specified user
            CreateScheduledTask(taskName, arguments);
            RunScheduledTask(taskName);

            WaitForServiceReady(serviceType);
        }

        /// <summary>
        /// Uninstall existing service and re-install as the specified user
        /// </summary>
        /// <param name="connectorServiceAccountUser"></param>
        /// <param name="connectorServiceAccountPassword"></param>
        public static void UninstallAndReInstallAs(
            string connectorServiceAccountUser,
            string connectorServiceAccountPassword)
        {
            // Convert user name to token for stock account if necessry
            string tmpConnectorServiceAccountUser = ConvertToTokenIfStockAccount(connectorServiceAccountUser);
            if (!string.IsNullOrEmpty(tmpConnectorServiceAccountUser))
            {
                connectorServiceAccountUser = tmpConnectorServiceAccountUser;
            }

            using (var logger = new SimpleTraceLogger())
            {
                if (ConnectorServiceUtils.IsServiceRegistered(logger))
                {
                    // For service mode, store the original service account user
                    // Since we will be uninstalling and re-installing as a different user.
                    // That way we can make a best attempt to restore to our original state.
                    if (string.IsNullOrEmpty(_originalServiceAccountUser))
                    {
                        _originalServiceAccountUser = ConnectorServiceUtils.GetServiceAccountUserName();
                    }

                    TraceUtils.WriteLine("UnitTestSetup: Uninstalling Connector Service HostingFx");

                    // Uninstall service
                    ConnectorServiceUtils.UninstallService(TIMEOUT_IN_MS);
                }
            }

            TraceUtils.WriteLine(String.Format(
               "UnitTestSetup: Installing Connector Service HostingFx as user '{0}'",
               connectorServiceAccountUser));

            // Re-install service
            ConnectorServiceUtils.InstallService(
                TIMEOUT_IN_MS,
                connectorServiceAccountUser,
                connectorServiceAccountPassword.ToSecureString(),
                true);
        }

        /// <summary>
        /// Wait for a service of the given type to not be ready
        /// </summary>
        /// <param name="serviceType"></param>
        private static void WaitForServiceProcessNotRunning(TestServiceType serviceType)
        {
            switch (serviceType)
            {
                case TestServiceType.Connector:
                    ConnectorServiceUtils.WaitForServiceProcessNotRunning(TIMEOUT_IN_MS, SLEEP_IN_MS);
                    break;
                case TestServiceType.Monitor:
                    ConnectorMonitorServiceUtils.WaitForServiceProcessNotRunning(TIMEOUT_IN_MS, SLEEP_IN_MS);
                    break;
                default:
                    throw new ArgumentException(
                        String.Format("WaitForServiceProcessNotRunning: Unrecognized service type '{0}'",
                        Enum.GetName(typeof(TestServiceType), serviceType)));
            }
        }

        /// <summary>
        /// Wait for a service of the given type to be ready
        /// </summary>
        /// <param name="serviceType"></param>
        private static void WaitForServiceReady(TestServiceType serviceType)
        {
            switch (serviceType)
            {
                case TestServiceType.Connector:
                    ConnectorServiceUtils.WaitForServiceReady(TIMEOUT_IN_MS, SLEEP_IN_MS);
                    break;
                case TestServiceType.Monitor:
                    ConnectorMonitorServiceUtils.WaitForServiceReady(TIMEOUT_IN_MS, SLEEP_IN_MS);
                    break;
                default:
                    throw new ArgumentException(
                        String.Format("WaitForServiceReady: Unrecognized service type '{0}'",
                        Enum.GetName(typeof(TestServiceType), serviceType)));
            }
        }

        /// <summary>
        /// Get service type specific data
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="taskName"></param>
        /// <param name="exePath"></param>
        private static void GetDataForServiceType(
            TestServiceType serviceType,
            out string taskName,
            out string exePath)
        {
            switch (serviceType)
            {
                case TestServiceType.Connector:
                    taskName = CONNECTOR_SERVICE_SCHEDULED_TASK_NAME;
                    exePath = ConnectorServiceUtils.ServiceExeFilePath;
                    break;
                case TestServiceType.Monitor:
                    taskName = CONNECTOR_MONITOR_SERVICE_SCHEDULED_TASK_NAME;
                    exePath = ConnectorMonitorServiceUtils.ServiceExeFilePath;
                    break;
                default:
                    throw new ArgumentException(
                        String.Format("GetDataForServiceType: Unrecognized service type '{0}'",
                        Enum.GetName(typeof(TestServiceType), serviceType)));
            }
        }

        public static Boolean RunHostingFxAsService()
        {
            Boolean result = false;

            String unitTestAsService = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_UNIT_TEST_HOSTING_FX_AS_SERVICE", EnvironmentVariableTarget.Machine);
            if (!String.IsNullOrEmpty(unitTestAsService) && unitTestAsService == "1")
            {
                result = true;
            }

            return result;
        }

        public static void GetDataForRunAsSpecifiedUser(out string user, out string password)
        {
            user = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_UNIT_TEST_SERVICE_ACCOUNT_NAME", EnvironmentVariableTarget.Machine);
            password = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_UNIT_TEST_SERVICE_ACCOUNT_PASSWORD", EnvironmentVariableTarget.Machine);
        }

        private static void CreateScheduledTask(String taskName, String arguments)
        {
            DeleteScheduledTask(taskName);
            ScheduledTaskShim(arguments);
        }

        private static void RunScheduledTask(String taskName)
        {
            String taskArguments = String.Format(@"/run /I /TN {0}", taskName);
            ScheduledTaskShim(taskArguments);
        }

        private static void DeleteScheduledTask(String taskName)
        {
            String taskArguments = String.Format(@"/delete /F /TN {0}", taskName);
            ScheduledTaskShim(taskArguments);
        }

        private static void ScheduledTaskShim(String taskArguments)
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = _schtasksExeFilePath;
                p.StartInfo.Arguments = taskArguments;
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(_schtasksExeFilePath);
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.Verb = "runas";
                p.Start();
                p.BeginOutputReadLine();
                String stdError = p.StandardError.ReadToEnd();
                p.WaitForExit();
            }
        }


        /// <summary>
        /// Inject the mock cloud configuration into the database so we will use it.
        /// </summary>
        private static void ActivateMockCloudConfiguration(Action<String, String, PremiseConfigurationRecord> setConfigurationValues)
        {
            using (var logger = new SimpleTraceLogger())
            {
                PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(logger);

                // Create canned tenants if they don't already exist
                int index = 1;
                foreach (String tenantId in CannedTenantIds)
                {
                    PremiseConfigurationRecord ipc = factory.GetEntryByTenantId(tenantId);

                    if (ipc == null)
                    {
                        ipc = factory.CreateNewEntry();

                        setConfigurationValues(tenantId, "Test", ipc);

                        string backOfficeCompanyName = String.Format("BackOffice{0}", index.ToString());
                        //string backOfficeConnectionInformation = String.Format("C:\\DataFolder{0}", index.ToString());

                        ipc.BackOfficeCompanyName = backOfficeCompanyName;
                        //TODO: JSB update this for the BackOfficeConnectionCredentials

                        // Add it
                        factory.AddEntry(ipc);
                    }

                    ++index;
                }
            }
        }

        /// <summary>
        /// Default setup for testing premise configuration record.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="record"></param>
        public static void SetConfigValuesForMockCloud(String tenantId, String premiseKey, PremiseConfigurationRecord record)
        {
            record.CloudTenantId = tenantId;
            record.CloudPremiseKey = premiseKey;
            record.CloudConnectionEnabledToReceive = true;

            //TODO: JSB update this for the BackOfficeConnectionCredentials
            //record.BackOfficeConnectionInformation = "C:\\DataFolder";
            //record.BackOfficeConnectionInformationDisplayable = "C:\\DataFolder";
            //record.BackOfficeUserName = "USER";
            //record.BackOfficeUserPassword = "PASSWORD";
            record.ConnectorPluginId = "Mock";
            record.BackOfficeProductName = "Mock Back Office Product";
            record.BackOfficeAllowableConcurrentExecutions = 1;
            //TODO: JSB make this the correct Connection Credentials value for mock back office.
            record.BackOfficeConnectionCredentials = string.Empty;

            record.PremiseAgent = "unit test premise agent";

            record.SiteAddress = "http://localhost:8002";
            record.MinCommunicationFailureRetryInterval = (Int32)TimeSpan.FromSeconds(10).TotalMilliseconds;
            record.MaxCommunicationFailureRetryInterval = (Int32)TimeSpan.FromSeconds(30).TotalMilliseconds;

            // TODO: re-evaluate:  values? where should these defaults be persisted? should the defaults be updateable?
            record.SentDocumentStoragePolicy = 0;   // Not touched
            record.SentDocumentStorageDays = 30;    // 30 Days
            record.SentDocumentStorageMBs = 1024;   // 1 GB
            record.SentDocumentFolderName = "Sent"; // Sent

            record.BackOfficeAllowableConcurrentExecutions = 1;

            // NOTE: If this is updated, consider updating SetDefaultPremiseConfigurationRecordValues() (Libraries\CRE\CloudConnector\Sage.Connector.Data\PremiseConfigurationRecordFactory.cs).
        }

        private static string _originalServiceAccountUser = string.Empty;
        private static readonly int TIMEOUT_IN_MS = 120 * 1000;
        private static readonly int SLEEP_IN_MS = 1000;
    }
}
