using System.Threading.Tasks;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.Runtime
{
    interface IConnectionStringProvider
    {
        Task<string> GetAsync<TFactory>(string resourceName);

        string Get<TFactory>(string resourceName);
    }
}
