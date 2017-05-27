using System;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using AccidentalFish.Foundations.Resources.Azure.Implementation;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    internal class QueueFactory : IQueueFactory
    {
        private readonly IQueueSerializer _queueSerializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IAzureSettings _azureSettings;

        public QueueFactory(IQueueSerializer queueSerializer,
            ILoggerFactory loggerFactory,
            IConnectionStringProvider connectionStringProvider,
            IAzureSettings azureSettings)
        {
            _queueSerializer = queueSerializer ?? throw new ArgumentNullException(nameof(queueSerializer));
            _loggerFactory = loggerFactory;
            _connectionStringProvider = connectionStringProvider;
            _azureSettings = azureSettings;
        }

        public IAsyncQueue<T> CreateAsyncQueue<T>(string queueName) where T : class
        {
            if (_azureSettings.CreateIfNotExists)
            {
                throw new InvalidOperationException("Creation of resources can only be done through an asynchronous factory");
            }

            string connectionString = _connectionStringProvider.Get<IAsyncQueue<T>>(queueName);
            return new AsyncQueue<T>(_queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncQueue<T>>());
        }

        public IAsyncQueue<T> CreateAsyncQueue<T>(string queueName, IQueueSerializer queueSerializer) where T : class
        {
            if (_azureSettings.CreateIfNotExists)
            {
                throw new InvalidOperationException("Creation of resources can only be done through an asynchronous factory");
            }

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
