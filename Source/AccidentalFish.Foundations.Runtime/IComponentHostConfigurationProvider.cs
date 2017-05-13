using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Runtime
{
    /// <summary>
    /// Provides a component host configuration.
    /// </summary>
    public interface IComponentHostConfigurationProvider
    {
        /// <summary>
        /// Get the configuration
        /// </summary>
        /// <returns>An enumerable of component configurations</returns>
        Task<IEnumerable<ComponentConfiguration>> GetConfigurationAsync();
    }
}
