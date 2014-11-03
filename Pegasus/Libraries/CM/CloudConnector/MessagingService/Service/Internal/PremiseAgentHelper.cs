using System;
using System.ServiceModel;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Interfaces;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// Helper to get premise agent data
    /// </summary>
    public static class PremiseAgentHelper
    {
        /// <summary>
        /// Get the premise agent object for the provided tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static CloudContracts.PremiseAgent GetPremiseAgent(String tenantId)
        {
            CloudContracts.PremiseAgent result = null;

            try
            {
                var connectorState = GetConnectorState(tenantId);
                var integratedConnectionState = GetIntegratedConnectionState(tenantId, connectorState);
                var backOfficePluginInformation = GetBackOfficePluginInformation(tenantId, connectorState);

                string sConnectorFileVersion = string.Empty;
                string sConnectorProductCode = string.Empty;
                string sConnectorProductName = string.Empty;
                string sConnectorProductVersion = string.Empty;
                string sBackOfficeProductId = string.Empty;
                string sBackOfficeProductName = string.Empty;
                string sBackOfficeProductVersion = string.Empty;
                string sBackOfficeProductPluginFileVersion = string.Empty;
                string sInterfaceVersion = string.Empty;
                DateTime dtSystemDate = DateTime.UtcNow;

                string backOfficeCompanyName = string.Empty;
                string autoUpdateProductId = string.Empty;
                string autoUpdateProductVersion = string.Empty;
                string autoUpdateComponentBaseName = String.Empty;
                bool   runAsUserRequired = false;
                string platform = String.Empty;

                if (connectorState != null)
                {
                    if (null != connectorState.FileVersion)
                        sConnectorFileVersion = connectorState.FileVersion;
                    if (null != connectorState.ProductCode)
                        sConnectorProductCode = connectorState.ProductCode;
                    if (null != connectorState.ProductName)
                        sConnectorProductName = connectorState.ProductName;
                    if (null != connectorState.ProductVersion)
                        sConnectorProductVersion = connectorState.ProductVersion;
                    if (null != backOfficePluginInformation)
                    {
                        if (null != backOfficePluginInformation.PluginId)
                            sBackOfficeProductId = backOfficePluginInformation.PluginId;
                        if (null != backOfficePluginInformation.ProductName)
                            sBackOfficeProductName = backOfficePluginInformation.ProductName;
                        if (null != backOfficePluginInformation.ProductVersion)
                            sBackOfficeProductVersion = backOfficePluginInformation.ProductVersion;
                        if (null != backOfficePluginInformation.ProductPluginFileVersion)
                            sBackOfficeProductPluginFileVersion = backOfficePluginInformation.ProductPluginFileVersion;

                        if (null != backOfficePluginInformation.AutoUpdateProductId)
                            autoUpdateProductId = backOfficePluginInformation.AutoUpdateProductId;
                        if (null != backOfficePluginInformation.AutoUpdateProductVersion)
                            autoUpdateProductVersion = backOfficePluginInformation.AutoUpdateProductVersion;
                        if (null != backOfficePluginInformation.AutoUpdateComponentBaseName)
                            autoUpdateComponentBaseName = backOfficePluginInformation.AutoUpdateComponentBaseName;
                        if (null != backOfficePluginInformation.Platform)
                            platform = backOfficePluginInformation.Platform;
                        runAsUserRequired = backOfficePluginInformation.RunAsUserRequried;

                    }
                    if (null != connectorState.CloudInterfaceVersion)
                        sInterfaceVersion = connectorState.CloudInterfaceVersion;

                    if (null != integratedConnectionState)
                    {
                        if (null != integratedConnectionState.BackOfficeCompanyName)
                            backOfficeCompanyName = integratedConnectionState.BackOfficeCompanyName;

                    }
                }

                result = new CloudContracts.PremiseAgent(
                    sConnectorFileVersion,
                    sConnectorProductCode,
                    sConnectorProductName,
                    sConnectorProductVersion,
                    sBackOfficeProductId,
                    sBackOfficeProductName,
                    sBackOfficeProductVersion,
                    sBackOfficeProductPluginFileVersion,
                    sInterfaceVersion,
                    dtSystemDate,
                    backOfficeCompanyName,
                    autoUpdateProductId,
                    autoUpdateProductVersion,
                    autoUpdateComponentBaseName,
                    runAsUserRequired,
                    platform
                    );
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteCriticalWithEventLogging(null, "Messaging Service", "Problem getting PremiseAgent for tenant '{0}'; exception: {1}", tenantId, ex.ExceptionAsString());
                }
            }

            return result;
        }

        private static ConnectorState GetConnectorState(String tenantId)
        {
            ConnectorState result = null;

            using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    result = proxy.GetConnectorState();
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Messaging Service", "Problem updating/retrieving connector state for tenant '{0}': {1}; exception: {2}", tenantId, dafe.Reason, dafe.ExceptionAsString());
                    }
                    result = null;
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Messaging Service", "Problem updating/retrieving connector state for tenant '{0}'; exception: {1}", tenantId, ex.ExceptionAsString());
                    }
                    result = null;
                }
            }

            return result;
        }

        private static BackOfficePluginInformation GetBackOfficePluginInformation(String tenantId, ConnectorState connectorState)
        {
            BackOfficePluginInformation result = null;

            if (null != connectorState)
            {
                foreach (var integratedState in connectorState.IntegratedConnectionStates)
                {
                    if (integratedState.TenantId == tenantId)
                    {
                        return integratedState.BackOfficePluginInformation;
                    }
                }
            }

            return result;
        }

        private static IntegratedConnectionState GetIntegratedConnectionState(String tenantId, ConnectorState connectorState)
        {
            IntegratedConnectionState result = null;

            if (null != connectorState)
            {
                foreach (var integratedState in connectorState.IntegratedConnectionStates)
                {
                    if (integratedState.TenantId == tenantId)
                    {
                        return integratedState;
                    }
                }
            }

            return result;
        }
    }
}
