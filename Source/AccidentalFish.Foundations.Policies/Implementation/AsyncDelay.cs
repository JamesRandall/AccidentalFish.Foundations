using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Policies;

namespace AccidentalFish.Foundations.Policies.Implementation
{
    internal class AsyncDelay : IAsyncDelay
    {
        public Task Delay(TimeSpan interval)
        {
            return Task.Delay(interval);
        }

        public Task Delay(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }
    }
}
