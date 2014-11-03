using Sage.Connector.Customers.Contracts.Data;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Customers.Contracts.BackOffice
{
    /// <summary>
    /// Sync the BackOffice Customers. 
    /// </summary>
    public interface ISyncCustomers : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Initialize the SyncCustomers to get ready to start processing the sync.
        /// This could be something like loading the customer business object to get 
        /// ready to process.  
        /// </summary>
        /// <returns>The <see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncCustomers(); 

        /// <summary>
        /// Get the next customer to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="Customer"/></returns>
        Customer GetNextSyncCustomer();
    }
}
