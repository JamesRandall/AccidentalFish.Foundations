using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Resources.Abstractions.Implementation;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using AccidentalFish.Foundations.Resources.Abstractions.Queues.Implementation;

namespace AccidentalFish.Foundations.Resources.Abstractions
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Register dependencies for using resources
        /// </summary>
        /// <param name="resolver">The dependency resolver to register within</param>
        /// <param name="connectionString">A connection string for all storage</param>
        /// <returns>A dependency resolver</returns>
        public static IDependencyResolver UseAbstractReources(this IDependencyResolver resolver, string connectionString)
        {
            IConnectionStringProvider connectionStringProvider = new BasicConnectionStringProvider(connectionString);
            resolver.RegisterInstance(connectionStringProvider);
            Register(resolver);
            return resolver;
        }

        /// <summary>
        /// Register dependencies for using resources
        /// </summary>
        /// <param name="resolver">The dependency resolver to register within</param>
        /// <param name="storageAccountConnectionString">A connection string for general storage</param>
        /// <param name="sqlConnectionString">A connection string for SQL</param>
        /// <returns>A dependency resolver</returns>
        public static IDependencyResolver UseAbstractReources(this IDependencyResolver resolver, string storageAccountConnectionString, string sqlConnectionString)
        {
            IConnectionStringProvider connectionStringProvider = new DualConnectionStringProvider(storageAccountConnectionString, sqlConnectionString);
            resolver.RegisterInstance(connectionStringProvider);
            Register(resolver);
            return resolver;
        }

        /// <summary>
        /// Register dependencies for using resources
        /// </summary>
        /// <param name="resolver">The dependency resolver to register within</param>
        /// <typeparam name="TConnectionStringProvider">The type of a custom connection string provider</typeparam>
        /// <returns>A dependency resolver</returns>
        public static IDependencyResolver UseAbstractReources<TConnectionStringProvider>(this IDependencyResolver resolver) where TConnectionStringProvider : IConnectionStringProvider
        {
            resolver.Register<IConnectionStringProvider, TConnectionStringProvider>();
            Register(resolver);
            return resolver;
        }

        /// <summary>
        /// Register dependencies for using resources
        /// </summary>
        /// <param name="resolver">The dependency resolver to register within</param>
        /// <param name="connectionStringProvider">Custom connection string provider instance</param>
        /// <returns>A dependency resolver</returns>
        public static IDependencyResolver UseAbstractReources(this IDependencyResolver resolver, IConnectionStringProvider connectionStringProvider)
        {
            resolver.RegisterInstance(connectionStringProvider);
            Register(resolver);
            return resolver;
        }

        private static void Register(IDependencyResolver resolver)
        {
            
            resolver.Register<IAsyncLargeMessageQueueFactory, AsyncLargeMessageQueueFactory>();
            resolver.Register<ILargeMessageQueueFactory, LargeMessageQueueFactory>();
        }
    }
}
