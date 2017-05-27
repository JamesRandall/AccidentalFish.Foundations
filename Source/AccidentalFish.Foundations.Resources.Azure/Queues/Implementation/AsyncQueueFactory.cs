using System;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using AccidentalFish.Foundations.Resources.Azure.Implementation;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    class AsyncQueueFactory : IAsyncQueueFactory
    {
        private readonly IQueueSerializer _queueSerializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IAzureSettings _azureSettings;

        public AsyncQueueFactory(IQueueSerializer queueSerializer,
            ILoggerFactory loggerFactory,
            IConnectionStringProvider connectionStringProvider,
            IAzureSettings azureSettings)
        {
            _queueSerializer = queueSerializer ?? throw new ArgumentNullException(nameof(queueSerializer));
            _loggerFactory = loggerFactory;
            _connectionStringProvider = connectionStringProvider;
            _azureSettings = azureSettings;
        }

        public async Task<IAsyncQueue<T>> CreateAsyncQueueAsync<T>(string queueName) where T : class
        {
            string connectionString = await _connectionStringProvider.GetAsync<IAsyncQueue<T>>(queueName);
            IAsyncQueue<T> result = new AsyncQueue<T>(_queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncQueue<T>>());
            if (_azureSettings.CreateIfNotExists)
            {
                await result.GetCloudQueue().CreateIfNotExistsAsync();
            }
            return result;
        }

        public async Task<IAsyncQueue<T>> CreateAsyncQueueAsync<T>(string queueName, IQueueSerializer queueSerializer) where T : class
        {
            string connectionString = await _connectionStringProvider.GetAsync<IAsyncQueue<T>>(queueName);
            IAsyncQueue<T> result = new AsyncQueue<T>(queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncQueue<T>>());
            if (_azureSettings.CreateIfNotExists)
            {
                await result.GetCloudQueue().CreateIfNotExistsAsync();
            }
            return result;
        }

        public Task<IAsyncQueue<T>> CreateAsyncBrokeredMessageQueueAsync<T>(string queueName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public Task<IAsyncTopic<T>> CreateAsyncTopicAsync<T>(string topicName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public Task<IAsyncSubscription<T>> CreateAsyncSubscriptionWithConfigurationAsync<T>(string topicName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public Task<IAsyncSubscription<T>> CreateAsyncSubscriptionWithConfigurationAsync<T>(string topicName, string subscrioptionName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }
    }
}
