using System;
using System.ServiceModel;
using Sage.Connector.NotificationService.Interfaces;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.NotificationService.Proxy.Internal
{
    internal sealed class RawNotificationServiceProxy : ClientBase<INotificationService>, INotificationService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawNotificationServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawNotificationServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawNotificationServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawNotificationServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawNotificationServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region INotificationService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void NotifyConfigurationAdded(String tenantId)
        { base.Channel.NotifyConfigurationAdded(tenantId); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void NotifyConfigurationUpdated(String tenantId)
        { base.Channel.NotifyConfigurationUpdated(tenantId); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void NotifyConfigurationDeleted(String tenantId)
        { base.Channel.NotifyConfigurationDeleted(tenantId); }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyRestoreRequestMessageAvailableConnections()
        { base.Channel.NotifyRestoreRequestMessageAvailableConnections(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void NotifyRequestMessageAvailable(String tenantId, Guid messageId)
        { base.Channel.NotifyRequestMessageAvailable(tenantId, messageId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void NotifyMessageEnqueued(QueueTypes queueType, String tenantId, Guid messageId)
        { base.Channel.NotifyMessageEnqueued(queueType, tenantId, messageId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementEnqueued(String tenantId, String elementId)
        { base.Channel.NotifyBinderElementEnqueued(tenantId, elementId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementDeleted(String tenantId, String elementId)
        { base.Channel.NotifyBinderElementDeleted(tenantId, elementId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementRestored(String tenantId, String elementId)
        { base.Channel.NotifyBinderElementRestored(tenantId, elementId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementCompleted(String tenantId, String elementId)
        { base.Channel.NotifyBinderElementCompleted(tenantId, elementId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void NotifyConfigParamsUpdated(
            String tenantId, CloudContracts.WebAPI.Configuration configParams)
        { base.Channel.NotifyConfigParamsUpdated(tenantId, configParams); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void NotifyConfigParamsObtained(
            String tenantId, CloudContracts.WebAPI.Configuration configParams)
        { base.Channel.NotifyConfigParamsObtained(tenantId, configParams); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="siteAddressBaseUri"></param>
        public void NotifyServiceInfoUpdated(
            String tenantId, Uri siteAddressBaseUri)
        { base.Channel.NotifyServiceInfoUpdated(tenantId, siteAddressBaseUri); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void NotifyTenantRestart(
            String tenantId)
        { base.Channel.NotifyTenantRestart(tenantId); }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyIncompatibleClient()
        { base.Channel.NotifyIncompatibleClient(); }

        #endregion
    }
}
