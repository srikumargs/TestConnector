using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Taxes.Contracts.Data;

namespace Sage.Connector.Taxes.Contracts.BackOffice
{
    /// <summary>
    /// Sync the BackOffice Tax Codes 
    /// </summary>
    public interface ISyncTaxCodes : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Initialize the Tax Codes to get ready to start processing the sync.
        /// This could be something like loading the tax code business object to get 
        /// ready to process.  
        /// </summary>
        /// <returns><see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncTaxCodes(); 


        /// <summary>
        /// Get the next tax code to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="TaxCode"/></returns>
        TaxCode GetNextSyncTaxCode();
    }
}
