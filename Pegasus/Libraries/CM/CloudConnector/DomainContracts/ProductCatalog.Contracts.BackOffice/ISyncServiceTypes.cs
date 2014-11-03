using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.ProductCatalog.Contracts.Data;

namespace Sage.Connector.ProductCatalog.Contracts.BackOffice
{
    /// <summary>
    /// Sync the BackOffice ServiceTypes. 
    /// </summary>
    public interface ISyncServiceTypes : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Initialize the SyncServiceTypes to get ready to start processing the sync.
        /// This could be something like loading the serviceType business object to get 
        /// ready to process.  
        /// </summary>
        /// <returns><see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncServiceTypes();

        /// <summary>
        /// Get the next serviceType to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="ServiceType"/></returns>
        ServiceType GetNextSyncServiceType();
    }
}
