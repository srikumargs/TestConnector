using System.Collections.Generic;
using Sage.Connector.Customers.Contracts.Data;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace Sage.Connector.Customers.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Response for Synchronize Customers
    /// </summary>
    public class SyncCustomersResponse : AbstractSyncResponse
    {

        /// <summary>
        /// List of added or changed(updated/status deleted) customers
        /// </summary>
        public ICollection<Customer> Customers { get; set; }


    }
}
