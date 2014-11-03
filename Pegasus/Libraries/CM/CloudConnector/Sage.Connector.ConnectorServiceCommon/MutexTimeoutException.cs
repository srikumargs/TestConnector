using System;
using System.Runtime.Serialization;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// An exception for mutex timeout to utilize with retry
    /// </summary>
    [Serializable]
    public class MutexTimeoutExeption : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the MutexTimeoutExeption class.
        /// </summary>
        public MutexTimeoutExeption() : base() { }

        /// <summary>
        /// Initializes a new instance of the MutexTimeoutExeption class with
        ///     a specified error message.
        /// </summary>
        /// <param name="message"></param>
        public MutexTimeoutExeption(string message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the MutexTimeoutExeption class with
        ///     serialized data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MutexTimeoutExeption(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        ///     Initializes a new instance of the MutexTimeoutExeption class with
        ///     a specified error message and a reference to the inner exception that is
        ///     the cause of this exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MutexTimeoutExeption(string message, Exception innerException) : base(message, innerException) { }
    }
}
