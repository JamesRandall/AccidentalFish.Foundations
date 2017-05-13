using System;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    internal class QueueFactory : IQueueFactory
    {
        private readonly IQueueSerializer _queueSerializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConnectionStringProvider _connectionStringProvider;

        public QueueFactory(IQueueSerializer queueSerializer,
            ILoggerFactory loggerFactory,
            IConnectionStringProvider connectionStringProvider)
        {
            _queueSerializer = queueSerializer ?? throw new ArgumentNullException(nameof(queueSerializer));
            _loggerFactory = loggerFactory;
            _connectionStringProvider = connectionStringProvider;
        }

        public IAsyncQueue<T> CreateAsyncQueue<T>(string queueName) where T : class
        {
            string connectionString = _connectionStringProvider.Get<IAsyncQueue<T>>(queueName);
            return new AsyncQueue<T>(_queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncQueue<T>>());
        }

        public IAsyncQueue<T> CreateAsyncQueue<T>(string queueName, IQueueSerializer queueSerializer) where T : class
        {
            string connectionString = _connectionStringProvider.Get<IAsyncQueue<T>>(queueName);
            return new AsyncQueue<T>(queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncQueue<T>>());
        }        

        public IAsyncQueue<T> CreateAsyncBrokeredMessageQueue<T>(string queueName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }        

        public IAsyncTopic<T> CreateAsyncTopic<T>(string topicName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public IAsyncSubscription<T> CreateAsyncSubscriptionWithConfiguration<T>(string topicName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public IAsyncSubscription<T> CreateAsyncSubscriptionWithConfiguration<T>(string topicName, string subscrioptionName) where T : class
        {
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }        
    }
}
