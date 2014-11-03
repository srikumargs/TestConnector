using System;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Sync.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Deleted entity
    /// </summary>
    public class DeletedEntity
    {
        /// <summary>
        /// Connector ResourceKind
        /// </summary>
        public String ResourceKind { get; set; }

        /// <summary>
        /// ExternalId 
        /// </summary>
        [ExternalIdReference]
        public String ExternalId { get; set;  }

        

    }
}
