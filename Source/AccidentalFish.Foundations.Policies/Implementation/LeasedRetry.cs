using System;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Policies;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Policies.Implementation
{
    internal class LeasedRetry : ILeasedRetry
    {
        private readonly ILogger<LeasedRetry> _logger;

        public LeasedRetry(ILogger<LeasedRetry> logger)
        {
            _logger = logger;
        }

        public Task<bool> RetryAsync<T>(ILeaseManager<T> leaseManager, T key, Func<Task> func)
        {
            return RetryAsync(leaseManager, key, TimeSpan.FromSeconds(30), 10, false, func);
        }

        public Task<bool> RetryAsync<T>(ILeaseManager<T> leaseManager, T key, TimeSpan leaseDuration, Func<Task> func)
        {
            return RetryAsync(leaseManager, key, leaseDuration, 30, false, func);
        }

        public Task<bool> RetryAsync<T>(ILeaseManager<T> leaseManager, T key, bool createLazyLeaseObject, Func<Task> func)
        {
            return RetryAsync(leaseManager, key, TimeSpan.FromSeconds(30), 10, createLazyLeaseObject, func);
        }

        public Task<bool> RetryAsync<T>(ILeaseManager<T> leaseManager, T key, TimeSpan leaseDuration, int maxRetries, Func<Task> func)
        {
            return RetryAsync(leaseManager, key, leaseDuration, maxRetries, false, func);
        }

        public Task<bool> RetryAsync<T>(ILeaseManager<T> leaseManager, T key, TimeSpan leaseDuration, bool createLazyLeaseObject, Func<Task> func)
        {
            return RetryAsync(leaseManager, key, leaseDuration, 10, createLazyLeaseObject, func);
        }

        public async Task<bool> RetryAsync<T>(ILeaseManager<T> leaseManager, T key, TimeSpan leaseDuration, int maxRetries,
            bool createLazyLeaseObject, Func<Task> func)
        {
            TimeSpan retryDelay = TimeSpan.FromSeconds(leaseDuration.TotalSeconds / maxRetries);
            string leaseId = null;
            int retry = 0;

            while (String.IsNullOrWhiteSpace(leaseId) && retry < maxRetries)
            {
                _logger?.LogTrace("LeasedRetry - RetryAsync - attempting to acquire lease {KEY}, retry {RETRY}", key, retry);
                try
                {
                    leaseId = await leaseManager.LeaseAsync(key, leaseDuration);
                }
                catch (Exception)
                {
                    leaseId = null;
                }

                if (String.IsNullOrWhiteSpace(leaseId))
                {
                    if (retry == 0)
                    {
                        await leaseManager.CreateLeaseObjectIfNotExistAsync(key);
                    }
                    retry++;
                    await Task.Delay(retryDelay);
                }
            }

            if (String.IsNullOrWhiteSpace(leaseId))
            {
                _logger?.LogTrace("LeasedRetry - RetryAsync - failed to acquire lease {KEY} after retry {RETRYCOUNT} and giving up", key, retry);
                return false;
            }

            _logger?.LogTrace("LeasedRetry - RetryAsync - acquired lease {KEY} after retry {RETRYCOUNT}", key, retry);

            try
            {
                await func();
            }
            finally
            {
                leaseManager.ReleaseAsync(key, leaseId).Wait();
                _logger?.LogTrace("LeasedRetry - RetryAsync - released lease {KEY}", key);
            }

            return true;
        }
    }
}
