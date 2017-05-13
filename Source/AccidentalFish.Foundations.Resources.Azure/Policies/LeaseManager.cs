using System;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.Foundations.Resources.Azure.TableStorage;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AccidentalFish.Foundations.Resources.Azure.Policies
{
    internal class LeaseManager<T> : ILeaseManager<T>
    {
        private readonly ILogger<LeaseManager<T>> _logger;
        private readonly CloudBlobContainer _container;

        public LeaseManager(string storageAccountConnectionString, string leaseBlockName, ILogger<LeaseManager<T>> logger)
        {
            _logger = logger;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            _container = client.GetContainerReference(leaseBlockName);
        }

        public async Task<bool> CreateLeaseObjectIfNotExistAsync(T key)
        {
            string leaseName = GetLeaseName(key);
            CloudBlockBlob blob = _container.GetBlockBlobReference(leaseName);
            if (!(await blob.ExistsAsync()))
            {
                _logger?.LogTrace("LeaseManager: CreateLeaseObjectIfNotExistAsync - creating lease object");
                await blob.UploadTextAsync("");
                return true;
            }
            return false;
        }

        public async Task<string> LeaseAsync(T key)
        {
            return await LeaseAsync(key, TimeSpan.FromSeconds(30));
        }

        public async Task<string> LeaseAsync(T key, TimeSpan leaseTime)
        {
            string leaseName = GetLeaseName(key);
            CloudBlockBlob blob = _container.GetBlockBlobReference(leaseName);
            try
            {
                string leaseId = await blob.AcquireLeaseAsync(leaseTime, Guid.NewGuid().ToString());
                _logger?.LogTrace("LeaseManager: LeaseAsync - acquired lease for key {KEY}", key);
                return leaseId;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 400)
                {
                    _logger?.LogTrace("LeaseManager: LeaseAsync - failed to acquire lease for key {KEY}", key);
                    throw new UnableToAcquireLeaseException("Unable to acquire lease", ex);
                }
                throw;
            }
        }

        public async Task ReleaseAsync(T key, string leaseId)
        {
            string leaseName = GetLeaseName(key);
            CloudBlockBlob blob = _container.GetBlockBlobReference(leaseName);
            await blob.ReleaseLeaseAsync(new AccessCondition
            {
                LeaseId = leaseId
            });
            _logger?.LogTrace("LeaseManager: ReleaseAsync - released lease for key {KEY}", key);
        }

        public async Task RenewAsync(T key, string leaseId)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(GetLeaseName(key));
            await blob.RenewLeaseAsync(new AccessCondition
            {
                LeaseId = leaseId
            });
            _logger?.LogTrace("LeaseManager: RenewAsync -renewed lease for key {KEY}", key);
        }

        private static string GetLeaseName(T key)
        {
            string leaseName = $"{key}.lck";
            return leaseName;
        }
    }
}
