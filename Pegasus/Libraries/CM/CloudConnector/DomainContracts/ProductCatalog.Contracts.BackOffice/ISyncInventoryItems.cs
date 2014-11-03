using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.ProductCatalog.Contracts.Data;

namespace Sage.Connector.ProductCatalog.Contracts.BackOffice
{
    /// <summary>
    /// Sync the BackOffice Inventory Items
    /// </summary>
    public interface ISyncInventoryItems : IBackOfficeSessionHandler
    {

        /// <summary>
        /// Initialize the Inventory Items to get ready to start processing the sync.
        /// This could be something like loading the inventory item business object to get 
        /// ready to process.  
        /// </summary>
        /// <returns><see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncInventoryItems();

        /// <summary>
        /// Get the next inventory item to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="InventoryItem"/></returns>
        InventoryItem GetNextSyncInventoryItem();
    }
}
