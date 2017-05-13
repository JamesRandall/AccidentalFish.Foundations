using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage.Implementation
{
    interface ITableStorageQueryBuilder
    {
        TableQuery<T> TableQuery<T>(Dictionary<string, object> columnValues, TableStorageQueryOperator op) where T : ITableEntity, new();
        TableQuery<T> TableQuery<T>(string column, IEnumerable<object> values, TableStorageQueryOperator op) where T : ITableEntity, new();
    }
}
