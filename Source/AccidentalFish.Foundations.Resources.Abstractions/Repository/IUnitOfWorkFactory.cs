namespace AccidentalFish.Foundations.Resources.Abstractions.Repository
{
    /// <summary>
    /// Creates unit of work implementations
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Create an asynchronous unit of work
        /// </summary>
        /// <returns></returns>
        IUnitOfWorkAsync Create();
    }
}
