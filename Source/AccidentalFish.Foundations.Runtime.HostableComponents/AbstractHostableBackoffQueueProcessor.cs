using System;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.Foundations.Resources.Abstractions.BackoffProcesses;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Runtime.HostableComponents
{
    public abstract class AbstractHostableBackoffQueueProcessor<T> : AbstractBackoffQueueProcessor<T>, IHostableComponent where T : class
    {
        protected AbstractHostableBackoffQueueProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncQueue<T> queue, Func<Exception, Task<bool>> dequeErrorHandler = null) : base(backoffPolicy, queue, dequeErrorHandler)
        {
        }

        protected AbstractHostableBackoffQueueProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncQueue<T> queue, ILogger<AbstractBackoffQueueProcessor<T>> logger, Func<Exception, Task<bool>> dequeErrorHandler = null) : base(backoffPolicy, queue, logger, dequeErrorHandler)
        {
        }
    }
}
