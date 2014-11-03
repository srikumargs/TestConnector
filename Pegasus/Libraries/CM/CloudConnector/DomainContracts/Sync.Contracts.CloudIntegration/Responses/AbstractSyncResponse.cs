using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Sync.Contracts.Data;

namespace Sage.Connector.Sync.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// AbstractSyncResponse
    /// </summary>
    public  abstract class AbstractSyncResponse: Response
    {
        private ICollection<DeletedEntity> _deletedEntities;

        /// <summary>
        /// Sync Digest for the resource
        /// </summary>
        public virtual SyncDigest SyncDigest { get; set; }

        /// <summary>
        /// Collection of Deleted Entities 
        /// </summary>
        public virtual ICollection<DeletedEntity> DeletedEntities
        {
            get { return _deletedEntities ?? (_deletedEntities = new Collection<DeletedEntity>()); }
            set { _deletedEntities  = value; }
        }
    }
}