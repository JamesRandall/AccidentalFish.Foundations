using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Runtime
{
    /// <summary>
    /// Implementations of this interface handle the restart of components running in the component host when they error.
    /// The default implementation will log the error and restart the component pausing for 30 seconds on every fifth error.
    /// </summary>
    public interface IComponentHostRestartHandler
    {
        /// <summary>
        /// Called when a component faults
        /// </summary>
        /// <param name="ex">The exception that triggered the fault</param>
        /// <param name="count"></param>
        /// <param name="retryCount">The number of times the component has already been restarted</param>
        /// <param name="logger">An optional logger - null if no logger is configured</param>
        /// <returns>True if the component instance should be restarted, false if it should be terminated</returns>
        Task<bool> HandleRestart(Exception ex, string count, int retryCount, ILogger logger);
    }
}
