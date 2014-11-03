using System.Collections.Generic;
using Sage.Connector.Invoices.Contracts.Data;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace Sage.Connector.Invoices.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Response for Synchronize Invoices
    /// </summary>
    public class SyncInvoicesResponse : AbstractSyncResponse
    {

        /// <summary>
        /// List of added or changed(updated/status deleted) invoices
        /// </summary>
        public ICollection<Invoice> Invoices { get; set; }


    }
}
