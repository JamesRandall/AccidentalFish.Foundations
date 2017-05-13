using AccidentalFish.DependencyResolver;
using AccidentalFish.Foundations.Threading.Implementation;

namespace AccidentalFish.Foundations.Threading
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseThreading(this IDependencyResolver resolver)
        {
            resolver.Register<IWaitHandle, ManualResetEventWaitHandle>();
            return resolver;
        }
    }
}
