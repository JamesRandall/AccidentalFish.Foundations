using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Resources.Azure.EventHubs
{
    public interface IEventHubClient
    {
        Task SendAsync(string text, string partitionKey);
        Task SendAsync(string text);
        Task SendAsync(byte[] bytes);
        Task SendAsync(byte[] bytes, string partitionKey);
        Task SendAsync<T>(T payload);
        Task SendAsync<T>(T payload, string partitionKey);
    }
}
