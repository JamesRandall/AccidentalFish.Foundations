using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;

namespace AccidentalFish.Foundations.Resources.Abstractions.Queues
{
    /// <summary>
    /// Factory for creating large message queues. Large message queues are not limited to the size constraints of Azure service bus and storage queues
    /// as they use a blob store for their message payload.
    /// </summary>
    public interface IAsyncLargeMessageQueueFactory
    {
        /// <summary>
        /// Create a large message queue
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="queueName">Name of the queue</param>
        /// <param name="blobContainerName">Name of the blob container</param>
        /// <returns>A queue</returns>
        Task<ILargeMessageQueue<T>> CreateAsync<T>(string queueName, string blobContainerName) where T : class;

        /// <summary>
        /// Create a large message queue
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="queueName">Name of the queue</param>
        /// <param name="blobContainerName">Name of the blob container</param>
        /// <param name="errorHandler">Optional error handler for blob errors</param>
        /// <returns>A queue</returns>
        Task<ILargeMessageQueue<T>> CreateAsync<T>(string queueName, string blobContainerName, ILargeMessageQueueErrorHandler errorHandler) where T : class;

        /// <summary>
        /// Create a large message queue
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="referenceQueue">The underlying queue that contains the blob messages</param>
        /// <param name="blobRepository"></param>
        /// <returns>A queue</returns>
        Task<ILargeMessageQueue<T>> CreateAsync<T>(
            IAsyncQueue<LargeMessageReference> referenceQueue,
            IAsyncBlockBlobRepository blobRepository) where T : class;

        /// <summary>
        /// Create a large message queue
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="referenceQueue">The underlying queue that contains the blob messages</param>
        /// <param name="blobRepository"></param>
        /// <param name="errorHandler">Optional error handler for blob errors</param>
        /// <returns>A queue</returns>
        Task<ILargeMessageQueue<T>> CreateAsync<T>(
            IAsyncQueue<LargeMessageReference> referenceQueue,
            IAsyncBlockBlobRepository blobRepository,
            ILargeMessageQueueErrorHandler errorHandler) where T : class;
    }
}
