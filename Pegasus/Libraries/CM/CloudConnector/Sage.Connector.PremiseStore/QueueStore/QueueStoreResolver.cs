using System;
using System.Data.Common;
using Sage.Connector.Common;

namespace Sage.Connector.PremiseStore.QueueStore
{
    /// <summary>
    /// Returns the well-known entity model / premise store
    /// </summary>
    internal static class QueueStoreResolver
    {
        private static string _connectionString = "metadata=res://*/QueueStore.QueueStoreModel.csdl|res://*/QueueStore.QueueStoreModel.ssdl|res://*/QueueStore.QueueStoreModel.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=\"Data Source=|DataDirectory|\\QueueStore.sdf;Flush Interval=1;Max Database Size=2048;Temp File Max Size=2048;Autoshrink Threshold=100\"";

        /// <summary>
        /// The path where premise stores are stored
        /// </summary>
        private static string StorePath
        { get { return ConnectorServiceUtils.InstanceApplicationDataFolder; } }

        /// <summary>
        /// Model Container (requires proper disposal)
        /// </summary>
        public static QueueStoreModelContainer ModelContainer
        {
            get
            {
                {
                    AppDomain.CurrentDomain.SetData("DataDirectory", StorePath);
                    return new QueueStoreModelContainer(_connectionString);
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
