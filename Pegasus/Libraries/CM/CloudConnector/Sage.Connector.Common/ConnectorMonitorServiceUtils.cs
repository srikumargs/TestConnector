using System;
using System.IO;
using System.Reflection;
using System.Security;
using Sage.CRE.HostingFramework.Interfaces;

namespace Sage.Connector.Common
{
    /// <summary>
    /// General utility helper class for the Connector Monitor Service
    /// </summary>
    public class ConnectorMonitorServiceUtils : HostingFxUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override String GetServiceExeFilePath()
        {
            String result = String.Empty;

            // If the executing assembly is the Connector Monitor Service, then adjust teh path back to the Connector Service
            String executingAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (executingAssemblyLocation.EndsWith(@"Monitor\Tray"))
            {
               executingAssemblyLocation= executingAssemblyLocation.Replace(@"Monitor\Tray", @"Monitor\Service");
            }
            else if (!executingAssemblyLocation.EndsWith(@"Monitor\Service"))
            {
                executingAssemblyLocation = Path.GetFullPath(Path.Combine(executingAssemblyLocation, @"Monitor\Service"));
            }
            result = Path.Combine(executingAssemblyLocation, "Sage.CRE.HostingFramework.Service.exe");

            return result;
        }

        /// <summary>
        /// Gets the service name of the Hosting Framework
        /// </summary>
        public static String ServiceName
        { get { return GetInstance().GetServiceName(); } }

        /// <summary>
        /// Gets the service display name of the Hosting Framework
        /// </summary>
        public static String ServiceDisplayName
        { get { return GetInstance().GetServiceDisplayName(); } }

        /// <summary>
        /// Gets the file path of the Hosting Framework Service EXE
        /// </summary>
        public static String ServiceExeFilePath
        { get { return GetInstance().GetServiceExeFilePath(); } }

        /// <summary>
        /// Gets the port number being used by the CatalogService
        /// </summary>
        public static Int32 CatalogServicePortNumber
        { get { return GetInstance().GetCatalogServicePortNumber(); } }

        /// <summary>
        /// Gets the InstanceApplicationDataFolder for the Hosting Framework
        /// </summary>
        public static String InstanceApplicationDataFolder
        { get { return GetInstance().GetInstanceApplicationDataFolder(); } }

        /// <summary>
        /// Blocks current thread until the HostingFramework is ready, is needed if hosted services expect other hosted services in order to proceed
        /// </summary>
        /// <param name="timeoutInMS">Timeout (in milliseconds) that the method should block attempting to see if the HostingFramework is ready</param>
        /// <param name="sleepIntervalInMS">Sleep interval (in milliseconds) that the method should test the HostingFramework</param>
        public static void WaitForServiceReady(Int32 timeoutInMS, Int32 sleepIntervalInMS)
        { GetInstance().DoWaitForServiceReady(timeoutInMS, sleepIntervalInMS); }

        /// <summary>
        /// Blocks current thread until the HostingFramework is not ready
        /// </summary>
        /// <param name="timeoutInMS">Timeout (in milliseconds) that the method should block attempting to see if the HostingFramework is ready</param>
        /// <param name="sleepIntervalInMS">Sleep interval (in milliseconds) that the method should test the HostingFramework</param>
        public static void WaitForServiceProcessNotRunning(Int32 timeoutInMS, Int32 sleepIntervalInMS)
        { GetInstance().DoWaitForServiceProcessNotRunning(timeoutInMS, sleepIntervalInMS); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Boolean IsServiceReady()
        { return GetInstance().DoIsServiceReady(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Boolean IsServiceProcessRunning()
        { return GetInstance().DoIsServiceProcessRunning(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Boolean IsServiceRegistered(ILogging logging)
        { return GetInstance().DoIsServiceRegistered(logging); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Boolean IsServiceRunning(ILogging logging)
        { return GetInstance().DoIsServiceRunning(logging); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="delayedStart"></param>
        /// <returns></returns>
        public static ExitCode InstallService(Int32 timeoutInMS, string user, SecureString password, bool delayedStart)
        { return GetInstance().DoInstallService(timeoutInMS, user, password, delayedStart); }

        /// <summary>
        /// 
        /// </summary>
        public static void VerifyAndPatchExecutionPath()
        { GetInstance().DoVerifyAndPathImagePath(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <param name="startRetries"></param>
        /// <returns></returns>
        public static ExitCode StartService(Int32 timeoutInMS, Int32 startRetries)
        { return GetInstance().DoStartService(timeoutInMS, startRetries); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        public static Boolean StopService(Int32 timeoutInMS)
        { return GetInstance().DoStopService(timeoutInMS); }

        /// <summary>
        ///
        /// </summary>
        public static void KillService(ILogging logging)
        { GetInstance().DoKillService(logging); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInMS"></param>
        /// <returns></returns>
        public static Boolean UninstallService(Int32 timeoutInMS)
        { return GetInstance().DoUninstallService(timeoutInMS); }

        private static readonly ConnectorMonitorServiceUtils _instance = new ConnectorMonitorServiceUtils();
        private static ConnectorMonitorServiceUtils GetInstance()
        { return _instance; }
    }
}
