using System.Collections.Generic;
using System.Data;

namespace Sage.Connector.Sync.Interfaces
{
    /// <summary>
    /// Interface to handle property interaction with an instance of an object.
    /// </summary>
    public interface ISyncObject
    {
        /// <summary>
        /// Determines if the SyncObject's hash matches the metadata's hash value. If the hashes do not match, the data row is 
        /// udpated with the wrapper objects fields and written to the database. Also checks to see if the local tick count is >
        /// the cloud tick count. If so, the record will be marked as "changed" so that it is pushed up.
        /// </summary>
        /// <param name="database">The SyncDatabase that will be updated if the hash values do not match.</param>
        /// <param name="row">The metadata row used for hash comparison and updating.</param>
        /// <param name="state">The root level entity state which determines the overall state.</param>
        /// <returns>True if the root state is not unchanged and the database is updated. False otherwise.</returns>
        bool UpdateMetadata(ISyncDatabase database, DataRow row, SyncEntityState state);

        /// <summary>
        /// The generic model object that is being wrapped by the interface.
        /// </summary>
        object Instance { get; }

        /// <summary>
        /// Returns the name of the property field that identifies the external id for the object.
        /// </summary>
        string IdFieldName { get; }

        /// <summary>
        /// The resource kind that will be stored in the metadata. For root level objects like Customers, this will be null.
        /// For child items such as Contacts and Addresses, this will be the name of the collection that the instance belongs to.
        /// </summary>
        string ResourceKind { get; }

        /// <summary>
        /// The ExternalId for the wrapped model object. For child objects, this will be the id of the owning object.
        /// </summary>
        string ExternalId { get; }

        /// <summary>
        /// The ExternalId for the wrapped child model object. For root level objects this will be null.
        /// </summary>
        string ExternalChildId { get; }

        /// <summary>
        /// The hash value calculated from the instance of the Nephos model object. The hash is calculated on construction of the
        /// wrapper to ensure that further modifications are not compared to the metadata stored hash. Eg status flag changes, etc.
        /// </summary>
        string HashKey { get; }

        /// <summary>
        /// Collection of child model objects (eg Addresses and Contacts) that belong to the root object.
        /// </summary>
        IList<ISyncObject> Children { get; }
    }
}
