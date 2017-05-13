using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Policies.Implementation
{
    internal class BackoffPolicyFactory : IBackoffPolicyFactory
    {
        private readonly ILogger<AsyncBackoffPolicy> _asyncBackOffPolicyLogger;
        private readonly ILogger<BackoffPolicy> _backOffPolicyLogger;

        public BackoffPolicyFactory(ILogger<AsyncBackoffPolicy> asyncBackOffPolicyLogger, ILogger<BackoffPolicy> backOffPolicyLogger)
        {
            _asyncBackOffPolicyLogger = asyncBackOffPolicyLogger;
            _backOffPolicyLogger = backOffPolicyLogger;
        }

        public IAsyncBackoffPolicy CreateAsyncBackoffPolicy(IReadOnlyCollection<TimeSpan> backoffTimings = null)
        {
            return new AsyncBackoffPolicy(_asyncBackOffPolicyLogger, new FactoryBackoffPolicyTimingProvider(backoffTimings));
        }

        public IBackoffPolicy CreateBackoffPolicy(IReadOnlyCollection<TimeSpan> backoffTimings = null)
        {
            return new BackoffPolicy(_backOffPolicyLogger, new FactoryBackoffPolicyTimingProvider(backoffTimings));
        }
    }
}
