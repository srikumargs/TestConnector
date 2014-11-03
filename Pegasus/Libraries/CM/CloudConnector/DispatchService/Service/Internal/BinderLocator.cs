using System;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// Locates binder for request message - 
    /// Notified of a dispatch message, locates
    /// appropriate binder and enqueues the message
    /// to the tenant in-memory binder queue
    /// </summary>
    internal static class BinderLocator
    {
        /// <summary>
        /// Type name for error purposes
        /// </summary>
        private static readonly String _myTypeName = typeof(BinderLocator).FullName;

        /// <summary>
        /// Queues the dispatch message for binding
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        /// <param name="maxBlobSize"></param>
        /// <param name="queueManager"></param>
        /// <param name="binderQueue"></param>
        /// <param name="logManager"></param>
        public static void QueueMessageForBinding(
            string tenantId,
            string messageId,
            long maxBlobSize,
            QueueManager queueManager,
            BinderQueue binderQueue,
            LogManager logManager)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "Tenant Identifier", _myTypeName + ".QueueMessageForBinding()");
            ArgumentValidator.ValidateNonEmptyString(messageId, "Message Identifier", _myTypeName + ".QueueMessageForBinding()");
            ArgumentValidator.ValidateNonNullReference(queueManager, "Queue Manager", _myTypeName + ".QueueMessageForBinding()");
            ArgumentValidator.ValidateNonNullReference(binderQueue, "Binder Queue", _myTypeName + ".QueueMessageForBinding()");

            using (new StackTraceContext(null, "messageId={0}", messageId))
            {
                try
                {
                    var message = queueManager.GetSpecificMessage(messageId);
                    if (message != null)
                    {
                        RequestWrapper requestWrapper = Utils.JsonDeserialize<RequestWrapper>(message);
                        if (null != requestWrapper)
                        {
                            logManager.AdvanceActivityState(null, requestWrapper.ActivityTrackingContext, ActivityState.State3_DequeueTenantInboxRequest, ActivityEntryStatus.InProgress);

                            IBindable bindableCandidate = BindableCatalog.FindIBindable(requestWrapper.ActivityTrackingContext.RequestType, logManager);
                            if (bindableCandidate == null)
                            {
                                bindableCandidate = new NoApplicableBinderBinder();
                            }

                            using (var lm = new LogManager())
                            {
                                lm.WriteInfo(null, "Adding inbox message to binder; messageId={0}", messageId);
                            }

                            BinderQueueElement bqe = new BinderQueueElement(Guid.NewGuid().ToString(), messageId, bindableCandidate, requestWrapper)
                            {
                                MaxBlobSize = maxBlobSize
                            };
                            binderQueue.Enqueue(bqe);

                            SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.DispatchService);
                        }
                        else
                        {
                            if (null != logManager)
                            {
                                logManager.WriteError(null, "Failed to deserialize inbox message (id='" + messageId + "'");
                            }
                            SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.DispatchService, "Message deserialization error", "Error encountered processing inbox message.");
                        }
                    }
                    else
                    {
                        if (null != logManager)
                        {
                            logManager.WriteError(null, "Failed to retrieve inbox message (id='" + messageId + "'");
                        }
                        SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.DispatchService, "Inbox message retrieval error", "Error encountered retrieving inbox message.");
                    }
                }
                catch (Exception ex)
                {
                    if (null != logManager)
                    {
                        logManager.WriteError(null, "Error dispatching incoming message to the binder queue; exception: {0}", ex.ExceptionAsString());
                    }
                    SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.DispatchService, ex.ExceptionAsString(), "Error dispatching inbox message to workers.");

                    // Force a new thread context
                    if (ex is System.Data.EntityException)
                    {
                        try
                        {
                            using (
                                var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost",
                                                                                                ConnectorServiceUtils.
                                                                                                    CatalogServicePortNumber)
                                )
                            {
                                proxy.NotifyTenantRestart(tenantId);
                            }
                        }
                        catch (Exception)
                        {
                            // Best attempt notification to spawn restarts
                        }
                    }
                }
            }
        }
    }
}
