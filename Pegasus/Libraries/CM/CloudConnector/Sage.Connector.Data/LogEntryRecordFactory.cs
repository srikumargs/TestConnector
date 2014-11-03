using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using LogStore = Sage.Connector.PremiseStore.LogStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// A factory for manipulating log entries
    /// </summary>
    public class LogEntryRecordFactory
    {
        #region Private Constructors
        
        /// <summary>
        /// Private default ctor
        /// </summary>
        private LogEntryRecordFactory()
        {
            RetentionPolicyThreshold = ConnectorRegistryUtils.LogRetentionPolicyThreshold;
        }

        /// <summary>
        /// Private ctor taking a logger
        /// </summary>
        /// <param name="logger"></param>
        private LogEntryRecordFactory(ILogging logger)
            : this()
        {
            Logger = logger;
        }

        #endregion


        #region Public Members

        /// <summary>
        /// Static creation of a log entry record factory
        /// </summary>
        /// <remarks>
        /// This factory method is deliberately different than the other ones (it doesn't take a logger).
        /// The reason for this difference is to prevent self-referential retry logic.
        /// </remarks>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method; rule does not apply")]
        public static LogEntryRecordFactory Create(/*ILogging logger*/)
        { return new LogEntryRecordFactory(new SimpleTraceLogger()); }

        /// <summary>
        /// Creates a new log entry
        /// </summary>
        /// <returns></returns>
        public LogEntryRecord CreateNewEntry()
        { return new LogEntryRecord(new LogStore.LogEntry()); }

        /// <summary>
        /// Retrieves an existing log entry (null if not found)
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public LogEntryRecord GetEntry(Guid entryId)
        {
            LogEntryRecord result = null;

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    try
                    {
                        using (var mc = LogStore.LogStoreResolver.ModelContainer)
                        {
                            var entry = mc.LogEntries.First(x => x.Id == entryId);
                            result = new LogEntryRecord(entry);
                            result.Description = entry.Content;
                            mc.Detach(entry);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // No matching entry
                    }
            });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);
            
            return result;
        }

        /// <summary>
        /// Retrieves the entire collection of log entries
        /// </summary>
        /// <returns></returns>
        public LogEntryRecord[] GetEntries()
        {
            var result = new List<LogEntryRecord>();

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = LogStore.LogStoreResolver.ModelContainer)
                    {
                        var entries = mc.LogEntries;
                        foreach (var entry in entries)
                        {
                            var entryRecord = new LogEntryRecord(entry);
                            entryRecord.Description = entry.Content;
                            result.Add(entryRecord);
                            mc.Detach(entry);
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);

            return result.ToArray();
        }

        /// <summary>
        /// Deletes an existing log entry (false if not exists)
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public bool DeleteEntry(Guid entryId)
        {
            bool result = false;

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = LogStore.LogStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var entry = mc.LogEntries.First(x => x.Id == entryId);
                            mc.DeleteObject(entry);
                            mc.SaveChanges();

                            result = true;
                        }
                        catch (InvalidOperationException)
                        {
                            // No matching entry - leave result as false
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan RetentionPolicyThreshold { get; set; }

        /// <summary>
        /// The logger to use if needed
        /// </summary>
        private ILogging Logger { get; set; }

        /// <summary>
        /// Persists a new log (false if saves fails)
        /// </summary>
        /// <param name="activityEntryId"></param>
        /// <param name="newEntryRecord"></param>
        /// <returns></returns>
        public Guid AddEntry(Guid? activityEntryId, LogEntryRecord newEntryRecord)
        {
            if (null == newEntryRecord)
            {
                return Guid.Empty;
            }
            var entry = newEntryRecord.GetInternalLogEntry();
            if (null == entry)
            {
                return Guid.Empty;
            }
            SetNewEntryAutoValues(ref entry);
            entry.Content = newEntryRecord.Description;

            Guid entryGuid = Guid.Empty;
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = LogStore.LogStoreResolver.ModelContainer)
                    {
                        try
                        {
                            CleanupOldEntries(mc);

                            mc.LogEntries.AddObject(entry);

                            // associate with the parent activity
                            if (activityEntryId.HasValue)
                            {
                                var activityEntry = mc.ActivityEntries.FirstOrDefault(x => x.Id == activityEntryId.Value);
                                if (activityEntry != null)
                                {
                                    activityEntry.LogEntries.Add(entry);
                                }
                            }

                            mc.SaveChanges();

                            // Set the resulting guid
                            entryGuid = entry.Id;
                        }
                        catch (System.Data.OptimisticConcurrencyException)
                        {
                            // concurrency conflict - leave the result as an empty guid
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);

            return entryGuid;
        }

        /// <summary>
        /// Persists a new log (false if saves fails)
        /// </summary>
        /// <param name="newEntryRecord"></param>
        /// <returns></returns>
        public Guid AddEntry(LogEntryRecord newEntryRecord)
        {
            return AddEntry(null, newEntryRecord);
        }

        #endregion


        #region Private Members

        /// <summary>
        /// Sets context for a new log entry
        /// </summary>
        /// <param name="entry"></param>
        private static void SetNewEntryAutoValues(ref LogStore.LogEntry entry)
        {
            string sMachineName = string.Empty;
            try
            {
                sMachineName = System.Environment.MachineName;
            }
            catch (InvalidOperationException)
            {
                // Machine name could not be retrieved
            }

            entry.DateTimeUtc = DateTime.UtcNow;
            if (Guid.Empty.Equals(entry.Id))
            {
                entry.Id = Guid.NewGuid();
            }
            entry.Machine = sMachineName;
            entry.User = Environment.UserName;
        }

        private void CleanupOldEntries(LogStore.LogStoreModelContainer mc)
        {
            DateTime oldEntriesDateTimeUtc = DateTime.UtcNow.Subtract(RetentionPolicyThreshold);
            var entriesToDelete = mc.LogEntries.Where(x => x.DateTimeUtc < oldEntriesDateTimeUtc);
            foreach (var entryToDelete in entriesToDelete)
            {
                mc.DeleteObject(entryToDelete);
            }
        }

        #endregion
    }
}
