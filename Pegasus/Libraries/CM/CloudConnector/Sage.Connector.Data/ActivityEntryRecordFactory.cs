using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Connector.Common;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.ConnectorServiceCommon;
using LogStore = Sage.Connector.PremiseStore.LogStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// A factory for manipulating activity entries
    /// </summary>
    public class ActivityEntryRecordFactory
    {
        #region Private Constructors

        /// <summary>
        /// Private default ctor
        /// </summary>
        private ActivityEntryRecordFactory()
        {
            RetentionPolicyThreshold = ConnectorRegistryUtils.LogRetentionPolicyThreshold;
        }

        /// <summary>
        /// Private ctor taking a logger
        /// </summary>
        /// <param name="logger"></param>
        private ActivityEntryRecordFactory(ILogging logger)
            : this()
        {
            Logger = logger;
        }

        #endregion


        #region Public Members

        /// <summary>
        /// Static creation of a activity entry record factory
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ActivityEntryRecordFactory Create(ILogging logger)
        { return new ActivityEntryRecordFactory(logger); }

        /// <summary>
        /// Creates a new activity entry
        /// </summary>
        /// <returns></returns>
        public ActivityEntryRecord CreateNewEntry()
        { return new ActivityEntryRecord(new LogStore.ActivityEntry()); }

        /// <summary>
        /// Retrieves an existing activity entry (null if not found)
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public ActivityEntryRecord GetEntry(Guid entryId)
        {
            ActivityEntryRecord result = null;
            Action retryAction = new Action(() =>
                {
                    using (var mc = LogStore.LogStoreResolver.ModelContainer)
                    {
                    // Create the retry action
                        try
                        {
                            var entry = mc.ActivityEntries.First(x => x.Id == entryId);
                            result = new ActivityEntryRecord(entry);
                            mc.Detach(entry);
                        }
                        catch (InvalidOperationException)
                        {
                            // No matching entry
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);

            return result;
        }

        /// <summary>
        /// Retrieves the entire collection of activity entries
        /// </summary>
        /// <returns></returns>
        public ActivityEntryRecord[] GetEntries()
        {
            var result = new List<ActivityEntryRecord>();

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = LogStore.LogStoreResolver.ModelContainer)
                    {
                        var entries = mc.ActivityEntries;
                        foreach (var entry in entries)
                        {
                            var entryRecord = new ActivityEntryRecord(entry);
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
        /// Retrieves the recently created activity entries as well as all currently in-progress ones
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        public RequestState[] GetRecentAndInProgressEntriesAsRequestState(TimeSpan recentEntriesThreshold)
        {
            var result = new List<RequestState>();

            DateTime recentEntriesDateTimeUtc = DateTime.UtcNow.Subtract(recentEntriesThreshold);

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    var fac = PremiseConfigurationRecordFactory.Create(null);
                    var pcrs = fac.GetEntries().ToDictionary((x) => x.CloudTenantId);

                    using (var mc = LogStore.LogStoreResolver.ModelContainer)
                    {
                        var entries = from x in mc.ActivityEntries
                                      where x.DateTimeUtc > recentEntriesDateTimeUtc || x.Status == (Int32)ActivityEntryStatus.InProgress || x.Status == (Int32)ActivityEntryStatus.InProgressBindableWorkProcessing
                                      orderby x.DateTimeUtc descending
                                      select x;

                        foreach (var entry in entries)
                        {
                            PremiseConfigurationRecord pcr = null;
                            pcrs.TryGetValue(entry.CloudTenantId, out pcr);

                            var requestState = new RequestState(
                                entry.Id,
                                entry.CloudTenantId,
                                entry.CloudRequestId,
                                entry.CloudRequestCreatedTimestampUtc,
                                entry.CloudRequestRetryCount,
                                entry.CloudRequestType,
                                entry.CloudRequestInnerType,
                                entry.CloudRequestRequestingUser,
                                entry.IsSystemRequest,
                                entry.State1DateTimeUtc,
                                entry.State2DateTimeUtc,
                                entry.State3DateTimeUtc,
                                entry.State4DateTimeUtc,
                                entry.State5DateTimeUtc,
                                entry.State6DateTimeUtc,
                                entry.State7DateTimeUtc,
                                entry.State8DateTimeUtc,
                                entry.State9DateTimeUtc,
                                entry.State10DateTimeUtc,
                                entry.State11DateTimeUtc,
                                entry.State12DateTimeUtc,
                                entry.State13DateTimeUtc,
                                entry.State14DateTimeUtc,
                                entry.State15DateTimeUtc,
                                entry.State16DateTimeUtc,
                                entry.State17DateTimeUtc,
                                entry.DateTimeUtc,
                                (RequestStatus)entry.Status, 
                                
                                pcr != null ? pcr.CloudCompanyName : ((entry.CloudTenantId == Guid.Empty.ToString())? "(System)"  : "(Deleted)"),
                                pcr != null ? pcr.BackOfficeCompanyName : ((entry.CloudTenantId == Guid.Empty.ToString()) ? "(System)" : "(Deleted)"),
                                entry.CloudProjectName,
                                entry.CloudRequestSummary);
                            result.Add(requestState);
                            mc.Detach(entry);
                        }
                    }
                });
  
            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);

            return result.ToArray();
        }

        /// <summary>
        /// Deletes an existing activity entry (false if not exists)
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
                            var entry = mc.ActivityEntries.First(x => x.Id == entryId);
                            entry.LogEntries.Load();
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
        /// Deletes activity entries for a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public void DeleteTenantEntry(String tenantId)
        {
            // Create the retry action
            Action retryAction = new Action(() =>
            {
                using (var mc = LogStore.LogStoreResolver.ModelContainer)
                {
                    var entries = mc.ActivityEntries.Where(entry => entry.CloudTenantId == tenantId);
                    foreach (var entry in entries)
                    {
                        mc.DeleteObject(entry);
                    }
                    mc.SaveChanges();
                }
            });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);
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
        /// Update the specified activity entry
        /// </summary>
        /// <param name="updatedEntryRecord"></param>
        /// <returns></returns>
        public void UpdateEntry(ActivityEntryRecord updatedEntryRecord)
        {
            if ((null != updatedEntryRecord) && (null != updatedEntryRecord.Id) && (Guid.Empty != updatedEntryRecord.Id))
            {
                // Create the retry action
                Action retryAction = new Action(() =>
                    {
                        using (var mc = LogStore.LogStoreResolver.ModelContainer)
                        {
                            try
                            {
                                var entry = mc.ActivityEntries.First(x => x.Id == updatedEntryRecord.Id);
                                if (null != entry)
                                {
                                    MapPublicRecordToPrivateRecord(updatedEntryRecord, ref entry);
                                    mc.SaveChanges();
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                // does not exist
                            }
                            catch (System.Data.OptimisticConcurrencyException)
                            {
                                // concurrency conflict
                            }
                        }
                    });

                // Execute
                RetryPolicyManager.ExecuteInRetry(RetryPurpose.LogStore, retryAction, Logger);
            }
        }

        /// <summary>
        /// Persists a new activity entry (false if saves fails)
        /// </summary>
        /// <param name="newEntryRecord"></param>
        /// <returns></returns>
        public Guid AddEntry(ActivityEntryRecord newEntryRecord)
        {
            if (null == newEntryRecord)
            {
                return Guid.Empty;
            }
            var entry = newEntryRecord.GetInternalActivityEntry();
            if (null == entry)
            {
                return Guid.Empty;
            }
            SetNewEntryAutoValues(ref entry);

            Guid entryGuid = Guid.Empty;

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = LogStore.LogStoreResolver.ModelContainer)
                    {
                        try
                        {
                            CleanupOldEntries(mc);

                            mc.ActivityEntries.AddObject(entry);
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

        #endregion


        #region Private Members

        /// <summary>
        /// Sets context for a new activity entry
        /// </summary>
        /// <param name="entry"></param>
        private static void SetNewEntryAutoValues(ref LogStore.ActivityEntry entry)
        {
            entry.DateTimeUtc = DateTime.UtcNow;
            if (Guid.Empty.Equals(entry.Id))
            {
                entry.Id = Guid.NewGuid();
            }
        }

        private void CleanupOldEntries(LogStore.LogStoreModelContainer mc)
        {
            DateTime oldEntriesDateTimeUtc = DateTime.UtcNow.Subtract(RetentionPolicyThreshold);
            var entriesToDelete = mc.ActivityEntries.Where(x => x.DateTimeUtc < oldEntriesDateTimeUtc);
            foreach (var entryToDelete in entriesToDelete)
            {
                mc.DeleteObject(entryToDelete);
            }
        }

        private static void MapPublicRecordToPrivateRecord(ActivityEntryRecord publicRecord, ref LogStore.ActivityEntry privateRecord)
        {
            if ((null != publicRecord) && (null != privateRecord))
            {
                privateRecord.Id = publicRecord.Id;
                privateRecord.CloudRequestCreatedTimestampUtc = publicRecord.CloudRequestCreatedTimestampUtc;
                privateRecord.CloudRequestId = publicRecord.CloudRequestId;
                privateRecord.CloudRequestRequestingUser = publicRecord.CloudRequestRequestingUser;
                privateRecord.CloudRequestType = publicRecord.CloudRequestType;
                privateRecord.CloudRequestInnerType = publicRecord.CloudRequestInnerType;
                privateRecord.CloudRequestRetryCount = publicRecord.CloudRequestRetryCount;
                privateRecord.CloudTenantId = publicRecord.CloudTenantId;
                privateRecord.IsSystemRequest = publicRecord.IsSystemRequest;
                privateRecord.State1DateTimeUtc = publicRecord.State1DateTimeUtc;
                privateRecord.State2DateTimeUtc = publicRecord.State2DateTimeUtc;
                privateRecord.State3DateTimeUtc = publicRecord.State3DateTimeUtc;
                privateRecord.State4DateTimeUtc = publicRecord.State4DateTimeUtc;
                privateRecord.State5DateTimeUtc = publicRecord.State5DateTimeUtc;
                privateRecord.State6DateTimeUtc = publicRecord.State6DateTimeUtc;
                privateRecord.State7DateTimeUtc = publicRecord.State7DateTimeUtc;
                privateRecord.State8DateTimeUtc = publicRecord.State8DateTimeUtc;
                privateRecord.State9DateTimeUtc = publicRecord.State9DateTimeUtc;
                privateRecord.State10DateTimeUtc = publicRecord.State10DateTimeUtc;
                privateRecord.State11DateTimeUtc = publicRecord.State11DateTimeUtc;
                privateRecord.State12DateTimeUtc = publicRecord.State12DateTimeUtc;
                privateRecord.State13DateTimeUtc = publicRecord.State13DateTimeUtc;
                privateRecord.State14DateTimeUtc = publicRecord.State14DateTimeUtc;
                privateRecord.State15DateTimeUtc = publicRecord.State15DateTimeUtc;
                privateRecord.State16DateTimeUtc = publicRecord.State16DateTimeUtc;
                privateRecord.State17DateTimeUtc = publicRecord.State17DateTimeUtc;
                privateRecord.Status = (Int32)publicRecord.Status;
            }
        }

        #endregion
    }
}
