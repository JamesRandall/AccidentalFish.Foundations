using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Policies.Implementation;

namespace AccidentalFish.Foundations.Policies
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Registers policies.
        /// Note that the LeasedRetry requires a registered ILeaseManager and this needs to be supplied by a resource implementation
        /// such as AccidentalFish.Foundations.Resources.Azure
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static IDependencyResolver UsePolicies(this IDependencyResolver resolver)
        {
            resolver.Register<IAsyncDelay, AsyncDelay>()
                .Register<ITimerThreadPoolExecuter, TimerThreadPoolExecuter>()
                .Register<ITimerFactory, TimerFactory>()
                .Register<IBackoffPolicy, BackoffPolicy>()
                .Register<IBackoffPolicyTimingProvider, BackoffPolicyDefaultTimingProvider>()
                .Register<IAsyncBackoffPolicy, AsyncBackoffPolicy>()
                .Register<IBackoffPolicyFactory, BackoffPolicyFactory>()
                .Register<ILeasedRetry, LeasedRetry>();
            return resolver;
        }
    }
}
