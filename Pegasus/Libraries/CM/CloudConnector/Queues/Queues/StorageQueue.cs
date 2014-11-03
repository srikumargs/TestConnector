using System;
using System.Collections.Generic;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Documents;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Interfaces;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;
using Sage.ExtensionMethods;

namespace Sage.Connector.Queues
{
    /// <summary>
    /// Persistent/durable queue management
    /// </summary>
    public class StorageQueue
    {
        #region Private fields

        private static bool _createdNew;
        private static readonly Mutex _lock = new Mutex(false, "StorageQueue", out _createdNew, Utils.AllowEveryoneMutexSecurity);
        private readonly string _queueName = string.Empty;
        private readonly TimeSpan _waitTimeout = ConnectorRegistryUtils.StorageQueueMutexWaitTimeout;
        private double _processingTimeout = ConnectorRegistryUtils.StorageQueueProcessingTimeout;
        private static readonly String _myTypeName = typeof(StorageQueue).FullName;

        #endregion

        #region Private methods

        /// <summary>
        /// Attempts to aquire a lock on the mutex.
        /// </summary>
        /// <returns>True if the lock is aquired, otherwise false.</returns>
        private bool TryLock()
        {
            try
            {
                return _lock.WaitOne(_waitTimeout);
            }
            catch (AbandonedMutexException)
            {
                return true;
            }
        }

        /// <summary>
        /// Release the lock on the mutex.
        /// </summary>
        private void Unlock()
        {
            _lock.ReleaseMutex();        
        }

        /// <summary>
        /// Write error message to log.
        /// </summary>
        /// <param name="message">The error message to write.</param>
        private void WriteErrorForGeneral(String message)
        {
            using (var lm = new LogManager())
            {
                lm.WriteError(this, message);
            }
        }

        /// <summary>
        /// Write info message to log.
        /// </summary>
        /// <param name="message">The info message to write.</param>
        private void WriteTraceForGeneral(String message)
        {
            using (var lm = new LogManager())
            {
                lm.WriteInfo(this, message);
            }
        }

        /// <summary>
        /// Write info for tracking context.
        /// </summary>
        /// <param name="trackingContext">The tracking context.</param>
        /// <param name="message">The info message to write.</param>
        private void WriteTraceForRequest(ActivityTrackingContext trackingContext, String message)
        {
            using (var lm = new LogManager())
            {
                lm.WriteInfoForRequest(this, trackingContext, message);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The queue name.</param>
        public StorageQueue(string name)
        {
            ArgumentValidator.ValidateNonEmptyString(name, "name", _myTypeName + ".ctor()");

            _queueName = name;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Retrieves a specific queue entry
        /// </summary>
        /// <param name="queueId">The id of the queue element to retrieve.</param>
        /// <returns>The storage queue message on success, otherwise null.</returns>
        public StorageQueueMessage GetQueueElement(Guid queueId)
        {
            QueueEntryRecord entry = null;

            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                if (factory == null) return null;

                try
                {
                    Action retryAction = () =>
                    {
                        if (TryLock())
                        {
                            try
                            {
                                entry = factory.GetEntry(queueId);
                                if ((null != entry) && (Guid.Empty != entry.QueueId))
                                {
                                    entry.Content = DocumentManager.RetrieveContent(queueId.ToString());
                                }

                            }
                            finally
                            {
                                Unlock();
                            }
                        }
                        else
                        {
                            throw new MutexTimeoutExeption();
                        }

                    };

                    RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                }
                catch (Exception ex)
                {
                    WriteErrorForGeneral(ex.Message);
                }
            }

            return (entry == null) ? null : new StorageQueueMessage(entry.QueueId.ToString(), entry.PayloadType, entry.Content);
        }

        /// <summary>
        /// Renames our current queue type to the supplied new type name
        /// </summary>
        /// <param name="newTypeName">The new type name to rename the queue to.</param>
        public void RenameQueueType(String newTypeName)
        {
            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);

                if (factory == null) return;
                    
                    try
                    {
                        Action retryAction = () =>
                            {
                                if (TryLock())
                                {
                                    try
                                    {
                                        factory.RenameQueueType(_queueName, newTypeName);
                                    }
                                    finally
                                    {
                                        Unlock();
                                    }
                                }
                                else
                                {
                                    throw new MutexTimeoutExeption();
                                }
                            };

                        RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                    }
                    catch (MutexTimeoutExeption)
                    {
                        WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.RenameQueueType()");
                    }
                }
            }

