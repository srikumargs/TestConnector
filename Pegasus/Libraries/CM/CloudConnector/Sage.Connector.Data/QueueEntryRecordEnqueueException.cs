using System;
using System.Runtime.Serialization;

namespace Sage.Connector.Data
{
    /// <summary>
    /// An exception for enqueue failures
    /// </summary>
    [Serializable]
    public class QueueEntryRecordEnqueueException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the QueueEntryRecordEnqueueException class.
        /// </summary>
        public QueueEntryRecordEnqueueException() : base() { }

        /// <summary>
        /// Initializes a new instance of the QueueEntryRecordEnqueueException class with
        ///     a specified error message.
        /// </summary>
        /// <param name="message"></param>
        public QueueEntryRecordEnqueueException(string message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the QueueEntryRecordEnqueueException class with
        ///     serialized data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected QueueEntryRecordEnqueueException(SerializationInfo info, StreamingContext context) : base(info, context) { }   

        /// <summary>
        ///     Initializes a new instance of the QueueEntryRecordEnqueueException class with
        ///     a specified error message and a reference to the inner exception that is
        ///     the cause of this exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public QueueEntryRecordEnqueueException(string message, Exception innerException) : base(message, innerException) { }
    }
}
