using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sage.Connector.Sync.Common
{
    /// <summary>
    /// Constant format strings used by the sync engine.
    /// </summary>
    public static class SyncFormats
    {
        /// <summary>
        /// Primary key format.
        /// </summary>
        public static string PrimaryKey = "PK_Sync{0}";

        /// <summary>
        /// Table name format.
        /// </summary>
        public static string Metatable = "Sync{0}";

        /// <summary>
        /// Resource kind format.
        /// </summary>
        public static string Resource = "{0}/{1}";
        
        /// <summary>
        /// Guid format.
        /// </summary>
        public static string Guid = "D";

        /// <summary>
        /// SQL CE connection format.
        /// </summary>
        public static string Connection = "Data Source='{0}'; LCID=1033;";

        /// <summary>
        /// Database filename format.
        /// </summary>
        public static string Database = "{0}.sdf";

        /// <summary>
        /// SQL CE identifier format.
        /// </summary>
        public static string Identifier = "[{0}]";
    }

    /// <summary>
    /// Constant class for maintaining integer values used by the sync engine.
    /// </summary>
    public static class SyncValues
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        public static int Invalid = (-1);

        /// <summary>
        /// Key column size.
        /// </summary>
        public static int KeyColSize = 64;

        /// <summary>
        /// Purge check value.
        /// </summary>
        public static int PurgeCheck = 1024;


        /// <summary>
        /// Purge delta value.
        /// </summary>
        public static int PurgeDelta = 512;
    }

    /// <summary>
    /// Constant parameter values that are used for the parameterized SQL execution.
    /// </summary>
    public static class SqlParameters
    {
        /// <summary>
        /// Tablename parameter.
        /// </summary>
        public const string TableName = "@TableName";

        /// <summary>
        /// ResourceKind parameter.
        /// </summary>
        public const string ResourceKind = "@ResourceKind";

        /// <summary>
        /// Tick parameter.
        /// </summary>
        public const string Tick = "@Tick";

        /// <summary>
        /// Id parameter.
        /// </summary>
        public const string Id = "@Id";
    }

    /// <summary>
    /// Constant syntax characters that are used in the SQL statement generation.
    /// </summary>
    public static class SqlSyntax
    {
        /// <summary>
        /// Quote character.
        /// </summary>
        public const char Quote = '\'';
        
        /// <summary>
        /// Statement terminating character.
        /// </summary>
        public const char Term = ';';
        
        /// <summary>
        /// Assignment operator.
        /// </summary>
        public static string EqualsOp = " = ";
    }

    /// <summary>
    /// Constant field names that are used by the SyncMetaTable.
    /// </summary>
    public static class SyncFields
    {
        /// <summary>
        /// Id field name.
        /// </summary>
        public const string Id = "Id";

        /// <summary>
        /// ResourceKind field name.
        /// </summary>
        public const string ResourceKind = "ResourceKind";

        /// <summary>
        /// ExternalId field name.
        /// </summary>
        public const string ExternalId = "ExternalId";

        /// <summary>
        /// ExternalChildId field name.
        /// </summary>
        public const string ExternalChildId = "ExternalChildId";

        /// <summary>
        /// HashKey field name.
        /// </summary>
        public const string HashKey = "HashKey";

        /// <summary>
        /// EndpointId field name.
        /// </summary>
        public const string EndpointId = "EndpointId";

        /// <summary>
        /// EndpointTick field name.
        /// </summary>
        public const string EndpointTick = "EndpointTick";

        /// <summary>
        /// Deleted field name.
        /// </summary>
        public const string Deleted = "Deleted";

        /// <summary>
        /// Active field name.
        /// </summary>
        public const string Active = "Active";
    }

    /// <summary>
    /// Constant field indexes that are used by the SyncMetaTable.
    /// </summary>
    public static class SyncFieldIndexes
    {
        /// <summary>
        /// Id field index.
        /// </summary>
        public static int Id = 0;

        /// <summary>
        /// ResourceKind field index.
        /// </summary>
        public static int ResourceKind = 1;

        /// <summary>
        /// ExternalId field index.
        /// </summary>
        public static int ExternalId = 2;

        /// <summary>
        /// ExternalChildId field index.
        /// </summary>
        public static int ExternalChildId = 3;

        /// <summary>
        /// HashKey field index.
        /// </summary>
        public static int HashKey = 4;
        
        /// <summary>
        /// EndpointId field index.
        /// </summary>
        public static int EndpointId = 5;

        /// <summary>
        /// EndpointTick field index.
        /// </summary>
        public static int EndpointTick = 6;

        /// <summary>
        /// Deleted field index.
        /// </summary>
        public static int Deleted = 7;

        /// <summary>
        /// Active field index.
        /// </summary>
        public static int Active = 8;
    }

    /// <summary>
    /// Constant exception messages returned from the Sync framework.
    /// </summary>
    public static class SyncExceptions
    {
        /// <summary>
        /// Message for resource kind exceeding max length.
        /// </summary>
        public static string LengthResourceKind = "The ResourceKind ({0}) must be less than or equal to 64 characters.";

        /// <summary>
        /// Message for sync being started before previous sync has been ended with EndSession.
        /// </summary>
        public static string UnsavedSync = "The previous sync operation has not been commited by EndSession.";

        /// <summary>
        /// Message for wrong mode of operation.
        /// </summary>
        public static string WrongMode = "Operation is not valid due to the current mode of the object.";

        /// <summary>
        /// Message for wrong state of operation.
        /// </summary>
        public static string WrongState = "Operation is not valid due to the current state of the object.";

        /// <summary>
        /// Message for range error.
        /// </summary>
        public static string RangeError = "The specified value must be greater than or equal to zero.";
    }

    /// <summary>
    /// SQL DML statements used by the Sync framework.
    /// </summary>
    public sealed class SyncDml
    {
        /// <summary>
        /// IS NULL statmement.
        /// </summary>
        public const string DynamicNull = " IS NULL";

        /// <summary>
        /// Statement for reading schema.
        /// </summary>
        public const string ReadSchema = "SELECT * FROM [{0}] WHERE 1=0;";

        /// <summary>
        /// Statement for reading all externalid values.
        /// </summary>
        public const string ReadKeys = "SELECT ExternalId from [{0}] WHERE ResourceKind IS NULL;";

        /// <summary>
        /// Statement for reading all externalid values where deleted is false.
        /// </summary>
        public const string ReadActiveKeys = "SELECT ExternalId from [{0}] WHERE ([Deleted] = 0) AND (ResourceKind IS NULL);";

        /// <summary>
        /// Statement for checking if tablename exists.
        /// </summary>
        public const string TableExists = "SELECT 1 FROM Information_Schema.Tables WHERE TABLE_NAME = @TableName;";

        /// <summary>
        /// Statement for reading the digest record for a specified resource kind.
        /// </summary>
        public const string SelectDigestRecord = "SELECT [Id], [Tick] FROM [SyncDigest] WHERE ([ResourceKind] = @ResourceKind);";
        
        /// <summary>
        /// Statement for inserting a new digest record for a specific resource kind.
        /// </summary>
        public const string InsertDigestRecord = "INSERT INTO [SyncDigest] ([ResourceKind], [EndpointId], [Tick]) VALUES (@ResourceKind, 0, @Tick);";

        /// <summary>
        /// Statement for updating a digest record for a specific resource kind.
        /// </summary>
        public const string UpdateDigestRecord = "UPDATE [SyncDigest] SET [Tick]=@Tick WHERE ([ResourceKind] = @ResourceKind);";
        
        /// <summary>
        /// Statement to determine if the metatable is empty.
        /// </summary>
        public const string ReadMetadataCount = "SELECT TOP(1) Id FROM [{0}];";

        /// <summary>
        /// Statement to determine if there are records that did not get moved to the cloud.
        /// </summary>
        public const string ReadUnsyncedCount = "SELECT TOP(1) Id FROM [{0}] WHERE (EndpointTick > {1});";

        /// <summary>
        /// Statement to read sets of metarecords that match a specific externalid.
        /// </summary>
        public const string ReadMetadataRecords = "SELECT * FROM [{0}] WHERE [ExternalId] = '{1}';";

        /// <summary>
        /// Statement to purge metarecords that are marked as deleted.
        /// </summary>
        public const string PurgeDeleted = "DELETE FROM [{0}] WHERE ([Deleted] = 1) AND (EndpointTick < {1});";

        /// <summary>
        /// Statement to clear the deleted flag on any records that did not make it to the cloud.
        /// </summary>
        public const string ClearUnsynced = "UPDATE [{0}] SET [Deleted] = 0 WHERE ([EndpointTick] > {1});";

        /// <summary>
        /// Statement to set all records to inactive.
        /// </summary>
        public const string ClearAllActive = "UPDATE [{0}] SET [Active] = 0;";

        /// <summary>
        /// Statement to clear the active flag for the root record that matches the given externalid.
        /// </summary>
        public const string ClearRootActive = "UPDATE [{0}] SET [Active] = 0 WHERE ([ExternalId]{1});";

        /// <summary>
        /// Statement to clear the active flag for all records that matches the given externalid.
        /// </summary>
        public const string ClearChildActive = "UPDATE [{0}] SET [Active] = 0 WHERE ([ExternalId]{1}) AND ([ResourceKind]{2}) AND ([ExternalChildId]{3});";
        
        /// <summary>
        /// Statement to set all records to active.
        /// </summary>
        public const string SetAllActive = "UPDATE [{0}] SET [Active] = 1 WHERE [Deleted] = 0;";

        /// <summary>
        /// Statement to read all records where both active and deleted are false.
        /// </summary>
        public const string ReadDeleted = "SELECT Id, ResourceKind, ExternalId, ExternalChildId FROM [{0}] WHERE ([Active] = 0) AND ([Deleted] = 0) ORDER BY ExternalId, ResourceKind, ExternalChildId;";
        
        /// <summary>
        /// Statement to set all records to deleted where both active and deleted are false.
        /// </summary>
        public const string UpdateDeleted = "UPDATE [{0}] SET EndpointTick = {1}, Deleted = 1 WHERE ([Active] = 0) AND ([Deleted] = 0);";
    }

    /// <summary>
    /// SQL DDL statements used by the Sync framework.
    /// </summary>
    public sealed class SyncDdl
    {
        /// <summary>
        /// DDL statement for creating the SyncDigest table.
        /// </summary>
        public const string CreateDigestTable = @"
            CREATE TABLE [SyncDigest]
            (
                [Id] INT NOT NULL IDENTITY (1,1),
                [ResourceKind] NVARCHAR(64) NOT NULL,
                [EndpointId] INT NOT NULL,
                [Tick] INT NOT NULL
            );
            ALTER TABLE [SyncDigest] ADD CONSTRAINT [PK_SyncDigest] PRIMARY KEY ([Id]);
            CREATE UNIQUE INDEX [IX_SyncDigest_Id] ON [SyncDigest] ([Id] ASC);
            CREATE UNIQUE INDEX [IX_SyncDigest_ResourceKind] ON [SyncDigest] ([ResourceKind] ASC);";

        /// <summary>
        /// DDL statement for creating the Sync{ResourceKind} table.
        /// </summary>
        public const string CreateMetaTable = @"
            CREATE TABLE [{0}]
            (
                [Id] INT NOT NULL IDENTITY (1,1),
                [ResourceKind] NVARCHAR(64) NULL,
                [ExternalId] NVARCHAR(64) NOT NULL,
                [ExternalChildId] NVARCHAR(64) NULL,
                [HashKey] NVARCHAR(128) NOT NULL,
                [EndpointId] INT NOT NULL,
                [EndpointTick] INT NOT NULL,
                [Deleted] BIT NOT NULL,
                [Active] BIT NOT NULL
            );
            ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}] PRIMARY KEY ([Id]);
            CREATE UNIQUE INDEX [IX_{0}_Entity] ON [{0}] ([ExternalId] ASC, [ResourceKind] ASC, [ExternalChildId] ASC);
            CREATE INDEX [IX_{0}_ExternalId] ON [{0}] ([ExternalId] ASC);
            CREATE INDEX [IX_{0}_Endpoint] ON [{0}] ([EndpointTick] ASC);";
    }

    /// <summary>
    /// Extension class for data handling.
    /// </summary>
    public static class SyncCeExtensions
    {
        /// <summary>
        /// Determines if the column value for a data row has changed.
        /// </summary>
        /// <param name="row">The row object to act upon.</param>
        /// <param name="columnName">The name of the column that will be evaluated.</param>
        /// <returns>True if the current value does not equal the original column value.</returns>
        public static bool ColumnChanged(this DataRow row, string columnName)
        {
            if (String.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            return !(row[columnName, DataRowVersion.Current].Equals(row[columnName, DataRowVersion.Original]));
        }

        /// <summary>
        /// Allows direct execution of sql DDL and DML statements, such as INSERT/UPDATE/DELETE. 
        /// This is needed for database and table generation, as well as batch updates where single record 
        /// processing does not make sense.
        /// </summary>
        /// <param name="connection">The connection object to act upon.</param>
        /// <param name="commandText">The sql statement to process.</param>
        /// <param name="throwErrors">True if exceptions should be thrown, otherwise false.</param>
        /// <returns>The affected row count, or (-1) if no statement was passed.</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")] 
        public static int Execute(this SqlCeConnection connection, string commandText, bool throwErrors = true)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Execute requires an open and available Connection. The connection's current state is " + connection.State);
            }

            if (string.IsNullOrEmpty(commandText)) return SyncValues.Invalid;

            try
            {
                var commands = commandText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                using (var command = new SqlCeCommand(commandText.Trim(), connection))
                {
                    command.CommandTimeout = 0;

                    if (commands.Length == 1)
                    {
                        command.CommandText = commands[0] + SqlSyntax.Term;
                        return command.ExecuteNonQuery();
                    }

                    foreach (var line in commands)
                    {
                        command.CommandText = line + SqlSyntax.Term;
                        command.ExecuteNonQuery();
                    }
                    return 0;
                }
            }
            catch (Exception)
            {
                if (throwErrors)
                {
                    throw;
                }
            }

            return SyncValues.Invalid;
        }

        /// <summary>
        /// Determines if the table name exists in the Sql CE database.
        /// </summary>
        /// <param name="connection">The connection object to act upon.</param>
        /// <param name="tableName">The table name to verify.</param>
        /// <returns>True if the table exists, otherwise false.</returns>
        public static bool TableExists(this SqlCeConnection connection, string tableName)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (tableName == null) throw new ArgumentNullException("tableName");
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("Invalid table name");
            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("TableExists requires an open and available Connection. The connection's current state is " + connection.State);
            }

            using (var command = new SqlCeCommand(SyncDml.TableExists, connection))
            {
                command.Parameters.Add(SqlParameters.TableName, SqlDbType.NVarChar).Value = tableName;
                return (command.ExecuteScalar() != null);
            }
        }
    }

    /// <summary>
    /// Conversion class for handling database string values that may contain embedded nulls.
    /// </summary>
    internal static class SyncConvert
    {
        /// <summary>
        /// Encodes the specified string to a base-64 version of the string.
        /// </summary>
        /// <param name="value">The string value to encode.</param>
        /// <returns>The string representation, in base-64, of the specified source string.</returns>
        public static string Encode(string value)
        {
            return (string.IsNullOrEmpty(value) ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
        }

        /// <summary>
        /// Decodes the specified base-64 encoded string back to its original form.
        /// </summary>
        /// <param name="value">The string value to decode.</param>
        /// <returns>The original source string before encoding.</returns>
        public static string Decode(string value)
        {
            return (string.IsNullOrEmpty(value) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(value)));
        }
    }
}