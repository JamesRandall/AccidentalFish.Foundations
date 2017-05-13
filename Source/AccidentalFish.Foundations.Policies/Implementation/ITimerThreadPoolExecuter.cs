using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Policies.Implementation
{
    interface ITimerThreadPoolExecuter
    {
        Task Run(Action task, CancellationToken taskCancellationToken);
    }
}
