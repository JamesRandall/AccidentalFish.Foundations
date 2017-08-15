using Microsoft.Azure.EventHubs;

namespace AccidentalFish.Foundations.Resources.Azure.EventHubs
{
    public interface IEventHubClientFactory
    {
        IEventHubClient Create(EventHubClient client);
        IEventHubClient Create(string connectionString);
        IEventHubClient Create(string namespaceConnectionString, string eventHubName);
    }
}
