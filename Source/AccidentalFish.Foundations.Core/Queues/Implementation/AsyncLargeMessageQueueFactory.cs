using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.Blobs;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.Queues.Implementation
{
    class AsyncLargeMessageQueueFactory : IAsyncLargeMessageQueueFactory
    {
        private readonly IAsyncQueueFactory _queueFactory;
        private readonly IAsyncBlobRepositoryFactory _blobRepositoryFactory;
        private readonly IQueueSerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;

        public AsyncLargeMessageQueueFactory(
            IAsyncQueueFactory queueFactory,
            IAsyncBlobRepositoryFactory blobRepositoryFactory,
            IQueueSerializer serializer,
            ILoggerFactory loggerFactory)
        {
            _queueFactory = queueFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
            _serializer = serializer;
            _loggerFactory = loggerFactory;            
        }

        public Task<ILargeMessageQueue<T>> CreateAsync<T>(string queueName, string blobContainerName) where T : class
        {
            return CreateAsync<T>(queueName, blobContainerName, null);
        }

        public async Task<ILargeMessageQueue<T>> CreateAsync<T>(string queueName, string blobContainerName, ILargeMessageQueueErrorHandler errorHandler) where T : class
        {
            IAsyncQueue<LargeMessageReference> referenceQueue = await _queueFactory.CreateAsyncQueueAsync<LargeMessageReference>(queueName);
            IAsyncBlockBlobRepository blobRepository = await _blobRepositoryFactory.CreateAsyncBlockBlobRepositoryAsync(blobContainerName);

            ILargeMessageQueue<T> queue = new LargeMessageQueue<T>(_serializer, referenceQueue, blobRepository, _loggerFactory.CreateLogger<LargeMessageQueue<T>>(), errorHandler);
            return queue;
        }

        public Task<ILargeMessageQueue<T>> CreateAsync<T>(IAsyncQueue<LargeMessageReference> referenceQueue, IAsyncBlockBlobRepository blobRepository) where T : class
        {
            return CreateAsync<T>(referenceQueue, blobRepository, null);
        }

        public Task<ILargeMessageQueue<T>> CreateAsync<T>(
            IAsyncQueue<LargeMessageReference> referenceQueue,
            IAsyncBlockBlobRepository blobRepository,
            ILargeMessageQueueErrorHandler errorHandler) where T : class
        {
            return Task.FromResult<ILargeMessageQueue<T>>(new LargeMessageQueue<T>(_serializer, referenceQueue, blobRepository, _loggerFactory.CreateLogger<LargeMessageQueue<T>>(), errorHandler));
        }
    }
}
