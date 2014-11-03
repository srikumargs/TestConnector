using System;
using System.ServiceModel;
using Sage.Connector.Common;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.ServiceModel;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.NotificationService
{
    /// <summary>
    /// A configuration service for storing and retrieving premise-cloud connectivity attributes
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, ConfigurationName = "NotificationService")]
    public sealed class NotificationService : PublishService<INotificationCallback>, INotificationService
    {
        #region INotificationService Members

        /// <summary>
        /// Occurs when a configuration is added
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the added configuration</param>
        /// <remarks>
        /// It is the callers responsibility to make sure that any data that will be used by the consumers of this message is flushed
        /// before calling this function.
        /// </remarks>
        public void NotifyConfigurationAdded(String tenantId)
        {
            try
            {
                //Note: used to thread.sleep here. Responsibility for this is now on the caller this prevents race conditions, and allows faster processing.
                PublishService<INotificationCallback>.FireEvent("ConfigurationAdded", tenantId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying a configuration added: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a configuration is updated
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the updated configuration</param>
        /// <remarks>
        /// It is the callers responsibility to make sure that any data that will be used by the consumers of this message is flushed
        /// before calling this function.
        /// </remarks>
        public void NotifyConfigurationUpdated(String tenantId)
        {
            try
            {
                //Note: used to thread.sleep here. Responsibility for this is now on the caller this prevents race conditions, and allows faster processing.
                PublishService<INotificationCallback>.FireEvent("ConfigurationUpdated", tenantId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying a configuration updated: " + ex.Message);
            }
        }



        /// <summary>
        /// Occurs when a configuration is deleted
        /// </summary>
        /// <param name="tenantId">The CloudTenantId of the deleted configuration</param>
        /// <remarks>
        /// It is the callers responsibility to make sure that any data that will be used by the consumers of this message is flushed
        /// before calling this function.
        /// </remarks>
        public void NotifyConfigurationDeleted(String tenantId)
        {
            try
            {
                //Note: used to thread.sleep here. Responsibility for this is now on the caller this prevents race conditions, and allows faster processing.
                PublishService<INotificationCallback>.FireEvent("ConfigurationDeleted", tenantId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying a configuration deleted: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a request message availability connection(s) needs to be re-established
        /// </summary>
        public void NotifyRestoreRequestMessageAvailableConnections()
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("RestoreRequestMessageAvailableConnections");
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of restore message available connection: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a request message is available for retrieval
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        public void NotifyRequestMessageAvailable(String tenantId, Guid messageId)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("RequestMessageAvailable", tenantId, messageId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of message available: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a message is enqueued
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        /// <remarks>
        /// It is the callers responsibility to make sure that any data that will be used by the consumers of this message is flushed
        /// before calling this function.
        /// </remarks>
        public void NotifyMessageEnqueued(QueueTypes queueType, String tenantId, Guid messageId)
        {
            try
            {
                //Note: used to thread.sleep here. Responsibility for this is now on the caller this prevents race conditions, and allows faster processing.
                PublishService<INotificationCallback>.FireEvent("MessageEnqueued", queueType, tenantId, messageId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of message added: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a binder element is enqueued
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementEnqueued(String tenantId, String elementId)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("BinderElementEnqueued", tenantId, elementId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of a work message added: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a binder element is deleted
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementDeleted(String tenantId, String elementId)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("BinderElementDeleted", tenantId, elementId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of a work message deleted: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a binder element is restored
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementRestored(String tenantId, String elementId)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("BinderElementRestored", tenantId, elementId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of a work message restored: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a binder element task is completed
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void NotifyBinderElementCompleted(String tenantId, String elementId)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("BinderElementCompleted", tenantId, elementId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of work completed: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when the config params are updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void NotifyConfigParamsUpdated(String tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("ConfigParamsUpdated", tenantId, configParams);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch(Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of a configuration parameters updated: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when the config params are obtained by the messaging service.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="configParams"></param>
        public void NotifyConfigParamsObtained(String tenantId, CloudContracts.WebAPI.Configuration configParams)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("ConfigParamsObtained", tenantId, configParams);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of a configuration parameters obtained: " + ex.Message);
            }
        }


        /// <summary>
        /// Occurs when the service info is updated externally
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="siteAddressBaseUri"></param>
        public void NotifyServiceInfoUpdated(String tenantId, Uri siteAddressBaseUri)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("ServiceInfoUpdated", tenantId, siteAddressBaseUri);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch(Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of service information updated: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a new tenant thread context is needed
        /// </summary>
        /// <param name="tenantId"></param>
        public void NotifyTenantRestart(String tenantId)
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("TenantRestart", tenantId);
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of tenant restart: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when a IncompatibleClient fault occurs
        /// </summary>
        public void NotifyIncompatibleClient()
        {
            try
            {
                PublishService<INotificationCallback>.FireEvent("IncompatibleClient");
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.NotificationService);
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Error notifying of tenant restart: " + ex.Message);
            }
        }
        #endregion
    }
}
