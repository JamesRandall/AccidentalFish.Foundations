using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage.Implementation
{
    internal interface ITableContinuationTokenSerializer
    {
        TableContinuationToken Deserialize(string serializedContinuationToken);
        string Serialize(TableContinuationToken continuationToken);
    }
}
