using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage
{
    interface IAsyncTableStorageRepositoryFactory
    {
        Task<IAsyncTableStorageRepository<T>> CreateAsync<T>(string tableName) where T : ITableEntity, new();
    }
}
