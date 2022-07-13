using Client.Configurations;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Wrap;
using System;

namespace Client.Services
{
    public class RetryCircuitBreakerService : IRetryCircuitBreakerService
    {
        private readonly PollyOptions _pollyOptions;

        public RetryCircuitBreakerService(IOptions<PollyOptions> pollyOptions)
        {
            _pollyOptions = pollyOptions.Value;
        }

        public AsyncPolicyWrap CreatePolicyManager()
        {
           return Policy.WrapAsync(WaitAndRetry(), CircuitBreaker());
        }

        private IAsyncPolicy WaitAndRetry()
        {
            return Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(
                retryCount: _pollyOptions.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(_pollyOptions.RetrySleepInSeconds),
                onRetry: (outcomeType, timespan, retryCount, context) =>
               {
                   Console.ForegroundColor = ConsoleColor.Blue;
                   Console.WriteLine($"Trying for the {retryCount} time!");
                   Console.WriteLine();
                   Console.WriteLine($"Retrying one more time for correlationId '{context.CorrelationId}', after {timespan}. Current retry count {retryCount}");
                   Console.ForegroundColor = ConsoleColor.White;
               });
        }



        private IAsyncPolicy CircuitBreaker()
        {
            return Policy
              .Handle<Exception>()
              .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: _pollyOptions.ExceptionsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromSeconds(_pollyOptions.DurationOfBreakInSeconds),
                    onBreak: (ex, breakDelay) =>
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Circuit breaker opened");
                        Console.WriteLine(".Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ConsoleColor.Magenta);
                        Console.WriteLine("..due to: " + ex.Message, ConsoleColor.Magenta);
                    },
                    onReset: () => Console.WriteLine(".Breaker logging: Call ok! Closed the circuit again!", ConsoleColor.Magenta),
                    onHalfOpen: () => Console.WriteLine(".Breaker logging: Half-open: Next call is a trial!", ConsoleColor.Magenta)
                );
        }
    }
}