        /// <summary>
        /// Retrieves and renames up to 'MaxEntries' of entries (renames retrieved entries to supplied 'NewTypeName').
        /// </summary>
        /// <param name="newTypeName">The new type name.</param>
        /// <param name="maxEntries">Maximum number of entries to obtain and rename.</param>
        /// <returns>The list of obtained queue entries.</returns>
        public List<StorageQueueMessage> GetAndRenameOrderedEntries(String newTypeName, int maxEntries)
        {
            List<StorageQueueMessage> retList = new List<StorageQueueMessage>();

            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                if (factory == null) return retList;

                    try
                    {
                        Action retryAction = () =>
                            {
                                if (TryLock())
                                {
                                    try
                                    {
                                        for (int iCount = 0; iCount < maxEntries; iCount++)
                                        {
                                            QueueEntryRecord qer = factory.GetAndRenameFirst(_queueName, newTypeName);

                                            if ((null != qer) && (Guid.Empty != qer.QueueId))
                                            {
                                                qer.Content = DocumentManager.RetrieveContent(qer.QueueId.ToString());
                                            }
                                            if (null == qer) break;
                                            retList.Add(new StorageQueueMessage(qer.QueueId.ToString(), qer.PayloadType, qer.Content));
                                        }
                                    }
                                    finally
                                    {
                                        Unlock();
                                    }
                                }
                                else
                                {
                                    throw new MutexTimeoutExeption();
                                }
                            };

                        RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                    }
                    catch (MutexTimeoutExeption)
                    {
                        WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.GetAndRenameOrderedEntries()");
                    }
                }

            return retList;
        }

        /// <summary>
        /// Restores a retrieved queue entry back onto the queue.
        /// </summary>
        /// <param name="queueId">The identifier for the queue entry to restore.</param>
        public void RestoreQueue(Guid queueId)
        {
            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);

                if (factory == null) return;

                try
                {
                    Action retryAction = () =>
                    {
                        if (TryLock())
                        {
                            try
                            {
                                factory.ClearProcessingTimeout(queueId);
                            }
                            finally
                            {
                                Unlock();
                            }
                        }
                        else
                        {
                            throw new MutexTimeoutExeption();
                        }
                    };

                    RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                }
                catch (MutexTimeoutExeption)
                {
                    WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.RestoreQueue()");
                }

                // notify subscribers
                var entry = factory.GetEntry(queueId);
                if (null != entry)
                {
                    String tenantId;
                    QueueTypes queueType;
                    QueueManager.DecodeQueueName(entry.Type, out tenantId, out queueType);
                    try
                    {
                        using (
                            var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost",
                                ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            //NotifyMessageEnqueued requires that any data its recipients need is persisted before calling. 
                            //We used to have a Thread.Sleep(1000) to ensure database flushed and ready for consumers.
                            //However it does not appear this is needed anymore for this path. All use of this should be
                            //from sub services in core, preventing multiprocess flushing issues.
                            proxy.NotifyMessageEnqueued(queueType, tenantId, queueId);
                        }
                    }
                    catch (Exception ex)
                    {
                        SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService,
                            ex.ExceptionAsString(), "Unable to notify of message addition: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Restores all previously hidden queue entries.
        /// </summary>
        public void RestoreAllHiddenMessages()
        {
            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                if (factory == null) return;

                try
                {
                    Action retryAction = () =>
                        {
                            if (TryLock())
                            {
                                try
                                {
                                    factory.ClearAllProcessingTimeout();
                                }
                                finally
                                {
                                    Unlock();
                                }
                            }
                            else
                            {
                                throw new MutexTimeoutExeption();
                            }
                        };

                    RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                }
                catch (MutexTimeoutExeption)
                {
                    WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.RestoreAllHiddenMessages()");
                }
            }
        }

        /// <summary>
        /// Clears all remaining elements of the queue.
        /// </summary>
        public void Clear()
        {
            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                if (factory == null) return;

                try
                {
                    Action retryAction = () =>
                    {
                        if (TryLock())
                        {
                            try
                            {
                                var deletedEntryIds = factory.DeleteTypedEntry(_queueName);
                                deletedEntryIds.ForEach(id => DocumentManager.DeleteContent(id.ToString()));
                            }
                            finally
                            {
                                Unlock();
                            }
                        }
                        else
                        {
                            throw new MutexTimeoutExeption();
                        }
                    };

                    RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                }
                catch (MutexTimeoutExeption)
                {
                    WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.RestoreAllHiddenMessages()");
                }
            }
        }

