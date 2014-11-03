

namespace Sage.Connector.Queues
{
    /// <summary>
    /// A queued message storage element
    /// </summary>
    public class StorageQueueMessage
    {
        private string _id = string.Empty;
        private string _payloadType = string.Empty;
        private string _payload = string.Empty;

        /// <summary>
        /// Constructor for setting id/payload for this read-only container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="payloadType"></param>
        /// <param name="payload"></param>
        public StorageQueueMessage(string id, string payloadType, string payload)
        {
            _id = id;
            _payloadType = payloadType;
            _payload = payload;
        }

        /// <summary>
        /// The unique identifier for this message
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The type of payload
        /// </summary>
        public string PayloadType
        {
            get { return _payloadType; }
        }

        /// <summary>
        /// The message payload
        /// </summary>
        public string Payload
        {
            get { return _payload; }
        }
    }
}
