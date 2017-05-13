using AccidentalFish.Foundations.Resources.Abstractions.Queues;

namespace AccidentalFish.Foundations.Resources.Azure.Queues
{
    public interface IAzureQueueFactory : IQueueFactory
    {
        IAsyncQueue<T> CreateAsyncBrokeredMessageQueue<T>(string queueName) where T : class;        
    }
}
