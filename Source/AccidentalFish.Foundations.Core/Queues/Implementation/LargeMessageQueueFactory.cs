using AccidentalFish.ApplicationSupport.Resources.Abstractions.Blobs;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.Queues.Implementation
{
    internal class LargeMessageQueueFactory : ILargeMessageQueueFactory
    {
        private readonly IQueueFactory _queueFactory;
        private readonly IBlobRepositoryFactory _blobRepositoryFactory;
        private readonly IQueueSerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;

        public LargeMessageQueueFactory(
            IQueueFactory queueFactory,
            IBlobRepositoryFactory blobRepositoryFactory,
            IQueueSerializer serializer,
            ILoggerFactory loggerFactory)
        {
            _queueFactory = queueFactory;
            _blobRepositoryFactory = blobRepositoryFactory;
            _serializer = serializer;
            _loggerFactory = loggerFactory;
        }        

        public ILargeMessageQueue<T> Create<T>(string queueName, string blobContainerName) where T : class
        {
            return Create<T>(queueName, blobContainerName, null);
        }

        public ILargeMessageQueue<T> Create<T>(string queueName, string blobContainerName, ILargeMessageQueueErrorHandler errorHandler) where T : class
        {
            IAsyncQueue<LargeMessageReference> referenceQueue = _queueFactory.CreateAsyncQueue<LargeMessageReference>(queueName);
            IAsyncBlockBlobRepository blobRepository = _blobRepositoryFactory.CreateAsyncBlockBlobRepository(blobContainerName);

            ILargeMessageQueue<T> queue = new LargeMessageQueue<T>(_serializer, referenceQueue, blobRepository, _loggerFactory.CreateLogger<LargeMessageQueue<T>>(), errorHandler);
            return queue;
        }

        public ILargeMessageQueue<T> Create<T>(IAsyncQueue<LargeMessageReference> referenceQueue, IAsyncBlockBlobRepository blobRepository) where T : class
        {
            return Create<T>(referenceQueue, blobRepository, null);
        }

        public ILargeMessageQueue<T> Create<T>(
            IAsyncQueue<LargeMessageReference> referenceQueue,
            IAsyncBlockBlobRepository blobRepository,
            ILargeMessageQueueErrorHandler errorHandler) where T : class
        {
            return new LargeMessageQueue<T>(_serializer, referenceQueue, blobRepository, _loggerFactory.CreateLogger<LargeMessageQueue<T>>(), errorHandler);
        }
    }
}
