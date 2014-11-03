using System.Collections.Generic;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;
using Sage.Connector.Taxes.Contracts.Data;

namespace Sage.Connector.Taxes.Contracts.CloudIntegration
{
    /// <summary>
    /// Response for Synchronize Tax Schedules
    /// </summary>
    public class SyncTaxSchedulesResponse: AbstractSyncResponse
    {
        /// <summary>
        /// List of added or changed(updated/status deleted) tax schedules
        /// </summary>
        public ICollection<TaxSchedule> TaxSchedules { get; set; }
     
      
    }
}
