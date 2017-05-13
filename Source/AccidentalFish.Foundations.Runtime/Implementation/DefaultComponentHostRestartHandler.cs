using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.Runtime.Implementation
{
    internal class DefaultComponentHostRestartHandler : IComponentHostRestartHandler
    {
        public async Task<bool> HandleRestart(Exception ex, string name, int retryCount, ILogger logger)
        {
            try
            {
                bool doDelay = retryCount % 5 == 0;

                if (doDelay)
                {
                    logger?.LogWarning("Error {EX} occurred in component {NAME}. Restarting in 30 seconds.", ex, name);
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
                else
                {
                    logger?.LogWarning("Error {EX} occurred in component {NAME}. Restarting immediately.", ex, name);
                }
            }
            catch (Exception)
            {
                // swallow any issues
            }
            return true;
        }
    }
}
