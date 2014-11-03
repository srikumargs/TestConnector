using System;
using System.Collections.Generic;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.ClientProxies;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// A manager that invokes the GetRequests work for a given tenant at the appropriate times
    /// </summary>
    internal sealed class GetRequestsWorkManager : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the GetRequestsWorkManager class
        /// </summary>
        /// <param name="syncRoot"></param>
        /// <param name="requestEndpointAddress"></param>
        /// <param name="requestResourcePath"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="tenantClaim"></param>
        /// <param name="scheduler"></param>
        public GetRequestsWorkManager(
            Object syncRoot,
            Uri requestEndpointAddress,
            String requestResourcePath,
            String tenantId,
            String premiseKey,
            String tenantClaim,
            IScheduler scheduler)
        {
            _syncRoot = syncRoot;
            _stopEvent = new ManualResetEvent(false);
            _inboxMessageReadyForTenant = new AutoResetEvent(false);
            _requestEndpointAddress = requestEndpointAddress;
            _requestResourcePath = requestResourcePath;
            _tenantId = tenantId;
            _premiseKey = premiseKey;
            _tenantClaim = tenantClaim;
            _scheduler = scheduler;
        }

        /// <summary>
        /// The task function which is spun up by the TenantWorkCoordinator.  Under normal circumstances there is one
        /// of these running at all times, per each tenant.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void InvokeWorker(CancellationToken cancellationToken)
        {
            using (new StackTraceContext(this, "_tenantId={0}", _tenantId))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                try
                {
                    Int32 timeToNextWorkInMS = 0;
                    // TODO: Consider preloading with existing requests on disk to prevent startup race condition
                    List<Guid> previouslyRetrievedRequestIds = new List<Guid>();
                    do
                    {
                        // Make sure that we completed the above wait normally
                        // If we exited because a stop was signalled, then we don't want to proceed
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        Boolean someWorkDone = false;
                        try
                        {
                            lock (_syncRoot)
                            {
                                PremiseAgent premiseAgent = PremiseAgentHelper.GetPremiseAgent(_tenantId);
                                MessageLogger logger = new MessageLogger(LogManager.StaticWriteInfo);
                                using (var requestProxy =
                                    new APIRequestServiceProxy(
                                    _requestEndpointAddress, _requestResourcePath, _tenantId, _premiseKey, _tenantClaim, premiseAgent, logger))
                                {
                                        GetRequestsWorker worker = new GetRequestsWorker(
                                            _requestEndpointAddress,
                                            requestProxy,
                                            _tenantId);
                                        someWorkDone = worker.DoWork(cancellationToken, ref previouslyRetrievedRequestIds);
                                }
                                Monitor.Pulse(_syncRoot);
                            }

                        }
                        catch (Exception ex)
                        {
                            // do not let any exception in this work item shutdown the
                            // while loop
                            using (var lm = new LogManager())
                            {
                                lm.WriteError(this, ex.ExceptionAsString());
                            }
                        }

                        // Get the next processing time according to the scheduler
                        timeToNextWorkInMS = _scheduler.GetTimeToNextWork(someWorkDone);

                        CloudConnectivityStateMonitorHelper.UpdateNextScheduledCommunication(_tenantId,
                            timeToNextWorkInMS);

                    } while (!cancellationToken.IsCancellationRequested && !StopSignalled(timeToNextWorkInMS));
                }
                catch (Exception ex)
                {
                    // do not allow any exceptions to cross the thread boundary
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, ex.ExceptionAsString());
                    }
                }
            }
        }

        /// <summary>
        /// Called to signal the worker thread function should stop
        /// </summary>
        public void SetStopWorkEvent()
        {
            RaiseObjectDisposedExceptionIfNeeded();
            _stopEvent.Set();
        }

        /// <summary>
        /// Blocks until the specified timeout has elapsed, or the stop event has been signaled
        /// </summary>
        /// <param name="timeToNextWorkInMS"></param>
        /// <returns></returns>
        private Boolean StopSignalled(Int32 timeToNextWorkInMS)
        {
            RaiseObjectDisposedExceptionIfNeeded();
            Int32 waitResult = WaitHandle.WaitAny(new WaitHandle[] {_stopEvent, _inboxMessageReadyForTenant},
                timeToNextWorkInMS);
            return (waitResult == 0);
        }

        /// <summary>
        /// Wakes up sleeping giant to start picking up requests
        /// </summary>
        /// <param name="messageId"></param>
        public void SetInboxMessageReadyEvent(Guid messageId)
        {
            RaiseObjectDisposedExceptionIfNeeded();
            _inboxMessageReadyForTenant.Set();
        }

        #region IDisposable implementation

        /// <summary>
        /// Dispose pattern
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose pattern
        /// </summary>
        /// <param name="disposing">Must true if not invoking from a finalizer, otherwise must be false</param>
        void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_stopEvent != null) 
                    _stopEvent.Dispose();

                if (_inboxMessageReadyForTenant != null)
                    _inboxMessageReadyForTenant.Dispose();
            }
            _disposed = true;
        }
        // - Signature to use if we ever unseal this class
        //protected virtual void Dispose(bool disposing)

        private void RaiseObjectDisposedExceptionIfNeeded()
        {
            if (_disposed) throw new ObjectDisposedException(_myTypeName);
        }

        #endregion

        private readonly Object _syncRoot;
        private readonly IScheduler _scheduler;
        private readonly ManualResetEvent _stopEvent;
        private readonly AutoResetEvent _inboxMessageReadyForTenant;
        private readonly Uri _requestEndpointAddress;
        private readonly String _requestResourcePath;
        private readonly String _tenantId;
        private readonly String _premiseKey;
        private readonly String _tenantClaim;
        private static readonly String _myTypeName = typeof(GetRequestsWorkManager).FullName;
        private Boolean _disposed;
    }
}
