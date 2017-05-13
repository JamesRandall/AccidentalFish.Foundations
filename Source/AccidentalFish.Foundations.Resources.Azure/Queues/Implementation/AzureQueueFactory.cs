using System;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    internal class AzureQueueFactory : IAzureQueueFactory
    {
        private readonly IQueueFactory _queueFactory;
        private readonly IQueueSerializer _queueSerializer;
        private readonly IConnectionStringProvider _connectionStringProvider;

        public AzureQueueFactory(IQueueFactory queueFactory,
            IQueueSerializer queueSerializer,
            IConnectionStringProvider connectionStringProvider)
        {
            _queueFactory = queueFactory;
            _queueSerializer = queueSerializer;
            _connectionStringProvider = connectionStringProvider;
        }

        public IAsyncQueue<T> CreateAsyncQueue<T>(string queueName) where T : class
        {
            return _queueFactory.CreateAsyncQueue<T>(queueName);
        }

        public IAsyncQueue<T> CreateAsyncQueue<T>(string queueName, IQueueSerializer queueSerializer) where T : class
        {
            return _queueFactory.CreateAsyncQueue<T>(queueName, queueSerializer);
        }

        public IAsyncTopic<T> CreateAsyncTopic<T>(string topicName) where T : class
        {
            //return _queueFactory.CreateAsyncTopic<T>(topicName);
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public IAsyncSubscription<T> CreateAsyncSubscriptionWithConfiguration<T>(string topicName) where T : class
        {
            //return _queueFactory.CreateAsyncSubscriptionWithConfiguration<T>(topicName);
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public IAsyncSubscription<T> CreateAsyncSubscriptionWithConfiguration<T>(string topicName, string subscriptionName) where T : class
        {
            //return _queueFactory.CreateAsyncSubscriptionWithConfiguration<T>(topicName, subscriptionName);
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }

        public IAsyncQueue<T> CreateAsyncBrokeredMessageQueue<T>(string queueName) where T : class
        {
            /*string connectionString = _connectionStringProvider.Get<IAsyncBrokeredMessageQueue<T>>(queueName);
            return new AsyncBrokeredMessageQueue<T>(_queueSerializer, connectionString, queueName, _loggerFactory.CreateLogger<AsyncBrokeredMessageQueue<T>>);*/
            throw new NotSupportedException("Awaiting finalization of Microsoft.Azure.ServiceBus");
        }        
    }
}
