using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace Sage.Connector.DomainMediator.Core
{
    /// <summary>
    /// Storage mode options.
    /// </summary>
    public enum StorageMode
    {
        /// <summary>
        /// Opens the database dictionary in read only mode. No changes can be made, but external changes 
        /// made to the database will be reflected.
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Opens the database dictionary in read write mode. Changes can be made, and external changes 
        /// made to the database will be reflected.
        /// </summary>
        ReadWrite
    }

    /// <summary>
    /// Class to manage a database backed implementation of IDictionary(TKey, TValue)
    /// </summary>
    public sealed class StorageDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
    {
        #region Private constants

        private const int KeySize = 512;
        private const string MutexFormat = "Global\\{0}.SD";
        private const string DatabaseFormat = "{0}.SD.sdf";
        private const string ReadOnlyParam = "File Mode=Read Only;";
        private const string ConnectionFormat = "Data Source='{0}';LCID=1033;Flush Interval=1;Max Buffer Size=1024;Max Database Size=1024;";
        private const string DeleteStmt = "DELETE FROM NameValue;";
        private const string CountStmt = "SELECT COUNT(Name) FROM [NameValue];";
        private const string CreateTableStmt = @"CREATE TABLE [NameValue] (
                                                        [Name] nvarchar(512) NOT NULL,
                                                        [Value] ntext NULL
                                                    );
                                                    ALTER TABLE [NameValue] ADD CONSTRAINT [PK_NameValue] PRIMARY KEY ([Name]);";

        #endregion

        #region Private fields

        private SqlCeConnection _connection = new SqlCeConnection();
        private readonly JsonSerializerSettings _settings;
        private readonly StorageMode _mode;
        private bool _disposed;
        private Mutex _lock;

        #endregion

        #region Private methods

        /// <summary>
        /// Aquire exclusive access through the use of the global mutex.
        /// </summary>
        private void Lock()
        {
            try
            {
                _lock.WaitOne(Timeout.Infinite);
            }
            catch (AbandonedMutexException)
            {
                /* We have aquired access */
            }
        }

        /// <summary>
        /// Release exclusive access to the global mutex.
        /// </summary>
        private void Unlock()
        {
            _lock.ReleaseMutex();
        }

        /// <summary>
        /// Throws an exception if the instance has been disposed.
        /// </summary>
        private void ThrowOnDisposed()
        {
            if (_disposed) throw new ObjectDisposedException("Connection");
        }

        /// <summary>
        /// Throws an exception if the instance is in read only mode.
        /// </summary>
        private void ThrowOnReadOnly()
        {
            if (_mode != StorageMode.ReadWrite) throw new ReadOnlyException();
        }

        /// <summary>
        /// Converts the key type to a serialized string, and verifies that the length is within the
        /// allowed limits of the database field.
        /// </summary>
        /// <param name="key">The key value to convert</param>
        /// <returns>The serialized value from the key.</returns>
        private string ConvertKey(TKey key)
        {
            var result = JsonConvert.SerializeObject(key, _settings);

            if (result.Length > KeySize)
            {
                throw new ArgumentOutOfRangeException("key", key, "Key must serialize to less than 512 bytes.");
            }

            return result;
        }

        /// <summary>
        /// Performs application defined tasks associated with freeing and releasing.
        /// </summary>
        /// <param name="disposing">True if we should be disposing of managed items, otherwise false.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            try
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }

                    if (_lock != null)
                    {
                        _lock.Dispose();
                        _lock = null;
                    }
                }
            }
            finally
            {
                _disposed = true;
            }
        }

        /// <summary>
        /// Allows direct execution of sql DDL and DML statements, such as INSERT/UPDATE/DELETE. 
        /// This is needed for database and table generation, as well as batch updates where single record 
        /// processing does not make sense.
        /// </summary>
        /// <param name="commandText">The sql statement to process.</param>
        /// <param name="throwErrors">True if exceptions should be thrown, otherwise false.</param>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        private void Execute(string commandText, bool throwErrors = true)
        {
            ThrowOnDisposed();

            if (String.IsNullOrEmpty(commandText)) return;

            Lock();

            try
            {
                var commands = commandText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                using (var command = new SqlCeCommand(commandText.Trim(), _connection))
                {
                    command.CommandTimeout = 0;

                    foreach (var line in commands)
                    {
                        command.CommandText = line + ";";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                if (throwErrors) throw;
            }
            finally
            {
                Unlock();
            }
        }

        /// <summary>
        /// Opens or creates the desired database file.
        /// </summary>
        /// <param name="databaseFile">The name of the SQL compact database file.</param>
        private void OpenDatabase(string databaseFile)
        {
            var connectionString = String.Format(ConnectionFormat, databaseFile);
            var existing = File.Exists(databaseFile);

            if (!existing)
            {
                using (var engine = new SqlCeEngine(connectionString))
                {
                    engine.CreateDatabase();
                }
            }

            Lock();

            try
            {
                _connection.ConnectionString = connectionString;

                try
                {
                    _connection.Open();
                }
                catch (SqlCeException ceException)
                {
                    if (ceException.NativeError == 25017)
                    {
                        using (var engine = new SqlCeEngine(connectionString))
                        {
                            engine.Repair(connectionString, RepairOption.DeleteCorruptedRows);
                        }
                        _connection.Open();
                    }
                }

                if (!existing)
                {
                    Execute(CreateTableStmt);
                }
            }
            catch (Exception)
            {
                _connection.Close();
                throw;
            }
            finally
            {
                Unlock();
            }
        }


        /// <summary>
        /// Gets an initialized SQL compact command object.
        /// </summary>
        /// <returns>The command object.</returns>
        private SqlCeCommand GetCommand()
        {
            var cmd = _connection.CreateCommand();

            cmd.CommandType = CommandType.TableDirect;
            cmd.IndexName = "PK_NameValue";
            cmd.CommandText = "NameValue";

            return cmd;
        }

        /// <summary>
        /// Creates a snapshot of the column data for all rows and returns the resulting collection.
        /// </summary>
        /// <typeparam name="T">The type for the items that will be returned.</typeparam>
        /// <param name="index">The column index for the data to be returned.</param>
        /// <returns>The collection of T items from the requested column.</returns>
        private ICollection<T> GetColumnSnapshot<T>(int index)
        {
            ThrowOnDisposed();

            var result = new List<T>();

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (resultSet.ReadFirst())
                        {
                            do
                            {
                                result.Add(JsonConvert.DeserializeObject<T>(resultSet.GetString(index) ?? String.Empty));

                            } while (resultSet.Read());
                        }
                    }
                }
            }
            finally
            {
                Unlock();
            }

            return result;
        }

        /// <summary>
        /// Creates a snapshot of the database by loading all the key value pairs into a dictionary.
        /// </summary>
        /// <returns>A dictionary representation of the data table.</returns>
        private IDictionary<TKey, TValue> GetSnapshot()
        {
            ThrowOnDisposed();

            var result = new Dictionary<TKey, TValue>();

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (resultSet.ReadFirst())
                        {
                            do
                            {
                                var k = JsonConvert.DeserializeObject<TKey>(resultSet.GetString(0) ?? String.Empty);
                                var v = JsonConvert.DeserializeObject<TValue>(resultSet.GetString(1) ?? String.Empty);

                                result.Add(k, v);

                            } while (resultSet.Read());
                        }
                    }
                }
            }
            finally
            {
                Unlock();
            }

            return result;
        }

        #endregion

        #region Constructors and destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path to open or create the database dictionary in.</param>
        /// <param name="identifier">The identifier for the desired database dictionary.</param>
        public StorageDictionary(string path, string identifier)
            : this(path, identifier, StorageMode.ReadWrite)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path to open or create the database dictionary in.</param>
        /// <param name="identifier">The identifier for the desired database dictionary.</param>
        /// <param name="mode">The desired mode for the database dictionary.</param>
        public StorageDictionary(string path, string identifier, StorageMode mode)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (String.IsNullOrEmpty(identifier)) throw new ArgumentNullException("identifier");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            _lock = new Mutex(false, String.Format(MutexFormat, identifier));
            _settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            _mode = mode;

            OpenDatabase(Path.Combine(path, String.Format(DatabaseFormat, identifier)));
        }

        /// <summary>
        /// Remove the Storage 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="identifier"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RemoveStorage(string path, string identifier)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (String.IsNullOrEmpty(identifier)) throw new ArgumentNullException("identifier");
            if (!Directory.Exists(path)) return;
            string filename = Path.Combine(path, String.Format(DatabaseFormat, identifier));
            if (!File.Exists(filename)) return;
            //can't lock because lock is a mutex and not static. 
            File.Delete(filename);
        }


       
        /// <summary>
        /// Destructor
        /// </summary>
        ~StorageDictionary()
        {
            /* Finalizer is being called because we were never properly disposed; need to do it now */
            Dispose(true);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Performs application defined tasks associated with freeing and releasing.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return GetSnapshot().GetEnumerator();
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="item">The key value pair to add.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ThrowOnDisposed();
            ThrowOnReadOnly();

            var k = ConvertKey(item.Key);

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable | ResultSetOptions.Updatable))
                    {
                        if (resultSet.Seek(DbSeekOptions.FirstEqual, new object[] { k }))
                        {
                            resultSet.Read();
                            resultSet.SetValue(1, JsonConvert.SerializeObject(item.Value, _settings));
                            resultSet.Update();

                            return;
                        }

                        var record = resultSet.CreateRecord();

                        record.SetValue(0, k);
                        record.SetValue(1, JsonConvert.SerializeObject(item.Value, _settings));

                        resultSet.Insert(record, DbInsertOptions.PositionOnInsertedRow);
                    }
                }
            }
            finally
            {
                Unlock();
            }
        }

        /// <summary>
        /// Removes all keys value from the database dictionary.
        /// </summary>
        public void Clear()
        {
            ThrowOnReadOnly();

            Execute(DeleteStmt);
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific key and value.
        /// </summary>
        /// <param name="item">The key value pair to locate.</param>
        /// <returns>True if the key value pair is located, otherwise false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            ThrowOnDisposed();

            var k = ConvertKey(item.Key);

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (resultSet.Seek(DbSeekOptions.FirstEqual, new object[] { k }) && resultSet.Read())
                        {
                            return String.Equals(resultSet.GetString(1), JsonConvert.SerializeObject(item.Value, _settings), StringComparison.Ordinal);
                        }
                    }
                }
            }
            finally
            {
                Unlock();
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the collection to an array of key value pairs, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one dimensional array of key value pairs.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            GetSnapshot().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a key and value from the dictionary.
        /// </summary>
        /// <param name="item">The key value pair to locate.</param>
        /// <returns>True if the key value pair was found and removed, otherwise false.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowOnDisposed();
            ThrowOnReadOnly();

            var k = ConvertKey(item.Key);

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable | ResultSetOptions.Updatable))
                    {
                        if (resultSet.Seek(DbSeekOptions.FirstEqual, new object[] { k }) && resultSet.Read())
                        {
                            if (String.Equals(resultSet.GetString(1), JsonConvert.SerializeObject(item.Value, _settings), StringComparison.Ordinal))
                            {
                                resultSet.Delete();

                                return true;
                            }
                        }
                    }
                }
            }
            finally
            {
                Unlock();
            }

            return false;
        }

        /// <summary>
        /// Determines whether the database dictionary contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>True if the key is located, otherwise false.</returns>
        public bool ContainsKey(TKey key)
        {
            ThrowOnDisposed();

            var k = ConvertKey(key);

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        return resultSet.Seek(DbSeekOptions.FirstEqual, new object[] { k });
                    }
                }
            }
            finally
            {
                Unlock();
            }
        }

        /// <summary>
        /// Adds a new key value pair to the dictionary's underlying data store.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The associated value for the pair.</param>
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Removes the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>True if the element is successfully found and removed, otherwise false.</returns>
        public bool Remove(TKey key)
        {
            ThrowOnDisposed();
            ThrowOnReadOnly();

            var k = ConvertKey(key);

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable | ResultSetOptions.Updatable))
                    {
                        if (resultSet.Seek(DbSeekOptions.FirstEqual, new object[] { k }) && resultSet.Read())
                        {
                            resultSet.Delete();

                            return true;
                        }
                    }
                }
            }
            finally
            {
                _lock.ReleaseMutex();
            }

            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">On return contains the value associated with the specified key (if found).</param>
        /// <returns>True if the dictionary contains an element with the specified key, otherwise false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            ThrowOnDisposed();

            var k = ConvertKey(key);

            Lock();

            try
            {
                using (var cmd = GetCommand())
                {
                    using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (resultSet.Seek(DbSeekOptions.FirstEqual, new object[] { k }) && resultSet.Read())
                        {
                            value = JsonConvert.DeserializeObject<TValue>(resultSet.GetString(1) ?? String.Empty);

                            return true;
                        }
                    }
                }
            }
            finally
            {
                _lock.ReleaseMutex();
            }

            value = default(TValue);

            return false;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the count of key value pairs in the dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                ThrowOnDisposed();

                Lock();

                try
                {
                    using (var cmd = new SqlCeCommand(CountStmt, _connection))
                    {
                        using (var resultSet = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                        {
                            return (resultSet.Read() ? resultSet.GetInt32(0) : (-1));
                        }
                    }
                }
                finally
                {
                    _lock.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// True if the dictionary is read only, false if read / write.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (_mode != StorageMode.ReadWrite);
            }
        }

        /// <summary>
        /// Indexer property allowing read/write access to the key value pairs.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>The associated string  value if the key is found, otherwise a default(string).</returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue v;

                return (TryGetValue(key, out v) ? v : default(TValue));
            }

            set
            {
                Add(key, value);
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                return GetColumnSnapshot<TKey>(0);
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the dictionary.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                return GetColumnSnapshot<TValue>(1);
            }
        }

        #endregion

        /// <summary>
        /// Determines storage dictionary existence. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Exists(string path, string identifier)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (String.IsNullOrEmpty(identifier)) throw new ArgumentNullException("identifier");
            if (!Directory.Exists(path)) return false;
            string filename = Path.Combine(path, String.Format(DatabaseFormat, identifier));
            if (!File.Exists(filename)) return false;
            return true;
        }
    }
}
