using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.BackoffProcesses;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.Queues;
using AccidentalFish.ApplicationSupport.Resources.Abstractions.Runtime;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Runtime.HostableComponents
{
    public abstract class HostableBackoffSubscriptionProcessor<T> : BackoffSubscriptionProcessor<T>, IHostableComponent where T : class
    {
        public HostableBackoffSubscriptionProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncSubscription<T> subscription) : base(backoffPolicy, subscription)
        {
        }

        public HostableBackoffSubscriptionProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncSubscription<T> subscription, ILogger<BackoffSubscriptionProcessor<T>> logger) : base(backoffPolicy, subscription, logger)
        {
        }
    }
}
