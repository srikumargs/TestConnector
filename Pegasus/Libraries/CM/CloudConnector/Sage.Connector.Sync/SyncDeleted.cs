using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sync
{
    /// <summary>
    /// Implementation for the ISyncDeleted interface. Provides an object wrapper for exposing the required information
    /// for a deleted item back to the consumer.
    /// </summary>
    public class SyncDeleted : ISyncDeleted
    {
        #region Private Members

        private readonly string _resourceKind;
        private readonly string _externalId;
        private readonly bool _isRoot;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resourceKind">The resource kind for the deleted entity.</param>
        /// <param name="externalId">The external id for the deleted entity.</param>
        /// <param name="isRoot">True if this is was a root level entity.</param>
        public SyncDeleted(string resourceKind, string externalId, bool isRoot)
        {
            _resourceKind = resourceKind;
            _externalId = externalId;
            _isRoot = isRoot;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The resource kind of the deleted entity.
        /// </summary>
        public string ResourceKind
        {
            get
            {
                return _resourceKind;
            }
        }

        /// <summary>
        /// The external if of the deleted entity.
        /// </summary>
        public string ExternalId
        {
            get { return _externalId; }
        }

        /// <summary>
        /// True if this is a root level entity, false if a child.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return _isRoot;
            }
        }

        #endregion
    }
}
