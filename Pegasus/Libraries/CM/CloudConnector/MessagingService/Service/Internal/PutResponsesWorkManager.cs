using System;
using System.Collections.Generic;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.ClientProxies;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// A manager that invokes the PutResponses work for a given tenant at the appropriate times
    /// </summary>
    internal sealed class PutResponsesWorkManager : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the PutResponsesWorkManager class
        /// </summary>
        /// <param name="syncRoot"></param>
        /// <param name="requestEndpointAddress"></param>
        /// <param name="requestUploadResourcePath"></param>
        /// <param name="responseEndpointAddress"></param>
        /// <param name="responseUploadResourcePath"></param>
        /// <param name="responseResourcePath"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="tenantClaim"></param>
        /// <param name="policy"></param>
        /// <param name="scheduler"></param>
        public PutResponsesWorkManager(
            Object syncRoot, 
            Uri requestEndpointAddress,
            String requestUploadResourcePath,
            Uri responseEndpointAddress,
            String responseUploadResourcePath,
            String responseResourcePath,
            String tenantId, 
            String premiseKey,
            String tenantClaim,
            PutResponsesPolicy policy, 
            IScheduler scheduler)
        {
            _stopEvent = new ManualResetEvent(false);
            _outboxMessageReadyForTenant = new AutoResetEvent(false);
            _requestEndpointAddress = requestEndpointAddress;
            _requestUploadResourcePath = requestUploadResourcePath;
            _responseEndpointAddress = responseEndpointAddress;
            _responseUploadResourcePath = responseUploadResourcePath;
            _responseResourcePath = responseResourcePath;
            _tenantId = tenantId;
            _premiseKey = premiseKey;
            _tenantClaim = tenantClaim;
            _policy = policy;
            _scheduler = scheduler;
        }

        /// <summary>
        /// When the configuration is re-enabled, or created, we need to explicitly
        /// refire existing response messages as if they 'just' came in
        /// </summary>
        public void RefireResponseMessageNotification()
        {
            try
            {
                List<String> existingResponseIds = new List<String>();
                using (var qm = new QueueManager())
                {
                    StorageQueueMessage sqm = qm.GetMessageFromOutput(_tenantId);
                    while (null != sqm)
                    {
                        existingResponseIds.Add(sqm.Id);
                        sqm = qm.GetMessageFromOutput(_tenantId);
                    }
                    foreach (string responseId in existingResponseIds)
                    {
                        qm.RestoreQueueMessage(responseId);
                    }
                }
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.Queues);

                // Technically, we only need to be notified once, but to keep
                // assumptions minimal, we will notify our system of every response
                foreach (string responseId in existingResponseIds)
                {
                    SetOutboxMessageReadyEvent(new Guid(responseId));
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteCriticalWithEventLogging(this, "Messaging Service Tenant Work Coordinator", "Error encountered retrieving messages for the tenant outbox; exception: {0}", ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.Queues, ex.ExceptionAsString(), "Error retrieving outbox entries: " + ex.Message);
            }
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
                    do
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        Boolean someWorkDone = false;
                        try
                        {
                            lock (_syncRoot)
                            {
                                var premiseAgent = PremiseAgentHelper.GetPremiseAgent(_tenantId);
                                var logger = new MessageLogger(LogManager.StaticWriteInfo);

                                var responseProxy = new APIResponseServiceProxy(
                                    _responseEndpointAddress, _responseResourcePath, _tenantId, _premiseKey, _tenantClaim,
                                    premiseAgent, logger);
                                var requestBeginUploadProxy =
                                    new APIBeginUploadSessionServiceProxy(_requestEndpointAddress,
                                        _requestUploadResourcePath, _tenantId, _premiseKey, _tenantClaim,  premiseAgent, logger);
                                var responseConcludeUploadProxy =
                                    new APIConcludeUploadSessionServiceProxy(_responseEndpointAddress,
                                        _responseUploadResourcePath, _tenantId, _premiseKey, _tenantClaim,  premiseAgent,
                                        logger);
                                PutResponsesWorker worker = new PutResponsesWorker(_responseEndpointAddress, responseProxy,
                                                requestBeginUploadProxy,
                                                responseConcludeUploadProxy,
                                                _tenantId,
                                                _policy, _limiter);

                                someWorkDone = worker.DoWork(cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            using (var lm = new LogManager())
                            {
                                lm.WriteError(this, ex.ExceptionAsString());
                            }
                        }

                        timeToNextWorkInMS = _scheduler.GetTimeToNextWork(someWorkDone);

                        CloudConnectivityStateMonitorHelper.UpdateNextScheduledCommunication(_tenantId, timeToNextWorkInMS);

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
        /// Blocks until an outbox message is ready for processing, the specified timeout has elapsed, or the stop event
        /// has been signaled
        /// </summary>
        /// <param name="timeToNextWorkInMS"></param>
        /// <returns></returns>
        private Boolean StopSignalled(Int32 timeToNextWorkInMS)
        {
            RaiseObjectDisposedExceptionIfNeeded();
            Int32 waitResult = WaitHandle.WaitAny(new WaitHandle[] { _stopEvent, _outboxMessageReadyForTenant }, timeToNextWorkInMS);
            return (waitResult == 0);
        }

        public void SetOutboxMessageReadyEvent(Guid messageId)
        {
            RaiseObjectDisposedExceptionIfNeeded();
            _outboxMessageReadyForTenant.Set(); 
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

                if (_outboxMessageReadyForTenant != null)
                    _outboxMessageReadyForTenant.Dispose();

                if (_limiter != null)
                    _limiter.Dispose();
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

        private readonly Object _syncRoot = new object();
        private readonly IScheduler _scheduler;
        private readonly ManualResetEvent _stopEvent;
        private readonly AutoResetEvent _outboxMessageReadyForTenant;
        private readonly Uri _requestEndpointAddress;
        private readonly String _requestUploadResourcePath;
        private readonly Uri _responseEndpointAddress;
        private readonly String _responseUploadResourcePath;
        private readonly String _responseResourcePath;
        private readonly String _tenantId;
        private readonly String _premiseKey;
        private readonly String _tenantClaim;
        private readonly Semaphore _limiter = new Semaphore(4, 4);
        private readonly PutResponsesPolicy _policy;
        private static readonly String _myTypeName = typeof(PutResponsesWorkManager).FullName;
        private Boolean _disposed;
    }
}
