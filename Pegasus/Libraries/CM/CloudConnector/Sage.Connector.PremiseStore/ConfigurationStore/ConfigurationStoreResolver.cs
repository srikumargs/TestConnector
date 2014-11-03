using System;
using System.Data.Common;
using Sage.Connector.Common;

namespace Sage.Connector.PremiseStore.ConfigurationStore
{
    /// <summary>
    /// Returns the well-known entity model / premise store
    /// </summary>
    internal static class ConfigurationStoreResolver
    {
        private static string _connectionString = "metadata=res://*/ConfigurationStore.ConfigurationStoreModel.csdl|res://*/ConfigurationStore.ConfigurationStoreModel.ssdl|res://*/ConfigurationStore.ConfigurationStoreModel.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=\"Data Source=|DataDirectory|\\ConfigurationStore.sdf;Flush Interval=1\"";

        /// <summary>
        /// The path where premise stores are stored
        /// </summary>
        private static string StorePath
        { get { return ConnectorServiceUtils.InstanceApplicationDataFolder; } }

        /// <summary>
        /// Model Container (requires proper disposal)
        /// </summary>
        public static ConfigurationStoreModelContainer ModelContainer
        {
            get
            {
                {
                    AppDomain.CurrentDomain.SetData("DataDirectory", StorePath);
                    return new ConfigurationStoreModelContainer(_connectionString);
                }
            }
        }

        /// <summary>
        /// Normally, connection management by entity framework
        /// but expose for unit testing DB rollbacks
        /// </summary>
        /// <returns></returns>
        public static DbConnection Connection()
        {
            return ModelContainer.Connection;
        }
    }
}
