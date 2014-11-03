using System;
using Microsoft.Win32;

namespace Sage.Connector.Common.Internal
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ConnectorRegistry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectorServiceName"></param>
        public ConnectorRegistry(String connectorServiceName)
        {
            String productId = GetStringValue(connectorServiceName, "ProductId");

            InstallPath = GetStringValue(productId, "InstallPath");
            ProductId = GetStringValue(productId, "ProductId");
            ProductCode = GetStringValue(productId, "ProductCode");
            BriefProductName = GetStringValue(productId, "BriefProductName");
            FullProductName = GetStringValue(productId, "FullProductName");
            ProductHelpBaseUrl = GetStringValue(productId, "ProductHelpBaseUrl");
            ProductVersion = GetStringValue(productId, "ProductVersion");
            MinimumConnectorIntegrationInterfacesVersion = GetStringValue(productId, "MinimumConnectorIntegrationInterfacesVersion");
            ConnectorInstanceGuid = GetConnectorInstanceGuid(productId);
            
            // Note: This value should be persisted in someplace more configurable
            //Pre Pegasus threshold was 30days. Higher traffic rate and more logging lead to much faster growth.
            //Cut retention to 7 days, and moved max logging database from 2gb to 3gb.
            //saw "typical" daily growth of 120,000 kb, error rate of 231,000kb a day.
            //new settings will allow for 400,000 a day for 7 days with 200,000 to spair.
            LogRetentionPolicyThreshold = new TimeSpan(7, 0, 0, 0, 0);

            // Note: This value should be persisted in someplace more configurable
            StorageQueueMutexWaitTimeout = new TimeSpan(0, 1, 0);

            // TODO: re-evaluate:  value? where should it be persisted? updateable?
            // TODO: remove this hack around the fact that the messages are too large!
            // should we use this on other proxies?
            CloudProxyRetryTimeout = 1000 * 60 * 2;

            // TODO: re-evaluate:  value? where should it be persisted? updateable?
            StorageQueueProcessingTimeout = 1000 * 60 * 60 * 24; // Milliseconds X Seconds X Minutes X Hours

            // Note: This value should be persisted in someplace more configurable
            HostingFxWaitForReadySleepInterval = 1000;

            // Note: This value should be persisted in someplace more configurable
            HostingFxWaitForNotReadySleepInterval = 1000;

            // Note: This is not used and should be removed.
            BinderInvokerRetrySleepInterval = 5000;

            // Note: This value should be persisted in someplace more configurable
            MessagingServiceStopWorkerThreadsSleepInterval = 1000;

            // TODO: re-evaluate:  value? where should it be persisted? updateable?
            MessagingServiceStopWorkerTimeoutInterval = 60000;

            // Note: This value should be persisted in someplace more configurable
            ErrorResponseRetryMax = 3;

            // Note: This value should be persisted in someplace more configurable
            InternetConnectivityStatusTcpConnectTimeout = Convert.ToInt32(TimeSpan.FromSeconds(30).TotalMilliseconds);

            // Note: This value should be persisted in someplace more configurable
            SiteAddress = "https://app.sageconstructionanywhere.com";

            // TODO: re-evaluate: values for the retry policies
            FixedIntervalRetryCount = 5;
            FixedIntervalRetryInterval = TimeSpan.FromSeconds(1);
            FixedIntervalFirstFastRetry = false;
            CrucialFixedIntervalRetryCount = 60;
            ExponentialBackoffRetryCount = 10;
            ExponentialBackoffMinBackoff = TimeSpan.FromSeconds(1);
            ExponentialBackoffMaxBackoff = TimeSpan.FromMinutes(1);
            ExponentialBackoffDeltaBackoff = TimeSpan.FromSeconds(10);
            ExponentialBackoffFirstFastRetry = false;
            InitBackOfficeConnectionStateRetryCount = 1;

            // Note: This value should be persisted in someplace more configurable
            DatabaseRecoveryBackupDirectory = "Backups";
            // Note: This value should be persisted in someplace more configurable
            SubsystemHealthDisplayLimit = 5;

            // Note: This value should be persisted in someplace more configurable
            // Note: connector start timeout needs to be greater than the hosting framework timeout!
            // This is because after the HF timeout happens, it investigates the reason why startup failed
            // And can potentially report back a more descriptive error than just timeout.
            ConnectorServiceInstallTimeout = Convert.ToInt32(TimeSpan.FromMinutes(1).TotalMilliseconds);
            ConnectorServiceStartTimeout = Convert.ToInt32(TimeSpan.FromMinutes(2).TotalMilliseconds);
            ConnectorServiceWaitForReadyTimeout = Convert.ToInt32(TimeSpan.FromMinutes(2).TotalMilliseconds);
            MonitorServiceInstallTimeout = Convert.ToInt32(TimeSpan.FromMinutes(1).TotalMilliseconds);
            MonitorServiceStartTimeout = Convert.ToInt32(TimeSpan.FromMinutes(1).TotalMilliseconds);
            MonitorServiceAccount = KnownStockAccountType.LocalService;
            
            ConnectorServiceStartRetries = 5;
            MonitorServiceStartRetries = 2;

            // Given all the churn around switching the provided stock account option, this is now
            // The ONLY PLACE that you need to change when switching from, say, network service to local system.
            // It will handle the UI text as well.
            ConnectorServiceStockAccount = KnownStockAccountType.LocalSystem;

            // TODO: re-evaluate: values for the connector UI refreshers
            ConnectorGetRefreshDataInterval = Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds);
            ConnectorApplyRefreshDataInterval = Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds);
            ConnectorGetRefreshDataThreadTimeout = Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds);

            // The default timeout for blob upload/download operation is supposedly 10 minutes per megabytes.
            // http://msdn.microsoft.com/en-us/library/windowsazure/dd179431.aspx
            // But we have seen instances from external clients with really slow networks (less than 1Mbps)
            // timing out 14 MB uploads after 10 minutes. We don't have good numbers for maximum packet sizes
            // and minimum bandwidth requirements, and we could switch to a more dynamic evaluation of
            // timeouts by calculating file sizes and transfer speed (or switching to paged block uploads/
            // downloads).  But the limits below of 30 minutes _should_ be sufficient to transfer 25MB
            // of data over a 128kbps ISDN/DSL connection.  (They host their accounting servers out on
            // the job site trailer...)
            BlobDownloadTimeout = BlobUploadTimeout = TimeSpan.FromMinutes(30);
        }

        private static String GetStringValue(String productId, String valueName)
        {
            String result = String.Empty;

            using (var key = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Sage\SageConnector\{0}", productId), false))
            {
                Object valueResult = key.GetValue(valueName);
                if (valueResult != null)
                {
                    result = (String)valueResult;
                }
            }

            return result;
        }

        private static Guid GetConnectorInstanceGuid(String productId)
        {
            var stringConnectorInstanceId = GetStringValue(productId, "ConnectorInstanceId");
            if (!string.IsNullOrEmpty(stringConnectorInstanceId))
            {
                Guid registryConnectorInstanceId;
                if (Guid.TryParse(stringConnectorInstanceId, out registryConnectorInstanceId))
                    return registryConnectorInstanceId;
            }

            try
            {
                // Best attempt at establishing a new connector instance identifier
                using (
                    var key =
                        Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Sage\SageConnector\{0}", productId),
                            true))
                {
                    if (key != null)
                    {
                        Guid newConnectorInstanceGuid = Guid.NewGuid();
                        key.SetValue("ConnectorInstanceId", newConnectorInstanceGuid.ToString());
                        return newConnectorInstanceGuid;
                    }
                }
            }
            catch
            {
            }

            return Guid.Empty;

        }

        /// <summary>
        /// 
        /// </summary>
        public String InstallPath
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String ProductId
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String ProductCode
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String ProductHelpBaseUrl
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String BriefProductName
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String FullProductName
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String ProductVersion
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan LogRetentionPolicyThreshold
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan StorageQueueMutexWaitTimeout
        { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public Int32 CloudProxyRetryTimeout
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double StorageQueueProcessingTimeout
        { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public Int32 HostingFxWaitForReadySleepInterval
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 HostingFxWaitForNotReadySleepInterval
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 BinderInvokerRetrySleepInterval
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 MessagingServiceStopWorkerThreadsSleepInterval
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 MessagingServiceStopWorkerTimeoutInterval
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public UInt32 ErrorResponseRetryMax
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 InternetConnectivityStatusTcpConnectTimeout
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String SiteAddress
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 FixedIntervalRetryCount
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan FixedIntervalRetryInterval
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool FixedIntervalFirstFastRetry
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 CrucialFixedIntervalRetryCount
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 InitBackOfficeConnectionStateRetryCount
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ExponentialBackoffRetryCount
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan ExponentialBackoffMinBackoff
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan ExponentialBackoffMaxBackoff
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan ExponentialBackoffDeltaBackoff
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ExponentialBackoffFirstFastRetry
        { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public string DatabaseRecoveryBackupDirectory
        { get; private set; }

        /// <summary>
        /// The assembly file version of Sage.CRE.CloudConnector.Integration.Interfaces.dll that we 
        /// expect the backoffice to be using.  They return what they actually have through
        /// the IVersionInfo.GetInterfaceVersion()
        /// </summary>
        public String MinimumConnectorIntegrationInterfacesVersion
        { get; private set; }

        /// <summary>
        /// The unique connector instance GUID that is set in the registry when the
        /// connector service is installed.
        /// </summary>
        public Guid ConnectorInstanceGuid
        { get; private set; }

        /// <summary>
        /// The display limit for subsystem health issues
        /// </summary>
        public uint SubsystemHealthDisplayLimit
        { get; private set; }

        /// <summary>
        /// Interval for the connector UI get refresh data thread
        /// </summary>
        public Int32 ConnectorGetRefreshDataInterval
        { get; private set; }

        /// <summary>
        /// Interval for the connector UI apply refresh data timer
        /// </summary>
        public Int32 ConnectorApplyRefreshDataInterval
        { get; private set; }

        /// <summary>
        /// Timeout for stopping the connector UI get refresh data thread
        /// </summary>
        public Int32 ConnectorGetRefreshDataThreadTimeout
        { get; private set; }

        public Int32  ConnectorServiceInstallTimeout
        { get; private set; }

        public Int32 ConnectorServiceStartTimeout
        { get; private set; }

        public Int32 ConnectorServiceStartRetries 
        { get; private set; }

        public Int32 ConnectorServiceWaitForReadyTimeout
        { get; private set; }

        public Int32 MonitorServiceInstallTimeout
        { get; private set; }

        public Int32 MonitorServiceStartTimeout
        { get; private set; }

        public Int32 MonitorServiceStartRetries
        { get; private set; }

        public KnownStockAccountType MonitorServiceAccount 
        { get; private set; }

        public KnownStockAccountType ConnectorServiceStockAccount 
        { get; private set; }

        public TimeSpan BlobUploadTimeout
        { get; private set; }

        public TimeSpan BlobDownloadTimeout
        { get; private set; }
    }
}
