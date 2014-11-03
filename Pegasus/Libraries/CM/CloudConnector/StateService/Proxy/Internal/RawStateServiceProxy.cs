using System;
using System.ServiceModel;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace Sage.Connector.StateService.Proxy.Internal
{
    internal sealed class RawStateServiceProxy : ClientBase<IStateService>, IStateService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawStateServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawStateServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawStateServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawStateServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawStateServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region IStateService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectorState GetConnectorState()
        { return base.Channel.GetConnectorState(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="backOfficeProductName"></param>
        public BackOfficePluginInformation GetBackOfficePluginInformation(String pluginId, String backOfficeProductName)
        { return base.Channel.GetBackOfficePluginInformation(pluginId, backOfficeProductName); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        public RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold)
        { return base.Channel.GetRecentAndInProgressRequestsState(recentEntriesThreshold); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void RaiseSubsystemHealthIssue(SubsystemHealthMessage message)
        { base.Channel.RaiseSubsystemHealthIssue(message); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        public void ClearSubsystemHealthIssues(Subsystem subsystem)
        { base.Channel.ClearSubsystemHealthIssues(subsystem); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxUptime"></param>
        public void SetMaxUptimeBeforeRestart(TimeSpan? maxUptime)
        { base.Channel.SetMaxUptimeBeforeRestart(maxUptime); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void SetRestartMode(RestartMode mode)
        { base.Channel.SetRestartMode(mode); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blackoutEnd"></param>
        public void SetTimeToBlackoutEnd(TimeSpan? blackoutEnd)
        { base.Channel.SetTimeToBlackoutEnd(blackoutEnd); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="info"></param>
        public void SetUpdateInfo(ConnectorUpdateStatus status, UpdateInfo info)
        { base.Channel.SetUpdateInfo(status, info); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="backOfficeCompanyName"></param>
        /// <param name="tenantUri"></param>
        /// <param name="integrationEnabledStatus"></param>
        /// <param name="backOfficePluginInformation"></param>
        public void UpdateIntegratedConnectionState(String tenantId, String tenantName, String backOfficeCompanyName, Uri tenantUri, IntegrationEnabledStatus integrationEnabledStatus, BackOfficePluginInformation backOfficePluginInformation)
        { base.Channel.UpdateIntegratedConnectionState(tenantId, tenantName, backOfficeCompanyName, tenantUri, integrationEnabledStatus, backOfficePluginInformation); }


        /// <summary>
        /// Removes the tenant state for the given identifier
        /// </summary>
        /// <param name="tenantId"></param>
        public void RemoveIntegratedConnectionState(string tenantId)
        {
            base.Channel.RemoveIntegratedConnectionState(tenantId);
        }

        /// <summary>
        /// Updates the last attempted communication time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateLastAttemptedCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            base.Channel.UpdateLastAttemptedCommunicationWithCloud(tenantId, commTime);
        }

        /// <summary>
        /// Updates the last successful communication time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateLastSuccessfulCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            base.Channel.UpdateLastSuccessfulCommunicationWithCloud(tenantId, commTime);
        }

        /// <summary>
        /// Updates the next scheduled communication time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateNextScheduledCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            base.Channel.UpdateNextScheduledCommunicationWithCloud(tenantId, commTime);
        }

        /// <summary>
        /// Increments the requests received count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementRequestsReceivedCount(string tenantId, uint count)
        {
            base.Channel.IncrementRequestsReceivedCount(tenantId, count);
        }

        /// <summary>
        /// Increments the non-error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementNonErrorResponsesSentCount(string tenantId, uint count)
        {
            base.Channel.IncrementNonErrorResponsesSentCount(tenantId, count);
        }

        /// <summary>
        /// Increments the error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementErrorResponsesSentCount(string tenantId, uint count)
        {
            base.Channel.IncrementErrorResponsesSentCount(tenantId, count);
        }

        /// <summary>
        /// Adjusts the number of requests in process
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void AdjustRequestsInProgressCount(string tenantId, int count)
        {
            base.Channel.AdjustRequestsInProgressCount(tenantId, count);
        }

        /// <summary>
        /// Updates the TenantConnectivityStatus
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        public void UpdateTenantConnectivityStatus(String tenantId, TenantConnectivityStatus status)
        { base.Channel.UpdateTenantConnectivityStatus(tenantId, status); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        public void UpdateBackOfficeConnectivityStatus(String tenantId, BackOfficeConnectivityStatus status)
        { base.Channel.UpdateBackOfficeConnectivityStatus(tenantId, status); }

        #endregion
    }
}
