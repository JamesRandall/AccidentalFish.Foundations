using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation;
using AccidentalFish.Foundations.Resources.Azure.Queues;
using AccidentalFish.Foundations.Resources.Azure.Queues.Implementation;

namespace AccidentalFish.Foundations.Resources.Azure
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseAzureResources(this IDependencyResolver resolver)
        {
            resolver.Register<IAsyncBlobRepositoryFactory, AsyncBlobRepositoryFactory>();
            resolver.Register<IBlobRepositoryFactory, BlobRepositoryFactory>();
            resolver.Register<IQueueSerializer, QueueSerializer>();
            resolver.Register<IAzureQueueFactory, AzureQueueFactory>();
            resolver.Register<IQueueFactory, QueueFactory>();
            resolver.Register<IAsyncQueueFactory, AsyncQueueFactory>();
            return resolver;
        }
    }
}
