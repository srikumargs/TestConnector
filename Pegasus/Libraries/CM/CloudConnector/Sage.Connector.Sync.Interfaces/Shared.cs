using System;

namespace Sage.Connector.Sync.Interfaces
{
    /// <summary>
    /// Custom attribute for marking a class with a property name that identifies the external id for the object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ExternalIdentifier : Attribute
    {
        private readonly string _name;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the property which will be used as the external id key.</param>
        public ExternalIdentifier(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            _name = name;
        }

        /// <summary>
        /// The name of the property which will be used as the external id key. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
    }

    /// <summary>
    /// Enumeration of states that the ISyncDatabase can be in.
    /// </summary>
    public enum SyncDbState
    {
        /// <summary>
        /// Database is closed or does not exist.
        /// </summary>
        Closed,

        /// <summary>
        /// Database is open.
        /// </summary>
        Opened,

        /// <summary>
        /// Database is set to work against a specific resource kind and metadata table.
        /// </summary>
        Session,

        /// <summary>
        /// Database is in a sync process against a specific resource kind.
        /// </summary>
        Syncing
    }

    /// <summary>
    /// Indicates the state of the ResourceKind entity after processing through the framework.
    /// </summary>
    public enum SyncEntityState
    {
        /// <summary>
        /// ResourceKind entity existed in the metadata store, but was unchanged.
        /// </summary>
        Unchanged,

        /// <summary>
        /// ResourceKind entity was added to the metadata store.
        /// </summary>
        Added,

        /// <summary>
        /// ResourceKind entity existed in the metadata store, but was updated due to changes.
        /// </summary>
        Updated,

        /// <summary>
        /// ResourceKind entity existed in the metadata store, but was removed from the ERP system. This status is never
        /// returned from a call to SyncClient.SyncEntity.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Flags used by the ReadMetaData method. These flags determine how the database read operation will function.
    /// </summary>
    [Flags]
    public enum SyncReadFlags
    {
        /// <summary>
        /// No flags set
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Indicates that if the record is not found a new in-memory record should be generated.
        /// </summary>
        AutoAdd = 0x01
    }

    /// <summary>
    /// Indicates the sync type to use as specified by the plugin.
    /// </summary>
    public enum SyncType
    {
        /// <summary>
        /// Sync process will be performed using internal logic which means running in "full" mode for each run.
        /// </summary>
        Internal,

        /// <summary>
        /// Sync process will be performed using plugin external change logic if allowed by the data state.
        /// </summary>
        External
    }

    /// <summary>
    /// Indicates the mode that the sync is operating in.
    /// </summary>
    public enum SyncMode
    {
        /// <summary>
        /// Default state where the ResourceKind has not yet been set.
        /// </summary>
        None,

        /// <summary>
        /// In a session mode where the database is open and the resource kind has been set.
        /// </summary>
        Session,

        /// <summary>
        /// Sync process has been started and is running in full sync mode, where it is expected that all ERP records will
        /// be processed.
        /// </summary>
        FullSync,

        /// <summary>
        /// Sync process has been started and is running in changed sync mode, where only changed records from the ERP
        /// will be processed.
        /// </summary>
        ChangeSync
    }
}