using System;
using System.Collections.Generic;
using System.Threading;
using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.Foundations.Threading;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Policies.Implementation
{
    internal class BackoffPolicy : IBackoffPolicy
    {
        private readonly ILogger<BackoffPolicy> _logger;
        private readonly IReadOnlyCollection<TimeSpan> _backoffTimings;

        public BackoffPolicy(ILogger<BackoffPolicy> logger, IBackoffPolicyTimingProvider provider)
        {
            _logger = logger;
            _backoffTimings = provider.GetIntervals();
        }

        public void Execute(Func<bool> function, IWaitHandle waitHandle)
        {
            do
            {
                bool result = true;
                while (result && !waitHandle.IsSet)
                {
                    result = function();
                }

                IEnumerator<TimeSpan> enumerator = _backoffTimings.GetEnumerator();
                enumerator.MoveNext();
                TimeSpan currentWaitTime = enumerator.Current;
                if (!result)
                {
                    _logger?.LogTrace("BackoffPolicy - backing off to {WAITTIME}ms", currentWaitTime.TotalMilliseconds);
                }
                while (!result && !waitHandle.Wait(currentWaitTime))
                {
                    result = function();
                    if (!result)
                    {
                        if (enumerator.MoveNext())
                        {
                            currentWaitTime = enumerator.Current;
                        }
                    }
                }
            }
            while (!waitHandle.IsSet);
        }

        public void Execute(Func<bool> function, CancellationToken cancellationToken)
        {
            do
            {
                bool result = true;
                while (result && !cancellationToken.IsCancellationRequested)
                {
                    result = function();
                }

                IEnumerator<TimeSpan> enumerator = _backoffTimings.GetEnumerator();
                enumerator.MoveNext();
                TimeSpan currentWaitTime = enumerator.Current;

                if (!result)
                {
                    _logger?.LogTrace("BackoffPolicy - backing off to {WAITTIME}ms", currentWaitTime.TotalMilliseconds);
                }
                while (!result && !cancellationToken.WaitHandle.WaitOne(currentWaitTime))
                {
                    result = function();
                    if (!result)
                    {
                        if (enumerator.MoveNext())
                        {
                            currentWaitTime = enumerator.Current;
                        }
                    }
                }
            }
            while (!cancellationToken.IsCancellationRequested);
        }
    }
}
