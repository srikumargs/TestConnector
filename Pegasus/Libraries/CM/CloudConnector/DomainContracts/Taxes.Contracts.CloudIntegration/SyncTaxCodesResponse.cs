using System.Collections.Generic;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;
using Sage.Connector.Taxes.Contracts.Data;

namespace Sage.Connector.Taxes.Contracts.CloudIntegration
{
    /// <summary>
    /// Response for Synchronize Tax Codes
    /// </summary>
    public class SyncTaxCodesResponse: AbstractSyncResponse
    {
        /// <summary>
        /// List of added or changed(updated/status deleted) tax codes
        /// </summary>
        public ICollection<TaxCode> TaxCodes { get; set; }
     
      
    }
}
