using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AccidentalFish.Foundations.Resources.Azure.Blobs
{
    // ReSharper disable once InconsistentNaming
    public static class IAsyncBlockBlobRepositoryExtensions
    {
        public static string GetSharedAccessSignature(this IAsyncBlockBlobRepository repository, SharedAccessBlobPolicy policy)
        {
            AsyncBlockBlobRepository blobRepository = (AsyncBlockBlobRepository)repository;
            return blobRepository.Container.GetSharedAccessSignature(policy);
        }

        public static CloudBlobContainer GetContainer(this IAsyncBlockBlobRepository repository)
        {
            AsyncBlockBlobRepository blobRepository = (AsyncBlockBlobRepository)repository;
            return blobRepository.Container;
        }
    }
}
