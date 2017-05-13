using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.Foundations.Resources.Abstractions.BackoffProcesses;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Runtime.HostableComponents
{
    public abstract class HostableBackoffSubscriptionProcessor<T> : AbstractBackoffSubscriptionProcessor<T>, IHostableComponent where T : class
    {
        public HostableBackoffSubscriptionProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncSubscription<T> subscription) : base(backoffPolicy, subscription)
        {
        }

        public HostableBackoffSubscriptionProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncSubscription<T> subscription, ILogger<AbstractBackoffSubscriptionProcessor<T>> logger) : base(backoffPolicy, subscription, logger)
        {
        }
    }
}
