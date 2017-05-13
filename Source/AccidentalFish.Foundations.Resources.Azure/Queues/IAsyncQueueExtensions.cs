using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using AccidentalFish.Foundations.Resources.Azure.Queues.Implementation;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Foundations.Resources.Azure.Queues
{
    // ReSharper disable once InconsistentNaming
    public static class IAsyncQueueExtensions
    {
        public static CloudQueue GetCloudQueue<T>(this IAsyncQueue<T> asyncQueue) where T : class
        {
            AsyncQueue<T> queueImpl = asyncQueue as AsyncQueue<T>;
            return queueImpl?.UnderlyingQueue;
        }
    }
}
