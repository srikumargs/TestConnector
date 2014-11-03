using Sage.Connector.StateService.Interfaces.DataContracts;

namespace Sage.Connector.StateService.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConnectionActiveStatusHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="integratedConnectionState"></param>
        /// <param name="companyEnabled"></param>
        /// <param name="backOfficeUpdateRequired"></param>
        /// <returns></returns>
        public static ConnectionActiveStatus GetBackOfficeConnectionStatus(
            IntegratedConnectionState integratedConnectionState,
            out bool companyEnabled,
            out bool backOfficeUpdateRequired)
        {
            ConnectionActiveStatus backOfficeConnectionStatus = ConnectionActiveStatus.None;

            // Determine the enabled status and if an update is required
            companyEnabled = (integratedConnectionState != null && ((integratedConnectionState.IntegrationEnabledStatus & IntegrationEnabledStatus.BackOfficeProcessing) == IntegrationEnabledStatus.BackOfficeProcessing));
            backOfficeUpdateRequired = (integratedConnectionState != null && (integratedConnectionState.BackOfficeConnectivityStatus == BackOfficeConnectivityStatus.Incompatible));

            // Check upgrade scenario first
            if (backOfficeUpdateRequired)
            {
                backOfficeConnectionStatus = ConnectionActiveStatus.Broken;
            }

            // Check for null or none state second
            else if (integratedConnectionState == null || integratedConnectionState.BackOfficeConnectivityStatus == BackOfficeConnectivityStatus.None)
            {
                backOfficeConnectionStatus = ConnectionActiveStatus.None;
            }

            // Now all other state logic
            else if (integratedConnectionState.BackOfficeConnectivityStatus == BackOfficeConnectivityStatus.Normal)
            {
                if (companyEnabled)
                    backOfficeConnectionStatus = ConnectionActiveStatus.Active;
                else
                    backOfficeConnectionStatus = ConnectionActiveStatus.Inactive;
            }
            else
            {
                backOfficeConnectionStatus = ConnectionActiveStatus.Broken;
            }

            return backOfficeConnectionStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectorState"></param>
        /// <param name="integratedConnectionState"></param>
        /// <param name="tenantReceiveEnabled"></param>
        /// <param name="tenantSendEnabled"></param>
        /// <param name="connectorUpdateRequired"></param>
        /// <returns></returns>
        public static ConnectionActiveStatus GetCloudConnectivityStatus(
            ConnectorState connectorState,
            IntegratedConnectionState integratedConnectionState,
            out bool tenantReceiveEnabled,
            out bool tenantSendEnabled,
            out bool connectorUpdateRequired)
        {
            ConnectionActiveStatus cloudConnectionStatus = ConnectionActiveStatus.None;

            // Determine the enabled statuses and if an update is required
            tenantReceiveEnabled = (integratedConnectionState != null && ((integratedConnectionState.IntegrationEnabledStatus & IntegrationEnabledStatus.CloudGetRequests) == IntegrationEnabledStatus.CloudGetRequests));
            tenantSendEnabled = (integratedConnectionState != null && ((integratedConnectionState.IntegrationEnabledStatus & IntegrationEnabledStatus.CloudPutResponses) == IntegrationEnabledStatus.CloudPutResponses));
            connectorUpdateRequired = (connectorState.ConnectorUpdateStatus == ConnectorUpdateStatus.UpdateRequired);

            // Check for upgrade required first
            if (connectorUpdateRequired)
            {
                cloudConnectionStatus = ConnectionActiveStatus.Broken;
            }

            // Check for null or none state second
            else if (integratedConnectionState == null || integratedConnectionState.TenantConnectivityStatus == TenantConnectivityStatus.None)
            {
                cloudConnectionStatus = ConnectionActiveStatus.None;
            }

            // Now all other state logic
            else if (integratedConnectionState.TenantConnectivityStatus == TenantConnectivityStatus.Normal)
            {
                if (tenantReceiveEnabled && tenantSendEnabled)
                    cloudConnectionStatus = ConnectionActiveStatus.Active;
                else
                    cloudConnectionStatus = ConnectionActiveStatus.Inactive;
            }
            else if (integratedConnectionState.TenantConnectivityStatus == TenantConnectivityStatus.TenantDisabled)
            {
                cloudConnectionStatus = ConnectionActiveStatus.Inactive;
            }
            else
            {
                cloudConnectionStatus = ConnectionActiveStatus.Broken;
            }

            return cloudConnectionStatus;
        }
    }
}
