using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Sql.Implementation
{
    class EntityFrameworkUnitOfWorkAsync : IUnitOfWorkAsync
    {
        private readonly DbContext _context;
        private readonly bool _isOwnedContext;
        private readonly ILogger<EntityFrameworkUnitOfWorkAsync> _logger;

        public EntityFrameworkUnitOfWorkAsync(DbContext context,
            ILoggerFactory loggerFactory,
            bool isOwnedContext = false)
        {
            _context = context;
            _isOwnedContext = isOwnedContext;
            _logger = loggerFactory.CreateLogger<EntityFrameworkUnitOfWorkAsync>();
        }

        public void Dispose()
        {
            if (_isOwnedContext)
            {
                _context.Dispose();
            }
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new EntityFrameworkRepository<T>(_context);
        }

        public async Task<int> SaveAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();
            int result = await _context.SaveChangesAsync();
            sw.Stop();
            _logger.LogTrace("EntityFrameworkUnitOfWorkAsync: Committed changes in {0}ms", sw.ElapsedMilliseconds);
            return result;
        }

        public Task OptimisticRepositoryWinsUpdateAsync(Action update)
        {
            return OptimisticRepositoryWinsUpdateAsync(update, int.MaxValue);
        }

        public async Task<bool> OptimisticRepositoryWinsUpdateAsync(Action update, int maxRetries)
        {
            bool saveFailed;
            int retries = 0;
            do
            {
                saveFailed = false;
                try
                {
                    _logger?.LogTrace("EntityFrameworkUnitOfWorkAsync: OptimisticRepositoryWinsUpdateAsync - attempting update retry {0}", retries);
                    update();
                    await SaveAsync();
                    _logger?.LogTrace("EntityFrameworkUnitOfWorkAsync: OptimisticRepositoryWinsUpdateAsync - update succeeded on retry {0}", retries);
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    _logger?.LogTrace("EntityFrameworkUnitOfWorkAsync: OptimisticRepositoryWinsUpdateAsync - update failed on retry {0}", retries);
                    retries++;
                    foreach (EntityEntry entity in concurrencyException.Entries) entity.Reload();
                    saveFailed = true;
                }
            } while (saveFailed && retries < maxRetries);
            if (saveFailed)
            {
                _logger?.LogTrace("EntityFrameworkUnitOfWorkAsync: OptimisticRepositoryWinsUpdateAsync - updated failed after max retried of {0}", maxRetries);
            }
            return !saveFailed;
        }
    }
}
