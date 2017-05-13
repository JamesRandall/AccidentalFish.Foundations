using System;
using System.Threading.Tasks;
using AccidentalFish.Foundations.Resources.Abstractions.Queues;

namespace AccidentalFish.Foundations.Resources.Azure.Queues
{
    public interface IAsyncStorageQueue<T> : IAsyncQueue<T> where T : class
    {
        Task DequeueAsync(Func<IQueueItem<T>, Task<bool>> processor, TimeSpan? visibilityTimeout);
    }
}
