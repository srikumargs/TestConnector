using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using ConfigurationStore = Sage.Connector.PremiseStore.ConfigurationStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// The factory to retrieve and update the configuration
    /// </summary>
    public class PremiseConfigurationRecordFactory
    {
        #region Private Constructors
        
        /// <summary>
        /// Private default ctor
        /// </summary>
        private PremiseConfigurationRecordFactory()
        { }

        /// <summary>
        /// Private ctor taking a logger
        /// </summary>
        /// <param name="logger"></param>
        private PremiseConfigurationRecordFactory(ILogging logger)
            : this()
        {
            Logger = logger;
        }

        #endregion


        #region Public Members

        /// <summary>
        /// The logger to use if needed
        /// </summary>
        private ILogging Logger { get; set;  }

        /// <summary>
        /// Static creation of a new PremiseConfigurationRecordFactory
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static PremiseConfigurationRecordFactory Create(ILogging logger)
        { return new PremiseConfigurationRecordFactory(logger); }

        /// <summary>
        /// Update the specified premise configuration
        /// </summary>
        /// <param name="updatedConfigurationRecord"></param>
        /// <returns></returns>
        public void UpdateEntry(PremiseConfigurationRecord updatedConfigurationRecord)
        {
            if ((null != updatedConfigurationRecord) && (null != updatedConfigurationRecord.Id) && (Guid.Empty != updatedConfigurationRecord.Id))
            {
                // Create the retry action
                Action retryAction = new Action(() =>
                    {
                        using (var mc = ConfigurationStore.ConfigurationStoreResolver.ModelContainer)
                        {
                            try
                            {
                                var configuration = mc.PremiseConfigurations.First(x => x.Id == updatedConfigurationRecord.Id);
                                if (null != configuration)
                                {
                                    MapPublicRecordToPrivateRecord(updatedConfigurationRecord, ref configuration);
                                    mc.SaveChanges();
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                // does not exist
                            }
                        }
                    });

                // Execute
                RetryPolicyManager.ExecuteInRetry(RetryPurpose.ConfigurationStore, retryAction, Logger);
            }
        }

        /// <summary>
        /// Retrieves the specified entry by tenant id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public PremiseConfigurationRecord GetEntryByTenantId(string tenantId)
        {
            PremiseConfigurationRecord result = null;
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = ConfigurationStore.ConfigurationStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var configuration = mc.PremiseConfigurations.Single(x => x.CloudTenantId == tenantId);
                            result = new PremiseConfigurationRecord(configuration);
                            mc.Detach(configuration);
                        }
                        catch (InvalidOperationException)
                        {
                            // No matching entry
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.ConfigurationStore, retryAction, Logger);

            return result;
        }

        /// <summary>
        /// Deletes the specified premise configuration
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public bool DeleteEntryByTenantId(string tenantId)
        {
            bool result = false;
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = ConfigurationStore.ConfigurationStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var configuration = mc.PremiseConfigurations.First(x => x.CloudTenantId == tenantId);
                            mc.DeleteObject(configuration);
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
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.ConfigurationStore, retryAction, Logger);

            return result;
        }

        /// <summary>
        /// Creates a new PermiseConfigurationRecord
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord CreateNewEntry()
        {
            var record = new PremiseConfigurationRecord();
            SetDefaultPremiseConfigurationRecordValues(record);
            return record;
        }

        /// <summary>
        /// Retrieves the specified entry
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public PremiseConfigurationRecord GetEntry(Guid entryId)
        {
            PremiseConfigurationRecord result = null;
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = ConfigurationStore.ConfigurationStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var configuration = mc.PremiseConfigurations.Single(x => x.Id == entryId);
                            result = new PremiseConfigurationRecord(configuration);
                            mc.Detach(configuration);
                        }
                        catch (InvalidOperationException)
                        {
                            // No matching entry
                        }
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.ConfigurationStore, retryAction, Logger);

            return result;
        }

        /// <summary>
        /// Retrieves the collection of premise configurations
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord[] GetEntries()
        {
            var result = new List<PremiseConfigurationRecord>();

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    // Reset the collection if we retry
                    result.Clear();

                    using (var mc = ConfigurationStore.ConfigurationStoreResolver.ModelContainer)
                    {
                        var configurations = mc.PremiseConfigurations;
                        foreach (var configuration in configurations)
                        {
                            var configurationRecord = new PremiseConfigurationRecord(configuration);
                            result.Add(configurationRecord);
                            mc.Detach(configuration);
                        }
                    }

                    // Sort the collection
                    result.Sort();
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.ConfigurationStore, retryAction, Logger);

            return result.ToArray();
        }

        /// <summary>
        /// Deletes the specified premise configuration
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public bool DeleteEntry(Guid entryId)
        {
            bool result = false;
            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = ConfigurationStore.ConfigurationStoreResolver.ModelContainer)
                    {
                        try
                        {
                            var configuration = mc.PremiseConfigurations.First(x => x.Id == entryId);
                            mc.DeleteObject(configuration);
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
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.ConfigurationStore, retryAction, Logger);

            return result;
        }

        /// <summary>
        /// Adds the specified premise configuration
        /// </summary>
        /// <param name="newConfigurationRecord"></param>
        /// <returns></returns>
        public Guid AddEntry(PremiseConfigurationRecord newConfigurationRecord)
        {
            if (null == newConfigurationRecord)
            {
                return Guid.Empty;
            }

            var configuration = new ConfigurationStore.PremiseConfiguration();
            MapPublicRecordToPrivateRecord(newConfigurationRecord, ref configuration);

            if ((null == configuration.Id) || (configuration.Id == Guid.Empty))
            {
                configuration.Id = Guid.NewGuid();
            }

            // Create the retry action
            Action retryAction = new Action(() =>
                {
                    using (var mc = ConfigurationStore.ConfigurationStoreResolver.ModelContainer)
                    {
                        mc.PremiseConfigurations.AddObject(configuration);
                        mc.SaveChanges();
                    }
                });

            // Execute
            RetryPolicyManager.ExecuteInRetry(RetryPurpose.ConfigurationStore, retryAction, Logger);

            return configuration.Id;
        }

        #endregion


        #region Private Members

        /// <summary>
        /// For use in updating config record, for example
        /// </summary>
        /// <param name="publicRecord">The updated config</param>
        /// <param name="privateRecord">The stored config</param>
        private static void MapPublicRecordToPrivateRecord(PremiseConfigurationRecord publicRecord, ref ConfigurationStore.PremiseConfiguration privateRecord)
        {
            if ((null != publicRecord) && (null != privateRecord))
            {
                privateRecord.PremiseAgent = publicRecord.PremiseAgent;
                privateRecord.CloudTenantId = publicRecord.CloudTenantId;
                privateRecord.CloudPremiseKey = publicRecord.CloudPremiseKey;
                privateRecord.SiteAddress = publicRecord.SiteAddress;
                privateRecord.MinCommunicationFailureRetryInterval = publicRecord.MinCommunicationFailureRetryInterval;
                privateRecord.MaxCommunicationFailureRetryInterval = publicRecord.MaxCommunicationFailureRetryInterval;

                privateRecord.BackOfficeConnectionEnabledToReceive = publicRecord.BackOfficeConnectionEnabledToReceive;
                privateRecord.CloudConnectionEnabledToReceive = publicRecord.CloudConnectionEnabledToReceive;
                privateRecord.CloudConnectionEnabledToSend = publicRecord.CloudConnectionEnabledToSend;
                privateRecord.CloudCompanyUrl = publicRecord.CloudCompanyUrl;
                privateRecord.CloudCompanyName = publicRecord.CloudCompanyName;
                privateRecord.BackOfficeCompanyName = publicRecord.BackOfficeCompanyName;
                privateRecord.BackOfficeAllowableConcurrentExecutions = publicRecord.BackOfficeAllowableConcurrentExecutions;

                privateRecord.SentDocumentStoragePolicy = publicRecord.SentDocumentStoragePolicy;
                privateRecord.SentDocumentStorageDays = publicRecord.SentDocumentStorageDays;
                privateRecord.SentDocumentStorageMBs = publicRecord.SentDocumentStorageMBs;
                privateRecord.SentDocumentFolderName = publicRecord.SentDocumentFolderName;

                privateRecord.ConnectorPluginId = publicRecord.ConnectorPluginId;
                privateRecord.BackOfficeProductName = publicRecord.BackOfficeProductName;
                privateRecord.BackOfficeConnectionCredentials = publicRecord.BackOfficeConnectionCredentials;
                privateRecord.BackOfficeCompanyUniqueId = publicRecord.BackOfficeCompanyUniqueId;
                privateRecord.CloudTenantClaim = publicRecord.CloudTenantClaim;
                privateRecord.BackOfficeConnectionCredentialsDescription = publicRecord.BackOfficeConnectionCredentialsDescription;
            }
        }

        private void SetDefaultPremiseConfigurationRecordValues(PremiseConfigurationRecord configurationRecord)
        {
            //TODO: revisit this in light of what what control we want from the cloud.
            //I suspect we might want to move these back to zero and provide defaults at the messaging service
            //from a default values record.
            //However one could make the argument that they should be set on a per record basis and that is
            //the end of it.

            //Leave these values at the defaults from creation. 
            //pcr.UserName
            //pcr.Password;
            //pcr.DataDirectoryPath;
            //pcr.PremiseAgent;
            //pcr.TenantId;
            //pcr.PremiseKey;

            //but provide "rational" defaults for thse... 
            //set to the default cloud end point by default.
            configurationRecord.SiteAddress = ConnectorRegistryUtils.SiteAddress;
            configurationRecord.MinCommunicationFailureRetryInterval = (Int32)TimeSpan.FromMinutes(1).TotalMilliseconds;
            configurationRecord.MaxCommunicationFailureRetryInterval = (Int32)TimeSpan.FromMinutes(5).TotalMilliseconds;

            // TODO: re-evaluate:  values? where should these defaults be persisted? should the defaults be updateable?
            configurationRecord.SentDocumentStoragePolicy = 1;   // Immediate delete of sent files
            configurationRecord.SentDocumentStorageDays = 30;    // 30 Days
            configurationRecord.SentDocumentStorageMBs = 1024;   // 1 GB
            configurationRecord.SentDocumentFolderName = "Sent"; // Sent

            configurationRecord.BackOfficeAllowableConcurrentExecutions = 1;

            // NOTE: If this is updated, consider updating SetConfigValuesForMockCloud() (Libraries\CRE\CloudConnector\Utilities\Sage.CRE.CloudConnector.TestUtilities\TestUtils.cs).
        }

        #endregion
    }
}
