using System;
using System.Linq;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.Signalr.Interfaces;
using Sage.ServiceModel;

namespace Sage.Connector.MessagingService.Internal
{
    internal sealed class RequestAvailabilityWorker
    {
        public RequestAvailabilityWorker(
            IConnectorClient connectorClient,
            Guid tenantGuid,
            ILogging loggingServices,
            Double connectionTimeout)
        {
            ConnectorClient = connectorClient;
            TenantGuid = tenantGuid;
            LoggingServices = loggingServices;
            ConnectionTimeout = connectionTimeout;
        }

        public bool DoWork(CancellationToken cancelToken)
        {
            TimeSpan timeoutTimeSpan = TimeSpan.FromMilliseconds(ConnectionTimeout);
            ProgressiveBackoffHelper progressiveBackoffHelper = new ProgressiveBackoffHelper();

            try
            {
                while (!cancelToken.IsCancellationRequested &&
                       progressiveBackoffHelper.ElapsedTime < timeoutTimeSpan)
                {
                    progressiveBackoffHelper.DelayIfNeeded(ConnectionTimeout);
                    if ((ConnectorClient.State == ClientState.Disconnected) ||
                        (ConnectorClient.State == ClientState.UnexpectedDisconnect))
                    {
                        ConnectorClient.Startup();
                    }
                }

                if (cancelToken.IsCancellationRequested ||
                    (ConnectorClient.State != ClientState.Connected))
                {
                    LoggingServices.WriteError(this, "Unable to connect to message availability at {0} or was cancelled.", ConnectorClient.BaseAddress);
                    return false;
                }

                if (!ConnectorClient.GetSubscribedTenants().Any(st => st.Equals(TenantGuid)))
                {
                    return ConnectorClient.TenantSubscribe(TenantGuid);
                }
            }
            catch (Exception ex)
            {
                // Send to the common fault processing logic
                FaultHelper.ProcessFaultException(
                    ex, this, TenantGuid.ToString(), new Uri(ConnectorClient.BaseAddress),
                    @"Messaging Service: An exception was encountered connecting to message availability notifications.");
                return false;
            }

            return true;
        }

        public void EndWork()
        {
            try
            {
                ConnectorClient.TenantUnsubscribe(TenantGuid);
            }
            catch (Exception ex)
            {
                LoggingServices.WriteError(this, ex.ExceptionAsString());
            }
        }

        private IConnectorClient ConnectorClient { get; set; }
        private Guid TenantGuid { get; set; }
        private ILogging LoggingServices { get; set; }
        private Double ConnectionTimeout { get; set; }
    }
}
