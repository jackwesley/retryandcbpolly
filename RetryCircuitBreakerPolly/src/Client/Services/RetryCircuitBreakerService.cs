using Polly;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Services
{
    public class RetryCircuitBreakerService : IRetryCircuitBreakerService
    {

        public AsyncPolicyWrap CreatePolicyManager()
        {
           return Policy.WrapAsync(WaitAndRetry(), CircuitBreaker());
        }

        public IAsyncPolicy WaitAndRetry()
        {
            return Policy
               .Handle<HttpRequestException>()
               .WaitAndRetryAsync(sleepDurations: new[]
               {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
               }, onRetry: (outcomeType, timespan, retryCount, context) =>
               {
                   Console.ForegroundColor = ConsoleColor.Blue;
                   Console.WriteLine($"Trying for the {retryCount} time!");
                   Console.ForegroundColor = ConsoleColor.White;
               });
        }



        public IAsyncPolicy CircuitBreaker()
        {
            return Policy
              .Handle<Exception>()
              .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 4,
                    durationOfBreak: TimeSpan.FromSeconds(3),
                    onBreak: (ex, breakDelay) =>
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(".Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ConsoleColor.Magenta);
                        Console.WriteLine("..due to: " + ex.Message, ConsoleColor.Magenta);
                    },
                    onReset: () => Console.WriteLine(".Breaker logging: Call ok! Closed the circuit again!", ConsoleColor.Magenta),
                    onHalfOpen: () => Console.WriteLine(".Breaker logging: Half-open: Next call is a trial!", ConsoleColor.Magenta)
                );
        }
    }
}
