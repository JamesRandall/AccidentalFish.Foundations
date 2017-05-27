using System;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using AccidentalFish.Foundations.Resources.Azure.Implementation;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation
{
    internal class BlobRepositoryFactory : IBlobRepositoryFactory
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAzureSettings _azureSettings;

        public BlobRepositoryFactory(IConnectionStringProvider connectionStringProvider,
            ILoggerFactory loggerFactory,
            IAzureSettings azureSettings)
        {
            _connectionStringProvider = connectionStringProvider;
            _loggerFactory = loggerFactory;
            _azureSettings = azureSettings;
        }

        public IAsyncBlockBlobRepository CreateAsyncBlockBlobRepository(string containerName)
        {
            if (_azureSettings.CreateIfNotExists)
            {
                throw new InvalidOperationException("Creation of resources can only be done through an asynchronous factory");
            }
            return new AsyncBlockBlobRepository(_connectionStringProvider.Get<IAsyncBlockBlobRepository>(containerName), containerName, _loggerFactory);
        }
    }
}
