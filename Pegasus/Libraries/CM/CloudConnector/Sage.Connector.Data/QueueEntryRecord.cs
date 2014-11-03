using System;
using QueueStore = Sage.Connector.PremiseStore.QueueStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// An encapsulated record for queue entries that wraps data access object
    /// </summary>
    [Serializable]
    public class QueueEntryRecord
    {
        private QueueStore.QueueEntry _entry = null;
        /// <summary>
        /// Constructs a record object from an entity object
        /// </summary>
        /// <param name="qe"></param>
        internal QueueEntryRecord(QueueStore.QueueEntry qe)
        {
            _entry = qe;
        }

        /// <summary>
        /// Internal retrieval of entity object for internal updates
        /// </summary>
        /// <returns></returns>
        internal QueueStore.QueueEntry GetInternalQueueEntry()
        {
            return _entry;
        }

        /// <summary>
        /// The database unique identifier for the queue, sets after saves
        /// </summary>
        public Guid QueueId
        {
            get { return _entry.QueueId; }
            set { _entry.QueueId = value; }
        }

        /// <summary>
        /// The date and time the queue entry is created
        /// </summary>
        public DateTime DateTimeUtc
        {
            get { return _entry.DateTimeUtc; }
            set { _entry.DateTimeUtc = value; }
        }

        /// <summary>
        /// The type of payload/content
        /// </summary>
        public String PayloadType
        {
            get { return _entry.Content; }
            set { _entry.Content = value; }
        }


        private string _content;

        /// <summary>
        /// The content of the queue
        /// </summary>
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// The machine user that created the queue
        /// </summary>
        public string User
        {
            get { return _entry.User; }
            set { _entry.User = value; }
        }

        /// <summary>
        /// The machine that created the queue
        /// </summary>
        public string Machine
        {
            get { return _entry.Machine; }
            set { _entry.Machine = value; }
        }

        /// <summary>
        /// The queue category
        /// </summary>
        public string Type
        {
            get { return _entry.Type; }
            set { _entry.Type = value; }
        }

        /// <summary>
        /// The datetime at which point this queue entry
        /// becomes 'visible' again for processing
        /// </summary>
        public DateTime ProcessingExpirationDateTimeUtc
        {
            get { return _entry.ProcessingExpirationDateTimeUtc; }
            set { _entry.ProcessingExpirationDateTimeUtc = value; }
        }
    }
}
