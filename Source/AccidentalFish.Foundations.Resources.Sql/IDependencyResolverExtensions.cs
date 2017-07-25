using System;
using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Resources.Abstractions.Repository;
using AccidentalFish.Foundations.Resources.Sql.Implementation;
using Microsoft.EntityFrameworkCore;

namespace AccidentalFish.Foundations.Resources.Sql
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseSql<T>(this IDependencyResolver resolver) where T : DbContext
        {
            return UseSql(resolver, typeof(T));
        }

        public static IDependencyResolver UseSql(this IDependencyResolver resolver, Type dbContextType)
        {
            IUnitOfWorkDbContextProvider unitOfWorkDbContextProvider = new UnitOfWorkDbContextProvider(() => (DbContext)resolver.Resolve(dbContextType));
            resolver.RegisterInstance(unitOfWorkDbContextProvider);
            resolver.Register<IUnitOfWorkFactory, EntityFrameworkUnitOfWorkFactory>();
            return resolver;
        }
    }
}
