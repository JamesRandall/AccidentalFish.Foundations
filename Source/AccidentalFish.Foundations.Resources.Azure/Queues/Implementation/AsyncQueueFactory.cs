using System;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    class AsyncQueueFactory : IAsyncQueueFactory
    {
        private readonly IQueueSerializer _queueSerializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConnectionStringProvider _connectionStringProvider;

        public AsyncQueueFactory(IQueueSerializer queueSerializer,
            ILoggerFactory loggerFactory,
            IConnectionStringProvider connectionStringProvider)
        {
            _queueSerializer = queueSerializer ?? throw new ArgumentNullException(nameof(queueSerializer));
            _loggerFactory = loggerFactory;
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<IAsyncQueue<T>> CreateAsyncQueueAsync<T>(string queueName) where T : class
        {
            string connectionString = await _connectionStringProvider.GetAsync<AsyncQueue<T>>(queueName);
            return new AsyncQueue<T>(_queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncQueue<T>>());
        }

        public async Task<IAsyncQueue<T>> CreateAsyncQueueAsync<T>(string queueName, IQueueSerializer queueSerializer) where T : class
        {
            string connectionString = await _connectionStringProvider.GetAsync<AsyncQueue<T>>(queueName);
            return new AsyncQueue<T>(queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncQueue<T>>());
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
