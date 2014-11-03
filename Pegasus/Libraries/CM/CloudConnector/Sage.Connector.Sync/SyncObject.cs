using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Sage.Connector.Sync.Common;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sync
{
    /// <summary>
    /// Implementation of the ISyncObject interface. This wrapper class handles the serialization and hashing of generic objects.
    /// </summary>
    public class SyncObject : ISyncObject
    {
        #region Private Members

        private readonly IList<ISyncObject> _children = new List<ISyncObject>();
        private readonly object _instance;
        private readonly string _resourceKind;
        private readonly string _idFieldName;
        private readonly string _externalChildId;
        private readonly string _externalId;
        private readonly string _hashKey;

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs the reading of a property via reflection. If the property does not exist, then the default value will be returned.
        /// </summary>
        /// <param name="propertyName">The name of the property to read the value for.</param>
        /// <param name="defaultValue">The value to return if the property does not exist.</param>
        /// <returns>The property value, or defaultValue if the property does not exist.</returns>
        private object GetProperty(string propertyName, object defaultValue)
        {
            if (String.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

            var property = _instance.GetType().GetProperty(propertyName);

            return ((property == null) ? defaultValue : property.GetValue(_instance, null));
        }

        /// <summary>
        /// Performs the setting of a property via reflection. If the property does not exist, then nothing is done.
        /// </summary>
        /// <param name="propertyName">The name of the property to set the value for.</param>
        /// <param name="value">The value to assign to the property.</param>
        private void SetProperty(string propertyName, object value)
        {
            if (String.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

            var property = _instance.GetType().GetProperty(propertyName);

            if (property == null) return;

            property.SetValue(_instance, value, null);
        }

        /// <summary>
        /// Gets the value for the field identified by propertyName as a string value.
        /// </summary>
        /// <param name="propertyName">The name of the property to get the value for.</param>
        /// <returns>The property value as a string.</returns>
        private string GetExternalId(string propertyName)
        {
            object value = GetProperty(propertyName, String.Empty);

            return (value is String) ? (string)value : value.ToString();
        }

        /// <summary>
        /// Enumerates the properties of the object instance looking for collections. The items in each collection will then 
        /// be added as wrapped child items of the object.
        /// </summary>
        private void LoadChildren()
        {
            if (String.IsNullOrEmpty(ExternalId)) return;

            foreach (var property in _instance.GetType().GetProperties())
            {
                object value;

                try
                {
                    value = property.GetValue(_instance, null);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Sage SyncObject", "LoadChildren " + ex.Message, EventLogEntryType.Error);
                    value = null;
                }

                if (value == null)
                {
                    continue;
                }

                // ReSharper disable UseIsOperator.2
                if (typeof(ICollection).IsInstanceOfType(value) || typeof(ICollection<>).IsInstanceOfType(value))
                // ReSharper restore UseIsOperator.2
                {

                    var resourceKind = (_resourceKind == null) ? property.Name : string.Format(SyncFormats.Resource, _resourceKind, property.Name);

                    if (resourceKind.Length > SyncValues.KeyColSize)
                    {
                        EventLog.WriteEntry("Sage SyncObject", "LoadChildren resourcekind col length exceeded", EventLogEntryType.Error);
                        throw new ArgumentOutOfRangeException(string.Format(SyncExceptions.LengthResourceKind, resourceKind));
                    }

                    foreach (var child in (ICollection) value)
                    {
                        try
                        {
                            _children.Add(new SyncObject(this, resourceKind, child));

                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry("Sage SyncObject", "LoadChildren -- add to _children collection error " + property.Name + " " + ex.Message, EventLogEntryType.Error);
                            throw;
                        }
                    }
                }

            }

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Child entity constructor for the wrapper class.
        /// </summary>
        /// <param name="parent">The root object that the instance belongs to.</param>
        /// <param name="resourceKind">The resource kind name for the instance being wrapped.</param>
        /// <param name="instance">The Nephos model object that is being wrapped.</param>
        private SyncObject(SyncObject parent, string resourceKind, object instance)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (String.IsNullOrEmpty(resourceKind)) throw new ArgumentNullException("resourceKind");
            if (instance == null) throw new ArgumentNullException("instance");

            var attr = instance.GetType().GetCustomAttributes(typeof(ExternalIdentifier), true);

            _instance = instance;
            _resourceKind = resourceKind;
            _idFieldName = (attr.Length == 0) ? parent.IdFieldName : ((ExternalIdentifier)attr[0]).Name;
            _hashKey = parent.HashKey;
            _externalId = parent.ExternalId;
            _externalChildId = GetExternalId(_idFieldName);

            LoadChildren();
        }

        /// <summary>
        /// Root entity constructor for the wrapper class.
        /// </summary>
        /// <param name="instance">The model object that is being wrapped.</param>
        /// <param name="idFieldName">The name of the property that identifies the external id field.</param>
        public SyncObject(object instance, string idFieldName = null)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            if (String.IsNullOrEmpty(idFieldName))
            {
                var attr = instance.GetType().GetCustomAttributes(typeof(ExternalIdentifier), true);
                if (attr.Length == 0)
                {
                    EventLog.WriteEntry("Sage SyncObject", "SyncObject ExternalIdentifier attribute missing from the instance of", EventLogEntryType.Error);
                    throw new ArgumentNullException("ExternalIdentifier attribute missing from the instance of " + instance.GetType());
                }
                _idFieldName = ((ExternalIdentifier)attr[0]).Name;
            }
            else
            {
                _idFieldName = idFieldName;
            }

            _instance = instance;
            _resourceKind = null;
            _hashKey = SyncHash.GetHash(_instance);
            _externalId = GetExternalId(_idFieldName);

            LoadChildren();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the SyncObject's hash matches the metadata's hash value. If the hashes do not match, the data row is 
        /// udpated with the wrapper objects fields and written to the database. Also checks to see if the local tick count is >
        /// the cloud tick count. If so, the record will be marked as "changed" so that it is pushed up.
        /// </summary>
        /// <param name="database">The SyncDatabase that will be updated if the hash values do not match.</param>
        /// <param name="row">The metadata row used for hash comparison and updating.</param>
        /// <param name="state">The root level entity state which determines the overall state.</param>
        /// <returns>True if the root state is not unchanged and the database is updated. False otherwise.</returns>
        public bool UpdateMetadata(ISyncDatabase database, DataRow row, SyncEntityState state)
        {
            if (database == null) throw new ArgumentNullException("database");
            if (row == null) throw new ArgumentNullException("row");

            row.BeginEdit();

            try
            {
                if (state != SyncEntityState.Unchanged)
                {
                    row[SyncFields.HashKey] = HashKey;
                    row[SyncFields.EndpointId] = 0;
                    row[SyncFields.EndpointTick] = database.Tick;
                }

                row[SyncFields.Deleted] = false;
                row[SyncFields.Active] = true;
            }
            finally
            {
                row.EndEdit();
            }

            return (state != SyncEntityState.Unchanged);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Indexer to allow for generic property access.
        /// </summary>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>The value of the accessed property.</returns>
        public object this[string propertyName]
        {
            get
            {
                return GetProperty(propertyName, null);
            }
            set
            {
                SetProperty(propertyName, value);
            }
        }

        /// <summary>
        /// The generic model object that is being wrapped by the interface.
        /// </summary>
        public object Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// The resource kind that will be stored in the metadata. For root level objects like Customers, this will be null.
        /// For child items such as Contacts and Addresses, this will be the name of the collection that the instance belongs to.
        /// </summary>
        public string ResourceKind
        {
            get
            {
                return _resourceKind;
            }
        }

        /// <summary>
        /// Returns the name of the property field that identifies the external id for the object.
        /// </summary>
        public string IdFieldName
        {
            get
            {
                return _idFieldName;
            }
        }

        /// <summary>
        /// The ExternalId for the wrapped model object. For child objects, this will be the id of the owning object.
        /// </summary>
        public string ExternalId
        {
            get
            {
                return _externalId;
            }
        }

        /// <summary>
        /// The ExternalId for the wrapped child model object. For root level objects this will be null.
        /// </summary>
        public string ExternalChildId
        {
            get
            {
                return _externalChildId;
            }
        }

        /// <summary>
        /// The hash value calculated from the instance of the Nephos model object. The hash is calculated on construction of the
        /// wrapper to ensure that further modifications are not compared to the metadata stored hash. Eg status flag changes, etc.
        /// </summary>
        public string HashKey
        {
            get
            {
                return _hashKey;
            }
        }

        /// <summary>
        /// Collection of child model objects (eg Addresses and Contacts) that belong to the root object.
        /// </summary>
        public IList<ISyncObject> Children
        {
            get
            {
                return _children;
            }
        }

        #endregion
    }
}
