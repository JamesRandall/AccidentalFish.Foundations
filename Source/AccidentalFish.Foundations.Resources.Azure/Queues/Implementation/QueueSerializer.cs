using AccidentalFish.Foundations.Resources.Abstractions.Queues;
using Newtonsoft.Json;

namespace AccidentalFish.Foundations.Resources.Azure.Queues.Implementation
{
    internal class QueueSerializer : IQueueSerializer
    {
        public string Serialize<T>(T item) where T : class
        {
            return JsonConvert.SerializeObject(item);
        }

        public T Deserialize<T>(string item) where T : class
        {
            return JsonConvert.DeserializeObject<T>(item);
        }
    }
}
