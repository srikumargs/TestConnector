using System.Collections.Generic;
using Sage.Connector.Sales.Contracts.Data;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace Sage.Connector.Sales.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Response for Synchronize Salesperons
    /// </summary>
    public class SyncSalespersonsResponse : AbstractSyncResponse
    {

        /// <summary>
        /// List of added or changed(updated/status deleted) salespersons
        /// </summary>
        public ICollection<Salesperson> Salespersons { get; set; }


    }
}
