using System;
using System.Collections.Generic;
using Sage.Connector.Data;
using Sage.Connector.Documents;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Interfaces;
using Sage.Diagnostics;

namespace Sage.Connector.Queues
{
    /// <summary>
    /// A queuing service for message storage and retrieval
    /// </summary>
    public sealed class QueueManager : IDisposable
    {
        /// <summary>
        /// Converts the QueueType enum into an "name" we use for the queue
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        private static string GetEncodedQueueName(QueueTypes type, String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".GetQueueName()");

            // Make up a name based on the tenant id and queue type
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}_{1}", tenantId, type);
        }

        /// <summary>
        /// Decodes the queue name back to its constituant tenant Id and queue type components
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="tenantId"></param>
        /// <param name="queueType"></param>
        internal static void DecodeQueueName(String queueName, out String tenantId, out QueueTypes queueType)
        {
            ArgumentValidator.ValidateNonEmptyString(queueName, "queueName", _myTypeName + ".DecodeQueueName()");

            queueType = QueueTypes.None;

            String[] splitQueueName = queueName.Split('_');
            tenantId = splitQueueName[0];
            if (splitQueueName.Length == 2)
            {
                queueType = (QueueTypes)Enum.Parse(typeof(QueueTypes), splitQueueName[1], true);
            }
        }
        
        /// <summary>
        /// Unified factory retrieval
        /// </summary>
        /// <returns></returns>
        private static StorageQueueFactory GetQueueFactory()
        {
            return new StorageQueueFactory();
        }

        /// <summary>
        /// Unified inbox queue retrieval
        /// </summary>
        /// <returns></returns>
        /// <param name="tenantId"></param>
        private StorageQueue GetInboxQueue(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".GetInboxQueue()");

            var factory = GetQueueFactory();
            return factory.GetQueue(GetEncodedQueueName(QueueTypes.Inbox, tenantId));
        }

        /// <summary>
        /// Unified outbox queue retrieval
        /// </summary>
        /// <returns></returns>
        private StorageQueue GetOutboxQueue(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".GetOutboxQueue()");

            var factory = GetQueueFactory();
            return factory.GetQueue(GetEncodedQueueName(QueueTypes.Outbox, tenantId)) as StorageQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private StorageQueue GetOutboxPendingDeletionQueue(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".GetOutboxPendingDeletionQueue()");

            var factory = GetQueueFactory();
            return factory.GetQueue(GetEncodedQueueName(QueueTypes.OutboxPendingDeletion, tenantId)) as StorageQueue;
        }

        /// <summary>
        /// Retrives a non-specific queue for custom queue operations
        /// </summary>
        /// <returns></returns>
        private StorageQueue GetNonSpecificQueue()
        {
            var factory = GetQueueFactory();
            return factory.GetQueue(GetEncodedQueueName(QueueTypes.None, ALL_TENANTS));
        }

        /// <summary>
        /// Adds a message to the input queue
        /// </summary>
        /// <param name="sInputMessage"></param>
        /// <param name="context"></param>
        /// <param name="messageType"></param>
        /// <param name="messageId"></param>
        public void AddMessageToInput(string sInputMessage, string messageType, QueueContext context, string messageId = null)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", _myTypeName + ".AddMessageToInput()");
            ArgumentValidator.ValidateNonNullReference(context.ActivityTrackingContext, "context.ActivityTrackingContext", _myTypeName + ".AddMessageToInput()");
            ArgumentValidator.ValidateNonEmptyString(context.ActivityTrackingContext.TenantId, "context.ActivityTrackingContext.TenantId", _myTypeName + ".AddMessageToInput()");

            // First perform a quick check to see if the tenant still represents a valid configuration
            // Since it could have been deleted during processing of this message
            using (var lm = new LogManager())
            {
                PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);

                if (factory.GetEntryByTenantId(context.ActivityTrackingContext.TenantId) != null)
                {
                    var isq = GetInboxQueue(context.ActivityTrackingContext.TenantId);
                    if (null != isq)
                    {
                        isq.Enqueue(sInputMessage, messageType, context, messageId);
                    }
                }

                else
                {
                    // Log that the message was dropped for a deleted tenant
                    lm.WriteWarningForRequest(this, context.ActivityTrackingContext,
                        "Message dropped for non-existent tenant with Id '{0}': {1}", context.ActivityTrackingContext.TenantId, sInputMessage);
                }
            }
        }

        /// <summary>
        /// Adds a message to the output queue
        /// </summary>
        /// <param name="sOutputMessage"></param>
        /// <param name="messageType"></param>
        /// <param name="context"></param>
        /// <param name="messageId"></param>
        /// <param name="removeExisting"></param>
        public void AddMessageToOutput(string sOutputMessage, string messageType, QueueContext context, string messageId = null, bool removeExisting = false)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", _myTypeName + ".AddMessageToOutput()");
            ArgumentValidator.ValidateNonNullReference(context.ActivityTrackingContext, "context.ActivityTrackingContext", _myTypeName + ".AddMessageToOutput()");
            ArgumentValidator.ValidateNonNullReference(context.ActivityTrackingContext.TenantId, "context.ActivityTrackingContext.TenantId", _myTypeName + ".AddMessageToOutput()");

            // First perform a quick check to see if the tenant still represents a valid configuration
            // Since it could have been deleted during processing of this message
            using (var lm = new LogManager())
            {
                PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                if (factory.GetEntryByTenantId(context.ActivityTrackingContext.TenantId) != null)
                {
                    // Valid tenant, so add the response to the output queue
                    var sq = GetOutboxQueue(context.ActivityTrackingContext.TenantId);
                    if (null != sq)
                    {
                        sq.Enqueue(sOutputMessage, messageType, context, messageId, removeExisting);
                    }
                    else
                    {
                        // Critical error: 
                        // The tenant is valid, but its queue could not be retrieved/created
                        throw new InvalidOperationException(
                            string.Format("Could not retrieve output queue for tenant with Id '{0}'", context.ActivityTrackingContext.TenantId));
                    }
                }

                else
                {
                    // Log that the message was dropped for a deleted tenant
                    lm.WriteWarningForRequest(this, context.ActivityTrackingContext,
                        "Message dropped for non-existent tenant with Id '{0}': {1}", context.ActivityTrackingContext.TenantId, sOutputMessage);
                }
            }
        }

        /// <summary>
        /// Retrieves the top message from the output queue (client should either restore or delete the message)
        /// </summary>
        public StorageQueueMessage GetMessageFromOutput(String tenantId, double dProcessingInterval)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".GetMessageFromOutput()");

            var sq = GetOutboxQueue(tenantId);
            if (null != sq)
            {
                var isqm = sq.Dequeue(dProcessingInterval);
                return isqm;
            }
            return null;
        }

        /// <summary>
        /// Retrieves the top message from the output queue (client should either restore or delete the message)
        /// </summary>
        public StorageQueueMessage GetMessageFromOutput(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".GetMessageFromOutput()");

            var sq = GetOutboxQueue(tenantId);
            if (null != sq)
            {
                var isqm = sq.Dequeue();
                return isqm;
            }
            return null;
        }

        /// <summary>
        /// Restores a previously dequeued message
        /// </summary>
        /// <param name="QueueId"></param>
        public void RestoreQueueMessage(string QueueId)
        {
            var sq = GetNonSpecificQueue();
            if (null != sq)
            {
                sq.RestoreQueue(new Guid(QueueId));
            }
        }

        /// <summary>
        /// Restores all previously dequeued messages
        /// </summary>
        public void RestoreAllProcessingMessages()
        {
            var sq = GetNonSpecificQueue();
            if (null != sq)
            {
                sq.RestoreAllHiddenMessages();
            }
        }

        /// <summary>
        /// Retrieves up to MaxMessages from the output
        /// (temporarily moved to output-peek queue)
        /// </summary>
        /// <param name="MaxMessages"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public string[] PeekMessagesFromOutput(int MaxMessages, String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".PeekMessagesFromOutput()");

            List<string> retList = new List<string>();

            var outbox = GetOutboxQueue(tenantId);
            if (null != outbox)
            {
                var messages = outbox.GetAndRenameOrderedEntries(
                    GetEncodedQueueName(QueueTypes.OutboxPendingDeletion, tenantId),
                    MaxMessages);
                foreach (var message in messages)
                {
                    retList.Add(message.Payload);
                }
            }

            return retList.ToArray();
        }

        /// <summary>
        /// Removes all messages in output-peek queue
        /// </summary>
        public void RemoveLastPeekMessagesFromOutput(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".RemoveLastPeekMessagesFromOutput()");

            var sq = GetOutboxPendingDeletionQueue(tenantId);
            if (null != sq)
            {
                sq.Clear();
            }
        }

        /// <summary>
        /// Restores all output-peek queue messages to output
        /// </summary>
        public void RestorePeekMessagesToOutput(String tenantId)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".RestorePeekMessagesToOutput()");

            var sq = GetOutboxPendingDeletionQueue(tenantId);
            if (null != sq)
            {
                sq.RenameQueueType(GetEncodedQueueName(QueueTypes.Outbox, tenantId));
            }
        }

        /// <summary>
        /// Retrieves the top message from the input queue
        /// </summary>
        public StorageQueueMessage GetMessageFromInput(double dProcessingInterval, string tenantId)
        {
            var isq = GetInboxQueue(tenantId);
            if (null != isq)
            {
                var isqm = isq.Dequeue(dProcessingInterval);
                if (null != isqm)
                {
                    return isqm;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the top message from the input queue
        /// </summary>
        public StorageQueueMessage GetMessageFromInput(string tenantId)
        {
            var isq = GetInboxQueue(tenantId);
            if (null != isq)
            {
                var isqm = isq.Dequeue();
                if (null != isqm)
                {
                    return isqm;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves a specific message from the queue
        /// </summary>
        /// <param name="messageIdentifier"></param>
        /// <returns></returns>
        public string GetSpecificMessage(string messageIdentifier)
        {
            var content = DocumentManager.RetrieveContent(messageIdentifier);

            return (String.IsNullOrEmpty(content)) ? null : content;
        }

        /// <summary>
        /// Removes a specific message from the binder queue
        /// </summary>
        /// <param name="messageIdentifier"></param>
        public bool RemoveSpecificMessage(String messageIdentifier)
        {
            var isq = GetNonSpecificQueue();
            if (null != isq)
            {
                // Shortcut retrieval
                //isq.GetQueueElement(BinderIdentifier);
                StorageQueueMessage sqm = new StorageQueueMessage(messageIdentifier, String.Empty, String.Empty);
                return isq.Delete(sqm);
            }

            return false;
        }


        /// <summary>
        /// Deletes all queue messages for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        public void DeleteTenantMessages(string tenantId)
        {
            // Note: Input and Binder messages (i.e. those currently in process) for this tenant 
            // Will be allowed to complete, but the responses (error or otherwise) 
            // Will not be added to the output queue
            StorageQueue sq_inbox = GetInboxQueue(tenantId);
            sq_inbox.Clear();

            StorageQueue sq_outbox_pending = GetOutboxPendingDeletionQueue(tenantId);
            sq_outbox_pending.Clear();

            StorageQueue sq_outbox = GetOutboxQueue(tenantId);
            sq_outbox.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        { }

        private static readonly String _myTypeName = typeof(QueueManager).FullName;

        // some queues are currently not tenant-specific because the components which are the consumers of these
        // queues don't care (e.g., Dispatcher & the Inbox queue, Binders & the Binder queue)
        private static readonly String ALL_TENANTS = "ALL";
    }
}
