using System;
using System.ServiceModel;
using Sage.Connector.NotificationService.Interfaces;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.NotificationService.Proxy
{
    /// <summary>
    /// An implementation of INotificationCallback that allows clients to only implement delegates for the 
    /// specific notifications they want to receive.
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext = false)]
    public sealed class NotificationCallbackInstanceHelper : INotificationCallback
    {
        /// <summary>
        /// Subscribe to ConfigurationAdded notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeConfigurationAdded(NotificationSubscriptionServiceProxy proxy, Action<String> action)
        {
            lock (_syncRoot)
            {
                _configurationAddedAction = action;
            }
            proxy.Subscribe("ConfigurationAdded");
        }

        /// <summary>
        /// Subscribe to ConfigurationUpdated notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeConfigurationUpdated(NotificationSubscriptionServiceProxy proxy, Action<String> action)
        {
            lock (_syncRoot)
            {
                _configurationUpdatedAction = action;
            }
            proxy.Subscribe("ConfigurationUpdated");
        }

        /// <summary>
        /// Subscribe to ConfigurationDeleted notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeConfigurationDeleted(NotificationSubscriptionServiceProxy proxy, Action<String> action)
        {
            lock (_syncRoot)
            {
                _configurationDeletedAction = action;
            }
            proxy.Subscribe("ConfigurationDeleted");
        }

        /// <summary>
        /// Subscribe to RequestMessageAvailable notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeRestoreRequestMessageAvailableConnections(NotificationSubscriptionServiceProxy proxy, Action action)
        {
            lock (_syncRoot)
            {
                _restoreRequestMessageAvailableConnectionsAction = action;
            }
            proxy.Subscribe("RestoreRequestMessageAvailableConnections");
        }

        /// <summary>
        /// Subscribe to RequestMessageAvailable notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeRequestMessageAvailable(NotificationSubscriptionServiceProxy proxy, Action<String, Guid> action)
        {
            lock (_syncRoot)
            {
                _requestMessageAvailableAction = action;
            }
            proxy.Subscribe("RequestMessageAvailable");
        }

        /// <summary>
        /// Subscribe to MessageEnqueued notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeMessageEnqueued(NotificationSubscriptionServiceProxy proxy, Action<QueueTypes, String, Guid> action)
        {
            lock (_syncRoot)
            {
                _messageEnqueuedAction = action;
            }
            proxy.Subscribe("MessageEnqueued");
        }

        /// <summary>
        /// Subscribe to BinderElementEnqueued notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeBinderElementEnqueued(NotificationSubscriptionServiceProxy proxy, Action<String, String> action)
        {
            lock (_syncRoot)
            {
                _binderElementEnqueuedAction = action;
            }
            proxy.Subscribe("BinderElementEnqueued");
        }

        /// <summary>
        /// Subscribe to BinderElementDeleted notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeBinderElementDeleted(NotificationSubscriptionServiceProxy proxy, Action<String, String> action)
        {
            lock (_syncRoot)
            {
                _binderElementDeletedAction = action;
            }
            proxy.Subscribe("BinderElementDeleted");
        }

        /// <summary>
        /// Subscribe to BinderElementRestored notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeBinderElementRestored(NotificationSubscriptionServiceProxy proxy, Action<String, String> action)
        {
            lock (_syncRoot)
            {
                _binderElementRestoredAction = action;
            }
            proxy.Subscribe("BinderElementRestored");
        }

        /// <summary>
        /// Subscribe to BinderElementCompleted notification
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeBinderElementCompleted(NotificationSubscriptionServiceProxy proxy, Action<String, String> action)
        {
            lock (_syncRoot)
            {
                _binderElementCompletedAction = action;
            }
            proxy.Subscribe("BinderElementCompleted");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeConfigParamsUpdated(NotificationSubscriptionServiceProxy proxy, Action<String, CloudContracts.WebAPI.Configuration> action)
        {
            lock (_syncRoot)
            {
                _configParamsUpdatedAction = action;
            }
            proxy.Subscribe("ConfigParamsUpdated");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeConfigParamsObtained(NotificationSubscriptionServiceProxy proxy, Action<String, CloudContracts.WebAPI.Configuration> action)
        {
            lock (_syncRoot)
            {
                _configParamsObtainedAction = action;
            }
            proxy.Subscribe("ConfigParamsObtained");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeServiceInfoUpdated(NotificationSubscriptionServiceProxy proxy, Action<String, Uri> action)
        {
            lock (_syncRoot)
            {
                _serviceInfoUpdatedAction = action;
            }
            proxy.Subscribe("ServiceInfoUpdated");
        }

        /// <summary>
        /// Subscribes to tenant restart
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeTenantRestart(NotificationSubscriptionServiceProxy proxy, Action<String> action)
        {
            lock (_syncRoot)
            {
                _tenantRestartAction = action;
            }
            proxy.Subscribe("TenantRestart");
        }

        /// <summary>
        /// Subscribes to Incompatible Client
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        public void SubscribeIncompatibleClient(NotificationSubscriptionServiceProxy proxy, Action action)
        {
            lock(_syncRoot)
            {
                _tenantIncompatibleAction = action;
            }
            proxy.Subscribe("IncompatibleClient");
        }

        /// <summary>
        /// Unsubscribe from all notifications
        /// </summary>
        /// <param name="proxy"></param>
        public void Unsubscribe(NotificationSubscriptionServiceProxy proxy)
        {
            proxy.Unsubscribe(null);

            lock (_syncRoot)
            {
                _configurationAddedAction = delegate(String tenantId) { };
                _configurationUpdatedAction = delegate(String tenantId) { };
                _configurationDeletedAction = delegate(String tenantId) { };
                _restoreRequestMessageAvailableConnectionsAction = delegate { };
                _requestMessageAvailableAction = delegate(String tenantId, Guid messageId) { };
                _messageEnqueuedAction = delegate(QueueTypes queueType, String tenantId, Guid messageId) { };
                _binderElementEnqueuedAction = delegate(String tenantId, String elementId) { };
                _binderElementDeletedAction = delegate(String tenantId, String elementId) { };
                _binderElementRestoredAction = delegate(String tenantId, String elementId) { };
                _binderElementCompletedAction = delegate(String tenandId, String elementId) { };
                _configParamsUpdatedAction = delegate(String tenantId, CloudContracts.WebAPI.Configuration configParams) { };
                _configParamsObtainedAction = delegate(String tenantId, CloudContracts.WebAPI.Configuration configParams) { };                _serviceInfoUpdatedAction = delegate(String tenantId, Uri siteAddressBaseUri) { };
                _tenantRestartAction = delegate(String tenantId) { };
                _tenantIncompatibleAction = delegate { };
            }
        }

        #region INotificationCallback
        /// <summary>
        /// Occurs when a configuration is added
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the added configuration</param>
        public void ConfigurationAdded(String tenantId)
        {
            lock (_syncRoot)
            {
                _configurationAddedAction(tenantId);
            }
        }

        /// <summary>
        /// Occurs when an existing configuration is updated
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the updated configuration</param>
        public void ConfigurationUpdated(String tenantId)
        {
            lock (_syncRoot)
            {
                _configurationUpdatedAction(tenantId);
            }
        }

        /// <summary>
        /// Occurs when an existing configuration is deleted
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the deleted configuration</param>
        public void ConfigurationDeleted(String tenantId)
        {
            lock (_syncRoot)
            {
                _configurationDeletedAction(tenantId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RestoreRequestMessageAvailableConnections()
        {
            lock (_syncRoot)
            {
                _restoreRequestMessageAvailableConnectionsAction();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void RequestMessageAvailable(String tenantId, Guid messageId)
        {
            lock (_syncRoot)
            {
                _requestMessageAvailableAction(tenantId, messageId);
            }
        }

        /// <summary>
        /// Occurs when a message is enqueued
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void MessageEnqueued(QueueTypes queueType, String tenantId, Guid messageId)
        {
            lock (_syncRoot)
            {
                _messageEnqueuedAction(queueType, tenantId, messageId);
            }
        }

        /// <summary>
        /// Occurs when a binder element is enqueued
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void BinderElementEnqueued(String tenantId, String elementId)
        {
            lock (_syncRoot)
            {
                _binderElementEnqueuedAction(tenantId, elementId);
            }
        }

        /// <summary>
        /// Occurs when a binder element is deleted
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void BinderElementDeleted(String tenantId, String elementId)
        {
            lock (_syncRoot)
            {
                _binderElementDeletedAction(tenantId, elementId);
            }
        }

        /// <summary>
        /// Occurs when a binder element is restored
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void BinderElementRestored(String tenantId, String elementId)
        {
            lock (_syncRoot)
            {
                _binderElementRestoredAction(tenantId, elementId);
            }
        }

        /// <summary>
        /// Occurs when a binder element is completed
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void BinderElementCompleted(String tenantId, String elementId)
        {
            lock (_syncRoot)
            {
                _binderElementCompletedAction(tenantId, elementId);
            }
        }

        /// <summary>
        /// Occurs when the config params are updated
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void ConfigParamsUpdated(String tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            lock (_syncRoot)
            {
                _configParamsUpdatedAction(tenantId, configParams);
            }
        }

        /// <summary>
        /// Occurs when the config params are obtained.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void ConfigParamsObtained(String tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            lock (_syncRoot)
            {
                _configParamsObtainedAction(tenantId, configParams);
            }
        }

        /// <summary>
        /// Occurs when the service info has been updated
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="siteAddressBaseUri"></param>
        public void ServiceInfoUpdated(String tenantId, Uri siteAddressBaseUri)
        {
            lock (_syncRoot)
            {
                _serviceInfoUpdatedAction(tenantId, siteAddressBaseUri);
            }
        }

        /// <summary>
        /// Occurs when a tenant restart is requested
        /// </summary>
        /// <param name="tenantId"></param>
        public void TenantRestart(String tenantId)
        {
            lock (_syncRoot)
            {
                _tenantRestartAction(tenantId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void IncompatibleClient()
        {
            lock (_syncRoot)
            {
                _tenantIncompatibleAction();
            }
        }

        #endregion

        #region Private fields
        private readonly Object _syncRoot = new Object();
        private Action<String> _configurationAddedAction = delegate(String tenantId) { };
        private Action<String> _configurationUpdatedAction = delegate(String tenantId) { };
        private Action<String> _configurationDeletedAction = delegate(String tenantId) { };
        private Action _restoreRequestMessageAvailableConnectionsAction = delegate { };
        private Action<String, Guid> _requestMessageAvailableAction = delegate(String tenantId, Guid messageId) { };
        private Action<QueueTypes, String, Guid> _messageEnqueuedAction = delegate(QueueTypes queueType, String tenantId, Guid messageId) { };
        private Action<String, String> _binderElementEnqueuedAction = delegate(String tenantId, String elementId) { };
        private Action<String, String> _binderElementDeletedAction = delegate(String tenantId, String elementId) { };
        private Action<String, String> _binderElementRestoredAction = delegate(String tenantId, String elementId) { };
        private Action<String, String> _binderElementCompletedAction = delegate(String tenantId, String elementId) { };
        private Action<String, CloudContracts.WebAPI.Configuration> _configParamsUpdatedAction = delegate(String tenantId, CloudContracts.WebAPI.Configuration configParams) { };
        private Action<String, CloudContracts.WebAPI.Configuration> _configParamsObtainedAction = delegate(String tenantId, CloudContracts.WebAPI.Configuration configParams) { };
        private Action<String, Uri> _serviceInfoUpdatedAction = delegate(String tenantId, Uri siteAddressBaseUri) { };
        private Action<String> _tenantRestartAction = delegate(String tenantId) { };
        private Action _tenantIncompatibleAction = delegate { };

        #endregion

    }
}
