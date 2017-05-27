using System;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions;
using AccidentalFish.Foundations.Resources.Azure.Implementation;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage.Implementation
{
    class AsyncTableStorageRepositoryFactory : IAsyncTableStorageRepositoryFactory
    {
        private readonly ITableStorageQueryBuilder _tableStorageQueryBuilder;
        private readonly ITableContinuationTokenSerializer _tableContinuationTokenSerializer;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAzureSettings _azureSettings;

        public AsyncTableStorageRepositoryFactory(
            ITableStorageQueryBuilder tableStorageQueryBuilder,
            ITableContinuationTokenSerializer tableContinuationTokenSerializer,
            IConnectionStringProvider connectionStringProvider,
            ILoggerFactory loggerFactory,
            IAzureSettings azureSettings)
        {
            _tableStorageQueryBuilder = tableStorageQueryBuilder;
            _tableContinuationTokenSerializer = tableContinuationTokenSerializer;
            _connectionStringProvider = connectionStringProvider;
            _loggerFactory = loggerFactory;
            _azureSettings = azureSettings;
        }

        public async Task<IAsyncTableStorageRepository<T>> CreateAsync<T>(string tableName) where T : ITableEntity, new()
        {
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            string connectionString = await _connectionStringProvider.GetAsync<IAsyncTableStorageRepository<T>>(tableName);
            IAsyncTableStorageRepository<T> result = new AsyncTableStorageRepository<T>(connectionString, tableName, _tableStorageQueryBuilder, _tableContinuationTokenSerializer, _loggerFactory.CreateLogger<AsyncTableStorageRepository<T>>());

            if (_azureSettings.CreateIfNotExists)
            {
                await result.GetCloudTable().CreateIfNotExistsAsync();
            }

            return result;
        }
    }
}
