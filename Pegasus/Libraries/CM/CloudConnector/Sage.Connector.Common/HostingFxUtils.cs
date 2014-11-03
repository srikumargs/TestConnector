using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using Sage.CRE.HostingFramework.Interfaces;

namespace Sage.Connector.Common
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class HostingFxUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract String GetServiceExeFilePath();

        /// <summary>
        /// Gets the service name of the Hosting Framework
        /// </summary>
        public String GetServiceName()
        {  return GetHostingFrameworkServiceConstants().Name; } 

        /// <summary>
        /// Gets the service display name of the Hosting Framework
        /// </summary>
        public String GetServiceDisplayName()
        { return GetHostingFrameworkServiceConstants().DisplayName; }

        /// <summary>
        /// Gets the port number being used by the CatalogService
        /// </summary>
        public Int32 GetCatalogServicePortNumber()
        { return GetHostingFrameworkServiceConstants().CatalogServicePortNumber; }

        /// <summary>
        /// Gets the InstanceApplicationDataFolder for the Hosting Framework
        /// </summary>
        public String GetInstanceApplicationDataFolder()
        { return GetHostingFrameworkServiceConstants().InstanceApplicationDataFolder; }

        #region Patching ImagePath After Upgrade Install

        private String ServiceRegistryKey
        {
            get
            {
                return String.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    @"SYSTEM\CurrentControlSet\services\{0}",
                    GetServiceName());
            }
        }

        private String DoGetImagePath()
        {
            string imagePath = null;
            if (!string.IsNullOrEmpty(GetServiceName()))
            {
                using (RegistryKey subKey = Registry.LocalMachine.OpenSubKey(ServiceRegistryKey, false))
                {
                    if (subKey != null)
                    {
                        imagePath = (string) subKey.GetValue("ImagePath", null);
                    }
                    else
                    {
                        string errorMessage = String.Format(
                            System.Globalization.CultureInfo.InvariantCulture,
                            "Failed LocalMachine.OpenSubKey('{0}')",
                            ServiceRegistryKey);

                        throw new Exception(errorMessage);
                    }
                }
            }
            return imagePath;
        }

        private void DoSetImagePath(string imagePath)
        {
            if (!string.IsNullOrEmpty(GetServiceName()))
            {
                using (RegistryKey subKey = Registry.LocalMachine.OpenSubKey(ServiceRegistryKey, true))
                {
                    if (subKey != null)
                    {
                        subKey.SetValue("ImagePath", imagePath);
                    }
                    else
                    {
                        string errorMessage = String.Format(
                            System.Globalization.CultureInfo.InvariantCulture,
                            "Failed LocalMachine.OpenSubKey('{0}')",
                            ServiceRegistryKey);

                        throw new Exception(errorMessage);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// During upgrade installs, if the installation path is changed
        /// the service image path needs to be re-patched
        /// </summary>
        /// <returns></returns>
        public void DoVerifyAndPathImagePath()
        {
            string servicePath = DoGetImagePath();
            string exePath = GetServiceExeFilePath();

            if (servicePath != exePath)
            {
                DoSetImagePath(exePath);
            }
        }

        /// <summary>
        /// Blocks current thread until the HostingFramework is ready, is needed if hosted services expect other hosted services in order to proceed
        /// </summary>
        /// <param name="timeoutInMS">Timeout (in milliseconds) that the method should block attempting to see if the HostingFramework is ready</param>
        /// <param name="sleepIntervalInMS">Sleep interval (in milliseconds) that the method should test the HostingFramework</param>
        public void DoWaitForServiceReady(Int32 timeoutInMS, Int32 sleepIntervalInMS)
        { Sage.CRE.HostingFramework.LinkedSource.ServiceUtils.WaitForServiceMutexToBeSet(GetHostingFrameworkServiceConstants().ServiceReadyMutexName, timeoutInMS, sleepIntervalInMS); }

        /// <summary>
        /// Blocks current thread until the HostingFramework is not ready
        /// </summary>
        /// <param name="timeoutInMS">Timeout (in milliseconds) that the method should block attempting to see if the HostingFramework is ready</param>
        /// <param name="sleepIntervalInMS">Sleep interval (in milliseconds) that the method should test the HostingFramework</param>
        public void DoWaitForServiceProcessNotRunning(Int32 timeoutInMS, Int32 sleepIntervalInMS)
        { Sage.CRE.HostingFramework.LinkedSource.ServiceUtils.WaitForServiceMutexToBeReleased(GetHostingFrameworkServiceConstants().ServiceProcessRunningMutexName, timeoutInMS, sleepIntervalInMS); }

        /// <summary>
        /// Get the service account user name for our hosting framework
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public String DoGetServiceAccountUserName(string serviceName)
        {
            return CRE.HostingFramework.LinkedSource.ServiceUtils.GetServiceAccountUserName(serviceName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean DoIsServiceReady()
        {
            Boolean result = false;

            try
            {
                DoWaitForServiceReady(0, 0);
                result = true;
            }
            catch (Exception)
            {
                // turn exception-oriented result into a boolean
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean DoIsServiceProcessRunning()
        {
            Boolean result = false;

            try
            {
                Sage.CRE.HostingFramework.LinkedSource.ServiceUtils.WaitForServiceMutexToBeSet(GetHostingFrameworkServiceConstants().ServiceProcessRunningMutexName, 0, 0);
                result = true;
            }
            catch (Exception)
            {
                // turn exception-oriented result into a boolean
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean DoIsServiceRegistered(ILogging logging)
        { return DoIsWindowsServiceInstalled(GetServiceName(), logging); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean DoIsServiceRunning(ILogging logging)
        { return DoIsWindowsServiceRunning(GetServiceName(), logging); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean DoIsServiceEnabled()
        { return DoIsWindowsServiceEnabled(GetServiceName()); }

        private Boolean DoIsWindowsServiceEnabled(String serviceName)
        {
            Boolean result = false;

            using (var reg = Registry.LocalMachine.OpenSubKey(String.Format(@"SYSTEM\CurrentControlSet\Services\{0}", serviceName), false))
            {
                Int32 startValue = Convert.ToInt32(reg.GetValue("Start"));

                // 2 - automatic
                // 3 - manual
                // 4 - disabled
                if (startValue == 2 || startValue == 3)
                {
                    result = true;
                }
            }

            return result;
        }

        private Boolean DoIsWindowsServiceInstalled(String serviceName, ILogging logging)
        {
            Boolean result = false;
            try
            {
                using (var serviceController = new System.ServiceProcess.ServiceController(serviceName))
                {
                    // access a property to determine if the service is installed
                    System.ServiceProcess.ServiceControllerStatus status = serviceController.Status;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                if (logging != null)
                {
                    logging.WriteError(null, Extensions.ExceptionAsString(ex));
                }
            }

            return result;
        }

        private Boolean DoIsWindowsServiceRunning(String serviceName, ILogging logging)
        {
            Boolean result = false;
            try
            {
                using (var serviceController = new System.ServiceProcess.ServiceController(serviceName))
                {
                    // determine if the service is running
                    result = (serviceController.Status == System.ServiceProcess.ServiceControllerStatus.Running);
                }
            }
            catch (Exception ex)
            {
                logging.WriteError(null, Extensions.ExceptionAsString(ex));
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="delayedStart"></param>
        /// <returns></returns>
        public ExitCode DoInstallService(Int32 timeoutInMS, string user, SecureString password, bool delayedStart)
        {
            // Default result to failed
            ExitCode result = ExitCode.Fail;

            // Setup optional install parameters string
            // User name
            string optionalParamsString = string.Empty;
            if (!string.IsNullOrEmpty(user))
            {
                optionalParamsString += string.Format(" /user=\"{0}\"", user);
            }

            // Password
            string unsecurePassword = ConvertToUnsecureString(password);
            if (!string.IsNullOrEmpty(unsecurePassword))
            {
                optionalParamsString += string.Format(" /password=\"{0}\"", unsecurePassword);
            }

            // Delayed start if set to automatic start type
            if (delayedStart)
            {
                optionalParamsString += " /delayedstart=\"true\"";
            }

            // Set the full arguments string
            string arguments = String.Concat("/silent /install ", optionalParamsString);

            // Create and run the process to install the HF service
            using (var p = new Process())
            {
                p.StartInfo.FileName = GetServiceExeFilePath();
                p.StartInfo.Arguments = arguments;
                p.Start();
                p.WaitForExit(timeoutInMS);
                if (p.HasExited)
                {
                    // Get the resulting exit code
                    result = GetExitCodeEnum(p.ExitCode);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <param name="startRetries"></param>
        /// <returns></returns>
        public ExitCode DoStartService(Int32 timeoutInMS, Int32 startRetries)
        {
            // Default result to failed
            ExitCode result = ExitCode.Fail;

            using (var p = new Process())
            {
                string arguments = "/silent /start";
                if (startRetries > 0)
                {
                    // Append the start retries param
                    arguments += string.Format(" /startretries={0}", startRetries);
                }

                p.StartInfo.FileName = GetServiceExeFilePath();
                p.StartInfo.Arguments = arguments;
                p.Start();
                p.WaitForExit(timeoutInMS);
                if (p.HasExited)
                {
                    // Get the resulting exit code
                    result = GetExitCodeEnum(p.ExitCode);
                    if (result == ExitCode.Success)
                    {
                        DoWaitForServiceReady(timeoutInMS, ConnectorRegistryUtils.HostingFxWaitForReadySleepInterval);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        public Boolean DoStopService(Int32 timeoutInMS)
        {
            Boolean result = false;

            using (var p = new Process())
            {
                p.StartInfo.FileName = GetServiceExeFilePath();
                p.StartInfo.Arguments = "/silent /stop";
                p.Start();
                p.WaitForExit(timeoutInMS);
                if (p.HasExited && GetExitCodeEnum(p.ExitCode) == ExitCode.Success)
                {
                    DoWaitForServiceProcessNotRunning(timeoutInMS, ConnectorRegistryUtils.HostingFxWaitForNotReadySleepInterval);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Used to absolutely guarantee service has been killed
        /// (intend usage for build server unit test ONLY and
        ///  ONLY after asking for it to stop)
        /// </summary>
        /// <returns></returns>
        public void DoKillService(ILogging logging)
        {
            var serviceExeFilePath = GetServiceExeFilePath().ToLowerInvariant();
            var serviceExeName = Path.GetFileNameWithoutExtension(GetServiceExeFilePath());
            Process[] services = Process.GetProcessesByName(serviceExeName);
            foreach (Process serviceProcess in services)
            {
                try
                {
                    if (serviceProcess.MainModule.FileName.ToLowerInvariant() == serviceExeFilePath)
                    {
                        serviceProcess.Kill();
                        serviceProcess.WaitForExit(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds));
                    }
                }
                catch (Exception ex)
                {
                    logging.ErrorTrace(this, ex.ExceptionAsString());

                    // Win32Exception - Cannot be terminated
                    // NotSupportedException - remote
                    // InvalidOperationException - Already exited
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Boolean DoIsServiceProcessRunning(String serviceName, ILogging logging)
        {
            var result = false;

            var serviceExeFilePath = GetServiceExeFilePath().ToLowerInvariant();
            var serviceExeName = Path.GetFileNameWithoutExtension(GetServiceExeFilePath());
            Process[] services = Process.GetProcessesByName(serviceExeName);
            foreach (Process serviceProcess in services)
            {
                try
                {
                    if (serviceProcess.MainModule.FileName.ToLowerInvariant() == serviceExeFilePath)
                    {
                        result = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    logging.ErrorTrace(this, ex.ExceptionAsString());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        public Boolean DoUninstallService(Int32 timeoutInMS)
        {
            Boolean result = false;

            using (var p = new Process())
            {
                p.StartInfo.FileName = GetServiceExeFilePath();
                p.StartInfo.Arguments = "/silent /uninstall";
                p.Start();
                p.WaitForExit(timeoutInMS);
                if (p.HasExited && GetExitCodeEnum(p.ExitCode) == ExitCode.Success)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieve the HostingFramework's ServiceConstants object
        /// </summary>
        /// <returns></returns>
        private Sage.CRE.HostingFramework.LinkedSource.ServiceConstants GetHostingFrameworkServiceConstants()
        {
            lock (_lockObject)
            {
                if (_serviceConstants == null)
                {
                    String hostingFrameworkExeFilePath = GetServiceExeFilePath();
                    _serviceConstants = Sage.CRE.HostingFramework.LinkedSource.ServiceConstantsFactory.GetConstants(hostingFrameworkExeFilePath);
                }
            }

            return _serviceConstants;
        }

        /// <summary>
        /// Convert the int32 exit code to a hosting framework exit code enum
        /// </summary>
        /// <param name="exitCodeInt"></param>
        /// <returns></returns>
        private ExitCode GetExitCodeEnum(Int32 exitCodeInt)
        {
            ExitCode result;
            if (!Enum.TryParse(Convert.ToString(exitCodeInt), false, out result))
            {
                result = ExitCode.Unknown;
            }
            return result;
        }

        /// <summary>
        /// Convert the secure string password used for installing into plain text
        /// Which is required by the hosting framework command line
        /// </summary>
        /// <param name="securePassword"></param>
        /// <returns></returns>
        private static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                return null;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        private Sage.CRE.HostingFramework.LinkedSource.ServiceConstants _serviceConstants;
        private readonly Object _lockObject = new object();
    }
}
