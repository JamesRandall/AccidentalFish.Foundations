using System;

namespace AccidentalFish.ApplicationSupport.Policies.Implementation
{
    internal class NotSupportedLeaseManagerFactory : ILeaseManagerFactory
    {
        public ILeaseManager<T> CreateLeaseManager<T>(string storageAccountConnectionString, string leaseBlockName)
        {
            throw new NotImplementedException();
        }
    }
}