using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Resources.Abstractions.Blobs
{
    /// <summary>
    /// Factory for creating a blob repository
    /// </summary>
    public interface IAsyncBlobRepositoryFactory
    {
        /// <summary>
        /// Create a blob repository implementation looking at the specified container
        /// </summary>
        /// <param name="containerName">The container for the blobs</param>
        /// <returns>A blob repository implementation</returns>
        Task<IAsyncBlockBlobRepository> CreateAsyncBlockBlobRepositoryAsync(string containerName);
    }
}
