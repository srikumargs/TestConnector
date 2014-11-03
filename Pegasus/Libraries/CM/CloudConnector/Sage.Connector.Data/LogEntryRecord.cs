using System;
using LogStore = Sage.Connector.PremiseStore.LogStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// Record class for a log entry
    /// </summary>
    [Serializable]
    public class LogEntryRecord
    {
        private LogStore.LogEntry _logEntry = null;

        /// <summary>
        /// Constructs a record object from an entity object
        /// </summary>
        /// <param name="le"></param>
        internal LogEntryRecord(LogStore.LogEntry le)
        {
            _logEntry = le;
        }

        /// <summary>
        /// Internal retrieval of entity object for internal updates
        /// </summary>
        /// <returns></returns>
        internal LogStore.LogEntry GetInternalLogEntry()
        {
            return _logEntry;
        }

        /// <summary>
        /// Log entry identifier
        /// </summary>
        public Guid Id
        {
            get { return _logEntry.Id; }
        }

        /// <summary>
        /// The logging entry type
        /// </summary>
        public string Type
        {
            get { return _logEntry.Type; }
            set { _logEntry.Type = value; }
        }

        /// <summary>
        /// The source of the logging entry
        /// </summary>
        public string SourceTypeName
        {
            get { return _logEntry.SourceTypeName; }
            set { _logEntry.SourceTypeName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SourceMemberName
        {
            get { return _logEntry.SourceMemberName; }
            set { _logEntry.SourceMemberName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ProcessId
        {
            get { return _logEntry.ProcessId; }
            set { _logEntry.ProcessId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 AppDomainId
        {
            get { return _logEntry.AppDomainId; }
            set { _logEntry.AppDomainId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ThreadId
        {
            get { return _logEntry.ThreadId; }
            set { _logEntry.ThreadId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ObjectId
        {
            get { return _logEntry.ObjectId; }
            set { _logEntry.ObjectId = value; }
        }
        
        /// <summary>
        /// The user that generated the logging entry
        /// </summary>
        public string User
        {
            get { return _logEntry.User; }
            set { _logEntry.User = value; }
        }

        /// <summary>
        /// The machine that generated the logging entry
        /// </summary>
        public string Machine
        {
            get { return _logEntry.Machine; }
            set { _logEntry.Machine = value; }
        }

        /// <summary>
        /// The date and time of the logged entry
        /// </summary>
        public DateTime DateTime
        {
            get { return _logEntry.DateTimeUtc; }
            set { _logEntry.DateTimeUtc = value; }
        }

        /// <summary>
        /// The description of the log
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _description;

        /// <summary>
        /// Tenant id associated with the log entry
        /// </summary>
        public string CloudTenantId
        {
            get { return _logEntry.CloudTenantId; }
            set { _logEntry.CloudTenantId = value; }
        }

        /// <summary>
        /// Request id associated with the log entry
        /// </summary>
        public Guid? CloudRequestId
        {
            get { return _logEntry.CloudRequestId; }
            set { _logEntry.CloudRequestId = value; }
        }
    }
}
