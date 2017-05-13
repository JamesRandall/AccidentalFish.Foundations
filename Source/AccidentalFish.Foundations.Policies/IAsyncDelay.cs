using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccidentalFish.ApplicationSupport.Policies
{
    /// <summary>
    /// Delays for a given interval
    /// </summary>
    public interface IAsyncDelay
    {
        /// <summary>
        /// Delay for the specified interval
        /// </summary>
        /// <param name="interval">Time do delay for</param>
        /// <returns>Awaitable task</returns>
        Task Delay(TimeSpan interval);

        /// <summary>
        /// Delay for the specified interval
        /// </summary>
        /// <param name="interval">Time do delay for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Awaitable task</returns>
        Task Delay(TimeSpan interval, CancellationToken cancellationToken);
    }
}
