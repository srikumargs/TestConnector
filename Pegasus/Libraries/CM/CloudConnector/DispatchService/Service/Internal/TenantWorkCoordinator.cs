using System;
using System.Collections.Generic;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// Centralizes creation and cleanup of tenant-based components to enable dispatching and binding
    /// </summary>
    internal sealed class TenantWorkCoordinator : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TenantWorkCoordinator class
        /// </summary>
        /// <param name="configuration">The tenant premise configuration record.</param>
        /// <param name="maxBlobSize">The max blob size allowed for the tenant.</param>
        public TenantWorkCoordinator(PremiseConfigurationRecord configuration, long maxBlobSize = 0)
        {
            ArgumentValidator.ValidateNonNullReference(configuration, "configuration", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonEmptyString(configuration.CloudTenantId, "configuration.CloudTenantId", _myTypeName + ".ctor()");

            using (new StackTraceContext(this, "configuration.CloudTenantId={0}", configuration.CloudTenantId))
            {
                lock (_syncRoot)
                {
                    _maxBlobSize = maxBlobSize;
                    CreateWorkManagers(configuration);
                }
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// When the configuration is re-enabled, or created, we need to explicitly
        /// refire existing dispatch message as if they 'just' came in
        /// </summary>
        /// <param name="tenantId"></param>
        public void RefireDispatchMessageNotification(String tenantId)
        {
            try
            {
                using (var qm = new QueueManager())
                {
                    StorageQueueMessage sqm = qm.GetMessageFromInput(tenantId);
                    while (null != sqm)
                    {
                        MessageReadyForDispatch(tenantId, sqm.Id);
                        sqm = qm.GetMessageFromInput(tenantId);
                    }
                }
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.Queues);
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteCriticalWithEventLogging(this, "Dispatch Tenant Work Coordinator", "Error encountered retrieving messages for the binder; exception: {0}", ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.Queues, ex.ExceptionAsString(), "Error retrieving inbox entries: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates the tenant scheduler with the updated configuration information
        /// </summary>
        /// <param name="newConfiguration"></param>
        public void ConfigurationUpdated(PremiseConfigurationRecord newConfiguration)
        {
            ArgumentValidator.ValidateNonNullReference(newConfiguration, "newConfiguration", _myTypeName + ".ConfigurationUpdated");

            using (new StackTraceContext(this, "configuration.CloudTenantId={0};backOfficeEnabled={1}",
                newConfiguration.CloudTenantId,
                newConfiguration.BackOfficeConnectionEnabledToReceive.ToString()))
            {
                TenantScheduler.ConfigurationUpdated(newConfiguration, TenantQueue);
                // WE DO NOT SUPPORT TENANTID UPDATES, so the tenant queue does not need to be notified
            }
        }

        /// <summary>
        /// Shutdown our integration
        /// </summary>
        public void ConfigurationDeleted()
        {
            using (new StackTraceContext(this))
            {
                DisposeWorkManagers();
            }
        }

        /// <summary>
        /// Processes notification of a message added to the tenant's inbox
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void MessageReadyForDispatch(String tenantId, String messageId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".MessageReadyForDispatch");
            ArgumentValidator.ValidateNonEmptyString(messageId, "messageId", _myTypeName + ".MessageReadyForDispatch");

            using (new StackTraceContext(this, "messageId={0}", messageId))
            {
                using (var qm = new QueueManager())
                {
                    using (var lm = new LogManager())
                    {
                        BinderLocator.QueueMessageForBinding(tenantId, messageId, _maxBlobSize, qm, TenantQueue, lm);
                    }
                }
            }
        }

        /// <summary>
        /// Notification of a message added (enqueued) or restored to the binder queue
        /// </summary>
        public void MessageReadyForBinding()
        {
            using (new StackTraceContext(this))
            {
                TenantScheduler.BinderMessageEnqueued(TenantQueue);
            }
        }

        /// <summary>
        /// Notification of a message task completion
        /// </summary>
        /// <param name="bindingMessageId"></param>
        public void BindingWorkCompleted(String bindingMessageId)
        {
            ArgumentValidator.ValidateNonEmptyString(bindingMessageId, "bindingMessageId", _myTypeName + ".BindingWorkCompleted");

            using (new StackTraceContext(this, "bindingMessageId={0}", bindingMessageId))
            {

                try
                {
                    BinderQueueElement bqe = TenantQueue.Retrieve(bindingMessageId);
                    if (null != bqe)
                    {
                        // Let the memory queue continue despite potential
                        // unxpected persistance deletion failure.
                        TenantQueue.Delete(bindingMessageId);
                        TenantScheduler.BinderMessageDeleted(TenantQueue);
                    }
                    SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.DispatchService);
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(this, "Dispatch Tenant Work Coordinator", "Error encountered removing binding work queue element; exception: {0}", ex.ExceptionAsString());
                    }
                    SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.Queues, ex.ExceptionAsString(), "Unable to delete inbox message: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Cancels in-progress work
        /// </summary>
        public void CancelInProgressWork()
        {
            using (new StackTraceContext(this))
            {
                try
                {
                    BinderInvoker.CancelWork(TenantQueue);
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteErrorWithEventLogging(this, "Dispatch Tenant Work Coordinator", "Error encountered cancelling in-progress work; exception: {0}", ex.ExceptionAsString());
                    }
                }
            }
        }

        /// <summary>
        /// Cancels in-progress work
        /// <param name="activityTrackingContextId"></param>
        /// </summary>
        public void CancelWork(String activityTrackingContextId)
        {
            using (new StackTraceContext(this))
            {
                try
                {
                    BinderInvoker.CancelWork(activityTrackingContextId, TenantQueue);
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteErrorWithEventLogging(this, "Dispatch Tenant Work Coordinator", "Error encountered cancelling work; exception: {0}", ex.ExceptionAsString());
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns the count of in-progress work.
        /// </summary>
        /// <returns>The count of in-progress work.</returns>
        public int InProgressWorkCount()
        {
            return TenantQueue.InProgressWorkCount();
        }

        /// <summary>
        /// List of in-progress work
        /// </summary>
        public IEnumerable<RequestWrapper> InProgressWork()
        {
            using (new StackTraceContext(this))
            {
                try
                {
                    return TenantQueue.InProgressWork;
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteErrorWithEventLogging(this, "Dispatch Tenant Work Coordinator", "Error encountered cancelling work; exception: {0}", ex.ExceptionAsString());
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Sets the maximum single blob size for the tenant
        /// </summary>
        /// <param name="maxBlobSize"></param>
        public void SetMaxBlobSize(long maxBlobSize)
        {
            _maxBlobSize = maxBlobSize;
        }

        /// <summary>
        /// Destroys and recreate the work managers
        /// </summary>
        /// <param name="configuration"></param>
        public void RestartTenant(PremiseConfigurationRecord configuration)
        {
            using (new StackTraceContext(this))
            {
                DisposeWorkManagers();
                CreateWorkManagers(configuration);
            }
        }

        #endregion

        #region Private methods
        private void CreateWorkManagers(PremiseConfigurationRecord configuration)
        {
            using (new StackTraceContext(this))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                TenantScheduler = new BinderScheduler(configuration);
                TenantQueue = new BinderQueue(configuration.CloudTenantId);
                RefireDispatchMessageNotification(configuration.CloudTenantId);
            }
        }

        private void DisposeWorkManagers()
        {
            using (new StackTraceContext(this))
            {
                RaiseObjectDisposedExceptionIfNeeded();
                if (null != TenantQueue)
                {
                    TenantQueue.RestoreAllElementsToTenantDispatchQueue();
                    TenantQueue.Dispose();
                    TenantQueue = null;
                }
            }
        }

        private BinderScheduler TenantScheduler { get; set; }
        private BinderQueue TenantQueue { get; set; }
        #endregion

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
                DisposeWorkManagers();
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

        #region Private fields
        private static readonly String _myTypeName = typeof(TenantWorkCoordinator).FullName;

        // the synchronization object used to guard the internal data fields of this class against multiple thread access
        private readonly Object _syncRoot = new Object();
        private long _maxBlobSize;
        private Boolean _disposed;

        #endregion
    }
}
