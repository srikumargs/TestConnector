using System;

namespace Sage.Connector.Sync.Contracts.CloudIntegration.Requests
{
    /// <summary>
    /// Sync Request Payload
    /// </summary>
    public class SyncRequest
    {
        /// <summary>
        /// Resource Kind Name to be used for this synchronization
        /// </summary>
        public String ResourceKindName { get; set; }

        /// <summary>
        /// Cloud Tick value used by sync to determine changed entities for internal sync's
        /// </summary>
        public int CloudTick { get; set; }
   
    }
}
