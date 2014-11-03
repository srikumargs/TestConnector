
using System;

namespace Sage.Connector.Sync.Contracts.Data
{

    /// <summary>
    /// SyncDigest contract for sync responses
    /// </summary>
    public class SyncDigest
    {
        /// <summary>
        /// Resource Kind Name
        /// </summary>
        public String ResourceKindName { get; set; }

        /// <summary>
        /// Endpoint Id associated with this tick value
        /// </summary>
        public int EndpointId { get; set; }

        /// <summary>
        /// Endpoint Tick Value
        /// </summary>
        public int EndpointTick { get; set; }
    }
}
