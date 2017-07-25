using System;
using System.Linq;
using System.Linq.Expressions;
using AccidentalFish.Foundations.Resources.Abstractions.Repository;
using Microsoft.EntityFrameworkCore;

namespace AccidentalFish.Foundations.Resources.Sql.Implementation
{
    class EntityFrameworkRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;

        public EntityFrameworkRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> All => _dbContext.Set<T>();

        public IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            return includeProperties.Aggregate(All, (current, includeProperty) => current.Include(includeProperty));
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        
        public void Insert(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        
        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
