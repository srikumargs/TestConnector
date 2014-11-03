using System.Collections.Generic;
using Sage.Connector.Invoices.Contracts.Data;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace Sage.Connector.Invoices.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Response for Synchronize Invoice Balances
    /// </summary>
    public class SyncInvoiceBalancesResponse : AbstractSyncResponse
    {

        /// <summary>
        /// List of added or changed(updated/status deleted) invoice balances
        /// </summary>
        public ICollection<InvoiceBalance> InvoiceBalances { get; set; }


    }
}
