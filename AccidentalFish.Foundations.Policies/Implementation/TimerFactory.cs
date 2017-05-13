using System;

namespace AccidentalFish.ApplicationSupport.Policies.Implementation
{
    internal class TimerFactory : ITimerFactory
    {
        private readonly ITimerThreadPoolExecuter _timerThreadPoolExecuter;
        private readonly IAsyncDelay _taskDelay;

        public TimerFactory(ITimerThreadPoolExecuter timerThreadPoolExecuter, IAsyncDelay taskDelay)
        {
            _timerThreadPoolExecuter = timerThreadPoolExecuter;
            _taskDelay = taskDelay;
        }

        public IAsyncIntervalTimer CreateAsyncIntervalTimer(TimeSpan interval, bool delayOnExecute = false)
        {
            return new AsyncIntervalTimer(_taskDelay, interval, delayOnExecute);
        }

        public IAsyncRegularTimer CreateAsyncRegularTimer(TimeSpan interval, bool delayOnExecute = false)
        {
            return new AsyncRegularTimer(_timerThreadPoolExecuter, _taskDelay, interval, delayOnExecute);
        }
    }
}
