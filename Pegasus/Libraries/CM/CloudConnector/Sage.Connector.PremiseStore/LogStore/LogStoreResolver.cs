using System;
using System.Data.Common;
using Sage.Connector.Common;

namespace Sage.Connector.PremiseStore.LogStore
{
    /// <summary>
    /// Returns the well-known entity model / premise store
    /// </summary>
    internal static class LogStoreResolver
    {
        private static string _connectionString = "metadata=res://*/LogStore.LogStoreModel.csdl|res://*/LogStore.LogStoreModel.ssdl|res://*/LogStore.LogStoreModel.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=\"Data Source=|DataDirectory|\\LogStore.sdf;Max Database Size=3072\"";

        /// <summary>
        /// The path where premise stores are stored
        /// </summary>
        private static string StorePath
        { get { return ConnectorServiceUtils.InstanceApplicationDataFolder; } }

        /// <summary>
        /// Model Container (requires proper disposal)
        /// </summary>
        public static LogStoreModelContainer ModelContainer
        {
            get
            {
                {
                    AppDomain.CurrentDomain.SetData("DataDirectory", StorePath);
                    return new LogStoreModelContainer(_connectionString);
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
