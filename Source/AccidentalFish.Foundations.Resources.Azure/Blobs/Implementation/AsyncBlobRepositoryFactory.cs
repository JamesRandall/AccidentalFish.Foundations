using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation
{
    class AsyncBlobRepositoryFactory : IAsyncBlobRepositoryFactory
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILoggerFactory _loggerFactory;

        public AsyncBlobRepositoryFactory(IConnectionStringProvider connectionStringProvider,
            ILoggerFactory loggerFactory)
        {
            _connectionStringProvider = connectionStringProvider;
            _loggerFactory = loggerFactory;
        }

        public async Task<IAsyncBlockBlobRepository> CreateAsyncBlockBlobRepositoryAsync(string containerName)
        {
            return new AsyncBlockBlobRepository(await _connectionStringProvider.GetAsync<IAsyncBlobRepositoryFactory>(containerName), containerName, _loggerFactory);
        }
    }
}
