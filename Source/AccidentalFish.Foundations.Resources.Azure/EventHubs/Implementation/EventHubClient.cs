using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace AccidentalFish.Foundations.Resources.Azure.EventHubs.Implementation
{
    internal class EventHubClient : IEventHubClient
    {
        private readonly Microsoft.Azure.EventHubs.EventHubClient _client;
        private readonly IEventHubSerializer _serializer;

        public EventHubClient(Microsoft.Azure.EventHubs.EventHubClient client, IEventHubSerializer serializer)
        {
            _client = client;
            _serializer = serializer;
        }
        
        public Task SendAsync(string text, string partitionKey)
        {
            return _client.SendAsync(new EventData(Encoding.UTF8.GetBytes(text)), partitionKey);
        }

        public Task SendAsync(string text)
        {
            return _client.SendAsync(new EventData(Encoding.UTF8.GetBytes(text)));
        }

        public Task SendAsync(byte[] bytes)
        {
            return _client.SendAsync(new EventData(bytes));
        }

        public Task SendAsync(byte[] bytes, string partitionKey)
        {
            return _client.SendAsync(new EventData(bytes), partitionKey);
        }

        public Task SendAsync<T>(T payload)
        {
            string serializedContent = _serializer.Serialize(payload);
            return SendAsync(serializedContent);
        }

        public Task SendAsync<T>(T payload, string partitionKey)
        {
            string serializedContent = _serializer.Serialize(payload);
            return SendAsync(serializedContent, partitionKey);
        }
    }
}
