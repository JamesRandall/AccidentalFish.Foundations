using AccidentalFish.Foundations.Resources.Abstractions.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Sql.Implementation
{
    class EntityFrameworkUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly DbContext _context;
        private readonly ILoggerFactory _loggerFactory;

        public EntityFrameworkUnitOfWorkFactory(IUnitOfWorkDbContextProvider contextProvider, ILoggerFactory loggerFactory)
        {
            _context = contextProvider.Context;
            _loggerFactory = loggerFactory;
        }

        public IUnitOfWorkAsync Create()
        {
            return new EntityFrameworkUnitOfWorkAsync(_context, _loggerFactory);
        }
    }
}
