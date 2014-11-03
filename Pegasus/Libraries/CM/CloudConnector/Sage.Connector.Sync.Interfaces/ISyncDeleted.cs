namespace Sage.Connector.Sync.Interfaces
{
    /// <summary>
    /// Interface definition for entities that have been marked as deleted by the sync process.
    /// </summary>
    public interface ISyncDeleted
    {
        /// <summary>
        /// The resource kind for the deleted entity.
        /// </summary>
        string ResourceKind { get; }

        /// <summary>
        /// The external id for the deleted entity.
        /// </summary>
        string ExternalId { get; }

        /// <summary>
        /// True if this is a root level entity, false if a child of another entity.
        /// </summary>
        bool IsRoot { get; }
    }
}