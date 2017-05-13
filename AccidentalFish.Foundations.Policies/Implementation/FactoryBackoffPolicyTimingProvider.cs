using System;
using System.Collections.Generic;

namespace AccidentalFish.ApplicationSupport.Policies.Implementation
{
    class FactoryBackoffPolicyTimingProvider : IBackoffPolicyTimingProvider
    {
        private readonly IReadOnlyCollection<TimeSpan> _values;

        public FactoryBackoffPolicyTimingProvider(IReadOnlyCollection<TimeSpan> backoffTimings)
        {
            _values = backoffTimings;
        }

        public IReadOnlyCollection<TimeSpan> GetIntervals()
        {
            return _values;
        }
    }
}
