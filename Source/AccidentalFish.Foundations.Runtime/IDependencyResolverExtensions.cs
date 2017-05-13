using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Runtime.Implementation;

namespace AccidentalFish.Foundations.Runtime
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseRuntime(this IDependencyResolver resolver)
        {
            resolver.Register<IComponentHost, ComponentHost>();
            resolver.Register<IComponentHostRestartHandler, DefaultComponentHostRestartHandler>();
            return resolver;
        }
    }
}
