using AccidentalFish.Foundations.Resources.Abstractions.Queues;

namespace AccidentalFish.Foundations.Resources.Azure.Queues
{
    public interface IAsyncBrokeredMessageQueue<T> : IAsyncQueue<T> where T : class
    {
    }
}
