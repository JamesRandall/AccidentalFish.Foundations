using System.Threading.Tasks;

namespace AccidentalFish.Foundations.Resources.Abstractions
{
    public interface IConnectionStringProvider
    {
        Task<string> GetAsync<TFactory>(string resourceName);

        string Get<TFactory>(string resourceName);
    }
}
