using System;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.ServiceModel;

namespace Sage.Connector.StateService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StateServiceProxy : RetryClientBase<IStateService>, IStateService
    {
        /// <summary>
        /// 
        /// </summary>
        public StateServiceProxy(RetryClientBase<IStateService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region IStateService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectorState GetConnectorState()
        {
            return (ConnectorState)RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetConnectorState();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="backOfficeProductName"></param>
        public BackOfficePluginInformation GetBackOfficePluginInformation(String pluginId, String backOfficeProductName)
        {
            return (BackOfficePluginInformation)RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetBackOfficePluginInformation(pluginId, backOfficeProductName);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        public RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold)
        {
            return (RequestState[])RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetRecentAndInProgressRequestsState(recentEntriesThreshold);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void RaiseSubsystemHealthIssue(SubsystemHealthMessage message)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.RaiseSubsystemHealthIssue(message); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        public void ClearSubsystemHealthIssues(Subsystem subsystem)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.ClearSubsystemHealthIssues(subsystem); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxUptime"></param>
        public void SetMaxUptimeBeforeRestart(TimeSpan? maxUptime)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.SetMaxUptimeBeforeRestart(maxUptime); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void SetRestartMode(RestartMode mode)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.SetRestartMode(mode); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blackoutEnd"></param>
        public void SetTimeToBlackoutEnd(TimeSpan? blackoutEnd)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.SetTimeToBlackoutEnd(blackoutEnd); }); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="info"></param>
        public void SetUpdateInfo(ConnectorUpdateStatus status, UpdateInfo info)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.SetUpdateInfo(status, info); }); }

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
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.UpdateIntegratedConnectionState(tenantId, tenantName, backOfficeCompanyName, tenantUri, integrationEnabledStatus, backOfficePluginInformation); }); }

        /// <summary>
        /// Removes the tenant state for the given tenant identifier
        /// </summary>
        /// <param name="tenantId"></param>
        public void RemoveIntegratedConnectionState(string tenantId)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.RemoveIntegratedConnectionState(tenantId); });
        }

        /// <summary>
        /// Updates the last attempted communication time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateLastAttemptedCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.UpdateLastAttemptedCommunicationWithCloud(tenantId, commTime); });
        }

        /// <summary>
        /// Updates the last successful communication time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateLastSuccessfulCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.UpdateLastSuccessfulCommunicationWithCloud(tenantId, commTime); });
        }

        /// <summary>
        /// Updates the next scheduled communication time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateNextScheduledCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.UpdateNextScheduledCommunicationWithCloud(tenantId, commTime); });
        }

        /// <summary>
        /// Increments the requests received count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementRequestsReceivedCount(string tenantId, uint count)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.IncrementRequestsReceivedCount(tenantId, count); });
        }

        /// <summary>
        /// Increments the non-error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementNonErrorResponsesSentCount(string tenantId, uint count)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.IncrementNonErrorResponsesSentCount(tenantId, count); });
        }

        /// <summary>
        /// Increments the error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementErrorResponsesSentCount(string tenantId, uint count)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.IncrementErrorResponsesSentCount(tenantId, count); });
        }

        /// <summary>
        /// Adjusts the number of requests in process
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void AdjustRequestsInProgressCount(string tenantId, int count)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.AdjustRequestsInProgressCount(tenantId, count); });
        }

        /// <summary>
        /// Updates the TenantConnectivityStatus
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        public void UpdateTenantConnectivityStatus(String tenantId, TenantConnectivityStatus status)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.UpdateTenantConnectivityStatus(tenantId, status); }); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        public void UpdateBackOfficeConnectivityStatus(String tenantId, BackOfficeConnectivityStatus status)
        { VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.UpdateBackOfficeConnectivityStatus(tenantId, status); }); }


        #endregion
    }
}
