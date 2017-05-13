using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Resources.Abstractions.Blobs;
using AccidentalFish.Foundations.Resources.Azure.Blobs.Implementation;

namespace AccidentalFish.Foundations.Resources.Azure
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseAzureResources(this IDependencyResolver resolver)
        {
            resolver.Register<IAsyncBlobRepositoryFactory, AsyncBlobRepositoryFactory>();
            resolver.Register<IBlobRepositoryFactory, BlobRepositoryFactory>();
            return resolver;
        }
    }
}
