using System.Collections.Generic;
using Sage.Connector.ProductCatalog.Contracts.Data;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace ProductCatalog.Contracts.CloudIntegration
{
    /// <summary>
    /// Response for Synchronize InventoryItems
    /// </summary>
    public class SyncInventoryItemsResponse: AbstractSyncResponse
    {
        /// <summary>
        /// List of added or changed(updated/status deleted) inventory items
        /// </summary>
        public ICollection<InventoryItem> InventoryItems { get; set; }
     
      
    }
}
