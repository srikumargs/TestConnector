using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using QueueStore = Sage.Connector.PremiseStore.QueueStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// A factory for manipulating queue entries
    /// </summary>
    public class QueueEntryRecordFactory
    {
        #region Private Constructors

        /// <summary>
        /// Private default ctor
        /// </summary>
        private QueueEntryRecordFactory()
        { }

        /// <summary>
        /// Private ctor taking a logger
        /// </summary>
        /// <param name="logger"></param>
        private QueueEntryRecordFactory(ILogging logger)
            : this()
        {
            Logger = logger;
        }

        #endregion


        #region Public Members

        /// <summary>
        /// The logger to use if needed
        /// </summary>
        private ILogging Logger { get; set; }

        /// <summary>
        /// SQL Server compact's minimum is 01/01/1753 versus C#'s 01/01/0001
        /// </summary>
        public static DateTime ProcessingExpirationMinimum
        {
            get
            {
                return new DateTime(1753, 01, 01);
            }
        }

        /// <summary>
        /// Define a 'maximum' date to use for resetting queue dates when restoring
        /// </summary>
        public static DateTime ProcessingMaximum
        {
            get
            {
                return new DateTime(9999, 12, 12);
            }
        }

        /// <summary>
        /// Static creation of a new queue entry record factory
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static QueueEntryRecordFactory Create(ILogging logger)
        { return new QueueEntryRecordFactory(logger); }

        /// <summary>
        /// Returns the number of elements of a given queue 'type'
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public int Count(string Type)
        {
            int count = 0;

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        count = mc.QueueEntries.Count(x => x.Type == Type);
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);

            // Return the count
            return count;
        }

        /// <summary>
        /// Returns the first element of a given queue 'type' (null if none)
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="processingTimeout"></param>
        /// <returns></returns>
        public QueueEntryRecord GetFirst(string Type, double processingTimeout)
        {
            QueueEntryRecord result = null;

            // Retrieve the identifier for the 'first' queue entry
            // (matching our type and by date time creation)
            Guid QueueId = Guid.Empty;
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        // SQL Compact LINQ does not support DateTime.UTCNow
                        // so doing post-retrieval expiration testing
                        var orderedQueues =
                            from qe in mc.QueueEntries
                            where qe.Type == Type
                            orderby qe.DateTimeUtc
                            select qe;

                        if (null != orderedQueues)
                        {
                            foreach (var queueElement in orderedQueues)
                            {
                                if (queueElement.ProcessingExpirationDateTimeUtc <= DateTime.UtcNow)
                                {
                                    queueElement.ProcessingExpirationDateTimeUtc = DateTime.UtcNow.AddMilliseconds(processingTimeout);
                                    mc.SaveChanges();
                                    QueueId = queueElement.QueueId;
                                    break;
                                }
                            }
                        }
                    }
                });

            // Execute 
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);

            // If found, invoke retrieval
            if (Guid.Empty != QueueId)
            {
                result = GetEntry(QueueId);
            }

            return result;
        }

        /// <summary>
        /// Clear all queue's processing timeouts
        /// </summary>
        public void ClearAllProcessingTimeout()
        {
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        var entries =
                            from entry in mc.QueueEntries
                            where entry.ProcessingExpirationDateTimeUtc > ProcessingExpirationMinimum
                            select entry;

                        foreach (var entry in entries)
                        {
                            entry.ProcessingExpirationDateTimeUtc = ProcessingExpirationMinimum;
                        }
                        mc.SaveChanges();
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);
        }

        /// <summary>
        /// Resets the processing timeout to its default
        /// </summary>
        /// <param name="queueId"></param>
        public void ClearProcessingTimeout(Guid queueId)
        {
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var entry = mc.QueueEntries.First(x => x.QueueId == queueId);
                            // When clearing the processing timeout of a specific queue
                            // entry, it gets shoved to the back of the line
                            entry.DateTimeUtc = ProcessingMaximum;
                            entry.ProcessingExpirationDateTimeUtc = ProcessingExpirationMinimum;
                            mc.SaveChanges();
                        }
                        catch (InvalidOperationException)
                        {
                            // No entries to clear
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);
        }

        /// <summary>
        /// Returns the first element of a given queue 'type' (null if none)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newType"></param>
        /// <returns></returns>
        public QueueEntryRecord GetAndRenameFirst(string type, string newType)
        {
            QueueEntryRecord result = null;

            // Retrieve the identifier for the 'first' queue entry
            // (matching our type and by date time creation)
            Guid QueueId = Guid.Empty;
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        var orderedQueues = mc.QueueEntries.OrderBy(x => x.DateTimeUtc);
                        if (null != orderedQueues)
                        {
                            if (orderedQueues.Count(x => x.Type == type) > 0)
                            {
                                var topQueueElement = orderedQueues.First(x => x.Type == type);
                                if (null != topQueueElement)
                                {
                                    QueueId = topQueueElement.QueueId;
                                    topQueueElement.Type = newType;
                                    mc.SaveChanges();
                                }
                            }
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);

            // If found, invoke retrieval
            if (Guid.Empty != QueueId)
            {
                result = GetEntry(QueueId);
            }

            return result;
        }

        /// <summary>
        /// Renames existing queue entries of a given type with the supplied
        /// new type name
        /// </summary>
        /// <param name="originalTypeName"></param>
        /// <param name="newTypeName"></param>
        public void RenameQueueType(string originalTypeName, string newTypeName)
        {
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        foreach (var entry in mc.QueueEntries.Where(x => x.Type == originalTypeName))
                        {
                            entry.Type = newTypeName;
                        }
                        mc.SaveChanges();
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);
        }

        /// <summary>
        /// Creates a new queue entry
        /// </summary>
        /// <returns></returns>
        public QueueEntryRecord CreateNewEntry()
        {
            return new QueueEntryRecord(new QueueStore.QueueEntry());
        }

        /// <summary>
        /// Retrieves an existing queue entry (null if none)
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public QueueEntryRecord GetEntry(Guid entryId)
        {
            QueueEntryRecord result = null;

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var entry = mc.QueueEntries.First(x => x.QueueId == entryId);
                            result = new QueueEntryRecord(entry);
                            // If needed delay load actual result.Content from document store
                            mc.Detach(entry);
                        }
                        catch (InvalidOperationException)
                        {
                            // no matching entry
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);
            
            return result;
        }

        /// <summary>
        /// Retrieves the entire collection of queue entries
        /// </summary>
        /// <returns></returns>
        public QueueEntryRecord[] GetEntries()
        {
            var result = new List<QueueEntryRecord>();

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        var entries = mc.QueueEntries;
                        foreach (QueueStore.QueueEntry entry in entries)
                        {
                            var entryRecord = new QueueEntryRecord(entry);
                            // If needed delay load actual result.Content from document store
                            result.Add(entryRecord);
                            mc.Detach(entry);
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);

            return result.ToArray();
        }

        /// <summary>
        /// Deletes an existing queue entry (false if not exists)
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public bool DeleteEntry(Guid entryId)
        {
            bool result = false;
    
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var entry = mc.QueueEntries.First(x => x.QueueId == entryId);
                            mc.DeleteObject(entry);
                            mc.SaveChanges();

                            result = true;
                        }
                        catch (System.Data.OptimisticConcurrencyException)
                        {
                            // concurrency conflict - leave result as false
                        }
                        catch (InvalidOperationException)
                        {
                            // Nothing to delete - leave result as false
                        }
                    }
                });

            // Execute 
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);

            return result;
        }

        /// <summary>
        /// Deletes all entries of a particular type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Guid> DeleteTypedEntry(string type)
        {
            List<Guid> deletedEntryIds = new List<Guid>();

            // Create the retry action
            Action retryAction = new Action(() =>
            {
                using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                {
                    var entries = mc.QueueEntries.Where(entry => entry.Type.Equals(type));
                    if (null != entries)
                    {

                        foreach (var entry in entries)
                        {
                            deletedEntryIds.Add(entry.QueueId);
                            mc.DeleteObject(entry);
                        }
                        mc.SaveChanges();

                    }
                }
            });

            // Execute 
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);

            return deletedEntryIds;
        }

        /// <summary>
        /// Persists a new queue (false if saves fails)
        /// </summary>
        /// <param name="newEntryRecord"></param>
        /// <param name="updateExisting"></param>
        /// <returns></returns>
        public Guid AddEntry(QueueEntryRecord newEntryRecord, bool updateExisting = false)
        {
            try
            {
                Sage.Diagnostics.ArgumentValidator.ValidateNonNullReference(newEntryRecord, "New Entry Record", "Queue Entry Record Factory");

                // Create the retry action
                Guid entryGuid = Guid.Empty;
                Action retryAction = new Action(() =>
                    {
                        var entry = newEntryRecord.GetInternalQueueEntry();
                        SetNewEntryAutoValues(ref entry);

                        /* GC.Collect(); */

                        using (var mc = QueueStore.QueueStoreResolver.ModelContainer)
                        {
                            if (updateExisting)
                            {
                                mc.QueueEntries.Attach(entry);
                                mc.ObjectStateManager.ChangeObjectState(entry, EntityState.Modified);
                            }
                            else
                            {
                                mc.QueueEntries.AddObject(entry);
                            }

                            mc.SaveChanges();

                            // const string queueCommand = @"INSERT INTO QueueEntries ([QueueId], [DateTimeUtc], [User], [Machine], [Type], [ProcessingExpirationDateTimeUtc], [Content]) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})";
                            // mc.ExecuteStoreCommand(queueCommand, entry.QueueId, entry.DateTimeUtc, entry.User,
                            //                       entry.Machine, entry.Type, entry.ProcessingExpirationDateTimeUtc,
                            //                       entry.Content);
                        }

                        // Set the resulting guid for the new entry
                        entryGuid = entry.QueueId;
                    });

                // Execute
                RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, Logger);

                return entryGuid;
            }
            catch (Exception ex)
            {
                throw new QueueEntryRecordEnqueueException("Unable to add a queue entry.", ex);
            }
        }

        #endregion


        #region Private Members

        /// <summary>
        /// Sets context information (date/id/machine/user) on a new entry
        /// </summary>
        /// <param name="entry"></param>
        private static void SetNewEntryAutoValues(ref QueueStore.QueueEntry entry)
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
            entry.ProcessingExpirationDateTimeUtc = ProcessingExpirationMinimum;
            if (Guid.Empty.Equals(entry.QueueId))
            {
                entry.QueueId = Guid.NewGuid();
            }
            entry.Machine = sMachineName;
            entry.User = Environment.UserName;
        }

        #endregion

    }
}
