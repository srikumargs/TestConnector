using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Invoices.Contracts.Data;

namespace Sage.Connector.Invoices.Contracts.BackOffice
{
    /// <summary>
    /// Sync the Invoice Balances
    /// </summary>
    public interface ISyncInvoiceBalances: IBackOfficeSessionHandler
    {

        /// <summary>
        /// Initialize the Sync Invoice Balances to get ready to start processing the selection.
        /// This could be something like loading the invoice business object to get 
        /// ready to process.  
        /// </summary>
        /// <returns><see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncInvoiceBalances(); 

        /// <summary>
        /// Get the next invoice balance to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="InvoiceBalance"/></returns>
        InvoiceBalance GetNextSyncInvoiceBalance();
    }
}
