using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    internal class AsyncQueue<T> : IAsyncStorageQueue<T> where T : class
    {
        private readonly CloudQueue _queue;
        private readonly IQueueSerializer _serializer;
        private readonly string _queueName;
        private readonly ILogger<AsyncQueue<T>> _logger;

        public AsyncQueue(IQueueSerializer queueSerializer, string connectionString, string queueName, ILogger<AsyncQueue<T>> logger)
        {
            if (queueSerializer == null) throw new ArgumentNullException(nameof(queueSerializer));
            if (String.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (String.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(120), 3);
            _queue = queueClient.GetQueueReference(queueName);
            _serializer = queueSerializer;
            _queueName = queueName;
            _logger = logger;

            _logger?.LogTrace("AsyncQueue: created for queue {QUEUENAME}", queueName);
        }

        public Task EnqueueAsync(T item, IDictionary<string, object> messageProperties = null)
        {
            if (messageProperties != null)
            {
                throw new NotSupportedException("Message properties are not supported by Azure Storage queues");
            }

            _logger?.LogTrace("AsyncQueue: EnqueueAsync - enqueueing item");
            CloudQueueMessage message = new CloudQueueMessage(_serializer.Serialize(item));
            return _queue.AddMessageAsync(message);
        }

        public Task EnqueueAsync(T item, TimeSpan initialVisibilityDelay, IDictionary<string, object> messageProperties = null)
        {
            if (messageProperties != null)
            {
                throw new NotSupportedException("Message properties are not supported by Azure Storage queues");
            }

            _logger?.LogTrace("AsyncQueue: EnqueueAsync - enqueueing item with initial visibility delay of {TIMEMS}ms", initialVisibilityDelay.TotalMilliseconds);
            CloudQueueMessage message = new CloudQueueMessage(_serializer.Serialize(item));
            return _queue.AddMessageAsync(message, null, initialVisibilityDelay, null, null);
        }

        public Task DequeueAsync(Func<IQueueItem<T>, Task<bool>> processor)
        {
            return DequeueAsync(processor, null);
        }

        public async Task DequeueAsync(Func<IQueueItem<T>, Task<bool>> processor, TimeSpan? visibilityTimeout)
        {
            CloudQueueMessage message = await _queue.GetMessageAsync(visibilityTimeout, null, null);
            if (message != null)
            {
                _logger?.LogTrace("AsyncQueue: DequeueAsync - dequeued item from queue {QUEUENAME}", _queueName);
                T item = _serializer.Deserialize<T>(message.AsString);
                if (await processor(new CloudQueueItem<T>(message, item, message.DequeueCount, message.PopReceipt)))
                {
                    _logger?.LogTrace("AsyncQueue: DequeueAsync - deleting item from queue {QUEUENAME}", _queueName);
                    await _queue.DeleteMessageAsync(message);
                }
                else
                {
                    _logger?.LogTrace("AsyncQueue: DequeueAsync - returnng item to queue {QUEUENAME}", _queueName);
                }
            }
            else
            {
                await processor(null);
            }
        }

        public async Task ExtendLeaseAsync(IQueueItem<T> queueItem, TimeSpan visibilityTimeout)
        {
            CloudQueueItem<T> queueItemImpl = queueItem as CloudQueueItem<T>;
            if (queueItemImpl == null)
            {
                throw new InvalidOperationException("Cannot mix Azure and non-Azure queue items when extending a lease");
            }
            await _queue.UpdateMessageAsync(queueItemImpl.CloudQueueMessage, visibilityTimeout, MessageUpdateFields.Visibility);
            _logger?.LogTrace("AsyncQueue: ExtendLeaseAsync - extending by {TIMEMS}ms", visibilityTimeout.TotalMilliseconds);
        }

        public async Task ExtendLeaseAsync(IQueueItem<T> queueItem)
        {
            CloudQueueItem<T> queueItemImpl = queueItem as CloudQueueItem<T>;
            if (queueItemImpl == null)
            {
                throw new InvalidOperationException("Cannot mix Azure and non-Azure queue items when extending a lease");
            }
            await _queue.UpdateMessageAsync(queueItemImpl.CloudQueueMessage, TimeSpan.FromSeconds(30), MessageUpdateFields.Visibility);
            _logger?.LogTrace("AsyncQueue: ExtendLeaseAsync - extending by {TIMEMS}ms", TimeSpan.FromSeconds(30).TotalMilliseconds);
        }

        internal CloudQueue UnderlyingQueue => _queue;
    }
}
