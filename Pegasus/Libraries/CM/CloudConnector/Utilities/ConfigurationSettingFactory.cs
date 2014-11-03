using System.ServiceModel;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Interfaces;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.Data;
using Sage.Connector.Logging;

namespace Sage.Connector.Utilities
{
 
    /// <summary>
    /// Helper for accessing and updating configuration settings
    /// </summary>
    public static class ConfigurationSettingFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static string TENANT_CONFIG_NOT_FOUND = "Unable to find the tenant configuration in the connector database.";
    
        /// <summary>
        /// Retrieves the specified configuration for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static PremiseConfigurationRecord RetrieveConfiguration(string tenantId)
        {
            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    return proxy.GetConfiguration(tenantId);
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Configuration", "Unable to retrieve configuration: " + dafe.Reason);
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Retrieves all existing premise configurations
        /// </summary>
        /// <returns></returns>
        public static PremiseConfigurationRecord[] RetrieveAllConfigurations()
        {
            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    return proxy.GetConfigurations();
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Configuration", "Unable to retrieve configurations: {0}; exception: {1}", dafe.Reason, dafe.ExceptionAsString());
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Creates a new tenant in-memory only.  Use SaveNewTenant to persist configuration.
        /// </summary>
        /// <returns></returns>
        public static PremiseConfigurationRecord CreateNewTenant()
        {
            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    return proxy.CreateNewConfiguration();
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Configuration", "Unable to create a new tenant: {0}; exception: {1}", dafe.Reason, dafe.ExceptionAsString());
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Adds a configuration to the database
        /// </summary>
        /// <param name="newPCR"></param>
        /// <returns></returns>
        public static bool SaveNewTenant(PremiseConfigurationRecord newPCR)
        {
            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    proxy.AddConfiguration(newPCR);
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Configuration", "Unable to save a new tenant: {0}; exception: {1}", dafe.Reason, dafe.ExceptionAsString());
                    }
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Deletes a configuration from the database
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static bool DeleteTenant(string tenantId)
        {
            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    proxy.DeleteConfiguration(tenantId);
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Configuration", "Unable to delete a tenant: {0}; exception: {1}", dafe.Reason, dafe.ExceptionAsString());
                    }
                    return false;
                }

            }
            return true;
        }

        /// <summary>
        /// Updates the specified configuration
        /// </summary>
        /// <param name="savedPCR"></param>
        /// <returns></returns>
        public static bool UpdateConfiguration(PremiseConfigurationRecord savedPCR)
        {
            using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                try
                {
                    proxy.UpdateConfiguration(savedPCR);
                }
                catch (FaultException<DataAccessFaultException> dafe)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(null, "Configuration", "Unable to update a configuration: {0}; exception: {1}", dafe.Reason, dafe.ExceptionAsString());
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
