using Microsoft.EntityFrameworkCore;

namespace AccidentalFish.Foundations.Resources.Sql.Implementation
{
    internal interface IUnitOfWorkDbContextProvider
    {
        DbContext Context { get; }
    }
}
