using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Data.SqlServerCe;
using System.Text;
using Sage.Connector.Sync.Common;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sync
{
    /// <summary>
    /// An ISyncDatabase implementation for metadata storage using a SQL CE database based on tenant id and resource kind.
    /// </summary>
    public class SyncDatabase : ISyncDatabase
    {
        #region Private Members

        private SqlCeConnection _connection = new SqlCeConnection();
        private DataTable _metaRecords;
        private SyncDbState _state = SyncDbState.Closed;
        private SyncMode _mode = SyncMode.None;
        private readonly string _dataPath;
        private readonly string _tenantId;
        private string _resourceKind;
        private bool _trackDeletes;
        private bool _unsavedSync;
        private bool _disposed;
        private int _tick = SyncValues.Invalid;
        private int _cloudTick = SyncValues.Invalid;

        #endregion

        #region Private Methods

        /// <summary>
        /// Encodes the specified string value so that it can be used in a sql statement. If the string is null, 
        /// then " IS NULL" will be returned as the value.
        /// </summary>
        /// <param name="value">The string value to build for the SQL condition.</param>
        /// <returns>The operator and encoded string value, eg: " = 'value'"</returns>
        private static string GenerateCondition(string value)
        {
            if (value == null) return SyncDml.DynamicNull;

            var sql = new StringBuilder();

            sql.Append(SqlSyntax.EqualsOp);
            sql.Append(SqlSyntax.Quote);

            if (!String.IsNullOrEmpty(value))
            {
                sql.Append(value.Replace("\'", "\'\'"));
            }

            sql.Append(SqlSyntax.Quote);


            return sql.ToString();
        }

        /// <summary>
        /// Uses direct SQL CE table access to insert a new record.
        /// </summary>
        /// <param name="recordset">The recordset to insert the new record into.</param>
        /// <param name="row">The data row containing the values to insert.</param>
        private static void InsertMetaRecord(SqlCeResultSet recordset, DataRow row)
        {
            if (recordset == null)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "InsertMetaRecord recordset is null", EventLogEntryType.Error);
                throw new ArgumentNullException("recordset");
            }
            if (row == null)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "InsertMetaRecord row is null", EventLogEntryType.Error);
                throw new ArgumentNullException("row");
            }

            var record = recordset.CreateRecord();

            for (var i = SyncFieldIndexes.ResourceKind; i < recordset.FieldCount; i++)
            {
                record[i] = row[i];
            }

            try
            {
                row.AcceptChanges();

                recordset.Insert(record);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "InsertMetaRecord " + ex.Message, EventLogEntryType.Error);

                throw;
            }
        }

        /// <summary>
        /// Uses direct SQL CE table access to update an existing record.
        /// </summary>
        /// <param name="recordset">The recordset to insert the new record into.</param>
        /// <param name="row">The data row containing the values to insert.</param>
        private static void UpdateMetaRecord(SqlCeResultSet recordset, DataRow row)
        {
            if (recordset == null)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "UpdateMetaRecord recordset is null", EventLogEntryType.Error);
                throw new ArgumentNullException("recordset");
            }
            if (row == null)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "UpdateMetaRecord row is null", EventLogEntryType.Error);
                throw new ArgumentNullException("row");
            }

            if (row.RowState == DataRowState.Unchanged) return;

            if (row.RowState == DataRowState.Modified)
            {
                if (recordset.Seek(DbSeekOptions.FirstEqual, new[] { row[SyncFieldIndexes.Id] }))
                {
                    if (recordset.Read())
                    {
                        for (var i = SyncFieldIndexes.ResourceKind; i < recordset.FieldCount; i++)
                        {
                            if (row.ColumnChanged(recordset.GetName(i)))
                            {
                                recordset.SetValue(i, row[i]);
                            }
                        }

                        try
                        {
                            recordset.Update();
                            row.AcceptChanges();
                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry("Sage SyncDatabase", "UpdateMetaRecord " + ex.Message, EventLogEntryType.Error);
                            throw;
                        }

                        return;
                    }
                }
            }

            InsertMetaRecord(recordset, row);
        }

        /// <summary>
        /// Builds the formatted metatable name using the specified resource kind name.
        /// </summary>
        /// <param name="resouceKind">The resource kind name to use for the table name.</param>
        /// <returns>The database metatable name.</returns>
        private static string GetMetatableName(string resouceKind)
        {
            if (String.IsNullOrEmpty(resouceKind)) throw new ArgumentNullException("resouceKind");
            return String.Format(SyncFormats.Metatable, resouceKind);
        }

        /// <summary>
        /// Compares the row's column value against the specified value.
        /// </summary>
        /// <param name="row">The row containing the column to compare.</param>
        /// <param name="columnName">The name of the column to get the value from.</param>
        /// <param name="value">The value to compare against.</param>
        /// <returns>True if the column value matches, otherwise false.</returns>
        private static bool CompareColumnValue(DataRow row, string columnName, string value)
        {
            if (row == null) throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            return (String.Compare(value, (row.IsNull(columnName) ? null : (string)row[columnName]), StringComparison.Ordinal) == 0);
        }

        /// <summary>
        /// Generates a new metadata row seeded with default values. To persist the data row; fill in the fields and 
        /// then call WriteMetaRecords.
        /// </summary>
        /// <returns>The newly generated (in memory) data row.</returns>
        private DataRow NewMetaRecord(ISyncObject syncObject)
        {
            if (!IsResourceState())
            {
                EventLog.WriteEntry("Sage SyncDatabase", "NewMetaRecord wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            var result = _metaRecords.NewRow();

            result.BeginEdit();

            try
            {
                result[SyncFields.ResourceKind] = syncObject.ResourceKind;
                result[SyncFields.ExternalId] = SyncConvert.Encode(syncObject.ExternalId);
                result[SyncFields.ExternalChildId] = SyncConvert.Encode(syncObject.ExternalChildId);
                result[SyncFields.HashKey] = String.Empty;
                result[SyncFields.EndpointId] = 0;
                result[SyncFields.EndpointTick] = (_state == SyncDbState.Syncing) ? _tick : 0;
                result[SyncFields.Deleted] = false;
                result[SyncFields.Active] = true;
            }
            finally
            {
                result.EndEdit();
            }

            _metaRecords.Rows.Add(result);

            return result;
        }

        /// <summary>
        /// Determines if the sync database is in a state where the ResourceKind has been set. This means being in either a
        /// Session or Syncing state.
        /// </summary>
        /// <returns>True if in a resource state, false otherwise</returns>
        private bool IsResourceState()
        {
            return ((_state == SyncDbState.Session) || (_state == SyncDbState.Syncing));
        }

        /// <summary>
        /// Builds an absolute path to the database filename.
        /// </summary>
        /// <returns>The absolute file path to the database.</returns>
        private string GetDatabaseName()
        {
            return GetDatabaseFilename(_dataPath, _tenantId);
        }

        /// <summary>
        /// Builds a connection string that can be used to connect to the Sql CE database.
        /// </summary>
        /// <returns>A valid Sql CE connection string.</returns>
        private string GetConnectionString()
        {
            return String.Format(SyncFormats.Connection, GetDatabaseName());
        }

        /// <summary>
        /// Purges deleted entities from the metadata table based on tick counts that fall below the specified threshold.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        private void PurgeDeleted()
        {
            if ((_tick > SyncValues.PurgeDelta) && ((_tick % SyncValues.PurgeCheck) == 0))
            {
                using (var command = new SqlCeCommand(String.Format(SyncDml.PurgeDeleted, GetMetatableName(_resourceKind), _tick), _connection))
                {
                    command.CommandTimeout = 0;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Returns a hashset of all the external id values from the specified table.
        /// </summary>
        /// <param name="resourceKind">Determines the sync table name to read the data from.</param>
        /// <param name="includeDeleted">True if records marked as deleted should be queries.</param>
        /// <returns>The hashset of external id values read from the table</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public HashSet<string> ReadResourceKeys(string resourceKind, bool includeDeleted)
        {
            if (String.IsNullOrEmpty(resourceKind))
            {
                EventLog.WriteEntry("Sage SyncDatabase", "ReadResourceKeys  null resourceKind", EventLogEntryType.Error);
                throw new ArgumentNullException("resourceKind");
            }

            if (_state == SyncDbState.Closed)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "ReadResourceKeys  state closed - wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            var keys = new HashSet<string>();

            if (_connection.TableExists(GetMetatableName(resourceKind)))
            {
                using (var command = new SqlCeCommand(String.Format(includeDeleted ? SyncDml.ReadKeys : SyncDml.ReadActiveKeys, GetMetatableName(resourceKind)), _connection))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                keys.Add(SyncConvert.Decode((string)reader[0]));
                            }
                        }
                    }
                }
            }

            return keys;
        }

        /// <summary>
        /// Ensures that the database state transition rules are enforced. 
        /// </summary>
        /// <param name="newState">The new state we are transitioning to.</param>
        private void SetState(SyncDbState newState)
        {
            if (newState == _state) return;

            switch (newState)
            {
                case SyncDbState.Closed:
                    if (_state != SyncDbState.Opened)
                    {
                        EventLog.WriteEntry("Sage SyncDatabase", "SetState  state open -- wrong state", EventLogEntryType.Error);
                        throw new InvalidOperationException(SyncExceptions.WrongState);
                    }
                    _connection.Close();
                    _metaRecords = null;
                    _unsavedSync = false;
                    _resourceKind = string.Empty;
                    _tick = SyncValues.Invalid;
                    _cloudTick = SyncValues.Invalid;
                    _mode = SyncMode.None;
                    break;

                case SyncDbState.Opened:
                    if (_state == SyncDbState.Session)
                    {
                        if (_metaRecords != null)
                        {
                            _metaRecords.Dispose();
                        }
                        _metaRecords = null;
                        _unsavedSync = false;
                        _resourceKind = string.Empty;
                        _tick = SyncValues.Invalid;
                        _cloudTick = SyncValues.Invalid;
                        _mode = SyncMode.None;
                    }
                    else if (_state != SyncDbState.Closed)
                    {
                        EventLog.WriteEntry("Sage SyncDatabase", "SetState  state closed -- wrong state", EventLogEntryType.Error);
                        throw new InvalidOperationException(SyncExceptions.WrongState);
                    }
                    break;

                case SyncDbState.Session:
                    if (!((_state == SyncDbState.Opened) || (_state == SyncDbState.Syncing)))
                    {
                        EventLog.WriteEntry("Sage SyncDatabase", "SetState  !((_state == SyncDbState.Opened) || (_state == SyncDbState.Syncing) -- wrong state", EventLogEntryType.Error);
                        throw new InvalidOperationException(SyncExceptions.WrongState);
                    }
                    _mode = SyncMode.Session;
                    break;

                case SyncDbState.Syncing:
                    if (_state != SyncDbState.Session)
                    {
                        EventLog.WriteEntry("Sage SyncDatabase", "SetState state syncing -- wrong state", EventLogEntryType.Error);
                        throw new InvalidOperationException(SyncExceptions.WrongState);
                    }
                    if (_unsavedSync)
                    {
                        EventLog.WriteEntry("Sage SyncDatabase", "SetState state unsaved -- wrong state", EventLogEntryType.Error);
                        throw new InvalidOperationException(SyncExceptions.UnsavedSync);
                    }
                    break;
            }

            _state = newState;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tenantId">The tenant id which defines the database naming convention.</param>
        /// <param name="dataPath">The data path where the tenant sync metadata will be stored.</param>
        /// <remarks>
        /// The specified path will be tested for write ability, and an exception will be thrown if this location
        /// cannot be written to.
        /// </remarks>
        public SyncDatabase(string tenantId, string dataPath)
        {
            if (String.IsNullOrEmpty(tenantId))
            {
                EventLog.WriteEntry("Sage SyncDatabase", "SyncDatabase constructor null tenantid", EventLogEntryType.Error);
                throw new ArgumentNullException("tenantId");
            }
            if (String.IsNullOrEmpty(dataPath))
            {
                EventLog.WriteEntry("Sage SyncDatabase", "SyncDatabase constructor null dataPath", EventLogEntryType.Error);
                throw new ArgumentNullException("dataPath");
            }

            _tenantId = tenantId;
            _dataPath = dataPath;

            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }

            using (File.Create(Path.Combine(_dataPath, _tenantId), 1, FileOptions.DeleteOnClose)) { }
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor for SyncDatabase
        /// </summary>
        ~SyncDatabase()
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
                if (_metaRecords != null)
                {
                    _metaRecords.Dispose();
                    _metaRecords = null;
                }
                try
                {
                    if (_state == SyncDbState.Syncing)
                    {
                        EndSync(false);
                    }
                    if (_state == SyncDbState.Session)
                    {
                        EndSession();
                    }
                }
                finally
                {
                    _connection.Dispose();
                    _connection = null;
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
        /// Opens or creates a new Sql CE database to store tenant metadata.
        /// </summary>
        public void OpenDatabase()
        {
            if (_state != SyncDbState.Closed)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "OpenDatabase state != closed -- wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            var existing = File.Exists(GetDatabaseName());

            if (!existing)
            {
                using (var engine = new SqlCeEngine(GetConnectionString()))
                {
                    engine.CreateDatabase();
                }
            }

            _connection.ConnectionString = GetConnectionString();

            try
            {
                try
                {
                    _connection.Open();
                }
                catch (SqlCeException ceException)
                {
                    if (ceException.NativeError == 25017)
                    {
                        using (var engine = new SqlCeEngine(GetConnectionString()))
                        {
                            engine.Repair(GetConnectionString(), RepairOption.DeleteCorruptedRows);
                        }
                        _connection.Open();
                    }
                }

                if (!existing)
                {
                    _connection.Execute(SyncDdl.CreateDigestTable);
                }

                SetState(SyncDbState.Opened);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "OpenDatabase " + ex.Message, EventLogEntryType.Error);
                CloseDatabase();
                throw;
            }
        }

        /// <summary>
        /// Closes the tenant database.
        /// </summary>
        public void CloseDatabase()
        {
            if ((_state == SyncDbState.Syncing) || (_state == SyncDbState.Session))
            {
                EventLog.WriteEntry("Sage SyncDatabase", "CloseDatabase (_state == SyncDbState.Syncing) || (_state == SyncDbState.Session) -- wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            SetState(SyncDbState.Closed);
        }

        /// <summary>
        /// Sets the ResourceKind scope and sets the current tick and cloud tick count. 
        /// </summary>
        /// <param name="resourceKind">The ResourceKind that determines which metadata table will be used.</param>
        /// <param name="cloudTick">The current cloud tick value.</param>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public void BeginSession(string resourceKind, int cloudTick)
        {
            if (_state != SyncDbState.Opened)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "BeginSession wrong state", EventLogEntryType.Error);

                throw new InvalidOperationException(SyncExceptions.WrongState);
            }
            if (cloudTick < 0)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "BeginSession cloudTick < 0 -- tick range error", EventLogEntryType.Error);
                throw new ArgumentOutOfRangeException(SyncExceptions.RangeError);
            }

            if (String.IsNullOrEmpty(resourceKind))
            {
                EventLog.WriteEntry("Sage SyncDatabase", "BeginSession null or empty resource kind", EventLogEntryType.Error);
                throw new ArgumentNullException("resourceKind");
            }

            _resourceKind = resourceKind;
            _cloudTick = cloudTick;

            using (var command = new SqlCeCommand(SyncDml.SelectDigestRecord, _connection))
            {
                var existed = false;

                command.Parameters.Add(SqlParameters.ResourceKind, SqlDbType.NVarChar).Value = _resourceKind;
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        existed = true;

                        _tick = reader.GetInt32(1);
                        if (_tick < _cloudTick)
                        {
                            _tick = cloudTick;
                        }
                    }
                }

                if (!existed)
                {
                    _tick = cloudTick;

                    command.Parameters.Clear();
                    command.CommandText = SyncDml.InsertDigestRecord;
                    command.Parameters.Add(SqlParameters.ResourceKind, SqlDbType.NVarChar).Value = _resourceKind;
                    command.Parameters.Add(SqlParameters.Tick, SqlDbType.Int).Value = _tick;
                    command.ExecuteNonQuery();
                }
            }

            if (!_connection.TableExists(GetMetatableName(_resourceKind)))
            {
                _connection.Execute(String.Format(SyncDdl.CreateMetaTable, GetMetatableName(resourceKind)));
            }

            _metaRecords = new DataTable(GetMetatableName(resourceKind));

            using (var command = new SqlCeCommand(String.Format(SyncDml.ReadSchema, GetMetatableName(resourceKind)), _connection))
            {
                using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    _metaRecords.Load(reader);
                }
                _metaRecords.Columns[SyncFields.Id].ReadOnly = true;
                _metaRecords.Columns[SyncFields.Id].Unique = false;
            }

            SetState(SyncDbState.Session);
        }

        /// <summary>
        /// Ends the session on the ResourceKind scope.
        /// </summary>
        public void EndSession()
        {
            if (_state != SyncDbState.Session)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "EndSession _state != SyncDbState.Session -- wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            SetState(SyncDbState.Opened);
        }

        /// <summary>
        /// Starts the synchronization process.
        /// </summary>
        /// <param name="syncType">Specifies the desired client mode of operation. Internal = full sync, External = only changes from ERP.</param>
        /// <param name="trackDeletes">Determines if the sync will return a list of deleted items.</param>
        /// <returns>
        /// SyncMode.FullSync if all model objects are expected to be processed. If syncType was External, and the database is in a state
        /// to process changed records, then the return value will be SyncMode.ChangeSync. For this mode, only changed records are expected.
        /// </returns>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public SyncMode BeginSync(SyncType syncType, bool trackDeletes = true)
        {
            if (_state != SyncDbState.Session)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "BeginSync_state != SyncDbState.Session -- wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            _trackDeletes = trackDeletes;

            bool hasUnsynced = false;
            bool hasRows;

            using (var command = new SqlCeCommand(SyncDml.UpdateDigestRecord, _connection))
            {
                _tick++;

                command.Parameters.Add(SqlParameters.Tick, SqlDbType.Int).Value = _tick;
                command.Parameters.Add(SqlParameters.ResourceKind, SqlDbType.NVarChar).Value = _resourceKind;
                command.ExecuteNonQuery();

                command.Parameters.Clear();

                command.CommandText = String.Format(SyncDml.ReadMetadataCount, GetMetatableName(_resourceKind));
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    hasRows = reader.Read();
                }

                if (hasRows)
                {
                    command.CommandText = String.Format(SyncDml.ReadUnsyncedCount, GetMetatableName(_resourceKind), _cloudTick);
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        hasUnsynced = reader.Read();
                    }
                }
            }

            SetState(SyncDbState.Syncing);

            if ((syncType == SyncType.Internal) || !hasRows || hasUnsynced)
            {
                if (hasRows)
                {
                    if (hasUnsynced)
                    {
                        _connection.Execute(String.Format(SyncDml.ClearUnsynced, GetMetatableName(_resourceKind), _cloudTick));
                    }
                    _connection.Execute(String.Format(SyncDml.ClearAllActive, GetMetatableName(_resourceKind)));
                }

                _mode = SyncMode.FullSync;
            }
            else
            {
                _connection.Execute(String.Format(SyncDml.SetAllActive, GetMetatableName(_resourceKind)));

                _mode = SyncMode.ChangeSync;
            }

            return _mode;
        }

        /// <summary>
        /// Finalizes the sync process and returns the database to the opened state.
        /// </summary>
        /// <param name="syncCompleted">True if the Sync has completed processing the plugin data rows and processing was not stopped in the middle of synchronization.</param>
        /// <returns>A collection of resource kind and external id values that have were marked as deleted during the sync process.</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public IDictionary<int, ISyncDeleted> EndSync(bool syncCompleted)
        {
            if (_state != SyncDbState.Syncing)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "EndSync _state != SyncDbState.Syncing -- wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            _unsavedSync = true;

            var deleted = new Dictionary<int, ISyncDeleted>();

            if (!syncCompleted)
            {
                PurgeDeleted();
                SetState(SyncDbState.Session);
                return deleted;
            }

            using (var table = new DataTable(GetMetatableName(_resourceKind)))
            {
                using (var command = new SqlCeCommand(String.Format(SyncDml.ReadDeleted, GetMetatableName(_resourceKind)), _connection))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        bool hasDeleted;

                        if (_trackDeletes)
                        {
                            table.Load(reader);
                            hasDeleted = (table.Rows.Count > 0);
                        }
                        else
                        {
                            hasDeleted = reader.Read();
                        }

                        if (hasDeleted)
                        {
                            command.CommandText = String.Format(SyncDml.UpdateDeleted, GetMetatableName(_resourceKind), _tick);
                            command.ExecuteNonQuery();
                        }
                    }

                    if (_trackDeletes)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            var isRoot = row.IsNull(SyncFields.ResourceKind);
                            var resourceKind = (isRoot ? _resourceKind : row[SyncFields.ResourceKind].ToString());
                            var externalId = (isRoot ? SyncConvert.Decode(row[SyncFields.ExternalId].ToString()) : SyncConvert.Decode(row[SyncFields.ExternalChildId].ToString()));
                            var deletedEntity = new SyncDeleted(resourceKind, externalId, isRoot);

                            deleted.Add((int)row[SyncFields.Id], deletedEntity);
                        }
                    }
                }
            }

            PurgeDeleted();

            SetState(SyncDbState.Session);

            return deleted;
        }

        /// <summary>
        /// Finds the metadata record for the specified syncObject entity. 
        /// </summary>
        /// <param name="syncObject">The sync object to locate the record for.</param>
        /// <param name="readFlags">If AutoAdd is specified, a new in-memory record will be added on match failure.</param>
        /// <returns>
        /// The existing (or newly added) data row for the located entity. Null will be returned if the record did not
        /// exist and the AutoAdd flag was not specified.
        /// </returns>
        public DataRow FindMetaRecord(ISyncObject syncObject, SyncReadFlags readFlags)
        {
            if (syncObject == null)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "FindMetaRecord syncObject == null", EventLogEntryType.Error);
                throw new ArgumentNullException("syncObject");
            }
            if (!IsResourceState())
            {
                EventLog.WriteEntry("Sage SyncDatabase", "FindMetaRecord !IsResourceState -- wrong state", EventLogEntryType.Error);

                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            var keys = new[] 
            {
                SyncConvert.Encode(syncObject.ExternalId), SyncConvert.Encode(syncObject.ExternalChildId), syncObject.ResourceKind
            };

            foreach (DataRow row in _metaRecords.Rows)
            {
                if (CompareColumnValue(row, SyncFields.ResourceKind, keys[2]))
                {
                    if (CompareColumnValue(row, SyncFields.ExternalChildId, keys[1]))
                    {
                        if (CompareColumnValue(row, SyncFields.ExternalId, keys[0]))
                        {
                            return row;
                        }
                    }
                }
            }

            return ((readFlags & SyncReadFlags.AutoAdd) == SyncReadFlags.AutoAdd) ? NewMetaRecord(syncObject) : null;
        }

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
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public DataRow ReadMetaRecord(ISyncObject syncObject, SyncReadFlags readFlags)
        {
            if (syncObject == null)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "ReadMetaRecord null sync object", EventLogEntryType.Error);
                throw new ArgumentNullException("syncObject");
            }
            if (!IsResourceState())
            {
                EventLog.WriteEntry("Sage SyncDatabase", "ReadMetaRecord wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            if (_mode == SyncMode.ChangeSync)
            {
                _connection.Execute(String.Format(SyncDml.ClearRootActive, GetMetatableName(_resourceKind), GenerateCondition(SyncConvert.Encode(syncObject.ExternalId))));
            }

            var key = SyncConvert.Encode(syncObject.ExternalId);

            _metaRecords.Clear();

            using (var command = new SqlCeCommand(String.Format(SyncDml.ReadMetadataRecords, GetMetatableName(_resourceKind), key), _connection))
            {
                using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                {
                    _metaRecords.Load(reader);
                }
            }

            if (_metaRecords.Rows.Count == 0)
            {
                return ((readFlags & SyncReadFlags.AutoAdd) == SyncReadFlags.AutoAdd) ? NewMetaRecord(syncObject) : null;
            }

            return FindMetaRecord(syncObject, readFlags);
        }

        /// <summary>
        /// Allows the sync client to revert the tick count if no changes were detected during sync processing.
        /// </summary>
        public void RevertDigestTick()
        {
            if ((_state != SyncDbState.Session) || !_unsavedSync)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "RemoveMetaRecords (_state != SyncDbState.Session) || !_unsavedSync) --wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            using (var command = new SqlCeCommand(SyncDml.UpdateDigestRecord, _connection))
            {
                _tick--;

                command.Parameters.Add(SqlParameters.Tick, SqlDbType.Int).Value = _tick;
                command.Parameters.Add(SqlParameters.ResourceKind, SqlDbType.NVarChar).Value = _resourceKind;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Writes the current metadata records to the SQL table based on each row's RowState value. For unchanged rows, nothing is 
        /// done. For added rows, an INSERT is performed. And for changed rows, an UPDATE is performed.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public void WriteMetaRecords()
        {
            if (!IsResourceState())
            {
                EventLog.WriteEntry("Sage SyncDatabase", "WriteMetaRecords !IsResourceState() -- wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            if (_metaRecords.Rows.Count == 0) return;

            using (var command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.TableDirect;
                command.IndexName = String.Format(SyncFormats.PrimaryKey, _resourceKind);
                command.CommandText = GetMetatableName(_resourceKind);

                using (var recordset = command.ExecuteResultSet(ResultSetOptions.Scrollable | ResultSetOptions.Updatable))
                {
                    foreach (DataRow row in _metaRecords.Rows)
                    {
                        UpdateMetaRecord(recordset, row);
                    }
                }
            }
        }

        /// <summary>
        /// For external change detection, the ERP plugin won't be able to build an entity model object in order to represent a delete. This
        /// method allows for an entity (or child entity) to marked as Active=false, so that it will be picked up as deleted when
        /// the EndSync() method is called.
        /// </summary>
        /// <param name="externalId">The external id of the root entity to locate. Cannot be blank or null.</param>
        /// <param name="resourceKind">For child entities, the resource kind that represents the entity, eg: Addresses. Null for root entities.</param>
        /// <param name="externalChildId">For child entities, the external id of the entity. Null for root entities.</param>
        /// <returns>True if one or more records were marked as inactive.</returns>
        public bool RemoveMetaRecords(string externalId, string resourceKind = null, string externalChildId = null)
        {
            if (_state != SyncDbState.Syncing)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "RemoveMetaRecords _state != SyncDbState.Syncing -- wrong state", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongState);
            }

            if (_mode != SyncMode.ChangeSync)
            {
                EventLog.WriteEntry("Sage SyncDatabase", "RemoveMetaRecords wrong mode", EventLogEntryType.Error);
                throw new InvalidOperationException(SyncExceptions.WrongMode);
            }

            var keyId = SyncConvert.Encode(externalId);
            string statement;

            if (String.IsNullOrEmpty(resourceKind) || String.IsNullOrEmpty(externalChildId))
            {
                statement = string.Format(SyncDml.ClearRootActive, GetMetatableName(_resourceKind), GenerateCondition(keyId));
            }
            else
            {
                statement = string.Format(SyncDml.ClearChildActive, GetMetatableName(_resourceKind), GenerateCondition(keyId), GenerateCondition(resourceKind), GenerateCondition(SyncConvert.Encode(externalChildId)));
            }

            return (_connection.Execute(statement) > 0);
        }

        /// <summary>
        /// Creates a hashset that contains all the external id values that are being tracked in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resource kind to get the keys for.</param>
        /// <returns>A hashset that contains all the external id values for the specified resource.</returns>
        public HashSet<string> GetResourceKeys(string resourceKind)
        {
            return ReadResourceKeys(resourceKind, true);
        }

        /// <summary>
        /// Creates a hashset that contains all the external id values that are being tracked and marked as active in the metadata.
        /// </summary>
        /// <param name="resourceKind">The resource kind to get the keys for.</param>
        /// <returns>A hashset that contains all the external id values for the specified resource kind.</returns>
        public HashSet<string> GetActiveResourceKeys(string resourceKind)
        {
            return ReadResourceKeys(resourceKind, false);
        }

        /// <summary>
        /// Helper function to return the absolute file name of the database.
        /// </summary>
        /// <param name="dataPath">The file path to use for building the database path.</param>
        /// <param name="tenantId">The tenant id to use for building the database path.</param>
        /// <returns>The absolute filename for the CE database.</returns>
        public static string GetDatabaseFilename(string dataPath, string tenantId)
        {
            return Path.Combine(dataPath, String.Format(SyncFormats.Database, tenantId));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the current state of the database,
        /// </summary>
        public SyncDbState State
        {
            get
            {
                return _state;
            }
        }

        /// <summary>
        /// Returns the current SQL CE connection being used by the database.
        /// </summary>
        public SqlCeConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// Returns the sync mode that the database is currently in.
        /// </summary>
        public SyncMode Mode
        {
            get
            {
                return _mode;
            }
        }

        /// <summary>
        /// If State is FullSync or ChangeSync, this will be the updated local tick count for the sync process, otherwise
        /// this will return (-1).
        /// </summary>
        public int Tick
        {
            get
            {
                return _tick;
            }
        }

        /// <summary>
        /// Returns the cloud tick count passed in during the call to BeginSync
        /// </summary>
        public int CloudTick
        {
            get
            {
                return _cloudTick;
            }
        }

        /// <summary>
        /// The ResourceKind scope as set by the BeginResourceTrans method.
        /// </summary>
        public string ResourceKind
        {
            get
            {
                return _resourceKind;
            }
        }

        /// <summary>
        /// The tenant id that the metadata is stored for. This is used to define the actual database name.
        /// </summary>
        public string TenantId
        {
            get
            {
                return _tenantId;
            }
        }

        /// <summary>
        /// The DataTable that contains records filled from a call to ReadMetaRecords or ReadMetaRecord. 
        /// </summary>
        public DataTable MetaRecords
        {
            get
            {
                return _metaRecords;
            }
        }

        #endregion
    }
}
