using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.ClientProxies;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// The GetRequests work
    /// </summary>
    internal sealed class GetRequestsWorker
    {
        /// <summary>
        /// Initializes a new instance of the GetRequestsWorker class
        /// </summary>
        /// <param name="requestEndpointAddress"></param>
        /// <param name="requestService"></param>
        /// <param name="tenantId"></param>
        public GetRequestsWorker(
            Uri requestEndpointAddress,
            APIRequestServiceProxy requestService,
            String tenantId)
        {
            ArgumentValidator.ValidateNonNullReference(requestEndpointAddress, "requestEndpointAddress", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonNullReference(requestService, "requestService", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ctor()");

            _requestEndpointAddress = requestEndpointAddress;
            _requestService = requestService;
            _tenantId = tenantId;
        }

        /// <summary>
        /// Polls the cloud for requests for the back-office and forwards them to the input queue.
        /// </summary>
        /// <returns>True or false depending on if there were any requests received from the cloud.</returns>
        public Boolean DoWork(CancellationToken cancellationToken, ref List<Guid>previouslyRetrievedRequestIds)
        {
            Boolean result = false;

            using (var stc = new StackTraceContext(this, "_tenantId={0}", _tenantId))
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    IEnumerable<WebAPIMessage> messages = CloudGetMessages();
                    if (messages != null)
                    {
                        // TODO: Differentiate between full retrieval and throttled retrieval
                        //       If fully retrieved, we are done.
                        //       If throttled, count may be zero, but we are not done.
                        messages = FilterAlreadyReceivedMessages(messages, ref previouslyRetrievedRequestIds);
                        uint count = System.Convert.ToUInt32(messages.Count());
                        if (count > 0)
                        {
                            result = true; 

                            using (var lm = new LogManager())
                            {
                                lm.WriteInfo(this, "RECEIVED {0} new messages from the cloud", count);
                            }

                            CloudConnectivityStateMonitorHelper.IncrementRequestCount(_tenantId, count);
                            PutRequestsInInputQueue(messages);
                            CloudConnectivityStateMonitorHelper.IncrementRequestInProgressCount(_tenantId, count);
                        }
                    }

                    stc.SetResult(result);
                }
            }

            return result;
        }

        private IEnumerable<WebAPIMessage> CloudGetMessages()
        {
            IEnumerable<WebAPIMessage> result = null;
            using (var stc = new StackTraceContext(this, "_tenantId={0}", _tenantId))
            {
                try
                {
                    DateTime timeToUpdate = DateTime.UtcNow;
                    CloudConnectivityStateMonitorHelper.UpdateLastCommunicationAttempt(_tenantId, timeToUpdate);

                    result = _requestService.GetRequestMessages();

                    timeToUpdate = DateTime.UtcNow;
                    CloudConnectivityStateMonitorHelper.UpdateLastSuccessfulCommunication(_tenantId, timeToUpdate);
                }
                catch (Exception ex)
                {
                    // Send to the common fault processing logic
                    FaultHelper.ProcessFaultException(
                        ex, this, _tenantId, _requestEndpointAddress,
                        @"Messaging Service: An exception was encountered retrieving cloud requests");
                }

                stc.SetResult(result);
            }

            return result;
        }

        private IEnumerable<WebAPIMessage> FilterAlreadyReceivedMessages(IEnumerable<WebAPIMessage> requests, ref List<Guid> previouslyRetrievedRequestIds)
        {
            List<WebAPIMessage> filteredList = new List<WebAPIMessage>();
            List<Guid> currentlyRetrievedRequestIds = new List<Guid>();
            using (var qm = new QueueManager())
            {
                foreach (var request in requests)
                {
                    currentlyRetrievedRequestIds.Add(request.Id);
                    // If we didn't see it last time AND we don't already have it
                    // add it to the list of requests to process
                    if (!previouslyRetrievedRequestIds.Contains(request.Id) &&
                        (null == qm.GetSpecificMessage(request.Id.ToString())))
                        filteredList.Add(request);
                }
            }

            previouslyRetrievedRequestIds = currentlyRetrievedRequestIds;
            return filteredList;
        }

        private void PutRequestsInInputQueue(IEnumerable<WebAPIMessage> messages)
        {
            using (new StackTraceContext(this, "_tenantId={0}", _tenantId))
            {
                using (var lm = new LogManager())
                {
                    using (var qm = new QueueManager())
                    {
                        foreach (WebAPIMessage message in messages)
                        {
                            lm.WriteInfo(this, "Adding request to input queue: Id={0}, RequestKind={1}", message.Id, message.BodyType);
                            ActivityTrackingContext trackingContext = lm.BeginActivityTracking(this, _tenantId, message);

                            String requestAsString = FinalizeRequest(trackingContext, message);

                            try
                            {
                                qm.AddMessageToInput(requestAsString, message.BodyType, new QueueContext(trackingContext, ActivityState.State2_EnqueueTenantInboxRequest, ActivityEntryStatus.InProgress), message.Id.ToString());
                                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.Queues);
                            }
                            catch (Exception ex)
                            {
                                lm.WriteCriticalForRequestWithEventLogging(this, trackingContext, "Messaging",
                                    "Error adding request to the input queue: Id={0}, RequestKind={1}; exception: {2}",
                                    message.Id, message.BodyType, ex.ExceptionAsString());
                                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.Queues, ex.ExceptionAsString(),
                                    "Error adding a message to the inbox: " + ex.Message);

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
                                            proxy.NotifyTenantRestart(_tenantId);
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
        }

        private string FinalizeRequest(ActivityTrackingContext trackingContext, WebAPIMessage message)
        {
            return Utils.JsonSerialize(new RequestWrapper(trackingContext, message.Body));
        }

        private static readonly String _myTypeName = typeof(GetRequestsWorker).FullName;
        private readonly Uri _requestEndpointAddress;
        private readonly APIRequestServiceProxy _requestService;
        private readonly String _tenantId;
    }
}
