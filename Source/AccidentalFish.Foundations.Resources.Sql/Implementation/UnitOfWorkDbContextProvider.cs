using System;
using Microsoft.EntityFrameworkCore;

namespace AccidentalFish.Foundations.Resources.Sql.Implementation
{
    internal class UnitOfWorkDbContextProvider : IUnitOfWorkDbContextProvider
    {
        private readonly Func<DbContext> _resolveFunc;

        public UnitOfWorkDbContextProvider(Func<DbContext> resolveFunc)
        {
            _resolveFunc = resolveFunc;
        }

        public DbContext Context => _resolveFunc();
    }
}
