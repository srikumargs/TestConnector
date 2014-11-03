using System;
using System.Collections.Generic;
using System.Data;
using Sage.Connector.Sync.Common;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sync
{
    /// <summary>
    /// An implementation of the ISyncClient interface used for handling delta sync.
    /// </summary>
    public class SyncClient : ISyncClient
    {
        #region Private Members

        private readonly Dictionary<int, ISyncDeleted> _deleted = new Dictionary<int, ISyncDeleted>();
        private readonly string _dataPath;
        private ISyncDatabase _database;
        private IList<object> _session;
        private bool _disposed;
        private int _changeCount;

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if the client is in a syncing state.
        /// </summary>
        /// <returns>True if BeginSync has been called, false otherwise.</returns>
        private bool IsSyncMode()
        {
            return ((_database.Mode == SyncMode.ChangeSync) || (_database.Mode == SyncMode.FullSync));
        }

        /// <summary>
        /// Adds a child ResourceKind entity to the sync engine for processing. This is a recursive call that allows all
        /// child levels to be processed.
        /// </summary>
        /// <param name="childEntity">The child entity model object to process.</param>
        /// <param name="state">The state of the parent entity object.</param>
        /// <returns>The running state of the entity after processing.</returns>
        private void SyncChildEntity(ISyncObject childEntity, SyncEntityState state)
        {
            foreach (var child in childEntity.Children)
            {
                SyncChildEntity(child, state);
            }

            var rowRoot = _database.FindMetaRecord(childEntity, SyncReadFlags.AutoAdd);

            if (childEntity.UpdateMetadata(_database, rowRoot, state))
            {
                _changeCount++;
                if (_session != null)
                {
                    _session.Add(childEntity.Instance);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new SyncClient instance.
        /// </summary>
        /// <param name="tenantId">The tenant id which defines the database naming convention.</param>
        /// <param name="dataPath">The data path where the tenant sync metadata will be stored.</param>
        public SyncClient(string tenantId, string dataPath)
        {
            if (String.IsNullOrEmpty(tenantId)) throw new ArgumentNullException("tenantId");
            if (String.IsNullOrEmpty(dataPath)) throw new ArgumentNullException("dataPath");

            _dataPath = dataPath;
            _deleted.Clear();
            _database = new SyncDatabase(tenantId, _dataPath);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor for SyncClient
        /// </summary>
        ~SyncClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application defined tasks associated with freeing and releasing.
        /// </summary>
        /// <param name="disposing">True if we should be disposing of managed items, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_database != null)
                {
                    _database.Dispose();
                    _database = null;
                }
            }
            
            _disposed = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing and releasing.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Opens the underlying database object.
        /// </summary>
        public void OpenDatabase()
        {
            _database.OpenDatabase();
        }

        /// <summary>
        /// Closes the underlying database object.
        /// </summary>
        public void CloseDatabase()
        {
            _database.CloseDatabase();   
        }

        /// <summary>
        /// Starts the sync database resource transaction, and prepares the sync for processing against a specific resource kind.
        /// </summary>
        /// <param name="session">The object list to store changed or modified entities passed to SyncEntity.</param>
        /// <param name="resourceKind">The resource kind that the sync process will work against.</param>
        /// <param name="cloudTick">The current cloud tick for the resource kind.</param>
        public void BeginSession(IList<object> session, string resourceKind, int cloudTick)
        {
            _database.BeginSession(resourceKind, cloudTick);
            _deleted.Clear();
            _session = session;
            _changeCount = 0;
        }


        /// <summary>
        /// Starts the sync database resource transaction, and prepares the sync for processing against a specific resource kind.
        /// Call this method when session list is kept outside of the session.  
        /// </summary>
        /// <param name="resourceKind">The resource kind that the sync process will work against.</param>
        /// <param name="cloudTick">The current cloud tick for the resource kind.</param>
        public void BeginSession(string resourceKind, int cloudTick)
        {
            BeginSession(null, resourceKind, cloudTick);
        }
        /// <summary>
        /// Ends the session for the resource kind.
        /// </summary>
        public void EndSession()
        {
            _database.EndSession();
            _deleted.Clear();
            _changeCount = 0;
        }

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
        public SyncMode BeginSync(SyncType syncType, bool trackDeletes = true)
        {
            var mode = _database.BeginSync(syncType, trackDeletes);

            if (_session != null)
            {
                _session.Clear();
            }

            return mode;
        }

        /// <summary>
        /// Ends the sync process, determines records that have not been processed, and marks records for delete.
        /// </summary>
        /// <param name="syncComplete">True if the Sync has completed processing the plugin data rows and processing 
        /// was not stopped in the middle of synchronization.</param>
        /// <returns>The local tick count that was used during the sync process.</returns>
        public int EndSync(bool syncComplete)
        {
            var deleted = _database.EndSync(syncComplete);

            _deleted.Clear();

            foreach (var item in deleted)
            {
                _deleted.Add(item.Key, item.Value);
            }

            _changeCount += _deleted.Count;

            if (_changeCount == 0)
            {
                _database.RevertDigestTick();
            }

            return _database.Tick;
        }

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
        public SyncEntityState SyncEntity(object entityModel, string idFieldName = null)
        {
            if (!IsSyncMode())
            {
                throw new InvalidOperationException(SyncExceptions.WrongMode);
            }

            if (entityModel == null) return SyncEntityState.Unchanged;

            var entity = new SyncObject(entityModel, idFieldName);
            var rowRoot = _database.ReadMetaRecord(entity, SyncReadFlags.AutoAdd);
            var state = (rowRoot.RowState == DataRowState.Added) ? SyncEntityState.Added : SyncEntityState.Unchanged;

            if (state == SyncEntityState.Unchanged)
            {
                if (((int)rowRoot[SyncFields.EndpointTick] > _database.CloudTick) || !entity.HashKey.Equals(rowRoot[SyncFields.HashKey]) || (bool)rowRoot[SyncFields.Deleted])
                {
                    state = SyncEntityState.Updated;
                }
            }

            foreach (var child in entity.Children)
            {
                SyncChildEntity(child, state);
            }

            if (entity.UpdateMetadata(_database, rowRoot, state))
            {
                _changeCount++;
                if (_session != null)
                {
                    _session.Add(entity.Instance);
                }
            }

            _database.WriteMetaRecords();

            return state;
        }

        /// <summary>
        /// Marks the entity and all it's children as being soft deleted. This call is required for external change detection,
        /// as the ERP plugin will not have the data to create an entity model object.
        /// </summary>
        /// <param name="externalId">The entity model's external id.</param>
        /// <returns>True if a metadata record was located and marked as Deleted, false otherwise.</returns>
        public bool RemoveEntity(string externalId)
        {
            if (!_database.RemoveMetaRecords(externalId)) return false;
                
            _changeCount++;

            return true;
        }

        /// <summary>
        /// Creates a hashset that contains all the Erp_Id keys that are being tracked in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resourceKind to get the keys for.</param>
        /// <returns>A hashset that contains all the Erp_Id keys for the specified resource.</returns>
        public HashSet<string> GetResourceKeys(string resourceKind)
        {
            return _database.GetResourceKeys(resourceKind);
        }

        /// <summary>
        /// Creates a hashset that contains all the Erp_Id keys that are being tracked and active in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resourceKind to get the keys for.</param>
        /// <returns>A hashset that contains all the Erp_Id keys for the specified resource.</returns>
        public HashSet<string> GetActiveResourceKeys(string resourceKind)
        {
            return _database.GetActiveResourceKeys(resourceKind);  
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A dictionary object that will contain the list of deleted entity values. This dictionary is 
        /// cleared when BeginSync is called.
        /// </summary>
        public IDictionary<int, ISyncDeleted> DeletedEntities
        {
            get
            {
                return _deleted;
            }
        }

        /// <summary>
        /// Returns the mode that the Sync object is currently operating in.
        /// </summary>
        public SyncMode Mode
        {
            get
            {
                return _database.Mode;
            }
        }

        /// <summary>
        /// Returns the local tick count. If the current mode is None, (-1) will be returned.
        /// </summary>
        public int Tick
        {
            get
            {
                return _database.Tick;
            }
        }

        /// <summary>
        /// Returns the combined count of added, changed, and removed entities during the sync run.
        /// </summary>
        public int SyncChangeCount
        {
            get
            {
                return _changeCount;
            }
        }

        /// <summary>
        /// Returns the cloud tick count passed in BeginSync. If the mode is None, (-1) will be returned.
        /// </summary>
        public int CloudTick
        {
            get
            {
                return _database.CloudTick;
            }
        }

        /// <summary>
        /// Returns the path that is being used to store the sync metadata for the tenant.
        /// </summary>
        public string DataPath
        {
            get
            {
                return _dataPath;
            }
        }

        /// <summary>
        /// The tenant id that the metadata is stored for. 
        /// </summary>
        public string TenantId
        {
            get
            {
                return _database.TenantId;
            }
        }

        /// <summary>
        /// Returns the current state of the database object.
        /// </summary>
        public SyncDbState DatabaseState
        {
            get
            {
                return _database.State;
            }
        }

        /// <summary>
        /// The ResourceKind scope as set by the BeginSession method.
        /// </summary>
        public string ResourceKind
        {
            get
            {
                return _database.ResourceKind;
            }
        }

        #endregion
    }
}
