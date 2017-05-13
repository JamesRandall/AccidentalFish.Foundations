using System.Threading;
using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Runtime.HostableComponents
{
    public class HostableComponentWrapper : IHostableComponent
    {
        private readonly Task _wrappedTask;

        public HostableComponentWrapper(Task wrappedTask)
        {
            _wrappedTask = wrappedTask;
        }

        public Task StartAsync(CancellationToken token)
        {
            return _wrappedTask;
        }
    }
}
