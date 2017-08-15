using Newtonsoft.Json;

namespace AccidentalFish.Foundations.Resources.Azure.EventHubs.Implementation
{
    internal class EventHubSerializer : IEventHubSerializer
    {
        public string Serialize<T>(T item)
        {
            string json = JsonConvert.SerializeObject(item);
            return json;
        }
    }
}
