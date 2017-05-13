using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage
{
    public interface ITableStorageRepositoryFactory
    {
        IAsyncTableStorageRepository<T> Create<T>(string tableName) where T : ITableEntity, new();
    }
}
