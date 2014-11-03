using System;
using System.Collections.Generic;

namespace Sage.Connector.Sync.Interfaces
{
    /// <summary>
    /// The ISyncClient interface which will be interacted with in order to perform sync change detection.
    /// </summary>
    public interface ISyncClient : IDisposable
    {
        /// <summary>
        /// Opens the underlying database object.
        /// </summary>
        void OpenDatabase();

        /// <summary>
        /// Closes the underlying database object.
        /// </summary>
        void CloseDatabase();

        /// <summary>
        /// Starts the sync database resource transaction, and prepares the sync for processing against a specific resource kind.
        /// </summary>
        /// <param name="session">The object list to store changed or modified entities passed to SyncEntity.</param>
        /// <param name="resourceKind">The resource kind that the sync process will work against.</param>
        /// <param name="cloudTick">The current cloud tick for the resource kind.</param>
        void BeginSession(IList<object> session, string resourceKind, int cloudTick);

        /// <summary>
        /// Ends the session for the resource kind.
        /// </summary>
        void EndSession();

        /// <summary>
        /// Starts a sync process. The digest record for the resource kind will have it's tick count incremented, and the
        /// sync database will determine the actual SyncMode based on existing records as well as failed uploads. 
        /// If there are records and everything is current, then a request for SyncType.External will be honored.
        /// </summary>
        /// <param name="syncType">Specifies the desired client mode of operation. Internal = full sync, External = only changes from ERP</param>
        /// <param name="trackDeletes">Determines if the sync will return a list of deleted items.</param>
        /// <returns>
        /// SyncMode.FullSync if all model objects are expected to be processed. If syncType was External, and the database is in a state
        /// to process changed records, then the return value will be SyncMode.ChangeSync. For this mode, only changed records are expected.
        /// </returns>
        SyncMode BeginSync(SyncType syncType, bool trackDeletes = true);

        /// <summary>
        /// Ends the sync process, determines records that have not been processed, and marks records for delete.
        /// </summary>
        /// <param name="syncComplete">True if the Sync has completed processing the plugin data rows and processing 
        /// was not stopped in the middle of synchronization.</param>
        /// <returns>The local tick count that was used during the sync process.</returns>
        int EndSync(bool syncComplete);

        /// <summary>
        /// Adds a ResourceKind entity to the sync engine for processing. If added or changed, the entity object will be 
        /// placed in the object list session.
        /// </summary>
        /// <param name="entityModel">The entity model object to process.</param>
        /// <param name="idFieldName">The name of the property field that identifies the external id.</param>
        /// <returns>
        /// The state of the entity after processing. For entities that have children, this will reflect the state of the root
        /// entity based on its state plus the state of its children. Eg:
        /// 
        /// Root        Child         State
        /// ------------------------------------
        /// Unchanged   Unchanged   = Unchanged
        /// Changed     Unchanged   = Updated
        /// Unchanged   Changed     = Updated;
        /// Any         Added       = Updated
        /// Any         Deleted     = Updated
        /// Added       Any         = Added
        /// </returns>
        SyncEntityState SyncEntity(object entityModel, string idFieldName = null);

        /// <summary>
        /// Marks the entity and all it's children as being soft deleted. This call is required for external change detection,
        /// as the ERP plugin will not have the data to create an entity model object.
        /// </summary>
        /// <param name="externalId">The entity model's external id.</param>
        /// <returns>True if a metadata record was located and marked as Deleted, false otherwise.</returns>
        bool RemoveEntity(string externalId);

        /// <summary>
        /// A dictionary object that will contain the list of deleted entity values. This dictionary is 
        /// cleared when BeginSync is called.
        /// </summary>
        IDictionary<int, ISyncDeleted> DeletedEntities { get; }

        /// <summary>
        /// Creates a hashset that contains all the Erp_Id keys that are being tracked in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resourceKind to get the keys for.</param>
        /// <returns>A hashset that contains all the Erp_Id keys for the specified resource.</returns>
        HashSet<string> GetResourceKeys(string resourceKind);

        /// <summary>
        /// Creates a hashset that contains all the Erp_Id keys that are being tracked and active in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resourceKind to get the keys for.</param>
        /// <returns>A hashset that contains all the Erp_Id keys for the specified resource.</returns>
        HashSet<string> GetActiveResourceKeys(string resourceKind);

        /// <summary>
        /// Returns the mode that the Sync object is currently operating in.
        /// </summary>
        SyncMode Mode { get; }

        /// <summary>
        /// Returns the current state of the database object.
        /// </summary>
        SyncDbState DatabaseState { get; }

        /// <summary>
        /// Returns the local tick count. If the current mode is None, (-1) will be returned.
        /// </summary>
        int Tick { get; }

        /// <summary>
        /// Returns the combined count of added, changed, and removed entities during the sync run.
        /// </summary>
        int SyncChangeCount { get; }

        /// <summary>
        /// Returns the cloud tick count passed in BeginSync. If the mode is None, (-1) will be returned.
        /// </summary>
        int CloudTick { get; }

        /// <summary>
        /// Returns the path that is being used to store the sync metadata for the tenant.
        /// </summary>
        string DataPath { get; }

        /// <summary>
        /// The tenant id that the metadata is stored for. 
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// The ResourceKind scope as set by the BeginSession method.
        /// </summary>
        string ResourceKind { get; }
    }
}
