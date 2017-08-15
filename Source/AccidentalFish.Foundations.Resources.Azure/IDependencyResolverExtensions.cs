using System;
using AccidentalFish.ApplicationSupport.Policies;
using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation;
using AccidentalFish.Foundations.Resources.Azure.EventHubs;
using AccidentalFish.Foundations.Resources.Azure.EventHubs.Implementation;
using AccidentalFish.Foundations.Resources.Azure.Implementation;
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
        /// <summary>
        /// Configures usage of Azure resources
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="createIfNoExists">True if factories should attempt to create underyling resource providers</param>
        /// <param name="eventHubSerializer">If unspecified JSON is used as the event hub serialization format, otherwise this should be a type that implements IEventHubSerializer</param>
        /// <returns></returns>
        public static IDependencyResolver UseAzureResources(this IDependencyResolver resolver, bool createIfNoExists=false, Type eventHubSerializer=null)
        {
            // settings
            IAzureSettings azureSettings = new AzureSettings(createIfNoExists);
            resolver.RegisterInstance(azureSettings);

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

            // event hubs
            resolver.Register<IEventHubClientFactory, EventHubClientFactory>();
            resolver.Register(typeof(IEventHubSerializer), eventHubSerializer ?? typeof(EventHubSerializer));

            return resolver;
        }

        /// <summary>
        /// Configures usage of Azure resources
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="createIfNoExists">True if factories should attempt to create underyling resource providers</param>        
        /// <returns></returns>
        public static IDependencyResolver UseAzureResources<TEventHubSerializer>(this IDependencyResolver resolver,
            bool createIfNoExists = false) where TEventHubSerializer : IEventHubSerializer
        {
            return UseAzureResources(resolver, createIfNoExists, typeof(TEventHubSerializer));
        }
    }
}
