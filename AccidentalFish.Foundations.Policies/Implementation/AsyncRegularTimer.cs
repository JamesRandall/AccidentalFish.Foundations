using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccidentalFish.ApplicationSupport.Policies.Implementation
{
    internal class AsyncRegularTimer : IAsyncRegularTimer
    {
        private readonly ITimerThreadPoolExecuter _threadPoolExecuter;
        private readonly IAsyncDelay _taskDelay;
        private readonly TimeSpan _interval;
        private readonly bool _delayOnExecute;

        public AsyncRegularTimer(ITimerThreadPoolExecuter threadPoolExecuter, IAsyncDelay taskDelay, TimeSpan interval, bool delayOnExecute)
        {
            _threadPoolExecuter = threadPoolExecuter;
            _taskDelay = taskDelay;
            _interval = interval;
            _delayOnExecute = delayOnExecute;
        }

        public async Task ExecuteAsync(Action<CancellationToken> action, CancellationToken cancellationToken, Action shutdownAction = null)
        {
            if (_delayOnExecute)
            {
                await _taskDelay.Delay(_interval, cancellationToken);
            }
            
            while (!cancellationToken.IsCancellationRequested)
            {
                using (CancellationTokenSource timeLimitedTask = new CancellationTokenSource(_interval))
                {
                    await Task.WhenAll(_threadPoolExecuter.Run(() => action(cancellationToken), timeLimitedTask.Token), _taskDelay.Delay(_interval, cancellationToken));
                }
            }

            shutdownAction?.Invoke();
        }        
    }
}
