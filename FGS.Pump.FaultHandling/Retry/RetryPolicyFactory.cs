using System;
using System.Collections.Generic;
using System.Linq;

using FGS.Pump.FaultHandling.Configuration;
using FGS.Pump.Logging;
using FGS.Pump.Logging.Factories;

using Polly;

namespace FGS.Pump.FaultHandling.Retry
{
    internal sealed class RetryPolicyFactory : IRetryPolicyFactory
    {
        private readonly IFaultHandlingConfiguration _configuration;
        private readonly IRetryBackoffCalculator _backoffCalculator;
        private readonly Func<ISyncPolicy, IAsyncPolicy, IRetryPolicy> _wrapPolicies;
        private readonly ILogger _logger;

        public RetryPolicyFactory(
            IFaultHandlingConfiguration configuration,
            IRetryBackoffCalculator backoffCalculator,
            Func<ISyncPolicy, IAsyncPolicy, IRetryPolicy> wrapPolicies,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _backoffCalculator = backoffCalculator;
            _wrapPolicies = wrapPolicies;
            _logger = loggerFactory.Create(GetType());
        }

        public IRetryPolicy Create(IEnumerable<Func<Exception, bool>> exceptionPredicates)
        {
            if (!exceptionPredicates.Any())
                throw new ArgumentException($"Expected at least one exception predicate, given none, when creating an instance of {nameof(IRetryPolicy)}", nameof(exceptionPredicates));

            var policyBuilder = CreatePolicyBuilder(exceptionPredicates);

            var syncPolicy = policyBuilder.WaitAndRetry(
                retryCount: _configuration.MaxRetries,
                sleepDurationProvider: _backoffCalculator.CalculateBackoff,
                onRetry: LogRetryAttempt);

            var asyncPolicy = policyBuilder.WaitAndRetryAsync(
                retryCount: _configuration.MaxRetries,
                sleepDurationProvider: _backoffCalculator.CalculateBackoff,
                onRetry: LogRetryAttempt);

            return _wrapPolicies(syncPolicy, asyncPolicy);
        }

        private static PolicyBuilder CreatePolicyBuilder(IEnumerable<Func<Exception, bool>> exceptionPredicates)
        {
            var policyBuilder = Policy.Handle(exceptionPredicates.First());

            foreach (var func in exceptionPredicates.Skip(1))
                policyBuilder = policyBuilder.Or(func);

            return policyBuilder;
        }

        private void LogRetryAttempt(Exception exception, TimeSpan backoff, int attempt, Context ctx)
        {
            _logger.Trace($"Caught exception {exception.GetType().Name}. {_configuration.MaxRetries - attempt} attempts remaining… waiting for {backoff.TotalSeconds} seconds and retrying", exception);
        }
    }
}