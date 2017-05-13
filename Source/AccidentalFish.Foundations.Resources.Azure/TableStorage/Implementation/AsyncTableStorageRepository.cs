using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Azure.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage.Implementation
{
    internal class AsyncTableStorageRepository<T> : IAsyncTableStorageRepository<T> where T : ITableEntity, new()
    {
        private readonly string _tableName;
        private readonly ITableStorageQueryBuilder _tableStorageQueryBuilder;
        private readonly ITableContinuationTokenSerializer _tableContinuationTokenSerializer;
        private readonly ILogger<AsyncTableStorageRepository<T>> _logger;
        private readonly CloudTable _table;

        private const int MaxBatchSize = 100;

        public AsyncTableStorageRepository(
            string connectionString,
            string tableName,
            ITableStorageQueryBuilder tableStorageQueryBuilder,
            ITableContinuationTokenSerializer tableContinuationTokenSerializer,
            ILogger<AsyncTableStorageRepository<T>> logger)
        {
            _tableName = tableName;
            _tableStorageQueryBuilder = tableStorageQueryBuilder;
            _tableContinuationTokenSerializer = tableContinuationTokenSerializer;
            _logger = logger;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            tableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(120), 3);
            _table = tableClient.GetTableReference(tableName);

            _logger?.LogTrace("AsyncTableStorageRepository: created for table {TABLENAME}", tableName);
        }

        public async Task InsertAsync(T item)
        {
            TableOperation operation = TableOperation.Insert(item);
            try
            {
                _logger?.LogTrace("AsyncTableStorageRepository: InsertAsync - inserting item for table {TABLENAME}", _tableName);
                await _table.ExecuteAsync(operation);
            }
            catch (StorageException ex)
            {
                _logger?.LogTrace("AsyncTableStorageRepository: InsertAsync - storage exception for table {TABLENAME}", ex, _tableName);
                if (ex.RequestInformation.HttpStatusCode == 409)
                {
                    throw new UniqueKeyViolationException(item.PartitionKey, item.RowKey, ex);
                }
                throw;
            }
            
        }

        public async Task InsertBatchAsync(IEnumerable<T> items)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: InsertBatchAsync - inserting items for table {TABLENAME}", _tableName);
            List<Task> tasks = new List<Task>();
            foreach (IEnumerable<T> page in items.Page(MaxBatchSize))
            {
                TableBatchOperation operation = new TableBatchOperation();
                foreach (T item in page)
                {
                    operation.Insert(item);
                }
                tasks.Add(_table.ExecuteBatchAsync(operation));
            }
            _logger?.LogTrace("AsyncTableStorageRepository: InsertBatchAsync - broken batch into {PAGES} pages for table {TABLENAME}", tasks.Count, _tableName);

            await Task.WhenAll(tasks);
        }

        public async Task InsertOrReplaceBatchAsync(IEnumerable<T> items)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: InsertOrReplaceBatchAsync - insert or replacing items for table {TABLENAME}", _tableName);
            List<Task> tasks = new List<Task>();
            foreach (IEnumerable<T> page in items.Page(MaxBatchSize))
            {
                TableBatchOperation operation = new TableBatchOperation();
                foreach (T item in page)
                {
                    operation.InsertOrReplace(item);
                }
                tasks.Add(_table.ExecuteBatchAsync(operation));
            }
            _logger?.LogTrace("AsyncTableStorageRepository: InsertOrReplaceBatchAsync - broken batch into {PAGES} pages for table {TABLENAME}", tasks.Count, _tableName);

            await Task.WhenAll(tasks);
        }

        public async Task<T> GetAsync(string partitionKey, string rowKey)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: GetAsync - getting item with partition and row key for table {TABLENAME}", _tableName);
            TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult result = await _table.ExecuteAsync(operation);
            return (T)result.Result;
        }

        public async Task<IEnumerable<T>> GetAsync(string partitionKey)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: GetAsync - getting items for a partition for table {TABLENAME}", _tableName);
            TableQuery<T> query = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            IEnumerable<T> result = await _table.ExecuteQueryAsync(query);
            return result;
        }

        public async Task<IEnumerable<T>> GetAsync(string partitionKey, int take)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: GetAsync - getting {ITEMS} items for a partition for table {TABLENAME}", take, _tableName);
            TableQuery<T> query = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey))
                .Take(take);
            IEnumerable<T> result = await _table.ExecuteQueryAsync(query);
            return result;
        }

        public Task InsertOrUpdateAsync(T item)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: InsertOrUpdateAsync - for table {TABLENAME}", _tableName);
            TableOperation operation = TableOperation.InsertOrMerge(item);
            return _table.ExecuteAsync(operation);
        }

        public Task InsertOrReplaceAsync(T item)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: InsertOrReplaceAsync - for table {TABLENAME}", _tableName);
            TableOperation operation = TableOperation.InsertOrReplace(item);
            return _table.ExecuteAsync(operation);
        }

        public async Task<bool> DeleteAsync(T item)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: DeleteAsync - for table {TABLENAME}", _tableName);
            TableOperation operation = TableOperation.Delete(item);
            TableResult result = await _table.ExecuteAsync(operation);
            return result.HttpStatusCode == 204;
        }

        public Task UpdateAsync(T item)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: UpdateAsync - for table {TABLENAME}", _tableName);
            TableOperation operation = TableOperation.Replace(item);
            return _table.ExecuteAsync(operation);
        }

        #region Querying

        public async Task<IEnumerable<T>> QueryAsync(string column, string value)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: QueryAsync - on column {COLUMN} for table {TABLENAME}", column, _tableName);
            List<T> results = new List<T>();
            TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(column, QueryComparisons.Equal, value));
            TableQuerySegment<T> querySegment = null;

            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await _table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                results.AddRange(querySegment.Results);
            }
            _logger?.LogTrace("AsyncTableStorageRepository: QueryAsync - on column {COLUMN} for table {TABLENAME} returned {COUNT} results", column, _tableName, results.Count);
            return results;
        }

        public async Task<IEnumerable<T>> QueryAsync(Dictionary<string, object> columnValues)
        {
            if (columnValues != null && columnValues.Keys.Any())
            {
                _logger?.LogTrace("AsyncTableStorageRepository: QueryAsync - on columns {COLUMNS} for table {TABLENAME}",
                    String.Join(",", columnValues.Keys), _tableName);
            }
            else
            {
                _logger?.LogTrace("AsyncTableStorageRepository: QueryAsync - no column value pairs specified for table {TABLENAME}", _tableName);
            }

            List<T> results = new List<T>();

            TableQuery<T> query = _tableStorageQueryBuilder.TableQuery<T>(columnValues, TableStorageQueryOperator.And);
            TableQuerySegment<T> querySegment = null;

            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await _table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                results.AddRange(querySegment.Results);
            }
            _logger?.LogTrace("AsyncTableStorageRepository: QueryAsync - on columns for table {TABLENAME} returned {ROWS} rows",
                     _tableName, results.Count);
            return results;
        }

        public async Task QueryActionAsync(string column, string value, Func<IEnumerable<T>, Task> action)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: QueryActionAsync - on column {COLUMN} for table {TABLENAME}", column, _tableName);
            var query = !String.IsNullOrWhiteSpace(column) ? new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(column, QueryComparisons.Equal, value)) : new TableQuery<T>();
            
            TableQuerySegment<T> querySegment = null;
            int results = 0;

            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await _table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                results += querySegment.Results.Count;
                await action(new List<T>(querySegment.Results));
            }
            _logger?.LogTrace("AsyncTableStorageRepository: QueryActionAsync - on column {COLUMN} for table {TABLENAME} processed {ROWS} rows", column, _tableName, results);
        }

        public async Task QueryFuncAsync(string column, string value, Func<IEnumerable<T>, Task<bool>> func)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: QueryFuncAsync - on column {COLUMN} for table {TABLENAME}", column, _tableName);

            var query = !String.IsNullOrWhiteSpace(column) ? new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(column, QueryComparisons.Equal, value)) : new TableQuery<T>();

            TableQuerySegment<T> querySegment = null;
            bool doContinue = true;
            int results = 0;

            while ((querySegment == null || querySegment.ContinuationToken != null) && doContinue)
            {
                querySegment = await _table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                results += querySegment.Results.Count;
                doContinue = await func(new List<T>(querySegment.Results));
            }

            _logger?.LogTrace("AsyncTableStorageRepository: QueryFuncAsync - on column {COLUMN} for table {TABLENAME} processed {ROWS} rows", column, _tableName, results);
        }

        public async Task QueryFuncAsync(string column, IEnumerable<object> values, TableStorageQueryOperator op,
            Func<IEnumerable<T>, Task<bool>> func)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: QueryFuncAsync - on column {COLUMN} for table {TABLENAME} with operator {OPERATOR}", column, _tableName, op);

            TableQuery<T> query = _tableStorageQueryBuilder.TableQuery<T>(column, values, op);
            
            TableQuerySegment<T> querySegment = null;
            bool doContinue = true;
            int results = 0;

            while ((querySegment == null || querySegment.ContinuationToken != null) && doContinue)
            {
                querySegment = await _table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                results += querySegment.Results.Count;
                doContinue = await func(new List<T>(querySegment.Results));
            }
            _logger?.LogTrace("AsyncTableStorageRepository: QueryFuncAsync - on column {COLUMN} for table {TABLENAME} with operator {OPERATOR} processed {ROWS} rows", column, _tableName, op, results);
        }

        public async Task QueryFuncAsync(Dictionary<string, object> conditions, TableStorageQueryOperator op, Func<IEnumerable<T>, Task<bool>> func)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: QueryFuncAsync - on multiple columns for table {TABLENAME} with operator {OPERATOR}", _tableName, op);
            TableQuery<T> query =  _tableStorageQueryBuilder.TableQuery<T>(conditions, op);
            TableQuerySegment<T> querySegment = null;
            bool doContinue = true;
            int results = 0;

            while ((querySegment == null || querySegment.ContinuationToken != null) && doContinue)
            {
                querySegment = await _table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                results += querySegment.Results.Count;
                doContinue = await func(new List<T>(querySegment.Results));
            }
            _logger?.LogTrace("AsyncTableStorageRepository: QueryFuncAsync - on multiple columns for table {TABLENAME} with operator {OPERATOR} processed {ROWS} rows", _tableName, op, results);
        }

        public async Task AllActionAsync(Func<IEnumerable<T>, Task> action)
        {
            await QueryActionAsync(null, null, action);
        }

        public Task<PagedResultSegment<T>> PagedQueryAsync(Dictionary<string, object> columnValues, int pageSize)
        {
            return PagedQueryAsync(columnValues, TableStorageQueryOperator.And, pageSize);
        }

        public Task<PagedResultSegment<T>> PagedQueryAsync(Dictionary<string, object> columnValues, int pageSize, string serializedContinuationToken)
        {
            return PagedQueryAsync(columnValues, TableStorageQueryOperator.And, pageSize, serializedContinuationToken);
        }

        #endregion

        #region Paged queries

        public Task<PagedResultSegment<T>> PagedQueryAsync(Dictionary<string, object> columnValues, TableStorageQueryOperator op, int pageSize)
        {
            return PagedQueryAsync(columnValues, pageSize, null);
        }

        public async Task<PagedResultSegment<T>> PagedQueryAsync(Dictionary<string, object> columnValues, TableStorageQueryOperator op, int pageSize, string serializedContinuationToken)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: PagedQueryAsync - for table {TABLENAME} with operator {OPERATOR} and page size {PAGESIZE}", _tableName, op, pageSize);
            TableQuery<T> query = _tableStorageQueryBuilder.TableQuery<T>(columnValues, op).Take(pageSize);

            TableContinuationToken continuationToken = _tableContinuationTokenSerializer.Deserialize(serializedContinuationToken);

            TableQuerySegment<T> querySegment = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);

            _logger?.LogTrace("AsyncTableStorageRepository: PagedQueryAsync - for table {TABLENAME} with operator {OPERATOR} and page size {PAGESIZE} returned {ROWS}", _tableName, op, pageSize, querySegment.Results.Count);

            return new PagedResultSegment<T>
            {
                ContinuationToken = _tableContinuationTokenSerializer.Serialize(querySegment.ContinuationToken),
                Page = new List<T>(querySegment.Results)
            };
        }

        public async Task<PagedResultSegment<T>> PagedQueryAsync(string filter, int pageSize,
            string serializedContinuationToken)
        {
            _logger?.LogTrace("AsyncTableStorageRepository: PagedQueryAsync - for table {TABLENAME} with page size {PAGESIZE}", _tableName, pageSize);

            TableQuery<T> tableQuery = new TableQuery<T>();
            tableQuery.Where(filter);

            TableContinuationToken continuationToken = _tableContinuationTokenSerializer.Deserialize(serializedContinuationToken);

            TableQuerySegment<T> querySegment = await _table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

            _logger?.LogTrace("AsyncTableStorageRepository: PagedQueryAsync - for table {TABLENAME} with page size {PAGESIZE} returned {ROWS} rows", _tableName, pageSize, querySegment.Results.Count);

            return new PagedResultSegment<T>
            {
                ContinuationToken = _tableContinuationTokenSerializer.Serialize(querySegment.ContinuationToken),
                Page = new List<T>(querySegment.Results)
            };
        }

        #endregion

        internal CloudTable Table => _table;
    }
}
