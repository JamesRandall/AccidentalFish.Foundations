using System;
using AccidentalFish.ApplicationSupport.Policies;

namespace AccidentalFish.Foundations.Policies.Implementation
{
    internal class NotSupportedLeaseManagerFactory : ILeaseManagerFactory
    {
        public ILeaseManager<T> CreateLeaseManager<T>(string storageAccountConnectionString, string leaseBlockName)
        {
            throw new NotImplementedException();
        }
    }
}