using AccidentalFish.ApplicationSupport.Policies;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.Foundations.Resources.Azure.Policies
{
    internal class LeaseManagerFactory : ILeaseManagerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public LeaseManagerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILeaseManager<T> CreateLeaseManager<T>(string storageAccountConnectionString, string leaseBlockName)
        {
            return new LeaseManager<T>(storageAccountConnectionString, leaseBlockName, _loggerFactory.CreateLogger<LeaseManager<T>>());
        }
    }
}
