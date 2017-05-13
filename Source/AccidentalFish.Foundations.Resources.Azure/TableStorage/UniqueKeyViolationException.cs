using System;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage
{   
    public class UniqueKeyViolationException : Exception
    {
        public UniqueKeyViolationException(string partitionKey, string rowKey, Exception innerException) : base("Unique key violation", innerException)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string PartitionKey { get; }
        public string RowKey { get; }        
    }
}
