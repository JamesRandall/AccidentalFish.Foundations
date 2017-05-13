using System.Reflection;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions.Repository;

namespace AccidentalFish.Foundations.Resources.Abstractions.Implementation
{
    internal class DualConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string _storageConnectionString;
        private readonly string _sqlConnectionString;

        public DualConnectionStringProvider(string storageConnectionString, string sqlConnectionString)
        {
            _storageConnectionString = storageConnectionString;
            _sqlConnectionString = sqlConnectionString;
        }

        public Task<string> GetAsync<TFactory>(string resourceName)
        {
            if (typeof(TFactory).GetTypeInfo().IsAssignableFrom(typeof(IUnitOfWorkFactory).GetTypeInfo()))
            {
                return Task.FromResult(_sqlConnectionString);
            }
            return Task.FromResult(_storageConnectionString);
        }

        public string Get<TFactory>(string resourceName)
        {
            if (typeof(TFactory).GetTypeInfo().IsAssignableFrom(typeof(IUnitOfWorkFactory).GetTypeInfo()))
            {
                return _sqlConnectionString;
            }
            return _storageConnectionString;
        }
    }
}
