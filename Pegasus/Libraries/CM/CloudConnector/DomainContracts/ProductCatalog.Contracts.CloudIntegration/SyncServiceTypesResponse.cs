using System.Collections.Generic;
using Sage.Connector.ProductCatalog.Contracts.Data;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace ProductCatalog.Contracts.CloudIntegration
{
    /// <summary>
    /// Response for Synchronize ServiceTypes
    /// </summary>
    public class SyncServiceTypesResponse : AbstractSyncResponse
    {
        /// <summary>
        /// List of added or changed(updated/status deleted) service types
        /// </summary>
        public ICollection<ServiceType> ServiceTypes { get; set; }
     
      
    }
}
