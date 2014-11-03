using System;
using System.Collections.Generic;
using System.Data;

namespace Sage.Connector.Sync.Interfaces
{
    /// <summary>
    /// The sync database interface.
    /// </summary>
    public interface ISyncDatabase : IDisposable
    {
        /// <summary>
        /// Creates or opens the tenant database. Once open, the tenant database is set as the database for the connection.
        /// </summary>
        void OpenDatabase();

        /// <summary>
        /// Closes the tenant database.
        /// </summary>
        void CloseDatabase();

        /// <summary>
        /// Sets the ResourceKind scope and sets the current tick and cloud tick count. 
        /// </summary>
        /// <param name="resourceKind">The ResourceKind that determines which metadata table will be used.</param>
        /// <param name="cloudTick">The current cloud tick value.</param>
        void BeginSession(string resourceKind, int cloudTick);

        /// <summary>
        /// Ends the session on the ResourceKind scope.
        /// </summary>
        void EndSession();

        /// <summary>
        /// Starts the synchronization process.
        /// </summary>
        /// <param name="syncType">Specifies the desired client mode of operation. Internal = full sync, External = only changes from ERP.</param>
        /// <param name="trackDeletes">Determines if the sync will return a list of deleted items.</param>
        /// <returns>
        /// SyncMode.FullSync if all model objects are expected to be processed. If syncType was External, and the database is in a state
        /// to process changed records, then the return value will be SyncMode.ChangeSync. For this mode, only changed records are expected.
        /// </returns>
        SyncMode BeginSync(SyncType syncType, bool trackDeletes = true);

        /// <summary>
        /// Finalizes the sync process and returns the database to the opened state.
        /// </summary>
        /// <param name="syncCompleted">True if the Sync has completed processing the plugin data rows and processing was not stopped in the middle of synchronization.</param>
        /// <returns>A collection of resource kind and external id values that have were marked as deleted during the sync process.</returns>
        IDictionary<int, ISyncDeleted> EndSync(bool syncCompleted);

        /// <summary>
        /// Creates a hashset that contains all the external id values that are being tracked in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resource kind to get the keys for.</param>
        /// <returns>A hashset that contains all the external id values for the specified resource.</returns>
        HashSet<string> GetResourceKeys(string resourceKind);

        /// <summary>
        /// Creates a hashset that contains all the external id values that are being tracked and marked as active in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resource kind to get the keys for.</param>
        /// <returns>A hashset that contains all the external id values for the specified resource kind.</returns>
        HashSet<string> GetActiveResourceKeys(string resourceKind);

        /// <summary>
        /// Finds the metadata record for the specified syncObject entity. 
        /// </summary>
        /// <param name="syncObject">The sync object to locate the record for.</param>
        /// <param name="readFlags">If AutoAdd is specified, a new in-memory record will be added on match failure.</param>
        /// <returns>
        /// The existing (or newly added) data row for the located entity. Null will be returned if the record did not
        /// exist and the AutoAdd flag was not specified.
        /// </returns>
        DataRow FindMetaRecord(ISyncObject syncObject, SyncReadFlags readFlags);

        /// <summary>
        /// Reads the metadata record(s) from the table specified by the current ResourceKind. The SyncObject's
        /// ResourceKind, ExternalId and ExternalChildId will be used to determine the record that is returned. 
        /// </summary>
        /// <param name="syncObject">The sync object to read the record for.</param>
        /// <param name="readFlags">
        /// Determines if all records matching the ExternalId will be returned, and if a new record should be generated
        /// if zero records are found.
        /// </param>
        /// <returns>The existing or newly added data row for the entity.</returns>
        /// <remarks>
        /// For external change mode handling, all records that have an ExternalId field value == syncObject.ExternalId will
        /// have their Active flag set to false. This allows for child entity delete detection.
        /// </remarks>
        DataRow ReadMetaRecord(ISyncObject syncObject, SyncReadFlags readFlags);

        /// <summary>
        /// Allows the sync client to revert the tick count if no changes were detected during sync processing.
        /// </summary>
        void RevertDigestTick();

        /// <summary>
        /// Writes the current metadata records to the SQL table based on each row's RowState value. For unchanged rows, nothing is 
        /// done. For added rows, an INSERT is performed. And for changed rows, an UPDATE is performed.
        /// </summary>
        void WriteMetaRecords();

        /// <summary>
        /// For external change detection, the ERP plugin won't be able to build an entity model object in order to represent a delete. This
        /// method allows for an entity (or child entity) to marked as Active=false, so that it will be picked up as deleted when
        /// the EndSync() method is called.
        /// </summary>
        /// <param name="erpId">The ExternalId of the root entity to locate. Cannot be blank or null.</param>
        /// <param name="resourceKind">For child entities, the resource kind that represents the entity, eg: Addresses. Null for root entities.</param>
        /// <param name="erpChildId">For child entities, the ExternalId of the entity. Null for root entities.</param>
        /// <returns>True if one or more records were marked as inactive.</returns>
        bool RemoveMetaRecords(string erpId, string resourceKind = null, string erpChildId = null);

        /// <summary>
        /// Returns the current state of the database,
        /// </summary>
        SyncDbState State { get; }

        /// <summary>
        /// Returns the sync mode that the database is currently in.
        /// </summary>
        SyncMode Mode { get; }

        /// <summary>
        /// If State is FullSync or ChangeSync, this will be the updated local tick count for the sync process, otherwise
        /// this will return (-1).
        /// </summary>
        int Tick { get; }

        /// <summary>
        /// Returns the cloud tick count passed in during the call to BeginSync
        /// </summary>
        int CloudTick { get; }

        /// <summary>
        /// The ResourceKind scope as set by the BeginResourceTrans method.
        /// </summary>
        string ResourceKind { get; }

        /// <summary>
        /// The tenant id that the metadata is stored for. This is used to define the actual database name.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// The DataTable that contains records filled from a call to ReadMetaRecords or ReadMetaRecord. 
        /// </summary>
        DataTable MetaRecords { get; }
    }
}
