using AccidentalFish.Foundations.Resources.Abstractions.Repository;
using AccidentalFish.Foundations.Resources.Sql.Implementation;
using Microsoft.EntityFrameworkCore;

namespace AccidentalFish.Foundations.Resources.Sql
{
    // ReSharper disable once InconsistentNaming
    public static class IRepositoryExtensions
    {
        public static DbSet<T> Set<T>(this IRepository<T> repository) where T : class
        {
            EntityFrameworkRepository<T> internalRepository = (EntityFrameworkRepository<T>) repository;
            return internalRepository.DbContext.Set<T>();
        }
    }
}
