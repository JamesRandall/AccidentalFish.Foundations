using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Resources.Abstractions.Queues
{
    /// <summary>
    /// Factory for creating configured queues
    /// </summary>
    public interface IAsyncQueueFactory
    {
        /// <summary>
        /// Create an asynchronous queue with the given name with a connection string as specified in an app setting of azure-storage-connection-string
        /// </summary>
        /// <typeparam name="T">The type of the queue items</typeparam>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>A configured queue</returns>
        Task<IAsyncQueue<T>> CreateAsyncQueueAsync<T>(string queueName) where T : class;

        /// <summary>
        /// Create an asynchronous queue with the given name with a connection string as specified in an app setting of azure-storage-connection-string
        /// </summary>
        /// <typeparam name="T">The type of the queue items</typeparam>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="queueSerializer">Serializer for the queue items</param>
        /// <returns>A configured queue</returns>
        Task<IAsyncQueue<T>> CreateAsyncQueueAsync<T>(string queueName, IQueueSerializer queueSerializer) where T : class;

        /// <summary>
        /// Create an asynchronous topic with the given name with a connection string as specified in an app setting of azure-storage-connection-string
        /// </summary>
        /// <typeparam name="T">The type of the topic items</typeparam>
        /// <param name="topicName">The name of the topic</param>
        /// <returns>A configured topic</returns>
        Task<IAsyncTopic<T>> CreateAsyncTopicAsync<T>(string topicName) where T : class;

        /// <summary>
        /// Create an asynchronous subscription looking at the specified topic with the given name with a connection string as specified in an app setting of azure-storage-connection-string.
        /// The subscription is given an auto-generated name.
        /// </summary>
        /// <typeparam name="T">The type of the topic items</typeparam>
        /// <param name="topicName">The name of the topic</param>
        /// <returns>A configured subscription</returns>
        Task<IAsyncSubscription<T>> CreateAsyncSubscriptionWithConfigurationAsync<T>(string topicName) where T : class;

        /// <summary>
        /// Create an asynchronous subscription looking at the specified topic with the given name with a connection string as specified in an app setting of azure-storage-connection-string.
        /// </summary>
        /// <typeparam name="T">The type of the topic items</typeparam>
        /// <param name="topicName">The name of the topic</param>
        /// <param name="subscriptionName">The name of the subscription</param>
        /// <returns>A configured subscription</returns>
        Task<IAsyncSubscription<T>> CreateAsyncSubscriptionWithConfigurationAsync<T>(string topicName, string subscriptionName) where T : class;        
    }
}
