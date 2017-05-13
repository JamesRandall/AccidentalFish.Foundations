using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccidentalFish.ApplicationSupport.Policies.Implementation
{
    class TimerThreadPoolExecuter : ITimerThreadPoolExecuter
    {
        public Task Run(Action task, CancellationToken taskCancellationToken)
        {
            return Task.Run(task, taskCancellationToken);
        }
    }
}
