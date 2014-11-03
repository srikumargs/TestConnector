using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using Sage.Connector.AutoUpdate;
using Sage.Connector.AutoUpdate.Addin;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Interfaces;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.DispatchService.Internal;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Interfaces;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Diagnostics;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.DispatchService
{
    /// <summary>
    /// Container for all tenant-specific coordinators
    /// Takes care of creation/maintenance/deletion of coordinators
    /// And handles/forwards processing of relevant notifications
    /// </summary>
    internal sealed class DispatchController
    {
        /// <summary>
        ///  Class for storing tenants max blob size.
        /// </summary>
        internal class TenantBlobSize
        {
            public string TenantId { get; set; }
            public long MaxBlobSize { get; set; }
        }

        #region Public Methods

        /// <summary>
        /// Starts up the functionality of the Controller including:
        /// - subscribing to configuration service events
        /// - creating TenantWorkCoordinators
        /// - creating the auto-update process for the addin folder.
        /// </summary>
        public void Startup()
        {
            Assertions.Assert(_notificationSubscriptionServiceProxy == null,
                String.Format("_notificationSubscriptionServiceProxy is not null in '{0}' Startup", _myTypeName));

            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    CreateUpdateHandler();

                    // Subscribe to notifications first, as part of the creation
                    // bootstrap of the tenant work coordinators may fire
                    // notifications.  Notifications handlers will be blocked
                    // by our _syncObject until we have finished creating all
                    // tenant work coordinators.
                    SubscribeToNotificationServiceEvents();

                    // Create the tenant-specific coordinators
                    CreateTenantWorkCoordinators();

                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "Dispatch service startup.");
                    }
                }
            }
        }

        /// <summary>
        /// Shuts down the functionality of the Controller including:
        /// - stopping/disposing the TenantWorkCoordinators       
        /// - unsubscribing from configuration service events
        /// </summary>
        public void Shutdown()
        {
            Assertions.Assert(_notificationSubscriptionServiceProxy == null,
                String.Format("_notificationSubscriptionServiceProxy is null in '{0}' Shutdown", _myTypeName));
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "Dispatch service shutdown.");
                    }

                    _activeUpdaters.Dispose();
                    

                    // Unsubscribe from the notification events
                    UnsubscribeFromNotificationServiceEvents();

                    // Clean up the coordinators
                    DisposeTenantWorkCoordinators();
                }
            }
        }

        /// <summary>
        /// Cancel in-progress work for a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        public void CancelInProgressWork(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".CancelInProgressWork()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                // invoke the event-handling code in order to not block the publisher's execution thread
                TaskFactoryHelper.StartNew(_factory,
                    tenantId,
                    theTenantId =>
                    {
                        try
                        {
                            // for performance reasons, this lock may be commented out
                            // as our internal state is not being affected
                            // (after sufficient testing)
                            lock (_syncObject)
                            {
                                TenantWorkCoordinator coordinator;
                                if (_coordinators.TryGetValue(theTenantId, out coordinator))
                                {
                                    // Found matching coordinator, call the appropriate handler
                                    coordinator.CancelInProgressWork();
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
        }

        /// <summary>
        /// Cancel in-progress work for a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="activityTrackingContextId"></param>
        public void CancelWork(String tenantId, String activityTrackingContextId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".CancelWork()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                // invoke the event-handling code in order to not block the publisher's execution thread
                TaskFactoryHelper.StartNew(_factory,
                    tenantId,
                    theTenantId =>
                    {
                        try
                        {
                            // for performance reasons, this lock may be commented out
                            // as our internal state is not being affected
                            // (after sufficient testing)
                            lock (_syncObject)
                            {
                                TenantWorkCoordinator coordinator;
                                if (_coordinators.TryGetValue(theTenantId, out coordinator))
                                {
                                    // Found matching coordinator, call the appropriate handler
                                    coordinator.CancelWork(activityTrackingContextId);
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
        }

        /// <summary>
        /// Cancel in-progress work for a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<RequestWrapper> InProgressWork(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".InProgressWork()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                try
                {
                    // for performance reasons, this lock may be commented out
                    // as our internal state is not being affected
                    // (after sufficient testing)
                    lock (_syncObject)
                    {
                        TenantWorkCoordinator coordinator;
                        if (_coordinators.TryGetValue(tenantId, out coordinator))
                        {
                            // Found matching coordinator, call the appropriate handler
                            return coordinator.InProgressWork();
                        }

                        using (var lm = new LogManager())
                        {
                            lm.WriteInfo(this, "Failed to locate an expected TenantCoordinator for {0}", tenantId);
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
            }

            return null;
        }

        #endregion


        #region Notification Callbacks

        /// <summary>
        /// Callback when cloud configuration parameters are obtained.
        /// </summary>
        /// <param name="tenantId"></param>    
        /// <param name="configParams"></param>
        public void ConfigParamsObtained(string tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            SetCloudConfigurationBlobSize(tenantId, configParams);
        }

        /// <summary>
        /// Callback when cloud configuration parameters are updated.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void ConfigParamsUpdated(string tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            SetCloudConfigurationBlobSize(tenantId, configParams);
        }

        /// <summary>
        /// Callback when configuration record is added
        /// </summary>
        /// <param name="tenantId"></param>
        public void ConfigurationAdded(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ConfigurationAdded()");

            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                PremiseConfigurationRecord configuration = GetConfiguration(tenantId);
                if (configuration != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(_factory,
                        configuration,
                        theConfiguration =>
                    {
                        try
                        {
                            lock (_syncObject)
                            {
                                long blobSize;

                                if (!_tenantMaxBlobSizes.TryGetValue(theConfiguration.CloudTenantId, out blobSize))
                                {
                                    blobSize = 0;
                                }

                                // add updater check here?

                                // Create a new coordinator for this configuration
                                TenantWorkCoordinator coordinator = new TenantWorkCoordinator(theConfiguration, blobSize);
                                _coordinators.Add(theConfiguration.CloudTenantId, coordinator);
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
        /// Callback when configuration record is updated
        /// </summary>
        /// <param name="tenantId"></param>
        public void ConfigurationUpdated(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ConfigurationUpdated()");
            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                PremiseConfigurationRecord configuration = GetConfiguration(tenantId);
                if (configuration != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(_factory,
                        configuration,
                        theConfiguration =>
                    {
                        try
                        {
                            lock (_syncObject)
                            {
                                TenantWorkCoordinator coordinator;
                                if (_coordinators.TryGetValue(theConfiguration.CloudTenantId, out coordinator))
                                {
                                    // Found the matchin coordinator, update it
                                    coordinator.ConfigurationUpdated(theConfiguration);
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
            }
        }

        /// <summary>
        /// Callback when configuration record is deleted
        /// </summary>
        /// <param name="tenantId"></param>
        public void ConfigurationDeleted(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ConfigurationDeleted()");
            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                // invoke the event-handling code in order to not block the publisher's execution thread
                TaskFactoryHelper.StartNew(_factory,
                    tenantId,
                    theTenantId =>
                {
                    try
                    {
                        lock (_syncObject)
                        {
                            TenantWorkCoordinator coordinator;
                            if (_coordinators.TryGetValue(theTenantId, out coordinator))
                            {
                                long blobSize;

                                _tenantMaxBlobSizes.TryRemove(theTenantId, out blobSize);
                                coordinator.ConfigurationDeleted();
                                _coordinators.Remove(theTenantId);
                                _activeUpdaters.DeleteUnusedAutoUpdateSupport();
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
        }

        /// <summary>
        /// Callback when a binder element becomes available
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void BinderElementAvailable(String tenantId, String elementId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".BinderElementAvailable()");
            ArgumentValidator.ValidateNonEmptyString(elementId, "elementId", _myTypeName + ".BinderElementAvailable()");

            using (new StackTraceContext(this, "tenantId={0}, elementId={1}", tenantId, elementId))
            {
                // invoke the event-handling code in order to not block the publisher's execution thread
                TaskFactoryHelper.StartNew(_factory,
                    tenantId,
                    theTenantId =>
                {
                    try
                    {
                        // for performance reasons, this lock may be commented out
                        // as our internal state is not being affected
                        // (after sufficient testing)
                        lock (_syncObject)
                        {
                            TenantWorkCoordinator coordinator;
                            if (_coordinators.TryGetValue(theTenantId, out coordinator))
                            {
                                // Found matching coordinator, call the appropriate handler
                                coordinator.MessageReadyForBinding();
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
        }

        /// <summary>
        /// Callback when a binder element becomes available
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void BinderElementCompleted(String tenantId, String elementId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".BinderElementCompleted()");
            ArgumentValidator.ValidateNonEmptyString(elementId, "elementId", _myTypeName + ".BinderElementCompleted()");

            using (new StackTraceContext(this, "tenantId={0}, elementId={1}", tenantId, elementId))
            {
                // invoke the event-handling code in order to not block the publisher's execution thread
                TaskFactoryHelper.StartNew(_factory,
                    new Tuple<String, String>(tenantId, elementId),
                    tuple =>
                    {
                        try
                        {
                            string theTenantId = tuple.Item1;
                            string theElementId = tuple.Item2;

                            // for performance reasons, this lock may be commented out
                            // as our internal state is not being affected
                            // (after sufficient testing)
                            lock (_syncObject)
                            {
                                TenantWorkCoordinator coordinator;
                                if (_coordinators.TryGetValue(theTenantId, out coordinator))
                                {
                                    // Found matching coordinator, call the appropriate handler
                                    coordinator.BindingWorkCompleted(theElementId);
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
        }

        /// <summary>
        /// Callback when an item is ready for dispatching
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void MessageReadyForDispatch(QueueTypes queueType, String tenantId, Guid messageId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".MessageReadyForDispatch()");

            using (new StackTraceContext(this, "queueType={0}, tenantId={1}, messageId={2}", queueType, tenantId, messageId))
            {
                if (queueType == QueueTypes.Inbox)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(_factory, 
                        new Tuple<String, Guid>(tenantId, messageId),
                        tuple =>
                        {
                            try
                            {
                                string theTenantId = tuple.Item1;
                                Guid theMessageId = tuple.Item2;

                                // for performance reasons, this lock may be commented out
                                // as our internal state is not being affected
                                // (after sufficient testing)
                                lock (_syncObject)
                                {
                                    foreach (AddinUpdater updater in _activeUpdaters.Updaters)
                                    {
                                        if (updater.IsUpdateAvailable() && updater.ShouldApplyUpdate())
                                        {
                                            var count = _coordinators.Values.Sum(item => item.InProgressWorkCount());
                                            //only attempt to apply if no work in progress
                                            //note may eventually want to segment this based on package.
                                            if (count == 0)
                                            {
                                                if (!updater.ApplyUpdate())
                                                {
                                                    updater.IncrementFailureCount();
                                                }
                                            }
                                        }
                                    }

                                    //Check for the empty guid. It is passed in to create a "phantom" message to force an update check without
                                    //actually queuing a message
                                    if (string.Compare(theTenantId, Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        TenantWorkCoordinator coordinator;
                                        if (_coordinators.TryGetValue(theTenantId, out coordinator))
                                        {
                                            // Found matching coordinator, call the appropriate handler
                                            coordinator.MessageReadyForDispatch(theTenantId, theMessageId.ToString());
                                        }
                                        else
                                        {
                                            using (var lm = new LogManager())
                                            {
                                                lm.WriteInfo(this, "Failed to locate an expected TenantCoordinator for {0}", theTenantId);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        using (var lm = new LogManager())
                                        {
                                            lm.WriteInfo(this, "Processed phantom message to apply auto update, updates");
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
        /// Queues the phantom message to force update apply.
        /// </summary>
        /// <remarks>   
        /// Queue a "phantom message" 
        /// </remarks>
        private void QueuePhantomMessageToForceUpdateApply()
        {
            MessageReadyForDispatch(QueueTypes.Inbox, Guid.Empty.ToString(), Guid.Empty);
        }

        /// <summary>
        /// Callback when a tenant restart is requested
        /// </summary>
        /// <param name="tenantId"></param>
        public void TenantRestart(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".TenantRestart()");
            using (new StackTraceContext(this, "tenantId={0}", tenantId))
            {
                PremiseConfigurationRecord configuration = GetConfiguration(tenantId);
                if (configuration != null)
                {
                    // invoke the event-handling code in order to not block the publisher's execution thread
                    TaskFactoryHelper.StartNew(_factory,
                        configuration,
                        theConfiguration =>
                        {
                            try
                            {
                                lock (_syncObject)
                                {
                                    TenantWorkCoordinator coordinator;
                                    if (_coordinators.TryGetValue(theConfiguration.CloudTenantId, out coordinator))
                                    {
                                        // Found the matchin coordinator, update it
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
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Subscribe to all notifications of interest
        /// </summary>
        private void SubscribeToNotificationServiceEvents()
        {
            // Setup the notification subscription service
            _callbackInstance = new NotificationCallbackInstanceHelper();
            _notificationSubscriptionServiceProxy =
                NotificationSubscriptionServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber, _callbackInstance);
            _notificationSubscriptionServiceProxy.Connect();

            // Add subscriptions
            Task[] tasks =
            {
                Task.Factory.StartNew(() => _callbackInstance.SubscribeConfigurationAdded(
                    _notificationSubscriptionServiceProxy,
                    ConfigurationAdded)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeConfigurationUpdated(
                    _notificationSubscriptionServiceProxy,
                    ConfigurationUpdated)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeConfigurationDeleted(
                    _notificationSubscriptionServiceProxy,
                    ConfigurationDeleted)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeBinderElementEnqueued(
                    _notificationSubscriptionServiceProxy,
                    BinderElementAvailable)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeBinderElementRestored(
                    _notificationSubscriptionServiceProxy,
                    BinderElementAvailable)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeBinderElementCompleted(
                    _notificationSubscriptionServiceProxy,
                    BinderElementCompleted)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeMessageEnqueued(
                    _notificationSubscriptionServiceProxy,
                    MessageReadyForDispatch)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeTenantRestart(
                    _notificationSubscriptionServiceProxy,
                    TenantRestart)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeConfigParamsUpdated(
                                           _notificationSubscriptionServiceProxy,
                                           ConfigParamsUpdated)),
                Task.Factory.StartNew(() => _callbackInstance.SubscribeConfigParamsObtained(
                                           _notificationSubscriptionServiceProxy,
                                           ConfigParamsObtained))
            };
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Clean up the subscription service and all subscriptions
        /// </summary>
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

        
        /// <summary>
        /// Creates the addin update handler associated with the dispatch controller.
        /// </summary>
        private void CreateUpdateHandler()
        {
            _activeUpdaters = new ActiveUpdaters(GetConfigurations, QueuePhantomMessageToForceUpdateApply);
            _activeUpdaters.CreateUpdateHandlers();
        }

        /// <summary>
        /// Checks for automatic updates.
        /// </summary>
        internal void CheckForAutoUpdates()
        {
            _activeUpdaters.CheckForAutoUpdates();
        }

        /// <summary>
        /// Downloads the back office plugin.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="autoUpdateUri">The automatic update URI.</param>
        /// <param name="autoUpdateProductId">The automatic update product identifier.</param>
        /// <param name="autoUpdateProductVersion">The automatic update product version.</param>
        /// <param name="autoUpdateComponentBaseName">Name of the automatic update component base.</param>
        internal bool DownloadBackOfficePlugin(string backOfficeId, Uri autoUpdateUri, string autoUpdateProductId,
            string autoUpdateProductVersion,
            string autoUpdateComponentBaseName)
        {
            bool retval =_activeUpdaters.DownloadBackOfficePlugin(backOfficeId, autoUpdateUri, autoUpdateProductId, autoUpdateProductVersion,
                autoUpdateComponentBaseName);
            return retval;
        }

        /// <summary>
        /// Create a tenant work coordinator for each record in the premise configuration table
        /// </summary>
        private void CreateTenantWorkCoordinators()
        {
            PremiseConfigurationRecord[] configurations = GetConfigurations();

            if (configurations != null)
            {
                foreach (var configuration in configurations)
                {
                    long blobSize;

                    if (!_tenantMaxBlobSizes.TryGetValue(configuration.CloudTenantId, out blobSize))
                    {
                        blobSize = 0;
                    }
                    _coordinators.Add(configuration.CloudTenantId, new TenantWorkCoordinator(configuration, blobSize));
                }
            }
        }

        private PremiseConfigurationRecord[] GetConfigurations()
        {
            PremiseConfigurationRecord[] configurations;

            using (
                var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {

                try
                {
                    configurations = proxy.GetConfigurations();
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(this, "Dispatch Service", "Problem retrieving configurations: {0}; exception: {1}",
                            dafe.Reason, dafe.ExceptionAsString());
                    }
                    configurations = null;
                }
            }
            return configurations;
        }

        /// <summary>
        /// Get the premise configuration record for a specific tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        private PremiseConfigurationRecord GetConfiguration(String tenantId)
        {
            PremiseConfigurationRecord result;

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
                            lm.WriteError(this, "Dispatch Service: Unable to retrieve configuration for tenant '{0}'", tenantId);
                        }
                    }
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(this, "Dispatch Service", "Error retrieving configuration for tenant '{0}': {1}; exception: {2}", tenantId, dafe.Reason, dafe.ExceptionAsString());
                    }
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the max blob size for the tenant from the passed configuration parameters.
        /// </summary>
        /// <param name="tenantId">The tenant id that the configuration corresponds to.</param>
        /// <param name="configParams">The cloud configuration parameters for the tenant.</param>
        private void SetCloudConfigurationBlobSize(string tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".SetCloudConfigurationBlobSize()");
            ArgumentValidator.ValidateNonNullReference(configParams, "configParams", _myTypeName + ".SetCloudConfigurationBlobSize()");

            var configParamsString = configParams.GetType().FullName;

            using (new StackTraceContext(this, "tenantId={0}, configParams={1}", tenantId, configParamsString))
            {
                var tenantBlobSize = new TenantBlobSize {TenantId = tenantId, MaxBlobSize = configParams.MaxBlobSize};

                TaskFactoryHelper.StartNew(_factory, tenantBlobSize, theTenantBlobSize =>
                {
                    try
                    {
                        lock (_syncObject)
                        {
                            _tenantMaxBlobSizes.AddOrUpdate(tenantBlobSize.TenantId, tenantBlobSize.MaxBlobSize, (key, oldValue) => tenantBlobSize.MaxBlobSize);

                            TenantWorkCoordinator coordinator;
                            if (_coordinators.TryGetValue(tenantBlobSize.TenantId, out coordinator))
                            {
                                coordinator.SetMaxBlobSize(tenantBlobSize.MaxBlobSize);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(this, ex.ExceptionAsString());
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Cleanup all work coordinators
        /// </summary>
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


        

        #endregion


        #region Private Members
        
        //private List<AddinUpdater> _addinUpdaters = new List<AddinUpdater>();
        private readonly TaskFactory _factory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(Environment.ProcessorCount * 2));
        private static readonly String _myTypeName = typeof(DispatchController).FullName;
        private readonly Object _syncObject = new Object();
        private readonly ConcurrentDictionary<string, long> _tenantMaxBlobSizes = new ConcurrentDictionary<string, long>();
        private readonly Dictionary<String, TenantWorkCoordinator> _coordinators = new Dictionary<String, TenantWorkCoordinator>();
        private NotificationCallbackInstanceHelper _callbackInstance = new NotificationCallbackInstanceHelper();
        private NotificationSubscriptionServiceProxy _notificationSubscriptionServiceProxy;
        private ActiveUpdaters _activeUpdaters;

        #endregion
    }
}
