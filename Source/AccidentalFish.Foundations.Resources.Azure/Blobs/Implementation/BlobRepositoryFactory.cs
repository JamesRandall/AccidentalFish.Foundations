using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation
{
    internal class BlobRepositoryFactory : IBlobRepositoryFactory
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILoggerFactory _loggerFactory;

        public BlobRepositoryFactory(IConnectionStringProvider connectionStringProvider,
            ILoggerFactory loggerFactory)
        {
            _connectionStringProvider = connectionStringProvider;
            _loggerFactory = loggerFactory;
        }

        public IAsyncBlockBlobRepository CreateAsyncBlockBlobRepository(string containerName)
        {
            return new AsyncBlockBlobRepository(_connectionStringProvider.Get<IAsyncBlobRepositoryFactory>(containerName), containerName, _loggerFactory);
        }
    }
}
