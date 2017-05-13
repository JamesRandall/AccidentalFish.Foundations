namespace AccidentalFish.Foundations.Resources.Abstractions.Blobs
{
    /// <summary>
    /// Factory for creating a blob repository
    /// </summary>
    public interface IBlobRepositoryFactory
    {
        /// <summary>
        /// Create a blob repository implementation looking at the specified container
        /// </summary>
        /// <param name="containerName">The container for the blobs</param>
        /// <returns>A blob repository implementation</returns>
        IAsyncBlockBlobRepository CreateAsyncBlockBlobRepository(string containerName);
    }
}
