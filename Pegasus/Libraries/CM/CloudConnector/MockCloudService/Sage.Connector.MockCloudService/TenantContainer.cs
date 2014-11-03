using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Connector.Signalr.Controller;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// Container used by Mock service to represent a tenant.
    /// </summary>
    internal sealed class TenantContainer : IDisposable
    {
        private TenantContainer() { }

        public TenantContainer(
            String tenantId,
            String premiseKey,
            Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParams,
            ServiceInfo gatewayServiceInfo)
        {
            TenantId = tenantId;
            PremiseKey = premiseKey;
            TestTime = 0.0;
            RequestDelay = 0.0;
            ConfigParams = configParams;
            GatewayServiceInfo = gatewayServiceInfo;

            TenantDisabled = false;
            Name = String.Format("Site{0}", MockCloudService.TenantCount());
            Uri = "http://somewebsite/" + Name;

            ConfigParams.TenantName = Name;
            ConfigParams.TenantPublicUri = new Uri(Uri);

            SetupCleanupProcess();
        }

        public TenantContainer(
            ConfigurationTenant config,
            Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParams,
            ServiceInfo gatewayServiceInfo)
        {
            TenantEndpointAddress = config.EndpointAddress;
            TenantId = config.TenantId;
            PremiseKey = config.PremiseKey;
            TestTime = config.TestTime;
            RequestDelay = config.RequestDelay;
            InitialRequests = config.InitialRequests;
            ContinuedRequests = config.ContinuedRequests;
            ConfigParamsCustomUpdate = BuildConfigParams(config.UpdateConfigurationParameters);

            ConfigParams = configParams;
            GatewayServiceInfo = gatewayServiceInfo;

            TenantDisabled = false;
            Name = String.Format("Site{0}", MockCloudService.TenantCount());
            Uri = "http://somewebsite/" + Name;

            ConfigParams.TenantName = Name;
            ConfigParams.TenantPublicUri = new Uri(Uri);

            SetupCleanupProcess();
        }

        Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration BuildConfigParams(
            UpdateConfigurationParams updatedConfiguration)
        {
            var newParams = new Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration()
            {
                ConfigurationBaseUri = updatedConfiguration.ConfigurationBaseUri,
                ConfigurationResourcePath = updatedConfiguration.ConfigurationResourcePath,
                RequestBaseUri = updatedConfiguration.RequestBaseUri,
                RequestResourcePath = updatedConfiguration.ConfigurationResourcePath,
                ResponseBaseUri = updatedConfiguration.ResponseBaseUri,
                ResponseResourcePath = updatedConfiguration.ResponseResourcePath,
                RequestUploadResourcePath = updatedConfiguration.RequestUploadResourcePath,
                ResponseUploadResourcePath = updatedConfiguration.ResponseUploadResourcePath,
                NotificationResourceUri = updatedConfiguration.NotificationResourceUri,
                MinimumConnectorProductVersion = updatedConfiguration.MinimumConnectorProductVersion,
                UpgradeConnectorProductVersion = updatedConfiguration.UpgradeConnectorProductVersion,
                UpgradeConnectorPublicationDate = updatedConfiguration.UpgradeConnectorPublicationDate,
                UpgradeConnectorDescription = updatedConfiguration.UpgradeConnectorDescription,
                UpgradeConnectorLinkUri = updatedConfiguration.UpgradeConnectorLinkUri,
                SiteAddressBaseUri = updatedConfiguration.SiteAddressBaseUri,
                TenantPublicUri = updatedConfiguration.TenantPublicUri,
                TenantName = updatedConfiguration.TenantName,
                MaxBlobSize = updatedConfiguration.MaxBlobSize,
                LargeResponseSizeThreshold = updatedConfiguration.LargeResponseSizeThreshold,
                SuggestedMaxConnectorUptimeDuration = updatedConfiguration.SuggestedMaxConnectorUptimeDuration,
                MinCommunicationFailureRetryInterval = updatedConfiguration.MinCommunicationFailureRetryInterval,
                MaxCommunicationFailureRetryInterval = updatedConfiguration.MaxCommunicationFailureRetryInterval
            };
            return newParams;
        }

        public String TenantEndpointAddress
        { get; private set; }

        public String TenantId
        { get; private set; }

        public String PremiseKey
        { get; private set; }

        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration ConfigParams
        { get; internal set; }

        public bool? TenantDisabled
        { get; internal set; }

        public ServiceInfo GatewayServiceInfo
        { get; internal set; }

        public String Uri
        { get; internal set; }

        public String Name
        { get; internal set; }


        #region Automation Members

        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration ConfigParamsCustomUpdate
        { get; internal set; }

        public double TestTime
        { get; private set; }

        public double RequestDelay
        { get; private set; }

        public ConfigurationRequest[] InitialRequests
        { get; private set; }

        public ConfigurationRequest[] ContinuedRequests
        { get; private set; }

        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration PendingUpdateConfigurationParams
        { get; set; }

        public bool ConfigParamUpdatePending
        { get { return PendingUpdateConfigurationParams != null; } }

        #endregion

        public void ClearInboxOutbox()
        {
            lock (_outboxLock)
            {
                _outbox.Clear();
            }

            lock (_inboxLock)
            {
                _inbox.Clear();

                // We are completely resetting the tenant info here,
                // So also reset the total count
                _inboxTotalCount = 0;
            }
        }

        public Request[] GetRequests()
        {
            Guid tenantGuid = Guid.Parse(TenantId);

            lock (_outboxLock)
            {
                List<Request> allMessages = GetAllMessagesFromCloudOutbox();
                return allMessages.ToArray();
            }
        }

        public void PutResponses(Response[] messages)
        {
            Guid tenantGuid = Guid.Parse(TenantId);

            lock (_inboxLock)
            {
                PutMessagesToCloudInbox(messages);
            }
        }

        public Response GetResponse(Guid requestGuid)
        {
            lock (_inboxLock)
            {
                return GetMessageFromCloudInbox(requestGuid);
            }
        }

        /// <summary>
        /// Number of messages currently in the queue
        /// </summary>
        /// <returns></returns>
        public Tuple<Int32, Int32> ExternalPeekCurrentMessageCounts()
        {
            Int32 inboxCount;
            Int32 outboxCount;
            lock (_inboxLock)
            {
                inboxCount = _inbox.Count;
            }

            lock (_outboxLock)
            {
                outboxCount = _outbox.Count;
            }
            return new Tuple<Int32, Int32>(inboxCount, outboxCount);
        }

        /// <summary>
        /// Total number of messages ever received for this tenant
        /// </summary>
        /// <returns></returns>
        public Tuple<Int32, Int32, Int32> ExternalPeekTotalMessageCounts()
        {
            Int32 inboxCount;
            Int32 outboxCount;
            Int32 errorCount;

            // Take a lock out on the inbox even though we're getting this info
            // From the inbox total, just to make sure the inbox is not getting responses
            // While we get the tally
            lock (_inboxLock)
            {
                // Get the total inbox tally
                inboxCount = _inboxTotalCount;
                errorCount = _errorTotalCount;
            }

            lock (_outboxLock)
            {
                // Outbox messages are not periodically cleared out, so get the total
                // From the outbox message list as usual
                outboxCount = _outbox.Count;
            }
            return new Tuple<Int32, Int32, Int32>(inboxCount, outboxCount, errorCount);
        }


        /// <summary>
        /// External retrieval of input or output messages
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Request> ExternalPeekOutboxMessage()
        {
            List<Request> result = new List<Request>();

            lock (_outboxLock)
            {
                TraceUtils.WriteLine("TenantContainer.ExternalPeekOutboxMessage: Tenant {0} Outbox has {1} messages", TenantId, _outbox.Count);
                result.AddRange(_outbox);
            }

            return result;
        }

        public IEnumerable<Response> ExternalPeekInboxMessage()
        {
            List<Response> result = new List<Response>();

            lock (_inboxLock)
            {
                TraceUtils.WriteLine("TenantContainer.ExternalPeekInboxMessage: Tenant {0} Inbox has {1} messages", TenantId, _inbox.Count);
                foreach (Tuple<DateTime, Response> inboxTuple in _inbox)
                {
                    // Add the response from the inbox tuple
                    result.Add(inboxTuple.Item2);
                }
            }

            return result;
        }

        /// <summary>
        /// External injection of a cloud-to-premise message (no premise-tenant segregation)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ExternalAddToPremiseMessage(Request message)
        {
            TraceUtils.WriteLine("TenantContainer.ExternalAddToPremiseMessage: message.RequestKind={0}, message.Id={1}", message.GetType().FullName, message.Id);

            lock (_outboxLock)
            {
                _outbox.Add(message);
            }

            Guid tenantGuid = Guid.Parse(TenantId);
              
            ConnectorController.Instance.AddNotification(tenantGuid);


            // If this is an update config params request
            // Then update the "pending" config data
            // Will overwrite our actual config data once a response is received
            UpdateConfigParamsRequest updateConfigParamsRequest = message as UpdateConfigParamsRequest;
            if (updateConfigParamsRequest != null)
            {
                PendingUpdateConfigurationParams = updateConfigParamsRequest.ConfigParams;
            }

            TraceUtils.WriteLine("TenantContainer.ExternalAddToPremiseMessage: complete");

            return true;
        }


        /// <summary>
        /// Return all unprocessed Requests for the given tenantGuid
        /// </summary>
        /// <returns></returns>
        private List<Request> GetAllMessagesFromCloudOutbox()
        {
            List<Request> result;
            Guid tenantGuid = Guid.Parse(TenantId);

            lock (_outboxLock)
            {
                result = _outbox.ToList();
                //_outbox.Clear();
            }
            return result;
        }

        /// <summary>
        /// Removes a specific request form the outbox
        /// </summary>
        /// <param name="requestId"></param>
        public void RemoveSpecificMessageFromCloudOutbox(Guid requestId)
        {
            lock (_outboxLock)
            {
                var requestIndex = _outbox.FindIndex(request => request.Id == requestId);
                if (requestIndex != -1)
                    _outbox.RemoveAt(requestIndex);
            }
        }


        /// <summary>
        /// Puts an array of Responses onto the cloud in box.  As part of this process,
        /// The corresponding out box messages will have their statuses updated, and new messages will
        /// Also be placed on the in box queue to tell the worker that there is something to do.
        /// </summary>
        /// <param name="messages"></param>
        private void PutMessagesToCloudInbox(Response[] messages)
        {
            Guid tenantGuid = Guid.Parse(TenantId);

            // Make sure all inbox messages are for the tenant specified
            lock (_inboxLock)
            {
                int errorCount = 0;
                List<Response> msgs = messages.ToList();
                foreach (Response msg in msgs)
                {
                    // Add the response with the current timestamp
                    _inbox.Add(new Tuple<DateTime, Response>(DateTime.UtcNow, msg));
                    RemoveSpecificMessageFromCloudOutbox(msg.RequestId);

                    if (msg.GetType() == typeof(ErrorResponse))
                        errorCount++;
                    else if (msg.GetType() == typeof(UpdateConfigParamsRequestResponse))
                        PersistPendingConfigParamsPending();
                }

                // Update the total tally by the number of messages added
                _inboxTotalCount += msgs.Count();
                _errorTotalCount += errorCount;
            }
        }

        private Response GetMessageFromCloudInbox(Guid requestGuid)
        {
            Guid tenantGuid = Guid.Parse(TenantId);

            // Make sure all inbox messages are for the tenant specified
            lock (_inboxLock)
            {
                foreach (var responseTuple in _inbox)
                {
                    var response = responseTuple.Item2;
                    if (response.RequestId == requestGuid)
                        return response;
                }
            }
            return null;
        }

        /// <summary>
        /// A UpdateConfigParamsRequestResponse was received by this tenant.  Persist the Pending changes to the
        /// live in memory config settings.
        /// </summary>
        private void PersistPendingConfigParamsPending()
        {
            ConfigParams = PendingUpdateConfigurationParams;
            PendingUpdateConfigurationParams = null;
        }

        /// <summary>
        /// Set up and kick off the cleanup process for old inbox messages
        /// </summary>
        private void SetupCleanupProcess()
        {
            _cleanupProcess = new System.Timers.Timer();
            _cleanupProcess.Elapsed += new System.Timers.ElapsedEventHandler(CleanupTimerHandler);
            _cleanupProcess.Interval = _cleanupProcessInterval;
            _cleanupProcess.Start();
        }

        /// <summary>
        /// Coordinates the execution of the method to clean up inbox elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CleanupTimerHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;

            try
            {
                // Disable timer for debug purposes, in case someone
                // Sets the interval to be very short
                timer.Enabled = false;

                // Call the actual cleanup method
                CleanupInboxCollection();
            }
            finally
            {
                // Re-enable when complete
                timer.Enabled = true;
            }
        }

        /// <summary>
        /// Performs the cleanup of inbox elements
        /// Based on age greater than a certain threshold
        /// </summary>
        private void CleanupInboxCollection()
        {
            // Set the cleanup timestamp threshold
            DateTime cleanupThreshold = DateTime.UtcNow - _cleanupThreshold;

            lock (_inboxLock)
            {
                // Create a list of inbox elements to clean up based on age
                List<Tuple<DateTime, Response>> cleanupElements = new List<Tuple<DateTime, Response>>();
                _inbox.ForEach(inboxTuple =>
                {
                    if (inboxTuple.Item1 < cleanupThreshold)
                    {
                        cleanupElements.Add(inboxTuple);
                    }
                });

                // Perform the clean up
                cleanupElements.ForEach(cleanupElement => _inbox.Remove(cleanupElement));
                _inbox.Capacity = _inbox.Count;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            if (_cleanupProcess != null)
            {
                _cleanupProcess.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// This is the cloud's in box that receives messages from the client
        /// </summary>
        private readonly List<Tuple<DateTime, Response>> _inbox = new List<Tuple<DateTime, Response>>();

        /// <summary>
        /// This is the cloud's out box that gets polled by the client
        /// </summary>
        private readonly List<Request> _outbox = new List<Request>();

        /// <summary>
        /// This stores the total number of responses received for this tenant.
        /// The actual messages are periodically cleaned up to decrease the memory footprint.
        /// </summary>
        private int _inboxTotalCount;

        /// <summary>
        /// This stores the total number of responses received for this tenant with an ErrorResponse.
        /// </summary>
        private int _errorTotalCount;

        /// <summary>
        /// Periodic cleanup check to make sure no messages are stuck on the in process queue forever
        /// </summary>
        private System.Timers.Timer _cleanupProcess;

        /// <summary>
        /// Interval for running the cleanup process
        /// Note: set to 1 minute
        /// </summary>
        private readonly double _cleanupProcessInterval = TimeSpan.FromMinutes(1).TotalMilliseconds;

        /// <summary>
        /// The threshold for cleaning up old in process messages
        /// Note: Set to 10 minutes
        /// </summary>
        private readonly TimeSpan _cleanupThreshold = TimeSpan.FromMinutes(10);

        private readonly Object _inboxLock = new Object();
        private readonly Object _outboxLock = new Object();
    }
}
