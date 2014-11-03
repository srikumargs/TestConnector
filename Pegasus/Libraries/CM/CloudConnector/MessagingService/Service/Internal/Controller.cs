using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Interfaces;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Documents;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Interfaces;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// The controller class manages a list of TenantWorkCoordinators and subscribes to the NotificationService events
    /// </summary>
    internal sealed class Controller : IDisposable
    {
        public Controller()
        {
            // Setup the process that checks all tenants controlled by this class
            // And determines based on their configuration whether or not we need to
            // Restart the hosting framework
            
            // Note: Commented out!  This will be implemented later in another location
            // SetupSystemRestartCheckProcess();
        }

        /// <summary>
        /// Starts up the functionality of the Controller including:
        /// - subscribing to configuration service events
        /// - creating TenantWorkCoordinators
        /// - starting the TenantWorkCoordinators
        /// </summary>
        public void Startup()
        {
            Assertions.Assert(_notificationSubscriptionServiceProxy == null, "_notificationSubscriptionServiceProxy is not null");

            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    // Subscribe to notifications first, as part of the creation
                    // bootstrap of the tenant work coordinators may fire
                    // notifications.  Notifications handlers will be blocked
                    // by our _syncObject until we have finished creating all
                    // tenant work coordinators.
                    SubscribeToNotificationServiceEvents();

                    RestoreAllPendingMessages();
                    PremiseConfigurationRecord[] configurations = GetConfigurations();
                    if (configurations != null && configurations.Length > 0)
                    {
                        CreateTenantWorkCoordinators(configurations);
                    }

                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "Message service startup.");
                    } 
                }
            }
        }

        /// <summary>
        /// Shuts down the functionality of the Controller including:
        /// - stopping the TenantWorkCoordinators       
        /// - disposing TenantWorkCoordinators
        /// - unsubscribing from configuration service events
        /// </summary>
        public void Shutdown()
        {
            Assertions.Assert(_notificationSubscriptionServiceProxy != null, "_notificationSubscriptionServiceProxy is null");

            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "Message service shutdown.");
                    } 

                    ConnectorClientFactory.Shutdown();
                    UnsubscribeFromNotificationServiceEvents();
                    StopTenantWorkCoordinators();
                    DisposeTenantWorkCoordinators();
                }
            }
        }


        #region INotificationCallback

        /// <summary>
        /// Occurs when a configuration is added
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the added configuration</param>
        public void ConfigurationAdded(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ConfigurationAdded()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Tenant {0} added.", tenantId);
                } 

                var configuration = GetConfiguration(tenantId);
                if (configuration != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(
                        configuration,
                        (theConfiguration) =>
                        {
                            try
                            {
                                lock (_syncObject)
                                {
                                    // Note: no need to call StartWorkerThreads because they will 
                                    // Be automatically started once the remote config thread completes
                                    CreateAndAddNewTenantWorkCoordinator(theConfiguration);
                                }
                            }
                            catch (Exception ex)
                            {
                                // don't let any unintended exception escape the IConfigurationCallback interface boundary
                                using (var lm = new LogManager())
                                {
                                    lm.WriteError(this, ex.ExceptionAsString());
                                }
                            }
                        });
                }
                else
                {
                    // Configuration could not be found!
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, "Could not find expected configuration to add for tenantId {0}", tenantId);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when an existing configuration is updated
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the updated configuration</param>
        public void ConfigurationUpdated(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ConfigurationUpdated()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Tenant {0} updated.", tenantId);
                } 

                var configuration = GetConfiguration(tenantId);
                if (configuration != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(
                        configuration,
                        (theConfiguration) =>
                    {
                        try
                        {
                            lock (_syncObject)
                            {
                                TenantWorkCoordinator coordinator = null;
                                if (_coordinators.TryGetValue(theConfiguration.CloudTenantId, out coordinator))
                                {
                                    coordinator.UpdateConfiguration(theConfiguration);
                                }
                                else
                                {
                                    // for some reason we missed the original ConfigurationAdded event ... add it now
                                    ConfigurationAdded(theConfiguration.CloudTenantId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // don't let any unintended exception escape the IConfigurationCallback interface boundary
                            using (var lm = new LogManager())
                            {
                                lm.WriteError(this, ex.ExceptionAsString());
                            }
                        }
                        });

                }
                else
                {
                    // Configuration could not be found!
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, "Could not find expected configuration to update for tenantId {0}", tenantId);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when an existing configuration is deleted
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the deleted configuration</param>
        public void ConfigurationDeleted(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ConfigurationDeleted()");

            using (new StackTraceContext("tenantId={0}", tenantId))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Tenant {0} deleted.", tenantId);
                } 

                // invoke the event-handling code in order to not block the publisher's execution thread
                TaskFactoryHelper.StartNew(
                    tenantId,
                    (theTenantId) =>
                {
                    try
                    {
                        lock (_syncObject)
                        {
                            TenantWorkCoordinator coordinator = null;
                            if (_coordinators.TryGetValue(theTenantId, out coordinator))
                            {
                                coordinator.StopActiveBackOfficeVerification();
                                coordinator.StopActiveRemoteConfigRetrieval();
                                coordinator.StopWorkers();
                                coordinator.Dispose();
                                _coordinators.Remove(theTenantId);
                            }
                            else
                            {
                                using (var lm = new LogManager())
                                {
                                    lm.WriteInfo(this, "Failed to locate an expected TenantCoordinator for {0}", tenantId);
                                }
                            }
                        }

                        // Clean up the queues
                        using (var qm = new QueueManager())
                        {
                            qm.DeleteTenantMessages(theTenantId);
                            using (var lm = new LogManager())
                            {
                                lm.DeleteTenantActivity(theTenantId);
                            }
                        }

                        // Clean up the document manager
                        DocumentManager dm = new DocumentManager();
                        dm.DeleteTenant(theTenantId);
                    }
                    catch (Exception ex)
                    {
                        // don't let any unintended exception escape the IConfigurationCallback interface boundary
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(this, ex.ExceptionAsString());
                        }
                    }
                    });
            }
        }

        /// <summary>
        /// Occurs when we are disconnected from the long-poll message availability channel - wake up availability worker
        /// </summary>
        public void RestoreRequestMessageAvailabilityChannels()
        {
            using (var lm = new LogManager())
            {
                lm.WriteInfo(this, "Messaging Service: Lost connectivity; restarting tenants.");
            }

            var configurations = GetConfigurations();
            Parallel.ForEach(configurations, configuration => TenantRestart(configuration.CloudTenantId));
            //Parallel.ForEach(configurations, configuration => ResetMessageAvailability(configuration.CloudTenantId));
        }

        /// <summary>
        /// Occurs when a request message is available for retrieval - wake up get request worker
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void RequestMessageAvailable(String tenantId, Guid messageId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".RequestMessageAvailable()");

            using (new StackTraceContext(this, "tenantId={0}, messageId={1}", tenantId, messageId))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Tenant {0} message available.", tenantId);
                } 

                // invoke the event-handling code in order to not block the publisher's execution thread
                TaskFactoryHelper.StartNew(
                    new Tuple<String, Guid>(tenantId, messageId),
                    (tuple) =>
                    {
                        try
                        {
                            lock (_syncObject)
                            {
                                TenantWorkCoordinator coordinator = null;
                                if (_coordinators.TryGetValue(tuple.Item1, out coordinator))
                                {
                                    coordinator.SetInboxMessageReadyEvent(tuple.Item2);
                                }
                                else
                                {
                                    using (var lm = new LogManager())
                                    {
                                        lm.WriteInfo(this, "Failed to locate an expected TenantCoordinator for {0}", tuple.Item1);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // don't let any unintended exception escape the IConfigurationCallback interface boundary
                            using (var lm = new LogManager())
                            {
                                lm.WriteError(this, ex.ExceptionAsString());
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Occurs when a message is enqueued - capture outbox puts
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void MessageEnqueued(QueueTypes queueType, String tenantId, Guid messageId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".MessageEnqueued()");

            using (new StackTraceContext(this, "queueType={0}, tenantId={1}, messageId={2}", queueType, tenantId, messageId))
            {
                if (queueType == QueueTypes.Outbox)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(
                        new Tuple<String, Guid>(tenantId, messageId),
                        (tuple) =>
                        {
                            try
                            {
                                lock (_syncObject)
                                {
                                    TenantWorkCoordinator coordinator = null;
                                    if (_coordinators.TryGetValue(tuple.Item1, out coordinator))
                                    {
                                        coordinator.SetOutboxMessageReadyEvent(tuple.Item2);
                                    }
                                    else
                                    {
                                        using (var lm = new LogManager())
                                        {
                                            lm.WriteInfo(this, "Failed to locate an expected TenantCoordinator for {0}", tuple.Item1);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // don't let any unintended exception escape the IConfigurationCallback interface boundary
                                using (var lm = new LogManager())
                                {
                                    lm.WriteError(this, ex.ExceptionAsString());
                                }
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Callback when the config params are updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void ConfigParamsUpdated(String tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ConfigParamsUpdated()");
            ArgumentValidator.ValidateNonNullReference(configParams, "configParams", _myTypeName + ".ConfigParamsUpdated()");
            
            string configParamsString = RemoteConfigHelper.ObjectPropertiesToString(configParams);
            using (new StackTraceContext(this, "tenantId={0}, configParams={1}", tenantId, configParamsString))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Configuration parameters for tenant {0} updated.", tenantId);
                } 

                // Get the existing configuration since we will need to update the worker threads
                var pcr = GetConfiguration(tenantId);
                if (pcr != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(
                        new Tuple<String, CloudContracts.WebAPI.Configuration>(tenantId, configParams),
                        (tuple) =>
                        {
                            try
                            {
                                string theTenantId = tuple.Item1;
                                CloudContracts.WebAPI.Configuration theConfigParams = tuple.Item2;

                                // for performance reasons, this lock may be commented out
                                // as our internal state is not being affected
                                // (after sufficient testing)
                                lock (_syncObject)
                                {
                                    TenantWorkCoordinator coordinator = null;
                                    if (_coordinators.TryGetValue(theTenantId, out coordinator))
                                    {
                                        // Found matching coordinator, call the appropriate handler
                                        coordinator.SetRemoteConfigParams(configParams, pcr);

                                        // Update the smallest min connector uptime, if necessary
                                        UpdateSmallestMaxConnectorUptime(configParams);
                                    }
                                    else
                                    {
                                        using (var lm = new LogManager())
                                        {
                                            lm.WriteInfo(this, "Failed to locate an expected TenantCoordinator for {0}", tenantId);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // don't let any unintended exception escape the IConfigurationCallback interface boundary
                                using (var lm = new LogManager())
                                {
                                    lm.WriteError(this, ex.ExceptionAsString());
                                }
                            }
                        });
                }
                else
                {
                    // Configuration could not be found!
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, "Could not find expected configuration when updating remote config params for tenantId {0}", tenantId);
                    }
                }
            }
        }

        /// <summary>
        /// Callback when the service info is updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="siteAddressBaseUri"></param>
        public void ServiceInfoUpdated(String tenantId, Uri siteAddressBaseUri)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ServiceInfoUpdated()");
            ArgumentValidator.ValidateNonNullReference(siteAddressBaseUri, "siteAddress", _myTypeName + ".ServiceInfoUpdated()");

            using (new StackTraceContext(this, "tenantId={0}, siteAddressBaseUri={1}", tenantId, siteAddressBaseUri.ToString()))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Service information for tenant {0} updated.", tenantId);
                } 

                // Get the existing configuration since we will need to update values there in 
                // Addition to updating the in-memory values
                var pcr = GetConfiguration(tenantId);
                if (pcr != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(
                        new Tuple<String, Uri>(tenantId, siteAddressBaseUri),
                        (tuple) =>
                        {
                            try
                            {
                                string theTenantId = tuple.Item1;
                                Uri theSiteAddressBaseUri = tuple.Item2;

                                // for performance reasons, this lock may be commented out
                                // as our internal state is not being affected
                                // (after sufficient testing)
                                lock (_syncObject)
                                {
                                    TenantWorkCoordinator coordinator = null;
                                    if (_coordinators.TryGetValue(theTenantId, out coordinator))
                                    {
                                        // Found matching coordinator, call the appropriate handler
                                        coordinator.SetRemoteServiceInfo(theSiteAddressBaseUri, pcr);
                                    }
                                    else
                                    {
                                        using (var lm = new LogManager())
                                        {
                                            lm.WriteInfo(this, "Failed to locate an expected TenantCoordinator for {0}", tenantId);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // don't let any unintended exception escape the IConfigurationCallback interface boundary
                                using (var lm = new LogManager())
                                {
                                    lm.WriteError(this, ex.ExceptionAsString());
                                }
                            }
                        });
                }
                else
                {
                    // Configuration could not be found!
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, "Could not find expected configuration when attempting to update site service info for tenantId {0}", tenantId);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when a tenant restart is requested - typically when the database is
        /// experience problems with our worker's thread context
        /// </summary>
        /// <param name="tenantId"></param>
        public void TenantRestart(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".TenantRestart()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Tenant {0} restarting.", tenantId);
                }

                var configuration = GetConfiguration(tenantId);
                if (configuration != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(
                        configuration,
                        (theConfiguration) =>
                        {
                            try
                            {
                                lock (_syncObject)
                                {
                                    TenantWorkCoordinator coordinator = null;
                                    if (_coordinators.TryGetValue(theConfiguration.CloudTenantId, out coordinator))
                                    {
                                        coordinator.RestartTenant(theConfiguration);
                                    }
                                    else
                                    {
                                        // for some reason we missed the original ConfigurationAdded event ... add it now
                                        ConfigurationAdded(theConfiguration.CloudTenantId);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // don't let any unintended exception escape the IConfigurationCallback interface boundary
                                using (var lm = new LogManager())
                                {
                                    lm.WriteError(this, ex.ExceptionAsString());
                                }
                            }
                        });

                }
                else
                {
                    // Configuration could not be found!
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, "Could not find expected configuration to update for tenantId {0}", tenantId);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void ResetMessageAvailability(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ResetMessageAvailability()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Messaging Service: Tenant {0} message availability reset.", tenantId);
                }

                var configuration = GetConfiguration(tenantId);
                if (configuration != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(
                        configuration,
                        (theConfiguration) =>
                        {
                            try
                            {
                                lock (_syncObject)
                                {
                                    TenantWorkCoordinator coordinator = null;
                                    if (_coordinators.TryGetValue(theConfiguration.CloudTenantId, out coordinator))
                                    {
                                        coordinator.SetRequestAvailableResubscribeEvent();
                                    }
                                    else
                                    {
                                        // for some reason we missed the original ConfigurationAdded event ... add it now
                                        ConfigurationAdded(theConfiguration.CloudTenantId);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // don't let any unintended exception escape the IConfigurationCallback interface boundary
                                using (var lm = new LogManager())
                                {
                                    lm.WriteError(this, ex.ExceptionAsString());
                                }
                            }
                        });

                }
                else
                {
                    // Configuration could not be found!
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, "Could not find expected configuration to update for tenantId {0}", tenantId);
                    }
                }
            }
        }

        public void IncompatibleClient()
        {
            foreach (var configuration in _coordinators)
            {

                string tenantId = configuration.Key;
                TenantWorkCoordinator coordinator = configuration.Value;
                var pcr = GetConfiguration(tenantId);

                if (coordinator != null && pcr != null)
                {
                    coordinator.StopAndDisableConfiguration(pcr);
                }
            }
        }


        #endregion

        
        #region Private methods
        /// <summary>
        /// Call this when a new coordinator is added, or any config params are updated.
        /// This updates the interval after which we should restart the hosting framework
        /// </summary>
        /// <param name="configParams"></param>
        private void UpdateSmallestMaxConnectorUptime(CloudContracts.WebAPI.Configuration configParams)
        {
            // If the config param value is not null, and either the stored smallest value is null
            // Or it is bigger than this value, then we want to set the stored value
            if (configParams != null &&
                (_smallestMaxConnectorUptimeAcrossCoordinators == null ||
                configParams.SuggestedMaxConnectorUptimeDuration < _smallestMaxConnectorUptimeAcrossCoordinators))
            {
                _smallestMaxConnectorUptimeAcrossCoordinators = configParams.SuggestedMaxConnectorUptimeDuration;
                CloudConnectivityStateMonitorHelper.UpdateMaxUptimeUntilRestart(_smallestMaxConnectorUptimeAcrossCoordinators);
            }
        }

        private void SubscribeToNotificationServiceEvents()
        {
            _callbackInstance = new NotificationCallbackInstanceHelper();
            _notificationSubscriptionServiceProxy =
                NotificationSubscriptionServiceProxyFactory.CreateFromCatalog("localhost",
                                                                              ConnectorServiceUtils.
                                                                                  CatalogServicePortNumber,
                                                                              _callbackInstance);
            _notificationSubscriptionServiceProxy.Connect();
            Task[] tasks = new Task[]
                               {
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeConfigurationAdded(
                                           _notificationSubscriptionServiceProxy,
                                           ConfigurationAdded)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeConfigurationUpdated(
                                           _notificationSubscriptionServiceProxy,
                                           ConfigurationUpdated)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeConfigurationDeleted(
                                           _notificationSubscriptionServiceProxy,
                                           ConfigurationDeleted)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeRestoreRequestMessageAvailableConnections(
                                           _notificationSubscriptionServiceProxy,
                                           RestoreRequestMessageAvailabilityChannels)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeRequestMessageAvailable(
                                           _notificationSubscriptionServiceProxy,
                                           RequestMessageAvailable)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeMessageEnqueued(
                                           _notificationSubscriptionServiceProxy,
                                           MessageEnqueued)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeConfigParamsUpdated(
                                           _notificationSubscriptionServiceProxy,
                                           ConfigParamsUpdated)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeServiceInfoUpdated(
                                           _notificationSubscriptionServiceProxy,
                                           ServiceInfoUpdated)),
                                   Task.Factory.StartNew(
                                       () =>
                                       _callbackInstance.SubscribeTenantRestart(
                                           _notificationSubscriptionServiceProxy,
                                           TenantRestart)),
                                   Task.Factory.StartNew(
                                        () => 
                                        _callbackInstance.SubscribeIncompatibleClient(
                                            _notificationSubscriptionServiceProxy, 
                                            IncompatibleClient)),
                               };
            // We need the handlers all hooked up to handle the startup renotifications
            Task.WaitAll(tasks);
        }

        private void RestoreAllPendingMessages()
        {
            try
            {
                using (var qm = new QueueManager())
                {
                    qm.RestoreAllProcessingMessages();
                }
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.Queues);
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteCriticalWithEventLogging(this, "Messaging Service", "Error encountered restoring pending messages; exception: {0}", ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.Queues, ex.ExceptionAsString(), "Error restoring working message to the inbox: " + ex.Message);
            }
        }

        private PremiseConfigurationRecord[] GetConfigurations()
        {
            PremiseConfigurationRecord[] result = null;

            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    result = proxy.GetConfigurations();
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(this, "Messaging Service", "Problem retrieving configurations: {0}; exception: {1}", dafe.Reason, dafe.ExceptionAsString());
                    }
                    result = null;
                }
            }

            return result;
        }

        private void CreateTenantWorkCoordinators(PremiseConfigurationRecord[] configurations)
        {
            foreach (var configuration in configurations)
            {
                CreateAndAddNewTenantWorkCoordinator(configuration);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateAndAddNewTenantWorkCoordinator(PremiseConfigurationRecord pcr)
        {
            // Create and add a new work coordinator
            TenantWorkCoordinator coordinator = new TenantWorkCoordinator(pcr);
            _coordinators.Add(pcr.CloudTenantId, coordinator);

            // Update the smallest min connector uptime, if necessary
            UpdateSmallestMaxConnectorUptime(coordinator.RemoteConfiguration);
        }

        private void StopTenantWorkCoordinators()
        {
            foreach (var coordinator in _coordinators)
            {
                coordinator.Value.StopActiveBackOfficeVerification();
                coordinator.Value.StopActiveRemoteConfigRetrieval();
                coordinator.Value.StopWorkers();
            }
        }

        private void DisposeTenantWorkCoordinators()
        {
            if (_coordinators != null)
            {
                foreach (KeyValuePair<string, TenantWorkCoordinator> keyValue in _coordinators)
                {
                    keyValue.Value.Dispose();
                }
                _coordinators.Clear();
            }
        }

        private void UnsubscribeFromNotificationServiceEvents()
        {
            if (_callbackInstance != null && _notificationSubscriptionServiceProxy != null)
            {
                try
                {
                    _callbackInstance.Unsubscribe(_notificationSubscriptionServiceProxy);
                    _notificationSubscriptionServiceProxy.Disconnect();
                    _notificationSubscriptionServiceProxy.Close();
                }
                finally
                {
                    _notificationSubscriptionServiceProxy.Abort();
                    _notificationSubscriptionServiceProxy = null;
                    _callbackInstance = null;
                }
            }
        }

        private PremiseConfigurationRecord GetConfiguration(String tenantId)
        {
            PremiseConfigurationRecord result = null;

            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    result = proxy.GetConfiguration(tenantId);

                    // We don't expect to be unable to retrieve a tenant-targed configuration
                    if (null == result)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(this, "Messaging Service: Unable to retrieve configuration for tenant '{0}'", tenantId);
                        }
                    }
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(this, "Messaging Service", "Problem retrieving configuration for tenant '{0}': {1}; exception: {2}", tenantId, dafe.Reason, dafe.ExceptionAsString());
                    }
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Set up and kick off the process that checks if we need to restart the system
        /// </summary>
        private void SetupSystemRestartCheckProcess()
        {
            _systemRestartCheckProcess = new System.Timers.Timer();
            _systemRestartCheckProcess.Elapsed += new System.Timers.ElapsedEventHandler(SystemRestartCheckTimerHandler);
            _systemRestartCheckProcess.Interval = _systemRestartCheckProcessInterval;
            _systemRestartCheckProcess.Start();
        }

        /// <summary>
        /// Coordinates the execution of the method to check if we have exceeded the suggested restart
        /// Duration, and restarts if possible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemRestartCheckTimerHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            // GGTODO
            //System.Timers.Timer timer = (System.Timers.Timer)sender;

            //try
            //{
            //    // Disable timer for debug purposes, in case someone
            //    // Sets the interval to be very short
            //    timer.Enabled = false;

            //    // Check if we are beyond the threshold
            //    if (_smallestMaxConnectorUptimeAcrossCoordinators != null)
            //    {
            //        TimeSpan totalUptime = DateTime.UtcNow - _startupUtc;
            //        if (totalUptime >= ((TimeSpan)_smallestMaxConnectorUptimeAcrossCoordinators)) 
            //        { 
            //            // GGTODO: Call the restart method
            //        }
            //    }
            //}
            //finally
            //{
            //    // Re-enable when complete
            //    timer.Enabled = true;
            //}
        }

        #endregion

        
        #region Private fields
        
        private static readonly String _myTypeName = typeof(Controller).FullName;
        private readonly Object _syncObject = new Object();
        private readonly Dictionary<String, TenantWorkCoordinator> _coordinators = new Dictionary<String, TenantWorkCoordinator>();
        private NotificationCallbackInstanceHelper _callbackInstance = new NotificationCallbackInstanceHelper();
        private NotificationSubscriptionServiceProxy _notificationSubscriptionServiceProxy;

        /// <summary>
        /// Store the time that this controller was created as the startup time
        /// </summary>
        private readonly DateTime _startupUtc = DateTime.UtcNow;

        /// <summary>
        /// Keep track of the smallest value for the "SuggestedMaxConnectorUptimeDuration" config param
        /// Across all tenant work coordinators. This is a changing value (e.g. when config params are updated,
        /// Or when a new configuration is added).  The whole hosting framework will restart when this duration
        /// Is exceeded, and when all queues are drained.  If the value remains null, no restart will ever happen.
        /// </summary>
        private TimeSpan? _smallestMaxConnectorUptimeAcrossCoordinators = null;

        /// <summary>
        /// Periodic process to check if we have exceeded the suggested max connector uptime duration
        /// After which, we will wait for the queues to drain and then restart the hosting framework
        /// </summary>
        private System.Timers.Timer _systemRestartCheckProcess;

        /// <summary>
        /// Interval for running the system restart checking process
        /// Left as not read only so internally we can test changing this value
        /// </summary>
        private double _systemRestartCheckProcessInterval = TimeSpan.FromMinutes(10).TotalMilliseconds;

        #endregion


        #region IDisposable Members

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (_systemRestartCheckProcess != null)
            {
                _systemRestartCheckProcess.Dispose();
            }
        }

        #endregion
    }
}
