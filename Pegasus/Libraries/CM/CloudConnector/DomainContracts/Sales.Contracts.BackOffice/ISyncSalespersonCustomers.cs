using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Sales.Contracts.Data;

namespace Sage.Connector.Sales.Contracts.BackOffice
{
    /// <summary>
    /// Sync the BackOffice Salespersons. 
    /// </summary>
    public interface ISyncSalespersonCustomers : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Initialize the SyncSalespersonCustomers to get ready to start processing the sync.
        /// This could be something like loading the salesperson business object to get 
        /// ready to process.  
        /// </summary>
        /// <returns><see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncSalespersonCustomers();

        /// <summary>
        /// Get the next salesperson to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="SalespersonCustomer"/></returns>
        SalespersonCustomer GetNextSyncSalespersonCustomer();
    }
}
