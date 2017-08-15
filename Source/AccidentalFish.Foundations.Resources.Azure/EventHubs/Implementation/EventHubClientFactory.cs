using Microsoft.Azure.EventHubs;

namespace AccidentalFish.Foundations.Resources.Azure.EventHubs.Implementation
{
    internal class EventHubClientFactory : IEventHubClientFactory
    {
        private readonly IEventHubSerializer _serializer;

        public EventHubClientFactory(IEventHubSerializer serializer)
        {
            _serializer = serializer;
        }

        public IEventHubClient Create(Microsoft.Azure.EventHubs.EventHubClient client)
        {
            return new EventHubClient(client, _serializer);
        }

        public IEventHubClient Create(string connectionString)
        {
            return new EventHubClient(Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(connectionString), _serializer);
        }

        public IEventHubClient Create(string namespaceConnectionString, string eventHubName)
        {
            EventHubsConnectionStringBuilder connectionStringBuilder =
                new EventHubsConnectionStringBuilder(namespaceConnectionString) {EntityPath = eventHubName};
            return new EventHubClient(Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString()), _serializer);
        }
    }
}
