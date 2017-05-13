using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Resources.Abstractions.Implementation
{
    internal class BasicConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string _connectionString;

        public BasicConnectionStringProvider(string connectionString)
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
