using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  -n/a
namespace Sage.CRE.HostingFramework.LinkedSource
{
    /// <summary>
    /// Simple typed exception to look for when waiting for service to be
    /// Ready or to not be ready is not achieved
    /// </summary>
    [Serializable]
    internal sealed class WaitForServiceException: Exception
    {
        public WaitForServiceException(string message)
            : base(message){}
    }

    internal static class ServiceUtils
    {
        /// <summary>
        /// Blocks current thread until the HostingFramework is ready, is needed if hosted services expect other hosted services in order to proceed
        /// </summary>
        /// <param name="mutexName"></param>
        /// <param name="timeoutInMS">Timeout (in milliseconds) that the method should block attempting to see if the HostingFramework is ready</param>
        /// <param name="sleepIntervalInMS">Sleep interval (in milliseconds) that the method should test the HostingFramework</param>
        /// <param name="logger">Logging method, if desired</param>
        public static void WaitForServiceMutexToBeSet(String mutexName, Int32 timeoutInMS, Int32 sleepIntervalInMS, Action<string> logger = null )
        {
            Int32 remainingWaitTimeInMS = timeoutInMS;
            while (true)
            {
                ConditionalLog(".", logger);
                Boolean createdNew;
                using (Mutex serviceMutex = new Mutex(false, mutexName, out createdNew, AllowEveryoneMutexSecurity))
                {
                    //Should not need to explicitly close this, the using will do it via IDispose.
                    //Closing this like this will likely double dispose the object and result in an ObjectDisposedExcpetion
                    //at least based on SA CA2202 which caught this.
                    //serviceMutex.Close();
                    if (createdNew == false)
                    {
                        ConditionalLog(Environment.NewLine, logger);
                        break;
                    }
                }
                remainingWaitTimeInMS -= sleepIntervalInMS;
                if (remainingWaitTimeInMS <= 0)
                {
                    throw new WaitForServiceException(String.Format("Failed to WaitForServiceMutexToBeSet({0}).", mutexName));
                }
                Thread.Sleep(sleepIntervalInMS);
            }
        }

        /// <summary>
        /// Li'l helper to log if there is a logging method
        /// </summary>
        /// <param name="content"></param>
        /// <param name="logger"></param>
        private static void ConditionalLog(string content, Action<string> logger)
        {
            if (logger != null)
            {
                logger(content);
            }
        }

        /// <summary>
        /// Blocks current thread until the HostingFramework is not ready
        /// </summary>
        /// <param name="mutexName"></param>
        /// <param name="timeoutInMS">Timeout (in milliseconds) that the method should block attempting to see if the HostingFramework is ready</param>
        /// <param name="sleepIntervalInMS">Sleep interval (in milliseconds) that the method should test the HostingFramework</param>
        /// <param name="logger">Logging method, if desired</param>
        public static void WaitForServiceMutexToBeReleased(String mutexName, Int32 timeoutInMS, Int32 sleepIntervalInMS, Action<string> logger = null )
        {
            Int32 remainingWaitTimeInMS = timeoutInMS;
            while (true)
            {
                ConditionalLog(".", logger);
                Boolean createdNew;
                using (Mutex serviceMutex = new Mutex(false, mutexName, out createdNew, AllowEveryoneMutexSecurity))
                {
                    //Should not need to explicitly close this, the using will do it via IDispose.
                    //Closing this like this will likely double dispose the object and result in an ObjectDisposedExcpetion
                    //at least based on SA CA2202 which caught this.
                    //serviceMutex.Close();
                    if (createdNew == true)
                    {
                        ConditionalLog(Environment.NewLine, logger);
                        break;
                    }
                }
                remainingWaitTimeInMS -= sleepIntervalInMS;
                if (remainingWaitTimeInMS <= 0)
                {
                    throw new WaitForServiceException(String.Format("Failed to WaitForServiceMutexToBeReleased({0}).", mutexName));
                }
                Thread.Sleep(sleepIntervalInMS);
            }
        }

        private static MutexSecurity AllowEveryoneMutexSecurity
        {
            get
            {
                MutexSecurity result = new MutexSecurity();
                result.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
                return result;
            }
        }

        /// <summary>
        /// Get the service account user name from the service registry
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string GetServiceAccountUserName(string serviceName)
        {
            string userName = null;

            if (!string.IsNullOrEmpty(serviceName))
            {
                // Get the registry key given the service name
                string regKey = String.Format(
                    CultureInfo.InvariantCulture, 
                    @"SYSTEM\CurrentControlSet\services\{0}", 
                    serviceName);

                // Get the string value from the service's registry key
                using (RegistryKey subKey = Registry.LocalMachine.OpenSubKey(regKey, false))
                {
                    if (subKey != null)
                    {
                        userName = (string) subKey.GetValue("ObjectName", null);
                    }
                    else
                    {
                        string errorMessage = String.Format(
                            System.Globalization.CultureInfo.InvariantCulture,
                            "Failed LocalMachine.OpenSubKey('{0}')",
                            regKey);

                        throw new Exception(errorMessage);
                    }
                }
            }

            return userName;
        }
    }
}
