using System.Collections.Generic;
using Sage.Connector.Sales.Contracts.Data;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace Sage.Connector.Sales.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Response for Synchronize Salesperon Customers
    /// </summary>
    public class SyncSalespersonCustomersResponse : AbstractSyncResponse
    {

        /// <summary>
        /// List of added or changed(updated/status deleted) salesperson customers
        /// </summary>
        public ICollection<SalespersonCustomer> SalespersonCustomers { get; set; }


    }
}
