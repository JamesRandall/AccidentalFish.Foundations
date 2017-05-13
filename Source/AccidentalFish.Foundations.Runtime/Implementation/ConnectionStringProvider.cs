using System.Threading.Tasks;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.Runtime.Implementation
{
    internal class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string _connectionString;

        public ConnectionStringProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<string> GetAsync<TFactory>(string resourceName)
        {
            return Task.FromResult(_connectionString);
        }

        public string Get<TFactory>(string resourceName)
        {
            return _connectionString;
        }
    }
}
