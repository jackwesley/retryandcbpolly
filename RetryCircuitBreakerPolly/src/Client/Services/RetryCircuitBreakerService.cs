using Client.Configurations;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using System;
using System.Net.Http;

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
            return Policy.WrapAsync(WaitAndRetry2(), CircuitBreaker2());
        }

        private IAsyncPolicy WaitAndRetry()
        {
            var a = Policy
               .Handle<HttpRequestException>(message => AllowRetry(message))
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
            return a;
        }

        private bool AllowRetry(HttpRequestException message)
        {
            if (message?.StatusCode is not null)
                return (int)message.StatusCode >= 500 || (int)message.StatusCode == 409;
            return true;
        }

        private IAsyncPolicy CircuitBreaker()
        {
             var t = Policy
              .Handle<HttpRequestException>()
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

            return t;
        }


        private IAsyncPolicy WaitAndRetry2()
        {
            int retries = 0;
            int eventualFailuresDueToCircuitBreaking = 0;
            int eventualFailuresForOtherReasons = 0;
            var waitAndRetryPolicy = Policy
               .Handle<Exception>(e => !(e is BrokenCircuitException)) // Exception filtering!  We don't retry if the inner circuit-breaker judges the underlying system is out of commission!
               .WaitAndRetryForeverAsync(
               attempt => TimeSpan.FromMilliseconds(200),
               (exception, calculatedWaitDuration) =>
               {
                   Console.WriteLine(".Log,then retry: " + exception.Message, ConsoleColor.Yellow);
                   retries++;
               });

            return waitAndRetryPolicy;
        }


        private IAsyncPolicy CircuitBreaker2()
        {
            // Define our CircuitBreaker policy: Break if the action fails 4 times in a row.
            var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 4,
                durationOfBreak: TimeSpan.FromSeconds(60),
                onBreak: (ex, breakDelay) =>
                {
                    Console.WriteLine(".Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ConsoleColor.Magenta);
                    Console.WriteLine("..due to: " + ex.Message, ConsoleColor.Magenta);
                },
                onReset: () => Console.WriteLine(".Breaker logging: Call ok! Closed the circuit again!", ConsoleColor.Magenta),
                onHalfOpen: () => Console.WriteLine(".Breaker logging: Half-open: Next call is a trial!", ConsoleColor.Magenta)
            );

            return circuitBreakerPolicy;
        }

            
    }
}
