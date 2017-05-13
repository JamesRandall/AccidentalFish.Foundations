using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    internal class CloudQueueItem<T> : QueueItem<T> where T : class
    {
        public CloudQueueItem(CloudQueueMessage message, T item, int dequeueCount, string popReceipt) : base(item, dequeueCount, popReceipt, null)
        {
            CloudQueueMessage = message;
        }

        public CloudQueueMessage CloudQueueMessage { get; set; }
    }
}
