using System;
using System.Collections.Generic;
using System.Linq;

using FGS.FaultHandling.Abstractions;
using FGS.FaultHandling.Abstractions.Retry;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;

namespace FGS.FaultHandling.Polly.Retry
{
    public sealed class RetryPolicyFactory : IRetryPolicyFactory
    {
        private readonly IOptionsSnapshot<FaultHandlingConfiguration> _configuration;
        private readonly IRetryBackoffCalculator _backoffCalculator;
        private readonly Func<ISyncPolicy, IAsyncPolicy, IRetryPolicy> _wrapPolicies;
        private readonly ILogger _logger;

        public RetryPolicyFactory(
            IOptionsSnapshot<FaultHandlingConfiguration> configuration,
            IRetryBackoffCalculator backoffCalculator,
            Func<ISyncPolicy, IAsyncPolicy, IRetryPolicy> wrapPolicies,
            ILogger<RetryPolicyFactory> logger)
        {
            _configuration = configuration;
            _backoffCalculator = backoffCalculator;
            _wrapPolicies = wrapPolicies;
            _logger = logger;
        }

        public IRetryPolicy Create(IEnumerable<Func<Exception, bool>> exceptionPredicates)
        {
            if (!exceptionPredicates.Any())
                throw new ArgumentException($"Expected at least one exception predicate, given none, when creating an instance of {nameof(IRetryPolicy)}", nameof(exceptionPredicates));

            var policyBuilder = CreatePolicyBuilder(exceptionPredicates);

            var syncPolicy = policyBuilder.WaitAndRetry(
                retryCount: _configuration.Value.MaxRetries,
                sleepDurationProvider: _backoffCalculator.CalculateBackoff,
                onRetry: LogRetryAttempt);

            var asyncPolicy = policyBuilder.WaitAndRetryAsync(
                retryCount: _configuration.Value.MaxRetries,
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
            var retriesRemaining = _configuration.Value.MaxRetries - attempt;
            var backoffTotalSeconds = backoff.TotalSeconds;
            _logger.LogDebug(exception, "Caught exception, {retriesRemaining} attempts remaining - waiting for {backoffTotalSeconds} seconds before retrying", retriesRemaining, backoffTotalSeconds);
        }
    }
}
