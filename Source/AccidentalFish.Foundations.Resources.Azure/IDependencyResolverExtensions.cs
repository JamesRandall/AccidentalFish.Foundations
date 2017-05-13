using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation;
using AccidentalFish.Foundations.Resources.Azure.Policies;
using AccidentalFish.Foundations.Resources.Azure.Queues;
using AccidentalFish.Foundations.Resources.Azure.Queues.Implementation;
using AccidentalFish.Foundations.Resources.Azure.TableStorage;
using AccidentalFish.Foundations.Resources.Azure.TableStorage.Implementation;

namespace AccidentalFish.Foundations.Resources.Azure
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseAzureResources(this IDependencyResolver resolver)
        {
            // blobs
            resolver.Register<IAsyncBlobRepositoryFactory, AsyncBlobRepositoryFactory>();
            resolver.Register<IBlobRepositoryFactory, BlobRepositoryFactory>();

            // queues
            resolver.Register<IQueueSerializer, QueueSerializer>();
            resolver.Register<IAzureQueueFactory, AzureQueueFactory>();
            resolver.Register<IQueueFactory, QueueFactory>();
            resolver.Register<IAsyncQueueFactory, AsyncQueueFactory>();

            // table storage
            resolver.Register<IAsyncTableStorageRepositoryFactory, AsyncTableStorageRepositoryFactory>();
            resolver.Register<ITableStorageRepositoryFactory, TableStorageRepositoryFactory>();
            resolver.Register<ITableStorageQueryBuilder, TableStorageQueryBuilder>();
            resolver.Register<ITableContinuationTokenSerializer, TableContinuationTokenSerializer>();
            resolver.Register<ITableStorageConcurrencyManager, TableStorageConcurrencyManager>();

            // policies
            resolver.Register<ILeaseManagerFactory, LeaseManagerFactory>();

            return resolver;
        }
    }
}
