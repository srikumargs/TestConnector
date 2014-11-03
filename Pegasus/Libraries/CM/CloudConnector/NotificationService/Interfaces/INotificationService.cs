using System;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.LinkedSource;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.NotificationService.Interfaces
{
    /// <summary>
    /// CRUD Management of Premise-Cloud Configurations
    /// </summary>
    /// <remarks>
    /// It is the callers responsibility to make sure any data needed by the recipients of these messages are available before making the call.
    /// </remarks>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface INotificationService
    {
        /// <summary>
        /// Occurs when a configuration is added
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the added configuration</param>
        /// <remarks>
        /// It is the callers responsibility to make sure any data needed by the recipients of this message is available before making this call.
        /// </remarks>
        [OperationContract]
        void NotifyConfigurationAdded(String tenantId);

        /// <summary>
        /// Occurs when a configuration is updated
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the updated configuration</param>
        /// <remarks>
        /// It is the callers responsibility to make sure any data needed by the recipients of this message is available before making this call.
        /// </remarks>
        [OperationContract]
        void NotifyConfigurationUpdated(String tenantId);

        /// <summary>
        /// Occurs when a configuration is deleted
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the deleted configuration</param>
        /// <remarks>
        /// It is the callers responsibility to make sure any data needed by the recipients of this message is available before making this call.
        /// </remarks>
        [OperationContract]
        void NotifyConfigurationDeleted(String tenantId);

        /// <summary>
        /// Occurs when a request message connection(s) should be reestablished
        /// </summary>
        [OperationContract]
        void NotifyRestoreRequestMessageAvailableConnections();

        /// <summary>
        /// Occurs when a request message is available for retrieval
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        [OperationContract]
        void NotifyRequestMessageAvailable(String tenantId, Guid messageId);

        /// <summary>
        /// Occurs when a message is enqueued
        /// </summary>
        /// <remarks>
        /// It is the callers responsibility to make sure any data needed by the recipients of this message is available before making this call.
        /// </remarks>
        [OperationContract]
        void NotifyMessageEnqueued(QueueTypes queueType, String tenantId, Guid messageId);

        /// <summary>
        /// Occurs when a binder element is enqueued
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract]
        void NotifyBinderElementEnqueued(String tenantId, String elementId);

        /// <summary>
        /// Occurs when a binder element is deleted
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract]
        void NotifyBinderElementDeleted(String tenantId, String elementId);

        /// <summary>
        /// Occurs when a binder element is restored
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract]
        void NotifyBinderElementRestored(String tenantId, String elementId);

        /// <summary>
        /// Occurs when a binder task is completed
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract]
        void NotifyBinderElementCompleted(String tenantId, String elementId);

        /// <summary>
        /// Occurs when the config params have been updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="cofigParams"></param>
        [OperationContract]
        void NotifyConfigParamsUpdated(string tenantId, CloudContracts.WebAPI.Configuration cofigParams);

        /// <summary>
        /// Occurs when the config params have been obtained by the messaging service.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="cofigParams"></param>
        [OperationContract]
        void NotifyConfigParamsObtained(string tenantId, CloudContracts.WebAPI.Configuration cofigParams);

        /// <summary>
        /// Occurs when the service info has been updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="siteAddressBaseUri"></param>
        [OperationContract]
        void NotifyServiceInfoUpdated(
            string tenantId, Uri siteAddressBaseUri);

        /// <summary>
        /// Request that tenant workers restart
        /// (generally due to bad current thread context with databases in W2K3)
        /// </summary>
        /// <param name="tenantId"></param>
        [OperationContract]
        void NotifyTenantRestart(string tenantId);

        /// <summary>
        /// Request that all tenants be disabled
        /// </summary>
        [OperationContract]
        void NotifyIncompatibleClient();
    }
}
