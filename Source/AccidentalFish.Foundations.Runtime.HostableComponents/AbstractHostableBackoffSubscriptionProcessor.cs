using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.Foundations.Resources.Abstractions.BackoffProcesses;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Runtime.HostableComponents
{
    public abstract class AbstractHostableBackoffSubscriptionProcessor<T> : AbstractBackoffSubscriptionProcessor<T>, IHostableComponent where T : class
    {
        protected AbstractHostableBackoffSubscriptionProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncSubscription<T> subscription) : base(backoffPolicy, subscription)
        {
        }

        protected AbstractHostableBackoffSubscriptionProcessor(IAsyncBackoffPolicy backoffPolicy, IAsyncSubscription<T> subscription, ILogger<AbstractBackoffSubscriptionProcessor<T>> logger) : base(backoffPolicy, subscription, logger)
        {
        }
    }
}
