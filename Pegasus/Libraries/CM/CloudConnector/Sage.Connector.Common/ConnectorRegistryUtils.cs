using System;
using Sage.Connector.Common.Internal;

namespace Sage.Connector.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConnectorRegistryUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public static String InstallPath
        { get { return GetRegistry().InstallPath; } }

        /// <summary>
        /// 
        /// </summary>
        public static String BriefProductName
        { get { return GetRegistry().BriefProductName; } }
        
        /// <summary>
        /// 
        /// </summary>
        public static String ProductId
        { get { return GetRegistry().ProductId; } }

        /// <summary>
        /// 
        /// </summary>
        public static String ProductCode
        { get { return GetRegistry().ProductCode; } }

        /// <summary>
        /// 
        /// </summary>
        public static String FullProductName
        { get { return GetRegistry().FullProductName; } }

        /// <summary>
        /// 
        /// </summary>
        public static String ProductHelpBaseUrl
        { get { return GetRegistry().ProductHelpBaseUrl; } }

        /// <summary>
        /// 
        /// </summary>
        public static String ProductVersion
        { get { return GetRegistry().ProductVersion; } }

        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan LogRetentionPolicyThreshold
        { get { return GetRegistry().LogRetentionPolicyThreshold; } }

        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan StorageQueueMutexWaitTimeout
        { get { return GetRegistry().StorageQueueMutexWaitTimeout; } }

        /// <summary>
        /// 
        /// </summary>
        public static double StorageQueueProcessingTimeout
        { get { return GetRegistry().StorageQueueProcessingTimeout; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 CloudProxyRetryTimeout
        { get { return GetRegistry().CloudProxyRetryTimeout; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 HostingFxWaitForReadySleepInterval
        { get { return GetRegistry().HostingFxWaitForReadySleepInterval; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 HostingFxWaitForNotReadySleepInterval
        { get { return GetRegistry().HostingFxWaitForNotReadySleepInterval; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 BinderInvokerRetrySleepInterval
        { get { return GetRegistry().BinderInvokerRetrySleepInterval; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 MessagingServiceStopWorkerThreadsSleepInterval
        { get { return GetRegistry().MessagingServiceStopWorkerThreadsSleepInterval; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 MessagingServiceStopWorkerTimeoutInterval
        { get { return GetRegistry().MessagingServiceStopWorkerTimeoutInterval; } }

        /// <summary>
        /// 
        /// </summary>
        public static UInt32 ErrorResponseRetryMax
        { get { return GetRegistry().ErrorResponseRetryMax; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 InternetConnectivityStatusTcpConnectTimeout
        { get { return GetRegistry().InternetConnectivityStatusTcpConnectTimeout; } }

        /// <summary>
        /// 
        /// </summary>
        public static String SiteAddress
        { get { return GetRegistry().SiteAddress; } }        

        /// <summary>
        /// 
        /// </summary>
        public static Int32 FixedIntervalRetryCount
        { get { return GetRegistry().FixedIntervalRetryCount; } }

        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan FixedIntervalRetryInterval
        { get { return GetRegistry().FixedIntervalRetryInterval; } }

        /// <summary>
        /// 
        /// </summary>
        public static bool FixedIntervalFirstFastRetry
        { get { return GetRegistry().FixedIntervalFirstFastRetry; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 CrucialFixedIntervalRetryCount
        { get { return GetRegistry().CrucialFixedIntervalRetryCount; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 InitBackOfficeConnectionStateRetryCount
        { get { return GetRegistry().InitBackOfficeConnectionStateRetryCount; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 ExponentialBackoffRetryCount
        { get { return GetRegistry().ExponentialBackoffRetryCount; } }

        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan ExponentialBackoffMinBackoff
        { get { return GetRegistry().ExponentialBackoffMinBackoff; } }

        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan ExponentialBackoffMaxBackoff
        { get { return GetRegistry().ExponentialBackoffMaxBackoff; } }

        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan ExponentialBackoffDeltaBackoff
        { get { return GetRegistry().ExponentialBackoffDeltaBackoff; } }

        /// <summary>
        /// 
        /// </summary>
        public static bool ExponentialBackoffFirstFastRetry
        { get { return GetRegistry().ExponentialBackoffFirstFastRetry; } }

        /// <summary>
        /// 
        /// </summary>
        public static string DatabaseRecoveryBackupDirectory
        { get { return GetRegistry().DatabaseRecoveryBackupDirectory; } }

        /// <summary>
        /// The assembly file version of Sage.CRE.CloudConnector.Integration.Interfaces.dll that we 
        /// expect the backoffice to be using.  They return what they actually have through
        /// the IVersionInfo.GetInterfaceVersion()
        /// </summary>
        public static String MinimumConnectorIntegrationInterfacesVersion
        { get { return GetRegistry().MinimumConnectorIntegrationInterfacesVersion; } }

        /// <summary>
        /// 
        /// </summary>
        public static Guid ConnectorInstanceGuid { get { return GetRegistry().ConnectorInstanceGuid; }}

        /// <summary>
        /// The number of displayed subsystem health issues
        /// </summary>
        public static uint SubsystemHealthDisplayLimit
        { get { return GetRegistry().SubsystemHealthDisplayLimit; } }

        /// <summary>
        /// Interval for the connector UI get refresh data thread
        /// </summary>
        public static Int32 ConnectorGetRefreshDataInterval
        { get { return GetRegistry().ConnectorGetRefreshDataInterval; } }

        /// <summary>
        /// Interval for the connector UI apply refresh data timer
        /// </summary>
        public static Int32 ConnectorApplyRefreshDataInterval
        { get { return GetRegistry().ConnectorApplyRefreshDataInterval; } }

        /// <summary>
        /// Timeout for stopping the connector UI get refresh data thread
        /// </summary>
        public static Int32 ConnectorGetRefreshDataThreadTimeout
        { get { return GetRegistry().ConnectorGetRefreshDataThreadTimeout; } }

        /// <summary>
        /// The max timeout to wait for the connector service install
        /// </summary>
        public static Int32 ConnectorServiceInstallTimeout
        { get { return GetRegistry().ConnectorServiceInstallTimeout; } }

        /// <summary>
        /// The max timeout to wait for the connector service start
        /// </summary>
        public static Int32 ConnectorServiceStartTimeout
        { get { return GetRegistry().ConnectorServiceStartTimeout; } }

        /// <summary>
        /// The number of times to retry a failed wait for service ready call
        /// </summary>
        public static Int32 ConnectorServiceStartRetries
        { get { return GetRegistry().ConnectorServiceStartRetries; } }

        /// <summary>
        /// The max timeout to wait for the connector service to reach READY status
        /// </summary>
        public static Int32 ConnectorServiceWaitForReadyTimeout
        { get { return GetRegistry().ConnectorServiceWaitForReadyTimeout; } }

        /// <summary>
        /// The max timeout to wait for the monitor service install
        /// </summary>
        public static Int32 MonitorServiceInstallTimeout
        { get { return GetRegistry().MonitorServiceInstallTimeout; } }

        /// <summary>
        /// The max timeout to wait for the monitor service start
        /// </summary>
        public static Int32 MonitorServiceStartTimeout
        { get { return GetRegistry().MonitorServiceStartTimeout; } }

        /// <summary>
        /// The number of times to retry a failed wait for service ready call
        /// </summary>
        public static Int32 MonitorServiceStartRetries
        { get { return GetRegistry().MonitorServiceStartRetries; } }

        /// <summary>
        /// The service account user for the monitor service
        /// </summary>
        public static KnownStockAccountType MonitorServiceAccount
        { get { return GetRegistry().MonitorServiceAccount; } }

        /// <summary>
        /// The stock service account option for the connector
        /// </summary>
        public static KnownStockAccountType ConnectorServiceStockAccount
        { get { return GetRegistry().ConnectorServiceStockAccount; } }

        /// <summary>
        /// The timeout value to use when uploading to the blob store
        /// </summary>
        public static TimeSpan BlobUploadTimeout
        { get { return GetRegistry().BlobUploadTimeout; } }

        /// <summary>
        /// The timeout value to use when downloading from the blob store
        /// </summary>
        public static TimeSpan BlobDownloadTimeout
        { get { return GetRegistry().BlobDownloadTimeout; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static ConnectorRegistry GetRegistry()
        {
            lock (_lockObject)
            {
                if (_registry == null)
                {
                    _registry = new ConnectorRegistry(ConnectorServiceUtils.ServiceName);
                }
            }

            return _registry;
        }

        private static ConnectorRegistry _registry;
        private static Object _lockObject = new Object();
    }
}
