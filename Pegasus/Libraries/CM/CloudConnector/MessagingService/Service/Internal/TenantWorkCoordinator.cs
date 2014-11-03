using System;
using System.Threading;
using System.Threading.Tasks;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.ConnectorServiceCommon.TaskRetry;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.MessagingService.ClientProxies;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// A class to coordinate the Put/Get work for a given tenant; handles the updating of workers when configuration 
    /// changes
    /// </summary>
    internal sealed class TenantWorkCoordinator : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the TenantWorkCoordinator class
        /// </summary>
        /// <param name="configuration"></param>
        public TenantWorkCoordinator(PremiseConfigurationRecord configuration)
        {
            ArgumentValidator.ValidateNonNullReference(configuration, "configuration", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonEmptyString(configuration.CloudTenantId, "configuration.CloudTenantId", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonEmptyString(configuration.SiteAddress, "configuration.CloudEndpoint", _myTypeName + ".ctor()");

            using (new StackTraceContext(this, "configuration.CloudTenantId={0}", configuration.CloudTenantId))
            {
                lock (_syncRoot)
                {
                    // Call the retrieve remote configuration data worker
                    // Which will set up and start the get and put workers
                    RetrieveRemoteConfigData(configuration);
                }

                // Start a new task that updates the back office connection status
                // The state service should be notified of the back office connection status on startup
                // Just like it is notified of the cloud connection status via the get worker on first 
                // Retrieve attempt
                InitBackOfficeConnectionState(configuration);
            }
        }

        #endregion


        #region Public methods        
        /// <summary>
        /// Start GetRequests and PutResponses worker tasks
        /// </summary>
        public void StartWorkers()
        {
            using (new StackTraceContext(this))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    if (null != _workerCancellationTokenSource)
                    {
                        _workerCancellationTokenSource.Cancel();
                        _workerCancellationTokenSource.Dispose();
                        _workerCancellationTokenSource = null;
                    }
                    _workerCancellationTokenSource = new CancellationTokenSource();

                    if (_requestAvailabilityWorkManager != null)
                    {
                        Task.Factory.StartNew(
                            () =>
                                _requestAvailabilityWorkManager.InvokeWorker(
                                    _workerCancellationTokenSource.Token),
                            _workerCancellationTokenSource.Token);
                    }
                    if (_getRequestsWorkManager != null)
                    {
                        Task.Factory.StartNew(
                            () =>
                                _getRequestsWorkManager.InvokeWorker(
                                _workerCancellationTokenSource.Token),
                            _workerCancellationTokenSource.Token);
                    }
                    if (_putResponsesWorkManager != null)
                    {
                        Task.Factory.StartNew(
                            () =>
                                _putResponsesWorkManager.InvokeWorker(
                                    _workerCancellationTokenSource.Token),
                            _workerCancellationTokenSource.Token);

                        // Re-fire response notification for the now enabled tenant
                        _putResponsesWorkManager.RefireResponseMessageNotification();
                    }
                }
            }
        }

        /// <summary>
        /// Stop GetRequests and PutResponses worker tasks.
        /// </summary>
        public void StopWorkers()
        {
            using (var stc = new StackTraceContext(this))
            {
                RaiseObjectDisposedExceptionIfNeeded();
                lock (_syncRoot)
                {
                    if (null != _workerCancellationTokenSource)
                    {
                        _workerCancellationTokenSource.Cancel();
                        _workerCancellationTokenSource.Dispose();
                        _workerCancellationTokenSource = null;
                    }

                    if (_requestAvailabilityWorkManager != null)
                    {
                        _requestAvailabilityWorkManager.InvokeTermination();

                        Uri messageWaitingNotificationUri =
                            _remoteConfiguration.NotificationResourceUri;
                        ConnectorClientFactory.DisposeEmptyConnectorClient(messageWaitingNotificationUri.ToString());
                    }

                    if (_getRequestsWorkManager != null)
                    {
                        _getRequestsWorkManager.SetStopWorkEvent();
                    }

                    if (_putResponsesWorkManager != null)
                    {
                        _putResponsesWorkManager.SetStopWorkEvent();
                    }
                }
            }
        }

        /// <summary>
        /// Update the configuration for a tenant; typically called in response to a configuration change event in the 
        /// configuration service
        /// </summary>
        /// <param name="configuration"></param>
        public void UpdateConfiguration(PremiseConfigurationRecord configuration)
        {
            ArgumentValidator.ValidateNonNullReference(configuration, "configuration", _myTypeName + ".UpdateConfiguration()");
            ArgumentValidator.ValidateNonEmptyString(configuration.CloudTenantId, "configuration.CloudTenantId", _myTypeName + ".UpdateConfiguration()");

            using (new StackTraceContext(this, "configuration.CloudTenantId={0}", configuration.CloudTenantId))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    // Refresh the remote configuration, which will also refresh the worker tasks
                    // Do this in all cases to prevent masking out of potential remote configuration updates
                    // By non-remote configuration updates
                    RetrieveRemoteConfigData(configuration);
                }
            }
        }

        /// <summary>
        /// Disable this tenant and cleanup current work.
        /// </summary>
        /// <param name="configuration"></param>
        public void StopAndDisableConfiguration(PremiseConfigurationRecord configuration)
        {
            //TODO: extend this to take a reason. Will use this for more then just incompatible clients now.
            if (configuration.CloudConnectionEnabledToReceive || configuration.CloudConnectionEnabledToSend)
            {
                string tenantId = configuration.CloudTenantId;
                using (new StackTraceContext(this, "tenantId={0}", tenantId))
                {
                    try
                    {

                        lock (_syncRoot)
                        {
                            using (var lm = new LogManager())
                            {
                                lm.WriteInfo(this,
                                             "Messaging Service: Disabling communications for Tenant {0}, due to incompatible client notification.",
                                             tenantId);
                            }

                            FaultHelper.IncompatibleDisableCollaboration(configuration, null);

                            //these actions should all be happening as a result of the notifications caused by setting 
                            //the configuration to disabled to send and receive.
                            //StopActiveBackOfficeVerification();
                            //StopActiveRemoteConfigRetrieval();
                            //StopWorkerTasks(ConnectorRegistryUtils.MessagingServiceStopWorkerTimeoutInterval);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(this,ex.ExceptionAsString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Trigger a worker restart by invoking UpdateConfiguration
        /// </summary>
        /// <param name="configuration"></param>
        public void RestartTenant(PremiseConfigurationRecord configuration)
        {
            UpdateConfiguration(configuration);
        }

        /// <summary>
        /// Event triggered to let the get worker know it has work to do
        /// </summary>
        /// <param name="messageId"></param>
        public void SetInboxMessageReadyEvent(Guid messageId)
        {
            using (new StackTraceContext(this, "messageId={0}", messageId))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    if (_getRequestsWorkManager != null)
                    {
                        _getRequestsWorkManager.SetInboxMessageReadyEvent(messageId);
                    }
                }
            }
        }

        /// <summary>
        /// Event triggered to let the put worker know it has work to do
        /// </summary>
        /// <param name="messageId"></param>
        public void SetOutboxMessageReadyEvent(Guid messageId)
        {
            using (new StackTraceContext(this, "messageId={0}", messageId))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    if (_putResponsesWorkManager != null)
                    {
                        _putResponsesWorkManager.SetOutboxMessageReadyEvent(messageId);
                    }
                }
            }
        }

        /// <summary>
        /// Event triggered to let the request availability work manager it has work to do
        /// </summary>
        public void SetRequestAvailableResubscribeEvent()
        {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    if (_requestAvailabilityWorkManager != null)
                    {
                        _requestAvailabilityWorkManager.PokeWorker();
                    }
                }
        }

        #endregion
        

        #region Private Worker Task Management Methods

        /// <summary>
        /// Stop, re-create and start the get and put worker tasks
        /// Should only be called after successful getting of remote configuration data
        /// </summary>
        /// <param name="pcr"></param>
        private void RefreshWorkerTasks(PremiseConfigurationRecord pcr)
        {
            lock (_syncRoot)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Attempting to restart tenant {0} collaboration.", pcr.CloudTenantId);
                } 
                StopAndDisposeWorkerManagers();
                CreateWorkManagers(pcr);
                StartWorkers();
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Successfully restarted tenant {0} collaboration.", pcr.CloudTenantId);
                }
            }
        }

        /// <summary>
        /// Stop and dispose all worker tasks, for use when refreshing them
        /// Or when retrieving (or re-retrieving) remote configuration data
        /// </summary>
        private void StopAndDisposeWorkerManagers()
        {
            lock (_syncRoot)
            {
                StopWorkers();
                DisposeWorkManagers();
            }
        }

        /// <summary>
        /// Create the get and put work managers
        /// Should only be called after successful getting of remote configuration data
        /// </summary>
        /// <param name="configuration"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateWorkManagers(PremiseConfigurationRecord configuration)
        {
            // Special logging diagnostics because we are encountering null exceptions in the field in this method
            using (var lm = new LogManager())
            {
                if (null == configuration)
                {
                    lm.WriteError(this, "Null premise configuration record supplied to CreateWorkManagers");
                }
                if (null == _remoteConfiguration)
                {
                    lm.WriteError(this, "Null remote configuration information while executing CreateWorkManager");
                }
                }

            using (new StackTraceContext(this))
            {
                RaiseObjectDisposedExceptionIfNeeded();
                if (configuration.CloudConnectionEnabledToReceive)
                {
                    // Create the scheduler for the request availability worker
                    var reqAvailabilityScheduler = SchedulerFactory.Create(
                        SchedulerPurpose.RequestAvailability,
                        _remoteConfiguration,
                        configuration);

                    // Create the request availability worker
                    var connectorClient = ConnectorClientFactory.CreateConnectorClient(
                        ConnectorRegistryUtils.ConnectorInstanceGuid,
                        _remoteConfiguration.NotificationResourceUri.ToString());
                    _requestAvailabilityWorkManager = new RequestAvailabilityWorkManager(
                        connectorClient,
                        new Guid(configuration.CloudTenantId),
                        new LogManager(),
                        reqAvailabilityScheduler);

                    // Create the scheduler for the get worker
                    var reqScheduler = SchedulerFactory.Create(
                        SchedulerPurpose.GetRequests,
                        _remoteConfiguration,
                        configuration);

                    // Create the get worker manager
                    _getRequestsWorkManager = new GetRequestsWorkManager(
                        _workSyncRoot,
                        _remoteConfiguration.RequestBaseUri,
                        _remoteConfiguration.RequestResourcePath,
                        configuration.CloudTenantId,
                        configuration.CloudPremiseKey,
                        configuration.CloudTenantClaim,
                        reqScheduler);
                }

                if (configuration.CloudConnectionEnabledToSend)
                {
                    // Create the put response policy for the put worker
                    var putResponsesPolicy = new PutResponsesPolicy(
                        configuration,
                        _remoteConfiguration);

                    // Create the scheduler for the put worker
                    var scheduler = SchedulerFactory.Create(
                        SchedulerPurpose.PutResponses,
                        _remoteConfiguration,
                        configuration);

                    // Create the put worker manager
                    _putResponsesWorkManager = new PutResponsesWorkManager(
                        _workSyncRoot,
                        _remoteConfiguration.RequestBaseUri,
                        _remoteConfiguration.RequestUploadResourcePath,
                        _remoteConfiguration.ResponseBaseUri,
                        _remoteConfiguration.ResponseUploadResourcePath,
                        _remoteConfiguration.ResponseResourcePath,
                        configuration.CloudTenantId,
                        configuration.CloudPremiseKey,
                        configuration.CloudTenantClaim,
                        putResponsesPolicy,
                        scheduler);
                }
            }
        }

        /// <summary>
        /// Dispose of all work managers
        /// </summary>
        private void DisposeWorkManagers()
        {
            using (new StackTraceContext(this))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                if (_requestAvailabilityWorkManager != null)
                {
                    _requestAvailabilityWorkManager.Dispose();
                    _requestAvailabilityWorkManager = null;
                }

                if (_getRequestsWorkManager != null)
                {
                    _getRequestsWorkManager.Dispose();
                    _getRequestsWorkManager = null;
                }

                if (_putResponsesWorkManager != null)
                {
                    _putResponsesWorkManager.Dispose();
                    _putResponsesWorkManager = null;
                }
            }
        }

        #endregion


        #region IDisposable implementation

        /// <summary>
        /// Dispose pattern
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //TODO look at if we really need a suppress given this class does not have a finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose pattern
        /// Signature to use if we ever unseal this class:
        /// protected virtual void Dispose(bool disposing)
        /// </summary>
        /// <param name="disposing">Must true if not invoking from a finalizer, otherwise must be false</param>
        void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_requestAvailabilityWorkManager != null)
                    _requestAvailabilityWorkManager.Dispose();

                if (_getRequestsWorkManager != null)
                    _getRequestsWorkManager.Dispose();

                if (_putResponsesWorkManager != null)
                    _putResponsesWorkManager.Dispose();

                if (_remoteConfigRetrievalTokenSource != null)
                {
                    _remoteConfigRetrievalTokenSource.Cancel();
                    _remoteConfigRetrievalTokenSource.Dispose();
                }

                if (_backOfficeVerificationTokenSource != null)
                {
                    _backOfficeVerificationTokenSource.Cancel();
                    _backOfficeVerificationTokenSource.Dispose();
                }

                if (_workerCancellationTokenSource != null)
                {
                    _workerCancellationTokenSource.Cancel();
                    _workerCancellationTokenSource.Dispose();
                }
            }
            _disposed = true;
        }

        private void RaiseObjectDisposedExceptionIfNeeded()
        {
            if (_disposed) throw new ObjectDisposedException(_myTypeName);
        }

        #endregion


        #region Remote Config Data Accessors

        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration RemoteConfiguration
        {
            get
            {
                lock (_syncRoot)
                {
                    return _remoteConfiguration;
                }
            }
        }

        /// <summary>
        /// Stop active remote configuration retrieval
        /// </summary>
        public void StopActiveRemoteConfigRetrieval()
        {
            // Attempt to stop any active retrieve remote configuration data task
            if (_activeRemoteConfigRetrievalTask != null)
            {
                try
                {
                    // Signal a cancel and wait for completion
                    _remoteConfigRetrievalTokenSource.Cancel();
                    _activeRemoteConfigRetrievalTask.Wait();
                }
                catch
                {
                    // Ignore exceptions on waiting to complete
                }
                finally
                {
                    _activeRemoteConfigRetrievalTask = null;
                }
            }
        }

        /// <summary>
        /// Update the configuration params stored by the remote configuration manager
        /// </summary>
        /// <param name="configParams"></param>
        /// <param name="pcr"></param>
        public void SetRemoteConfigParams(
            Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParams,
            PremiseConfigurationRecord pcr)
        {
            ArgumentValidator.ValidateNonNullReference(configParams, "configParams", _myTypeName + ".SetRemoteConfigParams()");
            ArgumentValidator.ValidateNonNullReference(pcr, "pcr", _myTypeName + ".SetRemoteConfigParams()");
            ArgumentValidator.ValidateNonEmptyString(pcr.CloudTenantId, "pcr.CloudTenantId", _myTypeName + ".SetRemoteConfigParams()");

            using (new StackTraceContext(this, "pcr.CloudTenantId={0}", pcr.CloudTenantId))
            {
                RaiseObjectDisposedExceptionIfNeeded();
                string configParamsString = RemoteConfigHelper.ObjectPropertiesToString(configParams);

                lock (_syncRoot)
                {
                    if (!RemoteConfigHelper.IsValidConfigParams(configParams))
                    {
                        // Cannot set our configuration params to something invalid
                        // Trigger the get remote configuration thread
                        RetrieveRemoteConfigData(pcr);

                        // Log that the set attempt was invalid
                        using (var lm = new LogManager())
                        {
                            lm.WriteWarning(this,
                                "Retrying get of all remote configuration data after invalid configuration params set attempted: {0}",
                                configParamsString);
                        }
                    }
                    else
                    {
                        // Valid params, just set our local copy
                        _remoteConfiguration = configParams;

                        // Log the new configuration params
                        using (var lm = new LogManager())
                        {
                            lm.WriteInfo(this, "configuration params reset for tenant {0} to {1}", 
                                pcr.CloudTenantId, configParamsString);
                        }

                        // We're not waiting on anything to complete, 
                        // So refresh the get and put worker tasks now
                        RefreshWorkerTasks(pcr);
                    }
                }
            }
        }

        /// <summary>
        /// Update the service infos stored by the remote configuration manager
        /// </summary>
        /// <param name="siteAddressBaseUri"></param>
        /// <param name="pcr"></param>
        public void SetRemoteServiceInfo(
            Uri siteAddressBaseUri,
            PremiseConfigurationRecord pcr)
        {
            ArgumentValidator.ValidateNonNullReference(siteAddressBaseUri, "siteAddressBaseUri", _myTypeName + ".SetRemoteServiceInfo()");
            ArgumentValidator.ValidateNonNullReference(pcr, "pcr", _myTypeName + ".SetRemoteServiceInfo()");
            ArgumentValidator.ValidateNonEmptyString(pcr.CloudTenantId, "pcr.CloudTenantId", _myTypeName + ".SetRemoteServiceInfo()");

            using (new StackTraceContext(this, "pcr.CloudTenantId={0}", pcr.CloudTenantId))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    // Trigger the get remote configuration thread
                    // When this completes successfully, the other worker tasks
                    // Will be refreshed, and the pcr will be updated if necessary
                    // With the new gateway URI
                    RetrieveRemoteConfigData(pcr, siteAddressBaseUri);
                }
            }
        }

        #endregion


        #region Remote Config Retrieved Retrieval And Callback

        /// <summary>
        /// Handle retrieval of the remote configuration data
        /// </summary>
        /// <param name="pcr"></param>
        /// <param name="siteAddressBaseUriParam">Override for using a new gateway address, otherwise uses the value in the provided PCR</param>
        private void RetrieveRemoteConfigData(
            PremiseConfigurationRecord pcr, 
            Uri siteAddressBaseUriParam = null)
        {
            if (pcr == null)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, "Null premise configuration record passed to retrieve remote configuration data!");
                }
                return;
            }

            bool noCloudCommunicationEnabled = false;
            //Don't attempt to get the RemoteConfig data if send and receive are off.
            if (!pcr.CloudConnectionEnabledToReceive && !pcr.CloudConnectionEnabledToSend)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteWarning(this, "Collaboration is disabled. Retrieval of remote configuration skipped for Tenant {0}", pcr.CloudTenantId);
                }
                
                //need to watch out for transitioning PCRs. This is being used for reset/updates. 
                //However if we re transitioning from active to disabled.
                noCloudCommunicationEnabled = true;
            }

            using (new StackTraceContext(this, "pcr.CloudTenantId={0}", pcr.CloudTenantId))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    // First stop any active retrieve remote configuration data task
                    StopActiveRemoteConfigRetrieval();

                    // The end result of this worker task will be to re-populate our remote configuration data
                    // Kill any existing worker tasks first, so they are not running with outdated
                    // Remote configuration data.  Also will kill any existing retrieve remote configuration data thread,
                    // Since there's no need to query twice.
                    StopAndDisposeWorkerManagers();

                    StopActiveBackOfficeVerification();

                    if (noCloudCommunicationEnabled)
                    {
                        //if we are not sending or receiving with the cloud we are done after we stop things.
                        return;
                    }
                        

                    // If the site address is set, use that, otherwise use the value in the PCR
                    Uri siteAddressBaseUri =
                        siteAddressBaseUriParam ?? new Uri(pcr.SiteAddress);

                    // Init out params
                    Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configuration = null; 

                    // Set the retry action for getting all remote configuration data
                    Action retryAction = new Action(() =>
                        {
                            // Throw OperationCancelledException if a cancel was requested
                            // This is a non-transient error for this type of retry
                            _remoteConfigRetrievalTokenSource.Token.ThrowIfCancellationRequested();


                            PremiseAgent premiseAgent = PremiseAgentHelper.GetPremiseAgent(pcr.CloudTenantId);
                            MessageLogger logger = new MessageLogger(LogManager.StaticWriteInfo);

                            using (var configurationProxy = new APIConfigurationServiceProxy(
                                siteAddressBaseUri,
                                @"api\configuration", // TODO: Replace from PCR / incoming
                                pcr.CloudTenantId,
                                pcr.CloudPremiseKey,
                                pcr.CloudTenantClaim,
                                premiseAgent,
                                logger))
                            {
                                configuration = configurationProxy.GetConfiguration();
                            }

                            // TODO: Remove this sample invocation of tenant registration
                            //using (var registrationProxy = new APITenantRegistrationProxy(
                            //    siteAddressBaseUri,
                            //    @"api\tenantregistration",
                            //    pcr.CloudTenantId,
                            //    premiseAgent,
                            //    logger))
                            //{
                            //    var registration = registrationProxy.RegisterTenant("ABC");
                            //    var tenantClaim = registration.TenantClaim;
                            //    var tenantId = registration.TenantId;
                            //    var tenantKey = registration.TenantKey;
                            //    var tenantName = registration.TenantName;
                            //    var tenantURL = registration.TenantUrl;
                            //    var siteURL = registration.SiteAddressBaseUri;

                            //    if ((tenantClaim == tenantKey) && (tenantId == tenantName) && (tenantURL == siteURL))
                            //    {
                            //        configuration.TenantName = tenantName;
                            //    }
                            //}

                            // Throw an exception if we did not get the results
                            if (null == configuration)
                            {
                                throw new Exception(
                                    String.Format("Could not retrieve remote config data for tenant {0}", pcr.CloudTenantId));
                            }

                            // Double check that we have not been cancelled before setting results
                            _remoteConfigRetrievalTokenSource.Token.ThrowIfCancellationRequested();

                            // Call to update params in PCR if needed and restart workers
                            RetrieveRemoteConfigDataCompleted(pcr, configuration, siteAddressBaseUri);

                            //consider updating the the state service for "unusual" cases if needed. Disabled tenant removed(mostly seems to work), incompatible version from cloud changes downward, etc.

                            // Task is complete
                            _activeRemoteConfigRetrievalTask = null;
                        });

                    // Run the retry action in a SEPARATE THREAD
                    // This is to prevent any blocking on the controller, since there are
                    // Potentially other tenant work coordinators that have things to do
                    // Note: store the configuration task reference in case of multiple successive 
                    // Retrieve remote configuration requests
                    _remoteConfigRetrievalTokenSource = new CancellationTokenSource();
                    _activeRemoteConfigRetrievalTask = Task.Factory.StartNew(
                        () =>
                        {
                            using (var lm = new LogManager())
                            {
                                // Set up the task retry manager
                                TaskRetryManager retryManager = new TaskRetryManager(
                                    TaskRetrySchedulerFactory.CreateGetRemoteConfigTaskRetryScheduler(pcr.MinCommunicationFailureRetryInterval, pcr.MaxCommunicationFailureRetryInterval),
                                    new GenericTaskRetryTransientErrorDetector(),
                                    lm);

                                // Execute our action in a retry
                                retryManager.ExecuteWithRetry(retryAction, _remoteConfigRetrievalTokenSource.Token);
                            }
                        },
                        _remoteConfigRetrievalTokenSource.Token);
                }
            }
        }

        /// <summary>
        /// Handle the successful completion of our remote configuration retrieval thread
        /// </summary>
        /// <param name="pcr"></param>
        /// <param name="configuration"></param>
        /// <param name="siteAddressBaseUri"></param>
        private void RetrieveRemoteConfigDataCompleted(
            PremiseConfigurationRecord pcr, Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configuration, Uri siteAddressBaseUri)
        {
            RaiseObjectDisposedExceptionIfNeeded();

            // Special logging diagnostics because we are encountering null exceptions in the field in this method
            using (var lm = new LogManager())
            {
                if (null == pcr)
                {
                    lm.WriteError(this, "Null premise configuration record supplied to RetrieveRemoteConfigDataCompleted");
                }
                else
                {
                    lm.WriteInfo(this, "Remote configuration data retrieved for tenant ID {0}", pcr.CloudTenantId);
                }

                if (null == configuration)
                {
                    lm.WriteError(this, "Null configuration parameters supplied to RetrieveRemoteConfigDataCompleted");
                }
                else
                {
                    string configParamsString = RemoteConfigHelper.ObjectPropertiesToString(configuration);
                    lm.WriteInfo(this, "Configuration parameters retrieved: {0}", configParamsString);
                }


                if (null == siteAddressBaseUri)
                {
                    lm.WriteError(this, "Null base site address supplied to RetrieveRemoteConfigDataCompleted");
                }
                else
                {
                    lm.WriteInfo(this, "Base site address retrieved: {0}", siteAddressBaseUri.ToString());
                }
            }

            lock (_syncRoot)
            {
                // Store the results
                _remoteConfiguration = configuration;

                // Get the millisecond values for the min/max communication retry interval
                int minCommunicationFailureRetryInterval = (int)_remoteConfiguration.MinCommunicationFailureRetryInterval.TotalMilliseconds;
                int maxCommunicationFailureRetryInterval = (int)_remoteConfiguration.MaxCommunicationFailureRetryInterval.TotalMilliseconds;

                if (!siteAddressBaseUri.ToString().Equals(pcr.SiteAddress) ||
                    !minCommunicationFailureRetryInterval.Equals(pcr.MinCommunicationFailureRetryInterval) ||
                    !maxCommunicationFailureRetryInterval.Equals(pcr.MaxCommunicationFailureRetryInterval))
                {
                    // Update the PCR on the database if the gateway URL changed
                    // OR if the min/max communication retry interval values in the configuration params changed
                    // Note: this will trigger a call to ConfigurationUpdated via the notification
                    // Service, which will refresh the worker tasks.  That is why we don't
                    // Need to call RefreshWorkerTasks here if we are updating the PCR.
                    UpdatePremiseConfigurationRecordRemoteConfigData(pcr, siteAddressBaseUri, minCommunicationFailureRetryInterval, maxCommunicationFailureRetryInterval);
                }
                else
                {
                    // Didn't update the PCR, but still need to create or re-create 
                    // The get and put workers
                    RefreshWorkerTasks(pcr);
                }
                    
                try
                {
                    using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        proxy.NotifyConfigParamsObtained(pcr.CloudTenantId, configuration);
                    }
                }
                catch (Exception ex)
                {
                    SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Unable to notify of config params obtained: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Method to update the actual premise configuration record with any stored values that
        /// Have changed as a result of a remote configuration service info update notification
        /// </summary>
        /// <param name="pcr"></param>
        /// <param name="siteAddressBaseUri"></param>
        /// <param name="maxCommunicationFailureRetryInterval"></param>
        /// <param name="minCommunicationFailureRetryInterval"></param>
        private void UpdatePremiseConfigurationRecordRemoteConfigData(
            PremiseConfigurationRecord pcr,
            Uri siteAddressBaseUri,
            Int32 minCommunicationFailureRetryInterval,
            Int32 maxCommunicationFailureRetryInterval)
        {
            ArgumentValidator.ValidateNonNullReference(pcr, "pcr", _myTypeName + ".UpdatePremiseConfigurationRecordRemoteConfigData()");
            ArgumentValidator.ValidateNonNullReference(siteAddressBaseUri, "siteAddressBaseUri", _myTypeName + ".UpdatePremiseConfigurationRecordRemoteConfigData()");

            // Change existing values for the service endpoint
            pcr.SiteAddress = siteAddressBaseUri.ToString();

            // Update the min/max communication failure retry values
            pcr.MinCommunicationFailureRetryInterval = minCommunicationFailureRetryInterval;
            pcr.MaxCommunicationFailureRetryInterval = maxCommunicationFailureRetryInterval;

            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                // Perform the update
                proxy.UpdateConfiguration(pcr);
            }

            using (var lm = new LogManager())
            {
                lm.WriteInfo(this, "Messaging Service: Tenant {0} site address updated: {1}",
                    pcr.CloudTenantId,
                    pcr.SiteAddress);
            }
        }

        #endregion


        #region Initialization For Back Office Connection State

        /// <summary>
        /// Initialize the back office connection state for the state service for this connection.
        /// Should be called only on tenant work coordinator creation
        /// </summary>
        /// <param name="pcr"></param>
        private void InitBackOfficeConnectionState(PremiseConfigurationRecord pcr)
        {
            if (pcr == null)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, "Null premise configuration record passed to init back office connection state!");
                }
                return;
            }

            using (new StackTraceContext(this, "pcr.CloudTenantId={0}", pcr.CloudTenantId))
            {
                RaiseObjectDisposedExceptionIfNeeded();

                lock (_syncRoot)
                {
                    // Create the retry action for our thread
                    Action retryAction = new Action(() =>
                    {
                        // Check that we have not been cancelled before starting
                        _backOfficeVerificationTokenSource.Token.ThrowIfCancellationRequested();

                        // Call validate on the back office connection to get the status
                        BackOfficeConnectivityStatus status = BackOfficeConnectivityStatus.None;
                        ValidateBackOfficeConnectionResponse rawResponse = null;
                        using (var validationProxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            rawResponse = validationProxy.ValidateBackOfficeConnectionCredentialsAsString(pcr.ConnectorPluginId, pcr.BackOfficeConnectionCredentials);

                            // Get back office connection status                
                            if (rawResponse != null)
                            {
                                status = rawResponse.BackOfficeConnectivityStatus;
                            }
                        }

                        // Double check that we have not been cancelled before calling the state service
                        _backOfficeVerificationTokenSource.Token.ThrowIfCancellationRequested();

                        // Update the state service with the resulting status
                        using (var stateServiceProxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            stateServiceProxy.UpdateBackOfficeConnectivityStatus(pcr.CloudTenantId, status);
                        }

                        // Task is complete
                        _activeBackOfficeVerifactionTask = null;
                    });

                    // Start the task for the retry action
                    _backOfficeVerificationTokenSource = new CancellationTokenSource();
                    _activeBackOfficeVerifactionTask = Task.Factory.StartNew(
                        () =>
                        {
                            using (var lm = new LogManager())
                            {
                                // Set up the task retry manager
                                TaskRetryManager retryManager = new TaskRetryManager(
                                    TaskRetrySchedulerFactory.CreateInitBackOfficeConnectionTaskRetryScheduler(),
                                    new GenericTaskRetryTransientErrorDetector(),
                                    lm);

                                // Execute our action in a retry
                                retryManager.ExecuteWithRetry(retryAction, _backOfficeVerificationTokenSource.Token);
                            }
                        },
                        _backOfficeVerificationTokenSource.Token);
                }
            }
        }

        /// <summary>
        /// Stop active back office verification
        /// </summary>
        public void StopActiveBackOfficeVerification()
        {
            if (_activeBackOfficeVerifactionTask != null)
            {
                try
                {
                    // Signal a cancel and wait for completion
                    _backOfficeVerificationTokenSource.Cancel();
                    _activeBackOfficeVerifactionTask.Wait();
                }
                catch
                {
                    // Ignore any exceptions on cancelling
                }
                finally
                {
                    _activeBackOfficeVerifactionTask = null;
                }
            }
        }

        #endregion


        #region Private Fields

        private Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration _remoteConfiguration;

        private Task _activeRemoteConfigRetrievalTask = null;
        private CancellationTokenSource _remoteConfigRetrievalTokenSource = new CancellationTokenSource();

        private Task _activeBackOfficeVerifactionTask = null;
        private CancellationTokenSource _backOfficeVerificationTokenSource = new CancellationTokenSource();

        private static readonly String _myTypeName = typeof(TenantWorkCoordinator).FullName;

        /// <summary>
        /// The synchronization object used to guard the internal data fields of this class against multiple thread access
        /// </summary>
        private readonly Object _syncRoot = new Object();

        /// <summary>
        /// The synchronization object passed on to the actual work manager tasks to ensure that we aren't trying to
        /// PutResponses and GetRequests on the cloud at the same time for this tenant
        /// </summary>
        private readonly Object _workSyncRoot = new Object();

        private CancellationTokenSource _workerCancellationTokenSource;

        private RequestAvailabilityWorkManager _requestAvailabilityWorkManager;
        private GetRequestsWorkManager _getRequestsWorkManager;
        private PutResponsesWorkManager _putResponsesWorkManager;

        private Boolean _disposed;

        #endregion
    }
}
