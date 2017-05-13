using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage.Implementation
{
    internal class TableContinuationTokenSerializer : ITableContinuationTokenSerializer
    {
        public TableContinuationToken Deserialize(string serializedContinuationToken)
        {
            if (serializedContinuationToken == null) return null;

            TableContinuationTokenSerializer serializer = new TableContinuationTokenSerializer();
            TableContinuationToken continuationToken = serializer.Deserialize(serializedContinuationToken);
            
            return continuationToken;
        }

        public string Serialize(TableContinuationToken continuationToken)
        {
            if (continuationToken == null) return null;
            TableContinuationTokenSerializer serializer = new TableContinuationTokenSerializer();
            return serializer.Serialize(continuationToken);            
        }
    }
}
