using System;
using System.Linq;
using System.Linq.Expressions;

namespace AccidentalFish.Foundations.Resources.Abstractions.Repository
{
    /// <summary>
    /// Repository pattern for use with an ORM that supports asynchronous access
    /// </summary>
    /// <typeparam name="T">The type of the objects represented by the repository</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get an IQueryable interface for the repository
        /// </summary>
        IQueryable<T> All { get; }
        /// <summary>
        /// Get an IQueryable interface for the repository including child object / collection references
        /// </summary>
        /// <param name="includeProperties">The child object / collection references to also fetch</param>
        /// <returns>An IQueryable interface</returns>
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        /// <summary>
        /// Insert the given entity
        /// </summary>
        /// <param name="entity">The entity to insert</param>
        void Insert(T entity);
        /// <summary>
        /// Update the entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        void Update(T entity);
        /// <summary>
        /// Delete the entity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        void Delete(T entity);
    }
}
