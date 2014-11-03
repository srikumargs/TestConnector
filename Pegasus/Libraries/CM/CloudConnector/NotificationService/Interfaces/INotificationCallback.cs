using System;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.LinkedSource;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.NotificationService.Interfaces
{
    /// <summary>
    /// interface for defining the events that are fired by the NotificationService.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface INotificationCallback
    {
        /// <summary>
        /// Occurs when a configuration is added
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the added configuration</param>
        [OperationContract(IsOneWay = true)]
        void ConfigurationAdded(String tenantId);

        /// <summary>
        /// Occurs when an existing configuration is updated
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the updated configuration</param>
        [OperationContract(IsOneWay = true)]
        void ConfigurationUpdated(String tenantId);

        /// <summary>
        /// Occurs when an existing configuration is deleted
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the deleted configuration</param>
        [OperationContract(IsOneWay = true)]
        void ConfigurationDeleted(String tenantId);

        /// <summary>
        /// Occurs when a request message connection(s) should be re-established
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void RestoreRequestMessageAvailableConnections();

        /// <summary>
        /// Occurs when a request message is available for retrieval
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        [OperationContract(IsOneWay = true)]
        void RequestMessageAvailable(String tenantId, Guid messageId);

        /// <summary>
        /// Occurs when a message is enqueued
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        [OperationContract(IsOneWay = true)]
        void MessageEnqueued(QueueTypes queueType, String tenantId, Guid messageId);

        /// <summary>
        /// Occurs when a binder element is enqueued
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract(IsOneWay = true)]
        void BinderElementEnqueued(String tenantId, String elementId);

        /// <summary>
        /// Occurs when a binder element is deleted
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract(IsOneWay = true)]
        void BinderElementDeleted(String tenantId, String elementId);

        /// <summary>
        /// Occurs when a binder element is restored
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract(IsOneWay = true)]
        void BinderElementRestored(String tenantId, String elementId);

        /// <summary>
        /// Occurs when a binder task is completed
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        [OperationContract(IsOneWay = true)]
        void BinderElementCompleted(String tenantId, String elementId);

        /// <summary>
        /// Occurs when config params have been updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        [OperationContract(IsOneWay = true)]
        void ConfigParamsUpdated(String tenantId, CloudContracts.WebAPI.Configuration configParams);

        /// <summary>
        /// Occurs when config params have been updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        [OperationContract(IsOneWay = true)]
        void ConfigParamsObtained(String tenantId, CloudContracts.WebAPI.Configuration configParams);

        /// <summary>
        /// Occurs when service info has been updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="siteAddressBaseUri"></param>
        [OperationContract(IsOneWay = true)]
        void ServiceInfoUpdated(String tenantId, Uri siteAddressBaseUri);

        /// <summary>
        /// Occurs when a tenant restart is requested
        /// </summary>
        /// <param name="tenantId"></param>
        [OperationContract(IsOneWay = true)]
        void TenantRestart(String tenantId);

        /// <summary>
        /// Occurs when a Client Incompatibility is detected
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void IncompatibleClient();

    }
}
