using System;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.Signalr.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace Sage.Connector.MessagingService.Internal
{
    internal sealed class RequestAvailabilityWorkManager : IDisposable
    {
        public RequestAvailabilityWorkManager(
            IConnectorClient connectorClient,
            Guid tenantGuid,
            ILogging logger,
            IScheduler scheduler)
        {
            ConnectorClient = connectorClient;
            TenantGuid = tenantGuid;
            Logger = logger;
            Scheduler = scheduler;

            _stopEvent = new ManualResetEvent(false);
            _pokedEvent = new AutoResetEvent(false);
        }

        public void InvokeWorker(CancellationToken cancelToken)
        {
            Int32 timeToNextWorkInMS = 0;
            do
            {
                try
                {
                    if (!cancelToken.IsCancellationRequested)
                    {
                        var worker = new RequestAvailabilityWorker(
                            ConnectorClient,
                            TenantGuid,
                            Logger,
                            10000); // TODO: Retrieve timeout from scheduler
                        try
                        {
                            bool bResult = worker.DoWork(cancelToken);
                            CloudConnectivityStateMonitorHelper.UpdateTenantConnectivityStatusToIfUriTestSucceeds(
                                TenantGuid.ToString(),
                                new Uri(ConnectorClient.BaseAddress),
                                bResult ? TenantConnectivityStatus.Normal : TenantConnectivityStatus.CloudUnavailable);
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteError(this, ex.ExceptionAsString());
                        }

                        // Progressive fallback to ensure connectivity
                        timeToNextWorkInMS = Scheduler.GetTimeToNextWork(true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteError(this, ex.ExceptionAsString());
                }
            }
            while (!cancelToken.IsCancellationRequested && !StopSignalled(timeToNextWorkInMS));
        }

        private Boolean StopSignalled(Int32 timeToNextWorkInMS)
        {
            Int32 waitResult = WaitHandle.WaitAny(new WaitHandle[] { _stopEvent, _pokedEvent }, timeToNextWorkInMS);
            return (waitResult == 0);
        }

        public void StopWorker()
        {
            _stopEvent.Set();
        }

        public void PokeWorker()
        {
            _pokedEvent.Set();
        }

        public void InvokeTermination()
        {
            StopWorker();

            var worker = new RequestAvailabilityWorker(
                ConnectorClient,
                TenantGuid,
                Logger,
                10000);
            worker.EndWork();
        }

        private IConnectorClient ConnectorClient { get; set; }
        private Guid TenantGuid { get; set; }
        private ILogging Logger { get; set; }
        private IScheduler Scheduler { get; set; }
        private readonly ManualResetEvent _stopEvent;
        private readonly AutoResetEvent _pokedEvent;
        private Boolean _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (ConnectorClient != null)
                {
                    ConnectorClient.Shutdown();
                    ConnectorClientFactory.DisposeEmptyConnectorClient(ConnectorClient.BaseAddress);
                }
                if (_stopEvent != null)
                    _stopEvent.Dispose();
                if (_pokedEvent != null)
                    _pokedEvent.Dispose();
            }
            _disposed = true;
        }
    }
}
