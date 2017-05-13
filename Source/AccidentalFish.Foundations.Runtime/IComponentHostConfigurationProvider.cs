using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.Runtime
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
