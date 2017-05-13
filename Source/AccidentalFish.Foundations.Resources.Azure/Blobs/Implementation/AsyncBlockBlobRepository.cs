using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation
{
    internal class AsyncBlockBlobRepository : IAsyncBlockBlobRepository
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<AsyncBlockBlobRepository> _logger;
        private readonly CloudBlobContainer _container;
        private readonly string _endpoint;
        
        public AsyncBlockBlobRepository(
            string storageAccountConnectionString,
            string containerName,
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AsyncBlockBlobRepository>();
            if (String.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (String.IsNullOrWhiteSpace(storageAccountConnectionString)) throw new ArgumentNullException(nameof(storageAccountConnectionString));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            client.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(120), 3);
            _container = client.GetContainerReference(containerName);

            // Development storage doesn't return a '/' on the end, live storage does. 
            _endpoint = !client.BaseUri.ToString().EndsWith("/") ? $"{client.BaseUri}/{containerName}" : $"{client.BaseUri}{containerName}";
            

            _logger?.LogTrace("AsyncBlockBlobRepository: create repository for endpoint {ENDPOINT}", _endpoint);
        }

        internal CloudBlobContainer CloudBlobContainer => _container;

        public async Task<IBlob> UploadAsync(string name, Stream stream)
        {
            _logger?.LogTrace("AsyncBlockBlobRepository: UploadAsync - attempting to upload blob {NAME}", name);
            CloudBlockBlob blob = _container.GetBlockBlobReference(name);
            await blob.UploadFromStreamAsync(stream);                
                
            BlockBlob result = new BlockBlob(blob, name, _loggerFactory.CreateLogger<BlockBlob>());

            _logger?.LogTrace("AsyncBlockBlobRepository: UploadAsync - successfull uploaded blob {NAME}", name);

            return result;
        }

        public IBlob Get(string name)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(name);
            return new BlockBlob(blob, name, _loggerFactory.CreateLogger<BlockBlob>());
        }

        public async Task<IReadOnlyCollection<IBlob>> ListAsync()
        {
            BlobContinuationToken continuationToken = null;
            List<IBlob> results = new List<IBlob>();
            do
            {
                var response = await _container.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results.Select(x => new BlockBlob(x as CloudBlockBlob, Path.GetFileName(x.Uri.LocalPath), _loggerFactory.CreateLogger<BlockBlob>())));
            }
            while (continuationToken != null);
            return results;
        }

        public async Task<IReadOnlyCollection<IBlob>> ListAsync(string prefix)
        {
            BlobContinuationToken continuationToken = null;
            List<IBlob> results = new List<IBlob>();
            do
            {
                var response = await _container.ListBlobsSegmentedAsync(prefix, continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results.Select(x => new BlockBlob(x as CloudBlockBlob, Path.GetFileName(x.Uri.LocalPath), _loggerFactory.CreateLogger<BlockBlob>())));
            }
            while (continuationToken != null);
            return results;
        }

        public string Endpoint => _endpoint;

        internal CloudBlobContainer Container => _container;
    }
}
