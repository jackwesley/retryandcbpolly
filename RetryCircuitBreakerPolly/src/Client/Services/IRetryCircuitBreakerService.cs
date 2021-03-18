using Polly;
using Polly.Wrap;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface IRetryCircuitBreakerService
    {
        public AsyncPolicyWrap CreatePolicyManager();
        public IAsyncPolicy WaitAndRetry();
        public IAsyncPolicy CircuitBreaker();
    }
}
