using System.Threading;
using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Runtime
{
    /// <summary>
    /// A hostable component is designed to be run in a worker role or web job and perform background activities
    /// </summary>
    public interface IHostableComponent
    {
        /// <summary>
        /// Starts the component
        /// </summary>
        /// <param name="token"></param>
        /// <returns>The task the component is running in</returns>
        Task StartAsync(CancellationToken token);
    }
}
