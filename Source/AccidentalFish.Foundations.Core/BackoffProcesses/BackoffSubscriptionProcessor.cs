using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.BackoffProcesses
{
    /// <summary>
    /// Base hostable component for processing items on a subscription falling away into a backoff pattern when no queue items are available.
    /// To implement basic subscription processing simply inherit from this class and override HandleRecievedItem supplying a back off policy
    /// and the subscription to the constructor.
    /// </summary>
    /// <typeparam name="T">The type of the queue item</typeparam>
    public abstract class BackoffSubscriptionProcessor<T> where T : class
    {
        private readonly IAsyncBackoffPolicy _backoffPolicy;
        private readonly IAsyncSubscription<T> _subscription;
        private readonly ILogger<BackoffSubscriptionProcessor<T>> _logger;

        private class ProcessResult
        {
            public bool Complete { get; set; }

            public bool DidWork { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backoffPolicy">The back off policy to use.</param>
        /// <param name="subscription">The subscription to be processed</param>
        protected BackoffSubscriptionProcessor(
            IAsyncBackoffPolicy backoffPolicy,
            IAsyncSubscription<T> subscription) : this(backoffPolicy, subscription, null)
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backoffPolicy">The back off policy to use.</param>
        /// <param name="subscription">The suubscription to be processed</param>
        /// <param name="logger">The logger to use for reporting issues</param>
        protected BackoffSubscriptionProcessor(
            IAsyncBackoffPolicy backoffPolicy,
            IAsyncSubscription<T> subscription,
            ILogger<BackoffSubscriptionProcessor<T>> logger)
        {
            _backoffPolicy = backoffPolicy;
            _subscription = subscription;
            _logger = logger;
        }

        /// <summary>
        /// The logger the processor is configured with, may be null
        /// </summary>
        protected ILogger Logger => _logger;

        /// <summary>
        /// The subscription the processor is configured with
        /// </summary>
        protected IAsyncSubscription<T> Subscription => _subscription;

        /// <summary>
        /// Override to process a subscription item. 
        /// </summary>
        /// <param name="item">The subscription item to process</param>
        /// <returns>Return true to remove the item from the queue, false to return it to the queue.</returns>
        protected abstract Task<bool> HandleRecievedItemAsync(T item);

        /// <summary>
        /// Start running the back off subscription processor
        /// </summary>
        /// <param name="token">Token to indicate cancellation</param>
        /// <returns>Awaitable task</returns>
        public async Task StartAsync(CancellationToken token)
        {
            Logger?.LogTrace("Starting BackoffSubscriptionProcessor for type {TYPE}", typeof(T).FullName);
            await _backoffPolicy.ExecuteAsync(AttemptDequeue, token);
            Logger?.LogTrace("Exiting BackoffSubscriptionProcessor for type {TYPE}", typeof(T).FullName);
        }

        private async Task<bool> AttemptDequeue()
        {
            try
            {
                Logger?.LogTrace("Attempting subscription dequeue for type {TYPE}", typeof(T).FullName);
                bool didWork = true;
                await _subscription.RecieveAsync(async item =>
                {
                    ProcessResult result = await ProcessItem(item);
                    didWork = result.DidWork;
                    Logger?.LogTrace("Subscription dequeue result for type {TYPE} of {RESULT}", typeof(T).FullName, result);
                    return result.Complete;
                });
                return didWork;
            }
            catch (Exception ex)
            {
                Logger?.LogError("Error occurred {EX} in subscription dequeue in {TYPE}", ex, typeof(T).FullName);
                throw;
            }
        }

        private async Task<ProcessResult> ProcessItem(T message)
        {
            if (message == null)
            {
                return new ProcessResult
                {
                    Complete = false,
                    DidWork = false
                };
            }

            ProcessResult result = new ProcessResult
            {
                Complete = await HandleRecievedItemAsync(message),
                DidWork = true
            };

            return result;
        }
    }
}
