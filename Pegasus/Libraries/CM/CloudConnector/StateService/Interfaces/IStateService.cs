using System;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces
{
    /// <summary>
    /// CRUD Management of Premise-Cloud Configurations
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IStateService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ConnectorState GetConnectorState();

        /// <summary>
        /// TODO KMS: Determine if we even need the product name here.  the plugin id should be sufficient
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="backOfficeProductName"></param>
        [OperationContract]
        BackOfficePluginInformation GetBackOfficePluginInformation(String pluginId, String backOfficeProductName);

        /// <summary>
        /// Retrieves the recently created request entries as well as all currently in-progress ones
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        [OperationContract]
        RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void RaiseSubsystemHealthIssue(SubsystemHealthMessage message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        [OperationContract]
        void ClearSubsystemHealthIssues(Subsystem subsystem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxUptime"></param>
        [OperationContract]
        void SetMaxUptimeBeforeRestart(TimeSpan? maxUptime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        [OperationContract]
        void SetRestartMode(RestartMode mode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blackoutEnd"></param>
        [OperationContract]
        void SetTimeToBlackoutEnd(TimeSpan? blackoutEnd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="info"></param>
        [OperationContract]
        void SetUpdateInfo(ConnectorUpdateStatus status, UpdateInfo info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="backOfficeCompanyName"></param>
        /// <param name="tenantUri"></param>
        /// <param name="integrationEnabledStatus"></param>
        /// <param name="backOfficePluginInformation"></param>
        [OperationContract]
        void UpdateIntegratedConnectionState(String tenantId, String tenantName, String backOfficeCompanyName, Uri tenantUri, IntegrationEnabledStatus integrationEnabledStatus, BackOfficePluginInformation backOfficePluginInformation);

        /// <summary>
        /// Removes the tenant state for the given tenant identifier
        /// </summary>
        /// <param name="tenantId"></param>
        [OperationContract]
        void RemoveIntegratedConnectionState(string tenantId);

        /// <summary>
        /// Updates the last attempted communication with the cloud for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        [OperationContract]
        void UpdateLastAttemptedCommunicationWithCloud(string tenantId, DateTime commTime);

        /// <summary>
        /// Update the last successful communication with the cloud for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        [OperationContract]
        void UpdateLastSuccessfulCommunicationWithCloud(string tenantId, DateTime commTime);

        /// <summary>
        /// Updates the next scheduled communication with the cloud for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        [OperationContract]
        void UpdateNextScheduledCommunicationWithCloud(string tenantId, DateTime commTime);

        /// <summary>
        /// Increments the requests received count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        [OperationContract]
        void IncrementRequestsReceivedCount(string tenantId, uint count);

        /// <summary>
        /// Increments the non-error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        [OperationContract]
        void IncrementNonErrorResponsesSentCount(string tenantId, uint count);

        /// <summary>
        /// Increments the error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        [OperationContract]
        void IncrementErrorResponsesSentCount(string tenantId, uint count);

        /// <summary>
        /// Adjusts the number of requests in process
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        [OperationContract]
        void AdjustRequestsInProgressCount(string tenantId, int count);

        /// <summary>
        /// Updates the TenantConnectivityStatus
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        [OperationContract]
        void UpdateTenantConnectivityStatus(String tenantId, TenantConnectivityStatus status);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        [OperationContract]
        void UpdateBackOfficeConnectivityStatus(String tenantId, BackOfficeConnectivityStatus status);


        //TODO:drain requested (update support)
        //TODO:restart support
    }
}
