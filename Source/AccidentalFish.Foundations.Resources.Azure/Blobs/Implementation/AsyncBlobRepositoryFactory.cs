using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using AccidentalFish.Foundations.Resources.Azure.Implementation;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation
{
    class AsyncBlobRepositoryFactory : IAsyncBlobRepositoryFactory
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAzureSettings _azureSettings;

        public AsyncBlobRepositoryFactory(IConnectionStringProvider connectionStringProvider,
            ILoggerFactory loggerFactory,
            IAzureSettings azureSettings)
        {
            _connectionStringProvider = connectionStringProvider;
            _loggerFactory = loggerFactory;
            _azureSettings = azureSettings;
        }

        public async Task<IAsyncBlockBlobRepository> CreateAsyncBlockBlobRepositoryAsync(string containerName)
        {
            IAsyncBlockBlobRepository result = new AsyncBlockBlobRepository(await _connectionStringProvider.GetAsync<IAsyncBlockBlobRepository>(containerName), containerName, _loggerFactory);
            if (_azureSettings.CreateIfNotExists)
            {
                await result.GetContainer().CreateIfNotExistsAsync();
            }
            return result;
        }
    }
}
