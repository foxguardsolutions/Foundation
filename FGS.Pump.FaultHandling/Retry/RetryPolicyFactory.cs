using System;
using System.Collections.Generic;
using System.Linq;

using FGS.Pump.FaultHandling.Configuration;
using FGS.Pump.Logging;

using Polly;

namespace FGS.Pump.FaultHandling.Retry
{
    internal sealed class RetryPolicyFactory : IRetryPolicyFactory
    {
        private readonly IFaultHandlingConfiguration _configuration;
        private readonly IRetryBackoffCalculator _backoffCalculator;
        private readonly Func<ISyncPolicy, IAsyncPolicy, IRetryPolicy> _wrapPolicies;
        private readonly IStructuralLogger _logger;

        public RetryPolicyFactory(
            IFaultHandlingConfiguration configuration,
            IRetryBackoffCalculator backoffCalculator,
            Func<ISyncPolicy, IAsyncPolicy, IRetryPolicy> wrapPolicies,
            IStructuralLoggerBuilder structuralLoggerBuilder)
        {
            _configuration = configuration;
            _backoffCalculator = backoffCalculator;
            _wrapPolicies = wrapPolicies;
            _logger = structuralLoggerBuilder.ForContext(GetType()).Create();
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
            var retriesRemaining = _configuration.MaxRetries - attempt;
            var backoffTotalSeconds = backoff.TotalSeconds;
            _logger.Debug(exception, "Caught exception, {retriesRemaining} attempts remaining - waiting for {backoffTotalSeconds} seconds before retrying", retriesRemaining, backoffTotalSeconds);
        }
    }
}