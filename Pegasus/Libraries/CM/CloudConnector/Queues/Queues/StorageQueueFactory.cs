
namespace Sage.Connector.Queues
{
    /// <summary>
    /// Factory for creating a storage queue
    /// </summary>
    public class StorageQueueFactory 
    {
        /// <summary>
        /// Retrieves a queue by name (creating if one does not exist by that name)
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public StorageQueue GetQueue(string queueName)
        {
            return new StorageQueue(queueName);
        }
    }
}
