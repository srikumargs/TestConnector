using System;
using System.ServiceModel;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Interfaces;
using Sage.Connector.ConfigurationService.Internal;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.Utilities;

namespace Sage.Connector.ConfigurationService
{
    /// <summary>
    /// A configuration service for storing and retrieving premise-cloud connectivity attributes
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, ConfigurationName = "ConfigurationService")]
    public sealed class ConfigurationService : IConfigurationService
    {
        /// <summary>
        /// Initialize a new instance of the configuration service
        /// </summary>
        public ConfigurationService()
        {
            using (new StackTraceContext(this))
            {
                // Need to set the database repairer method with each instance
                // Since this service is not a singleton
                InitRetryManagerDatabaseRepairMethod();
            }
        }

        #region IConfigurationService Members

        /// <summary>
        /// Retrieves existing configurations
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord[] GetConfigurations()
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            PremiseConfigurationRecord[] result = null;

            try
            {
                using (var lm = new LogManager())
                {
                    PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                    result = factory.GetEntries();
                }
                
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.ConfigurationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "GetConfigurations";
                dafe.Problem = "Error retrieving configurations: " + ex.Message;

                SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                    Subsystem.ConfigurationService,
                    ex.ExceptionAsString(),
                    dafe.Problem);

                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }

            return result;
        }

        /// <summary>
        /// Creates a new in-memory configuration
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord CreateNewConfiguration()
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            PremiseConfigurationRecord result = null;

            try
            {
                using (var lm = new LogManager())
                {
                    PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                    result = factory.CreateNewEntry();
                }
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.ConfigurationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "CreateNewConfiguration";
                dafe.Problem = "Error creating a new configuration: " + ex.Message;

                SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                    Subsystem.ConfigurationService,
                    ex.ExceptionAsString(),
                    dafe.Problem);

                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }

            return result;
        }

        /// <summary>
        /// Adds the supplied configuration to the database
        /// </summary>
        /// <param name="newConfiguration"></param>
        public void AddConfiguration(PremiseConfigurationRecord newConfiguration)
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            try
            {
                using (var lm = new LogManager())
                {
                    PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                    factory.AddEntry(newConfiguration);
                }

                UpdateIntegratedConnectionState(newConfiguration);

                // notify subscribers
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    //NotifyConfigurationAdded requires that any data its recipients need is persisted before calling. 
                    //We used to have a Thread.Sleep(1000) to ensure database flushed and ready for consumers.
                    //However it does not appear this is needed anymore for this path. All use of this should be
                    //from sub services in core, preventing multiprocess flushing issues.

                    proxy.NotifyConfigurationAdded(newConfiguration.CloudTenantId);
                }
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.ConfigurationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "AddConfiguration";
                dafe.Problem = "Error adding a configuration: " + ex.Message;

                SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                    Subsystem.ConfigurationService,
                    ex.ExceptionAsString(),
                    dafe.Problem);

                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }
        }

        /// <summary>
        /// Deletes the specified configuration from the database
        /// </summary>
        /// <param name="tenantId"></param>
        public void DeleteConfiguration(string tenantId)
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            try
            {
                using (var lm = new LogManager())
                {
                    PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                    factory.DeleteEntryByTenantId(tenantId);
                }
               
                // notify subscribers
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    //NotifyConfigurationDeleted requires that any data its recipients need is persisted before calling. 
                    //We used to have a Thread.Sleep(1000) to ensure database flushed and ready for consumers.
                    //However it does not appear this is needed anymore for this path. All use of this should be
                    //from sub services in core, preventing multiprocess flushing issues.

                    proxy.NotifyConfigurationDeleted(tenantId);
                }
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.ConfigurationService);

                RemoveIntegratedConnectionState(tenantId);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "DeleteConfiguration";
                dafe.Problem = "Error deleting a configuration: " + ex.Message;

                SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                    Subsystem.ConfigurationService,
                    ex.ExceptionAsString(),
                    dafe.Problem);

                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }
        }

        /// <summary>
        /// Retrieves the specified configuration from the database
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public PremiseConfigurationRecord GetConfiguration(string tenantId)
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            PremiseConfigurationRecord result = null;

            try
            {
                using (var lm = new LogManager())
                {
                    PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                    result = factory.GetEntryByTenantId(tenantId);
                }
                
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.ConfigurationService);    
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "DeleteConfiguration";
                dafe.Problem = "Error retrieving a configuration: " + ex.Message;

                SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                    Subsystem.ConfigurationService,
                    ex.ExceptionAsString(),
                    dafe.Problem);

                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }

            return result;
        }

        /// <summary>
        /// Updates the supplied configuration in the database
        /// </summary>
        /// <param name="updatedConfiguration"></param>
        public void UpdateConfiguration(PremiseConfigurationRecord updatedConfiguration)
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            try
            {
                using (var lm = new LogManager())
                {
                    PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                    factory.UpdateEntry(updatedConfiguration);
                }

                //State service needs to be updated before other subscribers.
                UpdateIntegratedConnectionState(updatedConfiguration);

                // notify subscribers
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    //NotifyConfigurationUpdated requires that any data its recipients need is persisted before calling. 
                    //We used to have a Thread.Sleep(1000) to ensure database flushed and ready for consumers.
                    //However it does not appear this is needed anymore for this path. All use of this should be
                    //from sub services in core, preventing multiprocess flushing issues.
                    proxy.NotifyConfigurationUpdated(updatedConfiguration.CloudTenantId);
                }
                
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.ConfigurationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "UpdateConfiguration";
                dafe.Problem = "Error updating a configuration: " + ex.Message;

                SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                    Subsystem.ConfigurationService,
                    ex.ExceptionAsString(),
                    dafe.Problem);

                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }
        }

        /// <summary>
        /// Retrieves LogEntries
        /// </summary>
        /// <returns></returns>
        public LogEntryRecord[] GetLogEntries()
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            LogEntryRecord[] result = null;

            try
            {
                LogEntryRecordFactory factory = LogEntryRecordFactory.Create();
                result = factory.GetEntries();
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "GetQueueEntries";
                dafe.Problem = "Error retrieving log entries: " + ex.Message;
                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }

            return result;
        }

        /// <summary>
        /// Retrieves QueueEntries
        /// </summary>
        /// <returns></returns>
        public QueueEntryRecord[] GetQueueEntries()
        {
            AuthorizationHelper.AuthorizeClientPrimaryIdentity();

            QueueEntryRecord[] result = null;

            try
            {
                using (var lm = new LogManager())
                {
                    QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                    result = factory.GetEntries();
                }
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.Queues); 
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }

                DataAccessFaultException dafe = new DataAccessFaultException();
                dafe.Operation = "GetQueueEntries";
                dafe.Problem = "Error retrieving queue entries: " + ex.Message;

                SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                    Subsystem.Queues,
                    ex.ExceptionAsString(),
                    dafe.Problem);

                throw new FaultException<DataAccessFaultException>(dafe, new FaultReason(dafe.Problem));
            }

            return result;
        }

        /// <summary>
        /// Updates the supplied tenant state with updated PCR information
        /// </summary>
        /// <param name="pcr"></param>
        /// <returns></returns>
        private void UpdateIntegratedConnectionState(PremiseConfigurationRecord pcr)
        {
            if (null != pcr)
            {
                IntegrationEnabledStatus updatedIntegrationStatus = IntegrationEnabledStatus.None;
                if (pcr.CloudConnectionEnabledToReceive)
                {
                    updatedIntegrationStatus |= IntegrationEnabledStatus.CloudGetRequests;
                }

                if (pcr.CloudConnectionEnabledToSend)
                {
                    updatedIntegrationStatus |= IntegrationEnabledStatus.CloudPutResponses;
                }

                if (pcr.BackOfficeConnectionEnabledToReceive)
                {
                    updatedIntegrationStatus |= IntegrationEnabledStatus.BackOfficeProcessing;
                }

                Uri cloudEndPoint = null;
                Uri.TryCreate(pcr.SiteAddress, UriKind.Absolute, out cloudEndPoint);

                UpdateIntegratedConnectionState(pcr.CloudTenantId, pcr.CloudCompanyName, pcr.BackOfficeCompanyName, cloudEndPoint, updatedIntegrationStatus, pcr.ConnectorPluginId, pcr.BackOfficeProductName);

                // If we are disabling receive, clear any lingering 'next' communication time
                if (!pcr.CloudConnectionEnabledToReceive)
                {
                    UpdateNextScheduledCommunicationWithCloud(pcr.CloudTenantId, DateTime.MinValue);
                }
            }
        }

        /// <summary>
        /// Sets the tenant state with the supplied updated tenant state
        /// Note: Do NOT make this an async operation.  We want all callers of this method (add/update config)
        /// To guarantee that the state service is also up to date when they complete.
        /// </summary>
        private void UpdateIntegratedConnectionState(String tenantId, String tenantName, String backOfficeCompanyName, Uri tenantUri, IntegrationEnabledStatus integrationEnabledStatus, String connectorPluginId, String backOfficeProductName)
        {
            try
            {
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                    "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    var backOfficeProductPluginInfo = proxy.GetBackOfficePluginInformation(connectorPluginId, backOfficeProductName);
                    proxy.UpdateIntegratedConnectionState(tenantId, tenantName, backOfficeCompanyName, tenantUri, integrationEnabledStatus, backOfficeProductPluginInfo);
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Set the next communication time with the cloud
        /// Note: Do NOT make this an async operation.  We want all callers of this method (add/update config)
        /// To guarantee that the state service is also up to date when they complete.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="value"></param>
        private void UpdateNextScheduledCommunicationWithCloud(String tenantId, DateTime value)
        {
            try
            {
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                    "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.UpdateNextScheduledCommunicationWithCloud(tenantId, value);
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Delete the connection for this tenant
        /// Note: Do NOT make this an async operation.  We want all callers of this method (delete config)
        /// To guarantee that the state service is also up to date when they complete.
        /// </summary>
        /// <param name="tenantId"></param>
        private void RemoveIntegratedConnectionState(string tenantId)
        {
            try
            {
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                    "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.RemoveIntegratedConnectionState(tenantId);
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Init the retry policy manager's database repairer method externally
        /// </summary>
        private void InitRetryManagerDatabaseRepairMethod()
        {
            RetryPolicyManager.SetDatabaseRepairMethod(DatabaseRepairUtils.RepairDatabaseCoordinator);
        }

        #endregion

        #region Private fields
        private static readonly String _myTypeName = typeof(ConfigurationService).FullName;
        #endregion
    }
}