        /// <summary>
        /// Delete a specific queue message.
        /// </summary>
        /// <param name="message">The storage queue message to delete.</param>
        public bool Delete(StorageQueueMessage message)
        {
            bool bDeleteResults = false;

            if ((message == null) || (message.Id == null)) return false;

            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                if (factory == null) return false;

                try
                {
                    Action retryAction = () =>
                        {
                            if (TryLock())
                            {
                                try
                                {
                                    bDeleteResults = factory.DeleteEntry(Guid.Parse(message.Id));
                                    if (bDeleteResults)
                                    {
                                        DocumentManager.DeleteContent(message.Id);
                                    }
                                    WriteTraceForGeneral(String.Format("StorageQueue.Delete: {0}, {1}", message.Id, _queueName));
                                }
                                finally
                                {
                                    Unlock();
                                }
                            }
                            else
                            {
                                throw new MutexTimeoutExeption();
                            }
                        };

                    RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                }
                catch (MutexTimeoutExeption)
                {
                    WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.Delete()");
                }
            }

            return bDeleteResults;
        }

        /// <summary>
        /// Returns a queue message (null if not storage queue message, but will  remove it from the
        /// queue, also null if empty).
        /// </summary>
        /// <returns>The first storage queue message, or null if no messages.</returns>
        public StorageQueueMessage Dequeue()
        {
            return Dequeue(DequeueProcessingExpiration);
        }

        /// <summary>
        /// Returns a queue message (null if not storage queue message, but will remove it from the 
        /// queue, also null if empty).
        /// </summary>
        /// <param name="processingInterval">The processing interval.</param>
        /// <returns>The first storage queue message, or null if no messages.</returns>
        public StorageQueueMessage Dequeue(double processingInterval)
        {
            QueueEntryRecord entry = null;

            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                if (factory == null) return null;

                try
                {
                    Action retryAction = () =>
                        {
                            if (TryLock())
                            {
                                try
                                {
                                    entry = factory.GetFirst(_queueName, processingInterval);

                                    if (entry != null)
                                    {
                                        if (!entry.QueueId.Equals(Guid.Empty))
                                        {
                                            entry.Content = DocumentManager.RetrieveContent(entry.QueueId.ToString());
                                        }
                                        WriteTraceForGeneral(String.Format("StorageQueue.Dequeue: {0}, {1}", entry.QueueId, _queueName));
                                    }
                                }
                                finally
                                {
                                    Unlock();
                                }
                            }
                            else
                            {
                                throw new MutexTimeoutExeption();
                            }
                        };

                    RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                }
                catch (MutexTimeoutExeption)
                {
                    WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.Dequeue()");
                }
            }

            return (entry == null) ? null : new StorageQueueMessage(entry.QueueId.ToString(), entry.PayloadType, entry.Content);
        }

        /// <summary>
        /// Adds the given storage queue message to the queue (callers should handle 
        /// QueueEntryRecordEnqueueException).
        /// </summary>
        /// <param name="message">The message to add to the queue.</param>
        /// <param name="context">The queue context.</param>
        /// <param name="removeExisting">True if this is an update (remove/insert), otherwise false.</param>
        public void Enqueue(StorageQueueMessage message, QueueContext context, bool removeExisting = false)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", string.Format("{0}.Enqueue()", _myTypeName));

            if ((message == null) || (message.Payload == null)) return;

            using (var lm = new LogManager())
            {
                QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                if (factory == null) return;

                QueueEntryRecord newEntry = factory.CreateNewEntry();
                Guid suppliedGuid;

                if (!Guid.TryParse(message.Id, out suppliedGuid))
                {
                    suppliedGuid = Guid.Empty;
                }

                newEntry.QueueId = suppliedGuid;
                newEntry.PayloadType = message.PayloadType;
                newEntry.Type = _queueName;

                try
                {
                    Action retryAction = () =>
                        {
                            if (TryLock())
                            {
                                try
                                {
                                    var identifier = factory.AddEntry(newEntry, removeExisting);

                                    if (!identifier.Equals(Guid.Empty))
                                    {
                                        if (removeExisting)
                                        {
                                            DocumentManager.DeleteContent(identifier.ToString());
                                        }
                                        DocumentManager.StoreContent(identifier.ToString(), message.Payload);
                                    }
                                    WriteTraceForRequest(context.ActivityTrackingContext, String.Format("StorageQueue.Enqueue: {0}, {1}", message.Id, _queueName));
                                    
                                    // ReSharper disable once AccessToDisposedClosure
                                    lm.AdvanceActivityState(this, context.ActivityTrackingContext, context.NewState, context.NewStatus);

                                }
                                finally
                                {
                                    Unlock();
                                }
                            }
                            else
                            {
                                throw new MutexTimeoutExeption();
                            }
                        };

                    RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                }
                catch (MutexTimeoutExeption)
                {
                    WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.Enqueue()");
                }

                String tenantId;
                QueueTypes queueType;
                QueueManager.DecodeQueueName(_queueName, out tenantId, out queueType);

                try
                {
                    using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        //NotifyMessageEnqueued requires that any data its recipients need is persisted before calling. 
                        //We used to have a Thread.Sleep(1000) to ensure database flushed and ready for consumers.
                        //However it does not appear this is needed anymore for this path. All use of this should be
                        //from sub services in core service, preventing multiprocess flushing issues.
                        proxy.NotifyMessageEnqueued(queueType, tenantId, newEntry.QueueId);
                    }
                }
                catch (Exception ex)
                {
                    SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Unable to notify of message addition: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Adds the given message as a queue message to the queue,
        /// </summary>
        /// <param name="message">The message to add to the queue.</param>
        /// <param name="messageType">The type of message to add.</param>
        /// <param name="context">>The queue context.</param>
        /// <param name="messageId">The identifier for the message.</param>
        /// <param name="removeExisting">True if this is an update (remove/insert), otherwise false.</param>
        public void Enqueue(string message, string messageType, QueueContext context, string messageId = null, bool removeExisting = false)
        {
            Enqueue(new StorageQueueMessage(messageId, messageType, message), context, removeExisting);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the element count of the queue.
        /// </summary>
        public int Count
        {
            get
            {
                int retCount = 0;

                using (var lm = new LogManager())
                {
                    QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(lm);
                    if (factory == null) return 0;

                    try
                    {
                        Action retryAction = () =>
                        {
                            if (TryLock())
                            {
                                try
                                {
                                    retCount = factory.Count(_queueName);
                                }
                                finally
                                {
                                    Unlock();
                                }
                            }
                            else
                            {
                                throw new MutexTimeoutExeption();
                            }
                        };

                        RetryPolicyManager.ExecuteInRetry(RetryPurpose.QueueStore, retryAction, null);
                    }
                    catch (MutexTimeoutExeption)
                    {
                        WriteErrorForGeneral("Timeout waiting for mutex in StorageQueue.Count");
                    }
                }

                return retCount;
            }
        }

        /// <summary>
        /// The interval whereby a dequeue message will be hidden until it re-appears on the queue (client needs 
        /// to delete the message before the timeout).
        /// </summary>
        public double DequeueProcessingExpiration
        {
            get
            {
                return _processingTimeout;
            }

            set
            {
                _processingTimeout = value;
            }
        }

        #endregion
    }
}
