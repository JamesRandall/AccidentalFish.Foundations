using System;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.BackoffProcesses;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.Queues;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.Runtime;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Runtime.HostableComponents
{
    public abstract class HostableBackoffQueueProcessor<T> : BackoffQueueProcessor<T>, IHostableComponent where T : class
    {
        protected HostableBackoffQueueProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncQueue<T> queue, Func<Exception, Task<bool>> dequeErrorHandler = null) : base(backoffPolicy, queue, dequeErrorHandler)
        {
        }

        protected HostableBackoffQueueProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncQueue<T> queue, ILogger<BackoffQueueProcessor<T>> logger, Func<Exception, Task<bool>> dequeErrorHandler = null) : base(backoffPolicy, queue, logger, dequeErrorHandler)
        {
        }
    }
}
