using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Abstractions.BackoffProcesses
{
    /// <summary>
    /// Base hostable component for processing items on a queue falling away into a backoff pattern when no queue items are available.
    /// To implement basic queue processing simply inherit from this class and override HandleRecievedItem supplying a back off policy
    /// and the queue to the constructor.
    /// </summary>
    /// <typeparam name="T">The type of the queue item</typeparam>
    public abstract class AbstractBackoffQueueProcessor<T> where T : class
    {
        private readonly Func<Exception, Task<bool>> _dequeErrorHandler;
        private readonly IAsyncBackoffPolicy _backoffPolicy;
        private readonly IAsyncQueue<T> _queue;
        private readonly ILogger<AbstractBackoffQueueProcessor<T>> _logger;

        private class ProcessResult
        {
            public bool Complete { get; set; }

            public bool DidWork { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backoffPolicy">The back off policy to use.</param>
        /// <param name="queue">The queue to be processed</param>
        /// <param name="dequeErrorHandler">Optional error handler for dequeue failures. This can return true / false or throw an exception</param>
        protected AbstractBackoffQueueProcessor(
            IAsyncBackoffPolicy backoffPolicy,
            IAsyncQueue<T> queue,
            Func<Exception, Task<bool>> dequeErrorHandler=null) : this(backoffPolicy, queue, null, dequeErrorHandler)
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backoffPolicy">The back off policy to use.</param>
        /// <param name="queue">The queue to be processed</param>
        /// <param name="logger">The logger to use for reporting issues</param>
        /// <param name="dequeErrorHandler">Optional error handler for dequeue failures. This can return true / false or throw an exception</param>
        protected AbstractBackoffQueueProcessor(
            IAsyncBackoffPolicy backoffPolicy,
            IAsyncQueue<T> queue,
            ILogger<AbstractBackoffQueueProcessor<T>> logger,
            Func<Exception, Task<bool>> dequeErrorHandler = null)
        {
            _backoffPolicy = backoffPolicy;
            _queue = queue;
            _logger = logger;
            _dequeErrorHandler = dequeErrorHandler;
        }

        /// <summary>
        /// The logger the processor is configured with, may be null
        /// </summary>
        protected ILogger Logger => _logger;

        /// <summary>
        /// The queue the processor is configured with
        /// </summary>
        protected IAsyncQueue<T> Queue => _queue;

        /// <summary>
        /// Override to process a queue item. 
        /// </summary>
        /// <param name="item">The queue item to process</param>
        /// <returns>Return true to remove the item from the queue, false to return it to the queue.</returns>
        protected abstract Task<bool> HandleRecievedItemAsync(IQueueItem<T> item);        

        /// <summary>
        /// Starts the component and begins queue processing.
        /// </summary>
        /// <param name="token">The cancellation token to use to indicate termination</param>
        /// <returns>An awaitable task</returns>
        public async Task StartAsync(CancellationToken token)
        {
            Logger?.LogTrace("Starting AbstractBackoffQueueProcessor for type {TYPE}", typeof(T).FullName);
            await _backoffPolicy.ExecuteAsync(AttemptDequeueAsync, token);
            Logger?.LogTrace("Exiting AbstractBackoffQueueProcessor for type {TYPE}", typeof(T).FullName);
        }

        private async Task<bool> AttemptDequeueAsync()
        {
            try
            {
                Logger?.LogTrace("Attempting dequeue for type {TYPE}", typeof(T).FullName);
                bool didWork = true;
                try
                {
                    await _queue.DequeueAsync(async item =>
                    {
                        ProcessResult result = await ProcessItem(item);
                        didWork = result.DidWork;
                        Logger?.LogTrace("Dequeue result for type {TYPE} of {RESULT}", typeof(T).FullName, result);
                        return result.Complete;
                    });
                    return didWork;
                }
                catch (Exception ex)
                {
                    if (_dequeErrorHandler != null)
                    {
                        return await _dequeErrorHandler(ex);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError("Error occurred {EX} in dequeue for type {TYPE}", ex, typeof(T).FullName);
                throw;
            }
        }

        private async Task<ProcessResult> ProcessItem(IQueueItem<T> message)
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
                Complete = await HandleRecievedItemAsync(message), // let the virtual method decide whether to complete (true) or abandon (false) the message,
                DidWork = true
            };

            return result;
        }
    }
}
