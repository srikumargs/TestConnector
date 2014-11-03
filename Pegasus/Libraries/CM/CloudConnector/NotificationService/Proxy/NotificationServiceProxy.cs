using System;
using Sage.Connector.NotificationService.Interfaces;
using Sage.ServiceModel;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.NotificationService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NotificationServiceProxy : RetryClientBase<INotificationService>, INotificationService
    {
        /// <summary>
        /// 
        /// </summary>
        public NotificationServiceProxy(RetryClientBase<INotificationService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region INotificationService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void NotifyConfigurationAdded(String tenantId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyConfigurationAdded(tenantId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void NotifyConfigurationUpdated(String tenantId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyConfigurationUpdated(tenantId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void NotifyConfigurationDeleted(String tenantId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyConfigurationDeleted(tenantId); }); }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyRestoreRequestMessageAvailableConnections()
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyRestoreRequestMessageAvailableConnections(); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void NotifyRequestMessageAvailable(String tenantId, Guid messageId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyRequestMessageAvailable(tenantId, messageId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void NotifyMessageEnqueued(QueueTypes queueType, String tenantId, Guid messageId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyMessageEnqueued(queueType, tenantId, messageId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementEnqueued(String tenantId, String elementId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyBinderElementEnqueued(tenantId, elementId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementDeleted(String tenantId, String elementId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyBinderElementDeleted(tenantId, elementId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementRestored(String tenantId, String elementId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyBinderElementRestored(tenantId, elementId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementCompleted(String tenantId, String elementId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyBinderElementCompleted(tenantId, elementId); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void NotifyConfigParamsUpdated(
            String tenantId, CloudContracts.WebAPI.Configuration configParams)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyConfigParamsUpdated(tenantId, configParams); }); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void NotifyConfigParamsObtained(
            String tenantId, CloudContracts.WebAPI.Configuration configParams)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyConfigParamsObtained(tenantId, configParams); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="siteAddressBaseUri"></param>
        public void NotifyServiceInfoUpdated(
            String tenantId, Uri siteAddressBaseUri)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyServiceInfoUpdated(tenantId, siteAddressBaseUri); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void NotifyTenantRestart(
            String tenantId)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyTenantRestart(tenantId); }); }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyIncompatibleClient()
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.NotifyIncompatibleClient(); }); }

        #endregion
    }
}
