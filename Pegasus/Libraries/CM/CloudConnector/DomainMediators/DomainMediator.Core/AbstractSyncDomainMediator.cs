using System.Linq;
using Sage.Connector.DomainMediator.Core.Utilities;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.DomainMediator.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractSyncDomainMediator : AbstractDomainMediator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="response"></param>
        protected void ProcessDeletedEntities(ISyncClient client, AbstractSyncResponse response)
        {
            foreach (var deletedEntity in client.DeletedEntities.Values.Select(syncDeleted => new DeletedEntity
            {
                ExternalId = syncDeleted.ExternalId,
                ResourceKind = syncDeleted.ResourceKind

            }))
            {
                ExternalIdUtilities.ApplyExternalIdEncoding(deletedEntity);
                response.DeletedEntities.Add(deletedEntity);
            }
        }
    }
}
