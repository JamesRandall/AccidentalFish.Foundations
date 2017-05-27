using AccidentalFish.Foundations.Resources.Azure.TableStorage.Implementation;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage
{
    // ReSharper disable once InconsistentNaming
    public static class IAsyncTableStorageRepositoryExtensions
    {
        public static CloudTable GetCloudTable<T>(this IAsyncTableStorageRepository<T> repository) where T : ITableEntity, new()
        {
            return ((AsyncTableStorageRepository<T>)repository).Table;
        }
    }
}
