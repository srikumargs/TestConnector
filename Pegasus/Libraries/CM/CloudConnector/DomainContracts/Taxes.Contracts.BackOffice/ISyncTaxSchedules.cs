using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Taxes.Contracts.Data;

namespace Sage.Connector.Taxes.Contracts.BackOffice
{
    /// <summary>
    /// Sync the BackOffice Tax Schedules 
    /// </summary>
    public interface ISyncTaxSchedules : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Initialize the Tax Schedules to get ready to start processing the sync.
        /// This could be something like loading the tax schedules business object to get 
        /// ready to process.  
        /// </summary>
        /// <returns><see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncTaxSchedules();

        /// <summary>
        /// Get the next tax schedule to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="TaxSchedule"/></returns>
        TaxSchedule GetNextSyncTaxSchedule();
    }
}
